using System.Security.Cryptography;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: classify captured dependency failures without inventing alias roots, suppressing issues or replacing source Includes with manifests.
internal static class SdkDependencyReview
{
    // Reborn: distinguish occurrence counts from unique logical paths and physical missing-file evidence.
    internal sealed record Group(string Classification,string Issue,string OwnerType,string Kind,string Name,string LogicalPath,int Occurrences,string[] Documents);
    internal sealed record PathIssue(string Classification,string Code,string Document,string LogicalPath);
    internal sealed record AliasEvidence(string LogicalPath,string CandidatePath,bool CandidateExists,bool CandidateCaptured,bool NativeDottedExists,bool NativeCanonicalPathMatches,long? Bytes,string? CandidateSha256,string? DottedSha256,bool ByteSnapshotsAgree);
    internal sealed record Report(int Documents,int ValidatedDocuments,int PathIssueCount,int DependencyOccurrences,int DistinctDependencyPaths,PathIssue[] PathIssues,Group[] Dependencies,AliasEvidence[] TrailingDotEvidence)
    {
        // Reborn: classification and native path observations neither expand resolver admission nor establish complete source/dependency/game coverage.
        public bool ReadOnly => true;
        public bool SnapshotOnly => true;
        public bool ResolverChanged => false;
        public bool FullDependencyCoverage => false;
        public bool ProductionBuildReady => false;
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: group only recorded failures, preserving field identity and affected documents; optionally observe two exact confined trailing-dot aliases. */
    //-------------------------------------------------------------------------------------------------
    internal static Report Inspect(SdkSourcePathAudit.Report paths,SdkTypedSourceGraph.Report graph,bool observeAliases = false)
    {
        if (paths.StoppedAtLimit || graph.StoppedAtLimit || paths.Issues.Length > 128 || graph.Documents.Length > 512) throw new InvalidDataException("Complete bounded captured inventories required for dependency classification.");
        var rows = graph.Documents.SelectMany(document => document.Dependencies.Where(field => field.Issue != null).Select(field => (Document:document.SourcePath,Field:field))).ToArray();
        if (rows.Length > 4096) throw new InvalidDataException("Dependency classification exceeds 4096 occurrences.");
        var groups = rows.GroupBy(row => (row.Field.Issue,row.Field.OwnerType,row.Field.Kind,row.Field.Name,row.Field.LogicalPath))
            .Select(group => new Group(Category(group.Key.Issue!,group.Key.LogicalPath,paths.AudioRoot),group.Key.Issue!,group.Key.OwnerType,group.Key.Kind,group.Key.Name,group.Key.LogicalPath,group.Count(),group.Select(row => row.Document).Distinct(StringComparer.OrdinalIgnoreCase).OrderBy(path => path,StringComparer.Ordinal).ToArray()))
            .OrderBy(group => group.LogicalPath,StringComparer.Ordinal).ThenBy(group => group.OwnerType,StringComparer.Ordinal).ThenBy(group => group.Name,StringComparer.Ordinal).ToArray();
        var issues = paths.Issues.Select(issue => new PathIssue(KnownAlias(issue.LogicalPath) && issue.Code == "IncludePath" ? "KnownTrailingDotOutsideResolver" : Category(issue.Code,issue.LogicalPath,paths.AudioRoot),issue.Code,issue.Document,issue.LogicalPath)).ToArray();
        var aliases = observeAliases ? paths.Issues.Where(issue => issue.Code == "IncludePath" && KnownAlias(issue.LogicalPath)).DistinctBy(issue => issue.LogicalPath,StringComparer.OrdinalIgnoreCase).Select(issue => Observe(paths,issue)).ToArray() : Array.Empty<AliasEvidence>();
        return new(graph.Documents.Length,graph.Documents.Count(document => document.Status == "Validated"),issues.Length,rows.Length,rows.Select(row => row.Field.LogicalPath).Distinct(StringComparer.Ordinal).Count(),issues,groups,aliases);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: report a missing explicit root separately from stat-proved absence; unknown failures remain unclassified and visible. */
    //-------------------------------------------------------------------------------------------------
    private static string Category(string issue,string logical,string? audioRoot) => audioRoot == null && logical.StartsWith("AUDIO:",StringComparison.OrdinalIgnoreCase) && issue is "IncludePath" or "ResourcePath"
        ? "AudioRootNotSupplied" : issue == "ResourceMissing" ? "ScopedResourceAbsent" : "UnclassifiedPathFailure";

    //-------------------------------------------------------------------------------------------------
    /** Reborn: limit native spelling experiments to the two reviewed map Includes, never arbitrary trailing-dot normalization. */
    //-------------------------------------------------------------------------------------------------
    private static bool KnownAlias(string logical) => new[] { "DATA:maps/official/CAMP_S06_Iceland_Bass/AIP_S06_SovietKrukov.xml.","DATA:maps/official/CAMP_S06_Iceland_Bass/AIS_S06_SovietKrukov.xml." }
        .Contains(logical.Replace('\\','/'),StringComparer.OrdinalIgnoreCase);

    //-------------------------------------------------------------------------------------------------
    /** Reborn: read small exact candidate/dotted source snapshots with confinement/reparse checks; observations do not admit the rejected Include. */
    //-------------------------------------------------------------------------------------------------
    private static AliasEvidence Observe(SdkSourcePathAudit.Report paths,SdkSourcePathAudit.Issue issue)
    {
        if (!KnownAlias(issue.LogicalPath) || !SdkEnvironmentPreflight.Inside(paths.SourceRoot,issue.Document)) throw new InvalidDataException("Known confined alias observation required.");
        var candidate = SdkSourcePathAudit.Resolve(issue.LogicalPath[..^1],Path.GetDirectoryName(issue.Document)!,paths.SourceRoot,paths.SourceRoot,paths.ArtRoot,paths.AudioRoot);
        string dotted = candidate.Path+".";
        bool exists = File.Exists(candidate.Path),dotExists = OperatingSystem.IsWindows() && File.Exists(dotted);
        if (!exists) return new(issue.LogicalPath,candidate.Path,false,false,dotExists,false,null,null,null,false);
        byte[] raw = SdkEnvironmentPreflight.Read(candidate.Path,4*1048576);
        byte[]? dotRaw = dotExists ? SdkEnvironmentPreflight.Read(dotted,4*1048576) : null;
        string hash = Hash(raw),dotHash = dotRaw == null ? "" : Hash(dotRaw);
        if (Hash(SdkEnvironmentPreflight.Read(candidate.Path,4*1048576)) != hash) throw new InvalidDataException("Trailing-dot candidate changed during observation.");
        return new(issue.LogicalPath,candidate.Path,true,paths.Sources.Any(source => source.PhysicalPath.Equals(candidate.Path,StringComparison.OrdinalIgnoreCase)),dotExists,
            OperatingSystem.IsWindows() && Path.GetFullPath(dotted).Equals(candidate.Path,StringComparison.OrdinalIgnoreCase),raw.Length,hash,dotRaw == null ? null : dotHash,dotRaw != null && raw.Length == dotRaw.Length && hash == dotHash);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: fingerprints represent source byte snapshots, not native file identity, type hashes or compiled assets. */
    //-------------------------------------------------------------------------------------------------
    private static string Hash(byte[] bytes) => Convert.ToHexString(SHA256.HashData(bytes));
}
