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
}
