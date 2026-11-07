using System.Runtime.InteropServices;
using Relo;
using AnsiString=Relo.String<sbyte>;
namespace SageBinaryData;

// Reborn: the original 148-byte enclosure update retains its hit FX, draw-module POID, optional bone string and inline full filter.
[StructLayout(LayoutKind.Sequential)]
public struct EncloseBehaviorModuleData
{
    public UpdateModuleData Base;
    public AssetReference<FXList> EnclosureHitFX;
    public TypedAssetId<BaseAssetType> ScalableDrawModuleId;
    public AnsiString ScaledBoneName;
    public float EnclosureRadius;
    public ObjectFilter Filter;
}

// Reborn: EP1 line damage keeps all status bits and three independently optional filter/status/particle graphs after its timing and FX fields.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct LineSegmentDamageUpdateModuleData
{
    public UpdateModuleData Base;
    public Time NodeLifetime,DamageInterval;
    public float Distance;
    public AssetReference<WeaponTemplate> WeaponName;
    public ObjectStatusBitFlags ObjectStatusForbiddenForDamage;
    public float MinDistanceTolerance,MaxValidHeightOffGround;
    public AssetReference<FXList> NodeFXList;
    public ObjectFilter* ObjectFilter;
    public ObjectStatusValidationDataType* ObjectStatusValidation;
    public ParticleSystemList* ParticleSystems;
}
