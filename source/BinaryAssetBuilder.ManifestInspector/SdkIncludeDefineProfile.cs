using System.Security.Cryptography;
using System.Xml;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: inspect only fingerprint-rechecked source-backed all/instance literal definition closures; never infer precompiled/reference visibility.
internal sealed class SdkIncludeDefineProfile
{
    internal const string Name = "diagnostic-include-literals-v1";
    private const string Ea = "uri:ea.com:eala:asset";
    // Reborn: identical-name definitions may coalesce only when they originate from the same captured document, matching core diamond inclusion rules.
    internal sealed record SourceIdentity(string SourcePath,string Sha256);
    internal sealed record Origin(string Name,string SourcePath);
    private sealed record Literal(string Value,string SourcePath);
    private readonly SdkSourcePathAudit.Report paths;
    private readonly Dictionary<string,SdkSourcePathAudit.Source> inventory = new(StringComparer.OrdinalIgnoreCase);
    private readonly Dictionary<string,byte[]> snapshots = new(StringComparer.OrdinalIgnoreCase);
    private long snapshotBytes;
    // Reborn: optional definition syntax admission is a separate explicit profile; the existing literal-only constructor remains the default.
    private readonly bool definitionExpressions;
    internal string Profile => definitionExpressions ? SdkDefinitionSubset.Name : Name;

