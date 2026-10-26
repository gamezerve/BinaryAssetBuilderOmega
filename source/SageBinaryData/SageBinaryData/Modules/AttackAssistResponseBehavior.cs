using System.Runtime.InteropServices;
namespace SageBinaryData;
// Reborn: retail EP1 attack assistance adds a 32-byte object-status mask and two slot words to the behavior header.
[StructLayout(LayoutKind.Sequential)]
public struct AttackAssistResponseBehaviorModuleData
{
    public BehaviorModuleData Base;
    public ObjectStatusBitFlags ObjectStatus;
    public uint WeaponSlotID;
    public WeaponSlotType WeaponSlotType;
}
