using Relo;
using SageBinaryData;

public static partial class Marshaler
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: marshal the EP1 CampaignMovie record from normalized source. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node, Ep1CampaignMovie* root, Tracker state)
    {
        Marshal(node.GetAttributeValue("ImageRef", null), &root->ImageRef, state);
        Marshal(node.GetAttributeValue("FileName", null), &root->FileName, state);
        Marshal(node.GetAttributeValue("ScreenName", null), &root->ScreenName, state);
        Marshal(node.GetAttributeValue("Description", null), &root->Description, state);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: marshal the EP1 CampaignEnumerator record from normalized source. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node, Ep1CampaignEnumerator* root, Tracker state)
    {
        Marshal(node.GetChildNodes("Campaign"), &root->Campaign, state);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: marshal the EP1 CampaignTemplate record from normalized source. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node, Ep1CampaignTemplate* root, Tracker state)
    {
        Marshal(node.GetAttributeValue("PrerequisiteCampaign", "Neutral"), &root->PrerequisiteCampaign, state);
        Marshal(node.GetAttributeValue("PrerequisiteCampaignMissionNumber", null), &root->PrerequisiteCampaignMissionNumber, state);
        Marshal(node.GetAttributeValue("DisplayName", null), &root->DisplayName, state);
        Marshal(node.GetAttributeValue("ConsoleAutosaveFilename", null), &root->ConsoleAutosaveFilename, state);
        Marshal(node.GetChildNodes("Mission"), &root->Mission, state);
        Marshal(node.GetChildNode("FinalMovie", null), &root->FinalMovie, state);
        Marshal(node.GetChildNode("AlternateFinalMovie", null), &root->AlternateFinalMovie, state);
        Marshal(node.GetAttributeValue("ForceSinglePlayer", null), &root->ForceSinglePlayer, state);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: marshal the schema-defined legacy campaign commander record. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node, Ep1CampaignAICommander* root, Tracker state)
    {
        Marshal(node.GetAttributeValue("Portrait", null), &root->Portrait, state);
        Marshal(node.GetAttributeValue("Personality", null), &root->Personality, state);
        Marshal(node.GetAttributeValue("Description", null), &root->Description, state);
        Marshal(node.GetAttributeValue("Faction", null), &root->Faction, state);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: marshal the EP1 MissionTemplate record from normalized source. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node, Ep1MissionTemplate* root, Tracker state)
    {
        Marshal(node.GetAttributeValue("DisplayName", null), &root->DisplayName, state);
        Marshal(node.GetAttributeValue("ShortDisplayName", null), &root->ShortDisplayName, state);
        Marshal(node.GetAttributeValue("OverviewTalkingHead", null), &root->OverviewTalkingHead, state);
        Marshal(node.GetAttributeValue("OverviewBackground", null), &root->OverviewBackground, state);
        Marshal(node.GetAttributeValue("Title", null), &root->Title, state);
        Marshal(node.GetAttributeValue("MapName", null), &root->MapName, state);
        Marshal(node.GetAttributeValue("BriefingFLA", null), &root->BriefingFLA, state);
        Marshal(node.GetAttributeValue("BriefingSummary", null), &root->BriefingSummary, state);
        Marshal(node.GetAttributeValue("VictoryAchEvent", null), &root->VictoryAchEvent, state);
        Marshal(node.GetAttributeValue("VictoryAchEventMedium", null), &root->VictoryAchEventMedium, state);
        Marshal(node.GetAttributeValue("VictoryAchEventHard", null), &root->VictoryAchEventHard, state);
        Marshal(node.GetAttributeValue("VictoryAchEventCoop", null), &root->VictoryAchEventCoop, state);
        Marshal(node.GetAttributeValue("VictoryAchEventBonusObj", null), &root->VictoryAchEventBonusObj, state);
        Marshal(node.GetChildNodes("Prerequisite"), &root->Prerequisite, state);
        Marshal(node.GetChildNodes("Objective"), &root->Objective, state);
        Marshal(node.GetChildNodes("BonusObjective"), &root->BonusObjective, state);
        Marshal(node.GetChildNodes("AICommander"), &root->AICommander, state);
        Marshal(node.GetChildNode("IntroMovie", null), &root->IntroMovie, state);
    }
}
