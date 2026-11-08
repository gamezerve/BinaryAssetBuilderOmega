using Relo;
using System.Runtime.InteropServices;
using AnsiString=Relo.String<sbyte>;

namespace SageBinaryData;

// Reborn: original water/lighting records retain INVALID=0 and MORNING=1.
public enum Ep1StockNativeTimeOfDay { INVALID, MORNING, AFTERNOON, EVENING, NIGHT, INTERPOLATE }

// Reborn: keep original WaterTextureType enum values and explicit schema aliases.
public enum Ep1StockWaterTextureType { FIRST=0, RIVER=0, BUMP, NORMAL, SKY, NOISE, SPARKLE }

// Reborn: keep original FontUse enum values and explicit schema aliases.
public enum Ep1StockFontUse { DrawableCaptionFont, DefaultWindowFont, DefaultDisplayStringFont, TooltipFontName, NativeDebugDisplay }

// Reborn: keep original AIBeaconType enum values and explicit schema aliases.
public enum Ep1StockAIBeaconType { Invalid=-1, Assault=0, HoldPosition, HoldObject }

// Reborn: keep original ShroudStatusType enum values and explicit schema aliases.
public enum Ep1StockShroudStatusType { FOGGED=0, CLEAR=1, SHROUDED=3 }

// Reborn: keep original UIMouseTacticalQuickChatDisplayChatDestination enum values and explicit schema aliases.
public enum Ep1StockUIMouseTacticalQuickChatDisplayChatDestination { Allies=0, Everyone }

// Reborn: keep original MouseHUDFixedElementId enum values and explicit schema aliases.
public enum Ep1StockMouseHUDFixedElementId { Invalid=-1, EnergyMeter, Resources, SellButton, RepairButton, PauseButton, Radar, SelectionRefinementTab, MainStructuresTab, OtherStructuresTab, InfantryTab, VehicleTab, AircraftTab, WatercraftTab, AttackMoveButton, ForceAttackButton, ForceMoveButton, ReverseMoveButton, ForceAttackMoveButton, StopButton, PlanningModeButton, WaypointModeButton, AggressiveStanceButton, GuardStanceButton, HoldFireStanceButton, HoldGroundStanceButton, OpenAdvancedCommandsButton, ThreatMeter, OpenPlayerTechStoreButton, CoOpAINormalOrderButton, CoOpAIAssaultOrderButton, CoOpAIHoldObjectOrderButton, CoOpAIHoldPositionOrderButton, DeactivateHotSpotButton, UnitIntroButton, CoOpAIOrderPanel }

// Reborn: original JoypadCommandBarTemplate schema fields require independent source/native qualification before emission.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1StockJoypadCommandBarTemplate
{
    public BaseAssetType Base;
    public AssetReference<BaseAssetType> HomeButton;
    public List<Ep1StockJoypadCommandBarTemplateButtonSlot> Slot;
    public List<AssetReference<BaseAssetType>> GeneralButton;
}

// Reborn: original JoypadCommandBarTemplateButtonSlot schema fields require independent source/native qualification before emission.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1StockJoypadCommandBarTemplateButtonSlot
{
    public uint Ordinal;
    public AssetReference<BaseAssetType> Button;
}

// Reborn: original DamageFX schema fields require independent source/native qualification before emission.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1StockDamageFX
{
    public BaseAssetType Base;
    public List<Ep1StockDamageFXGroup> FXGroup;
}

// Reborn: original DamageFXGroup schema fields require independent source/native qualification before emission.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1StockDamageFXGroup
{
    public DamageFXType Type;
    public float Amount;
    public Time ThrottleTime;
    public AssetReference<BaseAssetType> MajorFX;
    public AssetReference<BaseAssetType> MinorFX;
    public AssetReference<BaseAssetType> RearFX;
    public AssetReference<BaseAssetType> SideFX;
    public AssetReference<BaseAssetType> TopFX;
    public AssetReference<BaseAssetType> VeteranMajorFX;
    public AssetReference<BaseAssetType> VeteranMinorFX;
    public AssetReference<BaseAssetType> VeteranRearFX;
    public AssetReference<BaseAssetType> VeteranSideFX;
    public AssetReference<BaseAssetType> VeteranTopFX;
}

// Reborn: original WaterTextureList schema fields require independent source/native qualification before emission.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1StockWaterTextureList
{
    public BaseAssetType Base;
    public Ep1StockWaterTextureType Type;
    public List<AnsiString> Texture;
}

// Reborn: original StaticGameLOD schema fields require independent source/native qualification before emission.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1StockStaticGameLOD
{
    public BaseAssetType Base;
    public GameLODType Level;
    public ModelLODType ModelLOD;
    public EffectsLODType EffectsLOD;
    public int MaxParticleCount;
    public ShadowLODType ShadowLOD;
    public TerrainLODType TerrainLOD;
    public WaterLODType WaterLOD;
    public int MaxTankTrackEdges;
    public int MaxTankTrackOpaqueEdges;
    public int MaxTankTrackFadeDelay;
    public int TextureReductionFactor;
    public AnimationLODType AnimationLOD;
    public ShaderLODType ShaderLOD;
    public DecalLODType DecalLOD;
    public FXParticleSystem_Priority MinParticlePriority;
    public FXParticleSystem_Priority MinParticleSkipPriority;
    public int AntiAliasingQuality;
    public SageBool UseShadowDecals;
    public SageBool UseShadowMapping;
    public SageBool UseTerrainNormalMap;
    public SageBool ShowProps;
    public SageBool EnableVSync;
}

// Reborn: original WaterSetting schema fields require independent source/native qualification before emission.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1StockWaterSetting
{
    public BaseAssetType Base;
    public Ep1StockNativeTimeOfDay TimeOfDay;
    public Color DiffuseColor;
}

// Reborn: original GameLODPreset schema fields require independent source/native qualification before emission.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1StockGameLODPreset
{
    public BaseAssetType Base;
    public GameLODType Level;
    public int SystemMemory;
    public int Xres;
    public int YRes;
    public List<ProcessorRequirement> Processor;
}

// Reborn: original TerrainParticleMapping schema fields require independent source/native qualification before emission.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1StockTerrainParticleMapping
{
    public BaseInheritableAsset Base;
    public Percentage AccelerationMultiplier;
    public List<Ep1StockTerrainParticleMap> Mapping;
}

// Reborn: original TerrainParticleMap schema fields require independent source/native qualification before emission.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1StockTerrainParticleMap
{
    public TerrainClassType TerrainClass;
    public AssetReference<BaseAssetType> ParticleSystem;
    public ModelConditionBitFlags RequiredModelConditions;
    public ModelConditionBitFlags ForbiddenModelConditions;
}

// Reborn: original FontDefaultSetting schema fields require independent source/native qualification before emission.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1StockFontDefaultSetting
{
    public BaseInheritableAsset Base;
    public AnsiString Name;
    public uint* Size;
    public Ep1StockFontSettings Settings;
}

// Reborn: original FontSettings schema fields require independent source/native qualification before emission.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1StockFontSettings
{
    public SageBool AntiAliased;
}

