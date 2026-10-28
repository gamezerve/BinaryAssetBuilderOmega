using System.Runtime.InteropServices;

namespace SageBinaryData;

// Reborn: Kirov's unsuffixed EP1 update class has a 116-byte root with inline status/model-condition masks.
[StructLayout(LayoutKind.Sequential)]
public struct SwitchLocomotorUpdate
{
    public UpdateModuleData Base;
    // Reborn: use all EP1 locomotor ordinals rather than the shorter, differently ordered KW enum.
    public Ep1RootLocomotorSetType LocomotorSet;
    public Time UpdatePeriod;
    public float RevealShroudToAllPlayersRadius;
    public Percentage MinimumHealth;
    public ObjectStatusBitFlags ObjectStatusWhileActive;
    public ModelConditionBitFlags ModelConditionWhileActive;
}
