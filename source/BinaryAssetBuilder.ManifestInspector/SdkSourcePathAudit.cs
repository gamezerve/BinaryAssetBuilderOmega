using System.Security.Cryptography;
using System.Xml;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: audit only reachable source Include paths, explicit ART/AUDIO literals and AudioFile File attributes; never compile or dump asset payloads.
internal static class SdkSourcePathAudit
{
    // Reborn: metadata existence is not payload correctness, schema validity or a production build-ready result.
    internal sealed record Source(string PhysicalPath,string Root,string Sha256,long Bytes);
    internal sealed record Edge(string Document,string Kind,string LogicalPath,string? PhysicalPath);
    internal sealed record Resource(string Document,string Field,string LogicalPath,string PhysicalPath,long Bytes);
    // Reborn: coordinates describe the actual caller/source snapshot, not an assumed line in a missing target file.
    internal sealed record Issue(string Code,string Document,string LogicalPath,string Detail){public SdkXmlDiagnostics.Location? Location {get;init;}}
    internal sealed record Report(string SourceRoot,string? ArtRoot,string? AudioRoot,Source[] Sources,Edge[] Includes,Resource[] Resources,
        Issue[] Issues,bool ScopedPathAuditComplete,bool StoppedAtLimit,bool SnapshotOnly,bool FullDependencyCoverage,bool ProductionBuildReady,string[] Limitations)
    {
        // Reborn: exact opt-in aliases preserve original edge authority; older audits retain no alias witnesses.
        public SdkKnownMapAliases.Witness[] KnownMapAliases { get; init; } = Array.Empty<SdkKnownMapAliases.Witness>();
        // Reborn: ordered search is opt-in; old diagnostic callers retain strict single-root semantics.
        public SdkSourceSearch? Search { get; init; }
        public SdkSourceSearch.Selection[] Searches { get; init; } = Array.Empty<SdkSourceSearch.Selection>();
        public string[] AllowedRoots => Search?.Roots ?? new[] { SourceRoot,ArtRoot,AudioRoot }.Where(r=>r!=null).Cast<string>().ToArray();
        public string PathProfile => Search!=null ? "mod-over-sdk-aliases-v1" : KnownMapAliases.Length == 0 ? "strict-source-paths" : SdkKnownMapAliases.Name;
    }
    // Reborn: carry relative-path confinement with a resolved physical path; overlapping roots are not permitted.
    internal sealed record Resolved(string Path,string Root);

