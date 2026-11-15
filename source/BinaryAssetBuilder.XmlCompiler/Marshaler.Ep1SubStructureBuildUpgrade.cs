using Relo;
using SageBinaryData;

public static partial class Marshaler
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: compile the named SubStructureBuild element with both ordered lists and its Upgrade base. */
    //-------------------------------------------------------------------------------------------------
    private static unsafe void MarshalEp1SubStructureBuild(Node node, BehaviorModuleData** destination, Tracker state)
    {
        using var context = state.Push((void**)destination, (uint)sizeof(Ep1SubStructureBuildUpgradeModuleData), 1u);
        var result = (Ep1SubStructureBuildUpgradeModuleData*)*destination;
        Marshal(node, &result->Base, state);
        Marshal(node.GetChildNodes("AvailableSubStructure"), &result->AvailableSubStructure, state);
        Marshal(node.GetChildNodes("StructureOffset"), &result->StructureOffset, state);
        uint wire = Ep1SubStructureBuildUpgradeModuleData.WireTypeId;
        state.InplaceEndianToPlatform(&wire);
        *(uint*)result = wire;
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: keep both substructure identifiers weak. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node, Ep1AvailableSubStructure* result, Tracker state)
    {
        Marshal(node.GetChildNode("UpgradeTrigger", null), &result->UpgradeTrigger, state);
        Marshal(node.GetChildNode("ObjectTemplate", null), &result->ObjectTemplate, state);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: write the complete model-condition mask followed by the inline offset vector. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node, Ep1SubStructureOffset* result, Tracker state)
    {
        Marshal(node.GetAttributeValue("ModelCondition", null), &result->ModelCondition, state);
        Marshal(node.GetChildNode("Offset", null), &result->Offset, state);
    }
}
