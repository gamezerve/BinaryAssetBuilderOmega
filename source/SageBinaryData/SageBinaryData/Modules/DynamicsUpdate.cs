using Relo;
using System.Runtime.InteropServices;
namespace SageBinaryData
{
    // Reborn: motion ordinals follow the original RA3/EP1 schema declaration order.
    public enum DynamicsMotionType { TRACKED,SIMULATED,STATIC }
    // Reborn: update-only transform models preserve EA lowercase component attributes without changing legacy W3X marshaling.
    [StructLayout(LayoutKind.Sequential)]
    public struct Ep1DynamicsQuaternion { public float x,y,z,w; }
    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct Ep1DynamicsShapeType { public Coord3D* Translation; public Ep1DynamicsQuaternion* Rotation; }
    // Reborn: preserve the official sphere shape scalar layout after its two optional transform pointers.
    [StructLayout(LayoutKind.Sequential)]
    public struct Ep1DynamicsSphereShapeType { public Ep1DynamicsShapeType Base; public float Radius; }
    // Reborn: preserve the official capsule shape scalar layout after its two optional transform pointers.
    [StructLayout(LayoutKind.Sequential)]
    public struct Ep1DynamicsCapsuleShapeType { public Ep1DynamicsShapeType Base; public float Radius,HalfLength; }
    // Reborn: preserve the official box shape scalar layout after its two optional transform pointers.
    [StructLayout(LayoutKind.Sequential)]
    public struct Ep1DynamicsBoxShapeType { public Ep1DynamicsShapeType Base; public float HalfSizeX,HalfSizeY,HalfSizeZ,EdgeRadius; }
    // Reborn: preserve the official cylinder shape scalar layout after its two optional transform pointers.
    [StructLayout(LayoutKind.Sequential)]
    public struct Ep1DynamicsCylinderShapeType { public Ep1DynamicsShapeType Base; public float Radius,HalfLength,EdgeRadius; }
    // Reborn: preserve the official triangle shape scalar layout after its two optional transform pointers.
    [StructLayout(LayoutKind.Sequential)]
    public struct Ep1DynamicsTriangleShapeType { public Ep1DynamicsShapeType Base; public float X1,Y1,Z1,X2,Y2,Z2,X3,Y3,Z3,EdgeRadius; }
    // Reborn: retail EP1 puts ReportContacts after the five shape lists, unlike the pre-existing draw-only volume model.
    [StructLayout(LayoutKind.Sequential)]
    public struct Ep1DynamicsVolumeType
    {
        public float Mass;
        public Percentage Bounciness;
        public float Spinniness,StaticFriction,KineticFriction,AngularDrag,LinearDrag;
        public DynamicsContactTag ContactTag;
        public List<Ep1DynamicsSphereShapeType> Sphere;
        public List<Ep1DynamicsCapsuleShapeType> Capsule;
        public List<Ep1DynamicsBoxShapeType> Box;
        public List<Ep1DynamicsCylinderShapeType> Cylinder;
        public List<Ep1DynamicsTriangleShapeType> Triangle;
        public SageBool ReportContacts;
    }
    // Reborn: the complete thirty-six-byte Uprising update owns its optional volume separately from all fixed enum/real fields.
    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct DynamicsUpdateModuleData
    {
        public UpdateModuleData Base;
        public DynamicsMotionType Motion;
        public DynamicsCollisionType Collision;
        public IntuitiveCollisionType IntuitiveCollision;
        public DynamicsContactTag ContactTag;
        public DynamicsDrawPriority DrawPriority;
        public float FreezeDistanceFromCamera;
        public Ep1DynamicsVolumeType* Volume;
    }
}
