using System.Text;
using System.Xml;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: prove effective inherited/group/xsi:type file binding on valid in-memory schemas while preserving real staged compilation failure evidence.
internal static class SdkEffectiveSchemaSmokeTest
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: separate real duplicate-schema rejection from synthetic positive inherited attributes, element types and source validation tests. */
    //-------------------------------------------------------------------------------------------------
    internal static void Run()
    {
        var staged = SdkEffectiveSchema.Inspect();
        Require(!staged.SchemaCompiled && staged.IncludedFiles == 837 && staged.EffectiveFileAttributes.Length == 0
            && staged.Errors.Any(error => error.Contains("ShieldSphereUpdateModuleData",StringComparison.Ordinal))
            && staged.ReadOnly && staged.SnapshotOnly && !staged.FullDependencyCoverage && !staged.ProductionBuildReady,"Real duplicate schema must remain a closed gate.");
        const string basis = "<xs:simpleType name='FileReference'><xs:restriction base='xs:anyURI'/></xs:simpleType><xs:simpleType name='DataBlob'><xs:restriction base='FileReference'/></xs:simpleType><xs:attributeGroup name='Files'><xs:attribute name='File' type='DataBlob'/></xs:attributeGroup><xs:complexType name='Base'><xs:sequence><xs:element name='Payload' type='DataBlob' minOccurs='0'/></xs:sequence><xs:attributeGroup ref='Files'/></xs:complexType><xs:complexType name='Derived'><xs:complexContent><xs:extension base='Base'><xs:attribute name='Header' type='FileReference'/></xs:extension></xs:complexContent></xs:complexType><xs:complexType name='Restricted'><xs:complexContent><xs:restriction base='Base'><xs:sequence><xs:element name='Payload' type='DataBlob' minOccurs='0'/></xs:sequence><xs:attribute name='File' use='prohibited'/></xs:restriction></xs:complexContent></xs:complexType>";
        const string entry = "<xs:include schemaLocation='sub/base.xsd'/><xs:element name='AssetDeclaration'><xs:complexType><xs:sequence><xs:element name='Asset' type='Base' maxOccurs='unbounded'/></xs:sequence></xs:complexType></xs:element>";
        Dictionary<string,byte[]> snapshots = new(StringComparer.OrdinalIgnoreCase) { ["entry.xsd"] = Xsd(entry),["sub/base.xsd"] = Xsd(basis) };
        var compiled = SdkEffectiveSchema.Compile(snapshots,"entry.xsd");
        Require(compiled.Compiled && compiled.IncludedFiles == 2,"Valid captured Include schema failed.");
        var attributes = SdkEffectiveSchema.Attributes(compiled.Schemas);
        Require(attributes.Count(field => field.OwnerType.EndsWith(":Derived",StringComparison.Ordinal)) == 2
            && attributes.All(field => !field.OwnerType.EndsWith(":Restricted",StringComparison.Ordinal)),"Inherited/group/prohibited attributes differ.");
        const string source = "<AssetDeclaration xmlns='uri:ea.com:eala:asset' xmlns:xsi='http://www.w3.org/2001/XMLSchema-instance' xsi:schemaLocation='uri:ea.com:eala:asset https://forbidden.invalid/schema.xsd'><Asset xsi:type='Derived' File='relative.apt' Header='AUDIO:header.h'><Payload>relative.dat</Payload></Asset></AssetDeclaration>";
        var bound = SdkEffectiveSchema.Bind(compiled.Schemas,"memory-fixture.xml",Encoding.UTF8.GetBytes(source));
        Require(bound.XmlValidated && bound.Fields.Length == 3 && bound.Fields.Any(field => field.Name == "File" && field.OwnerType.EndsWith(":Derived",StringComparison.Ordinal))
            && bound.Fields.Any(field => field.Kind == "element" && field.LogicalPath == "relative.dat"),"xsi:type/element/inherited source bindings differ.");
        var invalid = SdkEffectiveSchema.Bind(compiled.Schemas,"invalid.xml",Encoding.UTF8.GetBytes(source.Replace("xsi:type='Derived'","xsi:type='Unknown'")));
        Require(!invalid.XmlValidated && invalid.Fields.Length == 0 && invalid.Errors.Length > 0,"Invalid source returned trusted partial bindings.");
        snapshots["entry.xsd"] = Xsd("<xs:include schemaLocation='https://forbidden.invalid/schema.xsd'/>"); Reject(() => SdkEffectiveSchema.Compile(snapshots,"entry.xsd"));
        snapshots["entry.xsd"] = Xsd("<xs:include schemaLocation='../outside.xsd'/>"); Reject(() => SdkEffectiveSchema.Compile(snapshots,"entry.xsd"));
        snapshots["entry.xsd"] = Xsd("<xs:import namespace='urn:outside' schemaLocation='sub/base.xsd'/>"); Reject(() => SdkEffectiveSchema.Compile(snapshots,"entry.xsd"));
        Reject(() => SdkEffectiveSchema.Bind(compiled.Schemas,"oversized.xml",new byte[4*1048576+1]));
        Reject(() => SdkEffectiveSchema.Bind(compiled.Schemas,"dtd.xml",Encoding.UTF8.GetBytes("<!DOCTYPE x [<!ENTITY e SYSTEM 'file:///forbidden'>]><AssetDeclaration xmlns='uri:ea.com:eala:asset'>&e;</AssetDeclaration>")));
        Console.WriteLine("SDK effective-schema self-test: OK (real 837-XSD closure duplicate rejection; synthetic Include/inheritance/group/prohibited/xsi:type/element binding; URI/escape/import/DTD/size rejection; not production readiness)");
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: keep positive schema fixtures entirely in memory; external/reference XSD remains unchanged. */
    //-------------------------------------------------------------------------------------------------
    private static byte[] Xsd(string body) => Encoding.UTF8.GetBytes("<xs:schema xmlns:xs='http://www.w3.org/2001/XMLSchema' xmlns='uri:ea.com:eala:asset' targetNamespace='uri:ea.com:eala:asset' elementFormDefault='qualified'>"+body+"</xs:schema>");

    //-------------------------------------------------------------------------------------------------
    /** Reborn: forbidden schema authority/source parsing must reject before external resolution or payload processing. */
    //-------------------------------------------------------------------------------------------------
    private static void Reject(Action action)
    { try { action(); } catch (Exception error) when (error is InvalidDataException or XmlException or ArgumentException) { return; } throw new InvalidDataException("Unsafe effective-schema input accepted."); }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: distinguish successful synthetic binding from a real schema gate that must remain incomplete. */
    //-------------------------------------------------------------------------------------------------
    private static void Require(bool condition,string message) { if (!condition) throw new InvalidDataException(message); }
}
