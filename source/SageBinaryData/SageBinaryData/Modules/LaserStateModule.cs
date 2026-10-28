using System.Runtime.InteropServices;

namespace SageBinaryData;

// Reborn: restore the RA3 laser header and append EP1's RequiresWeapon flag.
[StructLayout(LayoutKind.Sequential)]
public struct LaserStateModuleData
{
    public UpdateModuleData Base;
    public int LaserId;
    public Relo.String<sbyte> OriginBoneName;
    public Relo.List<Relo.AssetReference<FXParticleSystemTemplate>> LaserEndParticleSystem;
    public Relo.List<Relo.AssetReference<FXParticleSystemTemplate>> LaserStartParticleSystem;
    // Reborn: schema Vector3 uses lowercase x/y/z attributes; Coord3D has the same 12-byte layout with matching field names.
    public unsafe Coord3D* EndOffset;
    public unsafe ObjectStatusValidationDataType* ObjectStatusValidation;
    public SageBool RequiresWeapon;
}
