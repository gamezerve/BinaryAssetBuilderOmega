movie 'fes_m_chapterSelect.eaf' // flash 7, total frames: 1, frame rate: 30 fps, 1365x768 px

  &frame 0
    &push 'screen'    
    &push 'this'    
    &getVariable
    &push 1    
    &push 'fes_m_chapterSelect'    
    &new
    &varEquals
    &stop
  &end // of frame 0
  
  &importAssets &from '.\\Components\\fe_m_generalScreenComponents.swf'
    'lowerScreenBar Symbol' &as 3
  &end // of importAssets
  
  &importAssets &from '../cafe/mouseComponents/std_MouseButton.swf'
    'std_MousebuttonSymbol' &as 6
  &end // of importAssets

  &placeMovieClip 6 &as 'button_Back'
  
    &onClipEvent &construct
      &push 'm_focusDirs'      
      &push 'Up/Down'      
      &setVariable
      &push 'm_bEnabled'      
      &push &TRUE      
      &setVariable
      &push 'm_refFM'      
      &push '_root.gFM'      
      &setVariable
      &push 'm_initiallySelected'      
      &push &FALSE      
      &setVariable
      &push 'm_tabIndex'      
      &push 1      
      &setVariable
      &push 'm_bVisible'      
      &push &TRUE      
      &setVariable
      &push 'm_label'      
      &push '$BACK'      
      &setVariable
      &push 'm_contentSymbol'      
      &push 'buttonContentSymbol'      
      &setVariable
    &end
  &end // of placeMovieClip 6

  &placeMovieClip 6 &as 'button_quickLaunch'
  
    &onClipEvent &construct
      &constants 'm_focusDirs', 'Up/Down', 'm_bEnabled', 'm_refFM', '_root.gFM', 'm_initiallySelected', 'm_tabIndex', 'm_bVisible', 'm_label', '$QUICKPLAY', 'm_contentSymbol', 'buttonContentSymbol'  
      &push 'm_focusDirs'      
      &push 'Up/Down'      
      &setVariable
      &push 'm_bEnabled'      
      &push &TRUE      
      &setVariable
      &push 'm_refFM'      
      &push '_root.gFM'      
      &setVariable
      &push 'm_initiallySelected'      
      &push &FALSE      
      &setVariable
      &push 'm_tabIndex'      
      &push 0      
      &setVariable
      &push 'm_bVisible'      
      &push &TRUE      
      &setVariable
      &push 'm_label'      
      &push '$QUICKPLAY'      
      &setVariable
      &push 'm_contentSymbol'      
      &push 'buttonContentSymbol'      
      &setVariable
    &end
  &end // of placeMovieClip 6

  &placeMovieClip 6 &as 'button_Play'
  
    &onClipEvent &construct
      &push 'm_focusDirs'      
      &push 'Up/Down'      
      &setVariable
      &push 'm_bEnabled'      
      &push &TRUE      
      &setVariable
      &push 'm_refFM'      
      &push '_root.gFM'      
      &setVariable
      &push 'm_initiallySelected'      
      &push &FALSE      
      &setVariable
      &push 'm_tabIndex'      
      &push 0.0      
      &setVariable
      &push 'm_bVisible'      
      &push &TRUE      
      &setVariable
      &push 'm_label'      
      &push '$PLAY'      
      &setVariable
      &push 'm_contentSymbol'      
      &push 'buttonContentSymbol'      
      &setVariable
    &end
  &end // of placeMovieClip 6
  
  &importAssets &from '../mouse/Components/mouse_chapterSelectComponent.swf'
    'chapterSelectComponentSymbol' &as 7
  &end // of importAssets
  
  &importAssets &from 'screens/fe_m_assetsGameSetup.swf'
    'MapPreviewComponent' &as 10
  &end // of importAssets

  &placeMovieClip 10 &as 'mapPreview'
  
    &onClipEvent &construct
      &push '_type'      
      &push 'ChapterMapPreview'      
      &setVariable
      &push '_indexMap'      
      &push -1      
      &setVariable
    &end
  &end // of placeMovieClip 10

  &defineMovieClip 27 // total frames: 1
  &end // of defineMovieClip 27
  
  &importAssets &from '../cafe/mouseComponents/std_mouseDropDownMenu.swf'
    'std_mouseDropDownMenuSymbol' &as 24
  &end // of importAssets

  &placeMovieClip 24 &as 'ddDifficulty'
  
    &onClipEvent &construct
      &constants 'm_focusDirs', 'Up/Down', 'm_refFM', '_root.gFM', 'm_initiallySelected', 'm_tabIndex', 'm_bVisible', 'm_visibleEntryCount', 'm_width', 'm_contentSymbol', 'TextDropdownMenuContentSymbol', 'm_entrySymbol', 'std_mouseListboxEntryTextElementSymbol', 'm_ButtonWidth'  
      &push 'm_focusDirs'      
      &push 'Up/Down'      
      &setVariable
      &push 'm_refFM'      
      &push '_root.gFM'      
      &setVariable
      &push 'm_initiallySelected'      
      &push &FALSE      
      &setVariable
      &push 'm_tabIndex'      
      &push -1      
      &setVariable
      &push 'm_bVisible'      
      &push &TRUE      
      &setVariable
      &push 'm_visibleEntryCount'      
      &push 3      
      &setVariable
      &push 'm_width'      
      &push 150      
      &setVariable
      &push 'm_contentSymbol'      
      &push 'TextDropdownMenuContentSymbol'      
      &setVariable
      &push 'm_entrySymbol'      
      &push 'std_mouseListboxEntryTextElementSymbol'      
      &setVariable
      &push 'm_ButtonWidth'      
      &push 27      
      &setVariable
    &end
  &end // of placeMovieClip 24
  
  &importAssets &from '.\\Components\\fe_m_generalScreenComponents.swf'
    'ScreenTitleBarSymbol' &as 13
  &end // of importAssets

  &placeMovieClip 13 
  
    &onClipEvent &construct
      &push 'm_title'      
      &push '$HEADERCHAPTERSELECT'      
      &setVariable
      &push 'm_DropShadow'      
      &push &TRUE      
      &setVariable
      &push 'm_Outline'      
      &push &TRUE      
      &setVariable
      &push 'm_visible'      
      &push &TRUE      
      &setVariable
    &end
  &end // of placeMovieClip 13

  &defineMovieClip 14 // total frames: 0
  &end // of defineMovieClip 14
  
  &exportAssets
    14 &as '__Packages.Cafe2_BaseUIScreen'
  &end // of exportAssets
  
  &initMovieClip 14
    &constants '_global', 'Cafe2_BaseUIScreen', 'm_screen', 'onEnterFrame', 'frameDelayedInitCallback', 'bind0', 'prototype', 'frameDelayedInit', '\n\nCafe2_BaseUIScreen::frameDelayedInitCallback() :', '\nscreens and move the call to initGUI() to frameDelayedInit().\n', 'N.B. You *must* override frameDelayedInit() in your', 'Array', 'outtroComplete', 'Cafe2_BaseUIScreen::outtroComplete() ', 'm_thisClass', 'm_uiComponentArray', 'length', 'splice', 'm_changeToScreen', 'gSM', 'changeMainScreen', 'introComplete', 'Cafe2_BaseUIScreen::introComplete()', 'restoreVisualState', 'm_introCount', 'onIntrosComplete', 'Cafe2_BaseUIScreen::onIntrosComplete()', 'localEventHandler', 'gEH', 'setLocalEventHandler', 'enableCurrentLocalEventHandler', 'registerIntroOuttroComponents', 'Cafe2_BaseUIScreen::registerIntroOuttroComponents()', 'setIntroCallback', 'executeBackFunction', 'Cafe2_BaseUIScreen::executeBackFunction', 'goBackScreen', 'startIntro', 'Cafe2_BaseUIScreen::startIntro()', 'intro', 'startOuttro', 'Cafe2_BaseUIScreen::startOuttro', 'outtro', 'startOuttroAndCallBack', 'm_executeOuttroCallBack', 'outtroAndCallBackComplete', 'backButtonClicked', 'Cafe2_BaseUIScreen::backButtonClicked()', 'Cafe2_BaseUIScreen::localEventHandler() keyCode == ', 'gFM', 'handleDefaultInputs', 'clearData', 'obj', 'func', 'call', 'ASSetPropFlags'  
    &push '_global'    
    &getVariable
    &push 'Cafe2_BaseUIScreen'    
    &getMember
    &not
    &not
    &jnz label30    
    &push '_global'    
    &getVariable
    &push 'Cafe2_BaseUIScreen'    
    &function2  (r:3='screen') (r:1='this', r:2='_global')
      &push 'Cafe2_BaseUIScreen'      
      &getVariable
      &push 'm_screen'      
      &push r:3      
      &setMember
      &push 'Cafe2_BaseUIScreen'      
      &getVariable
      &push 'm_screen'      
      &getMember
      &push 'onEnterFrame'      
      &push r:1      
      &push 'frameDelayedInitCallback'      
      &getMember
      &push r:1      
      &push 2      
      &push r:2      
      &push 'bind0'      
      &callMethod
      &setMember
    &end // of function 

    &setRegister r:0
    &setMember
    &push r:0    
    &push 'prototype'    
    &getMember
    &setRegister r:1
    &pop
    &push r:1    
    &push 'frameDelayedInit'    
    &function2  () ()
      &push '\n\nCafe2_BaseUIScreen::frameDelayedInitCallback() :'      
      &trace
      &push '\nscreens and move the call to initGUI() to frameDelayedInit().\n'      
      &push 'N.B. You *must* override frameDelayedInit() in your'      
      &push 2      
      &push 'Array'      
      &new
      &setRegister r:1
      &pop
      &push r:1      
      &push 0.0      
      &getMember
      &push r:1      
      &push 1      
      &getMember
      &add
      &trace
    &end // of function 

    &setMember
    &push r:1    
    &push 'frameDelayedInitCallback'    
    &function2  () (r:1='this')
      &push 'Cafe2_BaseUIScreen'      
      &getVariable
      &push 'm_screen'      
      &getMember
      &push 'onEnterFrame'      
      &pushnull      
      &setMember
      &push 0.0      
      &push r:1      
      &push 'frameDelayedInit'      
      &callMethod
      &pop
    &end // of function 

    &setMember
    &push r:1    
    &push 'outtroComplete'    
    &function2  (r:3='theComponent') (r:1='_global')
      &push 'Cafe2_BaseUIScreen::outtroComplete() '      
      &push r:3      
      &add
      &trace
      &push 0.0      
      &setRegister r:2
      &pop
     label1:
      &push r:2      
      &push 'Cafe2_BaseUIScreen'      
      &getVariable
      &push 'm_thisClass'      
      &getMember
      &push 'm_uiComponentArray'      
      &getMember
      &push 'length'      
      &getMember
      &lessThan
      &not
      &jnz label3      
      &push r:3      
      &push 'Cafe2_BaseUIScreen'      
      &getVariable
      &push 'm_thisClass'      
      &getMember
      &push 'm_uiComponentArray'      
      &getMember
      &push r:2      
      &getMember
      &equals
      &not
      &jnz label2      
      &jmp label3      
     label2:
      &push r:2      
      &increment
      &setRegister r:2
      &pop
      &jmp label1      
     label3:
      &push 1      
      &push r:2      
      &push 2      
      &push 'Cafe2_BaseUIScreen'      
      &getVariable
      &push 'm_thisClass'      
      &getMember
      &push 'm_uiComponentArray'      
      &getMember
      &push 'splice'      
      &callMethod
      &pop
      &push 'Cafe2_BaseUIScreen'      
      &getVariable
      &push 'm_thisClass'      
      &getMember
      &push 'm_uiComponentArray'      
      &getMember
      &push 'length'      
      &getMember
      &push 0.0      
      &greaterThan
      &not
      &not
      &jnz label4      
      &push 'Cafe2_BaseUIScreen'      
      &getVariable
      &push 'm_thisClass'      
      &getMember
      &push 'm_changeToScreen'      
      &getMember
      &pushnull      
      &equals
      &not
      &not
      &jnz label4      
      &push 'Cafe2_BaseUIScreen'      
      &getVariable
      &push 'm_thisClass'      
      &getMember
      &push 'm_changeToScreen'      
      &getMember
      &push 1      
      &push r:1      
      &push 'gSM'      
      &getMember
      &push 'changeMainScreen'      
      &callMethod
      &pop
     label4:
    &end // of function 

    &setMember
    &push r:1    
    &push 'introComplete'    
    &function2  (r:1='theComponent') ()
      &push 'Cafe2_BaseUIScreen::introComplete()'      
      &trace
      &push 0.0      
      &push r:1      
      &push 'restoreVisualState'      
      &callMethod
      &pop
      &push 'Cafe2_BaseUIScreen'      
      &getVariable
      &push 'm_thisClass'      
      &getMember
      &push 'm_introCount'      
      &push 'Cafe2_BaseUIScreen'      
      &getVariable
      &push 'm_thisClass'      
      &getMember
      &push 'm_introCount'      
      &getMember
      &decrement
      &setMember
      &push 'Cafe2_BaseUIScreen'      
      &getVariable
      &push 'm_thisClass'      
      &getMember
      &push 'm_introCount'      
      &getMember
      &push 0.0      
      &greaterThan
      &not
      &not
      &jnz label5      
      &push 0.0      
      &push 'Cafe2_BaseUIScreen'      
      &getVariable
      &push 'm_thisClass'      
      &getMember
      &push 'onIntrosComplete'      
      &callMethod
      &pop
     label5:
    &end // of function 

    &setMember
    &push r:1    
    &push 'onIntrosComplete'    
    &function2  () (r:1='this', r:2='_global')
      &push 'Cafe2_BaseUIScreen::onIntrosComplete()'      
      &trace
      &push r:1      
      &push 'localEventHandler'      
      &getMember
      &push 1      
      &push r:2      
      &push 'gEH'      
      &getMember
      &push 'setLocalEventHandler'      
      &callMethod
      &pop
      &push 0.0      
      &push r:2      
      &push 'gEH'      
      &getMember
      &push 'enableCurrentLocalEventHandler'      
      &callMethod
      &pop
    &end // of function 

    &setMember
    &push r:1    
    &push 'registerIntroOuttroComponents'    
    &function2  (r:3='components') (r:1='this')
      &push 'Cafe2_BaseUIScreen::registerIntroOuttroComponents()'      
      &trace
      &push r:1      
      &push 'm_uiComponentArray'      
      &push r:3      
      &setMember
      &push r:1      
      &push 'm_introCount'      
      &push r:1      
      &push 'm_uiComponentArray'      
      &getMember
      &push 'length'      
      &getMember
      &setMember
      &push 0.0      
      &setRegister r:2
      &pop
     label6:
      &push r:2      
      &push r:1      
      &push 'm_introCount'      
      &getMember
      &lessThan
      &not
      &jnz label7      
      &push r:1      
      &push 'introComplete'      
      &getMember
      &push 1      
      &push r:1      
      &push 'm_uiComponentArray'      
      &getMember
      &push r:2      
      &getMember
      &push 'setIntroCallback'      
      &callMethod
      &pop
      &push r:2      
      &increment
      &setRegister r:2
      &pop
      &jmp label6      
     label7:
    &end // of function 

    &setMember
    &push r:1    
    &push 'executeBackFunction'    
    &function2  (r:3='theComponent') (r:1='_global')
      &push 'Cafe2_BaseUIScreen::executeBackFunction'      
      &push r:3      
      &add
      &trace
      &push 0.0      
      &setRegister r:2
      &pop
     label8:
      &push r:2      
      &push 'Cafe2_BaseUIScreen'      
      &getVariable
      &push 'm_thisClass'      
      &getMember
      &push 'm_uiComponentArray'      
      &getMember
      &push 'length'      
      &getMember
      &lessThan
      &not
      &jnz label10      
      &push r:3      
      &push 'Cafe2_BaseUIScreen'      
      &getVariable
      &push 'm_thisClass'      
      &getMember
      &push 'm_uiComponentArray'      
      &getMember
      &push r:2      
      &getMember
      &equals
      &not
      &jnz label9      
      &jmp label10      
     label9:
      &push r:2      
      &increment
      &setRegister r:2
      &pop
      &jmp label8      
     label10:
      &push 1      
      &push r:2      
      &push 2      
      &push 'Cafe2_BaseUIScreen'      
      &getVariable
      &push 'm_thisClass'      
      &getMember
      &push 'm_uiComponentArray'      
      &getMember
      &push 'splice'      
      &callMethod
      &pop
      &push 'Cafe2_BaseUIScreen'      
      &getVariable
      &push 'm_thisClass'      
      &getMember
      &push 'm_uiComponentArray'      
      &getMember
      &push 'length'      
      &getMember
      &push 0.0      
      &greaterThan
      &not
      &not
      &jnz label11      
      &push 0.0      
      &push r:1      
      &push 'gSM'      
      &getMember
      &push 'goBackScreen'      
      &callMethod
      &pop
     label11:
    &end // of function 

    &setMember
    &push r:1    
    &push 'startIntro'    
    &function2  () (r:1='this')
      &push 'Cafe2_BaseUIScreen::startIntro()'      
      &trace
      &push r:1      
      &push 'm_uiComponentArray'      
      &getMember
      &push 'length'      
      &getMember
      &setRegister r:3
      &pop
      &push 0.0      
      &setRegister r:2
      &pop
     label12:
      &push r:2      
      &push r:3      
      &lessThan
      &not
      &jnz label13      
      &push 0.0      
      &push r:1      
      &push 'm_uiComponentArray'      
      &getMember
      &push r:2      
      &getMember
      &push 'intro'      
      &callMethod
      &pop
      &push r:2      
      &increment
      &setRegister r:2
      &pop
      &jmp label12      
     label13:
    &end // of function 

    &setMember
    &push r:1    
    &push 'startOuttro'    
    &function2  (r:4='newScreen') (r:1='this')
      &push 'Cafe2_BaseUIScreen::startOuttro'      
      &trace
      &push r:1      
      &push 'm_changeToScreen'      
      &push r:4      
      &setMember
      &push 'Cafe2_BaseUIScreen'      
      &getVariable
      &push 'm_thisClass'      
      &getMember
      &push 'm_uiComponentArray'      
      &getMember
      &push 'length'      
      &getMember
      &setRegister r:3
      &pop
      &push r:3      
      &push 0.0      
      &greaterThan
      &not
      &jnz label16      
      &push 0.0      
      &setRegister r:2
      &pop
     label14:
      &push r:2      
      &push r:3      
      &lessThan
      &not
      &jnz label15      
      &push r:1      
      &push 'outtroComplete'      
      &getMember
      &push 1      
      &push 'Cafe2_BaseUIScreen'      
      &getVariable
      &push 'm_thisClass'      
      &getMember
      &push 'm_uiComponentArray'      
      &getMember
      &push r:2      
      &getMember
      &push 'outtro'      
      &callMethod
      &pop
      &push r:2      
      &increment
      &setRegister r:2
      &pop
      &jmp label14      
     label15:
      &jmp label17      
     label16:
      &pushnull      
      &push 1      
      &push r:1      
      &push 'outtroComplete'      
      &callMethod
      &pop
     label17:
    &end // of function 

    &setMember
    &push r:1    
    &push 'startOuttroAndCallBack'    
    &function2  (r:4='doAfterOuttroComplete') (r:1='this')
      &push 'Cafe2_BaseUIScreen::startOuttro'      
      &trace
      &push r:1      
      &push 'm_executeOuttroCallBack'      
      &push r:4      
      &setMember
      &push 'Cafe2_BaseUIScreen'      
      &getVariable
      &push 'm_thisClass'      
      &getMember
      &push 'm_uiComponentArray'      
      &getMember
      &push 'length'      
      &getMember
      &setRegister r:3
      &pop
      &push r:3      
      &push 0.0      
      &greaterThan
      &not
      &jnz label20      
      &push 0.0      
      &setRegister r:2
      &pop
     label18:
      &push r:2      
      &push r:3      
      &lessThan
      &not
      &jnz label19      
      &push r:1      
      &push 'outtroAndCallBackComplete'      
      &getMember
      &push 1      
      &push 'Cafe2_BaseUIScreen'      
      &getVariable
      &push 'm_thisClass'      
      &getMember
      &push 'm_uiComponentArray'      
      &getMember
      &push r:2      
      &getMember
      &push 'outtro'      
      &callMethod
      &pop
      &push r:2      
      &increment
      &setRegister r:2
      &pop
      &jmp label18      
     label19:
      &jmp label21      
     label20:
      &pushnull      
      &push 1      
      &push r:1      
      &push 'outtroAndCallBackComplete'      
      &callMethod
      &pop
     label21:
    &end // of function 

    &setMember
    &push r:1    
    &push 'outtroAndCallBackComplete'    
    &function2  (r:2='theComponent') ()
      &push 'Cafe2_BaseUIScreen::outtroComplete() '      
      &push r:2      
      &add
      &trace
      &push 0.0      
      &setRegister r:1
      &pop
     label22:
      &push r:1      
      &push 'Cafe2_BaseUIScreen'      
      &getVariable
      &push 'm_thisClass'      
      &getMember
      &push 'm_uiComponentArray'      
      &getMember
      &push 'length'      
      &getMember
      &lessThan
      &not
      &jnz label24      
      &push r:2      
      &push 'Cafe2_BaseUIScreen'      
      &getVariable
      &push 'm_thisClass'      
      &getMember
      &push 'm_uiComponentArray'      
      &getMember
      &push r:1      
      &getMember
      &equals
      &not
      &jnz label23      
      &jmp label24      
     label23:
      &push r:1      
      &increment
      &setRegister r:1
      &pop
      &jmp label22      
     label24:
      &push 1      
      &push r:1      
      &push 2      
      &push 'Cafe2_BaseUIScreen'      
      &getVariable
      &push 'm_thisClass'      
      &getMember
      &push 'm_uiComponentArray'      
      &getMember
      &push 'splice'      
      &callMethod
      &pop
      &push 'Cafe2_BaseUIScreen'      
      &getVariable
      &push 'm_thisClass'      
      &getMember
      &push 'm_uiComponentArray'      
      &getMember
      &push 'length'      
      &getMember
      &push 0.0      
      &greaterThan
      &not
      &not
      &jnz label25      
      &push 0.0      
      &push 'Cafe2_BaseUIScreen'      
      &getVariable
      &push 'm_thisClass'      
      &getMember
      &push 'm_executeOuttroCallBack'      
      &callMethod
      &pop
     label25:
    &end // of function 

    &setMember
    &push r:1    
    &push 'backButtonClicked'    
    &function2  () (r:1='_global')
      &push 'Cafe2_BaseUIScreen::backButtonClicked()'      
      &trace
      &push 'Cafe2_BaseUIScreen'      
      &getVariable
      &push 'm_thisClass'      
      &getMember
      &push 'm_uiComponentArray'      
      &getMember
      &push 'length'      
      &getMember
      &setRegister r:3
      &pop
      &push r:3      
      &push 0.0      
      &greaterThan
      &not
      &jnz label28      
      &push 0.0      
      &setRegister r:2
      &pop
     label26:
      &push r:2      
      &push r:3      
      &lessThan
      &not
      &jnz label27      
      &push 'Cafe2_BaseUIScreen'      
      &getVariable
      &push 'm_thisClass'      
      &getMember
      &push 'executeBackFunction'      
      &getMember
      &push 1      
      &push 'Cafe2_BaseUIScreen'      
      &getVariable
      &push 'm_thisClass'      
      &getMember
      &push 'm_uiComponentArray'      
      &getMember
      &push r:2      
      &getMember
      &push 'outtro'      
      &callMethod
      &pop
      &push r:2      
      &increment
      &setRegister r:2
      &pop
      &jmp label26      
     label27:
      &jmp label29      
     label28:
      &push 0.0      
      &push r:1      
      &push 'gSM'      
      &getMember
      &push 'goBackScreen'      
      &callMethod
      &pop
     label29:
    &end // of function 

    &setMember
    &push r:1    
    &push 'localEventHandler'    
    &function2  (r:2='keyCode', 'controller') (r:1='_global')
      &push 'Cafe2_BaseUIScreen::localEventHandler() keyCode == '      
      &push r:2      
      &add
      &trace
      &push r:2      
      &push 1      
      &push r:1      
      &push 'gFM'      
      &getMember
      &push 'handleDefaultInputs'      
      &callMethod
      &pop
    &end // of function 

    &setMember
    &push r:1    
    &push 'clearData'    
    &function2  () (r:1='this')
      &push 'Cafe2_BaseUIScreen'      
      &getVariable
      &push 'm_screen'      
      &pushnull      
      &setMember
      &push 'Cafe2_BaseUIScreen'      
      &getVariable
      &push 'm_thisClass'      
      &pushnull      
      &setMember
      &push r:1      
      &push 'm_changeToScreen'      
      &pushnull      
      &setMember
      &push r:1      
      &push 'm_uiComponentArray'      
      &pushnull      
      &setMember
    &end // of function 

    &setMember
    &push r:1    
    &push 'bind0'    
    &function  ('obj', 'func'    )    
      &function  (      )      
        &push 'obj'        
        &getVariable
        &push 1        
        &push 'func'        
        &getVariable
        &push 'call'        
        &callMethod
        &pop
      &end // of function 

      &return
    &end // of function 

    &setMember
    &push r:1    
    &push 'm_changeToScreen'    
    &pushnull    
    &setMember
    &push r:1    
    &push 'm_executeOuttroCallBack'    
    &pushnull    
    &setMember
    &push r:1    
    &push 'm_uiComponentArray'    
    &push 0.0    
    &push 'Array'    
    &new
    &setMember
    &push 1    
    &pushnull    
    &push '_global'    
    &getVariable
    &push 'Cafe2_BaseUIScreen'    
    &getMember
    &push 'prototype'    
    &getMember
    &push 3    
    &push 'ASSetPropFlags'    
    &callFunction
   label30:
    &pop
  &end // of initMovieClip 14

  &defineMovieClip 24 // total frames: 1
  &end // of defineMovieClip 24

  &defineMovieClip 25 // total frames: 1
  &end // of defineMovieClip 25

  &defineMovieClip 15 // total frames: 0
  &end // of defineMovieClip 15
  
  &exportAssets
    15 &as '__Packages.fes_m_chapterSelect'
  &end // of exportAssets
  
  &initMovieClip 15
    &constants '_global', 'fes_m_chapterSelect', 'fes_m_chapterSelect::fes_m_chapterSelect()', 'Cafe2_BaseUIScreen', 'm_thisClass', 'm_chapterSelectPanel', 'chapterSelectPanel', 'prototype', 'frameDelayedInit', 'm_localMessageHandlerFunc', 'localMessageHandler', 'bind0', 'gMH', 'addMessageHandler', 'm_eventHandlerIndex', 'localEventHandler', 'gEH', 'addLocalEventHandler', 'enableLocalEventHandler', 'initGUI', 'startIntro', 'onScreenExit', 'gSM', 'setOnExitScreen', 'Object', 'Array', '$HARDDIFFICULTY', '$MEDIUMDIFFICULTY', '$EASYDIFFICULTY', 'm_screen', 'ddDifficulty', 'setData', 'OnDifficultyClicked', 'setOnChange', 'setSelectedIndex', 'show', 'enable', 'updateDifficultySetting', 'button_Back', 'OnBackButtonClick', 'setOnMouseUpFunction', 'push', 'button_quickLaunch', 'OnQuickLaunchButtonClick', 'button_Play', 'OnPlayButtonClick', 'm_chapterSelectCallback', 'onChapterSelectChanged', 'bind1DynamicParams', 'addObserver', 'onEnterFrame', 'safeInitddDifficulty', 'bin0', 'registerIntroOuttroComponents', 'm_Difficulty', 'getCurrentIndex', 'FSCommand:', 'CallGameFunction', '%Campaign_SetDifficulty?Difficulty=', '%SetCampaignDifficulty?Difficulty=', 'refreshDifficulty', '_loc1_', 'QueryGameEngine?CAMPAIGN_DIFFICULTY', 'CAMPAIGN_DIFFICULTY', '%SetMap?Map=', 'getSelectedMapId', '%Campaign_SetMission?MissionIndex=', 'getSelectedMission', 'INPUTCODE', 'ESCAPE', 'gFM', 'handleDefaultInputs', 'removeObserver', 'removeMessageHandler', 'removeLocalEventHandler', 'goBackScreen', '%LoadAutoSaveSilent?FactionIndex=', 'getSelectedCampaign', 'GAME', 'COOPMODE', 'ENUM', 'GAME_COOPMODE_CHOOSER', 'PLAY_MISSION_INTRO_MOVIE', 'SCREEN', 'FES_COOP_MISSION_BRIEFING', 'changeMainScreen', 'ASSetPropFlags'  
    &push '_global'    
    &getVariable
    &push 'fes_m_chapterSelect'    
    &getMember
    &not
    &not
    &jnz label3    
    &push '_global'    
    &getVariable
    &push 'fes_m_chapterSelect'    
    &function2  (r:3='screen') (r:1='this', r:2='super')
      &push r:3      
      &push 1      
      &push r:2      
      &push UNDEF      
      &callMethod
      &pop
      &push 'fes_m_chapterSelect::fes_m_chapterSelect()'      
      &trace
      &push 'Cafe2_BaseUIScreen'      
      &getVariable
      &push 'm_thisClass'      
      &push r:1      
      &setMember
      &push r:1      
      &push 'm_chapterSelectPanel'      
      &push r:3      
      &push 'chapterSelectPanel'      
      &getMember
      &setMember
    &end // of function 

    &setRegister r:1
    &setMember
    &push '_global'    
    &getVariable
    &push 'fes_m_chapterSelect'    
    &getMember
    &push 'Cafe2_BaseUIScreen'    
    &getVariable
    &extends
    &push r:1    
    &push 'prototype'    
    &getMember
    &setRegister r:2
    &pop
    &push r:2    
    &push 'frameDelayedInit'    
    &function2  () (r:1='this', r:2='_global')
      &push r:1      
      &push 'm_localMessageHandlerFunc'      
      &push r:1      
      &push 'localMessageHandler'      
      &getMember
      &push r:1      
      &push 2      
      &push r:2      
      &push 'bind0'      
      &callMethod
      &setMember
      &push r:1      
      &push 'm_localMessageHandlerFunc'      
      &getMember
      &push 1      
      &push r:2      
      &push 'gMH'      
      &getMember
      &push 'addMessageHandler'      
      &callMethod
      &pop
      &push r:1      
      &push 'm_eventHandlerIndex'      
      &push r:1      
      &push 'localEventHandler'      
      &getMember
      &push 1      
      &push r:2      
      &push 'gEH'      
      &getMember
      &push 'addLocalEventHandler'      
      &callMethod
      &setMember
      &push 0      
      &push r:2      
      &push 'gEH'      
      &getMember
      &push 'enableLocalEventHandler'      
      &callMethod
      &pop
      &push 0      
      &push r:1      
      &push 'initGUI'      
      &callMethod
      &pop
      &push 0      
      &push r:1      
      &push 'startIntro'      
      &callMethod
      &pop
    &end // of function 

    &setMember
    &push r:2    
    &push 'initGUI'    
    &function2  () (r:1='this', r:2='_global')
      &push r:1      
      &push 'onScreenExit'      
      &getMember
      &push r:1      
      &push 2      
      &push r:2      
      &push 'bind0'      
      &callMethod
      &push 1      
      &push r:2      
      &push 'gSM'      
      &getMember
      &push 'setOnExitScreen'      
      &callMethod
      &pop
      &push 0      
      &push 'Object'      
      &new
      &setRegister r:3
      &pop
      &push 0      
      &push 'Array'      
      &new
      &setRegister r:4
      &pop
      &push '$HARDDIFFICULTY'      
      &push '$MEDIUMDIFFICULTY'      
      &push '$EASYDIFFICULTY'      
      &push 3      
      &initArray
      &setRegister r:5
      &pop
      &push 'Cafe2_BaseUIScreen'      
      &getVariable
      &push 'm_screen'      
      &getMember
      &push 'ddDifficulty'      
      &getMember
      &setRegister r:6
      &pop
      &push r:5      
      &push r:5      
      &push 2      
      &push r:6      
      &push 'setData'      
      &callMethod
      &pop
      &push r:1      
      &push 'OnDifficultyClicked'      
      &getMember
      &push r:1      
      &push 2      
      &push r:2      
      &push 'bind0'      
      &callMethod
      &push 1      
      &push r:6      
      &push 'setOnChange'      
      &callMethod
      &pop
      &push 1      
      &push 1      
      &push r:6      
      &push 'setSelectedIndex'      
      &callMethod
      &pop
      &push 0      
      &push r:6      
      &push 'show'      
      &callMethod
      &pop
      &push 0      
      &push r:6      
      &push 'enable'      
      &callMethod
      &pop
      &push 0      
      &push r:1      
      &push 'updateDifficultySetting'      
      &callMethod
      &pop
      &push 'Cafe2_BaseUIScreen'      
      &getVariable
      &push 'm_screen'      
      &getMember
      &push 'button_Back'      
      &getMember
      &setRegister r:7
      &pop
      &push 0      
      &push r:7      
      &push 'enable'      
      &callMethod
      &pop
      &push r:1      
      &push 'OnBackButtonClick'      
      &getMember
      &push r:1      
      &push 2      
      &push r:2      
      &push 'bind0'      
      &callMethod
      &push 1      
      &push r:7      
      &push 'setOnMouseUpFunction'      
      &callMethod
      &pop
      &push r:7      
      &push 1      
      &push r:4      
      &push 'push'      
      &callMethod
      &pop
      &push 'Cafe2_BaseUIScreen'      
      &getVariable
      &push 'm_screen'      
      &getMember
      &push 'button_quickLaunch'      
      &getMember
      &setRegister r:8
      &pop
      &push 0      
      &push r:8      
      &push 'enable'      
      &callMethod
      &pop
      &push r:1      
      &push 'OnQuickLaunchButtonClick'      
      &getMember
      &push r:1      
      &push 2      
      &push r:2      
      &push 'bind0'      
      &callMethod
      &push 1      
      &push r:8      
      &push 'setOnMouseUpFunction'      
      &callMethod
      &pop
      &push r:8      
      &push 1      
      &push r:4      
      &push 'push'      
      &callMethod
      &pop
      &push 'Cafe2_BaseUIScreen'      
      &getVariable
      &push 'm_screen'      
      &getMember
      &push 'button_Play'      
      &getMember
      &setRegister r:9
      &pop
      &push 0      
      &push r:9      
      &push 'enable'      
      &callMethod
      &pop
      &push r:1      
      &push 'OnPlayButtonClick'      
      &getMember
      &push r:1      
      &push 2      
      &push r:2      
      &push 'bind0'      
      &callMethod
      &push 1      
      &push r:9      
      &push 'setOnMouseUpFunction'      
      &callMethod
      &pop
      &push r:9      
      &push 1      
      &push r:4      
      &push 'push'      
      &callMethod
      &pop
      &push r:1      
      &push 'm_chapterSelectCallback'      
      &push r:1      
      &push 'onChapterSelectChanged'      
      &getMember
      &push r:1      
      &push 2      
      &push r:2      
      &push 'bind1DynamicParams'      
      &callMethod
      &setMember
      &push r:1      
      &push 'm_chapterSelectCallback'      
      &getMember
      &push 1      
      &push r:1      
      &push 'm_chapterSelectPanel'      
      &getMember
      &push 'addObserver'      
      &callMethod
      &pop
      &push 'Cafe2_BaseUIScreen'      
      &getVariable
      &push 'm_screen'      
      &getMember
      &push 'onEnterFrame'      
      &push r:1      
      &push 'safeInitddDifficulty'      
      &getMember
      &push r:1      
      &push 2      
      &push r:2      
      &push 'bin0'      
      &callMethod
      &setMember
      &push r:4      
      &push 1      
      &push r:1      
      &push 'registerIntroOuttroComponents'      
      &callMethod
      &pop
    &end // of function 

    &setMember
    &push r:2    
    &push 'OnDifficultyClicked'    
    &function2  () (r:1='this')
      &push 0      
      &push r:1      
      &push 'updateDifficultySetting'      
      &callMethod
      &pop
    &end // of function 

    &setMember
    &push r:2    
    &push 'updateDifficultySetting'    
    &function2  () (r:1='this')
      &push r:1      
      &push 'm_Difficulty'      
      &push 0      
      &push 'Cafe2_BaseUIScreen'      
      &getVariable
      &push 'm_screen'      
      &getMember
      &push 'ddDifficulty'      
      &getMember
      &push 'getCurrentIndex'      
      &callMethod
      &setMember
      &push 'FSCommand:'      
      &push 'CallGameFunction'      
      &concat
      &push '%Campaign_SetDifficulty?Difficulty='      
      &push r:1      
      &push 'm_Difficulty'      
      &getMember
      &add
      &getURL2 GET
      &push 'FSCommand:'      
      &push 'CallGameFunction'      
      &concat
      &push '%SetCampaignDifficulty?Difficulty='      
      &push r:1      
      &push 'm_Difficulty'      
      &getMember
      &add
      &getURL2 GET
    &end // of function 

    &setMember
    &push r:2    
    &push 'refreshDifficulty'    
    &function  (    )    
      &push '_loc1_'      
      &push 0      
      &push 'Object'      
      &new
      &varEquals
      &push 'QueryGameEngine?CAMPAIGN_DIFFICULTY'      
      &push '_loc1_'      
      &getVariable
      &loadVariables
      &push '_loc1_'      
      &getVariable
      &push 'CAMPAIGN_DIFFICULTY'      
      &getMember
      &push 1      
      &push 'Cafe2_BaseUIScreen'      
      &getVariable
      &push 'm_screen'      
      &getMember
      &push 'ddDifficulty'      
      &getMember
      &push 'setSelectedIndex'      
      &callMethod
      &pop
    &end // of function 

    &setMember
    &push r:2    
    &push 'onChapterSelectChanged'    
    &function2  () (r:1='this')
      &push 'FSCommand:'      
      &push 'CallGameFunction'      
      &concat
      &push '%SetMap?Map='      
      &push 0      
      &push 'Cafe2_BaseUIScreen'      
      &getVariable
      &push 'm_screen'      
      &getMember
      &push 'chapterSelectPanel'      
      &getMember
      &push 'getSelectedMapId'      
      &callMethod
      &toString
      &add
      &getURL2 GET
      &push 'FSCommand:'      
      &push 'CallGameFunction'      
      &concat
      &push '%Campaign_SetMission?MissionIndex='      
      &push 0      
      &push r:1      
      &push 'm_chapterSelectPanel'      
      &getMember
      &push 'getSelectedMission'      
      &callMethod
      &toString
      &add
      &getURL2 GET
    &end // of function 

    &setMember
    &push r:2    
    &push 'localMessageHandler'    
    &function2  (r:1='messageCode') ()
      &push &TRUE      
      &setRegister r:2
      &pop
      &push &FALSE      
      &setRegister r:2
      &pop
      &push r:2      
      &return
    &end // of function 

    &setMember
    &push r:2    
    &push 'localEventHandler'    
    &function2  (r:3='keyCode', r:4='controller') (r:1='this', r:2='_global')
      &push r:3      
      &push 'INPUTCODE'      
      &getVariable
      &push 'ESCAPE'      
      &getMember
      &strictEquals
      &not
      &not
      &jnz label1      
      &push r:3      
      &push 1      
      &push r:2      
      &push 'gFM'      
      &getMember
      &push 'handleDefaultInputs'      
      &callMethod
      &pop
      &jmp label2      
     label1:
      &push 0      
      &push r:1      
      &push 'OnBackButtonClick'      
      &callMethod
      &pop
     label2:
    &end // of function 

    &setMember
    &push r:2    
    &push 'onScreenExit'    
    &function2  () (r:1='this', r:2='_global')
      &push r:1      
      &push 'm_chapterSelectCallback'      
      &getMember
      &push 1      
      &push r:1      
      &push 'm_chapterSelectPanel'      
      &getMember
      &push 'removeObserver'      
      &callMethod
      &pop
      &push r:1      
      &push 'm_chapterSelectCallback'      
      &delete
      &pop
      &push r:1      
      &push 'm_localMessageHandlerFunc'      
      &getMember
      &push 1      
      &push r:2      
      &push 'gMH'      
      &getMember
      &push 'removeMessageHandler'      
      &callMethod
      &pop
      &push r:1      
      &push 'm_localMessageHandlerFunc'      
      &delete
      &pop
      &push r:1      
      &push 'm_eventHandlerIndex'      
      &getMember
      &push 1      
      &push r:2      
      &push 'gEH'      
      &getMember
      &push 'removeLocalEventHandler'      
      &callMethod
      &pop
      &push 'Cafe2_BaseUIScreen'      
      &getVariable
      &push 'm_screen'      
      &pushnull      
      &setMember
      &push 'Cafe2_BaseUIScreen'      
      &getVariable
      &push 'm_thisClass'      
      &pushnull      
      &setMember
      &push r:2      
      &push 'fes_m_chapterSelect'      
      &delete
      &pop
      &push r:2      
      &push 'Cafe2_BaseUIScreen'      
      &delete
      &pop
    &end // of function 

    &setMember
    &push r:2    
    &push 'OnBackButtonClick'    
    &function2  () (r:1='_global')
      &push 0      
      &push r:1      
      &push 'gSM'      
      &getMember
      &push 'goBackScreen'      
      &callMethod
      &pop
    &end // of function 

    &setMember
    &push r:2    
    &push 'OnPlayButtonClick'    
    &function2  () (r:1='this', r:2='_global')
      &push 0      
      &push r:1      
      &push 'm_chapterSelectPanel'      
      &getMember
      &push 'getSelectedMission'      
      &callMethod
      &setRegister r:3
      &pop
      &push 'FSCommand:'      
      &push 'CallGameFunction'      
      &concat
      &push '%LoadAutoSaveSilent?FactionIndex='      
      &push 0      
      &push r:1      
      &push 'm_chapterSelectPanel'      
      &getMember
      &push 'getSelectedCampaign'      
      &callMethod
      &add
      &getURL2 GET
      &push 'FSCommand:'      
      &push 'CallGameFunction'      
      &concat
      &push '%Campaign_SetMission?MissionIndex='      
      &push r:3      
      &add
      &getURL2 GET
      &push r:2      
      &push 'GAME'      
      &getMember
      &push 'COOPMODE'      
      &push r:2      
      &push 'ENUM'      
      &getMember
      &push 'GAME_COOPMODE_CHOOSER'      
      &getMember
      &setMember
      &push r:2      
      &push 'GAME'      
      &getMember
      &push 'PLAY_MISSION_INTRO_MOVIE'      
      &push &TRUE      
      &setMember
      &push r:2      
      &push 'SCREEN'      
      &getMember
      &push 'FES_COOP_MISSION_BRIEFING'      
      &getMember
      &push 1      
      &push r:2      
      &push 'gSM'      
      &getMember
      &push 'changeMainScreen'      
      &callMethod
      &pop
    &end // of function 

    &setMember
    &push r:2    
    &push 'OnQuickLaunchButtonClick'    
    &function2  () (r:1='this')
      &push 0      
      &push r:1      
      &push 'm_chapterSelectPanel'      
      &getMember
      &push 'getSelectedMission'      
      &callMethod
      &setRegister r:2
      &pop
      &push 'FSCommand:'      
      &push 'CallGameFunction'      
      &concat
      &push '%Campaign_SetMission?MissionIndex='      
      &push r:2      
      &add
      &getURL2 GET
      &getURL 'FSCommand:CallGameFunction'       '%Mission_SetAI?AIIndex=0'      
      &push 'FSCommand:'      
      &push 'CallGameFunction'      
      &concat
      &push '%Campaign_SetDifficulty?Difficulty='      
      &push r:1      
      &push 'm_Difficulty'      
      &getMember
      &add
      &getURL2 GET
      &getURL 'FSCommand:CallGameFunction'       '%Campaign_StartMission'      
    &end // of function 

    &setMember
    &push r:2    
    &push 'm_Difficulty'    
    &push 0    
    &setMember
    &push 1    
    &pushnull    
    &push '_global'    
    &getVariable
    &push 'fes_m_chapterSelect'    
    &getMember
    &push 'prototype'    
    &getMember
    &push 3    
    &push 'ASSetPropFlags'    
    &callFunction
   label3:
    &pop
  &end // of initMovieClip 15

  &defineMovieClip 26 // total frames: 1
  &end // of defineMovieClip 26
&end
