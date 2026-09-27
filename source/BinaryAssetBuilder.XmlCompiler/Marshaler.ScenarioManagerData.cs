using Relo;
using SageBinaryData;

public static partial class Marshaler
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: Compile one EP1 scenario enemy in its verified 28-byte native field order. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node, ScenarioEnemy* objT, Tracker state)
    {
        if (node is null)
        {
            return;
        }
        Marshal(node.GetAttributeValue(nameof(ScenarioEnemy.Faction), null), &objT->Faction, state);
        Marshal(node.GetAttributeValue(nameof(ScenarioEnemy.Personality), null), &objT->Personality, state);
        Marshal(node.GetAttributeValue(nameof(ScenarioEnemy.Portrait), null), &objT->Portrait, state);
        Marshal(node.GetAttributeValue(nameof(ScenarioEnemy.Difficulty), null), &objT->Difficulty, state);
        Marshal(node.GetAttributeValue(nameof(ScenarioEnemy.Team), null), &objT->Team, state);
        Marshal(node.GetAttributeValue(nameof(ScenarioEnemy.StartPosition), null), &objT->StartPosition, state);
        Marshal(node.GetAttributeValue(nameof(ScenarioEnemy.Color), null), &objT->Color, state);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: Compile the 96-byte EP1 scenario root with native trailing boolean packing. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node, ScenarioTemplate* objT, Tracker state)
    {
        if (node is null)
        {
            return;
        }
        Marshal(node.GetAttributeValue("id", null), &objT->Id, state);
        Marshal(node.GetAttributeValue(nameof(ScenarioTemplate.UiName), null), &objT->UiName, state);
        Marshal(node.GetAttributeValue(nameof(ScenarioTemplate.Title), null), &objT->Title, state);
        Marshal(node.GetAttributeValue(nameof(ScenarioTemplate.ShortTitle), null), &objT->ShortTitle, state);
        Marshal(node.GetAttributeValue(nameof(ScenarioTemplate.Description), null), &objT->Description, state);
        Marshal(node.GetAttributeValue(nameof(ScenarioTemplate.MapName), null), &objT->MapName, state);
        Marshal(node.GetAttributeValue(nameof(ScenarioTemplate.PlayerStartPosition), null), &objT->PlayerStartPosition, state);
        Marshal(node.GetAttributeValue(nameof(ScenarioTemplate.PlayerTeam), "1"), &objT->PlayerTeam, state);
        Marshal(node.GetAttributeValue(nameof(ScenarioTemplate.PlayerColor), "ColorGold"), &objT->PlayerColor, state);
        Marshal(node.GetAttributeValue(nameof(ScenarioTemplate.ParTime), "0s"), &objT->ParTime, state);
        Marshal(node.GetAttributeValue(nameof(ScenarioTemplate.ParTimeScoreBonus), null), &objT->ParTimeScoreBonus, state);
        Marshal(node.GetAttributeValue(nameof(ScenarioTemplate.DifficultyScoreBonus), null), &objT->DifficultyScoreBonus, state);
        Marshal(node.GetAttributeValue(nameof(ScenarioTemplate.MaxEfficiencyScoreMultiplier), "3.0"), &objT->MaxEfficiencyScoreMultiplier, state);
        Marshal(node.GetAttributeValue(nameof(ScenarioTemplate.UnitUnlock), null), &objT->UnitUnlock, state);
        Marshal(node.GetChildNodes(nameof(ScenarioTemplate.Enemy)), &objT->Enemy, state);
        Marshal(node.GetChildNodes(nameof(ScenarioTemplate.ScenarioUnlock)), &objT->ScenarioUnlock, state);
        Marshal(node.GetAttributeValue(nameof(ScenarioTemplate.IsStartingScenario), "false"), &objT->IsStartingScenario, state);
        Marshal(node.GetAttributeValue(nameof(ScenarioTemplate.IsCriticalPath), "false"), &objT->IsCriticalPath, state);
        Marshal(node.GetAttributeValue(nameof(ScenarioTemplate.UseRandomCrates), "false"), &objT->UseRandomCrates, state);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: Compile an EP1 unlockable-unit record using an imported faction and weak unit ID. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node, UnlockableUnit* objT, Tracker state)
    {
        if (node is null)
        {
            return;
        }
        Marshal(node.GetAttributeValue(nameof(UnlockableUnit.Faction), null), &objT->Faction, state);
        Marshal(node.GetAttributeValue(nameof(UnlockableUnit.Name), null), &objT->Name, state);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: Compile the EP1 scenario manager's movie strings and two verified record lists. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node, ScenarioManagerData* objT, Tracker state)
    {
        if (node is null)
        {
            return;
        }
        Marshal(node.GetAttributeValue(nameof(ScenarioManagerData.IntroMovie), null), &objT->IntroMovie, state);
        Marshal(node.GetAttributeValue(nameof(ScenarioManagerData.CriticalPathCompletionMovie), null), &objT->CriticalPathCompletionMovie, state);
        Marshal(node.GetAttributeValue(nameof(ScenarioManagerData.FullCompletionMovie), null), &objT->FullCompletionMovie, state);
        Marshal(node.GetAttributeValue(nameof(ScenarioManagerData.MaxRedAlertDeposit), "50000"), &objT->MaxRedAlertDeposit, state);
        Marshal(node.GetChildNodes(nameof(ScenarioManagerData.ScenarioTemplate)), &objT->ScenarioTemplate, state);
        Marshal(node.GetChildNodes(nameof(ScenarioManagerData.UnlockableUnit)), &objT->UnlockableUnit, state);
        Marshal(node, (BaseAssetType*)objT, state);
    }
}
