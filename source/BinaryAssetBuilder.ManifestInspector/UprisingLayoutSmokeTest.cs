using System.Runtime.InteropServices;

namespace BinaryAssetBuilder.ManifestInspector;

internal static class UprisingLayoutSmokeTest
{
    public static void Run()
    {
        TestPatchManifestTotals();
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
        ExpectSize<SageBinaryData.StoreObjectsSpecialPowerModuleData>(256);
        ExpectSize<SageBinaryData.AddObjectsToLiftUpdateSpecialPowerModuleData>(260);
        ExpectOffset<SageBinaryData.AddObjectsToLiftUpdateSpecialPowerModuleData>(nameof(SageBinaryData.AddObjectsToLiftUpdateSpecialPowerModuleData.LiftObjectLinkID), 256);
        ExpectSize<SageBinaryData.AddObjectsToLureUpdateSpecialPowerModuleData>(260);
        ExpectOffset<SageBinaryData.AddObjectsToLureUpdateSpecialPowerModuleData>(nameof(SageBinaryData.AddObjectsToLureUpdateSpecialPowerModuleData.LureObjectLinkID), 256);
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
