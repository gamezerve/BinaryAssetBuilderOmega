using Relo;
using SageBinaryData;

public static partial class Marshaler
{
    public static unsafe void Marshal(Node node, ReplaceSelfUpgradeModuleData* objT, Tracker state)
    {
        if (node is null)
        {
            return;
        }
        Marshal(node.GetAttributeValue(nameof(ReplaceSelfUpgradeModuleData.NewObjectUnpackTime), "0s"), &objT->NewObjectUnpackTime, state);
        Marshal(node.GetAttributeValue(nameof(ReplaceSelfUpgradeModuleData.DisabledDuringUnpack), "true"), &objT->DisabledDuringUnpack, state);
        Marshal(node.GetAttributeValue(nameof(ReplaceSelfUpgradeModuleData.CheckBuildAssistant), "false"), &objT->CheckBuildAssistant, state);
        Marshal(node.GetChildNodes(nameof(ReplaceSelfUpgradeModuleData.ReplacementTemplate)), &objT->ReplacementTemplate, state);
        Marshal(node, (UpgradeModuleData*)objT, state);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: emit the EP1 replacement mask, unpack timing and ordered weak GameObject IDs. */
    //-------------------------------------------------------------------------------------------------
    private static unsafe void Marshal(Node node, Ep1ReplaceSelfUpgradeModuleData* objT, Tracker state)
    {
        Marshal(node.GetAttributeValue("NewObjectUnpackTime", "0s"), &objT->NewObjectUnpackTime, state);
        Ep1ToggleEnum(node.GetAttributeValue("Options", "DISABLE_DURING_UNPACK"), objT->Options.Value, state, typeof(ReplaceSelfUpgradeOptions), true);
        Marshal(node.GetChildNodes("ReplacementTemplate"), &objT->ReplacementTemplate, state);
        Marshal(node, &objT->Base, state);
    }
}
