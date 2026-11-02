using System.Buffers.Binary;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Xml;
using BinaryAssetBuilder.Core;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: captured WAV resources use the real EA PC encoder and EP1 runtime/CDATAs through the ordinary mod build.
internal static class Ep1AudioFileCompiler
{
    private const string LibraryHash="149DE43E1E7C914B8E44DD5E0CDCBED45DE33890EBD2708C8223B278874A610F";
    internal sealed record Wave(int Samples,int Rate,int Channels);
    internal sealed record Request(string Name,string Input,int Codec,int Rate,int Quality,bool Streamed);
    internal sealed record Encoded(string Name,string Payload,int Samples,int Rate,byte Channels,byte[] Header);
    internal sealed record Result(AssetBuffer Native,byte[] Custom);
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)] private delegate void VoidCall();
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)] private delegate int Identify([MarshalAs(UnmanagedType.LPStr)]string path,long offset);
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)] private delegate int Open([MarshalAs(UnmanagedType.LPStr)]string path,long offset,int type,out IntPtr handle);
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)] private delegate int Create([MarshalAs(UnmanagedType.LPStr)]string path,int type,out IntPtr handle);
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)] private delegate int Info(IntPtr handle,out IntPtr info,int index);
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)] private delegate int Transfer(IntPtr handle,IntPtr info,int index);
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)] private delegate int Getter(IntPtr handle);
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)] private delegate void Setter(IntPtr info,int value);

    //-------------------------------------------------------------------------------------------------
    /** Reborn: parse actual RIFF chunk bounds and PCM metadata, accepting ancillary WAV chunks without a short-duration fixture limit. */
    //-------------------------------------------------------------------------------------------------
    internal static Wave InspectWave(byte[] bytes)
    {
        if(bytes.Length<12||Encoding.ASCII.GetString(bytes,0,4)!="RIFF"||Encoding.ASCII.GetString(bytes,8,4)!="WAVE")throw new InvalidDataException("AudioFile requires a RIFF/WAVE resource.");
        uint U(int at)=>BinaryPrimitives.ReadUInt32LittleEndian(bytes.AsSpan(at,4));ushort S(int at)=>BinaryPrimitives.ReadUInt16LittleEndian(bytes.AsSpan(at,2));
        if(U(4)+8L!=bytes.Length)throw new InvalidDataException("WAV RIFF length differs from its file size.");
        int channels=0,rate=0,align=0,data=-1;
        for(int at=12;at<bytes.Length;){
            if(at>bytes.Length-8)throw new InvalidDataException("Truncated WAV chunk.");int size=checked((int)U(at+4)),p=at+8;if(size>bytes.Length-p)throw new InvalidDataException("WAV chunk exceeds its resource.");string name=Encoding.ASCII.GetString(bytes,at,4);
            if(name=="fmt "){
                if(size<16||S(p)!=1||S(p+14)!=16)throw new InvalidDataException("AudioFile WAV currently requires PCM16 samples.");
                channels=S(p+2);rate=checked((int)U(p+4));align=S(p+12);
                if(channels is not (1 or 2 or 4 or 6)||rate<400||rate>65535||align!=channels*2||U(p+8)!=(uint)(rate*align))throw new InvalidDataException("WAV channel/rate/block alignment is incompatible with the PC sound header.");
            }else if(name=="data"){if(data>=0)throw new InvalidDataException("Multiple WAV data chunks are not supported.");data=size;}
            at=checked(p+size+(size&1));if(at>bytes.Length)throw new InvalidDataException("WAV chunk padding is truncated.");
        }
        if(align==0||data<=0||data%align!=0)throw new InvalidDataException("WAV requires complete nonempty PCM frames.");return new(data/align,rate,channels);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: freeze original path-dependent EA defaults before rewriting File to its content-addressed private WAV snapshot. */
    //-------------------------------------------------------------------------------------------------
    internal static void Prepare(XmlElement source,byte[] wave,string originalPath)
    {
        var pcm=InspectWave(wave);bool sound=Path.GetFileName(Path.GetDirectoryName(originalPath)!).Equals("Sounds",StringComparison.OrdinalIgnoreCase);
        if(!source.HasAttribute("PCCompression"))source.SetAttribute("PCCompression",sound?"XAS":"NONE");
        if(!source.HasAttribute("IsStreamedOnPC"))source.SetAttribute("IsStreamedOnPC",sound?"false":"true");
        if(!source.HasAttribute("SubtitleStringName"))source.SetAttribute("SubtitleStringName","DIALOGEVENT:"+Path.GetFileNameWithoutExtension(originalPath)+"SubTitle");
        if(source.GetAttribute("SubtitleStringName").Any(c=>c>127||char.IsControl(c)))throw new InvalidDataException("AudioFile subtitle keys currently require printable ASCII.");
        Settings(source,pcm);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: batch all current WAV snapshots in one disposable x86 worker; a codec crash cannot corrupt the compiler process. */
    //-------------------------------------------------------------------------------------------------
    internal static Dictionary<string,Result> Encode(XmlDocument[] sources,string entry)
    {
        var files=sources.Where(s=>s.DocumentElement!.LocalName=="AudioFile").ToArray();var results=new Dictionary<string,Result>(StringComparer.OrdinalIgnoreCase);if(files.Length==0)return results;
        string job=Path.Combine(Path.GetTempPath(),"RebornAudio-"+Guid.NewGuid().ToString("N"));Directory.CreateDirectory(job);var requests=new List<Request>();var waves=new List<Wave>();
        try{
            for(int i=0;i<files.Length;i++){
                var source=files[i].DocumentElement!;string file=source.GetAttribute("File");
                if(!System.Text.RegularExpressions.Regex.IsMatch(file,@"^resources/[0-9A-Fa-f]{64}\.wav$"))throw SdkXmlDiagnostics.At(new InvalidDataException("AudioFile requires its captured WAV resource."),source.GetAttributeNode("File"));
                var bytes=SdkEnvironmentPreflight.Read(Path.Combine(Path.GetDirectoryName(entry)!,file.Replace('/',Path.DirectorySeparatorChar)),64*1048576);
                if(!Path.GetFileNameWithoutExtension(file).Equals(Convert.ToHexString(SHA256.HashData(bytes)),StringComparison.OrdinalIgnoreCase))throw new InvalidDataException("Captured WAV hash differs: "+file);
                var pcm=InspectWave(bytes);waves.Add(pcm);var settings=Settings(source,pcm);string input=i.ToString("D6")+".wav";File.WriteAllBytes(Path.Combine(job,input),bytes);requests.Add(new(source.GetAttribute("id"),input,settings.Codec,settings.Rate,settings.Quality,settings.Streamed));
            }
            File.WriteAllText(Path.Combine(job,"request.json"),JsonSerializer.Serialize(requests));
            var start=new ProcessStartInfo(Environment.ProcessPath!){UseShellExecute=false,CreateNoWindow=true,RedirectStandardOutput=true,RedirectStandardError=true};start.ArgumentList.Add("audio-file-encode-worker");start.ArgumentList.Add(job);
            Console.WriteLine("Encoding AudioFile resources: "+files.Length+" WAV file(s).");using var process=Process.Start(start)??throw new InvalidDataException("Audio encoder worker could not start.");var stdout=process.StandardOutput.ReadToEndAsync();var stderr=process.StandardError.ReadToEndAsync();
            int timeout=checked((int)Math.Min(300000,30000+waves.Sum(w=>(double)w.Samples/w.Rate*w.Channels)*1000));
            if(!process.WaitForExit(timeout)){process.Kill(true);process.WaitForExit();throw new InvalidDataException("Audio encoder worker timed out.");}
            if(process.ExitCode!=0)throw new InvalidDataException("Audio encoder worker failed ("+process.ExitCode+"): "+stderr.GetAwaiter().GetResult()+stdout.GetAwaiter().GetResult());
            var encoded=JsonSerializer.Deserialize<Encoded[]>(File.ReadAllText(Path.Combine(job,"result.json")))??throw new InvalidDataException("Audio encoder result missing.");if(encoded.Length!=files.Length)throw new InvalidDataException("Audio encoder result count differs.");
            for(int i=0;i<encoded.Length;i++){
                var e=encoded[i];var request=requests[i];if(e.Name!=request.Name||Path.GetFileName(e.Payload)!=e.Payload||e.Rate!=request.Rate||e.Channels!=waves[i].Channels||e.Samples<=0||(request.Rate==waves[i].Rate&&e.Samples!=waves[i].Samples))throw new InvalidDataException("Audio encoder metadata differs from captured WAV: "+request.Name);
                byte[] custom=SdkEnvironmentPreflight.Read(Path.Combine(job,e.Payload),256*1048576);var native=Serialize(files[i].DocumentElement!.GetAttribute("SubtitleStringName"),e);var prefix=AudioFileRuntimeProbe.Parse(native.InstanceData,native.InstanceData.Length);using var stream=new MemoryStream(custom,false);var framing=AudioCustomDataProbe.Inspect(stream,prefix,e.Header);
                byte tag=request.Codec switch{1=>2,29=>4,31=>6,_=>throw new InvalidDataException("Unsupported audio codec.")};if(framing.CodecTag!=tag)throw new InvalidDataException("Encoded AudioFile codec tag differs from PCCompression: "+e.Name);
                results.Add(e.Name,new(native,custom));
            }
            return results;
        }finally{
            // Reborn: remove only this exact newly created temporary job, never caller sources or game files.
            if(Directory.Exists(job)){
                string full=Path.GetFullPath(job),parent=Path.GetFullPath(Path.GetTempPath()).TrimEnd(Path.DirectorySeparatorChar);
                if(Path.GetDirectoryName(full)!=parent||!Path.GetFileName(full).StartsWith("RebornAudio-",StringComparison.Ordinal)||!Guid.TryParseExact(Path.GetFileName(full)[12..],"N",out _)||(File.GetAttributes(full)&FileAttributes.ReparsePoint)!=0)throw new InvalidDataException("Owned audio temporary directory changed before cleanup.");
                Directory.Delete(full,true);
            }
        }
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: execute the existing SIMEX PC SND path on owned files and release every native resource between leaves. */
    //-------------------------------------------------------------------------------------------------
    internal static void Worker(string directory)
    {
        if(!OperatingSystem.IsWindows()||RuntimeInformation.ProcessArchitecture!=Architecture.X86)throw new InvalidDataException("AudioFile encoding requires the Windows x86 toolchain.");
        string library=Path.Combine(AppContext.BaseDirectory,"audio.dll");if(Convert.ToHexString(SHA256.HashData(File.ReadAllBytes(library)))!=LibraryHash)throw new InvalidDataException("Audio encoder library generation differs.");
        var requests=JsonSerializer.Deserialize<Request[]>(File.ReadAllText(Path.Combine(directory,"request.json")))??throw new InvalidDataException("Audio encoder request missing.");var results=new List<Encoded>();IntPtr module=NativeLibrary.Load(library);
        T Bind<T>(string name)where T:Delegate=>Marshal.GetDelegateForFunctionPointer<T>(NativeLibrary.GetExport(module,name));
        var shutdown=Bind<VoidCall>("SIMEX_shutdown");Bind<VoidCall>("SIMEX_init")();
        try{
            var open=Bind<Open>("SIMEX_open");var create=Bind<Create>("SIMEX_create");var getInfo=Bind<Info>("SIMEX_info");var read=Bind<Transfer>("SIMEX_read");var write=Bind<Transfer>("SIMEX_write");var close=Bind<Getter>("SIMEX_close");var wclose=Bind<Getter>("SIMEX_wclose");var free=Bind<Getter>("SIMEX_freesinfo");
            for(int i=0;i<requests.Length;i++){
                var r=requests[i];if(r.Codec is not (1 or 29 or 31)||r.Rate<400||r.Rate>65535||r.Quality<0||r.Quality>100)throw new InvalidDataException("Invalid native audio request settings.");if(r.Input!=i.ToString("D6")+".wav")throw new InvalidDataException("Audio worker input path differs.");string input=Path.Combine(directory,r.Input),output=Path.Combine(directory,i.ToString("D6")+"-encoded");IntPtr source=IntPtr.Zero,target=IntPtr.Zero,info=IntPtr.Zero;
                try{
                    if(Bind<Identify>("SIMEX_id")(input,0)!=1||open(input,0,1,out source)!=1||source==IntPtr.Zero||getInfo(source,out info,0)<=0||info==IntPtr.Zero||read(source,info,0)<=0)throw new InvalidDataException("WAV native read failed: "+r.Name);
                    if(Bind<Getter>("SIMEX_getsamplerate")(info)!=r.Rate)Bind<Setter>("SIMEX_resample")(info,r.Rate);
                    int rate=Bind<Getter>("SIMEX_getsamplerate")(info),samples=Bind<Getter>("SIMEX_getnumsamples")(info),channels=Bind<Getter>("SIMEX_getchannelconfig")(info);
                    Bind<Setter>("SIMEX_setcodec")(info,r.Codec);Bind<Setter>("SIMEX_setplayloc")(info,r.Streamed?4096:2048);if(r.Codec==31)Bind<Setter>("SIMEX_setvbrquality")(info,r.Quality);
                    AudioEncoderPoc.ValidateNativeOutputPrefix(output);if(create(output,39,out target)<=0||target==IntPtr.Zero||write(target,info,0)<=0)throw new InvalidDataException("PC SND encoding failed: "+r.Name);
                    IntPtr owned=target;target=IntPtr.Zero;wclose(owned);
                    string snr=output+".snr",payload=r.Streamed?output+".sns":snr;byte[] header=r.Streamed?File.ReadAllBytes(snr):Array.Empty<byte>();if(r.Streamed&&header.Length!=8)throw new InvalidDataException("Unsupported streamed SNR header size: "+r.Name);
                    results.Add(new(r.Name,Path.GetFileName(payload),samples,rate,checked((byte)channels),header));
                }finally{AudioEncoderPoc.Cleanup(ref target,ref info,ref source,p=>wclose(p),p=>free(p),p=>close(p));}
            }
            File.WriteAllText(Path.Combine(directory,"result.json"),JsonSerializer.Serialize(results));
        }finally{shutdown();NativeLibrary.Free(module);}
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: build the recovered 32-byte EP1 prefix with optional subtitle and streamed SNR storage, without legacy KW fields. */
    //-------------------------------------------------------------------------------------------------
    private static AssetBuffer Serialize(string subtitle,Encoded e)
    {
        if(subtitle.Any(c=>c>127||char.IsControl(c)))throw new InvalidDataException("AudioFile subtitle keys currently require printable ASCII.");
        byte[] text=Encoding.ASCII.GetBytes(subtitle);int end=32+(text.Length==0?0:(text.Length+4)&~3);var b=new byte[end+e.Header.Length];var relos=new List<uint>();
        void Put(int at,uint value)=>BinaryPrimitives.WriteUInt32LittleEndian(b.AsSpan(at,4),value);
        if(text.Length>0){Put(4,(uint)text.Length);Put(8,32);text.CopyTo(b,32);relos.Add(8);}Put(12,(uint)e.Samples);Put(16,(uint)e.Rate);b[28]=e.Channels;
        if(e.Header.Length>0){Put(20,(uint)end);Put(24,(uint)e.Header.Length);e.Header.CopyTo(b,end);relos.Add(20);}
        byte[] relocation=relos.Count==0?Array.Empty<byte>():new byte[(relos.Count+1)*4];for(int i=0;i<relos.Count;i++)BinaryPrimitives.WriteUInt32LittleEndian(relocation.AsSpan(i*4),relos[i]);if(relos.Count>0)BinaryPrimitives.WriteUInt32LittleEndian(relocation.AsSpan(relocation.Length-4),uint.MaxValue);
        return new(){InstanceData=b,RelocationData=relocation,ImportsData=Array.Empty<byte>()};
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: PC-only authoring honors compression, sample-rate override, quality and explicit RAM/streamed settings. */
    //-------------------------------------------------------------------------------------------------
    private static (int Codec,int Rate,int Quality,bool Streamed) Settings(XmlElement source,Wave pcm)
    {
        int codec=source.GetAttribute("PCCompression") switch{"XAS"=>29,"NONE"=>1,"EALAYER3"=>31,_=>throw new InvalidDataException("Unsupported AudioFile PCCompression.")};int rate=source.HasAttribute("PCSampleRate")?XmlConvert.ToInt32(source.GetAttribute("PCSampleRate")):0;if(rate==0)rate=pcm.Rate;
        int quality=source.HasAttribute("PCQuality")?XmlConvert.ToInt32(source.GetAttribute("PCQuality")):75;if(rate<400||rate>65535||quality<0||quality>100)throw new InvalidDataException("AudioFile PC rate/quality is outside the native format range.");
        // Reborn: the SNR sample word reserves its top two bits for play-location flags.
        if(Math.Ceiling((double)pcm.Samples*rate/pcm.Rate)>=0x40000000)throw new InvalidDataException("Resampled AudioFile exceeds the native SNR sample counter.");
        bool streamed=source.GetAttribute("IsStreamedOnPC").ToLowerInvariant() switch{"true" or "yes" or "1"=>true,"false" or "no" or "0"=>false,_=>throw new InvalidDataException("Invalid AudioFile IsStreamedOnPC.")};return(codec,rate,quality,streamed);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: add CDATA under all three actual mod stream names using the final manifest identity, then verify archive bytes and receipt. */
    //-------------------------------------------------------------------------------------------------
    internal static void Publish(string directory,IEnumerable<InstanceDeclaration> instances,Dictionary<string,Result> encoded)
    {
        var audio=instances.Where(i=>i.Handle.TypeId==0x166B084D).ToArray();if(audio.Length==0)return;string path=Path.Combine(directory,"RebornConscriptProbe.big");var files=new Dictionary<string,byte[]>(StringComparer.OrdinalIgnoreCase);
        using(var original=BigArchive.Open(path,100000,1048576))foreach(var item in original.Entries)files.Add(item.Name,original.ReadEntry(item,checked((int)item.StoredSize)));
        foreach(var instance in audio){var h=instance.Handle;string name=$"{h.TypeId:x8}.{h.TypeHash:x8}.{h.InstanceId:x8}.{h.InstanceHash:x8}.cdata";foreach(string suffix in new[]{"","_l","_m"})files.Add("data\\mod"+suffix+"\\cdata\\"+name,encoded[h.InstanceName].Custom);}
        byte[] packed=WorldBuilderAssetResolver.Pack(files);string temporary=path+".audio.tmp";File.WriteAllBytes(temporary,packed);
        using(var check=BigArchive.Open(temporary,files.Count,1048576))foreach(var file in files){var e=check.Entries.Single(e=>e.Name.Equals(file.Key,StringComparison.OrdinalIgnoreCase));if(!check.ReadEntry(e,file.Value.Length).SequenceEqual(file.Value))throw new InvalidDataException("AudioFile CDATA archive readback differs.");}
        File.Move(temporary,path,true);string receiptPath=Path.Combine(directory,"trial-plan.json");var receipt=JsonNode.Parse(File.ReadAllText(receiptPath))!;receipt["ArchiveEntryCount"]=files.Count;receipt["ArchiveSha256"]=Convert.ToHexString(SHA256.HashData(packed));receipt["AudioFileCDataCount"]=audio.Length;File.WriteAllText(receiptPath,receipt.ToJsonString(new JsonSerializerOptions{WriteIndented=true}));
        Console.WriteLine("AudioFile custom data packaged: "+audio.Length+" sound payload(s), three stream variants, verified readback.");
    }
}
