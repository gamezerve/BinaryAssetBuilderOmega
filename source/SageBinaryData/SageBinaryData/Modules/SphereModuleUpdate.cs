using System.Runtime.InteropServices;
using Relo;
using AnsiString = Relo.String<sbyte>;

namespace SageBinaryData;

[StructLayout(LayoutKind.Sequential)]
public struct SphereModuleUpdateModuleData
{
    public UpdateModuleData Base;
    public float RadiusMax;
    public float RadiusMin;
    public DamageBitFlags DamageTypesNotToAbsorb;
    public Time ScanFrequency;
    public Time Duration;
    public AnsiString SphereBoneName;
    public float SphereSizeMultiplier;
    public unsafe Coord3D* PositionOffset;
    public ObjectFilter ObjectFilter;
    public unsafe ObjectFilter* IgnoreInsideToInsideCheck;
    public SageBool InitiallyActive;
    public SageBool DrawDebugCircle;
}

[StructLayout(LayoutKind.Sequential)]
public struct DamageSphereUpdateModuleData
{
    public SphereModuleUpdateModuleData Base;
    public Time UnpackTime;
    public AssetReference<WeaponTemplate> Weapon;
    public float ExpansionPerSecond;
    public ModelConditionBitFlags UnpackModelConditions;
    public ObjectStatusBitFlags UnpackObjectStatus;
    public ModelConditionBitFlags ModelConditions;
    public ObjectStatusBitFlags ObjectStatus;
}

public enum ShieldSphereUpdateOption
{
    ALLOW_ALLIES_PROJECTTILE_GOTHROUGH
}

[StructLayout(LayoutKind.Sequential)]
public struct ShieldSphereUpdateOptionFlag
{
    public const int Count = 1;
    public const int BitsInSpan = 32;
    public const int NumSpans = 1;

    public unsafe fixed uint Value[NumSpans];
}

[StructLayout(LayoutKind.Sequential)]
public struct ShieldSphereUpdateModuleData
{
    public SphereModuleUpdateModuleData Base;
    public float MaxDamage;
    public ObjectStatusBitFlags ObjectStatus;
    public ModelConditionBitFlags ModelCondition;
    public AssetReference<AttributeModifier> AttributeModifierName;
    public ObjectStatusBitFlags ShieldedObjectStatus;
    public ShieldSphereUpdateOptionFlag Options;
}

[StructLayout(LayoutKind.Sequential)]
public struct YurikoShieldSphereUpdateModuleData
{
    public ShieldSphereUpdateModuleData Base;
    public AssetReference<FXList> MajorShieldHitFX;
    public AssetReference<FXList> MinorShieldHitFX;
    public DamageBitFlags MinorShieldDamageTypes;
}
