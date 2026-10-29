using BinaryAssetBuilder.Core.Diagnostics;
using Relo;
using SageBinaryData;

public static partial class Marshaler
{
    public static unsafe void Marshal(Node node, BehaviorModuleData** objT, Tracker state)
    {
        if (node is null)
        {
            return;
        }
        uint typeId = 0u;
        Marshal(node.GetAttributeValue("TypeId", "0"), &typeId, Tracker.NullTracker);
        switch (typeId)
        {
            // Reborn: falling projectiles and ship slow-death use independently captured EP1 class identities.
            case 0x114CC62Fu: MarshalPolymorphicType<ProjectileUpdateModuleData,BehaviorModuleData>(node,objT,state);break;
            case 0xECC6823Du: MarshalPolymorphicType<ShipSlowDeathBehaviorModuleData,BehaviorModuleData>(node,objT,state);break;

            // Reborn: compile the Allied receiver with its actual EP1 identity and full native upgrade/filter graph.
            case 0x1773BB2Eu:
                MarshalPolymorphicType<TechUpgradeReceiverModuleData,BehaviorModuleData>(node,objT,state);
                break;
            // Reborn: the original unsuffixed locomotor-switch wire selects the complete EP1 update record.
            case 0x4EDFB24Bu:
                MarshalPolymorphicType<SwitchLocomotorUpdate,BehaviorModuleData>(node,objT,state);
                break;
            // Reborn: naval update writers preserve the original Stingray and Dreadnought class hashes.
            case 0x829F7876u:
                MarshalPolymorphicType<CircularDamageUpdateModuleData,BehaviorModuleData>(node,objT,state);
                break;
            case 0xB14A0525u:
                MarshalPolymorphicType<LoseHealthUpdateModuleData,BehaviorModuleData>(node,objT,state);
                break;
            // Reborn: compile the actual EP1 Apocalypse grapple and identity-only MCV storage module.
            case 0x196FC7E8u:
                MarshalPolymorphicType<GrapplingHookUpdateModuleData,BehaviorModuleData>(node,objT,state);
                break;
            case 0x64B1001Au:
                MarshalPolymorphicType<SubStructureStorageBehaviorModuleData,BehaviorModuleData>(node,objT,state);
                break;
            // Reborn: secondary listeners keep their original unsuffixed EP1 class hash.
            case 0x9D88BC91u:
                MarshalPolymorphicType<SecondaryObjectListenerModule,BehaviorModuleData>(node,objT,state);
                break;
            // Reborn: EP1 bunker invisibility uses a strong template import and its real concrete behavior identity.
            case 0x054B09C4u:
                MarshalPolymorphicType<InvisibilityBehaviorModuleData,BehaviorModuleData>(node,objT,state);
                break;
            // Reborn: EP1 attack-assist responses preserve their real wire identity and complete status/slot record.
            case 0xEC308E0Eu:
                MarshalPolymorphicType<AttackAssistResponseBehaviorModuleData,BehaviorModuleData>(node,objT,state);
                break;
            // Reborn: EP1 passenger ejection uses its real concrete identity, inherited ability fields and optional bone offset.
            case 0x34E5D5B5u:
                MarshalPolymorphicType<EjectPassengersSpecialAbilityUpdateModuleData,BehaviorModuleData>(node,objT,state);
                break;
            // Reborn: Tesla attack notifications use their concrete EP1 identity and complete optional friend filter.
            case 0x02FDF9E4u:
                MarshalPolymorphicType<NotifyWhenAttackingBehaviorModuleData,BehaviorModuleData>(node,objT,state);
                break;
            // Reborn: template-ID stores use their real concrete identity and inherited behavior header.
            case 0x50B8001Cu:
                MarshalPolymorphicType<StoreTemplateIDBehaviorModuleData,BehaviorModuleData>(node,objT,state);
                break;
            // Reborn: EP1 teleport updates retain their own native layout and concrete wire identity.
            case 0x1C8BE51Du:
                MarshalPolymorphicType<TeleportStoredObjectsUpdateModuleData,BehaviorModuleData>(node,objT,state);
                break;
            // Reborn: teleport stored objects has its own native record and concrete EP1 wire identity.
            case 0x318DC4D6u:
                MarshalPolymorphicType<TeleportStoredObjectsSpecialPowerModuleData,BehaviorModuleData>(node,objT,state);
                break;
            // Reborn: retain the concrete EP1 production-bonus identity and complete SpecialPower allocation.
            case 0x8ED33E53u:
                MarshalPolymorphicType<GiveProductionBonusSpecialPowerModuleData,BehaviorModuleData>(node,objT,state);
                break;
            // Reborn: wall builders have their own EP1 behavior identity, separate from KW WallHubBehavior.
            case 0x9590538Au:
                MarshalPolymorphicType<WallBuilderBehaviorModuleData,BehaviorModuleData>(node,objT,state);
                break;
            // Reborn: stock EP1 QueueDock has its own create-module identity and five owned bone strings.
            case 0xAF4C1F21u:
                MarshalPolymorphicType<QueueDockModuleData,BehaviorModuleData>(node,objT,state);
                break;
            // Reborn: EP1 damage/motion classes preserve their real concrete IDs and nested import/mask layouts.
            case 0xB21C7E67u:
                MarshalPolymorphicType<SecondaryDamageTrackerModuleData, BehaviorModuleData>(node, objT, state);
                break;
            case 0xFC222F04u:
                MarshalPolymorphicType<FreefallUpdateModuleData, BehaviorModuleData>(node, objT, state);
                break;
            // Reborn: EP1 toggle states/base/AI and nonempty child lists have selected stock/Core native proof; this dispatch is not full GameObject publication.
            // Reborn: Engineer assembly uses its original EP1 concrete wire ID and complete native record.
            case 0x7737AC69u:
                MarshalPolymorphicType<AssembleStructureSpecialAbilityUpdateModuleData, BehaviorModuleData>(node,objT,state);
                break;
            case 0xED1AE55Cu:
                MarshalPolymorphicType<ToggleStatusSpecialAbilityUpdateModuleData, BehaviorModuleData>(node, objT, state);
                break;
            // Reborn: EP1 destruction/tech/LUA families retain their concrete identities; selected native proof is separate from tokenized publication.
            case 0x0D96A2F4u:
                MarshalPolymorphicType<BuildingDestructionModuleData, BehaviorModuleData>(node, objT, state);
                break;
            case 0xFF7BCDB3u:
                MarshalPolymorphicType<TechUpgradeModuleData, BehaviorModuleData>(node, objT, state);
                break;
            case 0xFA6AC8B6u:
                MarshalPolymorphicType<LUAEventListBehaviorModuleData, BehaviorModuleData>(node, objT, state);
                break;
            // Reborn: stock-proven EP1 helper/sell classes use their own concrete IDs, not approximate legacy substitutes.
            case 0x7F72F242u:
                MarshalPolymorphicType<ObjectSMCHelperModuleData, BehaviorModuleData>(node, objT, state);
                break;
            case 0x4E9CB811u:
                MarshalPolymorphicType<StructureSellUpdateModuleData, BehaviorModuleData>(node, objT, state);
                break;
            case 0x2B847E06u:
                MarshalPolymorphicType<AISpecialPowerUpdateModuleData, BehaviorModuleData>(node, objT, state);
                break;
            case 0x74A24C45u:
                MarshalPolymorphicType<AttributeModifierUpgradeModuleData, BehaviorModuleData>(node, objT, state);
                break;
            case 0x239D954Fu:
                MarshalPolymorphicType<AttackTargetSpecialPowerModuleData, BehaviorModuleData>(node, objT, state);
                break;
            case 0xA1FE37ACu:
                MarshalPolymorphicType<ArmorUpgradeModuleData, BehaviorModuleData>(node, objT, state);
                break;
            case 0xC939197Bu:
                MarshalPolymorphicType<BeamTargetModuleData, BehaviorModuleData>(node, objT, state);
                break;
            case 0xC8E1225Fu:
                MarshalPolymorphicType<SlavedUpdateModuleData, BehaviorModuleData>(node, objT, state);
                break;
            case 0x004CA983u:
                MarshalPolymorphicType<SpawnedSlaveUpdateModuleData, BehaviorModuleData>(node, objT, state);
                break;
            case 0xD9B247DFu:
                MarshalPolymorphicType<AddObjectsToLiftUpdateSpecialPowerModuleData, BehaviorModuleData>(node, objT, state);
                break;
            case 0xA7DEFFCCu:
                MarshalPolymorphicType<AddObjectsToLureUpdateSpecialPowerModuleData, BehaviorModuleData>(node, objT, state);
                break;
            case 0xB65AB72Au:
                MarshalPolymorphicType<LureObjectsUpdateModuleData, BehaviorModuleData>(node, objT, state);
                break;
            case 0x12D1FAD9u:
                MarshalPolymorphicType<FlingStoredObjectsSpecialPowerModuleData, BehaviorModuleData>(node, objT, state);
                break;
            case 0xBF91BFCDu:
                // Reborn: EP1 uses an optional shader import while retaining the original wire class identity.
                MarshalPolymorphicType<Ep1LiftObjectUpdateModuleData, BehaviorModuleData>(node, objT, state);
                ((ModuleData*)*objT)->TypeId = 0xBF91BFCDu;
                break;
            case 0x91222181u:
                MarshalPolymorphicType<ReplaceSelfSpecialAbilityModuleData, BehaviorModuleData>(node, objT, state);
                break;
            case 0xE2EE1D7Au:
                MarshalPolymorphicType<ProjectileReplaceSelfSpecialAbilityModuleData, BehaviorModuleData>(node, objT, state);
                break;
            case 0x0BBF2A39u:
                // Reborn: select the complete EP1 layout and retain the original runtime class ID.
                using(var context=state.Push((void**)objT,(uint)sizeof(Ep1BezierProjectileBehaviorModuleData),1u))Marshal(node,(Ep1BezierProjectileBehaviorModuleData*)*objT,state);
                *(uint*)*objT=0x0BBF2A39u;
                break;
            case 0x815A9161u:
                MarshalPolymorphicType<BuildingBehaviorModuleData, BehaviorModuleData>(node, objT, state);
                break;
            case 0x8FFB4C58u:
                MarshalPolymorphicType<CastleUpgradeModuleData, BehaviorModuleData>(node, objT, state);
                break;
            case 0xE48609FBu:
                MarshalPolymorphicType<ClusterBombUpdateModuleData, BehaviorModuleData>(node, objT, state);
                break;
            case 0x70E5C103u:
                MarshalPolymorphicType<CollideModuleData, BehaviorModuleData>(node, objT, state);
                break;
            case 0xD6C03AC2u:
                MarshalPolymorphicType<AudioDynamicsCollideModuleData, BehaviorModuleData>(node, objT, state);
                break;
            case 0x19BF3331u:
                MarshalPolymorphicType<DamageDynamicsCollideModuleData, BehaviorModuleData>(node, objT, state);
                break;
            case 0x8C3B49E2u:
                MarshalPolymorphicType<ReactionFXOnDamageModuleData, BehaviorModuleData>(node, objT, state);
                break;
            case 0x878CEA1Fu:
                MarshalPolymorphicType<DamageSphereUpdateModuleData, BehaviorModuleData>(node, objT, state);
                break;
            case 0x6C11B6F7u:
                MarshalPolymorphicType<ShieldSphereUpdateModuleData, BehaviorModuleData>(node, objT, state);
                break;
            case 0x95D855B6u:
                MarshalPolymorphicType<YurikoShieldSphereUpdateModuleData, BehaviorModuleData>(node, objT, state);
                break;
            case 0x7EB7F219u:
                MarshalPolymorphicType<CommandSetUpgradeModuleData, BehaviorModuleData>(node, objT, state);
                break;
            case 0x3B68B768u:
                MarshalPolymorphicType<CreateObjectDieModuleData, BehaviorModuleData>(node, objT, state);
                break;
            case 0x91ED30DCu:
                MarshalPolymorphicType<CrushDieModuleData, BehaviorModuleData>(node, objT, state);
                break;
            case 0xD25E223Au:
                MarshalPolymorphicType<RebuildHoleExposeDieModuleData, BehaviorModuleData>(node, objT, state);
                break;
            case 0x2F46AEF0u:
                MarshalPolymorphicType<RebuildHoleBehaviorModuleData, BehaviorModuleData>(node, objT, state);
                break;
            case 0x6A65E73Du:
                MarshalPolymorphicType<DefaultProductionExitUpdateModuleData, BehaviorModuleData>(node, objT, state);
                break;
            case 0x0F7EC801u:
                MarshalPolymorphicType<DestroyDieModuleData, BehaviorModuleData>(node, objT, state);
                break;
            case 0x03E414F0u:
                MarshalPolymorphicType<DynamicShroudClearingRangeUpdateModuleData, BehaviorModuleData>(node, objT, state);
                break;
            case 0xA966196Eu:
                // Reborn: retain the emotion tracker wire ID while allocating the distinct Uprising native layout.
                using(var context=state.Push((void**)objT,(uint)sizeof(Ep1EmotionTrackerUpdateModuleData),1u))Marshal(node,(Ep1EmotionTrackerUpdateModuleData*)*objT,state);
                *(uint*)*objT=0xA966196Eu;
                break;
            case 0x0FF760E0u:
                // Reborn: EP1 engineer uses the stock-qualified 92-byte record with inline status/EVA storage, not the KW pointer layout.
                using (var context = state.Push((void**)objT, (uint)sizeof(Ep1EngineerContainModuleData), 1u))
                    Marshal(node, (Ep1EngineerContainModuleData*)*objT, state);
                uint engineerTypeId = 0x0FF760E0u;
                state.InplaceEndianToPlatform(&engineerTypeId);
                *(uint*)*objT = engineerTypeId;
                break;
            case 0x5047272Eu:
                MarshalPolymorphicType<InfiltratorContainModuleData, BehaviorModuleData>(node, objT, state);
                break;
            case 0x5717D9DCu:
                MarshalPolymorphicType<InvalidTargetsDeathUpdateModuleData, BehaviorModuleData>(node, objT, state);
                break;
            case 0xAE4A97B0u:
                // Reborn: select the independently qualified Uprising collision record, retaining its original wire identity.
                using(var context=state.Push((void**)objT,(uint)sizeof(Ep1FireWeaponCollideModuleData),1u))Marshal(node,(Ep1FireWeaponCollideModuleData*)*objT,state);
                *(uint*)*objT=0xAE4A97B0u;
                break;
            case 0xBBAC8C1Cu:
                // Reborn: EP1 slave watchers use the independently qualified 24-byte record, retaining the original wire identity.
                using(var context=state.Push((void**)objT,(uint)sizeof(SlaveWatcherBehaviorModuleData),1u))Marshal(node,(SlaveWatcherBehaviorModuleData*)*objT,state);
                *(uint*)*objT=0xBBAC8C1Cu;
                break;
            case 0xB28C3D52u:
                // Reborn: EP1 owns the interval/status at the root and uses smaller firing nuggets than KW.
                using(var context=state.Push((void**)objT,(uint)sizeof(Ep1FireWeaponUpdateModuleData),1u))Marshal(node,(Ep1FireWeaponUpdateModuleData*)*objT,state);
                *(uint*)*objT=0xB28C3D52u;
                break;
            case 0xF5D17C70u:
                MarshalPolymorphicType<StructureToppleUpdateModuleData, BehaviorModuleData>(node, objT, state);
                break;
            case 0x06DE2D62u:
                MarshalPolymorphicType<ToppleUpdateModuleData, BehaviorModuleData>(node, objT, state);
                break;
            case 0x97482233u:
                MarshalPolymorphicType<OilSpillUpdateModuleData, BehaviorModuleData>(node, objT, state);
                break;
            case 0xF3B802D5u:
                MarshalPolymorphicType<DamageFieldUpdateModuleData, BehaviorModuleData>(node, objT, state);
                break;
            case 0x6BE00745u:
                MarshalPolymorphicType<FlammableUpdateModuleData, BehaviorModuleData>(node, objT, state);
                break;
            case 0x4D834921u:
                // Reborn: Uprising transactions have a distinct native layout; retain the retail class identity.
                using(var context=state.Push((void**)objT,(uint)sizeof(Ep1AutoDepositUpdateModuleData),1u))
                    Marshal(node,(Ep1AutoDepositUpdateModuleData*)*objT,state);
                uint depositTypeId=0x4D834921u;state.InplaceEndianToPlatform(&depositTypeId);*(uint*)*objT=depositTypeId;
                break;
            case 0xB07119E4u:
                MarshalPolymorphicType<FXListBehaviorModuleData, BehaviorModuleData>(node, objT, state);
                break;
            case 0xFF84FBDEu:
                MarshalPolymorphicType<GeometryUpgradeModuleData, BehaviorModuleData>(node, objT, state);
                break;
            case 0xF3967C53u:
                MarshalPolymorphicType<GettingBuiltBehaviorModuleData, BehaviorModuleData>(node, objT, state);
                break;
            case 0x2DE34173u:
                MarshalPolymorphicType<GiveUpgradeUpdateModuleData, BehaviorModuleData>(node, objT, state);
                break;
            case 0x04B8C430u:
                MarshalPolymorphicType<GrantUpgradeCreateModuleData, BehaviorModuleData>(node, objT, state);
                break;
            case 0x7517FE52u:
                MarshalPolymorphicType<HeightDieUpdateModuleData, BehaviorModuleData>(node, objT, state);
                break;
            case 0x91840787u:
                MarshalPolymorphicType<MissileUpdateModuleData, BehaviorModuleData>(node, objT, state);
                break;
            case 0xE7CF961Cu:
                MarshalPolymorphicType<JumpJetSpecialPowerModuleData, BehaviorModuleData>(node, objT, state);
                break;
            case 0x977D25A1u:
                MarshalPolymorphicType<LevelUpUpgradeModuleData, BehaviorModuleData>(node, objT, state);
                break;
            case 0x6C283C72u:
                MarshalPolymorphicType<LifetimeUpdateModuleData, BehaviorModuleData>(node, objT, state);
                break;
#if KANESWRATH
            case 0x4899B6CDu:
                MarshalPolymorphicType<MetaGameLifetimeUpdateModuleData, BehaviorModuleData>(node, objT, state);
                break;
#endif
            case 0x2799DFF2u:
                MarshalPolymorphicType<LocomotorSetModuleData, BehaviorModuleData>(node, objT, state);
                break;
            case 0x14E031A2u:
                MarshalPolymorphicType<SwitchLocomotorsSpecialAbilityUpdateModuleData, BehaviorModuleData>(node, objT, state);
                break;
            case 0x7C9A2737u:
                MarshalPolymorphicType<MaxHealthUpgradeModuleData, BehaviorModuleData>(node, objT, state);
                break;
            case 0xB53A18ADu:
                MarshalPolymorphicType<MinefieldBehaviorModuleData, BehaviorModuleData>(node, objT, state);
                break;
            case 0xC7248613u:
                MarshalPolymorphicType<ModelConditionSpecialAbilityUpdateModuleData, BehaviorModuleData>(node, objT, state);
                break;
            case 0x4DDD5C7Fu:
                MarshalPolymorphicType<ModelConditionUpgradeModuleData, BehaviorModuleData>(node, objT, state);
                break;
            case 0xDDD81EE9u:
                MarshalPolymorphicType<RandomModelConditionBehaviorModuleData, BehaviorModuleData>(node, objT, state);
                break;
            case 0x09BE107Cu:
                MarshalPolymorphicType<StrafeAreaUpdateModuleData, BehaviorModuleData>(node, objT, state);
                break;
            case 0xF145A667u:
                MarshalPolymorphicType<OCLMonitorUpdateModuleData, BehaviorModuleData>(node, objT, state);
                break;
            case 0x4E463B86u:
                MarshalPolymorphicType<TeleportSpecialAbilityUpdateModuleData, BehaviorModuleData>(node, objT, state);
                break;
            case 0x93833563u:
                MarshalPolymorphicType<TeleportToCasterSpecialPowerModuleData, BehaviorModuleData>(node, objT, state);
                break;
            case 0x69E2CD6Cu:
                MarshalPolymorphicType<ObjectCreationUpgradeModuleData, BehaviorModuleData>(node, objT, state);
                break;
            case 0x8999C6A4u:
                // Reborn: retain the retail class identity while using the complete EP1 offset/enum/upgrade-pair layout.
                using(var context=state.Push((void**)objT,(uint)sizeof(Ep1OCLSpecialPowerModuleData),1u))
                    Marshal(node,(Ep1OCLSpecialPowerModuleData*)*objT,state);
                uint oclPowerTypeId=0x8999C6A4u;state.InplaceEndianToPlatform(&oclPowerTypeId);*(uint*)*objT=oclPowerTypeId;
                break;
#if KANESWRATH
            case 0xA9778E00u:
                MarshalPolymorphicType<SpawnHordeMemberSpecialPowerModuleData, BehaviorModuleData>(node, objT, state);
                break;
#endif
            case 0xC662DEB4u:
                MarshalPolymorphicType<TemporaryOwnerSpecialPowerModuleData, BehaviorModuleData>(node, objT, state);
                break;
#if KANESWRATH
            case 0x9B27DE30u: // actually present in TW, with minor change to KW, enable?
                MarshalPolymorphicType<DominateEnemySpecialPowerModuleData, BehaviorModuleData>(node, objT, state);
                break;
            case 0x4D76910Cu: // actually present in TW, enable?
                MarshalPolymorphicType<TemporarilyDefectUpdateModuleData, BehaviorModuleData>(node, objT, state);
                break;
#endif
            case 0xB8B58445u:
                MarshalPolymorphicType<OCLUpdateModuleData, BehaviorModuleData>(node, objT, state);
                break;
            case 0xD7CC2E35u:
                MarshalPolymorphicType<OpenContainModuleData, BehaviorModuleData>(node, objT, state);
                break;
            case 0xE491340Bu:
                MarshalPolymorphicType<HealContainModuleData, BehaviorModuleData>(node, objT, state);
                break;
            case 0x79BFEDCBu:
                MarshalPolymorphicType<PhysicsBehaviorModuleData, BehaviorModuleData>(node, objT, state);
                break;
            case 0x784EC9B8u:
                MarshalPolymorphicType<CombinedInfoModuleData, BehaviorModuleData>(node, objT, state);
                break;
            case 0xA112A404u:
                MarshalPolymorphicType<CompositeStructureInfoModuleData, BehaviorModuleData>(node, objT, state);
                break;
            case 0x005B408Eu:
                MarshalPolymorphicType<PlayerUpgradeSpecialPowerModuleData, BehaviorModuleData>(node, objT, state);
                break;
            case 0x6EC75439u:
                // Reborn: EP1 production uses the stock-qualified 84-byte record, not the KW layout with DoorOpeningTimeMetaUnit and speed-bonus fields.
                using (var context = state.Push((void**)objT, (uint)sizeof(Ep1ProductionUpdateModuleData), 1u))
                    Marshal(node, (Ep1ProductionUpdateModuleData*)*objT, state);
                uint productionTypeId = 0x6EC75439u;
                state.InplaceEndianToPlatform(&productionTypeId);
                *(uint*)*objT = productionTypeId;
                break;
            case 0x1B337783u:
                MarshalPolymorphicType<ProductionUpdateInfoModuleData, BehaviorModuleData>(node, objT, state);
                break;
            case 0x9063FB6Cu:
                // Reborn: use the EP1 schema-compatible time field without changing the separate legacy KW model/marshaler.
                // Reborn: the isolated managed model name is not the engine wire name; retain the concrete EP1 type identity.
                using (var context = state.Push((void**)objT, (uint)sizeof(Ep1QueueProductionExitUpdateModuleData), 1u))
                    Marshal(node, (Ep1QueueProductionExitUpdateModuleData*)*objT, state);
                uint exitTypeId = 0x9063FB6Cu;
                state.InplaceEndianToPlatform(&exitTypeId);
                *(uint*)*objT = exitTypeId;
                break;
            case 0x599F60D5u:
                MarshalPolymorphicType<CombineSpecialPowerModuleData, BehaviorModuleData>(node, objT, state);
                break;
            case 0x7C298D04u:
                MarshalPolymorphicType<UncombineSpecialPowerModuleData, BehaviorModuleData>(node, objT, state);
                break;
            case 0xBED5AA3Au:
                MarshalPolymorphicType<RadarHackSpecialPowerModuleData, BehaviorModuleData>(node, objT, state);
                break;
            case 0xE6B352B9u:
                MarshalPolymorphicType<RadarJamSpecialPowerModuleData, BehaviorModuleData>(node, objT, state);
                break;
            case 0x47DF9C1Au:
                MarshalPolymorphicType<RadarSpySpecialPowerModuleData, BehaviorModuleData>(node, objT, state);
                break;
            case 0x52EECAEDu:
                MarshalPolymorphicType<RadarFreezeSpecialPowerModuleData, BehaviorModuleData>(node, objT, state);
                break;
            case 0xDDB97E4Bu:
                MarshalPolymorphicType<CaptureAndGiveCommandSetSpecialPowerModuleData, BehaviorModuleData>(node, objT, state);
                break;
            case 0x0453E648u:
                MarshalPolymorphicType<GiveOrRestoreUpgradeSpecialPowerModuleData, BehaviorModuleData>(node, objT, state);
                break;
            case 0xE1BD0410u:
                MarshalPolymorphicType<RefundDieModuleData, BehaviorModuleData>(node, objT, state);
                break;
            case 0x75A090B5u:
                MarshalPolymorphicType<RemoveUpgradeUpgradeModuleData, BehaviorModuleData>(node, objT, state);
                break;
            case 0x53FBA9B5u:
                MarshalPolymorphicType<ReplaceSelfUpgradeModuleData, BehaviorModuleData>(node, objT, state);
                break;
            case 0x1E205942u:
                MarshalPolymorphicType<SalvageCrateCollideModuleData, BehaviorModuleData>(node, objT, state);
                break;
            case 0x839AE83Cu:
                MarshalPolymorphicType<HealCrateCollideModuleData, BehaviorModuleData>(node, objT, state);
                break;
            case 0xEF293C44u:
                MarshalPolymorphicType<MoneyCrateCollideModuleData, BehaviorModuleData>(node, objT, state);
                break;
            case 0xB297F4B2u:
                MarshalPolymorphicType<ShroudCrateCollideModuleData, BehaviorModuleData>(node, objT, state);
                break;
            case 0xEE961694u:
                MarshalPolymorphicType<UnitCrateCollideModuleData, BehaviorModuleData>(node, objT, state);
                break;
            case 0x573DC203u:
                MarshalPolymorphicType<VeterancyCrateCollideModuleData, BehaviorModuleData>(node, objT, state);
                break;
            case 0x98769794u:
                MarshalPolymorphicType<SlowDeathBehaviorModuleData, BehaviorModuleData>(node, objT, state);
                break;
            case 0x0D916ADFu:
                // Reborn: the isolated EP1 model preserves the engine identity rather than hashing the managed Ep1 prefix.
                using (var context = state.Push((void**)objT, (uint)sizeof(Ep1SpawnBehaviorModuleData), 1u))
                    Marshal(node, (Ep1SpawnBehaviorModuleData*)*objT, state);
                uint spawnTypeId = 0x0D916ADFu;
                state.InplaceEndianToPlatform(&spawnTypeId);
                *(uint*)*objT = spawnTypeId;
                break;
            case 0xAC598F82u:
                MarshalPolymorphicType<SpecialAbilityUpdateModuleData, BehaviorModuleData>(node, objT, state);
                break;
            case 0x511F1BBBu:
                MarshalPolymorphicType<CurseSpecialPowerModuleData, BehaviorModuleData>(node, objT, state);
                break;
            case 0xC80E586Bu:
                MarshalPolymorphicType<OCLMonitorSpecialAbilityUpdateModuleData, BehaviorModuleData>(node, objT, state);
                break;
            case 0x78FDECC7u:
                MarshalPolymorphicType<LargeGroupAudioUpdateModuleData, BehaviorModuleData>(node, objT, state);
                break;
            case 0xCEFE1113u:
                MarshalPolymorphicType<SpecialPowerModuleData, BehaviorModuleData>(node, objT, state);
                break;
            case 0x9DAFD38Cu:
                MarshalPolymorphicType<AssignSlavesTargetObjectSpecialPowerModuleData, BehaviorModuleData>(node, objT, state);
                break;
            case 0x90A9315Bu:
                MarshalPolymorphicType<HordeDispatchSpecialPowerModuleData, BehaviorModuleData>(node, objT, state);
                break;
            case 0xD877BA74u:
                MarshalPolymorphicType<SpecialPowerDispatchSpecialPowerModuleData, BehaviorModuleData>(node, objT, state);
                break;
            case 0xB9B2A9B3u:
                MarshalPolymorphicType<SpecialPowerUpdateModuleData, BehaviorModuleData>(node, objT, state);
                break;
            case 0x74569AAAu:
                // Reborn: preserve the original wire ID while allocating the separate Uprising detector record.
                using(var context=state.Push((void**)objT,(uint)sizeof(Ep1StealthDetectorUpdateModuleData),1u))Marshal(node,(Ep1StealthDetectorUpdateModuleData*)*objT,state);
                uint detectorType=0x74569AAAu;state.InplaceEndianToPlatform(&detectorType);*(uint*)*objT=detectorType;
                break;
            case 0x8CE2210Cu:
                MarshalPolymorphicType<StructureCollapseUpdateModuleData, BehaviorModuleData>(node, objT, state);
                break;
            case 0xC6F3F62Du:
                MarshalPolymorphicType<InitiateRepairDieModuleData, BehaviorModuleData>(node, objT, state);
                break;
            case 0x5E1CAFDBu:
                // Reborn: allocate the Uprising aura layout while retaining its original wire identity.
                using(var context=state.Push((void**)objT,(uint)sizeof(Ep1AttributeModifierAuraUpdateModuleData),1u))Marshal(node,(Ep1AttributeModifierAuraUpdateModuleData*)*objT,state);
                uint auraType=0x5E1CAFDBu;state.InplaceEndianToPlatform(&auraType);*(uint*)*objT=auraType;
                break;
            case 0x643A2AC3u:
                MarshalPolymorphicType<ShieldBodyUpdateModuleData, BehaviorModuleData>(node, objT, state);
                break;
            case 0x5F06BA81u:
                // Reborn: retain the actual wire identity while allocating the separate EP1 nugget-based model.
                using(var context=state.Push((void**)objT,(uint)sizeof(Ep1SubObjectsUpgradeModuleData),1u))Marshal(node,(Ep1SubObjectsUpgradeModuleData*)*objT,state);
                uint subobjectsType=0x5F06BA81u;state.InplaceEndianToPlatform(&subobjectsType);*(uint*)*objT=subobjectsType;
                break;
            case 0x5A0E7D7Bu:
                MarshalPolymorphicType<TerrainResourceBehaviorModuleData, BehaviorModuleData>(node, objT, state);
                break;
            case 0x578C272Fu:
                MarshalPolymorphicType<TransportContainModuleData, BehaviorModuleData>(node, objT, state);
                break;
            case 0x408E9A3Fu:
                MarshalPolymorphicType<HordeTransportContainModuleData, BehaviorModuleData>(node, objT, state);
                break;
            case 0x4758FED0u:
                MarshalPolymorphicType<HordeContainModuleData, BehaviorModuleData>(node, objT, state);
                break;
            case 0x3FECABBBu:
                MarshalPolymorphicType<UnpauseSpecialPowerUpgradeModuleData, BehaviorModuleData>(node, objT, state);
                break;
            case 0xAE60AE98u:
                MarshalPolymorphicType<UpgradeModuleData, BehaviorModuleData>(node, objT, state);
                break;
            case 0x22ED33D8u:
                MarshalPolymorphicType<AudioLoopUpgradeModuleData, BehaviorModuleData>(node, objT, state);
                break;
            case 0x431722A9u:
                MarshalPolymorphicType<WeaponSetUpdateModuleData, BehaviorModuleData>(node, objT, state);
                break;
            case 0xA0AA07E1u:
                MarshalPolymorphicType<BeamSpecialAbilityUpdateModuleData, BehaviorModuleData>(node, objT, state);
                break;
            case 0x313998B5u:
                MarshalPolymorphicType<CaptureSpecialAbilityUpdateModuleData, BehaviorModuleData>(node, objT, state);
                break;
            case 0x1E45A393u:
                // Reborn: use the complete EP1 record while retaining the real weapon-fire concrete class identity.
                using(var context=state.Push((void**)objT,(uint)sizeof(Ep1WeaponFireSpecialAbilityUpdateModuleData),1u))
                    Marshal(node,(Ep1WeaponFireSpecialAbilityUpdateModuleData*)*objT,state);
                uint weaponFireAbilityTypeId=0x1E45A393u;state.InplaceEndianToPlatform(&weaponFireAbilityTypeId);*(uint*)*objT=weaponFireAbilityTypeId;
                break;
            case 0xF4568855u:
                MarshalPolymorphicType<TiberiumThiefSpecialAbilityUpdateModuleData, BehaviorModuleData>(node, objT, state);
                break;
#if KANESWRATH
            case 0x0ED97172u:
                MarshalPolymorphicType<MetaSpecialAbilityUpdateModuleData, BehaviorModuleData>(node, objT, state);
                break;
#endif
            case 0x6BC9FD0Eu:
                MarshalPolymorphicType<TiberiumFieldBehaviorModuleData, BehaviorModuleData>(node, objT, state);
                break;
            case 0x0F2D5192u:
                MarshalPolymorphicType<TiberiumCrystalBehaviorModuleData, BehaviorModuleData>(node, objT, state);
                break;
            case 0xAE3A906Cu:
                MarshalPolymorphicType<TiberiumGrowthModBehaviorModuleData, BehaviorModuleData>(node, objT, state);
                break;
#if KANESWRATH
            case 0x6045DA6Au:
                MarshalPolymorphicType<TiberiumFieldModBehaviorModuleData, BehaviorModuleData>(node, objT, state);
                break;
#endif
            case 0x2E546F8Eu:
                MarshalPolymorphicType<RadarBehaviorModuleData, BehaviorModuleData>(node, objT, state);
                break;
            case 0xD9A4DA61u:
                MarshalPolymorphicType<DockUpdateModuleData, BehaviorModuleData>(node, objT, state);
                break;
            case 0x0A2B8743u:
                // Reborn: EP1 repair keeps the shared layout but uses its own HealAmountPerSecond default.
                using (var context = state.Push((void**)objT, (uint)sizeof(Ep1RepairUpdateModuleData), 1u))
                    Marshal(node, (Ep1RepairUpdateModuleData*)*objT, state);
                uint repairTypeId = 0x0A2B8743u;
                state.InplaceEndianToPlatform(&repairTypeId);
                *(uint*)*objT = repairTypeId;
                break;
            case 0xC2F1EC20u:
                MarshalPolymorphicType<SupplyCenterDockUpdateModuleData, BehaviorModuleData>(node, objT, state);
                break;
            case 0xD6297BD1u:
                MarshalPolymorphicType<SupplyWarehouseDockUpdateModuleData, BehaviorModuleData>(node, objT, state);
                break;
            case 0xE42D5618u:
                // Reborn: retain the original aiming wire identity while applying Uprising-specific schema defaults.
                using(var context=state.Push((void**)objT,(uint)sizeof(AimWeaponBehaviorModuleData),1u))MarshalEp1AimWeapon(node,(AimWeaponBehaviorModuleData*)*objT,state);
                uint aimType=0xE42D5618u;state.InplaceEndianToPlatform(&aimType);*(uint*)*objT=aimType;
                break;
            case 0x3CFBDE26u:
                MarshalPolymorphicType<DistributedMoneyBehaviorModuleData, BehaviorModuleData>(node, objT, state);
                break;
            case 0x4CA37F3Eu:
                MarshalPolymorphicType<AutoAbilityBehaviorModuleData, BehaviorModuleData>(node, objT, state);
                break;
            case 0x0897B555u:
                MarshalPolymorphicType<FireWeaponWhenDamagedBehaviorModuleData, BehaviorModuleData>(node, objT, state);
                break;
            case 0x90CEB49Cu:
                MarshalPolymorphicType<FireWeaponWhenDeadBehaviorModuleData, BehaviorModuleData>(node, objT, state);
                break;
            case 0xBAE3BC7Du:
                MarshalPolymorphicType<GarrisonContainModuleData, BehaviorModuleData>(node, objT, state);
                break;
            // Reborn: register the missing RA3/EP1 contestable garrison type by its native name hash.
            case 0x8C50F0D7u:
                MarshalPolymorphicType<ContestableGarrisonContainModuleData, BehaviorModuleData>(node, objT, state);
                break;
            // Reborn: restore production queue containment dispatch using its native type-name hash.
            case 0xFE135CA0u:
                MarshalPolymorphicType<ProductionQueueHordeContainModuleData, BehaviorModuleData>(node, objT, state);
                break;
            // Reborn: retain the distinct leech type hash despite its identical native attach-base fields.
            case 0xCA6038A6u:
                MarshalPolymorphicType<LeechTargetingAttachUpdateModuleData, BehaviorModuleData>(node, objT, state);
                break;
            // Reborn: dispatch money-gain attach through its independent RA3/EP1 native type hash.
            case 0xB1A54585u:
                MarshalPolymorphicType<MoneyGainAttachUpdateModuleData, BehaviorModuleData>(node, objT, state);
                break;
            case 0xC2EBB54Du:
                MarshalPolymorphicType<ExperienceLevelCreateModuleData, BehaviorModuleData>(node, objT, state);
                break;
            case 0xC001089Bu:
                MarshalPolymorphicType<InheritUpgradeCreateModuleData, BehaviorModuleData>(node, objT, state);
                break;
            case 0x42409853u:
                MarshalPolymorphicType<DeletionUpdateModuleData, BehaviorModuleData>(node, objT, state);
                break;
            case 0xD3302D7Eu:
                MarshalPolymorphicType<EMPUpdateModuleData, BehaviorModuleData>(node, objT, state);
                break;
            case 0x5A7BCED3u:
                MarshalPolymorphicType<DelayedUpgradeModuleData, BehaviorModuleData>(node, objT, state);
                break;
            case 0x884BDE51u:
                MarshalPolymorphicType<ProductionSpeedBonusUpgradeModuleData, BehaviorModuleData>(node, objT, state);
                break;
            case 0x786C9458u:
                MarshalPolymorphicType<StatusBitsUpgradeModuleData, BehaviorModuleData>(node, objT, state);
                break;
            case 0x696245D9u:
                MarshalPolymorphicType<RadarUpdateModuleData, BehaviorModuleData>(node, objT, state);
                break;
            case 0x510D79C7u:
                MarshalPolymorphicType<UpgradeDieModuleData, BehaviorModuleData>(node, objT, state);
                break;
            case 0xD014A1C2u:
                MarshalPolymorphicType<KeepObjectDieModuleData, BehaviorModuleData>(node, objT, state);
                break;
            case 0xFBF6592Du:
                MarshalPolymorphicType<AutoHealBehaviorModuleData, BehaviorModuleData>(node, objT, state);
                break;
            case 0xE9BBF43Bu:
                // Reborn: EP1 parking has its own sixty-byte layout, distinct from the legacy KW record.
                using(var context=state.Push((void**)objT,(uint)sizeof(Ep1ParkingPlaceBehaviorModuleData),1u))
                    Marshal(node,(Ep1ParkingPlaceBehaviorModuleData*)*objT,state);
                uint parkingTypeId=0xE9BBF43Bu;state.InplaceEndianToPlatform(&parkingTypeId);*(uint*)*objT=parkingTypeId;
                break;
            case 0xE06AB787u:
                MarshalPolymorphicType<ExperienceScalarUpgradeModuleData, BehaviorModuleData>(node, objT, state);
                break;
            case 0x90F05519u:
                MarshalPolymorphicType<LevelGrantSpecialPowerModuleData, BehaviorModuleData>(node, objT, state);
                break;
            case 0xCDC0EFF7u:
                MarshalPolymorphicType<CostModifierUpgradeModuleData, BehaviorModuleData>(node, objT, state);
                break;
            case 0x93A6B8C0u:
                MarshalPolymorphicType<StealthUpgradeModuleData, BehaviorModuleData>(node, objT, state);
                break;
            case 0xB5EB41EDu:
                MarshalPolymorphicType<SquishCollideModuleData, BehaviorModuleData>(node, objT, state);
                break;
            case 0x29234E6Eu: // This seems to be here incorrectly
                MarshalPolymorphicType<InactiveBodyModuleData, BehaviorModuleData>(node, objT, state);
                break;
            case 0x025C0BD7u:
                MarshalPolymorphicType<DamageModuleData, BehaviorModuleData>(node, objT, state);
                break;
            case 0x2A79518Fu:
                MarshalPolymorphicType<DamagerModuleData, BehaviorModuleData>(node, objT, state);
                break;
            case 0xCB909BFEu:
                MarshalPolymorphicType<DestroyModuleData, BehaviorModuleData>(node, objT, state);
                break;
            case 0xF8079C05u:
                MarshalPolymorphicType<DamageStateListModuleData, BehaviorModuleData>(node, objT, state);
                break;
            case 0xCA9ED83Du:
                MarshalPolymorphicType<LaserUpdateModuleData, BehaviorModuleData>(node, objT, state);
                break;
            case 0xE7B6AF0Du:
                MarshalPolymorphicType<LaserStateModuleData, BehaviorModuleData>(node, objT, state);
                break;
            case 0x3C934753u:
                MarshalPolymorphicType<SweepingLaserStateModuleData, BehaviorModuleData>(node, objT, state);
                break;
            // Reborn: restore official converging-laser dispatch rather than dropping its native allocation.
            case 0x5F7498F9u:
                MarshalPolymorphicType<ConvergingLaserStateModuleData, BehaviorModuleData>(node, objT, state);
                break;
            case 0xFEF85F95u:
                MarshalPolymorphicType<StreamStateModuleData, BehaviorModuleData>(node, objT, state);
                break;
            case 0xA63DC46Bu:
                MarshalPolymorphicType<ActivateModuleSpecialPowerModuleData, BehaviorModuleData>(node, objT, state);
                break;
            case 0x5E58E0AEu:
                MarshalPolymorphicType<DualWeaponBehaviorModuleData, BehaviorModuleData>(node, objT, state);
                break;
            case 0xF8C4AA6Bu:
                MarshalPolymorphicType<CaptureBehaviorModuleData, BehaviorModuleData>(node, objT, state);
                break;
            case 0x2EB6C99Cu:
                MarshalPolymorphicType<StorePurchasedUpgradeBehaviorModuleData, BehaviorModuleData>(node, objT, state);
                break;
            case 0xA74A234Cu:
                MarshalPolymorphicType<StoreObjectsSpecialPowerModuleData, BehaviorModuleData>(node, objT, state);
                break;
            case 0xCDEF277Du:
                MarshalPolymorphicType<CloneStoredObjectsSpecialPowerUpdateModuleData, BehaviorModuleData>(node, objT, state);
                break;
            case 0x4E2D96C9u:
                // Reborn: Uprising adds MaxAircraftToRecall before the return-voice flag.
                using(var context=state.Push((void**)objT,(uint)sizeof(Ep1RecallUnitsSpecialPowerModuleData),1u))
                    Marshal(node,(Ep1RecallUnitsSpecialPowerModuleData*)*objT,state);
                uint recallTypeId=0x4E2D96C9u;state.InplaceEndianToPlatform(&recallTypeId);*(uint*)*objT=recallTypeId;
                break;
            case 0x28004AB1u:
                MarshalPolymorphicType<ReturnToProducerSpecialPowerModuleData, BehaviorModuleData>(node, objT, state);
                break;
            case 0x07FEA222u:
                MarshalPolymorphicType<ReturnToDockSpecialPowerModuleData, BehaviorModuleData>(node, objT, state);
                break;
            case 0x1D7FC977u:
                MarshalPolymorphicType<HordeGarrisonContainModuleData, BehaviorModuleData>(node, objT, state);
                break;
            case 0xB4841EABu:
                MarshalPolymorphicType<TunnelContainModuleData, BehaviorModuleData>(node, objT, state);
                break;
            case 0x8EC7A787u:
                MarshalPolymorphicType<TunnelTeleportContainModuleData, BehaviorModuleData>(node, objT, state);
                break;
            case 0xC1947EDCu:
                MarshalPolymorphicType<SlaughterHordeContainModuleData, BehaviorModuleData>(node, objT, state);
                break;
            case 0x35634018u:
                MarshalPolymorphicType<InvisibilityUpdateModuleData, BehaviorModuleData>(node, objT, state);
                break;
#if TIBERIUMWARS
            case 0xD2C1CFC2u: // actually present in KW, enable?
                MarshalPolymorphicType<InvisibilitySpecialPowerModuleData, BehaviorModuleData>(node, objT, state);
                break;
#endif
            case 0xF46EB384u:
                MarshalPolymorphicType<PowerUpdateModuleData, BehaviorModuleData>(node, objT, state);
                break;
            case 0x9776876Eu:
                MarshalPolymorphicType<SupplyCenterCreateModuleData, BehaviorModuleData>(node, objT, state);
                break;
            case 0x264EC61Au:
                MarshalPolymorphicType<GatherSlavesUpdateModuleData, BehaviorModuleData>(node, objT, state);
                break;
            case 0xC0A1E43Du:
                MarshalPolymorphicType<StancesBehaviorModuleData, BehaviorModuleData>(node, objT, state);
                break;
            case 0x311B2FAFu:
                MarshalPolymorphicType<RubbleRiseUpdateModuleData, BehaviorModuleData>(node, objT, state);
                break;
            case 0x98270BD0u:
                MarshalPolymorphicType<FiringTrackerModuleData, BehaviorModuleData>(node, objT, state);
                break;
            case 0x54203FA0u:
                MarshalPolymorphicType<SpecialPowerTimerRefreshSpecialPowerModuleData, BehaviorModuleData>(node, objT, state);
                break;
            case 0x18A4208Fu:
                MarshalPolymorphicType<ObjectDefectionHelperModuleData, BehaviorModuleData>(node, objT, state);
                break;
            case 0x0BA45362u:
                MarshalPolymorphicType<ClickReactionBehaviorModuleData, BehaviorModuleData>(node, objT, state);
                break;
            case 0x59D12990u:
                MarshalPolymorphicType<HitReactionBehaviorModuleData, BehaviorModuleData>(node, objT, state);
                break;
            case 0x55F9E6EDu:
                MarshalPolymorphicType<ReplaceObjectUpdateModuleData, BehaviorModuleData>(node, objT, state);
                break;
            case 0x006DBEBFu:
                MarshalPolymorphicType<HelicopterSlowDeathBehaviorModuleData, BehaviorModuleData>(node, objT, state);
                break;
            case 0x0EA26C39u:
                MarshalPolymorphicType<ClearanceTestingSlowDeathBehaviorModuleData, BehaviorModuleData>(node, objT, state);
                break;
            case 0xE9FD14C9u:
                // Reborn: select the complete EP1 layout and retain the original runtime class ID.
                using(var context=state.Push((void**)objT,(uint)sizeof(Ep1AttachUpdateModuleData),1u))Marshal(node,(Ep1AttachUpdateModuleData*)*objT,state);
                *(uint*)*objT=0xE9FD14C9u;
                break;
            case 0xE502295Eu:
                MarshalPolymorphicType<BoobyTrapUpdateModuleData, BehaviorModuleData>(node, objT, state);
                break;
            case 0x21068FF6u:
                MarshalPolymorphicType<StealUnitUpdateModuleData, BehaviorModuleData>(node, objT, state);
                break;
            case 0x4A32BF8Eu:
                MarshalPolymorphicType<SpawnCrateUpdateModuleData, BehaviorModuleData>(node, objT, state);
                break;
            case 0xA2F69397u:
                MarshalPolymorphicType<BridgeBuilderModuleData, BehaviorModuleData>(node, objT, state);
                break;
            case 0x08E25591u:
                // Reborn: select the complete EP1 passive-effect layout while retaining the original concrete wire ID.
                using(var context=state.Push((void**)objT,(uint)sizeof(Ep1PassiveAreaEffectBehaviorModuleData),1u))Marshal(node,(Ep1PassiveAreaEffectBehaviorModuleData*)*objT,state);
                *(uint*)*objT=0x08E25591u;
                break;
            case 0xC3A62C68u:
                MarshalPolymorphicType<PowerUpgradeModuleData, BehaviorModuleData>(node, objT, state);
                break;
            case 0x38C496D8u:
                MarshalPolymorphicType<CreateAndEnterObjectSpecialPowerModuleData, BehaviorModuleData>(node, objT, state);
                break;
            case 0xAC7E2A4Au:
                MarshalPolymorphicType<MoveToPositionAndEvacuateSpecialPowerModuleData, BehaviorModuleData>(node, objT, state);
                break;
            case 0xD6791F61u:
                MarshalPolymorphicType<MoveToPositionAndReplaceSelfSpecialPowerModuleData, BehaviorModuleData>(node, objT, state);
                break;
            case 0x877ACEADu:
                MarshalPolymorphicType<RunOffMapBehaviorModuleData, BehaviorModuleData>(node, objT, state);
                break;
            case 0xBCFB5BB7u:
                MarshalPolymorphicType<SuppressionUpdateModuleData, BehaviorModuleData>(node, objT, state);
                break;
            case 0xE622F055u:
                MarshalPolymorphicType<AttributeModifierPoolUpdateModuleData, BehaviorModuleData>(node, objT, state);
                break;
            case 0xF3F8ADFAu:
                MarshalPolymorphicType<WallHubBehaviorModuleData, BehaviorModuleData>(node, objT, state);
                break;
            case 0xE9D1B7EBu:
                // Reborn: EP1 StructurePlacementBehavior (stock-qualified 56-byte record with inline EP1 Shape).
                using (var context = state.Push((void**)objT, (uint)sizeof(StructurePlacementBehaviorModuleData), 1u))
                    Marshal(node, (StructurePlacementBehaviorModuleData*)*objT, state);
                uint structurePlacementTypeId = 0xE9D1B7EBu;
                state.InplaceEndianToPlatform(&structurePlacementTypeId);
                *(uint*)*objT = structurePlacementTypeId;
                break;
            case 0xAE0EE9A0u:
                // Reborn: EP1 AssistedPlacementBehavior (stock-qualified 20-byte record with optional requirement filter).
                using (var context = state.Push((void**)objT, (uint)sizeof(AssistedPlacementBehaviorModuleData), 1u))
                    Marshal(node, (AssistedPlacementBehaviorModuleData*)*objT, state);
                uint placementTypeId = 0xAE0EE9A0u;
                state.InplaceEndianToPlatform(&placementTypeId);
                *(uint*)*objT = placementTypeId;
                break;
            case 0x977F8391u:
                // Reborn: use the full EP1 GenericUnpack base while retaining the real StructureUnpack wire identity.
                using (var context = state.Push((void**)objT, (uint)sizeof(Ep1StructureUnpackUpdateModuleData), 1u))
                    Marshal(node, (Ep1StructureUnpackUpdateModuleData*)*objT, state);
                uint unpackTypeId = 0x977F8391u;
                state.InplaceEndianToPlatform(&unpackTypeId);
                *(uint*)*objT = unpackTypeId;
                break;
            case 0x6083010Au:
                MarshalPolymorphicType<GenericUnpackUpdateModuleData, BehaviorModuleData>(node, objT, state);
                break;
            case 0x7F7F2A1Au:
                MarshalPolymorphicType<UnitUnpackUpdateModuleData, BehaviorModuleData>(node, objT, state);
                break;
            case 0x384E25E7u:
                MarshalPolymorphicType<BannerCarrierUpgradeModuleData, BehaviorModuleData>(node, objT, state);
                break;
            case 0xBEB7D7A3u:
                MarshalPolymorphicType<FindCoverBehaviorModuleData, BehaviorModuleData>(node, objT, state);
                break;
            case 0x146948BCu:
                MarshalPolymorphicType<HighlightReachableTargetsSpecialAbilityUpdateModuleData, BehaviorModuleData>(node, objT, state);
                break;
            case 0x39B825FFu:
                MarshalPolymorphicType<RepairSpecialPowerModuleData, BehaviorModuleData>(node, objT, state);
                break;
            case 0xDA2F4EE5u:
                MarshalPolymorphicType<RadarScanMapSpecialAbilityUpdateModuleData, BehaviorModuleData>(node, objT, state);
                break;
            case 0x953C7F5Bu:
                MarshalPolymorphicType<RestrictSpecialPowerBehaviorModuleData, BehaviorModuleData>(node, objT, state);
                break;
            case 0x47E7EAC9u:
                MarshalPolymorphicType<AreaRestrictSpecialPowerBehaviorModuleData, BehaviorModuleData>(node, objT, state);
                break;
            case 0x0FEC21CBu:
                MarshalPolymorphicType<MonitorConditionUpdateModuleData, BehaviorModuleData>(node, objT, state);
                break;
            case 0x8DA12662u:
                MarshalPolymorphicType<MonitorSpecialPowerTimerUpdateModuleData, BehaviorModuleData>(node, objT, state);
                break;
            case 0xDFD38381u:
                MarshalPolymorphicType<ShroudClearBehaviorModuleData, BehaviorModuleData>(node, objT, state);
                break;
            case 0x1E5C747Au:
                MarshalPolymorphicType<LinearDamageUpdateModuleData, BehaviorModuleData>(node, objT, state);
                break;
            case 0xB956A21Au:
                MarshalPolymorphicType<GrantUpgradeAreaOfEffectSpecialPowerModuleData, BehaviorModuleData>(node, objT, state);
                break;
            case 0x2F4536A8u:
                MarshalPolymorphicType<SetLocomotorSpeedUpdateModuleData, BehaviorModuleData>(node, objT, state);
                break;
            case 0x7C27AD44u:
                MarshalPolymorphicType<DestroyObjectsUpdateModuleData, BehaviorModuleData>(node, objT, state);
                break;
            case 0x2E021907u:
                // Reborn: EP1 player-power managers include the optional EVA event import.
                using(var context=state.Push((void**)objT,(uint)sizeof(Ep1PlayerPowerManagerBehaviorModuleData),1u))
                    Marshal(node,(Ep1PlayerPowerManagerBehaviorModuleData*)*objT,state);
                uint playerPowerManagerTypeId=0x2E021907u;state.InplaceEndianToPlatform(&playerPowerManagerTypeId);*(uint*)*objT=playerPowerManagerTypeId;
                break;
#if TIBERIUMWARS
            case 0x9E322E47u: // actually present in KW, enable?
                MarshalPolymorphicType<CashHackSpecialPowerModuleData, BehaviorModuleData>(node, objT, state);
                break;
            case 0xAA507848u: // actually present in KW, enable?
                MarshalPolymorphicType<RespawnUpdateModuleData, BehaviorModuleData>(node, objT, state);
                break;
            case 0xB2483B20u: // actually present in KW, enable?
                MarshalPolymorphicType<TooltipUpgradeModuleData, BehaviorModuleData>(node, objT, state);
                break;
            case 0x36544EBAu: // actually present in KW, enable?
                MarshalPolymorphicType<CreateCrateDie, BehaviorModuleData>(node, objT, state);
                break;
#endif
            case 0x2827372Cu: // manually added in both
                MarshalPolymorphicType<ShareExperienceBehaviorModuleData, BehaviorModuleData>(node, objT, state);
                break;
#if KANESWRATH
            case 0x47D32CF8u:
                MarshalPolymorphicType<MetaUpgradeAuraBehaviorModuleData, BehaviorModuleData>(node, objT, state);
                break;
            case 0x68651C22u:
                MarshalPolymorphicType<MetaGameStratButtonInfoModuleData, BehaviorModuleData>(node, objT, state);
                break;
            case 0xA5FAC7DFu: // actually present in TW, enable?
                MarshalPolymorphicType<SpecialDisguiseUpdateModuleData, BehaviorModuleData>(node, objT, state);
                break;
            case 0x7BB5E0ADu:
                MarshalPolymorphicType<FloodUpdateModuleData, BehaviorModuleData>(node, objT, state);
                break;
            case 0xB47C1AC1u:
                MarshalPolymorphicType<SalvagerInfoModuleData, BehaviorModuleData>(node, objT, state);
                break;
#endif
            // Reborn: ore delivery and its armor-mode trackers each use their concrete retail module identity and layout.
            case 0x90F997EFu: MarshalPolymorphicType<OreMinerModuleData,BehaviorModuleData>(node,objT,state);break;
            case 0x15E9B3E0u: MarshalPolymorphicType<TrackIncomingWeaponFireBehaviorModuleData,BehaviorModuleData>(node,objT,state);break;
            case 0xD554889Du: MarshalPolymorphicType<TrackDamageModuleData,BehaviorModuleData>(node,objT,state);break;
            // Reborn: the EP1 dynamics update retains its own complete layout rather than unknown-module fallback.
            case 0x155A87BBu:
                MarshalPolymorphicType<DynamicsUpdateModuleData,BehaviorModuleData>(node,objT,state);
                break;
            default:
                Tracer.GetTracer("Marshaler", "Marshalling").TraceWarning("Marshalling unknown type {0} from {1}.", typeId, nameof(PolymorphicList<BehaviorModuleData>));
                MarshalUnknownPolymorphicType(node, objT, state);
                break;
        }
    }

