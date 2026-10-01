using System;
using System.Globalization;
using System.Xml;
using System.Xml.Schema;
using BinaryAssetBuilder.Core;
using BinaryAssetBuilder.Core.Hashing;
using Relo;
using SageBinaryData;

namespace BinaryAssetBuilder.XmlCompiler;

// Reborn: isolate stock-proven empty/sound FX roots from unproven nuggets, production output and compiled-document reuse.
public sealed class Ra3Ep1FXListPlugin : IAssetBuilderPlugin, IAssetBuilderOutputPolicy
{
    private bool _initialized;
    public string ProfileName => "RA3EP1-FXList-Experimental-v1";
    public bool CanWriteProductionOutput => false;
    public bool CanUseBuildCache => false;
    public bool CanReuseCompiledDocuments => false;
    public uint AllTypesHash => 0x5454A8E9u;
    public uint VersionNumber => 1;

    //-------------------------------------------------------------------------------------------------
    /** Reborn: revoke readiness before accepting only the recovered Win32 native ABI. */
    //-------------------------------------------------------------------------------------------------
    public void Initialize(TargetPlatform platform)
    {
        _initialized = false;
        if (platform != TargetPlatform.Win32) throw new NotSupportedException("Experimental EP1 FX supports only Win32.");
        _initialized = true;
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: retain the same readiness/platform checks on reinitialization. */
    //-------------------------------------------------------------------------------------------------
    public void ReInitialize(TargetPlatform platform) => Initialize(platform);

    //-------------------------------------------------------------------------------------------------
    /** Reborn: return fresh observed FX metadata in a private processing domain, never the legacy KW type table. */
    //-------------------------------------------------------------------------------------------------
    public ExtendedTypeInformation GetExtendedTypeInformation(uint typeId)
    {
        if (!_initialized) throw new InvalidOperationException("Experimental FX profile is not initialized.");
        if (typeId != 0x86682E78u) throw new NotSupportedException("Unsupported experimental FX type.");
        return new ExtendedTypeInformation { TypeId = typeId, TypeName = nameof(FXList), Type = typeof(FXList),
            TypeHash = 0x17B3B82Du, ProcessingHash = 0x17B3B82Du ^ 0x45503141u,
            Tokenized = false, HasCustomData = false, UseBuildCache = false };
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: marshal only checked current empty/sound declarations with ordered concrete AudioEvent/Multisound imports. */
    //-------------------------------------------------------------------------------------------------
    public unsafe AssetBuffer ProcessInstance(InstanceDeclaration instance)
    {
        if (instance?.Node is not XmlElement root) throw new NotSupportedException("A validated FX root is required.");
        GetExtendedTypeInformation(instance.Handle.TypeId);
        CheckElement(root, "FXList", nameof(FXList), new[] { "id", "PlayEvenIfShrouded", "Tailorable" });
        if (root.OwnerDocument.Schemas.Count == 0 || instance.Handle.TypeHash != 0x17B3B82Du
            || InstanceHandle.GetInstanceId(root.GetAttribute("id")) != instance.Handle.InstanceId
            || instance.InheritFromHandle != null || instance.HasCustomData || instance.WeakReferencedInstances.Count != 0
            || instance.ReferencedFiles.Count != 0 || instance.ValidatedReferencedInstances == null
            || instance.ValidatedReferencedInstances.Count != instance.ReferencedInstances.Count)
            throw new NotSupportedException("FX requires current schema-bound identity and completed strong dependency preparation, without inheritance/custom/weak/file metadata.");
        CheckFalse(root, "PlayEvenIfShrouded"); CheckFalse(root, "Tailorable");
        XmlElement nuggets = null;
        foreach (XmlNode child in root.ChildNodes)
        {
            if (child is XmlElement element)
            {
                if (nuggets != null) throw new NotSupportedException("FX requires exactly one NuggetList.");
                CheckElement(element, "NuggetList", "FXNuggetTypes", Array.Empty<string>()); nuggets = element;
            }
            else CheckWhitespace(child);
        }
        if (nuggets == null) throw new NotSupportedException("FX requires a NuggetList.");
        int count = 0;
        foreach (XmlNode child in nuggets.ChildNodes)
        {
            if (child is not XmlElement sound) { CheckWhitespace(child); continue; }
            if (count >= 2) throw new NotSupportedException("Experimental FX admits at most two Sound nuggets.");
            CheckElement(sound, "Sound", nameof(SoundFXNugget), new[] { "Value", "RequiredSourceModelConditions",
                "ExcludedSourceModelConditions", "Weather", "StopIfPlayed", "OnlyIfOnLand", "PlayIfSourceIsStealthed" });
            foreach (XmlNode value in sound.ChildNodes) CheckWhitespace(value);
            CheckFalse(sound, "StopIfPlayed"); CheckFalse(sound, "OnlyIfOnLand"); CheckFalse(sound, "PlayIfSourceIsStealthed");
            if (sound.GetAttribute("Weather") != "INVALID") throw new NotSupportedException("Only stock-proven INVALID FX weather is admitted.");
            int masks = 0;
            foreach (string field in new[] { "RequiredSourceModelConditions", "ExcludedSourceModelConditions" })
                if (sound.HasAttribute(field))
                {
                    if (++masks > 1 || sound.GetAttribute(field) != "FLYING") throw new NotSupportedException("Only one stock-proven FLYING source mask per Sound is admitted.");
                }
            CheckImport(sound.GetAttribute("Value"), instance, count++);
        }
        if (count != instance.ReferencedInstances.Count) throw new NotSupportedException("FX has unused dependency slots.");
        // Reborn: validate current values in a detached copy, then restore schema-derived polymorphic IDs for native dispatch.
        XmlDocument copy = new() { XmlResolver = null }; copy.Schemas.Add(root.OwnerDocument.Schemas); copy.LoadXml(root.OuterXml);
        foreach (XmlElement element in copy.DocumentElement.SelectNodes("descendant-or-self::*")) element.RemoveAttribute("TypeId");
        copy.Validate((_, args) => throw new XmlSchemaValidationException(args.Message));
        foreach (XmlElement element in copy.DocumentElement.SelectNodes("descendant-or-self::*"))
            element.SetAttribute("TypeId", FastHash.GetHashCode(element.SchemaInfo.SchemaType.Name).ToString(CultureInfo.InvariantCulture));
        XmlNamespaceManager namespaces = new(copy.NameTable); namespaces.AddNamespace("ea", root.NamespaceURI);
        Node node = new(copy.DocumentElement.CreateNavigator(), namespaces);
        FXList* nativeRoot;
        using Tracker tracker = new((void**)&nativeRoot, (uint)sizeof(FXList), false);
        Marshaler.Marshal(node, nativeRoot, tracker);
        Chunk chunk = new();
        if (!tracker.MakeRelocatable(chunk) || chunk.ImportsBuffer.Length != (count == 0 ? 0 : (count + 1) * 4))
            throw new InvalidOperationException("FX native marshalling or import count differs.");
        return new AssetBuffer { InstanceData = chunk.InstanceBuffer, RelocationData = chunk.RelocationBuffer, ImportsData = chunk.ImportsBuffer };
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: match each normalized selector to original and concretely resolved metadata without widening explicit sibling types. */
    //-------------------------------------------------------------------------------------------------
    private static void CheckImport(string value, InstanceDeclaration instance, int ordinal)
    {
        int separator = value.LastIndexOf('\\');
        if (separator <= 0 || value.IndexOf('\\') != separator
            || !uint.TryParse(value.Substring(separator + 1), NumberStyles.None, CultureInfo.InvariantCulture, out uint index)
            || index != ordinal || index >= instance.ReferencedInstances.Count)
            throw new NotSupportedException("Sound requires its ordered core-normalized selector.");
        string name = value.Substring(0, separator);
        int colon = name.IndexOf(':');
        if (name != name.Trim() || colon == 0 || colon == name.Length - 1 || colon != name.LastIndexOf(':'))
            throw new NotSupportedException("Sound normalized asset name is invalid.");
        InstanceHandle expected = new(name);
        if (expected.TypeId == 0) expected.TypeName = "BaseAudioEventInfo";
        InstanceHandle original = instance.ReferencedInstances[ordinal], concrete = instance.ValidatedReferencedInstances[ordinal];
        if (original == null || concrete == null
            || expected.TypeName != "BaseAudioEventInfo" && expected.TypeName != "AudioEvent" && expected.TypeName != "Multisound"
            || original.TypeName != expected.TypeName || original.TypeId != expected.TypeId || original.InstanceId != expected.InstanceId
            || concrete.InstanceId != expected.InstanceId || concrete.TypeId != new InstanceHandle(concrete.TypeName, concrete.InstanceName).TypeId
            || concrete.InstanceId != InstanceHandle.GetInstanceId(concrete.InstanceName)
            || concrete.TypeName != "AudioEvent" && concrete.TypeName != "Multisound"
            || expected.TypeName != "BaseAudioEventInfo" && expected.TypeName != concrete.TypeName)
            throw new NotSupportedException("Sound concrete audio identity does not match its normalized dependency slot.");
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: accept only matching official schema types, bounded attributes and intact core-injected identities. */
    //-------------------------------------------------------------------------------------------------
    private static void CheckElement(XmlElement element, string name, string type, string[] allowed)
    {
        if (element.LocalName != name || element.NamespaceURI != "uri:ea.com:eala:asset" || element.SchemaInfo.SchemaType?.Name != type)
            throw new NotSupportedException("Unsupported or unvalidated FX element.");
        foreach (XmlAttribute attribute in element.Attributes)
        {
            if (attribute.NamespaceURI == "http://www.w3.org/2000/xmlns/") continue;
            if (attribute.NamespaceURI.Length == 0 && attribute.LocalName == "TypeId"
                && uint.TryParse(attribute.Value, NumberStyles.None, CultureInfo.InvariantCulture, out uint injected)
                && injected == FastHash.GetHashCode(type)) continue;
            if (attribute.NamespaceURI.Length != 0 || Array.IndexOf(allowed, attribute.LocalName) < 0
                || attribute.Value.TrimStart().StartsWith("=", StringComparison.Ordinal))
                throw new NotSupportedException("Unsupported FX attribute or unresolved formula.");
        }
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: restrict native booleans to stock-proven false values rather than admitting untested control branches. */
    //-------------------------------------------------------------------------------------------------
    private static void CheckFalse(XmlElement element, string name)
    {
        if (element.HasAttribute(name) && element.GetAttribute(name) != "false" && element.GetAttribute(name) != "0")
            throw new NotSupportedException("Experimental FX boolean controls must be false.");
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: reject nested controls, processing instructions and non-whitespace text before native marshalling. */
    //-------------------------------------------------------------------------------------------------
    private static void CheckWhitespace(XmlNode node)
    {
        if (node is XmlComment || ((node is XmlText || node is XmlWhitespace || node is XmlSignificantWhitespace)
            && string.IsNullOrWhiteSpace(node.InnerText))) return;
        throw new NotSupportedException("FX supports only comments and whitespace outside admitted elements.");
    }
}
