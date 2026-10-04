namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: exercise the actual shared cleanup control flow with managed callbacks and fake handles; never load a native library.
internal static class AudioEncoderCleanupSmokeTest
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: a release exception cannot skip other resources or leave a handle eligible for a second release attempt. */
    //-------------------------------------------------------------------------------------------------
    internal static void Run()
    {
        foreach (int failAt in new[] { 0,1,2,3 })
        {
            IntPtr target = new(1),info = new(2),source = new(3); List<int> calls = new(); bool rejected = false;
            try { AudioEncoderPoc.Cleanup(ref target,ref info,ref source,Close,Close,Close); }
            catch (InvalidOperationException exception) when (exception.Message == "Reborn managed release failure") { rejected = true; }
            if (rejected != (failAt != 0) || target != IntPtr.Zero || info != IntPtr.Zero || source != IntPtr.Zero || !calls.SequenceEqual(new[] { 1,2,3 }))
                throw new InvalidDataException("Independent cleanup did not attempt each resource exactly once.");
            AudioEncoderPoc.Cleanup(ref target,ref info,ref source,Close,Close,Close);
            if (calls.Count != 3) throw new InvalidDataException("Detached cleanup handles were released twice.");

            //-------------------------------------------------------------------------------------------------
            /** Reborn: throw only from the selected managed test callback; preserve arbitrary raw return status for the others. */
            //-------------------------------------------------------------------------------------------------
            int Close(IntPtr handle)
            { int value = handle.ToInt32(); calls.Add(value); if (value == failAt) throw new InvalidOperationException("Reborn managed release failure"); return value-2; }
        }
        bool badName = false;
        try { _ = new AudioEncoderFaultAudit("unknown"); } catch (ArgumentException) { badName = true; }
        if (!badName || AudioEncoderFaultAudit.Scenarios.Length != 8) throw new InvalidDataException("Fault scenario admission differs.");
        // Reborn: exercise known native-crash prevention without invoking the DLL, using only a fresh owned fixture directory.
        string directory = Path.Combine(Path.GetTempPath(),"Reborn-NativeOutputGuard-"+Guid.NewGuid().ToString("N")); Directory.CreateDirectory(directory);
        string prefix = Path.Combine(directory,"encoded"); AudioEncoderPoc.ValidateNativeOutputPrefix(prefix);
        Reject(() => AudioEncoderPoc.ValidateNativeOutputPrefix(Path.Combine(directory,"missing","encoded")));
        Reject(() => AudioEncoderPoc.ValidateNativeOutputPrefix("relative"));
        Reject(() => AudioEncoderPoc.ValidateNativeOutputPrefix(Path.Combine(directory,"bad.prefix")));
        using (FileStream stream = new(prefix+".snr",FileMode.CreateNew,FileAccess.Write)) stream.WriteByte(0x7F);
        Reject(() => AudioEncoderPoc.ValidateNativeOutputPrefix(prefix));
        if (File.ReadAllBytes(prefix+".snr").Single() != 0x7F) throw new InvalidDataException("Output guard changed existing artifact.");
        Console.WriteLine("Audio encoder cleanup self-test: OK (shared cleanup success and each release failure, independent attempts, detached handles/no double release; managed callbacks only)");
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: unsafe prefix rejection must occur without entering native code or touching an existing artifact. */
    //-------------------------------------------------------------------------------------------------
    private static void Reject(Action action)
    { try { action(); } catch (InvalidDataException) { return; } throw new InvalidDataException("Unsafe native prefix unexpectedly passed."); }
}
