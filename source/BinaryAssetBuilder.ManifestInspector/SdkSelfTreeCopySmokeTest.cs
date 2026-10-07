using System.Text;
using System.Xml;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: prove nested sequence copying through the core joiner without enabling populated-child matching or imported inheritance.
internal static class SdkSelfTreeCopySmokeTest
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: test schema-selected anonymous/inherited/recursive branches, repeated order, bounds, atomic failures and trusted graph isolation. */
    //-------------------------------------------------------------------------------------------------
    internal static void Run()
    {
        const string xsd = "<xs:schema xmlns:xs='http://www.w3.org/2001/XMLSchema' xmlns='uri:ea.com:eala:asset' targetNamespace='uri:ea.com:eala:asset' elementFormDefault='qualified'><xs:simpleType name='FileReference'><xs:restriction base='xs:anyURI'/></xs:simpleType><xs:simpleType name='Flags'><xs:list itemType='xs:string'/></xs:simpleType><xs:complexType name='Branch'><xs:sequence><xs:element name='Filename' type='FileReference' minOccurs='0' maxOccurs='2'/><xs:element name='Branch' type='Branch' minOccurs='0' maxOccurs='2'/></xs:sequence><xs:attribute name='Value' type='xs:int'/><xs:attribute name='id' type='xs:string'/><xs:attribute name='Flags' type='Flags'/></xs:complexType><xs:complexType name='DerivedBranch'><xs:complexContent><xs:extension base='Branch'><xs:attribute name='Extra' type='xs:string'/></xs:extension></xs:complexContent></xs:complexType><xs:complexType name='Choice'><xs:choice><xs:element name='A' type='xs:string'/><xs:element name='B' type='xs:string'/></xs:choice></xs:complexType><xs:complexType name='Mixed' mixed='true'><xs:sequence><xs:element name='A' type='xs:string' minOccurs='0'/></xs:sequence></xs:complexType><xs:complexType name='Asset'><xs:sequence><xs:element name='Payload' type='DerivedBranch' minOccurs='0'/><xs:element name='List' minOccurs='0'><xs:complexType><xs:sequence><xs:element name='Item' type='FileReference' minOccurs='0' maxOccurs='unbounded'/></xs:sequence></xs:complexType></xs:element><xs:element name='Choice' type='Choice' minOccurs='0'/><xs:element name='Mixed' type='Mixed' minOccurs='0'/></xs:sequence><xs:attribute name='id' type='xs:string' use='required'/><xs:attribute name='inheritFrom' type='xs:string'/></xs:complexType><xs:element name='AssetDeclaration'><xs:complexType><xs:sequence><xs:element name='Asset' type='Asset' maxOccurs='unbounded'/></xs:sequence></xs:complexType></xs:element></xs:schema>";
        // Reborn: add a differently named sibling of the same type to prove that ID uniqueness is not mistakenly scoped by QName.
        string fixtureSchema = xsd.Replace("<xs:element name='List'","<xs:element name='OtherPayload' type='DerivedBranch' minOccurs='0'/><xs:element name='List'",StringComparison.Ordinal);
        var schemas = SdkEffectiveSchema.Compile(new Dictionary<string,byte[]> { ["entry.xsd"] = Encoding.UTF8.GetBytes(fixtureSchema) },"entry.xsd");
        const string children = "<Payload id='PayloadKey' Value='-1' Extra='literal'><Filename>local.bin</Filename><Filename>other.bin</Filename><Branch id='A' Value='2'><Filename>local.bin</Filename><Branch id='A'/></Branch><Branch id='B' Value='3'/></Payload><OtherPayload id='Other'/><List><Item>other.bin</Item></List>";
        byte[] raw = Source("<Asset id='Base'>"+children+"</Asset><Asset id='Child' inheritFrom='Base'/><Asset id='Grandchild' inheritFrom='Child'/>");
        foreach (byte[] input in new[] { raw,Source("<Asset id='Base'/><Asset id='Child' inheritFrom='Base'>"+children+"</Asset>") })
        {
            var result = SdkSelfAttributeInheritance.Apply(schemas.Schemas,input,treeCopy:true);
            Require(result.Bytes != null && result.Evidence.Profile == SdkSelfAttributeInheritance.TreeCopyName,"Tree copying failed.");
            XmlDocument xml = new(); xml.LoadXml(Encoding.UTF8.GetString(result.Bytes!));
            var copied = xml.DocumentElement!.ChildNodes.OfType<XmlElement>().Last();
            Require(string.Concat(copied.ChildNodes.OfType<XmlElement>().Select(element => element.OuterXml)).Replace(" xmlns=\"uri:ea.com:eala:asset\"","",StringComparison.Ordinal).Replace(" />","/>",StringComparison.Ordinal).Replace('"','\'') == children,"Recursive text/attributes/order differed.");
            Require(SdkEffectiveSchema.Bind(schemas.Schemas,"fixture.xml",result.Bytes!).XmlValidated,"Copied tree did not validate.");
        }
        Require(SdkSelfAttributeInheritance.Apply(schemas.Schemas,raw,complexChildCopy:true).Bytes == null,"Complex leaf profile admitted nested trees.");
        foreach (string invalid in new[] {
            "<Payload><Branch id='x'/><Branch id='x'/></Payload>","<Payload id='x'/><OtherPayload id='x'/>","<Payload><Branch id='../unsafe'/></Payload>","<Payload><Branch Value='=1+2'/></Payload>","<Payload><Branch Unknown='x'/></Payload>",
            "<Payload><Branch Flags='+A'/></Payload>","<Payload><Branch xmlns:i='uri:ea.com:eala:asset:instance' i:insertPosition='Top'/></Payload>",
            "<Payload><Branch inheritFrom='x'/></Payload>","<Payload><Branch><Filename> <!--split-->=1+2</Filename></Branch></Payload>",
            "<Payload><Branch><![CDATA[text]]></Branch></Payload>","<Payload>meaningful<Branch/></Payload>",
            "<Payload><Branch><Unknown/></Branch></Payload>","<Payload><Branch><Filename>A</Filename><Filename>B</Filename><Filename>C</Filename></Branch></Payload>",
            "<Payload><Branch><?unexpected x?></Branch></Payload>","<Choice><A>x</A></Choice>","<Mixed><A>x</A></Mixed>",
            "<Payload><a:Branch xmlns:a='uri:ea.com:eala:asset'/></Payload>" })
            Reject(Source("<Asset id='Base'>"+invalid+"</Asset><Asset id='Child' inheritFrom='Base'/>"));
        Reject(Source("<Asset id='Base'>"+children+"</Asset><Asset id='Child' inheritFrom='Base'><Payload/></Asset>"));
        // Reborn: test exact authored depth/node boundaries separately from serialized amplification and inheritance depth.
        string depth32 = "<Payload>"+string.Concat(Enumerable.Repeat("<Branch>",30))+"<Filename>local.bin</Filename>"+string.Concat(Enumerable.Repeat("</Branch>",30))+"</Payload>";
        Require(SdkSelfAttributeInheritance.Apply(schemas.Schemas,Source("<Asset id='Base'>"+depth32+"</Asset><Asset id='Child' inheritFrom='Base'/>"),treeCopy:true).Bytes != null,"Exact tree depth boundary failed.");
        Reject(Source("<Asset id='Base'>"+depth32.Replace("<Filename>","<Branch><Filename>",StringComparison.Ordinal).Replace("</Filename>","</Filename></Branch>",StringComparison.Ordinal)+"</Asset><Asset id='Child' inheritFrom='Base'/>"));
        string nodes = "<List>"+string.Concat(Enumerable.Repeat("<Item>local.bin</Item>",8189))+"</List>";
        Require(SdkSelfAttributeInheritance.Apply(schemas.Schemas,Source("<Asset id='Base'>"+nodes+"</Asset><Asset id='Child' inheritFrom='Base'/>"),treeCopy:true).Bytes != null,"Exact 8192-element tree boundary failed.");
        Reject(Source("<Asset id='Base'>"+nodes.Replace("</List>","<Item>local.bin</Item></List>",StringComparison.Ordinal)+"</Asset><Asset id='Child' inheritFrom='Base'/>"));
        string amplified = "<Asset id='Base'><Payload><Branch><Filename>"+new string('x',70000)+"</Filename></Branch></Payload></Asset>"+string.Concat(Enumerable.Range(0,70).Select(index => "<Asset id='D"+index+"' inheritFrom='Base'/>")); Reject(Source(amplified));
        string root = Path.Combine(Path.GetTempPath(),"Reborn-TreeCopy-"+Guid.NewGuid().ToString("N")),data = Path.Combine(root,"Data"); Directory.CreateDirectory(data);
        string entry = Path.Combine(data,"Entry.xml"); File.WriteAllBytes(entry,raw); File.WriteAllBytes(Path.Combine(data,"local.bin"),new byte[] { 7 }); File.WriteAllBytes(Path.Combine(data,"other.bin"),new byte[] { 8 });
        var paths = SdkSourcePathAudit.Inspect(SdkEnvironmentPreflight.Inspect("ra3ep1",SdkEnvironmentPreflight.BaselineSchemaRoot(),data,entry,Path.Combine(root,"NoOutput"),Array.Empty<string>()));
        var graph = SdkTypedSourceGraph.BindGraph(schemas.Schemas,paths,selfTreeCopy:true);
        Require(graph.ScopedGraphComplete && graph.PreprocessingProfile == SdkSelfAttributeInheritance.TreeCopyName && graph.Documents.Single().Dependencies.Length == 12 && !graph.ProductionBuildReady && File.ReadAllBytes(entry).SequenceEqual(raw) && !Directory.Exists(Path.Combine(root,"NoOutput")),"Tree graph fields/source/output isolation differed.");
        Require(SdkTypedSourceGraph.BindGraph(schemas.Schemas,paths,selfComplexChildCopy:true).Documents.Single().Status == "RequiresPreprocessing","Earlier graph profile changed.");
        File.WriteAllBytes(entry,Source("<Asset id='Base'><Payload><Branch Value='bad'><Filename>local.bin</Filename></Branch></Payload></Asset><Asset id='Child' inheritFrom='Base'/>"));
        var invalidPaths = SdkSourcePathAudit.Inspect(SdkEnvironmentPreflight.Inspect("ra3ep1",SdkEnvironmentPreflight.BaselineSchemaRoot(),data,entry,Path.Combine(root,"NoOutput"),Array.Empty<string>()));
        var invalidGraph = SdkTypedSourceGraph.BindGraph(schemas.Schemas,invalidPaths,selfTreeCopy:true);
        Require(invalidGraph.Documents.Single().Status == "SchemaInvalid" && invalidGraph.Documents.Single().Dependencies.Length == 0,"Invalid nested scalar leaked trusted fields.");
        bool conflict = false; try { SdkTypedSourceGraph.BindGraph(schemas.Schemas,paths,selfTreeCopy:true,selfComplexChildCopy:true); } catch (ArgumentException) { conflict = true; } Require(conflict,"Conflicting tree profiles admitted.");
        Console.WriteLine("SDK self tree copy self-test: OK (recursive/inherited/anonymous sequences, unique sibling IDs and parent-local ID reuse, one-sided/chained order/text/attributes, 32-depth/8192-element boundaries, atomic duplicate/unsafe-id/directive/expression/choice/mixed/prefix/occurrence/amplification rejection, schema-invalid no-fields and source/output/old-profile isolation; no populated-child matching)");

        //-------------------------------------------------------------------------------------------------
        /** Reborn: unsupported tree admission withholds every partial output and overlay, even after earlier eligible assets. */
        //-------------------------------------------------------------------------------------------------
        void Reject(byte[] input)
        { var rejected = SdkSelfAttributeInheritance.Apply(schemas.Schemas,input,treeCopy:true); Require(rejected.Bytes == null && rejected.Evidence.ProcessedSha256 == null && rejected.Evidence.Overlays.Length == 0,"Rejected tree published partial evidence."); }
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: encode only owned recursive fixtures without changing official XML or schema references. */
    //-------------------------------------------------------------------------------------------------
    private static byte[] Source(string body) => Encoding.UTF8.GetBytes("<AssetDeclaration xmlns='uri:ea.com:eala:asset'>"+body+"</AssetDeclaration>");

    //-------------------------------------------------------------------------------------------------
    /** Reborn: report tree-copy regression failures without claiming production or game stream compatibility. */
    //-------------------------------------------------------------------------------------------------
    private static void Require(bool condition,string message) { if (!condition) throw new InvalidDataException(message); }
}
