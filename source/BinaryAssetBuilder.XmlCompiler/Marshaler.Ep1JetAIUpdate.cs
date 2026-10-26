using Relo;
using SageBinaryData;
public static partial class Marshaler
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: allocate the separate EP1 jet record and retain its concrete identity after the shared AI base writer. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void MarshalEp1JetAIUpdate(Node node,UpdateModuleData** objT,Tracker state)
    {
        if(node is null)return;
        using(var context=state.Push((void**)objT,(uint)sizeof(Ep1JetAIUpdateModuleData),1u)){
            var jet=(Ep1JetAIUpdateModuleData*)*objT;
            Marshal(node,(Ep1AIUpdateModuleData*)jet,state);
            Marshal(node.GetAttributeValue("OutOfAmmoDamagePerSecond","0"),&jet->OutOfAmmoDamagePerSecond,state);
            Marshal(node.GetAttributeValue("OutOfAmmoDamageDelay",null),&jet->OutOfAmmoDamageDelay,state);
            Marshal(node.GetAttributeValue("ParkingOffset","0.0"),&jet->ParkingOffset,state);
            Marshal(node.GetAttributeValue("UseForOutOfAmmoCheck",null),&jet->UseForOutOfAmmoCheck,state);
            Marshal(node.GetAttributeValue("OutOfAmmoEvaEvent",null),&jet->OutOfAmmoEvaEvent,state);
            Marshal(node.GetAttributeValue("NeedsRunway","false"),&jet->NeedsRunway,state);
            Marshal(node.GetAttributeValue("ReturnToBaseWhenVictimDead","false"),&jet->ReturnToBaseWhenVictimDead,state);
            Marshal(node.GetAttributeValue("KeepsParkingSpaceWhenAirborne","false"),&jet->KeepsParkingSpaceWhenAirborne,state);
            Marshal(node.GetAttributeValue("CirclesForAttack","false"),&jet->CirclesForAttack,state);
            uint type=0x4486CF48;state.InplaceEndianToPlatform(&type);*(uint*)jet=type;
        }
    }
}
