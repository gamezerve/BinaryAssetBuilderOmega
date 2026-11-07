using System.Runtime.InteropServices;
using Relo;

namespace SageBinaryData;

// Reborn: original EP1 global EVA settings retain their twenty-byte scan-range/timing record.
[StructLayout(LayoutKind.Sequential)]
public struct Ep1EvaSettings
{
    public BaseAssetType Base;
    public int EnemySightedMaxVoicePositionScanRange;
    public Time MaxTimeToKeepJumpToEvents;
    public Time MaxTimeBeforeResettingLastJumpTo;
    public int MinDistanceBetweenJumpToEvents;
}

// Reborn: a faction voice selection is an audio occurrence followed by the weak player-template identity.
[StructLayout(LayoutKind.Sequential)]
public struct Ep1EvaSideSound
{
    public AssetReference<BaseAudioEventInfo,AudioEventInfo> Sound;
    // Reborn: XSD supplies the PlayerTemplate weak-ref type; the CLR carrier stores only its four-byte ID.
    public TypedAssetId<BaseAssetType> Side;
}

// Reborn: original EVA events place their two lists after the timing fields and collect four flags at the tail.
[StructLayout(LayoutKind.Sequential)]
public struct Ep1EvaEvent
{
    public BaseInheritableAsset Base;
    public uint Priority;
    public Time TimeBetweenEvents,ExpirationTime,QuietTime,TimeToWaitBeforePlaying;
    public List<Ep1EvaSideSound> SideSound;
    public List<TypedAssetId<BaseAssetType>> OtherEvaEventToBlock;
    public SageBool AlwaysPlayFromHomeBase,CountAsJumpToLocation,SuppressOn360,SuppressOnPC;
}
