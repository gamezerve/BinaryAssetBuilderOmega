movie 'E:\depot\CnC\RA3_EP1\PRODUC~1\Data\APTBUI~1\034A3~1.0-D\pc\Output\MOUSE_~1\\MOUSE_~1.eaf' &compressed // flash 7, total frames: 1, frame rate: 30 fps, 800x600 px

  &defineMovieClip 3 // total frames: 1
  &end // of defineMovieClip 3

  &defineMovieClip 4 // total frames: 10

    &frame 4
      &stop
    &end // of frame 4

    &frame 9
      &stop
    &end // of frame 9
  &end // of defineMovieClip 4

  &defineMovieClip 7 // total frames: 1
  &end // of defineMovieClip 7

  &defineMovieClip 10 // total frames: 1
  &end // of defineMovieClip 10

  &defineMovieClip 11 // total frames: 10

    &frame 4
      &stop
    &end // of frame 4

    &frame 9
      &stop
    &end // of frame 9
  &end // of defineMovieClip 11

  &defineMovieClip 14 // total frames: 1
  &end // of defineMovieClip 14

  &defineMovieClip 17 // total frames: 1
  &end // of defineMovieClip 17

  &defineMovieClip 18 // total frames: 10

    &frame 4
      &stop
    &end // of frame 4

    &frame 9
      &stop
    &end // of frame 9
  &end // of defineMovieClip 18

  &defineMovieClip 21 // total frames: 1
  &end // of defineMovieClip 21

  &defineMovieClip 24 // total frames: 1
  &end // of defineMovieClip 24

  &defineMovieClip 25 // total frames: 10

    &frame 4
      &stop
    &end // of frame 4

    &frame 9
      &stop
    &end // of frame 9
  &end // of defineMovieClip 25
  
  &importAssets &from '../cafe/mouseComponents/std_mouseRadioButton.swf'
    'std_mouseRadioButtonControllerSymbol' &as 26
  &end // of importAssets
  
  &importAssets &from '../cafe/mouseComponents/std_mouseRadioButton.swf'
    'std_mouseRadioButtonSymbol' &as 27
  &end // of importAssets
  
  &importAssets &from '../cafe/mouseComponents/std_mouseScrollingListbox.swf'
    'std_mouseScrollingListboxSymbol' &as 28
  &end // of importAssets

  &defineMovieClip 29 // total frames: 1

    &placeMovieClip 26 &as 'radioController_faction'
    
      &onClipEvent &construct
        &pushs 'm_name'
        &pushssv 'RadioButtonController'
      &end
    &end // of placeMovieClip 26

    &placeMovieClip 27 &as 'radioButton_soviets'
    
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
        &pushssv '$APT:SOVIETSSHORTNAME&Outline'
        &pushs 'm_bEnabled'
        &pushtrue
        &setVariable
        &pushs 'm_refFM'
        &pushssv '_root.gFM'
        &pushs 'm_initiallySelected'
        &pushtrue
        &setVariable
        &pushs 'm_tabIndex'
        &pushbyte -1
        &setVariable
        &pushs 'm_bVisible'
        &pushtrue
        &setVariable
        &pushs 'm_contentSymbol'
        &pushssv 'ChapterSelectRadioButtonContentClip'
        &pushs 'm_controller'
        &pushssv 'radioController_faction'
      &end
    &end // of placeMovieClip 27

    &placeMovieClip 27 &as 'radioButton_allies'
    
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
        &pushssv '$APT:ALLIESSHORTNAME&Outline'
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
        &pushssv 'ChapterSelectRadioButtonContentClip'
        &pushs 'm_controller'
        &pushssv 'radioController_faction'
      &end
    &end // of placeMovieClip 27

    &placeMovieClip 27 &as 'radioButton_japan'
    
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
        &pushssv '$APT:JAPANSHORTNAME&Outline'
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
        &pushssv 'ChapterSelectRadioButtonContentClip'
        &pushs 'm_controller'
        &pushssv 'radioController_faction'
      &end
    &end // of placeMovieClip 27

    &placeMovieClip 27 &as 'radioButton_yuriko'
    
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
        &pushssv '$APT:YURIKOSHORTNAME&Outline'
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
        &pushssv 'ChapterSelectRadioButtonContentClip'
        &pushs 'm_controller'
        &pushssv 'radioController_faction'
      &end
    &end // of placeMovieClip 27

    &placeMovieClip 28 &as 'missionList'
    
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
        &pushbyte 9
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
        &pushshort 435
        &setVariable
        &pushs 'm_renderHeight'
        &pushzerosv
        &pushs 'm_backgroundMargin'
        &pushbyte 3
        &setVariable
        &pushs 'm_useHighlights'
        &pushtrue
        &setVariable
        &pushs 'm_alternateBackgroundColors'
        &pushtrue
        &setVariable
        &pushs 'm_backgroundColor1'
        &pushlong 2622983
        &setVariable
        &pushs 'm_backgroundColor2'
        &pushzerosv
      &end
    &end // of placeMovieClip 28
  &end // of defineMovieClip 29
  
  &exportAssets
    29 &as 'chapterSelectComponentSymbol'
  &end // of exportAssets

  &defineMovieClip 31 // total frames: 1
  &end // of defineMovieClip 31
  
  &exportAssets
    31 &as 'disabledFlagsSymbol'
  &end // of exportAssets
&end
