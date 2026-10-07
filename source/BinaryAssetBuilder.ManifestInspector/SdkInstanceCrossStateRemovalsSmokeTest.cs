using System.Text;
using System.Xml;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: independently prove empty cross-QName state removal without opening replacement, missing targets or mixed operations.
internal static class SdkInstanceCrossStateRemovalsSmokeTest
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: pin schema reference types, actual core field/order projection, imported provenance and atomic scope refusals. */
    //-------------------------------------------------------------------------------------------------
    internal static void Run()
    {
        const string xsd = "<xs:schema xmlns:xs='http://www.w3.org/2001/XMLSchema' xmlns:xas='uri:ea.com:eala:asset:schema' xmlns='uri:ea.com:eala:asset' targetNamespace='uri:ea.com:eala:asset' elementFormDefault='qualified'><xs:simpleType name='FileReference'><xs:restriction base='xs:anyURI'/></xs:simpleType><xs:simpleType name='AssetReference'><xs:restriction base='xs:string'><xs:enumeration value='attack'/><xs:enumeration value='other'/></xs:restriction></xs:simpleType><xs:complexType name='AIStrategicState'><xs:attribute name='id' type='xs:string'/><xs:attribute name='State' type='AssetReference' use='required' xas:refType='AIStrategicStateDefinition'/><xs:attribute name='Difficulty' type='xs:string'/></xs:complexType><xs:complexType name='AIBuildState'><xs:attribute name='id' type='xs:string'/><xs:attribute name='State' type='AssetReference' use='required' xas:refType='AIBuildStateDefinition'/><xs:attribute name='Difficulty' type='xs:string'/></xs:complexType><xs:complexType name='BaseInheritableAsset'><xs:attribute name='id' type='xs:string'/><xs:attribute name='inheritFrom' type='xs:string'/></xs:complexType><xs:complexType name='AIPersonalityDefinition'><xs:complexContent><xs:extension base='BaseInheritableAsset'><xs:sequence><xs:element name='StrategicState' type='AIStrategicState' minOccurs='0' maxOccurs='unbounded'/><xs:element name='BuildState' type='AIBuildState' minOccurs='0' maxOccurs='unbounded'/></xs:sequence></xs:extension></xs:complexContent></xs:complexType><xs:element name='AssetDeclaration'><xs:complexType><xs:sequence><xs:element name='Includes' minOccurs='0'><xs:complexType><xs:sequence><xs:element name='Include' minOccurs='0' maxOccurs='unbounded'><xs:complexType><xs:attribute name='type' type='xs:string'/><xs:attribute name='source' type='xs:string'/></xs:complexType></xs:element></xs:sequence></xs:complexType></xs:element><xs:element name='AIPersonalityDefinition' type='AIPersonalityDefinition' maxOccurs='unbounded'/></xs:sequence></xs:complexType></xs:element></xs:schema>";
        var schemas = SdkEffectiveSchema.Compile(new Dictionary<string,byte[]> { ["entry.xsd"] = Encoding.UTF8.GetBytes(xsd) },"entry.xsd").Schemas;
        string root = Path.Combine(Path.GetTempPath(),"Reborn-CrossStateRemoval-"+Guid.NewGuid().ToString("N")),data = Path.Combine(root,"Data"),entry = Path.Combine(data,"Entry.xml"),child = Path.Combine(data,"Child.xml"),output = Path.Combine(root,"NoOutput"); Directory.CreateDirectory(data);
        const string states = "<StrategicState id='same' State='attack' Difficulty='HARD'/><StrategicState id='other' State='other'/><BuildState id='build' State='other'/>";
        const string remove = "<BuildState id='same' i:joinAction='Remove'/>";
        const string add = "<StrategicState id='new' State='other'/>";
        string basis = "<AIPersonalityDefinition id='Base'>"+states+"</AIPersonalityDefinition>";
        string owner = "<AIPersonalityDefinition id='Owner' inheritFrom='Base'>"+remove+add+"</AIPersonalityDefinition>";
        var paths = Fixture(basis+owner); byte[] raw = File.ReadAllBytes(entry);
        var result = new SdkInstanceInheritanceProfile(schemas,paths,crossStateRemovals:true).Apply(entry,raw);
        Require(result.Bytes != null && result.Evidence.Profile == SdkInstanceInheritanceProfile.CrossRemovalName && result.Evidence.CrossStateRemovals.Single() is { CommandName:"BuildState",TargetName:"StrategicState",ChildId:"same",StateReference:"attack",CommandReferenceType:"AIBuildStateDefinition",TargetReferenceType:"AIStrategicStateDefinition" },"Cross-state proof failed: "+string.Join("; ",result.Evidence.Diagnostics));
        XmlDocument xml = new() { XmlResolver = null }; xml.LoadXml(Encoding.UTF8.GetString(result.Bytes!));
        var actual = xml.DocumentElement!.ChildNodes.OfType<XmlElement>().Single(asset => asset.GetAttribute("id") == "Owner").ChildNodes.OfType<XmlElement>().ToArray();
        Require(actual.Select(node => node.GetAttribute("id")).SequenceEqual(new[] { "other","new","build" }) && actual[2].LocalName == "BuildState" && result.Evidence.Removals.Single().ChildName == "BuildState","Target deletion, branch count/order or original command evidence changed.");
        Require(new SdkInstanceInheritanceProfile(schemas,paths,stateReadds:true).Apply(entry,raw).Bytes == null,"Earlier ordered-state profile widened.");
        Require(SdkTypedSourceGraph.BindGraph(schemas,paths,instanceCrossStateRemovals:true).ScopedGraphComplete && File.ReadAllBytes(entry).SequenceEqual(raw) && !Directory.Exists(output),"Final cross-state schema/source/output scope changed.");
        foreach (string bad in new[] {
            "<BuildState id='same' State='attack'/>",
            remove.Replace("Remove","Replace",StringComparison.Ordinal),
            remove.Replace("id='same'","id='same' State='attack'",StringComparison.Ordinal),
            remove.Replace("/>","><!--payload--></BuildState>",StringComparison.Ordinal),
            remove.Replace("id='same'","id='missing'",StringComparison.Ordinal),
            "<StrategicState id='build' i:joinAction='Remove'/>",
            remove+"<BuildState id='other' i:joinAction='Remove'/>",
            remove+"<StrategicState id='other' i:joinAction='Remove'/><StrategicState id='other' State='other'/>" })
            Reject(Fixture(basis+"<AIPersonalityDefinition id='Owner' inheritFrom='Base'>"+bad+"</AIPersonalityDefinition>"));
        Reject(Fixture(basis+owner.Replace("id='Owner'","id='Owner' TypeId='late'",StringComparison.Ordinal)));
        Reject(Fixture(basis.Replace("id='other'","id='same'",StringComparison.Ordinal)+owner));
        // Reborn: reference annotation drift must not be treated as an equivalent empty-type schema.
        var wrongSchemas = SdkEffectiveSchema.Compile(new Dictionary<string,byte[]> { ["entry.xsd"] = Encoding.UTF8.GetBytes(xsd.Replace("xas:refType='AIBuildStateDefinition'","xas:refType='WrongStateDefinition'",StringComparison.Ordinal)) },"entry.xsd").Schemas;
        paths = Fixture(basis+owner);
        Require(SdkSelfAttributeInheritance.Apply(wrongSchemas,File.ReadAllBytes(entry),crossStateRemovals:true).Bytes == null,"Reference-type schema drift bypassed the gate.");
        File.WriteAllBytes(child,Source(basis));
        var importedPaths = Fixture("<Includes><Include type='instance' source='DATA:Child.xml'/></Includes>"+owner);
        var imported = new SdkInstanceInheritanceProfile(schemas,importedPaths,crossStateRemovals:true).Apply(entry,File.ReadAllBytes(entry));
        Require(imported.Bytes != null && imported.Evidence.ImportedBases.Length == 1 && imported.Evidence.PreparedSources.Length == 2 && imported.Evidence.CrossStateRemovals.Length == 1,"Imported resolved target/source evidence failed.");
        File.WriteAllBytes(child,Source(basis+"<!--stale-->")); Reject(importedPaths);
        var invalidPaths = Fixture(basis+owner.Replace("id='new' State='other'","id='new' State='invalid'",StringComparison.Ordinal));
        var invalid = SdkTypedSourceGraph.BindGraph(schemas,invalidPaths,instanceCrossStateRemovals:true).Documents.Single();
        Require(invalid.Status == "SchemaInvalid" && invalid.Dependencies.Length == 0,"Cross-state removal bypassed final scalar validation.");
        bool conflict = false; try { SdkTypedSourceGraph.BindGraph(schemas,invalidPaths,instanceCrossStateRemovals:true,instanceStateReadds:true); } catch (ArgumentException) { conflict = true; } Require(conflict,"Conflicting profiles admitted.");
        Console.WriteLine("SDK cross-state removals self-test: OK (exact reference types, empty unique resolved target, actual core field/order projection, imported closure, earlier isolation, replacement/missing/ambiguous/reverse/multiple/mixed/directive/payload/schema-drift/stale/late atomic refusals and final no-fields binding)");

        //-------------------------------------------------------------------------------------------------
        /** Reborn: capture owned source fixtures under an explicit bounded data root. */
        //-------------------------------------------------------------------------------------------------
        SdkSourcePathAudit.Report Fixture(string body)
        {
            File.WriteAllBytes(entry,Source(body));
            return SdkSourcePathAudit.Inspect(SdkEnvironmentPreflight.Inspect("ra3ep1",SdkEnvironmentPreflight.BaselineSchemaRoot(),data,entry,output,Array.Empty<string>()));
        }

        //-------------------------------------------------------------------------------------------------
        /** Reborn: withhold all source/operation authority on any failed cross-removal preparation. */
        //-------------------------------------------------------------------------------------------------
        void Reject(SdkSourcePathAudit.Report captured)
        {
            var rejected = new SdkInstanceInheritanceProfile(schemas,captured,crossStateRemovals:true).Apply(entry,File.ReadAllBytes(entry));
            Require(rejected.Bytes == null && rejected.Evidence.ProcessedSha256 == null && rejected.Evidence.CrossStateRemovals.Length == 0 && rejected.Evidence.Removals.Length == 0 && rejected.Evidence.Overlays.Length == 0 && rejected.Evidence.PreparedSources.Length == 0,"Rejected cross-state owner leaked partial authority.");
        }
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: encode owned empty command fixtures without modifying reference XML or schema files. */
    //-------------------------------------------------------------------------------------------------
    private static byte[] Source(string body) => Encoding.UTF8.GetBytes("<AssetDeclaration xmlns='uri:ea.com:eala:asset' xmlns:i='uri:ea.com:eala:asset:instance'>"+body+"</AssetDeclaration>");

    //-------------------------------------------------------------------------------------------------
    /** Reborn: stop rather than trusting a changed removal target, schema type or actual core projection. */
    //-------------------------------------------------------------------------------------------------
    private static void Require(bool condition,string message) { if (!condition) throw new InvalidDataException(message); }
}
