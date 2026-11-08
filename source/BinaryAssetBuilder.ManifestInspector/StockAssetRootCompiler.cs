using System.Runtime.InteropServices;
using System.Text;
using System.Xml;
using BinaryAssetBuilder.Core.SageXml;
using Relo;
using SageBinaryData;
// Reborn: original rendering settings are a native asset, separate from the CLR environment API.
using Environment=SageBinaryData.Environment;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: explicitly reviewed asset roots use original EP1 native witnesses; this is not a blanket reflection admission path.
internal static unsafe class StockAssetRootCompiler
{
    internal sealed record Family(string Name,Type Type,int Size,string Source,bool Global,uint TypeId,uint TypeHash,bool WorldBuilder=false,bool Locale=false,bool EngineLayout=false);
    internal static readonly Family[] Families={
        // Reborn: these source roots use independently observed original PC factory/field ABI, not fabricated emitted witnesses.
        new("ConquerCardDefinitionUnit",typeof(ConquerCardDefinitionUnit),32,"Multiplayer/ConquerMode.xml",false,0xD45BF3C5u,0xDA316C85u,EngineLayout:true),
        new("ConquerCardDefinitionStructure",typeof(ConquerCardDefinitionStructure),16,"Multiplayer/ConquerMode.xml",false,0xFFAF70D0u,0xCA2996F8u,EngineLayout:true),
        new("ConquerCardDefinitionPower",typeof(ConquerCardDefinitionPower),24,"Multiplayer/ConquerMode.xml",false,0xF832F82Au,0x22FF8476u,EngineLayout:true),
        new("SiegeEvaEvent",typeof(SiegeEvaEvent),16,"Multiplayer/SiegeEvaEvents.xml",false,0x4B943441u,0x1082F8ACu,EngineLayout:true),
        new("DLContent",typeof(DLContent),24,"dlcontent/packages/package1/Package.xml",false,0x0CAD7864u,0x629BD41Cu,EngineLayout:true),
        new("BootupDisplaySequence",typeof(BootupDisplaySequence),16,"Shell/BootupSequence.xml",false,0xC8E41828u,0xC10DBCE4u,EngineLayout:true),
        new("TheVersion",typeof(TheVersion),12,"GlobalData/TheVersion.xml",true,0xFC82DC06u,0xA5DF846Fu,EngineLayout:true),
        // Reborn: locale font substitutions use their actual independent locale fingerprint and five original native witnesses.
        new("FontSubstitution",typeof(Ep1FontSubstitution),20,"GlobalData/FontSubstitution.xml",true,0x057786F9u,0xA63E49F9u,Locale:true),
        // Reborn: processed single-file audio has a real original global fingerprint and mandatory common-base/filename proof.
        new("DialogEvent",typeof(Marshaler.Ep1SingleFileSound),132,"Sounds/Speech.xml",true,0xD414D1C3u,0xD21E8BF8u),
        // Reborn: processed single-file audio has a real original global fingerprint and mandatory common-base/filename proof.
        new("MusicTrack",typeof(Marshaler.Ep1SingleFileSound),132,"Sounds/Music.xml",true,0x7046D9F8u,0x07C50662u),
        // Reborn: processed single-file audio has a real original global fingerprint and mandatory common-base/filename proof.
        new("AmbientStream",typeof(Marshaler.Ep1SingleFileSound),132,"Sounds/AmbientStream.xml",true,0xEA2C2798u,0x6D1624FBu),
        // Reborn: complete original configuration graphs require their exact retail type fingerprint and source/native layout proof.
        new("AudioSettings",typeof(Ep1StockUIConfigAudioSettings),sizeof(Ep1StockUIConfigAudioSettings),"Sounds/AudioSettings.xml",true,0x5608EE71u,0xD86ED334u),
        // Reborn: complete original configuration graphs require their exact retail type fingerprint and source/native layout proof.
        new("UIConfigList",typeof(Ep1StockUIConfigUIConfigList),sizeof(Ep1StockUIConfigUIConfigList),"GlobalData/UIConfigs.xml",false,0xB71D7323u,0x388842A4u),
        // Reborn: complete original singleton fields require the independent retail fingerprint and native proof.
        new("GiftManagerSettings",typeof(Ep1CoverageGiftManagerSettings),128,"System/GiftManagerSettings.xml",false,0x3E7426ACu,0x24B27FD8u),
        // Reborn: complete original singleton fields require the independent retail fingerprint and native proof.
        new("ResourceSettings",typeof(Ep1CoverageResourceSettings),8,"System/ResourceSettings.xml",true,0xC376BC30u,0x5CFBBD27u),
        // Reborn: original locomotor bounce rows require the real static fingerprint and full native field proof.
        new("BounceKickTerrainMapping",typeof(Ep1StockBounceKickTerrainMapping),sizeof(Ep1StockBounceKickTerrainMapping),"GlobalData/Locomotor.xml",false,0x8D32382Du,0x7DC1F449u),
        // Reborn: these explicit settings roots require complete original field/list proof; no generated type fingerprint is accepted.
        new("JoypadCommandBarTemplate",typeof(Ep1StockJoypadCommandBarTemplate),sizeof(Ep1StockJoypadCommandBarTemplate),"UI/Joypad/CommandBar/CommandBarTemplates.xml",false,0x732477A0u,0x74C471D9u),
        new("DamageFX",typeof(Ep1StockDamageFX),sizeof(Ep1StockDamageFX),"GlobalData/DamageFX.xml",false,0xAD3568F5u,0xC1AFAE9Au),
        new("WaterTextureList",typeof(Ep1StockWaterTextureList),sizeof(Ep1StockWaterTextureList),"MapSpecific/WaterTextureLists.xml",false,0xB28CCA45u,0x7194A098u,WorldBuilder:true),
        new("MainMenuPersonalityTemplate",typeof(MainMenuPersonalityTemplate),sizeof(MainMenuPersonalityTemplate),"Shell/MainMenuPersonalityGroup.xml",false,0x8F43193Au,0xB8069AB7u),
        new("StaticGameLOD",typeof(Ep1StockStaticGameLOD),sizeof(Ep1StockStaticGameLOD),"System/GameLOD.xml",true,0x1FB298D1u,0x4A93FC61u),
        new("DynamicGameLOD",typeof(DynamicGameLOD),sizeof(DynamicGameLOD),"System/GameLOD.xml",true,0xACEF31A4u,0x692B85B7u),
        new("TargetingCompareList",typeof(TargetingCompareList),sizeof(TargetingCompareList),"GlobalData/UnitAITargetChooser.xml",true,0xBE06A9E5u,0x4D83D996u),
        new("WaterSetting",typeof(Ep1StockWaterSetting),sizeof(Ep1StockWaterSetting),"GlobalData/WaterTemplates.xml",true,0xA2A3374Cu,0x1BFAD724u),
        new("GameLODPreset",typeof(Ep1StockGameLODPreset),sizeof(Ep1StockGameLODPreset),"System/GameLOD.xml",true,0x151D037Cu,0xD948D68Du),
        new("TerrainParticleMapping",typeof(Ep1StockTerrainParticleMapping),sizeof(Ep1StockTerrainParticleMapping),"System/TerrainParticleMapping.xml",false,0x5796C4B7u,0x943B4A82u),
        new("FontDefaultSetting",typeof(Ep1StockFontDefaultSetting),sizeof(Ep1StockFontDefaultSetting),"GlobalData/FontSettings.xml",true,0x51B9CD64u,0x0BC966D0u),
        new("ArmyDefinition",typeof(Ep1StockArmyDefinition),sizeof(Ep1StockArmyDefinition),"SkirmishAI/ArmyDefAllied.xml",true,0xD5D580F6u,0x0CB47A77u),
        new("LargeGroupAudioMap",typeof(LargeGroupAudioMap),sizeof(LargeGroupAudioMap),"Sounds/LargeGroupAudio.xml",true,0x1FD451BFu,0x28AD421Du),
        new("AudioLOD",typeof(AudioLOD),sizeof(AudioLOD),"System/GameLOD.xml",true,0x11D8BAC7u,0x014CBB53u),
        new("ConnectionLineManager",typeof(ConnectionLineManager),sizeof(ConnectionLineManager),"GlobalData/ConnectionLineManager.xml",false,0x064B2184u,0x6B72136Fu),
        new("DebugHotKeys",typeof(Ep1StockDebugHotKeys),sizeof(Ep1StockDebugHotKeys),"GlobalData/DebugHotKeys.xml",true,0xC8438DFDu,0x045A4CECu),
        new("DefaultHotKeys",typeof(DefaultHotKeys),sizeof(DefaultHotKeys),"GlobalData/DefaultHotKeys.xml",true,0xDEFCA2F6u,0x49495ABDu),
        new("DrawGroupInfo",typeof(Ep1StockDrawGroupInfo),sizeof(Ep1StockDrawGroupInfo),"GlobalData/DrawGroupInfo.xml",true,0xADADEA42u,0x466BD19Eu),
        new("Environment",typeof(Environment),sizeof(Environment),"GlobalData/Environment.xml",false,0x2893E309u,0x4C11A8FEu),
        new("ShadowMap",typeof(ShadowMap),sizeof(ShadowMap),"GlobalData/Environment.xml",false,0xF7CE0BBDu,0x35DFB9F9u),
        new("Language",typeof(Ep1StockLanguage),sizeof(Ep1StockLanguage),"GlobalData/Language.xml",true,0xADCA75CAu,0xBFFF81D6u),
        new("LocalBuildListMonitor",typeof(LocalBuildListMonitor),sizeof(LocalBuildListMonitor),"GlobalData/LocalBuildListMonitor.xml",false,0x12CFE3EFu,0x062B9401u),
        new("MeshInstanceTemplate",typeof(Ep1StockMeshInstanceTemplate),sizeof(Ep1StockMeshInstanceTemplate),"GlobalData/MeshInstanceTemplate.xml",true,0x309E61A1u,0x75382ABDu),
        new("MultiplayerSettings",typeof(MultiplayerSettings),sizeof(MultiplayerSettings),"GlobalData/MultiplayerSettings.xml",false,0x0B63AEF0u,0x9C2B6E7Eu),
        new("OnlineChatColors",typeof(OnlineChatColors),sizeof(OnlineChatColors),"GlobalData/OnlineChatColors.xml",false,0x502EED32u,0x522E06C9u),
        new("ProjectilePath",typeof(ProjectilePath),sizeof(ProjectilePath),"GlobalData/ProjectilePaths.xml",false,0x65EFB153u,0x8DAABAD0u),
        new("ScenarioManagerData",typeof(ScenarioManagerData),sizeof(ScenarioManagerData),"GlobalData/ScenarioTemplates.xml",false,0xBDB09BDDu,0x19F42E4Au),
        new("TargetingDistanceCompare",typeof(TargetingDistanceCompare),sizeof(TargetingDistanceCompare),"GlobalData/UnitAITargetChooser.xml",true,0x8CA5A7D7u,0xC00C41F3u),
        new("TargetingCombatChainCompare",typeof(TargetingCombatChainCompare),sizeof(TargetingCombatChainCompare),"GlobalData/UnitAITargetChooser.xml",true,0x0559C032u,0xB6138C0Du),
        new("TargetingInTurretArcCompare",typeof(TargetingInTurretArcCompare),sizeof(TargetingInTurretArcCompare),"GlobalData/UnitAITargetChooser.xml",true,0x77AC3B08u,0x5C986FB1u),
        new("UnitOverlayIconSettings",typeof(Ep1StockUnitOverlayIconSettings),sizeof(Ep1StockUnitOverlayIconSettings),"GlobalData/UnitOverlayIconSettings.xml",false,0x9053D603u,0x81FF61CDu),
        new("WaterTransparency",typeof(Ep1StockWaterTransparency),sizeof(Ep1StockWaterTransparency),"GlobalData/WaterTemplates.xml",true,0x582FDC2Au,0x1F5909EBu),
        new("YurikoHotKeys",typeof(YurikoHotKeys),sizeof(YurikoHotKeys),"GlobalData/YurikoHotKeys.xml",true,0xCC220E9Eu,0xCF96257Eu),
        new("IntelDB",typeof(IntelDB),sizeof(IntelDB),"IntelDB.xml",false,0x1F9865CEu,0x2CC41189u),
        new("NetworkSettings",typeof(Ep1StockNetworkSettings),sizeof(Ep1StockNetworkSettings),"Multiplayer/NetworkSettings.xml",true,0xFEA876ADu,0x7D077C96u),
        new("MainMenuPersonalityGroup",typeof(MainMenuPersonalityGroup),sizeof(MainMenuPersonalityGroup),"Shell/MainMenuPersonalityGroup.xml",false,0x1F5EDCC8u,0xAC30CC23u),
        new("MiscEvaEvents",typeof(Ep1StockMiscEvaEvents),sizeof(Ep1StockMiscEvaEvents),"Sounds/MiscEvaEvents.xml",false,0x6BBCE2ECu,0xB521A78Fu),
        new("MusicPalette",typeof(MusicPalette),sizeof(MusicPalette),"Sounds/MusicScript_UnknownZone.xml",false,0x8F7DC19Bu,0xD96FC160u,WorldBuilder:true),
        new("AutoParticleSystemSettings",typeof(Ep1StockAutoParticleSystemSettings),sizeof(Ep1StockAutoParticleSystemSettings),"System/AutoParticleSettings.xml",true,0xC36CC13Eu,0x4A581767u),
        new("BreezeInfo",typeof(Ep1StockBreezeInfo),sizeof(Ep1StockBreezeInfo),"System/BreezeInfo.xml",false,0x60F120B8u,0x3516A119u),
        new("BuildSystemSettings",typeof(Ep1StockBuildSystemSettings),sizeof(Ep1StockBuildSystemSettings),"System/BuildSystemSettings.xml",true,0xECE7DB5Eu,0xF8069076u),
        new("DamageFXSettings",typeof(Ep1StockDamageFXSettings),sizeof(Ep1StockDamageFXSettings),"System/DamageFXSettings.xml",false,0x04878391u,0x7A57612Cu),
        new("DynamicsContactTagMapping",typeof(Ep1StockDynamicsContactTagMapping),sizeof(Ep1StockDynamicsContactTagMapping),"System/DynamicsContactTagMapping.xml",false,0x3E3CE4BAu,0x358AB09Du),
        new("GraphicsHardware",typeof(Ep1StockGraphicsHardware),sizeof(Ep1StockGraphicsHardware),"System/GraphicsHardware.xml",true,0xA9A98B43u,0x42AA52A8u),
        new("InvisibilityManagerSettings",typeof(Ep1StockInvisibilityManagerSettings),sizeof(Ep1StockInvisibilityManagerSettings),"System/InvisibilityManagerSettings.xml",false,0x0F2AEC14u,0xCB4D6A58u),
        new("LightingSettings",typeof(Ep1StockLightingSettings),sizeof(Ep1StockLightingSettings),"System/LightingSettings.xml",true,0xBCEDA263u,0x24BCD59Du),
        new("MissionHotSpotSettings",typeof(Ep1StockMissionHotSpotSettings),sizeof(Ep1StockMissionHotSpotSettings),"System/MissionHotSpotSettings.xml",false,0xBD468ACCu,0xFED76CC8u),
        new("ModelSettings",typeof(Ep1StockModelSettings),sizeof(Ep1StockModelSettings),"System/ModelSettings.xml",true,0x7559AD25u,0x9536391Fu),
        new("OrdersManagerSettings",typeof(Ep1StockOrdersManagerSettings),sizeof(Ep1StockOrdersManagerSettings),"System/OrdersManagerSettings.xml",false,0xB29F0574u,0x80FCF268u),
        new("PathfindSettings",typeof(Ep1StockPathfindSettings),sizeof(Ep1StockPathfindSettings),"System/PathfindSettings.xml",true,0x39DB25F8u,0x9CBCF9F1u),
        new("ScorchMarkSettings",typeof(Ep1StockScorchMarkSettings),sizeof(Ep1StockScorchMarkSettings),"System/ScorchMarkSettings.xml",false,0x8E14B8C3u,0x698D0DFAu),
        new("SkirmishSettings",typeof(Ep1StockSkirmishSettings),sizeof(Ep1StockSkirmishSettings),"System/SkirmishSettings.xml",true,0x17F749CFu,0xE35DB55Bu),
        new("TerrainVisualSettings",typeof(Ep1StockTerrainVisualSettings),sizeof(Ep1StockTerrainVisualSettings),"System/TerrainVisualSettings.xml",true,0x4696A138u,0x4FAD3C13u),
        new("W3DDeviceSettings",typeof(Ep1StockW3DDeviceSettings),sizeof(Ep1StockW3DDeviceSettings),"System/W3DDeviceSettings.xml",true,0x57900170u,0x6ED8F3FFu),
        new("TerrainAssetCollection",typeof(Ep1StockTerrainAssetCollection),sizeof(Ep1StockTerrainAssetCollection),"Terrain/Terrain.xml",false,0x22B4748Bu,0x143CC65Fu),
        new("UIJoypadCommandBarButtonBuild",typeof(Ep1StockUIJoypadCommandBarButtonBuild),sizeof(Ep1StockUIJoypadCommandBarButtonBuild),"UI/Joypad/Settings.xml",false,0xCC13A3DAu,0x491C58E4u),
        new("UIJoypadCommandBarControlGroups",typeof(Ep1StockUIJoypadCommandBarControlGroups),sizeof(Ep1StockUIJoypadCommandBarControlGroups),"UI/Joypad/Settings.xml",false,0xD9456C84u,0xE5994135u),
        new("UIJoypadCommandBarHomogenousGroup",typeof(Ep1StockUIJoypadCommandBarHomogenousGroup),sizeof(Ep1StockUIJoypadCommandBarHomogenousGroup),"UI/Joypad/Settings.xml",false,0x5C34A41Cu,0x84A0BF03u),
        new("UIJoypadCommandBarMixedGroup",typeof(Ep1StockUIJoypadCommandBarMixedGroup),sizeof(Ep1StockUIJoypadCommandBarMixedGroup),"UI/Joypad/Settings.xml",false,0x4C1D000Eu,0xED6B4DFAu),
        new("UIJoypadCommandBarSingleUnit",typeof(Ep1StockUIJoypadCommandBarSingleUnit),sizeof(Ep1StockUIJoypadCommandBarSingleUnit),"UI/Joypad/Settings.xml",false,0xE88B2522u,0x6B910C6Bu),
        new("UIJoypadCommandBarStances",typeof(Ep1StockUIJoypadCommandBarStances),sizeof(Ep1StockUIJoypadCommandBarStances),"UI/Joypad/Settings.xml",false,0x70868814u,0x444E408Du),
        new("UIJoypadCommandBarCoopCommunicator",typeof(Ep1StockUIJoypadCommandBarCoopCommunicator),sizeof(Ep1StockUIJoypadCommandBarCoopCommunicator),"UI/Joypad/Settings.xml",false,0x629A93BBu,0xA903F1E2u),
        new("UIJoypadCommandBarPlayerPowers",typeof(Ep1StockUIJoypadCommandBarPlayerPowers),sizeof(Ep1StockUIJoypadCommandBarPlayerPowers),"UI/Joypad/Settings.xml",false,0xA37B9988u,0x687A641Fu),
        new("UIJoypadCommandBarTopMenu",typeof(Ep1StockUIJoypadCommandBarTopMenu),sizeof(Ep1StockUIJoypadCommandBarTopMenu),"UI/Joypad/Settings.xml",false,0x81583E8Bu,0x157D0484u),
        new("UIMouseTacticalQuickChatDisplay",typeof(Ep1StockUIMouseTacticalQuickChatDisplay),sizeof(Ep1StockUIMouseTacticalQuickChatDisplay),"UI/Mouse/QuickChatSettings.xml",false,0xAC5E9172u,0x81F357B9u),
        new("UIMouseDragSelectionFeedback",typeof(Ep1StockUIMouseDragSelectionFeedback),sizeof(Ep1StockUIMouseDragSelectionFeedback),"UI/Mouse/Settings.xml",false,0x246D0C65u,0x20C10A91u),
        new("UIMouseHUDElementHotKeyGate",typeof(Ep1StockUIMouseHUDElementHotKeyGate),sizeof(Ep1StockUIMouseHUDElementHotKeyGate),"UI/Mouse/Settings.xml",false,0xA336BCC2u,0xCD311E8Eu),
        new("UIMouseTacticalAttackMoveButton",typeof(Ep1StockUIMouseTacticalAttackMoveButton),sizeof(Ep1StockUIMouseTacticalAttackMoveButton),"UI/Mouse/Settings.xml",false,0x1A66FEFCu,0x56563ED8u),
        new("UIMouseTacticalBuildQueueTab",typeof(Ep1StockUIMouseTacticalBuildQueueTab),sizeof(Ep1StockUIMouseTacticalBuildQueueTab),"UI/Mouse/Settings.xml",false,0x75B6EC99u,0x107AD864u),
        new("UIMouseTacticalBuildQueueTypeTab",typeof(Ep1StockUIMouseTacticalBuildQueueTypeTab),sizeof(Ep1StockUIMouseTacticalBuildQueueTypeTab),"UI/Mouse/Settings.xml",false,0xF40E5021u,0xFD80A557u),
        new("UIMouseTacticalCommandAttack",typeof(Ep1StockUIMouseTacticalCommandAttack),sizeof(Ep1StockUIMouseTacticalCommandAttack),"UI/Mouse/Settings.xml",false,0xD3863433u,0x02BD5240u),
        new("UIMouseTacticalCommandAttackMove",typeof(Ep1StockUIMouseTacticalCommandAttackMove),sizeof(Ep1StockUIMouseTacticalCommandAttackMove),"UI/Mouse/Settings.xml",false,0xD522E6EDu,0xD2657960u),
        new("UIMouseTacticalCommandCommunicateWithAllies",typeof(Ep1StockUIMouseTacticalCommandCommunicateWithAllies),sizeof(Ep1StockUIMouseTacticalCommandCommunicateWithAllies),"UI/Mouse/Settings.xml",false,0x06BCB058u,0x62BEE2E7u),
        new("UIMouseTacticalCommandDock",typeof(Ep1StockUIMouseTacticalCommandDock),sizeof(Ep1StockUIMouseTacticalCommandDock),"UI/Mouse/Settings.xml",false,0xED0A75ACu,0xACDD2F29u),
        new("UIMouseTacticalCommandEnter",typeof(Ep1StockUIMouseTacticalCommandEnter),sizeof(Ep1StockUIMouseTacticalCommandEnter),"UI/Mouse/Settings.xml",false,0xB63667C0u,0x43646C6Au),
        new("UIMouseTacticalCommandForceAttack",typeof(Ep1StockUIMouseTacticalCommandForceAttack),sizeof(Ep1StockUIMouseTacticalCommandForceAttack),"UI/Mouse/Settings.xml",false,0x7BA13C8Du,0xD04699E4u),
        new("UIMouseTacticalCommandForceMove",typeof(Ep1StockUIMouseTacticalCommandForceMove),sizeof(Ep1StockUIMouseTacticalCommandForceMove),"UI/Mouse/Settings.xml",false,0xA0C9EC41u,0x07BF94D3u),
        new("UIMouseTacticalCommandGetCrate",typeof(Ep1StockUIMouseTacticalCommandGetCrate),sizeof(Ep1StockUIMouseTacticalCommandGetCrate),"UI/Mouse/Settings.xml",false,0x7B3F8F48u,0x8E7A0FA6u),
        new("UIMouseTacticalCommandMineOre",typeof(Ep1StockUIMouseTacticalCommandMineOre),sizeof(Ep1StockUIMouseTacticalCommandMineOre),"UI/Mouse/Settings.xml",false,0xB0415646u,0xB3DDA9E2u),
        new("UIMouseTacticalCommandMove",typeof(Ep1StockUIMouseTacticalCommandMove),sizeof(Ep1StockUIMouseTacticalCommandMove),"UI/Mouse/Settings.xml",false,0x4994C173u,0xC0F904A1u),
        new("UIMouseTacticalCommandPlaceAIBeacon",typeof(Ep1StockUIMouseTacticalCommandPlaceAIBeacon),sizeof(Ep1StockUIMouseTacticalCommandPlaceAIBeacon),"UI/Mouse/Settings.xml",false,0xE3159616u,0x24622FB8u),
        new("UIMouseTacticalCommandPlaceRallyPoint",typeof(Ep1StockUIMouseTacticalCommandPlaceRallyPoint),sizeof(Ep1StockUIMouseTacticalCommandPlaceRallyPoint),"UI/Mouse/Settings.xml",false,0x57E00896u,0x417E47CCu),
        new("UIMouseTacticalCommandReverseMove",typeof(Ep1StockUIMouseTacticalCommandReverseMove),sizeof(Ep1StockUIMouseTacticalCommandReverseMove),"UI/Mouse/Settings.xml",false,0xF00548C6u,0xE7D4DD88u),
        new("UIMouseTacticalCommandSelect",typeof(Ep1StockUIMouseTacticalCommandSelect),sizeof(Ep1StockUIMouseTacticalCommandSelect),"UI/Mouse/Settings.xml",false,0x4F8B8410u,0xDBE5398Fu),
        new("UIMouseTacticalCommandSell",typeof(Ep1StockUIMouseTacticalCommandSell),sizeof(Ep1StockUIMouseTacticalCommandSell),"UI/Mouse/Settings.xml",false,0x4A753C69u,0x4C34A74Cu),
        new("UIMouseTacticalCommandSetRallyPoint",typeof(Ep1StockUIMouseTacticalCommandSetRallyPoint),sizeof(Ep1StockUIMouseTacticalCommandSetRallyPoint),"UI/Mouse/Settings.xml",false,0xF92B2739u,0xCE71E899u),
        new("UIMouseTacticalCommandToggleSelfRepair",typeof(Ep1StockUIMouseTacticalCommandToggleSelfRepair),sizeof(Ep1StockUIMouseTacticalCommandToggleSelfRepair),"UI/Mouse/Settings.xml",false,0x749B5209u,0x295A4872u),
        new("UIMouseTacticalCoOpAIButtonsAssaultButton",typeof(Ep1StockUIMouseTacticalCoOpAIButtonsAssaultButton),sizeof(Ep1StockUIMouseTacticalCoOpAIButtonsAssaultButton),"UI/Mouse/Settings.xml",false,0xB873251Cu,0xE640A7F2u),
        new("UIMouseTacticalCoOpAIButtonsHelpButton",typeof(Ep1StockUIMouseTacticalCoOpAIButtonsHelpButton),sizeof(Ep1StockUIMouseTacticalCoOpAIButtonsHelpButton),"UI/Mouse/Settings.xml",false,0x0634D9CCu,0xBC5A9F70u),
        new("UIMouseTacticalCoOpAIButtonsHoldObjectButton",typeof(Ep1StockUIMouseTacticalCoOpAIButtonsHoldObjectButton),sizeof(Ep1StockUIMouseTacticalCoOpAIButtonsHoldObjectButton),"UI/Mouse/Settings.xml",false,0x2E0AD8B0u,0x3432F9F7u),
        new("UIMouseTacticalCoOpAIButtonsHoldPositionButton",typeof(Ep1StockUIMouseTacticalCoOpAIButtonsHoldPositionButton),sizeof(Ep1StockUIMouseTacticalCoOpAIButtonsHoldPositionButton),"UI/Mouse/Settings.xml",false,0xADB2367Fu,0x7390FFC7u),
        new("UIMouseTacticalCoOpAIButtonsNormalButton",typeof(Ep1StockUIMouseTacticalCoOpAIButtonsNormalButton),sizeof(Ep1StockUIMouseTacticalCoOpAIButtonsNormalButton),"UI/Mouse/Settings.xml",false,0x51C6B6A8u,0x11F96319u),
        new("UIMouseTacticalDeactivateMissionHotSpotsButton",typeof(Ep1StockUIMouseTacticalDeactivateMissionHotSpotsButton),sizeof(Ep1StockUIMouseTacticalDeactivateMissionHotSpotsButton),"UI/Mouse/Settings.xml",false,0x7A0F9658u,0xEAA83CD0u),
        new("UIMouseTacticalExitContainerButton",typeof(Ep1StockUIMouseTacticalExitContainerButton),sizeof(Ep1StockUIMouseTacticalExitContainerButton),"UI/Mouse/Settings.xml",false,0x3C70F027u,0xB0F771E4u),
        new("UIMouseTacticalFastForwardButton",typeof(Ep1StockUIMouseTacticalFastForwardButton),sizeof(Ep1StockUIMouseTacticalFastForwardButton),"UI/Mouse/Settings.xml",false,0xFC44C1C3u,0xBA95A6F2u),
        new("UIMouseTacticalForceAttackButton",typeof(Ep1StockUIMouseTacticalForceAttackButton),sizeof(Ep1StockUIMouseTacticalForceAttackButton),"UI/Mouse/Settings.xml",false,0xDFE29605u,0x089A2D65u),
        new("UIMouseTacticalForceAttackMoveButton",typeof(Ep1StockUIMouseTacticalForceAttackMoveButton),sizeof(Ep1StockUIMouseTacticalForceAttackMoveButton),"UI/Mouse/Settings.xml",false,0x296A6335u,0xF9DD687Du),
        new("UIMouseTacticalForceMoveButton",typeof(Ep1StockUIMouseTacticalForceMoveButton),sizeof(Ep1StockUIMouseTacticalForceMoveButton),"UI/Mouse/Settings.xml",false,0xDD834D41u,0x99E2501Fu),
        new("UIMouseTacticalGroupSelection",typeof(Ep1StockUIMouseTacticalGroupSelection),sizeof(Ep1StockUIMouseTacticalGroupSelection),"UI/Mouse/Settings.xml",false,0x0E626554u,0x4B91C129u),
        new("UIMouseTacticalLookAt",typeof(Ep1StockUIMouseTacticalLookAt),sizeof(Ep1StockUIMouseTacticalLookAt),"UI/Mouse/Settings.xml",false,0x6B00C87Du,0x63E9948Bu),
        new("UIMouseTacticalMainCommandBarHotKeys",typeof(Ep1StockUIMouseTacticalMainCommandBarHotKeys),sizeof(Ep1StockUIMouseTacticalMainCommandBarHotKeys),"UI/Mouse/Settings.xml",false,0x9A7A151Du,0x73E311A9u),
        new("UIMouseTacticalMiscHotKeys",typeof(Ep1StockUIMouseTacticalMiscHotKeys),sizeof(Ep1StockUIMouseTacticalMiscHotKeys),"UI/Mouse/Settings.xml",false,0x780C2500u,0xD346B6AFu),
        new("UIMouseTacticalOpenAdvancedCommandsButton",typeof(Ep1StockUIMouseTacticalOpenAdvancedCommandsButton),sizeof(Ep1StockUIMouseTacticalOpenAdvancedCommandsButton),"UI/Mouse/Settings.xml",false,0xA76A7A93u,0x8D934671u),
        new("UIMouseTacticalPauseButton",typeof(Ep1StockUIMouseTacticalPauseButton),sizeof(Ep1StockUIMouseTacticalPauseButton),"UI/Mouse/Settings.xml",false,0xCBD960ECu,0x3C06FAC3u),
        new("UIMouseTacticalPlanningModeButton",typeof(Ep1StockUIMouseTacticalPlanningModeButton),sizeof(Ep1StockUIMouseTacticalPlanningModeButton),"UI/Mouse/Settings.xml",false,0x87C8CD72u,0xEE0DA44Fu),
        new("UIMouseTacticalPlayerPowerHotKeys",typeof(Ep1StockUIMouseTacticalPlayerPowerHotKeys),sizeof(Ep1StockUIMouseTacticalPlayerPowerHotKeys),"UI/Mouse/Settings.xml",false,0x88230CCDu,0x8AA6BAD8u),
        new("UIMouseTacticalPlayerTechStoreButton",typeof(Ep1StockUIMouseTacticalPlayerTechStoreButton),sizeof(Ep1StockUIMouseTacticalPlayerTechStoreButton),"UI/Mouse/Settings.xml",false,0x43E16D66u,0x090A0AC3u),
        new("UIMouseTacticalRadarTelestrator",typeof(Ep1StockUIMouseTacticalRadarTelestrator),sizeof(Ep1StockUIMouseTacticalRadarTelestrator),"UI/Mouse/Settings.xml",false,0xC8AA398Eu,0xAB665329u),
        new("UIMouseTacticalRedAlertButton",typeof(UIMouseTacticalRedAlertButton),sizeof(UIMouseTacticalRedAlertButton),"UI/Mouse/Settings.xml",false,0x025F8452u,0x12C3CE32u),
        new("UIMouseTacticalReverseMoveButton",typeof(Ep1StockUIMouseTacticalReverseMoveButton),sizeof(Ep1StockUIMouseTacticalReverseMoveButton),"UI/Mouse/Settings.xml",false,0xB20D9681u,0x6F0D7455u),
        new("UIMouseTacticalSelectionDetailsButtonHotKeys",typeof(Ep1StockUIMouseTacticalSelectionDetailsButtonHotKeys),sizeof(Ep1StockUIMouseTacticalSelectionDetailsButtonHotKeys),"UI/Mouse/Settings.xml",false,0x4D44696Au,0x46C4AFE8u),
        new("UIMouseTacticalSelectionRefinementTab",typeof(Ep1StockUIMouseTacticalSelectionRefinementTab),sizeof(Ep1StockUIMouseTacticalSelectionRefinementTab),"UI/Mouse/Settings.xml",false,0x9D131288u,0xF15481F9u),
        new("UIMouseTacticalSellModeButton",typeof(Ep1StockUIMouseTacticalSellModeButton),sizeof(Ep1StockUIMouseTacticalSellModeButton),"UI/Mouse/Settings.xml",false,0xF4834051u,0xB2D81257u),
        new("UIMouseTacticalSetStanceButton",typeof(Ep1StockUIMouseTacticalSetStanceButton),sizeof(Ep1StockUIMouseTacticalSetStanceButton),"UI/Mouse/Settings.xml",false,0xAB4CFCD4u,0xF2B7A980u),
        new("UIMouseTacticalStopButton",typeof(Ep1StockUIMouseTacticalStopButton),sizeof(Ep1StockUIMouseTacticalStopButton),"UI/Mouse/Settings.xml",false,0xF6C9D597u,0xE76FF3ABu),
        new("UIMouseTacticalTelestrator",typeof(Ep1StockUIMouseTacticalTelestrator),sizeof(Ep1StockUIMouseTacticalTelestrator),"UI/Mouse/Settings.xml",false,0x728B3825u,0x36A0014Eu),
        new("UIMouseTacticalThreatMeter",typeof(Ep1StockUIMouseTacticalThreatMeter),sizeof(Ep1StockUIMouseTacticalThreatMeter),"UI/Mouse/Settings.xml",false,0xF70043B6u,0x1C30F868u),
        new("UIMouseTacticalToggleRepairModeButton",typeof(Ep1StockUIMouseTacticalToggleRepairModeButton),sizeof(Ep1StockUIMouseTacticalToggleRepairModeButton),"UI/Mouse/Settings.xml",false,0x14A0B8E1u,0x875245F7u),
        new("UIMouseTacticalUnitIntroButton",typeof(Ep1StockUIMouseTacticalUnitIntroButton),sizeof(Ep1StockUIMouseTacticalUnitIntroButton),"UI/Mouse/Settings.xml",false,0x4C1F24EEu,0xF0B72BA3u),
        new("UIMouseTacticalWaypointModeButton",typeof(Ep1StockUIMouseTacticalWaypointModeButton),sizeof(Ep1StockUIMouseTacticalWaypointModeButton),"UI/Mouse/Settings.xml",false,0xB05D5241u,0xAD5F905Fu),
        new("UIMouseVoiceChat",typeof(Ep1StockUIMouseVoiceChat),sizeof(Ep1StockUIMouseVoiceChat),"UI/Mouse/Settings.xml",false,0xDF496AB2u,0xB3EECC17u),
        new("UITimerDisplay",typeof(Ep1StockUITimerDisplay),sizeof(Ep1StockUITimerDisplay),"UI/Settings.xml",false,0x38AFAA14u,0x7C1AF1F2u),
        new("UIWorldBuilderDecalGrid",typeof(Ep1StockUIWorldBuilderDecalGrid),sizeof(Ep1StockUIWorldBuilderDecalGrid),"UI/Settings.xml",false,0x49D519AFu,0x5793F574u),
        new("UICommandButton",typeof(Ep1StockUICommandButton),sizeof(Ep1StockUICommandButton),"UI/SettingsCommon.xml",false,0x08080BC0u,0x89D7EA91u),
        new("UICoopTinting",typeof(Ep1StockUICoopTinting),sizeof(Ep1StockUICoopTinting),"UI/SettingsCommon.xml",false,0xFCF3509Au,0x054F5ACEu),
        new("UIDecalCloud",typeof(Ep1StockUIDecalCloud),sizeof(Ep1StockUIDecalCloud),"UI/SettingsCommon.xml",false,0x967ED1DCu,0x1363FA2Bu),
        new("UIDecalGrid",typeof(Ep1StockUIDecalGrid),sizeof(Ep1StockUIDecalGrid),"UI/SettingsCommon.xml",false,0x9AEFFBAFu,0x003E27FFu),
        new("UIFloatingTextDisplay",typeof(Ep1StockUIFloatingTextDisplay),sizeof(Ep1StockUIFloatingTextDisplay),"UI/SettingsCommon.xml",false,0x5CFAC0D8u,0x8700DB82u),
        new("UIMessageDisplay",typeof(Ep1StockUIMessageDisplay),sizeof(Ep1StockUIMessageDisplay),"UI/SettingsCommon.xml",false,0x652AF19Du,0xD6D6EFAAu),
        new("UIRadarObjectsLayer",typeof(Ep1StockUIRadarObjectsLayer),sizeof(Ep1StockUIRadarObjectsLayer),"UI/SettingsCommon.xml",false,0xF0C74A4Au,0x5F4AA6C5u),
        new("UIScenarioMapPreview",typeof(UIScenarioMapPreview),sizeof(UIScenarioMapPreview),"UI/SettingsCommon.xml",false,0xC3ACC6B6u,0x8CD9F2DCu),
        new("UISubtitleDisplay",typeof(Ep1StockUISubtitleDisplay),sizeof(Ep1StockUISubtitleDisplay),"UI/SettingsCommon.xml",false,0x09DF0972u,0x434C6A31u),

        // Reborn: dynamic music transitions, state sets and conditions require original global source/native witnesses.
        new("PathMusicGameDynamicTransition",typeof(Ep1PathMusicGameDynamicTransition),12,"PathMusic/StandardPathMusicGameDynamicState.xml",true,0x8BACD634u,0x82F0728Eu),
        new("PathMusicGameDynamicStateSet",typeof(Ep1PathMusicGameDynamicStateSet),16,"PathMusic/StandardPathMusicGameDynamicState.xml",true,0x147335B2u,0x511D9D37u),
        new("MusicScriptConditionNugget_PathMusicStateLastActive",typeof(Ep1MusicScriptConditionNugget_PathMusicStateLastActive),20,"PathMusic/StandardPathMusicGameDynamicState.xml",true,0xAD983E7Eu,0x8C0AB66Fu),
        new("MusicScriptConditionNugget_PathMusicStateCurrentlyActive",typeof(Ep1MusicScriptConditionNugget_PathMusicStateCurrentlyActive),8,"PathMusic/ScriptedPathMusicGameDynamicState.xml",true,0xDE279056u,0xC9D7C134u),
        new("MusicScriptConditionNugget_EngagementCount",typeof(Ep1MusicScriptConditionNugget_EngagementCount),28,"PathMusic/StandardPathMusicGameDynamicState.xml",true,0xCF359C6Au,0xB5CBC912u),
        new("MusicScriptConditionNugget_LocalPlayerKillRatio",typeof(Ep1MusicScriptConditionNugget_LocalPlayerKillRatio),20,"PathMusic/StandardPathMusicGameDynamicState.xml",true,0xA80607E3u,0xC5676FEDu),
        new("MusicScriptConditionNugget_HitBySuperWeapon",typeof(Ep1MusicScriptConditionNugget_HitBySuperWeapon),24,"PathMusic/StandardPathMusicGameDynamicState.xml",true,0x6167B6A9u,0x243CC246u),
        new("MusicScriptConditionNugget_And",typeof(MusicScriptConditionNugget_And),12,"PathMusic/StandardPathMusicGameDynamicState.xml",true,0x5016F8C3u,0x1FBAD33Fu),
        new("MusicScriptConditionNugget_Or",typeof(MusicScriptConditionNugget_Or),12,"PathMusic/StandardPathMusicGameDynamicState.xml",true,0x88011B0Eu,0xF2E2E91Du),
        new("MusicScriptConditionNugget_Not",typeof(MusicScriptConditionNugget_Not),8,"PathMusic/StandardPathMusicGameDynamicState.xml",true,0xEED6A240u,0x33055A8Eu),
        new("MusicScriptConditionNugget_ScoredKillCount",typeof(MusicScriptConditionNugget_ScoredKillCount),20,"PathMusic/StandardPathMusicGameDynamicState.xml",true,0xD7D07964u,0xC2BE8D14u),
        new("MusicScriptConditionNugget_ObjectTypesInProximity",typeof(MusicScriptConditionNugget_ObjectTypesInProximity),28,"PathMusic/EP1_S01_PathMusicGameDynamicState.xml",true,0x4C574B2Cu,0x24354307u),
        new("MusicScriptConditionNugget_LocalPlayerHitRedAlertButton",typeof(MusicScriptConditionNugget_LocalPlayerHitRedAlertButton),8,"PathMusic/StandardPathMusicGameDynamicState.xml",true,0x9E4D0625u,0xFD517A3Du),

        // Reborn: all four original stance vectors use the static 12-byte header and original EP1 fingerprint.
        new("StanceTemplate",typeof(StanceTemplate),12,"GlobalData/Stances.xml",false,0x9684C743u,0x4829F362u),
        // Reborn: all five original icon animation sequences require their real static fingerprint and source-sized image vectors.
        new("ImageSequence",typeof(ImageSequence),12,"GlobalData/ImageSequences.xml",false,0x21BA45A7u,0xA1FBBAF4u),
        // Reborn: original global micromanagers retain their complete 176-byte header and actual EP1 fingerprint.
        new("AIMicroManagerData",typeof(Ep1AIMicroManagerData),176,"SkirmishAI/AIMicroManagerLibrary.xml",true,0x9077F377u,0x5960CDD4u),
        // Reborn: personality records use the complete original 520-byte global header and real EP1 fingerprint.
        new("AIPersonalityDefinition",typeof(Ep1AIPersonalityDefinition),520,"SkirmishAI/Personalities/BasePersonality.xml",true,0xD6D4F18Eu,0xC6B1A12Fu),
        // Reborn: strategic states require the complete original state/tactic/team ABI and global native fingerprint.
        new("AIStrategicStateDefinition",typeof(Ep1AIStrategicStateDefinition),64,"SkirmishAI/States/JapanBaseStates.xml",true,0x242FF6D4u,0xC04C76A1u),
        // Reborn: build-state entries retain their complete original global EP1 ABI and native fingerprint.
        new("AIBuildStateDefinition",typeof(Ep1AIBuildStateDefinition),20,"SkirmishAI/States/JapanBaseStates.xml",true,0x620C480Bu,0x835BC3A7u),
        // Reborn: budgets require the complete original EP1 heuristic graph and bank-account shares.
        new("AIBudgetStateDefinition",typeof(Ep1AIBudgetStateDefinition),20,"SkirmishAI/States/AIBudgetStateLibrary.xml",true,0xAC78CE63u,0xC4384827u),
        // Reborn: radius cursors require the complete original static decal-component ABI.
        new("RadiusCursor",typeof(Ep1RadiusCursor),12,"UI/RadiusCursors.xml",false,0x2078A161u,0xA0AF5530u),
        // Reborn: colors, UI fonts and shader override rules retain their independently observed EP1 fingerprints.
        new("MultiplayerColor",typeof(Ep1MultiplayerColor),28,"GlobalData/MultiplayerColors.xml",true,0x8E28081Du,0x131C20A0u),
        new("HeaderTemplate",typeof(Ep1HeaderTemplate),20,"System/HeaderTemplates.xml",true,0x48A3EEC6u,0x401FF46Bu),
        new("ShaderOverride",typeof(ShaderOverride),16,"GlobalData/ShaderOverrides.xml",false,0xBCC23F6Cu,0x3D5B1D16u),

        // Reborn: registries and rank grants have distinct original static/global native contracts.
        new("RegisteredObjectSet",typeof(Ep1RegisteredObjectSet),132,"GlobalData/RegisteredObjectSets.xml",false,0x3090CD40u,0x16B5ECC2u),
        new("PlayerTechRank",typeof(Ep1PlayerTechRank),20,"System/PlayerTechRanks.xml",true,0x6B7227C2u,0x12271BCEu),

        // Reborn: original emotion templates use their complete static 324-byte native header.
        new("EmotionTemplate",typeof(Ep1EmotionTemplate),324,"GlobalData/EmotionTemplates.xml",false,0xA9152D94u,0x3869C1A5u),
        // Reborn: all formation roots retain their original global fingerprints and variable nested lists.
        new("FormationAssistantUnitDefinition",typeof(Ep1FormationAssistantUnitDefinition),24,"GlobalData/FormationAssistant.xml",true,0x31F4DCE2u,0x5CD9BEA2u),
        new("FormationAssistantTemplate",typeof(Ep1FormationAssistantTemplate),12,"GlobalData/FormationAssistant.xml",true,0xAD9779B6u,0x09539ECBu),
        new("FormationSelectionTemplate",typeof(Ep1FormationSelectionTemplate),16,"GlobalData/FormationAssistant.xml",true,0xC92313E0u,0x05318E41u),
        new("FormationAssistant",typeof(Ep1FormationAssistant),40,"GlobalData/FormationAssistant.xml",true,0x7504005Cu,0x162572CCu),

        // Reborn: input singletons retain their actual static/global placement and native fingerprints.
        new("GeneralHotKeySlots",typeof(Ep1GeneralHotKeySlots),12,"GlobalData/GeneralHotKeySlots.xml",false,0x18DD9C43u,0x2710C7A0u),
        new("BasicMouseCursors",typeof(Ep1BasicMouseCursors),20,"GlobalData/BasicMouseCursors.xml",true,0x246F4433u,0x5A796288u),

        // Reborn: the EP1 singleton keeps all one hundred original audio slots and its real global fingerprint.
        new("MiscAudio",typeof(Ep1MiscAudio),404,"Sounds/MiscAudio.xml",true,0x7B6AE7D5u,0x28FADC88u),
        // Reborn: loading-screen strings, pools and per-map sequences retain distinct original global layouts.
        new("EngagingContent",typeof(Ep1EngagingContent),28,"GlobalData/EngagingContentCommon.xml",true,0xB6472112u,0x2B3B15E0u),
        new("EngagingContentGroup",typeof(Ep1EngagingContentGroup),12,"GlobalData/EngagingContentCommon.xml",true,0x00AC224Eu,0x59E0AC87u),
        new("EngagingContentSequence",typeof(Ep1EngagingContentSequence),20,"GlobalData/EngagingContentCommon.xml",true,0xE279D385u,0x0D81F33Du),
        // Reborn: cursor, font, vibration, mouse and sound settings retain distinct native fingerprints.
        new("MouseCursor",typeof(Ep1MouseCursor),16,"GlobalData/MouseCursors.xml",true,0xE1920287u,0x13ED4B89u),
        new("FontTranslation",typeof(Ep1FontTranslation),20,"GlobalData/FontTranslation.xml",true,0x4E9436E2u,0x6C315185u),
        new("FontDirectory",typeof(Ep1FontDirectory),12,"GlobalData/FontDirectory.xml",true,0xD4BABAC7u,0xD7993143u),
        new("Mouse",typeof(Ep1Mouse),20,"GlobalData/Mouse.xml",false,0x9687F57Bu,0x4A535D4Bu),
        new("ForceFeedbackEvent",typeof(Ep1ForceFeedbackEvent),20,"GlobalData/ForceFeedbackEvents.xml",false,0x1C48076Fu,0xE1FE71E8u),
        new("AudioEventLimitGroup",typeof(Ep1AudioEventLimitGroup),8,"Sounds/AudioEventLimitGroup.xml",true,0xE8D44EF5u,0x95314DEAu),
        new("AnimationSoundSettings",typeof(Ep1AnimationSoundSettings),8,"Sounds/AnimationSoundSettings.xml",true,0x4D9B7C79u,0xBCF803D4u),
        // Reborn: complete global AIData uses its original 220-byte native header and three tracked tree names.
        new("AIData",typeof(Ep1AIData),220,"SkirmishAI/AIData.xml",true,0xE4266B40u,0x4123A1AAu),
        // Reborn: Red Alert effects remain linked strong imports rather than copied modifier payloads.
        new("RedAlertTemplate",typeof(Ep1RedAlertTemplate),96,"GlobalData/RedAlerts.xml",false,0xF6D753D1u,0x49F4D026u),
        // Reborn: the original global targeting heuristics use the complete 212-byte EP1 record.
        new("AITargetingHeuristic",typeof(Ep1AITargetingHeuristic),212,"SkirmishAI/AITargetHeuristicLibrary.xml",true,0x373E10FAu,0x0EEA25F7u),
        // Reborn: retail static carries unit-intro voices and invisibility templates.
        new("UnitIntro",typeof(Ep1UnitIntro),12,"GlobalData/UnitIntros.xml",false,0x6F78B1A4u,0x570806D2u),
        new("InvisibilityTemplate",typeof(Ep1InvisibilityTemplate),252,"GlobalData/InvisibilityTemplates.xml",false,0x554FC288u,0xCCD315FFu),
        // Reborn: EVA roots are global; original terrain road witnesses are retained in the editor catalog.
        new("EvaSettings",typeof(Ep1EvaSettings),20,"Sounds/Eva.xml",true,0x8CC42BF2u,0xAFAD556Cu),
        new("EvaEvent",typeof(Ep1EvaEvent),44,"Sounds/Eva.xml",true,0x1553B19Eu,0x70A64BEAu),
        new("Road",typeof(Road),24,"GlobalData/Roads.xml",false,0xD19D90C6u,0x2BFD2964u,WorldBuilder:true),
        // Reborn: strategic-name imports and opening orders are distinct witnessed EP1 global roots.
        new("AIStrategicStateName",typeof(Ep1StrategicStateName),16,"SkirmishAI/States/AIStrategicStateNames.xml",true,0x77D6BE90u,0x048CE7A4u),
        new("SkirmishOpeningMove",typeof(SkirmishOpeningMove),20,"SkirmishAI/OpeningMoves/SovietOpeningMoves.xml",true,0xECBE73E8u,0x6FF5D680u),
        new("HotKeySlot",typeof(Ep1HotKeySlot),24,"GlobalData/HotKeySlots.xml",true,0xA6E6BBA7u,0x93E23672u),
        new("MappableKey",typeof(MappableKey),12,"GlobalData/MappableKeys.xml",true,0x5080A5D8u,0x84B6AEC1u),
        new("UIQuickChatMessage",typeof(Ep1UIQuickChatMessage),16,"UI/QuickChatMessages.xml",false,0xF4491BA9u,0x2C498C12u)};
    internal static bool Supports(string name)=>Families.Any(f=>f.Name==name);
    internal static Family For(string name)=>Families.Single(f=>f.Name==name);

