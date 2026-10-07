using System.Xml;
using System.Xml.Schema;
using BinaryAssetBuilder.Core.SageXml;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: fold only complementary anonymous upgrade singleton pairs through the unchanged core, not generic malformed XML.
internal static class SdkUpgradeNormalization
{
    // Reborn: source-owner witnesses distinguish normalization from inheritance and native dependency binding.
    internal sealed record Witness(string Id,string Child,int Before,int After,SdkFilterCopies.Leaf[] Leaves,string[] Attributes);

    //-------------------------------------------------------------------------------------------------
    /** Reborn: prove exact dependency particles, disjoint literal attributes and a single populated payload side before bounded core folding. */
    //-------------------------------------------------------------------------------------------------
    internal static Witness? Normalize(XmlSchemaSet schemas,XmlElement asset)
    {
        const string ea = "uri:ea.com:eala:asset";
        var branches = asset.ChildNodes.OfType<XmlElement>().Where(child => child.LocalName == "GameDependency").ToArray();
        if (asset.LocalName != "UpgradeTemplate" || branches.Length <= 1) return null;
        if (asset.NamespaceURI != ea || asset.Prefix.Length != 0 || !asset.HasAttribute("inheritFrom") || branches.Length != 2
            || asset.ChildNodes.OfType<XmlElement>().Count() != 2
            || schemas.GlobalTypes[new XmlQualifiedName("UpgradeTemplate",ea)] is not XmlSchemaComplexType owner
            || owner.ContentTypeParticle is not XmlSchemaSequence sequence
            || sequence.Items.OfType<XmlSchemaElement>().SingleOrDefault(item => item.QualifiedName == new XmlQualifiedName("GameDependency",ea)) is not XmlSchemaElement declaration
            || declaration.MinOccurs != 0 || declaration.MaxOccurs != 1
            || declaration.ElementSchemaType is not XmlSchemaComplexType type || type.QualifiedName != new XmlQualifiedName("GameDependencyType",ea)
            || type.ContentType != XmlSchemaContentType.ElementOnly || type.AttributeWildcard != null
            || type.ContentTypeParticle is not XmlSchemaSequence fields || fields.MinOccurs != 1 || fields.MaxOccurs != 1 || fields.Items.Count != 4)
            throw new InvalidDataException("Reviewed inherited UpgradeTemplate complementary dependency pair required.");
        string[] names = { "RequiredObject","ForbiddenUpgrade","NeededUpgrade","ObjectFilter" };
        // Reborn: inspect original owner metadata before the core can consume directives or silently strip TypeId attributes.
        foreach (XmlAttribute attribute in asset.Attributes)
        {
            if (attribute.NamespaceURI == "http://www.w3.org/2000/xmlns/") continue;
            if (attribute.NamespaceURI.Length != 0 || attribute.Name == "TypeId" || attribute.Value.StartsWith('=')
                || owner.AttributeUses[new XmlQualifiedName(attribute.Name)] is not XmlSchemaAttribute use
                || use.AttributeSchemaType?.Datatype?.Variety == XmlSchemaDatatypeVariety.List && (attribute.Value.Contains('+') || attribute.Value.Contains('-')))
                throw new InvalidDataException("Original upgrade owner directives/unknown attributes remain closed before normalization.");
        }
        foreach (XmlNode node in asset.ChildNodes)
            if (node is not XmlElement && node is not XmlComment && !(node.NodeType == XmlNodeType.Whitespace || node.NodeType == XmlNodeType.Text && string.IsNullOrWhiteSpace(node.Value)))
                throw new InvalidDataException("Original upgrade owner text/directives remain closed before normalization.");
        string[] types = { "GameObjectWeakRef","UpgradeTemplateWeakRef","UpgradeTemplateWeakRef","ObjectFilter" };
        var declarations = fields.Items.OfType<XmlSchemaElement>().ToArray();
        if (declarations.Length != 4 || declarations.Where((field,index) => field.QualifiedName != new XmlQualifiedName(names[index],ea)
            || field.MinOccurs != 0 || field.MaxOccurs != (index == 3 ? 1 : decimal.MaxValue)
            || index < 3 && field.ElementSchemaType is not XmlSchemaSimpleType
            || field.ElementSchemaType?.QualifiedName != new XmlQualifiedName(types[index],ea)).Any())
            throw new InvalidDataException("Exact upgrade dependency sequence required.");
        List<SdkFilterCopies.Leaf> expected = new(); Dictionary<string,string> attributes = new(StringComparer.Ordinal); int populated = 0;
        // Reborn: the empty payload side must supply actual complementary conditions, not merely a redundant empty branch.
        bool complementaryConditions = false;
        foreach (var branch in branches)
        {
            if (branch.NamespaceURI != ea || branch.Prefix.Length != 0) throw new InvalidDataException("Unprefixed EA dependency required.");
            foreach (XmlNode node in branch.ChildNodes)
                if (node is not XmlElement && node is not XmlComment && !(node.NodeType == XmlNodeType.Whitespace || node.NodeType == XmlNodeType.Text && string.IsNullOrWhiteSpace(node.Value)))
                    throw new InvalidDataException("Dependency branch text/directives remain closed.");
            foreach (XmlAttribute attribute in branch.Attributes)
            {
                if (attribute.NamespaceURI == "http://www.w3.org/2000/xmlns/") continue;
                if (attribute.NamespaceURI.Length != 0 || attribute.Name is not ("RequiredModelConditionsAny" or "ForbiddenModelConditions" or "RequiredObjectStatusAny")
                    || type.AttributeUses[new XmlQualifiedName(attribute.Name)] is not XmlSchemaAttribute
                    || !attributes.TryAdd(attribute.Name,attribute.Value) || attribute.Value.Length > 4096 || attribute.Value.Any(character => !char.IsAsciiLetterOrDigit(character) && character is not ('_' or ' ')))
                    throw new InvalidDataException("Dependency fields must be disjoint bounded literals; identities/modifiers/conflicts remain closed.");
            }
            var leaves = branch.ChildNodes.OfType<XmlElement>().ToArray();
            if (leaves.Length > 0) populated++;
            else if (branch.Attributes.OfType<XmlAttribute>().Any(attribute => attribute.NamespaceURI.Length == 0)) complementaryConditions = true;
            foreach (var leaf in leaves)
            {
                if (expected.Count >= 128 || leaf.NamespaceURI != ea || leaf.Prefix.Length != 0 || leaf.LocalName is not ("RequiredObject" or "ForbiddenUpgrade" or "NeededUpgrade")
                    || leaf.Attributes.OfType<XmlAttribute>().Any(attribute => attribute.NamespaceURI != "http://www.w3.org/2000/xmlns/")
                    || leaf.ChildNodes.OfType<XmlNode>().Any(node => node.NodeType != XmlNodeType.Text)
                    || leaf.InnerText.Length is < 1 or > 128 || leaf.InnerText.Any(character => !char.IsAsciiLetterOrDigit(character) && character is not ('_' or '-' or '.')))
                    throw new InvalidDataException("Dependency payload requires bounded literal anonymous weak references.");
                expected.Add(new(leaf.LocalName,leaf.InnerText));
            }
        }
        if (populated != 1 || !complementaryConditions) throw new InvalidDataException("Exactly one populated dependency side and complementary attributes required.");
        var xml = asset.OwnerDocument!; var empty = (XmlElement)asset.CloneNode(false);
        var merged = (XmlElement)NodeJoiner.Override(schemas,xml,empty,asset);
        var actual = merged.ChildNodes.OfType<XmlElement>().ToArray();
        if (actual.Length != 1 || !actual[0].ChildNodes.OfType<XmlElement>().Select(leaf => new SdkFilterCopies.Leaf(leaf.LocalName,leaf.InnerText)).SequenceEqual(expected)
            || actual[0].Attributes.OfType<XmlAttribute>().Count(attribute => attribute.NamespaceURI != "http://www.w3.org/2000/xmlns/") != attributes.Count
            || attributes.Any(pair => actual[0].GetAttribute(pair.Key) != pair.Value))
            throw new InvalidDataException("Core dependency normalization differs from predicted attribute union/ordered leaves.");
        asset.ParentNode!.ReplaceChild(merged,asset);
        return new(asset.GetAttribute("id"),"GameDependency",2,1,expected.ToArray(),attributes.OrderBy(pair => pair.Key,StringComparer.Ordinal).Select(pair => pair.Key+"="+pair.Value).ToArray());
    }
}
