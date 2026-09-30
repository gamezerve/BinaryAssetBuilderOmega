using Relo;
using SageBinaryData;

public static partial class Marshaler
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: Restore the leech targeting module by compiling its inherited attach-update data. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node, LeechTargetingAttachUpdateModuleData* objT, Tracker state)
    {
        if (node is null)
        {
            return;
        }
        Marshal(node, (AttachUpdateModuleData*)objT, state);
    }
}
