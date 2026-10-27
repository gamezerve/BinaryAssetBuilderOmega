using Relo;
using System.Runtime.InteropServices;

namespace SageBinaryData;

// Reborn: EP1 create announcements store three strong EVA references, a frame delay and four byte flags in a 28-byte client record.
[StructLayout(LayoutKind.Sequential)]
public struct Ep1EvaAnnounceClientCreateModuleData
{
    public ClientUpdateModuleData Base;
    public AssetReference<BaseAssetType> AnnouncementEventEnemy;
    public AssetReference<BaseAssetType> AnnouncementEventAlly;
    public AssetReference<BaseAssetType> AnnouncementEventOwner;
    // Reborn: this field stores a raw unsigned logic-frame count; SageDurationUnsignedInt has no DurationTag conversion.
    public uint DelayBeforeAnnouncementLogicFrames;
    public SageBool OnlyIfVisible;
    public SageBool CountAsFirstSightedAnnouncement;
    public SageBool UseObjectsPosition;
    public SageBool CreateFakeRadarEvent;
}
