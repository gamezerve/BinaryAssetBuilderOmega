using System.Runtime.InteropServices;
using AnsiString=Relo.String<sbyte>;
namespace SageBinaryData;

// Reborn: EP1 multiplayer colors omit the two legacy LivingWorld colors and keep the original 28-byte header.
[StructLayout(LayoutKind.Sequential)]
public struct Ep1MultiplayerColor
{
    public BaseAssetType Base;
    public Color RGBColor,RGBColorT,RGBNightColor;
    public AnsiString TooltipName;
    public SageBool AvailableInMetaGame;
}

// Reborn: UI header templates retain the original inline FontDesc after their asset base.
[StructLayout(LayoutKind.Sequential)]
public struct Ep1HeaderTemplate
{
    public BaseAssetType Base;
    public FontDesc Font;
}
