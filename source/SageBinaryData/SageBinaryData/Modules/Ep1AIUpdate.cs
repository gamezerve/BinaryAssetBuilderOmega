using Relo;
using System.Runtime.InteropServices;
using AnsiString=Relo.String<sbyte>;

namespace SageBinaryData;

// Reborn: EP1 state-machine values follow the independently checked official enum; legacy derived AI models remain unchanged.
public enum Ep1AIStateMachineType
{
    AIStateMachine, CombinedHelicopterAIStateMachine, HelicopterAIStateMachine, HordeAIStateMachine,
    JetAIStateMachine, OffensiveStructureAIStateMachine, StructureAIStateMachine, UnitAIStateMachine,
    OreNodeStateMachine, OreMinerStateMachine, OreMinerOffensiveStateMachine, OreRefineryStateMachine,
    DisguiseWhenIdleAIStateMachine, TransportHelicopterAIStateMachine, StrafingRunAIStateMachine,
    DiveBomberJetAIStateMachine, GunshipJetAIStateMachine
}

// Reborn: these four literal positions encode independent flag bits, not their ordinal values as a scalar.
public enum Ep1DynamicTargetFlag { NEVER, ATTACK_MOVE, AGGRESSIVE_STANCE, ALWAYS }

// Reborn: retail SovietAntiVehicleInfantry vision items are 128 bytes: zero runtime AssetIdInfo* word (BaseAssetType id is not stored), Range at +4, ObjectFilter inline at +8.
[StructLayout(LayoutKind.Sequential)]
public struct Ep1ObjectVisionRangeItem
{
    public uint IdInfo;
    public Percentage Range;
    public ObjectFilter Filter;
}

// Reborn: EP1 chooser adds a dynamic-target mask, vision-list storage and two booleans before its strong reference at +36.
[StructLayout(LayoutKind.Sequential)]
public struct Ep1UnitAITargetChooserData
{
    public Time IdleScanDelay;
    public Time ReacquireDelay;
    public Time StartupDelay;
    public float SympathyRange;
    public uint CanPickDynamicTargetType;
    public List<Ep1ObjectVisionRangeItem> ObjectVisionRange;
    public SageBool CheckVisionRange;
    public SageBool CanPickDynamicTargets;
    public SageBool RotateToTargetWhenAiming;
    public SageBool CanAutoAcquireNonAutoAcquirable;
    public SageBool CanChaseAttack;
    public SageBool CanOnlyAttackTargetsInFront;
    public AssetReference<TargetingCompareList> TargetingCompareList;
}

// Reborn: stock EP1 AIUpdate is 76 bytes, not the legacy KW graph; optional chooser/list payloads belong to the same Tracker.
[StructLayout(LayoutKind.Sequential)]
public struct Ep1AIUpdateModuleData
{
    public UpdateModuleData Base;
    public AnsiString AttackPriority;
    public uint AutoAcquireEnemiesWhenIdle;
    public float StopChaseDistance;
    public Time MinCowerTime;
    public Time MaxCowerTime;
    public Time BurningDeathTime;
    public float ComboLocoAttackDistance;
    public List<AnsiString> SpecialContactPoints;
    public Time IdleTargetTime;
    public uint MaxCombineChildren;
    public float SpawnOffsetRadius;
    public Ep1AIStateMachineType StateMachine;
    public Ep1RootLocomotorSetType DefaultLocomotorSet;
    public unsafe Ep1UnitAITargetChooserData* UnitAITargetChooserData;
    public SageBool StandGround;
    public SageBool FadeOnPortals;
}
