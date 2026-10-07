using System.Text;
using System.Xml;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: independently prove complementary upgrade singleton normalization before widening any diagnostic source scope.
internal static class SdkInstanceUpgradesSmokeTest
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: test ordered duplicate references, complementary conditions, atomic refusals and final whole-owner schema binding. */
    //-------------------------------------------------------------------------------------------------
    internal static void Run()
    {
        const string xsd = "<xs:schema xmlns:xs='http://www.w3.org/2001/XMLSchema' xmlns='uri:ea.com:eala:asset' targetNamespace='uri:ea.com:eala:asset' elementFormDefault='qualified'><xs:simpleType name='FileReference'><xs:restriction base='xs:anyURI'/></xs:simpleType><xs:simpleType name='GameObjectWeakRef'><xs:restriction base='xs:string'/></xs:simpleType><xs:simpleType name='UpgradeTemplateWeakRef'><xs:restriction base='xs:string'/></xs:simpleType><xs:complexType name='ObjectFilter'/><xs:complexType name='BaseInheritableAsset'><xs:attribute name='id' type='xs:string' use='required'/><xs:attribute name='inheritFrom' type='xs:string'/></xs:complexType><xs:complexType name='GameDependencyType'><xs:sequence><xs:element name='RequiredObject' type='GameObjectWeakRef' minOccurs='0' maxOccurs='unbounded'/><xs:element name='ForbiddenUpgrade' type='UpgradeTemplateWeakRef' minOccurs='0' maxOccurs='unbounded'/><xs:element name='NeededUpgrade' type='UpgradeTemplateWeakRef' minOccurs='0' maxOccurs='unbounded'/><xs:element name='ObjectFilter' type='ObjectFilter' minOccurs='0'/></xs:sequence><xs:attribute name='ForbiddenModelConditions' type='xs:string'/></xs:complexType><xs:complexType name='UpgradeTemplate'><xs:complexContent><xs:extension base='BaseInheritableAsset'><xs:sequence><xs:element name='GameDependency' type='GameDependencyType' minOccurs='0' maxOccurs='1'/></xs:sequence><xs:attribute name='Value' type='xs:int'/><xs:attribute name='LocalPlayerBuildOnHoldEvaEvent' type='xs:string'/></xs:extension></xs:complexContent></xs:complexType><xs:element name='AssetDeclaration'><xs:complexType><xs:sequence><xs:element name='UpgradeTemplate' type='UpgradeTemplate' maxOccurs='unbounded'/></xs:sequence></xs:complexType></xs:element></xs:schema>";
        var schemas = SdkEffectiveSchema.Compile(new Dictionary<string,byte[]> { ["entry.xsd"] = Encoding.UTF8.GetBytes(xsd) },"entry.xsd").Schemas;
        string root = Path.Combine(Path.GetTempPath(),"Reborn-InstanceUpgrades-"+Guid.NewGuid().ToString("N")),data = Path.Combine(root,"Data"),entry = Path.Combine(data,"Entry.xml"),output = Path.Combine(root,"NoOutput"); Directory.CreateDirectory(data);
        const string payload = "<GameDependency><RequiredObject>Unit_A</RequiredObject><RequiredObject>Unit_A</RequiredObject></GameDependency>";
        const string condition = "<GameDependency ForbiddenModelConditions='STRUCTURE_UNPACKING'/>";
        const string prefix = "<UpgradeTemplate id='Base'/>";
        string chain = prefix+"<UpgradeTemplate id='Middle' inheritFrom='Base'>"+payload+condition+"</UpgradeTemplate><UpgradeTemplate id='Owner' inheritFrom='Middle'/>";
        var paths = Fixture(chain); byte[] raw = File.ReadAllBytes(entry);
        var result = new SdkInstanceInheritanceProfile(schemas,paths,upgrades:true).Apply(entry,raw);
        Require(result.Bytes != null && result.Evidence.Profile == SdkInstanceInheritanceProfile.UpgradeName && result.Evidence.UpgradeNormalizations.Length == 1 && result.Evidence.Overlays.Length == 2,"Upgrade admission failed: "+string.Join(";",result.Evidence.Diagnostics));
        var xml = new XmlDocument { XmlResolver = null }; xml.LoadXml(Encoding.UTF8.GetString(result.Bytes!));
        var owner = xml.DocumentElement!.ChildNodes.OfType<XmlElement>().Single(asset => asset.GetAttribute("id") == "Owner");
        var dependency = owner.ChildNodes.OfType<XmlElement>().Single();
        Require(dependency.GetAttribute("ForbiddenModelConditions") == "STRUCTURE_UNPACKING" && dependency.ChildNodes.OfType<XmlElement>().Select(leaf => leaf.InnerText).SequenceEqual(new[] { "Unit_A","Unit_A" }),"Condition or ordered duplicate requirements were lost.");
        Require(result.Evidence.UpgradeNormalizations.Single().Id == "Middle" && result.Evidence.UpgradeNormalizations.Single().Leaves.Length == 2,"Inherited normalization was counted as a new command.");
        Require(new SdkInstanceInheritanceProfile(schemas,paths,filters:true).Apply(entry,raw).Bytes == null,"Older filter profile admitted singleton normalization.");
        var graph = SdkTypedSourceGraph.BindGraph(schemas,paths,instanceUpgrades:true);
        Require(graph.ScopedGraphComplete && !graph.ProductionBuildReady && !graph.FullDependencyCoverage && File.ReadAllBytes(entry).SequenceEqual(raw) && !Directory.Exists(output),"Final schema/source/output isolation failed.");
        var reversed = Fixture(chain.Replace(payload+condition,condition+payload,StringComparison.Ordinal));
        Require(new SdkInstanceInheritanceProfile(schemas,reversed,upgrades:true).Apply(entry,File.ReadAllBytes(entry)).Bytes != null,"Reversed complementary pair was refused.");
        foreach (string bad in new[] {
            payload+payload,condition+condition,payload+condition+condition,
            payload+"<GameDependency/>",
            payload.Replace("<GameDependency>","<GameDependency ForbiddenModelConditions='OTHER'>",StringComparison.Ordinal)+condition,
            payload.Replace("<GameDependency>","<GameDependency id='X'>",StringComparison.Ordinal)+condition,
            payload.Replace("<RequiredObject>Unit_A</RequiredObject>","<ObjectFilter/>",StringComparison.Ordinal)+condition,
            payload.Replace("Unit_A","=EXPR",StringComparison.Ordinal)+condition,
            payload.Replace("Unit_A","",StringComparison.Ordinal)+condition,
            payload.Replace("Unit_A",new string('A',129),StringComparison.Ordinal)+condition,
            "<GameDependency>"+string.Concat(Enumerable.Repeat("<RequiredObject>Unit_A</RequiredObject>",129))+"</GameDependency>"+condition,
            "<GameDependency><RequiredObject><![CDATA[Unit_A]]></RequiredObject></GameDependency>"+condition,
            payload+condition.Replace("STRUCTURE_UNPACKING","+STRUCTURE_UNPACKING",StringComparison.Ordinal) })
            Reject(Fixture(prefix+"<UpgradeTemplate id='Owner' inheritFrom='Base'>"+bad+"</UpgradeTemplate>"));
        // Reborn: a late failed source must withhold normalization evidence already accumulated for earlier assets.
        Reject(Fixture(chain+"<UpgradeTemplate id='Bad' inheritFrom='Base'>"+payload+payload+"</UpgradeTemplate>"));
        Reject(Fixture(prefix+"<UpgradeTemplate id='Plain'>"+payload+condition+"</UpgradeTemplate>"));
        // Reborn: TypeId and instance directives must be rejected on the original owner before core normalization can strip them.
        Reject(Fixture(chain.Replace("id='Middle'","id='Middle' TypeId='0x1234'",StringComparison.Ordinal)));
        Reject(Fixture(chain.Replace("id='Middle'","id='Middle' xmlns:i='uri:ea.com:eala:asset:instance' i:joinAction='Append'",StringComparison.Ordinal)));
        // Reborn: a differently declared dependency cardinality cannot acquire the reviewed singleton normalization exception.
        var changedSchema = SdkEffectiveSchema.Compile(new Dictionary<string,byte[]> { ["entry.xsd"] = Encoding.UTF8.GetBytes(xsd.Replace("type='GameDependencyType' minOccurs='0' maxOccurs='1'","type='GameDependencyType' minOccurs='0' maxOccurs='2'",StringComparison.Ordinal)) },"entry.xsd").Schemas;
        var changedResult = SdkSelfAttributeInheritance.Apply(changedSchema,Encoding.UTF8.GetBytes("<AssetDeclaration xmlns='uri:ea.com:eala:asset'>"+chain+"</AssetDeclaration>"),upgrades:true);
        Require(changedResult.Bytes == null && changedResult.Evidence.UpgradeNormalizations.Length == 0,"Wrong singleton schema was admitted.");
        paths = Fixture(chain.Replace("id='Base'","id='Base' Value='bad'",StringComparison.Ordinal));
        var invalid = SdkTypedSourceGraph.BindGraph(schemas,paths,instanceUpgrades:true).Documents.Single();
        Require(invalid.Status == "SchemaInvalid" && invalid.Dependencies.Length == 0,"Normalization bypassed final schema validation.");
        bool conflict = false; try { SdkTypedSourceGraph.BindGraph(schemas,paths,instanceUpgrades:true,instanceFilters:true); } catch (ArgumentException) { conflict = true; } Require(conflict,"Conflicting upgrade/filter profiles admitted.");
        Console.WriteLine("SDK instance upgrades self-test: OK (complementary singleton folding, reversed order, retained conditions/duplicate references/local chains, older-profile isolation, atomic conflicts/two-populated/identity/directive/oversize/late refusal, final no-fields/source/output checks)");

        //-------------------------------------------------------------------------------------------------
        /** Reborn: write only owned temporary fixtures and capture their paths before diagnostic preparation. */
        //-------------------------------------------------------------------------------------------------
        SdkSourcePathAudit.Report Fixture(string body)
        {
            File.WriteAllBytes(entry,Encoding.UTF8.GetBytes("<AssetDeclaration xmlns='uri:ea.com:eala:asset'>"+body+"</AssetDeclaration>"));
            return SdkSourcePathAudit.Inspect(SdkEnvironmentPreflight.Inspect("ra3ep1",SdkEnvironmentPreflight.BaselineSchemaRoot(),data,entry,output,Array.Empty<string>()));
        }

        //-------------------------------------------------------------------------------------------------
        /** Reborn: assert normalization failures erase all previously accumulated transformed/source/operation witnesses. */
        //-------------------------------------------------------------------------------------------------
        void Reject(SdkSourcePathAudit.Report captured)
        {
            var rejected = new SdkInstanceInheritanceProfile(schemas,captured,upgrades:true).Apply(entry,File.ReadAllBytes(entry));
            Require(rejected.Bytes == null && rejected.Evidence.ProcessedSha256 == null && rejected.Evidence.UpgradeNormalizations.Length == 0 && rejected.Evidence.Filters.Length == 0 && rejected.Evidence.Overlays.Length == 0 && rejected.Evidence.ConsumedMarkers.Length == 0 && rejected.Evidence.PreparedSources.Length == 0 && rejected.Evidence.ImportedBases.Length == 0 && rejected.Evidence.Bitflags.Length == 0 && rejected.Evidence.Removals.Length == 0,"Normalization failure leaked partial evidence.");
        }
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: fail when diagnostic normalization changes without asserting native or game compatibility. */
    //-------------------------------------------------------------------------------------------------
    private static void Require(bool condition,string message) { if (!condition) throw new InvalidDataException(message); }
}
