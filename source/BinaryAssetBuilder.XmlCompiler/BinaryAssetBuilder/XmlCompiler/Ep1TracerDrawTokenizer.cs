using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BinaryAssetBuilder.Core.Hashing;
using Chunk = Relo.Chunk;

namespace BinaryAssetBuilder.XmlCompiler;

// Reborn: generate the complete selected tracer Draw graph, with inline color tables and optional two-mask status validation; no whole GameObject publication.
public static class Ep1TracerDrawTokenizer
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: require canonical owned native records and exact pointer/import slots before emitting detached metadata coordinates. */
    //-------------------------------------------------------------------------------------------------
    public static Chunk Tokenize(Chunk native)
    {
        if (native?.InstanceBuffer == null || native.RelocationBuffer == null || native.ImportsBuffer == null || native.InstanceBuffer.Length is not (112 or 176)) throw new InvalidDataException("Bounded tracer native graph required.");
        var b = native.InstanceBuffer; bool status = Word(b, 104) != 0;
        if (Word(b, 0) != 0x54018E60 || Word(b, 4) == 0 || Word(b, 48) > 5 || b[108] > 1 || b[109] != 0 || b[110] != 0 || b[111] != 0 || b.Length != (status ? 176 : 112) || Word(b, 104) != (status ? 112u : 0u)) throw new InvalidDataException("Tracer identity/enum/bool/padding/owner differs.");
        foreach (int at in Enumerable.Range(0, 10).Select(i => 8 + i * 4).Concat(Enumerable.Range(0, 8).Select(i => 72 + i * 4))) if (!float.IsFinite(BitConverter.Int32BitsToSingle(unchecked((int)Word(b, at))))) throw new InvalidDataException("Tracer finite real/color required.");
        if (status && ((Word(b, 140) & ~63u) != 0 || (Word(b, 172) & ~63u) != 0)) throw new InvalidDataException("Tracer status has undefined high bits.");
        uint[] relos = status ? new[] { 104u } : Array.Empty<uint>();
        uint[] imports = new[] { 56u, 60u, 64u }.Where(s => Word(b, (int)s) != 0).ToArray();
        if (!native.RelocationBuffer.SequenceEqual(Pack(relos)) || !native.ImportsBuffer.SequenceEqual(Pack(imports))) throw new InvalidDataException("Tracer auxiliary ownership differs.");
        int prefix = status ? 492 : 456; var output = new Chunk { InstanceBuffer = new byte[prefix + b.Length], RelocationBuffer = Pack(relos.Select(s => s + (uint)prefix)), ImportsBuffer = Pack(imports.Select(s => s + (uint)prefix)) }; b.CopyTo(output.InstanceBuffer, prefix);
        Row(0, 2, 0x54018E60, 12); Row(12, 1, 0x17F61728, 276);
        string[] reals = { "MinLength", "MaxLength", "Width", "MinSpeed", "MaxSpeed", "SweepSpeed", "SpreadAngle", "MinTracersPerFrame", "MaxTracersPerFrame", "FrameLifeTime" };
        for (int i = 0; i < reals.Length; i++) Row(24 + i * 12, FastHash.GetHashCode(reals[i]), 0x645864F3, (uint)(prefix + 8 + i * 4));
        Row(144, FastHash.GetHashCode("WeaponSlotType"), 0x1848C549, (uint)(prefix + 48)); Row(156, FastHash.GetHashCode("WeaponSlotID"), 0x23E650B8, (uint)(prefix + 52)); Row(168, FastHash.GetHashCode("Texture"), 0xBDB63F16, (uint)(prefix + 56)); Row(180, FastHash.GetHashCode("UseAdditiveBlending"), 0x77E3F4F7, (uint)(prefix + 108));
        Row(192, FastHash.GetHashCode("TracerHitFx"), 0x0D1C8E83, (uint)(prefix + 60)); Row(204, FastHash.GetHashCode("TracerEmitFx"), 0x0D1C8E83, (uint)(prefix + 64)); Row(216, FastHash.GetHashCode("TextureIndex"), 0x23E650B8, (uint)(prefix + 68));
        Row(228, FastHash.GetHashCode("HeadColor"), 0x16ADF350, 336); Row(240, FastHash.GetHashCode("TailColor"), 0x16ADF350, 396); Row(252, FastHash.GetHashCode("ObjectStatusValidation"), 0x3E8F938C, status ? 456u : 0u);
        Row(276, 1, 0xD75D93A2, 300); Row(300, FastHash.GetHashCode("TypeId"), 0x23E650B8, (uint)prefix); Row(312, FastHash.GetHashCode("id"), 0xBDF07965, (uint)(prefix + 4));
        foreach (var color in new[] { (336, 72), (396, 88) }) for (int i = 0; i < 4; i++) Row(color.Item1 + i * 12, FastHash.GetHashCode(new[] { "r", "g", "b", "a" }[i]), 0x645864F3, (uint)(prefix + color.Item2 + i * 4));
        if (status) { Row(456, FastHash.GetHashCode("ForbiddenStatus"), 0xA7BD811E, (uint)(prefix + 112)); Row(468, FastHash.GetHashCode("RequiredStatus"), 0xA7BD811E, (uint)(prefix + 144)); Put(output.InstanceBuffer, prefix + 104, (uint)(prefix + 112)); }
        return output;
        //-------------------------------------------------------------------------------------------------
        /** Reborn: metadata targets are not native relocation slots and remain on a separate origin. */
        //-------------------------------------------------------------------------------------------------
        void Row(int at, uint name, uint type, uint target) { Put(output.InstanceBuffer, at, name); Put(output.InstanceBuffer, at + 4, type); Put(output.InstanceBuffer, at + 8, target); }
    }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: exact empty auxiliary streams omit the sentinel; nonempty streams end in FFFFFFFF. */
    //-------------------------------------------------------------------------------------------------
    private static byte[] Pack(IEnumerable<uint> slots) { var values = slots.ToArray(); if (values.Length == 0) return Array.Empty<byte>(); var b = new byte[(values.Length + 1) * 4]; for (int i = 0; i < values.Length; i++) Put(b, i * 4, values[i]); Put(b, values.Length * 4, uint.MaxValue); return b; }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: input word bounds are checked before interpreting offsets. */
    //-------------------------------------------------------------------------------------------------
    private static uint Word(byte[] b, int at) { if (at < 0 || at > b.Length - 4) throw new InvalidDataException("Tracer word out of bounds."); return BinaryPrimitives.ReadUInt32LittleEndian(b.AsSpan(at, 4)); }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: only newly owned output bytes are rewritten. */
    //-------------------------------------------------------------------------------------------------
    private static void Put(byte[] b, int at, uint value) => BinaryPrimitives.WriteUInt32LittleEndian(b.AsSpan(at, 4), value);
}
