using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: diagnostic literal substitution is not the unavailable EA evaluator, imported define visibility or asset inheritance.
internal static class SdkLocalDefineProfile
{
    internal const string Name = "diagnostic-local-literals-v1";
    // Reborn: retain raw and processed identities separately; rejected documents never publish partially substituted bytes.
    internal sealed record Evidence(string Profile,string RawSha256,string? ProcessedSha256,int Substitutions,string[] Diagnostics)
    {
        // Reborn: imported literal evidence records exact source identities and definition origins, not compiled child assets.
        public SdkIncludeDefineProfile.SourceIdentity[] DefinitionSources { get; init; } = Array.Empty<SdkIncludeDefineProfile.SourceIdentity>();
        public SdkIncludeDefineProfile.Origin[] DefinitionOrigins { get; init; } = Array.Empty<SdkIncludeDefineProfile.Origin>();
    }
    internal sealed record Result(byte[]? Bytes,Evidence Evidence);
    private const string Ea = "uri:ea.com:eala:asset";
    private static readonly Regex Identifier = new("\\A[A-Za-z_][A-Za-z0-9_]{0,127}\\z",RegexOptions.CultureInvariant,TimeSpan.FromSeconds(1));

    //-------------------------------------------------------------------------------------------------
    /** Reborn: substitute only exact local literal references in owned source memory, with no Include reads, expression engine or source writes. */
    //-------------------------------------------------------------------------------------------------
    internal static Result Apply(byte[] bytes) => ApplyCore(bytes,null);

    //-------------------------------------------------------------------------------------------------
    /** Reborn: only a rechecked Include closure may supply the complete literal table; this entry does not authorize disk reads itself. */
    //-------------------------------------------------------------------------------------------------
    internal static Result ApplyImported(byte[] bytes,IReadOnlyDictionary<string,string> literals) => ApplyCore(bytes,literals);

    //-------------------------------------------------------------------------------------------------
    /** Reborn: share bounded substitution without changing the default local-only Include refusal. */
    //-------------------------------------------------------------------------------------------------
    private static Result ApplyCore(byte[] bytes,IReadOnlyDictionary<string,string>? imported)
    {
        if (bytes.Length > 4*1048576) throw new InvalidDataException("Local define source exceeds 4 MiB.");
        string raw = Convert.ToHexString(SHA256.HashData(bytes));
        XmlDocument xml = new() { XmlResolver = null,PreserveWhitespace = true };
        using (MemoryStream input = new(bytes,false))
        using (XmlReader reader = XmlReader.Create(input,new XmlReaderSettings { DtdProcessing = DtdProcessing.Prohibit,XmlResolver = null,MaxCharactersInDocument = 4*1048576 })) xml.Load(reader);
        XmlElement? root = xml.DocumentElement;
        if (root?.LocalName != "AssetDeclaration" || root.NamespaceURI != Ea) return Reject(raw,"EA AssetDeclaration required.");
        List<XmlNode> slots = new();
        foreach (XmlElement asset in root.ChildNodes.OfType<XmlElement>())
        {
            if (asset.NamespaceURI != Ea || asset.LocalName is "Defines" or "Includes" or "Tags") continue;
            Collect(asset,slots,0);
        }
        if (slots.Count == 0) return new(bytes,new(Name,raw,raw,0,Array.Empty<string>()));
        if (imported == null && root.ChildNodes.OfType<XmlElement>().Any(element => element.NamespaceURI == Ea && element.LocalName == "Includes" && element.ChildNodes.OfType<XmlElement>().Any()))
            return Reject(raw,"Expressions with Includes require reviewed imported-definition visibility; local-only substitution refused.");
        if (root.SelectNodes(".//*")!.OfType<XmlElement>().Any(element => element.NamespaceURI == Ea && element.HasAttribute("inheritFrom")))
            return Reject(raw,"Asset inheritFrom processing is outside the local literal profile.");
        IReadOnlyDictionary<string,string> defines;
        try { defines = imported ?? ReadLiteralDefinitions(root); }
        catch (InvalidDataException) { return Reject(raw,"Unsupported, duplicate, chained or override definition; no partial substitution."); }
        foreach (XmlNode slot in slots)
        {
            string expression = slot.Value!;
            if (!expression.StartsWith("=$",StringComparison.Ordinal) || !Identifier.IsMatch(expression[2..]) || !defines.TryGetValue(expression[2..],out string? value))
                return Reject(raw,"Only exact =$NAME references to case-sensitive local literal definitions are admitted.");
            slot.Value = value;
        }
        using MemoryStream output = new();
        using (XmlWriter writer = XmlWriter.Create(output,new XmlWriterSettings { Encoding = new UTF8Encoding(false),Indent = false,NewLineHandling = NewLineHandling.None })) xml.Save(writer);
        byte[] processed = output.ToArray();
        if (processed.Length > 4*1048576) return Reject(raw,"Processed XML exceeds 4 MiB.");
        return new(processed,new(Name,raw,Convert.ToHexString(SHA256.HashData(processed)),slots.Count,Array.Empty<string>()));
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: share exact local literal admission rules with the source-backed Include profile, without evaluating chained/arithmetic definitions. */
    //-------------------------------------------------------------------------------------------------
    internal static Dictionary<string,string> ReadLiteralDefinitions(XmlElement root)
    {
        Dictionary<string,string> defines = new(StringComparer.Ordinal);
        var containers = root.ChildNodes.OfType<XmlElement>().Where(element => element.NamespaceURI == Ea && element.LocalName == "Defines").ToArray();
        if (containers.Length > 1) throw new InvalidDataException("Multiple Defines containers are unsupported.");
        foreach (XmlElement define in containers.SelectMany(element => element.ChildNodes.OfType<XmlElement>()))
        {
            string name = define.GetAttribute("name"),value = define.GetAttribute("value");
            if (define.NamespaceURI != Ea || define.LocalName != "Define" || !Identifier.IsMatch(name) || !define.HasAttribute("value")
                || value.Length == 0 || value.Length > 512 || value[0] == '=' || define.HasChildNodes
                || define.Attributes.OfType<XmlAttribute>().Any(attribute => attribute.Name is not ("name" or "value" or "override"))
                || (define.HasAttribute("override") && define.GetAttribute("override") != "false") || defines.Count >= 512 || !defines.TryAdd(name,value))
                throw new InvalidDataException("Unsupported, duplicate, chained or override definition; no partial substitution.");
        }
        return defines;
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: match the core's leading-equals attribute/text trigger while bounding recursion and substitution count. */
    //-------------------------------------------------------------------------------------------------
    private static void Collect(XmlNode node,List<XmlNode> slots,int depth)
    {
        if (depth > 128) throw new InvalidDataException("Local define XML depth bound exceeded.");
        if (node.Attributes != null)
            foreach (XmlAttribute attribute in node.Attributes)
                if (attribute.Value.StartsWith('=')) slots.Add(attribute);
        if (node.NodeType is XmlNodeType.Text or XmlNodeType.CDATA && node.Value?.StartsWith('=') == true) slots.Add(node);
        if (slots.Count > 2048) throw new InvalidDataException("Local define substitution bound exceeded.");
        foreach (XmlNode child in node.ChildNodes) Collect(child,slots,depth+1);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: rejected diagnostic preprocessing retains raw identity only and never exposes partial transformed content. */
    //-------------------------------------------------------------------------------------------------
    private static Result Reject(string raw,string reason) => new(null,new(Name,raw,null,0,new[] { reason }));
}
