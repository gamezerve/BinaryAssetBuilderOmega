using Relo;
using System.Runtime.InteropServices;
using AnsiString=Relo.String<sbyte>;
namespace SageBinaryData;

// Reborn: official EP1 DOTNuggetType fields, inherited prefix and trailing packed flags.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1DOTNuggetType
{
    public DamageNuggetType Base;
    public Time DamageInterval;
    public Time DamageDuration;
    public SageBool RemoveIfHealed;
}

// Reborn: official EP1 DamageAndSpawnNuggetType fields, inherited prefix and trailing packed flags.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1DamageAndSpawnNuggetType
{
    public DamageNuggetType Base;
    public float SpawnProbability;
    public ModelConditionBitFlags SpawnedModelConditionFlags;
    public TypedAssetId<GameObject> SpawnTemplate;
}

// Reborn: official EP1 TerrainCraterNuggetType fields, inherited prefix and trailing packed flags.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1TerrainCraterNuggetType
{
    public WeaponEffectNugget Base;
    public float Depth;
    public Percentage Lift;
}

// Reborn: official EP1 WeaponOCLNuggetType fields, inherited prefix and trailing packed flags.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1WeaponOCLNuggetType
{
    public WeaponEffectNugget Base;
    public AssetReference<ObjectCreationList> WeaponOCL;
    public SageBool TargetAsPrimaryObject;
}

// Reborn: official EP1 ActivateLaserNuggetType fields, inherited prefix and trailing packed flags.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1ActivateLaserNuggetType
{
    public WeaponEffectNugget Base;
    public Time Lifetime;
    public int LaserId;
    public AssetReference<FXList> HitGroundFX;
    public float OverShootDistance;
    public SageBool OnlyIfInactive;
}

// Reborn: official EP1 ActivateStreamNuggetType fields, inherited prefix and trailing packed flags.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1ActivateStreamNuggetType
{
    public WeaponEffectNugget Base;
    public Time Lifetime;
    public int StreamId;
}

// Reborn: official EP1 ActivateLiftObjectNuggetType fields, inherited prefix and trailing packed flags.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1ActivateLiftObjectNuggetType
{
    public WeaponEffectNugget Base;
    public Time Lifetime;
    public int LiftObjectLinkID;
}

// Reborn: official EP1 ActivateLinearDamageNuggetType fields, inherited prefix and trailing packed flags.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1ActivateLinearDamageNuggetType
{
    public WeaponEffectNugget Base;
    public Time Lifetime;
}

// Reborn: official EP1 ActivateCircularDamageNuggetType fields, inherited prefix and trailing packed flags.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1ActivateCircularDamageNuggetType
{
    public WeaponEffectNugget Base;
    public Time Lifetime;
}

// Reborn: official EP1 SlavesAttackNuggetType fields, inherited prefix and trailing packed flags.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1SlavesAttackNuggetType
{
    public WeaponEffectNugget Base;
    public SageBool SkipSlavesCanAttackCheck;
}

// Reborn: official EP1 LeechHealthDamageNuggetType fields, inherited prefix and trailing packed flags.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1LeechHealthDamageNuggetType
{
    public DamageNuggetType Base;
    public Percentage PercentDamageToHeal;
}

// Reborn: official EP1 SecondaryDamageNuggetType fields, inherited prefix and trailing packed flags.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1SecondaryDamageNuggetType
{
    public WeaponEffectNugget Base;
    public int Amount;
    public float DamageTaperOff;
    public Angle Arc;
    public Percentage PercentDamageToContained;
    public SageBool ArcInverted;
}

// Reborn: official EP1 SpecialPowerNuggetType fields, inherited prefix and trailing packed flags.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1SpecialPowerNuggetType
{
    public WeaponEffectNugget Base;
    public AssetReference<SpecialPowerTemplate> SpecialPowerTemplate;
}

// Reborn: official EP1 AttributeModifierNuggetType fields, inherited prefix and trailing packed flags.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1AttributeModifierNuggetType
{
    public WeaponEffectNugget Base;
    public AssetReference<AttributeModifier> AttributeModifierName;
    public AssetReference<AttributeModifier> AttributeModifierOwnerName;
    public DamageFXType DamageFXType;
    public Angle DamageArc;
    public AttributeModifierCategoryBitFlags AntiCategories;
    public AssetReference<FXList> AntiFX;
}

// Reborn: official EP1 DamageContainedNuggetType fields, inherited prefix and trailing packed flags.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1DamageContainedNuggetType
{
    public DamageNuggetType Base;
    public int MaxUnitsToDamage;
    public AssetReference<FXList> WindowBlastFX;
    public ObjectFilter DamageObjectFilter;
}

