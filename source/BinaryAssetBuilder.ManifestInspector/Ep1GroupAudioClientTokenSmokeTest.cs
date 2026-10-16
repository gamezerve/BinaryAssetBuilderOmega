using System.Buffers.Binary;
using System.Xml.Schema;
using BinaryAssetBuilder.Core.Hashing;
using BinaryAssetBuilder.XmlCompiler;
using Chunk = Relo.Chunk;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: verify a nonempty primitive-list client fragment against independent stock metadata, without claiming audio playback or complete GameObject publication.
internal static class Ep1GroupAudioClientTokenSmokeTest
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: compare complete stock metadata/native bytes, then vary ordered keys, raw frame counters and ushort weight through real root dispatch. */
    //-------------------------------------------------------------------------------------------------
    internal static void Run(string manifest)
    {
        XmlSchemaSet? schemas = null;
        SdkEffectiveSchema.Inspect(shieldCandidate: true, reviewHooks: true, onAdmitted: set => schemas = set);
        if (schemas == null) throw new InvalidDataException("Reviewed client schema required.");
        const string key = "HumanInfantryWithGearThatRuns";
        var native = Compile(Source(new[] { key }), schemas);
        var output = Check(native);
        var stock = GameObjectStockReview.Capture(manifest, "GameObject:SovietAntiInfantryInfantry");
        var module = GameObjectModuleReview.Inspect(manifest, "GameObject:SovietAntiInfantryInfantry").Containers.SelectMany(c => c.Modules).Single(m => m.Type == "LargeGroupAudioClientUpdateModuleData");
        int prefix = 192;
        var expected = stock.Bytes.AsSpan(module.Wrapper, prefix).ToArray();
        int stockItems = checked((int)Word(stock.Bytes, module.Native + 20));
        for (int at = 8; at < prefix; at += 12)
        {
            int target = checked((int)Word(expected, at));
            if (target == 0) continue;
            uint local = target >= module.Wrapper && target < module.Wrapper + prefix ? (uint)(target - module.Wrapper)
                : target >= module.Native && target < module.Native + 28 ? (uint)(prefix + target - module.Native)
                : target == stockItems ? (uint)(prefix + 28) : throw new InvalidDataException("Stock client metadata target escapes reviewed graph.");
            Put(expected, at, local);
        }
        if (!expected.SequenceEqual(output.InstanceBuffer.Take(prefix))) throw new InvalidDataException("Complete independent stock client prefix differs.");
        var stockNative = stock.Bytes.AsSpan(module.Native, 28).ToArray();
        Put(stockNative, 20, 28);
        if (!stockNative.SequenceEqual(native.InstanceBuffer.Take(28)) || Word(stock.Bytes, stockItems) != Word(native.InstanceBuffer, 28)) throw new InvalidDataException("Complete independent stock client native graph differs.");
        int cases = 0;
        foreach (int count in new[] { 1, 2, 32, 64 })
        foreach (var values in new[] { (3u, 2u, (ushort)1), (0u, 0u, (ushort)0), (uint.MaxValue, uint.MaxValue, ushort.MaxValue) })
        {
            var keys = Enumerable.Range(0, count).Select(i => i % 3 == 0 ? key : "RebornKey" + i).ToArray();
            var source = Source(keys, values.Item1, values.Item2, values.Item3);
            var authored = Compile(source, schemas); var tokenized = Check(authored);
            if (Word(authored.InstanceBuffer, 8) != values.Item1 || Word(authored.InstanceBuffer, 12) != values.Item2 || BinaryPrimitives.ReadUInt16LittleEndian(authored.InstanceBuffer.AsSpan(24, 2)) != values.Item3) throw new InvalidDataException("Client raw frame/weight values changed.");
            for (int i = 0; i < keys.Length; i++) if (Word(authored.InstanceBuffer, 28 + i * 4) != FastHash.GetHashCode(keys[i])) throw new InvalidDataException("Client key order/hash changed.");
            var repeated = Check(Compile(source, schemas));
            if (!repeated.InstanceBuffer.SequenceEqual(tokenized.InstanceBuffer) || !repeated.RelocationBuffer.SequenceEqual(tokenized.RelocationBuffer)) throw new InvalidDataException("Client repeat changed output.");
            cases++;
        }
        int nativeFaults = 0, outputFaults = 0, sourceFaults = 0;
        foreach (var change in new[] { (0, 0u), (4, 0u), (16, 0u), (16, 65u), (20, 0u), (20, 24u), (20, 32u), (24, 0x00010001u) })
        { var bad = Clone(native); Put(bad.InstanceBuffer, change.Item1, change.Item2); Refuse(() => Ep1GroupAudioClientTokenizer.Tokenize(bad)); nativeFaults++; }
        foreach (var bytes in new[] { Array.Empty<byte>(), new byte[] { 255, 255, 255, 255 }, Pack(24, uint.MaxValue), Pack(20, 20, uint.MaxValue) })
        { var bad = Clone(native); bad.RelocationBuffer = bytes; Refuse(() => Ep1GroupAudioClientTokenizer.Tokenize(bad)); nativeFaults++; }
        var imported = Clone(native); imported.ImportsBuffer = Pack(8, uint.MaxValue); Refuse(() => Ep1GroupAudioClientTokenizer.Tokenize(imported)); nativeFaults++;
        foreach (int size in new[] { native.InstanceBuffer.Length - 1, native.InstanceBuffer.Length + 4 })
        { var bad = Clone(native); Array.Resize(ref bad.InstanceBuffer, size); Refuse(() => Ep1GroupAudioClientTokenizer.Tokenize(bad)); nativeFaults++; }
        for (int at = 0; at < prefix; at += 4)
        { var bad = Clone(output); Put(bad.InstanceBuffer, at, Word(bad.InstanceBuffer, at) ^ 1u); Refuse(() => Verify(bad, native)); outputFaults++; }
        foreach (int at in new[] { prefix + 8, prefix + 20, prefix + 28 })
        { var bad = Clone(output); Put(bad.InstanceBuffer, at, Word(bad.InstanceBuffer, at) ^ 1u); Refuse(() => Verify(bad, native)); outputFaults++; }
        var aux = Clone(output); aux.RelocationBuffer = Array.Empty<byte>(); Refuse(() => Verify(aux, native)); outputFaults++;
        var imp = Clone(output); imp.ImportsBuffer = Pack(8, uint.MaxValue); Refuse(() => Verify(imp, native)); outputFaults++;
        foreach (string literal in new[] { "-1", "4294967296", "1.5", "3s", "=MissingFrame" })
        { RefuseSource(() => Compile(Source(new[] { key }).Replace("id=", "FramesBetweenUpdatesMin='" + literal + "' id=", StringComparison.Ordinal), schemas)); sourceFaults++; }
        RefuseSource(() => Compile(Source(Array.Empty<string>()), schemas)); sourceFaults++;
        RefuseSource(() => Compile(Source(new[] { key }).Replace("id=", "UnitWeight='65536' id=", StringComparison.Ordinal), schemas)); sourceFaults++;
        Refuse(() => Check(Compile(Source(Enumerable.Repeat(key, 65).ToArray()), schemas))); sourceFaults++;
        Console.WriteLine($"EP1 group-audio client tokens: PASS (one complete independent stock prefix/native graph; {cases} ordered 1/2/32/64-key raw-frame/weight dispatch cases; {nativeFaults} native/{outputFaults} detached/{sourceFaults} source/bound refusals; no shared root publication or audio playback).");
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: validate raw author input, dispatch the actual client module, and extract only its owned native graph and Key relocation. */
    //-------------------------------------------------------------------------------------------------
    private static Chunk Compile(string source, XmlSchemaSet schemas)
    {
        Ep1BasicModuleSmokeTest.Validate(schemas, source, true);
        var compiled = Ep1BasicModuleSmokeTest.Compile(source, "LargeGroupAudioClientUpdateModuleData", true);
        var b = compiled.Chunk.InstanceBuffer.AsSpan(compiled.Native).ToArray();
        uint items = Word(b, 20); if (items != compiled.Native + 28) throw new InvalidDataException("Dispatched client Key ownership differs.");
        Put(b, 20, 28);
        return new Chunk { InstanceBuffer = b, RelocationBuffer = Pack(20, uint.MaxValue), ImportsBuffer = Array.Empty<byte>() };
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: stock default frames are literal frame counts, distinct from GameObject millisecond Duration conversion. */
    //-------------------------------------------------------------------------------------------------
    private static string Source(string[] keys, uint? min = null, uint? variation = null, ushort? weight = null) => "<LargeGroupAudioClientUpdate id='LargeGroupKey'" + (min.HasValue ? $" FramesBetweenUpdatesMin='{min}' FramesBetweenUpdatesVariation='{variation}' UnitWeight='{weight}'" : "") + ">" + string.Concat(keys.Select(k => "<Key>" + k + "</Key>")) + "</LargeGroupAudioClientUpdate>";

    //-------------------------------------------------------------------------------------------------
    /** Reborn: independently verify every metadata word and inverse payload/auxiliary coordinate after generation. */
    //-------------------------------------------------------------------------------------------------
    private static Chunk Check(Chunk native) { var output = Ep1GroupAudioClientTokenizer.Tokenize(native); Verify(output, native); return output; }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: literal stock tags pin the primitive-list/base-chain contract, including zero terminators and direct native element targets. */
    //-------------------------------------------------------------------------------------------------
    internal static void Verify(Chunk output, Chunk native)
    {
        int count = checked((int)Word(native.InstanceBuffer, 16)), prefix = 180 + count * 12;
        if (output.InstanceBuffer.Length != prefix + native.InstanceBuffer.Length) throw new InvalidDataException("Client token extent differs.");
        var expected = new byte[prefix];
        Row(0, 2, 0x9826A297, 12); Row(12, 1, 0x4CAD6EDC, 84);
        Row(24, 0x26F638AB, 0x763177A4, (uint)(prefix + 8)); Row(36, 0xBEB8598D, 0x763177A4, (uint)(prefix + 12)); Row(48, 0x37CE9A7D, 0x43501092, (uint)(prefix + 24)); Row(60, 0xF5301BA7, 0x58BEC727, 168);
        Row(84, 1, 0xA50D729A, 108); Row(108, 1, 0xD75D93A2, 132); Row(132, 0x19B042A0, 0x23E650B8, (uint)prefix); Row(144, 0x8CDC9898, 0xBDF07965, (uint)(prefix + 4)); Row(168, 0x6C827073, 0x5D448FD8, (uint)(prefix + 16));
        for (int i = 0; i < count; i++) Row(180 + i * 12, 0xB151A0EB, 0, (uint)(prefix + 28 + i * 4));
        if (!expected.SequenceEqual(output.InstanceBuffer.Take(prefix)) || !output.RelocationBuffer.SequenceEqual(Pack((uint)(prefix + 20), uint.MaxValue)) || output.ImportsBuffer.Length != 0) throw new InvalidDataException("Detached client metadata/auxiliary differs.");
        var recovered = output.InstanceBuffer.AsSpan(prefix).ToArray();
        if (Word(recovered, 20) != prefix + 28) throw new InvalidDataException("Detached client Key pointer differs.");
        Put(recovered, 20, 28);
        if (!recovered.SequenceEqual(native.InstanceBuffer)) throw new InvalidDataException("Detached client inverse native graph differs.");
        //-------------------------------------------------------------------------------------------------
        /** Reborn: expected metadata never changes the generated fragment under test. */
        //-------------------------------------------------------------------------------------------------
        void Row(int at, uint name, uint type, uint target) { Put(expected, at, name); Put(expected, at + 4, type); Put(expected, at + 8, target); }
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: detached corruptions must fail without producer repair. */
    //-------------------------------------------------------------------------------------------------
    private static void Refuse(Action action) { try { action(); } catch (InvalidDataException) { return; } throw new InvalidDataException("Malformed client graph accepted."); }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: malformed source may fail schema validation or strict literal conversion, but may never yield a successful fragment. */
    //-------------------------------------------------------------------------------------------------
    private static void RefuseSource(Action action) { try { action(); } catch (Exception e) when (e is InvalidDataException or FormatException or OverflowException) { return; } throw new InvalidDataException("Malformed client source accepted."); }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: controls own separate instance and auxiliary buffers. */
    //-------------------------------------------------------------------------------------------------
    private static Chunk Clone(Chunk chunk) => new() { InstanceBuffer = chunk.InstanceBuffer.ToArray(), RelocationBuffer = chunk.RelocationBuffer.ToArray(), ImportsBuffer = chunk.ImportsBuffer.ToArray() };
    //-------------------------------------------------------------------------------------------------
    /** Reborn: auxiliary words preserve their exact order and sentinel. */
    //-------------------------------------------------------------------------------------------------
    private static byte[] Pack(params uint[] words) { var b = new byte[words.Length * 4]; for (int i = 0; i < words.Length; i++) Put(b, i * 4, words[i]); return b; }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: bounded little-endian reads fail as diagnostic errors. */
    //-------------------------------------------------------------------------------------------------
    private static uint Word(byte[] b, int at) { if (at < 0 || at > b.Length - 4) throw new InvalidDataException("Client word bounds exceeded."); return BinaryPrimitives.ReadUInt32LittleEndian(b.AsSpan(at, 4)); }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: writes operate only on owned diagnostic/output buffers. */
    //-------------------------------------------------------------------------------------------------
    private static void Put(byte[] b, int at, uint value) => BinaryPrimitives.WriteUInt32LittleEndian(b.AsSpan(at, 4), value);
}
