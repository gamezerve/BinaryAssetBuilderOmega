using Relo;
using SageBinaryData;

public static partial class Marshaler
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: retain the complete inherited behavior header for the original EP1 substructure marker. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,SubStructureBehaviorModuleData* target,Tracker state)
    {
        if(node is null)return;
        Marshal(node,(BehaviorModuleData*)target,state);
    }
}
