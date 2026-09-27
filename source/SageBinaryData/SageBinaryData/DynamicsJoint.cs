using Relo;
using System.Runtime.InteropServices;
using AnsiString = Relo.String<sbyte>;

namespace SageBinaryData
{
    // Reborn: Preserve the five native EP1 swing modes in schema order.
    public enum DynamicsJointSwingType
    {
        SWING_LOCKED,
        SWING_CONE,
        SWING_HINGE,
        SWING_AXLE,
        SWING_FREE
    }

    // Reborn: Preserve the three native EP1 twist modes in schema order.
    public enum DynamicsJointTwistType
    {
        TWIST_LOCKED,
        TWIST_ARC,
        TWIST_FREE
    }

    // Reborn: Match the 12-byte EP1 bone link with an optional local position.
    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct DynamicsJointLinkType
    {
        public AnsiString BoneName;
        public Vector3* Position;
    }

    // Reborn: Match the 24-byte EP1 frame in native child-then-parent order.
    [StructLayout(LayoutKind.Sequential)]
    public struct DynamicsJointFrameType
    {
        public DynamicsJointLinkType Child;
        public DynamicsJointLinkType Parent;
    }

    // Reborn: Match the 32-byte EP1 limit block with its trailing optional position pointer.
    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct DynamicsJointLimitsType
    {
        public DynamicsJointSwingType SwingType;
        public float SwingDisplacementLimit;
        public float SwingAngleLimit;
        public DynamicsJointTwistType TwistType;
        public float TwistDisplacementLimit;
        public float TwistAngleLimit;
        public float InertiaOverride;
        public Vector3* Position;
    }

    // Reborn: Match the 56-byte EP1 joint record recovered from real ragdoll draw data.
    [StructLayout(LayoutKind.Sequential)]
    public struct DynamicsJointType
    {
        public DynamicsJointFrameType Frame;
        public DynamicsJointLimitsType Limits;
    }

    // Reborn: Match the 8-byte EP1 joint-set list root.
    [StructLayout(LayoutKind.Sequential)]
    public struct DynamicsJointSetType
    {
        public List<DynamicsJointType> Joint;
    }
}
