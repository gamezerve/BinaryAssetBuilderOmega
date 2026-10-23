using System.Runtime.InteropServices;
using Relo;

namespace SageBinaryData;

// Reborn: preserve the four literal EP1 schema economy ordinals without reusing unrelated legacy economy enums.
public enum Ep1EconomyResourceType { TIBERIUM,ORE,SIGMA,ENERGY }

// Reborn: AI base flag aliases follow the official forceValue rule; PRIMARY_TYPE_MAX shares CAPTURED rather than consuming a bit.
public enum AIBaseType { MAIN_BASE,EXPANSION,CAPTURED,PRIMARY_TYPE_MAX=CAPTURED,POINT_CENTERED,LAND_CENTERED,UNDER_ATTACK,ENEMIES_IN_BASE,OLDEST_OF_TYPE,MOST_RECENT_OF_TYPE,NO_BUILD_RADIUS }
public enum AIBaseSortingOrder { PREFER_OLDEST,PREFER_NEWEST,PREFER_SMALLEST,PREFER_LARGEST,PREFER_MOST_RECENTLY_ATTACKED,PREFER_LEAST_RECENTLY_ATTACKED,PREFER_CLOSEST }
// Reborn: ten AI base flags fit one native span, including the schema alias above.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct AIBaseTypeBitFlags { public fixed uint Value[1]; }
// Reborn: stock criteria metadata qualifies four flag spans followed by the sort ordinal.
[StructLayout(LayoutKind.Sequential)]
public struct AIBaseSelectionCriteria
{
    public AIBaseTypeBitFlags RequiredBaseTypes,ExcludedBaseTypes,PreferredBaseTypes,PreferredNotBaseTypes;
    public AIBaseSortingOrder SortOrder;
}
// Reborn: stock own-field metadata anchors this 92-byte EP1 record; contextual-power entries remain a separate unsupported family.
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
    public AIBaseSelectionCriteria* ConstructionBaseSelectionCriteria;
    public AIBaseSelectionCriteria* ResourceNodeBaseClosenessTestSelectionCriteria;
    public ObjectFilter* DodgeWithUnits;
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
