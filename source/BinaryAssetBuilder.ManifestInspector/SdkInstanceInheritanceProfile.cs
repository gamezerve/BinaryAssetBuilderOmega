using System.Security.Cryptography;
using System.Text;
using System.Xml;
using System.Xml.Schema;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: admit rechecked direct-instance bases with separately selected file-free or explicit-alias scopes; transitive/relative-file provenance remains closed.
internal sealed class SdkInstanceInheritanceProfile
{
    internal const string Name = "diagnostic-direct-instance-inheritance-v1";
    // Reborn: root-qualified imported files are separately explicit; relative defining-document paths remain closed.
    internal const string RootFileName = "diagnostic-direct-instance-root-files-v1";
    // Reborn: recursive preparation is independent of older direct-only scopes and does not flatten transitive declarations into direct visibility.
    internal const string ChainName = "diagnostic-direct-instance-chains-v1";
    // Reborn: keyed empty-child removals are explicit and cannot silently widen the previous chain profile.
    internal const string RemovalName = "diagnostic-direct-instance-removals-v1";
    // Reborn: repeated-choice copy preparation is separately explicit and cannot widen the removal-only profile.
    internal const string ChoiceName = "diagnostic-direct-instance-choice-copy-v1";
    // Reborn: consumed pipeline markers have their own diagnostic option; imported non-inheritable types remain closed.
    internal const string MarkerName = "diagnostic-direct-instance-markers-v1";
    // Reborn: whole-token-proven KindOf modifiers have an independent scope and do not widen marker-only preparation.
    internal const string BitflagName = "diagnostic-direct-instance-bitflags-v1";
    // Reborn: identify every captured/prepared document contributing to an admitted closure, not native hashes or live-disk state.
    internal sealed record PreparedSource(string SourcePath,string RawSha256,string ProcessedSha256);
    private const string Ea = "uri:ea.com:eala:asset";
    // Reborn: source and processed-document hashes identify imported XML witnesses, never native streams or cache identities.
    internal sealed record ImportedBase(string Type,string BaseId,string SourcePath,string RawSha256,string ProcessedSha256)
    {
        // Reborn: these are selected pre-overlay base fields, not a claim that each survives a derived override or resolves to an existing payload.
        public SdkEffectiveSchema.BoundField[] RootQualifiedFields { get; init; } = Array.Empty<SdkEffectiveSchema.BoundField>();
        // Reborn: the direct base's own preparation closure remains explicit when recursive source preparation is selected.
        public PreparedSource[] PreparedSources { get; init; } = Array.Empty<PreparedSource>();
    }
    // Reborn: distinguish independent alias-root admission from the earlier zero-imported-file-field contract.
    private readonly bool rootFiles;
    // Reborn: all chain state is scoped/reset per public owner invocation, preventing a previous owner's prepared cache from concealing stale inputs.
    private readonly bool chains;
    // Reborn: select only the separately tested literal-removal subset during each child-first preparation.
    private readonly bool removals;
    // Reborn: retain nested repeated-choice admission consistently throughout the child-first source closure.
    private readonly bool choices;
    // Reborn: apply source-local marker consumption throughout child-first preparation without relaxing imported-base eligibility.
    private readonly bool markers;
    // Reborn: retain modifier proof and source-owner evidence across child-first preparation.
    private readonly bool bitflags;
    private sealed record Prepared(SdkSelfAttributeInheritance.Result Result,int Height);
    private readonly Dictionary<string,Prepared> prepared = new(StringComparer.OrdinalIgnoreCase);
    private readonly HashSet<string> active = new(StringComparer.OrdinalIgnoreCase);
    private long preparedBytes;
    private string Profile => bitflags ? BitflagName : markers ? MarkerName : choices ? ChoiceName : removals ? RemovalName : chains ? ChainName : rootFiles ? RootFileName : Name;
    private readonly XmlSchemaSet schemas;
    private readonly SdkSourcePathAudit.Report paths;
    private readonly Dictionary<string,SdkSourcePathAudit.Source> inventory = new(StringComparer.OrdinalIgnoreCase);
    private readonly Dictionary<string,byte[]> snapshots = new(StringComparer.OrdinalIgnoreCase);
    private long capturedBytes;

