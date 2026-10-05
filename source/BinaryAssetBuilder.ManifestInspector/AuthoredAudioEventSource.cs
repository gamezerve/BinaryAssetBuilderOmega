using System.Text;
using System.Xml;
// Reborn: scalar admission uses invariant literal parsing, never formulas or current user locale.
using System.Globalization;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: admit one literal caller event against a frozen ordered 1..8-leaf audio pool, not a general source graph.
internal static class AuthoredAudioEventSource
{
    // Reborn: immutable scalar evidence accompanies the frozen source and checked native event.
    internal sealed class Settings : IEquatable<Settings>
    {
        // Reborn: private cloned vectors own selection order and weights; equality compares contents, not array identities.
        private readonly uint[] _weights;
        private readonly int[] _slots;
        private readonly bool _legacyValid;
        internal float Volume { get; }
        internal uint ControlBits { get; }
        internal int PoolCount { get; }
        internal int Count { get; }
        internal uint FirstWeight => _weights[0];
        internal uint SecondWeight => Count == 1 ? 0 : _weights[1];
        internal int FirstSlot => _slots[0];
        internal int SecondSlot => Count == 1 ? -1 : _slots[1];
        internal static readonly Settings Default = new(60,1000,800);
        //-------------------------------------------------------------------------------------------------
        /** Reborn: preserve the old pair constructor/defaults and reject invalid pair reconstructions exactly as before. */
        //-------------------------------------------------------------------------------------------------
        internal Settings(float Volume,uint FirstWeight,uint SecondWeight,int Count = 2,int FirstSlot = 0,int SecondSlot = 1,uint ControlBits = 8,int PoolCount = 2)
        {
            this.Volume = Volume; this.ControlBits = ControlBits; this.PoolCount = PoolCount; this.Count = Count;
            _weights = Count == 1 ? new[] { FirstWeight } : new[] { FirstWeight,SecondWeight }; _slots = Count == 1 ? new[] { FirstSlot } : new[] { FirstSlot,SecondSlot };
            _legacyValid = Count is >= 1 and <= 2 && (Count != 1 || SecondWeight == 0 && SecondSlot == -1);
        }
        //-------------------------------------------------------------------------------------------------
        /** Reborn: freeze bounded vector candidates before validation so later caller-array edits cannot change admitted evidence. */
        //-------------------------------------------------------------------------------------------------
        internal Settings(float volume,uint[] weights,int[] slots,int poolCount,uint controlBits)
        { Volume = volume; ControlBits = controlBits; PoolCount = poolCount; Count = weights.Length; _weights = (uint[])weights.Clone(); _slots = (int[])slots.Clone(); _legacyValid = true; }
        internal uint VolumeBits => BitConverter.SingleToUInt32Bits(Volume*0.01f);
        // Reborn: layout and detached source-slot selection derive from admitted cardinality/order, not worker metadata.
        internal int NativeLength => 152+12*Count;
        internal int ImportsLength => 4*(Count+1);
        //-------------------------------------------------------------------------------------------------
        /** Reborn: return only a detached selected-slot array so callers cannot change frozen reference order. */
        //-------------------------------------------------------------------------------------------------
        internal int[] Slots() => (int[])_slots.Clone();
        //-------------------------------------------------------------------------------------------------
        /** Reborn: read the admitted ordinal weight, independently of the underlying RAM/streamed source slot. */
        //-------------------------------------------------------------------------------------------------
        internal uint WeightAt(int ordinal) => _weights[ordinal];

