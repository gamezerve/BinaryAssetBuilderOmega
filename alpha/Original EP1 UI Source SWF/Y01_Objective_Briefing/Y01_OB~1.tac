movie 'E:\depot\CnC\RA3_EP1\PRODUC~1\Data\APTBUI~1\034A3~1.0-D\pc\Output\Y01_OB~1\\Y01_OB~1.eaf' &compressed // flash 7, total frames: 1, frame rate: 30 fps, 455x474 px

  &frame 0
    &pushzero
    &pushglobal
    &getVariable
    &pushs 'objBriefing'
    &getMember
    &pushs 'hideReplayButton'
    &callmp
    &stop
  &end // of frame 0
  
  &importAssets &from 'fe_objectiveBriefingsAssets.swf'
    'Map frame' &as 1
  &end // of importAssets
&end
