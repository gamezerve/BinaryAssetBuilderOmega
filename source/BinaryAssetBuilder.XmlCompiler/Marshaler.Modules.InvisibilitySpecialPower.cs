using Relo;
using SageBinaryData;

public static partial class Marshaler
{
    public static unsafe void Marshal(Node node, InvisibilitySpecialPowerModuleData* objT, Tracker state)
    {
        if (node is null)
        {
            return;
        }
        // Reborn: marshal the official RA3/Uprising template, duration, permanence, and filter fields.
        Marshal(node.GetAttributeValue(nameof(InvisibilitySpecialPowerModuleData.InvisibilityTemplate), null), &objT->InvisibilityTemplate, state);
        Marshal(node.GetAttributeValue(nameof(InvisibilitySpecialPowerModuleData.BroadcastRadius), "0"), &objT->BroadcastRadius, state);
        Marshal(node.GetAttributeValue(nameof(InvisibilitySpecialPowerModuleData.Duration), "0s"), &objT->Duration, state);
        Marshal(node.GetAttributeValue(nameof(InvisibilitySpecialPowerModuleData.Permanent), "false"), &objT->Permanent, state);
        Marshal(node.GetChildNode(nameof(InvisibilitySpecialPowerModuleData.ObjectFilter), null), &objT->ObjectFilter, state);
        Marshal(node, (SpecialPowerModuleData*)objT, state);
    }
}
