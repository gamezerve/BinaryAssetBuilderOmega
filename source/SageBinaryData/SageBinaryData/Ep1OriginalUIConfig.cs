using Relo;
using System.Runtime.InteropServices;
using AnsiString=Relo.String<sbyte>;

namespace SageBinaryData;

// Reborn: all original UI component classes share their real wire identity and required source priority.
[StructLayout(LayoutKind.Sequential)]
public struct Ep1CoverageUIComponentBase : IPolymorphic { public uint TypeId; public int Priority; }

// Reborn: preserve original UI configuration enum ordinals and explicit aliases.
public enum Ep1StockUIConfigCareerStatType { Int=0, Float=1, Time=2 }

// Reborn: preserve original UI configuration enum ordinals and explicit aliases.
public enum Ep1StockUIConfigCareerStatOrder { CurrentWinStreak=0, CurrentLossStreak, LongestWinStreak, LongestLosingStreak, TotalMatchesPlayed, CareerLosses, CareerWins, WinLossRatio, AvgGameLength, TotalTimePlayed, StructuresBuilt, StructuresLost, StructuresDestroyed, StructuresCaptured, StructuresCapturedByEnemy, CaptureRatio, SuperWeaponsBuilt, BridgesDestroyed, StructuresSold, UnitsBuilt, UnitsLost, UnitsKilled, UnitKillLostRatio, AvgIncomePerMinute, SecondsInLowPower, ResourcesGathered, ResourcesSpent, FormationPreviewUsed, EfficiencyRatio, ValueOfUnitsLost, ValueOfUnitsKilled, FriendlyFireIncidents, ActivatedUnitAbilities, ActivatedCommanderPowers, StructuresInfiltrated }

// Reborn: preserve original UI configuration enum ordinals and explicit aliases.
public enum Ep1StockUIConfigJoyPadButtonType { LEFT_STICK, LEFT_TRIGGER, LEFT_BUMPER, RIGHT_STICK, RIGHT_TRIGGER, RIGHT_BUMPER, A, B, X, Y, D_PAD, BACK, START, GUIDE }

// Reborn: preserve original UI configuration enum ordinals and explicit aliases.
public enum Ep1StockUIConfigJoyPadActionType { PRESS, HOLD, CLICK, DOUBLE_CLICK, ON_GROUND, ON_TARGET, ON_UNIT_TYPE, MOVE, LEFT, RIGHT, UP, DOWN, LEFT_OR_RIGHT, UP_OR_DOWN, LINK_WITH_NEXT }

// Reborn: preserve original UI configuration enum ordinals and explicit aliases.
public enum Ep1StockUIConfigPostGameStatType { SessionLength=0, StructuresCreated, StructuresLost, StructuresDestroyed, StructuresCaptured, StructuresSold, UnitsCreated, UnitsLost, UnitsKilled, UnitKillDeathRatio, AvgIncomePerMinute, AvgIncomePerSecond, ResourcesGathered, EfficiencyRatio, EnemyStructuresInfiltrated, UnitAbilitiesUsed, CommanderPowersUsed }

// Reborn: original AudioSettings configuration layout is independently compared with its retail source/native record.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1StockUIConfigAudioSettings
{
    public BaseAssetType Base;
    public int StreamBufferSizePerChannel;
    public int MaxRequestsPerStream;
    public Time ForceResetTime;
    public Time EmergencyResetTime;
    public Percentage MinOcclusion;
    public Time DelayPriorToPlayingToReadSoundFile;
    public Time ReadAheadTime;
    public Time QueueAheadTime;
    public Time LongTime;
    public uint AudioFootprintInBytes;
    public uint DontLoadThirdVariationWhenFileReadQueueIsLongerThan;
    public uint DontLoadSecondVariationWhenFileReadQueueIsLongerThan;
    public uint DontLoadForLowPrioritySoundsWhenFileReadQueueIsLongerThan;
    public AudioPriority WhatIsLowPriorityForDontLoadForLowPrioritySoundsWhenFileReadQueueIsLongerThan;
    public float VolumeToGainConversionFactor;
    public Percentage DefaultSoundVolume;
    public Percentage DefaultVoiceVolume;
    public Percentage DefaultMusicVolume;
    public Percentage DefaultMovieVolume;
    public Percentage DefaultAmbientVolume;
    public Time AutomaticSubtitleDuration;
    public int AutomaticSubtitleLines;
    public float PanRadiusScaleValue;
    public float PanSize;
    public Percentage MinSampleVolume;
    public float PositionDeltaForReverbCheck;
    public Time TimeToFadeAudio;
    public int AmbientStreamHysteresisVolume;
    public Percentage VolumeMultiplierFor2DSounds;
    public Percentage VolumeMultiplierFor3DSounds;
    public uint VoiceMoveToCampMaxCampnessAtStartPoint;
    public uint VoiceMoveToCampMinCampnessAtEndPoint;
    public Time MinDelayBetweenEnterStateVoice;
    public Time TimeSinceLastAttackForVoiceRetreatToCastle;
    public float DistanceToLookForEnemiesForVoiceRetreatToCastle;
    public Time PostPostGameMusicWait;
    public int CapturedUnitsVoicePriorityOffset;
    public int UISelectionVoicePriorityOffset;
    public Time AddToSelectionVoiceTimeout;
    public Percentage MinVolumeBeforeFade;
    public Percentage MaxVolumeBeforeFade;
    public Time PathMusicBufferTime;
    public int PathMusicLatencyMilisec;
    public RGBAColor AutomaticSubtitleWindowColor;
    public RGBAColor AutomaticSubtitleTextColor;
    public List<Ep1StockUIConfigAudioVolumeSliderMultiplier> VolumeSliderMultiplierForMovie;
    public Ep1StockUIConfigMicrophoneSettings TacticalMicSettings;
    public Ep1StockUIConfigMicrophoneSettings LivingWorldMicSettings;
    public Ep1StockUIConfigMicrophoneSettings ShellMicSettings;
    public Ep1StockUIConfigVolumeCompressionSettings VolumeCompressorSettings;
    public SageBool SuppressOcclusion;
    public SageBool PlayEnemySightedEventsOnlyFromNearbyUnits;
}

