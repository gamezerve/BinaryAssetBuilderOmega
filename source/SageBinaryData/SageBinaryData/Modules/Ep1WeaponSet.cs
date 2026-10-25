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

// Reborn: 44-byte EP1 root includes disabled flags and four list headers; typed turret arrays retain their own EP1 stride; unimplemented hierarchical/contained slots remain explicitly refused.
[StructLayout(LayoutKind.Sequential)]
public struct Ep1WeaponSetUpdateModuleData
{
    public UpdateModuleData Base;
    public DisabledBitFlags ActiveWhenDisabledBy;
    public List<Ep1WeaponSlotHardpoint> WeaponSlotHardpoint;
    public List<Ep1WeaponSlotTurret> WeaponSlotTurret;
    public List<Ep1WeaponSlotTurret> WeaponSlotHierarchicalTurret;
    public List<Ep1WeaponSlotHardpoint> WeaponSlotContained;
}

// Reborn: EP1 turret AI is 100 bytes, including status flags, idle-rate control, jiggle and its optional 44-byte chooser.
[StructLayout(LayoutKind.Sequential)]
public struct Ep1TurretAIData
{
    public float TurretTurnRate,TurretPitchRate;
    public Angle MinimumPitch;
    public Percentage PitchHeight;
    public WeaponSlotBitFlags ControlledWeaponSlots;
    public ObjectStatusBitFlags InvalidIdleScanObjectStatus;
    public Percentage IdleTurnRateModifier;
    public Time MinIdleScanTime,MaxIdleScanTime;
    public float MinIdleScanAngle,MaxIdleScanAngle;
    public Angle MaxDeflectionClockwise,MaxDeflectionAntiClockwise,NaturalTurretAngle;
    public float BlindFireDistance;
    public Angle ComeToHaltJiggle;
    public unsafe Ep1TurretAITargetChooserData* TurretAITargetChooserData;
    public SageBool AllowsPitch,RecenterWhenOutOfTurnRange,SetAngleModelStates;
}
// Reborn: turret slots inherit the 28-byte hardpoint and append inline AI plus a firing-arc flag, totaling 132 bytes.
[StructLayout(LayoutKind.Sequential)]
public struct Ep1WeaponSlotTurret
{
    public Ep1WeaponSlotHardpoint Base;
    public Ep1TurretAIData TurretSettings;
    public SageBool ForceRotateObjectToMaintainFiringArc;
}
// Reborn: schema dynamic-target flags use one native mask word.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1TargetChooserDynamicTargetFlags { public fixed uint Value[1]; }
// Reborn: retain the source-visible vision list header; nonempty vision records still need their processor.
[StructLayout(LayoutKind.Sequential)]
public struct Ep1TurretVisionRangeItem { public BaseAssetType Base; public Percentage Range; public unsafe ObjectFilter* Filter; }
// Reborn: the base chooser has 36 bytes before the turret-specific comparison reference and flags.
[StructLayout(LayoutKind.Sequential)]
public struct Ep1BaseTurretTargetChooserData
{
    public Time IdleScanDelay,ReacquireDelay,StartupDelay;
    public float SympathyRange;
    public Ep1TargetChooserDynamicTargetFlags CanPickDynamicTargetType;
    public List<Ep1TurretVisionRangeItem> ObjectVisionRange;
    public SageBool CheckVisionRange,CanPickDynamicTargets,RotateToTargetWhenAiming,CanAutoAcquireNonAutoAcquirable,CanChaseAttack,CanOnlyAttackTargetsInFront;
}
// Reborn: turret comparison defaults generate a real stock/mod strong reference at offset 36.
[StructLayout(LayoutKind.Sequential)]
public struct Ep1TurretAITargetChooserData
{
    public Ep1BaseTurretTargetChooserData Base;
    public AssetReference<TargetingCompareList> TargetingCompareList;
    public SageBool ActiveWhenPerformingSpecialAbility,CanAcquireDynamicIfAssignedOutOfRange,CanPickTargetsOutOfTurretAngle;
}
