using System.Runtime.InteropServices;
using Relo;

namespace SageBinaryData;

// Reborn: EP1 AI heuristic and manager ordinals follow the official Uprising schema.
public enum Ep1AiSpecialPowerHeuristic { MOVEMENT, CAPTURE, UNPACK, PACK, AOE_ATTACK, TOGGLE, TIME_WARP, DOMINATE, DISGUISE, JUMP, DEPLOY, SPEED_BOOST, DUMB_FIRE, UPGRADE, INSTAKILL, INVULNERABILITY, ATTACHED_BOMB, RUSH_ATTACK, DISCHARGE, PLAYER_AOE_ATTACK, PLAYER_AOE_DEBUFF, PLAYER_AOE_BUFF, PLAYER_AOE_SHROUD_CLEAR, PLAYER_POSITION_SWAP, PLAYER_POWER_MAGNETIC_SATELLITE }
// Reborn: preserve every official manager alternative.
public enum Ep1AiSpecialPowerManager { SKIRMISH_AI, TACTIC, MICRO_MANAGER, BUILDER }

// Reborn: 188-byte Uprising AI record; status filters are optional pointers at +164/+168, weapon at +176 and flags at +180..185.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1AiSpecialPowerInfo
{
    public uint Heuristic;
    public uint Manager;
    public float Distance;
    public float HealthFraction;
    public KindOfBitFlags TargetObjectInclude;
    public KindOfBitFlags TargetObjectExclude;
    public float* UseOnlyWhenPowerAdvantageIsAbove;
    public Time InvulnerabilityDamageLookAheadTime;
    public Time InvulnerabilityDamageLookBehindTime;
    public float InvulnerabilityDamageFractionWhenOutOfRangeOfTarget;
    public float InvulnerabilityDamageFractionWhenInRangeOfTarget;
    public Angle InvulnerabilityAvoidUseIfAngleToNextPathNodeIsMoreThan;
    public float MaxTargetSpeed;
    public float MinKillValue;
    public int MinTargetsHit;
    public int MinEnemiesNearby;
    // Reborn: optional references allocate their own import-index pointee, as demonstrated by ChronoSwapSelect.
    public AssetReference<LogicCommand>* NextLogicCommand;
    public uint UseInMicroManagerState;
    public AssetReference<GameObject>* UpdateObject;
    public float SearchRadius;
    public uint Location;
    public int* MaxUpdatables;
    public SageBool* UseToCounterGarrison;
    public ObjectStatusBitFlags* RequiredForAttackObjectStatus;
    public ObjectStatusBitFlags* ForbiddenForAttackObjectStatus;
    public ObjectFilter* ObjectFilter;
    public AISpecialPowerWeaponData* SpecialPowerWeapon;
    public SageBool NeverUseWhileRetreating;
    public SageBool NeverUseWhilePullingBack;
    public SageBool RequireKillTargets;
    public SageBool UseNearTacticalTarget;
    public SageBool UseWhenOutOfAmmo;
    public SageBool DontUseIfOutRangeTarget;
}
