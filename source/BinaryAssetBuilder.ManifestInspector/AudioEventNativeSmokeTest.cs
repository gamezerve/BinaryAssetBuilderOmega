using System.Buffers.Binary;
using System.Runtime.InteropServices;
using System.Xml;
using BinaryAssetBuilder.Core;
using BinaryAssetBuilder.Core.SageXml;
using BinaryAssetBuilder.Core.Session;
using Relo;
using SageBinaryData;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: native-only AudioEvent proof does not admit audio production, codecs or diagnostic-build roots.
internal static class AudioEventNativeSmokeTest
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: compare independently assembled bytes with official-schema compilation and optional bounded stock slices. */
    //-------------------------------------------------------------------------------------------------
    internal static unsafe void Run(params string[] manifests)
    {
        Require(sizeof(Marshaler.Ep1BaseSingleSound) == 128 && sizeof(Marshaler.Ep1AudioEvent) == 152
            && sizeof(Marshaler.Ep1AudioFileRefWithWeight) == 12 && sizeof(Marshaler.Ep1TimeRange) == 8
            && sizeof(BaseSingleSound) == 96 && sizeof(AudioEvent) == 120 && sizeof(AudioFileRefWithWeight) == 8,
            "Native/legacy audio ABI changed.");
        string[] names = { "ShrunkenPitchModifier", "ShrunkenVolumeModifier", "Control", "SubmixSlider", "InitialDelay",
            "VolumeSliderMultiplier", "MinRangeShift", "MaxRangeShift", "LimitGroup", "NonInterruptibleTime" };
        int[] offsets = { 20,24,44,80,96,100,108,112,116,124 };
        for (int index = 0; index < names.Length; index++)
            Require(Marshal.OffsetOf<Marshaler.Ep1BaseSingleSound>(names[index]).ToInt32() == offsets[index], "Native audio offset differs.");
        Require(Marshal.OffsetOf<Marshaler.Ep1AudioEvent>("Sound").ToInt32() == 136
            && Marshal.OffsetOf<Marshaler.Ep1AudioFileRefWithWeight>("Volume").ToInt32() == 8, "Native audio list/volume offset differs.");
        string fixtures = Path.GetDirectoryName(ReferencePipelineSmokeTest.FindFixture())!;
        XmlDocument schema = new(); schema.Load(Path.GetFullPath(Path.Combine(fixtures, "../../schemas/ra3ep1/xsd/AssetTypeAudio.xsd")));
        XmlNamespaceManager ns = new(schema.NameTable); ns.AddNamespace("xs", "http://www.w3.org/2001/XMLSchema");
        Require(schema.SelectNodes("//xs:simpleType[@name='AudioControlFlag']/xs:restriction/xs:enumeration", ns)!
            .Cast<XmlElement>().Select(element => element.GetAttribute("value")).SequenceEqual(Enum.GetNames<Marshaler.Ep1AudioControlFlag>()),
            "Official EP1 audio control order drifted.");
        string directory = Path.Combine(Path.GetTempPath(), "Reborn-AudioEventNative-" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(directory);
        string source = Path.Combine(directory, "stock.xml"); File.Copy(Path.Combine(fixtures, "AudioEventProbe.xml"), source);
        Settings saved = Settings.Current;
        try
        {
            Settings.Current = new Settings { BuildCache = false, SchemaPath = Path.Combine(fixtures, "AudioEventPipeline.xsd"),
                DataRoot = directory, DataPaths = new[] { directory }, TargetPlatform = TargetPlatform.Win32,
                CustomPostfix = "", StreamPostfix = "", StringHashBinDescriptors = Array.Empty<StringHashBinDescriptor>() };
            SessionCache cache = new(); cache.InitializeCache(new System.Collections.Generic.List<string>());
            DocumentProcessor processor = new(Settings.Current, new PluginRegistry(Array.Empty<PluginDescriptor>(), TargetPlatform.Win32),
                new VerifierPluginRegistry(Array.Empty<PluginDescriptor>(), TargetPlatform.Win32)) { Cache = cache, SchemaSet = new SchemaSet(false) };
            InstanceDeclaration instance = Read(processor, source).SelfInstances.Single();
            Require(instance.Handle.TypeHash == 0 && instance.ReferencedInstances.Count == 16
                && instance.ReferencedFiles.Count == 0 && instance.WeakReferencedInstances.Count == 0, "Audio proof activated processors/files.");
            Chunk golden = ImpactGolden(); Equal(Compile(instance), golden); Equal(Compile(instance), Compile(instance));
            foreach (string manifest in manifests) Compare(golden, instance.ReferencedInstances.Select(handle => handle.InstanceId).ToArray(), manifest);
            string empty = Path.Combine(directory, "empty.xml");
            File.WriteAllText(empty, "<AssetDeclaration xmlns=\"uri:ea.com:eala:asset\"><AudioEvent id=\"Empty\" /></AssetDeclaration>");
            Chunk emptyGolden = Defaults(); Equal(Compile(Read(processor, empty).SelfInstances.Single()), emptyGolden);
            string controls = Path.Combine(directory, "controls.xml");
            File.WriteAllText(controls, "<AssetDeclaration xmlns=\"uri:ea.com:eala:asset\"><AudioEvent id=\"Controls\" Control=\"SMART_LIMITING FADE_ON_KILL IMMEDIATE_DECAY_ON_KILL\" /></AssetDeclaration>");
            Chunk controlGolden = Defaults(); Put(controlGolden.InstanceBuffer, 44, 0x130);
            Equal(Compile(Read(processor, controls).SelfInstances.Single()), controlGolden);
            string weighted = Path.Combine(directory, "weighted.xml");
            File.WriteAllText(weighted, "<AssetDeclaration xmlns=\"uri:ea.com:eala:asset\"><AudioEvent id=\"Weighted\"><Attack Weight=\"0\" Volume=\"0\">First</Attack><Sound Volume=\"25\">Second</Sound><Decay Weight=\"7\">Third</Decay></AudioEvent></AssetDeclaration>");
            Chunk weightedGolden = Defaults(188);
            for (int index = 0; index < 3; index++)
            {
                Put(weightedGolden.InstanceBuffer, 128 + index * 8, 1); Put(weightedGolden.InstanceBuffer, 132 + index * 8, (uint)(152 + index * 12));
                Put(weightedGolden.InstanceBuffer, 152 + index * 12, (uint)index + 1);
                Put(weightedGolden.InstanceBuffer, 156 + index * 12, index == 0 ? 0u : index == 1 ? 1000u : 7u);
                Float(weightedGolden.InstanceBuffer, 160 + index * 12, index == 0 ? 0f : index == 1 ? 0.25f : 1f);
            }
            weightedGolden.RelocationBuffer = Words(132,140,148,uint.MaxValue);
            weightedGolden.ImportsBuffer = Words(152,164,176,uint.MaxValue);
            Equal(Compile(Read(processor, weighted).SelfInstances.Single()), weightedGolden);
            // Reborn: independent offsets exercise every implemented optional base allocation and explicit zero versus absence.
            string optional = Path.Combine(directory, "optional.xml");
            File.WriteAllText(optional, """
                <AssetDeclaration xmlns="uri:ea.com:eala:asset"><AudioEvent id="Optional"
                 SubmixSlider="VOICE" ShrunkenPitchModifier="0" ShrunkenVolumeModifier="0" DryLevel="0">
                 <PitchShift Low="0" High="2" /><PerFilePitchShift Low="-1" High="1" />
                 <Delay Low="0" High="7" /><InitialDelay Low="3" High="9" />
                 <VolumeSliderMultiplier Slider="MUSIC" Multiplier="0" />
                 <MinRangeShift Low="0" High="4" /><MaxRangeShift Low="5" High="6" />
                 <NonInterruptibleTime Low="250ms" High="2s" />
                </AudioEvent></AssetDeclaration>
                """);
            Chunk optionalGolden = Defaults(220);
            foreach (int offset in new[] { 20,24,76 }) Put(optionalGolden.InstanceBuffer, offset, 0);
            int[] slots = { 80,84,88,92,96,104,108,112,124 };
            int[] targets = { 152,156,164,172,180,188,196,204,212 };
            for (int index = 0; index < slots.Length; index++) Put(optionalGolden.InstanceBuffer, slots[index], (uint)targets[index]);
            Put(optionalGolden.InstanceBuffer, 100, 1); Put(optionalGolden.InstanceBuffer, 152, 1);
            Float(optionalGolden.InstanceBuffer, 160, 2); Float(optionalGolden.InstanceBuffer, 164, -1); Float(optionalGolden.InstanceBuffer, 168, 1);
            Put(optionalGolden.InstanceBuffer, 176, 7); Put(optionalGolden.InstanceBuffer, 180, 3); Put(optionalGolden.InstanceBuffer, 184, 9);
            Put(optionalGolden.InstanceBuffer, 188, 2); Float(optionalGolden.InstanceBuffer, 200, 4);
            Float(optionalGolden.InstanceBuffer, 204, 5); Float(optionalGolden.InstanceBuffer, 208, 6);
            Float(optionalGolden.InstanceBuffer, 212, 0.25f); Float(optionalGolden.InstanceBuffer, 216, 2);
            optionalGolden.RelocationBuffer = Words(slots.Select(offset => (uint)offset).Append(uint.MaxValue).ToArray());
            Equal(Compile(Read(processor, optional).SelfInstances.Single()), optionalGolden);
            // Reborn: official validation rejects malformed scalars/flags; explicitly unrecovered LimitGroup cannot silently serialize empty slots.
            foreach (string bad in new[] { "Control=\"UNKNOWN\"", "ShrunkenPitchModifier=\"oops\"" })
            {
                string negative = Path.Combine(directory, Guid.NewGuid().ToString("N") + ".xml");
                File.WriteAllText(negative, "<AssetDeclaration xmlns=\"uri:ea.com:eala:asset\"><AudioEvent id=\"Bad\" " + bad + " /></AssetDeclaration>");
                bool rejected = false; try { Read(processor, negative); } catch (BinaryAssetBuilderException) { rejected = true; }
                Require(rejected, "Malformed audio input reached native compilation.");
            }
            string group = Path.Combine(directory, "group.xml");
            File.WriteAllText(group, "<AssetDeclaration xmlns=\"uri:ea.com:eala:asset\"><AudioEvent id=\"Group\"><LimitGroup>First</LimitGroup></AudioEvent></AssetDeclaration>");
            bool groupRejected = false;
            try { Compile(Read(processor, group).SelfInstances.Single()); }
            catch (NotSupportedException exception) when (exception.Message.Contains("LimitGroup", StringComparison.Ordinal)) { groupRejected = true; }
            Require(groupRejected, "Unrecovered LimitGroup was silently marshalled.");
            string recovery = Path.Combine(directory, "recovery.xml"); File.Copy(source, recovery);
            Equal(Compile(Read(processor, recovery).SelfInstances.Single()), golden);
            Require(!Directory.EnumerateFiles(directory, "*.bin").Any(), "Audio native proof emitted streams.");
            Console.WriteLine("AudioEvent native self-test: OK (isolated 128/152/12 layout; unchanged legacy 96/120/8; exact impact/default/flag/weighted-list/optional-pointer goldens; LimitGroup rejected; no audio plugin/codec)");
        }
        finally { Settings.Current = saved; }
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: validate and normalize official AudioFile references without processor registration or generated output. */
    //-------------------------------------------------------------------------------------------------
    private static AssetDeclarationDocument Read(DocumentProcessor processor, string source) => processor.ProcessDocumentInternal(source, source, null!,
        new DocumentProcessor.ProcessOptions { GenerateOutput = false, UsePrecompiled = false });

    //-------------------------------------------------------------------------------------------------
    /** Reborn: compile the explicit isolated Win32 root only; callers supply trusted schema-validated test instances. */
    //-------------------------------------------------------------------------------------------------
    private static unsafe Chunk Compile(InstanceDeclaration instance)
    {
        Marshaler.Ep1AudioEvent* root;
        using Tracker tracker = new((void**)&root, (uint)sizeof(Marshaler.Ep1AudioEvent), false);
        XmlNamespaceManager ns = new(instance.XmlNode.OwnerDocument!.NameTable); ns.AddNamespace("ea", "uri:ea.com:eala:asset");
        Marshaler.Marshal(new Node(instance.XmlNode.CreateNavigator()!, ns), root, tracker);
        Chunk chunk = new(); Require(tracker.MakeRelocatable(chunk), "Audio native compilation failed."); return chunk;
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: encode observed stock words independently of the marshaller, including native allocation and sentinel order. */
    //-------------------------------------------------------------------------------------------------
    private static Chunk ImpactGolden()
    {
        byte[] bin = new byte[364];
        uint[] root = { 0,0x3F199999,0xBE4CCCCC,0, 0,0x40000000,0x3F599999,0x3F800000,
            3,2,0x86,8, 0x43960000,0x447A0000,0,0x3F000000,
            0x3F800000,0x3E4CCCCC,0x3F800000,0x3F800000, 344,348,0,0,
            0,0,0,0, 0,0,0,356, 0,0,16,152, 0,0 };
        for (int index = 0; index < root.Length; index++) Put(bin, index * 4, root[index]);
        for (int index = 0; index < 16; index++)
        { Put(bin, 152 + index * 12, (uint)index + 1); Put(bin, 156 + index * 12, 1000); Float(bin, 160 + index * 12, 1); }
        Float(bin, 348, -5); Float(bin, 352, 5); Float(bin, 356, 0); Float(bin, 360, 1.25f);
        return new Chunk { InstanceBuffer = bin, RelocationBuffer = Words(140,80,84,124,uint.MaxValue),
            ImportsBuffer = Words(Enumerable.Range(0,16).Select(index => (uint)(152 + index * 12)).Append(uint.MaxValue).ToArray()) };
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: independent official default words preserve absent optional pointers, including absent SubmixSlider. */
    //-------------------------------------------------------------------------------------------------
    private static Chunk Defaults(int size = 152)
    {
        byte[] bin = new byte[size];
        foreach (int offset in new[] { 4,20,24,28,64,76 }) Float(bin, offset, 1);
        Put(bin, 36, 2); Float(bin, 48, 160); Float(bin, 52, 640); Float(bin, 60, 0.5f); Float(bin, 68, 20 * 0.01f);
        return new Chunk { InstanceBuffer = bin, RelocationBuffer = Array.Empty<byte>(), ImportsBuffer = Array.Empty<byte>() };
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: read only the selected record's three native slices and require ordered concrete AudioFile identities. */
    //-------------------------------------------------------------------------------------------------
    private static void Compare(Chunk golden, uint[] identities, string path)
    {
        ManifestDocument manifest = ManifestReader.Read(File.ReadAllBytes(path));
        TypeRegistryAudit.ValidateTarget(manifest.Header.Version, manifest.Header.AllTypesHash);
        Require(manifest.Header.IsLinked && manifest.Validate().Count == 0, "Invalid stock audio manifest.");
        long bin = manifest.Header.ContainerPrefixSize + 4, relo = bin, imp = bin; int count = 0;
        foreach (ManifestAsset asset in manifest.Assets)
        {
            if (asset.Name == "AudioEvent:ImpactDebrisHitsGround")
            {
                Require(asset.TypeId == 0x844D7B9Fu && asset.TypeHash == 0x560C2E45u && asset.Tokenized == 0
                    && asset.InstanceId == 0x30A09D68u && asset.InstanceDataSize == 364 && asset.RelocationDataSize == 20 && asset.ImportsDataSize == 68
                    && asset.References.All(reference => reference.TypeId == 0x166B084Du)
                    && asset.References.Select(reference => reference.InstanceId).SequenceEqual(identities), "Stock AudioEvent fingerprint/reference mismatch.");
                Equal(golden, new Chunk {
                    InstanceBuffer = AssetStreamProbe.ReadRange(Path.ChangeExtension(path, ".bin"), null, bin, asset.InstanceDataSize),
                    RelocationBuffer = AssetStreamProbe.ReadRange(Path.ChangeExtension(path, ".relo"), null, relo, asset.RelocationDataSize),
                    ImportsBuffer = AssetStreamProbe.ReadRange(Path.ChangeExtension(path, ".imp"), null, imp, asset.ImportsDataSize) });
                Console.WriteLine("  Real EP1 AudioEvent golden: ImpactDebrisHitsGround exact 364/20/68; 16 ordered AudioFile identities"); count++;
            }
            bin += asset.InstanceDataSize; relo += asset.RelocationDataSize; imp += asset.ImportsDataSize;
        }
        Require(count == 1, "Selected stock AudioEvent is missing or duplicated.");
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: encode independent little-endian fixture words and native sentinels. */
    //-------------------------------------------------------------------------------------------------
    private static byte[] Words(params uint[] words)
    { byte[] data = new byte[words.Length * 4]; for (int index = 0; index < words.Length; index++) Put(data, index * 4, words[index]); return data; }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: write one independent native word. */
    //-------------------------------------------------------------------------------------------------
    private static void Put(byte[] data, int offset, uint value) => BinaryPrimitives.WriteUInt32LittleEndian(data.AsSpan(offset,4), value);

    //-------------------------------------------------------------------------------------------------
    /** Reborn: preserve single-precision fixture normalization. */
    //-------------------------------------------------------------------------------------------------
    private static void Float(byte[] data, int offset, float value) => Put(data, offset, BitConverter.SingleToUInt32Bits(value));

    //-------------------------------------------------------------------------------------------------
    /** Reborn: compare complete native bytes, padding, relocations, selectors and sentinels. */
    //-------------------------------------------------------------------------------------------------
    private static void Equal(Chunk first, Chunk second) => Require(first.InstanceBuffer.SequenceEqual(second.InstanceBuffer)
        && first.RelocationBuffer.SequenceEqual(second.RelocationBuffer) && first.ImportsBuffer.SequenceEqual(second.ImportsBuffer), "AudioEvent native golden differs.");

    //-------------------------------------------------------------------------------------------------
    /** Reborn: fail closed on every native evidence mismatch. */
    //-------------------------------------------------------------------------------------------------
    private static void Require(bool condition, string message) { if (!condition) throw new InvalidDataException(message); }
}
