using System.Runtime.InteropServices;
using Relo;

namespace SageBinaryData;

// Reborn: the EP1 activation class owns the existing 12-byte local-module rows over the SpecialAbility parent.
[StructLayout(LayoutKind.Sequential)]
public struct Ep1ActivateModuleSpecialAbilityUpdateModuleData
{
    public const uint Wire = 0xCD33BCEEu;
    public SpecialAbilityUpdateModuleData Base;
    public List<SpecialPowerModuleInfo> SpecialPowerModules;
}

// Reborn: the naval create class adds one strong modifier reference.
[StructLayout(LayoutKind.Sequential)]
public struct Ep1AttributeModifierNavalModuleData
{
    public const uint Wire = 0xA34CED5Cu;
    public CreateModuleData Base;
    public AssetReference<AttributeModifier> AttributeModifier;
}
