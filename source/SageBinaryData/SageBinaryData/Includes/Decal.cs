using Relo;
using System.Runtime.InteropServices;

namespace SageBinaryData
{
    // Reborn: RA3/EP1 values follow the official 7-entry ShadowType map (EP1 Decal.xsd order; ADDITIVE_DECAL=3 is proved on the stock
    // SovietCrane decal). The former order was the KW enum with VOLUME at 2, which shifted every later value by one.
    // KW-only names stay at the end so nothing that names them stops compiling; the EP1 schema never admits them.
    public enum ShadowType
    {
        NONE = 0,
        DECAL = 1,
        ALPHA_DECAL = 2,
        ADDITIVE_DECAL = 3,
        ALPHA_DECAL_DYNAMIC = 4,
        ADDITIVE_DECAL_DYNAMIC = 5,
        MERGE_DECAL = 6,
        VOLUME = 7,
        VOLUME_OR_DECAL = 8,
        TIBERIUM_ROOT = 9
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct ShadowInfo
    {
        public ShadowType Type;
        public AssetReference<Texture> Texture;
        public AssetReference<Texture> AdditionalTexture;
        public float SizeX;
        public float SizeY;
        public float OffsetX;
        public float OffsetY;
        public float OpacityStart;
        public Time OpacityFadeInTime;
        public float OpacityPeak;
        public Time OpacityFadeOutTime;
        public float OpacityEnd;
        public float MaxHeight;
        public float SunAngle;
        public SageBool OverrideLODVisibility;
        public SageBool UseHouseColor;
        public SageBool IsRotatingWithObject;
        public SageBool LocalPlayerOnly;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct RadiusDecalTemplate
    {
        public AssetReference<Texture> Texture;
        public AssetReference<Texture> Texture2;
        public ShadowType Style;
        public Percentage OpacityMin;
        public Percentage OpacityMax;
        public float OpacityThrobTime;
        public float RotationsPerMinute;
        public float MaxRadius;
        public float MinRadius;
        public uint MaxSelectedUnits;
        public float SpiralAcceleration;
        public Color Color;
        public SageBool OnlyVisibleToOwningPlayer;
    }
}
