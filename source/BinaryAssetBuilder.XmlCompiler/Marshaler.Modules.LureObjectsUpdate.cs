using Relo;
using SageBinaryData;

public static partial class Marshaler
{
    public static unsafe void Marshal(Node node, LureObjectsUpdateModuleData* objT, Tracker state)
    {
        if (node is null)
        {
            return;
        }
        Marshal(node.GetAttributeValue(nameof(LureObjectsUpdateModuleData.LureObjectLinkID), "0"), &objT->LureObjectLinkID, state);
        Marshal(node.GetAttributeValue(nameof(LureObjectsUpdateModuleData.GuardRadiusMaxSqr), "1000000"), &objT->GuardRadiusMaxSqr, state);
        Marshal(node.GetAttributeValue(nameof(LureObjectsUpdateModuleData.GuardAttackRange), "99999"), &objT->GuardAttackRange, state);
        Marshal(node.GetAttributeValue(nameof(LureObjectsUpdateModuleData.GuardStatus), ""), &objT->GuardStatus, state);
        Marshal(node.GetAttributeValue(nameof(LureObjectsUpdateModuleData.UpdateRate), "10"), &objT->UpdateRate, state);
        Marshal(node.GetAttributeValue(nameof(LureObjectsUpdateModuleData.GuardMarkerObjectRef), null), &objT->GuardMarkerObjectRef, state);
        Marshal(node.GetAttributeValue(nameof(LureObjectsUpdateModuleData.DisabledTypesToProcess), ""), &objT->DisabledTypesToProcess, state);
        Marshal(node.GetChildNodes(nameof(LureObjectsUpdateModuleData.GuardOffset)), &objT->GuardOffset, state);
        Marshal(node, (UpdateModuleData*)objT, state);
    }
}
