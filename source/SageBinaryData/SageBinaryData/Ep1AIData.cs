using System.Runtime.InteropServices;
using Relo;
using AnsiString=Relo.String<sbyte>;

namespace SageBinaryData;

// Reborn: this explicit 220-byte EP1 AIData model is source-complete and independently qualified against the original global native record.
[StructLayout(LayoutKind.Sequential)]
public struct Ep1AIData
{
    public BaseInheritableAsset Base;
    public float StructureSeconds;
    public float TeamSeconds;
    public uint ResourcesWealthy;
    public uint ResourcesPoor;
    public float StructuresWealthyMod;
    public float TeamWealthyMod;
    public float StructuresPoorMod;
    public float TeamPoorMod;
    public float TeamResourcesToBuild;
    public uint GuardChaseUnitFrames;
    public uint GuardEnemyScanRate;
    public uint GuardEnemyReturnScanRate;
    public float WallHeight;
    public float SkirmishGroupFudgeValue;
    public float MaxRecruitDistance;
    public float RepulsedDistance;
    public float MinDistanceForGroup;
    public float FormationEnemyDistance;
    public int RebuildDelaySeconds;
    public float SupplyCenterSafeRadius;
    public float AIDozerBoredRadiusModifier;
    public float MeleeApproachTolerance;
    public float MeleeApproachDist;
    public float WadeWaterDepth;
    public float DeepWaterDepth;
    public float FormationColumnWidth;
    public float FormationRowDepth;
    public float FormationSquadSpacing;
    public float NarrowPassageScale;
    public int MaxSquadWidth;
    public float AltCameraZoomOverride;
    public float AltCameraPitchOverride;
    public float MaxRetaliateDistance;
    public float RetaliateFriendsRadius;
    public float ChaseFromBehindLimit;
    public float CastleSiegeStandBackDistance;
    public AnsiString LowLodTree;
    public AnsiString LowLodTreeNoGrab;
    public AnsiString LowLodTreeNoHarvest;
    public float LowLodTreeScale;
    public int Optimization_NumTeamsToRecruit;
    public int Optimization_NumEfficiencyUpdates;
    public Time Optimization_TimeToCacheCanEnterTransport;
    public Time Optimization_TimeToCacheCanReachMobileTarget;
    public Time Optimization_TimeToCacheCanReachImmobileTarget;
    public Time Optimization_ConquerMetricsCacheTimeOut;
    public Time Optimization_TacticsEmergencyKickPeriod;
    public SageBool EnableRepulsors;
    public SageBool ForceSkirmishAI;
    public SageBool RotateSkirmishBases;
    public SageBool AttackUsesLineOfSight;
    public SageBool AICrushesInfantry;
    public SageBool WaitForOthers;
    public SageBool HordesWaitForHordes;
    public SageBool AttackMoveUsesFormations;
    public SageBool AllowForestFires;
    public SageBool UseFormations;
    public SageBool UseLowLODTrees;
    public SageBool DisableTrees;
    public byte MaximumCraterDepth;
}
