using Relo;
using SageBinaryData;

public static partial class Marshaler
{
    public static unsafe void Marshal(Node node, EMPUpdateModuleData* objT, Tracker state)
    {
        if (node is null)
        {
            return;
        }
        Marshal(node.GetAttributeValue(nameof(EMPUpdateModuleData.Lifetime), "0s"), &objT->Lifetime, state);
        Marshal(node.GetAttributeValue(nameof(EMPUpdateModuleData.StartFadeDelay), "0s"), &objT->StartFadeDelay, state);
        Marshal(node.GetAttributeValue(nameof(EMPUpdateModuleData.DisabledDuration), "0s"), &objT->DisabledDuration, state);
        Marshal(node.GetAttributeValue(nameof(EMPUpdateModuleData.StartScale), "1"), &objT->StartScale, state);
        Marshal(node.GetAttributeValue(nameof(EMPUpdateModuleData.TargetScaleMin), "1"), &objT->TargetScaleMin, state);
        Marshal(node.GetAttributeValue(nameof(EMPUpdateModuleData.TargetScaleMax), "1"), &objT->TargetScaleMax, state);
        Marshal(node.GetAttributeValue(nameof(EMPUpdateModuleData.DisableFXParticleSystem), null), &objT->DisableFXParticleSystem, state);
        Marshal(node.GetAttributeValue(nameof(EMPUpdateModuleData.SparksPerCubicFoot), "0"), &objT->SparksPerCubicFoot, state);
        Marshal(node.GetChildNode(nameof(EMPUpdateModuleData.StartColor), null), &objT->StartColor, state);
        Marshal(node.GetChildNode(nameof(EMPUpdateModuleData.EndColor), null), &objT->EndColor, state);
        Marshal(node, (UpdateModuleData*)objT, state);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: emit optional color allocations at +0x20/+0x24 and retain the concrete EMP class. */
    //-------------------------------------------------------------------------------------------------
    private static unsafe void MarshalEp1EMPUpdate(Node node, BehaviorModuleData** destination, Tracker state)
    {
        using var context = state.Push((void**)destination, (uint)sizeof(Ep1EMPUpdateModuleData), 1u);
        var result = (Ep1EMPUpdateModuleData*)*destination;
        Marshal(node, &result->Base, state);
        Marshal(node.GetAttributeValue("Lifetime", "0s"), &result->Lifetime, state);
        Marshal(node.GetAttributeValue("StartFadeDelay", "0s"), &result->StartFadeDelay, state);
        Marshal(node.GetAttributeValue("DisabledDuration", "0s"), &result->DisabledDuration, state);
        Marshal(node.GetAttributeValue("StartScale", "1"), &result->StartScale, state);
        Marshal(node.GetAttributeValue("TargetScaleMin", "1"), &result->TargetScaleMin, state);
        Marshal(node.GetAttributeValue("TargetScaleMax", "1"), &result->TargetScaleMax, state);
        Marshal(node.GetChildNode("StartColor", null), &result->StartColor, state);
        Marshal(node.GetChildNode("EndColor", null), &result->EndColor, state);
        uint wire = Ep1EMPUpdateModuleData.Wire;
        state.InplaceEndianToPlatform(&wire);
        *(uint*)result = wire;
    }
}
