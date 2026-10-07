using System.Text;
using System.Xml;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: prove nonconflicting flat child copying without admitting actual child matching, instance directives or imported bases.
internal static class SdkSelfChildCopySmokeTest
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: test both one-sided directions, repeated simple children, negative scope gates and rechecked typed graph isolation. */
    //-------------------------------------------------------------------------------------------------
    internal static void Run()
    {
        const string xsd = "<xs:schema xmlns:xs='http://www.w3.org/2001/XMLSchema' xmlns='uri:ea.com:eala:asset' targetNamespace='uri:ea.com:eala:asset' elementFormDefault='qualified'><xs:simpleType name='FileReference'><xs:restriction base='xs:anyURI'/></xs:simpleType><xs:complexType name='Asset'><xs:sequence><xs:element name='Filename' type='FileReference' minOccurs='0'/><xs:element name='Tag' type='xs:string' minOccurs='0' maxOccurs='2'/></xs:sequence><xs:attribute name='id' type='xs:string' use='required'/><xs:attribute name='inheritFrom' type='xs:string'/></xs:complexType><xs:element name='AssetDeclaration'><xs:complexType><xs:sequence><xs:element name='Asset' type='Asset' maxOccurs='unbounded'/></xs:sequence></xs:complexType></xs:element></xs:schema>";
        var schemas = SdkEffectiveSchema.Compile(new Dictionary<string,byte[]> { ["entry.xsd"] = Encoding.UTF8.GetBytes(xsd) },"entry.xsd");
        const string body = "<Asset id='Base'><Filename>local.bin</Filename><Tag>A</Tag><Tag>B</Tag></Asset><Asset id='Child' inheritFrom='Base'/>";
        byte[] raw = Source(body); var result = SdkSelfAttributeInheritance.Apply(schemas.Schemas,raw,true);
        Require(result.Bytes != null && result.Evidence.Profile == SdkSelfAttributeInheritance.ChildCopyName && result.Evidence.Overlays.Length == 1
            && result.Evidence.ProcessedSha256 != result.Evidence.RawSha256,"Child-copy evidence differs.");
        XmlDocument xml = new(); xml.LoadXml(Encoding.UTF8.GetString(result.Bytes!));
        var child = xml.DocumentElement!.ChildNodes.OfType<XmlElement>().Last();
        Require(child.ChildNodes.OfType<XmlElement>().Select(element => element.InnerText).SequenceEqual(new[] { "local.bin","A","B" })
            && SdkEffectiveSchema.Bind(schemas.Schemas,"fixture.xml",result.Bytes!).XmlValidated,"Inherited simple child order/values differ.");
        var reverse = SdkSelfAttributeInheritance.Apply(schemas.Schemas,Source("<Asset id='Base'/><Asset id='Child' inheritFrom='Base'><Filename>local.bin</Filename></Asset>"),true);
        Require(reverse.Bytes != null && SdkEffectiveSchema.Bind(schemas.Schemas,"fixture.xml",reverse.Bytes).Fields.Single().LogicalPath == "local.bin","Derived-only child copy failed.");
        Require(SdkSelfAttributeInheritance.Apply(schemas.Schemas,raw).Bytes == null,"Original leaf profile silently gained child support.");
        // Reborn: original formatted source remains untouched while the child-copy parser drops only formatting nodes, not simple child text values.
        var formatted = SdkSelfAttributeInheritance.Apply(schemas.Schemas,Source(body.Replace("><",">\n\t<",StringComparison.Ordinal)),true);
        Require(formatted.Bytes != null && SdkEffectiveSchema.Bind(schemas.Schemas,"fixture.xml",formatted.Bytes).Fields.Length == 2,"Formatting whitespace was treated as a core sequence child.");
        foreach (string invalid in new[] {
            body.Replace("inheritFrom='Base'/>","inheritFrom='Base'><Tag>C</Tag></Asset>",StringComparison.Ordinal),
            body.Replace("<Tag>A</Tag>","<Tag><Nested/></Tag>",StringComparison.Ordinal),
            body.Replace("<Tag>A</Tag>","<Tag id='A'>A</Tag>",StringComparison.Ordinal),
            body.Replace("<Tag>A</Tag>","<Tag>=1+2</Tag>",StringComparison.Ordinal),
            body.Replace("<Tag>A</Tag>","<Tag> <!--split-->=1+2</Tag>",StringComparison.Ordinal),
            body.Replace("<Tag>A</Tag>","<Tag><![CDATA[A]]></Tag>",StringComparison.Ordinal),
            body.Replace("<Tag>A</Tag>","<Tag>A</Tag><Tag>C</Tag>",StringComparison.Ordinal),
            body.Replace("<Tag>A</Tag>","<Unknown>A</Unknown>",StringComparison.Ordinal),
            body.Replace("<Tag>A</Tag>","<Tag xmlns:i='uri:ea.com:eala:asset:instance' i:joinAction='Remove'>A</Tag>",StringComparison.Ordinal) })
        {
            var rejected = SdkSelfAttributeInheritance.Apply(schemas.Schemas,Source(invalid),true);
            Require(rejected.Bytes == null && rejected.Evidence.ProcessedSha256 == null && rejected.Evidence.Overlays.Length == 0,"Rejected child copy published partial evidence.");
        }
        string amplified = "<Asset id='Base'><Tag>"+new string('x',70000)+"</Tag></Asset>"+string.Concat(Enumerable.Range(0,70).Select(index => "<Asset id='D"+index+"' inheritFrom='Base'/>"));
        Require(SdkSelfAttributeInheritance.Apply(schemas.Schemas,Source(amplified),true).Bytes == null,"Inherited child amplification bound bypassed.");
        string root = Path.Combine(Path.GetTempPath(),"Reborn-ChildCopy-"+Guid.NewGuid().ToString("N")),data = Path.Combine(root,"Data"); Directory.CreateDirectory(data);
        string entry = Path.Combine(data,"Entry.xml"); File.WriteAllBytes(entry,raw); File.WriteAllBytes(Path.Combine(data,"local.bin"),new byte[] { 7 });
        var paths = SdkSourcePathAudit.Inspect(SdkEnvironmentPreflight.Inspect("ra3ep1",SdkEnvironmentPreflight.BaselineSchemaRoot(),data,entry,Path.Combine(root,"NoOutput"),Array.Empty<string>()));
        var graph = SdkTypedSourceGraph.BindGraph(schemas.Schemas,paths,selfChildCopy:true);
        Require(graph.ScopedGraphComplete && graph.PreprocessingProfile == SdkSelfAttributeInheritance.ChildCopyName && graph.Documents.Single().Dependencies.Length == 2
            && !graph.ProductionBuildReady && File.ReadAllBytes(entry).SequenceEqual(raw) && !Directory.Exists(Path.Combine(root,"NoOutput")),"Child graph violated resource/source/output isolation.");
        Require(SdkTypedSourceGraph.BindGraph(schemas.Schemas,paths,selfAttributeInheritance:true).Documents.Single().Status == "RequiresPreprocessing","Old graph leaf profile changed.");
        Console.WriteLine("SDK self child copy self-test: OK (base-only/derived-only simple children, order/repetition, both-sided/nested/attribute/expression/CDATA/unknown/occurrence/directive/amplification rejection, old profile and typed resource/source isolation; no actual child merge)");
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: encode only owned fixture XML without accessing reference sources. */
    //-------------------------------------------------------------------------------------------------
    private static byte[] Source(string body) => Encoding.UTF8.GetBytes("<AssetDeclaration xmlns='uri:ea.com:eala:asset'>"+body+"</AssetDeclaration>");

    //-------------------------------------------------------------------------------------------------
    /** Reborn: report regression failures without equating copy-only XML validity with production asset compatibility. */
    //-------------------------------------------------------------------------------------------------
    private static void Require(bool condition,string message) { if (!condition) throw new InvalidDataException(message); }
}
