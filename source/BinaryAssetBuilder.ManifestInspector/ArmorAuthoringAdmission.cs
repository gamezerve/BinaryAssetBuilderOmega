using System.Buffers.Binary;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Xml;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: admit bounded standalone ConYard percentages in fixed eighteen float slots; no general type or runtime authority.
internal static class ArmorAuthoringAdmission
{
    private const int Limit = 16384;
    private const string Namespace = "uri:ea.com:eala:asset";
    // Reborn: preserve the tokenizer's root field order; list shape, damage identities and pointer fields remain immutable.
    internal static readonly string[] ScalarFields = { "Default", "DamageScalar", "SideDamageScalar", "RearDamageScalar", "FlankedPenalty" };
    // Reborn: admit only same-shape faction yards whose untouched baseline must separately match the supplied stock stream.
    internal static bool SupportedIdentity(string identity) => identity is "AlliedConYardArmor" or "SovietConYardArmor";

    //-------------------------------------------------------------------------------------------------
    /** Reborn: extract only the bounded supported standalone identity, never a manifest-supplied name. */
    //-------------------------------------------------------------------------------------------------
    internal static string Identity(string xml)
    {
        string identity = Parse(Encoding.UTF8.GetBytes(xml)).DocumentElement!.GetAttribute("id");
        if (!SupportedIdentity(identity)) throw new InvalidDataException("Unsupported armor identity.");
        return identity;
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: prohibit DTD/entity resolution and admit a complete small XML document before any output is created. */
    //-------------------------------------------------------------------------------------------------
    private static XmlDocument Parse(byte[] bytes)
    {
        if (bytes.Length == 0 || bytes.Length > Limit) throw new InvalidDataException("Armor XML exceeds the 16 KiB admission bound.");
        var document = new XmlDocument { XmlResolver = null };
        using var stream = new MemoryStream(bytes, false);
        using var reader = XmlReader.Create(stream, new XmlReaderSettings { DtdProcessing = DtdProcessing.Prohibit,
            XmlResolver = null, MaxCharactersInDocument = Limit, IgnoreComments = true, IgnoreWhitespace = true });
        document.Load(reader);
        return document;
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: preserve identity/order and admit canonical integer percentages 0..10000 for thirteen entries and five optional scalars. */
    //-------------------------------------------------------------------------------------------------
    internal static (string Xml, int GunPercent) Admit(byte[] bytes)
    {
        var document = Parse(bytes);
        string fixtures = Path.GetDirectoryName(ReferencePipelineSmokeTest.FindFixture())!;
        var reference = Parse(File.ReadAllBytes(Path.Combine(fixtures, "AlliedConYardArmorProbe.xml")));
        var root = document.DocumentElement ?? throw new InvalidDataException("Armor root absent.");
        var expected = reference.DocumentElement!;
        if (root.LocalName != "ArmorTemplate" || root.NamespaceURI != Namespace || !SupportedIdentity(root.GetAttribute("id"))
            || root.Attributes.Cast<XmlAttribute>().Any(attribute => attribute.Name != "id" && attribute.Name != "xmlns" && !ScalarFields.Contains(attribute.Name))
            || document.ChildNodes.Cast<XmlNode>().Any(node => node is not XmlElement && node is not XmlDeclaration))
            throw new InvalidDataException("Only standalone AlliedConYardArmor or SovietConYardArmor is admitted.");
        expected.SetAttribute("id", root.GetAttribute("id"));
        var entries = root.ChildNodes.Cast<XmlNode>().ToArray();
        var originals = expected.ChildNodes.OfType<XmlElement>().ToArray();
        if (entries.Length != 13 || originals.Length != 13) throw new InvalidDataException("Exactly thirteen ordered armor entries are required.");
        int percent = -1;
        for (int index = 0; index < entries.Length; index++)
        {
            if (entries[index] is not XmlElement item || item.LocalName != "Armor" || item.NamespaceURI != Namespace || item.HasChildNodes
                || item.Attributes.Count != 2 || !item.HasAttribute("Damage") || !item.HasAttribute("Percent")
                || item.GetAttribute("Damage") != originals[index].GetAttribute("Damage"))
                throw new InvalidDataException("Armor entry shape/order differs.");
            string value = item.GetAttribute("Percent");
            int coefficient = ReadPercent(value);
            if (item.GetAttribute("Damage") == "GUN") percent = coefficient;
            originals[index].SetAttribute("Percent", value);
        }
        // Reborn: canonicalize through the known fixture, never carry caller comments, attributes, includes or formatting into a later compile.
        foreach (string field in ScalarFields)
            if (root.HasAttribute(field)) { ReadPercent(root.GetAttribute(field)); expected.SetAttribute(field, root.GetAttribute(field)); }
        string canonical = expected.OuterXml;
        var schemas = ArmorTokenSmokeTest.LoadSchemas(fixtures);
        var compiled = ArmorTokenSmokeTest.Compile(schemas, canonical);
        reference = Parse(File.ReadAllBytes(Path.Combine(fixtures, "AlliedConYardArmorProbe.xml")));
        reference.DocumentElement!.SetAttribute("id", root.GetAttribute("id"));
        reference.DocumentElement!.ChildNodes.OfType<XmlElement>().Single(item => item.GetAttribute("Damage") == "GUN").SetAttribute("Percent", "100");
        var baseline = ArmorTokenSmokeTest.Compile(schemas, reference.DocumentElement.OuterXml);
        if (compiled.InstanceBuffer.Length != 856 || baseline.InstanceBuffer.Length != 856
            || compiled.ImportsBuffer.Length != 0 || !compiled.RelocationBuffer.SequenceEqual(baseline.RelocationBuffer)
            || baseline.ImportsBuffer.Length != 0)
            throw new InvalidDataException("Editable armor changed stream shape.");
        VerifyPayload(canonical, baseline.InstanceBuffer, compiled.InstanceBuffer);
        return (canonical, percent);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: reject signs, leading zeros, formulas, percent suffixes, decimals and non-finite text before schema compilation. */
    //-------------------------------------------------------------------------------------------------
    private static int ReadPercent(string value)
    {
        if (!int.TryParse(value, NumberStyles.None, CultureInfo.InvariantCulture, out int number) || number < 0 || number > 10000
            || number.ToString(CultureInfo.InvariantCulture) != value) throw new InvalidDataException("Armor percentage requires canonical integer 0..10000.");
        return number;
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: expose normalized values and fixed native offsets without admitting alternate identities or list shapes. */
    //-------------------------------------------------------------------------------------------------
    internal static Dictionary<string, (int Percent, int Offset)> Values(string xml)
    {
        var root = Parse(Encoding.UTF8.GetBytes(xml)).DocumentElement!;
        var result = new Dictionary<string, (int Percent, int Offset)>();
        for (int index = 0; index < ScalarFields.Length; index++)
            result.Add(ScalarFields[index], (root.HasAttribute(ScalarFields[index]) ? ReadPercent(root.GetAttribute(ScalarFields[index])) : 100, 724 + index * 4));
        int itemIndex = 0;
        foreach (XmlElement item in root.ChildNodes.OfType<XmlElement>())
            result.Add("Armor/" + item.GetAttribute("Damage"), (ReadPercent(item.GetAttribute("Percent")), 756 + itemIndex++ * 8));
        return result;
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: independently verify each percentage float and require every identity/token/pointer byte outside the eighteen slots to match stock. */
    //-------------------------------------------------------------------------------------------------
    internal static void VerifyPayload(string xml, byte[] baseline, byte[] compiled)
    {
        if (baseline.Length != 856 || compiled.Length != 856) throw new InvalidDataException("Armor payload length differs.");
        byte[] masked = compiled.ToArray();
        var values = Values(xml);
        if (values.Count != 18) throw new InvalidDataException("Expected eighteen fixed percentage slots.");
        foreach (var value in values.Values)
        {
            // Reborn: use the marshaler's exact single-precision multiplication, not division with different rounding for values such as 73.
            if (BinaryPrimitives.ReadInt32LittleEndian(compiled.AsSpan(value.Offset)) != BitConverter.SingleToInt32Bits(value.Percent * 0.01f))
                throw new InvalidDataException("Armor percentage float differs from authored input.");
            baseline.AsSpan(value.Offset, 4).CopyTo(masked.AsSpan(value.Offset, 4));
        }
        if (!masked.SequenceEqual(baseline)) throw new InvalidDataException("Armor changed bytes outside the eighteen percentage slots.");
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: stage a validated snapshot in a fresh owned directory; this preflight emits neither linked streams nor a launchable package. */
    //-------------------------------------------------------------------------------------------------
    internal static void Stage(string source, string directory)
    {
        source = Path.GetFullPath(source); directory = Path.GetFullPath(directory);
        CheckPath(source); CheckPath(Path.GetDirectoryName(directory)!);
        if (File.Exists(directory) || Directory.Exists(directory)) throw new InvalidDataException("Armor admission output already exists.");
        using var input = new FileStream(source, FileMode.Open, FileAccess.Read, FileShare.Read);
        if (input.Length == 0 || input.Length > Limit) throw new InvalidDataException("Armor input exceeds bound.");
        byte[] bytes = new byte[(int)input.Length]; input.ReadExactly(bytes);
        var admitted = Admit(bytes);
        byte[] canonical = Encoding.UTF8.GetBytes("<!-- Reborn: admitted ConYard authoring snapshot; not a built mod. -->\n" + admitted.Xml + "\n");
        Directory.CreateDirectory(directory);
        string snapshot = Path.Combine(directory, "admitted-armor.xml"); File.WriteAllBytes(snapshot, canonical);
        var report = new { Source = source, SourceSha256 = Convert.ToHexString(SHA256.HashData(bytes)), Snapshot = snapshot,
            SnapshotSha256 = Convert.ToHexString(SHA256.HashData(canonical)), admitted.GunPercent,
            ArmorId = Identity(admitted.Xml), InputAdmitted = true, CompilerPayloadCompared = true, OnlyGunFloatEditable = false,
            EditableFloatCount = 18, Percentages = Values(admitted.Xml).ToDictionary(item => item.Key, item => item.Value.Percent),
            LinkedStreamsEmitted = false, ModPackageBuilt = false, GameExecuted = false, RuntimeOverrideProven = false,
            NextGate = "Use armor-authored-build with a fresh output directory and stock baseline; this admission receipt alone proves no package or runtime effect." };
        string json = JsonSerializer.Serialize(report, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(Path.Combine(directory, "admission.json"), json); Console.WriteLine(json);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: reject ordinary reparse ancestry; concurrent hostile filesystem replacement is outside this trusted-local authoring scope. */
    //-------------------------------------------------------------------------------------------------
    private static void CheckPath(string path)
    {
        for (FileSystemInfo? item = File.Exists(path) ? new FileInfo(path) : new DirectoryInfo(path); item != null;
            item = item is FileInfo file ? file.Directory : ((DirectoryInfo)item).Parent)
            if ((item.Attributes & FileAttributes.ReparsePoint) != 0) throw new InvalidDataException("Redirected armor path refused.");
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: exercise supported values and malformed/expanded input refusals entirely in memory, without production output or game startup. */
    //-------------------------------------------------------------------------------------------------
    internal static void SelfTest()
    {
        string fixtures = Path.GetDirectoryName(ReferencePipelineSmokeTest.FindFixture())!;
        string xml = File.ReadAllText(Path.Combine(fixtures, "AlliedConYardArmorProbe.xml"));
        foreach (int value in new[] { 0, 100, 250, 5000, 10000 })
            if (Admit(Encoding.UTF8.GetBytes(xml.Replace("Percent=\"5000\"", $"Percent=\"{value}\""))).GunPercent != value)
                throw new InvalidDataException("Armor admitted value differs.");
        string[] faults = {
            xml.Replace("AlliedConYardArmor", "OtherArmor"), xml.Replace("id=\"AlliedConYardArmor\"", "id=\"AlliedConYardArmor\" inheritFrom=\"Base\""),
            xml.Replace("Percent=\"5000\"", "Percent=\"-1\""), xml.Replace("Percent=\"5000\"", "Percent=\"10001\""),
            xml.Replace("Percent=\"5000\"", "Percent=\"05000\""), xml.Replace("Percent=\"5000\"", "Percent=\"=Formula\""),
            xml.Replace("Percent=\"5000\"", "Percent=\"NaN\""), xml.Replace("Percent=\"5000\"", "Percent=\"5.5\""),
            xml.Replace("Damage=\"GUN\"", "Damage=\"CANNON\""), xml.Replace("Damage=\"CANNON\" Percent=\"100\"", "Damage=\"CANNON\" Percent=\"10001\""),
            xml.Replace("<Armor Damage=\"GUN\"", "<Armor Extra=\"1\" Damage=\"GUN\""), xml.Replace("</ArmorTemplate>", "<Include source=\"other.xml\" /></ArmorTemplate>"),
            xml.Replace("uri:ea.com:eala:asset", "wrong:namespace"), xml.Replace("<ArmorTemplate", "<GameObject").Replace("</ArmorTemplate>", "</GameObject>"),
            xml.Replace("?>", "?><!DOCTYPE ArmorTemplate [<!ENTITY x '100'>]>")
        };
        foreach (string fault in faults) Refuse(Encoding.UTF8.GetBytes(fault));
        Refuse(Array.Empty<byte>()); Refuse(new byte[Limit + 1]);
        // Reborn: every editable slot gets boundary and rounding-sensitive coverage, while all shape refusals remain active.
        foreach (string identity in new[] { "AlliedConYardArmor", "SovietConYardArmor" })
        foreach (string field in Values(Admit(Encoding.UTF8.GetBytes(xml)).Xml).Keys)
            foreach (int value in new[] { 0, 73, 250, 10000 })
            {
                var edited = Parse(Encoding.UTF8.GetBytes(xml));
                // Reborn: cover both bounded identities, including preservation through canonicalization.
                edited.DocumentElement!.SetAttribute("id", identity);
                if (field.StartsWith("Armor/")) edited.DocumentElement!.ChildNodes.OfType<XmlElement>().Single(item => item.GetAttribute("Damage") == field.Substring(6)).SetAttribute("Percent", value.ToString(CultureInfo.InvariantCulture));
                else edited.DocumentElement!.SetAttribute(field, value.ToString(CultureInfo.InvariantCulture));
                var admitted = Admit(Encoding.UTF8.GetBytes(edited.OuterXml));
                if (Identity(admitted.Xml) != identity || Values(admitted.Xml)[field].Percent != value) throw new InvalidDataException("Expanded percentage/identity differs.");
            }
        Console.WriteLine("Armor authoring admission: PASS (five legacy values, two identities x eighteen fields x four values, seventeen refusals; no linked output or game).");
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: count only explicit input/XML refusals, not arbitrary compiler or environment failures. */
    //-------------------------------------------------------------------------------------------------
    private static void Refuse(byte[] bytes)
    {
        try { Admit(bytes); } catch (InvalidDataException) { return; } catch (XmlException) { return; }
        throw new InvalidDataException("Unsupported armor authoring input accepted.");
    }
}
