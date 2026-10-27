using Relo;
using SageBinaryData;
public static partial class Marshaler
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: compile the required EP1 invisibility-template import and the authored local module identity through the existing behavior header writer. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,InvisibilityBehaviorModuleData* value,Tracker state)
    {
        if(node is null)return;
        Marshal(node.GetAttributeValue("InvisibilityTemplate",null),&value->InvisibilityTemplate,state);
        Marshal(node,(BehaviorModuleData*)value,state);
    }
}
