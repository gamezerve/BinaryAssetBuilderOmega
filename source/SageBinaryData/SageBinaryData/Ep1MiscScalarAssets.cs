using System.Runtime.InteropServices;
using Relo;
using AnsiString=Relo.String<sbyte>;

namespace SageBinaryData;

// Reborn: original MouseCursor native header is 16 bytes; every authored field is independently source/native checked.
[StructLayout(LayoutKind.Sequential)]
public struct Ep1MouseCursor
{
    public BaseAssetType Base;
    public AnsiString File;
    public int NumDirections;
}

// Reborn: original FontTranslation native header is 20 bytes; every authored field is independently source/native checked.
[StructLayout(LayoutKind.Sequential)]
public struct Ep1FontTranslation
{
    public BaseAssetType Base;
    public AnsiString Name;
    public float TranslateX;
    public float TranslateY;
}

// Reborn: original FontDirectory native header is 12 bytes; every authored field is independently source/native checked.
[StructLayout(LayoutKind.Sequential)]
public struct Ep1FontDirectory
{
    public BaseAssetType Base;
    public AnsiString Path;
}

// Reborn: original Mouse native header is 20 bytes; every authored field is independently source/native checked.
[StructLayout(LayoutKind.Sequential)]
public struct Ep1Mouse
{
    public BaseAssetType Base;
    public int DragTolerance;
    public Time DragThreshold;
    public uint InGameUIMaxDoubleClickDistance;
    public uint AptHitRegionMaxDoubleClickDistance;
}

// Reborn: original ForceFeedbackEvent native header is 20 bytes; every authored field is independently source/native checked.
[StructLayout(LayoutKind.Sequential)]
public struct Ep1ForceFeedbackEvent
{
    public BaseInheritableAsset Base;
    public Percentage LeftMotorUse;
    public Percentage RightMotorUse;
    public Time Duration;
    public float PlayCount;
}

// Reborn: original AudioEventLimitGroup native header is 8 bytes; every authored field is independently source/native checked.
[StructLayout(LayoutKind.Sequential)]
public struct Ep1AudioEventLimitGroup
{
    public BaseInheritableAsset Base;
    public int Limit;
}

// Reborn: original AnimationSoundSettings native header is 8 bytes; every authored field is independently source/native checked.
[StructLayout(LayoutKind.Sequential)]
public struct Ep1AnimationSoundSettings
{
    public BaseAssetType Base;
    public int MinMicrophoneDistanceToDirty;
}