// Reborn: original AudioVolumeSliderMultiplier configuration layout is independently compared with its retail source/native record.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1StockUIConfigAudioVolumeSliderMultiplier
{
    public AudioVolumeSlider Slider;
    public float Multiplier;
}

// Reborn: original MicrophoneSettings configuration layout is independently compared with its retail source/native record.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1StockUIConfigMicrophoneSettings
{
    public BaseAssetType Base;
    public Percentage MicrophonePreferredFractionCameraToGround;
    public Percentage VolumeMicrophonePullTowardsTerrainLookAtPoint;
    public Percentage PanningMicrophonePullTowardsTerrainLookAtPoint;
    public float MicrophoneMinDistanceToCamera;
    public float MicrophoneMaxDistanceToCamera;
    public float ZoomMinDistance;
    public float ZoomMaxDistance;
    public Percentage ZoomSoundVolumePercentageAmount;
    public float ZoomFadeDistanceForMaxEffect;
    public float ZoomFadeZeroEffectEdgeLength;
    public float ZoomFadeFullEffectEdgeLength;
}

// Reborn: original VolumeCompressionSettings configuration layout is independently compared with its retail source/native record.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1StockUIConfigVolumeCompressionSettings
{
    public float Threshold;
    public float Ratio;
    public Time AttackTime;
    public Time ReleaseTime;
    public SageBool AppliesEquallyToAllChannels;
}

// Reborn: original UIConfigList configuration layout is independently compared with its retail source/native record.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1StockUIConfigUIConfigList
{
    public BaseAssetType Base;
    public List<Ep1StockUIConfigUIDynamicConfig> UIDynamicConfig;
}

// Reborn: original UIDynamicConfig configuration layout is independently compared with its retail source/native record.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1StockUIConfigUIDynamicConfig
{
    public StringHash Name;
    public UIConfigType Type;
    public PolymorphicList<Ep1CoverageUIComponentBase> ComponentList;
}

// Reborn: original UIComponentAptJoypad configuration layout is independently compared with its retail source/native record.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1StockUIConfigUIComponentAptJoypad
{
    public Ep1CoverageUIComponentBase Base;
    public SageBool RelayLeftStickPosition;
    public SageBool RelayRightStickPosition;
}

// Reborn: original UIComponentAptMediator configuration layout is independently compared with its retail source/native record.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1StockUIConfigUIComponentAptMediator
{
    public Ep1CoverageUIComponentBase Base;
    public AnsiString AptGlobalLoadVarName;
    public AnsiString AptEventHandlerName;
    public AnsiString AptFSCommandHandlerName;
    public AnsiString AptTokenDelimiters;
    public AnsiString NewSkoolIndicator;
    public AnsiString FunctionSeparator;
    public AnsiString ValueSeparator;
    public AnsiString InputParameterSeparator;
    public AnsiString ParameterNameSeparator;
    public AnsiString ParameterStartIndicator;
    public AnsiString SubObjectSeparator;
    public AnsiString OutputParameterSeparator;
}

// Reborn: original UIComponentAutoMatch configuration layout is independently compared with its retail source/native record.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1StockUIConfigUIComponentAutoMatch
{
    public Ep1CoverageUIComponentBase Base;
}

// Reborn: original UIComponentBootup configuration layout is independently compared with its retail source/native record.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1StockUIConfigUIComponentBootup
{
    public Ep1CoverageUIComponentBase Base;
    public AnsiString RenderImageName;
    public AnsiString LoadingTextImageName;
}

// Reborn: original UIComponentCampaign configuration layout is independently compared with its retail source/native record.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1StockUIConfigUIComponentCampaign
{
    public Ep1CoverageUIComponentBase Base;
}

