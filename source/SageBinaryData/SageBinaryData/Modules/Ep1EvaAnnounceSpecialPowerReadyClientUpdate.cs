using Relo;
using System.Runtime.InteropServices;

namespace SageBinaryData;

// Reborn: retail EP1 stores three strong EVA references, a strong power reference and two byte flags in a 28-byte client record.
[StructLayout(LayoutKind.Sequential)]
public struct Ep1EvaAnnounceSpecialPowerReadyClientUpdateModuleData
{
    public ClientUpdateModuleData Base;
    public AssetReference<BaseAssetType> AnnouncementEventEnemy;
    public AssetReference<BaseAssetType> AnnouncementEventAlly;
    public AssetReference<BaseAssetType> AnnouncementEventOwner;
    public AssetReference<SpecialPowerTemplate> SpecialPowerTemplate;
    public SageBool JustMonitorPercentReady;
    public SageBool CheckPlayerPowerVisibility;
}
