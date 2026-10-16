using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BinaryAssetBuilder.Core.Hashing;
using Chunk = Relo.Chunk;

namespace BinaryAssetBuilder.XmlCompiler;

// Reborn: generate reviewed scripted-draw metadata recursively from owned native records, never from a captured stock template or hard-coded unit identity.
public static class Ep1ScriptedDrawTokenizer
{
    private enum Kind { Int, UInt, Real, Bool, Enum, Flags, String, Ref, List, Strings, EmptyList, StringPointer, NullPointer }
    private sealed record Field(string Name, uint Tag, int Offset, Kind Kind, int Width = 4, int Limit = 0, Record Child = null);
    private sealed record Record(string Name, int Size, Field[] Fields, int PaddingAt = 0, int PaddingLength = 0, Record Parent = null, uint ParentTag = 0);
    // Reborn: literal wire tags/order are independently stock-reviewed; CLR type names are not used as guessed type IDs.
    private static readonly Record Module = new("ModuleData", 8, new[] { F("TypeId", 0x23E650B8, 0), F("id", 0xBDF07965, 4, Kind.UInt) });
    private static readonly Record Draw = new("DrawModuleData", 8, Array.Empty<Field>(), Parent: Module, ParentTag: 0xD75D93A2);
    private static readonly Record Model = new("ScriptedModelDrawModel", 8, new[] { F("Name", 0xFCB106D5, 0, Kind.Ref), F("ExtraMesh", 0x77E3F4F7, 4, Kind.Bool, 1) }, 5, 3);
    private static readonly Record Bone = new("BoneAttachPoint", 16, new[] { F("WeaponSlotID", 0x23E650B8, 0), F("WeaponSlotType", 0x1848C549, 4, Kind.Enum, limit: 6), F("BoneName", 0x1A2933AA, 8, Kind.String, 8) });
    private static readonly Record Texture = new("ReplaceTexture", 8, new[] { F("Original", 0x4C3E3938, 0, Kind.Ref), F("New", 0x4C3E3938, 4, Kind.Ref) });
    private static readonly Record Particle = new("ParticleSysBone", 36, new[] {
        F("BoneName",0x1A2933AA,0,Kind.String,8), F("FXParticleSystemTemplate",0x43F1CE36,8,Kind.Ref), F("FollowBone",0x77E3F4F7,32,Kind.Bool,1),
        F("FXTrigger",0x808B85BE,12,Kind.Enum,limit:3), F("FXAction",0x7C49BC33,16,Kind.Enum,limit:4), F("PersistID",0x23E650B8,20),
        F("OnlyIfOnWater",0x77E3F4F7,33,Kind.Bool,1), F("OnlyIfOnLand",0x77E3F4F7,34,Kind.Bool,1), F("id",0x805D25F4,24,Kind.String,8) }, 35, 1);
    private static readonly Record Animation = new("Animation", 60, new[] {
        F("Flags",0x1A2933AA,0,Kind.String,8), F("AnimationName",0xFE2DCB83,8,Kind.Ref), F("AnimationMode",0xE3CDC786,12,Kind.Enum,limit:8), F("AnimNickName",0x1A2933AA,16,Kind.String,8),
        F("Distance",0x645864F3,24,Kind.Real), F("AnimationBlendTime",0x645864F3,28,Kind.Real), F("AnimationMustCompleteBlend",0x77E3F4F7,56,Kind.Bool,1),
        F("AnimationSpeedFactorMin",0x645864F3,32,Kind.Real), F("AnimationSpeedFactorMax",0x645864F3,36,Kind.Real), F("AnimationAbsoluteTime",0x998669E8,40,Kind.Real),
        F("UseWeaponTiming",0x77E3F4F7,57,Kind.Bool,1), F("WeaponTimingOrdering",0x1848C549,44,Kind.Enum,limit:6), F("WeaponTimingSlotID",0x23E650B8,48), F("AnimationPriority",0x23E650B8,52) }, 58, 2);
    private static readonly Record ModelState = new("ModelConditionState", 196, new[] {
        F("ParseCondStateType",0x1323B870,0,Kind.Enum,limit:3), F("Name",0x1A2933AA,4,Kind.String,8), F("ConditionsYes",0x67DBBFC9,12,Kind.Flags,60,463), F("Skeleton",0x805D25F4,72,Kind.String,8),
        F("PortraitImage",0x0108BD1E,80,Kind.Ref), F("ButtonImage",0x0108BD1E,84,Kind.Ref), F("OverrideTooltip",0x1A2933AA,88,Kind.String,8), F("RetainSubObjects",0x77E3F4F7,192,Kind.Bool,1), F("id",0x805D25F4,96,Kind.String,8),
        F("Model",0xCD8EC40B,104,Kind.List,8,child:Model), F("Material",0xC7EA3CF4,112,Kind.NullPointer), F("Texture",0x6E5D40FB,116,Kind.List,8,child:Texture),
        F("WeaponFireFXBone",0xBD39D1A1,124,Kind.List,8,child:Bone), F("WeaponRecoilBone",0xBD39D1A1,132,Kind.List,8,child:Bone), F("WeaponMuzzleFlash",0xBD39D1A1,140,Kind.List,8,child:Bone), F("WeaponLaunchBone",0xBD39D1A1,148,Kind.List,8,child:Bone),
        F("ParticleSysBone",0x98C53FE6,156,Kind.List,8,child:Particle), F("FXEvent",0x75991AD3,164,Kind.EmptyList,8), F("ShadowInfo",0x82FE7E88,172,Kind.NullPointer), F("Turret",0xF3095903,176,Kind.EmptyList,8), F("SubObject",0x03A28C40,184,Kind.EmptyList,8) }, 193, 3);
    private static readonly Record AnimationState = new("AnimationState", 140, new[] {
        F("ParseCondStateType",0x1323B870,0,Kind.Enum,limit:3), F("AnimNickName",0x1A2933AA,4,Kind.String,8), F("ConditionsYes",0x67DBBFC9,12,Kind.Flags,60,463), F("Name",0x1A2933AA,72,Kind.String,8), F("StateName",0x1A2933AA,80,Kind.String,8),
        F("Flags",0x5EC2B352,88,Kind.Flags,4,12), F("ShareAnimation",0x77E3F4F7,136,Kind.Bool,1), F("EnteringStateFX",0x0D1C8E83,92,Kind.Ref), F("FrameForPristineBonePositions",0x23E650B8,96),
        F("AllowRepeatInRandomPick",0x77E3F4F7,137,Kind.Bool,1), F("SimilarRestart",0x77E3F4F7,138,Kind.Bool,1), F("Animation",0x58A175D9,100,Kind.List,8,child:Animation), F("Script",0x1847C490,108,Kind.StringPointer),
        F("FXEvent",0x75991AD3,112,Kind.EmptyList,8), F("LuaEvent",0xDA05EB72,120,Kind.EmptyList,8), F("ParticleSysBone",0x98C53FE6,128,Kind.List,8,child:Particle) }, 139, 1);
    private static readonly Record Root = new("W3DScriptedModelDrawModuleData", 216, new[] {
        F("Name",0x1A2933AA,8,Kind.String,8), F("InitialRecoilSpeed",0x7A76C60E,16,Kind.Real), F("MaxRecoilDistance",0x645864F3,20,Kind.Real), F("RecoilDamping",0x645864F3,24,Kind.Real), F("RecoilSettleSpeed",0x7A76C60E,28,Kind.Real),
        F("OkToChangeModelColor",0x77E3F4F7,196,Kind.Bool,1), F("AnimationsRequirePower",0x77E3F4F7,197,Kind.Bool,1), F("UseYAxisForTurretRotation",0x77E3F4F7,198,Kind.Bool,1), F("MinLODRequired",0xA80D9F50,32,Kind.Enum,limit:2),
        F("ProjectileBoneFeedbackEnabledSlots",0x28AEDB15,36,Kind.Flags,4,6), F("TrackMarks",0x4C3E3938,40,Kind.Ref), F("TrackMarksOnlyWhenCorneringQuickly",0x77E3F4F7,199,Kind.Bool,1), F("ExtraPublicBone",0x6D66AE18,44,Kind.Strings,8),
        F("AttachToBoneInAnotherModule",0x1A2933AA,52,Kind.String,8), F("DependencySharedModelFlags",0x67DBBFC9,60,Kind.Flags,60,463), F("UseProducerTexture",0x77E3F4F7,200,Kind.Bool,1), F("NoRotate",0x77E3F4F7,201,Kind.Bool,1),
        F("UseFiringArcRotation",0x77E3F4F7,202,Kind.Bool,1), F("Selectable",0x77E3F4F7,203,Kind.Bool,1), F("RandomTextureFixedRandomIndex",0x77E3F4F7,204,Kind.Bool,1), F("TrackMarksLeftBone",0x1A2933AA,120,Kind.String,8), F("TrackMarksRightBone",0x1A2933AA,128,Kind.String,8),
        F("ParticleBonesCheckDrawable",0x77E3F4F7,205,Kind.Bool,1), F("ShadowForceDisable",0x77E3F4F7,206,Kind.Bool,1), F("HighDetailLODThreshold",0x645864F3,136,Kind.Real), F("LowDetailLODThreshold",0x645864F3,140,Kind.Real),
        F("SwitchModelLODMode",0x77E3F4F7,207,Kind.Bool,1), F("StaticModelLODMode",0x77E3F4F7,208,Kind.Bool,1), F("ShowShadowWhileContained",0x77E3F4F7,209,Kind.Bool,1), F("WadingParticleSys",0x43F1CE36,144,Kind.Ref),
        F("AlphaCameraFadeOuterRadius",0x645864F3,148,Kind.Real), F("AlphaCameraFadeInnerRadius",0x645864F3,152,Kind.Real), F("AlphaCameraAtInnerRadius",0x645864F3,156,Kind.Real), F("BirthFadeTime",0x23E650B8,160),
        F("MultiPlayerOnly",0x77E3F4F7,210,Kind.Bool,1), F("AffectedByStealth",0x77E3F4F7,211,Kind.Bool,1), F("InvertStealthOpacity",0x77E3F4F7,212,Kind.Bool,1), F("HighDetailOnly",0x77E3F4F7,213,Kind.Bool,1),
        F("ModelConditionState",0x953CF093,164,Kind.List,8,child:ModelState), F("AnimationState",0x99B342C4,172,Kind.List,8,child:AnimationState), F("AttachModel",0xB4E523BF,180,Kind.EmptyList,8), F("EmbedPortal",0xAFC5E3E3,188,Kind.EmptyList,8) }, 214, 2, Draw, 0x17F61728);

