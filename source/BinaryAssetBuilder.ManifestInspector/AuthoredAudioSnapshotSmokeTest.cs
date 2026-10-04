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
        // Reborn: explicitly freeze a caller event; three-file mode must ignore it, while four-file mode checks every byte and exact local references.
        string eventXml = AudioFileLocalEventProbe.SourceXml.Replace("RebornLocalAudio","CallerLocalEvent");
        Write("event.xml",Encoding.UTF8.GetBytes(eventXml)); var withEvent = AuthoredAudioSnapshot.Read(directory,true);
        if (withEvent.EventName != "CallerLocalEvent" || withEvent.FileNames.Count() != 4) throw new InvalidDataException("Authored event snapshot lost its identity.");
        string eventCopy = Path.Combine(directory,"event-copy"); withEvent.Install(eventCopy); withEvent.VerifyCopies(eventCopy);
        byte[] eventBytes = withEvent.Copy("event.xml"); eventBytes[0] ^= 1; withEvent.VerifyCurrent();
        // Reborn: an altered installed event must fail the parent's frozen-copy gate independently of schema/native validity.
        Write(Path.Combine("event-copy","event.xml"),eventBytes); Reject(() => withEvent.VerifyCopies(eventCopy));
        Write(Path.Combine("event-copy","event.xml"),withEvent.Copy("event.xml")); withEvent.VerifyCopies(eventCopy);
        DateTime eventTime = File.GetLastWriteTimeUtc(Path.Combine(directory,"event.xml"));
        Write("event.xml",Encoding.UTF8.GetBytes(eventXml.Replace("CallerLocalEvent","callerLocalEvent"))); File.SetLastWriteTimeUtc(Path.Combine(directory,"event.xml"),eventTime);
        Reject(withEvent.VerifyCurrent); snapshot.VerifyCurrent();
        foreach (string invalid in new[] { eventXml.Replace("RebornAudioRAM","Unknown"),eventXml.Replace("RebornAudioRAM","rebornAudioRAM"),eventXml.Replace("RebornAudioRAM","RebornAudioStream"),
            eventXml.Replace("AudioFile:","AudioEvent:"),eventXml.Replace("RebornAudioRAM","RebornAudioRAM\\0"),eventXml.Replace("Volume=\"60\"","Volume=\"NaN\""),
            eventXml.Replace("INTERRUPT","LOOP"),eventXml.Replace("800","1000001"),eventXml.Replace("CallerLocalEvent","Bad:Name"),
            eventXml.Replace("<AudioEvent","<Includes /><AudioEvent"),eventXml.Replace("<AudioEvent","<AudioEvent inheritFrom=\"Other\""),
            eventXml.Replace("<Sound>","<Sound Volume=\"100\">"),eventXml.Replace("<AssetDeclaration","<!DOCTYPE x [<!ENTITY e 'x'>]><AssetDeclaration"),
            "<?xml version=\"1.0\" encoding=\"utf-16\"?>"+eventXml })
        { Write("event.xml",Encoding.UTF8.GetBytes(invalid)); Reject(() => AuthoredAudioSnapshot.Read(directory,true)); }
        foreach (byte[] invalid in new[] { new byte[8193],new byte[] { 0xFF },new byte[] { 0xEF,0xBB,0xBF }.Concat(Encoding.UTF8.GetBytes(eventXml)).ToArray() })
        { Write("event.xml",invalid); Reject(() => AuthoredAudioSnapshot.Read(directory,true)); }
        Write("event.xml",Encoding.UTF8.GetBytes(eventXml)); withEvent.VerifyCurrent();
        // Reborn: singleton leaves and reversed pairs retain exact caller order, while empty/oversized/duplicate lists remain closed.
        const string ramSound = "<Sound>AudioFile:RebornAudioRAM</Sound>",streamSound = "<Sound Weight=\"800\">AudioFile:RebornAudioStream</Sound>";
        foreach (var list in new[] { (Xml:ramSound,Slots:new[] { 0 }),(Xml:streamSound,Slots:new[] { 1 }),(Xml:streamSound+ramSound,Slots:new[] { 1,0 }) })
        {
            Write("event.xml",Encoding.UTF8.GetBytes(eventXml.Replace(ramSound+streamSound,list.Xml)));
            var selected = AuthoredAudioSnapshot.Read(directory,true);
            if (!selected.EventSettings!.Slots().SequenceEqual(list.Slots)) throw new InvalidDataException("Authored Sound selection/order changed.");
            int[] detached = selected.EventSettings.Slots(); detached[0] ^= 1;
            if (!selected.EventSettings.Slots().SequenceEqual(list.Slots)) throw new InvalidDataException("Caller mutated frozen Sound selection.");
            selected.VerifyCurrent();
        }
        foreach (string bad in new[] { "",ramSound+ramSound,ramSound+streamSound+ramSound,ramSound.Replace("<Sound>","<Sound Weight=\"0\">") })
        { Write("event.xml",Encoding.UTF8.GetBytes(eventXml.Replace(ramSound+streamSound,bad))); Reject(() => AuthoredAudioSnapshot.Read(directory,true)); }
        Write("event.xml",Encoding.UTF8.GetBytes(eventXml)); withEvent.VerifyCurrent();
        // Reborn: exercise exact scalar limits/default attribution and reject nonliteral/overflow or meaningless all-zero mixtures before worker launch.
        foreach (string volume in new[] { "0","37.5","100","0.0000001" })
        {
            string varied = eventXml.Replace("Volume=\"60\"","Volume=\""+volume+"\"").Replace("<Sound>","<Sound Weight=\"0\">").Replace("800","1000000");
            Write("event.xml",Encoding.UTF8.GetBytes(varied)); var admitted = AuthoredAudioSnapshot.Read(directory,true);
            if (admitted.EventSettings!.FirstWeight != 0 || admitted.EventSettings.SecondWeight != 1000000) throw new InvalidDataException("Event weight bounds were lost.");
            admitted.VerifyCurrent();
        }
        foreach (string bad in new[] { "NaN","INF","-1","101","100.0000001","1e2","50%","37,5","=60"," 60",new string('1',17) })
        { Write("event.xml",Encoding.UTF8.GetBytes(eventXml.Replace("Volume=\"60\"","Volume=\""+bad+"\""))); Reject(() => AuthoredAudioSnapshot.Read(directory,true)); }
        foreach (string bad in new[] { "-1","1.5","+1","1e3","=1","4294967296","1000001"," 1","" })
        { Write("event.xml",Encoding.UTF8.GetBytes(eventXml.Replace("800",bad))); Reject(() => AuthoredAudioSnapshot.Read(directory,true)); }
        Write("event.xml",Encoding.UTF8.GetBytes(eventXml.Replace("<Sound>","<Sound Weight=\"0\">").Replace("800","0"))); Reject(() => AuthoredAudioSnapshot.Read(directory,true));
        Write("event.xml",Encoding.UTF8.GetBytes(eventXml.Replace(" Weight=\"800\"","")));
        if (AuthoredAudioSnapshot.Read(directory,true).EventSettings!.SecondWeight != 1000) throw new InvalidDataException("Absent Sound weight did not use the official default.");
        Write("event.xml",Encoding.UTF8.GetBytes(eventXml)); withEvent.VerifyCurrent();
        Reject(() => AudioEncoderSupervisor.Run("unused.dll","encode-authored",authored:withEvent));
        Reject(() => AudioEncoderSupervisor.Run("unused.dll","encode-authored-event",authored:snapshot));
        Console.WriteLine("Authored audio snapshot self-test: OK (bounded XML/PCM/event admission, exact immutable copies, stale timestamp-preserving edits, explicit 3/4-file modes, local reference/settings/DTD/Include/inheritance rejection and recovery; no native codecs)");

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
