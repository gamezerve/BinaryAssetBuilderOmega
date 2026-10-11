using System.Buffers.Binary;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Xml;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: admit only a bounded standalone ConYard XML whose sole editable field is the GUN coefficient; no production or runtime authority.
internal static class ArmorAuthoringAdmission
{
    private const int Limit = 16384;
    private const string Namespace = "uri:ea.com:eala:asset";

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
    /** Reborn: allow exact identity/order/coefficient shape and canonical integer GUN percentages from zero through ten thousand only. */
    //-------------------------------------------------------------------------------------------------
    internal static (string Xml, int GunPercent) Admit(byte[] bytes)
    {
        var document = Parse(bytes);
        string fixtures = Path.GetDirectoryName(ReferencePipelineSmokeTest.FindFixture())!;
        var reference = Parse(File.ReadAllBytes(Path.Combine(fixtures, "AlliedConYardArmorProbe.xml")));
        var root = document.DocumentElement ?? throw new InvalidDataException("Armor root absent.");
        var expected = reference.DocumentElement!;
        if (root.LocalName != "ArmorTemplate" || root.NamespaceURI != Namespace || root.GetAttribute("id") != "AlliedConYardArmor"
            || root.Attributes.Cast<XmlAttribute>().Any(attribute => attribute.Name != "id" && attribute.Name != "xmlns")
            || document.ChildNodes.Cast<XmlNode>().Any(node => node is not XmlElement && node is not XmlDeclaration))
            throw new InvalidDataException("Only the standalone AlliedConYardArmor root is admitted.");
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
            if (item.GetAttribute("Damage") == "GUN")
            {
                if (!int.TryParse(value, NumberStyles.None, CultureInfo.InvariantCulture, out percent) || percent < 0 || percent > 10000
                    || percent.ToString(CultureInfo.InvariantCulture) != value) throw new InvalidDataException("GUN requires canonical integer Percent 0..10000.");
            }
            else if (value != originals[index].GetAttribute("Percent")) throw new InvalidDataException("Only GUN Percent is editable in this admission version.");
        }
        // Reborn: canonicalize through the known fixture, never carry caller comments, attributes, includes or formatting into a later compile.
        expected.ChildNodes.OfType<XmlElement>().Single(item => item.GetAttribute("Damage") == "GUN")
            .SetAttribute("Percent", percent.ToString(CultureInfo.InvariantCulture));
        string canonical = expected.OuterXml;
        var schemas = ArmorTokenSmokeTest.LoadSchemas(fixtures);
        var compiled = ArmorTokenSmokeTest.Compile(schemas, canonical);
        expected.ChildNodes.OfType<XmlElement>().Single(item => item.GetAttribute("Damage") == "GUN").SetAttribute("Percent", "100");
        var baseline = ArmorTokenSmokeTest.Compile(schemas, expected.OuterXml);
        const int gunOffset = 788;
        if (compiled.InstanceBuffer.Length != 856 || baseline.InstanceBuffer.Length != 856
            || compiled.ImportsBuffer.Length != 0 || !compiled.RelocationBuffer.SequenceEqual(baseline.RelocationBuffer)
            || !compiled.InstanceBuffer.AsSpan(0, gunOffset).SequenceEqual(baseline.InstanceBuffer.AsSpan(0, gunOffset))
            || !compiled.InstanceBuffer.AsSpan(gunOffset + 4).SequenceEqual(baseline.InstanceBuffer.AsSpan(gunOffset + 4))
            || BitConverter.Int32BitsToSingle(BinaryPrimitives.ReadInt32LittleEndian(compiled.InstanceBuffer.AsSpan(gunOffset))) != percent / 100f)
            throw new InvalidDataException("Editable armor changed bytes outside the admitted GUN float.");
        return (canonical, percent);
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
        byte[] canonical = Encoding.UTF8.GetBytes("<!-- Reborn: admitted ConYard GUN-only authoring snapshot; not a built mod. -->\n" + admitted.Xml + "\n");
        Directory.CreateDirectory(directory);
        string snapshot = Path.Combine(directory, "admitted-armor.xml"); File.WriteAllBytes(snapshot, canonical);
        var report = new { Source = source, SourceSha256 = Convert.ToHexString(SHA256.HashData(bytes)), Snapshot = snapshot,
            SnapshotSha256 = Convert.ToHexString(SHA256.HashData(canonical)), admitted.GunPercent,
            InputAdmitted = true, CompilerPayloadCompared = true, OnlyGunFloatEditable = true,
            LinkedStreamsEmitted = false, ModPackageBuilt = false, GameExecuted = false, RuntimeOverrideProven = false,
            NextGate = "Bind the admitted snapshot to fresh document/Core identities and the scoped OutputManager, then verify a parameterized package." };
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
            xml.Replace("Damage=\"GUN\"", "Damage=\"CANNON\""), xml.Replace("Damage=\"CANNON\" Percent=\"100\"", "Damage=\"CANNON\" Percent=\"200\""),
            xml.Replace("<Armor Damage=\"GUN\"", "<Armor Extra=\"1\" Damage=\"GUN\""), xml.Replace("</ArmorTemplate>", "<Include source=\"other.xml\" /></ArmorTemplate>"),
            xml.Replace("uri:ea.com:eala:asset", "wrong:namespace"), xml.Replace("<ArmorTemplate", "<GameObject").Replace("</ArmorTemplate>", "</GameObject>"),
            xml.Replace("?>", "?><!DOCTYPE ArmorTemplate [<!ENTITY x '100'>]>")
        };
        foreach (string fault in faults) Refuse(Encoding.UTF8.GetBytes(fault));
        Refuse(Array.Empty<byte>()); Refuse(new byte[Limit + 1]);
        Console.WriteLine("Armor authoring admission: PASS (five coefficients, seventeen refusals; no linked output or game).");
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
