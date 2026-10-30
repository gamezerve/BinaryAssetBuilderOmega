using BinaryAssetBuilder.Core.Diagnostics;
using Relo;
using SageBinaryData;

public static partial class Marshaler
{
    public static unsafe void Marshal(Node node, RangeDuration* objT, Tracker state)
    {
        if (node is null)
        {
            return;
        }
        Marshal(node.GetAttributeValue(nameof(RangeDuration.MinSeconds), "0s"), &objT->MinSeconds, state);
        Marshal(node.GetAttributeValue(nameof(RangeDuration.MaxSeconds), "0s"), &objT->MaxSeconds, state);
    }

    public static unsafe void Marshal(Node node, RangeDuration** objT, Tracker state)
    {
        if (node is null)
        {
            return;
        }
        using Tracker.Context context = state.Push((void**)objT, (uint)sizeof(RangeDuration), 1u);
        Marshal(node, *objT, state);
    }

    public static unsafe void Marshal(Node node, LinearTargetType* objT, Tracker state)
    {
        if (node is null)
        {
            return;
        }
        Marshal(node.GetAttributeValue(nameof(LinearTargetType.Time), null), &objT->Time, state);
        Marshal(node.GetChildNode(nameof(LinearTargetType.Position), null), &objT->Position, state);
    }

    public static unsafe void Marshal(Node node, LinearTargetType** objT, Tracker state)
    {
        if (node is null)
        {
            return;
        }
        using Tracker.Context context = state.Push((void**)objT, (uint)sizeof(LinearTargetType), 1u);
        Marshal(node, *objT, state);
    }

    public static unsafe void Marshal(Node node, WeaponEffectNugget* objT, Tracker state)
    {
        if (node is null)
        {
            return;
        }
        Marshal(node.GetAttributeValue(nameof(WeaponEffectNugget.PartitionFilterTestType), nameof(PartitionManagerDistTestType.EXTENTS_3D)), &objT->PartitionFilterTestType, state);
        Marshal(node.GetAttributeValue(nameof(WeaponEffectNugget.ForbiddenTargetObjectStatus), null), &objT->ForbiddenTargetObjectStatus, state);
        Marshal(node.GetAttributeValue(nameof(WeaponEffectNugget.ForbiddenTargetModelCondition), null), &objT->ForbiddenTargetModelCondition, state);
        Marshal(node.GetAttributeValue(nameof(WeaponEffectNugget.Radius), "0.0"), &objT->Radius, state);
        Marshal(node.GetChildNode(nameof(WeaponEffectNugget.SpecialObjectFilter), null), &objT->SpecialObjectFilter, state);
        Marshal(node.GetChildNodes(nameof(WeaponEffectNugget.RequiredUpgrade)), &objT->RequiredUpgrade, state);
        Marshal(node.GetChildNodes(nameof(WeaponEffectNugget.ForbiddenUpgrade)), &objT->ForbiddenUpgrade, state);
    }

    public static unsafe void Marshal(Node node, ScalarInfo* objT, Tracker state)
    {
        if (node is null)
        {
            return;
        }
        Marshal(node.GetAttributeValue(nameof(ScalarInfo.Scalar), "100%"), &objT->Scalar, state);
        Marshal(node.GetChildNode(nameof(ScalarInfo.Filter), null), &objT->Filter, state);
        Marshal(node.GetChildNodes(nameof(ScalarInfo.RequiredUpgrade)), &objT->RequiredUpgrade, state);
    }

    public static unsafe void Marshal(Node node, ParalyzeNuggetType* objT, Tracker state)
    {
        if (node is null)
        {
            return;
        }
        Marshal(node.GetAttributeValue(nameof(ParalyzeNuggetType.Radius), null), &objT->Radius, state);
        Marshal(node.GetAttributeValue(nameof(ParalyzeNuggetType.EffectArc), "360d"), &objT->EffectArc, state);
        Marshal(node.GetAttributeValue(nameof(ParalyzeNuggetType.DurationSeconds), "0s"), &objT->DurationSeconds, state);
        Marshal(node.GetAttributeValue(nameof(ParalyzeNuggetType.ParalyzeType), "NONE"), &objT->ParalyzeType, state);
        Marshal(node.GetAttributeValue(nameof(ParalyzeNuggetType.RemoveParalyzeType), "NONE"), &objT->RemoveParalyzeType, state);
        Marshal(node.GetAttributeValue(nameof(ParalyzeNuggetType.ParalyzeFX), null), &objT->ParalyzeFX, state);
        Marshal(node, (WeaponEffectNugget*)objT, state);
    }

    public static unsafe void Marshal(Node node, InformationWarfareNuggetType* objT, Tracker state)
    {
        if (node is null)
        {
            return;
        }
        Marshal(node.GetAttributeValue(nameof(InformationWarfareNuggetType.InfoWarType), nameof(InfoWarEffect.NONE)), &objT->InfoWarType, state);
        Marshal(node.GetAttributeValue(nameof(InformationWarfareNuggetType.RadarJamRadius), "0"), &objT->RadarJamRadius, state);
        Marshal(node.GetAttributeValue(nameof(InformationWarfareNuggetType.RadarJamDuration), "0s"), &objT->RadarJamDuration, state);
        Marshal(node, (WeaponEffectNugget*)objT, state);
    }

    public static unsafe void Marshal(Node node, SpendStolenTiberiumNuggetType* objT, Tracker state)
    {
        if (node is null)
        {
            return;
        }
        Marshal(node.GetAttributeValue(nameof(SpendStolenTiberiumNuggetType.AmountToSpend), "1"), &objT->AmountToSpend, state);
        Marshal(node, (WeaponEffectNugget*)objT, state);
    }

    public static unsafe void Marshal(Node node, ReportWeaponFiredStatNuggetType* objT, Tracker state)
    {
        if (node is null)
        {
            return;
        }
        Marshal(node, (WeaponEffectNugget*)objT, state);
    }

