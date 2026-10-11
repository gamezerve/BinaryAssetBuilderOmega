using System.Text;
using System.Text.Json;
using System.Xml;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: admit exactly the supported yard pair, compile one actual Core document and publish an independently checked default-reader package.
internal static class ArmorPairAuthoredBuild
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: canonical declaration order is identity-based; neither includes nor duplicate/unknown assets enter compilation. */
    //-------------------------------------------------------------------------------------------------
    internal static string Declaration(string[] pair) => "<AssetDeclaration xmlns=\"uri:ea.com:eala:asset\">" + string.Concat(pair) + "</AssetDeclaration>";

    //-------------------------------------------------------------------------------------------------
    /** Reborn: bound XML, prohibit DTDs and admit exactly one of each supported identity through the existing eighteen-field validator. */
    //-------------------------------------------------------------------------------------------------
    internal static string[] Admit(string xml)
    {
        if (Encoding.UTF8.GetByteCount(xml) > 16384) throw new InvalidDataException("Pair XML exceeds bound.");
        using var reader = XmlReader.Create(new StringReader(xml), new XmlReaderSettings { DtdProcessing = DtdProcessing.Prohibit,
            XmlResolver = null, IgnoreComments = true, IgnoreWhitespace = true, MaxCharactersInDocument = 16384 });
        var document = new XmlDocument { XmlResolver = null }; document.Load(reader);
        var root = document.DocumentElement!;
        if (root.Name != "AssetDeclaration" || root.NamespaceURI != "uri:ea.com:eala:asset" || root.Attributes.Count != 1
            || root.Attributes[0]!.Name != "xmlns" || root.ChildNodes.Count != 2
            || document.ChildNodes.Cast<XmlNode>().Any(node => node is not XmlElement && node is not XmlDeclaration))
            throw new InvalidDataException("Pair requires one plain two-armor AssetDeclaration.");
        string[] pair = root.ChildNodes.Cast<XmlNode>().Select(node => ArmorAuthoringAdmission.Admit(Encoding.UTF8.GetBytes(node.OuterXml)).Xml)
            .OrderBy(ArmorAuthoringAdmission.Identity, StringComparer.Ordinal).ToArray();
        if (!pair.Select(ArmorAuthoringAdmission.Identity).SequenceEqual(new[] { "AlliedConYardArmor", "SovietConYardArmor" }))
            throw new InvalidDataException("Pair requires two distinct supported armor identities.");
        return pair;
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: describe both admitted identities and all values without implying linked output, package or gameplay evidence. */
    //-------------------------------------------------------------------------------------------------
    internal static object[] Values(string[] pair) => pair.Select(xml => (object)new { ArmorId = ArmorAuthoringAdmission.Identity(xml),
        EditableFloatCount = 18, Percentages = ArmorAuthoringAdmission.Values(xml).ToDictionary(item => item.Key, item => item.Value.Percent) }).ToArray();

    //-------------------------------------------------------------------------------------------------
    /** Reborn: preserve the selected file and retain a canonical pair snapshot in a fresh ordinary directory. */
    //-------------------------------------------------------------------------------------------------
    internal static string[] Stage(string source, string directory)
    {
        source = Path.GetFullPath(source); directory = Path.GetFullPath(directory);
        Check(source); Check(Path.GetDirectoryName(directory)!);
        if (File.Exists(directory) || Directory.Exists(directory)) throw new InvalidDataException("Pair output occupied.");
        using var input = new FileStream(source, FileMode.Open, FileAccess.Read, FileShare.Read);
        if (input.Length == 0 || input.Length > 16384) throw new InvalidDataException("Pair source exceeds bound.");
        byte[] bytes = new byte[(int)input.Length]; input.ReadExactly(bytes);
        string[] pair = Admit(Encoding.UTF8.GetString(bytes));
        Directory.CreateDirectory(directory);
        File.WriteAllText(Path.Combine(directory, "admitted-pair.xml"), Declaration(pair));
        string report = JsonSerializer.Serialize(new { ArmorCount = 2, Assets = Values(pair), InputAdmitted = true,
            LinkedStreamsEmitted = false, ModPackageBuilt = false, GameExecuted = false }, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(Path.Combine(directory, "pair-admission.json"), report); Console.WriteLine(report);
        return pair;
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: produce a real two-instance stream, retain its diagnostic archive and verify default packaging against every authored member. */
    //-------------------------------------------------------------------------------------------------
    internal static void Run(string source, string directory, string stockManifest)
    {
        directory = Path.GetFullPath(directory);
        string[] pair = Stage(source, directory);
        string prefix = ArmorPairDocumentSmokeTest.Run(Path.Combine(directory, "core"), stockManifest, pair);
        byte[][] payloads = new[] { ".manifest", ".bin", ".relo", ".imp" }.Select(extension => File.ReadAllBytes(prefix + extension)).ToArray();
        string diagnostic = Path.Combine(directory, "pair-diagnostic.big");
        using (var output = new FileStream(diagnostic, FileMode.CreateNew, FileAccess.Write, FileShare.None)) output.Write(ArmorDiagnosticPackage.Serialize(payloads));
        ArmorDiagnosticPackage.Verify(diagnostic, payloads);
        string package = Path.Combine(directory, "package"), declaration = Declaration(pair);
        ArmorTrialPreparation.Prepare(diagnostic, package, true, declaration);
        ArmorTrialPreparation.Controls(diagnostic, package, true, declaration);
        // Reborn: changing either member's value must reject reuse of this compiled pair before creating a new package.
        foreach (int index in new[] { 0, 1 })
        {
            var xml = new XmlDocument { XmlResolver = null }; xml.LoadXml(pair[index]);
            var gun = xml.DocumentElement!.ChildNodes.OfType<XmlElement>().Single(item => item.GetAttribute("Damage") == "GUN");
            gun.SetAttribute("Percent", gun.GetAttribute("Percent") == "100" ? "250" : "100");
            string[] wrong = pair.ToArray(); wrong[index] = xml.DocumentElement.OuterXml;
            string rejected = Path.Combine(directory, "wrong-member-" + index); bool refused = false;
            try { ArmorTrialPreparation.Prepare(diagnostic, rejected, true, Declaration(wrong)); } catch (InvalidDataException) { refused = true; }
            if (!refused || Directory.Exists(rejected)) throw new InvalidDataException("Pair member source mismatch accepted.");
        }
        string archive = Path.Combine(package, "RebornArmorProbe.big");
        var receipt = new { ArmorCount = 2, Assets = Values(pair), ActualCoreOutputUsed = true, StockBaselineCompared = true,
            AuthoredPayloadVerified = true, DefaultReaderLayout = true, ArchiveEntryCount = 15, CorruptionControlsPassed = true,
            WrongMemberSourceRefused = true, PackagePath = archive, ConfigPath = Path.Combine(package, "reborn_1.0.cfg"),
            ArchiveSha256 = Convert.ToHexString(System.Security.Cryptography.SHA256.HashData(File.ReadAllBytes(archive))),
            InstalledFilesModified = false, GameExecuted = false, RuntimeOverrideProven = false };
        File.WriteAllText(Path.Combine(directory, "pair-build.json"), JsonSerializer.Serialize(receipt, new JsonSerializerOptions { WriteIndented = true }));
        Console.WriteLine("Authored armor pair: PASS; " + archive);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: refuse ordinary source/output redirection; hostile concurrent replacement is outside trusted local authoring scope. */
    //-------------------------------------------------------------------------------------------------
    private static void Check(string path)
    {
        for (FileSystemInfo? item = File.Exists(path) ? new FileInfo(path) : new DirectoryInfo(path); item != null;
            item = item is FileInfo file ? file.Directory : ((DirectoryInfo)item).Parent)
            if ((item.Attributes & FileAttributes.ReparsePoint) != 0) throw new InvalidDataException("Pair redirected path refused.");
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: pin repeat/order invariance, both full value sets and explicit refusal controls in a fresh owned test tree. */
    //-------------------------------------------------------------------------------------------------
    internal static void SelfTest(string source, string directory, string stockManifest)
    {
        directory = Path.GetFullPath(directory); Check(Path.GetDirectoryName(directory)!);
        if (Directory.Exists(directory) || File.Exists(directory)) throw new InvalidDataException("Pair regression output occupied.");
        Check(Path.GetFullPath(source));
        if (new FileInfo(source).Length > 16384) throw new InvalidDataException("Pair regression input oversized.");
        string[] pair = Admit(File.ReadAllText(source));
        Directory.CreateDirectory(directory);
        string normal = Path.Combine(directory, "normal.xml"), reverse = Path.Combine(directory, "reverse.xml");
        File.WriteAllText(normal, Declaration(pair)); File.WriteAllText(reverse, Declaration(pair.Reverse().ToArray()));
        Run(normal, Path.Combine(directory, "normal"), stockManifest); Run(reverse, Path.Combine(directory, "reverse"), stockManifest);
        byte[] original = File.ReadAllBytes(Path.Combine(directory, "normal", "package", "RebornArmorProbe.big"));
        if (!original.SequenceEqual(File.ReadAllBytes(Path.Combine(directory, "reverse", "package", "RebornArmorProbe.big"))))
            throw new InvalidDataException("Pair order changed canonical package.");
        bool occupied = false;
        try { Run(normal, Path.Combine(directory, "normal"), stockManifest); } catch (InvalidDataException) { occupied = true; }
        if (!occupied || !original.SequenceEqual(File.ReadAllBytes(Path.Combine(directory, "normal", "package", "RebornArmorProbe.big"))))
            throw new InvalidDataException("Pair occupied output changed.");
        string[] faults = { Declaration(new[] { pair[0], pair[0] }), Declaration(new[] { pair[0] }),
            Declaration(new[] { pair[0], pair[1], pair[0] }), Declaration(pair).Replace("SovietConYardArmor", "OtherArmor"),
            Declaration(pair).Replace("<ArmorTemplate", "<WeaponTemplate").Replace("</ArmorTemplate>", "</WeaponTemplate>"),
            "<!DOCTYPE AssetDeclaration [<!ENTITY x '1'>]>" + Declaration(pair), Declaration(pair).Replace("<ArmorTemplate", "<Include source=\"x.xml\"/><ArmorTemplate"),
            Declaration(pair).Replace("Percent=\"5000\"", "Percent=\"10001\"") };
        for (int index = 0; index < faults.Length; index++)
        {
            string rejectedSource = Path.Combine(directory, "fault-" + index + ".xml"), rejectedOutput = Path.Combine(directory, "fault-" + index);
            File.WriteAllText(rejectedSource, faults[index]); bool refused = false;
            try { Stage(rejectedSource, rejectedOutput); } catch (InvalidDataException) { refused = true; } catch (XmlException) { refused = true; }
            if (!refused || Directory.Exists(rejectedOutput)) throw new InvalidDataException("Unsupported pair created admitted output.");
        }
        // Reborn: change all thirty-six fields together, independently stock-compare both identities and require a new deterministic package.
        var changed = pair.Select(xml => { var document = new XmlDocument { XmlResolver = null }; document.LoadXml(xml);
            foreach (string field in ArmorAuthoringAdmission.ScalarFields) document.DocumentElement!.SetAttribute(field, "73");
            foreach (var entry in document.DocumentElement!.ChildNodes.OfType<XmlElement>()) entry.SetAttribute("Percent", "73");
            return document.DocumentElement.OuterXml; }).ToArray();
        string edited = Path.Combine(directory, "edited.xml"); File.WriteAllText(edited, Declaration(changed));
        Run(edited, Path.Combine(directory, "edited"), stockManifest);
        if (original.SequenceEqual(File.ReadAllBytes(Path.Combine(directory, "edited", "package", "RebornArmorProbe.big")))) throw new InvalidDataException("Pair edits did not change package.");
        Console.WriteLine("Armor pair regression: PASS (real combined package/reordered repeat/36 values/8 refusals/member mismatch/corruption/no-overwrite; no game).");
    }
}
