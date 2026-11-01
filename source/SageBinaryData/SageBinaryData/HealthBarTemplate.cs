
using System.Runtime.InteropServices;

namespace SageBinaryData;

[StructLayout(LayoutKind.Sequential)]
public struct HealthBarTemplate
{
    public BaseAssetType Base;
    public uint Length;
}
