using System.Buffers.Binary;
using System.Runtime.InteropServices;
using System.Text;
using System.Xml;
using System.Xml.Schema;
using BinaryAssetBuilder.Core.Hashing;
using Relo;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: verify three new module marshalers against independent stock fields/payloads; no general GameObject tokenizer/package/game authority.
internal static class Ep1BasicModuleSmokeTest
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: batch schema-valid module leaves, actual root dispatch, stock payload equality and edited-field/ID/key-order controls. */
    //-------------------------------------------------------------------------------------------------
    internal static void Run(string manifest)
    {
        var cases = new[] {
            ("GameObject:SovietAntiInfantryInfantry", "ObjectSMCHelperModuleData", "<ObjectSMCHelper id='ObjectSMCHelper'/>", false),
            ("GameObject:AlliedConstructionYard", "ObjectSMCHelperModuleData", "<ObjectSMCHelper id='ObjectSMCHelper'/>", false),
            ("GameObject:AlliedConstructionYard", "StructureSellUpdateModuleData", "<StructureSellUpdate id='ModuleTag_StructureSellUpdate' SellTime='2s' ActiveModelCondition='SOLD'/>", false),
            ("GameObject:SovietAntiInfantryInfantry", "LargeGroupAudioClientUpdateModuleData", "<LargeGroupAudioClientUpdate id='LargeGroupKey'><Key>HumanInfantryWithGearThatRuns</Key></LargeGroupAudioClientUpdate>", true) };
        XmlSchemaSet? schemas = null;
        SdkEffectiveSchema.Inspect(shieldCandidate: true, reviewHooks: true, onAdmitted: set => schemas = set);
        if (schemas == null) throw new InvalidDataException("Reviewed module validation schema required.");
        MixedDispatchControl();
        foreach (var sample in cases)
        {
            var stock = GameObjectStockReview.Capture(manifest, sample.Item1);
            var graph = GameObjectModuleReview.Inspect(manifest, sample.Item1);
            var module = graph.Containers.SelectMany(c => c.Modules).Single(m => m.Type == sample.Item2);
            Validate(schemas, sample.Item3, sample.Item4);
            // Reborn: raw schema validation must reject unknown authored fields instead of treating a successful native marshal as source admission.
            string invalid = sample.Item3.Replace("id='", "Unknown='1' id='", StringComparison.Ordinal);
            bool rejected = false;
            try { Validate(schemas, invalid, sample.Item4); }
            catch (InvalidDataException) { rejected = true; }
            if (!rejected) throw new InvalidDataException("Unknown raw module attribute admitted.");
            var output = Compile(sample.Item3, sample.Item2, sample.Item4);
            var repeat = Compile(sample.Item3, sample.Item2, sample.Item4);
            if (!output.Chunk.InstanceBuffer.SequenceEqual(repeat.Chunk.InstanceBuffer)
                || !output.Chunk.RelocationBuffer.SequenceEqual(repeat.Chunk.RelocationBuffer)) throw new InvalidDataException("Module repeat changed native output.");
            CheckStock(stock.Bytes, module, output.Chunk.InstanceBuffer, output.Native, sample.Item2);
            var edited = Compile(sample.Item3.Replace("id='", "id='Changed", StringComparison.Ordinal), sample.Item2, sample.Item4);
            if (output.Chunk.InstanceBuffer.SequenceEqual(edited.Chunk.InstanceBuffer)) throw new InvalidDataException("Module weak ID edit did not affect output.");
            // Reborn: detached own-field token offsets must not escape the independently anchored concrete payload.
            if (sample.Item2 != "ObjectSMCHelperModuleData")
            {
                var broken = stock.Bytes.ToArray();
                int table = checked((int)Word(broken, module.Wrapper + 8));
                BinaryPrimitives.WriteUInt32LittleEndian(broken.AsSpan(table + 20, 4), (uint)(module.Native + 4));
                try { CheckStock(broken, module, output.Chunk.InstanceBuffer, output.Native, sample.Item2); }
                catch (InvalidDataException) { continue; }
                throw new InvalidDataException("Module own-field offset corruption accepted.");
            }
        }
        var sell = Compile(cases[2].Item3.Replace("2s", "4s", StringComparison.Ordinal), cases[2].Item2, false);
        if (Word(sell.Chunk.InstanceBuffer, sell.Native + 8) != unchecked((uint)BitConverter.SingleToInt32Bits(4))) throw new InvalidDataException("Edited sell time lost.");
        var condition = Compile(cases[2].Item3.Replace("SOLD", "DAMAGED", StringComparison.Ordinal), cases[2].Item2, false);
        if (Word(condition.Chunk.InstanceBuffer, condition.Native + 12) != (uint)SageBinaryData.ModelConditionFlagType.DAMAGED) throw new InvalidDataException("Edited sell condition lost.");
        var audio = Compile("<LargeGroupAudioClientUpdate id='TwoKeys' FramesBetweenUpdatesMin='17' FramesBetweenUpdatesVariation='9' UnitWeight='7'><Key>First</Key><Key>Second</Key></LargeGroupAudioClientUpdate>", cases[3].Item2, true);
        var b = audio.Chunk.InstanceBuffer; int n = audio.Native, keys = checked((int)Word(b, n + 20));
        if (Word(b, n + 8) != 17 || Word(b, n + 12) != 9 || Word(b, n + 16) != 2 || Word(b, n + 24) != 7
            || Word(b, keys) != FastHash.GetHashCode("First") || Word(b, keys + 4) != FastHash.GetHashCode("Second"))
            throw new InvalidDataException("Client audio literal frames/weight/key order differs.");
        // Reborn: the observed frame-counter profile accepts unsigned integer literals, not silently rounded fractional values.
        bool badFrames = false;
        try { Compile(cases[3].Item3.Replace("id='", "FramesBetweenUpdatesMin='1.5' id='", StringComparison.Ordinal), cases[3].Item2, true); }
        catch (FormatException) { badFrames = true; }
        if (!badFrames) throw new InvalidDataException("Fractional client frame count silently accepted.");
        // Reborn: this selected client's dispatcher must reject an unknown class instead of leaving a null entry.
        try { Compile("<Unknown id='Probe'/>", "UnimplementedClientUpdate", true); }
        catch (BinaryAssetBuilderException) { Console.WriteLine("EP1 basic modules: PASS (four stock payload comparisons, real three-class dispatch, scalar/ID/key-order and refusal controls; no package/game)."); return; }
        throw new InvalidDataException("Unknown client class silently compiled.");
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: compile all three new families together and verify independent container/item/key relocations in one owned native graph, not a mixed Core asset stream. */
    //-------------------------------------------------------------------------------------------------
    private static unsafe void MixedDispatchControl()
    {
        var xml = new XmlDocument { XmlResolver = null };
        xml.LoadXml("<GameObject xmlns='uri:ea.com:eala:asset'><Behaviors><ObjectSMCHelper id='SMC'/><StructureSellUpdate id='Sell' SellTime='2s' ActiveModelCondition='SOLD'/></Behaviors><ClientUpdates><LargeGroupAudioClientUpdate id='Audio'><Key>HumanInfantryWithGearThatRuns</Key></LargeGroupAudioClientUpdate></ClientUpdates></GameObject>");
        var ns = new XmlNamespaceManager(xml.NameTable); ns.AddNamespace("ea", "uri:ea.com:eala:asset");
        foreach (XmlElement node in xml.SelectNodes("/ea:GameObject/ea:Behaviors/* | /ea:GameObject/ea:ClientUpdates/*", ns)!)
            node.SetAttribute("TypeId", FastHash.GetHashCode(node.LocalName + "ModuleData").ToString(System.Globalization.CultureInfo.InvariantCulture));
        SageBinaryData.GameObject* root;
        using var tracker = new Tracker((void**)&root, (uint)sizeof(SageBinaryData.GameObject), false);
        Marshaler.Marshal(new Node(xml.DocumentElement!.CreateNavigator()!, ns), root, tracker);
        var chunk = new Chunk(); if (!tracker.MakeRelocatable(chunk)) throw new InvalidDataException("Mixed module allocation failed.");
        var b = chunk.InstanceBuffer;
        int behaviors = checked((int)Word(b, 476)), clients = checked((int)Word(b, 492));
        int p = checked((int)Word(b, behaviors + 4)), q = checked((int)Word(b, clients + 4));
        int smc = checked((int)Word(b, p)), sell = checked((int)Word(b, p + 4)), audio = checked((int)Word(b, q));
        int key = checked((int)Word(b, audio + 20));
        if (b.Length != 684 || Word(b, behaviors) != 2 || Word(b, clients) != 1
            || Word(b, smc) != 0x7F72F242 || Word(b, sell) != 0x4E9CB811 || Word(b, audio) != 0x9826A297
            || Word(b, sell + 8) != unchecked((uint)BitConverter.SingleToInt32Bits(2))
            || Word(b, audio + 8) != 3 || Word(b, audio + 12) != 2 || Word(b, key) != FastHash.GetHashCode("HumanInfantryWithGearThatRuns"))
            throw new InvalidDataException("Mixed module native values/container dimensions differ.");
        var actual = Enumerable.Range(0, chunk.RelocationBuffer.Length / 4).Select(i => Word(chunk.RelocationBuffer, i * 4)).ToArray();
        var expected = new[] { 476u, 492u, (uint)(behaviors + 4), (uint)p, (uint)(p + 4), (uint)(clients + 4), (uint)q, (uint)(audio + 20), uint.MaxValue };
        if (!actual.Order().SequenceEqual(expected.Order()) || actual[^1] != uint.MaxValue || chunk.ImportsBuffer.Length != 0)
            throw new InvalidDataException("Mixed module relocation/import inventory differs.");
        Console.WriteLine("Combined three-family module graph: PASS (684/36/0 native BIN/RELO/IMP; no Core stream/package).");
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: validate the raw leaf in a minimal complete schema container before adding an internal normalized TypeId for the diagnostic dispatch. */
    //-------------------------------------------------------------------------------------------------
    internal static void Validate(XmlSchemaSet schemas, string leaf, bool client)
    {
        string container = client ? "ClientUpdates" : "Behaviors";
        // Reborn: preserve the official GameObject particle order around required Body and optional client updates.
        string body = "<Body><ActiveBody id='ValidationBody' MaxHealth='100'/></Body>";
        string content = "<" + container + ">" + leaf + "</" + container + ">";
        var binding = SdkEffectiveSchema.Bind(schemas, "in-memory-basic-module.xml", Encoding.UTF8.GetBytes(
            "<AssetDeclaration xmlns='uri:ea.com:eala:asset'><GameObject id='ValidationProbe'>" + (client ? body + content : content + body)
            + "</GameObject></AssetDeclaration>"));
        if (!binding.XmlValidated) throw new InvalidDataException("Basic module raw source rejected: " + string.Join("; ", binding.Errors));
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: exercise actual GameObject polymorphic dispatch with internally injected class IDs; the diagnostic root is deliberately not a published complete object. */
    //-------------------------------------------------------------------------------------------------
    private static unsafe (Chunk Chunk, int Native) Compile(string leaf, string type, bool client)
    {
        string container = client ? "ClientUpdates" : "Behaviors";
        var xml = new XmlDocument { XmlResolver = null };
        xml.LoadXml("<GameObject xmlns='uri:ea.com:eala:asset'><" + container + ">" + leaf + "</" + container + "></GameObject>");
        var module = (XmlElement)xml.DocumentElement!.FirstChild!.FirstChild!;
        module.SetAttribute("TypeId", FastHash.GetHashCode(type).ToString(System.Globalization.CultureInfo.InvariantCulture));
        var ns = new XmlNamespaceManager(xml.NameTable); ns.AddNamespace("ea", "uri:ea.com:eala:asset");
        SageBinaryData.GameObject* root;
        using var tracker = new Tracker((void**)&root, (uint)sizeof(SageBinaryData.GameObject), false);
        Marshaler.Marshal(new Node(xml.DocumentElement.CreateNavigator()!, ns), root, tracker);
        var chunk = new Chunk(); if (!tracker.MakeRelocatable(chunk)) throw new InvalidDataException("Basic module native allocation failed.");
        int list = checked((int)Word(chunk.InstanceBuffer, client ? 492 : 476));
        if (Word(chunk.InstanceBuffer, list) != 1) throw new InvalidDataException("Basic module root dispatch list differs.");
        int array = checked((int)Word(chunk.InstanceBuffer, list + 4));
        int native = checked((int)Word(chunk.InstanceBuffer, array));
        if (Word(chunk.InstanceBuffer, native) != FastHash.GetHashCode(type)) throw new InvalidDataException("Basic module dispatch class ID differs.");
        var relocs = Enumerable.Range(0, chunk.RelocationBuffer.Length / 4).Select(i => Word(chunk.RelocationBuffer, i * 4)).ToArray();
        var expected = new[] { (uint)(client ? 492 : 476), (uint)(list + 4), (uint)array };
        if (client) expected = expected.Append((uint)(native + 20)).ToArray();
        if (!relocs.SequenceEqual(expected.Append(uint.MaxValue)) || chunk.ImportsBuffer.Length != 0)
            throw new InvalidDataException("Basic module relocation/import slots differ.");
        return (chunk, native);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: compare exact own-field offsets/type tags and complete selected payloads; rebase only the client key-list pointer, never arbitrary opaque words. */
    //-------------------------------------------------------------------------------------------------
    private static void CheckStock(byte[] stock, GameObjectModuleReview.Module module, byte[] compiled, int native, string type)
    {
        int table = checked((int)Word(stock, module.Wrapper + 8)); var fields = GameObjectStockReview.Tokens(stock, table);
        var definitions = type == "StructureSellUpdateModuleData" ? new[] { ("SellTime", 8, 0x998669E8u), ("ActiveModelCondition", 12, 0x15723E0Bu) } :
            type == "LargeGroupAudioClientUpdateModuleData" ? new[] { ("FramesBetweenUpdatesMin", 8, 0x763177A4u), ("FramesBetweenUpdatesVariation", 12, 0x763177A4u), ("UnitWeight", 24, 0x43501092u) } :
            Array.Empty<(string, int, uint)>();
        int expectedFields = type == "ObjectSMCHelperModuleData" ? 1 : type == "StructureSellUpdateModuleData" ? 3 : 5;
        if (fields.Count != expectedFields || Word(stock, table) != 1) throw new InvalidDataException("Basic module own-field table shape differs.");
        foreach (var f in definitions)
            if (!fields.TryGetValue(FastHash.GetHashCode(f.Item1), out var token) || token.Type != f.Item3 || token.Offset != module.Native + f.Item2)
                throw new InvalidDataException("Basic module field offset/type differs: " + f.Item1);
        int size = type == "ObjectSMCHelperModuleData" ? Marshal.SizeOf<SageBinaryData.ObjectSMCHelperModuleData>() :
            type == "StructureSellUpdateModuleData" ? Marshal.SizeOf<SageBinaryData.StructureSellUpdateModuleData>() : Marshal.SizeOf<SageBinaryData.LargeGroupAudioClientUpdateModuleData>();
        var a = stock.AsSpan(module.Native, size).ToArray(); var b = compiled.AsSpan(native, size).ToArray();
        if (type == "LargeGroupAudioClientUpdateModuleData")
        {
            if (!fields.TryGetValue(FastHash.GetHashCode("Key"), out var key) || key.Type != 0x58BEC727
                || Word(stock, key.Offset) != 0x6C827073 || Word(stock, key.Offset + 4) != 0x5D448FD8 || Word(stock, key.Offset + 8) != module.Native + 16)
                throw new InvalidDataException("Client audio Key list token/native anchor differs.");
            int p = checked((int)Word(a, 20)), q = checked((int)Word(b, 20)), count = checked((int)Word(a, 16));
            if (count != 1 || !stock.AsSpan(p, count * 4).SequenceEqual(compiled.AsSpan(q, count * 4))) throw new InvalidDataException("Client audio key payload differs.");
            BinaryPrimitives.WriteUInt32LittleEndian(a.AsSpan(20, 4), 0);
            BinaryPrimitives.WriteUInt32LittleEndian(b.AsSpan(20, 4), 0);
        }
        if (!a.SequenceEqual(b)) throw new InvalidDataException("Selected complete module native payload differs: " + type);
        Console.WriteLine("Stock module payload matches: " + type + " (" + size + " bytes).");
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: bound all diagnostic word reads to their captured/owned buffer. */
    //-------------------------------------------------------------------------------------------------
    private static uint Word(byte[] b, int at)
    {
        if (at < 0 || at > b.Length - 4) throw new InvalidDataException("Basic module word out of bounds.");
        return BinaryPrimitives.ReadUInt32LittleEndian(b.AsSpan(at, 4));
    }
}
