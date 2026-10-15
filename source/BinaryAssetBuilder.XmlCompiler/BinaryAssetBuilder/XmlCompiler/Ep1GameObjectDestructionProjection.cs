using System;
using System.Buffers.Binary;
using System.IO;
using System.Linq;
using BinaryAssetBuilder.Core.Hashing;
using Chunk = Relo.Chunk;

namespace BinaryAssetBuilder.XmlCompiler;

// Reborn: root-value/string/import/container projection only; dependency closure and other indirect fields still prevent complete GameObject publication.
public static class Ep1GameObjectDestructionProjection
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: compose reviewed root values/Behaviors and a generated destruction fragment while preserving the original native graph and reference indices. */
    //-------------------------------------------------------------------------------------------------
    public static Chunk Compose(Chunk native)
    {
        if (native == null || native.InstanceBuffer == null || native.InstanceBuffer.Length < 704 || native.InstanceBuffer.Length > 1048576)
            throw new InvalidDataException("GameObject destruction projection requires bounded native dispatch output.");
        byte[] b = native.InstanceBuffer;
        var relos = Slots(native.RelocationBuffer); var imports = Slots(native.ImportsBuffer);
        // Reborn: explicit root-string allocations may move the behavior list and module; decode and validate ownership before extracting the tail.
        var layout = Ep1GameObjectStringTokens.Inspect(b, relos, imports); int moduleStart = layout.Module;
        var module = new Chunk { InstanceBuffer = b.AsSpan(moduleStart).ToArray() };
        foreach (uint slot in relos.Where(s => s >= moduleStart))
        {
            if (slot > b.Length - 4 || Word(b, (int)slot) < moduleStart || Word(b, (int)slot) >= b.Length)
                throw new InvalidDataException("Destruction module pointer escapes its owned graph.");
            Put(module.InstanceBuffer, checked((int)slot - moduleStart), Word(b, (int)slot) - (uint)moduleStart);
        }
        module.RelocationBuffer = Pack(relos.Where(s => s >= moduleStart).Select(s => s - (uint)moduleStart));
        // Reborn: split slot ownership without subtracting the module start from root slots or renumbering shared dependency indices.
        module.ImportsBuffer = Pack(imports.Where(s => s >= moduleStart).Select(s => s - (uint)moduleStart));
        var fragment = Ep1DestructionTokenizer.Tokenize(module);
        // Reborn: root token count is descriptor-driven; nested fragment coordinates must never retain the old two-field prefix size.
        int container = checked((Ep1GameObjectRootValueTokens.AllFields.Count + 2) * 12), fragmentStart = container + 24;
        int modulePrefix = checked(312 + (int)Word(module.InstanceBuffer, 64) * 72), prefix = checked(fragmentStart + modulePrefix);
        var result = new Chunk { InstanceBuffer = new byte[checked(prefix + b.Length)] };
        b.CopyTo(result.InstanceBuffer, prefix);
        for (int i = 0; i < Ep1GameObjectRootValueTokens.AllFields.Count; i++)
        {
            var field = Ep1GameObjectRootValueTokens.AllFields[i];
            Token(i * 12, FastHash.GetHashCode(field.Name), field.Type, (uint)(prefix + field.Offset));
        }
        Token(Ep1GameObjectRootValueTokens.AllFields.Count * 12, FastHash.GetHashCode("Behaviors"), 0x24FCCAE2u, (uint)container);
        Token(container, 0x6C827073u, 0x5D448FD8u, (uint)(prefix + layout.List));
        Token(container + 12, 0xB151A0EBu, 0, (uint)fragmentStart);
        fragment.InstanceBuffer.AsSpan(0, modulePrefix).CopyTo(result.InstanceBuffer.AsSpan(fragmentStart));
        // Reborn: metadata targets have distinct ownership: fragment token offsets shift by the root/container extent, native targets bind the preserved module graph.
        for (int at = 8; at < modulePrefix; at += 12)
        {
            uint target = Word(fragment.InstanceBuffer, at);
            if (target != 0) Put(result.InstanceBuffer, fragmentStart + at, target < modulePrefix ? checked(target + (uint)fragmentStart) : checked(target - (uint)modulePrefix + (uint)prefix + (uint)moduleStart));
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
