using System.Runtime.InteropServices;

namespace SageBinaryData
{
    // Reborn: Uprising supplies a dedicated inheritable hot-key map for the Yuriko campaign UI.
    [StructLayout(LayoutKind.Sequential)]
    public struct YurikoHotKeys
    {
        public BaseInheritableAsset Base;
        public HotKeyMap Map;
    }
}
