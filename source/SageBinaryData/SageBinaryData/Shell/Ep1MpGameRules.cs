using System.Runtime.InteropServices;

namespace SageBinaryData;

// Reborn: original EP1 adds the money-transfer timer before its inline cash settings; preserve the old 20-byte legacy type.
[StructLayout(LayoutKind.Sequential)]
public struct Ep1MpGameRules
{
    public BaseAssetType Base;
    public int NumMinutesBeforePlayersCanTransferMoney;
    public SkirmishStartCash SkirmishStartCash;
}
