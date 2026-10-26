using Relo;
using SageBinaryData;

public static partial class Marshaler
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: retain EVA asset imports and both readiness/visibility flags instead of serializing legacy string pointers. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node, Ep1EvaAnnounceSpecialPowerReadyClientUpdateModuleData* objT, Tracker state)
    {
        if(node is null)return;
        Marshal(node.GetAttributeValue(nameof(Ep1EvaAnnounceSpecialPowerReadyClientUpdateModuleData.AnnouncementEventEnemy),null),&objT->AnnouncementEventEnemy,state);
        Marshal(node.GetAttributeValue(nameof(Ep1EvaAnnounceSpecialPowerReadyClientUpdateModuleData.AnnouncementEventAlly),null),&objT->AnnouncementEventAlly,state);
        Marshal(node.GetAttributeValue(nameof(Ep1EvaAnnounceSpecialPowerReadyClientUpdateModuleData.AnnouncementEventOwner),null),&objT->AnnouncementEventOwner,state);
        Marshal(node.GetAttributeValue(nameof(Ep1EvaAnnounceSpecialPowerReadyClientUpdateModuleData.SpecialPowerTemplate),null),&objT->SpecialPowerTemplate,state);
        Marshal(node.GetAttributeValue(nameof(Ep1EvaAnnounceSpecialPowerReadyClientUpdateModuleData.JustMonitorPercentReady),"false"),&objT->JustMonitorPercentReady,state);
        Marshal(node.GetAttributeValue(nameof(Ep1EvaAnnounceSpecialPowerReadyClientUpdateModuleData.CheckPlayerPowerVisibility),"false"),&objT->CheckPlayerPowerVisibility,state);
        Marshal(node,(ClientUpdateModuleData*)objT,state);
    }
}
