using Relo;
using System.Runtime.InteropServices;

namespace SageBinaryData;

// Reborn: Apocalypse grappling uses the retail 52-byte Update layout and two optional validation/filter pointers.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct GrapplingHookUpdateModuleData
{
    public UpdateModuleData Base;
    public float PullRate;
    public Time MinBounceDelay,MaxBounceDelay;
    public Percentage BouncePercentage,CollidingPullRateFactor;
    public AssetReference<WeaponTemplate> SingleFireWeapon,ContinuousFireWeapon,ContinuousFireWeaponBlocked;
    public Time ContinuousFireWeaponDelay;
    public ObjectStatusValidationDataType* ObjectStatusValidation;
    public ObjectFilter* KeepAliveObjectFilter;
}

// Reborn: MCV substructure storage is an identity-only retail Behavior module.
[StructLayout(LayoutKind.Sequential)]
public struct SubStructureStorageBehaviorModuleData { public BehaviorModuleData Base; }

// Reborn: EP1 TankDraw removes KW tread controls and extends ScriptedModelDraw with a terrain pointer and articulation list.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1W3DTankDrawModuleData
{
    public W3DScriptedModelDrawModuleData Base;
    public AssetReference<BaseAssetType>* TreadTerrainParticles;
    public List<ArticulationBone> ArticulationBone;
}
