using System.Xml;
using System.Xml.Schema;
using BinaryAssetBuilder.Core.SageXml;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: coalesce only identical direct StrategicState repeats after proving unchanged-core behavior and literal schema shape.
internal static class SdkIdenticalStates
{
    // Reborn: owner-local witnesses retain multiplicity and literal fields, not native reference or game identity authority.
    internal sealed record Witness(string OwnerId,string ChildId,int Before,int After,string[] Attributes);
    private const string Ea = "uri:ea.com:eala:asset";

    //-------------------------------------------------------------------------------------------------
    /** Reborn: predict identical empty-complex keyed folding, verify the actual core result and remove only redundant later copies in memory. */
    //-------------------------------------------------------------------------------------------------
    internal static Witness[] Normalize(XmlSchemaSet schemas,XmlElement asset)
    {
        if (asset.LocalName != "AIPersonalityDefinition") return Array.Empty<Witness>();
        var groups = asset.ChildNodes.OfType<XmlElement>().Where(child => child.HasAttribute("id"))
            .GroupBy(child => child.GetAttribute("id"),StringComparer.Ordinal).Where(group => group.Count() > 1).ToArray();
        if (groups.Length == 0) return Array.Empty<Witness>();
        if (asset.NamespaceURI != Ea || asset.Prefix.Length != 0
            || schemas.GlobalTypes[new XmlQualifiedName("AIPersonalityDefinition",Ea)] is not XmlSchemaComplexType owner
            || owner.ContentTypeParticle is not XmlSchemaSequence sequence
            || sequence.Items.OfType<XmlSchemaElement>().SingleOrDefault(item => item.QualifiedName == new XmlQualifiedName("StrategicState",Ea)) is not XmlSchemaElement declaration
            || declaration.MinOccurs != 0 || declaration.MaxOccurs != decimal.MaxValue
            || declaration.ElementSchemaType is not XmlSchemaComplexType type || type.QualifiedName != new XmlQualifiedName("AIStrategicState",Ea)
            || type.ContentType != XmlSchemaContentType.Empty || type.AttributeWildcard != null)
            throw new InvalidDataException("Exact repeated empty AIStrategicState sequence required for identical-state folding.");
        List<Witness> witnesses = new();
        foreach (var group in groups)
        {
            var nodes = group.ToArray();
            if (witnesses.Count >= 128 || nodes.Length > 16 || !Token(group.Key)) throw new InvalidDataException("Identical-state group/key bound exceeded.");
            string[]? expected = null;
            foreach (var node in nodes)
            {
                if (node.LocalName != "StrategicState" || node.NamespaceURI != Ea || node.Prefix.Length != 0 || node.HasChildNodes)
                    throw new InvalidDataException("Only identical empty same-QName StrategicState siblings may coalesce.");
                foreach (XmlAttribute attribute in node.Attributes)
                {
                    if (attribute.NamespaceURI == "http://www.w3.org/2000/xmlns/") continue;
                    if (attribute.NamespaceURI.Length != 0 || attribute.Name is not ("id" or "State" or "Difficulty")
                        || type.AttributeUses[new XmlQualifiedName(attribute.Name)] is not XmlSchemaAttribute use
                        || attribute.Value.Length is < 1 or > 4096 || attribute.Value.StartsWith('=')
                        || attribute.Name is "id" or "State" && !Token(attribute.Value)
                        || use.AttributeSchemaType?.Datatype?.Variety == XmlSchemaDatatypeVariety.List && (attribute.Value.Contains('+') || attribute.Value.Contains('-')))
                        throw new InvalidDataException("Identical-state fields require reviewed literal schema attributes; directives/modifiers remain closed.");
                }
                if (!node.HasAttribute("State")) throw new InvalidDataException("Explicit identical-state reference required.");
                var fields = Attributes(node);
                if (expected != null && !fields.SequenceEqual(expected,StringComparer.Ordinal)) throw new InvalidDataException("Conflicting or complementary repeated states remain closed.");
                expected = fields;
            }
            var xml = asset.OwnerDocument!;
            // Reborn: a bounded isolated owner probe preserves original owner fields for the later whole-owner gate, never hiding them in normalized XML.
            var empty = xml.CreateElement("AIPersonalityDefinition",Ea); empty.SetAttribute("id","RebornProbe");
            var repeated = (XmlElement)empty.CloneNode(false);
            foreach (var node in nodes) repeated.AppendChild(node.CloneNode(true));
            var merged = (XmlElement)NodeJoiner.Override(schemas,xml,empty,repeated);
            var actual = merged.ChildNodes.OfType<XmlElement>().ToArray();
            if (actual.Length != 1 || actual[0].LocalName != "StrategicState" || actual[0].NamespaceURI != Ea
                || actual[0].HasChildNodes || !Attributes(actual[0]).SequenceEqual(expected!,StringComparer.Ordinal))
                throw new InvalidDataException("Core identical-state result differs from predicted literal fields.");
            // Reborn: core retains the first same-key position; removing only later identical siblings preserves all unrelated nodes and metadata.
            foreach (var redundant in nodes.Skip(1)) asset.RemoveChild(redundant);
            witnesses.Add(new(asset.GetAttribute("id"),group.Key,nodes.Length,1,expected!));
        }
        return witnesses.ToArray();
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: compare explicit effective field values independent of XML attribute order and namespace declaration placement. */
    //-------------------------------------------------------------------------------------------------
    private static string[] Attributes(XmlElement node) => node.Attributes.OfType<XmlAttribute>().Where(attribute => attribute.NamespaceURI != "http://www.w3.org/2000/xmlns/")
        .OrderBy(attribute => attribute.Name,StringComparer.Ordinal).Select(attribute => attribute.Name+"="+attribute.Value).ToArray();

    //-------------------------------------------------------------------------------------------------
    /** Reborn: retain the earlier bounded literal handle alphabet rather than inventing duplicate identity semantics. */
    //-------------------------------------------------------------------------------------------------
    private static bool Token(string value) => value.Length is > 0 and <= 128 && value.All(character => char.IsAsciiLetterOrDigit(character) || character is '_' or '-' or '.');
}
