using Relo;
using SageBinaryData;

public static partial class Marshaler
{
    public static unsafe void Marshal(Node node, StoreObjectsSpecialPowerModuleData* objT, Tracker state)
    {
        if (node is null)
        {
            return;
        }
        Marshal(node.GetAttributeValue(nameof(StoreObjectsSpecialPowerModuleData.Radius), "0"), &objT->Radius, state);
        Marshal(node.GetAttributeValue(nameof(StoreObjectsSpecialPowerModuleData.TeleportLinkID), "0"), &objT->TeleportLinkID, state);
        Marshal(node.GetAttributeValue(nameof(StoreObjectsSpecialPowerModuleData.OCL), null), &objT->OCL, state);
        Marshal(node.GetAttributeValue(nameof(StoreObjectsSpecialPowerModuleData.TargetMarkerObjectRef), null), &objT->TargetMarkerObjectRef, state);
        Marshal(node, (SpecialPowerModuleData*)objT, state);
    }

    public static unsafe void Marshal(Node node, AddObjectsToLiftUpdateSpecialPowerModuleData* objT, Tracker state)
    {
        if (node is null)
        {
            return;
        }
        Marshal(node.GetAttributeValue(nameof(AddObjectsToLiftUpdateSpecialPowerModuleData.LiftObjectLinkID), "0"), &objT->LiftObjectLinkID, state);
        Marshal(node, (StoreObjectsSpecialPowerModuleData*)objT, state);
    }

    public static unsafe void Marshal(Node node, AddObjectsToLureUpdateSpecialPowerModuleData* objT, Tracker state)
    {
        if (node is null)
        {
            return;
        }
        Marshal(node.GetAttributeValue(nameof(AddObjectsToLureUpdateSpecialPowerModuleData.LureObjectLinkID), "0"), &objT->LureObjectLinkID, state);
        Marshal(node, (StoreObjectsSpecialPowerModuleData*)objT, state);
    }
}
