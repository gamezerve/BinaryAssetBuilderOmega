using System;
using System.Globalization;
using System.Xml;
using System.Xml.Schema;
using BinaryAssetBuilder.Core;
using BinaryAssetBuilder.Core.Hashing;
using Relo;
using SageBinaryData;

namespace BinaryAssetBuilder.XmlCompiler;

// Reborn: isolate golden-validated EP1 modifier subsets from the legacy KW registry and all production/cache writes.
public sealed class Ra3Ep1AttributeModifierPlugin : IAssetBuilderPlugin, IAssetBuilderOutputPolicy
{
    private bool _initialized;
    private readonly bool _allowNormalizedImports;
    public string ProfileName => _allowNormalizedImports ? "RA3EP1-AttributeModifier-Imports-Experimental-v2" : "RA3EP1-AttributeModifier-Experimental-v1";
    public bool CanWriteProductionOutput => false;
    public bool CanUseBuildCache => false;
    public bool CanReuseCompiledDocuments => false;
    public uint AllTypesHash => 0x5454A8E9u;
    public uint VersionNumber => _allowNormalizedImports ? 2u : 1u;

    //-------------------------------------------------------------------------------------------------
    /** Reborn: descriptor activation keeps the original no-dependency subset; imports require explicit diagnostic construction. */
    //-------------------------------------------------------------------------------------------------
    public Ra3Ep1AttributeModifierPlugin() : this(false) { }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: opt in to proven normalized FXList/ShaderOverride imports without relaxing any output/cache/platform policy. */
    //-------------------------------------------------------------------------------------------------
    public Ra3Ep1AttributeModifierPlugin(bool allowNormalizedImports) => _allowNormalizedImports = allowNormalizedImports;

