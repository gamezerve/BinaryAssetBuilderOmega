using System.Runtime.InteropServices;
namespace SageBinaryData;

// Reborn: the original EP1 substructure marker is an identity-only BehaviorModuleData record.
[StructLayout(LayoutKind.Sequential)]
public struct SubStructureBehaviorModuleData
{
    public BehaviorModuleData Base;
}
