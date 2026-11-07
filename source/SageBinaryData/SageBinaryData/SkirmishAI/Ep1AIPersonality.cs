using System.Runtime.InteropServices;
using Relo;
using AnsiString=Relo.String<sbyte>;
namespace SageBinaryData;

// Reborn: original AIEnemySortingType values retain all explicit EP1 forceValue aliases.
public enum Ep1AIEnemySortingType { NearestFirst_AllowTies=0,NearestFirst_BreakTies=1,Flat=2,StrongestFirst_AllowTies=3,StrongestFirst_BreakTies=4 }

// Reborn: original AIFeedbackWorthySkirmishGameEvent values retain all explicit EP1 forceValue aliases.
public enum Ep1AIFeedbackWorthySkirmishGameEvent { StartGame=0,InTrouble=1,FirstSeen=2,DefeatedLocalPlayer=3,DefeatedByLocalPlayer=4,Lost=5 }

// Reborn: original AIStrategicDirective values retain all explicit EP1 forceValue aliases.
public enum Ep1AIStrategicDirective { Invalid=-1,Normal=0,Assault=1,HoldPosition=2,HoldObject=3,Help=4 }

// Reborn: original AIStrategicDirectiveEvent values retain all explicit EP1 forceValue aliases.
public enum Ep1AIStrategicDirectiveEvent { Begin=0,Complete=1,Cancel=2 }

// Reborn: original AIUnitBuilderUnitChoiceShape values retain all explicit EP1 forceValue aliases.
public enum Ep1AIUnitBuilderUnitChoiceShape { BEST=0,MIDDLE=1,WORST=2,FLAT=3 }

// Reborn: inline personality state IDs are source-inheritance labels; the original runtime id-info word remains zero.
[StructLayout(LayoutKind.Sequential)]
public struct Ep1AIPersonalityStateSlotBase { public uint IdInfo; }
// Reborn: an optional opening-move heuristic owns the original ordered polymorphic list in its own eight-byte record.
[StructLayout(LayoutKind.Sequential)]
public struct Ep1AIHeuristicChoice { public PolymorphicList<Ep1AIStateHeuristic> Value; }

// Reborn: the complete original EP1 AIStrategicState record retains every schema field and source-sized list.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1AIStrategicState
{
    public Ep1AIPersonalityStateSlotBase Base;
    public AssetReference<BaseAssetType> State;
    public AIDifficultyBitFlags Difficulty;
}

// Reborn: the complete original EP1 AIBudgetState record retains every schema field and source-sized list.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1AIBudgetState
{
    public Ep1AIPersonalityStateSlotBase Base;
    public AssetReference<BaseAssetType> State;
    public AIDifficultyBitFlags Difficulty;
}

// Reborn: the complete original EP1 AIBuildState record retains every schema field and source-sized list.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1AIBuildState
{
    public Ep1AIPersonalityStateSlotBase Base;
    public AssetReference<BaseAssetType> State;
    public AIDifficultyBitFlags Difficulty;
}

// Reborn: the complete original EP1 AIBuildDelay record retains every schema field and source-sized list.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1AIBuildDelay
{
    public Time Delay;
    public AIDifficultyBitFlags Difficulty;
}

// Reborn: the complete original EP1 AIResourceMultiplierCheat record retains every schema field and source-sized list.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1AIResourceMultiplierCheat
{
    public Percentage Percentage;
    public AIDifficultyBitFlags Difficulty;
}

// Reborn: the complete original EP1 AIBuildDelayRange record retains every schema field and source-sized list.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1AIBuildDelayRange
{
    public AnsiString id;
    public Time MinDelay;
    public Time MaxDelay;
    public float MinTTKRatio;
    public float MaxTTKRatio;
    public AIDifficultyBitFlags Difficulty;
}

// Reborn: the complete original EP1 AIUnitBuilderUnitChoiceStrategy record retains every schema field and source-sized list.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1AIUnitBuilderUnitChoiceStrategy
{
    public Ep1AIUnitBuilderUnitChoiceShape Shape;
    public float StandardDeviation;
    public AIDifficultyBitFlags Difficulty;
}

// Reborn: the complete original EP1 AIUnitBuilderUnitChoiceStrategyAdaptive record retains every schema field and source-sized list.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1AIUnitBuilderUnitChoiceStrategyAdaptive
{
    public float MinEffectiveness;
    public float MaxEffectiveness;
    public float MinStandardDeviation;
    public float MaxStandardDeviation;
    public float MinEfficiency;
    public float MaxEfficiency;
    public AIDifficultyBitFlags Difficulty;
}

