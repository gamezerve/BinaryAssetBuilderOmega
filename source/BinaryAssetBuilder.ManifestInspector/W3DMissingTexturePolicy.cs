using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;
using BinaryAssetBuilder.Core;
using BinaryAssetBuilder.Core.SageXml;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: a missing optional shader binding uses the shader default, without inventing a texture or leaving an unresolved import.
internal static class W3DMissingTexturePolicy
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: only W3DMesh/FXShader/Constants/Texture/Value is an optional material assignment, not every Texture reference. */
    //-------------------------------------------------------------------------------------------------
    internal static bool IsOptionalBinding(XObject node)
    {
        var current=node as XElement;
        foreach(string name in new[]{"Value","Texture","Constants","FXShader","W3DMesh"}){
            if(current?.Name!=XName.Get(name,SchemaSet.XmlNamespace))return false;
            current=current.Parent;
        }
        return true;
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: run before Core assigns import indices; keep present, ambiguous and wrongly typed references for normal resolution/errors. */
    //-------------------------------------------------------------------------------------------------
    internal static string[] OmitMissingBindings(XmlDocument document,SchemaSet schemas,ManifestAsset[] catalog)
    {
        if(document.DocumentElement?.LocalName!="W3DMesh")return Array.Empty<string>();
        var ns=new XmlNamespaceManager(document.NameTable);ns.AddNamespace("ea",SchemaSet.XmlNamespace);
        var texture=schemas.GetXmlType("Texture")??throw new InvalidDataException("Texture schema missing.");var omitted=new List<string>();
        foreach(XmlElement binding in document.SelectNodes("/ea:W3DMesh/ea:FXShader/ea:Constants/ea:Texture",ns)!.Cast<XmlElement>().ToArray()){
            var value=binding.ChildNodes.OfType<XmlElement>().Single(e=>e.LocalName=="Value"&&e.NamespaceURI==SchemaSet.XmlNamespace);string name=value.InnerText.Trim();
            if(name.Length==0||name.StartsWith('=')||name.Contains('\\'))continue;
            string selector="Texture";int colon=name.IndexOf(':');if(colon>=0){selector=name[..colon];name=name[(colon+1)..];}
            var selected=schemas.GetXmlType(selector);if(selected==null||!XmlSchemaType.IsDerivedFrom(selected,texture,XmlSchemaDerivationMethod.None))continue;
            uint id=new InstanceHandle(name).InstanceId;
            if(catalog.Any(a=>a.InstanceId==id&&schemas.GetXmlType(a.TypeName) is XmlSchemaType actual&&XmlSchemaType.IsDerivedFrom(actual,selected,XmlSchemaDerivationMethod.None)))continue;
            // Reborn: preflight emits the source-located MISSINGASSET warning; removing this whole optional constant preserves all other material values.
            omitted.Add(selector+":"+name);binding.ParentNode!.RemoveChild(binding);
        }
        return omitted.ToArray();
    }
}
