using Relo;
using SageBinaryData;
public static partial class Marshaler
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: compile all official EP1 attack-assist response fields with schema defaults, without fixing the local ID, status or weapon slot to Tesla Coil values. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,AttackAssistResponseBehaviorModuleData* value,Tracker state)
    {
        if(node is null)return;
        Marshal(node.GetAttributeValue("ObjectStatus",""),&value->ObjectStatus,state);
        Marshal(node.GetAttributeValue("WeaponSlotID",null),&value->WeaponSlotID,state);
        Marshal(node.GetAttributeValue("WeaponSlotType",null),&value->WeaponSlotType,state);
        Marshal(node,(BehaviorModuleData*)value,state);
    }
}
