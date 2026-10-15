using System.Buffers.Binary;
using BinaryAssetBuilder.Core.Hashing;
using BinaryAssetBuilder.XmlCompiler;
using Relo;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: projection tests prove scalar root/container/module ownership without pretending omitted root fields are complete GameObject support.
internal static class GameObjectDestructionProjectionSmokeTest
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: compare stock root type tags and every nested module token row in the composed global coordinate system, then reject malformed envelopes. */
    //-------------------------------------------------------------------------------------------------
    internal static void Stock(Ep1AlliedModuleSmokeTest.Output source, byte[] stock, GameObjectModuleReview.Module module)
    {
        var result = Check(source.Chunk); var root = GameObjectStockReview.Tokens(stock, 0);
        var generatedRoot = GameObjectStockReview.Tokens(result.InstanceBuffer, 0);
        if (root[FastHash.GetHashCode("Behaviors")].Type != generatedRoot[FastHash.GetHashCode("Behaviors")].Type || root[FastHash.GetHashCode("BuildTime")].Type != generatedRoot[FastHash.GetHashCode("BuildTime")].Type)
            throw new InvalidDataException("Projection root type tags differ from stock.");
        int prefix = generatedRoot[FastHash.GetHashCode("BuildTime")].Offset - 140;
        int fragmentStart = generatedRoot[FastHash.GetHashCode("Behaviors")].Offset + 24, modulePrefix = prefix - fragmentStart;
        var expected = stock.AsSpan(module.Wrapper, modulePrefix).ToArray();
        for (int at = 8; at < expected.Length; at += 12)
        {
            uint target = Word(expected, at);
            if (target != 0) Put(expected, at, target >= module.Native ? checked(target - (uint)module.Native + (uint)prefix + 612u) : checked(target - (uint)module.Wrapper + (uint)fragmentStart));
        }
        if (!expected.SequenceEqual(result.InstanceBuffer.AsSpan(fragmentStart, modulePrefix).ToArray())) throw new InvalidDataException("Composed module tokens differ from stock semantic coordinates.");
        foreach (int at in new[] { 476, 600, 604, 608, 612, 680 })
        {
            var bad = Clone(source.Chunk); Put(bad.InstanceBuffer, at, Word(bad.InstanceBuffer, at) ^ 4u);
            Refuse(() => Ep1GameObjectDestructionProjection.Compose(bad));
        }
        var badOwner = Clone(source.Chunk); badOwner.RelocationBuffer = Pack(Slots(badOwner.RelocationBuffer).Append(140u)); Refuse(() => Ep1GameObjectDestructionProjection.Compose(badOwner));
        var cycle = Clone(source.Chunk); Put(cycle.InstanceBuffer, 680, 476); Refuse(() => Ep1GameObjectDestructionProjection.Compose(cycle));
        var badOutput = Clone(result); Put(badOutput.InstanceBuffer, fragmentStart - 4, (uint)(fragmentStart - 24)); Refuse(() => Verify(badOutput, source.Chunk));
        var edited = Clone(source.Chunk); Put(edited.InstanceBuffer, 140, unchecked((uint)BitConverter.SingleToInt32Bits(12.5f))); Check(edited);
        // Reborn: every emitted scalar type and native target must reject independent output metadata corruption, not merely pass a shared descriptor loop.
        for (int i = 0; i < Ep1GameObjectRootValueTokens.AllFields.Count; i++)
            foreach (int word in new[] { 4, 8 })
            {
                var badScalar = Clone(result); int at = i * 12 + word;
                Put(badScalar.InstanceBuffer, at, Word(badScalar.InstanceBuffer, at) ^ 4u);
                Refuse(() => Verify(badScalar, source.Chunk));
            }
        Console.WriteLine($"GameObject root-value metadata corruptions: {Ep1GameObjectRootValueTokens.AllFields.Count * 2} rejected.");
        Console.WriteLine($"GameObject destruction projection: BIN/RELO/IMP={result.InstanceBuffer.Length}/{result.RelocationBuffer.Length}/{result.ImportsBuffer.Length}; root/container/stock module joins, BuildTime edit and nine refusals PASS (partial root, no asset publication).");
    }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: authored item counts and reference/no-reference variants share the root/container composition path. */
    //-------------------------------------------------------------------------------------------------
    internal static void Authored(Ep1AlliedModuleSmokeTest.Output source) => Check(source.Chunk);
    //-------------------------------------------------------------------------------------------------
    /** Reborn: repeated composition must preserve deterministic BIN/RELO/IMP as well as the full native input graph. */
    //-------------------------------------------------------------------------------------------------
    private static Chunk Check(Chunk native)
    {
        var result = Ep1GameObjectDestructionProjection.Compose(native); var repeat = Ep1GameObjectDestructionProjection.Compose(native);
        if (!result.InstanceBuffer.SequenceEqual(repeat.InstanceBuffer) || !result.RelocationBuffer.SequenceEqual(repeat.RelocationBuffer) || !result.ImportsBuffer.SequenceEqual(repeat.ImportsBuffer))
            throw new InvalidDataException("GameObject destruction composition repeat drift.");
        Verify(result, native); return result;
    }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: independently walk admitted root fields through behavior marker/native pointers, validate item targets, then reverse the global shift. */
    //-------------------------------------------------------------------------------------------------
    private static void Verify(Chunk result, Chunk native)
    {
        byte[] b = result.InstanceBuffer; var root = GameObjectStockReview.Tokens(b, 0);
        int prefix = root[FastHash.GetHashCode("BuildTime")].Offset - 140;
        int container = (Ep1GameObjectRootValueTokens.AllFields.Count + 2) * 12, fragmentStart = container + 24;
        // Reborn: decode original native ownership independently; string allocations can move all three behavior envelope addresses.
        int list = (int)Word(native.InstanceBuffer,476), items = (int)Word(native.InstanceBuffer,list+4), module = (int)Word(native.InstanceBuffer,items);
        int count = (int)Word(native.InstanceBuffer, module+64);
        if (root.Count != Ep1GameObjectRootValueTokens.AllFields.Count + 1 || prefix != fragmentStart + 312 + count * 72 || root[FastHash.GetHashCode("Behaviors")].Offset != container ||
            Word(b, container) != 0x6C827073u || Word(b, container + 4) != 0x5D448FD8u || Word(b, container + 8) != prefix + list ||
            Word(b, container + 12) != 0xB151A0EBu || Word(b, container + 16) != 0 || Word(b, container + 20) != fragmentStart ||
            Word(b, prefix + 476) != prefix + list || Word(b, prefix + list) != 1 || Word(b, prefix + list + 4) != prefix + items || Word(b, prefix + items) != prefix + module ||
            Word(b, fragmentStart) != 2 || Word(b, fragmentStart + 4) != 0x0D96A2F4u || Word(b, fragmentStart + 8) != fragmentStart + 12 || Word(b, prefix + module) != Word(b, fragmentStart + 4))
            throw new InvalidDataException("Composed root/container/module coordinate chain differs.");
        // Reborn: every scalar token is independently bound to its emitted native storage, including packed byte/ushort fields.
        foreach (var field in Ep1GameObjectRootValueTokens.AllFields)
            if (!root.TryGetValue(FastHash.GetHashCode(field.Name), out var token) || token.Type != field.Type || token.Offset != prefix + field.Offset)
                throw new InvalidDataException("Composed scalar token differs: " + field.Name);
        var own = GameObjectStockReview.Tokens(b, fragmentStart + 12); int itemTable = own[FastHash.GetHashCode("Item")].Offset;
        if (Word(b, itemTable + 8) != prefix + module + 64) throw new InvalidDataException("Composed Item count binding differs.");
        for (int i = 0; i < count; i++)
        {
            int table = (int)Word(b, itemTable + 20 + i * 12); var fields = GameObjectStockReview.Tokens(b, table);
            foreach (var f in new[] { ("BonePrefix", 0), ("FX", 8), ("CreationObject", 12), ("FlingVelocity", 16) })
                if (fields[FastHash.GetHashCode(f.Item1)].Offset != prefix + module + 72 + i * 20 + f.Item2) throw new InvalidDataException("Composed authored Item field differs.");
        }
        var recovered = b.AsSpan(prefix).ToArray(); var relos = Slots(result.RelocationBuffer); var imports = Slots(result.ImportsBuffer);
        if (!relos.Select(s => checked(s - (uint)prefix)).SequenceEqual(Slots(native.RelocationBuffer)) || !imports.Select(s => checked(s - (uint)prefix)).SequenceEqual(Slots(native.ImportsBuffer)))
            throw new InvalidDataException("Composed root/module auxiliary ownership differs.");
        foreach (uint slot in relos)
        {
            if (slot < prefix || Word(b, (int)slot) < prefix) throw new InvalidDataException("Composed native pointer escapes its region.");
            Put(recovered, (int)slot - prefix, Word(b, (int)slot) - (uint)prefix);
        }
        if (!recovered.SequenceEqual(native.InstanceBuffer)) throw new InvalidDataException("Composed native graph/reference indices changed.");
    }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: test auxiliary streams preserve slot order and explicit sentinel semantics. */
    //-------------------------------------------------------------------------------------------------
    private static uint[] Slots(byte[] b) { if (b.Length == 0) return Array.Empty<uint>(); if (b.Length % 4 != 0 || Word(b, b.Length - 4) != uint.MaxValue) throw new InvalidDataException("Projection test sentinel differs."); return Enumerable.Range(0, b.Length / 4 - 1).Select(i => Word(b, i * 4)).ToArray(); }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: detached ownership faults must not mutate live compiler chunks. */
    //-------------------------------------------------------------------------------------------------
    private static Chunk Clone(Chunk c) => new() { InstanceBuffer = c.InstanceBuffer.ToArray(), RelocationBuffer = c.RelocationBuffer.ToArray(), ImportsBuffer = c.ImportsBuffer.ToArray() };
    //-------------------------------------------------------------------------------------------------
    /** Reborn: construct one explicit extra-slot fixture, not a replacement production stream. */
    //-------------------------------------------------------------------------------------------------
    private static byte[] Pack(IEnumerable<uint> values) { var slots = values.Append(uint.MaxValue).ToArray(); var b = new byte[slots.Length * 4]; for (int i = 0; i < slots.Length; i++) Put(b, i * 4, slots[i]); return b; }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: malformed envelopes/metadata must fail explicitly. */
    //-------------------------------------------------------------------------------------------------
    private static void Refuse(Action action) { try { action(); } catch (InvalidDataException) { return; } throw new InvalidDataException("Malformed projection accepted."); }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: bound all independently decoded projection words. */
    //-------------------------------------------------------------------------------------------------
    private static uint Word(byte[] b, int at) { if (at < 0 || at > b.Length - 4) throw new InvalidDataException("Projection test bounds exceeded."); return BinaryPrimitives.ReadUInt32LittleEndian(b.AsSpan(at, 4)); }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: test edits and inverse rebasing use little-endian storage. */
    //-------------------------------------------------------------------------------------------------
    private static void Put(byte[] b, int at, uint value) => BinaryPrimitives.WriteUInt32LittleEndian(b.AsSpan(at, 4), value);
}
