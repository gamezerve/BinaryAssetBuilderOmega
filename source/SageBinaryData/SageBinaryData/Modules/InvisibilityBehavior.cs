using Relo;
using System.Runtime.InteropServices;
namespace SageBinaryData;
// Reborn: the EP1 invisibility behavior is a behavior header followed by one strong InvisibilityTemplate asset reference.
[StructLayout(LayoutKind.Sequential)]
public struct InvisibilityBehaviorModuleData
{
    public BehaviorModuleData Base;
    public AssetReference<BaseAssetType> InvisibilityTemplate;
}
