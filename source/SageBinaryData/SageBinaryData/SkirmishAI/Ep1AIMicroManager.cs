using System.Runtime.InteropServices;
using Relo;
namespace SageBinaryData;

// Reborn: retain every original EP1 special-power heuristic ordinal.
public enum Ep1AISpecialPowerHeuristic { MOVEMENT,CAPTURE,UNPACK,PACK,AOE_ATTACK,TOGGLE,TIME_WARP,DOMINATE,DISGUISE,JUMP,DEPLOY,SPEED_BOOST,DUMB_FIRE,UPGRADE,INSTAKILL,INVULNERABILITY,ATTACHED_BOMB,RUSH_ATTACK,DISCHARGE,PLAYER_AOE_ATTACK,PLAYER_AOE_DEBUFF,PLAYER_AOE_BUFF,PLAYER_AOE_SHROUD_CLEAR,PLAYER_POSITION_SWAP,PLAYER_POWER_MAGNETIC_SATELLITE }

// Reborn: complete original EP1 TargetBiasInfo layout, retaining lists, optional records and trailing flags.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1AITargetBiasInfo
{
    public TypedAssetId<GameObject> UnitName;
    public float HealthMultiplier;
    public float DamageMultiplier;
    public float HealthOverride;
    public float DamageOverride;
}

// Reborn: complete original EP1 SpecificSpecialPowerDelayInfo layout, retaining lists, optional records and trailing flags.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1AISpecificSpecialPowerDelayInfo
{
    public Ep1AISpecialPowerHeuristic PowerType;
    public Ep1ToggleStatusAiHint TogglePowerSubType;
    public Time MinimumDelay;
    public Time MaximumDelay;
}

// Reborn: complete original EP1 AIMicroManagerData layout, retaining lists, optional records and trailing flags.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1AIMicroManagerData
{
    public BaseAssetType Base;
    public Time UpdateTime;
    public Time MaxTeamIdleTime;
    public float MaxTeamIdleDistanceChange;
    public Time MaxUnitIdleTime;
    public float MaxUnitIdleDistanceChange;
    public float PassiveTargetSearchDistance;
    public float EnemySearchDistanceVisionMultiplier;
    public Time MinTimeToKeepTempVisionRangeBonus;
    public Time TimeToKeepTargetMin;
    public Time TimeToKeepTargetMax;
    public float RandomTargetScoreModifier;
    public float CleanupPowerAdvantage;
    public float GatherTempVisionRangeBonus;
    public float RetreatPowerAdvantage;
    public float RetreatMinDistanceFromBase;
    public float RetreatMinDistanceToFallback;
    public float RetreatSpeedBuffer;
    public float RetreatTempVisionRangeBonus;
    public float MaxDistanceFromTeamToIncludeInArmy;
    public float AlphaStrikeTargetHealthPct;
    public float AlphaStrikeTargetTimeToKill;
    public float AlphaStrikeEngineerDistanceFromStructure;
    public float SiegeWeaponBuffer;
    public float CrushPreference;
    public float EnterGarrisonableStructureDistance;
    public float PullbackInjuredHealthPct;
    public Time DodgeThreatsReactionTimeMin;
    public Time DodgeThreatsReactionTimeMax;
    public Time SpecialPowerDelayMinimum;
    public Time SpecialPowerDelayMaximum;
    public float MinDistanceFromBaseToIgnoreTargets;
    public float MaxSecondaryDamageFraction;
    public float FractionBlockedToDestroyWalls;
    public List<Ep1AITargetBiasInfo> TargetBias;
    public List<Ep1AISpecificSpecialPowerDelayInfo> SpecificSpecialPowerDelay;
    public AIBaseSelectionCriteria* RetreatBaseCriteria;
    public ObjectFilter* IgnoreTargets;
    public SageBool EngageHostileTargets;
    public SageBool EngagePassiveTargets;
    public SageBool EngageTacticalTarget;
    public SageBool UseGatherState;
    public SageBool CanUseSiegeState;
    public SageBool Crush;
    public SageBool EnterGarrisons;
    public SageBool ClearGarrisons;
    public SageBool PullbackInjured;
    public SageBool ReverseMove;
    public SageBool DodgeThreats;
    public SageBool UseSpecialPowers;
    public SageBool EnterBaseDefenses;
    public SageBool EnterBaseDefensesWhenIdle;
    public SageBool IsHarvesterManager;
    public SageBool CanCollectCratesWhenIdle;
}
