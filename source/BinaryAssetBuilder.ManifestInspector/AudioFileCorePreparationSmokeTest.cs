using System.Text;
using System.Xml;
using BinaryAssetBuilder.Core;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: exercise real core-to-authored preparation with current disk changes and synthetic runtime headers, never native codecs.
internal static class AudioFileCorePreparationSmokeTest
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: admit RAM/streamed direct files, reject stale XML/PCM/metadata and recover only after restoring or rebuilding inputs. */
    //-------------------------------------------------------------------------------------------------
    internal static void Run()
    {
        string parent = Path.Combine(Path.GetTempPath(),"Reborn-CoreAudio-"+Guid.NewGuid().ToString("N")); Directory.CreateDirectory(parent);
        string schema = Path.Combine(Path.GetDirectoryName(ReferencePipelineSmokeTest.FindFixture())!,"AudioFileIdentityPipeline.xsd");
        using MemoryStream pcm = new(); AudioEncoderPoc.WriteWave(pcm); byte[] wave = pcm.ToArray();
        foreach (bool streamed in new[] { false,true })
        {
            string directory = Path.Combine(parent,streamed ? "streamed" : "ram"); Directory.CreateDirectory(directory);
            string xml = $"<AssetDeclaration xmlns=\"uri:ea.com:eala:asset\"><AudioFile id=\"RebornCoreAudio\" File=\"input.wav\" PCSampleRate=\"48000\" PCCompression=\"XAS\" IsStreamedOnPC=\"{(streamed ? "true" : "false")}\" /></AssetDeclaration>";
            string source = Path.Combine(directory,"audio.xml"),file = Path.Combine(directory,"input.wav");
            Write(source,Encoding.UTF8.GetBytes(xml),true); Write(file,wave,true);
            InstanceDeclaration instance = Build(); var prepared = AudioFileCorePreparation.Prepare(instance);
            byte[] header = streamed ? Convert.FromHexString("0400BB8040002EE0") : Array.Empty<byte>();
            AssetBuffer baseline = prepared.SerializeCurrent(instance,header);
            byte[] detached = prepared.CopyWave(); detached[100] ^= 1; prepared.VerifyCurrent(instance);
            XmlElement root = (XmlElement)instance.XmlNode;
            string path = root.GetAttribute("File"); root.SetAttribute("File",Path.Combine(directory,"other.wav")); Reject(() => prepared.VerifyCurrent(instance)); root.SetAttribute("File",path);
            root.SetAttribute("PCSampleRate","44100"); Reject(() => prepared.VerifyCurrent(instance)); root.SetAttribute("PCSampleRate","48000");
            uint hash = instance.Handle.InstanceHash; instance.Handle.InstanceHash ^= 1; Reject(() => prepared.VerifyCurrent(instance)); instance.Handle.InstanceHash = hash;
            instance.ReferencedFiles.Add("extra.wav"); Reject(() => prepared.VerifyCurrent(instance)); instance.ReferencedFiles.RemoveAt(1);
            instance.ReferencedInstances.Add(new InstanceHandle("AudioFile","Unexpected")); Reject(() => prepared.VerifyCurrent(instance)); instance.ReferencedInstances.Clear();
            uint typeHash = instance.Handle.TypeHash; instance.Handle.TypeHash ^= 1; Reject(() => prepared.VerifyCurrent(instance)); instance.Handle.TypeHash = typeHash;
            uint processing = instance.ProcessingHash; instance.ProcessingHash ^= 1; Reject(() => prepared.VerifyCurrent(instance)); instance.ProcessingHash = processing;
            DateTime timestamp = File.GetLastWriteTimeUtc(file);
            byte[] changed = (byte[])wave.Clone(); changed[100] ^= 1; Write(file,changed,false); File.SetLastWriteTimeUtc(file,timestamp);
            Reject(() => prepared.SerializeCurrent(instance,header));
            InstanceDeclaration refreshed = Build(); var refreshedPreparation = AudioFileCorePreparation.Prepare(refreshed);
            Reject(() => prepared.VerifyCurrent(refreshed)); refreshedPreparation.SerializeCurrent(refreshed,header);
            Write(file,wave,false); File.SetLastWriteTimeUtc(file,timestamp); prepared.VerifyCurrent(instance);
            // Reborn: temporarily move only this owned fixture, restore it even if the missing-file assertion fails.
            string moved = Path.Combine(directory,"saved-input.wav"); File.Move(file,moved);
            try { Reject(() => prepared.VerifyCurrent(instance)); }
            finally { File.Move(moved,file); }
            Write(source,Encoding.UTF8.GetBytes(xml.Replace("input.wav","absent.wav")),false); Reject(() => prepared.VerifyCurrent(instance));
            Write(source,Encoding.UTF8.GetBytes(xml.Replace("PCCompression=\"XAS\"","PCCompression=\"NONE\"")),false); Reject(() => prepared.VerifyCurrent(instance));
            Write(source,Encoding.UTF8.GetBytes(xml.Replace("input.wav","../input.wav")),false); Reject(() => prepared.VerifyCurrent(instance));
            Write(source,Encoding.UTF8.GetBytes(xml.Replace("<AssetDeclaration","<!DOCTYPE x [<!ENTITY e 'x'>]><AssetDeclaration")),false); Reject(() => prepared.VerifyCurrent(instance));
            Write(source,new byte[8193],false); Reject(() => prepared.VerifyCurrent(instance));
            Write(source,Encoding.UTF8.GetBytes(xml),false); Write(file,new byte[24045],false); Reject(() => prepared.VerifyCurrent(instance)); Write(file,wave,false);
            Reject(() => prepared.SerializeCurrent(instance,new byte[streamed ? 7 : 8]));
            AssetBuffer restored = prepared.SerializeCurrent(instance,header);
            Require(baseline.InstanceData.SequenceEqual(restored.InstanceData) && baseline.RelocationData.SequenceEqual(restored.RelocationData),"Core AudioFile preparation recovery changed runtime bytes.");

            //-------------------------------------------------------------------------------------------------
            /** Reborn: rebuild the actual core instance with fresh cache after disk changes, not by editing an identity field. */
            //-------------------------------------------------------------------------------------------------
            InstanceDeclaration Build() => AudioFileIdentitySmokeTest.Build(directory,schema,AudioFileIdentitySmokeTest.Processing);
        }
        Console.WriteLine("Core AudioFile preparation self-test: OK (RAM/streamed real core identities, disk XML/PCM and timestamp-preserving edits, metadata rejection, detached copies, refresh/recovery; synthetic headers, no native codecs/output)");
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: mutate only this test's owned GUID fixture files and use exclusive creation for initial fixtures. */
    //-------------------------------------------------------------------------------------------------
    private static void Write(string path,byte[] bytes,bool create)
    { using FileStream stream = new(path,create ? FileMode.CreateNew : FileMode.Create,FileAccess.Write); stream.Write(bytes); }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: changed inputs must fail before any runtime output becomes available. */
    //-------------------------------------------------------------------------------------------------
    private static void Reject(Action action)
    {
        try { action(); }
        catch (Exception exception) when (exception is InvalidDataException or NotSupportedException or XmlException or System.Xml.Schema.XmlSchemaException or IOException) { return; }
        throw new InvalidDataException("Changed core AudioFile preparation unexpectedly succeeded.");
    }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: fail the first immutable-preparation mismatch. */
    //-------------------------------------------------------------------------------------------------
    private static void Require(bool condition,string message) { if (!condition) throw new InvalidDataException(message); }
}
