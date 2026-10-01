using System;
using System.Globalization;
using System.Xml;
using System.Xml.Schema;
using BinaryAssetBuilder.Core;
using BinaryAssetBuilder.Core.Hashing;
using Relo;
using SageBinaryData;

namespace BinaryAssetBuilder.XmlCompiler;

// Reborn: isolate bounded native shader rules from the KW registry, material compilation and production/cache output.
public sealed class Ra3Ep1ShaderOverridePlugin : IAssetBuilderPlugin, IAssetBuilderOutputPolicy
{
    private bool _initialized;
    public string ProfileName => "RA3EP1-ShaderOverride-Experimental-v1";
    public bool CanWriteProductionOutput => false;
    public bool CanUseBuildCache => false;
    public bool CanReuseCompiledDocuments => false;
    public uint AllTypesHash => 0x5454A8E9u;
    public uint VersionNumber => 1;

    //-------------------------------------------------------------------------------------------------
    /** Reborn: admit only the recovered Win32 ABI and revoke readiness before rejecting any other platform. */
    //-------------------------------------------------------------------------------------------------
    public void Initialize(TargetPlatform platform)
    {
        _initialized = false;
        if (platform != TargetPlatform.Win32) throw new NotSupportedException("Experimental EP1 shaders support only Win32.");
        _initialized = true;
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: preserve platform and readiness checks without shared mutable plugin state. */
    //-------------------------------------------------------------------------------------------------
    public void ReInitialize(TargetPlatform platform) => Initialize(platform);

    //-------------------------------------------------------------------------------------------------
    /** Reborn: expose fresh native shader metadata in an isolated processing domain without falling back to KW types. */
    //-------------------------------------------------------------------------------------------------
    public ExtendedTypeInformation GetExtendedTypeInformation(uint typeId)
    {
        if (!_initialized) throw new InvalidOperationException("Experimental EP1 shader profile is not initialized.");
        if (typeId != 0xBCC23F6Cu) throw new NotSupportedException($"Unsupported experimental shader type 0x{typeId:X8}.");
        return new ExtendedTypeInformation { TypeId = typeId, TypeName = nameof(ShaderOverride), Type = typeof(ShaderOverride),
            TypeHash = 0x3D5B1D16u, ProcessingHash = 0x3D5B1D16u ^ 0x45503131u,
            Tokenized = false, HasCustomData = false, UseBuildCache = false };
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: compile only standalone schema-bound literal rules with checked identity, Default techniques and no dependency metadata. */
    //-------------------------------------------------------------------------------------------------
    public unsafe AssetBuffer ProcessInstance(InstanceDeclaration instance)
    {
        if (instance?.Node is not XmlElement root) throw new ArgumentException("A shader root is required.", nameof(instance));
        GetExtendedTypeInformation(instance.Handle.TypeId);
        if (root.LocalName != nameof(ShaderOverride) || root.NamespaceURI != "uri:ea.com:eala:asset"
            || root.SchemaInfo.SchemaType?.Name != nameof(ShaderOverride) || root.OwnerDocument.Schemas.Count == 0
            || instance.InheritFromHandle != null || instance.HasCustomData || instance.ReferencedInstances.Count != 0
            || instance.WeakReferencedInstances.Count != 0 || instance.ReferencedFiles.Count != 0)
            throw new NotSupportedException("Experimental shaders require validated standalone XML without inheritance or strong/weak/file dependencies.");
        CheckAttributes(root, new[] { "id", "Priority" });
        if (InstanceHandle.GetInstanceId(root.GetAttribute("id")) != instance.Handle.InstanceId)
            throw new NotSupportedException("Shader XML id does not match its declaration identity.");
        int count = 0;
        foreach (XmlNode child in root.ChildNodes)
        {
            if (child is XmlElement rule)
            {
                if (rule.LocalName != "Rule" || rule.NamespaceURI != root.NamespaceURI
                    || rule.SchemaInfo.SchemaType?.Name != nameof(ShaderOverrideRule) || ++count > 16)
                    throw new NotSupportedException("Experimental shaders accept at most sixteen schema-bound rules.");
                CheckAttributes(rule, new[] { "IfOriginalShaderIs", "ReplaceShaderName", "ReplaceTechniqueName" });
                if (rule.HasAttribute("IfOriginalShaderIs")) CheckMaterialName(rule.GetAttribute("IfOriginalShaderIs"));
                CheckMaterialName(rule.GetAttribute("ReplaceShaderName"));
                if (rule.GetAttribute("ReplaceTechniqueName") != "Default")
                    throw new NotSupportedException("Experimental shader techniques are limited to the stock-proven Default literal.");
                foreach (XmlNode value in rule.ChildNodes) CheckWhitespace(value);
            }
            else CheckWhitespace(child);
        }
        if (count == 0) throw new NotSupportedException("Experimental shaders require at least one rule.");
        // Reborn: current values must be revalidated after checking core-injected TypeIds; never mutate the caller's normalized XML.
        XmlDocument copy = new() { XmlResolver = null };
        copy.Schemas.Add(root.OwnerDocument.Schemas);
        copy.LoadXml(root.OuterXml);
        foreach (XmlElement element in copy.DocumentElement.SelectNodes("descendant-or-self::*")) element.RemoveAttribute("TypeId");
        copy.Validate((_, args) => throw new XmlSchemaValidationException(args.Message));
        XmlNamespaceManager namespaces = new(copy.NameTable); namespaces.AddNamespace("ea", root.NamespaceURI);
        Node node = new(copy.DocumentElement.CreateNavigator(), namespaces);
        ShaderOverride* nativeRoot;
        using Tracker tracker = new((void**)&nativeRoot, (uint)sizeof(ShaderOverride), false);
        Marshaler.Marshal(node, nativeRoot, tracker);
        Chunk chunk = new();
        if (!tracker.MakeRelocatable(chunk) || chunk.ImportsBuffer.Length != 0)
            throw new InvalidOperationException("Experimental shader produced imports or failed native marshalling.");
        return new AssetBuffer { InstanceData = chunk.InstanceBuffer, RelocationData = chunk.RelocationBuffer, ImportsData = chunk.ImportsBuffer };
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: accept only short ASCII material basename literals, never normalized imports, paths, formulas or arbitrary string controls. */
    //-------------------------------------------------------------------------------------------------
    private static void CheckMaterialName(string value)
    {
        if (value.Length <= 3 || value.Length > 128 || !value.EndsWith(".fx", StringComparison.Ordinal))
            throw new NotSupportedException("Experimental shader material must be a bounded .fx basename.");
        for (int index = 0; index < value.Length - 3; index++)
        {
            char character = value[index];
            if (!(character >= 'A' && character <= 'Z') && !(character >= 'a' && character <= 'z')
                && !(character >= '0' && character <= '9') && character != '_')
                throw new NotSupportedException("Experimental shader material contains unsupported controls.");
        }
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: restrict attributes and verify schema-bound injected type identities before detached current-value validation. */
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
                throw new NotSupportedException($"Unsupported experimental shader attribute/control '{attribute.Name}'.");
        }
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: permit only comments and whitespace, rejecting even empty nested elements or unsupported processing instructions. */
    //-------------------------------------------------------------------------------------------------
    private static void CheckWhitespace(XmlNode node)
    {
        if (node is XmlComment || ((node is XmlText || node is XmlWhitespace || node is XmlSignificantWhitespace)
            && string.IsNullOrWhiteSpace(node.InnerText))) return;
        throw new NotSupportedException("Experimental shader does not support nested or text controls.");
    }
}
