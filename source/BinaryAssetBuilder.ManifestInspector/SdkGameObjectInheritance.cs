using System.Security.Cryptography;
using System.Text;
using System.Xml;
using System.Xml.Schema;
using BinaryAssetBuilder.Core.SageXml;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: merge source-level GameObject bases with actual Core semantics, deferring complete Conscript/native admission until the final emitted object.
internal static class SdkGameObjectInheritance
{
    internal const string Profile="gameobject-core-source-inheritance-v1";
    private const string Ea="uri:ea.com:eala:asset";

    //-------------------------------------------------------------------------------------------------
    /** Reborn: partial base objects need schema-valid source trees, not production costs/body owners required only on the completed compiled unit. */
    //-------------------------------------------------------------------------------------------------
    internal static SdkSelfAttributeInheritance.Result Apply(XmlSchemaSet schemas,byte[] source)
    {
        string raw=Convert.ToHexString(SHA256.HashData(source));
        try
        {
            if(!schemas.IsCompiled||source.Length>4*1048576)throw new InvalidDataException("Compiled schemas and bounded GameObject source required.");
            var xml=new XmlDocument{PreserveWhitespace=true,XmlResolver=null};
            using(var input=new MemoryStream(source,false))using(var reader=XmlReader.Create(input,new XmlReaderSettings{DtdProcessing=DtdProcessing.Prohibit,XmlResolver=null,MaxCharactersInDocument=4*1048576}))xml.Load(reader);
            var root=xml.DocumentElement;if(root?.LocalName!="AssetDeclaration"||root.NamespaceURI!=Ea)throw new InvalidDataException("EA AssetDeclaration required for GameObject inheritance.");
            var assets=root.ChildNodes.OfType<XmlElement>().Where(e=>e.LocalName is not ("Tags" or "Defines" or "Includes")).ToArray();
            if(assets.Length>4096||root.SelectNodes(".//*")!.Count>8192)throw new InvalidDataException("GameObject source element/asset bound exceeded.");
            var originals=new Dictionary<string,XmlElement>(StringComparer.Ordinal);var resolved=new Dictionary<string,XmlElement>(StringComparer.Ordinal);var active=new HashSet<string>(StringComparer.Ordinal);var overlays=new List<SdkSelfAttributeInheritance.Overlay>();
            // Reborn: cap aggregate memoized trees as well as each individual merge to prevent repeated-base amplification.
            long expandedBytes=source.Length;
            foreach(var asset in assets)
            {
                // Reborn: the shared Core source overlay also prepares explicit support assets for the mixed GameObject stream.
                if(asset.LocalName is not ("GameObject" or "LogicCommand" or "LogicCommandSet" or "ArmorTemplate" or "WeaponTemplate")||!ConscriptSourceInput.Identity(asset.GetAttribute("id"))||!originals.TryAdd(asset.LocalName+":"+asset.GetAttribute("id"),asset))throw new InvalidDataException("GameObject source contains unsupported or duplicate asset identities.");
                Inspect(asset,0);
            }
            foreach(var asset in assets)root.ReplaceChild(Resolve(asset.LocalName+":"+asset.GetAttribute("id"),0),asset);
            // Reborn: validate a disposable clone so schema defaults never leak into the authored/merged source as explicit overriding values.
            var check=new XmlDocument{XmlResolver=null};check.LoadXml(xml.OuterXml);check.Schemas.Add(schemas);check.Validate((_,e)=>throw SdkXmlDiagnostics.SchemaError(e));
            byte[] bytes=Encoding.UTF8.GetBytes(xml.OuterXml);if(bytes.Length>4*1048576)throw new InvalidDataException("Merged GameObject source exceeds 4 MiB.");
            return new(bytes,new(Profile,raw,Convert.ToHexString(SHA256.HashData(bytes)),overlays.ToArray(),Array.Empty<string>()));

            //-------------------------------------------------------------------------------------------------
            /** Reborn: imported visibility is established by the caller; locally resolve only same-type unique handles and reject cycles before joining. */
            //-------------------------------------------------------------------------------------------------
            XmlElement Resolve(string handle,int depth)
            {
                if(depth>32||active.Contains(handle))throw new InvalidDataException("GameObject inheritance cycle/depth bound exceeded.");
                if(resolved.TryGetValue(handle,out var previous))return (XmlElement)previous.CloneNode(true);
                if(!originals.TryGetValue(handle,out var original))throw new InvalidDataException("GameObject inheritance base is not visible: "+handle);
                active.Add(handle);var derived=(XmlElement)original.CloneNode(true);XmlElement result=derived;
                if(derived.HasAttribute("inheritFrom"))
                {
                    // Reborn: each supported inheritable asset keeps its own QName while reusing the actual Core overlay semantics.
                    string assetType=derived.LocalName;
                    string target=derived.GetAttribute("inheritFrom"),id=target;
                    if(target.Contains(':')){var parts=target.Split(':');if(parts.Length!=2||parts[0]!=assetType)throw new InvalidDataException("Cross-type asset inheritance refused.");id=parts[1];}
                    if(!ConscriptSourceInput.Identity(id))throw new InvalidDataException("Invalid GameObject base identity.");
                    var basis=Resolve(assetType+":"+id,depth+1);derived.RemoveAttribute("inheritFrom");
                    if(Encoding.UTF8.GetByteCount(basis.OuterXml)+Encoding.UTF8.GetByteCount(derived.OuterXml)>4*1048576)throw new InvalidDataException("GameObject merge input amplification exceeds 4 MiB.");
                    result=(XmlElement)NodeJoiner.Override(schemas,xml,basis,derived);
                    overlays.Add(new(assetType,derived.GetAttribute("id"),id));
                }
                expandedBytes+=Encoding.UTF8.GetByteCount(result.OuterXml);if(expandedBytes>32*1048576)throw new InvalidDataException("GameObject resolved tree budget exceeds 32 MiB.");
                active.Remove(handle);resolved.Add(handle,(XmlElement)result.CloneNode(true));return result;
            }

            //-------------------------------------------------------------------------------------------------
            /** Reborn: normalize structural whitespace only; preserve script/text payload and reject metadata the Core joiner would silently discard. */
            //-------------------------------------------------------------------------------------------------
            void Inspect(XmlElement element,int depth)
            {
                if(depth>32||element.Prefix.Length!=0||element.NamespaceURI!=Ea)throw new InvalidDataException("Unsupported prefixed/foreign/deep GameObject source tree.");
                foreach(XmlAttribute attribute in element.Attributes)
                {
                    if(attribute.NamespaceURI=="http://www.w3.org/2000/xmlns/")continue;
                    if(attribute.LocalName=="TypeId"||attribute.Value.StartsWith('='))throw SdkXmlDiagnostics.At(new InvalidDataException("Unevaluated expression or authored TypeId in asset source: "+attribute.Name),attribute);
                    if(attribute.NamespaceURI.Length!=0 && !(attribute.NamespaceURI=="uri:ea.com:eala:asset:instance"&&attribute.LocalName is "joinAction" or "insertPosition"))throw new InvalidDataException("Unsupported GameObject directive: "+attribute.Name);
                }
                bool structured=element.ChildNodes.OfType<XmlElement>().Any()||element.LocalName=="GameObject";
                foreach(XmlNode node in element.ChildNodes.Cast<XmlNode>().ToArray())
                    if(node.NodeType is XmlNodeType.Whitespace or XmlNodeType.SignificantWhitespace){if(structured)element.RemoveChild(node);else element.ReplaceChild(xml.CreateTextNode(node.Value??""),node);}
                    else if(node is XmlCDataSection)element.ReplaceChild(xml.CreateTextNode(node.Value??""),node);
                    else if(node is XmlElement child)Inspect(child,depth+1);
                    else if(node is not XmlText && node is not XmlComment)throw new InvalidDataException("Unsupported GameObject source node: "+node.NodeType);
            }
        }
        catch(Exception error) when(error is IOException or InvalidDataException or XmlException or XmlSchemaException or ArgumentException or BinaryAssetBuilderException)
        {return new(null,new(Profile,raw,null,Array.Empty<SdkSelfAttributeInheritance.Overlay>(),new[]{error.Message})){DiagnosticNode=SdkXmlDiagnostics.Node(error),DiagnosticLocation=SdkXmlDiagnostics.Origin(error)};}
    }
}
