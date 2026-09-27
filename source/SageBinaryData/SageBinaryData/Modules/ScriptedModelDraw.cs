using Relo;
using System.Runtime.InteropServices;
using AnsiString = Relo.String<sbyte>;

namespace SageBinaryData
{
    public enum AnimationMode
    {
        // Reborn: Preserve official RA3 animation-mode ordinals used by scripted model draws.
        MANUAL,
        LOOP,
        ONCE,
        LOOP_BACKWARDS,
        ONCE_BACKWARDS,
        LOOP_PINGPONG,
        PLAY_TO_FRAME,
        MATCH_UNPACKING
    }

    public enum AnimationStateFlag
    {
        // Reborn: Preserve the official RA3 AnimationState flag ordering.
        NONE,
        RANDOMSTART,
        START_FRAME_FIRST,
        START_FRAME_LAST,
        RESTART_ANIM_WHEN_COMPLETE,
        MAINTAIN_FRAME_ACROSS_STATES,
        MAINTAIN_FRAME_ACROSS_STATES2,
        MAINTAIN_FRAME_ACROSS_STATES3,
        MAINTAIN_FRAME_ACROSS_STATES4,
        DO_NOT_PLAY_WHEN_UNPOWERED,
        ADJUST_HEIGHT_BY_CONSTRUCTION_PERCENT,
        IGNORE_MOVEMENT_SPEED
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct AnimationStateBitFlags
    {
        public const int Count = 12;
        public const int BitsInSpan = 32;
        public const int NumSpans = (Count + (BitsInSpan - 1)) / BitsInSpan;

        // Reborn: Store RA3 animation-state flags in the engine's native bit span.
        public unsafe fixed uint Value[NumSpans];
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct ScriptedModelDrawTexture
    {
        public AssetReference<Texture> Texture;
        public AnsiString Object;
        public TimeOfDayType TimeOfDay;
        public int TexturePass;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct Animation
    {
        public AnsiString Flags;
        public AssetReference<W3DAnimation> AnimationName;
        public AnimationMode AnimationMode;
        public AnsiString AnimNickName;
        public float Distance;
        public float AnimationBlendTime;
        public float AnimationSpeedFactorMin;
        public float AnimationSpeedFactorMax;
        // Reborn: RA3 uses absolute animation time instead of Kane's Wrath fade bookkeeping.
        public Time AnimationAbsoluteTime;
        public WeaponSlotType WeaponTimingOrdering;
        public int WeaponTimingSlotID;
        public int AnimationPriority;
        public SageBool AnimationMustCompleteBlend;
        public SageBool UseWeaponTiming;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct AnimationState
    {
        public ParseCondStateType ParseCondStateType;
        public AnsiString AnimNickName;
        public ModelConditionBitFlags ConditionsYes;
        public AnsiString Name;
        public AnsiString StateName;
        public AnimationStateBitFlags Flags;
        public AssetReference<FXList> EnteringStateFX;
        public int FrameForPristineBonePositions;
        public List<Animation> Animation;
        public unsafe AnsiString* Script;
        public List<FXEvent> FXEvent;
        public List<LuaEvent> LuaEvent;
        public List<ParticleSysBone> ParticleSysBone;
        public SageBool ShareAnimation;
        public SageBool AllowRepeatInRandomPick;
        public SageBool SimilarRestart;
    }

    // Reborn: Match the official 216-byte RA3/EP1 Tokenizer layout without Kane's Wrath-only fields.
    [StructLayout(LayoutKind.Sequential)]
    public struct W3DScriptedModelDrawModuleData
    {
        public DrawModuleData Base;
        public AnsiString Name;
        public Velocity InitialRecoilSpeed;
        public float MaxRecoilDistance;
        public float RecoilDamping;
        public Velocity RecoilSettleSpeed;
        public ModelLODType MinLODRequired;
        public WeaponSlotBitFlags ProjectileBoneFeedbackEnabledSlots;
        public AssetReference<Texture> TrackMarks;
        public List<AnsiString> ExtraPublicBone;
        public AnsiString AttachToBoneInAnotherModule;
        public ModelConditionBitFlags DependencySharedModelFlags;
        public AnsiString TrackMarksLeftBone;
        public AnsiString TrackMarksRightBone;
        public float HighDetailLODThreshold;
        public float LowDetailLODThreshold;
        public AssetReference<FXParticleSystemTemplate> WadingParticleSys;
        public float AlphaCameraFadeOuterRadius;
        public float AlphaCameraFadeInnerRadius;
        public Percentage AlphaCameraAtInnerRadius;
        public int BirthFadeTime;
        public List<ModelConditionState> ModelConditionState;
        public List<AnimationState> AnimationState;
        public List<ScriptedModelDrawAttachModel> AttachModel;
        public List<ScriptedModelDrawEmbedPortal> EmbedPortal;
        public SageBool OkToChangeModelColor;
        public SageBool AnimationsRequirePower;
        public SageBool UseYAxisForTurretRotation;
        public SageBool TrackMarksOnlyWhenCorneringQuickly;
        public SageBool UseProducerTexture;
        public SageBool NoRotate;
        public SageBool UseFiringArcRotation;
        public SageBool Selectable;
        public SageBool RandomTextureFixedRandomIndex;
        public SageBool ParticleBonesCheckDrawable;
        public SageBool ShadowForceDisable;
        public SageBool SwitchModelLODMode;
        public SageBool StaticModelLODMode;
        public SageBool ShowShadowWhileContained;
        public SageBool MultiPlayerOnly;
        public SageBool AffectedByStealth;
        public SageBool InvertStealthOpacity;
        public SageBool HighDetailOnly;
    }
}
