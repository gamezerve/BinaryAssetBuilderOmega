
using System.Buffers.Binary;

namespace BinaryAssetBuilder.ManifestInspector;

internal static class OnDemandTextureCData
{
    internal static byte[] Build(
        uint typeId,
        uint instanceId,
        uint typeHash,
        uint instanceHash,
        byte[] dds)
    {
        if (typeId != 0x9312B9AC || typeHash != 0xF8488EDE)
            throw new InvalidDataException("Invalid OnDemandTexture asset type.");

        if (dds.Length < 128 ||
            BinaryPrimitives.ReadUInt32LittleEndian(dds) != 0x20534444 ||
            BinaryPrimitives.ReadUInt32LittleEndian(dds.AsSpan(4)) != 124)
            throw new InvalidDataException("Invalid DDS payload.");

        byte[] data = new byte[checked(dds.Length + 48)];
        Span<byte> header = data.AsSpan(0, 40);

        BinaryPrimitives.WriteUInt32LittleEndian(header.Slice(0), typeId);
        BinaryPrimitives.WriteUInt32LittleEndian(header.Slice(4), instanceId);
        BinaryPrimitives.WriteUInt32LittleEndian(header.Slice(8), typeHash);
        BinaryPrimitives.WriteUInt32LittleEndian(header.Slice(12), instanceHash);
        BinaryPrimitives.WriteUInt32LittleEndian(header.Slice(16), checked((uint)dds.Length + 8));
        BinaryPrimitives.WriteUInt32LittleEndian(header.Slice(20), 8);
        BinaryPrimitives.WriteUInt32LittleEndian(header.Slice(32), 8);
        BinaryPrimitives.WriteUInt32LittleEndian(header.Slice(36), checked((uint)dds.Length));

        dds.CopyTo(data.AsSpan(40));
        BinaryPrimitives.WriteUInt32LittleEndian(data.AsSpan(data.Length - 4), uint.MaxValue);

        return data;
    }

    internal static string FileName(
        uint typeId,
        uint instanceId,
        uint typeHash,
        uint instanceHash)
    {
        return $"{typeId:x8}.{typeHash:x8}.{instanceId:x8}.{instanceHash:x8}.cdata";
    }

    internal static void Verify(
        byte[] data,
        byte[] dds,
        uint typeId,
        uint instanceId,
        uint typeHash,
        uint instanceHash)
    {
        if (data.Length != checked(dds.Length + 48) ||
            BinaryPrimitives.ReadUInt32LittleEndian(data) != typeId ||
            BinaryPrimitives.ReadUInt32LittleEndian(data.AsSpan(4)) != instanceId ||
            BinaryPrimitives.ReadUInt32LittleEndian(data.AsSpan(8)) != typeHash ||
            BinaryPrimitives.ReadUInt32LittleEndian(data.AsSpan(12)) != instanceHash ||
            BinaryPrimitives.ReadUInt32LittleEndian(data.AsSpan(16)) != dds.Length + 8 ||
            BinaryPrimitives.ReadUInt32LittleEndian(data.AsSpan(20)) != 8 ||
            BinaryPrimitives.ReadUInt32LittleEndian(data.AsSpan(24)) != 0 ||
            BinaryPrimitives.ReadUInt32LittleEndian(data.AsSpan(28)) != 0 ||
            BinaryPrimitives.ReadUInt32LittleEndian(data.AsSpan(32)) != 8 ||
            BinaryPrimitives.ReadUInt32LittleEndian(data.AsSpan(36)) != dds.Length ||
            !data.AsSpan(40, dds.Length).SequenceEqual(dds) ||
            BinaryPrimitives.ReadUInt32LittleEndian(data.AsSpan(data.Length - 8)) != 0 ||
            BinaryPrimitives.ReadUInt32LittleEndian(data.AsSpan(data.Length - 4)) != uint.MaxValue)
            throw new InvalidDataException("OnDemandTexture CDATA verification failed.");
    }
}
