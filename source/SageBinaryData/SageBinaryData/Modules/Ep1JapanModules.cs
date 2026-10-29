using Relo;
using System.Runtime.InteropServices;
namespace SageBinaryData;

// Reborn: original EP1 movement and monitor action ordinals are stored as schema enums and a one-word mask.
public enum SPMovementType { NONE,STRAIGHT_AHEAD,TARGETED }
public enum MonitorConditionActionType { DUMP_ORE }
[StructLayout(LayoutKind.Sequential)]
public unsafe struct MonitorConditionActionBitFlags { public const int Count=1,BitsInSpan=32,NumSpans=1;public fixed uint Value[NumSpans]; }

// Reborn: measured original EP1 ChargeWeaponUpdateModuleData header and independent allocation owners.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1ChargeWeaponUpdateModuleData
{
    public UpdateModuleData Base;
    public Time ChargeTime;
    public Time OverChargeTime;
    public Percentage OverChargePercent;
    public ObjectStatusValidationDataType* ObjectStatusValidation;
}

// Reborn: measured original EP1 PrematureDischargeSpecialPowerModuleData header and independent allocation owners.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1PrematureDischargeSpecialPowerModuleData
{
    public SpecialPowerModuleData Base;
}

// Reborn: measured original EP1 SpiderHoleUpdateModuleData header and independent allocation owners.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1SpiderHoleUpdateModuleData
{
    public UpdateModuleData Base;
    public float Radius;
    public Time ScanFrequency;
    public ObjectFilter ObjectFilter;
}

// Reborn: measured original EP1 ActivateSpecialPowerOnCreate header and independent allocation owners.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1ActivateSpecialPowerOnCreate
{
    public UpdateModuleData Base;
    public List<AssetReference<SpecialPowerTemplate>> SpecialPowerTemplate;
}

// Reborn: measured original EP1 MovementSpecialPowerModuleData header and independent allocation owners.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1MovementSpecialPowerModuleData
{
    public SpecialPowerModuleData Base;
    public SPMovementType MovementType;
    public Time Duration;
    public ModelConditionBitFlags* ModelCondition;
    public ObjectStatusBitFlags* ObjectStatus;
    public Ep1RootLocomotorSetType LocomotorSet;
    public Time BusyDurationOnExit;
    public float CollisionBounceHeight;
    public float CollisionBounceDistance;
    public float CollisionBounceSpeedScalar;
}

// Reborn: measured original EP1 TimedOpacityUpdateModuleData header and independent allocation owners.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1TimedOpacityUpdateModuleData
{
    public UpdateModuleData Base;
    public float Opacity;
    public Time Duration;
}

// Reborn: measured original EP1 MonitorConditionUpdateModuleData header and independent allocation owners.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1MonitorConditionUpdateModuleData
{
    public UpdateModuleData Base;
    public ModelConditionBitFlags ModelConditionFlags;
    public AssetReference<ObjectCreationList> OCL;
    public Time PreDelay;
    public Time PostDelay;
    public MonitorConditionActionBitFlags ActionsOnTriggered;
}

// Reborn: measured original EP1 LifetimeUpdateModuleData header and independent allocation owners.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1LifetimeUpdateModuleData
{
    public UpdateModuleData Base;
    public Time MinLifetime;
    public Time MaxLifetime;
    public DeathType DeathType;
    public AssetReference<BaseAudioEventInfo,AudioEventInfo> CountdownSound;
    public AssetReference<BaseAudioEventInfo,AudioEventInfo> CountdownCompleteSound;
    public Time TimeBeforeDeathThatCountdownCompletes;
    public Time TimeBetweenCountdownSounds;
    public int* NumberOfCountdownSoundPlays;
    public SageBool WaitForWakeUp;
    public SageBool ScoreKill;
    public SageBool IgnoreProjectileState;
}
