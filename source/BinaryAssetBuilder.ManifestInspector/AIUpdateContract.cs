using System.Runtime.InteropServices;
using System.Xml;
using BinaryAssetBuilder.Core.Hashing;
using BinaryAssetBuilder.Core.SageXml;
using SageBinaryData;
using AnsiString=Relo.String<sbyte>;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: source-shaped AIUpdate verification uses typed schema/native views over the unchanged production EP1 ABI.
internal static class AIUpdateContract
{
    // Reborn: these proof views retain actual field widths while exposing flag/list/base ownership hidden by older uint/void storage.
    [StructLayout(LayoutKind.Sequential)]
    internal unsafe struct DynamicBitFlags { public fixed uint Value[1]; }
    [StructLayout(LayoutKind.Sequential)]
    internal struct BaseChooser
    {
        public Time IdleScanDelay,ReacquireDelay,StartupDelay;
        public float SympathyRange;
        public DynamicBitFlags CanPickDynamicTargetType;
        public Relo.List<Ep1ObjectVisionRangeItem> ObjectVisionRange;
        public SageBool CheckVisionRange,CanPickDynamicTargets,RotateToTargetWhenAiming,CanAutoAcquireNonAutoAcquirable,CanChaseAttack,CanOnlyAttackTargetsInFront;
    }
    [StructLayout(LayoutKind.Sequential)]
    internal struct Chooser { public BaseChooser Base;public Relo.AssetReference<TargetingCompareList> TargetingCompareList; }
    [StructLayout(LayoutKind.Sequential)]
    internal unsafe struct Native
    {
        public UpdateModuleData Base;
        public AnsiString AttackPriority;
        public AutoAcquireEnemiesBitFlags AutoAcquireEnemiesWhenIdle;
        public float StopChaseDistance;
        public Time MinCowerTime,MaxCowerTime,BurningDeathTime;
        public float ComboLocoAttackDistance;
        public Relo.List<AnsiString> SpecialContactPoints;
        public Time IdleTargetTime;
        public uint MaxCombineChildren;
        public float SpawnOffsetRadius;
        public Ep1AIStateMachineType StateMachine;
        public Ep1RootLocomotorSetType DefaultLocomotorSet;
        public Chooser* UnitAITargetChooserData;
        public SageBool StandGround,FadeOnPortals;
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: expose derived AI records to the schema verifier using the typed common proof view. */
    //-------------------------------------------------------------------------------------------------
    [StructLayout(LayoutKind.Sequential)]
    internal struct Transport { public Native Base; }

    [StructLayout(LayoutKind.Sequential)]
    internal struct Wander
    {
        public Native Base;
        public ModelConditionBitFlags EntryCondition;
        public uint WanderDistance;
        public SageBool AttackAll, Selectable;
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: retain the existing typed chooser proof in the concrete Horde and truck records. */
    //-------------------------------------------------------------------------------------------------
    [StructLayout(LayoutKind.Sequential)]
    internal struct Horde { public Native Base; }

