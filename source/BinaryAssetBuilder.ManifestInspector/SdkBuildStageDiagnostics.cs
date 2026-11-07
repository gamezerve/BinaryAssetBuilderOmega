using System.Text.Json;
using System.Xml.Linq;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: retain immutable warning coordinates and inheritance edges with each compiled family.
internal static class SdkBuildStageDiagnostics
{
    internal sealed record Origin(string Type,string Id,SdkXmlDiagnostics.Location Location);
    internal sealed record Evidence(Origin[] Origins,SdkSelfAttributeInheritance.Overlay[] InheritanceUses);

    //-------------------------------------------------------------------------------------------------
    /** Reborn: snapshot diagnostics only for assets actually emitted by this saved stage. */
    //-------------------------------------------------------------------------------------------------
    internal static void Capture(string payload,string search,string output)
    {
        var names=Directory.GetFiles(payload,"*.manifest",SearchOption.AllDirectories)
            .SelectMany(p=>ManifestReader.Read(File.ReadAllBytes(p)).Assets).Select(a=>a.Name).ToHashSet(StringComparer.OrdinalIgnoreCase);
        var origins=new Dictionary<string,Origin>(StringComparer.OrdinalIgnoreCase);
        var inheritance=new List<SdkSelfAttributeInheritance.Overlay>();
        // Reborn: retain only the current source tree rather than every XML document in a large mod.
        string? currentFile=null;XDocument? document=null;
        foreach(string path in Directory.GetFiles(search,"source-input.json",SearchOption.AllDirectories))
        {
            using var json=JsonDocument.Parse(File.ReadAllBytes(path));
            if(json.RootElement.TryGetProperty("Origins",out var rows))foreach(var row in rows.EnumerateArray())
            {
                string type=row.GetProperty("Type").GetString()!,id=row.GetProperty("Id").GetString()!,file=row.GetProperty("Path").GetString()!;
                if(!names.Contains(type+":"+id))continue;
                if(!string.Equals(currentFile,file,StringComparison.OrdinalIgnoreCase)){document=SdkXmlDiagnostics.Lines(File.ReadAllBytes(file),file);currentFile=file;}
                var owner=document!.Descendants().FirstOrDefault(e=>e.Name.LocalName==type&&(string?)e.Attribute("id")==id);
                var location=owner==null?new SdkXmlDiagnostics.Location(file,1,1,true):SdkXmlDiagnostics.Point(file,(XObject?)owner.Attribute("id")??owner);
                origins[type+":"+id]=new(type,id,location);
            }
            if(json.RootElement.TryGetProperty("InheritanceUses",out var uses))inheritance.AddRange((uses.Deserialize<SdkSelfAttributeInheritance.Overlay[]>()??Array.Empty<SdkSelfAttributeInheritance.Overlay>()).Where(e=>names.Contains(e.Type+":"+e.DerivedId)));
        }
        File.WriteAllText(output,JsonSerializer.Serialize(new Evidence(origins.Values.ToArray(),inheritance.Distinct().ToArray())));
    }
}
