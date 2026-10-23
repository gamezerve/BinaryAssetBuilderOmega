using System.Buffers.Binary;
using System.Runtime.InteropServices;
using System.Text;
using System.Xml;
using System.Xml.Schema;
using BinaryAssetBuilder.Core.Hashing;
using BinaryAssetBuilder.Core;
using Relo;
using SageBinaryData;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: compare three new families/four stock instances through real native dispatch; captured FX indices and callback execution remain explicit limits.
internal static class Ep1AlliedModuleSmokeTest
{
    internal sealed record Output(Chunk Chunk, int Native);
    //-------------------------------------------------------------------------------------------------
    /** Reborn: batch schema/native/base/child checks, repeat/edit/default controls and invalid source/pointer refusals without launching the game. */
    //-------------------------------------------------------------------------------------------------
    internal static void Run(string manifest)
    {
        XmlSchemaSet? schemas = null;
        SdkEffectiveSchema.Inspect(shieldCandidate: true, reviewHooks: true, onAdmitted: s => schemas = s);
        if (schemas == null) throw new InvalidDataException("Reviewed schema candidate required.");
        // Reborn: scalar root families are audited/compiled as one batch, not promoted from native-name coverage alone.
        GameObjectScalarTokenSmokeTest.Run(schemas, manifest);
        // Reborn: EP1 enum/flag/weak-reference root families share one stock/source/metadata batch gate.
        GameObjectRootValueSmokeTest.Run(schemas, manifest);
        // Reborn: explicit root strings move allocation ownership and must survive the same generated projection path.
        GameObjectRootStringSmokeTest.Run(schemas,manifest);
        // Reborn: reviewed strong root slots must coexist with module imports without index renumbering or ownership underflow.
        GameObjectRootStrongSmokeTest.Run(schemas,manifest);
        var stock = GameObjectStockReview.Capture(manifest, "GameObject:AlliedConstructionYard");
        var graph = GameObjectModuleReview.Inspect(manifest, "GameObject:AlliedConstructionYard");
        var cases = new[] {
            ("LUAEventListBehaviorModuleData", "ModuleTag_LUAEventList", "<LUAEventList id='ModuleTag_LUAEventList' EventListName='AlliedConstructionYardFunctions'/>", 16),
            ("BuildingDestructionModuleData", "ModuleTag_BuildingDestruction", "<BuildingDestructionBehavior id='ModuleTag_BuildingDestruction'><DieMuxData DeathTypes='ALL'/><Item BonePrefix='BONE_CONTACT_POINT_' FX='SovietRefineryHit' CreationObject='AlliedBarracks_Debris' FlingVelocity='100.0'/></BuildingDestructionBehavior>", 72),
            ("TechUpgradeModuleData", "ModuleTag_TechUpgrade_Tech3", Tech("3"), 176), ("TechUpgradeModuleData", "ModuleTag_TechUpgrade_Tech2", Tech("2"), 176) };
        foreach (var c in cases)
        {
            var module = graph.Containers.SelectMany(g => g.Modules).Single(m => m.Type == c.Item1 && Word(stock.Bytes, m.Native + 4) == FastHash.GetHashCode(c.Item2.ToLowerInvariant()));
            Ep1BasicModuleSmokeTest.Validate(schemas, c.Item3, false);
            string source = c.Item3;
            if (c.Item1 == "BuildingDestructionModuleData")
            {
                int item = (int)Word(stock.Bytes, module.Native + 68); uint index = Word(stock.Bytes, item + 8);
                if (index == 0 || index > stock.Asset.References.Count) throw new InvalidDataException("Destruction FX index bounds exceeded.");
                var reference = stock.Asset.References[(int)index - 1];
                if (reference.TypeId != FastHash.GetHashCode("FXList") || reference.InstanceId != FastHash.GetHashCode("sovietrefineryhit")) throw new InvalidDataException("Destruction source FX identity differs.");
                source = source.Replace("FX='SovietRefineryHit'", "FX='CapturedDiagnostic\\" + (index - 1) + "'", StringComparison.Ordinal);
            }
            var output = Compile(source, c.Item1); Compare(stock.Bytes, module, output, c.Item4);
            var repeat = Compile(source, c.Item1);
            if (!output.Chunk.InstanceBuffer.SequenceEqual(repeat.Chunk.InstanceBuffer) || !output.Chunk.RelocationBuffer.SequenceEqual(repeat.Chunk.RelocationBuffer) || !output.Chunk.ImportsBuffer.SequenceEqual(repeat.Chunk.ImportsBuffer))
                throw new InvalidDataException("Allied module repeat drift.");
            if (output.Chunk.InstanceBuffer.SequenceEqual(Compile(source.Replace("id='", "id='Edited", StringComparison.Ordinal), c.Item1).Chunk.InstanceBuffer)) throw new InvalidDataException("Allied module ID edit ignored.");
            Refuse(() => Ep1BasicModuleSmokeTest.Validate(schemas, c.Item3.Replace("id='", "Unknown='1' id='", StringComparison.Ordinal), false));
            var broken = stock.Bytes.ToArray(); int table = (int)Word(broken, module.Wrapper + 8);
            BinaryPrimitives.WriteUInt32LittleEndian(broken.AsSpan(table + 20, 4), (uint)(module.Native + 4));
            Refuse(() => Compare(broken, module, output, c.Item4));

            // Reborn: corrupt only a detached stock copy's BonePrefix token target; the native item remains unchanged and comparison must refuse it.
            if (c.Item1 == "BuildingDestructionModuleData")
            {
                var itemBroken = stock.Bytes.ToArray();

                var ownFields = GameObjectStockReview.Tokens(
                    itemBroken, (int)Word(itemBroken, module.Wrapper + 8));

                int itemTable = ownFields[FastHash.GetHashCode("Item")].Offset;
                int fieldTable = (int)Word(itemBroken, itemTable + 20);

                if (Word(itemBroken, fieldTable) != FastHash.GetHashCode("BonePrefix"))
                {
                    throw new InvalidDataException(
                        "Destruction corruption fixture expected BonePrefix first.");
                }

                uint originalTarget = Word(itemBroken, fieldTable + 8);

                BinaryPrimitives.WriteUInt32LittleEndian(
                    itemBroken.AsSpan(fieldTable + 8, 4),
                    originalTarget + 4);

                Refuse(() => Compare(itemBroken, module, output, c.Item4));

                Console.WriteLine("Destruction Item token corruption: rejected.");
                // Reborn: independently corrupt every admitted base/inline token type, pointer and selected empty-storage obligation on fresh memory copies.
                var baseSlots = DestructionBaseTokenReview.Inspect(stock.Bytes, module);
                foreach (int slot in baseSlots)
                {
                    var baseBroken = stock.Bytes.ToArray();
                    BinaryPrimitives.WriteUInt32LittleEndian(baseBroken.AsSpan(slot, 4), Word(baseBroken, slot) ^ 4u);
                    Refuse(() => Compare(baseBroken, module, output, c.Item4));
                }
                // Reborn: a self-cycle and a token link into native storage must fail even though their values remain within the captured asset.
                int concreteTable = (int)Word(stock.Bytes, module.Wrapper + 8);
                foreach (uint target in new[] { (uint)concreteTable, (uint)module.Native })
                {
                    var badLink = stock.Bytes.ToArray();
                    BinaryPrimitives.WriteUInt32LittleEndian(badLink.AsSpan(concreteTable + 8, 4), target);
                    Refuse(() => Compare(badLink, module, output, c.Item4));
                }
                Console.WriteLine($"Destruction inheritance/DieMux token corruptions: {baseSlots.Length + 2} rejected.");
            }

            Console.WriteLine($"Allied native {c.Item2}: BIN/RELO/IMP={output.Chunk.InstanceBuffer.Length}/{output.Chunk.RelocationBuffer.Length}/{output.Chunk.ImportsBuffer.Length}");
        }
        var own = Compile("<TechUpgrade id='Custom' Radius='20' RadiusY='0' DistanceType='CIRCULAR' FXLineBoneName='new bone'/>", "TechUpgradeModuleData");
        int n = own.Native; var b = own.Chunk.InstanceBuffer;
        if (Word(b, n + 36) != Float(20) || Word(b, n + 40) == 0 || Word(b, (int)Word(b, n + 40)) != 0 || Word(b, n + 44) != 0 || Text(b, n + 48) != "new bone") throw new InvalidDataException("Tech edited/optional zero values differ.");
        var lua = Compile("<LUAEventList id='CustomLua' EventListName='NewFunctions'/>", "LUAEventListBehaviorModuleData");
        if (Text(lua.Chunk.InstanceBuffer, lua.Native + 8) != "NewFunctions") throw new InvalidDataException("LUA event-list edit lost.");
        // Reborn: authored destruction controls independently exercise ordered multi-items and omitted official defaults, not just the stock singleton.
        const string destruction = "<BuildingDestructionBehavior id='CustomDestruction'><DieMuxData/><Item BonePrefix='first'/><Item BonePrefix='second' FlingVelocity='7'/></BuildingDestructionBehavior>";
        Ep1BasicModuleSmokeTest.Validate(schemas, destruction, false);
        var authored = Compile(destruction, "BuildingDestructionModuleData");
        // Reborn: authored two-item fragments with absent FX imports use the same generated-token path.
        DestructionTokenWriterSmokeTest.Authored(authored);
        // Reborn: null strings and absent imports remain valid beyond the former 256-item experiment quota.
        const string emptyBoneStart = "<BuildingDestructionBehavior id='WriterBoundary'><DieMuxData/>";
        const string emptyBoneEnd = "</BuildingDestructionBehavior>";
        foreach (int count in new[] { 1, 256, 257 })
            DestructionTokenWriterSmokeTest.Authored(Compile(emptyBoneStart + string.Concat(Enumerable.Repeat("<Item/>", count)) + emptyBoneEnd, "BuildingDestructionModuleData"));
        var db = authored.Chunk.InstanceBuffer; int dn = authored.Native, di = (int)Word(db, dn + 68);
        if (Word(db, dn + 56) != Float(2) || Word(db, dn + 60) != FastHash.GetHashCode("newskin_fill") || Word(db, dn + 64) != 2 ||
            Text(db, di) != "first" || Text(db, di + 20) != "second" || Word(db, di + 16) != Float(1) || Word(db, di + 36) != Float(7) ||
            Word(db, di + 8) != 0 || Word(db, di + 12) != 0 || authored.Chunk.ImportsBuffer.Length != 0)
            throw new InvalidDataException("Authored destruction order/default/absent reference differs.");
        Slots(authored.Chunk.RelocationBuffer, new System.Collections.Generic.List<uint> { 476, 604, 608, (uint)(dn + 68), (uint)(di + 4), (uint)(di + 24) });
        Refuse(() => Compile("<TechUpgrade id='Invalid' DistanceType='UNKNOWN'/>", "TechUpgradeModuleData"));
        Refuse(() => Compile("<LUAEventList id='Invalid'/>", "LUAEventListBehaviorModuleData"));
        Refuse(() => Compile("<BuildingDestructionBehavior id='Invalid'/>", "BuildingDestructionModuleData"));
        Console.WriteLine("EP1 allied modules: PASS (three families/four complete selected stock graphs, real dispatch, schema/ID/optional/default/edit controls; FX uses captured-index diagnostic, no generic source closure/tokenized GameObject/package/LUA execution).");
    }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: two stock tech variants differ by their source upgrade identity, not a copied numeric weak ID. */
    //-------------------------------------------------------------------------------------------------
    private static string Tech(string level) => "<TechUpgrade id='ModuleTag_TechUpgrade_Tech" + level + "' Radius='600.0' DistanceType='RECTANGULAR' FXLineBoneName='fx_bone_upgrade_dish'><TriggeredBy>Upgrade_AlliedTech" + level + "</TriggeredBy><ObjectFilter Rule='ANY' Relationship='ALLIES'><IncludeThing>AlliedAirfield</IncludeThing><IncludeThing>AlliedBarracks</IncludeThing><IncludeThing>AlliedNavalYard</IncludeThing><IncludeThing>AlliedWarFactory</IncludeThing></ObjectFilter></TechUpgrade>";
    //-------------------------------------------------------------------------------------------------
    /** Reborn: native class selection is injected internally after schema validation, then passed through actual polymorphic GameObject behavior dispatch. */
    //-------------------------------------------------------------------------------------------------
    internal static unsafe Output Compile(string source, string type)
    {
        var d = new XmlDocument { XmlResolver = null }; d.LoadXml("<GameObject xmlns='uri:ea.com:eala:asset'><Behaviors>" + source + "</Behaviors></GameObject>");
        ((XmlElement)d.DocumentElement!.FirstChild!.FirstChild!).SetAttribute("TypeId", FastHash.GetHashCode(type).ToString(System.Globalization.CultureInfo.InvariantCulture));
        var ns = new XmlNamespaceManager(d.NameTable); ns.AddNamespace("ea", "uri:ea.com:eala:asset");
        GameObject* root; using var tracker = new Tracker((void**)&root, (uint)sizeof(GameObject), false);
        Marshaler.Marshal(new Node(d.DocumentElement.CreateNavigator()!, ns), root, tracker);
        var chunk = new Chunk(); if (!tracker.MakeRelocatable(chunk)) throw new InvalidDataException("Allied module allocation failed.");
        int list = (int)Word(chunk.InstanceBuffer, 476), items = (int)Word(chunk.InstanceBuffer, list + 4), native = (int)Word(chunk.InstanceBuffer, items);
        if (Word(chunk.InstanceBuffer, list) != 1 || Word(chunk.InstanceBuffer, native) != FastHash.GetHashCode(type)) throw new InvalidDataException("Allied module dispatch differs.");
        return new(chunk, native);
    }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: compare full selected native roots/child graphs after joining own-field tokens and rebasing only admitted string/list pointers. */
    //-------------------------------------------------------------------------------------------------
    internal static void Compare(byte[] stock, GameObjectModuleReview.Module module, Output o, int size, AssetId[]? references = null, InstanceDeclaration? instance = null)
    {
        var b = o.Chunk.InstanceBuffer; int n = o.Native;
        var fields = GameObjectStockReview.Tokens(stock, (int)Word(stock, module.Wrapper + 8));
        var defs = module.Type == "LUAEventListBehaviorModuleData" ? new[] { ("EventListName", 8) } : module.Type == "BuildingDestructionModuleData" ?
            new[] { ("AssociatedModule", 52), ("MaxDebrisCloudsPerSecond", 56), ("FillerMesh", 60) } : new[] { ("Radius", 36), ("DistanceType", 44), ("FXLineBoneName", 48) };
        foreach (var f in defs) if (fields[FastHash.GetHashCode(f.Item1)].Offset != module.Native + f.Item2) throw new InvalidDataException("Allied own token/native anchor differs: " + f.Item1);
        var left = stock.AsSpan(module.Native, size).ToArray(); var right = b.AsSpan(n, size).ToArray();
        var relocs = new System.Collections.Generic.List<uint> { 476, 604, 608 }; var imports = new System.Collections.Generic.List<uint>();
        if (module.Type == "LUAEventListBehaviorModuleData") String(8);
        else if (module.Type == "BuildingDestructionModuleData")
        {
            // Reborn: equal native bytes cannot substitute for concrete/base/inline token graph validation.
            DestructionBaseTokenReview.Inspect(stock, module);
            // Reborn: locate stock and compiled destruction items before validating their token/native bindings.
            int a = List(64, 20, "Item"), c = (int)Word(b, n + 68);

            // Reborn: require the stock Item marker and all four named field tokens to bind their verified native offsets and schema types.
            int itemTokenTable = fields[FastHash.GetHashCode("Item")].Offset;

            if (Word(stock, itemTokenTable + 12) != 0xB151A0EBu ||
                Word(stock, itemTokenTable + 16) != 0)
            {
                throw new InvalidDataException("Destruction Item token marker differs.");
            }

            int itemFieldTable = (int)Word(stock, itemTokenTable + 20);
            var itemFields = GameObjectStockReview.Tokens(stock, itemFieldTable);

            foreach (var field in new (string Name, int Offset, uint Type)[]
            {
                ("BonePrefix", 0, 0x1A2933AAu),
                ("FX", 8, 0x0D1C8E83u),
                ("CreationObject", 12, 0x148ED75Du),
                ("FlingVelocity", 16, 0x645864F3u)
            })
            {
                if (!itemFields.TryGetValue(FastHash.GetHashCode(field.Name), out var token) ||
                    token.Type != field.Type ||
                    token.Offset != a + field.Offset)
                {
                    throw new InvalidDataException(
                        $"Destruction Item token/native anchor differs: {field.Name}.");
                }
            }

            var l = stock.AsSpan(a, 20).ToArray(); var r = b.AsSpan(c, 20).ToArray();
            if (Text(stock, a) != Text(b, c)) throw new InvalidDataException("Destruction bone string differs.");
            l.AsSpan(4, 4).Clear(); r.AsSpan(4, 4).Clear(); relocs.Add((uint)(c + 4));
            // Reborn: source-normalized dependency indices may differ; rebase only after the actual type/instance identity agrees with stock.
            if (instance != null)
            {
                uint x = Word(stock, a + 8), y = Word(b, c + 8);
                if (references == null || x == 0 || x > references.Length || y == 0 || y > instance.ReferencedInstances.Count)
                    throw new InvalidDataException("Destruction source import index bounds differ.");
                var expected = references[(int)x - 1]; var actual = instance.ReferencedInstances[(int)y - 1];
                if (expected.TypeId != actual.TypeId || expected.InstanceId != actual.InstanceId)
                    throw new InvalidDataException("Destruction source import identity differs.");
                l.AsSpan(8, 4).Clear(); r.AsSpan(8, 4).Clear();
            }
            if (!l.SequenceEqual(r)) throw new InvalidDataException("Destruction complete item differs."); imports.Add((uint)(c + 8));
        }
        else
        {
            if (fields[FastHash.GetHashCode("RadiusY")].Offset != 0 || Word(stock, module.Native + 40) != 0 || Word(b, n + 40) != 0) throw new InvalidDataException("Stock optional RadiusY differs.");
            String(48);
            int a = List(12, 4, null), c = (int)Word(b, n + 16);
            if (!stock.AsSpan(a, 4).SequenceEqual(b.AsSpan(c, 4))) throw new InvalidDataException("Tech upgrade weak ID differs.");
            a = List(160, 4, null); c = (int)Word(b, n + 164);
            if (!stock.AsSpan(a, 16).SequenceEqual(b.AsSpan(c, 16))) throw new InvalidDataException("Tech filter weak identities differ.");
        }
        if (!left.SequenceEqual(right)) throw new InvalidDataException("Allied complete root/base/inline graph differs: " + string.Join(",", Enumerable.Range(0,size).Where(i => left[i]!=right[i]).Take(10).Select(i=>$"+{i}:{left[i]:X2}/{right[i]:X2}")));
        Slots(o.Chunk.RelocationBuffer, relocs); Slots(o.Chunk.ImportsBuffer, imports);

        //-------------------------------------------------------------------------------------------------
        /** Reborn: native strings require equal text/length/terminator before pointer rebasing. */
        //-------------------------------------------------------------------------------------------------
        void String(int offset)
        {
            if (Text(stock, module.Native + offset) != Text(b, n + offset)) throw new InvalidDataException("Allied string differs.");
            left.AsSpan(offset + 4, 4).Clear(); right.AsSpan(offset + 4, 4).Clear(); relocs.Add((uint)(n + offset + 4));
        }
        //-------------------------------------------------------------------------------------------------
        /** Reborn: selected list dimensions and admitted native targets must agree before erasing only their owning pointer words. */
        //-------------------------------------------------------------------------------------------------
        int List(int offset, int width, string? ownName)
        {
            int count = (int)Word(stock, module.Native + offset), a = (int)Word(stock, module.Native + offset + 4), c = (int)Word(b, n + offset + 4);
            if (count != Word(b, n + offset) || count != (offset == 160 ? 4 : 1) || a < 0 || a > stock.Length - count * width || c < 0 || c > b.Length - count * width)
                throw new InvalidDataException("Allied list count/target differs.");
            if (ownName != null)
            {
                int t = fields[FastHash.GetHashCode(ownName)].Offset;
                if (Word(stock,t)!=0x6C827073 || Word(stock,t+4)!=0x5D448FD8 || Word(stock,t+8)!=module.Native+offset) throw new InvalidDataException("Allied list token count anchor differs.");
            }
            left.AsSpan(offset + 4, 4).Clear(); right.AsSpan(offset + 4, 4).Clear(); relocs.Add((uint)(n + offset + 4)); return a;
        }
    }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: pointer/import streams must contain exactly the expected slots plus sentinel, never unexamined extras. */
    //-------------------------------------------------------------------------------------------------
    private static void Slots(byte[] bytes, System.Collections.Generic.List<uint> slots)
    {
        var values = Enumerable.Range(0, bytes.Length/4).Select(i=>Word(bytes,i*4)).ToArray();
        if (slots.Count == 0 ? values.Length != 0 : values.Length != slots.Count+1 || values[^1]!=uint.MaxValue || !values[..^1].Order().SequenceEqual(slots.Order())) throw new InvalidDataException("Allied auxiliary slots differ.");
    }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: bounded native strings retain length and null terminator even when their data addresses differ. */
    //-------------------------------------------------------------------------------------------------
    private static string Text(byte[] b, int at) { int len=(int)Word(b,at),p=(int)Word(b,at+4); if(len<0 || p<=0 || p>b.Length-len-1 || b[p+len]!=0) throw new InvalidDataException("Allied string bounds differ."); return Encoding.ASCII.GetString(b,p,len); }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: invalid source/native controls must not report successful compilation. */
    //-------------------------------------------------------------------------------------------------
    private static void Refuse(Action action) { try{action();}catch(Exception e)when(e is InvalidDataException or BinaryAssetBuilderException){return;} throw new InvalidDataException("Invalid Allied module input admitted."); }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: independent literal bit patterns avoid deriving expected scalar values from compiler output. */
    //-------------------------------------------------------------------------------------------------
    private static uint Float(float f)=>unchecked((uint)BitConverter.SingleToInt32Bits(f));
    //-------------------------------------------------------------------------------------------------
    /** Reborn: every native test word stays inside its owned/captured buffer. */
    //-------------------------------------------------------------------------------------------------
    private static uint Word(byte[] b,int at){if(at<0 || at>b.Length-4)throw new InvalidDataException("Allied word bounds exceeded."); return BinaryPrimitives.ReadUInt32LittleEndian(b.AsSpan(at,4));}
}
