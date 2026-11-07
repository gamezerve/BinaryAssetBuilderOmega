using System.Runtime.InteropServices;
namespace SageBinaryData;

// Reborn: this mask follows Relationship.xsd, whose ordinal order differs from ObjectFilterRelationship.
public enum RelationshipType { ENEMIES, NEUTRAL, ALLIES }

// Reborn: the original visibility relationship mask occupies one 32-bit span.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct RelationshipBitFlags
{
    public const int Count=3, BitsInSpan=32, NumSpans=1;
    public fixed uint Value[NumSpans];
}

// Reborn: both original visibility fields are optional allocations, not inline defaults.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct SelectiveVisibilityBehaviorModuleData
{
    public BehaviorModuleData Base;
    public Time* UpdateRate;
    public RelationshipBitFlags* AllowedPlayers;
}
