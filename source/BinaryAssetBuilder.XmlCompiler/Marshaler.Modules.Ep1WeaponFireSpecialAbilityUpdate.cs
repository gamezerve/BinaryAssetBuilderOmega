using Relo;
using SageBinaryData;

public static partial class Marshaler
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: compile every official EP1 weapon-fire attribute and the inherited ability graph with schema-defined defaults. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node, Ep1WeaponFireSpecialAbilityUpdateModuleData* objT, Tracker state)
    {
        if(node is null)return;
        Marshal(node.GetAttributeValue(nameof(Ep1WeaponFireSpecialAbilityUpdateModuleData.SpecialWeapon),null),&objT->SpecialWeapon,state);
        Marshal(node.GetAttributeValue(nameof(Ep1WeaponFireSpecialAbilityUpdateModuleData.WhichSpecialWeapon),"0"),&objT->WhichSpecialWeapon,state);
        Marshal(node.GetAttributeValue(nameof(Ep1WeaponFireSpecialAbilityUpdateModuleData.SkipContinue),"false"),&objT->SkipContinue,state);
        Marshal(node.GetAttributeValue(nameof(Ep1WeaponFireSpecialAbilityUpdateModuleData.CanTriggerWhileDead),"false"),&objT->CanTriggerWhileDead,state);
        Marshal(node.GetAttributeValue(nameof(Ep1WeaponFireSpecialAbilityUpdateModuleData.BusyForDuration),"0s"),&objT->BusyForDuration,state);
        Marshal(node.GetAttributeValue(nameof(Ep1WeaponFireSpecialAbilityUpdateModuleData.NeedLivingTargets),"false"),&objT->NeedLivingTargets,state);
        Marshal(node.GetAttributeValue(nameof(Ep1WeaponFireSpecialAbilityUpdateModuleData.PlayWeaponPreFireFX),"false"),&objT->PlayWeaponPreFireFX,state);
        Marshal(node.GetAttributeValue(nameof(Ep1WeaponFireSpecialAbilityUpdateModuleData.OffsetTargetLocationFromObject),"false"),&objT->OffsetTargetLocationFromObject,state);
        Marshal(node.GetAttributeValue(nameof(Ep1WeaponFireSpecialAbilityUpdateModuleData.WeaponSlotID),"1"),&objT->WeaponSlotID,state);
        Marshal(node.GetAttributeValue(nameof(Ep1WeaponFireSpecialAbilityUpdateModuleData.WeaponSlotType),"PRIMARY_WEAPON"),&objT->WeaponSlotType,state);
        Marshal(node.GetAttributeValue(nameof(Ep1WeaponFireSpecialAbilityUpdateModuleData.DestealthClosestObjectMatchingFilter),"false"),&objT->DestealthClosestObjectMatchingFilter,state);
        Marshal(node.GetAttributeValue(nameof(Ep1WeaponFireSpecialAbilityUpdateModuleData.ModelConditionsOnUnpack),"INVALID"),&objT->ModelConditionsOnUnpack,state);
        Marshal(node,(SpecialAbilityUpdateModuleData*)objT,state);
    }
}
