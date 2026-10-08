using System.Text;
using System.Xml;
using System.Xml.Schema;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: exercise local authored content identities and stale-input rejection using owned temporary music/header fixtures only.
internal static class PathMusicAuthoredSnapshotSmokeTest
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: validate 1/3/8 local closures, native output detachment, preserved-timestamp edits and strict source/header/identity/alternate gates. */
    //-------------------------------------------------------------------------------------------------
    internal static void Run()
    {
        XmlSchemaSet? schemas = null; var schema = SdkEffectiveSchema.Inspect(shieldCandidate:true,reviewHooks:true,onAdmitted:set => schemas = set);
        Require(schema.SchemaAdmitted && schemas != null,"Reviewed schema unavailable.");
        string directory = Path.Combine(Path.GetTempPath(),"Reborn-AuthoredMusic-"+Guid.NewGuid().ToString("N")); Directory.CreateDirectory(directory);
        foreach (int count in new[] { 1,3,8 })
        {
            var fixture = Fixture(count); Write(directory,fixture.Source,fixture.Header);
            var snapshot = PathMusicAuthoredSnapshot.Read(directory,schemas); var report = snapshot.Preflight(); var chunks = snapshot.Compile();
            Require(report.Rows.Length == count && report.Rows[0].EventValue == 1 && !report.Rows[0].Cacheable && chunks.All(chunk => chunk.ImportsBuffer.Length == 0)
                && !report.ProductionBuildReady && !report.OfficialAudioDependenciesResolved,"Local music identity/readiness changed.");
            Require(report.DiagnosticFingerprint == PathMusicAuthoredSnapshot.Read(directory,schemas).Preflight().DiagnosticFingerprint,"Repeated music content identity differs.");
            chunks[0].InstanceBuffer[4] = 99; Require(snapshot.Compile()[0].InstanceBuffer[4] == 1,"Native chunk aliases frozen preparation.");
            report.Rows[0] = report.Rows[0] with { Name = "forged" }; Require(snapshot.Preflight().Rows[0].Name == "RebornMusic0","Report rows alias private state.");
            string path = Path.Combine(directory,"events.h"); DateTime stamp = File.GetLastWriteTimeUtc(path);
            File.WriteAllText(path,fixture.Header.Replace("0x00000001","0x00000009"),new UTF8Encoding(false)); File.SetLastWriteTimeUtc(path,stamp);
            Refuses(() => snapshot.VerifyCurrent()); Refuses(() => snapshot.Compile());
            var refreshed = PathMusicAuthoredSnapshot.Read(directory,schemas).Preflight();
            Require(refreshed.Rows[0].EventValue == 9 && refreshed.DiagnosticFingerprint != snapshot.Preflight().DiagnosticFingerprint,"Current header did not invalidate diagnostic identity.");
            Write(directory,fixture.Source,fixture.Header); snapshot.VerifyCurrent();
            path = Path.Combine(directory,"events.xml"); stamp = File.GetLastWriteTimeUtc(path);
            File.WriteAllText(path,fixture.Source.Replace("IsCacheable='false'","IsCacheable='true' "),new UTF8Encoding(false)); File.SetLastWriteTimeUtc(path,stamp);
            Refuses(() => snapshot.Compile()); Require(PathMusicAuthoredSnapshot.Read(directory,schemas).Preflight().Rows[0].Cacheable,"Current source cache setting did not refresh.");
        }
        var single = Fixture(1);
        // Reborn: exact numeric boolean forms and schema default retain their intended native cache byte.
        foreach (string literal in new[] { "true","false","1","0" })
        {
            Write(directory,single.Source.Replace("IsCacheable='false'","IsCacheable='"+literal+"'"),single.Header);
            Require(PathMusicAuthoredSnapshot.Read(directory,schemas).Preflight().Rows[0].Cacheable == (literal is "true" or "1"),"Boolean cache literal changed.");
        }
        Write(directory,single.Source.Replace(" IsCacheable='false'",""),single.Header);
        Require(PathMusicAuthoredSnapshot.Read(directory,schemas).Preflight().Rows[0].Cacheable,"Default cache value changed.");
        foreach (string source in new[] { single.Source.Replace("events.h","AUDIO:events.h"),single.Source.Replace("events.h","../events.h"),
            single.Source.Replace("events.h","D:/events.h"),single.Source.Replace("PathMusicEvent","PathMusicEventRuntime"),
            single.Source.Replace(" id="," inheritFrom='Other' id="),single.Source.Replace("</AssetDeclaration>","<Includes/></AssetDeclaration>"),
            single.Source.Replace("id='RebornMusic0'","id='../unsafe'"),single.Source.Replace("IsCacheable='false'","IsCacheable='maybe'"),
            single.Source.Replace("IsCacheable='false'","IsCacheable=' true '"),single.Source.Replace("IsCacheable='false'","IsCacheable=''"),
            single.Source.Replace(" id="," RestartAlternateEvent='External' id="),single.Source.Replace(" id="," RestartAlternateEvent='' id="),
            "<!DOCTYPE x [<!ENTITY e 'bad'>]>"+single.Source })
        { Write(directory,source,single.Header); Refuses(() => PathMusicAuthoredSnapshot.Read(directory,schemas)); }
        var pair = Fixture(2); Write(directory,pair.Source.Replace("RebornMusic1","rebornmusic0"),pair.Header); Refuses(() => PathMusicAuthoredSnapshot.Read(directory,schemas));
        var tooMany = Fixture(9); Write(directory,tooMany.Source,tooMany.Header); Refuses(() => PathMusicAuthoredSnapshot.Read(directory,schemas));
        foreach (string header in new[] { "",single.Header.Replace("0x00000001","0x00000000"),single.Header+single.Header,"// "+single.Header,single.Header.Replace("0x00000001","0xZZ") })
        { Write(directory,single.Source,header); Refuses(() => PathMusicAuthoredSnapshot.Read(directory,schemas)); }
        Write(directory,single.Source,single.Header); File.Delete(Path.Combine(directory,"events.h")); Refuses(() => PathMusicAuthoredSnapshot.Read(directory,schemas));
        Console.WriteLine("PathMusic authored snapshot self-test: OK (1/3/8 schema-validated local closures, content fingerprints/native detachment, timestamp-preserving edits/stale compile/refreshed identity, source/header/alias/inheritance/Include/runtime/DTD/identity/alternate/budget/missing/zero/duplicate gates; no official dependency/processor/package admission)");
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: generate arbitrary synthetic local music values, not recovered stock event constants or an official header replacement. */
    //-------------------------------------------------------------------------------------------------
    private static (string Source,string Header) Fixture(int count)
    {
        string source = "<AssetDeclaration xmlns='uri:ea.com:eala:asset'>",header = "";
        for (int index = 0; index < count; index++)
        {
            source += "<PathMusicEvent id='RebornMusic"+index+"' PathfinderEventHeader='events.h'"+(index == 0 ? " IsCacheable='false'" : "")
                +(index+1 < count ? " RestartAlternateEvent='RebornMusic"+(index+1)+"'" : "")+"/>";
            header += "#define PATH_EVENT_RebornMusic"+index+" 0x"+(index+1).ToString("X8")+"\n";
        }
        return (source+"</AssetDeclaration>",header);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: write only freshly owned temporary fixtures; reference XML/header paths remain untouched. */
    //-------------------------------------------------------------------------------------------------
    private static void Write(string directory,string source,string header)
    { File.WriteAllText(Path.Combine(directory,"events.xml"),source,new UTF8Encoding(false)); File.WriteAllText(Path.Combine(directory,"events.h"),header,new UTF8Encoding(false)); }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: failed input admission or stale evidence cannot become successful native preparation. */
    //-------------------------------------------------------------------------------------------------
    private static void Refuses(Action action)
    {
        try { action(); } catch (Exception error) when (error is InvalidDataException or IOException or XmlException or DecoderFallbackException) { return; }
        throw new InvalidDataException("Unsupported authored music accepted.");
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: stop on content identity, closure or immutable-preparation mismatch. */
    //-------------------------------------------------------------------------------------------------
    private static void Require(bool condition,string message) { if (!condition) throw new InvalidDataException(message); }
}
