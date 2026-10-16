using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BinaryAssetBuilder.Core.Hashing;
using Chunk = Relo.Chunk;

namespace BinaryAssetBuilder.XmlCompiler;

// Reborn: generate the three reviewed EP1 client families recursively from native ownership, without accepting stock metadata or asset identities as input.
public static class Ep1ClientBehaviorsTokenizer
{
    private enum Kind { Word, Ref, Flags, Enum, RefPointer, MaskPointer, IntPointer, RealPointer, RecordPointer, List, Frames, EmptyList, NullPointer, EmptyEva }
    private sealed record Field(string Name, uint Tag, int Offset, Kind Kind = Kind.Word, Record Child = null, int Limit = 0);
    private sealed record Record(int Size, Field[] Fields, Record Parent = null, uint ParentTag = 0);
    // Reborn: these literal tags and field order are independently checked against the complete six-leaf stock prefix.
    private static readonly Record Module = new(8, new[] { new Field("TypeId", 0x23E650B8, 0), new Field("id", 0xBDF07965, 4) });
    private static readonly Record Client = new(8, Array.Empty<Field>(), Module, 0xD75D93A2);
    private static readonly Record LoopItem = new(124, new[] { new Field("Sound", 0x63D7B468, 0, Kind.RefPointer), new Field("RequiredFlags", 0x67DBBFC9, 4, Kind.Flags), new Field("ExcludedFlags", 0x67DBBFC9, 64, Kind.Flags) });
    private static readonly Record Loop = new(16, new[] { new Field("ModelConditionSound", 0xD8326DDD, 8, Kind.List, LoopItem) }, Client, 0x7B8C5772);
    private static readonly Record SoundOrEva = new(8, new[] { new Field("Sound", 0x63D7B468, 0, Kind.RefPointer), new Field("EvaEvent", 0xC4EC43B1, 4, Kind.EmptyEva) });
    private static readonly Record Voice = new(12, new[] { new Field("AudioType", 0x4DA708B0, 8, Kind.Enum, Limit: 67) }, SoundOrEva, 0x1AE6B6B7);
    private static readonly Record Voices = new(24, new[] { new Field("AudioEntry", 0x8AA9CB14, 0, Kind.List, Voice), new Field("ObjectSpecificEntry", 0xAEDFC4CD, 8, Kind.EmptyList), new Field("NamedEntry", 0xF8ED487B, 16, Kind.EmptyList) });
    private static readonly Record Override = new(132, new[] { new Field("RequiredFlags", 0x67DBBFC9, 0, Kind.Flags), new Field("ExcludedFlags", 0x67DBBFC9, 60, Kind.Flags), new Field("VoicePriorityOverride", 0x23E650B8, 120, Kind.IntPointer), new Field("AudioArrayVoice", 0x09D18241, 124, Kind.RecordPointer, Voices), new Field("AudioArraySound", 0x59867D0B, 128, Kind.NullPointer) });
    private static readonly Record Selector = new(16, new[] { new Field("Override", 0xBFA5BEBA, 8, Kind.List, Override) }, Client, 0x7B8C5772);
    private static readonly Record AnimationItem = new(24, new[] { new Field("Sound", 0x63D7B468, 0, Kind.Ref), new Field("RequiredMC", 0x67DBBFC9, 4, Kind.MaskPointer), new Field("ExcludedMC", 0x67DBBFC9, 8, Kind.MaskPointer), new Field("Animation", 0x26480425, 12), new Field("Frame", 0x49EF147D, 16, Kind.Frames) });
    private static readonly Record Animation = new(20, new[] { new Field("MaxUpdateRangeCap", 0xBC2F1CBF, 8, Kind.RealPointer), new Field("Sound", 0xEB6C6024, 12, Kind.List, AnimationItem) }, Client, 0x7B8C5772);

    //-------------------------------------------------------------------------------------------------
    /** Reborn: admit only bounded complete canonical containers; populated unreviewed audio children and malformed auxiliary ownership fail before returning output. */
    //-------------------------------------------------------------------------------------------------
    public static Chunk Tokenize(Chunk native)
    {
        if (native?.InstanceBuffer == null || native.RelocationBuffer == null || native.ImportsBuffer == null || native.InstanceBuffer.Length < 8 || native.InstanceBuffer.Length > 2 * 1048576) throw new InvalidDataException("Bounded client native list required.");
        return new Builder(native.InstanceBuffer).Build(native);
    }

