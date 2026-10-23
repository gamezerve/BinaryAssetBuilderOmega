using System.Runtime.InteropServices;
using AnsiString=Relo.String<sbyte>;

namespace SageBinaryData;

// Reborn: EP1 stock QueueDock contains five inline ASCII headers and one model-condition enum after the Create base.
[StructLayout(LayoutKind.Sequential)]
public struct QueueDockModuleData
{
    public CreateModuleData Base;
    public AnsiString DockStartBoneName;
    public AnsiString DockActionBoneName;
    public AnsiString DockEndBoneName;
    public AnsiString QueueBonePrefix;
    public AnsiString TurnTableBoneName;
    public ModelConditionFlagType DockingAnimation;
}
