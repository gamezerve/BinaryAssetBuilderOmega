using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Xml;
using System.Xml.Schema;
using BinaryAssetBuilder.Core.Hashing;
using Chunk = Relo.Chunk;

namespace BinaryAssetBuilder.XmlCompiler;

// Reborn: schema-order metadata for the explicitly reviewed original Conscript behavior families; input contains native data and schemas, never stock token tables.
public static class Ep1BehaviorMetadataTokenizer
{
    private const string Ns = "uri:ea.com:eala:asset";
    private static readonly HashSet<string> Families = new(StringComparer.Ordinal) { "ObjectSMCHelperModuleData", "ObjectDefectionHelperModuleData", "AttributeModifierPoolUpdateModuleData", "SquishCollideModuleData", "CrushDieModuleData", "StancesBehaviorModuleData", "DestroyDieModuleData", "CreateObjectDieModuleData", "FXListBehaviorModuleData", "FireWeaponWhenDeadBehaviorModuleData", "AutoHealBehaviorModuleData", "SecondaryDamageTrackerModuleData", "RunOffMapBehaviorModuleData", "LevelUpUpgradeModuleData", "SuppressionUpdateModuleData", "FreefallUpdateModuleData", "WeaponSetUpdateModuleData", "PhysicsBehaviorModuleData", "SlowDeathBehaviorModuleData", "SpecialPowerModuleData", "ToggleStatusSpecialAbilityUpdateModuleData", "StatusBitsUpgradeModuleData", "HitReactionBehaviorModuleData", "ModelConditionUpgradeModuleData" };

    //-------------------------------------------------------------------------------------------------
    /** Reborn: select a bounded concrete family by its original wire identity; callers cannot submit unreviewed classes through the reflection/schema seam. */
    //-------------------------------------------------------------------------------------------------
    public static Chunk Tokenize(Chunk native, XmlSchemaSet schemas, string wireName, Type nativeType)
    {
        if (native?.InstanceBuffer == null || native.RelocationBuffer == null || native.ImportsBuffer == null || native.InstanceBuffer.Length < 8 || native.InstanceBuffer.Length > 2 * 1048576 || schemas == null || !Families.Contains(wireName) || Word(native.InstanceBuffer, 0) != FastHash.GetHashCode(wireName) || Word(native.InstanceBuffer, 4) == 0) throw new InvalidDataException("Reviewed bounded behavior native graph required.");
        if (nativeType == null || nativeType.Assembly != typeof(SageBinaryData.GameObject).Assembly || (nativeType.Name.StartsWith("Ep1", StringComparison.Ordinal) ? nativeType.Name.Substring(3) : nativeType.Name) != wireName) throw new InvalidDataException("Behavior native model/wire identity differs.");
        if (wireName == "WeaponSetUpdateModuleData") return Ep1WeaponSetTokenizer.Tokenize(native);
        return new Builder(native.InstanceBuffer, schemas).Build(native, wireName, nativeType);
    }