// Reborn: original UIComponentCareerStats configuration layout is independently compared with its retail source/native record.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1StockUIConfigUIComponentCareerStats
{
    public Ep1CoverageUIComponentBase Base;
    public AssetReference<BaseAssetType> DefaultRankImage;
    public List<Ep1StockUIConfigCareerStatData> CareerStat;
}

// Reborn: original CareerStatData configuration layout is independently compared with its retail source/native record.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1StockUIConfigCareerStatData
{
    public Ep1StockUIConfigCareerStatType StatType;
    public AnsiString StatLabel;
    public Ep1StockUIConfigCareerStatOrder RatioNumerator;
    public Ep1StockUIConfigCareerStatOrder RatioDenominator;
    public SageBool IsRatio;
    public SageBool NoSummary;
}

// Reborn: original UIComponentChat configuration layout is independently compared with its retail source/native record.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1StockUIConfigUIComponentChat
{
    public Ep1CoverageUIComponentBase Base;
    public AnsiString ChatTextLocalLabelPrefix;
    public AnsiString ChatTextGlobalLabelPrefix;
}

// Reborn: original UIComponentCheats configuration layout is independently compared with its retail source/native record.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1StockUIConfigUIComponentCheats
{
    public Ep1CoverageUIComponentBase Base;
}

// Reborn: original UIComponentComlinkNotification configuration layout is independently compared with its retail source/native record.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1StockUIConfigUIComponentComlinkNotification
{
    public Ep1CoverageUIComponentBase Base;
    public AnsiString MessageTokenSpec;
}

// Reborn: original UIComponentCommandBar configuration layout is independently compared with its retail source/native record.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1StockUIConfigUIComponentCommandBar
{
    public Ep1CoverageUIComponentBase Base;
    public AnsiString HomeButtonImageName;
    public AnsiString ButtonSlotImageBaseName;
    public AnsiString GeneralButtonImageBaseName;
    public AnsiString RenderClockName;
    public float ClockScale;
    public TypedAssetId<BaseAssetType> ClockImage;
    public int UnitCapFlashSeconds;
    public int UnitCapFinalWarningAmount;
    public int UnitCapInitialWarningAmount;
    public AnsiString UnitCapNoMoreCP;
    public AnsiString UnitCapAlmostNoMoreCP;
    public AnsiString ScrollPowersSoundName;
    public AnsiString OpenUISoundName;
    public AnsiString CloseUISoundName;
    public AnsiString HighlightButtonSoundName;
    public TypedAssetId<BaseAssetType>* HighlightButtonForceFeedback;
    public TypedAssetId<BaseAssetType>* TransitionForceFeedback;
    public AnsiString SelectButtonSoundName;
    public AnsiString ZoomInSoundName;
    public AnsiString ZoomOutSoundName;
    public AnsiString CycleSoundName;
    public AnsiString AptTokenTitle;
    public AnsiString AptTokenCost;
    public AnsiString AptTokenTime;
    public AnsiString AptTokenEnergy;
    public Color ClockColor;
}

// Reborn: original UIComponentCommandList configuration layout is independently compared with its retail source/native record.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1StockUIConfigUIComponentCommandList
{
    public Ep1CoverageUIComponentBase Base;
    public AnsiString AptTokenLeftTrigger;
    public AnsiString AptTokenRightTrigger;
    public AnsiString AptTokenAButton;
    public AnsiString AptTokenBButton;
    public AnsiString AptTokenXButton;
    public AnsiString AptTokenYButton;
}

// Reborn: original UIComponentCoopCommander configuration layout is independently compared with its retail source/native record.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1StockUIConfigUIComponentCoopCommander
{
    public Ep1CoverageUIComponentBase Base;
    public AnsiString AssaultButtonName;
    public AnsiString AssaultButtonDesc;
    public TypedAssetId<BaseAssetType> AssaultButtonCommandBarButtonTemplate;
    public AnsiString NormalButtonName;
    public AnsiString NormalButtonDesc;
    public TypedAssetId<BaseAssetType> NormalButtonCommandBarButtonTemplate;
    public AnsiString HoldPosButtonName;
    public AnsiString HoldPosButtonDesc;
    public TypedAssetId<BaseAssetType> HoldPosButtonCommandBarButtonTemplate;
    public AnsiString HoldObjButtonName;
    public AnsiString HoldObjButtonDesc;
    public TypedAssetId<BaseAssetType> HoldObjButtonCommandBarButtonTemplate;
}

// Reborn: original UIComponentCredits configuration layout is independently compared with its retail source/native record.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1StockUIConfigUIComponentCredits
{
    public Ep1CoverageUIComponentBase Base;
}

// Reborn: original UIComponentCursor configuration layout is independently compared with its retail source/native record.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1StockUIConfigUIComponentCursor
{
    public Ep1CoverageUIComponentBase Base;
    public StringHash AptEventCursorChanged;
    public TypedAssetId<BaseAssetType>* HoverSelectForceFeedback;
}

// Reborn: original UIComponentDisconnect configuration layout is independently compared with its retail source/native record.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1StockUIConfigUIComponentDisconnect
{
    public Ep1CoverageUIComponentBase Base;
}

