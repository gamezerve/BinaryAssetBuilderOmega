using System.Buffers.Binary;
using System.Text;
using System.Text.Json;
using BinaryAssetBuilder.Core;
using BinaryAssetBuilder.XmlCompiler;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: validate explicit duration pools with managed transport by default; actual codec proofs are separately requested.
internal static class AudioDurationPoolSmokeTest
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: test versioned admission, actual current core identities and isolated metadata-only workers without codecs. */
    //-------------------------------------------------------------------------------------------------
    internal static void Run()
    {
        foreach (var shape in new[] { (Samples:new[] { 12000,24000,48000,96000 },Shared:false),(Samples:Enumerable.Repeat(96000,8).ToArray(),Shared:false),(Samples:Enumerable.Repeat(96000,8).ToArray(),Shared:true) })
        {
            string source = Fixture(shape.Samples,shape.Shared); var pool = AuthoredAudioPool.Read(source,durationCandidate:true);
            Require(pool.DurationCandidate && pool.Rows.Length == shape.Samples.Length,"Duration admission/count differs.");
            Reject(() => AuthoredAudioPool.Read(source));
            Reject(() => AuthoredAudioPool.Read(source,includeEvent:true,durationCandidate:true));
            Reject(() => AudioEncoderSupervisor.Run(UnusedLibrary(),"preflight-pool",pool:pool));
            string job = AudioEncoderSupervisor.Run(UnusedLibrary(),"preflight-pool-duration",pool:pool);
            string work = Path.Combine(job,"worker"); AudioPoolResultGate.Verify(work,pool,false);
            Require(File.Exists(Path.Combine(job,"ACCEPTED.json")),"Duration preflight was not accepted.");
            AudioPoolWorkerSmokeTest.RejectForgedResult(work,pool,false,"pool-core.json");
            string schema = Path.Combine(Path.GetDirectoryName(ReferencePipelineSmokeTest.FindFixture())!,"AudioFileIdentityPipeline.xsd");
            for (int index = 0; index < pool.Rows.Length; index++)
            {
                var row = pool.Rows[index]; var core = AudioFileIdentitySmokeTest.Build(work,schema,AudioFileIdentitySmokeTest.Processing,row.Source);
                var prepared = AudioFileCorePreparation.Prepare(core,true); prepared.VerifyCurrent(core);
                Require(prepared.Settings.Samples == shape.Samples[index],"Core duration is not PCM-derived.");
                if (shape.Samples[index] != 12000) Reject(() => AudioFileCorePreparation.Prepare(core));
                var entry = Synthetic(row,prepared.Settings.Samples);
                Reject(() => new AudioFilePackageProbe.Entry(row.Name,row.Source,entry.CopyNative(),entry.CopyCustom(),row.Streamed,prepared.Settings.Samples+1));
                if (entry.Samples != 12000)
                {
                    Reject(() => new AudioFilePackageProbe.Entry(row.Name,row.Source,entry.CopyNative(),entry.CopyCustom(),row.Streamed));
                    Reject(() => new AudioFilePackageProbe.Entry(row.Name,row.Streamed ? "streamed.xml" : "ram.xml",entry.CopyNative(),entry.CopyCustom(),expectedSamples:entry.Samples));
                }
                string output = Path.Combine(Path.GetTempPath(),"Reborn-DurationSynthetic-"+Guid.NewGuid().ToString("N"));
                AudioFilePackageProbe.Publish(output,new[] { entry },variable:true); AudioFilePackageProbe.Verify(output,new[] { entry },variable:true);
                // Reborn: timestamp-preserving PCM edits invalidate both immutable source and current-core preparation.
                string path = Path.Combine(work,row.Wave); byte[] original = File.ReadAllBytes(path),changed = (byte[])original.Clone(); DateTime timestamp = File.GetLastWriteTimeUtc(path);
                changed[100] ^= 1; File.WriteAllBytes(path,changed); File.SetLastWriteTimeUtc(path,timestamp);
                Reject(() => prepared.VerifyCurrent(core)); Reject(() => pool.VerifyCopies(work));
                File.WriteAllBytes(path,original); prepared.VerifyCurrent(core); pool.VerifyCopies(work);
            }
            pool.VerifyCurrent();
        }
        foreach (bool original in new[] { false,true })
        {
            string source = Fixture(new[] { 96000 },false); var pool = AuthoredAudioPool.Read(source,durationCandidate:true); string? rejectedJob = null;
            Reject(() => AudioEncoderSupervisor.Run(UnusedLibrary(),"preflight-pool-duration",jobCreated:job =>
            {
                rejectedJob = job; string path = Path.Combine(original ? source : Path.Combine(job,"inputs"),"wave0.wav");
                byte[] changed = File.ReadAllBytes(path); changed[100] ^= 1; File.WriteAllBytes(path,changed);
            },pool:pool));
            Require(rejectedJob != null && !File.Exists(Path.Combine(rejectedJob,"ACCEPTED.json")),"Stale duration job was accepted.");
        }
        string negative = Fixture(new[] { 48000 },false); byte[] inventory = File.ReadAllBytes(Path.Combine(negative,"audio-pool.json"));
        File.WriteAllBytes(Path.Combine(negative,"audio-pool.json"),JsonSerializer.SerializeToUtf8Bytes(new { version = 1,sources = new[] { "tone0.xml" } }));
        Reject(() => AuthoredAudioPool.Read(negative,durationCandidate:true)); Reject(() => AuthoredAudioPool.Read(negative));
        File.WriteAllBytes(Path.Combine(negative,"audio-pool.json"),inventory);
        foreach (int samples in new[] { 11999,96001 })
        { File.WriteAllBytes(Path.Combine(negative,"wave0.wav"),AudioDurationCandidateSmokeTest.Wave(samples)); Reject(() => AuthoredAudioPool.Read(negative,durationCandidate:true)); }
        // Reborn: canonical source snapshots cannot be promoted into a duration worker mode without versioned explicit admission.
        string canonical = Path.Combine(Path.GetTempPath(),"Reborn-DurationLegacy-"+Guid.NewGuid().ToString("N")); Directory.CreateDirectory(canonical);
        AuthoredAudioPoolSmokeTest.Fixture(canonical,1,false); var legacyPool = AuthoredAudioPool.Read(canonical);
        Reject(() => AudioEncoderSupervisor.Run(UnusedLibrary(),"preflight-pool-duration",pool:legacyPool));
        Console.WriteLine("Audio duration pool self-test: OK (version-2 admission, 4/8-source distinct/shared core/managed-worker proofs, two-reader synthetic packages, legacy/mode/stale/forged rejection; no native codecs)");
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: encode duration boundaries and unaligned sample tails only in supervised opt-in native children. */
    //-------------------------------------------------------------------------------------------------
    internal static void NativeProof(string library)
    {
        foreach (var shape in new[] { (Samples:new[] { 12000,12000,24000,24000,48000,48000,96000,96000 },Shared:false,AllStreamed:false),(Samples:new[] { 12001,12001,47999,47999,95999,95999 },Shared:false,AllStreamed:false),(Samples:Enumerable.Repeat(96000,8).ToArray(),Shared:false,AllStreamed:false),(Samples:Enumerable.Repeat(96000,8).ToArray(),Shared:true,AllStreamed:false),(Samples:Enumerable.Repeat(96000,8).ToArray(),Shared:false,AllStreamed:true) })
        {
            string source = Fixture(shape.Samples,shape.Shared);
            // Reborn: exercise maximum streamed artifact membership only by changing fresh owned fixture XML.
            if (shape.AllStreamed) foreach (string path in Directory.EnumerateFiles(source,"*.xml")) File.WriteAllText(path,File.ReadAllText(path).Replace("IsStreamedOnPC=\"false\"","IsStreamedOnPC=\"true\""),new UTF8Encoding(false));
            var pool = AuthoredAudioPool.Read(source,durationCandidate:true);
            string job = AudioEncoderSupervisor.Run(library,"encode-pool-duration-package",pool:pool),work = Path.Combine(job,"worker");
            var entries = AudioPoolResultGate.Verify(work,pool,true,true);
            Require(entries.Select(entry => entry.Samples).SequenceEqual(shape.Samples),"Native duration sample totals differ from source.");
            foreach (string leaf in new[] { "encoded/leaf0.runtime.bin","encoded/leaf0.input.wav","encoded/leaf0.snr","pool-core.json","package/diagnostic.manifest","package/diagnostic.bin" })
                AudioPoolWorkerSmokeTest.RejectForgedResult(work,pool,true,leaf,true,leaf.EndsWith("runtime.bin",StringComparison.Ordinal) ? 12 : 0);
            AudioPoolWorkerSmokeTest.RejectForgedResult(work,pool,true,"package/diagnostic/cdata/"+entries[0].CustomName,true,4);
            // Reborn: alter an owned streamed custom block count with a matching inventory; parent source totals must still reject.
            int streamed = Array.FindIndex(pool.Rows,row => row.Streamed);
            AudioPoolWorkerSmokeTest.RejectForgedResult(work,pool,true,"encoded/leaf"+streamed+".sns",true,4);
            AudioPoolResultGate.Verify(work,pool,true,true); pool.VerifyCurrent();
            var artifacts = AudioEncoderSupervisor.Inventory(work,80);
            Console.WriteLine($"Duration native evidence: samples={string.Join(',',shape.Samples)} shared={shape.Shared} allStreamed={shape.AllStreamed} files={artifacts.Length} bytes={artifacts.Sum(item => item.Length)} job={job}");
        }
        Console.WriteLine("Audio duration native proof: OK (250/500/1000/2000ms RAM+streamed, unaligned sample tails, 8 maximum distinct/shared leaves, exact parent/two-reader packages and forged-total rejection; no decode/game-load proof)");
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: freeze synthetic sample-matched custom framing for managed package tests, not codec correctness evidence. */
    //-------------------------------------------------------------------------------------------------
    private static AudioFilePackageProbe.Entry Synthetic(AuthoredAudioPool.Row row,int samples)
    {
        byte[] header = Convert.FromHexString("0400BB8000000000"); BinaryPrimitives.WriteUInt32BigEndian(header.AsSpan(4,4),(uint)samples|(row.Streamed ? 0x40000000u : 0u));
        byte[] block = Convert.FromHexString(row.Streamed ? "8000000C00000000DEADBEEF" : "0000000C00000000DEADBEEF");
        BinaryPrimitives.WriteUInt32BigEndian(block.AsSpan(4,4),(uint)samples);
        AssetBuffer native = Ra3Ep1AudioFileRuntimeSerializer.Serialize(TargetPlatform.Win32,"DurationSubtitle",samples,48000,1,row.Streamed ? header : Array.Empty<byte>());
        return new(row.Name,row.Source,native,row.Streamed ? block : header.Concat(block).ToArray(),row.Streamed,samples);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: generate only owned version-2 source fixtures; caller/game sources remain untouched. */
    //-------------------------------------------------------------------------------------------------
    private static string Fixture(int[] samples,bool shared)
    {
        string root = Path.Combine(Path.GetTempPath(),"Reborn-DurationPool-"+Guid.NewGuid().ToString("N")); Directory.CreateDirectory(root);
        AuthoredAudioPoolSmokeTest.Fixture(root,samples.Length,shared);
        File.WriteAllBytes(Path.Combine(root,"audio-pool.json"),JsonSerializer.SerializeToUtf8Bytes(new { version = 2,sources = Enumerable.Range(0,samples.Length).Select(index => "tone"+index+".xml").ToArray() }));
        for (int index = 0; index < samples.Length; index++)
        {
            if (shared && index != 0) continue;
            byte[] wave = AudioDurationCandidateSmokeTest.Wave(samples[index]);
            // Reborn: use a reproducible non-silent PCM tone while preserving the exact canonical header and candidate data length.
            for (int sample = 0; sample < samples[index]; sample++) BinaryPrimitives.WriteInt16LittleEndian(wave.AsSpan(44+2*sample,2),(short)(Math.Sin(2*Math.PI*440*sample/48000)*8192));
            File.WriteAllBytes(Path.Combine(root,shared ? "shared.wav" : "wave"+index+".wav"),wave);
        }
        return root;
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: default managed transport has no dependency on an existing or loadable native library. */
    //-------------------------------------------------------------------------------------------------
    private static string UnusedLibrary() => Path.Combine(Path.GetTempPath(),"unused-native-library.dll");

    //-------------------------------------------------------------------------------------------------
    /** Reborn: invalid duration evidence must reject before any acceptance marker is emitted. */
    //-------------------------------------------------------------------------------------------------
    private static void Reject(Action action)
    { try { action(); } catch (Exception error) when (error is InvalidDataException or IOException or NotSupportedException) { return; } throw new InvalidDataException("Invalid duration pool unexpectedly accepted."); }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: stop at the first mismatch between independently expected duration and current evidence. */
    //-------------------------------------------------------------------------------------------------
    private static void Require(bool condition,string message) { if (!condition) throw new InvalidDataException(message); }
}
