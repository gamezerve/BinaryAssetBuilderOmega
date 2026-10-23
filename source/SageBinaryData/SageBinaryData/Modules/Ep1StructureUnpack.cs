using System.Runtime.InteropServices;

namespace SageBinaryData;

// Reborn: EP1 StructureUnpack inherits the complete existing GenericUnpack record, including water-height offset and optional audio storage.
[StructLayout(LayoutKind.Sequential)]
public struct Ep1StructureUnpackUpdateModuleData
{
    public GenericUnpackUpdateModuleData Base;
}
