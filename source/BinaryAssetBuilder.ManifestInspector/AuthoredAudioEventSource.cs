using System.Text;
using System.Xml;
// Reborn: scalar admission uses invariant literal parsing, never formulas or current user locale.
using System.Globalization;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: admit one literal caller event against the frozen ordered audio pair, not a general source graph.
internal static class AuthoredAudioEventSource
{
    // Reborn: immutable scalar evidence accompanies the frozen source and checked native event.
    internal sealed record Settings(float Volume,uint FirstWeight,uint SecondWeight,int Count = 2,int FirstSlot = 0,int SecondSlot = 1)
    {
        internal static readonly Settings Default = new(60,1000,800);
        internal uint VolumeBits => BitConverter.SingleToUInt32Bits(Volume*0.01f);
        // Reborn: layout and detached source-slot selection derive from admitted cardinality/order, not worker metadata.
        internal int NativeLength => 152+12*Count;
        internal int ImportsLength => 4*(Count+1);
        //-------------------------------------------------------------------------------------------------
        /** Reborn: return only a detached selected-slot array so callers cannot change frozen reference order. */
        //-------------------------------------------------------------------------------------------------
        internal int[] Slots() => Count == 1 ? new[] { FirstSlot } : new[] { FirstSlot,SecondSlot };
        //-------------------------------------------------------------------------------------------------
        /** Reborn: read the admitted ordinal weight, independently of the underlying RAM/streamed source slot. */
        //-------------------------------------------------------------------------------------------------
        internal uint WeightAt(int ordinal) => ordinal == 0 ? FirstWeight : SecondWeight;

        //-------------------------------------------------------------------------------------------------
        /** Reborn: internal reconstructed profiles must obey the same bounds as authored literal admission. */
        //-------------------------------------------------------------------------------------------------
        internal void Validate()
        {
            if (!float.IsFinite(Volume) || Volume is < 0 or > 100 || FirstWeight > 1000000 || SecondWeight > 1000000 || (FirstWeight == 0 && SecondWeight == 0)
                || Count is < 1 or > 2 || FirstSlot is < 0 or > 1 || (Count == 1 ? SecondSlot != -1 || SecondWeight != 0 : SecondSlot is < 0 or > 1 || FirstSlot == SecondSlot))
                throw new InvalidDataException("Audio event scalar evidence is outside the admitted profile.");
        }
    }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: validate raw authored structure before schema defaults or core reference normalization can hide unsupported input. */
    //-------------------------------------------------------------------------------------------------
    internal static string Validate(byte[] bytes,string ram,string streamed) => Read(bytes,ram,streamed).Name;

    //-------------------------------------------------------------------------------------------------
    /** Reborn: retain validated caller scalar values alongside the event name before native work. */
    //-------------------------------------------------------------------------------------------------
    internal static (string Name,Settings Settings) Read(byte[] bytes,string ram,string streamed)
    {
        if (bytes.Length > 8192 || bytes.AsSpan().StartsWith(new byte[] { 0xEF,0xBB,0xBF })) throw new InvalidDataException("Authored event exceeds UTF-8 source bounds.");
        XmlDocument document = new() { XmlResolver = null };
        using (XmlReader reader = XmlReader.Create(new StringReader(new UTF8Encoding(false,true).GetString(bytes)),new XmlReaderSettings { DtdProcessing = DtdProcessing.Prohibit,XmlResolver = null })) document.Load(reader);
        if (document.FirstChild is XmlDeclaration declaration && declaration.Encoding.Length > 0 && !declaration.Encoding.Equals("utf-8",StringComparison.OrdinalIgnoreCase)) throw new InvalidDataException("Authored event must declare UTF-8.");
        XmlElement? wrapper = document.DocumentElement;
        if (wrapper == null || wrapper.Name != "AssetDeclaration" || wrapper.NamespaceURI != "uri:ea.com:eala:asset"
            || wrapper.Attributes.Count != 1 || wrapper.GetAttribute("xmlns") != wrapper.NamespaceURI
            || wrapper.ChildNodes.OfType<XmlElement>().Count() != 1 || wrapper.ChildNodes.OfType<XmlElement>().Single() is not XmlElement root
            || root.Name != "AudioEvent" || root.Attributes.Count != 3 || !root.HasAttribute("id")
            || !root.HasAttribute("Volume") || root.GetAttribute("Control") != "INTERRUPT")
            throw new InvalidDataException("Authored event requires one literal id/Volume/Control=INTERRUPT AudioEvent.");
        AudioFileDiagnosticIdentity.Validate(root.GetAttribute("id"));
        XmlElement[] sounds = root.ChildNodes.OfType<XmlElement>().ToArray();
        if (sounds.Length is < 1 or > 2 || sounds.Any(sound => sound.Name != "Sound" || sound.NamespaceURI != wrapper.NamespaceURI || sound.ChildNodes.OfType<XmlElement>().Any())
            || sounds.Any(sound => sound.Attributes.Count > 1 || (sound.Attributes.Count == 1 && !sound.HasAttribute("Weight"))))
            throw new InvalidDataException("Authored event requires one or two bounded literal Sound references.");
        // Reborn: exact names map to the frozen source slots; aliases, unknown targets and repeated concrete leaves reject.
        int[] slots = sounds.Select(sound => sound.InnerText == "AudioFile:"+ram ? 0 : sound.InnerText == "AudioFile:"+streamed ? 1 : -1).ToArray();
        if (slots.Contains(-1) || slots.Distinct().Count() != slots.Length) throw new InvalidDataException("Authored event Sound targets must be unique exact local names.");
        Settings settings = ReadSettings(root,slots);
        // Reborn: schema validation rejects extra text/structure; only trusted checked-in schema includes can resolve.
        document.Schemas.XmlResolver = new XmlUrlResolver();
        document.Schemas.Add(wrapper.NamespaceURI,Path.Combine(Path.GetDirectoryName(ReferencePipelineSmokeTest.FindFixture())!,"AudioEventPipeline.xsd"));
        document.Validate((_,args) => throw new InvalidDataException(args.Message));
        return (root.GetAttribute("id"),settings);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: parse only finite bounded decimal volume and unsigned weights, including the official absent-weight default. */
    //-------------------------------------------------------------------------------------------------
    internal static Settings ReadSettings(XmlElement root,int[] slots)
    {
        string text = root.GetAttribute("Volume");
        if (text.Length is < 1 or > 16 || text.Any(c => !(c is >= '0' and <= '9' or '.'))
            || !decimal.TryParse(text,NumberStyles.AllowDecimalPoint,CultureInfo.InvariantCulture,out decimal percent) || percent is < 0 or > 100)
            throw new InvalidDataException("Authored event Volume requires a decimal literal in 0..100.");
        XmlElement[] sounds = root.ChildNodes.OfType<XmlElement>().Where(element => element.LocalName == "Sound").ToArray();
        if (sounds.Length is < 1 or > 2 || slots.Length != sounds.Length) throw new InvalidDataException("Authored event scalar/reference cardinality differs.");
        Settings settings = new(float.Parse(text,NumberStyles.AllowDecimalPoint,CultureInfo.InvariantCulture),Weight(sounds[0]),sounds.Length == 2 ? Weight(sounds[1]) : 0,
            sounds.Length,slots[0],sounds.Length == 2 ? slots[1] : -1);
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
}
