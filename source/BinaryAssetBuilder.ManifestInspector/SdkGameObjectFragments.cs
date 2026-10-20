using System.Security.Cryptography;
using System.Text.RegularExpressions;
using System.Xml;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: resolve the EA namespace/root/child::* XPointer fragment form with confined source lookup, not arbitrary network XInclude execution.
internal sealed class SdkGameObjectFragments
{
    private const string Ea="uri:ea.com:eala:asset",Xi="http://www.w3.org/2001/XInclude";
    private static readonly Regex Pointer=new("\\A\\s*xmlns\\((?<prefix>[A-Za-z_][A-Za-z0-9_]*)=uri:ea\\.com:eala:asset\\)\\s+xpointer\\(/\\k<prefix>:(?<root>[A-Za-z_][A-Za-z0-9_]*)/child::\\*\\)\\s*\\z",RegexOptions.CultureInvariant,TimeSpan.FromSeconds(1));
    private readonly SdkSourcePathAudit.Report paths;
    private readonly Dictionary<string,ConscriptSourceInput.SourceFile> sources=new(StringComparer.OrdinalIgnoreCase);
    private readonly List<SdkSourceSearch.Selection> searches=new();
    private readonly HashSet<string> active=new(StringComparer.OrdinalIgnoreCase);
    private long bytesRead;
    internal IEnumerable<ConscriptSourceInput.SourceFile> Sources=>sources.Values;
    internal IEnumerable<SdkSourceSearch.Selection> Searches=>searches;

    // Reborn: each fragment session belongs to one already captured source graph and exports its extra source witnesses for final publication checks.
    internal SdkGameObjectFragments(SdkSourcePathAudit.Report paths){this.paths=paths;}

    //-------------------------------------------------------------------------------------------------
    /** Reborn: expand fragments in their defining-document context before inheritance, retaining exact raw hashes and mod-first lookup evidence. */
    //-------------------------------------------------------------------------------------------------
    internal void Expand(XmlDocument document,string path,string root,int depth=0)
    {
        if(depth>16||!active.Add(path))throw new InvalidDataException("XInclude fragment cycle/depth limit: "+path);
        try
        {
            foreach(var include in document.GetElementsByTagName("include",Xi).OfType<XmlElement>().ToArray())
            {
                if(include.Attributes.OfType<XmlAttribute>().Any(a=>a.NamespaceURI!="http://www.w3.org/2000/xmlns/"&&(a.NamespaceURI.Length!=0||a.Name is not ("href" or "xpointer")))||include.ChildNodes.Cast<XmlNode>().Any(n=>n is XmlElement||!string.IsNullOrWhiteSpace(n.InnerText)))throw new InvalidDataException("Unsupported XInclude attributes/fallback: "+path);
                var pointer=Pointer.Match(include.GetAttribute("xpointer"));string href=include.GetAttribute("href");
                if(!pointer.Success||string.IsNullOrWhiteSpace(href))throw new InvalidDataException("Only EA xmlns(prefix=asset) xpointer(/prefix:Root/child::*) fragments are enabled: "+path);
                string selected,authority;
                if(paths.Search!=null){var selection=paths.Search.Select(path,href,root);searches.Add(selection);selected=selection.PhysicalPath;authority=selection.Root;}
                else{var selection=SdkSourcePathAudit.Resolve(href,Path.GetDirectoryName(path)!,root,paths.SourceRoot,paths.ArtRoot,paths.AudioRoot);selected=selection.Path;authority=selection.Root;}
                if(!selected.EndsWith(".xml",StringComparison.OrdinalIgnoreCase))throw new InvalidDataException("XInclude fragment must be XML: "+selected);
                byte[] bytes=SdkEnvironmentPreflight.Read(selected,1048576);bytesRead+=bytes.Length;
                if(bytesRead>4*1048576||sources.Count>=32&&!sources.ContainsKey(selected))throw new InvalidDataException("XInclude fragment source budget exceeded.");
                string hash=Convert.ToHexString(SHA256.HashData(bytes));
                if(sources.TryGetValue(selected,out var old)&&old.Sha256!=hash)throw new InvalidDataException("XInclude fragment changed during preparation: "+selected);
                sources[selected]=new(selected,hash);
                var fragment=new XmlDocument{PreserveWhitespace=true,XmlResolver=null};
                using(var input=new MemoryStream(bytes,false))using(var reader=XmlReader.Create(input,new XmlReaderSettings{DtdProcessing=DtdProcessing.Prohibit,XmlResolver=null,MaxCharactersInDocument=1048576}))fragment.Load(reader);
                if(fragment.DocumentElement?.LocalName!=pointer.Groups["root"].Value||fragment.DocumentElement.NamespaceURI!=Ea)throw new InvalidDataException("XInclude XPointer root does not match fragment: "+selected);
                Expand(fragment,selected,authority,depth+1);
                var children=fragment.DocumentElement.ChildNodes.OfType<XmlElement>().ToArray();
                if(children.Length==0)throw new InvalidDataException("XInclude fragment selects no child elements: "+selected);
                foreach(var child in children)
                {
                    if(child.SelectNodes("descendant-or-self::*/@*")!.OfType<XmlAttribute>().Any(a=>a.Value.StartsWith('=')))throw new InvalidDataException("Definition expressions inside XInclude fragments require additional defining-context support: "+selected);
                    include.ParentNode!.InsertBefore(document.ImportNode(child,true),include);
                }
                include.ParentNode!.RemoveChild(include);
                if(document.SelectNodes("//*")!.Count>8192||document.OuterXml.Length>4*1048576)throw new InvalidDataException("Expanded XInclude document budget exceeded.");
                Console.WriteLine("Reborn: source path [fragment] "+href+" -> "+selected);
            }
        }
        finally{active.Remove(path);}
    }
}
