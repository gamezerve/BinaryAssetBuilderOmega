using Relo;
using SageBinaryData;

public static partial class Marshaler
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: marshal complete EP1 teleport timing, effects, filtering and optional shader imports using official defaults. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node, TeleportStoredObjectsUpdateModuleData* objT, Tracker state)
    {
        if(node is null)return;
        Marshal(node.GetAttributeValue(nameof(TeleportStoredObjectsUpdateModuleData.TriggerFX),null),&objT->TriggerFX,state);
        Marshal(node.GetAttributeValue(nameof(TeleportStoredObjectsUpdateModuleData.TargetFX),null),&objT->TargetFX,state);
        Marshal(node.GetAttributeValue(nameof(TeleportStoredObjectsUpdateModuleData.FadeInTime),"0s"),&objT->FadeInTime,state);
        Marshal(node.GetAttributeValue(nameof(TeleportStoredObjectsUpdateModuleData.InitialDelayTime),"0s"),&objT->InitialDelayTime,state);
        Marshal(node.GetAttributeValue(nameof(TeleportStoredObjectsUpdateModuleData.DelayTime),"0s"),&objT->DelayTime,state);
        Marshal(node.GetAttributeValue(nameof(TeleportStoredObjectsUpdateModuleData.ScaleFadeInTimeByDistance),"0.0"),&objT->ScaleFadeInTimeByDistance,state);
        Marshal(node.GetAttributeValue(nameof(TeleportStoredObjectsUpdateModuleData.Telefrag),"NONE"),&objT->Telefrag,state);
        Marshal(node.GetAttributeValue(nameof(TeleportStoredObjectsUpdateModuleData.SpecialPowerTemplate),null),&objT->SpecialPowerTemplate,state);
        Marshal(node.GetAttributeValue(nameof(TeleportStoredObjectsUpdateModuleData.Shader),null),&objT->Shader,state);
        Marshal(node.GetAttributeValue(nameof(TeleportStoredObjectsUpdateModuleData.DisabledTypesToProcess),"HELD"),&objT->DisabledTypesToProcess,state);
        Marshal(node.GetAttributeValue(nameof(TeleportStoredObjectsUpdateModuleData.GetObjectsCurrentlyAtStorePosition),"false"),&objT->GetObjectsCurrentlyAtStorePosition,state);
        Marshal(node.GetChildNode(nameof(TeleportStoredObjectsUpdateModuleData.InstaKillObjectFilter),null),&objT->InstaKillObjectFilter,state);
        Marshal(node.GetChildNode(nameof(TeleportStoredObjectsUpdateModuleData.CollidesWithOnTeleport),null),&objT->CollidesWithOnTeleport,state);
        Marshal(node,(UpdateModuleData*)objT,state);
    }
}
