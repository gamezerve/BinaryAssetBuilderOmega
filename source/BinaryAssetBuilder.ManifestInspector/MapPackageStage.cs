using System.Buffers.Binary;
using System.Text;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: map publication accepts only a linked EP1 map graph, never a renamed static/support-only package.
internal static class MapPackageStage
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: stage the verified base stream as map_mod.*, retain CDATA payloads and write the unsuffixed _mod selector. */
    //-------------------------------------------------------------------------------------------------
    internal static void Run(string package,string output)
    {
        output=Path.GetFullPath(output);if(Directory.Exists(output)||File.Exists(output))throw new InvalidDataException("Fresh map staging directory required.");
        using var archive=BigArchive.Open(package);
        var entries=archive.Entries.ToDictionary(e=>e.Name.Replace('/','\\'),StringComparer.OrdinalIgnoreCase);
        const string stream="data\\mod";
        if(!entries.TryGetValue(stream+".manifest",out var manifest))throw new InvalidDataException("Map backend package has no base mod.manifest stream.");
        var doc=ManifestReader.Read(archive.ReadEntry(manifest));TypeRegistryAudit.ValidateTarget(doc.Header.Version,doc.Header.AllTypesHash);
        if(!doc.Header.IsLinked||doc.Validate().Count!=0||doc.Assets.Count(a=>a.TypeName=="GameMap")!=1)
            throw new InvalidDataException("MAPCOMPILER: expected exactly one compiled GameMap in a valid linked EP1 map stream. Support-only assets cannot substitute for map compilation.");
        var staged=new Dictionary<string,byte[]>(StringComparer.OrdinalIgnoreCase);
        foreach(string ext in new[]{"manifest","bin","relo","imp"}){
            if(!entries.TryGetValue(stream+"."+ext,out var item))throw new InvalidDataException("Compiled map sidecar is missing: "+ext);
            byte[] bytes=archive.ReadEntry(item);
            if(ext!="manifest"){
                long expected=8+doc.Assets.Sum(a=>(long)(ext=="bin"?a.InstanceDataSize:ext=="relo"?a.RelocationDataSize:a.ImportsDataSize));
                // Reborn: EP1 uses distinct BIN/RELO/IMP kind markers, all followed by the same stream checksum.
                uint magic=ext=="bin"?0xBABB0000u:ext=="relo"?0xBABE0000u:0xBAB10000u;
                if(bytes.Length!=expected||BinaryPrimitives.ReadUInt32LittleEndian(bytes)!=magic||BinaryPrimitives.ReadUInt32LittleEndian(bytes.AsSpan(4))!=doc.Header.StreamChecksum)
                    throw new InvalidDataException("Compiled map sidecar size/checksum differs: "+ext);
            }
            staged.Add("map_mod."+ext,bytes);
        }
        // Reborn: replace the backend selector with map.version; a map_mod.version selector would redirect the wrong stream name.
        foreach(var pair in entries.Where(p=>p.Key.StartsWith(stream+"\\",StringComparison.OrdinalIgnoreCase))){
            string relative=pair.Key[(stream.Length+1)..];
            if(relative.Split('\\').Any(p=>p.Length==0||p is "." or ".."||p.Contains(':'))||Path.IsPathRooted(relative))throw new InvalidDataException("Unsafe map payload path: "+relative);
            if(relative.EndsWith(".asset",StringComparison.OrdinalIgnoreCase))continue;
            if(!staged.TryAdd(relative,archive.ReadEntry(pair.Value)))throw new InvalidDataException("Duplicate map output: "+relative);
        }
        staged.Add("map.version",Encoding.ASCII.GetBytes("_mod\r\n"));
        Directory.CreateDirectory(output);
        foreach(var pair in staged){string path=Path.Combine(output,pair.Key);Directory.CreateDirectory(Path.GetDirectoryName(path)!);File.WriteAllBytes(path,pair.Value);}
        Console.WriteLine($"Map stream staged: {doc.Assets.Count} assets; map_mod.bin/imp/manifest/relo; map.version=_mod; {staged.Count} files.");
    }
}
