using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Chunk = Relo.Chunk;

namespace BinaryAssetBuilder.XmlCompiler;

// Reborn: compose selected complete scripted/tracer/stream modules on one native Draws list and one unchanged Core import occurrence table, not separate compiled assets.
public static class Ep1DrawsTokenizer
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: verify canonical list/leaf ownership before rebasing metadata and the shared native graph exactly once; unknown classes and duplicate local identities fail. */
    //-------------------------------------------------------------------------------------------------
    public static Chunk Tokenize(Chunk native)
        => TokenizeSelected(native, leaf => Word(leaf.InstanceBuffer, 0) switch { 0x4ECF2A4B => Ep1ScriptedDrawTokenizer.Tokenize(leaf), 0x54018E60 => Ep1TracerDrawTokenizer.Tokenize(leaf), 0x60020441 => Ep1ScriptedDrawTokenizer.TokenizeStream(leaf), 0xA44C2999 => Ep1ScriptedDrawTokenizer.TokenizeTruck(leaf), 0x2FDDEC4D => Ep1ScriptedDrawTokenizer.TokenizeLaser(leaf), _ => throw new InvalidDataException("Draws class is not token-enabled.") });
    //-------------------------------------------------------------------------------------------------
    /** Reborn: selected pointer-list families share canonical ownership and coordinate composition, while the caller supplies an explicit fail-closed leaf tokenizer. */
    //-------------------------------------------------------------------------------------------------
    internal static Chunk TokenizeSelected(Chunk native, Func<Chunk, Chunk> tokenize)
    {
        if (native?.InstanceBuffer == null || native.RelocationBuffer == null || native.ImportsBuffer == null || native.InstanceBuffer.Length < 8 || native.InstanceBuffer.Length > 4 * 1048576) throw new InvalidDataException("Bounded Draws native list required.");
        // Reborn: the actual allocated pointer array bounds the count; 64 was a diagnostic quota, not an engine limit.
        var bytes = native.InstanceBuffer; uint serializedCount = Word(bytes, 0); if (serializedCount > (bytes.Length-8)/4) throw new InvalidDataException("Draws module count exceeds allocated buffer."); int count = (int)serializedCount;
        if (Word(bytes, 4) != (count == 0 ? 0u : 8u) || bytes.Length < 8 + count * 4 || count == 0 && bytes.Length != 8) throw new InvalidDataException("Draws canonical header differs.");
        uint[] relos = Slots(native.RelocationBuffer), imports = Slots(native.ImportsBuffer);
        var expectedRelos = new HashSet<uint>(); var expectedImports = new HashSet<uint>(); var ids = new HashSet<uint>();
        if (count != 0) expectedRelos.Add(4);
        var fragments = new List<(int Native, int Prefix, Chunk Tokens)>(); int prefix = checked((1 + count) * 12);
        for (int i = 0; i < count; i++)
        {
            int start = Offset(bytes, 8 + i * 4), end = i + 1 == count ? bytes.Length : Offset(bytes, 12 + i * 4);
            if (start < 8 + count * 4 || i == 0 && start != 8 + count * 4 || end <= start || end - start < 8 || (start | end) % 4 != 0) throw new InvalidDataException("Draws leaf order/extent/alignment differs.");
            uint id = Word(bytes, start + 4); if (id == 0 || !ids.Add(id)) throw new InvalidDataException("Draws module identity missing/duplicate.");
            expectedRelos.Add((uint)(8 + i * 4));
            var leaf = new Chunk { InstanceBuffer = bytes.AsSpan(start, end - start).ToArray() };
            uint[] localRelos = relos.Where(s => s >= start && s < end).Select(s => s - (uint)start).ToArray();
            uint[] localImports = imports.Where(s => s >= start && s < end).Select(s => s - (uint)start).ToArray();
            foreach (uint slot in localRelos)
            {
                uint target = Word(leaf.InstanceBuffer, (int)slot); if (target < start || target >= end) throw new InvalidDataException("Draws leaf pointer crosses owner boundary.");
                Put(leaf.InstanceBuffer, (int)slot, target - (uint)start); expectedRelos.Add(slot + (uint)start);
            }
            foreach (uint slot in localImports) expectedImports.Add(slot + (uint)start);
            leaf.RelocationBuffer = Pack(localRelos); leaf.ImportsBuffer = Pack(localImports);
            Chunk tokens = tokenize(leaf);
            int leafPrefix = tokens.InstanceBuffer.Length - leaf.InstanceBuffer.Length;
            if (leafPrefix <= 0 || leafPrefix % 12 != 0 || prefix > 8 * 1048576 - leafPrefix) throw new InvalidDataException("Draws metadata extent exceeds bound.");
            fragments.Add((start, leafPrefix, tokens)); prefix += leafPrefix;
        }
        if (!expectedRelos.SetEquals(relos) || !expectedImports.SetEquals(imports)) throw new InvalidDataException("Draws parent/leaf auxiliary ownership differs.");
        var output = new Chunk { InstanceBuffer = new byte[prefix + bytes.Length], RelocationBuffer = Pack(relos.Select(s => s + (uint)prefix)), ImportsBuffer = Pack(imports.Select(s => s + (uint)prefix)) };
        Put(output.InstanceBuffer, 0, 0x6C827073); Put(output.InstanceBuffer, 4, 0x5D448FD8); Put(output.InstanceBuffer, 8, (uint)prefix);
        int table = (count + 1) * 12;
        for (int i = 0; i < fragments.Count; i++)
        {
            var fragment = fragments[i]; int marker = (i + 1) * 12;
            Put(output.InstanceBuffer, marker, 0xB151A0EB); Put(output.InstanceBuffer, marker + 8, (uint)table);
            fragment.Tokens.InstanceBuffer.AsSpan(0, fragment.Prefix).CopyTo(output.InstanceBuffer.AsSpan(table));
            for (int at = 8; at < fragment.Prefix; at += 12)
            {
                uint target = Word(output.InstanceBuffer, table + at); if (target == 0) continue;
                Put(output.InstanceBuffer, table + at, target < fragment.Prefix ? checked((uint)table + target) : checked((uint)(prefix + fragment.Native) + target - (uint)fragment.Prefix));
            }
            table += fragment.Prefix;
        }
        bytes.CopyTo(output.InstanceBuffer, prefix);
        foreach (uint slot in relos) Put(output.InstanceBuffer, prefix + (int)slot, checked(Word(bytes, (int)slot) + (uint)prefix));
        return output;
    }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: native pointer offsets must be bounded before narrowing or slicing. */
    //-------------------------------------------------------------------------------------------------
    private static int Offset(byte[] bytes, int at) { uint value = Word(bytes, at); if (value >= bytes.Length) throw new InvalidDataException("Draws module pointer out of bounds."); return (int)value; }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: reject malformed, duplicated or unbounded auxiliary slots before any leaf dereference. */
    //-------------------------------------------------------------------------------------------------
    private static uint[] Slots(byte[] bytes)
    {
        if (bytes.Length == 0) return Array.Empty<uint>(); if (bytes.Length > 4 * 1048576 || bytes.Length % 4 != 0 || Word(bytes, bytes.Length - 4) != uint.MaxValue) throw new InvalidDataException("Draws auxiliary sentinel/bound differs.");
        var result = Enumerable.Range(0, bytes.Length / 4 - 1).Select(i => Word(bytes, i * 4)).ToArray(); if (result.Distinct().Count() != result.Length || result.Any(s => s > 4 * 1048576 - 4)) throw new InvalidDataException("Draws auxiliary slot differs."); return result;
    }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: preserve actual Tracker slot order and omit empty sentinels. */
    //-------------------------------------------------------------------------------------------------
    private static byte[] Pack(IEnumerable<uint> slots) { var values = slots.ToArray(); if (values.Length == 0) return Array.Empty<byte>(); var bytes = new byte[(values.Length + 1) * 4]; for (int i = 0; i < values.Length; i++) Put(bytes, i * 4, values[i]); Put(bytes, values.Length * 4, uint.MaxValue); return bytes; }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: count/type/pointer reads never escape their owned buffer. */
    //-------------------------------------------------------------------------------------------------
    private static uint Word(byte[] bytes, int at) { if (at < 0 || at > bytes.Length - 4) throw new InvalidDataException("Draws word out of bounds."); return BinaryPrimitives.ReadUInt32LittleEndian(bytes.AsSpan(at, 4)); }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: only owned output/leaf arrays are patched. */
    //-------------------------------------------------------------------------------------------------
    private static void Put(byte[] bytes, int at, uint value) => BinaryPrimitives.WriteUInt32LittleEndian(bytes.AsSpan(at, 4), value);
}
