using System.Text;
using System.Xml;
using System.Xml.Schema;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: common existing value capabilities belong to every GameObject route, not a historical fixed-shape admission branch.
internal static class ConscriptSharedSourceContract
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: validate the same supported values before either fixed or variable structural matching; defaults are materialized only in a comparison document. */
    //-------------------------------------------------------------------------------------------------
    internal static void Validate(XmlDocument source,XmlDocument baseline,XmlSchemaSet schemas)
    {
        GameObjectScalarContract.Validate(source);GameObjectRootAttributeContract.Validate(source);GameObjectStrongReferenceContract.Validate(source,baseline);
        GameObjectRootStringContract.Validate(source,baseline);_=GameObjectRootTextContract.Read(source);
        ConscriptModuleScalarContract.Validate(source);ConscriptModuleIdentityContract.Validate(source);
        var qualified=SdkXmlDiagnostics.Parse(Encoding.UTF8.GetBytes(source.DocumentElement!.OuterXml));qualified.Schemas.Add(schemas);qualified.Validate((_,e)=>throw SdkXmlDiagnostics.SchemaError(e));
        foreach(var field in ConscriptEditableScalars.Fields)ConscriptEditableScalars.Read(qualified,field);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: mask only capabilities already supported by both native paths; structural lists and pointed-text presence remain owned by their variable contracts. */
    //-------------------------------------------------------------------------------------------------
    internal static void Mask(XmlDocument candidate,XmlDocument model)
    {
        foreach(var field in ConscriptEditableScalars.Fields)ConscriptEditableScalars.Element(candidate,field).SetAttribute(field.Attribute,ConscriptEditableScalars.Literal(model,field));
        GameObjectScalarContract.Mask(candidate,model);GameObjectRootAttributeContract.Mask(candidate,model);GameObjectStrongReferenceContract.Mask(candidate,model);GameObjectRootStringContract.Mask(candidate,model);
        ConscriptModuleIdentityContract.Mask(candidate,model);ConscriptModuleScalarContract.Mask(candidate,model);
    }
}
