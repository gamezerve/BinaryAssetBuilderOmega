using System;
using System.Buffers.Binary;
using System.IO;
using System.Linq;
using BinaryAssetBuilder.Core.Hashing;
using Chunk = Relo.Chunk;

namespace BinaryAssetBuilder.XmlCompiler;

// Reborn: partial root/container projection only; two root tokens cannot represent or publish a complete GameObject asset.
public static class Ep1GameObjectDestructionProjection
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: compose BuildTime/Behaviors and a generated destruction fragment into one address space while preserving the original native graph and reference indices. */
    //-------------------------------------------------------------------------------------------------
    public static Chunk Compose(Chunk native)
    {
        if (native == null || native.InstanceBuffer == null || native.InstanceBuffer.Length < 704 || native.InstanceBuffer.Length > 1048576)
            throw new InvalidDataException("GameObject destruction projection requires bounded native dispatch output.");
        byte[] b = native.InstanceBuffer;
        if (Word(b, 476) != 600 || Word(b, 600) != 1 || Word(b, 604) != 608 || Word(b, 608) != 612)
            throw new InvalidDataException("GameObject destruction projection requires its canonical single-behavior envelope.");
        var relos = Slots(native.RelocationBuffer); var imports = Slots(native.ImportsBuffer);
        if (!relos.Where(s => s < 612).OrderBy(s => s).SequenceEqual(new[] { 476u, 604u, 608u }) || imports.Any(s => s < 612))
            throw new InvalidDataException("Unsupported root pointer/import ownership in destruction projection.");
        var module = new Chunk { InstanceBuffer = b.AsSpan(612).ToArray() };
        foreach (uint slot in relos.Where(s => s >= 612))
        {
            if (slot > b.Length - 4 || Word(b, (int)slot) < 612 || Word(b, (int)slot) >= b.Length)
                throw new InvalidDataException("Destruction module pointer escapes its owned graph.");
            Put(module.InstanceBuffer, checked((int)slot - 612), Word(b, (int)slot) - 612);
        }
        module.RelocationBuffer = Pack(relos.Where(s => s >= 612).Select(s => s - 612));
        module.ImportsBuffer = Pack(imports.Select(s => s - 612));
        var fragment = Ep1DestructionTokenizer.Tokenize(module);
        int modulePrefix = checked(312 + (int)Word(module.InstanceBuffer, 64) * 72), prefix = checked(60 + modulePrefix);
        var result = new Chunk { InstanceBuffer = new byte[checked(prefix + b.Length)] };
        b.CopyTo(result.InstanceBuffer, prefix);
        Token(0, FastHash.GetHashCode("BuildTime"), 0x645864F3u, (uint)(prefix + 140));
        Token(12, FastHash.GetHashCode("Behaviors"), 0x24FCCAE2u, 36);
        Token(36, 0x6C827073u, 0x5D448FD8u, (uint)(prefix + 600));
        Token(48, 0xB151A0EBu, 0, 60);
        fragment.InstanceBuffer.AsSpan(0, modulePrefix).CopyTo(result.InstanceBuffer.AsSpan(60));
        // Reborn: metadata targets have distinct ownership: fragment token offsets shift by 60, module-native targets bind the preserved root-native graph.
        for (int at = 8; at < modulePrefix; at += 12)
        {
            uint target = Word(fragment.InstanceBuffer, at);
            if (target != 0) Put(result.InstanceBuffer, 60 + at, target < modulePrefix ? checked(target + 60u) : checked(target - (uint)modulePrefix + (uint)prefix + 612u));
        }
        foreach (uint slot in relos)
        {
            if (slot > b.Length - 4 || Word(b, (int)slot) >= b.Length) throw new InvalidDataException("Root projection relocation bounds exceeded.");
            Put(result.InstanceBuffer, checked(prefix + (int)slot), checked(Word(b, (int)slot) + (uint)prefix));
        }
        result.RelocationBuffer = Pack(relos.Select(s => checked(s + (uint)prefix)));
        result.ImportsBuffer = Pack(imports.Select(s => checked(s + (uint)prefix)));
        return result;

        //-------------------------------------------------------------------------------------------------
        /** Reborn: root projection metadata never enters native relocation/import streams. */
        //-------------------------------------------------------------------------------------------------
        void Token(int at, uint name, uint type, uint target) { Put(result.InstanceBuffer, at, name); Put(result.InstanceBuffer, at + 4, type); Put(result.InstanceBuffer, at + 8, target); }
    }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: auxiliary streams require an aligned, unique bounded slot list and one final sentinel. */
    //-------------------------------------------------------------------------------------------------
    private static uint[] Slots(byte[] b)
    {
        if (b == null || b.Length % 4 != 0) throw new InvalidDataException("Malformed projection auxiliary stream.");
        if (b.Length == 0) return Array.Empty<uint>();
        if (b.Length == 4 || Word(b, b.Length - 4) != uint.MaxValue) throw new InvalidDataException("Projection auxiliary sentinel differs.");
        var slots = Enumerable.Range(0, b.Length / 4 - 1).Select(i => Word(b, i * 4)).ToArray();
        if (slots.Any(s => s % 4 != 0 || s > 1048572) || slots.Distinct().Count() != slots.Length) throw new InvalidDataException("Projection slot alignment/uniqueness differs.");
        return slots;
    }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: preserve original slot order and absent streams when changing ownership coordinates. */
    //-------------------------------------------------------------------------------------------------
    private static byte[] Pack(System.Collections.Generic.IEnumerable<uint> values)
    {
        var slots = values.ToArray(); if (slots.Length == 0) return Array.Empty<byte>();
        var b = new byte[(slots.Length + 1) * 4]; for (int i = 0; i < slots.Length; i++) Put(b, i * 4, slots[i]); Put(b, slots.Length * 4, uint.MaxValue); return b;
    }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: native input and generated metadata word reads stay within their respective buffers. */
    //-------------------------------------------------------------------------------------------------
    private static uint Word(byte[] b, int at) { if (at < 0 || at > b.Length - 4) throw new InvalidDataException("Projection word bounds exceeded."); return BinaryPrimitives.ReadUInt32LittleEndian(b.AsSpan(at, 4)); }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: all generated root/container/module coordinates use little-endian Win32 words. */
    //-------------------------------------------------------------------------------------------------
    private static void Put(byte[] b, int at, uint value) => BinaryPrimitives.WriteUInt32LittleEndian(b.AsSpan(at, 4), value);
}
