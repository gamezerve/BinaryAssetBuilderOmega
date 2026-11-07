using System.Runtime.InteropServices;
using Relo;

namespace SageBinaryData;

// Reborn: EP1 stores the default render info inline followed by a contiguous list of destination overrides.
[StructLayout(LayoutKind.Sequential)]
public struct UIDecalDestinationMarker
{
    public BaseInheritableAsset Base;
    public UIDecalDestinationMarkerRenderInfo DefaultRenderInfo;
    public List<UIDecalDestinationMarkerDestinationTypeSettings> DestinationTypeSettings;
}

// Reborn: the marker's render model is an import, alongside an inline depth bias and RGB color.
[StructLayout(LayoutKind.Sequential)]
public struct UIDecalDestinationMarkerRenderInfo
{
    public AssetReference<BaseRenderAssetType> Model;
    public float ZBias;
    public RGBColor Color;
}

// Reborn: match the EP1 DestinationType enumeration order.
public enum DestinationType : uint
{
    Normal,
    Aggressive
}

// Reborn: each destination override owns its complete render record.
[StructLayout(LayoutKind.Sequential)]
public struct UIDecalDestinationMarkerDestinationTypeSettings
{
    public DestinationType DestinationType;
    public UIDecalDestinationMarkerRenderInfo RenderInfo;
}
