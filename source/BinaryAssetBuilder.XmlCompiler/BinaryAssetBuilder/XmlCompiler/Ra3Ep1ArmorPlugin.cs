using System;
using System.Globalization;
using System.Xml;
using BinaryAssetBuilder.Core;
using BinaryAssetBuilder.Core.Hashing;
using Relo;
using SageBinaryData;

namespace BinaryAssetBuilder.XmlCompiler;

// Reborn: opt-in experimental EP1 processor; never reuse the KW plugin's static registry or dispatch cache.
public sealed class Ra3Ep1ArmorPlugin : IAssetBuilderPlugin, IAssetBuilderOutputPolicy
{
    private bool _initialized;
    public string ProfileName => "RA3EP1-Armor-Experimental-v1";
    public bool CanWriteProductionOutput => false;
    public bool CanUseBuildCache => false;
    public bool CanReuseCompiledDocuments => false;
    public uint AllTypesHash => 0x5454A8E9u;
    public uint VersionNumber => 1;

    //-------------------------------------------------------------------------------------------------
    /** Reborn: invalidate previous initialization before rejecting unsupported platforms. */
    //-------------------------------------------------------------------------------------------------
    public void Initialize(TargetPlatform platform)
    {
        _initialized = false;
        if (platform != TargetPlatform.Win32)
            throw new NotSupportedException("The experimental EP1 armor profile supports only Win32.");
        _initialized = true;
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: reinitialization has the same fail-closed platform rules and no shared mutable caches. */
    //-------------------------------------------------------------------------------------------------
    public void ReInitialize(TargetPlatform platform) => Initialize(platform);

    //-------------------------------------------------------------------------------------------------
    /** Reborn: expose only the golden-validated armor identity, with a project-local processor revision stamp. */
    //-------------------------------------------------------------------------------------------------
    public ExtendedTypeInformation GetExtendedTypeInformation(uint typeId)
    {
        if (!_initialized) throw new InvalidOperationException("EP1 armor profile has not been initialized for Win32.");
        if (typeId != 0x3A6C5E8Eu)
            throw new NotSupportedException($"EP1 armor profile does not support type 0x{typeId:X8}; no KW fallback is allowed.");
        return new ExtendedTypeInformation
        {
            TypeId = typeId, TypeName = nameof(ArmorTemplate), Type = typeof(ArmorTemplate), TypeHash = 0xA0E237D8u,
            // Reborn: this stamp is a local cache-domain revision, not EA's runtime TypeHash derivation.
            ProcessingHash = 0xA0E237D8u ^ 0x45503101u, Tokenized = true, HasCustomData = false, UseBuildCache = false
        };
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: process a normalized standalone armor declaration through native marshalling and EP1 tokenization. */
    //-------------------------------------------------------------------------------------------------
    public unsafe AssetBuffer ProcessInstance(InstanceDeclaration instance)
    {
        if (instance?.Node is not XmlElement root) throw new ArgumentException("An armor XML element is required.", nameof(instance));
        GetExtendedTypeInformation(instance.Handle.TypeId);
        if (root.LocalName != nameof(ArmorTemplate) || root.NamespaceURI != "uri:ea.com:eala:asset"
            || instance.InheritFromHandle != null || instance.HasCustomData
            || instance.ReferencedInstances.Count != 0 || instance.WeakReferencedInstances.Count != 0)
            throw new NotSupportedException("Experimental EP1 armor accepts only standalone armor without inheritance, custom data or dependencies.");
        CheckAttributes(root, new[] { "id", "Default", "DamageScalar", "SideDamageScalar", "RearDamageScalar", "FlankedPenalty" });
        foreach (XmlNode child in root.ChildNodes)
        {
            if (child is XmlElement element)
            {
                if (element.LocalName != "Armor" || element.NamespaceURI != root.NamespaceURI || element.HasChildNodes)
                    throw new NotSupportedException("Experimental EP1 armor accepts only leaf Armor entries.");
                CheckAttributes(element, new[] { "Damage", "Percent" });
                if (element.GetAttribute("Damage") == "ALL") throw new NotSupportedException("Damage=ALL is not validated by the EP1 armor profile.");
            }
            else if (child is not XmlComment && !string.IsNullOrWhiteSpace(child.InnerText))
                throw new NotSupportedException("Armor does not accept text content.");
        }
        XmlNamespaceManager namespaces = new(root.OwnerDocument.NameTable);
        namespaces.AddNamespace("ea", root.NamespaceURI);
        Node node = new(root.CreateNavigator(), namespaces);
        ArmorTemplate* nativeRoot;
        using Tracker tracker = new((void**)&nativeRoot, (uint)sizeof(ArmorTemplate), false);
        Marshaler.Marshal(node, nativeRoot, tracker);
        Chunk native = new();
        if (!tracker.MakeRelocatable(native)) throw new InvalidOperationException("Native EP1 armor marshalling failed.");
        Chunk tokenized = Ep1ArmorTokenizer.Tokenize(native);
        return new AssetBuffer { InstanceData = tokenized.InstanceBuffer, RelocationData = tokenized.RelocationBuffer, ImportsData = tokenized.ImportsBuffer };
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: refuse unsupported XML controls and unevaluated formulas instead of silently dropping their semantics. */
    //-------------------------------------------------------------------------------------------------
    private static void CheckAttributes(XmlElement element, string[] allowed)
    {
        foreach (XmlAttribute attribute in element.Attributes)
        {
            if (attribute.NamespaceURI == "http://www.w3.org/2000/xmlns/") continue;
            // Reborn: the core injects TypeId after validation; accept only the exact validated schema type identity.
            if (attribute.NamespaceURI.Length == 0 && attribute.LocalName == "TypeId"
                && element.SchemaInfo?.SchemaType?.Name is string schemaType
                && uint.TryParse(attribute.Value, NumberStyles.None, CultureInfo.InvariantCulture, out uint injectedType)
                && injectedType == FastHash.GetHashCode(schemaType)) continue;
            if (attribute.NamespaceURI.Length != 0 || Array.IndexOf(allowed, attribute.LocalName) < 0
                || (attribute.LocalName != "id" && attribute.Value.TrimStart().StartsWith("=", StringComparison.Ordinal)))
                throw new NotSupportedException($"EP1 armor profile does not support attribute '{attribute.Name}' or its unevaluated formula.");
        }
    }
}
