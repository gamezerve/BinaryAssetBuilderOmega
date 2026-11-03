using Relo;
using System.Runtime.InteropServices;

namespace SageBinaryData
{
    // Reborn: Uprising extends the legacy environment by a fourth texture import and the map root by optional analysis data.
    [StructLayout(LayoutKind.Sequential)]
    public struct Ep1EnvironmentObject
    {
        public AssetReference<Texture> Cloud;
        public AssetReference<Texture> Macro;
        public AssetReference<Texture> Environment;
        public AssetReference<Texture> DepthLut;
    }
    // Reborn: enumeration values follow the original EP1 WaterBodyType XSD order.
    public enum WaterBodyType { INTERIOR, EXTERIOR, EXTERIOR_PARTIAL }
    // Reborn: map analysis stores the five declared counters followed by its source-sized water-area list.
    [StructLayout(LayoutKind.Sequential)]
    public struct MapAnalysisType
    {
        public int GarrisonableStructures;
        public int TechStructures;
        public int CellsExamined;
        public int WaterCells;
        public int WaterAreas;
        public List<WaterBodyType> WaterAreaType;
    }
    // Reborn: keep the original KW GameMap untouched while explicitly modelling all EP1 runtime fields; MapMetaData is pipeline-only.
    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct Ep1GameMap
    {
        public BaseAssetType Base;
        public List<MapObject> Waypoint;
        public List<MapObjectUSP> Structure;
        public List<MapObjectUSP> Unit;
        public List<MapObjectUSP> Prop;
        public List<MapObject> Audio;
        public List<MapObject> Unknown;
        public List<RoadObject> Road;
        public Ep1EnvironmentObject* EnvironmentData;
        public Dictionary* WorldDict;
        public List<PostEffectObject> PostEffect;
        public MapAnalysisType* MapAnalysis;
    }
}
