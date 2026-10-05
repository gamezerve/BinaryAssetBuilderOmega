namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: prove reviewed warning admission cannot become an arbitrary warning bypass or alter the default schema/reference gate.
internal static class SdkSchemaHookReviewSmokeTest
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: test pinned candidate, exact warning inventory, positive/negative input semantics and distinct clean/admitted status without payload processing. */
    //-------------------------------------------------------------------------------------------------
    internal static void Run()
    {
        var report = SdkEffectiveSchema.Inspect(shieldCandidate:true,reviewHooks:true);
        Require(report.SchemaEngineCompiled && !report.SchemaCompiled && report.SchemaAdmitted && report.WarningPolicy == SdkSchemaHookReview.Policy
            && report.WarningReviewChecks.Length == 4 && report.Warnings.Length == 2 && report.Errors.Length == 0 && report.EffectiveFileAttributes.Length > 0
            && !report.ProductionBuildReady && !report.FullDependencyCoverage,"Reviewed warning/clean/admitted evidence differs.");
        Reject(() => SdkEffectiveSchema.Inspect(reviewHooks:true));
        // Reborn: rebuild only captured staged snapshots to exercise policy negatives without editing any schema files.
        string root = SdkEnvironmentPreflight.BaselineSchemaRoot(); var catalog = SdkEnvironmentPreflight.Catalog(root);
        var snapshots = catalog.ToDictionary(row => row.Key,row => SdkEnvironmentPreflight.Read(Path.Combine(root,row.Key.Replace('/',Path.DirectorySeparatorChar)),2*1048576),StringComparer.OrdinalIgnoreCase);
        SdkShieldSchemaCandidate.Apply(snapshots); var evidence = SdkEffectiveSchema.Compile(snapshots,"cnc3types.xsd");
        Require(SdkSchemaHookReview.Review(evidence,SdkSchemaHookReview.CandidateDigest).Length == 4,"Reviewed probes are not repeatable.");
        Reject(() => SdkSchemaHookReview.Review(evidence,"wrong-digest"));
        Reject(() => SdkSchemaHookReview.Review(evidence with { Warnings = Array.Empty<string>() },SdkSchemaHookReview.CandidateDigest));
        Reject(() => SdkSchemaHookReview.Review(evidence with { Warnings = evidence.Warnings.Concat(new[] { "unreviewed warning" }).ToArray() },SdkSchemaHookReview.CandidateDigest));
        Reject(() => SdkSchemaHookReview.Review(evidence with { Warnings = evidence.Warnings.Select(value => value.Replace(":337]",":338]")).ToArray() },SdkSchemaHookReview.CandidateDigest));
        Reject(() => SdkSchemaHookReview.Review(evidence with { Errors = new[] { "real schema error" } },SdkSchemaHookReview.CandidateDigest));
        Require(SdkEffectiveSchema.Inspect(shieldCandidate:true).SchemaAdmitted == false,"Review silently widened the unreviewed candidate.");
        Console.WriteLine("SDK reviewed-hook self-test: OK (exact candidate/two-warning/provenance pinning, effective hook absence, positive mesh/track and authored-hook rejection, changed/missing/extra/error/default rejection; no processor/game readiness)");
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: invalid review authority or diagnostic inventory must reject instead of auto-waiving warnings. */
    //-------------------------------------------------------------------------------------------------
    private static void Reject(Action action)
    { try { action(); } catch (Exception error) when (error is InvalidDataException or ArgumentException) { return; } throw new InvalidDataException("Unreviewed schema warnings accepted."); }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: distinguish reviewed diagnostic admission from zero-warning schema status and usable SDK claims. */
    //-------------------------------------------------------------------------------------------------
    private static void Require(bool condition,string message) { if (!condition) throw new InvalidDataException(message); }
}
