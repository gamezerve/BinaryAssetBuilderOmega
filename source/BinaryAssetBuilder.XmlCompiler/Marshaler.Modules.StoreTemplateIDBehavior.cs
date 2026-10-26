using Relo;
using SageBinaryData;

public static partial class Marshaler
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: retain the authored module ID through the complete inherited behavior writer without injecting template IDs or payload fields. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node, StoreTemplateIDBehaviorModuleData* objT, Tracker state)
    {
        if(node is null)return;
        Marshal(node,(BehaviorModuleData*)objT,state);
    }
}