// Reborn: original ArmyDefinition schema fields require independent source/native qualification before emission.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1StockArmyDefinition
{
    public BaseAssetType Base;
    public AnsiString Side;
    public TypedAssetId<BaseAssetType> MCVTemplate;
    public TypedAssetId<BaseAssetType> GathererTemplate;
    public TypedAssetId<BaseAssetType> ExpanderTemplate;
    public TypedAssetId<BaseAssetType> EnergyProducer;
    public TypedAssetId<BaseAssetType> PrimaryResourceNodeCollector;
    public TypedAssetId<BaseAssetType> PrimaryResourceThingToBuild;
    public TypedAssetId<BaseAssetType> SecondaryResourceNodeCollector;
    public TypedAssetId<BaseAssetType> VehicleRepairFacility;
    public TypedAssetId<BaseAssetType> NavalRepairFacility;
    public TypedAssetId<BaseAssetType> AircraftRepairFacility;
    public TypedAssetId<BaseAssetType> WallPiece;
    public List<TypedAssetId<BaseAssetType>> TechStructure;
}

// Reborn: original DebugHotKeys schema fields require independent source/native qualification before emission.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1StockDebugHotKeys
{
    public BaseInheritableAsset Base;
    public HotKeyMap Map;
}

// Reborn: original DrawGroupInfo schema fields require independent source/native qualification before emission.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1StockDrawGroupInfo
{
    public BaseAssetType Base;
    public AnsiString FontName;
    public uint FontSize;
    public int DropShadowOffsetX;
    public int DropShadowOffsetY;
    public int PixelOffetXInteger;
    public int PixelOffetYInteger;
    public float PixelOffetXReal;
    public float PixelOffetYReal;
    public Color TextColor;
    public Color TextDropShadowColor;
    public SageBool IsBold;
    public SageBool UsePlayerColor;
    public SageBool PixelOffsetAsPercentage;
}

// Reborn: original Language schema fields require independent source/native qualification before emission.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1StockLanguage
{
    public BaseInheritableAsset Base;
    public Ep1StockSeparatorList Separators;
    public AnsiString UnicodeFontName;
    public List<Ep1StockFontDefine> FontInfo;
}

// Reborn: original SeparatorList schema fields require independent source/native qualification before emission.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1StockSeparatorList
{
    public AnsiString Thousand;
    public AnsiString Decimal;
    public AnsiString TimeMinuteToSecond;
}

// Reborn: original FontDefine schema fields require independent source/native qualification before emission.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1StockFontDefine
{
    public Ep1StockFontUse FontTitle;
    public FontDesc Font;
}

// Reborn: original BounceKickTerrainMapping schema fields require independent source/native qualification before emission.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1StockBounceKickTerrainMapping
{
    public BaseInheritableAsset Base;
    public List<Ep1StockBounceKickTerrainData> Mapping;
}

// Reborn: original BounceKickTerrainData schema fields require independent source/native qualification before emission.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1StockBounceKickTerrainData
{
    public TerrainClassType TerrainClass;
    public Percentage BounceMultiplier;
}

// Reborn: original MeshInstanceTemplate schema fields require independent source/native qualification before emission.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1StockMeshInstanceTemplate
{
    public BaseInheritableAsset Base;
    public List<Ep1StockMatchRule> Rule;
}

// Reborn: original MatchRule schema fields require independent source/native qualification before emission.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1StockMatchRule
{
    public AnsiString MeshID;
    public int Instances;
}

// Reborn: original UnitOverlayIconSettings schema fields require independent source/native qualification before emission.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1StockUnitOverlayIconSettings
{
    public BaseInheritableAsset Base;
    public AssetReference<BaseAssetType> AmmoPipImage;
    public AssetReference<BaseAssetType> AmmoPipFrameImage;
    public AssetReference<BaseAssetType> ContainPipImage;
    public AssetReference<BaseAssetType> ContainPipFrameImage;
    public AssetReference<BaseAssetType> GarrisonPipImage;
    public AssetReference<BaseAssetType> UnitTypeIconBackgroundImage;
    public AssetReference<BaseAssetType> TargetIndicatorImage;
    public AssetReference<BaseAssetType> TargetIndicatorGlowImage;
    public AssetReference<BaseAssetType> PowerIconImageSequence;
    public AssetReference<BaseAssetType> RepairIconImageSequence;
    public AssetReference<BaseAssetType> InfiltratedIconImageSequence;
    public AssetReference<BaseAssetType> HealUnitIconImageSequence;
    public AssetReference<BaseAssetType> RepairVehicleIconImageSequence;
    public Percentage AmmoPipScale;
    public int AmmoPipRange;
    public Percentage ContainPipScale;
    public Percentage GarrisonPipScale;
    public Percentage UnitTypeIconScale;
    public Percentage StatusIconScale;
    public Percentage VeterancyIconScale;
    public Percentage TargetIndicatorScale;
    public Percentage HealingIconScale;
    public Time HealingIconDisplayTime;
    public List<FactionVeterancyOverlayIcons> FactionVeterancy;
}

// Reborn: original WaterTransparency schema fields require independent source/native qualification before emission.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1StockWaterTransparency
{
    public BaseAssetType Base;
    public float TransparentWaterDepth;
    public float TransparentWaterMinOpacity;
    public TypedAssetId<BaseAssetType> StandingWaterTexture;
}

// Reborn: original NetworkSettings schema fields require independent source/native qualification before emission.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1StockNetworkSettings
{
    public Ep1StockBaseSingletonSettings Base;
    public int RunAheadSlack;
}

// Reborn: original BaseSingletonSettings schema fields require independent source/native qualification before emission.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1StockBaseSingletonSettings
{
    public BaseAssetType Base;
}

// Reborn: original MiscEvaEvents schema fields require independent source/native qualification before emission.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1StockMiscEvaEvents
{
    public BaseInheritableAsset Base;
    public AssetReference<BaseAssetType> LowPower;
    public AssetReference<BaseAssetType> SilosNeeded;
    public AssetReference<BaseAssetType> BuildingStolen;
    public AssetReference<BaseAssetType> IncomingRadarTransmission;
    public AssetReference<BaseAssetType> AllyDefeated;
    public AssetReference<BaseAssetType> EnemyDefeated;
    public AssetReference<BaseAssetType> LocalPlayerDefeated;
    public AssetReference<BaseAssetType> LocalPlayerVictorious;
    public AssetReference<BaseAssetType> CannotBuildDueToCPLimit;
    public AssetReference<BaseAssetType> BuildQueuePausedDueToCPLimit;
    public AssetReference<BaseAssetType> BuildQueuePausedDueToFunds;
    public AssetReference<BaseAssetType> CannotBuildDueToFunds;
    public AssetReference<BaseAssetType> CannotRepairDueToFunds;
    public AssetReference<BaseAssetType> CannotUsePowerDueToFunds;
    public AssetReference<BaseAssetType> TiberiumFieldDepleted;
    public AssetReference<BaseAssetType> NewMissionObjective;
    public AssetReference<BaseAssetType> CompletedMissionObjective;
    public AssetReference<BaseAssetType> NewBonusObjective;
    public AssetReference<BaseAssetType> CompletedBonusMissionObjective;
    public AssetReference<BaseAssetType> IntelDatabaseEntryReceived;
    public AssetReference<BaseAssetType> UpgradeComplete;
    public AssetReference<BaseAssetType> BuildingBeingStolen;
    public AssetReference<BaseAssetType> BeaconPlacedPlayer;
    public AssetReference<BaseAssetType> BeaconPlacedAlly;
    public AssetReference<BaseAssetType> ObservedPlayerDefeated;
    public AssetReference<BaseAssetType> WallsBeingClimbed;
    public AssetReference<BaseAssetType> StructureInfiltrated;
    public AssetReference<BaseAssetType> ControlPointCaptured;
    public AssetReference<BaseAssetType> ControlPointLost;
    public AssetReference<BaseAssetType> ControlPointHasBeenCaptured;
    public AssetReference<BaseAssetType> CTFFlagCaptured;
    public AssetReference<BaseAssetType> CTFEnemyCapturedFlag;
    public AssetReference<BaseAssetType> CTFFlagDropped;
    public AssetReference<BaseAssetType> CTFEnemyDroppedFlag;
    public AssetReference<BaseAssetType> CTFYouHaveFlag;
    public AssetReference<BaseAssetType> CTFEnemyHasFlag;
    public AssetReference<BaseAssetType> SiegeBarrierDisengaged;
    public AssetReference<BaseAssetType> SiegeBarrierEngaged;
    public List<Ep1StockMiscEvaEventsAIBeaconTypeSettings> AIBeaconTypeSettings;
}