    public static unsafe void Marshal(Node node, DamageNuggetType* objT, Tracker state)
    {
        if (node is null)
        {
            return;
        }
        Marshal(node.GetAttributeValue(nameof(DamageNuggetType.Damage), "0"), &objT->Damage, state);
        Marshal(node.GetAttributeValue(nameof(DamageNuggetType.DamageTaperOff), "-1.0"), &objT->DamageTaperOff, state);
        Marshal(node.GetAttributeValue(nameof(DamageNuggetType.MinRadius), "0.0"), &objT->MinRadius, state);
        Marshal(node.GetAttributeValue(nameof(DamageNuggetType.DamageArc), "360d"), &objT->DamageArc, state);
        Marshal(node.GetAttributeValue(nameof(DamageNuggetType.DamageArcInverted), "false"), &objT->DamageArcInverted, state);
        Marshal(node.GetAttributeValue(nameof(DamageNuggetType.DamageMaxHeight), "-1"), &objT->DamageMaxHeight, state);
        Marshal(node.GetAttributeValue(nameof(DamageNuggetType.DamageMaxHeightAboveTerrain), "-1"), &objT->DamageMaxHeightAboveTerrain, state);
        Marshal(node.GetAttributeValue(nameof(DamageNuggetType.AcceptDamageAdd), "true"), &objT->AcceptDamageAdd, state);
        Marshal(node.GetAttributeValue(nameof(DamageNuggetType.FlankingBonus), "0"), &objT->FlankingBonus, state);
        Marshal(node.GetAttributeValue(nameof(DamageNuggetType.FlankedScalar), "1"), &objT->FlankedScalar, state);
        Marshal(node.GetAttributeValue(nameof(DamageNuggetType.DelayTimeSeconds), "0s"), &objT->DelayTimeSeconds, state);
        Marshal(node.GetAttributeValue(nameof(DamageNuggetType.DamageType), nameof(DamageType.UNDEFINED)), &objT->DamageType, state);
        Marshal(node.GetAttributeValue(nameof(DamageNuggetType.DeathType), nameof(DeathType.NORMAL)), &objT->DeathType, state);
        Marshal(node.GetAttributeValue(nameof(DamageNuggetType.DamageFXType), nameof(DamageFXType.UNDEFINED)), &objT->DamageFXType, state);
        Marshal(node.GetAttributeValue(nameof(DamageNuggetType.DamageSubType), nameof(DamageSubType.NORMAL)), &objT->DamageSubType, state);
        Marshal(node.GetAttributeValue(nameof(DamageNuggetType.OnlyKillOwnerWhenTriggered), "false"), &objT->OnlyKillOwnerWhenTriggered, state);
        Marshal(node.GetAttributeValue(nameof(DamageNuggetType.DrainLifeMultiplier), "1"), &objT->DrainLifeMultiplier, state);
        Marshal(node.GetAttributeValue(nameof(DamageNuggetType.DrainLife), "false"), &objT->DrainLife, state);
        Marshal(node.GetAttributeValue(nameof(DamageNuggetType.DamageSpeed), "0"), &objT->DamageSpeed, state);
        // Reborn: EP1's schema-inserted empty EvaEventRef means no override; allocating an empty string shifts every subsequent native block.
        Value evaOverride = node.GetAttributeValue(nameof(DamageNuggetType.UnderAttackOverrideEvaEvent), null);
        if (evaOverride is not null && !string.IsNullOrWhiteSpace(evaOverride.GetText()))
            Marshal(evaOverride, &objT->UnderAttackOverrideEvaEvent, state);
        Marshal(node.GetAttributeValue(nameof(DamageNuggetType.VictimShroudRevealer), null), &objT->VictimShroudRevealer, state);
        Marshal(node.GetChildNodes(nameof(DamageNuggetType.DamageScalarDetails)), &objT->DamageScalarDetails, state);
        Marshal(node.GetAttributeValue(nameof(DamageNuggetType.NotifyOwnerOnVictimDeath), "false"), &objT->NotifyOwnerOnVictimDeath, state);
        Marshal(node.GetAttributeValue(nameof(DamageNuggetType.NotifyObserversOnPreDamageEffectPosition), "false"), &objT->NotifyObserversOnPreDamageEffectPosition, state);
        Marshal(node.GetAttributeValue(nameof(DamageNuggetType.ForceFXPositionToVictim), "false"), &objT->ForceFXPositionToVictim, state);
        Marshal(node.GetAttributeValue(nameof(DamageNuggetType.RadiusAffectsBridges), "false"), &objT->RadiusAffectsBridges, state);
        Marshal(node.GetAttributeValue(
            nameof(DamageNuggetType.InvalidTargetStatus),
            "INSIDE_GARRISON UNATTACKABLE UNDER_IRON_CURTAIN"),
            &objT->InvalidTargetStatus,
            state);
        Marshal(node, (WeaponEffectNugget*)objT, state);
    }

#if KANESWRATH
    public static unsafe void Marshal(Node node, MetaGameOperationNuggetType* objT, Tracker state)
    {
        if (node is null)
        {
            return;
        }
        Marshal(node.GetAttributeValue(nameof(MetaGameOperationNuggetType.OperationToFire), nameof(MetagameOperationsEnums.INVALID)), &objT->OperationToFire, state);
        Marshal(node.GetAttributeValue(nameof(MetaGameOperationNuggetType.SpecialPowerTemplate), null), &objT->SpecialPowerTemplate, state);
        Marshal(node, (DamageNuggetType*)objT, state);
    }

    public static unsafe void Marshal(Node node, DOTNuggetType* objT, Tracker state)
    {
        if (node is null)
        {
            return;
        }
        Marshal(node.GetAttributeValue(nameof(DOTNuggetType.DamageInterval), null), &objT->DamageInterval, state);
        Marshal(node.GetAttributeValue(nameof(DOTNuggetType.DamageDuration), null), &objT->DamageDuration, state);
        Marshal(node.GetAttributeValue(nameof(DOTNuggetType.DamageDecay), "0"), &objT->DamageDecay, state);
        Marshal(node.GetAttributeValue(nameof(DOTNuggetType.UseMetaGameTurns), "false"), &objT->UseMetaGameTurns, state);
        Marshal(node.GetAttributeValue(nameof(DOTNuggetType.AppliedStatus), null), &objT->AppliedStatus, state);
        Marshal(node, (DamageNuggetType*)objT, state);
    }
#endif

    public static unsafe void Marshal(Node node, DamageAndSpawnNuggetType* objT, Tracker state)
    {
        if (node is null)
        {
            return;
        }
        Marshal(node.GetAttributeValue(nameof(DamageAndSpawnNuggetType.SpawnProbability), "1.0"), &objT->SpawnProbability, state);
        Marshal(node.GetAttributeValue(nameof(DamageAndSpawnNuggetType.SpawnedModelConditionFlags), null), &objT->SpawnedModelConditionFlags, state);
        Marshal(node.GetChildNode(nameof(DamageAndSpawnNuggetType.SpawnTemplate), null), &objT->SpawnTemplate, state);
        Marshal(node, (DamageNuggetType*)objT, state);
    }

    public static unsafe void Marshal(Node node, FireLogicNuggetType* objT, Tracker state)
    {
        if (node is null)
        {
            return;
        }
        Marshal(node.GetAttributeValue(nameof(FireLogicNuggetType.FireEffect), null), &objT->FireEffect, state);
        Marshal(node.GetAttributeValue(nameof(FireLogicNuggetType.MinMaxBurnRate), null), &objT->MinMaxBurnRate, state);
        Marshal(node.GetAttributeValue(nameof(FireLogicNuggetType.MinDecay), null), &objT->MinDecay, state);
        Marshal(node.GetAttributeValue(nameof(FireLogicNuggetType.MaxResistance), null), &objT->MaxResistance, state);
        Marshal(node, (DamageNuggetType*)objT, state);
    }

    public static unsafe void Marshal(Node node, FireOnObjectsNuggetType* objT, Tracker state)
    {
        if (node is null)
        {
            return;
        }
        Marshal(node.GetAttributeValue(nameof(FireOnObjectsNuggetType.Weapon), null), &objT->Weapon, state);
        Marshal(node.GetChildNode(nameof(FireOnObjectsNuggetType.Filter), null), &objT->Filter, state);
        Marshal(node, (DamageNuggetType*)objT, state);
    }

    public static unsafe void Marshal(Node node, TintObjectsNuggetType* objT, Tracker state)
    {
        if (node is null)
        {
            return;
        }
        Marshal(node.GetAttributeValue(nameof(TintObjectsNuggetType.PreColorTime), "2s"), &objT->PreColorTime, state);
        Marshal(node.GetAttributeValue(nameof(TintObjectsNuggetType.PostColorTime), "2s"), &objT->PostColorTime, state);
        Marshal(node.GetAttributeValue(nameof(TintObjectsNuggetType.SustainedColorTime), "1s"), &objT->SustainedColorTime, state);
        Marshal(node.GetAttributeValue(nameof(TintObjectsNuggetType.Frequency), "1.0"), &objT->Frequency, state);
        Marshal(node.GetAttributeValue(nameof(TintObjectsNuggetType.Amplitude), "1.0"), &objT->Amplitude, state);
        Marshal(node.GetChildNode(nameof(TintObjectsNuggetType.Color), null), &objT->Color, state);
        // Reborn: preserve the official 40-byte effect header before tint fields at offset 40.
        Marshal(node, (WeaponEffectNugget*)objT, state);
    }

