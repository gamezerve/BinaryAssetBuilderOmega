using System.Runtime.InteropServices;
namespace SageBinaryData;

// Reborn: EP1 laser scatter owns two native floats following its inherited update identity.
[StructLayout(LayoutKind.Sequential)]
public struct LaserScatterUpdateModuleData
{
    public UpdateModuleData Base;
    public Time LaserLifetime;
    public Percentage LaserHitGroundWidthMultiplier;
}
