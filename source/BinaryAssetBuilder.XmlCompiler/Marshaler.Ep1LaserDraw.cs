using Relo;
using SageBinaryData;
public static partial class Marshaler
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: write the complete 112-byte EP1 laser draw with official defaults while leaving the original KW writer unchanged. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1W3DLaserDrawModuleData* objT,Tracker state)
    {
        if(node==null)return;
        Marshal(node.GetAttributeValue("Texture1_UTile","1.0"),&objT->Texture1_UTile,state);
        Marshal(node.GetAttributeValue("Texture1_VTile","1.0"),&objT->Texture1_VTile,state);
        Marshal(node.GetAttributeValue("Texture2_UTile","1.0"),&objT->Texture2_UTile,state);
        Marshal(node.GetAttributeValue("Texture2_VTile","1.0"),&objT->Texture2_VTile,state);
        Marshal(node.GetAttributeValue("Texture1_UScrollRate","0.0"),&objT->Texture1_UScrollRate,state);
        Marshal(node.GetAttributeValue("Texture1_VScrollRate","0.0"),&objT->Texture1_VScrollRate,state);
        Marshal(node.GetAttributeValue("Texture2_UScrollRate","0.0"),&objT->Texture2_UScrollRate,state);
        Marshal(node.GetAttributeValue("Texture2_VScrollRate","0.0"),&objT->Texture2_VScrollRate,state);
        Marshal(node.GetAttributeValue("Texture1_NumFrames","1"),&objT->Texture1_NumFrames,state);
        Marshal(node.GetAttributeValue("Texture1_FrameRate","30.0"),&objT->Texture1_FrameRate,state);
        Marshal(node.GetAttributeValue("Texture2_NumFrames","1"),&objT->Texture2_NumFrames,state);
        Marshal(node.GetAttributeValue("Texture2_FrameRate","30.0"),&objT->Texture2_FrameRate,state);
        Marshal(node.GetAttributeValue("LaserWidth","5.0"),&objT->LaserWidth,state);
        Marshal(node.GetAttributeValue("FadeInTime","0s"),&objT->FadeInTime,state);
        Marshal(node.GetAttributeValue("FadeOutTime","0s"),&objT->FadeOutTime,state);
        Marshal(node.GetAttributeValue("WeaponSlotID","-1"),&objT->WeaponSlotID,state);
        Marshal(node.GetAttributeValue("LaserStateID","0"),&objT->LaserStateID,state);
        Marshal(node.GetChildNode("FXShader",null),&objT->FXShader,state);
        Marshal(node.GetChildNode("ObjectStatusValidation",null),&objT->ObjectStatusValidation,state);
        Marshal(node.GetAttributeValue("UseDistortionShader","false"),&objT->UseDistortionShader,state);
        Marshal(node,(DrawModuleData*)objT,state);
    }
}
