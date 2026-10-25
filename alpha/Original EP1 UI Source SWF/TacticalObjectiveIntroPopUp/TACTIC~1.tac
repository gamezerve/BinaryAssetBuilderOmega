movie 'E:\depot\CnC\RA3_EP1\PRODUC~1\Data\APTBUI~1\034A3~1.0-D\pc\Output\TAA2A0~1\\TACTIC~1.eaf' &compressed // flash 7, total frames: 136, frame rate: 30 fps, 1024x768 px

  &frame 0
    &constants 'GetCurrentFaction', 'Yuriko', 'popUpMC', 'outerFrameMC', 'SetOuterFrameState', 'innerFrameMC', 'SetInnerFrameState', 'symbolMC', 'gotoAndPlay', '_primary', '_bonus', 'titleTF', 'textColor', 'dividerMC', 'Color', 'setRGB', 'closing', 'bl', '_y', 'bottom', 'br', 'left', '_height', 'right', 'bg', 'tl', 'top', 'tr', 'SetObjectiveType', 'Close', 'AdjustOuterFrameHeight', 'AdjustInnerFrameHeight', 'onUnload', 'initialized', 'this', '_parent', 'OnContentLoaded', 'origTextHeight', 'textTF', 'text', '$', 'Title', '_global', 'qualifyName', '&outline', 'autoSize', 'Text', 'newTextHeight', 'heightIncrease', 'FSCommand:', 'OnInitialized', ''  
    &function2 SetObjectiveType (r:8='cat', r:9='bonusArg') (r:1='_root')
      &pushundef
      &setRegister r:2
      &pop
      &pushzero
      &pushregister 1
      &pushsdb 0							//'GetCurrentFaction'
      &callMethod
      &pushsdb 1							//'Yuriko'
      &equals
      &not
      &jnz label1      
      &pushlong 16754419
      &setRegister r:2
      &pop
      &pushsdb 1							//'Yuriko'
      &pushsdbgv 2							//'popUpMC'
      &pushsdbgm 3							//'outerFrameMC'
      &pushbyte 2
      &dcallfp 4							// SetOuterFrameState()
      &pushsdb 1							//'Yuriko'
      &pushsdbgv 2							//'popUpMC'
      &pushsdbgm 5							//'innerFrameMC'
      &pushbyte 2
      &dcallfp 6							// SetInnerFrameState()
      &pushsdb 1							//'Yuriko'
      &pushone
      &pushsdbgv 2							//'popUpMC'
      &pushsdbgm 7							//'symbolMC'
      &dcallmp 8							// gotoAndPlay()
      &jmp label6      
     label1:
      &pushregister 9
      &pushzero
      &equals
      &not
      &setRegister r:4
      &pop
      &pushlong 11993018
      &setRegister r:5
      &pop
      &pushlong 16772674
      &setRegister r:7
      &pop
      &pushregister 4
      &jnz label2      
      &pushregister 5
      &jmp label3      
     label2:
      &pushregister 7
     label3:
      &setRegister r:2
      &pop
      &pushregister 4
      &jnz label4      
      &pushsdb 9							//'_primary'
      &jmp label5      
     label4:
      &pushsdb 10							//'_bonus'
     label5:
      &setRegister r:3
      &pop
      &pushregister 3
      &pushsdbgv 2							//'popUpMC'
      &pushsdbgm 3							//'outerFrameMC'
      &pushbyte 2
      &dcallfp 4							// SetOuterFrameState()
      &pushregister 3
      &pushsdbgv 2							//'popUpMC'
      &pushsdbgm 5							//'innerFrameMC'
      &pushbyte 2
      &dcallfp 6							// SetInnerFrameState()
      &pushregister 8
      &pushregister 3
      &add
      &pushone
      &pushsdbgv 2							//'popUpMC'
      &pushsdbgm 7							//'symbolMC'
      &dcallmp 8							// gotoAndPlay()
     label6:
      &pushsdbgv 2							//'popUpMC'
      &pushsdbgm 11							//'titleTF'
      &pushsdb 12							//'textColor'
      &pushregister 2
      &setMember
      &pushsdbgv 2							//'popUpMC'
      &pushsdbgm 13							//'dividerMC'
      &pushone
      &pushsdb 14							//'Color'
      &new
      &setRegister r:6
      &pop
      &pushregister 2
      &pushone
      &pushregister 6
      &dcallmp 15							// setRGB()
    &end // of function SetObjectiveType

    &function Close (    )    
      &pushsdbgv 16							//'closing'
      &not
      &not
      &jnz label7      
      &gotoLabel '_close'
      &play
      &pushsdb 16							//'closing'
      &pushtrue
      &setVariable
     label7:
    &end // of function Close

    &function2 AdjustOuterFrameHeight (r:1='frameMC', r:2='heightChange') ()
      &pushregister 1
      &pushsdbgm 17							//'bl'
      &pushsdb 18							//'_y'
      &pushregister 1
      &pushsdbgm 17							//'bl'
      &pushsdbgm 18							//'_y'
      &pushregister 2
      &add
      &setMember
      &pushregister 1
      &pushsdbgm 19							//'bottom'
      &pushsdb 18							//'_y'
      &pushregister 1
      &pushsdbgm 19							//'bottom'
      &pushsdbgm 18							//'_y'
      &pushregister 2
      &add
      &setMember
      &pushregister 1
      &pushsdbgm 20							//'br'
      &pushsdb 18							//'_y'
      &pushregister 1
      &pushsdbgm 20							//'br'
      &pushsdbgm 18							//'_y'
      &pushregister 2
      &add
      &setMember
      &pushregister 1
      &pushsdbgm 21							//'left'
      &pushsdb 22							//'_height'
      &pushregister 1
      &pushsdbgm 21							//'left'
      &pushsdbgm 22							//'_height'
      &pushregister 2
      &add
      &setMember
      &pushregister 1
      &pushsdbgm 23							//'right'
      &pushsdb 22							//'_height'
      &pushregister 1
      &pushsdbgm 23							//'right'
      &pushsdbgm 22							//'_height'
      &pushregister 2
      &add
      &setMember
      &pushregister 1
      &pushsdbgm 24							//'bg'
      &pushsdb 22							//'_height'
      &pushregister 1
      &pushsdbgm 24							//'bg'
      &pushsdbgm 22							//'_height'
      &pushregister 2
      &add
      &setMember
    &end // of function AdjustOuterFrameHeight

    &function2 SetOuterFrameState (r:1='frameMC', r:2='stateLabel') ()
      &pushregister 2
      &pushone
      &pushregister 1
      &pushsdbgm 25							//'tl'
      &dcallmp 8							// gotoAndPlay()
      &pushregister 2
      &pushone
      &pushregister 1
      &pushsdbgm 26							//'top'
      &dcallmp 8							// gotoAndPlay()
      &pushregister 2
      &pushone
      &pushregister 1
      &pushsdbgm 27							//'tr'
      &dcallmp 8							// gotoAndPlay()
      &pushregister 2
      &pushone
      &pushregister 1
      &pushsdbgm 23							//'right'
      &dcallmp 8							// gotoAndPlay()
      &pushregister 2
      &pushone
      &pushregister 1
      &pushsdbgm 20							//'br'
      &dcallmp 8							// gotoAndPlay()
      &pushregister 2
      &pushone
      &pushregister 1
      &pushsdbgm 19							//'bottom'
      &dcallmp 8							// gotoAndPlay()
      &pushregister 2
      &pushone
      &pushregister 1
      &pushsdbgm 17							//'bl'
      &dcallmp 8							// gotoAndPlay()
      &pushregister 2
      &pushone
      &pushregister 1
      &pushsdbgm 21							//'left'
      &dcallmp 8							// gotoAndPlay()
    &end // of function SetOuterFrameState

    &function2 AdjustInnerFrameHeight (r:1='frameMC', r:2='heightChange') ()
      &pushregister 1
      &pushsdbgm 19							//'bottom'
      &pushsdb 18							//'_y'
      &pushregister 1
      &pushsdbgm 19							//'bottom'
      &pushsdbgm 18							//'_y'
      &pushregister 2
      &add
      &setMember
      &pushregister 1
      &pushsdbgm 21							//'left'
      &pushsdb 22							//'_height'
      &pushregister 1
      &pushsdbgm 21							//'left'
      &pushsdbgm 22							//'_height'
      &pushregister 2
      &add
      &setMember
      &pushregister 1
      &pushsdbgm 23							//'right'
      &pushsdb 22							//'_height'
      &pushregister 1
      &pushsdbgm 23							//'right'
      &pushsdbgm 22							//'_height'
      &pushregister 2
      &add
      &setMember
      &pushregister 1
      &pushsdbgm 24							//'bg'
      &pushsdb 22							//'_height'
      &pushregister 1
      &pushsdbgm 24							//'bg'
      &pushsdbgm 22							//'_height'
      &pushregister 2
      &add
      &setMember
    &end // of function AdjustInnerFrameHeight

    &function2 SetInnerFrameState (r:1='frameMC', r:2='stateLabel') ()
      &pushregister 2
      &pushone
      &pushregister 1
      &pushsdbgm 26							//'top'
      &dcallmp 8							// gotoAndPlay()
      &pushregister 2
      &pushone
      &pushregister 1
      &pushsdbgm 23							//'right'
      &dcallmp 8							// gotoAndPlay()
      &pushregister 2
      &pushone
      &pushregister 1
      &pushsdbgm 19							//'bottom'
      &dcallmp 8							// gotoAndPlay()
      &pushregister 2
      &pushone
      &pushregister 1
      &pushsdbgm 21							//'left'
      &dcallmp 8							// gotoAndPlay()
    &end // of function SetInnerFrameState

    &function onUnload (    )    
      &pushsdb 28							//'SetObjectiveType'
      &delete2
      &pop
      &pushsdb 29							//'Close'
      &delete2
      &pop
      &pushsdb 30							//'AdjustOuterFrameHeight'
      &delete2
      &pop
      &pushsdb 4							//'SetOuterFrameState'
      &delete2
      &pop
      &pushsdb 31							//'AdjustInnerFrameHeight'
      &delete2
      &pop
      &pushsdb 6							//'SetInnerFrameState'
      &delete2
      &pop
      &pushsdb 32							//'onUnload'
      &delete2
      &pop
    &end // of function onUnload

    &pushsdbgv 33							//'initialized'
    &not
    &not
    &jnz label9    
    &pushthisgv
    &pushone
    &pushsdbgv 35							//'_parent'
    &dcallmp 36							// OnContentLoaded()
    &pushsdb 37							//'origTextHeight'
    &pushsdbgv 2							//'popUpMC'
    &pushsdbgm 38							//'textTF'
    &pushsdbgm 22							//'_height'
    &varEquals
    &pushsdbgv 2							//'popUpMC'
    &pushsdbgm 11							//'titleTF'
    &pushsdb 39							//'text'
    &pushsdb 40							//'$'
    &pushsdb 41							//'Title'
    &pushthisgv
    &pushbyte 2
    &pushglobalgv
    &pushsdb 43							//'qualifyName'
    &callMethod
    &add
    &pushsdb 44							//'&outline'
    &add
    &setMember
    &pushsdbgv 2							//'popUpMC'
    &pushsdbgm 38							//'textTF'
    &pushsdb 45							//'autoSize'
    &pushsdb 21							//'left'
    &setMember
    &pushsdbgv 2							//'popUpMC'
    &pushsdbgm 38							//'textTF'
    &pushsdb 39							//'text'
    &pushsdb 40							//'$'
    &pushsdb 46							//'Text'
    &pushthisgv
    &pushbyte 2
    &pushglobalgv
    &pushsdb 43							//'qualifyName'
    &callMethod
    &add
    &pushsdb 44							//'&outline'
    &add
    &setMember
    &pushsdb 47							//'newTextHeight'
    &pushsdbgv 2							//'popUpMC'
    &pushsdbgm 38							//'textTF'
    &pushsdbgm 22							//'_height'
    &varEquals
    &pushsdbgv 47							//'newTextHeight'
    &pushsdbgv 37							//'origTextHeight'
    &greaterThan
    &not
    &jnz label8    
    &pushsdb 48							//'heightIncrease'
    &pushsdbgv 47							//'newTextHeight'
    &pushsdbgv 37							//'origTextHeight'
    &subtract
    &varEquals
    &pushsdbgv 48							//'heightIncrease'
    &pushsdbgv 2							//'popUpMC'
    &pushsdbgm 3							//'outerFrameMC'
    &pushbyte 2
    &dcallfp 30							// AdjustOuterFrameHeight()
    &pushsdbgv 48							//'heightIncrease'
    &pushsdbgv 2							//'popUpMC'
    &pushsdbgm 5							//'innerFrameMC'
    &pushbyte 2
    &dcallfp 31							// AdjustInnerFrameHeight()
   label8:
    &pushsdb 16							//'closing'
    &pushfalse
    &setVariable
    &pushsdb 33							//'initialized'
    &pushtrue
    &setVariable
    &pushsdb 49							//'FSCommand:'
    &pushsdb 50							//'OnInitialized'
    &pushthisgv
    &pushbyte 2
    &pushsdb 43							//'qualifyName'
    &callFunction
    &concat
    &pushsdb 51							//''
    &getURL2
   label9:
  &end // of frame 0

  &defineMovieClip 2 // total frames: 1
  &end // of defineMovieClip 2

  &defineMovieClip 9 // total frames: 30

    &frame 9
      &stop
    &end // of frame 9

    &frame 19
      &stop
    &end // of frame 19

    &frame 29
      &stop
    &end // of frame 29
  &end // of defineMovieClip 9

  &defineMovieClip 16 // total frames: 30

    &frame 9
      &stop
    &end // of frame 9

    &frame 19
      &stop
    &end // of frame 19

    &frame 29
      &stop
    &end // of frame 29
  &end // of defineMovieClip 16

  &defineMovieClip 17 // total frames: 1
  &end // of defineMovieClip 17

  &defineMovieClip 19 // total frames: 1
  &end // of defineMovieClip 19

  &defineMovieClip 22 // total frames: 1
  &end // of defineMovieClip 22

  &defineMovieClip 23 // total frames: 30

    &frame 9
      &stop
    &end // of frame 9

    &frame 19
      &stop
    &end // of frame 19

    &frame 29
      &stop
    &end // of frame 29
  &end // of defineMovieClip 23

  &defineMovieClip 24 // total frames: 2
  &end // of defineMovieClip 24

  &defineMovieClip 49 // total frames: 60

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
      &stop
    &end // of frame 29

    &frame 34
      &stop
    &end // of frame 34

    &frame 39
      &stop
    &end // of frame 39

    &frame 44
      &stop
    &end // of frame 44

    &frame 49
      &stop
    &end // of frame 49

    &frame 54
      &stop
    &end // of frame 54

    &frame 59
      &stop
    &end // of frame 59
  &end // of defineMovieClip 49

  &defineMovieClip 54 // total frames: 1
  &end // of defineMovieClip 54

  &frame 126
    &pushs 'closing'
    &pushtrue
    &setVariable
  &end // of frame 126

  &frame 135
    &pushs 'FSCommand:'
    &pushs 'OnClosed'
    &pushthisgv
    &pushbyte 2
    &pushs 'qualifyName'
    &callFunction
    &concat
    &pushs ''
    &getURL2
    &stop
  &end // of frame 135
&end
