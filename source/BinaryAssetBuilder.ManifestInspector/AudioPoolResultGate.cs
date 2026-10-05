using System.Text.Json;
using BinaryAssetBuilder.Core;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: reconstruct pool evidence in the parent without loading the codec or trusting child identities/order.
internal static class AudioPoolResultGate
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: require an exact file set and source-derived core/runtime/PCM metadata before accepting a variable raw-audio worker result. */
    //-------------------------------------------------------------------------------------------------
    internal static AudioFilePackageProbe.Entry[] Verify(string directory,AuthoredAudioPool pool,bool encoded,bool packaged = false)
    {
        // Reborn: package evidence cannot be requested for metadata-only output.
        if (packaged && !encoded) throw new InvalidDataException("Pool package requires encoded leaves.");
        pool.VerifyCopies(directory);
        string schema = Path.Combine(Path.GetDirectoryName(ReferencePipelineSmokeTest.FindFixture())!,"AudioFileIdentityPipeline.xsd");
        var rows = pool.Rows; List<AuthoredAudioPool.CoreRow> metadata = new();
        List<AudioFilePackageProbe.Entry> entries = new();
        HashSet<string> expectedFiles = new(pool.FileNames,StringComparer.Ordinal) { "pool-core.json" };
        for (int index = 0; index < rows.Length; index++)
        {
            var row = rows[index]; var core = AudioFileIdentitySmokeTest.Build(directory,schema,AudioFileIdentitySmokeTest.Processing,row.Source);
            var prepared = AudioFileCorePreparation.Prepare(core,pool.DurationCandidate); prepared.VerifyCurrent(core);
            if (core.Handle.InstanceName != row.Name || core.Handle.InstanceId != row.Id || prepared.Settings.FileName != row.Wave || prepared.Settings.Streamed != row.Streamed)
                throw new InvalidDataException("Pool parent core binding differs.");
            metadata.Add(new(row.Source,row.Name,row.Id,core.Handle.InstanceHash,row.Streamed));
            if (!encoded) continue;
            string prefix = "encoded/leaf"+index;
            foreach (string suffix in new[] { ".input.wav",".runtime.bin",".runtime.relo",".snr" }) expectedFiles.Add(prefix+suffix);
            if (row.Streamed) expectedFiles.Add(prefix+".sns");
            if (!AudioEncoderSupervisor.Read(Path.Combine(directory,prefix+".input.wav"),pool.DurationCandidate ? BinaryAssetBuilder.XmlCompiler.Ra3Ep1AudioFileInputProfile.MaximumCandidateWaveBytes : 24044).SequenceEqual(prepared.CopyWave())) throw new InvalidDataException("Pool frozen PCM differs.");
            var native = new AssetBuffer { InstanceData = AudioEncoderSupervisor.Read(Path.Combine(directory,prefix+".runtime.bin"),2048),RelocationData = AudioEncoderSupervisor.Read(Path.Combine(directory,prefix+".runtime.relo"),12),ImportsData = Array.Empty<byte>() };
            byte[] header = row.Streamed ? AudioEncoderSupervisor.Read(Path.Combine(directory,prefix+".snr"),8) : Array.Empty<byte>();
            // Reborn: bind actual variable source names to explicit play location, never infer it from a source slot label.
            entries.Add(new AudioFilePackageProbe.Entry(row.Name,row.Source,native,AudioEncoderSupervisor.Read(Path.Combine(directory,prefix+(row.Streamed ? ".sns" : ".snr"))),row.Streamed,prepared.Settings.Samples));
            AssetBuffer reconstructed = prepared.SerializeCurrent(core,header);
            if (!native.InstanceData.SequenceEqual(reconstructed.InstanceData) || !native.RelocationData.SequenceEqual(reconstructed.RelocationData)) throw new InvalidDataException("Pool runtime differs from parent core preparation.");
        }
        if (!AudioEncoderSupervisor.Read(Path.Combine(directory,"pool-core.json"),8192).SequenceEqual(JsonSerializer.SerializeToUtf8Bytes(metadata.ToArray()))) throw new InvalidDataException("Pool core evidence differs.");
        if (packaged)
        {
            // Reborn: regenerate all linked package bytes from parent-checked leaves, then verify both manifest readers and exact artifact membership.
            var localEvent = BuildEvent(directory,pool,entries.ToArray());
            AudioFilePackageProbe.Verify(Path.Combine(directory,"package"),entries.ToArray(),localEvent,variable:true);
            foreach (string name in AudioFilePackageProbe.Serialize(entries.ToArray(),localEvent,variable:true).Keys) expectedFiles.Add("package/"+name.Replace('\\','/'));
        }
        var actual = AudioEncoderSupervisor.Inventory(directory,packaged ? 80 : 64);
        if (actual.Length != expectedFiles.Count || actual.Any(item => !expectedFiles.Contains(item.Path))) throw new InvalidDataException("Pool output file set differs.");
        pool.VerifyCopies(directory); pool.VerifyCurrent();
        return entries.ToArray();
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: reconstruct raw/core bindings immediately before no-overwrite staged variable publication, then recheck full packaged evidence. */
    //-------------------------------------------------------------------------------------------------
    internal static void Publish(string directory,AuthoredAudioPool pool)
    {
        var entries = Verify(directory,pool,true);
        AudioFilePackageProbe.Publish(Path.Combine(directory,"package"),entries,BuildEvent(directory,pool,entries),variable:true);
        Verify(directory,pool,true,true);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: derive mixed event bytes independently from frozen source and current leaf fingerprints, never worker-provided event metadata. */
    //-------------------------------------------------------------------------------------------------
    internal static AudioFileLocalEventProbe.Entry? BuildEvent(string directory,AuthoredAudioPool pool,AudioFilePackageProbe.Entry[] entries)
    {
        if (pool.EventName == null) return null;
        pool.VerifyCopies(directory);
        var compiled = AudioFileLocalEventProbe.Build(directory,entries,authoredName:pool.EventName,variable:true);
        if (compiled.Settings != pool.EventSettings) throw new InvalidDataException("Mixed pool event source/settings differ.");
        var replay = AudioFileLocalEventProbe.Build(directory,entries,authoredName:pool.EventName,variable:true);
        var native = compiled.CopyNative(); var other = replay.CopyNative();
        if (compiled.Hash != replay.Hash || !native.InstanceData.SequenceEqual(other.InstanceData) || !native.RelocationData.SequenceEqual(other.RelocationData) || !native.ImportsData.SequenceEqual(other.ImportsData)) throw new InvalidDataException("Mixed pool event recompilation differs.");
        pool.VerifyCopies(directory); return compiled;
    }
}
