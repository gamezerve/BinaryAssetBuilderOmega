using System.Runtime.InteropServices;
using Relo;

namespace SageBinaryData;

// Reborn: preserve the four literal EP1 schema economy ordinals without reusing unrelated legacy economy enums.
public enum Ep1EconomyResourceType { TIBERIUM,ORE,SIGMA,ENERGY }

// Reborn: stock own-field metadata anchors this 92-byte EP1 record; opaque child pointers and the contextual-power header are admitted only null/empty, not guessed child ABIs.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1SkirmishAIInformation
{
    public SkirmishAIBaseLocation BaseBuildingLocation;
    public Angle AdditionalRotation;
    public int BaseBuildingMaxNudges;
    public float CrushSpeedBonus,OutOfRangePenalty;
    public float* ConquerMetricsOverrideDPS;
    public DamageType ConquerMetricsOverrideDamageType;
    public WeaponAntiBitFlags ConquerMetricsOverrideAntiMask,ConquerMetricsOverrideForbiddenAntiMask;
    public WeaponAntiBitFlags* ConquerMetricsOverrideDefendingAntiMask;
    public float ConquerMetricsDpsBonusAgainstDefenses;
    public Ep1EconomyResourceType NearResourceNodeType;
    public float DodgeSafetyDistance;
    public float* OverrideInnerDistanceToTarget;
    public float* OverrideOuterDistanceToTarget;
    public ObjectStatusBitFlags* DodgingRequiredObjectStatus;
    public List<uint> AIContextualPower;
    public void* ConstructionBaseSelectionCriteria;
    public void* ResourceNodeBaseClosenessTestSelectionCriteria;
    public void* DodgeWithUnits;
    public SageBool BlockIfUnpackTargetBuildingCannotBeRebuilt,UnitBuilderStandardCombatUnit,TargetDuringCrush,ContainedMembersProvideWeapon,CanFireUpCliffs,DoesSecondaryDamage;
}

// Reborn: EP1 moves DefaultCrushKillDelay to 20 before both list headers; KW's 52-byte size alone concealed the wrong internal offsets.
[StructLayout(LayoutKind.Sequential)]
public struct Ep1CrusherInfo
{
    public int CrushKnockback;
    public float CrushZFactor;
    public Percentage MinCrushVelocityPercent,CrushDecelerationPercent;
    public AssetReference<WeaponTemplate> CrushWeapon;
    public Time DefaultCrushKillDelay;
    public List<CrusherLevelModelConditionInfo> ExtraCrushLevels;
    public List<CrushKillDelayForObjectFilter> ExtraCrushKillDelays;
    public sbyte CrusherLevel,CrushableLevel;
    public SageBool CrushEqualLevelProps,UseCrushAttack,CrushOnlyWhileCharging,CrushAllies,CrushAircraftWhileStationary,UseDirectionCheck,ShouldSquishOnCollide,DecelerateForAllCollides,CannotCrushTarget;
}
