using System.Runtime.InteropServices;
using System.Security.Cryptography;
// Reborn: optional owned encoding evidence now carries the checked EP1 envelope, never legacy audio output.
using BinaryAssetBuilder.Core;
using BinaryAssetBuilder.XmlCompiler;
// Reborn: schema-bound authored settings are prepared before invoking the native encoder.
using System.Xml;
using System.Xml.Schema;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: opt-in standalone-process encoding experiment; never register an AudioFile processor or touch game/source assets.
internal static class AudioEncoderPoc
{
    // Reborn: use only the audited repository library, not a search-path DLL or an unverified substitute.
    private const string ExpectedHash = "149DE43E1E7C914B8E44DD5E0CDCBED45DE33890EBD2708C8223B278874A610F";
    // Reborn: this subset follows repository-declared cdecl signatures; native status returns remain explicit signed integers.
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)] private delegate void VoidCall();
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)] private delegate int Identify([MarshalAs(UnmanagedType.LPStr)] string file,long offset);
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)] private delegate int Open([MarshalAs(UnmanagedType.LPStr)] string file,long offset,int type,out IntPtr instance);
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)] private delegate int Create([MarshalAs(UnmanagedType.LPStr)] string file,int type,out IntPtr instance);
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)] private delegate int Info(IntPtr instance,out IntPtr info,int element);
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)] private delegate int Transfer(IntPtr instance,IntPtr info,int element);
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)] private delegate int Getter(IntPtr value);
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)] private delegate void Setter(IntPtr info,int value);
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)] private delegate IntPtr Error();

    //-------------------------------------------------------------------------------------------------
    /** Reborn: pin architecture/hash/exports before invoking native code in an explicitly launched disposable CLI process. */
    //-------------------------------------------------------------------------------------------------
    internal static void Run(string path)
    {
        path = Path.GetFullPath(path);
        if (!OperatingSystem.IsWindows() || RuntimeInformation.ProcessArchitecture != Architecture.X86)
            throw new NotSupportedException("Audio encoder PoC requires a Windows x86 worker process.");
        NativeAudioApiProbe.Evidence evidence = NativeAudioApiProbe.Read(path);
        if (evidence.Managed || evidence.Machine != System.Reflection.PortableExecutable.Machine.I386
            || evidence.Magic != System.Reflection.PortableExecutable.PEMagic.PE32
            || Convert.ToHexString(SHA256.HashData(File.ReadAllBytes(path))) != ExpectedHash)
            throw new InvalidDataException("Audio encoder PoC only admits the audited native library.");
        string directory = Path.Combine(Path.GetTempPath(),"Reborn-AudioEncoder-"+Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(directory); string input = Path.Combine(directory,"input.wav"); WriteWave(input);
        // Reborn: freeze both authored play locations and owned WAV before native initialization; unsupported XML cannot start encoding.
        XmlElement ramRoot = CreateDefinition(false),streamRoot = CreateDefinition(true);
        InstanceHandle ramId = Identity(ramRoot),streamId = Identity(streamRoot);
        byte[] wave = ReadOwnedWave(input);
        var ramInput = Ra3Ep1AudioFileInputProfile.Prepare(ramRoot,ramId,TargetPlatform.Win32,wave);
        var streamInput = Ra3Ep1AudioFileInputProfile.Prepare(streamRoot,streamId,TargetPlatform.Win32,wave);
        Console.WriteLine("Owned audio encoder PoC directory: "+directory);
        IntPtr module = NativeLibrary.Load(path);
        bool initialized = false;
        try
        {
            VoidCall init = Bind<VoidCall>(module,"SIMEX_init"),shutdown = Bind<VoidCall>(module,"SIMEX_shutdown");
            init(); initialized = true;
            try { Encode(module,input,Path.Combine(directory,"ram"),ramRoot,ramId,ramInput); Encode(module,input,Path.Combine(directory,"streamed"),streamRoot,streamId,streamInput); }
            finally { shutdown(); initialized = false; }
            Console.WriteLine("Audio encoder PoC: OK; owned mono 48 kHz PCM WAV -> codec 29 RAM/streamed framing with checked EP1 runtime envelopes; no production registration or game-loading proof.");
        }
        finally
        {
            // Reborn: unload only this process's pinned module after native handles have been closed; preserve owned outputs as evidence.
            if (!initialized) NativeLibrary.Free(module);
        }
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: encode one owned WAV using source-declared cdecl signatures, then independently validate generated framing. */
    //-------------------------------------------------------------------------------------------------
    private static void Encode(IntPtr module,string input,string output,XmlElement root,InstanceHandle identity,Ra3Ep1AudioFileInputProfile.PreparedInput prepared)
    {
        // Reborn: preparation must still match the actual owned input immediately before starting native work.
        prepared.VerifyCurrent(root,identity,TargetPlatform.Win32,ReadOwnedWave(input)); bool streamed = prepared.Streamed;
        Identify identify = Bind<Identify>(module,"SIMEX_id"); Open open = Bind<Open>(module,"SIMEX_open"); Create create = Bind<Create>(module,"SIMEX_create");
        Info getInfo = Bind<Info>(module,"SIMEX_info"); Transfer read = Bind<Transfer>(module,"SIMEX_read"),write = Bind<Transfer>(module,"SIMEX_write");
        Getter close = Bind<Getter>(module,"SIMEX_close"),wclose = Bind<Getter>(module,"SIMEX_wclose"),free = Bind<Getter>(module,"SIMEX_freesinfo");
        IntPtr source = IntPtr.Zero,target = IntPtr.Zero,info = IntPtr.Zero;
        try
        {
            if (identify(input,0) != 1 || open(input,0,1,out source) != 1 || source == IntPtr.Zero) Fail(module,"WAV open");
            if (getInfo(source,out info,0) <= 0 || info == IntPtr.Zero || read(source,info,0) <= 0) Fail(module,"WAV info/read");
            int rate = Bind<Getter>(module,"SIMEX_getsamplerate")(info),samples = Bind<Getter>(module,"SIMEX_getnumsamples")(info),channels = Bind<Getter>(module,"SIMEX_getchannelconfig")(info);
            if (rate != prepared.Rate || samples != prepared.Samples || channels != prepared.Channels) throw new InvalidDataException("Native input getters disagree with the prepared WAV.");
            Bind<Setter>(module,"SIMEX_setcodec")(info,prepared.Codec); Bind<Setter>(module,"SIMEX_setplayloc")(info,streamed ? 4096 : 2048);
            // Reborn: reference AudioCompiler IL_0524 uses SND 39; legacy LAYER3 34 was rejected by this audited native library.
            if (create(output,prepared.OutputContainer,out target) <= 0 || target == IntPtr.Zero || write(target,info,0) <= 0) Fail(module,"encoded create/write");
            // Reborn: close exactly once; retain the raw status instead of guessing fclose-style versus SIMEX-style success semantics.
            int closeStatus = wclose(target); target = IntPtr.Zero; Console.WriteLine($"  native output close status={closeStatus}");
        }
        finally { if (target != IntPtr.Zero) wclose(target); if (info != IntPtr.Zero) free(info); if (source != IntPtr.Zero) close(source); }
        string snr = output+".snr",custom = streamed ? output+".sns" : snr;
        if (streamed && new FileInfo(snr).Length != 8) throw new InvalidDataException("Unexpected generated streamed SNR size.");
        byte[] header = streamed ? File.ReadAllBytes(snr) : Array.Empty<byte>();
        if (streamed && header.Length != 8) throw new InvalidDataException("Unexpected generated streamed SNR size.");
        using FileStream encoded = File.OpenRead(custom);
        // Reborn: bind generated custom framing to independently parsed serialized EP1 fields, not hardcoded fake native metadata.
        AssetBuffer runtime = prepared.SerializeCurrent(root,identity,TargetPlatform.Win32,ReadOwnedWave(input),header);
        AudioFileRuntimeProbe.Header parsed = AudioFileRuntimeProbe.Parse(runtime.InstanceData,runtime.InstanceData.Length);
        byte[] inline = parsed.HeaderSize == 0 ? Array.Empty<byte>() : runtime.InstanceData.AsSpan(checked((int)parsed.HeaderPointer),checked((int)parsed.HeaderSize)).ToArray();
        AudioCustomDataProbe.Result framing = AudioCustomDataProbe.Inspect(encoded,parsed,inline);
        // Reborn: this prepared codec 29 experiment admits only the observed tag 04, not merely any structurally valid frame.
        if (framing.CodecTag != 4) throw new InvalidDataException("Prepared XAS custom output has an unproven codec tag.");
        WriteOwned(output+".runtime.bin",runtime.InstanceData); WriteOwned(output+".runtime.relo",runtime.RelocationData);
        Console.WriteLine($"  EP1 raw runtime: BIN={runtime.InstanceData.Length}, RELO={runtime.RelocationData.Length}, IMP={runtime.ImportsData.Length}; no linked manifest/container emitted");
        encoded.Position = 0;
        Console.WriteLine($"  {(streamed ? "streamed" : "RAM")}: header={Convert.ToHexString(header)}, bytes={framing.Bytes}, blocks={framing.Blocks}, tag={framing.CodecTag:X2}, sha256={Convert.ToHexString(SHA256.HashData(encoded))}");
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: resolve an explicitly named audited export without implicit library search or delegate guessing. */
    //-------------------------------------------------------------------------------------------------
    private static T Bind<T>(IntPtr module,string name) where T : Delegate => Marshal.GetDelegateForFunctionPointer<T>(NativeLibrary.GetExport(module,name));

    //-------------------------------------------------------------------------------------------------
    /** Reborn: report native errors and abort the experiment instead of publishing partial build output. */
    //-------------------------------------------------------------------------------------------------
    private static void Fail(IntPtr module,string operation) => throw new InvalidDataException(operation+": "+Marshal.PtrToStringAnsi(Bind<Error>(module,"SIMEX_getlasterr")()));

    //-------------------------------------------------------------------------------------------------
    /** Reborn: generate a fixed owned 250 ms mono 48 kHz 16-bit PCM tone, never overwrite any existing input. */
    //-------------------------------------------------------------------------------------------------
    private static void WriteWave(string path)
    {
        using FileStream stream = new(path,FileMode.CreateNew,FileAccess.Write); WriteWave(stream);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: preserve only raw native evidence in the fresh owned PoC directory; never overwrite existing artifacts. */
    //-------------------------------------------------------------------------------------------------
    private static void WriteOwned(string path,byte[] bytes)
    {
        using FileStream stream = new(path,FileMode.CreateNew,FileAccess.Write); stream.Write(bytes);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: serialize the fixed PoC waveform to an owned stream while retaining caller ownership. */
    //-------------------------------------------------------------------------------------------------
    internal static void WriteWave(Stream stream)
    {
        using BinaryWriter writer = new(stream,System.Text.Encoding.ASCII,true);
        writer.Write(System.Text.Encoding.ASCII.GetBytes("RIFF")); writer.Write(24036); writer.Write(System.Text.Encoding.ASCII.GetBytes("WAVEfmt "));
        writer.Write(16); writer.Write((short)1); writer.Write((short)1); writer.Write(48000); writer.Write(96000); writer.Write((short)2); writer.Write((short)16);
        writer.Write(System.Text.Encoding.ASCII.GetBytes("data")); writer.Write(24000);
        for (int sample = 0; sample < 12000; sample++) writer.Write((short)(Math.Sin(2*Math.PI*440*sample/48000)*8192));
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: validate the generated PCM fixture without writing files or executing the native library. */
    //-------------------------------------------------------------------------------------------------
    internal static void SelfTest()
    {
        using MemoryStream stream = new(); WriteWave(stream); byte[] bytes = stream.ToArray();
        byte[] header = Convert.FromHexString("52494646E45D000057415645666D7420100000000100010080BB0000007701000200100064617461C05D0000");
        if (bytes.Length != 24044 || !bytes.AsSpan(0,44).SequenceEqual(header)
            || BitConverter.ToInt16(bytes,44) != 0 || BitConverter.ToInt16(bytes,46) != 471)
            throw new InvalidDataException("Owned encoder WAV header/sample golden differs.");
        Console.WriteLine("Audio encoder WAV self-test: OK (24,044 bytes, PCM16 mono 48 kHz/12,000 samples, header/first samples; no native codec calls)");
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: validate generated authored definitions against unchanged EP1 AudioFile types, including official defaults. */
    //-------------------------------------------------------------------------------------------------
    internal static XmlElement CreateDefinition(bool streamed)
    {
        string fixture = Path.Combine(Path.GetDirectoryName(ReferencePipelineSmokeTest.FindFixture())!,"AudioFilePipeline.xsd");
        XmlDocument document = new() { XmlResolver = null };
        // Reborn: resolve only the checked-in harness's official relative schema includes; authored XML still has no resolver.
        document.Schemas.XmlResolver = new XmlUrlResolver();
        document.Schemas.Add("uri:ea.com:eala:asset",fixture);
        document.LoadXml($"<AudioFile xmlns=\"uri:ea.com:eala:asset\" id=\"RebornAudioInput\" File=\"input.wav\" PCSampleRate=\"48000\" PCCompression=\"XAS\" IsStreamedOnPC=\"{(streamed ? "true" : "false")}\" SubtitleStringName=\"DIALOGEVENT:reborn_audio_encoder_pocSubTitle\" />");
        document.Validate((_, args) => throw new XmlSchemaValidationException(args.Message)); return document.DocumentElement!;
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: bind the authored asset to recovered EP1 metadata without registering it in a production type table. */
    //-------------------------------------------------------------------------------------------------
    internal static InstanceHandle Identity(XmlElement root) => new("AudioFile",root.GetAttribute("id")) { TypeHash = 0x53C81E47u };

    //-------------------------------------------------------------------------------------------------
    /** Reborn: bound repeated owned-input reads before checking the frozen PCM snapshot. */
    //-------------------------------------------------------------------------------------------------
    private static byte[] ReadOwnedWave(string path)
    {
        using FileStream source = File.OpenRead(path);
        if (source.Length != 24044) throw new InvalidDataException("Owned WAV size changed after preparation.");
        byte[] bytes = new byte[24044]; source.ReadExactly(bytes);
        if (source.ReadByte() != -1) throw new InvalidDataException("Owned WAV grew during bounded input read.");
        return bytes;
    }
}
