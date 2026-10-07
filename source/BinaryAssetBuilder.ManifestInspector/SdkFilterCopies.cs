using System.Xml;
using System.Xml.Schema;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: prove one-sided anonymous weak-reference payload copying inside a matched ObjectFilter without enabling recursive populated merging.
internal static class SdkFilterCopies
{
    // Reborn: leaf evidence identifies literal diagnostic XML references, not resolved game objects or native hashes.
    internal sealed record Leaf(string Name,string Value);
    // Reborn: predict the unchanged joiner's one-sided payload source and ordered leaves before allocation.
    internal sealed record Plan(string PayloadSource,Leaf[] Expected);

    //-------------------------------------------------------------------------------------------------
    /** Reborn: require a singleton named ObjectFilter with flat optional repeated WeakReference leaves and at most one populated payload side. */
    //-------------------------------------------------------------------------------------------------
    internal static Plan Prove(XmlSchemaElement declaration,XmlElement before,XmlElement after)
    {
        const string ea = "uri:ea.com:eala:asset";
        if (declaration.MinOccurs != 0 || declaration.MaxOccurs != 1 || declaration.ElementSchemaType is not XmlSchemaComplexType type
            || type.QualifiedName != new XmlQualifiedName("ObjectFilter",ea) || type.ContentType != XmlSchemaContentType.ElementOnly
            || type.AttributeWildcard != null || type.ContentTypeParticle is not XmlSchemaSequence sequence
            || sequence.MinOccurs != 1 || sequence.MaxOccurs != 1 || sequence.Items.Count != 2
            || !sequence.Items.OfType<XmlSchemaElement>().Select(element => element.QualifiedName.Name).SequenceEqual(new[] { "IncludeThing","ExcludeThing" })
            || sequence.Items.OfType<XmlSchemaObject>().Any(item => item is not XmlSchemaElement element
                || element.QualifiedName.Namespace != ea || element.QualifiedName.Name is not ("IncludeThing" or "ExcludeThing")
                || element.MinOccurs != 0 || element.MaxOccurs <= 1 || element.ElementSchemaType is not XmlSchemaSimpleType simple
                || simple.QualifiedName != new XmlQualifiedName("WeakReference",ea)))
            throw new InvalidDataException("Reviewed singleton ObjectFilter with flat optional repeated WeakReference leaves required.");
        var baseLeaves = Leaves(before); var derivedLeaves = Leaves(after);
        if (baseLeaves.Length > 0 && derivedLeaves.Length > 0) throw new InvalidDataException("Two populated ObjectFilter payload sides remain closed.");
        return new(derivedLeaves.Length > 0 ? "derived" : baseLeaves.Length > 0 ? "base" : "none",derivedLeaves.Length > 0 ? derivedLeaves : baseLeaves);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: bound and record unprefixed, attribute-free literal weak-reference leaves before the joiner can allocate a merged branch. */
    //-------------------------------------------------------------------------------------------------
    internal static Leaf[] Leaves(XmlElement filter)
    {
        var elements = filter.ChildNodes.OfType<XmlElement>().ToArray();
        if (elements.Length > 128) throw new InvalidDataException("128-leaf ObjectFilter payload bound exceeded.");
        foreach (var element in elements)
            if (element.NamespaceURI != "uri:ea.com:eala:asset" || element.Prefix.Length != 0 || element.LocalName is not ("IncludeThing" or "ExcludeThing")
                || element.Attributes.OfType<XmlAttribute>().Any(attribute => attribute.NamespaceURI != "http://www.w3.org/2000/xmlns/")
                || element.ChildNodes.OfType<XmlElement>().Any() || element.InnerText.Length is < 1 or > 128
                || element.InnerText.Any(character => !char.IsAsciiLetterOrDigit(character) && character is not ('_' or '-' or '.')))
                throw new InvalidDataException("ObjectFilter payload requires bounded literal anonymous weak-reference leaves.");
        return elements.Select(element => new Leaf(element.LocalName,element.InnerText)).ToArray();
    }
}
