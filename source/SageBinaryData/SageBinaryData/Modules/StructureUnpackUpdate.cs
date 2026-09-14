using System.Runtime.InteropServices;
using Relo;

namespace SageBinaryData;

[StructLayout(LayoutKind.Sequential)]
public struct StructureUnpackUpdateModuleData
{
    public UpdateModuleData Base;
    public Time UnpackTime;
    public unsafe AssetReference<BaseAudioEventInfo, AudioEventInfo>* UnpackCompleteSound;
}

[StructLayout(LayoutKind.Sequential)]
public struct GenericUnpackUpdateModuleData
{
    public UpdateModuleData Base;
    public Time UnpackTime;
    public unsafe AssetReference<BaseAudioEventInfo, AudioEventInfo>* UnpackCompleteSound;
    public float OffsetHeightAboveWater;
}

[StructLayout(LayoutKind.Sequential)]
public struct UnitUnpackUpdateModuleData
{
    public GenericUnpackUpdateModuleData Base;
}