        //-------------------------------------------------------------------------------------------------
        /** Reborn: internal reconstructed profiles must obey the same bounds as authored literal admission. */
        //-------------------------------------------------------------------------------------------------
        internal void Validate()
        {
            if (!_legacyValid || !float.IsFinite(Volume) || Volume is < 0 or > 100 || _weights.Any(weight => weight > 1000000) || !_weights.Any(weight => weight > 0)
                || PoolCount is < 1 or > 8 || Count is < 1 or > 8 || Count > PoolCount || _weights.Length != Count || _slots.Length != Count || _slots.Any(slot => slot < 0 || slot >= PoolCount) || _slots.Distinct().Count() != Count
                || (ControlBits & ~0x129u) != 0)
                throw new InvalidDataException("Audio event scalar evidence is outside the admitted profile.");
        }
        //-------------------------------------------------------------------------------------------------
        /** Reborn: independently parsed/core-derived vectors must compare by exact scalar and ordered vector content. */
        //-------------------------------------------------------------------------------------------------
        public bool Equals(Settings? other) => other is not null && Volume.Equals(other.Volume) && ControlBits == other.ControlBits && PoolCount == other.PoolCount && Count == other.Count && _legacyValid == other._legacyValid && _weights.SequenceEqual(other._weights) && _slots.SequenceEqual(other._slots);
        //-------------------------------------------------------------------------------------------------
        /** Reborn: retain value semantics for existing object comparisons. */
        //-------------------------------------------------------------------------------------------------
        public override bool Equals(object? other) => other is Settings settings && Equals(settings);
        //-------------------------------------------------------------------------------------------------
        /** Reborn: hash immutable scalar fields; equality additionally checks every ordered vector element. */
        //-------------------------------------------------------------------------------------------------
        public override int GetHashCode() => HashCode.Combine(Volume,ControlBits,PoolCount,Count,_legacyValid);
        //-------------------------------------------------------------------------------------------------
        /** Reborn: existing snapshot/source equality retains ordered value semantics after vectorization. */
        //-------------------------------------------------------------------------------------------------
        public static bool operator ==(Settings? left,Settings? right) => ReferenceEquals(left,right) || left?.Equals(right) == true;
        //-------------------------------------------------------------------------------------------------
        /** Reborn: inequality is the exact inverse of immutable vector equality. */
        //-------------------------------------------------------------------------------------------------
        public static bool operator !=(Settings? left,Settings? right) => !(left == right);
    }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: validate raw authored structure before schema defaults or core reference normalization can hide unsupported input. */
    //-------------------------------------------------------------------------------------------------
    internal static string Validate(byte[] bytes,string ram,string streamed) => Read(bytes,ram,streamed).Name;

    //-------------------------------------------------------------------------------------------------
    /** Reborn: retain validated caller scalar values alongside the event name before native work. */
    //-------------------------------------------------------------------------------------------------
    internal static (string Name,Settings Settings) Read(byte[] bytes,string ram,string streamed) => Read(bytes,new[] { ram,streamed });

