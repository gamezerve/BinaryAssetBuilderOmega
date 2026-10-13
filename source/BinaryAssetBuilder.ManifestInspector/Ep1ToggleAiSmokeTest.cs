using System.Buffers.Binary;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Xml;
using BinaryAssetBuilder.Core;
using BinaryAssetBuilder.Core.Hashing;
using BinaryAssetBuilder.Core.SageXml;
using Relo;
using SageBinaryData;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: exercise standalone native AI graphs and real named-reference normalization without admitting an enclosing toggle/GameObject compiler.
internal static class Ep1ToggleAiSmokeTest
{
    internal sealed record Output(Chunk Chunk, InstanceDeclaration Instance);
    //-------------------------------------------------------------------------------------------------
    /** Reborn: compare six complete selected AI graphs, present optional pointers, flags, imports and edits in one offline gate. */
    //-------------------------------------------------------------------------------------------------
    internal static void Run(string manifest)
    {
        Settings saved = Settings.Current;
        try
        {
            string fixture = Path.Combine(Path.GetDirectoryName(ReferencePipelineSmokeTest.FindFixture())!, "ToggleAiPipeline.xsd");
            CheckSchema(fixture);
            Settings.Current = new Settings { SchemaPath = fixture, BuildCache = false, StringHashBinDescriptors = Array.Empty<StringHashBinDescriptor>() };
            var schemas = new SchemaSet(false);
            const string ns = " xmlns='uri:ea.com:eala:asset'";
            var cases = new[] {
                ("SovietAntiInfantryInfantry", new[] { "<SkirmishAiInfo" + ns + " ToggleHint='TOGGLE_DEFAULT'><StateWeapon Weapon='SovietAntiInfantryInfantryAK47'/></SkirmishAiInfo>",
                    "<SkirmishAiInfo" + ns + " ToggleHint='TOGGLE_WEAPON'><StateWeapon Weapon='SovietAntiInfantryInfantryMolotovCocktail'/></SkirmishAiInfo>" }),
                ("SovietMiner", new[] { "<SkirmishAiInfo" + ns + " ToggleHint='TOGGLE_DEFAULT'/>",
                    "<SkirmishAiInfo" + ns + " ToggleHint='TOGGLE_DEFENSE_SELF_ONLY' SelfDefenseDamageLookAheadTime='2s' SelfDefenseDamageLookBehindTime='2s' SelfDefenseDamageFraction='0.2'/>" }),
                ("SovietScoutVehicle", new[] { "<SkirmishAiInfo" + ns + " ToggleHint='TOGGLE_DEFAULT'/>",
                    "<SkirmishAiInfo" + ns + " ToggleHint='TOGGLE_IMMOBILIZE' NeverUseInState='RETREAT' NeverUseDuringTask='REPAIRING' UseOnlyWhenPowerAdvantageIsAbove='1.0'><StateWeapon Weapon='SovietScoutVehicleParalyzeWeapon'/></SkirmishAiInfo>" }) };
            int present = 0;
            foreach (var c in cases)
            {
                var capture = GameObjectStockReview.Capture(manifest, "GameObject:" + c.Item1);
                var proof = ToggleStatusLayoutReview.Inspect(manifest, "GameObject:" + c.Item1);
                for (int i = 0; i < 2; i++)
                {
                    var output = Compile(schemas, c.Item2[i]); Resolve(output.Instance, capture.Document);
                    Compare(capture.Bytes, capture.Asset.References.ToArray(), proof.States[i], output);
                    var repeat = Compile(schemas, c.Item2[i]);
                    if (!output.Chunk.InstanceBuffer.SequenceEqual(repeat.Chunk.InstanceBuffer) || !output.Chunk.RelocationBuffer.SequenceEqual(repeat.Chunk.RelocationBuffer)
                        || !output.Chunk.ImportsBuffer.SequenceEqual(repeat.Chunk.ImportsBuffer)) throw new InvalidDataException("Toggle AI repeat drift.");
                    present += proof.States[i].Optionals.Count(o => o.Target != 0);
                    // Reborn: a present pointer must bind its own token, not any other valid in-slice address.
                    foreach (var optional in proof.States[i].Optionals.Where(o => o.Target != 0))
                    {
                        var broken = capture.Bytes.ToArray(); BinaryPrimitives.WriteUInt32LittleEndian(broken.AsSpan(optional.Slot, 4), (uint)(optional.Target + 4));
                        Refuse(() => Compare(broken, capture.Asset.References.ToArray(), proof.States[i], output));
                    }
                }
            }
            if (present != 4) throw new InvalidDataException("Expected four independently observed present optional joins.");
            string custom = "<SkirmishAiInfo" + ns + " ToggleHint='TOGGLE_SHIELD' UseOnlyWhenPowerAdvantageIsAbove='0' SelfDefenseDamageLookAheadTime='3s' OnlyUseWhenHaveTarget='true' AlwaysUseWhenEnemiesInRange='true' Location='Land' NeverUseInState='RETREAT COMBAT' NeverUseDuringTask='REPAIRING DODGING'><StateWeapon Weapon='SovietScoutVehicleParalyzeWeapon' AoeAttackPowerLocationPreference='PREFER_MAXIMUM_DAMAGE' AoeAttackEmanationLocation='EMANATE_FROM_TARGET' ToggleRangeHysteresis='9' IsDisablingAttack='true'/></SkirmishAiInfo>";
            var edited = Compile(schemas, custom); var b = edited.Chunk.InstanceBuffer; int p = (int)Word(b, 144);
            if (Word(b, 4) == 0 || Word(b, (int)Word(b, 4)) != 0 || Word(b, (int)Word(b, 44)) != Float(3) || Word(b, 0) != 14
                || Word(b, 36) != 6 || Word(b, 40) != 528 || Word(b, 140) != 2 || b[148] != 1 || b[152] != 1
                || Word(b, p + 8) != 2 || Word(b, p + 16) != 1 || Word(b, p + 20) != Float(9) || b[p + 24] != 1)
                throw new InvalidDataException("Toggle AI edited values/masks/bools ignored.");
            // Reborn: exercise all optional allocation slots independently; nine present layouts remain authored diagnostics without stock corroboration.
            foreach (var f in typeof(AIToggleSpecialPowerInfo).GetFields().Where(f => f.FieldType.IsPointer && f.Name != "StateWeapon"))
            {
                string value = f.Name.Contains("ModuleId", StringComparison.Ordinal) ? "ModuleTag_Custom" : f.FieldType.GetElementType() == typeof(Time) ? "4s" : "4";
                var o = Compile(schemas, "<SkirmishAiInfo" + ns + " ToggleHint='TOGGLE_DEFAULT' " + f.Name + "='" + value + "'/>");
                int slot = Marshal.OffsetOf<AIToggleSpecialPowerInfo>(f.Name).ToInt32();
                uint expected = value == "ModuleTag_Custom" ? FastHash.GetHashCode(value.ToLowerInvariant()) : Float(4);
                if (Word(o.Chunk.InstanceBuffer, slot) != 156 || Word(o.Chunk.InstanceBuffer, 156) != expected || o.Chunk.InstanceBuffer.Length != 160)
                    throw new InvalidDataException("Toggle AI optional allocation/value differs: " + f.Name);
            }
            foreach (string bad in new[] { "ToggleHint='UNKNOWN'", "ToggleHint='1'", "ToggleHint='TOGGLE_DEFAULT' NeverUseInState='UNKNOWN'",
                "ToggleHint='TOGGLE_DEFAULT' Location='Space'", "ToggleHint='TOGGLE_DEFAULT' SelfDefenseDamageLookAheadTime='bad'", "" })
                Refuse(() => Compile(schemas, "<SkirmishAiInfo" + ns + " " + bad + "/>"));
            var missing = Compile(schemas, custom.Replace("SovietScoutVehicleParalyzeWeapon", "MissingWeapon", StringComparison.Ordinal));
            Refuse(() => Resolve(missing.Instance, GameObjectStockReview.Capture(manifest, "GameObject:SovietScoutVehicle").Document));
            // Reborn: authored upgrade and weapon imports must retain separate slots/identities, even though this upgrade choice has no stock toggle graph comparison.
            var catalog = GameObjectStockReview.Capture(manifest, "GameObject:SovietScoutVehicle").Document;
            string upgrade = catalog.Assets.First(a => a.TypeId == FastHash.GetHashCode("UpgradeTemplate")).Name.Split(':')[1];
            var twoImports = Compile(schemas, custom.Replace("ToggleHint='TOGGLE_SHIELD'", "ToggleHint='TOGGLE_SHIELD' LeechWeaponUpgrade='" + upgrade + "'", StringComparison.Ordinal));
            Resolve(twoImports.Instance, catalog);
            if (twoImports.Instance.ReferencedInstances.Count != 2 || Word(twoImports.Chunk.InstanceBuffer, 60) != 1 || twoImports.Chunk.ImportsBuffer.Length != 12)
                throw new InvalidDataException("Toggle AI authored upgrade/weapon import separation differs.");
            foreach (string bad in new[] { "UNKNOWN", "1", "TOGGLE_DEFAULT TOGGLE_WEAPON", "" })
                Refuse(() => CompileRaw("<SkirmishAiInfo" + ns + " ToggleHint='" + bad + "'/>"));
            Console.WriteLine("EP1 toggle AI: PASS (six complete selected stock AI graphs, four present optional joins, Core reference identity/imports, defaults/edited masks/bools/all optional allocation controls and refusals; no enclosing toggle dispatch/package/game).");
        }
        finally { Settings.Current = saved; }
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: schema validation and actual Core reference normalization precede standalone native marshalling. */
    //-------------------------------------------------------------------------------------------------
    private static Output Compile(SchemaSet schemas, string xml)
    {
        // Reborn: Core requires an asset ID; the explicitly diagnostic wrapper carries it while its official AI child remains unchanged.
        var instance = ReferencePipelineSmokeTest.Normalize(schemas, "<DiagnosticToggleAi xmlns='uri:ea.com:eala:asset' id='DiagnosticAi'>" + xml + "</DiagnosticToggleAi>");
        return new(CompileRaw(instance.XmlNode.ChildNodes.OfType<XmlElement>().Single().OuterXml), instance);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: compile only the 156-byte AI graph, retaining actual child/import/relocation output rather than a GameObject root. */
    //-------------------------------------------------------------------------------------------------
    private static unsafe Chunk CompileRaw(string xml)
    {
        var doc = new XmlDocument { XmlResolver = null }; doc.LoadXml(xml);
        var ns = new XmlNamespaceManager(doc.NameTable); ns.AddNamespace("ea", "uri:ea.com:eala:asset");
        AIToggleSpecialPowerInfo* root;
        using var tracker = new Tracker((void**)&root, (uint)sizeof(AIToggleSpecialPowerInfo), false);
        Marshaler.Marshal(new Node(doc.DocumentElement!.CreateNavigator()!, ns), root, tracker);
        var chunk = new Chunk(); if (!tracker.MakeRelocatable(chunk)) throw new InvalidDataException("Toggle AI allocation failed.");
        var pointers = typeof(AIToggleSpecialPowerInfo).GetFields().Where(f => f.FieldType.IsPointer)
            .Select(f => Marshal.OffsetOf<AIToggleSpecialPowerInfo>(f.Name).ToInt32()).Where(o => Word(chunk.InstanceBuffer, o) != 0).Order().ToArray();
        var relocs = pointers.Length == 0 ? Array.Empty<uint>() : pointers.Select(p => (uint)p).Append(uint.MaxValue).ToArray();
        if (!relocs.SequenceEqual(Enumerable.Range(0, chunk.RelocationBuffer.Length / 4).Select(i => Word(chunk.RelocationBuffer, i * 4))))
            throw new InvalidDataException("Toggle AI relocation slots differ.");
        int weapon = (int)Word(chunk.InstanceBuffer, 144);
        var imports = new System.Collections.Generic.List<uint>(); if (Word(chunk.InstanceBuffer, 60) != 0) imports.Add(60); if (weapon != 0 && Word(chunk.InstanceBuffer, weapon) != 0) imports.Add((uint)weapon);
        var expectedImports = imports.Count == 0 ? Array.Empty<uint>() : imports.Append(uint.MaxValue).ToArray();
        if (!expectedImports.SequenceEqual(Enumerable.Range(0, chunk.ImportsBuffer.Length / 4).Select(i => Word(chunk.ImportsBuffer, i * 4)))
            || chunk.InstanceBuffer.Length != 156 + (pointers.Length - (weapon == 0 ? 0 : 1)) * 4 + (weapon == 0 ? 0 : 28))
            throw new InvalidDataException("Toggle AI import/allocation dimensions differ.");
        return chunk;
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: compare full selected native records after clearing only proven pointer slots and rebasing strong refs to resolved identities. */
    //-------------------------------------------------------------------------------------------------
    private static void Compare(byte[] stock, AssetId[] refs, ToggleStatusLayoutReview.State state, Output output)
    {
        var native = output.Chunk.InstanceBuffer;
        byte[] left = stock.AsSpan(state.Native + 132, 156).ToArray(), right = native.AsSpan(0, 156).ToArray();
        foreach (var o in state.Optionals)
        {
            int offset = o.Slot - state.Native - 132, p = (int)Word(native, offset);
            if (Word(stock, o.Slot) != o.Target || (o.Target == 0) != (p == 0) || (p != 0 && Word(stock, o.Target) != Word(native, p)))
                throw new InvalidDataException("Toggle AI optional join/value differs: " + o.Name);
            left.AsSpan(offset, 4).Clear(); right.AsSpan(offset, 4).Clear();
        }
        CompareReference(left, 60, right, 60);
        int stockWeapon = (int)Word(stock, state.Native + 276), ownWeapon = (int)Word(native, 144);
        if ((stockWeapon == 0) != (ownWeapon == 0)) throw new InvalidDataException("Toggle AI weapon presence differs.");
        left.AsSpan(144, 4).Clear(); right.AsSpan(144, 4).Clear();
        if (!left.SequenceEqual(right)) throw new InvalidDataException("Toggle AI full native payload differs.");
        if (stockWeapon != 0)
        {
            var l = stock.AsSpan(stockWeapon, 28).ToArray(); var r = native.AsSpan(ownWeapon, 28).ToArray(); CompareReference(l, 0, r, 0);
            if (!l.SequenceEqual(r)) throw new InvalidDataException("Toggle AI weapon native payload differs.");
        }

        //-------------------------------------------------------------------------------------------------
        /** Reborn: resolve one-biased stock/Core indices to the same type/instance identity rather than comparing captured numeric indices. */
        //-------------------------------------------------------------------------------------------------
        void CompareReference(byte[] l, int lo, byte[] r, int ro)
        {
            uint a = Word(l, lo), b = Word(r, ro);
            if ((a == 0) != (b == 0)) throw new InvalidDataException("Toggle AI reference nullability differs.");
            if (a == 0) return;
            if (a > refs.Length || b > output.Instance.ReferencedInstances.Count) throw new InvalidDataException("Toggle AI reference index bounds exceeded.");
            var sa = refs[(int)a - 1]; var sb = output.Instance.ReferencedInstances[(int)b - 1];
            if (sa.TypeId != sb.TypeId || sa.InstanceId != sb.InstanceId) throw new InvalidDataException("Toggle AI reference identity differs.");
            l.AsSpan(lo, 4).Clear(); r.AsSpan(ro, 4).Clear();
        }
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: every plain strong dependency must resolve uniquely in actual linked stock metadata. */
    //-------------------------------------------------------------------------------------------------
    private static void Resolve(InstanceDeclaration instance, ManifestDocument document)
    {
        foreach (var r in instance.ReferencedInstances)
            if (document.Assets.Count(a => a.TypeId == r.TypeId && a.InstanceId == r.InstanceId) != 1) throw new InvalidDataException("Missing/ambiguous toggle AI dependency: " + r.Name);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: copied declaration shapes and all enum orders must agree with official staged schema before focused source testing. */
    //-------------------------------------------------------------------------------------------------
    internal static void CheckSchema(string fixture)
    {
        if (Marshal.SizeOf<AIToggleSpecialPowerInfo>() != 156 || Marshal.SizeOf<AISpecialPowerWeaponData>() != 28) throw new InvalidDataException("Toggle AI model sizes differ.");
        //-------------------------------------------------------------------------------------------------
        /** Reborn: schema inputs are bounded and cannot resolve DTDs or external entities. */
        //-------------------------------------------------------------------------------------------------
        XmlDocument Load(string path)
        {
            var d = new XmlDocument { XmlResolver = null };
            using var s = new MemoryStream(SdkEnvironmentPreflight.Read(path, 2 * 1048576), false);
            using var r = XmlReader.Create(s, new XmlReaderSettings { XmlResolver = null, DtdProcessing = DtdProcessing.Prohibit }); d.Load(r); return d;
        }
        //-------------------------------------------------------------------------------------------------
        /** Reborn: select one explicit declaration rather than admitting arbitrary schema include graphs. */
        //-------------------------------------------------------------------------------------------------
        XmlElement Select(XmlDocument d, string name)
        {
            var ns = new XmlNamespaceManager(d.NameTable); ns.AddNamespace("xs", "http://www.w3.org/2001/XMLSchema");
            return (XmlElement)(d.SelectSingleNode("/xs:schema/*[@name='" + name + "']", ns) ?? throw new InvalidDataException("Missing diagnostic declaration: " + name));
        }
        //-------------------------------------------------------------------------------------------------
        /** Reborn: compare semantic names/attributes/child order independent of indentation and redundant namespace declarations. */
        //-------------------------------------------------------------------------------------------------
        string Shape(XmlElement e)
        {
            return e.NamespaceURI + ":" + e.LocalName + "[" + string.Join("|", e.Attributes.Cast<XmlAttribute>().Where(a => a.NamespaceURI != "http://www.w3.org/2000/xmlns/")
                .OrderBy(a => a.NamespaceURI + ":" + a.LocalName, StringComparer.Ordinal).Select(a => a.NamespaceURI + ":" + a.LocalName + "=" + a.Value)) + "]{" + string.Join("", e.ChildNodes.OfType<XmlElement>().Select(Shape)) + "}";
        }
        var focused = Load(fixture); string root = SdkEnvironmentPreflight.BaselineSchemaRoot();
        var official = Load(Path.Combine(root, "SkirmishAI", "AssetTypeAISpecialPower.xsd"));
        foreach (string name in new[] { "ToggleStatusAiHint", "AIAoeAttackPowerLocationPreference", "AIAoeAttackPowerEmanationLocation", "AISpecialPowerWeaponData", "AIToggleSpecialPowerInfo" })
            if (Shape(Select(focused, name)) != Shape(Select(official, name))) throw new InvalidDataException("Copied toggle AI declaration drift: " + name);
        // Reborn: the minimal location pair is also compared structurally, not admitted merely because its enum names happen to match.
        var location = Load(Path.Combine(root, "SkirmishAI", "AssetTypeTargetHeuristics.xsd"));
        foreach (string name in new[] { "AITargetLocationType", "AITargetLocationBitFlags" })
            if (Shape(Select(focused, name)) != Shape(Select(location, name))) throw new InvalidDataException("Copied toggle location declaration drift: " + name);
        foreach (var item in new[] { (typeof(Ep1ToggleStatusAiHint), official, "ToggleStatusAiHint"),
            (typeof(Ep1AoeAttackPowerLocationPreference), official, "AIAoeAttackPowerLocationPreference"), (typeof(Ep1AoeAttackPowerEmanationLocation), official, "AIAoeAttackPowerEmanationLocation"),
            (typeof(Ep1MicroManagerState), Load(Path.Combine(root, "SkirmishAI", "AssetTypeAIMicroManagerStates.xsd")), "MicroManagerState"),
            (typeof(Ep1MicroManagerUnitTask), Load(Path.Combine(root, "SkirmishAI", "AssetTypeAIMicroManagerStates.xsd")), "MicroManagerUnitTask"),
            (typeof(Ep1AiTargetLocation), Load(Path.Combine(root, "SkirmishAI", "AssetTypeTargetHeuristics.xsd")), "AITargetLocationType") })
        {
            var enums = Select(item.Item2, item.Item3).GetElementsByTagName("enumeration", "http://www.w3.org/2001/XMLSchema").Cast<XmlElement>().Select(e => e.GetAttribute("value"));
            if (!Enum.GetNames(item.Item1).SequenceEqual(enums)) throw new InvalidDataException("Toggle AI enum order drift: " + item.Item3);
        }
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: schema/native/dependency refusals must never report success. */
    //-------------------------------------------------------------------------------------------------
    private static void Refuse(Action action)
    {
        try { action(); } catch (Exception e) when (e is InvalidDataException or System.Xml.Schema.XmlSchemaValidationException or BinaryAssetBuilderException) { return; }
        throw new InvalidDataException("Invalid toggle AI input accepted.");
    }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: use exact independent scalar bits for authored controls. */
    //-------------------------------------------------------------------------------------------------
    private static uint Float(float f) => unchecked((uint)BitConverter.SingleToInt32Bits(f));
    //-------------------------------------------------------------------------------------------------
    /** Reborn: native test reads must remain inside their captured/owned buffer. */
    //-------------------------------------------------------------------------------------------------
    private static uint Word(byte[] b, int at) { if (at < 0 || at > b.Length - 4) throw new InvalidDataException("Toggle AI word bounds exceeded."); return BinaryPrimitives.ReadUInt32LittleEndian(b.AsSpan(at, 4)); }
}
