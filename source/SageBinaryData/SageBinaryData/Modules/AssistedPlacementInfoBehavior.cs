using System.Runtime.InteropServices;
using Relo;

namespace SageBinaryData;

// Reborn: one placement hint is 20 bytes: optional player-template weak reference, display object, opacity, optional filter, shader flag.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct HintObjectMapping
{
    public TypedAssetId<BaseAssetType>* PlayerTemplate;
    public TypedAssetId<GameObject> HintDisplayObject;
    public float HintOpacity;
    public ObjectFilter* ObjectFilter;
    public SageBool HintUsesShaders;
}

// Reborn: the optional placement restriction is a radius followed by an optional filter.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct PlacementRestriction
{
    public float FilterRadius;
    public ObjectFilter* RestrictedObjectFilter;
}

// Reborn: retail EP1 assisted-placement info is 36 bytes: hint list, optional restriction, inline suggested offset, AllowShrouded.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct AssistedPlacementInfoBehaviorModuleData
{
    public BehaviorModuleData Base;
    public List<HintObjectMapping> HintObject;
    public PlacementRestriction* PlacementRestriction;
    public Coord3D SuggestedOffset;
    public SageBool AllowShrouded;
}
