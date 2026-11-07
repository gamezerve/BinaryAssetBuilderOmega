using Relo;
using SageBinaryData;

public static partial class Marshaler
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: serialize the original global EVA scan distances and jump-event lifetimes. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1EvaSettings* target,Tracker state)
    {
        if(node is null)return;
        Marshal(node.GetAttributeValue(nameof(Ep1EvaSettings.EnemySightedMaxVoicePositionScanRange),null),&target->EnemySightedMaxVoicePositionScanRange,state);
        Marshal(node.GetAttributeValue(nameof(Ep1EvaSettings.MaxTimeToKeepJumpToEvents),null),&target->MaxTimeToKeepJumpToEvents,state);
        Marshal(node.GetAttributeValue(nameof(Ep1EvaSettings.MaxTimeBeforeResettingLastJumpTo),null),&target->MaxTimeBeforeResettingLastJumpTo,state);
        Marshal(node.GetAttributeValue(nameof(Ep1EvaSettings.MinDistanceBetweenJumpToEvents),null),&target->MinDistanceBetweenJumpToEvents,state);
        Marshal(node,(BaseAssetType*)target,state);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: link the resolved voice event and hash the authored faction as a weak player identity. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1EvaSideSound* target,Tracker state)
    {
        if(node is null)return;
        Marshal(node.GetAttributeValue(nameof(Ep1EvaSideSound.Sound),null),&target->Sound,state);
        Marshal(node.GetAttributeValue(nameof(Ep1EvaSideSound.Side),null),&target->Side,state);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: compile ordered faction sounds, blocking identities, timings and platform flags with independent native proof. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1EvaEvent* target,Tracker state)
    {
        if(node is null)return;
        Marshal(node.GetAttributeValue(nameof(Ep1EvaEvent.Priority),"0"),&target->Priority,state);
        Marshal(node.GetAttributeValue(nameof(Ep1EvaEvent.TimeBetweenEvents),"0s"),&target->TimeBetweenEvents,state);
        Marshal(node.GetAttributeValue(nameof(Ep1EvaEvent.ExpirationTime),"2s"),&target->ExpirationTime,state);
        Marshal(node.GetAttributeValue(nameof(Ep1EvaEvent.QuietTime),"0s"),&target->QuietTime,state);
        Marshal(node.GetAttributeValue(nameof(Ep1EvaEvent.TimeToWaitBeforePlaying),"0s"),&target->TimeToWaitBeforePlaying,state);
        Marshal(node.GetChildNodes(nameof(Ep1EvaEvent.SideSound)),&target->SideSound,state);
        Marshal(node.GetChildNodes(nameof(Ep1EvaEvent.OtherEvaEventToBlock)),&target->OtherEvaEventToBlock,state);
        Marshal(node.GetAttributeValue(nameof(Ep1EvaEvent.AlwaysPlayFromHomeBase),"false"),&target->AlwaysPlayFromHomeBase,state);
        Marshal(node.GetAttributeValue(nameof(Ep1EvaEvent.CountAsJumpToLocation),"true"),&target->CountAsJumpToLocation,state);
        Marshal(node.GetAttributeValue(nameof(Ep1EvaEvent.SuppressOn360),"false"),&target->SuppressOn360,state);
        Marshal(node.GetAttributeValue(nameof(Ep1EvaEvent.SuppressOnPC),"false"),&target->SuppressOnPC,state);
        Marshal(node,(BaseInheritableAsset*)target,state);
    }
}