// Reborn: original UIComponentFormation configuration layout is independently compared with its retail source/native record.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1StockUIConfigUIComponentFormation
{
    public Ep1CoverageUIComponentBase Base;
    public float MaxDragLength;
}

// Reborn: original UIComponentGameInfo configuration layout is independently compared with its retail source/native record.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1StockUIConfigUIComponentGameInfo
{
    public Ep1CoverageUIComponentBase Base;
    public AnsiString GameScoreDisplayToken;
    public AnsiString ScorePlayerToken;
    public AnsiString FactionLabelToken;
    public int GameScoreXLocWS;
    public int GameScoreXLoc;
    public int GameScoreYLocWS;
    public int GameScoreYLoc;
    public int Player1ScoreXLocWS;
    public int Player1ScoreXLoc;
    public int Player1ScoreYLocWS;
    public int Player1ScoreYLoc;
    public int Player2ScoreXLocWS;
    public int Player2ScoreXLoc;
    public int Player2ScoreYLocWS;
    public int Player2ScoreYLoc;
    public int Player3ScoreXLocWS;
    public int Player3ScoreXLoc;
    public int Player3ScoreYLocWS;
    public int Player3ScoreYLoc;
    public int Player4ScoreXLocWS;
    public int Player4ScoreXLoc;
    public int Player4ScoreYLocWS;
    public int Player4ScoreYLoc;
    public List<AnsiString> FactionValue;
}

// Reborn: original UIComponentGameSetup configuration layout is independently compared with its retail source/native record.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1StockUIConfigUIComponentGameSetup
{
    public Ep1CoverageUIComponentBase Base;
}

// Reborn: original UIComponentGameViewJoypad configuration layout is independently compared with its retail source/native record.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1StockUIConfigUIComponentGameViewJoypad
{
    public Ep1CoverageUIComponentBase Base;
    public float LeftStickMultiplier;
    public float RightStickMultiplier;
    public float ObjectAttractivityForce;
    public float MinAttractDistance;
    public float MinAttractDistanceZoomed;
    public float MaxAttractiveForce;
    public KindOfBitFlags KindOfExemptFromAttraction;
    public float ScrollSpeedMin;
    public float ScrollSpeedMax;
    public float ScrollSpeedMaxCutoff;
    public float ScrollSpeedMinCutoff;
    public float InternalZoomInValue;
    public float InternalZoomOutValue;
    public float RapidScrollMultiplier;
    public float MagnetismTuning_LOW_Strength;
    public float MagnetismTuning_LOW_Distance;
    public float MagnetismTuning_HIGH_Strength;
    public float MagnetismTuning_HIGH_Distance;
    public ObjectFilter* MagnetismObjectFilter;
}

// Reborn: original UIComponentInGameShowHide configuration layout is independently compared with its retail source/native record.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1StockUIConfigUIComponentInGameShowHide
{
    public Ep1CoverageUIComponentBase Base;
}

// Reborn: original UIComponentInGameText configuration layout is independently compared with its retail source/native record.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1StockUIConfigUIComponentInGameText
{
    public Ep1CoverageUIComponentBase Base;
    public float AdjustFactor;
    public float TextIndent;
    public uint TooltipAppearDelayMS;
    public uint TooltipDisappearDelayMS;
    public AnsiString AptTokenTitle;
    public AnsiString AptTokenPrereq;
    public AnsiString AptTokenCost;
    public AnsiString AptTokenTime;
    public AnsiString AptTokenEnergy;
    public AnsiString AptTokenShortDesc;
    public AnsiString AptTokenLongDesc;
    public AnsiString ProductionTextConstruction;
    public AnsiString ProductionTextUpgrade;
    public AnsiString ProductionTextRecruit;
    public float ProductionTextScale;
    public AnsiString SubtitleStringLabelPrefix;
    public AnsiString SubtitleStringLabelSuffix;
    public AnsiString SubtitleStringExclusionChar;
    public Vector2 StatusTextPosition;
    public RGBColor ProductionTextColor;
    public FontDesc ProductionTextFont;
}

// Reborn: original UIComponentInputBridge configuration layout is independently compared with its retail source/native record.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1StockUIConfigUIComponentInputBridge
{
    public Ep1CoverageUIComponentBase Base;
}

// Reborn: original UIComponentJoypadTactical configuration layout is independently compared with its retail source/native record.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1StockUIConfigUIComponentJoypadTactical
{
    public Ep1CoverageUIComponentBase Base;
}

// Reborn: original UIComponentKeyboard configuration layout is independently compared with its retail source/native record.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1StockUIConfigUIComponentKeyboard
{
    public Ep1CoverageUIComponentBase Base;
    public AnsiString EditableTextLabelPrefix;
}

// Reborn: original UIComponentLanLobby configuration layout is independently compared with its retail source/native record.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1StockUIConfigUIComponentLanLobby
{
    public Ep1CoverageUIComponentBase Base;
    public uint PlayerlistMinUpdateFreq;
    public uint GamelistMinUpdateFreq;
    public AnsiString PlayerNameAptTokenSpec;
    public AnsiString GameNameAptTokenSpec;
    public AnsiString LocalPlayerNameAptTokenSpec;
}

