using System.Runtime.InteropServices;
namespace SageBinaryData;

// Reborn: the original EP1 parabolic-flight update is an identity-only UpdateModuleData record.
[StructLayout(LayoutKind.Sequential)]
public struct ParabolicCurveUpdateModuleData
{
    public UpdateModuleData Base;
}
