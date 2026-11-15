using Relo;
using System.Runtime.InteropServices;
using AnsiString = Relo.String<sbyte>;

namespace SageBinaryData;

// Reborn: retain this pre-EP1 layout under an explicit legacy name, outside wire-name selection.
[StructLayout(LayoutKind.Sequential)]
public struct LegacyBeamSpecialAbilityUpdateModuleData
{
    public SpecialAbilityUpdateModuleData Base;
    public float DamagePerSecond;
    public AssetReference<AttributeModifier> TargetAttributeModifierAdd;
    public AssetReference<FXList> SweepFX;
    public float ReflectorExtendDistance;
    public AnsiString PreferredTargetBone;
    public SageBool JoinWithOtherBeams;
    public SageBool DissapateWhenTargetDead;
    public SageBool LineType;
    public SageBool TargetSamePlayerOnly;
}
