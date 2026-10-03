using System.Buffers.Binary;
using System.Xml;
using BinaryAssetBuilder.Core;
using BinaryAssetBuilder.Core.SageXml;
using BinaryAssetBuilder.Core.Session;
using BinaryAssetBuilder.Utility;
using BinaryAssetBuilder.XmlCompiler;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: fixed local AudioEvent/Multisound/FX stream proves narrow closure only, never general command or production audio admission.
internal static class AudioEventFXStreamSmokeTest
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: verify prepared leaf fingerprints, local native selectors, stream readback, corruptions and recursive recovery. */
    //-------------------------------------------------------------------------------------------------
    internal static void Run(params string[] stocks)
    {
        string fixtures = Path.GetDirectoryName(ReferencePipelineSmokeTest.FindFixture())!;
        string directory = Path.Combine(Path.GetTempPath(),"Reborn-AudioEventFXStream-"+Guid.NewGuid().ToString("N")); Directory.CreateDirectory(directory);
        Settings saved = Settings.Current;
        try
        {
            FXListNativeSmokeTest.CheckExtractedEnums(fixtures,"AudioEventFXPipeline.xsd");
            Settings.Current = new Settings { BuildCache = false, ErrorLevel = 1, SchemaPath = Path.Combine(fixtures,"AudioEventFXPipeline.xsd"),
                DataRoot = directory, DataPaths = new[] { directory }, TargetPlatform = TargetPlatform.Win32,
                CustomPostfix = "", StreamPostfix = "", StringHashBinDescriptors = Array.Empty<StringHashBinDescriptor>() };
            Ra3Ep1AudioEventPlugin events = new(); events.Initialize(TargetPlatform.Win32);
            Ra3Ep1MultisoundPlugin sounds = new(); sounds.Initialize(TargetPlatform.Win32);
            Ra3Ep1FXListPlugin fx = new(); fx.Initialize(TargetPlatform.Win32);
            PluginRegistry plugins = new(Array.Empty<PluginDescriptor>(),TargetPlatform.Win32);
            plugins.AddPlugin(0x844D7B9Fu,events); plugins.AddPlugin(0xA3A7AF37u,sounds); plugins.AddPlugin(0x86682E78u,fx);
            SessionCache cache = new(); cache.InitializeCache(new List<string>());
            DocumentProcessor processor = new(Settings.Current,plugins,new VerifierPluginRegistry(Array.Empty<PluginDescriptor>(),TargetPlatform.Win32))
                { Cache = cache, SchemaSet = new SchemaSet(false) };
            string source = Path.Combine(directory,"fx.xml"), leaf = Path.Combine(directory,"event.xml");
            File.WriteAllText(source,Xml("<Includes><Include type=\"instance\" source=\"sound.xml\" /></Includes>"
                +"<FXList id=\"RebornLocalFX\"><NuggetList><Sound Value=\"RebornLocalMultisound\" /></NuggetList></FXList>"));
            File.WriteAllText(Path.Combine(directory,"sound.xml"),Xml("<Includes><Include type=\"instance\" source=\"event.xml\" /></Includes>"
                +"<Multisound id=\"RebornLocalMultisound\" Control=\"PLAY_ONE\"><Subsound>AudioEvent:RebornLocalAudio</Subsound></Multisound>"));
            string original = Xml("<AudioEvent id=\"RebornLocalAudio\" Volume=\"60\" Control=\"INTERRUPT\" SubmixSlider=\"SOUNDFX\">"
                +"<PitchShift Low=\"-5\" High=\"5\" /><Sound>WImpact_DebrisVsGrounda</Sound><Sound Weight=\"800\">WImpact_DebrisVsGroundb</Sound></AudioEvent>"
                +"<AudioEvent id=\"UnusedAudio\"><Sound>AbsentUnusedFile</Sound></AudioEvent>");
            File.WriteAllText(leaf,original);
            InstanceHandle[] audio = new[] { "WImpact_DebrisVsGrounda","WImpact_DebrisVsGroundb","WImpact_DebrisVsGroundc" }
                .Select(name => new InstanceHandle("AudioFile",name)).ToArray();
            string[] paths = audio.Select((handle,index) =>
            {
                string path = Path.Combine(directory,"audio-"+index+".manifest");
                ExternalLinkSmokeTest.WriteFixture(path,handle,new ReferencedFileBuffer(),typeHash:0x53C81E47u,tokenized:false); return path;
            }).ToArray();
            string[] names = { "data\\audio-one.manifest","data\\audio-two.manifest","data\\audio-three.manifest" };
            Settings.Current.ProcessedExternalManifests = paths;
            AssetDeclarationDocument doc = Read(processor,source); InstanceDeclaration parent = doc.SelfInstances.Single();
            InstanceDeclaration[] ordered = Prepare(doc,parent);
            Require(parent.AllDependentInstances!.Count == 2 && ordered[0].AllDependentInstances!.Count == 0 && ordered[1].AllDependentInstances!.Count == 1,
                "AudioEvent chain lost local closure or compiled tentative/external roots.");
            BoundedDiagnosticBuild.ValidateExternalDependencies(ordered,MultisoundFXStreamSmokeTest.Metadata(paths));
            AssetBuffer[] chunks = MultisoundFXStreamSmokeTest.Compile(plugins,ordered);
            Require(chunks.Select(chunk => chunk.InstanceData.Length).SequenceEqual(new[] { 188,44,80 })
                && chunks.Sum(chunk => chunk.RelocationData.Length) == 36 && chunks.Sum(chunk => chunk.ImportsData.Length) == 28,"AudioEvent chain sizes differ.");
            var payloads = MultisoundFXStreamSmokeTest.Serialize(ordered,chunks,paths,names,out uint checksum);
            string first = Path.Combine(directory,"first"); ModifierFXStreamSmokeTest.WriteNew(first,payloads);
            BoundedDiagnosticBuild.Verify(first,ordered,chunks,payloads,checksum,names); CheckSelectors(first,audio.Take(2).ToArray());
            Require(System.Text.Encoding.UTF8.GetString(payloads["DIAGNOSTIC_ONLY.txt"]).Contains("Local AudioEvent is experimental",StringComparison.Ordinal)
                && !System.Text.Encoding.UTF8.GetString(payloads["DIAGNOSTIC_ONLY.txt"]).Contains("AudioEvent/AudioFile payloads are not rebuilt",StringComparison.Ordinal),
                "AudioEvent stream notice incorrectly describes local event payloads.");
            var repeated = MultisoundFXStreamSmokeTest.Serialize(ordered,MultisoundFXStreamSmokeTest.Compile(plugins,ordered),paths,names,out uint repeatedChecksum);
            Require(checksum == repeatedChecksum && payloads.All(file => file.Value.SequenceEqual(repeated[file.Key])),"Repeated AudioEvent stream differs.");
            string repeat = Path.Combine(directory,"repeat"); ModifierFXStreamSmokeTest.WriteNew(repeat,repeated);
            BoundedDiagnosticBuild.Verify(repeat,ordered,chunks,repeated,checksum,names); CheckSelectors(repeat,audio.Take(2).ToArray());
            // Reborn: stream admission independently checks unique external AudioFile records, not merely prepared core identity handles.
            byte[] approved = File.ReadAllBytes(paths[1]);
            try
            {
                foreach (bool tokenized in new[] { false,true })
                {
                    ExternalLinkSmokeTest.WriteFixture(paths[1],audio[1],new ReferencedFileBuffer(),typeHash:tokenized ? 0x53C81E47u : 0x53C81E46u,tokenized:tokenized);
                    Reject(() => BoundedDiagnosticBuild.ValidateExternalDependencies(ordered,MultisoundFXStreamSmokeTest.Metadata(paths)),"External audio fingerprint");
                }
            }
            finally { File.WriteAllBytes(paths[1],approved); }
            string duplicate = Path.Combine(directory,"duplicate.manifest"); File.Copy(paths[1],duplicate);
            Reject(() => BoundedDiagnosticBuild.ValidateExternalDependencies(ordered,MultisoundFXStreamSmokeTest.Metadata(paths.Append(duplicate).ToArray())),"resolve uniquely");
            Reject(() => BoundedDiagnosticBuild.ValidateExternalDependencies(ordered,MultisoundFXStreamSmokeTest.Metadata(paths.Take(1).ToArray())),"resolve uniquely");
            // Reborn: duplicate selected identities inside one metadata document must fail just like duplicates across mapped files.
            ManifestDocument[] internalDuplicate = MultisoundFXStreamSmokeTest.Metadata(paths);
            internalDuplicate[1] = internalDuplicate[1] with { Assets = internalDuplicate[1].Assets.Concat(internalDuplicate[1].Assets).ToArray() };
            Reject(() => BoundedDiagnosticBuild.ValidateExternalDependencies(ordered,internalDuplicate),"resolve uniquely");
            BoundedDiagnosticBuild.ValidateExternalDependencies(ordered,MultisoundFXStreamSmokeTest.Metadata(paths));
            // Reborn: matching snapshot corruption cannot bypass the independent header/native/identity readback.
            foreach (string stream in new[] { "diagnostic.bin","diagnostic.relo","diagnostic.imp" })
            {
                foreach (int offset in new[] { 0,4 })
                {
                    byte[] bad = (byte[])payloads[stream].Clone(); bad[offset] ^= 1;
                    ModifierFXStreamSmokeTest.RejectCorruption(first,stream,bad,ordered,chunks,payloads,checksum,names);
                }
                ModifierFXStreamSmokeTest.RejectCorruption(first,stream,payloads[stream][..^1],ordered,chunks,payloads,checksum,names);
                ModifierFXStreamSmokeTest.RejectCorruption(first,stream,payloads[stream].Concat(new byte[] { 0 }).ToArray(),ordered,chunks,payloads,checksum,names);
            }
            foreach (int offset in new[] { 160,172,212,316 })
            {
                byte[] bad = (byte[])payloads["diagnostic.bin"].Clone(); BinaryPrimitives.WriteUInt32LittleEndian(bad.AsSpan(offset,4),0);
                ModifierFXStreamSmokeTest.RejectCorruption(first,"diagnostic.bin",bad,ordered,chunks,payloads,checksum,names);
            }
            foreach (string stream in new[] { "diagnostic.relo","diagnostic.imp" })
            {
                byte[] bad = (byte[])payloads[stream].Clone(); bad[8] ^= 4;
                ModifierFXStreamSmokeTest.RejectCorruption(first,stream,bad,ordered,chunks,payloads,checksum,names);
            }
            int references = 4 + 48 + 3 * 48;
            byte[] swapped = (byte[])payloads["diagnostic.manifest"].Clone();
            Array.Copy(payloads["diagnostic.manifest"],references + 8,swapped,references,8);
            Array.Copy(payloads["diagnostic.manifest"],references,swapped,references + 8,8);
            ModifierFXStreamSmokeTest.RejectCorruption(first,"diagnostic.manifest",swapped,ordered,chunks,payloads,checksum,names);
            byte[] wrongType = (byte[])payloads["diagnostic.manifest"].Clone();
            BinaryPrimitives.WriteUInt32LittleEndian(wrongType.AsSpan(references,4),0x844D7B9Fu);
            ModifierFXStreamSmokeTest.RejectCorruption(first,"diagnostic.manifest",wrongType,ordered,chunks,payloads,checksum,names);
            try { ModifierFXStreamSmokeTest.WriteNew(first,repeated); throw new InvalidOperationException("Existing output was replaced."); } catch (InvalidDataException) { }
            BoundedDiagnosticBuild.Verify(first,ordered,chunks,payloads,checksum,names);
            // Reborn: losing the second AudioFile revokes event, sound and FX preparation on repeated attempts.
            Settings.Current.ProcessedExternalManifests = paths.Take(1).ToArray(); Missing(doc,parent); Missing(doc,parent);
            Require(ordered.All(instance => instance.ValidatedReferencedInstances == null),"Failed event chain retained prepared tables.");
            foreach (InstanceDeclaration instance in ordered)
                EntryReject(() => plugins.GetPlugin(instance.Handle.TypeId).ProcessInstance(instance));
            Reject(() => BoundedDiagnosticBuild.ValidateExternalDependencies(ordered,MultisoundFXStreamSmokeTest.Metadata(paths)),"Prepared diagnostic dependencies");
            Settings.Current.ProcessedExternalManifests = paths; DependencyResolutionSmokeTest.Prepare(doc,parent);
            BoundedDiagnosticBuild.ValidateExternalDependencies(ordered,MultisoundFXStreamSmokeTest.Metadata(paths));
            Require(MultisoundFXStreamSmokeTest.Compile(plugins,ordered).Zip(chunks).All(pair => Equal(pair.First,pair.Second)),"Recovered event chain differs.");
            // Reborn: edits in an included AudioEvent leaf refresh external identity and every local ancestor before new serialization.
            File.WriteAllText(leaf,original.Replace("WImpact_DebrisVsGroundb","WImpact_DebrisVsGroundc",StringComparison.Ordinal));
            AssetDeclarationDocument edited = Read(processor,source); InstanceDeclaration[] changed = Prepare(edited,edited.SelfInstances.Single());
            Require(changed[0].ValidatedReferencedInstances![1] == audio[2],"Edited event leaf retained stale AudioFile identity.");
            BoundedDiagnosticBuild.ValidateExternalDependencies(changed,MultisoundFXStreamSmokeTest.Metadata(paths));
            AssetBuffer[] changedChunks = MultisoundFXStreamSmokeTest.Compile(plugins,changed);
            var editedPayloads = MultisoundFXStreamSmokeTest.Serialize(changed,changedChunks,paths,names,out uint editedChecksum);
            string editedDirectory = Path.Combine(directory,"edited"); ModifierFXStreamSmokeTest.WriteNew(editedDirectory,editedPayloads);
            BoundedDiagnosticBuild.Verify(editedDirectory,changed,changedChunks,editedPayloads,editedChecksum,names); CheckSelectors(editedDirectory,new[] { audio[0],audio[2] });
            File.WriteAllText(leaf,original);
            AssetDeclarationDocument restored = Read(processor,source); InstanceDeclaration[] recovered = Prepare(restored,restored.SelfInstances.Single());
            Require(MultisoundFXStreamSmokeTest.Compile(plugins,recovered).Zip(chunks).All(pair => Equal(pair.First,pair.Second)),"Restored event leaf did not recover bytes.");
            if (stocks.Length > 0)
            {
                Settings.Current.ProcessedExternalManifests = stocks.Select(Path.GetFullPath).ToArray();
                AssetDeclarationDocument real = Read(processor,source); InstanceDeclaration[] selected = Prepare(real,real.SelfInstances.Single());
                BoundedDiagnosticBuild.ValidateExternalDependencies(selected,MultisoundFXStreamSmokeTest.Metadata(Settings.Current.ProcessedExternalManifests));
                AssetBuffer[] realChunks = MultisoundFXStreamSmokeTest.Compile(plugins,selected);
                Require(realChunks.Zip(chunks).All(pair => Equal(pair.First,pair.Second)),"Actual AudioFile mappings changed chain native bytes.");
                string[] realNames = stocks.Select((_,index) => "data\\stock-"+index+".manifest").ToArray();
                var realPayloads = MultisoundFXStreamSmokeTest.Serialize(selected,realChunks,Settings.Current.ProcessedExternalManifests,realNames,out uint realChecksum);
                string realDirectory = Path.Combine(directory,"stock"); ModifierFXStreamSmokeTest.WriteNew(realDirectory,realPayloads);
                BoundedDiagnosticBuild.Verify(realDirectory,selected,realChunks,realPayloads,realChecksum,realNames); CheckSelectors(realDirectory,audio.Take(2).ToArray());
                Console.WriteLine("  Actual EP1 AudioFile metadata: unique 53C81E47/non-tokenized identities; local native 312/36/28 and linked readback OK.");
            }
            Policy(plugins.ValidateProductionOutput); processor.Cache = null!;
            Policy(() => processor.ProcessDocumentInternal("missing","missing",null!,new DocumentProcessor.ProcessOptions { GenerateOutput = true }));
            Require(!plugins.CanReuseCompiledDocuments,"Event chain enabled compiled reuse.");
            Console.WriteLine($"AudioEvent/Multisound/FX stream self-test: OK (312/36/28 native, 320/44/36 linked; unique AudioFile fingerprints; 20 corruptions; missing/edit/recovery; existing output preserved); diagnostic={Path.Combine(first,"diagnostic.manifest")}");
        }
        finally { Settings.Current = saved; }
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: derive fixed dependency-first order from real recursive closure, excluding unselected roots and external AudioFile payloads. */
    //-------------------------------------------------------------------------------------------------
    private static InstanceDeclaration[] Prepare(AssetDeclarationDocument doc,InstanceDeclaration parent)
    {
        DependencyResolutionSmokeTest.Prepare(doc,parent);
        var selected = DependencyResolutionSmokeTest.Visited(doc).Values.OrderBy(instance => instance.Handle.TypeId switch
            { 0x844D7B9Fu => 0,0xA3A7AF37u => 1,0x86682E78u => 2,_ => throw new InvalidDataException("Unexpected event chain type.") }).ToArray();
        Require(selected.Length == 3,"Fixed event chain must contain exactly three selected local roots."); return selected;
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: independently check explicit local roots, source attribution, native slots, weighted defaults and concrete import identities. */
    //-------------------------------------------------------------------------------------------------
    private static void CheckSelectors(string directory,InstanceHandle[] audio)
    {
        ManifestDocument manifest = ManifestReader.Read(File.ReadAllBytes(Path.Combine(directory,"diagnostic.manifest")));
        byte[] bin = File.ReadAllBytes(Path.Combine(directory,"diagnostic.bin"));
        Require(manifest.Assets.Count == 3 && manifest.Header.TotalInstanceDataSize == 312 && manifest.Header.AssetReferenceBufferSize == 32
            && manifest.Assets.Select(asset => asset.SourceFile).SequenceEqual(new[] { "event.xml","sound.xml","fx.xml" }),"Event chain totals/sources differ.");
        Require(BitConverter.ToSingle(bin,12) == 60 * 0.01f && BitConverter.ToUInt32(bin,52) == 8
            && BitConverter.ToUInt32(bin,88) == 176 && BitConverter.ToUInt32(bin,92) == 180
            && BitConverter.ToUInt32(bin,144) == 2 && BitConverter.ToUInt32(bin,148) == 152
            && BitConverter.ToUInt32(bin,164) == 1000 && BitConverter.ToUInt32(bin,176) == 800
            && BitConverter.ToSingle(bin,168) == 1 && BitConverter.ToSingle(bin,180) == 1
            && BitConverter.ToSingle(bin,188) == -5 && BitConverter.ToSingle(bin,192) == 5,"Local AudioEvent native fields differ.");
        Require(BitConverter.ToUInt32(bin,200) == 2 && BitConverter.ToUInt32(bin,204) == 1
            && BitConverter.ToUInt32(bin,208) == 16 && BitConverter.ToUInt32(bin,216) == 1000
            && bin.AsSpan(220,20).IndexOfAnyExcept((byte)0) == -1,"Local Multisound control/list/weight/pointers differ.");
        AssetId[][] expected = { audio.Select(handle => new AssetId(handle.TypeId,handle.InstanceId)).ToArray(),
            new[] { new AssetId(0x844D7B9Fu,InstanceHandle.GetInstanceId("RebornLocalAudio")) },
            new[] { new AssetId(0xA3A7AF37u,InstanceHandle.GetInstanceId("RebornLocalMultisound")) } };
        int[][] slots = { new[] { 160,172 },new[] { 212 },new[] { 316 } };
        for (int asset = 0; asset < 3; asset++)
        {
            Require(manifest.Assets[asset].References.SequenceEqual(expected[asset]),"Event chain reference table differs.");
            for (int index = 0; index < slots[asset].Length; index++)
            {
                uint selector = BitConverter.ToUInt32(bin,slots[asset][index]);
                Require(selector == index + 1 && manifest.Assets[asset].References[(int)selector - 1] == expected[asset][index],"Event chain selector chose a wrong identity.");
            }
        }
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: force current Include/schema processing despite precompiled requests, never production output. */
    //-------------------------------------------------------------------------------------------------
    private static AssetDeclarationDocument Read(DocumentProcessor processor,string source) => processor.ProcessDocumentInternal(source,source,null!,new DocumentProcessor.ProcessOptions { GenerateOutput = false,UsePrecompiled = true });

    //-------------------------------------------------------------------------------------------------
    /** Reborn: preserve the official namespace in owned test inputs. */
    //-------------------------------------------------------------------------------------------------
    private static string Xml(string body) => "<AssetDeclaration xmlns=\"uri:ea.com:eala:asset\">"+body+"</AssetDeclaration>";

    //-------------------------------------------------------------------------------------------------
    /** Reborn: require recursive missing-leaf failure on each preparation attempt. */
    //-------------------------------------------------------------------------------------------------
    private static void Missing(AssetDeclarationDocument doc,InstanceDeclaration parent)
    { try { DependencyResolutionSmokeTest.Prepare(doc,parent); } catch (BinaryAssetBuilderException error) when (error.ErrorCode == ErrorCode.UnknownReference) { return; } throw new InvalidDataException("Missing AudioFile leaf was accepted."); }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: require specific independent metadata failure messages. */
    //-------------------------------------------------------------------------------------------------
    private static void Reject(Action action,string message)
    { try { action(); } catch (InvalidDataException error) when (error.Message.Contains(message,StringComparison.Ordinal)) { return; } throw new InvalidOperationException("Invalid AudioFile admission was accepted."); }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: revoked prepared entries must fail eligibility before native allocation. */
    //-------------------------------------------------------------------------------------------------
    private static void EntryReject(Action action)
    { try { action(); } catch (NotSupportedException) { return; } throw new InvalidOperationException("Revoked event chain entry was accepted."); }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: production policy must reject before input/cache access. */
    //-------------------------------------------------------------------------------------------------
    private static void Policy(Action action)
    { try { action(); } catch (BinaryAssetBuilderException error) when (error.Message.Contains("Experimental",StringComparison.Ordinal)) { return; } throw new InvalidOperationException("Event chain production policy was bypassed."); }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: compare every compiled native buffer byte across preparation attempts. */
    //-------------------------------------------------------------------------------------------------
    private static bool Equal(AssetBuffer a,AssetBuffer b) => a.InstanceData.SequenceEqual(b.InstanceData) && a.RelocationData.SequenceEqual(b.RelocationData) && a.ImportsData.SequenceEqual(b.ImportsData);

    //-------------------------------------------------------------------------------------------------
    /** Reborn: fail at the first mixed-stream evidence violation. */
    //-------------------------------------------------------------------------------------------------
    private static void Require(bool condition,string message) { if (!condition) throw new InvalidDataException(message); }
}