// Reborn: original MiscEvaEventsAIBeaconTypeSettings schema fields require independent source/native qualification before emission.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1StockMiscEvaEventsAIBeaconTypeSettings
{
    public Ep1StockAIBeaconType BeaconType;
    public AssetReference<BaseAssetType> PlacedEvent;
}

// Reborn: original AutoParticleSystemSettings schema fields require independent source/native qualification before emission.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1StockAutoParticleSystemSettings
{
    public Ep1StockBaseSingletonSettings Base;
    public float ParticleScale;
    public int MaxParticleCount;
    public Ep1StockAutoParticleSettings FireSmall;
    public Ep1StockAutoParticleSettings FireMedium;
    public Ep1StockAutoParticleSettings FireLarge;
    public Ep1StockAutoParticleSettings SmokeSmall;
    public Ep1StockAutoParticleSettings SmokeMedium;
    public Ep1StockAutoParticleSettings SmokeLarge;
    public Ep1StockAutoParticleSettings Aflame;
}

// Reborn: original AutoParticleSettings schema fields require independent source/native qualification before emission.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1StockAutoParticleSettings
{
    public AnsiString ParticlePrefix;
    public TypedAssetId<BaseAssetType> ParticleSystemId;
    public int ParticleMax;
}

// Reborn: original BreezeInfo schema fields require independent source/native qualification before emission.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1StockBreezeInfo
{
    public Ep1StockBaseSubsystemSettings Base;
    public float Direction;
    public float Intensity;
    public float Lean;
    public float Randomness;
    public Time BreezePeriod;
}

// Reborn: original BaseSubsystemSettings schema fields require independent source/native qualification before emission.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1StockBaseSubsystemSettings
{
    public BaseAssetType Base;
    public AssetReference<BaseAssetType> LoadAfter;
}

// Reborn: original BuildSystemSettings schema fields require independent source/native qualification before emission.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1StockBuildSystemSettings
{
    public Ep1StockBaseSingletonSettings Base;
    public float AllowedHeightVariationForBuildingPlacement;
    public float RotationalSnapIterations;
    public float BuildabilityDisplayRadius;
    public GeometryType BuildabilityDisplayShape;
    public float BuildabilityDecalMargin;
    public float BuildabilityDecalSize;
    public Percentage BuildabilityDecalRadiusPercent;
    public SageBool BuildabilityUsesStaticDecalSize;
}

// Reborn: original DamageFXSettings schema fields require independent source/native qualification before emission.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1StockDamageFXSettings
{
    public Ep1StockBaseSingletonSettings Base;
    public AssetReference<BaseAssetType>* GroundDamageFX;
}

// Reborn: original DynamicsContactTagMapping schema fields require independent source/native qualification before emission.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1StockDynamicsContactTagMapping
{
    public Ep1StockBaseSingletonSettings Base;
    public List<Ep1StockContactTagMapping> Mapping;
}

// Reborn: original ContactTagMapping schema fields require independent source/native qualification before emission.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1StockContactTagMapping
{
    public DynamicsContactTag ContactA;
    public DynamicsContactTag ContactB;
    public float MinForce;
    public AssetReference<BaseAssetType> FX;
}

// Reborn: original GraphicsHardware schema fields require independent source/native qualification before emission.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1StockGraphicsHardware
{
    public BaseAssetType Base;
    public List<Ep1StockGraphicsDevice> Device;
}

// Reborn: original GraphicsDevice schema fields require independent source/native qualification before emission.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1StockGraphicsDevice
{
    public uint VendorId;
    public uint DeviceId;
    public AnsiString Name;
    public GameLODType Level;
    public GameLODType* MacLevel;
}

// Reborn: original InvisibilityManagerSettings schema fields require independent source/native qualification before emission.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1StockInvisibilityManagerSettings
{
    public Ep1StockBaseSingletonSettings Base;
    public ObjectFilter CamouflageDetectorObjectFilter;
}

// Reborn: original LightingSettings schema fields require independent source/native qualification before emission.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1StockLightingSettings
{
    public Ep1StockBaseSubsystemSettings Base;
    public Ep1StockNativeTimeOfDay TimeOfDay;
    public float OccludedLuminanceScale;
    public RGBColor LightingMultiplierIfNoClouds;
    public Ep1StockLightingTime CurrentLightingSettings;
    public Ep1StockLightingGroup TerrainLighting;
}

// Reborn: original LightingTime schema fields require independent source/native qualification before emission.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1StockLightingTime
{
    public Ep1StockNativeTimeOfDay TimeOfDay;
    public List<Ep1StockLightingInfo> Lighting;
}

// Reborn: original LightingInfo schema fields require independent source/native qualification before emission.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1StockLightingInfo
{
    public RGBColor Ambient;
    public RGBColor Diffuse;
    public Vector3 LightPos;
}

// Reborn: original LightingGroup schema fields require independent source/native qualification before emission.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1StockLightingGroup
{
    public List<Ep1StockLightingTime> LightingTimeGroup;
}

// Reborn: original MissionHotSpotSettings schema fields require independent source/native qualification before emission.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1StockMissionHotSpotSettings
{
    public Ep1StockBaseSingletonSettings Base;
    public AssetReference<BaseAssetType> WaypointMarker;
    public Ep1RadiusDecalTemplate DecalTemplate;
}

// Reborn: original ModelSettings schema fields require independent source/native qualification before emission.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1StockModelSettings
{
    public Ep1StockBaseSubsystemSettings Base;
    public int AnimationSharingCap;
    public float AnimationSharingFrameTolerance;
    public float AnimationSharingDrasticThreshold;
    public SageBool ForceModelsToFollowTimeOfDay;
    public SageBool ForceModelsToFollowWeather;
}

// Reborn: original OrdersManagerSettings schema fields require independent source/native qualification before emission.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1StockOrdersManagerSettings
{
    public Ep1StockBaseSingletonSettings Base;
    public Time LineFadeTime;
    public AssetReference<BaseAssetType>* FormationPreviewShader;
    public AssetReference<BaseAssetType>* BuildPlacementShader;
    public Ep1StockLineSetting ExecutingLine;
    public Ep1StockLineSetting PendingLine;
    public Ep1StockLineSetting PatrolLine;
    public Ep1StockLineSetting PatrolPendingLine;
    public Ep1StockLineSetting TargetMoveLine;
    public Ep1StockLineSetting TargetAttackLine;
    public ShadowInfo* PatrolNode;
    public ShadowInfo* PatrolStart;
    public ShadowInfo* PendingNode;
    public ShadowInfo* PendingStart;
}

