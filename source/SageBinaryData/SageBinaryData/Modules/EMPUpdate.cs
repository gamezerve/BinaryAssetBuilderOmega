using System.Runtime.InteropServices;
using Relo;

namespace SageBinaryData;

[StructLayout(LayoutKind.Sequential)]
public struct EMPUpdateModuleData
{
    public UpdateModuleData Base;
    public Time Lifetime;
    public Time StartFadeDelay;
    public Time DisabledDuration;
    public float StartScale;
    public float TargetScaleMin;
    public float TargetScaleMax;
    public AssetReference<FXParticleSystemTemplate> DisableFXParticleSystem;
    public float SparksPerCubicFoot;
    public unsafe Color3f* StartColor;
    public unsafe Color3f* EndColor;
}

// Reborn: the EP1 reader takes two separate color pointers directly after the six scalar fields.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1EMPUpdateModuleData
{
    public const uint Wire = 0xD3302D7Eu;
    public UpdateModuleData Base;
    public Time Lifetime, StartFadeDelay, DisabledDuration;
    public float StartScale, TargetScaleMin, TargetScaleMax;
    public Color3f* StartColor;
    public Color3f* EndColor;
}