    //-------------------------------------------------------------------------------------------------
    /** Reborn: inspect a bounded reachable path graph from explicit environment evidence without recursive directory scans or source writes. */
    //-------------------------------------------------------------------------------------------------
    internal static Report Inspect(SdkEnvironmentPreflight.Report environment,string? artRoot = null,string? audioRoot = null,bool knownMapAliases = false,SdkSourceSearch? search = null,bool w3xSources = false)
    {
        string data = SdkEnvironmentPreflight.DirectoryPath(environment.SourceRoot);
        artRoot = search?.ModArt ?? (artRoot == null ? null : SdkEnvironmentPreflight.DirectoryPath(artRoot));
        audioRoot = search?.ModAudio ?? (audioRoot == null ? null : SdkEnvironmentPreflight.DirectoryPath(audioRoot));
        if(search!=null && !search.ModData.Equals(data,StringComparison.OrdinalIgnoreCase))throw new InvalidDataException("Search profile data root differs from source entry authority.");
        string[] roots = search?.Roots ?? new[] { data,artRoot,audioRoot }.Where(root => root != null).Cast<string>().ToArray();
        for (int index = 0; index < roots.Length; index++)
        {
            if (SdkEnvironmentPreflight.Inside(roots[index],environment.OutputDirectory)) throw new InvalidDataException("SDK output cannot be inside an audited source/art/audio root.");
            for (int other = index+1; other < roots.Length; other++)
                if (roots[index].Equals(roots[other],StringComparison.OrdinalIgnoreCase) || SdkEnvironmentPreflight.Inside(roots[index],roots[other]) || SdkEnvironmentPreflight.Inside(roots[other],roots[index]))
                    throw new InvalidDataException("SDK source/art/audio roots must be distinct and non-overlapping for this attribution profile.");
        }
        List<Source> sources = new(); List<Edge> edges = new(); List<Resource> resources = new(); List<Issue> issues = new();
        // Reborn: retain only already bounded source reads for locating path/parse diagnostics; no later read can silently change their coordinates.
        var rawSnapshots=new Dictionary<string,byte[]>(StringComparer.OrdinalIgnoreCase);
        // Reborn: every resolved include/resource records its search winner and earlier candidates for later replay.
        List<SdkSourceSearch.Selection> searches = new();
        // Reborn: cache at most two exact recipe proofs; repeated edges do not multiply source reads.
        List<SdkKnownMapAliases.Witness> aliases = new();
        HashSet<string> visited = new(StringComparer.OrdinalIgnoreCase),active = new(StringComparer.OrdinalIgnoreCase);
        long total = 0; bool stopped = false;
        Visit(environment.SourceEntry,data,0);
        if (sources.Count > 0 && sources[0].Sha256 != environment.SourceEntrySha256) Add("StaleEntry",environment.SourceEntry,"","Source entry changed after environment inspection.");
        // Reborn: no alias witness survives an uncaptured, changed or invalid target; the source graph must include its proved bytes.
        foreach (var alias in aliases)
            if (!sources.Any(source => source.PhysicalPath.Equals(alias.PhysicalPath,StringComparison.OrdinalIgnoreCase) && source.Sha256 == alias.Sha256 && source.Bytes == alias.Bytes)
                || Convert.ToHexString(SHA256.HashData(SdkEnvironmentPreflight.Read(alias.PhysicalPath,4*1048576))) != alias.Sha256)
                throw new InvalidDataException("Known map alias target changed or was not captured as valid source XML.");
        return new(data,artRoot,audioRoot,sources.ToArray(),edges.ToArray(),resources.ToArray(),issues.ToArray(),issues.Count == 0 && !stopped,
            stopped,true,false,false,new[] { "Path scope only: all/instance/reference Include edges are inspected, not compiled or substituted with manifests.",
                "Explicit ART/AUDIO attribute and leaf-text literals plus AudioFile File attributes only; no schema-typed complete file dependency inventory.",
                "Resource existence/length only: compressed/audio/art bodies are not read or SHA-fingerprinted.",
                "ROOT alias, expressions/macros, registry discovery, postfix/LOD variants and overlapping roots are outside this profile; ordered search requires explicit opt-in.",
                "Snapshot-only with bounded current reads; no atomic concurrent-filesystem transaction or production/game-load readiness." }) { KnownMapAliases = aliases.ToArray(),Search=search,Searches=searches.ToArray() };

        //-------------------------------------------------------------------------------------------------
        /** Reborn: cap retained diagnostics and stop traversal instead of silently treating a partial graph as complete. */
        //-------------------------------------------------------------------------------------------------
        void Add(string code,string document,string logical,string detail,SdkXmlDiagnostics.Location? location=null)
        {
            if (issues.Count >= 127) { if (!stopped) issues.Add(new("IssueLimit",document,"","128-issue cap reached; traversal stopped.")); stopped = true; return; }
            location??=Locate(document,logical);
            issues.Add(new(code,document,logical.Length > 512 ? logical[..512] : logical,detail){Location=location});
        }

        // Reborn: an absent/unreadable target has no XML line; preserve its caller's captured Include coordinate instead.
        SdkXmlDiagnostics.Location? Locate(string document,string logical)
        {
            SdkXmlDiagnostics.Location? location=null;
            // Reborn: identify a unique authored literal where possible; ambiguous path literals retain an explicitly labelled document context.
            if(rawSnapshots.TryGetValue(document,out var raw))
            {
                try{var captured=SdkXmlDiagnostics.Lines(raw,document);var matches=captured.Descendants().SelectMany(e=>e.Attributes().Cast<System.Xml.Linq.XObject>().Concat(!e.HasElements?new[]{(System.Xml.Linq.XObject)e}:Array.Empty<System.Xml.Linq.XObject>())).Where(n=>(n is System.Xml.Linq.XAttribute a?a.Value:((System.Xml.Linq.XElement)n).Value)==logical&&logical.Length!=0).ToArray();if(matches.Length==1)location=SdkXmlDiagnostics.Point(document,matches[0]);else if(captured.Root!=null)location=SdkXmlDiagnostics.Point(document,captured.Root,true);}
                catch(XmlException syntax){location=SdkXmlDiagnostics.Origin(syntax);}
            }
            return location;
        }

        //-------------------------------------------------------------------------------------------------
        /** Reborn: read each reachable XML once, preserve all edge roles/order and detect active cycles before shared-node reuse. */
        //-------------------------------------------------------------------------------------------------
        void Visit(string path,string confinement,int depth,Func<SdkXmlDiagnostics.Location?>? caller=null)
        {
            if (stopped) return;
            if (active.Contains(path)) { Add("IncludeCycle",path,"","Active source Include cycle; this planning profile does not compile reference-cycle semantics."); return; }
            if (visited.Contains(path)) return;
            if (depth > 32 || sources.Count >= 512) { Add("GraphLimit",path,"","32-depth/512-source bound exceeded."); stopped = true; return; }
            byte[] bytes; XmlDocument xml = new() { XmlResolver = null };
            try
            {
                bytes = SdkEnvironmentPreflight.Read(path,4*1048576); total += bytes.Length;
                if (total > 32*1048576) { Add("GraphLimit",path,"","32 MiB source-byte cap reached."); stopped = true; return; }
                rawSnapshots[path]=bytes;xml=SdkXmlDiagnostics.Parse(bytes,path);
                if (xml.DocumentElement?.LocalName != "AssetDeclaration" || xml.DocumentElement.NamespaceURI != "uri:ea.com:eala:asset") throw new InvalidDataException("EA AssetDeclaration required for Include documents.");
            }
            catch (Exception error) when (error is IOException or InvalidDataException or XmlException or ArgumentException)
            { Add("SourceRead",path,"","Missing, oversized, redirected or malformed source XML; no external entity processing. "+error.Message,SdkXmlDiagnostics.Origin(error)??caller?.Invoke()); return; }
            visited.Add(path); active.Add(path); sources.Add(new(path,confinement,Convert.ToHexString(SHA256.HashData(bytes)),bytes.Length));
            XmlElement root = xml.DocumentElement!;
            foreach (XmlElement container in root.ChildNodes.OfType<XmlElement>().Where(node => node.LocalName == "Includes"))
            {
                if (container.NamespaceURI != root.NamespaceURI) { Add("IncludeShape",path,"","Foreign Include container."); continue; }
                foreach (XmlElement include in container.ChildNodes.OfType<XmlElement>())
                {
                    if (stopped) break;
                    if (edges.Count >= 4096) { Add("GraphLimit",path,"","4096-Include-edge cap reached."); stopped = true; break; }
                    string logical = include.GetAttribute("source"),kind = include.GetAttribute("type").ToLowerInvariant();
                    if (include.LocalName != "Include" || include.NamespaceURI != root.NamespaceURI || kind is not ("all" or "instance" or "reference"))
                    { Add("IncludeShape",path,logical,"Only direct all/instance/reference Include elements are audited."); continue; }
                    try
                    {
                        // Reborn: opt in only to exact two-library/all aliases, keeping original logical spelling and role intact.
                        var alias = knownMapAliases ? aliases.SingleOrDefault(item => item.Document.Equals(path,StringComparison.OrdinalIgnoreCase) && item.LogicalPath == logical && item.Kind == kind)
                            ?? SdkKnownMapAliases.Prove(data,path,kind,logical) : null;
                        if (alias != null && !aliases.Contains(alias)) { if (aliases.Count >= 2) throw new InvalidDataException("Two known map alias proof bound exceeded."); aliases.Add(alias); }
                        Resolved child = alias == null ? ResolveSelected(path,logical,confinement) : new(alias.PhysicalPath,data);
                        if (!child.Path.EndsWith(".xml",StringComparison.OrdinalIgnoreCase) && !child.Path.EndsWith(".w3x",StringComparison.OrdinalIgnoreCase)) throw new InvalidDataException("Include must name XML/W3X source.");
                        edges.Add(new(path,kind,logical,child.Path));
                        // Reborn: production W3X declarations join the captured XML graph; legacy path-only audits retain metadata-only resource reads.
                        if(!w3xSources && search!=null && child.Path.EndsWith(".w3x",StringComparison.OrdinalIgnoreCase))
                        {if(resources.Count>=2048){Add("ResourceLimit",path,logical,"2048-resource cap reached.");stopped=true;}else resources.Add(new(path,"Include",logical,child.Path,new FileInfo(child.Path).Length));}
                        else Visit(child.Path,child.Root,depth+1,()=>Locate(path,logical));
                    }
                    catch (Exception error) when (error is IOException or InvalidDataException or ArgumentException or NotSupportedException)
                    { edges.Add(new(path,kind,logical,null)); Add("IncludePath",path,logical,search==null?"Unsupported/missing alias root, unsafe path or non-XML Include; no fallback attempted.":error.Message[..Math.Min(error.Message.Length,2048)]); }
                }
            }
            foreach (XmlElement element in root.SelectNodes("descendant-or-self::*")!)
            {
                if (stopped) break;
                if (element.NamespaceURI != root.NamespaceURI || element.LocalName == "Include") continue;
                foreach (XmlAttribute attribute in element.Attributes)
                    // Reborn: Texture File is a real file reference even without ART:, relative to its declaring XML like AudioFile File.
                    if (attribute.NamespaceURI.Length == 0 && (Alias(attribute.Value) || (element.LocalName is "AudioFile" or "Texture") && attribute.Name == "File")) ResourcePath(path,element.LocalName+"/@"+attribute.Name,attribute.Value,confinement);
                if (!element.ChildNodes.OfType<XmlElement>().Any() && Alias(element.InnerText)) ResourcePath(path,element.LocalName+"/text()",element.InnerText,confinement);
            }
            active.Remove(path);
        }

        //-------------------------------------------------------------------------------------------------
        /** Reborn: stat only explicitly scoped resource paths; do not read arbitrary asset payloads or infer untyped dependency fields. */
        //-------------------------------------------------------------------------------------------------
        void ResourcePath(string document,string field,string logical,string confinement)
        {
            // Reborn: retain the global graph bound even when an element contains many resource attributes.
            if (stopped) return;
            if (resources.Count >= 2048) { Add("ResourceLimit",document,logical,"2048-resource cap reached."); stopped = true; return; }
            try
            {
                Resolved resolved = ResolveSelected(document,logical,confinement);
                SdkEnvironmentPreflight.CheckPath(resolved.Path);
                if (!File.Exists(resolved.Path)) { Add("ResourceMissing",document,logical,"Scoped resource file is absent; no codec/art/native payload read."); return; }
                resources.Add(new(document,field,logical,resolved.Path,new FileInfo(resolved.Path).Length));
            }
            catch (Exception error) when (error is IOException or InvalidDataException or ArgumentException or NotSupportedException)
            { Add("ResourcePath",document,logical,search==null?"Unsupported/missing alias root or unsafe scoped resource path; no fallback attempted.":error.Message[..Math.Min(error.Message.Length,2048)]); }
        }

        //-------------------------------------------------------------------------------------------------
        /** Reborn: strict callers remain unchanged; SDK callers retain a trace of the first existing confined candidate. */
        //-------------------------------------------------------------------------------------------------
        Resolved ResolveSelected(string document,string logical,string confinement)
        {
            if(search==null)return Resolve(logical,Path.GetDirectoryName(document)!,confinement,data,artRoot,audioRoot);
            var selection=search.Select(document,logical,confinement);searches.Add(selection);return new(selection.PhysicalPath,selection.Root);
        }
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: match only literal explicit ART/AUDIO references, not AudioFile asset IDs or arbitrary schema-untyped text. */
    //-------------------------------------------------------------------------------------------------
    private static bool Alias(string value) => value.StartsWith("ART:",StringComparison.OrdinalIgnoreCase) || value.StartsWith("AUDIO:",StringComparison.OrdinalIgnoreCase);

    //-------------------------------------------------------------------------------------------------
    /** Reborn: resolve confined relative/DATA paths, ART basename two-character fanout and AUDIO paths without registry/postfix/search fallback. */
    //-------------------------------------------------------------------------------------------------
    internal static Resolved Resolve(string logical,string parent,string confinement,string data,string? art,string? audio)
    {
        if (logical.Length is < 1 or > 512 || logical != logical.Trim() || logical.Any(value => char.IsControl(value) || ";$%*?".Contains(value))) throw new InvalidDataException("Scoped source paths must be bounded literal values.");
        string relative = logical.Replace('/',Path.DirectorySeparatorChar).Replace('\\',Path.DirectorySeparatorChar),root = confinement,basePath = parent;
        int colon = relative.IndexOf(':');
        if (colon >= 0)
        {
            string alias = relative[..colon].ToLowerInvariant(); relative = relative[(colon+1)..];
            root = alias switch { "data" => data,"art" => art ?? throw new InvalidDataException("Explicit ART root required."),"audio" => audio ?? throw new InvalidDataException("Explicit AUDIO root required."),_ => throw new NotSupportedException("Alias outside scoped path profile.") };
            basePath = root;
            if (alias == "art" && relative.IndexOf(Path.DirectorySeparatorChar) < 0)
            { if (relative.Length < 2 || relative is "." or "..") throw new InvalidDataException("ART basename requires its two-character folder."); basePath = Path.Combine(root,relative[..2]); }
        }
        if (relative.Length == 0 || Path.IsPathRooted(relative) || relative.Contains(':') || relative.Split(Path.DirectorySeparatorChar).Any(part => part.Length == 0 || part.EndsWith(' ') || part.EndsWith('.') && part is not ("." or ".."))) throw new InvalidDataException("Unsafe scoped relative dependency path.");
        string path = SdkEnvironmentPreflight.Absolute(Path.Combine(basePath,relative));
        if (!SdkEnvironmentPreflight.Inside(root,path)) throw new InvalidDataException("Dependency path escaped its explicit root.");
        SdkEnvironmentPreflight.CheckPath(path); return new(path,root);
    }
}
