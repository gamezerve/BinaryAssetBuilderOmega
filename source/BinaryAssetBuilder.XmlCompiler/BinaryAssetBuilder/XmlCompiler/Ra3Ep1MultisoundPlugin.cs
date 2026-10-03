using System;
using System.Collections.Generic;
using System.Globalization;
using System.Xml;
using System.Xml.Schema;
using BinaryAssetBuilder.Core;
using BinaryAssetBuilder.Core.Hashing;
using Relo;

namespace BinaryAssetBuilder.XmlCompiler;

// Reborn: admit only the stock-proven default/weighted Multisound subset in an isolated profile, never production audio output.
public sealed class Ra3Ep1MultisoundPlugin : IAssetBuilderPlugin, IAssetBuilderOutputPolicy
{
    private bool _initialized;
    public string ProfileName => "RA3EP1-Multisound-Experimental-v1";
    public bool CanWriteProductionOutput => false;
    public bool CanUseBuildCache => false;
    public bool CanReuseCompiledDocuments => false;
    public uint AllTypesHash => 0x5454A8E9u;
    public uint VersionNumber => 1;

    //-------------------------------------------------------------------------------------------------
    /** Reborn: revoke readiness before accepting only the recovered Win32 sound ABI. */
    //-------------------------------------------------------------------------------------------------
    public void Initialize(TargetPlatform platform)
    {
        _initialized = false;
        if (platform != TargetPlatform.Win32) throw new NotSupportedException("Experimental Multisound supports only Win32.");
        _initialized = true;
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: retain platform and readiness checks during reinitialization. */
    //-------------------------------------------------------------------------------------------------
    public void ReInitialize(TargetPlatform platform) => Initialize(platform);

    //-------------------------------------------------------------------------------------------------
    /** Reborn: return fresh observed EP1 metadata in a separate processing domain, not the legacy registry. */
    //-------------------------------------------------------------------------------------------------
    public ExtendedTypeInformation GetExtendedTypeInformation(uint typeId)
    {
        if (!_initialized) throw new InvalidOperationException("Experimental Multisound profile is not initialized.");
        if (typeId != 0xA3A7AF37u) throw new NotSupportedException("Unsupported experimental sound type.");
        return new ExtendedTypeInformation { TypeId = typeId, TypeName = "Multisound", Type = typeof(Marshaler.Ep1Multisound),
            TypeHash = 0xF79C5A89u, ProcessingHash = 0xF79C5A89u ^ 0x45503141u,
            Tokenized = false, HasCustomData = false, UseBuildCache = false };
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: compile only current schema-bound weighted children with complete ordered concrete sound dependencies. */
    //-------------------------------------------------------------------------------------------------
    public unsafe AssetBuffer ProcessInstance(InstanceDeclaration instance)
    {
        if (instance?.Node is not XmlElement root) throw new NotSupportedException("A validated Multisound root is required.");
        GetExtendedTypeInformation(instance.Handle.TypeId);
        CheckElement(root, "Multisound", "Multisound", new[] { "id", "Control" });
        if (root.OwnerDocument.Schemas.Count == 0 || instance.Handle.TypeHash != 0xF79C5A89u
            || InstanceHandle.GetInstanceId(root.GetAttribute("id")) != instance.Handle.InstanceId
            || instance.InheritFromHandle != null || instance.HasCustomData || instance.WeakReferencedInstances.Count != 0
            || instance.ReferencedFiles.Count != 0 || instance.ValidatedReferencedInstances == null
            || instance.ValidatedReferencedInstances.Count != instance.ReferencedInstances.Count)
            throw new NotSupportedException("Multisound requires current schema-bound identity and complete strong dependency preparation without inheritance/custom/weak/file metadata.");
        string id = root.GetAttribute("id");
        if (id.Length == 0 || id.Length > 128 || id != id.Trim()) throw new NotSupportedException("Multisound root ID is not bounded.");
        if (root.GetAttribute("Control") != "" && root.GetAttribute("Control") != "PLAY_ONE")
            throw new NotSupportedException("Only stock-proven default/PLAY_ONE control is admitted.");
        int count = 0;
        HashSet<ulong> identities = new();
        foreach (XmlNode child in root.ChildNodes)
        {
            if (child is not XmlElement sound) { CheckWhitespace(child); continue; }
            if (count >= 32) throw new NotSupportedException("Experimental Multisound admits at most 32 children.");
            CheckElement(sound, "Subsound", "MultisoundSubsoundRef", new[] { "Weight" });
            foreach (XmlNode value in sound.ChildNodes)
                if (value is not XmlText && value is not XmlWhitespace && value is not XmlSignificantWhitespace && value is not XmlComment)
                    throw new NotSupportedException("Multisound child requires plain reference text.");
            CheckImport(sound.InnerText, instance, count);
            InstanceHandle concrete = instance.ValidatedReferencedInstances[count++];
            if (!identities.Add(((ulong)concrete.TypeId << 32) | concrete.InstanceId))
                throw new NotSupportedException("Duplicate concrete Multisound children are not admitted.");
        }
        if (count != instance.ReferencedInstances.Count) throw new NotSupportedException("Multisound has unused dependency slots.");
        // Reborn: validate current scalar/default values in a detached copy; stale PSVI cannot authorize a mutated weight or control.
        XmlDocument copy = new() { XmlResolver = null }; copy.Schemas.Add(root.OwnerDocument.Schemas); copy.LoadXml(root.OuterXml);
        foreach (XmlElement element in copy.DocumentElement.SelectNodes("descendant-or-self::*")) element.RemoveAttribute("TypeId");
        copy.Validate((_, args) => throw new XmlSchemaValidationException(args.Message));
        XmlNamespaceManager namespaces = new(copy.NameTable); namespaces.AddNamespace("ea", root.NamespaceURI);
        Marshaler.Ep1Multisound* nativeRoot;
        using Tracker tracker = new((void**)&nativeRoot, (uint)sizeof(Marshaler.Ep1Multisound), false);
        Marshaler.Marshal(new Node(copy.DocumentElement.CreateNavigator(), namespaces), nativeRoot, tracker);
        Chunk chunk = new();
        if (!tracker.MakeRelocatable(chunk) || chunk.InstanceBuffer.Length != 16 + count * 28
            || chunk.RelocationBuffer.Length != (count == 0 ? 0 : 8)
            || chunk.ImportsBuffer.Length != (count == 0 ? 0 : (count + 1) * 4))
            throw new InvalidOperationException("Multisound native marshalling or buffer shape differs.");
        return new AssetBuffer { InstanceData = chunk.InstanceBuffer, RelocationData = chunk.RelocationBuffer, ImportsData = chunk.ImportsBuffer };
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: match normalized ordinal selectors to original and concrete metadata without widening explicitly typed siblings. */
    //-------------------------------------------------------------------------------------------------
    private static void CheckImport(string value, InstanceDeclaration instance, int ordinal)
    {
        int separator = value.LastIndexOf('\\');
        if (separator <= 0 || value.IndexOf('\\') != separator
            || !uint.TryParse(value.Substring(separator + 1), NumberStyles.None, CultureInfo.InvariantCulture, out uint index)
            || index != ordinal || index >= instance.ReferencedInstances.Count)
            throw new NotSupportedException("Subsound requires its ordered core-normalized selector.");
        string name = value.Substring(0, separator); int colon = name.IndexOf(':');
        if (name != name.Trim() || colon == 0 || colon == name.Length - 1 || colon != name.LastIndexOf(':'))
            throw new NotSupportedException("Subsound normalized name is invalid.");
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
            throw new NotSupportedException("Subsound concrete identity does not match its normalized dependency slot.");
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: require official schema types, bounded attributes and intact compiler-injected type identities. */
    //-------------------------------------------------------------------------------------------------
    private static void CheckElement(XmlElement element, string name, string type, string[] allowed)
    {
        if (element.LocalName != name || element.NamespaceURI != "uri:ea.com:eala:asset" || element.SchemaInfo.SchemaType?.Name != type)
            throw new NotSupportedException("Unsupported or unvalidated Multisound element.");
        foreach (XmlAttribute attribute in element.Attributes)
        {
            if (attribute.NamespaceURI == "http://www.w3.org/2000/xmlns/") continue;
            if (attribute.NamespaceURI.Length == 0 && attribute.LocalName == "TypeId"
                && uint.TryParse(attribute.Value, NumberStyles.None, CultureInfo.InvariantCulture, out uint injected)
                && injected == FastHash.GetHashCode(type)) continue;
            if (attribute.NamespaceURI.Length != 0 || Array.IndexOf(allowed, attribute.LocalName) < 0
                || attribute.Value.TrimStart().StartsWith("=", StringComparison.Ordinal))
                throw new NotSupportedException("Unsupported Multisound attribute or unresolved formula.");
        }
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: reject processing instructions and non-whitespace content outside admitted sound children. */
    //-------------------------------------------------------------------------------------------------
    private static void CheckWhitespace(XmlNode node)
    {
        if (node is XmlComment || ((node is XmlText || node is XmlWhitespace || node is XmlSignificantWhitespace)
            && string.IsNullOrWhiteSpace(node.InnerText))) return;
        throw new NotSupportedException("Multisound allows only comments/whitespace outside children.");
    }
}
