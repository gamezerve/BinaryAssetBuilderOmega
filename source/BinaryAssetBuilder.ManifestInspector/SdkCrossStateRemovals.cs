using System.Xml;
using System.Xml.Schema;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: admit only reviewed empty BuildState Remove commands against unique resolved StrategicState identities, never cross-QName replacement.
internal static class SdkCrossStateRemovals
{
    private const string Ea = "uri:ea.com:eala:asset",Instance = "uri:ea.com:eala:asset:instance",Annotation = "uri:ea.com:eala:asset:schema";
    // Reborn: retain command and matched target QNames separately from native reference/type compatibility.
    internal sealed record Witness(string OwnerId,string BaseId,string ChildId,string CommandName,string TargetName,string StateReference,string CommandReferenceType,string TargetReferenceType);
    internal sealed record Plan(XmlElement[] Commands,string[] Expected,Witness[] Witnesses);

    //-------------------------------------------------------------------------------------------------
    /** Reborn: prove one reviewed cross-QName removal and predict all direct StrategicState/BuildState fields and order before core allocation. */
    //-------------------------------------------------------------------------------------------------
    internal static Plan? Prove(XmlSchemaSet schemas,XmlElement basis,XmlElement owner)
    {
        var before = basis.ChildNodes.OfType<XmlElement>().ToArray();
        var after = owner.ChildNodes.OfType<XmlElement>().ToArray();
        var commands = after.Where(node => node.HasAttribute("id") && before.Any(old => old.GetAttribute("id") == node.GetAttribute("id") && old.LocalName != node.LocalName)).ToArray();
        if (commands.Length == 0) return null;
        if (commands.Length != 1 || owner.LocalName != "AIPersonalityDefinition" || owner.NamespaceURI != Ea
            || schemas.GlobalTypes[new XmlQualifiedName(owner.LocalName,Ea)] is not XmlSchemaComplexType type || type.ContentTypeParticle is not XmlSchemaSequence sequence)
            throw new InvalidDataException("Only one reviewed AI cross-QName removal per owner is admitted.");
        string buildRef = ReferenceType(sequence,"BuildState","AIBuildState","AIBuildStateDefinition");
        string strategicRef = ReferenceType(sequence,"StrategicState","AIStrategicState","AIStrategicStateDefinition");
        var command = commands[0]; string id = command.GetAttribute("id");
        var matches = before.Where(node => node.GetAttribute("id") == id).ToArray();
        if (matches.Length != 1 || command.LocalName != "BuildState" || command.NamespaceURI != Ea || command.Prefix.Length != 0 || command.HasChildNodes
            || command.GetAttribute("joinAction",Instance) != "Remove" || !Token(id)
            || command.Attributes.OfType<XmlAttribute>().Any(field => field.NamespaceURI != "http://www.w3.org/2000/xmlns/"
                && !(field.NamespaceURI.Length == 0 && field.Name == "id") && !(field.NamespaceURI == Instance && field.LocalName == "joinAction" && field.Value == "Remove")))
            throw new InvalidDataException("Reviewed cross-QName command must be an empty literal BuildState Remove stub.");
        var matched = matches[0];
        if (matched.LocalName != "StrategicState" || matched.NamespaceURI != Ea || matched.HasChildNodes || !Token(matched.GetAttribute("State")))
            throw new InvalidDataException("Cross-QName removal requires a unique resolved literal StrategicState target.");
        var ranks = sequence.Items.OfType<XmlSchemaElement>().Select((element,index) => (element.Name,index)).ToDictionary(pair => pair.Name!,pair => pair.index,StringComparer.Ordinal);
        List<XmlElement> current = before.Where(State).Select(node => (XmlElement)node.CloneNode(true)).ToList();
        foreach (var node in after.Where(State))
        {
            var old = node.HasAttribute("id") ? current.SingleOrDefault(candidate => candidate.GetAttribute("id") == node.GetAttribute("id")) : null;
            if (node.GetAttribute("joinAction",Instance) == "Remove")
            {
                if (old == null) throw new InvalidDataException("Missing state removal target cannot authorize cross-QName prediction.");
                current.Remove(old);
            }
            else if (old != null)
            {
                if (old.LocalName != node.LocalName) throw new InvalidDataException("Cross-QName state replacement remains closed.");
                foreach (XmlAttribute field in node.Attributes) if (field.NamespaceURI.Length == 0) old.SetAttribute(field.Name,field.Value);
            }
            else
            {
                int index = current.FindIndex(candidate => ranks[candidate.LocalName] > ranks[node.LocalName]);
                current.Insert(index < 0 ? current.Count : index,(XmlElement)node.CloneNode(true));
            }
        }
        return new(commands,current.Select(Projection).ToArray(),new[] { new Witness(owner.GetAttribute("id"),basis.GetAttribute("id"),id,command.LocalName,matched.LocalName,matched.GetAttribute("State"),buildRef,strategicRef) });
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: require exact repeated empty named state types with distinct schema-authored AssetReference annotations. */
    //-------------------------------------------------------------------------------------------------
    private static string ReferenceType(XmlSchemaSequence sequence,string child,string typeName,string expected)
    {
        var element = sequence.Items.OfType<XmlSchemaElement>().SingleOrDefault(item => item.QualifiedName == new XmlQualifiedName(child,Ea));
        if (element?.MinOccurs != 0 || element.MaxOccurs != decimal.MaxValue || element.ElementSchemaType is not XmlSchemaComplexType type
            || type.QualifiedName != new XmlQualifiedName(typeName,Ea) || type.ContentType != XmlSchemaContentType.Empty || type.AttributeWildcard != null
            || type.AttributeUses[new XmlQualifiedName("State")] is not XmlSchemaAttribute attribute || attribute.Use != XmlSchemaUse.Required
            || attribute.AttributeSchemaType?.QualifiedName != new XmlQualifiedName("AssetReference",Ea)
            || attribute.UnhandledAttributes?.Count(field => field.NamespaceURI == Annotation && field.LocalName == "refType" && field.Value == expected) != 1)
            throw new InvalidDataException("Exact reviewed state schema/reference types required for cross-QName removal.");
        return expected;
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: withhold removal authority unless the actual core preserves every predicted state field/order and deletes the intended target. */
    //-------------------------------------------------------------------------------------------------
    internal static void Verify(XmlElement merged,Plan plan)
    {
        if (!merged.ChildNodes.OfType<XmlElement>().Where(State).Select(Projection).SequenceEqual(plan.Expected,StringComparer.Ordinal)
            || plan.Witnesses.Any(witness => merged.ChildNodes.OfType<XmlElement>().Any(node => node.GetAttribute("id") == witness.ChildId)))
            throw new InvalidDataException("Core cross-QName removal differs from predicted state fields/order.");
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: project only the two reviewed reference-bearing state branches. */
    //-------------------------------------------------------------------------------------------------
    private static bool State(XmlElement node) => node.LocalName is "StrategicState" or "BuildState";

    //-------------------------------------------------------------------------------------------------
    /** Reborn: length-prefix QName and explicit fields to preserve value/presence equality without ambiguous separators. */
    //-------------------------------------------------------------------------------------------------
    private static string Projection(XmlElement node) => node.LocalName+":"+string.Concat(node.Attributes.OfType<XmlAttribute>().Where(field => field.NamespaceURI.Length == 0).OrderBy(field => field.Name,StringComparer.Ordinal).Select(field => field.Name.Length+":"+field.Name+field.Value.Length+":"+field.Value));

    //-------------------------------------------------------------------------------------------------
    /** Reborn: keep literal keyed commands within the earlier bounded handle alphabet. */
    //-------------------------------------------------------------------------------------------------
    private static bool Token(string value) => value.Length is > 0 and <= 128 && value.All(character => char.IsAsciiLetterOrDigit(character) || character is '_' or '-' or '.');
}
