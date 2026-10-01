using System.Buffers.Binary;
using System.Xml;
using System.Xml.Schema;
using BinaryAssetBuilder.Core;
using BinaryAssetBuilder.Utility;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: extend the diagnostic command to proven weak filters without treating weak GameObject names as resolvable strong imports.
internal static class DiagnosticFilterBuildSmokeTest
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: compare eleven filter outputs to native goldens, then verify mixed included families, weak IDs, mutation and rejection behavior. */
    //-------------------------------------------------------------------------------------------------
    internal static void Run()
    {
        string fixtures = Path.GetDirectoryName(ReferencePipelineSmokeTest.FindFixture())!;
        string directory = Path.Combine(Path.GetTempPath(), "Reborn-DiagnosticFilter-" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(directory);
        Settings saved = Settings.Current;
        string source = Path.Combine(directory, "source.xml"), child = Path.Combine(directory, "child.xml");
        XmlDocument stock = new(); stock.Load(Path.Combine(fixtures, "ObjectFilterProbe.xml"));
        XmlSchemaSet schemas = new() { XmlResolver = new XmlUrlResolver() };
        schemas.Add(null, Path.Combine(fixtures, "ObjectFilterPipeline.xsd")); schemas.Compile();
        string stockXml = stock.OuterXml; File.WriteAllText(source, stockXml);
        string stockOutput = BoundedDiagnosticBuild.Build(source, Path.Combine(directory, "stock"), Array.Empty<string>());
        ManifestDocument metadata = ManifestReader.Read(File.ReadAllBytes(stockOutput));
        Require(metadata.Assets.Count == 11 && metadata.Header.AssetReferenceBufferSize == 0 && metadata.ReferencedManifests.Count == 0,
            "Weak-only stock filters acquired strong dependency metadata.");
        int bin = 8, relo = 8;
        foreach (ManifestAsset asset in metadata.Assets)
        {
            XmlElement original = stock.DocumentElement!.ChildNodes.OfType<XmlElement>().Single(element => "ObjectFilterAsset:" + element.GetAttribute("id") == asset.Name);
            var golden = ObjectFilterNativeSmokeTest.Compile(schemas, original.OuterXml);
            Require(asset.TypeId == 0x44A5973Du && asset.TypeHash == 0xDF72B4BAu && asset.Tokenized == 0 && asset.SourceFile == "source.xml"
                && asset.ImportsDataSize == 0 && asset.References.Count == 0
                && AssetStreamProbe.ReadRange(Path.ChangeExtension(stockOutput, ".bin"), null, bin, asset.InstanceDataSize).SequenceEqual(golden.InstanceBuffer)
                && AssetStreamProbe.ReadRange(Path.ChangeExtension(stockOutput, ".relo"), null, relo, asset.RelocationDataSize).SequenceEqual(golden.RelocationBuffer),
                "Diagnostic filter slice differs from its stock-proven standalone native golden.");
            bin += asset.InstanceDataSize; relo += asset.RelocationDataSize;
        }
        Require(new FileInfo(Path.ChangeExtension(stockOutput, ".imp")).Length == 8 && File.ReadAllText(source) == stockXml,
            "Weak filters generated native imports or modified source.");
        const string filter = """
            <ObjectFilterAsset id="DiagnosticFilter"><Filter Relationship="ENEMIES">
            <IncludeThing>AlliedInfiltrationInfantry</IncludeThing><IncludeThing>JapanInfiltrationInfantry</IncludeThing>
            </Filter></ObjectFilterAsset>
            """;
        const string shader = """<ShaderOverride id="DiagnosticShader"><Rule ReplaceShaderName="Null.fx" ReplaceTechniqueName="Default" /></ShaderOverride>""";
        File.WriteAllText(child, Xml(filter + shader));
        const string parent = """
            <Includes><Include type="all" source="child.xml" /></Includes>
            <AttributeModifier id="DiagnosticModifier" Shader="DiagnosticShader" />
            """;
        File.WriteAllText(source, Xml(parent));
        string mixedOutput = BoundedDiagnosticBuild.Build(source, Path.Combine(directory, "mixed"), Array.Empty<string>());
        ManifestDocument mixed = ManifestReader.Read(File.ReadAllBytes(mixedOutput));
        Require(mixed.Assets.Select(asset => asset.TypeName).SequenceEqual(new[] { "ShaderOverride", "ObjectFilterAsset", "AttributeModifier" })
            && mixed.Header.TotalInstanceDataSize == 232 && mixed.Header.AssetReferenceBufferSize == 8,
            "Three-family deterministic order, native total or reference table differs.");
        ManifestAsset filterAsset = mixed.Assets[1], modifier = mixed.Assets[2];
        Require(filterAsset.SourceFile == "input-0001.xml" && filterAsset.InstanceDataSize == 132 && filterAsset.RelocationDataSize == 8
            && filterAsset.ImportsDataSize == 0 && filterAsset.References.Count == 0 && modifier.SourceFile == "source.xml"
            && modifier.References.Single() == new AssetId(mixed.Assets[0].TypeId, mixed.Assets[0].InstanceId),
            "Mixed filter weak IDs leaked into strong imports or source attribution differs.");
        byte[] native = File.ReadAllBytes(Path.ChangeExtension(mixedOutput, ".bin"));
        Require(Read(native, 8 + 40 + 124) == InstanceHandle.GetInstanceId("AlliedInfiltrationInfantry")
            && Read(native, 8 + 40 + 128) == InstanceHandle.GetInstanceId("JapanInfiltrationInfantry")
            && Read(native, 8 + 40 + 132 + 56) == 1, "Mixed native weak IDs/order or modifier shader selector differs.");
        string repeat = BoundedDiagnosticBuild.Build(source, Path.Combine(directory, "repeat"), Array.Empty<string>());
        foreach (string extension in new[] { ".manifest", ".bin", ".relo", ".imp" })
            Require(File.ReadAllBytes(Path.ChangeExtension(mixedOutput, extension)).SequenceEqual(File.ReadAllBytes(Path.ChangeExtension(repeat, extension))),
                "Three-family output is nondeterministic.");
        // Reborn: the official weak schema rejects Type:name syntax; retain its constraint instead of widening source admission.
        File.WriteAllText(child, Xml(filter.Replace(">AlliedInfiltrationInfantry<", ">GameObject:AlliedInfiltrationInfantry<", StringComparison.Ordinal) + shader));
        Reject(source, directory, saved);
        File.WriteAllText(child, Xml(filter.Replace("AlliedInfiltrationInfantry", "RebornUnresolvedWeakName", StringComparison.Ordinal) + shader));
        string changed = BoundedDiagnosticBuild.Build(source, Path.Combine(directory, "changed"), Array.Empty<string>());
        byte[] changedNative = File.ReadAllBytes(Path.ChangeExtension(changed, ".bin"));
        Require(Read(changedNative, 8 + 40 + 124) == InstanceHandle.GetInstanceId("RebornUnresolvedWeakName")
            && Read(changedNative, 8 + 40 + 128) == InstanceHandle.GetInstanceId("JapanInfiltrationInfantry"),
            "Fresh weak source edit retained a stale ID or required an absent weak target.");
        // Reborn: instance-only unreferenced filters stay tentative rather than acquiring an invented strong edge from modifiers.
        File.WriteAllText(child, Xml(filter + shader));
        File.WriteAllText(source, Xml(parent.Replace("type=\"all\"", "type=\"instance\"", StringComparison.Ordinal)));
        ManifestDocument tentative = ManifestReader.Read(File.ReadAllBytes(BoundedDiagnosticBuild.Build(source, Path.Combine(directory, "tentative"), Array.Empty<string>())));
        Require(tentative.Assets.Count == 2 && tentative.Assets.All(asset => asset.TypeName != "ObjectFilterAsset"), "Unused tentative filter was forced into output.");
        foreach (string invalid in new[]
        {
            filter.Replace("Relationship=\"ENEMIES\"", "Rule=\"ANY\"", StringComparison.Ordinal),
            filter.Replace("Relationship=\"ENEMIES\"", "Relationship=\"ALLIES\"", StringComparison.Ordinal),
            filter.Replace("Relationship=\"ENEMIES\"", "StatusBitFlags=\"NO_BRIBE\"", StringComparison.Ordinal),
            filter.Replace("Relationship=\"ENEMIES\"", "Exclude=\"INFANTRY\"", StringComparison.Ordinal),
            filter.Replace("IncludeThing", "ExcludeThing", StringComparison.Ordinal),
            filter.Replace("AlliedInfiltrationInfantry", "=Unresolved", StringComparison.Ordinal),
            filter.Replace("AlliedInfiltrationInfantry", "FXList:WrongType", StringComparison.Ordinal),
            "<ObjectFilterAsset id=\"TooMany\"><Filter>" + string.Concat(Enumerable.Range(0, 16).Select(index => $"<IncludeThing>Weak{index}</IncludeThing>")) + "</Filter></ObjectFilterAsset>",
            "<ObjectFilterAsset id=\"Nested\"><Filter><IncludeThing><Rule /></IncludeThing></Filter></ObjectFilterAsset>",
            "<ObjectFilterAsset id=\"Poisoned\"><Filter TypeId=\"0\" /></ObjectFilterAsset>"
        })
        {
            File.WriteAllText(source, Xml(invalid)); Reject(source, directory, saved);
        }
        // Reborn: adding unrelated external lookup metadata does not turn weak filter IDs into linked reference entries.
        string external = Path.Combine(directory, "external.manifest");
        ExternalLinkSmokeTest.WriteFixture(external, new InstanceHandle("GameObject", "AlliedInfiltrationInfantry"), new ReferencedFileBuffer());
        File.WriteAllText(source, Xml(filter));
        ManifestDocument mapped = ManifestReader.Read(File.ReadAllBytes(BoundedDiagnosticBuild.Build(source, Path.Combine(directory, "mapped"), new[] { external + "=base/objects.manifest" })));
        Require(mapped.Assets.Single().References.Count == 0 && mapped.Assets.Single().ImportsDataSize == 0
            && mapped.Header.AssetReferenceBufferSize == 0 && ReferenceEquals(Settings.Current, saved), "External mapping promoted filter weak IDs or settings changed.");
        Console.WriteLine("Diagnostic filter build self-test: OK (eleven native goldens, three-family Include/order/source readback, weak IDs without strong imports, fresh weak names, tentative exclusion, official weak syntax and eligibility/tamper rejection)");
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: wrap only harness-owned root text in the official EA declaration namespace. */
    //-------------------------------------------------------------------------------------------------
    private static string Xml(string body) => "<AssetDeclaration xmlns=\"uri:ea.com:eala:asset\">" + body + "</AssetDeclaration>";

    //-------------------------------------------------------------------------------------------------
    /** Reborn: inspect explicit little-endian native words rather than assuming process pointer layouts. */
    //-------------------------------------------------------------------------------------------------
    private static uint Read(byte[] data, int offset) => BinaryPrimitives.ReadUInt32LittleEndian(data.AsSpan(offset, 4));

    //-------------------------------------------------------------------------------------------------
    /** Reborn: unsupported filter input must not publish output, retain staging files or mutate caller settings. */
    //-------------------------------------------------------------------------------------------------
    private static void Reject(string source, string directory, Settings saved)
    {
        string output = Path.Combine(directory, "rejected-" + Guid.NewGuid().ToString("N")); bool rejected = false;
        try { BoundedDiagnosticBuild.Build(source, output, Array.Empty<string>()); }
        catch (Exception error) when (error is InvalidDataException or NotSupportedException or BinaryAssetBuilderException or XmlSchemaException) { rejected = true; }
        Require(rejected && !Directory.Exists(output) && !Directory.EnumerateDirectories(directory, ".reborn-diagnostic-*").Any()
            && ReferenceEquals(Settings.Current, saved), "Unsupported filter published output or leaked caller state.");
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: stop weak-filter diagnostic proof at the first native, reference or publication mismatch. */
    //-------------------------------------------------------------------------------------------------
    private static void Require(bool condition, string message)
    {
        if (!condition) throw new InvalidDataException(message);
    }
}
