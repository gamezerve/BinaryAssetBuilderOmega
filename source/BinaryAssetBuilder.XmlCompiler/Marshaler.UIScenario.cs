using Relo;
using SageBinaryData;

public static partial class Marshaler
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: Compile an EP1 scenario-preview faction and its packed player portrait reference. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node, UIScenarioMapPreviewFactionSettings* objT, Tracker state)
    {
        if (node is null)
        {
            return;
        }
        Marshal(node.GetAttributeValue(nameof(UIScenarioMapPreviewFactionSettings.Faction), null), &objT->Faction, state);
        Marshal(node.GetAttributeValue(nameof(UIScenarioMapPreviewFactionSettings.PlayerImage), null), &objT->PlayerImage, state);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: Compile EP1's ordered scenario-preview faction list. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node, UIScenarioMapPreview* objT, Tracker state)
    {
        if (node is null)
        {
            return;
        }
        Marshal(node.GetChildNodes(nameof(UIScenarioMapPreview.FactionSettings)), &objT->FactionSettings, state);
        Marshal(node, (BaseAssetType*)objT, state);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: Compile EP1's fieldless scenario UI component through UIBaseComponent. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node, UIComponentScenario* objT, Tracker state)
    {
        Marshal(node, (UIBaseComponent*)objT, state);
    }
}
