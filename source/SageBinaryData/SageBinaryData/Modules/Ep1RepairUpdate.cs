using System.Runtime.InteropServices;

namespace SageBinaryData;

// Reborn: EP1 RepairUpdate keeps the shared 44-byte field order (stock SovietRefinery witness); it is isolated because EP1's
// HealAmountPerSecond default is 10, not the KW 90 compiled into the legacy marshaler.
[StructLayout(LayoutKind.Sequential)]
public struct Ep1RepairUpdateModuleData
{
    public UpdateModuleData Base;
    public float RepopThreshold;
    public uint HealAmountPerSecond;
    public RepairUpdateModuleDataPercentOfBuildCostToRebuildInfo PercentOfBuildCostToRebuildInfo;
    public RepairUpdateModuleDataSelfAudio SelfAudio;
    public SageBool Toggleable;
    public SageBool RepairableWhenDead;
}
