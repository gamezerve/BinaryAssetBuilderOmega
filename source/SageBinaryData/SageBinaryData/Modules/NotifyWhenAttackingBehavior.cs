using System.Runtime.InteropServices;
using Relo;

namespace SageBinaryData;

// Reborn: 40-byte EP1 attack notification record, anchored by the original Tesla Coil metadata/native field map.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct NotifyWhenAttackingBehaviorModuleData
{
    public UpdateModuleData Base;
    public float NotifyRadius;
    public TypedAssetId<ModuleData> AssistResponseModuleId;
    public Time SleepTime;
    public int MaxFriendsToNotify;
    public uint WeaponSlotID;
    public WeaponSlotType WeaponSlotType;
    public ObjectFilter* AcceptableFriends;
    public SageBool HasMobileFriends;
    public SageBool IgnoreDisabledStatus;
}
