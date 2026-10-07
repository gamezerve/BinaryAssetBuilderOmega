using System.Text;
using System.Xml;
using BinaryAssetBuilder.Core;
using BinaryAssetBuilder.Core.SageXml;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: reproduce core marker consumption without allowing imported non-inheritable types or arbitrary schema mismatches.
internal static class SdkInstanceMarkersSmokeTest
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: pin core declaration consumption, reviewed local AI exceptions, owner-only witnesses and final binding/source/import isolation. */
    //-------------------------------------------------------------------------------------------------
    internal static void Run()
    {
        const string xsd = "<xs:schema xmlns:xs='http://www.w3.org/2001/XMLSchema' xmlns='uri:ea.com:eala:asset' targetNamespace='uri:ea.com:eala:asset' elementFormDefault='qualified'><xs:simpleType name='FileReference'><xs:restriction base='xs:anyURI'/></xs:simpleType><xs:complexType name='BaseAssetType'><xs:attribute name='id' type='xs:string' use='required'/></xs:complexType><xs:complexType name='BaseInheritableAsset'><xs:complexContent><xs:extension base='BaseAssetType'><xs:attribute name='inheritFrom' type='xs:string'/></xs:extension></xs:complexContent></xs:complexType><xs:complexType name='AITargetingHeuristic'><xs:complexContent><xs:extension base='BaseAssetType'><xs:attribute name='Value' type='xs:int'/><xs:attribute name='Filename' type='FileReference'/></xs:extension></xs:complexContent></xs:complexType><xs:complexType name='AIMicroManagerData'><xs:complexContent><xs:extension base='BaseAssetType'><xs:attribute name='Value' type='xs:int'/></xs:extension></xs:complexContent></xs:complexType><xs:complexType name='Other'><xs:complexContent><xs:extension base='BaseAssetType'><xs:attribute name='Value' type='xs:int'/></xs:extension></xs:complexContent></xs:complexType><xs:complexType name='Asset'><xs:complexContent><xs:extension base='BaseInheritableAsset'><xs:attribute name='Value' type='xs:int'/></xs:extension></xs:complexContent></xs:complexType><xs:element name='AssetDeclaration'><xs:complexType><xs:sequence><xs:element name='Includes' minOccurs='0'><xs:complexType><xs:sequence><xs:element name='Include' minOccurs='0' maxOccurs='unbounded'><xs:complexType><xs:attribute name='type' type='xs:string'/><xs:attribute name='source' type='xs:string'/></xs:complexType></xs:element></xs:sequence></xs:complexType></xs:element><xs:choice minOccurs='0' maxOccurs='unbounded'><xs:element name='AITargetingHeuristic' type='AITargetingHeuristic'/><xs:element name='AIMicroManagerData' type='AIMicroManagerData'/><xs:element name='Other' type='Other'/><xs:element name='Asset' type='Asset'/></xs:choice></xs:sequence></xs:complexType></xs:element></xs:schema>";
        var schemas = SdkEffectiveSchema.Compile(new Dictionary<string,byte[]> { ["entry.xsd"] = Encoding.UTF8.GetBytes(xsd) },"entry.xsd").Schemas;
        string root = Path.Combine(Path.GetTempPath(),"Reborn-InstanceMarkers-"+Guid.NewGuid().ToString("N")),data = Path.Combine(root,"Data"),entry = Path.Combine(data,"Entry.xml"),output = Path.Combine(root,"NoOutput"); Directory.CreateDirectory(data);
        File.WriteAllBytes(Path.Combine(data,"payload.bin"),new byte[] { 9 });
        string local = "<AITargetingHeuristic id='Base' Value='1' Filename='DATA:payload.bin'/><AITargetingHeuristic id='Middle' inheritFrom='AITargetingHeuristic:Base' Value='2'/><AITargetingHeuristic id='Owner' inheritFrom='Middle'/><AIMicroManagerData id='MicroBase' Value='7'/><AIMicroManagerData id='MicroOwner' inheritFrom='MicroBase'/>";
        byte[] raw = Source(local); var paths = Fixture(local,"");
        // Reborn: exercise the real core setter, not a presumed NodeJoiner behavior: declaration loading removes the pipeline marker before merging.
        var coreXml = Parse(Source("<AITargetingHeuristic id='Base' Value='1'/><AITargetingHeuristic id='Owner' inheritFrom='Base'/>"));
        var coreAssets = coreXml.DocumentElement!.ChildNodes.OfType<XmlElement>().ToArray();
        InstanceDeclaration core = new(new AssetDeclarationDocument()) { XmlNode = coreAssets[1] };
        Require(!coreAssets[1].HasAttribute("inheritFrom") && core.InheritFromHandle.TypeName == "AITargetingHeuristic" && core.InheritFromHandle.InstanceName == "Base","Core marker consumption/handle capture changed.");
        var mergedCore = NodeJoiner.Override(schemas,coreXml,coreAssets[0],core.XmlNode);
        Require(((XmlElement)mergedCore).GetAttribute("Value") == "1" && !((XmlElement)mergedCore).HasAttribute("inheritFrom"),"Core post-consumption joining retained a marker or lost base attributes.");
        var result = new SdkInstanceInheritanceProfile(schemas,paths,markers:true).Apply(entry,raw);
        Require(result.Bytes != null && result.Evidence.Profile == SdkInstanceInheritanceProfile.MarkerName && result.Evidence.ConsumedMarkers.Length == 3 && result.Evidence.ConsumedMarkers.All(marker => !marker.SchemaDeclared) && result.Evidence.PreparedSources.Length == 1,"Reviewed marker/closure evidence failed.");
        var xml = Parse(result.Bytes!); var assets = xml.DocumentElement!.ChildNodes.OfType<XmlElement>().ToArray();
        Require(xml.SelectNodes("//@inheritFrom")!.Count == 0 && assets.Single(asset => asset.GetAttribute("id") == "Owner").GetAttribute("Value") == "2" && assets.Single(asset => asset.GetAttribute("id") == "MicroOwner").GetAttribute("Value") == "7" && result.Evidence.ConsumedMarkers.Single(marker => marker.DerivedId == "Middle").BaseId == "Base","Consumed markers, qualified target identity or local chain attributes differ.");
        Require(new SdkInstanceInheritanceProfile(schemas,paths,choices:true).Apply(entry,raw).Bytes == null,"Older choice-only profile admitted undeclared AI markers.");
        var graph = SdkTypedSourceGraph.BindGraph(schemas,paths,instanceMarkers:true);
        Require(graph.ScopedGraphComplete && graph.PreprocessingProfile == SdkInstanceInheritanceProfile.MarkerName && graph.Documents.Single().Dependencies.Length == 3 && !graph.ProductionBuildReady && !graph.FullDependencyCoverage && File.ReadAllBytes(entry).SequenceEqual(raw) && !Directory.Exists(output),"Marker field/source/output isolation failed.");
        // Reborn: declared imported inheritance still works, but only the owner's command appears in consumption evidence.
        paths = Fixture(Include()+"<Asset id='Owner' inheritFrom='Middle'/>","<Asset id='Base' Value='3'/><Asset id='Middle' inheritFrom='Base'/>");
        result = new SdkInstanceInheritanceProfile(schemas,paths,markers:true).Apply(entry,File.ReadAllBytes(entry));
        Require(result.Bytes != null && result.Evidence.ConsumedMarkers.Length == 1 && result.Evidence.ConsumedMarkers.Single().SchemaDeclared && result.Evidence.ConsumedMarkers.Single().DerivedId == "Owner" && result.Evidence.PreparedSources.Length == 2,"Imported declared marker consumption replayed a child or lost source closure.");
        foreach (string type in new[] { "AITargetingHeuristic","AIMicroManagerData" })
        {
            paths = Fixture(Include()+"<"+type+" id='Owner' inheritFrom='Base'/>","<"+type+" id='Base' Value='1'/>");
            var refused = Reject(paths);
            Require(refused.Evidence.Diagnostics.Any(message => message.Contains("not derived from BaseInheritableAsset",StringComparison.Ordinal)),"Non-inheritable imported base refused for an unexpected reason.");
        }
        foreach (string bad in new[] {
            "<Other id='Base' Value='1'/><Other id='Owner' inheritFrom='Base'/>",
            "<AITargetingHeuristic id='Base' Value='1'/><AITargetingHeuristic id='Owner' inheritFrom='Base' Unknown='1'/>",
            "<AITargetingHeuristic id='Base' Value='1'/><AITargetingHeuristic id='Owner' i:inheritFrom='Base'/>",
            "<AITargetingHeuristic id='Base' Value='1'/><AITargetingHeuristic id='Owner' inheritFrom='=BASE'/>",
            "<AITargetingHeuristic id='Owner' inheritFrom='Missing'/>","<AITargetingHeuristic id='Owner' inheritFrom='Owner'/>",
            "<AITargetingHeuristic id='Base' inheritFrom='Owner'/><AITargetingHeuristic id='Owner' inheritFrom='Base'/>",
            "<AIMicroManagerData id='Base' Value='1'/><AITargetingHeuristic id='Owner' inheritFrom='AIMicroManagerData:Base'/>" }) Reject(Fixture(bad,""));
        // Reborn: marker consumption cannot make an invalid native payload trustworthy; final schema binding still withholds dependency fields.
        paths = Fixture("<AITargetingHeuristic id='Base' Value='bad' Filename='DATA:payload.bin'/><AITargetingHeuristic id='Owner' inheritFrom='Base'/>","");
        var invalid = SdkTypedSourceGraph.BindGraph(schemas,paths,instanceMarkers:true).Documents.Single();
        Require(invalid.Status == "SchemaInvalid" && invalid.Dependencies.Length == 0,"Consumed marker bypassed final payload validation.");
        bool conflict = false; try { SdkTypedSourceGraph.BindGraph(schemas,paths,instanceMarkers:true,instanceChoices:true); } catch (ArgumentException) { conflict = true; } Require(conflict,"Conflicting marker/choice flags were admitted.");
        Console.WriteLine("SDK instance markers self-test: OK (actual core setter consumption, two reviewed same-document AI types, qualified/local chain identities, consumed declared/imported owner-only witnesses, root fields, final no-fields payload failure, atomic unknown/wrong-namespace/expression/missing/same-handle/cyclic/cross-type/imported non-inheritable refusal and older-profile isolation)");

        //-------------------------------------------------------------------------------------------------
        /** Reborn: author and snapshot only owned marker fixtures without modifying official XML or XSD. */
        //-------------------------------------------------------------------------------------------------
        SdkSourcePathAudit.Report Fixture(string body,string baseBody)
        {
            File.WriteAllBytes(entry,Source(body)); File.WriteAllBytes(Path.Combine(data,"Base.xml"),Source(baseBody));
            return SdkSourcePathAudit.Inspect(SdkEnvironmentPreflight.Inspect("ra3ep1",SdkEnvironmentPreflight.BaselineSchemaRoot(),data,entry,output,Array.Empty<string>()));
        }

        //-------------------------------------------------------------------------------------------------
        /** Reborn: refuse all transformed XML and consumption/import/source witnesses together on any unsupported marker scope. */
        //-------------------------------------------------------------------------------------------------
        SdkSelfAttributeInheritance.Result Reject(SdkSourcePathAudit.Report captured)
        {
            var refused = new SdkInstanceInheritanceProfile(schemas,captured,markers:true).Apply(entry,File.ReadAllBytes(entry));
            Require(refused.Bytes == null && refused.Evidence.ProcessedSha256 == null && refused.Evidence.Overlays.Length == 0 && refused.Evidence.ConsumedMarkers.Length == 0 && refused.Evidence.PreparedSources.Length == 0 && refused.Evidence.ImportedBases.Length == 0 && refused.Evidence.Removals.Length == 0,"Marker refusal leaked partial evidence."); return refused;
        }
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: restrict imported fixture visibility to a literal direct instance Include. */
    //-------------------------------------------------------------------------------------------------
    private static string Include() => "<Includes><Include type='instance' source='Base.xml'/></Includes>";

    //-------------------------------------------------------------------------------------------------
    /** Reborn: encode owned source-marker fixtures with the explicit instance namespace for refusal tests. */
    //-------------------------------------------------------------------------------------------------
    private static byte[] Source(string body) => Encoding.UTF8.GetBytes("<AssetDeclaration xmlns='uri:ea.com:eala:asset' xmlns:i='uri:ea.com:eala:asset:instance'>"+body+"</AssetDeclaration>");

    //-------------------------------------------------------------------------------------------------
    /** Reborn: inspect only owned in-memory XML without external resolution. */
    //-------------------------------------------------------------------------------------------------
    private static XmlDocument Parse(byte[] bytes) { XmlDocument xml = new() { XmlResolver = null }; xml.LoadXml(Encoding.UTF8.GetString(bytes)); return xml; }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: stop on changed declaration/loading semantics without asserting production or game compatibility. */
    //-------------------------------------------------------------------------------------------------
    private static void Require(bool condition,string message) { if (!condition) throw new InvalidDataException(message); }
}
