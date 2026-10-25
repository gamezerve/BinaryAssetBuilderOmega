using System.Runtime.InteropServices;

namespace SageBinaryData;

// Reborn: retail EP1 wall metadata places the inline connection filter before the trailing boolean; WallHubBehavior is a different class.
[StructLayout(LayoutKind.Sequential)]
public struct WallBuilderBehaviorModuleData
{
    public BehaviorModuleData Base;
    public int MaxNumSegments;
    public ModelConditionBitFlags SecondaryModelConditions;
    public ObjectStatusBitFlags SecondaryObjectStatus;
    public ObjectFilter ConnectingObjectFilter;
    public SageBool StartInSecondaryModel;
}
