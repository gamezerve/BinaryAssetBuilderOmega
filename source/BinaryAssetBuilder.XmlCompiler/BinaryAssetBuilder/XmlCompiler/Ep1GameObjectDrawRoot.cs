using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Chunk = Relo.Chunk;

namespace BinaryAssetBuilder.XmlCompiler;

// Reborn: isolate the complete selected Draws owner from neighboring root strings, arrays, pointed records and inline AI before joining root metadata.
public static class Ep1GameObjectDrawRoot
{
    // Reborn: absent and allocated-empty Draws remain distinct; native and metadata offsets use separate origins.
    public sealed record Layout(int Start, int End, IReadOnlyList<int> Modules, Chunk Tokens)
    {
        public int Prefix => Tokens == null ? 0 : Tokens.InstanceBuffer.Length - (End - Start);
    }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: determine the next independent root owner, reject every overlap/cross-owner pointer, then reuse the stock-proven standalone Draws tokenizer unchanged. */
    //-------------------------------------------------------------------------------------------------
    internal static Layout Inspect(byte[] data, uint list, uint[] relos, uint[] imports, uint armor, uint armorCount, uint locomotor, uint locomotorCount, IReadOnlyList<Ep1GameObjectPointerRoots.Field> pointerRoots, IReadOnlyList<(uint Start, uint End)> strings, Ep1GameObjectAiRoot.Layout ai, List<uint> expectedRelos, List<uint> expectedImports)
    {
        uint pointer = Word(472); if (pointer == 0) return new(0, 0, Array.Empty<int>(), null);
        if (pointer < 600 || pointer % 4 != 0 || pointer > list || 8 > list - pointer) throw new InvalidDataException("Draws root pointer bounds/alignment differ.");
        var outer = strings.Select(s => ((int)s.Start, (int)s.End)).ToList();
        if (armorCount != 0) outer.Add(((int)armor, checked((int)(armor + armorCount * 12))));
        if (locomotorCount != 0) outer.Add(((int)locomotor, checked((int)(locomotor + locomotorCount * 12))));
        foreach (var field in pointerRoots)
        {
            outer.Add((field.Target, field.Target + field.Size));
            if (field.Name is "DisplayName" or "DisplayNameTransformed") { uint text = Word(field.Target + 4), length = Word(field.Target); if (text != 0) outer.Add(((int)text, checked((int)(text + length + 1)))); }
        }
        if (ai.Modules.Count != 0) { outer.Add((ai.Items, ai.Items + ai.Modules.Count * 4)); outer.AddRange(ai.Modules.Select(m => (m.Start, m.End))); }
        int start = (int)pointer, end = (int)list;
        foreach (var range in outer) if (range.Item1 > start && range.Item1 < end) end = range.Item1;
        if (end - start < 8 || outer.Any(r => start < r.Item2 && end > r.Item1)) throw new InvalidDataException("Draws root owner aliases other root storage.");
        uint[] localRelos = relos.Where(s => s >= start && s < end).ToArray(), localImports = imports.Where(s => s >= start && s < end).ToArray();
        if (localRelos.Concat(localImports).Any(s => s % 4 != 0 || s > end - 4)) throw new InvalidDataException("Draws root auxiliary word straddles its owner.");
        var native = new Chunk { InstanceBuffer = data.AsSpan(start, end - start).ToArray(), RelocationBuffer = Pack(localRelos.Select(s => s - (uint)start)), ImportsBuffer = Pack(localImports.Select(s => s - (uint)start)) };
        foreach (uint slot in localRelos) { uint target = Word((int)slot); if (target < start || target >= end) throw new InvalidDataException("Draws pointer crosses root owners."); Put(native.InstanceBuffer, (int)slot - start, target - (uint)start); }
        var tokens = Ep1DrawsTokenizer.Tokenize(native); int count = (int)Word(start); var modules = new int[count];
        for (int i = 0; i < count; i++) modules[i] = (int)Word(start + 8 + i * 4);
        expectedRelos.Add(472); expectedRelos.AddRange(localRelos); expectedImports.AddRange(localImports);
        return new(start, end, Array.AsReadOnly(modules), tokens);
        //-------------------------------------------------------------------------------------------------
        /** Reborn: root/native record reads remain bounded to the current owned GameObject buffer. */
        //-------------------------------------------------------------------------------------------------
        uint Word(int at) { if (at < 0 || at > data.Length - 4) throw new InvalidDataException("Draws root read exceeds native bounds."); return BinaryPrimitives.ReadUInt32LittleEndian(data.AsSpan(at, 4)); }
    }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: extracted auxiliary slots preserve the shared Core occurrence values and their actual Tracker order. */
    //-------------------------------------------------------------------------------------------------
    private static byte[] Pack(IEnumerable<uint> slots) { var values = slots.ToArray(); if (values.Length == 0) return Array.Empty<byte>(); var bytes = new byte[(values.Length + 1) * 4]; for (int i = 0; i < values.Length; i++) Put(bytes, i * 4, values[i]); Put(bytes, values.Length * 4, uint.MaxValue); return bytes; }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: native pointer rebasing only changes freshly extracted arrays. */
    //-------------------------------------------------------------------------------------------------
    private static void Put(byte[] bytes, int at, uint value) => BinaryPrimitives.WriteUInt32LittleEndian(bytes.AsSpan(at, 4), value);
}
