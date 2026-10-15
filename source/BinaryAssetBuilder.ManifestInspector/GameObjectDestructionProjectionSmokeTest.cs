using System.Buffers.Binary;
using BinaryAssetBuilder.Core.Hashing;
using BinaryAssetBuilder.XmlCompiler;
using Relo;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: projection tests prove root/container/module coordinate ownership without pretending two root tokens are a complete GameObject asset.
internal static class GameObjectDestructionProjectionSmokeTest
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: compare stock root type tags and every nested module token row in the composed global coordinate system, then reject malformed envelopes. */
    //-------------------------------------------------------------------------------------------------
    internal static void Stock(Ep1AlliedModuleSmokeTest.Output source, byte[] stock, GameObjectModuleReview.Module module)
    {
        var result = Check(source.Chunk); var root = GameObjectStockReview.Tokens(stock, 0);
        if (root[FastHash.GetHashCode("Behaviors")].Type != Word(result.InstanceBuffer, 16) || root[FastHash.GetHashCode("BuildTime")].Type != Word(result.InstanceBuffer, 4))
            throw new InvalidDataException("Projection root type tags differ from stock.");
        int prefix = (int)Word(result.InstanceBuffer, 8) - 140, modulePrefix = prefix - 60;
        var expected = stock.AsSpan(module.Wrapper, modulePrefix).ToArray();
        for (int at = 8; at < expected.Length; at += 12)
        {
            uint target = Word(expected, at);
            if (target != 0) Put(expected, at, target >= module.Native ? checked(target - (uint)module.Native + (uint)prefix + 612u) : checked(target - (uint)module.Wrapper + 60u));
        }
        if (!expected.SequenceEqual(result.InstanceBuffer.AsSpan(60, modulePrefix).ToArray())) throw new InvalidDataException("Composed module tokens differ from stock semantic coordinates.");
        foreach (int at in new[] { 476, 600, 604, 608, 612, 680 })
        {
            var bad = Clone(source.Chunk); Put(bad.InstanceBuffer, at, Word(bad.InstanceBuffer, at) ^ 4u);
            Refuse(() => Ep1GameObjectDestructionProjection.Compose(bad));
        }
        var badOwner = Clone(source.Chunk); badOwner.RelocationBuffer = Pack(Slots(badOwner.RelocationBuffer).Append(140u)); Refuse(() => Ep1GameObjectDestructionProjection.Compose(badOwner));
        var cycle = Clone(source.Chunk); Put(cycle.InstanceBuffer, 680, 476); Refuse(() => Ep1GameObjectDestructionProjection.Compose(cycle));
        var badOutput = Clone(result); Put(badOutput.InstanceBuffer, 56, 36); Refuse(() => Verify(badOutput, source.Chunk));
        var edited = Clone(source.Chunk); Put(edited.InstanceBuffer, 140, unchecked((uint)BitConverter.SingleToInt32Bits(12.5f))); Check(edited);
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
    /** Reborn: independently walk two root fields through behavior marker and native pointers, validate authored item targets, then reverse the global shift. */
    //-------------------------------------------------------------------------------------------------
    private static void Verify(Chunk result, Chunk native)
    {
        byte[] b = result.InstanceBuffer; var root = GameObjectStockReview.Tokens(b, 0);
        int prefix = root[FastHash.GetHashCode("BuildTime")].Offset - 140;
        int count = (int)Word(native.InstanceBuffer, 676);
        if (root.Count != 2 || prefix != 372 + count * 72 || root[FastHash.GetHashCode("Behaviors")].Offset != 36 ||
            Word(b, 36) != 0x6C827073u || Word(b, 40) != 0x5D448FD8u || Word(b, 44) != prefix + 600 ||
            Word(b, 48) != 0xB151A0EBu || Word(b, 52) != 0 || Word(b, 56) != 60 ||
            Word(b, prefix + 476) != prefix + 600 || Word(b, prefix + 600) != 1 || Word(b, prefix + 604) != prefix + 608 || Word(b, prefix + 608) != prefix + 612 ||
            Word(b, 60) != 2 || Word(b, 64) != 0x0D96A2F4u || Word(b, 68) != 72 || Word(b, prefix + 612) != Word(b, 64))
            throw new InvalidDataException("Composed root/container/module coordinate chain differs.");
        var own = GameObjectStockReview.Tokens(b, 72); int itemTable = own[FastHash.GetHashCode("Item")].Offset;
        if (Word(b, itemTable + 8) != prefix + 676) throw new InvalidDataException("Composed Item count binding differs.");
        for (int i = 0; i < count; i++)
        {
            int table = (int)Word(b, itemTable + 20 + i * 12); var fields = GameObjectStockReview.Tokens(b, table);
            foreach (var f in new[] { ("BonePrefix", 0), ("FX", 8), ("CreationObject", 12), ("FlingVelocity", 16) })
                if (fields[FastHash.GetHashCode(f.Item1)].Offset != prefix + 684 + i * 20 + f.Item2) throw new InvalidDataException("Composed authored Item field differs.");
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
