movie 'E:\depot\CnC\RA3_EP1\PRODUC~1\Data\APTBUI~1\034A3~1.0-D\pc\Output\S04_OB~1\\S04_OB~1.eaf' &compressed // flash 7, total frames: 1500, frame rate: 30 fps, 455x474 px

  &frame 0
    &constants 'UBriefObjective_Arrow1', 'playSound', 'UBriefObjective_Circle1TargetFlash', 'UBriefObjective_ObjectiveMarker1Flash', 'UBriefObjective_ObjectiveMarker2Flash', 'UBriefObjective_SquareTarget1Flash', 'UBriefObjective_Start', 'UBriefObjective_ZoomIn', 'UBriefObjective_ZoomOut', 'SOUND', 'LAST_VO_PLAYED', 'objBriefing', 'requestShowSubtitle', 'setSubtitle'  
    &function2 playSound_Arrow () (r:1='_global')
      &pushsdb 0							//'UBriefObjective_Arrow1'
      &pushone
      &pushregister 1
      &dcallmp 1							// playSound()
    &end // of function playSound_Arrow

    &function2 playSound_CircularTargetFlash () (r:1='_global')
      &pushsdb 2							//'UBriefObjective_Circle1TargetFlash'
      &pushone
      &pushregister 1
      &dcallmp 1							// playSound()
    &end // of function playSound_CircularTargetFlash

    &function2 playSound_GreenMarkerFlash () (r:1='_global')
      &pushsdb 3							//'UBriefObjective_ObjectiveMarker1Flash'
      &pushone
      &pushregister 1
      &dcallmp 1							// playSound()
    &end // of function playSound_GreenMarkerFlash

    &function2 playSound_YellowMarkerFlash () (r:1='_global')
      &pushsdb 4							//'UBriefObjective_ObjectiveMarker2Flash'
      &pushone
      &pushregister 1
      &dcallmp 1							// playSound()
    &end // of function playSound_YellowMarkerFlash

    &function2 playSound_SquareTargetFlash () (r:1='_global')
      &pushsdb 5							//'UBriefObjective_SquareTarget1Flash'
      &pushone
      &pushregister 1
      &dcallmp 1							// playSound()
    &end // of function playSound_SquareTargetFlash

    &function2 playSound_Start () (r:1='_global')
      &pushsdb 6							//'UBriefObjective_Start'
      &pushone
      &pushregister 1
      &dcallmp 1							// playSound()
    &end // of function playSound_Start

    &function2 playSound_ZoomIn () (r:1='_global')
      &pushsdb 7							//'UBriefObjective_ZoomIn'
      &pushone
      &pushregister 1
      &dcallmp 1							// playSound()
    &end // of function playSound_ZoomIn

    &function2 playSound_ZoomOut () (r:1='_global')
      &pushsdb 8							//'UBriefObjective_ZoomOut'
      &pushone
      &pushregister 1
      &dcallmp 1							// playSound()
    &end // of function playSound_ZoomOut

    &function2 playSound_Intel (r:2='soundName', r:3='subtitleName') (r:1='_global')
      &pushregister 1
      &pushsdbgm 9							//'SOUND'
      &pushsdb 10							//'LAST_VO_PLAYED'
      &pushregister 2
      &setMember
      &pushregister 1
      &pushsdbgm 9							//'SOUND'
      &pushsdbgm 10							//'LAST_VO_PLAYED'
      &pushone
      &pushregister 1
      &dcallmp 1							// playSound()
      &pushzero
      &pushregister 1
      &pushsdbgm 11							//'objBriefing'
      &dcallmp 12							// requestShowSubtitle()
      &pushregister 3
      &pushone
      &pushregister 1
      &pushsdbgm 11							//'objBriefing'
      &dcallmp 13							// setSubtitle()
    &end // of function playSound_Intel

  &end // of frame 0
  
  &importAssets &from 'fe_objectiveBriefingsAssets.swf'
    'Map frame' &as 4
  &end // of importAssets

  &frame 2
    &pushzero
    &pushs 'playSound_Start'
    &callfp
  &end // of frame 2

  &frame 29
    &pushs '$DIALOGEVENT:MS04_SOVIETINTEL53'
    &pushs 'MS04_SovietIntel53'
    &pushbyte 2
    &pushs 'playSound_Intel'
    &callfp
  &end // of frame 29

  &defineMovieClip 7 // total frames: 1
  &end // of defineMovieClip 7

  &frame 479
    &pushs '$DIALOGEVENT:MS04_SOVIETINTEL54'
    &pushs 'MS04_SovietIntel54'
    &pushbyte 2
    &pushs 'playSound_Intel'
    &callfp
  &end // of frame 479

  &frame 569
    &pushs '$DIALOGEVENT:MS04_SOVIETINTEL55'
    &pushs 'MS04_SovietIntel55'
    &pushbyte 2
    &pushs 'playSound_Intel'
    &callfp
  &end // of frame 569

  &frame 599
    &pushzero
    &pushs 'playSound_Arrow'
    &callfp
  &end // of frame 599
  
  &importAssets &from 'fe_objectiveBriefingsAssets.swf'
    'blue arrow wide mc' &as 8
  &end // of importAssets

  &frame 609
    &pushzero
    &pushs 'playSound_CircularTargetFlash'
    &callfp
  &end // of frame 609
  
  &importAssets &from 'fe_objectiveBriefingsAssets.swf'
    'circle mc' &as 9
  &end // of importAssets

  &frame 619
    &pushzero
    &pushs 'playSound_GreenMarkerFlash'
    &callfp
  &end // of frame 619
  
  &importAssets &from 'fe_objectiveBriefingsAssets.swf'
    'OI Primary Move' &as 10
  &end // of importAssets

  &defineMovieClip 11 // total frames: 8

    &frame 7
      &stop
    &end // of frame 7
  &end // of defineMovieClip 11

  &frame 809
    &pushs '$DIALOGEVENT:MS04_SOVIETINTEL56'
    &pushs 'MS04_SovietIntel56'
    &pushbyte 2
    &pushs 'playSound_Intel'
    &callfp
  &end // of frame 809

  &frame 826
    &pushzero
    &pushs 'playSound_Arrow'
    &callfp
  &end // of frame 826

  &frame 838
    &pushzero
    &pushs 'playSound_Arrow'
    &callfp
  &end // of frame 838

  &frame 839
    &pushzero
    &pushs 'playSound_CircuilarTargetFlash'
    &callfp
  &end // of frame 839

  &frame 849
    &pushzero
    &pushs 'playSound_GreenMarkerFlash'
    &callfp
    &pushzero
    &pushs 'playSound_CircularTargetFlash'
    &callfp
    &pushzero
    &pushs 'playSound_Arrow'
    &callfp
  &end // of frame 849
  
  &importAssets &from 'fe_objectiveBriefingsAssets.swf'
    'OI Primary Attack mc' &as 12
  &end // of importAssets

  &frame 859
    &pushzero
    &pushs 'playSound_GreenMarkerFlash'
    &callfp
    &pushzero
    &pushs 'playSound_CircularTargetFlash'
    &callfp
  &end // of frame 859

  &frame 869
    &pushzero
    &pushs 'playSound_GreenMarkerFlash'
    &callfp
  &end // of frame 869

  &frame 989
    &pushs '$DIALOGEVENT:MS04_SOVIETINTEL57'
    &pushs 'MS04_SovietIntel57'
    &pushbyte 2
    &pushs 'playSound_Intel'
    &callfp
  &end // of frame 989

  &frame 1007
    &pushzero
    &pushs 'playSound_Arrow'
    &callfp
  &end // of frame 1007

  &frame 1019
    &pushzero
    &pushs 'playSound_CircularTargetFlash'
    &callfp
  &end // of frame 1019

  &frame 1029
    &pushzero
    &pushs 'playSound_GreenMarkerFlash'
    &callfp
  &end // of frame 1029

  &frame 1199
    &pushs '$DIALOGEVENT:MS04_SOVIETINTEL58'
    &pushs 'MS04_SovietIntel58'
    &pushbyte 2
    &pushs 'playSound_Intel'
    &callfp
  &end // of frame 1199

  &frame 1499
    &pushzero
    &pushglobalgv
    &pushsgm 'objBriefing'
    &pushs 'requestHideSubtitle'
    &callmp
    &stop
  &end // of frame 1499
&end
