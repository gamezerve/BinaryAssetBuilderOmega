using System.Buffers.Binary;
using BinaryAssetBuilder.Core.Hashing;
using BinaryAssetBuilder.XmlCompiler;
using Relo;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: independent stock-prefix joins and native round trips validate the generated module fragment without claiming an enclosing GameObject stream.
internal static class DestructionTokenWriterSmokeTest
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: compare every generated stock token row after semantic address rebasing, then check malformed native input refusals. */
    //-------------------------------------------------------------------------------------------------
    internal static void Stock(Ep1AlliedModuleSmokeTest.Output source, byte[] stock, GameObjectModuleReview.Module module)
    {
        var native = Extract(source); var result = Check(native); int prefix = (int)Word(result.InstanceBuffer, 152);
        if (prefix != 384) throw new InvalidDataException("Selected destruction token prefix size differs.");
        var expected = stock.AsSpan(module.Wrapper, prefix).ToArray();
        for (int at = 8; at < prefix; at += 12)
        {
            uint target = Word(expected, at);
            if (target == 0) continue;
            uint rebased = target >= module.Native ? checked(target - (uint)module.Native + (uint)prefix) : checked(target - (uint)module.Wrapper);
            Put(expected, at, rebased);
        }
        if (!expected.SequenceEqual(result.InstanceBuffer.AsSpan(0, prefix).ToArray())) throw new InvalidDataException("Generated destruction token prefix differs from independently captured stock.");
        foreach (int slot in new[] { 0, 8, 12, 16, 20, 68, 76 })
        {
            var bad = Clone(native); Put(bad.InstanceBuffer, slot, Word(bad.InstanceBuffer, slot) ^ 4u);
            Refuse(() => Ep1DestructionTokenizer.Tokenize(bad));
        }
        var badRelo = Clone(native); Put(badRelo.RelocationBuffer, 0, 0); Refuse(() => Ep1DestructionTokenizer.Tokenize(badRelo));
        var badImp = Clone(native); Put(badImp.ImportsBuffer, badImp.ImportsBuffer.Length - 4, 0); Refuse(() => Ep1DestructionTokenizer.Tokenize(badImp));
        Console.WriteLine($"Destruction generated tokens: stock prefix={prefix}, BIN/RELO/IMP={result.InstanceBuffer.Length}/{result.RelocationBuffer.Length}/{result.ImportsBuffer.Length}; nine input corruptions refused.");
    }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: authored variable item counts, strings and named-reference occurrence indices use the same writer and independent field/slot round trip. */
    //-------------------------------------------------------------------------------------------------
    internal static void Authored(Ep1AlliedModuleSmokeTest.Output source) => Check(Extract(source));
    //-------------------------------------------------------------------------------------------------
    /** Reborn: validate generated list/field metadata and remove exactly its prefix/pointer shift to recover the untouched native graph and auxiliary slots. */
    //-------------------------------------------------------------------------------------------------
    private static Chunk Check(Chunk native)
    {
        var result = Ep1DestructionTokenizer.Tokenize(native); var repeat = Ep1DestructionTokenizer.Tokenize(native);
        if (!result.InstanceBuffer.SequenceEqual(repeat.InstanceBuffer) || !result.RelocationBuffer.SequenceEqual(repeat.RelocationBuffer) || !result.ImportsBuffer.SequenceEqual(repeat.ImportsBuffer)) throw new InvalidDataException("Destruction token writer repeat drift.");
        int prefix = (int)Word(result.InstanceBuffer, 152), count = (int)Word(native.InstanceBuffer, 64);
        if (prefix != 312 + count * 72 || Word(result.InstanceBuffer, 308) != prefix + 64) throw new InvalidDataException("Generated destruction list anchor differs.");
        for (int i = 0; i < count; i++)
        {
            int marker = 312 + i * 12, table = (int)Word(result.InstanceBuffer, marker + 8);
            if (Word(result.InstanceBuffer, marker) != 0xB151A0EBu || Word(result.InstanceBuffer, marker + 4) != 0 || table != 312 + count * 12 + i * 60)
                throw new InvalidDataException("Generated destruction item marker differs.");
            var fields = GameObjectStockReview.Tokens(result.InstanceBuffer, table);
            foreach (var f in new[] { ("BonePrefix", 0), ("FX", 8), ("CreationObject", 12), ("FlingVelocity", 16) })
                if (fields[FastHash.GetHashCode(f.Item1)].Offset != prefix + 72 + i * 20 + f.Item2) throw new InvalidDataException("Generated authored Item field differs.");
        }
        var recovered = result.InstanceBuffer.AsSpan(prefix).ToArray();
        var relos = Slots(result.RelocationBuffer); var imports = Slots(result.ImportsBuffer);
        if (!relos.Select(s => s - (uint)prefix).SequenceEqual(Slots(native.RelocationBuffer)) || !imports.Select(s => s - (uint)prefix).SequenceEqual(Slots(native.ImportsBuffer)))
            throw new InvalidDataException("Generated destruction auxiliary slots drift.");
        foreach (uint slot in relos)
        {
            if (slot < prefix || Word(result.InstanceBuffer, (int)slot) < prefix) throw new InvalidDataException("Generated destruction pointer escapes native region.");
            Put(recovered, (int)slot - prefix, Word(result.InstanceBuffer, (int)slot) - (uint)prefix);
        }
        if (!recovered.SequenceEqual(native.InstanceBuffer)) throw new InvalidDataException("Generated destruction native graph changed.");
        return result;
    }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: remove only the known GameObject parent/list envelope from actual dispatch output; no stock bytes or token prefix enter the writer. */
    //-------------------------------------------------------------------------------------------------
    private static Chunk Extract(Ep1AlliedModuleSmokeTest.Output source)
    {
        int start = source.Native; var b = source.Chunk.InstanceBuffer.AsSpan(start).ToArray();
        var parent = new[] { 476u, 604u, 608u }; var slots = Slots(source.Chunk.RelocationBuffer);
        if (!slots.Where(s => s < start).Order().SequenceEqual(parent)) throw new InvalidDataException("Unexpected destruction parent envelope.");
        var own = slots.Where(s => s >= start).ToArray();
        foreach (uint slot in own)
        {
            uint pointer = Word(source.Chunk.InstanceBuffer, (int)slot);
            if (pointer < start) throw new InvalidDataException("Destruction module points outside its graph.");
            Put(b, (int)slot - start, pointer - (uint)start);
        }
        return new Chunk { InstanceBuffer = b, RelocationBuffer = Pack(own.Select(s => s - (uint)start)), ImportsBuffer = Pack(Slots(source.Chunk.ImportsBuffer).Select(s => checked(s - (uint)start))) };
    }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: auxiliary test readers require one final sentinel and preserve actual allocation order. */
    //-------------------------------------------------------------------------------------------------
    private static uint[] Slots(byte[] bytes)
    {
        if (bytes.Length == 0) return Array.Empty<uint>();
        if (bytes.Length % 4 != 0 || Word(bytes, bytes.Length - 4) != uint.MaxValue) throw new InvalidDataException("Malformed destruction test slots.");
        return Enumerable.Range(0, bytes.Length / 4 - 1).Select(i => Word(bytes, i * 4)).ToArray();
    }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: extraction repacks only native-module slots, excluding the enclosing GameObject envelope. */
    //-------------------------------------------------------------------------------------------------
    private static byte[] Pack(IEnumerable<uint> values) { var slots = values.ToArray(); if (slots.Length == 0) return Array.Empty<byte>(); var b = new byte[(slots.Length + 1) * 4]; for (int i = 0; i < slots.Length; i++) Put(b, i * 4, slots[i]); Put(b, slots.Length * 4, uint.MaxValue); return b; }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: corruptions always own their buffers. */
    //-------------------------------------------------------------------------------------------------
    private static Chunk Clone(Chunk c) => new() { InstanceBuffer = c.InstanceBuffer.ToArray(), RelocationBuffer = c.RelocationBuffer.ToArray(), ImportsBuffer = c.ImportsBuffer.ToArray() };
    //-------------------------------------------------------------------------------------------------
    /** Reborn: unsupported shapes must fail instead of publishing partial tokens. */
    //-------------------------------------------------------------------------------------------------
    private static void Refuse(Action action) { try { action(); } catch (InvalidDataException) { return; } throw new InvalidDataException("Malformed destruction tokenizer input admitted."); }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: test word reads remain inside their own buffers. */
    //-------------------------------------------------------------------------------------------------
    private static uint Word(byte[] b, int at) { if (at < 0 || at > b.Length - 4) throw new InvalidDataException("Destruction writer test bounds exceeded."); return BinaryPrimitives.ReadUInt32LittleEndian(b.AsSpan(at, 4)); }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: detached edits and pointer recovery retain little-endian storage. */
    //-------------------------------------------------------------------------------------------------
    private static void Put(byte[] b, int at, uint value) => BinaryPrimitives.WriteUInt32LittleEndian(b.AsSpan(at, 4), value);
}
