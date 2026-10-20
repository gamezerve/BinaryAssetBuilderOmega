using System.Xml;
using System.Xml.Schema;
using BinaryAssetBuilder.Core;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: bare EA source references and qualified captured references can name the same asset; compare identity without enabling non-root retargeting.
internal static class ConscriptReferenceSpellings
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: normalize comparison copies only; actual authored names still pass independent Core/catalog/import resolution during compilation. */
    //-------------------------------------------------------------------------------------------------
    internal static void Compare(XmlDocument candidate,XmlDocument model,XmlSchemaSet schemas)
    {
        foreach(var document in new[]{candidate,model}){document.Schemas.Add(schemas);document.Validate((_,e)=>throw new InvalidDataException(e.Message));}
        var strong=schemas.GlobalTypes[new XmlQualifiedName("AssetReference","uri:ea.com:eala:asset")] as XmlSchemaType;
        Visit(candidate.DocumentElement!,model.DocumentElement!);

        //-------------------------------------------------------------------------------------------------
        /** Reborn: structural differences remain visible; never align different children or reinterpret ordinary string fields as asset IDs. */
        //-------------------------------------------------------------------------------------------------
        void Visit(XmlElement actual,XmlElement expected)
        {
            if(actual.LocalName!=expected.LocalName||actual.NamespaceURI!=expected.NamespaceURI)return;
            foreach(XmlAttribute attribute in actual.Attributes)
                if(expected.GetAttributeNode(attribute.LocalName,attribute.NamespaceURI) is XmlAttribute baseline)Canonicalize(attribute,baseline);
            var children=actual.ChildNodes.OfType<XmlElement>().ToArray();var originals=expected.ChildNodes.OfType<XmlElement>().ToArray();
            if(children.Length!=originals.Length)return;
            if(children.Length==0)Canonicalize(actual,expected);
            for(int index=0;index<children.Length;index++)Visit(children[index],originals[index]);
        }

        //-------------------------------------------------------------------------------------------------
        /** Reborn: require matching hashed instance identity and compatible declared/concrete schema types before ignoring a spelling difference. */
        //-------------------------------------------------------------------------------------------------
        void Canonicalize(XmlNode actual,XmlNode expected)
        {
            var type=actual.SchemaInfo?.SchemaType;if(type==null||strong==null||!XmlSchemaType.IsDerivedFrom(type,strong,XmlSchemaDerivationMethod.None))return;
            string a=actual is XmlAttribute aa?aa.Value:actual.InnerText,b=expected is XmlAttribute bb?bb.Value:expected.InnerText;
            if(a==b||a.Length==0||b.Length==0||a.Contains('\\')||b.Contains('\\'))return;
            string? required=Annotation(actual.SchemaInfo?.SchemaAttribute?.UnhandledAttributes)??Annotation(actual.SchemaInfo?.SchemaElement?.UnhandledAttributes);
            for(XmlSchemaType? current=type;required==null&&current!=null;){required=Annotation(current.UnhandledAttributes);var next=current.BaseXmlSchemaType;if(ReferenceEquals(next,current))break;current=next;}
            if(required==null)return;
            int ac=a.IndexOf(':'),bc=b.IndexOf(':');string an=ac<0?a:a[(ac+1)..],bn=bc<0?b:b[(bc+1)..];
            if(InstanceHandle.GetInstanceId(an)!=InstanceHandle.GetInstanceId(bn))return;
            var selected=schemas.GlobalTypes[new XmlQualifiedName(ac<0?required:a[..ac],"uri:ea.com:eala:asset")] as XmlSchemaType;
            var concrete=schemas.GlobalTypes[new XmlQualifiedName(bc<0?required:b[..bc],"uri:ea.com:eala:asset")] as XmlSchemaType;
            var constraint=schemas.GlobalTypes[new XmlQualifiedName(required,"uri:ea.com:eala:asset")] as XmlSchemaType;
            if(selected==null||concrete==null||constraint==null||!XmlSchemaType.IsDerivedFrom(concrete,selected,XmlSchemaDerivationMethod.None)||!XmlSchemaType.IsDerivedFrom(concrete,constraint,XmlSchemaDerivationMethod.None))return;
            if(actual is XmlAttribute attribute)attribute.Value=b;else actual.InnerText=b;
        }
    }

    // Reborn: only official EA schema annotations supply reference target types.
    private static string? Annotation(XmlAttribute[]? attributes)=>attributes?.FirstOrDefault(a=>a.LocalName=="refType"&&a.NamespaceURI=="uri:ea.com:eala:asset:schema")?.Value;
}
