using Relo;
using System.Runtime.InteropServices;

namespace SageBinaryData
{
    // Reborn: Store one EP1 scenario-preview faction and its packed player portrait import.
    [StructLayout(LayoutKind.Sequential)]
    public struct UIScenarioMapPreviewFactionSettings
    {
        public FactionType Faction;
        public AssetReference<PackedTextureImage> PlayerImage;
    }

    // Reborn: Store the ordered EP1 scenario-preview faction settings as a standalone asset.
    [StructLayout(LayoutKind.Sequential)]
    public struct UIScenarioMapPreview
    {
        public BaseAssetType Base;
        public List<UIScenarioMapPreviewFactionSettings> FactionSettings;
    }

    // Reborn: Represent EP1's fieldless scenario UI component through its inherited component ABI.
    [StructLayout(LayoutKind.Sequential)]
    public struct UIComponentScenario
    {
        public UIBaseComponent Base;
    }
}
