using Relo;
using System.Runtime.InteropServices;
using AnsiString = Relo.String<sbyte>;

namespace SageBinaryData;

[StructLayout(LayoutKind.Sequential)]
public struct GiveOrRestoreUpgradeSpecialPowerModuleData
{
    public SpecialAbilityUpdateModuleData Base;
    public AnsiString CommandButton;
    public AssetReference<UpgradeTemplate> UpgradeToGive;
    public WeaponSetBitFlags WeaponFlags;
}

// Reborn: EP1 ends immediately after its command string and upgrade reference.
[StructLayout(LayoutKind.Sequential)]
public struct Ep1GiveOrRestoreUpgradeSpecialPowerModuleData
{
    public const uint Wire = 0x0453E648u;
    public SpecialAbilityUpdateModuleData Base;
    public AnsiString CommandButton;
    public AssetReference<UpgradeTemplate> UpgradeToGive;
}