// Reborn: original UIComponentLoadScreen configuration layout is independently compared with its retail source/native record.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1StockUIConfigUIComponentLoadScreen
{
    public Ep1CoverageUIComponentBase Base;
}

// Reborn: original UIComponentLobby configuration layout is independently compared with its retail source/native record.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1StockUIConfigUIComponentLobby
{
    public Ep1CoverageUIComponentBase Base;
    public AnsiString RandomStringLabel;
    public AnsiString FactionLabelSpec;
    public AnsiString AIPersonalityLabelSpec;
    public AnsiString AIDifficultyLabelSpec;
    public AnsiString TeamLabelSpec;
    public AnsiString MapListSpec;
    public AnsiString GameplayTypeSpec;
    public AnsiString GameTimeLimitSpec;
    public AnsiString GameResourcesSpec;
    public AnsiString HillTimeSpec;
    public AnsiString CapturePointsSpec;
    public AnsiString BarrierTimeSpec;
    public AnsiString CaptureFlagsSpec;
    public List<FactionType> Faction;
    public List<AIDifficultySetting> AIDifficulty;
    public List<TeamSetting> Team;
    public List<GameplayTypeSetting> GameType;
    public List<GameTimeSetting> GameTime;
    public List<GameResourcesSetting> GameResources;
    public List<GameTimeSetting> HillTime;
    public List<GameTimeSetting> BarrierTime;
    public List<GamePointsSetting> CapturePoints;
    public List<GameFlagsSetting> CaptureFlags;
    public MultiplayerLobbyData MultiplayerLobbySettings;
    public SkirmishLobbyData SkirmishLobbySettings;
}

// Reborn: original UIComponentMapList configuration layout is independently compared with its retail source/native record.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1StockUIConfigUIComponentMapList
{
    public Ep1CoverageUIComponentBase Base;
    public AnsiString MapListLabelPrefix;
}

// Reborn: original UIComponentMessage configuration layout is independently compared with its retail source/native record.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1StockUIConfigUIComponentMessage
{
    public Ep1CoverageUIComponentBase Base;
}

// Reborn: original UIComponentMode configuration layout is independently compared with its retail source/native record.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1StockUIConfigUIComponentMode
{
    public Ep1CoverageUIComponentBase Base;
}

// Reborn: original UIComponentMouse configuration layout is independently compared with its retail source/native record.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1StockUIConfigUIComponentMouse
{
    public Ep1CoverageUIComponentBase Base;
}

// Reborn: original UIComponentMouseInGame configuration layout is independently compared with its retail source/native record.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1StockUIConfigUIComponentMouseInGame
{
    public Ep1CoverageUIComponentBase Base;
}

// Reborn: original UIComponentMouseTactical configuration layout is independently compared with its retail source/native record.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1StockUIConfigUIComponentMouseTactical
{
    public Ep1CoverageUIComponentBase Base;
}

// Reborn: original UIComponentMovie configuration layout is independently compared with its retail source/native record.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1StockUIConfigUIComponentMovie
{
    public Ep1CoverageUIComponentBase Base;
    public AnsiString MovieComponentName;
    public int MaxNumberOfStream;
}

// Reborn: original UIComponentMovieArchive configuration layout is independently compared with its retail source/native record.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1StockUIConfigUIComponentMovieArchive
{
    public Ep1CoverageUIComponentBase Base;
    public List<GeneralArchiveMovie> GeneralMovie;
    public List<ScenarioArchiveMovie> ScenarioMovie;
    public List<CampaignArchiveMovie> CampaignMovie;
}

// Reborn: original UIComponentObjectAction configuration layout is independently compared with its retail source/native record.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1StockUIConfigUIComponentObjectAction
{
    public Ep1CoverageUIComponentBase Base;
}

// Reborn: original UIComponentObjectSelector configuration layout is independently compared with its retail source/native record.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1StockUIConfigUIComponentObjectSelector
{
    public Ep1CoverageUIComponentBase Base;
    public AnsiString PortraitName;
    public AnsiString UnitAbility;
    public AnsiString StatusTextNothingSelected;
    public AnsiString StatusTextSelectedAcrossMap;
    public AnsiString StatusTextSelectedAcrossScreen;
    public TypedAssetId<BaseAssetType> AllArmyImageGDI;
    public TypedAssetId<BaseAssetType> AllArmyImageNOD;
    public TypedAssetId<BaseAssetType> AllArmyImageAlien;
    public TypedAssetId<BaseAssetType> ClockImage;
    public TypedAssetId<BaseAssetType>* AssaultTargetForceFeedback;
    public Ep1RadiusDecalTemplate AlienHighlightDecal;
    public Ep1RadiusDecalTemplate GDIHighlightDecal;
    public Ep1RadiusDecalTemplate NODHighlightDecal;
    public List<ObjectFilter> ShortcutObjects;
    public List<ObjectFilter> SupportObjects;
    public Color ClockColor;
    public SageBool ShowAllHealthBars;
}

