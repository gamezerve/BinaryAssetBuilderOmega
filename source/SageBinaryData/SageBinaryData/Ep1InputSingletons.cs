using System.Runtime.InteropServices;
using Relo;
namespace SageBinaryData;

// Reborn: original GeneralHotKeySlots keeps inline strong references in its witnessed 12-byte root.
[StructLayout(LayoutKind.Sequential)]
public struct Ep1GeneralHotKeySlots
{
    public BaseAssetType Base;
    public AssetReference<Ep1HotKeySlot> TakeScreenShot;
    public AssetReference<Ep1HotKeySlot> ReloadRapidIterationFeature;
}

// Reborn: original BasicMouseCursors keeps inline strong references in its witnessed 20-byte root.
[StructLayout(LayoutKind.Sequential)]
public struct Ep1BasicMouseCursors
{
    public BaseAssetType Base;
    public AssetReference<Ep1MouseCursor> Normal;
    public AssetReference<Ep1MouseCursor> Arrow;
    public AssetReference<Ep1MouseCursor> Invalid;
    public AssetReference<Ep1MouseCursor> EnterText;
}
