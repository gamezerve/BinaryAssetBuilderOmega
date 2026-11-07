using Relo;
using SageBinaryData;
public static partial class Marshaler
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: serialize every original GeneralHotKeySlots reference and preserve absent inline handles. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1GeneralHotKeySlots* target,Tracker state)
    {
        if(node is null)return;
        Marshal(node.GetAttributeValue(nameof(Ep1GeneralHotKeySlots.TakeScreenShot),null),&target->TakeScreenShot,state);
        Marshal(node.GetAttributeValue(nameof(Ep1GeneralHotKeySlots.ReloadRapidIterationFeature),null),&target->ReloadRapidIterationFeature,state);
        Marshal(node,(BaseAssetType*)target,state);
    }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: serialize every original BasicMouseCursors reference and preserve absent inline handles. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1BasicMouseCursors* target,Tracker state)
    {
        if(node is null)return;
        Marshal(node.GetAttributeValue(nameof(Ep1BasicMouseCursors.Normal),null),&target->Normal,state);
        Marshal(node.GetAttributeValue(nameof(Ep1BasicMouseCursors.Arrow),null),&target->Arrow,state);
        Marshal(node.GetAttributeValue(nameof(Ep1BasicMouseCursors.Invalid),null),&target->Invalid,state);
        Marshal(node.GetAttributeValue(nameof(Ep1BasicMouseCursors.EnterText),null),&target->EnterText,state);
        Marshal(node,(BaseAssetType*)target,state);
    }
}
