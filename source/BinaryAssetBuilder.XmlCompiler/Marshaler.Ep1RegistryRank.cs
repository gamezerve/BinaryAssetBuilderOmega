using Relo;
using SageBinaryData;
public static partial class Marshaler
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: encode the original EP1 registry identity without legacy KW ordinal drift. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Value value,Ep1RegisteredObjectSetId* target,Tracker state)
        =>Ep1ToggleEnum(value,(uint*)target,state,typeof(Ep1RegisteredObjectSetId),false);
    //-------------------------------------------------------------------------------------------------
    /** Reborn: preserve the original global/player ownership enum. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Value value,Ep1RegisteredObjectSetType* target,Tracker state)
        =>Ep1ToggleEnum(value,(uint*)target,state,typeof(Ep1RegisteredObjectSetType),false);
    //-------------------------------------------------------------------------------------------------
    /** Reborn: compile the complete source-authored registry filter and both required enumeration fields. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1RegisteredObjectSet* target,Tracker state)
    {
        if(node is null)return;
        Marshal(node.GetAttributeValue(nameof(Ep1RegisteredObjectSet.SetId),null),&target->SetId,state);
        Marshal(node.GetAttributeValue(nameof(Ep1RegisteredObjectSet.Type),null),&target->Type,state);
        Marshal(node.GetChildNode(nameof(Ep1RegisteredObjectSet.ObjectFilter),null),&target->ObjectFilter,state);
        Marshal(node,(BaseAssetType*)target,state);
    }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: compile the authored rank thresholds, tech-point grant and ordered strong tech occurrences. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1PlayerTechRank* target,Tracker state)
    {
        if(node is null)return;
        Marshal(node.GetAttributeValue(nameof(Ep1PlayerTechRank.ExperiencePointsToNextRank),"0"),&target->ExperiencePointsToNextRank,state);
        Marshal(node.GetAttributeValue(nameof(Ep1PlayerTechRank.TechPointsGranted),"0"),&target->TechPointsGranted,state);
        Marshal(node.GetChildNodes(nameof(Ep1PlayerTechRank.TechGranted)),&target->TechGranted,state);
        Marshal(node,(BaseAssetType*)target,state);
    }
}
