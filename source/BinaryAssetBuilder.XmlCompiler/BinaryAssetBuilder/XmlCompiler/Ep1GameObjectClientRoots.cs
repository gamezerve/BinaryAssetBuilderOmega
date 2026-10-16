using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Chunk = Relo.Chunk;

namespace BinaryAssetBuilder.XmlCompiler;

// Reborn: isolate reviewed pre-behavior ClientUpdates/ClientBehaviors owners without absorbing Draws, AI, pointed roots or one another.
public static class Ep1GameObjectClientRoots
{
    // Reborn: absent and allocated-empty containers keep distinct root semantics and independent metadata/native origins.
    public sealed record Layout(int Slot, int Start, int End, IReadOnlyList<int> Modules, Chunk Tokens)
    {
        public int Prefix => Tokens == null ? 0 : Tokens.InstanceBuffer.Length - (End - Start);
    }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: check disjoint owner extents and exact auxiliary partitions, then reuse the independently stock-proven leaf/container tokenizers. */
    //-------------------------------------------------------------------------------------------------
    internal static IReadOnlyList<Layout> Inspect(byte[] data, uint list, uint[] relos, uint[] imports, uint armor, uint armorCount, uint locomotor, uint locomotorCount, IReadOnlyList<Ep1GameObjectPointerRoots.Field> pointerRoots, IReadOnlyList<(uint Start, uint End)> strings, Ep1GameObjectAiRoot.Layout ai, Ep1GameObjectDrawRoot.Layout draws, List<uint> expectedRelos, List<uint> expectedImports)
    {
        var outer = strings.Select(s => ((int)s.Start, (int)s.End)).ToList();
        if (armorCount != 0) outer.Add(((int)armor, checked((int)(armor + armorCount * 12))));
        if (locomotorCount != 0) outer.Add(((int)locomotor, checked((int)(locomotor + locomotorCount * 12))));
        foreach (var field in pointerRoots)
        {
            outer.Add((field.Target, field.Target + field.Size));
            if (field.Name is "DisplayName" or "DisplayNameTransformed") { uint text = Word(field.Target + 4), length = Word(field.Target); if (text != 0) outer.Add(((int)text, checked((int)(text + length + 1)))); }
        }
        if (ai.Modules.Count != 0) { outer.Add((ai.Items, ai.Items + ai.Modules.Count * 4)); outer.AddRange(ai.Modules.Select(m => (m.Start, m.End))); }
        if (draws.Start != 0) outer.Add((draws.Start, draws.End));
        foreach (int slot in new[] { 492, 496 }) { uint start = Word(slot); if (start != 0) { if (start < 600 || start % 4 != 0 || start > list || 8 > list - start) throw new InvalidDataException("Client root pointer bounds/alignment differ."); } }
        if (Word(492) != 0 && Word(492) == Word(496)) throw new InvalidDataException("Client root containers alias.");
        var result = new List<Layout>();
        foreach (int slot in new[] { 492, 496 })
        {
            int start = (int)Word(slot); if (start == 0) { result.Add(new(slot, 0, 0, Array.Empty<int>(), null)); continue; }
            int end = (int)list, sibling = (int)Word(slot == 492 ? 496 : 492); if (sibling > start && sibling < end) end = sibling;
            foreach (var range in outer) if (range.Item1 > start && range.Item1 < end) end = range.Item1;
            if (end - start < 8 || outer.Any(r => start < r.Item2 && end > r.Item1)) throw new InvalidDataException("Client root aliases another native owner.");
            uint[] localRelos = relos.Where(s => s >= start && s < end).ToArray(), localImports = imports.Where(s => s >= start && s < end).ToArray();
            if (localRelos.Concat(localImports).Any(s => s % 4 != 0 || s > end - 4)) throw new InvalidDataException("Client root auxiliary word straddles owner.");
            var native = new Chunk { InstanceBuffer = data.AsSpan(start, end - start).ToArray(), RelocationBuffer = Pack(localRelos.Select(s => s - (uint)start)), ImportsBuffer = Pack(localImports.Select(s => s - (uint)start)) };
            foreach (uint at in localRelos) { uint target = Word((int)at); if (target < start || target >= end) throw new InvalidDataException("Client root pointer crosses owners."); Put(native.InstanceBuffer, (int)at - start, target - (uint)start); }
            var tokens = slot == 496 ? Ep1ClientBehaviorsTokenizer.Tokenize(native) : Ep1DrawsTokenizer.TokenizeSelected(native, Ep1GroupAudioClientTokenizer.Tokenize);
            int count = (int)Word(start); var modules = new int[count]; for (int i = 0; i < count; i++) modules[i] = (int)Word(start + 8 + i * 4);
            expectedRelos.Add((uint)slot); expectedRelos.AddRange(localRelos); expectedImports.AddRange(localImports); outer.Add((start, end));
            result.Add(new(slot, start, end, Array.AsReadOnly(modules), tokens));
        }
        return Array.AsReadOnly(result.ToArray());
        //-------------------------------------------------------------------------------------------------
        /** Reborn: all root/header reads stay inside this owned native graph. */
        //-------------------------------------------------------------------------------------------------
        uint Word(int at) { if (at < 0 || at > data.Length - 4) throw new InvalidDataException("Client root read bounds exceeded."); return BinaryPrimitives.ReadUInt32LittleEndian(data.AsSpan(at, 4)); }
    }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: extracted slots preserve original Tracker order and reference occurrence words. */
    //-------------------------------------------------------------------------------------------------
    private static byte[] Pack(IEnumerable<uint> slots) { var values = slots.ToArray(); if (values.Length == 0) return Array.Empty<byte>(); var bytes = new byte[(values.Length + 1) * 4]; for (int i = 0; i < values.Length; i++) Put(bytes, i * 4, values[i]); Put(bytes, values.Length * 4, uint.MaxValue); return bytes; }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: only newly owned extraction buffers receive local pointer patches. */
    //-------------------------------------------------------------------------------------------------
    private static void Put(byte[] bytes, int at, uint value) => BinaryPrimitives.WriteUInt32LittleEndian(bytes.AsSpan(at, 4), value);
}
