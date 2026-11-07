using System.Runtime.InteropServices;
using Relo;
namespace SageBinaryData;

// Reborn: preserve all eighteen official EP1 build-score heuristics, including RANDOM_UNIT, UPGRADE_TECH and both test modes.
public enum Ep1AIBuildHeuristics { CONSTANT,BEST_UNIT_EXPECTED,BEST_UNIT_ACTUAL,BEST_UNIT_WEIGHTED,FEWEST_UNIT,RANDOM_UNIT,WORST_UNIT,NEEDED_UNIT,FACTORY,TECH,POWER,REFINERY,HARVESTER,UNIQUE,UPGRADE,UPGRADE_TECH,BUILD_TEST,UPGRADE_TEST }
// Reborn: the complete 56-byte build entry retains weak identities, efficiency weights, times and an optional blocking filter.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1AIBuildEntry
{
    public TypedAssetId<GameObject> Build;
    public TypedAssetId<UpgradeTemplate> Upgrade;
    public Ep1AIBuildHeuristics Heuristic;
    public Percentage PercentLow,PercentHigh;
    public float ExpectedEfficiencyWeight,ActualEfficiencyWeight;
    public int SampleSizeForFullWeight;
    public Time MinTime,ForceTime;
    public float BlockIfThingNearFactoryDistance,CountModifier,Score;
    public ObjectFilter* BlockIfThingNearFactory;
}
// Reborn: EP1 build states append a variable entry array to the complete polymorphic heuristic base.
[StructLayout(LayoutKind.Sequential)]
public struct Ep1AIBuildStateDefinition
{
    public Ep1AIStateDefinition Base;
    public List<Ep1AIBuildEntry> BuildEntry;
}