    public static unsafe void Marshal(Node node, DamageFieldNuggetType* objT, Tracker state)
    {
        if (node is null)
        {
            return;
        }
        Marshal(node.GetAttributeValue(nameof(DamageFieldNuggetType.WeaponTemplate), null), &objT->WeaponTemplate, state);
        Marshal(node.GetAttributeValue(nameof(DamageFieldNuggetType.DurationSeconds), "0s"), &objT->DurationSeconds, state);
        Marshal(node, (WeaponEffectNugget*)objT, state);
    }

    public static unsafe void Marshal(Node node, VeteranProjectile* objT, Tracker state)
    {
        if (node is null)
        {
            return;
        }
        Marshal(node.GetAttributeValue(nameof(VeteranProjectile.VeterancyLevel), null), &objT->VeterancyLevel, state);
        Marshal(node.GetAttributeValue(nameof(VeteranProjectile.ProjectileTemplate), null), &objT->ProjectileTemplate, state);
    }

    public static unsafe void Marshal(Node node, ProjectileNuggetType* objT, Tracker state)
    {
        if (node is null)
        {
            return;
        }
        Marshal(node.GetAttributeValue(nameof(ProjectileNuggetType.WarheadTemplate), null), &objT->WarheadTemplate, state);
        Marshal(node.GetAttributeValue(nameof(ProjectileNuggetType.ProjectileTemplate), null), &objT->ProjectileTemplate, state);
        Marshal(node.GetAttributeValue(nameof(ProjectileNuggetType.WeaponLaunchBoneSlotOverride), nameof(WeaponSlotType.NO_WEAPON)), &objT->WeaponLaunchBoneSlotOverride, state);
        Marshal(node.GetChildNode(nameof(ProjectileNuggetType.AttackOffset), null), &objT->AttackOffset, state);
        Marshal(node.GetChildNodes(nameof(ProjectileNuggetType.VeterancyProjectiles)), &objT->VeterancyProjectiles, state);
        Marshal(node, (WeaponEffectNugget*)objT, state);
    }

    public static unsafe void Marshal(Node node, SuppressionNuggetType* objT, Tracker state)
    {
        if (node is null)
        {
            return;
        }
        Marshal(node.GetAttributeValue(nameof(SuppressionNuggetType.Radius), "0"), &objT->Radius, state);
        Marshal(node.GetAttributeValue(nameof(SuppressionNuggetType.Suppression), "0"), &objT->Suppression, state);
        Marshal(node.GetAttributeValue(nameof(SuppressionNuggetType.DurationSeconds), "0s"), &objT->DurationSeconds, state);
        Marshal(node, (WeaponEffectNugget*)objT, state);
    }

    public static unsafe void Marshal(Node node, WeaponOCLNuggetType* objT, Tracker state)
    {
        if (node is null)
        {
            return;
        }
        Marshal(node.GetAttributeValue(nameof(WeaponOCLNuggetType.WeaponOCL), null), &objT->WeaponOCL, state);
        Marshal(node, (WeaponEffectNugget*)objT, state);
    }

    public static unsafe void Marshal(Node node, ActivateLaserNuggetType* objT, Tracker state)
    {
        if (node is null)
        {
            return;
        }
        Marshal(node.GetAttributeValue(nameof(ActivateLaserNuggetType.Lifetime), "1s"), &objT->Lifetime, state);
        Marshal(node.GetAttributeValue(nameof(ActivateLaserNuggetType.LaserId), "0"), &objT->LaserId, state);
        Marshal(node.GetAttributeValue(nameof(ActivateLaserNuggetType.HitGroundFX), null), &objT->HitGroundFX, state);
        Marshal(node, (WeaponEffectNugget*)objT, state);
    }

    public static unsafe void Marshal(Node node, ActivateStreamNuggetType* objT, Tracker state)
    {
        if (node is null)
        {
            return;
        }
        Marshal(node.GetAttributeValue(nameof(ActivateStreamNuggetType.Lifetime), "1s"), &objT->Lifetime, state);
        Marshal(node.GetAttributeValue(nameof(ActivateStreamNuggetType.StreamId), "0"), &objT->StreamId, state);
        Marshal(node, (WeaponEffectNugget*)objT, state);
    }

    public static unsafe void Marshal(Node node, ActivateLinearDamageNuggetType* objT, Tracker state)
    {
        if (node is null)
        {
            return;
        }
        Marshal(node.GetAttributeValue(nameof(ActivateLinearDamageNuggetType.Lifetime), "1.0s"), &objT->Lifetime, state);
        Marshal(node, (WeaponEffectNugget*)objT, state);
    }

    public static unsafe void Marshal(Node node, SlavesAttackNuggetType* objT, Tracker state)
    {
        if (node is null)
        {
            return;
        }
        Marshal(node, (WeaponEffectNugget*)objT, state);
    }

    public static unsafe void Marshal(Node node, MetaImpactNuggetType* objT, Tracker state)
    {
        if (node is null)
        {
            return;
        }
        Marshal(node.GetAttributeValue(nameof(MetaImpactNuggetType.ShockWaveAmount), "0"), &objT->ShockWaveAmount, state);
        Marshal(node.GetAttributeValue(nameof(MetaImpactNuggetType.ShockWaveRadius), "0"), &objT->ShockWaveRadius, state);
        Marshal(node.GetAttributeValue(nameof(MetaImpactNuggetType.ShockWaveArc), "360d"), &objT->ShockWaveArc, state);
        Marshal(node.GetAttributeValue(nameof(MetaImpactNuggetType.ShockWaveArcInverted), "false"), &objT->ShockWaveArcInverted, state);
        Marshal(node.GetAttributeValue(nameof(MetaImpactNuggetType.ShockWaveTaperOff), "0"), &objT->ShockWaveTaperOff, state);
        Marshal(node.GetAttributeValue(nameof(MetaImpactNuggetType.ShockWaveSpeed), "0"), &objT->ShockWaveSpeed, state);
        Marshal(node.GetAttributeValue(nameof(MetaImpactNuggetType.ShockWaveZMult), "1"), &objT->ShockWaveZMult, state);
        Marshal(node.GetAttributeValue(nameof(MetaImpactNuggetType.CyclonicFactor), "0"), &objT->CyclonicFactor, state);
        Marshal(node.GetAttributeValue(nameof(MetaImpactNuggetType.ShockwaveDelaySeconds), "0s"), &objT->ShockwaveDelaySeconds, state);
        Marshal(node.GetAttributeValue(nameof(MetaImpactNuggetType.Suppression), "0"), &objT->Suppression, state);
        Marshal(node.GetAttributeValue(nameof(MetaImpactNuggetType.InvertShockWave), "false"), &objT->InvertShockWave, state);
        Marshal(node.GetAttributeValue(nameof(MetaImpactNuggetType.FlipDirection), "false"), &objT->FlipDirection, state);
        Marshal(node.GetAttributeValue(nameof(MetaImpactNuggetType.OnlyWhenJustDied), "false"), &objT->OnlyWhenJustDied, state);
        Marshal(node.GetAttributeValue(nameof(MetaImpactNuggetType.ShockWaveClearRadius), "false"), &objT->ShockWaveClearRadius, state);
        Marshal(node.GetAttributeValue(nameof(MetaImpactNuggetType.HeroResist), "0"), &objT->HeroResist, state);
        Marshal(node.GetAttributeValue(nameof(MetaImpactNuggetType.ShockWaveClearWaveMult), "2"), &objT->ShockWaveClearWaveMult, state);
        Marshal(node.GetAttributeValue(nameof(MetaImpactNuggetType.ShockWaveClearFlingHeight), "100"), &objT->ShockWaveClearFlingHeight, state);
        Marshal(node.GetChildNode(nameof(MetaImpactNuggetType.KillObjectFilter), null), &objT->KillObjectFilter, state);
        Marshal(node, (WeaponEffectNugget*)objT, state);
    }

