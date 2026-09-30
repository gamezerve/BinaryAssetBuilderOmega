using System.Runtime.InteropServices;

namespace SageBinaryData;

// Reborn: official RA3 allocation and EP1 schema define this as an unchanged attach-base wrapper.
[StructLayout(LayoutKind.Sequential)]
public struct LeechTargetingAttachUpdateModuleData
{
    public AttachUpdateModuleData Base;
}
