using System.Runtime.InteropServices;
namespace SageBinaryData;

// Reborn: original EP1 state-machine ordinals and option bits follow the official schema.
public enum ProjectileStateMachineType { BombProjectile, StraightLineProjectile, BalloonProjectile, BezierSplineProjectile }
public enum ProjectileUpdateOptions { LAUNCH_AT_TARGET_POS, TRACK_TARGET }
[StructLayout(LayoutKind.Sequential)]
public struct ProjectileMoveBase { public uint TypeId; }
// Reborn: retail falling projectiles store a class word followed by six scalar movement fields.
[StructLayout(LayoutKind.Sequential)]
public struct ProjectileMoveToGroundData
{
    public ProjectileMoveBase Base;
    public float InitialSpeed;
    public float Acceleration;
    public float TerminalVelocity;
    public Angle RotationSpeedPerSecond;
    public Angle TerminalRotation;
    public Percentage ForwardVelocityDamping;
}
// Reborn: the original EP1 straight-line movement stores its concrete class followed by three real-valued velocity fields.
[StructLayout(LayoutKind.Sequential)]
public struct ProjectileMoveInStraightLineData
{
    public ProjectileMoveBase Base;
    public float MuzzleVelocity;
    public float Acceleration;
    public float TerminalVelocity;
}
// Reborn: EA's BalloonMoveToGround record consists only of the concrete class word.
[StructLayout(LayoutKind.Sequential)]
public struct BalloonProjectileMoveToGroundData
{
    public const uint WireTypeId = 0x1A5B5A69u;
    public ProjectileMoveBase Base;
}
// Reborn: the EP1 Bezier reader inserts the adjustment scalar before the four-byte path list.
[StructLayout(LayoutKind.Sequential)]
public struct BezierSplineProjectileData
{
    public const uint WireTypeId=0xB38F4984u;
    public ProjectileMoveBase Base;
    public float Velocity, Range, Gravity, FlightPathAdjustMaxDistancePerFrame;
    public Relo.List<Relo.AssetReference<ProjectilePath>> Path;
}
// Reborn: the required Movement points directly at its concrete record, with no native XML-choice wrapper.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct ProjectileUpdateModuleData
{
    public UpdateModuleData Base;
    public ProjectileStateMachineType StateMachine;
    public float MinDetonationHeight;
    public float MinTargetDistance;
    public uint Flags;
    public ProjectileMoveBase* Movement;
}
// Reborn: the original ship slow-death class adds no bytes to the complete 116-byte SlowDeath record.
[StructLayout(LayoutKind.Sequential)]
public struct ShipSlowDeathBehaviorModuleData { public SlowDeathBehaviorModuleData Base; }
