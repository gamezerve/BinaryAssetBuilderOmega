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
    internal static string Run(string path,bool useCore = false,AudioEncoderFaultAudit? audit = null,string? ownedDirectory = null,AuthoredAudioSnapshot? authored = null)
    {
        // Reborn: fault injection is restricted to the isolated real-core path, never silently applied to another workflow.
        if (audit != null && !useCore) throw new NotSupportedException("Audio fault injection requires core preparation.");
        // Reborn: authored snapshots are admitted only through isolated current-core encoding, never the legacy authored-only path.
        if (authored != null && (!useCore || audit != null)) throw new NotSupportedException("Authored snapshots require the supervised core path without fault injection.");
        path = Path.GetFullPath(path);
        if (!OperatingSystem.IsWindows() || RuntimeInformation.ProcessArchitecture != Architecture.X86)
            throw new NotSupportedException("Audio encoder PoC requires a Windows x86 worker process.");
        NativeAudioApiProbe.Evidence evidence = NativeAudioApiProbe.Read(path);
        if (evidence.Managed || evidence.Machine != System.Reflection.PortableExecutable.Machine.I386
            || evidence.Magic != System.Reflection.PortableExecutable.PEMagic.PE32
            || Convert.ToHexString(SHA256.HashData(File.ReadAllBytes(path))) != ExpectedHash)
            throw new InvalidDataException("Audio encoder PoC only admits the audited native library.");
        string directory = ownedDirectory ?? Path.Combine(Path.GetTempPath(),"Reborn-AudioEncoder-"+Guid.NewGuid().ToString("N"));
        // Reborn: a supervised worker may receive only a fresh empty owned result directory, never overwrite earlier evidence.
        if (ownedDirectory != null && Directory.Exists(directory) && Directory.EnumerateFileSystemEntries(directory).Any())
            throw new InvalidDataException("Supervised audio directory must be empty.");
        Directory.CreateDirectory(directory); string input = Path.Combine(directory,"input.wav");
        if (authored != null) authored.Install(directory); else WriteWave(input);
        // Reborn: fault injection can access only this newly created worker-owned fixture directory.
        audit?.SetDirectory(directory);
        // Reborn: freeze both authored play locations and owned WAV before native initialization; unsupported XML cannot start encoding.
        XmlElement ramRoot = CreateDefinition(false),streamRoot = CreateDefinition(true);
        InstanceHandle ramId = Identity(ramRoot),streamId = Identity(streamRoot);
        byte[] wave = ReadOwnedWave(input);
        var ramInput = Ra3Ep1AudioFileInputProfile.Prepare(ramRoot,ramId,TargetPlatform.Win32,wave);
        var streamInput = Ra3Ep1AudioFileInputProfile.Prepare(streamRoot,streamId,TargetPlatform.Win32,wave);
        // Reborn: preserve the actual authored definitions as owned provenance, with distinct local AudioFile identities.
        if (authored == null)
        {
            WriteOwned(Path.Combine(directory,"ram.xml"),System.Text.Encoding.UTF8.GetBytes(useCore ? CoreSource(false) : ramRoot.OuterXml));
            WriteOwned(Path.Combine(directory,"streamed.xml"),System.Text.Encoding.UTF8.GetBytes(useCore ? CoreSource(true) : streamRoot.OuterXml));
        }
        // Reborn: optional actual core identities are prepared before native initialization, with no production AudioFile registration.
        InstanceDeclaration? ramCore = null,streamCore = null;
        AudioFileCorePreparation? ramPrepared = null,streamPrepared = null;
        if (useCore)
        {
            string schema = Path.Combine(Path.GetDirectoryName(ReferencePipelineSmokeTest.FindFixture())!,"AudioFileIdentityPipeline.xsd");
            ramCore = AudioFileIdentitySmokeTest.Build(directory,schema,AudioFileIdentitySmokeTest.Processing,"ram.xml");
            streamCore = AudioFileIdentitySmokeTest.Build(directory,schema,AudioFileIdentitySmokeTest.Processing,"streamed.xml");
            ramPrepared = AudioFileCorePreparation.Prepare(ramCore); streamPrepared = AudioFileCorePreparation.Prepare(streamCore);
            ramId = ramCore.Handle; streamId = streamCore.Handle;
            ramInput = ramPrepared.Settings; streamInput = streamPrepared.Settings;
        }
        Console.WriteLine("Owned audio encoder PoC directory: "+directory);
        IntPtr module = NativeLibrary.Load(path);
        bool initialized = false;
        try
        {
            VoidCall init = Bind<VoidCall>(module,"SIMEX_init"),shutdown = Bind<VoidCall>(module,"SIMEX_shutdown");
            init(); initialized = true; audit?.Lifecycle("init");
            AudioFilePackageProbe.Entry[] package;
            try { package = new[] { Encode(module,input,Path.Combine(directory,"ram"),ramRoot,ramId,ramInput,ramCore,ramPrepared,audit),Encode(module,input,Path.Combine(directory,"streamed"),streamRoot,streamId,streamInput,streamCore,streamPrepared,audit) }; }
            finally { shutdown(); initialized = false; audit?.Lifecycle("shutdown"); }
            // Reborn: only fully checked RAM/streamed results can enter a new staged diagnostic package after native shutdown.
            CoreAudioPackageGate.Binding[]? bindings = useCore ? new[] { new CoreAudioPackageGate.Binding(ramCore!,ramPrepared!,package[0]),new CoreAudioPackageGate.Binding(streamCore!,streamPrepared!,package[1]) } : null;
            // Reborn: test actual source changes only after both native results and shutdown, before any package staging.
            audit?.Phase("before-publish");
            string output = Path.Combine(directory,"package");
            if (bindings != null) CoreAudioPackageGate.Publish(output,bindings); else AudioFilePackageProbe.Publish(output,package);
            AudioFilePackageProbe.Verify(output,package);
            Console.WriteLine("Owned diagnostic audio package: "+Path.Combine(output,"diagnostic.manifest"));
            // Reborn: compile a core-normalized local AudioEvent only after capturing both encoded AudioFile fingerprints.
            AudioFileLocalEventProbe.Entry localEvent = AudioFileLocalEventProbe.Build(directory,package,authoredName:authored?.EventName);
            string mixed = Path.Combine(directory,"local-event-package");
            if (bindings != null) CoreAudioPackageGate.Publish(mixed,bindings,localEvent); else AudioFilePackageProbe.Publish(mixed,package,localEvent);
            AudioFilePackageProbe.Verify(mixed,package,localEvent);
            if (bindings != null)
            {
                CoreAudioPackageGate.Verify(bindings);
                WriteOwned(Path.Combine(directory,"core-identities.txt"),System.Text.Encoding.UTF8.GetBytes("Diagnostic evidence only; core hash and package content hash are different domains.\n"+
                    string.Join("\n",bindings.Select(binding => $"{binding.Encoded.Name}: core={binding.Instance.Handle.InstanceHash:X8}, diagnostic-content={binding.Encoded.Hash:X8}"))+"\n"));
                Console.WriteLine("Actual core AudioFile preparation -> native XAS -> checked diagnostic publication: OK (core and diagnostic identities kept separate).");
            }
            Console.WriteLine("Owned local AudioEvent/audio package: "+Path.Combine(mixed,"diagnostic.manifest"));
            Console.WriteLine("Audio encoder PoC: OK; prepared PCM/XAS -> checked runtime/custom data -> two/three-entry local diagnostic packages; no production registration or game-loading proof.");
        }
        finally
        {
            // Reborn: unload only this process's pinned module after native handles have been closed; preserve owned outputs as evidence.
            if (!initialized) { NativeLibrary.Free(module); audit?.Lifecycle("unload"); }
        }
        return directory;
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: encode an explicit frozen pool in a disposable worker, retaining raw per-leaf evidence without generalizing package/event admission. */
    //-------------------------------------------------------------------------------------------------
    internal static void RunPool(string path,string directory,AuthoredAudioPool pool,bool encode)
    {
        pool.Install(directory);
        string schema = Path.Combine(Path.GetDirectoryName(ReferencePipelineSmokeTest.FindFixture())!,"AudioFileIdentityPipeline.xsd");
        var rows = pool.Rows;
        var cores = rows.Select(row => AudioFileIdentitySmokeTest.Build(directory,schema,AudioFileIdentitySmokeTest.Processing,row.Source)).ToArray();
        var preparations = cores.Select(AudioFileCorePreparation.Prepare).ToArray();
        if (encode)
        {
            path = Path.GetFullPath(path);
            if (!OperatingSystem.IsWindows() || RuntimeInformation.ProcessArchitecture != Architecture.X86) throw new NotSupportedException("Pool encoder requires Windows x86.");
            NativeAudioApiProbe.Evidence evidence = NativeAudioApiProbe.Read(path);
            if (evidence.Managed || evidence.Machine != System.Reflection.PortableExecutable.Machine.I386 || evidence.Magic != System.Reflection.PortableExecutable.PEMagic.PE32
                || Convert.ToHexString(SHA256.HashData(File.ReadAllBytes(path))) != ExpectedHash) throw new InvalidDataException("Pool encoder requires the audited library.");
            string output = Path.Combine(directory,"encoded"); Directory.CreateDirectory(output);
            IntPtr module = NativeLibrary.Load(path); bool initialized = false;
            try
            {
                Bind<VoidCall>(module,"SIMEX_init")(); initialized = true;
                try
                {
                    for (int index = 0; index < rows.Length; index++)
                    {
                        pool.VerifyCopies(directory); preparations[index].VerifyCurrent(cores[index]);
                        // Reborn: output prefixes are generated ordinals in a separate directory, never caller IDs or WAV filenames.
                        Encode(module,Path.Combine(directory,rows[index].Wave),Path.Combine(output,"leaf"+index),CreateDefinition(rows[index].Streamed),cores[index].Handle,preparations[index].Settings,cores[index],preparations[index]);
                    }
                }
                finally { Bind<VoidCall>(module,"SIMEX_shutdown")(); initialized = false; }
            }
            finally { if (!initialized) NativeLibrary.Free(module); }
        }
        pool.VerifyCopies(directory); pool.VerifyCurrent();
        var metadata = rows.Select((row,index) => new AuthoredAudioPool.CoreRow(row.Source,row.Name,row.Id,cores[index].Handle.InstanceHash,row.Streamed)).ToArray();
        WriteOwned(Path.Combine(directory,"pool-core.json"),System.Text.Json.JsonSerializer.SerializeToUtf8Bytes(metadata));
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: encode one owned WAV using source-declared cdecl signatures, then independently validate generated framing. */
    //-------------------------------------------------------------------------------------------------
    private static AudioFilePackageProbe.Entry Encode(IntPtr module,string input,string output,XmlElement root,InstanceHandle identity,Ra3Ep1AudioFileInputProfile.PreparedInput prepared,InstanceDeclaration? core = null,AudioFileCorePreparation? corePrepared = null,AudioEncoderFaultAudit? audit = null)
    {
        // Reborn: actual core mode encodes a new frozen PCM snapshot, not a later reopened mutable source dependency.
        if ((core == null) != (corePrepared == null)) throw new InvalidDataException("Core audio encoding requires paired instance/preparation.");
        if (corePrepared != null)
        {
            corePrepared.VerifyCurrent(core!); input = output+".input.wav"; WriteOwned(input,corePrepared.CopyWave());
        }
        using FileStream inputLease = new(input,FileMode.Open,FileAccess.Read,FileShare.Read);
        // Reborn: preparation must still match the actual owned input immediately before starting native work.
        if (corePrepared == null) prepared.VerifyCurrent(root,identity,TargetPlatform.Win32,ReadOwnedWave(input));
        else if (!ReadOwnedWave(input).SequenceEqual(corePrepared.CopyWave())) throw new InvalidDataException("Frozen encoder PCM snapshot differs.");
        bool streamed = prepared.Streamed;
        Identify identify = Bind<Identify>(module,"SIMEX_id"); Open open = Bind<Open>(module,"SIMEX_open"); Create create = Bind<Create>(module,"SIMEX_create");
        Info getInfo = Bind<Info>(module,"SIMEX_info"); Transfer read = Bind<Transfer>(module,"SIMEX_read"),write = Bind<Transfer>(module,"SIMEX_write");
        Getter close = Bind<Getter>(module,"SIMEX_close"),wclose = Bind<Getter>(module,"SIMEX_wclose"),free = Bind<Getter>(module,"SIMEX_freesinfo");
        IntPtr source = IntPtr.Zero,target = IntPtr.Zero,info = IntPtr.Zero;
        try
        {
            if (identify(input,0) != 1) Fail(module,"WAV identify");
            int openStatus = open(input,0,1,out source); if (source != IntPtr.Zero) audit?.Acquired("source");
            if (openStatus != 1 || source == IntPtr.Zero) Fail(module,"WAV open"); audit?.Phase("after-open");
            int infoStatus = getInfo(source,out info,0); if (info != IntPtr.Zero) audit?.Acquired("info");
            if (infoStatus <= 0 || info == IntPtr.Zero || read(source,info,0) <= 0) Fail(module,"WAV info/read"); audit?.Phase("after-info");
            int rate = Bind<Getter>(module,"SIMEX_getsamplerate")(info),samples = Bind<Getter>(module,"SIMEX_getnumsamples")(info),channels = Bind<Getter>(module,"SIMEX_getchannelconfig")(info);
            if (rate != prepared.Rate || samples != prepared.Samples || channels != prepared.Channels) throw new InvalidDataException("Native input getters disagree with the prepared WAV.");
            Bind<Setter>(module,"SIMEX_setcodec")(info,prepared.Codec); Bind<Setter>(module,"SIMEX_setplayloc")(info,streamed ? 4096 : 2048);
            // Reborn: reference AudioCompiler IL_0524 uses SND 39; legacy LAYER3 34 was rejected by this audited native library.
            // Reborn: the audited DLL crashed on a nonexistent output parent; reject unsafe/existing/reparse output paths before native create.
            string nativeOutput = audit?.OutputPath(output) ?? output; ValidateNativeOutputPrefix(nativeOutput);
            int createStatus = create(nativeOutput,prepared.OutputContainer,out target); if (target != IntPtr.Zero) audit?.Acquired("target");
            if (createStatus <= 0 || target == IntPtr.Zero) Fail(module,"encoded create"); audit?.Phase("after-create");
            if (write(target,info,0) <= 0) Fail(module,"encoded write"); audit?.Phase("after-write");
            // Reborn: close exactly once; retain the raw status instead of guessing fclose-style versus SIMEX-style success semantics.
            int closeStatus = Release(ref target,value => wclose(value),"target",audit); Console.WriteLine($"  native output close status={closeStatus}");
        }
        // Reborn: one cleanup failure must not skip remaining resources; detach each pointer before attempting its release once.
        finally { Cleanup(ref target,ref info,ref source,value => wclose(value),value => free(value),value => close(value),audit); }
        audit?.Phase("after-encode");
        string snr = output+".snr",custom = streamed ? output+".sns" : snr;
        if (streamed && new FileInfo(snr).Length != 8) throw new InvalidDataException("Unexpected generated streamed SNR size.");
        byte[] header = streamed ? File.ReadAllBytes(snr) : Array.Empty<byte>();
        if (streamed && header.Length != 8) throw new InvalidDataException("Unexpected generated streamed SNR size.");
        using FileStream encoded = File.OpenRead(custom);
        // Reborn: bind generated custom framing to independently parsed serialized EP1 fields, not hardcoded fake native metadata.
        AssetBuffer runtime = corePrepared != null ? corePrepared.SerializeCurrent(core!,header) : prepared.SerializeCurrent(root,identity,TargetPlatform.Win32,ReadOwnedWave(input),header);
        AudioFileRuntimeProbe.Header parsed = AudioFileRuntimeProbe.Parse(runtime.InstanceData,runtime.InstanceData.Length);
        byte[] inline = parsed.HeaderSize == 0 ? Array.Empty<byte>() : runtime.InstanceData.AsSpan(checked((int)parsed.HeaderPointer),checked((int)parsed.HeaderSize)).ToArray();
        AudioCustomDataProbe.Result framing = AudioCustomDataProbe.Inspect(encoded,parsed,inline);
        // Reborn: this prepared codec 29 experiment admits only the observed tag 04, not merely any structurally valid frame.
        if (framing.CodecTag != 4) throw new InvalidDataException("Prepared XAS custom output has an unproven codec tag.");
        WriteOwned(output+".runtime.bin",runtime.InstanceData); WriteOwned(output+".runtime.relo",runtime.RelocationData);
        Console.WriteLine($"  EP1 raw runtime: BIN={runtime.InstanceData.Length}, RELO={runtime.RelocationData.Length}, IMP={runtime.ImportsData.Length}; no linked manifest/container emitted");
        encoded.Position = 0;
        Console.WriteLine($"  {(streamed ? "streamed" : "RAM")}: header={Convert.ToHexString(header)}, bytes={framing.Bytes}, blocks={framing.Blocks}, tag={framing.CodecTag:X2}, sha256={Convert.ToHexString(SHA256.HashData(encoded))}");
        // Reborn: own a bounded copy of the complete encoded payload; package verification cannot rely on subsequent source-file reads.
        if (encoded.Length > 1048576) throw new InvalidDataException("Owned audio package payload exceeds proof bound.");
        encoded.Position = 0; byte[] payload = new byte[checked((int)encoded.Length)]; encoded.ReadExactly(payload);
        return new AudioFilePackageProbe.Entry(identity.InstanceName,streamed ? "streamed.xml" : "ram.xml",runtime,payload);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: resolve an explicitly named audited export without implicit library search or delegate guessing. */
    //-------------------------------------------------------------------------------------------------
    private static T Bind<T>(IntPtr module,string name) where T : Delegate => Marshal.GetDelegateForFunctionPointer<T>(NativeLibrary.GetExport(module,name));

    //-------------------------------------------------------------------------------------------------
    /** Reborn: detach a nonzero handle before exactly one release attempt and record only calls that actually return. */
    //-------------------------------------------------------------------------------------------------
    private static int Release(ref IntPtr handle,Func<IntPtr,int> release,string kind,AudioEncoderFaultAudit? audit)
    {
        if (handle == IntPtr.Zero) return 0;
        IntPtr owned = handle; handle = IntPtr.Zero; int status = release(owned); audit?.Released(kind,status); return status;
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: independent finally paths attempt every remaining resource even if an earlier release throws; managed tests use no native handles. */
    //-------------------------------------------------------------------------------------------------
    internal static void Cleanup(ref IntPtr target,ref IntPtr info,ref IntPtr source,Func<IntPtr,int> closeTarget,Func<IntPtr,int> freeInfo,Func<IntPtr,int> closeSource,AudioEncoderFaultAudit? audit = null)
    {
        try { Release(ref target,closeTarget,"target",audit); }
        finally { try { Release(ref info,freeInfo,"info",audit); } finally { Release(ref source,closeSource,"source",audit); } }
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: reject known-unsafe native output conditions before SIMEX_create; other native failures still require worker isolation. */
    //-------------------------------------------------------------------------------------------------
    internal static void ValidateNativeOutputPrefix(string output)
    {
        if (!Path.IsPathFullyQualified(output) || !Directory.Exists(Path.GetDirectoryName(output)))
            throw new InvalidDataException("Native audio output requires an existing absolute parent; invalid-parent calls are unsafe.");
        string leaf = Path.GetFileName(output);
        if (leaf.Length is < 1 or > 128 || leaf.Contains("..",StringComparison.Ordinal)
            || leaf.Any(value => !(value is >= 'a' and <= 'z' or >= 'A' and <= 'Z' or >= '0' and <= '9' or '_' or '-')))
            throw new InvalidDataException("Native audio output requires a bounded simple prefix.");
        for (string? directory = Path.GetDirectoryName(output); directory != null; directory = Path.GetDirectoryName(directory))
            if ((File.GetAttributes(directory) & FileAttributes.ReparsePoint) != 0) throw new InvalidDataException("Native output does not admit reparse ancestry.");
        foreach (string candidate in new[] { output,output+".snr",output+".sns" })
            if (File.Exists(candidate) || Directory.Exists(candidate)) throw new InvalidDataException("Native audio output must not replace an existing artifact.");
    }

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
        document.LoadXml($"<AudioFile xmlns=\"uri:ea.com:eala:asset\" id=\"{(streamed ? "RebornAudioStream" : "RebornAudioRAM")}\" File=\"input.wav\" PCSampleRate=\"48000\" PCCompression=\"XAS\" IsStreamedOnPC=\"{(streamed ? "true" : "false")}\" SubtitleStringName=\"DIALOGEVENT:reborn_audio_encoder_pocSubTitle\" />");
        document.Validate((_, args) => throw new XmlSchemaValidationException(args.Message)); return document.DocumentElement!;
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: author one fixed core declaration without serializing inserted defaults as explicitly authored cross-platform settings. */
    //-------------------------------------------------------------------------------------------------
    internal static string CoreSource(bool streamed) => $"<AssetDeclaration xmlns=\"uri:ea.com:eala:asset\"><AudioFile id=\"{(streamed ? "RebornAudioStream" : "RebornAudioRAM")}\" File=\"input.wav\" PCSampleRate=\"48000\" PCCompression=\"XAS\" IsStreamedOnPC=\"{(streamed ? "true" : "false")}\" SubtitleStringName=\"DIALOGEVENT:reborn_audio_encoder_pocSubTitle\" /></AssetDeclaration>";

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
