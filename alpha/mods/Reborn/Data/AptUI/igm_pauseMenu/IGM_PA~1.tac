movie 'E:\depot\CnC\RA3_EP1\PRODUC~1\Data\APTBUI~1\034A3~1.0-D\pc\Output\IGM_PA~1\\IGM_PA~1.eaf' &compressed // flash 7, total frames: 1, frame rate: 30 fps, 1365x768 px

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

  &defineMovieClip 10 // total frames: 1
  &end // of defineMovieClip 10

  &defineMovieClip 13 // total frames: 1
  &end // of defineMovieClip 13

  &defineMovieClip 16 // total frames: 30

    &frame 9
      &stop
    &end // of frame 9

    &frame 19
      &stop
    &end // of frame 19

    &frame 29
      &stop
    &end // of frame 29
  &end // of defineMovieClip 16

  &defineMovieClip 17 // total frames: 1

    &placeMovieClip 4 &as 'spacerMC'
    
      &onClipEvent &load
        &pushs ''
        &pushbyte 7
        &pushfalse
        &setProperty
      &end
    &end // of placeMovieClip 4
  &end // of defineMovieClip 17
  
  &exportAssets
    17 &as 'ObjectiveLine'
  &end // of exportAssets

  &frame 0
    &pushs 'screen'
    &pushthisgv
    &pushone
    &pushs 'igm_pauseMenu'
    &new
    &varEquals
    &stop
  &end // of frame 0

  &defineMovieClip 18 // total frames: 1
  &end // of defineMovieClip 18
  
  &importAssets &from '../cafe/mouseComponents/std_MouseButton.swf'
    'std_MousebuttonSymbol' &as 24
  &end // of importAssets

  &placeMovieClip 24 &as 'resumeButton'
  
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
      &pushbyte 4
      &setVariable
      &pushs 'm_bVisible'
      &pushtrue
      &setVariable
      &pushs 'm_label'
      &pushssv '$GUI:TacticalPauseDialogResumeButton'
      &pushs 'm_contentSymbol'
      &pushssv 'buttonContentSymbol'
    &end
  &end // of placeMovieClip 24

  &placeMovieClip 24 &as 'settingsButton'
  
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
      &pushbyte 3
      &setVariable
      &pushs 'm_bVisible'
      &pushtrue
      &setVariable
      &pushs 'm_label'
      &pushssv '$GUI:TacticalPauseDialogOptionsButton'
      &pushs 'm_contentSymbol'
      &pushssv 'buttonContentSymbol'
    &end
  &end // of placeMovieClip 24

  &placeMovieClip 24 &as 'loadButton'
  
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
      &pushbyte 2
      &setVariable
      &pushs 'm_bVisible'
      &pushtrue
      &setVariable
      &pushs 'm_label'
      &pushssv '$GUI:TacticalPauseDialogLoadButton'
      &pushs 'm_contentSymbol'
      &pushssv 'buttonContentSymbol'
    &end
  &end // of placeMovieClip 24

  &placeMovieClip 24 &as 'saveButton'
  
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
      &pushssv '$GUI:TacticalPauseDialogSaveButton'
      &pushs 'm_contentSymbol'
      &pushssv 'buttonContentSymbol'
    &end
  &end // of placeMovieClip 24

  &placeMovieClip 24 &as 'exitButton'
  
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
  &end // of placeMovieClip 24
  
  &importAssets &from '../cafe/mouseComponents/std_mouseScrollbar.swf'
    'std_mouseScrollbarSymbol' &as 31
  &end // of importAssets

  &defineMovieClip 32 // total frames: 1
  &end // of defineMovieClip 32

  &defineMovieClip 34 // total frames: 1

    &placeMovieClip 31 &as 'objectiveListScrollBarMC'
    
      &onClipEvent &construct
        &pushs 'm_nLength'
        &pushbyte 100
        &setVariable
        &pushs 'm_nMax'
        &pushbyte 19
        &setVariable
        &pushs 'm_nMin'
        &pushzerosv
        &pushs 'm_nValue'
        &pushzerosv
      &end
    &end // of placeMovieClip 31
  &end // of defineMovieClip 34

  &defineMovieClip 38 // total frames: 1
  &end // of defineMovieClip 38

  &defineMovieClip 41 // total frames: 1
  &end // of defineMovieClip 41

  &defineMovieClip 43 // total frames: 1
  &end // of defineMovieClip 43

  &defineMovieClip 51 // total frames: 30

    &frame 9
      &stop
    &end // of frame 9

    &frame 19
      &stop
    &end // of frame 19

    &frame 29
      &stop
    &end // of frame 29
  &end // of defineMovieClip 51

  &defineMovieClip 57 // total frames: 1
  &end // of defineMovieClip 57

  &defineButton 58
  
    &on     &overUpToOverDown
      &gotoLabel '_down'
      &play
    &end
  
    &on     &overDownToOverUp
      &gotoLabel '_over'
      &play
      &pushzero
      &pushs 'onClicked'
      &callfp
    &end
  
    &on     &idleToOverUp
      &gotoLabel '_over'
      &play
    &end
  
    &on     &overUpToIdle
      &gotoLabel '_up'
      &play
    &end
  &end // of defineButton 58

  &defineMovieClip 65 // total frames: 40

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
  &end // of defineMovieClip 65

  &defineMovieClip 66 // total frames: 1
  &end // of defineMovieClip 66

  &defineMovieClip 67 // total frames: 1
  &end // of defineMovieClip 67

  &defineMovieClip 68 // total frames: 1
  &end // of defineMovieClip 68

  &defineMovieClip 76 // total frames: 0
  &end // of defineMovieClip 76
  
  &exportAssets
    76 &as '__Packages.Cafe2_BaseUIScreen'
  &end // of exportAssets
  
  &initMovieClip 76
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
  &end // of initMovieClip 76

  &defineMovieClip 77 // total frames: 0
  &end // of defineMovieClip 77
  
  &exportAssets
    77 &as '__Packages.IScrollbarEventListener'
  &end // of exportAssets
  
  &initMovieClip 77
    &constants '_global', 'IScrollbarEventListener'  
    &pushglobalgv
    &pushsdbgm 1							//'IScrollbarEventListener'
    &not
    &not
    &jnz label1    
    &pushglobalgv
    &pushsdb 1							//'IScrollbarEventListener'
    &function  (    )    
    &end // of function 

    &setMember
   label1:
    &pop
  &end // of initMovieClip 77

  &defineMovieClip 78 // total frames: 0
  &end // of defineMovieClip 78
  
  &exportAssets
    78 &as '__Packages.igm_pauseMenu'
  &end // of exportAssets
  
  &initMovieClip 78
    &constants '_global', 'igm_pauseMenu', 'igm_pauseMenu::igm_pauseMenu() screen = ', 'Cafe2_BaseUIScreen', 'm_screen', 'm_thisClass', 'm_objectiveListVisibleHeight', 'campaignMC', 'objectiveListMC', 'maskMC', '_height', 'initGUI', 'prototype', 'IScrollbarEventListener', 'OnScrollbarDelta', 'igm_pauseMenu::OnScrollbarDelta( ', ' )', 'scrollUp', 'scrollDown', 'OnScrollbarThumbUpdate', 'igm_pauseMenu::OnScrollbarThumbUpdate( ', 'm_campaignMC', 'linesMC', 'scrollObjectiveListBox', 'OnScrollbarMouseEnter', 'igm_pauseMenu::OnScrollbarMouseEnter()', 'OnScrollbarMouseExit', 'igm_pauseMenu::OnScrollbarMouseExit()', 'initMode', 'Object', 'QueryGameEngine?PauseMenu_mode', 'igm_pauseMenu: data.PauseMenu_mode = "', 'PauseMenu_mode', '"', 'm_mode', 'initButton', '_visible', 'enable', 'disable', 'setOnMouseUpFunction', 'initButtons', 'QueryGameEngine?igm_pauseMenu_enableSave', 'igm_pauseMenu_enableSave', '0', 'QueryGameEngine?igm_pauseMenu_enableLoad', 'igm_pauseMenu_enableLoad', 'onExitButtonClicked', 'bind0', 'exitButton', 'onSettingsButtonClicked', 'settingsButton', 'onLoadButtonClicked', 'loadButton', 'onSaveButtonClicked', 'saveButton', 'backButtonClicked', 'resumeButton', 'getObjectiveLineColor', '_root', 'GetCurrentFaction', 'Yuriko', 'clearObjectiveListBox', 'igm_pauseMenu::clearObjectiveListBox()', 'm_objectiveLines', 'length', 'pop', 'hitRegionMC', 'bttn', 'onRelease', 'removeMovieClip', 'm_selectedObjectiveLineIndex', 'objectiveDescTF', 'text', '', 'colorizeMovieClip', 'Color', 'setRGB', 'populateObjectiveListBox', 'igm_pauseMenu::populateObjectiveListBox()', 'QueryGameEngine?PauseMenu_missionObjectiveId&PauseMenu_missionObjectiveStatus&PauseMenu_missionObjectiveBonusness', 'igm_pauseMenu: PauseMenu_missionObjectiveId = "', 'PauseMenu_missionObjectiveId', 'igm_pauseMenu: PauseMenu_missionObjectiveStatus = "', 'PauseMenu_missionObjectiveStatus', 'igm_pauseMenu: PauseMenu_missionObjectiveBonusness = "', 'PauseMenu_missionObjectiveBonusness', ',', 'split', 'ObjectiveLine', 'attachMovie', 'objectiveId', 'textTF', '$PauseMenu_objectiveTitle', '&outline', 'autoSize', 'left', 'spacerMC', '_y', 'textColor', 'checkboxMC', 'gotoAndPlay', 'frameMC', 'checkMC', '_alpha', 'onObjectiveLineClicked', 'bind1', 'push', 'objectiveListScrollBarMC', 'setInfo', 'selectObjectiveListBoxEntry', 'igm_pauseMenu::scrollObjectiveListBox( ', 'ensureObjectiveLineVisibility', 'igm_pauseMenu::ensureObjectiveLineVisibility( ', 'refreshPlayerStatus', 'MAX_PLAYERS', 'MovieClip', 'm_playerLineMCs', 'statusMC', 'statusTF', '$PauseMenu_playerStatus', 'igm_pauseMenu::selectObjectiveListBoxEntry()', 'QueryGameEngine?PauseMenu_selectedMissionObjectiveId', 'igm_pauseMenu: PauseMenu_selectedMissionObjectiveId = "', 'PauseMenu_selectedMissionObjectiveId', '$PauseMenu_objectiveDesc', 'submitMissionMap', 'igm_pauseMenu::submitMissionMap()', 'FSCommand:CallGameFunction', '%PauseMenu_receiveMissionMap?missionMap=', 'missionMap', 'Pointer', 'encode', 'OnBound', 'refreshPlayerMuted', 'igm_pauseMenu::refreshPlayerMuted()', 'QueryGameEngine?PauseMenu_playerMuted', 'igm_pauseMenu: PauseMenu_playerMuted = "', 'PauseMenu_playerMuted', 'm_playerCount', 'muteButtonMC', 'muteMC', 'initPlayerList', 'igm_pauseMenu::initPlayerList()', 'Array', 'm_skirmishMC', 'playerListMC', 'nameMC', 'nameTF', '$PauseMenu_playerName', 'onClicked', 'onPlayerMuteButtonClicked', 'QueryGameEngine?PauseMenu_playerFaction&PauseMenu_playerColor&PauseMenu_playerIsNonLocalHuman&PauseMenu_playerMuted', 'igm_pauseMenu: PauseMenu_playerFaction = "', 'PauseMenu_playerFaction', 'igm_pauseMenu: PauseMenu_playerColor = "', 'PauseMenu_playerColor', 'igm_pauseMenu: PauseMenu_playerIsNonLocalHuman = "', 'PauseMenu_playerIsNonLocalHuman', 'Math', 'min', 'factionMC', 'colorMC', '_disabled', '_up', 'igm_pauseMenu::initGUI()', 'igm_pauseMenuShared.swf', 'backgroundMC', 'loadMovie', '../../common/ig_missionMap.swf', 'skirmishMC', 'Campaign', 'Skirmish', 'addListener', 'setSize', 'igm_pauseMenu: unrecognized mode "', 'm_localMessageHandlerFunc', 'localMessageHandler', 'bind1DynamicParams', 'gMH', 'addMessageHandler', 'm_localEventHandlerIndex', 'localEventHandler', 'bind2DynamicParams', 'gEH', 'addLocalEventHandler', 'registerIntroOuttroComponents', 'startIntro', 'onScreenExit', 'gSM', 'setOnExitScreen', 'OnContentLoaded', 'onContentLoaded', 'highlightedComponent', 'igm_pauseMenu::onScreenExit()', 'missionMapObjectiveLayerMC', 'OnDestroying', 'removeLocalEventHandler', 'removeMessageHandler', 'clearData', 'igm_pauseMenu::localMessageHandler() messageCode = \'', '\'', 'MSGCODE', 'IGM_CLOSE_PAUSE_MENU', 'IG_PAUSE_MENU_REFRESH_OBJECTIVES', 'IG_PAUSE_MENU_SUBMIT_MISSION_MAP', 'IG_PAUSE_MENU_SELECT_OBJECTIVE', 'IG_PAUSE_MENU_REFRESH_PLAYER_MUTED', 'IG_PAUSE_MENU_REFRESH_PLAYER_STATUS', 'igm_pauseMenu::localEventHandler() keyCode == ', 'INPUTCODE', 'ESCAPE', 'gFM', 'handleDefaultInputs', 'igm_pauseMenu::onExitButtonClicked()', 'igm_pauseMenu::onSettingsButtonClicked()', 'SCREEN', 'FEG_OPTIONS', 'startOuttro', 'igm_pauseMenu::onLoadButtonClicked()', 'GAME', 'SAVELOADSCREEN_TYPE', 'ENUM', 'SAVELOADSCREEN_LOAD', 'FEG_SAVELOAD', 'igm_pauseMenu::onSaveButtonClicked()', 'SAVELOADSCREEN_SAVE', 'igm_pauseMenu::onObjectiveLineClicked( ', '%PauseMenu_onMissionObjectiveClicked?objectiveId=', 'igm_pauseMenu::onPlayerMuteButtonClicked( ', '%PauseMenu_onPlayerMuteClicked?playerNum=', 'max', 'onMousewheelScroll', 'igm_pauseMenu::onMousewheelScroll( ', 'contentMC', 'createEmptyMovieClip', 'objectiveLayer', '../../common/ig_missionMapObjectiveLayer.swf', 'ASSetPropFlags'  
    &pushglobalgv
    &pushsdbgm 1							//'igm_pauseMenu'
    &not
    &not
    &jnz label61    
    &pushglobalgv
    &pushsdb 1							//'igm_pauseMenu'
    &function2  (r:3='screen') (r:1='this', r:2='super')
      &pushzero
      &pushregister 2
      &pushundef
      &callmp
      &pushsdb 2							//'igm_pauseMenu::igm_pauseMenu() screen = '
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
      &pushregister 1
      &pushsdb 6							//'m_objectiveListVisibleHeight'
      &pushsdbgv 3							//'Cafe2_BaseUIScreen'
      &pushsdbgm 4							//'m_screen'
      &pushsdbgm 7							//'campaignMC'
      &pushsdbgm 8							//'objectiveListMC'
      &pushsdbgm 9							//'maskMC'
      &pushsdbgm 10							//'_height'
      &setMember
      &pushzero
      &pushregister 1
      &dcallmp 11							// initGUI()
    &end // of function 

    &setRegister r:0
    &setMember
    &pushglobalgv
    &pushsdbgm 1							//'igm_pauseMenu'
    &pushsdbgv 3							//'Cafe2_BaseUIScreen'
    &extends
    &pushregister 0
    &pushsdbgm 12							//'prototype'
    &setRegister r:1
    &pop
    &pushglobalgv
    &pushsdbgm 13							//'IScrollbarEventListener'
    &pushone
    &pushglobalgv
    &pushsdbgm 1							//'igm_pauseMenu'
    &implements
    &pushregister 1
    &pushsdb 14							//'OnScrollbarDelta'
    &function2  (r:2='dir') (r:1='this')
      &pushsdb 15							//'igm_pauseMenu::OnScrollbarDelta( '
      &pushregister 2
      &toString
      &add
      &pushsdb 16							//' )'
      &add
      &trace
      &pushregister 2
      &pushzero
      &equals
      &not
      &jnz label1      
      &pushzero
      &pushregister 1
      &dcallmp 17							// scrollUp()
      &jmp label2      
     label1:
      &pushzero
      &pushregister 1
      &dcallmp 18							// scrollDown()
     label2:
    &end // of function 

    &setMember
    &pushregister 1
    &pushsdb 19							//'OnScrollbarThumbUpdate'
    &function2  (r:3='percentage') (r:1='this')
      &pushsdb 20							//'igm_pauseMenu::OnScrollbarThumbUpdate( '
      &pushregister 3
      &toString
      &add
      &pushsdb 16							//' )'
      &add
      &trace
      &pushregister 1
      &pushsdbgm 21							//'m_campaignMC'
      &pushsdbgm 8							//'objectiveListMC'
      &pushsdbgm 22							//'linesMC'
      &pushsdbgm 10							//'_height'
      &setRegister r:2
      &pop
      &pushregister 2
      &pushregister 1
      &pushsdbgm 6							//'m_objectiveListVisibleHeight'
      &greaterThan
      &not
      &jnz label3      
      &pushregister 3
      &pushfloat 0.009999999776483
      &multiply
      &pushregister 2
      &pushregister 1
      &pushsdbgm 6							//'m_objectiveListVisibleHeight'
      &subtract
      &multiply
      &pushone
      &pushregister 1
      &dcallmp 23							// scrollObjectiveListBox()
     label3:
    &end // of function 

    &setMember
    &pushregister 1
    &pushsdb 24							//'OnScrollbarMouseEnter'
    &function  (    )    
      &pushsdb 25							//'igm_pauseMenu::OnScrollbarMouseEnter()'
      &trace
    &end // of function 

    &setMember
    &pushregister 1
    &pushsdb 26							//'OnScrollbarMouseExit'
    &function  (    )    
      &pushsdb 27							//'igm_pauseMenu::OnScrollbarMouseExit()'
      &trace
    &end // of function 

    &setMember
    &pushregister 1
    &pushsdb 28							//'initMode'
    &function2  () (r:1='this')
      &pushzero
      &pushsdb 29							//'Object'
      &new
      &setRegister r:2
      &pop
      &pushsdb 30							//'QueryGameEngine?PauseMenu_mode'
      &pushregister 2
      &loadVariables
      &pushsdb 31							//'igm_pauseMenu: data.PauseMenu_mode = "'
      &pushregister 2
      &pushsdbgm 32							//'PauseMenu_mode'
      &add
      &pushsdb 33							//'"'
      &add
      &trace
      &pushregister 1
      &pushsdb 34							//'m_mode'
      &pushregister 2
      &pushsdbgm 32							//'PauseMenu_mode'
      &setMember
    &end // of function 

    &setMember
    &pushregister 1
    &pushsdb 35							//'initButton'
    &function2  (r:1='bttn', r:3='enable', r:2='onClickFn') ()
      &pushregister 1
      &pushsdb 36							//'_visible'
      &pushtrue
      &setMember
      &pushregister 3
      &not
      &jnz label4      
      &pushzero
      &pushregister 1
      &dcallmp 37							// enable()
      &jmp label5      
     label4:
      &pushzero
      &pushregister 1
      &dcallmp 38							// disable()
     label5:
      &pushregister 2
      &pushone
      &pushregister 1
      &dcallmp 39							// setOnMouseUpFunction()
    &end // of function 

    &setMember
    &pushregister 1
    &pushsdb 40							//'initButtons'
    &function2  () (r:1='this', r:2='_global')
      &pushzero
      &pushsdb 29							//'Object'
      &new
      &setRegister r:3
      &pop
      &pushsdb 41							//'QueryGameEngine?igm_pauseMenu_enableSave'
      &pushregister 3
      &loadVariables
      &pushregister 3
      &pushsdbgm 42							//'igm_pauseMenu_enableSave'
      &pushsdb 43							//'0'
      &equals
      &not
      &setRegister r:5
      &pop
      &pushsdb 44							//'QueryGameEngine?igm_pauseMenu_enableLoad'
      &pushregister 3
      &loadVariables
      &pushregister 3
      &pushsdbgm 45							//'igm_pauseMenu_enableLoad'
      &pushsdb 43							//'0'
      &equals
      &not
      &setRegister r:4
      &pop
      &pushregister 1
      &pushsdbgm 46							//'onExitButtonClicked'
      &pushregister 1
      &pushbyte 2
      &pushregister 2
      &pushsdb 47							//'bind0'
      &callMethod
      &pushtrue
      &pushsdbgv 3							//'Cafe2_BaseUIScreen'
      &pushsdbgm 4							//'m_screen'
      &pushsdbgm 48							//'exitButton'
      &pushbyte 3
      &pushregister 1
      &dcallmp 35							// initButton()
      &pushregister 1
      &pushsdbgm 49							//'onSettingsButtonClicked'
      &pushregister 1
      &pushbyte 2
      &pushregister 2
      &pushsdb 47							//'bind0'
      &callMethod
      &pushtrue
      &pushsdbgv 3							//'Cafe2_BaseUIScreen'
      &pushsdbgm 4							//'m_screen'
      &pushsdbgm 50							//'settingsButton'
      &pushbyte 3
      &pushregister 1
      &dcallmp 35							// initButton()
      &pushregister 1
      &pushsdbgm 51							//'onLoadButtonClicked'
      &pushregister 1
      &pushbyte 2
      &pushregister 2
      &pushsdb 47							//'bind0'
      &callMethod
      &pushregister 4
      &pushsdbgv 3							//'Cafe2_BaseUIScreen'
      &pushsdbgm 4							//'m_screen'
      &pushsdbgm 52							//'loadButton'
      &pushbyte 3
      &pushregister 1
      &dcallmp 35							// initButton()
      &pushregister 1
      &pushsdbgm 53							//'onSaveButtonClicked'
      &pushregister 1
      &pushbyte 2
      &pushregister 2
      &pushsdb 47							//'bind0'
      &callMethod
      &pushregister 5
      &pushsdbgv 3							//'Cafe2_BaseUIScreen'
      &pushsdbgm 4							//'m_screen'
      &pushsdbgm 54							//'saveButton'
      &pushbyte 3
      &pushregister 1
      &dcallmp 35							// initButton()
      &pushregister 1
      &pushsdbgm 55							//'backButtonClicked'
      &pushregister 1
      &pushbyte 2
      &pushregister 2
      &pushsdb 47							//'bind0'
      &callMethod
      &pushtrue
      &pushsdbgv 3							//'Cafe2_BaseUIScreen'
      &pushsdbgm 4							//'m_screen'
      &pushsdbgm 56							//'resumeButton'
      &pushbyte 3
      &pushregister 1
      &dcallmp 35							// initButton()
    &end // of function 

    &setMember
    &pushregister 1
    &pushsdb 57							//'getObjectiveLineColor'
    &function2  (r:1='isBonus') ()
      &pushzero
      &pushsdbgv 3							//'Cafe2_BaseUIScreen'
      &pushsdbgm 4							//'m_screen'
      &pushsdbgm 58							//'_root'
      &pushsdb 59							//'GetCurrentFaction'
      &callMethod
      &pushsdb 60							//'Yuriko'
      &equals
      &not
      &jnz label6      
      &pushlong 16754419
      &return
     label6:
      &pushregister 1
      &jnz label7      
      &pushlong 65280
      &jmp label8      
     label7:
      &pushlong 16763904
     label8:
      &return
    &end // of function 

    &setMember
    &pushregister 1
    &pushsdb 61							//'clearObjectiveListBox'
    &function2  () (r:1='this')
      &pushsdb 62							//'igm_pauseMenu::clearObjectiveListBox()'
      &trace
     label9:
      &pushregister 1
      &pushsdbgm 63							//'m_objectiveLines'
      &pushsdbgm 64							//'length'
      &pushzero
      &greaterThan
      &not
      &jnz label10      
      &pushzero
      &pushregister 1
      &pushsdbgm 63							//'m_objectiveLines'
      &pushsdb 65							//'pop'
      &callMethod
      &setRegister r:2
      &pop
      &pushregister 2
      &pushsdbgm 66							//'hitRegionMC'
      &pushsdbgm 67							//'bttn'
      &pushsdb 68							//'onRelease'
      &delete
      &pop
      &pushzero
      &pushregister 2
      &dcallmp 69							// removeMovieClip()
      &jmp label9      
     label10:
      &pushregister 1
      &pushsdb 70							//'m_selectedObjectiveLineIndex'
      &pushbyte -1
      &setMember
      &pushregister 1
      &pushsdbgm 21							//'m_campaignMC'
      &pushsdbgm 71							//'objectiveDescTF'
      &pushsdb 72							//'text'
      &pushsdb 73							//''
      &setMember
    &end // of function 

    &setMember
    &pushregister 0
    &pushsdb 74							//'colorizeMovieClip'
    &function2  (r:2='clip', r:3='clr') ()
      &pushregister 2
      &pushone
      &pushsdb 75							//'Color'
      &new
      &setRegister r:1
      &pop
      &pushregister 3
      &pushone
      &pushregister 1
      &dcallmp 76							// setRGB()
    &end // of function 

    &setMember
    &pushregister 1
    &pushsdb 77							//'populateObjectiveListBox'
    &function2  () (r:1='this', r:2='_global')
      &pushsdb 78							//'igm_pauseMenu::populateObjectiveListBox()'
      &trace
      &pushzero
      &pushregister 1
      &dcallmp 61							// clearObjectiveListBox()
      &pushzero
      &pushsdb 29							//'Object'
      &new
      &setRegister r:13
      &pop
      &pushsdb 79							//'QueryGameEngine?PauseMenu_missionObjectiveId&PauseMenu_missionObjectiveStatus&PauseMenu_missionObjectiveBonusness'
      &pushregister 13
      &loadVariables
      &pushsdb 80							//'igm_pauseMenu: PauseMenu_missionObjectiveId = "'
      &pushregister 13
      &pushsdbgm 81							//'PauseMenu_missionObjectiveId'
      &add
      &pushsdb 33							//'"'
      &add
      &trace
      &pushsdb 82							//'igm_pauseMenu: PauseMenu_missionObjectiveStatus = "'
      &pushregister 13
      &pushsdbgm 83							//'PauseMenu_missionObjectiveStatus'
      &add
      &pushsdb 33							//'"'
      &add
      &trace
      &pushsdb 84							//'igm_pauseMenu: PauseMenu_missionObjectiveBonusness = "'
      &pushregister 13
      &pushsdbgm 85							//'PauseMenu_missionObjectiveBonusness'
      &add
      &pushsdb 33							//'"'
      &add
      &trace
      &pushsdb 86							//','
      &pushone
      &pushregister 13
      &pushsdbgm 81							//'PauseMenu_missionObjectiveId'
      &pushsdb 87							//'split'
      &callMethod
      &setRegister r:14
      &pop
      &pushsdb 86							//','
      &pushone
      &pushregister 13
      &pushsdbgm 83							//'PauseMenu_missionObjectiveStatus'
      &pushsdb 87							//'split'
      &callMethod
      &setRegister r:16
      &pop
      &pushsdb 86							//','
      &pushone
      &pushregister 13
      &pushsdbgm 85							//'PauseMenu_missionObjectiveBonusness'
      &pushsdb 87							//'split'
      &callMethod
      &setRegister r:15
      &pop
      &pushzero
      &setRegister r:12
      &pop
      &pushregister 14
      &pushsdbgm 64							//'length'
      &setRegister r:17
      &pop
      &pushzero
      &setRegister r:4
      &pop
     label11:
      &pushregister 4
      &pushregister 17
      &lessThan
      &not
      &jnz label14      
      &pushregister 14
      &pushregister 4
      &getMember
      &toNumber
      &setRegister r:8
      &pop
      &pushregister 16
      &pushregister 4
      &getMember
      &setRegister r:10
      &pop
      &pushregister 15
      &pushregister 4
      &getMember
      &toNumber
      &pushzero
      &equals
      &not
      &setRegister r:11
      &pop
      &pushregister 4
      &pushregister 4
      &toString
      &pushsdb 88							//'ObjectiveLine'
      &pushbyte 3
      &pushregister 1
      &pushsdbgm 21							//'m_campaignMC'
      &pushsdbgm 8							//'objectiveListMC'
      &pushsdbgm 22							//'linesMC'
      &pushsdb 89							//'attachMovie'
      &callMethod
      &setRegister r:3
      &pop
      &pushregister 3
      &pushsdb 90							//'objectiveId'
      &pushregister 8
      &setMember
      &pushregister 3
      &pushsdbgm 91							//'textTF'
      &pushsdbgm 10							//'_height'
      &setRegister r:5
      &pop
      &pushregister 3
      &pushsdbgm 91							//'textTF'
      &pushsdb 72							//'text'
      &pushsdb 92							//'$PauseMenu_objectiveTitle'
      &pushregister 8
      &add
      &pushsdb 93							//'&outline'
      &add
      &setMember
      &pushregister 3
      &pushsdbgm 91							//'textTF'
      &pushsdb 94							//'autoSize'
      &pushsdb 95							//'left'
      &setMember
      &pushregister 3
      &pushsdbgm 91							//'textTF'
      &pushsdbgm 10							//'_height'
      &setRegister r:7
      &pop
      &pushregister 7
      &pushregister 5
      &lessThan
      &not
      &not
      &jnz label12      
      &pushregister 7
      &pushregister 5
      &subtract
      &setRegister r:9
      &pop
      &pushregister 3
      &pushsdbgm 96							//'spacerMC'
      &pushsdb 10							//'_height'
      &pushregister 3
      &pushsdbgm 96							//'spacerMC'
      &pushsdbgm 10							//'_height'
      &pushregister 9
      &add
      &setMember
      &pushregister 3
      &pushsdbgm 66							//'hitRegionMC'
      &pushsdb 10							//'_height'
      &pushregister 3
      &pushsdbgm 66							//'hitRegionMC'
      &pushsdbgm 10							//'_height'
      &pushregister 9
      &add
      &setMember
      &jmp label13      
     label12:
      &pushregister 3
      &pushsdbgm 91							//'textTF'
      &pushsdb 97							//'_y'
      &pushregister 3
      &pushsdbgm 91							//'textTF'
      &pushsdbgm 97							//'_y'
      &pushregister 5
      &pushregister 7
      &subtract
      &pushfloat 0.500000000000000
      &multiply
      &add
      &setMember
     label13:
      &pushregister 11
      &pushone
      &pushregister 1
      &pushsdb 57							//'getObjectiveLineColor'
      &callMethod
      &setRegister r:6
      &pop
      &pushregister 3
      &pushsdbgm 91							//'textTF'
      &pushsdb 98							//'textColor'
      &pushregister 6
      &setMember
      &pushregister 10
      &pushone
      &pushregister 3
      &pushsdbgm 99							//'checkboxMC'
      &dcallmp 100							// gotoAndPlay()
      &pushregister 6
      &pushregister 3
      &pushsdbgm 99							//'checkboxMC'
      &pushsdbgm 101							//'frameMC'
      &pushbyte 2
      &pushsdbgv 1							//'igm_pauseMenu'
      &dcallmp 74							// colorizeMovieClip()
      &pushregister 6
      &pushregister 3
      &pushsdbgm 99							//'checkboxMC'
      &pushsdbgm 102							//'checkMC'
      &pushbyte 2
      &pushsdbgv 1							//'igm_pauseMenu'
      &dcallmp 74							// colorizeMovieClip()
      &pushregister 3
      &pushsdb 103							//'_alpha'
      &pushbyte 75
      &setMember
      &pushregister 3
      &pushsdbgm 66							//'hitRegionMC'
      &pushsdbgm 67							//'bttn'
      &pushsdb 68							//'onRelease'
      &pushregister 8
      &pushregister 1
      &pushsdbgm 104							//'onObjectiveLineClicked'
      &pushregister 1
      &pushbyte 3
      &pushregister 2
      &pushsdb 105							//'bind1'
      &callMethod
      &setMember
      &pushregister 3
      &pushsdb 97							//'_y'
      &pushregister 12
      &setMember
      &pushregister 12
      &pushregister 3
      &pushsdbgm 10							//'_height'
      &add
      &setRegister r:12
      &pop
      &pushregister 3
      &pushone
      &pushregister 1
      &pushsdbgm 63							//'m_objectiveLines'
      &dcallmp 106							// push()
      &pushregister 4
      &increment
      &setRegister r:4
      &pop
      &jmp label11      
     label14:
      &pushregister 1
      &pushsdbgm 21							//'m_campaignMC'
      &pushsdbgm 8							//'objectiveListMC'
      &pushsdbgm 22							//'linesMC'
      &pushsdbgm 10							//'_height'
      &setRegister r:18
      &pop
      &pushregister 18
      &pushregister 1
      &pushsdbgm 6							//'m_objectiveListVisibleHeight'
      &greaterThan
      &not
      &jnz label15      
      &pushzero
      &pushregister 1
      &pushsdbgm 21							//'m_campaignMC'
      &pushsdbgm 107							//'objectiveListScrollBarMC'
      &dcallmp 37							// enable()
      &jmp label16      
     label15:
      &pushzero
      &pushregister 1
      &pushsdbgm 21							//'m_campaignMC'
      &pushsdbgm 107							//'objectiveListScrollBarMC'
      &dcallmp 38							// disable()
      &pushregister 1
      &pushsdbgm 21							//'m_campaignMC'
      &pushsdbgm 107							//'objectiveListScrollBarMC'
      &pushsdb 36							//'_visible'
      &pushfalse
      &setMember
      &pushzero
      &pushzero
      &pushzero
      &pushbyte 3
      &pushregister 1
      &pushsdbgm 21							//'m_campaignMC'
      &pushsdbgm 107							//'objectiveListScrollBarMC'
      &dcallmp 108							// setInfo()
     label16:
      &pushzero
      &pushregister 1
      &dcallmp 109							// selectObjectiveListBoxEntry()
    &end // of function 

    &setMember
    &pushregister 1
    &pushsdb 23							//'scrollObjectiveListBox'
    &function2  (r:4='minVisibleY') (r:1='this')
      &pushsdb 110							//'igm_pauseMenu::scrollObjectiveListBox( '
      &pushregister 4
      &toString
      &add
      &pushsdb 16							//' )'
      &add
      &trace
      &pushregister 1
      &pushsdbgm 21							//'m_campaignMC'
      &pushsdbgm 8							//'objectiveListMC'
      &pushsdbgm 22							//'linesMC'
      &pushsdbgm 10							//'_height'
      &setRegister r:7
      &pop
      &pushregister 1
      &pushsdbgm 21							//'m_campaignMC'
      &pushsdbgm 8							//'objectiveListMC'
      &pushsdbgm 22							//'linesMC'
      &pushsdb 97							//'_y'
      &pushzero
      &pushregister 4
      &subtract
      &setMember
      &pushregister 4
      &pushregister 1
      &pushsdbgm 6							//'m_objectiveListVisibleHeight'
      &add
      &setRegister r:5
      &pop
      &pushregister 1
      &pushsdbgm 63							//'m_objectiveLines'
      &pushsdbgm 64							//'length'
      &setRegister r:6
      &pop
      &pushzero
      &setRegister r:3
      &pop
     label17:
      &pushregister 3
      &pushregister 6
      &lessThan
      &not
      &jnz label19      
      &pushregister 1
      &pushsdbgm 63							//'m_objectiveLines'
      &pushregister 3
      &getMember
      &setRegister r:2
      &pop
      &pushregister 2
      &pushsdb 36							//'_visible'
      &pushregister 2
      &pushsdbgm 97							//'_y'
      &pushregister 5
      &lessThan
      &dup
      &not
      &jnz label18      
      &pop
      &pushregister 2
      &pushsdbgm 97							//'_y'
      &pushregister 2
      &pushsdbgm 10							//'_height'
      &add
      &pushregister 4
      &greaterThan
     label18:
      &setMember
      &pushregister 3
      &increment
      &setRegister r:3
      &pop
      &jmp label17      
     label19:
      &pushregister 7
      &pushregister 1
      &pushsdbgm 6							//'m_objectiveListVisibleHeight'
      &greaterThan
      &not
      &jnz label20      
      &pushregister 1
      &pushsdbgm 21							//'m_campaignMC'
      &pushsdbgm 107							//'objectiveListScrollBarMC'
      &pushsdb 36							//'_visible'
      &pushtrue
      &setMember
      &pushregister 4
      &pushregister 7
      &pushregister 1
      &pushsdbgm 6							//'m_objectiveListVisibleHeight'
      &subtract
      &pushzero
      &pushbyte 3
      &pushregister 1
      &pushsdbgm 21							//'m_campaignMC'
      &pushsdbgm 107							//'objectiveListScrollBarMC'
      &dcallmp 108							// setInfo()
     label20:
    &end // of function 

    &setMember
    &pushregister 1
    &pushsdb 111							//'ensureObjectiveLineVisibility'
    &function2  (r:6='lineIndex') (r:1='this')
      &pushsdb 112							//'igm_pauseMenu::ensureObjectiveLineVisibility( '
      &pushregister 6
      &toString
      &add
      &pushsdb 16							//' )'
      &add
      &trace
      &pushzero
      &pushregister 1
      &pushsdbgm 21							//'m_campaignMC'
      &pushsdbgm 8							//'objectiveListMC'
      &pushsdbgm 22							//'linesMC'
      &pushsdbgm 97							//'_y'
      &subtract
      &setRegister r:3
      &pop
      &pushregister 3
      &pushregister 1
      &pushsdbgm 6							//'m_objectiveListVisibleHeight'
      &add
      &setRegister r:4
      &pop
      &pushregister 1
      &pushsdbgm 63							//'m_objectiveLines'
      &pushregister 6
      &getMember
      &setRegister r:2
      &pop
      &pushregister 2
      &pushsdbgm 97							//'_y'
      &pushregister 3
      &lessThan
      &not
      &jnz label21      
      &pushregister 2
      &pushsdbgm 97							//'_y'
      &pushone
      &pushregister 1
      &dcallmp 23							// scrollObjectiveListBox()
      &jmp label22      
     label21:
      &pushregister 2
      &pushsdbgm 97							//'_y'
      &pushregister 2
      &pushsdbgm 10							//'_height'
      &add
      &setRegister r:5
      &pop
      &pushregister 5
      &pushregister 4
      &greaterThan
      &not
      &jnz label22      
      &pushregister 3
      &pushregister 5
      &pushregister 4
      &subtract
      &add
      &pushone
      &pushregister 1
      &dcallmp 23							// scrollObjectiveListBox()
     label22:
    &end // of function 

    &setMember
    &pushregister 1
    &pushsdb 113							//'refreshPlayerStatus'
    &function2  () (r:1='this')
      &pushzero
      &setRegister r:2
      &pop
     label23:
      &pushregister 2
      &pushsdbgv 1							//'igm_pauseMenu'
      &pushsdbgm 114							//'MAX_PLAYERS'
      &lessThan
      &not
      &jnz label24      
      &pushsdbgv 115							//'MovieClip'
      &pushregister 1
      &pushsdbgm 116							//'m_playerLineMCs'
      &pushregister 2
      &getMember
      &cast
      &setRegister r:3
      &pop
      &pushregister 3
      &pushsdbgm 117							//'statusMC'
      &pushsdbgm 118							//'statusTF'
      &pushsdb 72							//'text'
      &pushsdb 119							//'$PauseMenu_playerStatus'
      &pushregister 2
      &toString
      &add
      &pushsdb 93							//'&outline'
      &add
      &setMember
      &pushregister 2
      &increment
      &setRegister r:2
      &pop
      &jmp label23      
     label24:
    &end // of function 

    &setMember
    &pushregister 1
    &pushsdb 109							//'selectObjectiveListBoxEntry'
    &function2  () (r:1='this')
      &pushsdb 120							//'igm_pauseMenu::selectObjectiveListBoxEntry()'
      &trace
      &pushregister 1
      &pushsdbgm 70							//'m_selectedObjectiveLineIndex'
      &pushzero
      &lessThan
      &not
      &not
      &jnz label25      
      &pushregister 1
      &pushsdbgm 63							//'m_objectiveLines'
      &pushregister 1
      &pushsdbgm 70							//'m_selectedObjectiveLineIndex'
      &getMember
      &setRegister r:6
      &pop
      &pushregister 6
      &pushsdb 103							//'_alpha'
      &pushbyte 75
      &setMember
      &pushregister 1
      &pushsdb 70							//'m_selectedObjectiveLineIndex'
      &pushbyte -1
      &setMember
     label25:
      &pushzero
      &pushsdb 29							//'Object'
      &new
      &setRegister r:5
      &pop
      &pushsdb 121							//'QueryGameEngine?PauseMenu_selectedMissionObjectiveId'
      &pushregister 5
      &loadVariables
      &pushsdb 122							//'igm_pauseMenu: PauseMenu_selectedMissionObjectiveId = "'
      &pushregister 5
      &pushsdbgm 123							//'PauseMenu_selectedMissionObjectiveId'
      &add
      &pushsdb 33							//'"'
      &add
      &trace
      &pushregister 5
      &pushsdbgm 123							//'PauseMenu_selectedMissionObjectiveId'
      &toNumber
      &setRegister r:4
      &pop
      &pushregister 1
      &pushsdbgm 63							//'m_objectiveLines'
      &pushsdbgm 64							//'length'
      &setRegister r:3
      &pop
      &pushzero
      &setRegister r:2
      &pop
     label26:
      &pushregister 2
      &pushregister 3
      &lessThan
      &not
      &jnz label28      
      &pushregister 1
      &pushsdbgm 63							//'m_objectiveLines'
      &pushregister 2
      &getMember
      &setRegister r:6
      &pop
      &pushregister 6
      &pushsdbgm 90							//'objectiveId'
      &pushregister 4
      &equals
      &not
      &jnz label27      
      &jmp label28      
     label27:
      &pushregister 2
      &increment
      &setRegister r:2
      &pop
      &jmp label26      
     label28:
      &pushregister 2
      &pushregister 3
      &lessThan
      &not
      &jnz label29      
      &pushregister 2
      &pushone
      &pushregister 1
      &dcallmp 111							// ensureObjectiveLineVisibility()
      &pushregister 1
      &pushsdbgm 63							//'m_objectiveLines'
      &pushregister 2
      &getMember
      &setRegister r:6
      &pop
      &pushregister 6
      &pushsdb 103							//'_alpha'
      &pushbyte 100
      &setMember
      &pushregister 1
      &pushsdbgm 21							//'m_campaignMC'
      &pushsdbgm 71							//'objectiveDescTF'
      &pushsdb 72							//'text'
      &pushsdb 124							//'$PauseMenu_objectiveDesc'
      &pushregister 4
      &add
      &pushsdb 93							//'&outline'
      &add
      &setMember
      &pushregister 1
      &pushsdb 70							//'m_selectedObjectiveLineIndex'
      &pushregister 2
      &setMember
      &jmp label30      
     label29:
      &pushzero
      &pushone
      &pushregister 1
      &dcallmp 23							// scrollObjectiveListBox()
     label30:
    &end // of function 

    &setMember
    &pushregister 1
    &pushsdb 125							//'submitMissionMap'
    &function  (    )    
      &pushsdb 126							//'igm_pauseMenu::submitMissionMap()'
      &trace
      &pushsdb 127							//'FSCommand:CallGameFunction'
      &pushsdb 128							//'%PauseMenu_receiveMissionMap?missionMap='
      &pushsdbgv 3							//'Cafe2_BaseUIScreen'
      &pushsdbgm 4							//'m_screen'
      &pushsdbgm 129							//'missionMap'
      &pushone
      &pushsdbgv 130							//'Pointer'
      &pushsdb 131							//'encode'
      &callMethod
      &add
      &getURL2
      &pushzero
      &pushsdbgv 3							//'Cafe2_BaseUIScreen'
      &pushsdbgm 4							//'m_screen'
      &pushsdbgm 129							//'missionMap'
      &dcallmp 132							// OnBound()
    &end // of function 

    &setMember
    &pushregister 1
    &pushsdb 133							//'refreshPlayerMuted'
    &function2  () (r:1='this')
      &pushsdb 134							//'igm_pauseMenu::refreshPlayerMuted()'
      &trace
      &pushzero
      &pushsdb 29							//'Object'
      &new
      &setRegister r:5
      &pop
      &pushsdb 135							//'QueryGameEngine?PauseMenu_playerMuted'
      &pushregister 5
      &loadVariables
      &pushsdb 136							//'igm_pauseMenu: PauseMenu_playerMuted = "'
      &pushregister 5
      &pushsdbgm 137							//'PauseMenu_playerMuted'
      &add
      &pushsdb 33							//'"'
      &add
      &trace
      &pushsdb 86							//','
      &pushone
      &pushregister 5
      &pushsdbgm 137							//'PauseMenu_playerMuted'
      &pushsdb 87							//'split'
      &callMethod
      &setRegister r:4
      &pop
      &pushzero
      &setRegister r:2
      &pop
     label31:
      &pushregister 2
      &pushregister 1
      &pushsdbgm 138							//'m_playerCount'
      &lessThan
      &not
      &jnz label32      
      &pushsdbgv 115							//'MovieClip'
      &pushregister 1
      &pushsdbgm 116							//'m_playerLineMCs'
      &pushregister 2
      &getMember
      &cast
      &setRegister r:3
      &pop
      &pushregister 3
      &pushsdbgm 139							//'muteButtonMC'
      &pushsdbgm 140							//'muteMC'
      &pushsdb 36							//'_visible'
      &pushregister 4
      &pushregister 2
      &getMember
      &toNumber
      &pushzero
      &equals
      &not
      &setMember
      &pushregister 2
      &increment
      &setRegister r:2
      &pop
      &jmp label31      
     label32:
    &end // of function 

    &setMember
    &pushregister 1
    &pushsdb 141							//'initPlayerList'
    &function2  () (r:1='this', r:2='_global')
      &pushsdb 142							//'igm_pauseMenu::initPlayerList()'
      &trace
      &pushregister 1
      &pushsdb 116							//'m_playerLineMCs'
      &pushsdbgv 1							//'igm_pauseMenu'
      &pushsdbgm 114							//'MAX_PLAYERS'
      &pushone
      &pushsdb 143							//'Array'
      &new
      &setMember
      &pushzero
      &setRegister r:3
      &pop
     label33:
      &pushregister 3
      &pushsdbgv 1							//'igm_pauseMenu'
      &pushsdbgm 114							//'MAX_PLAYERS'
      &lessThan
      &not
      &jnz label34      
      &pushsdbgv 115							//'MovieClip'
      &pushregister 1
      &pushsdbgm 144							//'m_skirmishMC'
      &pushsdbgm 145							//'playerListMC'
      &pushregister 3
      &toString
      &getMember
      &cast
      &setRegister r:4
      &pop
      &pushregister 1
      &pushsdbgm 116							//'m_playerLineMCs'
      &pushregister 3
      &pushregister 4
      &setMember
      &pushregister 4
      &pushsdbgm 146							//'nameMC'
      &pushsdbgm 147							//'nameTF'
      &pushsdb 72							//'text'
      &pushsdb 148							//'$PauseMenu_playerName'
      &pushregister 3
      &toString
      &add
      &pushsdb 93							//'&outline'
      &add
      &setMember
      &pushregister 4
      &pushsdbgm 117							//'statusMC'
      &pushsdbgm 118							//'statusTF'
      &pushsdb 72							//'text'
      &pushsdb 119							//'$PauseMenu_playerStatus'
      &pushregister 3
      &toString
      &add
      &pushsdb 93							//'&outline'
      &add
      &setMember
      &pushregister 4
      &pushsdbgm 139							//'muteButtonMC'
      &pushsdb 149							//'onClicked'
      &pushregister 3
      &pushregister 1
      &pushsdbgm 150							//'onPlayerMuteButtonClicked'
      &pushregister 1
      &pushbyte 3
      &pushregister 2
      &pushsdb 105							//'bind1'
      &callMethod
      &setMember
      &pushregister 4
      &pushsdb 36							//'_visible'
      &pushfalse
      &setMember
      &pushregister 3
      &increment
      &setRegister r:3
      &pop
      &jmp label33      
     label34:
      &pushzero
      &pushsdb 29							//'Object'
      &new
      &setRegister r:7
      &pop
      &pushsdb 151							//'QueryGameEngine?PauseMenu_playerFaction&PauseMenu_playerColor&PauseMenu_playerIsNonLocalHuman&PauseMenu_playerMuted'
      &pushregister 7
      &loadVariables
      &pushsdb 152							//'igm_pauseMenu: PauseMenu_playerFaction = "'
      &pushregister 7
      &pushsdbgm 153							//'PauseMenu_playerFaction'
      &add
      &pushsdb 33							//'"'
      &add
      &trace
      &pushsdb 154							//'igm_pauseMenu: PauseMenu_playerColor = "'
      &pushregister 7
      &pushsdbgm 155							//'PauseMenu_playerColor'
      &add
      &pushsdb 33							//'"'
      &add
      &trace
      &pushsdb 156							//'igm_pauseMenu: PauseMenu_playerIsNonLocalHuman = "'
      &pushregister 7
      &pushsdbgm 157							//'PauseMenu_playerIsNonLocalHuman'
      &add
      &pushsdb 33							//'"'
      &add
      &trace
      &pushsdb 136							//'igm_pauseMenu: PauseMenu_playerMuted = "'
      &pushregister 7
      &pushsdbgm 137							//'PauseMenu_playerMuted'
      &add
      &pushsdb 33							//'"'
      &add
      &trace
      &pushsdb 86							//','
      &pushone
      &pushregister 7
      &pushsdbgm 153							//'PauseMenu_playerFaction'
      &pushsdb 87							//'split'
      &callMethod
      &setRegister r:8
      &pop
      &pushsdb 86							//','
      &pushone
      &pushregister 7
      &pushsdbgm 155							//'PauseMenu_playerColor'
      &pushsdb 87							//'split'
      &callMethod
      &setRegister r:9
      &pop
      &pushsdb 86							//','
      &pushone
      &pushregister 7
      &pushsdbgm 157							//'PauseMenu_playerIsNonLocalHuman'
      &pushsdb 87							//'split'
      &callMethod
      &setRegister r:11
      &pop
      &pushsdb 86							//','
      &pushone
      &pushregister 7
      &pushsdbgm 137							//'PauseMenu_playerMuted'
      &pushsdb 87							//'split'
      &callMethod
      &setRegister r:10
      &pop
      &pushregister 1
      &pushsdb 138							//'m_playerCount'
      &pushsdbgv 1							//'igm_pauseMenu'
      &pushsdbgm 114							//'MAX_PLAYERS'
      &pushregister 8
      &pushsdbgm 64							//'length'
      &pushbyte 2
      &pushsdbgv 158							//'Math'
      &pushsdb 159							//'min'
      &callMethod
      &setMember
      &pushzero
      &setRegister r:3
      &pop
     label35:
      &pushregister 3
      &pushregister 1
      &pushsdbgm 138							//'m_playerCount'
      &lessThan
      &not
      &jnz label38      
      &pushsdbgv 115							//'MovieClip'
      &pushregister 1
      &pushsdbgm 116							//'m_playerLineMCs'
      &pushregister 3
      &getMember
      &cast
      &setRegister r:4
      &pop
      &pushregister 4
      &pushsdb 36							//'_visible'
      &pushtrue
      &setMember
      &pushregister 8
      &pushregister 3
      &getMember
      &pushone
      &pushregister 4
      &pushsdbgm 160							//'factionMC'
      &dcallmp 100							// gotoAndPlay()
      &pushregister 4
      &pushsdbgm 161							//'colorMC'
      &pushsdbgm 161							//'colorMC'
      &pushone
      &pushsdb 75							//'Color'
      &new
      &setRegister r:5
      &pop
      &pushregister 9
      &pushregister 3
      &getMember
      &toNumber
      &pushone
      &pushregister 5
      &dcallmp 76							// setRGB()
      &pushregister 11
      &pushregister 3
      &getMember
      &toNumber
      &pushzero
      &equals
      &not
      &setRegister r:6
      &pop
      &pushregister 6
      &jnz label36      
      &pushsdb 162							//'_disabled'
      &jmp label37      
     label36:
      &pushsdb 163							//'_up'
     label37:
      &pushone
      &pushregister 4
      &pushsdbgm 139							//'muteButtonMC'
      &dcallmp 100							// gotoAndPlay()
      &pushregister 4
      &pushsdbgm 139							//'muteButtonMC'
      &pushsdbgm 140							//'muteMC'
      &pushsdb 36							//'_visible'
      &pushregister 10
      &pushregister 3
      &getMember
      &toNumber
      &pushzero
      &equals
      &not
      &setMember
      &pushregister 3
      &increment
      &setRegister r:3
      &pop
      &jmp label35      
     label38:
    &end // of function 

    &setMember
    &pushregister 1
    &pushsdb 11							//'initGUI'
    &function2  () (r:1='this', r:2='_global')
      &pushsdb 164							//'igm_pauseMenu::initGUI()'
      &trace
      &pushzero
      &pushregister 1
      &dcallmp 28							// initMode()
      &pushzero
      &pushregister 1
      &dcallmp 40							// initButtons()
      &pushsdb 165							//'igm_pauseMenuShared.swf'
      &pushone
      &pushsdbgv 3							//'Cafe2_BaseUIScreen'
      &pushsdbgm 4							//'m_screen'
      &pushsdbgm 166							//'backgroundMC'
      &dcallmp 167							// loadMovie()
      &pushsdb 168							//'../../common/ig_missionMap.swf'
      &pushone
      &pushsdbgv 3							//'Cafe2_BaseUIScreen'
      &pushsdbgm 4							//'m_screen'
      &pushsdbgm 129							//'missionMap'
      &dcallmp 167							// loadMovie()
      &pushzero
      &pushregister 1
      &dcallmp 125							// submitMissionMap()
      &pushregister 1
      &pushsdb 21							//'m_campaignMC'
      &pushsdbgv 3							//'Cafe2_BaseUIScreen'
      &pushsdbgm 4							//'m_screen'
      &pushsdbgm 7							//'campaignMC'
      &setMember
      &pushregister 1
      &pushsdbgm 21							//'m_campaignMC'
      &pushsdb 36							//'_visible'
      &pushfalse
      &setMember
      &pushregister 1
      &pushsdb 144							//'m_skirmishMC'
      &pushsdbgv 3							//'Cafe2_BaseUIScreen'
      &pushsdbgm 4							//'m_screen'
      &pushsdbgm 169							//'skirmishMC'
      &setMember
      &pushregister 1
      &pushsdbgm 144							//'m_skirmishMC'
      &pushsdb 36							//'_visible'
      &pushfalse
      &setMember
      &pushregister 1
      &pushsdbgm 34							//'m_mode'
      &setRegister r:0
      &pushsdb 170							//'Campaign'
      &strictEquals
      &jnz label39      
      &pushregister 0
      &pushsdb 171							//'Skirmish'
      &strictEquals
      &jnz label40      
      &jmp label41      
     label39:
      &pushregister 1
      &pushsdbgm 21							//'m_campaignMC'
      &pushsdb 36							//'_visible'
      &pushtrue
      &setMember
      &pushregister 1
      &pushone
      &pushregister 1
      &pushsdbgm 21							//'m_campaignMC'
      &pushsdbgm 107							//'objectiveListScrollBarMC'
      &dcallmp 172							// addListener()
      &pushregister 1
      &pushsdbgm 6							//'m_objectiveListVisibleHeight'
      &pushone
      &pushregister 1
      &pushsdbgm 21							//'m_campaignMC'
      &pushsdbgm 107							//'objectiveListScrollBarMC'
      &dcallmp 173							// setSize()
      &pushzero
      &pushregister 1
      &dcallmp 77							// populateObjectiveListBox()
      &jmp label42      
     label40:
      &pushregister 1
      &pushsdbgm 144							//'m_skirmishMC'
      &pushsdb 36							//'_visible'
      &pushtrue
      &setMember
      &pushzero
      &pushregister 1
      &dcallmp 141							// initPlayerList()
      &jmp label42      
     label41:
      &pushsdb 174							//'igm_pauseMenu: unrecognized mode "'
      &pushregister 1
      &pushsdbgm 34							//'m_mode'
      &add
      &pushsdb 33							//'"'
      &add
      &trace
      &jmp label42      
     label42:
      &pushregister 1
      &pushsdb 175							//'m_localMessageHandlerFunc'
      &pushregister 1
      &pushsdbgm 176							//'localMessageHandler'
      &pushregister 1
      &pushbyte 2
      &pushregister 2
      &pushsdb 177							//'bind1DynamicParams'
      &callMethod
      &setMember
      &pushregister 1
      &pushsdbgm 175							//'m_localMessageHandlerFunc'
      &pushone
      &pushregister 2
      &pushsdbgm 178							//'gMH'
      &dcallmp 179							// addMessageHandler()
      &pushregister 1
      &pushsdb 180							//'m_localEventHandlerIndex'
      &pushregister 1
      &pushsdbgm 181							//'localEventHandler'
      &pushregister 1
      &pushbyte 2
      &pushregister 2
      &pushsdb 182							//'bind2DynamicParams'
      &callMethod
      &pushone
      &pushregister 2
      &pushsdbgm 183							//'gEH'
      &pushsdb 184							//'addLocalEventHandler'
      &callMethod
      &setMember
      &pushsdbgv 3							//'Cafe2_BaseUIScreen'
      &pushsdbgm 4							//'m_screen'
      &pushsdbgm 56							//'resumeButton'
      &pushsdbgv 3							//'Cafe2_BaseUIScreen'
      &pushsdbgm 4							//'m_screen'
      &pushsdbgm 54							//'saveButton'
      &pushsdbgv 3							//'Cafe2_BaseUIScreen'
      &pushsdbgm 4							//'m_screen'
      &pushsdbgm 52							//'loadButton'
      &pushsdbgv 3							//'Cafe2_BaseUIScreen'
      &pushsdbgm 4							//'m_screen'
      &pushsdbgm 50							//'settingsButton'
      &pushsdbgv 3							//'Cafe2_BaseUIScreen'
      &pushsdbgm 4							//'m_screen'
      &pushsdbgm 48							//'exitButton'
      &pushbyte 5
      &initArray
      &pushone
      &pushregister 1
      &dcallmp 185							// registerIntroOuttroComponents()
      &pushzero
      &pushregister 1
      &dcallmp 186							// startIntro()
      &pushregister 1
      &pushsdbgm 187							//'onScreenExit'
      &pushregister 1
      &pushbyte 2
      &pushregister 2
      &pushsdb 47							//'bind0'
      &callMethod
      &pushone
      &pushregister 2
      &pushsdbgm 188							//'gSM'
      &dcallmp 189							// setOnExitScreen()
      &pushsdbgv 3							//'Cafe2_BaseUIScreen'
      &pushsdbgm 4							//'m_screen'
      &pushsdb 190							//'OnContentLoaded'
      &pushregister 1
      &pushsdbgm 191							//'onContentLoaded'
      &pushregister 1
      &pushbyte 2
      &pushregister 2
      &pushsdb 177							//'bind1DynamicParams'
      &callMethod
      &setMember
      &pushregister 2
      &pushsdb 192							//'highlightedComponent'
      &pushregister 1
      &setMember
    &end // of function 

    &setMember
    &pushregister 1
    &pushsdb 187							//'onScreenExit'
    &function2  () (r:1='this', r:2='_global')
      &pushsdb 193							//'igm_pauseMenu::onScreenExit()'
      &trace
      &pushregister 2
      &pushsdbgm 192							//'highlightedComponent'
      &pushregister 1
      &equals
      &not
      &jnz label43      
      &pushregister 2
      &pushsdb 192							//'highlightedComponent'
      &pushundef
      &setMember
     label43:
      &pushsdbgv 3							//'Cafe2_BaseUIScreen'
      &pushsdbgm 4							//'m_screen'
      &pushsdbgm 194							//'missionMapObjectiveLayerMC'
      &pushsdb 190							//'OnContentLoaded'
      &delete
      &pop
      &pushsdbgv 3							//'Cafe2_BaseUIScreen'
      &pushsdbgm 4							//'m_screen'
      &pushsdb 190							//'OnContentLoaded'
      &delete
      &pop
      &pushzero
      &pushsdbgv 3							//'Cafe2_BaseUIScreen'
      &pushsdbgm 4							//'m_screen'
      &pushsdbgm 129							//'missionMap'
      &dcallmp 195							// OnDestroying()
      &pushregister 1
      &pushsdbgm 180							//'m_localEventHandlerIndex'
      &pushone
      &pushregister 2
      &pushsdbgm 183							//'gEH'
      &dcallmp 196							// removeLocalEventHandler()
      &pushregister 1
      &pushsdbgm 175							//'m_localMessageHandlerFunc'
      &pushone
      &pushregister 2
      &pushsdbgm 178							//'gMH'
      &dcallmp 197							// removeMessageHandler()
      &pushregister 1
      &pushsdb 175							//'m_localMessageHandlerFunc'
      &delete
      &pop
      &pushzero
      &pushregister 1
      &dcallmp 61							// clearObjectiveListBox()
      &pushzero
      &pushregister 1
      &dcallmp 198							// clearData()
      &pushsdbgv 3							//'Cafe2_BaseUIScreen'
      &pushsdb 4							//'m_screen'
      &pushnull
      &setMember
      &pushsdbgv 3							//'Cafe2_BaseUIScreen'
      &pushsdb 5							//'m_thisClass'
      &pushnull
      &setMember
      &pushregister 2
      &pushsdb 1							//'igm_pauseMenu'
      &delete
      &pop
      &pushregister 2
      &pushsdb 3							//'Cafe2_BaseUIScreen'
      &delete
      &pop
    &end // of function 

    &setMember
    &pushregister 1
    &pushsdb 176							//'localMessageHandler'
    &function2  (r:3='messageCode') (r:1='this', r:2='_global')
      &pushsdb 199							//'igm_pauseMenu::localMessageHandler() messageCode = \''
      &pushregister 3
      &add
      &pushsdb 200							//'\''
      &add
      &trace
      &pushregister 3
      &setRegister r:0
      &pushregister 2
      &pushsdbgm 201							//'MSGCODE'
      &pushsdbgm 202							//'IGM_CLOSE_PAUSE_MENU'
      &strictEquals
      &jnz label44      
      &pushregister 0
      &pushregister 2
      &pushsdbgm 201							//'MSGCODE'
      &pushsdbgm 203							//'IG_PAUSE_MENU_REFRESH_OBJECTIVES'
      &strictEquals
      &jnz label45      
      &pushregister 0
      &pushregister 2
      &pushsdbgm 201							//'MSGCODE'
      &pushsdbgm 204							//'IG_PAUSE_MENU_SUBMIT_MISSION_MAP'
      &strictEquals
      &jnz label46      
      &pushregister 0
      &pushregister 2
      &pushsdbgm 201							//'MSGCODE'
      &pushsdbgm 205							//'IG_PAUSE_MENU_SELECT_OBJECTIVE'
      &strictEquals
      &jnz label47      
      &pushregister 0
      &pushregister 2
      &pushsdbgm 201							//'MSGCODE'
      &pushsdbgm 206							//'IG_PAUSE_MENU_REFRESH_PLAYER_MUTED'
      &strictEquals
      &jnz label48      
      &pushregister 0
      &pushregister 2
      &pushsdbgm 201							//'MSGCODE'
      &pushsdbgm 207							//'IG_PAUSE_MENU_REFRESH_PLAYER_STATUS'
      &strictEquals
      &jnz label49      
      &jmp label50      
     label44:
      &pushzero
      &pushregister 1
      &dcallmp 55							// backButtonClicked()
      &pushtrue
      &return
     label45:
      &pushzero
      &pushregister 1
      &dcallmp 77							// populateObjectiveListBox()
      &pushtrue
      &return
     label46:
      &pushzero
      &pushregister 1
      &dcallmp 125							// submitMissionMap()
      &pushtrue
      &return
     label47:
      &pushzero
      &pushregister 1
      &dcallmp 109							// selectObjectiveListBoxEntry()
      &pushtrue
      &return
     label48:
      &pushzero
      &pushregister 1
      &dcallmp 133							// refreshPlayerMuted()
      &pushtrue
      &return
     label49:
      &pushzero
      &pushregister 1
      &dcallmp 113							// refreshPlayerStatus()
      &pushtrue
      &return
     label50:
      &pushfalse
      &return
    &end // of function 

    &setMember
    &pushregister 1
    &pushsdb 181							//'localEventHandler'
    &function2  (r:3='keyCode', 'controller') (r:1='this', r:2='_global')
      &pushsdb 208							//'igm_pauseMenu::localEventHandler() keyCode == '
      &pushregister 3
      &add
      &trace
      &pushregister 3
      &pushsdbgv 209							//'INPUTCODE'
      &pushsdbgm 210							//'ESCAPE'
      &strictEquals
      &jnz label51      
      &jmp label52      
     label51:
      &pushzero
      &pushregister 1
      &dcallmp 55							// backButtonClicked()
      &jmp label53      
     label52:
      &pushregister 3
      &pushone
      &pushregister 2
      &pushsdbgm 211							//'gFM'
      &dcallmp 212							// handleDefaultInputs()
      &jmp label53      
     label53:
    &end // of function 

    &setMember
    &pushregister 1
    &pushsdb 46							//'onExitButtonClicked'
    &function  (    )    
      &pushsdb 213							//'igm_pauseMenu::onExitButtonClicked()'
      &trace
      &getURL 'FSCommand:CallGameFunction'       '%igm_pauseMenu_onExitClicked'      
    &end // of function 

    &setMember
    &pushregister 1
    &pushsdb 49							//'onSettingsButtonClicked'
    &function2  () (r:1='this', r:2='_global')
      &pushsdb 214							//'igm_pauseMenu::onSettingsButtonClicked()'
      &trace
      &pushregister 2
      &pushsdbgm 215							//'SCREEN'
      &pushsdbgm 216							//'FEG_OPTIONS'
      &pushone
      &pushregister 1
      &dcallmp 217							// startOuttro()
    &end // of function 

    &setMember
    &pushregister 1
    &pushsdb 51							//'onLoadButtonClicked'
    &function2  () (r:1='this', r:2='_global')
      &pushsdb 218							//'igm_pauseMenu::onLoadButtonClicked()'
      &trace
      &pushregister 2
      &pushsdbgm 219							//'GAME'
      &pushsdb 220							//'SAVELOADSCREEN_TYPE'
      &pushregister 2
      &pushsdbgm 221							//'ENUM'
      &pushsdbgm 222							//'SAVELOADSCREEN_LOAD'
      &setMember
      &pushregister 2
      &pushsdbgm 215							//'SCREEN'
      &pushsdbgm 223							//'FEG_SAVELOAD'
      &pushone
      &pushregister 1
      &dcallmp 217							// startOuttro()
    &end // of function 

    &setMember
    &pushregister 1
    &pushsdb 53							//'onSaveButtonClicked'
    &function2  () (r:1='this', r:2='_global')
      &pushsdb 224							//'igm_pauseMenu::onSaveButtonClicked()'
      &trace
      &getURL 'FSCommand:CallGameFunction'       '%igm_pauseMenu.OnSaveClicked'      
      &pushregister 2
      &pushsdbgm 219							//'GAME'
      &pushsdb 220							//'SAVELOADSCREEN_TYPE'
      &pushregister 2
      &pushsdbgm 221							//'ENUM'
      &pushsdbgm 225							//'SAVELOADSCREEN_SAVE'
      &setMember
      &pushregister 2
      &pushsdbgm 215							//'SCREEN'
      &pushsdbgm 223							//'FEG_SAVELOAD'
      &pushone
      &pushregister 1
      &dcallmp 217							// startOuttro()
    &end // of function 

    &setMember
    &pushregister 1
    &pushsdb 104							//'onObjectiveLineClicked'
    &function2  (r:1='objectiveId') ()
      &pushsdb 226							//'igm_pauseMenu::onObjectiveLineClicked( '
      &pushregister 1
      &toString
      &add
      &pushsdb 16							//' )'
      &add
      &trace
      &pushsdb 127							//'FSCommand:CallGameFunction'
      &pushsdb 227							//'%PauseMenu_onMissionObjectiveClicked?objectiveId='
      &pushregister 1
      &add
      &getURL2
    &end // of function 

    &setMember
    &pushregister 1
    &pushsdb 150							//'onPlayerMuteButtonClicked'
    &function2  (r:1='playerNum') ()
      &pushsdb 228							//'igm_pauseMenu::onPlayerMuteButtonClicked( '
      &pushregister 1
      &toString
      &add
      &pushsdb 16							//' )'
      &add
      &trace
      &pushsdb 127							//'FSCommand:CallGameFunction'
      &pushsdb 229							//'%PauseMenu_onPlayerMuteClicked?playerNum='
      &pushregister 1
      &toString
      &add
      &getURL2
    &end // of function 

    &setMember
    &pushregister 1
    &pushsdb 17							//'scrollUp'
    &function2  () (r:1='this')
      &pushzero
      &pushregister 1
      &pushsdbgm 21							//'m_campaignMC'
      &pushsdbgm 8							//'objectiveListMC'
      &pushsdbgm 22							//'linesMC'
      &pushsdbgm 97							//'_y'
      &subtract
      &setRegister r:2
      &pop
      &pushregister 2
      &pushzero
      &greaterThan
      &not
      &jnz label54      
      &pushzero
      &pushregister 2
      &pushbyte 20
      &subtract
      &pushbyte 2
      &pushsdbgv 158							//'Math'
      &pushsdb 230							//'max'
      &callMethod
      &setRegister r:2
      &pop
      &pushregister 2
      &pushone
      &pushregister 1
      &dcallmp 23							// scrollObjectiveListBox()
     label54:
    &end // of function 

    &setMember
    &pushregister 1
    &pushsdb 18							//'scrollDown'
    &function2  () (r:1='this')
      &pushzero
      &pushregister 1
      &pushsdbgm 21							//'m_campaignMC'
      &pushsdbgm 8							//'objectiveListMC'
      &pushsdbgm 22							//'linesMC'
      &pushsdbgm 97							//'_y'
      &subtract
      &setRegister r:2
      &pop
      &pushregister 1
      &pushsdbgm 21							//'m_campaignMC'
      &pushsdbgm 8							//'objectiveListMC'
      &pushsdbgm 22							//'linesMC'
      &pushsdbgm 10							//'_height'
      &pushregister 1
      &pushsdbgm 6							//'m_objectiveListVisibleHeight'
      &subtract
      &setRegister r:3
      &pop
      &pushregister 2
      &pushregister 3
      &lessThan
      &not
      &jnz label55      
      &pushregister 3
      &pushregister 2
      &pushbyte 20
      &add
      &pushbyte 2
      &pushsdbgv 158							//'Math'
      &pushsdb 159							//'min'
      &callMethod
      &setRegister r:2
      &pop
      &pushregister 2
      &pushone
      &pushregister 1
      &dcallmp 23							// scrollObjectiveListBox()
     label55:
    &end // of function 

    &setMember
    &pushregister 1
    &pushsdb 231							//'onMousewheelScroll'
    &function2  (r:2='spins') (r:1='this')
      &pushsdb 232							//'igm_pauseMenu::onMousewheelScroll( '
      &pushregister 2
      &toString
      &add
      &pushsdb 16							//' )'
      &add
      &trace
      &pushregister 2
      &pushzero
      &greaterThan
      &not
      &jnz label58      
     label56:
      &pushregister 2
      &pushzero
      &greaterThan
      &not
      &jnz label57      
      &pushzero
      &pushregister 1
      &dcallmp 17							// scrollUp()
      &pushregister 2
      &decrement
      &setRegister r:2
      &pop
      &jmp label56      
     label57:
      &jmp label59      
     label58:
      &pushregister 2
      &pushzero
      &lessThan
      &not
      &jnz label59      
      &pushzero
      &pushregister 1
      &dcallmp 18							// scrollDown()
      &pushregister 2
      &increment
      &setRegister r:2
      &pop
      &jmp label58      
     label59:
    &end // of function 

    &setMember
    &pushregister 1
    &pushsdb 191							//'onContentLoaded'
    &function2  (r:2='clip') (r:1='_global')
      &pushregister 2
      &pushsdbgv 3							//'Cafe2_BaseUIScreen'
      &pushsdbgm 4							//'m_screen'
      &pushsdbgm 129							//'missionMap'
      &equals
      &not
      &jnz label60      
      &pushzero
      &pushsdb 233							//'contentMC'
      &pushbyte 2
      &pushsdbgv 3							//'Cafe2_BaseUIScreen'
      &pushsdbgm 4							//'m_screen'
      &pushsdbgm 194							//'missionMapObjectiveLayerMC'
      &dcallmp 234							// createEmptyMovieClip()
      &pushsdbgv 3							//'Cafe2_BaseUIScreen'
      &pushsdbgm 4							//'m_screen'
      &pushsdbgm 129							//'missionMap'
      &pushsdb 235							//'objectiveLayer'
      &pushsdbgv 3							//'Cafe2_BaseUIScreen'
      &pushsdbgm 4							//'m_screen'
      &pushsdbgm 194							//'missionMapObjectiveLayerMC'
      &pushsdbgm 233							//'contentMC'
      &setMember
      &pushsdb 236							//'../../common/ig_missionMapObjectiveLayer.swf'
      &pushone
      &pushsdbgv 3							//'Cafe2_BaseUIScreen'
      &pushsdbgm 4							//'m_screen'
      &pushsdbgm 194							//'missionMapObjectiveLayerMC'
      &pushsdbgm 233							//'contentMC'
      &dcallmp 167							// loadMovie()
      &pushsdbgv 3							//'Cafe2_BaseUIScreen'
      &pushsdbgm 4							//'m_screen'
      &pushsdbgm 194							//'missionMapObjectiveLayerMC'
      &pushsdb 190							//'OnContentLoaded'
      &pushsdbgv 3							//'Cafe2_BaseUIScreen'
      &pushsdbgm 4							//'m_screen'
      &pushsdbgm 129							//'missionMap'
      &pushsdbgm 190							//'OnContentLoaded'
      &pushsdbgv 3							//'Cafe2_BaseUIScreen'
      &pushsdbgm 4							//'m_screen'
      &pushsdbgm 129							//'missionMap'
      &pushbyte 2
      &pushregister 1
      &pushsdb 177							//'bind1DynamicParams'
      &callMethod
      &setMember
      &pushsdbgv 3							//'Cafe2_BaseUIScreen'
      &pushsdbgm 4							//'m_screen'
      &pushsdb 190							//'OnContentLoaded'
      &delete
      &pop
     label60:
    &end // of function 

    &setMember
    &pushregister 1
    &pushsdb 63							//'m_objectiveLines'
    &pushzero
    &pushsdb 143							//'Array'
    &new
    &setMember
    &pushregister 1
    &pushsdb 70							//'m_selectedObjectiveLineIndex'
    &pushbyte -1
    &setMember
    &pushregister 1
    &pushsdb 138							//'m_playerCount'
    &pushzero
    &setMember
    &pushregister 0
    &pushsdb 114							//'MAX_PLAYERS'
    &pushbyte 6
    &setMember
    &pushone
    &pushnull
    &pushglobalgv
    &pushsdbgm 1							//'igm_pauseMenu'
    &pushsdbgm 12							//'prototype'
    &pushbyte 3
    &pushsdb 237							//'ASSetPropFlags'
    &callFunction
   label61:
    &pop
  &end // of initMovieClip 78
&end
