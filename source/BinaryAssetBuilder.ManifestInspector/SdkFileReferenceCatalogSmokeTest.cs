using System.Text;
using System.Xml;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: prove derived file-field declarations use expanded type identities and never masquerade as complete source dependency binding.
internal static class SdkFileReferenceCatalogSmokeTest
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: check the staged declaration inventory, DataBlob/inline restrictions, namespace collisions and malformed/bounded schema evidence. */
    //-------------------------------------------------------------------------------------------------
    internal static void Run()
    {
        var report = SdkFileReferenceCatalog.Inspect();
        Require(report.SchemaFiles == 843 && report.SchemaCatalogSha256.Length == 64 && report.DeclaredFields.Length == 17 && report.ReadOnly && report.SnapshotOnly && !report.FullDependencyCoverage && !report.ProductionBuildReady,"Staged file-field inventory differs.");
        Require(report.DeclaredFields.Count(field => field.DeclaredType.EndsWith("}DataBlob",StringComparison.Ordinal)) == 5
            && report.DeclaredFields.Single(field => field.OwnerType == "OnDemandTexture" && field.Name == "File").PipelineOnly
            && report.DeclaredFields.Any(field => field.OwnerType == "PathMusicTrack" && field.Name == "PathfinderTrackHeader"),"Derived/pipeline/music field inventory differs.");
        const string basis = "<xs:simpleType name='FileReference'><xs:restriction base='xs:anyURI'/></xs:simpleType><xs:simpleType name='DataBlob'><xs:restriction base='FileReference'/></xs:simpleType>";
        const string fields = "<xs:complexType name='Example'><xs:attribute name='Direct' type='FileReference'/><xs:attribute name='Derived' type='DataBlob'/><xs:attribute name='Unrelated' type='other:FileReference'/><xs:attribute name='Plain' type='xs:string'/><xs:sequence><xs:element name='Inline'><xs:simpleType><xs:restriction base='DataBlob'/></xs:simpleType></xs:element></xs:sequence></xs:complexType>";
        // Reborn: QName lexical padding is valid and must retain the same namespace-aware ancestry.
        var selected = Collect(basis+fields.Replace("type='DataBlob'","type=' DataBlob '"));
        Require(selected.Length == 3 && selected.All(field => field.OwnerType == "Example") && selected.Single(field => field.Name == "Derived").DerivationChain.Length == 2
            && selected.Single(field => field.Name == "Inline").Kind == "element" && selected.Single(field => field.Name == "Inline").InlineRestriction,"Inline/derived/QName declaration selection differs.");
        Reject(() => Collect(basis+"<xs:complexType name='Bad'><xs:attribute name='File' type='missing:FileReference'/></xs:complexType>"));
        // Reborn: non-XML Unicode whitespace is not xs:QName lexical padding and must not be silently stripped.
        Reject(() => Collect(basis+"<xs:complexType name='Bad'><xs:attribute name='File' type='\u00a0DataBlob\u00a0'/></xs:complexType>"));
        Reject(() => Collect(basis+"<xs:simpleType name='Loop'><xs:restriction base='Loop'/></xs:simpleType><xs:complexType name='Bad'><xs:attribute name='File' type='Loop'/></xs:complexType>"));
        Reject(() => Collect(basis+"<xs:simpleType name='DataBlob'><xs:restriction base='xs:string'/></xs:simpleType>"));
        Reject(() => SdkFileReferenceCatalog.Collect(new[] { ("oversized.xsd",new byte[2*1048576+1]) }));
        Reject(() => SdkFileReferenceCatalog.Collect(new[] { ("dtd.xsd",Encoding.UTF8.GetBytes("<!DOCTYPE x [<!ENTITY e SYSTEM 'file:///forbidden'>]><xs:schema xmlns:xs='http://www.w3.org/2001/XMLSchema'>&e;</xs:schema>")) }));
        Console.WriteLine("SDK file-reference catalog self-test: OK (843 schemas/17 declared fields, five DataBlob fields, QName/inline/pipeline evidence, cycle/conflict/DTD/size rejection; not inherited instance binding or a build)");
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: keep synthetic declaration parsing wholly in memory without writing schema/reference files. */
    //-------------------------------------------------------------------------------------------------
    private static SdkFileReferenceCatalog.Field[] Collect(string body) => SdkFileReferenceCatalog.Collect(new[] { ("fixture.xsd",Encoding.UTF8.GetBytes("<xs:schema xmlns:xs='http://www.w3.org/2001/XMLSchema' xmlns='uri:ea.com:eala:asset' xmlns:other='urn:unrelated' targetNamespace='uri:ea.com:eala:asset'>"+body+"</xs:schema>")) });

    //-------------------------------------------------------------------------------------------------
    /** Reborn: invalid declaration evidence must fail rather than quietly selecting a similarly named type. */
    //-------------------------------------------------------------------------------------------------
    private static void Reject(Action action)
    { try { action(); } catch (Exception error) when (error is InvalidDataException or XmlException or ArgumentException) { return; } throw new InvalidDataException("Invalid file-field declaration evidence accepted."); }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: halt at the first mismatch with independently counted staged schema declarations. */
    //-------------------------------------------------------------------------------------------------
    private static void Require(bool condition,string message) { if (!condition) throw new InvalidDataException(message); }
}
