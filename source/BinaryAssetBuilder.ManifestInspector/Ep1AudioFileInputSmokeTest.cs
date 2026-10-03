using System.Xml;
using System.Xml.Schema;
using BinaryAssetBuilder.Core;
using BinaryAssetBuilder.Core.Hashing;
using BinaryAssetBuilder.XmlCompiler;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: verify authored audio preparation without native codec calls, file resolution or production admission.
internal static class Ep1AudioFileInputSmokeTest
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: check official defaults, fixed PCM shape, fingerprint/snapshot gates, play location, rejection and recovery. */
    //-------------------------------------------------------------------------------------------------
    internal static void Run()
    {
        using MemoryStream source = new(); AudioEncoderPoc.WriteWave(source); byte[] wave = source.ToArray();
        byte[] streamHeader = Convert.FromHexString("0400BB8040002EE0");
        foreach (bool streamed in new[] { false,true })
        {
            XmlElement root = AudioEncoderPoc.CreateDefinition(streamed); InstanceHandle identity = AudioEncoderPoc.Identity(root);
            var prepared = Ra3Ep1AudioFileInputProfile.Prepare(root,identity,TargetPlatform.Win32,wave);
            byte[] header = streamed ? streamHeader : Array.Empty<byte>();
            Require(prepared.FileName == "input.wav" && prepared.InstanceName == "RebornAudioInput" && prepared.Streamed == streamed
                && prepared.Codec == 29 && prepared.OutputContainer == 39 && prepared.Rate == 48000 && prepared.Samples == 12000 && prepared.Channels == 1,"Prepared fields differ.");
            AssetBuffer baseline = prepared.SerializeCurrent(root,identity,TargetPlatform.Win32,wave,header);
            Require(baseline.InstanceData.Length == (streamed ? 88 : 80) && baseline.RelocationData.Length == (streamed ? 12 : 8) && baseline.ImportsData.Length == 0,"Prepared runtime size differs.");
            AudioFileRuntimeProbe.Parse(baseline.InstanceData,baseline.InstanceData.Length);
            byte[] detached = prepared.CopyWave(); detached[44] ^= 1; Require(prepared.CopyWave().SequenceEqual(wave),"Prepared WAV alias escaped.");
            byte[] changed = (byte[])wave.Clone(); changed[44] ^= 1;
            Reject(() => prepared.VerifyCurrent(root,identity,TargetPlatform.Win32,changed));
            var changedInput = Ra3Ep1AudioFileInputProfile.Prepare(root,identity,TargetPlatform.Win32,changed);
            changed[45] ^= 1; Reject(() => changedInput.VerifyCurrent(root,identity,TargetPlatform.Win32,changed));
            Require(changedInput.CopyWave()[45] == wave[45],"Preparation retained the input alias.");
            foreach (string field in new[] { "File","SubtitleStringName","id" })
            {
                string saved = root.GetAttribute(field); root.SetAttribute(field,field == "File" ? "other.wav" : "Changed");
                Reject(() => prepared.VerifyCurrent(root,identity,TargetPlatform.Win32,wave)); root.SetAttribute(field,saved);
            }
            identity.InstanceHash = 1; Reject(() => prepared.VerifyCurrent(root,identity,TargetPlatform.Win32,wave)); identity.InstanceHash = 0;
            identity.TypeHash = 0; Reject(() => Prepare(root,identity,wave)); identity.TypeHash = 0x53C81E47u;
            Reject(() => Prepare(root,new InstanceHandle("AudioEvent",identity.InstanceName) { TypeHash = 0x53C81E47u },wave));
            Reject(() => Ra3Ep1AudioFileInputProfile.Prepare(root,identity,TargetPlatform.Xbox360,wave));
            Reject(() => prepared.VerifyCurrent(root,identity,TargetPlatform.PlayStation3,wave));
            foreach (int length in new[] { 0,43,24043,24045 }) Reject(() => Prepare(root,identity,new byte[length]));
            foreach (int offset in new[] { 0,4,8,12,16,20,22,24,28,32,34,36,40 })
            { byte[] bad = (byte[])wave.Clone(); bad[offset] ^= 1; Reject(() => Prepare(root,identity,bad)); }
            foreach (string[] edit in new[] {
                new[] { "PCCompression","NONE" },new[] { "PCCompression","EALAYER3" },new[] { "PCSampleRate","44100" },
                new[] { "PCSampleRate","=RATE" },new[] { "PCQuality","74" },new[] { "IsStreamedOnPC","TRUE" },
                new[] { "File","../input.wav" },new[] { "File","C:\\input.wav" },new[] { "File","input.wav\\0" },new[] { "File","file://input.wav" },
                new[] { "File","input.mp3" },new[] { "File","input.WAV" },new[] { "File","=FILE.wav" },new[] { "File"," input.wav" },
                new[] { "File",new string('x',129)+".wav" },new[] { "SubtitleStringName","" },new[] { "SubtitleStringName","é" } })
            {
                string saved = root.GetAttribute(edit[0]); root.SetAttribute(edit[0],edit[1]);
                Reject(() => Prepare(root,identity,wave)); root.SetAttribute(edit[0],saved);
            }
            foreach (string field in new[] { "PCSampleRate","PCCompression","IsStreamedOnPC" })
            { string saved = root.GetAttribute(field); root.RemoveAttribute(field); Reject(() => Prepare(root,identity,wave)); root.SetAttribute(field,saved); }
            foreach (string[] option in new[] { new[] { "GUIPreset","Default" },new[] { "XenonCompression","XMA" },new[] { "XenonQuality","75" },
                new[] { "PS3Quality","75" },new[] { "typeHashCode","53C81E47" },new[] { "buildRule","Ignore" },new[] { "inheritFrom","Other" },new[] { "TypeId","0" } })
            {
                XmlElement other = AudioEncoderPoc.CreateDefinition(streamed); other.SetAttribute(option[0],option[1]);
                Reject(() => Prepare(other,AudioEncoderPoc.Identity(other),wave));
            }
            // Reborn: reread the original authored definition after remove/reinsert tests changed attribute ordering; isolate subsequent failures.
            root = AudioEncoderPoc.CreateDefinition(streamed); identity = AudioEncoderPoc.Identity(root);
            prepared.VerifyCurrent(root,identity,TargetPlatform.Win32,wave);
            XmlAttribute foreign = root.OwnerDocument.CreateAttribute("probe","Field","urn:probe"); foreign.Value = "x"; root.Attributes.Append(foreign);
            Reject(() => Prepare(root,identity,wave)); root.RemoveAttributeNode(foreign);
            foreach (XmlNode child in new XmlNode[] { root.OwnerDocument.CreateElement("Child",root.NamespaceURI),root.OwnerDocument.CreateTextNode("x"),
                root.OwnerDocument.CreateProcessingInstruction("probe","x"),root.OwnerDocument.CreateComment(new string('x',8192)) })
            { root.AppendChild(child); Reject(() => Prepare(root,identity,wave)); root.RemoveChild(child); root.IsEmpty = true; }
            XmlComment comment = root.OwnerDocument.CreateComment("changed"); root.AppendChild(comment);
            Prepare(root,identity,wave); Reject(() => prepared.VerifyCurrent(root,identity,TargetPlatform.Win32,wave)); root.RemoveChild(comment);
            // Reborn: removal leaves an explicit closing tag; restore the original empty-element spelling before testing headers/recovery.
            root.IsEmpty = true; prepared.VerifyCurrent(root,identity,TargetPlatform.Win32,wave);
            foreach (byte[] bad in new[] { new byte[1],new byte[7],new byte[9],streamed ? Array.Empty<byte>() : streamHeader })
                Reject(() => prepared.SerializeCurrent(root,identity,TargetPlatform.Win32,wave,bad));
            if (streamed)
                foreach (int offset in new[] { 0,1,2,4,7 })
                { byte[] bad = (byte[])header.Clone(); bad[offset] ^= offset == 1 ? (byte)4 : (byte)1; Reject(() => prepared.SerializeCurrent(root,identity,TargetPlatform.Win32,wave,bad)); }
            AssetBuffer restored = prepared.SerializeCurrent(root,identity,TargetPlatform.Win32,wave,header);
            Require(baseline.InstanceData.SequenceEqual(restored.InstanceData) && baseline.RelocationData.SequenceEqual(restored.RelocationData),"Restored input failed to recover exact output.");
            root.SetAttribute("TypeId",FastHash.GetHashCode("AudioFile").ToString(System.Globalization.CultureInfo.InvariantCulture));
            Prepare(root,identity,wave); Reject(() => prepared.VerifyCurrent(root,identity,TargetPlatform.Win32,wave)); root.RemoveAttribute("TypeId");
        }
        foreach (string play in new[] { "true","True","1","false","False","0" })
        {
            XmlElement root = AudioEncoderPoc.CreateDefinition(false); root.SetAttribute("IsStreamedOnPC",play); root.RemoveAttribute("SubtitleStringName");
            var prepared = Prepare(root,AudioEncoderPoc.Identity(root),wave);
            Require(prepared.Streamed == (play is "true" or "True" or "1") && prepared.Subtitle == "DIALOGEVENT:inputSubTitle","Boolean/default subtitle differs.");
        }
        XmlElement invalid = AudioEncoderPoc.CreateDefinition(false); invalid.SetAttribute("id","Invalid ID");
        Reject(() => Prepare(invalid,AudioEncoderPoc.Identity(invalid),wave));
        XmlDocument unvalidated = new(); unvalidated.LoadXml(AudioEncoderPoc.CreateDefinition(false).OuterXml);
        Reject(() => Prepare(unvalidated.DocumentElement!,AudioEncoderPoc.Identity(unvalidated.DocumentElement!),wave));
        Console.WriteLine("EP1 AudioFile input self-test: OK (official defaults, explicit XAS/PCM profile, immutable snapshots/current identity, settings/WAV/header rejection and recovery; no native codecs/plugin/output)");
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: keep the default input fixture in the only admitted native platform. */
    //-------------------------------------------------------------------------------------------------
    private static Ra3Ep1AudioFileInputProfile.PreparedInput Prepare(XmlElement root,InstanceHandle identity,byte[] wave) => Ra3Ep1AudioFileInputProfile.Prepare(root,identity,TargetPlatform.Win32,wave);

    //-------------------------------------------------------------------------------------------------
    /** Reborn: invalid authored/current inputs must reject, never silently normalize into the narrow profile. */
    //-------------------------------------------------------------------------------------------------
    private static void Reject(Action action)
    {
        try { action(); } catch (Exception error) when (error is NotSupportedException or ArgumentException or XmlSchemaValidationException) { return; }
        throw new InvalidDataException("Unsupported or stale AudioFile input accepted.");
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: stop at the first authored audio contract mismatch. */
    //-------------------------------------------------------------------------------------------------
    private static void Require(bool condition,string message) { if (!condition) throw new InvalidDataException(message); }
}
