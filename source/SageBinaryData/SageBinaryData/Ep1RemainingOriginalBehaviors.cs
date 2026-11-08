using Relo;
using System.Runtime.InteropServices;
using AnsiString=Relo.String<sbyte>;
namespace SageBinaryData;

// Reborn: all MoneyCrateCollideModuleData fields require independent original source/native proof before emission.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1MoneyCrateCollideModuleData
{
    public Ep1CrateCollideModuleData Base;
    public uint MoneyProvided;
}

// Reborn: all UnitCrateCollideModuleData fields require independent original source/native proof before emission.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1UnitCrateCollideModuleData
{
    public Ep1CrateCollideModuleData Base;
    public uint UnitCount;
    public TypedAssetId<GameObject> UnitType;
}

// Reborn: all VeterancyCrateCollideModuleData fields require independent original source/native proof before emission.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1VeterancyCrateCollideModuleData
{
    public Ep1CrateCollideModuleData Base;
    public uint RangeOfEffect;
    public int AffectsUpToLevel;
    public SageBool AddsOwnerVeterancy;
    public SageBool IsPilot;
}

// Reborn: all SalvageCrateCollideModuleData fields require independent original source/native proof before emission.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1SalvageCrateCollideModuleData
{
    public Ep1CrateCollideModuleData Base;
    public float PorterChance;
    public float BannerChance;
    public float LevelUpChance;
    public float LevelUpRadius;
    public float ResourceChance;
    public int MinimumResource;
    public int MaximumResource;
    public AssetReference<UpgradeTemplate> Upgrade;
    public SageBool AllowAIPickup;
}

// Reborn: all RandomCrateCollideModuleData fields require independent original source/native proof before emission.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1RandomCrateCollideModuleData
{
    public Ep1CrateCollideModuleData Base;
    public float AttributeCrateChance;
    public float UnitCrateChance;
    public float BombCrateChance;
    public float HealCrateChance;
    public float MoneyCrateChance;
    public float ShroudCrateChance;
    public float VeterancyCrateChance;
    public SageBool AllowAIPickup;
}

// Reborn: all SpawnCrateUpdateModuleData fields require independent original source/native proof before emission.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1SpawnCrateUpdateModuleData
{
    public UpdateModuleData Base;
    public uint MaxCrates;
    public Time CreateFrequency;
    public Time CrateLifetime;
    public float MapExclusionMargin;
    public float BlockingUnitRange;
    public float BlockingStartPositionRange;
    public SpawnCrateUpdateFlagsBitFlags Flags;
    public List<AssetReference<GameObject>> CrateList;
    public ObjectFilter* BlockingUnits;
}

// Reborn: all EnergyDisablingBehaviorModuleData fields require independent original source/native proof before emission.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1EnergyDisablingBehaviorModuleData
{
    public BehaviorModuleData Base;
}

// Reborn: all UnitVisionBehaviorModuleData fields require independent original source/native proof before emission.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1UnitVisionBehaviorModuleData
{
    public BehaviorModuleData Base;
    public int UpdateRate;
}

// Reborn: all SqueezeFlingUpdateModuleData fields require independent original source/native proof before emission.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1SqueezeFlingUpdateModuleData
{
    public UpdateModuleData Base;
    public Time ScanTime;
    public Time DelayBetweenVictimsTime;
    public float EffectDistance;
    public float GatherAcceleration;
    public float ModelLength;
    public AssetReference<GameObject>* ArmTemplate;
    public AssetReference<ShaderOverride>* VictimShader;
    public float HeightOffGround;
    public ObjectFilter Filter;
    public ObjectFilter* KillFilter;
    public List<Ep1SqueezeFlingParticleDataType> ParticleData;
}

// Reborn: all SqueezeFlingParticleDataType fields require independent original source/native proof before emission.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1SqueezeFlingParticleDataType
{
    public AnsiString SourceBone;
    public AnsiString DestinationBone;
    public AssetReference<FXParticleSystemTemplate> ParticleSystem;
}

// Reborn: all ReplaceDrawModuleBehaviorModuleData fields require independent original source/native proof before emission.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1ReplaceDrawModuleBehaviorModuleData
{
    public BehaviorModuleData Base;
    public SageBool ConsumeItem;
}

// Reborn: all GatherEjectUpdateModuleData fields require independent original source/native proof before emission.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1GatherEjectUpdateModuleData
{
    public UpdateModuleData Base;
    public Time Lifetime;
    public int Slots;
    public Time ScanTime;
    public float HeightOffGround;
    public float EffectDistance;
    public float LiftOffGroundDistance;
    public float GatherAcceleration;
    public Vector3 RotationSpeed;
    public Vector3 UnitSpacing;
    public ObjectFilter Filter;
}

// Reborn: original PC factory allocates 24 bytes; its field loader stores ComradePercentage at +20 after the native-qualified Capture base.
[StructLayout(LayoutKind.Sequential)]
public struct DistributeEnergyToAlliesBehaviorModuleData
{
    public CaptureBehaviorModuleData Base;
    public Percentage ComradePercentage;
}
