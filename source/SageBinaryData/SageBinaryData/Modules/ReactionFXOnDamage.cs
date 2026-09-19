using System.Runtime.InteropServices;
using Relo;

namespace SageBinaryData;

[StructLayout(LayoutKind.Sequential)]
public struct ReactionFXTriggerData
{
    public unsafe float* PercentDamagedThreshold;
    public unsafe Time* TimeBetweenTriggers;
    public unsafe AssetReference<ObjectFilterAsset>* SourceFilter;
    public unsafe StringHash* NameOfVoiceToPlay;
    public unsafe AssetReference<BaseAudioEventInfo, AudioEventInfo>* SoundToPlay;
    public SageBool ResetTimerWhenTimerBlocks;
}

[StructLayout(LayoutKind.Sequential)]
public struct ReactionFXOnDamageModuleData
{
    public DamageModuleData Base;
    public List<ReactionFXTriggerData> DamageReactionFXTrigger;
    public List<ReactionFXTriggerData> HealingReactionFXTrigger;
}