// Reborn: original LineSetting schema fields require independent source/native qualification before emission.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1StockLineSetting
{
    public float Width;
    public float VOffsetRate;
    public AssetReference<BaseAssetType> Texture;
    public float TextureTileSize;
}

// Reborn: original PathfindSettings schema fields require independent source/native qualification before emission.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1StockPathfindSettings
{
    public Ep1StockBaseSingletonSettings Base;
    public uint MaxPathfindCellsPerFrame;
    public uint MaxPathfindCellsPerPhase;
    public uint MaxCellsFindMeleeEngagementLocation;
    public uint MaxCellsAdjustDestination;
    public uint MaxCellsAdjustHordeMeleeDestination;
    public uint MaxCellsAdjustTargetDestination;
    public uint MaxCellsAdjustToPossibleDestination;
    public uint MaxCellsAdjustToMeleeDestination;
    public uint MaxCellsAdjustToNearestGroundCell;
    public uint MaxCellsAdjustToNearestValidCell;
    public uint MaxCellsPatchPath;
    public uint MaxCellsToExamineTowardsGoal;
    public uint MaxExtraCollisionResponsePathsPerFrame;
    public uint MaxExtraMoveAwayFromPathPathsPerFrame;
    public uint LayerZCloseEnough;
}

// Reborn: original ScorchMarkSettings schema fields require independent source/native qualification before emission.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1StockScorchMarkSettings
{
    public Ep1StockBaseSingletonSettings Base;
    public List<Ep1StockScorchMarkSetting> ScorchMarkSetting;
}

// Reborn: original ScorchMarkSetting schema fields require independent source/native qualification before emission.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1StockScorchMarkSetting
{
    public TerrainClassType TerrainClass;
    public List<TerrainScorchType> Scorch;
}

// Reborn: original SkirmishSettings schema fields require independent source/native qualification before emission.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1StockSkirmishSettings
{
    public Ep1StockBaseSubsystemSettings Base;
    public TypedAssetId<BaseAssetType> RandomCrateSpawner;
    public List<Ep1StockBuildSpeed> HandicapBuildSpeed;
}

// Reborn: original BuildSpeed schema fields require independent source/native qualification before emission.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1StockBuildSpeed
{
    public float Speed;
}

// Reborn: original TerrainVisualSettings schema fields require independent source/native qualification before emission.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1StockTerrainVisualSettings
{
    public Ep1StockBaseSubsystemSettings Base;
    public Vector3 WaterPosition;
    public Vector2 WaterExtents;
}

// Reborn: original W3DDeviceSettings schema fields require independent source/native qualification before emission.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1StockW3DDeviceSettings
{
    public Ep1StockBaseSingletonSettings Base;
    public int MaxTerrainTracks;
    public Percentage OpacityOfSimpleMergeDecals;
    public float OccludedColorLuminanceScale;
    public int MaxRoadSegments;
    public int MaxRoadVertex;
    public int MaxRoadIndex;
    public Ep1StockShroudStatusType DefaultShroudStatus;
    public RGBColor ShroudColor;
    public ObjectFilter TreeFadeObjectFilter;
    public List<AnsiString> StandardPublicBone;
    public SageBool UseSimpleHordeDecals;
    // Reborn: original device settings pack all local byte fields together after the bone list.
    public byte FogAlpha,ShroudAlpha,ClearAlpha;
}

// Reborn: original TerrainAssetCollection schema fields require independent source/native qualification before emission.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1StockTerrainAssetCollection
{
    public BaseAssetType Base;
    public List<Ep1StockTerrainAsset> TerrainAsset;
}

// Reborn: original TerrainAsset schema fields require independent source/native qualification before emission.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1StockTerrainAsset
{
    public AnsiString id;
    public AnsiString Texture;
    public AnsiString BumpTexture;
    public TerrainClassType Class;
    public AnsiString WBFolder;
}

// Reborn: original UIJoypadCommandBarButtonBuild schema fields require independent source/native qualification before emission.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1StockUIJoypadCommandBarButtonBuild
{
    public BaseAssetType Base;
    public List<Ep1StockUIJoypadCommandBarButtonBuildFactionSettings> FactionSettings;
}

// Reborn: original UIJoypadCommandBarButtonBuildFactionSettings schema fields require independent source/native qualification before emission.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1StockUIJoypadCommandBarButtonBuildFactionSettings
{
    public FactionType Faction;
    public AssetReference<BaseAssetType> LockedButtonStateData;
}

// Reborn: original UIJoypadCommandBarControlGroups schema fields require independent source/native qualification before emission.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1StockUIJoypadCommandBarControlGroups
{
    public BaseAssetType Base;
    public AssetReference<BaseAssetType> Template;
}

// Reborn: original UIJoypadCommandBarHomogenousGroup schema fields require independent source/native qualification before emission.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1StockUIJoypadCommandBarHomogenousGroup
{
    public BaseAssetType Base;
    public AssetReference<BaseAssetType> Template;
}

// Reborn: original UIJoypadCommandBarMixedGroup schema fields require independent source/native qualification before emission.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1StockUIJoypadCommandBarMixedGroup
{
    public BaseAssetType Base;
    public AssetReference<BaseAssetType> Template;
}

// Reborn: original UIJoypadCommandBarSingleUnit schema fields require independent source/native qualification before emission.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1StockUIJoypadCommandBarSingleUnit
{
    public BaseAssetType Base;
    public AssetReference<BaseAssetType> UnitTemplate;
    public AssetReference<BaseAssetType> ProductionStructureTemplate;
    public AssetReference<BaseAssetType> NonProductionStructureTemplate;
    public AssetReference<BaseAssetType> HotSpotTemplate;
}

// Reborn: original UIJoypadCommandBarStances schema fields require independent source/native qualification before emission.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1StockUIJoypadCommandBarStances
{
    public BaseAssetType Base;
    public AssetReference<BaseAssetType> Template;
}

// Reborn: original UIJoypadCommandBarCoopCommunicator schema fields require independent source/native qualification before emission.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1StockUIJoypadCommandBarCoopCommunicator
{
    public BaseAssetType Base;
    public AssetReference<BaseAssetType> AICoopTemplate;
}

// Reborn: original UIJoypadCommandBarPlayerPowers schema fields require independent source/native qualification before emission.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1StockUIJoypadCommandBarPlayerPowers
{
    public BaseAssetType Base;
    public AssetReference<BaseAssetType> Template;
}

// Reborn: original UIJoypadCommandBarTopMenu schema fields require independent source/native qualification before emission.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1StockUIJoypadCommandBarTopMenu
{
    public BaseAssetType Base;
    public AssetReference<BaseAssetType> Template;
}

// Reborn: original UIMouseTacticalQuickChatDisplay schema fields require independent source/native qualification before emission.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1StockUIMouseTacticalQuickChatDisplay
{
    public BaseAssetType Base;
    public AssetReference<BaseAssetType> CloseKey;
    public Time DelayDuration;
    public List<Ep1StockUIMouseTacticalQuickChatDisplayMenuItem> MenuItem;
    public Ep1StockUIMouseTacticalQuickChatDisplayCloseItem CloseMenuItem;
}

