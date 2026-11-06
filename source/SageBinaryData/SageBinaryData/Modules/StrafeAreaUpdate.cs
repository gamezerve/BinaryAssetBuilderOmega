using Relo;
using System.Runtime.InteropServices;

namespace SageBinaryData;

// Reborn: Uprising stores four strafe options in a single native word.
public enum StrafeAreaUpdateFlagType
{
    FLEE_STRAIGHT_AHEAD,
    USE_TARGET_CHOOSER,
    DO_NOT_AVOID_TALLBUILDING,
    CANNOT_IDLE
}

[StructLayout(LayoutKind.Sequential)]
public unsafe struct StrafeAreaUpdateFlags
{
    public fixed uint Value[1];
}

[StructLayout(LayoutKind.Sequential)]
public struct StrafeAreaUpdateModuleData
{
    public UpdateModuleData Base;
    public AssetReference<WeaponTemplate> WeaponName;
    // Reborn: retail EP1 places the optional secondary weapon before the radius.
    public AssetReference<WeaponTemplate> SecondaryWeaponName;
    public float Radius;
    public float MinRadius;
    public float PreattackDistance;
    public float SweepFrequency;
    public float SweepAmplitude;
    public float DivingFloor;
    public float InitialSweepPhase;
    public float TargetJitterOffset;
    public uint ShotsToFirePerFrame;
    // Reborn: retain the authored strafe option bits.
    public StrafeAreaUpdateFlags Options;
}
