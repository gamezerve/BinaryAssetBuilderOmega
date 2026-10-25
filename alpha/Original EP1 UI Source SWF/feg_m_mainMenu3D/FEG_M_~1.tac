movie 'E:\depot\CnC\RA3_EP1\PRODUC~1\Data\APTBUI~1\034A3~1.0-D\pc\Output\FE0B1D~1\\FEG_M_~1.eaf' &compressed // flash 7, total frames: 1, frame rate: 30 fps, 1365x768 px

  &frame 0
    &pushs 'screen'
    &pushthisgv
    &pushone
    &pushs 'feg_m_mainMenu3D'
    &new
    &varEquals
    &stop
  &end // of frame 0
  
  &importAssets &from '..\\common\\shell\\fe_shared_mainMenuLib.swf'
    'floorAnimationSymbol' &as 1
  &end // of importAssets
  
  &importAssets &from '..\\common\\shell\\fe_shared_mainMenuLib.swf'
    'cityscapeMC Symbol' &as 2
  &end // of importAssets
  
  &importAssets &from '../cafe/mouseComponents/std_MouseAccordionButton.swf'
    'std_AccordionMenuSymbol' &as 3
  &end // of importAssets

  &placeMovieClip 3 &as 'newLoadContinueMenuUI'
  
    &onClipEvent &construct
      &constants 'm_refFM', '_global.gFM', 'm_bEnabled', 'm_initiallySelected', 'm_tabIndex', 'm_bVisible', 'm_menuButtonSymbol', 'menuButtonContentClipSymbol', 'm_menuItemSymbol', 'menuItemContentClipSymbol', 'm_menuDefinitionArray', '$CampaignMenu_NewButton', '$CampaignMenu_LoadButton', '$CampaignMenu_ContinueButton', '$CampaignMenu_ChaptersButton', '$CampaignMenu_BackButton'  
      &pushsdb 0							//'m_refFM'
      &pushsdb 1							//'_global.gFM'
      &setVariable
      &pushsdb 2							//'m_bEnabled'
      &pushfalse
      &setVariable
      &pushsdb 3							//'m_initiallySelected'
      &pushfalse
      &setVariable
      &pushsdb 4							//'m_tabIndex'
      &pushzerosv
      &pushsdb 5							//'m_bVisible'
      &pushtrue
      &setVariable
      &pushsdb 6							//'m_menuButtonSymbol'
      &pushsdb 7							//'menuButtonContentClipSymbol'
      &setVariable
      &pushsdb 8							//'m_menuItemSymbol'
      &pushsdb 9							//'menuItemContentClipSymbol'
      &setVariable
      &pushsdb 10							//'m_menuDefinitionArray'
      &pushzero
      &initArray
      &setVariable
      &pushsdbgv 10							//'m_menuDefinitionArray'
      &pushzero
      &pushsdb 11							//'$CampaignMenu_NewButton'
      &setMember
      &pushsdbgv 10							//'m_menuDefinitionArray'
      &pushone
      &pushsdb 12							//'$CampaignMenu_LoadButton'
      &setMember
      &pushsdbgv 10							//'m_menuDefinitionArray'
      &pushbyte 2
      &pushsdb 13							//'$CampaignMenu_ContinueButton'
      &setMember
      &pushsdbgv 10							//'m_menuDefinitionArray'
      &pushbyte 3
      &pushsdb 14							//'$CampaignMenu_ChaptersButton'
      &setMember
      &pushsdbgv 10							//'m_menuDefinitionArray'
      &pushbyte 4
      &pushsdb 15							//'$CampaignMenu_BackButton'
      &setMember
    &end
  &end // of placeMovieClip 3

  &placeMovieClip 3 &as 'mainMenuUI'
  
    &onClipEvent &construct
      &constants 'm_refFM', '_global.gFM', 'm_bEnabled', 'm_initiallySelected', 'm_tabIndex', 'm_bVisible', 'm_menuButtonSymbol', 'menuButtonContentClipSymbol', 'm_menuItemSymbol', 'menuItemContentClipSymbol', 'm_menuDefinitionArray', '$Tutorial_MenuButton', '$Campaign_MenuButton,$APT:CAMPAIGNMENU_RECAPVIDEO,$CampaignMenu_NewButton,$CampaignMenu_LoadButton,$CampaignMenu_ContinueButton,$CampaignMenu_ChaptersButton', '$Scenario_MenuButton', '$Skirmish_MenuButton', '$Profiles_MenuButton,$Manage_MenuButton,$TransmissionLog_MenuButton,$ReplayTheater_MenuButton', '$Options_MenuButton,$Setting_MenuButton,$Credits_MenuButton', '$Exit_MenuButton'  
      &pushsdb 0							//'m_refFM'
      &pushsdb 1							//'_global.gFM'
      &setVariable
      &pushsdb 2							//'m_bEnabled'
      &pushfalse
      &setVariable
      &pushsdb 3							//'m_initiallySelected'
      &pushfalse
      &setVariable
      &pushsdb 4							//'m_tabIndex'
      &pushzerosv
      &pushsdb 5							//'m_bVisible'
      &pushtrue
      &setVariable
      &pushsdb 6							//'m_menuButtonSymbol'
      &pushsdb 7							//'menuButtonContentClipSymbol'
      &setVariable
      &pushsdb 8							//'m_menuItemSymbol'
      &pushsdb 9							//'menuItemContentClipSymbol'
      &setVariable
      &pushsdb 10							//'m_menuDefinitionArray'
      &pushzero
      &initArray
      &setVariable
      &pushsdbgv 10							//'m_menuDefinitionArray'
      &pushzero
      &pushsdb 11							//'$Tutorial_MenuButton'
      &setMember
      &pushsdbgv 10							//'m_menuDefinitionArray'
      &pushone
      &pushsdb 12							//'$Campaign_MenuButton,$APT:CAMPAIGNMENU_RECAPVIDEO,$CampaignMenu_NewButton,$CampaignMenu_LoadButton,$CampaignMenu_ContinueButton,$CampaignMenu_ChaptersButton'
      &setMember
      &pushsdbgv 10							//'m_menuDefinitionArray'
      &pushbyte 2
      &pushsdb 13							//'$Scenario_MenuButton'
      &setMember
      &pushsdbgv 10							//'m_menuDefinitionArray'
      &pushbyte 3
      &pushsdb 14							//'$Skirmish_MenuButton'
      &setMember
      &pushsdbgv 10							//'m_menuDefinitionArray'
      &pushbyte 4
      &pushsdb 15							//'$Profiles_MenuButton,$Manage_MenuButton,$TransmissionLog_MenuButton,$ReplayTheater_MenuButton'
      &setMember
      &pushsdbgv 10							//'m_menuDefinitionArray'
      &pushbyte 5
      &pushsdb 16							//'$Options_MenuButton,$Setting_MenuButton,$Credits_MenuButton'
      &setMember
      &pushsdbgv 10							//'m_menuDefinitionArray'
      &pushbyte 6
      &pushsdb 17							//'$Exit_MenuButton'
      &setMember
    &end
  &end // of placeMovieClip 3

  &defineMovieClip 5 // total frames: 1
  &end // of defineMovieClip 5

  &placeMovieClip 5 &as 'personalityImageMC'
  
    &onClipEvent &construct
      &pushs '_type'
      &pushssv 'RenderImage'
      &pushs '_mode'
      &pushssv 'ALPHA'
      &pushs '_imageMap'
      &pushssv 'MainMenuPersonalityImage'
    &end
  &end // of placeMovieClip 5
  
  &importAssets &from '..\\common\\shell\\fe_shared_mainMenuLib.swf'
    'hammer&SickleSymbol' &as 6
  &end // of importAssets

  &defineMovieClip 11 // total frames: 1
  &end // of defineMovieClip 11
  
  &importAssets &from '../cafe/mouseComponents/std_MouseButton.swf'
    'std_MousebuttonSymbol' &as 12
  &end // of importAssets

  &defineMovieClip 15 // total frames: 1

    &placeMovieClip 11 &as 'creditsMC'
    
      &onClipEvent &construct
        &pushs '_type'
        &pushssv 'CreditsImage'
      &end
    &end // of placeMovieClip 11

    &placeMovieClip 12 &as 'backButton'
    
      &onClipEvent &construct
        &pushs 'm_width'
        &pushshort 166
        &setVariable
        &pushs 'm_textAlign'
        &pushtrue
        &setVariable
        &pushs 'm_labelPosition'
        &pushssv 'right align'
        &pushs 'm_label_x'
        &pushzerosv
        &pushs 'm_label_y'
        &pushzerosv
        &pushs 'm_focusDirs'
        &pushssv 'Up/Down'
        &pushs 'm_iconType'
        &pushssv 'default'
        &pushs 'm_extLabel'
        &pushssv ''
        &pushs 'm_nTruncateType'
        &pushzerosv
        &pushs 'm_label'
        &pushssv '$Back'
        &pushs 'm_bEnabled'
        &pushtrue
        &setVariable
        &pushs 'm_refFM'
        &pushssv '_root.gFM'
        &pushs 'm_initiallySelected'
        &pushfalse
        &setVariable
        &pushs 'm_tabIndex'
        &pushzerosv
        &pushs 'm_bVisible'
        &pushtrue
        &setVariable
        &pushs 'm_contentSymbol'
        &pushssv 'buttonContentSymbol'
      &end
    &end // of placeMovieClip 12
  &end // of defineMovieClip 15
  
  &importAssets &from './screens/fe_m_assets.swf'
    'newProfileMC' &as 16
  &end // of importAssets

  &defineMovieClip 17 // total frames: 0
  &end // of defineMovieClip 17
  
  &exportAssets
    17 &as '__Packages.Cafe2_BaseUIScreen'
  &end // of exportAssets
  
  &initMovieClip 17
    &constants '_global', 'Cafe2_BaseUIScreen', 'm_screen', 'onEnterFrame', 'frameDelayedInitCallback', 'bind0', 'prototype', 'frameDelayedInit', '\n\nCafe2_BaseUIScreen::frameDelayedInitCallback() :', '\nscreens and move the call to initGUI() to frameDelayedInit().\n', 'N.B. You *must* override frameDelayedInit() in your', 'Array', 'outtroComplete', 'Cafe2_BaseUIScreen::outtroComplete() ', 'm_thisClass', 'm_uiComponentArray', 'length', 'splice', 'm_changeToScreen', 'gSM', 'changeMainScreen', 'introComplete', 'Cafe2_BaseUIScreen::introComplete()', 'restoreVisualState', 'm_introCount', 'onIntrosComplete', 'Cafe2_BaseUIScreen::onIntrosComplete()', 'localEventHandler', 'gEH', 'setLocalEventHandler', 'enableCurrentLocalEventHandler', 'registerIntroOuttroComponents', 'Cafe2_BaseUIScreen::registerIntroOuttroComponents()', 'setIntroCallback', 'executeBackFunction', 'Cafe2_BaseUIScreen::executeBackFunction', 'goBackScreen', 'startIntro', 'Cafe2_BaseUIScreen::startIntro()', 'intro', 'startOuttro', 'Cafe2_BaseUIScreen::startOuttro', 'outtro', 'startOuttroAndCallBack', 'm_executeOuttroCallBack', 'outtroAndCallBackComplete', 'backButtonClicked', 'Cafe2_BaseUIScreen::backButtonClicked()', 'Cafe2_BaseUIScreen::localEventHandler() keyCode == ', 'gFM', 'handleDefaultInputs', 'clearData', 'obj', 'func', 'call', 'ASSetPropFlags'  
    &pushglobalgv
    &pushsdbgm 1							//'Cafe2_BaseUIScreen'
    &not
    &not
    &jnz label30    
    &pushglobalgv
    &pushsdb 1							//'Cafe2_BaseUIScreen'
    &function2  (r:3='screen') (r:1='this', r:2='_global')
      &pushsdbgv 1							//'Cafe2_BaseUIScreen'
      &pushsdb 2							//'m_screen'
      &pushregister 3
      &setMember
      &pushsdbgv 1							//'Cafe2_BaseUIScreen'
      &pushsdbgm 2							//'m_screen'
      &pushsdb 3							//'onEnterFrame'
      &pushregister 1
      &pushsdbgm 4							//'frameDelayedInitCallback'
      &pushregister 1
      &pushbyte 2
      &pushregister 2
      &pushsdb 5							//'bind0'
      &callMethod
      &setMember
    &end // of function 

    &setRegister r:1
    &setMember
    &pushregister 1
    &pushsdbgm 6							//'prototype'
    &setRegister r:2
    &pop
    &pushregister 2
    &pushsdb 7							//'frameDelayedInit'
    &function2  () ()
      &pushsdb 8							//'\n\nCafe2_BaseUIScreen::frameDelayedInitCallback() :'
      &trace
      &pushsdb 9							//'\nscreens and move the call to initGUI() to frameDelayedInit().\n'
      &pushsdb 10							//'N.B. You *must* override frameDelayedInit() in your'
      &pushbyte 2
      &pushsdb 11							//'Array'
      &new
      &setRegister r:1
      &pop
      &pushregister 1
      &pushzero
      &getMember
      &pushregister 1
      &pushone
      &getMember
      &add
      &trace
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 4							//'frameDelayedInitCallback'
    &function2  () (r:1='this')
      &pushsdbgv 1							//'Cafe2_BaseUIScreen'
      &pushsdbgm 2							//'m_screen'
      &pushsdb 3							//'onEnterFrame'
      &pushnull
      &setMember
      &pushzero
      &pushregister 1
      &dcallmp 7							// frameDelayedInit()
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 12							//'outtroComplete'
    &function2  (r:3='theComponent') (r:1='_global')
      &pushsdb 13							//'Cafe2_BaseUIScreen::outtroComplete() '
      &pushregister 3
      &add
      &trace
      &pushzero
      &setRegister r:2
      &pop
     label1:
      &pushregister 2
      &pushsdbgv 1							//'Cafe2_BaseUIScreen'
      &pushsdbgm 14							//'m_thisClass'
      &pushsdbgm 15							//'m_uiComponentArray'
      &pushsdbgm 16							//'length'
      &lessThan
      &not
      &jnz label3      
      &pushregister 3
      &pushsdbgv 1							//'Cafe2_BaseUIScreen'
      &pushsdbgm 14							//'m_thisClass'
      &pushsdbgm 15							//'m_uiComponentArray'
      &pushregister 2
      &getMember
      &equals
      &not
      &jnz label2      
      &jmp label3      
     label2:
      &pushregister 2
      &increment
      &setRegister r:2
      &pop
      &jmp label1      
     label3:
      &pushone
      &pushregister 2
      &pushbyte 2
      &pushsdbgv 1							//'Cafe2_BaseUIScreen'
      &pushsdbgm 14							//'m_thisClass'
      &pushsdbgm 15							//'m_uiComponentArray'
      &dcallmp 17							// splice()
      &pushsdbgv 1							//'Cafe2_BaseUIScreen'
      &pushsdbgm 14							//'m_thisClass'
      &pushsdbgm 15							//'m_uiComponentArray'
      &pushsdbgm 16							//'length'
      &pushzero
      &greaterThan
      &not
      &not
      &jnz label4      
      &pushsdbgv 1							//'Cafe2_BaseUIScreen'
      &pushsdbgm 14							//'m_thisClass'
      &pushsdbgm 18							//'m_changeToScreen'
      &pushnull
      &equals
      &not
      &not
      &jnz label4      
      &pushsdbgv 1							//'Cafe2_BaseUIScreen'
      &pushsdbgm 14							//'m_thisClass'
      &pushsdbgm 18							//'m_changeToScreen'
      &pushone
      &pushregister 1
      &pushsdbgm 19							//'gSM'
      &dcallmp 20							// changeMainScreen()
     label4:
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 21							//'introComplete'
    &function2  (r:1='theComponent') ()
      &pushsdb 22							//'Cafe2_BaseUIScreen::introComplete()'
      &trace
      &pushzero
      &pushregister 1
      &dcallmp 23							// restoreVisualState()
      &pushsdbgv 1							//'Cafe2_BaseUIScreen'
      &pushsdbgm 14							//'m_thisClass'
      &pushsdb 24							//'m_introCount'
      &pushsdbgv 1							//'Cafe2_BaseUIScreen'
      &pushsdbgm 14							//'m_thisClass'
      &pushsdbgm 24							//'m_introCount'
      &decrement
      &setMember
      &pushsdbgv 1							//'Cafe2_BaseUIScreen'
      &pushsdbgm 14							//'m_thisClass'
      &pushsdbgm 24							//'m_introCount'
      &pushzero
      &greaterThan
      &not
      &not
      &jnz label5      
      &pushzero
      &pushsdbgv 1							//'Cafe2_BaseUIScreen'
      &pushsdbgm 14							//'m_thisClass'
      &dcallmp 25							// onIntrosComplete()
     label5:
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 25							//'onIntrosComplete'
    &function2  () (r:1='this', r:2='_global')
      &pushsdb 26							//'Cafe2_BaseUIScreen::onIntrosComplete()'
      &trace
      &pushregister 1
      &pushsdbgm 27							//'localEventHandler'
      &pushone
      &pushregister 2
      &pushsdbgm 28							//'gEH'
      &dcallmp 29							// setLocalEventHandler()
      &pushzero
      &pushregister 2
      &pushsdbgm 28							//'gEH'
      &dcallmp 30							// enableCurrentLocalEventHandler()
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 31							//'registerIntroOuttroComponents'
    &function2  (r:3='components') (r:1='this')
      &pushsdb 32							//'Cafe2_BaseUIScreen::registerIntroOuttroComponents()'
      &trace
      &pushregister 1
      &pushsdb 15							//'m_uiComponentArray'
      &pushregister 3
      &setMember
      &pushregister 1
      &pushsdb 24							//'m_introCount'
      &pushregister 1
      &pushsdbgm 15							//'m_uiComponentArray'
      &pushsdbgm 16							//'length'
      &setMember
      &pushzero
      &setRegister r:2
      &pop
     label6:
      &pushregister 2
      &pushregister 1
      &pushsdbgm 24							//'m_introCount'
      &lessThan
      &not
      &jnz label7      
      &pushregister 1
      &pushsdbgm 21							//'introComplete'
      &pushone
      &pushregister 1
      &pushsdbgm 15							//'m_uiComponentArray'
      &pushregister 2
      &getMember
      &dcallmp 33							// setIntroCallback()
      &pushregister 2
      &increment
      &setRegister r:2
      &pop
      &jmp label6      
     label7:
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 34							//'executeBackFunction'
    &function2  (r:3='theComponent') (r:1='_global')
      &pushsdb 35							//'Cafe2_BaseUIScreen::executeBackFunction'
      &pushregister 3
      &add
      &trace
      &pushzero
      &setRegister r:2
      &pop
     label8:
      &pushregister 2
      &pushsdbgv 1							//'Cafe2_BaseUIScreen'
      &pushsdbgm 14							//'m_thisClass'
      &pushsdbgm 15							//'m_uiComponentArray'
      &pushsdbgm 16							//'length'
      &lessThan
      &not
      &jnz label10      
      &pushregister 3
      &pushsdbgv 1							//'Cafe2_BaseUIScreen'
      &pushsdbgm 14							//'m_thisClass'
      &pushsdbgm 15							//'m_uiComponentArray'
      &pushregister 2
      &getMember
      &equals
      &not
      &jnz label9      
      &jmp label10      
     label9:
      &pushregister 2
      &increment
      &setRegister r:2
      &pop
      &jmp label8      
     label10:
      &pushone
      &pushregister 2
      &pushbyte 2
      &pushsdbgv 1							//'Cafe2_BaseUIScreen'
      &pushsdbgm 14							//'m_thisClass'
      &pushsdbgm 15							//'m_uiComponentArray'
      &dcallmp 17							// splice()
      &pushsdbgv 1							//'Cafe2_BaseUIScreen'
      &pushsdbgm 14							//'m_thisClass'
      &pushsdbgm 15							//'m_uiComponentArray'
      &pushsdbgm 16							//'length'
      &pushzero
      &greaterThan
      &not
      &not
      &jnz label11      
      &pushzero
      &pushregister 1
      &pushsdbgm 19							//'gSM'
      &dcallmp 36							// goBackScreen()
     label11:
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 37							//'startIntro'
    &function2  () (r:1='this')
      &pushsdb 38							//'Cafe2_BaseUIScreen::startIntro()'
      &trace
      &pushregister 1
      &pushsdbgm 15							//'m_uiComponentArray'
      &pushsdbgm 16							//'length'
      &setRegister r:3
      &pop
      &pushzero
      &setRegister r:2
      &pop
     label12:
      &pushregister 2
      &pushregister 3
      &lessThan
      &not
      &jnz label13      
      &pushzero
      &pushregister 1
      &pushsdbgm 15							//'m_uiComponentArray'
      &pushregister 2
      &getMember
      &dcallmp 39							// intro()
      &pushregister 2
      &increment
      &setRegister r:2
      &pop
      &jmp label12      
     label13:
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 40							//'startOuttro'
    &function2  (r:4='newScreen') (r:1='this')
      &pushsdb 41							//'Cafe2_BaseUIScreen::startOuttro'
      &trace
      &pushregister 1
      &pushsdb 18							//'m_changeToScreen'
      &pushregister 4
      &setMember
      &pushsdbgv 1							//'Cafe2_BaseUIScreen'
      &pushsdbgm 14							//'m_thisClass'
      &pushsdbgm 15							//'m_uiComponentArray'
      &pushsdbgm 16							//'length'
      &setRegister r:3
      &pop
      &pushregister 3
      &pushzero
      &greaterThan
      &not
      &jnz label16      
      &pushzero
      &setRegister r:2
      &pop
     label14:
      &pushregister 2
      &pushregister 3
      &lessThan
      &not
      &jnz label15      
      &pushregister 1
      &pushsdbgm 12							//'outtroComplete'
      &pushone
      &pushsdbgv 1							//'Cafe2_BaseUIScreen'
      &pushsdbgm 14							//'m_thisClass'
      &pushsdbgm 15							//'m_uiComponentArray'
      &pushregister 2
      &getMember
      &dcallmp 42							// outtro()
      &pushregister 2
      &increment
      &setRegister r:2
      &pop
      &jmp label14      
     label15:
      &jmp label17      
     label16:
      &pushnull
      &pushone
      &pushregister 1
      &dcallmp 12							// outtroComplete()
     label17:
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 43							//'startOuttroAndCallBack'
    &function2  (r:4='doAfterOuttroComplete') (r:1='this')
      &pushsdb 41							//'Cafe2_BaseUIScreen::startOuttro'
      &trace
      &pushregister 1
      &pushsdb 44							//'m_executeOuttroCallBack'
      &pushregister 4
      &setMember
      &pushsdbgv 1							//'Cafe2_BaseUIScreen'
      &pushsdbgm 14							//'m_thisClass'
      &pushsdbgm 15							//'m_uiComponentArray'
      &pushsdbgm 16							//'length'
      &setRegister r:3
      &pop
      &pushregister 3
      &pushzero
      &greaterThan
      &not
      &jnz label20      
      &pushzero
      &setRegister r:2
      &pop
     label18:
      &pushregister 2
      &pushregister 3
      &lessThan
      &not
      &jnz label19      
      &pushregister 1
      &pushsdbgm 45							//'outtroAndCallBackComplete'
      &pushone
      &pushsdbgv 1							//'Cafe2_BaseUIScreen'
      &pushsdbgm 14							//'m_thisClass'
      &pushsdbgm 15							//'m_uiComponentArray'
      &pushregister 2
      &getMember
      &dcallmp 42							// outtro()
      &pushregister 2
      &increment
      &setRegister r:2
      &pop
      &jmp label18      
     label19:
      &jmp label21      
     label20:
      &pushnull
      &pushone
      &pushregister 1
      &dcallmp 45							// outtroAndCallBackComplete()
     label21:
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 45							//'outtroAndCallBackComplete'
    &function2  (r:2='theComponent') ()
      &pushsdb 13							//'Cafe2_BaseUIScreen::outtroComplete() '
      &pushregister 2
      &add
      &trace
      &pushzero
      &setRegister r:1
      &pop
     label22:
      &pushregister 1
      &pushsdbgv 1							//'Cafe2_BaseUIScreen'
      &pushsdbgm 14							//'m_thisClass'
      &pushsdbgm 15							//'m_uiComponentArray'
      &pushsdbgm 16							//'length'
      &lessThan
      &not
      &jnz label24      
      &pushregister 2
      &pushsdbgv 1							//'Cafe2_BaseUIScreen'
      &pushsdbgm 14							//'m_thisClass'
      &pushsdbgm 15							//'m_uiComponentArray'
      &pushregister 1
      &getMember
      &equals
      &not
      &jnz label23      
      &jmp label24      
     label23:
      &pushregister 1
      &increment
      &setRegister r:1
      &pop
      &jmp label22      
     label24:
      &pushone
      &pushregister 1
      &pushbyte 2
      &pushsdbgv 1							//'Cafe2_BaseUIScreen'
      &pushsdbgm 14							//'m_thisClass'
      &pushsdbgm 15							//'m_uiComponentArray'
      &dcallmp 17							// splice()
      &pushsdbgv 1							//'Cafe2_BaseUIScreen'
      &pushsdbgm 14							//'m_thisClass'
      &pushsdbgm 15							//'m_uiComponentArray'
      &pushsdbgm 16							//'length'
      &pushzero
      &greaterThan
      &not
      &not
      &jnz label25      
      &pushzero
      &pushsdbgv 1							//'Cafe2_BaseUIScreen'
      &pushsdbgm 14							//'m_thisClass'
      &dcallmp 44							// m_executeOuttroCallBack()
     label25:
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 46							//'backButtonClicked'
    &function2  () (r:1='_global')
      &pushsdb 47							//'Cafe2_BaseUIScreen::backButtonClicked()'
      &trace
      &pushsdbgv 1							//'Cafe2_BaseUIScreen'
      &pushsdbgm 14							//'m_thisClass'
      &pushsdbgm 15							//'m_uiComponentArray'
      &pushsdbgm 16							//'length'
      &setRegister r:3
      &pop
      &pushregister 3
      &pushzero
      &greaterThan
      &not
      &jnz label28      
      &pushzero
      &setRegister r:2
      &pop
     label26:
      &pushregister 2
      &pushregister 3
      &lessThan
      &not
      &jnz label27      
      &pushsdbgv 1							//'Cafe2_BaseUIScreen'
      &pushsdbgm 14							//'m_thisClass'
      &pushsdbgm 34							//'executeBackFunction'
      &pushone
      &pushsdbgv 1							//'Cafe2_BaseUIScreen'
      &pushsdbgm 14							//'m_thisClass'
      &pushsdbgm 15							//'m_uiComponentArray'
      &pushregister 2
      &getMember
      &dcallmp 42							// outtro()
      &pushregister 2
      &increment
      &setRegister r:2
      &pop
      &jmp label26      
     label27:
      &jmp label29      
     label28:
      &pushzero
      &pushregister 1
      &pushsdbgm 19							//'gSM'
      &dcallmp 36							// goBackScreen()
     label29:
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 27							//'localEventHandler'
    &function2  (r:2='keyCode', 'controller') (r:1='_global')
      &pushsdb 48							//'Cafe2_BaseUIScreen::localEventHandler() keyCode == '
      &pushregister 2
      &add
      &trace
      &pushregister 2
      &pushone
      &pushregister 1
      &pushsdbgm 49							//'gFM'
      &dcallmp 50							// handleDefaultInputs()
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 51							//'clearData'
    &function2  () (r:1='this')
      &pushsdbgv 1							//'Cafe2_BaseUIScreen'
      &pushsdb 2							//'m_screen'
      &pushnull
      &setMember
      &pushsdbgv 1							//'Cafe2_BaseUIScreen'
      &pushsdb 14							//'m_thisClass'
      &pushnull
      &setMember
      &pushregister 1
      &pushsdb 18							//'m_changeToScreen'
      &pushnull
      &setMember
      &pushregister 1
      &pushsdb 15							//'m_uiComponentArray'
      &pushnull
      &setMember
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 5							//'bind0'
    &function  ('obj', 'func'    )    
      &function  (      )      
        &pushsdbgv 52							//'obj'
        &pushone
        &pushsdbgv 53							//'func'
        &dcallmp 54							// call()
      &end // of function 

      &return
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 18							//'m_changeToScreen'
    &pushnull
    &setMember
    &pushregister 2
    &pushsdb 44							//'m_executeOuttroCallBack'
    &pushnull
    &setMember
    &pushregister 2
    &pushsdb 15							//'m_uiComponentArray'
    &pushzero
    &pushsdb 11							//'Array'
    &new
    &setMember
    &pushone
    &pushnull
    &pushglobalgv
    &pushsdbgm 1							//'Cafe2_BaseUIScreen'
    &pushsdbgm 6							//'prototype'
    &pushbyte 3
    &pushsdb 55							//'ASSetPropFlags'
    &callFunction
   label30:
    &pop
  &end // of initMovieClip 17

  &defineMovieClip 18 // total frames: 0
  &end // of defineMovieClip 18
  
  &exportAssets
    18 &as '__Packages.feg_m_mainMenu3D'
  &end // of exportAssets
  
  &initMovieClip 18
    &constants '_global', 'feg_m_mainMenu3D', 'Cafe2_BaseUIScreen', 'm_thisClass', 'GAME', 'SAVELOADSCREEN_TYPE', 'ENUM', 'SAVELOADSCREEN_LOAD', 'MODE', 'GAME_MODE_SHELL', 'COOPMODE', 'GAME_COOPMODE_AI', 'm_screen', 'newProfileMC', '_visible', 'creditMC', 'onScreenExit', 'bind0', 'gSM', 'setOnExitScreen', 'prototype', 'frameDelayedInit', 'Object', 'QueryGameEngine?MENU_MUSIC_TRACK', 'SOUND', 'SHELL_MAIN', 'MENU_MUSIC_TRACK', 'PlayShellMusic', 'initGUI', 'startIntro', 'loadFloorMovie', 'm_floorMC', 'floorMC', '_xrotation', 'FLOOR_XROT', '_yrotation', 'FLOOR_YROT', '_x', 'FLOOR_X', '_y', 'FLOOR_Y', 'initFloorParameters', 'm_floorUI', 'floorUI', 'FLOOR_SCALE', 'UI_Y_ORIGIN', 'UI_POS_PADDING', 'UI_X_ORIGIN', 'FLOOR_UI_NAME', 'init', 'loadWallMovie', 'm_wallMC', 'wallMC', 'WALL_XROT', 'WALL_YROT', 'WALL_X', 'WALL_Y', '_z', 'WALL_Z', 'bombFactoryMC', 'disable', 'BackBombFactoryMC', 'FarBackBombFactoryMC', 'initWallParameters', 'm_wallUI', 'wallUI', 'WALL_SCALE', 'WALL_UI_NAME', 'loadMainMenu', 'm_mainMenuMC', 'mainMenuUI', 'processUserInputMainMenu', 'bind2DynamicParams', 'setOnClickCallBack', 'extern', 'IsDevelopment', '1', 'Map List', 'MENU_ITEM_2', 'appendToMenuElement', 'show', 'MENU_XROT', 'MENU_YROT', 'MENU_X', 'MENU_Y', 'MENU_Z', 'loadNewLoadContinueMenu', 'm_NLCMenuMC', 'newLoadContinueMenuUI', 'processUserInputNLC', 'NLC_XROT', 'NLC_YROT', 'NLC_X', 'NLC_Y', 'NLC_Z', 'MENU_ITEM_1', 'MENU_ITEM_3', 'MENU_ITEM_4', 'MENU_ITEM_5', 'MENU_ITEM_6', 'MENU_ITEM_7', 'processTutorialMenuClick', 'processCampaignMenuClick', 'processScenarioMenuClick', 'processSkirmishMenuClick', 'processProfileMenuClick', 'processOptionsMenuClick', 'processQuitMenuClick', 'SCREEN', 'FEG_TUTORIALMENU', 'startOuttro', 'FES_SKIRMISH_SETUP', 'FES_SCENARIO_MAP', 'processRecapMenuClick', 'processNewMenuClick', 'processLoadMenuClick', 'processContinueMenuClick', 'processChaptersMenuClick', 'feg_m_mainMenu3D::processCampaignMenuClick() Level List / Map List', 'FES_SINGLE_PLAYER_MENU', 'feg_m_mainMenu3D::processProfileMenuClick() :: Management', 'FEG_PROFILE_MANAGER', 'feg_m_mainMenu3D::processProfileMenuClick() :: Film Archive', 'FEG_FILMARCHIVE', 'feg_m_mainMenu3D::processProfileMenuClick() :: Replay Theater', 'FEG_SAVELOAD', 'feg_m_mainMenu3D::processOptionsMenuClick::Options : Settings', 'FEG_OPTIONS', 'feg_m_mainMenu3D::processOptionsMenuClick::Options : Credits', 'goToCredits', 'processBackMenuClick', 'GAME_MODE_CAMPAIGN', 'FES_FACTION_SELECT_MENU', 'FES_CHAPTER_SELECT', 'transitionToMainMenu', 'showCredits', 'hideMainMenu', 'startCredits', 'goToMainMenu', 'hideCredits', 'showMainMenu', 'stopCredits', 'initMenuParameters', 'm_menuUI', 'menuUI', 'MENU_SCALE', 'MENU_UI_NAME', 'loadpersonalityImageMovie', 'm_personalityImageMC', 'personalityImageMC', 'PERSONALITY_IMAGE_XROT', 'PERSONALITY_IMAGE_YROT', 'PERSONALITY_IMAGE_X', 'PERSONALITY_IMAGE_Y', 'PERSONALITY_IMAGE_Z', 'initPersonalityImageParameters', 'm_personalityImageUI', 'personalityImageUI', 'PERSONALITY_IMAGE_SCALE', 'PERSONALITY_UI_NAME', 'loadCreditsMovie', 'm_creditMC', 'CREDIT_XROT', 'CREDIT_YROT', 'CREDIT_X', 'CREDIT_Y', 'backButton', 'enable', 'setOnMouseUpFunction', 'push', 'loadHammerNSickleMovie', 'm_HammerSickleMC', 'hammerMC', 'HAMMER_XROT', 'HAMMER_YROT', 'HAMMER_X', 'HAMMER_Y', 'HAMMER_Z', 'initHammerParameters', 'm_hammerSickleUI', 'hammerUI', 'HAMMER_SCALE', 'HAMMER_UI_NAME', 'loadTestControls', 'm_testUIMC', 'UI_ADJUSTMENT_LAYER', 'testControls', 'userInterfaceSymbol', 'attachMovie', 'loadGuide', 'm_guideMC', 'UI_IMAGEGUIDE_LAYER', 'imageGuide', 'image_guideSymbol', '_alpha', 'm_localMessageHandlerFunc', 'localMessageHandler', 'bind1DynamicParams', 'gMH', 'addMessageHandler', 'Array', 'btnNewConfirm', 'showHideTestUI', 'showHideGuideMC', 'initNewProfilePrompt', 'QueryGameEngine?FE_PROFILE_COUNT', 'FE_PROFILE_COUNT', '0', 'newProfileName', 'takeFocus', 'showScenarioAward', 'showCampaignAward', 'showTutorialAward', 'registerIntroOuttroComponents', 'onCreateProfile', 'setOnEnterFunction', 'btnNewCancel', 'getContents', 'FSCommand:CallGameFunction', '%AddProfile?ProfileName=', '', 'setContents', 'm_isCreditsListScrolling', 'CREDITS', 'removeMessageHandler', 'clearData', 'MSGCODE', 'FE_PROFILE_CREATE_SUCCESS', 'toggleTestUI', 'm_testUIVisible', 'showTestUI', 'hideTestUI', 'm_speedUI', 'toggleGuide', 'initData', 'xPosSliderUI', 'setValue', 'yPosSliderUI', 'zPosSliderUI', 'rotXSliderUI', 'rotYSliderUI', 'update', 'transitionToNLC', 'm_frameCount', 'onEnterFrame', 'updateNLCAnimation', 'gotoAndPlay', 'NLC_ANIM_LENGTH', 'lerp', 'SLIDE_DISTANCE_X', 'SLIDE_DISTANCE_Z', 'stop', 'updateMainMenuAnimation', 'QueryGameEngine?SCENARIO_COMPLETION_STATE', 'SCENARIO_COMPLETION_STATE_ALL_UNDER_PAR', 'SCENARIO_COMPLETION_STATE_ALL_UNDER_PAR_FIRSTTIME', 'GuiAccordionButtonStar3', 'playSound', 'menuItemShowAward', 'SCENARIO_COMPLETION_STATE_ALL_COMPLETE', 'SCENARIO_COMPLETION_STATE_ALL_COMPLETE_FIRSTTIME', 'GuiAccordionButtonStar2', 'SCENARIO_COMPLETION_STATE_CRITICAL_COMPLETE', 'SCENARIO_COMPLETION_STATE_CRITICAL_COMPLETE_FIRSTTIME', 'GuiAccordionButtonStar1', 'QueryGameEngine?IS_CAMPAIGN_COMPLETE?FactionIndex=', 'CAMPAIGN_TYPE_SOVIET', 'IS_CAMPAIGN_COMPLETE', 'CAMPAIGN_TYPE_ALLIES', 'CAMPAIGN_TYPE_RISINGSUN', 'CAMPAIGN_TYPE_YURIKO', 'QueryGameEngine?JUST_COMPLETED_CAMPAIGN', 'JUST_COMPLETED_CAMPAIGN', '-1', 'QueryGameEngine?TUTORIAL_IS_COMPLETED', ',', 'TUTORIAL_IS_COMPLETED', 'split', 'length', 'FLOOR_Z', 'Wall Adjustment UI', 'Floor Adjustment UI', 'Hammer Adjustment UI', 'Menu Adjustment UI', 'Personality Adjustment UI', 'ASSetPropFlags'  
    &pushglobalgv
    &pushsdbgm 1							//'feg_m_mainMenu3D'
    &not
    &not
    &jnz label77    
    &pushglobalgv
    &pushsdb 1							//'feg_m_mainMenu3D'
    &function2  (r:4='screen') (r:1='this', r:2='super', r:3='_global')
      &pushregister 4
      &pushone
      &pushregister 2
      &pushundef
      &callmp
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdb 3							//'m_thisClass'
      &pushregister 1
      &setMember
      &pushregister 3
      &pushsdbgm 4							//'GAME'
      &pushsdb 5							//'SAVELOADSCREEN_TYPE'
      &pushregister 3
      &pushsdbgm 6							//'ENUM'
      &pushsdbgm 7							//'SAVELOADSCREEN_LOAD'
      &setMember
      &pushregister 3
      &pushsdbgm 4							//'GAME'
      &pushsdb 8							//'MODE'
      &pushregister 3
      &pushsdbgm 6							//'ENUM'
      &pushsdbgm 9							//'GAME_MODE_SHELL'
      &setMember
      &pushregister 3
      &pushsdbgm 4							//'GAME'
      &pushsdb 10							//'COOPMODE'
      &pushregister 3
      &pushsdbgm 6							//'ENUM'
      &pushsdbgm 11							//'GAME_COOPMODE_AI'
      &setMember
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 12							//'m_screen'
      &pushsdbgm 13							//'newProfileMC'
      &pushsdb 14							//'_visible'
      &pushfalse
      &setMember
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 12							//'m_screen'
      &pushsdbgm 15							//'creditMC'
      &pushsdb 14							//'_visible'
      &pushfalse
      &setMember
      &pushregister 1
      &pushsdbgm 16							//'onScreenExit'
      &pushregister 1
      &pushbyte 2
      &pushregister 3
      &pushsdb 17							//'bind0'
      &callMethod
      &pushone
      &pushregister 3
      &pushsdbgm 18							//'gSM'
      &dcallmp 19							// setOnExitScreen()
    &end // of function 

    &setRegister r:1
    &setMember
    &pushglobalgv
    &pushsdbgm 1							//'feg_m_mainMenu3D'
    &pushsdbgv 2							//'Cafe2_BaseUIScreen'
    &extends
    &pushregister 1
    &pushsdbgm 20							//'prototype'
    &setRegister r:2
    &pop
    &pushregister 2
    &pushsdb 21							//'frameDelayedInit'
    &function2  () (r:1='this', r:2='_global')
      &getURL 'FSCommand:CallGameFunction'       '%RegisterMainMenuPersonalityImage'      
      &pushzero
      &pushsdb 22							//'Object'
      &new
      &setRegister r:3
      &pop
      &pushsdb 23							//'QueryGameEngine?MENU_MUSIC_TRACK'
      &pushregister 3
      &loadVariables
      &pushregister 2
      &pushsdbgm 24							//'SOUND'
      &pushsdb 25							//'SHELL_MAIN'
      &pushregister 3
      &pushsdbgm 26							//'MENU_MUSIC_TRACK'
      &setMember
      &pushregister 2
      &pushsdbgm 24							//'SOUND'
      &pushsdbgm 25							//'SHELL_MAIN'
      &pushone
      &pushregister 2
      &dcallmp 27							// PlayShellMusic()
      &pushzero
      &pushregister 1
      &dcallmp 28							// initGUI()
      &pushzero
      &pushregister 1
      &dcallmp 29							// startIntro()
      &getURL 'FSCommand:CallGameFunction'       '%StartRevertResolutionTimer'      
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 30							//'loadFloorMovie'
    &function2  () (r:1='this')
      &pushregister 1
      &pushsdb 31							//'m_floorMC'
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 12							//'m_screen'
      &pushsdbgm 32							//'floorMC'
      &setMember
      &pushregister 1
      &pushsdbgm 31							//'m_floorMC'
      &pushsdb 14							//'_visible'
      &pushtrue
      &setMember
      &pushregister 1
      &pushsdbgm 31							//'m_floorMC'
      &pushsdb 14							//'_visible'
      &pushtrue
      &setMember
      &pushregister 1
      &pushsdbgm 31							//'m_floorMC'
      &pushsdb 33							//'_xrotation'
      &pushsdbgv 1							//'feg_m_mainMenu3D'
      &pushsdbgm 34							//'FLOOR_XROT'
      &setMember
      &pushregister 1
      &pushsdbgm 31							//'m_floorMC'
      &pushsdb 35							//'_yrotation'
      &pushsdbgv 1							//'feg_m_mainMenu3D'
      &pushsdbgm 36							//'FLOOR_YROT'
      &setMember
      &pushregister 1
      &pushsdbgm 31							//'m_floorMC'
      &pushsdb 37							//'_x'
      &pushsdbgv 1							//'feg_m_mainMenu3D'
      &pushsdbgm 38							//'FLOOR_X'
      &setMember
      &pushregister 1
      &pushsdbgm 31							//'m_floorMC'
      &pushsdb 39							//'_y'
      &pushsdbgv 1							//'feg_m_mainMenu3D'
      &pushsdbgm 40							//'FLOOR_Y'
      &setMember
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 41							//'initFloorParameters'
    &function2  () (r:1='this')
      &pushregister 1
      &pushsdb 42							//'m_floorUI'
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 12							//'m_screen'
      &pushsdbgm 43							//'floorUI'
      &setMember
      &pushsdbgv 44							//'FLOOR_SCALE'
      &pushsdbgv 1							//'feg_m_mainMenu3D'
      &pushsdbgm 45							//'UI_Y_ORIGIN'
      &pushshort 231
      &add
      &pushsdbgv 1							//'feg_m_mainMenu3D'
      &pushsdbgm 46							//'UI_POS_PADDING'
      &add
      &pushsdbgv 1							//'feg_m_mainMenu3D'
      &pushsdbgm 47							//'UI_X_ORIGIN'
      &pushregister 1
      &pushsdbgm 31							//'m_floorMC'
      &pushone
      &initArray
      &pushsdbgv 1							//'feg_m_mainMenu3D'
      &pushsdbgm 48							//'FLOOR_UI_NAME'
      &pushbyte 5
      &pushregister 1
      &pushsdbgm 42							//'m_floorUI'
      &dcallmp 49							// init()
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 50							//'loadWallMovie'
    &function2  () (r:1='this')
      &pushregister 1
      &pushsdb 51							//'m_wallMC'
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 12							//'m_screen'
      &pushsdbgm 52							//'wallMC'
      &setMember
      &pushregister 1
      &pushsdbgm 51							//'m_wallMC'
      &pushsdb 14							//'_visible'
      &pushtrue
      &setMember
      &pushregister 1
      &pushsdbgm 51							//'m_wallMC'
      &pushsdb 33							//'_xrotation'
      &pushsdbgv 1							//'feg_m_mainMenu3D'
      &pushsdbgm 53							//'WALL_XROT'
      &setMember
      &pushregister 1
      &pushsdbgm 51							//'m_wallMC'
      &pushsdb 35							//'_yrotation'
      &pushsdbgv 1							//'feg_m_mainMenu3D'
      &pushsdbgm 54							//'WALL_YROT'
      &setMember
      &pushregister 1
      &pushsdbgm 51							//'m_wallMC'
      &pushsdb 37							//'_x'
      &pushsdbgv 1							//'feg_m_mainMenu3D'
      &pushsdbgm 55							//'WALL_X'
      &setMember
      &pushregister 1
      &pushsdbgm 51							//'m_wallMC'
      &pushsdb 39							//'_y'
      &pushsdbgv 1							//'feg_m_mainMenu3D'
      &pushsdbgm 56							//'WALL_Y'
      &setMember
      &pushregister 1
      &pushsdbgm 51							//'m_wallMC'
      &pushsdb 57							//'_z'
      &pushsdbgv 1							//'feg_m_mainMenu3D'
      &pushsdbgm 58							//'WALL_Z'
      &setMember
      &pushzero
      &pushregister 1
      &pushsdbgm 51							//'m_wallMC'
      &pushsdbgm 59							//'bombFactoryMC'
      &dcallmp 60							// disable()
      &pushzero
      &pushregister 1
      &pushsdbgm 51							//'m_wallMC'
      &pushsdbgm 61							//'BackBombFactoryMC'
      &dcallmp 60							// disable()
      &pushzero
      &pushregister 1
      &pushsdbgm 51							//'m_wallMC'
      &pushsdbgm 62							//'FarBackBombFactoryMC'
      &dcallmp 60							// disable()
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 63							//'initWallParameters'
    &function2  () (r:1='this')
      &pushregister 1
      &pushsdb 64							//'m_wallUI'
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 12							//'m_screen'
      &pushsdbgm 65							//'wallUI'
      &setMember
      &pushsdbgv 66							//'WALL_SCALE'
      &pushsdbgv 1							//'feg_m_mainMenu3D'
      &pushsdbgm 45							//'UI_Y_ORIGIN'
      &pushsdbgv 1							//'feg_m_mainMenu3D'
      &pushsdbgm 47							//'UI_X_ORIGIN'
      &pushregister 1
      &pushsdbgm 51							//'m_wallMC'
      &pushone
      &initArray
      &pushsdbgv 1							//'feg_m_mainMenu3D'
      &pushsdbgm 67							//'WALL_UI_NAME'
      &pushbyte 5
      &pushregister 1
      &pushsdbgm 64							//'m_wallUI'
      &dcallmp 49							// init()
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 68							//'loadMainMenu'
    &function2  () (r:1='this', r:2='_global')
      &pushregister 1
      &pushsdb 69							//'m_mainMenuMC'
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 12							//'m_screen'
      &pushsdbgm 70							//'mainMenuUI'
      &setMember
      &pushregister 1
      &pushsdbgm 71							//'processUserInputMainMenu'
      &pushregister 1
      &pushbyte 2
      &pushregister 2
      &pushsdb 72							//'bind2DynamicParams'
      &callMethod
      &pushone
      &pushregister 1
      &pushsdbgm 69							//'m_mainMenuMC'
      &dcallmp 73							// setOnClickCallBack()
      &pushsdbgv 74							//'extern'
      &pushsdbgm 75							//'IsDevelopment'
      &pushsdb 76							//'1'
      &equals
      &not
      &jnz label1      
      &pushsdb 77							//'Map List'
      &pushsdbgv 1							//'feg_m_mainMenu3D'
      &pushsdbgm 78							//'MENU_ITEM_2'
      &pushbyte 2
      &pushregister 1
      &pushsdbgm 69							//'m_mainMenuMC'
      &dcallmp 79							// appendToMenuElement()
     label1:
      &pushzero
      &pushregister 1
      &pushsdbgm 69							//'m_mainMenuMC'
      &dcallmp 49							// init()
      &pushzero
      &pushregister 1
      &pushsdbgm 69							//'m_mainMenuMC'
      &dcallmp 80							// show()
      &pushregister 1
      &pushsdbgm 69							//'m_mainMenuMC'
      &pushsdb 33							//'_xrotation'
      &pushsdbgv 1							//'feg_m_mainMenu3D'
      &pushsdbgm 81							//'MENU_XROT'
      &setMember
      &pushregister 1
      &pushsdbgm 69							//'m_mainMenuMC'
      &pushsdb 35							//'_yrotation'
      &pushsdbgv 1							//'feg_m_mainMenu3D'
      &pushsdbgm 82							//'MENU_YROT'
      &setMember
      &pushregister 1
      &pushsdbgm 69							//'m_mainMenuMC'
      &pushsdb 37							//'_x'
      &pushsdbgv 1							//'feg_m_mainMenu3D'
      &pushsdbgm 83							//'MENU_X'
      &setMember
      &pushregister 1
      &pushsdbgm 69							//'m_mainMenuMC'
      &pushsdb 39							//'_y'
      &pushsdbgv 1							//'feg_m_mainMenu3D'
      &pushsdbgm 84							//'MENU_Y'
      &setMember
      &pushregister 1
      &pushsdbgm 69							//'m_mainMenuMC'
      &pushsdb 57							//'_z'
      &pushsdbgv 1							//'feg_m_mainMenu3D'
      &pushsdbgm 85							//'MENU_Z'
      &setMember
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 86							//'loadNewLoadContinueMenu'
    &function2  () (r:1='this', r:2='_global')
      &pushregister 1
      &pushsdb 87							//'m_NLCMenuMC'
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 12							//'m_screen'
      &pushsdbgm 88							//'newLoadContinueMenuUI'
      &setMember
      &pushzero
      &pushregister 1
      &pushsdbgm 87							//'m_NLCMenuMC'
      &dcallmp 49							// init()
      &pushregister 1
      &pushsdbgm 87							//'m_NLCMenuMC'
      &pushsdb 14							//'_visible'
      &pushtrue
      &setMember
      &pushregister 1
      &pushsdbgm 89							//'processUserInputNLC'
      &pushregister 1
      &pushbyte 2
      &pushregister 2
      &pushsdb 72							//'bind2DynamicParams'
      &callMethod
      &pushone
      &pushregister 1
      &pushsdbgm 87							//'m_NLCMenuMC'
      &dcallmp 73							// setOnClickCallBack()
      &pushregister 1
      &pushsdbgm 87							//'m_NLCMenuMC'
      &pushsdb 33							//'_xrotation'
      &pushsdbgv 1							//'feg_m_mainMenu3D'
      &pushsdbgm 90							//'NLC_XROT'
      &setMember
      &pushregister 1
      &pushsdbgm 87							//'m_NLCMenuMC'
      &pushsdb 35							//'_yrotation'
      &pushsdbgv 1							//'feg_m_mainMenu3D'
      &pushsdbgm 91							//'NLC_YROT'
      &setMember
      &pushregister 1
      &pushsdbgm 87							//'m_NLCMenuMC'
      &pushsdb 37							//'_x'
      &pushsdbgv 1							//'feg_m_mainMenu3D'
      &pushsdbgm 92							//'NLC_X'
      &setMember
      &pushregister 1
      &pushsdbgm 87							//'m_NLCMenuMC'
      &pushsdb 39							//'_y'
      &pushsdbgv 1							//'feg_m_mainMenu3D'
      &pushsdbgm 93							//'NLC_Y'
      &setMember
      &pushregister 1
      &pushsdbgm 87							//'m_NLCMenuMC'
      &pushsdb 57							//'_z'
      &pushsdbgv 1							//'feg_m_mainMenu3D'
      &pushsdbgm 94							//'NLC_Z'
      &setMember
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 71							//'processUserInputMainMenu'
    &function2  (r:3='menuID', r:2='itemID') (r:1='this')
      &pushregister 3
      &setRegister r:0
      &pushsdbgv 1							//'feg_m_mainMenu3D'
      &pushsdbgm 95							//'MENU_ITEM_1'
      &strictEquals
      &jnz label2      
      &pushregister 0
      &pushsdbgv 1							//'feg_m_mainMenu3D'
      &pushsdbgm 78							//'MENU_ITEM_2'
      &strictEquals
      &jnz label3      
      &pushregister 0
      &pushsdbgv 1							//'feg_m_mainMenu3D'
      &pushsdbgm 96							//'MENU_ITEM_3'
      &strictEquals
      &jnz label4      
      &pushregister 0
      &pushsdbgv 1							//'feg_m_mainMenu3D'
      &pushsdbgm 97							//'MENU_ITEM_4'
      &strictEquals
      &jnz label5      
      &pushregister 0
      &pushsdbgv 1							//'feg_m_mainMenu3D'
      &pushsdbgm 98							//'MENU_ITEM_5'
      &strictEquals
      &jnz label6      
      &pushregister 0
      &pushsdbgv 1							//'feg_m_mainMenu3D'
      &pushsdbgm 99							//'MENU_ITEM_6'
      &strictEquals
      &jnz label7      
      &pushregister 0
      &pushsdbgv 1							//'feg_m_mainMenu3D'
      &pushsdbgm 100							//'MENU_ITEM_7'
      &strictEquals
      &jnz label8      
      &jmp label9      
     label2:
      &pushregister 2
      &pushone
      &pushregister 1
      &dcallmp 101							// processTutorialMenuClick()
      &jmp label9      
     label3:
      &pushregister 2
      &pushone
      &pushregister 1
      &dcallmp 102							// processCampaignMenuClick()
      &jmp label9      
     label4:
      &pushzero
      &pushregister 1
      &dcallmp 103							// processScenarioMenuClick()
      &jmp label9      
     label5:
      &pushzero
      &pushregister 1
      &dcallmp 104							// processSkirmishMenuClick()
      &jmp label9      
     label6:
      &pushregister 2
      &pushone
      &pushregister 1
      &dcallmp 105							// processProfileMenuClick()
      &jmp label9      
     label7:
      &pushregister 2
      &pushone
      &pushregister 1
      &dcallmp 106							// processOptionsMenuClick()
      &jmp label9      
     label8:
      &pushregister 2
      &pushone
      &pushregister 1
      &dcallmp 107							// processQuitMenuClick()
      &jmp label9      
     label9:
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 101							//'processTutorialMenuClick'
    &function2  () (r:1='this', r:2='_global')
      &pushregister 2
      &pushsdbgm 108							//'SCREEN'
      &pushsdbgm 109							//'FEG_TUTORIALMENU'
      &pushone
      &pushregister 1
      &dcallmp 110							// startOuttro()
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 104							//'processSkirmishMenuClick'
    &function2  () (r:1='this', r:2='_global')
      &pushregister 2
      &pushsdbgm 108							//'SCREEN'
      &pushsdbgm 111							//'FES_SKIRMISH_SETUP'
      &pushone
      &pushregister 1
      &dcallmp 110							// startOuttro()
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 103							//'processScenarioMenuClick'
    &function2  () (r:1='this', r:2='_global')
      &pushregister 2
      &pushsdbgm 108							//'SCREEN'
      &pushsdbgm 112							//'FES_SCENARIO_MAP'
      &pushone
      &pushregister 1
      &dcallmp 110							// startOuttro()
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 102							//'processCampaignMenuClick'
    &function2  (r:3='menuItem') (r:1='this', r:2='_global')
      &pushregister 3
      &setRegister r:0
      &pushone
      &strictEquals
      &jnz label10      
      &pushregister 0
      &pushbyte 2
      &strictEquals
      &jnz label11      
      &pushregister 0
      &pushbyte 3
      &strictEquals
      &jnz label12      
      &pushregister 0
      &pushbyte 4
      &strictEquals
      &jnz label13      
      &pushregister 0
      &pushbyte 5
      &strictEquals
      &jnz label14      
      &pushregister 0
      &pushbyte 6
      &strictEquals
      &jnz label15      
      &jmp label16      
     label10:
      &pushzero
      &pushregister 1
      &dcallmp 113							// processRecapMenuClick()
      &jmp label17      
     label11:
      &pushbyte -1
      &pushone
      &pushregister 1
      &dcallmp 114							// processNewMenuClick()
      &jmp label17      
     label12:
      &pushbyte -1
      &pushone
      &pushregister 1
      &dcallmp 115							// processLoadMenuClick()
      &jmp label17      
     label13:
      &pushbyte -1
      &pushone
      &pushregister 1
      &dcallmp 116							// processContinueMenuClick()
      &jmp label17      
     label14:
      &pushbyte -1
      &pushone
      &pushregister 1
      &dcallmp 117							// processChaptersMenuClick()
      &jmp label17      
     label15:
      &pushsdb 118							//'feg_m_mainMenu3D::processCampaignMenuClick() Level List / Map List'
      &trace
      &pushregister 2
      &pushsdbgm 108							//'SCREEN'
      &pushsdbgm 119							//'FES_SINGLE_PLAYER_MENU'
      &pushone
      &pushregister 1
      &dcallmp 110							// startOuttro()
      &jmp label17      
     label16:
      &jmp label17      
     label17:
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 113							//'processRecapMenuClick'
    &function  (    )    
      &getURL 'FSCommand:CallGameFunction'       '%PlayMovieAtomic?movie=RECAPFS|allowCancel=1'      
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 105							//'processProfileMenuClick'
    &function2  (r:3='menuItem') (r:1='this', r:2='_global')
      &pushregister 3
      &setRegister r:0
      &pushone
      &strictEquals
      &jnz label18      
      &pushregister 0
      &pushbyte 2
      &strictEquals
      &jnz label19      
      &pushregister 0
      &pushbyte 3
      &strictEquals
      &jnz label20      
      &jmp label21      
     label18:
      &pushsdb 120							//'feg_m_mainMenu3D::processProfileMenuClick() :: Management'
      &trace
      &pushregister 2
      &pushsdbgm 108							//'SCREEN'
      &pushsdbgm 121							//'FEG_PROFILE_MANAGER'
      &pushone
      &pushregister 1
      &dcallmp 110							// startOuttro()
      &jmp label22      
     label19:
      &pushsdb 122							//'feg_m_mainMenu3D::processProfileMenuClick() :: Film Archive'
      &trace
      &pushregister 2
      &pushsdbgm 108							//'SCREEN'
      &pushsdbgm 123							//'FEG_FILMARCHIVE'
      &pushone
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 3							//'m_thisClass'
      &dcallmp 110							// startOuttro()
      &jmp label22      
     label20:
      &pushsdb 124							//'feg_m_mainMenu3D::processProfileMenuClick() :: Replay Theater'
      &trace
      &pushregister 2
      &pushsdbgm 108							//'SCREEN'
      &pushsdbgm 125							//'FEG_SAVELOAD'
      &pushone
      &pushregister 1
      &dcallmp 110							// startOuttro()
      &jmp label22      
     label21:
      &jmp label22      
     label22:
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 106							//'processOptionsMenuClick'
    &function2  (r:3='menuItem') (r:1='this', r:2='_global')
      &pushregister 3
      &setRegister r:0
      &pushone
      &strictEquals
      &jnz label23      
      &pushregister 0
      &pushbyte 2
      &strictEquals
      &jnz label24      
      &jmp label25      
     label23:
      &pushsdb 126							//'feg_m_mainMenu3D::processOptionsMenuClick::Options : Settings'
      &trace
      &pushregister 2
      &pushsdbgm 108							//'SCREEN'
      &pushsdbgm 127							//'FEG_OPTIONS'
      &pushone
      &pushregister 1
      &dcallmp 110							// startOuttro()
      &jmp label26      
     label24:
      &pushsdb 128							//'feg_m_mainMenu3D::processOptionsMenuClick::Options : Credits'
      &trace
      &pushzero
      &pushregister 1
      &dcallmp 129							// goToCredits()
      &jmp label26      
     label25:
      &jmp label26      
     label26:
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 107							//'processQuitMenuClick'
    &function2  (r:1='menuItem') ()
      &pushregister 1
      &pushbyte -1
      &strictEquals
      &jnz label27      
      &jmp label28      
     label27:
      &getURL 'FSCommand:CallGameFunction'       '%ExitApplication'      
      &jmp label29      
     label28:
      &jmp label29      
     label29:
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 89							//'processUserInputNLC'
    &function2  (r:3='menuID', r:2='itemID') (r:1='this')
      &pushregister 3
      &setRegister r:0
      &pushsdbgv 1							//'feg_m_mainMenu3D'
      &pushsdbgm 95							//'MENU_ITEM_1'
      &strictEquals
      &jnz label30      
      &pushregister 0
      &pushsdbgv 1							//'feg_m_mainMenu3D'
      &pushsdbgm 78							//'MENU_ITEM_2'
      &strictEquals
      &jnz label31      
      &pushregister 0
      &pushsdbgv 1							//'feg_m_mainMenu3D'
      &pushsdbgm 96							//'MENU_ITEM_3'
      &strictEquals
      &jnz label32      
      &pushregister 0
      &pushsdbgv 1							//'feg_m_mainMenu3D'
      &pushsdbgm 97							//'MENU_ITEM_4'
      &strictEquals
      &jnz label33      
      &pushregister 0
      &pushsdbgv 1							//'feg_m_mainMenu3D'
      &pushsdbgm 98							//'MENU_ITEM_5'
      &strictEquals
      &jnz label34      
      &jmp label35      
     label30:
      &pushregister 2
      &pushone
      &pushregister 1
      &dcallmp 114							// processNewMenuClick()
      &jmp label35      
     label31:
      &pushregister 2
      &pushone
      &pushregister 1
      &dcallmp 115							// processLoadMenuClick()
      &jmp label35      
     label32:
      &pushregister 2
      &pushone
      &pushregister 1
      &dcallmp 116							// processContinueMenuClick()
      &jmp label35      
     label33:
      &pushregister 2
      &pushone
      &pushregister 1
      &dcallmp 117							// processChaptersMenuClick()
      &jmp label35      
     label34:
      &pushregister 2
      &pushone
      &pushregister 1
      &dcallmp 130							// processBackMenuClick()
      &jmp label35      
     label35:
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 114							//'processNewMenuClick'
    &function2  (r:3='menuItem') (r:1='this', r:2='_global')
      &pushregister 3
      &pushbyte -1
      &strictEquals
      &jnz label36      
      &jmp label37      
     label36:
      &pushregister 2
      &pushsdbgm 4							//'GAME'
      &pushsdb 8							//'MODE'
      &pushregister 2
      &pushsdbgm 6							//'ENUM'
      &pushsdbgm 131							//'GAME_MODE_CAMPAIGN'
      &setMember
      &pushregister 2
      &pushsdbgm 108							//'SCREEN'
      &pushsdbgm 132							//'FES_FACTION_SELECT_MENU'
      &pushone
      &pushregister 1
      &dcallmp 110							// startOuttro()
      &jmp label38      
     label37:
      &jmp label38      
     label38:
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 115							//'processLoadMenuClick'
    &function2  (r:3='menuItem') (r:1='this', r:2='_global')
      &pushregister 3
      &pushbyte -1
      &strictEquals
      &jnz label39      
      &jmp label40      
     label39:
      &pushregister 2
      &pushsdbgm 4							//'GAME'
      &pushsdb 8							//'MODE'
      &pushregister 2
      &pushsdbgm 6							//'ENUM'
      &pushsdbgm 131							//'GAME_MODE_CAMPAIGN'
      &setMember
      &pushregister 2
      &pushsdbgm 4							//'GAME'
      &pushsdb 5							//'SAVELOADSCREEN_TYPE'
      &pushregister 2
      &pushsdbgm 6							//'ENUM'
      &pushsdbgm 7							//'SAVELOADSCREEN_LOAD'
      &setMember
      &pushregister 2
      &pushsdbgm 108							//'SCREEN'
      &pushsdbgm 125							//'FEG_SAVELOAD'
      &pushone
      &pushregister 1
      &dcallmp 110							// startOuttro()
      &jmp label41      
     label40:
      &jmp label41      
     label41:
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 116							//'processContinueMenuClick'
    &function2  (r:2='menuItem') (r:1='_global')
      &pushregister 2
      &pushbyte -1
      &strictEquals
      &jnz label42      
      &jmp label43      
     label42:
      &pushregister 1
      &pushsdbgm 4							//'GAME'
      &pushsdb 8							//'MODE'
      &pushregister 1
      &pushsdbgm 6							//'ENUM'
      &pushsdbgm 131							//'GAME_MODE_CAMPAIGN'
      &setMember
      &getURL 'FSCommand:CallGameFunction'       'LoadMostRecentCampaignSave'      
      &jmp label44      
     label43:
      &jmp label44      
     label44:
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 117							//'processChaptersMenuClick'
    &function2  (r:3='menuItem') (r:1='this', r:2='_global')
      &pushregister 3
      &pushbyte -1
      &strictEquals
      &jnz label45      
      &jmp label46      
     label45:
      &pushregister 2
      &pushsdbgm 4							//'GAME'
      &pushsdb 8							//'MODE'
      &pushregister 2
      &pushsdbgm 6							//'ENUM'
      &pushsdbgm 131							//'GAME_MODE_CAMPAIGN'
      &setMember
      &pushregister 2
      &pushsdbgm 108							//'SCREEN'
      &pushsdbgm 133							//'FES_CHAPTER_SELECT'
      &pushone
      &pushregister 1
      &dcallmp 110							// startOuttro()
      &jmp label47      
     label46:
      &jmp label47      
     label47:
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 130							//'processBackMenuClick'
    &function2  (r:2='menuItem') (r:1='this')
      &pushregister 2
      &pushbyte -1
      &strictEquals
      &jnz label48      
      &jmp label49      
     label48:
      &pushzero
      &pushregister 1
      &dcallmp 134							// transitionToMainMenu()
      &jmp label50      
     label49:
      &jmp label50      
     label50:
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 129							//'goToCredits'
    &function2  () (r:1='this')
      &pushzero
      &pushregister 1
      &dcallmp 135							// showCredits()
      &pushzero
      &pushregister 1
      &dcallmp 136							// hideMainMenu()
      &pushzero
      &pushregister 1
      &dcallmp 137							// startCredits()
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 138							//'goToMainMenu'
    &function2  () (r:1='this')
      &pushzero
      &pushregister 1
      &dcallmp 139							// hideCredits()
      &pushzero
      &pushregister 1
      &dcallmp 140							// showMainMenu()
      &pushzero
      &pushregister 1
      &dcallmp 141							// stopCredits()
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 142							//'initMenuParameters'
    &function2  () (r:1='this')
      &pushregister 1
      &pushsdb 143							//'m_menuUI'
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 12							//'m_screen'
      &pushsdbgm 144							//'menuUI'
      &setMember
      &pushsdbgv 145							//'MENU_SCALE'
      &pushsdbgv 1							//'feg_m_mainMenu3D'
      &pushsdbgm 45							//'UI_Y_ORIGIN'
      &pushshort 231
      &add
      &pushsdbgv 1							//'feg_m_mainMenu3D'
      &pushsdbgm 46							//'UI_POS_PADDING'
      &add
      &pushsdbgv 1							//'feg_m_mainMenu3D'
      &pushsdbgm 47							//'UI_X_ORIGIN'
      &pushshort 501
      &add
      &pushsdbgv 1							//'feg_m_mainMenu3D'
      &pushsdbgm 46							//'UI_POS_PADDING'
      &add
      &pushregister 1
      &pushsdbgm 69							//'m_mainMenuMC'
      &pushone
      &initArray
      &pushsdbgv 1							//'feg_m_mainMenu3D'
      &pushsdbgm 146							//'MENU_UI_NAME'
      &pushbyte 5
      &pushregister 1
      &pushsdbgm 143							//'m_menuUI'
      &dcallmp 49							// init()
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 147							//'loadpersonalityImageMovie'
    &function2  () (r:1='this')
      &pushregister 1
      &pushsdb 148							//'m_personalityImageMC'
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 12							//'m_screen'
      &pushsdbgm 149							//'personalityImageMC'
      &setMember
      &pushregister 1
      &pushsdbgm 148							//'m_personalityImageMC'
      &pushsdb 14							//'_visible'
      &pushtrue
      &setMember
      &pushregister 1
      &pushsdbgm 148							//'m_personalityImageMC'
      &pushsdb 33							//'_xrotation'
      &pushsdbgv 1							//'feg_m_mainMenu3D'
      &pushsdbgm 150							//'PERSONALITY_IMAGE_XROT'
      &setMember
      &pushregister 1
      &pushsdbgm 148							//'m_personalityImageMC'
      &pushsdb 35							//'_yrotation'
      &pushsdbgv 1							//'feg_m_mainMenu3D'
      &pushsdbgm 151							//'PERSONALITY_IMAGE_YROT'
      &setMember
      &pushregister 1
      &pushsdbgm 148							//'m_personalityImageMC'
      &pushsdb 37							//'_x'
      &pushsdbgv 1							//'feg_m_mainMenu3D'
      &pushsdbgm 152							//'PERSONALITY_IMAGE_X'
      &setMember
      &pushregister 1
      &pushsdbgm 148							//'m_personalityImageMC'
      &pushsdb 39							//'_y'
      &pushsdbgv 1							//'feg_m_mainMenu3D'
      &pushsdbgm 153							//'PERSONALITY_IMAGE_Y'
      &setMember
      &pushregister 1
      &pushsdbgm 148							//'m_personalityImageMC'
      &pushsdb 57							//'_z'
      &pushsdbgv 1							//'feg_m_mainMenu3D'
      &pushsdbgm 154							//'PERSONALITY_IMAGE_Z'
      &setMember
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 155							//'initPersonalityImageParameters'
    &function2  () (r:1='this')
      &pushregister 1
      &pushsdb 156							//'m_personalityImageUI'
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 12							//'m_screen'
      &pushsdbgm 157							//'personalityImageUI'
      &setMember
      &pushsdbgv 158							//'PERSONALITY_IMAGE_SCALE'
      &pushsdbgv 1							//'feg_m_mainMenu3D'
      &pushsdbgm 45							//'UI_Y_ORIGIN'
      &pushshort 462
      &add
      &pushsdbgv 1							//'feg_m_mainMenu3D'
      &pushsdbgm 46							//'UI_POS_PADDING'
      &add
      &pushsdbgv 1							//'feg_m_mainMenu3D'
      &pushsdbgm 47							//'UI_X_ORIGIN'
      &pushshort 501
      &add
      &pushsdbgv 1							//'feg_m_mainMenu3D'
      &pushsdbgm 46							//'UI_POS_PADDING'
      &add
      &pushregister 1
      &pushsdbgm 148							//'m_personalityImageMC'
      &pushone
      &initArray
      &pushsdbgv 1							//'feg_m_mainMenu3D'
      &pushsdbgm 159							//'PERSONALITY_UI_NAME'
      &pushbyte 5
      &pushregister 1
      &pushsdbgm 156							//'m_personalityImageUI'
      &dcallmp 49							// init()
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 160							//'loadCreditsMovie'
    &function2  (r:4='buttons') (r:1='this', r:2='_global')
      &pushsdb 161							//'m_creditMC'
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 12							//'m_screen'
      &pushsdbgm 15							//'creditMC'
      &setVariable
      &pushsdbgv 161							//'m_creditMC'
      &pushsdb 14							//'_visible'
      &pushtrue
      &setMember
      &pushsdbgv 161							//'m_creditMC'
      &pushsdb 33							//'_xrotation'
      &pushsdbgv 1							//'feg_m_mainMenu3D'
      &pushsdbgm 162							//'CREDIT_XROT'
      &setMember
      &pushsdbgv 161							//'m_creditMC'
      &pushsdb 35							//'_yrotation'
      &pushsdbgv 1							//'feg_m_mainMenu3D'
      &pushsdbgm 163							//'CREDIT_YROT'
      &setMember
      &pushsdbgv 161							//'m_creditMC'
      &pushsdb 37							//'_x'
      &pushsdbgv 1							//'feg_m_mainMenu3D'
      &pushsdbgm 164							//'CREDIT_X'
      &setMember
      &pushsdbgv 161							//'m_creditMC'
      &pushsdb 39							//'_y'
      &pushsdbgv 1							//'feg_m_mainMenu3D'
      &pushsdbgm 165							//'CREDIT_Y'
      &setMember
      &pushsdbgv 161							//'m_creditMC'
      &pushsdbgm 166							//'backButton'
      &setRegister r:3
      &pop
      &pushregister 3
      &pushsdb 14							//'_visible'
      &pushtrue
      &setMember
      &pushzero
      &pushregister 3
      &dcallmp 167							// enable()
      &pushzero
      &pushregister 3
      &dcallmp 80							// show()
      &pushregister 1
      &pushsdbgm 138							//'goToMainMenu'
      &pushregister 1
      &pushbyte 2
      &pushregister 2
      &pushsdb 17							//'bind0'
      &callMethod
      &pushone
      &pushregister 3
      &dcallmp 168							// setOnMouseUpFunction()
      &pushregister 3
      &pushone
      &pushregister 4
      &dcallmp 169							// push()
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 170							//'loadHammerNSickleMovie'
    &function2  () (r:1='this')
      &pushregister 1
      &pushsdb 171							//'m_HammerSickleMC'
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 12							//'m_screen'
      &pushsdbgm 172							//'hammerMC'
      &setMember
      &pushregister 1
      &pushsdbgm 171							//'m_HammerSickleMC'
      &pushsdb 14							//'_visible'
      &pushtrue
      &setMember
      &pushregister 1
      &pushsdbgm 171							//'m_HammerSickleMC'
      &pushsdb 33							//'_xrotation'
      &pushsdbgv 1							//'feg_m_mainMenu3D'
      &pushsdbgm 173							//'HAMMER_XROT'
      &setMember
      &pushregister 1
      &pushsdbgm 171							//'m_HammerSickleMC'
      &pushsdb 35							//'_yrotation'
      &pushsdbgv 1							//'feg_m_mainMenu3D'
      &pushsdbgm 174							//'HAMMER_YROT'
      &setMember
      &pushregister 1
      &pushsdbgm 171							//'m_HammerSickleMC'
      &pushsdb 37							//'_x'
      &pushsdbgv 1							//'feg_m_mainMenu3D'
      &pushsdbgm 175							//'HAMMER_X'
      &setMember
      &pushregister 1
      &pushsdbgm 171							//'m_HammerSickleMC'
      &pushsdb 39							//'_y'
      &pushsdbgv 1							//'feg_m_mainMenu3D'
      &pushsdbgm 176							//'HAMMER_Y'
      &setMember
      &pushregister 1
      &pushsdbgm 171							//'m_HammerSickleMC'
      &pushsdb 57							//'_z'
      &pushsdbgv 1							//'feg_m_mainMenu3D'
      &pushsdbgm 177							//'HAMMER_Z'
      &setMember
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 178							//'initHammerParameters'
    &function2  () (r:1='this')
      &pushregister 1
      &pushsdb 179							//'m_hammerSickleUI'
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 12							//'m_screen'
      &pushsdbgm 180							//'hammerUI'
      &setMember
      &pushsdbgv 181							//'HAMMER_SCALE'
      &pushsdbgv 1							//'feg_m_mainMenu3D'
      &pushsdbgm 45							//'UI_Y_ORIGIN'
      &pushsdbgv 1							//'feg_m_mainMenu3D'
      &pushsdbgm 47							//'UI_X_ORIGIN'
      &pushshort 501
      &add
      &pushsdbgv 1							//'feg_m_mainMenu3D'
      &pushsdbgm 46							//'UI_POS_PADDING'
      &add
      &pushregister 1
      &pushsdbgm 171							//'m_HammerSickleMC'
      &pushone
      &initArray
      &pushsdbgv 1							//'feg_m_mainMenu3D'
      &pushsdbgm 182							//'HAMMER_UI_NAME'
      &pushbyte 5
      &pushregister 1
      &pushsdbgm 179							//'m_hammerSickleUI'
      &dcallmp 49							// init()
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 183							//'loadTestControls'
    &function2  () (r:1='this')
      &pushregister 1
      &pushsdb 184							//'m_testUIMC'
      &pushsdbgv 1							//'feg_m_mainMenu3D'
      &pushsdbgm 185							//'UI_ADJUSTMENT_LAYER'
      &pushsdb 186							//'testControls'
      &pushsdb 187							//'userInterfaceSymbol'
      &pushbyte 3
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 12							//'m_screen'
      &pushsdb 188							//'attachMovie'
      &callMethod
      &setMember
      &pushregister 1
      &pushsdbgm 184							//'m_testUIMC'
      &pushsdb 14							//'_visible'
      &pushtrue
      &setMember
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 189							//'loadGuide'
    &function2  () (r:1='this')
      &pushregister 1
      &pushsdb 190							//'m_guideMC'
      &pushsdbgv 1							//'feg_m_mainMenu3D'
      &pushsdbgm 191							//'UI_IMAGEGUIDE_LAYER'
      &pushsdb 192							//'imageGuide'
      &pushsdb 193							//'image_guideSymbol'
      &pushbyte 3
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 12							//'m_screen'
      &pushsdb 188							//'attachMovie'
      &callMethod
      &setMember
      &pushregister 1
      &pushsdbgm 190							//'m_guideMC'
      &pushsdb 14							//'_visible'
      &pushtrue
      &setMember
      &pushregister 1
      &pushsdbgm 190							//'m_guideMC'
      &pushsdb 194							//'_alpha'
      &pushbyte 50
      &setMember
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 28							//'initGUI'
    &function2  () (r:1='this', r:2='_global')
      &pushsdb 195							//'m_localMessageHandlerFunc'
      &pushregister 1
      &pushsdbgm 196							//'localMessageHandler'
      &pushregister 1
      &pushbyte 2
      &pushregister 2
      &dcallmsv 197							// bind1DynamicParams()
      &pushsdbgv 195							//'m_localMessageHandlerFunc'
      &pushone
      &pushregister 2
      &pushsdbgm 198							//'gMH'
      &dcallmp 199							// addMessageHandler()
      &pushzero
      &pushsdb 200							//'Array'
      &new
      &setRegister r:3
      &pop
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 12							//'m_screen'
      &pushsdbgm 13							//'newProfileMC'
      &pushsdbgm 201							//'btnNewConfirm'
      &pushone
      &pushregister 3
      &dcallmp 169							// push()
      &pushzero
      &pushregister 1
      &dcallmp 50							// loadWallMovie()
      &pushzero
      &pushregister 1
      &dcallmp 30							// loadFloorMovie()
      &pushzero
      &pushregister 1
      &dcallmp 147							// loadpersonalityImageMovie()
      &pushzero
      &pushregister 1
      &dcallmp 170							// loadHammerNSickleMovie()
      &pushzero
      &pushregister 1
      &dcallmp 68							// loadMainMenu()
      &pushzero
      &pushregister 1
      &dcallmp 86							// loadNewLoadContinueMenu()
      &pushregister 3
      &pushone
      &pushregister 1
      &dcallmp 160							// loadCreditsMovie()
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 12							//'m_screen'
      &pushsdbgm 202							//'showHideTestUI'
      &pushsdb 37							//'_x'
      &pushsdbgv 1							//'feg_m_mainMenu3D'
      &pushsdbgm 47							//'UI_X_ORIGIN'
      &setMember
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 12							//'m_screen'
      &pushsdbgm 202							//'showHideTestUI'
      &pushsdb 39							//'_y'
      &pushsdbgv 1							//'feg_m_mainMenu3D'
      &pushsdbgm 45							//'UI_Y_ORIGIN'
      &pushshort 580
      &add
      &setMember
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 12							//'m_screen'
      &pushsdbgm 203							//'showHideGuideMC'
      &pushsdb 37							//'_x'
      &pushsdbgv 1							//'feg_m_mainMenu3D'
      &pushsdbgm 47							//'UI_X_ORIGIN'
      &pushsdbgv 202							//'showHideTestUI'
      &pushsdbgm 37							//'_x'
      &add
      &pushbyte 10
      &add
      &setMember
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 12							//'m_screen'
      &pushsdbgm 203							//'showHideGuideMC'
      &pushsdb 39							//'_y'
      &pushsdbgv 1							//'feg_m_mainMenu3D'
      &pushsdbgm 45							//'UI_Y_ORIGIN'
      &pushshort 580
      &add
      &setMember
      &pushzero
      &pushregister 1
      &pushsdbgm 69							//'m_mainMenuMC'
      &dcallmp 60							// disable()
      &pushzero
      &pushregister 1
      &dcallmp 139							// hideCredits()
      &pushzero
      &pushregister 1
      &dcallmp 204							// initNewProfilePrompt()
      &pushzero
      &pushsdb 22							//'Object'
      &new
      &setRegister r:4
      &pop
      &pushsdb 205							//'QueryGameEngine?FE_PROFILE_COUNT'
      &pushregister 4
      &loadVariables
      &pushregister 4
      &pushsdbgm 206							//'FE_PROFILE_COUNT'
      &pushsdb 207							//'0'
      &equals
      &not
      &jnz label51      
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 12							//'m_screen'
      &pushsdbgm 13							//'newProfileMC'
      &pushsdb 14							//'_visible'
      &pushtrue
      &setMember
      &pushzero
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 12							//'m_screen'
      &pushsdbgm 13							//'newProfileMC'
      &pushsdbgm 208							//'newProfileName'
      &dcallmp 209							// takeFocus()
     label51:
      &pushzero
      &pushregister 1
      &dcallmp 210							// showScenarioAward()
      &getURL 'FSCommand:CallGameFunction'       'ScenarioClearFirstTimeCompletionState'      
      &pushzero
      &pushregister 1
      &dcallmp 211							// showCampaignAward()
      &pushzero
      &pushregister 1
      &dcallmp 212							// showTutorialAward()
      &pushregister 3
      &pushone
      &pushregister 1
      &dcallmp 213							// registerIntroOuttroComponents()
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 139							//'hideCredits'
    &function  (    )    
      &pushsdbgv 161							//'m_creditMC'
      &pushsdb 14							//'_visible'
      &pushfalse
      &setMember
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 135							//'showCredits'
    &function  (    )    
      &pushsdbgv 161							//'m_creditMC'
      &pushsdb 14							//'_visible'
      &pushtrue
      &setMember
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 204							//'initNewProfilePrompt'
    &function2  () (r:1='this', r:2='_global')
      &pushregister 1
      &pushsdbgm 214							//'onCreateProfile'
      &pushregister 1
      &pushbyte 2
      &pushregister 2
      &pushsdb 17							//'bind0'
      &callMethod
      &pushone
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 12							//'m_screen'
      &pushsdbgm 13							//'newProfileMC'
      &pushsdbgm 201							//'btnNewConfirm'
      &dcallmp 168							// setOnMouseUpFunction()
      &pushregister 1
      &pushsdbgm 214							//'onCreateProfile'
      &pushregister 1
      &pushbyte 2
      &pushregister 2
      &pushsdb 17							//'bind0'
      &callMethod
      &pushone
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 12							//'m_screen'
      &pushsdbgm 13							//'newProfileMC'
      &pushsdbgm 208							//'newProfileName'
      &dcallmp 215							// setOnEnterFunction()
      &pushzero
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 12							//'m_screen'
      &pushsdbgm 13							//'newProfileMC'
      &pushsdbgm 201							//'btnNewConfirm'
      &dcallmp 167							// enable()
      &pushzero
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 12							//'m_screen'
      &pushsdbgm 13							//'newProfileMC'
      &pushsdbgm 216							//'btnNewCancel'
      &dcallmp 60							// disable()
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 214							//'onCreateProfile'
    &function2  () ()
      &pushzero
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 12							//'m_screen'
      &pushsdbgm 13							//'newProfileMC'
      &pushsdbgm 208							//'newProfileName'
      &pushsdb 217							//'getContents'
      &callMethod
      &setRegister r:1
      &pop
      &pushsdb 218							//'FSCommand:CallGameFunction'
      &pushsdb 219							//'%AddProfile?ProfileName='
      &pushregister 1
      &add
      &getURL2
      &pushsdb 220							//''
      &pushone
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 12							//'m_screen'
      &pushsdbgm 13							//'newProfileMC'
      &pushsdbgm 208							//'newProfileName'
      &dcallmp 221							// setContents()
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 136							//'hideMainMenu'
    &function2  () (r:1='this')
      &pushregister 1
      &pushsdbgm 69							//'m_mainMenuMC'
      &pushsdb 14							//'_visible'
      &pushfalse
      &setMember
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 140							//'showMainMenu'
    &function2  () (r:1='this')
      &pushregister 1
      &pushsdbgm 69							//'m_mainMenuMC'
      &pushsdb 14							//'_visible'
      &pushtrue
      &setMember
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 137							//'startCredits'
    &function2  () (r:1='this', r:2='_global')
      &pushregister 1
      &pushsdbgm 222							//'m_isCreditsListScrolling'
      &not
      &not
      &jnz label52      
      &pushregister 1
      &pushsdb 222							//'m_isCreditsListScrolling'
      &pushtrue
      &setMember
      &pushregister 2
      &pushsdbgm 24							//'SOUND'
      &pushsdbgm 223							//'CREDITS'
      &pushone
      &pushregister 2
      &dcallmp 27							// PlayShellMusic()
      &getURL 'FSCommand:CallGameFunction'       'Options_StartCredits'      
     label52:
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 141							//'stopCredits'
    &function2  () (r:1='this', r:2='_global')
      &pushregister 1
      &pushsdbgm 222							//'m_isCreditsListScrolling'
      &not
      &jnz label53      
      &pushregister 1
      &pushsdb 222							//'m_isCreditsListScrolling'
      &pushfalse
      &setMember
      &pushregister 2
      &pushsdbgm 24							//'SOUND'
      &pushsdbgm 25							//'SHELL_MAIN'
      &pushone
      &pushregister 2
      &dcallmp 27							// PlayShellMusic()
      &getURL 'FSCommand:CallGameFunction'       'Options_StopCredits'      
     label53:
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 16							//'onScreenExit'
    &function2  () (r:1='this', r:2='_global')
      &getURL 'FSCommand:CallGameFunction'       'InterruptSilentPatchCheck'      
      &pushzero
      &pushregister 1
      &dcallmp 139							// hideCredits()
      &pushzero
      &pushregister 1
      &dcallmp 141							// stopCredits()
      &pushsdbgv 195							//'m_localMessageHandlerFunc'
      &pushone
      &pushregister 2
      &pushsdbgm 198							//'gMH'
      &dcallmp 224							// removeMessageHandler()
      &pushsdb 195							//'m_localMessageHandlerFunc'
      &delete2
      &pop
      &pushzero
      &pushregister 1
      &dcallmp 225							// clearData()
      &pushregister 2
      &pushsdb 1							//'feg_m_mainMenu3D'
      &delete
      &pop
      &pushregister 2
      &pushsdb 2							//'Cafe2_BaseUIScreen'
      &delete
      &pop
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 196							//'localMessageHandler'
    &function2  (r:3='messageCode') (r:1='_global')
      &pushfalse
      &setRegister r:2
      &pop
      &pushregister 3
      &pushregister 1
      &pushsdbgm 226							//'MSGCODE'
      &pushsdbgm 227							//'FE_PROFILE_CREATE_SUCCESS'
      &strictEquals
      &jnz label54      
      &jmp label55      
     label54:
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 12							//'m_screen'
      &pushsdbgm 13							//'newProfileMC'
      &pushsdb 14							//'_visible'
      &pushfalse
      &setMember
      &pushtrue
      &setRegister r:2
      &pop
      &jmp label56      
     label55:
      &jmp label56      
     label56:
      &pushregister 2
      &return
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 228							//'toggleTestUI'
    &function2  () (r:1='this')
      &pushregister 1
      &pushsdb 229							//'m_testUIVisible'
      &pushregister 1
      &pushsdbgm 229							//'m_testUIVisible'
      &not
      &setMember
      &pushregister 1
      &pushsdbgm 229							//'m_testUIVisible'
      &not
      &jnz label57      
      &pushzero
      &pushregister 1
      &dcallmp 230							// showTestUI()
      &jmp label58      
     label57:
      &pushzero
      &pushregister 1
      &dcallmp 231							// hideTestUI()
     label58:
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 231							//'hideTestUI'
    &function2  () (r:1='this')
      &pushregister 1
      &pushsdbgm 64							//'m_wallUI'
      &pushsdb 14							//'_visible'
      &pushfalse
      &setMember
      &pushregister 1
      &pushsdbgm 42							//'m_floorUI'
      &pushsdb 14							//'_visible'
      &pushfalse
      &setMember
      &pushregister 1
      &pushsdbgm 179							//'m_hammerSickleUI'
      &pushsdb 14							//'_visible'
      &pushfalse
      &setMember
      &pushregister 1
      &pushsdbgm 143							//'m_menuUI'
      &pushsdb 14							//'_visible'
      &pushfalse
      &setMember
      &pushregister 1
      &pushsdbgm 156							//'m_personalityImageUI'
      &pushsdb 14							//'_visible'
      &pushfalse
      &setMember
      &pushregister 1
      &pushsdbgm 232							//'m_speedUI'
      &pushsdb 14							//'_visible'
      &pushfalse
      &setMember
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 230							//'showTestUI'
    &function2  () (r:1='this')
      &pushregister 1
      &pushsdbgm 64							//'m_wallUI'
      &pushsdb 14							//'_visible'
      &pushtrue
      &setMember
      &pushregister 1
      &pushsdbgm 42							//'m_floorUI'
      &pushsdb 14							//'_visible'
      &pushtrue
      &setMember
      &pushregister 1
      &pushsdbgm 179							//'m_hammerSickleUI'
      &pushsdb 14							//'_visible'
      &pushtrue
      &setMember
      &pushregister 1
      &pushsdbgm 143							//'m_menuUI'
      &pushsdb 14							//'_visible'
      &pushtrue
      &setMember
      &pushregister 1
      &pushsdbgm 156							//'m_personalityImageUI'
      &pushsdb 14							//'_visible'
      &pushtrue
      &setMember
      &pushregister 1
      &pushsdbgm 232							//'m_speedUI'
      &pushsdb 14							//'_visible'
      &pushtrue
      &setMember
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 233							//'toggleGuide'
    &function2  () (r:1='this')
      &pushregister 1
      &pushsdbgm 190							//'m_guideMC'
      &pushsdb 14							//'_visible'
      &pushregister 1
      &pushsdbgm 190							//'m_guideMC'
      &pushsdbgm 14							//'_visible'
      &not
      &setMember
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 234							//'initData'
    &function2  ('animationMC', r:1='uiMC', r:6='x', r:5='y', r:4='z', r:3='xrot', r:2='yrot') ()
      &pushregister 6
      &pushone
      &pushregister 1
      &pushsdbgm 235							//'xPosSliderUI'
      &dcallmp 236							// setValue()
      &pushregister 5
      &pushone
      &pushregister 1
      &pushsdbgm 237							//'yPosSliderUI'
      &dcallmp 236							// setValue()
      &pushregister 4
      &toNumber
      &pushone
      &pushregister 1
      &pushsdbgm 238							//'zPosSliderUI'
      &dcallmp 236							// setValue()
      &pushregister 3
      &pushone
      &pushregister 1
      &pushsdbgm 239							//'rotXSliderUI'
      &dcallmp 236							// setValue()
      &pushregister 2
      &pushone
      &pushregister 1
      &pushsdbgm 240							//'rotYSliderUI'
      &dcallmp 236							// setValue()
      &pushzero
      &pushregister 1
      &dcallmp 241							// update()
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 242							//'transitionToNLC'
    &function2  () (r:1='this')
      &pushsdbgv 1							//'feg_m_mainMenu3D'
      &pushsdb 243							//'m_frameCount'
      &pushzero
      &setMember
      &pushregister 1
      &pushsdbgm 87							//'m_NLCMenuMC'
      &pushsdb 244							//'onEnterFrame'
      &pushregister 1
      &pushsdbgm 245							//'updateNLCAnimation'
      &setMember
      &pushzero
      &pushone
      &pushregister 1
      &pushsdbgm 87							//'m_NLCMenuMC'
      &dcallmp 246							// gotoAndPlay()
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 245							//'updateNLCAnimation'
    &function2  () ()
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 3							//'m_thisClass'
      &pushsdbgm 87							//'m_NLCMenuMC'
      &setRegister r:2
      &pop
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 3							//'m_thisClass'
      &pushsdbgm 69							//'m_mainMenuMC'
      &setRegister r:3
      &pop
      &pushsdbgv 1							//'feg_m_mainMenu3D'
      &pushsdbgm 243							//'m_frameCount'
      &pushsdbgv 1							//'feg_m_mainMenu3D'
      &pushsdbgm 247							//'NLC_ANIM_LENGTH'
      &divide
      &setRegister r:1
      &pop
      &pushregister 2
      &pushsdb 37							//'_x'
      &pushregister 1
      &pushsdbgv 1							//'feg_m_mainMenu3D'
      &pushsdbgm 92							//'NLC_X'
      &pushsdbgv 1							//'feg_m_mainMenu3D'
      &pushsdbgm 83							//'MENU_X'
      &pushbyte 3
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 3							//'m_thisClass'
      &pushsdb 248							//'lerp'
      &callMethod
      &setMember
      &pushregister 2
      &pushsdb 57							//'_z'
      &pushregister 1
      &pushsdbgv 1							//'feg_m_mainMenu3D'
      &pushsdbgm 94							//'NLC_Z'
      &pushsdbgv 1							//'feg_m_mainMenu3D'
      &pushsdbgm 85							//'MENU_Z'
      &pushbyte 3
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 3							//'m_thisClass'
      &pushsdb 248							//'lerp'
      &callMethod
      &setMember
      &pushregister 3
      &pushsdb 37							//'_x'
      &pushregister 1
      &pushsdbgv 1							//'feg_m_mainMenu3D'
      &pushsdbgm 83							//'MENU_X'
      &pushsdbgv 1							//'feg_m_mainMenu3D'
      &pushsdbgm 83							//'MENU_X'
      &pushsdbgv 1							//'feg_m_mainMenu3D'
      &pushsdbgm 249							//'SLIDE_DISTANCE_X'
      &subtract
      &pushbyte 3
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 3							//'m_thisClass'
      &pushsdb 248							//'lerp'
      &callMethod
      &setMember
      &pushregister 3
      &pushsdb 57							//'_z'
      &pushregister 1
      &pushsdbgv 1							//'feg_m_mainMenu3D'
      &pushsdbgm 85							//'MENU_Z'
      &pushsdbgv 1							//'feg_m_mainMenu3D'
      &pushsdbgm 85							//'MENU_Z'
      &pushsdbgv 1							//'feg_m_mainMenu3D'
      &pushsdbgm 250							//'SLIDE_DISTANCE_Z'
      &subtract
      &pushbyte 3
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 3							//'m_thisClass'
      &pushsdb 248							//'lerp'
      &callMethod
      &setMember
      &pushsdbgv 1							//'feg_m_mainMenu3D'
      &pushsdbgm 243							//'m_frameCount'
      &pushsdbgv 1							//'feg_m_mainMenu3D'
      &pushsdbgm 247							//'NLC_ANIM_LENGTH'
      &lessThan
      &not
      &not
      &jnz label59      
      &pushzero
      &pushregister 2
      &dcallmp 251							// stop()
      &pushregister 2
      &pushsdb 244							//'onEnterFrame'
      &delete
      &pop
     label59:
      &pushsdbgv 1							//'feg_m_mainMenu3D'
      &pushsdb 243							//'m_frameCount'
      &pushsdbgv 1							//'feg_m_mainMenu3D'
      &pushsdbgm 243							//'m_frameCount'
      &increment
      &setMember
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 134							//'transitionToMainMenu'
    &function2  () (r:1='this')
      &pushsdbgv 1							//'feg_m_mainMenu3D'
      &pushsdb 243							//'m_frameCount'
      &pushzero
      &setMember
      &pushregister 1
      &pushsdbgm 69							//'m_mainMenuMC'
      &pushsdb 244							//'onEnterFrame'
      &pushregister 1
      &pushsdbgm 252							//'updateMainMenuAnimation'
      &setMember
      &pushzero
      &pushone
      &pushregister 1
      &pushsdbgm 69							//'m_mainMenuMC'
      &dcallmp 246							// gotoAndPlay()
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 252							//'updateMainMenuAnimation'
    &function2  () ()
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 3							//'m_thisClass'
      &pushsdbgm 87							//'m_NLCMenuMC'
      &setRegister r:3
      &pop
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 3							//'m_thisClass'
      &pushsdbgm 69							//'m_mainMenuMC'
      &setRegister r:2
      &pop
      &pushsdbgv 1							//'feg_m_mainMenu3D'
      &pushsdbgm 243							//'m_frameCount'
      &pushsdbgv 1							//'feg_m_mainMenu3D'
      &pushsdbgm 247							//'NLC_ANIM_LENGTH'
      &divide
      &setRegister r:1
      &pop
      &pushregister 2
      &pushsdb 37							//'_x'
      &pushregister 1
      &pushsdbgv 1							//'feg_m_mainMenu3D'
      &pushsdbgm 83							//'MENU_X'
      &pushsdbgv 1							//'feg_m_mainMenu3D'
      &pushsdbgm 249							//'SLIDE_DISTANCE_X'
      &subtract
      &pushsdbgv 1							//'feg_m_mainMenu3D'
      &pushsdbgm 83							//'MENU_X'
      &pushbyte 3
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 3							//'m_thisClass'
      &pushsdb 248							//'lerp'
      &callMethod
      &setMember
      &pushregister 2
      &pushsdb 57							//'_z'
      &pushregister 1
      &pushsdbgv 1							//'feg_m_mainMenu3D'
      &pushsdbgm 85							//'MENU_Z'
      &pushsdbgv 1							//'feg_m_mainMenu3D'
      &pushsdbgm 250							//'SLIDE_DISTANCE_Z'
      &subtract
      &pushsdbgv 1							//'feg_m_mainMenu3D'
      &pushsdbgm 85							//'MENU_Z'
      &pushbyte 3
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 3							//'m_thisClass'
      &pushsdb 248							//'lerp'
      &callMethod
      &setMember
      &pushregister 3
      &pushsdb 37							//'_x'
      &pushregister 1
      &pushsdbgv 1							//'feg_m_mainMenu3D'
      &pushsdbgm 83							//'MENU_X'
      &pushsdbgv 1							//'feg_m_mainMenu3D'
      &pushsdbgm 92							//'NLC_X'
      &pushbyte 3
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 3							//'m_thisClass'
      &pushsdb 248							//'lerp'
      &callMethod
      &setMember
      &pushregister 3
      &pushsdb 57							//'_z'
      &pushregister 1
      &pushsdbgv 1							//'feg_m_mainMenu3D'
      &pushsdbgm 85							//'MENU_Z'
      &pushsdbgv 1							//'feg_m_mainMenu3D'
      &pushsdbgm 94							//'NLC_Z'
      &pushbyte 3
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 3							//'m_thisClass'
      &pushsdb 248							//'lerp'
      &callMethod
      &setMember
      &pushsdbgv 1							//'feg_m_mainMenu3D'
      &pushsdbgm 243							//'m_frameCount'
      &pushsdbgv 1							//'feg_m_mainMenu3D'
      &pushsdbgm 247							//'NLC_ANIM_LENGTH'
      &lessThan
      &not
      &not
      &jnz label60      
      &pushzero
      &pushregister 2
      &dcallmp 251							// stop()
      &pushregister 2
      &pushsdb 244							//'onEnterFrame'
      &delete
      &pop
     label60:
      &pushsdbgv 1							//'feg_m_mainMenu3D'
      &pushsdb 243							//'m_frameCount'
      &pushsdbgv 1							//'feg_m_mainMenu3D'
      &pushsdbgm 243							//'m_frameCount'
      &increment
      &setMember
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 248							//'lerp'
    &function2  (r:3='num1', r:2='num2', r:1='fraction') ()
      &pushregister 3
      &pushregister 1
      &multiply
      &pushone
      &pushregister 1
      &subtract
      &pushregister 2
      &multiply
      &add
      &return
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 210							//'showScenarioAward'
    &function2  () (r:1='this', r:2='_global')
      &pushzero
      &pushsdb 22							//'Object'
      &new
      &setRegister r:3
      &pop
      &pushsdb 253							//'QueryGameEngine?SCENARIO_COMPLETION_STATE'
      &pushregister 3
      &loadVariables
      &pushregister 3
      &pushsdbgm 254							//'SCENARIO_COMPLETION_STATE_ALL_UNDER_PAR'
      &pushsdb 76							//'1'
      &equals
      &not
      &jnz label63      
      &pushzero
      &pushregister 1
      &pushsdbgm 51							//'m_wallMC'
      &pushsdbgm 59							//'bombFactoryMC'
      &dcallmp 167							// enable()
      &pushzero
      &pushregister 1
      &pushsdbgm 51							//'m_wallMC'
      &pushsdbgm 61							//'BackBombFactoryMC'
      &dcallmp 167							// enable()
      &pushzero
      &pushregister 1
      &pushsdbgm 51							//'m_wallMC'
      &pushsdbgm 62							//'FarBackBombFactoryMC'
      &dcallmp 167							// enable()
      &pushregister 3
      &pushsdbgm 255							//'SCENARIO_COMPLETION_STATE_ALL_UNDER_PAR_FIRSTTIME'
      &pushsdb 76							//'1'
      &equals
      &not
      &jnz label61      
      &pushsdw 256							//'GuiAccordionButtonStar3'
      &pushone
      &pushregister 2
      &pushsdw 257							//'playSound'
      &callmp
      &pushtrue
      &pushbyte 3
      &pushbyte 2
      &pushbyte 3
      &pushregister 1
      &pushsdbgm 69							//'m_mainMenuMC'
      &pushsdw 258							//'menuItemShowAward'
      &callmp
      &jmp label62      
     label61:
      &pushfalse
      &pushbyte 3
      &pushbyte 2
      &pushbyte 3
      &pushregister 1
      &pushsdbgm 69							//'m_mainMenuMC'
      &pushsdw 258							//'menuItemShowAward'
      &callmp
     label62:
      &jmp label68      
     label63:
      &pushregister 3
      &pushsdw 259							//'SCENARIO_COMPLETION_STATE_ALL_COMPLETE'
      &getMember
      &pushsdb 76							//'1'
      &equals
      &not
      &jnz label66      
      &pushregister 3
      &pushsdw 260							//'SCENARIO_COMPLETION_STATE_ALL_COMPLETE_FIRSTTIME'
      &getMember
      &pushsdb 76							//'1'
      &equals
      &not
      &jnz label64      
      &pushsdw 261							//'GuiAccordionButtonStar2'
      &pushone
      &pushregister 2
      &pushsdw 257							//'playSound'
      &callmp
      &pushtrue
      &pushbyte 2
      &pushbyte 2
      &pushbyte 3
      &pushregister 1
      &pushsdbgm 69							//'m_mainMenuMC'
      &pushsdw 258							//'menuItemShowAward'
      &callmp
      &jmp label65      
     label64:
      &pushfalse
      &pushbyte 2
      &pushbyte 2
      &pushbyte 3
      &pushregister 1
      &pushsdbgm 69							//'m_mainMenuMC'
      &pushsdw 258							//'menuItemShowAward'
      &callmp
     label65:
      &jmp label68      
     label66:
      &pushregister 3
      &pushsdw 262							//'SCENARIO_COMPLETION_STATE_CRITICAL_COMPLETE'
      &getMember
      &pushsdb 76							//'1'
      &equals
      &not
      &jnz label68      
      &pushregister 3
      &pushsdw 263							//'SCENARIO_COMPLETION_STATE_CRITICAL_COMPLETE_FIRSTTIME'
      &getMember
      &pushsdb 76							//'1'
      &equals
      &not
      &jnz label67      
      &pushsdw 264							//'GuiAccordionButtonStar1'
      &pushone
      &pushregister 2
      &pushsdw 257							//'playSound'
      &callmp
      &pushtrue
      &pushone
      &pushbyte 2
      &pushbyte 3
      &pushregister 1
      &pushsdbgm 69							//'m_mainMenuMC'
      &pushsdw 258							//'menuItemShowAward'
      &callmp
      &jmp label68      
     label67:
      &pushfalse
      &pushone
      &pushbyte 2
      &pushbyte 3
      &pushregister 1
      &pushsdbgm 69							//'m_mainMenuMC'
      &pushsdw 258							//'menuItemShowAward'
      &callmp
     label68:
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 211							//'showCampaignAward'
    &function2  () (r:1='this', r:2='_global')
      &pushzero
      &pushsdb 22							//'Object'
      &new
      &setRegister r:3
      &pop
      &pushzero
      &pushsdb 22							//'Object'
      &new
      &setRegister r:8
      &pop
      &pushsdw 265							//'QueryGameEngine?IS_CAMPAIGN_COMPLETE?FactionIndex='
      &pushregister 2
      &pushsdbgm 6							//'ENUM'
      &pushsdw 266							//'CAMPAIGN_TYPE_SOVIET'
      &getMember
      &add
      &pushregister 3
      &loadVariables
      &pushregister 3
      &pushsdw 267							//'IS_CAMPAIGN_COMPLETE'
      &getMember
      &pushsdb 76							//'1'
      &equals
      &setRegister r:4
      &pop
      &pushsdw 265							//'QueryGameEngine?IS_CAMPAIGN_COMPLETE?FactionIndex='
      &pushregister 2
      &pushsdbgm 6							//'ENUM'
      &pushsdw 268							//'CAMPAIGN_TYPE_ALLIES'
      &getMember
      &add
      &pushregister 3
      &loadVariables
      &pushregister 3
      &pushsdw 267							//'IS_CAMPAIGN_COMPLETE'
      &getMember
      &pushsdb 76							//'1'
      &equals
      &setRegister r:5
      &pop
      &pushsdw 265							//'QueryGameEngine?IS_CAMPAIGN_COMPLETE?FactionIndex='
      &pushregister 2
      &pushsdbgm 6							//'ENUM'
      &pushsdw 269							//'CAMPAIGN_TYPE_RISINGSUN'
      &getMember
      &add
      &pushregister 3
      &loadVariables
      &pushregister 3
      &pushsdw 267							//'IS_CAMPAIGN_COMPLETE'
      &getMember
      &pushsdb 76							//'1'
      &equals
      &setRegister r:6
      &pop
      &pushsdw 265							//'QueryGameEngine?IS_CAMPAIGN_COMPLETE?FactionIndex='
      &pushregister 2
      &pushsdbgm 6							//'ENUM'
      &pushsdw 270							//'CAMPAIGN_TYPE_YURIKO'
      &getMember
      &add
      &pushregister 3
      &loadVariables
      &pushregister 3
      &pushsdw 267							//'IS_CAMPAIGN_COMPLETE'
      &getMember
      &pushsdb 76							//'1'
      &equals
      &setRegister r:7
      &pop
      &pushsdw 271							//'QueryGameEngine?JUST_COMPLETED_CAMPAIGN'
      &pushregister 3
      &loadVariables
      &getURL 'FSCommand:CallGameFunction'       '%Campaign_ClearJustCompleted'      
      &pushregister 4
      &dup
      &not
      &jnz label69      
      &pop
      &pushregister 5
     label69:
      &dup
      &not
      &jnz label70      
      &pop
      &pushregister 6
     label70:
      &dup
      &not
      &jnz label71      
      &pop
      &pushregister 7
     label71:
      &not
      &jnz label72      
      &pushregister 3
      &pushsdw 272							//'JUST_COMPLETED_CAMPAIGN'
      &getMember
      &pushsdw 273							//'-1'
      &equals
      &not
      &pushone
      &pushone
      &pushbyte 3
      &pushregister 1
      &pushsdbgm 69							//'m_mainMenuMC'
      &pushsdw 258							//'menuItemShowAward'
      &callmp
     label72:
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 212							//'showTutorialAward'
    &function2  () (r:1='this')
      &pushzero
      &pushsdb 22							//'Object'
      &new
      &setRegister r:5
      &pop
      &pushsdw 274							//'QueryGameEngine?TUTORIAL_IS_COMPLETED'
      &pushregister 5
      &loadVariables
      &pushsdw 275							//','
      &pushone
      &pushregister 5
      &pushsdw 276							//'TUTORIAL_IS_COMPLETED'
      &getMember
      &pushsdw 277							//'split'
      &callMethod
      &setRegister r:3
      &pop
      &pushtrue
      &setRegister r:4
      &pop
      &pushzero
      &setRegister r:2
      &pop
     label73:
      &pushregister 2
      &pushregister 3
      &pushsdw 278							//'length'
      &getMember
      &lessThan
      &not
      &jnz label75      
      &pushregister 3
      &pushregister 2
      &getMember
      &pushsdb 76							//'1'
      &equals
      &not
      &not
      &jnz label74      
      &pushfalse
      &setRegister r:4
      &pop
      &jmp label75      
     label74:
      &pushregister 2
      &increment
      &setRegister r:2
      &pop
      &jmp label73      
     label75:
      &pushregister 4
      &not
      &jnz label76      
      &pushfalse
      &pushone
      &pushzero
      &pushbyte 3
      &pushregister 1
      &pushsdbgm 69							//'m_mainMenuMC'
      &pushsdw 258							//'menuItemShowAward'
      &callmp
     label76:
    &end // of function 

    &setMember
    &pushregister 1
    &pushsdb 47							//'UI_X_ORIGIN'
    &pushshort 300
    &setMember
    &pushregister 1
    &pushsdb 45							//'UI_Y_ORIGIN'
    &pushbyte 100
    &setMember
    &pushregister 1
    &pushsdb 46							//'UI_POS_PADDING'
    &pushbyte 2
    &setMember
    &pushregister 1
    &pushsdb 55							//'WALL_X'
    &pushshort 1450
    &setMember
    &pushregister 1
    &pushsdb 56							//'WALL_Y'
    &pushshort 1040
    &setMember
    &pushregister 1
    &pushsdb 58							//'WALL_Z'
    &pushbyte 60
    &setMember
    &pushregister 1
    &pushsdb 53							//'WALL_XROT'
    &pushzero
    &setMember
    &pushregister 1
    &pushsdb 54							//'WALL_YROT'
    &pushshort 328
    &setMember
    &pushregister 1
    &pushsdb 83							//'MENU_X'
    &pushshort 280
    &setMember
    &pushregister 1
    &pushsdb 84							//'MENU_Y'
    &pushshort 160
    &setMember
    &pushregister 1
    &pushsdb 85							//'MENU_Z'
    &pushzero
    &setMember
    &pushregister 1
    &pushsdb 81							//'MENU_XROT'
    &pushzero
    &setMember
    &pushregister 1
    &pushsdb 82							//'MENU_YROT'
    &pushshort 328
    &setMember
    &pushregister 1
    &pushsdb 249							//'SLIDE_DISTANCE_X'
    &pushshort 1900
    &setMember
    &pushregister 1
    &pushsdb 250							//'SLIDE_DISTANCE_Z'
    &pushsdbgv 1							//'feg_m_mainMenu3D'
    &pushsdbgm 249							//'SLIDE_DISTANCE_X'
    &pushfloat 0.661006033420563
    &multiply
    &setMember
    &pushregister 1
    &pushsdb 92							//'NLC_X'
    &pushsdbgv 1							//'feg_m_mainMenu3D'
    &pushsdbgm 83							//'MENU_X'
    &pushsdbgv 1							//'feg_m_mainMenu3D'
    &pushsdbgm 249							//'SLIDE_DISTANCE_X'
    &add
    &setMember
    &pushregister 1
    &pushsdb 93							//'NLC_Y'
    &pushsdbgv 1							//'feg_m_mainMenu3D'
    &pushsdbgm 84							//'MENU_Y'
    &setMember
    &pushregister 1
    &pushsdb 94							//'NLC_Z'
    &pushsdbgv 1							//'feg_m_mainMenu3D'
    &pushsdbgm 85							//'MENU_Z'
    &pushsdbgv 1							//'feg_m_mainMenu3D'
    &pushsdbgm 250							//'SLIDE_DISTANCE_Z'
    &add
    &setMember
    &pushregister 1
    &pushsdb 90							//'NLC_XROT'
    &pushsdbgv 1							//'feg_m_mainMenu3D'
    &pushsdbgm 81							//'MENU_XROT'
    &setMember
    &pushregister 1
    &pushsdb 91							//'NLC_YROT'
    &pushsdbgv 1							//'feg_m_mainMenu3D'
    &pushsdbgm 82							//'MENU_YROT'
    &setMember
    &pushregister 1
    &pushsdb 247							//'NLC_ANIM_LENGTH'
    &pushbyte 20
    &setMember
    &pushregister 1
    &pushsdb 38							//'FLOOR_X'
    &pushshort 900
    &setMember
    &pushregister 1
    &pushsdb 40							//'FLOOR_Y'
    &pushshort 610
    &setMember
    &pushregister 1
    &pushsdw 279							//'FLOOR_Z'
    &pushbyte 30
    &setMember
    &pushregister 1
    &pushsdb 34							//'FLOOR_XROT'
    &pushbyte 90
    &setMember
    &pushregister 1
    &pushsdb 36							//'FLOOR_YROT'
    &pushshort 358
    &setMember
    &pushregister 1
    &pushsdb 175							//'HAMMER_X'
    &pushshort 1050
    &setMember
    &pushregister 1
    &pushsdb 176							//'HAMMER_Y'
    &pushshort 570
    &setMember
    &pushregister 1
    &pushsdb 177							//'HAMMER_Z'
    &pushshort 420
    &setMember
    &pushregister 1
    &pushsdb 173							//'HAMMER_XROT'
    &pushshort 352
    &setMember
    &pushregister 1
    &pushsdb 174							//'HAMMER_YROT'
    &pushbyte 37
    &setMember
    &pushregister 1
    &pushsdb 152							//'PERSONALITY_IMAGE_X'
    &pushshort 600
    &setMember
    &pushregister 1
    &pushsdb 153							//'PERSONALITY_IMAGE_Y'
    &pushbyte 90
    &setMember
    &pushregister 1
    &pushsdb 154							//'PERSONALITY_IMAGE_Z'
    &pushshort -230
    &setMember
    &pushregister 1
    &pushsdb 150							//'PERSONALITY_IMAGE_XROT'
    &pushzero
    &setMember
    &pushregister 1
    &pushsdb 151							//'PERSONALITY_IMAGE_YROT'
    &pushbyte 23
    &setMember
    &pushregister 1
    &pushsdb 164							//'CREDIT_X'
    &pushshort 350
    &setMember
    &pushregister 1
    &pushsdb 165							//'CREDIT_Y'
    &pushshort 200
    &setMember
    &pushregister 1
    &pushsdb 162							//'CREDIT_XROT'
    &pushzero
    &setMember
    &pushregister 1
    &pushsdb 163							//'CREDIT_YROT'
    &pushzero
    &setMember
    &pushregister 1
    &pushsdb 185							//'UI_ADJUSTMENT_LAYER'
    &pushshort 150
    &setMember
    &pushregister 1
    &pushsdb 191							//'UI_IMAGEGUIDE_LAYER'
    &pushshort 200
    &setMember
    &pushregister 1
    &pushsdb 67							//'WALL_UI_NAME'
    &pushsdw 280							//'Wall Adjustment UI'
    &setMember
    &pushregister 1
    &pushsdb 48							//'FLOOR_UI_NAME'
    &pushsdw 281							//'Floor Adjustment UI'
    &setMember
    &pushregister 1
    &pushsdb 182							//'HAMMER_UI_NAME'
    &pushsdw 282							//'Hammer Adjustment UI'
    &setMember
    &pushregister 1
    &pushsdb 146							//'MENU_UI_NAME'
    &pushsdw 283							//'Menu Adjustment UI'
    &setMember
    &pushregister 1
    &pushsdb 159							//'PERSONALITY_UI_NAME'
    &pushsdw 284							//'Personality Adjustment UI'
    &setMember
    &pushregister 1
    &pushsdb 95							//'MENU_ITEM_1'
    &pushzero
    &setMember
    &pushregister 1
    &pushsdb 78							//'MENU_ITEM_2'
    &pushone
    &setMember
    &pushregister 1
    &pushsdb 96							//'MENU_ITEM_3'
    &pushbyte 2
    &setMember
    &pushregister 1
    &pushsdb 97							//'MENU_ITEM_4'
    &pushbyte 3
    &setMember
    &pushregister 1
    &pushsdb 98							//'MENU_ITEM_5'
    &pushbyte 4
    &setMember
    &pushregister 1
    &pushsdb 99							//'MENU_ITEM_6'
    &pushbyte 5
    &setMember
    &pushregister 1
    &pushsdb 100							//'MENU_ITEM_7'
    &pushbyte 6
    &setMember
    &pushregister 2
    &pushsdb 229							//'m_testUIVisible'
    &pushfalse
    &setMember
    &pushregister 1
    &pushsdb 243							//'m_frameCount'
    &pushzero
    &setMember
    &pushregister 2
    &pushsdb 222							//'m_isCreditsListScrolling'
    &pushfalse
    &setMember
    &pushone
    &pushnull
    &pushglobalgv
    &pushsdbgm 1							//'feg_m_mainMenu3D'
    &pushsdbgm 20							//'prototype'
    &pushbyte 3
    &pushsdw 285							//'ASSetPropFlags'
    &callFunction
   label77:
    &pop
  &end // of initMovieClip 18
&end
