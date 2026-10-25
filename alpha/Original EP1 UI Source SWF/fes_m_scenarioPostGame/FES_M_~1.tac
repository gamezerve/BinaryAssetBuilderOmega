movie 'E:\depot\CnC\RA3_EP1\PRODUC~1\Data\APTBUI~1\034A3~1.0-D\pc\Output\FE021C~1\\FES_M_~1.eaf' &compressed // flash 7, total frames: 1, frame rate: 30 fps, 1365x768 px

  &frame 0
    &pushs 'screen'
    &pushthisgv
    &pushone
    &pushs 'fes_m_scenarioPostGame'
    &new
    &varEquals
    &stop
  &end // of frame 0

  &defineMovieClip 7 // total frames: 1
  &end // of defineMovieClip 7

  &defineMovieClip 12 // total frames: 1
  &end // of defineMovieClip 12

  &defineMovieClip 13 // total frames: 1600
  &end // of defineMovieClip 13

  &defineMovieClip 16 // total frames: 1
  &end // of defineMovieClip 16

  &defineMovieClip 20 // total frames: 1
  &end // of defineMovieClip 20

  &defineMovieClip 21 // total frames: 1
  &end // of defineMovieClip 21

  &defineMovieClip 22 // total frames: 800
  &end // of defineMovieClip 22

  &defineMovieClip 27 // total frames: 1
  &end // of defineMovieClip 27

  &defineMovieClip 30 // total frames: 1
  &end // of defineMovieClip 30

  &defineMovieClip 31 // total frames: 320
  &end // of defineMovieClip 31

  &defineMovieClip 34 // total frames: 1
  &end // of defineMovieClip 34

  &defineMovieClip 35 // total frames: 1
  &end // of defineMovieClip 35
  
  &importAssets &from '.\\Components\\fe_m_generalScreenComponents.swf'
    'lowerScreenBar Symbol' &as 36
  &end // of importAssets

  &defineMovieClip 41 // total frames: 1
  &end // of defineMovieClip 41

  &defineMovieClip 43 // total frames: 1
  &end // of defineMovieClip 43

  &defineMovieClip 46 // total frames: 1
  &end // of defineMovieClip 46

  &defineMovieClip 49 // total frames: 1
  &end // of defineMovieClip 49

  &defineMovieClip 50 // total frames: 15

    &frame 14
      &stop
    &end // of frame 14
  &end // of defineMovieClip 50

  &defineMovieClip 51 // total frames: 20

    &frame 8
      &stop
    &end // of frame 8

    &frame 19
      &stop
      &pushzero
      &pushsgv '_parent'
      &pushsgm 'screen'
      &pushs 'onStatItemRevealCompleted'
      &callmp
    &end // of frame 19
  &end // of defineMovieClip 51
  
  &importAssets &from '../cafe/mouseComponents/std_MouseButton.swf'
    'std_MousebuttonSymbol' &as 52
  &end // of importAssets

  &placeMovieClip 52 &as 'btnContinue'
  
    &onClipEvent &construct
      &constants 'm_width', 'm_textAlign', 'm_labelPosition', 'right align', 'm_label_x', 'm_label_y', 'm_focusDirs', 'Up/Down', 'm_iconType', 'default', 'm_extLabel', '$CONTINUE', 'm_nTruncateType', 'm_label', 'm_bEnabled', 'm_refFM', '_root.gFM', 'm_initiallySelected', 'm_tabIndex', 'm_bVisible', 'm_contentSymbol', 'buttonContentSymbol'  
      &pushsdb 0							//'m_width'
      &pushshort 200
      &setVariable
      &pushsdb 1							//'m_textAlign'
      &pushfalse
      &setVariable
      &pushsdb 2							//'m_labelPosition'
      &pushsdb 3							//'right align'
      &setVariable
      &pushsdb 4							//'m_label_x'
      &pushzerosv
      &pushsdb 5							//'m_label_y'
      &pushzerosv
      &pushsdb 6							//'m_focusDirs'
      &pushsdb 7							//'Up/Down'
      &setVariable
      &pushsdb 8							//'m_iconType'
      &pushsdb 9							//'default'
      &setVariable
      &pushsdb 10							//'m_extLabel'
      &pushsdb 11							//'$CONTINUE'
      &setVariable
      &pushsdb 12							//'m_nTruncateType'
      &pushzerosv
      &pushsdb 13							//'m_label'
      &pushsdb 11							//'$CONTINUE'
      &setVariable
      &pushsdb 14							//'m_bEnabled'
      &pushtrue
      &setVariable
      &pushsdb 15							//'m_refFM'
      &pushsdb 16							//'_root.gFM'
      &setVariable
      &pushsdb 17							//'m_initiallySelected'
      &pushfalse
      &setVariable
      &pushsdb 18							//'m_tabIndex'
      &pushzerosv
      &pushsdb 19							//'m_bVisible'
      &pushtrue
      &setVariable
      &pushsdb 20							//'m_contentSymbol'
      &pushsdb 21							//'buttonContentSymbol'
      &setVariable
    &end
  &end // of placeMovieClip 52
  
  &importAssets &from '.\\Components\\fe_m_generalScreenComponents.swf'
    'ScreenTitleBarSymbol' &as 56
  &end // of importAssets

  &placeMovieClip 56 
  
    &onClipEvent &construct
      &pushs 'm_title'
      &pushssv '$SCENARIO_POST_GAME_HEADER'
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
  &end // of placeMovieClip 56

  &defineMovieClip 57 // total frames: 0
  &end // of defineMovieClip 57
  
  &exportAssets
    57 &as '__Packages.Cafe2_BaseUIScreen'
  &end // of exportAssets
  
  &initMovieClip 57
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
  &end // of initMovieClip 57

  &defineMovieClip 58 // total frames: 0
  &end // of defineMovieClip 58
  
  &exportAssets
    58 &as '__Packages.fes_m_scenarioPostGame'
  &end // of exportAssets
  
  &initMovieClip 58
    &constants '_global', 'fes_m_scenarioPostGame', 'Cafe2_BaseUIScreen', 'm_thisClass', 'm_currentStatRevealIndex', 'm_gotoUnlockScreen', 'm_statArray', 'Array', 'm_statLabels', 'm_statValues', 'm_statRevealCountdown', 'STAT_REVEAL_DELAY', 'prototype', 'onStatItemRevealCompleted', 'NUM_STATS', 'GuiScenarioSummary', 'playSound', '_animate', 'gotoAndPlay', 'frameDelayedInit', 'onScreenExit', 'bind0', 'gSM', 'setOnExitScreen', 'getPostGameData', 'initGUI', 'Object', 'QueryGameEngine?SCENARIO_POST_GAME_DATA', ',', 'SCENARIO_POST_GAME_DATA_LABELS', 'split', 'SCENARIO_POST_GAME_DATA_VALUES', 'SCENARIO_POST_GAME_DATA_HAS_NEW_UNLOCK', '1', 'beginStatsRevealCountdown', 'm_screen', 'onEnterFrame', 'statsRevealTimerUpdate', 'playStatsMusic', 'QueryGameEngine?DID_LOCAL_PLAYER_WIN', 'DID_LOCAL_PLAYER_WIN', 'SOUND', 'STATS_WIN', 'PlayShellMusic', 'STATS_LOSE', 'i', 'this', 'statMC', '.statItem', 'push', 'labelMC', 'tfLabel', 'text', '&Outline', 'valueMC', 'tfValue', 'onContinueClicked', 'btnContinue', 'setOnMouseUpFunction', 'enable', 'show', 'noIntro', 'displayMissionName', 'clearData', 'SHELL', 'SCENARIO_UNLOCK_MODE', 'ENUM', 'SCENARIO_UNLOCK_MODE_SHOW_NEW_UNLOCK', 'SCREEN', 'FES_SCENARIO_UNLOCKS', 'changeMainScreen', 'gotoScenarioPostGameFlow', 'QueryGameEngine?SCENARIO_CURRENT_MISSION', 'QueryGameEngine?SCENARIO_PREFERRED_FACTION&SCENARIO_MISSION_DATA?Mission=', 'SCENARIO_CURRENT_MISSION', '$', 'SCENARIO_MISSION_DATA_NAME', 'mapNameTF', 'mapNameShadowTF', 'ASSetPropFlags'  
    &pushglobalgv
    &pushsdbgm 1							//'fes_m_scenarioPostGame'
    &not
    &not
    &jnz label10    
    &pushglobalgv
    &pushsdb 1							//'fes_m_scenarioPostGame'
    &function2  (r:3='screen') (r:1='this', r:2='super')
      &pushregister 3
      &pushone
      &pushregister 2
      &pushundef
      &callmp
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdb 3							//'m_thisClass'
      &pushregister 1
      &setMember
      &pushregister 1
      &pushsdb 4							//'m_currentStatRevealIndex'
      &pushzero
      &setMember
      &pushregister 1
      &pushsdb 5							//'m_gotoUnlockScreen'
      &pushfalse
      &setMember
      &pushregister 1
      &pushsdb 6							//'m_statArray'
      &pushzero
      &pushsdb 7							//'Array'
      &new
      &setMember
      &pushsdb 8							//'m_statLabels'
      &pushzero
      &pushsdb 7							//'Array'
      &new
      &setVariable
      &pushsdb 9							//'m_statValues'
      &pushzero
      &pushsdb 7							//'Array'
      &new
      &setVariable
      &pushregister 1
      &pushsdb 10							//'m_statRevealCountdown'
      &pushregister 1
      &pushsdbgm 11							//'STAT_REVEAL_DELAY'
      &setMember
    &end // of function 

    &setRegister r:1
    &setMember
    &pushglobalgv
    &pushsdbgm 1							//'fes_m_scenarioPostGame'
    &pushsdbgv 2							//'Cafe2_BaseUIScreen'
    &extends
    &pushregister 1
    &pushsdbgm 12							//'prototype'
    &setRegister r:2
    &pop
    &pushregister 2
    &pushsdb 13							//'onStatItemRevealCompleted'
    &function2  () (r:1='this', r:2='_global')
      &pushregister 1
      &pushsdb 4							//'m_currentStatRevealIndex'
      &pushregister 1
      &pushsdbgm 4							//'m_currentStatRevealIndex'
      &increment
      &setMember
      &pushregister 1
      &pushsdbgm 4							//'m_currentStatRevealIndex'
      &pushregister 1
      &pushsdbgm 14							//'NUM_STATS'
      &lessThan
      &not
      &jnz label1      
      &pushsdb 15							//'GuiScenarioSummary'
      &pushregister 1
      &pushsdbgm 4							//'m_currentStatRevealIndex'
      &pushone
      &add
      &toNumber
      &add
      &pushone
      &pushregister 2
      &dcallmp 16							// playSound()
      &pushsdb 17							//'_animate'
      &pushone
      &pushregister 1
      &pushsdbgm 6							//'m_statArray'
      &pushregister 1
      &pushsdbgm 4							//'m_currentStatRevealIndex'
      &getMember
      &dcallmp 18							// gotoAndPlay()
     label1:
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 19							//'frameDelayedInit'
    &function2  () (r:1='this', r:2='_global')
      &pushregister 1
      &pushsdbgm 20							//'onScreenExit'
      &pushregister 1
      &pushbyte 2
      &pushregister 2
      &pushsdb 21							//'bind0'
      &callMethod
      &pushone
      &pushregister 2
      &pushsdbgm 22							//'gSM'
      &dcallmp 23							// setOnExitScreen()
      &pushzero
      &pushregister 1
      &dcallmp 24							// getPostGameData()
      &pushzero
      &pushregister 1
      &dcallmp 25							// initGUI()
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 24							//'getPostGameData'
    &function2  () (r:1='this')
      &pushzero
      &pushsdb 26							//'Object'
      &new
      &setRegister r:2
      &pop
      &pushsdb 27							//'QueryGameEngine?SCENARIO_POST_GAME_DATA'
      &pushregister 2
      &loadVariables
      &pushsdb 8							//'m_statLabels'
      &pushsdb 28							//','
      &pushone
      &pushregister 2
      &pushsdbgm 29							//'SCENARIO_POST_GAME_DATA_LABELS'
      &dcallmsv 30							// split()
      &pushsdb 9							//'m_statValues'
      &pushsdb 28							//','
      &pushone
      &pushregister 2
      &pushsdbgm 31							//'SCENARIO_POST_GAME_DATA_VALUES'
      &dcallmsv 30							// split()
      &pushregister 1
      &pushsdb 5							//'m_gotoUnlockScreen'
      &pushregister 2
      &pushsdbgm 32							//'SCENARIO_POST_GAME_DATA_HAS_NEW_UNLOCK'
      &pushsdb 33							//'1'
      &equals
      &setMember
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 34							//'beginStatsRevealCountdown'
    &function2  () (r:1='this', r:2='_global')
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 35							//'m_screen'
      &pushsdb 36							//'onEnterFrame'
      &pushregister 1
      &pushsdbgm 37							//'statsRevealTimerUpdate'
      &pushregister 1
      &pushbyte 2
      &pushregister 2
      &pushsdb 21							//'bind0'
      &callMethod
      &setMember
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 37							//'statsRevealTimerUpdate'
    &function2  () (r:1='this', r:2='_global')
      &pushregister 1
      &pushsdbgm 10							//'m_statRevealCountdown'
      &pushzero
      &greaterThan
      &not
      &jnz label2      
      &pushregister 1
      &pushsdb 10							//'m_statRevealCountdown'
      &pushregister 1
      &pushsdbgm 10							//'m_statRevealCountdown'
      &decrement
      &setMember
      &jmp label3      
     label2:
      &pushregister 1
      &pushsdb 10							//'m_statRevealCountdown'
      &pushregister 1
      &pushsdbgm 10							//'m_statRevealCountdown'
      &decrement
      &setMember
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 35							//'m_screen'
      &pushsdb 36							//'onEnterFrame'
      &pushnull
      &setMember
      &pushregister 1
      &pushsdb 4							//'m_currentStatRevealIndex'
      &pushzero
      &setMember
      &pushsdb 15							//'GuiScenarioSummary'
      &pushregister 1
      &pushsdbgm 4							//'m_currentStatRevealIndex'
      &pushone
      &add
      &toNumber
      &add
      &pushone
      &pushregister 2
      &dcallmp 16							// playSound()
      &pushsdb 17							//'_animate'
      &pushone
      &pushregister 1
      &pushsdbgm 6							//'m_statArray'
      &pushregister 1
      &pushsdbgm 4							//'m_currentStatRevealIndex'
      &getMember
      &dcallmp 18							// gotoAndPlay()
     label3:
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 38							//'playStatsMusic'
    &function2  () (r:1='_global')
      &pushzero
      &pushsdb 26							//'Object'
      &new
      &setRegister r:2
      &pop
      &pushsdb 39							//'QueryGameEngine?DID_LOCAL_PLAYER_WIN'
      &pushregister 2
      &loadVariables
      &pushregister 2
      &pushsdbgm 40							//'DID_LOCAL_PLAYER_WIN'
      &pushsdb 33							//'1'
      &equals
      &not
      &jnz label4      
      &pushregister 1
      &pushsdbgm 41							//'SOUND'
      &pushsdbgm 42							//'STATS_WIN'
      &pushone
      &pushregister 1
      &dcallmp 43							// PlayShellMusic()
      &jmp label5      
     label4:
      &pushregister 1
      &pushsdbgm 41							//'SOUND'
      &pushsdbgm 44							//'STATS_LOSE'
      &pushone
      &pushregister 1
      &dcallmp 43							// PlayShellMusic()
     label5:
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 25							//'initGUI'
    &function  (    )    
      &pushsdb 45							//'i'
      &pushzero
      &varEquals
     label6:
      &pushsdbgv 45							//'i'
      &pushthisgv
      &pushsdbgm 14							//'NUM_STATS'
      &lessThan
      &not
      &jnz label7      
      &pushsdb 47							//'statMC'
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 35							//'m_screen'
      &toString
      &pushsdb 48							//'.statItem'
      &add
      &pushsdbgv 45							//'i'
      &add
      &getVariable
      &varEquals
      &pushsdbgv 47							//'statMC'
      &pushone
      &pushthisgv
      &pushsdbgm 6							//'m_statArray'
      &dcallmp 49							// push()
      &pushsdbgv 47							//'statMC'
      &pushsdbgm 50							//'labelMC'
      &pushsdbgm 51							//'tfLabel'
      &pushsdb 52							//'text'
      &pushsdbgv 8							//'m_statLabels'
      &pushsdbgv 45							//'i'
      &getMember
      &pushsdb 53							//'&Outline'
      &add
      &setMember
      &pushsdbgv 47							//'statMC'
      &pushsdbgm 54							//'valueMC'
      &pushsdbgm 55							//'tfValue'
      &pushsdb 52							//'text'
      &pushsdbgv 9							//'m_statValues'
      &pushsdbgv 45							//'i'
      &getMember
      &pushsdb 53							//'&Outline'
      &add
      &setMember
      &pushsdb 45							//'i'
      &pushsdbgv 45							//'i'
      &increment
      &setVariable
      &jmp label6      
     label7:
      &pushthisgv
      &pushsdbgm 56							//'onContinueClicked'
      &pushthisgv
      &pushbyte 2
      &pushglobalgv
      &pushsdb 21							//'bind0'
      &callMethod
      &pushone
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 35							//'m_screen'
      &pushsdbgm 57							//'btnContinue'
      &dcallmp 58							// setOnMouseUpFunction()
      &pushzero
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 35							//'m_screen'
      &pushsdbgm 57							//'btnContinue'
      &dcallmp 59							// enable()
      &pushzero
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 35							//'m_screen'
      &pushsdbgm 57							//'btnContinue'
      &dcallmp 60							// show()
      &pushzero
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 35							//'m_screen'
      &pushsdbgm 57							//'btnContinue'
      &dcallmp 61							// noIntro()
      &pushzero
      &pushthisgv
      &dcallmp 38							// playStatsMusic()
      &pushzero
      &pushthisgv
      &dcallmp 62							// displayMissionName()
      &pushzero
      &pushthisgv
      &dcallmp 34							// beginStatsRevealCountdown()
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 20							//'onScreenExit'
    &function2  () (r:1='this', r:2='_global')
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 35							//'m_screen'
      &pushsdb 36							//'onEnterFrame'
      &pushnull
      &setMember
      &pushzero
      &pushregister 1
      &dcallmp 63							// clearData()
      &pushregister 2
      &pushsdb 1							//'fes_m_scenarioPostGame'
      &delete
      &pop
      &pushregister 2
      &pushsdb 2							//'Cafe2_BaseUIScreen'
      &delete
      &pop
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 56							//'onContinueClicked'
    &function2  () (r:1='this', r:2='_global')
      &pushregister 1
      &pushsdbgm 5							//'m_gotoUnlockScreen'
      &not
      &jnz label8      
      &pushregister 2
      &pushsdbgm 64							//'SHELL'
      &pushsdb 65							//'SCENARIO_UNLOCK_MODE'
      &pushregister 2
      &pushsdbgm 66							//'ENUM'
      &pushsdbgm 67							//'SCENARIO_UNLOCK_MODE_SHOW_NEW_UNLOCK'
      &setMember
      &pushregister 2
      &pushsdbgm 68							//'SCREEN'
      &pushsdbgm 69							//'FES_SCENARIO_UNLOCKS'
      &pushone
      &pushregister 2
      &pushsdbgm 22							//'gSM'
      &dcallmp 70							// changeMainScreen()
      &jmp label9      
     label8:
      &pushzero
      &pushregister 2
      &dcallmp 71							// gotoScenarioPostGameFlow()
     label9:
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 62							//'displayMissionName'
    &function2  () ()
      &pushzero
      &pushsdb 26							//'Object'
      &new
      &setRegister r:1
      &pop
      &pushsdb 72							//'QueryGameEngine?SCENARIO_CURRENT_MISSION'
      &pushregister 1
      &loadVariables
      &pushzero
      &pushsdb 26							//'Object'
      &new
      &setRegister r:2
      &pop
      &pushsdb 73							//'QueryGameEngine?SCENARIO_PREFERRED_FACTION&SCENARIO_MISSION_DATA?Mission='
      &pushregister 1
      &pushsdbgm 74							//'SCENARIO_CURRENT_MISSION'
      &add
      &pushregister 2
      &loadVariables
      &pushsdb 75							//'$'
      &pushregister 2
      &pushsdbgm 76							//'SCENARIO_MISSION_DATA_NAME'
      &add
      &pushsdb 53							//'&Outline'
      &add
      &setRegister r:3
      &pop
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 35							//'m_screen'
      &pushsdbgm 77							//'mapNameTF'
      &pushsdb 52							//'text'
      &pushregister 3
      &setMember
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 35							//'m_screen'
      &pushsdbgm 78							//'mapNameShadowTF'
      &pushsdb 52							//'text'
      &pushregister 3
      &setMember
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 14							//'NUM_STATS'
    &pushbyte 7
    &setMember
    &pushregister 2
    &pushsdb 11							//'STAT_REVEAL_DELAY'
    &pushbyte 20
    &setMember
    &pushone
    &pushnull
    &pushglobalgv
    &pushsdbgm 1							//'fes_m_scenarioPostGame'
    &pushsdbgm 12							//'prototype'
    &pushbyte 3
    &pushsdb 79							//'ASSetPropFlags'
    &callFunction
   label10:
    &pop
  &end // of initMovieClip 58
&end
