using System.Runtime.InteropServices;
using Relo;
using AnsiString=Relo.String<sbyte>;
namespace SageBinaryData;

// Reborn: Tactics uses every original EP1 value and explicit forceValue alias.
public enum Ep1Tactics { SimpleAttack=0,FlankAttack=1,DefenseAvoidanceAttack=2,SimpleSiege=3,Engineer=4,SimpleDefense=5,StaticDefense=6,ReactiveDefense=7,SimpleExpansion=8,ExactExpansion=9,Hunt=10,RoamingDefense=11,GarrisonBuilding=12,Scripted=13,NullTactic=14 }

// Reborn: TacticEndBehavior uses every original EP1 value and explicit forceValue alias.
public enum Ep1TacticEndBehavior { RAMPAGE=0,DISBAND=1,RETURN_HOME_AND_DISBAND=2,PICK_NEW_TARGET=3 }

// Reborn: TeamPriority uses every original EP1 value and explicit forceValue alias.
public enum Ep1TeamPriority { DEFAULT=0,DEFENSE=1,LOW=2,STANDARD=3,DEFENSE_HIGH=4,HIGH=5,COOP=6,SCRIPTED=7,HOTSPOT=8 }

// Reborn: TeamReleaseType uses every original EP1 value and explicit forceValue alias.
public enum Ep1TeamReleaseType { ALWAYS_RELEASE=0,RELEASE_TO_PRIORITY=1,NEVER_RELEASE=2 }

// Reborn: DistanceSortPoints uses every original EP1 value and explicit forceValue alias.
public enum Ep1DistanceSortPoints { TARGET_POINT=0,HOME_BASE=1 }

// Reborn: AITargetDisposition uses every original EP1 value and explicit forceValue alias.
public enum Ep1AITargetDisposition { Invalid=-1,Offensive=0,Defensive=1,Neutral=2 }

// Reborn: PathfindingCheckType uses every original EP1 value and explicit forceValue alias.
public enum Ep1PathfindingCheckType { QuickDoesPathExist=0,QuickDoesAttackPathExist=1,None=2 }

// Reborn: AICannotAttackTargetReaction uses every original EP1 value and explicit forceValue alias.
public enum Ep1AICannotAttackTargetReaction { TreatLikeSuccess=0,Disband=1,ReturnHomeAndDisband=2,Ignore=3 }

// Reborn: UnitLocomotorType uses every original EP1 value and explicit forceValue alias.
public enum Ep1UnitLocomotorType { LAND=0,WATER=1,AMPHIBIOUS=2,AIR=3 }

// Reborn: team recruitment uses its own four locomotor bits, separate from placement and target-location masks.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1UnitLocomotorTypeBitFlags { public const int Count=4,BitsInSpan=32,NumSpans=1;public fixed uint Value[NumSpans]; }

// Reborn: original team headers include optional numeric bounds, both kind masks, micro-manager import, complete recruitment flags and create/filter graphs.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1AITeamTemplate
{
    public int MinUnits;
    public int* MaxUnits;
    public float* MinPowerAdvantage;
    public int* MaxTypesOfUnits;
    public KindOfBitFlags IncludeKindOf,ExcludeKindOf;
    public Ep1TeamPriority Priority;
    public Ep1TeamReleaseType ReleaseType;
    public AssetReference<BaseAssetType> MicroManager;
    public Ep1UnitLocomotorTypeBitFlags AllowedLocomotorTypes;
    public Ep1DistanceSortPoints DistanceSortPoint;
    public Ep1RegisteredObjectSetId RegisteredObjectSet;
    public float* BaseDefenseMaxDistance;
    public List<CreateUnitInfo> CreateUnits;
    public ObjectFilter* ObjectFilter;
    public SageBool AlwaysRecruit,AllowRecruitUnitsInCombat,RequireCanAttackTarget,SortByDistance,IsBaseDefenseTeam;
}

// Reborn: each tactic retains its original POID, complete timing/disposition fields, optional waypoint/base criteria and ordered teams.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1AIStateTactic
{
    public TypedAssetId<BaseAssetType> id;
    public Ep1Tactics Tactic;
    public Ep1TacticEndBehavior EndBehavior;
    public int DisbandAfterRetreatTeamSize;
    public Time AestheticsManagerPatrolIdleTime;
    public float AestheticsManagerMinDistanceFromHarvester;
    public AIDifficultyBitFlags Difficulty;
    public float Distance,CombatLeashSlack;
    public Ep1AITargetDisposition TargetDispositionToDisplay;
    public AnsiString* GatherWaypoint;
    public Time UpdateTime;
    public AIBaseSelectionCriteria* TargetBaseCriteria;
    public AIBaseSelectionCriteria* HomeBaseCriteria;
    public List<Ep1AITeamTemplate> TeamTemplate;
    public SageBool Targetless,EndIfStateTurnsOff,UseAestheticsManager,UseTransports;
}

// Reborn: state target choices retain the original strong targeting-heuristic import and integer priority.
[StructLayout(LayoutKind.Sequential)]
public struct Ep1StateTargetHeuristic
{
    public AssetReference<Ep1AITargetingHeuristic> TargetHeuristic;
    public int Priority;
}

// Reborn: the complete 64-byte EP1 strategic state keeps all optional count/path/reaction/feedback values before its two ordered arrays.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1AIStrategicStateDefinition
{
    public Ep1AIStateDefinition Base;
    public uint MaxTargets;
    public uint* MaxRunningCopies;
    public uint* MaxInactiveCopies;
    public Ep1PathfindingCheckType* PathfindingCheckType;
    public Ep1AICannotAttackTargetReaction* CannotAttackTargetReaction;
    public StringHash* TeamGatherBeginFeedbackKey;
    public StringHash* ExecutionBeginFeedbackKey;
    public StringHash* ExecutionCanceledFeedbackKey;
    public StringHash* ExecutionCompleteFeedbackKey;
    public List<Ep1StateTargetHeuristic> TargetHeuristic;
    public List<Ep1AIStateTactic> Tactic;
}
