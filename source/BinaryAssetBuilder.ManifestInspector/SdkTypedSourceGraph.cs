using System.Security.Cryptography;
using System.Xml;
using System.Xml.Schema;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: bind only freshly rechecked reachable XML to the reviewed diagnostic schema; keep preprocessing, XML validation and physical paths separate.
internal static class SdkTypedSourceGraph
{
    // Reborn: resource evidence is existence/length only, never payload identity, native layout or game compatibility.
    internal sealed record Dependency(string OwnerType,string Kind,string Name,string LogicalPath,string? PhysicalPath,long? Bytes,string? Issue);
    internal sealed record Document(string SourcePath,string ExpectedSha256,string? ObservedSha256,string Status,Dependency[] Dependencies,string[] Diagnostics)
    {
        // Reborn: preprocessing evidence is opt-in and never replaces the captured raw source identity.
        public SdkLocalDefineProfile.Evidence? Preprocessing { get; init; }
        // Reborn: local overlay evidence has a separate identity chain from definition substitutions and raw source bytes.
        public SdkSelfAttributeInheritance.Evidence? Inheritance { get; init; }
    }
    internal sealed record Report(Document[] Documents,bool ScopedGraphComplete,bool StoppedAtLimit,bool ReadOnly,bool SnapshotOnly,bool FullDependencyCoverage,bool ProductionBuildReady)
    {
        // Reborn: report the requested diagnostic profile even when every source is blocked before expression processing.
        public string PreprocessingProfile { get; init; } = "raw-source";
    }
    internal sealed record Result(SdkEffectiveSchema.Report Schema,Report Graph);

