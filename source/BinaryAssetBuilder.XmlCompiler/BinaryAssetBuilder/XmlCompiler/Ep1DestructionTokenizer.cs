using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BinaryAssetBuilder.Core.Hashing;
using Chunk = Relo.Chunk;

namespace BinaryAssetBuilder.XmlCompiler;

// Reborn: explicit module-fragment token writer; no registry/publication support is granted to general GameObjects.
public static class Ep1DestructionTokenizer
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: generate reviewed inheritance/inline/list tokens from canonical native data, rebasing only validated native pointers and auxiliary slots. */
    //-------------------------------------------------------------------------------------------------
    public static Chunk Tokenize(Chunk native)
    {
        if (native == null || native.InstanceBuffer == null || native.InstanceBuffer.Length < 92 || native.InstanceBuffer.Length > 1048576)
            throw new InvalidDataException("Destruction tokenizer requires a bounded native module.");
        byte[] b = native.InstanceBuffer;
        uint count = Word(b, 64);
        // Reborn: item cardinality is bounded by the actual native buffer, not the former experiment's 256-row quota.
        if (Word(b, 0) != 0x0D96A2F4u || count < 1 || count > (b.Length-72)/20 || Word(b, 68) != 72 ||
            Word(b, 8) != 0 || Word(b, 12) != 0 || Word(b, 16) != 0 || Word(b, 20) != 0 || 72 + count * 20 > b.Length)
            throw new InvalidDataException("Destruction tokenizer requires canonical items and reviewed empty/absent DieMux storage.");
        var relos = new List<uint> { 68 }; var imports = new List<uint>();
        for (int i = 0; i < count; i++)
        {
            int item = 72 + i * 20; uint length = Word(b, item), pointer = Word(b, item + 4);
            if (pointer == 0)
            { if (length != 0) throw new InvalidDataException("Destruction null bone string has nonzero length."); }
            else
            {
                if (pointer < 72 + count * 20 || pointer >= b.Length || length > b.Length - pointer - 1 || b[(int)(pointer + length)] != 0)
                    throw new InvalidDataException("Destruction bone string bounds/terminator differ.");
                relos.Add((uint)(item + 4));
            }
            if (Word(b, item + 8) != 0) imports.Add((uint)(item + 8));
        }
        CheckSlots(native.RelocationBuffer, relos); CheckSlots(native.ImportsBuffer, imports);
        int prefix = checked(312 + (int)count * 72);
        var result = new Chunk { InstanceBuffer = new byte[checked(prefix + b.Length)] };
        b.CopyTo(result.InstanceBuffer, prefix);
        var output = result.InstanceBuffer;
        Token(0, 2, 0x0D96A2F4u, 12);
        Token(12, 1, FastHash.GetHashCode("DieModuleData"), 84);
        Field(24, "AssociatedModule", 0xBDF07965u, 52);
        Field(36, "MaxDebrisCloudsPerSecond", 0x645864F3u, 56);
        Field(48, "FillerMesh", 0x03A28C40u, 60);
        Token(60, FastHash.GetHashCode("Item"), 0xFFDDEC52u, 300);
        Token(84, 1, FastHash.GetHashCode("BehaviorModuleData"), 120);
        Token(96, FastHash.GetHashCode("DieMuxData"), 0x0084001Bu, 180);
        Token(120, 1, FastHash.GetHashCode("ModuleData"), 144);
        Field(144, "TypeId", 0x23E650B8u, 0); Field(156, "id", 0xBDF07965u, 4);
        Token(180, FastHash.GetHashCode("VeterancyLevels"), 0x6D66AE18u, 288);
        Token(192, FastHash.GetHashCode("ExemptStatus"), 0xA7BD811Eu, 0);
        Token(204, FastHash.GetHashCode("RequiredStatus"), 0xA7BD811Eu, 0);
        Field(216, "DamageAmountRequired", 0x645864F3u, 24);
        Field(228, "MinKillerAngle", 0x4E16EA4Du, 28); Field(240, "MaxKillerAngle", 0x4E16EA4Du, 32);
        Field(252, "DeathTypes", 0x034DF1A1u, 36); Field(264, "DeathTypesForbidden", 0x034DF1A1u, 44);
        Token(288, 0x6C827073u, 0x5D448FD8u, (uint)(prefix + 8));
        Token(300, 0x6C827073u, 0x5D448FD8u, (uint)(prefix + 64));
        for (int i = 0; i < count; i++)
        {
            int table = 312 + (int)count * 12 + i * 60, item = 72 + i * 20;
            Token(312 + i * 12, 0xB151A0EBu, 0, (uint)table);
            Field(table, "BonePrefix", 0x1A2933AAu, item); Field(table + 12, "FX", 0x0D1C8E83u, item + 8);
            Field(table + 24, "CreationObject", 0x148ED75Du, item + 12); Field(table + 36, "FlingVelocity", 0x645864F3u, item + 16);
        }
        foreach (uint slot in relos) Put(output, prefix + (int)slot, checked(Word(b, (int)slot) + (uint)prefix));
        result.RelocationBuffer = ShiftSlots(relos, prefix); result.ImportsBuffer = ShiftSlots(imports, prefix);
        return result;

        //-------------------------------------------------------------------------------------------------
        /** Reborn: native targets are shifted while token-table targets remain prefix-relative and zero terminators remain zeroed. */
        //-------------------------------------------------------------------------------------------------
        void Field(int at, string name, uint type, int target) => Token(at, FastHash.GetHashCode(name), type, (uint)(prefix + target));
        //-------------------------------------------------------------------------------------------------
        /** Reborn: token words are metadata, never native RELO entries. */
        //-------------------------------------------------------------------------------------------------
        void Token(int at, uint name, uint type, uint target) { Put(output, at, name); Put(output, at + 4, type); Put(output, at + 8, target); }
    }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: exact slot membership and a single final sentinel prevent rebasing unreviewed pointer/import locations. */
    //-------------------------------------------------------------------------------------------------
    private static void CheckSlots(byte[] bytes, List<uint> expected)
    {
        if (bytes == null || bytes.Length % 4 != 0 || (expected.Count == 0 ? bytes.Length != 0 : bytes.Length != (expected.Count + 1) * 4))
            throw new InvalidDataException("Destruction auxiliary slot dimensions differ.");
        if (expected.Count == 0) return;
        var slots = Enumerable.Range(0, expected.Count).Select(i => Word(bytes, i * 4)).ToArray();
        if (Word(bytes, bytes.Length - 4) != uint.MaxValue || !slots.OrderBy(x => x).SequenceEqual(expected.OrderBy(x => x)))
            throw new InvalidDataException("Destruction auxiliary slot membership differs.");
    }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: each validated native slot shifts once; absent streams stay empty. */
    //-------------------------------------------------------------------------------------------------
    private static byte[] ShiftSlots(List<uint> slots, int prefix)
    {
        if (slots.Count == 0) return Array.Empty<byte>();
        var result = new byte[(slots.Count + 1) * 4];
        for (int i = 0; i < slots.Count; i++) Put(result, i * 4, checked(slots[i] + (uint)prefix));
        Put(result, slots.Count * 4, uint.MaxValue); return result;
    }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: bounded word access protects the explicit fragment writer's input contract. */
    //-------------------------------------------------------------------------------------------------
    private static uint Word(byte[] b, int at) { if (at < 0 || at > b.Length - 4) throw new InvalidDataException("Destruction tokenizer bounds exceeded."); return BinaryPrimitives.ReadUInt32LittleEndian(b.AsSpan(at, 4)); }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: generated words retain little-endian Win32 storage. */
    //-------------------------------------------------------------------------------------------------
    private static void Put(byte[] b, int at, uint value) => BinaryPrimitives.WriteUInt32LittleEndian(b.AsSpan(at, 4), value);
}