// Reborn: original UIMouseTacticalQuickChatDisplayMenuItem schema fields require independent source/native qualification before emission.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1StockUIMouseTacticalQuickChatDisplayMenuItem
{
    public AnsiString Title;
    public Ep1StockUIMouseTacticalQuickChatDisplayChatDestination* ChatDestination;
    public List<Ep1StockUIMouseTacticalQuickChatDisplayMenuItemChatMessageGroup> ChatMessageGroup;
    public List<Ep1StockUIMouseTacticalQuickChatDisplayMenuItem> MenuItem;
    public Ep1StockUIMouseTacticalQuickChatDisplayCloseItem* CloseMenuItem;
}

// Reborn: original UIMouseTacticalQuickChatDisplayMenuItemChatMessageGroup schema fields require independent source/native qualification before emission.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1StockUIMouseTacticalQuickChatDisplayMenuItemChatMessageGroup
{
    public FactionType Faction;
    public List<Ep1StockUIMouseTacticalQuickChatDisplayMenuItemChatMessage> ChatMessage;
}

// Reborn: original UIMouseTacticalQuickChatDisplayMenuItemChatMessage schema fields require independent source/native qualification before emission.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1StockUIMouseTacticalQuickChatDisplayMenuItemChatMessage
{
    public TypedAssetId<BaseAssetType> MessageRef;
}

// Reborn: original UIMouseTacticalQuickChatDisplayCloseItem schema fields require independent source/native qualification before emission.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1StockUIMouseTacticalQuickChatDisplayCloseItem
{
    public AnsiString Title;
}

// Reborn: original UIMouseDragSelectionFeedback schema fields require independent source/native qualification before emission.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1StockUIMouseDragSelectionFeedback
{
    public BaseAssetType Base;
    public float BoxEdgeWidth;
    public float LassoWidth;
    public Color BoxColor;
    public Color LassoColor;
}

// Reborn: original UIMouseHUDElementHotKeyGate schema fields require independent source/native qualification before emission.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1StockUIMouseHUDElementHotKeyGate
{
    public BaseAssetType Base;
    public List<Ep1StockUIMouseHUDElementHotKeyMapping> ElementHotKeyMapping;
}

// Reborn: original UIMouseHUDElementHotKeyMapping schema fields require independent source/native qualification before emission.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1StockUIMouseHUDElementHotKeyMapping
{
    public Ep1StockMouseHUDFixedElementId ElementId;
    public AssetReference<BaseAssetType> HotKeySlot;
}

// Reborn: original UIMouseTacticalAttackMoveButton schema fields require independent source/native qualification before emission.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1StockUIMouseTacticalAttackMoveButton
{
    public UIMouseSimpleFixedButton Base;
}

// Reborn: original UIMouseTacticalBuildQueueTab schema fields require independent source/native qualification before emission.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1StockUIMouseTacticalBuildQueueTab
{
    public BaseAssetType Base;
    public List<Ep1StockUIMouseTacticalBuildQueueTabQueueTypeSettings> QueueTypeSettings;
}

// Reborn: original UIMouseTacticalBuildQueueTabQueueTypeSettings schema fields require independent source/native qualification before emission.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1StockUIMouseTacticalBuildQueueTabQueueTypeSettings
{
    public ProductionQueueType QueueType;
    public InGameUISimpleHelpTemplate MouseOverHelp;
}

// Reborn: original UIMouseTacticalBuildQueueTypeTab schema fields require independent source/native qualification before emission.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1StockUIMouseTacticalBuildQueueTypeTab
{
    public BaseAssetType Base;
    public List<Ep1StockUIMouseTacticalBuildQueueTypeTabQueueTypeSettings> QueueTypeSettings;
}

// Reborn: original UIMouseTacticalBuildQueueTypeTabQueueTypeSettings schema fields require independent source/native qualification before emission.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1StockUIMouseTacticalBuildQueueTypeTabQueueTypeSettings
{
    public ProductionQueueType QueueType;
    public InGameUISimpleHelpTemplate MouseOverHelp;
}

// Reborn: original UIMouseTacticalCommandAttack schema fields require independent source/native qualification before emission.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1StockUIMouseTacticalCommandAttack
{
    public BaseAssetType Base;
    public AssetReference<BaseAssetType> AttackObjectCursor;
    public AssetReference<BaseAssetType> ForceAttackObjectCursor;
    public AssetReference<BaseAssetType> ForceAttackGroundCursor;
    public AssetReference<BaseAssetType> ExplosiveChargeCursor;
    public AssetReference<BaseAssetType> ClearGarrisonCursor;
    public AssetReference<BaseAssetType> DisguiseCursor;
    public AssetReference<BaseAssetType> CallAirstrikeCursor;
}

// Reborn: original UIMouseTacticalCommandAttackMove schema fields require independent source/native qualification before emission.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1StockUIMouseTacticalCommandAttackMove
{
    public BaseAssetType Base;
    public AssetReference<BaseAssetType> NormalCursor;
    public AssetReference<BaseAssetType> ForcedCursor;
}

// Reborn: original UIMouseTacticalCommandCommunicateWithAllies schema fields require independent source/native qualification before emission.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1StockUIMouseTacticalCommandCommunicateWithAllies
{
    public BaseAssetType Base;
    public AssetReference<BaseAssetType> PlaceUnattachedBeaconCursor;
    public AssetReference<BaseAssetType> PlaceAttachedBeaconCursor;
    public AssetReference<BaseAssetType> PingRadarCursor;
    public ObjectFilter* BeaconAttachableObjectFilter;
}

// Reborn: original UIMouseTacticalCommandDock schema fields require independent source/native qualification before emission.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1StockUIMouseTacticalCommandDock
{
    public BaseAssetType Base;
    public AssetReference<BaseAssetType> Cursor;
}

// Reborn: original UIMouseTacticalCommandEnter schema fields require independent source/native qualification before emission.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1StockUIMouseTacticalCommandEnter
{
    public BaseAssetType Base;
    public AssetReference<BaseAssetType> DefaultCursor;
    public AssetReference<BaseAssetType> ContestCursor;
    public List<Ep1StockUIMouseTacticalCommandEnterInfiltratorEffectTypeSettings> InfiltratorEffectTypeSettings;
}

// Reborn: original UIMouseTacticalCommandEnterInfiltratorEffectTypeSettings schema fields require independent source/native qualification before emission.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1StockUIMouseTacticalCommandEnterInfiltratorEffectTypeSettings
{
    public InfiltratorEffectType Effect;
    public AssetReference<BaseAssetType> Cursor;
}

// Reborn: original UIMouseTacticalCommandForceAttack schema fields require independent source/native qualification before emission.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1StockUIMouseTacticalCommandForceAttack
{
    public BaseAssetType Base;
    public AssetReference<BaseAssetType> ObjectCursor;
    public AssetReference<BaseAssetType> GroundCursor;
}

// Reborn: original UIMouseTacticalCommandForceMove schema fields require independent source/native qualification before emission.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1StockUIMouseTacticalCommandForceMove
{
    public BaseAssetType Base;
    public AssetReference<BaseAssetType> Cursor;
}

// Reborn: original UIMouseTacticalCommandGetCrate schema fields require independent source/native qualification before emission.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1StockUIMouseTacticalCommandGetCrate
{
    public BaseAssetType Base;
    public AssetReference<BaseAssetType> Cursor;
}

// Reborn: original UIMouseTacticalCommandMineOre schema fields require independent source/native qualification before emission.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1StockUIMouseTacticalCommandMineOre
{
    public BaseAssetType Base;
    public AssetReference<BaseAssetType> Cursor;
}

