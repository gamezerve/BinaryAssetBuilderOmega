using System.Runtime.InteropServices;
using Relo;

namespace SageBinaryData;

// Reborn: EP1's sixteen-byte squish record replaces KW's extra kill-delay/direction fields with two distinct authored crush booleans; the legacy record is untouched.
[StructLayout(LayoutKind.Sequential)]
public struct Ep1SquishCollideModuleData
{
    public CollideModuleData Base;
    public Time CrushAnimTime;
    public SageBool UseVehicleStyleCrush;
    public SageBool AllowAllyCrush;
}

// Reborn: EP1 physics keeps the original 104-byte field order but stores the three shock timers as Time floats rather than KW unsigned integers.
[StructLayout(LayoutKind.Sequential)]
public struct Ep1PhysicsBehaviorModuleData
{
    public UpdateModuleData Base;
    public float MaxXRotationVelocity, MaxYRotationVelocity, MaxZRotationVelocity, GravityMultiplier;
    public Time ShockStunnedTimeLow, ShockStunnedTimeHigh, ShockStandingTime;
    public float FirstHeight, SecondHeight;
    public Percentage FirstPercentIndent, SecondPercentIndent;
    public int BounceCount;
    public float BounceDistance, BounceFirstHeight, BounceSecondHeight;
    public Percentage BounceFirstPercentIndent, BounceSecondPercentIndent;
    public AssetReference<FXList> GroundHitFX, GroundBounceFX;
    public Percentage FirstPercentHeight, SecondPercentHeight;
    public float CurveFlattenMinDist;
    public SageBool TumbleRandomly, AllowBouncing, KillWhenRestingOnGround, OrientToFlightPath, IgnoreTerrainHeight;
}

// Reborn: stock HitReaction own-field tokens place the two-word death mask at 32, optional status validation at 40 and three booleans at 44..46, not KW's two flags.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1HitReactionBehaviorModuleData
{
    public UpdateModuleData Base;
    public Time LightDamageHitReactionLifeTimer, MediumDamageHitReactionLifeTimer, HeavyDamageHitReactionLifeTimer;
    public float LightDamageHitReactionThreshold, MediumDamageHitReactionThreshold, HeavyDamageHitReactionThreshold;
    public DeathBitFlags RequiredDeathTypes;
    public ObjectStatusValidationDataType* ObjectStatusValidation;
    public SageBool RotateToSourceWhenDamaged, RotateToSourceIgnoreIdleCheck, GoIdleWhenDamaged;
}

// Reborn: EP1 has no KW ApplyToContained byte; its 100-byte StatusBitsUpgrade ends immediately before separately allocated upgrade hash lists.
[StructLayout(LayoutKind.Sequential)]
public struct Ep1StatusBitsUpgradeModuleData
{
    public UpgradeModuleData Base;
    public ObjectStatusBitFlags StatusToSet, StatusToClear;
}

// Reborn: EP1 CreateObjectDie has no KW DebrisPortionOfSelf header; the two list headers start at 56/64 in its 72-byte record.
[StructLayout(LayoutKind.Sequential)]
public struct Ep1CreateObjectDieModuleData
{
    public DieModuleData Base;
    public AssetReference<ObjectCreationList> CreationList;
    public List<AssetReference<UpgradeTemplate>> UpgradeRequired, UpgradeForbidden;
}

// Reborn: EP1 death-weapon metadata exposes two additional booleans at 143/144, extending the original record to 148 bytes.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1FireWeaponWhenDeadBehaviorModuleData
{
    public UpgradeModuleData Base;
    public Time DelayTime;
    public AssetReference<WeaponTemplate> DeathWeapon;
    public KindOfBitFlags FocusFireKindOfFlag;
    public Coord3D* WeaponOffset;
    public DieMuxDataType DieMuxData;
    public List<WeaponFireProbabilityType> WeaponFireProbability;
    public SageBool InitiallyActive, ActiveDuringConstruction, ContinueToBezierDestination, FireAtGround, UseMyKillerAsWeaponSource;
}
