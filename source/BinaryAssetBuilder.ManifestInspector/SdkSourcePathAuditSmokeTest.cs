using System.Security.Cryptography;
using System.Text;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: test confined source graphs and metadata-only resources using owned fixtures, never modifying synced or external source material.
internal static class SdkSourcePathAuditSmokeTest
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: prove shared-node roles, alias fanout, missing/cyclic/unsafe sources and bounded traversal without a production build. */
    //-------------------------------------------------------------------------------------------------
    internal static void Run()
    {
        string fixture = Path.Combine(Path.GetTempPath(),"Reborn-SourcePaths-"+Guid.NewGuid().ToString("N"));
        string data = Path.Combine(fixture,"Data"),art = Path.Combine(fixture,"Art"),audio = Path.Combine(fixture,"Audio"),entry = Path.Combine(data,"Entry.xml");
        Directory.CreateDirectory(data); Directory.CreateDirectory(art); Directory.CreateDirectory(audio);
        Directory.CreateDirectory(Path.Combine(data,"Sub")); Directory.CreateDirectory(Path.Combine(art,"ab")); Directory.CreateDirectory(Path.Combine(art,"Explicit"));
        Write(entry,"<Includes><Include type=\"all\" source=\"Sub/Child.xml\"/><Include type=\"instance\" source=\"DATA:Shared.xml\"/><Include type=\"reference\" source=\"Shared.xml\"/></Includes><Model File=\"ART:abModel.w3x\"/><Model File=\"ART:Explicit/Model.w3x\"/><AudioFile File=\"AUDIO:sound.wav\"/><AudioFile File=\"local.wav\"/><Texture>ART:abTexture.dds</Texture>");
        Write(Path.Combine(data,"Sub","Child.xml"),"<Includes><Include type=\"all\" source=\"../Shared.xml\"/></Includes>"); Write(Path.Combine(data,"Shared.xml"),"");
        foreach (string path in new[] { Path.Combine(art,"ab","abModel.w3x"),Path.Combine(art,"Explicit","Model.w3x"),Path.Combine(art,"ab","abTexture.dds"),Path.Combine(audio,"sound.wav"),Path.Combine(data,"local.wav") }) File.WriteAllBytes(path,new byte[] { 1,2,3 });
        var environment = SdkEnvironmentPreflight.Inspect("ra3ep1",SdkEnvironmentPreflight.BaselineSchemaRoot(),data,entry,Path.Combine(fixture,"Uncreated Output"),Array.Empty<string>());
        var report = SdkSourcePathAudit.Inspect(environment,art,audio);
        Require(report.ScopedPathAuditComplete && report.Sources.Length == 3 && report.Includes.Length == 4 && report.Resources.Length == 5
            && report.Includes.Select(edge => edge.Kind).SequenceEqual(new[] { "all","all","instance","reference" })
            && report.SnapshotOnly && !report.FullDependencyCoverage && !report.ProductionBuildReady,"Source graph roles/resources/flags differ.");
        // Reborn: an exclusively locked resource must still be statted successfully, proving its body is not opened for this audit.
        using (FileStream locked = new(Path.Combine(audio,"sound.wav"),FileMode.Open,FileAccess.ReadWrite,FileShare.None))
            Require(SdkSourcePathAudit.Inspect(environment,art,audio).ScopedPathAuditComplete,"Resource audit tried to open a payload body.");
        Require(!Directory.Exists(environment.OutputDirectory),"Source audit created build output.");
        Require(SdkSourcePathAudit.Inspect(environment).Issues.Any(issue => issue.Code == "ResourcePath"),"Missing explicit alias roots accepted.");
        File.Delete(Path.Combine(audio,"sound.wav")); Require(SdkSourcePathAudit.Inspect(environment,art,audio).Issues.Any(issue => issue.Code == "ResourceMissing"),"Missing resource accepted.");
        Write(Path.Combine(data,"Shared.xml"),"<Includes><Include type=\"all\" source=\"Entry.xml\"/></Includes>");
        Require(SdkSourcePathAudit.Inspect(environment,art,audio).Issues.Any(issue => issue.Code == "IncludeCycle"),"Active cycle accepted.");
        Write(Path.Combine(data,"Shared.xml"),"");
        // Reborn: unsafe paths reject before an OS payload read, including sibling-prefix escapes and Windows device/stream aliases.
        foreach (string logical in new[] { "../DataSibling/out.xml","DATA:../../out.xml","ROOT:Entry.xml","DATA:NUL.xml","DATA:Entry.xml:stream","DATA:%GAME%/Entry.xml","file:///forbidden.xml","DATA:/Entry.xml","ART:x" })
            Reject(() => SdkSourcePathAudit.Resolve(logical,data,data,data,art,audio));
        Reject(() => SdkSourcePathAudit.Inspect(environment,data,audio));
        Write(entry,"<Includes><Include type=\"all\" source=\"missing.xml\"/><Include type=\"all\" source=\"ROOT:Entry.xml\"/></Includes>");
        var broken = SdkSourcePathAudit.Inspect(Current(),art,audio);
        Require(!broken.ScopedPathAuditComplete && broken.Issues.Any(issue => issue.Code == "SourceRead") && broken.Issues.Any(issue => issue.Code == "IncludePath"),"Missing/unsupported Include accepted.");
        Require(SdkSourcePathAudit.Inspect(environment,art,audio).Issues.Any(issue => issue.Code == "StaleEntry"),"Changed entry snapshot accepted.");
        Write(entry,"<Includes><Include type=\"all\" source=\"Shared.xml\"/></Includes>");
        File.WriteAllText(Path.Combine(data,"Shared.xml"),"<!DOCTYPE x [<!ENTITY e SYSTEM 'file:///forbidden'>]><AssetDeclaration xmlns=\"uri:ea.com:eala:asset\">&e;</AssetDeclaration>");
        Require(SdkSourcePathAudit.Inspect(Current(),art,audio).Issues.Any(issue => issue.Code == "SourceRead"),"Child DTD accepted.");
        // Reborn: even a finite valid chain must return incomplete when it exceeds the declared graph depth.
        Write(entry,"<Includes><Include type=\"all\" source=\"Chain0.xml\"/></Includes>");
        for (int index = 0; index < 35; index++) Write(Path.Combine(data,"Chain"+index+".xml"),index == 34 ? "" : "<Includes><Include type=\"all\" source=\"Chain"+(index+1)+".xml\"/></Includes>");
        var bounded = SdkSourcePathAudit.Inspect(Current(),art,audio);
        Require(bounded.StoppedAtLimit && !bounded.ScopedPathAuditComplete && bounded.Issues.Any(issue => issue.Code == "GraphLimit"),"Depth cap reported complete.");
        // Reborn: exercise separate retained-diagnostic, edge and resource caps rather than assuming the depth cap covers them.
        Write(entry,"<Includes>"+string.Concat(Enumerable.Range(0,140).Select(index => "<Include type=\"all\" source=\"missing"+index+".xml\"/>"))+"</Includes>");
        var issueBound = SdkSourcePathAudit.Inspect(Current(),art,audio);
        Require(issueBound.StoppedAtLimit && issueBound.Issues.Length == 128 && issueBound.Issues.Last().Code == "IssueLimit","Issue retention cap differs.");
        Write(Path.Combine(data,"Shared.xml"),"");
        Write(entry,"<Includes>"+string.Concat(Enumerable.Repeat("<Include type=\"all\" source=\"Shared.xml\"/>",4097))+"</Includes>");
        var edgeBound = SdkSourcePathAudit.Inspect(Current(),art,audio);
        Require(edgeBound.StoppedAtLimit && edgeBound.Includes.Length == 4096 && !edgeBound.ScopedPathAuditComplete,"Include edge cap differs.");
        Write(entry,string.Concat(Enumerable.Repeat("<AudioFile File=\"local.wav\"/>",2049)));
        var resourceBound = SdkSourcePathAudit.Inspect(Current(),art,audio);
        Require(resourceBound.StoppedAtLimit && resourceBound.Resources.Length == 2048 && resourceBound.Issues.Single().Code == "ResourceLimit","Resource cap differs.");
        Console.WriteLine("SDK source-path preflight self-test: OK (Include roles/shared nodes/cycles, DATA/ART fanout/AUDIO, metadata-only locked resource, missing/unsafe/DTD/stale/depth rejection; not schema-complete or a build)");

        //-------------------------------------------------------------------------------------------------
        /** Reborn: refresh only the fixture entry fingerprint to isolate graph checks from the separately tested environment catalog. */
        //-------------------------------------------------------------------------------------------------
        SdkEnvironmentPreflight.Report Current() => environment with { SourceEntrySha256 = Convert.ToHexString(SHA256.HashData(File.ReadAllBytes(entry))) };
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: generate minimal owned EA source XML with explicit literal path fixtures. */
    //-------------------------------------------------------------------------------------------------
    private static void Write(string path,string body) => File.WriteAllText(path,"<AssetDeclaration xmlns=\"uri:ea.com:eala:asset\">"+body+"</AssetDeclaration>",new UTF8Encoding(false));

    //-------------------------------------------------------------------------------------------------
    /** Reborn: path syntax and confinement violations must reject without legacy fallback. */
    //-------------------------------------------------------------------------------------------------
    private static void Reject(Action action)
    { try { action(); } catch (Exception error) when (error is InvalidDataException or IOException or ArgumentException or NotSupportedException) { return; } throw new InvalidDataException("Unsafe source path accepted."); }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: stop at the first disagreement between bounded planning evidence and expected fixture behavior. */
    //-------------------------------------------------------------------------------------------------
    private static void Require(bool condition,string message) { if (!condition) throw new InvalidDataException(message); }
}
