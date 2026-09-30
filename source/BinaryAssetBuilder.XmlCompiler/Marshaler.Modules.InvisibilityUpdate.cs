using Relo;
using SageBinaryData;

public static partial class Marshaler
{
    public static unsafe void Marshal(Node node, InvisibilityUpdateModuleData* objT, Tracker state)
    {
        if (node is null)
        {
            return;
        }
        // Reborn: marshal the shared RA3/Uprising template-based invisibility contract.
        Marshal(node.GetAttributeValue(nameof(InvisibilityUpdateModuleData.InvisibilityTemplate), null), &objT->InvisibilityTemplate, state);
        Marshal(node.GetAttributeValue(nameof(InvisibilityUpdateModuleData.UpdatePeriod), "1s"), &objT->UpdatePeriod, state);
        Marshal(node.GetAttributeValue(nameof(InvisibilityUpdateModuleData.RequiredNearbyObjectRange), "0"), &objT->RequiredNearbyObjectRange, state);
        Marshal(node.GetAttributeValue(nameof(InvisibilityUpdateModuleData.NamedVoiceNameToUseAsVoiceMoveToStealthyArea), null), &objT->NamedVoiceNameToUseAsVoiceMoveToStealthyArea, state);
        Marshal(node.GetAttributeValue(nameof(InvisibilityUpdateModuleData.NamedVoiceNameToUseAsVoiceEnterStateMoveToStealthyArea), null), &objT->NamedVoiceNameToUseAsVoiceEnterStateMoveToStealthyArea, state);
        Marshal(node.GetChildNode(nameof(InvisibilityUpdateModuleData.RequiresNearbyObjectFilter), null), &objT->RequiresNearbyObjectFilter, state);
        Marshal(node, (UpdateModuleData*)objT, state);
    }
}