// Reborn: the complete original EP1 AIUnitBuilderEvaluationDelayRange record retains every schema field and source-sized list.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1AIUnitBuilderEvaluationDelayRange
{
    public Time MinDelay;
    public Time MaxDelay;
    public float MinEfficiency;
    public float MaxEfficiency;
    public AIDifficultyBitFlags Difficulty;
    public SageBool UseAllAvailableQueues;
}

// Reborn: the complete original EP1 AIPersonalityDefMap record retains every schema field and source-sized list.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1AIPersonalityDefMap
{
    public AssetReference<BaseAssetType> Side;
    public AssetReference<BaseAssetType> Personality;
}

// Reborn: the complete original EP1 AIWeightedOpeningMove record retains every schema field and source-sized list.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1AIWeightedOpeningMove
{
    public AssetReference<BaseAssetType> Name;
    public uint Priority;
    public Percentage Weight;
    public AIDifficultyBitFlags Difficulty;
    public Ep1AIHeuristicChoice* Heuristic;
}

// Reborn: the complete original EP1 AIStrategicDirectiveFeedbackMap record retains every schema field and source-sized list.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1AIStrategicDirectiveFeedbackMap
{
    public Ep1AIStrategicDirective StrategicDirective;
    public Ep1AIStrategicDirectiveEvent Event;
    public StringHash FeedbackKey;
}

// Reborn: the complete original EP1 AISkirmishGameEventFeedbackMap record retains every schema field and source-sized list.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1AISkirmishGameEventFeedbackMap
{
    public Ep1AIFeedbackWorthySkirmishGameEvent Event;
    public StringHash FeedbackKey;
}

// Reborn: the complete original EP1 AIUnitCap record retains every schema field and source-sized list.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1AIUnitCap
{
    public int Cap;
    public AIDifficultyBitFlags Difficulty;
}

// Reborn: the complete original EP1 AISpecificUnitCap record retains every schema field and source-sized list.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1AISpecificUnitCap
{
    public TypedAssetId<GameObject> Unit;
    public int Cap;
    public AIDifficultyBitFlags Difficulty;
}

// Reborn: the complete original EP1 AIUnitBuilderModifier record retains every schema field and source-sized list.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1AIUnitBuilderModifier
{
    public TypedAssetId<GameObject> Unit;
    public float OffensiveModifier;
    public float DefensiveModifier;
    public AIDifficultyBitFlags Difficulty;
}

// Reborn: the complete original EP1 AIStateFeedbackEntry record retains every schema field and source-sized list.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1AIStateFeedbackEntry
{
    public StringHash FeedbackKey;
    public AssetReference<BaseAssetType> EvaEvent;
    public AssetReference<BaseAudioEventInfo,AudioEventInfo> Sound;
    public AnsiString Text;
    public AnsiString Video;
    public ObjectFilterRelationshipBitMask FeedbackTo;
    public Time* Timeout;
    public AnsiString* ConditionOnFlag;
    public SageBool OnlyPlayIfNoOtherAliveInstancesOfCommander;
}

// Reborn: the complete original EP1 AIPlayerPowerChoice record retains every schema field and source-sized list.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1AIPlayerPowerChoice
{
    public AssetReference<BaseAssetType>* PlayerPower;
    public Percentage Weight;
}

