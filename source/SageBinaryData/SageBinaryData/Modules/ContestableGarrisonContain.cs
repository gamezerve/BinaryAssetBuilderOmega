using System.Runtime.InteropServices;

namespace SageBinaryData;

// Reborn: recovered RA3 contestable garrison fields use inline status masks widened by EP1.
[StructLayout(LayoutKind.Sequential)]
public struct ContestableGarrisonContainModuleData
{
    public GarrisonContainModuleData Base;
    public ObjectStatusBitFlags RequiredClearingObjectStatus;
    public ObjectStatusBitFlags ForbiddenContainerObjectStatus;
    public float EjectSpeed;
}
