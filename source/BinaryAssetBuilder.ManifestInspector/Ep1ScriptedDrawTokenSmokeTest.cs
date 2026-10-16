using System.Buffers.Binary;
using BinaryAssetBuilder.XmlCompiler;
using Chunk = Relo.Chunk;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: the complete independent stock prefixes, not the writer's descriptors, are the metadata oracle for both full Conscript scripted graphs.
internal static class Ep1ScriptedDrawTokenSmokeTest
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: normalize only independently compared stock native coordinates and in-prefix table addresses; all metadata words and inverse native/auxiliary bytes must agree. */
    //-------------------------------------------------------------------------------------------------
    internal static byte[] Stock(Chunk native, byte[] stock, int wrapper, int prefix, Dictionary<int, int> nativeOffsets, bool controls)
    {
        var expected = stock.AsSpan(wrapper, prefix).ToArray();
        for (int at = 8; at < prefix; at += 12)
        {
            uint target = Word(expected, at); if (target == 0) continue;
            uint rebased = target >= wrapper && target < wrapper + prefix ? target - (uint)wrapper
                : nativeOffsets.TryGetValue((int)target, out int actual) ? checked((uint)(prefix + actual))
                : throw new InvalidDataException("Stock scripted metadata target escapes the independently compared graph.");
            Put(expected, at, rebased);
        }
        var output = Ep1ScriptedDrawTokenizer.Tokenize(native); Verify(output, native, prefix, expected);
        if (controls) Controls(native, output, prefix, expected);
        Console.WriteLine($"Scripted metadata: complete independent {prefix}-byte stock prefix PASS; native/auxiliary inverse PASS."); return expected;
    }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: callers may verify edited full source graphs against the same stock-proven topology without regenerating the expected table through the producer. */
    //-------------------------------------------------------------------------------------------------
    internal static void Verify(Chunk output, Chunk native, int prefix, byte[] expected)
    {
        if (expected.Length != prefix || output.InstanceBuffer.Length != prefix + native.InstanceBuffer.Length || !output.InstanceBuffer.AsSpan(0, prefix).SequenceEqual(expected)) throw new InvalidDataException("Detached complete scripted metadata differs.");
        var relos = Slots(native.RelocationBuffer); var imports = Slots(native.ImportsBuffer);
        if (!Slots(output.RelocationBuffer).SequenceEqual(relos.Select(s => s + (uint)prefix)) || !Slots(output.ImportsBuffer).SequenceEqual(imports.Select(s => s + (uint)prefix))) throw new InvalidDataException("Detached scripted auxiliary origin/order differs.");
        var recovered = output.InstanceBuffer.AsSpan(prefix).ToArray();
        foreach (uint slot in relos) { uint value = Word(recovered, (int)slot); if (value < prefix || value != Word(native.InstanceBuffer, (int)slot) + (uint)prefix) throw new InvalidDataException("Detached scripted pointer target differs."); Put(recovered, (int)slot, value - (uint)prefix); }
        if (!recovered.SequenceEqual(native.InstanceBuffer)) throw new InvalidDataException("Detached scripted inverse native payload differs.");
    }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: mutate every detached metadata word and representative payload/auxiliary words; malformed native owners, enums, flags, strings and unreviewed children must refuse before publication. */
    //-------------------------------------------------------------------------------------------------
    private static void Controls(Chunk native, Chunk output, int prefix, byte[] expected)
    {
        int detached = 0, inputs = 0;
        for (int at = 0; at < prefix; at += 4)
        { var bad = new Chunk { InstanceBuffer = output.InstanceBuffer.ToArray(), RelocationBuffer = output.RelocationBuffer, ImportsBuffer = output.ImportsBuffer }; Put(bad.InstanceBuffer, at, Word(bad.InstanceBuffer, at) ^ 1u); Refuse(() => Verify(bad, native, prefix, expected)); detached++; }
        foreach (int at in new[] { 0, 4, 16, 168, native.InstanceBuffer.Length - 4 })
        { var bad = Clone(output); Put(bad.InstanceBuffer, prefix + at, Word(bad.InstanceBuffer, prefix + at) ^ 1u); Refuse(() => Verify(bad, native, prefix, expected)); detached++; }
        foreach (bool relo in new[] { false, true }) { var bad = Clone(output); if (relo) bad.RelocationBuffer = Array.Empty<byte>(); else bad.ImportsBuffer = Array.Empty<byte>(); Refuse(() => Verify(bad, native, prefix, expected)); detached++; }
        foreach (var change in new[] { (0, 0u), (4, 0u), (16, 0x7FC00000u), (32, 2u), (36, 64u), (116, 0x80000000u), (164, 4097u), (168, 16u), (172, 1u), (176, 216u), (180, 1u), (188, 1u), (196, 2u), (212, 0x10000u) })
        { var bad = Clone(native); Put(bad.InstanceBuffer, change.Item1, change.Item2); Refuse(() => Ep1ScriptedDrawTokenizer.Tokenize(bad)); inputs++; }
        int states = (int)Word(native.InstanceBuffer, 168), animations = (int)Word(native.InstanceBuffer, 176);
        foreach (var change in new[] { (states, 3u), (states + 112, 216u), (states + 164, 1u), (states + 172, 216u), (states + 176, 1u), (states + 184, 1u), (animations + 88, 4096u), (animations + 108, 16u), (animations + 112, 1u), (animations + 120, 1u) })
        { var bad = Clone(native); Put(bad.InstanceBuffer, change.Item1, change.Item2); Refuse(() => Ep1ScriptedDrawTokenizer.Tokenize(bad)); inputs++; }
        foreach (bool relo in new[] { false, true }) { var bad = Clone(native); if (relo) bad.RelocationBuffer = Array.Empty<byte>(); else bad.ImportsBuffer = Array.Empty<byte>(); Refuse(() => Ep1ScriptedDrawTokenizer.Tokenize(bad)); inputs++; }
        var duplicated = Clone(native); duplicated.RelocationBuffer = Pack(Slots(native.RelocationBuffer).Append(Slots(native.RelocationBuffer)[0])); Refuse(() => Ep1ScriptedDrawTokenizer.Tokenize(duplicated)); inputs++;
        var tail = Clone(native); tail.InstanceBuffer = tail.InstanceBuffer.Concat(new byte[4]).ToArray(); Refuse(() => Ep1ScriptedDrawTokenizer.Tokenize(tail)); inputs++;
        // Reborn: real script headers exercise malformed string pointers/lengths/bytes rather than a known-absent optional field only.
        int script = Enumerable.Range(0, (int)Word(native.InstanceBuffer, 172)).Select(i => (int)Word(native.InstanceBuffer, animations + i * 140 + 108)).First(p => p != 0), text = (int)Word(native.InstanceBuffer, script + 4), length = (int)Word(native.InstanceBuffer, script);
        foreach (var change in new[] { (script, 65537u), (script + 4, 16u) }) { var bad = Clone(native); Put(bad.InstanceBuffer, change.Item1, change.Item2); Refuse(() => Ep1ScriptedDrawTokenizer.Tokenize(bad)); inputs++; }
        foreach (var change in new[] { (text, (byte)128), (text, (byte)0), (text + length, (byte)1) }) { var bad = Clone(native); bad.InstanceBuffer[change.Item1] = change.Item2; Refuse(() => Ep1ScriptedDrawTokenizer.Tokenize(bad)); inputs++; }
        Console.WriteLine($"Scripted metadata refusals: {inputs} native/{detached} detached; every metadata word checked.");
    }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: failed controls cannot be converted to a successful but incomplete output. */
    //-------------------------------------------------------------------------------------------------
    private static void Refuse(Action action) { try { action(); } catch (InvalidDataException) { return; } throw new InvalidDataException("Malformed scripted metadata/native graph accepted."); }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: mutation controls own their buffers. */
    //-------------------------------------------------------------------------------------------------
    private static Chunk Clone(Chunk chunk) => new() { InstanceBuffer = chunk.InstanceBuffer.ToArray(), RelocationBuffer = chunk.RelocationBuffer.ToArray(), ImportsBuffer = chunk.ImportsBuffer.ToArray() };
    //-------------------------------------------------------------------------------------------------
    /** Reborn: nonempty auxiliary streams require one sentinel and no duplicated slots. */
    //-------------------------------------------------------------------------------------------------
    private static uint[] Slots(byte[] bytes) { if (bytes.Length == 0) return Array.Empty<uint>(); if (bytes.Length % 4 != 0 || Word(bytes, bytes.Length - 4) != uint.MaxValue) throw new InvalidDataException("Detached scripted auxiliary sentinel differs."); var result = Enumerable.Range(0, bytes.Length / 4 - 1).Select(i => Word(bytes, i * 4)).ToArray(); if (result.Distinct().Count() != result.Length) throw new InvalidDataException("Detached duplicate scripted auxiliary slot."); return result; }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: auxiliary mutations preserve the real stream encoding, including an explicit final sentinel. */
    //-------------------------------------------------------------------------------------------------
    private static byte[] Pack(IEnumerable<uint> slots) { var values = slots.ToArray(); var bytes = new byte[(values.Length + 1) * 4]; for (int i = 0; i < values.Length; i++) Put(bytes, i * 4, values[i]); Put(bytes, values.Length * 4, uint.MaxValue); return bytes; }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: all metadata/native reads stay bounded to owned byte arrays. */
    //-------------------------------------------------------------------------------------------------
    private static uint Word(byte[] bytes, int at) { if (at < 0 || at > bytes.Length - 4) throw new InvalidDataException("Scripted metadata read out of bounds."); return BinaryPrimitives.ReadUInt32LittleEndian(bytes.AsSpan(at, 4)); }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: controls only edit newly owned output/native buffers. */
    //-------------------------------------------------------------------------------------------------
    private static void Put(byte[] bytes, int at, uint value) => BinaryPrimitives.WriteUInt32LittleEndian(bytes.AsSpan(at, 4), value);
}
