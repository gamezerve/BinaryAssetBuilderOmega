movie 'E:\depot\CnC\RA3_EP1\PRODUC~1\Data\APTBUI~1\034A3~1.0-D\pc\Output\TACTIC~2\\TACTIC~1.eaf' &compressed // flash 7, total frames: 21, frame rate: 30 fps, 1365x768 px

  &frame 0
    &constants 'letterBoxLayer', '_visible', 'hideMainCount', 'main', 'upperSidebarLayer', 'selectionFeedbackSurface', 'floatingTextSurface', 'mainVisibility', 'UpdateUpperSidebarVisibility', 'upperSidebarExplicitlyHidden', '_fade', 'shroud', 'gotoAndPlay', 'inputShield', 'Pointer', 'encode', 'helpBox', 'upperSidebar', 'radar', 'mainCommandBar', 'lowerSidebar', 'selectionDetails', 'resourceDisplay', 'commandModeBar', 'energyMeter', 'playerPowersBar', 'specialPowerTimerDisplay', 'dialogStage', 'cutSceneStage', 'objectiveIndicatorStage', 'messageSurface', 'timerSurface', 'popUpMetaStage', 'observerControl', 'telestratorControl', 'replayControl', 'voiceChatControl', 'ticker', 'telestratorSurface', 'subtitlePanelSurface', 'coOpAIOrderPanel', 'beaconStage', 'fadeSurface', 'drawableOverlayStage', 'expPanel', 'displayLeft', 'displayTop', 'displayWidth', 'displayHeight', 'leftAnchoredClips', 'length', '_x', '_parent', 'GlobalToLocalX', 'leftMargins', 'rightAnchoredClips', 'rightMargins', 'topAnchoredClips', '_y', 'GlobalToLocalY', 'topMargins', 'bottomAnchoredClips', 'bottomMargins', 'centerAnchoredClips', 'centerOffsets', 'x', 'y', 'fullScreenClips', '_width', '_height', 'letterBoxVisible', '_fadeIn', 'showRadarDuringLetterBox', '_fadeOut', 'TacticalHUD::SetCurrentFaction', 'currentFaction', 'factionObserverMap', 'OnCurrentFactionChanged', 'Yuriko', 'UseYurikoOffsets', '_root', 'ancestor', '_xscale', '_yscale', 'upperSidebarActualVisibility', 'TacticalHUD.swf onUnloading()', 'gadgetFlashStage', 'onUnloading', 'TacticalHUD.swf onUnload()', 'Hide', 'Show', 'FadeIn', 'FadeOut', 'HideShroud', 'GetInputShield', 'GetHelpBox', 'GetSelectionFeedbackSurface', 'GetRadar', 'GetMainCommandBar', 'GetSelectionDetailsPanel', 'GetResourceDisplay', 'GetCommandModeBar', 'GetEnergyMeter', 'GetPlayerPowerBar', 'GetPlayerPowerTimerDisplay', 'GetDialogStage', 'GetCutSceneStage', 'GetObjectiveIndicatorStage', 'GetFloatingTextSurface', 'GetMessageSurface', 'GetTimerSurface', 'GetPopUpMetaStage', 'GetObserverControlPanel', 'GetTelestratorControlPanel', 'GetReplayControlPanel', 'GetVoiceChatControlPanel', 'GetTickerPanel', 'GetTelestratorSurface', 'GetSubtitlePanelSurface', 'GetCoOpAIOrderPanel', 'GetBeaconStage', 'GetDrawableOverlayStage', 'GetPlayerExperiencePanel', 'SetDisplayRegion', 'SetLetterBoxVisibility', 'SetCurrentFaction', 'onUnload', 'initialized', 'TacticalHUD::Initializing', 'this', 'OnContentLoaded', 'TacticalHUDBeaconStage.swf', 'loadMovie', 'ig_DrawableOverlayStage.swf', 'igm_gadgetFlashStage.swf', 'watermark', 'Array', 'i', 'clip', 'Object', 'FSCommand:', 'OnInitialized', 'qualifyName', '', 'extern', 'InGame', 'Soviet'  
    &function Hide (    )    
      &pushsdbgv 0							//'letterBoxLayer'
      &pushsdb 1							//'_visible'
      &pushfalse
      &setMember
      &pushsdbgv 2							//'hideMainCount'
      &pushsdb 2							//'hideMainCount'
      &pushsdbgv 2							//'hideMainCount'
      &increment
      &setVariable
      &pushzero
      &equals
      &not
      &jnz label1      
      &pushsdbgv 3							//'main'
      &pushsdb 1							//'_visible'
      &pushfalse
      &setMember
     label1:
      &pushsdbgv 4							//'upperSidebarLayer'
      &pushsdb 1							//'_visible'
      &pushfalse
      &setMember
      &pushsdbgv 5							//'selectionFeedbackSurface'
      &pushsdb 1							//'_visible'
      &pushfalse
      &setMember
      &pushsdbgv 6							//'floatingTextSurface'
      &pushsdb 1							//'_visible'
      &pushfalse
      &setMember
    &end // of function Hide

    &function Show (    )    
      &pushsdbgv 0							//'letterBoxLayer'
      &pushsdb 1							//'_visible'
      &pushtrue
      &setMember
      &pushsdb 2							//'hideMainCount'
      &pushsdbgv 2							//'hideMainCount'
      &decrement
      &setRegister r:0
      &setVariable
      &pushregister 0
      &pushzero
      &equals
      &not
      &jnz label2      
      &pushsdbgv 3							//'main'
      &pushsdb 1							//'_visible'
      &pushtrue
      &setMember
     label2:
      &pushsdbgv 4							//'upperSidebarLayer'
      &pushsdb 1							//'_visible'
      &pushtrue
      &setMember
      &pushsdbgv 5							//'selectionFeedbackSurface'
      &pushsdb 1							//'_visible'
      &pushtrue
      &setMember
      &pushsdbgv 6							//'floatingTextSurface'
      &pushsdb 1							//'_visible'
      &pushtrue
      &setMember
    &end // of function Show

    &function FadeIn (    )    
      &pushsdbgv 7							//'mainVisibility'
      &not
      &not
      &jnz label3      
      &pushsdb 7							//'mainVisibility'
      &pushtrue
      &setVariable
      &gotoLabel '_fadeIn'
      &play
      &pushzero
      &dcallfp 8							// UpdateUpperSidebarVisibility()
      &pushsdb 2							//'hideMainCount'
      &pushsdbgv 2							//'hideMainCount'
      &decrement
      &setRegister r:0
      &setVariable
      &pushregister 0
      &pushzero
      &equals
      &not
      &jnz label3      
      &pushsdbgv 3							//'main'
      &pushsdb 1							//'_visible'
      &pushtrue
      &setMember
     label3:
    &end // of function FadeIn

    &function FadeOut (    )    
      &pushsdbgv 7							//'mainVisibility'
      &not
      &jnz label4      
      &pushsdb 7							//'mainVisibility'
      &pushfalse
      &setVariable
      &gotoLabel '_fadeOut'
      &play
      &pushzero
      &dcallfp 8							// UpdateUpperSidebarVisibility()
     label4:
    &end // of function FadeOut

    &function FadeInUpperSidebar (    )    
      &pushsdbgv 9							//'upperSidebarExplicitlyHidden'
      &not
      &jnz label5      
      &pushsdb 9							//'upperSidebarExplicitlyHidden'
      &pushfalse
      &setVariable
      &pushzero
      &dcallfp 8							// UpdateUpperSidebarVisibility()
     label5:
    &end // of function FadeInUpperSidebar

    &function FadeOutUpperSidebar (    )    
      &pushsdbgv 9							//'upperSidebarExplicitlyHidden'
      &not
      &not
      &jnz label6      
      &pushsdb 9							//'upperSidebarExplicitlyHidden'
      &pushtrue
      &setVariable
      &pushzero
      &dcallfp 8							// UpdateUpperSidebarVisibility()
     label6:
    &end // of function FadeOutUpperSidebar

    &function HideShroud (    )    
      &pushsdb 10							//'_fade'
      &pushone
      &pushsdbgv 11							//'shroud'
      &dcallmp 12							// gotoAndPlay()
    &end // of function HideShroud

    &function GetInputShield (    )    
      &pushsdbgv 13							//'inputShield'
      &pushone
      &pushsdbgv 14							//'Pointer'
      &pushsdb 15							//'encode'
      &callMethod
      &return
    &end // of function GetInputShield

    &function GetHelpBox (    )    
      &pushsdbgv 3							//'main'
      &pushsdbgm 16							//'helpBox'
      &pushone
      &pushsdbgv 14							//'Pointer'
      &pushsdb 15							//'encode'
      &callMethod
      &return
    &end // of function GetHelpBox

    &function GetSelectionFeedbackSurface (    )    
      &pushsdbgv 5							//'selectionFeedbackSurface'
      &pushone
      &pushsdbgv 14							//'Pointer'
      &pushsdb 15							//'encode'
      &callMethod
      &return
    &end // of function GetSelectionFeedbackSurface

    &function GetRadar (    )    
      &pushsdbgv 4							//'upperSidebarLayer'
      &pushsdbgm 17							//'upperSidebar'
      &pushsdbgm 18							//'radar'
      &pushone
      &pushsdbgv 14							//'Pointer'
      &pushsdb 15							//'encode'
      &callMethod
      &return
    &end // of function GetRadar

    &function GetMainCommandBar (    )    
      &pushsdbgv 4							//'upperSidebarLayer'
      &pushsdbgm 17							//'upperSidebar'
      &pushsdbgm 19							//'mainCommandBar'
      &pushone
      &pushsdbgv 14							//'Pointer'
      &pushsdb 15							//'encode'
      &callMethod
      &return
    &end // of function GetMainCommandBar

    &function GetSelectionDetailsPanel (    )    
      &pushsdbgv 3							//'main'
      &pushsdbgm 20							//'lowerSidebar'
      &pushsdbgm 21							//'selectionDetails'
      &pushone
      &pushsdbgv 14							//'Pointer'
      &pushsdb 15							//'encode'
      &callMethod
      &return
    &end // of function GetSelectionDetailsPanel

    &function GetResourceDisplay (    )    
      &pushsdbgv 4							//'upperSidebarLayer'
      &pushsdbgm 17							//'upperSidebar'
      &pushsdbgm 22							//'resourceDisplay'
      &pushone
      &pushsdbgv 14							//'Pointer'
      &pushsdb 15							//'encode'
      &callMethod
      &return
    &end // of function GetResourceDisplay

    &function GetCommandModeBar (    )    
      &pushsdbgv 4							//'upperSidebarLayer'
      &pushsdbgm 17							//'upperSidebar'
      &pushsdbgm 23							//'commandModeBar'
      &pushone
      &pushsdbgv 14							//'Pointer'
      &pushsdb 15							//'encode'
      &callMethod
      &return
    &end // of function GetCommandModeBar

    &function GetEnergyMeter (    )    
      &pushsdbgv 4							//'upperSidebarLayer'
      &pushsdbgm 17							//'upperSidebar'
      &pushsdbgm 24							//'energyMeter'
      &pushone
      &pushsdbgv 14							//'Pointer'
      &pushsdb 15							//'encode'
      &callMethod
      &return
    &end // of function GetEnergyMeter

    &function GetPlayerPowersBar (    )    
      &pushsdbgv 3							//'main'
      &pushsdbgm 25							//'playerPowersBar'
      &pushone
      &pushsdbgv 14							//'Pointer'
      &pushsdb 15							//'encode'
      &callMethod
      &return
    &end // of function GetPlayerPowersBar

    &function GetSpecialPowerTimerDisplay (    )    
      &pushsdbgv 3							//'main'
      &pushsdbgm 26							//'specialPowerTimerDisplay'
      &pushone
      &pushsdbgv 14							//'Pointer'
      &pushsdb 15							//'encode'
      &callMethod
      &return
    &end // of function GetSpecialPowerTimerDisplay

    &function GetDialogStage (    )    
      &pushsdbgv 27							//'dialogStage'
      &pushone
      &pushsdbgv 14							//'Pointer'
      &pushsdb 15							//'encode'
      &callMethod
      &return
    &end // of function GetDialogStage

    &function GetCutSceneStage (    )    
      &pushsdbgv 28							//'cutSceneStage'
      &pushone
      &pushsdbgv 14							//'Pointer'
      &pushsdb 15							//'encode'
      &callMethod
      &return
    &end // of function GetCutSceneStage

    &function GetObjectiveIndicatorStage (    )    
      &pushsdbgv 3							//'main'
      &pushsdbgm 29							//'objectiveIndicatorStage'
      &pushone
      &pushsdbgv 14							//'Pointer'
      &pushsdb 15							//'encode'
      &callMethod
      &return
    &end // of function GetObjectiveIndicatorStage

    &function GetFloatingTextSurface (    )    
      &pushsdbgv 6							//'floatingTextSurface'
      &pushone
      &pushsdbgv 14							//'Pointer'
      &pushsdb 15							//'encode'
      &callMethod
      &return
    &end // of function GetFloatingTextSurface

    &function GetMessageSurface (    )    
      &pushsdbgv 3							//'main'
      &pushsdbgm 30							//'messageSurface'
      &pushone
      &pushsdbgv 14							//'Pointer'
      &pushsdb 15							//'encode'
      &callMethod
      &return
    &end // of function GetMessageSurface

    &function GetTimerSurface (    )    
      &pushsdbgv 3							//'main'
      &pushsdbgm 31							//'timerSurface'
      &pushone
      &pushsdbgv 14							//'Pointer'
      &pushsdb 15							//'encode'
      &callMethod
      &return
    &end // of function GetTimerSurface

    &function GetPopUpMetaStage (    )    
      &pushsdbgv 3							//'main'
      &pushsdbgm 32							//'popUpMetaStage'
      &pushone
      &pushsdbgv 14							//'Pointer'
      &pushsdb 15							//'encode'
      &callMethod
      &return
    &end // of function GetPopUpMetaStage

    &function GetObserverControlPanel (    )    
      &pushsdbgv 3							//'main'
      &pushsdbgm 33							//'observerControl'
      &pushone
      &pushsdbgv 14							//'Pointer'
      &pushsdb 15							//'encode'
      &callMethod
      &return
    &end // of function GetObserverControlPanel

    &function GetTelestratorControlPanel (    )    
      &pushsdbgv 3							//'main'
      &pushsdbgm 34							//'telestratorControl'
      &pushone
      &pushsdbgv 14							//'Pointer'
      &pushsdb 15							//'encode'
      &callMethod
      &return
    &end // of function GetTelestratorControlPanel

    &function GetReplayControlPanel (    )    
      &pushsdbgv 3							//'main'
      &pushsdbgm 35							//'replayControl'
      &pushone
      &pushsdbgv 14							//'Pointer'
      &pushsdb 15							//'encode'
      &callMethod
      &return
    &end // of function GetReplayControlPanel

    &function GetVoiceChatControlPanel (    )    
      &pushsdbgv 3							//'main'
      &pushsdbgm 36							//'voiceChatControl'
      &pushone
      &pushsdbgv 14							//'Pointer'
      &pushsdb 15							//'encode'
      &callMethod
      &return
    &end // of function GetVoiceChatControlPanel

    &function GetTickerPanel (    )    
      &pushsdbgv 3							//'main'
      &pushsdbgm 37							//'ticker'
      &pushone
      &pushsdbgv 14							//'Pointer'
      &pushsdb 15							//'encode'
      &callMethod
      &return
    &end // of function GetTickerPanel

    &function GetTelestratorSurface (    )    
      &pushsdbgv 3							//'main'
      &pushsdbgm 38							//'telestratorSurface'
      &pushone
      &pushsdbgv 14							//'Pointer'
      &pushsdb 15							//'encode'
      &callMethod
      &return
    &end // of function GetTelestratorSurface

    &function GetSubtitlePanelSurface (    )    
      &pushsdbgv 39							//'subtitlePanelSurface'
      &pushone
      &pushsdbgv 14							//'Pointer'
      &pushsdb 15							//'encode'
      &callMethod
      &return
    &end // of function GetSubtitlePanelSurface

    &function GetCoOpAIOrderPanel (    )    
      &pushsdbgv 3							//'main'
      &pushsdbgm 40							//'coOpAIOrderPanel'
      &pushone
      &pushsdbgv 14							//'Pointer'
      &pushsdb 15							//'encode'
      &callMethod
      &return
    &end // of function GetCoOpAIOrderPanel

    &function GetBeaconStage (    )    
      &pushsdbgv 41							//'beaconStage'
      &pushone
      &pushsdbgv 14							//'Pointer'
      &pushsdb 15							//'encode'
      &callMethod
      &return
    &end // of function GetBeaconStage

    &function GetFadeSurface (    )    
      &pushsdbgv 42							//'fadeSurface'
      &pushone
      &pushsdbgv 14							//'Pointer'
      &pushsdb 15							//'encode'
      &callMethod
      &return
    &end // of function GetFadeSurface

    &function GetDrawableOverlayStage (    )    
      &pushsdbgv 43							//'drawableOverlayStage'
      &pushone
      &pushsdbgv 14							//'Pointer'
      &pushsdb 15							//'encode'
      &callMethod
      &return
    &end // of function GetDrawableOverlayStage

    &function GetPlayerExperiencePanel (    )    
      &pushsdbgv 3							//'main'
      &pushsdbgm 44							//'expPanel'
      &pushone
      &pushsdbgv 14							//'Pointer'
      &pushsdb 15							//'encode'
      &callMethod
      &return
    &end // of function GetPlayerExperiencePanel

    &function2 SetDisplayRegion (r:15='topArg', r:13='leftArg', r:16='bottomArg', r:14='rightArg') (r:1='_root')
      &pushregister 15
      &toNumber
      &setRegister r:4
      &pop
      &pushregister 13
      &toNumber
      &setRegister r:5
      &pop
      &pushregister 16
      &toNumber
      &setRegister r:6
      &pop
      &pushregister 14
      &toNumber
      &setRegister r:7
      &pop
      &pushregister 7
      &pushregister 5
      &subtract
      &setRegister r:11
      &pop
      &pushregister 6
      &pushregister 4
      &subtract
      &setRegister r:12
      &pop
      &pushregister 5
      &pushregister 7
      &add
      &pushfloat 0.500000000000000
      &multiply
      &setRegister r:9
      &pop
      &pushregister 4
      &pushregister 6
      &add
      &pushfloat 0.500000000000000
      &multiply
      &setRegister r:8
      &pop
      &pushregister 1
      &pushsdb 45							//'displayLeft'
      &pushregister 5
      &setMember
      &pushregister 1
      &pushsdb 46							//'displayTop'
      &pushregister 4
      &setMember
      &pushregister 1
      &pushsdb 47							//'displayWidth'
      &pushregister 11
      &setMember
      &pushregister 1
      &pushsdb 48							//'displayHeight'
      &pushregister 12
      &setMember
      &pushzero
      &setRegister r:3
      &pop
     label7:
      &pushregister 3
      &pushsdbgv 49							//'leftAnchoredClips'
      &pushsdbgm 50							//'length'
      &lessThan
      &not
      &jnz label8      
      &pushsdbgv 49							//'leftAnchoredClips'
      &pushregister 3
      &getMember
      &setRegister r:2
      &pop
      &pushregister 2
      &pushsdb 51							//'_x'
      &pushregister 5
      &pushregister 2
      &pushsdbgm 52							//'_parent'
      &pushbyte 2
      &pushsdb 53							//'GlobalToLocalX'
      &callFunction
      &pushsdbgv 54							//'leftMargins'
      &pushregister 3
      &getMember
      &add
      &setMember
      &pushregister 3
      &increment
      &setRegister r:3
      &pop
      &jmp label7      
     label8:
      &pushzero
      &setRegister r:3
      &pop
     label9:
      &pushregister 3
      &pushsdbgv 55							//'rightAnchoredClips'
      &pushsdbgm 50							//'length'
      &lessThan
      &not
      &jnz label10      
      &pushsdbgv 55							//'rightAnchoredClips'
      &pushregister 3
      &getMember
      &setRegister r:2
      &pop
      &pushregister 2
      &pushsdb 51							//'_x'
      &pushregister 7
      &pushregister 2
      &pushsdbgm 52							//'_parent'
      &pushbyte 2
      &pushsdb 53							//'GlobalToLocalX'
      &callFunction
      &pushsdbgv 56							//'rightMargins'
      &pushregister 3
      &getMember
      &subtract
      &setMember
      &pushregister 3
      &increment
      &setRegister r:3
      &pop
      &jmp label9      
     label10:
      &pushzero
      &setRegister r:3
      &pop
     label11:
      &pushregister 3
      &pushsdbgv 57							//'topAnchoredClips'
      &pushsdbgm 50							//'length'
      &lessThan
      &not
      &jnz label12      
      &pushsdbgv 57							//'topAnchoredClips'
      &pushregister 3
      &getMember
      &setRegister r:2
      &pop
      &pushregister 2
      &pushsdb 58							//'_y'
      &pushregister 4
      &pushregister 2
      &pushsdbgm 52							//'_parent'
      &pushbyte 2
      &pushsdb 59							//'GlobalToLocalY'
      &callFunction
      &pushsdbgv 60							//'topMargins'
      &pushregister 3
      &getMember
      &add
      &setMember
      &pushregister 3
      &increment
      &setRegister r:3
      &pop
      &jmp label11      
     label12:
      &pushzero
      &setRegister r:3
      &pop
     label13:
      &pushregister 3
      &pushsdbgv 61							//'bottomAnchoredClips'
      &pushsdbgm 50							//'length'
      &lessThan
      &not
      &jnz label14      
      &pushsdbgv 61							//'bottomAnchoredClips'
      &pushregister 3
      &getMember
      &setRegister r:2
      &pop
      &pushregister 2
      &pushsdb 58							//'_y'
      &pushregister 6
      &pushregister 2
      &pushsdbgm 52							//'_parent'
      &pushbyte 2
      &pushsdb 59							//'GlobalToLocalY'
      &callFunction
      &pushsdbgv 62							//'bottomMargins'
      &pushregister 3
      &getMember
      &subtract
      &setMember
      &pushregister 3
      &increment
      &setRegister r:3
      &pop
      &jmp label13      
     label14:
      &pushzero
      &setRegister r:3
      &pop
     label15:
      &pushregister 3
      &pushsdbgv 63							//'centerAnchoredClips'
      &pushsdbgm 50							//'length'
      &lessThan
      &not
      &jnz label16      
      &pushsdbgv 63							//'centerAnchoredClips'
      &pushregister 3
      &getMember
      &setRegister r:2
      &pop
      &pushregister 2
      &pushsdb 51							//'_x'
      &pushregister 9
      &pushregister 2
      &pushsdbgm 52							//'_parent'
      &pushbyte 2
      &pushsdb 53							//'GlobalToLocalX'
      &callFunction
      &pushsdbgv 64							//'centerOffsets'
      &pushregister 3
      &getMember
      &pushsdbgm 65							//'x'
      &add
      &setMember
      &pushregister 2
      &pushsdb 58							//'_y'
      &pushregister 8
      &pushregister 2
      &pushsdbgm 52							//'_parent'
      &pushbyte 2
      &pushsdb 59							//'GlobalToLocalY'
      &callFunction
      &pushsdbgv 64							//'centerOffsets'
      &pushregister 3
      &getMember
      &pushsdbgm 66							//'y'
      &add
      &setMember
      &pushregister 3
      &increment
      &setRegister r:3
      &pop
      &jmp label15      
     label16:
      &pushzero
      &setRegister r:3
      &pop
     label17:
      &pushregister 3
      &pushsdbgv 67							//'fullScreenClips'
      &pushsdbgm 50							//'length'
      &lessThan
      &not
      &jnz label18      
      &pushsdbgv 67							//'fullScreenClips'
      &pushregister 3
      &getMember
      &setRegister r:2
      &pop
      &pushregister 2
      &pushsdb 51							//'_x'
      &pushregister 5
      &pushregister 2
      &pushsdbgm 52							//'_parent'
      &pushbyte 2
      &pushsdb 53							//'GlobalToLocalX'
      &callFunction
      &setMember
      &pushregister 2
      &pushsdb 68							//'_width'
      &pushregister 7
      &pushregister 2
      &pushsdbgm 52							//'_parent'
      &pushbyte 2
      &pushsdb 53							//'GlobalToLocalX'
      &callFunction
      &pushregister 2
      &pushsdbgm 51							//'_x'
      &subtract
      &setMember
      &pushregister 2
      &pushsdb 58							//'_y'
      &pushregister 4
      &pushregister 2
      &pushsdbgm 52							//'_parent'
      &pushbyte 2
      &pushsdb 59							//'GlobalToLocalY'
      &callFunction
      &setMember
      &pushregister 2
      &pushsdb 69							//'_height'
      &pushregister 6
      &pushregister 2
      &pushsdbgm 52							//'_parent'
      &pushbyte 2
      &pushsdb 59							//'GlobalToLocalY'
      &callFunction
      &pushregister 2
      &pushsdbgm 58							//'_y'
      &subtract
      &setMember
      &pushregister 3
      &increment
      &setRegister r:3
      &pop
      &jmp label17      
     label18:
      &pushbyte 5
      &setRegister r:10
      &pop
      &pushsdbgv 13							//'inputShield'
      &pushsdb 51							//'_x'
      &pushregister 5
      &pushregister 10
      &subtract
      &setMember
      &pushsdbgv 13							//'inputShield'
      &pushsdb 68							//'_width'
      &pushregister 11
      &pushregister 10
      &pushbyte 2
      &multiply
      &add
      &setMember
      &pushsdbgv 13							//'inputShield'
      &pushsdb 58							//'_y'
      &pushregister 4
      &pushregister 10
      &subtract
      &setMember
      &pushsdbgv 13							//'inputShield'
      &pushsdb 69							//'_height'
      &pushregister 12
      &pushregister 10
      &pushbyte 2
      &multiply
      &add
      &setMember
    &end // of function SetDisplayRegion

    &function2 SetLetterBoxVisibility (r:4='visArg', r:5='showRadarArg') ()
      &pushregister 4
      &toNumber
      &pushzero
      &equals
      &not
      &setRegister r:3
      &pop
      &pushregister 5
      &toNumber
      &pushzero
      &equals
      &not
      &setRegister r:2
      &pop
      &pushregister 3
      &not
      &jnz label22      
      &pushfalse
      &setRegister r:1
      &pop
      &pushsdbgv 70							//'letterBoxVisible'
      &not
      &not
      &jnz label19      
      &pushsdb 70							//'letterBoxVisible'
      &pushtrue
      &setVariable
      &pushsdb 71							//'_fadeIn'
      &pushone
      &pushsdbgv 0							//'letterBoxLayer'
      &dcallmp 12							// gotoAndPlay()
      &pushtrue
      &setRegister r:1
      &pop
     label19:
      &pushregister 2
      &pushsdbgv 72							//'showRadarDuringLetterBox'
      &equals
      &not
      &not
      &jnz label20      
      &pushsdb 72							//'showRadarDuringLetterBox'
      &pushregister 2
      &setVariable
      &pushtrue
      &setRegister r:1
      &pop
     label20:
      &pushregister 1
      &not
      &jnz label21      
      &pushzero
      &dcallfp 8							// UpdateUpperSidebarVisibility()
     label21:
      &jmp label23      
     label22:
      &pushsdbgv 70							//'letterBoxVisible'
      &not
      &jnz label23      
      &pushsdb 70							//'letterBoxVisible'
      &pushfalse
      &setVariable
      &pushsdb 73							//'_fadeOut'
      &pushone
      &pushsdbgv 0							//'letterBoxLayer'
      &dcallmp 12							// gotoAndPlay()
      &pushzero
      &dcallfp 8							// UpdateUpperSidebarVisibility()
     label23:
    &end // of function SetLetterBoxVisibility

    &function2 SetCurrentFaction (r:3='faction') (r:1='_root')
      &pushsdb 74							//'TacticalHUD::SetCurrentFaction'
      &trace
      &pushregister 3
      &pushregister 1
      &pushsdbgm 75							//'currentFaction'
      &equals
      &not
      &not
      &jnz label25      
      &pushregister 1
      &pushsdb 75							//'currentFaction'
      &pushregister 3
      &setMember
      &pushregister 1
      &pushsdbgm 76							//'factionObserverMap'
      &enumerateValue
     label24:
      &setRegister r:0
      &pushnull
      &equals
      &jnz label25      
      &pushregister 0
      &setRegister r:4
      &pop
      &pushregister 1
      &pushsdbgm 76							//'factionObserverMap'
      &pushregister 4
      &getMember
      &setRegister r:2
      &pop
      &pushregister 3
      &pushone
      &pushregister 2
      &dcallmp 77							// OnCurrentFactionChanged()
      &jmp label24      
     label25:
      &pushregister 1
      &pushsdbgm 75							//'currentFaction'
      &pushsdb 78							//'Yuriko'
      &equals
      &not
      &jnz label26      
      &pushzero
      &dcallfp 79							// UseYurikoOffsets()
     label26:
    &end // of function SetCurrentFaction

    &function2 UseYurikoOffsets () (r:1='_root')
      &pushsdbgv 3							//'main'
      &pushsdbgm 20							//'lowerSidebar'
      &pushsdb 51							//'_x'
      &pushregister 1
      &pushsdbgm 45							//'displayLeft'
      &pushsdbgv 3							//'main'
      &pushsdbgm 68							//'_width'
      &pushbyte 2
      &divide
      &add
      &setMember
      &pushsdbgv 3							//'main'
      &pushsdbgm 44							//'expPanel'
      &pushsdb 51							//'_x'
      &pushregister 1
      &pushsdbgm 45							//'displayLeft'
      &pushsdbgv 3							//'main'
      &pushsdbgm 68							//'_width'
      &pushbyte 2
      &divide
      &add
      &setMember
      &pushsdbgv 3							//'main'
      &pushsdbgm 25							//'playerPowersBar'
      &pushsdb 51							//'_x'
      &pushregister 1
      &pushsdbgm 45							//'displayLeft'
      &pushsdbgv 3							//'main'
      &pushsdbgm 68							//'_width'
      &pushbyte 2
      &divide
      &add
      &setMember
      &pushsdbgv 3							//'main'
      &pushsdbgm 25							//'playerPowersBar'
      &pushsdb 58							//'_y'
      &pushregister 1
      &pushsdbgm 46							//'displayTop'
      &pushsdbgv 3							//'main'
      &pushsdbgm 69							//'_height'
      &add
      &setMember
    &end // of function UseYurikoOffsets

    &function2 GlobalToLocalX (r:5='clip', r:6='x') ()
      &pushregister 5
      &pushsdbgm 80							//'_root'
      &setRegister r:4
      &pop
      &pushzero
      &setRegister r:2
      &pop
      &pushone
      &setRegister r:3
      &pop
      &pushsdb 81							//'ancestor'
      &pushregister 5
      &setVariable
     label27:
      &pushsdbgv 81							//'ancestor'
      &pushregister 4
      &equals
      &not
      &not
      &jnz label28      
      &pushsdbgv 81							//'ancestor'
      &pushsdbgm 82							//'_xscale'
      &pushfloat 0.009999999776483
      &multiply
      &setRegister r:1
      &pop
      &pushregister 2
      &pushregister 1
      &multiply
      &setRegister r:2
      &pop
      &pushregister 2
      &pushsdbgv 81							//'ancestor'
      &pushsdbgm 51							//'_x'
      &subtract
      &setRegister r:2
      &pop
      &pushregister 3
      &pushregister 1
      &divide
      &setRegister r:3
      &pop
      &pushsdb 81							//'ancestor'
      &pushsdbgv 81							//'ancestor'
      &pushsdbgm 52							//'_parent'
      &setVariable
      &jmp label27      
     label28:
      &pushregister 6
      &pushregister 2
      &add
      &pushregister 3
      &multiply
      &return
    &end // of function GlobalToLocalX

    &function2 GlobalToLocalY (r:5='clip', r:6='y') ()
      &pushregister 5
      &pushsdbgm 80							//'_root'
      &setRegister r:4
      &pop
      &pushzero
      &setRegister r:2
      &pop
      &pushone
      &setRegister r:3
      &pop
      &pushsdb 81							//'ancestor'
      &pushregister 5
      &setVariable
     label29:
      &pushsdbgv 81							//'ancestor'
      &pushregister 4
      &equals
      &not
      &not
      &jnz label30      
      &pushsdbgv 81							//'ancestor'
      &pushsdbgm 83							//'_yscale'
      &pushfloat 0.009999999776483
      &multiply
      &setRegister r:1
      &pop
      &pushregister 2
      &pushregister 1
      &multiply
      &setRegister r:2
      &pop
      &pushregister 2
      &pushsdbgv 81							//'ancestor'
      &pushsdbgm 58							//'_y'
      &subtract
      &setRegister r:2
      &pop
      &pushregister 3
      &pushregister 1
      &divide
      &setRegister r:3
      &pop
      &pushsdb 81							//'ancestor'
      &pushsdbgv 81							//'ancestor'
      &pushsdbgm 52							//'_parent'
      &setVariable
      &jmp label29      
     label30:
      &pushregister 6
      &pushregister 2
      &add
      &pushregister 3
      &multiply
      &return
    &end // of function GlobalToLocalY

    &function2 UpdateUpperSidebarVisibility () ()
      &pushsdbgv 70							//'letterBoxVisible'
      &dup
      &not
      &jnz label31      
      &pop
      &pushsdbgv 72							//'showRadarDuringLetterBox'
     label31:
      &dup
      &jnz label32      
      &pop
      &pushsdbgv 7							//'mainVisibility'
      &dup
      &not
      &jnz label32      
      &pop
      &pushsdbgv 9							//'upperSidebarExplicitlyHidden'
      &not
     label32:
      &setRegister r:1
      &pop
      &pushregister 1
      &dup
      &not
      &jnz label33      
      &pop
      &pushsdbgv 84							//'upperSidebarActualVisibility'
      &not
     label33:
      &not
      &jnz label34      
      &pushsdb 71							//'_fadeIn'
      &pushone
      &pushsdbgv 4							//'upperSidebarLayer'
      &dcallmp 12							// gotoAndPlay()
      &pushsdb 84							//'upperSidebarActualVisibility'
      &pushtrue
      &setVariable
      &jmp label36      
     label34:
      &pushregister 1
      &not
      &dup
      &not
      &jnz label35      
      &pop
      &pushsdbgv 84							//'upperSidebarActualVisibility'
     label35:
      &not
      &jnz label36      
      &pushsdb 73							//'_fadeOut'
      &pushone
      &pushsdbgv 4							//'upperSidebarLayer'
      &dcallmp 12							// gotoAndPlay()
      &pushsdb 84							//'upperSidebarActualVisibility'
      &pushfalse
      &setVariable
     label36:
    &end // of function UpdateUpperSidebarVisibility

    &function onUnloading (    )    
      &pushsdb 85							//'TacticalHUD.swf onUnloading()'
      &trace
      &pushzero
      &pushsdbgv 86							//'gadgetFlashStage'
      &dcallmp 87							// onUnloading()
    &end // of function onUnloading

    &function onUnload (    )    
      &pushsdb 88							//'TacticalHUD.swf onUnload()'
      &trace
      &pushsdb 89							//'Hide'
      &delete2
      &pop
      &pushsdb 90							//'Show'
      &delete2
      &pop
      &pushsdb 91							//'FadeIn'
      &delete2
      &pop
      &pushsdb 92							//'FadeOut'
      &delete2
      &pop
      &pushsdb 93							//'HideShroud'
      &delete2
      &pop
      &pushsdb 94							//'GetInputShield'
      &delete2
      &pop
      &pushsdb 95							//'GetHelpBox'
      &delete2
      &pop
      &pushsdb 96							//'GetSelectionFeedbackSurface'
      &delete2
      &pop
      &pushsdb 97							//'GetRadar'
      &delete2
      &pop
      &pushsdb 98							//'GetMainCommandBar'
      &delete2
      &pop
      &pushsdb 99							//'GetSelectionDetailsPanel'
      &delete2
      &pop
      &pushsdb 100							//'GetResourceDisplay'
      &delete2
      &pop
      &pushsdb 101							//'GetCommandModeBar'
      &delete2
      &pop
      &pushsdb 102							//'GetEnergyMeter'
      &delete2
      &pop
      &pushsdb 103							//'GetPlayerPowerBar'
      &delete2
      &pop
      &pushsdb 104							//'GetPlayerPowerTimerDisplay'
      &delete2
      &pop
      &pushsdb 105							//'GetDialogStage'
      &delete2
      &pop
      &pushsdb 106							//'GetCutSceneStage'
      &delete2
      &pop
      &pushsdb 107							//'GetObjectiveIndicatorStage'
      &delete2
      &pop
      &pushsdb 108							//'GetFloatingTextSurface'
      &delete2
      &pop
      &pushsdb 109							//'GetMessageSurface'
      &delete2
      &pop
      &pushsdb 110							//'GetTimerSurface'
      &delete2
      &pop
      &pushsdb 111							//'GetPopUpMetaStage'
      &delete2
      &pop
      &pushsdb 112							//'GetObserverControlPanel'
      &delete2
      &pop
      &pushsdb 113							//'GetTelestratorControlPanel'
      &delete2
      &pop
      &pushsdb 114							//'GetReplayControlPanel'
      &delete2
      &pop
      &pushsdb 115							//'GetVoiceChatControlPanel'
      &delete2
      &pop
      &pushsdb 116							//'GetTickerPanel'
      &delete2
      &pop
      &pushsdb 117							//'GetTelestratorSurface'
      &delete2
      &pop
      &pushsdb 118							//'GetSubtitlePanelSurface'
      &delete2
      &pop
      &pushsdb 119							//'GetCoOpAIOrderPanel'
      &delete2
      &pop
      &pushsdb 120							//'GetBeaconStage'
      &delete2
      &pop
      &pushsdb 121							//'GetDrawableOverlayStage'
      &delete2
      &pop
      &pushsdb 122							//'GetPlayerExperiencePanel'
      &delete2
      &pop
      &pushsdb 123							//'SetDisplayRegion'
      &delete2
      &pop
      &pushsdb 124							//'SetLetterBoxVisibility'
      &delete2
      &pop
      &pushsdb 125							//'SetCurrentFaction'
      &delete2
      &pop
      &pushsdb 53							//'GlobalToLocalX'
      &delete2
      &pop
      &pushsdb 59							//'GlobalToLocalY'
      &delete2
      &pop
      &pushsdb 8							//'UpdateUpperSidebarVisibility'
      &delete2
      &pop
      &pushsdb 87							//'onUnloading'
      &delete2
      &pop
      &pushsdb 126							//'onUnload'
      &delete2
      &pop
    &end // of function onUnload

    &pushsdbgv 127							//'initialized'
    &not
    &not
    &jnz label47    
    &pushsdb 128							//'TacticalHUD::Initializing'
    &trace
    &pushthisgv
    &pushone
    &pushsdbgv 52							//'_parent'
    &dcallmp 130							// OnContentLoaded()
    &pushsdb 131							//'TacticalHUDBeaconStage.swf'
    &pushone
    &pushsdbgv 41							//'beaconStage'
    &dcallmp 132							// loadMovie()
    &pushsdb 133							//'ig_DrawableOverlayStage.swf'
    &pushone
    &pushsdbgv 43							//'drawableOverlayStage'
    &dcallmp 132							// loadMovie()
    &pushsdb 134							//'igm_gadgetFlashStage.swf'
    &pushone
    &pushsdbgv 86							//'gadgetFlashStage'
    &dcallmp 132							// loadMovie()
    &pushsdb 2							//'hideMainCount'
    &pushone
    &setVariable
    &pushsdbgv 3							//'main'
    &pushsdb 1							//'_visible'
    &pushfalse
    &setMember
    &pushsdb 7							//'mainVisibility'
    &pushfalse
    &setVariable
    &pushsdb 70							//'letterBoxVisible'
    &pushfalse
    &setVariable
    &pushsdb 72							//'showRadarDuringLetterBox'
    &pushfalse
    &setVariable
    &pushsdb 9							//'upperSidebarExplicitlyHidden'
    &pushfalse
    &setVariable
    &pushsdb 84							//'upperSidebarActualVisibility'
    &pushfalse
    &setVariable
    &pushsdb 49							//'leftAnchoredClips'
    &pushsdbgv 135							//'watermark'
    &pushsdbgv 3							//'main'
    &pushsdbgm 44							//'expPanel'
    &pushsdbgv 3							//'main'
    &pushsdbgm 40							//'coOpAIOrderPanel'
    &pushsdbgv 3							//'main'
    &pushsdbgm 37							//'ticker'
    &pushsdbgv 3							//'main'
    &pushsdbgm 26							//'specialPowerTimerDisplay'
    &pushsdbgv 3							//'main'
    &pushsdbgm 25							//'playerPowersBar'
    &pushsdbgv 3							//'main'
    &pushsdbgm 30							//'messageSurface'
    &pushsdbgv 3							//'main'
    &pushsdbgm 31							//'timerSurface'
    &pushbyte 8
    &initArray
    &setVariable
    &pushsdb 55							//'rightAnchoredClips'
    &pushsdbgv 3							//'main'
    &pushsdbgm 20							//'lowerSidebar'
    &pushsdbgv 4							//'upperSidebarLayer'
    &pushsdbgv 3							//'main'
    &pushsdbgm 36							//'voiceChatControl'
    &pushsdbgv 3							//'main'
    &pushsdbgm 33							//'observerControl'
    &pushsdbgv 3							//'main'
    &pushsdbgm 34							//'telestratorControl'
    &pushsdbgv 3							//'main'
    &pushsdbgm 35							//'replayControl'
    &pushbyte 6
    &initArray
    &setVariable
    &pushsdb 57							//'topAnchoredClips'
    &pushsdbgv 4							//'upperSidebarLayer'
    &pushsdbgv 3							//'main'
    &pushsdbgm 40							//'coOpAIOrderPanel'
    &pushsdbgv 3							//'main'
    &pushsdbgm 36							//'voiceChatControl'
    &pushsdbgv 3							//'main'
    &pushsdbgm 33							//'observerControl'
    &pushsdbgv 3							//'main'
    &pushsdbgm 34							//'telestratorControl'
    &pushsdbgv 3							//'main'
    &pushsdbgm 35							//'replayControl'
    &pushsdbgv 3							//'main'
    &pushsdbgm 25							//'playerPowersBar'
    &pushsdbgv 3							//'main'
    &pushsdbgm 30							//'messageSurface'
    &pushsdbgv 3							//'main'
    &pushsdbgm 31							//'timerSurface'
    &pushbyte 9
    &initArray
    &setVariable
    &pushsdb 61							//'bottomAnchoredClips'
    &pushsdbgv 135							//'watermark'
    &pushsdbgv 39							//'subtitlePanelSurface'
    &pushsdbgv 3							//'main'
    &pushsdbgm 26							//'specialPowerTimerDisplay'
    &pushsdbgv 3							//'main'
    &pushsdbgm 44							//'expPanel'
    &pushsdbgv 3							//'main'
    &pushsdbgm 20							//'lowerSidebar'
    &pushsdbgv 3							//'main'
    &pushsdbgm 37							//'ticker'
    &pushbyte 6
    &initArray
    &setVariable
    &pushsdb 63							//'centerAnchoredClips'
    &pushsdbgv 3							//'main'
    &pushsdbgm 29							//'objectiveIndicatorStage'
    &pushsdbgv 3							//'main'
    &pushsdbgm 32							//'popUpMetaStage'
    &pushsdbgv 28							//'cutSceneStage'
    &pushsdbgv 27							//'dialogStage'
    &pushbyte 4
    &initArray
    &setVariable
    &pushsdb 67							//'fullScreenClips'
    &pushsdbgv 42							//'fadeSurface'
    &pushsdbgv 6							//'floatingTextSurface'
    &pushsdbgv 5							//'selectionFeedbackSurface'
    &pushsdbgv 3							//'main'
    &pushsdbgm 38							//'telestratorSurface'
    &pushsdbgv 0							//'letterBoxLayer'
    &pushsdbgv 11							//'shroud'
    &pushbyte 6
    &initArray
    &setVariable
    &pushsdb 54							//'leftMargins'
    &pushsdbgv 49							//'leftAnchoredClips'
    &pushsdbgm 50							//'length'
    &pushone
    &pushsdb 136							//'Array'
    &new
    &setVariable
    &pushsdb 137							//'i'
    &pushzero
    &varEquals
   label37:
    &pushsdbgv 137							//'i'
    &pushsdbgv 49							//'leftAnchoredClips'
    &pushsdbgm 50							//'length'
    &lessThan
    &not
    &jnz label38    
    &pushsdb 138							//'clip'
    &pushsdbgv 49							//'leftAnchoredClips'
    &pushsdbgv 137							//'i'
    &getMember
    &varEquals
    &pushsdbgv 54							//'leftMargins'
    &pushsdbgv 137							//'i'
    &pushsdbgv 138							//'clip'
    &pushsdbgm 51							//'_x'
    &pushzero
    &pushsdbgv 138							//'clip'
    &pushsdbgm 52							//'_parent'
    &pushbyte 2
    &pushsdb 53							//'GlobalToLocalX'
    &callFunction
    &subtract
    &setMember
    &pushsdb 137							//'i'
    &pushsdbgv 137							//'i'
    &increment
    &setVariable
    &jmp label37    
   label38:
    &pushsdb 56							//'rightMargins'
    &pushsdbgv 55							//'rightAnchoredClips'
    &pushsdbgm 50							//'length'
    &pushone
    &pushsdb 136							//'Array'
    &new
    &setVariable
    &pushsdb 137							//'i'
    &pushzero
    &varEquals
   label39:
    &pushsdbgv 137							//'i'
    &pushsdbgv 55							//'rightAnchoredClips'
    &pushsdbgm 50							//'length'
    &lessThan
    &not
    &jnz label40    
    &pushsdb 138							//'clip'
    &pushsdbgv 55							//'rightAnchoredClips'
    &pushsdbgv 137							//'i'
    &getMember
    &varEquals
    &pushsdbgv 56							//'rightMargins'
    &pushsdbgv 137							//'i'
    &pushfloat 1365.300048828125000
    &pushsdbgv 138							//'clip'
    &pushsdbgm 52							//'_parent'
    &pushbyte 2
    &pushsdb 53							//'GlobalToLocalX'
    &callFunction
    &pushsdbgv 138							//'clip'
    &pushsdbgm 51							//'_x'
    &subtract
    &setMember
    &pushsdb 137							//'i'
    &pushsdbgv 137							//'i'
    &increment
    &setVariable
    &jmp label39    
   label40:
    &pushsdb 60							//'topMargins'
    &pushsdbgv 57							//'topAnchoredClips'
    &pushsdbgm 50							//'length'
    &pushone
    &pushsdb 136							//'Array'
    &new
    &setVariable
    &pushsdb 137							//'i'
    &pushzero
    &varEquals
   label41:
    &pushsdbgv 137							//'i'
    &pushsdbgv 57							//'topAnchoredClips'
    &pushsdbgm 50							//'length'
    &lessThan
    &not
    &jnz label42    
    &pushsdb 138							//'clip'
    &pushsdbgv 57							//'topAnchoredClips'
    &pushsdbgv 137							//'i'
    &getMember
    &varEquals
    &pushsdbgv 60							//'topMargins'
    &pushsdbgv 137							//'i'
    &pushsdbgv 138							//'clip'
    &pushsdbgm 58							//'_y'
    &pushzero
    &pushsdbgv 138							//'clip'
    &pushsdbgm 52							//'_parent'
    &pushbyte 2
    &pushsdb 59							//'GlobalToLocalY'
    &callFunction
    &subtract
    &setMember
    &pushsdb 137							//'i'
    &pushsdbgv 137							//'i'
    &increment
    &setVariable
    &jmp label41    
   label42:
    &pushsdb 62							//'bottomMargins'
    &pushsdbgv 61							//'bottomAnchoredClips'
    &pushsdbgm 50							//'length'
    &pushone
    &pushsdb 136							//'Array'
    &new
    &setVariable
    &pushsdb 137							//'i'
    &pushzero
    &varEquals
   label43:
    &pushsdbgv 137							//'i'
    &pushsdbgv 61							//'bottomAnchoredClips'
    &pushsdbgm 50							//'length'
    &lessThan
    &not
    &jnz label44    
    &pushsdb 138							//'clip'
    &pushsdbgv 61							//'bottomAnchoredClips'
    &pushsdbgv 137							//'i'
    &getMember
    &varEquals
    &pushsdbgv 62							//'bottomMargins'
    &pushsdbgv 137							//'i'
    &pushshort 768
    &pushsdbgv 138							//'clip'
    &pushsdbgm 52							//'_parent'
    &pushbyte 2
    &pushsdb 59							//'GlobalToLocalY'
    &callFunction
    &pushsdbgv 138							//'clip'
    &pushsdbgm 58							//'_y'
    &subtract
    &setMember
    &pushsdb 137							//'i'
    &pushsdbgv 137							//'i'
    &increment
    &setVariable
    &jmp label43    
   label44:
    &pushsdb 64							//'centerOffsets'
    &pushsdbgv 63							//'centerAnchoredClips'
    &pushsdbgm 50							//'length'
    &pushone
    &pushsdb 136							//'Array'
    &new
    &setVariable
    &pushsdb 137							//'i'
    &pushzero
    &varEquals
   label45:
    &pushsdbgv 137							//'i'
    &pushsdbgv 63							//'centerAnchoredClips'
    &pushsdbgm 50							//'length'
    &lessThan
    &not
    &jnz label46    
    &pushsdb 138							//'clip'
    &pushsdbgv 63							//'centerAnchoredClips'
    &pushsdbgv 137							//'i'
    &getMember
    &varEquals
    &pushsdbgv 64							//'centerOffsets'
    &pushsdbgv 137							//'i'
    &pushzero
    &pushsdb 139							//'Object'
    &new
    &setMember
    &pushsdbgv 64							//'centerOffsets'
    &pushsdbgv 137							//'i'
    &getMember
    &pushsdb 65							//'x'
    &pushsdbgv 138							//'clip'
    &pushsdbgm 51							//'_x'
    &pushfloat 682.650024414062500
    &pushsdbgv 138							//'clip'
    &pushsdbgm 52							//'_parent'
    &pushbyte 2
    &pushsdb 53							//'GlobalToLocalX'
    &callFunction
    &subtract
    &setMember
    &pushsdbgv 64							//'centerOffsets'
    &pushsdbgv 137							//'i'
    &getMember
    &pushsdb 66							//'y'
    &pushsdbgv 138							//'clip'
    &pushsdbgm 58							//'_y'
    &pushshort 384
    &pushsdbgv 138							//'clip'
    &pushsdbgm 52							//'_parent'
    &pushbyte 2
    &pushsdb 59							//'GlobalToLocalY'
    &callFunction
    &subtract
    &setMember
    &pushsdb 137							//'i'
    &pushsdbgv 137							//'i'
    &increment
    &setVariable
    &jmp label45    
   label46:
    &pushsdbgv 80							//'_root'
    &pushsdb 45							//'displayLeft'
    &pushzero
    &setMember
    &pushsdbgv 80							//'_root'
    &pushsdb 46							//'displayTop'
    &pushzero
    &setMember
    &pushsdbgv 80							//'_root'
    &pushsdb 47							//'displayWidth'
    &pushfloat 1365.300048828125000
    &setMember
    &pushsdbgv 80							//'_root'
    &pushsdb 48							//'displayHeight'
    &pushshort 768
    &setMember
    &pushsdb 127							//'initialized'
    &pushtrue
    &setVariable
    &pushsdb 140							//'FSCommand:'
    &pushsdb 141							//'OnInitialized'
    &pushthisgv
    &pushbyte 2
    &pushsdb 142							//'qualifyName'
    &callFunction
    &concat
    &pushsdb 143							//''
    &getURL2
    &pushsdbgv 144							//'extern'
    &pushsdbgm 145							//'InGame'
    &not
    &not
    &jnz label47    
    &pushsdbgv 11							//'shroud'
    &pushsdb 1							//'_visible'
    &pushfalse
    &setMember
    &pushshort 1024
    &pushshort 768
    &pushzero
    &pushzero
    &pushbyte 4
    &dcallfp 123							// SetDisplayRegion()
    &pushsdb 146							//'Soviet'
    &pushone
    &dcallfp 125							// SetCurrentFaction()
    &pushzero
    &dcallfp 91							// FadeIn()
   label47:
  &end // of frame 0

  &frame 0
    &stop
  &end // of frame 0
  
  &importAssets &from '../../common/hud/ig_libHUD.swf'
    'Dummy' &as 1
  &end // of importAssets
  
  &importAssets &from '../../common/hud/ig_libHUD.swf'
    'SimpleRenderingSurface' &as 2
  &end // of importAssets

  &defineMovieClip 4 // total frames: 1
  &end // of defineMovieClip 4

  &defineMovieClip 5 // total frames: 1

    &placeMovieClip 4 
    
      &onClipEvent &load
        &pushs ''
        &pushbyte 7
        &pushfalse
        &setProperty
      &end
    &end // of placeMovieClip 4
  &end // of defineMovieClip 5

  &defineMovieClip 6 // total frames: 1

    &frame 0
      &constants 'initialized', 'TacticalHUDSelectionDetails.swf', 'selectionDetails', 'loadMovie'  
      &pushsdbgv 0							//'initialized'
      &not
      &not
      &jnz label1      
      &pushsdb 1							//'TacticalHUDSelectionDetails.swf'
      &pushone
      &pushsdbgv 2							//'selectionDetails'
      &dcallmp 3							// loadMovie()
      &pushsdb 0							//'initialized'
      &pushtrue
      &setVariable
     label1:
      &stop
    &end // of frame 0
  &end // of defineMovieClip 6

  &defineMovieClip 7 // total frames: 1

    &placeMovieClip 4 
    
      &onClipEvent &load
        &pushs ''
        &pushbyte 7
        &pushfalse
        &setProperty
      &end
    &end // of placeMovieClip 4
  &end // of defineMovieClip 7

  &defineMovieClip 8 // total frames: 1

    &placeMovieClip 4 
    
      &onClipEvent &load
        &pushs ''
        &pushbyte 7
        &pushfalse
        &setProperty
      &end
    &end // of placeMovieClip 4
  &end // of defineMovieClip 8

  &defineMovieClip 9 // total frames: 1

    &placeMovieClip 4 
    
      &onClipEvent &load
        &pushs ''
        &pushbyte 7
        &pushfalse
        &setProperty
      &end
    &end // of placeMovieClip 4
  &end // of defineMovieClip 9

  &defineMovieClip 10 // total frames: 1

    &frame 0
      &constants 'removeMovieClip', 'LoadMovieStage', 'attachMovie', 'Pointer', 'encode', 'AddStage', 'RemoveStage', 'onUnload', 'initialized', 'FSCommand:', 'OnInitialized', 'this', 'qualifyName', ''  
      &function2 AddStage (r:4='idArg') (r:1='this')
        &pushregister 4
        &toNumber
        &setRegister r:3
        &pop
        &pushregister 1
        &pushregister 3
        &toString
        &getMember
        &setRegister r:2
        &pop
        &pushregister 2
        &pushundef
        &equals
        &not
        &not
        &jnz label1        
        &pushzero
        &pushregister 2
        &dcallmp 0							// removeMovieClip()
       label1:
        &pushregister 3
        &pushregister 3
        &toString
        &pushsdb 1							//'LoadMovieStage'
        &pushbyte 3
        &pushregister 1
        &pushsdb 2							//'attachMovie'
        &callMethod
        &setRegister r:2
        &pop
        &pushregister 2
        &pushone
        &pushsdbgv 3							//'Pointer'
        &pushsdb 4							//'encode'
        &callMethod
        &return
      &end // of function AddStage

      &function2 RemoveStage (r:4='idArg') (r:1='this')
        &pushregister 4
        &toNumber
        &setRegister r:3
        &pop
        &pushregister 1
        &pushregister 3
        &toString
        &getMember
        &setRegister r:2
        &pop
        &pushzero
        &pushregister 2
        &dcallmp 0							// removeMovieClip()
      &end // of function RemoveStage

      &function onUnload (      )      
        &pushsdb 5							//'AddStage'
        &delete2
        &pop
        &pushsdb 6							//'RemoveStage'
        &delete2
        &pop
        &pushsdb 7							//'onUnload'
        &delete2
        &pop
      &end // of function onUnload

      &pushsdbgv 8							//'initialized'
      &not
      &not
      &jnz label2      
      &pushsdb 8							//'initialized'
      &pushtrue
      &setVariable
      &pushsdb 9							//'FSCommand:'
      &pushsdb 10							//'OnInitialized'
      &pushthisgv
      &pushbyte 2
      &pushsdb 12							//'qualifyName'
      &callFunction
      &concat
      &pushsdb 13							//''
      &getURL2
     label2:
      &stop
    &end // of frame 0
  &end // of defineMovieClip 10

  &defineMovieClip 11 // total frames: 1

    &frame 0
      &constants 'initialized', 'HelpBox.swf', 'helpBox', 'loadMovie', 'TacticalHUDObjectiveIndicatorStage.swf', 'objectiveIndicatorStage', 'TacticalHUDPlayerPowersBar.swf', 'playerPowersBar', 'HUDTickerPanel.swf', 'ticker', 'TacticalHUDSpecialPowerTimers.swf', 'specialPowerTimerDisplay', 'igm_replayControlPanel.swf', 'replayControl', 'TacticalHUDTelestratorControlPanel.swf', 'telestratorControl', 'TacticalHUDObserverControlPanel.swf', 'observerControl', 'HUDVoiceChatControlPanel.swf', 'voiceChatControl', 'TacticalHUDCoOpAIOrderPanel.swf', 'coOpAIOrderPanel', 'TacticalHUDExperiencePanel.swf', 'expPanel'  
      &pushsdbgv 0							//'initialized'
      &not
      &not
      &jnz label1      
      &pushsdb 1							//'HelpBox.swf'
      &pushone
      &pushsdbgv 2							//'helpBox'
      &dcallmp 3							// loadMovie()
      &pushsdb 4							//'TacticalHUDObjectiveIndicatorStage.swf'
      &pushone
      &pushsdbgv 5							//'objectiveIndicatorStage'
      &dcallmp 3							// loadMovie()
      &pushsdb 6							//'TacticalHUDPlayerPowersBar.swf'
      &pushone
      &pushsdbgv 7							//'playerPowersBar'
      &dcallmp 3							// loadMovie()
      &pushsdb 8							//'HUDTickerPanel.swf'
      &pushone
      &pushsdbgv 9							//'ticker'
      &dcallmp 3							// loadMovie()
      &pushsdb 10							//'TacticalHUDSpecialPowerTimers.swf'
      &pushone
      &pushsdbgv 11							//'specialPowerTimerDisplay'
      &dcallmp 3							// loadMovie()
      &pushsdb 12							//'igm_replayControlPanel.swf'
      &pushone
      &pushsdbgv 13							//'replayControl'
      &dcallmp 3							// loadMovie()
      &pushsdb 14							//'TacticalHUDTelestratorControlPanel.swf'
      &pushone
      &pushsdbgv 15							//'telestratorControl'
      &dcallmp 3							// loadMovie()
      &pushsdb 16							//'TacticalHUDObserverControlPanel.swf'
      &pushone
      &pushsdbgv 17							//'observerControl'
      &dcallmp 3							// loadMovie()
      &pushsdb 18							//'HUDVoiceChatControlPanel.swf'
      &pushone
      &pushsdbgv 19							//'voiceChatControl'
      &dcallmp 3							// loadMovie()
      &pushsdb 20							//'TacticalHUDCoOpAIOrderPanel.swf'
      &pushone
      &pushsdbgv 21							//'coOpAIOrderPanel'
      &dcallmp 3							// loadMovie()
      &pushsdb 22							//'TacticalHUDExperiencePanel.swf'
      &pushone
      &pushsdbgv 23							//'expPanel'
      &dcallmp 3							// loadMovie()
      &pushsdb 0							//'initialized'
      &pushtrue
      &setVariable
     label1:
      &stop
    &end // of frame 0
  &end // of defineMovieClip 11

  &defineMovieClip 12 // total frames: 1
  &end // of defineMovieClip 12

  &defineMovieClip 13 // total frames: 33

    &frame 0
      &constants 'initialized', 'content', '_visible', '_parent', 'letterBoxVisible'  
      &pushsdbgv 0							//'initialized'
      &not
      &not
      &jnz label2      
      &pushsdbgv 1							//'content'
      &pushsdb 2							//'_visible'
      &pushfalse
      &setMember
      &pushsdbgv 3							//'_parent'
      &pushsdbgm 4							//'letterBoxVisible'
      &not
      &jnz label1      
      &gotoLabel '_fadeIn'
      &play
     label1:
      &pushsdb 0							//'initialized'
      &pushtrue
      &setVariable
     label2:
    &end // of frame 0

    &frame 0
      &pushsgv 'content'
      &pushs '_visible'
      &pushfalse
      &setMember
    &end // of frame 0

    &frame 1
      &stop
    &end // of frame 1

    &frame 2
      &pushsgv 'content'
      &pushs '_visible'
      &pushtrue
      &setMember
    &end // of frame 2

    &frame 21
      &stop
    &end // of frame 21

    &frame 32
      &gotoLabel '_hide'
      &play
    &end // of frame 32
  &end // of defineMovieClip 13

  &defineMovieClip 26 // total frames: 40

    &frame 0
      &constants 'DetachFactionObserver', 'OnCurrentFactionChanged', 'onUnload', 'factionSkinned', '_root', 'GetCurrentFaction', 'this', 'AttachFactionObserver'  
      &function2 OnCurrentFactionChanged (r:1='faction') ()
        &pushregister 1
        &gotoAndPlay      &end // of function OnCurrentFactionChanged

      &function2 onUnload () (r:1='this', r:2='_root')
        &pushregister 1
        &pushone
        &pushregister 2
        &dcallmp 0							// DetachFactionObserver()
        &pushsdb 1							//'OnCurrentFactionChanged'
        &delete2
        &pop
        &pushsdb 2							//'onUnload'
        &delete2
        &pop
      &end // of function onUnload

      &pushsdbgv 3							//'factionSkinned'
      &not
      &not
      &jnz label1      
      &pushzero
      &pushsdbgv 4							//'_root'
      &pushsdb 5							//'GetCurrentFaction'
      &callMethod
      &gotoAndPlay      &pushthisgv
      &pushone
      &pushsdbgv 4							//'_root'
      &dcallmp 7							// AttachFactionObserver()
      &pushsdb 3							//'factionSkinned'
      &pushtrue
      &setVariable
     label1:
    &end // of frame 0

    &frame 9
      &stop
    &end // of frame 9

    &frame 19
      &stop
    &end // of frame 19

    &frame 29
      &stop
    &end // of frame 29

    &frame 39
      &stop
    &end // of frame 39
  &end // of defineMovieClip 26

  &defineMovieClip 27 // total frames: 1
  &end // of defineMovieClip 27
  
  &importAssets &from '../../common/hud/ig_libHUD.swf'
    'WhiteRectangle' &as 28
  &end // of importAssets

  &defineMovieClip 29 // total frames: 1
  &end // of defineMovieClip 29

  &defineMovieClip 32 // total frames: 1
  &end // of defineMovieClip 32

  &defineMovieClip 35 // total frames: 1

    &frame 0
      &constants '', 'highlightClip', 'highlightDummy', 'CreateHUDGadgetFlash', 'Close', 'SetVisibility', 'SetHighlighted', 'onUnload', 'initialized', 'FSCommand:', 'OnInitialized', 'this', 'qualifyName'  
      &function2 SetVisibility (r:1='visArg') ()
        &pushsdb 0							//''
        &pushbyte 7
        &pushregister 1
        &pushzero
        &equals
        &not
        &setProperty
      &end // of function SetVisibility

      &function2 SetHighlighted (r:3='highlightArg') (r:1='_global')
        &pushregister 3
        &pushzero
        &equals
        &not
        &setRegister r:2
        &pop
        &pushregister 2
        &not
        &jnz label2        
        &pushsdbgv 1							//'highlightClip'
        &pushundef
        &equals
        &not
        &jnz label1        
        &pushsdb 1							//'highlightClip'
        &pushsdbgv 2							//'highlightDummy'
        &pushone
        &pushregister 1
        &dcallmsv 3							// CreateHUDGadgetFlash()
       label1:
        &jmp label3        
       label2:
        &pushzero
        &pushsdbgv 1							//'highlightClip'
        &dcallmp 4							// Close()
        &pushsdb 1							//'highlightClip'
        &pushundef
        &setVariable
       label3:
      &end // of function SetHighlighted

      &function onUnload (      )      
        &pushzero
        &pushsdbgv 1							//'highlightClip'
        &dcallmp 4							// Close()
        &pushsdb 5							//'SetVisibility'
        &delete2
        &pop
        &pushsdb 6							//'SetHighlighted'
        &delete2
        &pop
        &pushsdb 7							//'onUnload'
        &delete2
        &pop
      &end // of function onUnload

      &pushsdbgv 8							//'initialized'
      &not
      &not
      &jnz label4      
      &pushsdb 8							//'initialized'
      &pushtrue
      &setVariable
      &pushsdb 9							//'FSCommand:'
      &pushsdb 10							//'OnInitialized'
      &pushthisgv
      &pushbyte 2
      &pushsdb 12							//'qualifyName'
      &callFunction
      &concat
      &pushsdb 0							//''
      &getURL2
     label4:
      &stop
    &end // of frame 0

    &placeMovieClip 32 
    
      &onClipEvent &load
        &pushs 'text'
        &pushs '$'
        &pushs 'Resource'
        &pushsgv '_parent'
        &pushbyte 2
        &pushs 'qualifyName'
        &callFunction
        &add
        &setVariable
      &end
    &end // of placeMovieClip 32
  &end // of defineMovieClip 35

  &defineMovieClip 36 // total frames: 1

    &placeMovieClip 4 
    
      &onClipEvent &load
        &pushs ''
        &pushbyte 7
        &pushfalse
        &setProperty
      &end
    &end // of placeMovieClip 4
  &end // of defineMovieClip 36
  
  &importAssets &from 'HelpBox.swf'
    'HelpBoxSite' &as 37
  &end // of importAssets

  &defineMovieClip 38 // total frames: 1

    &placeMovieClip 4 
    
      &onClipEvent &load
        &pushs ''
        &pushbyte 7
        &pushfalse
        &setProperty
      &end
    &end // of placeMovieClip 4
  &end // of defineMovieClip 38

  &defineMovieClip 39 // total frames: 1

    &frame 0
      &constants 'energyMeter', 'helpSite', 'energyMeterHelpSite', 'OnContentLoaded', 'onUnload', 'initialized', 'TacticalHUDCommandModeBar.swf', 'commandModeBar', 'loadMovie', 'TacticalHUDEnergyMeter.swf', 'TacticalHUDRadar.swf', 'radar', 'TacticalHUDMainCommandBar.swf', 'mainCommandBar', '_root', 'GetCurrentFaction', 'Yuriko', '_visible', 'resourceDisplay'  
      &function2 OnContentLoaded (r:1='clip') ()
        &pushregister 1
        &pushsdbgv 0							//'energyMeter'
        &equals
        &not
        &jnz label1        
        &pushsdbgv 0							//'energyMeter'
        &pushsdb 1							//'helpSite'
        &pushsdbgv 2							//'energyMeterHelpSite'
        &setMember
       label1:
      &end // of function OnContentLoaded

      &function onUnload (      )      
        &pushsdb 3							//'OnContentLoaded'
        &delete2
        &pop
        &pushsdb 4							//'onUnload'
        &delete2
        &pop
      &end // of function onUnload

      &pushsdbgv 5							//'initialized'
      &not
      &not
      &jnz label3      
      &pushsdb 6							//'TacticalHUDCommandModeBar.swf'
      &pushone
      &pushsdbgv 7							//'commandModeBar'
      &dcallmp 8							// loadMovie()
      &pushsdb 9							//'TacticalHUDEnergyMeter.swf'
      &pushone
      &pushsdbgv 0							//'energyMeter'
      &dcallmp 8							// loadMovie()
      &pushsdb 10							//'TacticalHUDRadar.swf'
      &pushone
      &pushsdbgv 11							//'radar'
      &dcallmp 8							// loadMovie()
      &pushsdb 12							//'TacticalHUDMainCommandBar.swf'
      &pushone
      &pushsdbgv 13							//'mainCommandBar'
      &dcallmp 8							// loadMovie()
      &pushzero
      &pushsdbgv 14							//'_root'
      &pushsdb 15							//'GetCurrentFaction'
      &callMethod
      &pushsdb 16							//'Yuriko'
      &equals
      &not
      &jnz label2      
      &pushsdbgv 7							//'commandModeBar'
      &pushsdb 17							//'_visible'
      &pushfalse
      &setMember
      &pushsdbgv 0							//'energyMeter'
      &pushsdb 17							//'_visible'
      &pushfalse
      &setMember
      &pushsdbgv 18							//'resourceDisplay'
      &pushsdb 17							//'_visible'
      &pushfalse
      &setMember
      &pushsdbgv 13							//'mainCommandBar'
      &pushsdb 17							//'_visible'
      &pushfalse
      &setMember
     label2:
      &pushsdb 5							//'initialized'
      &pushtrue
      &setVariable
     label3:
      &stop
    &end // of frame 0

    &placeMovieClip 37 &as 'energyMeterHelpSite'
    
      &onClipEvent &construct
        &pushs 'vertAlignment'
        &pushssv 'top'
        &pushs 'horzAlignment'
        &pushssv 'right'
      &end
    &end // of placeMovieClip 37
  &end // of defineMovieClip 39

  &defineMovieClip 42 // total frames: 1
  &end // of defineMovieClip 42

  &defineMovieClip 45 // total frames: 1
  &end // of defineMovieClip 45

  &defineMovieClip 48 // total frames: 1
  &end // of defineMovieClip 48

  &defineMovieClip 51 // total frames: 1
  &end // of defineMovieClip 51

  &defineMovieClip 52 // total frames: 240
  &end // of defineMovieClip 52

  &defineMovieClip 53 // total frames: 49

    &frame 0
      &pushzero
      &pushsgv '_root'
      &pushs 'GetCurrentFaction'
      &callMethod
      &pushs 'Yuriko'
      &equals
      &not
      &not
      &jnz label1      
      &pushthisgv
      &pushs '_visible'
      &pushfalse
      &setMember
     label1:
    &end // of frame 0

    &frame 48
      &stop
    &end // of frame 48
  &end // of defineMovieClip 53

  &defineMovieClip 54 // total frames: 24

    &frame 0
      &pushsgv 'upperSidebar'
      &pushs '_visible'
      &pushfalse
      &setMember
    &end // of frame 0

    &frame 2
      &stop
    &end // of frame 2

    &frame 3
      &pushsgv 'upperSidebar'
      &pushs '_visible'
      &pushtrue
      &setMember
      &play
    &end // of frame 3

    &frame 9
      &pushsgv 'upperSidebar'
      &pushs '_visible'
      &pushtrue
      &setMember
      &play
    &end // of frame 9

    &frame 15
      &stop
    &end // of frame 15

    &frame 16
      &play
    &end // of frame 16

    &frame 22
      &pushsgv 'upperSidebar'
      &pushs '_visible'
      &pushfalse
      &setMember
    &end // of frame 22

    &frame 23
      &stop
    &end // of frame 23
  &end // of defineMovieClip 54
  
  &importAssets &from '../../common/hud/ig_libHUD.swf'
    'InputShield' &as 55
  &end // of importAssets
  
  &importAssets &from '../../common/hud/ig_libHUD.swf'
    'LoadMovieStage' &as 56
  &end // of importAssets

  &defineMovieClip 57 // total frames: 1

    &frame 0
      &constants 'box', '_alpha', '', 'SetOpacity', 'onUnload', 'initialized', 'FSCommand:', 'OnInitialized', 'this', 'qualifyName'  
      &function2 SetOpacity (r:1='opacity') ()
        &pushsdbgv 0							//'box'
        &pushsdb 1							//'_alpha'
        &pushregister 1
        &pushbyte 100
        &multiply
        &setMember
        &pushsdb 2							//''
        &pushbyte 7
        &pushregister 1
        &pushzero
        &greaterThan
        &setProperty
      &end // of function SetOpacity

      &function onUnload (      )      
        &pushsdb 3							//'SetOpacity'
        &delete2
        &pop
        &pushsdb 4							//'onUnload'
        &delete2
        &pop
      &end // of function onUnload

      &pushsdbgv 5							//'initialized'
      &not
      &not
      &jnz label1      
      &pushsdb 2							//''
      &pushbyte 7
      &pushfalse
      &setProperty
      &pushsdb 5							//'initialized'
      &pushtrue
      &setVariable
      &pushsdb 6							//'FSCommand:'
      &pushsdb 7							//'OnInitialized'
      &pushthisgv
      &pushbyte 2
      &pushsdb 9							//'qualifyName'
      &callFunction
      &concat
      &pushsdb 2							//''
      &getURL2
     label1:
      &stop
    &end // of frame 0
  &end // of defineMovieClip 57

  &defineMovieClip 58 // total frames: 11

    &frame 1
      &stop
    &end // of frame 1

    &frame 10
      &pushs ''
      &pushbyte 7
      &pushfalse
      &setProperty
      &stop
    &end // of frame 10
  &end // of defineMovieClip 58

  &frame 1
    &play
  &end // of frame 1

  &frame 7
    &pushs 'FSCommand:'
    &pushs 'OnFadeInComplete'
    &pushthisgv
    &pushbyte 2
    &pushs 'qualifyName'
    &callFunction
    &concat
    &pushs ''
    &getURL2
  &end // of frame 7

  &frame 13
    &stop
  &end // of frame 13

  &frame 14
    &play
  &end // of frame 14

  &frame 20
    &constants 'hideMainCount', 'main', '_visible', 'FSCommand:', 'OnFadeOutComplete', 'this', 'qualifyName', ''  
    &pushsdbgv 0							//'hideMainCount'
    &pushundef
    &equals
    &not
    &dup
    &not
    &jnz label1    
    &pop
    &pushsdbgv 0							//'hideMainCount'
    &pushsdb 0							//'hideMainCount'
    &pushsdbgv 0							//'hideMainCount'
    &increment
    &setVariable
    &pushzero
    &equals
   label1:
    &not
    &jnz label2    
    &pushsdbgv 1							//'main'
    &pushsdb 2							//'_visible'
    &pushfalse
    &setMember
   label2:
    &stop
    &pushsdb 3							//'FSCommand:'
    &pushsdb 4							//'OnFadeOutComplete'
    &pushthisgv
    &pushbyte 2
    &pushsdb 6							//'qualifyName'
    &callFunction
    &concat
    &pushsdb 7							//''
    &getURL2
  &end // of frame 20
&end
