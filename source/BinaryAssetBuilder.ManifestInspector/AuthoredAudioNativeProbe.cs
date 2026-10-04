using System.Security.Cryptography;
using System.Text;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: opt-in proof of caller-owned nonfixture PCM/subtitle snapshots and stale-original rejection, with native work confined to children.
internal static class AuthoredAudioNativeProbe
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: prepare changed authored content, verify accepted native output uses it, and reject a later caller-source change without modifying real user files. */
    //-------------------------------------------------------------------------------------------------
    internal static void Run(string library,bool includeEvent = false,string selection = "pair",string controls = "INTERRUPT",uint expectedControl = 8)
    {
        // Reborn: test choices must match the independently supplied expected word before creating any native job.
        if (AuthoredAudioEventSource.ReadControls(controls) != expectedControl || (!includeEvent && (selection != "pair" || controls != "INTERRUPT")))
            throw new InvalidDataException("Native control proof selection/expected word differs.");
        string source = Path.Combine(Path.GetTempPath(),"Reborn-AuthoredNative-"+Guid.NewGuid().ToString("N")); Directory.CreateDirectory(source);
        using MemoryStream pcm = new(); AudioEncoderPoc.WriteWave(pcm); byte[] wave = pcm.ToArray(); wave.AsSpan(44).Clear();
        foreach (bool streamed in new[] { false,true })
            Write(Path.Combine(source,streamed ? "streamed.xml" : "ram.xml"),Encoding.UTF8.GetBytes(AudioEncoderPoc.CoreSource(streamed)
                .Replace(streamed ? "RebornAudioStream" : "RebornAudioRAM",streamed ? "Caller_Stream-02" : "Caller_RAM-01",StringComparison.Ordinal)
                .Replace("reborn_audio_encoder_pocSubTitle","authored_snapshotSubTitle",StringComparison.Ordinal)),true);
        string file = Path.Combine(source,"input.wav"); Write(file,wave,true);
        // Reborn: optional caller event is a fourth immutable input with custom identity and exact ordered references.
        string eventXml = AudioFileLocalEventProbe.SourceXml.Replace("RebornLocalAudio","CallerLocalEvent").Replace("RebornAudioRAM","Caller_RAM-01").Replace("RebornAudioStream","Caller_Stream-02")
            .Replace("Volume=\"60\"","Volume=\"37.5\"").Replace("<Sound>","<Sound Weight=\"125\">").Replace("800","875")
            .Replace("Control=\"INTERRUPT\"","Control=\""+controls+"\"");
        // Reborn: native list proofs choose source order independently of compiler metadata, using only owned fixtures.
        int[] selectedSlots = selection switch { "pair" => new[] { 0,1 },"ram" => new[] { 0 },"streamed" => new[] { 1 },"reversed" => new[] { 1,0 },_ => throw new ArgumentException("Unknown native list proof selection.") };
        const string ramSound = "<Sound Weight=\"125\">AudioFile:Caller_RAM-01</Sound>",streamSound = "<Sound Weight=\"875\">AudioFile:Caller_Stream-02</Sound>";
        eventXml = eventXml.Replace(ramSound+streamSound,string.Concat(selectedSlots.Select(slot => slot == 0 ? ramSound : streamSound)));
        byte[] eventBytes = Encoding.UTF8.GetBytes(eventXml);
        if (includeEvent) Write(Path.Combine(source,"event.xml"),eventBytes,true);
        Console.WriteLine("Owned authored audio input fixture: "+source);
        var frozen = AuthoredAudioSnapshot.Read(source,includeEvent);
        string mode = includeEvent ? "encode-authored-event" : "encode-authored";
        var hashes = frozen.FileNames.ToDictionary(name => name,name => Convert.ToHexString(SHA256.HashData(File.ReadAllBytes(Path.Combine(source,name)))));
        string accepted = AudioEncoderSupervisor.Run(library,mode,authored:frozen);
        // Reborn: independently verify caller names/hashed IDs and the event dependency tuples in accepted native packages.
        var manifest = ManifestReader.Read(File.ReadAllBytes(Path.Combine(accepted,"worker","local-event-package","diagnostic.manifest")));
        if (manifest.Assets[0].Name != "AudioFile:Caller_RAM-01" || manifest.Assets[1].Name != "AudioFile:Caller_Stream-02"
            || manifest.Assets[0].InstanceId != BinaryAssetBuilder.Core.InstanceHandle.GetInstanceId(frozen.RamName)
            || manifest.Assets[1].InstanceId != BinaryAssetBuilder.Core.InstanceHandle.GetInstanceId(frozen.StreamName)
            || !manifest.Assets[2].References.SequenceEqual(selectedSlots.Select(slot => new AssetId(manifest.Assets[slot].TypeId,manifest.Assets[slot].InstanceId))))
            throw new InvalidDataException("Native package lost authored names or local dependency identities.");
        if (includeEvent && manifest.Assets[2].Name != "AudioEvent:CallerLocalEvent") throw new InvalidDataException("Caller event identity was silently replaced.");
        if (includeEvent)
        {
            // Reborn: inspect native words directly, not the profile object or compiler's normalized XML.
            byte[] bin = File.ReadAllBytes(Path.Combine(accepted,"worker","local-event-package","diagnostic.bin"));
            int start = 8+manifest.Assets.Take(2).Sum(asset => asset.InstanceDataSize);
            if (System.Buffers.Binary.BinaryPrimitives.ReadUInt32LittleEndian(bin.AsSpan(start+4)) != 0x3EC00000u
                || System.Buffers.Binary.BinaryPrimitives.ReadUInt32LittleEndian(bin.AsSpan(start+44)) != expectedControl
                || manifest.Assets[2].InstanceDataSize != 152+12*selectedSlots.Length || manifest.Assets[2].ImportsDataSize != 4*(selectedSlots.Length+1))
                throw new InvalidDataException("Native event lost caller volume/weights.");
            for (int index = 0; index < selectedSlots.Length; index++)
                if (System.Buffers.Binary.BinaryPrimitives.ReadUInt32LittleEndian(bin.AsSpan(start+152+12*index)) != index+1
                    || System.Buffers.Binary.BinaryPrimitives.ReadUInt32LittleEndian(bin.AsSpan(start+156+12*index)) != (selectedSlots[index] == 0 ? 125u : 875u))
                    throw new InvalidDataException("Native event lost caller list order/selector/weight.");
        }
        foreach (string name in frozen.FileNames)
            if (hashes[name] != Convert.ToHexString(SHA256.HashData(File.ReadAllBytes(Path.Combine(source,name))))) throw new InvalidDataException("Authored source was modified by encoding.");
        byte[] runtime = File.ReadAllBytes(Path.Combine(accepted,"worker","ram.runtime.bin")); var parsed = AudioFileRuntimeProbe.Parse(runtime,runtime.Length);
        string subtitle = Encoding.ASCII.GetString(runtime,32,checked((int)parsed.SubtitleLength));
        if (subtitle != "DIALOGEVENT:authored_snapshotSubTitle") throw new InvalidDataException("Native runtime lost caller subtitle.");
        string payloadHash = Convert.ToHexString(SHA256.HashData(File.ReadAllBytes(Path.Combine(accepted,"worker","ram.snr"))));
        if (payloadHash == "78EB78241914FB7F27512F8FC339FD250A90C1C4FD823E60E0C93C833141A9AD") throw new InvalidDataException("Changed silent PCM unexpectedly reproduced the old tone payload.");
        // Reborn: exercise event-source staleness in event mode, retaining the legacy WAV-staleness proof in three-file mode.
        if (includeEvent) file = Path.Combine(source,"event.xml");
        byte[] original = includeEvent ? eventBytes : wave;
        string? rejectedJob = null; bool rejected = false; DateTime timestamp = File.GetLastWriteTimeUtc(file);
        try
        {
            AudioEncoderSupervisor.Run(library,mode,jobCreated:job =>
            {
                // Reborn: after the parent's frozen input copy is installed, change only this owned source with the same size/timestamp.
                rejectedJob = job; byte[] changed = includeEvent ? Encoding.UTF8.GetBytes(Encoding.UTF8.GetString(eventBytes).Replace("CallerLocalEvent","callerLocalEvent")) : (byte[])wave.Clone();
                if (!includeEvent) changed[100] ^= 1; Write(file,changed,false); File.SetLastWriteTimeUtc(file,timestamp);
            },authored:frozen);
        }
        catch (InvalidDataException exception) when (exception.Message.Contains("snapshot is stale",StringComparison.Ordinal)) { rejected = true; }
        finally { Write(file,original,false); File.SetLastWriteTimeUtc(file,timestamp); }
        if (!rejected || rejectedJob == null || File.Exists(Path.Combine(rejectedJob,"ACCEPTED.json"))) throw new InvalidDataException("Stale caller source was accepted.");
        frozen.VerifyCurrent();
        // Reborn: corrupt only a new job input copy; the child must reject request/inventory mismatch before reaching codec startup.
        string? tamperedJob = null; bool tamperedRejected = false;
        try
        {
            AudioEncoderSupervisor.Run(library,mode,jobCreated:job =>
            {
                tamperedJob = job; string copied = Path.Combine(job,"inputs",includeEvent ? "event.xml" : "input.wav");
                using FileStream stream = new(copied,FileMode.Open,FileAccess.ReadWrite,FileShare.None); stream.Position = 100; int value = stream.ReadByte(); stream.Position = 100; stream.WriteByte((byte)(value^1));
            },authored:frozen);
        }
        catch (InvalidDataException exception) when (exception.Message.Contains("exit=1",StringComparison.Ordinal)) { tamperedRejected = true; }
        if (!tamperedRejected || tamperedJob == null || File.Exists(Path.Combine(tamperedJob,"ACCEPTED.json"))
            || !File.ReadAllText(Path.Combine(tamperedJob,"worker.stderr.txt")).Contains("Worker authored snapshot inventory differs",StringComparison.Ordinal))
            throw new InvalidDataException("Tampered authored input was not rejected at the intended child inventory gate.");
        Console.WriteLine($"Authored native snapshot proof: OK (mode={mode}, selection={selection}, control={expectedControl:X}, silence PCM, caller subtitle/identities, preserved originals, payload={payloadHash}; stale caller rejects acceptance, no production admission).");
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: write only fresh owned fixtures or restore this test's exact owned source after injected edits. */
    //-------------------------------------------------------------------------------------------------
    private static void Write(string path,byte[] bytes,bool create)
    { using FileStream stream = new(path,create ? FileMode.CreateNew : FileMode.Create,FileAccess.Write); stream.Write(bytes); }
}