// Reborn: original UIComponentObjectives configuration layout is independently compared with its retail source/native record.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1StockUIConfigUIComponentObjectives
{
    public Ep1CoverageUIComponentBase Base;
    public AnsiString AptStringNewBonusObjective;
    public AnsiString AptStringObjectiveCompleted;
}

// Reborn: original UIComponentOnlineBuddyList configuration layout is independently compared with its retail source/native record.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1StockUIConfigUIComponentOnlineBuddyList
{
    public Ep1CoverageUIComponentBase Base;
    public AnsiString BuddyNameAptTokenSpec;
    public AnsiString BuddyLocAptTokenSpec;
}

// Reborn: original UIComponentOnlineGameList configuration layout is independently compared with its retail source/native record.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1StockUIConfigUIComponentOnlineGameList
{
    public Ep1CoverageUIComponentBase Base;
    public AnsiString GameNameAptTokenSpec;
    public AnsiString GameMapNameAptTokenSpec;
    public int GameListRefreshInterval;
}

// Reborn: original UIComponentOnlineInvite configuration layout is independently compared with its retail source/native record.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1StockUIConfigUIComponentOnlineInvite
{
    public Ep1CoverageUIComponentBase Base;
    public uint MaxNumberLoggedInvites;
    public AnsiString InviteNameAptTokenSpec;
}

// Reborn: original UIComponentOnlineLobbyList configuration layout is independently compared with its retail source/native record.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1StockUIConfigUIComponentOnlineLobbyList
{
    public Ep1CoverageUIComponentBase Base;
    public AnsiString LobbyNameAptTokenSpec;
}

// Reborn: original UIComponentOnlineLogin configuration layout is independently compared with its retail source/native record.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1StockUIConfigUIComponentOnlineLogin
{
    public Ep1CoverageUIComponentBase Base;
}

// Reborn: original UIComponentOnlinePlayerList configuration layout is independently compared with its retail source/native record.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1StockUIConfigUIComponentOnlinePlayerList
{
    public Ep1CoverageUIComponentBase Base;
    public AnsiString PlayerNameAptTokenSpec;
}

// Reborn: original UIComponentOptions configuration layout is independently compared with its retail source/native record.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1StockUIConfigUIComponentOptions
{
    public Ep1CoverageUIComponentBase Base;
    public AnsiString ControlsToken;
    public AnsiString SaveFileName;
    public List<AnsiString> ControlsText;
    public Ep1StockUIConfigDefaultValues DefaultValues;
    public List<Ep1StockUIConfigSingleCommandList> CommandList;
}

// Reborn: original DefaultValues configuration layout is independently compared with its retail source/native record.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1StockUIConfigDefaultValues
{
    public int Difficulty;
    public float Gamma;
    public float Brightness;
    public float Contrast;
    public float VolumeMusic;
    public float VolumeFX;
    public float VolumeVoice;
    public float VolumeAmbient;
    public float VolumeMovie;
    public float ScrollSpeed;
    public int ScrollMagnetism;
    public int DefaultFaction;
    public int VisionCamZoom;
    public float ScrollSpeedMin;
    public float ScrollSpeedMax;
    public SageBool ButtonIcon;
    public SageBool ShowCommandList;
    public SageBool HealthBars;
    public SageBool HighlightPads;
    public SageBool ToolTips;
    public SageBool Subtitles;
    public SageBool InvertRotate;
    public SageBool Telemetry;
    public SageBool VisionCamVisible;
}

// Reborn: original SingleCommandList configuration layout is independently compared with its retail source/native record.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1StockUIConfigSingleCommandList
{
    public AnsiString CategoryName;
    public List<Ep1StockUIConfigSingleCommandLine> CommandLine;
}

// Reborn: original SingleCommandLine configuration layout is independently compared with its retail source/native record.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1StockUIConfigSingleCommandLine
{
    public AnsiString Purpose;
    public AnsiString Description;
    public List<Ep1StockUIConfigInputButton> Input;
}

// Reborn: original InputButton configuration layout is independently compared with its retail source/native record.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1StockUIConfigInputButton
{
    public Ep1StockUIConfigJoyPadButtonType ButtonName;
    public Ep1StockUIConfigJoyPadActionType ButtonAction;
}

// Reborn: original UIComponentOptionsSettingsAudio configuration layout is independently compared with its retail source/native record.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1StockUIConfigUIComponentOptionsSettingsAudio
{
    public Ep1CoverageUIComponentBase Base;
    public AnsiString LoadVarQueryReturnToken;
    public SageBool DefaultVoipEnable;
    public SageBool DefaultVoipBoostMicGain;
}

