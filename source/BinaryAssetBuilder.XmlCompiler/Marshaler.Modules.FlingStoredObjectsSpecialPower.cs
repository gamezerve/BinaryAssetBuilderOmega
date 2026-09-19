using Relo;
using SageBinaryData;

public static partial class Marshaler
{
    public static unsafe void Marshal(Node node, FlingStoredObjectsObjectMap* objT, Tracker state)
    {
        if (node is null)
        {
            return;
        }
        Marshal(node.GetAttributeValue(nameof(FlingStoredObjectsObjectMap.Source), null), &objT->Source, state);
        Marshal(node.GetAttributeValue(nameof(FlingStoredObjectsObjectMap.Target), null), &objT->Target, state);
    }

    public static unsafe void Marshal(Node node, FlingStoredObjectsSpecialPowerModuleData* objT, Tracker state)
    {
        if (node is null)
        {
            return;
        }
        Marshal(node.GetAttributeValue(nameof(FlingStoredObjectsSpecialPowerModuleData.StoreObjectsLinkID), "0"), &objT->StoreObjectsLinkID, state);
        Marshal(node.GetAttributeValue(nameof(FlingStoredObjectsSpecialPowerModuleData.LiftObjectLinkID), "0"), &objT->LiftObjectLinkID, state);
        Marshal(node.GetAttributeValue(nameof(FlingStoredObjectsSpecialPowerModuleData.MaximumVelocity), "300.0"), &objT->MaximumVelocity, state);
        Marshal(node.GetAttributeValue(nameof(FlingStoredObjectsSpecialPowerModuleData.MinimumVelocity), "100.0"), &objT->MinimumVelocity, state);
        Marshal(node.GetChildNodes(nameof(FlingStoredObjectsSpecialPowerModuleData.ObjectMap)), &objT->ObjectMap, state);
        Marshal(node, (SpecialPowerModuleData*)objT, state);
    }
}
