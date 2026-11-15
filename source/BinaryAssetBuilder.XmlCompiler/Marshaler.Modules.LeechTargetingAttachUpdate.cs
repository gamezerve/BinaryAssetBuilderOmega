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

    //-------------------------------------------------------------------------------------------------
    /** Reborn: dispatch the EP1 Leech class through the shared, stock-qualified EP1 Attach writer. */
    //-------------------------------------------------------------------------------------------------
    private static unsafe void MarshalEp1LeechTargeting(Node node, BehaviorModuleData** destination, Tracker state)
    {
        using var context = state.Push((void**)destination, (uint)sizeof(Ep1LeechTargetingAttachUpdateModuleData), 1u);
        var result = (Ep1LeechTargetingAttachUpdateModuleData*)*destination;
        Marshal(node, &result->Base, state);
        uint wire = Ep1LeechTargetingAttachUpdateModuleData.WireTypeId;
        state.InplaceEndianToPlatform(&wire);
        *(uint*)result = wire;
    }
}
