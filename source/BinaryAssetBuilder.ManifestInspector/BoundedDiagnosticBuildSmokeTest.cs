using System.Buffers.Binary;
using BinaryAssetBuilder.Core;
using BinaryAssetBuilder.Utility;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: validate standalone diagnostic admission, deterministic publication and rejection without overwriting or partially publishing user output.
internal static class BoundedDiagnosticBuildSmokeTest
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: build local and external-reference variants and prove malformed inputs/mappings leave the requested output absent and settings restored. */
    //-------------------------------------------------------------------------------------------------
    internal static void Run()
    {
        string directory = Path.Combine(Path.GetTempPath(), "Reborn-BoundedDiagnosticTest-" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(directory);
        string source = Path.Combine(directory, "input.xml");
        const string valid = """
            <AssetDeclaration xmlns="uri:ea.com:eala:asset">
            <AttributeModifier id="RebornDiagnosticModifier" Shader="RebornDiagnosticShader" />
            <ShaderOverride id="RebornDiagnosticShader"><Rule ReplaceShaderName="Null.fx" ReplaceTechniqueName="Default" /></ShaderOverride>
            </AssetDeclaration>
            """;
        File.WriteAllText(source, valid);
        Settings saved = Settings.Current;
        string first = BoundedDiagnosticBuild.Build(source, Path.Combine(directory, "first"), Array.Empty<string>());
        Require(ReferenceEquals(Settings.Current, saved) && File.ReadAllText(source) == valid, "Diagnostic build changed caller settings or original XML.");
        ManifestDocument parsed = ManifestReader.Read(File.ReadAllBytes(first));
        Require(parsed.Assets.Count == 2 && parsed.Assets[0].TypeName == "ShaderOverride" && parsed.Assets[1].TypeName == "AttributeModifier"
            && parsed.Assets[1].References.Single() == new AssetId(parsed.Assets[0].TypeId, parsed.Assets[0].InstanceId)
            && parsed.ReferencedManifests.Count == 0 && parsed.Header.TotalInstanceDataSize == 100,
            "Bounded diagnostic local output identities/order differ.");
        byte[] bin = File.ReadAllBytes(Path.ChangeExtension(first, ".bin"));
        Require(BinaryPrimitives.ReadUInt32LittleEndian(bin.AsSpan(8 + 40 + 56)) == 1
            && File.Exists(Path.Combine(Path.GetDirectoryName(first)!, "DIAGNOSTIC_ONLY.txt")), "Bounded diagnostic import/notice differs.");
        string repeated = BoundedDiagnosticBuild.Build(source, Path.Combine(directory, "repeat"), Array.Empty<string>());
        foreach (string extension in new[] { ".manifest", ".bin", ".relo", ".imp" })
            Require(File.ReadAllBytes(Path.ChangeExtension(first, extension)).SequenceEqual(File.ReadAllBytes(Path.ChangeExtension(repeated, extension))),
                "Approved snapshot builds are not deterministic.");
        string externalPath = Path.Combine(directory, "external.manifest");
        InstanceHandle fx = new("FXList", "RebornDiagnosticFX");
        ExternalLinkSmokeTest.WriteFixture(externalPath, fx, new ReferencedFileBuffer());
        File.WriteAllText(source, valid.Replace("id=\"RebornDiagnosticModifier\"", "id=\"RebornDiagnosticModifier\" StartFX=\"RebornDiagnosticFX\"", StringComparison.Ordinal));
        string mixed = BoundedDiagnosticBuild.Build(source, Path.Combine(directory, "mixed"), new[] { externalPath + "=Base/External.manifest" });
        ManifestDocument mixedMetadata = ManifestReader.Read(File.ReadAllBytes(mixed));
        Require(mixedMetadata.ReferencedManifests.Single() == new ReferencedManifest("base\\external.manifest", false)
            && mixedMetadata.Assets[1].References.Count == 2 && mixedMetadata.Assets[1].References.Contains(new AssetId(fx.TypeId, fx.InstanceId)),
            "Bounded external mapping lost normalization or dependency identity.");
        File.WriteAllText(source, valid);
        foreach (string invalid in new[]
        {
            valid.Replace("<AttributeModifier ", "<AttributeModifier inheritFrom=\"Base\" ", StringComparison.Ordinal),
            valid.Replace("<AttributeModifier ", "<AttributeModifier Duration=\"=Unresolved\" ", StringComparison.Ordinal),
            valid.Replace("<AttributeModifier ", "<AttributeModifier Duration=\"invalid\" ", StringComparison.Ordinal),
            valid.Replace("<ShaderOverride ", "<ShaderOverride Priority=\"-1\" ", StringComparison.Ordinal),
            valid.Replace("RebornDiagnosticShader", "NüllShader", StringComparison.Ordinal),
            valid.Replace("Shader=\"RebornDiagnosticShader\"", "Shader=\"MissingShader\"", StringComparison.Ordinal),
            valid.Replace("<AssetDeclaration xmlns=\"uri:ea.com:eala:asset\">", "<AssetDeclaration xmlns=\"uri:ea.com:eala:asset\"><Includes><Include type=\"all\" source=\"input.xml\" /></Includes>", StringComparison.Ordinal),
            """<AssetDeclaration xmlns="uri:ea.com:eala:asset"><GameObject id="Unsupported" /></AssetDeclaration>""",
            // Reborn: reject misplaced leaf records and nested assets before schema/core loading.
            """<AssetDeclaration xmlns="uri:ea.com:eala:asset"><Rule ReplaceShaderName="Null.fx" /></AssetDeclaration>""",
            """<AssetDeclaration xmlns="uri:ea.com:eala:asset"><AttributeModifier id="WrongLeaf"><Rule ReplaceShaderName="Null.fx" /></AttributeModifier></AssetDeclaration>""",
            """<AssetDeclaration xmlns="uri:ea.com:eala:asset"><ShaderOverride id="Nested"><ShaderOverride id="Inner" /></ShaderOverride></AssetDeclaration>""",
            """<!DOCTYPE AssetDeclaration [<!ENTITY external SYSTEM "never-read.txt">]><AssetDeclaration xmlns="uri:ea.com:eala:asset" />""",
            "<AssetDeclaration xmlns=\"uri:ea.com:eala:asset\">" + string.Concat(Enumerable.Range(0, 33).Select(index => $"<AttributeModifier id=\"Limit{index}\" />")) + "</AssetDeclaration>"
        })
        {
            File.WriteAllText(source, invalid); ExpectRejected(source, directory, Array.Empty<string>(), saved);
        }
        File.WriteAllText(source, valid);
        foreach (string[] mapping in new[] { new[] { externalPath + "=../escape.manifest" }, new[] { externalPath + "=C:\\absolute.manifest" },
            new[] { externalPath + "=" }, new[] { externalPath }, new[] { externalPath + "=one.manifest", externalPath + "=two.manifest" } })
            ExpectRejected(source, directory, mapping, saved);
        string secondExternal = Path.Combine(directory, "second-external.manifest");
        ExternalLinkSmokeTest.WriteFixture(secondExternal, new InstanceHandle("FXList", "OtherFX"), new ReferencedFileBuffer());
        ExpectRejected(source, directory, new[] { externalPath + "=Base/x.manifest", secondExternal + "=base\\X.manifest" }, saved);
        string wrong = Path.Combine(directory, "wrong-target.manifest");
        ExternalLinkSmokeTest.WriteFixture(wrong, fx, new ReferencedFileBuffer(), 0x54EEE764u);
        ExpectRejected(source, directory, new[] { wrong + "=wrong.manifest" }, saved);
        // Reborn: existing output is never replaced, including unrelated user files that must survive a refused build.
        string sentinel = Path.Combine(Path.GetDirectoryName(first)!, "user-marker.txt"); File.WriteAllText(sentinel, "preserve");
        bool existingRejected = false;
        try { BoundedDiagnosticBuild.Build(source, Path.GetDirectoryName(first)!, Array.Empty<string>()); }
        catch (InvalidDataException) { existingRejected = true; }
        Require(existingRejected && File.ReadAllText(sentinel) == "preserve" && !Directory.EnumerateDirectories(directory, ".reborn-diagnostic-*").Any(),
            "Existing-output rejection damaged user data or left staging output.");
        // Reborn: a late readback failure must remove staged files without publishing the requested destination.
        string corruptOutput = Path.Combine(directory, "corrupt-output"); bool corruptionRejected = false;
        try
        {
            BoundedDiagnosticBuild.Build(source, corruptOutput, Array.Empty<string>(), stage =>
            {
                string file = Path.Combine(stage, "diagnostic.bin"); byte[] data = File.ReadAllBytes(file); data[4] ^= 1; File.WriteAllBytes(file, data);
            });
        }
        catch (InvalidDataException) { corruptionRejected = true; }
        Require(corruptionRejected && !Directory.Exists(corruptOutput) && !Directory.EnumerateDirectories(directory, ".reborn-diagnostic-*").Any(),
            "Late verification failure published output or retained known staging files.");
        // Reborn: if another writer claims the destination after preflight, preserve its contents instead of replacing it.
        string racedOutput = Path.Combine(directory, "raced-output"); bool raceRejected = false;
        try
        {
            BoundedDiagnosticBuild.Build(source, racedOutput, Array.Empty<string>(), _ =>
            { Directory.CreateDirectory(racedOutput); File.WriteAllText(Path.Combine(racedOutput, "owner.txt"), "preserve-race"); });
        }
        catch (IOException) { raceRejected = true; }
        Require(raceRejected && File.ReadAllText(Path.Combine(racedOutput, "owner.txt")) == "preserve-race"
            && !File.Exists(Path.Combine(racedOutput, "diagnostic.manifest")) && !Directory.EnumerateDirectories(directory, ".reborn-diagnostic-*").Any(),
            "Concurrent destination creation overwrote existing files or leaked staging output.");
        Require(ReferenceEquals(Settings.Current, saved), "Diagnostic checks failed to restore settings.");
        Console.WriteLine("Bounded diagnostic build self-test: OK (local/mixed snapshots, native identity readback, deterministic publication, input/mapping limits, unchanged source/settings, existing/raced output preservation, late failure cleanup, no rejected output)");
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: malformed input or unsafe metadata must fail before the requested destination appears and preserve caller state. */
    //-------------------------------------------------------------------------------------------------
    private static void ExpectRejected(string source, string directory, string[] mappings, Settings saved)
    {
        string output = Path.Combine(directory, "rejected-" + Guid.NewGuid().ToString("N"));
        bool rejected = false;
        try { BoundedDiagnosticBuild.Build(source, output, mappings); }
        catch (Exception error) when (error is InvalidDataException or NotSupportedException or BinaryAssetBuilderException or System.Xml.XmlException or System.Xml.Schema.XmlSchemaException)
        { rejected = true; }
        Require(rejected && !Directory.Exists(output) && !File.Exists(output) && ReferenceEquals(Settings.Current, saved)
            && !Directory.EnumerateDirectories(directory, ".reborn-diagnostic-*").Any(), "Rejected build published output, leaked staging or mutated caller settings.");
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: stop bounded command proof on its first admission, publication or identity failure. */
    //-------------------------------------------------------------------------------------------------
    private static void Require(bool condition, string message)
    {
        if (!condition) throw new InvalidDataException(message);
    }
}
