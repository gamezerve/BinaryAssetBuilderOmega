using System.Xml;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: freeze a small confined Include graph and rewrite only its paths into deterministic owned snapshot names.
internal sealed class DiagnosticSourceGraph
{
    private readonly Dictionary<string, XmlDocument> documents = new(StringComparer.OrdinalIgnoreCase);
    private readonly Dictionary<string, string> names = new(StringComparer.OrdinalIgnoreCase);
    private readonly HashSet<string> active = new(StringComparer.OrdinalIgnoreCase);
    private readonly HashSet<string> identities = new(StringComparer.OrdinalIgnoreCase);
    private readonly string sourceRoot;
    private int roots, edges;
    private long characters;

    //-------------------------------------------------------------------------------------------------
    /** Reborn: anchor every relative Include beneath the entry document directory before reading any child. */
    //-------------------------------------------------------------------------------------------------
    private DiagnosticSourceGraph(string entry) => sourceRoot = Path.GetDirectoryName(entry)!;

    //-------------------------------------------------------------------------------------------------
    /** Reborn: validate the entire reachable graph before compiler settings or output state can change. */
    //-------------------------------------------------------------------------------------------------
    internal static DiagnosticSourceGraph Read(string entry)
    {
        DiagnosticSourceGraph graph = new(entry);
        graph.Visit(entry, 0);
        if (graph.roots == 0) throw new InvalidDataException("Diagnostic graph contains no asset roots.");
        return graph;
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: copy only approved XML snapshots; flat generated names keep nested source paths out of compiled manifests. */
    //-------------------------------------------------------------------------------------------------
    internal void Write(string directory, List<string> ownedNames)
    {
        foreach (var pair in documents)
        {
            string name = names[pair.Key]; ownedNames.Add(name);
            File.WriteAllText(Path.Combine(directory, name), pair.Value.DocumentElement!.OuterXml);
        }
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: reject cycles, excessive graphs, duplicate asset identities and uncontrolled input paths before recursive core loading. */
    //-------------------------------------------------------------------------------------------------
    private string Visit(string path, int depth)
    {
        if (depth > 8) throw new InvalidDataException("Diagnostic Include depth exceeds eight edges.");
        if (active.Contains(path)) throw new InvalidDataException("Diagnostic Include cycle detected.");
        if (names.TryGetValue(path, out string? known)) return known;
        if (documents.Count >= 16) throw new InvalidDataException("Diagnostic graph exceeds sixteen XML files.");
        RejectReparse(path);
        if (!File.Exists(path) || new FileInfo(path).Length > 1024 * 1024)
            throw new InvalidDataException("Diagnostic XML is absent or exceeds 1 MiB.");
        using XmlReader reader = XmlReader.Create(path, new XmlReaderSettings { DtdProcessing = DtdProcessing.Prohibit,
            XmlResolver = null, MaxCharactersInDocument = 1024 * 1024 });
        XmlDocument xml = new() { XmlResolver = null }; xml.Load(reader);
        XmlElement root = xml.DocumentElement ?? throw new InvalidDataException("An EA AssetDeclaration is required.");
        if (root.LocalName != "AssetDeclaration" || root.NamespaceURI != "uri:ea.com:eala:asset")
            throw new InvalidDataException("An EA AssetDeclaration is required.");
        characters += root.OuterXml.Length;
        if (characters > 2 * 1024 * 1024) throw new InvalidDataException("Diagnostic graph exceeds two MiB of XML characters.");
        string name = documents.Count == 0 ? "source.xml" : $"input-{documents.Count:D4}.xml";
        documents.Add(path, xml); names.Add(path, name); active.Add(path);
        foreach (XmlElement element in root.SelectNodes("descendant-or-self::*")!)
        {
            if (element.NamespaceURI != root.NamespaceURI) throw new InvalidDataException("Foreign elements are not admitted.");
            // Reborn: matching direct leaves and the official Include container are the only permitted non-root shapes.
            bool valid = element == root || (element.LocalName switch
            {
                "AttributeModifier" or "ShaderOverride" or "ObjectFilterAsset" or "FXList" or "Multisound" or "Includes" => element.ParentNode == root,
                // Reborn: admit only direct weighted sound references; nested/optional audio controls remain profile-gated.
                "Subsound" => element.ParentNode is XmlElement soundRoot && soundRoot.LocalName == "Multisound" && soundRoot.ParentNode == root,
                // Reborn: admit only the checked sound-only FX shape; filters, particles and other nuggets remain outside command preflight.
                "NuggetList" => element.ParentNode is XmlElement fx && fx.LocalName == "FXList" && fx.ParentNode == root,
                "Sound" => element.ParentNode is XmlElement nuggets && nuggets.LocalName == "NuggetList"
                    && nuggets.ParentNode is XmlElement fxOwner && fxOwner.LocalName == "FXList" && fxOwner.ParentNode == root,
                "Modifier" => element.ParentNode is XmlElement modifier && modifier.LocalName == "AttributeModifier" && modifier.ParentNode == root,
                "Rule" => element.ParentNode is XmlElement shader && shader.LocalName == "ShaderOverride" && shader.ParentNode == root,
                "Filter" => element.ParentNode is XmlElement filterAsset && filterAsset.LocalName == "ObjectFilterAsset" && filterAsset.ParentNode == root,
                "IncludeThing" => element.ParentNode is XmlElement filter && filter.LocalName == "Filter"
                    && filter.ParentNode is XmlElement owner && owner.LocalName == "ObjectFilterAsset" && owner.ParentNode == root,
                "Include" => element.ParentNode is XmlElement includes && includes.LocalName == "Includes" && includes.ParentNode == root,
                _ => false
            });
            if (!valid) throw new InvalidDataException("Unsupported diagnostic XML structure.");
            foreach (XmlAttribute attribute in element.Attributes)
                // Reborn: authored input must not supply compiler-injected TypeIds or pre-normalized Sound selector suffixes that the core could silently rewrite.
                if (attribute.NamespaceURI != "http://www.w3.org/2000/xmlns/" && (attribute.LocalName is "inheritFrom" or "override" or "TypeId"
                    || element.LocalName == "Sound" && attribute.LocalName == "Value" && attribute.Value.Contains('\\')
                    || attribute.Value.TrimStart().StartsWith("=", StringComparison.Ordinal)))
                    throw new InvalidDataException("Inheritance, overrides, unresolved expressions, authored TypeIds and normalized Sound selectors are not admitted.");
            // Reborn: authored subsound text cannot contain the selector suffix/formula that normalization would otherwise rewrite.
            if (element.LocalName == "Subsound" && (element.InnerText.Contains('\\') || element.InnerText.TrimStart().StartsWith("=", StringComparison.Ordinal)))
                throw new InvalidDataException("Authored normalized Subsound selectors and expressions are not admitted.");
            if (element.ParentNode == root && element.LocalName is "AttributeModifier" or "ShaderOverride" or "ObjectFilterAsset" or "FXList" or "Multisound")
            {
                string id = element.GetAttribute("id");
                if (id.Length is < 1 or > 128 || id.Any(c => !char.IsAsciiLetterOrDigit(c) && c != '_' && c != '-' && c != '.'))
                    throw new InvalidDataException("Diagnostic root ids must be 1–128 safe ASCII characters.");
                if (++roots > 32 || !identities.Add(element.LocalName + ":" + id))
                    throw new InvalidDataException("Diagnostic graph exceeds 32 roots or has duplicate identities.");
            }
        }
        // Reborn: materialize the element list before rewriting edges; instance/all semantics stay intact for the core processor.
        foreach (XmlElement include in root.SelectNodes("descendant::*[local-name()='Include']")!)
        {
            if (++edges > 32 || include.GetAttribute("type") is not "all" and not "instance")
                throw new InvalidDataException("At most 32 all/instance Include edges are admitted; reference Includes remain disabled.");
            string relative = include.GetAttribute("source").Replace('\\', '/');
            string[] segments = relative.Split('/');
            if (relative.Length is < 1 or > 512 || Path.IsPathRooted(relative) || relative.Any(c => c < 32 || ":;$*?".Contains(c))
                || segments.Any(segment => segment is "" or "." or ".." || segment.EndsWith(' ') || segment.EndsWith('.'))
                || !relative.EndsWith(".xml", StringComparison.OrdinalIgnoreCase))
                throw new InvalidDataException("Include source must be a confined relative XML path without traversal or macros.");
            string child = Path.GetFullPath(Path.Combine(Path.GetDirectoryName(path)!, relative));
            if (!child.StartsWith(sourceRoot + Path.DirectorySeparatorChar, StringComparison.OrdinalIgnoreCase))
                throw new InvalidDataException("Include escaped the entry source directory.");
            include.SetAttribute("source", Visit(child, depth + 1));
        }
        active.Remove(path);
        return name;
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: reject redirected files/directories so a relative Include cannot cross the admitted source boundary through a junction. */
    //-------------------------------------------------------------------------------------------------
    private static void RejectReparse(string path)
    {
        if (File.Exists(path) && (File.GetAttributes(path) & FileAttributes.ReparsePoint) != 0)
            throw new InvalidDataException("Diagnostic XML cannot be a reparse point.");
        for (DirectoryInfo? directory = new(Path.GetDirectoryName(path)!); directory != null; directory = directory.Parent)
            if (!directory.Exists || (directory.Attributes & FileAttributes.ReparsePoint) != 0)
                throw new InvalidDataException("Diagnostic source ancestors cannot contain reparse points.");
    }
}
