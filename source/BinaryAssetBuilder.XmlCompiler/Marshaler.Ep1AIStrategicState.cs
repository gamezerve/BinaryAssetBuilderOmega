using System;
using Relo;
using SageBinaryData;
public static partial class Marshaler
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: allocate optional unsigned strategic counts only for authored values, retaining zero as a real allocated value. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Value value,uint** target,Tracker state)
    { if(value is null)return;using var context=state.Push((void**)target,4,1);Marshal(value,*target,state); }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: preserve the exact original Tactics ordinals, including any negative sentinel. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Value value,Ep1Tactics* target,Tracker state)
    { if(value is null)return;uint word=unchecked((uint)(int)Enum.Parse<Ep1Tactics>(value.GetText()));state.InplaceEndianToPlatform(&word);*target=(Ep1Tactics)word; }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: preserve the exact original TacticEndBehavior ordinals, including any negative sentinel. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Value value,Ep1TacticEndBehavior* target,Tracker state)
    { if(value is null)return;uint word=unchecked((uint)(int)Enum.Parse<Ep1TacticEndBehavior>(value.GetText()));state.InplaceEndianToPlatform(&word);*target=(Ep1TacticEndBehavior)word; }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: preserve the exact original TeamPriority ordinals, including any negative sentinel. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Value value,Ep1TeamPriority* target,Tracker state)
    { if(value is null)return;uint word=unchecked((uint)(int)Enum.Parse<Ep1TeamPriority>(value.GetText()));state.InplaceEndianToPlatform(&word);*target=(Ep1TeamPriority)word; }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: preserve the exact original TeamReleaseType ordinals, including any negative sentinel. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Value value,Ep1TeamReleaseType* target,Tracker state)
    { if(value is null)return;uint word=unchecked((uint)(int)Enum.Parse<Ep1TeamReleaseType>(value.GetText()));state.InplaceEndianToPlatform(&word);*target=(Ep1TeamReleaseType)word; }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: preserve the exact original DistanceSortPoints ordinals, including any negative sentinel. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Value value,Ep1DistanceSortPoints* target,Tracker state)
    { if(value is null)return;uint word=unchecked((uint)(int)Enum.Parse<Ep1DistanceSortPoints>(value.GetText()));state.InplaceEndianToPlatform(&word);*target=(Ep1DistanceSortPoints)word; }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: preserve the exact original AITargetDisposition ordinals, including any negative sentinel. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Value value,Ep1AITargetDisposition* target,Tracker state)
    { if(value is null)return;uint word=unchecked((uint)(int)Enum.Parse<Ep1AITargetDisposition>(value.GetText()));state.InplaceEndianToPlatform(&word);*target=(Ep1AITargetDisposition)word; }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: preserve the exact original PathfindingCheckType ordinals, including any negative sentinel. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Value value,Ep1PathfindingCheckType* target,Tracker state)
    { if(value is null)return;uint word=unchecked((uint)(int)Enum.Parse<Ep1PathfindingCheckType>(value.GetText()));state.InplaceEndianToPlatform(&word);*target=(Ep1PathfindingCheckType)word; }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: preserve the exact original AICannotAttackTargetReaction ordinals, including any negative sentinel. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Value value,Ep1AICannotAttackTargetReaction* target,Tracker state)
    { if(value is null)return;uint word=unchecked((uint)(int)Enum.Parse<Ep1AICannotAttackTargetReaction>(value.GetText()));state.InplaceEndianToPlatform(&word);*target=(Ep1AICannotAttackTargetReaction)word; }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: allocate an optional original PathfindingCheckType word only when its source attribute exists. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Value value,Ep1PathfindingCheckType** target,Tracker state)
    { if(value is null)return;using var context=state.Push((void**)target,4,1);Marshal(value,*target,state); }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: allocate an optional original AICannotAttackTargetReaction word only when its source attribute exists. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Value value,Ep1AICannotAttackTargetReaction** target,Tracker state)
    { if(value is null)return;using var context=state.Push((void**)target,4,1);Marshal(value,*target,state); }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: encode all four original team locomotor bits in their own native mask. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Value value,Ep1UnitLocomotorTypeBitFlags* target,Tracker state)
        =>Ep1ToggleEnum(value,(uint*)target,state,typeof(Ep1UnitLocomotorType),true);
    //-------------------------------------------------------------------------------------------------
    /** Reborn: serialize every original AITeamTemplate field and its complete source-sized graphs. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1AITeamTemplate* target,Tracker state)
    {
        if(node is null)return;
        Marshal(node.GetAttributeValue(nameof(Ep1AITeamTemplate.MinUnits),"1"),&target->MinUnits,state);
        Marshal(node.GetAttributeValue(nameof(Ep1AITeamTemplate.MaxUnits),null),&target->MaxUnits,state);
        Marshal(node.GetAttributeValue(nameof(Ep1AITeamTemplate.MinPowerAdvantage),null),&target->MinPowerAdvantage,state);
        Marshal(node.GetAttributeValue(nameof(Ep1AITeamTemplate.MaxTypesOfUnits),null),&target->MaxTypesOfUnits,state);
        Marshal(node.GetAttributeValue(nameof(Ep1AITeamTemplate.IncludeKindOf),null),&target->IncludeKindOf,state);
        Marshal(node.GetAttributeValue(nameof(Ep1AITeamTemplate.ExcludeKindOf),"HARVESTER IGNORES_SELECT_ALL"),&target->ExcludeKindOf,state);
        Marshal(node.GetAttributeValue(nameof(Ep1AITeamTemplate.Priority),"STANDARD"),&target->Priority,state);
        Marshal(node.GetAttributeValue(nameof(Ep1AITeamTemplate.ReleaseType),"RELEASE_TO_PRIORITY"),&target->ReleaseType,state);
        Marshal(node.GetAttributeValue(nameof(Ep1AITeamTemplate.AlwaysRecruit),"false"),&target->AlwaysRecruit,state);
        Marshal(node.GetAttributeValue(nameof(Ep1AITeamTemplate.AllowRecruitUnitsInCombat),"false"),&target->AllowRecruitUnitsInCombat,state);
        Marshal(node.GetAttributeValue(nameof(Ep1AITeamTemplate.RequireCanAttackTarget),"false"),&target->RequireCanAttackTarget,state);
        Marshal(node.GetAttributeValue(nameof(Ep1AITeamTemplate.MicroManager),null),&target->MicroManager,state);
        Marshal(node.GetAttributeValue(nameof(Ep1AITeamTemplate.AllowedLocomotorTypes),"LAND WATER AMPHIBIOUS AIR"),&target->AllowedLocomotorTypes,state);
        Marshal(node.GetAttributeValue(nameof(Ep1AITeamTemplate.SortByDistance),"true"),&target->SortByDistance,state);
        Marshal(node.GetAttributeValue(nameof(Ep1AITeamTemplate.DistanceSortPoint),"TARGET_POINT"),&target->DistanceSortPoint,state);
        Marshal(node.GetAttributeValue(nameof(Ep1AITeamTemplate.RegisteredObjectSet),"UNITS"),&target->RegisteredObjectSet,state);
        Marshal(node.GetAttributeValue(nameof(Ep1AITeamTemplate.IsBaseDefenseTeam),"false"),&target->IsBaseDefenseTeam,state);
        Marshal(node.GetAttributeValue(nameof(Ep1AITeamTemplate.BaseDefenseMaxDistance),null),&target->BaseDefenseMaxDistance,state);
        Marshal(node.GetChildNodes(nameof(Ep1AITeamTemplate.CreateUnits)),&target->CreateUnits,state);
        Marshal(node.GetChildNode(nameof(Ep1AITeamTemplate.ObjectFilter),null),&target->ObjectFilter,state);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: serialize every original AIStateTactic field and its complete source-sized graphs. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1AIStateTactic* target,Tracker state)
    {
        if(node is null)return;
        Marshal(node.GetAttributeValue(nameof(Ep1AIStateTactic.id),null),&target->id,state);
        Marshal(node.GetAttributeValue(nameof(Ep1AIStateTactic.Tactic),null),&target->Tactic,state);
        Marshal(node.GetAttributeValue(nameof(Ep1AIStateTactic.Targetless),"false"),&target->Targetless,state);
        Marshal(node.GetAttributeValue(nameof(Ep1AIStateTactic.EndBehavior),"RAMPAGE"),&target->EndBehavior,state);
        Marshal(node.GetAttributeValue(nameof(Ep1AIStateTactic.DisbandAfterRetreatTeamSize),"0"),&target->DisbandAfterRetreatTeamSize,state);
        Marshal(node.GetAttributeValue(nameof(Ep1AIStateTactic.EndIfStateTurnsOff),"false"),&target->EndIfStateTurnsOff,state);
        Marshal(node.GetAttributeValue(nameof(Ep1AIStateTactic.UseAestheticsManager),"true"),&target->UseAestheticsManager,state);
        Marshal(node.GetAttributeValue(nameof(Ep1AIStateTactic.AestheticsManagerPatrolIdleTime),"10s"),&target->AestheticsManagerPatrolIdleTime,state);
        Marshal(node.GetAttributeValue(nameof(Ep1AIStateTactic.AestheticsManagerMinDistanceFromHarvester),"300.0"),&target->AestheticsManagerMinDistanceFromHarvester,state);
        Marshal(node.GetAttributeValue(nameof(Ep1AIStateTactic.Difficulty),"EASY MEDIUM HARD BRUTAL"),&target->Difficulty,state);
        Marshal(node.GetAttributeValue(nameof(Ep1AIStateTactic.Distance),"400.0"),&target->Distance,state);
        Marshal(node.GetAttributeValue(nameof(Ep1AIStateTactic.CombatLeashSlack),"1.5"),&target->CombatLeashSlack,state);
        Marshal(node.GetAttributeValue(nameof(Ep1AIStateTactic.TargetDispositionToDisplay),"Invalid"),&target->TargetDispositionToDisplay,state);
        Marshal(node.GetAttributeValue(nameof(Ep1AIStateTactic.GatherWaypoint),null),&target->GatherWaypoint,state);
        Marshal(node.GetAttributeValue(nameof(Ep1AIStateTactic.UseTransports),"true"),&target->UseTransports,state);
        Marshal(node.GetAttributeValue(nameof(Ep1AIStateTactic.UpdateTime),"1.0s"),&target->UpdateTime,state);
        Marshal(node.GetChildNodes(nameof(Ep1AIStateTactic.TeamTemplate)),&target->TeamTemplate,state);
        Marshal(node.GetChildNode(nameof(Ep1AIStateTactic.TargetBaseCriteria),null),&target->TargetBaseCriteria,state);
        Marshal(node.GetChildNode(nameof(Ep1AIStateTactic.HomeBaseCriteria),null),&target->HomeBaseCriteria,state);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: serialize every original StateTargetHeuristic field and its complete source-sized graphs. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1StateTargetHeuristic* target,Tracker state)
    {
        if(node is null)return;
        Marshal(node.GetAttributeValue(nameof(Ep1StateTargetHeuristic.TargetHeuristic),null),&target->TargetHeuristic,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StateTargetHeuristic.Priority),"10"),&target->Priority,state);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: serialize every original AIStrategicStateDefinition field and its complete source-sized graphs. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1AIStrategicStateDefinition* target,Tracker state)
    {
        if(node is null)return;
        Marshal(node.GetAttributeValue(nameof(Ep1AIStrategicStateDefinition.MaxTargets),"1"),&target->MaxTargets,state);
        Marshal(node.GetAttributeValue(nameof(Ep1AIStrategicStateDefinition.MaxRunningCopies),null),&target->MaxRunningCopies,state);
        Marshal(node.GetAttributeValue(nameof(Ep1AIStrategicStateDefinition.MaxInactiveCopies),null),&target->MaxInactiveCopies,state);
        Marshal(node.GetAttributeValue(nameof(Ep1AIStrategicStateDefinition.PathfindingCheckType),null),&target->PathfindingCheckType,state);
        Marshal(node.GetAttributeValue(nameof(Ep1AIStrategicStateDefinition.CannotAttackTargetReaction),null),&target->CannotAttackTargetReaction,state);
        Marshal(node.GetAttributeValue(nameof(Ep1AIStrategicStateDefinition.TeamGatherBeginFeedbackKey),null),&target->TeamGatherBeginFeedbackKey,state);
        Marshal(node.GetAttributeValue(nameof(Ep1AIStrategicStateDefinition.ExecutionBeginFeedbackKey),null),&target->ExecutionBeginFeedbackKey,state);
        Marshal(node.GetAttributeValue(nameof(Ep1AIStrategicStateDefinition.ExecutionCanceledFeedbackKey),null),&target->ExecutionCanceledFeedbackKey,state);
        Marshal(node.GetAttributeValue(nameof(Ep1AIStrategicStateDefinition.ExecutionCompleteFeedbackKey),null),&target->ExecutionCompleteFeedbackKey,state);
        Marshal(node.GetChildNodes(nameof(Ep1AIStrategicStateDefinition.TargetHeuristic)),&target->TargetHeuristic,state);
        Marshal(node.GetChildNodes(nameof(Ep1AIStrategicStateDefinition.Tactic)),&target->Tactic,state);
        Marshal(node,(Ep1AIStateDefinition*)target,state);
    }

}
