using System.Buffers.Binary;
using System.Text;
using BinaryAssetBuilder.Core;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: actual variable event core closure is tested against synthetic framed leaves by default; real codec integration is separately opt-in.
internal static class AudioPoolEventSmokeTest
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: prove singleton/late/reversed selection, ordinal selectors, selected fingerprints and exact stale-source/package/copy rejection. */
    //-------------------------------------------------------------------------------------------------
    internal static void Run()
    {
        foreach (int count in new[] { 1,3,8 })
        {
            string source = Fixture(count); var pool = AuthoredAudioPool.Read(source);
            string work = Path.Combine(Path.GetTempPath(),"Reborn-PoolEventFixture-"+Guid.NewGuid().ToString("N")); AudioEncoderPoc.RunPool(Path.Combine(source,"unused.dll"),work,pool,false);
            string schema = Path.Combine(Path.GetDirectoryName(ReferencePipelineSmokeTest.FindFixture())!,"AudioFileIdentityPipeline.xsd");
            System.IO.Directory.CreateDirectory(Path.Combine(work,"encoded"));
            for (int index = 0; index < count; index++)
            {
                var row = pool.Rows[index]; var core = AudioFileIdentitySmokeTest.Build(work,schema,AudioFileIdentitySmokeTest.Processing,row.Source); var prepared = AudioFileCorePreparation.Prepare(core);
                byte[] header = row.Streamed ? Convert.FromHexString("0400BB8040002EE0") : Array.Empty<byte>(); var native = prepared.SerializeCurrent(core,header);
                string prefix = Path.Combine(work,"encoded","leaf"+index);
                File.WriteAllBytes(prefix+".input.wav",prepared.CopyWave()); File.WriteAllBytes(prefix+".runtime.bin",native.InstanceData); File.WriteAllBytes(prefix+".runtime.relo",native.RelocationData);
                File.WriteAllBytes(prefix+".snr",row.Streamed ? header : Convert.FromHexString("0400BB8000002EE00000000C00002EE0DEADBEEF"));
                if (row.Streamed) File.WriteAllBytes(prefix+".sns",Convert.FromHexString("8000000C00002EE0DEADBEEF"));
            }
            AudioPoolResultGate.Publish(work,pool); Exercise(work,source,count);
        }
        Console.WriteLine("Pool event self-test: OK (1/3/8 pools, one/two late/reversed Sound targets, real core recompilation, ordinal selectors and selected hashes; aliases/unknown/duplicates/oversized selection and stale/corrupt evidence reject; synthetic audio framing, no codec/mixed publication).");
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: encode a real eight-leaf package once, then compile independent event selections without changing its sources or publishing a mixed graph. */
    //-------------------------------------------------------------------------------------------------
    internal static void NativeProof(string library)
    {
        string source = Fixture(8); var pool = AuthoredAudioPool.Read(source);
        string job = AudioEncoderSupervisor.Run(library,"encode-pool-package",pool:pool);
        Exercise(Path.Combine(job,"worker"),source,8);
        Console.WriteLine("Pool event native integration: OK (actual eight-leaf XAS package + real managed event closure/recompilation/staleness; no mixed package or playback proof).");
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: verify selection slots against event-local import ordinals and revoke preflight when any original/copied evidence changes. */
    //-------------------------------------------------------------------------------------------------
    private static void Exercise(string work,string source,int count)
    {
        var pool = AuthoredAudioPool.Read(work); var entries = AudioPoolResultGate.Verify(work,pool,true,true);
        string path = Path.Combine(source,"requested-event.xml");
        foreach (int[] slots in count == 1 ? new[] { new[] { 0 } } : new[] { new[] { count-1 },new[] { count-1,0 } })
        {
            byte[] original = Encoding.UTF8.GetBytes(Source(slots)); File.WriteAllBytes(path,original);
            var proof = AudioPoolEventPreflight.Run(work,path); var native = proof.Event.CopyNative();
            Require(proof.Event.Settings.Slots().SequenceEqual(slots) && native.InstanceData.Length == 152+12*slots.Length && native.ImportsData.Length == 4*(slots.Length+1),"Pool event selection/wire size differs.");
            for (int index = 0; index < slots.Length; index++) Require(BinaryPrimitives.ReadUInt32LittleEndian(native.InstanceData.AsSpan(152+12*index)) == index+1,"Event selector was incorrectly bound to pool index.");
            Require(proof.Event.References(entries).Select(reference => reference.InstanceId).SequenceEqual(slots.Select(slot => entries[slot].Id)),"Event reference order differs.");
            int selected = slots[0]; var changed = (AudioFilePackageProbe.Entry[])entries.Clone(); byte[] custom = changed[selected].CopyCustom(); custom[^1] ^= 1;
            changed[selected] = new(changed[selected].Name,changed[selected].Source,changed[selected].CopyNative(),custom,changed[selected].Streamed);
            Reject(() => proof.Event.ValidateDependencies(changed));
            if (count > slots.Length)
            {
                int unselected = Enumerable.Range(0,count).First(index => !slots.Contains(index)); changed = (AudioFilePackageProbe.Entry[])entries.Clone(); custom = changed[unselected].CopyCustom(); custom[^1] ^= 1;
                changed[unselected] = new(changed[unselected].Name,changed[unselected].Source,changed[unselected].CopyNative(),custom,changed[unselected].Streamed); proof.Event.ValidateDependencies(changed);
            }
            DateTime timestamp = File.GetLastWriteTimeUtc(path); File.WriteAllBytes(path,Encoding.UTF8.GetBytes(Source(slots).Replace("Volume=\"37.5\"","Volume=\"37.6\""))); File.SetLastWriteTimeUtc(path,timestamp); Reject(proof.VerifyCurrent); File.WriteAllBytes(path,original);
            string copy = Path.Combine(proof.Directory,"event.xml"); File.WriteAllBytes(copy,"bad"u8.ToArray()); Reject(proof.VerifyCurrent); File.WriteAllBytes(copy,original);
            // Reborn: returned raw event artifacts must remain identical to the immutable compiled buffers, not merely retain valid lengths.
            foreach (string name in new[] { "event.bin","event.relo","event.imp" })
            { string output = Path.Combine(proof.Directory,name); byte[] saved = File.ReadAllBytes(output),altered = (byte[])saved.Clone(); altered[0] ^= 1; File.WriteAllBytes(output,altered); Reject(proof.VerifyCurrent); File.WriteAllBytes(output,saved); }
            string package = Path.Combine(work,"package","diagnostic.bin"); byte[] bytes = File.ReadAllBytes(package),bad = (byte[])bytes.Clone(); bad[0] ^= 1; File.WriteAllBytes(package,bad); Reject(proof.VerifyCurrent); File.WriteAllBytes(package,bytes); proof.VerifyCurrent();
            Reject(() => AudioFilePackageProbe.Serialize(entries,proof.Event,variable:true));
        }
        foreach (string invalid in new[] { Source(new[] { 0 }).Replace("PoolAsset_0","poolasset_0"),Source(new[] { 0 }).Replace("PoolAsset_0","Unknown"),Source(new[] { 0,0 }),Source(new[] { 0,0,0 }),Source(new[] { 0 }).Replace("<Sound ","<Attack ").Replace("</Sound>","</Attack>") })
        { File.WriteAllBytes(path,Encoding.UTF8.GetBytes(invalid)); Reject(() => AudioPoolEventPreflight.Run(work,path)); }
        File.WriteAllBytes(path,Encoding.UTF8.GetBytes(Source(new[] { 0 })));
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: produce only literal bounded event XML with independently selected pool names and proven control/scalar fields. */
    //-------------------------------------------------------------------------------------------------
    private static string Source(int[] slots) => "<AssetDeclaration xmlns=\"uri:ea.com:eala:asset\"><AudioEvent id=\"PoolEvent\" Volume=\"37.5\" Control=\"LOOP INTERRUPT\">"+string.Concat(slots.Select(slot => "<Sound Weight=\"125\">AudioFile:PoolAsset_"+slot+"</Sound>"))+"</AudioEvent></AssetDeclaration>";

    //-------------------------------------------------------------------------------------------------
    /** Reborn: create source/event fixtures only in an exclusive fresh owned directory. */
    //-------------------------------------------------------------------------------------------------
    private static string Fixture(int count) { string path = Path.Combine(Path.GetTempPath(),"Reborn-PoolEventTest-"+Guid.NewGuid().ToString("N")); System.IO.Directory.CreateDirectory(path); AuthoredAudioPoolSmokeTest.Fixture(path,count,false); return path; }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: validation failures must revoke closure rather than silently accepting an unsupported graph. */
    //-------------------------------------------------------------------------------------------------
    private static void Reject(Action action) { try { action(); } catch (Exception error) when (error is InvalidDataException or NotSupportedException) { return; } throw new InvalidDataException("Invalid pool event evidence succeeded."); }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: enforce source-derived reference/order and raw event layout expectations. */
    //-------------------------------------------------------------------------------------------------
    private static void Require(bool condition,string message) { if (!condition) throw new InvalidDataException(message); }
}
