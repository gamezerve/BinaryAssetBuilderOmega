using Relo;
using SageBinaryData;

public static partial class Marshaler
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: EP1 structure placement reuses the reviewed EP1 Shape marshaler for its required inline restricted area. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,StructurePlacementBehaviorModuleData* objT,Tracker state)
    {
        if(node is null)return;
        Marshal(node.GetChildNode(nameof(StructurePlacementBehaviorModuleData.RestrictedAreaShape),null),&objT->RestrictedAreaShape,state);
        Marshal(node,(BehaviorModuleData*)objT,state);
    }
}
