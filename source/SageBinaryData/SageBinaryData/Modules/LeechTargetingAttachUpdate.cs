using System.Runtime.InteropServices;

namespace SageBinaryData;

// Reborn: official RA3 allocation and EP1 schema define this as an unchanged attach-base wrapper.
[StructLayout(LayoutKind.Sequential)]
public struct LeechTargetingAttachUpdateModuleData
{
    public AttachUpdateModuleData Base;
}

// Reborn: the base-only EP1 leaf keeps the retail attachment bone, filter and modifier order.
[StructLayout(LayoutKind.Sequential)]
public struct Ep1LeechTargetingAttachUpdateModuleData
{
    public const uint WireTypeId = 0xCA6038A6u;
    public Ep1AttachUpdateModuleData Base;
}