// Reborn: original UIComponentOptionsSettingsGeneral configuration layout is independently compared with its retail source/native record.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1StockUIConfigUIComponentOptionsSettingsGeneral
{
    public Ep1CoverageUIComponentBase Base;
    public AnsiString LoadVarQueryReturnToken;
    public int MinTooltipDelayMS;
    public int MaxTooltipDelayMS;
    public int DefaultTooltipDelayMS;
    public float DefaultMouseSensitivity;
    public SageBool DefaultAlternateMouse;
    public SageBool DefaultShowNews;
    public SageBool DefaultShowAds;
}

// Reborn: original UIComponentOptionsSettingsGraphics configuration layout is independently compared with its retail source/native record.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1StockUIConfigUIComponentOptionsSettingsGraphics
{
    public Ep1CoverageUIComponentBase Base;
    public AnsiString LoadVarQueryReturnToken;
    public float DefaultBrightness;
    public AnsiString PresetsAptTokenSpec;
    public AnsiString PresetsStrLookupSpec;
    public AnsiString ResolutionAptTokenSpec;
    public AnsiString AntialiasingAptTokenSpec;
    public AnsiString AntialiasingOffStrLookupSpec;
    public AnsiString AntialiasingOnStrLookupSpec;
    public AnsiString TerrainDetailAptTokenSpec;
    public AnsiString TerrainDetailStrLookupSpec;
    public AnsiString WaterDetailAptTokenSpec;
    public AnsiString WaterDetailStrLookupSpec;
    public AnsiString ModelDetailAptTokenSpec;
    public AnsiString ModelDetailStrLookupSpec;
    public AnsiString TextureQualityAptTokenSpec;
    public AnsiString TextureQualityStrLookupSpec;
    public AnsiString ShaderDetailAptTokenSpec;
    public AnsiString ShaderDetailStrLookupSpec;
    public AnsiString VFXDetailAptTokenSpec;
    public AnsiString VFXDetailStrLookupSpec;
    public AnsiString ShadowDetailAptTokenSpec;
    public AnsiString ShadowDetailStrLookupSpec;
    public AnsiString AnimationDetailAptTokenSpec;
    public AnsiString AnimationDetailStrLookupSpec;
    public AnsiString DecalDetailAptTokenSpec;
    public AnsiString DecalDetailStrLookupSpec;
}

// Reborn: original UIComponentOptionsSettingsHotkeys configuration layout is independently compared with its retail source/native record.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1StockUIConfigUIComponentOptionsSettingsHotkeys
{
    public Ep1CoverageUIComponentBase Base;
    public AnsiString LoadVarQueryReturnToken;
}

// Reborn: original UIComponentOptionsSettingsNetwork configuration layout is independently compared with its retail source/native record.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1StockUIConfigUIComponentOptionsSettingsNetwork
{
    public Ep1CoverageUIComponentBase Base;
    public AnsiString LoadVarQueryReturnToken;
}

// Reborn: original UIComponentPause configuration layout is independently compared with its retail source/native record.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1StockUIConfigUIComponentPause
{
    public Ep1CoverageUIComponentBase Base;
}

// Reborn: original UIComponentPcSaveLoad configuration layout is independently compared with its retail source/native record.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1StockUIConfigUIComponentPcSaveLoad
{
    public Ep1CoverageUIComponentBase Base;
    public AnsiString MapPrefix;
    public AnsiString DescriptionPrefix;
    public AnsiString TimePrefix;
    public AnsiString DatePrefix;
}

// Reborn: original UIComponentPlasmaInterface configuration layout is independently compared with its retail source/native record.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1StockUIConfigUIComponentPlasmaInterface
{
    public Ep1CoverageUIComponentBase Base;
}

// Reborn: original UIComponentPlayerPowers configuration layout is independently compared with its retail source/native record.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1StockUIConfigUIComponentPlayerPowers
{
    public Ep1CoverageUIComponentBase Base;
    public TypedAssetId<BaseAssetType> ClockImage;
}

// Reborn: original UIComponentPlayerTechStore configuration layout is independently compared with its retail source/native record.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1StockUIConfigUIComponentPlayerTechStore
{
    public Ep1CoverageUIComponentBase Base;
    public AnsiString HighlightButtonSoundName;
    public AnsiString SelectButtonSoundName;
    public AnsiString CancelButtonSoundName;
}

// Reborn: original UIComponentPostGame configuration layout is independently compared with its retail source/native record.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1StockUIConfigUIComponentPostGame
{
    public Ep1CoverageUIComponentBase Base;
    public AssetReference<BaseAssetType> AlliesFactionImage;
    public AssetReference<BaseAssetType> SovietFactionImage;
    public AssetReference<BaseAssetType> JapanFactionImage;
    public AssetReference<BaseAssetType> YurikoFactionImage;
    public AssetReference<BaseAssetType> DefaultImage;
    public List<Ep1StockUIConfigPostGameStatData> PostGameStat;
}

// Reborn: original PostGameStatData configuration layout is independently compared with its retail source/native record.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1StockUIConfigPostGameStatData
{
    public Ep1StockUIConfigPostGameStatType StatType;
    public AnsiString StatLabel;
}

