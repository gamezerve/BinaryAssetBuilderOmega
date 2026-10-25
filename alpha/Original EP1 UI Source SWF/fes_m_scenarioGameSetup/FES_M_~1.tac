movie 'E:\depot\CnC\RA3_EP1\PRODUC~1\Data\APTBUI~1\034A3~1.0-D\pc\Output\FE6C59~1\\FES_M_~1.eaf' &compressed // flash 7, total frames: 1, frame rate: 30 fps, 1365x768 px

  &defineMovieClip 9 // total frames: 1
  &end // of defineMovieClip 9

  &defineMovieClip 10 // total frames: 30
  &end // of defineMovieClip 10

  &defineMovieClip 14 // total frames: 10

    &frame 3
      &stop
    &end // of frame 3

    &frame 9
      &stop
    &end // of frame 9
  &end // of defineMovieClip 14

  &defineMovieClip 21 // total frames: 1
  &end // of defineMovieClip 21

  &defineMovieClip 22 // total frames: 37

    &frame 4
      &stop
    &end // of frame 4

    &frame 9
      &stop
    &end // of frame 9

    &frame 14
      &stop
    &end // of frame 14

    &frame 19
      &stop
    &end // of frame 19

    &frame 24
      &stop
    &end // of frame 24

    &frame 29
      &pushzero
      &pushthisgv
      &pushsgm '_parent'
      &pushs 'introComplete'
      &callmp
      &stop
    &end // of frame 29

    &frame 36
      &pushzero
      &pushthisgv
      &pushsgm '_parent'
      &pushs 'outtroComplete'
      &callmp
      &stop
    &end // of frame 36
  &end // of defineMovieClip 22
  
  &exportAssets
    22 &as 'Japan Faction Radion Button Symbol'
  &end // of exportAssets

  &defineMovieClip 25 // total frames: 10

    &frame 3
      &stop
    &end // of frame 3

    &frame 9
      &stop
    &end // of frame 9
  &end // of defineMovieClip 25

  &defineMovieClip 28 // total frames: 37

    &frame 4
      &stop
    &end // of frame 4

    &frame 9
      &stop
    &end // of frame 9

    &frame 14
      &stop
    &end // of frame 14

    &frame 19
      &stop
    &end // of frame 19

    &frame 24
      &stop
    &end // of frame 24

    &frame 29
      &pushzero
      &pushthisgv
      &pushsgm '_parent'
      &pushs 'introComplete'
      &callmp
      &stop
    &end // of frame 29

    &frame 36
      &pushzero
      &pushthisgv
      &pushsgm '_parent'
      &pushs 'outtroComplete'
      &callmp
      &stop
    &end // of frame 36
  &end // of defineMovieClip 28
  
  &exportAssets
    28 &as 'Soviet Faction Radion Button Symbol'
  &end // of exportAssets

  &defineMovieClip 31 // total frames: 10

    &frame 3
      &stop
    &end // of frame 3

    &frame 9
      &stop
    &end // of frame 9
  &end // of defineMovieClip 31

  &defineMovieClip 34 // total frames: 37

    &frame 4
      &stop
    &end // of frame 4

    &frame 9
      &stop
    &end // of frame 9

    &frame 14
      &stop
    &end // of frame 14

    &frame 19
      &stop
    &end // of frame 19

    &frame 24
      &stop
    &end // of frame 24

    &frame 29
      &pushzero
      &pushthisgv
      &pushsgm '_parent'
      &pushs 'introComplete'
      &callmp
      &stop
    &end // of frame 29

    &frame 36
      &pushzero
      &pushthisgv
      &pushsgm '_parent'
      &pushs 'outtroComplete'
      &callmp
      &stop
    &end // of frame 36
  &end // of defineMovieClip 34
  
  &exportAssets
    34 &as 'Allied Faction Radion Button Symbol'
  &end // of exportAssets

  &defineMovieClip 36 // total frames: 1
  &end // of defineMovieClip 36
  
  &exportAssets
    36 &as 'MapPreviewComponent'
  &end // of exportAssets

  &frame 0
    &pushs 'screen'
    &pushthisgv
    &pushone
    &pushs 'fes_m_scenarioGameSetup'
    &new
    &varEquals
    &stop
  &end // of frame 0

  &defineMovieClip 39 // total frames: 1
  &end // of defineMovieClip 39

  &defineMovieClip 42 // total frames: 1
  &end // of defineMovieClip 42
  
  &importAssets &from '../cafe/mouseComponents/std_mouseScrollingListbox.swf'
    'std_mouseScrollingListboxSymbol' &as 43
  &end // of importAssets

  &placeMovieClip 43 &as 'descriptionTextListBox'
  
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
  &end // of placeMovieClip 43
  
  &importAssets &from '../cafe/mouseComponents/std_mouseScrollbar.swf'
    'std_mouseScrollbarSymbol' &as 44
  &end // of importAssets

  &placeMovieClip 44 &as 'scrollBar'
  
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
  &end // of placeMovieClip 44
  
  &importAssets &from '../cafe/mouseComponents/std_mouseRadioButton.swf'
    'std_mouseRadioButtonSymbol' &as 45
  &end // of importAssets

  &placeMovieClip 45 &as 'sovietFaction'
  
    &onClipEvent &construct
      &constants 'm_width', 'm_textAlign', 'm_labelPosition', 'right align', 'm_label_x', 'm_label_y', 'm_focusDirs', 'Up/Down', 'm_iconType', 'default', 'm_extLabel', '', 'm_nTruncateType', 'm_label', 'm_bEnabled', 'm_refFM', '_root.gFM', 'm_initiallySelected', 'm_tabIndex', 'm_bVisible', 'm_contentSymbol', 'Soviet Faction Radion Button Symbol', 'm_controller', 'factionController'  
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
      &pushsdb 11							//''
      &setVariable
      &pushsdb 12							//'m_nTruncateType'
      &pushzerosv
      &pushsdb 13							//'m_label'
      &pushsdb 11							//''
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
      &pushbyte 3
      &setVariable
      &pushsdb 19							//'m_bVisible'
      &pushtrue
      &setVariable
      &pushsdb 20							//'m_contentSymbol'
      &pushsdb 21							//'Soviet Faction Radion Button Symbol'
      &setVariable
      &pushsdb 22							//'m_controller'
      &pushsdb 23							//'factionController'
      &setVariable
    &end
  &end // of placeMovieClip 45
  
  &importAssets &from '../cafe/mouseComponents/std_mouseRadioButton.swf'
    'std_mouseRadioButtonControllerSymbol' &as 46
  &end // of importAssets

  &placeMovieClip 46 &as 'factionController'
  
    &onClipEvent &construct
      &pushs 'm_name'
      &pushssv 'RadioButtonController'
    &end
  &end // of placeMovieClip 46

  &placeMovieClip 45 &as 'alliedFaction'
  
    &onClipEvent &construct
      &constants 'm_width', 'm_textAlign', 'm_labelPosition', 'right align', 'm_label_x', 'm_label_y', 'm_focusDirs', 'Up/Down', 'm_iconType', 'default', 'm_extLabel', '', 'm_nTruncateType', 'm_label', 'm_bEnabled', 'm_refFM', '_root.gFM', 'm_initiallySelected', 'm_tabIndex', 'm_bVisible', 'm_contentSymbol', 'Allied Faction Radion Button Symbol', 'm_controller', 'factionController'  
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
      &pushsdb 11							//''
      &setVariable
      &pushsdb 12							//'m_nTruncateType'
      &pushzerosv
      &pushsdb 13							//'m_label'
      &pushsdb 11							//''
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
      &pushbyte 4
      &setVariable
      &pushsdb 19							//'m_bVisible'
      &pushtrue
      &setVariable
      &pushsdb 20							//'m_contentSymbol'
      &pushsdb 21							//'Allied Faction Radion Button Symbol'
      &setVariable
      &pushsdb 22							//'m_controller'
      &pushsdb 23							//'factionController'
      &setVariable
    &end
  &end // of placeMovieClip 45

  &placeMovieClip 45 &as 'japanFaction'
  
    &onClipEvent &construct
      &constants 'm_width', 'm_textAlign', 'm_labelPosition', 'right align', 'm_label_x', 'm_label_y', 'm_focusDirs', 'Up/Down', 'm_iconType', 'default', 'm_extLabel', '', 'm_nTruncateType', 'm_label', 'm_bEnabled', 'm_refFM', '_root.gFM', 'm_initiallySelected', 'm_tabIndex', 'm_bVisible', 'm_contentSymbol', 'Japan Faction Radion Button Symbol', 'm_controller', 'factionController'  
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
      &pushsdb 11							//''
      &setVariable
      &pushsdb 12							//'m_nTruncateType'
      &pushzerosv
      &pushsdb 13							//'m_label'
      &pushsdb 11							//''
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
      &pushbyte 5
      &setVariable
      &pushsdb 19							//'m_bVisible'
      &pushtrue
      &setVariable
      &pushsdb 20							//'m_contentSymbol'
      &pushsdb 21							//'Japan Faction Radion Button Symbol'
      &setVariable
      &pushsdb 22							//'m_controller'
      &pushsdb 23							//'factionController'
      &setVariable
    &end
  &end // of placeMovieClip 45

  &defineMovieClip 49 // total frames: 1
  &end // of defineMovieClip 49

  &defineMovieClip 50 // total frames: 30
  &end // of defineMovieClip 50

  &defineMovieClip 53 // total frames: 5

    &frame 4
      &stop
    &end // of frame 4
  &end // of defineMovieClip 53

  &placeMovieClip 53 &as 'unitPreview'
  
    &onClipEvent &construct
      &pushs '_type'
      &pushssv 'RenderImage'
      &pushs '_imageMap'
      &pushssv 'ScenarioUnitPreview'
      &pushs '_mode'
      &pushssv 'ALPHA'
    &end
  &end // of placeMovieClip 53
  
  &exportAssets
    36 &as 'MapPreviewComponent'
  &end // of exportAssets

  &placeMovieClip 36 &as 'mapRender'
  
    &onClipEvent &construct
      &pushs '_type'
      &pushssv 'ScenarioMapPreview'
      &pushs '_indexMap'
      &pushbyte -1
      &setVariable
    &end
  &end // of placeMovieClip 36

  &defineMovieClip 64 // total frames: 2

    &frame 1
      &stop
    &end // of frame 1
  &end // of defineMovieClip 64

  &defineMovieClip 67 // total frames: 1
  &end // of defineMovieClip 67

  &defineMovieClip 68 // total frames: 1

    &placeMovieClip 64 &as 'renderImage'
    
      &onClipEvent &construct
        &pushs '_type'
        &pushssv 'RenderImage'
        &pushs '_imageMap'
        &pushssv 'ScenarioStartSpotRender0'
        &pushs '_mode'
        &pushssv 'ALPHA'
      &end
    &end // of placeMovieClip 64
  &end // of defineMovieClip 68

  &defineMovieClip 71 // total frames: 1
  &end // of defineMovieClip 71
  
  &importAssets &from '.\\Components\\fe_m_generalScreenComponents.swf'
    'lowerScreenBar Symbol' &as 72
  &end // of importAssets
  
  &importAssets &from '../cafe/mouseComponents/std_MouseButton.swf'
    'std_MousebuttonSymbol' &as 73
  &end // of importAssets

  &placeMovieClip 73 &as 'btnBack'
  
    &onClipEvent &construct
      &constants 'm_width', 'm_textAlign', 'm_labelPosition', 'right align', 'm_label_x', 'm_label_y', 'm_focusDirs', 'Up/Down', 'm_iconType', 'default', 'm_extLabel', '$BACK', 'm_nTruncateType', 'm_label', 'm_bEnabled', 'm_refFM', '_root.gFM', 'm_initiallySelected', 'm_tabIndex', 'm_bVisible', 'm_contentSymbol', 'buttonContentSymbol'  
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
      &pushsdb 11							//'$BACK'
      &setVariable
      &pushsdb 12							//'m_nTruncateType'
      &pushzerosv
      &pushsdb 13							//'m_label'
      &pushsdb 11							//'$BACK'
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
  &end // of placeMovieClip 73

  &placeMovieClip 73 &as 'btnUnlocks'
  
    &onClipEvent &construct
      &constants 'm_width', 'm_textAlign', 'm_labelPosition', 'right align', 'm_label_x', 'm_label_y', 'm_focusDirs', 'Up/Down', 'm_iconType', 'default', 'm_extLabel', '$UNLOCKS', 'm_nTruncateType', 'm_label', 'm_bEnabled', 'm_refFM', '_root.gFM', 'm_initiallySelected', 'm_tabIndex', 'm_bVisible', 'm_contentSymbol', 'buttonContentSymbol'  
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
      &pushsdb 11							//'$UNLOCKS'
      &setVariable
      &pushsdb 12							//'m_nTruncateType'
      &pushzerosv
      &pushsdb 13							//'m_label'
      &pushsdb 11							//'$UNLOCKS'
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
      &pushone
      &setVariable
      &pushsdb 19							//'m_bVisible'
      &pushtrue
      &setVariable
      &pushsdb 20							//'m_contentSymbol'
      &pushsdb 21							//'buttonContentSymbol'
      &setVariable
    &end
  &end // of placeMovieClip 73

  &placeMovieClip 73 &as 'btnPlay'
  
    &onClipEvent &construct
      &constants 'm_width', 'm_textAlign', 'm_labelPosition', 'right align', 'm_label_x', 'm_label_y', 'm_focusDirs', 'Up/Down', 'm_iconType', 'default', 'm_extLabel', '$PLAY', 'm_nTruncateType', 'm_label', 'm_bEnabled', 'm_refFM', '_root.gFM', 'm_initiallySelected', 'm_tabIndex', 'm_bVisible', 'm_contentSymbol', 'buttonContentSymbol'  
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
      &pushsdb 11							//'$PLAY'
      &setVariable
      &pushsdb 12							//'m_nTruncateType'
      &pushzerosv
      &pushsdb 13							//'m_label'
      &pushsdb 11							//'$PLAY'
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
      &pushbyte 2
      &setVariable
      &pushsdb 19							//'m_bVisible'
      &pushtrue
      &setVariable
      &pushsdb 20							//'m_contentSymbol'
      &pushsdb 21							//'buttonContentSymbol'
      &setVariable
    &end
  &end // of placeMovieClip 73
  
  &importAssets &from '.\\Components\\fe_m_generalScreenComponents.swf'
    'ScreenTitleBarSymbol' &as 74
  &end // of importAssets

  &placeMovieClip 74 &as 'screenTitleMC'
  
    &onClipEvent &construct
      &pushs 'm_title'
      &pushssv '$SCENARIO_GAME_SETUP_HEADER'
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
  &end // of placeMovieClip 74

  &defineMovieClip 75 // total frames: 0
  &end // of defineMovieClip 75
  
  &exportAssets
    75 &as '__Packages.Cafe2_BaseUIScreen'
  &end // of exportAssets
  
  &initMovieClip 75
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
  &end // of initMovieClip 75

  &defineMovieClip 76 // total frames: 0
  &end // of defineMovieClip 76
  
  &exportAssets
    76 &as '__Packages.fes_m_scenarioGameSetup'
  &end // of exportAssets
  
  &initMovieClip 76
    &constants '_global', 'fes_m_scenarioGameSetup', 'Cafe2_BaseUIScreen', 'm_thisClass', 'm_localEventHandlerIndex', 'm_localMessageHandlerFunc', 'm_localEventHandlerFunc', 'setup', 'm_mapPreviewMC', 'm_screen', 'mapRender', 'm_originalMapX', '_x', 'm_originalMapY', '_y', 'm_originalMapWidth', '_width', 'm_originalMapHeight', '_height', 'setupMapRenderClip', 'setupMapPlayerIcons', 'prototype', 'localMessageHandler', 'bind1DynamicParams', 'gMH', 'addMessageHandler', 'localEventHandler', 'bind2DynamicParams', 'gEH', 'addLocalEventHandler', 'onScreenExit', 'bind0', 'gSM', 'setOnExitScreen', 'cleanup', 'removeMessageHandler', 'removeLocalEventHandler', 'clearData', 'frameDelayedInit', 'initGUI', 'initData', 'onBackClicked', 'btnBack', 'setOnMouseUpFunction', 'onUnlocksClicked', 'btnUnlocks', 'onPlayClicked', 'btnPlay', 'enable', 'show', 'noIntro', 'onSovietsClicked', 'sovietFaction', 'setOnMouseDownFunction', 'onAlliesClicked', 'alliedFaction', 'onJapanClicked', 'japanFaction', 'scrollBar', 'descriptionTextListBox', 'setVerticalPadding', 'addListener', 'disableEntryHitRegion', 'setNumEntries', 'Array', 'symbol', 'std_mouseListboxEntryTextElementSymbol', 'width', 'leftMargin', 'rightMargin', 'useHighlights', 'isSelectable', 'useHitRegion', 'autoSize', 'push', 'init', 'attachScrollbarController', 'SOUND', 'SCENARIO_WORLD_MAP', 'PlayShellMusic', 'goBackScreen', 'SHELL', 'SCENARIO_UNLOCK_MODE', 'ENUM', 'SCENARIO_UNLOCK_MODE_DEFAULT', 'SCREEN', 'FES_SCENARIO_UNLOCKS', 'changeMainScreen', 'disable', 'FSCommand:CallGameFunction', '%ScenarioSetPlayerFaction?Faction=', 'SOVIET', 'ALLIES', 'JAPAN', 'Object', 'QueryGameEngine?SCENARIO_CURRENT_MISSION', 'SCENARIO_CURRENT_MISSION', 'Scenario_01', 'QueryGameEngine?SCENARIO_PREFERRED_FACTION&SCENARIO_MISSION_DATA?Mission=', '$', 'SCENARIO_MISSION_DATA_NAME', 'screenTitleMC', 'setTitle', 'SCENARIO_MISSION_DATA_DESCRIPTION', 'setMissionDescriptionText', 'recordTimeDataTF', 'text', 'SCENARIO_MISSION_DATA_BESTTIME', 'parTimeDataTF', 'SCENARIO_MISSION_DATA_PARTIME', 'unlockedUnitNameTF', 'SCENARIO_MISSION_DATA_UNITNAME', 'SCENARIO_PREFERRED_FACTION', 'factionController', 'SetSelectedRadioButton', 'SCENARIO_MISSION_DATA_STATE', 'COMPLETED_MISSION_STATE', 'unitPreview', '_mode', 'GRAYSCALE', 'ALPHA', '_refresh', 'gotoAndPlay', ',', 'split', 'setColumnData', 'setSelectedIndex', 'refreshDisplay', 'refreshScrollbar', 'moveViewToFirst', 'gFM', 'handleDefaultInputs', 'QueryGameEngine?SCENARIO_MAP_ASPECT_RATIO', 'SCENARIO_MAP_ASPECT_RATIO', 'posData', 'QueryGameEngine?SCENARIO_START_POSITIONS', 'x', 'SCENARIO_START_POSITIONS_X', 'y', 'SCENARIO_START_POSITIONS_Y', 'vis', 'SCENARIO_START_POSITIONS_VALID', 'colors', 'SCENARIO_START_POSITIONS_COLOR', 'i', 'START_POS_COUNT', 'startPos', '.playerIcon', 'renderImage', '_imageMap', 'ScenarioStartSpotRender', 'length', '1', '_visible', 'xRelativePosition', 'yRelativePosition', 'this', 'newColor', 'frame', 'Color', 'setRGB', 'Soviet', 'Allies', 'Japan', 'ASSetPropFlags'  
    &pushglobalgv
    &pushsdbgm 1							//'fes_m_scenarioGameSetup'
    &not
    &not
    &jnz label21    
    &pushglobalgv
    &pushsdb 1							//'fes_m_scenarioGameSetup'
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
      &pushzero
      &pushregister 1
      &dcallmp 7							// setup()
      &pushregister 1
      &pushsdb 8							//'m_mapPreviewMC'
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 9							//'m_screen'
      &pushsdbgm 10							//'mapRender'
      &setMember
      &pushregister 1
      &pushsdb 11							//'m_originalMapX'
      &pushregister 1
      &pushsdbgm 8							//'m_mapPreviewMC'
      &pushsdbgm 12							//'_x'
      &setMember
      &pushregister 1
      &pushsdb 13							//'m_originalMapY'
      &pushregister 1
      &pushsdbgm 8							//'m_mapPreviewMC'
      &pushsdbgm 14							//'_y'
      &setMember
      &pushregister 1
      &pushsdb 15							//'m_originalMapWidth'
      &pushregister 1
      &pushsdbgm 8							//'m_mapPreviewMC'
      &pushsdbgm 16							//'_width'
      &setMember
      &pushregister 1
      &pushsdb 17							//'m_originalMapHeight'
      &pushregister 1
      &pushsdbgm 8							//'m_mapPreviewMC'
      &pushsdbgm 18							//'_height'
      &setMember
      &getURL 'FSCommand:CallGameFunction'       '%ScenarioCreateSetupRenderImages'      
      &pushzero
      &pushregister 1
      &dcallmp 19							// setupMapRenderClip()
      &pushzero
      &pushregister 1
      &dcallmp 20							// setupMapPlayerIcons()
    &end // of function 

    &setRegister r:1
    &setMember
    &pushglobalgv
    &pushsdbgm 1							//'fes_m_scenarioGameSetup'
    &pushsdbgv 2							//'Cafe2_BaseUIScreen'
    &extends
    &pushregister 1
    &pushsdbgm 21							//'prototype'
    &setRegister r:2
    &pop
    &pushregister 2
    &pushsdb 7							//'setup'
    &function2  () (r:1='this', r:2='_global')
      &pushregister 1
      &pushsdb 5							//'m_localMessageHandlerFunc'
      &pushregister 1
      &pushsdbgm 22							//'localMessageHandler'
      &pushregister 1
      &pushbyte 2
      &pushregister 2
      &pushsdb 23							//'bind1DynamicParams'
      &callMethod
      &setMember
      &pushregister 1
      &pushsdbgm 5							//'m_localMessageHandlerFunc'
      &pushone
      &pushregister 2
      &pushsdbgm 24							//'gMH'
      &dcallmp 25							// addMessageHandler()
      &pushregister 1
      &pushsdb 6							//'m_localEventHandlerFunc'
      &pushregister 1
      &pushsdbgm 26							//'localEventHandler'
      &pushregister 1
      &pushbyte 2
      &pushregister 2
      &pushsdb 27							//'bind2DynamicParams'
      &callMethod
      &setMember
      &pushregister 1
      &pushsdb 4							//'m_localEventHandlerIndex'
      &pushregister 1
      &pushsdbgm 6							//'m_localEventHandlerFunc'
      &pushone
      &pushregister 2
      &pushsdbgm 28							//'gEH'
      &pushsdb 29							//'addLocalEventHandler'
      &callMethod
      &setMember
      &pushregister 1
      &pushsdbgm 30							//'onScreenExit'
      &pushregister 1
      &pushbyte 2
      &pushregister 2
      &pushsdb 31							//'bind0'
      &callMethod
      &pushone
      &pushregister 2
      &pushsdbgm 32							//'gSM'
      &dcallmp 33							// setOnExitScreen()
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 34							//'cleanup'
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
      &pushsdbgm 24							//'gMH'
      &dcallmp 35							// removeMessageHandler()
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
      &pushsdbgm 28							//'gEH'
      &dcallmp 36							// removeLocalEventHandler()
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
      &dcallmp 37							// clearData()
      &pushregister 2
      &pushsdb 1							//'fes_m_scenarioGameSetup'
      &delete
      &pop
      &pushregister 2
      &pushsdb 2							//'Cafe2_BaseUIScreen'
      &delete
      &pop
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 38							//'frameDelayedInit'
    &function2  () (r:1='this')
      &pushzero
      &pushregister 1
      &dcallmp 39							// initGUI()
      &pushzero
      &pushregister 1
      &dcallmp 40							// initData()
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 39							//'initGUI'
    &function2  () (r:1='this', r:2='_global')
      &pushregister 1
      &pushsdbgm 41							//'onBackClicked'
      &pushregister 1
      &pushbyte 2
      &pushregister 2
      &pushsdb 31							//'bind0'
      &callMethod
      &pushone
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 9							//'m_screen'
      &pushsdbgm 42							//'btnBack'
      &dcallmp 43							// setOnMouseUpFunction()
      &pushregister 1
      &pushsdbgm 44							//'onUnlocksClicked'
      &pushregister 1
      &pushbyte 2
      &pushregister 2
      &pushsdb 31							//'bind0'
      &callMethod
      &pushone
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 9							//'m_screen'
      &pushsdbgm 45							//'btnUnlocks'
      &dcallmp 43							// setOnMouseUpFunction()
      &pushregister 1
      &pushsdbgm 46							//'onPlayClicked'
      &pushregister 1
      &pushbyte 2
      &pushregister 2
      &pushsdb 31							//'bind0'
      &callMethod
      &pushone
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 9							//'m_screen'
      &pushsdbgm 47							//'btnPlay'
      &dcallmp 43							// setOnMouseUpFunction()
      &pushzero
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 9							//'m_screen'
      &pushsdbgm 42							//'btnBack'
      &dcallmp 48							// enable()
      &pushzero
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 9							//'m_screen'
      &pushsdbgm 45							//'btnUnlocks'
      &dcallmp 48							// enable()
      &pushzero
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 9							//'m_screen'
      &pushsdbgm 47							//'btnPlay'
      &dcallmp 48							// enable()
      &pushzero
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 9							//'m_screen'
      &pushsdbgm 42							//'btnBack'
      &dcallmp 49							// show()
      &pushzero
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 9							//'m_screen'
      &pushsdbgm 45							//'btnUnlocks'
      &dcallmp 49							// show()
      &pushzero
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 9							//'m_screen'
      &pushsdbgm 47							//'btnPlay'
      &dcallmp 49							// show()
      &pushzero
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 9							//'m_screen'
      &pushsdbgm 42							//'btnBack'
      &dcallmp 50							// noIntro()
      &pushzero
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 9							//'m_screen'
      &pushsdbgm 45							//'btnUnlocks'
      &dcallmp 50							// noIntro()
      &pushzero
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 9							//'m_screen'
      &pushsdbgm 47							//'btnPlay'
      &dcallmp 50							// noIntro()
      &pushregister 1
      &pushsdbgm 51							//'onSovietsClicked'
      &pushregister 1
      &pushbyte 2
      &pushregister 2
      &pushsdb 31							//'bind0'
      &callMethod
      &pushone
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 9							//'m_screen'
      &pushsdbgm 52							//'sovietFaction'
      &dcallmp 53							// setOnMouseDownFunction()
      &pushregister 1
      &pushsdbgm 54							//'onAlliesClicked'
      &pushregister 1
      &pushbyte 2
      &pushregister 2
      &pushsdb 31							//'bind0'
      &callMethod
      &pushone
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 9							//'m_screen'
      &pushsdbgm 55							//'alliedFaction'
      &dcallmp 53							// setOnMouseDownFunction()
      &pushregister 1
      &pushsdbgm 56							//'onJapanClicked'
      &pushregister 1
      &pushbyte 2
      &pushregister 2
      &pushsdb 31							//'bind0'
      &callMethod
      &pushone
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 9							//'m_screen'
      &pushsdbgm 57							//'japanFaction'
      &dcallmp 53							// setOnMouseDownFunction()
      &pushzero
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 9							//'m_screen'
      &pushsdbgm 52							//'sovietFaction'
      &dcallmp 50							// noIntro()
      &pushzero
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 9							//'m_screen'
      &pushsdbgm 55							//'alliedFaction'
      &dcallmp 50							// noIntro()
      &pushzero
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 9							//'m_screen'
      &pushsdbgm 57							//'japanFaction'
      &dcallmp 50							// noIntro()
      &pushzero
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 9							//'m_screen'
      &pushsdbgm 58							//'scrollBar'
      &dcallmp 49							// show()
      &pushzero
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 9							//'m_screen'
      &pushsdbgm 58							//'scrollBar'
      &dcallmp 48							// enable()
      &pushzero
      &pushone
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 9							//'m_screen'
      &pushsdbgm 59							//'descriptionTextListBox'
      &dcallmp 60							// setVerticalPadding()
      &pushregister 1
      &pushone
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 9							//'m_screen'
      &pushsdbgm 59							//'descriptionTextListBox'
      &dcallmp 61							// addListener()
      &pushzero
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 9							//'m_screen'
      &pushsdbgm 59							//'descriptionTextListBox'
      &dcallmp 62							// disableEntryHitRegion()
      &pushone
      &pushone
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 9							//'m_screen'
      &pushsdbgm 59							//'descriptionTextListBox'
      &dcallmp 63							// setNumEntries()
      &pushzero
      &pushsdb 64							//'Array'
      &new
      &setRegister r:3
      &pop
      &pushsdb 65							//'symbol'
      &pushsdb 66							//'std_mouseListboxEntryTextElementSymbol'
      &pushsdb 67							//'width'
      &pushshort 900
      &pushsdb 68							//'leftMargin'
      &pushbyte 10
      &pushsdb 69							//'rightMargin'
      &pushbyte 60
      &pushsdb 70							//'useHighlights'
      &pushfalse
      &pushsdb 71							//'isSelectable'
      &pushfalse
      &pushsdb 72							//'useHitRegion'
      &pushfalse
      &pushsdb 73							//'autoSize'
      &pushtrue
      &pushbyte 8
      &initObject
      &pushone
      &pushregister 3
      &dcallmp 74							// push()
      &pushbyte 29
      &pushregister 3
      &pushbyte 2
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 9							//'m_screen'
      &pushsdbgm 59							//'descriptionTextListBox'
      &dcallmp 75							// init()
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 9							//'m_screen'
      &pushsdbgm 58							//'scrollBar'
      &pushone
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 9							//'m_screen'
      &pushsdbgm 59							//'descriptionTextListBox'
      &dcallmp 76							// attachScrollbarController()
      &pushregister 2
      &pushsdbgm 77							//'SOUND'
      &pushsdbgm 78							//'SCENARIO_WORLD_MAP'
      &pushone
      &pushregister 2
      &dcallmp 79							// PlayShellMusic()
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 30							//'onScreenExit'
    &function2  () (r:1='this')
      &getURL 'FSCommand:CallGameFunction'       '%ScenarioDestroySetupRenderImages'      
      &pushzero
      &pushregister 1
      &dcallmp 34							// cleanup()
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 41							//'onBackClicked'
    &function2  () (r:1='_global')
      &pushzero
      &pushregister 1
      &pushsdbgm 32							//'gSM'
      &dcallmp 80							// goBackScreen()
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 44							//'onUnlocksClicked'
    &function2  () (r:1='_global')
      &pushregister 1
      &pushsdbgm 81							//'SHELL'
      &pushsdb 82							//'SCENARIO_UNLOCK_MODE'
      &pushregister 1
      &pushsdbgm 83							//'ENUM'
      &pushsdbgm 84							//'SCENARIO_UNLOCK_MODE_DEFAULT'
      &setMember
      &pushregister 1
      &pushsdbgm 85							//'SCREEN'
      &pushsdbgm 86							//'FES_SCENARIO_UNLOCKS'
      &pushone
      &pushregister 1
      &pushsdbgm 32							//'gSM'
      &dcallmp 87							// changeMainScreen()
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 46							//'onPlayClicked'
    &function  (    )    
      &pushzero
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 9							//'m_screen'
      &pushsdbgm 42							//'btnBack'
      &dcallmp 88							// disable()
      &pushzero
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 9							//'m_screen'
      &pushsdbgm 45							//'btnUnlocks'
      &dcallmp 88							// disable()
      &pushzero
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 9							//'m_screen'
      &pushsdbgm 47							//'btnPlay'
      &dcallmp 88							// disable()
      &getURL 'FSCommand:CallGameFunction'       '%ScenarioStartMission'      
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 22							//'localMessageHandler'
    &function2  (r:2='message') ()
      &pushtrue
      &setRegister r:1
      &pop
      &jmp label3      
     label3:
      &pushfalse
      &setRegister r:1
      &pop
      &jmp label4      
     label4:
      &pushregister 1
      &return
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 51							//'onSovietsClicked'
    &function  (    )    
      &pushsdb 89							//'FSCommand:CallGameFunction'
      &pushsdb 90							//'%ScenarioSetPlayerFaction?Faction='
      &pushsdbgv 1							//'fes_m_scenarioGameSetup'
      &pushsdbgm 91							//'SOVIET'
      &add
      &getURL2
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 54							//'onAlliesClicked'
    &function  (    )    
      &pushsdb 89							//'FSCommand:CallGameFunction'
      &pushsdb 90							//'%ScenarioSetPlayerFaction?Faction='
      &pushsdbgv 1							//'fes_m_scenarioGameSetup'
      &pushsdbgm 92							//'ALLIES'
      &add
      &getURL2
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 56							//'onJapanClicked'
    &function  (    )    
      &pushsdb 89							//'FSCommand:CallGameFunction'
      &pushsdb 90							//'%ScenarioSetPlayerFaction?Faction='
      &pushsdbgv 1							//'fes_m_scenarioGameSetup'
      &pushsdbgm 93							//'JAPAN'
      &add
      &getURL2
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 40							//'initData'
    &function2  () (r:1='this')
      &pushzero
      &pushsdb 94							//'Object'
      &new
      &setRegister r:3
      &pop
      &pushsdb 95							//'QueryGameEngine?SCENARIO_CURRENT_MISSION'
      &pushregister 3
      &loadVariables
      &pushregister 3
      &pushsdbgm 96							//'SCENARIO_CURRENT_MISSION'
      &pushundef
      &equals
      &not
      &jnz label5      
      &pushregister 3
      &pushsdb 96							//'SCENARIO_CURRENT_MISSION'
      &pushsdb 97							//'Scenario_01'
      &setMember
     label5:
      &pushzero
      &pushsdb 94							//'Object'
      &new
      &setRegister r:2
      &pop
      &pushsdb 98							//'QueryGameEngine?SCENARIO_PREFERRED_FACTION&SCENARIO_MISSION_DATA?Mission='
      &pushregister 3
      &pushsdbgm 96							//'SCENARIO_CURRENT_MISSION'
      &add
      &pushregister 2
      &loadVariables
      &pushtrue
      &pushtrue
      &pushsdb 99							//'$'
      &pushregister 2
      &pushsdbgm 100							//'SCENARIO_MISSION_DATA_NAME'
      &add
      &pushbyte 3
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 9							//'m_screen'
      &pushsdbgm 101							//'screenTitleMC'
      &dcallmp 102							// setTitle()
      &pushsdb 99							//'$'
      &pushregister 2
      &pushsdbgm 103							//'SCENARIO_MISSION_DATA_DESCRIPTION'
      &add
      &pushone
      &pushregister 1
      &dcallmp 104							// setMissionDescriptionText()
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 9							//'m_screen'
      &pushsdbgm 105							//'recordTimeDataTF'
      &pushsdb 106							//'text'
      &pushsdb 99							//'$'
      &pushregister 2
      &pushsdbgm 107							//'SCENARIO_MISSION_DATA_BESTTIME'
      &add
      &setMember
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 9							//'m_screen'
      &pushsdbgm 108							//'parTimeDataTF'
      &pushsdb 106							//'text'
      &pushsdb 99							//'$'
      &pushregister 2
      &pushsdbgm 109							//'SCENARIO_MISSION_DATA_PARTIME'
      &add
      &setMember
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 9							//'m_screen'
      &pushsdbgm 110							//'unlockedUnitNameTF'
      &pushsdb 106							//'text'
      &pushsdb 99							//'$'
      &pushregister 2
      &pushsdbgm 111							//'SCENARIO_MISSION_DATA_UNITNAME'
      &add
      &setMember
      &pushregister 2
      &pushsdbgm 112							//'SCENARIO_PREFERRED_FACTION'
      &setRegister r:0
      &pushsdbgv 1							//'fes_m_scenarioGameSetup'
      &pushsdbgm 91							//'SOVIET'
      &strictEquals
      &jnz label6      
      &pushregister 0
      &pushsdbgv 1							//'fes_m_scenarioGameSetup'
      &pushsdbgm 92							//'ALLIES'
      &strictEquals
      &jnz label7      
      &pushregister 0
      &pushsdbgv 1							//'fes_m_scenarioGameSetup'
      &pushsdbgm 93							//'JAPAN'
      &strictEquals
      &jnz label8      
      &jmp label6      
     label6:
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 9							//'m_screen'
      &pushsdbgm 52							//'sovietFaction'
      &pushone
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 9							//'m_screen'
      &pushsdbgm 113							//'factionController'
      &dcallmp 114							// SetSelectedRadioButton()
      &jmp label9      
     label7:
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 9							//'m_screen'
      &pushsdbgm 55							//'alliedFaction'
      &pushone
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 9							//'m_screen'
      &pushsdbgm 113							//'factionController'
      &dcallmp 114							// SetSelectedRadioButton()
      &jmp label9      
     label8:
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 9							//'m_screen'
      &pushsdbgm 57							//'japanFaction'
      &pushone
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 9							//'m_screen'
      &pushsdbgm 113							//'factionController'
      &dcallmp 114							// SetSelectedRadioButton()
      &jmp label9      
     label9:
      &pushregister 2
      &pushsdbgm 115							//'SCENARIO_MISSION_DATA_STATE'
      &toNumber
      &setRegister r:4
      &pop
      &pushregister 4
      &pushsdbgv 1							//'fes_m_scenarioGameSetup'
      &pushsdbgm 116							//'COMPLETED_MISSION_STATE'
      &lessThan
      &not
      &jnz label10      
      &pushsdbgv 117							//'unitPreview'
      &pushsdb 118							//'_mode'
      &pushsdb 119							//'GRAYSCALE'
      &setMember
      &jmp label11      
     label10:
      &pushsdbgv 117							//'unitPreview'
      &pushsdb 118							//'_mode'
      &pushsdb 120							//'ALPHA'
      &setMember
     label11:
      &pushsdb 121							//'_refresh'
      &pushone
      &pushsdbgv 117							//'unitPreview'
      &dcallmp 122							// gotoAndPlay()
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 104							//'setMissionDescriptionText'
    &function2  (r:2='missionDescription') ()
      &pushsdb 123							//','
      &pushone
      &pushregister 2
      &pushsdb 124							//'split'
      &callMethod
      &setRegister r:1
      &pop
      &pushregister 1
      &pushzero
      &pushbyte 2
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 9							//'m_screen'
      &pushsdbgm 59							//'descriptionTextListBox'
      &dcallmp 125							// setColumnData()
      &pushzero
      &pushone
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 9							//'m_screen'
      &pushsdbgm 59							//'descriptionTextListBox'
      &dcallmp 126							// setSelectedIndex()
      &pushzero
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 9							//'m_screen'
      &pushsdbgm 59							//'descriptionTextListBox'
      &dcallmp 127							// refreshDisplay()
      &pushzero
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 9							//'m_screen'
      &pushsdbgm 59							//'descriptionTextListBox'
      &dcallmp 128							// refreshScrollbar()
      &pushzero
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 9							//'m_screen'
      &pushsdbgm 59							//'descriptionTextListBox'
      &dcallmp 129							// moveViewToFirst()
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 26							//'localEventHandler'
    &function2  (r:2='key', 'controller') (r:1='_global')
      &jmp label12      
     label12:
      &pushregister 2
      &pushone
      &pushregister 1
      &pushsdbgm 130							//'gFM'
      &dcallmp 131							// handleDefaultInputs()
      &jmp label13      
     label13:
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 19							//'setupMapRenderClip'
    &function2  () (r:1='this')
      &pushzero
      &pushsdb 94							//'Object'
      &new
      &setRegister r:2
      &pop
      &pushsdb 132							//'QueryGameEngine?SCENARIO_MAP_ASPECT_RATIO'
      &pushregister 2
      &loadVariables
      &pushregister 2
      &pushsdbgm 133							//'SCENARIO_MAP_ASPECT_RATIO'
      &toNumber
      &setRegister r:3
      &pop
      &pushregister 3
      &pushregister 1
      &pushsdbgm 15							//'m_originalMapWidth'
      &pushregister 1
      &pushsdbgm 17							//'m_originalMapHeight'
      &divide
      &greaterThan
      &not
      &jnz label14      
      &pushregister 1
      &pushsdbgm 8							//'m_mapPreviewMC'
      &pushsdb 16							//'_width'
      &pushregister 1
      &pushsdbgm 15							//'m_originalMapWidth'
      &setMember
      &pushregister 1
      &pushsdbgm 8							//'m_mapPreviewMC'
      &pushsdb 18							//'_height'
      &pushregister 1
      &pushsdbgm 15							//'m_originalMapWidth'
      &pushregister 3
      &divide
      &setMember
      &pushregister 1
      &pushsdbgm 8							//'m_mapPreviewMC'
      &pushsdb 12							//'_x'
      &pushregister 1
      &pushsdbgm 11							//'m_originalMapX'
      &setMember
      &pushregister 1
      &pushsdbgm 8							//'m_mapPreviewMC'
      &pushsdb 14							//'_y'
      &pushregister 1
      &pushsdbgm 13							//'m_originalMapY'
      &pushregister 1
      &pushsdbgm 17							//'m_originalMapHeight'
      &pushregister 1
      &pushsdbgm 8							//'m_mapPreviewMC'
      &pushsdbgm 18							//'_height'
      &subtract
      &pushfloat 0.500000000000000
      &multiply
      &add
      &setMember
      &jmp label15      
     label14:
      &pushregister 1
      &pushsdbgm 8							//'m_mapPreviewMC'
      &pushsdb 18							//'_height'
      &pushregister 1
      &pushsdbgm 17							//'m_originalMapHeight'
      &setMember
      &pushregister 1
      &pushsdbgm 8							//'m_mapPreviewMC'
      &pushsdb 16							//'_width'
      &pushregister 1
      &pushsdbgm 17							//'m_originalMapHeight'
      &pushregister 3
      &multiply
      &setMember
      &pushregister 1
      &pushsdbgm 8							//'m_mapPreviewMC'
      &pushsdb 14							//'_y'
      &pushregister 1
      &pushsdbgm 13							//'m_originalMapY'
      &setMember
      &pushregister 1
      &pushsdbgm 8							//'m_mapPreviewMC'
      &pushsdb 12							//'_x'
      &pushregister 1
      &pushsdbgm 11							//'m_originalMapX'
      &pushregister 1
      &pushsdbgm 15							//'m_originalMapWidth'
      &pushregister 1
      &pushsdbgm 8							//'m_mapPreviewMC'
      &pushsdbgm 16							//'_width'
      &subtract
      &pushfloat 0.500000000000000
      &multiply
      &add
      &setMember
     label15:
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 20							//'setupMapPlayerIcons'
    &function  (    )    
      &pushsdb 134							//'posData'
      &pushzero
      &pushsdb 94							//'Object'
      &new
      &varEquals
      &pushsdb 135							//'QueryGameEngine?SCENARIO_START_POSITIONS'
      &pushsdbgv 134							//'posData'
      &loadVariables
      &pushsdb 136							//'x'
      &pushsdb 123							//','
      &pushone
      &pushsdbgv 134							//'posData'
      &pushsdbgm 137							//'SCENARIO_START_POSITIONS_X'
      &pushsdb 124							//'split'
      &callMethod
      &varEquals
      &pushsdb 138							//'y'
      &pushsdb 123							//','
      &pushone
      &pushsdbgv 134							//'posData'
      &pushsdbgm 139							//'SCENARIO_START_POSITIONS_Y'
      &pushsdb 124							//'split'
      &callMethod
      &varEquals
      &pushsdb 140							//'vis'
      &pushsdb 123							//','
      &pushone
      &pushsdbgv 134							//'posData'
      &pushsdbgm 141							//'SCENARIO_START_POSITIONS_VALID'
      &pushsdb 124							//'split'
      &callMethod
      &varEquals
      &pushsdb 142							//'colors'
      &pushsdb 123							//','
      &pushone
      &pushsdbgv 134							//'posData'
      &pushsdbgm 143							//'SCENARIO_START_POSITIONS_COLOR'
      &pushsdb 124							//'split'
      &callMethod
      &varEquals
      &pushsdb 144							//'i'
      &pushzero
      &varEquals
     label16:
      &pushsdbgv 144							//'i'
      &pushsdbgv 1							//'fes_m_scenarioGameSetup'
      &pushsdbgm 145							//'START_POS_COUNT'
      &lessThan
      &not
      &jnz label20      
      &pushsdb 146							//'startPos'
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 9							//'m_screen'
      &toString
      &pushsdb 147							//'.playerIcon'
      &add
      &pushsdbgv 144							//'i'
      &add
      &getVariable
      &varEquals
      &pushsdbgv 146							//'startPos'
      &pushsdbgm 148							//'renderImage'
      &pushsdb 149							//'_imageMap'
      &pushsdb 150							//'ScenarioStartSpotRender'
      &pushsdbgv 144							//'i'
      &add
      &setMember
      &pushsdb 121							//'_refresh'
      &pushone
      &pushsdbgv 146							//'startPos'
      &pushsdbgm 148							//'renderImage'
      &dcallmp 122							// gotoAndPlay()
      &pushsdbgv 144							//'i'
      &pushsdbgv 140							//'vis'
      &pushsdbgm 151							//'length'
      &lessThan
      &dup
      &not
      &jnz label17      
      &pop
      &pushsdbgv 140							//'vis'
      &pushsdbgv 144							//'i'
      &getMember
      &pushsdb 152							//'1'
      &equals
     label17:
      &not
      &jnz label18      
      &pushsdbgv 146							//'startPos'
      &pushsdb 153							//'_visible'
      &pushtrue
      &setMember
      &pushsdb 154							//'xRelativePosition'
      &pushsdbgv 136							//'x'
      &pushsdbgv 144							//'i'
      &getMember
      &varEquals
      &pushsdb 155							//'yRelativePosition'
      &pushsdbgv 138							//'y'
      &pushsdbgv 144							//'i'
      &getMember
      &varEquals
      &pushsdbgv 146							//'startPos'
      &pushsdb 12							//'_x'
      &pushsdbgv 154							//'xRelativePosition'
      &pushthisgv
      &pushsdbgm 8							//'m_mapPreviewMC'
      &pushsdbgm 16							//'_width'
      &multiply
      &pushthisgv
      &pushsdbgm 8							//'m_mapPreviewMC'
      &pushsdbgm 12							//'_x'
      &add
      &setMember
      &pushsdbgv 146							//'startPos'
      &pushsdb 14							//'_y'
      &pushsdbgv 155							//'yRelativePosition'
      &pushthisgv
      &pushsdbgm 8							//'m_mapPreviewMC'
      &pushsdbgm 18							//'_height'
      &multiply
      &pushthisgv
      &pushsdbgm 8							//'m_mapPreviewMC'
      &pushsdbgm 14							//'_y'
      &add
      &setMember
      &pushsdb 157							//'newColor'
      &pushsdbgv 146							//'startPos'
      &pushsdbgm 158							//'frame'
      &pushone
      &pushsdb 159							//'Color'
      &new
      &varEquals
      &pushsdbgv 142							//'colors'
      &pushsdbgv 144							//'i'
      &getMember
      &toNumber
      &pushone
      &pushsdbgv 157							//'newColor'
      &dcallmp 160							// setRGB()
      &jmp label19      
     label18:
      &pushsdbgv 146							//'startPos'
      &pushsdb 153							//'_visible'
      &pushfalse
      &setMember
     label19:
      &pushsdb 144							//'i'
      &pushsdbgv 144							//'i'
      &increment
      &setVariable
      &jmp label16      
     label20:
    &end // of function 

    &setMember
    &pushregister 1
    &pushsdb 145							//'START_POS_COUNT'
    &pushbyte 6
    &setMember
    &pushregister 1
    &pushsdb 91							//'SOVIET'
    &pushsdb 161							//'Soviet'
    &setMember
    &pushregister 1
    &pushsdb 92							//'ALLIES'
    &pushsdb 162							//'Allies'
    &setMember
    &pushregister 1
    &pushsdb 93							//'JAPAN'
    &pushsdb 163							//'Japan'
    &setMember
    &pushregister 1
    &pushsdb 116							//'COMPLETED_MISSION_STATE'
    &pushbyte 5
    &setMember
    &pushone
    &pushnull
    &pushglobalgv
    &pushsdbgm 1							//'fes_m_scenarioGameSetup'
    &pushsdbgm 21							//'prototype'
    &pushbyte 3
    &pushsdb 164							//'ASSetPropFlags'
    &callFunction
   label21:
    &pop
  &end // of initMovieClip 76
&end
