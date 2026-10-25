using System.Diagnostics;
using System.Text.Json;
using BinaryAssetBuilder.Core;
using BinaryAssetBuilder.Core.Hashing;
using BinaryAssetBuilder.Core.SageXml;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: readable EA BuildMod-shaped progress lines. Every value is taken from the prepared source graph (physical files, emitted
// asset type/id) or measured during the real compile loop; the bracketed pair is the type hash and instance id the native stream uses.
internal sealed class ConscriptCompileLog
{
    internal sealed class Entry{internal string Type="",Name="";internal Stopwatch Clock=new();}
    private readonly Dictionary<string,string> files=new(StringComparer.OrdinalIgnoreCase);
    private readonly List<Entry> processed=new();
    // Reborn: a fixture without declaration provenance can still report its honest prepared-input context.
    private string context="";

    //-------------------------------------------------------------------------------------------------
    /** Reborn: list every captured source file once, at the moment the source graph has actually been read and hash-verified. */
    //-------------------------------------------------------------------------------------------------
    internal static void Sources(ConscriptSourceInput.Input input){foreach(var source in input.Sources)Console.WriteLine("Loading '"+new Uri(source.Path).AbsoluteUri+"'");}

    //-------------------------------------------------------------------------------------------------
    /** Reborn: declaring files come from the preparation receipt beside the prepared entry; absent receipts (fixtures) fall back to <id>.xml. */
    //-------------------------------------------------------------------------------------------------
    internal static ConscriptCompileLog ForPrepared(string entry)
    {
        var log=new ConscriptCompileLog{context=System.IO.Path.GetFullPath(entry)};string receipt=System.IO.Path.Combine(System.IO.Path.GetDirectoryName(System.IO.Path.GetFullPath(entry))!,"source-input.json");
        if(!File.Exists(receipt))return log;
        using var json=JsonDocument.Parse(File.ReadAllText(receipt));
        if(json.RootElement.TryGetProperty("Origins",out var origins)&&origins.ValueKind==JsonValueKind.Array)
            foreach(var origin in origins.EnumerateArray())log.files[origin.GetProperty("Type").GetString()+":"+origin.GetProperty("Id").GetString()]=origin.GetProperty("Path").GetString()!;
        return log;
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: locate a declaration warning in the captured original file, using its id attribute rather than a generated XML line. */
    //-------------------------------------------------------------------------------------------------
    internal SdkXmlDiagnostics.Location Location(string type,string id)
    {
        string path=files.GetValueOrDefault(type+":"+id,context);var document=SdkXmlDiagnostics.Lines(File.ReadAllBytes(path),path);var owner=document.Descendants().FirstOrDefault(e=>e.Name.LocalName==type&&(string?)e.Attribute("id")==id);return owner==null?SdkXmlDiagnostics.Point(path,document.Root!,true):SdkXmlDiagnostics.Point(path,(System.Xml.Linq.XObject?)owner.Attribute("id")??owner);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: announce one asset immediately before its real processor runs and start its processing clock. */
    //-------------------------------------------------------------------------------------------------
    internal Entry Begin(string type,string id)
    {
        string name=type+":"+id;string file=files.TryGetValue(name,out var path)?System.IO.Path.GetFileName(path):id+".xml";
        Console.WriteLine(file.ToLowerInvariant()+": Compiling "+name+" ["+FastHash.GetHashCode(type).ToString("x8")+":"+InstanceHandle.GetInstanceId(id).ToString("x8")+"]");
        var entry=new Entry{Type=type,Name=name};entry.Clock.Start();return entry;
    }

    internal void End(Entry entry){entry.Clock.Stop();processed.Add(entry);}

    //-------------------------------------------------------------------------------------------------
    /** Reborn: per-type statistics over assets that completed their processor in this build (stream/link time is not included). */
    //-------------------------------------------------------------------------------------------------
    internal void Statistics()
    {
        foreach(var group in processed.GroupBy(e=>e.Type))
        {
            var longest=group.MaxBy(e=>e.Clock.Elapsed)!;var total=TimeSpan.FromTicks(group.Sum(e=>e.Clock.Elapsed.Ticks));
            Console.WriteLine("Statistics "+group.Key+": Instances Processed "+group.Count()+", Total Processing Time "+total.ToString("c")+", Longest Processing Time "+longest.Clock.Elapsed.ToString("c")+" ("+longest.Name+"), Average Processing Time "+TimeSpan.FromTicks(total.Ticks/group.Count()).ToString("c"));
        }
    }
}