// Reborn: original UIMouseTacticalCommandMove schema fields require independent source/native qualification before emission.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1StockUIMouseTacticalCommandMove
{
    public BaseAssetType Base;
    public AssetReference<BaseAssetType> ForwardCursor;
    public AssetReference<BaseAssetType> ReverseCursor;
}

// Reborn: original UIMouseTacticalCommandPlaceAIBeacon schema fields require independent source/native qualification before emission.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1StockUIMouseTacticalCommandPlaceAIBeacon
{
    public BaseAssetType Base;
    public List<Ep1StockUIMouseTacticalCommandPlaceAIBeaconTypeSettings> BeaconTypeSettings;
}

// Reborn: original UIMouseTacticalCommandPlaceAIBeaconTypeSettings schema fields require independent source/native qualification before emission.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1StockUIMouseTacticalCommandPlaceAIBeaconTypeSettings
{
    public Ep1StockAIBeaconType Type;
    public AssetReference<BaseAssetType> PlaceUnattachedCursor;
    public AssetReference<BaseAssetType> PlaceAttachedCursor;
    public ObjectFilter* AttachableObjectFilter;
    public SageBool CanTargetObjects;
    public SageBool CanTargetPosition;
}

// Reborn: original UIMouseTacticalCommandPlaceRallyPoint schema fields require independent source/native qualification before emission.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1StockUIMouseTacticalCommandPlaceRallyPoint
{
    public BaseAssetType Base;
    public AssetReference<BaseAssetType> Cursor;
}

// Reborn: original UIMouseTacticalCommandReverseMove schema fields require independent source/native qualification before emission.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1StockUIMouseTacticalCommandReverseMove
{
    public BaseAssetType Base;
    public AssetReference<BaseAssetType> Cursor;
}

// Reborn: original UIMouseTacticalCommandSelect schema fields require independent source/native qualification before emission.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1StockUIMouseTacticalCommandSelect
{
    public BaseAssetType Base;
    public AssetReference<BaseAssetType> Cursor;
    public AssetReference<BaseAssetType> TakeUnitsFromAllyHotKeySlot;
    public AssetReference<BaseAssetType> GiveUnitsToAllyHotKeySlot;
}

// Reborn: original UIMouseTacticalCommandSell schema fields require independent source/native qualification before emission.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1StockUIMouseTacticalCommandSell
{
    public BaseAssetType Base;
    public Ep1StockUIMouseTacticalCommandSellCursorSet DefaultCursors;
    public List<Ep1StockUIMouseTacticalCommandSellFactionCursorSet> FactionCursors;
}

// Reborn: original UIMouseTacticalCommandSellCursorSet schema fields require independent source/native qualification before emission.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1StockUIMouseTacticalCommandSellCursorSet
{
    public AssetReference<BaseAssetType> ValidTarget;
    public AssetReference<BaseAssetType> InvalidTarget;
}

// Reborn: original UIMouseTacticalCommandSellFactionCursorSet schema fields require independent source/native qualification before emission.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1StockUIMouseTacticalCommandSellFactionCursorSet
{
    public FactionType Faction;
    public Ep1StockUIMouseTacticalCommandSellCursorSet Cursors;
}

// Reborn: original UIMouseTacticalCommandSetRallyPoint schema fields require independent source/native qualification before emission.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1StockUIMouseTacticalCommandSetRallyPoint
{
    public BaseAssetType Base;
    public AssetReference<BaseAssetType> Cursor;
}

// Reborn: original UIMouseTacticalCommandToggleSelfRepair schema fields require independent source/native qualification before emission.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1StockUIMouseTacticalCommandToggleSelfRepair
{
    public BaseAssetType Base;
    public AssetReference<BaseAssetType> ValidTargetCursor;
    public AssetReference<BaseAssetType> InvalidTargetCursor;
}

// Reborn: original UIMouseTacticalCoOpAIButtonsAssaultButton schema fields require independent source/native qualification before emission.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1StockUIMouseTacticalCoOpAIButtonsAssaultButton
{
    public UIMouseSimpleFixedButton Base;
}

// Reborn: original UIMouseTacticalCoOpAIButtonsHelpButton schema fields require independent source/native qualification before emission.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1StockUIMouseTacticalCoOpAIButtonsHelpButton
{
    public UIMouseSimpleFixedButton Base;
}

// Reborn: original UIMouseTacticalCoOpAIButtonsHoldObjectButton schema fields require independent source/native qualification before emission.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1StockUIMouseTacticalCoOpAIButtonsHoldObjectButton
{
    public UIMouseSimpleFixedButton Base;
}

// Reborn: original UIMouseTacticalCoOpAIButtonsHoldPositionButton schema fields require independent source/native qualification before emission.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1StockUIMouseTacticalCoOpAIButtonsHoldPositionButton
{
    public UIMouseSimpleFixedButton Base;
}

// Reborn: original UIMouseTacticalCoOpAIButtonsNormalButton schema fields require independent source/native qualification before emission.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1StockUIMouseTacticalCoOpAIButtonsNormalButton
{
    public UIMouseSimpleFixedButton Base;
}

// Reborn: original UIMouseTacticalDeactivateMissionHotSpotsButton schema fields require independent source/native qualification before emission.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1StockUIMouseTacticalDeactivateMissionHotSpotsButton
{
    public UIMouseSimpleFixedButton Base;
}

// Reborn: original UIMouseTacticalExitContainerButton schema fields require independent source/native qualification before emission.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1StockUIMouseTacticalExitContainerButton
{
    public BaseAssetType Base;
    public AnsiString HelpTitleFormat;
    public AnsiString HelpDescription;
}

// Reborn: original UIMouseTacticalFastForwardButton schema fields require independent source/native qualification before emission.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1StockUIMouseTacticalFastForwardButton
{
    public UIMouseSimpleFixedButton Base;
}

// Reborn: original UIMouseTacticalForceAttackButton schema fields require independent source/native qualification before emission.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1StockUIMouseTacticalForceAttackButton
{
    public UIMouseSimpleFixedButton Base;
}

// Reborn: original UIMouseTacticalForceAttackMoveButton schema fields require independent source/native qualification before emission.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1StockUIMouseTacticalForceAttackMoveButton
{
    public UIMouseSimpleFixedButton Base;
}

// Reborn: original UIMouseTacticalForceMoveButton schema fields require independent source/native qualification before emission.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1StockUIMouseTacticalForceMoveButton
{
    public UIMouseSimpleFixedButton Base;
}

// Reborn: original UIMouseTacticalGroupSelection schema fields require independent source/native qualification before emission.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1StockUIMouseTacticalGroupSelection
{
    public BaseAssetType Base;
    public List<Ep1StockUIMouseTacticalGroupSelectionGroup> Group;
}

// Reborn: original UIMouseTacticalGroupSelectionGroup schema fields require independent source/native qualification before emission.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1StockUIMouseTacticalGroupSelectionGroup
{
    public AssetReference<BaseAssetType> CreateHotKeySlot;
    public AssetReference<BaseAssetType> AddToHotKeySlot;
    public AssetReference<BaseAssetType> SelectHotKeySlot;
    public AssetReference<BaseAssetType> ViewHotKeySlot;
}

