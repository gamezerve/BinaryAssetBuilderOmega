using System.Runtime.InteropServices;

namespace BinaryAssetBuilder.ManifestInspector;

internal static class UprisingLayoutSmokeTest
{
    public static void Run()
    {
        TestPatchManifestTotals();
        // Reborn: lock recovered RA3 containment offsets after EP1's two inline status-mask expansions.
        ExpectSize<SageBinaryData.OpenContainModuleData>(156);
        // Reborn: transport contains three widened KindOf masks; garrison keeps its eight-byte roster inline.
        ExpectSize<SageBinaryData.TransportContainModuleData>(352);
        ExpectOffset<SageBinaryData.TransportContainModuleData>(nameof(SageBinaryData.TransportContainModuleData.ExitBone), 156);
        ExpectOffset<SageBinaryData.TransportContainModuleData>(nameof(SageBinaryData.TransportContainModuleData.ConditionForEntry), 292);
        ExpectOffset<SageBinaryData.TransportContainModuleData>(nameof(SageBinaryData.TransportContainModuleData.InitialPayload), 316);
        ExpectOffset<SageBinaryData.TransportContainModuleData>(nameof(SageBinaryData.TransportContainModuleData.ScatterNearbyOnExit), 340);
        ExpectSize<SageBinaryData.HordeTransportContainModuleData>(356);
        ExpectOffset<SageBinaryData.HordeTransportContainModuleData>(nameof(SageBinaryData.HordeTransportContainModuleData.FlyOffMapOnEmpty), 352);
        ExpectSize<SageBinaryData.GarrisonContainModuleData>(168);
        ExpectOffset<SageBinaryData.GarrisonContainModuleData>(nameof(SageBinaryData.GarrisonContainModuleData.InitialRoster), 156);
        ExpectOffset<SageBinaryData.GarrisonContainModuleData>(nameof(SageBinaryData.GarrisonContainModuleData.MobileGarrison), 164);
        ExpectOffset<SageBinaryData.OpenContainModuleData>(nameof(SageBinaryData.OpenContainModuleData.IgnoreDisabledBitsForRiders), 36);
        ExpectOffset<SageBinaryData.OpenContainModuleData>(nameof(SageBinaryData.OpenContainModuleData.ObjectStatusWhileContaining), 72);
        ExpectOffset<SageBinaryData.OpenContainModuleData>(nameof(SageBinaryData.OpenContainModuleData.PassengerData), 120);
        ExpectOffset<SageBinaryData.OpenContainModuleData>(nameof(SageBinaryData.OpenContainModuleData.PassDisabilityToRiders), 149);
        ExpectSize<SageBinaryData.PassengerDataType>(136);
        ExpectOffset<SageBinaryData.PassengerDataType>(nameof(SageBinaryData.PassengerDataType.Filter), 12);
        ExpectOffset<SageBinaryData.PassengerDataType>(nameof(SageBinaryData.PassengerDataType.SlingUnderBone), 132);
        ExpectSize<SageBinaryData.ArmorTemplate>(32);
        ExpectSize<SageBinaryData.ArmorSetBitFlags>(4);
        Expect(SageBinaryData.ArmorSetBitFlags.Count == 23, "ArmorSetBitFlags.Count", 23, SageBinaryData.ArmorSetBitFlags.Count);

        ExpectSize<SageBinaryData.AttributeModifier>(56);
        ExpectOffset<SageBinaryData.AttributeModifier>(nameof(SageBinaryData.AttributeModifier.Category), 4);
        ExpectOffset<SageBinaryData.AttributeModifier>(nameof(SageBinaryData.AttributeModifier.Duration), 8);
        ExpectOffset<SageBinaryData.AttributeModifier>(nameof(SageBinaryData.AttributeModifier.StartFX), 12);
        ExpectOffset<SageBinaryData.AttributeModifier>(nameof(SageBinaryData.AttributeModifier.ModelConditionsSet), 20);
        ExpectOffset<SageBinaryData.AttributeModifier>(nameof(SageBinaryData.AttributeModifier.ObjectStatusToSet), 28);
        ExpectOffset<SageBinaryData.AttributeModifier>(nameof(SageBinaryData.AttributeModifier.ArmorSetType), 36);
        ExpectOffset<SageBinaryData.AttributeModifier>(nameof(SageBinaryData.AttributeModifier.Shader), 40);
        ExpectOffset<SageBinaryData.AttributeModifier>(nameof(SageBinaryData.AttributeModifier.Modifier), 44);
        ExpectOffset<SageBinaryData.AttributeModifier>(nameof(SageBinaryData.AttributeModifier.ReplaceInCategoryIfLongest), 52);

        Expect(SageBinaryData.ModelConditionBitFlags.Count == 463, "ModelConditionBitFlags.Count", 463, SageBinaryData.ModelConditionBitFlags.Count);
        Expect(SageBinaryData.ObjectStatusBitFlags.Count == 230, "ObjectStatusBitFlags.Count", 230, SageBinaryData.ObjectStatusBitFlags.Count);
        // Reborn: lock the RA3 DieMux pointer ABI that Uprising extends with a wider pointed-to mask.
        ExpectSize<SageBinaryData.DieMuxDataType>(44);
        ExpectOffset<SageBinaryData.DieMuxDataType>(nameof(SageBinaryData.DieMuxDataType.ExemptStatus), 8);
        ExpectOffset<SageBinaryData.DieMuxDataType>(nameof(SageBinaryData.DieMuxDataType.RequiredStatus), 12);
        ExpectOffset<SageBinaryData.DieMuxDataType>(nameof(SageBinaryData.DieMuxDataType.DamageAmountRequired), 16);
        ExpectOffset<SageBinaryData.DieMuxDataType>(nameof(SageBinaryData.DieMuxDataType.DeathTypesForbidden), 36);
        ExpectSize<SageBinaryData.DieModuleData>(52);
        // Reborn: lock the dependency pointer table recovered from the official RA3 marshaler IL.
        ExpectSize<SageBinaryData.GameDependencyType>(40);
        ExpectOffset<SageBinaryData.GameDependencyType>(nameof(SageBinaryData.GameDependencyType.RequiredModelConditionsAny), 0);
        ExpectOffset<SageBinaryData.GameDependencyType>(nameof(SageBinaryData.GameDependencyType.ForbiddenModelConditions), 4);
        ExpectOffset<SageBinaryData.GameDependencyType>(nameof(SageBinaryData.GameDependencyType.RequiredObjectStatusAny), 8);
        ExpectOffset<SageBinaryData.GameDependencyType>(nameof(SageBinaryData.GameDependencyType.RequiredObject), 12);
        ExpectOffset<SageBinaryData.GameDependencyType>(nameof(SageBinaryData.GameDependencyType.ObjectFilter), 36);
        // Reborn: lock the SlowDeath pointer offsets recovered from EA's official RA3 marshaler IL.
        ExpectSize<SageBinaryData.SlowDeathBehaviorModuleData>(116);
        ExpectOffset<SageBinaryData.SlowDeathBehaviorModuleData>(nameof(SageBinaryData.SlowDeathBehaviorModuleData.DeathFlags), 56);
        ExpectOffset<SageBinaryData.SlowDeathBehaviorModuleData>(nameof(SageBinaryData.SlowDeathBehaviorModuleData.FadeTime), 60);
        ExpectOffset<SageBinaryData.SlowDeathBehaviorModuleData>(nameof(SageBinaryData.SlowDeathBehaviorModuleData.DeathTypes), 68);
        ExpectOffset<SageBinaryData.SlowDeathBehaviorModuleData>(nameof(SageBinaryData.SlowDeathBehaviorModuleData.DeathObjectStatusBits), 72);
        ExpectOffset<SageBinaryData.SlowDeathBehaviorModuleData>(nameof(SageBinaryData.SlowDeathBehaviorModuleData.DieMuxData), 108);
        ExpectOffset<SageBinaryData.SlowDeathBehaviorModuleData>(nameof(SageBinaryData.SlowDeathBehaviorModuleData.ShadowWhenDead), 112);
        // Reborn: account for EP1's eight-byte ObjectFilter growth on top of official RA3 invisibility layouts.
        ExpectSize<SageBinaryData.InvisibilityUpdateModuleData>(148);
        ExpectOffset<SageBinaryData.InvisibilityUpdateModuleData>(nameof(SageBinaryData.InvisibilityUpdateModuleData.InvisibilityTemplate), 8);
        ExpectOffset<SageBinaryData.InvisibilityUpdateModuleData>(nameof(SageBinaryData.InvisibilityUpdateModuleData.RequiredNearbyObjectRange), 16);
        ExpectOffset<SageBinaryData.InvisibilityUpdateModuleData>(nameof(SageBinaryData.InvisibilityUpdateModuleData.RequiresNearbyObjectFilter), 28);
        ExpectSize<SageBinaryData.InvisibilitySpecialPowerModuleData>(496);
        ExpectOffset<SageBinaryData.InvisibilitySpecialPowerModuleData>(nameof(SageBinaryData.InvisibilitySpecialPowerModuleData.InvisibilityTemplate), 476);
        ExpectOffset<SageBinaryData.InvisibilitySpecialPowerModuleData>(nameof(SageBinaryData.InvisibilitySpecialPowerModuleData.ObjectFilter), 488);
        ExpectOffset<SageBinaryData.InvisibilitySpecialPowerModuleData>(nameof(SageBinaryData.InvisibilitySpecialPowerModuleData.Permanent), 492);
        Expect(SageBinaryData.DamageBitFlags.Count == 39, "DamageBitFlags.Count", 39, SageBinaryData.DamageBitFlags.Count);
        Expect(SageBinaryData.DisabledBitFlags.Count == 13, "DisabledBitFlags.Count", 13, SageBinaryData.DisabledBitFlags.Count);
        Expect((int)SageBinaryData.ArmorSetType.INVALID == 0, "ArmorSetType.INVALID", 0, (int)SageBinaryData.ArmorSetType.INVALID);
        Expect((int)SageBinaryData.ArmorSetType.SHIELDBODY_ENABLED == 22, "ArmorSetType.SHIELDBODY_ENABLED", 22, (int)SageBinaryData.ArmorSetType.SHIELDBODY_ENABLED);
        Expect((int)SageBinaryData.AttributeModifierCategoryType.SHRINK == 15, "AttributeModifierCategoryType.SHRINK", 15, (int)SageBinaryData.AttributeModifierCategoryType.SHRINK);
        Expect((int)SageBinaryData.AttributeType.RADIATION_ARMOR == 36, "AttributeType.RADIATION_ARMOR", 36, (int)SageBinaryData.AttributeType.RADIATION_ARMOR);

        ExpectSize<SageBinaryData.LocomotorTemplate>(396);
        ExpectOffset<SageBinaryData.LocomotorTemplate>(nameof(SageBinaryData.LocomotorTemplate.PreferredHeightPitchingEpsilon), 84);
        ExpectOffset<SageBinaryData.LocomotorTemplate>(nameof(SageBinaryData.LocomotorTemplate.ActiveModelConditions), 108);
        ExpectOffset<SageBinaryData.LocomotorTemplate>(nameof(SageBinaryData.LocomotorTemplate.ReverseMoveSpeed), 208);
        ExpectOffset<SageBinaryData.LocomotorTemplate>(nameof(SageBinaryData.LocomotorTemplate.SpeedBasedHeightOffset), 340);
        ExpectOffset<SageBinaryData.LocomotorTemplate>(nameof(SageBinaryData.LocomotorTemplate.JetLocomotorData), 364);
        ExpectOffset<SageBinaryData.LocomotorTemplate>(nameof(SageBinaryData.LocomotorTemplate.IgnoreLowSpeedAngleMultiplier), 393);
        Expect(SageBinaryData.LocomotorSurfaceBitFlags.Count == 11, "LocomotorSurfaceBitFlags.Count", 11, SageBinaryData.LocomotorSurfaceBitFlags.Count);

        ExpectSize<SageBinaryData.WeaponAiHintInfo>(12);
        // Reborn: enforce official tint offsets recovered from RA3 Tokenizer marshaler IL.
        ExpectSize<SageBinaryData.TintObjectsNuggetType>(72);
        ExpectOffset<SageBinaryData.TintObjectsNuggetType>(nameof(SageBinaryData.TintObjectsNuggetType.PreColorTime), 40);
        ExpectOffset<SageBinaryData.TintObjectsNuggetType>(nameof(SageBinaryData.TintObjectsNuggetType.Color), 60);
        ExpectOffset<SageBinaryData.WeaponAiHintInfo>(nameof(SageBinaryData.WeaponAiHintInfo.UseAsWarheadForDamageCalculations), 4);
        ExpectOffset<SageBinaryData.WeaponAiHintInfo>(nameof(SageBinaryData.WeaponAiHintInfo.IsAntiGarrisonWeapon), 8);
        ExpectSize<SageBinaryData.WeaponTemplate>(340);
        ExpectOffset<SageBinaryData.WeaponTemplate>(nameof(SageBinaryData.WeaponTemplate.AttackRange), 4);
        ExpectOffset<SageBinaryData.WeaponTemplate>(nameof(SageBinaryData.WeaponTemplate.PreferredTargetBone), 76);
        ExpectOffset<SageBinaryData.WeaponTemplate>(nameof(SageBinaryData.WeaponTemplate.ImpactLoopSound), 96);
        ExpectOffset<SageBinaryData.WeaponTemplate>(nameof(SageBinaryData.WeaponTemplate.RequiredFiringObjectStatus), 144);
        ExpectOffset<SageBinaryData.WeaponTemplate>(nameof(SageBinaryData.WeaponTemplate.VirtualDamage), 228);
        ExpectOffset<SageBinaryData.WeaponTemplate>(nameof(SageBinaryData.WeaponTemplate.WeaponAiHintInfo), 260);
        ExpectOffset<SageBinaryData.WeaponTemplate>(nameof(SageBinaryData.WeaponTemplate.IncompatibleAttributeModifier), 292);
        ExpectOffset<SageBinaryData.WeaponTemplate>(nameof(SageBinaryData.WeaponTemplate.ScatterIndependently), 300);
        ExpectOffset<SageBinaryData.WeaponTemplate>(nameof(SageBinaryData.WeaponTemplate.ProjectileSelfUsesPathfinder), 312);
        ExpectOffset<SageBinaryData.WeaponTemplate>(nameof(SageBinaryData.WeaponTemplate.UpdateBarrelModelConditions), 338);
        Expect(SageBinaryData.WeaponFlagsBitFlags.Count == 13, "WeaponFlagsBitFlags.Count", 13, SageBinaryData.WeaponFlagsBitFlags.Count);
        Expect(SageBinaryData.WeaponAntiBitFlags.Count == 14, "WeaponAntiBitFlags.Count", 14, SageBinaryData.WeaponAntiBitFlags.Count);

        ExpectSize<SageBinaryData.MoneyTransaction>(8);
        ExpectSize<SageBinaryData.ObjectResourceInfo>(8);
        Expect(SageBinaryData.KindOfBitFlags.Count == 291, "KindOfBitFlags.Count", 291, SageBinaryData.KindOfBitFlags.Count);
        Expect(SageBinaryData.BuildPlacementTypeBitFlags.Count == 5, "BuildPlacementTypeBitFlags.Count", 5, SageBinaryData.BuildPlacementTypeBitFlags.Count);
        ExpectSize<SageBinaryData.GameObject>(600);
        ExpectSize<SageBinaryData.ObjectFilter>(120);
        ExpectOffset<SageBinaryData.ObjectFilter>(nameof(SageBinaryData.ObjectFilter.StatusBitFlags), 96);
        ExpectOffset<SageBinaryData.ObjectFilter>(nameof(SageBinaryData.ObjectFilter.StatusBitFlagsExclude), 100);
        ExpectOffset<SageBinaryData.ObjectFilter>(nameof(SageBinaryData.ObjectFilter.IncludeThing), 104);
        ExpectOffset<SageBinaryData.GameObject>(nameof(SageBinaryData.GameObject.KindOf), 4);
        ExpectOffset<SageBinaryData.GameObject>(nameof(SageBinaryData.GameObject.Description), 52);
        ExpectOffset<SageBinaryData.GameObject>(nameof(SageBinaryData.GameObject.RadarPriority), 84);
        ExpectOffset<SageBinaryData.GameObject>(nameof(SageBinaryData.GameObject.Side), 120);
        ExpectOffset<SageBinaryData.GameObject>(nameof(SageBinaryData.GameObject.SelectPortrait), 156);
        ExpectOffset<SageBinaryData.GameObject>(nameof(SageBinaryData.GameObject.VoiceMoveLandToWaterTimeout), 244);
        ExpectOffset<SageBinaryData.GameObject>(nameof(SageBinaryData.GameObject.ExperienceScalarTable), 284);
        ExpectOffset<SageBinaryData.GameObject>(nameof(SageBinaryData.GameObject.BuildOnRequiredObjectKindOf), 316);
        ExpectOffset<SageBinaryData.GameObject>(nameof(SageBinaryData.GameObject.InvisibilityOpacityMin), 376);
        ExpectOffset<SageBinaryData.GameObject>(nameof(SageBinaryData.GameObject.UnitIntro), 400);
        ExpectOffset<SageBinaryData.GameObject>(nameof(SageBinaryData.GameObject.ObjectResourceInfo), 416);
        ExpectOffset<SageBinaryData.GameObject>(nameof(SageBinaryData.GameObject.UnitSpecificFX), 484);
        ExpectOffset<SageBinaryData.GameObject>(nameof(SageBinaryData.GameObject.UpgradeCameo), 544);
        ExpectOffset<SageBinaryData.GameObject>(nameof(SageBinaryData.GameObject.DisplayUpgrade), 576);
        ExpectOffset<SageBinaryData.GameObject>(nameof(SageBinaryData.GameObject.RefundValue), 584);
        ExpectOffset<SageBinaryData.GameObject>(nameof(SageBinaryData.GameObject.BuildInProximityToSamePlayerStucture), 598);
        ExpectSize<SageBinaryData.CrusherLevelModelConditionInfo>(12);
        ExpectOffset<SageBinaryData.CrusherLevelModelConditionInfo>(nameof(SageBinaryData.CrusherLevelModelConditionInfo.ObjectFilter), 4);
        ExpectSize<SageBinaryData.CrushKillDelayForObjectFilter>(12);
        ExpectSize<SageBinaryData.CrusherInfo>(52);
        ExpectOffset<SageBinaryData.CrusherInfo>(nameof(SageBinaryData.CrusherInfo.ExtraCrushLevels), 20);
        ExpectOffset<SageBinaryData.CrusherInfo>(nameof(SageBinaryData.CrusherInfo.ExtraCrushKillDelays), 28);
        ExpectOffset<SageBinaryData.CrusherInfo>(nameof(SageBinaryData.CrusherInfo.DefaultCrushKillDelay), 36);
        ExpectOffset<SageBinaryData.CrusherInfo>(nameof(SageBinaryData.CrusherInfo.CannotCrushTarget), 50);
        ExpectSize<SageBinaryData.ProjectedBuildabilityInfo>(116);
        ExpectOffset<SageBinaryData.ProjectedBuildabilityInfo>(nameof(SageBinaryData.ProjectedBuildabilityInfo.ModelConditionsToReject), 44);
        ExpectOffset<SageBinaryData.ProjectedBuildabilityInfo>(nameof(SageBinaryData.ProjectedBuildabilityInfo.AllowedObjectFilter), 108);
        ExpectOffset<SageBinaryData.ProjectedBuildabilityInfo>(nameof(SageBinaryData.ProjectedBuildabilityInfo.PrimaryBuidability), 112);
        ExpectSize<SageBinaryData.SlavedUpdateModuleData>(112);
        ExpectSize<SageBinaryData.SpawnedSlaveUpdateModuleData>(112);
        ExpectOffset<SageBinaryData.SpawnedSlaveUpdateModuleData>(nameof(SageBinaryData.SpawnedSlaveUpdateModuleData.Base), 0);
        ExpectSize<SageBinaryData.GenericUnpackUpdateModuleData>(20);
        ExpectOffset<SageBinaryData.GenericUnpackUpdateModuleData>(nameof(SageBinaryData.GenericUnpackUpdateModuleData.UnpackTime), 8);
        ExpectOffset<SageBinaryData.GenericUnpackUpdateModuleData>(nameof(SageBinaryData.GenericUnpackUpdateModuleData.UnpackCompleteSound), 12);
        ExpectOffset<SageBinaryData.GenericUnpackUpdateModuleData>(nameof(SageBinaryData.GenericUnpackUpdateModuleData.OffsetHeightAboveWater), 16);
        ExpectSize<SageBinaryData.UnitUnpackUpdateModuleData>(20);
        ExpectSize<SageBinaryData.SpecialPowerModuleData>(476);
        ExpectOffset<SageBinaryData.SpecialPowerModuleData>(nameof(SageBinaryData.SpecialPowerModuleData.AttributeModifierAffects), 96);
        ExpectOffset<SageBinaryData.SpecialPowerModuleData>(nameof(SageBinaryData.SpecialPowerModuleData.InitiateSound), 456);
        ExpectOffset<SageBinaryData.SpecialPowerModuleData>(nameof(SageBinaryData.SpecialPowerModuleData.AvailableAtStart), 469);
        ExpectSize<SageBinaryData.StoreObjectsSpecialPowerModuleData>(492);
        ExpectOffset<SageBinaryData.StoreObjectsSpecialPowerModuleData>(nameof(SageBinaryData.StoreObjectsSpecialPowerModuleData.TeleportLinkID), 480);
        ExpectOffset<SageBinaryData.StoreObjectsSpecialPowerModuleData>(nameof(SageBinaryData.StoreObjectsSpecialPowerModuleData.TargetMarkerObjectRef), 488);
        ExpectSize<SageBinaryData.AddObjectsToLiftUpdateSpecialPowerModuleData>(496);
        ExpectOffset<SageBinaryData.AddObjectsToLiftUpdateSpecialPowerModuleData>(nameof(SageBinaryData.AddObjectsToLiftUpdateSpecialPowerModuleData.LiftObjectLinkID), 492);
        ExpectSize<SageBinaryData.AddObjectsToLureUpdateSpecialPowerModuleData>(496);
        ExpectOffset<SageBinaryData.AddObjectsToLureUpdateSpecialPowerModuleData>(nameof(SageBinaryData.AddObjectsToLureUpdateSpecialPowerModuleData.LureObjectLinkID), 492);
        ExpectSize<SageBinaryData.LureObjectsUpdateModuleData>(72);
        ExpectOffset<SageBinaryData.LureObjectsUpdateModuleData>(nameof(SageBinaryData.LureObjectsUpdateModuleData.GuardStatus), 20);
        ExpectOffset<SageBinaryData.LureObjectsUpdateModuleData>(nameof(SageBinaryData.LureObjectsUpdateModuleData.GuardOffset), 64);
        ExpectSize<SageBinaryData.FlingStoredObjectsObjectMap>(8);
        ExpectSize<SageBinaryData.FlingStoredObjectsSpecialPowerModuleData>(500);
        ExpectOffset<SageBinaryData.FlingStoredObjectsSpecialPowerModuleData>(nameof(SageBinaryData.FlingStoredObjectsSpecialPowerModuleData.StoreObjectsLinkID), 476);
        ExpectOffset<SageBinaryData.FlingStoredObjectsSpecialPowerModuleData>(nameof(SageBinaryData.FlingStoredObjectsSpecialPowerModuleData.ObjectMap), 492);
        ExpectSize<SageBinaryData.LiftedUnitModelStateMap>(64);
        ExpectOffset<SageBinaryData.LiftedUnitModelStateMap>(nameof(SageBinaryData.LiftedUnitModelStateMap.ObjectFilter), 60);
        ExpectSize<SageBinaryData.LiftedObjectModelStateMapList>(8);
        ExpectSize<SageBinaryData.LiftObjectUpdateModuleData>(76);
        ExpectOffset<SageBinaryData.LiftObjectUpdateModuleData>(nameof(SageBinaryData.LiftObjectUpdateModuleData.LiftObjectLinkID), 8);
        ExpectOffset<SageBinaryData.LiftObjectUpdateModuleData>(nameof(SageBinaryData.LiftObjectUpdateModuleData.RotationSpeed), 40);
        ExpectOffset<SageBinaryData.LiftObjectUpdateModuleData>(nameof(SageBinaryData.LiftObjectUpdateModuleData.DisabledTypesToProcess), 60);
        ExpectOffset<SageBinaryData.LiftObjectUpdateModuleData>(nameof(SageBinaryData.LiftObjectUpdateModuleData.ModelStateObjectFilters), 64);
        ExpectOffset<SageBinaryData.LiftObjectUpdateModuleData>(nameof(SageBinaryData.LiftObjectUpdateModuleData.CrusherModifiesVelocity), 72);
        Expect(SageBinaryData.SpecialAbilityUpdateOptionsTypeBitFlags.Count == 32, "SpecialAbilityUpdateOptionsTypeBitFlags.Count", 32, SageBinaryData.SpecialAbilityUpdateOptionsTypeBitFlags.Count);
        ExpectSize<SageBinaryData.SpecialAbilityUpdateModuleData>(252);
        ExpectOffset<SageBinaryData.SpecialAbilityUpdateModuleData>(nameof(SageBinaryData.SpecialAbilityUpdateModuleData.SpecialPowerTemplate), 8);
        ExpectOffset<SageBinaryData.SpecialAbilityUpdateModuleData>(nameof(SageBinaryData.SpecialAbilityUpdateModuleData.Options), 44);
        ExpectOffset<SageBinaryData.SpecialAbilityUpdateModuleData>(nameof(SageBinaryData.SpecialAbilityUpdateModuleData.SetObjectStatusOnTrigger), 140);
        ExpectOffset<SageBinaryData.SpecialAbilityUpdateModuleData>(nameof(SageBinaryData.SpecialAbilityUpdateModuleData.ClearObjectStatusOnExit), 172);
        ExpectOffset<SageBinaryData.SpecialAbilityUpdateModuleData>(nameof(SageBinaryData.SpecialAbilityUpdateModuleData.DisabledTypesToProcess), 228);
        ExpectOffset<SageBinaryData.SpecialAbilityUpdateModuleData>(nameof(SageBinaryData.SpecialAbilityUpdateModuleData.ActiveModelCondition), 236);
        ExpectOffset<SageBinaryData.SpecialAbilityUpdateModuleData>(nameof(SageBinaryData.SpecialAbilityUpdateModuleData.MinimumUnpackTimeAfterSpecialPowerInitiation), 240);
        ExpectOffset<SageBinaryData.SpecialAbilityUpdateModuleData>(nameof(SageBinaryData.SpecialAbilityUpdateModuleData.CustomAnimAndDuration), 244);
        ExpectOffset<SageBinaryData.SpecialAbilityUpdateModuleData>(nameof(SageBinaryData.SpecialAbilityUpdateModuleData.StartRechargeOnExit), 248);
        ExpectSize<SageBinaryData.ReplaceSelfSpecialAbilityModuleData>(272);
        ExpectOffset<SageBinaryData.ReplaceSelfSpecialAbilityModuleData>(nameof(SageBinaryData.ReplaceSelfSpecialAbilityModuleData.NewObjectUnpackTime), 252);
        ExpectOffset<SageBinaryData.ReplaceSelfSpecialAbilityModuleData>(nameof(SageBinaryData.ReplaceSelfSpecialAbilityModuleData.ReplaceOptions), 256);
        ExpectOffset<SageBinaryData.ReplaceSelfSpecialAbilityModuleData>(nameof(SageBinaryData.ReplaceSelfSpecialAbilityModuleData.ClearTriggerDistance), 260);
        ExpectOffset<SageBinaryData.ReplaceSelfSpecialAbilityModuleData>(nameof(SageBinaryData.ReplaceSelfSpecialAbilityModuleData.ReplacementTemplate), 264);
        ExpectSize<SageBinaryData.ProjectileReplaceSelfSpecialAbilityModuleData>(280);
        ExpectOffset<SageBinaryData.ProjectileReplaceSelfSpecialAbilityModuleData>(nameof(SageBinaryData.ProjectileReplaceSelfSpecialAbilityModuleData.LaunchingWeapon), 272);
        ExpectOffset<SageBinaryData.ProjectileReplaceSelfSpecialAbilityModuleData>(nameof(SageBinaryData.ProjectileReplaceSelfSpecialAbilityModuleData.OtherObjectCreationList), 276);
        // Reborn: Pin the Uprising ProjectilePath replacement-list ABI recovered from static.bin.
        ExpectSize<SageBinaryData.ProjectilePathNode>(48);
        ExpectOffset<SageBinaryData.ProjectilePathNode>(nameof(SageBinaryData.ProjectilePathNode.Point), 16);
        ExpectOffset<SageBinaryData.ProjectilePathNode>(nameof(SageBinaryData.ProjectilePathNode.OutVec), 32);
        ExpectSize<SageBinaryData.ProjectilePath>(16);
        ExpectOffset<SageBinaryData.ProjectilePath>(nameof(SageBinaryData.ProjectilePath.ComponentScale), 4);
        ExpectOffset<SageBinaryData.ProjectilePath>(nameof(SageBinaryData.ProjectilePath.Node), 8);
        // Reborn: Pin the EP1 Yuriko hot-key root and shared 12-byte entry layout recovered from global.bin.
        ExpectSize<SageBinaryData.YurikoHotKeys>(12);
        ExpectOffset<SageBinaryData.YurikoHotKeys>(nameof(SageBinaryData.YurikoHotKeys.Map), 4);
        ExpectSize<SageBinaryData.HotKeyDef>(12);
        // Reborn: Pin the EP1 physics-capacity and shell-personality roots recovered from global/static streams.
        ExpectSize<SageBinaryData.DynamicsSettings>(24);
        ExpectOffset<SageBinaryData.DynamicsSettings>(nameof(SageBinaryData.DynamicsSettings.MaximumObjects), 4);
        ExpectOffset<SageBinaryData.DynamicsSettings>(nameof(SageBinaryData.DynamicsSettings.CreateGlobalIsland), 20);
        ExpectSize<SageBinaryData.MainMenuPersonalityTemplate>(16);
        ExpectOffset<SageBinaryData.MainMenuPersonalityTemplate>(nameof(SageBinaryData.MainMenuPersonalityTemplate.MainMenuPersonalityImage), 4);
        ExpectOffset<SageBinaryData.MainMenuPersonalityTemplate>(nameof(SageBinaryData.MainMenuPersonalityTemplate.MainMenuPersonalityMusic), 8);
        ExpectSize<SageBinaryData.MainMenuPersonalityGroup>(16);
        ExpectOffset<SageBinaryData.MainMenuPersonalityGroup>(nameof(SageBinaryData.MainMenuPersonalityGroup.DefaultPersonality), 4);
        ExpectOffset<SageBinaryData.MainMenuPersonalityGroup>(nameof(SageBinaryData.MainMenuPersonalityGroup.MainMenuPersonality), 8);
        // Reborn: EP1's overridable audio roots are ABI-identical wrappers around their transient bases.
        ExpectSize<SageBinaryData.AudioEventOverridable>(120);
        ExpectOffset<SageBinaryData.AudioEventOverridable>(nameof(SageBinaryData.AudioEventOverridable.Base), 0);
        ExpectSize<SageBinaryData.MultisoundOverridable>(16);
        ExpectOffset<SageBinaryData.MultisoundOverridable>(nameof(SageBinaryData.MultisoundOverridable.Base), 0);
        // Reborn: Pin the EP1 movie-archive hierarchy and the three-list component root.
        ExpectSize<SageBinaryData.GeneralArchiveMovie>(36);
        ExpectOffset<SageBinaryData.GeneralArchiveMovie>(nameof(SageBinaryData.GeneralArchiveMovie.PreviewImage), 8);
        ExpectOffset<SageBinaryData.GeneralArchiveMovie>(nameof(SageBinaryData.GeneralArchiveMovie.Icon), 28);
        ExpectSize<SageBinaryData.ScenarioArchiveMovie>(40);
        ExpectOffset<SageBinaryData.ScenarioArchiveMovie>(nameof(SageBinaryData.ScenarioArchiveMovie.UnlockRequirement), 36);
        ExpectSize<SageBinaryData.CampaignArchiveMovie>(44);
        ExpectOffset<SageBinaryData.CampaignArchiveMovie>(nameof(SageBinaryData.CampaignArchiveMovie.Faction), 36);
        ExpectOffset<SageBinaryData.CampaignArchiveMovie>(nameof(SageBinaryData.CampaignArchiveMovie.ProgressLock), 40);
        ExpectSize<SageBinaryData.UIComponentMovieArchive>(32);
        ExpectOffset<SageBinaryData.UIComponentMovieArchive>(nameof(SageBinaryData.UIComponentMovieArchive.GeneralMovie), 8);
        ExpectOffset<SageBinaryData.UIComponentMovieArchive>(nameof(SageBinaryData.UIComponentMovieArchive.ScenarioMovie), 16);
        ExpectOffset<SageBinaryData.UIComponentMovieArchive>(nameof(SageBinaryData.UIComponentMovieArchive.CampaignMovie), 24);
        // Reborn: Pin the EP1 scenario-preview records and fieldless scenario component.
        ExpectSize<SageBinaryData.UIScenarioMapPreviewFactionSettings>(8);
        ExpectOffset<SageBinaryData.UIScenarioMapPreviewFactionSettings>(nameof(SageBinaryData.UIScenarioMapPreviewFactionSettings.PlayerImage), 4);
        ExpectSize<SageBinaryData.UIScenarioMapPreview>(12);
        ExpectOffset<SageBinaryData.UIScenarioMapPreview>(nameof(SageBinaryData.UIScenarioMapPreview.FactionSettings), 4);
        ExpectSize<SageBinaryData.UIComponentScenario>(8);
        ExpectOffset<SageBinaryData.UIComponentScenario>(nameof(SageBinaryData.UIComponentScenario.Base), 0);
        // Reborn: Pin the EP1 scenario-manager records recovered from the 15,856-byte static fixture.
        ExpectSize<SageBinaryData.ScenarioEnemy>(28);
        ExpectOffset<SageBinaryData.ScenarioEnemy>(nameof(SageBinaryData.ScenarioEnemy.Color), 24);
        ExpectSize<SageBinaryData.ScenarioTemplate>(96);
        ExpectOffset<SageBinaryData.ScenarioTemplate>(nameof(SageBinaryData.ScenarioTemplate.MapName), 36);
        ExpectOffset<SageBinaryData.ScenarioTemplate>(nameof(SageBinaryData.ScenarioTemplate.Enemy), 76);
        ExpectOffset<SageBinaryData.ScenarioTemplate>(nameof(SageBinaryData.ScenarioTemplate.ScenarioUnlock), 84);
        ExpectOffset<SageBinaryData.ScenarioTemplate>(nameof(SageBinaryData.ScenarioTemplate.IsStartingScenario), 92);
        ExpectSize<SageBinaryData.UnlockableUnit>(8);
        ExpectSize<SageBinaryData.ScenarioManagerData>(48);
        ExpectOffset<SageBinaryData.ScenarioManagerData>(nameof(SageBinaryData.ScenarioManagerData.MaxRedAlertDeposit), 28);
        ExpectOffset<SageBinaryData.ScenarioManagerData>(nameof(SageBinaryData.ScenarioManagerData.ScenarioTemplate), 32);
        ExpectOffset<SageBinaryData.ScenarioManagerData>(nameof(SageBinaryData.ScenarioManagerData.UnlockableUnit), 40);
        // Reborn: Pin the fieldless EP1 Red Alert button to the 36-byte base observed in static.bin.
        ExpectSize<SageBinaryData.UIMouseSimpleFixedButton>(36);
        ExpectOffset<SageBinaryData.UIMouseSimpleFixedButton>(nameof(SageBinaryData.UIMouseSimpleFixedButton.MouseOverHelp), 4);
        ExpectSize<SageBinaryData.UIMouseTacticalRedAlertButton>(36);
        // Reborn: Pin both EP1-only music conditions to their real global.bin root sizes.
        ExpectSize<SageBinaryData.MusicScriptConditionNugget_LocalPlayerHitRedAlertButton>(8);
        ExpectOffset<SageBinaryData.MusicScriptConditionNugget_LocalPlayerHitRedAlertButton>(nameof(SageBinaryData.MusicScriptConditionNugget_LocalPlayerHitRedAlertButton.DurationToReturnTrueAfterButtonHit), 4);
        ExpectSize<SageBinaryData.MusicScriptConditionNugget_ObjectTypesInProximity>(28);
        ExpectOffset<SageBinaryData.MusicScriptConditionNugget_ObjectTypesInProximity>(nameof(SageBinaryData.MusicScriptConditionNugget_ObjectTypesInProximity.TypeAFilter), 8);
        ExpectOffset<SageBinaryData.MusicScriptConditionNugget_ObjectTypesInProximity>(nameof(SageBinaryData.MusicScriptConditionNugget_ObjectTypesInProximity.TypeBFilter), 16);
        ExpectOffset<SageBinaryData.MusicScriptConditionNugget_ObjectTypesInProximity>(nameof(SageBinaryData.MusicScriptConditionNugget_ObjectTypesInProximity.Distance), 24);
        // Reborn: Pin the EP1 map-name heuristic to the 16-byte polymorphic record seen in global.bin.
        ExpectSize<SageBinaryData.AIStateMapNameHeuristic>(16);
        ExpectOffset<SageBinaryData.AIStateMapNameHeuristic>(nameof(SageBinaryData.AIStateMapNameHeuristic.Name), 4);
        ExpectOffset<SageBinaryData.AIStateMapNameHeuristic>(nameof(SageBinaryData.AIStateMapNameHeuristic.PassIfTrue), 12);
        // Reborn: Pin the complete EP1 joint hierarchy to the real ragdoll record strides.
        ExpectSize<SageBinaryData.DynamicsJointLinkType>(12);
        ExpectOffset<SageBinaryData.DynamicsJointLinkType>(nameof(SageBinaryData.DynamicsJointLinkType.Position), 8);
        ExpectSize<SageBinaryData.DynamicsJointFrameType>(24);
        ExpectOffset<SageBinaryData.DynamicsJointFrameType>(nameof(SageBinaryData.DynamicsJointFrameType.Parent), 12);
        ExpectSize<SageBinaryData.DynamicsJointLimitsType>(32);
        ExpectOffset<SageBinaryData.DynamicsJointLimitsType>(nameof(SageBinaryData.DynamicsJointLimitsType.Position), 28);
        ExpectSize<SageBinaryData.DynamicsJointType>(56);
        ExpectOffset<SageBinaryData.DynamicsJointType>(nameof(SageBinaryData.DynamicsJointType.Limits), 24);
        ExpectSize<SageBinaryData.DynamicsJointSetType>(8);
        // Reborn: Prevent the shared scripted-model base from regressing to its 276-byte Kane's Wrath layout.
        ExpectSize<SageBinaryData.W3DScriptedModelDrawModuleData>(216);
        ExpectOffset<SageBinaryData.W3DScriptedModelDrawModuleData>(nameof(SageBinaryData.W3DScriptedModelDrawModuleData.ModelConditionState), 164);
        ExpectOffset<SageBinaryData.W3DScriptedModelDrawModuleData>(nameof(SageBinaryData.W3DScriptedModelDrawModuleData.OkToChangeModelColor), 196);
        // Reborn: Pin the RA3 nested scripted-model records that replaced KW strings and fade fields.
        ExpectSize<SageBinaryData.ModelConditionState>(196);
        ExpectOffset<SageBinaryData.ModelConditionState>(nameof(SageBinaryData.ModelConditionState.Material), 112);
        ExpectOffset<SageBinaryData.ModelConditionState>(nameof(SageBinaryData.ModelConditionState.SubObject), 184);
        ExpectSize<SageBinaryData.AnimationState>(140);
        ExpectOffset<SageBinaryData.AnimationState>(nameof(SageBinaryData.AnimationState.Flags), 88);
        ExpectSize<SageBinaryData.Animation>(60);
        ExpectOffset<SageBinaryData.Animation>(nameof(SageBinaryData.Animation.AnimationAbsoluteTime), 40);
        ExpectSize<SageBinaryData.ParticleSysBone>(36);
        ExpectOffset<SageBinaryData.ParticleSysBone>(nameof(SageBinaryData.ParticleSysBone.FXAction), 20);
        // Reborn: Pin official shared dynamics sizes and the EP1 +4-byte joint-pointer extension.
        ExpectSize<SageBinaryData.DynamicsShapeType>(8);
        ExpectSize<SageBinaryData.DynamicsSphereShapeType>(12);
        ExpectSize<SageBinaryData.DynamicsCapsuleShapeType>(16);
        ExpectSize<SageBinaryData.DynamicsBoxShapeType>(24);
        ExpectSize<SageBinaryData.DynamicsCylinderShapeType>(20);
        ExpectSize<SageBinaryData.DynamicsTriangleShapeType>(48);
        ExpectSize<SageBinaryData.DynamicsVolumeType>(76);
        ExpectOffset<SageBinaryData.DynamicsVolumeType>(nameof(SageBinaryData.DynamicsVolumeType.Sphere), 36);
        ExpectSize<SageBinaryData.DynamicsBoneVolumeType>(84);
        ExpectSize<SageBinaryData.DynamicsBoneVolumeSetType>(8);
        ExpectSize<SageBinaryData.DynamicsLifetime>(8);
        ExpectSize<SageBinaryData.W3DDynamicsDrawModuleData>(256);
        ExpectOffset<SageBinaryData.W3DDynamicsDrawModuleData>(nameof(SageBinaryData.W3DDynamicsDrawModuleData.BoneVolumes), 240);
        ExpectOffset<SageBinaryData.W3DDynamicsDrawModuleData>(nameof(SageBinaryData.W3DDynamicsDrawModuleData.Lifetime), 244);
        ExpectOffset<SageBinaryData.W3DDynamicsDrawModuleData>(nameof(SageBinaryData.W3DDynamicsDrawModuleData.Joints), 248);
        ExpectOffset<SageBinaryData.W3DDynamicsDrawModuleData>(nameof(SageBinaryData.W3DDynamicsDrawModuleData.InitiallyActive), 252);
        ExpectSize<SageBinaryData.DynamicsCollideModuleData>(8);
        ExpectSize<SageBinaryData.MagnitudeSoundSelectorEntry>(8);
        ExpectOffset<SageBinaryData.MagnitudeSoundSelectorEntry>(nameof(SageBinaryData.MagnitudeSoundSelectorEntry.Sound), 4);
        ExpectSize<SageBinaryData.MagnitudeSoundSelectorTable>(8);
        ExpectSize<SageBinaryData.AudioDynamicsCollideModuleData>(20);
        ExpectOffset<SageBinaryData.AudioDynamicsCollideModuleData>(nameof(SageBinaryData.AudioDynamicsCollideModuleData.MinimumImpactVelocity), 8);
        ExpectOffset<SageBinaryData.AudioDynamicsCollideModuleData>(nameof(SageBinaryData.AudioDynamicsCollideModuleData.MagnitudeSoundSelector), 12);
        ExpectSize<SageBinaryData.WeaponEffectNugget>(40);
        ExpectOffset<SageBinaryData.WeaponEffectNugget>(nameof(SageBinaryData.WeaponEffectNugget.Radius), 16);
        ExpectOffset<SageBinaryData.WeaponEffectNugget>(nameof(SageBinaryData.WeaponEffectNugget.SpecialObjectFilter), 20);
        ExpectOffset<SageBinaryData.WeaponEffectNugget>(nameof(SageBinaryData.WeaponEffectNugget.RequiredUpgrade), 24);
        ExpectSize<SageBinaryData.DamageNuggetType>(156);
        ExpectOffset<SageBinaryData.DamageNuggetType>(nameof(SageBinaryData.DamageNuggetType.Damage), 40);
        ExpectOffset<SageBinaryData.DamageNuggetType>(nameof(SageBinaryData.DamageNuggetType.InvalidTargetStatus), 116);
        ExpectOffset<SageBinaryData.DamageNuggetType>(nameof(SageBinaryData.DamageNuggetType.DamageArcInverted), 148);
        ExpectOffset<SageBinaryData.DamageNuggetType>(nameof(SageBinaryData.DamageNuggetType.RadiusAffectsBridges), 155);
        ExpectSize<SageBinaryData.DamageDynamicsCollideModuleData>(20);
        ExpectOffset<SageBinaryData.DamageDynamicsCollideModuleData>(nameof(SageBinaryData.DamageDynamicsCollideModuleData.MaxMagnitude), 8);
        ExpectOffset<SageBinaryData.DamageDynamicsCollideModuleData>(nameof(SageBinaryData.DamageDynamicsCollideModuleData.DamageNugget), 12);
        ExpectSize<SageBinaryData.ReactionFXTriggerData>(24);
        ExpectOffset<SageBinaryData.ReactionFXTriggerData>(nameof(SageBinaryData.ReactionFXTriggerData.PercentDamagedThreshold), 0);
        ExpectOffset<SageBinaryData.ReactionFXTriggerData>(nameof(SageBinaryData.ReactionFXTriggerData.TimeBetweenTriggers), 4);
        ExpectOffset<SageBinaryData.ReactionFXTriggerData>(nameof(SageBinaryData.ReactionFXTriggerData.SourceFilter), 8);
        ExpectOffset<SageBinaryData.ReactionFXTriggerData>(nameof(SageBinaryData.ReactionFXTriggerData.NameOfVoiceToPlay), 12);
        ExpectOffset<SageBinaryData.ReactionFXTriggerData>(nameof(SageBinaryData.ReactionFXTriggerData.SoundToPlay), 16);
        ExpectOffset<SageBinaryData.ReactionFXTriggerData>(nameof(SageBinaryData.ReactionFXTriggerData.ResetTimerWhenTimerBlocks), 20);
        ExpectSize<SageBinaryData.ReactionFXOnDamageModuleData>(24);
        ExpectOffset<SageBinaryData.ReactionFXOnDamageModuleData>(nameof(SageBinaryData.ReactionFXOnDamageModuleData.DamageReactionFXTrigger), 8);
        ExpectOffset<SageBinaryData.ReactionFXOnDamageModuleData>(nameof(SageBinaryData.ReactionFXOnDamageModuleData.HealingReactionFXTrigger), 16);
        ExpectSize<SageBinaryData.SphereModuleUpdateModuleData>(176);
        ExpectOffset<SageBinaryData.SphereModuleUpdateModuleData>(nameof(SageBinaryData.SphereModuleUpdateModuleData.RadiusMax), 8);
        ExpectOffset<SageBinaryData.SphereModuleUpdateModuleData>(nameof(SageBinaryData.SphereModuleUpdateModuleData.SphereBoneName), 32);
        ExpectOffset<SageBinaryData.SphereModuleUpdateModuleData>(nameof(SageBinaryData.SphereModuleUpdateModuleData.ObjectFilter), 48);
        ExpectOffset<SageBinaryData.SphereModuleUpdateModuleData>(nameof(SageBinaryData.SphereModuleUpdateModuleData.IgnoreInsideToInsideCheck), 168);
        ExpectOffset<SageBinaryData.SphereModuleUpdateModuleData>(nameof(SageBinaryData.SphereModuleUpdateModuleData.InitiallyActive), 172);
        ExpectOffset<SageBinaryData.SphereModuleUpdateModuleData>(nameof(SageBinaryData.SphereModuleUpdateModuleData.DrawDebugCircle), 173);
        ExpectSize<SageBinaryData.DamageSphereUpdateModuleData>(372);
        ExpectOffset<SageBinaryData.DamageSphereUpdateModuleData>(nameof(SageBinaryData.DamageSphereUpdateModuleData.UnpackTime), 176);
        ExpectOffset<SageBinaryData.DamageSphereUpdateModuleData>(nameof(SageBinaryData.DamageSphereUpdateModuleData.UnpackModelConditions), 188);
        ExpectOffset<SageBinaryData.DamageSphereUpdateModuleData>(nameof(SageBinaryData.DamageSphereUpdateModuleData.ObjectStatus), 340);
        ExpectSize<SageBinaryData.ShieldSphereUpdateOptionFlag>(4);
        ExpectSize<SageBinaryData.ShieldSphereUpdateModuleData>(312);
        ExpectOffset<SageBinaryData.ShieldSphereUpdateModuleData>(nameof(SageBinaryData.ShieldSphereUpdateModuleData.MaxDamage), 176);
        ExpectOffset<SageBinaryData.ShieldSphereUpdateModuleData>(nameof(SageBinaryData.ShieldSphereUpdateModuleData.ObjectStatus), 180);
        ExpectOffset<SageBinaryData.ShieldSphereUpdateModuleData>(nameof(SageBinaryData.ShieldSphereUpdateModuleData.ModelCondition), 212);
        ExpectOffset<SageBinaryData.ShieldSphereUpdateModuleData>(nameof(SageBinaryData.ShieldSphereUpdateModuleData.AttributeModifierName), 272);
        ExpectOffset<SageBinaryData.ShieldSphereUpdateModuleData>(nameof(SageBinaryData.ShieldSphereUpdateModuleData.ShieldedObjectStatus), 276);
        ExpectOffset<SageBinaryData.ShieldSphereUpdateModuleData>(nameof(SageBinaryData.ShieldSphereUpdateModuleData.Options), 308);
        ExpectSize<SageBinaryData.YurikoShieldSphereUpdateModuleData>(328);
        ExpectOffset<SageBinaryData.YurikoShieldSphereUpdateModuleData>(nameof(SageBinaryData.YurikoShieldSphereUpdateModuleData.MajorShieldHitFX), 312);
        ExpectOffset<SageBinaryData.YurikoShieldSphereUpdateModuleData>(nameof(SageBinaryData.YurikoShieldSphereUpdateModuleData.MinorShieldDamageTypes), 320);

        Console.WriteLine("Uprising layout self-test: OK");
    }

