using BinaryAssetBuilder.Core;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: bind fixed diagnostic encoded records to frozen current core preparations before publication, without relabeling content hashes as production hashes.
internal static class CoreAudioPackageGate
{
    // Reborn: retain immutable encoded records and the core preparation whose current sources must still agree.
    internal sealed record Binding(InstanceDeclaration Instance,AudioFileCorePreparation Prepared,AudioFilePackageProbe.Entry Encoded);

    //-------------------------------------------------------------------------------------------------
    /** Reborn: reject stale identity/source/runtime metadata for both ordered local leaves before any package staging. */
    //-------------------------------------------------------------------------------------------------
    internal static void Verify(Binding[] bindings)
    {
        if (bindings.Length != 2 || bindings[0].Encoded.Name != "RebornAudioRAM" || bindings[1].Encoded.Name != "RebornAudioStream")
            throw new InvalidDataException("Core audio publication requires RAM then streamed bindings.");
        foreach (Binding binding in bindings)
        {
            if (binding.Instance.Handle.InstanceName != binding.Encoded.Name || binding.Instance.Handle.InstanceId != binding.Encoded.Id
                || Path.GetFileName(binding.Instance.Document.SourcePath) != binding.Encoded.Source)
                throw new InvalidDataException("Core audio identity/source does not match its encoded record.");
            AssetBuffer native = binding.Encoded.CopyNative();
            var parsed = AudioFileRuntimeProbe.Parse(native.InstanceData,native.InstanceData.Length);
            byte[] header = parsed.HeaderSize == 0 ? Array.Empty<byte>() : native.InstanceData.AsSpan(checked((int)parsed.HeaderPointer),checked((int)parsed.HeaderSize)).ToArray();
            AssetBuffer expected = binding.Prepared.SerializeCurrent(binding.Instance,header);
            if (!native.InstanceData.SequenceEqual(expected.InstanceData) || !native.RelocationData.SequenceEqual(expected.RelocationData)
                || !native.ImportsData.SequenceEqual(expected.ImportsData)) throw new InvalidDataException("Core prepared runtime differs from encoded record.");
        }
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: inspect current core/files immediately before the existing no-overwrite diagnostic publisher; not a concurrent filesystem transaction. */
    //-------------------------------------------------------------------------------------------------
    internal static void Publish(string output,Binding[] bindings,AudioFileLocalEventProbe.Entry? localEvent = null)
    {
        Verify(bindings); AudioFilePackageProbe.Publish(output,bindings.Select(binding => binding.Encoded).ToArray(),localEvent);
    }
}
