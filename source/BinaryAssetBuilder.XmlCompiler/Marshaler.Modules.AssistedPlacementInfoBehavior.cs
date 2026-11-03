using Relo;
using SageBinaryData;

public static partial class Marshaler
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: serialize one placement hint with its optional player template and filter. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,HintObjectMapping* value,Tracker state)
    {
        if(node is null)return;
        Marshal(node.GetAttributeValue("PlayerTemplate",null),&value->PlayerTemplate,state);
        Marshal(node.GetAttributeValue("HintDisplayObject",null),&value->HintDisplayObject,state);
        Marshal(node.GetAttributeValue("HintOpacity","1.0"),&value->HintOpacity,state);
        Marshal(node.GetChildNode("ObjectFilter",null),&value->ObjectFilter,state);
        Marshal(node.GetAttributeValue("HintUsesShaders","false"),&value->HintUsesShaders,state);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: serialize the placement radius and its optional restricted-object filter. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,PlacementRestriction* value,Tracker state)
    {
        if(node is null)return;
        Marshal(node.GetAttributeValue("FilterRadius","0.0"),&value->FilterRadius,state);
        Marshal(node.GetChildNode("RestrictedObjectFilter",null),&value->RestrictedObjectFilter,state);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: allocate the optional placement restriction only when the source declares it. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,PlacementRestriction** value,Tracker state)
    {
        if(node is null)return;
        using var context=state.Push((void**)value,(uint)sizeof(PlacementRestriction),1u);
        Marshal(node,*value,state);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: serialize hints, the optional restriction, the required suggested offset and AllowShrouded. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,AssistedPlacementInfoBehaviorModuleData* value,Tracker state)
    {
        if(node is null)return;
        Marshal(node.GetChildNodes("HintObject"),&value->HintObject,state);
        Marshal(node.GetChildNode("PlacementRestriction",null),&value->PlacementRestriction,state);
        Marshal(node.GetChildNode("SuggestedOffset",null),&value->SuggestedOffset,state);
        Marshal(node.GetAttributeValue("AllowShrouded","true"),&value->AllowShrouded,state);
        Marshal(node,(BehaviorModuleData*)value,state);
    }
}