    //-------------------------------------------------------------------------------------------------
    /** Reborn: prove every distinct authored group/optional-audio shape against its actual original native record before admission. */
    //-------------------------------------------------------------------------------------------------
    internal static void Qualify(Family family,string manifest,string global,SchemaSet schemas,ManifestAsset[] available)
    {
        // Reborn: the recorded PC factory/tagged-field observations are a separate proof kind from original asset bytes.
        if(family.EngineLayout){StockRuntimeAbiContract.Qualify(family.Name,family.Type,family.TypeId,family.TypeHash);return;}
        if(Marshal.SizeOf(family.Type)!=family.Size)throw new InvalidDataException("EP1 asset root layout differs: "+family.Name);
        // Reborn: streamed audio filenames resolve against the separate real AudioFile catalog, never invented sample declarations.
        if(family.Name is "AmbientStream" or "MusicTrack" or "DialogEvent")
        {
            var audio=ManifestReader.Read(SdkEnvironmentPreflight.Read(AuthoredAmbientCompiler.CatalogPath(),64*1048576));
            TypeRegistryAudit.ValidateTarget(audio.Header.Version,audio.Header.AllTypesHash);
            available=available.Concat(audio.Assets).DistinctBy(a=>new AssetId(a.TypeId,a.InstanceId)).ToArray();
        }
        string path=family.Locale?(StockLocaleCatalog.Find()??throw new FileNotFoundException("FontSubstitution requires the original SDK locale stream.")):family.WorldBuilder?WorldBuilderAssetResolver.CatalogPath():family.Global?global:manifest;
        var catalog=ManifestReader.Read(SdkEnvironmentPreflight.Read(path,64*1048576));
        // Reborn: the independent witness must be a valid linked EP1 catalog, including editor-only witnesses.
        TypeRegistryAudit.ValidateTarget(catalog.Header.Version,catalog.Header.AllTypesHash);
        if(!catalog.Header.IsLinked||catalog.Validate().Count!=0)throw new InvalidDataException("Invalid original asset-root witness catalog.");
        var examples=catalog.Assets.Where(a=>a.TypeName==family.Name).ToArray();
        if(examples.Length==0||examples.Any(a=>a.Tokenized!=0||a.TypeId!=family.TypeId||a.TypeHash!=family.TypeHash))throw new InvalidDataException("EP1 asset root fingerprint differs: "+family.Name);
        var originals=(family.Name is "UIConfigList" or "AudioSettings" or "AmbientStream" or "MusicTrack" or "DialogEvent"?StockOriginalUiConfig.Load(family.Source,family.Name):GameObjectExtendedRootContract.Original(family.Source,family.Name))
            .Where(e=>examples.Any(a=>a.Name==family.Name+":"+e.GetAttribute("id")))
            .Where(e=>!e.OuterXml.Contains("=$",StringComparison.Ordinal)&&!e.HasAttribute("inheritFrom")).ToArray();
        if(originals.Length==0)throw new InvalidDataException("No original source-backed root witness: "+family.Name);
        // Reborn: editor-only witness textures are allowed only in this original proof, never injected into runtime resolution.
        if(family.WorldBuilder){var known=available.Select(a=>new AssetId(a.TypeId,a.InstanceId)).ToHashSet();available=available.Concat(catalog.Assets.Where(a=>!known.Contains(new(a.TypeId,a.InstanceId)))).ToArray();}
        // Reborn: the base witness has no delay entries; prove populated inherited list records independently as well.
        if(family.Name=="AIMicroManagerData")foreach(string id in new[]{"StandardMicroManager_MEDIUM","StandardMicroManager_EASY","NoRiotShieldTest"})
        {
            var original=GameObjectExtendedRootContract.Original(family.Source,family.Name).Single(e=>e.GetAttribute("id")==id);
            var entries=original.ChildNodes.OfType<XmlElement>().Where(e=>e.LocalName=="SpecificSpecialPowerDelay"&&e.NamespaceURI==SchemaSet.XmlNamespace).ToArray();
            var owner=examples.Single(a=>a.Name==family.Name+":"+id);var bytes=W3DContainerCompiler.ReadStock(path,catalog,owner);
            uint count=System.Buffers.Binary.BinaryPrimitives.ReadUInt32LittleEndian(bytes.AsSpan(144,4)),pointer=System.Buffers.Binary.BinaryPrimitives.ReadUInt32LittleEndian(bytes.AsSpan(148,4));
            if(count!=entries.Length||count!=1||pointer>bytes.Length-16)throw new InvalidDataException("Original micro-manager delay list extent differs.");
            var probe=SdkXmlDiagnostics.Parse(Encoding.UTF8.GetBytes("<AIMicroManagerData xmlns=\""+SchemaSet.XmlNamespace+"\" id=\"RebornMicroDelayWitness\"/>"));
            probe.DocumentElement!.AppendChild(probe.ImportNode(entries[0],true));var proof=SchemaNativeContract.Resolve(probe,schemas,available).GetElementsByTagName("SpecificSpecialPowerDelay",SchemaSet.XmlNamespace)[0] as XmlElement;
            SchemaNativeContract.VerifyStock(proof!,typeof(Ep1AISpecificSpecialPowerDelayInfo),0,bytes,checked((int)pointer),owner.References.ToArray(),required:true);
        }
        foreach(var original in originals)
        {
            string name=family.Name+":"+original.GetAttribute("id");
            var asset=examples.Single(a=>a.Name==name);
            var source=SdkXmlDiagnostics.Parse(Encoding.UTF8.GetBytes(original.OuterXml));
            Normalize(source,schemas.Schemas);var proof=SchemaNativeContract.Resolve(source,schemas,available);proof.DocumentElement!.RemoveAttribute("id");
            SchemaNativeContract.VerifyStock(proof.DocumentElement,family.Type,0,W3DContainerCompiler.ReadStock(path,catalog,asset),0,asset.References.ToArray(),required:true);
        }
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: remove indentation-only character nodes from reviewed complex asset roots with no element children. */
    //-------------------------------------------------------------------------------------------------
    internal static void NormalizeFormatting(XmlElement root,System.Xml.Schema.XmlSchemaSet schemas)
    {
        // Reborn: preserve real text content and remove indentation only where the original schema declares an empty complex record.
        if(Supports(root.LocalName)&&root.NamespaceURI==SchemaSet.XmlNamespace)StockRootFormatting.Normalize(root,schemas);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: EA's BuildingPlacementDistance list is explicitly sorted by Distance in the original XSD. */
    //-------------------------------------------------------------------------------------------------
    internal static void Normalize(XmlDocument source,System.Xml.Schema.XmlSchemaSet schemas)
    {
        var root=source.DocumentElement!;NormalizeFormatting(root,schemas);if(root.LocalName!="AITargetingHeuristic")return;
        var distances=root.ChildNodes.OfType<XmlElement>().Where(e=>e.LocalName=="BuildingPlacementDistance"&&e.NamespaceURI==SchemaSet.XmlNamespace).ToArray();
        var sorted=distances.OrderBy(e=>XmlConvert.ToSingle(e.GetAttribute("Distance"))).ToArray();
        if(sorted.SequenceEqual(distances))return;
        foreach(var item in distances)root.RemoveChild(item);foreach(var item in sorted)root.AppendChild(item);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: allocate a source-sized native root through its explicitly selected typed writer and track all auxiliary storage. */
    //-------------------------------------------------------------------------------------------------
    internal static unsafe Chunk Compile(XmlNode source,System.Xml.Schema.XmlSchemaSet schemas)
    {
        var family=For(source.LocalName);
        var xml=SdkXmlDiagnostics.Parse(Encoding.UTF8.GetBytes(source.OuterXml));
        Normalize(xml,schemas);
        // Reborn: original settings can own polymorphic shader constants; schema-derived IDs belong only to the disposable writer clone.
        var constantLists=xml.GetElementsByTagName("Constants",SchemaSet.XmlNamespace).OfType<XmlElement>().ToArray();
        if(constantLists.Length!=0)
        {
            xml.Schemas.Add(schemas);xml.Validate((_,error)=>throw SdkXmlDiagnostics.SchemaError(error));
            foreach(var constants in constantLists)
            foreach(var constant in constants.ChildNodes.OfType<XmlElement>())
            {
                uint wire=BinaryAssetBuilder.Core.Hashing.FastHash.GetHashCode(constant.SchemaInfo.SchemaType!.QualifiedName.Name);
                _=SchemaNativeContract.ShaderConstantType(wire);constant.SetAttribute("TypeId",XmlConvert.ToString(wire));
            }
        }

        // Reborn: component IDs are derived from the explicit source class allowlist only for the private typed writer.
        if(family.Name=="UIConfigList")StockUIComponentCatalog.PrepareWriter(xml.DocumentElement!);
        // Reborn: heuristic discriminators are private writer metadata; the admitted source and proof XML remain unchanged.
        if(family.Name is "AIBudgetStateDefinition" or "AIBuildStateDefinition" or "AIStrategicStateDefinition" or "AIPersonalityDefinition")BinaryAssetBuilder.XmlCompiler.Ep1AIHeuristicLayout.PrepareWriter(xml.DocumentElement!);
        var namespaces=new XmlNamespaceManager(xml.NameTable);namespaces.AddNamespace("ea",SchemaSet.XmlNamespace);
        var node=new Node(xml.DocumentElement!.CreateNavigator()!,namespaces);
        void* root;using var tracker=new Tracker(&root,(uint)family.Size,false);
        switch(family.Name)
        {
            case "PathMusicGameDynamicTransition":Marshaler.Marshal(node,(Ep1PathMusicGameDynamicTransition*)root,tracker);break;
            case "PathMusicGameDynamicStateSet":Marshaler.Marshal(node,(Ep1PathMusicGameDynamicStateSet*)root,tracker);break;
            case "MusicScriptConditionNugget_PathMusicStateLastActive":Marshaler.Marshal(node,(Ep1MusicScriptConditionNugget_PathMusicStateLastActive*)root,tracker);break;
            case "MusicScriptConditionNugget_PathMusicStateCurrentlyActive":Marshaler.Marshal(node,(Ep1MusicScriptConditionNugget_PathMusicStateCurrentlyActive*)root,tracker);break;
            case "MusicScriptConditionNugget_EngagementCount":Marshaler.Marshal(node,(Ep1MusicScriptConditionNugget_EngagementCount*)root,tracker);break;
            case "MusicScriptConditionNugget_LocalPlayerKillRatio":Marshaler.Marshal(node,(Ep1MusicScriptConditionNugget_LocalPlayerKillRatio*)root,tracker);break;
            case "MusicScriptConditionNugget_HitBySuperWeapon":Marshaler.Marshal(node,(Ep1MusicScriptConditionNugget_HitBySuperWeapon*)root,tracker);break;
            case "MusicScriptConditionNugget_And":Marshaler.Marshal(node,(MusicScriptConditionNugget_And*)root,tracker);break;
            case "MusicScriptConditionNugget_Or":Marshaler.Marshal(node,(MusicScriptConditionNugget_Or*)root,tracker);break;
            case "MusicScriptConditionNugget_Not":Marshaler.Marshal(node,(MusicScriptConditionNugget_Not*)root,tracker);break;
            case "MusicScriptConditionNugget_ScoredKillCount":Marshaler.Marshal(node,(MusicScriptConditionNugget_ScoredKillCount*)root,tracker);break;
            case "MusicScriptConditionNugget_ObjectTypesInProximity":Marshaler.Marshal(node,(MusicScriptConditionNugget_ObjectTypesInProximity*)root,tracker);break;
            case "MusicScriptConditionNugget_LocalPlayerHitRedAlertButton":Marshaler.Marshal(node,(MusicScriptConditionNugget_LocalPlayerHitRedAlertButton*)root,tracker);break;
            case "JoypadCommandBarTemplate":Marshaler.Marshal(node,(Ep1StockJoypadCommandBarTemplate*)root,tracker);break;
            case "DamageFX":Marshaler.Marshal(node,(Ep1StockDamageFX*)root,tracker);break;
            case "WaterTextureList":Marshaler.Marshal(node,(Ep1StockWaterTextureList*)root,tracker);break;
            case "MainMenuPersonalityTemplate":Marshaler.Marshal(node,(MainMenuPersonalityTemplate*)root,tracker);break;
            case "StaticGameLOD":Marshaler.Marshal(node,(Ep1StockStaticGameLOD*)root,tracker);break;
            case "DynamicGameLOD":Marshaler.Marshal(node,(DynamicGameLOD*)root,tracker);break;
            case "TargetingCompareList":Marshaler.Marshal(node,(TargetingCompareList*)root,tracker);break;
            case "WaterSetting":Marshaler.Marshal(node,(Ep1StockWaterSetting*)root,tracker);break;
            case "GameLODPreset":Marshaler.Marshal(node,(Ep1StockGameLODPreset*)root,tracker);break;
            case "TerrainParticleMapping":Marshaler.Marshal(node,(Ep1StockTerrainParticleMapping*)root,tracker);break;
            case "FontDefaultSetting":Marshaler.Marshal(node,(Ep1StockFontDefaultSetting*)root,tracker);break;
            case "ArmyDefinition":Marshaler.Marshal(node,(Ep1StockArmyDefinition*)root,tracker);break;
            case "LargeGroupAudioMap":Marshaler.Marshal(node,(LargeGroupAudioMap*)root,tracker);break;
            case "AudioLOD":Marshaler.Marshal(node,(AudioLOD*)root,tracker);break;
            case "ConnectionLineManager":Marshaler.Marshal(node,(ConnectionLineManager*)root,tracker);break;
            case "DebugHotKeys":Marshaler.Marshal(node,(Ep1StockDebugHotKeys*)root,tracker);break;
            case "DefaultHotKeys":Marshaler.Marshal(node,(DefaultHotKeys*)root,tracker);break;
            case "DrawGroupInfo":Marshaler.Marshal(node,(Ep1StockDrawGroupInfo*)root,tracker);break;
            case "Environment":Marshaler.Marshal(node,(Environment*)root,tracker);break;
            case "ShadowMap":Marshaler.Marshal(node,(ShadowMap*)root,tracker);break;
            case "Language":Marshaler.Marshal(node,(Ep1StockLanguage*)root,tracker);break;
            case "LocalBuildListMonitor":Marshaler.Marshal(node,(LocalBuildListMonitor*)root,tracker);break;
            case "MeshInstanceTemplate":Marshaler.Marshal(node,(Ep1StockMeshInstanceTemplate*)root,tracker);break;
            case "MultiplayerSettings":Marshaler.Marshal(node,(MultiplayerSettings*)root,tracker);break;
            case "OnlineChatColors":Marshaler.Marshal(node,(OnlineChatColors*)root,tracker);break;
            case "ProjectilePath":Marshaler.Marshal(node,(ProjectilePath*)root,tracker);break;
            case "ScenarioManagerData":Marshaler.Marshal(node,(ScenarioManagerData*)root,tracker);break;
            case "TargetingDistanceCompare":Marshaler.Marshal(node,(TargetingDistanceCompare*)root,tracker);break;
            case "TargetingCombatChainCompare":Marshaler.Marshal(node,(TargetingCombatChainCompare*)root,tracker);break;
            case "TargetingInTurretArcCompare":Marshaler.Marshal(node,(TargetingInTurretArcCompare*)root,tracker);break;
            case "UnitOverlayIconSettings":Marshaler.Marshal(node,(Ep1StockUnitOverlayIconSettings*)root,tracker);break;
            case "WaterTransparency":Marshaler.Marshal(node,(Ep1StockWaterTransparency*)root,tracker);break;
            case "YurikoHotKeys":Marshaler.Marshal(node,(YurikoHotKeys*)root,tracker);break;
            case "IntelDB":Marshaler.Marshal(node,(IntelDB*)root,tracker);break;
            case "NetworkSettings":Marshaler.Marshal(node,(Ep1StockNetworkSettings*)root,tracker);break;
            case "MainMenuPersonalityGroup":Marshaler.Marshal(node,(MainMenuPersonalityGroup*)root,tracker);break;
            case "MiscEvaEvents":Marshaler.Marshal(node,(Ep1StockMiscEvaEvents*)root,tracker);break;
            case "MusicPalette":Marshaler.Marshal(node,(MusicPalette*)root,tracker);break;
            case "AutoParticleSystemSettings":Marshaler.Marshal(node,(Ep1StockAutoParticleSystemSettings*)root,tracker);break;
            case "BreezeInfo":Marshaler.Marshal(node,(Ep1StockBreezeInfo*)root,tracker);break;
            case "BuildSystemSettings":Marshaler.Marshal(node,(Ep1StockBuildSystemSettings*)root,tracker);break;
            case "DamageFXSettings":Marshaler.Marshal(node,(Ep1StockDamageFXSettings*)root,tracker);break;
            case "DynamicsContactTagMapping":Marshaler.Marshal(node,(Ep1StockDynamicsContactTagMapping*)root,tracker);break;
            case "GraphicsHardware":Marshaler.Marshal(node,(Ep1StockGraphicsHardware*)root,tracker);break;
            case "InvisibilityManagerSettings":Marshaler.Marshal(node,(Ep1StockInvisibilityManagerSettings*)root,tracker);break;
            case "LightingSettings":Marshaler.Marshal(node,(Ep1StockLightingSettings*)root,tracker);break;
            case "MissionHotSpotSettings":Marshaler.Marshal(node,(Ep1StockMissionHotSpotSettings*)root,tracker);break;
            case "ModelSettings":Marshaler.Marshal(node,(Ep1StockModelSettings*)root,tracker);break;
            case "OrdersManagerSettings":Marshaler.Marshal(node,(Ep1StockOrdersManagerSettings*)root,tracker);break;
            case "PathfindSettings":Marshaler.Marshal(node,(Ep1StockPathfindSettings*)root,tracker);break;
            case "ScorchMarkSettings":Marshaler.Marshal(node,(Ep1StockScorchMarkSettings*)root,tracker);break;
            case "SkirmishSettings":Marshaler.Marshal(node,(Ep1StockSkirmishSettings*)root,tracker);break;
            case "TerrainVisualSettings":Marshaler.Marshal(node,(Ep1StockTerrainVisualSettings*)root,tracker);break;
            case "W3DDeviceSettings":Marshaler.Marshal(node,(Ep1StockW3DDeviceSettings*)root,tracker);break;
            case "TerrainAssetCollection":Marshaler.Marshal(node,(Ep1StockTerrainAssetCollection*)root,tracker);break;
            case "UIJoypadCommandBarButtonBuild":Marshaler.Marshal(node,(Ep1StockUIJoypadCommandBarButtonBuild*)root,tracker);break;
            case "UIJoypadCommandBarControlGroups":Marshaler.Marshal(node,(Ep1StockUIJoypadCommandBarControlGroups*)root,tracker);break;
            case "UIJoypadCommandBarHomogenousGroup":Marshaler.Marshal(node,(Ep1StockUIJoypadCommandBarHomogenousGroup*)root,tracker);break;
            case "UIJoypadCommandBarMixedGroup":Marshaler.Marshal(node,(Ep1StockUIJoypadCommandBarMixedGroup*)root,tracker);break;
            case "UIJoypadCommandBarSingleUnit":Marshaler.Marshal(node,(Ep1StockUIJoypadCommandBarSingleUnit*)root,tracker);break;
            case "UIJoypadCommandBarStances":Marshaler.Marshal(node,(Ep1StockUIJoypadCommandBarStances*)root,tracker);break;
            case "UIJoypadCommandBarCoopCommunicator":Marshaler.Marshal(node,(Ep1StockUIJoypadCommandBarCoopCommunicator*)root,tracker);break;
            case "UIJoypadCommandBarPlayerPowers":Marshaler.Marshal(node,(Ep1StockUIJoypadCommandBarPlayerPowers*)root,tracker);break;
            case "UIJoypadCommandBarTopMenu":Marshaler.Marshal(node,(Ep1StockUIJoypadCommandBarTopMenu*)root,tracker);break;
            case "UIMouseTacticalQuickChatDisplay":Marshaler.Marshal(node,(Ep1StockUIMouseTacticalQuickChatDisplay*)root,tracker);break;
            case "UIMouseDragSelectionFeedback":Marshaler.Marshal(node,(Ep1StockUIMouseDragSelectionFeedback*)root,tracker);break;
            case "UIMouseHUDElementHotKeyGate":Marshaler.Marshal(node,(Ep1StockUIMouseHUDElementHotKeyGate*)root,tracker);break;
            case "UIMouseTacticalAttackMoveButton":Marshaler.Marshal(node,(Ep1StockUIMouseTacticalAttackMoveButton*)root,tracker);break;
            case "UIMouseTacticalBuildQueueTab":Marshaler.Marshal(node,(Ep1StockUIMouseTacticalBuildQueueTab*)root,tracker);break;
            case "UIMouseTacticalBuildQueueTypeTab":Marshaler.Marshal(node,(Ep1StockUIMouseTacticalBuildQueueTypeTab*)root,tracker);break;
            case "UIMouseTacticalCommandAttack":Marshaler.Marshal(node,(Ep1StockUIMouseTacticalCommandAttack*)root,tracker);break;
            case "UIMouseTacticalCommandAttackMove":Marshaler.Marshal(node,(Ep1StockUIMouseTacticalCommandAttackMove*)root,tracker);break;
            case "UIMouseTacticalCommandCommunicateWithAllies":Marshaler.Marshal(node,(Ep1StockUIMouseTacticalCommandCommunicateWithAllies*)root,tracker);break;
            case "UIMouseTacticalCommandDock":Marshaler.Marshal(node,(Ep1StockUIMouseTacticalCommandDock*)root,tracker);break;
            case "UIMouseTacticalCommandEnter":Marshaler.Marshal(node,(Ep1StockUIMouseTacticalCommandEnter*)root,tracker);break;
            case "UIMouseTacticalCommandForceAttack":Marshaler.Marshal(node,(Ep1StockUIMouseTacticalCommandForceAttack*)root,tracker);break;
            case "UIMouseTacticalCommandForceMove":Marshaler.Marshal(node,(Ep1StockUIMouseTacticalCommandForceMove*)root,tracker);break;
            case "UIMouseTacticalCommandGetCrate":Marshaler.Marshal(node,(Ep1StockUIMouseTacticalCommandGetCrate*)root,tracker);break;
            case "UIMouseTacticalCommandMineOre":Marshaler.Marshal(node,(Ep1StockUIMouseTacticalCommandMineOre*)root,tracker);break;
            case "UIMouseTacticalCommandMove":Marshaler.Marshal(node,(Ep1StockUIMouseTacticalCommandMove*)root,tracker);break;
            case "UIMouseTacticalCommandPlaceAIBeacon":Marshaler.Marshal(node,(Ep1StockUIMouseTacticalCommandPlaceAIBeacon*)root,tracker);break;
            case "UIMouseTacticalCommandPlaceRallyPoint":Marshaler.Marshal(node,(Ep1StockUIMouseTacticalCommandPlaceRallyPoint*)root,tracker);break;
            case "UIMouseTacticalCommandReverseMove":Marshaler.Marshal(node,(Ep1StockUIMouseTacticalCommandReverseMove*)root,tracker);break;
            case "UIMouseTacticalCommandSelect":Marshaler.Marshal(node,(Ep1StockUIMouseTacticalCommandSelect*)root,tracker);break;
            case "UIMouseTacticalCommandSell":Marshaler.Marshal(node,(Ep1StockUIMouseTacticalCommandSell*)root,tracker);break;
            case "UIMouseTacticalCommandSetRallyPoint":Marshaler.Marshal(node,(Ep1StockUIMouseTacticalCommandSetRallyPoint*)root,tracker);break;
            case "UIMouseTacticalCommandToggleSelfRepair":Marshaler.Marshal(node,(Ep1StockUIMouseTacticalCommandToggleSelfRepair*)root,tracker);break;
            case "UIMouseTacticalCoOpAIButtonsAssaultButton":Marshaler.Marshal(node,(Ep1StockUIMouseTacticalCoOpAIButtonsAssaultButton*)root,tracker);break;
            case "UIMouseTacticalCoOpAIButtonsHelpButton":Marshaler.Marshal(node,(Ep1StockUIMouseTacticalCoOpAIButtonsHelpButton*)root,tracker);break;
            case "UIMouseTacticalCoOpAIButtonsHoldObjectButton":Marshaler.Marshal(node,(Ep1StockUIMouseTacticalCoOpAIButtonsHoldObjectButton*)root,tracker);break;
            case "UIMouseTacticalCoOpAIButtonsHoldPositionButton":Marshaler.Marshal(node,(Ep1StockUIMouseTacticalCoOpAIButtonsHoldPositionButton*)root,tracker);break;
            case "UIMouseTacticalCoOpAIButtonsNormalButton":Marshaler.Marshal(node,(Ep1StockUIMouseTacticalCoOpAIButtonsNormalButton*)root,tracker);break;
            case "UIMouseTacticalDeactivateMissionHotSpotsButton":Marshaler.Marshal(node,(Ep1StockUIMouseTacticalDeactivateMissionHotSpotsButton*)root,tracker);break;
            case "UIMouseTacticalExitContainerButton":Marshaler.Marshal(node,(Ep1StockUIMouseTacticalExitContainerButton*)root,tracker);break;
            case "UIMouseTacticalFastForwardButton":Marshaler.Marshal(node,(Ep1StockUIMouseTacticalFastForwardButton*)root,tracker);break;
            case "UIMouseTacticalForceAttackButton":Marshaler.Marshal(node,(Ep1StockUIMouseTacticalForceAttackButton*)root,tracker);break;
            case "UIMouseTacticalForceAttackMoveButton":Marshaler.Marshal(node,(Ep1StockUIMouseTacticalForceAttackMoveButton*)root,tracker);break;
            case "UIMouseTacticalForceMoveButton":Marshaler.Marshal(node,(Ep1StockUIMouseTacticalForceMoveButton*)root,tracker);break;
            case "UIMouseTacticalGroupSelection":Marshaler.Marshal(node,(Ep1StockUIMouseTacticalGroupSelection*)root,tracker);break;
            case "UIMouseTacticalLookAt":Marshaler.Marshal(node,(Ep1StockUIMouseTacticalLookAt*)root,tracker);break;
            case "UIMouseTacticalMainCommandBarHotKeys":Marshaler.Marshal(node,(Ep1StockUIMouseTacticalMainCommandBarHotKeys*)root,tracker);break;
            case "UIMouseTacticalMiscHotKeys":Marshaler.Marshal(node,(Ep1StockUIMouseTacticalMiscHotKeys*)root,tracker);break;
            case "UIMouseTacticalOpenAdvancedCommandsButton":Marshaler.Marshal(node,(Ep1StockUIMouseTacticalOpenAdvancedCommandsButton*)root,tracker);break;
            case "UIMouseTacticalPauseButton":Marshaler.Marshal(node,(Ep1StockUIMouseTacticalPauseButton*)root,tracker);break;
            case "UIMouseTacticalPlanningModeButton":Marshaler.Marshal(node,(Ep1StockUIMouseTacticalPlanningModeButton*)root,tracker);break;
            case "UIMouseTacticalPlayerPowerHotKeys":Marshaler.Marshal(node,(Ep1StockUIMouseTacticalPlayerPowerHotKeys*)root,tracker);break;
            case "UIMouseTacticalPlayerTechStoreButton":Marshaler.Marshal(node,(Ep1StockUIMouseTacticalPlayerTechStoreButton*)root,tracker);break;
            case "UIMouseTacticalRadarTelestrator":Marshaler.Marshal(node,(Ep1StockUIMouseTacticalRadarTelestrator*)root,tracker);break;
            case "UIMouseTacticalRedAlertButton":Marshaler.Marshal(node,(UIMouseTacticalRedAlertButton*)root,tracker);break;
            case "UIMouseTacticalReverseMoveButton":Marshaler.Marshal(node,(Ep1StockUIMouseTacticalReverseMoveButton*)root,tracker);break;
            case "UIMouseTacticalSelectionDetailsButtonHotKeys":Marshaler.Marshal(node,(Ep1StockUIMouseTacticalSelectionDetailsButtonHotKeys*)root,tracker);break;
            case "UIMouseTacticalSelectionRefinementTab":Marshaler.Marshal(node,(Ep1StockUIMouseTacticalSelectionRefinementTab*)root,tracker);break;
            case "UIMouseTacticalSellModeButton":Marshaler.Marshal(node,(Ep1StockUIMouseTacticalSellModeButton*)root,tracker);break;
            case "UIMouseTacticalSetStanceButton":Marshaler.Marshal(node,(Ep1StockUIMouseTacticalSetStanceButton*)root,tracker);break;
            case "UIMouseTacticalStopButton":Marshaler.Marshal(node,(Ep1StockUIMouseTacticalStopButton*)root,tracker);break;
            case "UIMouseTacticalTelestrator":Marshaler.Marshal(node,(Ep1StockUIMouseTacticalTelestrator*)root,tracker);break;
            case "UIMouseTacticalThreatMeter":Marshaler.Marshal(node,(Ep1StockUIMouseTacticalThreatMeter*)root,tracker);break;
            case "UIMouseTacticalToggleRepairModeButton":Marshaler.Marshal(node,(Ep1StockUIMouseTacticalToggleRepairModeButton*)root,tracker);break;
            case "UIMouseTacticalUnitIntroButton":Marshaler.Marshal(node,(Ep1StockUIMouseTacticalUnitIntroButton*)root,tracker);break;
            case "UIMouseTacticalWaypointModeButton":Marshaler.Marshal(node,(Ep1StockUIMouseTacticalWaypointModeButton*)root,tracker);break;
            case "UIMouseVoiceChat":Marshaler.Marshal(node,(Ep1StockUIMouseVoiceChat*)root,tracker);break;
            case "UITimerDisplay":Marshaler.Marshal(node,(Ep1StockUITimerDisplay*)root,tracker);break;
            case "UIWorldBuilderDecalGrid":Marshaler.Marshal(node,(Ep1StockUIWorldBuilderDecalGrid*)root,tracker);break;
            case "UICommandButton":Marshaler.Marshal(node,(Ep1StockUICommandButton*)root,tracker);break;
            case "UICoopTinting":Marshaler.Marshal(node,(Ep1StockUICoopTinting*)root,tracker);break;
            case "UIDecalCloud":Marshaler.Marshal(node,(Ep1StockUIDecalCloud*)root,tracker);break;
            case "UIDecalGrid":Marshaler.Marshal(node,(Ep1StockUIDecalGrid*)root,tracker);break;
            case "UIFloatingTextDisplay":Marshaler.Marshal(node,(Ep1StockUIFloatingTextDisplay*)root,tracker);break;
            case "UIMessageDisplay":Marshaler.Marshal(node,(Ep1StockUIMessageDisplay*)root,tracker);break;
            case "UIRadarObjectsLayer":Marshaler.Marshal(node,(Ep1StockUIRadarObjectsLayer*)root,tracker);break;
            case "UIScenarioMapPreview":Marshaler.Marshal(node,(UIScenarioMapPreview*)root,tracker);break;
            case "UISubtitleDisplay":Marshaler.Marshal(node,(Ep1StockUISubtitleDisplay*)root,tracker);break;
            case "BounceKickTerrainMapping":Marshaler.Marshal(node,(Ep1StockBounceKickTerrainMapping*)root,tracker);break;
            case "ResourceSettings":Marshaler.Marshal(node,(Ep1CoverageResourceSettings*)root,tracker);break;
            case "GiftManagerSettings":Marshaler.Marshal(node,(Ep1CoverageGiftManagerSettings*)root,tracker);break;
            case "UIConfigList":Marshaler.Marshal(node,(Ep1StockUIConfigUIConfigList*)root,tracker);break;
            case "AudioSettings":Marshaler.Marshal(node,(Ep1StockUIConfigAudioSettings*)root,tracker);break;
            case "AmbientStream":Marshaler.Marshal(node,(Marshaler.Ep1SingleFileSound*)root,tracker);break;
            case "MusicTrack":Marshaler.Marshal(node,(Marshaler.Ep1SingleFileSound*)root,tracker);break;
            case "DialogEvent":Marshaler.Marshal(node,(Marshaler.Ep1SingleFileSound*)root,tracker);break;
            case "ConquerCardDefinitionUnit":Marshaler.Marshal(node,(ConquerCardDefinitionUnit*)root,tracker);break;
            case "ConquerCardDefinitionStructure":Marshaler.Marshal(node,(ConquerCardDefinitionStructure*)root,tracker);break;
            case "ConquerCardDefinitionPower":Marshaler.Marshal(node,(ConquerCardDefinitionPower*)root,tracker);break;
            case "SiegeEvaEvent":Marshaler.Marshal(node,(SiegeEvaEvent*)root,tracker);break;
            case "DLContent":Marshaler.Marshal(node,(DLContent*)root,tracker);break;
            case "BootupDisplaySequence":Marshaler.Marshal(node,(BootupDisplaySequence*)root,tracker);break;
            case "TheVersion":Marshaler.Marshal(node,(TheVersion*)root,tracker);break;
            case "FontSubstitution":Marshaler.Marshal(node,(Ep1FontSubstitution*)root,tracker);break;
            case "StanceTemplate":Marshaler.Marshal(node,(StanceTemplate*)root,tracker);break;
            case "ImageSequence":Marshaler.Marshal(node,(ImageSequence*)root,tracker);break;
            case "AIStrategicStateName":Marshaler.Marshal(node,(Ep1StrategicStateName*)root,tracker);break;
            case "SkirmishOpeningMove":Marshaler.Marshal(node,(SkirmishOpeningMove*)root,tracker);break;
            case "EvaSettings":Marshaler.Marshal(node,(Ep1EvaSettings*)root,tracker);break;
            case "EvaEvent":Marshaler.Marshal(node,(Ep1EvaEvent*)root,tracker);break;
            case "Road":Marshaler.Marshal(node,(Road*)root,tracker);break;
            case "UnitIntro":Marshaler.Marshal(node,(Ep1UnitIntro*)root,tracker);break;
            case "InvisibilityTemplate":Marshaler.Marshal(node,(Ep1InvisibilityTemplate*)root,tracker);break;
            case "AITargetingHeuristic":Marshaler.Marshal(node,(Ep1AITargetingHeuristic*)root,tracker);break;
            case "RedAlertTemplate":Marshaler.Marshal(node,(Ep1RedAlertTemplate*)root,tracker);break;
            case "AIData":Marshaler.Marshal(node,(Ep1AIData*)root,tracker);break;
            case "MouseCursor":Marshaler.Marshal(node,(Ep1MouseCursor*)root,tracker);break;
            case "FontTranslation":Marshaler.Marshal(node,(Ep1FontTranslation*)root,tracker);break;
            case "FontDirectory":Marshaler.Marshal(node,(Ep1FontDirectory*)root,tracker);break;
            case "Mouse":Marshaler.Marshal(node,(Ep1Mouse*)root,tracker);break;
            case "ForceFeedbackEvent":Marshaler.Marshal(node,(Ep1ForceFeedbackEvent*)root,tracker);break;
            case "AudioEventLimitGroup":Marshaler.Marshal(node,(Ep1AudioEventLimitGroup*)root,tracker);break;
            case "AnimationSoundSettings":Marshaler.Marshal(node,(Ep1AnimationSoundSettings*)root,tracker);break;
            case "EngagingContent":Marshaler.Marshal(node,(Ep1EngagingContent*)root,tracker);break;
            case "EngagingContentGroup":Marshaler.Marshal(node,(Ep1EngagingContentGroup*)root,tracker);break;
            case "EngagingContentSequence":Marshaler.Marshal(node,(Ep1EngagingContentSequence*)root,tracker);break;
            case "GeneralHotKeySlots":Marshaler.Marshal(node,(Ep1GeneralHotKeySlots*)root,tracker);break;
            case "BasicMouseCursors":Marshaler.Marshal(node,(Ep1BasicMouseCursors*)root,tracker);break;
            case "FormationAssistantUnitDefinition":Marshaler.Marshal(node,(Ep1FormationAssistantUnitDefinition*)root,tracker);break;
            case "FormationAssistantTemplate":Marshaler.Marshal(node,(Ep1FormationAssistantTemplate*)root,tracker);break;
            case "FormationSelectionTemplate":Marshaler.Marshal(node,(Ep1FormationSelectionTemplate*)root,tracker);break;
            case "FormationAssistant":Marshaler.Marshal(node,(Ep1FormationAssistant*)root,tracker);break;
            case "EmotionTemplate":Marshaler.Marshal(node,(Ep1EmotionTemplate*)root,tracker);break;
            case "RegisteredObjectSet":Marshaler.Marshal(node,(Ep1RegisteredObjectSet*)root,tracker);break;
            case "PlayerTechRank":Marshaler.Marshal(node,(Ep1PlayerTechRank*)root,tracker);break;
            case "MultiplayerColor":Marshaler.Marshal(node,(Ep1MultiplayerColor*)root,tracker);break;
            case "HeaderTemplate":Marshaler.Marshal(node,(Ep1HeaderTemplate*)root,tracker);break;
            case "ShaderOverride":Marshaler.Marshal(node,(ShaderOverride*)root,tracker);break;
            case "RadiusCursor":Marshaler.Marshal(node,(Ep1RadiusCursor*)root,tracker);break;
            case "AIBudgetStateDefinition":Marshaler.Marshal(node,(Ep1AIBudgetStateDefinition*)root,tracker);break;
            case "AIBuildStateDefinition":Marshaler.Marshal(node,(Ep1AIBuildStateDefinition*)root,tracker);break;
            case "AIStrategicStateDefinition":Marshaler.Marshal(node,(Ep1AIStrategicStateDefinition*)root,tracker);break;
            case "AIPersonalityDefinition":Marshaler.Marshal(node,(Ep1AIPersonalityDefinition*)root,tracker);break;
            case "AIMicroManagerData":Marshaler.Marshal(node,(Ep1AIMicroManagerData*)root,tracker);break;
            case "MiscAudio":Marshaler.Marshal(node,(Ep1MiscAudio*)root,tracker);break;
            case "HotKeySlot":Marshaler.Marshal(node,(Ep1HotKeySlot*)root,tracker);break;
            case "MappableKey":Marshaler.Marshal(node,(MappableKey*)root,tracker);break;
            case "UIQuickChatMessage":Marshaler.Marshal(node,(Ep1UIQuickChatMessage*)root,tracker);break;
            default:throw new InvalidDataException("Unreviewed asset root: "+family.Name);
        }
        var chunk=new Chunk();if(!tracker.MakeRelocatable(chunk))throw new InvalidDataException("asset root allocation failed: "+family.Name);return chunk;
    }
}
