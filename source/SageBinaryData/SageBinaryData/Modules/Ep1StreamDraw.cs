using Relo;
using System.Runtime.InteropServices;
namespace SageBinaryData
{
    // Reborn: Uprising adds OffsetMult after Velocity; shader storage and the particle-list pointer follow the 44-byte fixed draw prefix.
    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct Ep1W3DStreamDrawModuleData
    {
        public DrawModuleData Base;
        public float Velocity, OffsetMult, ArcHeightFactor, UVWorldSize;
        public int NumTubeSides;
        public float TubeRadius;
        public AssetReference<FXList> HitFx;
        public int WeaponSlotID, StreamStateID;
        public FXShaderMaterial FXShader;
        public ParticleSystemList* ParticleSystems;
        public SageBool UseDistortionShader;
    }
}
