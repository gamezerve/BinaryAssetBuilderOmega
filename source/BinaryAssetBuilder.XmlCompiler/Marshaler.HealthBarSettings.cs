
using Relo;
using SageBinaryData;

public static partial class Marshaler
{
    // Reborn: mapping HealthBarTemplateRef fields must populate the IMP table used by the runtime linker.
    public static unsafe void Marshal(Node node, HealthBarObjectMapping* objT, Tracker state)
    {
        if (node is null)
            return;

        Marshal(node.GetAttributeValue(nameof(HealthBarObjectMapping.HealthBar), null), &objT->HealthBar, state);
        Marshal(node.GetChildNode(nameof(HealthBarObjectMapping.ObjectFilter), null), &objT->ObjectFilter, state);
    }

    // Reborn: marshal the retail EP1 header order and keep repeated strong mapping references distinct.
    public static unsafe void Marshal(Node node, HealthBarSettings* objT, Tracker state)
    {
        if (node is null)
            return;

        Marshal(node.GetAttributeValue(nameof(HealthBarSettings.DefaultHealthBar), null), &objT->DefaultHealthBar, state);
        Marshal(node.GetChildNodes(nameof(HealthBarSettings.ObjectMapping)), &objT->ObjectMapping, state);
        Marshal(node, (BaseAssetType*)objT, state);
    }
}
