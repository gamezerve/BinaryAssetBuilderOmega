using Relo;
using System.Runtime.InteropServices;

namespace SageBinaryData;

// Reborn: preserve official RA3/EP1 infiltrator effect values, including INVALID=-1.
public enum InfiltratorEffectType
{
    INVALID = -1,
    RADAR_FREEZE,
    DISABLE,
    STEAL_MONEY,
    VISION,
    ENERGY,
    RESET,
    VISION_UNITS,
    PARALYZE,
    KILL
}

// Reborn: restore inline status masks, asset references and optional filter-reference pointers from RA3 IL.
[StructLayout(LayoutKind.Sequential)]
public struct InfiltratorContainModuleData
{
    public ContainModuleData Base;
    public Time BlockedDuration;
    public ObjectStatusBitFlags InfiltratedStatusFilter;
    public AssetReference<ObjectFilterAsset> CanEnterFilter;
    public AssetReference<ObjectCreationList> ReplaceWith;
    public StringHash NameOfVoiceToUseForHostileEnter;
    public StringHash NameOfVoiceToUseForInfiltrationDeath;
    public AssetReference<BaseAssetType> EvaEventForInfiltratingPlayer;
    public AssetReference<BaseAssetType> EvaEventForInfiltratedPlayer;
    public AssetReference<FXList> FXForInfiltrate;
    public Time Duration;
    public Time UnitDuration;
    public float Amount;
    public InfiltratorEffectType Effect;
    public TypedAssetId<GameObject> ObjectRef;
    public unsafe AssetReference<ObjectFilterAsset>* UnitFilter;
    public unsafe AssetReference<ObjectFilterAsset>* StructureFilter;
    public ObjectStatusBitFlags ObjectStatusToSet;
    public SageBool ImmediatelyEnabled;
}
