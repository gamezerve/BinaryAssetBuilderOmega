using Relo;
using System.Runtime.InteropServices;
using AnsiString = Relo.String<sbyte>;

namespace SageBinaryData;

// Reborn: EP1 ObjectCreationList ABI. Offsets/sizes follow the official RA3 marshaler (identical OCL schema except one inserted
// OCNuggetOption) and are re-qualified against original EP1 stock OCLs before any authored list is emitted. The legacy
// ObjectCreationList model (three lists, old option/disposition order, ParticleSystem field) is not the EP1 layout and stays unused.
// Reborn: enumerations are the official EP1 XSD order; the marshaler parses names, so C# ordinals must equal XSD positions.
public enum Ep1DispositionType{DISPOSITION_NONE,LIKE_EXISTING,ON_GROUND_ALIGNED,SEND_IT_FLYING,SEND_IT_UP,SEND_IT_OUT,RANDOM_FORCE,FLOATING,INHERIT_VELOCITY,FORWARD_IMPACT,REVERSE_IMPACT,BUILDING_CHUNKS,ANIMATED,ABSOLUTE_ANGLE,SPAWN_AROUND,RELATIVE_ANGLE,USE_WATER_SURFACE,USE_CLIFF,CLAMP_TO_GROUND,FALL_TO_GROUND,USE_DYNAMICS_FOR_FLING}
public enum Ep1OCNuggetOption{OCCOMMONNUGGETOPTION_NONE,ORIENT_IN_FORCE_DIRECTION,ISSUE_MOVE_AFTER_CREATION,ISSUE_ATTACK_AFTER_CREATION,ORIENT_IN_PRIMARY_DIRECTION,ORIENT_IN_SECONDARY_DIRECTION,CLEAR_REMOVEABLES,MOVE_USES_STRAFE_UPDATE,MOVE_TARGET_USES_DISPOSITION_OFFSET,MOVE_USES_EVACUATE_AND_EXIT,OFFSET_IN_LOCAL_SPACE,SPREAD_FORMATION,FADE_IN,FADE_OUT,PRESERVE_LAYER,IGNORE_ALL_OBJECTS,IGNORE_ENEMY_UNITS,IGNORE_ALLY_UNITS,CONTAIN_INSIDE_SOURCE_OBJECT,SKIP_IF_SIGNIFICANTLY_AIRBORNE,REQUIRES_LIVE_PLAYER,IGNORE_COMMANDPOINT_LIMIT,INHERIT_ATTRIBUTES_FROM_SOURCE,INHERIT_SCRIPTING_NAME,INHERIT_PRICE_PURCHASED,OK_TO_CHANGE_MODEL_COLOR,DONT_SET_PRODUCER,SCALE_TIBERIUM_FIELD,SCALE_ION_STORM,DO_NOT_TREAT_AS_STRUCTURE,USE_CREATORS_HEALTH_AS_BASE,USE_CREATORS_HEALTH_PERCENTAGE,COPY_CREATORS_UPGRADES,MOVE_TARGET_USES_OFFSET,TRANSFER_TEMPORARY_SLAVE,COPY_PRODUCTION_QUEUE_INDEX,CREATE_AT_TARGET,RETURN_TO_SOURCE,ATTACH_TO_SOURCE,CREATE_AT_SOURCE_IF_TARGET_REFLECTS,ORIENT_TOWARD_MOVE_DESTINATION,OWNED_BY_SECONDARY_OBJECT,FIRE_SPECIAL_POWER,DO_NOT_COPY_SHADERS,DO_NOT_COPY_SCALE}
public enum Ep1OCLType{NONE,CHOOSE_ONE_CREATE_NUGGET_FROM_LIST}
public enum Ep1ParabolicOption{TRACK_TARGET_OBJECT}

[StructLayout(LayoutKind.Sequential)] public struct Ep1DispositionTypeFlag{public const int Count=21;public const int BitsInSpan=32;public const int NumSpans=(Count+(BitsInSpan-1))/BitsInSpan;public unsafe fixed uint Value[NumSpans];}
[StructLayout(LayoutKind.Sequential)] public struct Ep1OCNuggetOptionFlag{public const int Count=45;public const int BitsInSpan=32;public const int NumSpans=(Count+(BitsInSpan-1))/BitsInSpan;public unsafe fixed uint Value[NumSpans];}
[StructLayout(LayoutKind.Sequential)] public struct Ep1OCLBitFlags{public const int Count=2;public const int BitsInSpan=32;public const int NumSpans=(Count+(BitsInSpan-1))/BitsInSpan;public unsafe fixed uint Value[NumSpans];}
[StructLayout(LayoutKind.Sequential)] public struct Ep1ParabolicOptionFlag{public const int Count=1;public const int BitsInSpan=32;public const int NumSpans=(Count+(BitsInSpan-1))/BitsInSpan;public unsafe fixed uint Value[NumSpans];}

