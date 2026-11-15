using System.Runtime.InteropServices;

namespace SageBinaryData;

// Reborn: the retail loader appends one optional vector pointer to the 524-byte EP1 OCL power.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1CreateAndEnterObjectSpecialPowerModuleData
{
    public const uint Wire = 0x38C496D8u;
    public Ep1OCLSpecialPowerModuleData Base;
    public Coord3D* FXOffset;
}

// Reborn: the original EP1 toggle stores two separate 32-byte status masks.
[StructLayout(LayoutKind.Sequential)]
public struct Ep1ObjectStatusSpecialAbilityUpdateModuleData
{
    public const uint Wire = 0x079B6EADu;
    public SpecialAbilityUpdateModuleData Base;
    public ObjectStatusBitFlags Status1, Status2;
}
