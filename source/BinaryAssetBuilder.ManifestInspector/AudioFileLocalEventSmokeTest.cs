using BinaryAssetBuilder.Core;
using BinaryAssetBuilder.XmlCompiler;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: prove fixed local event/audio closure with synthetic compressed bodies; native codecs remain opt-in.
internal static class AudioFileLocalEventSmokeTest
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: exercise core normalization, local tuple/selectors, parent fingerprint invalidation and mixed readback/corruptions. */
    //-------------------------------------------------------------------------------------------------
    internal static void Run()
    {
        string root = Path.Combine(Path.GetTempPath(),"Reborn-LocalAudioPackage-"+Guid.NewGuid().ToString("N")); Directory.CreateDirectory(root);
        const string subtitle = "DIALOGEVENT:reborn_audio_encoder_pocSubTitle";
        var files = new[] { new AudioFilePackageProbe.Entry("RebornAudioRAM","ram.xml",
            Ra3Ep1AudioFileRuntimeSerializer.Serialize(TargetPlatform.Win32,subtitle,12000,48000,1,Array.Empty<byte>()),Convert.FromHexString("0400BB8000002EE00000000C00002EE0DEADBEEF")),
            new AudioFilePackageProbe.Entry("RebornAudioStream","streamed.xml",
            Ra3Ep1AudioFileRuntimeSerializer.Serialize(TargetPlatform.Win32,subtitle,12000,48000,1,Convert.FromHexString("0400BB8040002EE0")),Convert.FromHexString("8000000C00002EE0DEADBEEF")) };
        AudioFileLocalEventProbe.Entry parent = AudioFileLocalEventProbe.Build(root,files);
        // Reborn: test current core-normalized identities/source values, not just corruption after a successful serialization.
        string auditDirectory = Path.Combine(root,"audit"); Directory.CreateDirectory(auditDirectory);
        AudioFileLocalEventProbe.Entry audited = AudioFileLocalEventProbe.Build(auditDirectory,files,(instance,plugin) =>
        {
            AudioFileLocalEventProbe.Compile(instance,files,plugin);
            InstanceHandle original = instance.ReferencedInstances[0];
            instance.ReferencedInstances[0] = new InstanceHandle("AudioFile","Changed");
            Reject(() => AudioFileLocalEventProbe.Compile(instance,files,plugin)); Require(instance.ValidatedReferencedInstances == null,"Wrong identity retained preparation."); instance.ReferencedInstances[0] = original;
            foreach (var missing in new[] { files.Take(1).ToArray(),files.Reverse().ToArray(),new[] { files[0],files[0] } })
            { Reject(() => AudioFileLocalEventProbe.Compile(instance,missing,plugin)); Require(instance.ValidatedReferencedInstances == null,"Incomplete/wrong local table retained preparation."); }
            uint hash = instance.Handle.TypeHash; instance.Handle.TypeHash = 0;
            Reject(() => AudioFileLocalEventProbe.Compile(instance,files,plugin)); instance.Handle.TypeHash = hash;
            var element = (System.Xml.XmlElement)instance.Node;
            var sound = element.ChildNodes.OfType<System.Xml.XmlElement>().First(); string selector = sound.InnerText;
            foreach (string bad in new[] { "AudioFile:Changed\\0","AudioFile:RebornAudioRAM\\99" })
            { sound.InnerText = bad; Reject(() => AudioFileLocalEventProbe.Compile(instance,files,plugin)); Require(instance.ValidatedReferencedInstances == null,"Invalid current selector retained preparation."); }
            sound.InnerText = selector; string volume = element.GetAttribute("Volume"); element.SetAttribute("Volume","NaN");
            Reject(() => AudioFileLocalEventProbe.Compile(instance,files,plugin)); Require(instance.ValidatedReferencedInstances == null,"Invalid scalar retained preparation."); element.SetAttribute("Volume",volume);
            AudioFileLocalEventProbe.Compile(instance,files,plugin);
        });
        Require(audited.Hash == parent.Hash && audited.CopyNative().InstanceData.SequenceEqual(parent.CopyNative().InstanceData),"Recovered current event preparation changed frozen output.");
        var payloads = AudioFilePackageProbe.Serialize(files,parent); string output = Path.Combine(root,"first");
        AudioFilePackageProbe.Publish(output,files,parent); AudioFilePackageProbe.Verify(output,files,parent);
        ManifestDocument manifest = ManifestReader.Read(payloads["diagnostic.manifest"]);
        Require(manifest.Assets.Count == 3 && manifest.Assets[2].References.SequenceEqual(files.Select(file => new AssetId(0x166B084Du,file.Id)))
            && payloads["diagnostic.bin"].Length == 352 && payloads["diagnostic.relo"].Length == 36 && payloads["diagnostic.imp"].Length == 20,"Mixed event/audio sizes/tuples differ.");
        Require(manifest.ReferencedManifests.Count == 0,"Local event/audio package unexpectedly uses external manifests.");
        AssetBuffer copy = parent.CopyNative(); copy.InstanceData[152] ^= 1; parent.CopyNative().ImportsData[0] ^= 1;
        AudioFilePackageProbe.Verify(output,files,parent);
        foreach (string file in new[] { "diagnostic.bin","diagnostic.relo","diagnostic.imp","diagnostic.manifest" })
        {
            byte[] good = payloads[file]; string path = Path.Combine(output,file);
            foreach (int offset in new[] { 0,4,good.Length-1 })
            { byte[] bad = (byte[])good.Clone(); bad[offset] ^= 1; File.WriteAllBytes(path,bad); Reject(() => AudioFilePackageProbe.Verify(output,files,parent)); File.WriteAllBytes(path,good); }
        }
        foreach (int slot in new[] { 136,140,152,156,160,164,168,172 })
        { AssetBuffer bad = parent.CopyNative(); bad.InstanceData[slot] ^= 1; Reject(() => new AudioFileLocalEventProbe.Entry(bad,files)); }
        foreach (bool imports in new[] { false,true })
        { AssetBuffer bad = parent.CopyNative(); (imports ? bad.ImportsData : bad.RelocationData)[0] ^= 4; Reject(() => new AudioFileLocalEventProbe.Entry(bad,files)); }
        byte[] custom = files[0].CopyCustom(); custom[^1] ^= 1;
        var edited = new[] { new AudioFilePackageProbe.Entry(files[0].Name,files[0].Source,files[0].CopyNative(),custom),files[1] };
        Reject(() => AudioFilePackageProbe.Serialize(edited,parent)); Reject(() => AudioFilePackageProbe.Publish(Path.Combine(root,"stale"),edited,parent));
        Require(!Directory.Exists(Path.Combine(root,"stale")),"Stale parent was published.");
        Reject(() => parent.ValidateDependencies(files.Reverse().ToArray())); Reject(() => parent.ValidateDependencies(new[] { files[0],files[0] }));
        string changed = Path.Combine(root,"changed"); Directory.CreateDirectory(changed);
        AudioFileLocalEventProbe.Entry refreshed = AudioFileLocalEventProbe.Build(changed,edited);
        Require(refreshed.Hash != parent.Hash && refreshed.CopyNative().InstanceData.SequenceEqual(parent.CopyNative().InstanceData),"Changed leaf failed to refresh diagnostic parent identity independently of native selectors.");
        AudioFilePackageProbe.Publish(Path.Combine(changed,"package"),edited,refreshed); AudioFilePackageProbe.Verify(Path.Combine(changed,"package"),edited,refreshed);
        Reject(() => AudioFilePackageProbe.Publish(output,files,parent)); AudioFilePackageProbe.Verify(output,files,parent);
        var repeated = AudioFilePackageProbe.Serialize(files,parent);
        Require(payloads.All(file => file.Value.SequenceEqual(repeated[file.Key])),"Recovered mixed package differs.");
        // Reborn: arbitrary caller names and old names exchanged between slots must survive core normalization and manifest closure unchanged.
        foreach (var names in new[] { (Ram:"Caller_RAM-01",Stream:"Caller_Stream-02"),(Ram:"RebornAudioStream",Stream:"RebornAudioRAM") })
        {
            var named = new[] { new AudioFilePackageProbe.Entry(names.Ram,"ram.xml",files[0].CopyNative(),files[0].CopyCustom()),
                new AudioFilePackageProbe.Entry(names.Stream,"streamed.xml",files[1].CopyNative(),files[1].CopyCustom()) };
            string namedDirectory = Path.Combine(root,Guid.NewGuid().ToString("N")); Directory.CreateDirectory(namedDirectory);
            var namedEvent = AudioFileLocalEventProbe.Build(namedDirectory,named,(instance,plugin) =>
            {
                InstanceHandle original = instance.ReferencedInstances[0]; instance.ReferencedInstances[0] = instance.ReferencedInstances[1];
                Reject(() => AudioFileLocalEventProbe.Compile(instance,named,plugin)); instance.ReferencedInstances[0] = original;
            });
            AudioFilePackageProbe.Publish(Path.Combine(namedDirectory,"package"),named,namedEvent);
            AudioFilePackageProbe.Verify(Path.Combine(namedDirectory,"package"),named,namedEvent);
            Reject(() => parent.ValidateDependencies(named));
        }
        var alias = new AudioFilePackageProbe.Entry("rebornAudioRAM","streamed.xml",files[1].CopyNative(),files[1].CopyCustom());
        Reject(() => AudioFilePackageProbe.Serialize(new[] { files[0],alias }));
        Require(AudioFileLocalEventProbe.Source(files) == AudioFileLocalEventProbe.SourceXml,"Baseline source compatibility differs.");
        // Reborn: compile installed raw caller provenance without rewriting it and retain the custom event identity in both readers.
        string authoredDirectory = Path.Combine(root,"authored-event"); Directory.CreateDirectory(authoredDirectory);
        byte[] authoredXml = System.Text.Encoding.UTF8.GetBytes(SourceWithComment());
        File.WriteAllBytes(Path.Combine(authoredDirectory,"event.xml"),authoredXml);
        var authored = AudioFileLocalEventProbe.Build(authoredDirectory,files,authoredName:"CallerLocalEvent");
        Require(authored.Name == "CallerLocalEvent" && authored.Id == InstanceHandle.GetInstanceId("CallerLocalEvent")
            && authored.CopyNative().InstanceData.SequenceEqual(parent.CopyNative().InstanceData)
            && File.ReadAllBytes(Path.Combine(authoredDirectory,"event.xml")).SequenceEqual(authoredXml),"Authored event identity/provenance changed.");
        AudioFilePackageProbe.Publish(Path.Combine(authoredDirectory,"package"),files,authored);
        AudioFilePackageProbe.Verify(Path.Combine(authoredDirectory,"package"),files,authored);
        // Reborn: compare varied authored scalar words independently with the stock layout and preserve all untouched bytes/tables.
        foreach (var scalars in new[] { (Volume:"37.5",Bits:0x3EC00000u,Ram:125u,Stream:875u),(Volume:"0",Bits:0u,Ram:0u,Stream:1000000u),
            (Volume:"100",Bits:0x3F800000u,Ram:1000000u,Stream:0u),(Volume:"60",Bits:0x3F199999u,Ram:1000u,Stream:1000u) })
        {
            string variedDirectory = Path.Combine(root,Guid.NewGuid().ToString("N")); Directory.CreateDirectory(variedDirectory);
            string xml = SourceWithComment().Replace("Volume=\"60\"","Volume=\""+scalars.Volume+"\"")
                .Replace("<Sound>","<Sound Weight=\""+scalars.Ram+"\">").Replace("Weight=\"800\"","Weight=\""+scalars.Stream+"\"");
            // Reborn: absent weights must compile exactly like the explicit official 1000 default on both sounds.
            if (scalars.Volume == "60") xml = xml.Replace(" Weight=\"1000\"","");
            File.WriteAllBytes(Path.Combine(variedDirectory,"event.xml"),System.Text.Encoding.UTF8.GetBytes(xml));
            var varied = AudioFileLocalEventProbe.Build(variedDirectory,files,authoredName:"CallerLocalEvent");
            AssetBuffer golden = parent.CopyNative();
            System.Buffers.Binary.BinaryPrimitives.WriteUInt32LittleEndian(golden.InstanceData.AsSpan(4),scalars.Bits);
            System.Buffers.Binary.BinaryPrimitives.WriteUInt32LittleEndian(golden.InstanceData.AsSpan(156),scalars.Ram);
            System.Buffers.Binary.BinaryPrimitives.WriteUInt32LittleEndian(golden.InstanceData.AsSpan(168),scalars.Stream);
            Require(golden.InstanceData.SequenceEqual(varied.CopyNative().InstanceData) && golden.RelocationData.SequenceEqual(varied.CopyNative().RelocationData)
                && golden.ImportsData.SequenceEqual(varied.CopyNative().ImportsData) && varied.Hash != parent.Hash,"Varied event scalar bytes/hash differ.");
            AudioFilePackageProbe.Publish(Path.Combine(variedDirectory,"package"),files,varied);
            AudioFilePackageProbe.Verify(Path.Combine(variedDirectory,"package"),files,varied);
            foreach (int offset in new[] { 4,156,168 })
            { AssetBuffer bad = varied.CopyNative(); bad.InstanceData[offset] ^= 1; Reject(() => new AudioFileLocalEventProbe.Entry(bad,files,varied.Name,varied.Settings)); }
            Reject(() => new AudioFileLocalEventProbe.Entry(varied.CopyNative(),files,varied.Name));
        }
        foreach (var bad in new[] { new AuthoredAudioEventSource.Settings(float.NaN,1,1),new AuthoredAudioEventSource.Settings(101,1,1),
            new AuthoredAudioEventSource.Settings(60,0,0),new AuthoredAudioEventSource.Settings(60,1000001,1) })
            Reject(() => new AudioFileLocalEventProbe.Entry(parent.CopyNative(),files,settings:bad));
        // Reborn: prove source-selected singleton/reversed references, exact variable native/import shapes and dependency hash scope.
        foreach (int[] slots in new[] { new[] { 0 },new[] { 1 },new[] { 1,0 } })
        {
            string selectedDirectory = Path.Combine(root,Guid.NewGuid().ToString("N")); Directory.CreateDirectory(selectedDirectory);
            string sounds = string.Concat(slots.Select(slot => "<Sound>AudioFile:"+files[slot].Name+"</Sound>"));
            string xml = SourceWithComment().Replace("<Sound>AudioFile:RebornAudioRAM</Sound><Sound Weight=\"800\">AudioFile:RebornAudioStream</Sound>",sounds);
            File.WriteAllBytes(Path.Combine(selectedDirectory,"event.xml"),System.Text.Encoding.UTF8.GetBytes(xml));
            var selected = AudioFileLocalEventProbe.Build(selectedDirectory,files,authoredName:"CallerLocalEvent");
            byte[] goldenBin = new byte[152+12*slots.Length],goldenImp = new byte[4*(slots.Length+1)];
            parent.CopyNative().InstanceData.AsSpan(0,152).CopyTo(goldenBin);
            System.Buffers.Binary.BinaryPrimitives.WriteUInt32LittleEndian(goldenBin.AsSpan(136),(uint)slots.Length);
            for (int index = 0; index < slots.Length; index++)
            {
                System.Buffers.Binary.BinaryPrimitives.WriteUInt32LittleEndian(goldenBin.AsSpan(152+12*index),(uint)(index+1));
                System.Buffers.Binary.BinaryPrimitives.WriteUInt32LittleEndian(goldenBin.AsSpan(156+12*index),1000);
                System.Buffers.Binary.BinaryPrimitives.WriteUInt32LittleEndian(goldenBin.AsSpan(160+12*index),0x3F800000u);
                System.Buffers.Binary.BinaryPrimitives.WriteUInt32LittleEndian(goldenImp.AsSpan(4*index),(uint)(152+12*index));
            }
            System.Buffers.Binary.BinaryPrimitives.WriteUInt32LittleEndian(goldenImp.AsSpan(4*slots.Length),uint.MaxValue);
            Require(selected.CopyNative().InstanceData.SequenceEqual(goldenBin) && selected.CopyNative().ImportsData.SequenceEqual(goldenImp)
                && selected.CopyNative().RelocationData.SequenceEqual(parent.CopyNative().RelocationData),"Selected list native golden differs.");
            var expectedRefs = slots.Select(slot => new AssetId(0x166B084Du,files[slot].Id)).ToArray();
            Require(selected.References(files).SequenceEqual(expectedRefs),"Selected manifest reference order differs.");
            string outputSelected = Path.Combine(selectedDirectory,"package"); AudioFilePackageProbe.Publish(outputSelected,files,selected);
            AudioFilePackageProbe.Verify(outputSelected,files,selected);
            var selectedManifest = ManifestReader.Read(File.ReadAllBytes(Path.Combine(outputSelected,"diagnostic.manifest")));
            Require(selectedManifest.Assets[2].References.SequenceEqual(expectedRefs),"Serialized selected references differ.");
            foreach (int offset in new[] { 136,140,152,156,160 })
            { AssetBuffer bad = selected.CopyNative(); bad.InstanceData[offset] ^= 1; Reject(() => new AudioFileLocalEventProbe.Entry(bad,files,selected.Name,selected.Settings)); }
            // Reborn: cardinality-specific auxiliary tables cannot be replaced by the old two-reference envelope.
            foreach (bool imports in new[] { false,true })
            { AssetBuffer bad = selected.CopyNative(); (imports ? bad.ImportsData : bad.RelocationData)[^1] ^= 1; Reject(() => new AudioFileLocalEventProbe.Entry(bad,files,selected.Name,selected.Settings)); }
            foreach (int slot in new[] { 0,1 })
            {
                byte[] editedCustom = files[slot].CopyCustom(); editedCustom[^1] ^= 1;
                var editedFiles = (AudioFilePackageProbe.Entry[])files.Clone(); editedFiles[slot] = new(files[slot].Name,files[slot].Source,files[slot].CopyNative(),editedCustom);
                if (slots.Contains(slot)) Reject(() => selected.ValidateDependencies(editedFiles));
                else
                {
                    selected.ValidateDependencies(editedFiles);
                    AudioFilePackageProbe.Publish(Path.Combine(selectedDirectory,"unused-leaf-change"),editedFiles,selected);
                }
            }
        }
        // Reborn: malformed reconstructed cardinalities/slots reject before any native evidence can bless them.
        foreach (var bad in new[] { new AuthoredAudioEventSource.Settings(60,1,1,0),new AuthoredAudioEventSource.Settings(60,1,1,3),
            new AuthoredAudioEventSource.Settings(60,1,1,2,0,0),new AuthoredAudioEventSource.Settings(60,1,0,1,2,-1),
            new AuthoredAudioEventSource.Settings(60,1,0,1,0,1),new AuthoredAudioEventSource.Settings(60,0,0,1,0,-1) }) Reject(bad.Validate);
        Console.WriteLine("Local AudioEvent/audio package self-test: OK (actual core-normalized event; explicitly prepared two local AudioFiles; 352/36/20 linked; native selectors/tuples; corruption/ownership/stale-leaf refresh; production/general graph closed)");
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: unsupported or stale local package evidence must fail instead of being silently rebound. */
    //-------------------------------------------------------------------------------------------------
    private static void Reject(Action action)
    {
        try { action(); } catch (Exception error) when (error is InvalidDataException or IOException or ArgumentException or NotSupportedException) { return; }
        throw new InvalidOperationException("Invalid local event/audio package accepted.");
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: stop at the first local graph proof mismatch. */
    //-------------------------------------------------------------------------------------------------
    private static void Require(bool condition,string message) { if (!condition) throw new InvalidDataException(message); }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: comments and whitespace are caller provenance, never regenerated from normalized/defaulted XML. */
    //-------------------------------------------------------------------------------------------------
    private static string SourceWithComment() => "<!-- Reborn: owned caller event fixture. -->\n"+AudioFileLocalEventProbe.SourceXml.Replace("RebornLocalAudio","CallerLocalEvent");
}
