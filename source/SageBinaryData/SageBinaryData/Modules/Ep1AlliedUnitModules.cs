using System.Runtime.InteropServices;
using Relo;
using AnsiString=Relo.String<sbyte>;

namespace SageBinaryData;

// Reborn: EP1 BeamOptions uses the five original schema ordinals, stored in one 32-bit mask.
public enum Ep1BeamOptionsType { SWEEP_LEFT,SWEEP_RIGHT,SWEEP_CRISSCROSS,DIAGONAL_SWEEPING,FIRES_MULTIPLE_LASERS }
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1BeamOptionsBitFlags { public const int Count=5, BitsInSpan=32, NumSpans=1; public fixed uint Value[NumSpans]; }

// Reborn: original EP1 ActivateModuleSpecialPower native field order; schema defaults remain source-driven.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1ActivateModuleSpecialPower
{
    public SpecialPowerModuleData Base;
    public List<Ep1ActivateModuleInfo> Modules;
}

// Reborn: original EP1 ActivateModuleInfo native field order; schema defaults remain source-driven.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1ActivateModuleInfo
{
    public SpecialPowerModuleInfo Base;
    public SageBool Deactivate;
}

// Reborn: original EP1 BeamSpecialAbilityUpdateModuleData native field order; schema defaults remain source-driven.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1BeamSpecialAbilityUpdateModuleData
{
    public SpecialAbilityUpdateModuleData Base;
    public AssetReference<AttributeModifier> TargetAttributeModifierAdd;
    public ObjectStatusBitFlags* TargetObjectStatusAdd;
    public AssetReference<FXList> SweepFX;
    public float ReflectorExtendDistance;
    public AnsiString PreferredTargetBone;
    public AssetReference<WeaponTemplate> WeaponTemplate;
    public Ep1BeamOptionsBitFlags BeamOptions;
    public Time BeamDuration;
    public float SweepDistance;
    public Time DamageInterval;
    public SageBool JoinWithOtherBeams;
    public SageBool DissapateWhenTargetDead;
    public SageBool LineType;
    public SageBool TargetSamePlayerOnly;
}

// Reborn: original EP1 DominateEnemySpecialPowerModuleData native field order; schema defaults remain source-driven.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1DominateEnemySpecialPowerModuleData
{
    public SpecialAbilityUpdateModuleData Base;
    public float DominateRadius;
    public AssetReference<FXList> TriggerFX;
    public AssetReference<FXList> DominatedFX;
    public ObjectFilter* AttributeModifierAffects;
    public SageBool PermanentlyConvert;
}

// Reborn: original EP1 InvisibilityBroadcastUpdateModuleData native field order; schema defaults remain source-driven.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1InvisibilityBroadcastUpdateModuleData
{
    public UpdateModuleData Base;
    public AssetReference<BaseAssetType> InvisibilityTemplate;
    public Time UpdatePeriod;
    public float BroadcastRange;
    public TypedAssetId<ModuleData> ScalableDrawModuleId;
    public AnsiString ScaledBoneName;
    public ObjectFilter* BroadcastObjectFilter;
    public ObjectStatusValidationDataType* ObjectStatusValidation;
    public SageBool AffectsSelf;
}

// Reborn: original EP1 LinearDamageUpdateModuleData native field order; schema defaults remain source-driven.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1LinearDamageUpdateModuleData
{
    public UpdateModuleData Base;
    public float StartDistanceOffset;
    public float Radius;
    public float Length;
    public AssetReference<FXList> SweepFXList;
    public AssetReference<WeaponTemplate> SweepWeapon;
    public Velocity Speed;
    public AssetReference<LocomotorTemplate>* Locomotor;
    public ObjectStatusValidationDataType* ObjectStatusValidation;
    public ObjectFilter* BlockingObjectFilter;
    public SageBool FollowTerrain;
}

