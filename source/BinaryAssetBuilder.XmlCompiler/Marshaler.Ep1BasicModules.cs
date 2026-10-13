using Relo;
using SageBinaryData;
using System.Globalization;

public static partial class Marshaler
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: the required SMC helper has no own fields; preserve its existing helper/update/base identity. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node, ObjectSMCHelperModuleData* objT, Tracker state)
    {
        if (node is null) return;
        Marshal(node, (ObjectHelperModuleData*)objT, state);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: marshal the EP1 structure-sell time/condition after the existing update base. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node, StructureSellUpdateModuleData* objT, Tracker state)
    {
        if (node is null) return;
        Marshal(node.GetAttributeValue(nameof(StructureSellUpdateModuleData.SellTime), "0s"), &objT->SellTime, state);
        Marshal(node.GetAttributeValue(nameof(StructureSellUpdateModuleData.ActiveModelCondition), null), &objT->ActiveModelCondition, state);
        Marshal(node, (UpdateModuleData*)objT, state);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: keep EP1 client audio frame values literal and marshal ordered StringHash keys without changing the legacy audio update type. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node, LargeGroupAudioClientUpdateModuleData* objT, Tracker state)
    {
        if (node is null) return;
        MarshalEp1ClientAudioFrame(node.GetAttributeValue(nameof(LargeGroupAudioClientUpdateModuleData.FramesBetweenUpdatesMin), "3"), &objT->FramesBetweenUpdatesMin, state);
        MarshalEp1ClientAudioFrame(node.GetAttributeValue(nameof(LargeGroupAudioClientUpdateModuleData.FramesBetweenUpdatesVariation), "2"), &objT->FramesBetweenUpdatesVariation, state);
        Marshal(node.GetAttributeValue(nameof(LargeGroupAudioClientUpdateModuleData.UnitWeight), "1"), &objT->UnitWeight, state);
        Marshal(node.GetChildNodes(nameof(LargeGroupAudioClientUpdateModuleData.Key)), &objT->Key, state);
        Marshal(node, (ClientUpdateModuleData*)objT, state);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: this bounded EP1 frame-counter path refuses fractional/suffixed input; do not alter the legacy general uint/Duration conversions. */
    //-------------------------------------------------------------------------------------------------
    private static unsafe void MarshalEp1ClientAudioFrame(Value value, uint* objT, Tracker state)
    {
        uint frames = uint.Parse(value.GetText(), NumberStyles.None, CultureInfo.InvariantCulture);
        state.InplaceEndianToPlatform(&frames);
        *objT = frames;
    }
}
