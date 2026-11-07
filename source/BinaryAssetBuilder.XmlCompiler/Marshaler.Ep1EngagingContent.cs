using Relo;
using SageBinaryData;

public static partial class Marshaler
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: serialize the localized loading-screen title/description IDs and runtime image lookup string. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1EngagingContent* target,Tracker state)
    {
        if(node is null)return;
        Marshal(node.GetAttributeValue(nameof(Ep1EngagingContent.TitleId),null),&target->TitleId,state);
        Marshal(node.GetAttributeValue(nameof(Ep1EngagingContent.DescId),null),&target->DescId,state);
        Marshal(node.GetAttributeValue(nameof(Ep1EngagingContent.ImageId),null),&target->ImageId,state);
        Marshal(node,(BaseAssetType*)target,state);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: write the ordered loading-screen imports using Core's concrete occurrence indices. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1EngagingContentGroup* target,Tracker state)
    {
        if(node is null)return;
        Marshal(node.GetChildNodes(nameof(Ep1EngagingContentGroup.EngagingContent)),&target->EngagingContent,state);
        Marshal(node,(BaseAssetType*)target,state);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: connect the map-specific required/random pools without repacking stock content into the sequence. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1EngagingContentSequence* target,Tracker state)
    {
        if(node is null)return;
        Marshal(node.GetAttributeValue(nameof(Ep1EngagingContentSequence.MapName),null),&target->MapName,state);
        Marshal(node.GetAttributeValue(nameof(Ep1EngagingContentSequence.RequiredContent),null),&target->RequiredContent,state);
        Marshal(node.GetAttributeValue(nameof(Ep1EngagingContentSequence.RandomContent),null),&target->RandomContent,state);
        Marshal(node,(BaseAssetType*)target,state);
    }
}
