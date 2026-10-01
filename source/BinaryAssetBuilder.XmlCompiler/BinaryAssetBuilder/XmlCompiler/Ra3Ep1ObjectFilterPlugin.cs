using System;
using System.Globalization;
using System.Xml;
using System.Xml.Schema;
using BinaryAssetBuilder.Core;
using BinaryAssetBuilder.Core.Hashing;
using Relo;
using SageBinaryData;

namespace BinaryAssetBuilder.XmlCompiler;

// Reborn: isolate the stock-proven infiltration filter subset and normalized weak identities from the legacy registry and all production/cache output.
public sealed class Ra3Ep1ObjectFilterPlugin : IAssetBuilderPlugin, IAssetBuilderOutputPolicy
{
    private bool _initialized;
    public string ProfileName => "RA3EP1-ObjectFilter-Experimental-v1";
    public bool CanWriteProductionOutput => false;
    public bool CanUseBuildCache => false;
    public bool CanReuseCompiledDocuments => false;
    public uint AllTypesHash => 0x5454A8E9u;
    public uint VersionNumber => 1;

    //-------------------------------------------------------------------------------------------------
    /** Reborn: enable only the proven Win32 ABI and revoke readiness on unsupported initialization. */
    //-------------------------------------------------------------------------------------------------
    public void Initialize(TargetPlatform platform)
    {
        _initialized = false;
        if (platform != TargetPlatform.Win32) throw new NotSupportedException("Experimental EP1 filters support only Win32.");
        _initialized = true;
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: reinitialization retains the platform gate without shared static state. */
    //-------------------------------------------------------------------------------------------------
    public void ReInitialize(TargetPlatform platform) => Initialize(platform);

    //-------------------------------------------------------------------------------------------------
    /** Reborn: return detached observed EP1 metadata with an isolated local processing stamp and no KW fallback. */
    //-------------------------------------------------------------------------------------------------
    public ExtendedTypeInformation GetExtendedTypeInformation(uint typeId)
    {
        if (!_initialized) throw new InvalidOperationException("Experimental EP1 filter profile is not initialized.");
        if (typeId != 0x44A5973Du) throw new NotSupportedException($"Unsupported experimental filter type 0x{typeId:X8}.");
        return new ExtendedTypeInformation { TypeId = typeId, TypeName = nameof(ObjectFilterAsset), Type = typeof(ObjectFilterAsset),
            TypeHash = 0xDF72B4BAu, ProcessingHash = 0xDF72B4BAu ^ 0x45503121u, Tokenized = false,
            HasCustomData = false, UseBuildCache = false };
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: compile only validated NONE-rule/default-alignment filters with proven kind/relationship values and checked ordered weak names. */
    //-------------------------------------------------------------------------------------------------
    public unsafe AssetBuffer ProcessInstance(InstanceDeclaration instance)
    {
        if (instance?.Node is not XmlElement root) throw new ArgumentException("A filter root is required.", nameof(instance));
        GetExtendedTypeInformation(instance.Handle.TypeId);
        if (root.LocalName != nameof(ObjectFilterAsset) || root.NamespaceURI != "uri:ea.com:eala:asset"
            || root.SchemaInfo.SchemaType?.Name != nameof(ObjectFilterAsset) || root.OwnerDocument.Schemas.Count == 0
            || instance.InheritFromHandle != null || instance.HasCustomData || instance.ReferencedInstances.Count != 0 || instance.ReferencedFiles.Count != 0)
            throw new NotSupportedException("Experimental filters require validated standalone XML without inheritance or strong/file dependencies.");
        CheckAttributes(root, new[] { "id" });
        // Reborn: a valid post-validation ID edit must not compile under the declaration's stale asset identity.
        if (InstanceHandle.GetInstanceId(root.GetAttribute("id")) != instance.Handle.InstanceId)
            throw new NotSupportedException("Filter XML id does not match its declaration identity.");
        XmlElement filter = null;
        foreach (XmlNode child in root.ChildNodes)
        {
            if (child is XmlElement element)
            {
                if (filter != null || element.LocalName != "Filter" || element.NamespaceURI != root.NamespaceURI)
                    throw new NotSupportedException("Experimental filters require exactly one inline Filter.");
                filter = element;
            }
            else CheckWhitespace(child);
        }
        if (filter == null || filter.SchemaInfo.SchemaType?.Name != "ObjectFilter")
            throw new NotSupportedException("Schema-bound inline Filter is required.");
        CheckAttributes(filter, new[] { "Rule", "Relationship", "Alignment", "Include" });
        if (filter.GetAttribute("Rule") != "NONE" || filter.GetAttribute("Alignment") != "NONE"
            || Array.IndexOf(new[] { "", "ENEMIES" }, filter.GetAttribute("Relationship")) < 0
            || Array.IndexOf(new[] { "", "INFANTRY", "AIRCRAFT", "SHIP", "VEHICLE" }, filter.GetAttribute("Include")) < 0)
            throw new NotSupportedException("Filter rule, relationship, alignment or kind mask is outside the stock-proven subset.");
        int index = 0;
        foreach (XmlNode child in filter.ChildNodes)
        {
            if (child is XmlElement element)
            {
                if (element.LocalName != "IncludeThing" || element.NamespaceURI != root.NamespaceURI || index >= 15)
                    throw new NotSupportedException("Experimental filters accept at most fifteen IncludeThing weak leaves.");
                CheckAttributes(element, Array.Empty<string>());
                foreach (XmlNode value in element.ChildNodes)
                    if (value is not XmlText && value is not XmlWhitespace && value is not XmlSignificantWhitespace && value is not XmlComment)
                        throw new NotSupportedException("Weak filter leaves cannot contain nested controls.");
                string name = element.InnerText;
                if (string.IsNullOrWhiteSpace(name) || name != name.Trim() || name.IndexOfAny(new[] { ':', '\\', '=' }) >= 0
                    || index >= instance.WeakReferencedInstances.Count)
                    throw new NotSupportedException("Filter weak leaf must be a core-normalized name with matching metadata.");
                InstanceHandle actual = instance.WeakReferencedInstances[index++];
                InstanceHandle expected = new("GameObject", name);
                if (actual.TypeName != "GameObject" || actual.TypeId != expected.TypeId || actual.InstanceId != expected.InstanceId)
                    throw new NotSupportedException("Filter weak metadata name/type/order does not match the normalized leaf.");
            }
            else CheckWhitespace(child);
        }
        if (index != instance.WeakReferencedInstances.Count) throw new NotSupportedException("Unconsumed filter weak dependency metadata.");
        // Reborn: TypeId insertion invalidates old validity flags; check injected identities then validate a detached current-value copy.
        XmlDocument copy = new() { XmlResolver = null };
        copy.Schemas.Add(root.OwnerDocument.Schemas);
        copy.LoadXml(root.OuterXml);
        foreach (XmlElement element in copy.DocumentElement.SelectNodes("descendant-or-self::*")) element.RemoveAttribute("TypeId");
        copy.Validate((_, args) => throw new XmlSchemaValidationException(args.Message));
        XmlNamespaceManager namespaces = new(copy.NameTable);
        namespaces.AddNamespace("ea", root.NamespaceURI);
        Node node = new(copy.DocumentElement.CreateNavigator(), namespaces);
        ObjectFilterAsset* nativeRoot;
        using Tracker tracker = new((void**)&nativeRoot, (uint)sizeof(ObjectFilterAsset), false);
        Marshaler.Marshal(node, nativeRoot, tracker);
        Chunk chunk = new();
        if (!tracker.MakeRelocatable(chunk) || chunk.ImportsBuffer.Length != 0)
            throw new InvalidOperationException("Experimental weak filter generated strong imports or failed native marshalling.");
        return new AssetBuffer { InstanceData = chunk.InstanceBuffer, RelocationData = chunk.RelocationBuffer, ImportsData = chunk.ImportsBuffer };
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: accept only bounded attributes and exact schema-bound injected TypeId values, rejecting unresolved expressions. */
    //-------------------------------------------------------------------------------------------------
    private static void CheckAttributes(XmlElement element, string[] allowed)
    {
        foreach (XmlAttribute attribute in element.Attributes)
        {
            if (attribute.NamespaceURI == "http://www.w3.org/2000/xmlns/") continue;
            if (attribute.NamespaceURI.Length == 0 && attribute.LocalName == "TypeId"
                && element.SchemaInfo.SchemaType?.Name is string schemaType
                && uint.TryParse(attribute.Value, NumberStyles.None, CultureInfo.InvariantCulture, out uint injected)
                && injected == FastHash.GetHashCode(schemaType)) continue;
            if (attribute.NamespaceURI.Length != 0 || Array.IndexOf(allowed, attribute.LocalName) < 0
                || (attribute.LocalName != "id" && attribute.Value.TrimStart().StartsWith("=", StringComparison.Ordinal)))
                throw new NotSupportedException($"Unsupported experimental filter attribute/control '{attribute.Name}'.");
        }
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: ignore formatting/comments but never silently discard non-whitespace control content. */
    //-------------------------------------------------------------------------------------------------
    private static void CheckWhitespace(XmlNode node)
    {
        if (node is not XmlComment && !string.IsNullOrWhiteSpace(node.InnerText))
            throw new NotSupportedException("Experimental filter does not support text controls.");
    }
}
