using System.Buffers.Binary;
using System.Xml;
using System.Xml.Schema;
using BinaryAssetBuilder;
using BinaryAssetBuilder.Core;
using BinaryAssetBuilder.Core.SageXml;
using BinaryAssetBuilder.Core.Session;
using BinaryAssetBuilder.XmlCompiler;
using Relo;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: prove core normalization and explicitly gated import compilation against real EP1 modifier slices, without production eligibility.
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
            Console.WriteLine("Modifier import self-test: OK (full core normalization, explicit import compiler entry, identity/index tamper rejection, four deterministic native roots; default/production/cache gates unchanged)");
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
        // Reborn: only direct diagnostic construction admits imports; descriptor-created profiles keep v1 restrictions.
        Ra3Ep1AttributeModifierPlugin imported = new(true), restricted = new();
        imported.Initialize(TargetPlatform.Win32);
        restricted.Initialize(TargetPlatform.Win32);
        Require(imported.VersionNumber == 2 && imported.ProfileName == "RA3EP1-AttributeModifier-Imports-Experimental-v2"
            && !imported.CanWriteProductionOutput && !imported.CanUseBuildCache && !imported.CanReuseCompiledDocuments
            && imported.GetExtendedTypeInformation(0xC5E07887u).ProcessingHash == (0x74425C11u ^ 0x45503112u),
            "Import opt-in relaxed policies or reused the no-import processing domain.");
        PluginRegistry importRegistry = new(Array.Empty<PluginDescriptor>(), TargetPlatform.Win32) { DefaultPlugin = imported };
        Require(!importRegistry.CanReuseCompiledDocuments && !importRegistry.GetExtendedTypeInformation(0xC5E07887u).UseBuildCache,
            "Direct import profile bypassed registry cache policy.");
        // Reborn: even explicit diagnostic activation must fail production policy before any output manager or dependency lookup runs.
        try { importRegistry.ValidateProductionOutput(); throw new InvalidDataException("Import profile bypassed production gate."); }
        catch (BinaryAssetBuilderException error) when (error.Message.Contains(imported.ProfileName, StringComparison.Ordinal)) { }
        foreach (InstanceDeclaration instance in document.SelfInstances)
        {
            Require(instance.Handle.TypeHash == 0 && instance.WeakReferencedInstances.Count == 0, "Diagnostic root unexpectedly became registered.");
            XmlDocument copy = new();
            copy.LoadXml(instance.XmlNode.OuterXml);
            copy.DocumentElement!.RemoveAttribute("TypeId");
            foreach (XmlElement child in copy.DocumentElement.ChildNodes.OfType<XmlElement>()) child.RemoveAttribute("TypeId");
            Chunk expected = AttributeModifierNativeSmokeTest.Compile(schemas, copy.DocumentElement.OuterXml);
            AssetBuffer actual = imported.ProcessInstance(instance);
            Require(actual.InstanceData.SequenceEqual(expected.InstanceBuffer) && actual.RelocationData.SequenceEqual(expected.RelocationBuffer)
                && actual.ImportsData.SequenceEqual(expected.ImportsBuffer), "Import profile compiler entry differs from native marshalling.");
            Chunk chunk = new() { InstanceBuffer = actual.InstanceData, RelocationBuffer = actual.RelocationData, ImportsBuffer = actual.ImportsData };
            ExpectRejected(() => restricted.ProcessInstance(instance));
            if (instance.Handle.InstanceName == "AttributeModifier_IronCurtain") CheckTamperedImports(imported, instance);
            result.Add(instance.Handle.Name, (chunk, instance.ReferencedInstances.Select(handle => new AssetId(handle.TypeId, handle.InstanceId)).ToArray()));
        }
        return result;
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: reject raw, malformed, out-of-range, duplicate, stale and wrong-type imports before native allocation; restore each mutation. */
    //-------------------------------------------------------------------------------------------------
    private static void CheckTamperedImports(Ra3Ep1AttributeModifierPlugin plugin, InstanceDeclaration instance)
    {
        XmlElement root = (XmlElement)instance.XmlNode;
        string original = root.GetAttribute("StartFX");
        foreach (string invalid in new[] { "FX_IronCurtainHit", "FX_IronCurtainHit\\-1", "FX_IronCurtainHit\\+1",
            "FX_IronCurtainHit\\4294967295", "FX_IronCurtainHit\\3", "FX_IronCurtainHit\\1\\1",
            "FX_WrongName\\1", "ShaderOverride:FX_IronCurtainHit\\1", "FX_IronCurtainHit\\0", "", "=Unresolved",
            "FXList::FX_IronCurtainHit\\1", ":FX_IronCurtainHit\\1", "FXList:\\1", " FX_IronCurtainHit\\1" })
        {
            root.SetAttribute("StartFX", invalid);
            try { ExpectRejected(() => plugin.ProcessInstance(instance)); }
            finally { root.SetAttribute("StartFX", original); }
        }
        InstanceHandle saved = instance.ReferencedInstances[1];
        foreach (InstanceHandle invalid in new[] { new InstanceHandle("FXList", "WrongIdentity"), new InstanceHandle("ShaderOverride", "FX_IronCurtainHit") })
        {
            instance.ReferencedInstances[1] = invalid;
            try { ExpectRejected(() => plugin.ProcessInstance(instance)); }
            finally { instance.ReferencedInstances[1] = saved; }
        }
        // Reborn: the table may not contain orphan slots even when every remaining XML suffix is parseable.
        instance.ReferencedInstances.Add(new InstanceHandle("FXList", "ExtraDependency"));
        try { ExpectRejected(() => plugin.ProcessInstance(instance)); }
        finally { instance.ReferencedInstances.RemoveAt(instance.ReferencedInstances.Count - 1); }
        root.RemoveAttribute("StartFX");
        try { ExpectRejected(() => plugin.ProcessInstance(instance)); }
        finally { root.SetAttribute("StartFX", original); }
        instance.WeakReferencedInstances.Add(new InstanceHandle("GameObject", "UnexpectedWeak"));
        try { ExpectRejected(() => plugin.ProcessInstance(instance)); }
        finally { instance.WeakReferencedInstances.Clear(); }
        instance.ReferencedFiles.Add("UnexpectedFile");
        try { ExpectRejected(() => plugin.ProcessInstance(instance)); }
        finally { instance.ReferencedFiles.Clear(); }
        ExpectRejected(() => plugin.ReInitialize(TargetPlatform.Xbox360));
        ExpectRejected(() => plugin.ProcessInstance(instance));
        plugin.ReInitialize(TargetPlatform.Win32);
        Require(plugin.ProcessInstance(instance).ImportsData.Length == 16, "Restored import profile did not reproduce three slots.");
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: malformed reference metadata must fail explicitly, not through an incidental parser or null exception. */
    //-------------------------------------------------------------------------------------------------
    private static void ExpectRejected(Action action)
    {
        try { action(); }
        catch (Exception error) when (error is NotSupportedException or InvalidOperationException) { return; }
        throw new InvalidDataException("Import profile accepted unsupported or inconsistent dependency metadata.");
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: require exact dependency order/identities and all native chunks, using only bounded reads from supplied linked EP1 streams. */
    //-------------------------------------------------------------------------------------------------
    private static void Compare(IReadOnlyDictionary<string, (Chunk Chunk, AssetId[] Dependencies)> expected, string path)
    {
        ManifestDocument manifest = ManifestReader.Read(File.ReadAllBytes(path));
        TypeRegistryAudit.ValidateTarget(manifest.Header.Version, manifest.Header.AllTypesHash);
        Require(manifest.Header.IsLinked && manifest.Validate().Count == 0, "Invalid linked EP1 manifest.");
        // Reborn: prove production metadata lookup finds the exact normalized targets without requiring FX/Shader compilation or BIN writes.
        CheckExternalTargets(expected.Values.SelectMany(value => value.Dependencies).Distinct().ToArray(), manifest, path);
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
    /** Reborn: resolve all modifier targets through the real external-manifest index and reject missing targets/stale identities after removing the mapping. */
    //-------------------------------------------------------------------------------------------------
    private static void CheckExternalTargets(AssetId[] dependencies, ManifestDocument manifest, string path)
    {
        string[]? previous = Settings.Current.ProcessedExternalManifests;
        int previousLevel = Settings.Current.ErrorLevel;
        try
        {
            Settings.Current.ProcessedExternalManifests = new[] { Path.GetFullPath(path) };
            Settings.Current.ErrorLevel = 1;
            InstanceHandle[] targets = dependencies.Select(id => new InstanceHandle(manifest.Assets.Single(asset =>
                asset.TypeId == id.TypeId && asset.InstanceId == id.InstanceId).Name)).ToArray();
            Require(targets.All(ExternalLinkSmokeTest.Contains), "Production external lookup missed a normalized FX/Shader target.");
            Require(!ExternalLinkSmokeTest.Contains(new InstanceHandle("FXList", "Reborn_Missing_Modifier_Import_Target")),
                "Missing external modifier target resolved unexpectedly.");
            Settings.Current.ProcessedExternalManifests = Array.Empty<string>();
            Require(targets.All(target => !ExternalLinkSmokeTest.Contains(target)), "Removed external mapping retained stale target identities.");
            Settings.Current.ProcessedExternalManifests = new[] { Path.GetFullPath(path) };
            Require(targets.All(ExternalLinkSmokeTest.Contains), "Restored external mapping failed to reload modifier identities.");
            Console.WriteLine($"  Production external metadata lookup: {targets.Length} modifier FX/Shader targets; missing/unmapped targets rejected");
        }
        finally { Settings.Current.ProcessedExternalManifests = previous; Settings.Current.ErrorLevel = previousLevel; }
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