    //-------------------------------------------------------------------------------------------------
    /** Reborn: resolve 1..8 unique exact selected targets within an explicit pool without widening other event fields. */
    //-------------------------------------------------------------------------------------------------
    internal static (string Name,Settings Settings) Read(byte[] bytes,string[] names)
    {
        if (names.Length is < 1 or > 8 || names.Select(BinaryAssetBuilder.Core.InstanceHandle.GetInstanceId).Distinct().Count() != names.Length) throw new InvalidDataException("Event pool requires unique bounded SAGE identities.");
        foreach (string name in names) AudioFileDiagnosticIdentity.Validate(name);
        if (bytes.Length > 8192 || bytes.AsSpan().StartsWith(new byte[] { 0xEF,0xBB,0xBF })) throw new InvalidDataException("Authored event exceeds UTF-8 source bounds.");
        XmlDocument document = new() { XmlResolver = null };
        using (XmlReader reader = XmlReader.Create(new StringReader(new UTF8Encoding(false,true).GetString(bytes)),new XmlReaderSettings { DtdProcessing = DtdProcessing.Prohibit,XmlResolver = null })) document.Load(reader);
        if (document.FirstChild is XmlDeclaration declaration && declaration.Encoding.Length > 0 && !declaration.Encoding.Equals("utf-8",StringComparison.OrdinalIgnoreCase)) throw new InvalidDataException("Authored event must declare UTF-8.");
        XmlElement? wrapper = document.DocumentElement;
        if (wrapper == null || wrapper.Name != "AssetDeclaration" || wrapper.NamespaceURI != "uri:ea.com:eala:asset"
            || wrapper.Attributes.Count != 1 || wrapper.GetAttribute("xmlns") != wrapper.NamespaceURI
            || wrapper.ChildNodes.OfType<XmlElement>().Count() != 1 || wrapper.ChildNodes.OfType<XmlElement>().Single() is not XmlElement root
            || root.Name != "AudioEvent" || root.Attributes.Count != 3 || !root.HasAttribute("id")
            || !root.HasAttribute("Volume") || !root.HasAttribute("Control"))
            throw new InvalidDataException("Authored event requires one literal id/Volume/Control AudioEvent.");
        AudioFileDiagnosticIdentity.Validate(root.GetAttribute("id"));
        XmlElement[] sounds = root.ChildNodes.OfType<XmlElement>().ToArray();
        if (sounds.Length < 1 || sounds.Length > names.Length || sounds.Any(sound => sound.Name != "Sound" || sound.NamespaceURI != wrapper.NamespaceURI || sound.ChildNodes.OfType<XmlElement>().Any())
            || sounds.Any(sound => sound.Attributes.Count > 1 || (sound.Attributes.Count == 1 && !sound.HasAttribute("Weight"))))
            throw new InvalidDataException("Authored event requires bounded unique local Sound references.");
        // Reborn: exact names map to the frozen source slots; aliases, unknown targets and repeated concrete leaves reject.
        int[] slots = sounds.Select(sound => Array.FindIndex(names,name => sound.InnerText == "AudioFile:"+name)).ToArray();
        if (slots.Contains(-1) || slots.Distinct().Count() != slots.Length) throw new InvalidDataException("Authored event Sound targets must be unique exact local names.");
        Settings settings = ReadSettings(root,slots,names.Length);
        // Reborn: schema validation rejects extra text/structure; only trusted checked-in schema includes can resolve.
        document.Schemas.XmlResolver = new XmlUrlResolver();
        document.Schemas.Add(wrapper.NamespaceURI,Path.Combine(Path.GetDirectoryName(ReferencePipelineSmokeTest.FindFixture())!,"AudioEventPipeline.xsd"));
        document.Validate((_,args) => throw new InvalidDataException(args.Message));
        return (root.GetAttribute("id"),settings);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: parse only finite bounded decimal volume and unsigned weights, including the official absent-weight default. */
    //-------------------------------------------------------------------------------------------------
    internal static Settings ReadSettings(XmlElement root,int[] slots,int poolCount = 2)
    {
        string text = root.GetAttribute("Volume");
        if (text.Length is < 1 or > 16 || text.Any(c => !(c is >= '0' and <= '9' or '.'))
            || !decimal.TryParse(text,NumberStyles.AllowDecimalPoint,CultureInfo.InvariantCulture,out decimal percent) || percent is < 0 or > 100)
            throw new InvalidDataException("Authored event Volume requires a decimal literal in 0..100.");
        XmlElement[] sounds = root.ChildNodes.OfType<XmlElement>().Where(element => element.LocalName == "Sound").ToArray();
        if (sounds.Length < 1 || sounds.Length > poolCount || slots.Length != sounds.Length) throw new InvalidDataException("Authored event scalar/reference cardinality differs.");
        Settings settings = new(float.Parse(text,NumberStyles.AllowDecimalPoint,CultureInfo.InvariantCulture),sounds.Select(Weight).ToArray(),slots,poolCount,ReadControls(root.GetAttribute("Control")));
        settings.Validate(); return settings;
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: cap each weight independently without changing the official uint representation or default value. */
    //-------------------------------------------------------------------------------------------------
    private static uint Weight(XmlElement sound)
    {
        if (!sound.HasAttribute("Weight")) return 1000;
        string text = sound.GetAttribute("Weight");
        if (text.Length is < 1 or > 7 || !uint.TryParse(text,NumberStyles.None,CultureInfo.InvariantCulture,out uint value) || value > 1000000)
            throw new InvalidDataException("Authored event Weight requires an unsigned literal in 0..1000000.");
        return value;
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: map only plugin-admitted EP1 flags to audited bits; reject numeric enums, duplicate tokens and unproven controls. */
    //-------------------------------------------------------------------------------------------------
    internal static uint ReadControls(string text)
    {
        if (text.Length > 128 || text.Any(c => !(c is >= 'A' and <= 'Z' or '_' or ' ' or '\t' or '\r' or '\n')))
            throw new InvalidDataException("Authored event Control requires bounded literal EP1 flag tokens.");
        uint bits = 0;
        foreach (string token in text.Split(new[] { ' ','\t','\r','\n' },StringSplitOptions.RemoveEmptyEntries))
        {
            uint flag = token switch { "LOOP" => 0x1u,"INTERRUPT" => 0x8u,"FADE_ON_KILL" => 0x20u,"IMMEDIATE_DECAY_ON_KILL" => 0x100u,
                _ => throw new InvalidDataException("Authored event Control is outside the proven plugin profile.") };
            if ((bits & flag) != 0) throw new InvalidDataException("Authored event Control has duplicate tokens.");
            bits |= flag;
        }
        return bits;
    }
}
