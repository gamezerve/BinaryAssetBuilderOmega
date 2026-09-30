using System.Runtime.InteropServices;
using Relo;

namespace SageBinaryData;

// Reborn: EP1 sweep-option bit positions follow schema enumeration order.
public enum SweepingLaserOptionsType
{
    SWEEP_HORIZONTAL,
    SWEEP_VERTICAL,
    MOVE_BACK_AND_FORTH,
    SWEEP_FROM_SOURCE_ON_ANGLE
}

// Reborn: four sweep options occupy one native word.
[StructLayout(LayoutKind.Sequential)]
public struct SweepingLaserOptionsBitFlags
{
    public const int Count = 4;
    public const int BitsInSpan = 32;
    public const int NumSpans = (Count + (BitsInSpan - 1)) / BitsInSpan;
    public unsafe fixed uint Value[NumSpans];
}

// Reborn: EP1 adds Angle and sweep flags to the recovered RA3 laser-derived tail.
[StructLayout(LayoutKind.Sequential)]
public struct SweepingLaserStateModuleData
{
    public LaserStateModuleData Base;
    public float Radius;
    public Angle Angle;
    public AssetReference<FXList> SweepFXList;
    public AssetReference<FXList> VeteranSweepFXList;
    public Time SweepFXTimeout;
    public AssetReference<WeaponTemplate> SweepWeapon;
    public SweepingLaserOptionsBitFlags SweepingLaserOptions;
}
