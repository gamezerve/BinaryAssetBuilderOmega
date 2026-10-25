movie 'E:\depot\CnC\RA3_EP1\PRODUC~1\Data\APTBUI~1\034A3~1.0-D\pc\Output\FED422~1\\FES_M_~1.eaf' &compressed // flash 7, total frames: 1, frame rate: 30 fps, 1365x768 px

  &frame 0
    &pushs 'screen'
    &pushthisgv
    &pushone
    &pushs 'fes_m_skirmishGameSetup'
    &new
    &varEquals
    &stop
  &end // of frame 0
  
  &importAssets &from './screens/fe_m_assets.swf'
    'Background_mc' &as 1
  &end // of importAssets
  
  &importAssets &from '.\\Components\\fe_m_generalScreenComponents.swf'
    'lowerScreenBar Symbol' &as 2
  &end // of importAssets
  
  &importAssets &from '.\\Components\\fe_m_generalScreenComponents.swf'
    'ScreenTitleBarSymbol' &as 3
  &end // of importAssets

  &placeMovieClip 3 
  
    &onClipEvent &construct
      &pushs 'm_title'
      &pushssv '$SKIRMISH_LOBBY_SCREEN_TITLE'
      &pushs 'm_DropShadow'
      &pushtrue
      &setVariable
      &pushs 'm_Outline'
      &pushtrue
      &setVariable
      &pushs 'm_visible'
      &pushtrue
      &setVariable
    &end
  &end // of placeMovieClip 3

  &defineMovieClip 6 // total frames: 1
  &end // of defineMovieClip 6
  
  &importAssets &from 'screens/fe_m_assetsGameSetup.swf'
    'gameSettings_skirmish' &as 7
  &end // of importAssets
  
  &importAssets &from '../cafe/mouseComponents/std_MouseButton.swf'
    'std_MousebuttonSymbol' &as 16
  &end // of importAssets

  &placeMovieClip 16 &as 'backButton'
  
    &onClipEvent &construct
      &pushs 'm_width'
      &pushshort 200
      &setVariable
      &pushs 'm_textAlign'
      &pushfalse
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
      &pushbyte -1
      &setVariable
      &pushs 'm_bVisible'
      &pushtrue
      &setVariable
      &pushs 'm_contentSymbol'
      &pushssv 'buttonContentSymbol'
    &end
  &end // of placeMovieClip 16

  &placeMovieClip 16 &as 'playButton'
  
    &onClipEvent &construct
      &pushs 'm_width'
      &pushshort 200
      &setVariable
      &pushs 'm_textAlign'
      &pushfalse
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
      &pushssv '$StartGame'
      &pushs 'm_bEnabled'
      &pushtrue
      &setVariable
      &pushs 'm_refFM'
      &pushssv '_root.gFM'
      &pushs 'm_initiallySelected'
      &pushfalse
      &setVariable
      &pushs 'm_tabIndex'
      &pushbyte -1
      &setVariable
      &pushs 'm_bVisible'
      &pushtrue
      &setVariable
      &pushs 'm_contentSymbol'
      &pushssv 'buttonContentSymbol'
    &end
  &end // of placeMovieClip 16
  
  &importAssets &from '../cafe/mouseComponents/std_MouseButton.swf'
    'std_MousebuttonSymbol' &as 17
  &end // of importAssets

  &placeMovieClip 17 &as 'loadButton'
  
    &onClipEvent &construct
      &pushs 'm_focusDirs'
      &pushssv 'Up/Down'
      &pushs 'm_bEnabled'
      &pushtrue
      &setVariable
      &pushs 'm_refFM'
      &pushssv '_root.gFM'
      &pushs 'm_initiallySelected'
      &pushfalse
      &setVariable
      &pushs 'm_tabIndex'
      &pushbyte -1
      &setVariable
      &pushs 'm_bVisible'
      &pushtrue
      &setVariable
      &pushs 'm_label'
      &pushssv '$LOAD'
      &pushs 'm_contentSymbol'
      &pushssv 'buttonContentSymbol'
    &end
  &end // of placeMovieClip 17
  
  &importAssets &from '../cafe/mouseComponents/std_mouseDropDownMenu.swf'
    'std_mouseDropDownMenuSymbol' &as 26
  &end // of importAssets

  &defineMovieClip 27 // total frames: 1

    &frame 0
      &stop
    &end // of frame 0

    &placeMovieClip 26 &as 'colorOptions'
    
      &onClipEvent &construct
        &pushs 'm_focusDirs'
        &pushssv 'Up/Down'
        &pushs 'm_refFM'
        &pushssv '_root.gFM'
        &pushs 'm_initiallySelected'
        &pushfalse
        &setVariable
        &pushs 'm_tabIndex'
        &pushbyte -1
        &setVariable
        &pushs 'm_bVisible'
        &pushtrue
        &setVariable
        &pushs 'm_visibleEntryCount'
        &pushbyte 5
        &setVariable
        &pushs 'm_width'
        &pushbyte 99
        &setVariable
        &pushs 'm_contentSymbol'
        &pushssv 'ColorSwatchDropdownContentSymbol'
        &pushs 'm_entrySymbol'
        &pushssv 'std_mouseListboxEntryColorSwatchElementSymbol'
        &pushs 'm_ButtonWidth'
        &pushbyte 27
        &setVariable
      &end
    &end // of placeMovieClip 26

    &placeMovieClip 26 &as 'teamOptions'
    
      &onClipEvent &construct
        &pushs 'm_focusDirs'
        &pushssv 'Up/Down'
        &pushs 'm_refFM'
        &pushssv '_root.gFM'
        &pushs 'm_initiallySelected'
        &pushfalse
        &setVariable
        &pushs 'm_tabIndex'
        &pushbyte -1
        &setVariable
        &pushs 'm_bVisible'
        &pushtrue
        &setVariable
        &pushs 'm_visibleEntryCount'
        &pushbyte 4
        &setVariable
        &pushs 'm_width'
        &pushbyte 99
        &setVariable
        &pushs 'm_contentSymbol'
        &pushssv 'TextDropdownMenuContentSymbol'
        &pushs 'm_entrySymbol'
        &pushssv 'std_mouseListboxEntryTextElementSymbol'
        &pushs 'm_ButtonWidth'
        &pushbyte 27
        &setVariable
      &end
    &end // of placeMovieClip 26

    &placeMovieClip 26 &as 'personalityOptions'
    
      &onClipEvent &construct
        &pushs 'm_focusDirs'
        &pushssv 'Up/Down'
        &pushs 'm_refFM'
        &pushssv '_root.gFM'
        &pushs 'm_initiallySelected'
        &pushfalse
        &setVariable
        &pushs 'm_tabIndex'
        &pushbyte -1
        &setVariable
        &pushs 'm_bVisible'
        &pushtrue
        &setVariable
        &pushs 'm_visibleEntryCount'
        &pushbyte 4
        &setVariable
        &pushs 'm_width'
        &pushshort 209
        &setVariable
        &pushs 'm_contentSymbol'
        &pushssv 'TextDropdownMenuContentSymbol'
        &pushs 'm_entrySymbol'
        &pushssv 'std_mouseListboxEntryTextElementSymbol'
        &pushs 'm_ButtonWidth'
        &pushbyte 27
        &setVariable
      &end
    &end // of placeMovieClip 26

    &placeMovieClip 26 &as 'factionOptions'
    
      &onClipEvent &construct
        &pushs 'm_focusDirs'
        &pushssv 'Up/Down'
        &pushs 'm_refFM'
        &pushssv '_root.gFM'
        &pushs 'm_initiallySelected'
        &pushfalse
        &setVariable
        &pushs 'm_tabIndex'
        &pushbyte -1
        &setVariable
        &pushs 'm_bVisible'
        &pushtrue
        &setVariable
        &pushs 'm_visibleEntryCount'
        &pushbyte 5
        &setVariable
        &pushs 'm_width'
        &pushbyte 120
        &setVariable
        &pushs 'm_contentSymbol'
        &pushssv 'RenderImageDropdownContentSymbol'
        &pushs 'm_entrySymbol'
        &pushssv 'std_mouseListboxEntryRenderImageElementSymbol'
        &pushs 'm_ButtonWidth'
        &pushbyte 27
        &setVariable
        &pushs 'm_contentClipWidth'
        &pushbyte 32
        &setVariable
        &pushs 'm_contentClipHeight'
        &pushbyte 19
        &setVariable
      &end
    &end // of placeMovieClip 26

    &placeMovieClip 26 &as 'playerSlotOptions'
    
      &onClipEvent &construct
        &pushs 'm_focusDirs'
        &pushssv 'Up/Down'
        &pushs 'm_refFM'
        &pushssv '_root.gFM'
        &pushs 'm_initiallySelected'
        &pushfalse
        &setVariable
        &pushs 'm_tabIndex'
        &pushbyte -1
        &setVariable
        &pushs 'm_bVisible'
        &pushtrue
        &setVariable
        &pushs 'm_visibleEntryCount'
        &pushbyte 4
        &setVariable
        &pushs 'm_width'
        &pushshort 312
        &setVariable
        &pushs 'm_contentSymbol'
        &pushssv 'TextDropdownMenuContentSymbol'
        &pushs 'm_entrySymbol'
        &pushssv 'std_mouseListboxEntryTextElementSymbol'
        &pushs 'm_ButtonWidth'
        &pushbyte 27
        &setVariable
      &end
    &end // of placeMovieClip 26
  &end // of defineMovieClip 27

  &defineMovieClip 28 // total frames: 0
  &end // of defineMovieClip 28
  
  &exportAssets
    28 &as '__Packages.Cafe2_BaseUIScreen'
  &end // of exportAssets
  
  &initMovieClip 28
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
  &end // of initMovieClip 28

  &defineMovieClip 29 // total frames: 0
  &end // of defineMovieClip 29
  
  &exportAssets
    29 &as '__Packages.IListboxEventListener'
  &end // of exportAssets
  
  &initMovieClip 29
    &constants '_global', 'IListboxEventListener'  
    &pushglobalgv
    &pushsdbgm 1							//'IListboxEventListener'
    &not
    &not
    &jnz label1    
    &pushglobalgv
    &pushsdb 1							//'IListboxEventListener'
    &function  (    )    
    &end // of function 

    &setMember
   label1:
    &pop
  &end // of initMovieClip 29

  &defineMovieClip 30 // total frames: 0
  &end // of defineMovieClip 30
  
  &exportAssets
    30 &as '__Packages.GameSetupBase'
  &end // of exportAssets
  
  &initMovieClip 30
    &constants '_global', 'GameSetupBase', 'Cafe2_BaseUIScreen', 'm_thisClass', 'm_localMessageDispatcher', 'localMessageHandler', 'bind1DynamicParams', 'm_mapIds', 'Array', 'm_screenButtons', 'm_mapPreviewMC', 'm_screen', 'gameSettings', 'mapPanel', 'mapPreview', 'm_originalMapX', '_x', 'm_originalMapY', '_y', 'm_originalMapWidth', '_width', 'm_originalMapHeight', '_height', 'onScreenExit', 'bind0', 'gSM', 'setOnExitScreen', 'prototype', 'IListboxEventListener', 'frameDelayedInit', 'Object', 'QueryGameEngine?IsPcGameHost', 'm_isHost', 'IsPcGameHost', '1', 'initGUI', 'gMH', 'addMessageHandler', 'OnListboxIndexMouseClick', 'OnMapListSelectionChanged', 'OnListboxIndexElementMouseClick', 'initNavigationButtons', 'initMapPanel', 'initRulesPanel', 'initPlayerSlots', 'playButton', 'show', 'enable', 'playGame', 'setOnMouseUpFunction', 'push', 'backButton', 'backButtonClicked', 'm_playerStartPosMCArray', 'startPosition5', 'startPosition4', 'startPosition3', 'startPosition2', 'startPosition1', 'startPosition0', 'length', 'hide', 'disable', 'onStartPositionClicked', 'bind1', 'campaignCheckbox', 'onMapTypeChanged', 'setOnMouseDownFunction', 'createMapSelectionScrollList', 'rulesPanel', 'extern', 'RA3PublicBeta', '_visible', 'gameSpeedSlider', 'resourcesDropdown', 'cratesCheckbox', 'commentaryCheckbox', 'enableVoipCheckbox', 'setBroadcastCheckboxVisibility', 'onGameSpeedChanged', 'bind2DynamicParams', 'setOnChange', 'onResourcesChanged', 'onCratesChanged', 'onCommentaryChanged', 'onVoipChanged', 'defaultRules', 'onResetRules', 'm_playerSlots', 'slotMC_6', 'slotMC_5', 'slotMC_4', 'slotMC_3', 'slotMC_2', 'slotMC_1', 'slotId', 'playerSlotOptions', 'onPlayerStatusChanged', 'factionOptions', 'onFactionChanged', 'personalityOptions', 'onAIPersonalityChanged', 'teamOptions', 'onTeamChanged', 'colorOptions', 'onColorChanged', 'initReadyControls', 'readyCheckBox', 'onReadyStatusChanged', 'initPlayButton', 'broadcastCheckbox', 'broadcastCheckboxLabel', 'broadcastCheckboxShadowLabel', 'mapListScrollbar', 'mapList', 'addListener', 'symbol', 'std_mouseListboxEntryTextElementSymbol', 'width', 'leftMargin', 'rightMargin', 'init', 'attachScrollbarController', 'moveViewToFirst', 'refreshDisplay', 'refreshScrollbar', 'MSGCODE', 'FE_MP_UPDATE_GAME_SETTINGS', 'refreshComponents', 'removeMessageHandler', 'removeListener', 'shutdown', 'clearData', 'isChecked', '0', 'FSCommand:CallGameFunction', '%SetMapType?MapType=', 'GAME', 'MODE', 'ENUM', 'GAME_MODE_CAMPAIGN', 'GAME_MODE_SKIRMISH', '%SetMap?Map=', 'onShowMapPanel', 'onShowRulesPanel', 'getCurrentIndex', 'getValueAtIndex', '_parent', '%SetPlayerStatus?Slot=', '|Status=', '%SetPlayerTemplate?Slot=', '|Template=', '%SetTeam?Slot=', '|Team=', '%SetColor?Slot=', '|Color=', '%SetAIPersonality?Slot=', '|Personality=', '%CycleStartPositionOwner?Position=', '%SetGameSpeed?GameSpeed=', '%SetInitialResources?Resources=', 'refreshMapList', 'refreshSlotStatusControls', 'refreshCampaignMapControls', 'refreshResourcesControl', 'RANDOM_CRATES', 'refreshRulesCheckbox', 'ENABLE_VOIP', 'ALLOW_COMMENTARY', 'refreshGameSpeedControls', 'refreshTeamControls', 'refreshFactionControls', 'refreshAIPersonalityControls', 'setupMapRenderClip', 'refreshStartPositionControls', 'refreshColorControls', 'QueryGameEngine?MP_MAP_LIST', ',', 'MP_MAP_LIST', 'split', 'shift', '$MP_MAP_LIST_', '$MP_MAP_SIZE_', 'setColumnData', 'QueryGameEngine?PLAYER_STATUS?Slot=', 'PLAYER_STATUS', '$PLAYER_STATUS_', '_', 'enableControl', 'setData', 'setSelectedIndex', 'QueryGameEngine?TEAM_OPTIONS?Slot=', 'TEAM_OPTIONS_VALUES', '$TEAM_', 'TEAM_OPTIONS_ENABLE', 'QueryGameEngine?PLAYER_TEMPLATE_OPTIONS?Slot=', 'PLAYER_TEMPLATE_OPTIONS_VALUES', 'PLAYER_TEMPLATE_OPTIONS_IMAGES', 'QueryGameEngine?AI_PERSONALITY_OPTIONS?Slot=', 'AI_PERSONALITY_OPTIONS', 'clear', '$APT:AI_PERSONALITY_', 'QueryGameEngine?PLAYER_COLOR?Slot=', 'PLAYER_COLOR_VALUES', 'PLAYER_COLOR_ENABLE', 'QueryGameEngine?MAP_ASPECT_RATIO', 'MAP_ASPECT_RATIO', 'QueryGameEngine?START_POSITION?Position=', 'START_POSITION_VALID', 'Boolean', '$START_POSITION_', 'setText', 'START_POSITION_X', 'START_POSITION_Y', 'QueryGameEngine?PLAY_AS_CAMPAIGN', 'PLAY_AS_CAMPAIGN_VALUE', 'unCheck', 'check', 'PLAY_AS_CAMPAIGN_ENABLE', 'QueryGameEngine?RESOURCES_OPTIONS', 'RESOURCES_OPTIONS_VALUES', 'RESOURCES_OPTIONS_STRINGS', 'QueryGameEngine?', '_VALUE', '_ENABLE', 'QueryGameEngine?GAMESPEED_OPTIONS', 'GAMESPEED_OPTIONS_CURRENT', 'GAMESPEED_OPTIONS_MAX', 'GAMESPEED_OPTIONS_MIN', 'setIndexRange', 'GAMESPEED_OPTIONS_ENABLE', 'refreshReadyControls', 'refreshReadyCheckbox', 'QueryGameEngine?READY_STATUS?Slot=', 'READY_STATUS_ENABLED', 'READY_STATUS_VALUE', 'SetUnCheck', 'SetCheck', 'ASSetPropFlags'  
    &pushglobalgv
    &pushsdbgm 1							//'GameSetupBase'
    &not
    &not
    &jnz label71    
    &pushglobalgv
    &pushsdb 1							//'GameSetupBase'
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
      &pushregister 1
      &pushsdb 4							//'m_localMessageDispatcher'
      &pushregister 1
      &pushsdbgm 5							//'localMessageHandler'
      &pushregister 1
      &pushbyte 2
      &pushregister 3
      &pushsdb 6							//'bind1DynamicParams'
      &callMethod
      &setMember
      &pushregister 1
      &pushsdb 7							//'m_mapIds'
      &pushzero
      &pushsdb 8							//'Array'
      &new
      &setMember
      &pushregister 1
      &pushsdb 9							//'m_screenButtons'
      &pushzero
      &pushsdb 8							//'Array'
      &new
      &setMember
      &pushregister 1
      &pushsdb 10							//'m_mapPreviewMC'
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 11							//'m_screen'
      &pushsdbgm 12							//'gameSettings'
      &pushsdbgm 13							//'mapPanel'
      &pushsdbgm 14							//'mapPreview'
      &setMember
      &pushregister 1
      &pushsdb 15							//'m_originalMapX'
      &pushregister 1
      &pushsdbgm 10							//'m_mapPreviewMC'
      &pushsdbgm 16							//'_x'
      &setMember
      &pushregister 1
      &pushsdb 17							//'m_originalMapY'
      &pushregister 1
      &pushsdbgm 10							//'m_mapPreviewMC'
      &pushsdbgm 18							//'_y'
      &setMember
      &pushregister 1
      &pushsdb 19							//'m_originalMapWidth'
      &pushregister 1
      &pushsdbgm 10							//'m_mapPreviewMC'
      &pushsdbgm 20							//'_width'
      &setMember
      &pushregister 1
      &pushsdb 21							//'m_originalMapHeight'
      &pushregister 1
      &pushsdbgm 10							//'m_mapPreviewMC'
      &pushsdbgm 22							//'_height'
      &setMember
      &pushregister 1
      &pushsdbgm 23							//'onScreenExit'
      &pushregister 1
      &pushbyte 2
      &pushregister 3
      &pushsdb 24							//'bind0'
      &callMethod
      &pushone
      &pushregister 3
      &pushsdbgm 25							//'gSM'
      &dcallmp 26							// setOnExitScreen()
    &end // of function 

    &setRegister r:1
    &setMember
    &pushglobalgv
    &pushsdbgm 1							//'GameSetupBase'
    &pushsdbgv 2							//'Cafe2_BaseUIScreen'
    &extends
    &pushregister 1
    &pushsdbgm 27							//'prototype'
    &setRegister r:2
    &pop
    &pushglobalgv
    &pushsdbgm 28							//'IListboxEventListener'
    &pushone
    &pushglobalgv
    &pushsdbgm 1							//'GameSetupBase'
    &implements
    &pushregister 2
    &pushsdb 29							//'frameDelayedInit'
    &function2  () (r:1='this', r:2='_global')
      &pushzero
      &pushsdb 30							//'Object'
      &new
      &setRegister r:3
      &pop
      &pushsdb 31							//'QueryGameEngine?IsPcGameHost'
      &pushregister 3
      &loadVariables
      &pushregister 1
      &pushsdb 32							//'m_isHost'
      &pushregister 3
      &pushsdbgm 33							//'IsPcGameHost'
      &pushsdb 34							//'1'
      &equals
      &setMember
      &pushzero
      &pushregister 1
      &dcallmp 35							// initGUI()
      &pushregister 1
      &pushsdbgm 4							//'m_localMessageDispatcher'
      &pushone
      &pushregister 2
      &pushsdbgm 36							//'gMH'
      &dcallmp 37							// addMessageHandler()
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 38							//'OnListboxIndexMouseClick'
    &function2  (r:2='index', 'previouslySelectedIndex') (r:1='this')
      &pushregister 2
      &pushone
      &pushregister 1
      &dcallmp 39							// OnMapListSelectionChanged()
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 40							//'OnListboxIndexElementMouseClick'
    &function2  (r:2='index', 'element', 'previouslySelectedIndex') (r:1='this')
      &pushregister 2
      &pushone
      &pushregister 1
      &dcallmp 39							// OnMapListSelectionChanged()
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 35							//'initGUI'
    &function2  () (r:1='this')
      &pushregister 1
      &pushsdbgm 9							//'m_screenButtons'
      &pushone
      &pushregister 1
      &dcallmp 41							// initNavigationButtons()
      &pushzero
      &pushregister 1
      &dcallmp 42							// initMapPanel()
      &pushregister 1
      &pushsdbgm 9							//'m_screenButtons'
      &pushone
      &pushregister 1
      &dcallmp 43							// initRulesPanel()
      &pushzero
      &pushregister 1
      &dcallmp 44							// initPlayerSlots()
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 41							//'initNavigationButtons'
    &function2  (r:3='navButtons') (r:1='this', r:2='_global')
      &pushregister 1
      &pushsdbgm 32							//'m_isHost'
      &not
      &jnz label1      
      &pushzero
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 11							//'m_screen'
      &pushsdbgm 45							//'playButton'
      &dcallmp 46							// show()
      &pushzero
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 11							//'m_screen'
      &pushsdbgm 45							//'playButton'
      &dcallmp 47							// enable()
      &pushregister 1
      &pushsdbgm 48							//'playGame'
      &pushregister 1
      &pushbyte 2
      &pushregister 2
      &pushsdb 24							//'bind0'
      &callMethod
      &pushone
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 11							//'m_screen'
      &pushsdbgm 45							//'playButton'
      &dcallmp 49							// setOnMouseUpFunction()
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 11							//'m_screen'
      &pushsdbgm 45							//'playButton'
      &pushone
      &pushregister 3
      &dcallmp 50							// push()
     label1:
      &pushzero
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 11							//'m_screen'
      &pushsdbgm 51							//'backButton'
      &dcallmp 46							// show()
      &pushzero
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 11							//'m_screen'
      &pushsdbgm 51							//'backButton'
      &dcallmp 47							// enable()
      &pushregister 1
      &pushsdbgm 52							//'backButtonClicked'
      &pushregister 1
      &pushbyte 2
      &pushregister 2
      &pushsdb 24							//'bind0'
      &callMethod
      &pushone
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 11							//'m_screen'
      &pushsdbgm 51							//'backButton'
      &dcallmp 49							// setOnMouseUpFunction()
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 11							//'m_screen'
      &pushsdbgm 51							//'backButton'
      &pushone
      &pushregister 3
      &dcallmp 50							// push()
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 42							//'initMapPanel'
    &function2  () (r:1='this', r:2='_global')
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 11							//'m_screen'
      &pushsdbgm 12							//'gameSettings'
      &pushsdbgm 13							//'mapPanel'
      &setRegister r:4
      &pop
      &pushregister 1
      &pushsdb 53							//'m_playerStartPosMCArray'
      &pushregister 4
      &pushsdbgm 54							//'startPosition5'
      &pushregister 4
      &pushsdbgm 55							//'startPosition4'
      &pushregister 4
      &pushsdbgm 56							//'startPosition3'
      &pushregister 4
      &pushsdbgm 57							//'startPosition2'
      &pushregister 4
      &pushsdbgm 58							//'startPosition1'
      &pushregister 4
      &pushsdbgm 59							//'startPosition0'
      &pushbyte 6
      &initArray
      &setMember
      &pushzero
      &setRegister r:3
      &pop
     label2:
      &pushregister 3
      &pushregister 1
      &pushsdbgm 53							//'m_playerStartPosMCArray'
      &pushsdbgm 60							//'length'
      &lessThan
      &not
      &jnz label3      
      &pushzero
      &pushregister 1
      &pushsdbgm 53							//'m_playerStartPosMCArray'
      &pushregister 3
      &getMember
      &dcallmp 61							// hide()
      &pushzero
      &pushregister 1
      &pushsdbgm 53							//'m_playerStartPosMCArray'
      &pushregister 3
      &getMember
      &dcallmp 62							// disable()
      &pushregister 3
      &pushregister 1
      &pushsdbgm 63							//'onStartPositionClicked'
      &pushregister 1
      &pushbyte 3
      &pushregister 2
      &pushsdb 64							//'bind1'
      &callMethod
      &pushone
      &pushregister 1
      &pushsdbgm 53							//'m_playerStartPosMCArray'
      &pushregister 3
      &getMember
      &dcallmp 49							// setOnMouseUpFunction()
      &pushregister 3
      &increment
      &setRegister r:3
      &pop
      &jmp label2      
     label3:
      &pushzero
      &pushregister 4
      &pushsdbgm 65							//'campaignCheckbox'
      &dcallmp 46							// show()
      &pushzero
      &pushregister 4
      &pushsdbgm 65							//'campaignCheckbox'
      &dcallmp 47							// enable()
      &pushregister 1
      &pushsdbgm 66							//'onMapTypeChanged'
      &pushregister 1
      &pushbyte 2
      &pushregister 2
      &pushsdb 24							//'bind0'
      &callMethod
      &pushone
      &pushregister 4
      &pushsdbgm 65							//'campaignCheckbox'
      &dcallmp 67							// setOnMouseDownFunction()
      &pushzero
      &pushregister 1
      &dcallmp 68							// createMapSelectionScrollList()
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 43							//'initRulesPanel'
    &function2  (r:5='buttons') (r:1='this', r:2='_global')
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 11							//'m_screen'
      &pushsdbgm 12							//'gameSettings'
      &pushsdbgm 69							//'rulesPanel'
      &setRegister r:3
      &pop
      &pushsdbgv 70							//'extern'
      &pushsdbgm 71							//'RA3PublicBeta'
      &pushsdb 34							//'1'
      &equals
      &not
      &jnz label4      
      &pushregister 3
      &pushsdb 72							//'_visible'
      &pushfalse
      &setMember
      &jmp label6      
     label4:
      &pushzero
      &pushregister 3
      &pushsdbgm 73							//'gameSpeedSlider'
      &dcallmp 46							// show()
      &pushzero
      &pushregister 3
      &pushsdbgm 74							//'resourcesDropdown'
      &dcallmp 46							// show()
      &pushzero
      &pushregister 3
      &pushsdbgm 75							//'cratesCheckbox'
      &dcallmp 46							// show()
      &pushzero
      &pushregister 3
      &pushsdbgm 76							//'commentaryCheckbox'
      &dcallmp 46							// show()
      &pushzero
      &pushregister 3
      &pushsdbgm 77							//'enableVoipCheckbox'
      &dcallmp 46							// show()
      &pushfalse
      &pushone
      &pushregister 1
      &dcallmp 78							// setBroadcastCheckboxVisibility()
      &pushregister 1
      &pushsdbgm 32							//'m_isHost'
      &not
      &jnz label5      
      &pushzero
      &pushregister 3
      &pushsdbgm 73							//'gameSpeedSlider'
      &dcallmp 47							// enable()
      &pushzero
      &pushregister 3
      &pushsdbgm 74							//'resourcesDropdown'
      &dcallmp 47							// enable()
      &pushzero
      &pushregister 3
      &pushsdbgm 75							//'cratesCheckbox'
      &dcallmp 47							// enable()
      &pushzero
      &pushregister 3
      &pushsdbgm 76							//'commentaryCheckbox'
      &dcallmp 47							// enable()
      &pushzero
      &pushregister 3
      &pushsdbgm 77							//'enableVoipCheckbox'
      &dcallmp 47							// enable()
      &pushregister 1
      &pushsdbgm 79							//'onGameSpeedChanged'
      &pushregister 1
      &pushbyte 2
      &pushregister 2
      &pushsdb 80							//'bind2DynamicParams'
      &callMethod
      &pushone
      &pushregister 3
      &pushsdbgm 73							//'gameSpeedSlider'
      &dcallmp 81							// setOnChange()
      &pushregister 1
      &pushsdbgm 82							//'onResourcesChanged'
      &pushregister 1
      &pushbyte 2
      &pushregister 2
      &pushsdb 6							//'bind1DynamicParams'
      &callMethod
      &pushone
      &pushregister 3
      &pushsdbgm 74							//'resourcesDropdown'
      &dcallmp 81							// setOnChange()
      &pushregister 1
      &pushsdbgm 83							//'onCratesChanged'
      &pushregister 1
      &pushbyte 2
      &pushregister 2
      &pushsdb 24							//'bind0'
      &callMethod
      &pushone
      &pushregister 3
      &pushsdbgm 75							//'cratesCheckbox'
      &dcallmp 67							// setOnMouseDownFunction()
      &pushregister 1
      &pushsdbgm 84							//'onCommentaryChanged'
      &pushregister 1
      &pushbyte 2
      &pushregister 2
      &pushsdb 24							//'bind0'
      &callMethod
      &pushone
      &pushregister 3
      &pushsdbgm 76							//'commentaryCheckbox'
      &dcallmp 67							// setOnMouseDownFunction()
      &pushregister 1
      &pushsdbgm 85							//'onVoipChanged'
      &pushregister 1
      &pushbyte 2
      &pushregister 2
      &pushsdb 24							//'bind0'
      &callMethod
      &pushone
      &pushregister 3
      &pushsdbgm 77							//'enableVoipCheckbox'
      &dcallmp 67							// setOnMouseDownFunction()
      &pushregister 3
      &pushsdbgm 86							//'defaultRules'
      &setRegister r:4
      &pop
      &pushzero
      &pushregister 4
      &dcallmp 46							// show()
      &pushzero
      &pushregister 4
      &dcallmp 47							// enable()
      &pushregister 1
      &pushsdbgm 87							//'onResetRules'
      &pushregister 1
      &pushbyte 2
      &pushregister 2
      &pushsdb 24							//'bind0'
      &callMethod
      &pushone
      &pushregister 4
      &dcallmp 49							// setOnMouseUpFunction()
      &pushregister 4
      &pushone
      &pushregister 5
      &dcallmp 50							// push()
      &jmp label6      
     label5:
      &pushzero
      &pushregister 3
      &pushsdbgm 73							//'gameSpeedSlider'
      &dcallmp 62							// disable()
      &pushzero
      &pushregister 3
      &pushsdbgm 74							//'resourcesDropdown'
      &dcallmp 62							// disable()
      &pushzero
      &pushregister 3
      &pushsdbgm 75							//'cratesCheckbox'
      &dcallmp 62							// disable()
      &pushzero
      &pushregister 3
      &pushsdbgm 76							//'commentaryCheckbox'
      &dcallmp 62							// disable()
      &pushzero
      &pushregister 3
      &pushsdbgm 77							//'enableVoipCheckbox'
      &dcallmp 62							// disable()
      &pushzero
      &pushregister 3
      &pushsdbgm 86							//'defaultRules'
      &dcallmp 61							// hide()
      &pushzero
      &pushregister 3
      &pushsdbgm 86							//'defaultRules'
      &dcallmp 62							// disable()
     label6:
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 44							//'initPlayerSlots'
    &function2  () (r:1='this', r:2='_global')
      &pushregister 1
      &pushsdb 88							//'m_playerSlots'
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 11							//'m_screen'
      &pushsdbgm 89							//'slotMC_6'
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 11							//'m_screen'
      &pushsdbgm 90							//'slotMC_5'
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 11							//'m_screen'
      &pushsdbgm 91							//'slotMC_4'
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 11							//'m_screen'
      &pushsdbgm 92							//'slotMC_3'
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 11							//'m_screen'
      &pushsdbgm 93							//'slotMC_2'
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 11							//'m_screen'
      &pushsdbgm 94							//'slotMC_1'
      &pushbyte 6
      &initArray
      &setMember
      &pushzero
      &setRegister r:3
      &pop
     label7:
      &pushregister 3
      &pushregister 1
      &pushsdbgm 88							//'m_playerSlots'
      &pushsdbgm 60							//'length'
      &lessThan
      &not
      &jnz label8      
      &pushregister 1
      &pushsdbgm 88							//'m_playerSlots'
      &pushregister 3
      &getMember
      &pushsdb 95							//'slotId'
      &pushregister 3
      &setMember
      &pushzero
      &pushregister 1
      &pushsdbgm 88							//'m_playerSlots'
      &pushregister 3
      &getMember
      &pushsdbgm 96							//'playerSlotOptions'
      &dcallmp 46							// show()
      &pushzero
      &pushregister 1
      &pushsdbgm 88							//'m_playerSlots'
      &pushregister 3
      &getMember
      &pushsdbgm 96							//'playerSlotOptions'
      &dcallmp 62							// disable()
      &pushregister 1
      &pushsdbgm 97							//'onPlayerStatusChanged'
      &pushregister 1
      &pushbyte 2
      &pushregister 2
      &pushsdb 6							//'bind1DynamicParams'
      &callMethod
      &pushone
      &pushregister 1
      &pushsdbgm 88							//'m_playerSlots'
      &pushregister 3
      &getMember
      &pushsdbgm 96							//'playerSlotOptions'
      &dcallmp 81							// setOnChange()
      &pushzero
      &pushregister 1
      &pushsdbgm 88							//'m_playerSlots'
      &pushregister 3
      &getMember
      &pushsdbgm 98							//'factionOptions'
      &dcallmp 46							// show()
      &pushregister 1
      &pushsdbgm 99							//'onFactionChanged'
      &pushregister 1
      &pushbyte 2
      &pushregister 2
      &pushsdb 6							//'bind1DynamicParams'
      &callMethod
      &pushone
      &pushregister 1
      &pushsdbgm 88							//'m_playerSlots'
      &pushregister 3
      &getMember
      &pushsdbgm 98							//'factionOptions'
      &dcallmp 81							// setOnChange()
      &pushzero
      &pushregister 1
      &pushsdbgm 88							//'m_playerSlots'
      &pushregister 3
      &getMember
      &pushsdbgm 100							//'personalityOptions'
      &dcallmp 46							// show()
      &pushregister 1
      &pushsdbgm 101							//'onAIPersonalityChanged'
      &pushregister 1
      &pushbyte 2
      &pushregister 2
      &pushsdb 6							//'bind1DynamicParams'
      &callMethod
      &pushone
      &pushregister 1
      &pushsdbgm 88							//'m_playerSlots'
      &pushregister 3
      &getMember
      &pushsdbgm 100							//'personalityOptions'
      &dcallmp 81							// setOnChange()
      &pushzero
      &pushregister 1
      &pushsdbgm 88							//'m_playerSlots'
      &pushregister 3
      &getMember
      &pushsdbgm 102							//'teamOptions'
      &dcallmp 46							// show()
      &pushregister 1
      &pushsdbgm 103							//'onTeamChanged'
      &pushregister 1
      &pushbyte 2
      &pushregister 2
      &pushsdb 6							//'bind1DynamicParams'
      &callMethod
      &pushone
      &pushregister 1
      &pushsdbgm 88							//'m_playerSlots'
      &pushregister 3
      &getMember
      &pushsdbgm 102							//'teamOptions'
      &dcallmp 81							// setOnChange()
      &pushzero
      &pushregister 1
      &pushsdbgm 88							//'m_playerSlots'
      &pushregister 3
      &getMember
      &pushsdbgm 104							//'colorOptions'
      &dcallmp 46							// show()
      &pushregister 1
      &pushsdbgm 105							//'onColorChanged'
      &pushregister 1
      &pushbyte 2
      &pushregister 2
      &pushsdb 6							//'bind1DynamicParams'
      &callMethod
      &pushone
      &pushregister 1
      &pushsdbgm 88							//'m_playerSlots'
      &pushregister 3
      &getMember
      &pushsdbgm 104							//'colorOptions'
      &dcallmp 81							// setOnChange()
      &pushregister 3
      &increment
      &setRegister r:3
      &pop
      &jmp label7      
     label8:
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 106							//'initReadyControls'
    &function2  () (r:1='this', r:2='_global')
      &pushzero
      &setRegister r:3
      &pop
     label9:
      &pushregister 3
      &pushregister 1
      &pushsdbgm 88							//'m_playerSlots'
      &pushsdbgm 60							//'length'
      &lessThan
      &not
      &jnz label10      
      &pushregister 1
      &pushsdbgm 88							//'m_playerSlots'
      &pushregister 3
      &getMember
      &pushsdbgm 107							//'readyCheckBox'
      &setRegister r:4
      &pop
      &pushzero
      &pushregister 4
      &dcallmp 46							// show()
      &pushzero
      &pushregister 4
      &dcallmp 62							// disable()
      &pushregister 1
      &pushsdbgm 108							//'onReadyStatusChanged'
      &pushregister 1
      &pushbyte 2
      &pushregister 2
      &pushsdb 24							//'bind0'
      &callMethod
      &pushone
      &pushregister 4
      &dcallmp 67							// setOnMouseDownFunction()
      &pushregister 3
      &increment
      &setRegister r:3
      &pop
      &jmp label9      
     label10:
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 109							//'initPlayButton'
    &function2  () (r:1='this')
      &pushregister 1
      &pushsdbgm 32							//'m_isHost'
      &not
      &jnz label11      
      &pushzero
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 11							//'m_screen'
      &pushsdbgm 45							//'playButton'
      &dcallmp 46							// show()
      &pushzero
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 11							//'m_screen'
      &pushsdbgm 45							//'playButton'
      &dcallmp 47							// enable()
      &jmp label12      
     label11:
      &pushzero
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 11							//'m_screen'
      &pushsdbgm 45							//'playButton'
      &dcallmp 61							// hide()
      &pushzero
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 11							//'m_screen'
      &pushsdbgm 45							//'playButton'
      &dcallmp 62							// disable()
     label12:
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 78							//'setBroadcastCheckboxVisibility'
    &function2  (r:2='isVisible') ()
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 11							//'m_screen'
      &pushsdbgm 12							//'gameSettings'
      &pushsdbgm 69							//'rulesPanel'
      &setRegister r:1
      &pop
      &pushregister 2
      &not
      &jnz label13      
      &pushzero
      &pushregister 1
      &pushsdbgm 110							//'broadcastCheckbox'
      &dcallmp 46							// show()
      &pushregister 1
      &pushsdbgm 111							//'broadcastCheckboxLabel'
      &pushsdb 72							//'_visible'
      &pushtrue
      &setMember
      &pushregister 1
      &pushsdbgm 112							//'broadcastCheckboxShadowLabel'
      &pushsdb 72							//'_visible'
      &pushtrue
      &setMember
      &jmp label14      
     label13:
      &pushzero
      &pushregister 1
      &pushsdbgm 110							//'broadcastCheckbox'
      &dcallmp 61							// hide()
      &pushregister 1
      &pushsdbgm 111							//'broadcastCheckboxLabel'
      &pushsdb 72							//'_visible'
      &pushfalse
      &setMember
      &pushregister 1
      &pushsdbgm 112							//'broadcastCheckboxShadowLabel'
      &pushsdb 72							//'_visible'
      &pushfalse
      &setMember
     label14:
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 68							//'createMapSelectionScrollList'
    &function2  () (r:1='this')
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 11							//'m_screen'
      &pushsdbgm 12							//'gameSettings'
      &pushsdbgm 13							//'mapPanel'
      &pushsdbgm 113							//'mapListScrollbar'
      &setRegister r:4
      &pop
      &pushzero
      &pushregister 4
      &dcallmp 46							// show()
      &pushzero
      &pushregister 4
      &dcallmp 47							// enable()
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 11							//'m_screen'
      &pushsdbgm 12							//'gameSettings'
      &pushsdbgm 13							//'mapPanel'
      &pushsdbgm 114							//'mapList'
      &setRegister r:2
      &pop
      &pushregister 1
      &pushone
      &pushregister 2
      &dcallmp 115							// addListener()
      &pushzero
      &pushsdb 8							//'Array'
      &new
      &setRegister r:3
      &pop
      &pushsdb 116							//'symbol'
      &pushsdb 117							//'std_mouseListboxEntryTextElementSymbol'
      &pushsdb 118							//'width'
      &pushshort 270
      &pushsdb 119							//'leftMargin'
      &pushzero
      &pushsdb 120							//'rightMargin'
      &pushzero
      &pushbyte 4
      &initObject
      &pushone
      &pushregister 3
      &dcallmp 50							// push()
      &pushsdb 116							//'symbol'
      &pushsdb 117							//'std_mouseListboxEntryTextElementSymbol'
      &pushsdb 118							//'width'
      &pushbyte 75
      &pushsdb 119							//'leftMargin'
      &pushbyte 26
      &pushsdb 120							//'rightMargin'
      &pushzero
      &pushbyte 4
      &initObject
      &pushone
      &pushregister 3
      &dcallmp 50							// push()
      &pushbyte 26
      &pushregister 3
      &pushbyte 2
      &pushregister 2
      &dcallmp 121							// init()
      &pushregister 4
      &pushone
      &pushregister 2
      &dcallmp 122							// attachScrollbarController()
      &pushzero
      &pushregister 2
      &dcallmp 46							// show()
      &pushzero
      &pushregister 2
      &dcallmp 47							// enable()
      &pushzero
      &pushregister 2
      &dcallmp 123							// moveViewToFirst()
      &pushzero
      &pushregister 2
      &dcallmp 124							// refreshDisplay()
      &pushzero
      &pushregister 2
      &dcallmp 125							// refreshScrollbar()
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 5							//'localMessageHandler'
    &function2  (r:4='messageCode') (r:1='this', r:2='_global')
      &pushfalse
      &setRegister r:3
      &pop
      &pushregister 4
      &pushregister 2
      &pushsdbgm 126							//'MSGCODE'
      &pushsdbgm 127							//'FE_MP_UPDATE_GAME_SETTINGS'
      &strictEquals
      &jnz label15      
      &jmp label16      
     label15:
      &pushzero
      &pushregister 1
      &dcallmp 128							// refreshComponents()
      &pushtrue
      &setRegister r:3
      &pop
      &jmp label17      
     label16:
      &jmp label17      
     label17:
      &pushregister 3
      &return
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 23							//'onScreenExit'
    &function2  () (r:1='this', r:2='_global')
      &getURL 'FSCommand:CallGameFunction'       '%SetGameSetupMode?Mode=None'      
      &pushregister 1
      &pushsdbgm 4							//'m_localMessageDispatcher'
      &pushone
      &pushregister 2
      &pushsdbgm 36							//'gMH'
      &dcallmp 129							// removeMessageHandler()
      &pushregister 1
      &pushsdb 4							//'m_localMessageDispatcher'
      &delete
      &pop
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 11							//'m_screen'
      &pushsdbgm 12							//'gameSettings'
      &pushsdbgm 13							//'mapPanel'
      &pushsdbgm 114							//'mapList'
      &setRegister r:3
      &pop
      &pushregister 1
      &pushone
      &pushregister 3
      &dcallmp 130							// removeListener()
      &pushzero
      &pushregister 3
      &dcallmp 131							// shutdown()
      &pushzero
      &pushregister 1
      &dcallmp 132							// clearData()
      &pushregister 2
      &pushsdb 1							//'GameSetupBase'
      &delete
      &pop
      &pushregister 2
      &pushsdb 2							//'Cafe2_BaseUIScreen'
      &delete
      &pop
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 48							//'playGame'
    &function  (    )    
      &getURL 'FSCommand:CallGameFunction'       '%StartGame'      
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 66							//'onMapTypeChanged'
    &function2  () (r:1='_global')
      &pushzero
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 11							//'m_screen'
      &pushsdbgm 12							//'gameSettings'
      &pushsdbgm 13							//'mapPanel'
      &pushsdbgm 65							//'campaignCheckbox'
      &pushsdb 133							//'isChecked'
      &callMethod
      &jnz label18      
      &pushsdb 134							//'0'
      &jmp label19      
     label18:
      &pushsdb 34							//'1'
     label19:
      &setRegister r:2
      &pop
      &pushsdb 135							//'FSCommand:CallGameFunction'
      &pushsdb 136							//'%SetMapType?MapType='
      &pushregister 2
      &add
      &getURL2
      &pushregister 2
      &not
      &jnz label20      
      &pushregister 1
      &pushsdbgm 137							//'GAME'
      &pushsdb 138							//'MODE'
      &pushregister 1
      &pushsdbgm 139							//'ENUM'
      &pushsdbgm 140							//'GAME_MODE_CAMPAIGN'
      &setMember
      &jmp label21      
     label20:
      &pushregister 1
      &pushsdbgm 137							//'GAME'
      &pushsdb 138							//'MODE'
      &pushregister 1
      &pushsdbgm 139							//'ENUM'
      &pushsdbgm 141							//'GAME_MODE_SKIRMISH'
      &setMember
     label21:
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 39							//'OnMapListSelectionChanged'
    &function2  (r:2='index') (r:1='this')
      &pushsdb 135							//'FSCommand:CallGameFunction'
      &pushsdb 142							//'%SetMap?Map='
      &pushregister 1
      &pushsdbgm 7							//'m_mapIds'
      &pushregister 2
      &getMember
      &add
      &getURL2
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 143							//'onShowMapPanel'
    &function  (    )    
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 11							//'m_screen'
      &pushsdbgm 12							//'gameSettings'
      &pushsdbgm 13							//'mapPanel'
      &pushsdb 72							//'_visible'
      &pushtrue
      &setMember
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 11							//'m_screen'
      &pushsdbgm 12							//'gameSettings'
      &pushsdbgm 69							//'rulesPanel'
      &pushsdb 72							//'_visible'
      &pushfalse
      &setMember
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 144							//'onShowRulesPanel'
    &function  (    )    
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 11							//'m_screen'
      &pushsdbgm 12							//'gameSettings'
      &pushsdbgm 13							//'mapPanel'
      &pushsdb 72							//'_visible'
      &pushfalse
      &setMember
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 11							//'m_screen'
      &pushsdbgm 12							//'gameSettings'
      &pushsdbgm 69							//'rulesPanel'
      &pushsdb 72							//'_visible'
      &pushtrue
      &setMember
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 97							//'onPlayerStatusChanged'
    &function2  (r:3='playerStatusMC') ()
      &pushzero
      &pushregister 3
      &pushsdb 145							//'getCurrentIndex'
      &callMethod
      &pushone
      &pushregister 3
      &pushsdb 146							//'getValueAtIndex'
      &callMethod
      &toString
      &setRegister r:2
      &pop
      &pushregister 3
      &pushsdbgm 147							//'_parent'
      &pushsdbgm 95							//'slotId'
      &toString
      &setRegister r:1
      &pop
      &pushsdb 135							//'FSCommand:CallGameFunction'
      &pushsdb 148							//'%SetPlayerStatus?Slot='
      &pushregister 1
      &add
      &pushsdb 149							//'|Status='
      &add
      &pushregister 2
      &add
      &getURL2
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 99							//'onFactionChanged'
    &function2  (r:1='factionsMC') ()
      &pushzero
      &pushregister 1
      &pushsdb 145							//'getCurrentIndex'
      &callMethod
      &pushone
      &pushregister 1
      &pushsdb 146							//'getValueAtIndex'
      &callMethod
      &toString
      &setRegister r:3
      &pop
      &pushregister 1
      &pushsdbgm 147							//'_parent'
      &pushsdbgm 95							//'slotId'
      &toString
      &setRegister r:2
      &pop
      &pushsdb 135							//'FSCommand:CallGameFunction'
      &pushsdb 150							//'%SetPlayerTemplate?Slot='
      &pushregister 2
      &add
      &pushsdb 151							//'|Template='
      &add
      &pushregister 3
      &add
      &getURL2
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 103							//'onTeamChanged'
    &function2  (r:2='teamsMC') ()
      &pushzero
      &pushregister 2
      &pushsdb 145							//'getCurrentIndex'
      &callMethod
      &pushone
      &pushregister 2
      &pushsdb 146							//'getValueAtIndex'
      &callMethod
      &toString
      &setRegister r:3
      &pop
      &pushregister 2
      &pushsdbgm 147							//'_parent'
      &pushsdbgm 95							//'slotId'
      &toString
      &setRegister r:1
      &pop
      &pushsdb 135							//'FSCommand:CallGameFunction'
      &pushsdb 152							//'%SetTeam?Slot='
      &pushregister 1
      &add
      &pushsdb 153							//'|Team='
      &add
      &pushregister 3
      &add
      &getURL2
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 105							//'onColorChanged'
    &function2  (r:3='colorsMC') ()
      &pushzero
      &pushregister 3
      &pushsdb 145							//'getCurrentIndex'
      &callMethod
      &pushone
      &pushregister 3
      &pushsdb 146							//'getValueAtIndex'
      &callMethod
      &toString
      &setRegister r:2
      &pop
      &pushregister 3
      &pushsdbgm 147							//'_parent'
      &pushsdbgm 95							//'slotId'
      &toString
      &setRegister r:1
      &pop
      &pushsdb 135							//'FSCommand:CallGameFunction'
      &pushsdb 154							//'%SetColor?Slot='
      &pushregister 1
      &add
      &pushsdb 155							//'|Color='
      &add
      &pushregister 2
      &add
      &getURL2
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 101							//'onAIPersonalityChanged'
    &function2  (r:2='personalitiesMC') ()
      &pushzero
      &pushregister 2
      &pushsdb 145							//'getCurrentIndex'
      &callMethod
      &pushone
      &pushregister 2
      &pushsdb 146							//'getValueAtIndex'
      &callMethod
      &toString
      &setRegister r:3
      &pop
      &pushregister 2
      &pushsdbgm 147							//'_parent'
      &pushsdbgm 95							//'slotId'
      &toString
      &setRegister r:1
      &pop
      &pushsdb 135							//'FSCommand:CallGameFunction'
      &pushsdb 156							//'%SetAIPersonality?Slot='
      &pushregister 1
      &add
      &pushsdb 157							//'|Personality='
      &add
      &pushregister 3
      &add
      &getURL2
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 108							//'onReadyStatusChanged'
    &function  (    )    
      &getURL 'FSCommand:CallGameFunction'       '%ToggleReadyStatus'      
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 63							//'onStartPositionClicked'
    &function2  (r:1='startPosition') ()
      &pushsdb 135							//'FSCommand:CallGameFunction'
      &pushsdb 158							//'%CycleStartPositionOwner?Position='
      &pushregister 1
      &add
      &getURL2
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 79							//'onGameSpeedChanged'
    &function2  ('component', r:1='newSpeed') ()
      &pushsdb 135							//'FSCommand:CallGameFunction'
      &pushsdb 159							//'%SetGameSpeed?GameSpeed='
      &pushregister 1
      &toString
      &add
      &getURL2
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 82							//'onResourcesChanged'
    &function2  (r:2='resourcesMC') ()
      &pushzero
      &pushregister 2
      &pushsdb 145							//'getCurrentIndex'
      &callMethod
      &pushone
      &pushregister 2
      &pushsdb 146							//'getValueAtIndex'
      &callMethod
      &toString
      &setRegister r:1
      &pop
      &pushsdb 135							//'FSCommand:CallGameFunction'
      &pushsdb 160							//'%SetInitialResources?Resources='
      &pushregister 1
      &add
      &getURL2
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 85							//'onVoipChanged'
    &function  (    )    
      &getURL 'FSCommand:CallGameFunction'       '%ToggleVoipRule'      
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 83							//'onCratesChanged'
    &function  (    )    
      &getURL 'FSCommand:CallGameFunction'       '%ToggleRandomCrates'      
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 84							//'onCommentaryChanged'
    &function  (    )    
      &getURL 'FSCommand:CallGameFunction'       '%ToggleCommentary'      
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 87							//'onResetRules'
    &function  (    )    
      &getURL 'FSCommand:CallGameFunction'       '%ResetRules'      
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 128							//'refreshComponents'
    &function2  () (r:1='this')
      &pushzero
      &pushregister 1
      &dcallmp 161							// refreshMapList()
      &pushzero
      &pushregister 1
      &dcallmp 162							// refreshSlotStatusControls()
      &pushzero
      &pushregister 1
      &dcallmp 163							// refreshCampaignMapControls()
      &pushzero
      &pushregister 1
      &dcallmp 164							// refreshResourcesControl()
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 11							//'m_screen'
      &pushsdbgm 12							//'gameSettings'
      &pushsdbgm 69							//'rulesPanel'
      &pushsdbgm 75							//'cratesCheckbox'
      &pushsdb 165							//'RANDOM_CRATES'
      &pushbyte 2
      &pushregister 1
      &dcallmp 166							// refreshRulesCheckbox()
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 11							//'m_screen'
      &pushsdbgm 12							//'gameSettings'
      &pushsdbgm 69							//'rulesPanel'
      &pushsdbgm 77							//'enableVoipCheckbox'
      &pushsdb 167							//'ENABLE_VOIP'
      &pushbyte 2
      &pushregister 1
      &dcallmp 166							// refreshRulesCheckbox()
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 11							//'m_screen'
      &pushsdbgm 12							//'gameSettings'
      &pushsdbgm 69							//'rulesPanel'
      &pushsdbgm 76							//'commentaryCheckbox'
      &pushsdb 168							//'ALLOW_COMMENTARY'
      &pushbyte 2
      &pushregister 1
      &dcallmp 166							// refreshRulesCheckbox()
      &pushzero
      &pushregister 1
      &dcallmp 169							// refreshGameSpeedControls()
      &pushzero
      &pushregister 1
      &dcallmp 170							// refreshTeamControls()
      &pushzero
      &pushregister 1
      &dcallmp 171							// refreshFactionControls()
      &pushzero
      &pushregister 1
      &dcallmp 172							// refreshAIPersonalityControls()
      &pushzero
      &pushregister 1
      &dcallmp 173							// setupMapRenderClip()
      &pushzero
      &pushregister 1
      &dcallmp 174							// refreshStartPositionControls()
      &pushzero
      &pushregister 1
      &dcallmp 175							// refreshColorControls()
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 161							//'refreshMapList'
    &function2  () (r:1='this')
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 11							//'m_screen'
      &pushsdbgm 12							//'gameSettings'
      &pushsdbgm 13							//'mapPanel'
      &pushsdbgm 114							//'mapList'
      &setRegister r:5
      &pop
      &pushzero
      &pushsdb 30							//'Object'
      &new
      &setRegister r:6
      &pop
      &pushzero
      &pushsdb 8							//'Array'
      &new
      &setRegister r:4
      &pop
      &pushzero
      &pushsdb 8							//'Array'
      &new
      &setRegister r:3
      &pop
      &pushzero
      &setRegister r:7
      &pop
      &pushsdb 176							//'QueryGameEngine?MP_MAP_LIST'
      &pushregister 6
      &loadVariables
      &pushregister 1
      &pushsdb 7							//'m_mapIds'
      &pushsdb 177							//','
      &pushone
      &pushregister 6
      &pushsdbgm 178							//'MP_MAP_LIST'
      &pushsdb 179							//'split'
      &callMethod
      &setMember
      &pushzero
      &pushregister 1
      &pushsdbgm 7							//'m_mapIds'
      &pushsdb 180							//'shift'
      &callMethod
      &toNumber
      &setRegister r:7
      &pop
      &pushzero
      &setRegister r:2
      &pop
     label22:
      &pushregister 2
      &pushregister 1
      &pushsdbgm 7							//'m_mapIds'
      &pushsdbgm 60							//'length'
      &lessThan
      &not
      &jnz label23      
      &pushregister 4
      &pushregister 2
      &pushsdb 181							//'$MP_MAP_LIST_'
      &pushregister 2
      &toString
      &add
      &setMember
      &pushregister 3
      &pushregister 2
      &pushsdb 182							//'$MP_MAP_SIZE_'
      &pushregister 2
      &toString
      &add
      &setMember
      &pushregister 2
      &increment
      &setRegister r:2
      &pop
      &jmp label22      
     label23:
      &pushregister 4
      &pushzero
      &pushbyte 2
      &pushregister 5
      &dcallmp 183							// setColumnData()
      &pushregister 3
      &pushone
      &pushbyte 2
      &pushregister 5
      &dcallmp 183							// setColumnData()
      &pushzero
      &pushregister 5
      &dcallmp 124							// refreshDisplay()
      &pushzero
      &pushregister 5
      &dcallmp 125							// refreshScrollbar()
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 162							//'refreshSlotStatusControls'
    &function2  () (r:1='this')
      &pushzero
      &setRegister r:3
      &pop
     label24:
      &pushregister 3
      &pushregister 1
      &pushsdbgm 88							//'m_playerSlots'
      &pushsdbgm 60							//'length'
      &lessThan
      &not
      &jnz label27      
      &pushzero
      &pushsdb 30							//'Object'
      &new
      &setRegister r:5
      &pop
      &pushzero
      &pushsdb 8							//'Array'
      &new
      &setRegister r:6
      &pop
      &pushzero
      &pushsdb 8							//'Array'
      &new
      &setRegister r:4
      &pop
      &pushzero
      &setRegister r:7
      &pop
      &pushsdb 184							//'QueryGameEngine?PLAYER_STATUS?Slot='
      &pushregister 3
      &toString
      &add
      &pushregister 5
      &loadVariables
      &pushsdb 177							//','
      &pushone
      &pushregister 5
      &pushsdbgm 185							//'PLAYER_STATUS'
      &pushsdb 179							//'split'
      &callMethod
      &setRegister r:4
      &pop
      &pushzero
      &pushregister 4
      &pushsdb 180							//'shift'
      &callMethod
      &toNumber
      &setRegister r:7
      &pop
      &pushzero
      &setRegister r:2
      &pop
     label25:
      &pushregister 2
      &pushregister 4
      &pushsdbgm 60							//'length'
      &lessThan
      &not
      &jnz label26      
      &pushregister 6
      &pushregister 2
      &pushsdb 186							//'$PLAYER_STATUS_'
      &pushregister 3
      &toString
      &add
      &pushsdb 187							//'_'
      &add
      &pushregister 2
      &toString
      &add
      &setMember
      &pushregister 2
      &increment
      &setRegister r:2
      &pop
      &jmp label25      
     label26:
      &pushregister 1
      &pushsdbgm 88							//'m_playerSlots'
      &pushregister 3
      &getMember
      &pushsdbgm 96							//'playerSlotOptions'
      &pushregister 4
      &pushsdbgm 60							//'length'
      &pushone
      &greaterThan
      &pushbyte 2
      &pushregister 1
      &dcallmp 188							// enableControl()
      &pushregister 4
      &pushregister 6
      &pushbyte 2
      &pushregister 1
      &pushsdbgm 88							//'m_playerSlots'
      &pushregister 3
      &getMember
      &pushsdbgm 96							//'playerSlotOptions'
      &dcallmp 189							// setData()
      &pushregister 7
      &pushone
      &pushregister 1
      &pushsdbgm 88							//'m_playerSlots'
      &pushregister 3
      &getMember
      &pushsdbgm 96							//'playerSlotOptions'
      &dcallmp 190							// setSelectedIndex()
      &pushregister 3
      &increment
      &setRegister r:3
      &pop
      &jmp label24      
     label27:
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 170							//'refreshTeamControls'
    &function2  () (r:1='this')
      &pushzero
      &setRegister r:4
      &pop
     label28:
      &pushregister 4
      &pushregister 1
      &pushsdbgm 88							//'m_playerSlots'
      &pushsdbgm 60							//'length'
      &lessThan
      &not
      &jnz label32      
      &pushzero
      &pushsdb 30							//'Object'
      &new
      &setRegister r:5
      &pop
      &pushzero
      &pushsdb 8							//'Array'
      &new
      &setRegister r:6
      &pop
      &pushzero
      &pushsdb 8							//'Array'
      &new
      &setRegister r:3
      &pop
      &pushzero
      &setRegister r:7
      &pop
      &pushsdb 191							//'QueryGameEngine?TEAM_OPTIONS?Slot='
      &pushregister 4
      &toString
      &add
      &pushregister 5
      &loadVariables
      &pushsdb 177							//','
      &pushone
      &pushregister 5
      &pushsdbgm 192							//'TEAM_OPTIONS_VALUES'
      &pushsdb 179							//'split'
      &callMethod
      &setRegister r:3
      &pop
      &pushzero
      &pushregister 3
      &pushsdb 180							//'shift'
      &callMethod
      &toNumber
      &setRegister r:7
      &pop
      &pushzero
      &setRegister r:2
      &pop
     label29:
      &pushregister 2
      &pushregister 3
      &pushsdbgm 60							//'length'
      &lessThan
      &not
      &jnz label30      
      &pushregister 6
      &pushregister 2
      &pushsdb 193							//'$TEAM_'
      &pushregister 3
      &pushregister 2
      &getMember
      &toString
      &add
      &setMember
      &pushregister 2
      &increment
      &setRegister r:2
      &pop
      &jmp label29      
     label30:
      &pushregister 1
      &pushsdbgm 88							//'m_playerSlots'
      &pushregister 4
      &getMember
      &pushsdbgm 102							//'teamOptions'
      &pushregister 5
      &pushsdbgm 194							//'TEAM_OPTIONS_ENABLE'
      &pushsdb 34							//'1'
      &equals
      &dup
      &not
      &jnz label31      
      &pop
      &pushregister 3
      &pushsdbgm 60							//'length'
      &pushone
      &greaterThan
     label31:
      &pushbyte 2
      &pushregister 1
      &dcallmp 188							// enableControl()
      &pushregister 3
      &pushregister 6
      &pushbyte 2
      &pushregister 1
      &pushsdbgm 88							//'m_playerSlots'
      &pushregister 4
      &getMember
      &pushsdbgm 102							//'teamOptions'
      &dcallmp 189							// setData()
      &pushregister 7
      &pushone
      &pushregister 1
      &pushsdbgm 88							//'m_playerSlots'
      &pushregister 4
      &getMember
      &pushsdbgm 102							//'teamOptions'
      &dcallmp 190							// setSelectedIndex()
      &pushregister 4
      &increment
      &setRegister r:4
      &pop
      &jmp label28      
     label32:
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 171							//'refreshFactionControls'
    &function2  () (r:1='this')
      &pushzero
      &setRegister r:2
      &pop
     label33:
      &pushregister 2
      &pushregister 1
      &pushsdbgm 88							//'m_playerSlots'
      &pushsdbgm 60							//'length'
      &lessThan
      &not
      &jnz label34      
      &pushzero
      &pushsdb 30							//'Object'
      &new
      &setRegister r:4
      &pop
      &pushsdb 195							//'QueryGameEngine?PLAYER_TEMPLATE_OPTIONS?Slot='
      &pushregister 2
      &toString
      &add
      &pushregister 4
      &loadVariables
      &pushzero
      &pushsdb 8							//'Array'
      &new
      &setRegister r:3
      &pop
      &pushsdb 177							//','
      &pushone
      &pushregister 4
      &pushsdbgm 196							//'PLAYER_TEMPLATE_OPTIONS_VALUES'
      &pushsdb 179							//'split'
      &callMethod
      &setRegister r:3
      &pop
      &pushzero
      &pushregister 3
      &pushsdb 180							//'shift'
      &callMethod
      &toNumber
      &setRegister r:6
      &pop
      &pushzero
      &pushsdb 8							//'Array'
      &new
      &setRegister r:5
      &pop
      &pushsdb 177							//','
      &pushone
      &pushregister 4
      &pushsdbgm 197							//'PLAYER_TEMPLATE_OPTIONS_IMAGES'
      &pushsdb 179							//'split'
      &callMethod
      &setRegister r:5
      &pop
      &pushregister 1
      &pushsdbgm 88							//'m_playerSlots'
      &pushregister 2
      &getMember
      &pushsdbgm 98							//'factionOptions'
      &pushregister 3
      &pushsdbgm 60							//'length'
      &pushone
      &greaterThan
      &pushbyte 2
      &pushregister 1
      &dcallmp 188							// enableControl()
      &pushregister 3
      &pushregister 5
      &pushbyte 2
      &pushregister 1
      &pushsdbgm 88							//'m_playerSlots'
      &pushregister 2
      &getMember
      &pushsdbgm 98							//'factionOptions'
      &dcallmp 189							// setData()
      &pushregister 6
      &pushone
      &pushregister 1
      &pushsdbgm 88							//'m_playerSlots'
      &pushregister 2
      &getMember
      &pushsdbgm 98							//'factionOptions'
      &dcallmp 190							// setSelectedIndex()
      &pushregister 2
      &increment
      &setRegister r:2
      &pop
      &jmp label33      
     label34:
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 172							//'refreshAIPersonalityControls'
    &function2  () (r:1='this')
      &pushzero
      &setRegister r:2
      &pop
     label35:
      &pushregister 2
      &pushregister 1
      &pushsdbgm 88							//'m_playerSlots'
      &pushsdbgm 60							//'length'
      &lessThan
      &not
      &jnz label40      
      &pushzero
      &pushsdb 30							//'Object'
      &new
      &setRegister r:5
      &pop
      &pushsdb 198							//'QueryGameEngine?AI_PERSONALITY_OPTIONS?Slot='
      &pushregister 2
      &toString
      &add
      &pushregister 5
      &loadVariables
      &pushregister 5
      &pushsdbgm 199							//'AI_PERSONALITY_OPTIONS'
      &pushundef
      &equals
      &not
      &jnz label36      
      &pushzero
      &pushregister 1
      &pushsdbgm 88							//'m_playerSlots'
      &pushregister 2
      &getMember
      &pushsdbgm 100							//'personalityOptions'
      &dcallmp 200							// clear()
      &pushzero
      &pushregister 1
      &pushsdbgm 88							//'m_playerSlots'
      &pushregister 2
      &getMember
      &pushsdbgm 100							//'personalityOptions'
      &dcallmp 62							// disable()
      &jmp label39      
     label36:
      &pushzero
      &pushsdb 8							//'Array'
      &new
      &setRegister r:4
      &pop
      &pushsdb 177							//','
      &pushone
      &pushregister 5
      &pushsdbgm 199							//'AI_PERSONALITY_OPTIONS'
      &pushsdb 179							//'split'
      &callMethod
      &setRegister r:4
      &pop
      &pushzero
      &pushregister 4
      &pushsdb 180							//'shift'
      &callMethod
      &toNumber
      &setRegister r:7
      &pop
      &pushzero
      &pushsdb 8							//'Array'
      &new
      &setRegister r:6
      &pop
      &pushzero
      &setRegister r:3
      &pop
     label37:
      &pushregister 3
      &pushregister 4
      &pushsdbgm 60							//'length'
      &lessThan
      &not
      &jnz label38      
      &pushregister 6
      &pushregister 3
      &pushsdb 201							//'$APT:AI_PERSONALITY_'
      &pushregister 2
      &toString
      &add
      &pushsdb 187							//'_'
      &add
      &pushregister 4
      &pushregister 3
      &getMember
      &toString
      &add
      &setMember
      &pushregister 3
      &increment
      &setRegister r:3
      &pop
      &jmp label37      
     label38:
      &pushregister 1
      &pushsdbgm 88							//'m_playerSlots'
      &pushregister 2
      &getMember
      &pushsdbgm 100							//'personalityOptions'
      &pushregister 4
      &pushsdbgm 60							//'length'
      &pushone
      &greaterThan
      &pushbyte 2
      &pushregister 1
      &dcallmp 188							// enableControl()
      &pushregister 4
      &pushregister 6
      &pushbyte 2
      &pushregister 1
      &pushsdbgm 88							//'m_playerSlots'
      &pushregister 2
      &getMember
      &pushsdbgm 100							//'personalityOptions'
      &dcallmp 189							// setData()
      &pushregister 7
      &pushone
      &pushregister 1
      &pushsdbgm 88							//'m_playerSlots'
      &pushregister 2
      &getMember
      &pushsdbgm 100							//'personalityOptions'
      &dcallmp 190							// setSelectedIndex()
     label39:
      &pushregister 2
      &increment
      &setRegister r:2
      &pop
      &jmp label35      
     label40:
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 175							//'refreshColorControls'
    &function2  () (r:1='this')
      &pushzero
      &setRegister r:2
      &pop
     label41:
      &pushregister 2
      &pushregister 1
      &pushsdbgm 88							//'m_playerSlots'
      &pushsdbgm 60							//'length'
      &lessThan
      &not
      &jnz label44      
      &pushzero
      &pushsdb 30							//'Object'
      &new
      &setRegister r:3
      &pop
      &pushsdb 202							//'QueryGameEngine?PLAYER_COLOR?Slot='
      &pushregister 2
      &add
      &pushregister 3
      &loadVariables
      &pushzero
      &pushsdb 8							//'Array'
      &new
      &setRegister r:5
      &pop
      &pushsdb 177							//','
      &pushone
      &pushregister 3
      &pushsdbgm 203							//'PLAYER_COLOR_VALUES'
      &pushsdb 179							//'split'
      &callMethod
      &setRegister r:5
      &pop
      &pushzero
      &pushregister 5
      &pushsdb 180							//'shift'
      &callMethod
      &toNumber
      &setRegister r:6
      &pop
      &pushregister 1
      &pushsdbgm 88							//'m_playerSlots'
      &pushregister 2
      &getMember
      &pushsdbgm 104							//'colorOptions'
      &setRegister r:4
      &pop
      &pushregister 5
      &pushregister 5
      &pushbyte 2
      &pushregister 4
      &dcallmp 189							// setData()
      &pushregister 6
      &pushone
      &pushregister 4
      &dcallmp 190							// setSelectedIndex()
      &pushregister 3
      &pushsdbgm 204							//'PLAYER_COLOR_ENABLE'
      &pushsdb 34							//'1'
      &equals
      &not
      &jnz label42      
      &pushzero
      &pushregister 4
      &dcallmp 47							// enable()
      &jmp label43      
     label42:
      &pushzero
      &pushregister 4
      &dcallmp 62							// disable()
     label43:
      &pushregister 2
      &increment
      &setRegister r:2
      &pop
      &jmp label41      
     label44:
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 173							//'setupMapRenderClip'
    &function2  () (r:1='this')
      &pushzero
      &pushsdb 30							//'Object'
      &new
      &setRegister r:2
      &pop
      &pushsdb 205							//'QueryGameEngine?MAP_ASPECT_RATIO'
      &pushregister 2
      &loadVariables
      &pushregister 2
      &pushsdbgm 206							//'MAP_ASPECT_RATIO'
      &toNumber
      &setRegister r:3
      &pop
      &pushregister 3
      &pushregister 1
      &pushsdbgm 19							//'m_originalMapWidth'
      &pushregister 1
      &pushsdbgm 21							//'m_originalMapHeight'
      &divide
      &greaterThan
      &not
      &jnz label45      
      &pushregister 1
      &pushsdbgm 10							//'m_mapPreviewMC'
      &pushsdb 20							//'_width'
      &pushregister 1
      &pushsdbgm 19							//'m_originalMapWidth'
      &setMember
      &pushregister 1
      &pushsdbgm 10							//'m_mapPreviewMC'
      &pushsdb 22							//'_height'
      &pushregister 1
      &pushsdbgm 19							//'m_originalMapWidth'
      &pushregister 3
      &divide
      &setMember
      &pushregister 1
      &pushsdbgm 10							//'m_mapPreviewMC'
      &pushsdb 16							//'_x'
      &pushregister 1
      &pushsdbgm 15							//'m_originalMapX'
      &setMember
      &pushregister 1
      &pushsdbgm 10							//'m_mapPreviewMC'
      &pushsdb 18							//'_y'
      &pushregister 1
      &pushsdbgm 17							//'m_originalMapY'
      &pushregister 1
      &pushsdbgm 21							//'m_originalMapHeight'
      &pushregister 1
      &pushsdbgm 10							//'m_mapPreviewMC'
      &pushsdbgm 22							//'_height'
      &subtract
      &pushfloat 0.500000000000000
      &multiply
      &add
      &setMember
      &jmp label46      
     label45:
      &pushregister 1
      &pushsdbgm 10							//'m_mapPreviewMC'
      &pushsdb 22							//'_height'
      &pushregister 1
      &pushsdbgm 21							//'m_originalMapHeight'
      &setMember
      &pushregister 1
      &pushsdbgm 10							//'m_mapPreviewMC'
      &pushsdb 20							//'_width'
      &pushregister 1
      &pushsdbgm 21							//'m_originalMapHeight'
      &pushregister 3
      &multiply
      &setMember
      &pushregister 1
      &pushsdbgm 10							//'m_mapPreviewMC'
      &pushsdb 18							//'_y'
      &pushregister 1
      &pushsdbgm 17							//'m_originalMapY'
      &setMember
      &pushregister 1
      &pushsdbgm 10							//'m_mapPreviewMC'
      &pushsdb 16							//'_x'
      &pushregister 1
      &pushsdbgm 15							//'m_originalMapX'
      &pushregister 1
      &pushsdbgm 19							//'m_originalMapWidth'
      &pushregister 1
      &pushsdbgm 10							//'m_mapPreviewMC'
      &pushsdbgm 20							//'_width'
      &subtract
      &pushfloat 0.500000000000000
      &multiply
      &add
      &setMember
     label46:
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 174							//'refreshStartPositionControls'
    &function2  () (r:1='this')
      &pushzero
      &setRegister r:2
      &pop
     label47:
      &pushregister 2
      &pushregister 1
      &pushsdbgm 53							//'m_playerStartPosMCArray'
      &pushsdbgm 60							//'length'
      &lessThan
      &not
      &jnz label50      
      &pushzero
      &pushsdb 30							//'Object'
      &new
      &setRegister r:3
      &pop
      &pushsdb 207							//'QueryGameEngine?START_POSITION?Position='
      &pushregister 2
      &toString
      &add
      &pushregister 3
      &loadVariables
      &pushregister 3
      &pushsdbgm 208							//'START_POSITION_VALID'
      &pushone
      &pushsdb 209							//'Boolean'
      &callFunction
      &setRegister r:6
      &pop
      &pushsdb 210							//'$START_POSITION_'
      &pushregister 2
      &toString
      &add
      &pushone
      &pushregister 1
      &pushsdbgm 53							//'m_playerStartPosMCArray'
      &pushregister 2
      &getMember
      &dcallmp 211							// setText()
      &pushregister 6
      &not
      &jnz label48      
      &pushregister 3
      &pushsdbgm 212							//'START_POSITION_X'
      &toNumber
      &setRegister r:5
      &pop
      &pushregister 3
      &pushsdbgm 213							//'START_POSITION_Y'
      &toNumber
      &setRegister r:4
      &pop
      &pushzero
      &pushregister 1
      &pushsdbgm 53							//'m_playerStartPosMCArray'
      &pushregister 2
      &getMember
      &dcallmp 46							// show()
      &pushzero
      &pushregister 1
      &pushsdbgm 53							//'m_playerStartPosMCArray'
      &pushregister 2
      &getMember
      &dcallmp 47							// enable()
      &pushregister 1
      &pushsdbgm 53							//'m_playerStartPosMCArray'
      &pushregister 2
      &getMember
      &pushsdb 16							//'_x'
      &pushregister 5
      &pushregister 1
      &pushsdbgm 10							//'m_mapPreviewMC'
      &pushsdbgm 20							//'_width'
      &multiply
      &pushregister 1
      &pushsdbgm 10							//'m_mapPreviewMC'
      &pushsdbgm 16							//'_x'
      &add
      &setMember
      &pushregister 1
      &pushsdbgm 53							//'m_playerStartPosMCArray'
      &pushregister 2
      &getMember
      &pushsdb 18							//'_y'
      &pushregister 4
      &pushregister 1
      &pushsdbgm 10							//'m_mapPreviewMC'
      &pushsdbgm 22							//'_height'
      &multiply
      &pushregister 1
      &pushsdbgm 10							//'m_mapPreviewMC'
      &pushsdbgm 18							//'_y'
      &add
      &setMember
      &jmp label49      
     label48:
      &pushzero
      &pushregister 1
      &pushsdbgm 53							//'m_playerStartPosMCArray'
      &pushregister 2
      &getMember
      &dcallmp 62							// disable()
      &pushzero
      &pushregister 1
      &pushsdbgm 53							//'m_playerStartPosMCArray'
      &pushregister 2
      &getMember
      &dcallmp 61							// hide()
     label49:
      &pushregister 2
      &increment
      &setRegister r:2
      &pop
      &jmp label47      
     label50:
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 163							//'refreshCampaignMapControls'
    &function2  () (r:1='this')
      &pushzero
      &pushsdb 30							//'Object'
      &new
      &setRegister r:2
      &pop
      &pushsdb 214							//'QueryGameEngine?PLAY_AS_CAMPAIGN'
      &pushregister 2
      &loadVariables
      &pushregister 2
      &pushsdbgm 215							//'PLAY_AS_CAMPAIGN_VALUE'
      &pushsdb 134							//'0'
      &equals
      &not
      &jnz label51      
      &pushzero
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 11							//'m_screen'
      &pushsdbgm 12							//'gameSettings'
      &pushsdbgm 13							//'mapPanel'
      &pushsdbgm 65							//'campaignCheckbox'
      &dcallmp 216							// unCheck()
      &jmp label52      
     label51:
      &pushzero
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 11							//'m_screen'
      &pushsdbgm 12							//'gameSettings'
      &pushsdbgm 13							//'mapPanel'
      &pushsdbgm 65							//'campaignCheckbox'
      &dcallmp 217							// check()
     label52:
      &pushregister 2
      &pushsdbgm 218							//'PLAY_AS_CAMPAIGN_ENABLE'
      &pushsdb 134							//'0'
      &equals
      &dup
      &jnz label53      
      &pop
      &pushregister 1
      &pushsdbgm 32							//'m_isHost'
      &not
     label53:
      &not
      &jnz label54      
      &pushzero
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 11							//'m_screen'
      &pushsdbgm 12							//'gameSettings'
      &pushsdbgm 13							//'mapPanel'
      &pushsdbgm 65							//'campaignCheckbox'
      &dcallmp 62							// disable()
      &jmp label55      
     label54:
      &pushzero
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 11							//'m_screen'
      &pushsdbgm 12							//'gameSettings'
      &pushsdbgm 13							//'mapPanel'
      &pushsdbgm 65							//'campaignCheckbox'
      &dcallmp 47							// enable()
     label55:
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 164							//'refreshResourcesControl'
    &function2  () ()
      &pushzero
      &pushsdb 30							//'Object'
      &new
      &setRegister r:1
      &pop
      &pushsdb 219							//'QueryGameEngine?RESOURCES_OPTIONS'
      &pushregister 1
      &loadVariables
      &pushzero
      &pushsdb 8							//'Array'
      &new
      &setRegister r:2
      &pop
      &pushsdb 177							//','
      &pushone
      &pushregister 1
      &pushsdbgm 220							//'RESOURCES_OPTIONS_VALUES'
      &pushsdb 179							//'split'
      &callMethod
      &setRegister r:2
      &pop
      &pushzero
      &pushregister 2
      &pushsdb 180							//'shift'
      &callMethod
      &toNumber
      &setRegister r:5
      &pop
      &pushzero
      &pushsdb 8							//'Array'
      &new
      &setRegister r:3
      &pop
      &pushsdb 177							//','
      &pushone
      &pushregister 1
      &pushsdbgm 221							//'RESOURCES_OPTIONS_STRINGS'
      &pushsdb 179							//'split'
      &callMethod
      &setRegister r:3
      &pop
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 11							//'m_screen'
      &pushsdbgm 12							//'gameSettings'
      &pushsdbgm 69							//'rulesPanel'
      &pushsdbgm 74							//'resourcesDropdown'
      &setRegister r:4
      &pop
      &pushregister 2
      &pushregister 3
      &pushbyte 2
      &pushregister 4
      &dcallmp 189							// setData()
      &pushregister 5
      &pushone
      &pushregister 4
      &dcallmp 190							// setSelectedIndex()
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 166							//'refreshRulesCheckbox'
    &function2  (r:2='ruleStr', r:3='checkbox') ()
      &pushzero
      &pushsdb 30							//'Object'
      &new
      &setRegister r:1
      &pop
      &pushsdb 222							//'QueryGameEngine?'
      &pushregister 2
      &add
      &pushregister 1
      &loadVariables
      &pushregister 1
      &pushregister 2
      &pushsdb 223							//'_VALUE'
      &add
      &getMember
      &pushsdb 134							//'0'
      &equals
      &not
      &jnz label56      
      &pushzero
      &pushregister 3
      &dcallmp 216							// unCheck()
      &jmp label57      
     label56:
      &pushzero
      &pushregister 3
      &dcallmp 217							// check()
     label57:
      &pushregister 1
      &pushregister 2
      &pushsdb 224							//'_ENABLE'
      &add
      &getMember
      &pushsdb 134							//'0'
      &equals
      &not
      &jnz label58      
      &pushzero
      &pushregister 3
      &dcallmp 62							// disable()
      &jmp label59      
     label58:
      &pushzero
      &pushregister 3
      &dcallmp 47							// enable()
     label59:
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 169							//'refreshGameSpeedControls'
    &function2  () (r:1='this')
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 11							//'m_screen'
      &pushsdbgm 12							//'gameSettings'
      &pushsdbgm 69							//'rulesPanel'
      &pushsdbgm 73							//'gameSpeedSlider'
      &setRegister r:3
      &pop
      &pushzero
      &pushsdb 30							//'Object'
      &new
      &setRegister r:2
      &pop
      &pushsdb 225							//'QueryGameEngine?GAMESPEED_OPTIONS'
      &pushregister 2
      &loadVariables
      &pushregister 2
      &pushsdbgm 226							//'GAMESPEED_OPTIONS_CURRENT'
      &toNumber
      &pushregister 2
      &pushsdbgm 227							//'GAMESPEED_OPTIONS_MAX'
      &toNumber
      &pushregister 2
      &pushsdbgm 228							//'GAMESPEED_OPTIONS_MIN'
      &toNumber
      &pushbyte 3
      &pushregister 3
      &dcallmp 229							// setIndexRange()
      &pushregister 2
      &pushsdbgm 230							//'GAMESPEED_OPTIONS_ENABLE'
      &pushsdb 134							//'0'
      &equals
      &dup
      &jnz label60      
      &pop
      &pushregister 1
      &pushsdbgm 32							//'m_isHost'
      &not
     label60:
      &not
      &jnz label61      
      &pushzero
      &pushregister 3
      &dcallmp 62							// disable()
      &jmp label62      
     label61:
      &pushzero
      &pushregister 3
      &dcallmp 47							// enable()
     label62:
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 231							//'refreshReadyControls'
    &function2  () (r:1='this')
      &pushzero
      &setRegister r:2
      &pop
     label63:
      &pushregister 2
      &pushregister 1
      &pushsdbgm 88							//'m_playerSlots'
      &pushsdbgm 60							//'length'
      &lessThan
      &not
      &jnz label64      
      &pushregister 1
      &pushsdbgm 88							//'m_playerSlots'
      &pushregister 2
      &getMember
      &pushsdbgm 107							//'readyCheckBox'
      &setRegister r:3
      &pop
      &pushregister 2
      &pushregister 3
      &pushbyte 2
      &pushregister 1
      &dcallmp 232							// refreshReadyCheckbox()
      &pushregister 2
      &increment
      &setRegister r:2
      &pop
      &jmp label63      
     label64:
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 232							//'refreshReadyCheckbox'
    &function2  (r:2='checkbox', r:3='slotId') ()
      &pushzero
      &pushsdb 30							//'Object'
      &new
      &setRegister r:1
      &pop
      &pushsdb 233							//'QueryGameEngine?READY_STATUS?Slot='
      &pushregister 3
      &toString
      &add
      &pushregister 1
      &loadVariables
      &pushregister 1
      &pushsdbgm 234							//'READY_STATUS_ENABLED'
      &pushsdb 34							//'1'
      &equals
      &jnz label65      
      &pushzero
      &pushregister 2
      &pushsdb 62							//'disable'
      &callMethod
      &jmp label66      
     label65:
      &pushzero
      &pushregister 2
      &pushsdb 47							//'enable'
      &callMethod
     label66:
      &pop
      &pushregister 1
      &pushsdbgm 235							//'READY_STATUS_VALUE'
      &pushsdb 34							//'1'
      &equals
      &jnz label67      
      &pushzero
      &pushregister 2
      &pushsdb 236							//'SetUnCheck'
      &callMethod
      &jmp label68      
     label67:
      &pushzero
      &pushregister 2
      &pushsdb 237							//'SetCheck'
      &callMethod
     label68:
      &pop
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 188							//'enableControl'
    &function2  (r:2='enable', r:1='control') ()
      &pushregister 2
      &not
      &jnz label69      
      &pushzero
      &pushregister 1
      &dcallmp 47							// enable()
      &jmp label70      
     label69:
      &pushzero
      &pushregister 1
      &dcallmp 62							// disable()
     label70:
    &end // of function 

    &setMember
    &pushone
    &pushnull
    &pushglobalgv
    &pushsdbgm 1							//'GameSetupBase'
    &pushsdbgm 27							//'prototype'
    &pushbyte 3
    &pushsdb 238							//'ASSetPropFlags'
    &callFunction
   label71:
    &pop
  &end // of initMovieClip 30

  &defineMovieClip 31 // total frames: 0
  &end // of defineMovieClip 31
  
  &exportAssets
    31 &as '__Packages.fes_m_skirmishGameSetup'
  &end // of exportAssets
  
  &initMovieClip 31
    &constants '_global', 'fes_m_skirmishGameSetup', 'fes_m_skirmishGameSetup::fes_m_skirmishGameSetup() screen = ', 'GAME', 'MODE', 'ENUM', 'GAME_MODE_SKIRMISH', 'GameSetupBase', 'prototype', 'frameDelayedInit', 'initSkirmishGUI', 'startIntro', 'Cafe2_BaseUIScreen', 'm_screen', 'gameSettings', 'rulesPanel', 'fes_m_skirmishGameSetup::initSkirmishGUI()', 'commentaryCheckbox', 'hide', 'disable', 'enableVoipCheckbox', 'onLoadButtonClick', 'bind0', 'loadButton', 'setOnMouseUpFunction', 'enable', 'm_screenButtons', 'push', 'registerIntroOuttroComponents', 'SAVELOADSCREEN_TYPE', 'SAVELOADSCREEN_LOAD', 'SCREEN', 'FEG_SAVELOAD', 'gSM', 'changeMainScreen', 'onScreenExit', 'fes_m_skirmishGameSetup::onScreenExit()', 'ASSetPropFlags'  
    &pushglobalgv
    &pushsdbgm 1							//'fes_m_skirmishGameSetup'
    &not
    &not
    &jnz label1    
    &pushglobalgv
    &pushsdb 1							//'fes_m_skirmishGameSetup'
    &function2  (r:3='screen') (r:1='super', r:2='_global')
      &pushregister 3
      &pushone
      &pushregister 1
      &pushundef
      &callmp
      &pushsdb 2							//'fes_m_skirmishGameSetup::fes_m_skirmishGameSetup() screen = '
      &pushregister 3
      &add
      &trace
      &pushregister 2
      &pushsdbgm 3							//'GAME'
      &pushsdb 4							//'MODE'
      &pushregister 2
      &pushsdbgm 5							//'ENUM'
      &pushsdbgm 6							//'GAME_MODE_SKIRMISH'
      &setMember
    &end // of function 

    &setRegister r:1
    &setMember
    &pushglobalgv
    &pushsdbgm 1							//'fes_m_skirmishGameSetup'
    &pushsdbgv 7							//'GameSetupBase'
    &extends
    &pushregister 1
    &pushsdbgm 8							//'prototype'
    &setRegister r:2
    &pop
    &pushregister 2
    &pushsdb 9							//'frameDelayedInit'
    &function2  () (r:1='this', r:2='super')
      &getURL 'FSCommand:CallGameFunction'       '%SetGameSetupMode?Mode=Skirmish'      
      &pushzero
      &pushregister 2
      &dcallmp 9							// frameDelayedInit()
      &pushzero
      &pushregister 1
      &dcallmp 10							// initSkirmishGUI()
      &pushzero
      &pushregister 1
      &dcallmp 11							// startIntro()
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 10							//'initSkirmishGUI'
    &function2  () (r:1='this', r:2='_global')
      &pushsdbgv 12							//'Cafe2_BaseUIScreen'
      &pushsdbgm 13							//'m_screen'
      &pushsdbgm 14							//'gameSettings'
      &pushsdbgm 15							//'rulesPanel'
      &setRegister r:3
      &pop
      &pushsdb 16							//'fes_m_skirmishGameSetup::initSkirmishGUI()'
      &trace
      &pushzero
      &pushregister 3
      &pushsdbgm 17							//'commentaryCheckbox'
      &dcallmp 18							// hide()
      &pushzero
      &pushregister 3
      &pushsdbgm 17							//'commentaryCheckbox'
      &dcallmp 19							// disable()
      &pushzero
      &pushregister 3
      &pushsdbgm 20							//'enableVoipCheckbox'
      &dcallmp 18							// hide()
      &pushzero
      &pushregister 3
      &pushsdbgm 20							//'enableVoipCheckbox'
      &dcallmp 19							// disable()
      &pushregister 1
      &pushsdbgm 21							//'onLoadButtonClick'
      &pushregister 1
      &pushbyte 2
      &pushregister 2
      &pushsdb 22							//'bind0'
      &callMethod
      &pushone
      &pushsdbgv 23							//'loadButton'
      &dcallmp 24							// setOnMouseUpFunction()
      &pushzero
      &pushsdbgv 23							//'loadButton'
      &dcallmp 25							// enable()
      &pushsdbgv 23							//'loadButton'
      &pushone
      &pushregister 1
      &pushsdbgm 26							//'m_screenButtons'
      &dcallmp 27							// push()
      &pushregister 1
      &pushsdbgm 26							//'m_screenButtons'
      &pushone
      &pushregister 1
      &dcallmp 28							// registerIntroOuttroComponents()
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 21							//'onLoadButtonClick'
    &function2  () (r:1='_global')
      &pushregister 1
      &pushsdbgm 3							//'GAME'
      &pushsdb 29							//'SAVELOADSCREEN_TYPE'
      &pushregister 1
      &pushsdbgm 5							//'ENUM'
      &pushsdbgm 30							//'SAVELOADSCREEN_LOAD'
      &setMember
      &pushregister 1
      &pushsdbgm 31							//'SCREEN'
      &pushsdbgm 32							//'FEG_SAVELOAD'
      &pushone
      &pushregister 1
      &pushsdbgm 33							//'gSM'
      &dcallmp 34							// changeMainScreen()
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 35							//'onScreenExit'
    &function2  () (r:1='super', r:2='_global')
      &pushsdb 36							//'fes_m_skirmishGameSetup::onScreenExit()'
      &trace
      &pushzero
      &pushregister 1
      &dcallmp 35							// onScreenExit()
      &pushregister 2
      &pushsdb 1							//'fes_m_skirmishGameSetup'
      &delete
      &pop
    &end // of function 

    &setMember
    &pushone
    &pushnull
    &pushglobalgv
    &pushsdbgm 1							//'fes_m_skirmishGameSetup'
    &pushsdbgm 8							//'prototype'
    &pushbyte 3
    &pushsdb 37							//'ASSetPropFlags'
    &callFunction
   label1:
    &pop
  &end // of initMovieClip 31
&end
