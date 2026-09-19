using System.Runtime.InteropServices;
using Relo;

namespace SageBinaryData;

[StructLayout(LayoutKind.Sequential)]
public struct FlingStoredObjectsObjectMap
{
    public TypedAssetId<GameObject> Source;
    public TypedAssetId<GameObject> Target;
}

[StructLayout(LayoutKind.Sequential)]
public struct FlingStoredObjectsSpecialPowerModuleData
{
    public SpecialPowerModuleData Base;
    public int StoreObjectsLinkID;
    public int LiftObjectLinkID;
    public float MaximumVelocity;
    public float MinimumVelocity;
    public List<FlingStoredObjectsObjectMap> ObjectMap;
}
