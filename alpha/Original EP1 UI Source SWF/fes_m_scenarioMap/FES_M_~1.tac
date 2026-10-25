movie 'E:\depot\CnC\RA3_EP1\PRODUC~1\Data\APTBUI~1\034A3~1.0-D\pc\Output\FEF2BB~1\\FES_M_~1.eaf' &compressed // flash 7, total frames: 1, frame rate: 30 fps, 1365x768 px

  &frame 0
    &stop
    &pushs 'screen'
    &pushthisgv
    &pushone
    &pushs 'fes_m_scenarioMap'
    &new
    &varEquals
  &end // of frame 0

  &defineMovieClip 3 // total frames: 1
  &end // of defineMovieClip 3

  &defineMovieClip 7 // total frames: 1
  &end // of defineMovieClip 7

  &defineMovieClip 8 // total frames: 240
  &end // of defineMovieClip 8

  &defineMovieClip 10 // total frames: 1
  &end // of defineMovieClip 10

  &defineMovieClip 11 // total frames: 1
  &end // of defineMovieClip 11
  
  &importAssets &from '.\\Components\\fe_m_generalScreenComponents.swf'
    'lowerScreenBar Symbol' &as 12
  &end // of importAssets
  
  &importAssets &from '.\\Components\\fe_m_generalScreenComponents.swf'
    'ScreenTitleBarSymbol' &as 13
  &end // of importAssets

  &placeMovieClip 13 
  
    &onClipEvent &construct
      &pushs 'm_title'
      &pushssv '$SCENARIO_MAP_HEADER'
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
  &end // of placeMovieClip 13
  
  &importAssets &from '../cafe/mouseComponents/std_MouseButton.swf'
    'std_MousebuttonSymbol' &as 14
  &end // of importAssets

  &placeMovieClip 14 &as 'btnBack'
  
    &onClipEvent &construct
      &constants 'm_width', 'm_textAlign', 'm_labelPosition', 'right align', 'm_label_x', 'm_label_y', 'm_focusDirs', 'Up/Down', 'm_iconType', 'default', 'm_extLabel', '$MAIN_MENU', 'm_nTruncateType', 'm_label', 'm_bEnabled', 'm_refFM', '_root.gFM', 'm_initiallySelected', 'm_tabIndex', 'm_bVisible', 'm_contentSymbol', 'buttonContentSymbol'  
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
      &pushsdb 11							//'$MAIN_MENU'
      &setVariable
      &pushsdb 12							//'m_nTruncateType'
      &pushzerosv
      &pushsdb 13							//'m_label'
      &pushsdb 11							//'$MAIN_MENU'
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
  &end // of placeMovieClip 14

  &placeMovieClip 14 &as 'btnUnlocks'
  
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
      &pushbyte 2
      &setVariable
      &pushsdb 19							//'m_bVisible'
      &pushtrue
      &setVariable
      &pushsdb 20							//'m_contentSymbol'
      &pushsdb 21							//'buttonContentSymbol'
      &setVariable
    &end
  &end // of placeMovieClip 14

  &placeMovieClip 14 &as 'btnHelp'
  
    &onClipEvent &construct
      &constants 'm_width', 'm_textAlign', 'm_labelPosition', 'right align', 'm_label_x', 'm_label_y', 'm_focusDirs', 'Up/Down', 'm_iconType', 'default', 'm_extLabel', '$HELP', 'm_nTruncateType', 'm_label', 'm_bEnabled', 'm_refFM', '_root.gFM', 'm_initiallySelected', 'm_tabIndex', 'm_bVisible', 'm_contentSymbol', 'buttonContentSymbol'  
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
      &pushsdb 11							//'$HELP'
      &setVariable
      &pushsdb 12							//'m_nTruncateType'
      &pushzerosv
      &pushsdb 13							//'m_label'
      &pushsdb 11							//'$HELP'
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
  &end // of placeMovieClip 14

  &placeMovieClip 14 &as 'btnContinue'
  
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
      &pushbyte 3
      &setVariable
      &pushsdb 19							//'m_bVisible'
      &pushtrue
      &setVariable
      &pushsdb 20							//'m_contentSymbol'
      &pushsdb 21							//'buttonContentSymbol'
      &setVariable
    &end
  &end // of placeMovieClip 14

  &defineMovieClip 17 // total frames: 1
  &end // of defineMovieClip 17

  &defineMovieClip 20 // total frames: 1
  &end // of defineMovieClip 20

  &defineMovieClip 21 // total frames: 30
  &end // of defineMovieClip 21

  &defineMovieClip 22 // total frames: 17

    &frame 1
      &stop
    &end // of frame 1

    &frame 16
      &stop
    &end // of frame 16
  &end // of defineMovieClip 22
  
  &importAssets &from '../mouse/components/mouse_scenarioComponents.swf'
    'scenariomarkericon' &as 23
  &end // of importAssets

  &placeMovieClip 23 &as 'marker_2'
  
    &onClipEvent &construct
      &pushs 'm_uiName'
      &pushssv 'Scenario_02'
    &end
  &end // of placeMovieClip 23

  &placeMovieClip 23 &as 'marker_3'
  
    &onClipEvent &construct
      &pushs 'm_uiName'
      &pushssv 'Scenario_03'
    &end
  &end // of placeMovieClip 23

  &placeMovieClip 23 &as 'marker_1'
  
    &onClipEvent &construct
      &pushs 'm_uiName'
      &pushssv 'Scenario_01'
    &end
  &end // of placeMovieClip 23

  &placeMovieClip 23 &as 'marker_7'
  
    &onClipEvent &construct
      &pushs 'm_uiName'
      &pushssv 'Scenario_07'
    &end
  &end // of placeMovieClip 23

  &placeMovieClip 23 &as 'marker_11'
  
    &onClipEvent &construct
      &pushs 'm_uiName'
      &pushssv 'Scenario_11'
    &end
  &end // of placeMovieClip 23

  &placeMovieClip 23 &as 'marker_13'
  
    &onClipEvent &construct
      &pushs 'm_uiName'
      &pushssv 'Scenario_13'
    &end
  &end // of placeMovieClip 23

  &placeMovieClip 23 &as 'marker_16'
  
    &onClipEvent &construct
      &pushs 'm_uiName'
      &pushssv 'Scenario_16'
    &end
  &end // of placeMovieClip 23

  &placeMovieClip 23 &as 'marker_17'
  
    &onClipEvent &construct
      &pushs 'm_uiName'
      &pushssv 'Scenario_17'
    &end
  &end // of placeMovieClip 23

  &placeMovieClip 23 &as 'marker_28'
  
    &onClipEvent &construct
      &pushs 'm_uiName'
      &pushssv 'Scenario_28'
    &end
  &end // of placeMovieClip 23

  &placeMovieClip 23 &as 'marker_32'
  
    &onClipEvent &construct
      &pushs 'm_uiName'
      &pushssv 'Scenario_32'
    &end
  &end // of placeMovieClip 23

  &placeMovieClip 23 &as 'marker_27'
  
    &onClipEvent &construct
      &pushs 'm_uiName'
      &pushssv 'Scenario_27'
    &end
  &end // of placeMovieClip 23

  &placeMovieClip 23 &as 'marker_33'
  
    &onClipEvent &construct
      &pushs 'm_uiName'
      &pushssv 'Scenario_33'
    &end
  &end // of placeMovieClip 23

  &placeMovieClip 23 &as 'marker_24'
  
    &onClipEvent &construct
      &pushs 'm_uiName'
      &pushssv 'Scenario_24'
    &end
  &end // of placeMovieClip 23

  &placeMovieClip 23 &as 'marker_34'
  
    &onClipEvent &construct
      &pushs 'm_uiName'
      &pushssv 'Scenario_34'
    &end
  &end // of placeMovieClip 23

  &placeMovieClip 23 &as 'marker_39'
  
    &onClipEvent &construct
      &pushs 'm_uiName'
      &pushssv 'Scenario_39'
    &end
  &end // of placeMovieClip 23

  &placeMovieClip 23 &as 'marker_38'
  
    &onClipEvent &construct
      &pushs 'm_uiName'
      &pushssv 'Scenario_38'
    &end
  &end // of placeMovieClip 23

  &placeMovieClip 23 &as 'marker_43'
  
    &onClipEvent &construct
      &pushs 'm_uiName'
      &pushssv 'Scenario_43'
    &end
  &end // of placeMovieClip 23

  &placeMovieClip 23 &as 'marker_48'
  
    &onClipEvent &construct
      &pushs 'm_uiName'
      &pushssv 'Scenario_48'
    &end
  &end // of placeMovieClip 23

  &placeMovieClip 23 &as 'marker_5'
  
    &onClipEvent &construct
      &pushs 'm_uiName'
      &pushssv 'Scenario_05'
    &end
  &end // of placeMovieClip 23

  &placeMovieClip 23 &as 'marker_4'
  
    &onClipEvent &construct
      &pushs 'm_uiName'
      &pushssv 'Scenario_04'
    &end
  &end // of placeMovieClip 23

  &placeMovieClip 23 &as 'marker_14'
  
    &onClipEvent &construct
      &pushs 'm_uiName'
      &pushssv 'Scenario_14'
    &end
  &end // of placeMovieClip 23

  &placeMovieClip 23 &as 'marker_9'
  
    &onClipEvent &construct
      &pushs 'm_uiName'
      &pushssv 'Scenario_09'
    &end
  &end // of placeMovieClip 23

  &placeMovieClip 23 &as 'marker_6'
  
    &onClipEvent &construct
      &pushs 'm_uiName'
      &pushssv 'Scenario_06'
    &end
  &end // of placeMovieClip 23

  &placeMovieClip 23 &as 'marker_23'
  
    &onClipEvent &construct
      &pushs 'm_uiName'
      &pushssv 'Scenario_23'
    &end
  &end // of placeMovieClip 23

  &placeMovieClip 23 &as 'marker_20'
  
    &onClipEvent &construct
      &pushs 'm_uiName'
      &pushssv 'Scenario_20'
    &end
  &end // of placeMovieClip 23

  &placeMovieClip 23 &as 'marker_21'
  
    &onClipEvent &construct
      &pushs 'm_uiName'
      &pushssv 'Scenario_21'
    &end
  &end // of placeMovieClip 23

  &placeMovieClip 23 &as 'marker_22'
  
    &onClipEvent &construct
      &pushs 'm_uiName'
      &pushssv 'Scenario_22'
    &end
  &end // of placeMovieClip 23

  &placeMovieClip 23 &as 'marker_25'
  
    &onClipEvent &construct
      &pushs 'm_uiName'
      &pushssv 'Scenario_25'
    &end
  &end // of placeMovieClip 23

  &placeMovieClip 23 &as 'marker_31'
  
    &onClipEvent &construct
      &pushs 'm_uiName'
      &pushssv 'Scenario_31'
    &end
  &end // of placeMovieClip 23

  &placeMovieClip 23 &as 'marker_40'
  
    &onClipEvent &construct
      &pushs 'm_uiName'
      &pushssv 'Scenario_40'
    &end
  &end // of placeMovieClip 23

  &placeMovieClip 23 &as 'marker_36'
  
    &onClipEvent &construct
      &pushs 'm_uiName'
      &pushssv 'Scenario_36'
    &end
  &end // of placeMovieClip 23

  &placeMovieClip 23 &as 'marker_50'
  
    &onClipEvent &construct
      &pushs 'm_uiName'
      &pushssv 'Scenario_50'
    &end
  &end // of placeMovieClip 23

  &placeMovieClip 23 &as 'marker_47'
  
    &onClipEvent &construct
      &pushs 'm_uiName'
      &pushssv 'Scenario_47'
    &end
  &end // of placeMovieClip 23

  &placeMovieClip 23 &as 'marker_35'
  
    &onClipEvent &construct
      &pushs 'm_uiName'
      &pushssv 'Scenario_35'
    &end
  &end // of placeMovieClip 23

  &placeMovieClip 23 &as 'marker_44'
  
    &onClipEvent &construct
      &pushs 'm_uiName'
      &pushssv 'Scenario_44'
    &end
  &end // of placeMovieClip 23

  &placeMovieClip 23 &as 'marker_46'
  
    &onClipEvent &construct
      &pushs 'm_uiName'
      &pushssv 'Scenario_46'
    &end
  &end // of placeMovieClip 23

  &placeMovieClip 23 &as 'marker_45'
  
    &onClipEvent &construct
      &pushs 'm_uiName'
      &pushssv 'Scenario_45'
    &end
  &end // of placeMovieClip 23

  &placeMovieClip 23 &as 'marker_18'
  
    &onClipEvent &construct
      &pushs 'm_uiName'
      &pushssv 'Scenario_18'
    &end
  &end // of placeMovieClip 23

  &placeMovieClip 23 &as 'marker_42'
  
    &onClipEvent &construct
      &pushs 'm_uiName'
      &pushssv 'Scenario_42'
    &end
  &end // of placeMovieClip 23

  &placeMovieClip 23 &as 'marker_41'
  
    &onClipEvent &construct
      &pushs 'm_uiName'
      &pushssv 'Scenario_41'
    &end
  &end // of placeMovieClip 23

  &placeMovieClip 23 &as 'marker_10'
  
    &onClipEvent &construct
      &pushs 'm_uiName'
      &pushssv 'Scenario_10'
    &end
  &end // of placeMovieClip 23

  &placeMovieClip 23 &as 'marker_12'
  
    &onClipEvent &construct
      &pushs 'm_uiName'
      &pushssv 'Scenario_12'
    &end
  &end // of placeMovieClip 23

  &placeMovieClip 23 &as 'marker_49'
  
    &onClipEvent &construct
      &pushs 'm_uiName'
      &pushssv 'Scenario_49'
    &end
  &end // of placeMovieClip 23

  &placeMovieClip 23 &as 'marker_19'
  
    &onClipEvent &construct
      &pushs 'm_uiName'
      &pushssv 'Scenario_19'
    &end
  &end // of placeMovieClip 23

  &placeMovieClip 23 &as 'marker_29'
  
    &onClipEvent &construct
      &pushs 'm_uiName'
      &pushssv 'Scenario_29'
    &end
  &end // of placeMovieClip 23

  &placeMovieClip 23 &as 'marker_26'
  
    &onClipEvent &construct
      &pushs 'm_uiName'
      &pushssv 'Scenario_26'
    &end
  &end // of placeMovieClip 23

  &placeMovieClip 23 &as 'marker_30'
  
    &onClipEvent &construct
      &pushs 'm_uiName'
      &pushssv 'Scenario_30'
    &end
  &end // of placeMovieClip 23

  &placeMovieClip 23 &as 'marker_37'
  
    &onClipEvent &construct
      &pushs 'm_uiName'
      &pushssv 'Scenario_37'
    &end
  &end // of placeMovieClip 23

  &placeMovieClip 23 &as 'marker_15'
  
    &onClipEvent &construct
      &pushs 'm_uiName'
      &pushssv 'Scenario_15'
    &end
  &end // of placeMovieClip 23

  &placeMovieClip 23 &as 'marker_8'
  
    &onClipEvent &construct
      &pushs 'm_uiName'
      &pushssv 'Scenario_08'
    &end
  &end // of placeMovieClip 23

  &defineMovieClip 27 // total frames: 3

    &frame 0
      &constants 'this', 'toptext', 'autoSize', 'droptext', 'right', 'text', 'm_text', '&Outline', 'tf', 'TextFormat', 'color', 'm_color', 'setTextFormat'  
      &pushthisgv
      &pushsdbgm 1							//'toptext'
      &pushsdb 2							//'autoSize'
      &pushthisgv
      &pushsdbgm 3							//'droptext'
      &pushsdb 2							//'autoSize'
      &pushsdb 4							//'right'
      &setRegister r:0
      &setMember
      &pushregister 0
      &setMember
      &pushthisgv
      &pushsdbgm 1							//'toptext'
      &pushsdb 5							//'text'
      &pushthisgv
      &pushsdbgm 3							//'droptext'
      &pushsdb 5							//'text'
      &pushthisgv
      &pushsdbgm 6							//'m_text'
      &pushsdb 7							//'&Outline'
      &add
      &setRegister r:0
      &setMember
      &pushregister 0
      &setMember
      &pushsdb 8							//'tf'
      &pushzero
      &pushsdb 9							//'TextFormat'
      &new
      &varEquals
      &pushsdbgv 8							//'tf'
      &pushsdb 10							//'color'
      &pushthisgv
      &pushsdbgm 11							//'m_color'
      &setMember
      &pushsdbgv 8							//'tf'
      &pushone
      &pushthisgv
      &pushsdbgm 1							//'toptext'
      &dcallmp 12							// setTextFormat()
      &stop
    &end // of frame 0

    &frame 1
      &constants 't', 'this', 'toptext', 'text', '', 'droptext'  
      &pushsdb 0							//'t'
      &pushthisgv
      &pushsdbgm 2							//'toptext'
      &pushsdbgm 3							//'text'
      &varEquals
      &pushthisgv
      &pushsdbgm 2							//'toptext'
      &pushsdb 3							//'text'
      &pushsdb 4							//''
      &setMember
      &pushthisgv
      &pushsdbgm 2							//'toptext'
      &pushsdb 3							//'text'
      &pushsdbgv 0							//'t'
      &setMember
      &pushsdb 0							//'t'
      &pushthisgv
      &pushsdbgm 5							//'droptext'
      &pushsdbgm 3							//'text'
      &setVariable
      &pushthisgv
      &pushsdbgm 5							//'droptext'
      &pushsdb 3							//'text'
      &pushsdb 4							//''
      &setMember
      &pushthisgv
      &pushsdbgm 5							//'droptext'
      &pushsdb 3							//'text'
      &pushsdbgv 0							//'t'
      &setMember
    &end // of frame 1

    &frame 2
      &stop
    &end // of frame 2
  &end // of defineMovieClip 27

  &defineMovieClip 30 // total frames: 3

    &frame 0
      &constants 'this', 'toptext', 'autoSize', 'droptext', 'left', 'text', 'm_text', '&Outline', 'tf', 'TextFormat', 'color', 'm_color', 'setTextFormat'  
      &pushthisgv
      &pushsdbgm 1							//'toptext'
      &pushsdb 2							//'autoSize'
      &pushthisgv
      &pushsdbgm 3							//'droptext'
      &pushsdb 2							//'autoSize'
      &pushsdb 4							//'left'
      &setRegister r:0
      &setMember
      &pushregister 0
      &setMember
      &pushthisgv
      &pushsdbgm 1							//'toptext'
      &pushsdb 5							//'text'
      &pushthisgv
      &pushsdbgm 3							//'droptext'
      &pushsdb 5							//'text'
      &pushthisgv
      &pushsdbgm 6							//'m_text'
      &pushsdb 7							//'&Outline'
      &add
      &setRegister r:0
      &setMember
      &pushregister 0
      &setMember
      &pushsdb 8							//'tf'
      &pushzero
      &pushsdb 9							//'TextFormat'
      &new
      &varEquals
      &pushsdbgv 8							//'tf'
      &pushsdb 10							//'color'
      &pushthisgv
      &pushsdbgm 11							//'m_color'
      &setMember
      &pushsdbgv 8							//'tf'
      &pushone
      &pushthisgv
      &pushsdbgm 1							//'toptext'
      &dcallmp 12							// setTextFormat()
      &stop
    &end // of frame 0

    &frame 1
      &constants 't', 'this', 'toptext', 'text', '', 'droptext'  
      &pushsdb 0							//'t'
      &pushthisgv
      &pushsdbgm 2							//'toptext'
      &pushsdbgm 3							//'text'
      &varEquals
      &pushthisgv
      &pushsdbgm 2							//'toptext'
      &pushsdb 3							//'text'
      &pushsdb 4							//''
      &setMember
      &pushthisgv
      &pushsdbgm 2							//'toptext'
      &pushsdb 3							//'text'
      &pushsdbgv 0							//'t'
      &setMember
      &pushsdb 0							//'t'
      &pushthisgv
      &pushsdbgm 5							//'droptext'
      &pushsdbgm 3							//'text'
      &setVariable
      &pushthisgv
      &pushsdbgm 5							//'droptext'
      &pushsdb 3							//'text'
      &pushsdb 4							//''
      &setMember
      &pushthisgv
      &pushsdbgm 5							//'droptext'
      &pushsdb 3							//'text'
      &pushsdbgv 0							//'t'
      &setMember
    &end // of frame 1

    &frame 2
      &stop
    &end // of frame 2
  &end // of defineMovieClip 30

  &defineMovieClip 31 // total frames: 1

    &placeMovieClip 27 &as 'left'
    
      &onClipEvent &construct
        &pushs 'm_text'
        &pushssv ''
        &pushs 'm_color'
        &pushlong 16777215
        &setVariable
      &end
    &end // of placeMovieClip 27

    &placeMovieClip 30 &as 'right'
    
      &onClipEvent &construct
        &pushs 'm_text'
        &pushssv ''
        &pushs 'm_color'
        &pushlong 16777215
        &setVariable
      &end
    &end // of placeMovieClip 30
  &end // of defineMovieClip 31

  &defineMovieClip 35 // total frames: 5

    &frame 4
      &stop
    &end // of frame 4
  &end // of defineMovieClip 35

  &defineMovieClip 36 // total frames: 1

    &placeMovieClip 35 &as 'renderimage'
    
      &onClipEvent &construct
        &pushs '_type'
        &pushssv 'RenderImage'
        &pushs '_imageMap'
        &pushssv 'ScenarioUnitPreview'
        &pushs '_mode'
        &pushssv 'ALPHA'
      &end
    &end // of placeMovieClip 35
  &end // of defineMovieClip 36

  &defineMovieClip 37 // total frames: 1

    &placeMovieClip 30 
    
      &onClipEvent &construct
        &pushs 'm_text'
        &pushssv '$APT:SCENARIO_MAP_PAR_TIME'
        &pushs 'm_color'
        &pushlong 12258816
        &setVariable
      &end
    &end // of placeMovieClip 30

    &placeMovieClip 30 
    
      &onClipEvent &construct
        &pushs 'm_text'
        &pushssv '$APT:SCENARIO_MAP_RECORD_TIME'
        &pushs 'm_color'
        &pushlong 12258816
        &setVariable
      &end
    &end // of placeMovieClip 30

    &placeMovieClip 30 &as 'name'
    
      &onClipEvent &construct
        &pushs 'm_text'
        &pushssv ''
        &pushs 'm_color'
        &pushlong 16036194
        &setVariable
      &end
    &end // of placeMovieClip 30

    &placeMovieClip 30 &as 'bestTime'
    
      &onClipEvent &construct
        &pushs 'm_text'
        &pushssv ''
        &pushs 'm_color'
        &pushlong 16036194
        &setVariable
      &end
    &end // of placeMovieClip 30

    &placeMovieClip 30 &as 'parTime'
    
      &onClipEvent &construct
        &pushs 'm_text'
        &pushssv ''
        &pushs 'm_color'
        &pushlong 16036194
        &setVariable
      &end
    &end // of placeMovieClip 30

    &placeMovieClip 27 
    
      &onClipEvent &construct
        &pushs 'm_text'
        &pushssv '$APT:SCENARIO_MAP_UNIT_UNLOCK'
        &pushs 'm_color'
        &pushlong 12258816
        &setVariable
      &end
    &end // of placeMovieClip 27

    &placeMovieClip 27 &as 'unitName'
    
      &onClipEvent &construct
        &pushs 'm_text'
        &pushssv ''
        &pushs 'm_color'
        &pushlong 16036194
        &setVariable
      &end
    &end // of placeMovieClip 27
  &end // of defineMovieClip 37

  &defineMovieClip 42 // total frames: 3

    &frame 0
      &constants 'this', 'toptext', 'text', 'droptext', 'm_text', '&Outline', 'tf', 'TextFormat', 'color', 'm_color', 'setTextFormat'  
      &pushthisgv
      &pushsdbgm 1							//'toptext'
      &pushsdb 2							//'text'
      &pushthisgv
      &pushsdbgm 3							//'droptext'
      &pushsdb 2							//'text'
      &pushthisgv
      &pushsdbgm 4							//'m_text'
      &pushsdb 5							//'&Outline'
      &add
      &setRegister r:0
      &setMember
      &pushregister 0
      &setMember
      &pushsdb 6							//'tf'
      &pushzero
      &pushsdb 7							//'TextFormat'
      &new
      &varEquals
      &pushsdbgv 6							//'tf'
      &pushsdb 8							//'color'
      &pushthisgv
      &pushsdbgm 9							//'m_color'
      &setMember
      &pushsdbgv 6							//'tf'
      &pushone
      &pushthisgv
      &pushsdbgm 1							//'toptext'
      &dcallmp 10							// setTextFormat()
      &stop
    &end // of frame 0

    &frame 1
      &constants 't', 'this', 'toptext', 'text', '', 'droptext'  
      &pushsdb 0							//'t'
      &pushthisgv
      &pushsdbgm 2							//'toptext'
      &pushsdbgm 3							//'text'
      &varEquals
      &pushthisgv
      &pushsdbgm 2							//'toptext'
      &pushsdb 3							//'text'
      &pushsdb 4							//''
      &setMember
      &pushthisgv
      &pushsdbgm 2							//'toptext'
      &pushsdb 3							//'text'
      &pushsdbgv 0							//'t'
      &setMember
      &pushsdb 0							//'t'
      &pushthisgv
      &pushsdbgm 5							//'droptext'
      &pushsdbgm 3							//'text'
      &setVariable
      &pushthisgv
      &pushsdbgm 5							//'droptext'
      &pushsdb 3							//'text'
      &pushsdb 4							//''
      &setMember
      &pushthisgv
      &pushsdbgm 5							//'droptext'
      &pushsdb 3							//'text'
      &pushsdbgv 0							//'t'
      &setMember
    &end // of frame 1

    &frame 2
      &stop
    &end // of frame 2
  &end // of defineMovieClip 42

  &defineMovieClip 43 // total frames: 1

    &placeMovieClip 42 
    
      &onClipEvent &construct
        &pushs 'm_text'
        &pushssv '$APT:SCENARIO_MAP_MISSION_UNLOCKED_PLURAL'
        &pushs 'm_color'
        &pushlong 16777215
        &setVariable
      &end
    &end // of placeMovieClip 42
  &end // of defineMovieClip 43

  &defineMovieClip 44 // total frames: 1

    &placeMovieClip 42 
    
      &onClipEvent &construct
        &pushs 'm_text'
        &pushssv '$APT:SCENARIO_MAP_MISSION_UNLOCKED'
        &pushs 'm_color'
        &pushlong 16777215
        &setVariable
      &end
    &end // of placeMovieClip 42
  &end // of defineMovieClip 44

  &defineMovieClip 47 // total frames: 3

    &frame 0
      &constants 'this', 'toptext', 'text', 'droptext', 'm_text', '&Outline', 'tf', 'TextFormat', 'color', 'm_color', 'setTextFormat'  
      &pushthisgv
      &pushsdbgm 1							//'toptext'
      &pushsdb 2							//'text'
      &pushthisgv
      &pushsdbgm 3							//'droptext'
      &pushsdb 2							//'text'
      &pushthisgv
      &pushsdbgm 4							//'m_text'
      &pushsdb 5							//'&Outline'
      &add
      &setRegister r:0
      &setMember
      &pushregister 0
      &setMember
      &pushsdb 6							//'tf'
      &pushzero
      &pushsdb 7							//'TextFormat'
      &new
      &varEquals
      &pushsdbgv 6							//'tf'
      &pushsdb 8							//'color'
      &pushthisgv
      &pushsdbgm 9							//'m_color'
      &setMember
      &pushsdbgv 6							//'tf'
      &pushone
      &pushthisgv
      &pushsdbgm 1							//'toptext'
      &dcallmp 10							// setTextFormat()
      &stop
    &end // of frame 0

    &frame 1
      &constants 't', 'this', 'toptext', 'text', '', 'droptext'  
      &pushsdb 0							//'t'
      &pushthisgv
      &pushsdbgm 2							//'toptext'
      &pushsdbgm 3							//'text'
      &varEquals
      &pushthisgv
      &pushsdbgm 2							//'toptext'
      &pushsdb 3							//'text'
      &pushsdb 4							//''
      &setMember
      &pushthisgv
      &pushsdbgm 2							//'toptext'
      &pushsdb 3							//'text'
      &pushsdbgv 0							//'t'
      &setMember
      &pushsdb 0							//'t'
      &pushthisgv
      &pushsdbgm 5							//'droptext'
      &pushsdbgm 3							//'text'
      &setVariable
      &pushthisgv
      &pushsdbgm 5							//'droptext'
      &pushsdb 3							//'text'
      &pushsdb 4							//''
      &setMember
      &pushthisgv
      &pushsdbgm 5							//'droptext'
      &pushsdb 3							//'text'
      &pushsdbgv 0							//'t'
      &setMember
    &end // of frame 1

    &frame 2
      &stop
    &end // of frame 2
  &end // of defineMovieClip 47

  &defineMovieClip 48 // total frames: 1

    &placeMovieClip 47 
    
      &onClipEvent &construct
        &pushs 'm_text'
        &pushssv '$APT:SCENARIO_MAP_CONGRATS'
        &pushs 'm_color'
        &pushlong 16777215
        &setVariable
      &end
    &end // of placeMovieClip 47

    &placeMovieClip 47 
    
      &onClipEvent &construct
        &pushs 'm_text'
        &pushssv '$APT:SCENARIO_MAP_COMPLETED_UNDER_PAR'
        &pushs 'm_color'
        &pushlong 16777215
        &setVariable
      &end
    &end // of placeMovieClip 47
  &end // of defineMovieClip 48

  &defineMovieClip 49 // total frames: 1

    &placeMovieClip 47 
    
      &onClipEvent &construct
        &pushs 'm_text'
        &pushssv '$APT:SCENARIO_MAP_CONGRATS'
        &pushs 'm_color'
        &pushlong 16777215
        &setVariable
      &end
    &end // of placeMovieClip 47

    &placeMovieClip 47 
    
      &onClipEvent &construct
        &pushs 'm_text'
        &pushssv '$APT:SCENARIO_MAP_COMPLETED'
        &pushs 'm_color'
        &pushlong 16777215
        &setVariable
      &end
    &end // of placeMovieClip 47
  &end // of defineMovieClip 49

  &defineMovieClip 50 // total frames: 20

    &frame 0
      &constants 'this', 'completed', '_visible', 'completedpar', 'unlockedone', 'unlockedmany'  
      &pushthisgv
      &pushsdbgm 1							//'completed'
      &pushsdb 2							//'_visible'
      &pushtrue
      &setMember
      &pushthisgv
      &pushsdbgm 3							//'completedpar'
      &pushsdb 2							//'_visible'
      &pushthisgv
      &pushsdbgm 4							//'unlockedone'
      &pushsdb 2							//'_visible'
      &pushthisgv
      &pushsdbgm 5							//'unlockedmany'
      &pushsdb 2							//'_visible'
      &pushfalse
      &setRegister r:0
      &setMember
      &pushregister 0
      &setRegister r:0
      &setMember
      &pushregister 0
      &setMember
      &stop
    &end // of frame 0

    &frame 4
      &stop
    &end // of frame 4

    &frame 5
      &constants 'this', 'completedpar', '_visible', 'completed', 'unlockedone', 'unlockedmany'  
      &pushthisgv
      &pushsdbgm 1							//'completedpar'
      &pushsdb 2							//'_visible'
      &pushtrue
      &setMember
      &pushthisgv
      &pushsdbgm 3							//'completed'
      &pushsdb 2							//'_visible'
      &pushthisgv
      &pushsdbgm 4							//'unlockedone'
      &pushsdb 2							//'_visible'
      &pushthisgv
      &pushsdbgm 5							//'unlockedmany'
      &pushsdb 2							//'_visible'
      &pushfalse
      &setRegister r:0
      &setMember
      &pushregister 0
      &setRegister r:0
      &setMember
      &pushregister 0
      &setMember
      &stop
    &end // of frame 5

    &frame 9
      &stop
    &end // of frame 9

    &frame 10
      &constants 'this', 'unlockedone', '_visible', 'completed', 'completedpar', 'unlockedmany'  
      &pushthisgv
      &pushsdbgm 1							//'unlockedone'
      &pushsdb 2							//'_visible'
      &pushtrue
      &setMember
      &pushthisgv
      &pushsdbgm 3							//'completed'
      &pushsdb 2							//'_visible'
      &pushthisgv
      &pushsdbgm 4							//'completedpar'
      &pushsdb 2							//'_visible'
      &pushthisgv
      &pushsdbgm 5							//'unlockedmany'
      &pushsdb 2							//'_visible'
      &pushfalse
      &setRegister r:0
      &setMember
      &pushregister 0
      &setRegister r:0
      &setMember
      &pushregister 0
      &setMember
      &stop
    &end // of frame 10

    &frame 14
      &stop
    &end // of frame 14

    &frame 15
      &constants 'this', 'unlockedmany', '_visible', 'completed', 'completedpar', 'unlockedone'  
      &pushthisgv
      &pushsdbgm 1							//'unlockedmany'
      &pushsdb 2							//'_visible'
      &pushtrue
      &setMember
      &pushthisgv
      &pushsdbgm 3							//'completed'
      &pushsdb 2							//'_visible'
      &pushthisgv
      &pushsdbgm 4							//'completedpar'
      &pushsdb 2							//'_visible'
      &pushthisgv
      &pushsdbgm 5							//'unlockedone'
      &pushsdb 2							//'_visible'
      &pushfalse
      &setRegister r:0
      &setMember
      &pushregister 0
      &setRegister r:0
      &setMember
      &pushregister 0
      &setMember
      &stop
    &end // of frame 15

    &frame 19
      &stop
    &end // of frame 19
  &end // of defineMovieClip 50

  &defineMovieClip 53 // total frames: 1
  &end // of defineMovieClip 53

  &defineMovieClip 56 // total frames: 1
  &end // of defineMovieClip 56

  &defineMovieClip 59 // total frames: 1
  &end // of defineMovieClip 59

  &defineMovieClip 60 // total frames: 51

    &frame 4
      &stop
    &end // of frame 4

    &frame 45
      &stop
    &end // of frame 45
  &end // of defineMovieClip 60

  &defineMovieClip 61 // total frames: 26

    &frame 4
      &stop
    &end // of frame 4

    &frame 21
      &stop
    &end // of frame 21

    &frame 25
      &stop
    &end // of frame 25
  &end // of defineMovieClip 61

  &defineMovieClip 66 // total frames: 3

    &frame 0
      &constants 'this', 'toptext', 'autoSize', 'droptext', 'left', 'text', 'm_text', '&Outline', 'tf', 'TextFormat', 'color', 'm_color', 'setTextFormat'  
      &pushthisgv
      &pushsdbgm 1							//'toptext'
      &pushsdb 2							//'autoSize'
      &pushthisgv
      &pushsdbgm 3							//'droptext'
      &pushsdb 2							//'autoSize'
      &pushsdb 4							//'left'
      &setRegister r:0
      &setMember
      &pushregister 0
      &setMember
      &pushthisgv
      &pushsdbgm 1							//'toptext'
      &pushsdb 5							//'text'
      &pushthisgv
      &pushsdbgm 3							//'droptext'
      &pushsdb 5							//'text'
      &pushthisgv
      &pushsdbgm 6							//'m_text'
      &pushsdb 7							//'&Outline'
      &add
      &setRegister r:0
      &setMember
      &pushregister 0
      &setMember
      &pushsdb 8							//'tf'
      &pushzero
      &pushsdb 9							//'TextFormat'
      &new
      &varEquals
      &pushsdbgv 8							//'tf'
      &pushsdb 10							//'color'
      &pushthisgv
      &pushsdbgm 11							//'m_color'
      &setMember
      &pushsdbgv 8							//'tf'
      &pushone
      &pushthisgv
      &pushsdbgm 1							//'toptext'
      &dcallmp 12							// setTextFormat()
      &stop
    &end // of frame 0

    &frame 1
      &constants 't', 'this', 'toptext', 'text', '', 'droptext'  
      &pushsdb 0							//'t'
      &pushthisgv
      &pushsdbgm 2							//'toptext'
      &pushsdbgm 3							//'text'
      &varEquals
      &pushthisgv
      &pushsdbgm 2							//'toptext'
      &pushsdb 3							//'text'
      &pushsdb 4							//''
      &setMember
      &pushthisgv
      &pushsdbgm 2							//'toptext'
      &pushsdb 3							//'text'
      &pushsdbgv 0							//'t'
      &setMember
      &pushsdb 0							//'t'
      &pushthisgv
      &pushsdbgm 5							//'droptext'
      &pushsdbgm 3							//'text'
      &setVariable
      &pushthisgv
      &pushsdbgm 5							//'droptext'
      &pushsdb 3							//'text'
      &pushsdb 4							//''
      &setMember
      &pushthisgv
      &pushsdbgm 5							//'droptext'
      &pushsdb 3							//'text'
      &pushsdbgv 0							//'t'
      &setMember
    &end // of frame 1

    &frame 2
      &stop
    &end // of frame 2
  &end // of defineMovieClip 66

  &defineMovieClip 69 // total frames: 3

    &frame 0
      &constants 'this', 'toptext', 'autoSize', 'droptext', 'right', 'text', 'm_text', '&Outline', 'tf', 'TextFormat', 'color', 'm_color', 'setTextFormat'  
      &pushthisgv
      &pushsdbgm 1							//'toptext'
      &pushsdb 2							//'autoSize'
      &pushthisgv
      &pushsdbgm 3							//'droptext'
      &pushsdb 2							//'autoSize'
      &pushsdb 4							//'right'
      &setRegister r:0
      &setMember
      &pushregister 0
      &setMember
      &pushthisgv
      &pushsdbgm 1							//'toptext'
      &pushsdb 5							//'text'
      &pushthisgv
      &pushsdbgm 3							//'droptext'
      &pushsdb 5							//'text'
      &pushthisgv
      &pushsdbgm 6							//'m_text'
      &pushsdb 7							//'&Outline'
      &add
      &setRegister r:0
      &setMember
      &pushregister 0
      &setMember
      &pushsdb 8							//'tf'
      &pushzero
      &pushsdb 9							//'TextFormat'
      &new
      &varEquals
      &pushsdbgv 8							//'tf'
      &pushsdb 10							//'color'
      &pushthisgv
      &pushsdbgm 11							//'m_color'
      &setMember
      &pushsdbgv 8							//'tf'
      &pushone
      &pushthisgv
      &pushsdbgm 1							//'toptext'
      &dcallmp 12							// setTextFormat()
      &stop
    &end // of frame 0

    &frame 1
      &constants 't', 'this', 'toptext', 'text', '', 'droptext'  
      &pushsdb 0							//'t'
      &pushthisgv
      &pushsdbgm 2							//'toptext'
      &pushsdbgm 3							//'text'
      &varEquals
      &pushthisgv
      &pushsdbgm 2							//'toptext'
      &pushsdb 3							//'text'
      &pushsdb 4							//''
      &setMember
      &pushthisgv
      &pushsdbgm 2							//'toptext'
      &pushsdb 3							//'text'
      &pushsdbgv 0							//'t'
      &setMember
      &pushsdb 0							//'t'
      &pushthisgv
      &pushsdbgm 5							//'droptext'
      &pushsdbgm 3							//'text'
      &setVariable
      &pushthisgv
      &pushsdbgm 5							//'droptext'
      &pushsdb 3							//'text'
      &pushsdb 4							//''
      &setMember
      &pushthisgv
      &pushsdbgm 5							//'droptext'
      &pushsdb 3							//'text'
      &pushsdbgv 0							//'t'
      &setMember
    &end // of frame 1

    &frame 2
      &stop
    &end // of frame 2
  &end // of defineMovieClip 69

  &defineMovieClip 72 // total frames: 1
  &end // of defineMovieClip 72

  &defineMovieClip 75 // total frames: 1
  &end // of defineMovieClip 75

  &defineMovieClip 76 // total frames: 30

    &frame 0
      &stop
    &end // of frame 0

    &frame 29
      &pushzero
      &pushthisgv
      &pushs 'doTextReveal'
      &callmp
      &stop
    &end // of frame 29
  &end // of defineMovieClip 76

  &defineMovieClip 79 // total frames: 1
  &end // of defineMovieClip 79

  &defineMovieClip 80 // total frames: 1
  &end // of defineMovieClip 80

  &defineMovieClip 81 // total frames: 1

    &placeMovieClip 66 
    
      &onClipEvent &construct
        &pushs 'm_text'
        &pushssv '$APT:SCENARIO_MAP_TOTAL_TIME'
        &pushs 'm_color'
        &pushlong 12258816
        &setVariable
      &end
    &end // of placeMovieClip 66

    &placeMovieClip 66 
    
      &onClipEvent &construct
        &pushs 'm_text'
        &pushssv '$APT:SCENARIO_MAP_TOTAL_MONEY'
        &pushs 'm_color'
        &pushlong 12258816
        &setVariable
      &end
    &end // of placeMovieClip 66

    &placeMovieClip 69 &as 'time'
    
      &onClipEvent &construct
        &pushs 'm_text'
        &pushssv ''
        &pushs 'm_color'
        &pushlong 16036194
        &setVariable
      &end
    &end // of placeMovieClip 69

    &placeMovieClip 69 &as 'money'
    
      &onClipEvent &construct
        &pushs 'm_text'
        &pushssv ''
        &pushs 'm_color'
        &pushlong 16036194
        &setVariable
      &end
    &end // of placeMovieClip 69

    &placeMovieClip 69 &as 'pct'
    
      &onClipEvent &construct
        &pushs 'm_text'
        &pushssv ''
        &pushs 'm_color'
        &pushlong 16036194
        &setVariable
      &end
    &end // of placeMovieClip 69
  &end // of defineMovieClip 81

  &defineMovieClip 86 // total frames: 1
  &end // of defineMovieClip 86

  &defineMovieClip 89 // total frames: 1
  &end // of defineMovieClip 89
  
  &importAssets &from '../cafe/mouseComponents/std_mouseScrollingListbox.swf'
    'std_mouseScrollingListboxSymbol' &as 90
  &end // of importAssets
  
  &importAssets &from '../cafe/mouseComponents/std_mouseScrollbar.swf'
    'std_mouseScrollbarSymbol' &as 91
  &end // of importAssets

  &defineMovieClip 102 // total frames: 32

    &frame 7
      &stop
    &end // of frame 7

    &frame 15
      &stop
    &end // of frame 15

    &frame 23
      &stop
    &end // of frame 23

    &frame 31
      &stop
    &end // of frame 31
  &end // of defineMovieClip 102

  &defineMovieClip 103 // total frames: 1

    &frame 0
      &constants '_frame_', 'this', 'm_type', 'symbol', 'gotoAndPlay', 'tf', 'autoSize', 'text', '$APT:WORLDMAP_LEGEND_', '&Outline', '_y', '_height'  
      &pushsdb 0							//'_frame_'
      &pushthisgv
      &pushsdbgm 2							//'m_type'
      &toString
      &add
      &pushone
      &pushthisgv
      &pushsdbgm 3							//'symbol'
      &dcallmp 4							// gotoAndPlay()
      &pushthisgv
      &pushsdbgm 5							//'tf'
      &pushsdb 6							//'autoSize'
      &pushtrue
      &setMember
      &pushthisgv
      &pushsdbgm 5							//'tf'
      &pushsdb 7							//'text'
      &pushsdb 8							//'$APT:WORLDMAP_LEGEND_'
      &pushthisgv
      &pushsdbgm 2							//'m_type'
      &toString
      &add
      &pushsdb 9							//'&Outline'
      &add
      &setMember
      &pushthisgv
      &pushsdbgm 5							//'tf'
      &pushsdb 10							//'_y'
      &pushthisgv
      &pushsdbgm 5							//'tf'
      &pushsdbgm 10							//'_y'
      &pushthisgv
      &pushsdbgm 5							//'tf'
      &pushsdbgm 11							//'_height'
      &pushfloat 0.500000000000000
      &multiply
      &subtract
      &setMember
    &end // of frame 0
  &end // of defineMovieClip 103

  &defineMovieClip 104 // total frames: 1

    &placeMovieClip 90 &as 'listbox'
    
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
        &pushbyte 10
        &setVariable
        &pushs 'm_initiallySelected'
        &pushfalse
        &setVariable
        &pushs 'm_BgVisible'
        &pushfalse
        &setVariable
        &pushs 'm_renderWidth'
        &pushshort 686
        &setVariable
        &pushs 'm_renderHeight'
        &pushshort 339
        &setVariable
        &pushs 'm_backgroundMargin'
        &pushbyte 2
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
    &end // of placeMovieClip 90

    &placeMovieClip 91 &as 'scrollbar'
    
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
    &end // of placeMovieClip 91

    &placeMovieClip 103 
    
      &onClipEvent &construct
        &pushs 'm_type'
        &pushssv 'NEW_MAIN'
      &end
    &end // of placeMovieClip 103

    &placeMovieClip 103 
    
      &onClipEvent &construct
        &pushs 'm_type'
        &pushssv 'NEW_SIDE'
      &end
    &end // of placeMovieClip 103

    &placeMovieClip 103 
    
      &onClipEvent &construct
        &pushs 'm_type'
        &pushssv 'STAR_PLAIN'
      &end
    &end // of placeMovieClip 103

    &placeMovieClip 103 
    
      &onClipEvent &construct
        &pushs 'm_type'
        &pushssv 'STAR_PAR'
      &end
    &end // of placeMovieClip 103

    &placeMovieClip 14 &as 'btnClose'
    
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
        &pushbyte -1
        &setVariable
        &pushs 'm_bVisible'
        &pushtrue
        &setVariable
        &pushs 'm_label'
        &pushssv '$CLOSE'
        &pushs 'm_contentSymbol'
        &pushssv 'buttonContentSymbol'
      &end
    &end // of placeMovieClip 14
  &end // of defineMovieClip 104

  &defineMovieClip 105 // total frames: 1
  &end // of defineMovieClip 105

  &defineMovieClip 106 // total frames: 34

    &frame 0
      &pushthisgv
      &pushs '_visible'
      &pushtrue
      &setMember
      &stop
    &end // of frame 0

    &frame 29
      &pushzero
      &pushthisgv
      &pushs 'onFadeComplete'
      &callmp
    &end // of frame 29

    &frame 30
      &pushthisgv
      &pushs '_visible'
      &pushfalse
      &setMember
      &stop
    &end // of frame 30
  &end // of defineMovieClip 106

  &defineMovieClip 107 // total frames: 0
  &end // of defineMovieClip 107
  
  &exportAssets
    107 &as '__Packages.Cafe2_BaseUIScreen'
  &end // of exportAssets
  
  &initMovieClip 107
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
  &end // of initMovieClip 107

  &defineMovieClip 108 // total frames: 0
  &end // of defineMovieClip 108
  
  &exportAssets
    108 &as '__Packages.fes_m_scenarioMap'
  &end // of exportAssets
  
  &initMovieClip 108
    &constants '_global', 'fes_m_scenarioMap', 'Cafe2_BaseUIScreen', 'm_thisClass', 'm_localEventHandlerIndex', 'm_localMessageHandlerFunc', 'm_localEventHandlerFunc', 'm_scenarioProgressEffectIndex', 'Object', 'QueryGameEngine?SCENARIO_INTRO', 'm_isFirstRun', 'SCENARIO_INTRO', '', 'FSCommand:CallGameFunction', '%PlayMovieAtomic?movie=', '|allowCancel=0', '_hide', '_show', 'm_screen', 'helplayer', 'gotoAndPlay', 'helpmc', 'bgimage', '_visible', 'prototype', 'setup', 'localMessageHandler', 'bind1DynamicParams', 'gMH', 'addMessageHandler', 'localEventHandler', 'bind2DynamicParams', 'gEH', 'addLocalEventHandler', 'onScreenExit', 'bind0', 'gSM', 'setOnExitScreen', 'cleanup', 'removeMessageHandler', 'removeLocalEventHandler', 'clearData', 'frameDelayedInit', 'initMarkers', 'initGUI', 'updateProgress', 'updateScenarios', 'beginIconReveal', '$APT:CONTINUE', 'dialog', 'btnClose', 'setText', 'm_iconRevealState', 'IRSTATE_BEGIN', 'onEnterFrame', 'updateIconRevealState', 'IRSTATE_DELAY', 'IRSTATE_CUE_CHANGED_TEXT_REVEAL', 'IRSTATE_CUEMSG_COMPLETED', 'IRSTATE_SHOWICONS_COMPLETED', 'IRSTATE_CUEMSG_UNLOCKED', 'IRSTATE_UNLOCK_DELAY', 'IRSTATE_SHOWICONS_UNLOCKED', 'IRSTATE_ABORT', 'IRSTATE_END', 'QueryGameEngine?SCENARIO_MOST_RECENT_MISSION', 'm_iconsCompleted', 'Array', 'm_iconsUnlocked', 'm_initialIconSelected', 'SCENARIO_MOST_RECENT_MISSION', 'm_markers', 'isLocked', 'm_scenarioIds', 'length', 'isNewlyCompleted', 'push', 'preVisualize', 'isNewlyUnlocked', 'applyPreset', 'reorderUnlockedIcons', '_textinfo', 'lowerpanel', 'textinfo', 'setCursorPosition', 'blocker', 'FSCommand:MouseSetVisibility', 'm_changedTextFields', 'enterDelayState', 'm_iconRevealDelay', 'm_postDelayState', 'MovieClip', 'shift', '_reveal', 'revealEffectMC', 'GuiScenarioProgressUpdateEffect', 'playSound', 'MAX_SCENARIO_PROGRESS_EFFECT', 'isUnderPar', '_completed', '_completedpar', 'textinfotext', 'GuiScenarioMessageBoxWipe', '*** ERROR in fes_m_scenarioMap *** - Scenario marked as complete cannt be found, scenarioId is ', 'GuiScenarioCompleteUnderParEffect', 'GuiScenarioCompleteEffect', '_unlockedmany', '_unlockedone', 'unlockNextIcon', 'pendingString', 'updateDynTextClip', 'onMarkerSelected', '_fade', 'IRSTATE_NONE', 'onBackClicked', 'btnBack', 'initGUIButton', 'onHelpClicked', 'btnHelp', 'onUnlocksClicked', 'btnUnlocks', 'onContinueClicked', 'btnContinue', 'setGlowing', 'onFadeComplete', 'onHelpFadeOutComplete', 'onCloseHelpClicked', 'scrollbar', 'enable', 'listbox', 'disableEntryHitRegion', 'symbol', 'std_mouseListboxEntryTextElementSymbol', 'width', 'leftMargin', 'rightMargin', 'fontSize', 'useHighlights', 'isSelectable', 'useHitRegion', 'autoSize', 'color', 'init', 'scrollBar', 'attachScrollbarController', '$APT:WORLDMAP_HELP_TEXT_01', 'setColumnData', 'refreshDisplay', 'moveViewToFirst', 'onPress', 'onBlockerClicked', 'extern', 'IsDevelopment', '1', 'onUnlockAll', 'btnDEBUGUNLOCK', 'onCompleteMission', 'btnDEBUGCOMPLETE', 'disable', 'SOUND', 'SCENARIO_WORLD_MAP', 'PlayShellMusic', 'setOnMouseUpFunction', 'show', 'noIntro', 'cursor', '_x', '_y', 'crosshairv', 'crosshairh', 'texttip', '_zoom', 'GuiScenarioIconSelect', 'm_selectedScenarioId', 'updateScenarioInfoBox', 'this', 'lp', 'marker', '.marker_', 'getUiName', 'onMarkerMouseEnter', 'setMouseEnterCallback', 'onMarkerMouseExit', 'setMouseExitCallback', 'setSelectedCallback', 'onUnlockAnimComplete', 'setUnlockAnimCallback', '*** ERROR in fes_m_scenarioMap *** - Found marker clip with no UI name property in MC ', 'updateTextTip', 'GuiScenarioRevealEffect', 'QueryGameEngine?SCENARIO_CURRENT_STATUS', 'SCENARIO_CURRENT_STATUS_PERCENTAGE_FORMER', '$', 'progressinfo', 'pct', 'SCENARIO_CURRENT_STATUS_PERCENTAGE', 'addChangedTextField', 'SCENARIO_CURRENT_STATUS_TOTALTIME_FORMER', 'time', 'SCENARIO_CURRENT_STATUS_TOTALTIME', 'SCENARIO_CURRENT_STATUS_TOTALMONEY_FORMER', 'money', 'SCENARIO_CURRENT_STATUS_TOTALMONEY', 'pct_effect', 'positionRevealEffectElement', 'time_effect', 'money_effect', 'SCENARIO_CURRENT_STATUS_PROGRESS', 'Math', 'max', 'min', 'progressbar', 'fg', '_width', 'bg', 'doTextReveal', 'onDoTextReveal', 'bind1', 'toptext', 'TEXT_REVEAL_EFFECT_MARGIN', 'textWidth', 'QueryGameEngine?SCENARIO_MISSION_IDS', ',', 'SCENARIO_MISSION_IDS', 'split', 'm_scenarioNames', 'm_scenarioShortNames', 'm_scenarioParTimes', 'm_scenarioBestTimes', 'm_scenarioUnitNames', 'm_scenarioUnitDescs', 'QueryGameEngine?SCENARIO_MISSION_DATA?Mission=', 'SCENARIO_MISSION_DATA_NAME', 'SCENARIO_MISSION_DATA_PARTIME', 'SCENARIO_MISSION_DATA_BESTTIME', 'SCENARIO_MISSION_DATA_UNITNAME', 'SCENARIO_MISSION_DATA_UNITDESC', 'SCENARIO_MISSION_DATA_STATE', 'presetState', '*** ERROR in fes_m_scenarioMap *** - Unable to find marker clip for scenario \'', '\'', 'scenario', 'name', 'bestTime', 'parTime', 'unitName', 'unitDesc', '%ScenarioSetUnitUnlockRendererScenario?Mission=', 'uniticon', 'renderimage', '_mode', 'isCompleted', 'GRAYSCALE', 'ALPHA', '_refresh', 'text', 'droptext', '&Outline', 'left', 'right', 'CURSOR_X_MIDPOINT', 'goBackScreen', '$APT:CLOSE', 'SHELL', 'SCENARIO_UNLOCK_MODE', 'ENUM', 'SCENARIO_UNLOCK_MODE_DEFAULT', 'SCREEN', 'FES_SCENARIO_UNLOCKS', 'changeMainScreen', '%ScenarioSelectMission?Mission=', 'FES_SCENARIO_GAME_SETUP', '%ScenarioCompleteMission?Mission=', '|Time=50|Efficiency=1.0', 'gFM', 'handleDefaultInputs', 'ASSetPropFlags'  
    &pushglobalgv
    &pushsdbgm 1							//'fes_m_scenarioMap'
    &not
    &not
    &jnz label103    
    &pushglobalgv
    &pushsdb 1							//'fes_m_scenarioMap'
    &function2  (r:4='screen') (r:1='this', r:2='super')
      &pushregister 4
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
      &pushsdb 7							//'m_scenarioProgressEffectIndex'
      &pushone
      &setMember
      &pushzero
      &pushsdb 8							//'Object'
      &new
      &setRegister r:3
      &pop
      &pushsdb 9							//'QueryGameEngine?SCENARIO_INTRO'
      &pushregister 3
      &loadVariables
      &pushregister 1
      &pushsdb 10							//'m_isFirstRun'
      &pushregister 3
      &pushsdbgm 11							//'SCENARIO_INTRO'
      &pushundef
      &equals
      &not
      &dup
      &not
      &jnz label1      
      &pop
      &pushregister 3
      &pushsdbgm 11							//'SCENARIO_INTRO'
      &pushsdb 12							//''
      &equals
      &not
     label1:
      &setMember
      &pushregister 1
      &pushsdbgm 10							//'m_isFirstRun'
      &not
      &jnz label2      
      &pushsdb 13							//'FSCommand:CallGameFunction'
      &pushsdb 14							//'%PlayMovieAtomic?movie='
      &pushregister 3
      &pushsdbgm 11							//'SCENARIO_INTRO'
      &add
      &pushsdb 15							//'|allowCancel=0'
      &add
      &getURL2
     label2:
      &pushregister 1
      &pushsdbgm 10							//'m_isFirstRun'
      &jnz label3      
      &pushsdb 16							//'_hide'
      &jmp label4      
     label3:
      &pushsdb 17							//'_show'
     label4:
      &pushone
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 18							//'m_screen'
      &pushsdbgm 19							//'helplayer'
      &dcallmp 20							// gotoAndPlay()
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 18							//'m_screen'
      &pushsdbgm 19							//'helplayer'
      &pushsdbgm 21							//'helpmc'
      &pushsdbgm 22							//'bgimage'
      &pushsdb 23							//'_visible'
      &pushregister 1
      &pushsdbgm 10							//'m_isFirstRun'
      &setMember
    &end // of function 

    &setRegister r:1
    &setMember
    &pushglobalgv
    &pushsdbgm 1							//'fes_m_scenarioMap'
    &pushsdbgv 2							//'Cafe2_BaseUIScreen'
    &extends
    &pushregister 1
    &pushsdbgm 24							//'prototype'
    &setRegister r:2
    &pop
    &pushregister 2
    &pushsdb 25							//'setup'
    &function2  () (r:1='this', r:2='_global')
      &pushregister 1
      &pushsdb 5							//'m_localMessageHandlerFunc'
      &pushregister 1
      &pushsdbgm 26							//'localMessageHandler'
      &pushregister 1
      &pushbyte 2
      &pushregister 2
      &pushsdb 27							//'bind1DynamicParams'
      &callMethod
      &setMember
      &pushregister 1
      &pushsdbgm 5							//'m_localMessageHandlerFunc'
      &pushone
      &pushregister 2
      &pushsdbgm 28							//'gMH'
      &dcallmp 29							// addMessageHandler()
      &pushregister 1
      &pushsdb 6							//'m_localEventHandlerFunc'
      &pushregister 1
      &pushsdbgm 30							//'localEventHandler'
      &pushregister 1
      &pushbyte 2
      &pushregister 2
      &pushsdb 31							//'bind2DynamicParams'
      &callMethod
      &setMember
      &pushregister 1
      &pushsdb 4							//'m_localEventHandlerIndex'
      &pushregister 1
      &pushsdbgm 6							//'m_localEventHandlerFunc'
      &pushone
      &pushregister 2
      &pushsdbgm 32							//'gEH'
      &pushsdb 33							//'addLocalEventHandler'
      &callMethod
      &setMember
      &pushregister 1
      &pushsdbgm 34							//'onScreenExit'
      &pushregister 1
      &pushbyte 2
      &pushregister 2
      &pushsdb 35							//'bind0'
      &callMethod
      &pushone
      &pushregister 2
      &pushsdbgm 36							//'gSM'
      &dcallmp 37							// setOnExitScreen()
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 38							//'cleanup'
    &function2  () (r:1='this', r:2='_global')
      &getURL 'FSCommand:CallGameFunction'       '%ScenarioSetUnitUnlockRendererScenario?Mission='      
      &getURL 'FSCommand:CallGameFunction'       '%ScenarioCleanupWorldMapStringTokens'      
      &pushregister 1
      &pushsdbgm 5							//'m_localMessageHandlerFunc'
      &pushundef
      &equals
      &not
      &not
      &jnz label5      
      &pushregister 1
      &pushsdbgm 5							//'m_localMessageHandlerFunc'
      &pushone
      &pushregister 2
      &pushsdbgm 28							//'gMH'
      &dcallmp 39							// removeMessageHandler()
      &pushregister 1
      &pushsdb 5							//'m_localMessageHandlerFunc'
      &delete
      &pop
      &pushregister 1
      &pushsdb 5							//'m_localMessageHandlerFunc'
      &pushundef
      &setMember
     label5:
      &pushregister 1
      &pushsdbgm 4							//'m_localEventHandlerIndex'
      &pushbyte -1
      &equals
      &not
      &not
      &jnz label6      
      &pushregister 1
      &pushsdbgm 4							//'m_localEventHandlerIndex'
      &pushone
      &pushregister 2
      &pushsdbgm 32							//'gEH'
      &dcallmp 40							// removeLocalEventHandler()
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
     label6:
      &pushzero
      &pushregister 1
      &dcallmp 41							// clearData()
      &pushregister 2
      &pushsdb 1							//'fes_m_scenarioMap'
      &delete
      &pop
      &pushregister 2
      &pushsdb 2							//'Cafe2_BaseUIScreen'
      &delete
      &pop
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 42							//'frameDelayedInit'
    &function2  () (r:1='this')
      &pushzero
      &pushregister 1
      &dcallmp 25							// setup()
      &pushzero
      &pushregister 1
      &dcallmp 43							// initMarkers()
      &pushzero
      &pushregister 1
      &dcallmp 44							// initGUI()
      &pushzero
      &pushregister 1
      &dcallmp 45							// updateProgress()
      &pushzero
      &pushregister 1
      &dcallmp 46							// updateScenarios()
      &pushregister 1
      &pushsdbgm 10							//'m_isFirstRun'
      &not
      &not
      &jnz label7      
      &pushzero
      &pushregister 1
      &dcallmp 47							// beginIconReveal()
      &jmp label8      
     label7:
      &pushsdb 48							//'$APT:CONTINUE'
      &pushone
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 18							//'m_screen'
      &pushsdbgm 19							//'helplayer'
      &pushsdbgm 21							//'helpmc'
      &pushsdbgm 49							//'dialog'
      &pushsdbgm 50							//'btnClose'
      &dcallmp 51							// setText()
     label8:
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 47							//'beginIconReveal'
    &function2  () (r:1='this', r:2='_global')
      &pushregister 1
      &pushsdb 52							//'m_iconRevealState'
      &pushsdbgv 1							//'fes_m_scenarioMap'
      &pushsdbgm 53							//'IRSTATE_BEGIN'
      &setMember
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 18							//'m_screen'
      &pushsdb 54							//'onEnterFrame'
      &pushregister 1
      &pushsdbgm 55							//'updateIconRevealState'
      &pushregister 1
      &pushbyte 2
      &pushregister 2
      &pushsdb 35							//'bind0'
      &callMethod
      &setMember
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 55							//'updateIconRevealState'
    &function2  () (r:1='this', r:2='_global')
      &pushregister 1
      &pushsdbgm 52							//'m_iconRevealState'
      &setRegister r:0
      &pushsdbgv 1							//'fes_m_scenarioMap'
      &pushsdbgm 53							//'IRSTATE_BEGIN'
      &strictEquals
      &jnz label9      
      &pushregister 0
      &pushsdbgv 1							//'fes_m_scenarioMap'
      &pushsdbgm 56							//'IRSTATE_DELAY'
      &strictEquals
      &jnz label22      
      &pushregister 0
      &pushsdbgv 1							//'fes_m_scenarioMap'
      &pushsdbgm 57							//'IRSTATE_CUE_CHANGED_TEXT_REVEAL'
      &strictEquals
      &jnz label24      
      &pushregister 0
      &pushsdbgv 1							//'fes_m_scenarioMap'
      &pushsdbgm 58							//'IRSTATE_CUEMSG_COMPLETED'
      &strictEquals
      &jnz label31      
      &pushregister 0
      &pushsdbgv 1							//'fes_m_scenarioMap'
      &pushsdbgm 59							//'IRSTATE_SHOWICONS_COMPLETED'
      &strictEquals
      &jnz label37      
      &pushregister 0
      &pushsdbgv 1							//'fes_m_scenarioMap'
      &pushsdbgm 60							//'IRSTATE_CUEMSG_UNLOCKED'
      &strictEquals
      &jnz label43      
      &pushregister 0
      &pushsdbgv 1							//'fes_m_scenarioMap'
      &pushsdbgm 61							//'IRSTATE_UNLOCK_DELAY'
      &strictEquals
      &jnz label46      
      &pushregister 0
      &pushsdbgv 1							//'fes_m_scenarioMap'
      &pushsdbgm 62							//'IRSTATE_SHOWICONS_UNLOCKED'
      &strictEquals
      &jnz label48      
      &pushregister 0
      &pushsdbgv 1							//'fes_m_scenarioMap'
      &pushsdbgm 63							//'IRSTATE_ABORT'
      &strictEquals
      &jnz label50      
      &pushregister 0
      &pushsdbgv 1							//'fes_m_scenarioMap'
      &pushsdbgm 64							//'IRSTATE_END'
      &strictEquals
      &jnz label55      
      &jmp label57      
     label9:
      &pushzero
      &pushsdb 8							//'Object'
      &new
      &setRegister r:6
      &pop
      &pushsdb 65							//'QueryGameEngine?SCENARIO_MOST_RECENT_MISSION'
      &pushregister 6
      &loadVariables
      &pushregister 1
      &pushsdb 66							//'m_iconsCompleted'
      &pushzero
      &pushsdb 67							//'Array'
      &new
      &setMember
      &pushregister 1
      &pushsdb 68							//'m_iconsUnlocked'
      &pushzero
      &pushsdb 67							//'Array'
      &new
      &setMember
      &pushregister 1
      &pushsdb 69							//'m_initialIconSelected'
      &pushregister 6
      &pushsdbgm 70							//'SCENARIO_MOST_RECENT_MISSION'
      &setMember
      &pushregister 1
      &pushsdbgm 69							//'m_initialIconSelected'
      &pushundef
      &equals
      &not
      &dup
      &not
      &jnz label10      
      &pop
      &pushregister 1
      &pushsdbgm 71							//'m_markers'
      &pushregister 1
      &pushsdbgm 69							//'m_initialIconSelected'
      &getMember
      &pushundef
      &equals
      &not
     label10:
      &dup
      &not
      &jnz label11      
      &pop
      &pushzero
      &pushregister 1
      &pushsdbgm 71							//'m_markers'
      &pushregister 1
      &pushsdbgm 69							//'m_initialIconSelected'
      &getMember
      &pushsdb 72							//'isLocked'
      &callMethod
     label11:
      &not
      &jnz label12      
      &pushregister 1
      &pushsdb 69							//'m_initialIconSelected'
      &pushundef
      &setMember
     label12:
      &pushzero
      &setRegister r:3
      &pop
     label13:
      &pushregister 3
      &pushregister 1
      &pushsdbgm 73							//'m_scenarioIds'
      &pushsdbgm 74							//'length'
      &lessThan
      &not
      &jnz label19      
      &pushregister 1
      &pushsdbgm 73							//'m_scenarioIds'
      &pushregister 3
      &getMember
      &setRegister r:7
      &pop
      &pushregister 1
      &pushsdbgm 71							//'m_markers'
      &pushregister 7
      &getMember
      &setRegister r:4
      &pop
      &pushregister 4
      &pushundef
      &equals
      &not
      &not
      &jnz label18      
      &pushzero
      &pushregister 4
      &pushsdb 72							//'isLocked'
      &callMethod
      &not
      &dup
      &not
      &jnz label14      
      &pop
      &pushregister 1
      &pushsdbgm 69							//'m_initialIconSelected'
      &pushundef
      &equals
     label14:
      &not
      &jnz label15      
      &pushregister 1
      &pushsdb 69							//'m_initialIconSelected'
      &pushregister 7
      &setMember
     label15:
      &pushzero
      &pushregister 4
      &pushsdb 75							//'isNewlyCompleted'
      &callMethod
      &not
      &jnz label16      
      &pushregister 7
      &toString
      &pushone
      &pushregister 1
      &pushsdbgm 66							//'m_iconsCompleted'
      &dcallmp 76							// push()
      &pushzero
      &pushregister 4
      &dcallmp 77							// preVisualize()
      &jmp label18      
     label16:
      &pushzero
      &pushregister 4
      &pushsdb 78							//'isNewlyUnlocked'
      &callMethod
      &not
      &jnz label17      
      &pushregister 7
      &toString
      &pushone
      &pushregister 1
      &pushsdbgm 68							//'m_iconsUnlocked'
      &dcallmp 76							// push()
      &jmp label18      
     label17:
      &pushzero
      &pushregister 4
      &dcallmp 79							// applyPreset()
     label18:
      &pushregister 3
      &increment
      &setRegister r:3
      &pop
      &jmp label13      
     label19:
      &pushzero
      &pushregister 1
      &dcallmp 80							// reorderUnlockedIcons()
      &pushsdb 81							//'_textinfo'
      &pushone
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 18							//'m_screen'
      &pushsdbgm 82							//'lowerpanel'
      &dcallmp 20							// gotoAndPlay()
      &pushsdb 16							//'_hide'
      &pushone
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 18							//'m_screen'
      &pushsdbgm 82							//'lowerpanel'
      &pushsdbgm 83							//'textinfo'
      &dcallmp 20							// gotoAndPlay()
      &pushzero
      &pushzero
      &pushbyte 2
      &pushregister 1
      &dcallmp 84							// setCursorPosition()
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 18							//'m_screen'
      &pushsdbgm 85							//'blocker'
      &pushsdb 23							//'_visible'
      &pushtrue
      &setMember
      &pushsdb 86							//'FSCommand:MouseSetVisibility'
      &pushfalse
      &getURL2
      &pushregister 1
      &pushsdb 10							//'m_isFirstRun'
      &pushfalse
      &setMember
      &pushsdbgv 1							//'fes_m_scenarioMap'
      &pushsdbgm 57							//'IRSTATE_CUE_CHANGED_TEXT_REVEAL'
      &pushregister 1
      &pushsdbgm 87							//'m_changedTextFields'
      &pushsdbgm 74							//'length'
      &pushzero
      &greaterThan
      &jnz label20      
      &pushone
      &jmp label21      
     label20:
      &pushbyte 10
     label21:
      &pushbyte 2
      &pushregister 1
      &dcallmp 88							// enterDelayState()
      &jmp label57      
     label22:
      &pushregister 1
      &pushsdb 89							//'m_iconRevealDelay'
      &pushregister 1
      &pushsdbgm 89							//'m_iconRevealDelay'
      &decrement
      &setRegister r:0
      &setMember
      &pushregister 0
      &pushzero
      &lessThan
      &not
      &jnz label23      
      &pushregister 1
      &pushsdb 52							//'m_iconRevealState'
      &pushregister 1
      &pushsdbgm 90							//'m_postDelayState'
      &setMember
     label23:
      &jmp label57      
     label24:
      &pushregister 1
      &pushsdbgm 87							//'m_changedTextFields'
      &pushsdbgm 74							//'length'
      &pushzero
      &greaterThan
      &not
      &jnz label26      
      &pushsdbgv 91							//'MovieClip'
      &pushzero
      &pushregister 1
      &pushsdbgm 87							//'m_changedTextFields'
      &pushsdb 92							//'shift'
      &callMethod
      &cast
      &setRegister r:5
      &pop
      &pushsdb 93							//'_reveal'
      &pushone
      &pushregister 5
      &pushsdbgm 94							//'revealEffectMC'
      &dcallmp 20							// gotoAndPlay()
      &pushsdb 95							//'GuiScenarioProgressUpdateEffect'
      &pushregister 1
      &pushsdbgm 7							//'m_scenarioProgressEffectIndex'
      &toString
      &add
      &pushone
      &pushregister 2
      &dcallmp 96							// playSound()
      &pushregister 1
      &pushsdb 7							//'m_scenarioProgressEffectIndex'
      &pushregister 1
      &pushsdbgm 7							//'m_scenarioProgressEffectIndex'
      &increment
      &setMember
      &pushregister 1
      &pushsdbgm 7							//'m_scenarioProgressEffectIndex'
      &pushsdbgv 1							//'fes_m_scenarioMap'
      &pushsdbgm 97							//'MAX_SCENARIO_PROGRESS_EFFECT'
      &greaterThan
      &not
      &jnz label25      
      &pushregister 1
      &pushsdb 7							//'m_scenarioProgressEffectIndex'
      &pushone
      &setMember
     label25:
      &pushsdbgv 1							//'fes_m_scenarioMap'
      &pushsdbgm 57							//'IRSTATE_CUE_CHANGED_TEXT_REVEAL'
      &pushbyte 20
      &pushbyte 2
      &pushregister 1
      &dcallmp 88							// enterDelayState()
      &jmp label30      
     label26:
      &pushregister 1
      &pushsdbgm 66							//'m_iconsCompleted'
      &pushsdbgm 74							//'length'
      &pushzero
      &greaterThan
      &not
      &jnz label27      
      &pushsdbgv 1							//'fes_m_scenarioMap'
      &pushsdbgm 58							//'IRSTATE_CUEMSG_COMPLETED'
      &pushbyte 5
      &pushbyte 2
      &pushregister 1
      &dcallmp 88							// enterDelayState()
      &jmp label30      
     label27:
      &pushregister 1
      &pushsdb 52							//'m_iconRevealState'
      &pushregister 1
      &pushsdbgm 68							//'m_iconsUnlocked'
      &pushsdbgm 74							//'length'
      &pushzero
      &greaterThan
      &jnz label28      
      &pushsdbgv 1							//'fes_m_scenarioMap'
      &pushsdbgm 64							//'IRSTATE_END'
      &jmp label29      
     label28:
      &pushsdbgv 1							//'fes_m_scenarioMap'
      &pushsdbgm 60							//'IRSTATE_CUEMSG_UNLOCKED'
     label29:
      &setMember
     label30:
      &jmp label57      
     label31:
      &pushregister 1
      &pushsdbgm 66							//'m_iconsCompleted'
      &pushsdbgm 74							//'length'
      &pushzero
      &greaterThan
      &not
      &jnz label36      
      &pushregister 1
      &pushsdbgm 66							//'m_iconsCompleted'
      &pushzero
      &getMember
      &setRegister r:7
      &pop
      &pushregister 1
      &pushsdbgm 71							//'m_markers'
      &pushregister 7
      &getMember
      &setRegister r:4
      &pop
      &pushregister 4
      &pushundef
      &equals
      &not
      &dup
      &not
      &jnz label32      
      &pop
      &pushzero
      &pushregister 4
      &pushsdb 75							//'isNewlyCompleted'
      &callMethod
     label32:
      &not
      &jnz label35      
      &pushsdb 93							//'_reveal'
      &pushone
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 18							//'m_screen'
      &pushsdbgm 82							//'lowerpanel'
      &pushsdbgm 83							//'textinfo'
      &dcallmp 20							// gotoAndPlay()
      &pushzero
      &pushregister 4
      &pushsdb 98							//'isUnderPar'
      &callMethod
      &jnz label33      
      &pushsdb 99							//'_completed'
      &jmp label34      
     label33:
      &pushsdb 100							//'_completedpar'
     label34:
      &pushone
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 18							//'m_screen'
      &pushsdbgm 82							//'lowerpanel'
      &pushsdbgm 83							//'textinfo'
      &pushsdbgm 101							//'textinfotext'
      &dcallmp 20							// gotoAndPlay()
      &pushsdb 102							//'GuiScenarioMessageBoxWipe'
      &pushone
      &pushregister 2
      &dcallmp 96							// playSound()
      &pushsdbgv 1							//'fes_m_scenarioMap'
      &pushsdbgm 59							//'IRSTATE_SHOWICONS_COMPLETED'
      &pushbyte 50
      &pushbyte 2
      &pushregister 1
      &dcallmp 88							// enterDelayState()
      &jmp label36      
     label35:
      &pushsdb 103							//'*** ERROR in fes_m_scenarioMap *** - Scenario marked as complete cannt be found, scenarioId is '
      &pushregister 7
      &add
      &trace
     label36:
      &jmp label57      
     label37:
      &pushregister 1
      &pushsdbgm 66							//'m_iconsCompleted'
      &pushsdbgm 74							//'length'
      &pushzero
      &greaterThan
      &not
      &jnz label40      
      &pushregister 1
      &pushsdbgm 71							//'m_markers'
      &pushzero
      &pushregister 1
      &pushsdbgm 66							//'m_iconsCompleted'
      &pushsdb 92							//'shift'
      &callMethod
      &getMember
      &setRegister r:4
      &pop
      &pushregister 4
      &pushundef
      &equals
      &not
      &not
      &jnz label39      
      &pushzero
      &pushregister 4
      &dcallmp 79							// applyPreset()
      &pushzero
      &pushregister 4
      &pushsdb 98							//'isUnderPar'
      &callMethod
      &not
      &jnz label38      
      &pushsdb 104							//'GuiScenarioCompleteUnderParEffect'
      &pushone
      &pushregister 2
      &dcallmp 96							// playSound()
      &jmp label39      
     label38:
      &pushsdb 105							//'GuiScenarioCompleteEffect'
      &pushone
      &pushregister 2
      &dcallmp 96							// playSound()
     label39:
      &jmp label37      
     label40:
      &pushregister 1
      &pushsdbgm 68							//'m_iconsUnlocked'
      &pushsdbgm 74							//'length'
      &pushzero
      &greaterThan
      &jnz label41      
      &pushsdbgv 1							//'fes_m_scenarioMap'
      &pushsdbgm 64							//'IRSTATE_END'
      &jmp label42      
     label41:
      &pushsdbgv 1							//'fes_m_scenarioMap'
      &pushsdbgm 60							//'IRSTATE_CUEMSG_UNLOCKED'
     label42:
      &pushbyte 45
      &pushbyte 2
      &pushregister 1
      &dcallmp 88							// enterDelayState()
      &jmp label57      
     label43:
      &pushsdb 93							//'_reveal'
      &pushone
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 18							//'m_screen'
      &pushsdbgm 82							//'lowerpanel'
      &pushsdbgm 83							//'textinfo'
      &dcallmp 20							// gotoAndPlay()
      &pushregister 1
      &pushsdbgm 68							//'m_iconsUnlocked'
      &pushsdbgm 74							//'length'
      &pushone
      &equals
      &jnz label44      
      &pushsdb 106							//'_unlockedmany'
      &jmp label45      
     label44:
      &pushsdb 107							//'_unlockedone'
     label45:
      &pushone
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 18							//'m_screen'
      &pushsdbgm 82							//'lowerpanel'
      &pushsdbgm 83							//'textinfo'
      &pushsdbgm 101							//'textinfotext'
      &dcallmp 20							// gotoAndPlay()
      &pushsdb 102							//'GuiScenarioMessageBoxWipe'
      &pushone
      &pushregister 2
      &dcallmp 96							// playSound()
      &pushregister 1
      &pushsdb 89							//'m_iconRevealDelay'
      &pushbyte 60
      &setMember
      &pushregister 1
      &pushsdb 52							//'m_iconRevealState'
      &pushsdbgv 1							//'fes_m_scenarioMap'
      &pushsdbgm 61							//'IRSTATE_UNLOCK_DELAY'
      &setMember
      &jmp label57      
     label46:
      &pushregister 1
      &pushsdb 89							//'m_iconRevealDelay'
      &pushregister 1
      &pushsdbgm 89							//'m_iconRevealDelay'
      &decrement
      &setRegister r:0
      &setMember
      &pushregister 0
      &pushzero
      &lessThan
      &not
      &jnz label47      
      &pushtrue
      &pushone
      &pushregister 1
      &dcallmp 108							// unlockNextIcon()
      &pushregister 1
      &pushsdb 52							//'m_iconRevealState'
      &pushsdbgv 1							//'fes_m_scenarioMap'
      &pushsdbgm 62							//'IRSTATE_SHOWICONS_UNLOCKED'
      &setMember
     label47:
      &jmp label57      
     label48:
      &pushregister 1
      &pushsdbgm 68							//'m_iconsUnlocked'
      &pushsdbgm 74							//'length'
      &pushzero
      &equals
      &not
      &jnz label49      
      &pushsdbgv 1							//'fes_m_scenarioMap'
      &pushsdbgm 64							//'IRSTATE_END'
      &pushbyte 70
      &pushbyte 2
      &pushregister 1
      &dcallmp 88							// enterDelayState()
     label49:
      &jmp label57      
     label50:
      &pushregister 1
      &pushsdbgm 66							//'m_iconsCompleted'
      &pushsdbgm 74							//'length'
      &pushzero
      &greaterThan
      &not
      &jnz label52      
      &pushregister 1
      &pushsdbgm 71							//'m_markers'
      &pushzero
      &pushregister 1
      &pushsdbgm 66							//'m_iconsCompleted'
      &pushsdb 92							//'shift'
      &callMethod
      &getMember
      &setRegister r:4
      &pop
      &pushregister 4
      &pushundef
      &equals
      &not
      &not
      &jnz label51      
      &pushzero
      &pushregister 4
      &dcallmp 79							// applyPreset()
     label51:
      &jmp label50      
     label52:
      &pushregister 1
      &pushsdbgm 68							//'m_iconsUnlocked'
      &pushsdbgm 74							//'length'
      &pushzero
      &greaterThan
      &not
      &jnz label53      
      &pushfalse
      &pushone
      &pushregister 1
      &dcallmp 108							// unlockNextIcon()
      &jmp label52      
     label53:
      &pushregister 1
      &pushsdbgm 87							//'m_changedTextFields'
      &pushsdbgm 74							//'length'
      &pushzero
      &greaterThan
      &not
      &jnz label54      
      &pushsdbgv 91							//'MovieClip'
      &pushzero
      &pushregister 1
      &pushsdbgm 87							//'m_changedTextFields'
      &pushsdb 92							//'shift'
      &callMethod
      &cast
      &setRegister r:5
      &pop
      &pushregister 5
      &pushsdbgm 109							//'pendingString'
      &pushregister 5
      &pushbyte 2
      &pushregister 1
      &dcallmp 110							// updateDynTextClip()
      &jmp label53      
     label54:
      &pushregister 1
      &pushsdb 52							//'m_iconRevealState'
      &pushsdbgv 1							//'fes_m_scenarioMap'
      &pushsdbgm 64							//'IRSTATE_END'
      &setMember
      &jmp label57      
     label55:
      &pushregister 1
      &pushsdbgm 69							//'m_initialIconSelected'
      &pushsdb 12							//''
      &equals
      &not
      &not
      &jnz label56      
      &pushregister 1
      &pushsdbgm 71							//'m_markers'
      &pushregister 1
      &pushsdbgm 69							//'m_initialIconSelected'
      &getMember
      &pushone
      &pushregister 1
      &dcallmp 111							// onMarkerSelected()
     label56:
      &pushsdb 112							//'_fade'
      &pushone
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 18							//'m_screen'
      &pushsdbgm 82							//'lowerpanel'
      &dcallmp 20							// gotoAndPlay()
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 18							//'m_screen'
      &pushsdbgm 85							//'blocker'
      &pushsdb 23							//'_visible'
      &pushfalse
      &setMember
      &pushregister 1
      &pushsdb 52							//'m_iconRevealState'
      &pushsdbgv 1							//'fes_m_scenarioMap'
      &pushsdbgm 113							//'IRSTATE_NONE'
      &setMember
      &pushsdb 86							//'FSCommand:MouseSetVisibility'
      &pushtrue
      &getURL2
      &jmp label57      
     label57:
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 88							//'enterDelayState'
    &function2  (r:3='delay', r:2='nextState') (r:1='this')
      &pushregister 1
      &pushsdb 89							//'m_iconRevealDelay'
      &pushregister 3
      &setMember
      &pushregister 1
      &pushsdb 90							//'m_postDelayState'
      &pushregister 2
      &setMember
      &pushregister 1
      &pushsdb 52							//'m_iconRevealState'
      &pushsdbgv 1							//'fes_m_scenarioMap'
      &pushsdbgm 56							//'IRSTATE_DELAY'
      &setMember
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 44							//'initGUI'
    &function2  () (r:1='this', r:2='_global')
      &pushregister 1
      &pushsdbgm 114							//'onBackClicked'
      &pushregister 1
      &pushbyte 2
      &pushregister 2
      &pushsdb 35							//'bind0'
      &callMethod
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 18							//'m_screen'
      &pushsdbgm 115							//'btnBack'
      &pushbyte 2
      &pushregister 1
      &dcallmp 116							// initGUIButton()
      &pushregister 1
      &pushsdbgm 117							//'onHelpClicked'
      &pushregister 1
      &pushbyte 2
      &pushregister 2
      &pushsdb 35							//'bind0'
      &callMethod
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 18							//'m_screen'
      &pushsdbgm 118							//'btnHelp'
      &pushbyte 2
      &pushregister 1
      &dcallmp 116							// initGUIButton()
      &pushregister 1
      &pushsdbgm 119							//'onUnlocksClicked'
      &pushregister 1
      &pushbyte 2
      &pushregister 2
      &pushsdb 35							//'bind0'
      &callMethod
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 18							//'m_screen'
      &pushsdbgm 120							//'btnUnlocks'
      &pushbyte 2
      &pushregister 1
      &dcallmp 116							// initGUIButton()
      &pushregister 1
      &pushsdbgm 121							//'onContinueClicked'
      &pushregister 1
      &pushbyte 2
      &pushregister 2
      &pushsdb 35							//'bind0'
      &callMethod
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 18							//'m_screen'
      &pushsdbgm 122							//'btnContinue'
      &pushbyte 2
      &pushregister 1
      &dcallmp 116							// initGUIButton()
      &pushregister 1
      &pushsdbgm 10							//'m_isFirstRun'
      &pushone
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 18							//'m_screen'
      &pushsdbgm 122							//'btnContinue'
      &dcallmp 123							// setGlowing()
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 18							//'m_screen'
      &pushsdbgm 19							//'helplayer'
      &pushsdb 124							//'onFadeComplete'
      &pushregister 1
      &pushsdbgm 125							//'onHelpFadeOutComplete'
      &pushregister 1
      &pushbyte 2
      &pushregister 2
      &pushsdb 35							//'bind0'
      &callMethod
      &setMember
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 18							//'m_screen'
      &pushsdbgm 19							//'helplayer'
      &pushsdbgm 21							//'helpmc'
      &pushsdbgm 49							//'dialog'
      &setRegister r:3
      &pop
      &pushregister 1
      &pushsdbgm 126							//'onCloseHelpClicked'
      &pushregister 1
      &pushbyte 2
      &pushregister 2
      &pushsdb 35							//'bind0'
      &callMethod
      &pushregister 3
      &pushsdbgm 50							//'btnClose'
      &pushbyte 2
      &pushregister 1
      &dcallmp 116							// initGUIButton()
      &pushzero
      &pushregister 3
      &pushsdbgm 127							//'scrollbar'
      &dcallmp 128							// enable()
      &pushzero
      &pushregister 3
      &pushsdbgm 129							//'listbox'
      &dcallmp 130							// disableEntryHitRegion()
      &pushsdb 131							//'symbol'
      &pushsdb 132							//'std_mouseListboxEntryTextElementSymbol'
      &pushsdb 133							//'width'
      &pushshort 655
      &pushsdb 134							//'leftMargin'
      &pushbyte 8
      &pushsdb 135							//'rightMargin'
      &pushbyte 8
      &pushsdb 136							//'fontSize'
      &pushbyte 13
      &pushsdb 137							//'useHighlights'
      &pushfalse
      &pushsdb 138							//'isSelectable'
      &pushfalse
      &pushsdb 139							//'useHitRegion'
      &pushfalse
      &pushsdb 140							//'autoSize'
      &pushtrue
      &pushsdb 141							//'color'
      &pushlong 16777215
      &pushbyte 10
      &initObject
      &pushone
      &pushsdb 67							//'Array'
      &new
      &setRegister r:5
      &pop
      &pushbyte 20
      &pushregister 5
      &pushbyte 2
      &pushregister 3
      &pushsdbgm 129							//'listbox'
      &dcallmp 142							// init()
      &pushregister 3
      &pushsdbgm 143							//'scrollBar'
      &pushone
      &pushregister 3
      &pushsdbgm 129							//'listbox'
      &dcallmp 144							// attachScrollbarController()
      &pushsdb 145							//'$APT:WORLDMAP_HELP_TEXT_01'
      &pushone
      &pushsdb 67							//'Array'
      &new
      &setRegister r:4
      &pop
      &pushregister 4
      &pushzero
      &pushbyte 2
      &pushregister 3
      &pushsdbgm 129							//'listbox'
      &dcallmp 146							// setColumnData()
      &pushzero
      &pushregister 3
      &pushsdbgm 129							//'listbox'
      &dcallmp 147							// refreshDisplay()
      &pushzero
      &pushregister 3
      &pushsdbgm 129							//'listbox'
      &dcallmp 148							// moveViewToFirst()
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 18							//'m_screen'
      &pushsdbgm 85							//'blocker'
      &pushsdb 149							//'onPress'
      &pushregister 1
      &pushsdbgm 150							//'onBlockerClicked'
      &pushregister 1
      &pushbyte 2
      &pushregister 2
      &pushsdb 35							//'bind0'
      &callMethod
      &setMember
      &pushsdbgv 151							//'extern'
      &pushsdbgm 152							//'IsDevelopment'
      &pushsdb 153							//'1'
      &equals
      &not
      &jnz label58      
      &pushregister 1
      &pushsdbgm 154							//'onUnlockAll'
      &pushregister 1
      &pushbyte 2
      &pushregister 2
      &pushsdb 35							//'bind0'
      &callMethod
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 18							//'m_screen'
      &pushsdbgm 155							//'btnDEBUGUNLOCK'
      &pushbyte 2
      &pushregister 1
      &dcallmp 116							// initGUIButton()
      &pushregister 1
      &pushsdbgm 156							//'onCompleteMission'
      &pushregister 1
      &pushbyte 2
      &pushregister 2
      &pushsdb 35							//'bind0'
      &callMethod
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 18							//'m_screen'
      &pushsdbgm 157							//'btnDEBUGCOMPLETE'
      &pushbyte 2
      &pushregister 1
      &dcallmp 116							// initGUIButton()
     label58:
      &pushzero
      &pushzero
      &pushbyte 2
      &pushregister 1
      &dcallmp 84							// setCursorPosition()
      &pushzero
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 18							//'m_screen'
      &pushsdbgm 122							//'btnContinue'
      &dcallmp 158							// disable()
      &pushregister 2
      &pushsdbgm 159							//'SOUND'
      &pushsdbgm 160							//'SCENARIO_WORLD_MAP'
      &pushone
      &pushregister 2
      &dcallmp 161							// PlayShellMusic()
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 116							//'initGUIButton'
    &function2  (r:1='button', r:2='callback') ()
      &pushregister 2
      &pushone
      &pushregister 1
      &dcallmp 162							// setOnMouseUpFunction()
      &pushzero
      &pushregister 1
      &dcallmp 128							// enable()
      &pushzero
      &pushregister 1
      &dcallmp 163							// show()
      &pushzero
      &pushregister 1
      &dcallmp 164							// noIntro()
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 84							//'setCursorPosition'
    &function2  (r:5='x', r:4='y') (r:1='this', r:2='_global')
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 18							//'m_screen'
      &pushsdbgm 165							//'cursor'
      &pushsdb 166							//'_x'
      &pushregister 5
      &setMember
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 18							//'m_screen'
      &pushsdbgm 165							//'cursor'
      &pushsdb 167							//'_y'
      &pushregister 4
      &setMember
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 18							//'m_screen'
      &pushsdbgm 168							//'crosshairv'
      &pushsdb 166							//'_x'
      &pushregister 5
      &setMember
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 18							//'m_screen'
      &pushsdbgm 169							//'crosshairh'
      &pushsdb 167							//'_y'
      &pushregister 4
      &setMember
      &pushregister 5
      &pushzero
      &greaterThan
      &dup
      &not
      &jnz label59      
      &pop
      &pushregister 4
      &pushzero
      &greaterThan
     label59:
      &setRegister r:3
      &pop
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 18							//'m_screen'
      &pushsdbgm 170							//'texttip'
      &pushsdb 23							//'_visible'
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 18							//'m_screen'
      &pushsdbgm 168							//'crosshairv'
      &pushsdb 23							//'_visible'
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 18							//'m_screen'
      &pushsdbgm 169							//'crosshairh'
      &pushsdb 23							//'_visible'
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 18							//'m_screen'
      &pushsdbgm 165							//'cursor'
      &pushsdb 23							//'_visible'
      &pushregister 3
      &setRegister r:0
      &setMember
      &pushregister 0
      &setRegister r:0
      &setMember
      &pushregister 0
      &setRegister r:0
      &setMember
      &pushregister 0
      &setMember
      &pushregister 3
      &not
      &jnz label60      
      &pushsdb 171							//'_zoom'
      &pushone
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 18							//'m_screen'
      &pushsdbgm 165							//'cursor'
      &dcallmp 20							// gotoAndPlay()
      &pushsdb 172							//'GuiScenarioIconSelect'
      &pushone
      &pushregister 2
      &dcallmp 96							// playSound()
      &jmp label61      
     label60:
      &pushregister 1
      &pushsdb 173							//'m_selectedScenarioId'
      &pushsdb 12							//''
      &setMember
      &pushregister 1
      &pushsdbgm 173							//'m_selectedScenarioId'
      &pushone
      &pushregister 1
      &dcallmp 174							// updateScenarioInfoBox()
     label61:
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 43							//'initMarkers'
    &function  (    )    
      &pushthisgv
      &pushsdb 71							//'m_markers'
      &pushzero
      &pushsdb 67							//'Array'
      &new
      &setMember
      &pushsdb 176							//'lp'
      &pushone
      &varEquals
     label62:
      &pushsdb 177							//'marker'
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 18							//'m_screen'
      &toString
      &pushsdb 178							//'.marker_'
      &add
      &pushsdbgv 176							//'lp'
      &toString
      &add
      &getVariable
      &varEquals
      &pushsdbgv 177							//'marker'
      &pushundef
      &equals
      &not
      &not
      &jnz label65      
      &pushzero
      &pushsdbgv 177							//'marker'
      &pushsdb 179							//'getUiName'
      &callMethod
      &pushsdb 12							//''
      &equals
      &not
      &not
      &jnz label63      
      &pushthisgv
      &pushsdbgm 71							//'m_markers'
      &pushzero
      &pushsdbgv 177							//'marker'
      &pushsdb 179							//'getUiName'
      &callMethod
      &pushsdbgv 177							//'marker'
      &setMember
      &pushsdbgv 177							//'marker'
      &pushsdb 23							//'_visible'
      &pushfalse
      &setMember
      &pushthisgv
      &pushsdbgm 180							//'onMarkerMouseEnter'
      &pushthisgv
      &pushbyte 2
      &pushglobalgv
      &pushsdb 27							//'bind1DynamicParams'
      &callMethod
      &pushone
      &pushsdbgv 177							//'marker'
      &dcallmp 181							// setMouseEnterCallback()
      &pushthisgv
      &pushsdbgm 182							//'onMarkerMouseExit'
      &pushthisgv
      &pushbyte 2
      &pushglobalgv
      &pushsdb 27							//'bind1DynamicParams'
      &callMethod
      &pushone
      &pushsdbgv 177							//'marker'
      &dcallmp 183							// setMouseExitCallback()
      &pushthisgv
      &pushsdbgm 111							//'onMarkerSelected'
      &pushthisgv
      &pushbyte 2
      &pushglobalgv
      &pushsdb 27							//'bind1DynamicParams'
      &callMethod
      &pushone
      &pushsdbgv 177							//'marker'
      &dcallmp 184							// setSelectedCallback()
      &pushthisgv
      &pushsdbgm 185							//'onUnlockAnimComplete'
      &pushthisgv
      &pushbyte 2
      &pushglobalgv
      &pushsdb 27							//'bind1DynamicParams'
      &callMethod
      &pushone
      &pushsdbgv 177							//'marker'
      &dcallmp 186							// setUnlockAnimCallback()
      &jmp label64      
     label63:
      &pushsdb 187							//'*** ERROR in fes_m_scenarioMap *** - Found marker clip with no UI name property in MC '
      &pushsdbgv 177							//'marker'
      &add
      &trace
     label64:
      &jmp label66      
     label65:
      &jmp label67      
     label66:
      &pushsdb 176							//'lp'
      &pushsdbgv 176							//'lp'
      &increment
      &setVariable
      &jmp label62      
     label67:
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 180							//'onMarkerMouseEnter'
    &function2  (r:2='marker') (r:1='this')
      &pushzero
      &pushregister 2
      &pushsdb 179							//'getUiName'
      &callMethod
      &pushone
      &pushregister 1
      &dcallmp 174							// updateScenarioInfoBox()
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 182							//'onMarkerMouseExit'
    &function2  ('marker') (r:1='this')
      &pushregister 1
      &pushsdbgm 173							//'m_selectedScenarioId'
      &pushone
      &pushregister 1
      &dcallmp 174							// updateScenarioInfoBox()
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 111							//'onMarkerSelected'
    &function2  (r:2='marker') (r:1='this')
      &pushregister 2
      &pushsdbgm 167							//'_y'
      &pushregister 2
      &pushsdbgm 166							//'_x'
      &pushbyte 2
      &pushregister 1
      &dcallmp 84							// setCursorPosition()
      &pushregister 2
      &pushone
      &pushregister 1
      &dcallmp 188							// updateTextTip()
      &pushregister 1
      &pushsdb 173							//'m_selectedScenarioId'
      &pushzero
      &pushregister 2
      &pushsdb 179							//'getUiName'
      &callMethod
      &setMember
      &pushregister 1
      &pushsdbgm 173							//'m_selectedScenarioId'
      &pushone
      &pushregister 1
      &dcallmp 174							// updateScenarioInfoBox()
      &pushzero
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 18							//'m_screen'
      &pushsdbgm 122							//'btnContinue'
      &dcallmp 128							// enable()
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 185							//'onUnlockAnimComplete'
    &function2  ('marker') (r:1='this')
      &pushtrue
      &pushone
      &pushregister 1
      &dcallmp 108							// unlockNextIcon()
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 108							//'unlockNextIcon'
    &function2  (r:4='playSound') (r:1='this', r:2='_global')
      &pushregister 1
      &pushsdbgm 71							//'m_markers'
      &pushzero
      &pushregister 1
      &pushsdbgm 68							//'m_iconsUnlocked'
      &pushsdb 92							//'shift'
      &callMethod
      &getMember
      &setRegister r:3
      &pop
      &pushregister 3
      &pushundef
      &equals
      &not
      &not
      &jnz label68      
      &pushzero
      &pushregister 3
      &dcallmp 79							// applyPreset()
      &pushregister 4
      &not
      &jnz label68      
      &pushsdb 189							//'GuiScenarioRevealEffect'
      &pushone
      &pushregister 2
      &dcallmp 96							// playSound()
     label68:
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 80							//'reorderUnlockedIcons'
    &function2  () (r:1='this')
      &pushregister 1
      &pushsdbgm 68							//'m_iconsUnlocked'
      &pushsdbgm 74							//'length'
      &setRegister r:5
      &pop
      &pushzero
      &setRegister r:3
      &pop
     label69:
      &pushregister 3
      &pushregister 5
      &lessThan
      &not
      &jnz label73      
      &pushregister 3
      &pushone
      &add
      &setRegister r:2
      &pop
     label70:
      &pushregister 2
      &pushregister 5
      &lessThan
      &not
      &jnz label72      
      &pushregister 1
      &pushsdbgm 71							//'m_markers'
      &pushregister 1
      &pushsdbgm 68							//'m_iconsUnlocked'
      &pushregister 2
      &getMember
      &getMember
      &pushsdbgm 166							//'_x'
      &pushregister 1
      &pushsdbgm 71							//'m_markers'
      &pushregister 1
      &pushsdbgm 68							//'m_iconsUnlocked'
      &pushregister 3
      &getMember
      &getMember
      &pushsdbgm 166							//'_x'
      &lessThan
      &not
      &jnz label71      
      &pushregister 1
      &pushsdbgm 68							//'m_iconsUnlocked'
      &pushregister 3
      &getMember
      &setRegister r:4
      &pop
      &pushregister 1
      &pushsdbgm 68							//'m_iconsUnlocked'
      &pushregister 3
      &pushregister 1
      &pushsdbgm 68							//'m_iconsUnlocked'
      &pushregister 2
      &getMember
      &setMember
      &pushregister 1
      &pushsdbgm 68							//'m_iconsUnlocked'
      &pushregister 2
      &pushregister 4
      &setMember
     label71:
      &pushregister 2
      &increment
      &setRegister r:2
      &pop
      &jmp label70      
     label72:
      &pushregister 3
      &increment
      &setRegister r:3
      &pop
      &jmp label69      
     label73:
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 45							//'updateProgress'
    &function2  () (r:1='this')
      &pushregister 1
      &pushsdb 87							//'m_changedTextFields'
      &pushzero
      &pushsdb 67							//'Array'
      &new
      &setMember
      &pushzero
      &pushsdb 8							//'Object'
      &new
      &setRegister r:2
      &pop
      &pushsdb 190							//'QueryGameEngine?SCENARIO_CURRENT_STATUS'
      &pushregister 2
      &loadVariables
      &pushregister 2
      &pushsdbgm 191							//'SCENARIO_CURRENT_STATUS_PERCENTAGE_FORMER'
      &pushundef
      &equals
      &not
      &not
      &jnz label74      
      &pushsdb 192							//'$'
      &pushregister 2
      &pushsdbgm 191							//'SCENARIO_CURRENT_STATUS_PERCENTAGE_FORMER'
      &toString
      &add
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 18							//'m_screen'
      &pushsdbgm 193							//'progressinfo'
      &pushsdbgm 194							//'pct'
      &pushbyte 2
      &pushregister 1
      &dcallmp 110							// updateDynTextClip()
      &pushsdb 192							//'$'
      &pushregister 2
      &pushsdbgm 195							//'SCENARIO_CURRENT_STATUS_PERCENTAGE'
      &toString
      &add
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 18							//'m_screen'
      &pushsdbgm 193							//'progressinfo'
      &pushsdbgm 194							//'pct'
      &pushbyte 2
      &pushregister 1
      &dcallmp 196							// addChangedTextField()
      &jmp label75      
     label74:
      &pushsdb 192							//'$'
      &pushregister 2
      &pushsdbgm 195							//'SCENARIO_CURRENT_STATUS_PERCENTAGE'
      &toString
      &add
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 18							//'m_screen'
      &pushsdbgm 193							//'progressinfo'
      &pushsdbgm 194							//'pct'
      &pushbyte 2
      &pushregister 1
      &dcallmp 110							// updateDynTextClip()
     label75:
      &pushregister 2
      &pushsdbgm 197							//'SCENARIO_CURRENT_STATUS_TOTALTIME_FORMER'
      &pushundef
      &equals
      &not
      &not
      &jnz label76      
      &pushsdb 192							//'$'
      &pushregister 2
      &pushsdbgm 197							//'SCENARIO_CURRENT_STATUS_TOTALTIME_FORMER'
      &toString
      &add
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 18							//'m_screen'
      &pushsdbgm 193							//'progressinfo'
      &pushsdbgm 198							//'time'
      &pushbyte 2
      &pushregister 1
      &dcallmp 110							// updateDynTextClip()
      &pushsdb 192							//'$'
      &pushregister 2
      &pushsdbgm 199							//'SCENARIO_CURRENT_STATUS_TOTALTIME'
      &toString
      &add
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 18							//'m_screen'
      &pushsdbgm 193							//'progressinfo'
      &pushsdbgm 198							//'time'
      &pushbyte 2
      &pushregister 1
      &dcallmp 196							// addChangedTextField()
      &jmp label77      
     label76:
      &pushsdb 192							//'$'
      &pushregister 2
      &pushsdbgm 199							//'SCENARIO_CURRENT_STATUS_TOTALTIME'
      &toString
      &add
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 18							//'m_screen'
      &pushsdbgm 193							//'progressinfo'
      &pushsdbgm 198							//'time'
      &pushbyte 2
      &pushregister 1
      &dcallmp 110							// updateDynTextClip()
     label77:
      &pushregister 2
      &pushsdbgm 200							//'SCENARIO_CURRENT_STATUS_TOTALMONEY_FORMER'
      &pushundef
      &equals
      &not
      &not
      &jnz label78      
      &pushsdb 192							//'$'
      &pushregister 2
      &pushsdbgm 200							//'SCENARIO_CURRENT_STATUS_TOTALMONEY_FORMER'
      &toString
      &add
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 18							//'m_screen'
      &pushsdbgm 193							//'progressinfo'
      &pushsdbgm 201							//'money'
      &pushbyte 2
      &pushregister 1
      &dcallmp 110							// updateDynTextClip()
      &pushsdb 192							//'$'
      &pushregister 2
      &pushsdbgm 202							//'SCENARIO_CURRENT_STATUS_TOTALMONEY'
      &toString
      &add
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 18							//'m_screen'
      &pushsdbgm 193							//'progressinfo'
      &pushsdbgm 201							//'money'
      &pushbyte 2
      &pushregister 1
      &dcallmp 196							// addChangedTextField()
      &jmp label79      
     label78:
      &pushsdb 192							//'$'
      &pushregister 2
      &pushsdbgm 202							//'SCENARIO_CURRENT_STATUS_TOTALMONEY'
      &toString
      &add
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 18							//'m_screen'
      &pushsdbgm 193							//'progressinfo'
      &pushsdbgm 201							//'money'
      &pushbyte 2
      &pushregister 1
      &dcallmp 110							// updateDynTextClip()
     label79:
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 18							//'m_screen'
      &pushsdbgm 193							//'progressinfo'
      &pushsdbgm 203							//'pct_effect'
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 18							//'m_screen'
      &pushsdbgm 193							//'progressinfo'
      &pushsdbgm 194							//'pct'
      &pushbyte 2
      &pushregister 1
      &dcallmp 204							// positionRevealEffectElement()
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 18							//'m_screen'
      &pushsdbgm 193							//'progressinfo'
      &pushsdbgm 205							//'time_effect'
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 18							//'m_screen'
      &pushsdbgm 193							//'progressinfo'
      &pushsdbgm 198							//'time'
      &pushbyte 2
      &pushregister 1
      &dcallmp 204							// positionRevealEffectElement()
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 18							//'m_screen'
      &pushsdbgm 193							//'progressinfo'
      &pushsdbgm 206							//'money_effect'
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 18							//'m_screen'
      &pushsdbgm 193							//'progressinfo'
      &pushsdbgm 201							//'money'
      &pushbyte 2
      &pushregister 1
      &dcallmp 204							// positionRevealEffectElement()
      &pushone
      &pushzero
      &pushregister 2
      &pushsdbgm 207							//'SCENARIO_CURRENT_STATUS_PROGRESS'
      &toNumber
      &pushbyte 2
      &pushsdbgv 208							//'Math'
      &pushsdb 209							//'max'
      &callMethod
      &pushbyte 2
      &pushsdbgv 208							//'Math'
      &pushsdb 210							//'min'
      &callMethod
      &setRegister r:3
      &pop
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 18							//'m_screen'
      &pushsdbgm 193							//'progressinfo'
      &pushsdbgm 211							//'progressbar'
      &pushsdbgm 212							//'fg'
      &pushsdb 213							//'_width'
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 18							//'m_screen'
      &pushsdbgm 193							//'progressinfo'
      &pushsdbgm 211							//'progressbar'
      &pushsdbgm 214							//'bg'
      &pushsdbgm 213							//'_width'
      &pushregister 3
      &multiply
      &setMember
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 18							//'m_screen'
      &pushsdbgm 193							//'progressinfo'
      &pushsdbgm 211							//'progressbar'
      &pushsdb 23							//'_visible'
      &pushregister 3
      &pushzero
      &greaterThan
      &setMember
      &getURL 'FSCommand:CallGameFunction'       '%ScenarioAcknowledgeTrackedChanges'      
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 204							//'positionRevealEffectElement'
    &function2  (r:3='textmc', r:4='effectmc') (r:1='this', r:2='_global')
      &pushregister 4
      &pushsdb 215							//'doTextReveal'
      &pushregister 3
      &pushregister 1
      &pushsdbgm 216							//'onDoTextReveal'
      &pushregister 1
      &pushbyte 3
      &pushregister 2
      &pushsdb 217							//'bind1'
      &callMethod
      &setMember
      &pushregister 4
      &pushsdb 166							//'_x'
      &pushregister 3
      &pushsdbgm 166							//'_x'
      &pushregister 3
      &pushsdbgm 213							//'_width'
      &add
      &pushregister 3
      &pushsdbgm 218							//'toptext'
      &pushsdbgm 213							//'_width'
      &subtract
      &pushsdbgv 1							//'fes_m_scenarioMap'
      &pushsdbgm 219							//'TEXT_REVEAL_EFFECT_MARGIN'
      &subtract
      &setMember
      &pushregister 4
      &pushsdb 213							//'_width'
      &pushregister 3
      &pushsdbgm 218							//'toptext'
      &pushsdbgm 220							//'textWidth'
      &pushsdbgv 1							//'fes_m_scenarioMap'
      &pushsdbgm 219							//'TEXT_REVEAL_EFFECT_MARGIN'
      &pushbyte 2
      &multiply
      &add
      &setMember
      &pushregister 3
      &pushsdb 94							//'revealEffectMC'
      &pushregister 4
      &setMember
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 216							//'onDoTextReveal'
    &function2  (r:2='textmc') (r:1='this')
      &pushregister 2
      &pushsdbgm 109							//'pendingString'
      &pushregister 2
      &pushbyte 2
      &pushregister 1
      &dcallmp 110							// updateDynTextClip()
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 196							//'addChangedTextField'
    &function2  (r:2='textmc', r:3='pending') (r:1='this')
      &pushregister 2
      &pushsdb 109							//'pendingString'
      &pushregister 3
      &setMember
      &pushregister 2
      &pushone
      &pushregister 1
      &pushsdbgm 87							//'m_changedTextFields'
      &dcallmp 76							// push()
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 46							//'updateScenarios'
    &function2  () (r:1='this')
      &pushzero
      &pushsdb 8							//'Object'
      &new
      &setRegister r:6
      &pop
      &pushsdb 221							//'QueryGameEngine?SCENARIO_MISSION_IDS'
      &pushregister 6
      &loadVariables
      &pushregister 1
      &pushsdb 73							//'m_scenarioIds'
      &pushzero
      &pushsdb 67							//'Array'
      &new
      &setMember
      &pushregister 1
      &pushsdb 73							//'m_scenarioIds'
      &pushsdb 222							//','
      &pushone
      &pushregister 6
      &pushsdbgm 223							//'SCENARIO_MISSION_IDS'
      &pushsdb 224							//'split'
      &callMethod
      &setMember
      &pushregister 1
      &pushsdb 225							//'m_scenarioNames'
      &pushzero
      &pushsdb 67							//'Array'
      &new
      &setMember
      &pushregister 1
      &pushsdb 226							//'m_scenarioShortNames'
      &pushzero
      &pushsdb 67							//'Array'
      &new
      &setMember
      &pushregister 1
      &pushsdb 227							//'m_scenarioParTimes'
      &pushzero
      &pushsdb 67							//'Array'
      &new
      &setMember
      &pushregister 1
      &pushsdb 228							//'m_scenarioBestTimes'
      &pushzero
      &pushsdb 67							//'Array'
      &new
      &setMember
      &pushregister 1
      &pushsdb 229							//'m_scenarioUnitNames'
      &pushzero
      &pushsdb 67							//'Array'
      &new
      &setMember
      &pushregister 1
      &pushsdb 230							//'m_scenarioUnitDescs'
      &pushzero
      &pushsdb 67							//'Array'
      &new
      &setMember
      &pushzero
      &setRegister r:4
      &pop
     label80:
      &pushregister 4
      &pushregister 1
      &pushsdbgm 73							//'m_scenarioIds'
      &pushsdbgm 74							//'length'
      &lessThan
      &not
      &jnz label83      
      &pushregister 1
      &pushsdbgm 73							//'m_scenarioIds'
      &pushregister 4
      &getMember
      &setRegister r:2
      &pop
      &pushzero
      &pushsdb 8							//'Object'
      &new
      &setRegister r:3
      &pop
      &pushsdb 231							//'QueryGameEngine?SCENARIO_MISSION_DATA?Mission='
      &pushregister 2
      &add
      &pushregister 3
      &loadVariables
      &pushregister 1
      &pushsdbgm 225							//'m_scenarioNames'
      &pushregister 2
      &pushsdb 192							//'$'
      &pushregister 3
      &pushsdbgm 232							//'SCENARIO_MISSION_DATA_NAME'
      &add
      &setMember
      &pushregister 1
      &pushsdbgm 226							//'m_scenarioShortNames'
      &pushregister 2
      &pushsdb 192							//'$'
      &pushregister 3
      &pushsdbgm 232							//'SCENARIO_MISSION_DATA_NAME'
      &add
      &setMember
      &pushregister 1
      &pushsdbgm 227							//'m_scenarioParTimes'
      &pushregister 2
      &pushsdb 192							//'$'
      &pushregister 3
      &pushsdbgm 233							//'SCENARIO_MISSION_DATA_PARTIME'
      &add
      &setMember
      &pushregister 1
      &pushsdbgm 228							//'m_scenarioBestTimes'
      &pushregister 2
      &pushsdb 192							//'$'
      &pushregister 3
      &pushsdbgm 234							//'SCENARIO_MISSION_DATA_BESTTIME'
      &add
      &setMember
      &pushregister 1
      &pushsdbgm 229							//'m_scenarioUnitNames'
      &pushregister 2
      &pushsdb 192							//'$'
      &pushregister 3
      &pushsdbgm 235							//'SCENARIO_MISSION_DATA_UNITNAME'
      &add
      &setMember
      &pushregister 1
      &pushsdbgm 230							//'m_scenarioUnitDescs'
      &pushregister 2
      &pushsdb 192							//'$'
      &pushregister 3
      &pushsdbgm 236							//'SCENARIO_MISSION_DATA_UNITDESC'
      &add
      &setMember
      &pushregister 1
      &pushsdbgm 71							//'m_markers'
      &pushregister 2
      &getMember
      &setRegister r:5
      &pop
      &pushregister 5
      &pushundef
      &equals
      &not
      &not
      &jnz label81      
      &pushregister 3
      &pushsdbgm 237							//'SCENARIO_MISSION_DATA_STATE'
      &toNumber
      &pushone
      &pushregister 5
      &dcallmp 238							// presetState()
      &jmp label82      
     label81:
      &pushsdb 239							//'*** ERROR in fes_m_scenarioMap *** - Unable to find marker clip for scenario \''
      &pushregister 2
      &add
      &pushsdb 240							//'\''
      &add
      &trace
     label82:
      &pushregister 4
      &increment
      &setRegister r:4
      &pop
      &jmp label80      
     label83:
      &getURL 'FSCommand:CallGameFunction'       '%ScenarioAcknowledgeNewlyUnlockedMissions'      
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 174							//'updateScenarioInfoBox'
    &function2  (r:3='scenarioId') (r:1='this')
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 18							//'m_screen'
      &pushsdbgm 82							//'lowerpanel'
      &pushsdbgm 241							//'scenario'
      &setRegister r:2
      &pop
      &pushregister 3
      &pushsdb 12							//''
      &equals
      &setRegister r:4
      &pop
      &pushregister 4
      &not
      &jnz label84      
      &pushregister 2
      &pushsdb 23							//'_visible'
      &pushfalse
      &setMember
      &jmp label87      
     label84:
      &pushregister 2
      &pushsdb 23							//'_visible'
      &pushtrue
      &setMember
      &pushregister 1
      &pushsdbgm 226							//'m_scenarioShortNames'
      &pushregister 3
      &getMember
      &pushregister 2
      &pushsdbgm 242							//'name'
      &pushbyte 2
      &pushregister 1
      &dcallmp 110							// updateDynTextClip()
      &pushregister 1
      &pushsdbgm 228							//'m_scenarioBestTimes'
      &pushregister 3
      &getMember
      &pushregister 2
      &pushsdbgm 243							//'bestTime'
      &pushbyte 2
      &pushregister 1
      &dcallmp 110							// updateDynTextClip()
      &pushregister 1
      &pushsdbgm 227							//'m_scenarioParTimes'
      &pushregister 3
      &getMember
      &pushregister 2
      &pushsdbgm 244							//'parTime'
      &pushbyte 2
      &pushregister 1
      &dcallmp 110							// updateDynTextClip()
      &pushregister 1
      &pushsdbgm 229							//'m_scenarioUnitNames'
      &pushregister 3
      &getMember
      &pushregister 2
      &pushsdbgm 245							//'unitName'
      &pushbyte 2
      &pushregister 1
      &dcallmp 110							// updateDynTextClip()
      &pushregister 1
      &pushsdbgm 230							//'m_scenarioUnitDescs'
      &pushregister 3
      &getMember
      &pushregister 2
      &pushsdbgm 246							//'unitDesc'
      &pushbyte 2
      &pushregister 1
      &dcallmp 110							// updateDynTextClip()
      &pushsdb 13							//'FSCommand:CallGameFunction'
      &pushsdb 247							//'%ScenarioSetUnitUnlockRendererScenario?Mission='
      &pushregister 3
      &add
      &getURL2
      &pushregister 2
      &pushsdbgm 248							//'uniticon'
      &pushsdbgm 249							//'renderimage'
      &pushsdb 250							//'_mode'
      &pushzero
      &pushregister 1
      &pushsdbgm 71							//'m_markers'
      &pushregister 3
      &getMember
      &pushsdb 251							//'isCompleted'
      &callMethod
      &jnz label85      
      &pushsdb 252							//'GRAYSCALE'
      &jmp label86      
     label85:
      &pushsdb 253							//'ALPHA'
     label86:
      &setMember
      &pushsdb 254							//'_refresh'
      &pushone
      &pushregister 2
      &pushsdbgm 248							//'uniticon'
      &pushsdbgm 249							//'renderimage'
      &dcallmp 20							// gotoAndPlay()
     label87:
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 110							//'updateDynTextClip'
    &function2  (r:1='mc', r:2='text') ()
      &pushregister 1
      &pushsdbgm 218							//'toptext'
      &pushsdb 255							//'text'
      &pushregister 1
      &pushsdw 256							//'droptext'
      &getMember
      &pushsdb 255							//'text'
      &pushregister 2
      &pushsdw 257							//'&Outline'
      &add
      &setRegister r:0
      &setMember
      &pushregister 0
      &setMember
      &pushsdb 254							//'_refresh'
      &pushone
      &pushregister 1
      &dcallmp 20							// gotoAndPlay()
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 188							//'updateTextTip'
    &function2  (r:3='mc') (r:1='this')
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 18							//'m_screen'
      &pushsdbgm 170							//'texttip'
      &setRegister r:2
      &pop
      &pushregister 2
      &pushsdb 166							//'_x'
      &pushregister 3
      &pushsdbgm 166							//'_x'
      &setMember
      &pushregister 2
      &pushsdb 167							//'_y'
      &pushregister 3
      &pushsdbgm 167							//'_y'
      &setMember
      &pushzero
      &pushregister 3
      &pushsdb 179							//'getUiName'
      &callMethod
      &setRegister r:4
      &pop
      &pushregister 2
      &pushsdw 258							//'left'
      &getMember
      &pushsdbgm 218							//'toptext'
      &pushsdb 255							//'text'
      &pushregister 2
      &pushsdw 258							//'left'
      &getMember
      &pushsdw 256							//'droptext'
      &getMember
      &pushsdb 255							//'text'
      &pushregister 2
      &pushsdw 259							//'right'
      &getMember
      &pushsdbgm 218							//'toptext'
      &pushsdb 255							//'text'
      &pushregister 2
      &pushsdw 259							//'right'
      &getMember
      &pushsdw 256							//'droptext'
      &getMember
      &pushsdb 255							//'text'
      &pushregister 4
      &pushsdb 12							//''
      &equals
      &jnz label88      
      &pushregister 1
      &pushsdbgm 226							//'m_scenarioShortNames'
      &pushregister 4
      &getMember
      &pushsdw 257							//'&Outline'
      &add
      &jmp label89      
     label88:
      &pushsdb 12							//''
     label89:
      &setRegister r:0
      &setMember
      &pushregister 0
      &setRegister r:0
      &setMember
      &pushregister 0
      &setRegister r:0
      &setMember
      &pushregister 0
      &setMember
      &pushregister 2
      &pushsdw 259							//'right'
      &getMember
      &pushsdb 23							//'_visible'
      &pushregister 3
      &pushsdbgm 166							//'_x'
      &pushsdbgv 1							//'fes_m_scenarioMap'
      &pushsdw 260							//'CURSOR_X_MIDPOINT'
      &getMember
      &lessThan
      &setMember
      &pushregister 2
      &pushsdw 258							//'left'
      &getMember
      &pushsdb 23							//'_visible'
      &pushregister 3
      &pushsdbgm 166							//'_x'
      &pushsdbgv 1							//'fes_m_scenarioMap'
      &pushsdw 260							//'CURSOR_X_MIDPOINT'
      &getMember
      &lessThan
      &not
      &setMember
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 34							//'onScreenExit'
    &function2  () (r:1='this')
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 18							//'m_screen'
      &pushsdb 54							//'onEnterFrame'
      &pushnull
      &setMember
      &pushzero
      &pushregister 1
      &dcallmp 38							// cleanup()
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 114							//'onBackClicked'
    &function2  () (r:1='_global')
      &getURL 'FSCommand:CallGameFunction'       '%ScenarioClearMission'      
      &pushzero
      &pushregister 1
      &pushsdbgm 36							//'gSM'
      &pushsdw 261							//'goBackScreen'
      &callmp
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 117							//'onHelpClicked'
    &function  (    )    
      &pushsdb 17							//'_show'
      &pushone
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 18							//'m_screen'
      &pushsdbgm 19							//'helplayer'
      &dcallmp 20							// gotoAndPlay()
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 18							//'m_screen'
      &pushsdbgm 19							//'helplayer'
      &pushsdbgm 21							//'helpmc'
      &pushsdbgm 22							//'bgimage'
      &pushsdb 23							//'_visible'
      &pushfalse
      &setMember
      &pushsdw 262							//'$APT:CLOSE'
      &pushone
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 18							//'m_screen'
      &pushsdbgm 19							//'helplayer'
      &pushsdbgm 21							//'helpmc'
      &pushsdbgm 49							//'dialog'
      &pushsdbgm 50							//'btnClose'
      &dcallmp 51							// setText()
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 119							//'onUnlocksClicked'
    &function2  () (r:1='_global')
      &pushregister 1
      &pushsdw 263							//'SHELL'
      &getMember
      &pushsdw 264							//'SCENARIO_UNLOCK_MODE'
      &pushregister 1
      &pushsdw 265							//'ENUM'
      &getMember
      &pushsdw 266							//'SCENARIO_UNLOCK_MODE_DEFAULT'
      &getMember
      &setMember
      &pushregister 1
      &pushsdw 267							//'SCREEN'
      &getMember
      &pushsdw 268							//'FES_SCENARIO_UNLOCKS'
      &getMember
      &pushone
      &pushregister 1
      &pushsdbgm 36							//'gSM'
      &pushsdw 269							//'changeMainScreen'
      &callmp
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 121							//'onContinueClicked'
    &function2  () (r:1='this', r:2='_global')
      &pushregister 1
      &pushsdbgm 173							//'m_selectedScenarioId'
      &pushsdb 12							//''
      &equals
      &not
      &not
      &jnz label90      
      &pushsdb 13							//'FSCommand:CallGameFunction'
      &pushsdw 270							//'%ScenarioSelectMission?Mission='
      &pushregister 1
      &pushsdbgm 173							//'m_selectedScenarioId'
      &add
      &getURL2
      &pushregister 2
      &pushsdw 267							//'SCREEN'
      &getMember
      &pushsdw 271							//'FES_SCENARIO_GAME_SETUP'
      &getMember
      &pushone
      &pushregister 2
      &pushsdbgm 36							//'gSM'
      &pushsdw 269							//'changeMainScreen'
      &callmp
     label90:
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 126							//'onCloseHelpClicked'
    &function2  () (r:1='this')
      &pushregister 1
      &pushsdbgm 10							//'m_isFirstRun'
      &jnz label91      
      &pushsdb 16							//'_hide'
      &jmp label92      
     label91:
      &pushsdb 112							//'_fade'
     label92:
      &pushone
      &pushsdbgv 2							//'Cafe2_BaseUIScreen'
      &pushsdbgm 18							//'m_screen'
      &pushsdbgm 19							//'helplayer'
      &dcallmp 20							// gotoAndPlay()
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 150							//'onBlockerClicked'
    &function2  () (r:1='this')
      &pushregister 1
      &pushsdbgm 52							//'m_iconRevealState'
      &pushsdbgv 1							//'fes_m_scenarioMap'
      &pushsdbgm 113							//'IRSTATE_NONE'
      &equals
      &not
      &not
      &jnz label93      
      &pushregister 1
      &pushsdb 52							//'m_iconRevealState'
      &pushsdbgv 1							//'fes_m_scenarioMap'
      &pushsdbgm 63							//'IRSTATE_ABORT'
      &setMember
     label93:
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 154							//'onUnlockAll'
    &function2  () (r:1='this')
      &pushzero
      &setRegister r:3
      &pop
     label94:
      &pushregister 3
      &pushregister 1
      &pushsdbgm 73							//'m_scenarioIds'
      &pushsdbgm 74							//'length'
      &lessThan
      &not
      &jnz label97      
      &pushregister 1
      &pushsdbgm 71							//'m_markers'
      &pushregister 1
      &pushsdbgm 73							//'m_scenarioIds'
      &pushregister 3
      &getMember
      &getMember
      &setRegister r:2
      &pop
      &pushregister 2
      &pushundef
      &equals
      &not
      &not
      &jnz label96      
      &pushzero
      &pushregister 2
      &pushsdb 72							//'isLocked'
      &callMethod
      &not
      &jnz label95      
      &pushone
      &pushone
      &pushregister 2
      &dcallmp 238							// presetState()
     label95:
      &pushzero
      &pushregister 2
      &dcallmp 79							// applyPreset()
     label96:
      &pushregister 3
      &increment
      &setRegister r:3
      &pop
      &jmp label94      
     label97:
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 156							//'onCompleteMission'
    &function2  () (r:1='this')
      &pushregister 1
      &pushsdbgm 173							//'m_selectedScenarioId'
      &pushsdb 12							//''
      &equals
      &not
      &not
      &jnz label98      
      &pushsdb 13							//'FSCommand:CallGameFunction'
      &pushsdw 272							//'%ScenarioCompleteMission?Mission='
      &pushregister 1
      &pushsdbgm 173							//'m_selectedScenarioId'
      &add
      &pushsdw 273							//'|Time=50|Efficiency=1.0'
      &add
      &getURL2
      &pushzero
      &pushregister 1
      &dcallmp 45							// updateProgress()
      &pushzero
      &pushregister 1
      &dcallmp 46							// updateScenarios()
      &pushregister 1
      &pushsdb 52							//'m_iconRevealState'
      &pushsdbgv 1							//'fes_m_scenarioMap'
      &pushsdbgm 53							//'IRSTATE_BEGIN'
      &setMember
     label98:
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 125							//'onHelpFadeOutComplete'
    &function2  () (r:1='this')
      &pushzero
      &pushregister 1
      &dcallmp 47							// beginIconReveal()
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 26							//'localMessageHandler'
    &function2  (r:2='message') ()
      &pushtrue
      &setRegister r:1
      &pop
      &jmp label99      
     label99:
      &pushfalse
      &setRegister r:1
      &pop
      &jmp label100      
     label100:
      &pushregister 1
      &return
    &end // of function 

    &setMember
    &pushregister 2
    &pushsdb 30							//'localEventHandler'
    &function2  (r:2='key', 'controller') (r:1='_global')
      &jmp label101      
     label101:
      &pushregister 2
      &pushone
      &pushregister 1
      &pushsdw 274							//'gFM'
      &getMember
      &pushsdw 275							//'handleDefaultInputs'
      &callmp
      &jmp label102      
     label102:
    &end // of function 

    &setMember
    &pushregister 1
    &pushsdb 113							//'IRSTATE_NONE'
    &pushzero
    &setMember
    &pushregister 1
    &pushsdb 53							//'IRSTATE_BEGIN'
    &pushone
    &setMember
    &pushregister 1
    &pushsdb 56							//'IRSTATE_DELAY'
    &pushbyte 2
    &setMember
    &pushregister 1
    &pushsdb 57							//'IRSTATE_CUE_CHANGED_TEXT_REVEAL'
    &pushbyte 3
    &setMember
    &pushregister 1
    &pushsdb 58							//'IRSTATE_CUEMSG_COMPLETED'
    &pushbyte 4
    &setMember
    &pushregister 1
    &pushsdb 59							//'IRSTATE_SHOWICONS_COMPLETED'
    &pushbyte 5
    &setMember
    &pushregister 1
    &pushsdb 60							//'IRSTATE_CUEMSG_UNLOCKED'
    &pushbyte 6
    &setMember
    &pushregister 1
    &pushsdb 61							//'IRSTATE_UNLOCK_DELAY'
    &pushbyte 7
    &setMember
    &pushregister 1
    &pushsdb 62							//'IRSTATE_SHOWICONS_UNLOCKED'
    &pushbyte 8
    &setMember
    &pushregister 1
    &pushsdb 63							//'IRSTATE_ABORT'
    &pushbyte 9
    &setMember
    &pushregister 1
    &pushsdb 64							//'IRSTATE_END'
    &pushbyte 10
    &setMember
    &pushregister 1
    &pushsdw 260							//'CURSOR_X_MIDPOINT'
    &pushshort 850
    &setMember
    &pushregister 1
    &pushsdb 219							//'TEXT_REVEAL_EFFECT_MARGIN'
    &pushbyte 15
    &setMember
    &pushregister 1
    &pushsdb 97							//'MAX_SCENARIO_PROGRESS_EFFECT'
    &pushbyte 3
    &setMember
    &pushone
    &pushnull
    &pushglobalgv
    &pushsdbgm 1							//'fes_m_scenarioMap'
    &pushsdbgm 24							//'prototype'
    &pushbyte 3
    &pushsdw 276							//'ASSetPropFlags'
    &callFunction
   label103:
    &pop
  &end // of initMovieClip 108
&end
