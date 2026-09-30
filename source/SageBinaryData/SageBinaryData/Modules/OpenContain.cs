using Relo;
using System.Runtime.InteropServices;
using AnsiString = Relo.String<sbyte>;

namespace SageBinaryData;

[StructLayout(LayoutKind.Sequential)]
public struct PassengerDataType
{
    public AnsiString BonePrefix;
    // Reborn: RA3 passenger records use capacity and a trailing sling flag, not KW's string flags.
    public int MaxPassengers;
    public ObjectFilter Filter;
    public SageBool SlingUnderBone;
}

[StructLayout(LayoutKind.Sequential)]
public struct MemberTemplateStatusData
{
    public TypedAssetId<GameObject> ThingTemplate;
    public ObjectStatusType ObjectStatus;
}

[StructLayout(LayoutKind.Sequential)]
public struct OpenContainUpgradeOverrideData
{
    public TypedAssetId<UpgradeTemplate> UpgradeTriggeredBy;
    public ObjectStatusBitFlags ObjectStatusOfContained;
}

[StructLayout(LayoutKind.Sequential)]
public struct OpenContainModuleData
{
    public UpdateModuleData Base;
    public uint ContainMax;
    public AssetReference<BaseAudioEventInfo, AudioEventInfo> EnterSound;
    public AssetReference<BaseAudioEventInfo, AudioEventInfo> ExitSound;
    public Percentage DamagePercentToUnits;
    public float PassengersTestCollisionHeight;
    public int NumberOfExitPaths;
    public uint DoorOpenTime;
    // Reborn: retain both inline status masks; EP1 widens each from 28 to 32 bytes.
    public DisabledBitFlags IgnoreDisabledBitsForRiders;
    public ObjectStatusBitFlags ObjectStatusOfContained;
    public ObjectStatusBitFlags ObjectStatusWhileContaining;
    public uint ModifierRequiredTime;
    public Time KillIfEmptyTime;
    public unsafe ObjectFilter* PassengerFilter;
    public unsafe DieMuxDataType* DieMuxData;
    public List<PassengerDataType> PassengerData;
    public List<AssetReference<AttributeModifier>> ModifierToGiveOnExit;
    public List<MemberTemplateStatusData> MemberTemplateStatusInfo;
    public SageBool PassengersInTurret;
    public SageBool AllowOwnPlayerInsideOverride;
    public SageBool AllowAlliesInside;
    public SageBool AllowEnemiesInside;
    public SageBool AllowNeutralInside;
    // Reborn: native RA3/EP1 rider disability flag precedes the presentation flags.
    public SageBool PassDisabilityToRiders;
    public SageBool ShowPips;
    public SageBool CollidePickup;
    public SageBool EjectPassengersOnDeath;
    public SageBool KillPassengersOnDeath;
    public SageBool HasObjectStatusOfContainedEntry;
    public SageBool Enabled;
}
