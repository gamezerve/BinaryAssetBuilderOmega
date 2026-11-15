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
        // Reborn: validating this private document only needs the already compiled schema set.
        var qualified=SdkXmlDiagnostics.Parse(Encoding.UTF8.GetBytes(source.DocumentElement!.OuterXml));qualified.Schemas=schemas;qualified.Validate((_,e)=>throw SdkXmlDiagnostics.SchemaError(e));
        ValidateModuleTypeOverrides(source,schemas);
        // Reborn: PropBody has no MaxHealth; type-specific legacy receipts cannot require ActiveBody storage.
        // Reborn: the historical single Speed receipt is not an admission requirement for source-sized movement lists.
        foreach(var field in ConscriptEditableScalars.Fields)if(ConscriptEditableScalars.Applies(qualified,field)&&field.Attribute!="Amount"&&(field.Attribute!="Speed"||ConscriptLocomotorSetContract.Entries(qualified).Length==1))ConscriptEditableScalars.Read(qualified,field);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: reject schema-valid type overrides that would otherwise fall back to the element's base writer. */
    //-------------------------------------------------------------------------------------------------
    private static void ValidateModuleTypeOverrides(XmlDocument source,XmlSchemaSet schemas)
    {
        const string xsi="http://www.w3.org/2001/XMLSchema-instance";
        var root=source.DocumentElement!;
        foreach(var container in root.ChildNodes.OfType<XmlElement>())
        {
            if(container.LocalName is not ("Behaviors" or "AI" or "Body" or "Draws" or "ClientUpdates" or "ClientBehaviors"))
                continue;
            foreach(var module in container.ChildNodes.OfType<XmlElement>())
            {
                var attribute=module.GetAttributeNode("type",xsi);
                if(attribute==null)
                    continue;
                var game=schemas.GlobalElements[new XmlQualifiedName(root.LocalName,root.NamespaceURI)] as XmlSchemaElement;
                var containerType=FindChild(game?.ElementSchemaType,container.LocalName)?.ElementSchemaType;
                var declaredType=FindChild(containerType,module.LocalName)?.ElementSchemaType?.QualifiedName;
                string text=attribute.Value.Trim();
                int colon=text.IndexOf(':');
                string prefix=colon<0?"":text[..colon];
                string localName=text[(colon+1)..];
                if(declaredType!=null &&
                    declaredType.Equals(new XmlQualifiedName(localName,module.GetNamespaceOfPrefix(prefix)??"")))
                    continue;
                throw SdkXmlDiagnostics.At(new InvalidDataException(
                    $"GameObject module xsi:type '{attribute.Value}' has no supported concrete writer for '{module.LocalName}'."),attribute);
            }
        }
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: read the original declaration from the schema particle, independently of xsi:type PSVI. */
    //-------------------------------------------------------------------------------------------------
    private static XmlSchemaElement? FindChild(XmlSchemaType? type,string name)
        => type is XmlSchemaComplexType complex ? FindChildParticle(complex.ContentTypeParticle,name) : null;

    //-------------------------------------------------------------------------------------------------
    /** Reborn: follow sequence/choice/group particles without traversing into a child element's own fields. */
    //-------------------------------------------------------------------------------------------------
    private static XmlSchemaElement? FindChildParticle(XmlSchemaParticle? particle,string name) => particle switch
    {
        XmlSchemaElement element when element.QualifiedName.Name==name => element,
        XmlSchemaGroupBase group => group.Items.OfType<XmlSchemaParticle>()
            .Select(child=>FindChildParticle(child,name)).FirstOrDefault(found=>found!=null),
        XmlSchemaGroupRef group => FindChildParticle(group.Particle,name),
        _ => null
    };

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
