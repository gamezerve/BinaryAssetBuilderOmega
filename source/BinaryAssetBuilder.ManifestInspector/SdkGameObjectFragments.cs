using System.Security.Cryptography;
using System.Text.RegularExpressions;
using System.Xml;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: resolve EA whole-root and child::* XInclude forms with confined source lookup, not arbitrary network execution.
internal sealed class SdkGameObjectFragments
{
    private const string Ea="uri:ea.com:eala:asset",Xi="http://www.w3.org/2001/XInclude";
    private static readonly Regex Pointer=new("\\A\\s*xmlns\\((?<prefix>[A-Za-z_][A-Za-z0-9_]*)=uri:ea\\.com:eala:asset\\)\\s+xpointer\\(/\\k<prefix>:(?<root>[A-Za-z_][A-Za-z0-9_]*)/child::\\*\\)\\s*\\z",RegexOptions.CultureInvariant,TimeSpan.FromSeconds(1));
    private readonly SdkSourcePathAudit.Report paths;
    private readonly Dictionary<string,ConscriptSourceInput.SourceFile> sources=new(StringComparer.OrdinalIgnoreCase);
    // Reborn: raw captured fragment bytes retain original coordinates after imports and inheritance detach their DOM nodes.
    private readonly Dictionary<string,byte[]> snapshots=new(StringComparer.OrdinalIgnoreCase);
    internal IEnumerable<SdkXmlDiagnostics.Snapshot> Snapshots=>snapshots.Select(s=>new SdkXmlDiagnostics.Snapshot(s.Key,s.Value));
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
                // Reborn: rejected fragment directives retain the actual including node through the preparation result wrapper.
                try{
                if(include.Attributes.OfType<XmlAttribute>().Any(a=>a.NamespaceURI!="http://www.w3.org/2000/xmlns/"&&(a.NamespaceURI.Length!=0||a.Name is not ("href" or "xpointer" or "parse")))||include.ChildNodes.Cast<XmlNode>().Any(n=>n is XmlElement||!string.IsNullOrWhiteSpace(n.InnerText)))throw new InvalidDataException("Unsupported XInclude attributes/fallback: "+path);
                // Reborn: an absent XPointer inserts the document element itself, not its children; explicit pointers retain their existing grammar.
                bool wholeRoot=!include.HasAttribute("xpointer");
                var pointer=Pointer.Match(include.GetAttribute("xpointer"));string href=include.GetAttribute("href");
                if(include.HasAttribute("parse")&&include.GetAttribute("parse")!="xml")throw new InvalidDataException("Only parse=xml XInclude is supported: "+path);
                if((!wholeRoot&&!pointer.Success)||string.IsNullOrWhiteSpace(href))throw new InvalidDataException("XInclude requires href and either no xpointer or EA xmlns(prefix=asset) xpointer(/prefix:Root/child::*) syntax: "+path);
                string selected,authority;
                if(paths.Search!=null){var selection=paths.Search.Select(path,href,root);searches.Add(selection);selected=selection.PhysicalPath;authority=selection.Root;}
                else{var selection=SdkSourcePathAudit.Resolve(href,Path.GetDirectoryName(path)!,root,paths.SourceRoot,paths.ArtRoot,paths.AudioRoot);selected=selection.Path;authority=selection.Root;}
                if(!selected.EndsWith(".xml",StringComparison.OrdinalIgnoreCase))throw new InvalidDataException("XInclude fragment must be XML: "+selected);
                byte[] bytes=SdkEnvironmentPreflight.Read(selected,1048576);bytesRead+=bytes.Length;
                if(bytesRead>4*1048576||sources.Count>=32&&!sources.ContainsKey(selected))throw new InvalidDataException("XInclude fragment source budget exceeded.");
                string hash=Convert.ToHexString(SHA256.HashData(bytes));
                if(sources.TryGetValue(selected,out var old)&&old.Sha256!=hash)throw new InvalidDataException("XInclude fragment changed during preparation: "+selected);
                sources[selected]=new(selected,hash);
                snapshots[selected]=bytes;
                var fragment=SdkXmlDiagnostics.Parse(bytes,selected);
                if(fragment.DocumentElement==null||fragment.DocumentElement.NamespaceURI!=Ea||(!wholeRoot&&fragment.DocumentElement.LocalName!=pointer.Groups["root"].Value))throw new InvalidDataException("XInclude root namespace or XPointer root does not match fragment: "+selected);
                Expand(fragment,selected,authority,depth+1);
                // Reborn: preserve module attributes and empty root modules for the whole-document EA include form.
                var children=wholeRoot?new[]{fragment.DocumentElement}:fragment.DocumentElement.ChildNodes.OfType<XmlElement>().ToArray();
                if(children.Length==0)throw new InvalidDataException("XInclude fragment selects no child elements: "+selected);
                foreach(var child in children)
                {
                    if(child.SelectNodes("descendant-or-self::*/@*")!.OfType<XmlAttribute>().Any(a=>a.Value.StartsWith('=')))throw new InvalidDataException("Definition expressions inside XInclude fragments require additional defining-context support: "+selected);
                    include.ParentNode!.InsertBefore(document.ImportNode(child,true),include);
                }
                include.ParentNode!.RemoveChild(include);
                if(document.SelectNodes("//*")!.Count>8192||document.OuterXml.Length>4*1048576)throw new InvalidDataException("Expanded XInclude document budget exceeded.");
                Console.WriteLine("Reborn: source path [fragment] "+href+" -> "+selected);
                }catch(Exception error)when(error is IOException or InvalidDataException or XmlException or ArgumentException){SdkXmlDiagnostics.At(error,include);throw;}
            }
        }
        finally{active.Remove(path);}
    }
}
