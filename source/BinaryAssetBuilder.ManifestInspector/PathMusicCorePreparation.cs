using System.Xml.Schema;
using Relo;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: bind immutable synthetic-domain Core identities to the same strict local source/header/native closure without admitting a production processor.
internal sealed class PathMusicCorePreparation
{
    // Reborn: copied value records pair actual independently verified Core hashes with exact native content identities.
    internal sealed record Row(string Name,uint InstanceId,uint CoreInstanceHash,uint EventValue,string? Alternate,bool Cacheable,int BinBytes,int RelocationBytes,string BinSha256,string RelocationSha256);
    internal sealed record Report(uint ProcessingHash,uint DocumentVersion,string SourceSha256,string HeaderSha256,string DiagnosticFingerprint,Row[] Rows)
    {
        public bool ReadOnly => true;
        public bool SnapshotOnly => true;
        public bool SyntheticProcessingDomain => true;
        public bool Ep1ProcessingHashRecovered => false;
        public bool OfficialAudioDependenciesResolved => false;
        public bool ProductionBuildReady => false;
    }
    private readonly string _directory;
    private readonly XmlSchemaSet _schemas;
    private readonly PathMusicAuthoredSnapshot _snapshot;
    private readonly PathMusicCoreIdentity.Report _core;

    //-------------------------------------------------------------------------------------------------
    /** Reborn: retain only privately owned snapshots and detached Core value records, never a mutable InstanceDeclaration or exported schema set. */
    //-------------------------------------------------------------------------------------------------
    private PathMusicCorePreparation(string directory,XmlSchemaSet schemas,PathMusicAuthoredSnapshot snapshot,PathMusicCoreIdentity.Report core)
    { _directory = directory; _schemas = schemas; _snapshot = snapshot; _core = core with { Rows = core.Rows.ToArray() }; }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: admit one bounded local closure and pair its native snapshot with actual Core identity only after raw fingerprint and ordered owner agreement. */
    //-------------------------------------------------------------------------------------------------
    internal static PathMusicCorePreparation Read(string directory,XmlSchemaSet? schemas = null)
    {
        directory = Path.GetFullPath(directory);
        if (schemas == null)
        {
            var reviewed = SdkEffectiveSchema.Inspect(shieldCandidate:true,reviewHooks:true,onAdmitted:set => schemas = set);
            if (!reviewed.SchemaAdmitted || schemas == null) throw new InvalidDataException("Reviewed music schema required.");
        }
        var snapshot = PathMusicAuthoredSnapshot.Read(directory,schemas); var native = snapshot.Preflight();
        var core = PathMusicCoreIdentity.Inspect(directory,schemas);
        if (core.ProcessingHash != PathMusicCoreIdentity.Processing || !core.SyntheticProcessingDomain || core.ProductionBuildReady
            || core.SourceSha256 != native.SourceSha256 || core.HeaderSha256 != native.HeaderSha256 || core.DiagnosticFingerprint != native.DiagnosticFingerprint
            || core.Rows.Length != native.Rows.Length) throw new InvalidDataException("Music Core/native snapshots are not the same admitted closure.");
        for (int index = 0; index < native.Rows.Length; index++)
        {
            var row = native.Rows[index]; var identity = core.Rows[index];
            if (row.Name != identity.Name || row.InstanceId != identity.InstanceId || row.Cacheable != identity.Cacheable
                || identity.CoreInstanceHash != identity.ExpectedInstanceHash || identity.WeakReferences != (row.Alternate == null ? 0 : 1))
                throw new InvalidDataException("Ordered Core music identities differ from native preparation.");
        }
        snapshot.VerifyCurrent(); return new(directory,schemas!,snapshot,core);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: copy identity/native projections without leaking captured arrays or mutable Core/XML objects. */
    //-------------------------------------------------------------------------------------------------
    internal Report Preflight()
    {
        var native = _snapshot.Preflight();
        Row[] rows = native.Rows.Select((row,index) => new Row(row.Name,row.InstanceId,_core.Rows[index].CoreInstanceHash,row.EventValue,row.Alternate,row.Cacheable,
            row.BinBytes,row.RelocationBytes,row.BinSha256,row.RelocationSha256)).ToArray();
        return new(_core.ProcessingHash,_core.DocumentVersion,native.SourceSha256,native.HeaderSha256,native.DiagnosticFingerprint,rows);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: reprocess fresh Core instances and compare every captured identity/default/dependency projection plus exact raw source/header snapshots. */
    //-------------------------------------------------------------------------------------------------
    internal void VerifyCurrent()
    {
        _snapshot.VerifyCurrent(); var current = PathMusicCoreIdentity.Inspect(_directory,_schemas);
        if (current.ProcessingHash != _core.ProcessingHash || current.DocumentVersion != _core.DocumentVersion
            || current.SourceSha256 != _core.SourceSha256 || current.HeaderSha256 != _core.HeaderSha256 || current.DiagnosticFingerprint != _core.DiagnosticFingerprint
            || !current.Rows.SequenceEqual(_core.Rows)) throw new InvalidDataException("Music Core preparation is stale or its identity domain changed.");
        _snapshot.VerifyCurrent();
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: serialize only the captured closure between current-Core/raw checks; return fresh detached chunks and never refresh stale preparation implicitly. */
    //-------------------------------------------------------------------------------------------------
    internal Chunk[] Compile()
    {
        VerifyCurrent(); Chunk[] chunks = _snapshot.Compile(); VerifyCurrent(); return chunks;
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: publish the unchanged diagnostic hash policy only through mandatory current-Core gates before staging and immediately before commit. */
    //-------------------------------------------------------------------------------------------------
    internal void PublishDiagnosticPackage(string output,Action? beforeCommit = null)
    { PathMusicPackageProbe.Publish(output,_snapshot,VerifyCurrent,beforeCommit); }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: bracket independent existing package readback with actual current-Core checks, without relabeling manifest identity fields. */
    //-------------------------------------------------------------------------------------------------
    internal void VerifyDiagnosticPackage(string directory)
    { VerifyCurrent(); PathMusicPackageProbe.Verify(directory,_snapshot); VerifyCurrent(); }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: publish versioned synthetic Core identities from privately owned paired snapshots, keeping legacy diagnostics separate. */
    //-------------------------------------------------------------------------------------------------
    internal void PublishExperimentalPackage(string output,Action? beforeCommit = null)
    { PathMusicPackageProbe.PublishExperimental(output,_snapshot,this,beforeCommit); }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: verify experimental profile membership and current Core/native evidence without authenticating historical publication. */
    //-------------------------------------------------------------------------------------------------
    internal void VerifyExperimentalPackage(string directory)
    { PathMusicPackageProbe.VerifyExperimental(directory,_snapshot,this); }
}
