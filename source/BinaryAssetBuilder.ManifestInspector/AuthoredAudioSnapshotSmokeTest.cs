using System.Text;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: test bounded caller-source snapshot admission and ownership without launching native codecs.
internal static class AuthoredAudioSnapshotSmokeTest
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: admit changed PCM/subtitles, freeze exact bytes and reject unsupported/stale authored inputs before worker launch. */
    //-------------------------------------------------------------------------------------------------
    internal static void Run()
    {
        string directory = Path.Combine(Path.GetTempPath(),"Reborn-AuthoredAudio-"+Guid.NewGuid().ToString("N")); Directory.CreateDirectory(directory);
        using MemoryStream pcm = new(); AudioEncoderPoc.WriteWave(pcm); byte[] wave = pcm.ToArray(); wave[100] ^= 3;
        string ram = AudioEncoderPoc.CoreSource(false).Replace("reborn_audio_encoder_pocSubTitle","authored_caseSubTitle",StringComparison.Ordinal);
        string streamed = AudioEncoderPoc.CoreSource(true).Replace("reborn_audio_encoder_pocSubTitle","authored_caseSubTitle",StringComparison.Ordinal);
        Write("ram.xml",Encoding.UTF8.GetBytes(ram)); Write("streamed.xml",Encoding.UTF8.GetBytes(streamed)); Write("input.wav",wave);
        AuthoredAudioSnapshot snapshot = AuthoredAudioSnapshot.Read(directory);
        byte[] copy = snapshot.Copy("input.wav"); copy[100] ^= 1; snapshot.VerifyCurrent();
        string installed = Path.Combine(directory,"snapshot"); snapshot.Install(installed); snapshot.VerifyCopies(installed);
        Reject(() => snapshot.Install(installed));
        DateTime timestamp = File.GetLastWriteTimeUtc(Path.Combine(directory,"input.wav")); byte[] changed = (byte[])wave.Clone(); changed[100] ^= 1;
        Write("input.wav",changed); File.SetLastWriteTimeUtc(Path.Combine(directory,"input.wav"),timestamp); Reject(snapshot.VerifyCurrent); Write("input.wav",wave); snapshot.VerifyCurrent();
        // Reborn: names come from caller XML, including names formerly associated with the opposite play-location slot.
        foreach (var pair in new[] { (Ram:"Caller_RAM-01",Stream:"Caller_Stream-02"),(Ram:"RebornAudioStream",Stream:"RebornAudioRAM"),(Ram:"A",Stream:new string('b',128)) })
        {
            Write("ram.xml",Encoding.UTF8.GetBytes(ram.Replace("RebornAudioRAM",pair.Ram)));
            Write("streamed.xml",Encoding.UTF8.GetBytes(streamed.Replace("RebornAudioStream",pair.Stream)));
            var named = AuthoredAudioSnapshot.Read(directory);
            if (named.RamName != pair.Ram || named.StreamName != pair.Stream) throw new InvalidDataException("Caller identities were silently renamed.");
            named.VerifyCurrent();
        }
        Write("streamed.xml",Encoding.UTF8.GetBytes(streamed));
        foreach (string invalidName in new[] { "", "bad:name", "bad/name", "bad\\name", "bad name", "é",new string('a',129),"RebornAudioStream","rebornAudioStream" })
        { Write("ram.xml",Encoding.UTF8.GetBytes(ram.Replace("RebornAudioRAM",invalidName))); Reject(() => AuthoredAudioSnapshot.Read(directory)); }
        foreach (string invalid in new[] { ram.Replace("input.wav","../input.wav"),ram.Replace("XAS","NONE"),ram.Replace("48000","44100"),
            ram.Replace("IsStreamedOnPC=\"false\"","IsStreamedOnPC=\"true\""),ram.Replace("<AssetDeclaration","<!DOCTYPE x [<!ENTITY e 'x'>]><AssetDeclaration"),
            ram.Replace("<AudioFile","<Includes /><AudioFile"),ram.Replace("File=\"input.wav\"","File=\"input.wav\" inheritFrom=\"Other\""),ram.Replace("PCCompression=\"XAS\"","PCCompression=\"XAS\" XenonQuality=\"75\"") })
        { Write("ram.xml",Encoding.UTF8.GetBytes(invalid)); Reject(() => AuthoredAudioSnapshot.Read(directory)); }
        Write("ram.xml",new byte[8193]); Reject(() => AuthoredAudioSnapshot.Read(directory)); Write("ram.xml",new byte[] { 0xFF }); Reject(() => AuthoredAudioSnapshot.Read(directory)); Write("ram.xml",Encoding.UTF8.GetBytes(ram));
        Write("ram.xml",new byte[] { 0xEF,0xBB,0xBF }.Concat(Encoding.UTF8.GetBytes(ram)).ToArray()); Reject(() => AuthoredAudioSnapshot.Read(directory));
        Write("ram.xml",Encoding.UTF8.GetBytes("<?xml version=\"1.0\" encoding=\"utf-16\"?>"+ram)); Reject(() => AuthoredAudioSnapshot.Read(directory)); Write("ram.xml",Encoding.UTF8.GetBytes(ram));
        foreach (byte[] invalid in new[] { Array.Empty<byte>(),new byte[24045] }) { Write("input.wav",invalid); Reject(() => AuthoredAudioSnapshot.Read(directory)); }
        Write("input.wav",wave); snapshot.VerifyCurrent();
        if (!File.ReadAllBytes(Path.Combine(installed,"input.wav")).SequenceEqual(wave)) throw new InvalidDataException("Authored snapshot copy changed through caller mutation.");
        Console.WriteLine("Authored audio snapshot self-test: OK (bounded XML/PCM/subtitle admission, exact immutable copies, stale timestamp-preserving edits, path/identity/settings/DTD/Include/inheritance rejection and recovery; no native codecs)");

        //-------------------------------------------------------------------------------------------------
        /** Reborn: mutate only this test's owned temporary fixtures; never a user's existing source directory. */
        //-------------------------------------------------------------------------------------------------
        void Write(string name,byte[] bytes) { using FileStream stream = new(Path.Combine(directory,name),FileMode.Create,FileAccess.Write); stream.Write(bytes); }
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: malformed or stale inputs must reject before codec-worker launch. */
    //-------------------------------------------------------------------------------------------------
    private static void Reject(Action action)
    { try { action(); } catch (Exception exception) when (exception is InvalidDataException or IOException or NotSupportedException or System.Xml.XmlException or System.Xml.Schema.XmlSchemaException or DecoderFallbackException) { return; } throw new InvalidDataException("Invalid authored audio input unexpectedly succeeded."); }
}
