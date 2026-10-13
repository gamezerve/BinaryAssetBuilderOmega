using System.Buffers.Binary;
using System.Runtime.InteropServices;
using System.Xml;
using BinaryAssetBuilder.Core.Hashing;
using Relo;
using SageBinaryData;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: compare selected complete native toggle graphs using explicitly captured reference indices; source/Core normalization and dispatch remain separate gates.
internal static class Ep1ToggleModuleSmokeTest
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: test the enclosing special-ability base, ordered states, nested AI graphs and authored nonempty lists in one bounded offline gate. */
    //-------------------------------------------------------------------------------------------------
    internal static void Run(string manifest)
    {
        if (Marshal.SizeOf<ToggleStatusNugget>() != 304 || Marshal.SizeOf<ToggleStatusSpecialAbilityUpdateModuleData>() != 264 || Marshal.SizeOf<SpecialPowerModuleInfo>() != 12)
            throw new InvalidDataException("Toggle native model sizes differ.");
        const string ns = " xmlns='uri:ea.com:eala:asset'";
        var sources = Cases();
        foreach (var c in sources)
        {
            var capture = GameObjectStockReview.Capture(manifest, "GameObject:" + c.Item1);
            var proof = ToggleStatusLayoutReview.Inspect(manifest, "GameObject:" + c.Item1);
            string diagnostic = CapturedInput(c.Item2, capture.Bytes, proof, capture.Asset.References.ToArray());
            var chunk = Compile(diagnostic); Compare(capture.Bytes, proof, chunk);
            var repeat = Compile(diagnostic);
            if (!chunk.InstanceBuffer.SequenceEqual(repeat.InstanceBuffer) || !chunk.ImportsBuffer.SequenceEqual(repeat.ImportsBuffer) || !chunk.RelocationBuffer.SequenceEqual(repeat.RelocationBuffer))
                throw new InvalidDataException("Toggle module repeat drift.");
            var edited = Compile(diagnostic.Replace("id='", "id='Edited", StringComparison.Ordinal));
            // Reborn: XmlDocument serialization uses double quotes; edit the actual normalized spelling if the literal replacement did nothing.
            if (chunk.InstanceBuffer.SequenceEqual(edited.InstanceBuffer)) edited = Compile(diagnostic.Replace("id=\"", "id=\"Edited", StringComparison.Ordinal));
            if (chunk.InstanceBuffer.SequenceEqual(edited.InstanceBuffer)) throw new InvalidDataException("Toggle module ID edit ignored.");
            var broken = capture.Bytes.ToArray(); BinaryPrimitives.WriteUInt32LittleEndian(broken.AsSpan(proof.Native + 256, 4), 4);
            Refuse(() => Compare(broken, proof, chunk));
            Console.WriteLine($"Toggle selected native {c.Item1}: BIN/RELO/IMP={chunk.InstanceBuffer.Length}/{chunk.RelocationBuffer.Length}/{chunk.ImportsBuffer.Length} (captured-index diagnostic, not source publication)");
        }
        string own = "<ToggleStatusSpecialAbilityUpdate" + ns + " id='Custom' ReloadAllWeaponsOnToggle='false' ForceIdleAfterToggle='true'><ToggleState LocomotorSet='EXITING_PRODUCTION_STRUCTURE' ArmorSet='HERO' AIUpdateModuleId='CustomAi' GenericLuaEventParameter='event payload'><SkirmishAiInfo ToggleHint='TOGGLE_ALWAYS'/><Module ModuleId='Drain' TriggerType='TARGET_OBJECT' TriggerAtTargetPosition='false'/><ActiveGeometry>ship geom</ActiveGeometry><ActiveGeometry>air_geom</ActiveGeometry></ToggleState></ToggleStatusSpecialAbilityUpdate>";
        var authored = Compile(own); int state = (int)Word(authored.InstanceBuffer, 256), modules = (int)Word(authored.InstanceBuffer, state + 292), geometry = (int)Word(authored.InstanceBuffer, state + 300);
        var b = authored.InstanceBuffer;
        if (b[260] != 0 || b[261] != 1 || Word(b, state + 100) != 3 || Word(b, state + 104) != 17 || Word(b, state + 108) != FastHash.GetHashCode("customai")
            || Word(b, state + 288) != 1 || Word(b, state + 296) != 2 || Word(b, modules) != FastHash.GetHashCode("drain") || Word(b, modules + 4) != 2 || b[modules + 8] != 0
            || Text(b, state + 112) != "event payload" || Text(b, geometry) != "ship geom" || Text(b, geometry + 8) != "air_geom")
            throw new InvalidDataException("Toggle authored list/string/enum/bool controls differ.");
        Refuse(() => Compile("<ToggleStatusSpecialAbilityUpdate" + ns + " id='Empty'/>"));
        Refuse(() => Compile(own.Replace("<SkirmishAiInfo ToggleHint='TOGGLE_ALWAYS'/>", "", StringComparison.Ordinal)));
        Refuse(() => Compile(own.Replace("LocomotorSet='EXITING_PRODUCTION_STRUCTURE'", "LocomotorSet='COMBO2'", StringComparison.Ordinal)));
        Refuse(() => Compile(own.Replace("<Module ModuleId='Drain' TriggerType='TARGET_OBJECT' TriggerAtTargetPosition='false'/>", string.Concat(Enumerable.Repeat("<Module ModuleId='TooMany'/>", 5)), StringComparison.Ordinal)));
        Console.WriteLine("EP1 toggle module: PASS (three complete selected native graphs/base, captured-index reference diagnostics, repeats/edits, authored Module/geometry/string lists and refusals; no full source normalization, polymorphic dispatch, tokenized package or game).");
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: share reviewed literal stock module fixtures between captured-index and actual-Core normalization gates. */
    //-------------------------------------------------------------------------------------------------
    internal static (string Name, string Xml)[] Cases()
    {
        const string ns = " xmlns='uri:ea.com:eala:asset'";
        return new[] {
            ("SovietAntiInfantryInfantry", "<ToggleStatusSpecialAbilityUpdate" + ns + " id='ModuleTag_ToggleMolotovCocktail' SpecialPowerTemplate='SpecialPower_MolotovCocktail' Options='RECONSTITUTE_STORED_COMMAND' PackTime='1s'><ToggleState><SkirmishAiInfo ToggleHint='TOGGLE_DEFAULT'><StateWeapon Weapon='SovietAntiInfantryInfantryAK47'/></SkirmishAiInfo></ToggleState><ToggleState ObjectStatus='GENERIC_TOGGLE_STATE ANTI_GARRISON' ModelConditions='USER_5' EnterStateSound='SOV_Conscript_ArmMolotovCocktailMS'><SkirmishAiInfo ToggleHint='TOGGLE_WEAPON'><StateWeapon Weapon='SovietAntiInfantryInfantryMolotovCocktail'/></SkirmishAiInfo></ToggleState></ToggleStatusSpecialAbilityUpdate>"),
            ("SovietMiner", "<ToggleStatusSpecialAbilityUpdate" + ns + " id='ModuleTag_ToggleArmorUpUpdate' Options='RECONSTITUTE_STORED_COMMAND' SpecialPowerTemplate='SpecialPower_ToggleArmorUp'><ToggleState><SkirmishAiInfo ToggleHint='TOGGLE_DEFAULT'/></ToggleState><ToggleState AttributeModifier='AttributeModifier_ArmorUp' ObjectStatus='GENERIC_TOGGLE_STATE SPECIAL_ARMOR_ACTIVE' ModelConditions='WEAPONSTATE_ONE' EnterStateSound='SOV_OreCollector_ReactiveArmorOn'><SkirmishAiInfo ToggleHint='TOGGLE_DEFENSE_SELF_ONLY' SelfDefenseDamageLookAheadTime='2s' SelfDefenseDamageLookBehindTime='2s' SelfDefenseDamageFraction='0.2'/></ToggleState></ToggleStatusSpecialAbilityUpdate>"),
            ("SovietScoutVehicle", "<ToggleStatusSpecialAbilityUpdate" + ns + " id='ModuleTag_ToggleBinaryWeaponsTriggerUpdate' Options='RECONSTITUTE_STORED_COMMAND' SpecialPowerTemplate='SpecialPower_ToggleBinaryWeapons'><ToggleState ObjectStatus='WEAPON_UPGRADED_01' EnterStateSound='SOV_TerrorDrone_WeaponToggleMS'><SkirmishAiInfo ToggleHint='TOGGLE_DEFAULT'/></ToggleState><ToggleState ObjectStatus='WEAPON_UPGRADED_02' AttributeModifier='AttributeModifier_TerrorDroneVisionBonus' EnterStateSound='SOV_TerrorDrone_WeaponToggleMS' ModelConditions='USER_4'><SkirmishAiInfo ToggleHint='TOGGLE_IMMOBILIZE' NeverUseInState='RETREAT' NeverUseDuringTask='REPAIRING' UseOnlyWhenPowerAdvantageIsAbove='1.0'><StateWeapon Weapon='SovietScoutVehicleParalyzeWeapon'/></SkirmishAiInfo></ToggleState></ToggleStatusSpecialAbilityUpdate>") };
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: preserve a literal source identity-to-stock dependency check while explicitly substituting captured indices only for native diagnostic input. */
    //-------------------------------------------------------------------------------------------------
    private static string CapturedInput(string source, byte[] stock, ToggleStatusLayoutReview.Proof proof, AssetId[] refs)
    {
        var d = new XmlDocument { XmlResolver = null }; d.LoadXml(source);
        Bind(d.DocumentElement!, "SpecialPowerTemplate", "SpecialPowerTemplate", proof.Native + 8);
        var states = d.DocumentElement!.ChildNodes.OfType<XmlElement>().Where(e => e.LocalName == "ToggleState").ToArray();
        for (int i = 0; i < states.Length; i++)
        {
            int native = proof.States[i].Native;
            Bind(states[i], "AttributeModifier", "AttributeModifier", native);
            Bind(states[i], "EnterStateSound", "BaseAudioEventInfo", native + 120);
            var weapon = states[i].GetElementsByTagName("StateWeapon").OfType<XmlElement>().SingleOrDefault();
            if (weapon != null) Bind(weapon, "Weapon", "WeaponTemplate", (int)Word(stock, native + 276));
        }
        return d.OuterXml;

        //-------------------------------------------------------------------------------------------------
        /** Reborn: repeated dependencies may have multiple indices; the actual token-anchored native slot selects the occurrence and must match source name/type. */
        //-------------------------------------------------------------------------------------------------
        void Bind(XmlElement e, string field, string typeName, int at)
        {
            if (!e.HasAttribute(field)) return;
            uint index = Word(stock, at);
            if (index == 0 || index > refs.Length) throw new InvalidDataException("Captured toggle reference index out of bounds.");
            string name = e.GetAttribute(field); var reference = refs[(int)index - 1];
            // Reborn: the reviewed EP1 audio base admits actual AudioEvent/Multisound identities, not the legacy empty AudioEventInfo marker hash.
            bool typeMatches = typeName == "BaseAudioEventInfo" ? reference.TypeId is 0x844D7B9Fu or 0xA3A7AF37u : reference.TypeId == FastHash.GetHashCode(typeName);
            if (reference.InstanceId != FastHash.GetHashCode(name.ToLowerInvariant()) || !typeMatches)
                throw new InvalidDataException($"Selected source reference does not match its stock dependency slot: {name} type={reference.TypeId:X8} id={reference.InstanceId:X8} expected={FastHash.GetHashCode(typeName):X8}/{FastHash.GetHashCode(name.ToLowerInvariant()):X8}");
            e.SetAttribute(field, "CapturedDiagnostic\\" + (index - 1));
        }
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: compile a direct concrete module diagnostic without changing the production GameObject behavior dispatch table. */
    //-------------------------------------------------------------------------------------------------
    internal static unsafe Chunk Compile(string source)
    {
        var d = new XmlDocument { XmlResolver = null }; d.LoadXml(source);
        var ns = new XmlNamespaceManager(d.NameTable); ns.AddNamespace("ea", "uri:ea.com:eala:asset");
        ToggleStatusSpecialAbilityUpdateModuleData* root;
        using var tracker = new Tracker((void**)&root, (uint)sizeof(ToggleStatusSpecialAbilityUpdateModuleData), false);
        Marshaler.Marshal(new Node(d.DocumentElement!.CreateNavigator()!, ns), root, tracker);
        // Reborn: direct diagnostics inject the reviewed concrete ID; normal polymorphic ID/dispatch is a later independent gate.
        *(uint*)root = FastHash.GetHashCode(nameof(ToggleStatusSpecialAbilityUpdateModuleData));
        var chunk = new Chunk(); if (!tracker.MakeRelocatable(chunk)) throw new InvalidDataException("Toggle module allocation failed."); return chunk;
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: independently join every selected native pointer/reference slot, then compare complete root/state/AI/weapon payload bytes. */
    //-------------------------------------------------------------------------------------------------
    internal static void Compare(byte[] stock, ToggleStatusLayoutReview.Proof proof, Chunk chunk, AssetId[]? stockRefs = null, BinaryAssetBuilder.Core.InstanceDeclaration? instance = null)
    {
        var b = chunk.InstanceBuffer; int items = (int)Word(b, 256);
        if (Word(stock, proof.Native + 256) != proof.States[0].Native || Word(b, 252) != 2 || Word(b, 0) != FastHash.GetHashCode(nameof(ToggleStatusSpecialAbilityUpdateModuleData)))
            throw new InvalidDataException("Toggle root state list/concrete ID differs.");
        byte[] left = stock.AsSpan(proof.Native, 264).ToArray(), right = b.AsSpan(0, 264).ToArray(); left.AsSpan(256, 4).Clear(); right.AsSpan(256, 4).Clear();
        // Reborn: compare actual Core dependency identities in the source gate; preserve captured numeric comparison for the earlier raw diagnostic.
        if (instance != null) Identity(left, right, 8);
        if (!left.SequenceEqual(right)) throw new InvalidDataException("Toggle complete special-ability base/root payload differs.");
        var relocs = new System.Collections.Generic.List<uint> { 256 }; var imports = new System.Collections.Generic.List<uint>();
        if (Word(b, 8) != 0) imports.Add(8);
        for (int i = 0; i < 2; i++)
        {
            var s = proof.States[i]; int n = items + i * 304;
            left = stock.AsSpan(s.Native, 304).ToArray(); right = b.AsSpan(n, 304).ToArray();
            if (instance != null) foreach (int slot in new[] { 0, 96, 120, 124, 192 }) Identity(left, right, slot);
            // Reborn: even the empty generic-event default has its own relocated string target in stock.
            if (Word(stock, s.Native + 116) == 0 || Word(b, n + 116) == 0 || Text(stock, s.Native + 112) != Text(b, n + 112)) throw new InvalidDataException("Toggle generic-event string differs.");
            left.AsSpan(116, 4).Clear(); right.AsSpan(116, 4).Clear(); relocs.Add((uint)(n + 116));
            foreach (var o in s.Optionals)
            {
                int offset = o.Slot - s.Native; int p = (int)Word(b, n + offset);
                if ((o.Target == 0) != (p == 0) || (p != 0 && Word(stock, o.Target) != Word(b, p))) throw new InvalidDataException("Toggle optional child differs.");
                if (p != 0) relocs.Add((uint)(n + offset)); left.AsSpan(offset, 4).Clear(); right.AsSpan(offset, 4).Clear();
            }
            int sw = (int)Word(stock, s.Native + 276), ow = (int)Word(b, n + 276);
            if ((sw == 0) != (ow == 0)) throw new InvalidDataException("Toggle weapon presence differs.");
            left.AsSpan(276, 4).Clear(); right.AsSpan(276, 4).Clear();
            if (!left.SequenceEqual(right)) throw new InvalidDataException("Toggle complete state/AI native payload differs: " + string.Join(", ", Enumerable.Range(0, 304).Where(o => left[o] != right[o]).Take(12).Select(o => $"+{o}:{left[o]:X2}/{right[o]:X2}")));
            foreach (int slot in new[] { 0, 96, 120, 124, 192 }) if (Word(b, n + slot) != 0) imports.Add((uint)(n + slot));
            if (ow != 0)
            {
                relocs.Add((uint)(n + 276));
                var sl = stock.AsSpan(sw, 28).ToArray(); var sr = b.AsSpan(ow, 28).ToArray();
                if (instance != null) Identity(sl, sr, 0);
                if (!sl.SequenceEqual(sr)) throw new InvalidDataException("Toggle weapon payload differs.");
                if (Word(b, ow) != 0) imports.Add((uint)ow);
            }
        }
        CheckSlots(chunk.RelocationBuffer, relocs); CheckSlots(chunk.ImportsBuffer, imports);

        //-------------------------------------------------------------------------------------------------
        /** Reborn: stock/Core one-biased words must identify the same concrete type and instance before clearing their bytes. */
        //-------------------------------------------------------------------------------------------------
        void Identity(byte[] a, byte[] c, int slot)
        {
            uint x = Word(a, slot), y = Word(c, slot);
            if ((x == 0) != (y == 0)) throw new InvalidDataException("Toggle Core reference presence differs.");
            if (x == 0) return;
            if (stockRefs == null || instance == null || x > stockRefs.Length || y > instance.ReferencedInstances.Count) throw new InvalidDataException("Toggle Core reference index bounds exceeded.");
            var one = stockRefs[(int)x - 1]; var two = instance.ReferencedInstances[(int)y - 1];
            if (one.TypeId != two.TypeId || one.InstanceId != two.InstanceId) throw new InvalidDataException("Toggle Core reference identity differs.");
            a.AsSpan(slot, 4).Clear(); c.AsSpan(slot, 4).Clear();
        }
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: reject extra/missing/duplicated pointer/import slots and require the tracker sentinel when a slot stream is present. */
    //-------------------------------------------------------------------------------------------------
    private static void CheckSlots(byte[] bytes, System.Collections.Generic.List<uint> slots)
    {
        var actual = Enumerable.Range(0, bytes.Length / 4).Select(i => Word(bytes, i * 4)).ToArray();
        if (slots.Count == 0 ? actual.Length != 0 : actual.Length != slots.Count + 1 || actual[^1] != uint.MaxValue || !actual[..^1].Order().SequenceEqual(slots.Order()))
            throw new InvalidDataException("Toggle auxiliary slots differ.");
    }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: compare authored string values through their own length/target storage without splitting embedded spaces into list items. */
    //-------------------------------------------------------------------------------------------------
    private static string Text(byte[] bytes, int at)
    {
        int length = checked((int)Word(bytes, at)), target = checked((int)Word(bytes, at + 4));
        if (target <= 0 || length < 0 || length >= bytes.Length || target > bytes.Length - length - 1 || bytes[target + length] != 0)
            throw new InvalidDataException("Toggle string length/target/terminator differs.");
        return System.Text.Encoding.ASCII.GetString(bytes, target, length);
    }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: malformed native/source diagnostics must not report success. */
    //-------------------------------------------------------------------------------------------------
    private static void Refuse(Action action) { try { action(); } catch (Exception e) when (e is InvalidDataException or BinaryAssetBuilderException) { return; } throw new InvalidDataException("Invalid toggle module diagnostic accepted."); }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: constrain diagnostic word reads to the owned/captured buffer. */
    //-------------------------------------------------------------------------------------------------
    private static uint Word(byte[] b, int at) { if (at < 0 || at > b.Length - 4) throw new InvalidDataException("Toggle word bounds exceeded."); return BinaryPrimitives.ReadUInt32LittleEndian(b.AsSpan(at, 4)); }
}