    public static unsafe void Marshal(Node node, SpecialPowerNuggetType* objT, Tracker state)
    {
        if (node is null)
        {
            return;
        }
        Marshal(node.GetAttributeValue(nameof(SpecialPowerNuggetType.SpecialPowerTemplate), null), &objT->SpecialPowerTemplate, state);
        Marshal(node, (WeaponEffectNugget*)objT, state);
    }

    public static unsafe void Marshal(Node node, AttributeModifierNuggetType* objT, Tracker state)
    {
        if (node is null)
        {
            return;
        }
        Marshal(node.GetAttributeValue(nameof(AttributeModifierNuggetType.AttributeModifierName), null), &objT->AttributeModifierName, state);
        Marshal(node.GetAttributeValue(nameof(AttributeModifierNuggetType.AttributeModifierOwnerName), null), &objT->AttributeModifierOwnerName, state);
        Marshal(node.GetAttributeValue(nameof(AttributeModifierNuggetType.DamageFXType), null), &objT->DamageFXType, state);
        Marshal(node.GetAttributeValue(nameof(AttributeModifierNuggetType.Radius), "0.0"), &objT->Radius, state);
        Marshal(node.GetAttributeValue(nameof(AttributeModifierNuggetType.DamageArc), "360d"), &objT->DamageArc, state);
        Marshal(node.GetAttributeValue(nameof(AttributeModifierNuggetType.AntiCategories), null), &objT->AntiCategories, state);
        Marshal(node.GetAttributeValue(nameof(AttributeModifierNuggetType.AntiFX), null), &objT->AntiFX, state);
        Marshal(node, (WeaponEffectNugget*)objT, state);
    }

    public static unsafe void Marshal(Node node, DamageContainedNuggetType* objT, Tracker state)
    {
        if (node is null)
        {
            return;
        }
        Marshal(node.GetAttributeValue(nameof(DamageContainedNuggetType.MaxUnitsToDamage), "100"), &objT->MaxUnitsToDamage, state);
        Marshal(node.GetAttributeValue(nameof(DamageContainedNuggetType.WindowBlastFX), null), &objT->WindowBlastFX, state);
        Marshal(node.GetChildNode(nameof(DamageContainedNuggetType.DamageObjectFilter), null), &objT->DamageObjectFilter, state);
        Marshal(node, (DamageNuggetType*)objT, state);
    }

    public static unsafe void Marshal(Node node, LuaEventNuggetType* objT, Tracker state)
    {
        if (node is null)
        {
            return;
        }
        Marshal(node.GetAttributeValue(nameof(LuaEventNuggetType.EventName), null), &objT->EventName, state);
        Marshal(node.GetAttributeValue(nameof(LuaEventNuggetType.Radius), "0"), &objT->Radius, state);
        Marshal(node.GetAttributeValue(nameof(LuaEventNuggetType.SendToEnemies), "false"), &objT->SendToEnemies, state);
        Marshal(node.GetAttributeValue(nameof(LuaEventNuggetType.SendToAllies), "false"), &objT->SendToAllies, state);
        Marshal(node.GetAttributeValue(nameof(LuaEventNuggetType.SendToNeutral), "false"), &objT->SendToNeutral, state);
        Marshal(node, (WeaponEffectNugget*)objT, state);
    }

    public static unsafe void Marshal(Node node, LineDamageNuggetType* objT, Tracker state)
    {
        if (node is null)
        {
            return;
        }
        Marshal(node.GetAttributeValue(nameof(LineDamageNuggetType.OffsetAngle), "0d"), &objT->OffsetAngle, state);
        Marshal(node.GetAttributeValue(nameof(LineDamageNuggetType.LineWidth), "0"), &objT->LineWidth, state);
        Marshal(node.GetAttributeValue(nameof(LineDamageNuggetType.LineLengthLeadIn), "0"), &objT->LineLengthLeadIn, state);
        Marshal(node.GetAttributeValue(nameof(LineDamageNuggetType.LineLengthLeadOut), "0"), &objT->LineLengthLeadOut, state);
        Marshal(node, (DamageNuggetType*)objT, state);
    }

    public static unsafe void Marshal(Node node, SeedTiberiumNuggetType* objT, Tracker state)
    {
        if (node is null)
        {
            return;
        }
        Marshal(node.GetAttributeValue(nameof(SeedTiberiumNuggetType.FieldAmount), "0"), &objT->FieldAmount, state);
        Marshal(node.GetAttributeValue(nameof(SeedTiberiumNuggetType.SpawnedInFieldBonus), "0"), &objT->SpawnedInFieldBonus, state);
        Marshal(node, (WeaponOCLNuggetType*)objT, state);
    }

    public static unsafe void Marshal(Node node, ScatterProjectileNuggetType* objT, Tracker state)
    {
        if (node is null)
        {
            return;
        }
        Marshal(node.GetAttributeValue(nameof(ScatterProjectileNuggetType.ScatterMin), "0"), &objT->ScatterMin, state);
        Marshal(node.GetAttributeValue(nameof(ScatterProjectileNuggetType.ScatterMax), "0"), &objT->ScatterMax, state);
        Marshal(node, (WeaponEffectNugget*)objT, state);
    }

    public static unsafe void Marshal(Node node, ScatterRadiusType* objT, Tracker state)
    {
        if (node is null)
        {
            return;
        }
        Marshal(node.GetAttributeValue(nameof(ScatterRadiusType.Radius), "0.0"), &objT->Radius, state);
        Marshal(node.GetAttributeValue(nameof(ScatterRadiusType.RequiredModelConditions), ""), &objT->RequiredModelConditions, state);
        Marshal(node.GetChildNode(nameof(ScatterRadiusType.Filter), null), &objT->Filter, state);
    }

    public static unsafe void Marshal(Node node, ContainedObjectAttackNuggetType* objT, Tracker state)
    {
        if (node is null)
        {
            return;
        }
        Marshal(node, (WeaponEffectNugget*)objT, state);
    }

#if KANESWRATH
    public static unsafe void Marshal(Node node, CrushTiberiumNuggetType* objT, Tracker state)
    {
        if (node is null)
        {
            return;
        }
        Marshal(node.GetAttributeValue(nameof(CrushTiberiumNuggetType.DamageFXType), null), &objT->DamageFXType, state);
        Marshal(node.GetAttributeValue(nameof(CrushTiberiumNuggetType.Radius), "0.0"), &objT->Radius, state);
        Marshal(node.GetAttributeValue(nameof(CrushTiberiumNuggetType.DamageArc), "360d"), &objT->DamageArc, state);
        Marshal(node.GetAttributeValue(nameof(CrushTiberiumNuggetType.ProductionModifer), "100"), &objT->ProductionModifer, state);
        Marshal(node, (WeaponEffectNugget*)objT, state);
    }
#endif

