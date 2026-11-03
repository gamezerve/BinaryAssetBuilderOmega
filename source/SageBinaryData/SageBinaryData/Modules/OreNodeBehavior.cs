using System.Runtime.InteropServices;
using Relo;

namespace SageBinaryData;

// Reborn: retail EP1 ore nodes are 72 bytes: gathering amounts, four phase times, four model conditions, then EVA and audio imports.
[StructLayout(LayoutKind.Sequential)]
public struct OreNodeModuleData
{
    public BehaviorModuleData Base;
    public uint MaximumGatheredValue;
    public uint DeliveryAmount;
    public uint DeliveryAmountWhenEmpty;
    public Percentage CashMultiplier;
    public Time MoveToExtractTime;
    public Time ExtractTime;
    public Time MoveToDeliverTime;
    public Time DeliveryTime;
    public ModelConditionFlagType ExtractAnimation;
    public ModelConditionFlagType DeliveryAnimation;
    public ModelConditionFlagType ReturnAnimation;
    public ModelConditionFlagType MoveBackAnimation;
    public Percentage LowNotificationPercentage;
    public AssetReference<BaseAssetType> LowNotificationEvaEventRef;
    public AssetReference<BaseAssetType> DepletedNotificationEvaEventRef;
    public AssetReference<BaseAudioEventInfo, AudioEventInfo> DepletedNotificationAudioEventInfoRef;
}