    // Reborn: each invocation owns all tables, allocation checks and final coordinate patches.
    private sealed class Builder
    {
        private readonly byte[] bytes;
        private readonly XmlSchemaSet schemas;
        private readonly bool[] covered;
        private readonly List<uint> metadata = new();
        private readonly List<(int At, int Target)> targets = new();
        private readonly HashSet<uint> relos = new(), imports = new();
        private int visits;
        //-------------------------------------------------------------------------------------------------
        /** Reborn: schemas and native bytes are read-only; all mutable ownership state is local. */
        //-------------------------------------------------------------------------------------------------
        internal Builder(byte[] bytes, XmlSchemaSet schemas) { this.bytes = bytes; this.schemas = schemas; covered = new bool[bytes.Length]; }
        //-------------------------------------------------------------------------------------------------
        /** Reborn: finish only after complete native storage and exact auxiliary inventories have been consumed, then shift native addresses once. */
        //-------------------------------------------------------------------------------------------------
        internal Chunk Build(Chunk native, string wireName, Type type)
        {
            Claim(0, Size(type)); int wrapper = Allocate(1); Row(wrapper, 2, FastHash.GetHashCode(wireName), (uint)Emit(wireName, type, 0, 0));
            uint[] suppliedRelos = Slots(native.RelocationBuffer), suppliedImports = Slots(native.ImportsBuffer);
            if (!relos.SetEquals(suppliedRelos) || !imports.SetEquals(suppliedImports)) throw new InvalidDataException("Behavior metadata auxiliary ownership differs.");
            int gap = 0; for (int i = 0; i < bytes.Length; i++) { if (covered[i]) { gap = 0; continue; } if (bytes[i] != 0 || ++gap > 3) throw new InvalidDataException("Behavior metadata unowned allocation at " + i); }
            int prefix = checked(metadata.Count * 4); var result = new Chunk { InstanceBuffer = new byte[prefix + bytes.Length], RelocationBuffer = Pack(suppliedRelos.Select(s => s + (uint)prefix)), ImportsBuffer = Pack(suppliedImports.Select(s => s + (uint)prefix)) };
            for (int i = 0; i < metadata.Count; i++) Put(result.InstanceBuffer, i * 4, metadata[i]); foreach (var target in targets) Put(result.InstanceBuffer, target.At, checked((uint)(prefix + target.Target)));
            bytes.CopyTo(result.InstanceBuffer, prefix); foreach (uint slot in suppliedRelos) Put(result.InstanceBuffer, prefix + (int)slot, checked(Word(bytes, (int)slot) + (uint)prefix)); return result;
        }
        //-------------------------------------------------------------------------------------------------
        /** Reborn: own attributes precede own elements, with one explicit inherited table; unresolved native members are rejected rather than silently omitted. */
        //-------------------------------------------------------------------------------------------------
        private int Emit(string schemaName, Type type, int native, int depth)
        {
            if (depth > 32 || ++visits > 100000) throw new InvalidDataException("Behavior metadata traversal bound exceeded."); Range(native, Size(type));
            var schema = schemas.GlobalTypes[new XmlQualifiedName(schemaName, Ns)] as XmlSchemaComplexType ?? throw new InvalidDataException("Behavior complex schema missing: " + schemaName);
            var extension = (schema.ContentModel as XmlSchemaComplexContent)?.Content as XmlSchemaComplexContentExtension;
            if (schema.ContentModel != null && extension == null) throw new InvalidDataException("Unreviewed behavior schema content: " + schemaName);
            var fields = new List<(string Name, XmlSchemaType Schema, string Reference, bool Repeated)>(); var attributes = extension?.Attributes ?? schema.Attributes;
            if (schemaName == "ModuleData") fields.Add(("TypeId", null, "", false));
            foreach (XmlSchemaObject item in attributes) { if (item is not XmlSchemaAttribute a || a.Name == null || a.AttributeSchemaType == null) throw new InvalidDataException("Unreviewed behavior attribute declaration."); fields.Add((a.Name, a.AttributeSchemaType, Reference(a.UnhandledAttributes), false)); }
            Elements(extension?.Particle ?? schema.Particle, fields);
            bool parent = extension != null; int table = Allocate(fields.Count + (parent ? 1 : 0) + 1), next = table; var used = new bool[Size(type)];
            if (parent)
            {
                var baseField = type.GetField("Base") ?? throw new InvalidDataException("Behavior native base missing: " + type.Name); if (Offset(type, baseField) != 0) throw new InvalidDataException("Behavior native base is not first.");
                Mark(used, 0, Size(baseField.FieldType)); Row(next, 1, FastHash.GetHashCode(extension.BaseTypeName.Name), (uint)Emit(extension.BaseTypeName.Name, baseField.FieldType, native, depth + 1)); next += 12;
            }
            foreach (var field in fields)
            {
                var info = type.GetField(field.Name) ?? throw new InvalidDataException("Behavior native field missing: " + schemaName + "." + field.Name); int offset = Offset(type, info); Mark(used, offset, Size(info.FieldType));
                string wire = field.Schema?.QualifiedName.Name; if (field.Name != "TypeId" && string.IsNullOrEmpty(wire)) throw new InvalidDataException("Behavior anonymous field wire type: " + schemaName + "." + field.Name);
                // Reborn: field signatures use reviewed type/reference/list identities, not plain concrete-class name hashing.
                if (field.Schema?.QualifiedName.Namespace == XmlSchema.Namespace) wire = "xs:" + wire;
                if (field.Reference.Length != 0) wire += ":" + field.Reference;
                if (field.Repeated) wire += "[]";
                Row(next, FastHash.GetHashCode(field.Name), field.Name == "TypeId" ? 0x23E650B8 : Signature(wire), 0); Field(field.Schema, info.FieldType, checked(native + offset), next + 8, depth + 1); next += 12;
            }
            foreach (var info in type.GetFields(BindingFlags.Public | BindingFlags.Instance)) { int at = Offset(type, info); if (Enumerable.Range(at, Size(info.FieldType)).Any(i => !used[i])) throw new InvalidDataException("Behavior native member is not schema-enabled: " + schemaName + "." + info.Name); }
            for (int i = 0; i < used.Length; i++) if (!used[i] && bytes[native + i] != 0) throw new InvalidDataException("Behavior nonzero native padding."); return table;
        }
        //-------------------------------------------------------------------------------------------------
        /** Reborn: pointer, list, string and reference representations are derived from actual native types; schemas supply field order and wire tags only. */
        //-------------------------------------------------------------------------------------------------
        private void Field(XmlSchemaType schema, Type type, int slot, int targetAt, int depth)
        {
            if (type.IsPointer)
            {
                uint pointer = Word(bytes, slot); if (pointer == 0) return; int target = Pointer(pointer), size = Size(type.GetElementType()); Claim(target, size); relos.Add((uint)slot); Field(schema, type.GetElementType(), target, targetAt, depth + 1); return;
            }
            if (type.IsGenericType && type.GetGenericTypeDefinition().FullName == "Relo.List`1")
            {
                uint count = Word(bytes, slot), pointer = Word(bytes, slot + 4); if (count > 4096 || (count == 0) != (pointer == 0)) throw new InvalidDataException("Behavior metadata list invariant differs.");
                int list = Allocate(1 + (int)count); Row(list, 0x6C827073, 0x5D448FD8, 0); Native(list + 8, slot); Target(targetAt, list);
                if (count != 0)
                {
                    Type child = type.GetGenericArguments()[0]; int stride = Size(child), items = Pointer(pointer); Claim(items, checked((int)count * stride)); relos.Add((uint)(slot + 4));
                    XmlSchemaType itemSchema = schema is XmlSchemaSimpleType simple && simple.Content is XmlSchemaSimpleTypeList listType ? listType.BaseItemType : schema;
                    for (int i = 0; i < count; i++) { int marker = list + 12 + i * 12; Row(marker, 0xB151A0EB, 0, 0); Field(itemSchema, child, items + i * stride, marker + 8, depth + 1); }
                }
                return;
            }
            if (schema is XmlSchemaComplexType complex) { Target(targetAt, Emit(complex.Name, type, slot, depth + 1)); return; }
            Scalar(schema, type, slot); Native(targetAt, slot);
        }
        //-------------------------------------------------------------------------------------------------
        /** Reborn: scalar payload checks include actual reference slots and string allocations; optional empty references retain native-word targets without inventing imports. */
        //-------------------------------------------------------------------------------------------------
        private void Scalar(XmlSchemaType schema, Type type, int at)
        {
            Range(at, Size(type)); string generic = type.IsGenericType ? type.GetGenericTypeDefinition().FullName : "";
            if (generic == "Relo.String`1")
            {
                if (type.GetGenericArguments()[0] != typeof(sbyte)) throw new InvalidDataException("Behavior string encoding unreviewed."); uint length = Word(bytes, at), pointer = Word(bytes, at + 4); if (length > 65536 || pointer == 0 && length != 0) throw new InvalidDataException("Behavior text header differs.");
                if (pointer != 0) { int text = Pointer(pointer, false); Claim(text, checked((int)length + 1)); for (int i = 0; i < length; i++) if (bytes[text + i] == 0 || bytes[text + i] >= 128) throw new InvalidDataException("Behavior ASCII text differs."); if (bytes[text + length] != 0) throw new InvalidDataException("Behavior text terminator missing."); relos.Add((uint)(at + 4)); } return;
            }
            if (generic is "Relo.AssetReference`1" or "Relo.AssetReference`2") { if (Word(bytes, at) != 0) imports.Add((uint)at); return; }
            if (type == typeof(global::SageBool) && bytes[at] > 1) throw new InvalidDataException("Behavior bool invalid.");
            if (type.IsEnum && !Enum.IsDefined(type, Enum.ToObject(type, Word(bytes, at)))) throw new InvalidDataException("Behavior enum invalid.");
            if (schema != null && new[] { "Time", "Percentage", "SageReal", "Angle", "Velocity", "Acceleration" }.Contains(schema.Name) && Size(type) == 4 && !float.IsFinite(BitConverter.Int32BitsToSingle(unchecked((int)Word(bytes, at))))) throw new InvalidDataException("Behavior nonfinite real.");
        }
        //-------------------------------------------------------------------------------------------------
        /** Reborn: only literal sequence/element schema particles are admitted; choices, wildcards and unresolved groups do not become guessed layouts. */
        //-------------------------------------------------------------------------------------------------
        private static void Elements(XmlSchemaParticle particle, List<(string Name, XmlSchemaType Schema, string Reference, bool Repeated)> fields)
        { if (particle == null) return; if (particle is XmlSchemaSequence sequence) { foreach (XmlSchemaObject child in sequence.Items) { if (child is not XmlSchemaParticle p) throw new InvalidDataException("Behavior particle invalid."); Elements(p, fields); } } else if (particle is XmlSchemaElement e && e.Name != null && e.ElementSchemaType != null) fields.Add((e.Name, e.ElementSchemaType, Reference(e.UnhandledAttributes), e.MaxOccurs > 1 || e.MaxOccursString == "unbounded")); else throw new InvalidDataException("Behavior schema particle is not metadata-enabled."); }
        //-------------------------------------------------------------------------------------------------
        /** Reborn: all native allocation owners must be disjoint and bounded before any metadata target is emitted. */
        //-------------------------------------------------------------------------------------------------
        private void Claim(int at, int length) { Range(at, length); for (int i = at; i < at + length; i++) if (covered[i]) throw new InvalidDataException("Behavior metadata native owners overlap."); Array.Fill(covered, true, at, length); }
        //-------------------------------------------------------------------------------------------------
        /** Reborn: bounded row allocations are zero-initialized, including each required record terminator. */
        //-------------------------------------------------------------------------------------------------
        private int Allocate(int rows) { if (rows < 1 || rows > (1048576 - metadata.Count) / 3) throw new InvalidDataException("Behavior metadata allocation bound exceeded."); int at = metadata.Count * 4; for (int i = 0; i < rows * 3; i++) metadata.Add(0); return at; }
        //-------------------------------------------------------------------------------------------------
        /** Reborn: primitive/native targets shift only after the final prefix extent is known. */
        //-------------------------------------------------------------------------------------------------
        private void Native(int at, int target) { Range(target, 1); targets.Add((at, target)); }
        //-------------------------------------------------------------------------------------------------
        /** Reborn: metadata-only targets keep their own origin and never enter RELO. */
        //-------------------------------------------------------------------------------------------------
        private void Target(int at, int target) => metadata[at / 4] = (uint)target;
        //-------------------------------------------------------------------------------------------------
        /** Reborn: complete row writes address newly allocated metadata only. */
        //-------------------------------------------------------------------------------------------------
        private void Row(int at, uint name, uint tag, uint target) { metadata[at / 4] = name; metadata[at / 4 + 1] = tag; metadata[at / 4 + 2] = target; }
        //-------------------------------------------------------------------------------------------------
        /** Reborn: pointer narrowing and optional alignment checks precede all native slices. */
        //-------------------------------------------------------------------------------------------------
        private int Pointer(uint value, bool aligned = true) { if (value > int.MaxValue || aligned && value % 4 != 0) throw new InvalidDataException("Behavior pointer alignment/overflow differs."); Range((int)value, 1); return (int)value; }
        //-------------------------------------------------------------------------------------------------
        /** Reborn: bounds checks apply to every native owner and payload. */
        //-------------------------------------------------------------------------------------------------
        private void Range(int at, int length) { if (length < 0 || at < 0 || at > bytes.Length - length) throw new InvalidDataException("Behavior metadata native bounds differ."); }
    }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: these independently stock-reviewed schema signature constants contain no asset offsets, native payloads or token templates; unfamiliar signatures fail closed. */
    //-------------------------------------------------------------------------------------------------
    private static uint Signature(string key) => key switch
    {
        "AIAoeAttackPowerEmanationLocation" => 0x33433965, "AIAoeAttackPowerLocationPreference" => 0x0B3BCAE4,
        "AISpecialPowerWeaponData" => 0x43DBA49D, "AITargetLocationBitFlags" => 0x73B8B420, "AIToggleSpecialPowerInfo" => 0xFE374CCD,
        "Angle" => 0x4E16EA4D, "AnimAndDuration" => 0x6B746AF6, "ArmorSetType" => 0x2037048F, "AssetId" => 0x805D25F4,
        "AttributeModifierRef" => 0x09DDCFF3, "AudioEventInfoRef" => 0x63D7B468, "AudioEventInfoRefList" => 0xE1117F93,
        "ConditionsBitFlags" => 0x041F5A89, "DeathBitFlags" => 0x034DF1A1, "DieMuxDataType" => 0x0084001B,
        "DisabledBitFlags" => 0x196570C9, "DisabledType" => 0xBDE95B5A, "DurationUnsignedInt" => 0x6F6D1AE7,
        "EffectInfoBitFlags" => 0xA2F37A52, "EffectInfoType[]" => 0x25F5379F,
        "FXListBehaviorDirection" => 0x5E151008, "FXListBehaviorEvent[]" => 0x65BE8011, "FXListBehaviorType" => 0x03AF8D2F,
        "FXListRef" => 0x0D1C8E83, "FXParticleSystemRef" => 0x43F1CE36, "GameObjectWeakRef" => 0x148ED75D,
        "KindOfBitFlags" => 0x9AFC9193, "LocomotorSetType" => 0x845D12B2, "LogicCommandRef" => 0xE0E2A81A,
        "MicroManagerStateList" => 0xD7620AAA, "MicroManagerUnitTaskList" => 0x74BAC2ED,
        "ModelConditionBitFlags" => 0x67DBBFC9, "ModelConditionFlagType" => 0x15723E0B,
        "ObjectCreationListRef" => 0x42E4E9D1, "ObjectFilter" => 0xE46EA3B2, "ObjectFilterAlignment" => 0x9FFD43EB,
        "ObjectFilterAssetRef" => 0x1E592B76, "ObjectFilterRelationshipBitMask" => 0x5C8981A9, "ObjectFilterRule" => 0x0027BB58,
        "ObjectStatusBitFlags" => 0xA7BD811E, "ObjectStatusValidationDataType" => 0x3E8F938C,
        "PartitionManagerDistTestType" => 0x9F0DA607, "Percentage" or "SageReal" or "xs:float" => 0x645864F3,
        "PlayerTechRef" => 0xDBBF0FDF, "Poid:ModuleData" => 0xBDF07965, "Poid:ObjectFilter" => 0x4D3B3F19,
        "RGBColor" => 0xAEE762A5, "SageBool" => 0x77E3F4F7, "SageInt" => 0x23E650B8, "SageUnsignedInt" => 0x763177A4,
        "SlowDeathFXListType[]" => 0x33D0DFAE, "SlowDeathOCLType[]" => 0x97474300, "SlowDeathPhaseType" => 0x350A89A5,
        "SlowDeathSoundType[]" => 0xB8E87C25, "SlowDeathWeaponType[]" => 0x908107EC, "SoundOrEvaEvent" => 0x1AE6B6B7,
        "SpecialAbilityUpdateOptionsTypeBitFlags" => 0x982F7738, "SpecialPowerModuleInfo[]" => 0xADA54D29,
        "SpecialPowerRef" => 0x96311B79, "SpecialPowerTemplateRef" => 0xAEE83888, "SpecialPowerWeakRef" => 0x40BBFFB2,
        "StanceTemplateRef" => 0x0D8218FB, "StringList" => 0x6D66AE18, "Time" => 0x998669E8,
        "ToggleStatusAiHint" => 0x018DD8B9, "ToggleStatusNugget[]" => 0xFC294B1D,
        "UpgradeTemplateRef" or "UpgradeTemplateRef[]" => 0x4ABA1F29, "UpgradeTemplateWeakRef[]" => 0x26FACFC7,
        "Vector3" => 0x29368077, "Velocity" => 0x7A76C60E, "WeakReference:GameObject[]" => 0x9B19049F,
        "WeaponChoiceCriteria" => 0x1D110792, "WeaponFireProbabilityType[]" => 0x080BE21F, "WeaponRef" => 0xD046DD22,
        "WeaponSlotInterleavedStyleType" => 0xBE669829, "WeaponSlotType" => 0x1848C549,
        "WeaponSlot_Contained[]" => 0xBA2D72E2, "WeaponSlot_Hardpoint[]" => 0xCE5853D7,
        "WeaponSlot_HierarchicalTurret[]" => 0xC4884051, "WeaponSlot_Turret[]" => 0x4F14A301,
        "WeaponSlot_WeaponData[]" => 0x77994E4B, "WeatherType" => 0xC5006C0F,
        "xs:string" or "xs:string[]" => 0x1A2933AA,
        _ => throw new InvalidDataException("Behavior schema signature not reviewed: " + key)
    };
    //-------------------------------------------------------------------------------------------------
    /** Reborn: declaration-level reference annotations refine otherwise identical schema scalar identities. */
    //-------------------------------------------------------------------------------------------------
    private static string Reference(XmlAttribute[] attributes) => attributes?.SingleOrDefault(a => a.LocalName == "refType" && a.NamespaceURI == "uri:ea.com:eala:asset:schema")?.Value ?? "";
    //-------------------------------------------------------------------------------------------------
    /** Reborn: schema descriptors cannot overlap native fields or extend beyond a native record. */
    //-------------------------------------------------------------------------------------------------
    private static void Mark(bool[] used, int at, int length) { if (at < 0 || length < 0 || at > used.Length - length) throw new InvalidDataException("Behavior native field extent differs."); for (int i = at; i < at + length; i++) if (used[i]) throw new InvalidDataException("Behavior native fields overlap."); Array.Fill(used, true, at, length); }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: actual runtime layout supplies native offsets, never inferred XML declaration order. */
    //-------------------------------------------------------------------------------------------------
    private static int Offset(Type type, FieldInfo field) => Marshal.OffsetOf(type, field.Name).ToInt32();
    //-------------------------------------------------------------------------------------------------
    /** Reborn: explicit wire sizes cover closed generic primitives rejected by Marshal.SizeOf. */
    //-------------------------------------------------------------------------------------------------
    private static int Size(Type type) => type.IsPointer ? 4 : type.IsEnum ? 4 : type.IsGenericType ? type.GetGenericTypeDefinition().FullName switch { "Relo.List`1" or "Relo.String`1" => 8, "Relo.AssetReference`1" or "Relo.AssetReference`2" or "TypedAssetId`1" => 4, _ => throw new InvalidDataException("Behavior generic native type unreviewed.") } : Marshal.SizeOf(type);
    //-------------------------------------------------------------------------------------------------
    /** Reborn: auxiliary arrays require bounded unique aligned slots and a complete nonempty sentinel. */
    //-------------------------------------------------------------------------------------------------
    private static uint[] Slots(byte[] b) { if (b.Length == 0) return Array.Empty<uint>(); if (b.Length < 8 || b.Length > 2 * 1048576 || b.Length % 4 != 0 || Word(b, b.Length - 4) != uint.MaxValue) throw new InvalidDataException("Behavior metadata auxiliary framing differs."); var slots = Enumerable.Range(0, b.Length / 4 - 1).Select(i => Word(b, i * 4)).ToArray(); if (slots.Distinct().Count() != slots.Length || slots.Any(s => s % 4 != 0)) throw new InvalidDataException("Behavior metadata duplicate/unaligned slot."); return slots; }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: original Tracker auxiliary order is retained, with no sentinel for an empty array. */
    //-------------------------------------------------------------------------------------------------
    private static byte[] Pack(IEnumerable<uint> source) { var values = source.ToArray(); if (values.Length == 0) return Array.Empty<byte>(); var b = new byte[(values.Length + 1) * 4]; for (int i = 0; i < values.Length; i++) Put(b, i * 4, values[i]); Put(b, values.Length * 4, uint.MaxValue); return b; }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: all metadata/native word reads stay bounded. */
    //-------------------------------------------------------------------------------------------------
    private static uint Word(byte[] b, int at) { if (at < 0 || at > b.Length - 4) throw new InvalidDataException("Behavior metadata word bounds differ."); return BinaryPrimitives.ReadUInt32LittleEndian(b.AsSpan(at, 4)); }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: writes affect owned generated buffers only. */
    //-------------------------------------------------------------------------------------------------
    private static void Put(byte[] b, int at, uint value) => BinaryPrimitives.WriteUInt32LittleEndian(b.AsSpan(at, 4), value);
}
