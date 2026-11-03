using System.Runtime.InteropServices;

namespace SageBinaryData;

// Reborn: original EP1 CameraSettings is a four-byte asset header followed by twenty schema-ordered floats.
[StructLayout(LayoutKind.Sequential)]
public struct CameraSettings
{
    public BaseAssetType Base;
    public float DefaultMinHeight;
    public float DefaultMaxHeight;
    public float DefaultPitchAngle;
    public float DefaultYawAngle;
    public float DefaultScrollSpeedScalar;
    public float DefaultHeightMapSmoothnessScalar;
    public float ScrollAmountCutoff;
    public float ScrollPitchMultiplier;
    public float CameraAdjustSpeed;
    public float ShakeSubtleIntensity;
    public float ShakeNormalIntensity;
    public float ShakeStrongIntensity;
    public float ShakeSevereIntensity;
    public float ShakeCineExtremeIntensity;
    public float ShakeCineInsaneIntensity;
    public float MaxShakeIntensity;
    public float MaxShakeRange;
    public float CameraLockHeightDelta;
    public float CameraTerrainSampleRadiusForHeight;
    public float CameraEaseFactor;
}
