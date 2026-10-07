using System.Xml;
using System.Xml.Schema;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: prove an Include-free metadata definition leaf without granting any instance asset exports.
internal static class SdkMetadataDefinitions
{
    // Reborn: distinguish schema-validated metadata source witnesses from imported inheritance handles.
    internal sealed record Witness(string SourcePath,string RawSha256,int Definitions,int AssetExports = 0);

    //-------------------------------------------------------------------------------------------------
    /** Reborn: require only empty Tags/Includes and bounded reviewed backward-only definitions, with no hidden asset or directive content. */
    //-------------------------------------------------------------------------------------------------
    internal static int Prove(XmlSchemaSet schemas,XmlDocument xml,string path,byte[] bytes)
    {
        const string ea = "uri:ea.com:eala:asset";
        var root = xml.DocumentElement!;
        if (root.LocalName != "AssetDeclaration" || root.NamespaceURI != ea || root.Prefix.Length != 0
            || root.Attributes.OfType<XmlAttribute>().Any(attribute => attribute.NamespaceURI != "http://www.w3.org/2000/xmlns/")
            || root.SelectNodes(".//*")!.Count > 1024)
            throw new InvalidDataException("Bounded EA metadata-only definition leaf required.");
        var children = root.ChildNodes.OfType<XmlElement>().ToArray();
        if (children.Length is < 1 or > 3 || children.Select(child => child.LocalName).Distinct().Count() != children.Length
            || children.Any(child => child.NamespaceURI != ea || child.Prefix.Length != 0 || child.LocalName is not ("Tags" or "Includes" or "Defines")
                || child.Attributes.OfType<XmlAttribute>().Any(attribute => attribute.NamespaceURI != "http://www.w3.org/2000/xmlns/")))
            throw new InvalidDataException("Metadata leaf cannot export assets or arbitrary metadata.");
        foreach (XmlNode node in root.ChildNodes)
            if (node is not XmlElement && node is not XmlComment && !(node.NodeType == XmlNodeType.Whitespace || node.NodeType == XmlNodeType.Text && string.IsNullOrWhiteSpace(node.Value)))
                throw new InvalidDataException("Metadata root text/directives remain closed.");
        foreach (var child in children)
        {
            if (child.LocalName != "Defines" && child.ChildNodes.OfType<XmlElement>().Any()) throw new InvalidDataException("Metadata leaf Tags/Includes must be empty.");
            foreach (XmlNode node in child.ChildNodes)
                if (node is not XmlElement && node is not XmlComment && !(node.NodeType == XmlNodeType.Whitespace || node.NodeType == XmlNodeType.Text && string.IsNullOrWhiteSpace(node.Value)))
                    throw new InvalidDataException("Metadata container text/directives remain closed.");
        }
        var definitions = SdkLocalDefineProfile.ReadLiteralDefinitions(root,allowExpressions:true);
        if (definitions.Count == 0) throw new InvalidDataException("Metadata leaf requires at least one reviewed definition.");
        Dictionary<string,string> resolved = new(StringComparer.Ordinal);
        foreach (var definition in children.Where(child => child.LocalName == "Defines").SelectMany(child => child.ChildNodes.OfType<XmlElement>()))
        {
            string name = definition.GetAttribute("name"),value = definitions[name];
            if (value[0] == '=') value = SdkDefinitionSubset.Evaluate(value,key => resolved.TryGetValue(key,out var earlier) ? earlier : null);
            resolved.Add(name,value);
        }
        var binding = SdkEffectiveSchema.Bind(schemas,path,bytes);
        if (!binding.XmlValidated || binding.Fields.Length != 0) throw new InvalidDataException("Metadata leaf requires schema-valid zero-field source content.");
        return definitions.Count;
    }
}
