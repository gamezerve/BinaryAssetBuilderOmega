namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: test static identity evidence against the read-only pinned RA3 DLL and reject changed snapshots without any native/reference execution.
internal static class PathMusicReferenceIdentitySmokeTest
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: verify event metadata/dispatch provenance and explicit RA3-versus-EP1 mismatch, repeat determinism and tampered snapshot refusal. */
    //-------------------------------------------------------------------------------------------------
    internal static void Run()
    {
        string path = Path.GetFullPath(Path.Combine(Path.GetDirectoryName(ReferencePipelineSmokeTest.FindFixture())!,"../../Working RA3 Compiler for Reference/tools/BinaryAssetBuilder.AudioCompiler.dll"));
        var report = PathMusicReferenceIdentity.Inspect(path); var repeat = PathMusicReferenceIdentity.Inspect(path);
        if (report.TypeId != 0x9A651D89u || report.ReferenceProcessingHash != 0x76D0CEE6u || report.ReferenceTypeHash != 0x76D0CEEFu
            || report.ReferenceAllTypesHash != 0x54EEE764u || report.ReferenceWin32Version != 0x20B00004u || report.HasCustomData
            || report.ReferenceTypeHashMatchesEp1 || report.Ep1ProcessingHashRecovered || report.ReferenceExecuted || report.ProductionBuildReady
            || !report.ReferenceProcessingHashRecovered || report.Methods.Length != 5 || !report.Methods.SequenceEqual(repeat.Methods))
            throw new InvalidDataException("Reference music identity evidence/readiness differs.");
        // Reborn: mutate only owned in-memory copies; the reference artifact remains untouched.
        byte[] bytes = SdkEnvironmentPreflight.Read(path,4*1048576);
        foreach (int offset in new[] { 0,bytes.Length/2,bytes.Length-1 })
        { byte[] bad = (byte[])bytes.Clone(); bad[offset] ^= 1; Refuses(() => PathMusicReferenceIdentity.Verify(bad)); }
        Refuses(() => PathMusicReferenceIdentity.Verify(bytes[..^1])); Refuses(() => PathMusicReferenceIdentity.Verify(Array.Empty<byte>()));
        report.Methods[0] = report.Methods[0] with { Name = "forged" };
        if (PathMusicReferenceIdentity.Inspect(path).Methods[0].Name != "GetPathMusicExtendedTypeInformation") throw new InvalidDataException("Reference report aliases trusted metadata.");
        Console.WriteLine("PathMusic reference identity self-test: OK (pinned static metadata/field/branch/dispatcher proof, RA3 ProcessingHash=76D0CEE6/TypeHash=76D0CEEF differs from EP1 599CDAF2, repeat/tampered snapshot refusal; no DLL execution/EP1 hash recovery)");
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: changed or incomplete reference bytes cannot inherit reviewed identity authority. */
    //-------------------------------------------------------------------------------------------------
    private static void Refuses(Action action)
    { try { action(); } catch (InvalidDataException) { return; } throw new InvalidDataException("Changed reference identity accepted."); }
}
