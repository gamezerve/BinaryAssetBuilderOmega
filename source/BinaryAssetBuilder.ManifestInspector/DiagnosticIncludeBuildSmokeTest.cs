using System.Buffers.Binary;
using BinaryAssetBuilder.Core;
using BinaryAssetBuilder.Utility;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: prove public diagnostic Include admission, actual local closure and frozen source identities without production stream writes.
internal static class DiagnosticIncludeBuildSmokeTest
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: exercise nested instance/all graphs, source attribution, edited/removed inputs, confinement and deterministic snapshots. */
    //-------------------------------------------------------------------------------------------------
    internal static void Run()
    {
        string directory = Path.Combine(Path.GetTempPath(), "Reborn-DiagnosticInclude-" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(directory);
        string entry = Path.Combine(directory, "entry.xml"), child = Path.Combine(directory, "child.xml"), leaf = Path.Combine(directory, "leaf.xml");
        const string leafXml = """
            <AssetDeclaration xmlns="uri:ea.com:eala:asset">
            <ShaderOverride id="UsedShader" Priority="50"><Rule ReplaceShaderName="Null.fx" ReplaceTechniqueName="Default" /></ShaderOverride>
            <ShaderOverride id="UnusedShader"><Rule ReplaceShaderName="Null.fx" ReplaceTechniqueName="Default" /></ShaderOverride>
            </AssetDeclaration>
            """;
        File.WriteAllText(leaf, leafXml);
        File.WriteAllText(child, Xml("<Includes><Include type=\"all\" source=\"leaf.xml\" /></Includes>"));
        Settings saved = Settings.Current;
        foreach (string mode in new[] { "instance", "all" })
        {
            File.WriteAllText(entry, Xml($"<Includes><Include type=\"{mode}\" source=\"child.xml\" /></Includes><AttributeModifier id=\"Consumer\" Shader=\"UsedShader\" />"));
            string manifest = BoundedDiagnosticBuild.Build(entry, Path.Combine(directory, mode), Array.Empty<string>());
            ManifestDocument metadata = ManifestReader.Read(File.ReadAllBytes(manifest));
            Require(metadata.Assets.Count == (mode == "instance" ? 2 : 3), "Include selection forced an unused tentative shader or omitted an all shader.");
            ManifestAsset shader = metadata.Assets.Single(asset => asset.Name == "ShaderOverride:UsedShader");
            ManifestAsset modifier = metadata.Assets.Single(asset => asset.Name == "AttributeModifier:Consumer");
            Require(shader.SourceFile == "input-0002.xml" && modifier.SourceFile == "source.xml"
                && modifier.References.Single() == new AssetId(shader.TypeId, shader.InstanceId), "Included source identity or resolved local reference differs.");
            byte[] native = File.ReadAllBytes(Path.ChangeExtension(manifest, ".bin"));
            int offset = 8 + metadata.Assets.TakeWhile(asset => asset != shader).Sum(asset => asset.InstanceDataSize);
            Require(BinaryPrimitives.ReadUInt32LittleEndian(native.AsSpan(offset + 4)) == 50, "Included native priority differs.");
            string repeat = BoundedDiagnosticBuild.Build(entry, Path.Combine(directory, mode + "-repeat"), Array.Empty<string>());
            foreach (string extension in new[] { ".manifest", ".bin", ".relo", ".imp" })
                Require(File.ReadAllBytes(Path.ChangeExtension(manifest, extension)).SequenceEqual(File.ReadAllBytes(Path.ChangeExtension(repeat, extension))), "Include output is nondeterministic.");
        }
        // Reborn: combine an included shader's native closure with explicitly mapped metadata-only FX dependency resolution.
        string external = Path.Combine(directory, "external.manifest");
        InstanceHandle fx = new("FXList", "IncludedFX");
        ExternalLinkSmokeTest.WriteFixture(external, fx, new ReferencedFileBuffer());
        string originalEntry = File.ReadAllText(entry);
        File.WriteAllText(entry, originalEntry.Replace("id=\"Consumer\"", "id=\"Consumer\" StartFX=\"IncludedFX\"", StringComparison.Ordinal));
        ManifestDocument mixed = ManifestReader.Read(File.ReadAllBytes(BoundedDiagnosticBuild.Build(entry, Path.Combine(directory, "mixed"), new[] { external + "=base/fx.manifest" })));
        Require(mixed.Assets.Count == 3 && mixed.Assets.Single(asset => asset.TypeName == "AttributeModifier").References.Contains(new AssetId(fx.TypeId, fx.InstanceId))
            && mixed.ReferencedManifests.Single().Path == "base\\fx.manifest", "Included/mixed external dependency or runtime mapping differs.");
        File.WriteAllText(entry, originalEntry);
        // Reborn: a graph approved before an edit retains the old child XML when written later into an owned snapshot directory.
        DiagnosticSourceGraph frozen = DiagnosticSourceGraph.Read(entry);
        File.WriteAllText(leaf, leafXml.Replace("Priority=\"50\"", "Priority=\"51\"", StringComparison.Ordinal));
        string snapshot = Path.Combine(directory, "frozen"); Directory.CreateDirectory(snapshot); frozen.Write(snapshot, new List<string>());
        Require(File.ReadAllText(Path.Combine(snapshot, "input-0002.xml")).Contains("Priority=\"50\"", StringComparison.Ordinal), "Approved Include snapshot reopened edited source.");
        string frozenOutput = BoundedDiagnosticBuild.Build(Path.Combine(snapshot, "source.xml"), Path.Combine(directory, "frozen-output"), Array.Empty<string>());
        ManifestDocument frozenMetadata = ManifestReader.Read(File.ReadAllBytes(frozenOutput));
        int frozenOffset = 8 + frozenMetadata.Assets.TakeWhile(asset => asset.Name != "ShaderOverride:UsedShader").Sum(asset => asset.InstanceDataSize);
        Require(BinaryPrimitives.ReadUInt32LittleEndian(File.ReadAllBytes(Path.ChangeExtension(frozenOutput, ".bin")).AsSpan(frozenOffset + 4)) == 50,
            "Frozen graph no longer compiled the approved child native value.");
        string edited = BoundedDiagnosticBuild.Build(entry, Path.Combine(directory, "edited"), Array.Empty<string>());
        ManifestDocument editedMetadata = ManifestReader.Read(File.ReadAllBytes(edited));
        ManifestAsset editedShader = editedMetadata.Assets.Single(asset => asset.Name == "ShaderOverride:UsedShader");
        int editedOffset = 8 + editedMetadata.Assets.TakeWhile(asset => asset != editedShader).Sum(asset => asset.InstanceDataSize);
        Require(BinaryPrimitives.ReadUInt32LittleEndian(File.ReadAllBytes(Path.ChangeExtension(edited, ".bin")).AsSpan(editedOffset + 4)) == 51, "Fresh Include build retained old native source.");
        // Reborn: only harness-owned leaf data is removed; repeated failures must leave requested output absent and settings untouched.
        File.Delete(leaf); Reject(entry, directory, saved); Reject(entry, directory, saved);
        File.WriteAllText(leaf, Xml("<Includes><Include type=\"all\" source=\"entry.xml\" /></Includes>"));
        Reject(entry, directory, saved); Reject(entry, directory, saved);
        File.WriteAllText(leaf, leafXml);
        foreach (string source in new[] { "../escape.xml", "C:/absolute.xml", "$DATA/leaf.xml", "sub/../../leaf.xml", "./leaf.xml", "missing.xml", "leaf.bin" })
        {
            File.WriteAllText(entry, Xml($"<Includes><Include type=\"all\" source=\"{source}\" /></Includes>")); Reject(entry, directory, saved);
        }
        File.WriteAllText(entry, Xml("<Includes><Include type=\"reference\" source=\"child.xml\" /></Includes>")); Reject(entry, directory, saved);
        File.WriteAllText(entry, Xml("<Includes><Include type=\"all\" source=\"leaf.xml\" /></Includes><ShaderOverride id=\"UsedShader\"><Rule ReplaceShaderName=\"Null.fx\" /></ShaderOverride>"));
        Reject(entry, directory, saved);
        File.WriteAllText(entry, Xml("<Includes>" + string.Concat(Enumerable.Repeat("<Include type=\"all\" source=\"leaf.xml\" />", 33)) + "</Includes>")); Reject(entry, directory, saved);
        // Reborn: enforce a depth bound before any core load; every generated chain node stays in this private test directory.
        for (int index = 0; index < 10; index++) File.WriteAllText(Path.Combine(directory, $"depth-{index}.xml"),
            index == 9 ? leafXml : Xml($"<Includes><Include type=\"all\" source=\"depth-{index + 1}.xml\" /></Includes>"));
        Reject(Path.Combine(directory, "depth-0.xml"), directory, saved);
        // Reborn: file-count and aggregate-root bounds apply across the graph, not independently per child.
        for (int index = 0; index < 16; index++) File.WriteAllText(Path.Combine(directory, $"wide-{index}.xml"), Xml(""));
        File.WriteAllText(entry, Xml("<Includes>" + string.Concat(Enumerable.Range(0, 16).Select(index => $"<Include type=\"all\" source=\"wide-{index}.xml\" />")) + "</Includes>"));
        Reject(entry, directory, saved);
        File.WriteAllText(child, Xml(string.Concat(Enumerable.Range(0, 31).Select(index => $"<AttributeModifier id=\"Limit{index}\" />"))));
        File.WriteAllText(entry, Xml("<Includes><Include type=\"all\" source=\"child.xml\" /><Include type=\"all\" source=\"leaf.xml\" /></Includes>"));
        Reject(entry, directory, saved);
        // Reborn: nested physical paths rewrite into flat snapshots without changing relative-child resolution.
        string nested = Path.Combine(directory, "nested"); Directory.CreateDirectory(nested);
        File.WriteAllText(Path.Combine(nested, "child.xml"), Xml("<Includes><Include type=\"all\" source=\"leaf.xml\" /></Includes>"));
        File.WriteAllText(Path.Combine(nested, "leaf.xml"), leafXml);
        File.WriteAllText(entry, Xml("<Includes><Include type=\"all\" source=\"nested/child.xml\" /></Includes>"));
        ManifestDocument nestedMetadata = ManifestReader.Read(File.ReadAllBytes(BoundedDiagnosticBuild.Build(entry, Path.Combine(directory, "nested-output"), Array.Empty<string>())));
        Require(nestedMetadata.Assets.Count == 2 && nestedMetadata.Assets.All(asset => asset.SourceFile == "input-0002.xml"), "Nested path snapshot attribution differs.");
        // Reborn: repeated edges to the same physical source share one snapshot and must not duplicate emitted asset identities.
        File.WriteAllText(entry, Xml("<Includes><Include type=\"all\" source=\"leaf.xml\" /><Include type=\"all\" source=\"leaf.xml\" /></Includes>"));
        Require(ManifestReader.Read(File.ReadAllBytes(BoundedDiagnosticBuild.Build(entry, Path.Combine(directory, "shared-output"), Array.Empty<string>()))).Assets.Count == 2,
            "Repeated Include edge duplicated emitted assets.");
        File.WriteAllText(entry, Xml("<Includes><Include type=\"all\" source=\"leaf.xml\" /></Includes>"));
        string recovered = BoundedDiagnosticBuild.Build(entry, Path.Combine(directory, "recovered"), Array.Empty<string>());
        Require(ManifestReader.Read(File.ReadAllBytes(recovered)).Assets.Count == 2 && ReferenceEquals(Settings.Current, saved)
            && File.ReadAllText(leaf) == leafXml, "Includes-only root recovery changed source/settings or omitted all roots.");
        Console.WriteLine("Diagnostic Include build self-test: OK (instance/all selection, nested native closure, source attribution, deterministic/frozen snapshots, edits/loss/cycles/recovery, path/control/identity/depth/edge guards)");
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: assemble an EA namespace wrapper for private graph fixtures without changing official source files. */
    //-------------------------------------------------------------------------------------------------
    private static string Xml(string body) => "<AssetDeclaration xmlns=\"uri:ea.com:eala:asset\">" + body + "</AssetDeclaration>";

    //-------------------------------------------------------------------------------------------------
    /** Reborn: rejected graph attempts must never publish output or retain compiler-global settings. */
    //-------------------------------------------------------------------------------------------------
    private static void Reject(string source, string directory, Settings saved)
    {
        string output = Path.Combine(directory, "rejected-" + Guid.NewGuid().ToString("N"));
        bool rejected = false;
        try { BoundedDiagnosticBuild.Build(source, output, Array.Empty<string>()); }
        catch (Exception error) when (error is InvalidDataException or BinaryAssetBuilderException or System.Xml.XmlException) { rejected = true; }
        Require(rejected && !Directory.Exists(output) && ReferenceEquals(Settings.Current, saved)
            && !Directory.EnumerateDirectories(directory, ".reborn-diagnostic-*").Any(), "Rejected graph published output or retained state.");
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: stop diagnostic Include proof on the first source, closure, native or publication mismatch. */
    //-------------------------------------------------------------------------------------------------
    private static void Require(bool condition, string message)
    {
        if (!condition) throw new InvalidDataException(message);
    }
}