    public static unsafe void Marshal(Node node, WeaponEffectNugget** objT, Tracker state)
    {
        if (node is null)
        {
            return;
        }
        uint typeId = 0u;
        Marshal(node.GetAttributeValue("TypeId", "0"), &typeId, Tracker.NullTracker);
        switch (typeId)
        {
            case 0xC0F53E99u:
                MarshalPolymorphicType<DamageNuggetType, WeaponEffectNugget>(node, objT, state);
                break;
            case 0x87C7BDCCu:
                MarshalEp1Nugget<Ep1ProjectileNuggetType>(node, objT, state, 0x87C7BDCCu); // Reborn: EP1 68-byte projectile record.
                break;
            case 0xF4A6ADA7u:
                MarshalEp1Nugget<Ep1SuppressionNuggetType>(node, objT, state, 0xF4A6ADA7u); // Reborn: EP1 60-byte suppression record.
                break;
            case 0xAF4289FBu:
                MarshalEp1Nugget<Ep1MetaImpactNuggetType>(node, objT, state, 0xAF4289FBu); // Reborn: EP1 96-byte meta-impact record.
                break;
            case var ep1 when ep1==BinaryAssetBuilder.Core.Hashing.FastHash.GetHashCode("DOTNuggetType"):
                // Reborn: EP1 concrete allocation preserves the official class discriminator.
                MarshalEp1Nugget<Ep1DOTNuggetType>(node,objT,state,ep1);break;
            case var ep1 when ep1==BinaryAssetBuilder.Core.Hashing.FastHash.GetHashCode("DamageAndSpawnNuggetType"):
                // Reborn: EP1 concrete allocation preserves the official class discriminator.
                MarshalEp1Nugget<Ep1DamageAndSpawnNuggetType>(node,objT,state,ep1);break;
            case var ep1 when ep1==BinaryAssetBuilder.Core.Hashing.FastHash.GetHashCode("TerrainCraterNuggetType"):
                // Reborn: EP1 concrete allocation preserves the official class discriminator.
                MarshalEp1Nugget<Ep1TerrainCraterNuggetType>(node,objT,state,ep1);break;
            case var ep1 when ep1==BinaryAssetBuilder.Core.Hashing.FastHash.GetHashCode("WeaponOCLNuggetType"):
                // Reborn: EP1 concrete allocation preserves the official class discriminator.
                MarshalEp1Nugget<Ep1WeaponOCLNuggetType>(node,objT,state,ep1);break;
            case var ep1 when ep1==BinaryAssetBuilder.Core.Hashing.FastHash.GetHashCode("ActivateLaserNuggetType"):
                // Reborn: EP1 concrete allocation preserves the official class discriminator.
                MarshalEp1Nugget<Ep1ActivateLaserNuggetType>(node,objT,state,ep1);break;
            case var ep1 when ep1==BinaryAssetBuilder.Core.Hashing.FastHash.GetHashCode("ActivateStreamNuggetType"):
                // Reborn: EP1 concrete allocation preserves the official class discriminator.
                MarshalEp1Nugget<Ep1ActivateStreamNuggetType>(node,objT,state,ep1);break;
            case var ep1 when ep1==BinaryAssetBuilder.Core.Hashing.FastHash.GetHashCode("ActivateLiftObjectNuggetType"):
                // Reborn: EP1 concrete allocation preserves the official class discriminator.
                MarshalEp1Nugget<Ep1ActivateLiftObjectNuggetType>(node,objT,state,ep1);break;
            case var ep1 when ep1==BinaryAssetBuilder.Core.Hashing.FastHash.GetHashCode("ActivateLinearDamageNuggetType"):
                // Reborn: EP1 concrete allocation preserves the official class discriminator.
                MarshalEp1Nugget<Ep1ActivateLinearDamageNuggetType>(node,objT,state,ep1);break;
            case var ep1 when ep1==BinaryAssetBuilder.Core.Hashing.FastHash.GetHashCode("ActivateCircularDamageNuggetType"):
                // Reborn: EP1 concrete allocation preserves the official class discriminator.
                MarshalEp1Nugget<Ep1ActivateCircularDamageNuggetType>(node,objT,state,ep1);break;
            case var ep1 when ep1==BinaryAssetBuilder.Core.Hashing.FastHash.GetHashCode("SlavesAttackNuggetType"):
                // Reborn: EP1 concrete allocation preserves the official class discriminator.
                MarshalEp1Nugget<Ep1SlavesAttackNuggetType>(node,objT,state,ep1);break;
            case var ep1 when ep1==BinaryAssetBuilder.Core.Hashing.FastHash.GetHashCode("LeechHealthDamageNuggetType"):
                // Reborn: EP1 concrete allocation preserves the official class discriminator.
                MarshalEp1Nugget<Ep1LeechHealthDamageNuggetType>(node,objT,state,ep1);break;
            case var ep1 when ep1==BinaryAssetBuilder.Core.Hashing.FastHash.GetHashCode("SecondaryDamageNuggetType"):
                // Reborn: EP1 concrete allocation preserves the official class discriminator.
                MarshalEp1Nugget<Ep1SecondaryDamageNuggetType>(node,objT,state,ep1);break;
            case var ep1 when ep1==BinaryAssetBuilder.Core.Hashing.FastHash.GetHashCode("SpecialPowerNuggetType"):
                // Reborn: EP1 concrete allocation preserves the official class discriminator.
                MarshalEp1Nugget<Ep1SpecialPowerNuggetType>(node,objT,state,ep1);break;
            case var ep1 when ep1==BinaryAssetBuilder.Core.Hashing.FastHash.GetHashCode("AttributeModifierNuggetType"):
                // Reborn: EP1 concrete allocation preserves the official class discriminator.
                MarshalEp1Nugget<Ep1AttributeModifierNuggetType>(node,objT,state,ep1);break;
            case var ep1 when ep1==BinaryAssetBuilder.Core.Hashing.FastHash.GetHashCode("DamageContainedNuggetType"):
                // Reborn: EP1 concrete allocation preserves the official class discriminator.
                MarshalEp1Nugget<Ep1DamageContainedNuggetType>(node,objT,state,ep1);break;
            case var ep1 when ep1==BinaryAssetBuilder.Core.Hashing.FastHash.GetHashCode("LuaEventNuggetType"):
                // Reborn: EP1 concrete allocation preserves the official class discriminator.
                MarshalEp1Nugget<Ep1LuaEventNuggetType>(node,objT,state,ep1);break;
            case var ep1 when ep1==BinaryAssetBuilder.Core.Hashing.FastHash.GetHashCode("LineDamageNuggetType"):
                // Reborn: EP1 concrete allocation preserves the official class discriminator.
                MarshalEp1Nugget<Ep1LineDamageNuggetType>(node,objT,state,ep1);break;
            case var ep1 when ep1==BinaryAssetBuilder.Core.Hashing.FastHash.GetHashCode("SeedTiberiumNuggetType"):
                // Reborn: EP1 concrete allocation preserves the official class discriminator.
                MarshalEp1Nugget<Ep1SeedTiberiumNuggetType>(node,objT,state,ep1);break;
            case var ep1 when ep1==BinaryAssetBuilder.Core.Hashing.FastHash.GetHashCode("ParalyzeNuggetType"):
                // Reborn: EP1 concrete allocation preserves the official class discriminator.
                MarshalEp1Nugget<Ep1ParalyzeNuggetType>(node,objT,state,ep1);break;
            case var ep1 when ep1==BinaryAssetBuilder.Core.Hashing.FastHash.GetHashCode("InformationWarfareNuggetType"):
                // Reborn: EP1 concrete allocation preserves the official class discriminator.
                MarshalEp1Nugget<Ep1InformationWarfareNuggetType>(node,objT,state,ep1);break;
            case var ep1 when ep1==BinaryAssetBuilder.Core.Hashing.FastHash.GetHashCode("SpendStolenTiberiumNuggetType"):
                // Reborn: EP1 concrete allocation preserves the official class discriminator.
                MarshalEp1Nugget<Ep1SpendStolenTiberiumNuggetType>(node,objT,state,ep1);break;
            case var ep1 when ep1==BinaryAssetBuilder.Core.Hashing.FastHash.GetHashCode("ReportWeaponFiredStatNuggetType"):
                // Reborn: EP1 concrete allocation preserves the official class discriminator.
                MarshalEp1Nugget<Ep1ReportWeaponFiredStatNuggetType>(node,objT,state,ep1);break;
            case var ep1 when ep1==BinaryAssetBuilder.Core.Hashing.FastHash.GetHashCode("ScatterProjectileNuggetType"):
                // Reborn: EP1 concrete allocation preserves the official class discriminator.
                MarshalEp1Nugget<Ep1ScatterProjectileNuggetType>(node,objT,state,ep1);break;
            case var ep1 when ep1==BinaryAssetBuilder.Core.Hashing.FastHash.GetHashCode("FireOnObjectsNuggetType"):
                // Reborn: EP1 concrete allocation preserves the official class discriminator.
                MarshalEp1Nugget<Ep1FireOnObjectsNuggetType>(node,objT,state,ep1);break;
            case var ep1 when ep1==BinaryAssetBuilder.Core.Hashing.FastHash.GetHashCode("TintObjectsNuggetType"):
                // Reborn: EP1 concrete allocation preserves the official class discriminator.
                MarshalEp1Nugget<Ep1TintObjectsNuggetType>(node,objT,state,ep1);break;
            case var ep1 when ep1==BinaryAssetBuilder.Core.Hashing.FastHash.GetHashCode("ContainedObjectAttackNuggetType"):
                // Reborn: EP1 concrete allocation preserves the official class discriminator.
                MarshalEp1Nugget<Ep1ContainedObjectAttackNuggetType>(node,objT,state,ep1);break;
            case var ep1 when ep1==BinaryAssetBuilder.Core.Hashing.FastHash.GetHashCode("InformTargetNuggetType"):
                // Reborn: EP1 concrete allocation preserves the official class discriminator.
                MarshalEp1Nugget<Ep1InformTargetNuggetType>(node,objT,state,ep1);break;
            case var ep1 when ep1==BinaryAssetBuilder.Core.Hashing.FastHash.GetHashCode("AttachNugget"):
                // Reborn: EP1 concrete allocation preserves the official class discriminator.
                MarshalEp1Nugget<Ep1AttachNugget>(node,objT,state,ep1);break;
            case var ep1 when ep1==BinaryAssetBuilder.Core.Hashing.FastHash.GetHashCode("StripMaxHealthPercentNuggetType"):
                // Reborn: EP1 concrete allocation preserves the official class discriminator.
                MarshalEp1Nugget<Ep1StripMaxHealthPercentNuggetType>(node,objT,state,ep1);break;
            case var ep1 when ep1==BinaryAssetBuilder.Core.Hashing.FastHash.GetHashCode("GrapplingHookNuggetType"):
                // Reborn: EP1 concrete allocation preserves the official class discriminator.
                MarshalEp1Nugget<Ep1GrapplingHookNuggetType>(node,objT,state,ep1);break;
            case var ep1 when ep1==BinaryAssetBuilder.Core.Hashing.FastHash.GetHashCode("LeechPercentMaxHealthDamageNuggetType"):
                // Reborn: EP1 concrete allocation preserves the official class discriminator.
                MarshalEp1Nugget<Ep1LeechPercentMaxHealthDamageNuggetType>(node,objT,state,ep1);break;
            default:
                Tracer.GetTracer("Marshaler", "Marshalling").TraceWarning("Marshalling unknown type {0} from {1}.", typeId, nameof(PolymorphicList<WeaponEffectNugget>));
                MarshalUnknownPolymorphicType(node, objT, state);
                break;

        }
    }

