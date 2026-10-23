using Relo;
using SageBinaryData;

public static partial class Marshaler
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: EP1 structure unpack reuses all GenericUnpack fields instead of dropping the inherited water-height value. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1StructureUnpackUpdateModuleData* objT,Tracker state)
    {
        if(node is null)return;
        Marshal(node,(GenericUnpackUpdateModuleData*)objT,state);
    }
}
