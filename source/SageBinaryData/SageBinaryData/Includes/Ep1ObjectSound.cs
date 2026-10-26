using Relo;
using System.Runtime.InteropServices;

namespace SageBinaryData
{
    // Reborn: preserve the exact reviewed official EP1 ThingTemplateVoiceType order without changing the original KW enum.
    public enum Ep1ThingTemplateVoiceType
    {
        voiceSelect = 0,
        voiceSelectUnderConstruction = 1,
        voiceSelectBattle = 2,
        voiceSelectIdleWorker = 3,
        voiceSelectUnderFire = 4,
        voiceMove = 5,
        voiceMoveToHigherGround = 6,
        voiceMoveOverWalls = 7,
        voiceMoveWhileAttacking = 8,
        voiceMoveLandToWater = 9,
        voiceMoveWaterToLand = 10,
        voiceRetreatToCastle = 11,
        voiceMoveToCamp = 12,
        voiceDeliverRing = 13,
        voiceSendToSlaughterhouse = 14,
        voiceCreated = 15,
        voiceFullyCreated = 16,
        voiceAttack = 17,
        voiceAttackCharge = 18,
        voiceAttackStructure = 19,
        voiceAttackGarrisonedStructure = 20,
        voiceAttackMachine = 21,
        voiceAttackAir = 22,
        voiceAttackAfterMoving = 23,
        voiceDesperateAttack = 24,
        voiceBombard = 25,
        voiceFear = 26,
        voiceCombineWithHorde = 27,
        voiceTaskComplete = 28,
        voiceDefectingAway = 29,
        voiceDefectingTowards = 30,
        voiceGuard = 31,
        voiceAlert = 32,
        voiceSalvage = 33,
        voiceSupply = 34,
        voiceDock = 35,
        voiceUnload = 36,
        voiceRepair = 37,
        voiceCombatDrop = 38,
        voiceEnter = 39,
        voiceEnterHostile = 40,
        voiceGetHealed = 41,
        voiceGarrison = 42,
        voiceBuildResponse = 43,
        voicePrimaryWeaponMode = 44,
        voiceSecondaryWeaponMode = 45,
        voiceTertiaryWeaponMode = 46,
        voiceCaptureBuildingComplete = 47,
        voiceRapidFire = 48,
        voiceStartCharging = 49,
        voiceNoBuild = 50,
        voiceLowFuel = 51,
        voiceEnterStateAttack = 52,
        voiceEnterStateAttackCharge = 53,
        voiceEnterStateAttackAir = 54,
        voiceEnterStateAttackStructure = 55,
        voiceEnterStateAttackGarrisonedStructure = 56,
        voiceEnterStateAttackMachine = 57,
        voiceEnterStateAttackAfterMoving = 58,
        voiceEnterStateMove = 59,
        voiceEnterStateMoveToHigherGround = 60,
        voiceEnterStateMoveOverWalls = 61,
        voiceEnterStateRetreatToCastle = 62,
        voiceEnterStateMoveToCamp = 63,
        voiceEnterStateMoveWhileAttacking = 64,
        voiceEnterStateMoveLandToWater = 65,
        voiceEnterStateMoveWaterToLand = 66
    }

    // Reborn: preserve the exact reviewed official EP1 ThingTemplateSoundType order without changing the original KW enum.
    public enum Ep1ThingTemplateSoundType
    {
        soundMoveStart = 0,
        soundMoveStartDamaged = 1,
        soundMoveLoop = 2,
        soundMoveLoopDamaged = 3,
        soundRotateStart = 4,
        soundRotateStartDamaged = 5,
        soundAmbient = 6,
        soundAmbientDamaged = 7,
        soundAmbientReallyDamaged = 8,
        soundAmbientRubble = 9,
        soundAmbientBattle = 10,
        soundStealthOn = 11,
        soundStealthOff = 12,
        soundCreated = 13,
        soundReenabled = 14,
        soundDisabled = 15,
        soundDisabledOnCreation = 16,
        soundOnDamaged = 17,
        soundOnReallyDamaged = 18,
        soundEnter = 19,
        soundExit = 20,
        soundPromotedVeteran = 21,
        soundPromotedElite = 22,
        soundPromotedHero = 23,
        soundFalling = 24,
        soundImpact = 25,
        soundImpactCyclonic = 26,
        soundCrushing = 27,
        soundDeploy = 28,
        soundUndeploy = 29,
        soundUnderConstruction = 30,
        soundUnderRepairFromRubble = 31,
        soundUnderRepairFromDamage = 32,
        soundAfterburner = 33,
        soundDisguiseStarted = 34,
        soundDisguiseRevealedSuccess = 35,
        soundDisguiseRevealedFailure = 36,
        soundTurretMoveLoop = 37,
        soundPrepositioned = 38
    }

    // Reborn: EP1 voice Sound is an optional reference pointee; EvaEvent is an inline strong occurrence, not a legacy string pointer.
    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct Ep1SoundOrEvaEvent { public AssetReference<BaseAudioEventInfo, AudioEventInfo>* Sound; public AssetReference<BaseAssetType> EvaEvent; }
    // Reborn: EP1 voice/sound ordinals differ from KW while the twelve/eight-byte native entry layouts remain unchanged.
    [StructLayout(LayoutKind.Sequential)]
    public struct Ep1AudioVoiceEntry { public Ep1SoundOrEvaEvent Base; public Ep1ThingTemplateVoiceType AudioType; }
    [StructLayout(LayoutKind.Sequential)]
    public struct Ep1AudioSoundEntry { public AssetReference<BaseAudioEventInfo, AudioEventInfo> Sound; public Ep1ThingTemplateSoundType AudioType; }
    // Reborn: EP1 named voices use the EP1 sound/EVA base (Sound pointer + inline typed EvaEvent) before the Name hash; KW's string EvaEvent base differs.
    [StructLayout(LayoutKind.Sequential)]
    public struct Ep1AudioVoiceReferentialEntry { public Ep1SoundOrEvaEvent Base; public StringHash Name; }
    [StructLayout(LayoutKind.Sequential)]
    public struct Ep1AudioArrayVoice { public List<Ep1AudioVoiceEntry> AudioEntry; public List<AudioObjectSpecificVoiceEntry> ObjectSpecificEntry; public List<Ep1AudioVoiceReferentialEntry> NamedEntry; }
    [StructLayout(LayoutKind.Sequential)]
    public struct Ep1AudioArraySound { public List<Ep1AudioSoundEntry> AudioEntry; }
    // Reborn: a separate selected selector graph prevents EP1 enum ordinals from leaking into legacy KW client/root audio dispatch.
    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct Ep1ModelConditionSoundSelectorOverrideBlock { public ModelConditionBitFlags RequiredFlags; public ModelConditionBitFlags ExcludedFlags; public int* VoicePriorityOverride; public Ep1AudioArrayVoice* AudioArrayVoice; public Ep1AudioArraySound* AudioArraySound; }
    [StructLayout(LayoutKind.Sequential)]
    public struct Ep1ModelConditionSoundSelectorClientBehaviorModuleData { public ClientBehaviorModuleData Base; public List<Ep1ModelConditionSoundSelectorOverrideBlock> Override; }
}
