using System.Text;
using System.Xml;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: independently prove captured expression substitution before source-owner inheritance without widening earlier profiles.
internal static class SdkInstanceExpressionsSmokeTest
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: test actual stage order, imported/local contexts, definition collisions, identity guards, atomic late rejection and final no-fields binding. */
    //-------------------------------------------------------------------------------------------------
    internal static void Run()
    {
        const string xsd = "<xs:schema xmlns:xs='http://www.w3.org/2001/XMLSchema' xmlns='uri:ea.com:eala:asset' targetNamespace='uri:ea.com:eala:asset' elementFormDefault='qualified'><xs:simpleType name='FileReference'><xs:restriction base='xs:anyURI'/></xs:simpleType><xs:complexType name='BaseAssetType'><xs:attribute name='id' type='xs:string'/></xs:complexType><xs:complexType name='ObjectCreationList'><xs:complexContent><xs:extension base='BaseAssetType'><xs:sequence><xs:element name='CreateObject' type='xs:string' minOccurs='0' maxOccurs='unbounded'/></xs:sequence></xs:extension></xs:complexContent></xs:complexType><xs:complexType name='BaseInheritableAsset'><xs:attribute name='id' type='xs:string'/><xs:attribute name='inheritFrom' type='xs:string'/></xs:complexType><xs:complexType name='Asset'><xs:complexContent><xs:extension base='BaseInheritableAsset'><xs:attribute name='Value' type='xs:int'/><xs:attribute name='A' type='xs:int'/></xs:extension></xs:complexContent></xs:complexType><xs:element name='AssetDeclaration'><xs:complexType><xs:sequence><xs:element name='Tags' minOccurs='0'><xs:complexType/></xs:element><xs:element name='Includes' minOccurs='0'><xs:complexType><xs:sequence><xs:element name='Include' minOccurs='0' maxOccurs='unbounded'><xs:complexType><xs:attribute name='type' type='xs:string'/><xs:attribute name='source' type='xs:string'/></xs:complexType></xs:element></xs:sequence></xs:complexType></xs:element><xs:element name='Defines' minOccurs='0'><xs:complexType><xs:sequence><xs:element name='Define' minOccurs='0' maxOccurs='unbounded'><xs:complexType><xs:attribute name='name' type='xs:string'/><xs:attribute name='value' type='xs:string'/><xs:attribute name='override' type='xs:boolean'/></xs:complexType></xs:element></xs:sequence></xs:complexType></xs:element><xs:choice minOccurs='0' maxOccurs='unbounded'><xs:element name='Asset' type='Asset'/><xs:element name='ObjectCreationList' type='ObjectCreationList'/></xs:choice></xs:sequence></xs:complexType></xs:element></xs:schema>";
        var schemas = SdkEffectiveSchema.Compile(new Dictionary<string,byte[]> { ["entry.xsd"] = Encoding.UTF8.GetBytes(xsd) },"entry.xsd").Schemas;
        string root = Path.Combine(Path.GetTempPath(),"Reborn-InstanceExpressions-"+Guid.NewGuid().ToString("N")),data = Path.Combine(root,"Data"),entry = Path.Combine(data,"Entry.xml"),leaf = Path.Combine(data,"Definitions.xml"),output = Path.Combine(root,"NoOutput"); Directory.CreateDirectory(data);
        const string definitions = "<Tags/><Includes/><Defines><Define name='BASE' value='2'/><Define name='ALIAS' value='=$BASE'/></Defines>";
        const string includes = "<Includes><Include type='all' source='DATA:Definitions.xml'/></Includes>";
        string source = includes+"<Defines><Define name='OWN' value='9'/></Defines><Asset id='Base' Value='=$ALIAS'/><Asset id='Owner' inheritFrom='Base' A='=$OWN'/>";
        var paths = Fixture(source,definitions); byte[] raw = File.ReadAllBytes(entry);
        var result = new SdkInstanceInheritanceProfile(schemas,paths,expressions:true).Apply(entry,raw);
        Require(result.Bytes != null && result.Evidence.Profile == SdkInstanceInheritanceProfile.ExpressionName && result.Evidence.ExpressionPreparation?.Substitutions == 2 && result.Evidence.ExpressionPreparation.RawSha256 == result.Evidence.RawSha256 && result.Evidence.ExpressionPreparation.DefinitionSources.Length == 2 && result.Evidence.ExpressionPreparation.EvaluatedDefinitions.Length == 1,"Expression stage/context witness failed: "+string.Join(";",result.Evidence.Diagnostics));
        var xml = new XmlDocument { XmlResolver = null }; xml.LoadXml(Encoding.UTF8.GetString(result.Bytes!));
        var owner = xml.DocumentElement!.ChildNodes.OfType<XmlElement>().Single(asset => asset.LocalName == "Asset" && asset.GetAttribute("id") == "Owner");
        Require(owner.GetAttribute("Value") == "2" && owner.GetAttribute("A") == "9" && !owner.HasAttribute("inheritFrom") && result.Evidence.MetadataDefinitionIncludes.Length == 1,"Expressions were not resolved before inheritance or markers/metadata changed.");
        Require(new SdkInstanceInheritanceProfile(schemas,paths,metadata:true).Apply(entry,raw).Bytes == null && SdkLocalDefineProfile.Apply(raw).Bytes == null && new SdkIncludeDefineProfile(paths,true).Apply(entry,raw).Bytes == null,"Earlier expression/metadata profiles widened.");
        var graph = SdkTypedSourceGraph.BindGraph(schemas,paths,instanceExpressions:true);
        Require(graph.ScopedGraphComplete && !graph.ProductionBuildReady && !graph.FullDependencyCoverage && File.ReadAllBytes(entry).SequenceEqual(raw) && !Directory.Exists(output),"Final expression/schema/source/output isolation failed.");
        // Reborn: local ObjectCreationList markers are consumed after expression resolution, and anonymous repeated nuggets remain separate.
        string ocl = includes+"<Defines><Define name='OWN' value='9'/></Defines><ObjectCreationList id='Base'><CreateObject>=$BASE</CreateObject></ObjectCreationList><ObjectCreationList id='Owner' inheritFrom='Base'><CreateObject>=$OWN</CreateObject></ObjectCreationList>";
        var oclPaths = Fixture(ocl,definitions);
        var oclResult = new SdkInstanceInheritanceProfile(schemas,oclPaths,expressions:true).Apply(entry,File.ReadAllBytes(entry));
        Require(oclResult.Bytes != null && oclResult.Evidence.ConsumedMarkers.Single().SchemaDeclared == false && oclResult.Evidence.ExpressionPreparation?.Substitutions == 2,"Reviewed local OCL marker was not consumed after expressions.");
        var oclXml = new XmlDocument { XmlResolver = null }; oclXml.LoadXml(Encoding.UTF8.GetString(oclResult.Bytes!));
        Require(oclXml.DocumentElement!.ChildNodes.OfType<XmlElement>().Single(asset => asset.GetAttribute("id") == "Owner").ChildNodes.OfType<XmlElement>().Select(child => child.InnerText).SequenceEqual(new[] { "2","9" }),"Anonymous OCL nuggets were replaced or evaluated in the wrong order.");
        Require(SdkTypedSourceGraph.BindGraph(schemas,oclPaths,instanceExpressions:true).ScopedGraphComplete,"Prepared OCL marker/payload failed final schema binding.");
        var literalOclPaths = Fixture(ocl.Replace("=$BASE","unitA",StringComparison.Ordinal).Replace("=$OWN","unitB",StringComparison.Ordinal),definitions);
        Require(new SdkInstanceInheritanceProfile(schemas,literalOclPaths,metadata:true).Apply(entry,File.ReadAllBytes(entry)).Bytes == null,"Older metadata profile acquired the new local OCL marker exception.");
        File.WriteAllBytes(Path.Combine(data,"Non.xml"),Source("<ObjectCreationList id='Base'><CreateObject>unit</CreateObject></ObjectCreationList>"));
        Reject(Fixture("<Includes><Include type='instance' source='DATA:Non.xml'/></Includes><ObjectCreationList id='Owner' inheritFrom='Base'/>",definitions));
        // Reborn: defining-source values are evaluated before importing child assets; the consumer's own field uses a distinct local definition.
        File.WriteAllBytes(Path.Combine(data,"Middle.xml"),Source(includes+"<Asset id='Middle' Value='=$BASE'/>"));
        var importedPaths = Fixture("<Includes><Include type='instance' source='DATA:Middle.xml'/></Includes><Defines><Define name='OWN' value='9'/></Defines><Asset id='Owner' inheritFrom='Middle' A='=$OWN'/>",definitions);
        var imported = new SdkInstanceInheritanceProfile(schemas,importedPaths,expressions:true).Apply(entry,File.ReadAllBytes(entry));
        Require(imported.Bytes != null && imported.Evidence.PreparedSources.Length == 3 && imported.Evidence.ImportedBases.Length == 1 && imported.Evidence.ExpressionPreparation?.Substitutions == 1 && Encoding.UTF8.GetString(imported.Bytes).Contains("Value=\"2\"",StringComparison.Ordinal),"Imported defining-document expression context was lost or replayed.");
        foreach (string bad in new[] {
            source.Replace("=$OWN","=$MISSING",StringComparison.Ordinal),
            source.Replace("=$OWN","=1+2",StringComparison.Ordinal),
            source.Replace("name='OWN'","name='BASE'",StringComparison.Ordinal),
            source.Replace("id='Owner'","id='=$OWN'",StringComparison.Ordinal),
            source.Replace("inheritFrom='Base'","inheritFrom='=$OWN'",StringComparison.Ordinal),
            source.Replace("A='=$OWN'","TypeId='=$OWN'",StringComparison.Ordinal),
            source.Replace("A='=$OWN'","xmlns:i='uri:ea.com:eala:asset:instance' i:joinAction='=$OWN'",StringComparison.Ordinal) })
            Reject(Fixture(bad,definitions));
        // Reborn: successful substitution cannot publish partial evidence when a later original directive fails inheritance admission.
        Reject(Fixture(source+"<Asset id='Bad' inheritFrom='Base' TypeId='literal'/>",definitions));
        paths = Fixture(source,definitions); File.WriteAllBytes(leaf,Source(definitions+"<!--stale-->")); Reject(paths);
        var invalidPaths = Fixture(source.Replace("value='9'","value='bad'",StringComparison.Ordinal),definitions);
        var invalid = SdkTypedSourceGraph.BindGraph(schemas,invalidPaths,instanceExpressions:true).Documents.Single(document => document.SourcePath == entry);
        Require(invalid.Status == "SchemaInvalid" && invalid.Dependencies.Length == 0,"Substitution bypassed final scalar validation.");
        bool conflict = false; try { SdkTypedSourceGraph.BindGraph(schemas,invalidPaths,instanceExpressions:true,instanceMetadata:true); } catch (ArgumentException) { conflict = true; } Require(conflict,"Conflicting expression/metadata profiles admitted.");
        Console.WriteLine("SDK instance expressions self-test: OK (expressions before overlays, local/imported defining contexts, alias evaluation/source hashes, earlier-profile isolation, unknown/unsupported/colliding/identity/directive/stale/late atomic refusals and final no-fields/source/output checks)");

        //-------------------------------------------------------------------------------------------------
        /** Reborn: capture owned source/definition fixtures under a bounded explicit data root. */
        //-------------------------------------------------------------------------------------------------
        SdkSourcePathAudit.Report Fixture(string ownerBody,string definitionBody)
        {
            File.WriteAllBytes(entry,Source(ownerBody)); File.WriteAllBytes(leaf,Source(definitionBody));
            return SdkSourcePathAudit.Inspect(SdkEnvironmentPreflight.Inspect("ra3ep1",SdkEnvironmentPreflight.BaselineSchemaRoot(),data,entry,output,Array.Empty<string>()));
        }

        //-------------------------------------------------------------------------------------------------
        /** Reborn: refuse every partial expression, metadata, import and overlay witness when preparation fails. */
        //-------------------------------------------------------------------------------------------------
        void Reject(SdkSourcePathAudit.Report captured)
        {
            var rejected = new SdkInstanceInheritanceProfile(schemas,captured,expressions:true).Apply(entry,File.ReadAllBytes(entry));
            Require(rejected.Bytes == null && rejected.Evidence.ProcessedSha256 == null && rejected.Evidence.ExpressionPreparation == null && rejected.Evidence.MetadataDefinitionIncludes.Length == 0 && rejected.Evidence.PreparedSources.Length == 0 && rejected.Evidence.ImportedBases.Length == 0 && rejected.Evidence.Overlays.Length == 0 && rejected.Evidence.ConsumedMarkers.Length == 0,"Expression failure leaked partial source/operation evidence.");
        }
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: encode owned pre-inheritance fixtures without modifying reference XML/XSD. */
    //-------------------------------------------------------------------------------------------------
    private static byte[] Source(string body) => Encoding.UTF8.GetBytes("<AssetDeclaration xmlns='uri:ea.com:eala:asset'>"+body+"</AssetDeclaration>");

    //-------------------------------------------------------------------------------------------------
    /** Reborn: fail on changed stage order without claiming EA evaluator or native/game equivalence. */
    //-------------------------------------------------------------------------------------------------
    private static void Require(bool condition,string message) { if (!condition) throw new InvalidDataException(message); }
}
