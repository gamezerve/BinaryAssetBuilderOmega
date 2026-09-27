using Relo;
using System.Runtime.InteropServices;
using AnsiString = Relo.String<sbyte>;

namespace SageBinaryData
{
    // Reborn: Match the 28-byte EP1 scenario enemy record recovered from static.bin.
    [StructLayout(LayoutKind.Sequential)]
    public struct ScenarioEnemy
    {
        public AssetReference<BaseAssetType> Faction;
        public AssetReference<AIPersonalityDefinition> Personality;
        public AssetReference<PackedTextureImage> Portrait;
        public AIDifficulty Difficulty;
        public uint Team;
        public uint StartPosition;
        public TypedAssetId<MultiplayerColor> Color;
    }

    // Reborn: Match the 96-byte EP1 scenario root and its trailing packed booleans.
    [StructLayout(LayoutKind.Sequential)]
    public struct ScenarioTemplate
    {
        public TypedAssetId<ScenarioTemplate> Id;
        public AnsiString UiName;
        public AnsiString Title;
        public AnsiString ShortTitle;
        public AnsiString Description;
        public AnsiString MapName;
        public uint PlayerStartPosition;
        public uint PlayerTeam;
        public TypedAssetId<MultiplayerColor> PlayerColor;
        public Time ParTime;
        public uint ParTimeScoreBonus;
        public uint DifficultyScoreBonus;
        public float MaxEfficiencyScoreMultiplier;
        public TypedAssetId<GameObject> UnitUnlock;
        public List<ScenarioEnemy> Enemy;
        public List<TypedAssetId<ScenarioTemplate>> ScenarioUnlock;
        public SageBool IsStartingScenario;
        public SageBool IsCriticalPath;
        public SageBool UseRandomCrates;
    }

    // Reborn: Match the 8-byte EP1 unlockable-unit faction/import and weak unit ID pair.
    [StructLayout(LayoutKind.Sequential)]
    public struct UnlockableUnit
    {
        public AssetReference<BaseAssetType> Faction;
        public TypedAssetId<GameObject> Name;
    }

    // Reborn: Match the 48-byte EP1 scenario-manager root recovered from the real static stream.
    [StructLayout(LayoutKind.Sequential)]
    public struct ScenarioManagerData
    {
        public BaseAssetType Base;
        public AnsiString IntroMovie;
        public AnsiString CriticalPathCompletionMovie;
        public AnsiString FullCompletionMovie;
        public int MaxRedAlertDeposit;
        public List<ScenarioTemplate> ScenarioTemplate;
        public List<UnlockableUnit> UnlockableUnit;
    }
}
