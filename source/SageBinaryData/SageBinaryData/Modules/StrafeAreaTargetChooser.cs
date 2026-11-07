using System.Runtime.InteropServices;

namespace SageBinaryData;

// Reborn: retail EP1 packs both target overflow choices in one word.
public enum StrafeAreaTargetChooserFlagType
{
    CHOOSE_RANDOM_POSITION,
    BREAK_OFF_ATTACK
}

[StructLayout(LayoutKind.Sequential)]
public unsafe struct StrafeAreaTargetChooserFlags
{
    public fixed uint Value[1];
}

// Reborn: the retail target chooser has a 24-byte header followed by an optional ObjectFilter allocation.
[StructLayout(LayoutKind.Sequential)]
public struct StrafeAreaTargetChooserModuleData
{
    public BehaviorModuleData Base;
    public float Radius;
    public int MaxAttacksPerTarget;
    public StrafeAreaTargetChooserFlags OverflowOptions;
    public unsafe ObjectFilter* ObjectFilter;
}
