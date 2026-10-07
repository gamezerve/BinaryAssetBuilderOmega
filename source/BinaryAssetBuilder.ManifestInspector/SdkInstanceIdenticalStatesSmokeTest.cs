using System.Text;
using System.Xml;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: independently prove identical state folding, source-local witnesses, unchanged earlier refusals and final schema binding.
internal static class SdkInstanceIdenticalStatesSmokeTest
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: test bounded same-QName literal equality and actual core agreement without admitting conflicts, directives or command pairs. */
    //-------------------------------------------------------------------------------------------------
    internal static void Run()
    {
        const string xsd = "<xs:schema xmlns:xs='http://www.w3.org/2001/XMLSchema' xmlns='uri:ea.com:eala:asset' targetNamespace='uri:ea.com:eala:asset' elementFormDefault='qualified'><xs:simpleType name='FileReference'><xs:restriction base='xs:anyURI'/></xs:simpleType><xs:simpleType name='Ref'><xs:restriction base='xs:string'><xs:enumeration value='attack'/><xs:enumeration value='other'/></xs:restriction></xs:simpleType><xs:complexType name='AIStrategicState'><xs:attribute name='id' type='xs:string'/><xs:attribute name='State' type='Ref' use='required'/><xs:attribute name='Difficulty' type='xs:string'/></xs:complexType><xs:complexType name='BaseInheritableAsset'><xs:attribute name='id' type='xs:string'/><xs:attribute name='inheritFrom' type='xs:string'/></xs:complexType><xs:complexType name='AIPersonalityDefinition'><xs:complexContent><xs:extension base='BaseInheritableAsset'><xs:sequence><xs:element name='StrategicState' type='AIStrategicState' minOccurs='0' maxOccurs='unbounded'/><xs:element name='BuildState' type='AIStrategicState' minOccurs='0' maxOccurs='unbounded'/></xs:sequence></xs:extension></xs:complexContent></xs:complexType><xs:element name='AssetDeclaration'><xs:complexType><xs:sequence><xs:element name='Includes' minOccurs='0'><xs:complexType><xs:sequence><xs:element name='Include' minOccurs='0' maxOccurs='unbounded'><xs:complexType><xs:attribute name='type' type='xs:string'/><xs:attribute name='source' type='xs:string'/></xs:complexType></xs:element></xs:sequence></xs:complexType></xs:element><xs:element name='AIPersonalityDefinition' type='AIPersonalityDefinition' maxOccurs='unbounded'/></xs:sequence></xs:complexType></xs:element></xs:schema>";
        var schemas = SdkEffectiveSchema.Compile(new Dictionary<string,byte[]> { ["entry.xsd"] = Encoding.UTF8.GetBytes(xsd) },"entry.xsd").Schemas;
        string root = Path.Combine(Path.GetTempPath(),"Reborn-IdenticalStates-"+Guid.NewGuid().ToString("N")),data = Path.Combine(root,"Data"),entry = Path.Combine(data,"Entry.xml"),child = Path.Combine(data,"Child.xml"),output = Path.Combine(root,"NoOutput"); Directory.CreateDirectory(data);
        const string state = "<StrategicState id='same' State='attack' Difficulty='HARD BRUTAL'/>";
        const string other = "<StrategicState id='other' State='other'/>";
        const string equivalent = "<StrategicState Difficulty='HARD BRUTAL' State='attack' id='same'/>";
        string body = "<AIPersonalityDefinition id='Base'/><AIPersonalityDefinition id='Owner' inheritFrom='Base'>"+state+other+equivalent+"</AIPersonalityDefinition>";
        var paths = Fixture(body); byte[] raw = File.ReadAllBytes(entry);
        var result = new SdkInstanceInheritanceProfile(schemas,paths,identicalStates:true).Apply(entry,raw);
        Require(result.Bytes != null && result.Evidence.Profile == SdkInstanceInheritanceProfile.IdenticalStateName && result.Evidence.IdenticalStates.Single() is { OwnerId:"Owner",ChildId:"same",Before:2,After:1 },"Identical-state normalization/evidence failed: "+string.Join(";",result.Evidence.Diagnostics));
        XmlDocument xml = new() { XmlResolver = null }; xml.LoadXml(Encoding.UTF8.GetString(result.Bytes!));
        var owner = xml.DocumentElement!.ChildNodes.OfType<XmlElement>().Single(asset => asset.GetAttribute("id") == "Owner");
        Require(owner.ChildNodes.OfType<XmlElement>().Select(node => node.GetAttribute("id")).SequenceEqual(new[] { "same","other" }) && !owner.HasAttribute("inheritFrom"),"Identical folding changed first-key position or marker consumption.");
        Require(new SdkInstanceInheritanceProfile(schemas,paths,expressions:true).Apply(entry,raw).Bytes == null,"Earlier expression profile acquired identical-state admission.");
        var graph = SdkTypedSourceGraph.BindGraph(schemas,paths,instanceIdenticalStates:true);
        Require(graph.ScopedGraphComplete && !graph.ProductionBuildReady && !graph.FullDependencyCoverage && File.ReadAllBytes(entry).SequenceEqual(raw) && !Directory.Exists(output),"Final schema/source/output scope changed: "+string.Join("; ",graph.Documents.SelectMany(document => document.Diagnostics)));
        foreach (string bad in new[] {
            equivalent.Replace("State='attack'","State='other'",StringComparison.Ordinal),
            equivalent.Replace("Difficulty='HARD BRUTAL'","Difficulty='EASY'",StringComparison.Ordinal),
            equivalent.Replace("id='same'","id='same' TypeId='1'",StringComparison.Ordinal),
            equivalent.Replace("id='same'","id='same' Unknown='x'",StringComparison.Ordinal),
            equivalent.Replace("id='same'","id='same' i:joinAction='Remove'",StringComparison.Ordinal),
            equivalent.Replace("StrategicState","BuildState",StringComparison.Ordinal),
            equivalent.Replace("/>",">text</StrategicState>",StringComparison.Ordinal),
            equivalent.Replace("/>","><!--payload--></StrategicState>",StringComparison.Ordinal),
            equivalent.Replace(" State='attack'","",StringComparison.Ordinal),
            equivalent.Replace("State='attack'","State='=$VAR'",StringComparison.Ordinal) })
            Reject(Fixture("<AIPersonalityDefinition id='Base'/><AIPersonalityDefinition id='Owner' inheritFrom='Base'>"+state+bad+"</AIPersonalityDefinition>"));
        Reject(Fixture(body.Replace("id='same'","id='unsafe key'",StringComparison.Ordinal)));
        Reject(Fixture(body.Replace("id='Owner'","id='Owner' TypeId='late'",StringComparison.Ordinal)));
        Reject(Fixture("<AIPersonalityDefinition id='Base'/><AIPersonalityDefinition id='Owner' inheritFrom='Base'>"+string.Concat(Enumerable.Repeat(state,17))+"</AIPersonalityDefinition>"));
        // Reborn: a real ordered command pair is not an identical duplicate and must remain atomically refused.
        Reject(Fixture("<AIPersonalityDefinition id='Base'>"+state+"</AIPersonalityDefinition><AIPersonalityDefinition id='Owner' inheritFrom='Base'><StrategicState id='same' i:joinAction='Remove'/>"+state+"</AIPersonalityDefinition>"));
        // Reborn: imported defining documents publish their own normalization, not replayed parent-local events.
        File.WriteAllBytes(child,Source("<AIPersonalityDefinition id='Base'>"+state+equivalent+"</AIPersonalityDefinition>"));
        var importedPaths = Fixture("<Includes><Include type='instance' source='DATA:Child.xml'/></Includes><AIPersonalityDefinition id='Owner' inheritFrom='Base'>"+other+"</AIPersonalityDefinition>");
        var imported = new SdkInstanceInheritanceProfile(schemas,importedPaths,identicalStates:true).Apply(entry,File.ReadAllBytes(entry));
        Require(imported.Bytes != null && imported.Evidence.ImportedBases.Length == 1 && imported.Evidence.PreparedSources.Length == 2 && imported.Evidence.IdenticalStates.Length == 0,"Imported normalization was lost or replayed as owner-local evidence.");
        Require(SdkTypedSourceGraph.BindGraph(schemas,importedPaths,instanceIdenticalStates:true).ScopedGraphComplete,"Imported normalized base failed whole-graph validation.");
        File.WriteAllBytes(child,Source("<AIPersonalityDefinition id='Base'>"+state+equivalent+"</AIPersonalityDefinition><!--stale-->")); Reject(importedPaths);
        var invalidPaths = Fixture(body.Replace("State='attack'","State='invalid'",StringComparison.Ordinal));
        var invalid = SdkTypedSourceGraph.BindGraph(schemas,invalidPaths,instanceIdenticalStates:true).Documents.Single();
        Require(invalid.Status == "SchemaInvalid" && invalid.Dependencies.Length == 0,"Identical folding bypassed final reference scalar validation.");
        bool conflict = false; try { SdkTypedSourceGraph.BindGraph(schemas,invalidPaths,instanceIdenticalStates:true,instanceExpressions:true); } catch (ArgumentException) { conflict = true; } Require(conflict,"Conflicting profiles were admitted.");
        Console.WriteLine("SDK identical states self-test: OK (literal equality/attribute order, first-key position, core agreement, imported source-local events, conflicting/directive/command/payload/key/group/stale/late atomic refusals, earlier isolation and final no-fields binding)");

        //-------------------------------------------------------------------------------------------------
        /** Reborn: capture owned explicit-root XML without modifying any external reference corpus. */
        //-------------------------------------------------------------------------------------------------
        SdkSourcePathAudit.Report Fixture(string ownerBody)
        {
            File.WriteAllBytes(entry,Source(ownerBody));
            return SdkSourcePathAudit.Inspect(SdkEnvironmentPreflight.Inspect("ra3ep1",SdkEnvironmentPreflight.BaselineSchemaRoot(),data,entry,output,Array.Empty<string>()));
        }

        //-------------------------------------------------------------------------------------------------
        /** Reborn: withhold every normalized source/operation witness on any preparation failure, including a later owner guard. */
        //-------------------------------------------------------------------------------------------------
        void Reject(SdkSourcePathAudit.Report captured)
        {
            var rejected = new SdkInstanceInheritanceProfile(schemas,captured,identicalStates:true).Apply(entry,File.ReadAllBytes(entry));
            Require(rejected.Bytes == null && rejected.Evidence.ProcessedSha256 == null && rejected.Evidence.IdenticalStates.Length == 0 && rejected.Evidence.Overlays.Length == 0 && rejected.Evidence.PreparedSources.Length == 0 && rejected.Evidence.ExpressionPreparation == null,"Rejected identical-state owner leaked partial authority.");
        }
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: encode owned duplicate-state fixtures with explicit instance directive namespace. */
    //-------------------------------------------------------------------------------------------------
    private static byte[] Source(string body) => Encoding.UTF8.GetBytes("<AssetDeclaration xmlns='uri:ea.com:eala:asset' xmlns:i='uri:ea.com:eala:asset:instance'>"+body+"</AssetDeclaration>");

    //-------------------------------------------------------------------------------------------------
    /** Reborn: stop rather than accepting changed core folding or source-local evidence behavior. */
    //-------------------------------------------------------------------------------------------------
    private static void Require(bool condition,string message) { if (!condition) throw new InvalidDataException(message); }
}
