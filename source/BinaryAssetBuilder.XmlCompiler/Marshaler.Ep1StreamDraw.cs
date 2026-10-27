using Relo;
using SageBinaryData;
public static partial class Marshaler
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: write the complete eighty-byte EP1 stream draw while leaving the original KW writer unchanged. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1W3DStreamDrawModuleData* objT,Tracker state)
    {
        if(node==null)return;
        Marshal(node.GetAttributeValue("Velocity","200.0"),&objT->Velocity,state);
        Marshal(node.GetAttributeValue("OffsetMult","0.0"),&objT->OffsetMult,state);
        Marshal(node.GetAttributeValue("ArcHeightFactor","0.0"),&objT->ArcHeightFactor,state);
        Marshal(node.GetAttributeValue("UVWorldSize","10.0"),&objT->UVWorldSize,state);
        Marshal(node.GetAttributeValue("NumTubeSides","8"),&objT->NumTubeSides,state);
        Marshal(node.GetAttributeValue("TubeRadius","1.0"),&objT->TubeRadius,state);
        Marshal(node.GetAttributeValue("HitFx",null),&objT->HitFx,state);
        Marshal(node.GetAttributeValue("WeaponSlotID","-1"),&objT->WeaponSlotID,state);
        Marshal(node.GetAttributeValue("StreamStateID","0"),&objT->StreamStateID,state);
        Marshal(node.GetChildNode("FXShader",null),&objT->FXShader,state);
        Marshal(node.GetChildNode("ParticleSystems",null),&objT->ParticleSystems,state);
        Marshal(node.GetAttributeValue("UseDistortionShader","false"),&objT->UseDistortionShader,state);
        Marshal(node,(DrawModuleData*)objT,state);
    }
}
