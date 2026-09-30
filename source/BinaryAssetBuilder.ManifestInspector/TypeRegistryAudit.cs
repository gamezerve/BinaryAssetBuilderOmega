using System.Reflection;
using System.Security.Cryptography;
using System.Text.Json;
using BinaryAssetBuilder.Core;
using BinaryAssetBuilder.Core.Hashing;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: observed game identities are evidence, not authorization to activate unverified native layouts.
internal static class TypeRegistryAudit
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: join EP1 manifest fingerprints with the actual plugin registry and typed marshal inventory. */
    //-------------------------------------------------------------------------------------------------
    public static void Print(string[] paths, bool json)
    {
        if (paths.Length == 0) throw new ArgumentException("At least one EP1 manifest is required.");
        // Reborn: fingerprint exactly the metadata bytes parsed, never the adjacent multi-gigabyte BIN.
        var inputs = paths.Select(Path.GetFullPath).Distinct(StringComparer.OrdinalIgnoreCase)
            .Select(path =>
            {
                var bytes = File.ReadAllBytes(path);
                return (Path: path, Sha256: Convert.ToHexString(SHA256.HashData(bytes)), Document: ManifestReader.Read(bytes));
            }).ToArray();
        foreach (var input in inputs)
        {
            ValidateTarget(input.Document.Header.Version, input.Document.Header.AllTypesHash);
            var errors = input.Document.Validate();
            if (errors.Count != 0) throw new InvalidDataException($"{input.Path}: {string.Join("; ", errors)}");
        }

        // Reborn: snapshot and restore shared registry state even when the audit is called in-process.
        var plugin = new BinaryAssetBuilder.XmlCompiler.Plugin();
        var registry = (IDictionary<uint, ExtendedTypeInformation>)typeof(BinaryAssetBuilder.XmlCompiler.Plugin)
            .GetField("_extendedTypeInformations", BindingFlags.NonPublic | BindingFlags.Static)!.GetValue(null)!;
        var saved = registry.ToArray();
        Dictionary<uint, ExtendedTypeInformation> active;
        try
        {
            plugin.ReInitialize(TargetPlatform.Win32);
            active = registry.ToDictionary(pair => pair.Key, pair => pair.Value);
            var fallback = plugin.GetExtendedTypeInformation(0x11E5CF64u);
            if (fallback != null) active[fallback.TypeId] = fallback;
        }
        finally
        {
            registry.Clear();
            foreach (var pair in saved) registry.Add(pair.Key, pair.Value);
        }

        var models = typeof(SageBinaryData.GameObject).Assembly.GetTypes()
            .Where(type => type.Namespace == "SageBinaryData" && !type.IsNested).ToDictionary(type => type.Name);
        var marshalled = typeof(Marshaler).GetMethods(BindingFlags.Public | BindingFlags.Static)
            .Where(method => method.Name == "Marshal")
            .SelectMany(method => method.GetParameters().Where(parameter => parameter.ParameterType.IsPointer)
                .Select(parameter => parameter.ParameterType.GetElementType()!.Name)).ToHashSet();
        var rows = inputs.SelectMany(input => input.Document.Assets.Select(asset => (input.Path, Asset: asset)))
            .GroupBy(value => value.Asset.TypeId).OrderBy(group => group.First().Asset.TypeName, StringComparer.Ordinal)
            .Select(group =>
            {
                var fingerprints = group.Select(value => (value.Asset.TypeName, value.Asset.TypeHash, value.Asset.Tokenized)).Distinct().ToArray();
                var sample = group.First().Asset;
                active.TryGetValue(group.Key, out var registered);
                var status = Classify(fingerprints.Length, FastHash.GetHashCode(sample.TypeName) == group.Key,
                    registered != null, registered?.TypeHash == sample.TypeHash);
                return new
                {
                    sample.TypeName, TypeId = $"0x{group.Key:X8}", Status = status,
                    Fingerprints = fingerprints.Select(value => new { value.TypeName, TypeHash = $"0x{value.TypeHash:X8}", value.Tokenized }).ToArray(),
                    CompilerTypeHash = registered == null ? null : $"0x{registered.TypeHash:X8}",
                    HasModel = models.ContainsKey(sample.TypeName), HasTypedMarshaller = marshalled.Contains(sample.TypeName),
                    AssetCount = group.Count(), Sources = group.Select(value => value.Path).Distinct().Order(StringComparer.Ordinal).ToArray()
                };
            }).ToArray();
        var report = new
        {
            Target = "RA3 Uprising EP1", ExpectedAllTypesHash = "0x5454A8E9",
            CompilerAllTypesHash = $"0x{plugin.AllTypesHash:X8}",
            ProductionGatePassed = plugin.AllTypesHash == 0x5454A8E9u,
            Limitations = "Observed streams are not a complete type table. Matching hashes/models/marshallers do not prove native layout, processor or runtime compatibility.",
            ManifestCount = inputs.Length, ObservedTypeCount = rows.Length, RegisteredTypeCount = active.Count,
            Inputs = inputs.Select(input => new { input.Path, input.Sha256, AssetCount = input.Document.Assets.Count }).ToArray(),
            StatusCounts = rows.GroupBy(row => row.Status).ToDictionary(group => group.Key, group => group.Count()), Types = rows
        };
        if (json) Console.WriteLine(JsonSerializer.Serialize(report, new JsonSerializerOptions { WriteIndented = true }));
        else
        {
            Console.WriteLine($"EP1 observed types={rows.Length}; compiler registered={active.Count}; AllTypesHash={report.CompilerAllTypesHash}; production gate={report.ProductionGatePassed}");
            foreach (var row in rows) Console.WriteLine($"{row.Status,-20} {row.TypeName,-40} {row.TypeId} model={row.HasModel} marshal={row.HasTypedMarshaller} compiler={row.CompilerTypeHash ?? "-"}");
            Console.WriteLine(report.Limitations);
        }
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: reject other games before treating their fingerprints as EP1 migration evidence. */
    //-------------------------------------------------------------------------------------------------
    internal static void ValidateTarget(ushort version, uint allTypesHash)
    {
        if (version != 7 || allTypesHash != 0x5454A8E9u)
            throw new InvalidDataException("Type audit requires EP1 v7 / AllTypesHash 0x5454A8E9 manifests.");
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: conflicts take precedence over registry matches so mixed inputs cannot produce false readiness. */
    //-------------------------------------------------------------------------------------------------
    internal static string Classify(int fingerprintCount, bool identityMatches, bool registered, bool hashMatches)
    {
        if (fingerprintCount != 1) return "conflicting-evidence";
        if (!identityMatches) return "identity-mismatch";
        if (!registered) return "unregistered";
        return hashMatches ? "hash-match-only" : "type-hash-mismatch";
    }
}