// Reborn: the complete original EP1 AIPersonalityDefinition record retains every schema field and source-sized list.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1AIPersonalityDefinition
{
    public BaseAssetType Base;
    public AnsiString PersonalityType;
    public AnsiString PersonalityUIName;
    public float SecondsTillTargetsExpire;
    public int DesiredExcessPowerBuffer;
    public float BaseCompactness;
    public float DefaultThreatFindRadius;
    public float UnitBuilderCostEffectivenessWeight;
    public float UnitBuilderMoneyVersusTimePreference;
    public Percentage UnitBuilderOverallOffensivePreference;
    public Percentage UnitBuilderOverallDefensivePreference;
    public Percentage UnitBuilderEnemyTowerWeight;
    public float UnitBuilderEnemyNearbyDistance;
    public uint UnitBuilderEnemyNearbyMultiplier;
    public float BuildTechEfficiencyThreshold;
    public float BuildTechMinPowerAdvantage;
    public Time BuildExtraFactoryMinTime;
    public float ResourceNodeSearchRadius;
    public int ResourceNodeValueRemainingForExpansion;
    public float RefineryPlacementStartDistanceDelta;
    public float RefineryPlacementIncreaseTickAmount;
    public int RefineryPlacementNumberOfIncreaseTicksBeforeGivingUp;
    public float ReactiveDefenseRadius;
    public AIDifficultyBitFlags RepairBuildingsAtDifficulty;
    public AIDifficultyBitFlags UsePlayerPowersAtDifficulty;
    public AIDifficultyBitFlags EmergencyManagerHandleNoPowerAtDifficulty;
    public AIDifficultyBitFlags EmergencyManagerHandleNoIncomeAtDifficulty;
    public AIDifficultyBitFlags EmergencyManagerHandleNoConyardAtDifficulty;
    public AIDifficultyBitFlags EmergencyManagerHandleUnderAttackAtDifficulty;
    public AIDifficultyBitFlags EmergencyManagerHandleConyardInDangerAtDifficulty;
    public Time ConyardInDangerEmergencyUnderAttackTime;
    public Percentage ConyardInDangerEmergencyHealthPercent;
    public float ConyardInDangerEmergencyEngineerDistance;
    public Ep1AIEnemySortingType EnemySortingBrutalDifficulty;
    public Ep1AIEnemySortingType EnemySortingHardDifficulty;
    public Ep1AIEnemySortingType EnemySortingMediumDifficulty;
    public Ep1AIEnemySortingType EnemySortingEasyDifficulty;
    public Time EnemyResortingTime;
    public float StructureSaveChanceEasy;
    public float StructureSaveChanceMedium;
    public float StructureSaveChanceHard;
    public Time BaseBuilderKeepUnderAttackFlagOnTime;
    public Time BaseBuilderUpdateUnderAttackTime;
    public Time BaseBuilderUpdateEnemiesInBaseTime;
    public uint MinHarvesters;
    public int MaxStrategicTargets;
    public float AssaultBeaconRadius;
    public float HoldObjectBeaconRadius;
    public float HoldPositionBeaconRadius;
    public float RedAlertPowerAdvantage;
    public float RedAlertIncome;
    public int RedAlertProductionBuildings;
    public Time RedAlertAttackTime;
    public Time MaxWallDestroyTimeToCountAsGoThroughWalls;
    public TypedAssetId<BaseAssetType> CommanderPortrait;
    public Time SkirmishTimeTillGreeting;
    public Time CoopMonitorUpdateTime;
    public List<AssetReference<BaseAssetType>> Side;
    public List<Ep1AIPersonalityDefMap> PersonalityMap;
    public List<Ep1AIUnitCap> UnitCap;
    public List<Ep1AIBuildDelay> BuildDelay;
    public List<Ep1AIBuildDelayRange> BuildDelayRange;
    public List<Ep1AIUnitBuilderUnitChoiceStrategy> UnitBuilderUnitChoiceStrategy;
    public List<Ep1AIUnitBuilderUnitChoiceStrategyAdaptive> UnitBuilderUnitChoiceStrategyAdaptive;
    public List<Ep1AIUnitBuilderEvaluationDelayRange> UnitBuilderEvaluationDelayRange;
    public List<Ep1AIResourceMultiplierCheat> ResourceMultiplierCheat;
    public List<Ep1AIStrategicDirectiveFeedbackMap> StrategicDirectiveFeedback;
    public List<Ep1AISkirmishGameEventFeedbackMap> SkirmishGameEventFeedback;
    public List<Ep1AIPlayerPowerChoice> PowerChoice;
    public List<Ep1AIWeightedOpeningMove> OpeningMove;
    public List<Ep1AIStrategicState> StrategicState;
    public List<Ep1AIBudgetState> BudgetState;
    public List<Ep1AIBuildState> BuildState;
    public List<Ep1AISpecificUnitCap> SpecificUnitCap;
    public List<Ep1AIUnitBuilderModifier> UnitModifier;
    public AIBaseSelectionCriteria* DefaultTacticHomeBaseCriteria;
    public ObjectFilter CountAsStructureForBuildPurposesFilter;
    public Ep1AIInTroubleSkirmishFeedbackControl* InTroubleSkirmishFeedbackControl;
    public List<Ep1AIStateFeedbackEntry> FeedbackEntry;
    public SageBool SkirmishPersonality;
    public SageBool CoopPersonality;
    public SageBool CleanupRefineries;
    public SageBool CanBuildInAlliedBuildRadii;
}

// Reborn: the complete original EP1 AIInTroubleSkirmishFeedbackControl record retains every schema field and source-sized list.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1AIInTroubleSkirmishFeedbackControl
{
    public ObjectFilterRelationshipBitMask OnlyIfLocalPlayerIs;
    public Time TimeoutAfterPlaying;
    public Time TimeBetweenScans;
    public Time TimeThatConditionsMustBeMet;
    public int NumberOfObjectsRecentlyLost;
    public Time TimeForNumberOfObjectLost;
    public AssetReference<BaseAssetType> ObjectLostFilter;
    public float MinPowerAdvantageEnemiesInMyBaseVsAllMyUnits;
}
