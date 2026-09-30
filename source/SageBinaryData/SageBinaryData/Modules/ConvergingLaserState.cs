using System.Runtime.InteropServices;
using Relo;

namespace SageBinaryData;

// Reborn: recover the official RA3 converging tail after EP1's expanded laser base.
[StructLayout(LayoutKind.Sequential)]
public struct ConvergingLaserStateModuleData
{
    public LaserStateModuleData Base;
    public Angle ConvergingAngle;
    public Angle AngleRotation;
    public float Radius;
    public AssetReference<FXList> SweepFXList;
    public AssetReference<FXList> VeteranSweepFXList;
    public Time SweepFXTimeout;
    public AssetReference<WeaponTemplate> SweepWeapon;
    public ModelConditionBitFlags ModelConditions;
    public Time Lifetime;
}
