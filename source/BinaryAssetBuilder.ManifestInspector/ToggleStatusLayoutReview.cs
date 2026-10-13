using System.Buffers.Binary;
using System.Runtime.InteropServices;
using System.Text.Json;
using System.Xml;
using BinaryAssetBuilder.Core.Hashing;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: recover bounded toggle/base/nested field anchors before enabling a new native module; no source compilation or publication claim.
internal static class ToggleStatusLayoutReview
{
    internal sealed record Field(string Name, uint Type, int Target, int RelativeOffset, string Relationship);
    internal sealed record Optional(string Name, int Slot, int Target, uint? Value);
    internal sealed record Child(string Name, int Count, int Items, int[] Tables);
    internal sealed record State(int Native, Field[] Fields, Field[] AiFields, Field[] WeaponFields, Optional[] Optionals, Child[] Children);
    internal sealed record Proof(string Asset, int Native, int ExistingBaseSize, Field[] BaseFields, State[] States)
    {
        // Reborn: token/native anchors are only layout evidence, not complete payload, references, defaults or marshaler correctness.
        public bool ExistingBaseFieldOffsetsVerified => true;
        public bool ToggleStateStrideVerified => true;
        // Reborn: absent optional tokens cannot independently prove which native pointer slot belongs to a future present value.
        public bool PresentOptionalAiLayoutsVerified => false;
        // Reborn: report only the present pointer/value joins actually observed; this count is not coverage of all thirteen optional fields.
        public int ObservedPresentOptionalJoins => States.Sum(s => s.Optionals.Count(o => o.Target != 0));
        public bool ReferenceIdentitiesVerified => false;
        public bool MarshalerPayloadVerified => false;
        public bool ToggleCompilerEnabled => false;
        public bool GameObjectPublicationEnabled => false;
        public bool GameExecuted => false;
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: reuse the fingerprint-admitted stock capture and select the known Conscript toggle module through its reviewed class chain. */
    //-------------------------------------------------------------------------------------------------
    internal static Proof Inspect(string manifest, string name = "GameObject:SovietAntiInfantryInfantry")
    {
        var capture = GameObjectStockReview.Capture(manifest, name);
        var module = GameObjectModuleReview.Inspect(manifest, name).Containers.SelectMany(c => c.Modules)
            .Single(m => m.Type == "ToggleStatusSpecialAbilityUpdateModuleData");
        return Decode(name, capture.Bytes, capture.Projection.NativeRoot, module);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: require direct reflected base offsets, two ordered 304-byte state records and bounded nested field tables without inferring their type semantics. */
    //-------------------------------------------------------------------------------------------------
    internal static Proof Decode(string name, byte[] bytes, int root, GameObjectModuleReview.Module module)
    {
        //-------------------------------------------------------------------------------------------------
        /** Reborn: read only aligned words inside the admitted asset slice. */
        //-------------------------------------------------------------------------------------------------
        uint Word(int at)
        {
            if (at < 0 || at % 4 != 0 || at > bytes.Length - 4) throw new InvalidDataException("Toggle word range exceeded.");
            return BinaryPrimitives.ReadUInt32LittleEndian(bytes.AsSpan(at, 4));
        }
        //-------------------------------------------------------------------------------------------------
        /** Reborn: token tables must stay before the native root, never point into arbitrary native data. */
        //-------------------------------------------------------------------------------------------------
        int Table(int at)
        {
            int target = checked((int)Word(at));
            if (target <= 0 || target % 4 != 0 || target >= root) throw new InvalidDataException("Toggle token target out of bounds.");
            return target;
        }
        //-------------------------------------------------------------------------------------------------
        /** Reborn: apply a token/native boundary check to the shared bounded parser for every nested table. */
        //-------------------------------------------------------------------------------------------------
        Dictionary<uint, (uint Type, int Offset)> Tokens(int table)
        {
            var tokens = GameObjectStockReview.Tokens(bytes, table);
            if (table + (tokens.Count + 1) * 12 > root) throw new InvalidDataException("Toggle table crosses native boundary.");
            return tokens;
        }
        int own = Table(module.Wrapper + 8);
        var ownFields = Tokens(own);
        int baseTable = Table(own + 8);
        var baseFields = Tokens(baseTable);
        var model = typeof(SageBinaryData.SpecialAbilityUpdateModuleData);
        int size = Marshal.SizeOf(model);
        if (size != 252 || Word(own) != 1 || Word(own + 4) != FastHash.GetHashCode(model.Name)) throw new InvalidDataException("Toggle special-ability base drift.");
        var reflected = model.GetFields().Where(f => f.Name != "Base").ToArray();
        if (baseFields.Count != reflected.Length + 1) throw new InvalidDataException("Toggle base field coverage differs.");
        var fields = new List<Field>();
        foreach (var f in reflected)
        {
            int offset = Marshal.OffsetOf(model, f.Name).ToInt32();
            if (!baseFields.TryGetValue(FastHash.GetHashCode(f.Name), out var token)) throw new InvalidDataException("Missing toggle base field: " + f.Name);
            bool absent = f.FieldType.IsPointer && token.Offset == 0 && Word(module.Native + offset) == 0;
            if (!absent && token.Offset != module.Native + offset) throw new InvalidDataException("Toggle base offset differs: " + f.Name);
            fields.Add(new(f.Name, token.Type, token.Offset, offset, absent ? "AbsentPointer" : "DirectNativeOffset"));
        }
        foreach (var f in new[] { ("ReloadAllWeaponsOnToggle", 260), ("ForceIdleAfterToggle", 261) })
            if (!ownFields.TryGetValue(FastHash.GetHashCode(f.Item1), out var token) || token.Offset != module.Native + f.Item2 || token.Type != 0x77E3F4F7)
                throw new InvalidDataException("Toggle boolean offset/type differs.");
        if (ownFields.Count != 4 || ownFields[FastHash.GetHashCode("ToggleState")].Type != 0xFC294B1D) throw new InvalidDataException("Toggle own field coverage/type differs.");
        int listTable = ownFields[FastHash.GetHashCode("ToggleState")].Offset;
        if (listTable <= 0 || listTable + 36 > root || Word(listTable) != 0x6C827073 || Word(listTable + 4) != 0x5D448FD8
            || Word(listTable + 8) != module.Native + 252 || Word(module.Native + 252) != 2) throw new InvalidDataException("Toggle state list anchor differs.");
        int items = checked((int)Word(module.Native + 256));
        if (items < root || items > bytes.Length - 608) throw new InvalidDataException("Toggle state items out of bounds.");
        var states = new List<State>();
        for (int i = 0; i < 2; i++)
        {
            int marker = listTable + 12 + i * 12;
            if (Word(marker) != 0xB151A0EB || Word(marker + 4) != 0) throw new InvalidDataException("Toggle state marker differs.");
            int native = items + i * 304, table = Table(marker + 8);
            var state = Named(table, native, "Modules/ToggleStatusSpecialAbilityUpdate.xsd", "ToggleStatusNugget");
            CheckAnchors(state, native, new[] { ("AttributeModifier", 0), ("ModelConditions", 4), ("ObjectStatus", 64), ("Upgrade", 96),
                ("ArmorSet", 100), ("LocomotorSet", 104), ("AIUpdateModuleId", 108), ("GenericLuaEventParameter", 112),
                ("EnterStateSound", 120), ("InitiateIntentToExitStateSound", 124), ("DisabledType", 128) });
            int aiTable = state.Single(f => f.Name == "SkirmishAiInfo").Target;
            var ai = Named(aiTable, native + 132, "SkirmishAI/AssetTypeAISpecialPower.xsd", "AIToggleSpecialPowerInfo");
            CheckAnchors(ai, native + 132, new[] { ("ToggleHint", 0), ("NeverUseInState", 36), ("NeverUseDuringTask", 40),
                ("NumOtherAttackersOnTarget", 56), ("LeechWeaponUpgrade", 60), ("LeechWeaponTargetHealthFraction", 64),
                ("LeechHealthActivatePowerAdvantage", 72), ("LeechHealthDeactivatePowerAdvantage", 76), ("ShieldMinCostProtected", 84),
                ("ShieldMinEnemyDPS", 88), ("ShieldCostAdjustPastDistanceFraction", 92), ("ShieldCostAdjustMultiplierAtEdge", 96),
                ("NeverUseWhenTargetIsAnyKindOf", 100), ("Location", 140), ("OnlyUseWhenHaveTarget", 148), ("NeverUseWhenClear", 149),
                ("DontUseIfInRange", 150), ("EnemyMustBeInVisionRange", 151), ("AlwaysUseWhenEnemiesInRange", 152) });
            // Reborn: the candidate optional-pointer slots are zero here; absent tokens alone do not prove their individual present-value mappings.
            var optionals = new List<Optional>();
            foreach (var optional in new[] { ("UseOnlyWhenPowerAdvantageIsAbove", 4), ("UseOnlyWhenPowerAdvantageIsBelow", 8),
                ("UseOnlyWhenCostAdvantageIsAbove", 12), ("UseOnlyWhenCostAdvantageIsBelow", 16), ("NeverUseWhenFurtherThanNUnitsFromTarget", 20),
                ("NeverUseWhenCloserThanNUnitsFromTarget", 24), ("NeverUseWhenHealthIsBelow", 28), ("MaxZDifferenceFromTarget", 32),
                ("SelfDefenseDamageLookAheadTime", 44), ("SelfDefenseDamageLookBehindTime", 48), ("SelfDefenseDamageFraction", 52),
                ("LeechWeaponSOLModuleId", 68), ("StealthOthersInvisibilityModuleId", 80) })
            {
                int target = ai.Single(f => f.Name == optional.Item1).Target, slot = native + 132 + optional.Item2;
                if (target == 0)
                {
                    if (Word(slot) != 0) throw new InvalidDataException("Toggle absent optional storage differs.");
                    optionals.Add(new(optional.Item1, slot, 0, null));
                }
                else
                {
                    if (target < root || target > bytes.Length - 4 || Word(slot) != target) throw new InvalidDataException("Toggle present optional pointer/token join differs.");
                    optionals.Add(new(optional.Item1, slot, target, Word(target)));
                }
            }
            int weaponTable = ai.Single(f => f.Name == "StateWeapon").Target;
            int weaponNative = checked((int)Word(native + 276));
            if ((weaponTable == 0) != (weaponNative == 0) || (weaponNative != 0 && (weaponNative < root || weaponNative > bytes.Length - 28))) throw new InvalidDataException("Toggle weapon pointer out of bounds.");
            var weapon = weaponTable == 0 ? Array.Empty<Field>() : Named(weaponTable, weaponNative, "SkirmishAI/AssetTypeAISpecialPower.xsd", "AISpecialPowerWeaponData");
            if (weaponTable != 0)
            CheckAnchors(weapon, weaponNative, new[] { ("Weapon", 0), ("TotalValueGreaterThan", 4), ("AoeAttackPowerLocationPreference", 8),
                ("AoeAttackPowerBlockAreaAfterActivationTime", 12), ("AoeAttackEmanationLocation", 16), ("ToggleRangeHysteresis", 20),
                ("IsDisablingAttack", 24), ("AffectsFriendlies", 25), ("AffectsSelf", 26), ("ToggleShortRangeTargetForcesTarget", 27) });
            // Reborn: independently join bounded child-list markers to inline module/string records, not only zero-count headers.
            var children = new List<Child>();
            foreach (var child in new[] { ("Module", 288, 0xADA54D29u), ("ActiveGeometry", 296, 0x1A2933AAu) })
            {
                var f = state.Single(f => f.Name == child.Item1); int t = f.Target;
                int count = checked((int)Word(native + child.Item2)), data = checked((int)Word(native + child.Item2 + 4));
                int width = child.Item1 == "Module" ? 12 : 8;
                if (count < 0 || count > (width == 12 ? 4 : 128) || t <= 0 || t + (count + 1) * 12 > root || f.Type != child.Item3
                    || Word(t) != 0x6C827073 || Word(t + 4) != 0x5D448FD8 || Word(t + 8) != native + child.Item2
                    || (count == 0 ? data != 0 : data < root || data > bytes.Length - count * width)) throw new InvalidDataException("Toggle child list header/range differs.");
                var tables = new List<int>();
                for (int j = 0; j < count; j++)
                {
                    int childMarker = t + 12 + j * 12, target = data + j * width;
                    if (Word(childMarker) != 0xB151A0EB || Word(childMarker + 4) != 0) throw new InvalidDataException("Toggle child marker differs.");
                    int entry = checked((int)Word(childMarker + 8)); tables.Add(entry);
                    if (width == 12)
                    {
                        var moduleFields = Named(entry, target, "Modules/ActivateModuleSpecialAbilityUpdate.xsd", "SpecialPowerModuleInfo");
                        CheckAnchors(moduleFields, target, new[] { ("ModuleId", 0), ("TriggerType", 4), ("TriggerAtTargetPosition", 8) });
                    }
                    else
                    {
                        int length = checked((int)Word(target)), text = checked((int)Word(target + 4));
                        if (entry != target || length < 0 || length > 65536 || text < root || text > bytes.Length - length - 1 || bytes[text + length] != 0)
                            throw new InvalidDataException("Toggle geometry string marker/length/target differs.");
                    }
                }
                children.Add(new(child.Item1, count, data, tables.ToArray()));
            }
            states.Add(new(native, state, ai, weapon, optionals.ToArray(), children.ToArray()));
        }
        return new(name, module.Native, size, fields.ToArray(), states.ToArray());

        //-------------------------------------------------------------------------------------------------
        /** Reborn: independently pin named native addresses across both records rather than trusting a relative offset printed in a report. */
        //-------------------------------------------------------------------------------------------------
        void CheckAnchors(Field[] fields, int native, (string Name, int Offset)[] expected)
        {
            foreach (var f in expected)
                if (fields.Single(s => s.Name == f.Name).Target != native + f.Offset)
                    throw new InvalidDataException("Toggle nested field offset differs: " + f.Name);
        }

        //-------------------------------------------------------------------------------------------------
        /** Reborn: map schema field names to stock tokens for diagnostics; indirect offsets remain explicitly uninterpreted. */
        //-------------------------------------------------------------------------------------------------
        Field[] Named(int table, int native, string relativeSchema, string type)
        {
            if (table <= 0 || table >= root) throw new InvalidDataException("Nested toggle schema table target out of bounds.");
            var xml = new XmlDocument { XmlResolver = null };
            using var stream = new MemoryStream(SdkEnvironmentPreflight.Read(Path.Combine(SdkEnvironmentPreflight.BaselineSchemaRoot(), relativeSchema), 2 * 1048576), false);
            using var reader = XmlReader.Create(stream, new XmlReaderSettings { XmlResolver = null, DtdProcessing = DtdProcessing.Prohibit }); xml.Load(reader);
            var ns = new XmlNamespaceManager(xml.NameTable); ns.AddNamespace("xs", "http://www.w3.org/2001/XMLSchema");
            var names = xml.SelectNodes("/xs:schema/xs:complexType[@name='" + type + "']//xs:attribute[@name] | /xs:schema/xs:complexType[@name='" + type + "']//xs:element[@name]", ns)!
                .Cast<XmlElement>().Select(e => e.GetAttribute("name")).ToDictionary(FastHash.GetHashCode);
            var tokens = Tokens(table);
            if (tokens.Count != names.Count || tokens.Keys.Any(k => !names.ContainsKey(k))) throw new InvalidDataException("Toggle nested schema field coverage differs: " + type);
            return tokens.Select(t => new Field(names[t.Key], t.Value.Type, t.Value.Offset, t.Value.Offset >= root ? t.Value.Offset - native : -1,
                t.Value.Offset >= root ? "NativeAnchorUnverified" : t.Value.Offset == 0 ? "AbsentOptionalUnverified" : "IndirectTokenTableUnverified")).ToArray();
        }
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: detached mutations must not pass stock layout checks; the real stock inputs remain untouched. */
    //-------------------------------------------------------------------------------------------------
    internal static void SelfTest(string manifest)
    {
        const string name = "GameObject:SovietAntiInfantryInfantry";
        var capture = GameObjectStockReview.Capture(manifest, name);
        var module = GameObjectModuleReview.Inspect(manifest, name).Containers.SelectMany(c => c.Modules).Single(m => m.Type == "ToggleStatusSpecialAbilityUpdateModuleData");
        var proof = Decode(name, capture.Bytes, capture.Projection.NativeRoot, module);
        Console.WriteLine(JsonSerializer.Serialize(proof, new JsonSerializerOptions { WriteIndented = true }));
        int own = (int)BinaryPrimitives.ReadUInt32LittleEndian(capture.Bytes.AsSpan(module.Wrapper + 8, 4));
        int baseTable = (int)BinaryPrimitives.ReadUInt32LittleEndian(capture.Bytes.AsSpan(own + 8, 4));
        int stateTable = (int)BinaryPrimitives.ReadUInt32LittleEndian(capture.Bytes.AsSpan(own + 44, 4));
        int firstState = (int)BinaryPrimitives.ReadUInt32LittleEndian(capture.Bytes.AsSpan(stateTable + 20, 4));
        foreach (var fault in new[] { (module.Native + 252, 3u), (module.Native + 256, 4u), (baseTable + 20, (uint)(module.Native + 12)),
            (own + 20, (uint)(module.Native + 264)), (own + 44, 4u), (firstState + 20, 4u),
            (proof.States[0].Native + 276, 4u), (proof.States[1].Native + 288, 1u), (proof.States[0].Native + 136, 1u) })
        {
            var broken = capture.Bytes.ToArray(); BinaryPrimitives.WriteUInt32LittleEndian(broken.AsSpan(fault.Item1, 4), fault.Item2);
            try { Decode(name, broken, capture.Projection.NativeRoot, module); }
            catch (InvalidDataException) { continue; }
            throw new InvalidDataException("Toggle layout corruption accepted.");
        }
        Console.WriteLine("Toggle layout: PASS (existing base offsets, two nested state/schema inventories and nine refusal controls; no payload/compiler/package/game proof).");
    }
}
