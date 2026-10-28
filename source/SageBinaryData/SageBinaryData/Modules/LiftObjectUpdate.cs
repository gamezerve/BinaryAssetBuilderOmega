using System.Runtime.InteropServices;
using Relo;

namespace SageBinaryData;

[StructLayout(LayoutKind.Sequential)]
public struct LiftedUnitModelStateMap
{
    public ModelConditionBitFlags ModelState;
    public unsafe ObjectFilter* ObjectFilter;
}

[StructLayout(LayoutKind.Sequential)]
public struct LiftedObjectModelStateMapList
{
    public List<LiftedUnitModelStateMap> LiftedUnitModelState;
}

[StructLayout(LayoutKind.Sequential)]
public struct LiftObjectUpdateModuleData
{
    public UpdateModuleData Base;
    public int LiftObjectLinkID;
    public float LiftVelocity;
    public float MaxElevationFromGround;
    public Time TimeIncrement;
    public Time MaxTimeLifted;
    public float MaxStructureShakeVelocity;
    public AssetReference<WeaponTemplate> AirplaneCrashWeapon;
    public AssetReference<WeaponTemplate> SoftLandingWeapon;
    public float RotationSpeed;
    public AssetReference<ShaderOverride> Shader;
    public float ShakeIntensity;
    public float ShakeRadius;
    public float ShakeFade;
    public DisabledBitFlags DisabledTypesToProcess;
    public LiftedObjectModelStateMapList ModelStateObjectFilters;
    public SageBool CrusherModifiesVelocity;
}

// Reborn: original EP1 lift header keeps the optional Shader allocation distinct from an inline import.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1LiftObjectUpdateModuleData
{
    public UpdateModuleData Base;
    public int LiftObjectLinkID;
    public float LiftVelocity;
    public float MaxElevationFromGround;
    public Time TimeIncrement;
    public Time MaxTimeLifted;
    public float MaxStructureShakeVelocity;
    public AssetReference<WeaponTemplate> AirplaneCrashWeapon;
    public AssetReference<WeaponTemplate> SoftLandingWeapon;
    public float RotationSpeed;
    // Reborn: retail null metadata and the shader reference schema require an optional import pointee.
    public AssetReference<ShaderOverride>* Shader;
    public float ShakeIntensity;
    public float ShakeRadius;
    public float ShakeFade;
    public DisabledBitFlags DisabledTypesToProcess;
    public LiftedObjectModelStateMapList ModelStateObjectFilters;
    public SageBool CrusherModifiesVelocity;
}