// Reborn: original UIComponentProfileManager configuration layout is independently compared with its retail source/native record.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1StockUIConfigUIComponentProfileManager
{
    public Ep1CoverageUIComponentBase Base;
    public AnsiString ProfileListLabelPrefix;
}

// Reborn: original UIComponentRadar configuration layout is independently compared with its retail source/native record.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1StockUIConfigUIComponentRadar
{
    public Ep1CoverageUIComponentBase Base;
    public int RadarWidth;
    public int RadarHeight;
    public float OrientationArrowSize;
    public TypedAssetId<BaseAssetType> OrientationArrowImage;
    public AnsiString StatusTextInfiltration;
    public float LeftStickMultiplier;
    public float RightStickMultiplier;
    public int LeftTriggerTolerance;
}

// Reborn: original UIComponentSUDs configuration layout is independently compared with its retail source/native record.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1StockUIConfigUIComponentSUDs
{
    public Ep1CoverageUIComponentBase Base;
    public AssetReference<BaseAssetType>* TechPointAwardedSound;
}

// Reborn: original UIComponentSaveLoad configuration layout is independently compared with its retail source/native record.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1StockUIConfigUIComponentSaveLoad
{
    public Ep1CoverageUIComponentBase Base;
    public AnsiString FileToken;
}

// Reborn: original UIComponentScenario configuration layout is independently compared with its retail source/native record.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1StockUIConfigUIComponentScenario
{
    public Ep1CoverageUIComponentBase Base;
}

// Reborn: original UIComponentShell configuration layout is independently compared with its retail source/native record.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1StockUIConfigUIComponentShell
{
    public Ep1CoverageUIComponentBase Base;
    public AnsiString LoadMusic;
    public AnsiString LogoRenderImage;
    public AssetReference<BaseAssetType> LogoTexture;
}

// Reborn: original UIComponentShellMultiplayer configuration layout is independently compared with its retail source/native record.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1StockUIConfigUIComponentShellMultiplayer
{
    public Ep1CoverageUIComponentBase Base;
    public Ep1StockUIConfigMapListType OfficialMapList;
    public AnsiString MatchOptionsRankedLabelSpec;
    public AnsiString MatchOptionsMatchTypeLabelSpec;
    public AnsiString MatchOptionsPlayersLabelSpec;
    public AnsiString AnyOptionLabelSpec;
    public AnsiString TACLineLabelSpec;
    public List<GameplayTypeSetting> GameType;
    public List<RankedOptionSetting> RankedOption;
}

// Reborn: original MapListType configuration layout is independently compared with its retail source/native record.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1StockUIConfigMapListType
{
    public List<TypedAssetId<BaseAssetType>> Map;
}

// Reborn: original UIComponentShellVisibility configuration layout is independently compared with its retail source/native record.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1StockUIConfigUIComponentShellVisibility
{
    public Ep1CoverageUIComponentBase Base;
    public AssetReference<BaseAssetType> MainMenuPersonalityGroup;
}

// Reborn: original UIComponentSound configuration layout is independently compared with its retail source/native record.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1StockUIConfigUIComponentSound
{
    public Ep1CoverageUIComponentBase Base;
}

// Reborn: original UIComponentSpecialPowers configuration layout is independently compared with its retail source/native record.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1StockUIConfigUIComponentSpecialPowers
{
    public Ep1CoverageUIComponentBase Base;
    public TypedAssetId<BaseAssetType> PowerTimerImage;
    public AnsiString PowerTimerFlashImageName;
}

// Reborn: original UIComponentStats configuration layout is independently compared with its retail source/native record.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1StockUIConfigUIComponentStats
{
    public Ep1CoverageUIComponentBase Base;
    public AnsiString PlayerToken;
    public AnsiString TitleToken;
    public AnsiString DataToken;
}

// Reborn: original UIComponentTicker configuration layout is independently compared with its retail source/native record.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1StockUIConfigUIComponentTicker
{
    public Ep1CoverageUIComponentBase Base;
    public AnsiString CustomRenderName;
    public int NumberOfTickerDisplays;
    public float ScrollSpeedPixelPerSec;
    public float NumberOfPixelsBetweenHeadline;
    public Color TickerColor;
    public List<UIComponentTickerGroup> TickerGroup;
}

// Reborn: original UIComponentTutorial configuration layout is independently compared with its retail source/native record.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1StockUIConfigUIComponentTutorial
{
    public Ep1CoverageUIComponentBase Base;
}

// Reborn: original UIComponentVictoryDefeat configuration layout is independently compared with its retail source/native record.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1StockUIConfigUIComponentVictoryDefeat
{
    public Ep1CoverageUIComponentBase Base;
    public AnsiString PostEndGameMusic;
    public List<AnsiString> EndGameMusicAlias;
}

// Reborn: original UIComponentVoiceChat configuration layout is independently compared with its retail source/native record.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1StockUIConfigUIComponentVoiceChat
{
    public Ep1CoverageUIComponentBase Base;
    public AssetReference<BaseAssetType> TalkHotKeySlot;
}

