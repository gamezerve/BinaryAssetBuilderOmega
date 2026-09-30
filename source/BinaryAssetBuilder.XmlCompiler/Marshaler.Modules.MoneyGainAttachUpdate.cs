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
}
