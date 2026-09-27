using System.Runtime.InteropServices;
using Relo;

namespace SageBinaryData
{
    // Reborn: Encode the four official Commander's Challenge movie unlock thresholds.
    public enum ScenarioMovieUnlockRequirement
    {
        None,
        CriticalPath,
        All,
        AllUnderPar
    }

    // Reborn: Store the shared EP1 movie-archive metadata in native schema order.
    [StructLayout(LayoutKind.Sequential)]
    public struct GeneralArchiveMovie
    {
        public String<sbyte> Movie;
        public AssetReference<OnDemandTextureImage> PreviewImage;
        public String<sbyte> DisplayName;
        public String<sbyte> Description;
        public String<sbyte> Icon;
    }

    // Reborn: Extend an archive entry with the Commander's Challenge unlock threshold.
    [StructLayout(LayoutKind.Sequential)]
    public struct ScenarioArchiveMovie
    {
        public GeneralArchiveMovie Base;
        public ScenarioMovieUnlockRequirement UnlockRequirement;
    }

    // Reborn: Extend an archive entry with the campaign faction and mission progress lock.
    [StructLayout(LayoutKind.Sequential)]
    public struct CampaignArchiveMovie
    {
        public GeneralArchiveMovie Base;
        public FactionType Faction;
        public int ProgressLock;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct UIComponentMovieArchive
    {
        public UIBaseComponent Base;
        // Reborn: EP1 replaces KW's MissionSpec string with three typed movie-entry lists.
        public List<GeneralArchiveMovie> GeneralMovie;
        public List<ScenarioArchiveMovie> ScenarioMovie;
        public List<CampaignArchiveMovie> CampaignMovie;
    }
}
