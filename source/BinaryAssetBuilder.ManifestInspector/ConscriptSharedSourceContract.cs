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
        // Reborn: nonmodule scalar codecs share admission across all routes; structural changes continue through the variable graph path.
        GameObjectNestedValueContract.Validate(source);
        GameObjectResourceContract.Validate(source);
        var qualified=SdkXmlDiagnostics.Parse(Encoding.UTF8.GetBytes(source.DocumentElement!.OuterXml));qualified.Schemas.Add(schemas);qualified.Validate((_,e)=>throw SdkXmlDiagnostics.SchemaError(e));
        // Reborn: PropBody has no MaxHealth; type-specific legacy receipts cannot require ActiveBody storage.
        // Reborn: the historical single Speed receipt is not an admission requirement for source-sized movement lists.
        foreach(var field in ConscriptEditableScalars.Fields)if(ConscriptEditableScalars.Applies(qualified,field)&&field.Attribute!="Amount"&&(field.Attribute!="Speed"||ConscriptLocomotorSetContract.Entries(qualified).Length==1))ConscriptEditableScalars.Read(qualified,field);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: mask only capabilities already supported by both native paths; structural lists and pointed-text presence remain owned by their variable contracts. */
    //-------------------------------------------------------------------------------------------------
    internal static void Mask(XmlDocument candidate,XmlDocument model)
    {
        foreach(var field in ConscriptEditableScalars.Fields)if(ConscriptEditableScalars.Applies(candidate,field)&&(field.Attribute!="Amount"||GameObjectResourceContract.Entries(candidate).Length==1)&&(field.Attribute!="Speed"||ConscriptLocomotorSetContract.Entries(candidate).Length==1))ConscriptEditableScalars.Element(candidate,field).SetAttribute(field.Attribute,ConscriptEditableScalars.Literal(model,field));
        GameObjectScalarContract.Mask(candidate,model);GameObjectRootAttributeContract.Mask(candidate,model);GameObjectStrongReferenceContract.Mask(candidate,model);GameObjectRootStringContract.Mask(candidate,model);
        ConscriptModuleIdentityContract.Mask(candidate,model);ConscriptModuleScalarContract.Mask(candidate,model);
    }
}