    //-------------------------------------------------------------------------------------------------
    /** Reborn: only Win32 initialization enables the bounded native modifier processor; failures revoke prior readiness. */
    //-------------------------------------------------------------------------------------------------
    public void Initialize(TargetPlatform platform)
    {
        _initialized = false;
        if (platform != TargetPlatform.Win32) throw new NotSupportedException("Experimental EP1 modifiers support only Win32.");
        _initialized = true;
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: apply the same platform gate on reinitialization without static registrations or cached dispatch. */
    //-------------------------------------------------------------------------------------------------
    public void ReInitialize(TargetPlatform platform) => Initialize(platform);

    //-------------------------------------------------------------------------------------------------
    /** Reborn: expose observed native EP1 identity with a distinct local processing/cache-domain stamp. */
    //-------------------------------------------------------------------------------------------------
    public ExtendedTypeInformation GetExtendedTypeInformation(uint typeId)
    {
        if (!_initialized) throw new InvalidOperationException("Experimental EP1 modifier profile is not initialized for Win32.");
        if (typeId != 0xC5E07887u) throw new NotSupportedException($"Unsupported EP1 modifier profile type 0x{typeId:X8}; no KW fallback is allowed.");
        return new ExtendedTypeInformation
        {
            TypeId = typeId, TypeName = nameof(AttributeModifier), Type = typeof(AttributeModifier), TypeHash = 0x74425C11u,
            // Reborn: this local revision distinguishes the experimental processor, not EA's runtime type-hash generation.
            ProcessingHash = 0x74425C11u ^ (_allowNormalizedImports ? 0x45503112u : 0x45503111u), Tokenized = false, HasCustomData = false, UseBuildCache = false
        };
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: marshal validated standalone modifiers, admitting proven imports only through explicit identity-checked opt-in. */
    //-------------------------------------------------------------------------------------------------
    public unsafe AssetBuffer ProcessInstance(InstanceDeclaration instance)
    {
        if (instance?.Node is not XmlElement root) throw new ArgumentException("A modifier XML element is required.", nameof(instance));
        GetExtendedTypeInformation(instance.Handle.TypeId);
        if (root.LocalName != nameof(AttributeModifier) || root.NamespaceURI != "uri:ea.com:eala:asset"
            || root.SchemaInfo.SchemaType?.Name != nameof(AttributeModifier) || root.OwnerDocument.Schemas.Count == 0
            || instance.InheritFromHandle != null || instance.HasCustomData
            || (!_allowNormalizedImports && instance.ReferencedInstances.Count != 0)
            || instance.WeakReferencedInstances.Count != 0 || instance.ReferencedFiles.Count != 0)
            throw new NotSupportedException("Experimental EP1 modifiers require validated standalone XML with only explicitly enabled normalized imports and no inheritance.");
        string[] allowed = _allowNormalizedImports
            ? new[] { "id", "Category", "Duration", "ReplaceInCategoryIfLongest", "IgnoreIfAnticategoryActive",
                "ModelConditionsSet", "ModelConditionsClear", "ObjectStatusToSet", "StackingLimit", "ArmorSetType", "StartFX", "EndFX", "Shader" }
            : new[] { "id", "Category", "Duration", "ReplaceInCategoryIfLongest", "IgnoreIfAnticategoryActive",
                "ModelConditionsSet", "ModelConditionsClear", "ObjectStatusToSet", "StackingLimit", "ArmorSetType" };
        CheckAttributes(root, allowed);
        // Reborn: never trust a parseable numeric suffix without checking its ordered dependency identity.
        if (_allowNormalizedImports) CheckNormalizedImports(root, instance);
        foreach (XmlNode child in root.ChildNodes)
        {
            if (child is XmlElement element)
            {
                if (element.LocalName != "Modifier" || element.NamespaceURI != root.NamespaceURI
                    || element.HasChildNodes)
                    throw new NotSupportedException("Experimental EP1 modifiers accept only leaf Modifier records.");
                CheckAttributes(element, new[] { "Type", "Value" });
            }
            else if (child is not XmlComment && !string.IsNullOrWhiteSpace(child.InnerText))
                throw new NotSupportedException("Modifiers do not accept text content.");
        }
        // Reborn: core TypeId insertion invalidates XML validity flags; revalidate a detached copy after checking those injected identities.
        root = CopyValidatedRoot(root);
        XmlNamespaceManager namespaces = new(root.OwnerDocument.NameTable);
        namespaces.AddNamespace("ea", root.NamespaceURI);
        Node node = new(root.CreateNavigator(), namespaces);
        AttributeModifier* nativeRoot;
        using Tracker tracker = new((void**)&nativeRoot, (uint)sizeof(AttributeModifier), false);
        Marshaler.Marshal(node, nativeRoot, tracker);
        Chunk native = new();
        int expectedImports = instance.ReferencedInstances.Count == 0 ? 0 : (instance.ReferencedInstances.Count + 1) * 4;
        if (!tracker.MakeRelocatable(native) || native.ImportsBuffer.Length != expectedImports)
            throw new InvalidOperationException("Standalone EP1 modifier marshalling failed or import count differed from checked dependencies.");
        return new AssetBuffer { InstanceData = native.InstanceBuffer, RelocationData = native.RelocationBuffer, ImportsData = native.ImportsBuffer };
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: verify current values against the caller's schema set without mutating normalized source XML or accepting stale validity flags. */
    //-------------------------------------------------------------------------------------------------
    private static XmlElement CopyValidatedRoot(XmlElement root)
    {
        XmlDocument copy = new() { XmlResolver = null };
        copy.Schemas.Add(root.OwnerDocument.Schemas);
        copy.LoadXml(root.OuterXml);
        copy.DocumentElement.RemoveAttribute("TypeId");
        foreach (XmlNode child in copy.DocumentElement.ChildNodes)
            if (child is XmlElement element) element.RemoveAttribute("TypeId");
        copy.Validate((_, args) => throw new XmlSchemaValidationException(args.Message));
        return copy.DocumentElement;
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: require each normalized index exactly once and match its name/type to the current ordered strong dependency table. */
    //-------------------------------------------------------------------------------------------------
    private static void CheckNormalizedImports(XmlElement root, InstanceDeclaration instance)
    {
        int count = instance.ReferencedInstances.Count;
        if (count > 3) throw new NotSupportedException("Modifier import subset has at most three strong slots.");
        bool[] used = new bool[count];
        foreach (string attributeName in new[] { "StartFX", "EndFX", "Shader" })
        {
            if (!root.HasAttribute(attributeName)) continue;
            string text = root.GetAttribute(attributeName);
            int separator = text.LastIndexOf('\\');
            if (separator <= 0 || text.IndexOf('\\') != separator
                || !uint.TryParse(text.Substring(separator + 1), NumberStyles.None, CultureInfo.InvariantCulture, out uint index)
                || index >= count || used[index])
                throw new NotSupportedException($"Modifier '{attributeName}' must contain a unique bounded core-normalized dependency index.");
            string type = attributeName == "Shader" ? "ShaderOverride" : "FXList";
            string name = text.Substring(0, separator);
            int colon = name.IndexOf(':');
            if (name != name.Trim() || colon == 0 || colon == name.Length - 1 || colon != name.LastIndexOf(':'))
                throw new NotSupportedException($"Modifier '{attributeName}' has an invalid normalized asset name.");
            InstanceHandle expected = new(name);
            if (expected.TypeId == 0) expected.TypeName = type;
            InstanceHandle actual = instance.ReferencedInstances[(int)index];
            if (expected.TypeName != type || actual.TypeName != type || expected.TypeId != actual.TypeId
                || expected.InstanceId != actual.InstanceId)
                throw new NotSupportedException($"Modifier '{attributeName}' name/type does not match normalized dependency {index}.");
            used[index] = true;
        }
        if (Array.IndexOf(used, false) >= 0)
            throw new NotSupportedException("Modifier dependency table contains unconsumed slots.");
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: reject unvalidated attributes and formulas; accept only exact schema-bound core-injected TypeId values. */
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
                throw new NotSupportedException($"EP1 modifier profile does not support attribute '{attribute.Name}' or its unresolved formula.");
        }
    }
}