    private static unsafe void Marshal(Node node, AutoResolveInfo* objT, Tracker state)
    {
        if (node is null)
        {
            return;
        }
        Marshal(node.GetAttributeValue(nameof(AutoResolveInfo.AutoResolveUnitType), null), &objT->AutoResolveUnitType, state);
        Marshal(node.GetAttributeValue(nameof(AutoResolveInfo.AutoResolveBody), null), &objT->AutoResolveBody, state);
        Marshal(node.GetAttributeValue(nameof(AutoResolveInfo.AutoResolveArmor), null), &objT->AutoResolveArmor, state);
        Marshal(node.GetAttributeValue(nameof(AutoResolveInfo.AutoResolveWeapon), null), &objT->AutoResolveWeapon, state);
        Marshal(node.GetAttributeValue(nameof(AutoResolveInfo.AutoResolveLeadership), null), &objT->AutoResolveLeadership, state);
        Marshal(node.GetAttributeValue(nameof(AutoResolveInfo.AutoResolveCombatChain), null), &objT->AutoResolveCombatChain, state);
    }

    private static unsafe void Marshal(Node node, AutoResolveInfo** objT, Tracker state)
    {
        if (node is null)
        {
            return;
        }
        using Tracker.Context context = state.Push((void**)objT, (uint)sizeof(AutoResolveInfo), 1u);
        Marshal(node, *objT, state);
    }

