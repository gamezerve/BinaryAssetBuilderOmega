using System;
using Relo;
using SageBinaryData;

public static partial class Marshaler
{
    // Reborn: serialize the schema-resolved EP1 player root with ordinary native imports and owned lists.
    public static unsafe void Marshal(Node node, Ep1PlayerTemplate* target, Tracker state)
    {
        Marshal(node, (BaseAssetType*)target, state);
        uint kind = unchecked((uint)Enum.Parse<Ep1PlayerTemplateKind>(node.GetAttributeValue("Type", "NONPLAYABLE").GetText()));
        state.InplaceEndianToPlatform(&kind);
        target->Type = (Ep1PlayerTemplateKind)kind;
        Marshal(node.GetAttributeValue("DisplayNameToken", null), &target->DisplayNameToken, state);
        Marshal(node.GetAttributeValue("FactionIconImageName", null), &target->FactionIconImageName, state);
        Marshal(node.GetAttributeValue("LoadScreenMusic", null), &target->LoadScreenMusic, state);
        Marshal(node.GetAttributeValue("SkirmishMPVictoryMusic", null), &target->SkirmishMPVictoryMusic, state);
        Marshal(node.GetAttributeValue("SkirmishMPDefeatMusic", null), &target->SkirmishMPDefeatMusic, state);
        Marshal(node.GetAttributeValue("FactionDataPathMusicEvent", null), &target->FactionDataPathMusicEvent, state);
        Marshal(node.GetAttributeValue("MatchVictoryPathMusicEvent", null), &target->MatchVictoryPathMusicEvent, state);
        Marshal(node.GetAttributeValue("MatchDefeatPathMusicEvent", null), &target->MatchDefeatPathMusicEvent, state);
        Marshal(node.GetAttributeValue("Side", "Neutral"), &target->Side, state);
        Marshal(node.GetAttributeValue("StartingBuilding", null), &target->StartingBuilding, state);
        Marshal(node.GetAttributeValue("SpellBookSP", null), &target->SpellBookSP, state);
        Marshal(node.GetAttributeValue("SpellBookMP", null), &target->SpellBookMP, state);
        Marshal(node.GetAttributeValue("UnattachedMpBeaconTemplate", null), &target->UnattachedMpBeaconTemplate, state);
        Marshal(node.GetAttributeValue("AttachedMpBeaconTemplate", null), &target->AttachedMpBeaconTemplate, state);
        Marshal(node.GetAttributeValue("MaxMpBeacons", "0"), &target->MaxMpBeacons, state);
        string[] values = (node.GetAttributeValue("ResourceModifierValueList", null)?.GetText() ?? "").Split(WhiteSpaces, StringSplitOptions.RemoveEmptyEntries);
        if (values.Length != 0)
        {
            uint count = (uint)values.Length;
            state.InplaceEndianToPlatform(&count);
            target->ResourceModifierValueList.Count = count;
            using var allocation = state.Push((void**)&target->ResourceModifierValueList.Items, sizeof(uint), (uint)values.Length);
            for (int i = 0; i < values.Length; i++) Marshal(values[i], &target->ResourceModifierValueList.Items[i], state);
        }
        Marshal(node.GetAttributeValue("NumberOfResourceAccounts", "1"), &target->NumberOfResourceAccounts, state);
        Marshal(node.GetAttributeValue("ScriptedHuntTactic", null), &target->ScriptedHuntTactic, state);
        Marshal(node.GetAttributeValue("ParachuteOCL", null), &target->ParachuteOCL, state);
        Marshal(node.GetAttributeValue("DefaultTech", null), &target->DefaultTech, state);
        Marshal(node.GetAttributeValue("PlayerTechUpgradeBinding", null), &target->PlayerTechUpgradeBinding, state);
        Marshal(node.GetAttributeValue("TechRankPointsPerSecond", "0"), &target->TechRankPointsPerSecond, state);
        Marshal(node.GetAttributeValue("InitialTechPoints", "0"), &target->InitialTechPoints, state);
        Marshal(node.GetChildNode("RedAlertTemplate", null), &target->RedAlertTemplate, state);
        Marshal(node.GetChildNode("PreferredColor", null), &target->PreferredColor, state);
        Marshal(node.GetChildNode("PreferredCampaignRadarColor", null), &target->PreferredCampaignRadarColor, state);
        Marshal(node.GetChildNodes("InitialUpgrade"), &target->InitialUpgrade, state);
        Marshal(node.GetChildNodes("StartingUnitInfo"), &target->StartingUnitInfo, state);
        Marshal(node.GetChildNode("ResourceModifierObjectFilter", null), &target->ResourceModifierObjectFilter, state);
        Marshal(node.GetChildNodes("StartingMoney"), &target->StartingMoney, state);
        Marshal(node.GetChildNodes("MoneyCap"), &target->MoneyCap, state);
        Marshal(node.GetChildNodes("AIBeaconSettings"), &target->AIBeaconSettings, state);
        Marshal(node.GetChildNodes("TechRank"), &target->TechRank, state);
        Marshal(node.GetChildNodes("TechProductionBonuses"), &target->TechProductionBonuses, state);
        Marshal(node.GetAttributeValue("PowerOutageTurnsOffRadar", "true"), &target->PowerOutageTurnsOffRadar, state);
    }

    // Reborn: retain the starting unit import and inline position.
    public static unsafe void Marshal(Node node, Ep1StartingUnitInfo* target, Tracker state)
    {
        Marshal(node.GetChildNode("Object", null), &target->Object, state);
        Marshal(node.GetChildNode("Position", null), &target->Position, state);
    }

    // Reborn: beacon object names are weak handles.
    public static unsafe void Marshal(Node node, Ep1PlayerBeacon* target, Tracker state)
    {
        uint kind = unchecked((uint)Enum.Parse<Ep1PlayerBeaconKind>(node.GetAttributeValue("BeaconType", null).GetText()));
        state.InplaceEndianToPlatform(&kind);
        target->BeaconType = (Ep1PlayerBeaconKind)kind;
        Marshal(node.GetAttributeValue("UnattachedTemplate", null), &target->UnattachedTemplate, state);
        Marshal(node.GetAttributeValue("AttachedTemplate", null), &target->AttachedTemplate, state);
    }

    // Reborn: production modifiers keep the six source percentages in retail order.
    public static unsafe void Marshal(Node node, Ep1TechProductionBonus* target, Tracker state)
    {
        Marshal(node.GetAttributeValue("RequiredTech", null), &target->RequiredTech, state);
        Marshal(node.GetAttributeValue("BuildCostBonus", "0"), &target->BuildCostBonus, state);
        Marshal(node.GetAttributeValue("BuildCostBonusAllies", "0"), &target->BuildCostBonusAllies, state);
        Marshal(node.GetAttributeValue("BuildTimeBonus", "0"), &target->BuildTimeBonus, state);
        Marshal(node.GetAttributeValue("BuildTimeBonusAllies", "0"), &target->BuildTimeBonusAllies, state);
        Marshal(node.GetAttributeValue("HarvesterBonus", "0"), &target->HarvesterBonus, state);
        Marshal(node.GetAttributeValue("HarvesterBonusAllies", "0"), &target->HarvesterBonusAllies, state);
    }
}
