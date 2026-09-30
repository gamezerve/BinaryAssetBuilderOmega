using Relo;
using SageBinaryData;

public static partial class Marshaler
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: Restore RA3/EP1 contestable garrison status defaults and inherited inline roster. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node, ContestableGarrisonContainModuleData* objT, Tracker state)
    {
        if (node is null)
        {
            return;
        }
        Marshal(node.GetAttributeValue(nameof(ContestableGarrisonContainModuleData.RequiredClearingObjectStatus), ""), &objT->RequiredClearingObjectStatus, state);
        Marshal(node.GetAttributeValue(nameof(ContestableGarrisonContainModuleData.ForbiddenContainerObjectStatus), "UNDER_IRON_CURTAIN"), &objT->ForbiddenContainerObjectStatus, state);
        Marshal(node.GetAttributeValue(nameof(ContestableGarrisonContainModuleData.EjectSpeed), "1.0"), &objT->EjectSpeed, state);
        Marshal(node, (GarrisonContainModuleData*)objT, state);
    }
}
