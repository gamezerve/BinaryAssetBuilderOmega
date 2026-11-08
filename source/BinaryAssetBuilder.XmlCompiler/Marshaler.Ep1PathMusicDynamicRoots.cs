using Relo;
using SageBinaryData;

public static partial class Marshaler
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: reject unknown music enum tokens instead of silently replacing them with zero. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(string value,Ep1MusicStateSetCategory* target,Tracker state)
    {
        if(!System.Enum.TryParse<Ep1MusicStateSetCategory>(value,false,out var parsed)||!System.Enum.IsDefined(parsed))throw new System.IO.InvalidDataException("Invalid Ep1MusicStateSetCategory: "+value);
        *target=parsed;
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: reject unknown music enum tokens instead of silently replacing them with zero. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(string value,Ep1MusicStateChange* target,Tracker state)
    {
        if(!System.Enum.TryParse<Ep1MusicStateChange>(value,false,out var parsed)||!System.Enum.IsDefined(parsed))throw new System.IO.InvalidDataException("Invalid Ep1MusicStateChange: "+value);
        *target=parsed;
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: reject unknown music enum tokens instead of silently replacing them with zero. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(string value,Ep1MusicComparison* target,Tracker state)
    {
        if(!System.Enum.TryParse<Ep1MusicComparison>(value,false,out var parsed)||!System.Enum.IsDefined(parsed))throw new System.IO.InvalidDataException("Invalid Ep1MusicComparison: "+value);
        *target=parsed;
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: reject unknown music enum tokens instead of silently replacing them with zero. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(string value,Ep1MusicScreenStatus* target,Tracker state)
    {
        if(!System.Enum.TryParse<Ep1MusicScreenStatus>(value,false,out var parsed)||!System.Enum.IsDefined(parsed))throw new System.IO.InvalidDataException("Invalid Ep1MusicScreenStatus: "+value);
        *target=parsed;
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: marshal the reviewed PathMusicGameDynamicTransition fields with ordinary reference relocation tracking. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1PathMusicGameDynamicTransition* target,Tracker state)
    {
        if(node is null)return;
        Marshal(node.GetAttributeValue(nameof(Ep1PathMusicGameDynamicTransition.DestinationState),null),&target->DestinationState,state);
        Marshal(node.GetChildNode(nameof(Ep1PathMusicGameDynamicTransition.Condition),null),&target->Condition,state);
        Marshal(node,(BaseAssetType*)target,state);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: marshal the reviewed PathMusicGameDynamicStateSet fields with ordinary reference relocation tracking. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1PathMusicGameDynamicStateSet* target,Tracker state)
    {
        if(node is null)return;
        Marshal(node.GetAttributeValue(nameof(Ep1PathMusicGameDynamicStateSet.Category),null),&target->Category,state);
        Marshal(node.GetChildNodes(nameof(Ep1PathMusicGameDynamicStateSet.State)),&target->State,state);
        Marshal(node,(BaseAssetType*)target,state);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: marshal the reviewed MusicScriptConditionNugget_PathMusicStateLastActive fields with ordinary reference relocation tracking. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1MusicScriptConditionNugget_PathMusicStateLastActive* target,Tracker state)
    {
        if(node is null)return;
        Marshal(node.GetAttributeValue(nameof(Ep1MusicScriptConditionNugget_PathMusicStateLastActive.TrackedState),null),&target->TrackedState,state);
        Marshal(node.GetAttributeValue(nameof(Ep1MusicScriptConditionNugget_PathMusicStateLastActive.TrackedStateChange),null),&target->TrackedStateChange,state);
        Marshal(node.GetAttributeValue(nameof(Ep1MusicScriptConditionNugget_PathMusicStateLastActive.TimeSinceLastActive),null),&target->TimeSinceLastActive,state);
        Marshal(node.GetAttributeValue(nameof(Ep1MusicScriptConditionNugget_PathMusicStateLastActive.TimeSicneLastActiveOp),null),&target->TimeSicneLastActiveOp,state);
        Marshal(node,(MusicScriptConditionNuggetBase*)target,state);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: marshal the reviewed MusicScriptConditionNugget_PathMusicStateCurrentlyActive fields with ordinary reference relocation tracking. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1MusicScriptConditionNugget_PathMusicStateCurrentlyActive* target,Tracker state)
    {
        if(node is null)return;
        Marshal(node.GetAttributeValue(nameof(Ep1MusicScriptConditionNugget_PathMusicStateCurrentlyActive.TrackedState),null),&target->TrackedState,state);
        Marshal(node,(MusicScriptConditionNuggetBase*)target,state);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: marshal the reviewed MusicScriptConditionNugget_EngagementCount fields with ordinary reference relocation tracking. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1MusicScriptConditionNugget_EngagementCount* target,Tracker state)
    {
        if(node is null)return;
        Marshal(node.GetAttributeValue(nameof(Ep1MusicScriptConditionNugget_EngagementCount.Count),null),&target->Count,state);
        Marshal(node.GetAttributeValue(nameof(Ep1MusicScriptConditionNugget_EngagementCount.Time),null),&target->Time,state);
        Marshal(node.GetAttributeValue(nameof(Ep1MusicScriptConditionNugget_EngagementCount.CountOnlyEngagementsAgainstTheLocalPlayer),null),&target->CountOnlyEngagementsAgainstTheLocalPlayer,state);
        Marshal(node.GetAttributeValue(nameof(Ep1MusicScriptConditionNugget_EngagementCount.CountOnlyEngagementsByTheLocalPlayer),null),&target->CountOnlyEngagementsByTheLocalPlayer,state);
        Marshal(node.GetAttributeValue(nameof(Ep1MusicScriptConditionNugget_EngagementCount.RequiredScreenStatus),null),&target->RequiredScreenStatus,state);
        Marshal(node.GetAttributeValue(nameof(Ep1MusicScriptConditionNugget_EngagementCount.TargetFilter),null),&target->TargetFilter,state);
        Marshal(node,(MusicScriptConditionExpensiveNuggetBase*)target,state);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: marshal the reviewed MusicScriptConditionNugget_LocalPlayerKillRatio fields with ordinary reference relocation tracking. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1MusicScriptConditionNugget_LocalPlayerKillRatio* target,Tracker state)
    {
        if(node is null)return;
        Marshal(node.GetAttributeValue(nameof(Ep1MusicScriptConditionNugget_LocalPlayerKillRatio.Time),null),&target->Time,state);
        Marshal(node.GetAttributeValue(nameof(Ep1MusicScriptConditionNugget_LocalPlayerKillRatio.Ratio),null),&target->Ratio,state);
        Marshal(node.GetAttributeValue(nameof(Ep1MusicScriptConditionNugget_LocalPlayerKillRatio.RatioOp),null),&target->RatioOp,state);
        Marshal(node,(MusicScriptConditionExpensiveNuggetBase*)target,state);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: marshal the reviewed MusicScriptConditionNugget_HitBySuperWeapon fields with ordinary reference relocation tracking. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1MusicScriptConditionNugget_HitBySuperWeapon* target,Tracker state)
    {
        if(node is null)return;
        Marshal(node.GetAttributeValue(nameof(Ep1MusicScriptConditionNugget_HitBySuperWeapon.TrackedWeapon),null),&target->TrackedWeapon,state);
        Marshal(node.GetAttributeValue(nameof(Ep1MusicScriptConditionNugget_HitBySuperWeapon.Time),null),&target->Time,state);
        Marshal(node.GetAttributeValue(nameof(Ep1MusicScriptConditionNugget_HitBySuperWeapon.Count),null),&target->Count,state);
        Marshal(node.GetAttributeValue(nameof(Ep1MusicScriptConditionNugget_HitBySuperWeapon.CountOnlyHitsAgainstTheLocalPlayer),null),&target->CountOnlyHitsAgainstTheLocalPlayer,state);
        Marshal(node.GetAttributeValue(nameof(Ep1MusicScriptConditionNugget_HitBySuperWeapon.TargetFilter),null),&target->TargetFilter,state);
        Marshal(node,(MusicScriptConditionNuggetBase*)target,state);
    }

}
