using System.Xml;
using BinaryAssetBuilder.Core.SageXml;
using SageBinaryData;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: the literal allowlist covers actual original UI configuration components and excludes arbitrary schema reflection.
internal static class StockUIComponentCatalog
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: resolve the source-owned concrete component class only within the original-qualified registry. */
    //-------------------------------------------------------------------------------------------------
    internal static Type For(XmlElement source)=>source.LocalName switch
    {
        "UIComponentAptJoypad"=>typeof(Ep1StockUIConfigUIComponentAptJoypad),
        "UIComponentAptMediator"=>typeof(Ep1StockUIConfigUIComponentAptMediator),
        "UIComponentAutoMatch"=>typeof(Ep1StockUIConfigUIComponentAutoMatch),
        "UIComponentBootup"=>typeof(Ep1StockUIConfigUIComponentBootup),
        "UIComponentCampaign"=>typeof(Ep1StockUIConfigUIComponentCampaign),
        "UIComponentCareerStats"=>typeof(Ep1StockUIConfigUIComponentCareerStats),
        "UIComponentChat"=>typeof(Ep1StockUIConfigUIComponentChat),
        "UIComponentCheats"=>typeof(Ep1StockUIConfigUIComponentCheats),
        "UIComponentComlinkNotification"=>typeof(Ep1StockUIConfigUIComponentComlinkNotification),
        "UIComponentCommandBar"=>typeof(Ep1StockUIConfigUIComponentCommandBar),
        "UIComponentCommandList"=>typeof(Ep1StockUIConfigUIComponentCommandList),
        "UIComponentCoopCommander"=>typeof(Ep1StockUIConfigUIComponentCoopCommander),
        "UIComponentCredits"=>typeof(Ep1StockUIConfigUIComponentCredits),
        "UIComponentCursor"=>typeof(Ep1StockUIConfigUIComponentCursor),
        "UIComponentDisconnect"=>typeof(Ep1StockUIConfigUIComponentDisconnect),
        "UIComponentFormation"=>typeof(Ep1StockUIConfigUIComponentFormation),
        "UIComponentGameInfo"=>typeof(Ep1StockUIConfigUIComponentGameInfo),
        "UIComponentGameSetup"=>typeof(Ep1StockUIConfigUIComponentGameSetup),
        "UIComponentGameViewJoypad"=>typeof(Ep1StockUIConfigUIComponentGameViewJoypad),
        "UIComponentInGameShowHide"=>typeof(Ep1StockUIConfigUIComponentInGameShowHide),
        "UIComponentInGameText"=>typeof(Ep1StockUIConfigUIComponentInGameText),
        "UIComponentInputBridge"=>typeof(Ep1StockUIConfigUIComponentInputBridge),
        "UIComponentJoypadTactical"=>typeof(Ep1StockUIConfigUIComponentJoypadTactical),
        "UIComponentKeyboard"=>typeof(Ep1StockUIConfigUIComponentKeyboard),
        "UIComponentLanLobby"=>typeof(Ep1StockUIConfigUIComponentLanLobby),
        "UIComponentLoadScreen"=>typeof(Ep1StockUIConfigUIComponentLoadScreen),
        "UIComponentLobby"=>typeof(Ep1StockUIConfigUIComponentLobby),
        "UIComponentMapList"=>typeof(Ep1StockUIConfigUIComponentMapList),
        "UIComponentMessage"=>typeof(Ep1StockUIConfigUIComponentMessage),
        "UIComponentMode"=>typeof(Ep1StockUIConfigUIComponentMode),
        "UIComponentMouse"=>typeof(Ep1StockUIConfigUIComponentMouse),
        "UIComponentMouseInGame"=>typeof(Ep1StockUIConfigUIComponentMouseInGame),
        "UIComponentMouseTactical"=>typeof(Ep1StockUIConfigUIComponentMouseTactical),
        "UIComponentMovie"=>typeof(Ep1StockUIConfigUIComponentMovie),
        "UIComponentMovieArchive"=>typeof(Ep1StockUIConfigUIComponentMovieArchive),
        "UIComponentObjectAction"=>typeof(Ep1StockUIConfigUIComponentObjectAction),
        "UIComponentObjectSelector"=>typeof(Ep1StockUIConfigUIComponentObjectSelector),
        "UIComponentObjectives"=>typeof(Ep1StockUIConfigUIComponentObjectives),
        "UIComponentOnlineBuddyList"=>typeof(Ep1StockUIConfigUIComponentOnlineBuddyList),
        "UIComponentOnlineGameList"=>typeof(Ep1StockUIConfigUIComponentOnlineGameList),
        "UIComponentOnlineInvite"=>typeof(Ep1StockUIConfigUIComponentOnlineInvite),
        "UIComponentOnlineLobbyList"=>typeof(Ep1StockUIConfigUIComponentOnlineLobbyList),
        "UIComponentOnlineLogin"=>typeof(Ep1StockUIConfigUIComponentOnlineLogin),
        "UIComponentOnlinePlayerList"=>typeof(Ep1StockUIConfigUIComponentOnlinePlayerList),
        "UIComponentOptions"=>typeof(Ep1StockUIConfigUIComponentOptions),
        "UIComponentOptionsSettingsAudio"=>typeof(Ep1StockUIConfigUIComponentOptionsSettingsAudio),
        "UIComponentOptionsSettingsGeneral"=>typeof(Ep1StockUIConfigUIComponentOptionsSettingsGeneral),
        "UIComponentOptionsSettingsGraphics"=>typeof(Ep1StockUIConfigUIComponentOptionsSettingsGraphics),
        "UIComponentOptionsSettingsHotkeys"=>typeof(Ep1StockUIConfigUIComponentOptionsSettingsHotkeys),
        "UIComponentOptionsSettingsNetwork"=>typeof(Ep1StockUIConfigUIComponentOptionsSettingsNetwork),
        "UIComponentPause"=>typeof(Ep1StockUIConfigUIComponentPause),
        "UIComponentPcSaveLoad"=>typeof(Ep1StockUIConfigUIComponentPcSaveLoad),
        "UIComponentPlasmaInterface"=>typeof(Ep1StockUIConfigUIComponentPlasmaInterface),
        "UIComponentPlayerPowers"=>typeof(Ep1StockUIConfigUIComponentPlayerPowers),
        "UIComponentPlayerTechStore"=>typeof(Ep1StockUIConfigUIComponentPlayerTechStore),
        "UIComponentPostGame"=>typeof(Ep1StockUIConfigUIComponentPostGame),
        "UIComponentProfileManager"=>typeof(Ep1StockUIConfigUIComponentProfileManager),
        "UIComponentRadar"=>typeof(Ep1StockUIConfigUIComponentRadar),
        "UIComponentSUDs"=>typeof(Ep1StockUIConfigUIComponentSUDs),
        "UIComponentSaveLoad"=>typeof(Ep1StockUIConfigUIComponentSaveLoad),
        "UIComponentScenario"=>typeof(Ep1StockUIConfigUIComponentScenario),
        "UIComponentShell"=>typeof(Ep1StockUIConfigUIComponentShell),
        "UIComponentShellMultiplayer"=>typeof(Ep1StockUIConfigUIComponentShellMultiplayer),
        "UIComponentShellVisibility"=>typeof(Ep1StockUIConfigUIComponentShellVisibility),
        "UIComponentSound"=>typeof(Ep1StockUIConfigUIComponentSound),
        "UIComponentSpecialPowers"=>typeof(Ep1StockUIConfigUIComponentSpecialPowers),
        "UIComponentStats"=>typeof(Ep1StockUIConfigUIComponentStats),
        "UIComponentTicker"=>typeof(Ep1StockUIConfigUIComponentTicker),
        "UIComponentTutorial"=>typeof(Ep1StockUIConfigUIComponentTutorial),
        "UIComponentVictoryDefeat"=>typeof(Ep1StockUIConfigUIComponentVictoryDefeat),
        "UIComponentVoiceChat"=>typeof(Ep1StockUIConfigUIComponentVoiceChat),
        _=>throw new InvalidDataException("UI component has no original native witness: "+source.LocalName)
    };
    //-------------------------------------------------------------------------------------------------
    /** Reborn: inject reviewed class words into private writer XML while the independent proof keeps the original source. */
    //-------------------------------------------------------------------------------------------------
    internal static void PrepareWriter(XmlElement root)
    {
        foreach(XmlElement list in root.GetElementsByTagName("ComponentList",SchemaSet.XmlNamespace))
        foreach(var child in list.ChildNodes.OfType<XmlElement>())
        {
            _=For(child);
            child.SetAttribute("TypeId",XmlConvert.ToString(BinaryAssetBuilder.Core.Hashing.FastHash.GetHashCode(child.LocalName)));
        }
    }}
