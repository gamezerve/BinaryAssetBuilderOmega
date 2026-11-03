using Relo;
using System.Runtime.InteropServices;
using AnsiString = Relo.String<sbyte>;

namespace SageBinaryData;

// Reborn: EP1 campaign records use the retail global layout, separate from the KW campaign structs.
public enum Ep1CampaignFaction { RANDOM = -1, Allies, Soviet, Japan, Yuriko, Neutral }

[StructLayout(LayoutKind.Sequential)]
public struct Ep1CampaignMovie
{
    public AssetReference<PackedTextureImage> ImageRef;
    public AnsiString FileName;
    public AnsiString ScreenName;
    public AnsiString Description;
}

[StructLayout(LayoutKind.Sequential)]
public struct Ep1CampaignEnumerator
{
    public BaseAssetType Base;
    public List<AssetReference<Ep1CampaignTemplate>> Campaign;
}

[StructLayout(LayoutKind.Sequential)]
public struct Ep1CampaignTemplate
{
    public BaseAssetType Base;
    public Ep1CampaignFaction PrerequisiteCampaign;
    public uint PrerequisiteCampaignMissionNumber;
    public AnsiString DisplayName;
    public AnsiString ConsoleAutosaveFilename;
    public List<AssetReference<Ep1MissionTemplate>> Mission;
    public Ep1CampaignMovie FinalMovie;
    public Ep1CampaignMovie AlternateFinalMovie;
    public SageBool ForceSinglePlayer;
}

// Reborn: legacy campaign commander fields follow the EP1 XSD field order.
[StructLayout(LayoutKind.Sequential)]
public struct Ep1CampaignAICommander
{
    public AssetReference<OnDemandTextureImage> Portrait;
    public AnsiString Personality;
    public AnsiString Description;
    public Ep1CampaignFaction Faction;
}

[StructLayout(LayoutKind.Sequential)]
public struct Ep1MissionTemplate
{
    public BaseAssetType Base;
    public AnsiString DisplayName;
    public AnsiString ShortDisplayName;
    public AnsiString OverviewTalkingHead;
    public AnsiString OverviewBackground;
    public AnsiString Title;
    public AnsiString MapName;
    public AnsiString BriefingFLA;
    public AnsiString BriefingSummary;
    public uint VictoryAchEvent;
    public uint VictoryAchEventMedium;
    public uint VictoryAchEventHard;
    public uint VictoryAchEventCoop;
    public uint VictoryAchEventBonusObj;
    public List<int> Prerequisite;
    public List<AnsiString> Objective;
    public List<AnsiString> BonusObjective;
    public List<Ep1CampaignAICommander> AICommander;
    public Ep1CampaignMovie IntroMovie;
}
