using System.Runtime.InteropServices;

namespace SageBinaryData;

[StructLayout(LayoutKind.Sequential)]
public struct InvalidTargetsDeathUpdateModuleData
{
    public UpdateModuleData Base;
    // Reborn: optional EP1 invalid-target mask follows the inherited update identity.
    public unsafe ObjectStatusBitFlags* InvalidStatus;
}
