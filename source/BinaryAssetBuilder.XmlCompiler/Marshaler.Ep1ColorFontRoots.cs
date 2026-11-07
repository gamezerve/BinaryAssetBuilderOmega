using Relo;
using SageBinaryData;
public static partial class Marshaler
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: compile all three original color slots, localized tooltip and availability byte without KW-only fields. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1MultiplayerColor* target,Tracker state)
    {
        if(node is null)return;
        Marshal(node.GetChildNode(nameof(Ep1MultiplayerColor.RGBColor),null),&target->RGBColor,state);
        Marshal(node.GetChildNode(nameof(Ep1MultiplayerColor.RGBColorT),null),&target->RGBColorT,state);
        Marshal(node.GetChildNode(nameof(Ep1MultiplayerColor.RGBNightColor),null),&target->RGBNightColor,state);
        Marshal(node.GetChildNode(nameof(Ep1MultiplayerColor.TooltipName),null),&target->TooltipName,state);
        Marshal(node.GetChildNode(nameof(Ep1MultiplayerColor.AvailableInMetaGame),null),&target->AvailableInMetaGame,state);
        Marshal(node,(BaseAssetType*)target,state);
    }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: serialize the header's original font name, size and bold flag with existing FontDesc storage. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1HeaderTemplate* target,Tracker state)
    {
        if(node is null)return;
        Marshal(node.GetChildNode(nameof(Ep1HeaderTemplate.Font),null),&target->Font,state);
        Marshal(node,(BaseAssetType*)target,state);
    }
}