    //-------------------------------------------------------------------------------------------------
    /** Reborn: validate every captured source's canonical confinement before any imported-document read; forged graph paths cannot create authority. */
    //-------------------------------------------------------------------------------------------------
    internal SdkIncludeDefineProfile(SdkSourcePathAudit.Report paths,bool definitionExpressions = false)
    {
        this.paths = paths;
        this.definitionExpressions = definitionExpressions;
        if (paths.Sources.Length > 512 || paths.Includes.Length > 4096) throw new InvalidDataException("Bounded Include inventory required.");
        string[] roots = new[] { paths.SourceRoot,paths.ArtRoot,paths.AudioRoot }.Where(root => root != null).Cast<string>().Select(SdkEnvironmentPreflight.DirectoryPath).ToArray();
        foreach (var source in paths.Sources)
        {
            string canonical = SdkEnvironmentPreflight.Absolute(source.PhysicalPath);
            if (!canonical.Equals(source.PhysicalPath,StringComparison.OrdinalIgnoreCase) || !roots.Contains(source.Root,StringComparer.OrdinalIgnoreCase)
                || !SdkEnvironmentPreflight.Inside(source.Root,canonical) || !inventory.TryAdd(canonical,source)) throw new InvalidDataException("Ambiguous or escaped Include source inventory.");
        }
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: build an atomic child-first literal table from the existing graph only, then substitute the owner's captured XML without source writes. */
    //-------------------------------------------------------------------------------------------------
    internal SdkLocalDefineProfile.Result Apply(string path,byte[] bytes)
    {
        string raw = Convert.ToHexString(SHA256.HashData(bytes));
        try
        {
            if (paths.StoppedAtLimit) throw new InvalidDataException("Incomplete path inventory cannot authorize imported definitions.");
            Capture(path,bytes);
            var local = SdkLocalDefineProfile.Apply(bytes);
            // Reborn: an asset with no expressions requires no definition-closure claim and keeps its original bytes.
            if (local.Bytes != null && local.Evidence.Substitutions == 0) return local with { Evidence = local.Evidence with { Profile = Profile } };
            Dictionary<string,Dictionary<string,Literal>> tables = new(StringComparer.OrdinalIgnoreCase);
            HashSet<string> active = new(StringComparer.OrdinalIgnoreCase);
            Dictionary<string,SourceIdentity> witnesses = new(StringComparer.OrdinalIgnoreCase);
            int edges = 0;
            // Reborn: deduplicate computations by origin/name across diamond paths without publishing partial results on rejection.
            Dictionary<string,SdkDefinitionSubset.Evaluation> evaluations = new(StringComparer.Ordinal);
            var table = Visit(path,0);
            var result = SdkLocalDefineProfile.ApplyImported(bytes,table.ToDictionary(pair => pair.Key,pair => pair.Value.Value,StringComparer.Ordinal));
            // Reborn: rejected transformations never publish origins or a partial closure as trusted preprocessing evidence.
            return result with { Evidence = result.Evidence with { Profile = Profile,
                EvaluatedDefinitions = result.Bytes == null ? Array.Empty<SdkDefinitionSubset.Evaluation>() : evaluations.Values.OrderBy(item => item.SourcePath,StringComparer.OrdinalIgnoreCase).ThenBy(item => item.Name,StringComparer.Ordinal).ToArray(),
                DefinitionSources = result.Bytes == null ? Array.Empty<SourceIdentity>() : witnesses.Values.OrderBy(item => item.SourcePath,StringComparer.OrdinalIgnoreCase).ToArray(),
                DefinitionOrigins = result.Bytes == null ? Array.Empty<Origin>() : table.OrderBy(pair => pair.Key,StringComparer.Ordinal).Select(pair => new Origin(pair.Key,pair.Value.SourcePath)).ToArray() } };

            //-------------------------------------------------------------------------------------------------
            /** Reborn: preserve direct Include order, prove edge identities against raw XML, and reject stale/missing/cyclic/reference/duplicate closures. */
            //-------------------------------------------------------------------------------------------------
            Dictionary<string,Literal> Visit(string current,int depth)
            {
                if (depth > 32 || active.Contains(current)) throw new InvalidDataException("Include definition cycle/depth bound exceeded.");
                if (tables.TryGetValue(current,out var previous)) return previous;
                if (witnesses.Count >= 512) throw new InvalidDataException("512 definition-source bound exceeded.");
                byte[] captured = Capture(current);
                witnesses.Add(current,new(current,inventory[current].Sha256)); active.Add(current);
                XmlDocument xml = new() { XmlResolver = null };
                using (MemoryStream input = new(captured,false))
                using (XmlReader reader = XmlReader.Create(input,new XmlReaderSettings { DtdProcessing = DtdProcessing.Prohibit,XmlResolver = null,MaxCharactersInDocument = 4*1048576 })) xml.Load(reader);
                XmlElement? root = xml.DocumentElement;
                if (root?.LocalName != "AssetDeclaration" || root.NamespaceURI != Ea) throw new InvalidDataException("EA definition source required.");
                if (root.SelectNodes(".//*")!.OfType<XmlElement>().Any(element => element.NamespaceURI == Ea && element.HasAttribute("inheritFrom"))) throw new InvalidDataException("Inherited definition source requires asset preprocessing.");
                var containers = root.ChildNodes.OfType<XmlElement>().Where(element => element.LocalName == "Includes").ToArray();
                if (containers.Length > 1 || containers.Any(element => element.NamespaceURI != Ea)
                    || root.ChildNodes.OfType<XmlElement>().Any(element => element.LocalName == "Defines" && element.NamespaceURI != Ea)) throw new InvalidDataException("Unsupported definition/Include container shape.");
                var includes = containers.SelectMany(element => element.ChildNodes.OfType<XmlElement>()).ToArray();
                var expected = paths.Includes.Where(edge => edge.Document.Equals(current,StringComparison.OrdinalIgnoreCase)).ToArray();
                if (includes.Length != expected.Length) throw new InvalidDataException("Captured Include edges do not match raw definition source.");
                Dictionary<string,Literal> merged = new(StringComparer.Ordinal);
                for (int index = 0; index < includes.Length; index++)
                {
                    if (++edges > 4096) throw new InvalidDataException("4096 definition-edge bound exceeded.");
                    XmlElement include = includes[index]; var edge = expected[index];
                    string kind = include.GetAttribute("type").ToLowerInvariant(),logical = include.GetAttribute("source");
                    if (include.LocalName != "Include" || include.NamespaceURI != Ea || kind is not ("all" or "instance")
                        || include.HasChildNodes || include.Attributes.OfType<XmlAttribute>().Any(attribute => attribute.Name is not ("source" or "type"))
                        || edge.Kind != kind || edge.LogicalPath != logical || edge.PhysicalPath == null) throw new InvalidDataException("Only captured source-backed all/instance definition Includes are admitted; reference/precompiled paths remain closed.");
                    var resolved = SdkSourcePathAudit.Resolve(logical,Path.GetDirectoryName(current)!,inventory[current].Root,paths.SourceRoot,paths.ArtRoot,paths.AudioRoot);
                    if (!resolved.Path.Equals(edge.PhysicalPath,StringComparison.OrdinalIgnoreCase) || !inventory.TryGetValue(resolved.Path,out var child)
                        || !resolved.Root.Equals(child.Root,StringComparison.OrdinalIgnoreCase)) throw new InvalidDataException("Include definition target differs from captured confined inventory.");
                    foreach (var pair in Visit(resolved.Path,depth+1)) Merge(merged,pair.Key,pair.Value);
                }
                // Reborn: derive declaration order from XML, not dictionary enumeration, before admitting backward-only definition expressions.
                var localDefinitions = SdkLocalDefineProfile.ReadLiteralDefinitions(root,definitionExpressions);
                var orderedNames = root.ChildNodes.OfType<XmlElement>().Where(element => element.NamespaceURI == Ea && element.LocalName == "Defines")
                    .SelectMany(element => element.ChildNodes.OfType<XmlElement>()).Select(element => element.GetAttribute("name"));
                foreach (string localName in orderedNames)
                {
                    var pair = new KeyValuePair<string,string>(localName,localDefinitions[localName]);
                    // Reborn: no local override support; imported/local name collisions are rejected even if their literal values happen to match.
                    if (merged.ContainsKey(pair.Key)) throw new InvalidDataException("Local definition collides with imported definition; overrides remain closed.");
                    // Reborn: evaluate each local definition in declaration order against imported and earlier local values, never forward names.
                    string value = pair.Value;
                    if (value[0] == '=')
                    {
                        value = SdkDefinitionSubset.Evaluate(value,name => merged.TryGetValue(name,out var literal) ? literal.Value : null);
                        if (evaluations.Count >= 2048) throw new InvalidDataException("2048 evaluated-definition bound exceeded.");
                        evaluations.Add(current+"\0"+pair.Key,new(pair.Key,current,pair.Value,value));
                    }
                    Merge(merged,pair.Key,new(value,current));
                }
                active.Remove(current); tables.Add(current,merged); return merged;
            }
        }
        catch (Exception error) when (error is IOException or InvalidDataException or XmlException or ArgumentException or UnauthorizedAccessException or NotSupportedException)
        { return new(null,new(Profile,raw,null,0,new[] { "Imported definition closure rejected: "+error.Message[..Math.Min(error.Message.Length,512)] })); }
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: bound the complete visible literal table and reject cross-document duplicates while coalescing a shared diamond leaf. */
    //-------------------------------------------------------------------------------------------------
    private static void Merge(Dictionary<string,Literal> table,string name,Literal literal)
    {
        if (table.TryGetValue(name,out var other))
        { if (other.SourcePath.Equals(literal.SourcePath,StringComparison.OrdinalIgnoreCase) && other.Value == literal.Value) return; throw new InvalidDataException("Duplicate imported definition from different source documents."); }
        if (table.Count >= 512) throw new InvalidDataException("512 visible definition bound exceeded.");
        table.Add(name,literal);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: cache only bounded, confined bytes whose size/hash match path-audit evidence; missing or changed imported sources never fall back to disk guesses. */
    //-------------------------------------------------------------------------------------------------
    private byte[] Capture(string path,byte[]? ownerBytes = null)
    {
        if (!inventory.TryGetValue(path,out var source)) throw new InvalidDataException("Definition source is absent from captured inventory.");
        if (ownerBytes == null && snapshots.TryGetValue(path,out var previous)) return previous;
        byte[] bytes = ownerBytes ?? SdkEnvironmentPreflight.Read(path,4*1048576);
        if (bytes.Length > 4*1048576 || bytes.Length != source.Bytes || Convert.ToHexString(SHA256.HashData(bytes)) != source.Sha256) throw new InvalidDataException("Stale definition source differs from path snapshot.");
        if (!snapshots.ContainsKey(path))
        {
            if (snapshotBytes+bytes.Length > 32*1048576) throw new InvalidDataException("32 MiB definition snapshot bound exceeded.");
            snapshotBytes += bytes.Length; snapshots.Add(path,bytes);
        }
        return snapshots[path];
    }
}
