using Relo;
using System.Runtime.InteropServices;
using AnsiString = Relo.String<sbyte>;

namespace SageBinaryData;

// Reborn: EP1 distance-check names follow the official schema order.
public enum Ep1DistanceCheckType { CIRCULAR, RECTANGULAR }

// Reborn: stock destruction records store bone string, FX import, weak object identity and literal velocity inline.
[StructLayout(LayoutKind.Sequential)]
public struct BuildingDestructionItem
{
    public AnsiString BonePrefix;
    public AssetReference<FXList> FX;
    public TypedAssetId<GameObject> CreationObject;
    public float FlingVelocity;
}

// Reborn: selected EP1 destruction fields extend the existing DieModuleData layout; native evidence does not admit a general GameObject stream.
[StructLayout(LayoutKind.Sequential)]
public struct BuildingDestructionModuleData
{
    public DieModuleData Base;
    public TypedAssetId<ModuleData> AssociatedModule;
    public float MaxDebrisCloudsPerSecond;
    // Reborn: schema Poid/SubObject uses invariant lowercase weak identity, not case-sensitive StringHash.
    public TypedAssetId<SubObject> FillerMesh;
    public List<BuildingDestructionItem> Item;
}

// Reborn: EP1 tech upgrades embed ObjectFilter by value and preserve optional RadiusY pointer storage.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct TechUpgradeModuleData
{
    public UpgradeModuleData Base;
    public float Radius;
    public float* RadiusY;
    public uint DistanceType;
    public AnsiString FXLineBoneName;
    public ObjectFilter ObjectFilter;
}

// Reborn: event-list naming is serialized as a native string; this does not resolve or execute LUA callbacks.
[StructLayout(LayoutKind.Sequential)]
public struct LUAEventListBehaviorModuleData
{
    public BehaviorModuleData Base;
    public AnsiString EventListName;
}
