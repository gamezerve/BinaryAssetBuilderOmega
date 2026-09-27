using Relo;
using SageBinaryData;

public static partial class Marshaler
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: Compile the shared EP1 archive-movie strings and on-demand preview reference. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node, GeneralArchiveMovie* objT, Tracker state)
    {
        if (node is null)
        {
            return;
        }
        Marshal(node.GetAttributeValue(nameof(GeneralArchiveMovie.Movie), null), &objT->Movie, state);
        Marshal(node.GetAttributeValue(nameof(GeneralArchiveMovie.PreviewImage), null), &objT->PreviewImage, state);
        Marshal(node.GetAttributeValue(nameof(GeneralArchiveMovie.DisplayName), null), &objT->DisplayName, state);
        Marshal(node.GetAttributeValue(nameof(GeneralArchiveMovie.Description), null), &objT->Description, state);
        Marshal(node.GetAttributeValue(nameof(GeneralArchiveMovie.Icon), null), &objT->Icon, state);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: Compile an EP1 Commander's Challenge movie and its unlock requirement. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node, ScenarioArchiveMovie* objT, Tracker state)
    {
        if (node is null)
        {
            return;
        }
        Marshal(node.GetAttributeValue(nameof(ScenarioArchiveMovie.UnlockRequirement), null), &objT->UnlockRequirement, state);
        Marshal(node, (GeneralArchiveMovie*)objT, state);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: Compile an EP1 campaign movie and its faction/progress unlock keys. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node, CampaignArchiveMovie* objT, Tracker state)
    {
        if (node is null)
        {
            return;
        }
        Marshal(node.GetAttributeValue(nameof(CampaignArchiveMovie.Faction), null), &objT->Faction, state);
        Marshal(node.GetAttributeValue(nameof(CampaignArchiveMovie.ProgressLock), null), &objT->ProgressLock, state);
        Marshal(node, (GeneralArchiveMovie*)objT, state);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: Compile EP1's three movie-archive lists instead of the obsolete KW MissionSpec field. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node, UIComponentMovieArchive* objT, Tracker state)
    {
        if (node is null)
        {
            return;
        }
        Marshal(node.GetChildNodes(nameof(UIComponentMovieArchive.GeneralMovie)), &objT->GeneralMovie, state);
        Marshal(node.GetChildNodes(nameof(UIComponentMovieArchive.ScenarioMovie)), &objT->ScenarioMovie, state);
        Marshal(node.GetChildNodes(nameof(UIComponentMovieArchive.CampaignMovie)), &objT->CampaignMovie, state);
        Marshal(node, (UIBaseComponent*)objT, state);
    }
}
