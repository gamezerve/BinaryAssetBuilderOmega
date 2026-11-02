using System.Runtime.InteropServices;
using Relo;

namespace SageBinaryData;

// Reborn: EP1 ObjectSet has no KW Shader word; three items occupy 48 bytes in retail AlliedAAGun01.
[StructLayout(LayoutKind.Sequential)]
public struct Ep1FXParticleDrawRenderObjectObjectSet
{
    public String<sbyte> RenderGroup;
    public int NumObjects;
    public Percentage Percent;
}

// Reborn: preserve the EP1 20-byte draw header while selecting its corrected 16-byte list item.
[StructLayout(LayoutKind.Sequential)]
public struct Ep1FXParticleDrawRenderObject
{
    public FXParticleDrawBase Base;
    public float SinkRate;
    public List<Ep1FXParticleDrawRenderObjectObjectSet> ObjectSet;
    public SageBool MultiRenderObjects;
    public SageBool SinkOnTerrainCollision;
}

// Reborn: original EP1 rotation names replace KW axis selectors and retain their native XSD ordinals.
public enum Ep1FXParticleRotationType { INVALID_ROTATION, ROTATION_OFF, ROTATE_AROUND_VELOCITY, ROTATE_IN_WORLD_SPACE }

// Reborn: EP1 RenderObject updates include both X/Y angle pairs before the legacy Z/damping/size fields.
[StructLayout(LayoutKind.Sequential)]
public struct Ep1FXParticleUpdateRenderObject
{
    public FXParticleUpdateBase Base;
    public Ep1FXParticleRotationType Rotation;
    public ClientRandomVariable AngleX;
    public ClientRandomVariable AngularRateX;
    public ClientRandomVariable AngleY;
    public ClientRandomVariable AngularRateY;
    public ClientRandomVariable AngleZ;
    public ClientRandomVariable AngularRateZ;
    public ClientRandomVariable AngularDamping;
    public RandCoord3D StartSize;
    public RandCoord3D SizeRate;
    public RandCoord3D SizeDamping;
}
