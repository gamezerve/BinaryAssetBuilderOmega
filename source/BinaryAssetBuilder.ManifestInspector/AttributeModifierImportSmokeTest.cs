using System.Buffers.Binary;
using System.Xml;
using System.Xml.Schema;
using BinaryAssetBuilder;
using BinaryAssetBuilder.Core;
using BinaryAssetBuilder.Core.SageXml;
using BinaryAssetBuilder.Core.Session;
using Relo;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: prove actual core reference normalization and one-biased final import encoding against real EP1 modifier slices, without changing experimental eligibility.
internal static class AttributeModifierImportSmokeTest
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: compare deterministic source/default/identity normalization with native imports and optional real-game golden chunks. */
    //-------------------------------------------------------------------------------------------------
    internal static void Run(string[] manifests)
    {
        // Reborn: guard the shared serializer's index boundary and repeatability independently of schema fixtures.
        CheckImportEncoding();
        string fixtures = Path.GetDirectoryName(ReferencePipelineSmokeTest.FindFixture())!;
        string root = Path.Combine(Path.GetTempPath(), "Reborn-Ep1ModifierImports-" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(root);
        Settings saved = Settings.Current;
        try
        {
            Settings.Current = new Settings { BuildCache = false, SchemaPath = Path.Combine(fixtures, "AttributeModifierPipeline.xsd"),
                DataRoot = root, DataPaths = new[] { root }, TargetPlatform = TargetPlatform.Win32,
                CustomPostfix = "", StreamPostfix = "", StringHashBinDescriptors = Array.Empty<StringHashBinDescriptor>() };
            string source = Path.Combine(root, "imports.xml");
            File.WriteAllText(source, File.ReadAllText(Path.Combine(fixtures, "AttributeModifierImportProbe.xml")));
            XmlSchemaSet schemas = new() { XmlResolver = new XmlUrlResolver() };
            schemas.Add(null, Settings.Current.SchemaPath);
            schemas.Compile();
            var first = Build(source, schemas);
            var second = Build(source, schemas);
            Require(first.Count == 4 && second.Count == 4, "Import fixture root count differs.");
            foreach (var entry in first)
            {
                var repeated = second[entry.Key];
                Require(entry.Value.Chunk.InstanceBuffer.SequenceEqual(repeated.Chunk.InstanceBuffer)
                    && entry.Value.Chunk.RelocationBuffer.SequenceEqual(repeated.Chunk.RelocationBuffer)
                    && entry.Value.Chunk.ImportsBuffer.SequenceEqual(repeated.Chunk.ImportsBuffer)
                    && entry.Value.Dependencies.SequenceEqual(repeated.Dependencies), "Repeated full core import build differs.");
                byte[] bin = entry.Value.Chunk.InstanceBuffer, imp = entry.Value.Chunk.ImportsBuffer;
                Require(imp.Length == (entry.Value.Dependencies.Length + 1) * 4 && Read(imp, imp.Length - 4) == uint.MaxValue,
                    "Import count/sentinel differs from normalized dependencies.");
                for (int index = 0; index < imp.Length / 4 - 1; index++)
                {
                    uint slot = Read(imp, index * 4), encoded = Read(bin, (int)slot);
                    Require(encoded > 0 && encoded <= entry.Value.Dependencies.Length, "Final import does not select a one-biased dependency.");
                }
            }
            var suppression = first["AttributeModifier:Modifier_Test_Suppression"];
            Require(suppression.Chunk.InstanceBuffer.Length == 132 && Read(suppression.Chunk.InstanceBuffer, 12) == 1
                && suppression.Dependencies.Single() == new AssetId(0x86682E78u, 0xFDD67044u),
                "First FX dependency did not become runtime import one with the correct identity.");
            var iron = first["AttributeModifier:AttributeModifier_IronCurtain"];
            Require(iron.Chunk.InstanceBuffer.Length == 160 && Read(iron.Chunk.InstanceBuffer, 40) == 148
                && Read(iron.Chunk.InstanceBuffer, 148) == 1 && Read(iron.Chunk.InstanceBuffer, 12) == 2
                && Read(iron.Chunk.InstanceBuffer, 16) == 3 && iron.Chunk.ImportsBuffer.Length == 16,
                "Optional shader pointer or mixed Shader/FX dependency indices differ.");
            foreach (string manifest in manifests) Compare(first, manifest);
            Require(!Directory.EnumerateFiles(root, "*.manifest").Any(), "Import diagnostic emitted production output.");
            Console.WriteLine("Modifier import self-test: OK (full core normalization, zero-based XML/one-biased BIN, Shader pointer, four deterministic native roots, no production profile activation)");
        }
        finally { Settings.Current = saved; }
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: use actual document loader/schema/default/reference stages; the root stays unregistered and writes no native production output. */
    //-------------------------------------------------------------------------------------------------
    private static Dictionary<string, (Chunk Chunk, AssetId[] Dependencies)> Build(string source, XmlSchemaSet schemas)
    {
        SessionCache cache = new();
        cache.InitializeCache(new System.Collections.Generic.List<string>());
        PluginRegistry plugins = new(Array.Empty<PluginDescriptor>(), TargetPlatform.Win32);
        DocumentProcessor processor = new(Settings.Current, plugins, new VerifierPluginRegistry(Array.Empty<PluginDescriptor>(), TargetPlatform.Win32))
            { Cache = cache, SchemaSet = new SchemaSet(false) };
        AssetDeclarationDocument document = processor.ProcessDocumentInternal(source, source, null!,
            new DocumentProcessor.ProcessOptions { GenerateOutput = false, UsePrecompiled = false });
        Dictionary<string, (Chunk, AssetId[])> result = new(StringComparer.Ordinal);
        foreach (InstanceDeclaration instance in document.SelfInstances)
        {
            Require(instance.Handle.TypeHash == 0 && instance.WeakReferencedInstances.Count == 0, "Diagnostic root unexpectedly became registered.");
            XmlDocument copy = new();
            copy.LoadXml(instance.XmlNode.OuterXml);
            copy.DocumentElement!.RemoveAttribute("TypeId");
            foreach (XmlElement child in copy.DocumentElement.ChildNodes.OfType<XmlElement>()) child.RemoveAttribute("TypeId");
            Chunk chunk = AttributeModifierNativeSmokeTest.Compile(schemas, copy.DocumentElement.OuterXml);
            result.Add(instance.Handle.Name, (chunk, instance.ReferencedInstances.Select(handle => new AssetId(handle.TypeId, handle.InstanceId)).ToArray()));
        }
        return result;
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: require exact dependency order/identities and all native chunks, using only bounded reads from supplied linked EP1 streams. */
    //-------------------------------------------------------------------------------------------------
    private static void Compare(IReadOnlyDictionary<string, (Chunk Chunk, AssetId[] Dependencies)> expected, string path)
    {
        ManifestDocument manifest = ManifestReader.Read(File.ReadAllBytes(path));
        TypeRegistryAudit.ValidateTarget(manifest.Header.Version, manifest.Header.AllTypesHash);
        Require(manifest.Header.IsLinked && manifest.Validate().Count == 0, "Invalid linked EP1 manifest.");
        foreach (var stream in new[] { (Extension: ".bin", Magic: 0xBABB0000u), (Extension: ".relo", Magic: 0xBABE0000u), (Extension: ".imp", Magic: 0xBAB10000u) })
        {
            byte[] header = AssetStreamProbe.ReadRange(Path.ChangeExtension(path, stream.Extension), null, 0, 8);
            Require(Read(header, 0) == stream.Magic && Read(header, 4) == manifest.Header.StreamChecksum, "Golden stream target/checksum differs.");
        }
        long bin = manifest.Header.ContainerPrefixSize + 4, relo = bin, imp = bin;
        int matches = 0;
        foreach (ManifestAsset asset in manifest.Assets)
        {
            if (expected.TryGetValue(asset.Name, out var value))
            {
                Require(asset.TypeId == 0xC5E07887u && asset.TypeHash == 0x74425C11u && asset.Tokenized == 0
                    && asset.InstanceDataSize == value.Chunk.InstanceBuffer.Length && asset.RelocationDataSize == value.Chunk.RelocationBuffer.Length
                    && asset.ImportsDataSize == value.Chunk.ImportsBuffer.Length && asset.References.SequenceEqual(value.Dependencies),
                    $"Golden import fingerprint, size or dependency order differs: {asset.Name}");
                Require(value.Dependencies.All(id => manifest.Assets.Any(target => target.TypeId == id.TypeId && target.InstanceId == id.InstanceId)),
                    "A golden dependency target is absent from the supplied stock manifest.");
                Require(value.Chunk.InstanceBuffer.SequenceEqual(AssetStreamProbe.ReadRange(Path.ChangeExtension(path, ".bin"), null, bin, asset.InstanceDataSize))
                    && value.Chunk.RelocationBuffer.SequenceEqual(AssetStreamProbe.ReadRange(Path.ChangeExtension(path, ".relo"), null, relo, asset.RelocationDataSize))
                    && value.Chunk.ImportsBuffer.SequenceEqual(AssetStreamProbe.ReadRange(Path.ChangeExtension(path, ".imp"), null, imp, asset.ImportsDataSize)),
                    $"Golden native import payload differs: {asset.Name}");
                Console.WriteLine($"  Real EP1 import golden: {asset.Name} exact {asset.InstanceDataSize}/{asset.RelocationDataSize}/{asset.ImportsDataSize}; {asset.References.Count} ordered dependencies");
                matches++;
            }
            bin = checked(bin + asset.InstanceDataSize); relo = checked(relo + asset.RelocationDataSize); imp = checked(imp + asset.ImportsDataSize);
        }
        Require(matches == expected.Count, "Supplied manifest lacks one or more supported modifier import goldens.");
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: decode bounded little-endian runtime import words without interpreting them as asset hashes. */
    //-------------------------------------------------------------------------------------------------
    private static uint Read(byte[] data, int offset) => BinaryPrimitives.ReadUInt32LittleEndian(data.AsSpan(offset, 4));

    //-------------------------------------------------------------------------------------------------
    /** Reborn: null stays zero, imported index zero becomes one, the largest representable index does not wrap, and repeated serialization does not double-bias. */
    //-------------------------------------------------------------------------------------------------
    private static unsafe void CheckImportEncoding()
    {
        uint* root;
        using Tracker tracker = new((void**)&root, 16, false);
        tracker.AddReference(root, 0);
        tracker.AddReference(root + 1, 42);
        tracker.AddReference(root + 2, uint.MaxValue - 1);
        Chunk first = new(), repeated = new();
        Require(tracker.MakeRelocatable(first) && tracker.MakeRelocatable(repeated), "Tracker rejected bounded imports.");
        Require(Read(first.InstanceBuffer, 0) == 1 && Read(first.InstanceBuffer, 4) == 43
            && Read(first.InstanceBuffer, 8) == uint.MaxValue && Read(first.InstanceBuffer, 12) == 0
            && first.InstanceBuffer.SequenceEqual(repeated.InstanceBuffer)
            && first.ImportsBuffer.SequenceEqual(repeated.ImportsBuffer)
            && *root == 0 && root[1] == 42 && root[2] == uint.MaxValue - 1,
            "Import bias changed nulls, source bookmarks, boundaries or repeated serialization.");
        uint* invalid;
        using Tracker overflow = new((void**)&invalid, 4, false);
        overflow.AddReference(invalid, uint.MaxValue);
        try { overflow.MakeRelocatable(new Chunk()); }
        catch (OverflowException) { return; }
        throw new InvalidDataException("Unrepresentable import index wrapped to runtime null.");
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: fail at the first dependency-order or native-byte mismatch, keeping production eligibility closed. */
    //-------------------------------------------------------------------------------------------------
    private static void Require(bool condition, string message)
    {
        if (!condition) throw new InvalidDataException(message);
    }
}