    public static unsafe void Marshal(Node node, ProjectedBuildabilityInfo* objT, Tracker state)
    {
        if (node is null)
        {
            return;
        }
        Marshal(node.GetAttributeValue(nameof(ProjectedBuildabilityInfo.Radius), "0"), &objT->Radius, state);
        Marshal(node.GetAttributeValue(nameof(ProjectedBuildabilityInfo.RadiusY), null), &objT->RadiusY, state);
        Marshal(node.GetAttributeValue(nameof(ProjectedBuildabilityInfo.BuildPlacementTypes), "INVALID"), &objT->BuildPlacementTypes, state);
        Marshal(node.GetAttributeValue(nameof(ProjectedBuildabilityInfo.StatusToReject), null), &objT->StatusToReject, state);
        Marshal(node.GetAttributeValue(nameof(ProjectedBuildabilityInfo.ModelConditionsToReject), "STRUCTURE_UNPACKING"), &objT->ModelConditionsToReject, state);
        Marshal(node.GetAttributeValue(nameof(ProjectedBuildabilityInfo.AllowedBuildabilityHeightVariation), "50.0"), &objT->AllowedBuildabilityHeightVariation, state);
        Marshal(node.GetChildNode(nameof(ProjectedBuildabilityInfo.AllowedObjectFilter), null), &objT->AllowedObjectFilter, state);
        Marshal(node.GetAttributeValue(nameof(ProjectedBuildabilityInfo.PrimaryBuidability), "true"), &objT->PrimaryBuidability, state);
    }

