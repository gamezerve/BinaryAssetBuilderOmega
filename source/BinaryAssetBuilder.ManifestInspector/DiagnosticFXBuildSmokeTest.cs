using System.Buffers.Binary;
using System.Xml;
using System.Xml.Schema;
using BinaryAssetBuilder.Core;
using BinaryAssetBuilder.Core.SageXml;
using BinaryAssetBuilder.Utility;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: exercise narrow FX admission through the actual bounded build service, preserving snapshots, external mapping and publication guards.
internal static class DiagnosticFXBuildSmokeTest
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: prove stock FX buffers, four-family Include graphs, concrete audio resolution and guarded publication through the public diagnostic service. */
    //-------------------------------------------------------------------------------------------------
    internal static void Run(params string[] stockManifests)
    {
        string fixtures = Path.GetDirectoryName(ReferencePipelineSmokeTest.FindFixture())!;
        FXListNativeSmokeTest.CheckExtractedEnums(fixtures, "DiagnosticAssetPipeline.xsd");
        string directory = Path.Combine(Path.GetTempPath(), "Reborn-DiagnosticFX-" + Guid.NewGuid().ToString("N")); Directory.CreateDirectory(directory);
        Settings saved = Settings.Current;
        string source = Path.Combine(directory, "source.xml"), child = Path.Combine(directory, "child.xml"), leaf = Path.Combine(directory, "leaf.xml");
        InstanceHandle[] audio = { new("AudioEvent", "ImpactDebrisHitsGround"), new("AudioEvent", "TEMP_RA2_AlliedAir_VoiceCrash"), new("Multisound", "GDI_Generic_VoiceDieMS") };
        string[] manifests = audio.Select((handle, index) =>
        {
            string path = Path.Combine(directory, "audio-" + index + ".manifest");
            // Reborn: synthetic identity fixtures now carry observed stock fingerprints, not arbitrary audio ABI claims.
            ExternalLinkSmokeTest.WriteFixture(path, handle, new ReferencedFileBuffer(),
                typeHash: handle.TypeId == 0x844D7B9Fu ? 0x560C2E45u : 0xF79C5A89u, tokenized: false); return path;
        }).ToArray();
        string[] mappings = manifests.Select((path, index) => path + "=data/audio-" + index + ".manifest").ToArray();
        File.Copy(Path.Combine(fixtures, "FXListProbe.xml"), source);
        string stockOutput = BoundedDiagnosticBuild.Build(source, Path.Combine(directory, "stock"), mappings);
        CheckFXGoldens(stockOutput, fixtures);
        const string sound = """
            <FXList id="DiagnosticSoundFX"><NuggetList>
            <Sound RequiredSourceModelConditions="FLYING" Value="TEMP_RA2_AlliedAir_VoiceCrash" />
            <Sound ExcludedSourceModelConditions="FLYING" Value="GDI_Generic_VoiceDieMS" />
            </NuggetList></FXList>
            """;
        const string leafBody = """
            <FXList id="DiagnosticEmptyFX"><NuggetList /></FXList>
            <FXList id="UnusedSoundFX"><NuggetList><Sound Value="AbsentUnusedAudio" /></NuggetList></FXList>
            """;
        const string childBody = """
            <Includes><Include type="instance" source="leaf.xml" /></Includes>
            <ShaderOverride id="DiagnosticShader"><Rule ReplaceShaderName="Null.fx" ReplaceTechniqueName="Default" /></ShaderOverride>
            <ObjectFilterAsset id="DiagnosticFilter"><Filter /></ObjectFilterAsset>
            """;
        const string parentBody = """
            <Includes><Include type="all" source="child.xml" /></Includes>
            <AttributeModifier id="DiagnosticModifier" StartFX="DiagnosticSoundFX" EndFX="DiagnosticEmptyFX" Shader="DiagnosticShader" />
            """;
        File.WriteAllText(leaf, Xml(sound + leafBody)); File.WriteAllText(child, Xml(childBody)); File.WriteAllText(source, Xml(parentBody));
        string[] originals = { File.ReadAllText(source), File.ReadAllText(child), File.ReadAllText(leaf) };
        string mixedOutput = BoundedDiagnosticBuild.Build(source, Path.Combine(directory, "mixed"), mappings);
        CheckMixed(mixedOutput, audio);
        Require(originals.SequenceEqual(new[] { File.ReadAllText(source), File.ReadAllText(child), File.ReadAllText(leaf) }), "FX build changed caller source files.");
        string repeat = BoundedDiagnosticBuild.Build(source, Path.Combine(directory, "repeat"), mappings);
        EqualFiles(mixedOutput, repeat);
        // Reborn: freeze actual FX children, then prove edited live audio names cannot replace the approved snapshot's identity.
        DiagnosticSourceGraph frozen = DiagnosticSourceGraph.Read(source);
        File.WriteAllText(leaf, Xml(sound.Replace("GDI_Generic_VoiceDieMS", "MissingChangedAudio", StringComparison.Ordinal) + leafBody));
        string snapshot = Path.Combine(directory, "frozen"); Directory.CreateDirectory(snapshot); frozen.Write(snapshot, new List<string>());
        string frozenOutput = BoundedDiagnosticBuild.Build(Path.Combine(snapshot, "source.xml"), Path.Combine(directory, "frozen-output"), mappings);
        EqualFiles(mixedOutput, frozenOutput); Reject(source, directory, mappings, saved); Reject(source, directory, mappings, saved);
        File.WriteAllText(leaf, originals[2]);
        // Reborn: duplicate exact identities across runtime streams remain forbidden even when the core identity index deduplicates them.
        string duplicate = Path.Combine(directory, "duplicate.manifest");
        ExternalLinkSmokeTest.WriteFixture(duplicate, audio[1], new ReferencedFileBuffer(), typeHash: 0x560C2E45u, tokenized: false);
        Reject(source, directory, mappings.Append(duplicate + "=data/duplicate.manifest").ToArray(), saved);
        string ambiguous = Path.Combine(directory, "ambiguous.manifest");
        ExternalLinkSmokeTest.WriteFixture(ambiguous, new InstanceHandle("Multisound", audio[1].InstanceName), new ReferencedFileBuffer(),
            typeHash: 0xF79C5A89u, tokenized: false);
        Exception ambiguity = Reject(source, directory, mappings.Append(ambiguous + "=data/ambiguous.manifest").ToArray(), saved);
        Require(ambiguity is BinaryAssetBuilderException error && error.ErrorCode == ErrorCode.ReferencingError, "Ambiguous audio did not report a strict referencing error.");
        string wrong = Path.Combine(directory, "wrong.manifest"); ExternalLinkSmokeTest.WriteFixture(wrong, new InstanceHandle("FXList", audio[2].InstanceName), new ReferencedFileBuffer());
        Reject(source, directory, mappings.Take(2).Append(wrong + "=data/wrong.manifest").ToArray(), saved);
        Reject(source, directory, mappings.Take(2).ToArray(), saved); Reject(source, directory, mappings.Take(2).ToArray(), saved);
        string recovered = BoundedDiagnosticBuild.Build(source, Path.Combine(directory, "recovered"), mappings); EqualFiles(mixedOutput, recovered);
        // Reborn: reject wrong hashes and tokenization for both concrete audio families, then prove deterministic recovery and unused-record scope.
        foreach (int index in new[] { 1, 2 })
        {
            byte[] approved = File.ReadAllBytes(manifests[index]);
            uint expectedHash = index == 1 ? 0x560C2E45u : 0xF79C5A89u;
            try
            {
                foreach (bool badToken in new[] { false, true })
                {
                    ExternalLinkSmokeTest.WriteFixture(manifests[index], audio[index], new ReferencedFileBuffer(),
                        typeHash: badToken ? expectedHash : expectedHash ^ 1u, tokenized: badToken);
                    Exception failure = Reject(source, directory, mappings, saved);
                    Require(failure is InvalidDataException && failure.Message.Contains("External audio fingerprint", StringComparison.Ordinal),
                        "Wrong audio fingerprint did not reject at the selected dependency guard.");
                }
            }
            finally { File.WriteAllBytes(manifests[index], approved); }
            string restored = BoundedDiagnosticBuild.Build(source, Path.Combine(directory, "fingerprint-restored-" + index), mappings);
            EqualFiles(mixedOutput, restored);
        }
        string unused = Path.Combine(directory, "unused-audio.manifest");
        ExternalLinkSmokeTest.WriteFixture(unused, new InstanceHandle("AudioEvent", "UnusedWrongFingerprint"), new ReferencedFileBuffer());
        string unusedOutput = BoundedDiagnosticBuild.Build(source, Path.Combine(directory, "unused-fingerprint"),
            mappings.Append(unused + "=data/unused-audio.manifest").ToArray());
        Require(ManifestReader.Read(File.ReadAllBytes(unusedOutput)).Assets.Count == 5,
            "An unselected external audio record incorrectly blocked the bounded build.");
        // Reborn: explicitly authored concrete audio types must compile through command preflight, while a wrong sibling type must not widen during resolution.
        foreach (InstanceHandle target in new[] { audio[0], audio[2] })
        {
            File.WriteAllText(source, Xml("<FXList id=\"TypedSoundFX\"><NuggetList><Sound Value=\"" + target.Name + "\" /></NuggetList></FXList>"));
            ManifestDocument typed = ManifestReader.Read(File.ReadAllBytes(BoundedDiagnosticBuild.Build(source,
                Path.Combine(directory, "typed-" + target.TypeName), mappings)));
            Require(typed.Assets.Single().InstanceDataSize == 80 && typed.Assets.Single().References.Single() == new AssetId(target.TypeId, target.InstanceId),
                "Explicit concrete audio type did not survive command normalization/resolution.");
        }
        File.WriteAllText(source, Xml("<FXList id=\"WrongTypedFX\"><NuggetList><Sound Value=\"Multisound:ImpactDebrisHitsGround\" /></NuggetList></FXList>"));
        Reject(source, directory, mappings, saved);
        foreach (string invalid in new[]
        {
            sound.Replace("RequiredSourceModelConditions=\"FLYING\"", "RequiredSourceModelConditions=\"USER_1\"", StringComparison.Ordinal),
            sound.Replace("RequiredSourceModelConditions=\"FLYING\"", "RequiredSecondaryModelConditions=\"FLYING\"", StringComparison.Ordinal),
            sound.Replace("<Sound ", "<Sound StopIfPlayed=\"true\" ", StringComparison.Ordinal),
            sound.Replace("<Sound ", "<Sound Weather=\"SUNNY\" ", StringComparison.Ordinal),
            sound.Replace("<FXList id=", "<FXList Tailorable=\"true\" id=", StringComparison.Ordinal),
            sound.Replace("TEMP_RA2_AlliedAir_VoiceCrash", "=Unresolved", StringComparison.Ordinal),
            sound.Replace("TEMP_RA2_AlliedAir_VoiceCrash", "TEMP_RA2_AlliedAir_VoiceCrash\\0", StringComparison.Ordinal),
            "<FXList id=\"TooManySounds\"><NuggetList>" + string.Concat(Enumerable.Repeat("<Sound Value=\"ImpactDebrisHitsGround\" />", 3)) + "</NuggetList></FXList>",
            "<FXList id=\"Particle\"><NuggetList><EvaEvent /></NuggetList></FXList>",
            "<FXList id=\"Nested\"><NuggetList><Sound Value=\"ImpactDebrisHitsGround\"><SourceObjectFilter /></Sound></NuggetList></FXList>",
            "<FXList id=\"Poisoned\"><NuggetList><Sound TypeId=\"0\" Value=\"ImpactDebrisHitsGround\" /></NuggetList></FXList>",
            "<AudioEvent id=\"UnadmittedAudioRoot\" />",
            string.Concat(Enumerable.Range(0, 33).Select(index => "<FXList id=\"TooManyFX" + index + "\"><NuggetList /></FXList>"))
        })
        { File.WriteAllText(source, Xml(invalid)); Reject(source, directory, mappings, saved); }
        File.WriteAllText(source, originals[0]);
        File.WriteAllText(leaf, Xml(sound + leafBody + "<FXList id=\"diagnosticsoundfx\"><NuggetList /></FXList>"));
        Reject(source, directory, mappings, saved); File.WriteAllText(leaf, originals[2]);
        // Reborn: source/audio files edited after compilation cannot change the approved snapshots or staged publication.
        byte[] originalManifest = File.ReadAllBytes(manifests[2]);
        string frozenStage;
        try
        {
            frozenStage = BoundedDiagnosticBuild.Build(source, Path.Combine(directory, "frozen-stage"), mappings, _ =>
            {
                File.WriteAllText(leaf, Xml(sound.Replace("GDI_Generic_VoiceDieMS", "MissingChangedAudio", StringComparison.Ordinal) + leafBody));
                ExternalLinkSmokeTest.WriteFixture(manifests[2], new InstanceHandle("FXList", audio[2].InstanceName), new ReferencedFileBuffer());
            });
        }
        finally { File.WriteAllText(leaf, originals[2]); File.WriteAllBytes(manifests[2], originalManifest); }
        EqualFiles(mixedOutput, frozenStage);
        Reject(source, directory, mappings, saved, Path.GetDirectoryName(mixedOutput)); EqualFiles(mixedOutput, repeat);
        string late = Path.Combine(directory, "late-failure");
        Reject(source, directory, mappings, saved, late, stage =>
        {
            string path = Path.Combine(stage, "diagnostic.bin"); byte[] bytes = File.ReadAllBytes(path); bytes[4] ^= 1; File.WriteAllBytes(path, bytes);
        });
        string race = Path.Combine(directory, "raced"); bool raced = false;
        try { BoundedDiagnosticBuild.Build(source, race, mappings, _ => { Directory.CreateDirectory(race); File.WriteAllText(Path.Combine(race, "owner.txt"), "preserve"); }); }
        catch (IOException) { raced = true; }
        Require(raced && File.ReadAllText(Path.Combine(race, "owner.txt")) == "preserve" && !File.Exists(Path.Combine(race, "diagnostic.manifest"))
            && !Directory.EnumerateDirectories(directory, ".reborn-diagnostic-*").Any() && ReferenceEquals(Settings.Current, saved), "FX destination race damaged existing state.");
        // Reborn: the committed user-facing example must build through the same public service, including stock sound targets and weak filter leaves.
        ManifestDocument example = ManifestReader.Read(File.ReadAllBytes(BoundedDiagnosticBuild.Build(Path.Combine(fixtures, "DiagnosticFXProbe.xml"),
            Path.Combine(directory, "committed-example"), mappings)));
        Require(example.Assets.Count == 5 && example.Header.TotalInstanceDataSize == 564 && example.Header.AssetReferenceBufferSize == 48,
            "Committed four-family FX example differs from its documented native totals.");
        if (stockManifests.Length > 0)
        {
            File.Copy(Path.Combine(fixtures, "FXListProbe.xml"), source, true);
            string[] realMappings = stockManifests.Select((path, index) => path + "=data/stock-" + index + ".manifest").ToArray();
            string actual = BoundedDiagnosticBuild.Build(source, Path.Combine(directory, "real-stock"), realMappings); CheckFXGoldens(actual, fixtures);
            ManifestDocument metadata = ManifestReader.Read(File.ReadAllBytes(actual));
            Dictionary<string, (Relo.Chunk Data, uint[] Names)> chunks = new(StringComparer.Ordinal);
            int bin = 8, relo = 8, imp = 8;
            foreach (ManifestAsset asset in metadata.Assets)
            {
                chunks.Add(asset.Name, (new Relo.Chunk { InstanceBuffer = AssetStreamProbe.ReadRange(Path.ChangeExtension(actual, ".bin"), null, bin, asset.InstanceDataSize),
                    RelocationBuffer = AssetStreamProbe.ReadRange(Path.ChangeExtension(actual, ".relo"), null, relo, asset.RelocationDataSize),
                    ImportsBuffer = AssetStreamProbe.ReadRange(Path.ChangeExtension(actual, ".imp"), null, imp, asset.ImportsDataSize) }, asset.References.Select(reference => reference.InstanceId).ToArray()));
                bin += asset.InstanceDataSize; relo += asset.RelocationDataSize; imp += asset.ImportsDataSize;
            }
            int comparisons = 0;
            foreach (string path in stockManifests)
            {
                ManifestDocument stock = ManifestReader.Read(File.ReadAllBytes(path));
                if (!stock.Assets.Any(asset => chunks.ContainsKey(asset.Name))) continue;
                foreach (ManifestAsset asset in stock.Assets.Where(asset => chunks.ContainsKey(asset.Name)))
                    Require(asset.References.SequenceEqual(metadata.Assets.Single(compiled => compiled.Name == asset.Name).References), "Real diagnostic build concrete references differ from stock.");
                FXListNativeSmokeTest.Compare(chunks, path); comparisons++;
            }
            Require(comparisons > 0, "Real stock test requires the static manifest containing all three FX roots.");
        }
        Require(ReferenceEquals(Settings.Current, saved), "Diagnostic FX tests did not restore settings.");
        Console.WriteLine("Diagnostic FX build self-test: OK (three FX goldens, four-family Include/order/source/selector readback, tentative exclusion, concrete audio mapping, duplicate/ambiguous/missing rejection, frozen/edited inputs, limits, corruption/race/existing-output preservation)");
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: verify known empty/sound native sizes, exact import selectors and concrete audio identities in command-produced streams. */
    //-------------------------------------------------------------------------------------------------
    private static void CheckFXGoldens(string path, string fixtures)
    {
        ManifestDocument metadata = ManifestReader.Read(File.ReadAllBytes(path));
        Require(metadata.Assets.Count == 3 && metadata.Assets.All(asset => asset.TypeId == 0x86682E78u && asset.TypeHash == 0x17B3B82Du && asset.SourceFile == "source.xml")
            && metadata.Assets.Sum(asset => asset.InstanceDataSize) == 360, "Command FX native roots or source attribution differs.");
        XmlDocument original = new(); original.Load(Path.Combine(fixtures, "FXListProbe.xml"));
        // Reborn: rebuild independent native goldens from official schema defaults and explicit ordered source selectors, without the command registry or serializer.
        XmlSchemaSet schemas = new() { XmlResolver = new XmlUrlResolver() }; schemas.Add(null, Path.Combine(fixtures, "FXListPipeline.xsd")); schemas.Compile();
        original.Schemas.Add(schemas); original.Validate((_, args) => throw new XmlSchemaValidationException(args.Message));
        Dictionary<string, Relo.Chunk> expected = new(StringComparer.Ordinal);
        foreach (XmlElement root in original.DocumentElement!.ChildNodes.OfType<XmlElement>())
        {
            int index = 0;
            foreach (XmlElement sound in root.SelectNodes("descendant::*[local-name()='Sound']")!)
                sound.SetAttribute("Value", sound.GetAttribute("Value") + "\\" + index++);
            foreach (XmlElement element in root.SelectNodes("descendant-or-self::*")!)
                element.SetAttribute("TypeId", InstanceHandle.GetTypeId(element.SchemaInfo.SchemaType!.Name).ToString(System.Globalization.CultureInfo.InvariantCulture));
            expected.Add("FXList:" + root.GetAttribute("id"), FXListNativeSmokeTest.Compile(new InstanceDeclaration(new AssetDeclarationDocument()) { XmlNode = root }));
        }
        int bin = 8, relo = 8, imp = 8;
        foreach (ManifestAsset asset in metadata.Assets)
        {
            byte[] native = AssetStreamProbe.ReadRange(Path.ChangeExtension(path, ".bin"), null, bin, asset.InstanceDataSize);
            Relo.Chunk golden = expected[asset.Name];
            Require(native.SequenceEqual(golden.InstanceBuffer)
                && AssetStreamProbe.ReadRange(Path.ChangeExtension(path, ".relo"), null, relo, asset.RelocationDataSize).SequenceEqual(golden.RelocationBuffer)
                && AssetStreamProbe.ReadRange(Path.ChangeExtension(path, ".imp"), null, imp, asset.ImportsDataSize).SequenceEqual(golden.ImportsBuffer), "Command FX slices differ from independent native goldens.");
            if (asset.Name == "FXList:FX_NONE") Require(native.Length == 28 && native.All(value => value == 0) && asset.References.Count == 0, "Empty command FX differs.");
            else if (asset.Name == "FXList:FX_DebrisHitGround") Require(native.Length == 80 && Read(native, 76) == 1 && asset.References.Single() == Id("AudioEvent", "ImpactDebrisHitsGround"), "Single command FX audio differs.");
            else Require(native.Length == 252 && Read(native, 80) == 1 && Read(native, 188) == 2
                && asset.References.SequenceEqual(new[] { Id("AudioEvent", "TEMP_RA2_AlliedAir_VoiceCrash"), Id("Multisound", "GDI_Generic_VoiceDieMS") }), "Two-Sound command FX differs.");
            bin += asset.InstanceDataSize; relo += asset.RelocationDataSize; imp += asset.ImportsDataSize;
        }
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: decode the command's four-family offsets and import tables while ensuring unused tentative FX/audio never enter output. */
    //-------------------------------------------------------------------------------------------------
    private static void CheckMixed(string path, InstanceHandle[] audio)
    {
        ManifestDocument metadata = ManifestReader.Read(File.ReadAllBytes(path)); byte[] bin = File.ReadAllBytes(Path.ChangeExtension(path, ".bin"));
        Require(metadata.Assets.Select(asset => asset.TypeName).SequenceEqual(new[] { "ShaderOverride", "ObjectFilterAsset", "FXList", "FXList", "AttributeModifier" })
            && metadata.Header.TotalInstanceDataSize == 504 && metadata.Header.AssetReferenceBufferSize == 40 && metadata.ReferencedManifests.Count == 3,
            "Four-family diagnostic order, counts or native totals differ.");
        ManifestAsset empty = metadata.Assets[2], sound = metadata.Assets[3], modifier = metadata.Assets[4];
        Require(empty.Name == "FXList:DiagnosticEmptyFX" && empty.References.Count == 0 && sound.SourceFile == "input-0002.xml" && empty.SourceFile == sound.SourceFile
            && metadata.Assets[0].SourceFile == "input-0001.xml" && metadata.Assets[1].SourceFile == "input-0001.xml" && modifier.SourceFile == "source.xml"
            && sound.References.SequenceEqual(audio.Skip(1).Select(handle => new AssetId(handle.TypeId, handle.InstanceId)))
            && Read(bin, 8 + 40 + 124 + 28 + 80) == 1 && Read(bin, 8 + 40 + 124 + 28 + 188) == 2,
            "Four-family source attribution or concrete audio selectors differ.");
        int root = 8 + 40 + 124 + 28 + 252;
        uint start = Read(bin, root + 12), end = Read(bin, root + 16), shader = Read(bin, root + 56);
        Require(start > 0 && start <= 3 && end > 0 && end <= 3 && shader > 0 && shader <= 3
            && modifier.References[(int)start - 1] == new AssetId(sound.TypeId, sound.InstanceId)
            && modifier.References[(int)end - 1] == new AssetId(empty.TypeId, empty.InstanceId)
            && modifier.References[(int)shader - 1] == new AssetId(metadata.Assets[0].TypeId, metadata.Assets[0].InstanceId), "Modifier imports chose wrong command-produced local targets.");
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: require rejected FX builds to preserve caller settings, existing output and known staging cleanup. */
    //-------------------------------------------------------------------------------------------------
    private static Exception Reject(string source, string directory, string[] mappings, Settings saved, string? output = null, Action<string>? stage = null)
    {
        output ??= Path.Combine(directory, "rejected-" + Guid.NewGuid().ToString("N")); bool existed = Directory.Exists(output);
        try { BoundedDiagnosticBuild.Build(source, output, mappings, stage); }
        catch (Exception error) when (error is InvalidDataException or NotSupportedException or BinaryAssetBuilderException or XmlException or XmlSchemaException)
        {
            Require((existed || !Directory.Exists(output)) && ReferenceEquals(Settings.Current, saved)
                && !Directory.EnumerateDirectories(directory, ".reborn-diagnostic-*").Any(), "Rejected FX build published output, leaked staging or changed settings."); return error;
        }
        throw new InvalidDataException("Unsupported FX build was accepted.");
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: compare complete diagnostic streams and warning text across deterministic recompilation/publication. */
    //-------------------------------------------------------------------------------------------------
    private static void EqualFiles(string first, string second)
    {
        foreach (string extension in new[] { ".manifest", ".bin", ".relo", ".imp" })
            Require(File.ReadAllBytes(Path.ChangeExtension(first, extension)).SequenceEqual(File.ReadAllBytes(Path.ChangeExtension(second, extension))), "FX diagnostic output differs.");
        Require(File.ReadAllText(Path.Combine(Path.GetDirectoryName(first)!, "DIAGNOSTIC_ONLY.txt")) == File.ReadAllText(Path.Combine(Path.GetDirectoryName(second)!, "DIAGNOSTIC_ONLY.txt")), "Diagnostic FX warnings differ.");
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: create known comparison identities using the same invariant asset hashing contract as the core. */
    //-------------------------------------------------------------------------------------------------
    private static AssetId Id(string type, string name)
    {
        InstanceHandle handle = new(type, name); return new AssetId(handle.TypeId, handle.InstanceId);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: wrap only test-owned literal asset roots in the EA declaration namespace. */
    //-------------------------------------------------------------------------------------------------
    private static string Xml(string body) => "<AssetDeclaration xmlns=\"uri:ea.com:eala:asset\">" + body + "</AssetDeclaration>";

    //-------------------------------------------------------------------------------------------------
    /** Reborn: decode bounded little-endian native selectors independently of host pointer values. */
    //-------------------------------------------------------------------------------------------------
    private static uint Read(byte[] bytes, int offset) => BinaryPrimitives.ReadUInt32LittleEndian(bytes.AsSpan(offset, 4));

    //-------------------------------------------------------------------------------------------------
    /** Reborn: stop public FX command proof on the first admission, identity, native or publication mismatch. */
    //-------------------------------------------------------------------------------------------------
    private static void Require(bool condition, string message)
    {
        if (!condition) throw new InvalidDataException(message);
    }
}
