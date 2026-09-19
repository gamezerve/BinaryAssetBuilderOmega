using System.Runtime.InteropServices;

using Relo;

namespace SageBinaryData;

[StructLayout(LayoutKind.Sequential)]
public struct StoreObjectsSpecialPowerModuleData
{
    public SpecialPowerModuleData Base;
    public float Radius;
    public int TeleportLinkID;
    public AssetReference<ObjectCreationList> OCL;
    public TypedAssetId<GameObject> TargetMarkerObjectRef;
}

[StructLayout(LayoutKind.Sequential)]
public struct AddObjectsToLiftUpdateSpecialPowerModuleData
{
    public StoreObjectsSpecialPowerModuleData Base;
    public int LiftObjectLinkID;
}

[StructLayout(LayoutKind.Sequential)]
public struct AddObjectsToLureUpdateSpecialPowerModuleData
{
    public StoreObjectsSpecialPowerModuleData Base;
    public int LureObjectLinkID;
}
