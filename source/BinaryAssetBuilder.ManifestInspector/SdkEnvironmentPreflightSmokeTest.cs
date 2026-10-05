using System.Text;
using System.Xml;
using BinaryAssetBuilder.Core;
using BinaryAssetBuilder.Utility;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: prove SDK environment planning is read-only, explicit-target and bounded without invoking a builder or native toolchain.
internal static class SdkEnvironmentPreflightSmokeTest
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: check stable path/schema evidence, no output/settings mutation, and schema/source/manifest/lookup rejection. */
    //-------------------------------------------------------------------------------------------------
    internal static void Run()
    {
        string root = Path.Combine(Path.GetTempPath(),"Reborn-SdkPreflight-"+Guid.NewGuid().ToString("N")); Directory.CreateDirectory(root);
        string sources = Path.Combine(root,"Source Root"),entry = Path.Combine(sources,"Entry.xml"),output = Path.Combine(root,"New Output"); Directory.CreateDirectory(sources);
        byte[] source = Encoding.UTF8.GetBytes("<AssetDeclaration xmlns=\"uri:ea.com:eala:asset\"><Includes><Include type=\"all\" source=\"not-validated.xml\" /></Includes></AssetDeclaration>");
        File.WriteAllBytes(entry,source); string baseline = SdkEnvironmentPreflight.BaselineSchemaRoot(); var settings = Settings.Current;
        var report = SdkEnvironmentPreflight.Inspect("ra3ep1",baseline,sources,entry,output,Array.Empty<string>());
        Require(report.ReadOnly && report.SnapshotOnly && report.SchemaCatalogMatches && !report.ProductionBuildReady && !report.IncludedSourcesValidated
            && report.SchemaFiles == 843 && report.SchemaCatalogSha256.Length == 64 && report.ExternalMappings.Length == 0,"SDK planning flags/catalog differ.");
        Require(!Directory.Exists(output) && File.ReadAllBytes(entry).SequenceEqual(source) && ReferenceEquals(Settings.Current,settings),"SDK preflight wrote output/source or mutated settings.");
        string clone = Path.Combine(root,"Schema Clone"); Directory.CreateDirectory(clone);
        // Reborn: copy staged references only into an owned fixture; tests never modify original schema/source material.
        foreach (string path in Directory.EnumerateFiles(baseline,"*.xsd",SearchOption.AllDirectories))
        { string target = Path.Combine(clone,Path.GetRelativePath(baseline,path)); Directory.CreateDirectory(Path.GetDirectoryName(target)!); File.Copy(path,target); }
        Require(SdkEnvironmentPreflight.Inspect("ra3ep1",clone,sources,entry,output,Array.Empty<string>()).SchemaCatalogSha256 == report.SchemaCatalogSha256,"SDK schema fingerprint depends on the machine path.");
        string changed = Path.Combine(clone,"CnC3Types.xsd"); byte[] original = File.ReadAllBytes(changed);
        File.WriteAllBytes(changed,original.Concat(Encoding.UTF8.GetBytes("\n<!-- changed -->")).ToArray()); Reject(() => Inspect(clone)); File.WriteAllBytes(changed,original);
        File.Move(changed,changed+".missing"); Reject(() => Inspect(clone)); File.Move(changed+".missing",changed);
        File.WriteAllBytes(Path.Combine(clone,"Unreviewed.xsd"),original); Reject(() => Inspect(clone)); File.Delete(Path.Combine(clone,"Unreviewed.xsd"));
        foreach (string target in new[] { "ra3","kw","RA3EP1","" }) Reject(() => SdkEnvironmentPreflight.Inspect(target,baseline,sources,entry,output,Array.Empty<string>()));
        string ra3 = Path.GetFullPath(Path.Combine(baseline,"..","..","ra3","xsd")); Reject(() => Inspect(ra3));
        Reject(() => SdkEnvironmentPreflight.Inspect("ra3ep1","schemas/ra3ep1/xsd",sources,entry,output,Array.Empty<string>()));
        Reject(() => SdkEnvironmentPreflight.Inspect("ra3ep1",baseline,sources,entry,"D:Temp",Array.Empty<string>()));
        foreach (string destination in new[] { entry,sources,Path.Combine(sources,"output"),Path.Combine(baseline,"output"),Path.Combine(root,"missing","output") })
            Reject(() => SdkEnvironmentPreflight.Inspect("ra3ep1",baseline,sources,entry,destination,Array.Empty<string>()));
        string sibling = sources+"Sibling"; Directory.CreateDirectory(sibling); string outside = Path.Combine(sibling,"Entry.xml"); File.WriteAllBytes(outside,source);
        Reject(() => SdkEnvironmentPreflight.Inspect("ra3ep1",baseline,sources,outside,output,Array.Empty<string>()));
        // Reborn: device/ADS negatives reject by path syntax before any OS read; never open an actual console, pipe or reserved device.
        foreach (string unsafeEntry in new[] { Path.Combine(sources,"NUL.xml"),Path.Combine(sources,"COM1.extra.xml"),entry+":alternate.xml",@"\\.\NUL.xml" })
            Reject(() => SdkEnvironmentPreflight.Inspect("ra3ep1",baseline,sources,unsafeEntry,output,Array.Empty<string>()));
        foreach (byte[] invalid in new[] { Encoding.UTF8.GetBytes("<Other/>"),Encoding.UTF8.GetBytes("<!DOCTYPE x [<!ENTITY e SYSTEM 'file:///forbidden'>]><AssetDeclaration xmlns=\"uri:ea.com:eala:asset\">&e;</AssetDeclaration>"),new byte[4*1048576+1] })
        { File.WriteAllBytes(entry,invalid); Reject(() => Inspect(baseline)); } File.WriteAllBytes(entry,source);
        string manifest = Path.Combine(root,"external.manifest"); InstanceHandle asset = new("FXList","PreflightFX"); ExternalLinkSmokeTest.WriteFixture(manifest,asset,new ReferencedFileBuffer());
        var mapped = Inspect(baseline,new[] { manifest+"=Base/External.manifest" });
        Require(mapped.ExternalMappings.Single().RuntimeManifest == "base\\external.manifest" && mapped.ExternalMappings.Single().AssetCount == 1
            && !File.Exists(Path.ChangeExtension(manifest,".bin")),"SDK metadata mapping inferred physical BIN availability.");
        foreach (string runtime in new[] { "../bad.manifest","C:\\bad.manifest","bad//file.manifest","=bad.manifest","%GAME%/bad.manifest","bad.bin","bad;file.manifest" }) Reject(() => Inspect(baseline,new[] { manifest+"="+runtime }));
        Reject(() => Inspect(baseline,new[] { manifest+"=one.manifest",manifest+"=two.manifest" }));
        string second = Path.Combine(root,"second.manifest"); File.Copy(manifest,second); Reject(() => Inspect(baseline,new[] { manifest+"=same.manifest",second+"=SAME.manifest" }));
        Reject(() => Inspect(baseline,Enumerable.Repeat(manifest+"=same.manifest",9).ToArray()));
        ExternalLinkSmokeTest.WriteFixture(second,asset,new ReferencedFileBuffer(),0x54EEE764u); Reject(() => Inspect(baseline,new[] { second+"=wrong.manifest" }));
        ReferencedFileBuffer patch = new(); patch.AddReference("base.manifest",true); ExternalLinkSmokeTest.WriteFixture(second,asset,patch); Reject(() => Inspect(baseline,new[] { second+"=patch.manifest" }));
        File.WriteAllBytes(second,Convert.FromHexString("90FB02000000")); Reject(() => Inspect(baseline,new[] { second+"=oversized.manifest" }));
        Require(!Directory.Exists(output) && ReferenceEquals(settings,Settings.Current) && File.ReadAllBytes(entry).SequenceEqual(source),"SDK rejection mutated caller inputs/settings/output.");
        Console.WriteLine("SDK environment preflight self-test: OK (explicit EP1 target/absolute roots, stable 843-XSD metadata fingerprint, source/output/DTD/schema/manifest/runtime rejection, no output/settings/native/registry changes; not a build)");

        //-------------------------------------------------------------------------------------------------
        /** Reborn: keep every planning test on explicit owned source/output roots while varying only reviewed schema/mapping evidence. */
        //-------------------------------------------------------------------------------------------------
        SdkEnvironmentPreflight.Report Inspect(string schema,string[]? mappings = null) => SdkEnvironmentPreflight.Inspect("ra3ep1",schema,sources,entry,output,mappings ?? Array.Empty<string>());
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: malformed target/path/metadata evidence must reject rather than silently falling back to legacy discovery. */
    //-------------------------------------------------------------------------------------------------
    private static void Reject(Action action)
    { try { action(); } catch (Exception error) when (error is InvalidDataException or IOException or NotSupportedException or ArgumentException or XmlException) { return; } throw new InvalidDataException("Invalid SDK preflight evidence accepted."); }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: stop at the first mismatch between immutable expected inputs and read-only SDK evidence. */
    //-------------------------------------------------------------------------------------------------
    private static void Require(bool condition,string message) { if (!condition) throw new InvalidDataException(message); }
}
