using Relo;
using System.Runtime.InteropServices;

namespace SageBinaryData;

// Reborn: EP1 weapon rows add a separate forbidden-status mask and metrics flag; the legacy KW row is unchanged.
[StructLayout(LayoutKind.Sequential)]
public struct Ep1WeaponSlotWeaponData
{
    public WeaponSlotType Ordering;
    public AssetReference<WeaponTemplate> Template;
    public AssetReference<UpgradeTemplate> Upgrade;
    public ObjectStatusBitFlags ObjectStatus;
    public ObjectStatusBitFlags ForbiddenObjectStatus;
    public SageBool IsPlayerUpgradePermanent;
    public SageBool ConquerMetricsAlwaysCount;
}

// Reborn: 28-byte EP1 hardpoint includes the stock AutoChooseWeapon byte at +25.
[StructLayout(LayoutKind.Sequential)]
public struct Ep1WeaponSlotHardpoint
{
    public uint TypeId;
    public uint ID;
    public WeaponSlotInterleavedStyleType InterleavedStyle;
    public WeaponChoiceCriteria WeaponChoiceCriteria;
    public List<Ep1WeaponSlotWeaponData> Weapon;
    public SageBool AllowInterleavedFiring;
    public SageBool AutoChooseWeapon;
}

// Reborn: 44-byte EP1 root includes disabled flags and four list headers; unsupported slot families must remain empty, not be approximated as hardpoints.
[StructLayout(LayoutKind.Sequential)]
public struct Ep1WeaponSetUpdateModuleData
{
    public UpdateModuleData Base;
    public DisabledBitFlags ActiveWhenDisabledBy;
    public List<Ep1WeaponSlotHardpoint> WeaponSlotHardpoint;
    public uint TurretCount,TurretItems;
    public uint HierarchicalTurretCount,HierarchicalTurretItems;
    public uint ContainedCount,ContainedItems;
}
