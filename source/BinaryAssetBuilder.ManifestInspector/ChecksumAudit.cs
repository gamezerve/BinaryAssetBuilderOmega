using System.Buffers.Binary;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Xml;
using BinaryAssetBuilder.Core;
using BinaryAssetBuilder.Core.Hashing;
using BinaryAssetBuilder.Core.SageXml;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: distinguish official identity checksums and patch equivalence from compiled-payload integrity.
internal static class ChecksumAudit
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: reconstruct checksum candidates from manifest metadata only, retaining mismatches as evidence rather than guessed fixes. */
    //-------------------------------------------------------------------------------------------------
    internal static void Print(string[] paths)
    {
        List<object> results = new();
        foreach (string input in paths)
        {
            string path = Path.GetFullPath(input);
            // Reborn: reject accidental game BIN/BIG dumps and cap this metadata-only diagnostic at 16 MiB per manifest.
            Require(Path.GetExtension(path).Equals(".manifest", StringComparison.OrdinalIgnoreCase)
                && new FileInfo(path).Length <= 16 * 1024 * 1024, "Checksum audit accepts only .manifest metadata up to 16 MiB.");
            byte[] bytes = File.ReadAllBytes(path);
            ManifestDocument manifest = ManifestReader.Read(bytes);
            Require(manifest.Header.Version == 7 && manifest.Header.AllTypesHash == 0x5454A8E9u
                && manifest.Validate().Count == 0, "Checksum audit requires valid EP1 v7 metadata.");
            using MemoryStream stream = new();
            using BinaryWriter writer = new(stream, Encoding.UTF8, true);
            foreach (ManifestAsset asset in manifest.Assets)
            {
                writer.Write(asset.TypeId);
                writer.Write(asset.TypeHash);
                writer.Write(asset.InstanceId);
                writer.Write(asset.InstanceHash);
                writer.Write(asset.AssetReferenceCount);
            }
            uint padded = stream.Length == 0 ? 0 : FastHash.GetHashCode(stream.GetBuffer());
            uint logical = stream.Length == 0 ? 0 : FastHash.GetHashCode(stream.ToArray());
            results.Add(new { Path = path, Sha256 = Convert.ToHexString(SHA256.HashData(bytes)),
                Assets = manifest.Assets.Count, LogicalBytes = stream.Length, CapacityBytes = stream.Capacity,
                StoredChecksum = $"{manifest.Header.StreamChecksum:X8}", PaddedCandidate = $"{padded:X8}",
                LogicalCandidate = $"{logical:X8}", PaddedMatch = padded == manifest.Header.StreamChecksum,
                LogicalMatch = logical == manifest.Header.StreamChecksum,
                IsPatch = manifest.ReferencedManifests.Any(reference => reference.IsPatch) });
        }
        Console.WriteLine(JsonSerializer.Serialize(results, new JsonSerializerOptions { WriteIndented = true }));
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: lock production checksum packing/capacity behavior and the official tokenized patch-equivalence exception. */
    //-------------------------------------------------------------------------------------------------
    internal static void Run()
    {
        MethodInfo compute = typeof(AssetDeclarationDocument).GetMethod("ComputeOutputChecksum", BindingFlags.NonPublic | BindingFlags.Static)!;
        foreach (int count in new[] { 0, 1, 12, 13, 25, 26, 51, 52 })
        {
            InstanceDeclaration[] instances = Enumerable.Range(0, count).Select(Declare).ToArray();
            int length = count * 20;
            int capacity = length == 0 ? 0 : 256;
            while (capacity < length) capacity *= 2;
            byte[] packed = new byte[capacity];
            for (int i = 0; i < count; i++)
            {
                InstanceHandle handle = instances[i].Handle;
                uint[] fields = { handle.TypeId, handle.TypeHash, handle.InstanceId, handle.InstanceHash, (uint)instances[i].ReferencedInstances.Count };
                for (int field = 0; field < fields.Length; field++)
                    BinaryPrimitives.WriteUInt32LittleEndian(packed.AsSpan(i * 20 + field * 4), fields[field]);
            }
            uint expected = count == 0 ? 0 : FastHash.GetHashCode(packed);
            Require(Compute(compute, instances) == expected, $"Checksum differs at {count} identities / {capacity} capacity bytes.");
            if (count != 0)
                Require(expected != FastHash.GetHashCode(packed.Take(length).ToArray()), "Fixture failed to distinguish padded and logical checksum candidates.");
        }
        InstanceDeclaration[] pair = { Declare(0), Declare(1) };
        uint baseline = Compute(compute, pair);
        Require(Compute(compute, pair.Reverse().ToArray()) != baseline, "Output ordering did not affect checksum.");
        uint original = pair[0].Handle.TypeHash;
        pair[0].Handle.TypeHash ^= 1;
        Require(Compute(compute, pair) != baseline, "Type hash did not affect checksum.");
        pair[0].Handle.TypeHash = original;
        original = pair[0].Handle.InstanceHash;
        pair[0].Handle.InstanceHash ^= 1;
        Require(Compute(compute, pair) != baseline, "Instance hash did not affect checksum.");
        pair[0].Handle.InstanceHash = original;
        pair[0].ReferencedInstances.Add(new InstanceHandle("ArmorTemplate", "ReferenceOne"));
        Require(Compute(compute, pair) != baseline, "Reference count did not affect checksum.");
        uint oneReference = Compute(compute, pair);
        pair[0].ReferencedInstances[0] = new InstanceHandle("GameObject", "DifferentReference");
        Require(Compute(compute, pair) == oneReference, "Checksum unexpectedly included reference identities instead of only their count.");
        TestPatchEquivalence();
        TestIntermediateCopy();
        Console.WriteLine("Checksum/cache audit self-test: OK (8 capacity boundaries, output order, identity/count sensitivity, reference-identity limitation, patch equivalence matrix, strict intermediate copy)");
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: generate declarations with synthetic fixed hashes without claiming game-compatible compiler output. */
    //-------------------------------------------------------------------------------------------------
    private static InstanceDeclaration Declare(int index)
    {
        XmlDocument xml = new();
        xml.LoadXml($"<ArmorTemplate xmlns=\"uri:ea.com:eala:asset\" id=\"Checksum{index}\" />");
        InstanceDeclaration instance = new(new AssetDeclarationDocument()) { XmlNode = xml.DocumentElement! };
        instance.Handle.TypeHash = 0xA0E237D8u;
        instance.Handle.InstanceHash = (uint)(0x12340000 + index);
        return instance;
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: invoke the exact helper used by PrepareOutputInstances rather than a separate production-like implementation. */
    //-------------------------------------------------------------------------------------------------
    private static uint Compute(MethodInfo method, InstanceDeclaration[] instances)
    {
        return (uint)method.Invoke(null, new object[] { instances })!;
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: preserve the official patch predicate while proving it is not the strict intermediate binary-cache rule. */
    //-------------------------------------------------------------------------------------------------
    private static void TestPatchEquivalence()
    {
        string directory = Path.Combine(Path.GetTempPath(), "Reborn-Ep1Equivalence-" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(directory);
        string path = Path.Combine(directory, "equivalence.manifest");
        byte[] names = Encoding.UTF8.GetBytes("ArmorTemplate:Raw\0ArmorTemplate:Tokens\0");
        using (Stream stream = File.Create(path))
        {
            using BinaryAssetBuilder.Utility.ManifestHeader header = new() { IsLinked = true, AllTypesHash = 0x5454A8E9u,
                AssetCount = 2, AssetNameBufferSize = (uint)names.Length, SourceFileNameBufferSize = 1 };
            header.SaveToStream(stream, false);
            for (int i = 0; i < 2; i++)
            {
                using BinaryAssetBuilder.Utility.AssetEntry entry = new() { TypeId = 0x3A6C5E8Eu, TypeHash = 0xA0E237D8u,
                    InstanceId = (uint)(100 + i), InstanceHash = 0x11223344u, Tokenized = i == 1,
                    NameOffset = i == 0 ? 0 : Encoding.UTF8.GetByteCount("ArmorTemplate:Raw\0") };
                entry.SaveToStream(stream, false);
            }
            stream.Write(names);
            stream.WriteByte(0);
        }
        using BinaryAssetBuilder.Utility.Manifest manifest = new();
        Require(manifest.Load(path, false), "Equivalence metadata fixture did not load.");
        foreach (BinaryAssetBuilder.Utility.Asset asset in manifest.Assets)
        {
            using BinaryAssetBuilder.Utility.AssetEntry entry = new() { TypeId = asset.TypeId, TypeHash = asset.TypeHash,
                InstanceId = asset.InstanceId, InstanceHash = asset.InstanceHash, Tokenized = !asset.Tokenized };
            Require(BinaryAssetBuilder.Utility.Manifest.IsEquivalent(entry, asset), "Identical identity tuple failed patch equivalence.");
            entry.TypeHash ^= 1;
            Require(BinaryAssetBuilder.Utility.Manifest.IsEquivalent(entry, asset) == asset.Tokenized,
                "Type-hash exception did not follow the stored base asset's tokenization flag.");
            entry.TypeHash = asset.TypeHash;
            entry.InstanceHash ^= 1;
            Require(!BinaryAssetBuilder.Utility.Manifest.IsEquivalent(entry, asset), "Tokenization bypassed instance hash.");
            entry.InstanceHash = asset.InstanceHash;
            entry.InstanceId ^= 1;
            Require(!BinaryAssetBuilder.Utility.Manifest.IsEquivalent(entry, asset), "Tokenization bypassed instance ID.");
            entry.InstanceId = asset.InstanceId;
            entry.TypeId ^= 1;
            Require(!BinaryAssetBuilder.Utility.Manifest.IsEquivalent(entry, asset), "Tokenization bypassed type ID.");
        }
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: exercise the real local/cache copy seam to prove tokenized patch matching cannot relax intermediate identity checks. */
    //-------------------------------------------------------------------------------------------------
    private static void TestIntermediateCopy()
    {
        Settings previous = Settings.Current;
        try
        {
            string directory = Path.Combine(Path.GetTempPath(), "Reborn-Ep1StrictCopy-" + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(directory);
            Settings.Current = new Settings { BuildCache = false, TargetPlatform = TargetPlatform.Win32, CustomPostfix = "", BigEndian = false };
            PluginRegistry registry = new(Array.Empty<PluginDescriptor>(), TargetPlatform.Win32);
            DocumentProcessor processor = new(Settings.Current, registry, null!);
            MethodInfo copy = typeof(BinaryAsset).GetMethod("CopyAsset", BindingFlags.NonPublic | BindingFlags.Instance)!;
            for (int variant = 0; variant < 5; variant++)
            {
                using OutputManager manager = new(processor, null, Path.Combine(directory, "output" + variant),
                    Path.Combine(directory, "intermediate" + variant), null!, null!, Array.Empty<string>());
                InstanceDeclaration declaration = Declare(variant);
                BinaryAsset asset = manager.GetBinaryAsset(declaration, true);
                string source = Path.Combine(directory, "candidate" + variant + ".asset");
                using (Stream stream = File.Create(source))
                {
                    BinaryAssetBuilder.Utility.AssetHeader header = new() { TypeId = declaration.Handle.TypeId,
                        TypeHash = declaration.Handle.TypeHash, InstanceId = declaration.Handle.InstanceId,
                        InstanceHash = declaration.Handle.InstanceHash, InstanceDataSize = 4 };
                    if (variant == 1) header.TypeId ^= 1;
                    if (variant == 2) header.TypeHash ^= 1;
                    if (variant == 3) header.InstanceId ^= 1;
                    if (variant == 4) header.InstanceHash ^= 1;
                    header.SaveToStream(stream, false);
                    stream.Write(new byte[] { 10, 20, 30, 40 });
                }
                bool accepted = (bool)copy.Invoke(asset, new object[] { source, "unused-no-custom-data" })!;
                Require(accepted == (variant == 0), $"Strict intermediate copy accepted incorrect identity field {variant}.");
                string destination = Path.Combine(asset.AssetOutputDirectory, asset.AssetFileName);
                if (accepted) Require(File.ReadAllBytes(destination).SequenceEqual(File.ReadAllBytes(source)), "Accepted intermediate copy changed bytes.");
                else Require(!File.Exists(destination), "Rejected candidate was published as an intermediate asset.");
            }
        }
        finally { Settings.Current = previous; }
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: stop checksum/equivalence evidence at the first contract mismatch. */
    //-------------------------------------------------------------------------------------------------
    private static void Require(bool condition, string message)
    {
        if (!condition) throw new InvalidDataException(message);
    }
}
