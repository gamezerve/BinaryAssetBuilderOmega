using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Xml;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: selected XML admission is separate from compilation/publication; only the reviewed AK47 Damage slot is editable.
internal static class WeaponAuthoringAdmission
{
    private const string Namespace = "uri:ea.com:eala:asset";
    private const int Limit = 16384;

    //-------------------------------------------------------------------------------------------------
    /** Reborn: bound standalone/declaration XML and prohibit DTDs, processing instructions and hidden declaration members. */
    //-------------------------------------------------------------------------------------------------
    private static XmlDocument Parse(byte[] bytes)
    {
        if (bytes.Length == 0 || bytes.Length > Limit) throw new InvalidDataException("Weapon XML must be 1..16384 bytes.");
        var document = new XmlDocument { XmlResolver = null };
        using var stream = new MemoryStream(bytes, false);
        using var reader = XmlReader.Create(stream, new XmlReaderSettings { DtdProcessing = DtdProcessing.Prohibit,
            XmlResolver = null, MaxCharactersInDocument = Limit, IgnoreComments = true, IgnoreWhitespace = true });
        document.Load(reader);
        if (document.ChildNodes.Cast<XmlNode>().Any(node => node is not XmlElement && node is not XmlDeclaration))
            throw new InvalidDataException("Weapon document contains unsupported top-level content.");
        var root = document.DocumentElement!;
        if (root.LocalName == "AssetDeclaration")
        {
            if (root.NamespaceURI != Namespace || Attributes(root).Count != 0 || root.ChildNodes.Count != 1 || root.FirstChild is not XmlElement)
                throw new InvalidDataException("Weapon declaration requires exactly one weapon, without includes or definitions.");
            var standalone = new XmlDocument { XmlResolver = null };
            standalone.AppendChild(standalone.ImportNode(root.FirstChild, true));
            return standalone;
        }
        return document;
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: compare namespace-aware attribute sets without accepting injected namespace aliases or ignoring caller semantics. */
    //-------------------------------------------------------------------------------------------------
    private static Dictionary<string, string> Attributes(XmlElement element)
    {
        var result = new Dictionary<string, string>(StringComparer.Ordinal);
        foreach (XmlAttribute attribute in element.Attributes)
        {
            if (attribute.Name == "xmlns" && attribute.Value == Namespace) continue;
            if (attribute.NamespaceURI.Length != 0) throw new InvalidDataException("Namespaced weapon attributes are not admitted.");
            result.Add(attribute.Name, attribute.Value);
        }
        return result;
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: accept invariant unsigned integers or a .0 suffix in 0..10000, preserving exact binary32 values. */
    //-------------------------------------------------------------------------------------------------
    private static int ReadDamage(string text)
    {
        string integer = text.EndsWith(".0", StringComparison.Ordinal) ? text[..^2] : text;
        if (!int.TryParse(integer, NumberStyles.None, CultureInfo.InvariantCulture, out int value)
            || value < 0 || value > 10000 || integer != value.ToString(CultureInfo.InvariantCulture))
            throw new InvalidDataException("AK47 Damage requires an integer 0..10000, optionally followed by .0.");
        return value;
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: canonicalize only after recursively matching the reviewed complete shape; never discard unsupported fields. */
    //-------------------------------------------------------------------------------------------------
    internal static (string Xml, int Damage) Admit(byte[] bytes)
    {
        var source = Parse(bytes);
        string fixture = Path.Combine(Path.GetDirectoryName(ReferencePipelineSmokeTest.FindFixture())!, "WeaponAuthoringProbe.xml");
        var canonical = Parse(File.ReadAllBytes(fixture));
        int damage = -1;
        Compare(source.DocumentElement!, canonical.DocumentElement!);
        if (damage < 0) throw new InvalidDataException("Weapon Damage is absent.");
        return (canonical.DocumentElement!.OuterXml, damage);

        //-------------------------------------------------------------------------------------------------
        /** Reborn: require exact element order and attributes; only the one known DamageNugget Damage value may differ. */
        //-------------------------------------------------------------------------------------------------
        void Compare(XmlElement actual, XmlElement expected)
        {
            if (actual.LocalName != expected.LocalName || actual.NamespaceURI != Namespace || actual.Prefix.Length != 0)
                throw new InvalidDataException("Unsupported weapon element or namespace: " + actual.Name);
            var fields = Attributes(actual); var reference = Attributes(expected);
            if (fields.Count != reference.Count || fields.Keys.Any(key => !reference.ContainsKey(key)))
                throw new InvalidDataException("Unsupported or missing weapon attributes: " + actual.LocalName);
            foreach (var field in reference)
            {
                if (expected.LocalName == "DamageNugget" && field.Key == "Damage")
                {
                    damage = ReadDamage(fields[field.Key]);
                    expected.SetAttribute(field.Key, damage.ToString(CultureInfo.InvariantCulture) + ".0");
                }
                else if (fields[field.Key] != field.Value)
                    throw new InvalidDataException("Only AK47 Damage is editable; changed " + actual.LocalName + "/@" + field.Key);
            }
            if (actual.ChildNodes.Count != expected.ChildNodes.Count) throw new InvalidDataException("Weapon child count differs.");
            for (int index = 0; index < expected.ChildNodes.Count; index++)
            {
                if (actual.ChildNodes[index] is not XmlElement child || expected.ChildNodes[index] is not XmlElement original)
                    throw new InvalidDataException("Unsupported weapon child content.");
                Compare(child, original);
            }
        }
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: stage an immutable admitted snapshot/receipt in a fresh checked directory without compiling or launching. */
    //-------------------------------------------------------------------------------------------------
    internal static void Stage(string source, string directory)
    {
        using var file = File.Open(source, FileMode.Open, FileAccess.Read, FileShare.Read);
        if (file.Length == 0 || file.Length > Limit) throw new InvalidDataException("Weapon XML admission size bound exceeded.");
        byte[] bytes = new byte[(int)file.Length]; file.ReadExactly(bytes);
        var admitted = Admit(bytes);
        directory = Path.GetFullPath(directory);
        if (Directory.Exists(directory) || File.Exists(directory) || !Directory.Exists(Path.GetDirectoryName(directory)))
            throw new InvalidDataException("Weapon admission output must be a new child of an existing directory.");
        for (var parent = new DirectoryInfo(Path.GetDirectoryName(directory)!); parent != null; parent = parent.Parent)
            if ((parent.Attributes & FileAttributes.ReparsePoint) != 0) throw new InvalidDataException("Redirected weapon output refused.");
        Directory.CreateDirectory(directory);
        File.WriteAllText(Path.Combine(directory, "selected-weapon.xml"), admitted.Xml);
        var receipt = new { WeaponId = "SovietAntiInfantryInfantryAK47", admitted.Damage,
            SelectedInputSha256 = Convert.ToHexString(SHA256.HashData(bytes)), SelectedInputAdmitted = true,
            EditableFieldCount = 1, PackageBuilt = false, GameExecuted = false };
        string json = JsonSerializer.Serialize(receipt, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(Path.Combine(directory, "admission.json"), json); Console.WriteLine(json);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: exercise selected value boundaries and semantic refusal controls without native compilation or output. */
    //-------------------------------------------------------------------------------------------------
    internal static void SelfTest()
    {
        string fixture = Path.Combine(Path.GetDirectoryName(ReferencePipelineSmokeTest.FindFixture())!, "WeaponAuthoringProbe.xml");
        string original = Parse(File.ReadAllBytes(fixture)).DocumentElement!.OuterXml;
        foreach (int value in new[] { 0, 5, 50, 73, 10000 })
        {
            string authored = original.Replace("Damage=\"5.0\"", $"Damage=\"{value}\"");
            var standalone = Admit(Encoding.UTF8.GetBytes(authored));
            var declaration = Admit(Encoding.UTF8.GetBytes("<AssetDeclaration xmlns=\"" + Namespace + "\">" + authored + "</AssetDeclaration>"));
            if (standalone.Damage != value || standalone != declaration || Admit(Encoding.UTF8.GetBytes(standalone.Xml)) != standalone)
                throw new InvalidDataException("Weapon authoring canonicalization differs.");
        }
        string[] faults = {
            original.Replace("Damage=\"5.0\"", "Damage=\"10001\""),
            original.Replace("Damage=\"5.0\"", "Damage=\"-1\""),
            original.Replace("Damage=\"5.0\"", "Damage=\"NaN\""),
            original.Replace("Damage=\"5.0\"", "Damage=\"=$DAMAGE\""),
            original.Replace("Damage=\"5.0\"", "Damage=\"05\""),
            original.Replace("Damage=\"5.0\"", "Damage=\"5.5\""),
            original.Replace("AttackRange=\"150.0\"", "AttackRange=\"151.0\""),
            original.Replace("SovietAntiInfantryInfantryAK47", "NewWeapon"),
            original.Replace("DeathType=\"NORMAL\"", "DeathType=\"NORMAL\" TypeId=\"1\""),
            original.Replace("FireFX=\"FX_MuzzleSoviet\"", "FireFX=\"MissingFX\""),
            original.Replace("<Nuggets>", "<Extra/><Nuggets>"),
            original.Replace("<Nuggets>", "text<Nuggets>"),
            original.Replace("<WeaponTemplate ", "<WeaponTemplate inheritFrom=\"BaseWeapon\" "),
            "<AssetDeclaration xmlns=\"" + Namespace + "\">" + original + original + "</AssetDeclaration>",
            "<AssetDeclaration xmlns=\"" + Namespace + "\"><Includes/>" + original + "</AssetDeclaration>",
            "<!DOCTYPE WeaponTemplate [<!ENTITY bad 'x'>]>" + original,
            "<?unsupported value?>" + original,
            original.Replace("DamageType=\"GUN\"", "DamageType=\"CANNON\"")
        };
        foreach (string fault in faults)
        {
            bool refused = false;
            try { Admit(Encoding.UTF8.GetBytes(fault)); }
            catch (Exception error) when (error is InvalidDataException or XmlException) { refused = true; }
            if (!refused) throw new InvalidDataException("Weapon authoring refusal control was accepted.");
        }
        Console.WriteLine("Weapon authoring admission: PASS (five values, standalone/declaration canonical repeat, eighteen semantic refusals; no package/game).");
    }
}
