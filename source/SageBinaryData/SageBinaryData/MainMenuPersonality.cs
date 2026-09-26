using Relo;
using System.Runtime.InteropServices;
using AnsiString = Relo.String<sbyte>;

namespace SageBinaryData
{
    // Reborn: Store an EP1 shell personality's weak image ID and relocatable music-track name.
    [StructLayout(LayoutKind.Sequential)]
    public struct MainMenuPersonalityTemplate
    {
        public BaseAssetType Base;
        public TypedAssetId<OnDemandTextureImage> MainMenuPersonalityImage;
        public AnsiString MainMenuPersonalityMusic;
    }

    // Reborn: Store the default EP1 shell personality and its ordered personality reference list.
    [StructLayout(LayoutKind.Sequential)]
    public struct MainMenuPersonalityGroup
    {
        public BaseAssetType Base;
        public AssetReference<MainMenuPersonalityTemplate> DefaultPersonality;
        public List<AssetReference<MainMenuPersonalityTemplate>> MainMenuPersonality;
    }
}
