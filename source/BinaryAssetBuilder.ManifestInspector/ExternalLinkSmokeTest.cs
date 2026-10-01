using System.Reflection;
using System.Runtime.ExceptionServices;
using System.Text;
using System.Xml;
using BinaryAssetBuilder.Core;
using BinaryAssetBuilder.Core.SageXml;
using BinaryAssetBuilder.Utility;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: validate external runtime references, patch roles and target-aware manifest refresh without BIN reads.
internal static class ExternalLinkSmokeTest
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: exercise production writer/lookup seams using small metadata fixtures and optional real EP1 streams. */
    //-------------------------------------------------------------------------------------------------
    internal static void Run(string outputDirectory, params string[] realManifestPaths)
    {
        string[]? previousFiles = Settings.Current.ProcessedExternalManifests;
        string? previousNames = Settings.Current.ExternalManifestReferences;
        int previousErrorLevel = Settings.Current.ErrorLevel;
        // Reborn: metadata-only tests have no network build-cache directory and must restore the caller's setting.
        bool previousBuildCache = Settings.Current.BuildCache;
        try
        {
            Settings.Current.BuildCache = false;
            Directory.CreateDirectory(outputDirectory);
            outputDirectory = Path.GetFullPath(outputDirectory);
            string basePath = Path.Combine(outputDirectory, "base.manifest");
            string patchPath = Path.Combine(outputDirectory, "patch.manifest");
            InstanceHandle first = new("FXList", "ExternalOne");
            InstanceHandle second = new("FXList", "ExternalTwo");
            WriteFixture(basePath, first, new ReferencedFileBuffer());
            // Reborn: test the actual OutputManager base-retention path without invoking a compiler plugin.
            using (OutputManager manager = new(null!, null, Path.Combine(outputDirectory, "manager-" + Guid.NewGuid().ToString("N")),
                Path.Combine(outputDirectory, "intermediate"), basePath, string.Empty, new[] { outputDirectory }))
            {
                Require(manager.BasePatchStreamManifest is not null && manager.BasePatchStreamAssets.Count == 1,
                    "OutputManager did not retain the loaded patch base metadata.");
                using AssetEntry expected = new() { TypeId = first.TypeId, InstanceId = first.InstanceId, TypeHash = 0x11223344u, InstanceHash = 0x55667788u, Tokenized = true };
                Require(manager.BasePatchStreamManifest!.GetBaseStreamPosition(expected) == 0, "OutputManager lost the base asset's stream position.");
            }

            ReferencedFileBuffer references = new();
            AddRuntimeReferences(references, new[] { basePath, "lookup-static.manifest" }, " GLOBAL.manifest ;Data/Static.manifest");
            int normal = references.AddReference("base.manifest", false);
            int patch = references.AddReference("base.manifest", true);
            Require(normal != patch && references.AddReference("base.manifest", true) == patch, "Normal/patch identity or exact-entry deduplication failed.");
            int before = references.Length;
            ExpectInvalidMapping(references, new[] { basePath }, null);
            ExpectInvalidMapping(references, new[] { basePath }, "one.manifest;two.manifest");
            foreach (string invalid in new[] { "D:\\local\\global.manifest", "../global.manifest", "bad//global.manifest", "global.bin", "bad\0.manifest" })
            {
                ExpectInvalidMapping(references, new[] { basePath }, invalid);
            }
            Require(references.Length == before, "Invalid runtime mapping mutated the reference buffer.");
            WriteFixture(patchPath, first, references);
            ManifestDocument parsed = ManifestReader.Read(File.ReadAllBytes(patchPath));
            Require(parsed.Validate().Count == 0 && parsed.ReferencedManifests.SequenceEqual(new[]
            {
                new ReferencedManifest("global.manifest", false), new ReferencedManifest("data\\static.manifest", false),
                new ReferencedManifest("base.manifest", false), new ReferencedManifest("base.manifest", true)
            }), "Serialized external/patch roles did not round-trip through the independent reader.");
            using (BinaryAssetBuilder.Utility.Manifest utility = new())
            {
                Require(utility.Load(patchPath, false) && utility.PatchManifest is not null, "Utility reader failed to load the patch base.");
                Require(utility.Assets.Single().SourceManifest.FileName == basePath, "Zero-size patch entry did not inherit its base asset metadata.");
                // Reborn: disposing the patch now also disposes its owned base metadata.
            }
            CheckSettingsRoundTrip();

            Settings.Current.ErrorLevel = 1;
            Settings.Current.ProcessedExternalManifests = new[] { basePath };
            Require(Contains(first) && !Contains(second), "External lookup did not select the first fixture.");
            DateTime oldWriteTime = File.GetLastWriteTimeUtc(basePath);
            WriteFixture(basePath, second, new ReferencedFileBuffer());
            File.SetLastWriteTimeUtc(basePath, oldWriteTime.AddSeconds(2));
            Require(!Contains(first) && Contains(second), "Same-path external manifest refresh kept stale asset IDs.");

            string wrongTarget = Path.Combine(outputDirectory, "wrong-target.manifest");
            WriteFixture(wrongTarget, first, new ReferencedFileBuffer(), 0x54EEE764u);
            // Reborn: wrong-target patch bases must fail even when the outer manifest is a valid EP1 header.
            ReferencedFileBuffer wrongBaseReference = new();
            wrongBaseReference.AddReference("wrong-target.manifest", true);
            string wrongPatch = Path.Combine(outputDirectory, "wrong-patch.manifest");
            WriteFixture(wrongPatch, first, wrongBaseReference);
            using (BinaryAssetBuilder.Utility.Manifest wrong = new())
            {
                bool rejected = false;
                try { wrong.Load(wrongPatch, false); }
                catch (InvalidDataException) { rejected = true; }
                Require(rejected, "Patch inherited metadata from a different target.");
            }
            Settings.Current.ProcessedExternalManifests = new[] { wrongTarget };
            ExpectLookupFailure(first, ErrorCode.ReferencingError);
            Settings.Current.ProcessedExternalManifests = new[] { Path.Combine(outputDirectory, "missing.manifest") };
            ExpectLookupFailure(first, ErrorCode.FileNotFound);
            Settings.Current.ProcessedExternalManifests = new[] { basePath };
            Require(Contains(second) && !Contains(first), "Failed external reload poisoned the last valid identity index.");

            if (realManifestPaths.Length != 0)
            {
                Settings.Current.ProcessedExternalManifests = realManifestPaths.Select(Path.GetFullPath).ToArray();
                foreach (InstanceHandle target in new[]
                {
                    new InstanceHandle("ObjectFilterAsset", "InfiltrationCanEnterObjectFilter"),
                    new InstanceHandle("EvaEvent", "EnemyBuildingInfiltrated"),
                    new InstanceHandle("EvaEvent", "OurBuildingInfiltrated"),
                    new InstanceHandle("FXList", "FX_Building_Infiltrated_Generic")
                })
                {
                    Require(Contains(target), $"Production external lookup missed real EP1 asset {target.Name}.");
                    Console.WriteLine($"  Production external lookup: {target.Name}");
                }
            }
            Console.WriteLine($"External link self-test: OK (runtime mapping, roles, patch metadata, refresh, target gate, settings); fixtures={outputDirectory}");
        }
        finally
        {
            Settings.Current.ProcessedExternalManifests = previousFiles;
            Settings.Current.ExternalManifestReferences = previousNames;
            Settings.Current.ErrorLevel = previousErrorLevel;
            Settings.Current.BuildCache = previousBuildCache;
        }
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: call the exact production external-reference writer rather than a test-specific serializer. */
    //-------------------------------------------------------------------------------------------------
    private static void AddRuntimeReferences(ReferencedFileBuffer buffer, string[] files, string? names)
    {
        MethodInfo method = typeof(OutputManager).GetMethod("AddExternalManifestReferences", BindingFlags.NonPublic | BindingFlags.Static)!;
        Invoke(method, buffer, files, names);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: fail closed on absent, count-mismatched or unsafe runtime mappings. */
    //-------------------------------------------------------------------------------------------------
    private static void ExpectInvalidMapping(ReferencedFileBuffer buffer, string[] files, string? names)
    {
        try { AddRuntimeReferences(buffer, files, names); }
        catch (BinaryAssetBuilderException exception) when (exception.ErrorCode == ErrorCode.InvalidArgument) { return; }
        throw new InvalidDataException("Invalid external runtime mapping was accepted.");
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: exercise the real manifest identity index, including same-file replacement and EP1 target rejection. */
    //-------------------------------------------------------------------------------------------------
    internal static bool Contains(InstanceHandle target)
    {
        MethodInfo method = typeof(AssetDeclarationDocument).GetMethod("ManifestContainsAsset", BindingFlags.NonPublic | BindingFlags.Static)!;
        return (bool)Invoke(method, target.TypeId, target.InstanceId)!;
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: assert that missing/wrong-target manifests cannot masquerade as successfully resolved dependencies. */
    //-------------------------------------------------------------------------------------------------
    private static void ExpectLookupFailure(InstanceHandle target, ErrorCode expected)
    {
        try { Contains(target); }
        catch (BinaryAssetBuilderException exception) when (exception.ErrorCode == expected) { return; }
        throw new InvalidDataException($"Expected external lookup failure {expected}.");
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: keep runtime mapping intact when settings are saved and reloaded as XML. */
    //-------------------------------------------------------------------------------------------------
    private static void CheckSettingsRoundTrip()
    {
        Settings original = new() { ExternalManifests = "local/global.manifest;local/static.manifest", ExternalManifestReferences = "global.manifest;static.manifest" };
        StringBuilder xml = new();
        using (XmlWriter writer = XmlWriter.Create(xml, new XmlWriterSettings { OmitXmlDeclaration = true }))
        {
            // Reborn: mirror the namespace expected by the production Settings XML reader.
            writer.WriteStartElement("Settings", SchemaSet.XmlNamespace);
            original.WriteXml(writer);
            writer.WriteEndElement();
        }
        XmlDocument document = new();
        document.LoadXml(xml.ToString());
        Settings restored = new();
        XmlNamespaceManager namespaces = new(document.NameTable);
        namespaces.AddNamespace("ea", SchemaSet.XmlNamespace);
        restored.ReadXml(new Node(document.DocumentElement!.CreateNavigator()!, namespaces));
        Require(restored.ExternalManifests == original.ExternalManifests && restored.ExternalManifestReferences == original.ExternalManifestReferences, "Settings lost the local/runtime mapping.");
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: generate a tiny v7 metadata fixture; no production type-table gate or game-load claim is bypassed. */
    //-------------------------------------------------------------------------------------------------
    internal static void WriteFixture(string path, InstanceHandle handle, ReferencedFileBuffer references, uint allTypesHash = 0x5454A8E9u)
    {
        byte[] name = Encoding.ASCII.GetBytes(handle.Name + '\0');
        byte[] source = Encoding.ASCII.GetBytes("Tests/ExternalLink.xml\0");
        using MemoryStream bytes = new();
        using (BinaryAssetBuilder.Utility.ManifestHeader header = new()
        {
            IsLinked = true, AllTypesHash = allTypesHash, AssetCount = 1,
            ReferenceManifestNameBufferSize = (uint)references.Length,
            AssetNameBufferSize = (uint)name.Length, SourceFileNameBufferSize = (uint)source.Length
        }) { header.SaveToStream(bytes, false); }
        using (AssetEntry entry = new()
        {
            TypeId = handle.TypeId, InstanceId = handle.InstanceId, TypeHash = 0x11223344u,
            InstanceHash = 0x55667788u, Tokenized = true
        }) { entry.SaveToStream(bytes, false); }
        references.SaveToStream(bytes);
        bytes.Write(name);
        bytes.Write(source);
        File.WriteAllBytes(path, bytes.ToArray());
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: preserve production error categories across private test seams. */
    //-------------------------------------------------------------------------------------------------
    private static object? Invoke(MethodInfo method, params object?[] arguments)
    {
        try { return method.Invoke(null, arguments); }
        catch (TargetInvocationException exception) when (exception.InnerException is not null)
        {
            ExceptionDispatchInfo.Capture(exception.InnerException).Throw();
            throw;
        }
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: surface stream-link regression failures clearly. */
    //-------------------------------------------------------------------------------------------------
    private static void Require(bool condition, string message)
    {
        if (!condition) { throw new InvalidDataException(message); }
    }
}
