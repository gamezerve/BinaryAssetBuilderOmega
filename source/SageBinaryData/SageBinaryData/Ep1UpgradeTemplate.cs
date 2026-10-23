using Relo;
using System.Runtime.InteropServices;
using AnsiString = Relo.String<sbyte>;

namespace SageBinaryData;

// Reborn: the official EP1 option adds OBJECT_UPGRADE_PROJECTED after the three legacy KW values.
public enum Ep1UpgradeOptions{DO_NOT_TRANSFER,PERSISTS_IN_CAMPAIGN,NO_UPGRADE_DISCOUNT,OBJECT_UPGRADE_PROJECTED}

// Reborn: EP1 upgrade EVA events are asset references, not the legacy KW string records; keep the old model unchanged.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1UpgradeTemplate
{
    public BaseInheritableAsset Base;
    public AnsiString DisplayName,Description,TypeDescription,AcquireHint;
    public UpgradeType Type;
    public Time BuildTime;
    public int BuildCost;
    public AssetReference<BaseAudioEventInfo,AudioEventInfo> ResearchSound;
    public AssetReference<BaseAssetType> ResearchCompleteEvaEvent;
    public AssetReference<BaseAudioEventInfo,AudioEventInfo> UnitSpecificSound;
    public AssetReference<FXList> UpgradeFX;
    public AssetReference<BaseAssetType> LocalPlayerGainsUpgradeEvaEvent,AlliedPlayerGainsUpgradeEvaEvent,EnemyPlayerGainsUpgradeEvaEvent;
    public AssetReference<BaseAssetType> LocalPlayerLosesUpgradeEvaEvent,AlliedPlayerLosesUpgradeEvaEvent,EnemyPlayerLosesUpgradeEvaEvent;
    public AssetReference<BaseAssetType> LocalPlayerProductionStartedEvaEvent;
    public AssetReference<BaseAssetType> LocalPlayerBuildOnHoldEvaEvent;
    public AssetReference<BaseAssetType> LocalPlayerBuildCancelledEvaEvent;
    public TypedAssetId<BaseAssetType> UseObjectTemplateForCostDiscount;
    public AnsiString GroupName;
    public uint GroupOrder;
    public AIUpgradeHeuristicType SkirmishAIHeuristic;
    public Ep1UpgradeOptions Options;
    public AnsiString IconImage;
    public GameDependencyType* GameDependency;
    public SageBool WaypointQueueable;
}
