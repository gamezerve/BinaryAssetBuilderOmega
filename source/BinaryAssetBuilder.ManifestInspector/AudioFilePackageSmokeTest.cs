using BinaryAssetBuilder.Core;
using BinaryAssetBuilder.XmlCompiler;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: test diagnostic audio packaging with fake compressed bodies; never execute codecs in default tests.
internal static class AudioFilePackageSmokeTest
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: prove two-entry linked/custom identity mapping, frozen ownership, corruption rejection and existing-output preservation. */
    //-------------------------------------------------------------------------------------------------
    internal static void Run()
    {
        string directory = Path.Combine(Path.GetTempPath(),"Reborn-AudioPackageTest-"+Guid.NewGuid().ToString("N")); Directory.CreateDirectory(directory);
        byte[] header = Convert.FromHexString("0400BB8040002EE0");
        byte[] ram = Convert.FromHexString("0400BB8000002EE00000000C00002EE0DEADBEEF"),stream = Convert.FromHexString("8000000C00002EE0DEADBEEF");
        const string subtitle = "DIALOGEVENT:reborn_audio_encoder_pocSubTitle";
        AssetBuffer ramNative = Ra3Ep1AudioFileRuntimeSerializer.Serialize(TargetPlatform.Win32,subtitle,12000,48000,1,Array.Empty<byte>());
        AssetBuffer streamNative = Ra3Ep1AudioFileRuntimeSerializer.Serialize(TargetPlatform.Win32,subtitle,12000,48000,1,header);
        var entries = new[] { new AudioFilePackageProbe.Entry("RebornAudioRAM","ram.xml",ramNative,ram),new AudioFilePackageProbe.Entry("RebornAudioStream","streamed.xml",streamNative,stream) };
        var first = AudioFilePackageProbe.Serialize(entries); string output = Path.Combine(directory,"first"); AudioFilePackageProbe.Publish(output,entries); AudioFilePackageProbe.Verify(output,entries);
        ManifestDocument parsed = ManifestReader.Read(first["diagnostic.manifest"]);
        Require(parsed.Assets.Count == 2 && first["diagnostic.bin"].Length == 176 && first["diagnostic.relo"].Length == 28 && first["diagnostic.imp"].Length == 8,"Audio package sizes differ.");
        Require(entries.All(entry => first.ContainsKey(Path.Combine("diagnostic","cdata",entry.CustomName))),"Custom identity path differs.");
        // Reborn: edits to caller inputs, copied native/custom data or returned payload dictionaries cannot mutate captured evidence.
        ram[^1] ^= 1; ramNative.InstanceData[0] = 1; entries[0].CopyNative().InstanceData[0] = 1;
        byte[] detached = entries[1].CopyCustom(); detached[^1] ^= 1; first["diagnostic.bin"][0] ^= 1;
        var restored = AudioFilePackageProbe.Serialize(entries); AudioFilePackageProbe.Verify(output,entries);
        Require(restored["diagnostic.bin"][0] == 0 && entries[1].CopyCustom()[^1] == 0xEF,"Captured package alias escaped.");
        string repeat = Path.Combine(directory,"repeat"); AudioFilePackageProbe.Publish(repeat,entries); AudioFilePackageProbe.Verify(repeat,entries);
        foreach (var file in restored) Require(File.ReadAllBytes(Path.Combine(output,file.Key)).SequenceEqual(File.ReadAllBytes(Path.Combine(repeat,file.Key))),"Repeated package differs.");
        Reject(() => AudioFilePackageProbe.Serialize(new[] { entries[0],entries[0] })); Reject(() => AudioFilePackageProbe.Serialize(entries.Reverse().ToArray()));
        Reject(() => AudioFilePackageProbe.Publish(output,entries)); AudioFilePackageProbe.Verify(output,entries);
        string existingFile = Path.Combine(directory,"existing"); File.WriteAllBytes(existingFile,new byte[] { 42 });
        Reject(() => AudioFilePackageProbe.Publish(existingFile,entries)); Require(File.ReadAllBytes(existingFile).SequenceEqual(new byte[] { 42 }),"Existing output file replaced.");
        foreach (string name in new[] { "diagnostic.manifest","diagnostic.bin","diagnostic.relo","diagnostic.imp" }.Concat(entries.Select(entry => Path.Combine("diagnostic","cdata",entry.CustomName))))
        {
            string path = Path.Combine(output,name); byte[] good = File.ReadAllBytes(path);
            foreach (byte[] bad in new[] { good[..^1],good.Concat(new byte[] { 0 }).ToArray(),Flip(good,0),Flip(good,good.Length-1) })
            { File.WriteAllBytes(path,bad); Reject(() => AudioFilePackageProbe.Verify(output,entries)); File.WriteAllBytes(path,good); }
            File.Move(path,path+".missing"); Reject(() => AudioFilePackageProbe.Verify(output,entries)); File.Move(path+".missing",path);
        }
        string orphan = Path.Combine(output,"diagnostic","cdata","unexpected.cdata"); File.WriteAllBytes(orphan,new byte[] { 1 });
        Reject(() => AudioFilePackageProbe.Verify(output,entries)); File.Move(orphan,Path.Combine(directory,"retained-orphan.cdata"));
        AudioFilePackageProbe.Verify(output,entries);
        AssetBuffer approved = entries[0].CopyNative(); byte[] custom = entries[0].CopyCustom();
        foreach (int offset in new[] { 0,4,8,12,16,20,24,28 })
        { AssetBuffer bad = entries[0].CopyNative(); bad.InstanceData[offset] ^= 1; Reject(() => new AudioFilePackageProbe.Entry("RebornAudioRAM","ram.xml",bad,custom)); }
        AssetBuffer wrongRelocation = entries[0].CopyNative(); wrongRelocation.RelocationData[0] ^= 4;
        Reject(() => new AudioFilePackageProbe.Entry("RebornAudioRAM","ram.xml",wrongRelocation,custom));
        // Reborn: arbitrary literal names now pass; only unsafe tokens are rejected at the identity boundary.
        Reject(() => new AudioFilePackageProbe.Entry("Other:Selector","ram.xml",approved,custom));
        Reject(() => new AudioFilePackageProbe.Entry("RebornAudioRAM","../ram.xml",approved,custom));
        Reject(() => new AudioFilePackageProbe.Entry("RebornAudioStream","streamed.xml",approved,custom));
        Reject(() => new AudioFilePackageProbe.Entry("RebornAudioRAM","ram.xml",approved,Flip(custom,0)));
        Reject(() => new AudioFilePackageProbe.Entry("RebornAudioRAM","ram.xml",approved,new byte[1048577]));
        byte[] edited = (byte[])custom.Clone(); edited[^1] ^= 1; var changed = new AudioFilePackageProbe.Entry("RebornAudioRAM","ram.xml",approved,edited);
        Require(changed.Hash != entries[0].Hash && changed.CustomName != entries[0].CustomName,"Payload edit retained stale diagnostic custom identity.");
        Console.WriteLine("AudioFile package self-test: OK (two readers; 176/28/8 linked; custom tuple names; frozen/repeated output; corruption/missing/orphan rejection; existing output preserved; synthetic framing only)");
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: mutate a detached byte for independent package corruption checks. */
    //-------------------------------------------------------------------------------------------------
    private static byte[] Flip(byte[] bytes,int offset) { byte[] copy = (byte[])bytes.Clone(); copy[offset] ^= 1; return copy; }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: invalid package evidence/publication must fail before being treated as approved output. */
    //-------------------------------------------------------------------------------------------------
    private static void Reject(Action action)
    {
        try { action(); } catch (Exception error) when (error is InvalidDataException or IOException or ArgumentException or NotSupportedException) { return; }
        throw new InvalidOperationException("Invalid diagnostic audio package accepted.");
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: stop on the first fixed package contract violation. */
    //-------------------------------------------------------------------------------------------------
    private static void Require(bool condition,string message) { if (!condition) throw new InvalidDataException(message); }
}
