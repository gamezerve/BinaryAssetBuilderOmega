using System.Runtime.InteropServices;

namespace SageBinaryData;

// Reborn: retail magnetic satellites store five update scalars followed by an inline filter.
[StructLayout(LayoutKind.Sequential)]
public struct SuckUpObjectsUpdateModuleData
{
    public UpdateModuleData Base;
    public Time InitialDelay;
    public float LiftVelocity;
    public float DeathHeight;
    public float Radius;
    public Time Duration;
    public ObjectFilter ObjectFilter;
}
