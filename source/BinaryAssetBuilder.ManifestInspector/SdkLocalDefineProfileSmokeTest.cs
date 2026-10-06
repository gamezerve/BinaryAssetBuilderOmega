using System.Security.Cryptography;
using System.Text;
using System.Xml;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: local expression diagnostics must be atomic, bounded, source-preserving and opt-in before schema/resource binding.
internal static class SdkLocalDefineProfileSmokeTest
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: prove literal attribute/text replacement and reject imported, chained, duplicate, override and general expressions without partial bytes. */
    //-------------------------------------------------------------------------------------------------
    internal static void Run()
    {
        byte[] raw = Source("<Defines><Define name='FILE' value='local.bin'/><Define name='TEXT' value='A&amp;B'/></Defines><Asset File='=$FILE'>=$TEXT</Asset>");
        byte[] saved = raw.ToArray(); var result = SdkLocalDefineProfile.Apply(raw);
        Require(result.Bytes != null && result.Evidence.Substitutions == 2 && raw.SequenceEqual(saved)
            && result.Evidence.RawSha256 == Convert.ToHexString(SHA256.HashData(raw)) && result.Evidence.ProcessedSha256 != result.Evidence.RawSha256,"Source mutation or identity/substitution mismatch.");
        XmlDocument xml = new(); xml.LoadXml(Encoding.UTF8.GetString(result.Bytes!));
        XmlElement asset = xml.DocumentElement!.ChildNodes.OfType<XmlElement>().Last();
        Require(asset.GetAttribute("File") == "local.bin" && asset.InnerText == "A&B","Escaped literal attribute/text substitution differs.");
        foreach (string body in new[] {
            "<Defines><Define name='FILE' value='x'/></Defines><Asset File='=$file'/>",
            "<Defines><Define name='FILE' value='x'/></Defines><Asset File='=$FILE + 1'/>",
            "<Defines><Define name='FILE' value='x'/></Defines><Asset File='=$FILE'/><Asset File='=$MISSING'/>",
            "<Defines><Define name='FILE' value='=$OTHER'/></Defines><Asset File='=$FILE'/>",
            "<Defines><Define name='FILE' value='x'/><Define name='FILE' value='y'/></Defines><Asset File='=$FILE'/>",
            "<Defines><Define name='FILE' value='x' override='true'/></Defines><Asset File='=$FILE'/>",
            "<Defines><Define name='FILE' value='x'/></Defines><Defines/><Asset File='=$FILE'/>",
            "<Includes><Include type='all' source='missing.xml'/></Includes><Defines><Define name='FILE' value='x'/></Defines><Asset File='=$FILE'/>",
            "<Defines><Define name='FILE' value='x'/></Defines><Asset inheritFrom='Base' File='=$FILE'/>",
            "<Asset File='=1+2'/>" })
        {
            var rejected = SdkLocalDefineProfile.Apply(Source(body));
            Require(rejected.Bytes == null && rejected.Evidence.ProcessedSha256 == null && rejected.Evidence.Substitutions == 0 && rejected.Evidence.Diagnostics.Length == 1,"Rejected preprocessing exposed partial output.");
        }
        byte[] noop = Source("<Includes><Include source='not-opened.xml'/></Includes><Asset File='literal.bin'/>");
        var unchanged = SdkLocalDefineProfile.Apply(noop);
        Require(ReferenceEquals(noop,unchanged.Bytes) && unchanged.Evidence.RawSha256 == unchanged.Evidence.ProcessedSha256,"No-expression profile rewrote source.");
        Reject(() => SdkLocalDefineProfile.Apply(Encoding.UTF8.GetBytes("<!DOCTYPE x [<!ENTITY e SYSTEM 'file:///not-opened'>]><AssetDeclaration xmlns='uri:ea.com:eala:asset'>&e;</AssetDeclaration>")));
        Reject(() => SdkLocalDefineProfile.Apply(new byte[4*1048576+1]));
        Reject(() => SdkLocalDefineProfile.Apply(Source("<Asset>"+new string(' ',1)+string.Concat(Enumerable.Repeat("<X>",130))+string.Concat(Enumerable.Repeat("</X>",130))+"</Asset>")));
        Reject(() => SdkLocalDefineProfile.Apply(Source(string.Concat(Enumerable.Repeat("<Asset File='=$FILE'/>",2049)))));
        // Reborn: graph integration must keep raw default binding, processed source hashes and unsafe literal resource rejection distinct.
        string root = Path.Combine(Path.GetTempPath(),"Reborn-LocalDefines-"+Guid.NewGuid().ToString("N")),data = Path.Combine(root,"Data"); Directory.CreateDirectory(data);
        string entry = Path.Combine(data,"Entry.xml"),blob = Path.Combine(data,"local.bin"); File.WriteAllBytes(entry,raw); File.WriteAllBytes(blob,new byte[] { 7 });
        const string xsd = "<xs:schema xmlns:xs='http://www.w3.org/2001/XMLSchema' xmlns='uri:ea.com:eala:asset' targetNamespace='uri:ea.com:eala:asset' elementFormDefault='qualified'><xs:simpleType name='FileReference'><xs:restriction base='xs:anyURI'/></xs:simpleType><xs:element name='AssetDeclaration'><xs:complexType><xs:sequence><xs:element name='Defines' type='xs:anyType' minOccurs='0'/><xs:element name='Asset' maxOccurs='unbounded'><xs:complexType mixed='true'><xs:attribute name='File' type='FileReference' use='required'/></xs:complexType></xs:element></xs:sequence></xs:complexType></xs:element></xs:schema>";
        var schemas = SdkEffectiveSchema.Compile(new Dictionary<string,byte[]> { ["entry.xsd"] = Encoding.UTF8.GetBytes(xsd) },"entry.xsd");
        var environment = SdkEnvironmentPreflight.Inspect("ra3ep1",SdkEnvironmentPreflight.BaselineSchemaRoot(),data,entry,Path.Combine(root,"NoOutput"),Array.Empty<string>());
        var paths = SdkSourcePathAudit.Inspect(environment);
        var defaultGraph = SdkTypedSourceGraph.BindGraph(schemas.Schemas,paths);
        Require(!defaultGraph.ScopedGraphComplete && defaultGraph.Documents.Single().Preprocessing == null && defaultGraph.Documents.Single().Dependencies.Single().Issue == "ResourcePath","Raw graph default silently enabled expressions.");
        var graph = SdkTypedSourceGraph.BindGraph(schemas.Schemas,paths,true); var document = graph.Documents.Single();
        Require(graph.ScopedGraphComplete && document.Status == "Validated" && document.Preprocessing?.Substitutions == 2 && document.ExpectedSha256 == result.Evidence.RawSha256
            && document.Preprocessing.ProcessedSha256 == result.Evidence.ProcessedSha256 && document.Dependencies.Single().Bytes == 1
            && File.ReadAllBytes(entry).SequenceEqual(saved) && !Directory.Exists(environment.OutputDirectory) && !graph.ProductionBuildReady,"Opt-in typed graph identity/resource/source isolation differs.");
        File.WriteAllBytes(entry,Source("<Defines><Define name='FILE' value='../outside.bin'/></Defines><Asset File='=$FILE'/>") );
        Require(SdkTypedSourceGraph.BindGraph(schemas.Schemas,paths,true).Documents.Single().Status == "StaleSource","Preprocessing bypassed raw snapshot recheck.");
        environment = SdkEnvironmentPreflight.Inspect("ra3ep1",SdkEnvironmentPreflight.BaselineSchemaRoot(),data,entry,Path.Combine(root,"NoOutput"),Array.Empty<string>());
        var unsafeGraph = SdkTypedSourceGraph.BindGraph(schemas.Schemas,SdkSourcePathAudit.Inspect(environment),true);
        Require(!unsafeGraph.ScopedGraphComplete && unsafeGraph.Documents.Single().Dependencies.Single().Issue == "ResourcePath","Substituted literal escaped resource root.");
        Console.WriteLine("SDK local define profile self-test: OK (literal attribute/text, identities, atomic rejection, Include/override/chain/case/arithmetic/DTD/depth/count/size gates, raw default, stale source and resource confinement; no production evaluator)");
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: generate owned EA document bytes with no file access or reference mutation. */
    //-------------------------------------------------------------------------------------------------
    private static byte[] Source(string body) => Encoding.UTF8.GetBytes("<AssetDeclaration xmlns='uri:ea.com:eala:asset'>"+body+"</AssetDeclaration>");

    //-------------------------------------------------------------------------------------------------
    /** Reborn: security/size/depth bounds must fail closed before any transformed binding is returned. */
    //-------------------------------------------------------------------------------------------------
    private static void Reject(Action action)
    { try { action(); } catch (Exception error) when (error is InvalidDataException or XmlException) { return; } throw new InvalidDataException("Local define bound accepted invalid input."); }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: surface profile regressions without equating diagnostic XML binding to working Uprising assets. */
    //-------------------------------------------------------------------------------------------------
    private static void Require(bool condition,string message) { if (!condition) throw new InvalidDataException(message); }
}
