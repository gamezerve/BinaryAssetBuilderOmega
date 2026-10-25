movie 'E:\depot\CnC\RA3_EP1\PRODUC~1\Data\APTBUI~1\034A3~1.0-D\pc\Output\A02_OB~1\\A02_OB~1.eaf' &compressed // flash 7, total frames: 840, frame rate: 30 fps, 455x474 px

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
    &pushs '$DIALOGEVENT:MA02_ALLIEDINTEL50'
    &pushs 'MA02_AlliedIntel50'
    &pushbyte 2
    &pushs 'playSound_Intel'
    &callfp
  &end // of frame 29

  &frame 149
    &pushzero
    &pushs 'playSound_SquareTargetFlash'
    &callfp
  &end // of frame 149
  
  &importAssets &from 'fe_objectiveBriefingsAssets.swf'
    'box 2 mc' &as 5
  &end // of importAssets

  &frame 159
    &pushzero
    &pushs 'playSound_GreenMarkerFlash'
    &callfp
  &end // of frame 159
  
  &importAssets &from 'fe_objectiveBriefingsAssets.swf'
    'OI Primary Attack mc' &as 6
  &end // of importAssets

  &frame 299
    &pushs '$DIALOGEVENT:MA02_ALLIEDINTEL51'
    &pushs 'MA02_AlliedIntel51'
    &pushbyte 2
    &pushs 'playSound_Intel'
    &callfp
  &end // of frame 299

  &frame 359
    &pushzero
    &pushs 'playSound_SquareTargetFlash'
    &callfp
  &end // of frame 359

  &frame 369
    &pushzero
    &pushs 'playSound_GreenMarkerFlash'
    &callfp
  &end // of frame 369

  &frame 389
    &pushzero
    &pushs 'playSound_SquareTargetFlash'
    &callfp
  &end // of frame 389

  &frame 399
    &pushzero
    &pushs 'playSound_GreenMarkerFlash'
    &callfp
  &end // of frame 399

  &frame 419
    &pushzero
    &pushs 'playSound_SquareTargetFlash'
    &callfp
  &end // of frame 419

  &frame 429
    &pushzero
    &pushs 'playSound_GreenMarkerFlash'
    &callfp
  &end // of frame 429

  &frame 449
    &pushzero
    &pushs 'playSound_SquareTargetFlash'
    &callfp
    &pushs '$DIALOGEVENT:MA02_ALLIEDINTEL53'
    &pushs 'MA02_AlliedIntel53'
    &pushbyte 2
    &pushs 'playSound_Intel'
    &callfp
  &end // of frame 449

  &frame 459
    &pushzero
    &pushs 'playSound_GreenMarkerFlash'
    &callfp
  &end // of frame 459

  &frame 539
    &pushs '$DIALOGEVENT:MA02_ALLIEDINTEL54'
    &pushs 'MA02_AlliedIntel54'
    &pushbyte 2
    &pushs 'playSound_Intel'
    &callfp
  &end // of frame 539

  &frame 659
    &pushs '$DIALOGEVENT:MA02_ALLIEDINTEL55'
    &pushs 'MA02_AlliedIntel55'
    &pushbyte 2
    &pushs 'playSound_Intel'
    &callfp
  &end // of frame 659

  &frame 839
    &pushzero
    &pushglobalgv
    &pushsgm 'objBriefing'
    &pushs 'requestHideSubtitle'
    &callmp
    &stop
  &end // of frame 839
&end
