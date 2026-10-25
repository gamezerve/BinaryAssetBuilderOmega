movie 'E:\depot\CnC\RA3_EP1\PRODUC~1\Data\APTBUI~1\034A3~1.0-D\pc\Output\FEG_M_~4\\FEG_M_~1.eaf' // flash 7, total frames: 1, frame rate: 30 fps, 1365x768 px
  
  &exportAssets
    2 &as 'FullScreenButtonGuard'
  &end // of exportAssets

  &frame 0
    &pushs 'screen'
    &pushthisgv
    &pushone
    &pushs 'feg_m_main'
    &new
    &varEquals
    &stop
  &end // of frame 0

  &defineMovieClip 3 // total frames: 0
  &end // of defineMovieClip 3
  
  &exportAssets
    3 &as '__Packages.feg_m_main'
  &end // of exportAssets
  
  &initMovieClip 3
    &constants '_global', 'feg_m_main', 'm_screen', 'm_messageHandler', 'localMessageHandler', 'bind1DynamicParams', 'gMH', 'addMessageHandler', 'prototype', 'shutdown', 'removeMessageHandler', 'onChildLoad', 'MSGCODE', 'FE_LOAD_SHELL', 'FE_UNLOAD', 'loadContent', 'unloadContent', 'LAYER_BACKGROUNDSCREEN', 'backgroundMC', 'createEmptyMovieClip', 'LAYER_SHELLSCREEN', 'shellScreenMC', 'm_buttonGuard', 'LAYER_BUTTONGUARD', 'transitionButtonGuard', 'FullScreenButtonGuard', 'attachMovie', '_alpha', '_visible', 'SCREEN', 'FEG_BACKGROUND', '.swf', 'loadMovie', 'hideBackgroundMovie', 'extern', 'InGame', 'FEG_MAIN_MENU', 'NONE', 'Unload shellScreenMC due to unloadContent() call.', 'unloadMovie', 'changeScreen', 'm_transitionScreenState', 'Start', 'screenTransition', 'doScreenTransition', 'm_transitionScreenIntervalId', 'clearInterval', 'm_transitionFunc', 'bind0', 'setInterval', 'FadeOut', 'ChangeScreen', 'FadeIn', 'Done', 'FADE_STEP', 'gCT', 'unregisterFromRoot', 'Unloading screen clip due to \'ChangeScreen\' transistion state.', 'onUnload', 'Screen\'s onUnload exists.', 'gSM', 'm_file', 'hideForeground', 'showForeground', 'showBackgroundMovie', 'backgroundUnload', 'screen', 'm_callbackCount', 'ASSetPropFlags'  
    &pushglobalgv
    &pushsdbgm 1							//'feg_m_main'
    &not
    &not
    &jnz label22    
    &pushglobalgv
    &pushsdb 1							//'feg_m_main'
    &function2  (r:3='screen') (r:1='this', r:2='_global')
      &pushregister 1
      &pushsdb 2							//'m_screen'
      &pushregister 3
      &setMember
      &pushregister 1
      &pushsdb 3							//'m_messageHandler'
      &pushregister 1
      &pushsdbgm 4							//'localMessageHandler'
      &pushregister 1
      &pushbyte 2
      &pushregister 2
      &pushsdb 5							//'bind1DynamicParams'
      &callMethod
      &setMember
      &pushregister 1
      &pushsdbgm 3							//'m_messageHandler'
      &pushone
      &pushsdbgv 6							//'gMH'
      &dcallmp 7							// addMessageHandler()
    &end // of function 

    &setRegister r:1
    &setMember
    &pushregister 1
    &pushsdbgm 8							//'prototype'
    &setRegister r:2
    &pop
    &pushregister 2
    &pushsdb 9							//'shutdown'
    &function2  () (r:1='this', r:2='_global')
      &pushregister 1
      &pushsdbgm 3							//'m_messageHandler'
      &pushone
      &pushsdbgv 6							//'gMH'
      &dcallmp 10							// removeMessageHandler()
      &pushregister 1
      &pushsdb 3							//'m_messageHandler'
      &delete
      &pop
      &pushregister 1
      &pushsdbgm 2							//'m_screen'
      &pushsdb 11							//'onChildLoad'
      &delete
      &pop
      &pushregister 2
      &pushsdb 1							//'feg_m_main'
      &delete
      &pop
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 4							//'localMessageHandler'
    &function2  (r:4='messageCode') (r:1='this', r:2='_global')
      &pushtrue
      &setRegister r:3
      &pop
      &pushregister 4
      &setRegister r:0
      &pushregister 2
      &pushsdbgm 12							//'MSGCODE'
      &pushsdbgm 13							//'FE_LOAD_SHELL'
      &strictEquals
      &jnz label1      
      &pushregister 0
      &pushregister 2
      &pushsdbgm 12							//'MSGCODE'
      &pushsdbgm 14							//'FE_UNLOAD'
      &strictEquals
      &jnz label2      
      &jmp label3      
     label1:
      &pushzero
      &pushregister 1
      &dcallmp 15							// loadContent()
      &jmp label4      
     label2:
      &pushzero
      &pushregister 1
      &dcallmp 16							// unloadContent()
      &jmp label4      
     label3:
      &pushfalse
      &setRegister r:3
      &pop
      &jmp label4      
     label4:
      &pushregister 3
      &return
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 15							//'loadContent'
    &function2  () (r:1='this', r:2='_global')
      &pushsdbgv 1							//'feg_m_main'
      &pushsdbgm 17							//'LAYER_BACKGROUNDSCREEN'
      &pushsdb 18							//'backgroundMC'
      &pushbyte 2
      &pushregister 1
      &pushsdbgm 2							//'m_screen'
      &dcallmp 19							// createEmptyMovieClip()
      &pushsdbgv 1							//'feg_m_main'
      &pushsdbgm 20							//'LAYER_SHELLSCREEN'
      &pushsdb 21							//'shellScreenMC'
      &pushbyte 2
      &pushregister 1
      &pushsdbgm 2							//'m_screen'
      &dcallmp 19							// createEmptyMovieClip()
      &pushregister 1
      &pushsdb 22							//'m_buttonGuard'
      &pushsdbgv 1							//'feg_m_main'
      &pushsdbgm 23							//'LAYER_BUTTONGUARD'
      &pushsdb 24							//'transitionButtonGuard'
      &pushsdb 25							//'FullScreenButtonGuard'
      &pushbyte 3
      &pushregister 1
      &pushsdbgm 2							//'m_screen'
      &pushsdb 26							//'attachMovie'
      &callMethod
      &setMember
      &pushregister 1
      &pushsdbgm 22							//'m_buttonGuard'
      &pushsdb 27							//'_alpha'
      &pushzero
      &setMember
      &pushregister 1
      &pushsdbgm 22							//'m_buttonGuard'
      &pushsdb 28							//'_visible'
      &pushfalse
      &setMember
      &pushregister 2
      &pushsdbgm 29							//'SCREEN'
      &pushsdbgm 30							//'FEG_BACKGROUND'
      &pushsdb 31							//'.swf'
      &add
      &pushone
      &pushregister 1
      &pushsdbgm 2							//'m_screen'
      &pushsdbgm 18							//'backgroundMC'
      &dcallmp 32							// loadMovie()
      &pushzero
      &pushregister 1
      &dcallmp 33							// hideBackgroundMovie()
      &pushsdbgv 34							//'extern'
      &pushsdbgm 35							//'InGame'
      &not
      &not
      &jnz label5      
      &pushregister 2
      &pushsdbgm 29							//'SCREEN'
      &pushsdbgm 36							//'FEG_MAIN_MENU'
      &pushsdb 31							//'.swf'
      &add
      &pushone
      &pushregister 1
      &pushsdbgm 2							//'m_screen'
      &pushsdbgm 21							//'shellScreenMC'
      &dcallmp 32							// loadMovie()
      &jmp label6      
     label5:
      &pushregister 2
      &pushsdbgm 29							//'SCREEN'
      &pushsdbgm 37							//'NONE'
      &pushone
      &pushregister 1
      &pushsdbgm 2							//'m_screen'
      &pushsdbgm 21							//'shellScreenMC'
      &dcallmp 32							// loadMovie()
     label6:
      &getURL 'FSCommand:CallGameFunction'       'OnFrameworkInitialized'      
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 16							//'unloadContent'
    &function2  () (r:1='this')
      &pushsdb 38							//'Unload shellScreenMC due to unloadContent() call.'
      &trace
      &pushzero
      &pushregister 1
      &pushsdbgm 2							//'m_screen'
      &pushsdbgm 18							//'backgroundMC'
      &dcallmp 39							// unloadMovie()
      &pushzero
      &pushregister 1
      &pushsdbgm 2							//'m_screen'
      &pushsdbgm 21							//'shellScreenMC'
      &dcallmp 39							// unloadMovie()
      &pushzero
      &pushregister 1
      &pushsdbgm 22							//'m_buttonGuard'
      &dcallmp 39							// unloadMovie()
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 40							//'changeScreen'
    &function2  () (r:1='this')
      &pushregister 1
      &pushsdb 41							//'m_transitionScreenState'
      &pushsdb 42							//'Start'
      &setMember
      &pushzero
      &pushregister 1
      &dcallmp 43							// screenTransition()
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 43							//'screenTransition'
    &function2  () (r:1='this', r:2='_global')
      &pushzero
      &pushregister 1
      &pushsdb 44							//'doScreenTransition'
      &callMethod
      &not
      &jnz label8      
      &pushregister 1
      &pushsdbgm 45							//'m_transitionScreenIntervalId'
      &pushbyte -1
      &equals
      &not
      &not
      &jnz label7      
      &pushregister 1
      &pushsdbgm 45							//'m_transitionScreenIntervalId'
      &pushone
      &dcallfp 46							// clearInterval()
      &pushregister 1
      &pushsdb 45							//'m_transitionScreenIntervalId'
      &pushbyte -1
      &setMember
      &pushregister 1
      &pushsdb 47							//'m_transitionFunc'
      &delete
      &pop
     label7:
      &jmp label9      
     label8:
      &pushregister 1
      &pushsdbgm 45							//'m_transitionScreenIntervalId'
      &pushbyte -1
      &equals
      &not
      &jnz label9      
      &pushregister 1
      &pushsdb 47							//'m_transitionFunc'
      &pushregister 1
      &pushsdbgm 43							//'screenTransition'
      &pushregister 1
      &pushbyte 2
      &pushregister 2
      &pushsdb 48							//'bind0'
      &callMethod
      &setMember
      &pushregister 1
      &pushsdb 45							//'m_transitionScreenIntervalId'
      &pushbyte 30
      &pushregister 1
      &pushsdbgm 47							//'m_transitionFunc'
      &pushbyte 2
      &pushsdb 49							//'setInterval'
      &callFunction
      &setMember
     label9:
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 44							//'doScreenTransition'
    &function2  () (r:1='this', r:2='_global')
      &pushregister 1
      &pushsdbgm 41							//'m_transitionScreenState'
      &setRegister r:0
      &pushsdb 42							//'Start'
      &strictEquals
      &jnz label10      
      &pushregister 0
      &pushsdb 50							//'FadeOut'
      &strictEquals
      &jnz label12      
      &pushregister 0
      &pushsdb 51							//'ChangeScreen'
      &strictEquals
      &jnz label15      
      &pushregister 0
      &pushsdb 52							//'FadeIn'
      &strictEquals
      &jnz label18      
      &pushregister 0
      &pushsdb 53							//'Done'
      &strictEquals
      &jnz label20      
      &jmp label21      
     label10:
      &pushregister 1
      &pushsdbgm 22							//'m_buttonGuard'
      &pushsdb 28							//'_visible'
      &pushtrue
      &setMember
      &pushregister 1
      &pushsdbgm 2							//'m_screen'
      &pushsdbgm 21							//'shellScreenMC'
      &pushundef
      &equals
      &not
      &jnz label11      
      &pushregister 1
      &pushsdb 41							//'m_transitionScreenState'
      &pushsdb 51							//'ChangeScreen'
      &setMember
      &pushzero
      &pushregister 1
      &pushsdb 44							//'doScreenTransition'
      &callMethod
      &return
     label11:
      &pushregister 1
      &pushsdb 41							//'m_transitionScreenState'
      &pushsdb 50							//'FadeOut'
      &setMember
      &pushregister 1
      &pushsdbgm 2							//'m_screen'
      &pushsdbgm 21							//'shellScreenMC'
      &pushsdb 27							//'_alpha'
      &pushregister 1
      &pushsdbgm 2							//'m_screen'
      &pushsdbgm 21							//'shellScreenMC'
      &pushsdbgm 27							//'_alpha'
      &pushsdbgv 1							//'feg_m_main'
      &pushsdbgm 54							//'FADE_STEP'
      &subtract
      &setMember
      &jmp label21      
     label12:
      &pushregister 1
      &pushsdbgm 2							//'m_screen'
      &pushsdbgm 21							//'shellScreenMC'
      &pushsdbgm 27							//'_alpha'
      &pushsdbgv 1							//'feg_m_main'
      &pushsdbgm 54							//'FADE_STEP'
      &greaterThan
      &not
      &not
      &jnz label13      
      &pushregister 1
      &pushsdbgm 2							//'m_screen'
      &pushsdbgm 21							//'shellScreenMC'
      &pushsdb 27							//'_alpha'
      &pushzero
      &setMember
      &pushregister 1
      &pushsdb 41							//'m_transitionScreenState'
      &pushsdb 51							//'ChangeScreen'
      &setMember
      &jmp label14      
     label13:
      &pushregister 1
      &pushsdbgm 2							//'m_screen'
      &pushsdbgm 21							//'shellScreenMC'
      &pushsdb 27							//'_alpha'
      &pushregister 1
      &pushsdbgm 2							//'m_screen'
      &pushsdbgm 21							//'shellScreenMC'
      &pushsdbgm 27							//'_alpha'
      &pushsdbgv 1							//'feg_m_main'
      &pushsdbgm 54							//'FADE_STEP'
      &subtract
      &setMember
     label14:
      &jmp label21      
     label15:
      &pushregister 1
      &pushsdbgm 2							//'m_screen'
      &pushsdbgm 21							//'shellScreenMC'
      &pushundef
      &equals
      &not
      &not
      &jnz label17      
      &pushregister 1
      &pushsdbgm 2							//'m_screen'
      &pushsdbgm 21							//'shellScreenMC'
      &pushone
      &pushregister 2
      &pushsdbgm 55							//'gCT'
      &dcallmp 56							// unregisterFromRoot()
      &pushsdb 57							//'Unloading screen clip due to \'ChangeScreen\' transistion state.'
      &trace
      &pushregister 1
      &pushsdbgm 2							//'m_screen'
      &pushsdbgm 21							//'shellScreenMC'
      &pushsdbgm 58							//'onUnload'
      &pushundef
      &equals
      &not
      &not
      &jnz label16      
      &pushsdb 59							//'Screen\'s onUnload exists.'
      &trace
      &pushzero
      &pushregister 1
      &pushsdbgm 2							//'m_screen'
      &pushsdbgm 21							//'shellScreenMC'
      &dcallmp 58							// onUnload()
     label16:
      &pushzero
      &pushregister 1
      &pushsdbgm 2							//'m_screen'
      &pushsdbgm 21							//'shellScreenMC'
      &dcallmp 39							// unloadMovie()
     label17:
      &pushsdbgv 1							//'feg_m_main'
      &pushsdbgm 20							//'LAYER_SHELLSCREEN'
      &pushsdb 21							//'shellScreenMC'
      &pushbyte 2
      &pushregister 1
      &pushsdbgm 2							//'m_screen'
      &dcallmp 19							// createEmptyMovieClip()
      &pushregister 2
      &pushsdbgm 60							//'gSM'
      &pushsdbgm 61							//'m_file'
      &pushone
      &pushregister 1
      &pushsdbgm 2							//'m_screen'
      &pushsdbgm 21							//'shellScreenMC'
      &dcallmp 32							// loadMovie()
      &pushregister 1
      &pushsdbgm 2							//'m_screen'
      &pushsdbgm 21							//'shellScreenMC'
      &pushsdb 27							//'_alpha'
      &pushzero
      &setMember
      &pushregister 1
      &pushsdb 41							//'m_transitionScreenState'
      &pushsdb 52							//'FadeIn'
      &setMember
      &jmp label21      
     label18:
      &pushregister 1
      &pushsdbgm 2							//'m_screen'
      &pushsdbgm 21							//'shellScreenMC'
      &pushsdb 27							//'_alpha'
      &pushregister 1
      &pushsdbgm 2							//'m_screen'
      &pushsdbgm 21							//'shellScreenMC'
      &pushsdbgm 27							//'_alpha'
      &pushsdbgv 1							//'feg_m_main'
      &pushsdbgm 54							//'FADE_STEP'
      &add
      &setMember
      &pushregister 1
      &pushsdbgm 2							//'m_screen'
      &pushsdbgm 21							//'shellScreenMC'
      &pushsdbgm 27							//'_alpha'
      &pushbyte 100
      &lessThan
      &not
      &not
      &jnz label19      
      &pushregister 1
      &pushsdb 41							//'m_transitionScreenState'
      &pushsdb 53							//'Done'
      &setMember
      &pushregister 1
      &pushsdbgm 2							//'m_screen'
      &pushsdbgm 21							//'shellScreenMC'
      &pushsdb 27							//'_alpha'
      &pushbyte 100
      &setMember
     label19:
      &jmp label21      
     label20:
      &pushregister 1
      &pushsdbgm 22							//'m_buttonGuard'
      &pushsdb 28							//'_visible'
      &pushfalse
      &setMember
      &pushtrue
      &return
     label21:
      &pushfalse
      &return
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 62							//'hideForeground'
    &function2  () (r:1='this')
      &pushregister 1
      &pushsdbgm 2							//'m_screen'
      &pushsdbgm 21							//'shellScreenMC'
      &pushsdb 28							//'_visible'
      &pushfalse
      &setMember
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 63							//'showForeground'
    &function2  () (r:1='this')
      &pushregister 1
      &pushsdbgm 2							//'m_screen'
      &pushsdbgm 21							//'shellScreenMC'
      &pushsdb 28							//'_visible'
      &pushtrue
      &setMember
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 33							//'hideBackgroundMovie'
    &function2  () (r:1='this')
      &pushregister 1
      &pushsdbgm 2							//'m_screen'
      &pushsdbgm 18							//'backgroundMC'
      &pushsdb 28							//'_visible'
      &pushfalse
      &setMember
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 64							//'showBackgroundMovie'
    &function2  () (r:1='this')
      &pushregister 1
      &pushsdbgm 2							//'m_screen'
      &pushsdbgm 18							//'backgroundMC'
      &pushsdb 28							//'_visible'
      &pushtrue
      &setMember
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 65							//'backgroundUnload'
    &function2  () (r:1='this')
      &pushzero
      &pushregister 1
      &pushsdbgm 2							//'m_screen'
      &pushsdbgm 18							//'backgroundMC'
      &pushsdbgm 66							//'screen'
      &dcallmp 58							// onUnload()
      &pushzero
      &pushregister 1
      &pushsdbgm 2							//'m_screen'
      &pushsdbgm 18							//'backgroundMC'
      &dcallmp 39							// unloadMovie()
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 67							//'m_callbackCount'
    &pushzero
    &setMember
    &pushregister 2
    &pushsdb 45							//'m_transitionScreenIntervalId'
    &pushbyte -1
    &setMember
    &pushregister 1
    &pushsdb 23							//'LAYER_BUTTONGUARD'
    &pushbyte 3
    &setMember
    &pushregister 1
    &pushsdb 20							//'LAYER_SHELLSCREEN'
    &pushbyte 2
    &setMember
    &pushregister 1
    &pushsdb 17							//'LAYER_BACKGROUNDSCREEN'
    &pushone
    &setMember
    &pushregister 1
    &pushsdb 54							//'FADE_STEP'
    &pushbyte 25
    &setMember
    &pushone
    &pushnull
    &pushglobalgv
    &pushsdbgm 1							//'feg_m_main'
    &pushsdbgm 8							//'prototype'
    &pushbyte 3
    &pushsdb 68							//'ASSetPropFlags'
    &callFunction
   label22:
    &pop
  &end // of initMovieClip 3
&end
