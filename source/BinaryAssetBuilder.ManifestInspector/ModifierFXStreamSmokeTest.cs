using System.Buffers.Binary;
using System.Reflection;
using System.Text;
using System.Xml;
using BinaryAssetBuilder.Core;
using BinaryAssetBuilder.Core.SageXml;
using BinaryAssetBuilder.Core.Session;
using BinaryAssetBuilder.Utility;
using BinaryAssetBuilder.XmlCompiler;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: prove a fixed modifier/local-FX/external-audio stream graph without widening command input or production registration.
internal static class ModifierFXStreamSmokeTest
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: compile included FX dependencies, serialize via the shared diagnostic writer and independently verify concrete identities/selectors and corruption rejection. */
    //-------------------------------------------------------------------------------------------------
    internal static void Run()
    {
        string fixtures = Path.GetDirectoryName(ReferencePipelineSmokeTest.FindFixture())!;
        string directory = Path.Combine(Path.GetTempPath(), "Reborn-ModifierFXStream-" + Guid.NewGuid().ToString("N")); Directory.CreateDirectory(directory);
        Settings saved = Settings.Current;
        try
        {
            FXListNativeSmokeTest.CheckExtractedEnums(fixtures, "ModifierFXPipeline.xsd");
            Settings.Current = new Settings { BuildCache = false, ErrorLevel = 1, SchemaPath = Path.Combine(fixtures, "ModifierFXPipeline.xsd"),
                DataRoot = directory, DataPaths = new[] { directory }, TargetPlatform = TargetPlatform.Win32,
                CustomPostfix = "", StreamPostfix = "", StringHashBinDescriptors = Array.Empty<StringHashBinDescriptor>() };
            Ra3Ep1AttributeModifierPlugin modifiers = new(true); modifiers.Initialize(TargetPlatform.Win32);
            Ra3Ep1FXListPlugin fx = new(); fx.Initialize(TargetPlatform.Win32);
            PluginRegistry plugins = new(Array.Empty<PluginDescriptor>(), TargetPlatform.Win32);
            plugins.AddPlugin(0xC5E07887u, modifiers); plugins.AddPlugin(0x86682E78u, fx);
            SessionCache cache = new(); cache.InitializeCache(new List<string>());
            DocumentProcessor processor = new(Settings.Current, plugins, new VerifierPluginRegistry(Array.Empty<PluginDescriptor>(), TargetPlatform.Win32))
                { Cache = cache, SchemaSet = new SchemaSet(false) };
            File.Copy(Path.Combine(fixtures, "FXListProbe.xml"), Path.Combine(directory, "fx.xml"));
            string source = Path.Combine(directory, "parent.xml");
            File.WriteAllText(source, """
                <AssetDeclaration xmlns="uri:ea.com:eala:asset"><Includes><Include type="instance" source="fx.xml" /></Includes>
                <AttributeModifier id="RebornModifierFXStream" StartFX="FX_DebrisHitGround" EndFX="FX_ALL_AntiGroundAircraft_VoiceDie" />
                </AssetDeclaration>
                """);
            InstanceHandle[] audio = { new("AudioEvent", "ImpactDebrisHitsGround"), new("AudioEvent", "TEMP_RA2_AlliedAir_VoiceCrash"),
                new("Multisound", "GDI_Generic_VoiceDieMS") };
            string[] paths = audio.Select((handle, index) =>
            {
                string path = Path.Combine(directory, "audio-" + index + ".manifest");
                ExternalLinkSmokeTest.WriteFixture(path, handle, new ReferencedFileBuffer()); return path;
            }).ToArray();
            string[] runtimeNames = { "data\\audio-one.manifest", "data\\audio-two.manifest", "data\\audio-three.manifest" };
            Settings.Current.ProcessedExternalManifests = paths;
            AssetDeclarationDocument document = processor.ProcessDocumentInternal(source, source, null!,
                new DocumentProcessor.ProcessOptions { GenerateOutput = false, UsePrecompiled = true });
            InstanceDeclaration modifier = document.SelfInstances.Single(); DependencyResolutionSmokeTest.Prepare(document, modifier);
            InstanceDeclaration one = document.FindInstance(new InstanceHandle("FXList", "FX_DebrisHitGround"), FindLocation.None, out _);
            InstanceDeclaration two = document.FindInstance(new InstanceHandle("FXList", "FX_ALL_AntiGroundAircraft_VoiceDie"), FindLocation.None, out _);
            Require(modifier.AllDependentInstances!.Count == 2 && DependencyResolutionSmokeTest.Visited(document).Count == 3
                && one.AllDependentInstances!.Count == 0 && two.AllDependentInstances!.Count == 0,
                "Fixed stream graph queued audio or unused instance-Include roots, or lost local FX closure.");
            Require(one.ValidatedReferencedInstances!.Single() == audio[0] && two.ValidatedReferencedInstances!.SequenceEqual(audio.Skip(1)),
                "Core did not select the expected ordered concrete audio identities.");
            InstanceDeclaration[] ordered = { one, two, modifier };
            AssetBuffer[] chunks = ordered.Select(instance => plugins.GetPlugin(instance.Handle.TypeId).ProcessInstance(instance)).ToArray();
            Require(chunks.Select(chunk => chunk.InstanceData.Length).SequenceEqual(new[] { 80, 252, 56 })
                && chunks.Sum(chunk => chunk.RelocationData.Length) == 36 && chunks.Sum(chunk => chunk.ImportsData.Length) == 32,
                "Fixed mixed FX/modifier native chunk sizes differ.");
            uint checksum = (uint)typeof(AssetDeclarationDocument).GetMethod("ComputeOutputChecksum", BindingFlags.NonPublic | BindingFlags.Static)!
                .Invoke(null, new object[] { ordered })!;
            // Reborn: use the real physical/runtime mapping seam rather than manually assuming serialized reference names.
            ReferencedFileBuffer runtime = new();
            typeof(OutputManager).GetMethod("AddExternalManifestReferences", BindingFlags.NonPublic | BindingFlags.Static)!
                .Invoke(null, new object[] { runtime, paths, string.Join(';', runtimeNames) });
            Dictionary<string, byte[]> payloads = BoundedDiagnosticBuild.Serialize(ordered, chunks, checksum, runtime);
            string first = Path.Combine(directory, "first"); WriteNew(first, payloads);
            BoundedDiagnosticBuild.Verify(first, ordered, chunks, payloads, checksum, runtimeNames); CheckSelectors(first, audio);
            string repeat = Path.Combine(directory, "repeat");
            AssetBuffer[] freshChunks = ordered.Select(instance => plugins.GetPlugin(instance.Handle.TypeId).ProcessInstance(instance)).ToArray();
            Dictionary<string, byte[]> repeated = BoundedDiagnosticBuild.Serialize(ordered, freshChunks, checksum, runtime);
            WriteNew(repeat, repeated); BoundedDiagnosticBuild.Verify(repeat, ordered, freshChunks, repeated, checksum, runtimeNames); CheckSelectors(repeat, audio);
            foreach (var payload in payloads) Require(payload.Value.SequenceEqual(repeated[payload.Key]), "Repeated graph compilation/serialization is nondeterministic.");
            // Reborn: bypass byte-snapshot equality deliberately so independent header/native/identity checks must detect each corruption.
            foreach (var stream in new[] { "diagnostic.bin", "diagnostic.relo", "diagnostic.imp" })
            {
                byte[] header = (byte[])payloads[stream].Clone(); header[0] ^= 1;
                RejectCorruption(first, stream, header, ordered, chunks, payloads, checksum, runtimeNames);
                byte[] badChecksum = (byte[])payloads[stream].Clone(); badChecksum[4] ^= 1;
                RejectCorruption(first, stream, badChecksum, ordered, chunks, payloads, checksum, runtimeNames);
                RejectCorruption(first, stream, payloads[stream][..^1], ordered, chunks, payloads, checksum, runtimeNames);
                RejectCorruption(first, stream, payloads[stream].Concat(new byte[] { 0 }).ToArray(), ordered, chunks, payloads, checksum, runtimeNames);
            }
            foreach (int selector in new[] { 8 + 76, 8 + 80 + 80, 8 + 80 + 188, 8 + 80 + 252 + 12, 8 + 80 + 252 + 16 })
            {
                byte[] bad = (byte[])payloads["diagnostic.bin"].Clone(); BinaryPrimitives.WriteUInt32LittleEndian(bad.AsSpan(selector, 4), 0);
                RejectCorruption(first, "diagnostic.bin", bad, ordered, chunks, payloads, checksum, runtimeNames);
            }
            byte[] wrongRelo = (byte[])payloads["diagnostic.relo"].Clone(); wrongRelo[8] ^= 4;
            RejectCorruption(first, "diagnostic.relo", wrongRelo, ordered, chunks, payloads, checksum, runtimeNames);
            byte[] wrongImp = (byte[])payloads["diagnostic.imp"].Clone(); wrongImp[8] ^= 4;
            RejectCorruption(first, "diagnostic.imp", wrongImp, ordered, chunks, payloads, checksum, runtimeNames);
            int references = 4 + 48 + ordered.Length * 48;
            byte[] swapped = (byte[])payloads["diagnostic.manifest"].Clone();
            Array.Copy(payloads["diagnostic.manifest"], references + 16, swapped, references + 8, 8);
            Array.Copy(payloads["diagnostic.manifest"], references + 8, swapped, references + 16, 8);
            RejectCorruption(first, "diagnostic.manifest", swapped, ordered, chunks, payloads, checksum, runtimeNames);
            byte[] baseType = (byte[])payloads["diagnostic.manifest"].Clone();
            BinaryPrimitives.WriteUInt32LittleEndian(baseType.AsSpan(references, 4), 0x4053C714u);
            RejectCorruption(first, "diagnostic.manifest", baseType, ordered, chunks, payloads, checksum, runtimeNames);
            byte[] badMaximum = (byte[])payloads["diagnostic.manifest"].Clone();
            BinaryPrimitives.WriteUInt32LittleEndian(badMaximum.AsSpan(24, 4), 1);
            RejectCorruption(first, "diagnostic.manifest", badMaximum, ordered, chunks, payloads, checksum, runtimeNames);
            byte[] badChunk = (byte[])payloads["diagnostic.manifest"].Clone();
            BinaryPrimitives.WriteUInt32LittleEndian(badChunk.AsSpan(4 + 48 + 32, 4), 79);
            RejectCorruption(first, "diagnostic.manifest", badChunk, ordered, chunks, payloads, checksum, runtimeNames);
            byte[] badRuntime = (byte[])payloads["diagnostic.manifest"].Clone();
            int runtimeOffset = references + 40;
            int nameOffset = badRuntime.AsSpan(runtimeOffset).IndexOf(Encoding.UTF8.GetBytes(runtimeNames[0]));
            Require(nameOffset >= 0, "Runtime name was not serialized into its expected manifest buffer.");
            badRuntime[runtimeOffset + nameOffset] = (byte)'x';
            RejectCorruption(first, "diagnostic.manifest", badRuntime, ordered, chunks, payloads, checksum, runtimeNames);
            // Reborn: no serialization attempt may replace a pre-existing diagnostic directory.
            try { WriteNew(first, repeated); throw new InvalidOperationException("Existing stream directory was overwritten."); }
            catch (InvalidDataException) { }
            BoundedDiagnosticBuild.Verify(first, ordered, chunks, payloads, checksum, runtimeNames);
            Settings.Current.ProcessedExternalManifests = paths.Take(2).ToArray();
            ExpectMissing(document, modifier); ExpectMissing(document, modifier);
            Require(DependencyResolutionSmokeTest.Visited(document).Count == 1 && modifier.ValidatedReferencedInstances == null && two.ValidatedReferencedInstances == null,
                "Failed recursive preparation retained partial ancestor/FX validation.");
            Settings.Current.ProcessedExternalManifests = paths; DependencyResolutionSmokeTest.Prepare(document, modifier);
            foreach (var pair in ordered.Select((instance, index) => (instance, index)))
                Equal(chunks[pair.index], plugins.GetPlugin(pair.instance.Handle.TypeId).ProcessInstance(pair.instance));
            BoundedDiagnosticBuild.Verify(first, ordered, chunks, payloads, checksum, runtimeNames);
            ExpectPolicy(plugins.ValidateProductionOutput); processor.Cache = null!;
            ExpectPolicy(() => processor.ProcessDocumentInternal("missing", "missing", null!, new DocumentProcessor.ProcessOptions { GenerateOutput = true }));
            Require(!plugins.CanReuseCompiledDocuments && !modifiers.CanWriteProductionOutput && !fx.CanWriteProductionOutput,
                "Stream proof relaxed experimental output/cache policy.");
            Console.WriteLine($"Modifier/FX stream self-test: OK (included local FX closure, external concrete audio, 388/36/32 native bytes, independent manifest/utility offsets/selectors, deterministic streams, corruption/target loss/recovery, existing-output preservation, production gates); diagnostic={Path.Combine(first, "diagnostic.manifest")}");
        }
        finally { Settings.Current = saved; }
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: write only this proof's five owned files into an absent directory, never overwrite an existing output location. */
    //-------------------------------------------------------------------------------------------------
    internal static void WriteNew(string directory, Dictionary<string, byte[]> payloads)
    {
        if (Directory.Exists(directory) || File.Exists(directory)) throw new InvalidDataException("Fixed stream proof requires a new output directory.");
        Directory.CreateDirectory(directory); foreach (var payload in payloads) File.WriteAllBytes(Path.Combine(directory, payload.Key), payload.Value);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: decode serialized one-biased imports into the known concrete audio and local FX targets, independent of source XML selectors. */
    //-------------------------------------------------------------------------------------------------
    private static void CheckSelectors(string directory, InstanceHandle[] audio)
    {
        ManifestDocument manifest = ManifestReader.Read(File.ReadAllBytes(Path.Combine(directory, "diagnostic.manifest")));
        byte[] bin = File.ReadAllBytes(Path.Combine(directory, "diagnostic.bin"));
        Require(manifest.Header.AssetReferenceBufferSize == 40 && manifest.Header.TotalInstanceDataSize == 388 && manifest.Assets.Count == 3,
            "Mixed manifest totals differ.");
        int[] roots = { 8, 88, 340 };
        int[][] slots = { new[] { 76 }, new[] { 80, 188 }, new[] { 12, 16 } };
        AssetId[][] expected = { new[] { new AssetId(audio[0].TypeId, audio[0].InstanceId) },
            audio.Skip(1).Select(handle => new AssetId(handle.TypeId, handle.InstanceId)).ToArray(),
            manifest.Assets.Take(2).Select(asset => new AssetId(asset.TypeId, asset.InstanceId)).ToArray() };
        for (int index = 0; index < roots.Length; index++)
            for (int slot = 0; slot < slots[index].Length; slot++)
            {
                uint selector = BinaryPrimitives.ReadUInt32LittleEndian(bin.AsSpan(roots[index] + slots[index][slot], 4));
                Require(selector > 0 && selector <= manifest.Assets[index].References.Count
                    && manifest.Assets[index].References[(int)selector - 1] == expected[index][slot], "Serialized native selector chose the wrong concrete dependency.");
            }
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: corrupt owned bytes and matching snapshot expectations, require independent readback failure, then restore both. */
    //-------------------------------------------------------------------------------------------------
    internal static void RejectCorruption(string directory, string name, byte[] bad, InstanceDeclaration[] ordered, AssetBuffer[] chunks,
        Dictionary<string, byte[]> payloads, uint checksum, string[] runtimeNames)
    {
        string path = Path.Combine(directory, name); byte[] saved = payloads[name]; payloads[name] = bad; File.WriteAllBytes(path, bad);
        try
        {
            try { BoundedDiagnosticBuild.Verify(directory, ordered, chunks, payloads, checksum, runtimeNames); }
            catch (InvalidDataException) { return; }
            throw new InvalidOperationException("Independent readback accepted corrupted stream metadata or native bytes.");
        }
        finally { payloads[name] = saved; File.WriteAllBytes(path, saved); }
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: recursive external target loss must remain a strict failure rather than stale prior validation success. */
    //-------------------------------------------------------------------------------------------------
    private static void ExpectMissing(AssetDeclarationDocument document, InstanceDeclaration instance)
    {
        try { DependencyResolutionSmokeTest.Prepare(document, instance); }
        catch (BinaryAssetBuilderException error) when (error.ErrorCode == ErrorCode.UnknownReference) { return; }
        throw new InvalidDataException("Missing concrete audio target did not fail recursive preparation.");
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: repeated compiler output after dependency recovery must retain exact native payloads and tables. */
    //-------------------------------------------------------------------------------------------------
    private static void Equal(AssetBuffer expected, AssetBuffer actual)
    {
        Require(expected.InstanceData.SequenceEqual(actual.InstanceData) && expected.RelocationData.SequenceEqual(actual.RelocationData)
            && expected.ImportsData.SequenceEqual(actual.ImportsData), "Recovered graph native output differs.");
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: diagnostic serializers cannot bypass experimental production-output policy guards. */
    //-------------------------------------------------------------------------------------------------
    private static void ExpectPolicy(Action action)
    {
        try { action(); } catch (BinaryAssetBuilderException error) when (error.Message.Contains("Experimental", StringComparison.Ordinal)) { return; }
        throw new InvalidDataException("Fixed FX stream proof enabled production output.");
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: fail the fixed mixed stream proof on its first identity, native offset, policy or restoration mismatch. */
    //-------------------------------------------------------------------------------------------------
    private static void Require(bool condition, string message)
    {
        if (!condition) throw new InvalidDataException(message);
    }
}
