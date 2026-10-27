using Relo;
using System.Runtime.InteropServices;

namespace SageBinaryData;

// Reborn: Uprising inserts movement and collision flags into the legacy KW enumeration.
public enum Ep1FireWeaponFlagsType
{
    NONE, ONLY_WHILE_MOVING, TARGET_MUST_BE_MOVING, DESTROY_SELF_ON_COLLIDE,
    DO_NOT_COLLIDE_WITH_PRODUCER, WHEN_BEING_CRUSHED, ONLY_FRONT_COLLIDES,
    DISABLE_WHILE_FIRING, NOT_WHILE_ATTACKING, CHECK_WEAPON_AIM_DELTA,
    STANCE_ONLY_FORBIDDEN_WHILE_STATIONARY
}

// Reborn: the original EP1 112-byte collision record includes optional stance/filter pointers and trailing FireOnce.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1FireWeaponCollideModuleData
{
    public CollideModuleData Base;
    public AssetReference<WeaponTemplate> CollideWeapon;
    public FireWeaponFlagsBitFlags Flags;
    public ObjectStatusBitFlags RequiredStatus;
    public ObjectStatusBitFlags ForbiddenStatus;
    public WhereToFireType WhereToFire;
    public DisabledBitFlags DisabledTypesToProcess;
    public DisabledType DisableType;
    public Time DisableDuration;
    public Time FiringInterval;
    public Stance* ForbiddenStance;
    public ObjectFilter* TargetObjectFilter;
    public SageBool FireOnce;
}
