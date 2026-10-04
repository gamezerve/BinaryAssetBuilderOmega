using System.Text;
using System.Text.Json;
using System.Xml;
using System.Xml.Schema;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: exercise variable source inventories and real core preparations without native codecs or production output.
internal static class AuthoredAudioPoolSmokeTest
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: prove cardinality/order/shared dependencies, frozen ownership, strict admission and timestamp-preserving source recovery. */
    //-------------------------------------------------------------------------------------------------
    internal static void Run()
    {
        string root = Path.Combine(Path.GetTempPath(),"Reborn-PoolTest-"+Guid.NewGuid().ToString("N")); Directory.CreateDirectory(root);
        foreach (var shape in new[] { (Count:1,Shared:false),(Count:3,Shared:false),(Count:8,Shared:false),(Count:8,Shared:true) })
        {
            string directory = Path.Combine(root,Guid.NewGuid().ToString("N")); Directory.CreateDirectory(directory); Fixture(directory,shape.Count,shape.Shared);
            var pool = AuthoredAudioPool.Read(directory); var preflight = pool.Preflight();
            Require(pool.Rows.Length == shape.Count && preflight.Rows.Select(row => row.Name).SequenceEqual(pool.Rows.Select(row => row.Name)),"Pool order/core count differs.");
            Require(pool.FileNames.Length == 1+shape.Count+(shape.Shared ? 1 : shape.Count),"Pool shared dependency accounting differs.");
            Require(preflight.Rows.Select(row => row.Id).Distinct().Count() == shape.Count && preflight.Rows.All(row => row.CoreHash != 0),"Pool core identity evidence differs.");
            Require(!Directory.EnumerateFiles(preflight.Directory,"*.manifest").Any() && !Directory.EnumerateFiles(preflight.Directory,"*.bin").Any(),"Preflight emitted binary output.");
            byte[] clone = pool.Copy(pool.Rows[0].Wave); clone[100] ^= 1; var rows = pool.Rows; rows[0] = rows[0] with { Name = "Changed" };
            pool.VerifyCurrent(); pool.VerifyCopies(preflight.Directory); Require(pool.Rows[0].Name != "Changed","Pool metadata alias escaped.");
            Reject(() => pool.Install(preflight.Directory));
        }
        string target = Path.Combine(root,"negative"); Directory.CreateDirectory(target); Fixture(target,3,false);
        byte[] inventory = File.ReadAllBytes(Path.Combine(target,"audio-pool.json")),source = File.ReadAllBytes(Path.Combine(target,"tone0.xml")),wave = File.ReadAllBytes(Path.Combine(target,"wave0.wav"));
        var frozen = AuthoredAudioPool.Read(target);
        foreach (string invalid in new[] { "[]","{}","{\"version\":2,\"sources\":[\"tone0.xml\"]}","{\"version\":1,\"sources\":[]}",
            "{\"version\":1,\"version\":1,\"sources\":[\"tone0.xml\"]}","{\"version\":1,\"sources\":[\"tone0.xml\"],\"unknown\":1}",
            "{\"version\":1,\"sources\":[null]}","{\"version\":1,\"sources\":[\"../tone0.xml\"]}","{\"version\":1,\"sources\":[\"tone0.xml\",\"TONE0.xml\"]}",
            "{\"version\":1,\"sources\":[\"CON.xml\"]}","{\"version\":1,\"sources\":[\"LPT1.xml\"]}",
            JsonSerializer.Serialize(new { version = 1,sources = Enumerable.Repeat("tone0.xml",9).ToArray() }) })
        { Write(target,"audio-pool.json",Encoding.UTF8.GetBytes(invalid)); Reject(() => AuthoredAudioPool.Read(target)); }
        foreach (byte[] invalid in new[] { new byte[4097],new byte[] { 0xFF },new byte[] { 0xEF,0xBB,0xBF }.Concat(inventory).ToArray() })
        { Write(target,"audio-pool.json",invalid); Reject(() => AuthoredAudioPool.Read(target)); } Write(target,"audio-pool.json",inventory);
        string xml = Encoding.UTF8.GetString(source);
        foreach (string invalid in new[] { xml.Replace("PoolAsset_0","poolAsset_1"),xml.Replace("wave0.wav","../wave0.wav"),xml.Replace("XAS","NONE"),
            xml.Replace("<AudioFile","<Includes /><AudioFile"),xml.Replace("<AssetDeclaration","<!DOCTYPE x [<!ENTITY e 'x'>]><AssetDeclaration"),
            xml.Replace("<AudioFile","<AudioFile inheritFrom=\"Other\""),xml.Replace("PoolAsset_0","Bad:Name"),xml.Replace("wave0.wav","NUL.extra.wav"),
            "<?xml version=\"1.0\" encoding=\"utf-16\"?>"+xml })
        { Write(target,"tone0.xml",Encoding.UTF8.GetBytes(invalid)); Reject(() => AuthoredAudioPool.Read(target)); }
        foreach (byte[] invalid in new[] { new byte[8193],new byte[] { 0xFF },new byte[] { 0xEF,0xBB,0xBF }.Concat(source).ToArray() })
        { Write(target,"tone0.xml",invalid); Reject(() => AuthoredAudioPool.Read(target)); }
        Write(target,"tone0.xml",source);
        foreach (byte[] invalid in new[] { new byte[24045],Array.Empty<byte>() })
        { Write(target,"wave0.wav",invalid); Reject(() => AuthoredAudioPool.Read(target)); } Write(target,"wave0.wav",wave);
        DateTime timestamp = File.GetLastWriteTimeUtc(Path.Combine(target,"wave0.wav")); byte[] changed = (byte[])wave.Clone(); changed[100] ^= 1;
        Write(target,"wave0.wav",changed); File.SetLastWriteTimeUtc(Path.Combine(target,"wave0.wav"),timestamp); Reject(frozen.VerifyCurrent);
        Write(target,"wave0.wav",wave); frozen.VerifyCurrent();
        Write(target,"tone0.xml",Encoding.UTF8.GetBytes(xml.Replace("PoolAsset_0","PoolAsset_X"))); Reject(frozen.VerifyCurrent); Write(target,"tone0.xml",source);
        Write(target,"audio-pool.json",JsonSerializer.SerializeToUtf8Bytes(new { version = 1,sources = new[] { "tone2.xml","tone1.xml","tone0.xml" } }));
        Reject(frozen.VerifyCurrent); Write(target,"audio-pool.json",inventory); frozen.VerifyCurrent();
        var copied = frozen.Preflight(); Write(copied.Directory,"wave0.wav",changed); Reject(() => frozen.VerifyCopies(copied.Directory)); Write(copied.Directory,"wave0.wav",wave);
        frozen.VerifyCopies(copied.Directory); frozen.Preflight();
        Console.WriteLine("Authored audio pool self-test: OK (1/3/8 leaves, distinct/shared PCM, actual core preparations, immutable source inventory/order, aliases/paths/DTD/settings/size/stale-copy rejection; no native/event/stream/production admission).");
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: generate only owned short PCM/XML fixtures; both play locations retain the existing narrow input profile. */
    //-------------------------------------------------------------------------------------------------
    internal static void Fixture(string directory,int count,bool shared)
    {
        Write(directory,"audio-pool.json",JsonSerializer.SerializeToUtf8Bytes(new { version = 1,sources = Enumerable.Range(0,count).Select(index => "tone"+index+".xml").ToArray() }));
        using MemoryStream pcm = new(); AudioEncoderPoc.WriteWave(pcm); byte[] wave = pcm.ToArray();
        for (int index = 0; index < count; index++)
        {
            bool streamed = index%2 != 0; string leaf = shared ? "shared.wav" : "wave"+index+".wav";
            string xml = AudioEncoderPoc.CoreSource(streamed).Replace(streamed ? "RebornAudioStream" : "RebornAudioRAM","PoolAsset_"+index).Replace("input.wav",leaf);
            Write(directory,"tone"+index+".xml",Encoding.UTF8.GetBytes(xml)); if (!shared || index == 0) Write(directory,leaf,wave);
        }
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: test writes affect only the exact owned fixture directories created by this test. */
    //-------------------------------------------------------------------------------------------------
    private static void Write(string directory,string name,byte[] bytes)
    { using FileStream writer = new(Path.Combine(directory,name),FileMode.Create,FileAccess.Write); writer.Write(bytes); }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: reject malformed/stale source evidence before any native encoder launch. */
    //-------------------------------------------------------------------------------------------------
    private static void Reject(Action action)
    { try { action(); } catch (Exception error) when (error is InvalidDataException or IOException or NotSupportedException or XmlException or XmlSchemaException or JsonException or DecoderFallbackException) { return; } throw new InvalidDataException("Invalid authored pool unexpectedly succeeded."); }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: stop immediately when pool metadata/core evidence disagrees. */
    //-------------------------------------------------------------------------------------------------
    private static void Require(bool condition,string message) { if (!condition) throw new InvalidDataException(message); }
}
