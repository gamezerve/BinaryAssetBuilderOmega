using Relo;
using SageBinaryData;

public static partial class Marshaler
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: serialize every official attack-notification field and its optional, source-sized friend filter without fixing values to the retail Tesla Coil. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,NotifyWhenAttackingBehaviorModuleData* value,Tracker state)
    {
        if(node is null)return;
        Marshal(node.GetAttributeValue("NotifyRadius",null),&value->NotifyRadius,state);
        Marshal(node.GetAttributeValue("AssistResponseModuleId",null),&value->AssistResponseModuleId,state);
        Marshal(node.GetAttributeValue("SleepTime","0s"),&value->SleepTime,state);
        Marshal(node.GetAttributeValue("MaxFriendsToNotify","100"),&value->MaxFriendsToNotify,state);
        Marshal(node.GetAttributeValue("WeaponSlotID","1"),&value->WeaponSlotID,state);
        Marshal(node.GetAttributeValue("WeaponSlotType","PRIMARY_WEAPON"),&value->WeaponSlotType,state);
        Marshal(node.GetAttributeValue("HasMobileFriends","false"),&value->HasMobileFriends,state);
        Marshal(node.GetAttributeValue("IgnoreDisabledStatus","false"),&value->IgnoreDisabledStatus,state);
        Marshal(node.GetChildNode("AcceptableFriends",null),&value->AcceptableFriends,state);
        Marshal(node,(UpdateModuleData*)value,state);
    }
}