    private static unsafe void Marshal(Node node, ProjectedBuildabilityInfo** objT, Tracker state)
    {
        if (node is null)
        {
            return;
        }
        using Tracker.Context context = state.Push((void**)objT, (uint)sizeof(ProjectedBuildabilityInfo), 1u);
        Marshal(node, *objT, state);
    }

    public static unsafe void Marshal(Node node, ArmorTemplateSet* objT, Tracker state)
    {
        if (node is null)
        {
            return;
        }
        Marshal(node.GetAttributeValue(nameof(ArmorTemplateSet.Armor), null), &objT->Armor, state);
        Marshal(node.GetAttributeValue(nameof(ArmorTemplateSet.DamageFX), null), &objT->DamageFX, state);
        Marshal(node.GetAttributeValue(nameof(ArmorTemplateSet.Conditions), null), &objT->Conditions, state);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: marshal the EP1 root record with its independently reviewed condition policy; missing fields retain native zero defaults. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node, Ep1LocomotorSet* objT, Tracker state)
    {
        if (node is null) return;
        Marshal(node.GetAttributeValue(nameof(Ep1LocomotorSet.Speed), null), &objT->Speed, state);
        Marshal(node.GetAttributeValue(nameof(Ep1LocomotorSet.Condition), null), &objT->Condition, state);
        Marshal(node.GetAttributeValue(nameof(Ep1LocomotorSet.Locomotor), null), &objT->Locomotor, state);
    }

