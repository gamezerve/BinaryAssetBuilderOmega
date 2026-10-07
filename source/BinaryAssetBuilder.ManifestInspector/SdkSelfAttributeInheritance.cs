using System.Security.Cryptography;
using System.Text;
using System.Xml;
using System.Xml.Schema;
using BinaryAssetBuilder.Core.SageXml;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: use the existing core joiner only for bounded same-document, same-type attribute-only overlays; all richer inheritance remains closed.
internal static class SdkSelfAttributeInheritance
{
    internal const string Name = "diagnostic-self-attribute-inheritance-v1";
    // Reborn: one-sided flat child copying is independently explicit; matching/replacing children remains closed.
    internal const string ChildCopyName = "diagnostic-self-child-copy-v1";
    // Reborn: overlay evidence identifies source-level handles and transformed bytes, never native asset/stream identities.
    internal sealed record Overlay(string Type,string DerivedId,string BaseId);
    internal sealed record Evidence(string Profile,string RawSha256,string? ProcessedSha256,Overlay[] Overlays,string[] Diagnostics);
    internal sealed record Result(byte[]? Bytes,Evidence Evidence);
    private const string Ea = "uri:ea.com:eala:asset";

    //-------------------------------------------------------------------------------------------------
    /** Reborn: expand local leaf-asset chains with the core's attribute replacement behavior and reject the entire document on unsupported semantics. */
    //-------------------------------------------------------------------------------------------------
    internal static Result Apply(XmlSchemaSet schemas,byte[] bytes,bool childCopy = false)
    {
        string raw = Convert.ToHexString(SHA256.HashData(bytes));
        string profile = childCopy ? ChildCopyName : Name;
        try
        {
            if (!schemas.IsCompiled || bytes.Length > 4*1048576) throw new InvalidDataException("Compiled schema and bounded source required.");
            // Reborn: child-copy parsing follows core's default whitespace handling so formatting XmlWhitespace nodes do not become unexpected sequence children.
            XmlDocument xml = new() { XmlResolver = null,PreserveWhitespace = !childCopy };
            using (MemoryStream input = new(bytes,false))
            using (XmlReader reader = XmlReader.Create(input,new XmlReaderSettings { DtdProcessing = DtdProcessing.Prohibit,XmlResolver = null,MaxCharactersInDocument = 4*1048576 })) xml.Load(reader);
            XmlElement? root = xml.DocumentElement;
            if (root?.LocalName != "AssetDeclaration" || root.NamespaceURI != Ea) throw new InvalidDataException("EA AssetDeclaration required.");
            var assets = root.ChildNodes.OfType<XmlElement>().Where(element => element.LocalName is not ("Includes" or "Defines" or "Tags")).ToArray();
            if (assets.Length > 4096) throw new InvalidDataException("4096 local asset bound exceeded.");
            Dictionary<string,XmlElement> originals = new(StringComparer.Ordinal),resolved = new(StringComparer.Ordinal);
            // Reborn: track semantic chain height as well as active recursion so memoized/forward declarations cannot bypass the chain bound.
            Dictionary<string,int> heights = new(StringComparer.Ordinal);
            HashSet<string> active = new(StringComparer.Ordinal); List<Overlay> overlays = new();
            // Reborn: bound aggregate inherited-attribute amplification before allocating each merged node, not only after final serialization.
            long expandedBytes = bytes.Length+1024L;
            foreach (var asset in assets)
            {
                string id = asset.GetAttribute("id");
                if (asset.NamespaceURI != Ea || !Token(id) || !originals.TryAdd(asset.LocalName+":"+id,asset)) throw new InvalidDataException("Unsupported or duplicate local asset identity.");
                // Reborn: require the whole document's asset slice to be expression-free leaves; unrelated complex/raw expressions cannot ride along unprocessed.
                CheckLeaf(asset);
            }
            // Reborn: inherited attributes on nested/metadata elements cannot silently bypass this top-level-only profile.
            if (root.SelectNodes(".//*")!.OfType<XmlElement>().Any(element => element.HasAttribute("inheritFrom") && !assets.Contains(element))) throw new InvalidDataException("Nested inheritFrom is outside the profile.");
            foreach (var asset in assets.Where(element => element.HasAttribute("inheritFrom"))) Resolve(asset.LocalName+":"+asset.GetAttribute("id"),0);
            foreach (var asset in assets.Where(element => element.HasAttribute("inheritFrom"))) root.ReplaceChild(resolved[asset.LocalName+":"+asset.GetAttribute("id")],asset);
            using MemoryStream output = new();
            using (XmlWriter writer = XmlWriter.Create(output,new XmlWriterSettings { Encoding = new UTF8Encoding(false),NewLineHandling = NewLineHandling.None })) xml.Save(writer);
            byte[] processed = output.ToArray();
            if (processed.Length > 4*1048576) throw new InvalidDataException("Processed inheritance XML exceeds 4 MiB.");
            return new(processed,new(profile,raw,Convert.ToHexString(SHA256.HashData(processed)),overlays.ToArray(),Array.Empty<string>()));

            //-------------------------------------------------------------------------------------------------
            /** Reborn: local handles precede imported visibility; reject same-handle overrides, missing bases, cross-type inheritance and cycles. */
            //-------------------------------------------------------------------------------------------------
            XmlElement Resolve(string handle,int depth)
            {
                if (depth > 32 || active.Contains(handle)) throw new InvalidDataException("Local inheritance cycle/depth bound exceeded.");
                if (resolved.TryGetValue(handle,out var previous)) return previous;
                if (!originals.TryGetValue(handle,out var asset)) throw new InvalidDataException("Base is not a uniquely captured same-document asset; Include visibility remains closed.");
                CheckLeaf(asset); active.Add(handle);
                XmlElement result = (XmlElement)asset.CloneNode(true);
                int height = 0;
                if (asset.HasAttribute("inheritFrom"))
                {
                    string target = asset.GetAttribute("inheritFrom"),type = asset.LocalName;
                    string baseId = target;
                    if (target.Contains(':'))
                    { var parts = target.Split(':'); if (parts.Length != 2 || parts[0] != type) throw new InvalidDataException("Cross-type inherited handle is outside the profile."); baseId = parts[1]; }
                    if (!Token(baseId) || baseId == asset.GetAttribute("id")) throw new InvalidDataException("Empty/unsafe base or same-handle imported override remains closed.");
                    XmlElement baseAsset = Resolve(type+":"+baseId,depth+1);
                    // Reborn: both populated sides require actual child matching semantics and cannot pass a copy-only admission rule.
                    if (childCopy && baseAsset.ChildNodes.OfType<XmlElement>().Any() && asset.ChildNodes.OfType<XmlElement>().Any()) throw new InvalidDataException("Both base and derived contain children; child merge semantics remain closed.");
                    height = heights[type+":"+baseId]+1;
                    if (height > 32) throw new InvalidDataException("32-link local inheritance chain bound exceeded.");
                    foreach (XmlAttribute attribute in baseAsset.Attributes)
                        if (attribute.NamespaceURI.Length == 0 && !asset.HasAttribute(attribute.Name)) expandedBytes += Encoding.UTF8.GetByteCount(attribute.OuterXml)+1L;
                    if (childCopy) foreach (XmlElement child in baseAsset.ChildNodes.OfType<XmlElement>()) expandedBytes += Encoding.UTF8.GetByteCount(child.OuterXml)+1L;
                    if (expandedBytes > 4*1048576) throw new InvalidDataException("Inherited attribute amplification exceeds 4 MiB before merge.");
                    result = (XmlElement)NodeJoiner.Override(schemas,xml,baseAsset,asset);
                    overlays.Add(new(type,asset.GetAttribute("id"),baseId));
                }
                active.Remove(handle); resolved.Add(handle,result); heights.Add(handle,height); return result;
            }

            //-------------------------------------------------------------------------------------------------
            /** Reborn: refuse child content, expression inputs, instance directives, xsi:type and list modifiers before invoking the core joiner. */
            //-------------------------------------------------------------------------------------------------
            void CheckLeaf(XmlElement asset)
            {
                if (!childCopy && asset.ChildNodes.OfType<XmlNode>().Any(node => node is not XmlComment && !((node.NodeType is XmlNodeType.Text or XmlNodeType.Whitespace or XmlNodeType.SignificantWhitespace) && string.IsNullOrWhiteSpace(node.Value)))) throw new InvalidDataException("Only attribute-only base/derived assets are admitted.");
                var name = new XmlQualifiedName(asset.LocalName,Ea);
                if (schemas.GlobalTypes[name] is not XmlSchemaComplexType type || type.AttributeWildcard != null) throw new InvalidDataException("Exact named complex asset schema without attribute wildcard required.");
                foreach (XmlAttribute attribute in asset.Attributes)
                {
                    if (attribute.NamespaceURI == "http://www.w3.org/2000/xmlns/") continue;
                    if (attribute.NamespaceURI.Length != 0 || attribute.Name == "TypeId" || attribute.Value.StartsWith('=')) throw new InvalidDataException("Namespaced directives, TypeId or unevaluated attributes require broader preprocessing.");
                    if (type.AttributeUses[new XmlQualifiedName(attribute.Name)] is not XmlSchemaAttribute use) throw new InvalidDataException("Unknown asset attribute cannot be hidden by an overlay.");
                    if (use.AttributeSchemaType?.Datatype?.Variety == XmlSchemaDatatypeVariety.List && (attribute.Value.Contains('+') || attribute.Value.Contains('-'))) throw new InvalidDataException("Bitflag/list modifiers require separately reviewed semantics.");
                }
                if (childCopy) CheckChildren(asset,type);
            }

            //-------------------------------------------------------------------------------------------------
            /** Reborn: admit only direct simple-content sequence children; refuse nested structures, wildcards, choice/group particles and instance directives. */
            //-------------------------------------------------------------------------------------------------
            void CheckChildren(XmlElement asset,XmlSchemaComplexType type)
            {
                var children = asset.ChildNodes.OfType<XmlElement>().ToArray();
                foreach (XmlNode node in asset.ChildNodes)
                    if (node is not XmlElement && node is not XmlComment && !((node.NodeType is XmlNodeType.Text or XmlNodeType.Whitespace or XmlNodeType.SignificantWhitespace) && string.IsNullOrWhiteSpace(node.Value))) throw new InvalidDataException("Meaningful asset text or non-element content is outside child-copy scope.");
                if (children.Length == 0) return;
                if (type.ContentTypeParticle is not XmlSchemaSequence sequence || sequence.Items.OfType<XmlSchemaObject>().Any(item => item is not XmlSchemaElement)) throw new InvalidDataException("Flat direct sequence child schema required.");
                Dictionary<XmlQualifiedName,int> counts = new();
                foreach (var child in children)
                {
                    var qualified = new XmlQualifiedName(child.LocalName,child.NamespaceURI);
                    var declaration = sequence.Items.OfType<XmlSchemaElement>().SingleOrDefault(item => item.QualifiedName == qualified);
                    if (child.NamespaceURI != Ea || declaration?.ElementSchemaType is not XmlSchemaSimpleType || child.Attributes.OfType<XmlAttribute>().Any(attribute => attribute.NamespaceURI != "http://www.w3.org/2000/xmlns/")) throw new InvalidDataException("Only attribute-free simple sequence children are admitted.");
                    counts.TryGetValue(qualified,out int count); counts[qualified] = ++count;
                    if (count > declaration.MaxOccurs) throw new InvalidDataException("Child occurrence bound exceeded before copying.");
                    foreach (XmlNode node in child.ChildNodes)
                    {
                        if (node is not XmlComment && node.NodeType is not (XmlNodeType.Text or XmlNodeType.Whitespace or XmlNodeType.SignificantWhitespace)) throw new InvalidDataException("Nested/CDATA/directive child content requires broader merging.");
                        if (node is not XmlComment && node.Value?.StartsWith('=') == true) throw new InvalidDataException("Child node expressions require preprocessing before inheritance.");
                    }
                    if (child.InnerText.StartsWith('=')) throw new InvalidDataException("Child expressions require preprocessing before inheritance.");
                }
            }
        }
        catch (Exception error) when (error is IOException or InvalidDataException or XmlException or ArgumentException or BinaryAssetBuilderException)
        { return new(null,new(profile,raw,null,Array.Empty<Overlay>(),new[] { error.Message[..Math.Min(error.Message.Length,512)] })); }
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: admit bounded literal local identity tokens without guessing hash/case/traversal normalization. */
    //-------------------------------------------------------------------------------------------------
    private static bool Token(string value) => value.Length is > 0 and <= 128 && value.All(character => char.IsAsciiLetterOrDigit(character) || character is '_' or '-' or '.');
}
