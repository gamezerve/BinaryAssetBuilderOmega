using System.Buffers.Binary;
using System.Xml;
using BinaryAssetBuilder.Core;
using BinaryAssetBuilder.Core.SageXml;
using BinaryAssetBuilder.Core.Session;
using Relo;
using SageBinaryData;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: prove native FX optional-mask layout independently of experimental processors or production registration.
internal static class FXListNativeSmokeTest
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: compile actual schema/default/reference document stages and compare only selected native game slices when supplied. */
    //-------------------------------------------------------------------------------------------------
    internal static void Run(string[] manifests)
    {
        string fixtures = Path.GetDirectoryName(ReferencePipelineSmokeTest.FindFixture())!;
        string directory = Path.Combine(Path.GetTempPath(), "Reborn-FXNative-" + Guid.NewGuid().ToString("N")); Directory.CreateDirectory(directory);
        string source = Path.Combine(directory, "fx.xml"); File.Copy(Path.Combine(fixtures, "FXListProbe.xml"), source);
        Settings saved = Settings.Current;
        try
        {
            // Reborn: extracted dependency enums must stay identical to their official declarations, not become permissive test stubs.
            CheckExtractedEnums(fixtures);
            Settings.Current = new Settings { BuildCache = false, SchemaPath = Path.Combine(fixtures, "FXListPipeline.xsd"),
                DataRoot = directory, DataPaths = new[] { directory }, TargetPlatform = TargetPlatform.Win32,
                CustomPostfix = "", StreamPostfix = "", StringHashBinDescriptors = Array.Empty<StringHashBinDescriptor>() };
            SessionCache cache = new(); cache.InitializeCache(new System.Collections.Generic.List<string>());
            PluginRegistry plugins = new(Array.Empty<PluginDescriptor>(), TargetPlatform.Win32);
            DocumentProcessor processor = new(Settings.Current, plugins, new VerifierPluginRegistry(Array.Empty<PluginDescriptor>(), TargetPlatform.Win32))
                { Cache = cache, SchemaSet = new SchemaSet(false) };
            AssetDeclarationDocument document = processor.ProcessDocumentInternal(source, source, null!,
                new DocumentProcessor.ProcessOptions { GenerateOutput = false, UsePrecompiled = false });
            Dictionary<string, (Chunk Data, uint[] Names)> expected = new(StringComparer.Ordinal);
            foreach (InstanceDeclaration instance in document.SelfInstances)
            {
                Require(instance.Handle.TypeHash == 0 && instance.ReferencedFiles.Count == 0 && instance.WeakReferencedInstances.Count == 0,
                    "FX native proof unexpectedly registered production metadata or file/weak dependencies.");
                Chunk data = Compile(instance), repeat = Compile(instance);
                Require(data.InstanceBuffer.SequenceEqual(repeat.InstanceBuffer) && data.RelocationBuffer.SequenceEqual(repeat.RelocationBuffer)
                    && data.ImportsBuffer.SequenceEqual(repeat.ImportsBuffer), "FX native output is nondeterministic.");
                expected.Add(instance.Handle.Name, (data, instance.ReferencedInstances.Select(handle => handle.InstanceId).ToArray()));
            }
            Chunk empty = expected["FXList:FX_NONE"].Data, sound = expected["FXList:FX_DebrisHitGround"].Data,
                masks = expected["FXList:FX_ALL_AntiGroundAircraft_VoiceDie"].Data;
            Require(empty.InstanceBuffer.Length == 28 && empty.InstanceBuffer.All(value => value == 0)
                && empty.RelocationBuffer.Length == 0 && empty.ImportsBuffer.Length == 0, "Empty native FX root differs.");
            Require(sound.InstanceBuffer.Length == 80 && Read(sound.InstanceBuffer, 16) == 1 && Read(sound.InstanceBuffer, 20) == 28
                && Read(sound.InstanceBuffer, 28) == 32 && Read(sound.InstanceBuffer, 32) == 0x1E08C8FBu
                && Read(sound.InstanceBuffer, 52) == 6 && Read(sound.InstanceBuffer, 76) == 1
                && Words(sound.RelocationBuffer).SequenceEqual(new uint[] { 20, 28, uint.MaxValue })
                && Words(sound.ImportsBuffer).SequenceEqual(new uint[] { 76, uint.MaxValue }), "Single sound FX layout/import/default differs.");
            Require(masks.InstanceBuffer.Length == 252 && Read(masks.InstanceBuffer, 16) == 2 && Read(masks.InstanceBuffer, 28) == 36
                && Read(masks.InstanceBuffer, 32) == 144 && Read(masks.InstanceBuffer, 48) == 84 && Read(masks.InstanceBuffer, 160) == 192
                && Read(masks.InstanceBuffer, 80) == 1 && Read(masks.InstanceBuffer, 188) == 2
                && Words(masks.RelocationBuffer).SequenceEqual(new uint[] { 20, 28, 32, 48, 160, uint.MaxValue })
                && Words(masks.ImportsBuffer).SequenceEqual(new uint[] { 80, 188, uint.MaxValue }), "Two sound FX optional-mask allocation or selectors differ.");
            foreach (string manifest in manifests) Compare(expected, manifest);
            CheckOptionalMasks(processor, source);
            CheckWeatherFallback();
            Require(!Directory.EnumerateFiles(directory, "*.bin").Any(), "Native-only FX proof emitted stream output.");
            Console.WriteLine("FX native self-test: OK (28-byte root, 44-byte optional-pointer base, empty/sound/two-mask polymorphic chunks, defaults, native selectors, determinism; no FX registration)");
        }
        finally { Settings.Current = saved; }
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: marshal the real core-normalized FX declaration only; this is native evidence, not ProcessInstance readiness or derived audio resolution. */
    //-------------------------------------------------------------------------------------------------
    internal static unsafe Chunk Compile(InstanceDeclaration instance)
    {
        FXList* root;
        using Tracker tracker = new((void**)&root, (uint)sizeof(FXList), false);
        XmlNamespaceManager namespaces = new(instance.XmlNode.OwnerDocument!.NameTable);
        namespaces.AddNamespace("ea", "uri:ea.com:eala:asset");
        Node node = new(instance.XmlNode.CreateNavigator()!, namespaces);
        Marshaler.Marshal(node, root, tracker);
        Chunk chunk = new(); Require(tracker.MakeRelocatable(chunk), "FX native marshalling failed."); return chunk;
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: compare selected exact native slices and ordered audio name hashes; compiler-profile callers also verify concrete dependency tuples separately. */
    //-------------------------------------------------------------------------------------------------
    internal static void Compare(Dictionary<string, (Chunk Data, uint[] Names)> expected, string path)
    {
        ManifestDocument manifest = ManifestReader.Read(File.ReadAllBytes(path));
        TypeRegistryAudit.ValidateTarget(manifest.Header.Version, manifest.Header.AllTypesHash);
        Require(manifest.Header.IsLinked && manifest.Validate().Count == 0, "Invalid stock FX manifest.");
        long bin = manifest.Header.ContainerPrefixSize + 4, relo = bin, imp = bin; int count = 0;
        foreach (ManifestAsset asset in manifest.Assets)
        {
            if (expected.TryGetValue(asset.Name, out var value))
            {
                Require(asset.TypeId == 0x86682E78u && asset.TypeHash == 0x17B3B82Du && asset.Tokenized == 0
                    && asset.References.Select(reference => reference.InstanceId).SequenceEqual(value.Names)
                    && asset.References.All(reference => reference.TypeId is 0x844D7B9Fu or 0xA3A7AF37u)
                    && value.Data.InstanceBuffer.SequenceEqual(AssetStreamProbe.ReadRange(Path.ChangeExtension(path, ".bin"), null, bin, asset.InstanceDataSize))
                    && value.Data.RelocationBuffer.SequenceEqual(AssetStreamProbe.ReadRange(Path.ChangeExtension(path, ".relo"), null, relo, asset.RelocationDataSize))
                    && value.Data.ImportsBuffer.SequenceEqual(AssetStreamProbe.ReadRange(Path.ChangeExtension(path, ".imp"), null, imp, asset.ImportsDataSize)),
                    "Stock FX native/dependency-order comparison failed: " + asset.Name);
                Console.WriteLine($"  Real EP1 FX golden: {asset.Name} exact {asset.InstanceDataSize}/{asset.RelocationDataSize}/{asset.ImportsDataSize}; stock audio types={string.Join(',', asset.References.Select(reference => reference.TypeId.ToString("X8")))}"); count++;
            }
            bin += asset.InstanceDataSize; relo += asset.RelocationDataSize; imp += asset.ImportsDataSize;
        }
        Require(count == expected.Count, "Stock manifest is missing one or more selected FX goldens.");
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: interpret only explicit little-endian words in bounded native buffers. */
    //-------------------------------------------------------------------------------------------------
    private static uint Read(byte[] data, int offset) => BinaryPrimitives.ReadUInt32LittleEndian(data.AsSpan(offset, 4));

    //-------------------------------------------------------------------------------------------------
    /** Reborn: decode native offset/sentinel arrays for exact relocation and import assertions. */
    //-------------------------------------------------------------------------------------------------
    private static uint[] Words(byte[] data) => Enumerable.Range(0, data.Length / 4).Select(index => Read(data, index * 4)).ToArray();

    //-------------------------------------------------------------------------------------------------
    /** Reborn: check all four optional masks, absent versus explicit empty values and schema-negative input independently of stock literals. */
    //-------------------------------------------------------------------------------------------------
    private static void CheckOptionalMasks(DocumentProcessor processor, string source)
    {
        const string prefix = "<AssetDeclaration xmlns=\"uri:ea.com:eala:asset\"><FXList id=\"Synthetic\"><NuggetList><Sound Value=\"SyntheticAudio\" ";
        const string suffix = "/></NuggetList></FXList></AssetDeclaration>";
        // Reborn: these are independent native fixtures, not an unnotified resident-cache reload test.
        string directory = Path.GetDirectoryName(source)!;
        source = Path.Combine(directory, "all-masks.xml");
        string fields = "RequiredSecondaryModelConditions=\"USER_1\" ExcludedSecondaryModelConditions=\"USER_2\" RequiredSourceModelConditions=\"USER_3\" ExcludedSourceModelConditions=\"USER_4\"";
        File.WriteAllText(source, prefix + fields + suffix);
        InstanceDeclaration instance = processor.ProcessDocumentInternal(source, source, null!,
            new DocumentProcessor.ProcessOptions { GenerateOutput = false, UsePrecompiled = false }).SelfInstances.Single();
        Chunk data = Compile(instance);
        Require(data.InstanceBuffer.Length == 320 && Words(data.RelocationBuffer).SequenceEqual(new uint[] { 20, 28, 36, 40, 44, 48, uint.MaxValue })
            && Words(data.ImportsBuffer).SequenceEqual(new uint[] { 76, uint.MaxValue }), "Four optional FX masks did not keep the 44-byte base or relocation order.");
        int[] flags = { (int)ModelConditionFlagType.USER_1, (int)ModelConditionFlagType.USER_2, (int)ModelConditionFlagType.USER_3, (int)ModelConditionFlagType.USER_4 };
        for (int index = 0; index < 4; index++)
        {
            int pointer = 80 + index * 60;
            Require(Read(data.InstanceBuffer, 36 + index * 4) == pointer
                && Read(data.InstanceBuffer, pointer + flags[index] / 32 * 4) == (1u << (flags[index] % 32)), "Optional FX mask pointer or EP1 flag bit differs.");
        }
        source = Path.Combine(directory, "empty-mask.xml");
        File.WriteAllText(source, prefix + "RequiredSecondaryModelConditions=\"\"" + suffix);
        Chunk empty = Compile(processor.ProcessDocumentInternal(source, source, null!,
            new DocumentProcessor.ProcessOptions { GenerateOutput = false, UsePrecompiled = false }).SelfInstances.Single());
        Require(empty.InstanceBuffer.Length == 140 && Read(empty.InstanceBuffer, 36) == 80
            && empty.InstanceBuffer.AsSpan(80, 60).ToArray().All(value => value == 0), "Explicit empty mask collapsed into an absent pointer.");
        source = Path.Combine(directory, "bad-weather.xml");
        File.WriteAllText(source, prefix + "Weather=\"WRONG\"" + suffix);
        bool rejected = false;
        try { processor.ProcessDocumentInternal(source, source, null!, new DocumentProcessor.ProcessOptions { GenerateOutput = false, UsePrecompiled = false }); }
        catch (BinaryAssetBuilderException error) when (error.ErrorCode == ErrorCode.SchemaValidation) { rejected = true; }
        Require(rejected, "Invalid FX weather passed official validation.");
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: pin the two locally extracted FX dependency enums to the official BaseModules definitions without importing unrelated module layouts. */
    //-------------------------------------------------------------------------------------------------
    internal static void CheckExtractedEnums(string fixtures, string schemaName = "FXListPipeline.xsd")
    {
        XmlDocument official = new(), focused = new();
        official.Load(Path.GetFullPath(Path.Combine(fixtures, "../../schemas/ra3ep1/xsd/Modules/BaseModules.xsd")));
        focused.Load(Path.Combine(fixtures, schemaName));
        foreach (string name in new[] { "FXTriggerType", "FXActionType" })
        {
            string query = "//*[local-name()='simpleType' and @name='" + name + "']/*[local-name()='restriction']/*[local-name()='enumeration']/@value";
            string[] expected = official.SelectNodes(query)!.Cast<XmlAttribute>().Select(attribute => attribute.Value).ToArray();
            string[] actual = focused.SelectNodes(query)!.Cast<XmlAttribute>().Select(attribute => attribute.Value).ToArray();
            Require(expected.Length != 0 && expected.SequenceEqual(actual), "Focused FX enum drifted from official values/order: " + name);
        }
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: schema-free callers must receive the official INVALID weather enum, not the old unrecognized WEATHER token. */
    //-------------------------------------------------------------------------------------------------
    private static unsafe void CheckWeatherFallback()
    {
        XmlDocument xml = new(); xml.LoadXml("<FXNugget xmlns=\"uri:ea.com:eala:asset\" />");
        XmlNamespaceManager namespaces = new(xml.NameTable); namespaces.AddNamespace("ea", "uri:ea.com:eala:asset");
        Node node = new(xml.DocumentElement!.CreateNavigator()!, namespaces);
        FXNugget* root;
        using Tracker tracker = new((void**)&root, (uint)sizeof(FXNugget), false);
        Marshaler.Marshal(node, root, tracker); Chunk data = new();
        Require(tracker.MakeRelocatable(data) && data.InstanceBuffer.Length == 44 && Read(data.InstanceBuffer, 20) == 6
            && data.RelocationBuffer.Length == 0 && data.ImportsBuffer.Length == 0, "Schema-free FX weather fallback differs.");
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: stop FX native recovery at the first layout, identity or stream-slice mismatch. */
    //-------------------------------------------------------------------------------------------------
    private static void Require(bool condition, string message)
    {
        if (!condition) throw new InvalidDataException(message);
    }
}
