using Relo;
using SageBinaryData;
public static partial class Marshaler
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: compile editable ore capacity, delivery time and the optional inherited armor-mode status gate. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,OreMinerModuleData* objT,Tracker state)
    {
        if(node==null)return;
        Marshal(node.GetAttributeValue("DeliveryCapacity","1000"),&objT->DeliveryCapacity,state);
        Marshal(node.GetAttributeValue("DeliveryTime","1s"),&objT->DeliveryTime,state);
        Marshal(node.GetChildNode("ObjectStatusValidation",null),&objT->ObjectStatusValidation,state);
        Marshal(node,(CreateModuleData*)objT,state);
    }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: preserve the identity-only incoming-fire tracker instead of falling back to unknown native data. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,TrackIncomingWeaponFireBehaviorModuleData* objT,Tracker state)
    {if(node==null)return;Marshal(node,(BehaviorModuleData*)objT,state);}
    //-------------------------------------------------------------------------------------------------
    /** Reborn: damage tracking retains its required source time and official optional boolean default. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,TrackDamageModuleData* objT,Tracker state)
    {
        if(node==null)return;Marshal(node.GetAttributeValue("TimeToTrack",null),&objT->TimeToTrack,state);Marshal(node.GetAttributeValue("InstantWeaponsOnly","false"),&objT->InstantWeaponsOnly,state);Marshal(node,(DamageModuleData*)objT,state);
    }
}
