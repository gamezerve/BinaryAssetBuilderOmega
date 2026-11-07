using Relo;
using SageBinaryData;

public static partial class Marshaler
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: write the authored video identity and Core-resolved intro voice occurrence. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1UnitIntro* target,Tracker state)
    {
        if(node is null)return;
        Marshal(node.GetAttributeValue(nameof(Ep1UnitIntro.Video),null),&target->Video,state);
        Marshal(node.GetAttributeValue(nameof(Ep1UnitIntro.Audio),null),&target->Audio,state);
        Marshal(node,(BaseAssetType*)target,state);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: parse only Uprising's explicitly named option bits into its one-word native flag mask. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Value value,Ep1InvisibilityOptionsBitFlags* target,Tracker state)
        =>Ep1ToggleEnum(value,(uint*)target,state,typeof(Ep1InvisibilityOptions),true);

    //-------------------------------------------------------------------------------------------------
    /** Reborn: serialize the complete EP1 invisibility template with inline masks, FX imports and ordered weak upgrades. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1InvisibilityTemplate* target,Tracker state)
    {
        if(node is null)return;
        Marshal(node.GetAttributeValue(nameof(Ep1InvisibilityTemplate.ForbiddenConditions),"PREATTACK_A FIRING_A FIRING_OR_PREATTACK_A RELOADING_A USING_WEAPON_A PREATTACK_B FIRING_B FIRING_OR_PREATTACK_B RELOADING_B USING_WEAPON_B PREATTACK_C FIRING_C FIRING_OR_PREATTACK_C RELOADING_C USING_WEAPON_C"),&target->ForbiddenConditions,state);
        Marshal(node.GetAttributeValue(nameof(Ep1InvisibilityTemplate.ForbiddenConditionExceptions),""),&target->ForbiddenConditionExceptions,state);
        Marshal(node.GetAttributeValue(nameof(Ep1InvisibilityTemplate.ForbiddenStatus),"IS_FIRING_WEAPON IS_AIMING_WEAPON SPECIAL_ABILITY_PACKING_UNPACKING_OR_USING USING_ABILITY"),&target->ForbiddenStatus,state);
        Marshal(node.GetAttributeValue(nameof(Ep1InvisibilityTemplate.CamouflageLevel),null),&target->CamouflageLevel,state);
        Marshal(node.GetAttributeValue(nameof(Ep1InvisibilityTemplate.InvisibilityType),"STEALTH"),&target->InvisibilityType,state);
        Marshal(node.GetAttributeValue(nameof(Ep1InvisibilityTemplate.Options),null),&target->Options,state);
        Marshal(node.GetAttributeValue(nameof(Ep1InvisibilityTemplate.EnteringStealthFX),null),&target->EnteringStealthFX,state);
        Marshal(node.GetAttributeValue(nameof(Ep1InvisibilityTemplate.LeavingStealthFX),null),&target->LeavingStealthFX,state);
        Marshal(node.GetAttributeValue(nameof(Ep1InvisibilityTemplate.HintDetectableStates),"IS_ATTACKING"),&target->HintDetectableStates,state);
        Marshal(node.GetAttributeValue(nameof(Ep1InvisibilityTemplate.NoStealthForAttackWindow),"0s"),&target->NoStealthForAttackWindow,state);
        Marshal(node.GetChildNodes(nameof(Ep1InvisibilityTemplate.IgnoreTreeCheckUpgrade)),&target->IgnoreTreeCheckUpgrade,state);
        Marshal(node.GetAttributeValue(nameof(Ep1InvisibilityTemplate.RequiredStatus),null),&target->RequiredStatus,state);
        Marshal(node,(BaseAssetType*)target,state);
    }
}
