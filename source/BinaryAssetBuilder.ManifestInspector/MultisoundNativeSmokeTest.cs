using System.Buffers.Binary;
using System.Runtime.InteropServices;
using System.Xml;
using BinaryAssetBuilder.Core;
using BinaryAssetBuilder.Core.SageXml;
using BinaryAssetBuilder.Core.Session;
using Relo;
using SageBinaryData;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: native-only EP1 multisound proof keeps production audio registration and diagnostic command admission closed.
internal static class MultisoundNativeSmokeTest
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: exercise official schema/default/normalization stages, independent goldens and optional selected game slices. */
    //-------------------------------------------------------------------------------------------------
    internal static unsafe void Run(params string[] manifests)
    {
        Require(sizeof(Marshaler.Ep1Multisound) == 16 && sizeof(Marshaler.Ep1MultisoundSubsound) == 28
            && sizeof(MultisoundSubsoundRef) == 8 && sizeof(Multisound) == 16, "EP1 or legacy sound layout changed unexpectedly.");
        string[] fields = { "Base", "Weight", "PitchShiftLow", "PitchShiftHigh", "Volume", "PlayPercent", "VolumeShift" };
        for (int index = 0; index < fields.Length; index++)
            Require(Marshal.OffsetOf<Marshaler.Ep1MultisoundSubsound>(fields[index]).ToInt32() == index * 4, "EP1 child field offset differs.");
        string fixtures = Path.GetDirectoryName(ReferencePipelineSmokeTest.FindFixture())!;
        // Reborn: native flag bit order must agree with unchanged official EP1 schema declarations.
        XmlDocument schema = new(); schema.Load(Path.GetFullPath(Path.Combine(fixtures, "../../schemas/ra3ep1/xsd/AssetTypeAudio.xsd")));
        XmlNamespaceManager schemaNamespaces = new(schema.NameTable); schemaNamespaces.AddNamespace("xs", "http://www.w3.org/2001/XMLSchema");
        string[] controlNames = schema.SelectNodes("//xs:simpleType[@name='MultisoundControlFlag']/xs:restriction/xs:enumeration", schemaNamespaces)!
            .Cast<XmlElement>().Select(element => element.GetAttribute("value")).ToArray();
        Require(controlNames.SequenceEqual(Enum.GetNames<MultisoundControlFlag>()), "Official Multisound control enum drifted.");
        string directory = Path.Combine(Path.GetTempPath(), "Reborn-MultisoundNative-" + Guid.NewGuid().ToString("N")); Directory.CreateDirectory(directory);
        string source = Path.Combine(directory, "stock.xml"); File.Copy(Path.Combine(fixtures, "MultisoundProbe.xml"), source);
        Settings saved = Settings.Current;
        try
        {
            Settings.Current = new Settings { BuildCache = false, SchemaPath = Path.Combine(fixtures, "MultisoundPipeline.xsd"),
                DataRoot = directory, DataPaths = new[] { directory }, TargetPlatform = TargetPlatform.Win32,
                CustomPostfix = "", StreamPostfix = "", StringHashBinDescriptors = Array.Empty<StringHashBinDescriptor>() };
            SessionCache cache = new(); cache.InitializeCache(new System.Collections.Generic.List<string>());
            DocumentProcessor processor = new(Settings.Current, new PluginRegistry(Array.Empty<PluginDescriptor>(), TargetPlatform.Win32),
                new VerifierPluginRegistry(Array.Empty<PluginDescriptor>(), TargetPlatform.Win32)) { Cache = cache, SchemaSet = new SchemaSet(false) };
            AssetDeclarationDocument document = Read(processor, source);
            Dictionary<string, (Chunk Data, uint[] Names)> expected = new(StringComparer.Ordinal);
            foreach (InstanceDeclaration instance in document.SelfInstances)
            {
                Require(instance.Handle.TypeHash == 0 && instance.ReferencedFiles.Count == 0 && instance.WeakReferencedInstances.Count == 0,
                    "Native sound proof activated processor metadata or file/weak dependencies.");
                Chunk actual = Compile(instance); Equal(actual, Compile(instance));
                uint[] weights = instance.Handle.InstanceName switch
                {
                    "GDI_Generic_VoiceDieMS" => new uint[] { 1000,800,500,800,1000,500,300,600,800,500,500,500,300,1300,1000,200 },
                    "NOD_Generic_VoiceDieMS" => new uint[] { 500,400,500,500,100,700,500,500,500,600,500,600,500,600,900,700,400,700,300,200 },
                    "Gui_GlobalShellPanelExpand" => new uint[] { 1000,1000 },
                    _ => throw new InvalidDataException("Unexpected sound fixture.")
                };
                Equal(actual, Golden(instance.Handle.InstanceName == "Gui_GlobalShellPanelExpand" ? 0u : 2u, weights));
                expected.Add(instance.Handle.Name, (actual, instance.ReferencedInstances.Select(handle => handle.InstanceId).ToArray()));
            }
            foreach (string manifest in manifests) Compare(expected, manifest);
            string synthetic = Path.Combine(directory, "optional.xml");
            File.WriteAllText(synthetic, """
                <AssetDeclaration xmlns="uri:ea.com:eala:asset">
                <Multisound id="Optional" Control="LOOP PLAY_ONE">
                <Subsound Weight="0" PitchShiftLow="0" PitchShiftHigh="2" Volume="0" PlayPercent="100" VolumeShift="-10">First</Subsound>
                <Subsound>Second</Subsound></Multisound><Multisound id="Empty" /></AssetDeclaration>
                """);
            AssetDeclarationDocument optional = Read(processor, synthetic);
            Equal(Compile(optional.SelfInstances.Single(instance => instance.Handle.InstanceName == "Optional")),
                Golden(3, new uint[] { 0,1000 }, new float[] { 0,2,0,1,-10 * 0.01f }));
            Equal(Compile(optional.SelfInstances.Single(instance => instance.Handle.InstanceName == "Empty")), Golden(0, Array.Empty<uint>()));
            // Reborn: invalid official controls, unsigned weights and scalar values never reach native compilation.
            string[] invalid = { "Control=\"UNKNOWN\"", "Weight=\"-1\"", "PitchShiftLow=\"oops\"" };
            for (int index = 0; index < invalid.Length; index++)
            {
                string negative = Path.Combine(directory, "invalid-" + index + ".xml");
                File.WriteAllText(negative, "<AssetDeclaration xmlns=\"uri:ea.com:eala:asset\"><Multisound id=\"Invalid\" "
                    + (index == 0 ? invalid[index] : "") + "><Subsound " + (index != 0 ? invalid[index] : "")
                    + ">First</Subsound></Multisound></AssetDeclaration>");
                bool rejected = false;
                try { Read(processor, negative); } catch (BinaryAssetBuilderException) { rejected = true; }
                Require(rejected, "Invalid sound schema input reached native processing.");
            }
            string recovery = Path.Combine(directory, "recovery.xml"); File.Copy(source, recovery);
            foreach (InstanceDeclaration instance in Read(processor, recovery).SelfInstances) Equal(Compile(instance), expected[instance.Handle.Name].Data);
            Require(!Directory.EnumerateFiles(directory, "*.bin").Any(), "Native sound tests emitted production streams.");
            Console.WriteLine("Multisound native self-test: OK (isolated 16/28 layout, unchanged legacy 16/8, three goldens, optional pointers/zero/percentages, empty/default/control/import order; no audio registration)");
        }
        finally { Settings.Current = saved; }
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: validate and normalize references without generating production output or opening an audio plugin. */
    //-------------------------------------------------------------------------------------------------
    private static AssetDeclarationDocument Read(DocumentProcessor processor, string source) => processor.ProcessDocumentInternal(source, source, null!,
        new DocumentProcessor.ProcessOptions { GenerateOutput = false, UsePrecompiled = false });

    //-------------------------------------------------------------------------------------------------
    /** Reborn: use the explicit Win32 EP1 root rather than the legacy Multisound marshal overload. */
    //-------------------------------------------------------------------------------------------------
    private static unsafe Chunk Compile(InstanceDeclaration instance)
    {
        Marshaler.Ep1Multisound* root;
        using Tracker tracker = new((void**)&root, (uint)sizeof(Marshaler.Ep1Multisound), false);
        XmlNamespaceManager namespaces = new(instance.XmlNode.OwnerDocument!.NameTable); namespaces.AddNamespace("ea", "uri:ea.com:eala:asset");
        Marshaler.Marshal(new Node(instance.XmlNode.CreateNavigator()!, namespaces), root, tracker);
        Chunk chunk = new(); Require(tracker.MakeRelocatable(chunk), "EP1 native sound marshalling failed."); return chunk;
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: construct byte goldens independently from explicit ABI offsets and one-biased selectors, never from marshaller output. */
    //-------------------------------------------------------------------------------------------------
    private static Chunk Golden(uint control, uint[] weights, float[]? optionalFirst = null)
    {
        int tableEnd = 16 + weights.Length * 28;
        byte[] bin = new byte[tableEnd + (optionalFirst?.Length ?? 0) * 4]; Put(bin, 4, control); Put(bin, 8, (uint)weights.Length);
        System.Collections.Generic.List<uint> relo = new(), imp = new();
        if (weights.Length > 0) { Put(bin, 12, 16); relo.Add(12); }
        for (int index = 0; index < weights.Length; index++)
        {
            int offset = 16 + index * 28; Put(bin, offset, (uint)index + 1); Put(bin, offset + 4, weights[index]); imp.Add((uint)offset);
        }
        if (optionalFirst != null)
            for (int index = 0; index < optionalFirst.Length; index++)
            {
                int slot = 24 + index * 4, target = tableEnd + index * 4;
                Put(bin, slot, (uint)target); Put(bin, target, BitConverter.SingleToUInt32Bits(optionalFirst[index])); relo.Add((uint)slot);
            }
        return new Chunk { InstanceBuffer = bin, RelocationBuffer = Offsets(relo), ImportsBuffer = Offsets(imp) };
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: include the native sentinel only for a nonempty relocation/import array. */
    //-------------------------------------------------------------------------------------------------
    private static byte[] Offsets(System.Collections.Generic.List<uint> values)
    {
        if (values.Count == 0) return Array.Empty<byte>();
        byte[] bytes = new byte[(values.Count + 1) * 4];
        for (int index = 0; index < values.Count; index++) Put(bytes, index * 4, values[index]);
        Put(bytes, values.Count * 4, uint.MaxValue); return bytes;
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: compare only selected stock native slices and ordered concrete audio identities, not full game streams. */
    //-------------------------------------------------------------------------------------------------
    private static void Compare(Dictionary<string, (Chunk Data, uint[] Names)> expected, string path)
    {
        ManifestDocument manifest = ManifestReader.Read(File.ReadAllBytes(path));
        TypeRegistryAudit.ValidateTarget(manifest.Header.Version, manifest.Header.AllTypesHash);
        Require(manifest.Header.IsLinked && manifest.Validate().Count == 0, "Invalid stock sound manifest.");
        long bin = manifest.Header.ContainerPrefixSize + 4, relo = bin, imp = bin; int count = 0;
        foreach (ManifestAsset asset in manifest.Assets)
        {
            if (expected.TryGetValue(asset.Name, out var value))
            {
                Require(asset.TypeId == 0xA3A7AF37u && asset.TypeHash == 0xF79C5A89u && asset.Tokenized == 0
                    && asset.References.All(reference => reference.TypeId == 0x844D7B9Fu)
                    && asset.References.Select(reference => reference.InstanceId).SequenceEqual(value.Names)
                    && value.Data.InstanceBuffer.SequenceEqual(AssetStreamProbe.ReadRange(Path.ChangeExtension(path, ".bin"), null, bin, asset.InstanceDataSize))
                    && value.Data.RelocationBuffer.SequenceEqual(AssetStreamProbe.ReadRange(Path.ChangeExtension(path, ".relo"), null, relo, asset.RelocationDataSize))
                    && value.Data.ImportsBuffer.SequenceEqual(AssetStreamProbe.ReadRange(Path.ChangeExtension(path, ".imp"), null, imp, asset.ImportsDataSize)),
                    "Stock Multisound native/reference comparison failed: " + asset.Name);
                Console.WriteLine($"  Real EP1 Multisound golden: {asset.Name} exact {asset.InstanceDataSize}/{asset.RelocationDataSize}/{asset.ImportsDataSize}"); count++;
            }
            bin += asset.InstanceDataSize; relo += asset.RelocationDataSize; imp += asset.ImportsDataSize;
        }
        Require(count == expected.Count, "Stock manifest is missing selected Multisound goldens.");
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: write explicit little-endian native words for independent fixtures. */
    //-------------------------------------------------------------------------------------------------
    private static void Put(byte[] data, int offset, uint value) => BinaryPrimitives.WriteUInt32LittleEndian(data.AsSpan(offset, 4), value);

    //-------------------------------------------------------------------------------------------------
    /** Reborn: compare every native byte, including pointer offsets, padding, order and sentinels. */
    //-------------------------------------------------------------------------------------------------
    private static void Equal(Chunk first, Chunk second) => Require(first.InstanceBuffer.SequenceEqual(second.InstanceBuffer)
        && first.RelocationBuffer.SequenceEqual(second.RelocationBuffer) && first.ImportsBuffer.SequenceEqual(second.ImportsBuffer), "Multisound native golden differs.");

    //-------------------------------------------------------------------------------------------------
    /** Reborn: fail native evidence checks without claiming processor or runtime readiness. */
    //-------------------------------------------------------------------------------------------------
    private static void Require(bool condition, string message) { if (!condition) throw new InvalidDataException(message); }
}
