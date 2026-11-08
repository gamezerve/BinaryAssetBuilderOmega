using System.Text.Json;
using System.Xml;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: source fragments and definition libraries must be tested through real authored consumers, never relabeled as standalone native assets.
internal static class SdkStockSourcePlan
{
    internal sealed record Row(string Relative,string Entry,string Kind,string[] Consumers,string? Diagnostic);
    private sealed record FileInfo(string Relative,string Root,bool HasAssets,bool HasIncludes,string? Error);

    //-------------------------------------------------------------------------------------------------
    /** Reborn: inventory read-only original XML and find nearest source-authored include consumers with cycle detection. */
    //-------------------------------------------------------------------------------------------------
    internal static void Run(string sdk,string output)
    {
        sdk=SdkEnvironmentPreflight.DirectoryPath(sdk);string source=Path.Combine(sdk,"SageXML");
        output=SdkEnvironmentPreflight.Absolute(output);
        if(!output.StartsWith(Path.Combine(sdk,".work")+Path.DirectorySeparatorChar,StringComparison.OrdinalIgnoreCase))throw new InvalidDataException("Stock plan output must stay in SDK work.");
        SdkEnvironmentPreflight.CheckPath(output);
        var files=new Dictionary<string,FileInfo>(StringComparer.OrdinalIgnoreCase);
        var parents=new Dictionary<string,HashSet<string>>(StringComparer.OrdinalIgnoreCase);
        foreach(string path in Directory.GetFiles(source,"*.xml",SearchOption.AllDirectories).OrderBy(p=>p,StringComparer.OrdinalIgnoreCase))
        {
            string relative=Path.GetRelativePath(source,path);
            try
            {
                SdkEnvironmentPreflight.CheckPath(path);var xml=SdkXmlDiagnostics.Parse(File.ReadAllBytes(path),path);var root=xml.DocumentElement!;
                bool assets=root.LocalName=="AssetDeclaration"&&root.ChildNodes.OfType<XmlElement>().Any(e=>e.LocalName is not ("Tags" or "Includes" or "Defines"));
                bool includes=root.GetElementsByTagName("Include","uri:ea.com:eala:asset").Count!=0;
                files.Add(relative,new(relative,root.LocalName,assets,includes,null));
                foreach(XmlElement element in xml.GetElementsByTagName("*"))
                {
                    // Reborn: a reference stream loads compiled assets; it does not capture the referenced source closure.
                    if(element.LocalName=="Include"&&element.NamespaceURI=="uri:ea.com:eala:asset"&&element.GetAttribute("type")=="reference")continue;
                    string? dependency=element.LocalName=="Include"&&element.NamespaceURI=="uri:ea.com:eala:asset"?element.GetAttribute("source"):
                        element.LocalName=="include"&&element.NamespaceURI=="http://www.w3.org/2001/XInclude"?element.GetAttribute("href"):null;
                    if(string.IsNullOrWhiteSpace(dependency))continue;
                    string target;
                    if(dependency.StartsWith("DATA:",StringComparison.OrdinalIgnoreCase))target=Path.Combine(source,dependency[5..].Replace('/',Path.DirectorySeparatorChar));
                    else if(dependency.Contains(':')||Path.IsPathRooted(dependency))continue;
                    else target=Path.Combine(Path.GetDirectoryName(path)!,dependency.Replace('/',Path.DirectorySeparatorChar));
                    target=Path.GetFullPath(target);
                    if(!target.StartsWith(source+Path.DirectorySeparatorChar,StringComparison.OrdinalIgnoreCase))continue;
                    string child=Path.GetRelativePath(source,target);
                    if(!parents.TryGetValue(child,out var owners))parents.Add(child,owners=new(StringComparer.OrdinalIgnoreCase));
                    owners.Add(relative);
                }
            }
            catch(XmlException error){files.Add(relative,new(relative,"InvalidXml",false,false,error.Message));}
        }
        var plan=new List<Row>();
        foreach(var file in files.Values)
        {
            bool context=file.Error==null&&(file.Root!="AssetDeclaration"||!file.HasAssets&&!file.HasIncludes);
            string[] consumers=context?Consumers(file.Relative):Array.Empty<string>();
            plan.Add(new(file.Relative,consumers.FirstOrDefault()??file.Relative,
                file.Error!=null?"InvalidSource":!context?"Standalone":consumers.Length>0?"ConsumerContext":"UnreferencedSourceFragment",consumers,file.Error));
        }
        File.WriteAllText(output,JsonSerializer.Serialize(plan,new JsonSerializerOptions{WriteIndented=true}));
        Console.WriteLine("Reborn: stock source plan captured "+plan.Count+" files; "+plan.Count(r=>r.Kind=="ConsumerContext")+" source-only files have authored consumers; "+plan.Count(r=>r.Kind=="UnreferencedSourceFragment")+" lack a consumer.");

        //-------------------------------------------------------------------------------------------------
        /** Reborn: only actual include edges authorize a consumer context; breadth-first traversal prefers the nearest declarations. */
        //-------------------------------------------------------------------------------------------------
        string[] Consumers(string child)
        {
            var visited=new HashSet<string>(StringComparer.OrdinalIgnoreCase){child};var queue=new Queue<string>();queue.Enqueue(child);
            var candidates=new List<string>();
            while(queue.Count>0)
            {
                string current=queue.Dequeue();if(!parents.TryGetValue(current,out var owners))continue;
                foreach(string owner in owners.OrderBy(p=>p,StringComparer.OrdinalIgnoreCase))
                {
                    if(!visited.Add(owner))continue;
                    if(files.TryGetValue(owner,out var file)&&file.Error==null&&file.Root=="AssetDeclaration"&&file.HasAssets)candidates.Add(owner);
                    else queue.Enqueue(owner);
                }
            }
            return candidates.ToArray();
        }
    }
}