    // Reborn: metadata, pointer patches and owner inventories are isolated to one invocation.
    private sealed class Builder
    {
        private readonly byte[] bytes;
        private readonly bool[] covered;
        private readonly List<uint> metadata = new();
        private readonly List<(int At, int Target)> targets = new();
        private readonly HashSet<uint> relos = new(), imports = new();
        private int lower, upper, visits;

        //-------------------------------------------------------------------------------------------------
        /** Reborn: the only input is an owned native graph, never a stock token template. */
        //-------------------------------------------------------------------------------------------------
        internal Builder(byte[] bytes) { this.bytes = bytes; covered = new bool[bytes.Length]; upper = bytes.Length; }

        //-------------------------------------------------------------------------------------------------
        /** Reborn: preserve Tracker slot ordering and import occurrence values while shifting native pointers exactly once after generating all tables. */
        //-------------------------------------------------------------------------------------------------
        internal Chunk Build(Chunk native)
        {
            uint count = Word(0); if (count > 64 || Word(4) != (count == 0 ? 0u : 8u) || count == 0 && bytes.Length != 8) throw new InvalidDataException("Client canonical list differs.");
            Claim(0, checked(8 + (int)count * 4)); int list = Allocate(1 + (int)count); Row(list, 0x6C827073, 0x5D448FD8, 0); Native(list + 8, 0);
            if (count != 0) relos.Add(4); var ids = new HashSet<uint>();
            for (int i = 0; i < count; i++)
            {
                lower = 0; upper = bytes.Length; int start = Offset(Word(8 + i * 4)), end = i + 1 == count ? bytes.Length : Offset(Word(12 + i * 4));
                if (start < 8 + count * 4 || i == 0 && start != 8 + count * 4 || end <= start || (start | end) % 4 != 0) throw new InvalidDataException("Client leaf boundaries differ.");
                lower = start; upper = end; uint type = Word(start), id = Word(start + 4); if (id == 0 || !ids.Add(id)) throw new InvalidDataException("Client local identity missing/duplicate.");
                Record descriptor = type switch { 0x5A3EDBF4 => Loop, 0x327CFB76 => Selector, 0x27EF791D => Animation, _ => throw new InvalidDataException("Client class is not token-enabled.") };
                Claim(start, descriptor.Size); relos.Add((uint)(8 + i * 4)); int wrapper = Allocate(1); Row(list + 12 + i * 12, 0xB151A0EB, 0, (uint)wrapper); Row(wrapper, 2, type, (uint)Emit(descriptor, start, 0));
            }
            lower = 0; upper = bytes.Length; uint[] suppliedRelos = Slots(native.RelocationBuffer), suppliedImports = Slots(native.ImportsBuffer);
            if (!relos.SetEquals(suppliedRelos) || !imports.SetEquals(suppliedImports)) throw new InvalidDataException("Client auxiliary ownership differs.");
            int gap = 0; for (int i = 0; i < bytes.Length; i++) { if (covered[i]) { gap = 0; continue; } if (bytes[i] != 0 || ++gap > 3) throw new InvalidDataException("Client unowned storage at " + i); }
            int prefix = checked(metadata.Count * 4); var output = new Chunk { InstanceBuffer = new byte[prefix + bytes.Length], RelocationBuffer = Pack(suppliedRelos.Select(s => s + (uint)prefix)), ImportsBuffer = Pack(suppliedImports.Select(s => s + (uint)prefix)) };
            for (int i = 0; i < metadata.Count; i++) Put(output.InstanceBuffer, i * 4, metadata[i]); foreach (var patch in targets) Put(output.InstanceBuffer, patch.At, checked((uint)(prefix + patch.Target)));
            bytes.CopyTo(output.InstanceBuffer, prefix); foreach (uint slot in suppliedRelos) Put(output.InstanceBuffer, prefix + (int)slot, checked(Word((int)slot) + (uint)prefix)); return output;
        }

