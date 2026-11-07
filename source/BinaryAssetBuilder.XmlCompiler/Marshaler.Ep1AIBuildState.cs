using Relo;
using SageBinaryData;
public static partial class Marshaler
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: encode the complete official EP1 build heuristic enum while retaining zero for an absent optional selector. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Value value,Ep1AIBuildHeuristics* target,Tracker state)
    { if(value is null)return;Ep1ToggleEnum(value,(uint*)target,state,typeof(Ep1AIBuildHeuristics),false); }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: compile every original build-entry field, retaining weak object/upgrade identities and the optional filter graph. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1AIBuildEntry* target,Tracker state)
    {
        if(node is null)return;
        Marshal(node.GetAttributeValue(nameof(Ep1AIBuildEntry.Build),null),&target->Build,state);
        Marshal(node.GetAttributeValue(nameof(Ep1AIBuildEntry.Upgrade),null),&target->Upgrade,state);
        Marshal(node.GetAttributeValue(nameof(Ep1AIBuildEntry.Heuristic),null),&target->Heuristic,state);
        Marshal(node.GetAttributeValue(nameof(Ep1AIBuildEntry.PercentLow),"0%"),&target->PercentLow,state);
        Marshal(node.GetAttributeValue(nameof(Ep1AIBuildEntry.PercentHigh),"0%"),&target->PercentHigh,state);
        Marshal(node.GetAttributeValue(nameof(Ep1AIBuildEntry.ExpectedEfficiencyWeight),"0.4"),&target->ExpectedEfficiencyWeight,state);
        Marshal(node.GetAttributeValue(nameof(Ep1AIBuildEntry.ActualEfficiencyWeight),"0.6"),&target->ActualEfficiencyWeight,state);
        Marshal(node.GetAttributeValue(nameof(Ep1AIBuildEntry.SampleSizeForFullWeight),"10000"),&target->SampleSizeForFullWeight,state);
        Marshal(node.GetAttributeValue(nameof(Ep1AIBuildEntry.MinTime),"0s"),&target->MinTime,state);
        Marshal(node.GetAttributeValue(nameof(Ep1AIBuildEntry.ForceTime),"99999s"),&target->ForceTime,state);
        Marshal(node.GetAttributeValue(nameof(Ep1AIBuildEntry.BlockIfThingNearFactoryDistance),"150.0"),&target->BlockIfThingNearFactoryDistance,state);
        Marshal(node.GetAttributeValue(nameof(Ep1AIBuildEntry.CountModifier),"1.0"),&target->CountModifier,state);
        Marshal(node.GetAttributeValue(nameof(Ep1AIBuildEntry.Score),"0.0"),&target->Score,state);
        Marshal(node.GetChildNode(nameof(Ep1AIBuildEntry.BlockIfThingNearFactory),null),&target->BlockIfThingNearFactory,state);
    }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: preserve ordered build entries together with the complete independently witnessed EP1 heuristic list. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1AIBuildStateDefinition* target,Tracker state)
    {
        if(node is null)return;
        Marshal(node.GetChildNodes(nameof(Ep1AIBuildStateDefinition.BuildEntry)),&target->BuildEntry,state);
        Marshal(node,(Ep1AIStateDefinition*)target,state);
    }
}
