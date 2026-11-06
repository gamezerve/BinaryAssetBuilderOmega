using System.Runtime.InteropServices;
using Relo;

namespace SageBinaryData;

// Reborn: EP1 global PlayerTemplate root. The retail Allies record places the first string at 0xBC.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1PlayerTemplate
{
    public BaseAssetType Base;
    public Ep1PlayerTemplateKind Type;
    public Relo.String<sbyte> DisplayNameToken;
    public Relo.String<sbyte> FactionIconImageName;
    public AssetReference<BaseAssetType> LoadScreenMusic, SkirmishMPVictoryMusic, SkirmishMPDefeatMusic;
    public AssetReference<BaseAssetType> FactionDataPathMusicEvent, MatchVictoryPathMusicEvent, MatchDefeatPathMusicEvent;
    public FactionType Side;
    public TypedAssetId<BaseAssetType> StartingBuilding, SpellBookSP, SpellBookMP, UnattachedMpBeaconTemplate, AttachedMpBeaconTemplate;
    public uint MaxMpBeacons;
    public List<uint> ResourceModifierValueList;
    public uint NumberOfResourceAccounts;
    public AssetReference<BaseAssetType> ScriptedHuntTactic, ParachuteOCL, DefaultTech, PlayerTechUpgradeBinding;
    public uint TechRankPointsPerSecond, InitialTechPoints;
    public TypedAssetId<BaseAssetType>* RedAlertTemplate;
    public Color PreferredColor, PreferredCampaignRadarColor;
    public List<AssetReference<BaseAssetType>> InitialUpgrade;
    public List<Ep1StartingUnitInfo> StartingUnitInfo;
    public ObjectFilter* ResourceModifierObjectFilter;
    public List<MoneyTransaction> StartingMoney, MoneyCap;
    public List<Ep1PlayerBeacon> AIBeaconSettings;
    public List<AssetReference<BaseAssetType>> TechRank;
    public List<Ep1TechProductionBonus> TechProductionBonuses;
    public SageBool PowerOutageTurnsOffRadar;
}

// Reborn: retail enum values follow the Uprising schema declaration order.
public enum Ep1PlayerTemplateKind { NONPLAYABLE, PLAYABLE, OBSERVER, RANDOM, COMMENTATOR, PLAYABLE_SP_ONLY, NULL }
public enum Ep1PlayerBeaconKind { Invalid = -1, Assault, HoldPosition, HoldObject }

[StructLayout(LayoutKind.Sequential)]
public struct Ep1StartingUnitInfo
{
    public AssetReference<BaseAssetType> Object;
    public Coord3D Position;
}

[StructLayout(LayoutKind.Sequential)]
public struct Ep1PlayerBeacon
{
    public Ep1PlayerBeaconKind BeaconType;
    public TypedAssetId<BaseAssetType> UnattachedTemplate, AttachedTemplate;
}

[StructLayout(LayoutKind.Sequential)]
public struct Ep1TechProductionBonus
{
    public AssetReference<BaseAssetType> RequiredTech;
    public Percentage BuildCostBonus, BuildCostBonusAllies, BuildTimeBonus, BuildTimeBonusAllies, HarvesterBonus, HarvesterBonusAllies;
}
