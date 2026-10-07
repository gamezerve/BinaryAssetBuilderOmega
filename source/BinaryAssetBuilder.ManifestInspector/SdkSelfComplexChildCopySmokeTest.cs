using System.Text;
using System.Xml;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: prove attributed simpleContent and empty-content leaf copying without admitting recursive trees or child-ID matching.
internal static class SdkSelfComplexChildCopySmokeTest
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: cover leaf attributes/text/order, both one-sided directions, atomic scope failures, and schema-gated graph dependencies. */
    //-------------------------------------------------------------------------------------------------
    internal static void Run()
    {
        const string xsd = "<xs:schema xmlns:xs='http://www.w3.org/2001/XMLSchema' xmlns='uri:ea.com:eala:asset' targetNamespace='uri:ea.com:eala:asset' elementFormDefault='qualified'><xs:simpleType name='FileReference'><xs:restriction base='xs:anyURI'/></xs:simpleType><xs:simpleType name='Flags'><xs:list itemType='xs:string'/></xs:simpleType><xs:complexType name='Weighted'><xs:simpleContent><xs:extension base='FileReference'><xs:attribute name='Weight' type='xs:int' default='1000'/><xs:attribute name='id' type='xs:string'/><xs:attribute name='Flags' type='Flags'/></xs:extension></xs:simpleContent></xs:complexType><xs:complexType name='Range'><xs:attribute name='Low' type='xs:int' use='required'/><xs:attribute name='High' type='xs:int' use='required'/></xs:complexType><xs:complexType name='Nested'><xs:sequence><xs:element name='Inner' type='xs:string'/></xs:sequence></xs:complexType><xs:complexType name='Wild'><xs:anyAttribute processContents='lax'/></xs:complexType><xs:complexType name='Asset'><xs:sequence><xs:element name='Sound' type='Weighted' minOccurs='0' maxOccurs='2'/><xs:element name='PitchShift' type='Range' minOccurs='0'/><xs:element name='Nested' type='Nested' minOccurs='0'/><xs:element name='Wild' type='Wild' minOccurs='0'/></xs:sequence><xs:attribute name='id' type='xs:string' use='required'/><xs:attribute name='inheritFrom' type='xs:string'/></xs:complexType><xs:element name='AssetDeclaration'><xs:complexType><xs:sequence><xs:element name='Asset' type='Asset' maxOccurs='unbounded'/></xs:sequence></xs:complexType></xs:element></xs:schema>";
        var schemas = SdkEffectiveSchema.Compile(new Dictionary<string,byte[]> { ["entry.xsd"] = Encoding.UTF8.GetBytes(xsd) },"entry.xsd");
        const string children = "<Sound Weight='50'>local.bin</Sound><Sound>other.bin</Sound><PitchShift Low='-1' High='1'/>";
        byte[] raw = Source("<Asset id='Base'>"+children+"</Asset><Asset id='Child' inheritFrom='Base'/>");
        foreach (var input in new[] { raw,Source("<Asset id='Base'/><Asset id='Child' inheritFrom='Base'>"+children+"</Asset>") })
        {
            var result = SdkSelfAttributeInheritance.Apply(schemas.Schemas,input,complexChildCopy:true);
            Require(result.Bytes != null && result.Evidence.Profile == SdkSelfAttributeInheritance.ComplexChildCopyName && result.Evidence.Overlays.Length == 1,"Complex leaf copy rejected.");
            XmlDocument xml = new(); xml.LoadXml(Encoding.UTF8.GetString(result.Bytes!));
            var copied = xml.DocumentElement!.ChildNodes.OfType<XmlElement>().Last().ChildNodes.OfType<XmlElement>().ToArray();
            Require(copied.Select(element => element.LocalName).SequenceEqual(new[] { "Sound","Sound","PitchShift" }) && copied[0].InnerText == "local.bin" && copied[0].GetAttribute("Weight") == "50" && copied[1].InnerText == "other.bin" && !copied[1].HasAttribute("Weight") && copied[2].GetAttribute("Low") == "-1","Complex leaf text/order/attributes/default absence differs.");
            Require(SdkEffectiveSchema.Bind(schemas.Schemas,"fixture.xml",result.Bytes!).XmlValidated,"Complex copied document did not validate.");
        }
        Require(SdkSelfAttributeInheritance.Apply(schemas.Schemas,raw,true).Bytes == null,"Flat profile silently gained complex children.");
        // Reborn: complex leaves use core-compatible formatting normalization and the same pre-merge aggregate child budget.
        var formatted = SdkSelfAttributeInheritance.Apply(schemas.Schemas,Source("\n<Asset id='Base'>\n"+children+"\n</Asset>\n<Asset id='Child' inheritFrom='Base'/>\n"),complexChildCopy:true);
        Require(formatted.Bytes != null && SdkEffectiveSchema.Bind(schemas.Schemas,"fixture.xml",formatted.Bytes).XmlValidated,"Formatted complex leaves failed.");
        string amplified = "<Asset id='Base'><Sound>"+new string('x',70000)+"</Sound></Asset>"+string.Concat(Enumerable.Range(0,70).Select(index => "<Asset id='D"+index+"' inheritFrom='Base'/>"));
        Require(SdkSelfAttributeInheritance.Apply(schemas.Schemas,Source(amplified),complexChildCopy:true).Bytes == null,"Complex leaf amplification bound bypassed.");
        foreach (string invalid in new[] {
            "<Sound Weight='=1+2'>local.bin</Sound>","<Sound Unknown='x'>local.bin</Sound>","<Sound id='x'>local.bin</Sound>",
            "<Sound Flags='+A'>local.bin</Sound>","<Sound xmlns:i='uri:ea.com:eala:asset:instance' i:joinAction='Remove'>local.bin</Sound>",
            "<Sound> <!--split-->=1+2</Sound>","<Sound><![CDATA[local.bin]]></Sound>","<Sound><Inner/></Sound>",
            "<PitchShift Low='-1' High='1'>text</PitchShift>","<Nested><Inner>A</Inner></Nested>","<Wild Anything='x'/>",
            "<Sound>A</Sound><Sound>B</Sound><Sound>C</Sound>" })
        {
            var rejected = SdkSelfAttributeInheritance.Apply(schemas.Schemas,Source("<Asset id='Base'>"+invalid+"</Asset><Asset id='Child' inheritFrom='Base'/>"),complexChildCopy:true);
            Require(rejected.Bytes == null && rejected.Evidence.ProcessedSha256 == null && rejected.Evidence.Overlays.Length == 0,"Complex scope rejection published partial evidence.");
        }
        Require(SdkSelfAttributeInheritance.Apply(schemas.Schemas,Source("<Asset id='Base'>"+children+"</Asset><Asset id='Child' inheritFrom='Base'><PitchShift Low='0' High='1'/></Asset>"),complexChildCopy:true).Bytes == null,"Disjoint populated sides were merged.");
        // Reborn: declared invalid numeric/missing-required attributes are rejected by final schema binding, never trusted as physical fields.
        var invalidValue = SdkSelfAttributeInheritance.Apply(schemas.Schemas,Source("<Asset id='Base'><Sound Weight='bad'>local.bin</Sound><PitchShift Low='0'/></Asset><Asset id='Child' inheritFrom='Base'/>"),complexChildCopy:true);
        Require(invalidValue.Bytes != null && !SdkEffectiveSchema.Bind(schemas.Schemas,"fixture.xml",invalidValue.Bytes).XmlValidated,"Schema constraints were bypassed.");
        string root = Path.Combine(Path.GetTempPath(),"Reborn-ComplexChildCopy-"+Guid.NewGuid().ToString("N")),data = Path.Combine(root,"Data"); Directory.CreateDirectory(data);
        string entry = Path.Combine(data,"Entry.xml"); File.WriteAllBytes(entry,raw); File.WriteAllBytes(Path.Combine(data,"local.bin"),new byte[] { 7 }); File.WriteAllBytes(Path.Combine(data,"other.bin"),new byte[] { 8 });
        var paths = SdkSourcePathAudit.Inspect(SdkEnvironmentPreflight.Inspect("ra3ep1",SdkEnvironmentPreflight.BaselineSchemaRoot(),data,entry,Path.Combine(root,"NoOutput"),Array.Empty<string>()));
        var graph = SdkTypedSourceGraph.BindGraph(schemas.Schemas,paths,selfComplexChildCopy:true);
        Require(graph.ScopedGraphComplete && graph.Documents.Single().Dependencies.Length == 4 && graph.PreprocessingProfile == SdkSelfAttributeInheritance.ComplexChildCopyName && !graph.ProductionBuildReady && File.ReadAllBytes(entry).SequenceEqual(raw) && !Directory.Exists(Path.Combine(root,"NoOutput")),"Complex graph dependency/isolation differs.");
        Require(SdkTypedSourceGraph.BindGraph(schemas.Schemas,paths,selfChildCopy:true).Documents.Single().Status == "RequiresPreprocessing","Old graph copy profile changed.");
        bool conflict = false;
        try { SdkTypedSourceGraph.BindGraph(schemas.Schemas,paths,selfChildCopy:true,selfComplexChildCopy:true); } catch (ArgumentException) { conflict = true; }
        Require(conflict,"Conflicting graph profiles accepted.");
        // Reborn: invalid leaf values may be structurally copyable but must never produce trusted dependency fields.
        File.WriteAllBytes(entry,Source("<Asset id='Base'><Sound Weight='bad'>local.bin</Sound></Asset><Asset id='Child' inheritFrom='Base'/>"));
        var invalidPaths = SdkSourcePathAudit.Inspect(SdkEnvironmentPreflight.Inspect("ra3ep1",SdkEnvironmentPreflight.BaselineSchemaRoot(),data,entry,Path.Combine(root,"NoOutput"),Array.Empty<string>()));
        var invalidGraph = SdkTypedSourceGraph.BindGraph(schemas.Schemas,invalidPaths,selfComplexChildCopy:true);
        Require(invalidGraph.Documents.Single().Status == "SchemaInvalid" && invalidGraph.Documents.Single().Dependencies.Length == 0 && !invalidGraph.ScopedGraphComplete,"Invalid complex value published trusted fields.");
        Console.WriteLine("SDK self complex child copy self-test: OK (simpleContent/empty leaves, text/attributes/order/default absence, one-sided copies, atomic nested/wildcard/id/directive/expression/list/CDATA/occurrence/both-sided rejection, schema constraints and typed resource isolation; no recursive merge)");
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: encode only owned complex leaf fixtures, without modifying official reference XML. */
    //-------------------------------------------------------------------------------------------------
    private static byte[] Source(string body) => Encoding.UTF8.GetBytes("<AssetDeclaration xmlns='uri:ea.com:eala:asset'>"+body+"</AssetDeclaration>");

    //-------------------------------------------------------------------------------------------------
    /** Reborn: fail explicitly when a bounded complex leaf proof diverges from its admission contract. */
    //-------------------------------------------------------------------------------------------------
    private static void Require(bool condition,string message) { if (!condition) throw new InvalidDataException(message); }
}
