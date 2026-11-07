using System;
using Relo;
using SageBinaryData;

public static partial class Marshaler
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: write a source-authored placement radius in the ordered native search-distance list. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1BuildingPlacementDistance* target,Tracker state)
    {
        if(node is null)return;
        Marshal(node.GetAttributeValue(nameof(Ep1BuildingPlacementDistance.Distance),null),&target->Distance,state);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: keep the optional beacon enum's real -1/0/1/2 values without adding a pointee when it is absent. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Value value,Ep1AIBeaconType** target,Tracker state)
    {
        if(value is null)return;
        using var context=state.Push((void**)target,(uint)sizeof(Ep1AIBeaconType),1);
        var parsed=(Ep1AIBeaconType)Enum.Parse(typeof(Ep1AIBeaconType),value.GetText());
        if(!Enum.IsDefined(typeof(Ep1AIBeaconType),parsed))throw new InvalidOperationException("Unknown EP1 beacon type.");
        uint native=unchecked((uint)(int)parsed);state.InplaceEndianToPlatform(&native);**target=(Ep1AIBeaconType)native;
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: allocate an optional native relationship mask only for an explicitly authored relationship. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Value value,ObjectFilterRelationshipBitMask** target,Tracker state)
    {
        if(value is null)return;
        using var context=state.Push((void**)target,(uint)sizeof(ObjectFilterRelationshipBitMask),1);
        Marshal(value,*target,state);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: resolve Water/Land as schema-indexed bits in the existing strict flag codec. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Value value,Ep1AITargetLocationBitFlags* target,Tracker state)
        =>Ep1ToggleEnum(value,(uint*)target,state,typeof(Ep1AITargetLocationType),true);

    //-------------------------------------------------------------------------------------------------
    /** Reborn: compile the complete EP1 target-selection record instead of truncating it to the legacy KW layout. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1AITargetingHeuristic* target,Tracker state)
    {
        if(node is null)return;
        Ep1ToggleEnum(node.GetAttributeValue(nameof(Ep1AITargetingHeuristic.HeuristicType),null),(uint*)&target->HeuristicType,state,typeof(Ep1TargetingHeuristicType),false);
        Ep1ToggleEnum(node.GetAttributeValue(nameof(Ep1AITargetingHeuristic.SortType),"Distance"),(uint*)&target->SortType,state,typeof(Ep1AITargetSortType),false);
        Marshal(node.GetAttributeValue(nameof(Ep1AITargetingHeuristic.VitalKindOf),null),&target->VitalKindOf,state);
        Marshal(node.GetAttributeValue(nameof(Ep1AITargetingHeuristic.ForbiddenKindOf),null),&target->ForbiddenKindOf,state);
        Marshal(node.GetAttributeValue(nameof(Ep1AITargetingHeuristic.VitalObjectStatus),null),&target->VitalObjectStatus,state);
        Marshal(node.GetAttributeValue(nameof(Ep1AITargetingHeuristic.ForbiddenObjectStatus),null),&target->ForbiddenObjectStatus,state);
        Marshal(node.GetAttributeValue(nameof(Ep1AITargetingHeuristic.ThreatFinderRange),null),&target->ThreatFinderRange,state);
        Marshal(node.GetAttributeValue(nameof(Ep1AITargetingHeuristic.SearchRange),"1500.0"),&target->SearchRange,state);
        Marshal(node.GetAttributeValue(nameof(Ep1AITargetingHeuristic.Beacon),null),&target->Beacon,state);
        Marshal(node.GetAttributeValue(nameof(Ep1AITargetingHeuristic.Name),null),&target->Name,state);
        Marshal(node.GetAttributeValue(nameof(Ep1AITargetingHeuristic.Location),"Water Land"),&target->Location,state);
        Marshal(node.GetAttributeValue(nameof(Ep1AITargetingHeuristic.MaxDistanceToShore),"1000.0"),&target->MaxDistanceToShore,state);
        Marshal(node.GetAttributeValue(nameof(Ep1AITargetingHeuristic.Relationship),null),&target->Relationship,state);
        Marshal(node.GetAttributeValue(nameof(Ep1AITargetingHeuristic.MinDistanceFromEnemy),"0.0"),&target->MinDistanceFromEnemy,state);
        Marshal(node.GetAttributeValue(nameof(Ep1AITargetingHeuristic.MinDistanceFromConyardEntrance),"0.0"),&target->MinDistanceFromConyardEntrance,state);
        Marshal(node.GetAttributeValue(nameof(Ep1AITargetingHeuristic.IgnoreMinDistanceFromConyardEntranceForTime),"0s"),&target->IgnoreMinDistanceFromConyardEntranceForTime,state);
        Marshal(node.GetChildNode(nameof(Ep1AITargetingHeuristic.BaseSelection),null),&target->BaseSelection,state);
        Marshal(node.GetChildNodes(nameof(Ep1AITargetingHeuristic.BuildingPlacementDistance)),&target->BuildingPlacementDistance,state);
        Marshal(node.GetAttributeValue(nameof(Ep1AITargetingHeuristic.Destroyed),"false"),&target->Destroyed,state);
        Marshal(node.GetAttributeValue(nameof(Ep1AITargetingHeuristic.HighestPriorityEnemyPlayersOnly),"true"),&target->HighestPriorityEnemyPlayersOnly,state);
        Marshal(node.GetAttributeValue(nameof(Ep1AITargetingHeuristic.UnderAttack),"false"),&target->UnderAttack,state);
        Marshal(node.GetAttributeValue(nameof(Ep1AITargetingHeuristic.RequireUniqueTarget),"false"),&target->RequireUniqueTarget,state);
        Marshal(node,(BaseAssetType*)target,state);
    }
}
