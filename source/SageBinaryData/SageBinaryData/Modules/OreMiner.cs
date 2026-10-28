using Relo;
using System.Runtime.InteropServices;
namespace SageBinaryData
{
    // Reborn: ore delivery uses the retail twenty-byte Create layout and an optional status-validation pointer.
    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct OreMinerModuleData
    {
        public CreateModuleData Base;
        public uint DeliveryCapacity;
        public Time DeliveryTime;
        public ObjectStatusValidationDataType* ObjectStatusValidation;
    }
    // Reborn: incoming-fire tracking is an identity-only BehaviorModuleData in EP1.
    [StructLayout(LayoutKind.Sequential)]
    public struct TrackIncomingWeaponFireBehaviorModuleData { public BehaviorModuleData Base; }
    // Reborn: damage tracking adds a time and one-byte instant-weapon flag to the retail Damage base.
    [StructLayout(LayoutKind.Sequential)]
    public struct TrackDamageModuleData { public DamageModuleData Base; public Time TimeToTrack; public SageBool InstantWeaponsOnly; }
}
