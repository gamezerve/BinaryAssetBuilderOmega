using System.Security.Cryptography;
using System.Text;
using System.Xml.Linq;

namespace BinaryAssetBuilder.ManifestInspector;

internal sealed record SchemaDifference(string Status, string Path);

internal static class SchemaComparer
{
    private static readonly XNamespace XmlSchema = "http://www.w3.org/2001/XMLSchema";

    public static IReadOnlyList<SchemaDifference> Compare(string leftRoot, string rightRoot)
    {
        var left = BuildIndex(leftRoot);
        var right = BuildIndex(rightRoot);
        return left.Keys.Union(right.Keys, StringComparer.OrdinalIgnoreCase)
            .OrderBy(path => path, StringComparer.OrdinalIgnoreCase)
            .Select(path =>
            {
                var hasLeft = left.TryGetValue(path, out var leftHash);
                var hasRight = right.TryGetValue(path, out var rightHash);
                return !hasLeft ? new SchemaDifference("Added", path) :
                    !hasRight ? new SchemaDifference("Removed", path) :
                    !leftHash!.SequenceEqual(rightHash!) ? new SchemaDifference("Changed", path) : null;
            })
            .Where(difference => difference is not null)
            .Cast<SchemaDifference>()
            .ToArray();
    }

    private static Dictionary<string, byte[]> BuildIndex(string root)
    {
        root = Path.GetFullPath(root);
        if (!Directory.Exists(root))
        {
            throw new DirectoryNotFoundException(root);
        }

        return Directory.EnumerateFiles(root, "*.xsd", SearchOption.AllDirectories)
            .ToDictionary(
                path => Path.GetRelativePath(root, path).Replace('\\', '/'),
                ComputeStructuralHash,
                StringComparer.OrdinalIgnoreCase);
    }

    private static byte[] ComputeStructuralHash(string path)
    {
        var document = XDocument.Load(path, LoadOptions.None);
        if (document.Root is null)
        {
            throw new InvalidDataException($"Schema '{path}' has no root element.");
        }

        var signature = new StringBuilder();
        AppendElement(signature, document.Root);
        return SHA256.HashData(Encoding.UTF8.GetBytes(signature.ToString()));
    }

    private static void AppendElement(StringBuilder output, XElement element)
    {
        if (element.Name == XmlSchema + "annotation")
        {
            return;
        }

        output.Append('<').Append(element.Name.NamespaceName).Append('|').Append(element.Name.LocalName);
        foreach (var attribute in element.Attributes()
                     .Where(attribute => !attribute.IsNamespaceDeclaration)
                     .OrderBy(attribute => attribute.Name.NamespaceName, StringComparer.Ordinal)
                     .ThenBy(attribute => attribute.Name.LocalName, StringComparer.Ordinal))
        {
            output.Append(' ').Append(attribute.Name.NamespaceName).Append('|')
                .Append(attribute.Name.LocalName).Append('=').Append(attribute.Value.Trim());
        }

        output.Append('>');
        foreach (var child in element.Elements())
        {
            AppendElement(output, child);
        }

        var text = string.Join(' ', element.Nodes().OfType<XText>()
            .Select(node => node.Value.Trim())
            .Where(value => value.Length > 0));
        if (text.Length > 0)
        {
            output.Append(text);
        }

        output.Append("</>");
    }
}
