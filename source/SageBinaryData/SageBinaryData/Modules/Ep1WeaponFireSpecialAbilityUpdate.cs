using Relo;
using System.Runtime.InteropServices;

namespace SageBinaryData;

// Reborn: EP1 weapon-fire abilities include ModelConditionsOnUnpack and CanTriggerWhileDead; retail metadata anchors the 284-byte header.
[StructLayout(LayoutKind.Sequential)]
public struct Ep1WeaponFireSpecialAbilityUpdateModuleData
{
    public SpecialAbilityUpdateModuleData Base;
    public AssetReference<WeaponTemplate> SpecialWeapon;
    public int WhichSpecialWeapon;
    public Time BusyForDuration;
    public int WeaponSlotID;
    public WeaponSlotType WeaponSlotType;
    public ModelConditionFlagType ModelConditionsOnUnpack;
    public SageBool SkipContinue;
    public SageBool CanTriggerWhileDead;
    public SageBool NeedLivingTargets;
    public SageBool PlayWeaponPreFireFX;
    public SageBool OffsetTargetLocationFromObject;
    public SageBool DestealthClosestObjectMatchingFilter;
}
