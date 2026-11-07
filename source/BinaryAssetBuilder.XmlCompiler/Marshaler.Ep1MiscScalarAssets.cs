using Relo;
using SageBinaryData;

public static partial class Marshaler
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: compile all official MouseCursor attributes through the separately original-native-qualified EP1 layout. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1MouseCursor* target,Tracker state)
    {
        if(node is null)return;
        Marshal(node.GetAttributeValue(nameof(Ep1MouseCursor.File),null),&target->File,state);
        Marshal(node.GetAttributeValue(nameof(Ep1MouseCursor.NumDirections),"1"),&target->NumDirections,state);
        Marshal(node,(BaseAssetType*)target,state);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: compile all official FontTranslation attributes through the separately original-native-qualified EP1 layout. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1FontTranslation* target,Tracker state)
    {
        if(node is null)return;
        Marshal(node.GetAttributeValue(nameof(Ep1FontTranslation.Name),null),&target->Name,state);
        Marshal(node.GetAttributeValue(nameof(Ep1FontTranslation.TranslateX),null),&target->TranslateX,state);
        Marshal(node.GetAttributeValue(nameof(Ep1FontTranslation.TranslateY),null),&target->TranslateY,state);
        Marshal(node,(BaseAssetType*)target,state);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: compile all official FontDirectory attributes through the separately original-native-qualified EP1 layout. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1FontDirectory* target,Tracker state)
    {
        if(node is null)return;
        Marshal(node.GetAttributeValue(nameof(Ep1FontDirectory.Path),null),&target->Path,state);
        Marshal(node,(BaseAssetType*)target,state);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: compile all official Mouse attributes through the separately original-native-qualified EP1 layout. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1Mouse* target,Tracker state)
    {
        if(node is null)return;
        Marshal(node.GetAttributeValue(nameof(Ep1Mouse.DragTolerance),"48"),&target->DragTolerance,state);
        Marshal(node.GetAttributeValue(nameof(Ep1Mouse.DragThreshold),"100ms"),&target->DragThreshold,state);
        Marshal(node.GetAttributeValue(nameof(Ep1Mouse.InGameUIMaxDoubleClickDistance),"20"),&target->InGameUIMaxDoubleClickDistance,state);
        Marshal(node.GetAttributeValue(nameof(Ep1Mouse.AptHitRegionMaxDoubleClickDistance),"10"),&target->AptHitRegionMaxDoubleClickDistance,state);
        Marshal(node,(BaseAssetType*)target,state);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: compile all official ForceFeedbackEvent attributes through the separately original-native-qualified EP1 layout. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1ForceFeedbackEvent* target,Tracker state)
    {
        if(node is null)return;
        Marshal(node.GetAttributeValue(nameof(Ep1ForceFeedbackEvent.LeftMotorUse),null),&target->LeftMotorUse,state);
        Marshal(node.GetAttributeValue(nameof(Ep1ForceFeedbackEvent.RightMotorUse),null),&target->RightMotorUse,state);
        Marshal(node.GetAttributeValue(nameof(Ep1ForceFeedbackEvent.Duration),null),&target->Duration,state);
        Marshal(node.GetAttributeValue(nameof(Ep1ForceFeedbackEvent.PlayCount),"1"),&target->PlayCount,state);
        Marshal(node,(BaseInheritableAsset*)target,state);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: compile all official AudioEventLimitGroup attributes through the separately original-native-qualified EP1 layout. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1AudioEventLimitGroup* target,Tracker state)
    {
        if(node is null)return;
        Marshal(node.GetAttributeValue(nameof(Ep1AudioEventLimitGroup.Limit),null),&target->Limit,state);
        Marshal(node,(BaseInheritableAsset*)target,state);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: compile all official AnimationSoundSettings attributes through the separately original-native-qualified EP1 layout. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1AnimationSoundSettings* target,Tracker state)
    {
        if(node is null)return;
        Marshal(node.GetAttributeValue(nameof(Ep1AnimationSoundSettings.MinMicrophoneDistanceToDirty),null),&target->MinMicrophoneDistanceToDirty,state);
        Marshal(node,(BaseAssetType*)target,state);
    }

}
