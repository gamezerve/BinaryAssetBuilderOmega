namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: test failure classification independently from external source/game files or resolver mutations.
internal static class SdkDependencyReviewSmokeTest
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: preserve duplicate occurrences and field identity while refusing to call root-not-supplied evidence a missing physical file. */
    //-------------------------------------------------------------------------------------------------
    internal static void Run()
    {
        const string header = "AUDIO:Pathfinder\\RA3EPMus\\PC\\RA3EPMus.h";
        var issues = new[] { new SdkSourcePathAudit.Issue("ResourcePath","Base.xml",header,"owned"),new SdkSourcePathAudit.Issue("IncludePath","Library.xml","DATA:maps/official/CAMP_S06_Iceland_Bass/AIP_S06_SovietKrukov.xml.","owned") };
        var paths = new SdkSourcePathAudit.Report("owned-root",null,null,Array.Empty<SdkSourcePathAudit.Source>(),Array.Empty<SdkSourcePathAudit.Edge>(),Array.Empty<SdkSourcePathAudit.Resource>(),issues,false,false,true,false,false,Array.Empty<string>());
        var field = new SdkTypedSourceGraph.Dependency("PathMusicEvent","attribute","PathfinderEventHeader",header,null,null,"ResourcePath");
        var documents = new[] { new SdkTypedSourceGraph.Document("Base.xml","hash","hash","Validated",new[] { field },Array.Empty<string>()),new SdkTypedSourceGraph.Document("Events.xml","hash","hash","Validated",new[] { field,field },Array.Empty<string>()) };
        var graph = new SdkTypedSourceGraph.Report(documents,false,false,true,true,false,false);
        var report = SdkDependencyReview.Inspect(paths,graph);
        Require(report.DependencyOccurrences == 3 && report.DistinctDependencyPaths == 1 && report.Dependencies.Length == 1 && report.Dependencies[0].Documents.Length == 2
            && report.Dependencies[0].Classification == "AudioRootNotSupplied" && report.PathIssues[1].Classification == "KnownTrailingDotOutsideResolver" && !report.ResolverChanged && !report.ProductionBuildReady,"Dependency deduplication/classification contract changed.");
        report = SdkDependencyReview.Inspect(paths with { AudioRoot = "explicit-audio-root" },graph);
        Require(report.Dependencies[0].Classification == "UnclassifiedPathFailure","Supplied root was misreported as absent.");
        documents[1] = documents[1] with { Dependencies = new[] { field with { Name = "OtherField" },field with { Issue = "ResourceMissing",PhysicalPath = "scoped-file" } } };
        report = SdkDependencyReview.Inspect(paths,graph);
        Require(report.Dependencies.Length == 3 && report.Dependencies.Any(group => group.Classification == "ScopedResourceAbsent"),"Different issue/field identities collapsed or scoped absence hidden.");
        bool rejected = false; try { SdkDependencyReview.Inspect(paths with { StoppedAtLimit = true },graph); } catch (InvalidDataException) { rejected = true; }
        Require(rejected,"Stopped inventory accepted as complete classification.");
        report = SdkDependencyReview.Inspect(paths with { Issues = new[] { issues[1] with { LogicalPath = "DATA:arbitrary.xml." } } },graph);
        Require(report.PathIssues[0].Classification == "UnclassifiedPathFailure" && report.TrailingDotEvidence.Length == 0,"Unknown alias experiment implicitly enabled.");
        Console.WriteLine("SDK dependency review self-test: OK (occurrences vs unique paths, document/field identities, missing-root vs scoped absence, stopped inventory and unknown alias isolation; no resolver/source changes)");
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: fail on misleading completeness or missing-file classification. */
    //-------------------------------------------------------------------------------------------------
    private static void Require(bool condition,string message) { if (!condition) throw new InvalidDataException(message); }
}
