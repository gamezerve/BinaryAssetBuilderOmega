movie 'E:\depot\CnC\RA3_EP1\PRODUC~1\Data\APTBUI~1\034A3~1.0-D\pc\Output\FE187D~1\\FES_M_~1.eaf' &compressed // flash 7, total frames: 1, frame rate: 30 fps, 1365x768 px
  
  &importAssets &from '.\\Components\\fe_m_generalScreenComponents.swf'
    'Lower_Stripe_mc' &as 1
  &end // of importAssets

  &defineMovieClip 4 // total frames: 1
  &end // of defineMovieClip 4

  &defineMovieClip 8 // total frames: 1
  &end // of defineMovieClip 8
  
  &exportAssets
    8 &as 'ScreenTitleBarSymbol'
  &end // of exportAssets

  &frame 0
    &pushs 'screen'
    &pushthisgv
    &pushone
    &pushs 'fes_m_scenarioComplete'
    &new
    &varEquals
    &stop
  &end // of frame 0
  
  &importAssets &from './screens/fe_m_assets.swf'
    'red sun full' &as 11
  &end // of importAssets

  &defineMovieClip 12 // total frames: 240
  &end // of defineMovieClip 12

  &defineMovieClip 17 // total frames: 1
  &end // of defineMovieClip 17

  &defineMovieClip 22 // total frames: 1
  &end // of defineMovieClip 22

  &defineMovieClip 23 // total frames: 1600
  &end // of defineMovieClip 23

  &defineMovieClip 26 // total frames: 1
  &end // of defineMovieClip 26

  &defineMovieClip 30 // total frames: 1
  &end // of defineMovieClip 30

  &defineMovieClip 31 // total frames: 1
  &end // of defineMovieClip 31

  &defineMovieClip 32 // total frames: 800
  &end // of defineMovieClip 32

  &defineMovieClip 37 // total frames: 1
  &end // of defineMovieClip 37

  &defineMovieClip 40 // total frames: 1
  &end // of defineMovieClip 40

  &defineMovieClip 41 // total frames: 320
  &end // of defineMovieClip 41

  &defineMovieClip 44 // total frames: 1
  &end // of defineMovieClip 44

  &defineMovieClip 45 // total frames: 1
  &end // of defineMovieClip 45
  
  &exportAssets
    8 &as 'ScreenTitleBarSymbol'
  &end // of exportAssets

  &placeMovieClip 8 
  
    &onClipEvent &construct
      &pushs 'm_title'
      &pushssv '$SCENARIO_COMPLETE_HEADER'
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
  &end // of placeMovieClip 8
  
  &importAssets &from '..\\common\\shell\\fe_shared_mainMenuLib.swf'
    'bombFactorySymbol' &as 46
  &end // of importAssets

  &placeMovieClip 46 
  
    &onClipEvent &construct
      &pushs 'm_maxBombs'
      &pushbyte 10
      &setVariable
      &pushs 'm_scale'
      &pushfloat 0.500000000000000
      &setVariable
      &pushs 'm_exploding'
      &pushfalse
      &setVariable
      &pushs 'm_stepSize'
      &pushbyte 8
      &setVariable
      &pushs 'm_contentMC'
      &pushssv 'frontBombMC'
    &end
  &end // of placeMovieClip 46

  &placeMovieClip 46 
  
    &onClipEvent &construct
      &pushs 'm_maxBombs'
      &pushbyte 40
      &setVariable
      &pushs 'm_scale'
      &pushfloat 0.250000000000000
      &setVariable
      &pushs 'm_exploding'
      &pushfalse
      &setVariable
      &pushs 'm_stepSize'
      &pushbyte 4
      &setVariable
      &pushs 'm_contentMC'
      &pushssv 'midBombMC'
    &end
  &end // of placeMovieClip 46

  &placeMovieClip 46 
  
    &onClipEvent &construct
      &pushs 'm_maxBombs'
      &pushbyte 10
      &setVariable
      &pushs 'm_scale'
      &pushfloat 0.100000001490116
      &setVariable
      &pushs 'm_exploding'
      &pushfalse
      &setVariable
      &pushs 'm_stepSize'
      &pushone
      &setVariable
      &pushs 'm_contentMC'
      &pushssv 'farBombMC'
    &end
  &end // of placeMovieClip 46

  &defineMovieClip 49 // total frames: 1
  &end // of defineMovieClip 49

  &defineMovieClip 52 // total frames: 1
  &end // of defineMovieClip 52

  &defineMovieClip 55 // total frames: 1
  &end // of defineMovieClip 55

  &defineMovieClip 61 // total frames: 1
  &end // of defineMovieClip 61

  &defineMovieClip 62 // total frames: 150
  &end // of defineMovieClip 62

  &defineMovieClip 65 // total frames: 1
  &end // of defineMovieClip 65

  &defineMovieClip 68 // total frames: 1
  &end // of defineMovieClip 68

  &defineMovieClip 71 // total frames: 1
  &end // of defineMovieClip 71

  &defineMovieClip 72 // total frames: 260
  &end // of defineMovieClip 72

  &defineMovieClip 73 // total frames: 43
  &end // of defineMovieClip 73

  &defineMovieClip 78 // total frames: 125
  &end // of defineMovieClip 78
  
  &importAssets &from '../cafe/mouseComponents/std_mouseScrollingListbox.swf'
    'std_mouseScrollingListboxSymbol' &as 80
  &end // of importAssets
  
  &importAssets &from '../cafe/mouseComponents/std_mouseScrollbar.swf'
    'std_mouseScrollbarSymbol' &as 81
  &end // of importAssets

  &defineMovieClip 85 // total frames: 1
  &end // of defineMovieClip 85

  &defineMovieClip 86 // total frames: 1

    &placeMovieClip 80 &as 'listbox'
    
      &onClipEvent &construct
        &pushs 'm_focusDirs'
        &pushssv 'Up/Down'
        &pushs 'm_refFM'
        &pushssv '_root.gFM'
        &pushs 'm_tabIndex'
        &pushbyte -1
        &setVariable
        &pushs 'm_bVisible'
        &pushtrue
        &setVariable
        &pushs 'm_bEnabled'
        &pushtrue
        &setVariable
        &pushs 'm_numVisibleEntries'
        &pushone
        &setVariable
        &pushs 'm_vPadding'
        &pushbyte 5
        &setVariable
        &pushs 'm_initiallySelected'
        &pushfalse
        &setVariable
        &pushs 'm_BgVisible'
        &pushtrue
        &setVariable
        &pushs 'm_renderWidth'
        &pushshort 692
        &setVariable
        &pushs 'm_renderHeight'
        &pushshort 242
        &setVariable
        &pushs 'm_backgroundMargin'
        &pushbyte 3
        &setVariable
        &pushs 'm_useHighlights'
        &pushtrue
        &setVariable
        &pushs 'm_alternateBackgroundColors'
        &pushfalse
        &setVariable
        &pushs 'm_backgroundColor1'
        &pushzerosv
        &pushs 'm_backgroundColor2'
        &pushzerosv
      &end
    &end // of placeMovieClip 80

    &placeMovieClip 81 &as 'scrollbar'
    
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
    &end // of placeMovieClip 81
  &end // of defineMovieClip 86
  
  &importAssets &from '../cafe/mouseComponents/std_MouseButton.swf'
    'std_MousebuttonSymbol' &as 87
  &end // of importAssets

  &placeMovieClip 87 &as 'btnContinue'
  
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
  &end // of placeMovieClip 87

  &defineMovieClip 88 // total frames: 0
  &end // of defineMovieClip 88
  
  &exportAssets
    88 &as '__Packages.Cafe2_BaseUIScreen'
  &end // of exportAssets
  
  &initMovieClip 88
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
  &end // of initMovieClip 88

  &defineMovieClip 89 // total frames: 0
  &end // of defineMovieClip 89
  
  &exportAssets
    89 &as '__Packages.fes_m_scenarioComplete'
  &end // of exportAssets
  
  &initMovieClip 89
    &constants '_global', 'fes_m_scenarioComplete', 'Cafe2_BaseUIScreen', 'm_thisClass', 'm_localEventHandlerIndex', 'm_localMessageHandlerFunc', 'm_localEventHandlerFunc', 'm_showCode', 'prototype', 'setup', 'localMessageHandler', 'bind1DynamicParams', 'gMH', 'addMessageHandler', 'localEventHandler', 'bind2DynamicParams', 'gEH', 'addLocalEventHandler', 'onScreenExit', 'bind0', 'gSM', 'setOnExitScreen', 'cleanup', 'removeMessageHandler', 'removeLocalEventHandler', 'clearData', 'frameDelayedInit', 'Object', 'QueryGameEngine?SCENARIO_COMPLETION_STATE', 'SCENARIO_COMPLETION_STATE_ALL_UNDER_PAR', '1', 'm_mainlineCompleted', 'SCENARIO_COMPLETION_STATE_CRITICAL_COMPLETE_FIRSTTIME', 'initGUI', '$APT:SCENARIO_COMPLETE_TEXT_MAINLINE&Outline', '$APT:SCENARIO_COMPLETE_TEXT_CODE&Outline', '$APT:SCENARIO_COMPLETE_TEXT&Outline', 'getContextDataObject', 'PLAY_COMPLETE_MOVIE', 'FSCommand:CallGameFunction', '%PlayMovieAtomic?movie=', 'COMPLETE_MOVIE_NAME', '|allowCancel=1', 'm_screen', 'frame', 'listbox', 'disableEntryHitRegion', 'symbol', 'std_mouseListboxEntryTextElementSymbol', 'width', 'leftMargin', 'rightMargin', 'fontSize', 'useHighlights', 'isSelectable', 'useHitRegion', 'autoSize', 'color', 'Array', 'init', 'scrollBar', 'attachScrollbarController', 'setColumnData', 'refreshDisplay', 'moveViewToFirst', 'scrollbar', 'enable', 'codeMC', '_visible', 'onContinueClicked', 'btnContinue', 'setOnMouseUpFunction', 'show', 'noIntro', 'SOUND', 'SCENARIO_COMPLETE', 'PlayShellMusic', 'GuiScenarioCompleteScreenReveal', 'playSound', 'SCREEN', 'FEG_MAIN_MENU', 'gotoShellScreen', 'gFM', 'handleDefaultInputs', 'ASSetPropFlags'  
    &pushglobalgv
    &pushsdbgm 1							//'fes_m_scenarioComplete'
    &not
    &not
    &jnz label11    
    &pushglobalgv
    &pushsdb 1							//'fes_m_scenarioComplete'
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
      &pushsdb 4							//'m_localEventHandlerIndex'
      &pushbyte -1
      &setMember
      &pushregister 1
      &pushsdb 5							//'m_localMessageHandlerFunc'
      &pushundef
      &setMember
      &pushregister 1
      &pushsdb 6							//'m_localEventHandlerFunc'
      &pushundef
      &setMember
      &pushregister 1
      &pushsdb 7							//'m_showCode'
      &pushfalse
      &setMember
    &end // of function 

    &setRegister r:1
    &setMember
    &pushglobalgv
    &pushsdbgm 1							//'fes_m_scenarioComplete'
    &pushsdbgv 2							//'Cafe2_BaseUIScreen'
    &extends
    &pushregister 1
    &pushsdbgm 8							//'prototype'
    &setRegister r:2
    &pop
    &pushregister 2
    &pushsdb 9							//'setup'
    &function2  () (r:1='this', r:2='_global')
      &pushregister 1
      &pushsdb 5							//'m_localMessageHandlerFunc'
      &pushregister 1
      &pushsdbgm 10							//'localMessageHandler'
      &pushregister 1
      &pushbyte 2
      &pushregister 2
      &pushsdb 11							//'bind1DynamicParams'
      &callMethod
      &setMember
      &pushregister 1
      &pushsdbgm 5							//'m_localMessageHandlerFunc'
      &pushone
      &pushregister 2
      &pushsdbgm 12							//'gMH'
      &dcallmp 13							// addMessageHandler()
      &pushregister 1
      &pushsdb 6							//'m_localEventHandlerFunc'
      &pushregister 1
      &pushsdbgm 14							//'localEventHandler'
      &pushregister 1
      &pushbyte 2
      &pushregister 2
      &pushsdb 15							//'bind2DynamicParams'
      &callMethod
      &setMember
      &pushregister 1
      &pushsdb 4							//'m_localEventHandlerIndex'
      &pushregister 1
      &pushsdbgm 6							//'m_localEventHandlerFunc'
      &pushone
      &pushregister 2
      &pushsdbgm 16							//'gEH'
      &pushsdb 17							//'addLocalEventHandler'
      &callMethod
      &setMember
      &pushregister 1
      &pushsdbgm 18							//'onScreenExit'
      &pushregister 1
      &pushbyte 2
      &pushregister 2
      &pushsdb 19							//'bind0'
      &callMethod
      &pushone
      &pushregister 2
      &pushsdbgm 20							//'gSM'
      &dcallmp 21							// setOnExitScreen()
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 22							//'cleanup'
    &function2  () (r:1='this', r:2='_global')
      &pushregister 1
      &pushsdbgm 5							//'m_localMessageHandlerFunc'
      &pushundef
      &equals
      &not
      &not
      &jnz label1      
      &pushregister 1
      &pushsdbgm 5							//'m_localMessageHandlerFunc'
      &pushone
      &pushregister 2
      &pushsdbgm 12							//'gMH'
      &dcallmp 23							// removeMessageHandler()
      &pushregister 1
      &pushsdb 5							//'m_localMessageHandlerFunc'
      &delete
      &pop
      &pushregister 1
      &pushsdb 5							//'m_localMessageHandlerFunc'
      &pushundef
      &setMember
     label1:
      &pushregister 1
      &pushsdbgm 4							//'m_localEventHandlerIndex'
      &pushbyte -1
      &equals
      &not
      &not
      &jnz label2      
      &pushregister 1
      &pushsdbgm 4							//'m_localEventHandlerIndex'
      &pushone
      &pushregister 2
      &pushsdbgm 16							//'gEH'
      &dcallmp 24							// removeLocalEventHandler()
      &pushregister 1
      &pushsdb 6							//'m_localEventHandlerFunc'
      &delete
      &pop
      &pushregister 1
      &pushsdb 6							//'m_localEventHandlerFunc'
      &pushundef
      &setMember
      &pushregister 1
      &pushsdb 4							//'m_localEventHandlerIndex'
      &pushbyte -1
      &setMember
     label2:
      &pushzero
      &pushregister 1
      &dcallmp 25							// clearData()
      &pushregister 2
      &pushsdb 1							//'fes_m_scenarioComplete'
      &delete
      &pop
      &pushregister 2
      &pushsdb 2							//'Cafe2_BaseUIScreen'
      &delete
      &pop
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 26							//'frameDelayedInit'
    &function2  () (r:1='this')
      &pushzero
      &pushregister 1
      &dcallmp 9							// setup()
      &pushzero
      &pushsdb 27							//'Object'
      &new
      &setRegister r:2
      &pop
      &pushsdb 28							//'QueryGameEngine?SCENARIO_COMPLETION_STATE'
      &pushregister 2
      &loadVariables
      &pushregister 1
      &pushsdb 7							//'m_showCode'
      &pushregister 2
      &pushsdbgm 29							//'SCENARIO_COMPLETION_STATE_ALL_UNDER_PAR'
      &toNumber
      &pushsdb 30							//'1'
      &equals
      &setMember
      &pushregister 1
      &pushsdb 31							//'m_mainlineCompleted'
      &pushregister 2
      &pushsdbgm 32							//'SCENARIO_COMPLETION_STATE_CRITICAL_COMPLETE_FIRSTTIME'
      &toNumber
      &pushsdb 30							//'1'
      &equals
      &setMember
      &pushzero
      &pushregister 1
      &dcallmp 33							// initGUI()
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 33							//'initGUI'
    &function2  () (r:1='this', r:2='_global')
      &pushundef
      &setRegister r:4
      &pop
      &pushregister 1
      &pushsdbgm 31							//'m_mainlineCompleted'
      &not
      &jnz label3      
      &pushsdb 34							//'$APT:SCENARIO_COMPLETE_TEXT_MAINLINE&Outline'
      &setRegister r:4
      &pop
      &jmp label5      
     label3:
      &pushregister 1
      &pushsdbgm 7							//'m_showCode'
      &not
      &jnz label4      
      &pushsdb 35							//'$APT:SCENARIO_COMPLETE_TEXT_CODE&Outline'
      &setRegister r:4
      &pop
      &jmp label5      
     label4:
      &pushsdb 36							//'$APT:SCENARIO_COMPLETE_TEXT&Outline'
      &setRegister r:4
      &pop
     label5:
      &pushzero
      &pushsdb 27							//'Object'
      &new
      &setRegister r:5
      &pop
      &pushzero
      &pushregister 2
      &pushsdbgm 20							//'gSM'
      &pushsdb 37							//'getContextDataObject'
      &callMethod
      &setRegister r:5
      &pop
      &pushregister 5
      &pushsdbgm 38							//'PLAY_COMPLETE_MOVIE'
      &setRegister r:6
      &pop
      &pushregister 6
      &not
      &jnz label6      
      &pushsdb 39							//'FSCommand:CallGameFunction'
      &pushsdb 40							//'%PlayMovieAtomic?movie='
      &pushregister 5
      &pushsdbgm 41							//'COMPLETE_MOVIE_NAME'
      &add
      &pushsdb 42							//'|allowCancel=1'
      &add
      &getURL2
     label6:
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 43							//'m_screen'
      &pushsdbgm 44							//'frame'
      &setRegister r:3
      &pop
      &pushzero
      &pushregister 3
      &pushsdbgm 45							//'listbox'
      &dcallmp 46							// disableEntryHitRegion()
      &pushsdb 47							//'symbol'
      &pushsdb 48							//'std_mouseListboxEntryTextElementSymbol'
      &pushsdb 49							//'width'
      &pushshort 668
      &pushsdb 50							//'leftMargin'
      &pushbyte 4
      &pushsdb 51							//'rightMargin'
      &pushbyte 4
      &pushsdb 52							//'fontSize'
      &pushbyte 16
      &pushsdb 53							//'useHighlights'
      &pushfalse
      &pushsdb 54							//'isSelectable'
      &pushfalse
      &pushsdb 55							//'useHitRegion'
      &pushfalse
      &pushsdb 56							//'autoSize'
      &pushtrue
      &pushsdb 57							//'color'
      &pushlong 16036194
      &pushbyte 10
      &initObject
      &pushone
      &pushsdb 58							//'Array'
      &new
      &setRegister r:8
      &pop
      &pushbyte 20
      &pushregister 8
      &pushbyte 2
      &pushregister 3
      &pushsdbgm 45							//'listbox'
      &dcallmp 59							// init()
      &pushregister 3
      &pushsdbgm 60							//'scrollBar'
      &pushone
      &pushregister 3
      &pushsdbgm 45							//'listbox'
      &dcallmp 61							// attachScrollbarController()
      &pushregister 4
      &pushone
      &pushsdb 58							//'Array'
      &new
      &setRegister r:7
      &pop
      &pushregister 7
      &pushzero
      &pushbyte 2
      &pushregister 3
      &pushsdbgm 45							//'listbox'
      &dcallmp 62							// setColumnData()
      &pushzero
      &pushregister 3
      &pushsdbgm 45							//'listbox'
      &dcallmp 63							// refreshDisplay()
      &pushzero
      &pushregister 3
      &pushsdbgm 45							//'listbox'
      &dcallmp 64							// moveViewToFirst()
      &pushzero
      &pushregister 3
      &pushsdbgm 65							//'scrollbar'
      &dcallmp 66							// enable()
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 43							//'m_screen'
      &pushsdbgm 67							//'codeMC'
      &pushsdb 68							//'_visible'
      &pushregister 1
      &pushsdbgm 7							//'m_showCode'
      &setMember
      &pushregister 1
      &pushsdbgm 69							//'onContinueClicked'
      &pushregister 1
      &pushbyte 2
      &pushregister 2
      &pushsdb 19							//'bind0'
      &callMethod
      &pushone
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 43							//'m_screen'
      &pushsdbgm 70							//'btnContinue'
      &dcallmp 71							// setOnMouseUpFunction()
      &pushzero
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 43							//'m_screen'
      &pushsdbgm 70							//'btnContinue'
      &dcallmp 66							// enable()
      &pushzero
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 43							//'m_screen'
      &pushsdbgm 70							//'btnContinue'
      &dcallmp 72							// show()
      &pushzero
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 43							//'m_screen'
      &pushsdbgm 70							//'btnContinue'
      &dcallmp 73							// noIntro()
      &pushregister 2
      &pushsdbgm 74							//'SOUND'
      &pushsdbgm 75							//'SCENARIO_COMPLETE'
      &pushone
      &pushregister 2
      &dcallmp 76							// PlayShellMusic()
      &pushsdb 77							//'GuiScenarioCompleteScreenReveal'
      &pushone
      &pushregister 2
      &dcallmp 78							// playSound()
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 18							//'onScreenExit'
    &function2  () (r:1='this')
      &pushzero
      &pushregister 1
      &dcallmp 22							// cleanup()
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 69							//'onContinueClicked'
    &function2  () (r:1='_global')
      &getURL 'FSCommand:CallGameFunction'       '%ScenarioClearMission'      
      &pushzero
      &initArray
      &pushregister 1
      &pushsdbgm 79							//'SCREEN'
      &pushsdbgm 80							//'FEG_MAIN_MENU'
      &pushbyte 2
      &pushregister 1
      &dcallmp 81							// gotoShellScreen()
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 10							//'localMessageHandler'
    &function2  (r:2='message') ()
      &pushtrue
      &setRegister r:1
      &pop
      &jmp label7      
     label7:
      &pushfalse
      &setRegister r:1
      &pop
      &jmp label8      
     label8:
      &pushregister 1
      &return
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 14							//'localEventHandler'
    &function2  (r:2='key', 'controller') (r:1='_global')
      &jmp label9      
     label9:
      &pushregister 2
      &pushone
      &pushregister 1
      &pushsdbgm 82							//'gFM'
      &dcallmp 83							// handleDefaultInputs()
      &jmp label10      
     label10:
    &end // of function 

    &setMember
    &pushone
    &pushnull
    &pushglobalgv
    &pushsdbgm 1							//'fes_m_scenarioComplete'
    &pushsdbgm 8							//'prototype'
    &pushbyte 3
    &pushsdb 84							//'ASSetPropFlags'
    &callFunction
   label11:
    &pop
  &end // of initMovieClip 89
&end