    //-------------------------------------------------------------------------------------------------
    /** Reborn: compile/review once and validate the bounded source path graph without following new Includes or enabling production processors. */
    //-------------------------------------------------------------------------------------------------
    internal static Result Inspect(SdkSourcePathAudit.Report paths,bool localDefines = false,bool includeDefines = false,bool definitionExpressions = false,bool selfAttributeInheritance = false,bool selfChildCopy = false,bool selfComplexChildCopy = false,bool selfTreeCopy = false,bool instanceInheritance = false,bool instanceRootFiles = false,bool selfChildMerge = false,bool instanceChains = false,bool instanceRemovals = false,bool instanceChoices = false)
    {
        XmlSchemaSet? schemas = null;
        var schema = SdkEffectiveSchema.Inspect(shieldCandidate:true,reviewHooks:true,onAdmitted:set => schemas = set);
        if (!schema.SchemaAdmitted || schemas == null) throw new InvalidDataException("Reviewed schema required for typed graph binding.");
        return new(schema,BindGraph(schemas,paths,localDefines,includeDefines,definitionExpressions,selfAttributeInheritance,selfChildCopy,selfComplexChildCopy,selfTreeCopy,instanceInheritance,instanceRootFiles,selfChildMerge,instanceChains,instanceRemovals,instanceChoices));
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: recheck raw source bytes/confinement, refuse inheritFrom overlays, and stat only schema-selected literal dependencies under explicit roots. */
    //-------------------------------------------------------------------------------------------------
    internal static Report BindGraph(XmlSchemaSet schemas,SdkSourcePathAudit.Report paths,bool localDefines = false,bool includeDefines = false,bool definitionExpressions = false,bool selfAttributeInheritance = false,bool selfChildCopy = false,bool selfComplexChildCopy = false,bool selfTreeCopy = false,bool instanceInheritance = false,bool instanceRootFiles = false,bool selfChildMerge = false,bool instanceChains = false,bool instanceRemovals = false,bool instanceChoices = false)
    {
        if (!schemas.IsCompiled || paths.Sources.Length > 512) throw new InvalidDataException("Compiled schema and bounded source inventory required.");
        // Reborn: broader literal visibility is independently explicit and cannot silently replace the earlier local-only profile.
        // Reborn: the complex leaf profile is independently explicit; all earlier admission rules remain unchanged.
        // Reborn: recursive tree copying is independent of all earlier flags and preserves their raw/definition/inheritance contracts.
        // Reborn: direct-instance inheritance must be requested separately; provenance and visibility guards cannot widen local-only flags.
        // Reborn: admitting imported alias-root fields cannot silently widen the earlier file-free instance flag.
        // Reborn: empty-child matching is explicit and cannot widen the previous one-sided/imported profiles.
        // Reborn: recursive direct-instance preparation has its own source-closure contract and must not widen any earlier flag.
        // Reborn: repeated-choice copying is a thirteenth independent option, never an implicit expansion of removal-only admission.
        if ((localDefines ? 1 : 0)+(includeDefines ? 1 : 0)+(definitionExpressions ? 1 : 0)+(selfAttributeInheritance ? 1 : 0)+(selfChildCopy ? 1 : 0)+(selfComplexChildCopy ? 1 : 0)+(selfTreeCopy ? 1 : 0)+(instanceInheritance ? 1 : 0)+(instanceRootFiles ? 1 : 0)+(selfChildMerge ? 1 : 0)+(instanceChains ? 1 : 0)+(instanceRemovals ? 1 : 0)+(instanceChoices ? 1 : 0) > 1) throw new ArgumentException("Choose one diagnostic preprocessing profile.");
        SdkIncludeDefineProfile? includeProfile = includeDefines || definitionExpressions || selfAttributeInheritance || selfChildCopy || selfComplexChildCopy || selfTreeCopy || instanceInheritance || instanceRootFiles || selfChildMerge || instanceChains || instanceRemovals || instanceChoices ? new(paths,definitionExpressions || selfAttributeInheritance || selfChildCopy || selfComplexChildCopy || selfTreeCopy || instanceInheritance || instanceRootFiles || selfChildMerge || instanceChains || instanceRemovals || instanceChoices) : null;
        SdkInstanceInheritanceProfile? instanceProfile = instanceInheritance || instanceRootFiles || instanceChains || instanceRemovals || instanceChoices ? new(schemas,paths,instanceRootFiles,instanceChains,instanceRemovals,instanceChoices) : null;
        List<Document> documents = new(); HashSet<string> seen = new(StringComparer.OrdinalIgnoreCase);
        string[] roots = new[] { paths.SourceRoot,paths.ArtRoot,paths.AudioRoot }.Where(root => root != null).Cast<string>().Select(SdkEnvironmentPreflight.DirectoryPath).ToArray();
        long total = 0; int dependencies = 0; bool stopped = false;
        foreach (var source in paths.Sources)
        {
            // Reborn: canonicalize inventory paths before confinement checks even if an internal caller supplied a forged/stale path record.
            string canonical = SdkEnvironmentPreflight.Absolute(source.PhysicalPath);
            if (!canonical.Equals(source.PhysicalPath,StringComparison.OrdinalIgnoreCase) || !seen.Add(canonical) || !roots.Contains(source.Root,StringComparer.OrdinalIgnoreCase) || !SdkEnvironmentPreflight.Inside(source.Root,canonical))
                throw new InvalidDataException("Ambiguous or escaped source inventory.");
            string? observed = null;
            try
            {
                byte[] bytes = SdkEnvironmentPreflight.Read(source.PhysicalPath,4*1048576); total += bytes.Length;
                if (total > 32*1048576) { documents.Add(new(source.PhysicalPath,source.Sha256,null,"Limit",Array.Empty<Dependency>(),new[] { "32 MiB source recheck bound exceeded." })); stopped = true; break; }
                observed = Convert.ToHexString(SHA256.HashData(bytes));
                if (observed != source.Sha256 || bytes.Length != source.Bytes)
                { documents.Add(new(source.PhysicalPath,source.Sha256,observed,"StaleSource",Array.Empty<Dependency>(),new[] { "Source bytes differ from path graph snapshot; no new Include traversal or trusted binding." })); continue; }
                XmlDocument xml = new() { XmlResolver = null }; using MemoryStream input = new(bytes,false);
                using (XmlReader reader = XmlReader.Create(input,new XmlReaderSettings { DtdProcessing = DtdProcessing.Prohibit,XmlResolver = null,MaxCharactersInDocument = 4*1048576 })) xml.Load(reader);
                // Reborn: a combined diagnostic profile keeps the definition subset for non-inherited documents, but admits only expression-free local leaf overlays.
                SdkSelfAttributeInheritance.Evidence? inheritance = null;
                bool inherited = xml.SelectNodes("//*")!.OfType<XmlElement>().Any(element => element.NamespaceURI == "uri:ea.com:eala:asset" && element.HasAttribute("inheritFrom"));
                if (inherited)
                {
                    if (!selfAttributeInheritance && !selfChildCopy && !selfComplexChildCopy && !selfTreeCopy && !instanceInheritance && !instanceRootFiles && !selfChildMerge && !instanceChains && !instanceRemovals && !instanceChoices)
                    { documents.Add(new(source.PhysicalPath,source.Sha256,observed,"RequiresPreprocessing",Array.Empty<Dependency>(),new[] { "Asset inheritFrom overlays require an explicit admitted profile; no trusted fields." })); continue; }
                    var expanded = instanceProfile == null ? SdkSelfAttributeInheritance.Apply(schemas,bytes,selfChildCopy,selfComplexChildCopy,selfTreeCopy,selfChildMerge) : instanceProfile.Apply(source.PhysicalPath,bytes); inheritance = expanded.Evidence;
                    if (expanded.Bytes == null)
                    { documents.Add(new(source.PhysicalPath,source.Sha256,observed,"RequiresPreprocessing",Array.Empty<Dependency>(),inheritance.Diagnostics) { Inheritance = inheritance }); continue; }
                    bytes = expanded.Bytes;
                }
                // Reborn: explicit diagnostic substitution happens only after raw snapshot recheck and before schema binding.
                SdkLocalDefineProfile.Evidence? preprocessing = null;
                if (!inherited && (localDefines || includeDefines || definitionExpressions || selfAttributeInheritance || selfChildCopy || selfComplexChildCopy || selfTreeCopy || instanceInheritance || instanceRootFiles || selfChildMerge || instanceChains || instanceRemovals || instanceChoices))
                {
                    var processed = includeProfile == null ? SdkLocalDefineProfile.Apply(bytes) : includeProfile.Apply(source.PhysicalPath,bytes); preprocessing = processed.Evidence;
                    if (processed.Bytes == null)
                    { documents.Add(new(source.PhysicalPath,source.Sha256,observed,"RequiresPreprocessing",Array.Empty<Dependency>(),preprocessing.Diagnostics) { Preprocessing = preprocessing }); continue; }
                    bytes = processed.Bytes;
                }
                var binding = SdkEffectiveSchema.Bind(schemas,source.PhysicalPath,bytes);
                if (!binding.XmlValidated)
                { documents.Add(new(source.PhysicalPath,source.Sha256,observed,"SchemaInvalid",Array.Empty<Dependency>(),binding.Errors.Take(8).ToArray()) { Preprocessing = preprocessing,Inheritance = inheritance }); continue; }
                List<Dependency> fields = new();
                foreach (var field in binding.Fields)
                {
                    if (++dependencies > 4096) { stopped = true; break; }
                    string? physical = null,issue = null; long? length = null;
                    try
                    {
                        var resolved = SdkSourcePathAudit.Resolve(field.LogicalPath,Path.GetDirectoryName(source.PhysicalPath)!,source.Root,paths.SourceRoot,paths.ArtRoot,paths.AudioRoot);
                        physical = resolved.Path;
                        if (!File.Exists(physical)) issue = "ResourceMissing";
                        else { SdkEnvironmentPreflight.CheckPath(physical); length = new FileInfo(physical).Length; }
                    }
                    catch (Exception error) when (error is IOException or InvalidDataException or ArgumentException or NotSupportedException or UnauthorizedAccessException)
                    { issue = "ResourcePath"; }
                    fields.Add(new(field.OwnerType,field.Kind,field.Name,field.LogicalPath,physical,length,issue));
                }
                documents.Add(new(source.PhysicalPath,source.Sha256,observed,stopped ? "Limit" : "Validated",fields.ToArray(),stopped ? new[] { "4096 typed dependency bound exceeded; inventory incomplete." } : Array.Empty<string>()) { Preprocessing = preprocessing,Inheritance = inheritance });
                if (stopped) break;
            }
            catch (Exception error) when (error is IOException or InvalidDataException or XmlException or ArgumentException or UnauthorizedAccessException)
            { documents.Add(new(source.PhysicalPath,source.Sha256,observed,"SourceRead",Array.Empty<Dependency>(),new[] { "Missing, redirected, oversized or malformed source; no trusted binding." })); }
        }
        return new(documents.ToArray(),paths.ScopedPathAuditComplete && !paths.StoppedAtLimit && !stopped && documents.Count > 0 && documents.Count == paths.Sources.Length
            && documents.All(document => document.Status == "Validated" && document.Dependencies.All(field => field.Issue == null)),stopped || paths.StoppedAtLimit,true,true,false,false)
            { PreprocessingProfile = instanceChoices ? SdkInstanceInheritanceProfile.ChoiceName : instanceRemovals ? SdkInstanceInheritanceProfile.RemovalName : instanceChains ? SdkInstanceInheritanceProfile.ChainName : selfChildMerge ? SdkSelfAttributeInheritance.ChildMergeName : instanceRootFiles ? SdkInstanceInheritanceProfile.RootFileName : instanceInheritance ? SdkInstanceInheritanceProfile.Name : selfTreeCopy ? SdkSelfAttributeInheritance.TreeCopyName : selfComplexChildCopy ? SdkSelfAttributeInheritance.ComplexChildCopyName : selfChildCopy ? SdkSelfAttributeInheritance.ChildCopyName : selfAttributeInheritance ? SdkSelfAttributeInheritance.Name : definitionExpressions ? SdkDefinitionSubset.Name : includeDefines ? SdkIncludeDefineProfile.Name : localDefines ? SdkLocalDefineProfile.Name : "raw-source" };
    }
}