    public static unsafe void Marshal(Node node, LocomotorSet* objT, Tracker state)
    {
        if (node is null)
        {
            return;
        }
        Marshal(node.GetAttributeValue(nameof(LocomotorSet.Speed), null), &objT->Speed, state);
        Marshal(node.GetAttributeValue(nameof(LocomotorSet.Condition), null), &objT->Condition, state);
        Marshal(node.GetAttributeValue(nameof(LocomotorSet.Locomotor), null), &objT->Locomotor, state);
    }

    public static unsafe void Marshal(Node node, Flammability* objT, Tracker state)
    {
        if (node is null)
        {
            return;
        }
        Marshal(node.GetAttributeValue(nameof(Flammability.Fuel), "0"), &objT->Fuel, state);
        Marshal(node.GetAttributeValue(nameof(Flammability.MaxBurnRate), "0"), &objT->MaxBurnRate, state);
        Marshal(node.GetAttributeValue(nameof(Flammability.Decay), "0"), &objT->Decay, state);
        Marshal(node.GetAttributeValue(nameof(Flammability.Resistance), "0"), &objT->Resistance, state);
    }

    private static unsafe void Marshal(Node node, Flammability** objT, Tracker state)
    {
        if (node is null)
        {
            return;
        }
        using Tracker.Context context = state.Push((void**)objT, (uint)sizeof(Flammability), 1u);
        Marshal(node, *objT, state);
    }

