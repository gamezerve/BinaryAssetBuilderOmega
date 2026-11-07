using System.Runtime.InteropServices;
using Relo;
using AnsiString = Relo.String<sbyte>;

namespace SageBinaryData;

// Reborn: Uprising stores the schema's explicit hotkey group ordinal, followed by index, name and version.
public enum Ep1HotKeySlotGroup { Camera=0,SideBar=1,UnitCommands=2,Bookmarks=3,PlayerPowersBar=4,Misc=5,NonConfigurable=6,Cheat=7 }

[StructLayout(LayoutKind.Sequential)]
public struct Ep1HotKeySlot
{
    public BaseInheritableAsset Base;
    public Ep1HotKeySlotGroup Group;
    public int Index;
    public AnsiString Name;
    public int VersionAdded;
}

// Reborn: quick-chat messages contain one localized string and one concrete audio import in the original static stream.
[StructLayout(LayoutKind.Sequential)]
public struct Ep1UIQuickChatMessage
{
    public BaseAssetType Base;
    public AnsiString MessageText;
    public AssetReference<BaseAudioEventInfo,AudioEventInfo> Sound;
}