        //-------------------------------------------------------------------------------------------------
        /** Reborn: schema-order tables reserve parent/list markers before descendants, retaining null versus allocated-empty references. */
        //-------------------------------------------------------------------------------------------------
        private int Emit(Record descriptor, int native, int depth)
        {
            if (depth > 32 || ++visits > 100000) throw new InvalidDataException("Client traversal bound exceeded."); Range(native, descriptor.Size);
            int table = Allocate(descriptor.Fields.Length + (descriptor.Parent == null ? 0 : 1) + 1), next = table;
            if (descriptor.Parent != null) { Row(next, 1, descriptor.ParentTag, (uint)Emit(descriptor.Parent, native, depth + 1)); next += 12; }
            foreach (var field in descriptor.Fields)
            {
                int slot = checked(native + field.Offset); uint value = Word(slot); Row(next, FastHash.GetHashCode(field.Name), field.Tag, 0);
                switch (field.Kind)
                {
                    case Kind.List: case Kind.Frames: case Kind.EmptyList:
                        uint pointer = Word(slot + 4); if (value > 4096 || (value == 0) != (pointer == 0) || field.Kind == Kind.EmptyList && value != 0 || field.Kind == Kind.Frames && value == 0) throw new InvalidDataException("Client list count/pointer/admission differs: " + field.Name);
                        int list = Allocate(1 + (int)value); Row(list, 0x6C827073, 0x5D448FD8, 0); Native(list + 8, slot); Target(next + 8, list);
                        if (value != 0)
                        {
                            int stride = field.Kind == Kind.Frames ? 4 : field.Child.Size, items = Offset(pointer); Claim(items, checked((int)value * stride)); relos.Add((uint)(slot + 4));
                            for (int i = 0; i < value; i++) { int item = items + i * stride, marker = list + 12 + i * 12; Row(marker, 0xB151A0EB, 0, 0); if (field.Kind == Kind.Frames) { if (Word(item) > int.MaxValue) throw new InvalidDataException("Client negative frame."); Native(marker + 8, item); } else Target(marker + 8, Emit(field.Child, item, depth + 1)); }
                        }
                        break;
                    case Kind.RefPointer: case Kind.MaskPointer: case Kind.IntPointer: case Kind.RealPointer: case Kind.RecordPointer:
                        if (value != 0)
                        {
                            int target = Offset(value), size = field.Kind == Kind.MaskPointer ? 60 : field.Kind == Kind.RecordPointer ? field.Child.Size : 4; Claim(target, size); relos.Add((uint)slot);
                            if (field.Kind == Kind.RefPointer && Word(target) != 0) imports.Add((uint)target);
                            if (field.Kind == Kind.MaskPointer) Mask(target);
                            if (field.Kind == Kind.RealPointer && (!float.IsFinite(BitConverter.Int32BitsToSingle(unchecked((int)Word(target)))) || BitConverter.Int32BitsToSingle(unchecked((int)Word(target))) <= 0)) throw new InvalidDataException("Client positive finite range cap required.");
                            if (field.Kind == Kind.RecordPointer) Target(next + 8, Emit(field.Child, target, depth + 1)); else Native(next + 8, target);
                        }
                        break;
                    case Kind.NullPointer: if (value != 0) throw new InvalidDataException("Client populated child is not token-enabled: " + field.Name); break;
                    default:
                        if (field.Kind == Kind.EmptyEva && value != 0) throw new InvalidDataException("Client populated EvaEvent ABI is not reviewed.");
                        if (field.Kind == Kind.Enum && value >= field.Limit) throw new InvalidDataException("Client EP1 audio ordinal differs.");
                        if (field.Kind == Kind.Flags) Mask(slot); if (field.Kind == Kind.Ref && value != 0) imports.Add((uint)slot); Native(next + 8, slot); break;
                }
                next += 12;
            }
            return table;
        }
        //-------------------------------------------------------------------------------------------------
        /** Reborn: all 463 defined model-condition bits fit exactly fifteen words; undefined high bits fail. */
        //-------------------------------------------------------------------------------------------------
        private void Mask(int at) { Range(at, 60); if (Word(at + 56) >> 15 != 0) throw new InvalidDataException("Client undefined model-condition bits."); }
        //-------------------------------------------------------------------------------------------------
        /** Reborn: each allocation belongs to exactly one leaf and cannot alias another record or payload. */
        //-------------------------------------------------------------------------------------------------
        private void Claim(int at, int length) { Range(at, length); for (int i = at; i < at + length; i++) if (covered[i]) throw new InvalidDataException("Client native allocations overlap."); Array.Fill(covered, true, at, length); }
        //-------------------------------------------------------------------------------------------------
        /** Reborn: tables have bounded zero-initialized rows, including record terminators. */
        //-------------------------------------------------------------------------------------------------
        private int Allocate(int rows) { if (rows < 1 || rows > (1048576 - metadata.Count) / 3) throw new InvalidDataException("Client metadata allocation bound exceeded."); int at = metadata.Count * 4; for (int i = 0; i < rows * 3; i++) metadata.Add(0); return at; }
        //-------------------------------------------------------------------------------------------------
        /** Reborn: metadata targets and native targets are tracked separately until the prefix size is known. */
        //-------------------------------------------------------------------------------------------------
        private void Native(int at, int target) { Range(target, 4); targets.Add((at, target)); }
        //-------------------------------------------------------------------------------------------------
        /** Reborn: metadata offsets remain relative to the new table origin. */
        //-------------------------------------------------------------------------------------------------
        private void Target(int at, int target) => metadata[at / 4] = (uint)target;
        //-------------------------------------------------------------------------------------------------
        /** Reborn: row writes only address newly allocated metadata. */
        //-------------------------------------------------------------------------------------------------
        private void Row(int at, uint name, uint tag, uint target) { metadata[at / 4] = name; metadata[at / 4 + 1] = tag; metadata[at / 4 + 2] = target; }
        //-------------------------------------------------------------------------------------------------
        /** Reborn: narrowing a pointer is allowed only inside its current leaf owner. */
        //-------------------------------------------------------------------------------------------------
        private int Offset(uint value) { if (value > int.MaxValue || value % 4 != 0) throw new InvalidDataException("Client pointer overflow/alignment differs."); Range((int)value, 4); return (int)value; }
        //-------------------------------------------------------------------------------------------------
        /** Reborn: all native reads are bounded by the current leaf, not merely the whole container. */
        //-------------------------------------------------------------------------------------------------
        private void Range(int at, int length) { if (length < 0 || at < lower || at > upper - length) throw new InvalidDataException("Client native owner bounds differ."); }
        //-------------------------------------------------------------------------------------------------
        /** Reborn: bounded little-endian reads preserve signed payload bits. */
        //-------------------------------------------------------------------------------------------------
        private uint Word(int at) { Range(at, 4); return BinaryPrimitives.ReadUInt32LittleEndian(bytes.AsSpan(at, 4)); }
    }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: auxiliary arrays require complete framing, unique aligned bounded slots and no empty sentinel. */
    //-------------------------------------------------------------------------------------------------
    private static uint[] Slots(byte[] bytes) { if (bytes.Length == 0) return Array.Empty<uint>(); if (bytes.Length < 8 || bytes.Length > 2 * 1048576 || bytes.Length % 4 != 0 || BinaryPrimitives.ReadUInt32LittleEndian(bytes.AsSpan(bytes.Length - 4)) != uint.MaxValue) throw new InvalidDataException("Client auxiliary framing differs."); var result = Enumerable.Range(0, bytes.Length / 4 - 1).Select(i => BinaryPrimitives.ReadUInt32LittleEndian(bytes.AsSpan(i * 4, 4))).ToArray(); if (result.Distinct().Count() != result.Length || result.Any(s => s % 4 != 0 || s > 2 * 1048576 - 4)) throw new InvalidDataException("Client auxiliary slot differs."); return result; }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: real Tracker order is retained, with a sentinel only for nonempty arrays. */
    //-------------------------------------------------------------------------------------------------
    private static byte[] Pack(IEnumerable<uint> slots) { var values = slots.ToArray(); if (values.Length == 0) return Array.Empty<byte>(); var bytes = new byte[(values.Length + 1) * 4]; for (int i = 0; i < values.Length; i++) Put(bytes, i * 4, values[i]); Put(bytes, values.Length * 4, uint.MaxValue); return bytes; }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: only newly owned output arrays receive pointer and metadata patches. */
    //-------------------------------------------------------------------------------------------------
    private static void Put(byte[] bytes, int at, uint value) => BinaryPrimitives.WriteUInt32LittleEndian(bytes.AsSpan(at, 4), value);
}
