using System.Xml;
using BinaryAssetBuilder.Core;
using BinaryAssetBuilder.Utility;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: public command service sound admission must retain snapshot, identity, cycle and publication safeguards.
internal static class DiagnosticMultisoundBuildSmokeTest
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: exercise five-family Includes, nested local ordering, sound rejection/recovery and optional real stock mappings through Build. */
    //-------------------------------------------------------------------------------------------------
    internal static void Run(params string[] stockManifests)
    {
        string directory = Path.Combine(Path.GetTempPath(), "Reborn-DiagnosticMultisound-" + Guid.NewGuid().ToString("N")); Directory.CreateDirectory(directory);
        Settings saved = Settings.Current;
        string source = Path.Combine(directory, "source.xml"), sounds = Path.Combine(directory, "sounds.xml");
        File.WriteAllText(Path.Combine(directory, "basics.xml"), Xml("<ShaderOverride id=\"LocalShader\"><Rule ReplaceShaderName=\"Null.fx\" ReplaceTechniqueName=\"Default\" /></ShaderOverride><ObjectFilterAsset id=\"LocalFilter\"><Filter /></ObjectFilterAsset>"));
        string original = Xml("<Multisound id=\"AConsumer\" Control=\"PLAY_ONE\"><Subsound>GDI_Commando_VoiceDie</Subsound><Subsound Weight=\"800\">GDI_Engineer_VoiceDie</Subsound></Multisound>"
            + "<Multisound id=\"Unused\"><Subsound>MissingUnused</Subsound></Multisound>");
        File.WriteAllText(sounds, original);
        File.WriteAllText(source, Xml("<Includes><Include type=\"all\" source=\"basics.xml\" /><Include type=\"instance\" source=\"sounds.xml\" /></Includes>"
            + "<FXList id=\"LocalFX\"><NuggetList><Sound Value=\"AConsumer\" /></NuggetList></FXList><AttributeModifier id=\"LocalModifier\" StartFX=\"LocalFX\" />"));
        string[] paths = new[] { "GDI_Commando_VoiceDie","GDI_Engineer_VoiceDie" }.Select((name, index) =>
        {
            string path = Path.Combine(directory, "audio-" + index + ".manifest"); ExternalLinkSmokeTest.WriteFixture(path, new InstanceHandle("AudioEvent", name),
                new ReferencedFileBuffer(), typeHash: 0x560C2E45u, tokenized: false); return path;
        }).ToArray();
        string[] mappings = paths.Select((path, index) => path + "=data/audio-" + index + ".manifest").ToArray();
        string first = BoundedDiagnosticBuild.Build(source, Path.Combine(directory, "first"), mappings); Check(first);
        string repeat = BoundedDiagnosticBuild.Build(source, Path.Combine(directory, "repeat"), mappings); Same(first, repeat);
        Require(File.ReadAllText(sounds) == original && ReferenceEquals(saved, Settings.Current), "Command changed caller sound source/settings.");
        // Reborn: frozen Include snapshots preserve approved names; fresh missing target names reject and restored sources recover.
        DiagnosticSourceGraph frozen = DiagnosticSourceGraph.Read(source);
        File.WriteAllText(sounds, original.Replace("GDI_Commando_VoiceDie", "MissingChanged", StringComparison.Ordinal));
        Reject(source, directory, mappings, saved);
        string frozenDirectory = Path.Combine(directory, "frozen"); Directory.CreateDirectory(frozenDirectory); frozen.Write(frozenDirectory, new List<string>());
        Same(first, BoundedDiagnosticBuild.Build(Path.Combine(frozenDirectory, "source.xml"), Path.Combine(directory, "frozen-output"), mappings));
        File.WriteAllText(sounds, original);
        // Reborn: approved source/metadata snapshots must survive later caller edits during staged publication.
        byte[] approved = File.ReadAllBytes(paths[0]);
        string staged;
        try
        {
            staged = BoundedDiagnosticBuild.Build(source, Path.Combine(directory, "snapshot-stage"), mappings, _ =>
            {
                File.WriteAllText(sounds, original.Replace("GDI_Commando_VoiceDie", "MissingChanged", StringComparison.Ordinal));
                ExternalLinkSmokeTest.WriteFixture(paths[0], new InstanceHandle("FXList", "GDI_Commando_VoiceDie"), new ReferencedFileBuffer());
            });
        }
        finally { File.WriteAllText(sounds, original); File.WriteAllBytes(paths[0], approved); }
        Same(first, staged);
        foreach (bool badToken in new[] { false,true })
        {
            try
            {
                ExternalLinkSmokeTest.WriteFixture(paths[0], new InstanceHandle("AudioEvent", "GDI_Commando_VoiceDie"), new ReferencedFileBuffer(),
                    typeHash: badToken ? 0x560C2E45u : 0x560C2E44u, tokenized: badToken);
                Reject(source, directory, mappings, saved);
            }
            finally { File.WriteAllBytes(paths[0], approved); }
        }
        string duplicate = Path.Combine(directory, "duplicate.manifest"); File.Copy(paths[0], duplicate);
        Reject(source, directory, mappings.Append(duplicate + "=data/duplicate.manifest").ToArray(), saved);
        Reject(source, directory, mappings.Take(1).ToArray(), saved); Reject(source, directory, mappings.Take(1).ToArray(), saved);
        Same(first, BoundedDiagnosticBuild.Build(source, Path.Combine(directory, "recovered"), mappings));
        // Reborn: the checked-in CLI example and a no-dependency empty sound must use the same public build service.
        string fixtures = Path.GetDirectoryName(ReferencePipelineSmokeTest.FindFixture())!;
        ManifestDocument example = ManifestReader.Read(File.ReadAllBytes(BoundedDiagnosticBuild.Build(Path.Combine(fixtures, "DiagnosticMultisoundProbe.xml"),
            Path.Combine(directory, "committed-example"), mappings)));
        Require(example.Assets.Select(asset => asset.TypeName).SequenceEqual(new[] { "Multisound","FXList","AttributeModifier" })
            && example.Header.TotalInstanceDataSize == 208, "Committed sound command example differs.");
        string emptySource = Path.Combine(directory, "empty.xml"); File.WriteAllText(emptySource, Xml("<Multisound id=\"Empty\" />"));
        ManifestDocument empty = ManifestReader.Read(File.ReadAllBytes(BoundedDiagnosticBuild.Build(emptySource, Path.Combine(directory, "empty"), Array.Empty<string>())));
        Require(empty.Assets.Single().InstanceDataSize == 16 && empty.Assets.Single().References.Count == 0 && empty.ReferencedManifests.Count == 0,
            "Empty sound unexpectedly required external mappings or produced imports.");
        // Reborn: a same-family dependency that sorts after its consumer must still be emitted first; preserve real per-entry reference order.
        File.WriteAllText(sounds, Xml("<Multisound id=\"AConsumer\"><Subsound>Multisound:ZDependency</Subsound></Multisound>"
            + "<Multisound id=\"ZDependency\"><Subsound>GDI_Commando_VoiceDie</Subsound></Multisound>"));
        ManifestDocument nested = ManifestReader.Read(File.ReadAllBytes(BoundedDiagnosticBuild.Build(source, Path.Combine(directory, "nested"), mappings)));
        Require(nested.Assets.Select(asset => asset.Name).SequenceEqual(new[] { "ShaderOverride:LocalShader","ObjectFilterAsset:LocalFilter",
            "Multisound:ZDependency","Multisound:AConsumer","FXList:LocalFX","AttributeModifier:LocalModifier" })
            && nested.Assets[3].References.Single() == new AssetId(0xA3A7AF37u, InstanceHandle.GetInstanceId("ZDependency")), "Nested sounds were not dependency-first.");
        int negativeIndex = 0;
        foreach (string invalid in new[]
        {
            "<Multisound id=\"AConsumer\"><Subsound>AConsumer</Subsound></Multisound>",
            "<Multisound id=\"AConsumer\"><Subsound>ZDependency</Subsound></Multisound><Multisound id=\"ZDependency\"><Subsound>AConsumer</Subsound></Multisound>",
            "<Multisound id=\"AConsumer\" Control=\"LOOP\"><Subsound>GDI_Commando_VoiceDie</Subsound></Multisound>",
            "<Multisound id=\"AConsumer\"><Subsound Volume=\"100\">GDI_Commando_VoiceDie</Subsound></Multisound>",
            "<Multisound id=\"AConsumer\"><Subsound>GDI_Commando_VoiceDie\\0</Subsound></Multisound>",
            "<Multisound id=\"AConsumer\"><Subsound>=Unknown</Subsound></Multisound>",
            "<Multisound id=\"AConsumer\"><Subsound TypeId=\"0\">GDI_Commando_VoiceDie</Subsound></Multisound>",
            "<Multisound id=\"AConsumer\" inheritFrom=\"Other\" />",
            "<Multisound id=\"AConsumer\" /><Multisound id=\"aconsumer\" />",
            "<Multisound id=\"AConsumer\"><Subsound><Nested /></Subsound></Multisound>",
            string.Concat(Enumerable.Range(0,33).Select(index => "<Multisound id=\"Root" + index + "\" />"))
        })
        {
            File.WriteAllText(sounds, Xml(invalid)); Exception error = Reject(source, directory, mappings, saved);
            if (negativeIndex++ < 2) Require(error is InvalidDataException && error.Message.Contains("local dependency cycle", StringComparison.Ordinal),
                "Selected sound cycle was not rejected by dependency ordering.");
        }
        File.WriteAllText(sounds, original);
        Reject(source, directory, mappings, saved, Path.GetDirectoryName(first)); Same(first, repeat);
        Reject(source, directory, mappings, saved, hook: stage => { string path = Path.Combine(stage, "diagnostic.bin"); byte[] bytes = File.ReadAllBytes(path); bytes[4] ^= 1; File.WriteAllBytes(path, bytes); });
        string race = Path.Combine(directory, "race");
        Reject(source, directory, mappings, saved, race, _ => { Directory.CreateDirectory(race); File.WriteAllText(Path.Combine(race, "owner.txt"), "preserve"); });
        Require(File.ReadAllText(Path.Combine(race, "owner.txt")) == "preserve" && !File.Exists(Path.Combine(race, "diagnostic.manifest")), "Raced sound output was replaced.");
        if (stockManifests.Length > 0)
        {
            string[] stockMappings = stockManifests.Select((path, index) => path + "=data/stock-" + index + ".manifest").ToArray();
            Check(BoundedDiagnosticBuild.Build(source, Path.Combine(directory, "real-stock"), stockMappings));
            Console.WriteLine("  Real EP1 diagnostic Multisound command: five families, local sound/FX/modifier selectors and stock AudioEvent fingerprints OK.");
        }
        Require(!Directory.EnumerateDirectories(directory, ".reborn-diagnostic-*").Any() && ReferenceEquals(saved, Settings.Current), "Sound build left staging/settings state.");
        Console.WriteLine("Diagnostic Multisound build self-test: OK (five families, Include snapshots, nested ordering/cycle rejection, metadata uniqueness/fingerprints, edited/frozen/recovered inputs, existing/corrupt/raced publication)");
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: independently read exact five-family identities, native offsets and local/external selector tables. */
    //-------------------------------------------------------------------------------------------------
    private static void Check(string path)
    {
        ManifestDocument metadata = ManifestReader.Read(File.ReadAllBytes(path)); byte[] bin = File.ReadAllBytes(Path.ChangeExtension(path, ".bin"));
        Require(metadata.Assets.Select(asset => asset.TypeName).SequenceEqual(new[] { "ShaderOverride","ObjectFilterAsset","Multisound","FXList","AttributeModifier" })
            && metadata.Header.TotalInstanceDataSize == 372 && metadata.Header.AssetReferenceBufferSize == 32 && bin.Length == 380, "Five-family totals/order differ.");
        Require(metadata.Assets[2].TypeHash == 0xF79C5A89u && metadata.Assets[2].SourceFile == "input-0002.xml"
            && metadata.Assets[2].References.SequenceEqual(new[] { new AssetId(0x844D7B9Fu, InstanceHandle.GetInstanceId("GDI_Commando_VoiceDie")),
                new AssetId(0x844D7B9Fu, InstanceHandle.GetInstanceId("GDI_Engineer_VoiceDie")) })
            && metadata.Assets[3].References.Single() == new AssetId(0xA3A7AF37u, InstanceHandle.GetInstanceId("AConsumer"))
            && metadata.Assets[4].References.Single() == new AssetId(0x86682E78u, InstanceHandle.GetInstanceId("LocalFX"))
            && BitConverter.ToUInt32(bin, 188) == 1 && BitConverter.ToUInt32(bin, 216) == 2
            && BitConverter.ToUInt32(bin, 320) == 1 && BitConverter.ToUInt32(bin, 336) == 1, "Five-family native sound selectors differ.");
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: reject without publication and preserve caller settings; raced/existing owner directories are never removed. */
    //-------------------------------------------------------------------------------------------------
    private static Exception Reject(string source, string directory, string[] mappings, Settings saved, string? output = null, Action<string>? hook = null)
    {
        output ??= Path.Combine(directory, "reject-" + Guid.NewGuid().ToString("N")); bool existed = Directory.Exists(output);
        try { BoundedDiagnosticBuild.Build(source, output, mappings, hook); }
        catch (Exception error) when (error is InvalidDataException or BinaryAssetBuilderException or NotSupportedException or System.Xml.Schema.XmlSchemaException or IOException)
        {
            Require(ReferenceEquals(saved, Settings.Current) && (existed || hook != null || !Directory.Exists(output)), "Rejected sound build changed output/settings."); return error;
        }
        throw new InvalidOperationException("Invalid sound diagnostic was accepted.");
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: compare all five output files for exact source/metadata snapshot recovery. */
    //-------------------------------------------------------------------------------------------------
    private static void Same(string first, string second)
    {
        foreach (string name in new[] { "diagnostic.manifest","diagnostic.bin","diagnostic.relo","diagnostic.imp","DIAGNOSTIC_ONLY.txt" })
            Require(File.ReadAllBytes(Path.Combine(Path.GetDirectoryName(first)!, name)).SequenceEqual(File.ReadAllBytes(Path.Combine(Path.GetDirectoryName(second)!, name))), "Repeated sound command differs.");
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: retain the official namespace in owned proof XML. */
    //-------------------------------------------------------------------------------------------------
    private static string Xml(string body) => "<AssetDeclaration xmlns=\"uri:ea.com:eala:asset\">" + body + "</AssetDeclaration>";

    //-------------------------------------------------------------------------------------------------
    /** Reborn: stop at the first command admission/readback/publication contract violation. */
    //-------------------------------------------------------------------------------------------------
    private static void Require(bool condition, string message) { if (!condition) throw new InvalidDataException(message); }
}