    //-------------------------------------------------------------------------------------------------
    /** Reborn: validate the complete captured source inventory before any imported read; caller-supplied graph records cannot authorize escaped files. */
    //-------------------------------------------------------------------------------------------------
    internal SdkInstanceInheritanceProfile(XmlSchemaSet schemas,SdkSourcePathAudit.Report paths,bool rootFiles = false,bool chains = false,bool removals = false,bool choices = false,bool markers = false,bool bitflags = false)
    {
        this.schemas = schemas; this.paths = paths;
        // Reborn: choice admission includes the tested chain/removal subsets without changing any earlier constructor defaults.
        // Reborn: explicitly selecting markers includes earlier choices/chains without changing their constructor defaults.
        // Reborn: modifier admission implies markers/choices/chains, but all earlier constructor defaults remain unchanged.
        this.rootFiles = rootFiles || chains || removals || choices || markers || bitflags; this.chains = chains || removals || choices || markers || bitflags; this.removals = removals || choices || markers || bitflags; this.choices = choices || markers || bitflags; this.markers = markers || bitflags; this.bitflags = bitflags;
        if (!schemas.IsCompiled || paths.Sources.Length > 512 || paths.Includes.Length > 4096) throw new InvalidDataException("Compiled schema and bounded instance inventory required.");
        string[] roots = new[] { paths.SourceRoot,paths.ArtRoot,paths.AudioRoot }.Where(root => root != null).Cast<string>().Select(SdkEnvironmentPreflight.DirectoryPath).ToArray();
        foreach (var source in paths.Sources)
        {
            string canonical = SdkEnvironmentPreflight.Absolute(source.PhysicalPath);
            if (!canonical.Equals(source.PhysicalPath,StringComparison.OrdinalIgnoreCase) || !roots.Contains(source.Root,StringComparer.OrdinalIgnoreCase)
                || !SdkEnvironmentPreflight.Inside(source.Root,canonical) || !inventory.TryAdd(canonical,source)) throw new InvalidDataException("Ambiguous or escaped instance source inventory.");
        }
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: select direct-only or recursive direct-definition preparation, perform bounded core overlays, and publish owner-only XML with atomic source witnesses. */
    //-------------------------------------------------------------------------------------------------
    internal SdkSelfAttributeInheritance.Result Apply(string path,byte[] bytes)
    {
        if (!chains) return ApplyDocument(path,bytes,0,out _);
        // Reborn: reset both raw and prepared snapshots for each owner; shared nodes coalesce only within this invocation.
        snapshots.Clear(); capturedBytes = 0; prepared.Clear(); active.Clear(); preparedBytes = 0;
        try { return Prepare(path,bytes,0).Result; }
        catch (Exception error) when (error is IOException or InvalidDataException or XmlException or ArgumentException or UnauthorizedAccessException or NotSupportedException or BinaryAssetBuilderException)
        { return new(null,new(Profile,Convert.ToHexString(SHA256.HashData(bytes)),null,Array.Empty<SdkSelfAttributeInheritance.Overlay>(),new[] { error.Message[..Math.Min(error.Message.Length,512)] })); }
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: prepare each child in its own direct-instance scope, preserve active-cycle and cached semantic-depth limits, and bound aggregate prepared bytes. */
    //-------------------------------------------------------------------------------------------------
    private Prepared Prepare(string path,byte[]? supplied,int depth)
    {
        if (depth > 32 || active.Contains(path)) throw new InvalidDataException("Instance preparation cycle/32-depth bound exceeded.");
        byte[] bytes = Capture(path,supplied);
        if (prepared.TryGetValue(path,out var cached))
        {
            if (depth+cached.Height > 32) throw new InvalidDataException("Cached instance preparation exceeds 32-depth bound.");
            return cached;
        }
        active.Add(path);
        try
        {
            var result = ApplyDocument(path,bytes,depth,out int height);
            // Reborn: propagate the leaf diagnostic once so deep failures cannot hide their actual reason behind repeated prefixes and the bounded report length.
            if (result.Bytes == null) throw new InvalidDataException(string.Join("; ",result.Evidence.Diagnostics));
            preparedBytes += result.Bytes.Length;
            if (preparedBytes > 32*1048576 || prepared.Count >= 512) throw new InvalidDataException("32 MiB/512-document prepared instance closure bound exceeded.");
            // Reborn: closure hashes use final owner-only XML after temporary injected bases were removed; coalesce shared origins without inventing live-disk proof.
            var own = new PreparedSource(path,inventory[path].Sha256,result.Evidence.ProcessedSha256!);
            result = result with { Evidence = result.Evidence with { PreparedSources = result.Evidence.PreparedSources.Append(own).DistinctBy(source => source.SourcePath,StringComparer.OrdinalIgnoreCase).ToArray() } };
            var value = new Prepared(result,height); prepared.Add(path,value); return value;
        }
        finally { active.Remove(path); }
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: expose only prepared direct-child declarations and delegate independently scoped local copy or empty-child matching without transitive handle authority. */
    //-------------------------------------------------------------------------------------------------
    private SdkSelfAttributeInheritance.Result ApplyDocument(string path,byte[] bytes,int depth,out int height)
    {
        height = 0;
        string raw = Convert.ToHexString(SHA256.HashData(bytes));
        try
        {
            if (paths.StoppedAtLimit) throw new InvalidDataException("Incomplete path inventory cannot authorize instance inheritance.");
            Capture(path,bytes); XmlDocument owner = Parse(bytes);
            var ownAssets = Assets(owner); var local = Handles(ownAssets);
            // Reborn: preserve the earlier local tree path when no external handle is needed; malformed handles still fail the tree profile.
            var needed = ownAssets.Where(asset => asset.HasAttribute("inheritFrom")).Select(Target).Where(handle => !local.ContainsKey(handle)).Distinct(StringComparer.Ordinal).ToArray();
            if (needed.Length == 0 && !chains)
            { var localResult = SdkSelfAttributeInheritance.Apply(schemas,bytes,treeCopy:true); return localResult with { Evidence = localResult.Evidence with { Profile = Profile } }; }
            var includes = Includes(owner); var expected = paths.Includes.Where(edge => edge.Document.Equals(path,StringComparison.OrdinalIgnoreCase)).ToArray();
            if (includes.Length != expected.Length || includes.Length > 64) throw new InvalidDataException("Instance Include edges differ from captured source or exceed 64 direct Includes.");
            Dictionary<string,(XmlElement Asset,ImportedBase Witness)> external = new(StringComparer.Ordinal);
            // Reborn: include all visited preparation sources, including direct visibility candidates not ultimately selected as inherited handles.
            List<PreparedSource> closure = new();
            // Reborn: cap source-local expansion across the entire direct visibility set, not only each imported document individually.
            long expandedBaseBytes = 0;
            for (int index = 0; index < includes.Length; index++)
            {
                var include = includes[index]; var edge = expected[index]; string logical = include.GetAttribute("source");
                if (include.NamespaceURI != Ea || include.LocalName != "Include" || include.GetAttribute("type") != "instance" || include.HasChildNodes
                    || include.Attributes.OfType<XmlAttribute>().Any(attribute => attribute.Name is not ("source" or "type"))
                    || edge.Kind != "instance" || edge.LogicalPath != logical || edge.PhysicalPath == null) throw new InvalidDataException("Only direct source-backed instance Includes are admitted; all/reference/precompiled visibility remains closed.");
                var resolved = SdkSourcePathAudit.Resolve(logical,Path.GetDirectoryName(path)!,inventory[path].Root,paths.SourceRoot,paths.ArtRoot,paths.AudioRoot);
                if (!resolved.Path.Equals(edge.PhysicalPath,StringComparison.OrdinalIgnoreCase) || !inventory.TryGetValue(resolved.Path,out var source)
                    || !resolved.Root.Equals(source.Root,StringComparison.OrdinalIgnoreCase) || resolved.Path.Equals(path,StringComparison.OrdinalIgnoreCase)) throw new InvalidDataException("Instance target differs from captured confined inventory or is cyclic.");
                byte[] baseBytes = Capture(resolved.Path); XmlDocument baseXml = Parse(baseBytes);
                if (!chains && Includes(baseXml).Length != 0) throw new InvalidDataException("Imported base documents must be Include-free; transitive visibility remains closed.");
                // Reborn: source-local preparation never exports a descendant's declarations as this direct child's own assets.
                Prepared? child = chains ? Prepare(resolved.Path,null,depth+1) : null;
                var expanded = child?.Result ?? SdkSelfAttributeInheritance.Apply(schemas,baseBytes,treeCopy:true);
                if (child != null) { height = Math.Max(height,child.Height+1); closure.AddRange(expanded.Evidence.PreparedSources); }
                if (expanded.Bytes == null || !SdkEffectiveSchema.Bind(schemas,resolved.Path,expanded.Bytes).XmlValidated) throw new InvalidDataException("Imported base document requires unsupported preprocessing or fails schema validation.");
                expandedBaseBytes += expanded.Bytes.Length;
                if (expandedBaseBytes > 32*1048576) throw new InvalidDataException("32 MiB expanded instance-source bound exceeded.");
                foreach (var pair in Handles(Assets(Parse(expanded.Bytes))))
                {
                    // Reborn: bound aggregate direct visibility independently from per-source asset/source-byte bounds.
                    if (external.Count >= 4096) throw new InvalidDataException("4096 direct-instance handle bound exceeded.");
                    var witness = new ImportedBase(pair.Value.LocalName,pair.Value.GetAttribute("id"),resolved.Path,source.Sha256,expanded.Evidence.ProcessedSha256!) { PreparedSources = chains ? expanded.Evidence.PreparedSources : Array.Empty<PreparedSource>() };
                    if (!external.TryAdd(pair.Key,(pair.Value,witness))) throw new InvalidDataException("Duplicate direct-instance handles are ambiguous; no first-wins fallback.");
                }
            }
            List<ImportedBase> witnesses = new(); HashSet<string> injected = new(StringComparer.Ordinal); long expandedSize = bytes.Length+1024L;
            foreach (string handle in needed)
            {
                if (!external.TryGetValue(handle,out var entry)) throw new InvalidDataException("Inherited handle is absent from direct-instance source assets.");
                if (!Inheritable(entry.Asset.LocalName)) throw new InvalidDataException("Imported base type is not derived from BaseInheritableAsset.");
                // Reborn: inspect selected imported fields before overlay; defining-document relative paths cannot be guessed from the consuming document.
                XmlDocument single = Parse(Encoding.UTF8.GetBytes("<AssetDeclaration xmlns='"+Ea+"'/>")); single.DocumentElement!.AppendChild(single.ImportNode(entry.Asset,true));
                var baseBinding = SdkEffectiveSchema.Bind(schemas,entry.Witness.SourcePath,Serialize(single));
                // Reborn: explicit alias roots under the scoped diagnostic resolver are source-directory independent; native wildcard/postfix search and physical safety/existence are separate gates.
                if (!baseBinding.XmlValidated || (baseBinding.Fields.Length != 0 && (!rootFiles || baseBinding.Fields.Any(field => !RootQualified(field.LogicalPath))))) throw new InvalidDataException("Imported file-field provenance requires broader binding; only explicit DATA/ART/AUDIO roots are admitted by the root-file profile.");
                expandedSize += Encoding.UTF8.GetByteCount(entry.Asset.OuterXml)+1L;
                if (expandedSize > 4*1048576) throw new InvalidDataException("Injected instance bases exceed 4 MiB before merge.");
                // Reborn: imported source-local chains were already expanded/validated; remove only their retained diagnostic inheritFrom marker before local delegation.
                var imported = (XmlElement)owner.ImportNode(entry.Asset,true); imported.RemoveAttribute("inheritFrom"); owner.DocumentElement!.AppendChild(imported);
                injected.Add(handle); witnesses.Add(entry.Witness with { RootQualifiedFields = baseBinding.Fields });
            }
            // Reborn: the new chain profile includes only the independently tested empty-complex-child merge subset; older imported flags remain one-sided.
            var merged = SdkSelfAttributeInheritance.Apply(schemas,Serialize(owner),treeCopy:true,childMerge:chains,childRemoval:removals,choiceCopy:choices,consumeMarkers:markers,bitflags:bitflags);
            if (merged.Bytes == null) throw new InvalidDataException((chains ? "Instance overlay exceeds chain merge scope: " : "Instance overlay exceeds copy-only scope: ")+string.Join("; ",merged.Evidence.Diagnostics));
            XmlDocument output = Parse(merged.Bytes);
            foreach (var asset in Assets(output).Where(asset => injected.Contains(asset.LocalName+":"+asset.GetAttribute("id"))).ToArray()) output.DocumentElement!.RemoveChild(asset);
            byte[] processed = Serialize(output);
            if (processed.Length > 4*1048576) throw new InvalidDataException("Processed instance owner exceeds 4 MiB.");
            return new(processed,new(Profile,raw,Convert.ToHexString(SHA256.HashData(processed)),merged.Evidence.Overlays,Array.Empty<string>()) { ImportedBases = witnesses.ToArray(),PreparedSources = closure.DistinctBy(source => source.SourcePath,StringComparer.OrdinalIgnoreCase).ToArray(),Removals = merged.Evidence.Removals,ConsumedMarkers = merged.Evidence.ConsumedMarkers,Bitflags = merged.Evidence.Bitflags });
        }
        catch (Exception error) when (error is IOException or InvalidDataException or XmlException or ArgumentException or UnauthorizedAccessException or NotSupportedException or BinaryAssetBuilderException)
        { return new(null,new(Profile,raw,null,Array.Empty<SdkSelfAttributeInheritance.Overlay>(),new[] { error.Message[..Math.Min(error.Message.Length,512)] })); }
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: recognize only explicit supported alias authority, without claiming lexical confinement, root availability or payload identity. */
    //-------------------------------------------------------------------------------------------------
    private static bool RootQualified(string logical)
    {
        int colon = logical.IndexOf(':');
        return colon > 0 && colon < logical.Length-1 && logical == logical.Trim()
            && logical[..colon].ToLowerInvariant() is "data" or "art" or "audio";
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: recheck confined source length/hash once per captured snapshot and cap aggregate imported reads without invented paths. */
    //-------------------------------------------------------------------------------------------------
    private byte[] Capture(string path,byte[]? supplied = null)
    {
        if (!inventory.TryGetValue(path,out var source)) throw new InvalidDataException("Instance source absent from captured inventory.");
        if (supplied == null && snapshots.TryGetValue(path,out var previous)) return previous;
        byte[] bytes = supplied ?? SdkEnvironmentPreflight.Read(path,4*1048576);
        if (bytes.Length > 4*1048576 || bytes.Length != source.Bytes || Convert.ToHexString(SHA256.HashData(bytes)) != source.Sha256) throw new InvalidDataException("Stale instance source differs from captured snapshot.");
        if (!snapshots.ContainsKey(path))
        { capturedBytes += bytes.Length; if (capturedBytes > 32*1048576) throw new InvalidDataException("32 MiB instance snapshot bound exceeded."); snapshots.Add(path,bytes); }
        return bytes;
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: use effective schema ancestry, not element spelling or guessed native IDs, to prove imported inheritance eligibility. */
    //-------------------------------------------------------------------------------------------------
    private bool Inheritable(string name)
    {
        var type = schemas.GlobalTypes[new XmlQualifiedName(name,Ea)] as XmlSchemaComplexType;
        for (int depth = 0; type != null && depth < 128; depth++,type = type.BaseXmlSchemaType as XmlSchemaComplexType)
        { if (type.QualifiedName == new XmlQualifiedName("BaseInheritableAsset",Ea)) return depth > 0; if (ReferenceEquals(type,type.BaseXmlSchemaType)) break; }
        return false;
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: parse bounded owner/base XML without DTD, resolver or formatting whitespace that core treats as sequence children. */
    //-------------------------------------------------------------------------------------------------
    private static XmlDocument Parse(byte[] bytes)
    {
        XmlDocument xml = new() { XmlResolver = null }; using MemoryStream input = new(bytes,false);
        using (XmlReader reader = XmlReader.Create(input,new XmlReaderSettings { DtdProcessing = DtdProcessing.Prohibit,XmlResolver = null,MaxCharactersInDocument = 4*1048576 })) xml.Load(reader);
        if (xml.DocumentElement?.LocalName != "AssetDeclaration" || xml.DocumentElement.NamespaceURI != Ea) throw new InvalidDataException("EA instance declaration required."); return xml;
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: enumerate only direct asset declarations, leaving metadata containers out of instance handle authority. */
    //-------------------------------------------------------------------------------------------------
    private static XmlElement[] Assets(XmlDocument xml) => xml.DocumentElement!.ChildNodes.OfType<XmlElement>().Where(asset => asset.LocalName is not ("Includes" or "Defines" or "Tags")).ToArray();

    //-------------------------------------------------------------------------------------------------
    /** Reborn: reject duplicate/foreign direct handles instead of selecting an arbitrary imported/local declaration. */
    //-------------------------------------------------------------------------------------------------
    private static Dictionary<string,XmlElement> Handles(XmlElement[] assets)
    {
        Dictionary<string,XmlElement> result = new(StringComparer.Ordinal);
        if (assets.Length > 4096) throw new InvalidDataException("4096 instance asset bound exceeded.");
        foreach (var asset in assets) if (asset.NamespaceURI != Ea || !result.TryAdd(asset.LocalName+":"+asset.GetAttribute("id"),asset)) throw new InvalidDataException("Duplicate or foreign instance declaration."); return result;
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: allow only exact same-type literal target handles; same-handle imported overrides require a separate visibility proof. */
    //-------------------------------------------------------------------------------------------------
    private static string Target(XmlElement asset)
    {
        string target = asset.GetAttribute("inheritFrom"); var parts = target.Split(':');
        string id = parts.Length == 1 ? target : parts.Length == 2 && parts[0] == asset.LocalName ? parts[1] : throw new InvalidDataException("Cross-type instance inheritance remains closed.");
        if (id.Length is 0 or >128 || !id.All(character => char.IsAsciiLetterOrDigit(character) || character is '_' or '-' or '.') || id == asset.GetAttribute("id")) throw new InvalidDataException("Unsafe or same-handle instance override remains closed."); return asset.LocalName+":"+id;
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: reject duplicate/foreign Include containers and preserve their captured declaration order. */
    //-------------------------------------------------------------------------------------------------
    private static XmlElement[] Includes(XmlDocument xml)
    {
        var containers = xml.DocumentElement!.ChildNodes.OfType<XmlElement>().Where(element => element.LocalName == "Includes").ToArray();
        if (containers.Length > 1 || containers.Any(element => element.NamespaceURI != Ea)) throw new InvalidDataException("Unsupported instance Include container shape."); return containers.SelectMany(element => element.ChildNodes.OfType<XmlElement>()).ToArray();
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: serialize only bounded in-memory diagnostic documents; this helper never publishes source or production output files. */
    //-------------------------------------------------------------------------------------------------
    private static byte[] Serialize(XmlDocument xml)
    {
        using MemoryStream output = new(); using (XmlWriter writer = XmlWriter.Create(output,new XmlWriterSettings { Encoding = new UTF8Encoding(false),NewLineHandling = NewLineHandling.None })) xml.Save(writer); return output.ToArray();
    }
}
