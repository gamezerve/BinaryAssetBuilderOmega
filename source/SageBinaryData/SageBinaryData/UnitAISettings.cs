using System.Runtime.InteropServices;
using Relo;

namespace SageBinaryData;

// Reborn: the original EP1 singleton stores both ObjectFilters inline and its two bools after them.
[StructLayout(LayoutKind.Sequential)]
public struct UnitAISettings
{
    public BaseAssetType Base;
    public int SupplyBoxesPerTree;
    public int ValuePerSupplyBox;
    public float MinDistFromEdgeOfMapForBuild;
    public float SupplyBuildBorder;
    public int MaxLineBuildObjects;
    public int MaxTunnelCapacity;
    public float MinLowEnergyProductionSpeed;
    public float MaxLowEnergyProductionSpeed;
    public float LowEnergyPenaltyModifier;
    public float DamageRadiusMinimumForSplash;
    public Percentage SellPercentage;
    public Time UnlookPersistDuration;
    public float DefaultStructureRubbleHeight;
    public int TimeAfterDamageUntilRepairAllowed;
    public float SelectionFlashSaturationFactor;
    public float GarrisonedRangeMultiplier;
    public float GroupMoveClickToGatherAreaFactor;
    public float FormationMoveClusterRange;
    public int InvisibilityOpacityCycleFrames;
    public Time BuilderFadeOutTime;
    public Time BuilderFadeInTime;
    public float BuilderMoveFromNewStructureDistance;
    public float PartitionCellSize;
    public float TerrainResourceCellSize;
    public int DefaultStartingCash;
    public BodyDamageType MovementPenaltyDamageState;
    public float ParachuteDriftSpeed;
    public Time ParachuteLandTime;
    public float LinearDragSubmerged;
    public Angle AutomaticReverseAngle;
    public float AutomaticReverseDistance;
    public float SkipMoveMinDistance;
    public AssetReference<TargetingCompareList> CompleteTargetingCompareList;
    public float ScatterDistance;
    public int AttackMovementPersistFrames;
    public float AIIgnoreDuplicateMoveCommandDistance;
    public float HumanIgnoreDuplicateMoveCommandDistance;
    public float ConsiderHumanDuplicateMoveCommandDistance;
    public float CaptureWaitDistance;
    public Time CaptureWaitTime;
    public ModelConditionBitFlags CaptureWaitModelConditions;
    public ObjectFilter VictoryConditionUnitObjectFilter;
    public ObjectFilter VictoryConditionStructureObjectFilter;
    public SageBool SelectionFlashHouseColor;
    public SageBool EnableHouseColor;
}
