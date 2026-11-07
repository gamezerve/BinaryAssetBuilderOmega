using Relo;
using SageBinaryData;

public static partial class Marshaler
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: preserve all official AIData scalar/string defaults and the separately native-qualified flag tail. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1AIData* target,Tracker state)
    {
        if(node is null)return;
        Marshal(node.GetAttributeValue(nameof(Ep1AIData.StructureSeconds),"0.0"),&target->StructureSeconds,state);
        Marshal(node.GetAttributeValue(nameof(Ep1AIData.TeamSeconds),"0.0"),&target->TeamSeconds,state);
        Marshal(node.GetAttributeValue(nameof(Ep1AIData.ResourcesWealthy),"0"),&target->ResourcesWealthy,state);
        Marshal(node.GetAttributeValue(nameof(Ep1AIData.ResourcesPoor),"0"),&target->ResourcesPoor,state);
        Marshal(node.GetAttributeValue(nameof(Ep1AIData.StructuresWealthyMod),"0.0"),&target->StructuresWealthyMod,state);
        Marshal(node.GetAttributeValue(nameof(Ep1AIData.TeamWealthyMod),"0.0"),&target->TeamWealthyMod,state);
        Marshal(node.GetAttributeValue(nameof(Ep1AIData.StructuresPoorMod),"0.0"),&target->StructuresPoorMod,state);
        Marshal(node.GetAttributeValue(nameof(Ep1AIData.TeamPoorMod),"0.0"),&target->TeamPoorMod,state);
        Marshal(node.GetAttributeValue(nameof(Ep1AIData.TeamResourcesToBuild),"0.0"),&target->TeamResourcesToBuild,state);
        Marshal(node.GetAttributeValue(nameof(Ep1AIData.GuardChaseUnitFrames),"0"),&target->GuardChaseUnitFrames,state);
        Marshal(node.GetAttributeValue(nameof(Ep1AIData.GuardEnemyScanRate),"7"),&target->GuardEnemyScanRate,state);
        Marshal(node.GetAttributeValue(nameof(Ep1AIData.GuardEnemyReturnScanRate),"15"),&target->GuardEnemyReturnScanRate,state);
        Marshal(node.GetAttributeValue(nameof(Ep1AIData.WallHeight),"0.0"),&target->WallHeight,state);
        Marshal(node.GetAttributeValue(nameof(Ep1AIData.SkirmishGroupFudgeValue),"0.0"),&target->SkirmishGroupFudgeValue,state);
        Marshal(node.GetAttributeValue(nameof(Ep1AIData.MaxRecruitDistance),"0.0"),&target->MaxRecruitDistance,state);
        Marshal(node.GetAttributeValue(nameof(Ep1AIData.RepulsedDistance),"0.0"),&target->RepulsedDistance,state);
        Marshal(node.GetAttributeValue(nameof(Ep1AIData.EnableRepulsors),"false"),&target->EnableRepulsors,state);
        Marshal(node.GetAttributeValue(nameof(Ep1AIData.ForceSkirmishAI),"false"),&target->ForceSkirmishAI,state);
        Marshal(node.GetAttributeValue(nameof(Ep1AIData.RotateSkirmishBases),"false"),&target->RotateSkirmishBases,state);
        Marshal(node.GetAttributeValue(nameof(Ep1AIData.AttackUsesLineOfSight),"true"),&target->AttackUsesLineOfSight,state);
        Marshal(node.GetAttributeValue(nameof(Ep1AIData.MinDistanceForGroup),"100.0"),&target->MinDistanceForGroup,state);
        Marshal(node.GetAttributeValue(nameof(Ep1AIData.FormationEnemyDistance),"600.0"),&target->FormationEnemyDistance,state);
        Marshal(node.GetAttributeValue(nameof(Ep1AIData.RebuildDelaySeconds),"10"),&target->RebuildDelaySeconds,state);
        Marshal(node.GetAttributeValue(nameof(Ep1AIData.SupplyCenterSafeRadius),"250.0"),&target->SupplyCenterSafeRadius,state);
        Marshal(node.GetAttributeValue(nameof(Ep1AIData.AIDozerBoredRadiusModifier),"2.0"),&target->AIDozerBoredRadiusModifier,state);
        Marshal(node.GetAttributeValue(nameof(Ep1AIData.AICrushesInfantry),"true"),&target->AICrushesInfantry,state);
        Marshal(node.GetAttributeValue(nameof(Ep1AIData.MeleeApproachTolerance),"20.0"),&target->MeleeApproachTolerance,state);
        Marshal(node.GetAttributeValue(nameof(Ep1AIData.MeleeApproachDist),"60.0"),&target->MeleeApproachDist,state);
        Marshal(node.GetAttributeValue(nameof(Ep1AIData.WadeWaterDepth),"5.0"),&target->WadeWaterDepth,state);
        Marshal(node.GetAttributeValue(nameof(Ep1AIData.DeepWaterDepth),"50.0"),&target->DeepWaterDepth,state);
        Marshal(node.GetAttributeValue(nameof(Ep1AIData.FormationColumnWidth),"65.0"),&target->FormationColumnWidth,state);
        Marshal(node.GetAttributeValue(nameof(Ep1AIData.FormationRowDepth),"65.0"),&target->FormationRowDepth,state);
        Marshal(node.GetAttributeValue(nameof(Ep1AIData.FormationSquadSpacing),"30.0"),&target->FormationSquadSpacing,state);
        Marshal(node.GetAttributeValue(nameof(Ep1AIData.NarrowPassageScale),"1.0"),&target->NarrowPassageScale,state);
        Marshal(node.GetAttributeValue(nameof(Ep1AIData.MaxSquadWidth),"2"),&target->MaxSquadWidth,state);
        Marshal(node.GetAttributeValue(nameof(Ep1AIData.WaitForOthers),"false"),&target->WaitForOthers,state);
        Marshal(node.GetAttributeValue(nameof(Ep1AIData.HordesWaitForHordes),"true"),&target->HordesWaitForHordes,state);
        Marshal(node.GetAttributeValue(nameof(Ep1AIData.AttackMoveUsesFormations),"true"),&target->AttackMoveUsesFormations,state);
        Marshal(node.GetAttributeValue(nameof(Ep1AIData.AllowForestFires),"false"),&target->AllowForestFires,state);
        Marshal(node.GetAttributeValue(nameof(Ep1AIData.UseFormations),"true"),&target->UseFormations,state);
        Marshal(node.GetAttributeValue(nameof(Ep1AIData.AltCameraZoomOverride),"1.4"),&target->AltCameraZoomOverride,state);
        Marshal(node.GetAttributeValue(nameof(Ep1AIData.AltCameraPitchOverride),"0.5"),&target->AltCameraPitchOverride,state);
        Marshal(node.GetAttributeValue(nameof(Ep1AIData.MaxRetaliateDistance),"200.0"),&target->MaxRetaliateDistance,state);
        Marshal(node.GetAttributeValue(nameof(Ep1AIData.RetaliateFriendsRadius),"120.0"),&target->RetaliateFriendsRadius,state);
        Marshal(node.GetAttributeValue(nameof(Ep1AIData.ChaseFromBehindLimit),"50.0"),&target->ChaseFromBehindLimit,state);
        Marshal(node.GetAttributeValue(nameof(Ep1AIData.CastleSiegeStandBackDistance),"100.0"),&target->CastleSiegeStandBackDistance,state);
        Marshal(node.GetAttributeValue(nameof(Ep1AIData.UseLowLODTrees),"false"),&target->UseLowLODTrees,state);
        Marshal(node.GetAttributeValue(nameof(Ep1AIData.LowLodTree),null),&target->LowLodTree,state);
        Marshal(node.GetAttributeValue(nameof(Ep1AIData.LowLodTreeNoGrab),null),&target->LowLodTreeNoGrab,state);
        Marshal(node.GetAttributeValue(nameof(Ep1AIData.LowLodTreeNoHarvest),null),&target->LowLodTreeNoHarvest,state);
        Marshal(node.GetAttributeValue(nameof(Ep1AIData.LowLodTreeScale),"0.55"),&target->LowLodTreeScale,state);
        Marshal(node.GetAttributeValue(nameof(Ep1AIData.DisableTrees),"false"),&target->DisableTrees,state);
        Marshal(node.GetAttributeValue(nameof(Ep1AIData.MaximumCraterDepth),"8"),&target->MaximumCraterDepth,state);
        Marshal(node.GetAttributeValue(nameof(Ep1AIData.Optimization_NumTeamsToRecruit),"10"),&target->Optimization_NumTeamsToRecruit,state);
        Marshal(node.GetAttributeValue(nameof(Ep1AIData.Optimization_NumEfficiencyUpdates),"2"),&target->Optimization_NumEfficiencyUpdates,state);
        Marshal(node.GetAttributeValue(nameof(Ep1AIData.Optimization_TimeToCacheCanEnterTransport),"2s"),&target->Optimization_TimeToCacheCanEnterTransport,state);
        Marshal(node.GetAttributeValue(nameof(Ep1AIData.Optimization_TimeToCacheCanReachMobileTarget),"1s"),&target->Optimization_TimeToCacheCanReachMobileTarget,state);
        Marshal(node.GetAttributeValue(nameof(Ep1AIData.Optimization_TimeToCacheCanReachImmobileTarget),"1s"),&target->Optimization_TimeToCacheCanReachImmobileTarget,state);
        Marshal(node.GetAttributeValue(nameof(Ep1AIData.Optimization_ConquerMetricsCacheTimeOut),"3s"),&target->Optimization_ConquerMetricsCacheTimeOut,state);
        Marshal(node.GetAttributeValue(nameof(Ep1AIData.Optimization_TacticsEmergencyKickPeriod),"4s"),&target->Optimization_TacticsEmergencyKickPeriod,state);
        Marshal(node,(BaseInheritableAsset*)target,state);
    }
}
