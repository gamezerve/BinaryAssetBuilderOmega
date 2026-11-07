using System.Runtime.InteropServices;
using Relo;

namespace SageBinaryData;

// Reborn: unit intros use a weak video-sequence identity and one real audio-event import.
[StructLayout(LayoutKind.Sequential)]
public struct Ep1UnitIntro
{
    public BaseAssetType Base;
    public TypedAssetId<BaseAssetType> Video;
    public AssetReference<BaseAudioEventInfo,AudioEventInfo> Audio;
}

// Reborn: Uprising adds the fifth invisibility option without changing the one-word native mask width.
public enum Ep1InvisibilityOptions { ALLOW_NEAR_TREES,DETECTED_BY_FRIENDLIES,DISCONTINUE_WHEN_REVEALED,UNTOGGLE_HIDDEN_WHEN_LEAVING_STEALTH,CAN_ATTACK_WHILE_ACTIVE }
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1InvisibilityOptionsBitFlags
{
    public const int Count=5,BitsInSpan=32,NumSpans=1;
    public fixed uint Value[NumSpans];
}

// Reborn: the original 252-byte template has no legacy ForbiddenWeaponSets word and includes the required-status tail.
[StructLayout(LayoutKind.Sequential)]
public struct Ep1InvisibilityTemplate
{
    public BaseAssetType Base;
    public ModelConditionBitFlags ForbiddenConditions,ForbiddenConditionExceptions;
    public ObjectStatusBitFlags ForbiddenStatus;
    public uint CamouflageLevel;
    public InvisibilityType InvisibilityType;
    public Ep1InvisibilityOptionsBitFlags Options;
    public AssetReference<FXList> EnteringStealthFX,LeavingStealthFX;
    public ObjectStatusBitFlags HintDetectableStates;
    public Time NoStealthForAttackWindow;
    // Reborn: stock StealthGarrison binds RequiredStatus at +212, before the upgrade-list header at +244.
    public ObjectStatusBitFlags RequiredStatus;
    public List<TypedAssetId<UpgradeTemplate>> IgnoreTreeCheckUpgrade;
}
