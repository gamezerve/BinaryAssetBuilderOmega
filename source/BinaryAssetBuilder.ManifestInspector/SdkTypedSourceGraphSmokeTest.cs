using System.Text;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: prove source graph type binding is snapshot-confined and distinguishes valid XML, preprocessing, stale bytes and metadata-only resources.
internal static class SdkTypedSourceGraphSmokeTest
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: exercise owned two-document graph fixtures without altering reference sources, reading payload bodies or compiling assets. */
    //-------------------------------------------------------------------------------------------------
    internal static void Run()
    {
        string root = Path.Combine(Path.GetTempPath(),"Reborn-TypedGraph-"+Guid.NewGuid().ToString("N")),data = Path.Combine(root,"Data");
        Directory.CreateDirectory(Path.Combine(data,"Sub")); string entry = Path.Combine(data,"Entry.xml"),child = Path.Combine(data,"Sub","Child.xml"),blob = Path.Combine(data,"local.bin");
        Write(entry,"<Includes><Include type='all' source='Sub/Child.xml'/></Includes><Asset id='One' File='local.bin'/>"); Write(child,"<Asset id='Two' File='../local.bin'/>"); File.WriteAllBytes(blob,new byte[] { 1,2,3 });
        const string xsd = "<xs:schema xmlns:xs='http://www.w3.org/2001/XMLSchema' xmlns='uri:ea.com:eala:asset' targetNamespace='uri:ea.com:eala:asset' elementFormDefault='qualified'><xs:simpleType name='FileReference'><xs:restriction base='xs:anyURI'/></xs:simpleType><xs:complexType name='Asset'><xs:attribute name='id' type='xs:string' use='required'/><xs:attribute name='File' type='FileReference' use='required'/><xs:attribute name='inheritFrom' type='xs:string'/></xs:complexType><xs:element name='AssetDeclaration'><xs:complexType><xs:sequence><xs:element name='Includes' minOccurs='0'><xs:complexType><xs:sequence><xs:element name='Include' maxOccurs='unbounded'><xs:complexType><xs:attribute name='type' type='xs:string'/><xs:attribute name='source' type='xs:anyURI'/></xs:complexType></xs:element></xs:sequence></xs:complexType></xs:element><xs:element name='Asset' type='Asset' maxOccurs='unbounded'/></xs:sequence></xs:complexType></xs:element></xs:schema>";
        var schemas = SdkEffectiveSchema.Compile(new Dictionary<string,byte[]> { ["entry.xsd"] = Encoding.UTF8.GetBytes(xsd) },"entry.xsd");
        var environment = SdkEnvironmentPreflight.Inspect("ra3ep1",SdkEnvironmentPreflight.BaselineSchemaRoot(),data,entry,Path.Combine(root,"Uncreated Output"),Array.Empty<string>());
        var paths = SdkSourcePathAudit.Inspect(environment); var graph = SdkTypedSourceGraph.BindGraph(schemas.Schemas,paths);
        Require(graph.ScopedGraphComplete && graph.Documents.Length == 2 && graph.Documents.All(document => document.Dependencies.Single().Bytes == 3 && document.Status == "Validated")
            && graph.ReadOnly && graph.SnapshotOnly && !graph.FullDependencyCoverage && !graph.ProductionBuildReady && !Directory.Exists(environment.OutputDirectory),"Valid typed graph metadata/flags differ.");
        // Reborn: a locked payload body must not prevent schema-selected existence/length inspection.
        using (FileStream locked = new(blob,FileMode.Open,FileAccess.ReadWrite,FileShare.None)) Require(SdkTypedSourceGraph.BindGraph(schemas.Schemas,paths).ScopedGraphComplete,"Typed graph opened a resource body.");
        Write(child,"<Asset id='Two' inheritFrom='One'/>");
        Require(SdkTypedSourceGraph.BindGraph(schemas.Schemas,paths).Documents.Any(document => document.Status == "StaleSource" && document.Dependencies.Length == 0),"Changed source bytes trusted.");
        Require(SdkTypedSourceGraph.BindGraph(schemas.Schemas,SdkSourcePathAudit.Inspect(environment)).Documents.Any(document => document.Status == "RequiresPreprocessing" && document.Dependencies.Length == 0),"Raw inherited asset trusted without preprocessing.");
        Write(child,"<Asset id='Two' File='../local.bin' Unexpected='bad'/>");
        Require(SdkTypedSourceGraph.BindGraph(schemas.Schemas,SdkSourcePathAudit.Inspect(environment)).Documents.Any(document => document.Status == "SchemaInvalid" && document.Dependencies.Length == 0),"Invalid XML published partial typed fields.");
        Write(child,"<Asset id='Two' File='../local.bin'/>"); paths = SdkSourcePathAudit.Inspect(environment); File.Delete(blob);
        var missing = SdkTypedSourceGraph.BindGraph(schemas.Schemas,paths);
        Require(!missing.ScopedGraphComplete && missing.Documents.All(document => document.Status == "Validated" && document.Dependencies.Single().Issue == "ResourceMissing"),"Missing resources confused with XML validity.");
        File.WriteAllBytes(blob,new byte[] { 1,2,3 });
        // Reborn: an expression can be valid xs:anyURI text but is not admitted as a literal physical dependency path.
        Write(child,"<Asset id='Two' File='=$DATA'/>");
        var expression = SdkTypedSourceGraph.BindGraph(schemas.Schemas,SdkSourcePathAudit.Inspect(environment));
        Require(!expression.ScopedGraphComplete && expression.Documents.Any(document => document.Status == "Validated" && document.Dependencies.Any(field => field.Issue == "ResourcePath")),"Schema-valid expression silently resolved as a file.");
        Write(child,"<Asset id='Two' File='../local.bin'/>"); paths = SdkSourcePathAudit.Inspect(environment);
        var partial = SdkTypedSourceGraph.BindGraph(schemas.Schemas,paths with { StoppedAtLimit = true,ScopedPathAuditComplete = false });
        Require(partial.StoppedAtLimit && !partial.ScopedGraphComplete && partial.Documents.All(document => document.Status == "Validated"),"Prior path limit mislabeled valid source bindings.");
        Reject(() => SdkTypedSourceGraph.BindGraph(schemas.Schemas,paths with { Sources = paths.Sources.Concat(new[] { paths.Sources[0] }).ToArray() }));
        var escaped = paths.Sources[0] with { PhysicalPath = Path.Combine(data,"..","outside.xml") };
        Reject(() => SdkTypedSourceGraph.BindGraph(schemas.Schemas,paths with { Sources = new[] { escaped } }));
        File.Delete(child); Require(SdkTypedSourceGraph.BindGraph(schemas.Schemas,paths).Documents.Any(document => document.Status == "SourceRead"),"Deleted captured source trusted.");
        Console.WriteLine("SDK typed source graph self-test: OK (rechecked/confined two-source graph, relative typed files, locked-payload metadata, stale/inheritance/schema/missing/partial/duplicate/escape/read rejection; no production closure)");
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: generate only owned minimal EA source fixtures; never mutate synced or official source XML. */
    //-------------------------------------------------------------------------------------------------
    private static void Write(string path,string body) => File.WriteAllText(path,"<AssetDeclaration xmlns='uri:ea.com:eala:asset'>"+body+"</AssetDeclaration>",new UTF8Encoding(false));

    //-------------------------------------------------------------------------------------------------
    /** Reborn: malformed source inventory cannot create new read authority outside explicitly captured roots. */
    //-------------------------------------------------------------------------------------------------
    private static void Reject(Action action)
    { try { action(); } catch (Exception error) when (error is InvalidDataException or ArgumentException) { return; } throw new InvalidDataException("Invalid typed graph inventory accepted."); }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: keep raw XML validity separate from complete scoped paths and production readiness. */
    //-------------------------------------------------------------------------------------------------
    private static void Require(bool condition,string message) { if (!condition) throw new InvalidDataException(message); }
}
