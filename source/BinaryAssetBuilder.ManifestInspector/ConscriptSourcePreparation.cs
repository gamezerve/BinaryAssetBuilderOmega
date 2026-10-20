using System.Security.Cryptography;
using System.Text;
using System.Xml;
using BinaryAssetBuilder.Core.SageXml;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: bridge the existing source-definition/inheritance engine into GameObject authoring, retaining actual all/instance publication roles.
internal static class ConscriptSourcePreparation
{
    private const string Ea="uri:ea.com:eala:asset";

    //-------------------------------------------------------------------------------------------------
    /** Reborn: capture a confined graph, prepare each owner in its defining context and publish all-imported owners only, never instance bases. */
    //-------------------------------------------------------------------------------------------------
    internal static ConscriptSourceInput.Input Load(string entry,SdkSourceSearch? search=null)
    {
        entry=SdkEnvironmentPreflight.Absolute(entry);string root=SdkEnvironmentPreflight.DirectoryPath(Path.GetDirectoryName(entry)!);
        byte[] initial=SdkEnvironmentPreflight.Read(entry,1048576);var first=Parse(initial);
        // Reborn: preserve the previous standalone complete-object input without claiming it has an AssetDeclaration preprocessing context.
        if(first.DocumentElement?.LocalName=="GameObject")return new(first,null,new[]{new ConscriptSourceInput.SourceFile(entry,Hash(initial))});
        string fixture=ReferencePipelineSmokeTest.FindFixture();string output=Path.Combine(Path.GetTempPath(),"Reborn-SourcePlan-"+Guid.NewGuid().ToString("N"));
        var environment=new SdkEnvironmentPreflight.Report("ra3ep1",Path.GetDirectoryName(fixture)!,fixture,0,"",root,entry,Hash(initial),output,Array.Empty<SdkEnvironmentPreflight.Mapping>(),true,true,false,false,false,"0x5454A8E9",Array.Empty<string>());
        var paths=SdkSourcePathAudit.Inspect(environment,search:search);
        // Reborn: report the actual physical winner before resource/processor admission; finding a W3X never means it was compiled.
        foreach(var selection in paths.Searches.Where(s=>s.LogicalPath.Contains(':')))Console.WriteLine("Reborn: source path ["+selection.Layer+"] "+selection.LogicalPath+" -> "+selection.PhysicalPath);
        if(!paths.ScopedPathAuditComplete || paths.StoppedAtLimit || paths.Sources.Length>32 || paths.Sources.Sum(s=>s.Bytes)>4*1048576)
            throw new InvalidDataException("Source graph: "+string.Join("; ",paths.Issues.Select(i=>i.Code+" "+i.LogicalPath+": "+i.Detail)));
        if(paths.Resources.Length!=0 || paths.Sources.Any(s=>!s.PhysicalPath.EndsWith(".xml",StringComparison.OrdinalIgnoreCase)))throw new InvalidDataException("ART/audio/W3X source processors are not connected; no resources are silently skipped.");
        var snapshots=paths.Sources.ToDictionary(s=>s.PhysicalPath,s=>SdkEnvironmentPreflight.Read(s.PhysicalPath,1048576),StringComparer.OrdinalIgnoreCase);
        foreach(var source in paths.Sources)if(Hash(snapshots[source.PhysicalPath])!=source.Sha256)throw new InvalidDataException("Source changed during graph capture.");
        // Reborn: path inventory is not language admission; reject ignored metadata throughout the graph before emitting anything.
        foreach(var snapshot in snapshots.Values)ValidateEnvelope(Parse(snapshot).DocumentElement!);
        // Reborn: merge schema-valid partial bases first; final source/native admission still applies to the emitted complete object only.
        var schemas=ConscriptSourceInput.Schemas();var fragments=new SdkGameObjectFragments(paths);var profile=new SdkInstanceInheritanceProfile(schemas,paths,expressions:true,conscriptCopy:true,gameObjectTrees:true,fragments:fragments);
        var assets=new List<XmlElement>();var emitted=new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        Emit(entry);
        var games=assets.Where(a=>a.LocalName=="GameObject").ToArray();var commands=assets.Where(a=>a.LocalName=="LogicCommand").ToArray();
        if(games.Length!=1 || commands.Length>1)throw new InvalidDataException("Source output requires one GameObject and zero/one explicit LogicCommand; imported bases must use type=instance.");
        foreach(var source in paths.Sources)if(Hash(SdkEnvironmentPreflight.Read(source.PhysicalPath,1048576))!=source.Sha256)throw new InvalidDataException("Source changed during inheritance/definition preparation.");
        // Reborn: XInclude fragments are real source dependencies, including hash and higher-priority mod-shadow checks before publication.
        foreach(var source in fragments.Sources)if(Hash(SdkEnvironmentPreflight.Read(source.Path,1048576))!=source.Sha256)throw new InvalidDataException("XInclude source changed during preparation: "+source.Path);
        var searches=paths.Searches.Concat(fragments.Searches).ToArray();SdkSourceSearch.Verify(searches);
        return new(Detach(games[0]),commands.Length==0?null:Detach(commands[0]),paths.Sources.Select(s=>new ConscriptSourceInput.SourceFile(s.PhysicalPath,s.Sha256)).Concat(fragments.Sources).DistinctBy(s=>s.Path,StringComparer.OrdinalIgnoreCase).ToArray()){Searches=searches};

        //-------------------------------------------------------------------------------------------------
        /** Reborn: all Includes contribute emitted owner assets; instance Includes are read by the existing engine but never emitted by this traversal. */
        //-------------------------------------------------------------------------------------------------
        void Emit(string path)
        {
            if(!emitted.Add(path))return;var xml=Parse(snapshots[path]);var document=xml.DocumentElement!;
            foreach(var edge in paths.Includes.Where(e=>e.Document.Equals(path,StringComparison.OrdinalIgnoreCase) && e.Kind=="all"))Emit(edge.PhysicalPath??throw new InvalidDataException("Unresolved all Include."));
            var owners=document.ChildNodes.OfType<XmlElement>().Where(e=>e.LocalName is not ("Tags" or "Includes" or "Defines")).ToArray();
            if(owners.Any(a=>a.LocalName is not ("GameObject" or "LogicCommand")))throw new InvalidDataException("Unsupported emitted asset type; no silent skipping.");
            // Reborn: source-only definition leaves must still validate, even if no emitted owner happens to consume them.
            if(owners.Length==0){if(document.ChildNodes.OfType<XmlElement>().Any(e=>e.LocalName=="Defines"))SdkMetadataDefinitions.Prove(schemas,xml,path,snapshots[path]);return;}
            bool needsPreparation=document.GetElementsByTagName("Define",Ea).Count!=0 || document.GetElementsByTagName("include","http://www.w3.org/2001/XInclude").Count!=0 || owners.Any(a=>a.HasAttribute("inheritFrom") || a.SelectNodes(".//@* | .//text()")!.Cast<XmlNode>().Any(n=>(n.Value??"").StartsWith('=')));
            if(needsPreparation)
            {
                var prepared=profile.Apply(path,snapshots[path]);if(prepared.Bytes==null)throw new InvalidDataException("GameObject source preparation: "+string.Join("; ",prepared.Evidence.Diagnostics));
                xml=Parse(prepared.Bytes);owners=xml.DocumentElement!.ChildNodes.OfType<XmlElement>().Where(e=>e.LocalName is not ("Tags" or "Includes" or "Defines")).ToArray();
                // Reborn: only this successful Core overlay authorizes consuming the diagnostic marker; no raw inherited document is flattened by substitution.
                foreach(var owner in owners)owner.RemoveAttribute("inheritFrom");
            }
            foreach(var owner in owners){if(owner.HasAttribute("inheritFrom"))throw new InvalidDataException("Unprocessed inheritance marker.");assets.Add(owner);}
        }
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: preserve literal Lua/text while preventing XML entity/network processing in captured source preparation. */
    //-------------------------------------------------------------------------------------------------
    private static XmlDocument Parse(byte[] bytes){using var input=new MemoryStream(bytes,false);using var reader=XmlReader.Create(input,new XmlReaderSettings{DtdProcessing=DtdProcessing.Prohibit,XmlResolver=null,MaxCharactersInDocument=4*1048576});var xml=new XmlDocument{PreserveWhitespace=true,XmlResolver=null};xml.Load(reader);return xml;}

    //-------------------------------------------------------------------------------------------------
    /** Reborn: admit explicit EA envelopes, empty Tags and all/instance imports; reject foreign metadata and ignored reference directives. */
    //-------------------------------------------------------------------------------------------------
    private static void ValidateEnvelope(XmlElement root)
    {
        if(root.LocalName!="AssetDeclaration" || root.NamespaceURI!=Ea)throw new InvalidDataException("EA AssetDeclaration required.");
        var nodes=ConscriptSourceInput.Envelope(root).ToArray();
        if(nodes.Any(n=>n.NamespaceURI!=Ea) || nodes.GroupBy(n=>n.LocalName).Any(g=>(g.Key is "Tags" or "Includes" or "Defines") && g.Count()>1))throw new InvalidDataException("Foreign or duplicate source metadata.");
        foreach(var node in nodes)
        {
            if(node.LocalName is not ("Tags" or "Includes" or "Defines"))continue;
            if(node.Attributes.OfType<XmlAttribute>().Any(a=>a.NamespaceURI!="http://www.w3.org/2000/xmlns/"))throw new InvalidDataException("Unsupported source metadata attribute.");
            if(node.LocalName=="Tags" && node.ChildNodes.Cast<XmlNode>().Any(n=>n is XmlElement || n is not XmlComment && !string.IsNullOrWhiteSpace(n.InnerText)))throw new InvalidDataException("Only empty Tags are enabled.");
            if(node.LocalName!="Includes")continue;
            foreach(XmlNode child in node.ChildNodes)
            {
                if(child is XmlComment || child is not XmlElement && string.IsNullOrWhiteSpace(child.InnerText))continue;
                if(child is not XmlElement include || include.LocalName!="Include" || include.NamespaceURI!=Ea || include.GetAttribute("type") is not ("all" or "instance") || include.Attributes.OfType<XmlAttribute>().Any(a=>a.NamespaceURI!="http://www.w3.org/2000/xmlns/" && (a.NamespaceURI.Length!=0 || a.Name is not ("type" or "source"))) || include.ChildNodes.Cast<XmlNode>().Any(n=>n is XmlElement || n is not XmlComment && !string.IsNullOrWhiteSpace(n.InnerText)))throw new InvalidDataException("Only explicit all/instance XML Include directives are enabled.");
            }
        }
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: own the detached asset namespace without copying metadata or introducing new declarations. */
    //-------------------------------------------------------------------------------------------------
    private static XmlDocument Detach(XmlElement asset){var xml=new XmlDocument{PreserveWhitespace=true,XmlResolver=null};xml.AppendChild(xml.ImportNode(asset,true));if(xml.DocumentElement!.Prefix.Length==0 && !xml.DocumentElement.HasAttribute("xmlns"))xml.DocumentElement.SetAttribute("xmlns",Ea);return xml;}

    //-------------------------------------------------------------------------------------------------
    /** Reborn: raw source witnesses remain independent of processed owner hashes and native stream identity. */
    //-------------------------------------------------------------------------------------------------
    private static string Hash(byte[] bytes)=>Convert.ToHexString(SHA256.HashData(bytes));
}
