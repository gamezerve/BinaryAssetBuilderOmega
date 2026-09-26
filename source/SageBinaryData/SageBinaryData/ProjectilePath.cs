using Relo;
using System.Runtime.InteropServices;

namespace SageBinaryData
{
    // Reborn: Uprising replaces each legacy projectile-path point with a three-vector Bezier node.
    [StructLayout(LayoutKind.Sequential)]
    public struct ProjectilePathNode
    {
        public Vector4 InVec;
        public Vector4 Point;
        public Vector4 OutVec;
    }

    // Reborn: Preserve the 16-byte RA3 root ABI while interpreting its list as Uprising ProjectilePathNode values.
    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct ProjectilePath
    {
        public BaseInheritableAsset Base;
        public Vector4* ComponentScale;
        public List<ProjectilePathNode> Node;
    }
}
