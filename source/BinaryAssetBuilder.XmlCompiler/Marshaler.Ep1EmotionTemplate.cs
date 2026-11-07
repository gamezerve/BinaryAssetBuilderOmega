using Relo;
using SageBinaryData;
public static partial class Marshaler
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: encode EP1 emotion kinds in their official order rather than the legacy KW enumeration. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Value value,Ep1EmotionType* target,Tracker state)
        { if(value is null||string.IsNullOrWhiteSpace(value.GetText()))return;Ep1ToggleEnum(value,(uint*)target,state,typeof(Ep1EmotionType),false); }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: preserve EP1's explicit NONE AI state and subsequent original ordinals. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Value value,Ep1EmotionAIType* target,Tracker state)
        { if(value is null||string.IsNullOrWhiteSpace(value.GetText()))return;Ep1ToggleEnum(value,(uint*)target,state,typeof(Ep1EmotionAIType),false); }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: encode the three original threat comparison states. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Value value,Ep1EmotionTemplateThreatComparisonType* target,Tracker state)
        { if(value is null||string.IsNullOrWhiteSpace(value.GetText()))return;Ep1ToggleEnum(value,(uint*)target,state,typeof(Ep1EmotionTemplateThreatComparisonType),false); }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: convert the original emotion option names into their schema-ordered bit positions. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Value value,Ep1EmotionTemplateOptionsBitFlags* target,Tracker state)
        =>Ep1ToggleEnum(value,(uint*)target,state,typeof(Ep1EmotionTemplateOptions),true);
    //-------------------------------------------------------------------------------------------------
    /** Reborn: serialize every original emotion duration, effect, condition mask and event-list field. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1EmotionTemplate* target,Tracker state)
    {
        if(node is null)return;
        Marshal(node.GetAttributeValue(nameof(Ep1EmotionTemplate.Type),null),&target->Type,state);
        Marshal(node.GetAttributeValue(nameof(Ep1EmotionTemplate.Options),null),&target->Options,state);
        Marshal(node.GetAttributeValue(nameof(Ep1EmotionTemplate.DurationActive),null),&target->DurationActive,state);
        Marshal(node.GetAttributeValue(nameof(Ep1EmotionTemplate.DurationInactive),null),&target->DurationInactive,state);
        Marshal(node.GetAttributeValue(nameof(Ep1EmotionTemplate.DurationInactiveObject),null),&target->DurationInactiveObject,state);
        Marshal(node.GetAttributeValue(nameof(Ep1EmotionTemplate.DurationInactiveType),null),&target->DurationInactiveType,state);
        Marshal(node.GetAttributeValue(nameof(Ep1EmotionTemplate.EnemyThreatComparison),null),&target->EnemyThreatComparison,state);
        Marshal(node.GetAttributeValue(nameof(Ep1EmotionTemplate.EnemyThreat),null),&target->EnemyThreat,state);
        Marshal(node.GetAttributeValue(nameof(Ep1EmotionTemplate.FriendlyThreatComparison),null),&target->FriendlyThreatComparison,state);
        Marshal(node.GetAttributeValue(nameof(Ep1EmotionTemplate.FriendlyThreat),null),&target->FriendlyThreat,state);
        Marshal(node.GetAttributeValue(nameof(Ep1EmotionTemplate.StartFX),null),&target->StartFX,state);
        Marshal(node.GetAttributeValue(nameof(Ep1EmotionTemplate.UpdateFX),null),&target->UpdateFX,state);
        Marshal(node.GetAttributeValue(nameof(Ep1EmotionTemplate.EndFX),null),&target->EndFX,state);
        Marshal(node.GetAttributeValue(nameof(Ep1EmotionTemplate.AttributeModifier),null),&target->AttributeModifier,state);
        Marshal(node.GetAttributeValue(nameof(Ep1EmotionTemplate.AttributeModifierStartDelay),null),&target->AttributeModifierStartDelay,state);
        Marshal(node.GetAttributeValue(nameof(Ep1EmotionTemplate.AttributeModifierDuration),null),&target->AttributeModifierDuration,state);
        Marshal(node.GetAttributeValue(nameof(Ep1EmotionTemplate.AIStateLockDuration),null),&target->AIStateLockDuration,state);
        Marshal(node.GetAttributeValue(nameof(Ep1EmotionTemplate.AIState),null),&target->AIState,state);
        Marshal(node.GetAttributeValue(nameof(Ep1EmotionTemplate.ModelConditionsSetOnStart),null),&target->ModelConditionsSetOnStart,state);
        Marshal(node.GetAttributeValue(nameof(Ep1EmotionTemplate.ModelConditionsSetOnEnd),null),&target->ModelConditionsSetOnEnd,state);
        Marshal(node.GetAttributeValue(nameof(Ep1EmotionTemplate.ModelConditionsClearOnStart),null),&target->ModelConditionsClearOnStart,state);
        Marshal(node.GetAttributeValue(nameof(Ep1EmotionTemplate.ModelConditionsClearOnEnd),null),&target->ModelConditionsClearOnEnd,state);
        Marshal(node.GetAttributeValue(nameof(Ep1EmotionTemplate.AILuaEventsList),null),&target->AILuaEventsList,state);
        Marshal(node,(BaseAssetType*)target,state);
    }
}