// Reborn: original EP1 MagneticEffectUpdateModuleData native field order; schema defaults remain source-driven.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1MagneticEffectUpdateModuleData
{
    public UpdateModuleData Base;
    public TypedAssetId<ModuleData> ScalableDrawModuleId;
    public AnsiString ScaledBoneName;
    public float Radius;
    public float MaximumPullVelocity;
    public float MinimumPullVelocity;
    public ObjectStatusType VictimStatusToSet;
    public ObjectStatusValidationDataType* ObjectStatusValidation;
    // Reborn: the required magnetic filter is inline at +40 in retail EP1, unlike its optional status-validation pointer.
    public ObjectFilter ObjectFilter;
}

// Reborn: original EP1 ReconstituteStateSpecialAbilityModuleData native field order; schema defaults remain source-driven.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1ReconstituteStateSpecialAbilityModuleData
{
    public SpecialAbilityUpdateModuleData Base;
    public TypedAssetId<ModuleData> UpdateModuleId;
    public Time TimeStepsBack;
    public AssetReference<FXList> LeavePositionFX;
    public AssetReference<FXList> ReturnPositionFX;
    public AssetReference<AttributeModifier> PostReconstituteAttributeModifier;
}

// Reborn: original EP1 SpecialDisguiseTerrainData native field order; schema defaults remain source-driven.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1SpecialDisguiseTerrainData
{
    public TerrainClassType TerrainClass;
    public TypedAssetId<GameObject> ObjectTemplate;
}

// Reborn: original EP1 SpecialDisguiseUpdateModuleData native field order; schema defaults remain source-driven.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1SpecialDisguiseUpdateModuleData
{
    public SpecialAbilityUpdateModuleData Base;
    public float OpacityTarget;
    public AssetReference<FXList> DisguiseFX;
    public float ScanRadiusForPropDisguise;
    public Time DisguiseIdleDelay;
    public List<Ep1SpecialDisguiseTerrainData> TerrainData;
    public SageBool TriggerInstantlyOnCreate;
}

// Reborn: original EP1 StoreCurrentStateUpdateModuleData native field order; schema defaults remain source-driven.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1StoreCurrentStateUpdateModuleData
{
    public UpdateModuleData Base;
    public Time SleepTime;
    public Time MaxTime;
}

// Reborn: original EP1 ModelConditionEvaEventSelectorOverrideBlock native field order; schema defaults remain source-driven.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1ModelConditionEvaEventSelectorOverrideBlock
{
    public ModelConditionBitFlags RequiredFlags;
    public ModelConditionBitFlags ExcludedFlags;
    public Ep1EvaEventArray* EvaEventArray;
}

// Reborn: original EP1 ModelConditionEvaEventSelectorClientBehaviorModuleData native field order; schema defaults remain source-driven.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1ModelConditionEvaEventSelectorClientBehaviorModuleData
{
    public ClientBehaviorModuleData Base;
    public List<Ep1ModelConditionEvaEventSelectorOverrideBlock> Override;
}

// Reborn: original EP1 UpgradeSoundSelectorOverrideBlock native field order; schema defaults remain source-driven.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1UpgradeSoundSelectorOverrideBlock
{
    public ModelConditionBitFlags RequiredModelConditions;
    public ModelConditionBitFlags ConflictingModelConditions;
    public int* VoicePriorityOverride;
    public Ep1AudioArrayVoice* AudioArrayVoice;
    public Ep1AudioArraySound* AudioArraySound;
    public List<TypedAssetId<UpgradeTemplate>> RequiredUpgrade;
    public List<TypedAssetId<UpgradeTemplate>> ConflictingUpgrade;
}

// Reborn: original EP1 UpgradeSoundSelectorClientBehaviorModuleData native field order; schema defaults remain source-driven.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1UpgradeSoundSelectorClientBehaviorModuleData
{
    public ClientBehaviorModuleData Base;
    public List<Ep1UpgradeSoundSelectorOverrideBlock> Override;
}
