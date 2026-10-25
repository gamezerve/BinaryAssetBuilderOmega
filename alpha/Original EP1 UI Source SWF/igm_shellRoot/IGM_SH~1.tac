movie 'E:\depot\CnC\RA3_EP1\PRODUC~1\Data\APTBUI~1\034A3~1.0-D\pc\Output\IGM_SH~1\\IGM_SH~1.eaf' &compressed // flash 7, total frames: 1, frame rate: 30 fps, 1365x768 px

  &frame 0
    &pushs 'screen'
    &pushthisgv
    &pushone
    &pushs 'igm_shellRoot'
    &new
    &varEquals
    &stop
  &end // of frame 0

  &defineMovieClip 1 // total frames: 0
  &end // of defineMovieClip 1
  
  &exportAssets
    1 &as '__Packages.Cafe2_BaseUIScreen'
  &end // of exportAssets
  
  &initMovieClip 1
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
  &end // of initMovieClip 1

  &defineMovieClip 2 // total frames: 0
  &end // of defineMovieClip 2
  
  &exportAssets
    2 &as '__Packages.igm_shellRoot'
  &end // of exportAssets
  
  &initMovieClip 2
    &constants '_global', 'igm_shellRoot', 'igm_shellRoot::igm_shellRoot() screen = ', 'Cafe2_BaseUIScreen', 'm_screen', 'm_thisClass', 'onScreenExit', 'bind0', 'gSM', 'setOnExitScreen', 'Object', 'QueryGameEngine?igm_shellRoot_mode', 'igm_shellRoot_mode', 'PauseMenu', 'ScenarioPauseMenu', 'LoadMenu', 'SaveMenu', 'SCREEN', 'IGM_PAUSE_MENU', 'IGM_SCENARIO_PAUSE_MENU', 'GAME', 'SAVELOADSCREEN_TYPE', 'ENUM', 'SAVELOADSCREEN_LOAD', 'FEG_SAVELOAD', 'SAVELOADSCREEN_SAVE', 'changeMainScreen', 'prototype', 'obj', 'fn', 'call', 'igm_shellRoot::onScreenExit()', 'clearData', 'ASSetPropFlags'  
    &pushglobalgv
    &pushsdbgm 1							//'igm_shellRoot'
    &not
    &not
    &jnz label7    
    &pushglobalgv
    &pushsdb 1							//'igm_shellRoot'
    &function2  (r:6='screen') (r:1='this', r:2='super', r:3='_global')
      &pushzero
      &pushregister 2
      &pushundef
      &callmp
      &pushsdb 2							//'igm_shellRoot::igm_shellRoot() screen = '
      &pushregister 6
      &add
      &trace
      &pushsdbgv 3							//'Cafe2_BaseUIScreen'
      &pushsdb 4							//'m_screen'
      &pushregister 6
      &setMember
      &pushsdbgv 3							//'Cafe2_BaseUIScreen'
      &pushsdb 5							//'m_thisClass'
      &pushregister 1
      &setMember
      &pushregister 1
      &pushregister 1
      &pushsdbgm 6							//'onScreenExit'
      &pushbyte 2
      &pushsdbgv 1							//'igm_shellRoot'
      &pushsdb 7							//'bind0'
      &callMethod
      &pushone
      &pushregister 3
      &pushsdbgm 8							//'gSM'
      &dcallmp 9							// setOnExitScreen()
      &pushzero
      &pushsdb 10							//'Object'
      &new
      &setRegister r:5
      &pop
      &pushsdb 11							//'QueryGameEngine?igm_shellRoot_mode'
      &pushregister 5
      &loadVariables
      &pushundef
      &setRegister r:4
      &pop
      &pushregister 5
      &pushsdbgm 12							//'igm_shellRoot_mode'
      &setRegister r:0
      &pushsdb 13							//'PauseMenu'
      &strictEquals
      &jnz label1      
      &pushregister 0
      &pushsdb 14							//'ScenarioPauseMenu'
      &strictEquals
      &jnz label2      
      &pushregister 0
      &pushsdb 15							//'LoadMenu'
      &strictEquals
      &jnz label3      
      &pushregister 0
      &pushsdb 16							//'SaveMenu'
      &strictEquals
      &jnz label4      
      &jmp label5      
     label1:
      &pushregister 3
      &pushsdbgm 17							//'SCREEN'
      &pushsdbgm 18							//'IGM_PAUSE_MENU'
      &setRegister r:4
      &pop
      &jmp label6      
     label2:
      &pushregister 3
      &pushsdbgm 17							//'SCREEN'
      &pushsdbgm 19							//'IGM_SCENARIO_PAUSE_MENU'
      &setRegister r:4
      &pop
      &jmp label6      
     label3:
      &pushregister 3
      &pushsdbgm 20							//'GAME'
      &pushsdb 21							//'SAVELOADSCREEN_TYPE'
      &pushregister 3
      &pushsdbgm 22							//'ENUM'
      &pushsdbgm 23							//'SAVELOADSCREEN_LOAD'
      &setMember
      &pushregister 3
      &pushsdbgm 17							//'SCREEN'
      &pushsdbgm 24							//'FEG_SAVELOAD'
      &setRegister r:4
      &pop
      &jmp label6      
     label4:
      &pushregister 3
      &pushsdbgm 20							//'GAME'
      &pushsdb 21							//'SAVELOADSCREEN_TYPE'
      &pushregister 3
      &pushsdbgm 22							//'ENUM'
      &pushsdbgm 25							//'SAVELOADSCREEN_SAVE'
      &setMember
      &pushregister 3
      &pushsdbgm 17							//'SCREEN'
      &pushsdbgm 24							//'FEG_SAVELOAD'
      &setRegister r:4
      &pop
      &jmp label6      
     label5:
      &getURL 'FSCommand:CallGameFunction'       '%igm_shellRoot_onClosed'      
      &pushundef
      &return
     label6:
      &getURL 'FSCommand:CallGameFunction'       '%igm_shellRoot_onChildLoaded'      
      &pushregister 4
      &pushone
      &pushregister 3
      &pushsdbgm 8							//'gSM'
      &dcallmp 26							// changeMainScreen()
    &end // of function 

    &setRegister r:0
    &setMember
    &pushglobalgv
    &pushsdbgm 1							//'igm_shellRoot'
    &pushsdbgv 3							//'Cafe2_BaseUIScreen'
    &extends
    &pushregister 0
    &pushsdbgm 27							//'prototype'
    &setRegister r:1
    &pop
    &pushregister 0
    &pushsdb 7							//'bind0'
    &function  ('fn', 'obj'    )    
      &function2  (r:1='arg0') ()
        &pushregister 1
        &pushsdbgv 28							//'obj'
        &pushbyte 2
        &pushsdbgv 29							//'fn'
        &pushsdb 30							//'call'
        &callMethod
        &return
      &end // of function 

      &return
    &end // of function 

    &setMember
    &pushregister 1
    &pushsdb 6							//'onScreenExit'
    &function2  () (r:1='this', r:2='_global')
      &pushsdb 31							//'igm_shellRoot::onScreenExit()'
      &trace
      &pushzero
      &pushregister 1
      &dcallmp 32							// clearData()
      &pushsdbgv 3							//'Cafe2_BaseUIScreen'
      &pushsdb 5							//'m_thisClass'
      &pushnull
      &setMember
      &pushsdbgv 3							//'Cafe2_BaseUIScreen'
      &pushsdb 4							//'m_screen'
      &pushnull
      &setMember
      &pushregister 2
      &pushsdb 1							//'igm_shellRoot'
      &delete
      &pop
      &pushregister 2
      &pushsdb 3							//'Cafe2_BaseUIScreen'
      &delete
      &pop
    &end // of function 

    &setMember
    &pushone
    &pushnull
    &pushglobalgv
    &pushsdbgm 1							//'igm_shellRoot'
    &pushsdbgm 27							//'prototype'
    &pushbyte 3
    &pushsdb 33							//'ASSetPropFlags'
    &callFunction
   label7:
    &pop
  &end // of initMovieClip 2
&end