// Reborn: original UIMouseTacticalLookAt schema fields require independent source/native qualification before emission.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1StockUIMouseTacticalLookAt
{
    public BaseAssetType Base;
    public AssetReference<BaseAssetType> ScrollCursor;
    public AssetReference<BaseAssetType> ScrollLeftHotKeySlot;
    public AssetReference<BaseAssetType> ScrollRightHotKeySlot;
    public AssetReference<BaseAssetType> ScrollUpHotKeySlot;
    public AssetReference<BaseAssetType> ScrollDownHotKeySlot;
    public AssetReference<BaseAssetType> RotateLeftHotKeySlot;
    public AssetReference<BaseAssetType> RotateRightHotKeySlot;
    public AssetReference<BaseAssetType> ZoomInHotKeySlot;
    public AssetReference<BaseAssetType> ZoomOutHotKeySlot;
    public Percentage KeyScrollFactor;
    public Percentage MouseDragScrollFactor;
    public Percentage MouseEdgeScrollFactor;
    public List<Ep1StockUIMouseTacticalLookAtViewBookmark> ViewBookmark;
}

// Reborn: original UIMouseTacticalLookAtViewBookmark schema fields require independent source/native qualification before emission.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1StockUIMouseTacticalLookAtViewBookmark
{
    public AssetReference<BaseAssetType> GoToHotKeySlot;
    public AssetReference<BaseAssetType> SaveHotKeySlot;
}

// Reborn: original UIMouseTacticalMainCommandBarHotKeys schema fields require independent source/native qualification before emission.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1StockUIMouseTacticalMainCommandBarHotKeys
{
    public BaseAssetType Base;
    public AssetReference<BaseAssetType> SelectionRefinementPageSlot;
    public List<Ep1StockUIMouseTacticalMainCommandBarHotKeysBuildQueue> BuildQueuePage;
    public List<AssetReference<BaseAssetType>> ButtonSlot;
}

// Reborn: original UIMouseTacticalMainCommandBarHotKeysBuildQueue schema fields require independent source/native qualification before emission.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1StockUIMouseTacticalMainCommandBarHotKeysBuildQueue
{
    public ProductionQueueType QueueType;
    public AssetReference<BaseAssetType> Slot;
}

// Reborn: original UIMouseTacticalMiscHotKeys schema fields require independent source/native qualification before emission.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1StockUIMouseTacticalMiscHotKeys
{
    public BaseAssetType Base;
    public AssetReference<BaseAssetType> AllCheer;
    public AssetReference<BaseAssetType> AttackMove;
    public AssetReference<BaseAssetType> CameraReset;
    public AssetReference<BaseAssetType> ChatWithAllies;
    public AssetReference<BaseAssetType> ChatWithBuddies;
    public AssetReference<BaseAssetType> ChatWithEveryone;
    public AssetReference<BaseAssetType> CommunicateWithAlliesMode;
    public AssetReference<BaseAssetType> CreateFormation;
    public AssetReference<BaseAssetType> CycleHarvesters;
    public AssetReference<BaseAssetType> DeleteBeacon;
    public AssetReference<BaseAssetType> ForceAttack;
    public AssetReference<BaseAssetType> ForceAttackMove;
    public AssetReference<BaseAssetType> ForceMove;
    public AssetReference<BaseAssetType> GoToNextBuildQueue;
    public AssetReference<BaseAssetType> GoToPriorBuildQueue;
    public AssetReference<BaseAssetType> GoToNextSubGroup;
    public AssetReference<BaseAssetType> GoToPriorSubGroup;
    public AssetReference<BaseAssetType> OpenAdvancedCommands;
    public AssetReference<BaseAssetType> OpenLoadGameScreen;
    public AssetReference<BaseAssetType> OpenMessenger;
    public AssetReference<BaseAssetType> OpenPauseScreen;
    public AssetReference<BaseAssetType> OpenPlayerTechStore;
    public AssetReference<BaseAssetType> OpenSaveGameScreen;
    public AssetReference<BaseAssetType> OpenStatusScreen;
    public AssetReference<BaseAssetType> PlaceRallyPoint;
    public AssetReference<BaseAssetType> PlanningMode;
    public AssetReference<BaseAssetType> PreferSelection;
    public AssetReference<BaseAssetType> QuickChatMenu;
    public AssetReference<BaseAssetType> ReverseMove;
    public AssetReference<BaseAssetType> Scatter;
    public AssetReference<BaseAssetType> SelectAll;
    public AssetReference<BaseAssetType> SelectMatchingUnits;
    public AssetReference<BaseAssetType> SellMode;
    public AssetReference<BaseAssetType> ShowAllHealthBars;
    public AssetReference<BaseAssetType> Stop;
    public AssetReference<BaseAssetType> TelestratorErase;
    public AssetReference<BaseAssetType> TelestratorNextColor;
    public AssetReference<BaseAssetType> TelestratorNextLineWidth;
    public AssetReference<BaseAssetType> TelestratorPriorColor;
    public AssetReference<BaseAssetType> TelestratorToggle;
    public AssetReference<BaseAssetType> ToggleFastForwardMode;
    public AssetReference<BaseAssetType> ToggleHUD;
    public AssetReference<BaseAssetType> TogglePowerMode;
    public AssetReference<BaseAssetType> ToggleRepairMode;
    public AssetReference<BaseAssetType> ViewHomeBase;
    public AssetReference<BaseAssetType> ViewLastEvaEvent;
    public AssetReference<BaseAssetType> WaypointMode;
    public List<Ep1StockUIMouseTacticalMiscHotKeysStanceSettings> StanceSettings;
}

// Reborn: original UIMouseTacticalMiscHotKeysStanceSettings schema fields require independent source/native qualification before emission.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1StockUIMouseTacticalMiscHotKeysStanceSettings
{
    public Stance Stance;
    public AssetReference<BaseAssetType> Slot;
}

// Reborn: original UIMouseTacticalOpenAdvancedCommandsButton schema fields require independent source/native qualification before emission.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1StockUIMouseTacticalOpenAdvancedCommandsButton
{
    public UIMouseSimpleFixedButton Base;
}

// Reborn: original UIMouseTacticalPauseButton schema fields require independent source/native qualification before emission.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1StockUIMouseTacticalPauseButton
{
    public UIMouseSimpleFixedButton Base;
}

// Reborn: original UIMouseTacticalPlanningModeButton schema fields require independent source/native qualification before emission.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1StockUIMouseTacticalPlanningModeButton
{
    public UIMouseSimpleFixedButton Base;
}

// Reborn: original UIMouseTacticalPlayerPowerHotKeys schema fields require independent source/native qualification before emission.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1StockUIMouseTacticalPlayerPowerHotKeys
{
    public BaseAssetType Base;
    public List<AssetReference<BaseAssetType>> Slot;
}

// Reborn: original UIMouseTacticalPlayerTechStoreButton schema fields require independent source/native qualification before emission.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1StockUIMouseTacticalPlayerTechStoreButton
{
    public UIMouseSimpleFixedButton Base;
    public AssetReference<BaseAssetType>* TechPointAwardedSound;
    public InGameUISimpleHelpTemplate YurikoMouseOverHelp;
}

// Reborn: original UIMouseTacticalRadarTelestrator schema fields require independent source/native qualification before emission.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1StockUIMouseTacticalRadarTelestrator
{
    public BaseAssetType Base;
    public Time StrokeSustainDuration;
    public Time StrokeFadeDuration;
}