    public static unsafe void Marshal(Node node, SkirmishAIInformation* objT, Tracker state)
    {
        if (node is null)
        {
            return;
        }
        Marshal(node.GetAttributeValue(nameof(SkirmishAIInformation.BaseBuildingLocation), "FRONT"), &objT->BaseBuildingLocation, state);
        Marshal(node.GetAttributeValue(nameof(SkirmishAIInformation.UnitBuilderStandardCombatUnit), "false"), &objT->UnitBuilderStandardCombatUnit, state);
        Marshal(node.GetAttributeValue(nameof(SkirmishAIInformation.ConquerMetricsOverrideDPS), null), &objT->ConquerMetricsOverrideDPS, state);
        Marshal(node.GetAttributeValue(nameof(SkirmishAIInformation.ConquerMetricsOverrideDamageType), "UNDEFINED"), &objT->ConquerMetricsOverrideDamageType, state);
        Marshal(node.GetAttributeValue(nameof(SkirmishAIInformation.ConquerMetricsOverrideAntiMask), "ANTI_GROUND"), &objT->ConquerMetricsOverrideAntiMask, state);
    }

    private static unsafe void Marshal(Node node, SkirmishAIInformation** objT, Tracker state)
    {
        if (node is null)
        {
            return;
        }
        using Tracker.Context context = state.Push((void**)objT, (uint)sizeof(SkirmishAIInformation), 1u);
        Marshal(node, *objT, state);
    }

    public static unsafe void Marshal(Node node, PerUnitFX* objT, Tracker state)
    {
        if (node is null)
        {
            return;
        }
    }

    private static unsafe void Marshal(Node node, PerUnitFX** objT, Tracker state)
    {
        if (node is null)
        {
            return;
        }
        using Tracker.Context context = state.Push((void**)objT, (uint)sizeof(PerUnitFX), 1u);
        Marshal(node, *objT, state);
    }

    public static unsafe void Marshal(Node node, ReplaceModule* objT, Tracker state)
    {
        if (node is null)
        {
            return;
        }
    }

    private static unsafe void Marshal(Node node, ReplaceModule** objT, Tracker state)
    {
        if (node is null)
        {
            return;
        }
        using Tracker.Context context = state.Push((void**)objT, (uint)sizeof(ReplaceModule), 1u);
        Marshal(node, *objT, state);
    }

    public static unsafe void Marshal(Node node, InheritableModule* objT, Tracker state)
    {
        if (node is null)
        {
            return;
        }
    }

    private static unsafe void Marshal(Node node, InheritableModule** objT, Tracker state)
    {
        if (node is null)
        {
            return;
        }
        using Tracker.Context context = state.Push((void**)objT, (uint)sizeof(InheritableModule), 1u);
        Marshal(node, *objT, state);
    }

    public static unsafe void Marshal(Node node, AutoResolveArmor* objT, Tracker state)
    {
        if (node is null)
        {
            return;
        }
        Marshal(node.GetAttributeValue(nameof(AutoResolveArmor.Armor), null), &objT->Armor, state);
        Marshal(node.GetChildNodes(nameof(AutoResolveArmor.RequiredUpgrade)), &objT->RequiredUpgrade, state);
        Marshal(node.GetChildNodes(nameof(AutoResolveArmor.ExcludedUpgrade)), &objT->ExcludedUpgrade, state);
    }

    private static unsafe void Marshal(Node node, AutoResolveArmor** objT, Tracker state)
    {
        if (node is null)
        {
            return;
        }
        using Tracker.Context context = state.Push((void**)objT, (uint)sizeof(AutoResolveArmor), 1u);
        Marshal(node, *objT, state);
    }

    public static unsafe void Marshal(Node node, AutoResolveWeapon* objT, Tracker state)
    {
        if (node is null)
        {
            return;
        }
        Marshal(node.GetAttributeValue(nameof(AutoResolveWeapon.Weapon), null), &objT->Weapon, state);
        Marshal(node.GetChildNodes(nameof(AutoResolveWeapon.RequiredUpgrade)), &objT->RequiredUpgrade, state);
        Marshal(node.GetChildNodes(nameof(AutoResolveWeapon.ExcludedUpgrade)), &objT->ExcludedUpgrade, state);
    }

    private static unsafe void Marshal(Node node, AutoResolveWeapon** objT, Tracker state)
    {
        if (node is null)
        {
            return;
        }
        using Tracker.Context context = state.Push((void**)objT, (uint)sizeof(AutoResolveWeapon), 1u);
        Marshal(node, *objT, state);
    }

    public static unsafe void Marshal(Node node, FormationPreviewDecal* objT, Tracker state)
    {
        if (node is null)
        {
            return;
        }
        Marshal(node.GetAttributeValue(nameof(FormationPreviewDecal.Texture), null), &objT->Texture, state);
        Marshal(node.GetAttributeValue(nameof(FormationPreviewDecal.Width), "50.0"), &objT->Width, state);
        Marshal(node.GetAttributeValue(nameof(FormationPreviewDecal.Height), "50.0"), &objT->Height, state);
    }

    private static unsafe void Marshal(Node node, FormationPreviewDecal** objT, Tracker state)
    {
        if (node is null)
        {
            return;
        }
        using Tracker.Context context = state.Push((void**)objT, (uint)sizeof(FormationPreviewDecal), 1u);
        Marshal(node, *objT, state);
    }

    public static unsafe void Marshal(Node node, VisionInfo* objT, Tracker state)
    {
        if (node is null)
        {
            return;
        }
        Marshal(node.GetAttributeValue(nameof(VisionInfo.VisionRange), "0.00"), &objT->VisionRange, state);
        Marshal(node.GetAttributeValue(nameof(VisionInfo.ShroudClearingRange), "0.0"), &objT->ShroudClearingRange, state);
        Marshal(node.GetAttributeValue(nameof(VisionInfo.VisionSide), "100"), &objT->VisionSide, state);
        Marshal(node.GetAttributeValue(nameof(VisionInfo.VisionRear), "100"), &objT->VisionRear, state);
        Marshal(node.GetAttributeValue(nameof(VisionInfo.VisionBonusTestRadius), "100"), &objT->VisionBonusTestRadius, state);
        Marshal(node.GetAttributeValue(nameof(VisionInfo.VisionBonusTestSegments), "8"), &objT->VisionBonusTestSegments, state);
        Marshal(node.GetAttributeValue(nameof(VisionInfo.VisionBonusPercentPerFoot), "0"), &objT->VisionBonusPercentPerFoot, state);
        Marshal(node.GetAttributeValue(nameof(VisionInfo.MaxVisionBonusPercent), "50"), &objT->MaxVisionBonusPercent, state);
        Marshal(node.GetAttributeValue(nameof(VisionInfo.MinVisionBonusPercent), "-50"), &objT->MinVisionBonusPercent, state);
    }

    private static unsafe void Marshal(Node node, VisionInfo** objT, Tracker state)
    {
        if (node is null)
        {
            return;
        }
        using Tracker.Context context = state.Push((void**)objT, (uint)sizeof(VisionInfo), 1u);
        Marshal(node, *objT, state);
    }

