using Relo;
using System.Runtime.InteropServices;
namespace SageBinaryData;

// Reborn: Uprising PropDraw has a 12-byte header and a polymorphic model reference; the removed KW DistanceFog field is absent.
[StructLayout(LayoutKind.Sequential)]
public struct Ep1W3DPropDrawModuleData
{
    public DrawModuleData Base;
    public AssetReference<BaseRenderAssetType> Model;
}
