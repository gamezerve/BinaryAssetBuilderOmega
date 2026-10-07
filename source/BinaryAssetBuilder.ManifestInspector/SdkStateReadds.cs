using System.Xml;
using System.Xml.Schema;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: prove ordered literal StrategicState Remove/re-add pairs without deleting commands or converting them to ordinary overlay.
internal static class SdkStateReadds
{
    // Reborn: retain original command node identities for the narrow duplicate-ID exception only.
    internal sealed record Pair(XmlElement Removal,XmlElement Replacement);
    // Reborn: positions are among direct StrategicState siblings, not native state indexes.
    internal sealed record Witness(string OwnerId,string BaseId,string ChildId,string[] Before,string[] After,int BeforePosition,int AfterPosition);
    internal sealed record Plan(string[] Expected,Witness[] Witnesses);
    private const string Ea = "uri:ea.com:eala:asset",Instance = "uri:ea.com:eala:asset:instance";

    //-------------------------------------------------------------------------------------------------
    /** Reborn: admit exactly two same-QName literal siblings, with a payload-free Remove first and an explicit replacement later. */
    //-------------------------------------------------------------------------------------------------
    internal static Pair[] Find(XmlSchemaSet schemas,XmlElement asset)
    {
        if (asset.LocalName != "AIPersonalityDefinition") return Array.Empty<Pair>();
        var groups = asset.ChildNodes.OfType<XmlElement>().Where(child => child.HasAttribute("id")).GroupBy(child => child.GetAttribute("id"),StringComparer.Ordinal)
            .Where(group => group.Count() > 1 && group.Any(child => child.GetAttribute("joinAction",Instance) == "Remove")).ToArray();
        if (groups.Length == 0) return Array.Empty<Pair>();
        if (groups.Length > 128 || asset.NamespaceURI != Ea || asset.Prefix.Length != 0 || !asset.HasAttribute("inheritFrom")
            || schemas.GlobalTypes[new XmlQualifiedName("AIPersonalityDefinition",Ea)] is not XmlSchemaComplexType owner
            || owner.ContentTypeParticle is not XmlSchemaSequence sequence
            || sequence.Items.OfType<XmlSchemaElement>().SingleOrDefault(item => item.QualifiedName == new XmlQualifiedName("StrategicState",Ea)) is not XmlSchemaElement declaration
            || declaration.MinOccurs != 0 || declaration.MaxOccurs != decimal.MaxValue
            || declaration.ElementSchemaType is not XmlSchemaComplexType type || type.QualifiedName != new XmlQualifiedName("AIStrategicState",Ea)
            || type.ContentType != XmlSchemaContentType.Empty || type.AttributeWildcard != null)
            throw new InvalidDataException("Reviewed inherited empty StrategicState command pairs required.");
        List<Pair> result = new();
        foreach (var group in groups)
        {
            var nodes = group.ToArray();
            if (nodes.Length != 2 || !Token(group.Key) || nodes.Any(node => node.LocalName != "StrategicState" || node.NamespaceURI != Ea || node.Prefix.Length != 0 || node.HasChildNodes)
                || nodes[0].GetAttribute("joinAction",Instance) != "Remove" || !nodes[1].HasAttribute("State"))
                throw new InvalidDataException("Exactly ordered same-QName Remove/re-add with explicit State required.");
            foreach (XmlAttribute attribute in nodes[0].Attributes)
                if (attribute.NamespaceURI != "http://www.w3.org/2000/xmlns/" && !(attribute.NamespaceURI.Length == 0 && attribute.Name == "id")
                    && !(attribute.NamespaceURI == Instance && attribute.LocalName == "joinAction" && attribute.Value == "Remove"))
                    throw new InvalidDataException("Remove stub must carry only its literal id and Remove directive.");
            foreach (XmlAttribute attribute in nodes[1].Attributes)
            {
                if (attribute.NamespaceURI == "http://www.w3.org/2000/xmlns/") continue;
                if (attribute.NamespaceURI.Length != 0 || attribute.Name is not ("id" or "State" or "Difficulty")
                    || type.AttributeUses[new XmlQualifiedName(attribute.Name)] is not XmlSchemaAttribute use
                    || attribute.Value.Length is < 1 or > 4096 || attribute.Value.StartsWith('=')
                    || attribute.Name is "id" or "State" && !Token(attribute.Value)
                    || use.AttributeSchemaType?.Datatype?.Variety == XmlSchemaDatatypeVariety.List && (attribute.Value.Contains('+') || attribute.Value.Contains('-')))
                    throw new InvalidDataException("Re-added state requires reviewed literal fields without directives/modifiers.");
            }
            result.Add(new(nodes[0],nodes[1]));
        }
        return result.ToArray();
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: predict the complete ordered direct-state projection against an existing unique exact-QName resolved base target before core allocation. */
    //-------------------------------------------------------------------------------------------------
    internal static Plan Prove(XmlElement basis,XmlElement owner,Pair[] pairs)
    {
        var original = basis.ChildNodes.OfType<XmlElement>().ToArray();
        var before = original.Where(node => node.LocalName == "StrategicState").ToArray();
        foreach (var pair in pairs)
            if (original.Count(node => node.GetAttribute("id") == pair.Removal.GetAttribute("id")) != 1
                || !before.Any(node => node.GetAttribute("id") == pair.Removal.GetAttribute("id")))
                throw new InvalidDataException("State re-add requires an existing unique same-QName resolved base target.");
        List<XmlElement> current = before.Select(node => (XmlElement)node.CloneNode(true)).ToList();
        foreach (var node in owner.ChildNodes.OfType<XmlElement>().Where(node => node.LocalName == "StrategicState"))
        {
            var matched = node.HasAttribute("id") ? current.SingleOrDefault(old => old.GetAttribute("id") == node.GetAttribute("id")) : null;
            if (node.GetAttribute("joinAction",Instance) == "Remove")
            {
                if (matched == null) throw new InvalidDataException("Missing state removal target cannot authorize re-add prediction.");
                current.Remove(matched);
            }
            else if (matched == null) current.Add((XmlElement)node.CloneNode(true));
            else
                foreach (XmlAttribute field in node.Attributes)
                    if (field.NamespaceURI.Length == 0) matched.SetAttribute(field.Name,field.Value);
        }
        return new(current.Select(Projection).ToArray(),pairs.Select(pair => {
            string id = pair.Removal.GetAttribute("id");
            int oldIndex = Array.FindIndex(before,node => node.GetAttribute("id") == id),newIndex = current.FindIndex(node => node.GetAttribute("id") == id);
            return new Witness(owner.GetAttribute("id"),basis.GetAttribute("id"),id,Fields(before[oldIndex]),Fields(current[newIndex]),oldIndex,newIndex);
        }).ToArray());
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: trust no ordered command evidence until the actual unchanged core result matches every predicted state field and position. */
    //-------------------------------------------------------------------------------------------------
    internal static void Verify(XmlElement merged,Plan plan)
    {
        if (!merged.ChildNodes.OfType<XmlElement>().Where(node => node.LocalName == "StrategicState").Select(Projection).SequenceEqual(plan.Expected,StringComparer.Ordinal))
            throw new InvalidDataException("Core state Remove/re-add differs from predicted ordered literal projection.");
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: preserve explicit field presence and values for old-only field loss evidence. */
    //-------------------------------------------------------------------------------------------------
    private static string[] Fields(XmlElement node) => node.Attributes.OfType<XmlAttribute>().Where(field => field.NamespaceURI.Length == 0).OrderBy(field => field.Name,StringComparer.Ordinal).Select(field => field.Name+"="+field.Value).ToArray();

    //-------------------------------------------------------------------------------------------------
    /** Reborn: length-prefix fields to avoid ambiguous projection equality when literal values contain separators. */
    //-------------------------------------------------------------------------------------------------
    private static string Projection(XmlElement node) => string.Concat(Fields(node).Select(field => field.Length+":"+field));

    //-------------------------------------------------------------------------------------------------
    /** Reborn: retain the existing bounded literal handle alphabet for keyed state commands. */
    //-------------------------------------------------------------------------------------------------
    private static bool Token(string value) => value.Length is > 0 and <= 128 && value.All(character => char.IsAsciiLetterOrDigit(character) || character is '_' or '-' or '.');
}
