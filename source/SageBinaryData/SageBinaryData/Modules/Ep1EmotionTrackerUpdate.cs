using Relo;
using System.Runtime.InteropServices;

namespace SageBinaryData;

// Reborn: Uprising removes KW's quarrel probability and stores emotion-template imports instead of strings in its 56-byte tracker.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1EmotionTrackerUpdateModuleData
{
    public UpdateModuleData Base;
    public float TauntAndPointDistance;
    public float HeroScanDistance;
    public Time TauntAndPointUpdateDelay;
    public float FearScanDistance;
    public int ImmuneToFearLevel;
    public ObjectFilter* TauntAndPointExcluded;
    public ObjectFilter* AfraidOf;
    public ObjectFilter* AlwaysAfraidOf;
    public ObjectFilter* PointAt;
    public List<AssetReference<BaseAssetType>> AddEmotion;
    public SageBool IgnoreVeterancy;
}
