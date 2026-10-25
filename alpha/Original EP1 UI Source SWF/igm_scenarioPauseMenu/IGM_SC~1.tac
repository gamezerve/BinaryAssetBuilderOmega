movie 'E:\depot\CnC\RA3_EP1\PRODUC~1\Data\APTBUI~1\034A3~1.0-D\pc\Output\IGM_SC~1\\IGM_SC~1.eaf' &compressed // flash 7, total frames: 1, frame rate: 30 fps, 1365x768 px

  &frame 0
    &pushs 'screen'
    &pushthisgv
    &pushone
    &pushs 'igm_scenarioPauseMenu'
    &new
    &varEquals
    &stop
  &end // of frame 0

  &defineMovieClip 1 // total frames: 1
  &end // of defineMovieClip 1

  &defineMovieClip 3 // total frames: 1
  &end // of defineMovieClip 3

  &defineMovieClip 7 // total frames: 1
  &end // of defineMovieClip 7
  
  &importAssets &from '../cafe/mouseComponents/std_mouseScrollingListbox.swf'
    'std_mouseScrollingListboxSymbol' &as 8
  &end // of importAssets

  &placeMovieClip 8 &as 'descriptionTextListBox'
  
    &onClipEvent &construct
      &constants 'm_focusDirs', 'Up/Down', 'm_numEntries', 'm_vPadding', 'm_bEnabled', 'm_refFM', '_root.gFM', 'm_initiallySelected', 'm_tabIndex', 'm_bVisible', 'm_scrollbarOffset', 'm_BgVisible', 'm_renderWidth', 'm_renderHeight', 'm_backgroundMargin', 'm_useHighlights', 'm_alternateBackgroundColors', 'm_backgroundColor1', 'm_backgroundColor2'  
      &pushsdb 0							//'m_focusDirs'
      &pushsdb 1							//'Up/Down'
      &setVariable
      &pushsdb 2							//'m_numEntries'
      &pushbyte 6
      &setVariable
      &pushsdb 3							//'m_vPadding'
      &pushbyte 5
      &setVariable
      &pushsdb 4							//'m_bEnabled'
      &pushtrue
      &setVariable
      &pushsdb 5							//'m_refFM'
      &pushsdb 6							//'_root.gFM'
      &setVariable
      &pushsdb 7							//'m_initiallySelected'
      &pushfalse
      &setVariable
      &pushsdb 8							//'m_tabIndex'
      &pushbyte 6
      &setVariable
      &pushsdb 9							//'m_bVisible'
      &pushtrue
      &setVariable
      &pushsdb 10							//'m_scrollbarOffset'
      &pushzero
      &initArray
      &setVariable
      &pushsdbgv 10							//'m_scrollbarOffset'
      &pushzero
      &pushbyte 10
      &setMember
      &pushsdbgv 10							//'m_scrollbarOffset'
      &pushone
      &pushzero
      &setMember
      &pushsdb 11							//'m_BgVisible'
      &pushtrue
      &setVariable
      &pushsdb 12							//'m_renderWidth'
      &pushshort 942
      &setVariable
      &pushsdb 13							//'m_renderHeight'
      &pushshort 158
      &setVariable
      &pushsdb 14							//'m_backgroundMargin'
      &pushbyte 2
      &setVariable
      &pushsdb 15							//'m_useHighlights'
      &pushtrue
      &setVariable
      &pushsdb 16							//'m_alternateBackgroundColors'
      &pushfalse
      &setVariable
      &pushsdb 17							//'m_backgroundColor1'
      &pushzerosv
      &pushsdb 18							//'m_backgroundColor2'
      &pushzerosv
    &end
  &end // of placeMovieClip 8
  
  &importAssets &from '../cafe/mouseComponents/std_mouseScrollbar.swf'
    'std_mouseScrollbarSymbol' &as 9
  &end // of importAssets

  &placeMovieClip 9 &as 'scrollBar'
  
    &onClipEvent &construct
      &pushs 'm_nLength'
      &pushbyte 100
      &setVariable
      &pushs 'm_nMax'
      &pushbyte 10
      &setVariable
      &pushs 'm_nMin'
      &pushzerosv
      &pushs 'm_nValue'
      &pushzerosv
    &end
  &end // of placeMovieClip 9

  &defineMovieClip 12 // total frames: 1
  &end // of defineMovieClip 12

  &defineMovieClip 14 // total frames: 1
  &end // of defineMovieClip 14

  &defineMovieClip 19 // total frames: 1
  &end // of defineMovieClip 19

  &defineMovieClip 23 // total frames: 1
  &end // of defineMovieClip 23

  &defineMovieClip 46 // total frames: 1

    &placeMovieClip 23 
    
      &onClipEvent &construct
        &pushs '_type'
        &pushssv 'RenderImage'
        &pushs '_imageMap'
        &pushssv 'PauseMenu_unitUnlock'
        &pushs '_mode'
        &pushssv 'ALPHA'
      &end
    &end // of placeMovieClip 23
  &end // of defineMovieClip 46

  &defineMovieClip 48 // total frames: 1
  &end // of defineMovieClip 48
  
  &importAssets &from '../cafe/mouseComponents/std_MouseButton.swf'
    'std_MousebuttonSymbol' &as 49
  &end // of importAssets

  &placeMovieClip 49 &as 'resumeButton'
  
    &onClipEvent &construct
      &pushs 'm_focusDirs'
      &pushssv 'Left/Right'
      &pushs 'm_bEnabled'
      &pushtrue
      &setVariable
      &pushs 'm_refFM'
      &pushssv '_root.gFM'
      &pushs 'm_initiallySelected'
      &pushtrue
      &setVariable
      &pushs 'm_tabIndex'
      &pushbyte 2
      &setVariable
      &pushs 'm_bVisible'
      &pushtrue
      &setVariable
      &pushs 'm_label'
      &pushssv '$GUI:TacticalPauseDialogResumeButton'
      &pushs 'm_contentSymbol'
      &pushssv 'buttonContentSymbol'
    &end
  &end // of placeMovieClip 49

  &placeMovieClip 49 &as 'settingsButton'
  
    &onClipEvent &construct
      &pushs 'm_focusDirs'
      &pushssv 'Left/Right'
      &pushs 'm_bEnabled'
      &pushtrue
      &setVariable
      &pushs 'm_refFM'
      &pushssv '_root.gFM'
      &pushs 'm_initiallySelected'
      &pushfalse
      &setVariable
      &pushs 'm_tabIndex'
      &pushone
      &setVariable
      &pushs 'm_bVisible'
      &pushtrue
      &setVariable
      &pushs 'm_label'
      &pushssv '$GUI:TacticalPauseDialogOptionsButton'
      &pushs 'm_contentSymbol'
      &pushssv 'buttonContentSymbol'
    &end
  &end // of placeMovieClip 49

  &placeMovieClip 49 &as 'exitButton'
  
    &onClipEvent &construct
      &pushs 'm_focusDirs'
      &pushssv 'Left/Right'
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
      &pushs 'm_label'
      &pushssv '$GUI:TacticalPauseDialogExitMissionButton'
      &pushs 'm_contentSymbol'
      &pushssv 'buttonContentSymbol'
    &end
  &end // of placeMovieClip 49

  &defineMovieClip 50 // total frames: 0
  &end // of defineMovieClip 50
  
  &exportAssets
    50 &as '__Packages.Cafe2_BaseUIScreen'
  &end // of exportAssets
  
  &initMovieClip 50
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

    &setRegister r:0
    &setMember
    &pushregister 0
    &pushsdbgm 6							//'prototype'
    &setRegister r:1
    &pop
    &pushregister 1
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
    &pushregister 1
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
    &pushregister 1
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
    &pushregister 1
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
    &pushregister 1
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
    &pushregister 1
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
    &pushregister 1
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
    &pushregister 1
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
    &pushregister 1
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
    &pushregister 1
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
    &pushregister 1
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
    &pushregister 1
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
    &pushregister 1
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
    &pushregister 1
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
    &pushregister 1
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
    &pushregister 1
    &pushsdb 18							//'m_changeToScreen'
    &pushnull
    &setMember
    &pushregister 1
    &pushsdb 44							//'m_executeOuttroCallBack'
    &pushnull
    &setMember
    &pushregister 1
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
  &end // of initMovieClip 50

  &defineMovieClip 51 // total frames: 0
  &end // of defineMovieClip 51
  
  &exportAssets
    51 &as '__Packages.igm_scenarioPauseMenu'
  &end // of exportAssets
  
  &initMovieClip 51
    &constants '_global', 'igm_scenarioPauseMenu', 'igm_scenarioPauseMenu::igm_scenarioPauseMenu() screen = ', 'Cafe2_BaseUIScreen', 'm_screen', 'm_thisClass', 'initGUI', 'initData', 'prototype', 'initButton', '_visible', 'enable', 'disable', 'setOnMouseUpFunction', 'initButtons', 'onExitButtonClicked', 'bind0', 'exitButton', 'onSettingsButtonClicked', 'settingsButton', 'backButtonClicked', 'resumeButton', 'submitMissionMap', 'igm_scenarioPauseMenu::submitMissionMap()', 'FSCommand:CallGameFunction', '%PauseMenu_receiveMissionMap?missionMap=', 'missionMapMC', 'Pointer', 'encode', 'OnBound', 'igm_scenarioPauseMenu::initGUI()', 'igm_pauseMenuShared.swf', 'backgroundMC', 'loadMovie', '../../common/ig_scenarioMissionMap.swf', 'm_localMessageHandlerFunc', 'localMessageHandler', 'bind1DynamicParams', 'gMH', 'addMessageHandler', 'm_localEventHandlerIndex', 'localEventHandler', 'bind2DynamicParams', 'gEH', 'addLocalEventHandler', 'registerIntroOuttroComponents', 'startIntro', 'onScreenExit', 'gSM', 'setOnExitScreen', 'scrollBar', 'show', 'descriptionTextListBox', 'setVerticalPadding', 'addListener', 'disableEntryHitRegion', 'setNumEntries', 'Array', 'symbol', 'std_mouseListboxEntryTextElementSymbol', 'width', 'leftMargin', 'rightMargin', 'useHighlights', 'isSelectable', 'useHitRegion', 'autoSize', 'push', 'init', 'attachScrollbarController', 'Object', 'QueryGameEngine?PauseMenu_scenarioDesc', '$', 'PauseMenu_scenarioDesc', 'setMissionDescriptionText', 'igm_scenarioPauseMenu::onScreenExit()', 'OnDestroying', 'removeLocalEventHandler', 'removeMessageHandler', 'clearData', 'igm_scenarioPauseMenu::localMessageHandler() messageCode = \'', '\'', 'MSGCODE', 'IGM_CLOSE_PAUSE_MENU', 'IG_PAUSE_MENU_SUBMIT_MISSION_MAP', 'igm_scenarioPauseMenu::localEventHandler() keyCode == ', 'INPUTCODE', 'ESCAPE', 'gFM', 'handleDefaultInputs', 'igm_scenarioPauseMenu::onExitButtonClicked()', 'igm_scenarioPauseMenu::onSettingsButtonClicked()', 'SCREEN', 'FEG_OPTIONS', 'startOuttro', ',', 'split', 'setColumnData', 'setSelectedIndex', 'refreshDisplay', 'refreshScrollbar', 'moveViewToFirst', 'ASSetPropFlags'  
    &pushglobalgv
    &pushsdbgm 1							//'igm_scenarioPauseMenu'
    &not
    &not
    &jnz label9    
    &pushglobalgv
    &pushsdb 1							//'igm_scenarioPauseMenu'
    &function2  (r:3='screen') (r:1='this', r:2='super')
      &pushzero
      &pushregister 2
      &pushundef
      &callmp
      &pushsdb 2							//'igm_scenarioPauseMenu::igm_scenarioPauseMenu() screen = '
      &pushregister 3
      &add
      &trace
      &pushsdbgv 3							//'Cafe2_BaseUIScreen'
      &pushsdb 4							//'m_screen'
      &pushregister 3
      &setMember
      &pushsdbgv 3							//'Cafe2_BaseUIScreen'
      &pushsdb 5							//'m_thisClass'
      &pushregister 1
      &setMember
      &pushzero
      &pushregister 1
      &dcallmp 6							// initGUI()
      &pushzero
      &pushregister 1
      &dcallmp 7							// initData()
    &end // of function 

    &setRegister r:0
    &setMember
    &pushglobalgv
    &pushsdbgm 1							//'igm_scenarioPauseMenu'
    &pushsdbgv 3							//'Cafe2_BaseUIScreen'
    &extends
    &pushregister 0
    &pushsdbgm 8							//'prototype'
    &setRegister r:1
    &pop
    &pushregister 1
    &pushsdb 9							//'initButton'
    &function2  (r:1='bttn', r:3='enable', r:2='onClickFn') ()
      &pushregister 1
      &pushsdb 10							//'_visible'
      &pushtrue
      &setMember
      &pushregister 3
      &not
      &jnz label1      
      &pushzero
      &pushregister 1
      &dcallmp 11							// enable()
      &jmp label2      
     label1:
      &pushzero
      &pushregister 1
      &dcallmp 12							// disable()
     label2:
      &pushregister 2
      &pushone
      &pushregister 1
      &dcallmp 13							// setOnMouseUpFunction()
    &end // of function 

    &setMember
    &pushregister 1
    &pushsdb 14							//'initButtons'
    &function2  () (r:1='this', r:2='_global')
      &pushregister 1
      &pushsdbgm 15							//'onExitButtonClicked'
      &pushregister 1
      &pushbyte 2
      &pushregister 2
      &pushsdb 16							//'bind0'
      &callMethod
      &pushtrue
      &pushsdbgv 3							//'Cafe2_BaseUIScreen'
      &pushsdbgm 4							//'m_screen'
      &pushsdbgm 17							//'exitButton'
      &pushbyte 3
      &pushregister 1
      &dcallmp 9							// initButton()
      &pushregister 1
      &pushsdbgm 18							//'onSettingsButtonClicked'
      &pushregister 1
      &pushbyte 2
      &pushregister 2
      &pushsdb 16							//'bind0'
      &callMethod
      &pushtrue
      &pushsdbgv 3							//'Cafe2_BaseUIScreen'
      &pushsdbgm 4							//'m_screen'
      &pushsdbgm 19							//'settingsButton'
      &pushbyte 3
      &pushregister 1
      &dcallmp 9							// initButton()
      &pushregister 1
      &pushsdbgm 20							//'backButtonClicked'
      &pushregister 1
      &pushbyte 2
      &pushregister 2
      &pushsdb 16							//'bind0'
      &callMethod
      &pushtrue
      &pushsdbgv 3							//'Cafe2_BaseUIScreen'
      &pushsdbgm 4							//'m_screen'
      &pushsdbgm 21							//'resumeButton'
      &pushbyte 3
      &pushregister 1
      &dcallmp 9							// initButton()
    &end // of function 

    &setMember
    &pushregister 1
    &pushsdb 22							//'submitMissionMap'
    &function  (    )    
      &pushsdb 23							//'igm_scenarioPauseMenu::submitMissionMap()'
      &trace
      &pushsdb 24							//'FSCommand:CallGameFunction'
      &pushsdb 25							//'%PauseMenu_receiveMissionMap?missionMap='
      &pushsdbgv 3							//'Cafe2_BaseUIScreen'
      &pushsdbgm 4							//'m_screen'
      &pushsdbgm 26							//'missionMapMC'
      &pushone
      &pushsdbgv 27							//'Pointer'
      &pushsdb 28							//'encode'
      &callMethod
      &add
      &getURL2
      &pushzero
      &pushsdbgv 3							//'Cafe2_BaseUIScreen'
      &pushsdbgm 4							//'m_screen'
      &pushsdbgm 26							//'missionMapMC'
      &dcallmp 29							// OnBound()
    &end // of function 

    &setMember
    &pushregister 1
    &pushsdb 6							//'initGUI'
    &function2  () (r:1='this', r:2='_global')
      &pushsdb 30							//'igm_scenarioPauseMenu::initGUI()'
      &trace
      &pushzero
      &pushregister 1
      &dcallmp 14							// initButtons()
      &pushsdb 31							//'igm_pauseMenuShared.swf'
      &pushone
      &pushsdbgv 3							//'Cafe2_BaseUIScreen'
      &pushsdbgm 4							//'m_screen'
      &pushsdbgm 32							//'backgroundMC'
      &dcallmp 33							// loadMovie()
      &pushsdb 34							//'../../common/ig_scenarioMissionMap.swf'
      &pushone
      &pushsdbgv 3							//'Cafe2_BaseUIScreen'
      &pushsdbgm 4							//'m_screen'
      &pushsdbgm 26							//'missionMapMC'
      &dcallmp 33							// loadMovie()
      &pushzero
      &pushregister 1
      &dcallmp 22							// submitMissionMap()
      &pushregister 1
      &pushsdb 35							//'m_localMessageHandlerFunc'
      &pushregister 1
      &pushsdbgm 36							//'localMessageHandler'
      &pushregister 1
      &pushbyte 2
      &pushregister 2
      &pushsdb 37							//'bind1DynamicParams'
      &callMethod
      &setMember
      &pushregister 1
      &pushsdbgm 35							//'m_localMessageHandlerFunc'
      &pushone
      &pushregister 2
      &pushsdbgm 38							//'gMH'
      &dcallmp 39							// addMessageHandler()
      &pushregister 1
      &pushsdb 40							//'m_localEventHandlerIndex'
      &pushregister 1
      &pushsdbgm 41							//'localEventHandler'
      &pushregister 1
      &pushbyte 2
      &pushregister 2
      &pushsdb 42							//'bind2DynamicParams'
      &callMethod
      &pushone
      &pushregister 2
      &pushsdbgm 43							//'gEH'
      &pushsdb 44							//'addLocalEventHandler'
      &callMethod
      &setMember
      &pushsdbgv 3							//'Cafe2_BaseUIScreen'
      &pushsdbgm 4							//'m_screen'
      &pushsdbgm 21							//'resumeButton'
      &pushsdbgv 3							//'Cafe2_BaseUIScreen'
      &pushsdbgm 4							//'m_screen'
      &pushsdbgm 19							//'settingsButton'
      &pushsdbgv 3							//'Cafe2_BaseUIScreen'
      &pushsdbgm 4							//'m_screen'
      &pushsdbgm 17							//'exitButton'
      &pushbyte 3
      &initArray
      &pushone
      &pushregister 1
      &dcallmp 45							// registerIntroOuttroComponents()
      &pushzero
      &pushregister 1
      &dcallmp 46							// startIntro()
      &pushregister 1
      &pushsdbgm 47							//'onScreenExit'
      &pushregister 1
      &pushbyte 2
      &pushregister 2
      &pushsdb 16							//'bind0'
      &callMethod
      &pushone
      &pushregister 2
      &pushsdbgm 48							//'gSM'
      &dcallmp 49							// setOnExitScreen()
      &pushzero
      &pushsdbgv 3							//'Cafe2_BaseUIScreen'
      &pushsdbgm 4							//'m_screen'
      &pushsdbgm 50							//'scrollBar'
      &dcallmp 51							// show()
      &pushzero
      &pushsdbgv 3							//'Cafe2_BaseUIScreen'
      &pushsdbgm 4							//'m_screen'
      &pushsdbgm 50							//'scrollBar'
      &dcallmp 11							// enable()
      &pushzero
      &pushone
      &pushsdbgv 3							//'Cafe2_BaseUIScreen'
      &pushsdbgm 4							//'m_screen'
      &pushsdbgm 52							//'descriptionTextListBox'
      &dcallmp 53							// setVerticalPadding()
      &pushregister 1
      &pushone
      &pushsdbgv 3							//'Cafe2_BaseUIScreen'
      &pushsdbgm 4							//'m_screen'
      &pushsdbgm 52							//'descriptionTextListBox'
      &dcallmp 54							// addListener()
      &pushzero
      &pushsdbgv 3							//'Cafe2_BaseUIScreen'
      &pushsdbgm 4							//'m_screen'
      &pushsdbgm 52							//'descriptionTextListBox'
      &dcallmp 55							// disableEntryHitRegion()
      &pushone
      &pushone
      &pushsdbgv 3							//'Cafe2_BaseUIScreen'
      &pushsdbgm 4							//'m_screen'
      &pushsdbgm 52							//'descriptionTextListBox'
      &dcallmp 56							// setNumEntries()
      &pushzero
      &pushsdb 57							//'Array'
      &new
      &setRegister r:3
      &pop
      &pushsdb 58							//'symbol'
      &pushsdb 59							//'std_mouseListboxEntryTextElementSymbol'
      &pushsdb 60							//'width'
      &pushshort 900
      &pushsdb 61							//'leftMargin'
      &pushbyte 10
      &pushsdb 62							//'rightMargin'
      &pushbyte 60
      &pushsdb 63							//'useHighlights'
      &pushfalse
      &pushsdb 64							//'isSelectable'
      &pushfalse
      &pushsdb 65							//'useHitRegion'
      &pushfalse
      &pushsdb 66							//'autoSize'
      &pushtrue
      &pushbyte 8
      &initObject
      &pushone
      &pushregister 3
      &dcallmp 67							// push()
      &pushbyte 29
      &pushregister 3
      &pushbyte 2
      &pushsdbgv 3							//'Cafe2_BaseUIScreen'
      &pushsdbgm 4							//'m_screen'
      &pushsdbgm 52							//'descriptionTextListBox'
      &dcallmp 68							// init()
      &pushsdbgv 3							//'Cafe2_BaseUIScreen'
      &pushsdbgm 4							//'m_screen'
      &pushsdbgm 50							//'scrollBar'
      &pushone
      &pushsdbgv 3							//'Cafe2_BaseUIScreen'
      &pushsdbgm 4							//'m_screen'
      &pushsdbgm 52							//'descriptionTextListBox'
      &dcallmp 69							// attachScrollbarController()
    &end // of function 

    &setMember
    &pushregister 1
    &pushsdb 7							//'initData'
    &function2  () (r:1='this')
      &pushzero
      &pushsdb 70							//'Object'
      &new
      &setRegister r:2
      &pop
      &pushsdb 71							//'QueryGameEngine?PauseMenu_scenarioDesc'
      &pushregister 2
      &loadVariables
      &pushsdb 72							//'$'
      &pushregister 2
      &pushsdbgm 73							//'PauseMenu_scenarioDesc'
      &add
      &pushone
      &pushregister 1
      &dcallmp 74							// setMissionDescriptionText()
    &end // of function 

    &setMember
    &pushregister 1
    &pushsdb 47							//'onScreenExit'
    &function2  () (r:1='this', r:2='_global')
      &pushsdb 75							//'igm_scenarioPauseMenu::onScreenExit()'
      &trace
      &pushzero
      &pushsdbgv 3							//'Cafe2_BaseUIScreen'
      &pushsdbgm 4							//'m_screen'
      &pushsdbgm 26							//'missionMapMC'
      &dcallmp 76							// OnDestroying()
      &pushregister 1
      &pushsdbgm 40							//'m_localEventHandlerIndex'
      &pushone
      &pushregister 2
      &pushsdbgm 43							//'gEH'
      &dcallmp 77							// removeLocalEventHandler()
      &pushregister 1
      &pushsdbgm 35							//'m_localMessageHandlerFunc'
      &pushone
      &pushregister 2
      &pushsdbgm 38							//'gMH'
      &dcallmp 78							// removeMessageHandler()
      &pushregister 1
      &pushsdb 35							//'m_localMessageHandlerFunc'
      &delete
      &pop
      &pushzero
      &pushregister 1
      &dcallmp 79							// clearData()
      &pushsdbgv 3							//'Cafe2_BaseUIScreen'
      &pushsdb 4							//'m_screen'
      &pushnull
      &setMember
      &pushsdbgv 3							//'Cafe2_BaseUIScreen'
      &pushsdb 5							//'m_thisClass'
      &pushnull
      &setMember
      &pushregister 2
      &pushsdb 1							//'igm_scenarioPauseMenu'
      &delete
      &pop
      &pushregister 2
      &pushsdb 3							//'Cafe2_BaseUIScreen'
      &delete
      &pop
    &end // of function 

    &setMember
    &pushregister 1
    &pushsdb 36							//'localMessageHandler'
    &function2  (r:3='messageCode') (r:1='this', r:2='_global')
      &pushsdb 80							//'igm_scenarioPauseMenu::localMessageHandler() messageCode = \''
      &pushregister 3
      &add
      &pushsdb 81							//'\''
      &add
      &trace
      &pushregister 3
      &setRegister r:0
      &pushregister 2
      &pushsdbgm 82							//'MSGCODE'
      &pushsdbgm 83							//'IGM_CLOSE_PAUSE_MENU'
      &strictEquals
      &jnz label3      
      &pushregister 0
      &pushregister 2
      &pushsdbgm 82							//'MSGCODE'
      &pushsdbgm 84							//'IG_PAUSE_MENU_SUBMIT_MISSION_MAP'
      &strictEquals
      &jnz label4      
      &jmp label5      
     label3:
      &pushzero
      &pushregister 1
      &dcallmp 20							// backButtonClicked()
      &pushtrue
      &return
     label4:
      &pushzero
      &pushregister 1
      &dcallmp 22							// submitMissionMap()
      &pushtrue
      &return
     label5:
      &pushfalse
      &return
    &end // of function 

    &setMember
    &pushregister 1
    &pushsdb 41							//'localEventHandler'
    &function2  (r:3='keyCode', 'controller') (r:1='this', r:2='_global')
      &pushsdb 85							//'igm_scenarioPauseMenu::localEventHandler() keyCode == '
      &pushregister 3
      &add
      &trace
      &pushregister 3
      &pushsdbgv 86							//'INPUTCODE'
      &pushsdbgm 87							//'ESCAPE'
      &strictEquals
      &jnz label6      
      &jmp label7      
     label6:
      &pushzero
      &pushregister 1
      &dcallmp 20							// backButtonClicked()
      &jmp label8      
     label7:
      &pushregister 3
      &pushone
      &pushregister 2
      &pushsdbgm 88							//'gFM'
      &dcallmp 89							// handleDefaultInputs()
      &jmp label8      
     label8:
    &end // of function 

    &setMember
    &pushregister 1
    &pushsdb 15							//'onExitButtonClicked'
    &function  (    )    
      &pushsdb 90							//'igm_scenarioPauseMenu::onExitButtonClicked()'
      &trace
      &getURL 'FSCommand:CallGameFunction'       '%igm_pauseMenu_onExitClicked'      
    &end // of function 

    &setMember
    &pushregister 1
    &pushsdb 18							//'onSettingsButtonClicked'
    &function2  () (r:1='this', r:2='_global')
      &pushsdb 91							//'igm_scenarioPauseMenu::onSettingsButtonClicked()'
      &trace
      &pushregister 2
      &pushsdbgm 92							//'SCREEN'
      &pushsdbgm 93							//'FEG_OPTIONS'
      &pushone
      &pushregister 1
      &dcallmp 94							// startOuttro()
    &end // of function 

    &setMember
    &pushregister 1
    &pushsdb 74							//'setMissionDescriptionText'
    &function2  (r:2='missionDescription') ()
      &pushsdb 95							//','
      &pushone
      &pushregister 2
      &pushsdb 96							//'split'
      &callMethod
      &setRegister r:1
      &pop
      &pushregister 1
      &pushzero
      &pushbyte 2
      &pushsdbgv 3							//'Cafe2_BaseUIScreen'
      &pushsdbgm 4							//'m_screen'
      &pushsdbgm 52							//'descriptionTextListBox'
      &dcallmp 97							// setColumnData()
      &pushzero
      &pushone
      &pushsdbgv 3							//'Cafe2_BaseUIScreen'
      &pushsdbgm 4							//'m_screen'
      &pushsdbgm 52							//'descriptionTextListBox'
      &dcallmp 98							// setSelectedIndex()
      &pushzero
      &pushsdbgv 3							//'Cafe2_BaseUIScreen'
      &pushsdbgm 4							//'m_screen'
      &pushsdbgm 52							//'descriptionTextListBox'
      &dcallmp 99							// refreshDisplay()
      &pushzero
      &pushsdbgv 3							//'Cafe2_BaseUIScreen'
      &pushsdbgm 4							//'m_screen'
      &pushsdbgm 52							//'descriptionTextListBox'
      &dcallmp 100							// refreshScrollbar()
      &pushzero
      &pushsdbgv 3							//'Cafe2_BaseUIScreen'
      &pushsdbgm 4							//'m_screen'
      &pushsdbgm 52							//'descriptionTextListBox'
      &dcallmp 101							// moveViewToFirst()
    &end // of function 

    &setMember
    &pushone
    &pushnull
    &pushglobalgv
    &pushsdbgm 1							//'igm_scenarioPauseMenu'
    &pushsdbgm 8							//'prototype'
    &pushbyte 3
    &pushsdb 102							//'ASSetPropFlags'
    &callFunction
   label9:
    &pop
  &end // of initMovieClip 51
&end
