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

    //-------------------------------------------------------------------------------------------------
    /** Reborn: emit the delayed weapon class with the retail WeaponFire parent and seconds at +0x11c. */
    //-------------------------------------------------------------------------------------------------
    private static unsafe void MarshalEp1DelayedWeaponFire(Node node, BehaviorModuleData** destination, Tracker state)
    {
        using var context = state.Push((void**)destination, (uint)sizeof(Ep1DelayedWeaponFireSpecialAbilityUpdateModuleData), 1u);
        var result = (Ep1DelayedWeaponFireSpecialAbilityUpdateModuleData*)*destination;
        Marshal(node, &result->Base, state);
        Marshal(node.GetAttributeValue("FireDelay", "0s"), &result->FireDelay, state);
        uint wire = Ep1DelayedWeaponFireSpecialAbilityUpdateModuleData.WireTypeId;
        state.InplaceEndianToPlatform(&wire);
        *(uint*)result = wire;
    }
}
