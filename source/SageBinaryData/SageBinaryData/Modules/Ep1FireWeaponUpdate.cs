using Relo;
using System.Runtime.InteropServices;

namespace SageBinaryData;

// Reborn: EP1 firing nuggets contain no per-entry interval; their optional offset precedes the OneShot byte.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1FireWeaponUpdateModuleDataFireWeaponNugget
{
    public AssetReference<WeaponTemplate> WeaponName;
    public Time FireDelay;
    public Coord3D* Offset;
    public SageBool OneShot;
}

// Reborn: the 32-byte EP1 firing update stores the shared interval and optional status validation at the root.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1FireWeaponUpdateModuleData
{
    public UpdateModuleData Base;
    public Time FireInterval;
    public DisabledBitFlags ActiveWhenDisabled;
    public List<Ep1FireWeaponUpdateModuleDataFireWeaponNugget> FireWeaponNugget;
    public ObjectStatusValidationDataType* ObjectStatusValidation;
    public SageBool AliveOnly;
}
