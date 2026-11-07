using System.Runtime.InteropServices;
using Relo;
using AnsiString=Relo.String<sbyte>;
namespace SageBinaryData;

// Reborn: EP1 removed QUARRELSOME, so ALERT and later emotion values must not use the legacy KW ordinals.
public enum Ep1EmotionType { TAUNT,CHEER,HERO_CHEER,POINT,FEAR,UNCONTROLLABLE_FEAR,TERROR,DOOM,ALERT,BRACE_FOR_BEING_CRUSHED,CHEER_FOR_ABOUT_TO_CRUSH }
// Reborn: the original EP1 AI emotion enumeration begins with NONE and has no QUARREL state.
public enum Ep1EmotionAIType { NONE,BACK_AWAY,AVOID_SCARER,IDLE,RUN_AWAY_PANIC,FACE_OBJECT }
// Reborn: threat comparison ordinals match their original schema order.
public enum Ep1EmotionTemplateThreatComparisonType { NONE,VALID_IF_BELOW,VALID_IF_ABOVE }
// Reborn: these six flags are bit indices in the original one-word emotion option mask.
public enum Ep1EmotionTemplateOptions { IGNORE_IF_IDLE,IGNORE_IF_BUSY,IGNORE_IF_AI,IGNORE_IF_HUMAN,ATTRIBUTEMODIFIER_WHILE_EMOTION_ACTIVE,PREVENT_PLAYER_COMMANDS }
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1EmotionTemplateOptionsBitFlags { public const int Count=6,BitsInSpan=32,NumSpans=1;public fixed uint Value[NumSpans]; }

// Reborn: the complete 324-byte EP1 record retains inline handles, four full model-condition masks and the Lua event-list string.
[StructLayout(LayoutKind.Sequential)]
public struct Ep1EmotionTemplate
{
    public BaseAssetType Base;
    public Ep1EmotionType Type;
    public Ep1EmotionTemplateOptionsBitFlags Options;
    public Time DurationActive,DurationInactive,DurationInactiveObject,DurationInactiveType;
    public Ep1EmotionTemplateThreatComparisonType EnemyThreatComparison;
    public int EnemyThreat;
    public Ep1EmotionTemplateThreatComparisonType FriendlyThreatComparison;
    public int FriendlyThreat;
    public AssetReference<FXList> StartFX,UpdateFX,EndFX;
    public AssetReference<AttributeModifier> AttributeModifier;
    public Time AttributeModifierStartDelay,AttributeModifierDuration,AIStateLockDuration;
    public Ep1EmotionAIType AIState;
    public ModelConditionBitFlags ModelConditionsSetOnStart,ModelConditionsSetOnEnd,ModelConditionsClearOnStart,ModelConditionsClearOnEnd;
    public AnsiString AILuaEventsList;
}
