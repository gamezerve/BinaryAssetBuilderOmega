using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: locate diagnostics in captured authored XML, never present a generated document's line as an original source coordinate.
internal static class SdkXmlDiagnostics
{
    private const string LocationKey="Reborn.XmlLocation",NodeKey="Reborn.XmlNode";
    internal sealed record Location(string File,int Line,int Column,bool Context=false)
    {
        public override string ToString()=>File+(Line>0?$"({Line},{Column})":" (line unavailable)")+(Context?" [source context; exact field origin unavailable]":"");
    }
    internal sealed record Snapshot(string Path,byte[] Bytes);

    //-------------------------------------------------------------------------------------------------
    /** Reborn: retain exact syntax coordinates and the physical input file while disabling DTD/entity/network processing. */
    //-------------------------------------------------------------------------------------------------
    internal static XmlDocument Parse(byte[] bytes,string? path=null)
    {
        try{using var input=new MemoryStream(bytes,false);using var reader=XmlReader.Create(input,Settings(bytes.LongLength),path??string.Empty);var xml=new XmlDocument{PreserveWhitespace=true,XmlResolver=null};xml.Load(reader);return xml;}
        catch(XmlException error){if(path!=null)Attach(error,new(path,error.LineNumber,error.LinePosition,error.LineNumber<=0));throw;}
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: line-aware snapshots are bounded and immutable diagnostic evidence, not alternate source or schema resolvers. */
    //-------------------------------------------------------------------------------------------------
    internal static XDocument Lines(byte[] bytes,string? path=null)
    {
        try{using var input=new MemoryStream(bytes,false);using var reader=XmlReader.Create(input,Settings(bytes.LongLength),path??string.Empty);return XDocument.Load(reader,LoadOptions.PreserveWhitespace|LoadOptions.SetLineInfo);}
        catch(XmlException error){if(path!=null)Attach(error,new(path,error.LineNumber,error.LinePosition,error.LineNumber<=0));throw;}
    }

    // Reborn: source readers share one explicit size/entity policy and never trust xsi schema locations.
    // Reborn: callers bound captured bytes; expanded generated XML uses its actual input length, with no DTD/entity expansion.
    private static XmlReaderSettings Settings(long bytes)=>new(){DtdProcessing=DtdProcessing.Prohibit,XmlResolver=null,MaxCharactersInDocument=Math.Max(1,bytes)};

    // Reborn: enrich existing exception types so existing refusal controls and API contracts remain intact.
    internal static T At<T>(T error,XmlNode? node) where T:Exception {if(node!=null&&!error.Data.Contains(NodeKey))error.Data[NodeKey]=node;return error;}
    internal static void Attach(Exception error,Location location){if(!error.Data.Contains(LocationKey))error.Data[LocationKey]=location;}
    internal static XmlNode? Node(Exception error)=>error.Data[NodeKey] as XmlNode;
    internal static Location? Origin(Exception error)=>error.Data[LocationKey] as Location;

    // Reborn: preparation result wrappers preserve structured provenance independently of their bounded human-readable evidence text.
    internal static T Carry<T>(T error,SdkSelfAttributeInheritance.Result result) where T:Exception {At(error,result.DiagnosticNode);if(result.DiagnosticLocation!=null)Attach(error,result.DiagnosticLocation);return error;}

    //-------------------------------------------------------------------------------------------------
    /** Reborn: comparison masks can rename IDs or remove other owner families; bind matching expanded names and same-family ordinals instead of shifted absolute positions. */
    //-------------------------------------------------------------------------------------------------
    internal static XmlNode? Unmasked(XmlNode? masked,XmlDocument original)
    {
        var element=masked as XmlElement??(masked as XmlAttribute)?.OwnerElement;if(element==null)return original.DocumentElement;
        var steps=new List<(string Namespace,string Name,string Id,int Index)>();
        for(var current=element;current.ParentNode is XmlElement parent;current=parent)steps.Add((current.NamespaceURI,current.LocalName,current.GetAttribute("id"),parent.ChildNodes.OfType<XmlElement>().Where(e=>e.NamespaceURI==current.NamespaceURI&&e.LocalName==current.LocalName).TakeWhile(e=>!ReferenceEquals(e,current)).Count()));
        steps.Reverse();XmlElement? target=original.DocumentElement;
        foreach(var step in steps){var candidates=target?.ChildNodes.OfType<XmlElement>().Where(e=>e.NamespaceURI==step.Namespace&&e.LocalName==step.Name).ToArray()??Array.Empty<XmlElement>();var ids=step.Id.Length==0?Array.Empty<XmlElement>():candidates.Where(e=>e.GetAttribute("id")==step.Id).ToArray();target=ids.Length==1?ids[0]:candidates.ElementAtOrDefault(step.Index);}
        return masked is XmlAttribute attribute?(XmlNode?)target?.GetAttributeNode(attribute.LocalName,attribute.NamespaceURI)??target:target;
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: preserve the schema validator's actual offending attribute/element through the InvalidDataException-based frontend. */
    //-------------------------------------------------------------------------------------------------
    internal static InvalidDataException SchemaError(ValidationEventArgs args)=>At(new InvalidDataException(args.Message,args.Exception),args.Exception is XmlSchemaValidationException validation?validation.SourceObject as XmlNode:null);

    //-------------------------------------------------------------------------------------------------
    /** Reborn: readable one-line diagnostics retain raw exception details separately; generic callers get an honestly labelled input-file context only. */
    //-------------------------------------------------------------------------------------------------
    internal static string? Format(Exception error,IEnumerable<string>? arguments=null)
    {
        var location=Origin(error);
        if(location==null&&arguments!=null)
        {
            foreach(string argument in arguments.Where(a=>a.EndsWith(".xml",StringComparison.OrdinalIgnoreCase)&&File.Exists(a)))
            {
                try{string path=Path.GetFullPath(argument);var xml=Lines(SdkEnvironmentPreflight.Read(path,4*1048576),path);if(xml.Root!=null)location=Point(path,xml.Root,true);}
                catch(Exception fallback)when(fallback is IOException or InvalidDataException or XmlException or ArgumentException or UnauthorizedAccessException or NotSupportedException){location=Origin(fallback);}
                if(location!=null)break;
            }
        }
        // Reborn: coded duplicate-asset diagnostics lead with their code even after source-preparation wrappers add context.
        string message=error.Message.Replace('\r',' ').Replace('\n',' ');int duplicate=message.IndexOf("DUPLICATEASSET ",StringComparison.Ordinal);
        if(duplicate>=0)return message[duplicate..]+(location==null?string.Empty:" Source: "+location+".");
        return location==null?null:location+": "+message;
    }

    // Reborn: XObject coordinates refer to the captured original bytes, including XML declarations, comments and blank lines.
    internal static Location Point(string file,XObject value,bool context=false){var line=(IXmlLineInfo)value;return new(file,line.HasLineInfo()?line.LineNumber:0,line.HasLineInfo()?line.LinePosition:0,context);}

    // Reborn: source maps are build-local; no global mutable provenance or generated-XML line-number fallback is used.
    internal sealed class SourceMap
    {
        private readonly Snapshot[] snapshots;
        internal SourceMap(IEnumerable<Snapshot> sources)=>snapshots=sources.DistinctBy(s=>s.Path,StringComparer.OrdinalIgnoreCase).ToArray();

        //-------------------------------------------------------------------------------------------------
        /** Reborn: prefer a unique authored field on the actual inheritance chain; ambiguous/missing matches fall back to a labelled owner context. */
        //-------------------------------------------------------------------------------------------------
        internal void Enrich(Exception error,string contextFile,XmlDocument? final=null)
        {
            if(Origin(error)!=null)return;
            var documents=snapshots.Select(s=>(s.Path,Xml:Lines(s.Bytes,s.Path))).ToArray();
            XmlNode? node=Node(error);XmlElement? element=node as XmlElement??(node as XmlAttribute)?.OwnerElement;
            // Reborn: support assets are real diagnostic owners too; an included weapon error must not fall back to the entry's Includes line.
            // Reborn: a shader's nested Texture constant is not a declared Texture asset; real owners carry an asset id.
            var owner=element;while(owner!=null&&(!IsAsset(owner.LocalName)||!owner.HasAttribute("id")))owner=owner.ParentNode as XmlElement;
            if(owner==null&&final?.DocumentElement is XmlElement finalOwner&&IsAsset(finalOwner.LocalName))owner=finalOwner;
            var frames=documents.SelectMany(d=>d.Xml.Root==null?Array.Empty<(string Path,XElement Root)>():d.Xml.Root.Name.LocalName=="AssetDeclaration"?d.Xml.Root.Elements().Where(e=>IsAsset(e.Name.LocalName)).Select(e=>(d.Path,Root:e)):new[]{(d.Path,Root:d.Xml.Root)}).ToArray();
            var ranks=new Dictionary<XElement,int>();
            if(owner!=null)
            {
                string type=owner.LocalName,id=owner.GetAttribute("id");
                var visited=new HashSet<string>(StringComparer.Ordinal);
                for(int rank=0;rank<33&&visited.Add(type+":"+id);rank++)
                {
                    var matches=frames.Where(f=>f.Root.Name.LocalName==type&&(string?)f.Root.Attribute("id")==id).ToArray();
                    // Reborn: prefer an explicit field in the current source; inherited-only fields may have a unique same-ID instance-source origin.
                    if(matches.Length!=1){var local=matches.Where(f=>string.Equals(f.Path,contextFile,StringComparison.OrdinalIgnoreCase)).ToArray();if(rank==0&&local.Length==1&&((string?)local[0].Root.Attribute("inheritFrom")==id||(string?)local[0].Root.Attribute("inheritFrom")==type+":"+id)){ranks[local[0].Root]=0;foreach(var candidate in matches.Where(f=>!ReferenceEquals(f.Root,local[0].Root)))ranks[candidate.Root]=1;}break;}
                    ranks[matches[0].Root]=rank;string? parent=(string?)matches[0].Root.Attribute("inheritFrom");if(string.IsNullOrEmpty(parent))break;id=parent.Contains(':')?parent[(parent.IndexOf(':')+1)..]:parent;
                }
            }
            if(element!=null)
            {
                // Reborn: module IDs survive merge/fragment detachment and are stronger evidence than generated sibling indices.
                var module=element.SelectSingleNode("ancestor-or-self::*[@id][not(local-name()='GameObject' or local-name()='LogicCommand' or local-name()='ArmorTemplate' or local-name()='WeaponTemplate' or local-name()='LogicCommandSet' or local-name()='UpgradeTemplate' or local-name()='ExperienceLevelTemplate' or local-name()='SpecialPowerTemplate' or local-name()='PlayerPowerButtonTemplate' or local-name()='JoypadCommandBarButtonTemplate' or local-name()='ButtonSingleStateData')][1]") as XmlElement;
                string selector=Relative(element,module??owner);string? attribute=node is XmlAttribute a?a.LocalName:null;string attributeNs=node is XmlAttribute an?an.NamespaceURI:string.Empty;
                var candidates=new List<(string File,XObject Value,int Rank)>();
                foreach(var frame in frames)
                {
                    bool fragment=!IsAsset(frame.Root.Name.LocalName)||frame.Root.Attribute("id")==null;
                    if(!fragment&&!ranks.ContainsKey(frame.Root))continue;
                    var anchors=module==null?new[]{frame.Root}:frame.Root.DescendantsAndSelf().Where(e=>e.Name.LocalName==module.LocalName&&e.Name.NamespaceName==module.NamespaceURI&&(string?)e.Attribute("id")==module.GetAttribute("id"));
                    foreach(var anchor in anchors)
                    foreach(var candidate in anchor.DescendantsAndSelf().Where(e=>Relative(e,anchor)==selector))
                    {
                        XObject? value=attribute==null?candidate:candidate.Attribute(XName.Get(attribute,attributeNs));if(value==null)continue;
                        // Reborn: appended leaf/script text can have several contributors; require literal equality before claiming a unique leaf-text origin.
                        if(attribute==null&&!element.ChildNodes.OfType<XmlElement>().Any()&&candidate.Value!=element.InnerText)continue;
                        candidates.Add((frame.Path,value,fragment?100:ranks[frame.Root]));
                    }
                }
                if(candidates.Count>0)
                {
                    int rank=candidates.Min(c=>c.Rank);var best=candidates.Where(c=>c.Rank==rank).DistinctBy(c=>(c.File,Point(c.File,c.Value).Line,Point(c.File,c.Value).Column)).ToArray();
                    if(best.Length==1){Attach(error,Point(best[0].File,best[0].Value));return;}
                }
                // Reborn: metadata directives have no asset owner; match only in their known original document, never another include with the same spelling.
                var document=documents.FirstOrDefault(d=>d.Path.Equals(contextFile,StringComparison.OrdinalIgnoreCase));
                if(owner==null&&document.Xml?.Root!=null)
                {
                    string key=Relative(element,element.OwnerDocument!.DocumentElement);
                    var match=document.Xml.Root.DescendantsAndSelf().Where(e=>Relative(e,document.Xml.Root)==key).ToArray();
                    if(match.Length==1){XObject value=node is XmlAttribute aNode?(XObject?)match[0].Attribute(XName.Get(aNode.LocalName,aNode.NamespaceURI))??match[0]:match[0];Attach(error,Point(contextFile,value));return;}
                }
            }
            // Reborn: an entry containing only Includes is not the authored asset's context; prefer its uniquely resolved declaring document.
            var declared=frames.Where(f=>ranks.TryGetValue(f.Root,out int rank)&&rank==0).ToArray();
            if(declared.Length==1){Attach(error,Point(declared[0].Path,declared[0].Root,true));return;}
            var context=frames.FirstOrDefault(f=>f.Path.Equals(contextFile,StringComparison.OrdinalIgnoreCase)&&owner!=null&&f.Root.Name.LocalName==owner.LocalName&&(string?)f.Root.Attribute("id")==owner.GetAttribute("id"));
            if(context.Root!=null){Attach(error,Point(context.Path,context.Root,true));return;}
            var root=documents.FirstOrDefault(d=>d.Path.Equals(contextFile,StringComparison.OrdinalIgnoreCase));
            if(root.Xml?.Root!=null)Attach(error,Point(root.Path,root.Xml.Root,true));
        }
    }

    // Reborn: all emitted mixed-route types, including upgrades, share identity-based source provenance through their actual inherited source files.
    // Reborn: models and other emitted support assets own their W3X/XML coordinates, just like GameObjects and commands.
    private static bool IsAsset(string name)=>name is "GameObject" or "LogicCommand" or "LogicCommandSet" or "ArmorTemplate" or "WeaponTemplate" or "UpgradeTemplate" or "ButtonSingleStateData" or "JoypadCommandBarButtonTemplate" or "PlayerPowerButtonTemplate" or "SpecialPowerTemplate" or "ExperienceLevelTemplate" or "ObjectCreationList" or "Texture" or "PackedTextureImage" or "W3DContainer" or "W3DHierarchy" or "W3DCollisionBox" or "W3DMesh";

    //-------------------------------------------------------------------------------------------------
    /** Reborn: relative expanded-name paths preserve unkeyed sibling indices; keyed children use stable authored identity instead. */
    //-------------------------------------------------------------------------------------------------
    private static string Relative(XmlElement node,XmlElement? anchor)
    {
        if(anchor==null)return string.Empty;var parts=new List<string>();
        for(XmlElement? current=node;current!=null&&!ReferenceEquals(current,anchor);current=current.ParentNode as XmlElement)
        {string? id=current.HasAttribute("id")?current.GetAttribute("id"):current.HasAttribute("ID")?current.GetAttribute("ID"):null;int index=current.PreviousSiblings().Count(e=>e.LocalName==current.LocalName&&e.NamespaceURI==current.NamespaceURI);parts.Add(Segment(current.NamespaceURI,current.LocalName,id,index));}
        parts.Reverse();return string.Join('/',parts);
    }

    // Reborn: LINQ XML and DOM use identical locator semantics without embedding any locator in authored XML.
    private static string Relative(XElement node,XElement anchor)=>string.Join('/',node.AncestorsAndSelf().TakeWhile(e=>!ReferenceEquals(e,anchor)).Reverse().Select(e=>Segment(e.Name.NamespaceName,e.Name.LocalName,(string?)e.Attribute("id")??(string?)e.Attribute("ID"),e.ElementsBeforeSelf(e.Name).Count())));
    private static string Segment(string ns,string name,string? id,int index)=>"{"+ns+"}"+name+(id==null?"["+index+"]":"[id="+id+"]");

    // Reborn: avoid XPath text interpolation and any escaping ambiguity in source-controlled IDs.
    private static IEnumerable<XmlElement> PreviousSiblings(this XmlElement node){for(XmlNode? previous=node.PreviousSibling;previous!=null;previous=previous.PreviousSibling)if(previous is XmlElement element)yield return element;}
}
