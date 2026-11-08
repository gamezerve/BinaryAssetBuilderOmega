using System;
using Relo;
using SageBinaryData;
public static partial class Marshaler
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: compile the original AI timer origin enum in official order. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Value value,Ep1AITimerType* target,Tracker state)
        =>Ep1ToggleEnum(value,(uint*)target,state,typeof(Ep1AITimerType),false);
    //-------------------------------------------------------------------------------------------------
    /** Reborn: preserve explicit -1/0/1/2 beacon ordinals without Enum.GetNames reordering the negative sentinel. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Value value,Ep1AIBeaconType* target,Tracker state)
    { if(value is null)return;uint word=unchecked((uint)(int)Enum.Parse<Ep1AIBeaconType>(value.GetText()));state.InplaceEndianToPlatform(&word);*target=(Ep1AIBeaconType)word; }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: encode the three original building surface bits in their own official mask. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Value value,Ep1BuildingPlacementSurfaceBitFlags* target,Tracker state)
        =>Ep1ToggleEnum(value,(uint*)target,state,typeof(Ep1BuildingPlacementSurface),true);
    //-------------------------------------------------------------------------------------------------
    /** Reborn: serialize the complete original AIStateTimerHeuristic fields and schema defaults. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1AIStateTimerHeuristic* target,Tracker state)
    {
        if(node is null)return;
        Marshal(node.GetAttributeValue(nameof(Ep1AIStateTimerHeuristic.StartTime),"0s"),&target->StartTime,state);
        Marshal(node.GetAttributeValue(nameof(Ep1AIStateTimerHeuristic.EndTime),"-1s"),&target->EndTime,state);
        Marshal(node.GetAttributeValue(nameof(Ep1AIStateTimerHeuristic.CountFrom),"MAP_START"),&target->CountFrom,state);
        uint wire=0x10A03AC0u;state.InplaceEndianToPlatform(&wire);target->Base.TypeId=wire;
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: serialize the complete original AIStateIntervalHeuristic fields and schema defaults. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1AIStateIntervalHeuristic* target,Tracker state)
    {
        if(node is null)return;
        Marshal(node.GetAttributeValue(nameof(Ep1AIStateIntervalHeuristic.IntervalTime),"60s"),&target->IntervalTime,state);
        Marshal(node.GetAttributeValue(nameof(Ep1AIStateIntervalHeuristic.ActiveTime),"30s"),&target->ActiveTime,state);
        Marshal(node.GetAttributeValue(nameof(Ep1AIStateIntervalHeuristic.CountFrom),"MAP_START"),&target->CountFrom,state);
        uint wire=0x8199E979u;state.InplaceEndianToPlatform(&wire);target->Base.TypeId=wire;
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: serialize the complete original AIStateMapNameHeuristic fields and schema defaults. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1AIStateMapNameHeuristic* target,Tracker state)
    {
        if(node is null)return;
        Marshal(node.GetAttributeValue(nameof(Ep1AIStateMapNameHeuristic.Name),null),&target->Name,state);
        Marshal(node.GetAttributeValue(nameof(Ep1AIStateMapNameHeuristic.PassIfTrue),"true"),&target->PassIfTrue,state);
        uint wire=0xDAFA6EB8u;state.InplaceEndianToPlatform(&wire);target->Base.TypeId=wire;
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: serialize the complete original AIStatePathToTargetHeuristic fields and schema defaults. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1AIStatePathToTargetHeuristic* target,Tracker state)
    {
        if(node is null)return;
        Ep1ToggleEnum(node.GetAttributeValue(nameof(Ep1AIStatePathToTargetHeuristic.Surface),"Land"),(uint*)&target->Surface,state,typeof(Ep1AITargetLocationType),false);
        Marshal(node.GetAttributeValue(nameof(Ep1AIStatePathToTargetHeuristic.ObjectTypeToUseForTest),null),&target->ObjectTypeToUseForTest,state);
        Marshal(node.GetAttributeValue(nameof(Ep1AIStatePathToTargetHeuristic.LocomotorConditionToUseForTest),"NORMAL"),&target->LocomotorConditionToUseForTest,state);
        Marshal(node.GetAttributeValue(nameof(Ep1AIStatePathToTargetHeuristic.PathExists),"true"),&target->PathExists,state);
        uint wire=0xF1B0E2B3u;state.InplaceEndianToPlatform(&wire);target->Base.TypeId=wire;
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: serialize the complete original AIStateScriptedFlagHeuristic fields and schema defaults. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1AIStateScriptedFlagHeuristic* target,Tracker state)
    {
        if(node is null)return;
        Marshal(node.GetAttributeValue(nameof(Ep1AIStateScriptedFlagHeuristic.FlagName),null),&target->FlagName,state);
        uint wire=0x809071B7u;state.InplaceEndianToPlatform(&wire);target->Base.TypeId=wire;
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: serialize the complete original AIStateIncomeAdvantageHeuristic fields and schema defaults. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1AIStateIncomeAdvantageHeuristic* target,Tracker state)
    {
        if(node is null)return;
        Marshal(node.GetAttributeValue(nameof(Ep1AIStateIncomeAdvantageHeuristic.Value),"1.0"),&target->Value,state);
        Marshal(node.GetAttributeValue(nameof(Ep1AIStateIncomeAdvantageHeuristic.Above),"true"),&target->Above,state);
        uint wire=0x52DEE998u;state.InplaceEndianToPlatform(&wire);target->Base.TypeId=wire;
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: serialize the complete original AIStateIsBuildingHeuristic fields and schema defaults. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1AIStateIsBuildingHeuristic* target,Tracker state)
    {
        if(node is null)return;
        Marshal(node.GetAttributeValue(nameof(Ep1AIStateIsBuildingHeuristic.KindOf),""),&target->KindOf,state);
        Marshal(node.GetAttributeValue(nameof(Ep1AIStateIsBuildingHeuristic.PassIfBuilding),"true"),&target->PassIfBuilding,state);
        uint wire=0x45C8CCA2u;state.InplaceEndianToPlatform(&wire);target->Base.TypeId=wire;
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: serialize the complete original AIStateTechBuildingShareHeuristic fields and schema defaults. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1AIStateTechBuildingShareHeuristic* target,Tracker state)
    {
        if(node is null)return;
        Marshal(node.GetAttributeValue(nameof(Ep1AIStateTechBuildingShareHeuristic.PercentOfFairShare),"100%"),&target->PercentOfFairShare,state);
        Marshal(node.GetAttributeValue(nameof(Ep1AIStateTechBuildingShareHeuristic.Above),"false"),&target->Above,state);
        uint wire=0xCBA1A93Bu;state.InplaceEndianToPlatform(&wire);target->Base.TypeId=wire;
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: serialize the complete original AIStateAllNearbyNodesFullHeuristic fields and schema defaults. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1AIStateAllNearbyNodesFullHeuristic* target,Tracker state)
    {
        if(node is null)return;
        uint wire=0x38B3FD56u;state.InplaceEndianToPlatform(&wire);target->Base.TypeId=wire;
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: serialize the complete original AIStateTimeSinceLastExpansionHeuristic fields and schema defaults. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1AIStateTimeSinceLastExpansionHeuristic* target,Tracker state)
    {
        if(node is null)return;
        Marshal(node.GetAttributeValue(nameof(Ep1AIStateTimeSinceLastExpansionHeuristic.Time),"60s"),&target->Time,state);
        Marshal(node.GetAttributeValue(nameof(Ep1AIStateTimeSinceLastExpansionHeuristic.Past),"true"),&target->Past,state);
        uint wire=0xE40EBC24u;state.InplaceEndianToPlatform(&wire);target->Base.TypeId=wire;
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: serialize the complete original AIStateResourceNodeShareHeuristic fields and schema defaults. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1AIStateResourceNodeShareHeuristic* target,Tracker state)
    {
        if(node is null)return;
        Marshal(node.GetAttributeValue(nameof(Ep1AIStateResourceNodeShareHeuristic.PercentOfFairShare),"100%"),&target->PercentOfFairShare,state);
        Marshal(node.GetAttributeValue(nameof(Ep1AIStateResourceNodeShareHeuristic.Above),"false"),&target->Above,state);
        uint wire=0x874A6E09u;state.InplaceEndianToPlatform(&wire);target->Base.TypeId=wire;
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: serialize the complete original AIStateCanBuildBuildingsOnSurfaceHeuristic fields and schema defaults. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1AIStateCanBuildBuildingsOnSurfaceHeuristic* target,Tracker state)
    {
        if(node is null)return;
        Marshal(node.GetAttributeValue(nameof(Ep1AIStateCanBuildBuildingsOnSurfaceHeuristic.MinPlacementGridCells),"1"),&target->MinPlacementGridCells,state);
        Marshal(node.GetAttributeValue(nameof(Ep1AIStateCanBuildBuildingsOnSurfaceHeuristic.MaxPlacementGridCells),null),&target->MaxPlacementGridCells,state);
        Marshal(node.GetAttributeValue(nameof(Ep1AIStateCanBuildBuildingsOnSurfaceHeuristic.Surfaces),null),&target->Surfaces,state);
        Marshal(node.GetAttributeValue(nameof(Ep1AIStateCanBuildBuildingsOnSurfaceHeuristic.ObjectTypeToUseForTest),null),&target->ObjectTypeToUseForTest,state);
        uint wire=0x9EE54386u;state.InplaceEndianToPlatform(&wire);target->Base.TypeId=wire;
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: serialize the complete original AIAlliedBeaconExistsHeuristic fields and schema defaults. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1AIAlliedBeaconExistsHeuristic* target,Tracker state)
    {
        if(node is null)return;
        Marshal(node.GetAttributeValue(nameof(Ep1AIAlliedBeaconExistsHeuristic.Type),null),&target->Type,state);
        Marshal(node.GetAttributeValue(nameof(Ep1AIAlliedBeaconExistsHeuristic.Exists),null),&target->Exists,state);
        uint wire=0x7E198900u;state.InplaceEndianToPlatform(&wire);target->Base.TypeId=wire;
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: serialize the complete original AIStateOpenNodeHeuristic fields and schema defaults. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1AIStateOpenNodeHeuristic* target,Tracker state)
    {
        if(node is null)return;
        Marshal(node.GetAttributeValue(nameof(Ep1AIStateOpenNodeHeuristic.MinDistanceFromEnemy),"400.0"),&target->MinDistanceFromEnemy,state);
        Marshal(node.GetAttributeValue(nameof(Ep1AIStateOpenNodeHeuristic.Exists),"true"),&target->Exists,state);
        uint wire=0x84ACF588u;state.InplaceEndianToPlatform(&wire);target->Base.TypeId=wire;
    }


    //-------------------------------------------------------------------------------------------------
    /** Reborn: compile the engine-reviewed under-attack heuristic using the official Time and boolean defaults. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1AIStateStructureOrHarvesterUnderAttackHeuristic* target,Tracker state)
    {
        if(node is null)return;
        Marshal(node.GetAttributeValue("TimeSinceAttacked","4s"),&target->TimeSinceAttacked,state);
        Marshal(node.GetAttributeValue("UnderAttack","true"),&target->UnderAttack,state);
        uint wire=0x6B660E54u;state.InplaceEndianToPlatform(&wire);target->Base.TypeId=wire;
    }
}
