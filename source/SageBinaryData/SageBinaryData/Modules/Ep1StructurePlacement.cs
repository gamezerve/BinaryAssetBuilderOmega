using System.Runtime.InteropServices;

namespace SageBinaryData;

// Reborn: 56-byte EP1 record (stock SovietRefinery witness): the restricted-area shape is the inline 48-byte EP1 Shape record.
[StructLayout(LayoutKind.Sequential)]
public struct StructurePlacementBehaviorModuleData
{
    public BehaviorModuleData Base;
    public Ep1Shape RestrictedAreaShape;
}
