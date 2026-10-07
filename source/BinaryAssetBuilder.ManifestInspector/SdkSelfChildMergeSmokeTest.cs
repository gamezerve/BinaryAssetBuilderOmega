using System.Security.Cryptography;
using System.Text;
using System.Xml;
using BinaryAssetBuilder.Core.SageXml;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: prove a narrow two-sided empty-complex-child scope through the existing core, never assuming replacement semantics for text or populated branches.
internal static class SdkSelfChildMergeSmokeTest
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: verify literal attribute overlays, repeated-ID selection, anonymous append, singleton names, order, chain stability and atomic scope/resource guards. */
    //-------------------------------------------------------------------------------------------------
    internal static void Run()
    {
        const string xsd = "<xs:schema xmlns:xs='http://www.w3.org/2001/XMLSchema' xmlns='uri:ea.com:eala:asset' targetNamespace='uri:ea.com:eala:asset' elementFormDefault='qualified'><xs:simpleType name='FileReference'><xs:restriction base='xs:anyURI'/></xs:simpleType><xs:simpleType name='Flags'><xs:list itemType='xs:string'/></xs:simpleType><xs:complexType name='Empty'><xs:attribute name='id' type='xs:string'/><xs:attribute name='A' type='xs:int'/><xs:attribute name='B' type='xs:int'/><xs:attribute name='Filename' type='FileReference'/><xs:attribute name='Flags' type='Flags'/></xs:complexType><xs:complexType name='Branch'><xs:sequence><xs:element name='Item' type='Empty' minOccurs='0' maxOccurs='3'/></xs:sequence></xs:complexType><xs:complexType name='Asset'><xs:sequence><xs:element name='Single' type='Empty' minOccurs='0'/><xs:element name='Item' type='Empty' minOccurs='0' maxOccurs='3'/><xs:element name='Other' type='Empty' minOccurs='0' maxOccurs='3'/><xs:element name='Text' type='xs:string' minOccurs='0'/><xs:element name='Branch' type='Branch' minOccurs='0'/></xs:sequence><xs:attribute name='id' type='xs:string' use='required'/><xs:attribute name='inheritFrom' type='xs:string'/></xs:complexType><xs:element name='AssetDeclaration'><xs:complexType><xs:sequence><xs:element name='Asset' type='Asset' maxOccurs='unbounded'/></xs:sequence></xs:complexType></xs:element></xs:schema>";
        var schema = SdkEffectiveSchema.Compile(new Dictionary<string,byte[]> { ["entry.xsd"] = Encoding.UTF8.GetBytes(xsd) },"entry.xsd").Schemas;
        byte[] raw = Source("<Asset id='Base'><Single id='old' A='1' B='2'/><Item id='same' A='1' B='2' Filename='local.bin'/><Item id='keep' A='2'/><Text>base text</Text><Branch><Item A='7'/></Branch></Asset><Asset id='Child' inheritFrom='Base'><Single id='new' A='3'/><Item id='same' A='4'/><Item id='added' A='5'/></Asset><Asset id='Grandchild' inheritFrom='Child'><Item id='same' B='9'/></Asset>");
        var result = SdkSelfAttributeInheritance.Apply(schema,raw,childMerge:true);
        Require(result.Bytes != null && result.Evidence.Profile == SdkSelfAttributeInheritance.ChildMergeName && result.Evidence.Overlays.Length == 2 && result.Evidence.RawSha256 == Convert.ToHexString(SHA256.HashData(raw)),"Empty-child profile/chain identity failed.");
        var xml = Parse(result.Bytes!); var last = xml.DocumentElement!.ChildNodes.OfType<XmlElement>().Last();
        var children = last.ChildNodes.OfType<XmlElement>().ToArray();
        Require(string.Join(',',children.Select(child => child.LocalName)) == "Single,Item,Item,Item,Text,Branch","Sequence order or repeated-ID cardinality changed.");
        Require(children[0].GetAttribute("id") == "new" && children[0].GetAttribute("A") == "3" && children[0].GetAttribute("B") == "2","Singleton-name attribute overlay failed.");
        Require(children[1].GetAttribute("id") == "same" && children[1].GetAttribute("A") == "4" && children[1].GetAttribute("B") == "9" && children[1].GetAttribute("Filename") == "local.bin" && children[2].GetAttribute("id") == "keep" && children[3].GetAttribute("id") == "added","Repeated-ID overlay/retained attributes/order failed.");
        Require(children[4].InnerText == "base text" && SdkEffectiveSchema.Bind(schema,"fixture.xml",result.Bytes!).XmlValidated,"Unmatched text/branch copying or final schema failed.");
        Require(SdkSelfAttributeInheritance.Apply(schema,raw,treeCopy:true).Bytes == null,"Earlier tree-copy profile widened.");
        var anonymous = SdkSelfAttributeInheritance.Apply(schema,Source("<Asset id='Base'><Item A='1'/></Asset><Asset id='Child' inheritFrom='Base'><Item A='2'/></Asset>"),childMerge:true);
        Require(anonymous.Bytes != null && Parse(anonymous.Bytes!).DocumentElement!.LastChild!.ChildNodes.OfType<XmlElement>().Select(child => child.GetAttribute("A")).SequenceEqual(new[] { "1","2" }),"Anonymous repeated children did not append.");
        // Reborn: demonstrate the core hazard explicitly so rejection cannot be mistaken for an assumed replacement implementation.
        var textXml = Parse(Source("<Asset id='Base'><Text>left</Text></Asset><Asset id='Child' inheritFrom='Base'><Text>right</Text></Asset>"));
        var textAssets = textXml.DocumentElement!.ChildNodes.OfType<XmlElement>().ToArray();
        Require(NodeJoiner.Override(schema,textXml,textAssets[0],textAssets[1]).InnerText == "leftright","Core matched-text append observation changed; re-review required.");
        foreach (var pair in new[] {
            ("<Text>left</Text>","<Text>right</Text>"),
            ("<Branch><Item A='1'/></Branch>","<Branch><Item A='2'/></Branch>"),
            ("<Item id='x'/>","<Other id='x'/>"),
            ("<Single id='x'/>","<Other id='x'/>"),
            ("<Item id='a'/><Item id='b'/>","<Item id='c'/><Item id='d'/>"),
            ("<Item/>","<Item/><Item/><Item/>"),
            ("<Item id='x'/>","<Item id='x' A='=1+2'/>"),
            ("<Item id='x'/>","<Item id='x' Flags='+A'/>"),
            ("<Item id='x'/>","<Item id='x' Unknown='1'/>"),
            ("<Item id='x'/>","<Item id='x' xmlns:i='uri:ea.com:eala:asset:instance' i:joinAction='Replace'/>"),
            ("<Item id='x'/>","<Item id='x'/><Other id='x'/>"),
            ("<Item id='x'/>","<p:Item xmlns:p='uri:ea.com:eala:asset' id='x'/>"),
            ("<Item id='x'/>","<Item id='x' inheritFrom='y'/>"),
            ("<Item id='x'/>","<Item id='../unsafe'/>"),
            ("<Item id='x'/>","<Item id='x'><![CDATA[text]]></Item>") })
            Reject(Source("<Asset id='Base'>"+pair.Item1+"</Asset><Asset id='Child' inheritFrom='Base'>"+pair.Item2+"</Asset>"));
        // Reborn: copied payload fields remain consumer-local in this same-document profile; rejected/invalid sources never expose trusted dependency fields.
        string root = Path.Combine(Path.GetTempPath(),"Reborn-ChildMerge-"+Guid.NewGuid().ToString("N")),data = Path.Combine(root,"Data"),output = Path.Combine(root,"NoOutput"); Directory.CreateDirectory(data);
        string entry = Path.Combine(data,"Entry.xml"); File.WriteAllBytes(entry,raw); File.WriteAllBytes(Path.Combine(data,"local.bin"),new byte[] { 7 });
        var paths = SdkSourcePathAudit.Inspect(SdkEnvironmentPreflight.Inspect("ra3ep1",SdkEnvironmentPreflight.BaselineSchemaRoot(),data,entry,output,Array.Empty<string>()));
        var graph = SdkTypedSourceGraph.BindGraph(schema,paths,selfChildMerge:true);
        Require(graph.ScopedGraphComplete && graph.Documents.Single().Dependencies.Length == 3 && graph.PreprocessingProfile == SdkSelfAttributeInheritance.ChildMergeName && !graph.ProductionBuildReady && File.ReadAllBytes(entry).SequenceEqual(raw) && !Directory.Exists(output),"Graph identity/fields/output isolation failed.");
        bool conflict = false; try { SdkTypedSourceGraph.BindGraph(schema,paths,selfChildMerge:true,selfTreeCopy:true); } catch (ArgumentException) { conflict = true; } Require(conflict,"Mixed profiles admitted.");
        File.WriteAllBytes(entry,Source("<Asset id='Base'><Item id='x' A='1'/></Asset><Asset id='Child' inheritFrom='Base'><Item id='x' A='bad' Filename='local.bin'/></Asset>"));
        paths = SdkSourcePathAudit.Inspect(SdkEnvironmentPreflight.Inspect("ra3ep1",SdkEnvironmentPreflight.BaselineSchemaRoot(),data,entry,output,Array.Empty<string>()));
        graph = SdkTypedSourceGraph.BindGraph(schema,paths,selfChildMerge:true);
        Require(graph.Documents.Single().Status == "SchemaInvalid" && graph.Documents.Single().Dependencies.Length == 0,"Invalid merged scalar exposed trusted fields.");
        // Reborn: bound inherited allocation before core merging even for long literal child attributes and shared-base chains.
        Reject(Source("<Asset id='Base'><Item id='x' Filename='"+new string('x',70000)+"'/></Asset>"+string.Concat(Enumerable.Range(0,70).Select(index => "<Asset id='D"+index+"' inheritFrom='Base'><Item id='x' A='1'/></Asset>"))));
        Console.WriteLine("SDK self child merge self-test: OK (empty-complex singleton/repeated-ID overlays, anonymous append, order/chains/retained attributes, core text-append hazard, atomic collision/occurrence/directive/expression/prefix/branch/text/amplification rejection, schema-invalid no-fields and old-profile/source/output isolation)");

        //-------------------------------------------------------------------------------------------------
        /** Reborn: a rejected two-sided document must publish neither partial XML nor partial overlay identities. */
        //-------------------------------------------------------------------------------------------------
        void Reject(byte[] input)
        { var rejected = SdkSelfAttributeInheritance.Apply(schema,input,childMerge:true); Require(rejected.Bytes == null && rejected.Evidence.ProcessedSha256 == null && rejected.Evidence.Overlays.Length == 0,"Rejected merge leaked partial evidence."); }
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: encode owned EA fixture declarations without modifying official sources. */
    //-------------------------------------------------------------------------------------------------
    private static byte[] Source(string body) => Encoding.UTF8.GetBytes("<AssetDeclaration xmlns='uri:ea.com:eala:asset'>"+body+"</AssetDeclaration>");

    //-------------------------------------------------------------------------------------------------
    /** Reborn: inspect only owned in-memory fixture output with external resolution disabled. */
    //-------------------------------------------------------------------------------------------------
    private static XmlDocument Parse(byte[] bytes) { XmlDocument xml = new() { XmlResolver = null }; xml.LoadXml(Encoding.UTF8.GetString(bytes)); return xml; }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: fail diagnostic merge regressions without claiming native stream or game readiness. */
    //-------------------------------------------------------------------------------------------------
    private static void Require(bool condition,string message) { if (!condition) throw new InvalidDataException(message); }
}
