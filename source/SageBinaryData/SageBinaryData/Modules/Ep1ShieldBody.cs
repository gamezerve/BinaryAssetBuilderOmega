using System.Runtime.InteropServices;
using Relo;

namespace SageBinaryData;

// Reborn: retail ShieldBody appends its four FX imports and shield fields to the 76-byte EP1 ActiveBody.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1ShieldBodyModuleData
{
    public const uint Wire = 0xD3565857u;
    public Ep1ActiveBodyModuleData Base;
    public AssetReference<FXList> ShieldEnabledFX, ShieldDepleteFX, ShieldRechargeEndFX, ShieldTakeDamageFX;
    public float ShieldAmount;
    public AssetReference<ArmorTemplate>* ShieldArmor;
    public Time ShieldRechargeIdleTime;
    public ModelConditionFlagType ShieldActiveModelCondition;
}