    public static unsafe void Marshal(Node node, WeaponAiHintInfo* objT, Tracker state)
    {
        if (node is null)
        {
            return;
        }
        Marshal(node.GetAttributeValue(nameof(WeaponAiHintInfo.MaxSpeedOfTarget), null), &objT->MaxSpeedOfTarget, state);
        Marshal(node.GetAttributeValue(nameof(WeaponAiHintInfo.UseAsWarheadForDamageCalculations), null), &objT->UseAsWarheadForDamageCalculations, state);
        Marshal(node.GetAttributeValue(nameof(WeaponAiHintInfo.IsAntiGarrisonWeapon), "false"), &objT->IsAntiGarrisonWeapon, state);
        Marshal(node.GetAttributeValue(nameof(WeaponAiHintInfo.UseLongLockOnTimeCode), "false"), &objT->UseLongLockOnTimeCode, state);
    }

    public static unsafe void Marshal(Node node, WeaponAiHintInfo** objT, Tracker state)
    {
        if (node is null)
        {
            return;
        }
        using Tracker.Context context = state.Push((void**)objT, (uint)sizeof(WeaponAiHintInfo), 1u);
        Marshal(node, *objT, state);
    }

    public static unsafe void Marshal(Node node, WeaponTemplate* objT, Tracker state)
    {
        if (node is null)
        {
            return;
        }
        Marshal(node.GetAttributeValue(nameof(WeaponTemplate.AttackRange), "0"), &objT->AttackRange, state);
        Marshal(node.GetAttributeValue(nameof(WeaponTemplate.MinimumAttackRange), "0"), &objT->MinimumAttackRange, state);
        Marshal(node.GetAttributeValue(nameof(WeaponTemplate.RangeBonusMinHeight), "0"), &objT->RangeBonusMinHeight, state);
        Marshal(node.GetAttributeValue(nameof(WeaponTemplate.RangeBonus), "0"), &objT->RangeBonus, state);
        Marshal(node.GetAttributeValue(nameof(WeaponTemplate.RangeBonusPerFoot), "0"), &objT->RangeBonusPerFoot, state);
        Marshal(node.GetAttributeValue(nameof(WeaponTemplate.RequestAssistRange), "0"), &objT->RequestAssistRange, state);
        Marshal(node.GetAttributeValue(nameof(WeaponTemplate.AcceptableAimDelta), "0d"), &objT->AcceptableAimDelta, state);
        Marshal(node.GetAttributeValue(nameof(WeaponTemplate.AimDirection), "0d"), &objT->AimDirection, state);
        Marshal(node.GetAttributeValue(nameof(WeaponTemplate.ScatterRadius), "0"), &objT->ScatterRadius, state);
        Marshal(node.GetAttributeValue(nameof(WeaponTemplate.ScatterLength), "0"), &objT->ScatterLength, state);
        Marshal(node.GetAttributeValue(nameof(WeaponTemplate.ScatterIndependently), "false"), &objT->ScatterIndependently, state);
        Marshal(node.GetAttributeValue(nameof(WeaponTemplate.ScatterAlways), "false"), &objT->ScatterAlways, state);
        Marshal(node.GetAttributeValue(nameof(WeaponTemplate.WeaponSpeed), "99999"), &objT->WeaponSpeed, state);
        Marshal(node.GetAttributeValue(nameof(WeaponTemplate.MinWeaponSpeed), "99999"), &objT->MinWeaponSpeed, state);
        Marshal(node.GetAttributeValue(nameof(WeaponTemplate.MaxWeaponSpeed), "99999"), &objT->MaxWeaponSpeed, state);
        Marshal(node.GetAttributeValue(nameof(WeaponTemplate.ScaleWeaponSpeed), "false"), &objT->ScaleWeaponSpeed, state);
        Marshal(node.GetAttributeValue(nameof(WeaponTemplate.IgnoresContactPoints), "false"), &objT->IgnoresContactPoints, state);
        Marshal(node.GetAttributeValue(nameof(WeaponTemplate.ScaleAttackRangeByAmmoRemaining), "false"), &objT->ScaleAttackRangeByAmmoRemaining, state);
        Marshal(node.GetAttributeValue(nameof(WeaponTemplate.CanBeDodged), "false"), &objT->CanBeDodged, state);
        Marshal(node.GetAttributeValue(nameof(WeaponTemplate.IdleAfterFiringDelaySeconds), "-1s"), &objT->IdleAfterFiringDelaySeconds, state);
        Marshal(node.GetAttributeValue(nameof(WeaponTemplate.HoldAfterFiringDelaySeconds), "0s"), &objT->HoldAfterFiringDelaySeconds, state);
        Marshal(node.GetAttributeValue(nameof(WeaponTemplate.HoldDuringReload), "false"), &objT->HoldDuringReload, state);
        Marshal(node.GetAttributeValue(nameof(WeaponTemplate.CanFireWhileMoving), "false"), &objT->CanFireWhileMoving, state);
        Marshal(node.GetAttributeValue(nameof(WeaponTemplate.WeaponRecoil), "0d"), &objT->WeaponRecoil, state);
        Marshal(node.GetAttributeValue(nameof(WeaponTemplate.MinTargetPitch), "-180d"), &objT->MinTargetPitch, state);
        Marshal(node.GetAttributeValue(nameof(WeaponTemplate.MaxTargetPitch), "180d"), &objT->MaxTargetPitch, state);
        Marshal(node.GetAttributeValue(nameof(WeaponTemplate.PreferredTargetBone), null), &objT->PreferredTargetBone, state);
        Marshal(node.GetAttributeValue(nameof(WeaponTemplate.FireSound), null), &objT->FireSound, state);
        Marshal(node.GetAttributeValue(nameof(WeaponTemplate.FireSoundPerClip), null), &objT->FireSoundPerClip, state);
        Marshal(node.GetAttributeValue(nameof(WeaponTemplate.FiringLoopSound), null), &objT->FiringLoopSound, state);
        Marshal(node.GetAttributeValue(nameof(WeaponTemplate.ImpactLoopSound), null), &objT->ImpactLoopSound, state);
        Marshal(node.GetAttributeValue(nameof(WeaponTemplate.RetargetedWhileLoopingSound), null), &objT->RetargetedWhileLoopingSound, state);
        Marshal(node.GetAttributeValue(nameof(WeaponTemplate.FiringLoopSoundContinuesDuringReload), "true"), &objT->FiringLoopSoundContinuesDuringReload, state);
        Marshal(node.GetAttributeValue(nameof(WeaponTemplate.FireFX), null), &objT->FireFX, state);
        Marshal(node.GetAttributeValue(nameof(WeaponTemplate.FireVeteranFX), null), &objT->FireVeteranFX, state);
        Marshal(node.GetAttributeValue(nameof(WeaponTemplate.FireFlankFX), null), &objT->FireFlankFX, state);
        Marshal(node.GetAttributeValue(nameof(WeaponTemplate.PreAttackFX), null), &objT->PreAttackFX, state);
        Marshal(node.GetAttributeValue(nameof(WeaponTemplate.ClipSize), "0"), &objT->ClipSize, state);
        Marshal(node.GetAttributeValue(nameof(WeaponTemplate.ContinuousFireOne), "99999"), &objT->ContinuousFireOne, state);
        Marshal(node.GetAttributeValue(nameof(WeaponTemplate.ContinuousFireTwo), "99999"), &objT->ContinuousFireTwo, state);
        Marshal(node.GetAttributeValue(nameof(WeaponTemplate.ContinuousFireCoastSeconds), "0s"), &objT->ContinuousFireCoastSeconds, state);
        Marshal(node.GetAttributeValue(nameof(WeaponTemplate.AutoReloadWhenIdleSeconds), "0s"), &objT->AutoReloadWhenIdleSeconds, state);
        Marshal(node.GetAttributeValue(nameof(WeaponTemplate.ShotsPerBarrel), "0"), &objT->ShotsPerBarrel, state);
        Marshal(node.GetAttributeValue(nameof(WeaponTemplate.DamageDealtAtSelfPosition), "false"), &objT->DamageDealtAtSelfPosition, state);
        Marshal(node.GetAttributeValue(nameof(WeaponTemplate.RequiredFiringObjectStatus), null), &objT->RequiredFiringObjectStatus, state);
        Marshal(node.GetAttributeValue(nameof(WeaponTemplate.ForbiddenFiringObjectStatus), null), &objT->ForbiddenFiringObjectStatus, state);
        Marshal(node.GetAttributeValue(nameof(WeaponTemplate.CheckStatusFlagsInRangeChecks), "true"), &objT->CheckStatusFlagsInRangeChecks, state);
        Marshal(node.GetAttributeValue(nameof(WeaponTemplate.ProjectileSelf), "false"), &objT->ProjectileSelf, state);
        Marshal(node.GetAttributeValue(nameof(WeaponTemplate.ProjectileSelfUsesPathfinder), "true"), &objT->ProjectileSelfUsesPathfinder, state);
        Marshal(node.GetAttributeValue(nameof(WeaponTemplate.MeleeWeapon), "false"), &objT->MeleeWeapon, state);
        Marshal(node.GetAttributeValue(nameof(WeaponTemplate.ChaseWeapon), "false"), &objT->ChaseWeapon, state);
        Marshal(node.GetAttributeValue(nameof(WeaponTemplate.LeechRangeWeapon), "false"), &objT->LeechRangeWeapon, state);
        Marshal(node.GetAttributeValue(nameof(WeaponTemplate.HitStoredTarget), "false"), &objT->HitStoredTarget, state);
        Marshal(node.GetAttributeValue(nameof(WeaponTemplate.CapableOfFollowingWaypoints), "false"), &objT->CapableOfFollowingWaypoints, state);
        Marshal(node.GetAttributeValue(nameof(WeaponTemplate.ShowsAmmoPips), "false"), &objT->ShowsAmmoPips, state);
        Marshal(node.GetAttributeValue(nameof(WeaponTemplate.AllowAttackGarrisonedBldgs), "false"), &objT->AllowAttackGarrisonedBldgs, state);
        Marshal(node.GetAttributeValue(nameof(WeaponTemplate.PlayFXWhenStealthed), "false"), &objT->PlayFXWhenStealthed, state);
        Marshal(node.GetAttributeValue(nameof(WeaponTemplate.ContinueAttackRange), "0"), &objT->ContinueAttackRange, state);
        Marshal(node.GetAttributeValue(nameof(WeaponTemplate.SuspendFXDelaySeconds), "0s"), &objT->SuspendFXDelaySeconds, state);
        Marshal(node.GetAttributeValue(nameof(WeaponTemplate.IsAimingWeapon), "false"), &objT->IsAimingWeapon, state);
        Marshal(node.GetAttributeValue(nameof(WeaponTemplate.NoVictimNeeded), "false"), &objT->NoVictimNeeded, state);
        Marshal(node.GetAttributeValue(nameof(WeaponTemplate.PassengerProportionalAttack), "false"), &objT->PassengerProportionalAttack, state);
        Marshal(node.GetAttributeValue(nameof(WeaponTemplate.HitPercentage), "100%"), &objT->HitPercentage, state);
        Marshal(node.GetAttributeValue(nameof(WeaponTemplate.HitPassengerPercentage), "100%"), &objT->HitPassengerPercentage, state);
        Marshal(node.GetAttributeValue(nameof(WeaponTemplate.HealthProportionalResolution), "0"), &objT->HealthProportionalResolution, state);
        Marshal(node.GetAttributeValue(nameof(WeaponTemplate.MaxAttackPassengers), null), &objT->MaxAttackPassengers, state);
        // Reborn: EP1 WeaponTemplate schema defaults this flag to false; true changes unchanged source weapons.
        Marshal(node.GetAttributeValue(nameof(WeaponTemplate.FinishAttackOnceStarted), "false"), &objT->FinishAttackOnceStarted, state);
        Marshal(node.GetAttributeValue(nameof(WeaponTemplate.RestrictedHeightRange), "0"), &objT->RestrictedHeightRange, state);
        Marshal(node.GetAttributeValue(nameof(WeaponTemplate.CannotTargetCastleVictims), "false"), &objT->CannotTargetCastleVictims, state);
        Marshal(node.GetAttributeValue(nameof(WeaponTemplate.RequireFollowThru), "false"), &objT->RequireFollowThru, state);
        Marshal(node.GetAttributeValue(nameof(WeaponTemplate.ShareTimers), "false"), &objT->ShareTimers, state);
        Marshal(node.GetAttributeValue(nameof(WeaponTemplate.ShouldPlayUnderAttackEvaEvent), "true"), &objT->ShouldPlayUnderAttackEvaEvent, state);
        Marshal(node.GetAttributeValue(nameof(WeaponTemplate.InstantLoadClipOnActivate), "false"), &objT->InstantLoadClipOnActivate, state);
        Marshal(node.GetAttributeValue(nameof(WeaponTemplate.Flags), "NONE"), &objT->Flags, state);
        Marshal(node.GetAttributeValue(nameof(WeaponTemplate.LockWhenUsing), "false"), &objT->LockWhenUsing, state);
        Marshal(node.GetAttributeValue(nameof(WeaponTemplate.BombardType), "false"), &objT->BombardType, state);
        Marshal(node.GetAttributeValue(nameof(WeaponTemplate.UseInnateAttributes), "false"), &objT->UseInnateAttributes, state);
        Marshal(node.GetAttributeValue(nameof(WeaponTemplate.PreAttackType), nameof(WeaponPrefireType.PER_SHOT)), &objT->PreAttackType, state);
        Marshal(node.GetAttributeValue(nameof(WeaponTemplate.ReAcquireDetailType), nameof(WeaponReAcquireDetailType.PER_SHOT)), &objT->ReAcquireDetailType, state);
        Marshal(node.GetAttributeValue(nameof(WeaponTemplate.AutoReloadsClip), nameof(WeaponReloadType.AUTO)), &objT->AutoReloadsClip, state);
        Marshal(node.GetAttributeValue(nameof(WeaponTemplate.SingleAmmoReloadedNotFullSound), ""), &objT->SingleAmmoReloadedNotFullSound, state);
        Marshal(node.GetAttributeValue(nameof(WeaponTemplate.ClipReloadedSound), ""), &objT->ClipReloadedSound, state);
        Marshal(node.GetAttributeValue(nameof(WeaponTemplate.RadiusDamageAffects),
                                       $"{nameof(WeaponAffectsType.ALLIES)} {nameof(WeaponAffectsType.ENEMIES)} {nameof(WeaponAffectsType.NEUTRALS)}"),
                &objT->RadiusDamageAffects,
                state);
        Marshal(node.GetAttributeValue(nameof(WeaponTemplate.FXTrigger), nameof(FXTriggerType.NONE)), &objT->FXTrigger, state);
        Marshal(node.GetAttributeValue(nameof(WeaponTemplate.ProjectileCollidesWith), nameof(WeaponCollideType.STRUCTURES)), &objT->ProjectileCollidesWith, state);
        Marshal(node.GetAttributeValue(nameof(WeaponTemplate.RequiredAntiMask), $"{nameof(WpnAntiT.ANTI_GROUND)} {nameof(WpnAntiT.ANTI_WATER)} {nameof(WpnAntiT.ANTI_STRUCTURE)}"), &objT->RequiredAntiMask, state);
        Marshal(node.GetAttributeValue(nameof(WeaponTemplate.ForbiddenAntiMask), ""), &objT->ForbiddenAntiMask, state);
        Marshal(node.GetAttributeValue(nameof(WeaponTemplate.StopFiringOnCanBeInvisible), "false"), &objT->StopFiringOnCanBeInvisible, state);
        Marshal(node.GetAttributeValue(nameof(WeaponTemplate.ProjectileStreamName), null), &objT->ProjectileStreamName, state);
        Marshal(node.GetAttributeValue(nameof(WeaponTemplate.ContactWeapon), "false"), &objT->ContactWeapon, state);
        Marshal(node.GetAttributeValue(nameof(WeaponTemplate.UseCenterForRangeCheck), "false"), &objT->UseCenterForRangeCheck, state);
        Marshal(node.GetAttributeValue(nameof(WeaponTemplate.VirtualDamage), nameof(VirtualDamageType.NONE)), &objT->VirtualDamage, state);
        Marshal(node.GetAttributeValue(nameof(WeaponTemplate.PreAttackWeapon), null), &objT->PreAttackWeapon, state);
        Marshal(node.GetAttributeValue(nameof(WeaponTemplate.RevealShroudOnFire), "false"), &objT->RevealShroudOnFire, state);
        Marshal(node.GetAttributeValue(nameof(WeaponTemplate.ShouldPlayTargetDeadEvaEvent), "true"), &objT->ShouldPlayTargetDeadEvaEvent, state);
        Marshal(node.GetAttributeValue(nameof(WeaponTemplate.UpdateBarrelModelConditions), "false"), &objT->UpdateBarrelModelConditions, state);
        Marshal(node.GetChildNode(nameof(WeaponTemplate.OverrideVoiceAttackSound), null), &objT->OverrideVoiceAttackSound, state);
        Marshal(node.GetChildNode(nameof(WeaponTemplate.OverrideVoiceEnterStateAttackSound), null), &objT->OverrideVoiceEnterStateAttackSound, state);
        Marshal(node.GetChildNode(nameof(WeaponTemplate.PreAttackDelay), null), &objT->PreAttackDelay, state);
        Marshal(node.GetChildNode(nameof(WeaponTemplate.FiringDuration), null), &objT->FiringDuration, state);
        Marshal(node.GetChildNode(nameof(WeaponTemplate.CoolDownDelayBetweenShots), null), &objT->CoolDownDelayBetweenShots, state);
        Marshal(node.GetChildNode(nameof(WeaponTemplate.ClipReloadTime), null), &objT->ClipReloadTime, state);
        Marshal(node.GetChildNode(nameof(WeaponTemplate.WeaponAiHintInfo), null), &objT->WeaponAiHintInfo, state);
        Marshal(node.GetChildNode(nameof(WeaponTemplate.Nuggets), null), &objT->Nuggets, state);
        Marshal(node.GetChildNode(nameof(WeaponTemplate.SurpriseAttackObjectFilter), null), &objT->SurpriseAttackObjectFilter, state);
        Marshal(node.GetChildNode(nameof(WeaponTemplate.CombinedAttackObjectFilter), null), &objT->CombinedAttackObjectFilter, state);
        Marshal(node.GetChildNode(nameof(WeaponTemplate.HitStoredObjectFilter), null), &objT->HitStoredObjectFilter, state);
        Marshal(node.GetChildNodes(nameof(WeaponTemplate.ScatterRadiusVsType)), &objT->ScatterRadiusVsType, state);
        Marshal(node.GetChildNodes(nameof(WeaponTemplate.IncompatibleAttributeModifier)), &objT->IncompatibleAttributeModifier, state);
        Marshal(node, (BaseInheritableAsset*)objT, state);
    }
}
