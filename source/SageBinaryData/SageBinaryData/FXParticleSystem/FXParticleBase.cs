using System.Runtime.InteropServices;

namespace SageBinaryData
{
    [StructLayout(LayoutKind.Sequential)]
    public struct FXParticleBaseModule : IPolymorphic
    {
        public uint TypeId;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct RandCoord3D
    {
#pragma warning disable IDE1006 // Naming Styles
        public ClientRandomVariable x;
        public ClientRandomVariable y;
        public ClientRandomVariable z;
#pragma warning restore IDE1006 // Naming Styles
    }

    public enum FXParticleSystem_Type
    {
        INVALID_TYPE,
        PARTICLE,
        DRAWABLE,
        STREAK,
        VOLUME_PARTICLE,
        GPU_PARTICLE,
        GPU_TERRAINFIRE,
        SWARM,
        TRAIL,
        // Reborn: EP1 appends the original wave particle type at native ordinal nine.
        WAVE_PARTICLE
    }

    public enum FXParticleSystem_Priority
    {
        INVALID_PRIORITY,
        ULTRA_HIGH_ONLY,
        HIGH_OR_ABOVE,
        MEDIUM_OR_ABOVE,
        LOW_OR_ABOVE,
        VERY_LOW_OR_ABOVE,
        ALWAYS_RENDER,
        // Reborn: EP1 keeps the matching low-LOD range at native ordinals seven through eleven.
        ULTRA_HIGH_OR_BELOW,
        HIGH_OR_BELOW,
        MEDIUM_OR_BELOW,
        LOW_OR_BELOW,
        VERY_LOW_ONLY
    }

    public enum FXParticleSystem_WindMotion
    {
        INVALID,
        NOT_USED,
        PING_PONG,
        CIRCULAR
    }

    public enum FXParticleSystem_GeometryType
    {
        INVALID,
        SIMPLE_QUAD,
        CENTERED_QUAD,
        TWO_CONCENTRIC_QUADS,
        // Reborn: the original EP1 GPU record stores TWO_QUADS as four.
        TWO_QUADS
    }

    public enum FXParticleSystem_ShaderType
    {
        INVALID_SHADER,
        ADDITIVE,
        ADDITIVE_ALPHA_TEST,
        ALPHA,
        ALPHA_TEST,
        MULTIPLY,
        ADDITIVE_NO_DEPTH_TEST,
        ALPHA_NO_DEPTH_TEST,
        W3D_DIFFUSE,
        W3D_ALPHA,
        W3D_EMISSIVE
    }

    public enum FXParticleSystem_RotationType
    {
        INVALID_ROTATION,
        ROTATION_OFF,
        // Reborn: EP1 uses velocity/world rotation modes at native ordinals two and three.
        ROTATE_AROUND_VELOCITY,
        ROTATE_IN_WORLD_SPACE
    }
}
