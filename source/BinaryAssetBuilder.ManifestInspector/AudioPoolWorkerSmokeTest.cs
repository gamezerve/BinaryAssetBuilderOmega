namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: test real pool worker transport with managed-only fixtures; native execution is a separate explicit proof.
internal static class AudioPoolWorkerSmokeTest
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: verify bounded pool acceptance and parent semantic/stale-original/input-copy rejection without loading any codec. */
    //-------------------------------------------------------------------------------------------------
    internal static void Run()
    {
        foreach (var shape in new[] { (Count:1,Shared:false),(Count:3,Shared:false),(Count:8,Shared:true) })
        {
            string source = Fixture(shape.Count,shape.Shared); var pool = AuthoredAudioPool.Read(source);
            string job = AudioEncoderSupervisor.Run(UnusedLibrary(),"preflight-pool",pool:pool);
            if (!File.Exists(Path.Combine(job,"ACCEPTED.json"))) throw new InvalidDataException("Managed pool was not accepted.");
            string work = Path.Combine(job,"worker"),metadata = Path.Combine(work,"pool-core.json"); byte[] original = File.ReadAllBytes(metadata);
            File.WriteAllBytes(metadata,"[]"u8.ToArray()); Reject(() => AudioPoolResultGate.Verify(work,pool,false)); File.WriteAllBytes(metadata,original);
            // Reborn: a forged result with an updated matching hash inventory must still fail independent parent semantics.
            RejectForgedResult(work,pool,false,"pool-core.json");
            string extra = Path.Combine(work,"unexpected.txt"); File.WriteAllBytes(extra,new byte[] { 1 }); Reject(() => AudioPoolResultGate.Verify(work,pool,false)); File.Delete(extra);
            AudioPoolResultGate.Verify(work,pool,false);
        }
        foreach (bool mutateOriginal in new[] { false,true })
        {
            string source = Fixture(3,false); var pool = AuthoredAudioPool.Read(source); string? rejectedJob = null;
            string wave = Path.Combine(source,"wave0.wav"); byte[] original = File.ReadAllBytes(wave);
            Reject(() => AudioEncoderSupervisor.Run(UnusedLibrary(),"preflight-pool",jobCreated:job =>
            {
                rejectedJob = job;
                string target = mutateOriginal ? wave : Path.Combine(job,"inputs","wave0.wav");
                byte[] changed = File.ReadAllBytes(target); changed[100] ^= 1; File.WriteAllBytes(target,changed);
            },pool:pool));
            if (rejectedJob == null || File.Exists(Path.Combine(rejectedJob,"ACCEPTED.json"))) throw new InvalidDataException("Stale pool worker was accepted.");
            if (mutateOriginal) File.WriteAllBytes(wave,original);
            pool.VerifyCurrent();
        }
        Console.WriteLine("Pool worker self-test: OK (1/3/8-source managed children, parent metadata/file-set checks, stale original and input-copy rejection; no codec execution).");
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: execute explicitly requested native 1/3/8-source proofs and reject altered raw evidence only in owned copies. */
    //-------------------------------------------------------------------------------------------------
    internal static void NativeProof(string library)
    {
        foreach (var shape in new[] { (Count:1,Shared:false,AllStreamed:false),(Count:1,Shared:false,AllStreamed:true),(Count:3,Shared:false,AllStreamed:false),(Count:8,Shared:true,AllStreamed:false),(Count:8,Shared:false,AllStreamed:false),(Count:8,Shared:true,AllStreamed:true) })
        {
            string source = Fixture(shape.Count,shape.Shared);
            // Reborn: exercise singleton streamed and maximum streamed artifact cardinality only by editing fresh owned fixtures.
            if (shape.AllStreamed) foreach (string path in Directory.EnumerateFiles(source,"*.xml")) File.WriteAllText(path,File.ReadAllText(path).Replace("IsStreamedOnPC=\"false\"","IsStreamedOnPC=\"true\""),new System.Text.UTF8Encoding(false));
            var pool = AuthoredAudioPool.Read(source);
            string job = AudioEncoderSupervisor.Run(library,"encode-pool",pool:pool); string work = Path.Combine(job,"worker");
            foreach (string leaf in new[] { "encoded/leaf0.runtime.bin","encoded/leaf0.input.wav","encoded/leaf0.snr","pool-core.json" })
            {
                string path = Path.Combine(work,leaf); byte[] bytes = File.ReadAllBytes(path),changed = (byte[])bytes.Clone(); changed[0] ^= 1;
                File.WriteAllBytes(path,changed); Reject(() => AudioPoolResultGate.Verify(work,pool,true)); File.WriteAllBytes(path,bytes);
            }
            RejectForgedResult(work,pool,true,"encoded/leaf0.runtime.bin");
            AudioPoolResultGate.Verify(work,pool,true); pool.VerifyCurrent();
        }
        Console.WriteLine("Pool native proof: OK (1/3/8 leaves, shared/distinct dependencies, parent raw-evidence checks and corruption rejection; no package/event/game-load proof).");
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: copy only bounded owned evidence into a fresh rejected job, then forge matching hashes to test the full parent acceptance gate. */
    //-------------------------------------------------------------------------------------------------
    private static void RejectForgedResult(string source,AuthoredAudioPool pool,bool encoded,string leaf)
    {
        string nonce = Guid.NewGuid().ToString("N"),job = Path.Combine(Path.GetTempPath(),"Reborn-SupervisedAudio-"+nonce),work = Path.Combine(job,"worker");
        Directory.CreateDirectory(work); pool.Install(Path.Combine(job,"inputs"));
        foreach (var item in AudioEncoderSupervisor.Inventory(source))
        {
            string path = Path.Combine(work,item.Path); Directory.CreateDirectory(Path.GetDirectoryName(path)!);
            using FileStream writer = new(path,FileMode.CreateNew,FileAccess.Write); writer.Write(AudioEncoderSupervisor.Read(Path.Combine(source,item.Path)));
        }
        string target = Path.Combine(work,leaf); byte[] changed = File.ReadAllBytes(target); changed[0] ^= 1; File.WriteAllBytes(target,changed);
        using (FileStream writer = new(Path.Combine(job,"result.json"),FileMode.CreateNew,FileAccess.Write)) writer.Write(System.Text.Json.JsonSerializer.SerializeToUtf8Bytes(new AudioEncoderSupervisor.Result(2,nonce,AudioEncoderSupervisor.Inventory(work))));
        Reject(() => AudioEncoderSupervisor.ValidateResult(job,nonce,pool:pool,poolEncoded:encoded));
        if (File.Exists(Path.Combine(job,"ACCEPTED.json"))) throw new InvalidDataException("Forged pool job received acceptance.");
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: generate bounded source fixtures only under a fresh owned test directory. */
    //-------------------------------------------------------------------------------------------------
    private static string Fixture(int count,bool shared)
    { string directory = Path.Combine(Path.GetTempPath(),"Reborn-PoolWorkerTest-"+Guid.NewGuid().ToString("N")); Directory.CreateDirectory(directory); AuthoredAudioPoolSmokeTest.Fixture(directory,count,shared); return directory; }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: managed worker proofs must not require or load a real library. */
    //-------------------------------------------------------------------------------------------------
    private static string UnusedLibrary() => Path.Combine(Path.GetTempPath(),"unused-native-library.dll");

    //-------------------------------------------------------------------------------------------------
    /** Reborn: negative evidence must fail validation rather than silently becoming accepted. */
    //-------------------------------------------------------------------------------------------------
    private static void Reject(Action action)
    { try { action(); } catch (Exception error) when (error is InvalidDataException or NotSupportedException) { return; } throw new InvalidDataException("Invalid pool evidence succeeded."); }
}
