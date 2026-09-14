using System.Runtime.InteropServices;

namespace SageBinaryData;

[StructLayout(LayoutKind.Sequential)]
public struct StoreObjectsSpecialPowerModuleData
{
    public SpecialAbilityUpdateModuleData Base;
    public float Radius;
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
