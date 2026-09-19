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
