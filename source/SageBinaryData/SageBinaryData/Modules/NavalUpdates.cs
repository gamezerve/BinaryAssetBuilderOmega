using Relo;
using System.Runtime.InteropServices;

namespace SageBinaryData;

// Reborn: the original naval circular-damage flag uses bit zero for water-only processing.
public enum CircularDamageUpdateOptionsType { ONLY_ON_WATER }

// Reborn: the single official circular-damage option occupies one native word.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct CircularDamageUpdateOptionsTypeBitFlags { public const int Count=1; public fixed uint Value[1]; }

// Reborn: the 152-byte EP1 circular damage record embeds its required filter after the 32-byte scalar/reference prefix.
[StructLayout(LayoutKind.Sequential)]
public struct CircularDamageUpdateModuleData
{
    public UpdateModuleData Base;
    public CircularDamageUpdateOptionsTypeBitFlags Flags;
    public float MaxRadius,ArcDistance;
    public AssetReference<FXList> SweepFXList;
    public AssetReference<WeaponTemplate> SweepWeapon;
    public Time FXTimeout;
    public ObjectFilter ObjectFilter;
}

// Reborn: naval health drain uses a 24-byte update header and optional status-validation storage.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct LoseHealthUpdateModuleData
{
    public UpdateModuleData Base;
    public Percentage HealthTrigger;
    public float DamagePerSecond;
    public DeathType DeathType;
    public ObjectStatusValidationDataType* ObjectStatusValidation;
}
