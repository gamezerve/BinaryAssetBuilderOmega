using System.Runtime.InteropServices;
using Relo;
namespace SageBinaryData;

// Reborn: EP1 removed PickupScience and added the inline audio handle and KillOnPickup flag to the 116-byte crate base.
[StructLayout(LayoutKind.Sequential)]
public struct Ep1CrateCollideModuleData
{
    public CollideModuleData Base;
    public KindOfBitFlags KindOf,KindOfNot;
    public AssetReference<FXList> ExecuteFX;
    public TypedAssetId<GameObject> ExecutionAnimationTemplate;
    public float ExecuteAnimationDisplayTimeInSeconds,ExecuteAnimationZRisePerSecond;
    public AssetReference<BaseAudioEventInfo,AudioEventInfo> ExecuteAudioEvent;
    public SageBool IsForbidOwnerPlayer,IsBuildingPickup,IsHumanOnlyPickup,IsHumanOnlyPickupInSinglePlayerCampaign,ExecuteAnimationFades,KillOnPickup;
}

// Reborn: secret-file crates preserve the original modifier import and self-application flag after the complete crate base.
[StructLayout(LayoutKind.Sequential)]
public struct AttributeCrateCollideModuleData
{
    public Ep1CrateCollideModuleData Base;
    public AssetReference<AttributeModifier> AttributeModifier;
    public SageBool ApplyToSelf;
}

// Reborn: healing crates retain their optional filter pointer and 124-byte EP1 record, leaving the legacy KW model unchanged.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1HealCrateCollideModuleData
{
    public Ep1CrateCollideModuleData Base;
    public float Range;
    public ObjectFilter* ObjectFilter;
}
