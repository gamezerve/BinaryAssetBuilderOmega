namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: supervise real managed child processes for transport failures without loading a native codec in default tests.
internal static class AudioEncoderSupervisorSmokeTest
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: require failed/late/malformed/forged workers to leave no acceptance marker, including zero-exit workers with invalid results. */
    //-------------------------------------------------------------------------------------------------
    internal static void Run()
    {
        foreach (string mode in AudioEncoderSupervisor.TestModes)
        {
            string? job = null; bool rejected = false;
            try { AudioEncoderSupervisor.Run(Path.Combine(Path.GetTempPath(),"unused-native-library.dll"),mode,mode == "timeout" ? 1000 : 10000,path => job = path); }
            catch (InvalidDataException exception)
            {
                string expected = mode switch { "exit" => "exit=37", "crash-exit" => "exit=-1073741819", "timeout" => "timed out", "missing-result" => "missing result", "malformed-result" or "unknown-field" => "JSON is invalid", "log-overflow" => "log limit", "escape-path" => "path/size/hash", "duplicate-key" => "duplicate key", "oversized-result" => "exceeds bound", _ => "nonce/version/inventory" };
                if (!exception.Message.Contains(expected,StringComparison.Ordinal)) throw new InvalidDataException("Worker failed for an unintended reason: "+mode,exception);
                rejected = true;
            }
            if (!rejected || job == null || File.Exists(Path.Combine(job,"ACCEPTED.json"))) throw new InvalidDataException("Rejected worker received an acceptance marker.");
        }
        Console.WriteLine("Audio supervisor self-test: OK (12 real managed child-process exit/timeout/protocol/log/path failures, simulated crash exit, no acceptance; no native DLL execution)");
    }
}