// Reborn: original UIMouseTacticalReverseMoveButton schema fields require independent source/native qualification before emission.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1StockUIMouseTacticalReverseMoveButton
{
    public UIMouseSimpleFixedButton Base;
}

// Reborn: original UIMouseTacticalSelectionDetailsButtonHotKeys schema fields require independent source/native qualification before emission.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1StockUIMouseTacticalSelectionDetailsButtonHotKeys
{
    public BaseAssetType Base;
    public List<AssetReference<BaseAssetType>> UnitAbilitySlot;
}

// Reborn: original UIMouseTacticalSelectionRefinementTab schema fields require independent source/native qualification before emission.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1StockUIMouseTacticalSelectionRefinementTab
{
    public UIMouseSimpleFixedButton Base;
}

// Reborn: original UIMouseTacticalSellModeButton schema fields require independent source/native qualification before emission.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1StockUIMouseTacticalSellModeButton
{
    public UIMouseSimpleFixedButton Base;
}

// Reborn: original UIMouseTacticalSetStanceButton schema fields require independent source/native qualification before emission.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1StockUIMouseTacticalSetStanceButton
{
    public BaseAssetType Base;
    public List<Ep1StockUIMouseTacticalSetStanceButtonStanceSettings> StanceSettings;
}

// Reborn: original UIMouseTacticalSetStanceButtonStanceSettings schema fields require independent source/native qualification before emission.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1StockUIMouseTacticalSetStanceButtonStanceSettings
{
    public Stance Stance;
    public InGameUISimpleHelpTemplate MouseOverHelp;
}

// Reborn: original UIMouseTacticalStopButton schema fields require independent source/native qualification before emission.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1StockUIMouseTacticalStopButton
{
    public UIMouseSimpleFixedButton Base;
}

// Reborn: original UIMouseTacticalTelestrator schema fields require independent source/native qualification before emission.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1StockUIMouseTacticalTelestrator
{
    public BaseAssetType Base;
    public AssetReference<BaseAssetType> DrawCursor;
}

// Reborn: original UIMouseTacticalThreatMeter schema fields require independent source/native qualification before emission.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1StockUIMouseTacticalThreatMeter
{
    public UIMouseSimpleFixedButton Base;
}

// Reborn: original UIMouseTacticalToggleRepairModeButton schema fields require independent source/native qualification before emission.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1StockUIMouseTacticalToggleRepairModeButton
{
    public UIMouseSimpleFixedButton Base;
}

// Reborn: original UIMouseTacticalUnitIntroButton schema fields require independent source/native qualification before emission.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1StockUIMouseTacticalUnitIntroButton
{
    public UIMouseSimpleFixedButton Base;
}

// Reborn: original UIMouseTacticalWaypointModeButton schema fields require independent source/native qualification before emission.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1StockUIMouseTacticalWaypointModeButton
{
    public UIMouseSimpleFixedButton Base;
}

// Reborn: original UIMouseVoiceChat schema fields require independent source/native qualification before emission.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1StockUIMouseVoiceChat
{
    public BaseAssetType Base;
    public AssetReference<BaseAssetType> ToggleModeHotKeySlot;
    public AssetReference<BaseAssetType> TalkHotKeySlot;
    public InGameUISimpleHelpTemplate* ModeButtonHelp;
    public InGameUISimpleHelpTemplate* TalkButtonHelp;
}

// Reborn: original UITimerDisplay schema fields require independent source/native qualification before emission.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1StockUITimerDisplay
{
    public BaseAssetType Base;
    public Time FlashDuration;
    public Vector2 Position;
    public Color FlashColor;
    public FontDesc NormalFont;
    public Color NormalColor;
    public FontDesc ReadyFont;
    public Color ReadyColor;
    public SageBool Centered;
}

// Reborn: original UIWorldBuilderDecalGrid schema fields require independent source/native qualification before emission.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1StockUIWorldBuilderDecalGrid
{
    public BaseAssetType Base;
    public AssetReference<BaseAssetType> Texture;
    public float Opacity;
    public Color Color;
}

// Reborn: original UICommandButton schema fields require independent source/native qualification before emission.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1StockUICommandButton
{
    public BaseAssetType Base;
    public List<Ep1StockUICommandButtonFactionSettings> FactionSettings;
}

// Reborn: original UICommandButtonFactionSettings schema fields require independent source/native qualification before emission.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1StockUICommandButtonFactionSettings
{
    public FactionType Faction;
    public AssetReference<BaseAssetType> LockedImage;
    public Color ImageTintColor;
}

// Reborn: original UICoopTinting schema fields require independent source/native qualification before emission.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1StockUICoopTinting
{
    public Ep1StockBaseSingletonSettings Base;
    public int FadeInFrames;
    public int FadeOutFrames;
    public int SustainFrames;
    public float TintFrequency;
    public float TintAmplitude;
    public Color TintColor;
}

// Reborn: original UIDecalCloud schema fields require independent source/native qualification before emission.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1StockUIDecalCloud
{
    public BaseAssetType Base;
    public AssetReference<BaseAssetType> BuildTexture;
    public AssetReference<BaseAssetType> DefenseTexture;
    public AssetReference<BaseAssetType> SpecialPowerRestrictionTexture;
}

// Reborn: original UIDecalGrid schema fields require independent source/native qualification before emission.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1StockUIDecalGrid
{
    public BaseAssetType Base;
    public AssetReference<BaseAssetType> WallTexture;
    public float WallOpacity;
    public Color WallDecalColor;
    public Ep1StockDecalSettings PrimarySettings;
    public Ep1StockDecalSettings SecondarySettings;
}

// Reborn: original DecalSettings schema fields require independent source/native qualification before emission.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1StockDecalSettings
{
    public AssetReference<BaseAssetType> ValidTexture;
    public AssetReference<BaseAssetType> InvalidTexture;
    public float Opacity;
    public Color LegalColor;
    public Color InvalidColor;
}

// Reborn: original UIFloatingTextDisplay schema fields require independent source/native qualification before emission.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1StockUIFloatingTextDisplay
{
    public BaseAssetType Base;
    public Time TimeOut;
    public Velocity MoveUpSpeed;
    public Velocity MoveVanishRate;
    public FontDesc Font;
}

// Reborn: original UIMessageDisplay schema fields require independent source/native qualification before emission.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1StockUIMessageDisplay
{
    public BaseAssetType Base;
    public Time Delay;
    public Color Color1;
    public Color Color2;
    public FontDesc Font;
}

// Reborn: original UIRadarObjectsLayer schema fields require independent source/native qualification before emission.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1StockUIRadarObjectsLayer
{
    public BaseAssetType Base;
    public uint MinUnitDotSize;
    public uint MinStructureDotSize;
    public Color NeutralColor;
    public List<Ep1StockUIRadarObjectsLayerSpecialIcon> SpecialIcon;
}

// Reborn: original UIRadarObjectsLayerSpecialIcon schema fields require independent source/native qualification before emission.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1StockUIRadarObjectsLayerSpecialIcon
{
    public AssetReference<BaseAssetType> Image;
    public Vector2 ImageOffset;
    public ObjectFilter ObjectFilter;
}

// Reborn: original UISubtitleDisplay schema fields require independent source/native qualification before emission.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1StockUISubtitleDisplay
{
    public BaseAssetType Base;
    public Time HoldTime;
    public int LineCount;
    public FontDesc Font;
    public Color PanelColor;
    public Color TextColor;
}

