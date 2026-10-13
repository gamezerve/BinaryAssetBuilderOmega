using Relo;
using System.Runtime.InteropServices;

namespace SageBinaryData;

// Reborn: options are enum-indexed bits in the stock four-byte effect mask.
public enum EffectInfoFlagType { DIE_ON_ENTER, CRASH_IF_AIRBORNE }

// Reborn: stock effect records place options before the three packed bools.
[StructLayout(LayoutKind.Sequential)]
public struct EffectInfoType
{
    public float Fraction;
    public AssetReference<AttributeModifier> AttributeModifier;
    public uint Options;
    public SageBool OnEnterDisable;
    public SageBool OnEnterCancelSpecialAbilities;
    public SageBool SetUpgradedArmor;
}

// Reborn: root scalar/list/pointer layout independently anchored by two stock tracker variants.
[StructLayout(LayoutKind.Sequential)]
public struct SecondaryDamageTrackerModuleData
{
    public UpdateModuleData Base;
    public Time UpdateDuration;
    public uint DissipateAmount;
    public Percentage MaxDamageClampScaler;
    public unsafe RGBColor* Color;
    public List<EffectInfoType> EffectInfo;
    public unsafe ObjectStatusValidationDataType* ObjectStatusValidation;
}

// Reborn: EP1 freefall embeds the complete sixty-byte model-condition mask before its three real-valued motion parameters.
[StructLayout(LayoutKind.Sequential)]
public struct FreefallUpdateModuleData
{
    public UpdateModuleData Base;
    public ModelConditionBitFlags ModelConditionsToSet;
    public float InitialVelocity;
    public float Acceleration;
    public float TerminalVelocity;
}