// Reborn: the polymorphic nugget base word is not written by the official marshaler (runtime-owned); stock data keeps it zero.
[StructLayout(LayoutKind.Sequential,Size=4)] public struct Ep1OCNugget{}

// Reborn: 248 bytes; official offsets noted where alignment is not obvious.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1CreateObjectNugget
{
    public Ep1OCNugget Base;
    public TypedAssetId<GameObject> PutInContainer;            // +4
    public AssetReference<FXList> CreateFX;                   // +8
    public uint Count;                                        // +12
    public Ep1OCNuggetOptionFlag Options;                     // +16 (2 words)
    public Ep1DispositionTypeFlag Disposition;                // +24
    public float VelocityScale;                               // +28
    public float MinForceMagnitude,MaxForceMagnitude;         // +32,+36
    public Time MinLifetime,MaxLifetime;                      // +40,+44
    public float MinDistanceAFormation,MinDistanceBFormation,MaxDistanceFormation; // +48..+56
    public Time FadeTime;                                     // +60
    public ModelConditionBitFlags StartingModelConditions;    // +64 (15 words)
    public ModelConditionFlagType TempModelCondition;         // +124
    public Time TempModelConditionTime;                       // +128
    public Velocity DispositionIntensity;                     // +132
    public AnsiString* DestinationPlayer;                     // +136
    public AnsiString WaypointSpawn;                          // +140
    public Angle DispositionAngle,MinForcePitch,MaxForcePitch,MinForceYaw,MaxForceYaw,OrientationOffset; // +148..+168
    public AssetReference<BaseAudioEventInfo,AudioEventInfo> FadeSound; // +172
    public Percentage MinHordeDensity,MaxHordeDensity;        // +176,+180
    public float MinHealth,MaxHealth;                         // +184,+188
    public Time InvulnerableTime,StartingBusyTime,JustBuiltFrames; // +192..+200
    public int VeterancyLevel;                                // +204
    public AssetReference<SpecialPowerTemplate> FireSpecialPowerTemplate; // +208
    public ObjectStatusType* StatusToCopyFromParent;          // +212
    public Coord3D* Offset;                                   // +216
    public List<AssetReference<UpgradeTemplate>> RequiredUpgrade; // +220
    public List<AssetReference<UpgradeTemplate>> ForbiddenUpgrade; // +228
    public List<TypedAssetId<GameObject>> CreateObject;       // +236
    public SageBool UseUpgradedLocomotor;                     // +244
    public SageBool DisabledWhileBusy;                        // +245
}

[StructLayout(LayoutKind.Sequential)] public struct Ep1AttackNugget{public Ep1OCNugget Base;public uint NumberOfShots;public WeaponSlotType WeaponSlot;public float DeliveryDecalRadius;}
[StructLayout(LayoutKind.Sequential)] public struct Ep1FireWeaponNugget{public Ep1OCNugget Base;public AssetReference<WeaponTemplate> Weapon;}

[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1FlingObjectNugget
{
    public Ep1OCNugget Base;
    public TypedAssetId<GameObject> CreateObject;
    public float Speed,Height;
    public uint Count;
    public Time InitialDelay;
    public Coord3D* Offset;
    public RealRange DistanceFromTarget,AngleAroundTarget;
    public RealRange* DistanceAtTarget;
    public SageBool UsePrimaryObjectForOffsets;
}

[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1StrafeAreaNugget
{
    public Ep1OCNugget Base;
    public TypedAssetId<GameObject> CreateObject;
    public uint Count;
    public float Height,DistanceFromTarget;
    public Coord3D* InterOffset,IntraOffset;
    public SageBool MaintainOffestToTarget;
}

[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1ParabolicCurveNugget
{
    public Ep1OCNugget Base;
    public Ep1ParabolicOptionFlag Options;
    public TypedAssetId<GameObject> CreateObject;
    public Time InitialDelay,ApproachTime;
    public float StartDistanceFromTarget,EndDistanceFromTarget,CruiseHeight,AttackHeight;
    public Angle ApproachRoll,LeaveRoll;
    public Coord3D* Offset;
}

// Reborn: 56 bytes; XML element names are the list field names (StrafeAreaSquad holds StrafeAreaNugget records).
[StructLayout(LayoutKind.Sequential)]
public struct Ep1ObjectCreationList
{
    public BaseAssetType Base;
    public Ep1OCLBitFlags Flags;
    public List<Ep1CreateObjectNugget> CreateObject;
    public List<Ep1AttackNugget> Attack;
    public List<Ep1FireWeaponNugget> FireWeapon;
    public List<Ep1FlingObjectNugget> FlingObject;
    public List<Ep1StrafeAreaNugget> StrafeAreaSquad;
    public List<Ep1ParabolicCurveNugget> ParabolicCurve;
}
