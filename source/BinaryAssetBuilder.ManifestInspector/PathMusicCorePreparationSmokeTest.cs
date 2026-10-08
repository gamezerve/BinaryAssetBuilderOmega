using System.Text;
using System.Xml.Schema;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: verify immutable Core/native music pairing using owned synthetic source/header files, never official AUDIO or production output.
internal static class PathMusicCorePreparationSmokeTest
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: prove 1/3/8 identity/native pairs, detached reports/chunks, timestamp-preserving stale edits and explicit fresh preparation/restoration. */
    //-------------------------------------------------------------------------------------------------
    internal static void Run()
    {
        XmlSchemaSet? schemas = null; var evidence = SdkEffectiveSchema.Inspect(shieldCandidate:true,reviewHooks:true,onAdmitted:set => schemas = set);
        Require(evidence.SchemaAdmitted && schemas != null,"Reviewed Core music preparation schema unavailable.");
        string directory = Path.Combine(Path.GetTempPath(),"Reborn-CoreMusicPreparation-"+Guid.NewGuid().ToString("N")); Directory.CreateDirectory(directory);
        foreach (int count in new[] { 1,3,8 })
        {
            var fixture = Fixture(count); Write(directory,fixture.Source,fixture.Header); var prepared = PathMusicCorePreparation.Read(directory,schemas);
            var report = prepared.Preflight(); var chunks = prepared.Compile(); var expected = PathMusicCoreIdentity.Inspect(directory,schemas);
            Require(report.Rows.Length == count && report.Rows.Select(row => row.CoreInstanceHash).SequenceEqual(expected.Rows.Select(row => row.CoreInstanceHash))
                && chunks.Length == count && !report.ProductionBuildReady && !report.Ep1ProcessingHashRecovered && !report.OfficialAudioDependenciesResolved
                && report.SyntheticProcessingDomain,"Core/native music binding/readiness differs.");
            Require(report.Rows.SequenceEqual(PathMusicCorePreparation.Read(directory,schemas).Preflight().Rows),"Repeated prepared Core/native identities differ.");
            chunks[0].InstanceBuffer[4] = 99; Require(prepared.Compile()[0].InstanceBuffer[4] == 1,"Prepared music chunk aliases captured native evidence.");
            var frozenRow = report.Rows[0]; report.Rows[0] = frozenRow with { CoreInstanceHash = frozenRow.CoreInstanceHash^1,Name = "forged" };
            Require(prepared.Preflight().Rows[0] == frozenRow,"Prepared report aliases trusted Core/native records.");
            string path = Path.Combine(directory,"events.h"); DateTime stamp = File.GetLastWriteTimeUtc(path);
            File.WriteAllText(path,fixture.Header.Replace("0x00000001","0x00000009"),new UTF8Encoding(false)); File.SetLastWriteTimeUtc(path,stamp);
            Refuses(() => prepared.VerifyCurrent()); Refuses(() => prepared.Compile());
            var refreshed = PathMusicCorePreparation.Read(directory,schemas); Require(refreshed.Preflight().Rows[0].CoreInstanceHash != frozenRow.CoreInstanceHash
                && refreshed.Compile()[0].InstanceBuffer[4] == 9,"Fresh header preparation retained stale Core/native identity.");
            Write(directory,fixture.Source,fixture.Header); prepared.VerifyCurrent();
            // Reborn: a harmless unselected header comment changes Core/file identity despite unchanged native chunks.
            Write(directory,fixture.Source,fixture.Header+"// Reborn: comment only\n"); Refuses(() => prepared.Compile());
            var comment = PathMusicCorePreparation.Read(directory,schemas); var commentRow = comment.Preflight().Rows[0];
            Require(commentRow.CoreInstanceHash != frozenRow.CoreInstanceHash && commentRow.BinSha256 == frozenRow.BinSha256
                && commentRow.RelocationSha256 == frozenRow.RelocationSha256,"Core-only dependency edit was hidden by identical native bytes.");
            Write(directory,fixture.Source,fixture.Header);
            path = Path.Combine(directory,"events.xml"); stamp = File.GetLastWriteTimeUtc(path);
            File.WriteAllText(path,fixture.Source.Replace("IsCacheable='false'","IsCacheable='true' "),new UTF8Encoding(false)); File.SetLastWriteTimeUtc(path,stamp);
            Refuses(() => prepared.Compile()); var changed = PathMusicCorePreparation.Read(directory,schemas);
            Require(changed.Preflight().Rows[0].Cacheable && changed.Preflight().Rows[0].CoreInstanceHash != frozenRow.CoreInstanceHash
                && changed.Compile()[0].InstanceBuffer[12] == 1,"Fresh XML preparation retained old cache/native identity.");
            // Reborn: explicit/default true produce identical runtime chunks but retain distinct Core authored provenance.
            Write(directory,fixture.Source.Replace("IsCacheable='false'","IsCacheable='true'"),fixture.Header);
            var explicitTrue = PathMusicCorePreparation.Read(directory,schemas).Preflight();
            Write(directory,fixture.Source.Replace(" IsCacheable='false'",""),fixture.Header);
            var defaultTrue = PathMusicCorePreparation.Read(directory,schemas).Preflight();
            Require(explicitTrue.Rows[0].CoreInstanceHash != defaultTrue.Rows[0].CoreInstanceHash && explicitTrue.Rows[0].BinSha256 == defaultTrue.Rows[0].BinSha256,
                "Equivalent native cache bytes erased Core default provenance.");
            Write(directory,fixture.Source,fixture.Header); prepared.VerifyCurrent();
            File.Delete(Path.Combine(directory,"events.h")); Refuses(() => prepared.Compile()); Write(directory,fixture.Source,fixture.Header); prepared.VerifyCurrent();
        }
        Console.WriteLine("PathMusic Core preparation self-test: OK (1/3/8 immutable actual Core/native pairs, detached arrays/reports, same-size timestamp-preserving XML/header edits, native-equal Core-only comments/default provenance, missing/stale/refreshed/restored closure; no package/production admission)");
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: construct synthetic bounded local music values and alternates, with no recovered stock/header claim. */
    //-------------------------------------------------------------------------------------------------
    private static (string Source,string Header) Fixture(int count)
    {
        string source = "<AssetDeclaration xmlns='uri:ea.com:eala:asset'>",header = "";
        for (int index = 0; index < count; index++)
        {
            source += "<PathMusicEvent id='RebornPrepared"+index+"' PathfinderEventHeader='events.h'"
                +(index+1 < count ? " RestartAlternateEvent='RebornPrepared"+(index+1)+"'" : "")+(index == 0 ? " IsCacheable='false'" : "")+"/>";
            header += "#define PATH_EVENT_RebornPrepared"+index+" 0x"+(index+1).ToString("X8")+"\n";
        }
        return (source+"</AssetDeclaration>",header);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: write only owned temporary music leaves; preserve all official source/reference inputs. */
    //-------------------------------------------------------------------------------------------------
    private static void Write(string directory,string source,string header)
    { File.WriteAllText(Path.Combine(directory,"events.xml"),source,new UTF8Encoding(false)); File.WriteAllText(Path.Combine(directory,"events.h"),header,new UTF8Encoding(false)); }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: stale or missing inputs cannot silently gain fresh native preparation authority. */
    //-------------------------------------------------------------------------------------------------
    private static void Refuses(Action action)
    { try { action(); } catch (Exception error) when (error is InvalidDataException or IOException) { return; } throw new InvalidDataException("Stale Core music preparation accepted."); }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: fail at the first immutable identity/native closure regression. */
    //-------------------------------------------------------------------------------------------------
    private static void Require(bool condition,string message) { if (!condition) throw new InvalidDataException(message); }
}
