using Relo;
using SageBinaryData;

public static partial class Marshaler
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: Restore money-gain attach fields, percentage default and optional by-pointer validation. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node, MoneyGainAttachUpdateModuleData* objT, Tracker state)
    {
        if (node is null)
        {
            return;
        }
        Marshal(node.GetAttributeValue(nameof(MoneyGainAttachUpdateModuleData.ActionType), null), &objT->ActionType, state);
        Marshal(node.GetAttributeValue(nameof(MoneyGainAttachUpdateModuleData.PurchasePricePercent), "100%"), &objT->PurchasePricePercent, state);
        Marshal(node.GetChildNode(nameof(MoneyGainAttachUpdateModuleData.MoneyGainObjectStatusValidation), null), &objT->MoneyGainObjectStatusValidation, state);
        Marshal(node, (AttachUpdateModuleData*)objT, state);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: allocate the complete Uprising attachment graph and retain the original wire ID. */
    //-------------------------------------------------------------------------------------------------
    private static unsafe void MarshalEp1MoneyGainAttachUpdate(Node node, BehaviorModuleData** objT, Tracker state)
    {
        using var allocation = state.Push((void**)objT, (uint)sizeof(Ep1MoneyGainAttachUpdateModuleData), 1u);
        var data = (Ep1MoneyGainAttachUpdateModuleData*)*objT;
        Marshal(node.GetAttributeValue(nameof(Ep1MoneyGainAttachUpdateModuleData.ActionType), null), &data->ActionType, state);
        Marshal(node.GetAttributeValue(nameof(Ep1MoneyGainAttachUpdateModuleData.PurchasePricePercent), "100%"), &data->PurchasePricePercent, state);
        Marshal(node.GetChildNode(nameof(Ep1MoneyGainAttachUpdateModuleData.MoneyGainObjectStatusValidation), null), &data->MoneyGainObjectStatusValidation, state);
        Marshal(node, &data->Base, state);
        uint wire = 0xB1A54585u;
        state.InplaceEndianToPlatform(&wire);
        *(uint*)data = wire;
    }
}
