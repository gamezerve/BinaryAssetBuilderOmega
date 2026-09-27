using Relo;
using System.Runtime.InteropServices;
using AnsiString = Relo.String<sbyte>;

namespace SageBinaryData
{
    // Reborn: Preserve the official RA3/EP1 dynamics draw priorities in schema order.
    public enum DynamicsDrawPriority { REMOVABLE, REMOVE_ONLY_IF_NEEDED, MUST_BE_DRAWN }
    // Reborn: Preserve the official RA3/EP1 collision modes in schema order.
    public enum DynamicsCollisionType { NONINTERCOLLIDING, INTERCOLLIDING, OMNICOLLIDING, NONCOLLIDING }
    // Reborn: Preserve the official intuitive collision modes in schema order.
    public enum IntuitiveCollisionType { COLLIDES_WITH_GROUND_ONLY, COLLIDES_WITH_GROUND_AND_VEHICLES, COLLIDES_WITH_EVERYTHING }
    // Reborn: Preserve the official contact tags in schema order.
    public enum DynamicsContactTag { NONE, GROUND, WATER, VEHICLE, PROP, WALL, TREE, DEBRIS }

    // Reborn: Match the official 8-byte dynamics shape transform-pointer base.
    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct DynamicsShapeType
    {
        public Vector3* Translation;
        public Quaternion* Rotation;
    }

    // Reborn: Match the official 12-byte sphere shape.
    [StructLayout(LayoutKind.Sequential)]
    public struct DynamicsSphereShapeType { public DynamicsShapeType Base; public float Radius; }
    // Reborn: Match the official 16-byte capsule shape.
    [StructLayout(LayoutKind.Sequential)]
    public struct DynamicsCapsuleShapeType { public DynamicsShapeType Base; public float Radius; public float HalfLength; }
    // Reborn: Match the official 24-byte box shape.
    [StructLayout(LayoutKind.Sequential)]
    public struct DynamicsBoxShapeType { public DynamicsShapeType Base; public float HalfSizeX; public float HalfSizeY; public float HalfSizeZ; public float EdgeRadius; }
    // Reborn: Match the official 20-byte cylinder shape.
    [StructLayout(LayoutKind.Sequential)]
    public struct DynamicsCylinderShapeType { public DynamicsShapeType Base; public float Radius; public float HalfLength; public float EdgeRadius; }

    // Reborn: Match the official 48-byte triangle shape.
    [StructLayout(LayoutKind.Sequential)]
    public struct DynamicsTriangleShapeType
    {
        public DynamicsShapeType Base;
        public float X1; public float Y1; public float Z1;
        public float X2; public float Y2; public float Z2;
        public float X3; public float Y3; public float Z3;
        public float EdgeRadius;
    }

    // Reborn: Match the official 76-byte dynamics volume scalar and five-list layout.
    [StructLayout(LayoutKind.Sequential)]
    public struct DynamicsVolumeType
    {
        public float Mass;
        public Percentage Bounciness;
        public float Spinniness;
        public float StaticFriction;
        public float KineticFriction;
        public float AngularDrag;
        public float LinearDrag;
        public DynamicsContactTag ContactTag;
        public SageBool ReportContacts;
        public List<DynamicsSphereShapeType> Sphere;
        public List<DynamicsCapsuleShapeType> Capsule;
        public List<DynamicsBoxShapeType> Box;
        public List<DynamicsCylinderShapeType> Cylinder;
        public List<DynamicsTriangleShapeType> Triangle;
    }

    // Reborn: Match the official 84-byte bone volume and 8-byte volume-set root.
    [StructLayout(LayoutKind.Sequential)]
    public struct DynamicsBoneVolumeType { public DynamicsVolumeType Base; public AnsiString BoneName; }
    [StructLayout(LayoutKind.Sequential)]
    public struct DynamicsBoneVolumeSetType { public List<DynamicsBoneVolumeType> BoneVolume; }

    // Reborn: Match the official 8-byte dynamics lifetime pair.
    [StructLayout(LayoutKind.Sequential)]
    public struct DynamicsLifetime { public Time Delay; public Time FadeTime; }

    // Reborn: Extend the official 216-byte scripted-model base to the verified 256-byte EP1 root.
    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct W3DDynamicsDrawModuleData
    {
        public W3DScriptedModelDrawModuleData Base;
        public DynamicsCollisionType Collision;
        public IntuitiveCollisionType IntuitiveCollision;
        public DynamicsDrawPriority DrawPriority;
        public EffectsLODType LODPriority;
        public float Explodiness;
        public float FlingPerturbation;
        public DynamicsBoneVolumeSetType* BoneVolumes;
        public DynamicsLifetime* Lifetime;
        public DynamicsJointSetType* Joints;
        public SageBool InitiallyActive;
    }
}
