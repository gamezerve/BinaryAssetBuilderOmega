using System.Runtime.InteropServices;

namespace SageBinaryData;

// Reborn: EP1 uses schema-named floating seconds rather than the legacy KW ExitDelay integer field.
[StructLayout(LayoutKind.Sequential)]
public struct Ep1QueueProductionExitUpdateModuleData
{
    public UpdateModuleData Base;
    public Angle PlacementViewAngle;
    public Time ExitDelaySeconds;
    public uint InitialBurst;
    public Coord3D UnitCreatePoint;
    public Coord3D NaturalRallyPoint;
    public SageBool AllowAirborneCreation;
    public SageBool NoExitPath;
    public SageBool CanRallyToSlaughter;
    public SageBool ClearAlliesFromDestination;
}
