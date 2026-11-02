using Relo;
using SageBinaryData;

public static partial class Marshaler
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: consume all six original Uprising lightning range fields, preserving omitted ranges and negative changes. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node, Ep1FXParticleDrawLightningRandomSet* objT, Tracker state)
    {
        if(node is null)return;
        Marshal(node.GetChildNode("StartAmplitude",null),&objT->StartAmplitude,state);
        Marshal(node.GetChildNode("EndAmplitudeChange",null),&objT->EndAmplitudeChange,state);
        Marshal(node.GetChildNode("StartFrequency",null),&objT->StartFrequency,state);
        Marshal(node.GetChildNode("EndFrequencyChange",null),&objT->EndFrequencyChange,state);
        Marshal(node.GetChildNode("StartPhase",null),&objT->StartPhase,state);
        Marshal(node.GetChildNode("EndPhaseChange",null),&objT->EndPhaseChange,state);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: write the original EP1 lightning coordinates and ordered random-set list through the common allocator. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node, Ep1FXParticleDrawLightning* objT, Tracker state)
    {
        if(node is null)return;
        Marshal(node.GetChildNode("StartPoint",null),&objT->StartPoint,state);
        Marshal(node.GetChildNode("EndPoint",null),&objT->EndPoint,state);
        Marshal(node.GetChildNodes("RandomSet"),&objT->RandomSet,state);
        Marshal(node,&objT->Base,state);
    }
}