    public static unsafe void Marshal(Node node, CrusherInfo* objT, Tracker state)
    {
        if (node is null)
        {
            return;
        }
        Marshal(node.GetAttributeValue(nameof(CrusherInfo.CrusherLevel), "0"), &objT->CrusherLevel, state);
        Marshal(node.GetAttributeValue(nameof(CrusherInfo.CrushableLevel), "127"), &objT->CrushableLevel, state);
        Marshal(node.GetAttributeValue(nameof(CrusherInfo.CrushKnockback), "0"), &objT->CrushKnockback, state);
        Marshal(node.GetAttributeValue(nameof(CrusherInfo.CrushZFactor), "1"), &objT->CrushZFactor, state);
        Marshal(node.GetAttributeValue(nameof(CrusherInfo.CrushEqualLevelProps), "false"), &objT->CrushEqualLevelProps, state);
        Marshal(node.GetAttributeValue(nameof(CrusherInfo.UseCrushAttack), "true"), &objT->UseCrushAttack, state);
        Marshal(node.GetAttributeValue(nameof(CrusherInfo.CrushOnlyWhileCharging), "false"), &objT->CrushOnlyWhileCharging, state);
        Marshal(node.GetAttributeValue(nameof(CrusherInfo.MinCrushVelocityPercent), "1"), &objT->MinCrushVelocityPercent, state);
        Marshal(node.GetAttributeValue(nameof(CrusherInfo.CrushDecelerationPercent), "0"), &objT->CrushDecelerationPercent, state);
        Marshal(node.GetAttributeValue(nameof(CrusherInfo.CrushAllies), "false"), &objT->CrushAllies, state);
        Marshal(node.GetAttributeValue(nameof(CrusherInfo.CrushWeapon), null), &objT->CrushWeapon, state);
        Marshal(node.GetAttributeValue(nameof(CrusherInfo.CrushAircraftWhileStationary), "false"), &objT->CrushAircraftWhileStationary, state);
        Marshal(node.GetAttributeValue(nameof(CrusherInfo.DefaultCrushKillDelay), ".5s"), &objT->DefaultCrushKillDelay, state);
        Marshal(node.GetAttributeValue(nameof(CrusherInfo.UseDirectionCheck), "true"), &objT->UseDirectionCheck, state);
        Marshal(node.GetAttributeValue(nameof(CrusherInfo.ShouldSquishOnCollide), "true"), &objT->ShouldSquishOnCollide, state);
        Marshal(node.GetAttributeValue(nameof(CrusherInfo.DecelerateForAllCollides), "false"), &objT->DecelerateForAllCollides, state);
        Marshal(node.GetAttributeValue(nameof(CrusherInfo.CannotCrushTarget), "false"), &objT->CannotCrushTarget, state);
        Marshal(node.GetChildNodes(nameof(CrusherInfo.ExtraCrushLevels)), &objT->ExtraCrushLevels, state);
        Marshal(node.GetChildNodes(nameof(CrusherInfo.ExtraCrushKillDelays)), &objT->ExtraCrushKillDelays, state);
    }

    public static unsafe void Marshal(Node node, CrusherLevelModelConditionInfo* objT, Tracker state)
    {
        if (node is null) return;
        Marshal(node.GetAttributeValue(nameof(CrusherLevelModelConditionInfo.ModelConditionMatch), null), &objT->ModelConditionMatch, state);
        Marshal(node.GetChildNode(nameof(CrusherLevelModelConditionInfo.ObjectFilter), null), &objT->ObjectFilter, state);
        Marshal(node.GetAttributeValue(nameof(CrusherLevelModelConditionInfo.CrusherLevel), "0"), &objT->CrusherLevel, state);
        Marshal(node.GetAttributeValue(nameof(CrusherLevelModelConditionInfo.CrushableLevel), "127"), &objT->CrushableLevel, state);
        Marshal(node.GetAttributeValue(nameof(CrusherLevelModelConditionInfo.CrushEqualLevelProps), "false"), &objT->CrushEqualLevelProps, state);
    }

    public static unsafe void Marshal(Node node, CrushKillDelayForObjectFilter* objT, Tracker state)
    {
        if (node is null) return;
        Marshal(node.GetAttributeValue(nameof(CrushKillDelayForObjectFilter.CrushKillDelay), ".5s"), &objT->CrushKillDelay, state);
        Marshal(node.GetAttributeValue(nameof(CrushKillDelayForObjectFilter.CrushDecelerationPercent), "0"), &objT->CrushDecelerationPercent, state);
        Marshal(node.GetChildNode(nameof(CrushKillDelayForObjectFilter.ObjectFilter), null), &objT->ObjectFilter, state);
    }

    private static unsafe void Marshal(Node node, CrusherInfo** objT, Tracker state)
    {
        if (node is null)
        {
            return;
        }
        using Tracker.Context context = state.Push((void**)objT, (uint)sizeof(CrusherInfo), 1u);
        Marshal(node, *objT, state);
    }

    public static unsafe void Marshal(Node node, MoneyTransaction* objT, Tracker state)
    {
        if (node is null) return;
        Marshal(node.GetAttributeValue(nameof(MoneyTransaction.Account), null), &objT->Account, state);
        Marshal(node.GetAttributeValue(nameof(MoneyTransaction.Amount), null), &objT->Amount, state);
    }

    public static unsafe void Marshal(Node node, ObjectResourceInfo* objT, Tracker state)
    {
        if (node is null) return;
        Marshal(node.GetChildNodes(nameof(ObjectResourceInfo.BuildCost)), &objT->BuildCost, state);
    }

    public static unsafe void Marshal(Node node, ObjectResourceInfo** objT, Tracker state)
    {
        if (node is null) return;
        using Tracker.Context context = state.Push((void**)objT, (uint)sizeof(ObjectResourceInfo), 1u);
        Marshal(node, *objT, state);
    }

