using System.Buffers.Binary;
using System.Security.Cryptography;
using System.Reflection;
using System.Xml;
using System.Xml.Schema;
using BinaryAssetBuilder.Core;
using BinaryAssetBuilder.XmlCompiler;
using Relo;
using SageBinaryData;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: establish bounded real EP1 native-root proof before inventing any compiler registration or approving production output.
internal static class AttributeModifierNativeSmokeTest
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: validate native defaults, list/mask layouts and optional small real-game byte comparisons. */
    //-------------------------------------------------------------------------------------------------
    internal static void Run(string[] manifests)
    {
        string fixtures = Path.GetDirectoryName(ReferencePipelineSmokeTest.FindFixture())!;
        XmlSchemaSet schemas = new() { XmlResolver = new XmlUrlResolver() };
        schemas.Add(null, Path.Combine(fixtures, "AttributeModifierPipeline.xsd"));
        schemas.Compile();
        Chunk empty = Compile(schemas, """<AttributeModifier xmlns="uri:ea.com:eala:asset" id="DefaultProbe" />""");
        Require(empty.InstanceBuffer.Length == 56 && empty.RelocationBuffer.Length == 0 && empty.ImportsBuffer.Length == 0
            && Read(empty.InstanceBuffer, 32) == 1, "Empty native root/default stacking limit differs.");
        XmlDocument fixture = new();
        fixture.Load(Path.Combine(fixtures, "AttributeModifierProbe.xml"));
        Dictionary<string, Chunk> goldens = new(StringComparer.Ordinal);
        foreach (XmlElement element in fixture.DocumentElement!.ChildNodes.OfType<XmlElement>())
        {
            Chunk chunk = Compile(schemas, element.OuterXml);
            Chunk repeated = Compile(schemas, element.OuterXml);
            Require(chunk.InstanceBuffer.SequenceEqual(repeated.InstanceBuffer)
                && chunk.RelocationBuffer.SequenceEqual(repeated.RelocationBuffer) && chunk.ImportsBuffer.Length == 0,
                "Native modifier compilation was nondeterministic or created unexpected imports.");
            goldens.Add("AttributeModifier:" + element.GetAttribute("id"), chunk);
        }
        Chunk orange = goldens["AttributeModifier:AttributeModifier_RedAlert_Orange"];
        Require(orange.InstanceBuffer.Length == 64 && orange.RelocationBuffer.SequenceEqual(new byte[] { 48, 0, 0, 0, 255, 255, 255, 255 })
            && Read(orange.InstanceBuffer, 44) == 1 && Read(orange.InstanceBuffer, 48) == 56
            && Read(orange.InstanceBuffer, 56) == (uint)AttributeType.EXPERIENCE && Read(orange.InstanceBuffer, 60) == 0x3FC00000,
            "Native 56-byte root, eight-byte modifier stride or percentage conversion differs.");
        Require(goldens["AttributeModifier:Unit_Heroic"].InstanceBuffer.Length == 128
            && goldens["AttributeModifier:Modifier_Cover"].InstanceBuffer.Length == 132,
            "Optional 32-byte object-status or 60-byte model-condition mask layout differs.");
        foreach (string invalid in new[]
        {
            """<AttributeModifier xmlns="uri:ea.com:eala:asset" id="Bad" Category="NOT_A_CATEGORY" />""",
            """<AttributeModifier xmlns="uri:ea.com:eala:asset" id="Bad"><Modifier Type="NOT_A_TYPE" Value="1" /></AttributeModifier>""",
            """<AttributeModifier xmlns="uri:ea.com:eala:asset" id="Bad" ModelConditionsSet="NOT_A_CONDITION" />"""
        })
        {
            bool rejected = false;
            try { Compile(schemas, invalid); } catch (XmlSchemaException) { rejected = true; }
            Require(rejected, "Invalid modifier XML was accepted by the official schema graph.");
        }
        foreach (string manifest in manifests) Compare(goldens, manifest);
        CheckLegacyRevision();
        Console.WriteLine("Attribute modifier native self-test: OK (official defaults/enums, 56-byte root, list stride, optional masks, deterministic native bytes; no production registration)");
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: validate official schema defaults before feeding the unchanged native marshaller, with no tokenization for this EP1 root. */
    //-------------------------------------------------------------------------------------------------
    internal static unsafe Chunk Compile(XmlSchemaSet schemas, string xml)
    {
        XmlDocument document = new() { Schemas = schemas, XmlResolver = null };
        document.LoadXml(xml);
        document.Validate((_, args) => throw new XmlSchemaValidationException(args.Message));
        XmlNamespaceManager namespaces = new(document.NameTable);
        namespaces.AddNamespace("ea", "uri:ea.com:eala:asset");
        Node node = new(document.DocumentElement!.CreateNavigator(), namespaces);
        AttributeModifier* root;
        using Tracker tracker = new((void**)&root, (uint)sizeof(AttributeModifier), false);
        Marshaler.Marshal(node, root, tracker);
        Chunk result = new();
        Require(tracker.MakeRelocatable(result), "Native attribute modifier marshalling failed.");
        return result;
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: native output changes invalidate legacy processor cache identities without pretending KW registrations are EP1. */
    //-------------------------------------------------------------------------------------------------
    private static void CheckLegacyRevision()
    {
        var map = (IDictionary<uint, ExtendedTypeInformation>)typeof(Plugin)
            .GetField("_extendedTypeInformations", BindingFlags.NonPublic | BindingFlags.Static)!.GetValue(null)!;
        var saved = map.ToArray();
        try
        {
            Plugin plugin = new();
            plugin.ReInitialize(TargetPlatform.Win32);
            ExtendedTypeInformation info = plugin.GetExtendedTypeInformation(0xC5E07887u);
            Require(plugin.VersionNumber == 4 && info.ProcessingHash == (0x8C925761u ^ 4u)
                && info.TypeHash == 0x8C925761u && plugin.AllTypesHash == 0x12B3E763u,
                "Legacy processor revision or unchanged KW type identity policy differs.");
        }
        finally { map.Clear(); foreach (var entry in saved) map.Add(entry.Key, entry.Value); }
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: seek only named tiny asset slices, checking target/fingerprints and all linked headers before claiming a golden match. */
    //-------------------------------------------------------------------------------------------------
    private static void Compare(IReadOnlyDictionary<string, Chunk> goldens, string path)
    {
        byte[] metadata = File.ReadAllBytes(path);
        ManifestDocument manifest = ManifestReader.Read(metadata);
        TypeRegistryAudit.ValidateTarget(manifest.Header.Version, manifest.Header.AllTypesHash);
        Require(manifest.Header.IsLinked && manifest.Validate().Count == 0, "Invalid/unlinked EP1 manifest supplied.");
        foreach (var stream in new[] { (Extension: ".bin", Magic: 0xBABB0000u), (Extension: ".relo", Magic: 0xBABE0000u), (Extension: ".imp", Magic: 0xBAB10000u) })
        {
            byte[] header = AssetStreamProbe.ReadRange(Path.ChangeExtension(path, stream.Extension), null, 0, 8);
            Require(Read(header, 0) == stream.Magic && Read(header, 4) == manifest.Header.StreamChecksum,
                "Linked stream headers/checksums do not match supplied EP1 metadata.");
        }
        long bin = manifest.Header.ContainerPrefixSize + 4, relo = bin, imp = bin;
        int matches = 0;
        foreach (ManifestAsset asset in manifest.Assets)
        {
            if (goldens.TryGetValue(asset.Name, out Chunk? expected))
            {
                Require(asset.TypeId == 0xC5E07887u && asset.TypeHash == 0x74425C11u && asset.Tokenized == 0
                    && asset.InstanceDataSize == expected.InstanceBuffer.Length && asset.RelocationDataSize == expected.RelocationBuffer.Length
                    && asset.ImportsDataSize == expected.ImportsBuffer.Length, "Modifier golden fingerprint or bounded chunk length differs.");
                byte[] actual = AssetStreamProbe.ReadRange(Path.ChangeExtension(path, ".bin"), null, bin, asset.InstanceDataSize);
                byte[] actualRelo = AssetStreamProbe.ReadRange(Path.ChangeExtension(path, ".relo"), null, relo, asset.RelocationDataSize);
                byte[] actualImp = AssetStreamProbe.ReadRange(Path.ChangeExtension(path, ".imp"), null, imp, asset.ImportsDataSize);
                Require(expected.InstanceBuffer.SequenceEqual(actual) && expected.RelocationBuffer.SequenceEqual(actualRelo)
                    && expected.ImportsBuffer.SequenceEqual(actualImp), $"Native modifier differs from real EP1 golden: {asset.Name}");
                Console.WriteLine($"  Real EP1 golden: {asset.Name} exact {actual.Length}/{actualRelo.Length}/{actualImp.Length}; BIN SHA256={Convert.ToHexString(SHA256.HashData(actual))}");
                matches++;
            }
            bin = checked(bin + asset.InstanceDataSize);
            relo = checked(relo + asset.RelocationDataSize);
            imp = checked(imp + asset.ImportsDataSize);
        }
        Require(matches > 0, "Supplied manifest has no supported modifier golden assets.");
        Console.WriteLine($"  Evidence: {path}; metadata SHA256={Convert.ToHexString(SHA256.HashData(metadata))}; {matches} matched assets");
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: read bounded little-endian ABI words independently of the marshaller's host pointer layout. */
    //-------------------------------------------------------------------------------------------------
    private static uint Read(byte[] data, int offset) => BinaryPrimitives.ReadUInt32LittleEndian(data.AsSpan(offset, 4));

    //-------------------------------------------------------------------------------------------------
    /** Reborn: abort native processor proof at its first mismatch instead of silently approving a registry entry. */
    //-------------------------------------------------------------------------------------------------
    private static void Require(bool condition, string message)
    {
        if (!condition) throw new InvalidDataException(message);
    }
}
