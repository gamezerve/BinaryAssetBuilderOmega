using System.Buffers.Binary;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: separate AdditionalMaps streams are compiler dependencies and archive entries, never copied stock assets.
internal static class SdkGlobalStreams
{
    internal sealed record Stream(string Manifest,string Runtime,string Sha256);

    //-------------------------------------------------------------------------------------------------
    /** Reborn: accept only the exact compiled manifest generation and runtime discovery names supplied by this build. */
    //-------------------------------------------------------------------------------------------------
    internal static Stream[] Load(string? context)
    {
        if(string.IsNullOrEmpty(context))return Array.Empty<Stream>();
        var streams=JsonSerializer.Deserialize<Stream[]>(SdkEnvironmentPreflight.Read(context,1048576))??throw new InvalidDataException("Invalid global stream context.");
        var names=new HashSet<string>(StringComparer.OrdinalIgnoreCase);var identities=new HashSet<AssetId>();
        foreach(var stream in streams){
            if(!Regex.IsMatch(stream.Runtime,@"\Adata\\additionalmaps\\mapmetadata_[a-z0-9_-]+\.manifest\z",RegexOptions.IgnoreCase)||!names.Add(stream.Runtime))throw new InvalidDataException("Invalid/duplicate global stream runtime name: "+stream.Runtime);
            byte[] bytes=SdkEnvironmentPreflight.Read(stream.Manifest,64*1048576);
            if(Convert.ToHexString(SHA256.HashData(bytes))!=stream.Sha256)throw new InvalidDataException("Compiled global manifest changed: "+stream.Manifest);
            var document=ManifestReader.Read(bytes);TypeRegistryAudit.ValidateTarget(document.Header.Version,document.Header.AllTypesHash);
            if(!document.Header.IsLinked||document.Validate().Count!=0)throw new InvalidDataException("Invalid compiled global manifest: "+stream.Manifest);
            foreach(var asset in document.Assets)if(!identities.Add(new(asset.TypeId,asset.InstanceId)))throw new InvalidDataException("Asset declared in multiple global streams: "+asset.Name);
        }
        return streams;
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: preserve the static BIG verbatim by entry, append each early-load family once, and independently verify every stored payload. */
    //-------------------------------------------------------------------------------------------------
    internal static void Package(string staticBig,string context,string output)
    {
        var globals=Load(context);using var original=BigArchive.Open(staticBig);
        var payloads=new List<(string Name,byte[] Bytes)>();
        foreach(var entry in original.Entries)payloads.Add((entry.Name,original.ReadEntry(entry)));
        foreach(var global in globals)foreach(string extension in new[]{".manifest",".bin",".relo",".imp",".version"}){
            byte[] bytes=extension==".version"?Encoding.ASCII.GetBytes("\r\n"):SdkEnvironmentPreflight.Read(Path.ChangeExtension(global.Manifest,extension),256*1048576);
            payloads.Add((Path.ChangeExtension(global.Runtime,extension),bytes));
        }
        var unique=new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        foreach(var entry in payloads)if(!unique.Add(entry.Name)||entry.Name.Any(c=>c<32||c>126))throw new InvalidDataException("Invalid/duplicate archive name: "+entry.Name);
        long header=(16L+payloads.Sum(p=>8L+Encoding.ASCII.GetByteCount(p.Name)+1)+7)&~7L,total=header+payloads.Sum(p=>(long)p.Bytes.Length);
        if(total>uint.MaxValue)throw new InvalidDataException("Streams BIG exceeds BIG4 size limit.");
        using(var file=new FileStream(output,FileMode.CreateNew,FileAccess.Write,FileShare.None))using(var writer=new BinaryWriter(file,Encoding.ASCII)){
            writer.Write(Encoding.ASCII.GetBytes("BIG4"));writer.Write((uint)total);Word((uint)payloads.Count);Word((uint)header);long offset=header;
            foreach(var entry in payloads){Word((uint)offset);Word((uint)entry.Bytes.Length);writer.Write(Encoding.ASCII.GetBytes(entry.Name+"\0"));offset+=entry.Bytes.Length;}
            while(file.Position<header)writer.Write((byte)0);foreach(var entry in payloads)writer.Write(entry.Bytes);
            // Reborn: BIG4 directory words use network byte order; the total length word retains EA's little-endian convention.
            void Word(uint value){Span<byte> bytes=stackalloc byte[4];BinaryPrimitives.WriteUInt32BigEndian(bytes,value);writer.Write(bytes);}
        }
        using var readback=BigArchive.Open(output);
        if(new FileInfo(output).Length!=total||readback.Entries.Count!=payloads.Count)throw new InvalidDataException("Combined streams BIG header differs.");
        foreach(var entry in payloads)if(!readback.ReadEntry(readback.Entries.Single(e=>e.Name==entry.Name),entry.Bytes.Length).SequenceEqual(entry.Bytes))throw new InvalidDataException("Combined streams BIG payload differs: "+entry.Name);
        Console.WriteLine("Streams BIG verified: "+payloads.Count+" files; "+globals.Length+" separate AdditionalMaps global streams.");
    }
}
