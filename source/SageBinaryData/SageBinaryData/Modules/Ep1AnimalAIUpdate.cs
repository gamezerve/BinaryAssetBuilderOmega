using System.Runtime.InteropServices;
namespace SageBinaryData;

// Reborn: the Uprising WorldBuilder animal class extends the 76-byte EP1 AI header with forty bytes of flee/wander settings.
[StructLayout(LayoutKind.Sequential)]
public struct Ep1AnimalAIUpdateModuleData
{
    public Ep1AIUpdateModuleData Base;
    public int FleeRadius,FleeDistance,WanderPercentage,MinDistance,MaxDistance,Radius;
    public Time FleeUpdateTimer,WanderUpdateTimer;
    public float InitialFleeBlindlyRadius;
    public SageBool AfraidOfCastles;
}