    public static unsafe void Marshal(Node node, GameObject* objT, Tracker state)
    {
        if (node is null)
        {
            return;
        }
        Marshal(node.GetAttributeValue(nameof(GameObject.KindOf), null), &objT->KindOf, state);
        Marshal(node.GetAttributeValue(nameof(GameObject.Browser), null), &objT->Browser, state);
        Marshal(node.GetAttributeValue(nameof(GameObject.Description), null), &objT->Description, state);
        Marshal(node.GetAttributeValue(nameof(GameObject.DescriptionTransformed), null), &objT->DescriptionTransformed, state);
        Marshal(node.GetAttributeValue(nameof(GameObject.TypeDescription), null), &objT->TypeDescription, state);
        Marshal(node.GetAttributeValue(nameof(GameObject.TypeDescriptionTransformed), null), &objT->TypeDescriptionTransformed, state);
        // Reborn: official EP1 RadarPriority defaults to INVALID=-1 rather than zero/STRUCTURE when omitted.
        Marshal(node.GetAttributeValue(nameof(GameObject.RadarPriority), "INVALID"), &objT->RadarPriority, state);
        Marshal(node.GetAttributeValue(nameof(GameObject.TransportSlotCount), "0"), &objT->TransportSlotCount, state);
        Marshal(node.GetAttributeValue(nameof(GameObject.FenceWidth), "0"), &objT->FenceWidth, state);
        Marshal(node.GetAttributeValue(nameof(GameObject.FenceXOffset), "0"), &objT->FenceXOffset, state);
        Marshal(node.GetAttributeValue(nameof(GameObject.RemoveTerrainRadius), "0.0"), &objT->RemoveTerrainRadius, state);
        Marshal(node.GetAttributeValue(nameof(GameObject.EmotionRange), "0"), &objT->EmotionRange, state);
        Marshal(node.GetAttributeValue(nameof(GameObject.PlacementViewAngle), "0r"), &objT->PlacementViewAngle, state);
        Marshal(node.GetAttributeValue(nameof(GameObject.JustBuiltTime), "0s"), &objT->JustBuiltTime, state);
        Marshal(node.GetAttributeValue(nameof(GameObject.FactoryExitWidth), null), &objT->FactoryExitWidth, state);
        Marshal(node.GetAttributeValue(nameof(GameObject.FactoryExtraBibWidth), null), &objT->FactoryExtraBibWidth, state);
        Marshal(node.GetAttributeValue(nameof(GameObject.IsTrainable), "true"), &objT->IsTrainable, state);
        Marshal(node.GetAttributeValue(nameof(GameObject.Side), null), &objT->Side, state);
        Marshal(node.GetAttributeValue(nameof(GameObject.EditorName), null), &objT->EditorName, state);
        Marshal(node.GetAttributeValue(nameof(GameObject.EditorSorting), "NONE"), &objT->EditorSorting, state);
        Marshal(node.GetAttributeValue(nameof(GameObject.BountyValue), "0"), &objT->BountyValue, state);
        Marshal(node.GetAttributeValue(nameof(GameObject.BuildTime), "0.0"), &objT->BuildTime, state);
        Marshal(node.GetAttributeValue(nameof(GameObject.RefundValue), "0"), &objT->RefundValue, state);
        Marshal(node.GetAttributeValue(nameof(GameObject.EnergyProduction), "0"), &objT->EnergyProduction, state);
        Marshal(node.GetAttributeValue(nameof(GameObject.EnergyBonus), "0"), &objT->EnergyBonus, state);
        Marshal(node.GetAttributeValue(nameof(GameObject.IsForbidden), "false"), &objT->IsForbidden, state);
        Marshal(node.GetAttributeValue(nameof(GameObject.IsPrerequisite), "false"), &objT->IsPrerequisite, state);
        Marshal(node.GetAttributeValue(nameof(GameObject.IsGrabbable), "false"), &objT->IsGrabbable, state);
        Marshal(node.GetAttributeValue(nameof(GameObject.IsHarvestable), "false"), &objT->IsHarvestable, state);
        Marshal(node.GetAttributeValue(nameof(GameObject.CommandSet), null), &objT->CommandSet, state);
        Marshal(node.GetAttributeValue(nameof(GameObject.SelectPortrait), null), &objT->SelectPortrait, state);
        Marshal(node.GetAttributeValue(nameof(GameObject.SelectPortraitTransformed), null), &objT->SelectPortraitTransformed, state);
        Marshal(node.GetAttributeValue(nameof(GameObject.ButtonImage), null), &objT->ButtonImage, state);
        Marshal(node.GetAttributeValue(nameof(GameObject.ButtonImageTransformed), null), &objT->ButtonImageTransformed, state);
        Marshal(node.GetAttributeValue(nameof(GameObject.VoicePriority), null), &objT->VoicePriority, state);
        Marshal(node.GetAttributeValue(nameof(GameObject.MinZIncreaseForVoiceMoveToHigherGround), "-1"), &objT->MinZIncreaseForVoiceMoveToHigherGround, state);
        Marshal(node.GetAttributeValue(nameof(GameObject.CrowdResponse), null), &objT->CrowdResponse, state);
        Marshal(node.GetAttributeValue(nameof(GameObject.CampnessValue), "0"), &objT->CampnessValue, state);
        Marshal(node.GetAttributeValue(nameof(GameObject.CampnessValueRadius), "600.0"), &objT->CampnessValueRadius, state);
        Marshal(node.GetAttributeValue(nameof(GameObject.Scale), "1.0"), &objT->Scale, state);
        Marshal(node.GetAttributeValue(nameof(GameObject.HealthBoxHeightOffset), "10.0"), &objT->HealthBoxHeightOffset, state);
        Marshal(node.GetAttributeValue(nameof(GameObject.LiveCameraFXPitch), "0"), &objT->LiveCameraFXPitch, state);
        Marshal(node.GetAttributeValue(nameof(GameObject.FormationWidth), "1"), &objT->FormationWidth, state);
        Marshal(node.GetAttributeValue(nameof(GameObject.FormationDepth), "1"), &objT->FormationDepth, state);
        Marshal(node.GetAttributeValue(nameof(GameObject.InstanceScaleFuzziness), null), &objT->InstanceScaleFuzziness, state);
        Marshal(node.GetAttributeValue(nameof(GameObject.StructureRubbleHeight), "0"), &objT->StructureRubbleHeight, state);
        Marshal(node.GetAttributeValue(nameof(GameObject.MaxSimultaneousOfType), null), &objT->MaxSimultaneousOfType, state);
        Marshal(node.GetAttributeValue(nameof(GameObject.RamPower), "0"), &objT->RamPower, state);
        Marshal(node.GetAttributeValue(nameof(GameObject.RamZMult), "1.0"), &objT->RamZMult, state);
        Marshal(node.GetAttributeValue(nameof(GameObject.ShockwaveResistance), "0"), &objT->ShockwaveResistance, state);
        Marshal(node.GetAttributeValue(nameof(GameObject.CommandPoints), "0"), &objT->CommandPoints, state);
        Marshal(node.GetAttributeValue(nameof(GameObject.CommandPointBonus), "0"), &objT->CommandPointBonus, state);
        // Reborn: official RA3/EP1 stores this default as 150 raw logic frames, not zero or 150 seconds.
        Marshal(node.GetAttributeValue(nameof(GameObject.VoiceAttackChargeTimeout), "150"), &objT->VoiceAttackChargeTimeout, state);
        // Reborn: these four official Time defaults are seconds, unlike the adjacent raw-frame unsigned timeouts; explicit inherited/user attributes still override them.
        Marshal(node.GetAttributeValue(nameof(GameObject.VoiceMoveLandToWaterTimeout), "5s"), &objT->VoiceMoveLandToWaterTimeout, state);
        Marshal(node.GetAttributeValue(nameof(GameObject.VoiceMoveWaterToLandTimeout), "5s"), &objT->VoiceMoveWaterToLandTimeout, state);
        Marshal(node.GetAttributeValue(nameof(GameObject.VoiceSelectUnderFireTimeout), "20s"), &objT->VoiceSelectUnderFireTimeout, state);
        Marshal(node.GetAttributeValue(nameof(GameObject.VoiceSelectUnderFireDamageTime), "3s"), &objT->VoiceSelectUnderFireDamageTime, state);
        // Reborn: original-root stock equality exposes omitted official defaults; the engaged timeout is also raw logic frames, not Time seconds.
        Marshal(node.GetAttributeValue(nameof(GameObject.MaxDistanceForEngaged), "150"), &objT->MaxDistanceForEngaged, state);
        Marshal(node.GetAttributeValue(nameof(GameObject.EngagedStateTimeout), "30"), &objT->EngagedStateTimeout, state);
        Marshal(node.GetAttributeValue(nameof(GameObject.ThreatLevel), "1.0"), &objT->ThreatLevel, state);
        Marshal(node.GetAttributeValue(nameof(GameObject.ForceLuaRegistration), "false"), &objT->ForceLuaRegistration, state);
        Marshal(node.GetAttributeValue(nameof(GameObject.KeepSelectableWhenDead), "false"), &objT->KeepSelectableWhenDead, state);
        Marshal(node.GetAttributeValue(nameof(GameObject.IsAutoBuilt), "false"), &objT->IsAutoBuilt, state);
        Marshal(node.GetAttributeValue(nameof(GameObject.CanPathThroughGates), "true"), &objT->CanPathThroughGates, state);
        Marshal(node.GetAttributeValue(nameof(GameObject.SlopeLimitIndex), "0"), &objT->SlopeLimitIndex, state);
        Marshal(node.GetAttributeValue(nameof(GameObject.PathfindDiameter), "-1"), &objT->PathfindDiameter, state);
        Marshal(node.GetAttributeValue(nameof(GameObject.SupplyOverride), "0"), &objT->SupplyOverride, state);
        Marshal(node.GetAttributeValue(nameof(GameObject.ExperienceScalarTable), null), &objT->ExperienceScalarTable, state);
        Marshal(node.GetAttributeValue(nameof(GameObject.FiringArc), "360"), &objT->FiringArc, state);
        // Reborn: retain official RA3/EP1 detection/path defaults instead of the legacy zero fallbacks.
        Marshal(node.GetAttributeValue(nameof(GameObject.CamouflageDetectorLevel), "1"), &objT->CamouflageDetectorLevel, state);
        Marshal(node.GetAttributeValue(nameof(GameObject.SelectionPriority), "2147483647"), &objT->SelectionPriority, state);
        Marshal(node.GetAttributeValue(nameof(GameObject.PathPriority), "25"), &objT->PathPriority, state);
        Marshal(node.GetAttributeValue(nameof(GameObject.ProductionQueueType), "INVALID"), &objT->ProductionQueueType, state);
        Marshal(node.GetAttributeValue(nameof(GameObject.BuildPlacementTypeFlag), "INVALID"), &objT->BuildPlacementTypeFlag, state);
        Marshal(node.GetAttributeValue(nameof(GameObject.BuildInProximityToSamePlayerStucture), "true"), &objT->BuildInProximityToSamePlayerStucture, state);
        Marshal(node.GetAttributeValue(nameof(GameObject.BuildOnRequiredObjectKindOf), null), &objT->BuildOnRequiredObjectKindOf, state);
        Marshal(node.GetAttributeValue(nameof(GameObject.UnitCategory), "NONE"), &objT->UnitCategory, state);
        Marshal(node.GetAttributeValue(nameof(GameObject.WeaponCategory), "NONE"), &objT->WeaponCategory, state);
        Marshal(node.GetAttributeValue(nameof(GameObject.HasFiredRecentlyTime), "2s"), &objT->HasFiredRecentlyTime, state);
        Marshal(node.GetAttributeValue(nameof(GameObject.UnitTypeIcon), null), &objT->UnitTypeIcon, state);
        // Reborn: original stock root values match official 0.5 seconds and 0.4/0.9 opacity defaults, not the older 4/0/1 profile.
        Marshal(node.GetAttributeValue(nameof(GameObject.ReinvisibilityDelay), "0.5s"), &objT->ReinvisibilityDelay, state);
        Marshal(node.GetAttributeValue(nameof(GameObject.InvisibilityOpacityMin), "0.4"), &objT->InvisibilityOpacityMin, state);
        Marshal(node.GetAttributeValue(nameof(GameObject.InvisibilityOpacityMax), "0.9"), &objT->InvisibilityOpacityMax, state);
        Marshal(node.GetAttributeValue(nameof(GameObject.HealthBar), null), &objT->HealthBar, state);
        // Reborn: these schema defaults remain overrideable by concrete/inherited values such as Conscript's 505 and 600.
        Marshal(node.GetAttributeValue(nameof(GameObject.SubGroupPriority), "500"), &objT->SubGroupPriority, state);
        Marshal(node.GetAttributeValue(nameof(GameObject.EvaEventSecondDamageFarFromFirstScanRange), "200"), &objT->EvaEventSecondDamageFarFromFirstScanRange, state);
        // Reborn: official EP1 defaults to 31000 milliseconds, stored as 155 logic frames; preserve the existing Duration conversion rather than copying raw milliseconds.
        Marshal(node.GetAttributeValue(nameof(GameObject.EvaEventSecondDamageFarFromFirstTimeoutMS), "31000"), &objT->EvaEventSecondDamageFarFromFirstTimeoutMS, state);
        Marshal(node.GetAttributeValue(nameof(GameObject.UnitIntro), null), &objT->UnitIntro, state);
        Marshal(node.GetChildNode(nameof(GameObject.DisplayName), null), &objT->DisplayName, state);
        Marshal(node.GetChildNode(nameof(GameObject.DisplayNameTransformed), null), &objT->DisplayNameTransformed, state);
        Marshal(node.GetChildNode(nameof(GameObject.GameDependency), null), &objT->GameDependency, state);
        Marshal(node.GetChildNode(nameof(GameObject.ObjectResourceInfo), null), &objT->ObjectResourceInfo, state);
        Marshal(node.GetChildNodes(nameof(GameObject.ArmorSet)), &objT->ArmorSet, state);
        Marshal(node.GetChildNodes(nameof(GameObject.LocomotorSet)), &objT->LocomotorSet, state);
        Marshal(node.GetChildNode(nameof(GameObject.Buildable), null), &objT->Buildable, state);
        Marshal(node.GetChildNode(nameof(GameObject.ThingClass), null), &objT->ThingClass, state);
        Marshal(node.GetChildNode(nameof(GameObject.BuildFadeInOnCreateList), null), &objT->BuildFadeInOnCreateList, state);
        Marshal(node.GetChildNode(nameof(GameObject.BuildVariations), null), &objT->BuildVariations, state);
        Marshal(node.GetChildNodes(nameof(GameObject.EquivalentTo)), &objT->EquivalentTo, state);
        Marshal(node.GetChildNode(nameof(GameObject.DisplayColor), null), &objT->DisplayColor, state);
        Marshal(node.GetChildNode(nameof(GameObject.Flammability), null), &objT->Flammability, state);
        Marshal(node.GetChildNode(nameof(GameObject.SkirmishAIInformation), null), &objT->SkirmishAIInformation, state);
        Marshal(node.GetChildNode(nameof(GameObject.Draws), null), &objT->Draws, state);
        Marshal(node.GetChildNode(nameof(GameObject.Behaviors), null), &objT->Behaviors, state);
        // Reborn: preserve EP1's by-value AI list and refuse more than the official two modules instead of silently losing multiple children.
        var ai = node.GetChildNode(nameof(GameObject.AI), null);
        if (ai?.GetChildNodes(null)?.GetCount() > 2)
            throw new BinaryAssetBuilderException(ErrorCode.InternalError, "EP1 GameObject AI admits at most two modules.");
        Marshal(ai, &objT->AI, state);
        MarshalSinglePolymorphic(node.GetChildNode(nameof(GameObject.Body), null), &objT->Body, state);
        Marshal(node.GetChildNode(nameof(GameObject.ClientUpdates), null), &objT->ClientUpdates, state);
        Marshal(node.GetChildNode(nameof(GameObject.ClientBehaviors), null), &objT->ClientBehaviors, state);
        Marshal(node.GetChildNode(nameof(GameObject.Geometry), null), &objT->Geometry, state);
        Marshal(node.GetChildNode(nameof(GameObject.ReplaceModule), null), &objT->ReplaceModule, state);
        Marshal(node.GetChildNode(nameof(GameObject.InheritableModule), null), &objT->InheritableModule, state);
        Marshal(node.GetChildNode(nameof(GameObject.AutoResolveArmor), null), &objT->AutoResolveArmor, state);
        Marshal(node.GetChildNode(nameof(GameObject.AutoResolveWeapon), null), &objT->AutoResolveWeapon, state);
        Marshal(node.GetChildNode(nameof(GameObject.FormationPreviewDecal), null), &objT->FormationPreviewDecal, state);
        Marshal(node.GetChildNode(nameof(GameObject.FormationPreviewItemDecal), null), &objT->FormationPreviewItemDecal, state);
        Marshal(node.GetChildNode(nameof(GameObject.LiveCameraOffset), null), &objT->LiveCameraOffset, state);
        Marshal(node.GetChildNode(nameof(GameObject.AudioArrayVoice), null), &objT->AudioArrayVoice, state);
        Marshal(node.GetChildNode(nameof(GameObject.AudioArraySound), null), &objT->AudioArraySound, state);
        Marshal(node.GetChildNode(nameof(GameObject.EvaEventArray), null), &objT->EvaEventArray, state);
        Marshal(node.GetChildNodes(nameof(GameObject.UpgradeCameo)), &objT->UpgradeCameo, state);
        Marshal(node.GetChildNode(nameof(GameObject.ShadowInfo), null), &objT->ShadowInfo, state);
        Marshal(node.GetChildNode(nameof(GameObject.AutoResolveInfo), null), &objT->AutoResolveInfo, state);
        Marshal(node.GetChildNode(nameof(GameObject.VisionInfo), null), &objT->VisionInfo, state);
        Marshal(node.GetChildNode(nameof(GameObject.CrusherInfo), null), &objT->CrusherInfo, state);
        Marshal(node.GetChildNodes(nameof(GameObject.ProjectedBuildabilityInfo)), &objT->ProjectedBuildabilityInfo, state);
        Marshal(node.GetChildNodes(nameof(GameObject.DisplayUpgrade)), &objT->DisplayUpgrade, state);
        Marshal(node, (BaseInheritableAsset*)objT, state);
    }
}
