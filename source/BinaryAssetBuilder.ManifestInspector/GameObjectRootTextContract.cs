using System.Text;
using System.Xml;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: localization keys are authored text, not stock identities or asset references; pointed and inline forms share exact source-byte expectations.
internal static class GameObjectRootTextContract
{
    internal static readonly string[] Pointed = { "DisplayName", "DisplayNameTransformed" };

    //-------------------------------------------------------------------------------------------------
    /** Reborn: distinguish absent strings from explicit empty strings while preserving XML-decoded text and source diagnostics. */
    //-------------------------------------------------------------------------------------------------
    internal static Dictionary<string, string?> Read(XmlDocument source)
    {
        var values = new Dictionary<string, string?>(StringComparer.Ordinal);
        foreach (var field in GameObjectRootStringContract.Fields)
            Add(field.Name, source.DocumentElement!.GetAttributeNode(field.Name));
        foreach (string name in Pointed)
            Add(name, source.DocumentElement!.ChildNodes.OfType<XmlElement>().SingleOrDefault(e => e.LocalName == name));
        // Reborn: Browser is a separately proved string list; validate its tokens with source diagnostics here.
        _ = GameObjectBrowserContract.Tokens(source);
        return values;

        // Reborn: these wire fields are ASCII localization keys; translated display text belongs in a separate localization table.
        void Add(string name, XmlNode? node)
        {
            string? text = node is XmlAttribute attribute ? attribute.Value : node?.InnerText;
            if (text != null && text.Any(c => c == 0 || c > 127))
                throw SdkXmlDiagnostics.At(new InvalidDataException("GameObject text requires non-NUL ASCII: " + name), node);
            values.Add("GameObject." + name, text);
        }
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: comparison copies mask text/presence only; actual source remains unchanged for native allocation and metadata generation. */
    //-------------------------------------------------------------------------------------------------
    internal static void Mask(XmlDocument candidate, XmlDocument model)
    {
        GameObjectRootStringContract.Mask(candidate, model);
        GameObjectBrowserContract.Mask(candidate, model);
        foreach (string name in Pointed)
        {
            foreach (var node in candidate.DocumentElement!.ChildNodes.OfType<XmlElement>().Where(e => e.LocalName == name).ToArray())
                candidate.DocumentElement.RemoveChild(node);
            var original = model.DocumentElement!.ChildNodes.OfType<XmlElement>().SingleOrDefault(e => e.LocalName == name);
            if (original == null) continue;
            int ordinal = model.DocumentElement.ChildNodes.OfType<XmlElement>().TakeWhile(e => e != original).Count();
            var before = candidate.DocumentElement.ChildNodes.OfType<XmlElement>().ElementAtOrDefault(ordinal);
            candidate.DocumentElement.InsertBefore(candidate.ImportNode(original, true), before);
        }
    }
}
