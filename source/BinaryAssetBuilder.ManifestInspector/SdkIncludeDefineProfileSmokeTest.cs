using System.Text;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: prove imported literals are source-confined, origin-aware, atomic and separate from reference/override/general evaluator behavior.
internal static class SdkIncludeDefineProfileSmokeTest
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: exercise an owned diamond graph with same-origin coalescing and reject changed, absent, ambiguous or unsupported closures. */
    //-------------------------------------------------------------------------------------------------
    internal static void Run()
    {
        string root = Path.Combine(Path.GetTempPath(),"Reborn-IncludeDefines-"+Guid.NewGuid().ToString("N")),data = Path.Combine(root,"Data"); Directory.CreateDirectory(data);
        // Reborn: keep defining XML in a subdirectory so literal file references must resolve from the consuming asset, not the definition's folder.
        Directory.CreateDirectory(Path.Combine(data,"Defs"));
        string entry = Path.Combine(data,"Entry.xml"),left = Path.Combine(data,"Left.xml"),right = Path.Combine(data,"Right.xml"),leaf = Path.Combine(data,"Defs","Leaf.xml"),blob = Path.Combine(data,"local.bin");
        string parent = "<Includes><Include type='all' source='Left.xml'/><Include type='instance' source='Right.xml'/></Includes><Asset File='=$FILE'/>";
        Write(entry,parent); Write(left,"<Includes><Include type='instance' source='Defs/Leaf.xml'/></Includes>"); Write(right,"<Includes><Include type='all' source='Defs/Leaf.xml'/></Includes>"); Write(leaf,"<Defines><Define name='FILE' value='local.bin'/></Defines>"); File.WriteAllBytes(blob,new byte[] { 1 });
        var paths = Audit(); var result = new SdkIncludeDefineProfile(paths).Apply(entry,File.ReadAllBytes(entry));
        Require(result.Bytes != null && result.Evidence.Profile == SdkIncludeDefineProfile.Name && result.Evidence.Substitutions == 1
            && result.Evidence.DefinitionSources.Length == 4 && result.Evidence.DefinitionOrigins.Single().SourcePath == leaf
            && result.Evidence.ProcessedSha256 != result.Evidence.RawSha256,"Diamond literal closure/source origin differs.");
        Require(SdkLocalDefineProfile.Apply(File.ReadAllBytes(entry)).Bytes == null,"Local-only profile silently gained imported visibility.");
        const string xsd = "<xs:schema xmlns:xs='http://www.w3.org/2001/XMLSchema' xmlns='uri:ea.com:eala:asset' targetNamespace='uri:ea.com:eala:asset' elementFormDefault='qualified'><xs:simpleType name='FileReference'><xs:restriction base='xs:anyURI'/></xs:simpleType><xs:element name='AssetDeclaration'><xs:complexType><xs:sequence><xs:element name='Includes' type='xs:anyType' minOccurs='0'/><xs:element name='Defines' type='xs:anyType' minOccurs='0'/><xs:element name='Asset' minOccurs='0'><xs:complexType><xs:attribute name='File' type='FileReference'/></xs:complexType></xs:element></xs:sequence></xs:complexType></xs:element></xs:schema>";
        var schemas = SdkEffectiveSchema.Compile(new Dictionary<string,byte[]> { ["entry.xsd"] = Encoding.UTF8.GetBytes(xsd) },"entry.xsd");
        var graph = SdkTypedSourceGraph.BindGraph(schemas.Schemas,paths,includeDefines:true);
        Require(graph.ScopedGraphComplete && graph.PreprocessingProfile == SdkIncludeDefineProfile.Name && !graph.ProductionBuildReady
            && graph.Documents.Single(document => document.SourcePath == entry).Dependencies.Single().Bytes == 1
            && File.ReadAllText(entry).Contains("=$FILE",StringComparison.Ordinal) && !Directory.Exists(Path.Combine(root,"NoOutput")),"Imported graph did not isolate source/processed/production evidence.");
        Write(leaf,"<Defines><Define name='FILE' value='changed.bin'/></Defines>"); Reject(paths);
        File.Delete(leaf); Reject(paths);
        Write(leaf,"<Defines><Define name='FILE' value='local.bin'/></Defines>"); paths = Audit();
        Reject(paths with { Includes = paths.Includes.Where(edge => edge.Document != left).ToArray() });
        Reject(paths with { Includes = paths.Includes.Select(edge => edge.Document == left ? edge with { PhysicalPath = entry } : edge).ToArray() });
        Reject(paths with { StoppedAtLimit = true });
        // Reborn: cross-origin duplicates fail even for equal values; no first/last-wins or accidental override behavior.
        Write(right,"<Defines><Define name='FILE' value='local.bin'/></Defines>"); Reject(Audit());
        Write(right,"<Includes><Include type='all' source='Defs/Leaf.xml'/></Includes>");
        Write(entry,parent.Replace("<Asset","<Defines><Define name='FILE' value='local.bin'/></Defines><Asset",StringComparison.Ordinal)); Reject(Audit());
        Write(entry,parent.Replace("<Asset","<Defines><Define name='FILE' value='local.bin' override='true'/></Defines><Asset",StringComparison.Ordinal)); Reject(Audit());
        Write(entry,parent.Replace("type='all'","type='reference'",StringComparison.Ordinal)); Reject(Audit()); Write(entry,parent);
        Write(leaf,"<Defines><Define name='FILE' value='=$OTHER'/></Defines>"); Reject(Audit());
        Write(leaf,"<Defines><Define name='FILE' value='local.bin'/><Define name='UNUSED' value='=1+2'/></Defines>"); Reject(Audit());
        Write(leaf,"<Includes><Include type='all' source='../Left.xml'/></Includes><Defines><Define name='FILE' value='local.bin'/></Defines>"); Reject(Audit());
        Write(leaf,"<Asset inheritFrom='Base'/><Defines><Define name='FILE' value='local.bin'/></Defines>"); Reject(Audit());
        Write(leaf,"<Defines><Define name='FILE' value='local.bin'/></Defines>"); paths = Audit();
        bool forgedRejected = false;
        try { _ = new SdkIncludeDefineProfile(paths with { Sources = paths.Sources.Concat(new[] { paths.Sources[0] }).ToArray() }); } catch (InvalidDataException) { forgedRejected = true; }
        Require(forgedRejected,"Ambiguous imported inventory accepted.");
        // Reborn: forged noncanonical root escape is rejected by the constructor before imported source IO.
        bool escapeRejected = false;
        try { _ = new SdkIncludeDefineProfile(paths with { Sources = new[] { paths.Sources[0] with { PhysicalPath = Path.Combine(data,"..","outside.xml") } } }); } catch (InvalidDataException) { escapeRejected = true; }
        Require(escapeRejected,"Escaped imported inventory accepted.");
        bool profilesRejected = false;
        try { _ = SdkTypedSourceGraph.BindGraph(schemas.Schemas,paths,true,true); } catch (ArgumentException) { profilesRejected = true; }
        Require(profilesRejected,"Mutually exclusive graph profiles accepted.");
        Write(leaf,"<Defines><Define name='FILE' value='../outside.bin'/></Defines>");
        var unsafeGraph = SdkTypedSourceGraph.BindGraph(schemas.Schemas,Audit(),includeDefines:true);
        Require(!unsafeGraph.ScopedGraphComplete && unsafeGraph.Documents.Single(document => document.SourcePath == entry).Dependencies.Single().Issue == "ResourcePath","Imported literal escaped physical resource confinement.");
        Console.WriteLine("SDK Include define profile self-test: OK (all/instance diamond origins, source hashes, atomic stale/missing/edge/cycle/duplicate/local-collision/override/chain/arithmetic/reference/inheritance rejection, typed graph and physical confinement; no production evaluator)");

        //-------------------------------------------------------------------------------------------------
        /** Reborn: capture only the owned current source graph while leaving the output directory absent. */
        //-------------------------------------------------------------------------------------------------
        SdkSourcePathAudit.Report Audit() => SdkSourcePathAudit.Inspect(SdkEnvironmentPreflight.Inspect("ra3ep1",SdkEnvironmentPreflight.BaselineSchemaRoot(),data,entry,Path.Combine(root,"NoOutput"),Array.Empty<string>()));

        //-------------------------------------------------------------------------------------------------
        /** Reborn: rejected Include preprocessing must retain only raw identity, never partial bytes/digests/origins. */
        //-------------------------------------------------------------------------------------------------
        void Reject(SdkSourcePathAudit.Report captured)
        {
            var rejected = new SdkIncludeDefineProfile(captured).Apply(entry,File.ReadAllBytes(entry));
            Require(rejected.Bytes == null && rejected.Evidence.ProcessedSha256 == null && rejected.Evidence.Substitutions == 0
                && rejected.Evidence.DefinitionSources.Length == 0 && rejected.Evidence.DefinitionOrigins.Length == 0,"Rejected imported closure published partial evidence.");
        }
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: create only fresh owned minimal EA fixture documents, never change official/reference XML. */
    //-------------------------------------------------------------------------------------------------
    private static void Write(string path,string body) => File.WriteAllText(path,"<AssetDeclaration xmlns='uri:ea.com:eala:asset'>"+body+"</AssetDeclaration>",new UTF8Encoding(false));

    //-------------------------------------------------------------------------------------------------
    /** Reborn: fail diagnostic regression checks without implying that literal binding proves child compilation or game compatibility. */
    //-------------------------------------------------------------------------------------------------
    private static void Require(bool condition,string message) { if (!condition) throw new InvalidDataException(message); }
}
