using System.Buffers.Binary;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Xml;
using BinaryAssetBuilder.Core;
using BinaryAssetBuilder.Core.Hashing;
using BinaryAssetBuilder.Core.SageXml;
using Relo;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: join actual Core-normalized modifier names/imports and nested tracker/freefall native payloads to bounded stock evidence; no GameObject publication.
internal static class Ep1DamageMotionSmokeTest
{
    internal sealed record Output(Chunk Chunk, int Native, InstanceDeclaration Instance);
    //-------------------------------------------------------------------------------------------------
    /** Reborn: compare two tracker variants and one freefall graph, then test optional/default/list/reference/flag/edit boundaries in one batch. */
    //-------------------------------------------------------------------------------------------------
    internal static void Run(string manifest)
    {
        Settings saved = Settings.Current;
        try
        {
            string fixture = Path.Combine(Path.GetDirectoryName(ReferencePipelineSmokeTest.FindFixture())!, "DamageMotionPipeline.xsd");
            CheckStatusSchema(fixture);
            Settings.Current = new Settings { SchemaPath = fixture, BuildCache = false, StringHashBinDescriptors = Array.Empty<StringHashBinDescriptor>() };
            var schemas = new SchemaSet(false);
            string effects = string.Concat(new[] { "02", "04", "06", "08", "10" }.Select((s, i) =>
                "<EffectInfo Fraction='" + ((i + 1) * 0.2f).ToString("R", CultureInfo.InvariantCulture) + "' AttributeModifier='AttributeModifer_CryoBeam_" + s + "'"
                + (i == 4 ? " OnEnterDisable='true' OnEnterCancelSpecialAbilities='true' SetUpgradedArmor='true'" : "") + "/>"));
            const string ns = " xmlns='uri:ea.com:eala:asset'";
            string tracker = "<SecondaryDamageTracker" + ns + " id='ModuleTag_SecondaryDamageTracker' UpdateDuration='1s' DissipateAmount='25' MaxDamageClampScaler='200%'><Color R='1' G='1' B='1.75'/>" + effects + "</SecondaryDamageTracker>";
            string infantry = tracker.Replace("AttributeModifier='AttributeModifer_CryoBeam_10'", "Options='CRASH_IF_AIRBORNE' AttributeModifier='AttributeModifer_CryoBeam_10'", StringComparison.Ordinal);
            string structure = tracker.Replace("</SecondaryDamageTracker>", "<ObjectStatusValidation ForbiddenStatus='UNDER_IRON_CURTAIN'/></SecondaryDamageTracker>", StringComparison.Ordinal);
            string fall = "<FreefallUpdate" + ns + " id='ModuleTag_FreefallUpdate' ModelConditionsToSet='STUNNED_FLAILING' InitialVelocity='5' Acceleration='1' TerminalVelocity='15'/>";
            var cases = new[] { ("GameObject:SovietAntiInfantryInfantry", infantry, "SecondaryDamageTrackerModuleData"),
                ("GameObject:AlliedConstructionYard", structure, "SecondaryDamageTrackerModuleData"), ("GameObject:SovietAntiInfantryInfantry", fall, "FreefallUpdateModuleData") };
            foreach (var c in cases)
            {
                var stock = GameObjectStockReview.Capture(manifest, c.Item1);
                var graph = GameObjectModuleReview.Inspect(manifest, c.Item1);
                var module = graph.Containers.SelectMany(g => g.Modules).Single(m => m.Type == c.Item3);
                var output = Compile(schemas, c.Item2, c.Item3);
                Resolve(output.Instance, stock.Document);
                Compare(stock.Bytes, stock.Asset.References.ToArray(), module, output, c.Item3);
                var repeat = Compile(schemas, c.Item2, c.Item3);
                if (!output.Chunk.InstanceBuffer.SequenceEqual(repeat.Chunk.InstanceBuffer) || !output.Chunk.ImportsBuffer.SequenceEqual(repeat.Chunk.ImportsBuffer))
                    throw new InvalidDataException("Damage/motion repeat drift.");
                var edited = Compile(schemas, c.Item2.Replace("id='", "id='Edited", StringComparison.Ordinal), c.Item3);
                if (output.Chunk.InstanceBuffer.SequenceEqual(edited.Chunk.InstanceBuffer)) throw new InvalidDataException("Module ID edit ignored.");
                // Reborn: own-field offsets must remain anchored to the correct stock concrete payload.
                var broken = stock.Bytes.ToArray(); int table = (int)Word(broken, module.Wrapper + 8);
                BinaryPrimitives.WriteUInt32LittleEndian(broken.AsSpan(table + 20, 4), (uint)(module.Native + 4));
                Refuse(() => Compare(broken, stock.Asset.References.ToArray(), module, output, c.Item3));
            }
            var empty = Compile(schemas, "<SecondaryDamageTracker" + ns + " id='Empty'/>", "SecondaryDamageTrackerModuleData");
            if (Word(empty.Chunk.InstanceBuffer, empty.Native + 8) != Float(1) || Word(empty.Chunk.InstanceBuffer, empty.Native + 16) != Float(1.25f)
                || Word(empty.Chunk.InstanceBuffer, empty.Native + 20) != 0 || Word(empty.Chunk.InstanceBuffer, empty.Native + 24) != 0 || Word(empty.Chunk.InstanceBuffer, empty.Native + 32) != 0)
                throw new InvalidDataException("Tracker optional/default shape differs.");
            string custom = "<SecondaryDamageTracker" + ns + " id='Custom' UpdateDuration='2s' DissipateAmount='7' MaxDamageClampScaler='150%'><EffectInfo Fraction='0.7' Options='DIE_ON_ENTER CRASH_IF_AIRBORNE' OnEnterDisable='true'/></SecondaryDamageTracker>";
            var own = Compile(schemas, custom, "SecondaryDamageTrackerModuleData"); int p = (int)Word(own.Chunk.InstanceBuffer, own.Native + 28);
            if (Word(own.Chunk.InstanceBuffer, own.Native + 8) != Float(2) || Word(own.Chunk.InstanceBuffer, own.Native + 12) != 7 || Word(own.Chunk.InstanceBuffer, own.Native + 16) != Float(1.5f)
                || Word(own.Chunk.InstanceBuffer, p) != Float(0.7f) || Word(own.Chunk.InstanceBuffer, p + 8) != 3 || own.Chunk.InstanceBuffer[p + 12] != 1)
                throw new InvalidDataException("Tracker edited values/options/bools ignored.");
            var moved = Compile(schemas, fall.Replace("InitialVelocity='5'", "InitialVelocity='9'", StringComparison.Ordinal), "FreefallUpdateModuleData");
            if (Word(moved.Chunk.InstanceBuffer, moved.Native + 68) != Float(9)) throw new InvalidDataException("Freefall velocity edit ignored.");
            Refuse(() => Compile(schemas, custom.Replace("DIE_ON_ENTER", "UNKNOWN", StringComparison.Ordinal), "SecondaryDamageTrackerModuleData"));
            Refuse(() => Compile(schemas, custom.Replace("Fraction='0.7'", "", StringComparison.Ordinal), "SecondaryDamageTrackerModuleData"));
            Refuse(() => Compile(schemas, fall.Replace("STUNNED_FLAILING", "UNKNOWN", StringComparison.Ordinal), "FreefallUpdateModuleData"));
            var missing = Compile(schemas, tracker.Replace("AttributeModifer_CryoBeam_02", "MissingModifier", StringComparison.Ordinal), "SecondaryDamageTrackerModuleData");
            Refuse(() => Resolve(missing.Instance, GameObjectStockReview.Capture(manifest, cases[0].Item1).Document));
            Console.WriteLine("EP1 damage/motion: PASS (two complete tracker graphs, freefall, actual Core name/import normalization, defaults/options/edits and refusals; no tokenized GameObject package/game).");
        }
        finally { Settings.Current = saved; }
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: normalize plain modifier references through real Core, then exercise actual polymorphic GameObject dispatch with internally injected class IDs. */
    //-------------------------------------------------------------------------------------------------
    private static unsafe Output Compile(SchemaSet schemas, string source, string type)
    {
        var instance = ReferencePipelineSmokeTest.Normalize(schemas, source);
        var xml = new XmlDocument { XmlResolver = null }; xml.LoadXml("<GameObject xmlns='uri:ea.com:eala:asset'><Behaviors/></GameObject>");
        var leaf = (XmlElement)xml.ImportNode(instance.XmlNode, true); xml.DocumentElement!.FirstChild!.AppendChild(leaf);
        leaf.SetAttribute("TypeId", FastHash.GetHashCode(type).ToString(CultureInfo.InvariantCulture));
        var ns = new XmlNamespaceManager(xml.NameTable); ns.AddNamespace("ea", "uri:ea.com:eala:asset");
        SageBinaryData.GameObject* root;
        using var tracker = new Tracker((void**)&root, (uint)sizeof(SageBinaryData.GameObject), false);
        Marshaler.Marshal(new Node(xml.DocumentElement.CreateNavigator()!, ns), root, tracker);
        var chunk = new Chunk(); if (!tracker.MakeRelocatable(chunk)) throw new InvalidDataException("Damage/motion allocation failed.");
        int list = (int)Word(chunk.InstanceBuffer, 476), items = (int)Word(chunk.InstanceBuffer, list + 4), native = (int)Word(chunk.InstanceBuffer, items);
        if (Word(chunk.InstanceBuffer, list) != 1 || Word(chunk.InstanceBuffer, native) != FastHash.GetHashCode(type)) throw new InvalidDataException("Damage/motion dispatch differs.");
        // Reborn: allocation topology must yield exactly the root/list/item and present child-pointer relocations, never guessed extra slots.
        var expected = new System.Collections.Generic.List<uint> { 476, (uint)(list + 4), (uint)items };
        if (type == "SecondaryDamageTrackerModuleData")
            foreach (int offset in new[] { 20, 28, 32 })
                if (Word(chunk.InstanceBuffer, native + offset) != 0) expected.Add((uint)(native + offset));
        var relocs = Enumerable.Range(0, chunk.RelocationBuffer.Length / 4).Select(i => Word(chunk.RelocationBuffer, i * 4)).ToArray();
        if (!relocs.SequenceEqual(expected.Append(uint.MaxValue))) throw new InvalidDataException("Damage/motion relocation inventory differs.");
        return new(chunk, native, instance);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: every normalized strong target must exist exactly once in the explicit linked stock metadata, not merely be a syntactically valid name. */
    //-------------------------------------------------------------------------------------------------
    private static void Resolve(InstanceDeclaration instance, ManifestDocument doc)
    {
        foreach (var r in instance.ReferencedInstances)
            if (r.TypeName != "AttributeModifier" || doc.Assets.Count(a => a.TypeId == r.TypeId && a.InstanceId == r.InstanceId) != 1)
                throw new InvalidDataException("Missing/ambiguous modifier reference: " + r.Name);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: compare selected full native graphs after rebasing only known pointer/reference words; imports must identify the same real targets. */
    //-------------------------------------------------------------------------------------------------
    private static void Compare(byte[] stock, AssetId[] references, GameObjectModuleReview.Module module, Output output, string type)
    {
        var b = output.Chunk.InstanceBuffer; int native = output.Native; var fields = GameObjectStockReview.Tokens(stock, (int)Word(stock, module.Wrapper + 8));
        var defs = type == "FreefallUpdateModuleData" ? new[] { ("ModelConditionsToSet", 8, 0x67DBBFC9u), ("InitialVelocity", 68, 0x645864F3u), ("Acceleration", 72, 0x645864F3u), ("TerminalVelocity", 76, 0x645864F3u) } :
            new[] { ("UpdateDuration", 8, 0x998669E8u), ("DissipateAmount", 12, 0x763177A4u), ("MaxDamageClampScaler", 16, 0x645864F3u) };
        foreach (var f in defs)
            if (!fields.TryGetValue(FastHash.GetHashCode(f.Item1), out var token) || token.Offset != module.Native + f.Item2 || token.Type != f.Item3) throw new InvalidDataException("Damage/motion field offset/type drift: " + f.Item1);
        if (type == "FreefallUpdateModuleData")
        {
            if (!stock.AsSpan(module.Native, 80).SequenceEqual(b.AsSpan(native, 80)) || output.Chunk.ImportsBuffer.Length != 0) throw new InvalidDataException("Freefall full native payload differs.");
        }
        else
        {
            // Reborn: nested effect markers/field tables must independently bind the same native list and packed records before byte comparison.
            int effectTable = fields[FastHash.GetHashCode("EffectInfo")].Offset;
            int effectCount = (int)Word(stock, module.Native + 24), effectItems = (int)Word(stock, module.Native + 28);
            if (fields.Count != 7 || fields[FastHash.GetHashCode("EffectInfo")].Type != 0x25F5379F
                || Word(stock, effectTable) != 0x6C827073 || Word(stock, effectTable + 4) != 0x5D448FD8 || Word(stock, effectTable + 8) != module.Native + 24)
                throw new InvalidDataException("Tracker effect list token anchor differs.");
            for (int i = 0; i < effectCount; i++)
            {
                int marker = effectTable + 12 + i * 12, member = effectItems + i * 16;
                if (Word(stock, marker) != 0xB151A0EB || Word(stock, marker + 4) != 0) throw new InvalidDataException("Tracker effect marker differs.");
                var tokens = GameObjectStockReview.Tokens(stock, (int)Word(stock, marker + 8));
                var attrs = new[] { ("Fraction", 0, 0x645864F3u), ("AttributeModifier", 4, 0x09DDCFF3u), ("Options", 8, 0xA2F37A52u),
                    ("OnEnterDisable", 12, 0x77E3F4F7u), ("OnEnterCancelSpecialAbilities", 13, 0x77E3F4F7u), ("SetUpgradedArmor", 14, 0x77E3F4F7u) };
                if (tokens.Count != 6 || attrs.Any(f => !tokens.TryGetValue(FastHash.GetHashCode(f.Item1), out var t) || t.Type != f.Item3 || t.Offset != member + f.Item2))
                    throw new InvalidDataException("Effect field token/native layout differs.");
            }
            var a = stock.AsSpan(module.Native, 36).ToArray(); var c = b.AsSpan(native, 36).ToArray();
            foreach (int offset in new[] { 20, 28, 32 })
            {
                int p = (int)Word(a, offset), q = (int)Word(c, offset);
                if ((p == 0) != (q == 0)) throw new InvalidDataException("Tracker optional pointer shape differs.");
                int size = offset == 20 ? 12 : offset == 32 ? Marshal.SizeOf<SageBinaryData.ObjectStatusValidationDataType>() : (int)Word(a, 24) * 16;
                if (p != 0)
                {
                    var left = stock.AsSpan(p, size).ToArray(); var right = b.AsSpan(q, size).ToArray();
                    if (offset == 28)
                    {
                        for (int i = 0; i < size / 16; i++)
                        {
                            uint x = Word(left, i * 16 + 4), y = Word(right, i * 16 + 4);
                            if (x == 0 || x > references.Length || y == 0 || y > output.Instance.ReferencedInstances.Count) throw new InvalidDataException("Effect import index invalid.");
                            var actual = output.Instance.ReferencedInstances[(int)y - 1]; var expected = references[(int)x - 1];
                            if (actual.TypeId != expected.TypeId || actual.InstanceId != expected.InstanceId) throw new InvalidDataException("Effect import selects different target.");
                            BinaryPrimitives.WriteUInt32LittleEndian(left.AsSpan(i * 16 + 4, 4), actual.InstanceId);
                            BinaryPrimitives.WriteUInt32LittleEndian(right.AsSpan(i * 16 + 4, 4), actual.InstanceId);
                        }
                    }
                    if (!left.SequenceEqual(right)) throw new InvalidDataException("Tracker nested native payload differs.");
                }
                BinaryPrimitives.WriteUInt32LittleEndian(a.AsSpan(offset, 4), 0); BinaryPrimitives.WriteUInt32LittleEndian(c.AsSpan(offset, 4), 0);
            }
            if (!a.SequenceEqual(c)) throw new InvalidDataException("Tracker root scalar/identity/count payload differs.");
            var imports = Enumerable.Range(0, output.Chunk.ImportsBuffer.Length / 4).Select(i => Word(output.Chunk.ImportsBuffer, i * 4)).ToArray();
            int effects = (int)Word(b, native + 28), count = (int)Word(b, native + 24);
            if (!imports.SequenceEqual(Enumerable.Range(0, count).Select(i => (uint)(effects + i * 16 + 4)).Append(uint.MaxValue))) throw new InvalidDataException("Tracker native import slots differ.");
        }
        Console.WriteLine("Full selected stock module graph matches: " + type + " (" + output.Chunk.InstanceBuffer.Length + "/" + output.Chunk.RelocationBuffer.Length + "/" + output.Chunk.ImportsBuffer.Length + ").");
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: copied status declarations must retain the exact official name/type inventory; base stubs remain explicitly diagnostic. */
    //-------------------------------------------------------------------------------------------------
    private static void CheckStatusSchema(string fixture)
    {
        foreach (string path in new[] { fixture, Path.Combine(SdkEnvironmentPreflight.BaselineSchemaRoot(), "Modules", "BaseModules.xsd") })
        {
            var doc = new XmlDocument { XmlResolver = null };
            using var input = new MemoryStream(SdkEnvironmentPreflight.Read(path, 2 * 1048576), false);
            using var reader = XmlReader.Create(input, new XmlReaderSettings { DtdProcessing = DtdProcessing.Prohibit, XmlResolver = null }); doc.Load(reader);
            var ns = new XmlNamespaceManager(doc.NameTable); ns.AddNamespace("xs", "http://www.w3.org/2001/XMLSchema");
            var node = doc.SelectSingleNode("/xs:schema/xs:complexType[@name='ObjectStatusValidationDataType']", ns)!;
            var attrs = node.SelectNodes("xs:attribute", ns)!.Cast<XmlElement>().Select(e => e.GetAttribute("name") + ":" + e.GetAttribute("type")).ToArray();
            if (!attrs.SequenceEqual(new[] { "ForbiddenStatus:ObjectStatusBitFlags", "RequiredStatus:ObjectStatusBitFlags" })) throw new InvalidDataException("Status schema drift.");
        }
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: expected schema/native/lookup refusals cannot return a partial diagnostic success. */
    //-------------------------------------------------------------------------------------------------
    private static void Refuse(Action action)
    {
        try { action(); } catch (Exception e) when (e is InvalidDataException or System.Xml.Schema.XmlSchemaValidationException) { return; }
        throw new InvalidDataException("Damage/motion unsupported input accepted.");
    }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: retain exact literal float bits for independent field controls. */
    //-------------------------------------------------------------------------------------------------
    private static uint Float(float f) => unchecked((uint)BitConverter.SingleToInt32Bits(f));
    //-------------------------------------------------------------------------------------------------
    /** Reborn: bound all diagnostic native word reads. */
    //-------------------------------------------------------------------------------------------------
    private static uint Word(byte[] b, int at) { if (at < 0 || at > b.Length - 4) throw new InvalidDataException("Damage/motion word bounds exceeded."); return BinaryPrimitives.ReadUInt32LittleEndian(b.AsSpan(at, 4)); }
}