    //-------------------------------------------------------------------------------------------------
    /** Reborn: reject malformed/unreviewed graphs before producing any metadata, then rebase native pointers/slots once while preserving import occurrence values. */
    //-------------------------------------------------------------------------------------------------
    public static Chunk Tokenize(Chunk native)
    {
        if (native?.InstanceBuffer == null || native.RelocationBuffer == null || native.ImportsBuffer == null || native.InstanceBuffer.Length < 216 || native.InstanceBuffer.Length > 2 * 1048576) throw new InvalidDataException("Bounded scripted native graph required.");
        var builder = new Builder(native.InstanceBuffer); return builder.Build(native);
    }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: descriptors keep native offsets, wire tags and optional child policies explicit rather than guessing metadata from managed names. */
    //-------------------------------------------------------------------------------------------------
    private static Field F(string name, uint tag, int offset, Kind kind = Kind.Int, int width = 4, int limit = 0, Record child = null) => new(name, tag, offset, kind, width, limit, child);

    // Reborn: one invocation owns all metadata rows, patches and native allocation inventories; no static mutable compilation state is shared.
    private sealed class Builder
    {
        private readonly byte[] bytes;
        private readonly List<uint> metadata = new();
        private readonly List<(int Word, int Target)> nativeTargets = new();
        private readonly SortedSet<uint> relos = new(), imports = new();
        private readonly HashSet<(int Start, int Length)> sharedText = new();
        private readonly bool[] covered;
        private int visits;
        //-------------------------------------------------------------------------------------------------
        /** Reborn: allocation state is local to this source graph and cannot retain stock addresses. */
        //-------------------------------------------------------------------------------------------------
        internal Builder(byte[] bytes) { this.bytes = bytes; covered = new bool[bytes.Length]; }
        //-------------------------------------------------------------------------------------------------
        /** Reborn: finalize only after full native ownership/auxiliary validation; metadata native-target words and pointer payloads use distinct patch lists. */
        //-------------------------------------------------------------------------------------------------
        internal Chunk Build(Chunk native)
        {
            if (Word(0) != 0x4ECF2A4B || Word(4) == 0) throw new InvalidDataException("Scripted module type/local identity differs.");
            Claim(0, 216); int wrapper = Allocate(1); Row(wrapper, 2, 0x4ECF2A4B, 12); int root = Emit(Root, 0, 0); if (root != 12) throw new InvalidDataException("Scripted root metadata origin differs.");
            var suppliedRelos = Slots(native.RelocationBuffer); var suppliedImports = Slots(native.ImportsBuffer);
            if (!suppliedRelos.Order().SequenceEqual(relos) || !suppliedImports.Order().SequenceEqual(imports)) throw new InvalidDataException("Scripted auxiliary ownership differs.");
            int gap = 0; for (int i = 0; i < bytes.Length; i++) { if (covered[i]) { gap = 0; continue; } if (bytes[i] != 0 || ++gap > 3) throw new InvalidDataException("Scripted unowned storage at " + i); }
            int prefix = checked(metadata.Count * 4); var output = new Chunk { InstanceBuffer = new byte[prefix + bytes.Length], RelocationBuffer = Pack(suppliedRelos.Select(s => s + (uint)prefix)), ImportsBuffer = Pack(suppliedImports.Select(s => s + (uint)prefix)) };
            for (int i = 0; i < metadata.Count; i++) Put(output.InstanceBuffer, i * 4, metadata[i]);
            foreach (var patch in nativeTargets) Put(output.InstanceBuffer, patch.Word * 4, checked((uint)(prefix + patch.Target)));
            bytes.CopyTo(output.InstanceBuffer, prefix); foreach (uint slot in suppliedRelos) Put(output.InstanceBuffer, prefix + (int)slot, checked(Word((int)slot) + (uint)prefix)); return output;
        }
        //-------------------------------------------------------------------------------------------------
        /** Reborn: base tables precede child tables; each list reserves all item markers before recursively emitting its ordered record tables. */
        //-------------------------------------------------------------------------------------------------
        private int Emit(Record descriptor, int native, int depth)
        {
            if (depth > 32 || ++visits > 100000) throw new InvalidDataException("Scripted metadata traversal bound exceeded.");
            Range(native, descriptor.Size); for (int i = 0; i < descriptor.PaddingLength; i++) if (bytes[native + descriptor.PaddingAt + i] != 0) throw new InvalidDataException("Scripted native padding differs: " + descriptor.Name);
            int table = Allocate(descriptor.Fields.Length + (descriptor.Parent != null ? 1 : 0) + 1), next = table;
            if (descriptor.Parent != null) { Row(next, 1, descriptor.ParentTag, (uint)Emit(descriptor.Parent, native, depth + 1)); next += 12; }
            foreach (var field in descriptor.Fields)
            {
                int slot = checked(native + field.Offset); Range(slot, field.Width); Row(next, FastHash.GetHashCode(field.Name), field.Tag, 0);
                switch (field.Kind)
                {
                    case Kind.List: case Kind.Strings: case Kind.EmptyList:
                        uint count = Word(slot), pointer = Word(slot + 4); if (count > 4096 || (count == 0) != (pointer == 0)) throw new InvalidDataException("Scripted list count/pointer differs: " + field.Name);
                        if (field.Kind == Kind.EmptyList && count != 0) throw new InvalidDataException("Scripted nonempty child family is not token-enabled: " + field.Name);
                        int list = Allocate(checked(1 + (int)count)); Row(list, 0x6C827073, 0x5D448FD8, 0); Native(list + 8, slot); metadata[(next + 8) / 4] = (uint)list;
                        if (count != 0)
                        {
                            int stride = field.Kind == Kind.Strings ? 8 : field.Child.Size; int items = Offset(pointer); Claim(items, checked((int)count * stride)); relos.Add((uint)(slot + 4));
                            for (int i = 0; i < count; i++) { int item = items + i * stride, marker = list + 12 + i * 12; Row(marker, 0xB151A0EB, 0, 0); if (field.Kind == Kind.Strings) { Text(item); Native(marker + 8, item); } else metadata[(marker + 8) / 4] = (uint)Emit(field.Child, item, depth + 1); }
                        }
                        break;
                    case Kind.String: Text(slot); Native(next + 8, slot); break;
                    case Kind.StringPointer:
                        if (Word(slot) != 0) { int target = Offset(Word(slot)); Claim(target, 8); relos.Add((uint)slot); Text(target); Native(next + 8, target); } break;
                    case Kind.NullPointer: if (Word(slot) != 0) throw new InvalidDataException("Scripted optional child family is not token-enabled: " + field.Name); break;
                    default:
                        if (field.Kind == Kind.Bool && bytes[slot] > 1 || field.Kind == Kind.Enum && Word(slot) >= field.Limit) throw new InvalidDataException("Scripted enum/bool differs: " + field.Name);
                        if (field.Kind == Kind.Real && !float.IsFinite(BitConverter.Int32BitsToSingle(unchecked((int)Word(slot))))) throw new InvalidDataException("Scripted finite real required: " + field.Name);
                        if (field.Kind == Kind.Flags && field.Limit % 32 != 0 && (Word(slot + field.Width - 4) >> (field.Limit % 32)) != 0) throw new InvalidDataException("Scripted undefined flag bits: " + field.Name);
                        if (field.Kind == Kind.Ref && Word(slot) != 0) imports.Add((uint)slot); Native(next + 8, slot); break;
                }
                next += 12;
            }
            return table;
        }
        //-------------------------------------------------------------------------------------------------
        /** Reborn: strings own their exact ASCII/NUL payload; null and allocated-empty remain distinct and every nonnull pointer requires a relocation. */
        //-------------------------------------------------------------------------------------------------
        private void Text(int header)
        {
            Range(header, 8); uint length = Word(header), pointer = Word(header + 4); if (length > 65536 || pointer == 0 && length != 0) throw new InvalidDataException("Scripted text header differs."); if (pointer == 0) return;
            int start = Offset(pointer); Claim(start, checked((int)length + 1), true); for (int i = 0; i < length; i++) if (bytes[start + i] == 0 || bytes[start + i] >= 128) throw new InvalidDataException("Scripted ASCII/non-NUL text required."); if (bytes[start + (int)length] != 0) throw new InvalidDataException("Scripted text terminator differs."); relos.Add((uint)(header + 4));
        }
        //-------------------------------------------------------------------------------------------------
        /** Reborn: independent native owners cannot overlap; only identical already-owned text spans may be shared. */
        //-------------------------------------------------------------------------------------------------
        private void Claim(int start, int length, bool text = false)
        {
            Range(start, length); if (text && sharedText.Contains((start, length))) return;
            for (int i = start; i < start + length; i++) if (covered[i]) throw new InvalidDataException("Scripted native owners overlap.");
            Array.Fill(covered, true, start, length); if (text) sharedText.Add((start, length));
        }
        //-------------------------------------------------------------------------------------------------
        /** Reborn: all metadata allocations are bounded zero-initialized complete rows, including required table terminators. */
        //-------------------------------------------------------------------------------------------------
        private int Allocate(int rows) { if (rows < 1 || rows > (1048576 - metadata.Count) / 3) throw new InvalidDataException("Scripted metadata allocation bound exceeded."); int result = metadata.Count * 4; for (int i = 0; i < rows * 3; i++) metadata.Add(0); return result; }
        //-------------------------------------------------------------------------------------------------
        /** Reborn: metadata-only addresses remain unshifted by the final native origin. */
        //-------------------------------------------------------------------------------------------------
        private void Row(int at, uint name, uint tag, uint target) { metadata[at / 4] = name; metadata[at / 4 + 1] = tag; metadata[at / 4 + 2] = target; }
        //-------------------------------------------------------------------------------------------------
        /** Reborn: native metadata targets are patched only after the complete prefix extent is known. */
        //-------------------------------------------------------------------------------------------------
        private void Native(int at, int target) { Range(target, 1); nativeTargets.Add((at / 4, target)); }
        //-------------------------------------------------------------------------------------------------
        /** Reborn: bounds checks happen before converting serialized unsigned offsets to managed indices. */
        //-------------------------------------------------------------------------------------------------
        private int Offset(uint offset) { if (offset >= bytes.Length) throw new InvalidDataException("Scripted native offset out of bounds."); return (int)offset; }
        //-------------------------------------------------------------------------------------------------
        /** Reborn: no pointer-like word is dereferenced outside the owned native byte buffer. */
        //-------------------------------------------------------------------------------------------------
        private void Range(int at, int length) { if (at < 0 || length < 0 || at > bytes.Length - length) throw new InvalidDataException("Scripted native range exceeded."); }
        //-------------------------------------------------------------------------------------------------
        /** Reborn: all scalar/count/pointer words are bounded little-endian reads. */
        //-------------------------------------------------------------------------------------------------
        private uint Word(int at) { Range(at, 4); return BinaryPrimitives.ReadUInt32LittleEndian(bytes.AsSpan(at, 4)); }
    }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: malformed sentinels, duplicate slots and out-of-range serialized indices cannot be normalized into a valid auxiliary inventory. */
    //-------------------------------------------------------------------------------------------------
    private static uint[] Slots(byte[] bytes)
    {
        if (bytes.Length == 0) return Array.Empty<uint>(); if (bytes.Length % 4 != 0 || BinaryPrimitives.ReadUInt32LittleEndian(bytes.AsSpan(bytes.Length - 4, 4)) != uint.MaxValue) throw new InvalidDataException("Scripted auxiliary sentinel differs.");
        uint[] slots = Enumerable.Range(0, bytes.Length / 4 - 1).Select(i => BinaryPrimitives.ReadUInt32LittleEndian(bytes.AsSpan(i * 4, 4))).ToArray(); if (slots.Distinct().Count() != slots.Length) throw new InvalidDataException("Scripted duplicate auxiliary slot."); return slots;
    }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: empty auxiliary data has no sentinel; populated output preserves input occurrence order. */
    //-------------------------------------------------------------------------------------------------
    private static byte[] Pack(IEnumerable<uint> slots) { var values = slots.ToArray(); if (values.Length == 0) return Array.Empty<byte>(); var bytes = new byte[(values.Length + 1) * 4]; for (int i = 0; i < values.Length; i++) Put(bytes, i * 4, values[i]); Put(bytes, values.Length * 4, uint.MaxValue); return bytes; }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: pointer rebasing rewrites only newly owned output arrays. */
    //-------------------------------------------------------------------------------------------------
    private static void Put(byte[] bytes, int at, uint value) => BinaryPrimitives.WriteUInt32LittleEndian(bytes.AsSpan(at, 4), value);
}
