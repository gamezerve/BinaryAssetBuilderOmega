using System.Runtime.InteropServices;
using Relo;

namespace SageBinaryData;

// Reborn: EP1 GameObject root locomotor entries retain Speed/Condition/strong-reference order in a twelve-byte inline record.
[StructLayout(LayoutKind.Sequential)]
public struct Ep1LocomotorSet
{
    public float Speed;
    public Ep1RootLocomotorSetType Condition;
    public AssetReference<LocomotorTemplate> Locomotor;
}
