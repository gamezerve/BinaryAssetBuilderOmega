using System.Security.Cryptography;
using System.Text.Json;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: bind the bounded authored snapshot to real Core output and the user-tested default-reader layout, never install or launch.
internal static class ArmorAuthoredBuild
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: compare actual variable packages, repeated output, old recipe digest and no-overwrite refusal using owned fixtures only. */
    //-------------------------------------------------------------------------------------------------
    internal static void SelfTest(string directory, string stockManifest)
    {
        directory = Path.GetFullPath(directory);
        if (Directory.Exists(directory) || File.Exists(directory)) throw new InvalidDataException("Authored regression output already exists.");
        Directory.CreateDirectory(directory);
        string fixture = File.ReadAllText(Path.Combine(Path.GetDirectoryName(ReferencePipelineSmokeTest.FindFixture())!, "AlliedConYardArmorProbe.xml"));
        var packages = new Dictionary<int, byte[]>();
        var hashes = new HashSet<uint>(); byte[]? baseline = null;
        foreach (int value in new[] { 0, 100, 250, 5000, 10000 })
        {
            string source = Path.Combine(directory, $"gun-{value}.xml");
            File.WriteAllText(source, fixture.Replace("Percent=\"5000\"", $"Percent=\"{value}\""));
            byte[] original = File.ReadAllBytes(source);
            string output = Path.Combine(directory, $"build-{value}"); Run(source, output, stockManifest);
            if (!original.SequenceEqual(File.ReadAllBytes(source))) throw new InvalidDataException("Authored input modified.");
            string archivePath = Path.Combine(output, "package", "RebornArmorProbe.big");
            packages[value] = File.ReadAllBytes(archivePath);
            using var archive = BigArchive.Open(archivePath, 15, 4096);
            var manifest = ManifestReader.Read(archive.ReadEntry(archive.Entries[0], 1024 * 1024));
            if (!hashes.Add(manifest.Assets.Single().InstanceHash)) throw new InvalidDataException("Authored edit did not invalidate Core identity.");
            byte[] bin = archive.ReadEntry(archive.Entries[1], 1024 * 1024).AsSpan(8).ToArray();
            if (baseline != null && (!baseline.AsSpan(0, 788).SequenceEqual(bin.AsSpan(0, 788))
                || !baseline.AsSpan(792).SequenceEqual(bin.AsSpan(792)))) throw new InvalidDataException("Variable package changed non-GUN bytes.");
            baseline = bin;
            bool refused = false;
            try { Run(source, output, stockManifest); } catch (InvalidDataException) { refused = true; }
            if (!refused || !packages[value].SequenceEqual(File.ReadAllBytes(archivePath))) throw new InvalidDataException("Occupied authored output changed.");
        }
        Run(Path.Combine(directory, "gun-250.xml"), Path.Combine(directory, "repeat-250"), stockManifest);
        if (!packages[250].SequenceEqual(File.ReadAllBytes(Path.Combine(directory, "repeat-250", "package", "RebornArmorProbe.big"))))
            throw new InvalidDataException("Repeated authored package changed.");
        if (Convert.ToHexString(SHA256.HashData(packages[5000])) != "DD7DE09E3A81B9714DC2861ADE0AA9ECDFE72A1851EDD1ED20BE2155EF31C2DC")
            throw new InvalidDataException("Legacy user-tested 5000 package changed.");
        ExpandedSelfTest(directory, fixture, stockManifest);
        SovietIdentitySelfTest(directory, fixture, stockManifest);
        Console.WriteLine("Authored armor regression: PASS (five coefficients, fresh identities, single float edit, repeated package, legacy digest, occupied/source/corruption refusals; no game).");
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: exercise each percentage through actual Core/stock/default packaging and a combined edit; preserve legacy tests and never launch. */
    //-------------------------------------------------------------------------------------------------
    private static void ExpandedSelfTest(string directory, string fixture, string stockManifest)
    {
        var fields = ArmorAuthoringAdmission.Values(ArmorAuthoringAdmission.Admit(System.Text.Encoding.UTF8.GetBytes(fixture)).Xml);
        var combined = new System.Xml.XmlDocument { XmlResolver = null }; combined.LoadXml(fixture);
        int index = 0;
        foreach (string field in fields.Keys)
        {
            var xml = new System.Xml.XmlDocument { XmlResolver = null }; xml.LoadXml(fixture);
            SetValue(xml, field, "73"); SetValue(combined, field, "250");
            string source = Path.Combine(directory, $"field-{index}.xml"); File.WriteAllText(source, xml.OuterXml);
            string output = Path.Combine(directory, $"field-{index++}"); Run(source, output, stockManifest);
            using var archive = BigArchive.Open(Path.Combine(output, "package", "RebornArmorProbe.big"), 15, 4096);
            byte[] actual = archive.ReadEntry(archive.Entries[1], 1024 * 1024).AsSpan(8).ToArray();
            var root = new System.Xml.XmlDocument { XmlResolver = null }; root.LoadXml(fixture);
            SetValue(root, "Armor/GUN", "100");
            string fixtures = Path.GetDirectoryName(ReferencePipelineSmokeTest.FindFixture())!;
            var stock = ArmorTokenSmokeTest.Compile(ArmorTokenSmokeTest.LoadSchemas(fixtures), root.DocumentElement!.OuterXml);
            ArmorAuthoringAdmission.VerifyPayload(xml.DocumentElement!.OuterXml, stock.InstanceBuffer, actual);
            // Reborn: only this field may differ from the legacy fixed-GUN fixture in a one-field case.
            var legacy = ArmorTokenSmokeTest.Compile(ArmorTokenSmokeTest.LoadSchemas(fixtures), new System.Xml.XmlDocument { InnerXml = fixture }.DocumentElement!.OuterXml);
            actual.AsSpan(fields[field].Offset, 4).Clear(); legacy.InstanceBuffer.AsSpan(fields[field].Offset, 4).Clear();
            if (!actual.SequenceEqual(legacy.InstanceBuffer)) throw new InvalidDataException("One-field package changed another coefficient.");
        }
        string mixed = Path.Combine(directory, "combined.xml"); File.WriteAllText(mixed, combined.OuterXml);
        Run(mixed, Path.Combine(directory, "combined"), stockManifest);
        Run(mixed, Path.Combine(directory, "combined-repeat"), stockManifest);
        if (!File.ReadAllBytes(Path.Combine(directory, "combined", "package", "RebornArmorProbe.big")).SequenceEqual(
            File.ReadAllBytes(Path.Combine(directory, "combined-repeat", "package", "RebornArmorProbe.big")))) throw new InvalidDataException("Repeated combined edit changed package.");
        Console.WriteLine("Expanded armor regression: PASS (eighteen single-field packages, combined/repeat, stock comparison and detached corruption/mismatch controls; no game).");
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: change only an identified owned fixture field while keeping the thirteen-entry list and original identity. */
    //-------------------------------------------------------------------------------------------------
    private static void SetValue(System.Xml.XmlDocument xml, string field, string value)
    {
        if (field.StartsWith("Armor/")) xml.DocumentElement!.ChildNodes.OfType<System.Xml.XmlElement>().Single(item => item.GetAttribute("Damage") == field.Substring(6)).SetAttribute("Percent", value);
        else xml.DocumentElement!.SetAttribute(field, value);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: prove Soviet stock equality, field edits, identity separation, repeat output and foreign-source refusal using actual Core packages. */
    //-------------------------------------------------------------------------------------------------
    private static void SovietIdentitySelfTest(string directory, string fixture, string stockManifest)
    {
        string soviet = fixture.Replace("AlliedConYardArmor", "SovietConYardArmor");
        var fields = ArmorAuthoringAdmission.Values(ArmorAuthoringAdmission.Admit(System.Text.Encoding.UTF8.GetBytes(soviet)).Xml);
        int index = 0;
        foreach (string field in fields.Keys)
        {
            var xml = new System.Xml.XmlDocument { XmlResolver = null }; xml.LoadXml(soviet);
            SetValue(xml, field, "73");
            string source = Path.Combine(directory, $"soviet-field-{index}.xml"); File.WriteAllText(source, xml.OuterXml);
            Run(source, Path.Combine(directory, $"soviet-field-{index++}"), stockManifest);
        }
        string fixedSource = Path.Combine(directory, "soviet-gun.xml"); File.WriteAllText(fixedSource, soviet);
        string first = Path.Combine(directory, "soviet-gun"), repeat = Path.Combine(directory, "soviet-repeat");
        Run(fixedSource, first, stockManifest); Run(fixedSource, repeat, stockManifest);
        byte[] package = File.ReadAllBytes(Path.Combine(first, "package", "RebornArmorProbe.big"));
        if (!package.SequenceEqual(File.ReadAllBytes(Path.Combine(repeat, "package", "RebornArmorProbe.big"))))
            throw new InvalidDataException("Repeated Soviet package differs.");
        using var archive = BigArchive.Open(Path.Combine(first, "package", "RebornArmorProbe.big"), 15, 4096);
        var entry = ManifestReader.Read(archive.ReadEntry(archive.Entries[0], 1024 * 1024)).Assets.Single();
        var expected = new BinaryAssetBuilder.Core.InstanceHandle("ArmorTemplate", "SovietConYardArmor");
        if (entry.InstanceId != expected.InstanceId || entry.Name != expected.Name || package.SequenceEqual(
            File.ReadAllBytes(Path.Combine(directory, "build-5000", "package", "RebornArmorProbe.big"))))
            throw new InvalidDataException("Soviet package lost identity separation.");
        // Reborn: use the actual generated diagnostic path rather than assuming a file name.
        string diagnostic = Directory.GetFiles(Path.Combine(first, "core"), "*.big", SearchOption.AllDirectories).Single();
        string refusedPath = Path.Combine(directory, "soviet-foreign-source");
        bool refused = false;
        try { ArmorTrialPreparation.Prepare(diagnostic, refusedPath, true, fixture); }
        catch (InvalidDataException) { refused = true; }
        if (!refused || Directory.Exists(refusedPath)) throw new InvalidDataException("Foreign Allied snapshot accepted for Soviet package.");
        Console.WriteLine($"Soviet armor identity regression: PASS (eighteen fields, own stock baseline, repeat, foreign snapshot refusal; InstanceId={entry.InstanceId:X8}; no game).");
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: require stock comparison, fresh output and exact independently reproduced payloads before publishing a success receipt. */
    //-------------------------------------------------------------------------------------------------
    internal static void Run(string source, string directory, string stockManifest)
    {
        directory = Path.GetFullPath(directory);
        stockManifest = Path.GetFullPath(stockManifest);
        if (!File.Exists(stockManifest)) throw new InvalidDataException("Authored build requires the stock manifest and its linked streams.");
        ArmorAuthoringAdmission.Stage(source, directory);
        string snapshot = Path.Combine(directory, "admitted-armor.xml");
        var admitted = ArmorAuthoringAdmission.Admit(File.ReadAllBytes(snapshot));
        string core = Path.Combine(directory, "core"); Directory.CreateDirectory(core);
        string diagnostic = Ep1ArmorDocumentSmokeTest.BuildAuthored(core, admitted.Xml, admitted.GunPercent, stockManifest);
        string package = Path.Combine(directory, "package");
        ArmorTrialPreparation.Prepare(diagnostic, package, true, admitted.Xml);
        ArmorTrialPreparation.Controls(diagnostic, package, true, admitted.Xml);
        // Reborn: mismatched admitted XML must not relabel an existing compiled package or create a candidate directory.
        if(SdkBuildTests.Enabled){
        var wrong = ArmorAuthoringAdmission.Admit(System.Text.Encoding.UTF8.GetBytes(
            admitted.Xml.Replace($"Damage=\"GUN\" Percent=\"{admitted.GunPercent}\"", $"Damage=\"GUN\" Percent=\"{(admitted.GunPercent == 100 ? 250 : 100)}\"")));
        bool refused = false;
        string rejected = Path.Combine(directory, "wrong-source-package");
        try { ArmorTrialPreparation.Prepare(diagnostic, rejected, true, wrong.Xml); }
        catch (InvalidDataException) { refused = true; }
        if (!refused || Directory.Exists(rejected)) throw new InvalidDataException("Authored package/source mismatch accepted.");
        }
        string archive = Path.Combine(package, "RebornArmorProbe.big");
        var receipt = new { ArmorId = ArmorAuthoringAdmission.Identity(admitted.Xml), admitted.GunPercent, Snapshot = snapshot,
            EditableFloatCount = 18, Percentages = ArmorAuthoringAdmission.Values(admitted.Xml).ToDictionary(item => item.Key, item => item.Value.Percent),
            SnapshotSha256 = Convert.ToHexString(SHA256.HashData(File.ReadAllBytes(snapshot))),
            PackagePath = archive, ConfigPath = Path.Combine(package, "reborn_1.0.cfg"),
            ModSkuPath = Path.Combine(package, "reborn_1.0.skudef"),
            ArchiveSha256 = Convert.ToHexString(SHA256.HashData(File.ReadAllBytes(archive))),
            StockBaselineCompared = true, ActualCoreOutputUsed = true, AuthoredPayloadVerified = true,
            CanonicalFixedRecipeUsed = false, ArchiveEntryCount = 15, DefaultReaderLayout = true,
            CorruptionControlsPassed = SdkBuildTests.Enabled, WrongSourceRefused = SdkBuildTests.Enabled,
            InstalledFilesModified = false, GameExecuted = false, RuntimeOverrideProven = false };
        File.WriteAllText(Path.Combine(directory, "authored-build.json"), JsonSerializer.Serialize(receipt, new JsonSerializerOptions { WriteIndented = true }));
        Console.WriteLine($"Authored armor build: PASS (GUN={admitted.GunPercent}; real Core + verified default package; no game): {archive}");
    }
}
