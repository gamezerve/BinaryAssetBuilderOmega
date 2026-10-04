using System.Text.Json;
using BinaryAssetBuilder.Core;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: reconstruct pool evidence in the parent without loading the codec or trusting child identities/order.
internal static class AudioPoolResultGate
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: require an exact file set and source-derived core/runtime/PCM metadata before accepting a variable raw-audio worker result. */
    //-------------------------------------------------------------------------------------------------
    internal static void Verify(string directory,AuthoredAudioPool pool,bool encoded)
    {
        pool.VerifyCopies(directory);
        string schema = Path.Combine(Path.GetDirectoryName(ReferencePipelineSmokeTest.FindFixture())!,"AudioFileIdentityPipeline.xsd");
        var rows = pool.Rows; List<AuthoredAudioPool.CoreRow> metadata = new();
        HashSet<string> expectedFiles = new(pool.FileNames,StringComparer.Ordinal) { "pool-core.json" };
        for (int index = 0; index < rows.Length; index++)
        {
            var row = rows[index]; var core = AudioFileIdentitySmokeTest.Build(directory,schema,AudioFileIdentitySmokeTest.Processing,row.Source);
            var prepared = AudioFileCorePreparation.Prepare(core); prepared.VerifyCurrent(core);
            if (core.Handle.InstanceName != row.Name || core.Handle.InstanceId != row.Id || prepared.Settings.FileName != row.Wave || prepared.Settings.Streamed != row.Streamed)
                throw new InvalidDataException("Pool parent core binding differs.");
            metadata.Add(new(row.Source,row.Name,row.Id,core.Handle.InstanceHash,row.Streamed));
            if (!encoded) continue;
            string prefix = "encoded/leaf"+index;
            foreach (string suffix in new[] { ".input.wav",".runtime.bin",".runtime.relo",".snr" }) expectedFiles.Add(prefix+suffix);
            if (row.Streamed) expectedFiles.Add(prefix+".sns");
            if (!AudioEncoderSupervisor.Read(Path.Combine(directory,prefix+".input.wav"),24044).SequenceEqual(prepared.CopyWave())) throw new InvalidDataException("Pool frozen PCM differs.");
            var native = new AssetBuffer { InstanceData = AudioEncoderSupervisor.Read(Path.Combine(directory,prefix+".runtime.bin"),2048),RelocationData = AudioEncoderSupervisor.Read(Path.Combine(directory,prefix+".runtime.relo"),12),ImportsData = Array.Empty<byte>() };
            byte[] header = row.Streamed ? AudioEncoderSupervisor.Read(Path.Combine(directory,prefix+".snr"),8) : Array.Empty<byte>();
            // Reborn: the existing checked leaf envelope is reused only as a play-location validator, not as a fixed-slot package binding.
            _ = new AudioFilePackageProbe.Entry(row.Name,row.Streamed ? "streamed.xml" : "ram.xml",native,AudioEncoderSupervisor.Read(Path.Combine(directory,prefix+(row.Streamed ? ".sns" : ".snr"))));
            AssetBuffer reconstructed = prepared.SerializeCurrent(core,header);
            if (!native.InstanceData.SequenceEqual(reconstructed.InstanceData) || !native.RelocationData.SequenceEqual(reconstructed.RelocationData)) throw new InvalidDataException("Pool runtime differs from parent core preparation.");
        }
        if (!AudioEncoderSupervisor.Read(Path.Combine(directory,"pool-core.json"),8192).SequenceEqual(JsonSerializer.SerializeToUtf8Bytes(metadata.ToArray()))) throw new InvalidDataException("Pool core evidence differs.");
        var actual = AudioEncoderSupervisor.Inventory(directory);
        if (actual.Length != expectedFiles.Count || actual.Any(item => !expectedFiles.Contains(item.Path))) throw new InvalidDataException("Pool output file set differs.");
        pool.VerifyCopies(directory); pool.VerifyCurrent();
    }
}
