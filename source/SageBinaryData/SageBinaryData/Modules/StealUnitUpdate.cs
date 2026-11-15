using System.Runtime.InteropServices;

namespace SageBinaryData;

[StructLayout(LayoutKind.Sequential)]
public struct StealUnitUpdateModuleData
{
    // Reborn: retain the retail EP1 bone, filter and modifier offsets.
    public Ep1AttachUpdateModuleData Base;
}
