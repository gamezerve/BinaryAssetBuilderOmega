using System.Runtime.InteropServices;

namespace SageBinaryData;

// Reborn: Transport uses the complete stock EP1 AI record under its own class word.
[StructLayout(LayoutKind.Sequential)]
public struct Ep1TransportAIUpdateModuleData
{
    public const uint Wire = 0xBE9CD6FEu;
    public Ep1AIUpdateModuleData Base;
}

// Reborn: Wander appends an inline model-condition mask and the distance/flags after the EP1 parent.
[StructLayout(LayoutKind.Sequential)]
public struct Ep1WanderAIUpdateModuleData
{
    public const uint Wire = 0xD813AF2Au;
    public Ep1AIUpdateModuleData Base;
    public ModelConditionBitFlags EntryCondition;
    public uint WanderDistance;
    public SageBool AttackAll;
    public SageBool Selectable;
}
