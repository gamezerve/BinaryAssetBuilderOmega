using System.Buffers.Binary;
using System.Security.Cryptography;
using System.Xml;
using Relo;
using SageBinaryData;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: compile one unchanged reference weapon's native payload for comparison only; no tokenized output or publication authority.
internal static class WeaponNativeProbe
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: prepare known direct-reference indices in an owned XML copy and compare real marshaling to the bounded stock native tail. */
    //-------------------------------------------------------------------------------------------------
    internal static object Inspect(string source, string manifest)
    {
        using var stream = File.OpenRead(source);
        if (stream.Length == 0 || stream.Length > 16 * 1024 * 1024) throw new InvalidDataException("Weapon source is empty or exceeds 16 MiB.");
        using var reader = XmlReader.Create(stream, new XmlReaderSettings {
            DtdProcessing = DtdProcessing.Prohibit, XmlResolver = null, MaxCharactersInDocument = 16 * 1024 * 1024 });
        var xml = new XmlDocument { XmlResolver = null }; xml.Load(reader);
        var manager = new XmlNamespaceManager(xml.NameTable); manager.AddNamespace("ea", "uri:ea.com:eala:asset");
        var selected = xml.SelectSingleNode("/ea:AssetDeclaration/ea:WeaponTemplate[@id='SovietAntiInfantryInfantryAK47']", manager) as XmlElement
            ?? throw new InvalidDataException("Reference AK47 XML absent.");
        var owned = new XmlDocument { XmlResolver = null }; owned.LoadXml(selected.OuterXml);
        var root = owned.DocumentElement!;
        var names = new[] { "FireSoundPerClip", "FireFX", "FireVeteranFX" };
        var references = new[] { "SOV_Conscript_GunFireMS", "FX_MuzzleSoviet", "FX_MuzzleSoviet" };
        for (int index = 0; index < names.Length; index++)
        {
            if (root.GetAttribute(names[index]) != references[index]) throw new InvalidDataException("Reference weapon source changed: " + names[index]);
            root.SetAttribute(names[index], (index == 0 ? "Multisound" : "FXList") + "\\" + index);
        }
        var nugget = root.GetElementsByTagName("DamageNugget", "uri:ea.com:eala:asset").Cast<XmlElement>().Single();
        if (nugget.GetAttribute("Damage") != "5.0" || nugget.GetAttribute("DamageType") != "GUN")
            throw new InvalidDataException("Expected unedited reference AK47 damage.");
        nugget.SetAttribute("TypeId", "0xC0F53E99");
        var native = Compile(owned); var repeated = Compile(owned);
        if (!native.InstanceBuffer.SequenceEqual(repeated.InstanceBuffer) || !native.RelocationBuffer.SequenceEqual(repeated.RelocationBuffer)
            || !native.ImportsBuffer.SequenceEqual(repeated.ImportsBuffer)) throw new InvalidDataException("Native weapon compile nondeterministic.");
        // Reborn: an authored damage edit must change only its proven native scalar, without changing allocation or reference preparation.
        nugget.SetAttribute("Damage", "50.0");
        var edited = Compile(owned); nugget.SetAttribute("Damage", "5.0");
        var expectedEdit = native.InstanceBuffer.ToArray();
        BinaryPrimitives.WriteSingleLittleEndian(expectedEdit.AsSpan(412, 4), 50f);
        if (!edited.InstanceBuffer.SequenceEqual(expectedEdit) || !edited.RelocationBuffer.SequenceEqual(native.RelocationBuffer)
            || !edited.ImportsBuffer.SequenceEqual(native.ImportsBuffer)) throw new InvalidDataException("Authored Damage edit altered unproven native data.");
        var document = ManifestReader.Read(File.ReadAllBytes(manifest));
        TypeRegistryAudit.ValidateTarget(document.Header.Version, document.Header.AllTypesHash);
        if (document.Validate().Count != 0 || !document.Header.IsLinked) throw new InvalidDataException("Invalid weapon comparison manifest.");
        var asset = document.Assets.Single(item => item.Name == "WeaponTemplate:SovietAntiInfantryInfantryAK47");
        if (asset.InstanceDataSize != 2508 || asset.RelocationDataSize != 28 || asset.ImportsDataSize != 0
            || asset.TypeHash != 0x27E8D984u || asset.Tokenized != 1)
            throw new InvalidDataException("Comparison weapon fingerprint differs.");
        long offset = document.Header.ContainerPrefixSize + 4;
        long relocationOffset = document.Header.ContainerPrefixSize + 4;
        foreach (var item in document.Assets) { if (ReferenceEquals(item, asset)) break; offset += item.InstanceDataSize; relocationOffset += item.RelocationDataSize; }
        var stock = AssetStreamProbe.ReadRange(Path.ChangeExtension(manifest, ".bin"), null, offset, asset.InstanceDataSize);
        // Reborn: compare independently read stock relocation locations rather than deriving all expected locations from compiler output.
        var stockRelocations = AssetStreamProbe.ReadRange(Path.ChangeExtension(manifest, ".relo"), null, relocationOffset, asset.RelocationDataSize);
        // Reborn: the independently recovered native base is comparison evidence only, never a stock template for emitted output.
        const int prefix = 1980;
        for (int at = 0; at + 4 <= stockRelocations.Length; at += 4)
        {
            uint location = Word(stockRelocations, at); if (location == uint.MaxValue) break;
            if (location < prefix) throw new InvalidDataException("Stock relocation lies outside native tail.");
            BinaryPrimitives.WriteUInt32LittleEndian(stockRelocations.AsSpan(at), location - prefix);
        }
        if (!stockRelocations.SequenceEqual(native.RelocationBuffer)) throw new InvalidDataException("Native relocation locations differ from stock.");
        var expected = stock.AsSpan(prefix).ToArray();
        var relocations = native.RelocationBuffer;
        for (int at = 0; at + 4 <= relocations.Length; at += 4)
        {
            uint location = Word(relocations, at); if (location == uint.MaxValue) break;
            if (location > expected.Length - 4) throw new InvalidDataException("Native relocation outside stock tail.");
            uint pointer = Word(expected, (int)location);
            if (pointer < prefix || pointer >= stock.Length) throw new InvalidDataException("Stock relocation cannot normalize to native tail.");
            BinaryPrimitives.WriteUInt32LittleEndian(expected.AsSpan((int)location), pointer - prefix);
        }
        var differences = Enumerable.Range(0, Math.Min(expected.Length, native.InstanceBuffer.Length))
            .Where(at => expected[at] != native.InstanceBuffer[at]).Take(32).ToArray();
        return new {
            Asset = asset.Name, NativeBinBytes = native.InstanceBuffer.Length, NativeReloBytes = native.RelocationBuffer.Length,
            NativeImpBytes = native.ImportsBuffer.Length, StockNativeTailBytes = expected.Length,
            NativeBinSha256 = Convert.ToHexString(SHA256.HashData(native.InstanceBuffer)),
            NormalizedStockSha256 = Convert.ToHexString(SHA256.HashData(expected)),
            NativePayloadMatchesNormalizedStock = native.InstanceBuffer.SequenceEqual(expected),
            FirstDifferingNativeOffsets = differences,
            DifferenceDetails = differences.Select(at => new { Offset = at, StockByte = expected[at], CompiledByte = native.InstanceBuffer[at] }).ToArray(),
            Deterministic = true, SchemaValidated = false,
            NativeRelocationLocationsMatchStock = true, AuthoredDamageEditIsolated = true,
            ReferencePreparationIsFixedDiagnostic = true, CompleteTokenizedPayloadVerified = false,
            WeaponCompilerEnabled = false, ModPackageBuilt = false, GameExecuted = false
        };
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: invoke the existing weapon marshaler on diagnostic prepared XML without schema or production gate bypass. */
    //-------------------------------------------------------------------------------------------------
    private static unsafe Chunk Compile(XmlDocument xml)
    {
        var namespaces = new XmlNamespaceManager(xml.NameTable); namespaces.AddNamespace("ea", "uri:ea.com:eala:asset");
        var node = new Node(xml.CreateNavigator()!.SelectSingleNode("/ea:WeaponTemplate", namespaces)!, namespaces);
        WeaponTemplate* root;
        using var tracker = new Tracker((void**)&root, (uint)sizeof(WeaponTemplate), false);
        Marshaler.Marshal(node, root, tracker); var chunk = new Chunk();
        if (!tracker.MakeRelocatable(chunk)) throw new InvalidDataException("Native weapon marshaling failed.");
        return chunk;
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: read explicit little-endian relocation/value words for native comparison. */
    //-------------------------------------------------------------------------------------------------
    private static uint Word(byte[] bytes, int at) => BinaryPrimitives.ReadUInt32LittleEndian(bytes.AsSpan(at, 4));
}
