movie 'E:\depot\CnC\RA3_EP1\PRODUC~1\Data\APTBUI~1\034A3~1.0-D\pc\Output\J02_OB~1\\J02_OB~1.eaf' &compressed // flash 7, total frames: 960, frame rate: 30 fps, 455x474 px

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
    &pushs '$DIALOGEVENT:MI02_IMPERIALINTEL49'
    &pushs 'MI02_ImperialIntel49'
    &pushbyte 2
    &pushs 'playSound_Intel'
    &callfp
  &end // of frame 29

  &frame 59
    &pushzero
    &pushs 'playSound_Arrow'
    &callfp
  &end // of frame 59
  
  &importAssets &from 'fe_objectiveBriefingsAssets.swf'
    'red arrow wide mc' &as 5
  &end // of importAssets

  &frame 329
    &pushs '$DIALOGEVENT:MI02_IMPERIALINTEL50'
    &pushs 'MI02_ImperialIntel50'
    &pushbyte 2
    &pushs 'playSound_Intel'
    &callfp
  &end // of frame 329

  &frame 359
    &pushzero
    &pushs 'playSound_CircularTargetFlash'
    &callfp
  &end // of frame 359
  
  &importAssets &from 'fe_objectiveBriefingsAssets.swf'
    'circle mc' &as 6
  &end // of importAssets

  &frame 569
    &pushs '$DIALOGEVENT:MI02_IMPERIALINTEL51'
    &pushs 'MI02_ImperialIntel51'
    &pushbyte 2
    &pushs 'playSound_Intel'
    &callfp
  &end // of frame 569

  &frame 749
    &pushzero
    &pushs 'playSound_Arrow'
    &callfp
  &end // of frame 749
  
  &importAssets &from 'fe_objectiveBriefingsAssets.swf'
    'blue arrow thin mc' &as 7
  &end // of importAssets

  &frame 779
    &pushzero
    &pushs 'playSound_SquareTargetFlash'
    &callfp
  &end // of frame 779
  
  &importAssets &from 'fe_objectiveBriefingsAssets.swf'
    'box 2 mc' &as 8
  &end // of importAssets

  &frame 789
    &pushzero
    &pushs 'playSound_GreenMarkerFlash'
    &callfp
  &end // of frame 789
  
  &importAssets &from 'fe_objectiveBriefingsAssets.swf'
    'OI Primary Capture mc' &as 9
  &end // of importAssets

  &frame 839
    &pushzero
    &pushs 'playSound_Arrow'
    &callfp
  &end // of frame 839

  &frame 859
    &pushzero
    &pushs 'playSound_GreenMarkerFlash'
    &callfp
  &end // of frame 859
  
  &importAssets &from 'fe_objectiveBriefingsAssets.swf'
    'OI Primary Move mc' &as 10
  &end // of importAssets

  &frame 959
    &pushzero
    &pushglobalgv
    &pushsgm 'objBriefing'
    &pushs 'requestHideSubtitle'
    &callmp
    &stop
  &end // of frame 959
&end
