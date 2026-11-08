using Relo;
using System.Runtime.InteropServices;
using AnsiString = Relo.String<sbyte>;

namespace SageBinaryData
{
    public enum UIConfigType
    {
        INGAME,
        SHELL,
        // Reborn: original EP1 UI configurations include the bootup mode.
        BOOTUP,
        // Reborn: original EP1 also has a separate map loading-screen configuration.
        LOADSCREEN
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct UIDynamicConfig
    {
        public StringHash Name;
        public UIConfigType Type;
        public PolymorphicList<UIBaseComponent> ComponentList;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct UIConfigList
    {
        public BaseAssetType Base;
        public AnsiString GDIBackground;
        public AnsiString NodBackground;
        public AnsiString AlienBackground;
        public List<UIDynamicConfig> UIDynamicConfig;
    }
}
