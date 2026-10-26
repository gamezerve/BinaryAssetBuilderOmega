using System.Runtime.InteropServices;

namespace SageBinaryData;

// Reborn: the official EP1 template-ID store adds no fields to BehaviorModuleData; native type and local module ID occupy eight bytes.
[StructLayout(LayoutKind.Sequential)]
public struct StoreTemplateIDBehaviorModuleData
{
    public BehaviorModuleData Base;
}
