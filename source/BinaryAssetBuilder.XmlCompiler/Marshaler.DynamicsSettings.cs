using Relo;
using SageBinaryData;

public static partial class Marshaler
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: Compile the EP1 dynamics capacity settings with the schema-defined native defaults. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node, DynamicsSettings* objT, Tracker state)
    {
        if (node is null)
        {
            return;
        }
        Marshal(node.GetAttributeValue(nameof(DynamicsSettings.MaximumObjects), "1024"), &objT->MaximumObjects, state);
        Marshal(node.GetAttributeValue(nameof(DynamicsSettings.MaximumContacts), "2048"), &objT->MaximumContacts, state);
        Marshal(node.GetAttributeValue(nameof(DynamicsSettings.MaximumContactPairs), "1024"), &objT->MaximumContactPairs, state);
        Marshal(node.GetAttributeValue(nameof(DynamicsSettings.MaximumJoints), "256"), &objT->MaximumJoints, state);
        Marshal(node.GetAttributeValue(nameof(DynamicsSettings.CreateGlobalIsland), "false"), &objT->CreateGlobalIsland, state);
        Marshal(node, (BaseInheritableAsset*)objT, state);
    }
}
