using Relo;
using SageBinaryData;

public static partial class Marshaler
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: retain the signed attack cap, overflow flags and optional targeting filter on the same Tracker. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node, StrafeAreaTargetChooserModuleData* objT, Tracker state)
    {
        if (node is null)
            return;

        Marshal(node.GetAttributeValue(nameof(StrafeAreaTargetChooserModuleData.Radius), "0"), &objT->Radius, state);
        Marshal(node.GetAttributeValue(nameof(StrafeAreaTargetChooserModuleData.MaxAttacksPerTarget), "-1"), &objT->MaxAttacksPerTarget, state);
        Ep1Flags<StrafeAreaTargetChooserFlagType>(
            node.GetAttributeValue(nameof(StrafeAreaTargetChooserModuleData.OverflowOptions), null),
            objT->OverflowOptions.Value, 2, 1, state);
        Marshal(node.GetChildNode(nameof(StrafeAreaTargetChooserModuleData.ObjectFilter), null), &objT->ObjectFilter, state);
        Marshal(node, (BehaviorModuleData*)objT, state);
    }
}
