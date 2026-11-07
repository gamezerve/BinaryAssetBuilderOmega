using Relo;
using SageBinaryData;
public static partial class Marshaler
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: serialize the complete EP1 crate pickup fields and original defaults without the removed KW science slot. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1CrateCollideModuleData* target,Tracker state)
    {
        if(node is null)return;
        Marshal(node.GetAttributeValue(nameof(Ep1CrateCollideModuleData.KindOf),null),&target->KindOf,state);
        Marshal(node.GetAttributeValue(nameof(Ep1CrateCollideModuleData.KindOfNot),null),&target->KindOfNot,state);
        Marshal(node.GetAttributeValue(nameof(Ep1CrateCollideModuleData.IsForbidOwnerPlayer),"false"),&target->IsForbidOwnerPlayer,state);
        Marshal(node.GetAttributeValue(nameof(Ep1CrateCollideModuleData.IsBuildingPickup),"false"),&target->IsBuildingPickup,state);
        Marshal(node.GetAttributeValue(nameof(Ep1CrateCollideModuleData.IsHumanOnlyPickup),"false"),&target->IsHumanOnlyPickup,state);
        Marshal(node.GetAttributeValue(nameof(Ep1CrateCollideModuleData.IsHumanOnlyPickupInSinglePlayerCampaign),"true"),&target->IsHumanOnlyPickupInSinglePlayerCampaign,state);
        Marshal(node.GetAttributeValue(nameof(Ep1CrateCollideModuleData.ExecuteFX),null),&target->ExecuteFX,state);
        Marshal(node.GetAttributeValue(nameof(Ep1CrateCollideModuleData.ExecutionAnimationTemplate),null),&target->ExecutionAnimationTemplate,state);
        Marshal(node.GetAttributeValue(nameof(Ep1CrateCollideModuleData.ExecuteAnimationDisplayTimeInSeconds),"0"),&target->ExecuteAnimationDisplayTimeInSeconds,state);
        Marshal(node.GetAttributeValue(nameof(Ep1CrateCollideModuleData.ExecuteAnimationZRisePerSecond),"0"),&target->ExecuteAnimationZRisePerSecond,state);
        Marshal(node.GetAttributeValue(nameof(Ep1CrateCollideModuleData.ExecuteAnimationFades),"false"),&target->ExecuteAnimationFades,state);
        Marshal(node.GetAttributeValue(nameof(Ep1CrateCollideModuleData.ExecuteAudioEvent),"CratePickup"),&target->ExecuteAudioEvent,state);
        Marshal(node.GetAttributeValue(nameof(Ep1CrateCollideModuleData.KillOnPickup),"true"),&target->KillOnPickup,state);
        Marshal(node,(CollideModuleData*)target,state);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: preserve the source-authored modifier and self-application flag under the witnessed EP1 crate inheritance. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,AttributeCrateCollideModuleData* target,Tracker state)
    {
        if(node is null)return;
        Marshal(node.GetAttributeValue(nameof(AttributeCrateCollideModuleData.AttributeModifier),"AttributeModifier_InvulnerabilityCrate"),&target->AttributeModifier,state);
        Marshal(node.GetAttributeValue(nameof(AttributeCrateCollideModuleData.ApplyToSelf),"false"),&target->ApplyToSelf,state);
        Marshal(node,(Ep1CrateCollideModuleData*)target,state);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: emit the healing radius and optional filter after the original EP1 pickup base. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1HealCrateCollideModuleData* target,Tracker state)
    {
        if(node is null)return;
        Marshal(node.GetAttributeValue(nameof(Ep1HealCrateCollideModuleData.Range),"75"),&target->Range,state);
        Marshal(node.GetChildNode(nameof(Ep1HealCrateCollideModuleData.ObjectFilter),null),&target->ObjectFilter,state);
        Marshal(node,(Ep1CrateCollideModuleData*)target,state);
    }
}
