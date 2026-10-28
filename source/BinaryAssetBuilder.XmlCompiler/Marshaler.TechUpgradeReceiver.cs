using Relo;
using SageBinaryData;

public static partial class Marshaler
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: compile complete Allied receiver settings, optional rectangular radius, radar link and inherited upgrade conditions. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,TechUpgradeReceiverModuleData* objT,Tracker state)
    {
        if(node is null)return;
        Marshal(node.GetAttributeValue("Radius",null),&objT->Radius,state);
        Marshal(node.GetAttributeValue("RadiusY",null),&objT->RadiusY,state);
        Marshal(node.GetAttributeValue("DistanceType","CIRCULAR"),&objT->DistanceType,state);
        Marshal(node.GetAttributeValue("FXLineBoneName",null),&objT->FXLineBoneName,state);
        Marshal(node.GetAttributeValue("RadarDrawModule",null),&objT->RadarDrawModule,state);
        Marshal(node.GetAttributeValue("RadarRotateSpeed","0.1r"),&objT->RadarRotateSpeed,state);
        Marshal(node.GetAttributeValue("RadarRotateOffset","0r"),&objT->RadarRotateOffset,state);
        Marshal(node.GetChildNode("ObjectFilter",null),&objT->ObjectFilter,state);
        Marshal(node,(UpgradeModuleData*)objT,state);
    }
}
