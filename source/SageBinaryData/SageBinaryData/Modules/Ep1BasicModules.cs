using System.Runtime.InteropServices;
using Relo;

namespace SageBinaryData;

// Reborn: EP1 classes proven against selected stock payloads; legacy similarly named KW classes remain unchanged.
[StructLayout(LayoutKind.Sequential)]
public struct ObjectSMCHelperModuleData
{
    public ObjectHelperModuleData Base;
}

// Reborn: stock structure-sell payload follows the eight-byte update base with time and a model-condition enum.
[StructLayout(LayoutKind.Sequential)]
public struct StructureSellUpdateModuleData
{
    public UpdateModuleData Base;
    public Time SellTime;
    public ModelConditionFlagType ActiveModelCondition;
}

// Reborn: EP1 client audio counters are raw frame integers, not legacy Duration millisecond conversions.
[StructLayout(LayoutKind.Sequential)]
public struct LargeGroupAudioClientUpdateModuleData
{
    public ClientUpdateModuleData Base;
    public uint FramesBetweenUpdatesMin;
    public uint FramesBetweenUpdatesVariation;
    public List<StringHash> Key;
    public ushort UnitWeight;
}
