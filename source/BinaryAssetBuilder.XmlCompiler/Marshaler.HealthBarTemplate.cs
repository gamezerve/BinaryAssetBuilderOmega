
using Relo;
using SageBinaryData;

public static partial class Marshaler
{
    public static unsafe void Marshal(Node node, HealthBarTemplate* objT, Tracker state)
    {
        if (node is null)
            return;

        Marshal(node.GetAttributeValue(nameof(HealthBarTemplate.Length), null), &objT->Length, state);
        Marshal(node, (BaseAssetType*)objT, state);
    }
}
