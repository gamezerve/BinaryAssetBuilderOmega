using System;
using System.Buffers.Binary;
using System.IO;
using BinaryAssetBuilder.Core.Hashing;
using Relo;

namespace BinaryAssetBuilder.XmlCompiler;

// Reborn: experimental Win32 armor tokenization remains separate from the production registry/gate.
public static class Ep1ArmorTokenizer
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: prepend the official RA3/observed EP1 token tree to a canonical ArmorTemplate native chunk. */
    //-------------------------------------------------------------------------------------------------
    public static Chunk Tokenize(Chunk native)
    {
        if (native == null || native.InstanceBuffer.Length < 32 || native.ImportsBuffer.Length != 0)
            throw new InvalidDataException("Armor tokenization requires a Win32 native chunk without imports.");
        uint count = BinaryPrimitives.ReadUInt32LittleEndian(native.InstanceBuffer.AsSpan(24));
        int itemCount = checked((int)count);
        int nativeSize = checked(32 + itemCount * 8);
        uint pointer = BinaryPrimitives.ReadUInt32LittleEndian(native.InstanceBuffer.AsSpan(28));
        if (native.InstanceBuffer.Length != nativeSize
            || BinaryPrimitives.ReadUInt32LittleEndian(native.InstanceBuffer) != 0
            || (count == 0 ? pointer != 0 || native.RelocationBuffer.Length != 0
                : pointer != 32 || native.RelocationBuffer.Length != 8
                    || BinaryPrimitives.ReadUInt32LittleEndian(native.RelocationBuffer) != 28
                    || BinaryPrimitives.ReadUInt32LittleEndian(native.RelocationBuffer.AsSpan(4)) != uint.MaxValue))
            throw new InvalidDataException("Armor tokenization requires the canonical 32-byte root and contiguous 8-byte list stride.");

        int prefix = checked(96 + itemCount * 48);
        Chunk result = new() { InstanceBuffer = new byte[checked(prefix + nativeSize)] };
        native.InstanceBuffer.CopyTo(result.InstanceBuffer, prefix);
        string[] fields = { "Default", "DamageScalar", "SideDamageScalar", "RearDamageScalar", "FlankedPenalty" };
        for (int index = 0; index < fields.Length; index++)
            WriteToken(result.InstanceBuffer, index * 12, FastHash.GetHashCode(fields[index]), 0x645864F3u, (uint)(prefix + 4 + index * 4));
        WriteToken(result.InstanceBuffer, 60, FastHash.GetHashCode("Armor"), 0x24D5712Fu, 84);
        // Reborn: the zeroed root terminator is followed by a count record and one nested token table per item.
        WriteToken(result.InstanceBuffer, 84, 0x6C827073u, 0x5D448FD8u, (uint)(prefix + 24));
        int nestedStart = checked(96 + itemCount * 12);
        for (int index = 0; index < itemCount; index++)
        {
            int nested = checked(nestedStart + index * 36);
            int value = checked(prefix + 32 + index * 8);
            WriteToken(result.InstanceBuffer, 96 + index * 12, 0xB151A0EBu, 0, (uint)nested);
            WriteToken(result.InstanceBuffer, nested, FastHash.GetHashCode("Damage"), 0x46486FF1u, (uint)value);
            WriteToken(result.InstanceBuffer, nested + 12, FastHash.GetHashCode("Percent"), 0x645864F3u, (uint)(value + 4));
        }
        // Reborn: token member offsets are not relocation entries; only the actual native list pointer relocates.
        if (count != 0)
        {
            BinaryPrimitives.WriteUInt32LittleEndian(result.InstanceBuffer.AsSpan(prefix + 28), (uint)(prefix + 32));
            result.RelocationBuffer = new byte[8];
            BinaryPrimitives.WriteUInt32LittleEndian(result.RelocationBuffer, (uint)(prefix + 28));
            BinaryPrimitives.WriteUInt32LittleEndian(result.RelocationBuffer.AsSpan(4), uint.MaxValue);
        }
        return result;
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: each token is a field hash, a type/tag hash and an asset-relative value/table offset. */
    //-------------------------------------------------------------------------------------------------
    private static void WriteToken(byte[] bytes, int offset, uint name, uint type, uint value)
    {
        BinaryPrimitives.WriteUInt32LittleEndian(bytes.AsSpan(offset), name);
        BinaryPrimitives.WriteUInt32LittleEndian(bytes.AsSpan(offset + 4), type);
        BinaryPrimitives.WriteUInt32LittleEndian(bytes.AsSpan(offset + 8), value);
    }
}
