using System.Buffers.Binary;
using System.Reflection;
using System.Xml;
using BinaryAssetBuilder.Core;
using BinaryAssetBuilder.Core.SageXml;
using BinaryAssetBuilder.Core.Session;
using BinaryAssetBuilder.Utility;
using BinaryAssetBuilder.XmlCompiler;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: fixed modifier/local-FX/local-Multisound closure is diagnostic evidence only, not public audio-root admission.
internal static class MultisoundFXStreamSmokeTest
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: compile a three-level Include chain and verify concrete local/external selectors, fingerprints, independent streams and recovery. */
    //-------------------------------------------------------------------------------------------------
    internal static void Run(params string[] stockManifests)
    {
        string fixtures = Path.GetDirectoryName(ReferencePipelineSmokeTest.FindFixture())!;
        string directory = Path.Combine(Path.GetTempPath(), "Reborn-MultisoundFXStream-" + Guid.NewGuid().ToString("N")); Directory.CreateDirectory(directory);
        Settings saved = Settings.Current;
        try
        {
            FXListNativeSmokeTest.CheckExtractedEnums(fixtures, "MultisoundFXPipeline.xsd");
            Settings.Current = new Settings { BuildCache = false, ErrorLevel = 1, SchemaPath = Path.Combine(fixtures, "MultisoundFXPipeline.xsd"),
                DataRoot = directory, DataPaths = new[] { directory }, TargetPlatform = TargetPlatform.Win32,
                CustomPostfix = "", StreamPostfix = "", StringHashBinDescriptors = Array.Empty<StringHashBinDescriptor>() };
            Ra3Ep1AttributeModifierPlugin modifiers = new(true); modifiers.Initialize(TargetPlatform.Win32);
            Ra3Ep1FXListPlugin fx = new(); fx.Initialize(TargetPlatform.Win32);
            Ra3Ep1MultisoundPlugin sounds = new(); sounds.Initialize(TargetPlatform.Win32);
            PluginRegistry plugins = new(Array.Empty<PluginDescriptor>(), TargetPlatform.Win32);
            plugins.AddPlugin(0xC5E07887u, modifiers); plugins.AddPlugin(0x86682E78u, fx); plugins.AddPlugin(0xA3A7AF37u, sounds);
            SessionCache cache = new(); cache.InitializeCache(new List<string>());
            DocumentProcessor processor = new(Settings.Current, plugins, new VerifierPluginRegistry(Array.Empty<PluginDescriptor>(), TargetPlatform.Win32))
                { Cache = cache, SchemaSet = new SchemaSet(false) };
            string source = Path.Combine(directory, "parent.xml"), soundFile = Path.Combine(directory, "sound.xml");
            File.WriteAllText(source, Xml("<Includes><Include type=\"instance\" source=\"fx.xml\" /></Includes><AttributeModifier id=\"RebornSoundModifier\" StartFX=\"RebornSoundFX\" />"));
            File.WriteAllText(Path.Combine(directory, "fx.xml"), Xml("<Includes><Include type=\"instance\" source=\"sound.xml\" /></Includes>"
                + "<FXList id=\"RebornSoundFX\"><NuggetList><Sound Value=\"RebornLocalMultisound\" /></NuggetList></FXList>"));
            string originalSound = Xml("<Multisound id=\"RebornLocalMultisound\" Control=\"PLAY_ONE\"><Subsound>GDI_Commando_VoiceDie</Subsound>"
                + "<Subsound Weight=\"800\">GDI_Engineer_VoiceDie</Subsound></Multisound>"
                + "<Multisound id=\"UnusedSound\"><Subsound>AbsentUnusedAudio</Subsound></Multisound>");
            File.WriteAllText(soundFile, originalSound);
            InstanceHandle[] audio = { new("AudioEvent", "GDI_Commando_VoiceDie"), new("AudioEvent", "GDI_Engineer_VoiceDie"), new("AudioEvent", "GDI_FireHawk_VoiceDie") };
            string[] paths = audio.Select((handle, index) =>
            {
                string path = Path.Combine(directory, "audio-" + index + ".manifest");
                ExternalLinkSmokeTest.WriteFixture(path, handle, new ReferencedFileBuffer(), typeHash: 0x560C2E45u, tokenized: false); return path;
            }).ToArray();
            string[] runtimeNames = { "data\\audio-one.manifest", "data\\audio-two.manifest", "data\\audio-three.manifest" };
            Settings.Current.ProcessedExternalManifests = paths;
            AssetDeclarationDocument document = Read(processor, source); InstanceDeclaration parent = document.SelfInstances.Single();
            InstanceDeclaration[] ordered = Prepare(document, parent);
            Require(DependencyResolutionSmokeTest.Visited(document).Count == 3 && parent.AllDependentInstances!.Count == 2
                && ordered[0].AllDependentInstances!.Count == 0 && ordered[1].AllDependentInstances!.Count == 1,
                "Fixed chain lost local closure or compiled unused/external sound roots.");
            BoundedDiagnosticBuild.ValidateExternalDependencies(ordered, Metadata(paths));
            AssetBuffer[] chunks = Compile(plugins, ordered);
            Require(chunks.Select(chunk => chunk.InstanceData.Length).SequenceEqual(new[] { 72,80,56 })
                && chunks.Sum(chunk => chunk.RelocationData.Length) == 20 && chunks.Sum(chunk => chunk.ImportsData.Length) == 28, "Fixed sound chain native sizes differ.");
            var payloads = Serialize(ordered, chunks, paths, runtimeNames, out uint checksum);
            string first = Path.Combine(directory, "first"); ModifierFXStreamSmokeTest.WriteNew(first, payloads);
            BoundedDiagnosticBuild.Verify(first, ordered, chunks, payloads, checksum, runtimeNames); CheckSelectors(first, audio.Take(2).ToArray());
            AssetBuffer[] fresh = Compile(plugins, ordered); var repeat = Serialize(ordered, fresh, paths, runtimeNames, out uint repeatedChecksum);
            Require(checksum == repeatedChecksum && payloads.All(file => file.Value.SequenceEqual(repeat[file.Key])), "Repeated fixed sound chain differs.");
            string second = Path.Combine(directory, "repeat"); ModifierFXStreamSmokeTest.WriteNew(second, repeat);
            BoundedDiagnosticBuild.Verify(second, ordered, fresh, repeat, checksum, runtimeNames); CheckSelectors(second, audio.Take(2).ToArray());
            // Reborn: selected external hashes/flags and duplicate mapped identities are checked by the same gate as the public bounded command.
            byte[] approved = File.ReadAllBytes(paths[1]);
            try
            {
                foreach (bool tokenized in new[] { false,true })
                {
                    ExternalLinkSmokeTest.WriteFixture(paths[1], audio[1], new ReferencedFileBuffer(), typeHash: tokenized ? 0x560C2E45u : 0x560C2E44u, tokenized: tokenized);
                    RejectMetadata(() => BoundedDiagnosticBuild.ValidateExternalDependencies(ordered, Metadata(paths)), "External audio fingerprint");
                }
            }
            finally { File.WriteAllBytes(paths[1], approved); }
            string duplicate = Path.Combine(directory, "duplicate.manifest"); File.Copy(paths[1], duplicate);
            RejectMetadata(() => BoundedDiagnosticBuild.ValidateExternalDependencies(ordered, Metadata(paths.Append(duplicate).ToArray())), "resolve uniquely");
            RejectMetadata(() => BoundedDiagnosticBuild.ValidateExternalDependencies(ordered, Metadata(paths.Take(1).ToArray())), "resolve uniquely");
            BoundedDiagnosticBuild.ValidateExternalDependencies(ordered, Metadata(paths));
            foreach (string stream in new[] { "diagnostic.bin","diagnostic.relo","diagnostic.imp" })
            {
                foreach (int offset in new[] { 0,4 })
                {
                    byte[] bad = (byte[])payloads[stream].Clone(); bad[offset] ^= 1;
                    ModifierFXStreamSmokeTest.RejectCorruption(first, stream, bad, ordered, chunks, payloads, checksum, runtimeNames);
                }
                ModifierFXStreamSmokeTest.RejectCorruption(first, stream, payloads[stream][..^1], ordered, chunks, payloads, checksum, runtimeNames);
                ModifierFXStreamSmokeTest.RejectCorruption(first, stream, payloads[stream].Concat(new byte[] { 0 }).ToArray(), ordered, chunks, payloads, checksum, runtimeNames);
            }
            foreach (int selector in new[] { 24,52,156,172 })
            {
                byte[] bad = (byte[])payloads["diagnostic.bin"].Clone(); BinaryPrimitives.WriteUInt32LittleEndian(bad.AsSpan(selector, 4), 0);
                ModifierFXStreamSmokeTest.RejectCorruption(first, "diagnostic.bin", bad, ordered, chunks, payloads, checksum, runtimeNames);
            }
            foreach (string stream in new[] { "diagnostic.relo","diagnostic.imp" })
            {
                byte[] bad = (byte[])payloads[stream].Clone(); bad[8] ^= 4;
                ModifierFXStreamSmokeTest.RejectCorruption(first, stream, bad, ordered, chunks, payloads, checksum, runtimeNames);
            }
            int references = 4 + 48 + 3 * 48;
            byte[] swapped = (byte[])payloads["diagnostic.manifest"].Clone();
            Array.Copy(payloads["diagnostic.manifest"], references + 8, swapped, references, 8);
            Array.Copy(payloads["diagnostic.manifest"], references, swapped, references + 8, 8);
            ModifierFXStreamSmokeTest.RejectCorruption(first, "diagnostic.manifest", swapped, ordered, chunks, payloads, checksum, runtimeNames);
            byte[] baseType = (byte[])payloads["diagnostic.manifest"].Clone(); BinaryPrimitives.WriteUInt32LittleEndian(baseType.AsSpan(references + 16, 4), 0x4053C714u);
            ModifierFXStreamSmokeTest.RejectCorruption(first, "diagnostic.manifest", baseType, ordered, chunks, payloads, checksum, runtimeNames);
            try { ModifierFXStreamSmokeTest.WriteNew(first, repeat); throw new InvalidOperationException("Existing chain directory was replaced."); } catch (InvalidDataException) { }
            BoundedDiagnosticBuild.Verify(first, ordered, chunks, payloads, checksum, runtimeNames);
            // Reborn: losing a leaf target revokes the entire ancestor chain's prepared tables, then restored mappings recover all native entries.
            Settings.Current.ProcessedExternalManifests = paths.Take(1).ToArray(); Missing(document, parent); Missing(document, parent);
            Require(ordered.All(instance => instance.ValidatedReferencedInstances == null), "Failed chain retained partial validated tables.");
            // Reborn: FX/sound entries require preparation themselves; the older modifier entry checks original selectors, so the stream gate must reject all revoked chains.
            foreach (InstanceDeclaration instance in ordered.Take(2)) RejectEntry(() => plugins.GetPlugin(instance.Handle.TypeId).ProcessInstance(instance));
            RejectMetadata(() => BoundedDiagnosticBuild.ValidateExternalDependencies(ordered, Metadata(paths)), "Prepared diagnostic dependencies");
            Settings.Current.ProcessedExternalManifests = paths; DependencyResolutionSmokeTest.Prepare(document, parent);
            BoundedDiagnosticBuild.ValidateExternalDependencies(ordered, Metadata(paths));
            Require(Compile(plugins, ordered).Zip(chunks).All(pair => Equal(pair.First, pair.Second)), "Recovered chain bytes differ.");
            // Reborn: editing the included sound leaf must refresh concrete references through both ancestors, not reuse the earlier local closure.
            File.WriteAllText(soundFile, originalSound.Replace("GDI_Commando_VoiceDie", "GDI_FireHawk_VoiceDie", StringComparison.Ordinal));
            AssetDeclarationDocument edited = Read(processor, source); InstanceDeclaration[] changed = Prepare(edited, edited.SelfInstances.Single());
            Require(changed[0].ValidatedReferencedInstances![0] == audio[2] && changed[1].ValidatedReferencedInstances!.Single().TypeName == "Multisound",
                "Edited local sound leaf retained a stale concrete target/ancestor identity.");
            BoundedDiagnosticBuild.ValidateExternalDependencies(changed, Metadata(paths)); AssetBuffer[] changedChunks = Compile(plugins, changed);
            var editedPayloads = Serialize(changed, changedChunks, paths, runtimeNames, out uint editedChecksum);
            string editedDirectory = Path.Combine(directory, "edited"); ModifierFXStreamSmokeTest.WriteNew(editedDirectory, editedPayloads);
            BoundedDiagnosticBuild.Verify(editedDirectory, changed, changedChunks, editedPayloads, editedChecksum, runtimeNames);
            CheckSelectors(editedDirectory, new[] { audio[2],audio[1] });
            File.WriteAllText(soundFile, originalSound);
            AssetDeclarationDocument restored = Read(processor, source); InstanceDeclaration[] recovered = Prepare(restored, restored.SelfInstances.Single());
            Require(Compile(plugins, recovered).Zip(chunks).All(pair => Equal(pair.First, pair.Second)), "Restored source chain did not recover native bytes.");
            if (stockManifests.Length > 0)
            {
                Settings.Current.ProcessedExternalManifests = stockManifests.Select(Path.GetFullPath).ToArray();
                AssetDeclarationDocument real = Read(processor, source); InstanceDeclaration[] stock = Prepare(real, real.SelfInstances.Single());
                BoundedDiagnosticBuild.ValidateExternalDependencies(stock, Metadata(Settings.Current.ProcessedExternalManifests));
                AssetBuffer[] stockChunks = Compile(plugins, stock);
                Require(stockChunks.Zip(chunks).All(pair => Equal(pair.First, pair.Second)), "Real mapped audio changed fixed chain native bytes.");
                string[] stockNames = stockManifests.Select((_, index) => "data\\stock-" + index + ".manifest").ToArray();
                var stockPayloads = Serialize(stock, stockChunks, Settings.Current.ProcessedExternalManifests, stockNames, out uint stockChecksum);
                string stockDirectory = Path.Combine(directory, "stock"); ModifierFXStreamSmokeTest.WriteNew(stockDirectory, stockPayloads);
                BoundedDiagnosticBuild.Verify(stockDirectory, stock, stockChunks, stockPayloads, stockChecksum, stockNames); CheckSelectors(stockDirectory, audio.Take(2).ToArray());
                Console.WriteLine("  Real EP1 mapped AudioEvent targets: exact identities/fingerprints, local chain native 208/20/28 and linked readback OK.");
            }
            ExpectPolicy(plugins.ValidateProductionOutput); processor.Cache = null!;
            ExpectPolicy(() => processor.ProcessDocumentInternal("missing", "missing", null!, new DocumentProcessor.ProcessOptions { GenerateOutput = true }));
            Require(!plugins.CanReuseCompiledDocuments, "Fixed sound chain enabled compiled document reuse.");
            Console.WriteLine($"Multisound/FX stream self-test: OK (three-level Include closure, concrete local/external selectors, unique stock audio fingerprints, 208/20/28 native, independent readback, 20 corruptions, missing/edit/recovery, existing-output preservation, production gates); diagnostic={Path.Combine(first, "diagnostic.manifest")}");
        }
        finally { Settings.Current = saved; }
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: derive fixed dependency-first order from the real recursive local closure, excluding tentative and external roots. */
    //-------------------------------------------------------------------------------------------------
    private static InstanceDeclaration[] Prepare(AssetDeclarationDocument document, InstanceDeclaration parent)
    {
        DependencyResolutionSmokeTest.Prepare(document, parent);
        InstanceDeclaration[] ordered = DependencyResolutionSmokeTest.Visited(document).Values.OrderBy(instance => instance.Handle.TypeId switch
            { 0xA3A7AF37u => 0,0x86682E78u => 1,0xC5E07887u => 2,_ => throw new InvalidDataException("Unexpected fixed chain root.") }).ToArray();
        Require(ordered.Length == 3 && ordered[0].Handle.TypeName == "Multisound" && ordered[1].Handle.TypeName == "FXList", "Fixed sound chain closure differs."); return ordered;
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: compile only explicitly isolated profile entries after current dependency preparation. */
    //-------------------------------------------------------------------------------------------------
    internal static AssetBuffer[] Compile(PluginRegistry plugins, InstanceDeclaration[] ordered) => ordered.Select(instance => plugins.GetPlugin(instance.Handle.TypeId).ProcessInstance(instance)).ToArray();

    //-------------------------------------------------------------------------------------------------
    /** Reborn: share existing native serialization/header/mapping seams without invoking production OutputManager commits. */
    //-------------------------------------------------------------------------------------------------
    internal static Dictionary<string, byte[]> Serialize(InstanceDeclaration[] ordered, AssetBuffer[] chunks, string[] paths, string[] names, out uint checksum)
    {
        checksum = (uint)typeof(AssetDeclarationDocument).GetMethod("ComputeOutputChecksum", BindingFlags.NonPublic | BindingFlags.Static)!.Invoke(null, new object[] { ordered })!;
        ReferencedFileBuffer runtime = new(); typeof(OutputManager).GetMethod("AddExternalManifestReferences", BindingFlags.NonPublic | BindingFlags.Static)!
            .Invoke(null, new object[] { runtime,paths,string.Join(';', names) });
        return BoundedDiagnosticBuild.Serialize(ordered, chunks, checksum, runtime);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: verify each serialized one-biased import against explicit leaf or local identities, independent of XML-normalized selectors. */
    //-------------------------------------------------------------------------------------------------
    private static void CheckSelectors(string directory, InstanceHandle[] audio)
    {
        ManifestDocument manifest = ManifestReader.Read(File.ReadAllBytes(Path.Combine(directory, "diagnostic.manifest")));
        byte[] bin = File.ReadAllBytes(Path.Combine(directory, "diagnostic.bin"));
        Require(manifest.Assets.Count == 3 && manifest.Header.TotalInstanceDataSize == 208 && manifest.Header.AssetReferenceBufferSize == 32
            && manifest.Assets.Select(asset => asset.SourceFile).SequenceEqual(new[] { "sound.xml","fx.xml","parent.xml" }), "Fixed sound chain totals/sources differ.");
        // Reborn: independently assert the known weighted 16/28 sound shape and absent pointer fields rather than trusting compiler chunk lengths alone.
        Require(BinaryPrimitives.ReadUInt32LittleEndian(bin.AsSpan(12, 4)) == 2
            && BinaryPrimitives.ReadUInt32LittleEndian(bin.AsSpan(16, 4)) == 2
            && BinaryPrimitives.ReadUInt32LittleEndian(bin.AsSpan(20, 4)) == 16
            && BinaryPrimitives.ReadUInt32LittleEndian(bin.AsSpan(28, 4)) == 1000
            && BinaryPrimitives.ReadUInt32LittleEndian(bin.AsSpan(56, 4)) == 800
            && bin.AsSpan(32, 20).IndexOfAnyExcept((byte)0) == -1 && bin.AsSpan(60, 20).IndexOfAnyExcept((byte)0) == -1,
            "Fixed local Multisound weight/list/control/optional-pointer fields differ.");
        AssetId[][] expected = { audio.Select(handle => new AssetId(handle.TypeId, handle.InstanceId)).ToArray(),
            new[] { new AssetId(0xA3A7AF37u, InstanceHandle.GetInstanceId("RebornLocalMultisound")) },
            new[] { new AssetId(0x86682E78u, InstanceHandle.GetInstanceId("RebornSoundFX")) } };
        int[][] slots = { new[] { 24,52 },new[] { 156 },new[] { 172 } };
        for (int asset = 0; asset < expected.Length; asset++)
        {
            Require(manifest.Assets[asset].References.SequenceEqual(expected[asset]), "Fixed chain concrete reference table differs.");
            for (int index = 0; index < slots[asset].Length; index++)
            {
                uint selector = BinaryPrimitives.ReadUInt32LittleEndian(bin.AsSpan(slots[asset][index], 4));
                Require(selector == index + 1 && manifest.Assets[asset].References[(int)selector - 1] == expected[asset][index], "Fixed chain import chose another identity.");
            }
        }
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: validate mapped metadata without loading external audio payloads or accepting other games/patch bases. */
    //-------------------------------------------------------------------------------------------------
    internal static ManifestDocument[] Metadata(string[] paths) => paths.Select(path =>
    {
        ManifestDocument document = ManifestReader.Read(File.ReadAllBytes(path)); TypeRegistryAudit.ValidateTarget(document.Header.Version, document.Header.AllTypesHash);
        Require(document.Header.IsLinked && document.Validate().Count == 0 && !document.ReferencedManifests.Any(reference => reference.IsPatch), "Invalid external fixed-chain metadata."); return document;
    }).ToArray();

    //-------------------------------------------------------------------------------------------------
    /** Reborn: force current documents through core Include/schema stages without enabling production output. */
    //-------------------------------------------------------------------------------------------------
    private static AssetDeclarationDocument Read(DocumentProcessor processor, string source) => processor.ProcessDocumentInternal(source, source, null!, new DocumentProcessor.ProcessOptions { GenerateOutput = false,UsePrecompiled = true });

    //-------------------------------------------------------------------------------------------------
    /** Reborn: keep owned test source in the official asset namespace. */
    //-------------------------------------------------------------------------------------------------
    private static string Xml(string body) => "<AssetDeclaration xmlns=\"uri:ea.com:eala:asset\">" + body + "</AssetDeclaration>";

    //-------------------------------------------------------------------------------------------------
    /** Reborn: require strict recursive target-loss failure rather than reuse of earlier valid tables. */
    //-------------------------------------------------------------------------------------------------
    private static void Missing(AssetDeclarationDocument document, InstanceDeclaration instance)
    {
        try { DependencyResolutionSmokeTest.Prepare(document, instance); } catch (BinaryAssetBuilderException error) when (error.ErrorCode == ErrorCode.UnknownReference) { return; }
        throw new InvalidDataException("Missing leaf sound target was accepted.");
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: distinguish deliberate external admission failure from unrelated parsing or native errors. */
    //-------------------------------------------------------------------------------------------------
    private static void RejectMetadata(Action action, string expected)
    {
        try { action(); } catch (InvalidDataException error) when (error.Message.Contains(expected, StringComparison.Ordinal)) { return; }
        throw new InvalidOperationException("Wrong fixed-chain external metadata was accepted.");
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: unprepared ancestor and sound entries must reject before native writing. */
    //-------------------------------------------------------------------------------------------------
    private static void RejectEntry(Action action)
    {
        try { action(); } catch (Exception error) when (error is NotSupportedException or InvalidOperationException) { return; }
        throw new InvalidDataException("Failed chain retained compiler readiness.");
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: compare all native bytes after deterministic recompile or recursive recovery. */
    //-------------------------------------------------------------------------------------------------
    private static bool Equal(AssetBuffer first, AssetBuffer second) => first.InstanceData.SequenceEqual(second.InstanceData) && first.RelocationData.SequenceEqual(second.RelocationData) && first.ImportsData.SequenceEqual(second.ImportsData);

    //-------------------------------------------------------------------------------------------------
    /** Reborn: fixed serializers cannot bypass experimental production output policies. */
    //-------------------------------------------------------------------------------------------------
    private static void ExpectPolicy(Action action)
    {
        try { action(); } catch (BinaryAssetBuilderException error) when (error.Message.Contains("Experimental", StringComparison.Ordinal)) { return; }
        throw new InvalidDataException("Fixed sound chain enabled production output.");
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: fail on the first native, metadata, closure or policy contract violation. */
    //-------------------------------------------------------------------------------------------------
    private static void Require(bool condition, string message) { if (!condition) throw new InvalidDataException(message); }
}
