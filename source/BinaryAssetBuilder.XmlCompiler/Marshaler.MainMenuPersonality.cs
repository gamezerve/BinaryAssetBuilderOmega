using Relo;
using SageBinaryData;

public static partial class Marshaler
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: Compile an EP1 main-menu personality's image weak ID and music string. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node, MainMenuPersonalityTemplate* objT, Tracker state)
    {
        if (node is null)
        {
            return;
        }
        Marshal(node.GetChildNode(nameof(MainMenuPersonalityTemplate.MainMenuPersonalityImage), null), &objT->MainMenuPersonalityImage, state);
        Marshal(node.GetChildNode(nameof(MainMenuPersonalityTemplate.MainMenuPersonalityMusic), null), &objT->MainMenuPersonalityMusic, state);
        Marshal(node, (BaseAssetType*)objT, state);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: Compile the EP1 main-menu personality group and its ordered asset-reference list. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node, MainMenuPersonalityGroup* objT, Tracker state)
    {
        if (node is null)
        {
            return;
        }
        Marshal(node.GetAttributeValue(nameof(MainMenuPersonalityGroup.DefaultPersonality), null), &objT->DefaultPersonality, state);
        Marshal(node.GetChildNodes(nameof(MainMenuPersonalityGroup.MainMenuPersonality)), &objT->MainMenuPersonality, state);
        Marshal(node, (BaseAssetType*)objT, state);
    }
}
