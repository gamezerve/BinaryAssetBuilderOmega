using Relo;
using SageBinaryData;

public static partial class Marshaler
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: serialize every EP1 AI power attribute, optional scalar/status pointer, filter graph and weapon graph using official schema defaults. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1AiSpecialPowerInfo* value,Tracker state)
    {
        if(node is null)return;
        Ep1ToggleEnum(node.GetAttributeValue(nameof(Ep1AiSpecialPowerInfo.Heuristic), null), &value->Heuristic, state, typeof(Ep1AiSpecialPowerHeuristic), false);
        Ep1ToggleEnum(node.GetAttributeValue(nameof(Ep1AiSpecialPowerInfo.Manager), null), &value->Manager, state, typeof(Ep1AiSpecialPowerManager), false);
        Marshal(node.GetAttributeValue(nameof(Ep1AiSpecialPowerInfo.Distance), "100.0"), &value->Distance, state);
        Marshal(node.GetAttributeValue(nameof(Ep1AiSpecialPowerInfo.HealthFraction), "0.0"), &value->HealthFraction, state);
        Marshal(node.GetAttributeValue(nameof(Ep1AiSpecialPowerInfo.TargetObjectInclude), null), &value->TargetObjectInclude, state);
        Marshal(node.GetAttributeValue(nameof(Ep1AiSpecialPowerInfo.TargetObjectExclude), null), &value->TargetObjectExclude, state);
        Marshal(node.GetAttributeValue(nameof(Ep1AiSpecialPowerInfo.UseOnlyWhenPowerAdvantageIsAbove), null), &value->UseOnlyWhenPowerAdvantageIsAbove, state);
        Marshal(node.GetAttributeValue(nameof(Ep1AiSpecialPowerInfo.InvulnerabilityDamageLookAheadTime), "1s"), &value->InvulnerabilityDamageLookAheadTime, state);
        Marshal(node.GetAttributeValue(nameof(Ep1AiSpecialPowerInfo.InvulnerabilityDamageLookBehindTime), "1s"), &value->InvulnerabilityDamageLookBehindTime, state);
        Marshal(node.GetAttributeValue(nameof(Ep1AiSpecialPowerInfo.InvulnerabilityDamageFractionWhenOutOfRangeOfTarget), ".3"), &value->InvulnerabilityDamageFractionWhenOutOfRangeOfTarget, state);
        Marshal(node.GetAttributeValue(nameof(Ep1AiSpecialPowerInfo.InvulnerabilityDamageFractionWhenInRangeOfTarget), ".6"), &value->InvulnerabilityDamageFractionWhenInRangeOfTarget, state);
        Marshal(node.GetAttributeValue(nameof(Ep1AiSpecialPowerInfo.InvulnerabilityAvoidUseIfAngleToNextPathNodeIsMoreThan), "360d"), &value->InvulnerabilityAvoidUseIfAngleToNextPathNodeIsMoreThan, state);
        Marshal(node.GetAttributeValue(nameof(Ep1AiSpecialPowerInfo.MaxTargetSpeed), "99999.0"), &value->MaxTargetSpeed, state);
        Marshal(node.GetAttributeValue(nameof(Ep1AiSpecialPowerInfo.MinKillValue), "0.0"), &value->MinKillValue, state);
        Marshal(node.GetAttributeValue(nameof(Ep1AiSpecialPowerInfo.MinTargetsHit), "1"), &value->MinTargetsHit, state);
        Marshal(node.GetAttributeValue(nameof(Ep1AiSpecialPowerInfo.MinEnemiesNearby), "1"), &value->MinEnemiesNearby, state);
        Marshal(node.GetAttributeValue(nameof(Ep1AiSpecialPowerInfo.NextLogicCommand), null), &value->NextLogicCommand, state);
        Ep1ToggleEnum(node.GetAttributeValue(nameof(Ep1AiSpecialPowerInfo.UseInMicroManagerState), ""), &value->UseInMicroManagerState, state, typeof(Ep1MicroManagerState), true);
        Marshal(node.GetAttributeValue(nameof(Ep1AiSpecialPowerInfo.UpdateObject), null), &value->UpdateObject, state);
        Marshal(node.GetAttributeValue(nameof(Ep1AiSpecialPowerInfo.SearchRadius), "150.0"), &value->SearchRadius, state);
        Ep1ToggleEnum(node.GetAttributeValue(nameof(Ep1AiSpecialPowerInfo.Location), "Water Land"), &value->Location, state, typeof(Ep1AiTargetLocation), true);
        Marshal(node.GetAttributeValue(nameof(Ep1AiSpecialPowerInfo.MaxUpdatables), null), &value->MaxUpdatables, state);
        var counter=node.GetAttributeValue("UseToCounterGarrison",null);
        if(counter!=null){using Tracker.Context allocation=state.Push((void**)&value->UseToCounterGarrison,(uint)sizeof(SageBool),1);Marshal(counter,value->UseToCounterGarrison,state);}
        Marshal(node.GetAttributeValue(nameof(Ep1AiSpecialPowerInfo.RequiredForAttackObjectStatus), null), &value->RequiredForAttackObjectStatus, state);
        Marshal(node.GetAttributeValue(nameof(Ep1AiSpecialPowerInfo.ForbiddenForAttackObjectStatus), null), &value->ForbiddenForAttackObjectStatus, state);
        Marshal(node.GetAttributeValue(nameof(Ep1AiSpecialPowerInfo.NeverUseWhileRetreating), "false"), &value->NeverUseWhileRetreating, state);
        Marshal(node.GetAttributeValue(nameof(Ep1AiSpecialPowerInfo.NeverUseWhilePullingBack), "false"), &value->NeverUseWhilePullingBack, state);
        Marshal(node.GetAttributeValue(nameof(Ep1AiSpecialPowerInfo.RequireKillTargets), "true"), &value->RequireKillTargets, state);
        Marshal(node.GetAttributeValue(nameof(Ep1AiSpecialPowerInfo.UseNearTacticalTarget), "false"), &value->UseNearTacticalTarget, state);
        Marshal(node.GetAttributeValue(nameof(Ep1AiSpecialPowerInfo.UseWhenOutOfAmmo), "false"), &value->UseWhenOutOfAmmo, state);
        Marshal(node.GetAttributeValue(nameof(Ep1AiSpecialPowerInfo.DontUseIfOutRangeTarget), "true"), &value->DontUseIfOutRangeTarget, state);
        Marshal(node.GetChildNode("ObjectFilter",null),&value->ObjectFilter,state);
        var weapon=node.GetChildNode("SpecialPowerWeapon",null);
        if(weapon!=null){using Tracker.Context allocation=state.Push((void**)&value->SpecialPowerWeapon,(uint)sizeof(AISpecialPowerWeaponData),1);Marshal(weapon,value->SpecialPowerWeapon,state);}
    }
}
