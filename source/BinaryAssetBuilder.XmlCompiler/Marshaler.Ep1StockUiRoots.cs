using System;
using Relo;
using SageBinaryData;

public static partial class Marshaler
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: marshal the hotkey group's official ordinal and retain the schema defaults for absent slot fields. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1HotKeySlot* target,Tracker state)
    {
        if(node is null)return;
        target->Group=(Ep1HotKeySlotGroup)Enum.Parse(typeof(Ep1HotKeySlotGroup),node.GetAttributeValue(nameof(Ep1HotKeySlot.Group),"Misc").GetText());
        state.InplaceEndianToPlatform((uint*)&target->Group);
        Marshal(node.GetAttributeValue(nameof(Ep1HotKeySlot.Index),"-1"),&target->Index,state);
        Marshal(node.GetAttributeValue(nameof(Ep1HotKeySlot.Name),null),&target->Name,state);
        Marshal(node.GetAttributeValue(nameof(Ep1HotKeySlot.VersionAdded),"1"),&target->VersionAdded,state);
        Marshal(node,(BaseInheritableAsset*)target,state);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: write the localized message and Core-resolved audio occurrence without embedding audio samples. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1UIQuickChatMessage* target,Tracker state)
    {
        if(node is null)return;
        Marshal(node.GetAttributeValue(nameof(Ep1UIQuickChatMessage.MessageText),null),&target->MessageText,state);
        Marshal(node.GetAttributeValue(nameof(Ep1UIQuickChatMessage.Sound),null),&target->Sound,state);
        Marshal(node,(BaseAssetType*)target,state);
    }
}
