movie 'E:\depot\CnC\RA3_EP1\PRODUC~1\Data\APTBUI~1\034A3~1.0-D\pc\Output\FEAF17~1\\FEG_M_~1.eaf' &compressed // flash 7, total frames: 1, frame rate: 30 fps, 1365x768 px
  
  &importAssets &from '../cafe/mouseComponents/std_MouseCheckBox.swf'
    'std_MouseCheckBoxSymbol' &as 3
  &end // of importAssets

  &defineMovieClip 4 // total frames: 1

    &placeMovieClip 3 &as 'addCommentaryCheckbox'
    
      &onClipEvent &construct
        &constants 'm_width', 'm_textAlign', 'm_labelPosition', 'right align', 'm_label_x', 'm_label_y', 'm_focusDirs', 'Up/Down', 'm_iconType', 'default', 'm_extLabel', '$ADDCOMMENTARYTOREPLAY&DropShadow', 'm_nTruncateType', 'm_label', 'm_bEnabled', 'm_refFM', '_root.gFM', 'm_initiallySelected', 'm_tabIndex', 'm_bVisible', 'm_contentSymbol', 'checkBoxContentClipSymbol'  
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
        &pushsdb 11							//'$ADDCOMMENTARYTOREPLAY&DropShadow'
        &setVariable
        &pushsdb 12							//'m_nTruncateType'
        &pushzerosv
        &pushsdb 13							//'m_label'
        &pushsdb 11							//'$ADDCOMMENTARYTOREPLAY&DropShadow'
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
        &pushbyte -1
        &setVariable
        &pushsdb 19							//'m_bVisible'
        &pushtrue
        &setVariable
        &pushsdb 20							//'m_contentSymbol'
        &pushsdb 21							//'checkBoxContentClipSymbol'
        &setVariable
      &end
    &end // of placeMovieClip 3
  &end // of defineMovieClip 4
  
  &exportAssets
    4 &as 'AddCommentaryPanel'
  &end // of exportAssets

  &frame 0
    &pushs 'screen'
    &pushthisgv
    &pushone
    &pushs 'feg_m_saveLoadMenu'
    &new
    &varEquals
    &stop
  &end // of frame 0
  
  &importAssets &from './screens/fe_m_assets.swf'
    'Background_mc' &as 5
  &end // of importAssets
  
  &importAssets &from '.\\Components\\fe_m_generalScreenComponents.swf'
    'lowerScreenBar Symbol' &as 6
  &end // of importAssets
  
  &importAssets &from '.\\Components\\fe_m_generalScreenComponents.swf'
    'ScreenTitleBarSymbol' &as 7
  &end // of importAssets

  &placeMovieClip 7 &as 'titleBarMC'
  
    &onClipEvent &construct
      &pushs 'm_title'
      &pushssv ''
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
  &end // of placeMovieClip 7
  
  &importAssets &from '../cafe/mouseComponents/std_mouseScrollingListbox.swf'
    'std_mouseScrollingListboxSymbol' &as 10
  &end // of importAssets

  &placeMovieClip 10 &as 'saveFileList'
  
    &onClipEvent &construct
      &constants 'm_focusDirs', 'Up/Down', 'm_numEntries', 'm_vPadding', 'm_bEnabled', 'm_refFM', '_root.gFM', 'm_initiallySelected', 'm_tabIndex', 'm_bVisible', 'm_scrollbarOffset', 'm_BgVisible', 'm_renderWidth', 'm_renderHeight', 'm_backgroundMargin', 'm_useHighlights', 'm_alternateBackgroundColors', 'm_backgroundColor1', 'm_backgroundColor2'  
      &pushsdb 0							//'m_focusDirs'
      &pushsdb 1							//'Up/Down'
      &setVariable
      &pushsdb 2							//'m_numEntries'
      &pushbyte 22
      &setVariable
      &pushsdb 3							//'m_vPadding'
      &pushzerosv
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
      &pushzerosv
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
      &pushfalse
      &setVariable
      &pushsdb 12							//'m_renderWidth'
      &pushzerosv
      &pushsdb 13							//'m_renderHeight'
      &pushzerosv
      &pushsdb 14							//'m_backgroundMargin'
      &pushbyte 2
      &setVariable
      &pushsdb 15							//'m_useHighlights'
      &pushtrue
      &setVariable
      &pushsdb 16							//'m_alternateBackgroundColors'
      &pushtrue
      &setVariable
      &pushsdb 17							//'m_backgroundColor1'
      &pushlong 2622983
      &setVariable
      &pushsdb 18							//'m_backgroundColor2'
      &pushzerosv
    &end
  &end // of placeMovieClip 10
  
  &importAssets &from '../cafe/mouseComponents/std_mouseScrollbar.swf'
    'std_mouseScrollbarSymbol' &as 11
  &end // of importAssets

  &placeMovieClip 11 &as 'saveFileListScrollbar'
  
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
  &end // of placeMovieClip 11
  
  &importAssets &from '../cafe/mouseComponents/std_MouseButton.swf'
    'std_MousebuttonSymbol' &as 12
  &end // of importAssets

  &placeMovieClip 12 &as 'button_Cancel'
  
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
      &pushone
      &setVariable
      &pushs 'm_bVisible'
      &pushtrue
      &setVariable
      &pushs 'm_label'
      &pushssv '$CANCEL'
      &pushs 'm_contentSymbol'
      &pushssv 'buttonContentSymbol'
    &end
  &end // of placeMovieClip 12

  &placeMovieClip 12 &as 'button_Delete'
  
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
      &pushbyte 2
      &setVariable
      &pushs 'm_bVisible'
      &pushtrue
      &setVariable
      &pushs 'm_label'
      &pushssv '$DELETE'
      &pushs 'm_contentSymbol'
      &pushssv 'buttonContentSymbol'
    &end
  &end // of placeMovieClip 12

  &placeMovieClip 12 &as 'button_SaveLoad'
  
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
      &pushbyte 3
      &setVariable
      &pushs 'm_bVisible'
      &pushtrue
      &setVariable
      &pushs 'm_label'
      &pushssv ''
      &pushs 'm_contentSymbol'
      &pushssv 'buttonContentSymbol'
    &end
  &end // of placeMovieClip 12

  &defineMovieClip 16 // total frames: 1
  &end // of defineMovieClip 16
  
  &importAssets &from '../../cafe/mouseComponents/std_mouseTextInputField.swf'
    'std_mouseTextInputFieldSymbol' &as 22
  &end // of importAssets

  &placeMovieClip 22 &as 'txt_saveFileName'
  
    &onClipEvent &construct
      &pushs 'm_width'
      &pushshort 450
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
      &pushssv '$SAVEGAMENAME'
      &pushs 'm_nTruncateType'
      &pushzerosv
      &pushs 'm_label'
      &pushssv ''
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
      &pushs 'm_password'
      &pushfalse
      &setVariable
      &pushs 'm_charLimit'
      &pushbyte 26
      &setVariable
      &pushs 'm_allowHorzScroll'
      &pushfalse
      &setVariable
    &end
  &end // of placeMovieClip 22

  &defineMovieClip 23 // total frames: 0
  &end // of defineMovieClip 23
  
  &exportAssets
    23 &as '__Packages.Cafe2_BaseUIScreen'
  &end // of exportAssets
  
  &initMovieClip 23
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
  &end // of initMovieClip 23

  &defineMovieClip 24 // total frames: 0
  &end // of defineMovieClip 24
  
  &exportAssets
    24 &as '__Packages.IListboxEventListener'
  &end // of exportAssets
  
  &initMovieClip 24
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
  &end // of initMovieClip 24

  &defineMovieClip 25 // total frames: 0
  &end // of defineMovieClip 25
  
  &exportAssets
    25 &as '__Packages.feg_m_saveLoadMenu'
  &end // of exportAssets
  
  &initMovieClip 25
    &constants '_global', 'feg_m_saveLoadMenu', 'Cafe2_BaseUIScreen', 'm_screen', 'm_thisClass', 'GAME', 'MODE', 'ENUM', 'GAME_MODE_CAMPAIGN', 'GAME_MODE_TUTORIAL', 'GAME_MODE_SKIRMISH', 'GAME_MODE_MULTIPLAYER', 'GAME_MODE_SHELL', 'GAME_MODE_NONE', 'm_saveMode', 'SAVEMODE_CAMPAIGN', 'SAVEMODE_SKIRMISH', 'SAVEMODE_REPLAY', 'feg_m_saveLoadMenu::feg_m_saveLoadMenu() Bad _global.GAME.MODE encountered.', 'gSM', 'getLastScreenId', 'SCREEN', 'FEM_MP_POSTGAME_ROUNDUP', 'm_pendingLoad', 'initGUI', 'prototype', 'IListboxEventListener', 'OnListboxIndexMouseClick', 'm_fileIds', 'Object', 'QueryGameEngine?LASTMOUSEDOUBLECLICK', 'LASTMOUSEDOUBLECLICK', 'SAVELOADSCREEN_TYPE', 'SAVELOADSCREEN_SAVE', 'OnSaveButtonClick', 'OnLoadButtonClick', 'NEW_SAVE_INDEX', 'button_Delete', 'disable', 'enable', 'm_selectedSaveIndex', 'SetSaveNameTextfieldContents', 'OnListboxIndexElementMouseClick', 'feg_m_saveLoadMenu::initGUI()', 'onScreenExit', 'setOnExitScreen', 'localMessageHandler', 'gMH', 'addMessageHandler', 'm_eventHandlerIndex', 'localEventHandler', 'gEH', 'addLocalEventHandler', 'enableLocalEventHandler', '$APT:SAVE', 'button_SaveLoad', 'setText', '$APT:HEADERSAVEREPLAY', 'titleBarMC', 'setTitle', '$APT:HEADERSAVEGAME', 'bind0', 'SetOnMouseUpFunction', 'm_saveFilter', 'SAVEFILTER_USER', 'txt_saveFileName', 'show', 'SaveBottomImg', '_visible', 'saveFileList', 'setNumEntries', '$APT:LOAD', '$APT:HEADERLOADREPLAY', '$APT:HEADERLOADGAME', 'SAVEFILTER_ANY', 'hide', 'OnCancelButtonClick', 'button_Cancel', 'setOnMouseUpFunction', 'OnDeleteButtonClick', 'saveFileListScrollbar', 'enableGUI', 'setVerticalPadding', 'addListener', 'Array', 'symbol', 'std_mouseListboxEntryTextElementSymbol', 'width', 'leftMargin', 'rightMargin', 'useHitRegion', 'fontSize', 'push', 'init', 'attachScrollbarController', 'updateListboxData', 'registerIntroOuttroComponents', 'startIntro', 'feg_m_saveLoadMenu::initGui() done.', 'MSGCODE', 'FE_SAVEGAMES_UPDATE', 'FE_CLOSE_SAVELOAD', 'FE_LOAD_FAILED', 'FE_SAVE_FAILED', 'backButtonClicked', 'button_Saveload', 'fes_saveLoadMenu::localEventHandler() keyCode == ', 'INPUTCODE', 'ESCAPE', 'gFM', 'handleDefaultInputs', 'shutdown', 'clearData', 'removeListener', 'removeMessageHandler', 'removeLocalEventHandler', 'feg_m_saveLoadMenu::onScreenExit() done.', 'QueryGameEngine?SAVE_FILES?GameMode=', '|Filter=', 'SAVE_FILES_IDS', 'length', 'fes_m_saveLoadMenu::getData() Loadvar SAVE_FILES successful.', ',', 'split', 'm_fileNames', 'SAVE_FILES_NAMES', 'm_fileMaps', 'SAVE_FILES_MAPS', 'm_fileTimes', 'SAVE_FILES_TIMES', 'm_fileDates', 'SAVE_FILES_DATES', '-1', 'unshift', '$GUI:NEWSAVEGAME', '', 'setColumnData', 'setSelectedIndex', 'moveViewToFirst', 'refreshDisplay', 'refreshScrollbar', 'fes_m_saveLoadMenu::getData() SAVE_FILES = ', 'FSCommand:CallGameFunction', '%SaveFile?FileId=', '|GameMode=', '|Description=', 'getContents', '%LoadFile?FileId=', '%DeleteFile?FileId=', 'QueryGameEngine?SAVE_FILE_BY_INDEX?GameMode=', '|FileId=', 'SAVE_FILE_BY_INDEX', 'setContents', 'SAVEFILTER_AUTO', 'ASSetPropFlags'  
    &pushglobalgv
    &pushsdbgm 1							//'feg_m_saveLoadMenu'
    &not
    &not
    &jnz label40    
    &pushglobalgv
    &pushsdb 1							//'feg_m_saveLoadMenu'
    &function2  (r:4='screen') (r:1='this', r:2='super', r:3='_global')
      &pushzero
      &pushregister 2
      &pushundef
      &callmp
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdb 3							//'m_screen'
      &pushregister 4
      &setMember
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdb 4							//'m_thisClass'
      &pushregister 1
      &setMember
      &pushregister 3
      &pushsdbgm 5							//'GAME'
      &pushsdbgm 6							//'MODE'
      &setRegister r:0
      &pushregister 3
      &pushsdbgm 7							//'ENUM'
      &pushsdbgm 8							//'GAME_MODE_CAMPAIGN'
      &strictEquals
      &jnz label1      
      &pushregister 0
      &pushregister 3
      &pushsdbgm 7							//'ENUM'
      &pushsdbgm 9							//'GAME_MODE_TUTORIAL'
      &strictEquals
      &jnz label1      
      &pushregister 0
      &pushregister 3
      &pushsdbgm 7							//'ENUM'
      &pushsdbgm 10							//'GAME_MODE_SKIRMISH'
      &strictEquals
      &jnz label2      
      &pushregister 0
      &pushregister 3
      &pushsdbgm 7							//'ENUM'
      &pushsdbgm 11							//'GAME_MODE_MULTIPLAYER'
      &strictEquals
      &jnz label3      
      &pushregister 0
      &pushregister 3
      &pushsdbgm 7							//'ENUM'
      &pushsdbgm 12							//'GAME_MODE_SHELL'
      &strictEquals
      &jnz label3      
      &pushregister 0
      &pushregister 3
      &pushsdbgm 7							//'ENUM'
      &pushsdbgm 13							//'GAME_MODE_NONE'
      &strictEquals
      &jnz label3      
      &jmp label4      
     label1:
      &pushsdbgv 1							//'feg_m_saveLoadMenu'
      &pushsdb 14							//'m_saveMode'
      &pushsdbgv 1							//'feg_m_saveLoadMenu'
      &pushsdbgm 15							//'SAVEMODE_CAMPAIGN'
      &setMember
      &jmp label5      
     label2:
      &pushsdbgv 1							//'feg_m_saveLoadMenu'
      &pushsdb 14							//'m_saveMode'
      &pushsdbgv 1							//'feg_m_saveLoadMenu'
      &pushsdbgm 16							//'SAVEMODE_SKIRMISH'
      &setMember
      &jmp label5      
     label3:
      &pushsdbgv 1							//'feg_m_saveLoadMenu'
      &pushsdb 14							//'m_saveMode'
      &pushsdbgv 1							//'feg_m_saveLoadMenu'
      &pushsdbgm 17							//'SAVEMODE_REPLAY'
      &setMember
      &jmp label5      
     label4:
      &pushsdb 18							//'feg_m_saveLoadMenu::feg_m_saveLoadMenu() Bad _global.GAME.MODE encountered.'
      &trace
      &jmp label5      
     label5:
      &pushzero
      &pushregister 3
      &pushsdbgm 19							//'gSM'
      &pushsdb 20							//'getLastScreenId'
      &callMethod
      &pushregister 3
      &pushsdbgm 21							//'SCREEN'
      &pushsdbgm 22							//'FEM_MP_POSTGAME_ROUNDUP'
      &equals
      &not
      &jnz label6      
      &pushsdbgv 1							//'feg_m_saveLoadMenu'
      &pushsdb 14							//'m_saveMode'
      &pushsdbgv 1							//'feg_m_saveLoadMenu'
      &pushsdbgm 17							//'SAVEMODE_REPLAY'
      &setMember
     label6:
      &pushregister 1
      &pushsdb 23							//'m_pendingLoad'
      &pushfalse
      &setMember
      &pushzero
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 4							//'m_thisClass'
      &dcallmp 24							// initGUI()
    &end // of function 

    &setRegister r:1
    &setMember
    &pushglobalgv
    &pushsdbgm 1							//'feg_m_saveLoadMenu'
    &pushsdbgv 2							//'Cafe2_BaseUIScreen'
    &extends
    &pushregister 1
    &pushsdbgm 25							//'prototype'
    &setRegister r:2
    &pop
    &pushglobalgv
    &pushsdbgm 26							//'IListboxEventListener'
    &pushone
    &pushglobalgv
    &pushsdbgm 1							//'feg_m_saveLoadMenu'
    &implements
    &pushregister 2
    &pushsdb 27							//'OnListboxIndexMouseClick'
    &function2  (r:4='index', 'previouslySelectedIndex') (r:1='this', r:2='_global')
      &pushregister 1
      &pushsdbgm 28							//'m_fileIds'
      &pushregister 4
      &getMember
      &pushundef
      &equals
      &not
      &not
      &jnz label14      
      &pushzero
      &pushsdb 29							//'Object'
      &new
      &setRegister r:3
      &pop
      &pushsdb 30							//'QueryGameEngine?LASTMOUSEDOUBLECLICK'
      &pushregister 3
      &loadVariables
      &pushregister 3
      &pushsdbgm 31							//'LASTMOUSEDOUBLECLICK'
      &pushzero
      &equals
      &not
      &not
      &jnz label9      
      &pushregister 2
      &pushsdbgm 5							//'GAME'
      &pushsdbgm 32							//'SAVELOADSCREEN_TYPE'
      &pushregister 2
      &pushsdbgm 7							//'ENUM'
      &pushsdbgm 33							//'SAVELOADSCREEN_SAVE'
      &equals
      &not
      &jnz label7      
      &pushzero
      &pushregister 1
      &dcallmp 34							// OnSaveButtonClick()
      &jmp label8      
     label7:
      &pushzero
      &pushregister 1
      &dcallmp 35							// OnLoadButtonClick()
     label8:
      &jmp label12      
     label9:
      &pushregister 1
      &pushsdbgm 28							//'m_fileIds'
      &pushregister 4
      &getMember
      &pushsdbgv 1							//'feg_m_saveLoadMenu'
      &pushsdbgm 36							//'NEW_SAVE_INDEX'
      &equals
      &not
      &jnz label10      
      &pushzero
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 3							//'m_screen'
      &pushsdbgm 37							//'button_Delete'
      &pushsdb 38							//'disable'
      &callMethod
      &jmp label11      
     label10:
      &pushzero
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 3							//'m_screen'
      &pushsdbgm 37							//'button_Delete'
      &pushsdb 39							//'enable'
      &callMethod
     label11:
      &pop
     label12:
      &pushregister 1
      &pushsdb 40							//'m_selectedSaveIndex'
      &pushregister 1
      &pushsdbgm 28							//'m_fileIds'
      &pushregister 4
      &getMember
      &setMember
      &pushregister 2
      &pushsdbgm 5							//'GAME'
      &pushsdbgm 32							//'SAVELOADSCREEN_TYPE'
      &pushregister 2
      &pushsdbgm 7							//'ENUM'
      &pushsdbgm 33							//'SAVELOADSCREEN_SAVE'
      &equals
      &not
      &jnz label13      
      &pushregister 1
      &pushsdbgm 40							//'m_selectedSaveIndex'
      &pushone
      &pushregister 1
      &dcallmp 41							// SetSaveNameTextfieldContents()
     label13:
      &jmp label15      
     label14:
      &pushzero
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 3							//'m_screen'
      &pushsdbgm 37							//'button_Delete'
      &dcallmp 38							// disable()
     label15:
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 42							//'OnListboxIndexElementMouseClick'
    &function  ('index', 'element', 'previouslySelectedIndex'    )    
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 24							//'initGUI'
    &function2  () (r:1='this', r:2='_global')
      &pushsdb 43							//'feg_m_saveLoadMenu::initGUI()'
      &trace
      &pushregister 1
      &pushsdbgm 44							//'onScreenExit'
      &pushone
      &pushregister 2
      &pushsdbgm 19							//'gSM'
      &dcallmp 45							// setOnExitScreen()
      &pushregister 1
      &pushsdbgm 46							//'localMessageHandler'
      &pushone
      &pushregister 2
      &pushsdbgm 47							//'gMH'
      &dcallmp 48							// addMessageHandler()
      &pushregister 1
      &pushsdb 49							//'m_eventHandlerIndex'
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 4							//'m_thisClass'
      &pushsdbgm 50							//'localEventHandler'
      &pushone
      &pushregister 2
      &pushsdbgm 51							//'gEH'
      &pushsdb 52							//'addLocalEventHandler'
      &callMethod
      &setMember
      &pushzero
      &pushregister 2
      &pushsdbgm 51							//'gEH'
      &dcallmp 53							// enableLocalEventHandler()
      &pushregister 2
      &pushsdbgm 5							//'GAME'
      &pushsdbgm 32							//'SAVELOADSCREEN_TYPE'
      &pushregister 2
      &pushsdbgm 7							//'ENUM'
      &pushsdbgm 33							//'SAVELOADSCREEN_SAVE'
      &equals
      &not
      &jnz label18      
      &pushsdb 54							//'$APT:SAVE'
      &pushone
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 3							//'m_screen'
      &pushsdbgm 55							//'button_SaveLoad'
      &dcallmp 56							// setText()
      &pushsdbgv 1							//'feg_m_saveLoadMenu'
      &pushsdbgm 14							//'m_saveMode'
      &pushsdbgv 1							//'feg_m_saveLoadMenu'
      &pushsdbgm 17							//'SAVEMODE_REPLAY'
      &equals
      &not
      &jnz label16      
      &pushtrue
      &pushtrue
      &pushsdb 57							//'$APT:HEADERSAVEREPLAY'
      &pushbyte 3
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 3							//'m_screen'
      &pushsdbgm 58							//'titleBarMC'
      &dcallmp 59							// setTitle()
      &jmp label17      
     label16:
      &pushtrue
      &pushtrue
      &pushsdb 60							//'$APT:HEADERSAVEGAME'
      &pushbyte 3
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 3							//'m_screen'
      &pushsdbgm 58							//'titleBarMC'
      &dcallmp 59							// setTitle()
     label17:
      &pushregister 1
      &pushsdbgm 34							//'OnSaveButtonClick'
      &pushregister 1
      &pushbyte 2
      &pushregister 2
      &pushsdb 61							//'bind0'
      &callMethod
      &pushone
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 3							//'m_screen'
      &pushsdbgm 55							//'button_SaveLoad'
      &dcallmp 62							// SetOnMouseUpFunction()
      &pushsdbgv 1							//'feg_m_saveLoadMenu'
      &pushsdb 63							//'m_saveFilter'
      &pushsdbgv 1							//'feg_m_saveLoadMenu'
      &pushsdbgm 64							//'SAVEFILTER_USER'
      &setMember
      &pushzero
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 3							//'m_screen'
      &pushsdbgm 65							//'txt_saveFileName'
      &dcallmp 66							// show()
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 3							//'m_screen'
      &pushsdbgm 67							//'SaveBottomImg'
      &pushsdb 68							//'_visible'
      &pushtrue
      &setMember
      &pushbyte 17
      &pushone
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 3							//'m_screen'
      &pushsdbgm 69							//'saveFileList'
      &dcallmp 70							// setNumEntries()
      &pushregister 1
      &pushsdbgm 40							//'m_selectedSaveIndex'
      &pushone
      &pushregister 1
      &dcallmp 41							// SetSaveNameTextfieldContents()
      &jmp label21      
     label18:
      &pushtrue
      &pushtrue
      &pushsdb 71							//'$APT:LOAD'
      &pushbyte 3
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 3							//'m_screen'
      &pushsdbgm 55							//'button_SaveLoad'
      &dcallmp 56							// setText()
      &pushsdbgv 1							//'feg_m_saveLoadMenu'
      &pushsdbgm 14							//'m_saveMode'
      &pushsdbgv 1							//'feg_m_saveLoadMenu'
      &pushsdbgm 17							//'SAVEMODE_REPLAY'
      &equals
      &not
      &jnz label19      
      &pushtrue
      &pushtrue
      &pushsdb 72							//'$APT:HEADERLOADREPLAY'
      &pushbyte 3
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 3							//'m_screen'
      &pushsdbgm 58							//'titleBarMC'
      &dcallmp 59							// setTitle()
      &jmp label20      
     label19:
      &pushtrue
      &pushtrue
      &pushsdb 73							//'$APT:HEADERLOADGAME'
      &pushbyte 3
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 3							//'m_screen'
      &pushsdbgm 58							//'titleBarMC'
      &dcallmp 59							// setTitle()
     label20:
      &pushbyte 22
      &pushone
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 3							//'m_screen'
      &pushsdbgm 69							//'saveFileList'
      &dcallmp 70							// setNumEntries()
      &pushregister 1
      &pushsdbgm 35							//'OnLoadButtonClick'
      &pushregister 1
      &pushbyte 2
      &pushregister 2
      &pushsdb 61							//'bind0'
      &callMethod
      &pushone
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 3							//'m_screen'
      &pushsdbgm 55							//'button_SaveLoad'
      &dcallmp 62							// SetOnMouseUpFunction()
      &pushsdbgv 1							//'feg_m_saveLoadMenu'
      &pushsdb 63							//'m_saveFilter'
      &pushsdbgv 1							//'feg_m_saveLoadMenu'
      &pushsdbgm 74							//'SAVEFILTER_ANY'
      &setMember
      &pushzero
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 3							//'m_screen'
      &pushsdbgm 65							//'txt_saveFileName'
      &dcallmp 75							// hide()
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 3							//'m_screen'
      &pushsdbgm 67							//'SaveBottomImg'
      &pushsdb 68							//'_visible'
      &pushfalse
      &setMember
     label21:
      &pushregister 1
      &pushsdbgm 76							//'OnCancelButtonClick'
      &pushregister 1
      &pushbyte 2
      &pushregister 2
      &pushsdb 61							//'bind0'
      &callMethod
      &pushone
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 3							//'m_screen'
      &pushsdbgm 77							//'button_Cancel'
      &dcallmp 78							// setOnMouseUpFunction()
      &pushregister 1
      &pushsdbgm 79							//'OnDeleteButtonClick'
      &pushregister 1
      &pushbyte 2
      &pushregister 2
      &pushsdb 61							//'bind0'
      &callMethod
      &pushone
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 3							//'m_screen'
      &pushsdbgm 37							//'button_Delete'
      &dcallmp 78							// setOnMouseUpFunction()
      &pushzero
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 3							//'m_screen'
      &pushsdbgm 80							//'saveFileListScrollbar'
      &dcallmp 81							// enableGUI()
      &pushzero
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 3							//'m_screen'
      &pushsdbgm 80							//'saveFileListScrollbar'
      &dcallmp 66							// show()
      &pushzero
      &pushone
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 3							//'m_screen'
      &pushsdbgm 69							//'saveFileList'
      &dcallmp 82							// setVerticalPadding()
      &pushregister 1
      &pushone
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 3							//'m_screen'
      &pushsdbgm 69							//'saveFileList'
      &dcallmp 83							// addListener()
      &pushzero
      &pushsdb 84							//'Array'
      &new
      &setRegister r:3
      &pop
      &pushsdb 85							//'symbol'
      &pushsdb 86							//'std_mouseListboxEntryTextElementSymbol'
      &pushsdb 87							//'width'
      &pushshort 300
      &pushsdb 88							//'leftMargin'
      &pushzero
      &pushsdb 89							//'rightMargin'
      &pushzero
      &pushsdb 90							//'useHitRegion'
      &pushfalse
      &pushsdb 91							//'fontSize'
      &pushbyte 14
      &pushbyte 6
      &initObject
      &pushone
      &pushregister 3
      &dcallmp 92							// push()
      &pushsdb 85							//'symbol'
      &pushsdb 86							//'std_mouseListboxEntryTextElementSymbol'
      &pushsdb 87							//'width'
      &pushshort 314
      &pushsdb 88							//'leftMargin'
      &pushzero
      &pushsdb 89							//'rightMargin'
      &pushzero
      &pushsdb 90							//'useHitRegion'
      &pushfalse
      &pushsdb 91							//'fontSize'
      &pushbyte 14
      &pushbyte 6
      &initObject
      &pushone
      &pushregister 3
      &dcallmp 92							// push()
      &pushsdb 85							//'symbol'
      &pushsdb 86							//'std_mouseListboxEntryTextElementSymbol'
      &pushsdb 87							//'width'
      &pushbyte 110
      &pushsdb 88							//'leftMargin'
      &pushzero
      &pushsdb 89							//'rightMargin'
      &pushzero
      &pushsdb 90							//'useHitRegion'
      &pushfalse
      &pushsdb 91							//'fontSize'
      &pushbyte 14
      &pushbyte 6
      &initObject
      &pushone
      &pushregister 3
      &dcallmp 92							// push()
      &pushsdb 85							//'symbol'
      &pushsdb 86							//'std_mouseListboxEntryTextElementSymbol'
      &pushsdb 87							//'width'
      &pushbyte 76
      &pushsdb 88							//'leftMargin'
      &pushzero
      &pushsdb 89							//'rightMargin'
      &pushbyte 26
      &pushsdb 90							//'useHitRegion'
      &pushfalse
      &pushsdb 91							//'fontSize'
      &pushbyte 14
      &pushbyte 6
      &initObject
      &pushone
      &pushregister 3
      &dcallmp 92							// push()
      &pushbyte 20
      &pushregister 3
      &pushbyte 2
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 3							//'m_screen'
      &pushsdbgm 69							//'saveFileList'
      &dcallmp 93							// init()
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 3							//'m_screen'
      &pushsdbgm 80							//'saveFileListScrollbar'
      &pushone
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 3							//'m_screen'
      &pushsdbgm 69							//'saveFileList'
      &dcallmp 94							// attachScrollbarController()
      &pushzero
      &pushregister 1
      &dcallmp 95							// updateListboxData()
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 3							//'m_screen'
      &pushsdbgm 55							//'button_SaveLoad'
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 3							//'m_screen'
      &pushsdbgm 37							//'button_Delete'
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 3							//'m_screen'
      &pushsdbgm 77							//'button_Cancel'
      &pushbyte 3
      &initArray
      &pushone
      &pushregister 1
      &dcallmp 96							// registerIntroOuttroComponents()
      &pushzero
      &pushregister 1
      &dcallmp 97							// startIntro()
      &pushsdb 98							//'feg_m_saveLoadMenu::initGui() done.'
      &trace
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 46							//'localMessageHandler'
    &function2  (r:3='messageCode') (r:1='_global')
      &pushtrue
      &setRegister r:2
      &pop
      &pushregister 3
      &setRegister r:0
      &pushregister 1
      &pushsdbgm 99							//'MSGCODE'
      &pushsdbgm 100							//'FE_SAVEGAMES_UPDATE'
      &strictEquals
      &jnz label22      
      &pushregister 0
      &pushregister 1
      &pushsdbgm 99							//'MSGCODE'
      &pushsdbgm 101							//'FE_CLOSE_SAVELOAD'
      &strictEquals
      &jnz label23      
      &pushregister 0
      &pushregister 1
      &pushsdbgm 99							//'MSGCODE'
      &pushsdbgm 102							//'FE_LOAD_FAILED'
      &strictEquals
      &jnz label24      
      &pushregister 0
      &pushregister 1
      &pushsdbgm 99							//'MSGCODE'
      &pushsdbgm 103							//'FE_SAVE_FAILED'
      &strictEquals
      &jnz label25      
      &jmp label26      
     label22:
      &pushzero
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 4							//'m_thisClass'
      &dcallmp 95							// updateListboxData()
      &jmp label27      
     label23:
      &pushzero
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 4							//'m_thisClass'
      &dcallmp 104							// backButtonClicked()
      &jmp label27      
     label24:
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 4							//'m_thisClass'
      &pushsdb 23							//'m_pendingLoad'
      &pushfalse
      &setMember
     label25:
      &pushzero
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 3							//'m_screen'
      &pushsdbgm 105							//'button_Saveload'
      &dcallmp 39							// enable()
      &pushzero
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 3							//'m_screen'
      &pushsdbgm 77							//'button_Cancel'
      &dcallmp 39							// enable()
      &pushzero
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 3							//'m_screen'
      &pushsdbgm 37							//'button_Delete'
      &dcallmp 39							// enable()
      &jmp label27      
     label26:
      &pushfalse
      &setRegister r:2
      &pop
      &jmp label27      
     label27:
      &pushregister 2
      &return
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 50							//'localEventHandler'
    &function2  (r:2='keyCode', 'controller') (r:1='_global')
      &pushsdb 106							//'fes_saveLoadMenu::localEventHandler() keyCode == '
      &pushregister 2
      &add
      &trace
      &pushregister 2
      &pushsdbgv 107							//'INPUTCODE'
      &pushsdbgm 108							//'ESCAPE'
      &strictEquals
      &jnz label28      
      &jmp label29      
     label28:
      &pushzero
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 4							//'m_thisClass'
      &dcallmp 76							// OnCancelButtonClick()
      &jmp label30      
     label29:
      &pushregister 2
      &pushone
      &pushregister 1
      &pushsdbgm 109							//'gFM'
      &dcallmp 110							// handleDefaultInputs()
      &jmp label30      
     label30:
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 44							//'onScreenExit'
    &function2  () (r:1='this', r:2='_global')
      &getURL 'FSCommand:CallGameFunction'       '%SaveLoadExit'      
      &pushzero
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 3							//'m_screen'
      &pushsdbgm 69							//'saveFileList'
      &dcallmp 111							// shutdown()
      &pushzero
      &pushregister 1
      &dcallmp 112							// clearData()
      &pushregister 1
      &pushone
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 3							//'m_screen'
      &pushsdbgm 69							//'saveFileList'
      &dcallmp 113							// removeListener()
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 4							//'m_thisClass'
      &pushsdbgm 46							//'localMessageHandler'
      &pushone
      &pushregister 2
      &pushsdbgm 47							//'gMH'
      &dcallmp 114							// removeMessageHandler()
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 4							//'m_thisClass'
      &pushsdbgm 49							//'m_eventHandlerIndex'
      &pushone
      &pushregister 2
      &pushsdbgm 51							//'gEH'
      &dcallmp 115							// removeLocalEventHandler()
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdb 3							//'m_screen'
      &pushnull
      &setMember
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdb 4							//'m_thisClass'
      &pushnull
      &setMember
      &pushregister 2
      &pushsdb 1							//'feg_m_saveLoadMenu'
      &delete
      &pop
      &pushregister 2
      &pushsdb 2							//'Cafe2_BaseUIScreen'
      &delete
      &pop
      &pushsdb 116							//'feg_m_saveLoadMenu::onScreenExit() done.'
      &trace
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 95							//'updateListboxData'
    &function2  () (r:1='this', r:2='_global')
      &pushzero
      &pushsdb 29							//'Object'
      &new
      &setRegister r:3
      &pop
      &pushsdb 117							//'QueryGameEngine?SAVE_FILES?GameMode='
      &pushsdbgv 1							//'feg_m_saveLoadMenu'
      &pushsdbgm 14							//'m_saveMode'
      &toString
      &add
      &pushsdb 118							//'|Filter='
      &add
      &pushsdbgv 1							//'feg_m_saveLoadMenu'
      &pushsdbgm 63							//'m_saveFilter'
      &toString
      &add
      &pushregister 3
      &loadVariables
      &pushregister 3
      &pushsdbgm 119							//'SAVE_FILES_IDS'
      &pushundef
      &equals
      &not
      &dup
      &not
      &jnz label31      
      &pop
      &pushregister 3
      &pushsdbgm 119							//'SAVE_FILES_IDS'
      &pushsdbgm 120							//'length'
      &pushzero
      &greaterThan
     label31:
      &not
      &jnz label32      
      &pushsdb 121							//'fes_m_saveLoadMenu::getData() Loadvar SAVE_FILES successful.'
      &trace
      &pushregister 1
      &pushsdb 28							//'m_fileIds'
      &pushsdb 122							//','
      &pushone
      &pushregister 3
      &pushsdbgm 119							//'SAVE_FILES_IDS'
      &pushsdb 123							//'split'
      &callMethod
      &setMember
      &pushregister 1
      &pushsdb 124							//'m_fileNames'
      &pushsdb 122							//','
      &pushone
      &pushregister 3
      &pushsdbgm 125							//'SAVE_FILES_NAMES'
      &pushsdb 123							//'split'
      &callMethod
      &setMember
      &pushregister 1
      &pushsdb 126							//'m_fileMaps'
      &pushsdb 122							//','
      &pushone
      &pushregister 3
      &pushsdbgm 127							//'SAVE_FILES_MAPS'
      &pushsdb 123							//'split'
      &callMethod
      &setMember
      &pushregister 1
      &pushsdb 128							//'m_fileTimes'
      &pushsdb 122							//','
      &pushone
      &pushregister 3
      &pushsdbgm 129							//'SAVE_FILES_TIMES'
      &pushsdb 123							//'split'
      &callMethod
      &setMember
      &pushregister 1
      &pushsdb 130							//'m_fileDates'
      &pushsdb 122							//','
      &pushone
      &pushregister 3
      &pushsdbgm 131							//'SAVE_FILES_DATES'
      &pushsdb 123							//'split'
      &callMethod
      &setMember
      &jmp label33      
     label32:
      &pushregister 1
      &pushsdb 28							//'m_fileIds'
      &delete
      &pop
      &pushregister 1
      &pushsdb 124							//'m_fileNames'
      &delete
      &pop
      &pushregister 1
      &pushsdb 126							//'m_fileMaps'
      &delete
      &pop
      &pushregister 1
      &pushsdb 128							//'m_fileTimes'
      &delete
      &pop
      &pushregister 1
      &pushsdb 130							//'m_fileDates'
      &delete
      &pop
      &pushregister 1
      &pushsdb 28							//'m_fileIds'
      &pushzero
      &pushsdb 84							//'Array'
      &new
      &setMember
      &pushregister 1
      &pushsdb 124							//'m_fileNames'
      &pushzero
      &pushsdb 84							//'Array'
      &new
      &setMember
      &pushregister 1
      &pushsdb 126							//'m_fileMaps'
      &pushzero
      &pushsdb 84							//'Array'
      &new
      &setMember
      &pushregister 1
      &pushsdb 128							//'m_fileTimes'
      &pushzero
      &pushsdb 84							//'Array'
      &new
      &setMember
      &pushregister 1
      &pushsdb 130							//'m_fileDates'
      &pushzero
      &pushsdb 84							//'Array'
      &new
      &setMember
     label33:
      &pushregister 2
      &pushsdbgm 5							//'GAME'
      &pushsdbgm 32							//'SAVELOADSCREEN_TYPE'
      &pushregister 2
      &pushsdbgm 7							//'ENUM'
      &pushsdbgm 33							//'SAVELOADSCREEN_SAVE'
      &equals
      &not
      &jnz label34      
      &pushsdb 132							//'-1'
      &pushone
      &pushregister 1
      &pushsdbgm 28							//'m_fileIds'
      &dcallmp 133							// unshift()
      &pushsdb 134							//'$GUI:NEWSAVEGAME'
      &pushone
      &pushregister 1
      &pushsdbgm 124							//'m_fileNames'
      &dcallmp 133							// unshift()
      &pushsdb 135							//''
      &pushone
      &pushregister 1
      &pushsdbgm 126							//'m_fileMaps'
      &dcallmp 133							// unshift()
      &pushsdb 135							//''
      &pushone
      &pushregister 1
      &pushsdbgm 128							//'m_fileTimes'
      &dcallmp 133							// unshift()
      &pushsdb 135							//''
      &pushone
      &pushregister 1
      &pushsdbgm 130							//'m_fileDates'
      &dcallmp 133							// unshift()
     label34:
      &pushregister 1
      &pushsdbgm 124							//'m_fileNames'
      &pushzero
      &pushbyte 2
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 3							//'m_screen'
      &pushsdbgm 69							//'saveFileList'
      &dcallmp 136							// setColumnData()
      &pushregister 1
      &pushsdbgm 126							//'m_fileMaps'
      &pushone
      &pushbyte 2
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 3							//'m_screen'
      &pushsdbgm 69							//'saveFileList'
      &dcallmp 136							// setColumnData()
      &pushregister 1
      &pushsdbgm 128							//'m_fileTimes'
      &pushbyte 2
      &pushbyte 2
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 3							//'m_screen'
      &pushsdbgm 69							//'saveFileList'
      &dcallmp 136							// setColumnData()
      &pushregister 1
      &pushsdbgm 130							//'m_fileDates'
      &pushbyte 3
      &pushbyte 2
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 3							//'m_screen'
      &pushsdbgm 69							//'saveFileList'
      &dcallmp 136							// setColumnData()
      &pushregister 2
      &pushsdbgm 5							//'GAME'
      &pushsdbgm 32							//'SAVELOADSCREEN_TYPE'
      &pushregister 2
      &pushsdbgm 7							//'ENUM'
      &pushsdbgm 33							//'SAVELOADSCREEN_SAVE'
      &equals
      &not
      &jnz label35      
      &pushregister 1
      &pushsdb 40							//'m_selectedSaveIndex'
      &pushbyte -1
      &setMember
      &pushregister 1
      &pushsdbgm 40							//'m_selectedSaveIndex'
      &pushone
      &pushregister 1
      &dcallmp 41							// SetSaveNameTextfieldContents()
      &pushzero
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 3							//'m_screen'
      &pushsdbgm 37							//'button_Delete'
      &dcallmp 38							// disable()
      &jmp label38      
     label35:
      &pushregister 3
      &pushsdbgm 119							//'SAVE_FILES_IDS'
      &pushundef
      &equals
      &not
      &dup
      &not
      &jnz label36      
      &pop
      &pushregister 3
      &pushsdbgm 119							//'SAVE_FILES_IDS'
      &pushsdbgm 120							//'length'
      &pushzero
      &greaterThan
     label36:
      &not
      &jnz label37      
      &pushregister 1
      &pushsdb 40							//'m_selectedSaveIndex'
      &pushregister 1
      &pushsdbgm 28							//'m_fileIds'
      &pushzero
      &getMember
      &setMember
      &pushzero
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 3							//'m_screen'
      &pushsdbgm 37							//'button_Delete'
      &dcallmp 39							// enable()
      &jmp label38      
     label37:
      &pushzero
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 3							//'m_screen'
      &pushsdbgm 55							//'button_SaveLoad'
      &dcallmp 38							// disable()
      &pushzero
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 3							//'m_screen'
      &pushsdbgm 37							//'button_Delete'
      &dcallmp 38							// disable()
      &pushregister 1
      &pushsdb 40							//'m_selectedSaveIndex'
      &pushbyte -1
      &setMember
     label38:
      &pushzero
      &pushone
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 3							//'m_screen'
      &pushsdbgm 69							//'saveFileList'
      &dcallmp 137							// setSelectedIndex()
      &pushzero
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 3							//'m_screen'
      &pushsdbgm 69							//'saveFileList'
      &dcallmp 138							// moveViewToFirst()
      &pushzero
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 3							//'m_screen'
      &pushsdbgm 69							//'saveFileList'
      &dcallmp 139							// refreshDisplay()
      &pushzero
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 3							//'m_screen'
      &pushsdbgm 69							//'saveFileList'
      &dcallmp 140							// refreshScrollbar()
      &pushsdb 141							//'fes_m_saveLoadMenu::getData() SAVE_FILES = '
      &pushregister 1
      &pushsdbgm 28							//'m_fileIds'
      &toString
      &add
      &trace
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 76							//'OnCancelButtonClick'
    &function  (    )    
      &getURL 'FSCommand:CallGameFunction'       '%SaveLoadClosing'      
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 34							//'OnSaveButtonClick'
    &function2  () (r:1='this')
      &pushzero
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 3							//'m_screen'
      &pushsdbgm 55							//'button_SaveLoad'
      &dcallmp 38							// disable()
      &pushzero
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 3							//'m_screen'
      &pushsdbgm 77							//'button_Cancel'
      &dcallmp 38							// disable()
      &pushzero
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 3							//'m_screen'
      &pushsdbgm 37							//'button_Delete'
      &dcallmp 38							// disable()
      &pushsdb 142							//'FSCommand:CallGameFunction'
      &pushsdb 143							//'%SaveFile?FileId='
      &pushregister 1
      &pushsdbgm 40							//'m_selectedSaveIndex'
      &toString
      &add
      &pushsdb 144							//'|GameMode='
      &add
      &pushsdbgv 1							//'feg_m_saveLoadMenu'
      &pushsdbgm 14							//'m_saveMode'
      &toString
      &add
      &pushsdb 145							//'|Description='
      &add
      &pushzero
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 3							//'m_screen'
      &pushsdbgm 65							//'txt_saveFileName'
      &pushsdb 146							//'getContents'
      &callMethod
      &toString
      &add
      &getURL2
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 35							//'OnLoadButtonClick'
    &function2  () (r:1='this')
      &pushregister 1
      &pushsdbgm 23							//'m_pendingLoad'
      &not
      &not
      &jnz label39      
      &pushzero
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 3							//'m_screen'
      &pushsdbgm 55							//'button_SaveLoad'
      &dcallmp 38							// disable()
      &pushzero
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 3							//'m_screen'
      &pushsdbgm 77							//'button_Cancel'
      &dcallmp 38							// disable()
      &pushzero
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 3							//'m_screen'
      &pushsdbgm 37							//'button_Delete'
      &dcallmp 38							// disable()
      &pushregister 1
      &pushsdb 23							//'m_pendingLoad'
      &pushtrue
      &setMember
      &pushsdb 142							//'FSCommand:CallGameFunction'
      &pushsdb 147							//'%LoadFile?FileId='
      &pushregister 1
      &pushsdbgm 40							//'m_selectedSaveIndex'
      &toString
      &add
      &pushsdb 144							//'|GameMode='
      &add
      &pushsdbgv 1							//'feg_m_saveLoadMenu'
      &pushsdbgm 14							//'m_saveMode'
      &toString
      &add
      &getURL2
     label39:
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 79							//'OnDeleteButtonClick'
    &function2  () (r:1='this')
      &pushsdb 142							//'FSCommand:CallGameFunction'
      &pushsdb 148							//'%DeleteFile?FileId='
      &pushregister 1
      &pushsdbgm 40							//'m_selectedSaveIndex'
      &toString
      &add
      &pushsdb 144							//'|GameMode='
      &add
      &pushsdbgv 1							//'feg_m_saveLoadMenu'
      &pushsdbgm 14							//'m_saveMode'
      &toString
      &add
      &getURL2
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 41							//'SetSaveNameTextfieldContents'
    &function2  (r:2='fileId') ()
      &pushzero
      &pushsdb 29							//'Object'
      &new
      &setRegister r:1
      &pop
      &pushsdb 149							//'QueryGameEngine?SAVE_FILE_BY_INDEX?GameMode='
      &pushsdbgv 1							//'feg_m_saveLoadMenu'
      &pushsdbgm 14							//'m_saveMode'
      &toString
      &add
      &pushsdb 118							//'|Filter='
      &add
      &pushsdbgv 1							//'feg_m_saveLoadMenu'
      &pushsdbgm 63							//'m_saveFilter'
      &toString
      &add
      &pushsdb 150							//'|FileId='
      &add
      &pushregister 2
      &add
      &pushregister 1
      &loadVariables
      &pushregister 1
      &pushsdbgm 151							//'SAVE_FILE_BY_INDEX'
      &pushone
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 3							//'m_screen'
      &pushsdbgm 65							//'txt_saveFileName'
      &dcallmp 152							// setContents()
    &end // of function 

    &setMember
    &pushregister 1
    &pushsdb 17							//'SAVEMODE_REPLAY'
    &pushzero
    &setMember
    &pushregister 1
    &pushsdb 16							//'SAVEMODE_SKIRMISH'
    &pushone
    &setMember
    &pushregister 1
    &pushsdb 15							//'SAVEMODE_CAMPAIGN'
    &pushbyte 2
    &setMember
    &pushregister 1
    &pushsdb 74							//'SAVEFILTER_ANY'
    &pushzero
    &setMember
    &pushregister 1
    &pushsdb 64							//'SAVEFILTER_USER'
    &pushone
    &setMember
    &pushregister 1
    &pushsdb 153							//'SAVEFILTER_AUTO'
    &pushbyte 2
    &setMember
    &pushregister 1
    &pushsdb 36							//'NEW_SAVE_INDEX'
    &pushbyte -1
    &setMember
    &pushregister 2
    &pushsdb 124							//'m_fileNames'
    &pushzero
    &pushsdb 84							//'Array'
    &new
    &setMember
    &pushregister 2
    &pushsdb 126							//'m_fileMaps'
    &pushzero
    &pushsdb 84							//'Array'
    &new
    &setMember
    &pushregister 2
    &pushsdb 128							//'m_fileTimes'
    &pushzero
    &pushsdb 84							//'Array'
    &new
    &setMember
    &pushregister 2
    &pushsdb 130							//'m_fileDates'
    &pushzero
    &pushsdb 84							//'Array'
    &new
    &setMember
    &pushregister 2
    &pushsdb 28							//'m_fileIds'
    &pushzero
    &pushsdb 84							//'Array'
    &new
    &setMember
    &pushregister 2
    &pushsdb 40							//'m_selectedSaveIndex'
    &pushsdbgv 1							//'feg_m_saveLoadMenu'
    &pushsdbgm 36							//'NEW_SAVE_INDEX'
    &setMember
    &pushone
    &pushnull
    &pushglobalgv
    &pushsdbgm 1							//'feg_m_saveLoadMenu'
    &pushsdbgm 25							//'prototype'
    &pushbyte 3
    &pushsdb 154							//'ASSetPropFlags'
    &callFunction
   label40:
    &pop
  &end // of initMovieClip 25
&end