    [StructLayout(LayoutKind.Sequential)]
    internal struct SupplyTruck
    {
        public Native Base;
        public uint MaxBoxes;
        public Time SupplyCenterActionDelay, SupplyWarehouseActionDelay;
        public float SupplyWarehouseScanDistance, DepletedAnnounceDistance, HarvestActivationRange;
        public Time HarvestPreparationTime, HarvestActionTime;
        public SageBool HarvestTrees, HarvestTiberium, HarvestsFromRearEnd;
        public SageBool CanPathThroughUnitsWhileDocking, CanPathThroughUnitsWhileHarvesting;
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: restore only comparison AI leaves; actual source, native values, chooser presence and typed references are proved separately. */
    //-------------------------------------------------------------------------------------------------
    internal static void Mask(XmlDocument source,XmlDocument baseline)
    {
        var actual=ConscriptModuleScalarContract.Owners(source).Where(o=>o.Container=="AI").ToArray();var model=ConscriptModuleScalarContract.Owners(baseline).Where(o=>o.Container=="AI").ToArray();
        // Reborn: an AI-less object has nothing to restore; its zero-count declaration is independently proved by AIAbsenceContract.
        if(actual.Length==0)return;
        // Reborn: restore the whole comparison container; actual source-sized AI leaves retain independent native/metadata proofs.
        var container=actual[0].Element.ParentNode!;foreach(var owner in actual)container.RemoveChild(owner.Element);foreach(var owner in model)container.AppendChild(source.ImportNode(owner.Element,true));
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: bind the full source leaf to every native byte/import; unsupported contact/vision lists stay explicit rather than skipped. */
    //-------------------------------------------------------------------------------------------------
    internal static SchemaNativeContract.Result Verify(XmlElement source,Relo.Chunk native,IReadOnlyList<AssetId> references)
    {
        if(source.HasAttribute("SpecialContactPoints")&&!string.IsNullOrWhiteSpace(source.GetAttribute("SpecialContactPoints")))throw SdkXmlDiagnostics.At(new InvalidDataException("AIUpdate SpecialContactPoints item metadata is not implemented."),source.GetAttributeNode("SpecialContactPoints"));
        var (type, wire) = source.LocalName switch
        {
            "TransportAIUpdate" => (typeof(Transport), Ep1TransportAIUpdateModuleData.Wire),
            "WanderAIUpdate" => (typeof(Wander), Ep1WanderAIUpdateModuleData.Wire),
            "HordeAIUpdate" => (typeof(Horde), Ep1HordeAIUpdateModuleData.Wire),
            "SupplyTruckAIUpdate" => (typeof(SupplyTruck), Ep1SupplyTruckAIUpdateModuleData.Wire),
            _ => (typeof(Native), 0x08DBC00Eu)
        };
        return SchemaNativeContract.Verify(source,type,wire,native,references);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: preserve the stock AI rule while giving each EA-derived class its own parent-only or extended table. */
    //-------------------------------------------------------------------------------------------------
    internal static StockMetadataGrammar QualifyDerived(StockMetadataGrammar common, bool wander)
    {
        if (Marshal.SizeOf<Transport>() != 76 || Marshal.SizeOf<Ep1TransportAIUpdateModuleData>() != 76 ||
            Marshal.SizeOf<Wander>() != 0x90 || Marshal.SizeOf<Ep1WanderAIUpdateModuleData>() != 0x90 ||
            Marshal.OffsetOf<Wander>(nameof(Wander.EntryCondition)).ToInt32() != 0x4c ||
            Marshal.OffsetOf<Wander>(nameof(Wander.WanderDistance)).ToInt32() != 0x88 ||
            Marshal.OffsetOf<Wander>(nameof(Wander.AttackAll)).ToInt32() != 0x8c ||
            Marshal.OffsetOf<Wander>(nameof(Wander.Selectable)).ToInt32() != 0x8d)
            throw new InvalidDataException("Derived EP1 AI layout differs.");
        var fields = new List<StockMetadataGrammar.RuleField> { new(1,0x08DBC00E,2) };
        if (wander)
            fields.AddRange(new StockMetadataGrammar.RuleField[]
            {
                new(0x65B2AA1E,0x77E3F4F7,4), new(0x4859EEC6,0x67DBBFC9,4),
                new(0x9E8FEAE3,0x77E3F4F7,4), new(0x6CA763A8,0x763177A4,4)
            });
        var rule = new StockMetadataGrammar.Rule((wander?typeof(Wander):typeof(Transport)).FullName!,
            wander?Ep1WanderAIUpdateModuleData.Wire:Ep1TransportAIUpdateModuleData.Wire, fields.ToArray());
        return StockMetadataGrammar.Import(common.Export().Select(parent=>parent with { Class=null }).Append(rule));
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: qualify named Horde and truck inheritance and the EA truck tail. */
    //-------------------------------------------------------------------------------------------------
    internal static StockMetadataGrammar QualifyHordeOrTruck(StockMetadataGrammar common,bool supplyTruck)
    {
        if(Marshal.SizeOf<Horde>()!=0x4c || Marshal.SizeOf<Ep1HordeAIUpdateModuleData>()!=0x4c ||
            Marshal.SizeOf<SupplyTruck>()!=0x74 || Marshal.SizeOf<Ep1SupplyTruckAIUpdateModuleData>()!=0x74 ||
            Marshal.OffsetOf<Ep1SupplyTruckAIUpdateModuleData>("MaxBoxes").ToInt32()!=0x4c ||
            Marshal.OffsetOf<Ep1SupplyTruckAIUpdateModuleData>("SupplyCenterActionDelay").ToInt32()!=0x50 ||
            Marshal.OffsetOf<Ep1SupplyTruckAIUpdateModuleData>("SupplyWarehouseActionDelay").ToInt32()!=0x54 ||
            Marshal.OffsetOf<Ep1SupplyTruckAIUpdateModuleData>("CanPathThroughUnitsWhileDocking").ToInt32()!=0x6f ||
            Marshal.OffsetOf<Ep1SupplyTruckAIUpdateModuleData>("CanPathThroughUnitsWhileHarvesting").ToInt32()!=0x70)
            throw new InvalidDataException("EP1 Horde or Supply Truck AI ABI differs.");
        var fields=new List<StockMetadataGrammar.RuleField>{new(1,0x08DBC00Eu,2)};
        if(supplyTruck)
            fields.AddRange(new StockMetadataGrammar.RuleField[]
            {
                new(0x6F9636D9u,0x763177A4u,4),new(0xD2904A75u,0x998669E8u,4),
                new(0x565C0BE6u,0x998669E8u,4),new(0x763CC923u,0x645864F3u,4),
                new(0x8213E2BBu,0x645864F3u,4),new(0xE1146E25u,0x77E3F4F7u,4),
                new(0x81693423u,0x77E3F4F7u,4),new(0xCD2C3B18u,0x645864F3u,4),
                new(0xBFDEC31Au,0x998669E8u,4),new(0x7C9E6832u,0x998669E8u,4),
                new(0x8D9DAEB8u,0x77E3F4F7u,4),new(0xA99A1E4Au,0x77E3F4F7u,4),
                new(0xB14635F6u,0x77E3F4F7u,4)
            });
        var type=supplyTruck?typeof(SupplyTruck):typeof(Horde);
        var wire=supplyTruck?Ep1SupplyTruckAIUpdateModuleData.Wire:Ep1HordeAIUpdateModuleData.Wire;
        return StockMetadataGrammar.Import(common.Export().Select(row=>row with { Class=null })
            .Append(new(type.FullName!,wire,fields.ToArray())));
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: independent stock Conscript, refinery and flak-trooper witnesses qualify chooser-present, chooser-absent and one-vision-item graphs before authored composition. */
    //-------------------------------------------------------------------------------------------------
    internal static StockMetadataGrammar Qualify(string manifest,XmlDocument baseline,SchemaSet schemas,ManifestAsset[] catalog)
    {
        if(Marshal.SizeOf<Native>()!=76||Marshal.SizeOf<Chooser>()!=40||Marshal.SizeOf<BaseChooser>()!=36)throw new InvalidDataException("AIUpdate proof/native ABI size differs.");
        var grammar=new StockMetadataGrammar();
        foreach(string name in new[]{"SovietAntiInfantryInfantry","SovietRefinery","SovietAntiVehicleInfantry"}){
            var captured=GameObjectStockReview.Capture(manifest,"GameObject:"+name);var module=GameObjectModuleReview.Inspect(manifest,captured.Asset.Name).Containers.Single(c=>c.Name=="AI").Modules.Single();
            // Reborn: the original AI XML is a developer witness; fast builds learn row kinds from native records.
            if(SdkBuildTests.Enabled)
            {
                var document=(XmlDocument)baseline.CloneNode(true);
                if(name!="SovietAntiInfantryInfantry"){var original=(XmlElement)document.ImportNode(GameObjectExtendedRootContract.Original(name=="SovietRefinery"?"Soviet/Structures/SovietRefinery.xml":"Soviet/Units/SovietAntiVehicleInfantry.xml","AI").Single(),true);
                    // Reborn: the vision witness (one ObjectVisionRange item) uses a GlobalDefines idle-scan delay; resolve it from the original defines.
                    foreach(var attribute in original.SelectNodes(".//@*")!.OfType<XmlAttribute>().Where(a=>a.Value.StartsWith("=$",StringComparison.Ordinal)).ToArray())attribute.Value=GameObjectExtendedRootContract.Original("GlobalData/GlobalDefines.xml","Define").Single(e=>e.GetAttribute("name")==attribute.Value[2..]).GetAttribute("value");
                    var previous=document.DocumentElement!.ChildNodes.OfType<XmlElement>().Single(e=>e.LocalName=="AI");previous.ParentNode!.ReplaceChild(original,previous);}
                var resolved=SchemaNativeContract.Resolve(document,schemas,catalog);var element=ConscriptModuleScalarContract.Owners(resolved).Single(o=>o.Container=="AI").Element;
                SchemaNativeContract.VerifyStock(element,typeof(Native),0x08DBC00Eu,captured.Bytes,module.Native,captured.Asset.References.ToArray());
            }
            grammar.Learn(captured.Bytes,module.Wrapper,captured.Projection.NativeRoot,typeof(Native));
        }
        return grammar;
    }
}
