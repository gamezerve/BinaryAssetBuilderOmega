using Relo;
using SageBinaryData;

public static partial class Marshaler
{
    public static unsafe void Marshal(Node node, ReactionFXTriggerData* objT, Tracker state)
    {
        if (node is null)
        {
            return;
        }
        Marshal(node.GetAttributeValue(nameof(ReactionFXTriggerData.PercentDamagedThreshold), null), &objT->PercentDamagedThreshold, state);
        Marshal(node.GetAttributeValue(nameof(ReactionFXTriggerData.TimeBetweenTriggers), null), &objT->TimeBetweenTriggers, state);
        Marshal(node.GetAttributeValue(nameof(ReactionFXTriggerData.SourceFilter), null), &objT->SourceFilter, state);
        Marshal(node.GetAttributeValue(nameof(ReactionFXTriggerData.NameOfVoiceToPlay), null), &objT->NameOfVoiceToPlay, state);
        Marshal(node.GetAttributeValue(nameof(ReactionFXTriggerData.SoundToPlay), null), &objT->SoundToPlay, state);
        Marshal(node.GetAttributeValue(nameof(ReactionFXTriggerData.ResetTimerWhenTimerBlocks), "false"), &objT->ResetTimerWhenTimerBlocks, state);
    }

    public static unsafe void Marshal(Node node, ReactionFXOnDamageModuleData* objT, Tracker state)
    {
        if (node is null)
        {
            return;
        }
        Marshal(node.GetChildNodes(nameof(ReactionFXOnDamageModuleData.DamageReactionFXTrigger)), &objT->DamageReactionFXTrigger, state);
        Marshal(node.GetChildNodes(nameof(ReactionFXOnDamageModuleData.HealingReactionFXTrigger)), &objT->HealingReactionFXTrigger, state);
        Marshal(node, (DamageModuleData*)objT, state);
    }
}
