using Relo;
using SageBinaryData;

public static partial class Marshaler
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: serialize a prop body's authored local identity through the inherited BodyModuleData writer. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node, PropBodyModuleData* objT, Tracker state)
    {
        if(node is null)return;
        Marshal(node,(BodyModuleData*)objT,state);
    }
}
