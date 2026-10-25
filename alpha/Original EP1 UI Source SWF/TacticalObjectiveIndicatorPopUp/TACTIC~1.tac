movie 'E:\depot\CnC\RA3_EP1\PRODUC~1\Data\APTBUI~1\034A3~1.0-D\pc\Output\TAC739~1\\TACTIC~1.eaf' &compressed // flash 7, total frames: 9, frame rate: 30 fps, 1024x768 px

  &frame 0
    &constants 'Close', 'onUnload', 'initialized', 'this', '_parent', 'OnContentLoaded', 'FSCommand:', 'OnInitialized', 'qualifyName', ''  
    &function Close (    )    
      &gotoLabel '_close'
      &play
    &end // of function Close

    &function onUnload (    )    
      &pushsdb 0							//'Close'
      &delete2
      &pop
      &pushsdb 1							//'onUnload'
      &delete2
      &pop
    &end // of function onUnload

    &pushsdbgv 2							//'initialized'
    &not
    &not
    &jnz label1    
    &pushthisgv
    &pushone
    &pushsdbgv 4							//'_parent'
    &dcallmp 5							// OnContentLoaded()
    &pushsdb 2							//'initialized'
    &pushtrue
    &setVariable
    &pushsdb 6							//'FSCommand:'
    &pushsdb 7							//'OnInitialized'
    &pushthisgv
    &pushbyte 2
    &pushsdb 8							//'qualifyName'
    &callFunction
    &concat
    &pushsdb 9							//''
    &getURL2
   label1:
  &end // of frame 0

  &defineButton 2
  
    &on     &overDownToOverUp
      &constants 'FSCommand:', 'OnClicked', '_parent', 'qualifyName', ''  
      &pushsdb 0							//'FSCommand:'
      &pushsdb 1							//'OnClicked'
      &pushsdbgv 2							//'_parent'
      &pushsdbgm 2							//'_parent'
      &pushbyte 2
      &pushsdb 3							//'qualifyName'
      &callFunction
      &concat
      &pushsdb 4							//''
      &getURL2
    &end
  &end // of defineButton 2

  &defineMovieClip 4 // total frames: 1
  &end // of defineMovieClip 4

  &defineMovieClip 5 // total frames: 1

    &placeMovieClip 4 
    
      &onClipEvent &load
        &pushs ''
        &pushbyte 7
        &pushfalse
        &setProperty
      &end
    &end // of placeMovieClip 4
  &end // of defineMovieClip 5

  &defineMovieClip 8 // total frames: 1
  &end // of defineMovieClip 8

  &defineMovieClip 10 // total frames: 1
  &end // of defineMovieClip 10

  &defineMovieClip 13 // total frames: 1
  &end // of defineMovieClip 13

  &defineMovieClip 14 // total frames: 1
  &end // of defineMovieClip 14

  &defineMovieClip 16 // total frames: 1
  &end // of defineMovieClip 16

  &defineMovieClip 18 // total frames: 1
  &end // of defineMovieClip 18

  &defineMovieClip 20 // total frames: 1
  &end // of defineMovieClip 20

  &defineMovieClip 21 // total frames: 1
  &end // of defineMovieClip 21

  &defineMovieClip 23 // total frames: 1
  &end // of defineMovieClip 23

  &defineMovieClip 25 // total frames: 1
  &end // of defineMovieClip 25

  &defineMovieClip 26 // total frames: 1
  &end // of defineMovieClip 26

  &defineMovieClip 27 // total frames: 1
  &end // of defineMovieClip 27

  &defineMovieClip 31 // total frames: 1

    &frame 0
      &constants 'initialized', 'titleTF', 'text', '$', 'Title', '_parent', '_global', 'qualifyName', '&outline', 'oldDescHeight', 'descTF', '_height', 'autoSize', 'left', 'Description', 'newDescHeight', 'heightIncrease', 'borderMC', 'topMC', '_y', 'middleMC', 'backgroundMC', 'buttonMC'  
      &pushsdbgv 0							//'initialized'
      &not
      &not
      &jnz label2      
      &pushsdbgv 1							//'titleTF'
      &pushsdb 2							//'text'
      &pushsdb 3							//'$'
      &pushsdb 4							//'Title'
      &pushsdbgv 5							//'_parent'
      &pushbyte 2
      &pushglobalgv
      &pushsdb 7							//'qualifyName'
      &callMethod
      &add
      &pushsdb 8							//'&outline'
      &add
      &setMember
      &pushsdb 9							//'oldDescHeight'
      &pushsdbgv 10							//'descTF'
      &pushsdbgm 11							//'_height'
      &varEquals
      &pushsdbgv 10							//'descTF'
      &pushsdb 12							//'autoSize'
      &pushsdb 13							//'left'
      &setMember
      &pushsdbgv 10							//'descTF'
      &pushsdb 2							//'text'
      &pushsdb 3							//'$'
      &pushsdb 14							//'Description'
      &pushsdbgv 5							//'_parent'
      &pushbyte 2
      &pushglobalgv
      &pushsdb 7							//'qualifyName'
      &callMethod
      &add
      &pushsdb 8							//'&outline'
      &add
      &setMember
      &pushsdb 15							//'newDescHeight'
      &pushsdbgv 10							//'descTF'
      &pushsdbgm 11							//'_height'
      &varEquals
      &pushsdbgv 15							//'newDescHeight'
      &pushsdbgv 9							//'oldDescHeight'
      &greaterThan
      &not
      &jnz label1      
      &pushsdb 16							//'heightIncrease'
      &pushsdbgv 15							//'newDescHeight'
      &pushsdbgv 9							//'oldDescHeight'
      &subtract
      &varEquals
      &pushsdbgv 17							//'borderMC'
      &pushsdbgm 18							//'topMC'
      &pushsdb 19							//'_y'
      &pushsdbgv 17							//'borderMC'
      &pushsdbgm 18							//'topMC'
      &pushsdbgm 19							//'_y'
      &pushsdbgv 16							//'heightIncrease'
      &subtract
      &setMember
      &pushsdbgv 17							//'borderMC'
      &pushsdbgm 20							//'middleMC'
      &pushsdb 19							//'_y'
      &pushsdbgv 17							//'borderMC'
      &pushsdbgm 20							//'middleMC'
      &pushsdbgm 19							//'_y'
      &pushsdbgv 16							//'heightIncrease'
      &subtract
      &setMember
      &pushsdbgv 17							//'borderMC'
      &pushsdbgm 20							//'middleMC'
      &pushsdb 11							//'_height'
      &pushsdbgv 17							//'borderMC'
      &pushsdbgm 20							//'middleMC'
      &pushsdbgm 11							//'_height'
      &pushsdbgv 16							//'heightIncrease'
      &add
      &setMember
      &pushsdbgv 17							//'borderMC'
      &pushsdbgm 21							//'backgroundMC'
      &pushsdb 19							//'_y'
      &pushsdbgv 17							//'borderMC'
      &pushsdbgm 21							//'backgroundMC'
      &pushsdbgm 19							//'_y'
      &pushsdbgv 16							//'heightIncrease'
      &subtract
      &setMember
      &pushsdbgv 17							//'borderMC'
      &pushsdbgm 21							//'backgroundMC'
      &pushsdb 11							//'_height'
      &pushsdbgv 17							//'borderMC'
      &pushsdbgm 21							//'backgroundMC'
      &pushsdbgm 11							//'_height'
      &pushsdbgv 16							//'heightIncrease'
      &add
      &setMember
      &pushsdbgv 1							//'titleTF'
      &pushsdb 19							//'_y'
      &pushsdbgv 1							//'titleTF'
      &pushsdbgm 19							//'_y'
      &pushsdbgv 16							//'heightIncrease'
      &subtract
      &setMember
      &pushsdbgv 10							//'descTF'
      &pushsdb 19							//'_y'
      &pushsdbgv 10							//'descTF'
      &pushsdbgm 19							//'_y'
      &pushsdbgv 16							//'heightIncrease'
      &subtract
      &setMember
      &pushsdbgv 22							//'buttonMC'
      &pushsdb 19							//'_y'
      &pushsdbgv 22							//'buttonMC'
      &pushsdbgm 19							//'_y'
      &pushsdbgv 16							//'heightIncrease'
      &subtract
      &setMember
      &pushsdbgv 22							//'buttonMC'
      &pushsdb 11							//'_height'
      &pushsdbgv 22							//'buttonMC'
      &pushsdbgm 11							//'_height'
      &pushsdbgv 16							//'heightIncrease'
      &add
      &setMember
     label1:
      &pushsdb 0							//'initialized'
      &pushtrue
      &setVariable
     label2:
    &end // of frame 0
  &end // of defineMovieClip 31

  &frame 4
    &stop
  &end // of frame 4

  &frame 8
    &stop
    &pushs 'FSCommand:'
    &pushs 'OnClosed'
    &pushthisgv
    &pushbyte 2
    &pushs 'qualifyName'
    &callFunction
    &concat
    &pushs ''
    &getURL2
  &end // of frame 8
&end
