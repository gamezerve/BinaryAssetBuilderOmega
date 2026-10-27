using Relo;
using SageBinaryData;

public static partial class Marshaler
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: retain EVA asset imports for create announcements instead of serializing legacy string pointers. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node, Ep1EvaAnnounceClientCreateModuleData* objT, Tracker state)
    {
        if(node is null)return;
        Marshal(node.GetAttributeValue(nameof(Ep1EvaAnnounceClientCreateModuleData.AnnouncementEventEnemy),null),&objT->AnnouncementEventEnemy,state);
        Marshal(node.GetAttributeValue(nameof(Ep1EvaAnnounceClientCreateModuleData.AnnouncementEventAlly),null),&objT->AnnouncementEventAlly,state);
        Marshal(node.GetAttributeValue(nameof(Ep1EvaAnnounceClientCreateModuleData.AnnouncementEventOwner),null),&objT->AnnouncementEventOwner,state);
        Marshal(node.GetAttributeValue(nameof(Ep1EvaAnnounceClientCreateModuleData.DelayBeforeAnnouncementLogicFrames),"0"),&objT->DelayBeforeAnnouncementLogicFrames,state);
        Marshal(node.GetAttributeValue(nameof(Ep1EvaAnnounceClientCreateModuleData.OnlyIfVisible),"false"),&objT->OnlyIfVisible,state);
        Marshal(node.GetAttributeValue(nameof(Ep1EvaAnnounceClientCreateModuleData.CountAsFirstSightedAnnouncement),"false"),&objT->CountAsFirstSightedAnnouncement,state);
        Marshal(node.GetAttributeValue(nameof(Ep1EvaAnnounceClientCreateModuleData.UseObjectsPosition),"false"),&objT->UseObjectsPosition,state);
        Marshal(node.GetAttributeValue(nameof(Ep1EvaAnnounceClientCreateModuleData.CreateFakeRadarEvent),"false"),&objT->CreateFakeRadarEvent,state);
        Marshal(node,(ClientUpdateModuleData*)objT,state);
    }
}
