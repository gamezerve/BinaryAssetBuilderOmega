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
    // Reborn: both airfield families use EP1-specific native models and stock-qualified inherited metadata.
    private static readonly bool AirfieldFamilies=Families.Add("RecallUnitsSpecialPowerModuleData") & Families.Add("ParkingPlaceBehaviorModuleData");
    //-------------------------------------------------------------------------------------------------
    /** Reborn: raw source class admission uses only the already stock-qualified behavior family set; three EA leaf aliases differ from their native class names. */
    //-------------------------------------------------------------------------------------------------
    // Reborn: the existing stock-qualified sell ABI also participates in composed behavior metadata and scalar discovery.
    // Reborn: EP1 automatic deposits use the captured transaction-list ABI and metadata signatures.
    // Reborn: the slaughter container retains its existing reviewed native inheritance and gains source-sized metadata.
    // Reborn: ProductionUpdateInfo is an eight-byte identity-only behavior in the Uprising schema.
    // Reborn: self-replacement retains its existing Uprising writer and concrete wire identity.
    // Reborn: the retail wall-builder ABI participates in ordinary source-driven behavior metadata.
    // Reborn: timer monitors retain their reviewed power import and inline model-condition mask.
    // Reborn: RadarBehavior uses the retail-qualified CreateModuleData identity layout.
    // Reborn: qualify the complete EP1 power-manager UpdateModuleData chain and EVA import.
    // Reborn: production bonuses preserve all inherited SpecialPower metadata plus their six percentage fields.
    // Reborn: StoreObjects retains its complete SpecialPower chain, OCL import and weak marker identity.
    // Reborn: teleport storage retains its inherited SpecialPower metadata and all authored own fields.
    // Reborn: teleport updates include timing, filters, FX and optional shader metadata.
    // Reborn: OCL powers use the EP1 native record and source-sized upgrade lists.
    // Reborn: player upgrade powers retain their existing SpecialPower layout and source-sized strong upgrade list.
    // Reborn: template-ID stores retain the retail-qualified identity-only BehaviorModuleData chain.
    // Reborn: weapon-fire abilities retain all EP1 fields and their SpecialAbility metadata chain.
    // Reborn: attack notification metadata includes its Update inheritance and optional ObjectFilter graph.
    // Reborn: EP1 subobject upgrades use source-sized nuggets and their inherited upgrade metadata.
    // Reborn: transport containers share the reviewed schema/native metadata dispatcher.
    // Reborn: attribute modifier upgrades use the reviewed inherited upgrade metadata dispatcher.
    // Reborn: passenger-ejection abilities retain their real EP1 concrete and inherited metadata classes.
    // Reborn: attack-assist responses use the independently qualified EP1 behavior metadata chain.
    // Reborn: garrison containment retains its original concrete wire identity and full OpenContain inheritance.
    // Reborn: EP1 invisibility behaviors retain their required template reference and behavior metadata inheritance.
    // Reborn: collision behavior is qualified against both Flak Trooper and Grinder Tank retail graphs.
    // Reborn: the Engineer assembly family is qualified against its complete original EP1 record.
    // Reborn: emotion tracking uses EP1 strong references and its independently qualified field layout.
    // Reborn: capture abilities retain their concrete EP1 class identity and complete inherited ability record.
    // Reborn: passive healing and modifier graphs use the complete independently qualified Uprising record.
    // Reborn: repeated firing uses its independently qualified EP1 root and anonymous nugget layout.
    // Reborn: stream-state updates retain their retail-qualified identity and signed StreamId field.
    // Reborn: slave watchers keep their retail field order: upgrade imports, status validation pointer, four booleans.
    // Reborn: laser states use the restored RA3 header plus EP1 RequiresWeapon, qualified from three original laser units.
    // Reborn: OCL monitors store a one-word option mask and an inline status mask (44 bytes).
    // Reborn: physics-world updates use their independently qualified EP1 native model.
    static Ep1BehaviorMetadataTokenizer(){Families.Add("DynamicsUpdateModuleData");Families.Add("OCLMonitorUpdateModuleData");Families.Add("LaserStateModuleData");Families.Add("SlaveWatcherBehaviorModuleData");Families.Add("StreamStateModuleData");Families.Add("FireWeaponUpdateModuleData");Families.Add("PassiveAreaEffectBehaviorModuleData");Families.Add("CaptureSpecialAbilityUpdateModuleData");Families.Add("EmotionTrackerUpdateModuleData");Families.Add("AssembleStructureSpecialAbilityUpdateModuleData");Families.Add("FireWeaponCollideModuleData");Families.Add("AimWeaponBehaviorModuleData");Families.Add("StealthDetectorUpdateModuleData");Families.Add("AttributeModifierAuraUpdateModuleData");Families.Add("InvisibilityBehaviorModuleData");Families.Add("GarrisonContainModuleData");Families.Add("AttackAssistResponseBehaviorModuleData");Families.Add("EjectPassengersSpecialAbilityUpdateModuleData");Families.Add("AttributeModifierUpgradeModuleData");Families.Add("TransportContainModuleData");Families.Add("SubObjectsUpgradeModuleData");Families.Add("NotifyWhenAttackingBehaviorModuleData");Families.Add("WeaponFireSpecialAbilityUpdateModuleData");Families.Add("StoreTemplateIDBehaviorModuleData");Families.Add("PlayerUpgradeSpecialPowerModuleData");Families.Add("OCLSpecialPowerModuleData");Families.Add("TeleportStoredObjectsUpdateModuleData");Families.Add("TeleportStoredObjectsSpecialPowerModuleData");Families.Add("StoreObjectsSpecialPowerModuleData");Families.Add("GiveProductionBonusSpecialPowerModuleData");Families.Add("PlayerPowerManagerBehaviorModuleData");Families.Add("RadarBehaviorModuleData");Families.Add("MonitorSpecialPowerTimerUpdateModuleData");Families.Add("WallBuilderBehaviorModuleData");Families.Add("ReplaceSelfSpecialAbilityModuleData");Families.Add("ProductionUpdateInfoModuleData");Families.Add("SlaughterHordeContainModuleData");Families.Add("AutoDepositUpdateModuleData");Families.Add("StructureSellUpdateModuleData");Families.Add("BuildingDestructionModuleData");Families.Add("QueueDockModuleData");Families.Add("QueueProductionExitUpdateModuleData");Families.Add("SpawnBehaviorModuleData");Families.Add("StructureUnpackUpdateModuleData");Families.Add("ProductionUpdateModuleData");Families.Add("RepairUpdateModuleData");Families.Add("EngineerContainModuleData");Families.Add("AssistedPlacementBehaviorModuleData");Families.Add("InfiltratorContainModuleData");Families.Add("StructurePlacementBehaviorModuleData");Families.Add("LUAEventListBehaviorModuleData");Families.Add("PowerUpgradeModuleData");Families.Add("BuildingBehaviorModuleData");Families.Add("AssignSlavesTargetObjectSpecialPowerModuleData");}

    // Reborn: assign-slaves uses the existing native SpecialPower writer and independent stock-qualified metadata proof.
    public static string ReviewedWireName(string sourceName)
    {string wire=sourceName switch{"AimWeapon"=>"AimWeaponBehaviorModuleData","PlayerPowerManager"=>"PlayerPowerManagerBehaviorModuleData","QueueDockBehavior"=>"QueueDockModuleData","BuildingDestructionBehavior"=>"BuildingDestructionModuleData","FireWeaponWhenDead"=>"FireWeaponWhenDeadBehaviorModuleData","Physics"=>"PhysicsBehaviorModuleData","SlowDeath"=>"SlowDeathBehaviorModuleData","LUAEventList"=>"LUAEventListBehaviorModuleData",_=>sourceName+"ModuleData"};if(!Families.Contains(wire))throw new InvalidDataException("Behavior source family not reviewed: "+sourceName);return wire;}
    //-------------------------------------------------------------------------------------------------
    /** Reborn: shared compilation/composition uses one explicit wire-to-native-model mapping, including all seven isolated EP1 records. */
    //-------------------------------------------------------------------------------------------------
    public static Type ReviewedNativeType(uint wire)
    {var names=Families.Where(n=>FastHash.GetHashCode(n)==wire).ToArray();if(names.Length!=1)throw new InvalidDataException("Behavior wire family not reviewed.");string name=names[0];string model=name is "FireWeaponUpdateModuleData" or "PassiveAreaEffectBehaviorModuleData" or "EmotionTrackerUpdateModuleData" or "FireWeaponCollideModuleData" or "StealthDetectorUpdateModuleData" or "AttributeModifierAuraUpdateModuleData" or "SubObjectsUpgradeModuleData" or "WeaponFireSpecialAbilityUpdateModuleData" or "OCLSpecialPowerModuleData" or "PlayerPowerManagerBehaviorModuleData" or "AutoDepositUpdateModuleData" or "EngineerContainModuleData" or "RepairUpdateModuleData" or "ProductionUpdateModuleData" or "StructureUnpackUpdateModuleData" or "SpawnBehaviorModuleData" or "QueueProductionExitUpdateModuleData" or "WeaponSetUpdateModuleData" or "SquishCollideModuleData" or "PhysicsBehaviorModuleData" or "HitReactionBehaviorModuleData" or "StatusBitsUpgradeModuleData" or "CreateObjectDieModuleData" or "FireWeaponWhenDeadBehaviorModuleData" or "RecallUnitsSpecialPowerModuleData" or "ParkingPlaceBehaviorModuleData"?"Ep1"+name:name;return typeof(SageBinaryData.GameObject).Assembly.GetType("SageBinaryData."+model)??throw new InvalidDataException("Reviewed behavior native model absent.");}

    //-------------------------------------------------------------------------------------------------
    /** Reborn: select a bounded concrete family by its original wire identity; callers cannot submit unreviewed classes through the reflection/schema seam. */
    //-------------------------------------------------------------------------------------------------
    public static Chunk Tokenize(Chunk native, XmlSchemaSet schemas, string wireName, Type nativeType)
    {
        if (native?.InstanceBuffer == null || native.RelocationBuffer == null || native.ImportsBuffer == null || native.InstanceBuffer.Length < 8 || native.InstanceBuffer.Length > 2 * 1048576 || schemas == null || !Families.Contains(wireName) || Word(native.InstanceBuffer, 0) != FastHash.GetHashCode(wireName) || Word(native.InstanceBuffer, 4) == 0) throw new InvalidDataException("Reviewed bounded behavior native graph required.");
        if (nativeType == null || nativeType.Assembly != typeof(SageBinaryData.GameObject).Assembly || (nativeType.Name.StartsWith("Ep1", StringComparison.Ordinal) ? nativeType.Name.Substring(3) : nativeType.Name) != wireName) throw new InvalidDataException("Behavior native model/wire identity differs.");
        // Reborn: populated turrets use schema-sized typed metadata; retain the qualified literal hardpoint-only route.
        if (wireName == "WeaponSetUpdateModuleData" && Word(native.InstanceBuffer,20)==0) return Ep1WeaponSetTokenizer.Tokenize(native);
        return new Builder(native.InstanceBuffer, schemas).Build(native, wireName, nativeType);
    }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: serialize only the independently qualified PropBody class through the shared schema writer; it is not admitted as a Behaviors leaf. */
    //-------------------------------------------------------------------------------------------------
    public static Chunk TokenizePropBody(Chunk native,XmlSchemaSet schemas)
    {
        if(native?.InstanceBuffer==null||native.InstanceBuffer.Length!=8||native.RelocationBuffer==null||native.RelocationBuffer.Length!=0||native.ImportsBuffer==null||native.ImportsBuffer.Length!=0||schemas==null||Word(native.InstanceBuffer,0)!=FastHash.GetHashCode("PropBodyModuleData")||Word(native.InstanceBuffer,4)==0)throw new InvalidDataException("Complete identity-only PropBody input required.");
        return new Builder(native.InstanceBuffer,schemas).Build(native,"PropBodyModuleData",typeof(SageBinaryData.PropBodyModuleData));
    }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: EP1 WeaponTemplate own table (no asset base row) with a polymorphic Nuggets pointer list; the caller maps each native TypeId to its reviewed schema class and native model. */
    //-------------------------------------------------------------------------------------------------
    public static Chunk TokenizeWeapon(Chunk native,XmlSchemaSet schemas,Func<uint,(string Schema,Type Native)> nugget)
    {
        if(native?.InstanceBuffer==null||native.RelocationBuffer==null||native.ImportsBuffer==null||native.InstanceBuffer.Length<Size(typeof(SageBinaryData.WeaponTemplate))||native.InstanceBuffer.Length>2*1048576||schemas==null||nugget==null)throw new InvalidDataException("Bounded weapon native graph required.");
        return new Builder(native.InstanceBuffer,schemas){Polymorphic=nugget}.BuildRecord(native,"WeaponTemplate",typeof(SageBinaryData.WeaponTemplate));
    }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: reuse schema/native traversal for six explicitly selected non-polymorphic root records; no stock coordinates or arbitrary class admission enter this API. */
    //-------------------------------------------------------------------------------------------------
    public static Chunk TokenizeRootRecord(Chunk native,XmlSchemaSet schemas,string name)
    {
        // Reborn: GameDependency is an optional root record with named GameDependencyType schema/native ABI.
        Type type=name switch{"GameDependency"=>typeof(SageBinaryData.GameDependencyType),"ObjectResourceInfo"=>typeof(SageBinaryData.ObjectResourceInfo),"SkirmishAIInformation"=>typeof(SageBinaryData.Ep1SkirmishAIInformation),"AudioArrayVoice"=>typeof(SageBinaryData.Ep1AudioArrayVoice),"AudioArraySound"=>typeof(SageBinaryData.Ep1AudioArraySound),"EvaEventArray"=>typeof(SageBinaryData.Ep1EvaEventArray),"ShadowInfo"=>typeof(SageBinaryData.ShadowInfo),"CrusherInfo"=>typeof(SageBinaryData.Ep1CrusherInfo),_=>throw new InvalidDataException("Root record family not reviewed.")};
        if(native?.InstanceBuffer==null || native.RelocationBuffer==null || native.ImportsBuffer==null || native.InstanceBuffer.Length<Size(type) || native.InstanceBuffer.Length>2*1048576 || schemas==null)throw new InvalidDataException("Bounded root record required.");
        // Reborn: inactive child headers/pointers are not guessed native ABIs; caller-authored raw buffers cannot bypass source-side refusal.
        if(name=="SkirmishAIInformation" && new[]{64,68}.Any(at=>Word(native.InstanceBuffer,at)!=0))throw new InvalidDataException("SkirmishAIInformation/AIContextualPower native storage is not implemented.");
        // Reborn: source-sized voice arrays are bounded by actual twelve-byte items, not a diagnostic row quota; typed EVA words are imports.
        if(name=="AudioArrayVoice"){uint count=Word(native.InstanceBuffer,0),items=Word(native.InstanceBuffer,4);if(count>native.InstanceBuffer.Length/12 || items>native.InstanceBuffer.Length || count*12>native.InstanceBuffer.Length-items)throw new InvalidDataException("Root voice entries exceed bounds.");}
        return new Builder(native.InstanceBuffer,schemas).BuildRecord(native,name=="GameDependency"?"GameDependencyType":name,type);
    }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: serialize the reviewed root inline list through the same schema/native record writer, with a canonical standalone count/items header. */
    //-------------------------------------------------------------------------------------------------
    public static Chunk TokenizeRootList(Chunk native,XmlSchemaSet schemas,string name)
    {
        if(name!="ProjectedBuildabilityInfo"||native?.InstanceBuffer==null||native.InstanceBuffer.Length<8||native.InstanceBuffer.Length>2*1048576||schemas==null)throw new InvalidDataException("Reviewed root inline-list input required.");
        return new Builder(native.InstanceBuffer,schemas).BuildInlineList(native,name,typeof(SageBinaryData.ProjectedBuildabilityInfo));
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: generate one ordered Behaviors metadata container over one already compiled native/Core graph; only metadata fragments are joined, never independently compiled native payloads. */
    //-------------------------------------------------------------------------------------------------
    public static Chunk TokenizeContainer(Chunk native, XmlSchemaSet schemas)
    {
        if(native?.InstanceBuffer==null || native.RelocationBuffer==null || native.ImportsBuffer==null || schemas==null || native.InstanceBuffer.Length<16 || native.InstanceBuffer.Length>2*1048576)throw new InvalidDataException("Bounded shared behavior graph required.");
        // Reborn: validate the actual item-table extent rather than a sample-specific module count.
        byte[] bytes=native.InstanceBuffer;uint count=Word(bytes,0);if(count==0 || count>(uint)Math.Max(0,(bytes.Length-8)/4) || Word(bytes,4)!=8)throw new InvalidDataException("Nonempty canonical behavior item table is outside its native buffer.");
        int first=checked(8+(int)count*4);if(first>bytes.Length-8 || Word(bytes,8)!=first)throw new InvalidDataException("Behavior item array ownership differs.");
        uint[] relos=Slots(native.RelocationBuffer),imports=Slots(native.ImportsBuffer);var parent=new[]{4u}.Concat(Enumerable.Range(0,(int)count).Select(i=>(uint)(8+i*4))).ToArray();
        if(relos.Concat(imports).Any(s=>s>bytes.Length-4) || relos.Intersect(imports).Any() || !relos.Where(s=>s<first).OrderBy(s=>s).SequenceEqual(parent) || imports.Any(s=>s<first))throw new InvalidDataException("Shared behavior auxiliary parent/range ownership differs.");
        var pieces=new List<(Chunk Data,int Prefix,int Start)>();var ids=new HashSet<uint>();int prefix=checked(12+(int)count*12);
        for(int i=0;i<count;i++)
        {
            int start=checked((int)Word(bytes,8+i*4)),end=i+1<count?checked((int)Word(bytes,12+i*4)):bytes.Length;
            if(start<first || start%4!=0 || end%4!=0 || end<start+8 || end>bytes.Length || !ids.Add(Word(bytes,start+4)))throw new InvalidDataException("Shared behavior module order/identity/bounds differs.");
            uint wire=Word(bytes,start);var names=Families.Where(n=>FastHash.GetHashCode(n)==wire).ToArray();if(names.Length!=1)throw new InvalidDataException("Shared behavior family is not reviewed.");
            var leaf=new Chunk{InstanceBuffer=bytes.AsSpan(start,end-start).ToArray(),RelocationBuffer=Pack(relos.Where(s=>s>=start && s<end).Select(s=>s-(uint)start)),ImportsBuffer=Pack(imports.Where(s=>s>=start && s<end).Select(s=>s-(uint)start))};
            foreach(uint slot in Slots(leaf.RelocationBuffer)){uint target=Word(leaf.InstanceBuffer,(int)slot);if(slot>leaf.InstanceBuffer.Length-4 || target<start || target>=end)throw new InvalidDataException("Shared behavior pointer crosses native owners.");Put(leaf.InstanceBuffer,(int)slot,target-(uint)start);}
            string name=names[0];var piece=Tokenize(leaf,schemas,name,ReviewedNativeType(wire));int length=piece.InstanceBuffer.Length-leaf.InstanceBuffer.Length;prefix=checked(prefix+length);if(prefix>4*1048576)throw new InvalidDataException("Shared behavior metadata bound exceeded.");pieces.Add((piece,length,start));
        }
        var result=new Chunk{InstanceBuffer=new byte[checked(prefix+bytes.Length)],RelocationBuffer=Pack(relos.Select(s=>s+(uint)prefix)),ImportsBuffer=Pack(imports.Select(s=>s+(uint)prefix))};bytes.CopyTo(result.InstanceBuffer,prefix);
        Put(result.InstanceBuffer,0,0x6C827073);Put(result.InstanceBuffer,4,0x5D448FD8);Put(result.InstanceBuffer,8,(uint)prefix);int next=12+(int)count*12;
        for(int i=0;i<pieces.Count;i++)
        {
            var piece=pieces[i];Put(result.InstanceBuffer,12+i*12,0xB151A0EB);Put(result.InstanceBuffer,20+i*12,(uint)next);
            for(int at=0;at<piece.Prefix;at+=12){Put(result.InstanceBuffer,next+at,Word(piece.Data.InstanceBuffer,at));Put(result.InstanceBuffer,next+at+4,Word(piece.Data.InstanceBuffer,at+4));uint target=Word(piece.Data.InstanceBuffer,at+8);if(target!=0){if(target>=piece.Data.InstanceBuffer.Length)throw new InvalidDataException("Shared behavior fragment target escapes its owner.");Put(result.InstanceBuffer,next+at+8,checked(target<piece.Prefix?(uint)next+target:(uint)(prefix+piece.Start)+target-(uint)piece.Prefix));}}next=checked(next+piece.Prefix);
        }
        foreach(uint slot in relos){uint target=Word(bytes,(int)slot);if(target>=bytes.Length)throw new InvalidDataException("Shared behavior native pointer bounds differ.");Put(result.InstanceBuffer,prefix+(int)slot,checked(target+(uint)prefix));}return result;
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
        // Reborn: optional polymorphic pointer-list class resolver (weapon nuggets); absent resolvers keep such lists refused.
        internal Func<uint,(string Schema,Type Native)> Polymorphic;
        //-------------------------------------------------------------------------------------------------
        /** Reborn: weapon tables use the reviewed signature table first; any other named schema type uses its EA type-name hash, and the result is
            accepted only after the caller's stock-learned weapon grammar matches every emitted name/tag row. */
        //-------------------------------------------------------------------------------------------------
        private static uint WeaponSignature(string key)
        {
            if (TryReviewedScalarSignature(key, out uint reviewed)) return reviewed;
            string name = key.EndsWith("[]", StringComparison.Ordinal) ? key[..^2] : key; int colon = name.IndexOf(':'); if (colon > 0 && !name.StartsWith("xs:", StringComparison.Ordinal)) name = name[..colon];
            if (name.Length == 0 || name.StartsWith("xs:", StringComparison.Ordinal)) throw new InvalidDataException("Weapon schema signature not reviewed: " + key);
            return FastHash.GetHashCode(name);
        }
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
            return Finish(native);
        }
        //-------------------------------------------------------------------------------------------------
        /** Reborn: root own-field tables have no synthetic concrete wrapper or ModuleData identity. */
        //-------------------------------------------------------------------------------------------------
        internal Chunk BuildRecord(Chunk native,string schemaName,Type type){Claim(0,Size(type));if(Emit(schemaName,type,0,0)!=0)throw new InvalidDataException("Root metadata own table origin differs.");return Finish(native);}
        //-------------------------------------------------------------------------------------------------
        /** Reborn: repeated root records retain source cardinality, complete nested record tables and exact list relocation ownership. */
        //-------------------------------------------------------------------------------------------------
        internal Chunk BuildInlineList(Chunk native,string schemaName,Type type)
        {
            Claim(0,8);uint count=Word(bytes,0),items=Word(bytes,4);int stride=Size(type);
            if(count>bytes.Length/stride||(count==0)!=(items==0))throw new InvalidDataException("Root inline-list count/items differ.");
            int list=Allocate(checked(1+(int)count));Row(list,0x6C827073,0x5D448FD8,0);Native(list+8,0);
            if(count!=0){int start=Pointer(items);Claim(start,checked((int)count*stride));relos.Add(4);for(int i=0;i<count;i++){int row=list+12+i*12;Row(row,0xB151A0EB,0,(uint)Emit(schemaName,type,start+i*stride,0));}}
            return Finish(native);
        }
        //-------------------------------------------------------------------------------------------------
        /** Reborn: both reviewed envelope forms share the exact ownership, native-tail and auxiliary transformation. */
        //-------------------------------------------------------------------------------------------------
        private Chunk Finish(Chunk native)
        {
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
        private int Emit(string schemaName, Type type, int native, int depth, XmlSchemaComplexType resolved = null)
        {
            if (depth > 32 || ++visits > 100000) throw new InvalidDataException("Behavior metadata traversal bound exceeded."); Range(native, Size(type));
            var schema = resolved ?? schemas.GlobalTypes[new XmlQualifiedName(schemaName, Ns)] as XmlSchemaComplexType ?? throw new InvalidDataException("Behavior complex schema missing: " + schemaName);
            var extension = (schema.ContentModel as XmlSchemaComplexContent)?.Content as XmlSchemaComplexContentExtension;
            if (schema.ContentModel != null && extension == null) throw new InvalidDataException("Unreviewed behavior schema content: " + schemaName);
            var fields = new List<(string Name, XmlSchemaType Schema, string Reference, bool Repeated)>(); var attributes = extension?.Attributes ?? schema.Attributes;
            if (schemaName is "ModuleData" or "WeaponSlot_Hardpoint" or "WeaponEffectNugget") fields.Add(("TypeId", null, "", false));
            foreach (XmlSchemaObject item in attributes) { if (item is not XmlSchemaAttribute a || a.Name == null || a.AttributeSchemaType == null) throw new InvalidDataException("Unreviewed behavior attribute declaration.");if(a.UnhandledAttributes?.Any(x=>x.LocalName=="pipelineOnly" && x.NamespaceURI=="uri:ea.com:eala:asset:schema" && x.Value=="true")==true)continue; fields.Add((a.Name, a.AttributeSchemaType, Reference(a.UnhandledAttributes), false)); }
            Elements(extension?.Particle ?? schema.Particle, fields);
            // Reborn: stock EP1 weapon roots have no BaseInheritableAsset row; the 4-byte asset base word stays zero and is owned here.
            bool assetRoot = depth == 0 && schemaName == "WeaponTemplate" && Polymorphic != null;
            bool parent = extension != null && !assetRoot; int table = Allocate(fields.Count + (parent ? 1 : 0) + 1), next = table; var used = new bool[Size(type)];
            if (assetRoot) { var assetBase = type.GetField("Base") ?? throw new InvalidDataException("Weapon native base missing."); if (Offset(type, assetBase) != 0 || Size(assetBase.FieldType) != 4 || Word(bytes, native) != 0) throw new InvalidDataException("Weapon asset base word differs."); Mark(used, 0, 4); }
            if (parent)
            {
                var baseField = type.GetField("Base") ?? throw new InvalidDataException("Behavior native base missing: " + type.Name); if (Offset(type, baseField) != 0) throw new InvalidDataException("Behavior native base is not first.");
                Mark(used, 0, Size(baseField.FieldType)); Row(next, 1, FastHash.GetHashCode(extension.BaseTypeName.Name), (uint)Emit(extension.BaseTypeName.Name, baseField.FieldType, native, depth + 1)); next += 12;
            }
            foreach (var field in fields)
            {
                var info = type.GetField(field.Name) ?? throw new InvalidDataException("Behavior native field missing: " + schemaName + "." + field.Name); int offset = Offset(type, info); Mark(used, offset, Size(info.FieldType));
                string wire = field.Schema?.QualifiedName.Name;
                // Reborn: EP1 names an inline anonymous element record "Owner::Element" (stock RepairUpdate witness); named types keep their reviewed signatures.
                string anonymous = field.Schema is XmlSchemaComplexType && string.IsNullOrEmpty(wire) ? schemaName + "::" + field.Name : null;
                if (field.Name != "TypeId" && anonymous == null && string.IsNullOrEmpty(wire)) throw new InvalidDataException("Behavior anonymous field wire type: " + schemaName + "." + field.Name);
                // Reborn: field signatures use reviewed type/reference/list identities, not plain concrete-class name hashing.
                if (field.Schema?.QualifiedName.Namespace == XmlSchema.Namespace) wire = "xs:" + wire;
                if (field.Reference.Length != 0) wire += ":" + field.Reference;
                if (field.Repeated) wire += "[]";
                Row(next, FastHash.GetHashCode(field.Name), field.Name == "TypeId" ? 0x23E650B8 : anonymous != null ? FastHash.GetHashCode(anonymous) : Polymorphic != null ? WeaponSignature(wire) : Signature(wire), 0);
                // Reborn: empty SoundOrEvaEvent.EvaEvent is a strong-reference zero word in EP1 metadata, not a missing String pointee; populated legacy pointer storage stays refused.
                if(schemaName=="SoundOrEvaEvent" && field.Name=="EvaEvent"&&info.FieldType.IsPointer){if(Word(bytes,native+offset)!=0)throw new InvalidDataException("Populated legacy voice EvaEvent ABI is not implemented.");Native(next+8,native+offset);}else Field(field.Schema, info.FieldType, checked(native + offset), next + 8, depth + 1, anonymous);next += 12;
            }
            foreach (var info in type.GetFields(BindingFlags.Public | BindingFlags.Instance)) { int at = Offset(type, info); if (Enumerable.Range(at, Size(info.FieldType)).Any(i => !used[i])) throw new InvalidDataException("Behavior native member is not schema-enabled: " + schemaName + "." + info.Name); }
            for (int i = 0; i < used.Length; i++) if (!used[i] && bytes[native + i] != 0) throw new InvalidDataException("Behavior nonzero native padding."); return table;
        }
        //-------------------------------------------------------------------------------------------------
        /** Reborn: pointer, list, string and reference representations are derived from actual native types; schemas supply field order and wire tags only. */
        //-------------------------------------------------------------------------------------------------
        private void Field(XmlSchemaType schema, Type type, int slot, int targetAt, int depth, string anonymous = null)
        {
            // Reborn: update shape quaternions use the retail VectorHack native target rather than a synthetic component token table.
            if(schema?.Name=="Quaternion"&&type==typeof(SageBinaryData.Ep1DynamicsQuaternion)){Range(slot,16);for(int axis=0;axis<4;axis++)if(!float.IsFinite(BitConverter.Int32BitsToSingle(unchecked((int)Word(bytes,slot+axis*4)))))throw new InvalidDataException("Dynamics quaternion is not finite.");Native(targetAt,slot);return;}
            // Reborn: EP1 VectorHack metadata points straight to three inline float words; the stock queue-exit witness proves this is not a child token table.
            if(schema?.Name=="Vector3"&&type==typeof(SageBinaryData.Coord3D)){
                Range(slot,12);for(int axis=0;axis<3;axis++)if(!float.IsFinite(BitConverter.Int32BitsToSingle(unchecked((int)Word(bytes,slot+axis*4)))))throw new InvalidDataException("Behavior inline vector is not finite.");Native(targetAt,slot);return;
            }
            if (type.IsPointer)
            {
                uint pointer = Word(bytes, slot); if (pointer == 0) return; int target = Pointer(pointer), size = Size(type.GetElementType()); Claim(target, size); relos.Add((uint)slot); Field(schema, type.GetElementType(), target, targetAt, depth + 1, anonymous); return;
            }
            if (type.IsGenericType && type.GetGenericTypeDefinition().FullName == "Relo.PolymorphicList`1" && Polymorphic != null)
            {
                // Reborn: each pointer item is wrapped by its concrete class row; the class comes from the native TypeId word, never from position.
                uint count = Word(bytes, slot), pointer = Word(bytes, slot + 4); if (count > bytes.Length / 4 || (count == 0) != (pointer == 0)) throw new InvalidDataException("Polymorphic metadata list invariant differs.");
                int list = Allocate(1 + (int)count); Row(list, 0x6C827073, 0x5D448FD8, 0); Native(list + 8, slot); Target(targetAt, list);
                if (count != 0)
                {
                    int items = Pointer(pointer); Claim(items, checked((int)count * 4)); relos.Add((uint)(slot + 4));
                    for (int i = 0; i < count; i++)
                    {
                        int itemSlot = items + i * 4; uint value = Word(bytes, itemSlot); if (value == 0) throw new InvalidDataException("Polymorphic metadata item is null.");
                        int target = Pointer(value); Range(target, 4); var concrete = Polymorphic(Word(bytes, target)); Claim(target, Size(concrete.Native)); relos.Add((uint)itemSlot);
                        int marker = list + 12 + i * 12, wrapper = Allocate(1); Row(marker, 0xB151A0EB, 0, (uint)wrapper);
                        Row(wrapper, 2, FastHash.GetHashCode(concrete.Schema), (uint)Emit(concrete.Schema, concrete.Native, target, depth + 1));
                    }
                }
                return;
            }
            if (type.IsGenericType && type.GetGenericTypeDefinition().FullName == "Relo.List`1")
            {
                // Reborn: list cardinality is bounded by its real native item extent, not the former 4096-entry sample quota.
                Type child = type.GetGenericArguments()[0]; int stride = Size(child);
                uint count = Word(bytes, slot), pointer = Word(bytes, slot + 4); if (count > bytes.Length/stride || (count == 0) != (pointer == 0)) throw new InvalidDataException("Behavior metadata list invariant differs.");
                int list = Allocate(1 + (int)count); Row(list, 0x6C827073, 0x5D448FD8, 0); Native(list + 8, slot); Target(targetAt, list);
                if (count != 0)
                {
                    int items = Pointer(pointer); Claim(items, checked((int)count * stride)); relos.Add((uint)(slot + 4));
                    XmlSchemaType itemSchema = schema is XmlSchemaSimpleType simple && simple.Content is XmlSchemaSimpleTypeList listType ? listType.BaseItemType : schema;
                    // Reborn: retain Owner::Element identity when a repeated anonymous record is emitted through its list item.
                    for (int i = 0; i < count; i++) { int marker = list + 12 + i * 12; Row(marker, 0xB151A0EB, 0, 0); Field(itemSchema, child, items + i * stride, marker + 8, depth + 1, anonymous); }
                }
                return;
            }
            if (schema is XmlSchemaComplexType complex) { Target(targetAt, Emit(string.IsNullOrEmpty(complex.Name) ? anonymous ?? throw new InvalidDataException("Behavior anonymous record has no owner.") : complex.Name, type, slot, depth + 1, complex)); return; }
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
        { if (particle == null) return; if(particle is XmlSchemaAll all){foreach(XmlSchemaParticle child in all.Items)Elements(child,fields);} else if (particle is XmlSchemaSequence sequence) { foreach (XmlSchemaObject child in sequence.Items) { if (child is not XmlSchemaParticle p) throw new InvalidDataException("Behavior particle invalid."); Elements(p, fields); } } else if(particle is XmlSchemaChoice choice && choice.Items.Count==1 && choice.Items[0] is XmlSchemaElement single && (single.Name=="BuildCost"&&single.ElementSchemaType?.Name=="MoneyTransaction"||single.Name=="Volume"&&single.ElementSchemaType?.Name=="DynamicsVolumeType"))Elements(single,fields);
            // Reborn: the official dynamics choice represents five independently repeated shape families in native storage.
            else if(particle is XmlSchemaChoice shapes && shapes.MaxOccursString=="unbounded" && shapes.Items.Count==5 && shapes.Items.OfType<XmlSchemaElement>().Select(e=>e.Name).SequenceEqual(new[]{"Sphere","Capsule","Box","Cylinder","Triangle"})){foreach(XmlSchemaElement shape in shapes.Items)fields.Add((shape.Name!,shape.ElementSchemaType,Reference(shape.UnhandledAttributes),true));}else if (particle is XmlSchemaElement e && e.Name != null && e.ElementSchemaType != null) fields.Add((e.Name, e.ElementSchemaType, Reference(e.UnhandledAttributes), e.MaxOccurs > 1 || e.MaxOccursString == "unbounded")); else throw new InvalidDataException("Behavior schema particle is not metadata-enabled."); }
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
    // Reborn: schema/native attribute contracts reuse only signatures already qualified by this tokenizer, without guessing new enum or flag wire IDs.
    public static bool TryReviewedScalarSignature(string key,out uint signature)
    {try{signature=Signature(key);return true;}catch(InvalidDataException){signature=0;return false;}}

    private static uint Signature(string key) => key switch
    {
        // Reborn: stock Spawn metadata uses the same weak GameObject element signature for source-sized arrays.
        // Reborn: signatures captured from the retail Allied Bomber subobject-upgrade nugget tables.
        // Reborn: both array type signatures are captured from original AlliedBomberAircraft transport metadata.
        // Reborn: the single ObjectStatus ordinal uses its original named-enum schema identity, distinct from the bit mask.
        // Reborn: exact enum, volume and shape-list tags captured from retail DynamicsUpdate metadata.
        "DynamicsMotionType"=>0xA39AFB20,"DynamicsCollisionType"=>0x496E121D,"IntuitiveCollisionType"=>0xCE2C8DE9,"DynamicsContactTag"=>0x92E342E4,"DynamicsDrawPriority"=>0x41737FFB,
        "DynamicsVolumeType"=>0x96842FA5,"DynamicsSphereShapeType[]"=>0x9800E795,"DynamicsCapsuleShapeType[]"=>0xF7F58CA7,"DynamicsBoxShapeType[]"=>0x520B5F30,"DynamicsCylinderShapeType[]"=>0x3804CB6A,"DynamicsTriangleShapeType[]"=>0x1B5314E2,"Quaternion"=>0x854985C7,
        "ObjectStatusType"=>0xFA0DE777,
        "InitialPayload[]"=>0xB967AE82,"UpgradeCreation[]"=>0x1CE024D5,
        // Reborn: the bunker own-field metadata captures this explicitly annotated AssetReference wire, not a weak ID or string hash.
        "AssetReference:InvisibilityTemplate"=>0x914CA765,
        // Reborn: the Engineer emotion list carries this captured strong EmotionTemplate-array wire tag.
        "AssetReference:EmotionTemplate[]"=>0x9CFED1B4,
        "SubObjectsUpgradeNugget[]"=>0x7F67C232,"AssetIdList"=>0xE7980C56,"ReplaceTexture[]"=>0x6E5D40FB,"UpgradeTemplateWeakRef"=>0x26FACFC7,
        "GameObjectWeakRef[]"=>0x148ED75D,"DamageBitFlags"=>0x07FAAEA4,
        // Reborn: EP1 ProductionUpdate signatures observed in stock GameObject:SovietRefinery metadata.
        // Reborn: the real SovietBarracks translation list supplies its EP1 wire signature; emitted rows remain source-sized.
        "ProductionQueueType"=>0xBC88486A,"QuantityModifier[]"=>0x249DFB83,"ProductionModifier[]"=>0xEF83869A,"GiveUpgradeToProduced"=>0xB290DB31,"UpgradeTranslation[]"=>0xB38AB380,
        // Reborn: stock EngineerContain voice names use the plain StringHash signature.
        "StringHash"=>0x58BEC727,
        // Reborn: stock AssistedPlacementBehavior hint mask signature.
        "AssistanceHintBitFlags"=>0xFA98DE4D,
        // Reborn: stock InfiltratorContain effect enum signature.
        "InfiltratorEffectType"=>0xD0CE7F56,
        // Reborn: stock StructurePlacementBehavior inline Shape signatures.
        "Shape"=>0xE7210555,"GeometryType"=>0xB0D49164,"ContactPointGenerationType"=>0x465508CE,
        // Reborn: independently observed root-record signatures contain no native offsets or per-asset metadata templates.
        // Reborn: original Crane OpenContain/Garrison metadata signatures; these identify types, not asset-specific offsets.
        "ReplaceSelfOptionsBitFlags"=>0x63591CBA,"InitialRoster"=>0x201C212B,"PassengerDataType[]"=>0x5CB5DD8B,"MemberTemplateStatusData[]"=>0xB774AECB,
        "AutoDepositBitFlags"=>0xE5828841,"MoneyTransactionCategory"=>0x9E55F778,"MoneyTransaction[]"=>0xB706334D,"SkirmishAIBaseLocation"=>0xCE5B5436,"DamageType"=>0x46486FF1,
        "WeaponAntiBitFlags"=>0xC8C69390,"EconomyResourceType"=>0x071B12C5,"AISpecialPowerInfo[]"=>0xCBA6C0C2,
        "AIBaseSelectionCriteria"=>0xC153D5E0,"AudioVoiceEntry[]"=>0x8AA9CB14,"AudioObjectSpecificVoiceEntry[]"=>0xAEDFC4CD,
        // Reborn: stock SovietRefinery criteria contain four D7431325 flag fields and a B0E66C4E sort ordinal.
        "AIBaseTypeBitFlags"=>0xD7431325,"AIBaseSortingOrder"=>0xB0E66C4E,
        // Reborn: existing independently stock-qualified EP1 AIUpdate metadata already uses these scalar/mask wire identities.
        "AutoAcquireEnemiesBitFlags"=>0x8B0FB9DA,"StateMachineType"=>0xAC6E35E0,
        // Reborn: exact root sound/buildability signatures independently captured from power-plant/refinery stock metadata.
        "AudioSoundEntry[]"=>0x87D0686C,"ThingTemplateSoundType"=>0xD50DC497,"BuildPlacementTypeBitFlags"=>0xF446CF6F,
        "AudioVoiceReferentialEntry[]"=>0xF8ED487B,"ThingTemplateObjectSpecificVoiceType"=>0x94A69CB3,"ThingTemplateVoiceType"=>0x4DA708B0,"EvaEventEntry[]"=>0xD7B4DEDB,
        "EvaEventRef"=>0xC4EC43B1,"ThingTemplateEvaEventType"=>0xC053B716,"ShadowType"=>0x3B581951,
        "TextureRef"=>0x4C3E3938,"xs:boolean"=>0x77E3F4F7,"xs:byte"=>0x371E5DC2,
        "CrusherLevelModelConditionInfo[]"=>0x9091F97B,"CrushKillDelayForObjectFilter[]"=>0x038D6BB2,
        "AIAoeAttackPowerEmanationLocation" => 0x33433965, "AIAoeAttackPowerLocationPreference" => 0x0B3BCAE4,
        "AISpecialPowerWeaponData" => 0x43DBA49D, "AITargetLocationBitFlags" => 0x73B8B420, "AIToggleSpecialPowerInfo" => 0xFE374CCD,
        "Angle" => 0x4E16EA4D, "AnimAndDuration" => 0x6B746AF6, "ArmorSetType" => 0x2037048F, "AssetId" => 0x805D25F4,
        "AttributeModifierRef" or "AttributeModifierRef[]" => 0x09DDCFF3, "AudioEventInfoRef" => 0x63D7B468, "AudioEventInfoRefList" => 0xE1117F93,
        "ConditionsBitFlags" => 0x041F5A89, "DeathBitFlags" => 0x034DF1A1, "DieMuxDataType" => 0x0084001B,
        // Reborn: retail aura condition and modifier-category masks each occupy one word.
        "AttributeModifierAuraUpdateRequiredBitFlags" => 0x3D7EE58A, "AttributeModifierCategoryBitFlags" => 0x45F159E8,
        // Reborn: collision enum and mask wire signatures are anchored in original EP1 metadata.
        "FireWeaponFlagsBitFlags" => 0xD3BD617C, "WhereToFireType" => 0xE0AB5E11, "Stance" => 0x9FE5F2EE,
        "DisabledBitFlags" => 0x196570C9, "DisabledType" => 0xBDE95B5A, "DurationUnsignedInt" => 0x6F6D1AE7,
        "EffectInfoBitFlags" => 0xA2F37A52, "EffectInfoType[]" => 0x25F5379F,
        "FXListBehaviorDirection" => 0x5E151008, "FXListBehaviorEvent[]" => 0x65BE8011, "FXListBehaviorType" => 0x03AF8D2F,
        "FXListRef" or "FXListRef[]" => 0x0D1C8E83, "FXParticleSystemRef" or "FXParticleSystemRef[]" => 0x43F1CE36, "GameObjectWeakRef" => 0x148ED75D,
        "KindOfBitFlags" => 0x9AFC9193, "LocomotorSetType" => 0x845D12B2, "LogicCommandRef" => 0xE0E2A81A,
        "MicroManagerStateList" => 0xD7620AAA, "MicroManagerUnitTaskList" => 0x74BAC2ED,
        "ModelConditionBitFlags" => 0x67DBBFC9, "ModelConditionFlagType" => 0x15723E0B,
        // Reborn: OCL enum/list wire signatures are captured from retail PlayerSpellBook metadata.
        "OCLCreateLocType" => 0xE5C09280, "OCLUpgradePair[]" => 0xA8B856F2,
        "ObjectCreationListRef" or "ObjectCreationListRef[]" => 0x42E4E9D1, "ObjectFilter" => 0xE46EA3B2, "ObjectFilterAlignment" => 0x9FFD43EB,
        "ObjectFilterAssetRef" => 0x1E592B76, "ObjectFilterRelationshipBitMask" => 0x5C8981A9, "ObjectFilterRule" => 0x0027BB58,
        "ObjectStatusBitFlags" => 0xA7BD811E, "ObjectStatusValidationDataType" => 0x3E8F938C, "OCLMonitorOptionFlag" => 0x07E1B584,
        "PartitionManagerDistTestType" => 0x9F0DA607, "Percentage" or "SageReal" or "xs:float" => 0x645864F3,
        // Reborn: destruction mesh/item signatures match the independently captured stock tokens and literal writer.
        "Poid:SubObject"=>0x03A28C40,"BuildingDestructionItem[]"=>0xFFDDEC52,
        "PlayerTechRef" => 0xDBBF0FDF, "Poid:ModuleData" => 0xBDF07965, "Poid:ObjectFilter" => 0x4D3B3F19,
        "RGBColor" => 0xAEE762A5, "SageBool" => 0x77E3F4F7, "SageInt" => 0x23E650B8, "SageUnsignedInt" => 0x763177A4,
        // Reborn: death-phase containers retain their own list signatures; repeated typed references use the same stock element wire as scalar references.
        "SlowDeathFXListType[]" => 0x33D0DFAE, "SlowDeathOCLType[]" => 0x97474300, "SlowDeathPhaseType" => 0x350A89A5,
        "SlowDeathSoundType[]" => 0xB8E87C25, "SlowDeathWeaponType[]" => 0x908107EC, "SoundOrEvaEvent" => 0x1AE6B6B7,
        // Reborn: Dreadnought activation-module rows independently capture the TriggerType enum wire identity.
        "TriggerType" => 0x01C3A3A2,
        "SpecialAbilityUpdateOptionsTypeBitFlags" => 0x982F7738, "SpecialPowerModuleInfo[]" => 0xADA54D29,
        "SpecialPowerRef" => 0x96311B79, "SpecialPowerTemplateRef" => 0xAEE83888, "SpecialPowerWeakRef" => 0x40BBFFB2,
        "StanceTemplateRef" => 0x0D8218FB, "StringList" => 0x6D66AE18, "Time" => 0x998669E8,
        // Reborn: these enum and shader-reference signatures are read from retail teleport-update field metadata.
        "TelefragType" => 0x80446A38, "ShaderOverrideRef" => 0x8A16C611,
        "ToggleStatusAiHint" => 0x018DD8B9, "ToggleStatusNugget[]" => 0xFC294B1D,
        "UpgradeTemplateRef" or "UpgradeTemplateRef[]" => 0x4ABA1F29, "UpgradeTemplateWeakRef[]" => 0x26FACFC7,
        "Vector3" => 0x29368077, "Velocity" => 0x7A76C60E, "WeakReference:GameObject[]" => 0x9B19049F,
        // Reborn: these turret signatures were read from the original Soviet Sentry Gun metadata.
        "TurretAIData"=>0xF6753063,"TurretAITargetChooserData"=>0x08CF34B0,"TargetingCompareListRef"=>0x1B85B469,"TargetChooserDynamicTargetFlags"=>0x344E0F38,"ObjectVisionRangeItem[]"=>0x2E11FC54,"WeaponSlotBitFlags"=>0x28AEDB15,
        "WeaponChoiceCriteria" => 0x1D110792, "WeaponFireProbabilityType[]" => 0x080BE21F, "WeaponRef" or "WeaponRef[]" => 0xD046DD22,
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
    private static int Size(Type type) => type.IsPointer ? 4 : type.IsEnum ? 4 : type.IsGenericType ? type.GetGenericTypeDefinition().FullName switch { "Relo.List`1" or "Relo.String`1" or "Relo.PolymorphicList`1" => 8, "Relo.AssetReference`1" or "Relo.AssetReference`2" or "TypedAssetId`1" => 4, _ => throw new InvalidDataException("Behavior generic native type unreviewed.") } : Marshal.SizeOf(type);
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
