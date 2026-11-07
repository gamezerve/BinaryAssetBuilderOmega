using Relo;
using SageBinaryData;
public static partial class Marshaler
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: encode the original zero-based EP1 micro-manager heuristic selector. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Value value,Ep1AISpecialPowerHeuristic* target,Tracker state)
    { if(value is null)return;Ep1ToggleEnum(value,(uint*)target,state,typeof(Ep1AISpecialPowerHeuristic),false); }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: compile every authored TargetBiasInfo field using the original EP1 native ordering and defaults. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1AITargetBiasInfo* target,Tracker state)
    {
        if(node is null)return;
        Marshal(node.GetAttributeValue(nameof(Ep1AITargetBiasInfo.UnitName),null),&target->UnitName,state);
        Marshal(node.GetAttributeValue(nameof(Ep1AITargetBiasInfo.HealthMultiplier),"1.0"),&target->HealthMultiplier,state);
        Marshal(node.GetAttributeValue(nameof(Ep1AITargetBiasInfo.DamageMultiplier),"1.0"),&target->DamageMultiplier,state);
        Marshal(node.GetAttributeValue(nameof(Ep1AITargetBiasInfo.HealthOverride),"100.0"),&target->HealthOverride,state);
        Marshal(node.GetAttributeValue(nameof(Ep1AITargetBiasInfo.DamageOverride),"100.0"),&target->DamageOverride,state);
    }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: compile every authored SpecificSpecialPowerDelayInfo field using the original EP1 native ordering and defaults. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1AISpecificSpecialPowerDelayInfo* target,Tracker state)
    {
        if(node is null)return;
        Marshal(node.GetAttributeValue(nameof(Ep1AISpecificSpecialPowerDelayInfo.PowerType),null),&target->PowerType,state);
        Ep1ToggleEnum(node.GetAttributeValue(nameof(Ep1AISpecificSpecialPowerDelayInfo.TogglePowerSubType),"TOGGLE_NO_HINT"),(uint*)&target->TogglePowerSubType,state,typeof(Ep1ToggleStatusAiHint),false);
        Marshal(node.GetAttributeValue(nameof(Ep1AISpecificSpecialPowerDelayInfo.MinimumDelay),"0s"),&target->MinimumDelay,state);
        Marshal(node.GetAttributeValue(nameof(Ep1AISpecificSpecialPowerDelayInfo.MaximumDelay),"0s"),&target->MaximumDelay,state);
    }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: compile every authored AIMicroManagerData field using the original EP1 native ordering and defaults. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1AIMicroManagerData* target,Tracker state)
    {
        if(node is null)return;
        Marshal(node.GetAttributeValue(nameof(Ep1AIMicroManagerData.UpdateTime),"1s"),&target->UpdateTime,state);
        Marshal(node.GetAttributeValue(nameof(Ep1AIMicroManagerData.MaxTeamIdleTime),"30s"),&target->MaxTeamIdleTime,state);
        Marshal(node.GetAttributeValue(nameof(Ep1AIMicroManagerData.MaxTeamIdleDistanceChange),"50.0"),&target->MaxTeamIdleDistanceChange,state);
        Marshal(node.GetAttributeValue(nameof(Ep1AIMicroManagerData.MaxUnitIdleTime),"10s"),&target->MaxUnitIdleTime,state);
        Marshal(node.GetAttributeValue(nameof(Ep1AIMicroManagerData.MaxUnitIdleDistanceChange),"50.0"),&target->MaxUnitIdleDistanceChange,state);
        Marshal(node.GetAttributeValue(nameof(Ep1AIMicroManagerData.EngageHostileTargets),"true"),&target->EngageHostileTargets,state);
        Marshal(node.GetAttributeValue(nameof(Ep1AIMicroManagerData.EngagePassiveTargets),"true"),&target->EngagePassiveTargets,state);
        Marshal(node.GetAttributeValue(nameof(Ep1AIMicroManagerData.EngageTacticalTarget),"true"),&target->EngageTacticalTarget,state);
        Marshal(node.GetAttributeValue(nameof(Ep1AIMicroManagerData.PassiveTargetSearchDistance),"400.0"),&target->PassiveTargetSearchDistance,state);
        Marshal(node.GetAttributeValue(nameof(Ep1AIMicroManagerData.EnemySearchDistanceVisionMultiplier),"1.25"),&target->EnemySearchDistanceVisionMultiplier,state);
        Marshal(node.GetAttributeValue(nameof(Ep1AIMicroManagerData.MinTimeToKeepTempVisionRangeBonus),"10s"),&target->MinTimeToKeepTempVisionRangeBonus,state);
        Marshal(node.GetAttributeValue(nameof(Ep1AIMicroManagerData.TimeToKeepTargetMin),"0s"),&target->TimeToKeepTargetMin,state);
        Marshal(node.GetAttributeValue(nameof(Ep1AIMicroManagerData.TimeToKeepTargetMax),"0s"),&target->TimeToKeepTargetMax,state);
        Marshal(node.GetAttributeValue(nameof(Ep1AIMicroManagerData.RandomTargetScoreModifier),"0.0"),&target->RandomTargetScoreModifier,state);
        Marshal(node.GetAttributeValue(nameof(Ep1AIMicroManagerData.CleanupPowerAdvantage),"4.0"),&target->CleanupPowerAdvantage,state);
        Marshal(node.GetAttributeValue(nameof(Ep1AIMicroManagerData.UseGatherState),"true"),&target->UseGatherState,state);
        Marshal(node.GetAttributeValue(nameof(Ep1AIMicroManagerData.GatherTempVisionRangeBonus),"1.5"),&target->GatherTempVisionRangeBonus,state);
        Marshal(node.GetAttributeValue(nameof(Ep1AIMicroManagerData.RetreatPowerAdvantage),"0.25"),&target->RetreatPowerAdvantage,state);
        Marshal(node.GetAttributeValue(nameof(Ep1AIMicroManagerData.RetreatMinDistanceFromBase),"900.0"),&target->RetreatMinDistanceFromBase,state);
        Marshal(node.GetAttributeValue(nameof(Ep1AIMicroManagerData.RetreatMinDistanceToFallback),"500.0"),&target->RetreatMinDistanceToFallback,state);
        Marshal(node.GetAttributeValue(nameof(Ep1AIMicroManagerData.RetreatSpeedBuffer),"0.75"),&target->RetreatSpeedBuffer,state);
        Marshal(node.GetAttributeValue(nameof(Ep1AIMicroManagerData.RetreatTempVisionRangeBonus),"1.5"),&target->RetreatTempVisionRangeBonus,state);
        Marshal(node.GetAttributeValue(nameof(Ep1AIMicroManagerData.MaxDistanceFromTeamToIncludeInArmy),"600.0"),&target->MaxDistanceFromTeamToIncludeInArmy,state);
        Marshal(node.GetAttributeValue(nameof(Ep1AIMicroManagerData.AlphaStrikeTargetHealthPct),"0.5"),&target->AlphaStrikeTargetHealthPct,state);
        Marshal(node.GetAttributeValue(nameof(Ep1AIMicroManagerData.AlphaStrikeTargetTimeToKill),"4.0"),&target->AlphaStrikeTargetTimeToKill,state);
        Marshal(node.GetAttributeValue(nameof(Ep1AIMicroManagerData.AlphaStrikeEngineerDistanceFromStructure),"200.0"),&target->AlphaStrikeEngineerDistanceFromStructure,state);
        Marshal(node.GetAttributeValue(nameof(Ep1AIMicroManagerData.SiegeWeaponBuffer),"1.15"),&target->SiegeWeaponBuffer,state);
        Marshal(node.GetAttributeValue(nameof(Ep1AIMicroManagerData.CanUseSiegeState),"true"),&target->CanUseSiegeState,state);
        Marshal(node.GetAttributeValue(nameof(Ep1AIMicroManagerData.Crush),"true"),&target->Crush,state);
        Marshal(node.GetAttributeValue(nameof(Ep1AIMicroManagerData.CrushPreference),"1.0"),&target->CrushPreference,state);
        Marshal(node.GetAttributeValue(nameof(Ep1AIMicroManagerData.EnterGarrisons),"true"),&target->EnterGarrisons,state);
        Marshal(node.GetAttributeValue(nameof(Ep1AIMicroManagerData.EnterGarrisonableStructureDistance),"150.0"),&target->EnterGarrisonableStructureDistance,state);
        Marshal(node.GetAttributeValue(nameof(Ep1AIMicroManagerData.ClearGarrisons),"true"),&target->ClearGarrisons,state);
        Marshal(node.GetAttributeValue(nameof(Ep1AIMicroManagerData.PullbackInjured),"true"),&target->PullbackInjured,state);
        Marshal(node.GetAttributeValue(nameof(Ep1AIMicroManagerData.PullbackInjuredHealthPct),"0.2"),&target->PullbackInjuredHealthPct,state);
        Marshal(node.GetAttributeValue(nameof(Ep1AIMicroManagerData.ReverseMove),"true"),&target->ReverseMove,state);
        Marshal(node.GetAttributeValue(nameof(Ep1AIMicroManagerData.DodgeThreats),"true"),&target->DodgeThreats,state);
        Marshal(node.GetAttributeValue(nameof(Ep1AIMicroManagerData.DodgeThreatsReactionTimeMin),"0s"),&target->DodgeThreatsReactionTimeMin,state);
        Marshal(node.GetAttributeValue(nameof(Ep1AIMicroManagerData.DodgeThreatsReactionTimeMax),"0s"),&target->DodgeThreatsReactionTimeMax,state);
        Marshal(node.GetAttributeValue(nameof(Ep1AIMicroManagerData.UseSpecialPowers),"true"),&target->UseSpecialPowers,state);
        Marshal(node.GetAttributeValue(nameof(Ep1AIMicroManagerData.SpecialPowerDelayMinimum),"0s"),&target->SpecialPowerDelayMinimum,state);
        Marshal(node.GetAttributeValue(nameof(Ep1AIMicroManagerData.SpecialPowerDelayMaximum),"0s"),&target->SpecialPowerDelayMaximum,state);
        Marshal(node.GetAttributeValue(nameof(Ep1AIMicroManagerData.EnterBaseDefenses),"true"),&target->EnterBaseDefenses,state);
        Marshal(node.GetAttributeValue(nameof(Ep1AIMicroManagerData.EnterBaseDefensesWhenIdle),"false"),&target->EnterBaseDefensesWhenIdle,state);
        Marshal(node.GetAttributeValue(nameof(Ep1AIMicroManagerData.IsHarvesterManager),"false"),&target->IsHarvesterManager,state);
        Marshal(node.GetAttributeValue(nameof(Ep1AIMicroManagerData.MinDistanceFromBaseToIgnoreTargets),"0.0"),&target->MinDistanceFromBaseToIgnoreTargets,state);
        Marshal(node.GetAttributeValue(nameof(Ep1AIMicroManagerData.MaxSecondaryDamageFraction),"1.25"),&target->MaxSecondaryDamageFraction,state);
        Marshal(node.GetAttributeValue(nameof(Ep1AIMicroManagerData.CanCollectCratesWhenIdle),"true"),&target->CanCollectCratesWhenIdle,state);
        Marshal(node.GetAttributeValue(nameof(Ep1AIMicroManagerData.FractionBlockedToDestroyWalls),"0.5"),&target->FractionBlockedToDestroyWalls,state);
        Marshal(node.GetChildNodes(nameof(Ep1AIMicroManagerData.TargetBias)),&target->TargetBias,state);
        Marshal(node.GetChildNodes(nameof(Ep1AIMicroManagerData.SpecificSpecialPowerDelay)),&target->SpecificSpecialPowerDelay,state);
        Marshal(node.GetChildNode(nameof(Ep1AIMicroManagerData.RetreatBaseCriteria),null),&target->RetreatBaseCriteria,state);
        Marshal(node.GetChildNode(nameof(Ep1AIMicroManagerData.IgnoreTargets),null),&target->IgnoreTargets,state);
        Marshal(node,(BaseAssetType*)target,state);
    }
}
