using Relo;
using SageBinaryData;
public static partial class Marshaler
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: serialize the complete original EP1 AIStateConstantHeuristic record and schema defaults. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1AIStateConstantHeuristic* target,Tracker state)
    {
        if(node is null)return;
        Marshal(node.GetAttributeValue(nameof(Ep1AIStateConstantHeuristic.Weight),"1.0"),&target->Weight,state);
        uint wire=0x51FB6D92u;state.InplaceEndianToPlatform(&wire);target->Base.TypeId=wire;
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: serialize the complete original EP1 AIStateOpeningMoveHeuristic record and schema defaults. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1AIStateOpeningMoveHeuristic* target,Tracker state)
    {
        if(node is null)return;
        Marshal(node.GetAttributeValue(nameof(Ep1AIStateOpeningMoveHeuristic.Complete),"true"),&target->Complete,state);
        uint wire=0x7EE4C9B4u;state.InplaceEndianToPlatform(&wire);target->Base.TypeId=wire;
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: serialize the complete original EP1 AIStateEnemyNearbyHeuristic record and schema defaults. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1AIStateEnemyNearbyHeuristic* target,Tracker state)
    {
        if(node is null)return;
        Marshal(node.GetAttributeValue(nameof(Ep1AIStateEnemyNearbyHeuristic.Distance),"1000.0"),&target->Distance,state);
        Marshal(node.GetAttributeValue(nameof(Ep1AIStateEnemyNearbyHeuristic.MinEnemyValue),"0"),&target->MinEnemyValue,state);
        Marshal(node.GetAttributeValue(nameof(Ep1AIStateEnemyNearbyHeuristic.EnemyNearby),"true"),&target->EnemyNearby,state);
        Marshal(node.GetChildNode(nameof(Ep1AIStateEnemyNearbyHeuristic.EnemyFilter),null),&target->EnemyFilter,state);
        uint wire=0x67B985A7u;state.InplaceEndianToPlatform(&wire);target->Base.TypeId=wire;
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: serialize the complete original EP1 AIStateObjectOfTypeExistsHeuristic record and schema defaults. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1AIStateObjectOfTypeExistsHeuristic* target,Tracker state)
    {
        if(node is null)return;
        Marshal(node.GetAttributeValue(nameof(Ep1AIStateObjectOfTypeExistsHeuristic.Location),"Water Land"),&target->Location,state);
        Marshal(node.GetAttributeValue(nameof(Ep1AIStateObjectOfTypeExistsHeuristic.PassIfExists),"true"),&target->PassIfExists,state);
        Marshal(node.GetChildNode(nameof(Ep1AIStateObjectOfTypeExistsHeuristic.ObjectFilter),null),&target->ObjectFilter,state);
        uint wire=0xBE76E654u;state.InplaceEndianToPlatform(&wire);target->Base.TypeId=wire;
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: serialize the complete original EP1 AIStatePowerAdvantageHeuristic record and schema defaults. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1AIStatePowerAdvantageHeuristic* target,Tracker state)
    {
        if(node is null)return;
        Marshal(node.GetAttributeValue(nameof(Ep1AIStatePowerAdvantageHeuristic.Value),"1.0"),&target->Value,state);
        Marshal(node.GetAttributeValue(nameof(Ep1AIStatePowerAdvantageHeuristic.Above),"true"),&target->Above,state);
        Marshal(node.GetAttributeValue(nameof(Ep1AIStatePowerAdvantageHeuristic.IncludeOwnDefenses),"true"),&target->IncludeOwnDefenses,state);
        Marshal(node.GetAttributeValue(nameof(Ep1AIStatePowerAdvantageHeuristic.IncludeEnemyDefenses),"true"),&target->IncludeEnemyDefenses,state);
        Marshal(node.GetAttributeValue(nameof(Ep1AIStatePowerAdvantageHeuristic.OwnKindOf),"CAN_ATTACK"),&target->OwnKindOf,state);
        Marshal(node.GetAttributeValue(nameof(Ep1AIStatePowerAdvantageHeuristic.EnemyKindOf),"CAN_ATTACK"),&target->EnemyKindOf,state);
        uint wire=0xADC0BB89u;state.InplaceEndianToPlatform(&wire);target->Base.TypeId=wire;
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: allocate only explicitly reviewed EP1 heuristic classes; legacy KW dispatch is never used. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1AIStateHeuristic** target,Tracker state)
    {
        if(node is null)return;uint wire=0;Marshal(node.GetAttributeValue("TypeId","0"),&wire,Tracker.NullTracker);
        switch(wire)
        {
            case 0x51FB6D92u:MarshalPolymorphicType<Ep1AIStateConstantHeuristic,Ep1AIStateHeuristic>(node,target,state);break;
            case 0x7EE4C9B4u:MarshalPolymorphicType<Ep1AIStateOpeningMoveHeuristic,Ep1AIStateHeuristic>(node,target,state);break;
            case 0x67B985A7u:MarshalPolymorphicType<Ep1AIStateEnemyNearbyHeuristic,Ep1AIStateHeuristic>(node,target,state);break;
            case 0xBE76E654u:MarshalPolymorphicType<Ep1AIStateObjectOfTypeExistsHeuristic,Ep1AIStateHeuristic>(node,target,state);break;
            case 0xADC0BB89u:MarshalPolymorphicType<Ep1AIStatePowerAdvantageHeuristic,Ep1AIStateHeuristic>(node,target,state);break;
            case 0x10A03AC0u:MarshalPolymorphicType<Ep1AIStateTimerHeuristic,Ep1AIStateHeuristic>(node,target,state);break;
            case 0x8199E979u:MarshalPolymorphicType<Ep1AIStateIntervalHeuristic,Ep1AIStateHeuristic>(node,target,state);break;
            case 0xDAFA6EB8u:MarshalPolymorphicType<Ep1AIStateMapNameHeuristic,Ep1AIStateHeuristic>(node,target,state);break;
            case 0xF1B0E2B3u:MarshalPolymorphicType<Ep1AIStatePathToTargetHeuristic,Ep1AIStateHeuristic>(node,target,state);break;
            case 0x809071B7u:MarshalPolymorphicType<Ep1AIStateScriptedFlagHeuristic,Ep1AIStateHeuristic>(node,target,state);break;
            case 0x52DEE998u:MarshalPolymorphicType<Ep1AIStateIncomeAdvantageHeuristic,Ep1AIStateHeuristic>(node,target,state);break;
            case 0x45C8CCA2u:MarshalPolymorphicType<Ep1AIStateIsBuildingHeuristic,Ep1AIStateHeuristic>(node,target,state);break;
            case 0xCBA1A93Bu:MarshalPolymorphicType<Ep1AIStateTechBuildingShareHeuristic,Ep1AIStateHeuristic>(node,target,state);break;
            case 0x38B3FD56u:MarshalPolymorphicType<Ep1AIStateAllNearbyNodesFullHeuristic,Ep1AIStateHeuristic>(node,target,state);break;
            case 0xE40EBC24u:MarshalPolymorphicType<Ep1AIStateTimeSinceLastExpansionHeuristic,Ep1AIStateHeuristic>(node,target,state);break;
            case 0x874A6E09u:MarshalPolymorphicType<Ep1AIStateResourceNodeShareHeuristic,Ep1AIStateHeuristic>(node,target,state);break;
            case 0x9EE54386u:MarshalPolymorphicType<Ep1AIStateCanBuildBuildingsOnSurfaceHeuristic,Ep1AIStateHeuristic>(node,target,state);break;
            case 0x7E198900u:MarshalPolymorphicType<Ep1AIAlliedBeaconExistsHeuristic,Ep1AIStateHeuristic>(node,target,state);break;
            // Reborn: retain the actual original concrete heuristic wire ID.
            case 0x6B660E54u:MarshalPolymorphicType<Ep1AIStateStructureOrHarvesterUnderAttackHeuristic,Ep1AIStateHeuristic>(node,target,state);break;
            case 0x84ACF588u:MarshalPolymorphicType<Ep1AIStateOpenNodeHeuristic,Ep1AIStateHeuristic>(node,target,state);break;
            default:throw new System.NotSupportedException("AI heuristic source family not reviewed: "+wire.ToString("X8"));
        }
        state.InplaceEndianToPlatform(&wire);*(uint*)*target=wire;
    }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: preserve the complete authored heuristic list in the original AI state base. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1AIStateDefinition* target,Tracker state)
    {
        if(node is null)return;
        Marshal(node.GetChildNode(nameof(Ep1AIStateDefinition.Heuristic),null),&target->Heuristic,state);
        Marshal(node,(BaseAssetType*)target,state);
    }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: compile the budget's ordered account shares together with its full source-authored heuristic graph. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1AIBudgetStateDefinition* target,Tracker state)
    {
        if(node is null)return;
        Marshal(node.GetChildNode(nameof(Ep1AIBudgetStateDefinition.Budget),null),&target->Budget,state);
        Marshal(node,(Ep1AIStateDefinition*)target,state);
    }
}
