using System.Runtime.InteropServices;
using Relo;

namespace SageBinaryData;

// Reborn: EP1 campaign map radar images carry one texture import and three integer coordinate pairs.
public enum Ep1MapBorder { Invalid = -1, Orange, Green, Blue, Cyan, Magenta, Yellow, Purple, Pink }

[StructLayout(LayoutKind.Sequential, Size = 40)]
public struct Ep1RadarTerrainImage
{
    public BaseAssetType Base;
    public Ep1MapBorder MapBorder;
    public AssetReference<Texture> Texture;
    public ICoord2D ImageSize;
    public ICoord2D ImagePosition;
    public ICoord2D TextureSize;
    public SageBool Rotated;
}
