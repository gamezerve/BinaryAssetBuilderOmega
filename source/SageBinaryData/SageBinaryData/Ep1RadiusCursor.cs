using System.Runtime.InteropServices;
using Relo;
namespace SageBinaryData;

// Reborn: every EP1 radius decal component keeps both texture imports, complete motion/pulse fields and its inline optional color.
[StructLayout(LayoutKind.Sequential)]
public struct Ep1RadiusDecalComponent
{
    public AssetReference<Texture> Texture,Texture2;
    public ShadowType Style;
    public Percentage OpacityMin,OpacityMax;
    public float OpacityPulsesPerSecond,RotationMin,RotationMax,RotationDegreesPerSecond,MinRadius,MaxRadius,RadiusPulsesPerSecond,PositionOffsetMin,PositionOffsetMax,PositionOffsetPulsesPerSecond;
    public Color Color;
}

// Reborn: the EP1 decal template is a source-sized list rather than the incompatible fixed KW cursor settings.
[StructLayout(LayoutKind.Sequential)]
public struct Ep1RadiusDecalTemplate
{
    public List<Ep1RadiusDecalComponent> Component;
}

// Reborn: a standalone EP1 radius cursor has a zero asset base followed by its ordered decal template.
[StructLayout(LayoutKind.Sequential)]
public struct Ep1RadiusCursor
{
    public BaseAssetType Base;
    public Ep1RadiusDecalTemplate DecalTemplate;
}
