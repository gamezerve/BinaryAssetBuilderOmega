using System;
using System.Buffers.Binary;
using System.Globalization;
using System.Xml;
using System.Xml.Schema;
using BinaryAssetBuilder.Core;
using BinaryAssetBuilder.Core.Hashing;

namespace BinaryAssetBuilder.XmlCompiler;

// Reborn: freeze the narrow authored AudioFile/WAV boundary without native DLL loading, file resolution or plugin registration.
public static class Ra3Ep1AudioFileInputProfile
{
    // Reborn: candidate duration bounds are diagnostic-only; existing production/core callers retain the canonical profile.
    public const int MinimumCandidateSamples = 12000;
    public const int MaximumCandidateSamples = 96000;
    public const int MaximumCandidateWaveBytes = 44+2*MaximumCandidateSamples;
    // Reborn: expose immutable preparation only; do not return mutable source buffers or live identity handles.
    public sealed class PreparedInput
    {
        private readonly string _xml;
        private readonly byte[] _wave;
        private readonly uint _instanceHash;
        // Reborn: keep the admission mode with the snapshot so revalidation cannot silently switch profiles.
        private readonly bool _durationCandidate;
        public string InstanceName { get; }
        public string FileName { get; }
        public string Subtitle { get; }
        public bool Streamed { get; }
        public int Samples { get; }
        public int Rate => 48000;
        public byte Channels => 1;
        public int Codec => 29;
        public int OutputContainer => 39;

        //-------------------------------------------------------------------------------------------------
        /** Reborn: own exact current XML, identity hash and PCM bytes rather than retaining caller aliases. */
        //-------------------------------------------------------------------------------------------------
        internal PreparedInput(string xml,ReadOnlySpan<byte> wave,InstanceHandle identity,string file,string subtitle,bool streamed,int samples,bool durationCandidate)
        {
            _xml = xml; _wave = wave.ToArray(); _instanceHash = identity.InstanceHash;
            InstanceName = identity.InstanceName; FileName = file; Subtitle = subtitle; Streamed = streamed;
            // Reborn: samples come from the checked RIFF/data lengths, never caller-supplied runtime metadata.
            Samples = samples; _durationCandidate = durationCandidate;
        }

        //-------------------------------------------------------------------------------------------------
        /** Reborn: provide a detached encoder input snapshot; mutations cannot alter this preparation. */
        //-------------------------------------------------------------------------------------------------
        public byte[] CopyWave() => (byte[])_wave.Clone();

        //-------------------------------------------------------------------------------------------------
        /** Reborn: reject changed XML/identity/PCM before a caller begins native work or writes output. */
        //-------------------------------------------------------------------------------------------------
        public void VerifyCurrent(XmlElement root,InstanceHandle identity,TargetPlatform platform,ReadOnlySpan<byte> wave)
        {
            PreparedInput current = PrepareChecked(root,identity,platform,wave,_durationCandidate);
            if (_xml != current._xml) throw new NotSupportedException("AudioFile XML preparation is stale; prepare current XML again.");
            if (InstanceName != current.InstanceName || _instanceHash != current._instanceHash) throw new NotSupportedException("AudioFile identity preparation is stale.");
            if (!wave.SequenceEqual(_wave)) throw new NotSupportedException("AudioFile WAV preparation is stale.");
        }

        //-------------------------------------------------------------------------------------------------
        /** Reborn: recheck the snapshot then produce runtime bytes; custom framing/publication remains a separate gate. */
        //-------------------------------------------------------------------------------------------------
        public AssetBuffer SerializeCurrent(XmlElement root,InstanceHandle identity,TargetPlatform platform,ReadOnlySpan<byte> wave,ReadOnlySpan<byte> header)
        {
            VerifyCurrent(root,identity,platform,wave);
            if (header.Length != (Streamed ? 8 : 0) || (Streamed && header[0] != 4))
                throw new NotSupportedException("Prepared XAS play location requires its exact generated header shape/tag.");
            return Ra3Ep1AudioFileRuntimeSerializer.Serialize(platform,Subtitle,Samples,Rate,Channels,header);
        }
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: require current official-schema AudioFile, explicit mono 48kHz XAS settings and canonical 250ms PCM16 WAV before native work. */
    //-------------------------------------------------------------------------------------------------
    public static PreparedInput Prepare(XmlElement root,InstanceHandle identity,TargetPlatform platform,ReadOnlySpan<byte> wave)
        => PrepareChecked(root,identity,platform,wave,false);

