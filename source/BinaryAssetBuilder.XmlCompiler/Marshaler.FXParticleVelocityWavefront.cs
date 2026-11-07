using Relo;
using SageBinaryData;
public static partial class Marshaler
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: preserve the original zero-based wavefront shape enum rather than applying the unrelated INVALID=-1 convention. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Value value,FXParticleSystem_WavefrontShape* target,Tracker state)
    { if(value is null)return;uint word=(uint)System.Enum.Parse<FXParticleSystem_WavefrontShape>(value.GetText());state.InplaceEndianToPlatform(&word);*target=(FXParticleSystem_WavefrontShape)word; }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: compile all five original wave random variables inline, preserving absent line ranges as zero records. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,FXParticleEmissionVelocityWavefront* target,Tracker state)
    {
        if(node is null)return;
        Marshal(node.GetAttributeValue(nameof(FXParticleEmissionVelocityWavefront.Shape),"CIRCLE"),&target->Shape,state);
        Marshal(node.GetChildNode(nameof(FXParticleEmissionVelocityWavefront.Velocity),null),&target->Velocity,state);
        Marshal(node.GetChildNode(nameof(FXParticleEmissionVelocityWavefront.Amplitude),null),&target->Amplitude,state);
        Marshal(node.GetChildNode(nameof(FXParticleEmissionVelocityWavefront.InitialRadius),null),&target->InitialRadius,state);
        Marshal(node.GetChildNode(nameof(FXParticleEmissionVelocityWavefront.LineWidth),null),&target->LineWidth,state);
        Marshal(node.GetChildNode(nameof(FXParticleEmissionVelocityWavefront.LineAngle),null),&target->LineAngle,state);
        Marshal(node,(FXParticleEmissionVelocityBase*)target,state);
    }
}
