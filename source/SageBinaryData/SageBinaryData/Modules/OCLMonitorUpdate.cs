using System.Runtime.InteropServices;

namespace SageBinaryData;

// Reborn: EP1 adds RELEASE_WHEN_MASTER_CHANGES_TARGET (bit 2; retail Natasha bombing-run monitor stores 5).
public enum OCLMonitorOption
{
    MASTER,
    KILL_WHEN_RELEASED,
    RELEASE_WHEN_MASTER_CHANGES_TARGET
}

[StructLayout(LayoutKind.Sequential)]
public struct OCLMonitorOptionFlag
{
    public const int Count = 3;
    public const int BitsInSpan = 32;
    public const int NumSpans = (Count + (BitsInSpan - 1)) / BitsInSpan;

    public unsafe fixed uint Value[NumSpans];
}

[StructLayout(LayoutKind.Sequential)]
public struct OCLMonitorUpdateModuleData
{
    public UpdateModuleData Base;
    public OCLMonitorOptionFlag Options;
    public ObjectStatusBitFlags RequiredMasterObjectStatus;
}
