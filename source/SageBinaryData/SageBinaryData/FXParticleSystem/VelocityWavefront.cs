using System.Runtime.InteropServices;
namespace SageBinaryData;

// Reborn: the observed EP1 wavefront keeps INVALID at zero, CIRCLE at one and LINE at two.
public enum FXParticleSystem_WavefrontShape { INVALID,CIRCLE,LINE }
// Reborn: wavefront velocity owns five inline twelve-byte random variables after its class and shape words.
[StructLayout(LayoutKind.Sequential)]
public struct FXParticleEmissionVelocityWavefront
{
    public FXParticleEmissionVelocityBase Base;
    public FXParticleSystem_WavefrontShape Shape;
    public ClientRandomVariable Velocity,Amplitude,InitialRadius,LineWidth,LineAngle;
}
