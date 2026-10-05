using System.Security.Cryptography;
using System.Xml;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: inventory lexical schema declarations deriving from FileReference without inferring asset IDs, compiling sources or resolving payloads.
internal static class SdkFileReferenceCatalog
{
    // Reborn: declaration evidence is not effective inherited attributes, validated source binding or full dependency coverage.
    internal sealed record Field(string SchemaFile,string OwnerType,string Kind,string Name,string DeclaredType,string[] DerivationChain,bool InlineRestriction,bool PipelineOnly,string Use);
    internal sealed record Report(string SchemaRoot,int SchemaFiles,string SchemaCatalogSha256,Field[] DeclaredFields,bool ReadOnly,bool SnapshotOnly,bool FullDependencyCoverage,bool ProductionBuildReady,string[] Limitations);
    private const string Xsd = "http://www.w3.org/2001/XMLSchema",Ea = "uri:ea.com:eala:asset";

    //-------------------------------------------------------------------------------------------------
    /** Reborn: inventory only the staged bounded catalog and verify each exact schema snapshot before deriving field declarations. */
    //-------------------------------------------------------------------------------------------------
    internal static Report Inspect()
    {
        string root = SdkEnvironmentPreflight.BaselineSchemaRoot(); var catalog = SdkEnvironmentPreflight.Catalog(root);
        List<(string Name,byte[] Bytes)> documents = new();
        foreach (var row in catalog.OrderBy(row => row.Key,StringComparer.Ordinal))
        {
            byte[] bytes = SdkEnvironmentPreflight.Read(Path.Combine(root,row.Key.Replace('/',Path.DirectorySeparatorChar)),2*1048576);
            if (Convert.ToHexString(SHA256.HashData(bytes)) != row.Value) throw new InvalidDataException("Schema changed while collecting declaration evidence.");
            documents.Add((row.Key,bytes));
        }
        Field[] fields = Collect(documents);
        var after = SdkEnvironmentPreflight.Catalog(root);
        if (after.Count != catalog.Count || catalog.Any(row => !after.TryGetValue(row.Key,out string? hash) || hash != row.Value)) throw new InvalidDataException("Schema catalog changed during declaration inspection.");
        return new(root,catalog.Count,SdkEnvironmentPreflight.Digest(catalog),fields,true,true,false,false,new[] {
            "Lexical declared fields only, including named and inline restriction chains; not a compiled effective schema or inherited/attribute-group expansion.",
            "QName namespaces are significant; simple-type list/union semantics and instance xsi:type binding are outside this inventory.",
            "No source validation, payload reads, compiler hash equivalence or game loading; bounded consecutive reads are not an atomic filesystem snapshot." });
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: resolve QName restriction ancestry before identifying attribute/element declarations; DataBlob is a file dependency, not merely raw text. */
    //-------------------------------------------------------------------------------------------------
    internal static Field[] Collect(IEnumerable<(string Name,byte[] Bytes)> inputs)
    {
        List<(string Name,XmlDocument Xml)> documents = new(); Dictionary<string,string?> bases = new(StringComparer.Ordinal);
        long total = 0;
        foreach (var input in inputs)
        {
            if (documents.Count >= 1024 || input.Bytes.Length > 2*1048576 || (total += input.Bytes.Length) > 64*1048576) throw new InvalidDataException("Schema declaration input bound exceeded.");
            XmlDocument xml = new() { XmlResolver = null }; using MemoryStream bytes = new(input.Bytes,false);
            using XmlReader reader = XmlReader.Create(bytes,new XmlReaderSettings { DtdProcessing = DtdProcessing.Prohibit,XmlResolver = null,MaxCharactersInDocument = 2*1048576 }); xml.Load(reader);
            XmlElement root = xml.DocumentElement!;
            if (root.LocalName != "schema" || root.NamespaceURI != Xsd) throw new InvalidDataException("XSD schema declaration document required.");
            foreach (XmlElement type in root.ChildNodes.OfType<XmlElement>().Where(node => node.NamespaceURI == Xsd && node.LocalName == "simpleType" && node.HasAttribute("name")))
            {
                string key = "{"+root.GetAttribute("targetNamespace")+"}"+type.GetAttribute("name");
                XmlElement? restriction = type.ChildNodes.OfType<XmlElement>().FirstOrDefault(node => node.NamespaceURI == Xsd && node.LocalName == "restriction");
                string? parent = restriction == null || !restriction.HasAttribute("base") ? null : QName(restriction,restriction.GetAttribute("base"));
                if (!bases.TryAdd(key,parent) && bases[key] != parent) throw new InvalidDataException("Conflicting named simple-type ancestry.");
            }
            documents.Add((input.Name,xml));
        }
        string terminal = "{"+Ea+"}FileReference";
        if (!bases.ContainsKey(terminal)) throw new InvalidDataException("EA FileReference declaration required.");
        List<Field> fields = new();
        foreach (var document in documents)
            foreach (XmlElement node in document.Xml.SelectNodes("//*")!)
            {
                if (node.NamespaceURI != Xsd || node.LocalName is not ("attribute" or "element") || !node.HasAttribute("name")) continue;
                string? type = node.HasAttribute("type") ? QName(node,node.GetAttribute("type")) : null;
                if (type == null)
                {
                    XmlElement? inline = node.ChildNodes.OfType<XmlElement>().FirstOrDefault(child => child.NamespaceURI == Xsd && child.LocalName == "simpleType");
                    XmlElement? restriction = inline?.ChildNodes.OfType<XmlElement>().FirstOrDefault(child => child.NamespaceURI == Xsd && child.LocalName == "restriction");
                    if (restriction?.HasAttribute("base") == true) type = QName(restriction,restriction.GetAttribute("base"));
                }
                if (type == null) continue;
                List<string> chain = new(); HashSet<string> seen = new(StringComparer.Ordinal); string? current = type;
                while (current != null)
                {
                    if (chain.Count >= 64 || !seen.Add(current)) throw new InvalidDataException("Simple-type restriction cycle/depth bound exceeded.");
                    chain.Add(current); if (current == terminal) break;
                    current = bases.TryGetValue(current,out string? parent) ? parent : null;
                }
                if (chain.LastOrDefault() != terminal) continue;
                string owner = "(global/anonymous)";
                for (XmlNode? ancestor = node.ParentNode; ancestor is XmlElement element; ancestor = ancestor.ParentNode)
                    if (element.NamespaceURI == Xsd && element.LocalName == "complexType" && element.HasAttribute("name")) { owner = element.GetAttribute("name"); break; }
                if (fields.Count >= 4096) throw new InvalidDataException("Schema file-field declaration bound exceeded.");
                // Reborn: distinguish an anonymous inline restriction's named base from a field's explicitly declared QName type.
                fields.Add(new(document.Name,owner,node.LocalName,node.GetAttribute("name"),type,chain.ToArray(),!node.HasAttribute("type"),node.GetAttribute("pipelineOnly","uri:ea.com:eala:asset:schema") is "true" or "1",node.GetAttribute("use")));
            }
        return fields.ToArray();
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: compare expanded QName identities, never a coincidentally equal local type name from an unrelated namespace. */
    //-------------------------------------------------------------------------------------------------
    private static string QName(XmlElement node,string value)
    {
        // Reborn: xs:QName collapses surrounding whitespace; do not confuse valid schema lexical padding with filesystem path normalization.
        value = value.Trim(' ','\t','\r','\n');
        string[] parts = value.Split(':'); if (parts.Length > 2 || parts.Any(part => part.Length == 0)) throw new InvalidDataException("Invalid schema type QName.");
        foreach (string part in parts) XmlConvert.VerifyNCName(part);
        string prefix = parts.Length == 2 ? parts[0] : ""; string? uri = node.GetNamespaceOfPrefix(prefix);
        if (uri == null || prefix.Length > 0 && uri.Length == 0) throw new InvalidDataException("Unbound schema type QName prefix.");
        return "{"+uri+"}"+parts.Last();
    }
}
