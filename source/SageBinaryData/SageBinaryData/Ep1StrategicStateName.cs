using System.Runtime.InteropServices;
using Relo;
using AnsiString = Relo.String<sbyte>;

namespace SageBinaryData;

// Reborn: Uprising's strategic-name registry binds one real state import to the original editor-visible name.
[StructLayout(LayoutKind.Sequential)]
public struct Ep1StrategicStateName
{
    public BaseAssetType Base;
    public AssetReference<AIStrategicStateDefinition> State;
    public AnsiString Name;
}
