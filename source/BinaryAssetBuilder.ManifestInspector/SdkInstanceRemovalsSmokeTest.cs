using System.Text;
using System.Xml;
using BinaryAssetBuilder.Core.SageXml;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: prove exact keyed empty-child removal through the core without admitting silent missing-target deletion or other directives.
internal static class SdkInstanceRemovalsSmokeTest
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: verify required-payload command stubs, chained removal identities, consumed directives, resource closure and atomic scope/validation failures. */
    //-------------------------------------------------------------------------------------------------
    internal static void Run()
    {
        const string xsd = "<xs:schema xmlns:xs='http://www.w3.org/2001/XMLSchema' xmlns='uri:ea.com:eala:asset' targetNamespace='uri:ea.com:eala:asset' elementFormDefault='qualified'><xs:simpleType name='FileReference'><xs:restriction base='xs:anyURI'/></xs:simpleType><xs:complexType name='BaseInheritableAsset' abstract='true'><xs:attribute name='id' type='xs:string' use='required'/><xs:attribute name='inheritFrom' type='xs:string'/></xs:complexType><xs:complexType name='Item'><xs:attribute name='id' type='xs:string'/><xs:attribute name='State' type='xs:int' use='required'/><xs:attribute name='Filename' type='FileReference'/></xs:complexType><xs:complexType name='Branch'><xs:sequence><xs:element name='Item' type='Item' minOccurs='0' maxOccurs='4'/></xs:sequence><xs:attribute name='id' type='xs:string'/></xs:complexType><xs:complexType name='Asset'><xs:complexContent><xs:extension base='BaseInheritableAsset'><xs:sequence><xs:element name='Single' type='Item' minOccurs='0'/><xs:element name='Item' type='Item' minOccurs='1' maxOccurs='4'/><xs:element name='Other' type='Item' minOccurs='0' maxOccurs='4'/><xs:element name='Branch' type='Branch' minOccurs='0' maxOccurs='4'/></xs:sequence></xs:extension></xs:complexContent></xs:complexType><xs:element name='AssetDeclaration'><xs:complexType><xs:sequence><xs:element name='Includes' minOccurs='0'><xs:complexType><xs:sequence><xs:element name='Include' minOccurs='0' maxOccurs='unbounded'><xs:complexType><xs:attribute name='type' type='xs:string'/><xs:attribute name='source' type='xs:string'/></xs:complexType></xs:element></xs:sequence></xs:complexType></xs:element><xs:element name='Asset' type='Asset' minOccurs='0' maxOccurs='unbounded'/></xs:sequence></xs:complexType></xs:element></xs:schema>";
        var schemas = SdkEffectiveSchema.Compile(new Dictionary<string,byte[]> { ["entry.xsd"] = Encoding.UTF8.GetBytes(xsd) },"entry.xsd").Schemas;
        string root = Path.Combine(Path.GetTempPath(),"Reborn-InstanceRemovals-"+Guid.NewGuid().ToString("N")),data = Path.Combine(root,"Data"),entry = Path.Combine(data,"Entry.xml"),output = Path.Combine(root,"NoOutput"); Directory.CreateDirectory(data);
        File.WriteAllBytes(Path.Combine(data,"payload.bin"),new byte[] { 7 });
        string leaf = "<Asset id='Leaf'><Item id='keep' State='1' Filename='DATA:payload.bin'/><Item id='delete' State='2' Filename='DATA:payload.bin'/><Item id='second' State='3' Filename='DATA:payload.bin'/></Asset>";
        string middle = Includes("Leaf.xml")+"<Asset id='Middle' inheritFrom='Leaf'>"+Remove("Item","delete")+"</Asset>";
        string owner = Includes("Middle.xml")+"<Asset id='Owner' inheritFrom='Middle'>"+Remove("Item","second")+"<Item id='keep' State='7'/></Asset>";
        var paths = Fixture(owner,middle,leaf); byte[] raw = File.ReadAllBytes(entry);
        var result = new SdkInstanceInheritanceProfile(schemas,paths,removals:true).Apply(entry,raw);
        Require(result.Bytes != null && result.Evidence.Profile == SdkInstanceInheritanceProfile.RemovalName && result.Evidence.PreparedSources.Length == 3 && result.Evidence.Removals.Length == 1,"Removal source closure/profile evidence failed.");
        var witness = result.Evidence.Removals.Single();
        Require(witness.Type == "Asset" && witness.DerivedId == "Owner" && witness.BaseId == "Middle" && witness.ChildName == "Item" && witness.ChildId == "second","Owner removal witness replayed a child command or changed identity.");
        var xml = Parse(result.Bytes!); var items = xml.DocumentElement!.ChildNodes.OfType<XmlElement>().Single(node => node.LocalName == "Asset").ChildNodes.OfType<XmlElement>().ToArray();
        Require(items.Length == 1 && items[0].GetAttribute("id") == "keep" && items[0].GetAttribute("State") == "7" && items[0].GetAttribute("Filename") == "DATA:payload.bin" && xml.SelectNodes("//@*[local-name()='joinAction']")!.Count == 0,"Core removal/overlay order or consumed directives differed.");
        Require(new SdkInstanceInheritanceProfile(schemas,paths,chains:true).Apply(entry,raw).Bytes == null,"Older chain-only profile admitted removal directives.");
        var graph = SdkTypedSourceGraph.BindGraph(schemas,paths,instanceRemovals:true);
        Require(graph.ScopedGraphComplete && graph.Documents.Sum(document => document.Dependencies.Length) == 6 && graph.Documents.Single(document => document.SourcePath.EndsWith("Middle.xml",StringComparison.Ordinal)).Inheritance!.Removals.Single().ChildId == "delete" && !graph.ProductionBuildReady && File.ReadAllBytes(entry).SequenceEqual(raw) && !Directory.Exists(output),"Post-removal field/source/output isolation failed.");
        // Reborn: demonstrate that core missing-ID removal is a warning/no-op, then require the stricter diagnostic admission to refuse it.
        byte[] absent = Source("<Asset id='Base'><Item id='keep' State='1'/></Asset><Asset id='Child' inheritFrom='Base'>"+Remove("Item","missing")+"</Asset>");
        var coreXml = Parse(absent); var coreAssets = coreXml.DocumentElement!.ChildNodes.OfType<XmlElement>().ToArray();
        Require(NodeJoiner.Override(schemas,coreXml,coreAssets[0],coreAssets[1]).ChildNodes.OfType<XmlElement>().Count() == 1,"Core absent-removal behavior changed; review required.");
        Require(SdkSelfAttributeInheritance.Apply(schemas,absent,childRemoval:true).Bytes == null,"Absent target was silently accepted.");
        foreach (string bad in new[] {
            Remove("Item","missing"),Remove("Other","keep"),Remove("Single","keep"),Remove("Branch","keep"),
            "<Item id='keep' i:joinAction='Remove' State='1'/>","<Item id='keep' i:joinAction='Remove'><!--not empty--></Item>",
            "<Item id='keep' i:joinAction='Remove' i:insertPosition='Top'/>","<Item id='keep' i:joinAction='remove'/>",
            "<Item id='keep' i:joinAction='Replace'/>","<Item id='keep' i:joinAction='Append'/>","<Item i:joinAction='Remove'/>",
            "<Item id='../unsafe' i:joinAction='Remove'/>","<Item id='=ID' i:joinAction='Remove'/>",
            "<Item id='keep' joinAction='Remove'/>","<Item id='keep' xmlns:f='foreign' f:joinAction='Remove'/>",
            Remove("Item","keep")+Remove("Item","keep"),Remove("Item","keep")+"<Other id='keep' State='2'/>",
            "<Branch id='branch'>"+Remove("Item","keep")+"</Branch>" })
            Reject(Fixture(Includes("Middle.xml")+"<Asset id='Owner' inheritFrom='Middle'>"+bad+"</Asset>",Includes("Leaf.xml")+"<Asset id='Middle' inheritFrom='Leaf'/>",leaf));
        Reject(Fixture("<Asset id='Owner'>"+Remove("Item","keep")+"</Asset>",middle,leaf));
        // Reborn: even after a real deletion, a later missing target must withhold all earlier removal witnesses and transformed bytes.
        Reject(Fixture(Includes("Middle.xml")+"<Asset id='Owner' inheritFrom='Middle'>"+Remove("Item","delete")+Remove("Item","missing")+"</Asset>",Includes("Leaf.xml")+"<Asset id='Middle' inheritFrom='Leaf'/>",leaf));
        // Reborn: deleting all required children is mechanically admitted but fails final schema validation and cannot yield trusted file fields.
        paths = Fixture(Includes("Middle.xml")+"<Asset id='Owner' inheritFrom='Middle'>"+Remove("Item","keep")+Remove("Item","second")+"</Asset>",middle,leaf);
        graph = SdkTypedSourceGraph.BindGraph(schemas,paths,instanceRemovals:true);
        var invalid = graph.Documents.Single(document => document.SourcePath == entry);
        Require(invalid.Status == "SchemaInvalid" && invalid.Dependencies.Length == 0,"Removal bypassed required final cardinality.");
        bool conflict = false; try { SdkTypedSourceGraph.BindGraph(schemas,paths,instanceChains:true,instanceRemovals:true); } catch (ArgumentException) { conflict = true; } Require(conflict,"Conflicting removal profile flags admitted.");
        Console.WriteLine("SDK instance removals self-test: OK (keyed required-payload stubs, exact target/type identity, chained owner-only witnesses, consumed directives and field closure, core absent-target no-op observation, atomic missing/collision/duplicate/singleton/branch/nested/payload/wrong-namespace/directive rejection, post-removal cardinality no-fields and older-profile/source/output isolation)");

        //-------------------------------------------------------------------------------------------------
        /** Reborn: write and snapshot only owned removal fixtures, never synced official XML or schemas. */
        //-------------------------------------------------------------------------------------------------
        SdkSourcePathAudit.Report Fixture(string entryBody,string middleBody,string leafBody)
        {
            File.WriteAllBytes(entry,Source(entryBody)); File.WriteAllBytes(Path.Combine(data,"Middle.xml"),Source(middleBody)); File.WriteAllBytes(Path.Combine(data,"Leaf.xml"),Source(leafBody));
            return SdkSourcePathAudit.Inspect(SdkEnvironmentPreflight.Inspect("ra3ep1",SdkEnvironmentPreflight.BaselineSchemaRoot(),data,entry,output,Array.Empty<string>()));
        }

        //-------------------------------------------------------------------------------------------------
        /** Reborn: refusal must atomically withhold removal and source-closure evidence even after earlier successful commands. */
        //-------------------------------------------------------------------------------------------------
        void Reject(SdkSourcePathAudit.Report rejectedPaths)
        {
            var rejected = new SdkInstanceInheritanceProfile(schemas,rejectedPaths,removals:true).Apply(entry,File.ReadAllBytes(entry));
            Require(rejected.Bytes == null && rejected.Evidence.ProcessedSha256 == null && rejected.Evidence.Removals.Length == 0 && rejected.Evidence.Overlays.Length == 0 && rejected.Evidence.PreparedSources.Length == 0 && rejected.Evidence.ImportedBases.Length == 0,"Rejected removal leaked partial evidence.");
        }
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: author literal keyed removal commands under the explicit instance namespace. */
    //-------------------------------------------------------------------------------------------------
    private static string Remove(string name,string id) => "<"+name+" id='"+id+"' i:joinAction='Remove'/>";

    //-------------------------------------------------------------------------------------------------
    /** Reborn: retain literal direct-instance Include role and source identity for owned tests. */
    //-------------------------------------------------------------------------------------------------
    private static string Includes(string name) => "<Includes><Include type='instance' source='"+name+"'/></Includes>";

    //-------------------------------------------------------------------------------------------------
    /** Reborn: encode only owned EA removal fixture declarations with their explicit instance prefix. */
    //-------------------------------------------------------------------------------------------------
    private static byte[] Source(string body) => Encoding.UTF8.GetBytes("<AssetDeclaration xmlns='uri:ea.com:eala:asset' xmlns:i='uri:ea.com:eala:asset:instance'>"+body+"</AssetDeclaration>");

    //-------------------------------------------------------------------------------------------------
    /** Reborn: inspect owned diagnostic fixture XML without external resolution. */
    //-------------------------------------------------------------------------------------------------
    private static XmlDocument Parse(byte[] bytes) { XmlDocument xml = new() { XmlResolver = null }; xml.LoadXml(Encoding.UTF8.GetString(bytes)); return xml; }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: fail strict removal regressions without claiming native stream or game compatibility. */
    //-------------------------------------------------------------------------------------------------
    private static void Require(bool condition,string message) { if (!condition) throw new InvalidDataException(message); }
}