    //-------------------------------------------------------------------------------------------------
    /** Reborn: prepare a bounded 250ms-to-2s PCM duration candidate without opening codec, worker or package admission. */
    //-------------------------------------------------------------------------------------------------
    public static PreparedInput PrepareDurationCandidate(XmlElement root,InstanceHandle identity,TargetPlatform platform,ReadOnlySpan<byte> wave)
        => PrepareChecked(root,identity,platform,wave,true);

    //-------------------------------------------------------------------------------------------------
    /** Reborn: share official XML/identity gates while keeping canonical and experimental duration admission explicit. */
    //-------------------------------------------------------------------------------------------------
    private static PreparedInput PrepareChecked(XmlElement root,InstanceHandle identity,TargetPlatform platform,ReadOnlySpan<byte> wave,bool durationCandidate)
    {
        if (platform != TargetPlatform.Win32) throw new NotSupportedException("AudioFile input profile supports only Win32.");
        if (root == null || root.LocalName != "AudioFile" || root.NamespaceURI != "uri:ea.com:eala:asset"
            || root.SchemaInfo.SchemaType?.Name != "AudioFile" || root.OwnerDocument.Schemas.Count == 0)
            throw new NotSupportedException("Schema-bound authored AudioFile required.");
        string xml = root.OuterXml;
        if (xml.Length > 8192 || root.ChildNodes.Count > 16) throw new NotSupportedException("AudioFile XML exceeds the input profile bound.");
        string id = root.GetAttribute("id");
        if (id.Length is < 1 or > 128 || identity == null || identity.TypeName != "AudioFile" || identity.TypeId != 0x166B084Du
            || identity.TypeHash != 0x53C81E47u || identity.InstanceName != id || identity.InstanceId != InstanceHandle.GetInstanceId(id))
            throw new NotSupportedException("Current EP1 AudioFile identity/fingerprint required.");
        foreach (XmlAttribute attr in root.Attributes)
        {
            if (attr.NamespaceURI == "http://www.w3.org/2000/xmlns/") continue;
            if (attr.NamespaceURI.Length == 0 && attr.Name == "TypeId" && uint.TryParse(attr.Value,NumberStyles.None,CultureInfo.InvariantCulture,out uint injected)
                && injected == FastHash.GetHashCode("AudioFile")) continue;
            // Reborn: official validation inserts unused platform quality defaults; authored cross-platform settings remain closed.
            if (attr.NamespaceURI.Length == 0 && !attr.Specified && (attr.Name == "XenonQuality" || attr.Name == "PS3Quality") && attr.Value == "75") continue;
            if (attr.NamespaceURI.Length != 0 || Array.IndexOf(new[] { "id","File","PCSampleRate","PCCompression","PCQuality","IsStreamedOnPC","SubtitleStringName" },attr.Name) < 0
                || attr.Value.TrimStart().StartsWith("=",StringComparison.Ordinal)) throw new NotSupportedException("Unsupported authored AudioFile option/formula.");
        }
        foreach (XmlNode child in root.ChildNodes)
            if (child is not XmlComment && !((child is XmlText || child is XmlWhitespace || child is XmlSignificantWhitespace) && string.IsNullOrWhiteSpace(child.InnerText)))
                throw new NotSupportedException("AudioFile children are not admitted.");
        string file = root.GetAttribute("File");
        if (file.Length is < 5 or > 128 || !file.EndsWith(".wav",StringComparison.Ordinal) || file.Contains("..",StringComparison.Ordinal))
            throw new NotSupportedException("Only a bounded authored WAV leaf name is admitted; no filesystem resolution is performed.");
        foreach (char value in file)
            if (!(value is >= 'a' and <= 'z' or >= 'A' and <= 'Z' or >= '0' and <= '9' or '_' or '-' or '.'))
                throw new NotSupportedException("Paths, URIs and selectors are outside the authored WAV leaf profile.");
        string play = root.GetAttribute("IsStreamedOnPC");
        if (root.GetAttribute("PCSampleRate") != "48000" || root.GetAttribute("PCCompression") != "XAS" || root.GetAttribute("PCQuality") != "75"
            || play is not ("true" or "True" or "false" or "False" or "0" or "1")) throw new NotSupportedException("Explicit proven rate/compression/play location required; quality changes and resampling are closed.");
        // Reborn: stale PSVI cannot authorize current values; validate a detached copy without compiler-injected TypeId.
        XmlDocument copy = new() { XmlResolver = null }; copy.Schemas.Add(root.OwnerDocument.Schemas); copy.LoadXml(xml);
        copy.DocumentElement.RemoveAttribute("TypeId"); copy.Validate((_, args) => throw new XmlSchemaValidationException(args.Message));
        int samples = CheckWave(wave,durationCandidate);
        string subtitle = root.HasAttribute("SubtitleStringName") ? root.GetAttribute("SubtitleStringName") : "DIALOGEVENT:"+file.Substring(0,file.Length-4)+"SubTitle";
        // Reborn: reuse the wire serializer's proven subtitle bounds without executing an encoder or publishing files.
        Ra3Ep1AudioFileRuntimeSerializer.Serialize(platform,subtitle,samples,48000,1,ReadOnlySpan<byte>.Empty);
        return new PreparedInput(xml,wave,identity,file,subtitle,play is "true" or "True" or "1",samples,durationCandidate);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: reject truncated/chunked/compressed/resampled PCM; candidate durations must have exact bounded and aligned RIFF/data lengths. */
    //-------------------------------------------------------------------------------------------------
    private static int CheckWave(ReadOnlySpan<byte> wave,bool durationCandidate)
    {
        // Reborn: length checks precede every slice and limit allocation elsewhere to an explicit candidate maximum.
        if (wave.Length < 24044 || wave.Length > (durationCandidate ? MaximumCandidateWaveBytes : 24044) || (wave.Length-44)%2 != 0
            || !wave.Slice(0,4).SequenceEqual(new byte[] { 82,73,70,70 })
            || BinaryPrimitives.ReadUInt32LittleEndian(wave.Slice(4,4)) != (uint)(wave.Length-8)
            || !wave.Slice(8,8).SequenceEqual(new byte[] { 87,65,86,69,102,109,116,32 })
            || BinaryPrimitives.ReadUInt32LittleEndian(wave.Slice(16,4)) != 16
            || BinaryPrimitives.ReadUInt16LittleEndian(wave.Slice(20,2)) != 1 || BinaryPrimitives.ReadUInt16LittleEndian(wave.Slice(22,2)) != 1
            || BinaryPrimitives.ReadUInt32LittleEndian(wave.Slice(24,4)) != 48000 || BinaryPrimitives.ReadUInt32LittleEndian(wave.Slice(28,4)) != 96000
            || BinaryPrimitives.ReadUInt16LittleEndian(wave.Slice(32,2)) != 2 || BinaryPrimitives.ReadUInt16LittleEndian(wave.Slice(34,2)) != 16
            || !wave.Slice(36,4).SequenceEqual(new byte[] { 100,97,116,97 }) || BinaryPrimitives.ReadUInt32LittleEndian(wave.Slice(40,4)) != (uint)(wave.Length-44))
            throw new NotSupportedException(durationCandidate ? "Duration candidates require exact canonical PCM16 mono 48kHz WAV with 12000..96000 samples." : "Only canonical PCM16 mono 48kHz / 12000-sample WAV input is proven.");
        return (wave.Length-44)/2;
    }
}
