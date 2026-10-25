using System.ComponentModel.DataAnnotations;
using System.Runtime.InteropServices;
using Relo;
using AnsiString = Relo.String<sbyte>;

namespace SageBinaryData
{
    public enum SpecialPowerTemplateFlag
    {
        NEEDS_TARGET,
        NEEDS_OBJECT_FILTER,
        NO_FORBIDDEN_OBJECTS,
        NO_FORBIDDEN_MODEL_STATES,
        WATER_OK,
        LIMIT_DISTANCE,
        RESPECT_RECHARGE_TIME_DISCOUNT,
        PATHABLE_ONLY,
        IS_TAINTABLE,
        NOT_CLIFF_CELL,
        FOGGED_SHROUDED_CELLS_OK,
        KILL_SPECIAL_OBJECTS,
        TARGET_NEEDS_OBJECT_STATUS,
        SHARED_SYNC,
        HAS_PUBLIC_TIMER,
        IS_PLAYER_POWER,
        FIND_REACHABLE_TARGETS,
        IGNORE_MAP_EXTENTS,
        IGNORE_SELF_IN_NEEDED_OBJECT_FILTER,
        NOT_ON_BRIDGE,
        NOT_BENEATH_BRIDGE,
        NOT_WHILE_BENEATH_BRIDGE,
        DISABLE_FOR_MULTIPLE_SELECTION,
        CHECK_SPECIALPOWERSTORE_CANUSE,
        USE_CHAINED_COMMAND_MONEY
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct SpecialPowerTemplateBitFlag
    {
        public const int Count = 25;
        public const int BitsInSpan = 32;
        public const int NumSpans = (Count + (BitsInSpan - 1)) / BitsInSpan;

        public unsafe fixed uint Value[NumSpans];
    }

    public enum SpecialPowerTemplateTargetType
    {
        LOCATION,
        OBJECT,
        OBJECT_OR_LOCATION,
        NONE
    }

