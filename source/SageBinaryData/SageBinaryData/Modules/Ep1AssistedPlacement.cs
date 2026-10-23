using System.Runtime.InteropServices;

namespace SageBinaryData;

// Reborn: EP1 placement-hint ordinals; stock SovietRefinery "FACING_TOWARDS ALLOW_SHROUDED" stores bits 1 and 3.
public enum AssistanceHintType
{
    NONE,
    FACING_TOWARDS,
    FACING_AWAY,
    ALLOW_SHROUDED
}

[StructLayout(LayoutKind.Sequential)]
public struct AssistanceHintBitFlags
{
    public const int Count = 4;
    public const int BitsInSpan = 32;
    public const int NumSpans = (Count + (BitsInSpan - 1)) / BitsInSpan;

    public unsafe fixed uint Value[NumSpans];
}

// Reborn: 20-byte EP1 record (stock SovietRefinery witness): hint mask, snapping distance and an optional ObjectFilter pointee.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct AssistedPlacementBehaviorModuleData
{
    public BehaviorModuleData Base;
    public AssistanceHintBitFlags PlacementHint;
    public float SnappingDistance;
    public ObjectFilter* RequirementObjectFilter;
}
