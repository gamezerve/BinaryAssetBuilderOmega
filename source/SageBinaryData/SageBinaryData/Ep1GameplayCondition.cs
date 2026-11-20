using System.Runtime.InteropServices;
using Relo;

namespace SageBinaryData;

// Reborn: the PC condition reader owns an inline primary condition and three optional allocations.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct GameplayConditionData
{
    public BaseAssetType Base;
    public uint GameplayConditionBitFlags;
    public Time* TimeReq;
    public uint AllowedGameTypeBitFlags;
    public uint GameDifficultyBitFlags;
    public ObjectConditionData PrimaryObject;
    public ObjectConditionData* RequiredObjectCondition;
    public UpgradeConditionData* RequiredUpgradeCondition;
}

// Reborn: achievement triggers link to a separately managed GameplayConditionData asset.
[StructLayout(LayoutKind.Sequential)]
public struct GameplayAchievementTriggerData
{
    public BaseAssetType Base;
    public AssetReference<GameplayConditionData> GameplayConditionData;
    public uint AchievementEventId;
}

// Reborn: both nested condition records share the count, optional filter and unique byte.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct BaseConditionData
{
    public int Count;
    public ObjectFilter* ObjectFilter;
    public SageBool Unique;
}

// Reborn: an object condition may allocate the original EP1 DieMux record.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct ObjectConditionData
{
    public BaseConditionData Base;
    public DieMuxDataType* DieMuxData;
}

// Reborn: required upgrades use inline weak IDs in a counted, relocated array.
[StructLayout(LayoutKind.Sequential)]
public struct UpgradeConditionData
{
    public BaseConditionData Base;
    public List<TypedAssetId<UpgradeTemplate>> UpgradeRequired;
}
