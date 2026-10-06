using System.Text;
using System.Xml;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: prove the local leaf overlay subset preserves attributes/identity and rejects richer core inheritance semantics atomically.
internal static class SdkSelfAttributeInheritanceSmokeTest
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: exercise direct/chained leaf inheritance, all-document rejection gates and graph/source/resource isolation with owned fixtures. */
    //-------------------------------------------------------------------------------------------------
    internal static void Run()
    {
        const string xsd = "<xs:schema xmlns:xs='http://www.w3.org/2001/XMLSchema' xmlns='uri:ea.com:eala:asset' targetNamespace='uri:ea.com:eala:asset' elementFormDefault='qualified'><xs:simpleType name='FileReference'><xs:restriction base='xs:anyURI'/></xs:simpleType><xs:simpleType name='Flags'><xs:list itemType='xs:string'/></xs:simpleType><xs:complexType name='Asset'><xs:attribute name='id' type='xs:string' use='required'/><xs:attribute name='inheritFrom' type='xs:string'/><xs:attribute name='File' type='FileReference' use='required'/><xs:attribute name='Amount' type='xs:int'/><xs:attribute name='Flags' type='Flags'/></xs:complexType><xs:element name='AssetDeclaration'><xs:complexType><xs:sequence><xs:element name='Asset' type='Asset' maxOccurs='unbounded'/></xs:sequence></xs:complexType></xs:element></xs:schema>";
        var schemas = SdkEffectiveSchema.Compile(new Dictionary<string,byte[]> { ["entry.xsd"] = Encoding.UTF8.GetBytes(xsd) },"entry.xsd");
        const string body = "<Asset id='Base' File='local.bin' Amount='1' Flags='A B'/><Asset id='Child' inheritFrom='Base' Amount='2'/><Asset id='Grand' inheritFrom='Asset:Child'/>";
        byte[] raw = Source(body); var result = SdkSelfAttributeInheritance.Apply(schemas.Schemas,raw);
        Require(result.Bytes != null && result.Evidence.Overlays.Length == 2 && result.Evidence.RawSha256 != result.Evidence.ProcessedSha256 && raw.SequenceEqual(Source(body)),"Local overlay identity/source preservation differs.");
        XmlDocument xml = new(); xml.LoadXml(Encoding.UTF8.GetString(result.Bytes!));
        var grand = xml.DocumentElement!.ChildNodes.OfType<XmlElement>().Single(element => element.GetAttribute("id") == "Grand");
        Require(grand.GetAttribute("File") == "local.bin" && grand.GetAttribute("Amount") == "2" && grand.GetAttribute("Flags") == "A B" && grand.GetAttribute("inheritFrom") == "Asset:Child","Core leaf attribute override/retained handle differs.");
        Require(SdkEffectiveSchema.Bind(schemas.Schemas,"fixture.xml",result.Bytes!).XmlValidated,"Expanded required attributes failed schema validation.");
        foreach (string invalid in new[] {
            "<Asset id='Child' inheritFrom='Missing'/>",
            "<Asset id='Child' inheritFrom='Child'/>",
            "<Asset id='A' inheritFrom='B'/><Asset id='B' inheritFrom='A'/>",
            body+"<Asset id='Base'/>",
            body.Replace("inheritFrom='Base'","inheritFrom='Other:Base'",StringComparison.Ordinal),
            body.Replace("Amount='2'","Amount='=$VALUE'",StringComparison.Ordinal),
            body.Replace("Amount='2'","Flags='+A'",StringComparison.Ordinal),
            body.Replace("Amount='2'","Unknown='bad'",StringComparison.Ordinal),
            body+"<Asset id='Complex' File='x'><Nested/></Asset>",
            body+"<Asset id='Text' File='x'>text</Asset>",
            body.Replace("Amount='2'","xmlns:i='uri:ea.com:eala:asset:instance' i:joinAction='Replace'",StringComparison.Ordinal),
            body.Replace("Amount='2'","xmlns:xsi='http://www.w3.org/2001/XMLSchema-instance' xsi:type='Asset'",StringComparison.Ordinal),
            body+"<Asset id='Other' File='=$UNPROCESSED'/>" })
        {
            var rejected = SdkSelfAttributeInheritance.Apply(schemas.Schemas,Source(invalid));
            Require(rejected.Bytes == null && rejected.Evidence.ProcessedSha256 == null && rejected.Evidence.Overlays.Length == 0,"Rejected inheritance exposed partial transformed bytes/evidence.");
        }
        string deep = "<Asset id='A0' File='local.bin'/>"+string.Concat(Enumerable.Range(1,34).Select(index => "<Asset id='A"+index+"' inheritFrom='A"+(index-1)+"'/>"));
        Require(SdkSelfAttributeInheritance.Apply(schemas.Schemas,Source(deep)).Bytes == null,"Memoized declaration order bypassed the semantic chain cap.");
        // Reborn: resolve the deepest asset first to exercise active-chain depth rather than memoized declaration order.
        deep = string.Concat(Enumerable.Range(1,34).Reverse().Select(index => "<Asset id='A"+index+"' inheritFrom='A"+(index-1)+"'/>"))+"<Asset id='A0' File='local.bin'/>";
        Require(SdkSelfAttributeInheritance.Apply(schemas.Schemas,Source(deep)).Bytes == null,"Local inheritance depth cap bypassed.");
        // Reborn: many small derived assets cannot amplify one large base attribute beyond the admitted output budget before serialization.
        string amplified = "<Asset id='Base' File='"+new string('x',70000)+"'/>"+string.Concat(Enumerable.Range(0,70).Select(index => "<Asset id='D"+index+"' inheritFrom='Base'/>"));
        Require(SdkSelfAttributeInheritance.Apply(schemas.Schemas,Source(amplified)).Bytes == null,"Inherited attribute amplification bound bypassed.");
        string root = Path.Combine(Path.GetTempPath(),"Reborn-SelfInheritance-"+Guid.NewGuid().ToString("N")),data = Path.Combine(root,"Data"); Directory.CreateDirectory(data);
        string entry = Path.Combine(data,"Entry.xml"); File.WriteAllBytes(entry,raw); File.WriteAllBytes(Path.Combine(data,"local.bin"),new byte[] { 1 });
        var paths = SdkSourcePathAudit.Inspect(SdkEnvironmentPreflight.Inspect("ra3ep1",SdkEnvironmentPreflight.BaselineSchemaRoot(),data,entry,Path.Combine(root,"NoOutput"),Array.Empty<string>()));
        Require(SdkTypedSourceGraph.BindGraph(schemas.Schemas,paths,definitionExpressions:true).Documents.Single().Status == "RequiresPreprocessing","Earlier definition profile silently enabled inheritance.");
        var graph = SdkTypedSourceGraph.BindGraph(schemas.Schemas,paths,selfAttributeInheritance:true);
        Require(graph.ScopedGraphComplete && graph.Documents.Single().Inheritance?.Overlays.Length == 2 && graph.Documents.Single().Dependencies.Length == 3
            && !graph.ProductionBuildReady && !Directory.Exists(Path.Combine(root,"NoOutput")) && File.ReadAllBytes(entry).SequenceEqual(raw),"Local overlay graph violated source/resource/production isolation.");
        File.WriteAllBytes(entry,Source(body.Replace("Amount='2'","Amount='3'",StringComparison.Ordinal)));
        Require(SdkTypedSourceGraph.BindGraph(schemas.Schemas,paths,selfAttributeInheritance:true).Documents.Single().Status == "StaleSource","Inheritance bypassed raw fingerprint recheck.");
        Console.WriteLine("SDK self attribute inheritance self-test: OK (core leaf attribute merge, local/qualified/chained identity, required fields, atomic missing/self/cycle/cross-type/child/text/directive/modifier/expression/unknown/depth rejection, raw/default/stale/resource isolation; no imported or complex overlays)");
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: encode only owned fixture bytes with no source or payload access. */
    //-------------------------------------------------------------------------------------------------
    private static byte[] Source(string body) => Encoding.UTF8.GetBytes("<AssetDeclaration xmlns='uri:ea.com:eala:asset'>"+body+"</AssetDeclaration>");

    //-------------------------------------------------------------------------------------------------
    /** Reborn: fail bounded overlay regressions without claiming compiled streams or game compatibility. */
    //-------------------------------------------------------------------------------------------------
    private static void Require(bool condition,string message) { if (!condition) throw new InvalidDataException(message); }
}
