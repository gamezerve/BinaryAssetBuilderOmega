using Relo;
using SageBinaryData;

public static partial class Marshaler
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: Uprising shares the 24-byte aiming layout but defaults its low/high thresholds to -0.4/+0.4 rather than KW's zero values. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void MarshalEp1AimWeapon(Node node,AimWeaponBehaviorModuleData* value,Tracker state)
    {
        if(node is null)return;
        Marshal(node.GetAttributeValue("AimLowThreshold","-0.4"),&value->AimLowThreshold,state);
        Marshal(node.GetAttributeValue("AimHighThreshold","0.4"),&value->AimHighThreshold,state);
        Marshal(node.GetAttributeValue("AimNearDistance","0"),&value->AimNearDistance,state);
        Marshal(node.GetAttributeValue("AimFarDistance","0"),&value->AimFarDistance,state);
        Marshal(node,(UpdateModuleData*)value,state);
    }
}
