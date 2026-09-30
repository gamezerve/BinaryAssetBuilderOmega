using System.Buffers.Binary;
using System.Reflection;
using System.Text;
using System.Xml;
using System.Xml.Schema;
using BinaryAssetBuilder.Core;
using BinaryAssetBuilder.Core.Hashing;
using BinaryAssetBuilder.XmlCompiler;
using Relo;
using SageBinaryData;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: fixture-only EP1 tokenization/writer proof; production output and game compatibility remain gated.
internal static class ArmorTokenSmokeTest
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: validate defaults, token offsets, negative cases, linked streams and an optional real-game golden slice. */
    //-------------------------------------------------------------------------------------------------
    public static void Run(string outputDirectory, string? gameManifest = null)
    {
        string fixtureDirectory = Path.GetDirectoryName(ReferencePipelineSmokeTest.FindFixture())!;
        XmlSchemaSet schemas = LoadSchemas(fixtureDirectory);
        Chunk empty = Compile(schemas, """<ArmorTemplate xmlns="uri:ea.com:eala:asset" id="Defaults" />""");
        Require(empty.InstanceBuffer.Length == 128 && empty.RelocationBuffer.Length == 0, "Empty armor token tree size/relocations differ.");
        for (int index = 0; index < 5; index++)
            Require(Read(empty.InstanceBuffer, index * 12 + 8) == 100 + index * 4
                && Read(empty.InstanceBuffer, 100 + index * 4) == 0x3F800000u, "Schema-inserted percentage default differs.");
        Require(Read(empty.InstanceBuffer, 68) == 84 && Read(empty.InstanceBuffer, 92) == 120, "Empty list table/count offsets differ.");

        string xml = File.ReadAllText(Path.Combine(fixtureDirectory, "ArmorTokenProbe.xml"));
        Chunk populated = Compile(schemas, xml);
        Require(populated.InstanceBuffer.Length == 744 && populated.RelocationBuffer.SequenceEqual(new byte[] { 0x8C, 2, 0, 0, 255, 255, 255, 255 })
            && populated.ImportsBuffer.Length == 0, "Eleven-item armor fixture must produce 744/8/0.");
        Chunk repeated = Compile(schemas, xml);
        Require(populated.InstanceBuffer.SequenceEqual(repeated.InstanceBuffer)
            && populated.RelocationBuffer.SequenceEqual(repeated.RelocationBuffer), "Armor tokenization is not deterministic.");
        Chunk scalar = Compile(schemas, """<ArmorTemplate xmlns="uri:ea.com:eala:asset" id="Scalars" Default="50%" DamageScalar="25" SideDamageScalar="75%" RearDamageScalar="200" FlankedPenalty="10%" />""");
        Require(Read(scalar.InstanceBuffer, 100) == BitConverter.SingleToUInt32Bits(0.5f)
            && Read(scalar.InstanceBuffer, 104) == BitConverter.SingleToUInt32Bits(0.25f)
            && Read(scalar.InstanceBuffer, 108) == BitConverter.SingleToUInt32Bits(0.75f)
            && Read(scalar.InstanceBuffer, 112) == BitConverter.SingleToUInt32Bits(2f)
            && Read(scalar.InstanceBuffer, 116) == BitConverter.SingleToUInt32Bits(10f * 0.01f), "Non-default scalar normalization differs.");
        // Reborn: the harness must validate against the official enum/restriction, not accept arbitrary XML strings.
        foreach (string invalid in new[]
        {
            """<ArmorTemplate xmlns="uri:ea.com:eala:asset" id="Bad" Default="garbage" />""",
            """<ArmorTemplate xmlns="uri:ea.com:eala:asset" id="Bad"><Armor Damage="NOT_A_DAMAGE_TYPE" Percent="5" /></ArmorTemplate>"""
        })
        {
            bool rejected = false;
            try { Compile(schemas, invalid); }
            catch (XmlSchemaException) { rejected = true; }
            Require(rejected, "Invalid armor XML was accepted by the schema harness.");
        }
        Chunk malformed = new() { InstanceBuffer = new byte[32], ImportsBuffer = new byte[4] };
        ExpectRejected(malformed);
        malformed.ImportsBuffer = Array.Empty<byte>();
        BinaryPrimitives.WriteUInt32LittleEndian(malformed.InstanceBuffer.AsSpan(28), 32);
        ExpectRejected(malformed);
        BinaryPrimitives.WriteUInt32LittleEndian(malformed.InstanceBuffer.AsSpan(24), uint.MaxValue);
        ExpectRejected(malformed);

        // Reborn: WorldBuilder's local stream contains a different armor subset, not the static stream's A01 asset.
        if (gameManifest != null) CompareGolden(new Dictionary<string, Chunk>
        {
            ["ArmorTemplate:A01_CoastalGunArmor"] = populated,
            ["ArmorTemplate:TH_ShinzoHouseArmorSet"] = Compile(schemas,
                """<ArmorTemplate xmlns="uri:ea.com:eala:asset" id="HouseProbe" Default="10" />""")
        }, gameManifest);
        WriteFixture(outputDirectory, populated);
        Console.WriteLine($"Armor token self-test: OK (744/8/0, schema defaults, scalar values, offset tokens, determinism, linked fixture); output={Path.GetFullPath(outputDirectory)}");
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: preserve the official Percentage restriction and armor/damage declarations in a minimal schema graph. */
    //-------------------------------------------------------------------------------------------------
    internal static XmlSchemaSet LoadSchemas(string fixtureDirectory)
    {
        string basePath = Path.GetFullPath(Path.Combine(fixtureDirectory, "../../schemas/ra3ep1/xsd/Includes/Base.xsd"));
        using XmlReader reader = XmlReader.Create(basePath, new XmlReaderSettings { DtdProcessing = DtdProcessing.Prohibit });
        XmlSchema official = XmlSchema.Read(reader, (_, args) => throw new XmlSchemaException(args.Message))!;
        XmlSchema primitives = new() { TargetNamespace = "uri:ea.com:eala:asset" };
        primitives.Items.Add(official.Items.OfType<XmlSchemaSimpleType>().Single(type => type.Name == "Percentage"));
        XmlSchemaSet schemas = new() { XmlResolver = new XmlUrlResolver() };
        schemas.Add(primitives);
        schemas.Add(null, Path.Combine(fixtureDirectory, "ArmorTokenPipeline.xsd"));
        schemas.Compile();
        return schemas;
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: feed schema-inserted defaults to the production marshaller before experimental tokenization. */
    //-------------------------------------------------------------------------------------------------
    internal static unsafe Chunk Compile(XmlSchemaSet schemas, string xml)
    {
        XmlDocument document = new() { Schemas = schemas, XmlResolver = null };
        document.LoadXml(xml);
        document.Validate((_, args) => throw new XmlSchemaValidationException(args.Message));
        XmlNamespaceManager namespaces = new(document.NameTable);
        namespaces.AddNamespace("ea", "uri:ea.com:eala:asset");
        Node node = new(document.CreateNavigator()!.SelectSingleNode("/ea:ArmorTemplate", namespaces)!, namespaces);
        ArmorTemplate* root;
        using Tracker tracker = new((void**)&root, (uint)sizeof(ArmorTemplate), false);
        Marshaler.Marshal(node, root, tracker);
        Chunk native = new();
        Require(tracker.MakeRelocatable(native), "Native armor marshalling failed.");
        // Reborn: tokenization must not corrupt the reusable native chunk or its relocation metadata.
        byte[] original = (byte[])native.InstanceBuffer.Clone();
        byte[] originalRelocations = (byte[])native.RelocationBuffer.Clone();
        Chunk result = Ep1ArmorTokenizer.Tokenize(native);
        Require(native.InstanceBuffer.SequenceEqual(original) && native.RelocationBuffer.SequenceEqual(originalRelocations),
            "Armor tokenizer mutated its native input.");
        return result;
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: compare known tiny golden slices and stream headers, never the full static/WorldBuilder BIN. */
    //-------------------------------------------------------------------------------------------------
    private static void CompareGolden(IReadOnlyDictionary<string, Chunk> goldens, string path)
    {
        ManifestDocument manifest = ManifestReader.Read(File.ReadAllBytes(path));
        TypeRegistryAudit.ValidateTarget(manifest.Header.Version, manifest.Header.AllTypesHash);
        Require(manifest.Validate().Count == 0 && manifest.Header.IsLinked, "Golden manifest metadata must be valid and linked.");
        foreach (var stream in new[] { (Extension: ".bin", Magic: 0xBABB0000u), (Extension: ".relo", Magic: 0xBABE0000u), (Extension: ".imp", Magic: 0xBAB10000u) })
        {
            byte[] header = AssetStreamProbe.ReadRange(Path.ChangeExtension(path, stream.Extension), null, 0, 8);
            Require(Read(header, 0) == stream.Magic && Read(header, 4) == manifest.Header.StreamChecksum,
                "Golden linked stream header does not match the manifest.");
        }
        long binOffset = manifest.Header.ContainerPrefixSize + 4;
        long reloOffset = binOffset;
        int matches = 0;
        foreach (var asset in manifest.Assets)
        {
            if (goldens.TryGetValue(asset.Name, out Chunk? expected))
            {
                Require(asset.TypeId == 0x3A6C5E8Eu && asset.TypeHash == 0xA0E237D8u && asset.Tokenized == 1
                    && asset.InstanceDataSize == expected.InstanceBuffer.Length && asset.RelocationDataSize == expected.RelocationBuffer.Length
                    && asset.ImportsDataSize == 0, "Golden asset fingerprint or bounded slice size differs.");
                byte[] actual = AssetStreamProbe.ReadRange(Path.ChangeExtension(path, ".bin"), null, binOffset, expected.InstanceBuffer.Length);
                byte[] relocations = AssetStreamProbe.ReadRange(Path.ChangeExtension(path, ".relo"), null, reloOffset, expected.RelocationBuffer.Length);
                Require(expected.InstanceBuffer.SequenceEqual(actual) && expected.RelocationBuffer.SequenceEqual(relocations), "Armor fixture differs from the EP1 game golden chunk.");
                Console.WriteLine($"  Real EP1 golden: exact {actual.Length} BIN + {relocations.Length} RELO byte match; {path}::{asset.Name}");
                matches++;
            }
            binOffset = checked(binOffset + asset.InstanceDataSize);
            reloOffset = checked(reloOffset + asset.RelocationDataSize);
        }
        Require(matches != 0, "Golden manifest does not contain a supported armor golden asset.");
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: serialize an isolated one-asset diagnostic with production header writers, not CommitManifest bypasses. */
    //-------------------------------------------------------------------------------------------------
    private static void WriteFixture(string directory, Chunk chunk)
    {
        Directory.CreateDirectory(directory);
        string path = Path.Combine(directory, "armor-token.manifest");
        const uint checksum = 0xA11CE001u;
        byte[] assetName = Encoding.UTF8.GetBytes("ArmorTemplate:Ep1ArmorTokenProbe\0");
        byte[] sourceName = Encoding.UTF8.GetBytes("Tests/ArmorTokenProbe.xml\0");
        using MemoryStream metadata = new();
        using (var header = new BinaryAssetBuilder.Utility.ManifestHeader
        {
            IsLinked = true, StreamChecksum = checksum, AllTypesHash = 0x5454A8E9u, AssetCount = 1,
            TotalInstanceDataSize = (uint)chunk.InstanceBuffer.Length, MaxInstanceChunkSize = (uint)chunk.InstanceBuffer.Length,
            MaxRelocationChunkSize = (uint)chunk.RelocationBuffer.Length, MaxImportsChunkSize = 0,
            AssetNameBufferSize = (uint)assetName.Length, SourceFileNameBufferSize = (uint)sourceName.Length
        }) header.SaveToStream(metadata, false);
        using (var entry = new BinaryAssetBuilder.Utility.AssetEntry
        {
            TypeId = 0x3A6C5E8Eu, TypeHash = 0xA0E237D8u, InstanceId = InstanceHandle.GetInstanceId("Ep1ArmorTokenProbe"),
            InstanceHash = 0, Tokenized = true, InstanceDataSize = chunk.InstanceBuffer.Length,
            RelocationDataSize = chunk.RelocationBuffer.Length, ImportsDataSize = 0
        }) entry.SaveToStream(metadata, false);
        metadata.Write(assetName);
        metadata.Write(sourceName);
        File.WriteAllBytes(path, metadata.ToArray());
        MethodInfo headerWriter = typeof(OutputManager).GetMethod("WriteLinkedStreamHeader", BindingFlags.NonPublic | BindingFlags.Static)!;
        foreach (var stream in new[] { (Extension: ".bin", Magic: 0xBABB0000u, Bytes: chunk.InstanceBuffer), (Extension: ".relo", Magic: 0xBABE0000u, Bytes: chunk.RelocationBuffer), (Extension: ".imp", Magic: 0xBAB10000u, Bytes: chunk.ImportsBuffer) })
        {
            using MemoryStream bytes = new();
            using BinaryWriter writer = new(bytes, Encoding.UTF8, true);
            headerWriter.Invoke(null, new object[] { writer, checksum, stream.Magic });
            writer.Write(stream.Bytes);
            writer.Flush();
            File.WriteAllBytes(Path.ChangeExtension(path, stream.Extension), bytes.ToArray());
            Require(Read(bytes.ToArray(), 0) == stream.Magic && Read(bytes.ToArray(), 4) == checksum, "Linked stream header differs.");
        }
        ManifestDocument parsed = ManifestReader.Read(File.ReadAllBytes(path));
        Require(parsed.Validate().Count == 0 && parsed.Assets.Single().Tokenized == 1 && parsed.Assets.Single().TypeHash == 0xA0E237D8u, "Fixture manifest round trip failed.");
        using var loaded = new BinaryAssetBuilder.Utility.Manifest();
        Require(loaded.Load(path, false) && loaded.AssetCount == 1 && loaded.Assets.Single().Tokenized
            && loaded.MaxInstanceChunkSize == 744 && loaded.MaxRelocationChunkSize == 8 && loaded.MaxImportsChunkSize == 0,
            "Production utility reader rejected armor fixture or mixed stream maxima.");
        // Reborn: make the scope explicit alongside generated files so diagnostic metadata is not mistaken for a playable mod.
        File.WriteAllText(Path.Combine(directory, "DIAGNOSTIC_ONLY.txt"),
            "Armor tokenization and linked-stream fixture only. Not a playable Uprising mod.\n"
            + "Production registry/gate unchanged; checksum is a fixed test marker and InstanceHash is zero.\n"
            + "No packaging, inheritance, cache-hash policy or in-game loading is validated by this fixture.\n");
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: malformed native layouts must fail before offset-token output is emitted. */
    //-------------------------------------------------------------------------------------------------
    private static void ExpectRejected(Chunk chunk)
    {
        try { Ep1ArmorTokenizer.Tokenize(chunk); }
        catch (Exception exception) when (exception is InvalidDataException or OverflowException) { return; }
        throw new InvalidDataException("Malformed armor native input was accepted.");
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: little-endian word reads keep tests independent of culture and float formatting. */
    //-------------------------------------------------------------------------------------------------
    private static uint Read(byte[] bytes, int offset) => BinaryPrimitives.ReadUInt32LittleEndian(bytes.AsSpan(offset));

    //-------------------------------------------------------------------------------------------------
    /** Reborn: stop at the first mismatch so the fixture cannot claim a false successful migration. */
    //-------------------------------------------------------------------------------------------------
    private static void Require(bool condition, string message)
    {
        if (!condition) throw new InvalidDataException(message);
    }
}
