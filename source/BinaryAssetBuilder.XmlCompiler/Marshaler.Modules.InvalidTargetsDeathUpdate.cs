using Relo;
using SageBinaryData;

public static partial class Marshaler
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: retain the optional invalid-status mask and original inherited update fields. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node, InvalidTargetsDeathUpdateModuleData* objT, Tracker state)
    {
        if (node is null)
        {
            return;
        }
        Marshal(node.GetAttributeValue(nameof(InvalidTargetsDeathUpdateModuleData.InvalidStatus),null),&objT->InvalidStatus,state);
        Marshal(node, (UpdateModuleData*)objT, state);
    }
}