    private static void TestPatchManifestTotals()
    {
        ManifestHeader header = new(
            7, false, true, 0, 0x5454A8E9, 1, 16, 0, 0, 0, 0, 0, 0, 0, 4);
        ManifestAsset asset = new(
            1, 2, 3, 4, 0, 0, 0, 0, 8, 0, 0, 1,
            "Test:Patch", "Test.xml", Array.Empty<AssetId>());
        ManifestDocument patch = new(
            header, [asset], [new ReferencedManifest("base.manifest", true)], null, 0, false);
        if (patch.Validate().Count != 0)
        {
            throw new InvalidDataException("Patch manifests must allow logical instance totals larger than local chunks.");
        }

        ManifestDocument standalone = patch with { ReferencedManifests = Array.Empty<ReferencedManifest>() };
        if (standalone.Validate().Count != 1)
        {
            throw new InvalidDataException("Standalone manifests must still validate the instance chunk total.");
        }
    }

    private static void ExpectSize<T>(int expected) where T : struct =>
        Expect(Marshal.SizeOf<T>() == expected, $"sizeof({typeof(T).Name})", expected, Marshal.SizeOf<T>());

    private static void ExpectOffset<T>(string field, int expected) where T : struct =>
        Expect(Marshal.OffsetOf<T>(field).ToInt32() == expected, $"offsetof({typeof(T).Name}.{field})", expected, Marshal.OffsetOf<T>(field).ToInt32());

    private static void Expect(bool condition, string name, int expected, int actual)
    {
        if (!condition)
        {
            throw new InvalidDataException($"{name}: expected {expected}, got {actual}.");
        }
    }
}