// Reborn: official EP1 LuaEventNuggetType fields, inherited prefix and trailing packed flags.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1LuaEventNuggetType
{
    public WeaponEffectNugget Base;
    public AnsiString EventName;
    public SageBool SendToEnemies;
    public SageBool SendToAllies;
    public SageBool SendToNeutral;
}

// Reborn: official EP1 LineDamageNuggetType fields, inherited prefix and trailing packed flags.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1LineDamageNuggetType
{
    public DamageNuggetType Base;
    public Angle OffsetAngle;
    public float LineWidth;
    public float LineLengthLeadIn;
    public float LineLengthLeadOut;
    public float OverShootDistance;
    public SageBool UseDynamicLineLength;
}

// Reborn: official EP1 SeedTiberiumNuggetType fields, inherited prefix and trailing packed flags.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1SeedTiberiumNuggetType
{
    public Ep1WeaponOCLNuggetType Base;
    public float FieldAmount;
    public float SpawnedInFieldBonus;
}

// Reborn: official EP1 ParalyzeNuggetType fields, inherited prefix and trailing packed flags.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1ParalyzeNuggetType
{
    public WeaponEffectNugget Base;
    public Angle EffectArc;
    public Time DurationSeconds;
    public ParalyzeEffectType ParalyzeType;
    public ParalyzeEffectType RemoveParalyzeType;
    public AssetReference<FXList> ParalyzeFX;
    public SageBool OnlyAffectOwnerWhenTriggered;
}

// Reborn: official EP1 InformationWarfareNuggetType fields, inherited prefix and trailing packed flags.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1InformationWarfareNuggetType
{
    public WeaponEffectNugget Base;
    public InfoWarEffectBitFlags InfoWarType;
    public float RadarJamRadius;
    public Time RadarJamDuration;
}

// Reborn: official EP1 SpendStolenTiberiumNuggetType fields, inherited prefix and trailing packed flags.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1SpendStolenTiberiumNuggetType
{
    public WeaponEffectNugget Base;
    public int AmountToSpend;
}

// Reborn: official EP1 ReportWeaponFiredStatNuggetType fields, inherited prefix and trailing packed flags.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1ReportWeaponFiredStatNuggetType
{
    public WeaponEffectNugget Base;
}

// Reborn: official EP1 ScatterProjectileNuggetType fields, inherited prefix and trailing packed flags.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1ScatterProjectileNuggetType
{
    public WeaponEffectNugget Base;
    public float ScatterMin;
    public float ScatterMax;
}

// Reborn: official EP1 FireOnObjectsNuggetType fields, inherited prefix and trailing packed flags.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1FireOnObjectsNuggetType
{
    public DamageNuggetType Base;
    public AssetReference<WeaponTemplate> Weapon;
    public ObjectFilter* Filter;
    public SageBool FireOnVictimObject;
    public SageBool VictimMustBeAlive;
}

// Reborn: official EP1 TintObjectsNuggetType fields, inherited prefix and trailing packed flags.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1TintObjectsNuggetType
{
    public WeaponEffectNugget Base;
    public Time PreColorTime;
    public Time PostColorTime;
    public Time SustainedColorTime;
    public float Frequency;
    public float Amplitude;
    public RGBColor Color;
}

// Reborn: official EP1 ContainedObjectAttackNuggetType fields, inherited prefix and trailing packed flags.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1ContainedObjectAttackNuggetType
{
    public WeaponEffectNugget Base;
}

// Reborn: official EP1 InformTargetNuggetType fields, inherited prefix and trailing packed flags.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1InformTargetNuggetType
{
    public WeaponEffectNugget Base;
    public float MinTimeToImpactFudgeFactor;
    public float MaxTimeToImpactFudgeFactor;
}

// Reborn: official EP1 AttachNugget fields, inherited prefix and trailing packed flags.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1AttachNugget
{
    public WeaponEffectNugget Base;
    public TypedAssetId<ModuleData> AttachModuleId;
}

// Reborn: official EP1 StripMaxHealthPercentNuggetType fields, inherited prefix and trailing packed flags.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1StripMaxHealthPercentNuggetType
{
    public WeaponEffectNugget Base;
    public Percentage AmountToStrip;
    public DamageType DamageType;
    public DeathType DeathType;
    public DamageFXType DamageFXType;
}

// Reborn: official EP1 GrapplingHookNuggetType fields, inherited prefix and trailing packed flags.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1GrapplingHookNuggetType
{
    public DamageNuggetType Base;
}

// Reborn: official EP1 LeechPercentMaxHealthDamageNuggetType fields, inherited prefix and trailing packed flags.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1LeechPercentMaxHealthDamageNuggetType
{
    public DamageNuggetType Base;
    public Percentage PercentMaxHealthToTake;
}
