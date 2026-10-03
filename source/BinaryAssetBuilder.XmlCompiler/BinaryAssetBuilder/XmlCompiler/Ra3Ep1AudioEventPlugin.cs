using System;
using System.Collections.Generic;
using System.Globalization;
using System.Xml;
using System.Xml.Schema;
using BinaryAssetBuilder.Core;
using BinaryAssetBuilder.Core.Hashing;
using Relo;

namespace BinaryAssetBuilder.XmlCompiler;

// Reborn: narrow stock-proven AudioEvent entry, isolated from audio codecs, production output and public diagnostic admission.
public sealed class Ra3Ep1AudioEventPlugin : IAssetBuilderPlugin, IAssetBuilderOutputPolicy
{
    private bool _initialized;
    public string ProfileName => "RA3EP1-AudioEvent-Experimental-v1";
    public bool CanWriteProductionOutput => false;
    public bool CanUseBuildCache => false;
    public bool CanReuseCompiledDocuments => false;
    public uint AllTypesHash => 0x5454A8E9u;
    public uint VersionNumber => 1;

    //-------------------------------------------------------------------------------------------------
    /** Reborn: revoke readiness before checking the only recovered native platform. */
    //-------------------------------------------------------------------------------------------------
    public void Initialize(TargetPlatform platform)
    {
        _initialized = false;
        if (platform != TargetPlatform.Win32) throw new NotSupportedException("Experimental AudioEvent supports only Win32.");
        _initialized = true;
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: reinitialization must enforce the same readiness and platform boundary. */
    //-------------------------------------------------------------------------------------------------
    public void ReInitialize(TargetPlatform platform) => Initialize(platform);

    //-------------------------------------------------------------------------------------------------
    /** Reborn: return fresh stock metadata in an isolated processing domain without enabling AudioFile or overridable types. */
    //-------------------------------------------------------------------------------------------------
    public ExtendedTypeInformation GetExtendedTypeInformation(uint typeId)
    {
        if (!_initialized) throw new InvalidOperationException("Experimental AudioEvent profile is not initialized.");
        if (typeId != 0x844D7B9Fu) throw new NotSupportedException("Unsupported experimental AudioEvent type.");
        return new ExtendedTypeInformation { TypeId = typeId, TypeName = "AudioEvent", Type = typeof(Marshaler.Ep1AudioEvent),
            TypeHash = 0x560C2E45u, ProcessingHash = 0x560C2E45u ^ 0x45503141u,
            Tokenized = false, HasCustomData = false, UseBuildCache = false };
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: require current identity, bounded stock-proven shapes and complete ordered AudioFile preparation before native marshalling. */
    //-------------------------------------------------------------------------------------------------
    public unsafe AssetBuffer ProcessInstance(InstanceDeclaration instance)
    {
        if (instance?.Node is not XmlElement root) throw new NotSupportedException("A validated AudioEvent root is required.");
        GetExtendedTypeInformation(instance.Handle.TypeId);
        CheckElement(root, "AudioEvent", "AudioEvent", new[] { "id", "Volume", "VolumeShift", "PerFileVolumeShift", "MinVolume",
            "ShrunkenPitchModifier", "ShrunkenVolumeModifier", "PlayPercent", "Limit", "Priority", "Type", "Control",
            "MinRange", "MaxRange", "LowPassCutoff", "ZoomedInOffscreenVolumePercent", "ZoomedInOffscreenMinVolumePercent",
            "ZoomedInOffscreenOcclusionPercent", "ReverbEffectLevel", "DryLevel", "SubmixSlider" });
        if (root.OwnerDocument.Schemas.Count == 0 || instance.Handle.TypeHash != 0x560C2E45u
            || InstanceHandle.GetInstanceId(root.GetAttribute("id")) != instance.Handle.InstanceId
            || instance.InheritFromHandle != null || instance.HasCustomData || instance.WeakReferencedInstances.Count != 0
            || instance.ReferencedFiles.Count != 0 || instance.ValidatedReferencedInstances == null
            || instance.ValidatedReferencedInstances.Count != instance.ReferencedInstances.Count)
            throw new NotSupportedException("AudioEvent requires current schema-bound identity and complete strong dependencies without inheritance/custom/weak/file metadata.");
        string id = root.GetAttribute("id");
        if (id.Length == 0 || id.Length > 128 || id != id.Trim()) throw new NotSupportedException("AudioEvent ID is not bounded.");
        // Reborn: bound detached-copy work even when comments or whitespace accompany otherwise eligible children.
        string currentXml = root.OuterXml;
        if (currentXml.Length > 65536 || root.ChildNodes.Count > 128) throw new NotSupportedException("AudioEvent XML is not bounded.");
        foreach (XmlAttribute attr in root.Attributes)
            if (attr.NamespaceURI.Length == 0 && Array.IndexOf(new[] { "id", "TypeId", "Priority", "Type", "Control", "SubmixSlider" }, attr.Name) < 0)
                CheckReal(attr.Value);
        foreach (string flag in root.GetAttribute("Control").Split((char[]?)null, StringSplitOptions.RemoveEmptyEntries))
            if (flag != "LOOP" && flag != "INTERRUPT" && flag != "FADE_ON_KILL" && flag != "IMMEDIATE_DECAY_ON_KILL")
                throw new NotSupportedException("AudioEvent control flag is not stock-proven in this profile.");
        if (root.HasAttribute("SubmixSlider") && root.GetAttribute("SubmixSlider") != "SOUNDFX")
            throw new NotSupportedException("Only explicit stock-proven SOUNDFX slider is admitted.");
        int count = 0, extraBytes = root.HasAttribute("SubmixSlider") ? 4 : 0, relocations = root.HasAttribute("SubmixSlider") ? 1 : 0;
        HashSet<string> ranges = new(StringComparer.Ordinal), lists = new(StringComparer.Ordinal);
        HashSet<ulong> identities = new();
        foreach (XmlNode node in root.ChildNodes)
        {
            if (node is not XmlElement child) { CheckWhitespace(node); continue; }
            string name = child.LocalName;
            if (name == "Attack" || name == "Sound" || name == "Decay")
            {
                if (count >= 32) throw new NotSupportedException("AudioEvent admits at most 32 AudioFile references.");
                CheckElement(child, name, "AudioFileRefWithWeight", new[] { "Weight", "Volume" });
                if (child.HasAttribute("Volume") && child.GetAttribute("Volume") != "100")
                    throw new NotSupportedException("Nondefault child Volume is not stock-proven.");
                foreach (XmlNode value in child.ChildNodes)
                    if (value is not XmlText && value is not XmlWhitespace && value is not XmlSignificantWhitespace && value is not XmlComment)
                        throw new NotSupportedException("AudioFile child requires plain reference text.");
                CheckImport(child.InnerText, instance, count);
                InstanceHandle target = instance.ValidatedReferencedInstances[count++];
                if (!identities.Add(((ulong)target.TypeId << 32) | target.InstanceId))
                    throw new NotSupportedException("Repeated concrete AudioFile aliases are not admitted.");
                if (lists.Add(name)) relocations++;
            }
            else
            {
                string type = name == "PitchShift" || name == "PerFilePitchShift" ? "RealRange"
                    : name == "Delay" || name == "InitialDelay" ? "IntRange" : name == "NonInterruptibleTime" ? "TimeRange" : "";
                if (type.Length == 0 || !ranges.Add(name)) throw new NotSupportedException("Unsupported or repeated AudioEvent range.");
                CheckElement(child, name, type, new[] { "Low", "High" });
                foreach (XmlNode value in child.ChildNodes) CheckWhitespace(value);
                if (type == "RealRange") { CheckReal(child.GetAttribute("Low")); CheckReal(child.GetAttribute("High")); }
                // Reborn: schema-valid time text must not overflow to a nonfinite native second value.
                if (type == "TimeRange")
                    foreach (string field in new[] { "Low", "High" })
                    {
                        string value = child.GetAttribute(field);
                        if (value.EndsWith("ms", StringComparison.Ordinal)) value = value.Substring(0, value.Length - 2);
                        else if (value.EndsWith("s", StringComparison.Ordinal)) value = value.Substring(0, value.Length - 1);
                        CheckReal(value);
                    }
                extraBytes += 8; relocations++;
            }
        }
        if (count != instance.ReferencedInstances.Count) throw new NotSupportedException("AudioEvent has unused dependency slots.");
        // Reborn: current scalar/range/sequence values are revalidated after PSVI checks; never authorize stale current values.
        XmlDocument copy = new() { XmlResolver = null }; copy.Schemas.Add(root.OwnerDocument.Schemas); copy.LoadXml(currentXml);
        foreach (XmlElement element in copy.DocumentElement.SelectNodes("descendant-or-self::*")) element.RemoveAttribute("TypeId");
        copy.Validate((_, args) => throw new XmlSchemaValidationException(args.Message));
        XmlNamespaceManager ns = new(copy.NameTable); ns.AddNamespace("ea", root.NamespaceURI);
        Marshaler.Ep1AudioEvent* native;
        using Tracker tracker = new((void**)&native, (uint)sizeof(Marshaler.Ep1AudioEvent), false);
        Marshaler.Marshal(new Node(copy.DocumentElement.CreateNavigator(), ns), native, tracker);
        Chunk chunk = new();
        if (!tracker.MakeRelocatable(chunk) || chunk.InstanceBuffer.Length != 152 + count * 12 + extraBytes
            || chunk.RelocationBuffer.Length != (relocations == 0 ? 0 : (relocations + 1) * 4)
            || chunk.ImportsBuffer.Length != (count == 0 ? 0 : (count + 1) * 4))
            throw new InvalidOperationException("AudioEvent native buffer shape differs.");
        return new AssetBuffer { InstanceData = chunk.InstanceBuffer, RelocationData = chunk.RelocationBuffer, ImportsData = chunk.ImportsBuffer };
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: accept only one core-normalized ordinal whose original and concrete AudioFile identities match current text exactly. */
    //-------------------------------------------------------------------------------------------------
    private static void CheckImport(string value, InstanceDeclaration instance, int ordinal)
    {
        int slash = value.LastIndexOf('\\');
        if (slash <= 0 || value.IndexOf('\\') != slash
            || !uint.TryParse(value.Substring(slash + 1), NumberStyles.None, CultureInfo.InvariantCulture, out uint index)
            || index != ordinal || index >= instance.ReferencedInstances.Count)
            throw new NotSupportedException("AudioFile requires its ordered core-normalized selector.");
        string name = value.Substring(0, slash); int colon = name.IndexOf(':');
        if (name != name.Trim() || name.StartsWith("=", StringComparison.Ordinal) || colon == 0
            || colon == name.Length - 1 || colon != name.LastIndexOf(':') || name.Length > 256)
            throw new NotSupportedException("AudioFile normalized name is invalid.");
        InstanceHandle expected = new(name); if (expected.TypeId == 0) expected.TypeName = "AudioFile";
        InstanceHandle original = instance.ReferencedInstances[ordinal], concrete = instance.ValidatedReferencedInstances[ordinal];
        if (expected.TypeName != "AudioFile" || original == null || concrete == null || original.TypeName != "AudioFile"
            || original.TypeId != 0x166B084Du || original.InstanceId != expected.InstanceId
            || concrete.TypeName != "AudioFile" || concrete.TypeId != 0x166B084Du || concrete.InstanceId != expected.InstanceId
            || concrete.InstanceId != InstanceHandle.GetInstanceId(concrete.InstanceName))
            throw new NotSupportedException("AudioFile identity does not match its prepared slot.");
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: require official schema types and checked compiler-injected IDs; reject unknown/namespaced/formula attributes. */
    //-------------------------------------------------------------------------------------------------
    private static void CheckElement(XmlElement element, string name, string type, string[] allowed)
    {
        if (element.LocalName != name || element.NamespaceURI != "uri:ea.com:eala:asset" || element.SchemaInfo.SchemaType?.Name != type)
            throw new NotSupportedException("Unsupported or unvalidated AudioEvent element.");
        foreach (XmlAttribute attr in element.Attributes)
        {
            if (attr.NamespaceURI == "http://www.w3.org/2000/xmlns/") continue;
            if (attr.NamespaceURI.Length == 0 && attr.Name == "TypeId" && uint.TryParse(attr.Value, NumberStyles.None,
                CultureInfo.InvariantCulture, out uint injected) && injected == FastHash.GetHashCode(type)) continue;
            if (attr.NamespaceURI.Length != 0 || Array.IndexOf(allowed, attr.Name) < 0 || attr.Value.TrimStart().StartsWith("=", StringComparison.Ordinal))
                throw new NotSupportedException("Unsupported AudioEvent attribute or unresolved formula.");
        }
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: keep scalar values finite before percentage conversion or range marshalling. */
    //-------------------------------------------------------------------------------------------------
    private static void CheckReal(string value)
    {
        if (!float.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out float number) || float.IsNaN(number) || float.IsInfinity(number))
            throw new NotSupportedException("AudioEvent scalar must be finite.");
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: reject nested content and processing instructions outside reference text. */
    //-------------------------------------------------------------------------------------------------
    private static void CheckWhitespace(XmlNode node)
    {
        if (node is XmlComment || ((node is XmlText || node is XmlWhitespace || node is XmlSignificantWhitespace) && string.IsNullOrWhiteSpace(node.InnerText))) return;
        throw new NotSupportedException("AudioEvent permits only comments/whitespace outside reference text.");
    }
}
