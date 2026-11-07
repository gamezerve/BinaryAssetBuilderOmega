using System.Runtime.InteropServices;
using Relo;
using AnsiString=Relo.String<sbyte>;
namespace SageBinaryData;

// Reborn: original AI timer origins retain MAP_START=0 and AI_START=1.
public enum Ep1AITimerType { MAP_START,AI_START }
// Reborn: original placement surface bits are LAND, WATER and DEEP_WATER, independent of water/land targeting flags.
public enum Ep1BuildingPlacementSurface { LAND,WATER,DEEP_WATER }
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1BuildingPlacementSurfaceBitFlags { public const int Count=3,BitsInSpan=32,NumSpans=1;public fixed uint Value[NumSpans]; }

// Reborn: AIStateTimerHeuristic retains the complete field order observed in its original EP1 native witness.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1AIStateTimerHeuristic
{
    public Ep1AIStateHeuristic Base;
    public Time StartTime;
    public Time EndTime;
    public Ep1AITimerType CountFrom;
}

// Reborn: AIStateIntervalHeuristic retains the complete field order observed in its original EP1 native witness.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1AIStateIntervalHeuristic
{
    public Ep1AIStateHeuristic Base;
    public Time IntervalTime;
    public Time ActiveTime;
    public Ep1AITimerType CountFrom;
}

// Reborn: AIStateMapNameHeuristic retains the complete field order observed in its original EP1 native witness.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1AIStateMapNameHeuristic
{
    public Ep1AIStateHeuristic Base;
    public AnsiString Name;
    public SageBool PassIfTrue;
}

// Reborn: AIStatePathToTargetHeuristic retains the complete field order observed in its original EP1 native witness.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1AIStatePathToTargetHeuristic
{
    public Ep1AIStateHeuristic Base;
    public Ep1AITargetLocationType Surface;
    public TypedAssetId<GameObject> ObjectTypeToUseForTest;
    public LocomotorSetType LocomotorConditionToUseForTest;
    public SageBool PathExists;
}

// Reborn: AIStateScriptedFlagHeuristic retains the complete field order observed in its original EP1 native witness.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1AIStateScriptedFlagHeuristic
{
    public Ep1AIStateHeuristic Base;
    public AnsiString FlagName;
}

// Reborn: AIStateIncomeAdvantageHeuristic retains the complete field order observed in its original EP1 native witness.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1AIStateIncomeAdvantageHeuristic
{
    public Ep1AIStateHeuristic Base;
    public float Value;
    public SageBool Above;
}

// Reborn: AIStateIsBuildingHeuristic retains the complete field order observed in its original EP1 native witness.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1AIStateIsBuildingHeuristic
{
    public Ep1AIStateHeuristic Base;
    public KindOfBitFlags KindOf;
    public SageBool PassIfBuilding;
}

// Reborn: AIStateTechBuildingShareHeuristic retains the complete field order observed in its original EP1 native witness.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1AIStateTechBuildingShareHeuristic
{
    public Ep1AIStateHeuristic Base;
    public Percentage PercentOfFairShare;
    public SageBool Above;
}

// Reborn: AIStateAllNearbyNodesFullHeuristic retains the complete field order observed in its original EP1 native witness.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1AIStateAllNearbyNodesFullHeuristic
{
    public Ep1AIStateHeuristic Base;
}

// Reborn: AIStateTimeSinceLastExpansionHeuristic retains the complete field order observed in its original EP1 native witness.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1AIStateTimeSinceLastExpansionHeuristic
{
    public Ep1AIStateHeuristic Base;
    public Time Time;
    public SageBool Past;
}

// Reborn: AIStateResourceNodeShareHeuristic retains the complete field order observed in its original EP1 native witness.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1AIStateResourceNodeShareHeuristic
{
    public Ep1AIStateHeuristic Base;
    public Percentage PercentOfFairShare;
    public SageBool Above;
}

// Reborn: AIStateCanBuildBuildingsOnSurfaceHeuristic retains the complete field order observed in its original EP1 native witness.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1AIStateCanBuildBuildingsOnSurfaceHeuristic
{
    public Ep1AIStateHeuristic Base;
    public int MinPlacementGridCells;
    public int* MaxPlacementGridCells;
    public Ep1BuildingPlacementSurfaceBitFlags Surfaces;
    public TypedAssetId<GameObject> ObjectTypeToUseForTest;
}

// Reborn: AIAlliedBeaconExistsHeuristic retains the complete field order observed in its original EP1 native witness.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1AIAlliedBeaconExistsHeuristic
{
    public Ep1AIStateHeuristic Base;
    public Ep1AIBeaconType Type;
    public SageBool Exists;
}

// Reborn: AIStateOpenNodeHeuristic retains the complete field order observed in its original EP1 native witness.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1AIStateOpenNodeHeuristic
{
    public Ep1AIStateHeuristic Base;
    public float MinDistanceFromEnemy;
    public SageBool Exists;
}
