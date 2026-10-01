using System.Buffers.Binary;
using System.Xml;
using System.Xml.Schema;
using BinaryAssetBuilder.Core;
using BinaryAssetBuilder.Core.SageXml;
using BinaryAssetBuilder.Core.Session;
using Relo;
using SageBinaryData;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: establish native ObjectFilter root proof using official schema defaults and bounded real-game slices, without registry activation.
internal static class ObjectFilterNativeSmokeTest
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: validate inline filter layout, ordered four-byte weak-name lists, determinism and optional stock golden output. */
    //-------------------------------------------------------------------------------------------------
    internal static void Run(string[] manifests)
    {
        string fixtures = Path.GetDirectoryName(ReferencePipelineSmokeTest.FindFixture())!;
        XmlSchemaSet schemas = new() { XmlResolver = new XmlUrlResolver() };
        schemas.Add(null, Path.Combine(fixtures, "ObjectFilterPipeline.xsd"));
        schemas.Compile();
        Chunk empty = Compile(schemas, """<ObjectFilterAsset xmlns="uri:ea.com:eala:asset" id="DefaultFilter"><Filter /></ObjectFilterAsset>""");
        Require(empty.InstanceBuffer.Length == 124 && empty.RelocationBuffer.Length == 0 && empty.ImportsBuffer.Length == 0
            && Read(empty.InstanceBuffer, 8) == 3 && Read(empty.InstanceBuffer, 12) == 0 && Read(empty.InstanceBuffer, 16) == 0,
            "Default inline ObjectFilter root, rule, relationship or alignment differs.");
        XmlDocument fixture = new();
        fixture.Load(Path.Combine(fixtures, "ObjectFilterProbe.xml"));
        Dictionary<string, Chunk> goldens = new(StringComparer.Ordinal);
        foreach (XmlElement element in fixture.DocumentElement!.ChildNodes.OfType<XmlElement>())
        {
            Chunk chunk = Compile(schemas, element.OuterXml), repeated = Compile(schemas, element.OuterXml);
            Require(chunk.InstanceBuffer.SequenceEqual(repeated.InstanceBuffer) && chunk.RelocationBuffer.SequenceEqual(repeated.RelocationBuffer)
                && chunk.ImportsBuffer.Length == 0, "Weak filter IDs generated imports or nondeterministic native output.");
            goldens.Add("ObjectFilterAsset:" + element.GetAttribute("id"), chunk);
        }
        Require(goldens.Count == 11, "Source-derived filter fixture count differs.");
        // Reborn: weak metadata normalization must preserve the same native IDs as source-derived standalone compilation.
        CheckDocumentNormalization(fixtures, schemas, goldens);
        Chunk enter = goldens["ObjectFilterAsset:InfiltrationCanEnterObjectFilter"];
        Require(enter.InstanceBuffer.Length == 132 && Read(enter.InstanceBuffer, 12) == 2
            && Read(enter.InstanceBuffer, 108) == 2 && Read(enter.InstanceBuffer, 112) == 124
            && Read(enter.InstanceBuffer, 124) == InstanceHandle.GetInstanceId("AlliedInfiltrationInfantry")
            && Read(enter.InstanceBuffer, 128) == InstanceHandle.GetInstanceId("JapanInfiltrationInfantry")
            && enter.RelocationBuffer.SequenceEqual(new byte[] { 112, 0, 0, 0, 255, 255, 255, 255 }),
            "Weak list stride/order/hash or its pointer relocation differs.");
        Require(goldens["ObjectFilterAsset:InfiltrationEnergyStructuresFilter"].InstanceBuffer.Length == 140
            && goldens["ObjectFilterAsset:InfiltrationParalyzeObjectFilter"].InstanceBuffer.Length == 184,
            "Four/15-entry weak lists do not follow the native stride.");
        bool rejected = false;
        try { Compile(schemas, """<ObjectFilterAsset xmlns="uri:ea.com:eala:asset" id="Bad"><Filter Rule="INVALID" /></ObjectFilterAsset>"""); }
        catch (XmlSchemaException) { rejected = true; }
        Require(rejected, "Invalid filter enum passed official schema validation.");
        // Reborn: synthetic optional status/exclusion coverage supplements but does not replace stock golden evidence.
        CheckOptionalFields(schemas);
        foreach (string path in manifests) Compare(goldens, path);
        Console.WriteLine("ObjectFilter native self-test: OK (124-byte inline root, schema defaults, kind masks, ordered weak IDs, eleven deterministic fixtures; no production registration)");
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: insert current official defaults before native marshalling; do not infer a complete inherited/dependency-aware processor. */
    //-------------------------------------------------------------------------------------------------
    internal static unsafe Chunk Compile(XmlSchemaSet schemas, string xml)
    {
        XmlDocument document = new() { Schemas = schemas, XmlResolver = null };
        document.LoadXml(xml);
        document.Validate((_, args) => throw new XmlSchemaValidationException(args.Message));
        XmlNamespaceManager namespaces = new(document.NameTable);
        namespaces.AddNamespace("ea", "uri:ea.com:eala:asset");
        Node node = new(document.DocumentElement!.CreateNavigator(), namespaces);
        ObjectFilterAsset* root;
        using Tracker tracker = new((void**)&root, (uint)sizeof(ObjectFilterAsset), false);
        Marshaler.Marshal(node, root, tracker);
        Chunk chunk = new();
        Require(tracker.MakeRelocatable(chunk), "Native ObjectFilter marshalling failed.");
        return chunk;
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: use real schema/default/weak-reference document stages while keeping the root unregistered and all production writes disabled. */
    //-------------------------------------------------------------------------------------------------
    private static void CheckDocumentNormalization(string fixtures, XmlSchemaSet schemas, IReadOnlyDictionary<string, Chunk> expected)
    {
        string directory = Path.Combine(Path.GetTempPath(), "Reborn-ObjectFilterDocument-" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(directory);
        Settings previous = Settings.Current;
        try
        {
            Settings.Current = new Settings { BuildCache = false, SchemaPath = Path.Combine(fixtures, "ObjectFilterPipeline.xsd"),
                DataRoot = directory, DataPaths = new[] { directory }, TargetPlatform = TargetPlatform.Win32,
                CustomPostfix = "", StreamPostfix = "", StringHashBinDescriptors = Array.Empty<StringHashBinDescriptor>() };
            string source = Path.Combine(directory, "filters.xml");
            File.WriteAllText(source, File.ReadAllText(Path.Combine(fixtures, "ObjectFilterProbe.xml")));
            SessionCache cache = new();
            cache.InitializeCache(new System.Collections.Generic.List<string>());
            PluginRegistry plugins = new(Array.Empty<PluginDescriptor>(), TargetPlatform.Win32);
            DocumentProcessor processor = new(Settings.Current, plugins, new VerifierPluginRegistry(Array.Empty<PluginDescriptor>(), TargetPlatform.Win32))
                { Cache = cache, SchemaSet = new SchemaSet(false) };
            AssetDeclarationDocument document = processor.ProcessDocumentInternal(source, source, null!,
                new DocumentProcessor.ProcessOptions { GenerateOutput = false, UsePrecompiled = false });
            Require(document.SelfInstances.Count == 11, "Core filter document root count differs.");
            foreach (InstanceDeclaration instance in document.SelfInstances)
            {
                Require(instance.Handle.TypeHash == 0 && instance.ReferencedInstances.Count == 0 && instance.ReferencedFiles.Count == 0,
                    "Weak filter document unexpectedly registered a processor or strong/file reference.");
                XmlDocument copy = new();
                copy.LoadXml(instance.XmlNode.OuterXml);
                XmlElement root = copy.DocumentElement!;
                foreach (XmlElement element in root.SelectNodes("descendant-or-self::*")!) element.RemoveAttribute("TypeId");
                string[] weakNames = root.GetElementsByTagName("IncludeThing", root.NamespaceURI).OfType<XmlElement>().Select(element => element.InnerText).ToArray();
                Require(instance.WeakReferencedInstances.Select(handle => handle.InstanceName).SequenceEqual(weakNames)
                    && instance.WeakReferencedInstances.All(handle => handle.TypeName == "GameObject" && handle.InstanceId == InstanceHandle.GetInstanceId(handle.InstanceName)),
                    "Core weak filter identity, type or source order differs.");
                Chunk actual = Compile(schemas, root.OuterXml), golden = expected[instance.Handle.Name];
                Require(actual.InstanceBuffer.SequenceEqual(golden.InstanceBuffer) && actual.RelocationBuffer.SequenceEqual(golden.RelocationBuffer)
                    && actual.ImportsBuffer.Length == 0, "Core-normalized weak filter output differs from stock-tested source compilation.");
            }
            Require(!Directory.EnumerateFiles(directory, "*.manifest").Any(), "Core weak filter proof emitted production output.");
        }
        finally { Settings.Current = previous; }
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: verify optional 32-byte status masks and independent include/exclude weak lists using explicitly synthetic values. */
    //-------------------------------------------------------------------------------------------------
    private static void CheckOptionalFields(XmlSchemaSet schemas)
    {
        Chunk chunk = Compile(schemas, """
            <ObjectFilterAsset xmlns="uri:ea.com:eala:asset" id="OptionalProbe">
             <Filter Rule="ANY" Relationship="SAME_PLAYER" Alignment="GOOD" StatusBitFlags="NO_BRIBE" StatusBitFlagsExclude="UNDER_IRON_CURTAIN">
              <IncludeThing>TestA</IncludeThing><IncludeThing>TestB</IncludeThing><ExcludeThing>TestC</ExcludeThing>
             </Filter>
            </ObjectFilterAsset>
            """);
        Require(chunk.InstanceBuffer.Length == 200 && chunk.RelocationBuffer.Length == 20 && chunk.ImportsBuffer.Length == 0
            && Read(chunk.InstanceBuffer, 8) == 2 && Read(chunk.InstanceBuffer, 12) == 8 && Read(chunk.InstanceBuffer, 16) == 2
            && Read(chunk.InstanceBuffer, 100) == 124 && Read(chunk.InstanceBuffer, 104) == 156
            && Read(chunk.InstanceBuffer, 108) == 2 && Read(chunk.InstanceBuffer, 112) == 188
            && Read(chunk.InstanceBuffer, 116) == 1 && Read(chunk.InstanceBuffer, 120) == 196,
            "Synthetic optional filter masks or separate weak list pointers differ.");
        Require(Read(chunk.InstanceBuffer, 124 + (int)ObjectStatusType.NO_BRIBE / 32 * 4) == 1u << ((int)ObjectStatusType.NO_BRIBE % 32)
            && Read(chunk.InstanceBuffer, 156 + (int)ObjectStatusType.UNDER_IRON_CURTAIN / 32 * 4) == 1u << ((int)ObjectStatusType.UNDER_IRON_CURTAIN % 32)
            && Read(chunk.InstanceBuffer, 188) == InstanceHandle.GetInstanceId("TestA")
            && Read(chunk.InstanceBuffer, 192) == InstanceHandle.GetInstanceId("TestB")
            && Read(chunk.InstanceBuffer, 196) == InstanceHandle.GetInstanceId("TestC"),
            "Synthetic optional mask bits or weak name hashes differ.");
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: verify target/native fingerprint and all stream headers before exact bounded slice comparison of every supported stock filter. */
    //-------------------------------------------------------------------------------------------------
    private static void Compare(IReadOnlyDictionary<string, Chunk> expected, string path)
    {
        ManifestDocument manifest = ManifestReader.Read(File.ReadAllBytes(path));
        TypeRegistryAudit.ValidateTarget(manifest.Header.Version, manifest.Header.AllTypesHash);
        Require(manifest.Header.IsLinked && manifest.Validate().Count == 0, "Invalid linked EP1 golden manifest.");
        foreach (var stream in new[] { (Extension: ".bin", Magic: 0xBABB0000u), (Extension: ".relo", Magic: 0xBABE0000u), (Extension: ".imp", Magic: 0xBAB10000u) })
        {
            byte[] header = AssetStreamProbe.ReadRange(Path.ChangeExtension(path, stream.Extension), null, 0, 8);
            Require(Read(header, 0) == stream.Magic && Read(header, 4) == manifest.Header.StreamChecksum, "Stock filter stream header/checksum differs.");
        }
        long bin = manifest.Header.ContainerPrefixSize + 4, relo = bin, imp = bin;
        int matches = 0;
        foreach (ManifestAsset asset in manifest.Assets)
        {
            if (expected.TryGetValue(asset.Name, out Chunk? chunk))
            {
                Require(asset.TypeId == 0x44A5973Du && asset.TypeHash == 0xDF72B4BAu && asset.Tokenized == 0 && asset.References.Count == 0
                    && asset.InstanceDataSize == chunk.InstanceBuffer.Length && asset.RelocationDataSize == chunk.RelocationBuffer.Length
                    && asset.ImportsDataSize == 0, $"Filter golden native fingerprint or size differs: {asset.Name}");
                Require(chunk.InstanceBuffer.SequenceEqual(AssetStreamProbe.ReadRange(Path.ChangeExtension(path, ".bin"), null, bin, asset.InstanceDataSize))
                    && chunk.RelocationBuffer.SequenceEqual(AssetStreamProbe.ReadRange(Path.ChangeExtension(path, ".relo"), null, relo, asset.RelocationDataSize))
                    && chunk.ImportsBuffer.SequenceEqual(AssetStreamProbe.ReadRange(Path.ChangeExtension(path, ".imp"), null, imp, asset.ImportsDataSize)),
                    $"Native filter golden differs: {asset.Name}");
                Console.WriteLine($"  Real EP1 filter golden: {asset.Name} exact {asset.InstanceDataSize}/{asset.RelocationDataSize}/0");
                matches++;
            }
            bin = checked(bin + asset.InstanceDataSize); relo = checked(relo + asset.RelocationDataSize); imp = checked(imp + asset.ImportsDataSize);
        }
        Require(matches == expected.Count, "Supplied stock manifest lacks supported filter goldens.");
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: decode native Win32 words independently of pointer-sized host reads. */
    //-------------------------------------------------------------------------------------------------
    private static uint Read(byte[] bytes, int offset) => BinaryPrimitives.ReadUInt32LittleEndian(bytes.AsSpan(offset, 4));

    //-------------------------------------------------------------------------------------------------
    /** Reborn: stop root compatibility proof at the first mismatch without approving production registration. */
    //-------------------------------------------------------------------------------------------------
    private static void Require(bool condition, string message)
    {
        if (!condition) throw new InvalidDataException(message);
    }
}
