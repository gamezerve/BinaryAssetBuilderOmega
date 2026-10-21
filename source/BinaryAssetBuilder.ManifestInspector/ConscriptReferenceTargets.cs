using System.Xml;
using System.Xml.Schema;
using BinaryAssetBuilder.Core;
using BinaryAssetBuilder.Core.Hashing;
using BinaryAssetBuilder.Core.SageXml;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: derive authored target identities independently of compiler-assigned indices; keep native slot topology anchored to the stock graph.
internal static class ConscriptReferenceTargets
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: anchor source traversal to every captured stock occurrence before accepting a changed target table. */
    //-------------------------------------------------------------------------------------------------
    internal static AssetId[] Expected(XmlDocument source,XmlDocument baseline,SchemaSet schemas,ManifestAsset[] catalog,IReadOnlyList<AssetId> original)
    {
        if(!Read(baseline,schemas,catalog).SequenceEqual(original))throw new InvalidDataException("Strong-reference source traversal differs from captured stock occurrences.");
        var authored=Read(source,schemas,catalog);
        if(authored.Length!=GameObjectStrongReferenceContract.ExpectedCount(source))throw new InvalidDataException("Authored reference presence changes require a different native slot layout.");
        return authored;
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: schema annotations and the explicit stock catalog determine concrete IDs; no normalization result or authored numeric suffix is trusted as expected evidence. */
    //-------------------------------------------------------------------------------------------------
    internal static AssetId[] Read(XmlDocument source,SchemaSet schemas,ManifestAsset[] catalog)
    {
        var document=new XmlDocument{XmlResolver=null};document.LoadXml(source.DocumentElement!.OuterXml);document.Schemas.Add(schemas.Schemas);document.Validate((_,e)=>throw new InvalidDataException(e.Message));
        ConscriptBehaviorSurvey.ResolveStrong(document,schemas,catalog);
        // Reborn: index the immutable catalog once per read; checking every occurrence must not rescan tens of thousands of stock assets.
        var targetCounts=catalog.GroupBy(a=>new AssetId(a.TypeId,a.InstanceId)).ToDictionary(g=>g.Key,g=>g.Count());
        var result=new List<AssetId>();
        foreach(XmlElement element in document.GetElementsByTagName("*",SchemaSet.XmlNamespace))
        {
            Add(element);
            foreach(XmlAttribute attribute in element.Attributes)Add(attribute);
        }
        return result.ToArray();

        //-------------------------------------------------------------------------------------------------
        /** Reborn: enforce concrete type compatibility even for explicitly qualified names, and reject missing/ambiguous target identities. */
        //-------------------------------------------------------------------------------------------------
        void Add(XmlNode node)
        {
            var type=node.SchemaInfo?.SchemaType;
            if(type==null||!XmlSchemaType.IsDerivedFrom(type,schemas.XmlAssetReferenceType,XmlSchemaDerivationMethod.None))return;
            string value=(node is XmlAttribute attribute?attribute.Value:node.InnerText).Trim();if(value.Length==0)return;
            if(value.Contains('\\'))throw new InvalidDataException("Authored reference compiler index refused.");
            string? required=Annotation(node is XmlAttribute?node.SchemaInfo?.SchemaAttribute?.UnhandledAttributes:node.SchemaInfo?.SchemaElement?.UnhandledAttributes);
            for(XmlSchemaType? current=type;required==null&&current!=null;){required=Annotation(current.UnhandledAttributes);var next=current.BaseXmlSchemaType;if(ReferenceEquals(next,current))break;current=next;}
            int colon=value.IndexOf(':');if(required==null||colon<=0||colon==value.Length-1)throw new InvalidDataException("Concrete strong-reference type missing: "+value);
            var concrete=schemas.GetXmlType(value[..colon]);var constraint=schemas.GetXmlType(required);
            if(concrete==null||constraint==null||!XmlSchemaType.IsDerivedFrom(concrete,constraint,XmlSchemaDerivationMethod.None))throw new InvalidDataException("Strong-reference target type mismatch: "+value+"; expected "+required);
            var id=new AssetId(FastHash.GetHashCode(value[..colon]),InstanceHandle.GetInstanceId(value[(colon+1)..]));
            if(targetCounts.GetValueOrDefault(id)!=1)throw new InvalidDataException("Strong-reference target missing/ambiguous: "+value);
            result.Add(id);
        }
    }

    // Reborn: target types come only from official asset-schema annotations, never field-name guesses.
    private static string? Annotation(XmlAttribute[]? attributes)=>attributes?.FirstOrDefault(a=>a.LocalName=="refType"&&a.NamespaceURI=="uri:ea.com:eala:asset:schema")?.Value;
}
