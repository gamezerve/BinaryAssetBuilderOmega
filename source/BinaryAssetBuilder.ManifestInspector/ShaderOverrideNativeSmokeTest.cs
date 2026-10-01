using System.Buffers.Binary;
using System.Xml;
using System.Xml.Schema;
using BinaryAssetBuilder.Core;
using BinaryAssetBuilder.Core.SageXml;
using BinaryAssetBuilder.Core.Session;
using Relo;
using SageBinaryData;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: prove the recovered ShaderOverride ABI and material POID/string records without production registration or shader compilation.
internal static class ShaderOverrideNativeSmokeTest
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: validate default priority, optional condition pointers, rule stride, technique strings and bounded stock native slices. */
    //-------------------------------------------------------------------------------------------------
    internal static void Run(string[] manifests)
    {
        string fixtures = Path.GetDirectoryName(ReferencePipelineSmokeTest.FindFixture())!;
        XmlSchemaSet schemas = new() { XmlResolver = new XmlUrlResolver() };
        schemas.Add(null, Path.Combine(fixtures, "ShaderOverridePipeline.xsd")); schemas.Compile();
        Chunk defaults = Compile(schemas, """
            <ShaderOverride xmlns="uri:ea.com:eala:asset" id="DefaultShader">
             <Rule ReplaceShaderName="Null.fx" ReplaceTechniqueName="Default" />
            </ShaderOverride>
            """);
        Require(defaults.InstanceBuffer.Length == 40 && defaults.RelocationBuffer.Length == 12 && defaults.ImportsBuffer.Length == 0
            && Read(defaults.InstanceBuffer, 4) == 1 && Read(defaults.InstanceBuffer, 8) == 1 && Read(defaults.InstanceBuffer, 12) == 16
            && Read(defaults.InstanceBuffer, 16) == 0 && Read(defaults.InstanceBuffer, 20) == InstanceHandle.GetInstanceId("Null.fx")
            && Read(defaults.InstanceBuffer, 24) == 7 && Read(defaults.InstanceBuffer, 28) == 32
            && defaults.InstanceBuffer.AsSpan(32, 8).SequenceEqual("Default\0"u8),
            "Recovered shader root/rule/POID/string defaults differ.");
        XmlDocument fixture = new(); fixture.Load(Path.Combine(fixtures, "ShaderOverrideProbe.xml"));
        Dictionary<string, Chunk> goldens = new(StringComparer.Ordinal);
        foreach (XmlElement element in fixture.DocumentElement!.ChildNodes.OfType<XmlElement>())
        {
            Chunk chunk = Compile(schemas, element.OuterXml), repeated = Compile(schemas, element.OuterXml);
            Require(chunk.InstanceBuffer.SequenceEqual(repeated.InstanceBuffer) && chunk.RelocationBuffer.SequenceEqual(repeated.RelocationBuffer)
                && chunk.ImportsBuffer.Length == 0, "Shader native output differs on repeat or generated strong imports.");
            goldens.Add("ShaderOverride:" + element.GetAttribute("id"), chunk);
        }
        Chunk iron = goldens["ShaderOverride:ShaderOverride_ObjectsIronCurtain"];
        Require(goldens.Count == 4 && iron.InstanceBuffer.Length == 180 && iron.RelocationBuffer.Length == 52
            && Read(iron.InstanceBuffer, 4) == 100 && Read(iron.InstanceBuffer, 8) == 6
            && Read(iron.InstanceBuffer, 16) == 112 && Read(iron.InstanceBuffer, 112) == InstanceHandle.GetInstanceId("DefaultW3D.fx")
            && Read(iron.InstanceBuffer, 96) == 0 && Read(iron.InstanceBuffer, 108) == 172,
            "Shader rule order, optional condition or catch-all string offsets differ.");
        Require(goldens["ShaderOverride:ShaderOverride_PsychicCrush"].InstanceBuffer.Length == 460
            && goldens["ShaderOverride:ShaderOverride_BuildPlacementCursor"].InstanceBuffer.Length == 96
            && goldens["ShaderOverride:ShaderOverride_ObjectsWithoutXrayEffect"].InstanceBuffer.Length == 44,
            "Shader one/three/sixteen-rule layouts differ.");
        foreach (string xml in new[]
        {
            """<ShaderOverride xmlns="uri:ea.com:eala:asset" id="Bad" />""",
            """<ShaderOverride xmlns="uri:ea.com:eala:asset" id="Bad" Priority="-1"><Rule ReplaceShaderName="Null.fx" ReplaceTechniqueName="Default" /></ShaderOverride>""",
            """<ShaderOverride xmlns="uri:ea.com:eala:asset" id="Bad"><Rule ReplaceShaderName="Null.fx" /></ShaderOverride>"""
        })
        {
            bool rejected = false;
            try { Compile(schemas, xml); } catch (XmlSchemaException) { rejected = true; }
            Require(rejected, "Invalid shader priority/rule/required string passed schema validation.");
        }
        CheckDocument(fixtures, schemas, goldens);
        Dictionary<string, Chunk> stockGoldens = new(goldens, StringComparer.Ordinal);
        stockGoldens["ShaderOverride:ShaderOverride_PsychicCrush"] = CompileStockPsychicVariant(schemas, fixture,
            goldens["ShaderOverride:ShaderOverride_PsychicCrush"]);
        foreach (string path in manifests) Compare(stockGoldens, path);
        Console.WriteLine("ShaderOverride native self-test: OK (16-byte root/rules, optional material POID pointers, inline replacement IDs, ANSI length/pointer strings, document defaults, four deterministic roots; no production registration)");
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: keep the supplied psychic XML intact and independently compile the explicitly documented stock-only Lightning replacement variant. */
    //-------------------------------------------------------------------------------------------------
    private static Chunk CompileStockPsychicVariant(XmlSchemaSet schemas, XmlDocument fixture, Chunk source)
    {
        XmlElement original = fixture.DocumentElement!.ChildNodes.OfType<XmlElement>()
            .Single(element => element.GetAttribute("id") == "ShaderOverride_PsychicCrush");
        XmlDocument detached = new(); detached.LoadXml(original.OuterXml);
        XmlElement[] rules = detached.DocumentElement!.ChildNodes.OfType<XmlElement>().ToArray();
        Require(rules.Length == 16 && rules[5].GetAttribute("IfOriginalShaderIs") == "Lightning.fx"
            && rules[5].GetAttribute("ReplaceShaderName") == "Null.fx",
            "Supplied psychic source rule no longer matches the documented stock variance.");
        rules[5].SetAttribute("ReplaceShaderName", "Lightning.fx");
        Chunk variant = Compile(schemas, detached.DocumentElement.OuterXml);
        Require(InstanceHandle.GetInstanceId("Null.fx") == 0xA9C4BAA0u
            && InstanceHandle.GetInstanceId("Lightning.fx") == 0x2AD67137u
            && source.InstanceBuffer.Length == 460 && variant.InstanceBuffer.Length == 460
            && Read(source.InstanceBuffer, 100) == 0xA9C4BAA0u && Read(variant.InstanceBuffer, 100) == 0x2AD67137u
            && source.InstanceBuffer.AsSpan(0, 100).SequenceEqual(variant.InstanceBuffer.AsSpan(0, 100))
            && source.InstanceBuffer.AsSpan(104).SequenceEqual(variant.InstanceBuffer.AsSpan(104))
            && source.RelocationBuffer.SequenceEqual(variant.RelocationBuffer)
            && source.ImportsBuffer.Length == 0 && variant.ImportsBuffer.Length == 0
            && original.ChildNodes.OfType<XmlElement>().ElementAt(5).GetAttribute("ReplaceShaderName") == "Null.fx",
            "Psychic stock variant changed more than the documented replacement POID or mutated source XML.");
        return variant;
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: marshal schema-defaulted shader rules using recovered official field/allocation order, never FXShaderMaterial compilation. */
    //-------------------------------------------------------------------------------------------------
    internal static unsafe Chunk Compile(XmlSchemaSet schemas, string xml)
    {
        XmlDocument document = new() { Schemas = schemas, XmlResolver = null };
        document.LoadXml(xml); document.Validate((_, args) => throw new XmlSchemaValidationException(args.Message));
        XmlNamespaceManager namespaces = new(document.NameTable); namespaces.AddNamespace("ea", "uri:ea.com:eala:asset");
        Node node = new(document.DocumentElement!.CreateNavigator(), namespaces);
        ShaderOverride* root;
        using Tracker tracker = new((void**)&root, (uint)sizeof(ShaderOverride), false);
        Marshaler.Marshal(node, root, tracker);
        Chunk chunk = new(); Require(tracker.MakeRelocatable(chunk), "Shader native compilation failed.");
        return chunk;
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: verify actual document normalization does not misclassify material POIDs as strong or weak dependencies. */
    //-------------------------------------------------------------------------------------------------
    private static void CheckDocument(string fixtures, XmlSchemaSet schemas, IReadOnlyDictionary<string, Chunk> expected)
    {
        string directory = Path.Combine(Path.GetTempPath(), "Reborn-ShaderOverrideDocument-" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(directory);
        Settings saved = Settings.Current;
        try
        {
            Settings.Current = new Settings { BuildCache = false, SchemaPath = Path.Combine(fixtures, "ShaderOverridePipeline.xsd"),
                DataRoot = directory, DataPaths = new[] { directory }, TargetPlatform = TargetPlatform.Win32, CustomPostfix = "", StreamPostfix = "",
                StringHashBinDescriptors = Array.Empty<StringHashBinDescriptor>() };
            string source = Path.Combine(directory, "shaders.xml"); File.WriteAllText(source, File.ReadAllText(Path.Combine(fixtures, "ShaderOverrideProbe.xml")));
            SessionCache cache = new(); cache.InitializeCache(new System.Collections.Generic.List<string>());
            PluginRegistry plugins = new(Array.Empty<PluginDescriptor>(), TargetPlatform.Win32);
            DocumentProcessor processor = new(Settings.Current, plugins, new VerifierPluginRegistry(Array.Empty<PluginDescriptor>(), TargetPlatform.Win32))
                { Cache = cache, SchemaSet = new SchemaSet(false) };
            AssetDeclarationDocument document = processor.ProcessDocumentInternal(source, source, null!,
                new DocumentProcessor.ProcessOptions { GenerateOutput = false, UsePrecompiled = false });
            Require(document.SelfInstances.Count == 4, "Shader document root count differs.");
            foreach (InstanceDeclaration instance in document.SelfInstances)
            {
                Require(instance.Handle.TypeHash == 0 && instance.ReferencedInstances.Count == 0 && instance.WeakReferencedInstances.Count == 0
                    && instance.ReferencedFiles.Count == 0, "Shader POID unexpectedly became a dependency or registered root.");
                XmlDocument copy = new(); copy.LoadXml(instance.XmlNode.OuterXml);
                foreach (XmlElement element in copy.DocumentElement!.SelectNodes("descendant-or-self::*")!) element.RemoveAttribute("TypeId");
                Chunk actual = Compile(schemas, copy.DocumentElement.OuterXml), golden = expected[instance.Handle.Name];
                Require(actual.InstanceBuffer.SequenceEqual(golden.InstanceBuffer) && actual.RelocationBuffer.SequenceEqual(golden.RelocationBuffer)
                    && actual.ImportsBuffer.Length == 0, "Shader document/default/hash stages changed native output.");
            }
            Require(!Directory.EnumerateFiles(directory, "*.manifest").Any(), "Shader diagnostic emitted production output.");
        }
        finally { Settings.Current = saved; }
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: compare every selected native shader root with bounded stock chunks after EP1/header/fingerprint checks. */
    //-------------------------------------------------------------------------------------------------
    private static void Compare(IReadOnlyDictionary<string, Chunk> expected, string path)
    {
        ManifestDocument manifest = ManifestReader.Read(File.ReadAllBytes(path));
        TypeRegistryAudit.ValidateTarget(manifest.Header.Version, manifest.Header.AllTypesHash);
        Require(manifest.Header.IsLinked && manifest.Validate().Count == 0, "Invalid linked shader golden manifest.");
        foreach (var stream in new[] { (Extension: ".bin", Magic: 0xBABB0000u), (Extension: ".relo", Magic: 0xBABE0000u), (Extension: ".imp", Magic: 0xBAB10000u) })
        {
            byte[] header = AssetStreamProbe.ReadRange(Path.ChangeExtension(path, stream.Extension), null, 0, 8);
            Require(Read(header, 0) == stream.Magic && Read(header, 4) == manifest.Header.StreamChecksum, "Shader stream header/checksum differs.");
        }
        long bin = manifest.Header.ContainerPrefixSize + 4, relo = bin, imp = bin; int matches = 0;
        foreach (ManifestAsset asset in manifest.Assets)
        {
            if (expected.TryGetValue(asset.Name, out Chunk? chunk))
            {
                Require(asset.TypeId == 0xBCC23F6Cu && asset.TypeHash == 0x3D5B1D16u && asset.Tokenized == 0 && asset.References.Count == 0
                    && asset.InstanceDataSize == chunk.InstanceBuffer.Length && asset.RelocationDataSize == chunk.RelocationBuffer.Length && asset.ImportsDataSize == 0,
                    $"Shader golden fingerprint/size differs: {asset.Name}");
                byte[] actual = AssetStreamProbe.ReadRange(Path.ChangeExtension(path, ".bin"), null, bin, asset.InstanceDataSize);
                // Reborn: report the first bounded mismatch instead of silently rewriting source-derived rules to fit the game.
                if (!chunk.InstanceBuffer.SequenceEqual(actual))
                {
                    int difference = Enumerable.Range(0, actual.Length).First(index => actual[index] != chunk.InstanceBuffer[index]);
                    int word = difference / 4 * 4;
                    string changedWords = string.Join(", ", Enumerable.Range(0, actual.Length / 4).Select(index => index * 4)
                        .Where(offset => Read(actual, offset) != Read(chunk.InstanceBuffer, offset)).Take(16)
                        .Select(offset => $"0x{offset:X}:{Read(chunk.InstanceBuffer, offset):X8}->{Read(actual, offset):X8}"));
                    throw new InvalidDataException($"Shader native golden differs: {asset.Name}; first byte 0x{difference:X}, word 0x{word:X}: source=0x{Read(chunk.InstanceBuffer, word):X8}, stock=0x{Read(actual, word):X8}; changed words [{changedWords}]");
                }
                Require(chunk.InstanceBuffer.SequenceEqual(actual)
                    && chunk.RelocationBuffer.SequenceEqual(AssetStreamProbe.ReadRange(Path.ChangeExtension(path, ".relo"), null, relo, asset.RelocationDataSize)),
                    $"Shader native golden differs: {asset.Name}");
                string provenance = asset.Name == "ShaderOverride:ShaderOverride_PsychicCrush"
                    ? "explicit stock variant (source differs at 0x64: Null.fx -> Lightning.fx)" : "supplied source literal";
                Console.WriteLine($"  Real EP1 shader golden: {asset.Name} exact {asset.InstanceDataSize}/{asset.RelocationDataSize}/0; {provenance}"); matches++;
            }
            bin = checked(bin + asset.InstanceDataSize); relo = checked(relo + asset.RelocationDataSize); imp = checked(imp + asset.ImportsDataSize);
        }
        Require(matches == expected.Count, "Supplied manifest lacks one or more supported shader goldens.");
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: decode bounded little-endian shader words without relying on native host pointers. */
    //-------------------------------------------------------------------------------------------------
    private static uint Read(byte[] data, int offset) => BinaryPrimitives.ReadUInt32LittleEndian(data.AsSpan(offset, 4));

    //-------------------------------------------------------------------------------------------------
    /** Reborn: fail native recovery proof on its first schema/layout/POID/string mismatch. */
    //-------------------------------------------------------------------------------------------------
    private static void Require(bool condition, string message)
    {
        if (!condition) throw new InvalidDataException(message);
    }
}