    public enum SpecialPowerRestrictionType
    {
        UNRESTRICTED,
        [Display(Name = "1")] SPECIALPOWER_RESTRICTION_1,
        [Display(Name = "2")] SPECIALPOWER_RESTRICTION_2
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct SpecialPowerTemplate
    {
        public BaseInheritableAsset Base;
        public AnsiString DisplayName;
        public AnsiString Description;
        public SpecialPowerTemplateTargetType TargetType;
        public SpecialPowerTemplateBitFlag Flags;
        public Time ReloadTime;
        public AssetReference<BaseAudioEventInfo, AudioEventInfo> InitiateSound;
        public AssetReference<BaseAudioEventInfo, AudioEventInfo> InitiateAtLocationSound;
        public Time ViewObjectDuration;
        public float ViewObjectRange;
        public float RadiusCursorRadius;
        public ObjectStatusBitFlags PreventConditions;
        public ObjectStatusBitFlags RequiredConditions;
        public ModelConditionBitFlags DisallowedTargetModelStates;
        public ObjectStatusBitFlags RequiredTargetObjectStatus;
        public ObjectStatusBitFlags DisallowedTargetObjectStatus;
        public float MaxCastRange;
        public float MinCastRange;
        public float ForbiddenObjectRange;
        public AnsiString EvaEventToPlayOnSuccess;
        public AnsiString EvaEventToPlayWhenSelectingTarget;
        public AnsiString EvaEventToPlayOnInitiateOwner;
        public AnsiString EvaEventToPlayOnInitiateAlly;
        public AnsiString EvaEventToPlayOnInitiateEnemy;
        public unsafe StringHash* NameOfVoiceNameToUseAsInitiateIntendToDoVoice;
        public unsafe StringHash* NameOfVoiceNameToUseAsInitiateIntendToDoAfterMoveVoice;
        public unsafe StringHash* NameOfVoiceNameToUseAsInitiateIntendToDoWhileUnderAttackVoice;
        public unsafe StringHash* NameOfVoiceNameToUseAsInitiateIntendToDoAfterMoveWhileUnderAttackVoice;
        public unsafe StringHash* NameOfVoiceNameToUseAsEnterStateInitiateIntendToDoVoice;
        public unsafe StringHash* NameOfVoiceNameToUseAsEnterStateInitiateIntendToDoAfterMoveVoice;
        public unsafe StringHash* NameOfVoiceNameToUseAsEnterStateInitiateIntendToDoWhileUnderAttackVoice;
        public unsafe StringHash* NameOfVoiceNameToUseAsEnterStateInitiateIntendToDoAfterMoveWhileUnderAttackVoice;
        public SpecialPowerRestrictionType RestrictionType;
        public int Money;
        public AssetReference<PackedTextureImage> TimerImage;
        public float ReachableTargetsCircleDisplayRadius;
        public ObjectFilter ObjectFilter;
        public ObjectFilter ForbiddenObjectFilter;
        public unsafe GameDependencyType* GameDependency;
        public SageBool WaypointModeTerminal;
    }
    // Reborn: all 32 EP1 flag positions follow the official Uprising schema, without KW's IS_TAINTABLE insertion.
    public enum Ep1SpecialPowerTemplateFlag { NEEDS_TARGET, NEEDS_OBJECT_FILTER, NO_FORBIDDEN_OBJECTS, NO_FORBIDDEN_MODEL_STATES, WATER_OK, LIMIT_DISTANCE, RESPECT_RECHARGE_TIME_DISCOUNT, PATHABLE_ONLY, NOT_CLIFF_CELL, FOGGED_SHROUDED_CELLS_OK, KILL_SPECIAL_OBJECTS, TARGET_NEEDS_OBJECT_STATUS, SHARED_SYNC, HAS_PUBLIC_TIMER, IS_PLAYER_POWER, FIND_REACHABLE_TARGETS, IGNORE_MAP_EXTENTS, IGNORE_SELF_IN_NEEDED_OBJECT_FILTER, NOT_IN_NANOSWARM, NOT_ON_BRIDGE, NOT_BENEATH_BRIDGE, NOT_WHILE_BENEATH_BRIDGE, DISABLE_FOR_MULTIPLE_SELECTION, CHECK_SPECIALPOWERSTORE_CANUSE, USE_CHAINED_COMMAND_MONEY, CANNOT_LEAVE_ENCLOSURE, DO_NOT_REACQUIRE_TARGET, NOT_ON_OBSTACLES, CAN_NOT_BE_SCRAMBLED, CANNOT_TARGET_SELF, TREAT_BRIDGES_AS_LOCATIONS, PATHABLE_ONLY_NO_LOCOMOTOR_CHECK }
    // Reborn: EP1 adds LOCATION_AND_ANGLE before OBJECT, shifting all later target ordinals.
    public enum Ep1SpecialPowerTemplateTargetType { LOCATION, LOCATION_AND_ANGLE, OBJECT, OBJECT_OR_LOCATION, NONE }
    // Reborn: the full EP1 special-power flag set occupies one native word.
    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct Ep1SpecialPowerTemplateBitFlag { public fixed uint Value[1]; }
    // Reborn: the 584-byte EP1 header includes target count, five EVA selectors and player-tech dependencies.
    [StructLayout(LayoutKind.Sequential)]
    public struct Ep1SpecialPowerTemplate
    {
        public BaseInheritableAsset Base;
        public AnsiString DisplayName;
        public AnsiString Description;
        public Ep1SpecialPowerTemplateTargetType TargetType;
        public int NumTargets;
        public Ep1SpecialPowerTemplateBitFlag Flags;
        public Time ReloadTime;
        public AssetReference<BaseAudioEventInfo, AudioEventInfo> InitiateSound;
        public AssetReference<BaseAudioEventInfo, AudioEventInfo> InitiateAtLocationSound;
        public Time ViewObjectDuration;
        public float ViewObjectRange;
        public float RadiusCursorRadius;
        public ObjectStatusBitFlags PreventConditions;
        public ObjectStatusBitFlags RequiredConditions;
        public ModelConditionBitFlags DisallowedTargetModelStates;
        public ObjectStatusBitFlags RequiredTargetObjectStatus;
        public ObjectStatusBitFlags DisallowedTargetObjectStatus;
        public float MaxCastRange;
        public float MinCastRange;
        public float ForbiddenObjectRange;
        public AssetReference<BaseAssetType> EvaEventToPlayOnSuccess;
        public AssetReference<BaseAssetType> EvaEventToPlayWhenSelectingTarget;
        public AssetReference<BaseAssetType> EvaEventToPlayOnInitiateOwner;
        public AssetReference<BaseAssetType> EvaEventToPlayOnInitiateAlly;
        public AssetReference<BaseAssetType> EvaEventToPlayOnInitiateEnemy;
        public unsafe StringHash* NameOfVoiceNameToUseAsInitiateIntendToDoVoice;
        public unsafe StringHash* NameOfVoiceNameToUseAsInitiateIntendToDoAfterMoveVoice;
        public unsafe StringHash* NameOfVoiceNameToUseAsInitiateIntendToDoWhileUnderAttackVoice;
        public unsafe StringHash* NameOfVoiceNameToUseAsInitiateIntendToDoAfterMoveWhileUnderAttackVoice;
        public unsafe StringHash* NameOfVoiceNameToUseAsEnterStateInitiateIntendToDoVoice;
        public unsafe StringHash* NameOfVoiceNameToUseAsEnterStateInitiateIntendToDoAfterMoveVoice;
        public unsafe StringHash* NameOfVoiceNameToUseAsEnterStateInitiateIntendToDoWhileUnderAttackVoice;
        public unsafe StringHash* NameOfVoiceNameToUseAsEnterStateInitiateIntendToDoAfterMoveWhileUnderAttackVoice;
        public SpecialPowerRestrictionType RestrictionType;
        public int Money;
        public AssetReference<PackedTextureImage> TimerImage;
        public float ReachableTargetsCircleDisplayRadius;
        public AssetReference<BaseAssetType> RequiredPlayerTech;
        public ObjectFilter ObjectFilter;
        public ObjectFilter ForbiddenObjectFilter;
        public unsafe GameDependencyType* GameDependency;
        public List<AssetReference<BaseAssetType>> ForbiddenPlayerTech;
        public SageBool WaypointModeTerminal;
    }
}
