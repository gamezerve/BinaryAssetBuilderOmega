using Relo;
using SageBinaryData;
public static partial class Marshaler
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: compile the original EP1 light-source intensity ratio without inventing a GPU payload. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,FXParticleDrawLightSource* target,Tracker state)
    {
        if(node is null)return;
        Marshal(node.GetAttributeValue(nameof(FXParticleDrawLightSource.MaximumIntensityRadiusRatio),"0.0"),&target->MaximumIntensityRadiusRatio,state);
        Marshal(node,(FXParticleDrawBase*)target,state);
    }
}
