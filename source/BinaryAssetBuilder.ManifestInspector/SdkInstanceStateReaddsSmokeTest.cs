using System.Text;
using System.Xml;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: independently prove ordered source-local state command pairs against actual core output and final schema binding.
internal static class SdkInstanceStateReaddsSmokeTest
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: pin old-only field loss, new position, intervening entries, base authority and atomic refusals without widening earlier flags. */
    //-------------------------------------------------------------------------------------------------
    internal static void Run()
    {
        const string xsd = "<xs:schema xmlns:xs='http://www.w3.org/2001/XMLSchema' xmlns='uri:ea.com:eala:asset' targetNamespace='uri:ea.com:eala:asset' elementFormDefault='qualified'><xs:simpleType name='FileReference'><xs:restriction base='xs:anyURI'/></xs:simpleType><xs:simpleType name='Ref'><xs:restriction base='xs:string'><xs:enumeration value='attack'/><xs:enumeration value='other'/></xs:restriction></xs:simpleType><xs:complexType name='AIStrategicState'><xs:attribute name='id' type='xs:string'/><xs:attribute name='State' type='Ref' use='required'/><xs:attribute name='Difficulty' type='xs:string'/></xs:complexType><xs:complexType name='BaseInheritableAsset'><xs:attribute name='id' type='xs:string'/><xs:attribute name='inheritFrom' type='xs:string'/></xs:complexType><xs:complexType name='AIPersonalityDefinition'><xs:complexContent><xs:extension base='BaseInheritableAsset'><xs:sequence><xs:element name='StrategicState' type='AIStrategicState' minOccurs='0' maxOccurs='unbounded'/><xs:element name='BuildState' type='AIStrategicState' minOccurs='0' maxOccurs='unbounded'/></xs:sequence></xs:extension></xs:complexContent></xs:complexType><xs:element name='AssetDeclaration'><xs:complexType><xs:sequence><xs:element name='Includes' minOccurs='0'><xs:complexType><xs:sequence><xs:element name='Include' minOccurs='0' maxOccurs='unbounded'><xs:complexType><xs:attribute name='type' type='xs:string'/><xs:attribute name='source' type='xs:string'/></xs:complexType></xs:element></xs:sequence></xs:complexType></xs:element><xs:element name='AIPersonalityDefinition' type='AIPersonalityDefinition' maxOccurs='unbounded'/></xs:sequence></xs:complexType></xs:element></xs:schema>";
        var schemas = SdkEffectiveSchema.Compile(new Dictionary<string,byte[]> { ["entry.xsd"] = Encoding.UTF8.GetBytes(xsd) },"entry.xsd").Schemas;
        string root = Path.Combine(Path.GetTempPath(),"Reborn-StateReadds-"+Guid.NewGuid().ToString("N")),data = Path.Combine(root,"Data"),entry = Path.Combine(data,"Entry.xml"),child = Path.Combine(data,"Child.xml"),output = Path.Combine(root,"NoOutput"); Directory.CreateDirectory(data);
        const string oldState = "<StrategicState id='same' State='attack' Difficulty='HARD BRUTAL'/>";
        const string other = "<StrategicState id='other' State='other'/>";
        const string remove = "<StrategicState id='same' i:joinAction='Remove'/>";
        const string replacement = "<StrategicState id='same' State='attack'/>";
        const string intervening = "<StrategicState id='new' State='other'/>";
        string basis = "<AIPersonalityDefinition id='Base'>"+oldState+other+"</AIPersonalityDefinition>";
        string owner = "<AIPersonalityDefinition id='Owner' inheritFrom='Base'>"+remove+intervening+replacement+"</AIPersonalityDefinition>";
        var paths = Fixture(basis+owner); byte[] raw = File.ReadAllBytes(entry);
        var result = new SdkInstanceInheritanceProfile(schemas,paths,stateReadds:true).Apply(entry,raw);
        Require(result.Bytes != null && result.Evidence.Profile == SdkInstanceInheritanceProfile.StateReaddName && result.Evidence.StateReadds.Single() is { OwnerId:"Owner",BaseId:"Base",ChildId:"same",BeforePosition:0,AfterPosition:2 },"State command pair/evidence failed: "+string.Join("; ",result.Evidence.Diagnostics));
        var witness = result.Evidence.StateReadds.Single();
        Require(witness.Before.Contains("Difficulty=HARD BRUTAL") && !witness.After.Any(field => field.StartsWith("Difficulty=",StringComparison.Ordinal)) && result.Evidence.Removals.Length == 1,"Removed base-only field survived or removal witness was lost.");
        XmlDocument xml = new() { XmlResolver = null }; xml.LoadXml(Encoding.UTF8.GetString(result.Bytes!));
        var actual = xml.DocumentElement!.ChildNodes.OfType<XmlElement>().Single(asset => asset.GetAttribute("id") == "Owner").ChildNodes.OfType<XmlElement>().ToArray();
        Require(actual.Select(node => node.GetAttribute("id")).SequenceEqual(new[] { "other","new","same" }) && !actual[2].HasAttribute("Difficulty"),"Re-add order or field presence differs from literal prediction.");
        Require(new SdkInstanceInheritanceProfile(schemas,paths,identicalStates:true).Apply(entry,raw).Bytes == null,"Earlier identical-state profile acquired ordered command admission.");
        var graph = SdkTypedSourceGraph.BindGraph(schemas,paths,instanceStateReadds:true);
        Require(graph.ScopedGraphComplete && !graph.ProductionBuildReady && !graph.FullDependencyCoverage && File.ReadAllBytes(entry).SequenceEqual(raw) && !Directory.Exists(output),"Final state command/schema/source/output scope changed.");
        foreach (string bad in new[] {
            replacement+remove,
            remove+replacement+replacement,
            remove.Replace("StrategicState","BuildState",StringComparison.Ordinal)+replacement,
            remove.Replace("i:joinAction='Remove'","i:joinAction='Remove' State='attack'",StringComparison.Ordinal)+replacement,
            remove+replacement.Replace("id='same'","id='same' TypeId='1'",StringComparison.Ordinal),
            remove+replacement.Replace("id='same'","id='same' i:joinAction='Replace'",StringComparison.Ordinal),
            remove+replacement.Replace("/>","><StrategicState/></StrategicState>",StringComparison.Ordinal),
            remove+replacement.Replace("State='attack'","State='=$VALUE'",StringComparison.Ordinal) })
            Reject(Fixture(basis+"<AIPersonalityDefinition id='Owner' inheritFrom='Base'>"+bad+"</AIPersonalityDefinition>"));
        Reject(Fixture("<AIPersonalityDefinition id='Base'>"+other+"</AIPersonalityDefinition>"+owner));
        Reject(Fixture(basis+owner.Replace(" inheritFrom='Base'","",StringComparison.Ordinal)));
        Reject(Fixture(basis+owner.Replace("id='Owner'","id='Owner' TypeId='late'",StringComparison.Ordinal)));
        Reject(Fixture(basis.Replace("StrategicState id='same'","BuildState id='same'",StringComparison.Ordinal)+owner));
        // Reborn: directly imported resolved bases retain defining-source authority and do not replay child events as parent-local evidence.
        File.WriteAllBytes(child,Source(basis));
        var importedPaths = Fixture("<Includes><Include type='instance' source='DATA:Child.xml'/></Includes>"+owner);
        var imported = new SdkInstanceInheritanceProfile(schemas,importedPaths,stateReadds:true).Apply(entry,File.ReadAllBytes(entry));
        Require(imported.Bytes != null && imported.Evidence.ImportedBases.Length == 1 && imported.Evidence.PreparedSources.Length == 2 && imported.Evidence.StateReadds.Length == 1,"Imported base command proof or source closure failed.");
        File.WriteAllBytes(child,Source(basis+"<!--stale-->")); Reject(importedPaths);
        var invalidPaths = Fixture(basis+owner.Replace("State='attack'","State='invalid'",StringComparison.Ordinal));
        var invalid = SdkTypedSourceGraph.BindGraph(schemas,invalidPaths,instanceStateReadds:true).Documents.Single();
        Require(invalid.Status == "SchemaInvalid" && invalid.Dependencies.Length == 0,"Ordered commands bypassed final reference scalar validation.");
        bool conflict = false; try { SdkTypedSourceGraph.BindGraph(schemas,invalidPaths,instanceStateReadds:true,instanceIdenticalStates:true); } catch (ArgumentException) { conflict = true; } Require(conflict,"Conflicting re-add/identical profiles admitted.");
        Console.WriteLine("SDK state re-adds self-test: OK (resolved targets, old-only field loss, first/new position, intervening entries, original commands, imported provenance, reverse/third/cross-QName/directive/payload/expression/missing/stale/late atomic refusals and final no-fields binding)");

        //-------------------------------------------------------------------------------------------------
        /** Reborn: capture owned explicit-root command fixtures without changing reference source files. */
        //-------------------------------------------------------------------------------------------------
        SdkSourcePathAudit.Report Fixture(string body)
        {
            File.WriteAllBytes(entry,Source(body));
            return SdkSourcePathAudit.Inspect(SdkEnvironmentPreflight.Inspect("ra3ep1",SdkEnvironmentPreflight.BaselineSchemaRoot(),data,entry,output,Array.Empty<string>()));
        }

        //-------------------------------------------------------------------------------------------------
        /** Reborn: withhold every command/source witness on preparation refusal, even after earlier valid operations. */
        //-------------------------------------------------------------------------------------------------
        void Reject(SdkSourcePathAudit.Report captured)
        {
            var rejected = new SdkInstanceInheritanceProfile(schemas,captured,stateReadds:true).Apply(entry,File.ReadAllBytes(entry));
            Require(rejected.Bytes == null && rejected.Evidence.ProcessedSha256 == null && rejected.Evidence.StateReadds.Length == 0 && rejected.Evidence.Removals.Length == 0 && rejected.Evidence.Overlays.Length == 0 && rejected.Evidence.PreparedSources.Length == 0,"Rejected state command owner leaked partial evidence.");
        }
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: encode literal owned state commands with the explicit instance directive namespace. */
    //-------------------------------------------------------------------------------------------------
    private static byte[] Source(string body) => Encoding.UTF8.GetBytes("<AssetDeclaration xmlns='uri:ea.com:eala:asset' xmlns:i='uri:ea.com:eala:asset:instance'>"+body+"</AssetDeclaration>");

    //-------------------------------------------------------------------------------------------------
    /** Reborn: stop before trusting changed command order or source authority. */
    //-------------------------------------------------------------------------------------------------
    private static void Require(bool condition,string message) { if (!condition) throw new InvalidDataException(message); }
}
