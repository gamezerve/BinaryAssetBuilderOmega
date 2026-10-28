using Relo;
using SageBinaryData;
public static partial class Marshaler
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: preserve all source-driven wheel/cab fields and optional terrain/tread records with the actual Uprising offsets. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1W3DTruckDrawModuleData* objT,Tracker state)
    {
        if(node==null)return;
        Marshal(node.GetAttributeValue("LeftFrontTireBone",null),&objT->LeftFrontTireBone,state);
        Marshal(node.GetAttributeValue("RightFrontTireBone",null),&objT->RightFrontTireBone,state);
        Marshal(node.GetAttributeValue("LeftRearTireBone",null),&objT->LeftRearTireBone,state);
        Marshal(node.GetAttributeValue("RightRearTireBone",null),&objT->RightRearTireBone,state);
        Marshal(node.GetAttributeValue("MidLeftFrontTireBone",null),&objT->MidLeftFrontTireBone,state);
        Marshal(node.GetAttributeValue("MidRightFrontTireBone",null),&objT->MidRightFrontTireBone,state);
        Marshal(node.GetAttributeValue("MidLeftRearTireBone",null),&objT->MidLeftRearTireBone,state);
        Marshal(node.GetAttributeValue("MidRightRearTireBone",null),&objT->MidRightRearTireBone,state);
        Marshal(node.GetAttributeValue("MidLeftMidTireBone",null),&objT->MidLeftMidTireBone,state);
        Marshal(node.GetAttributeValue("MidRightMidTireBone",null),&objT->MidRightMidTireBone,state);
        Marshal(node.GetAttributeValue("LeftFrontTireBone2",null),&objT->LeftFrontTireBone2,state);
        Marshal(node.GetAttributeValue("RightFrontTireBone2",null),&objT->RightFrontTireBone2,state);
        Marshal(node.GetAttributeValue("LeftRearTireBone2",null),&objT->LeftRearTireBone2,state);
        Marshal(node.GetAttributeValue("RightRearTireBone2",null),&objT->RightRearTireBone2,state);
        Marshal(node.GetAttributeValue("MidLeftMidTireBone2",null),&objT->MidLeftMidTireBone2,state);
        Marshal(node.GetAttributeValue("MidRightMidTireBone2",null),&objT->MidRightMidTireBone2,state);
        Marshal(node.GetAttributeValue("TireRotationMultiplier","1.0"),&objT->TireRotationMultiplier,state);
        Marshal(node.GetAttributeValue("TireRotationMultiplierFront","1.0"),&objT->TireRotationMultiplierFront,state);
        Marshal(node.GetAttributeValue("PowerslideRotationAddition","0"),&objT->PowerslideRotationAddition,state);
        Marshal(node.GetAttributeValue("CabBone",null),&objT->CabBone,state);
        Marshal(node.GetAttributeValue("TrailerBone",null),&objT->TrailerBone,state);
        Marshal(node.GetAttributeValue("CabRotationMultiplier","1.0"),&objT->CabRotationMultiplier,state);
        Marshal(node.GetAttributeValue("TrailerRotationMultiplier","1.0"),&objT->TrailerRotationMultiplier,state);
        Marshal(node.GetAttributeValue("RotationDamping","1.0"),&objT->RotationDamping,state);
        Marshal(node.GetChildNode("TreadTerrainParticles",null),&objT->TreadTerrainParticles,state);
        Marshal(node.GetChildNodes("TreadScrollingMap"),&objT->TreadScrollingMap,state);
        Marshal(node,(W3DScriptedModelDrawModuleData*)objT,state);
    }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: authored track strings and percentages use the official XSD defaults without a stock-sized list restriction. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1TreadScrollingMapping* objT,Tracker state)
    {
        if(node==null)return;
        Marshal(node.GetAttributeValue("LeftTrack",null),&objT->LeftTrack,state);
        Marshal(node.GetAttributeValue("RightTrack",null),&objT->RightTrack,state);
        Marshal(node.GetAttributeValue("SpeedMultiplier","1.0"),&objT->SpeedMultiplier,state);
        Marshal(node.GetAttributeValue("TreadPivotSpeedFraction","60%"),&objT->TreadPivotSpeedFraction,state);
        Marshal(node.GetAttributeValue("TreadDriveSpeedFraction","20%"),&objT->TreadDriveSpeedFraction,state);
    }
}
