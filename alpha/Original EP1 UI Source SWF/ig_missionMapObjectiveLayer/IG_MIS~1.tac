movie 'E:\depot\CnC\RA3_EP1\PRODUC~1\Data\APTBUI~1\034A3~1.0-D\pc\Output\IG_MIS~3\\IG_MIS~1.eaf' &compressed // flash 7, total frames: 1, frame rate: 30 fps, 1024x768 px

  &defineMovieClip 5 // total frames: 1
  &end // of defineMovieClip 5

  &defineMovieClip 6 // total frames: 1

    &frame 0
      &constants 'MoveIndicator', 'arrowMC', '_rotation', 'initialized', 'FSCommand:', 'OnInitialized', 'qualifyName', '', 'OnClipDestroyed', 'Move', 'SetOrientation', 'OnBound', 'OnDestroying', 'onUnload', 'this'  
      &function2 Move (r:4='newX', r:3='newY') (r:1='this', r:2='_parent')
        &pushregister 3
        &pushregister 4
        &pushregister 1
        &pushbyte 3
        &pushregister 2
        &dcallmp 0							// MoveIndicator()
      &end // of function Move

      &function2 SetOrientation (r:1='orient') ()
        &pushsdbgv 1							//'arrowMC'
        &pushsdb 2							//'_rotation'
        &pushregister 1
        &pushshort 180
        &multiply
        &pushfloat 3.141592741012573
        &divide
        &setMember
      &end // of function SetOrientation

      &function2 OnBound () (r:1='this')
        &pushsdbgv 3							//'initialized'
        &not
        &jnz label1        
        &pushsdb 4							//'FSCommand:'
        &pushsdb 5							//'OnInitialized'
        &pushregister 1
        &pushbyte 2
        &pushsdb 6							//'qualifyName'
        &callFunction
        &concat
        &pushsdb 7							//''
        &getURL2
       label1:
      &end // of function OnBound

      &function2 OnDestroying () (r:1='this')
        &pushsdbgv 3							//'initialized'
        &not
        &jnz label2        
        &pushsdb 4							//'FSCommand:'
        &pushsdb 8							//'OnClipDestroyed'
        &pushregister 1
        &pushbyte 2
        &pushsdb 6							//'qualifyName'
        &callFunction
        &concat
        &pushsdb 7							//''
        &getURL2
       label2:
      &end // of function OnDestroying

      &function onUnload (      )      
        &pushsdb 9							//'Move'
        &delete2
        &pop
        &pushsdb 10							//'SetOrientation'
        &delete2
        &pop
        &pushsdb 11							//'OnBound'
        &delete2
        &pop
        &pushsdb 12							//'OnDestroying'
        &delete2
        &pop
        &pushsdb 13							//'onUnload'
        &delete2
        &pop
      &end // of function onUnload

      &pushsdbgv 3							//'initialized'
      &not
      &not
      &jnz label3      
      &pushsdb 3							//'initialized'
      &pushtrue
      &setVariable
      &pushsdb 4							//'FSCommand:'
      &pushsdb 5							//'OnInitialized'
      &pushthisgv
      &pushbyte 2
      &pushsdb 6							//'qualifyName'
      &callFunction
      &concat
      &pushsdb 7							//''
      &getURL2
     label3:
    &end // of frame 0
  &end // of defineMovieClip 6
  
  &exportAssets
    6 &as 'HeroIndicator'
  &end // of exportAssets

  &defineButton 8
  
    &on     &overDownToOverUp
      &pushzero
      &pushs 'OnClicked'
      &callfp
    &end
  &end // of defineButton 8

  &defineMovieClip 21 // total frames: 30

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
  &end // of defineMovieClip 21

  &defineMovieClip 34 // total frames: 35

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
  &end // of defineMovieClip 34

  &defineMovieClip 37 // total frames: 1
  &end // of defineMovieClip 37

  &defineMovieClip 38 // total frames: 1

    &frame 0
      &constants 'MoveIndicator', 'isSelected', '', 'getDepth', 'swapDepths', 'initialized', 'FSCommand:', 'OnInitialized', 'qualifyName', 'OnClipDestroyed', 'ObjectiveIndicator OnClicked(), this = ', 'OnClicked', 'Move', 'SetSelected', 'SetVisibility', 'OnBound', 'OnDestroying', 'onUnload', 'textMC', 'text', '$', 'Ordinal', 'this', '_root', 'GetCurrentFaction', 'Yuriko', 'primary', 'gotoAndPlay', 'bonus', 'category', 'isBonus', '_visible'  
      &function2 Move (r:4='newX', r:3='newY') (r:1='this', r:2='_parent')
        &pushregister 3
        &pushregister 4
        &pushregister 1
        &pushbyte 3
        &pushregister 2
        &dcallmp 0							// MoveIndicator()
      &end // of function Move

      &function2 SetSelected (r:3='val') ()
        &pushregister 3
        &pushzero
        &equals
        &not
        &setRegister r:1
        &pop
        &pushregister 1
        &pushsdbgv 1							//'isSelected'
        &equals
        &not
        &not
        &jnz label5        
        &pushsdb 1							//'isSelected'
        &pushregister 1
        &setVariable
        &pushsdb 2							//''
        &pushbyte 6
        &pushregister 1
        &jnz label1        
        &pushbyte 50
        &jmp label2        
       label1:
        &pushbyte 100
       label2:
        &setProperty
        &pushzero
        &pushsdb 3							//'getDepth'
        &callFunction
        &setRegister r:2
        &pop
        &pushregister 1
        &not
        &jnz label3        
        &pushregister 2
        &pushshort 1000
        &add
        &setRegister r:2
        &pop
        &jmp label4        
       label3:
        &pushregister 2
        &pushshort 1000
        &subtract
        &setRegister r:2
        &pop
       label4:
        &pushregister 2
        &pushone
        &dcallfp 4							// swapDepths()
       label5:
      &end // of function SetSelected

      &function2 SetVisibility (r:1='val') ()
        &pushsdb 2							//''
        &pushbyte 7
        &pushregister 1
        &pushzero
        &equals
        &not
        &setProperty
      &end // of function SetVisibility

      &function2 OnBound () (r:1='this')
        &pushsdbgv 5							//'initialized'
        &not
        &jnz label6        
        &pushsdb 6							//'FSCommand:'
        &pushsdb 7							//'OnInitialized'
        &pushregister 1
        &pushbyte 2
        &pushsdb 8							//'qualifyName'
        &callFunction
        &concat
        &pushsdb 2							//''
        &getURL2
       label6:
      &end // of function OnBound

      &function2 OnDestroying () (r:1='this')
        &pushsdbgv 5							//'initialized'
        &not
        &jnz label7        
        &pushsdb 6							//'FSCommand:'
        &pushsdb 9							//'OnClipDestroyed'
        &pushregister 1
        &pushbyte 2
        &pushsdb 8							//'qualifyName'
        &callFunction
        &concat
        &pushsdb 2							//''
        &getURL2
       label7:
      &end // of function OnDestroying

      &function2 OnClicked () (r:1='this')
        &pushsdb 10							//'ObjectiveIndicator OnClicked(), this = '
        &pushregister 1
        &toString
        &add
        &trace
        &pushsdb 6							//'FSCommand:'
        &pushsdb 11							//'OnClicked'
        &pushregister 1
        &pushbyte 2
        &pushsdb 8							//'qualifyName'
        &callFunction
        &concat
        &pushsdb 2							//''
        &getURL2
      &end // of function OnClicked

      &function onUnload (      )      
        &pushsdb 12							//'Move'
        &delete2
        &pop
        &pushsdb 13							//'SetSelected'
        &delete2
        &pop
        &pushsdb 14							//'SetVisibility'
        &delete2
        &pop
        &pushsdb 15							//'OnBound'
        &delete2
        &pop
        &pushsdb 16							//'OnDestroying'
        &delete2
        &pop
        &pushsdb 11							//'OnClicked'
        &delete2
        &pop
        &pushsdb 17							//'onUnload'
        &delete2
        &pop
      &end // of function onUnload

      &pushsdbgv 5							//'initialized'
      &not
      &not
      &jnz label12      
      &pushsdbgv 18							//'textMC'
      &pushsdb 19							//'text'
      &pushsdb 20							//'$'
      &pushsdb 21							//'Ordinal'
      &pushthisgv
      &pushbyte 2
      &pushsdb 8							//'qualifyName'
      &callFunction
      &add
      &setMember
      &pushzero
      &pushsdbgv 23							//'_root'
      &pushsdb 24							//'GetCurrentFaction'
      &callMethod
      &pushsdb 25							//'Yuriko'
      &equals
      &not
      &jnz label8      
      &pushsdb 25							//'Yuriko'
      &pushone
      &pushsdbgv 26							//'primary'
      &dcallmp 27							// gotoAndPlay()
      &pushsdb 25							//'Yuriko'
      &pushone
      &pushsdbgv 28							//'bonus'
      &dcallmp 27							// gotoAndPlay()
      &jmp label9      
     label8:
      &pushsdbgv 29							//'category'
      &pushone
      &pushsdbgv 26							//'primary'
      &dcallmp 27							// gotoAndPlay()
      &pushsdbgv 29							//'category'
      &pushone
      &pushsdbgv 28							//'bonus'
      &dcallmp 27							// gotoAndPlay()
     label9:
      &pushsdbgv 30							//'isBonus'
      &not
      &jnz label10      
      &pushsdbgv 26							//'primary'
      &pushsdb 31							//'_visible'
      &pushfalse
      &setMember
      &jmp label11      
     label10:
      &pushsdbgv 28							//'bonus'
      &pushsdb 31							//'_visible'
      &pushfalse
      &setMember
     label11:
      &pushsdb 1							//'isSelected'
      &pushfalse
      &setVariable
      &pushsdb 2							//''
      &pushbyte 6
      &pushbyte 50
      &setProperty
      &pushsdb 5							//'initialized'
      &pushtrue
      &setVariable
      &pushsdb 6							//'FSCommand:'
      &pushsdb 7							//'OnInitialized'
      &pushthisgv
      &pushbyte 2
      &pushsdb 8							//'qualifyName'
      &callFunction
      &concat
      &pushsdb 2							//''
      &getURL2
     label12:
    &end // of frame 0
  &end // of defineMovieClip 38
  
  &exportAssets
    38 &as 'ObjectiveIndicator'
  &end // of exportAssets

  &frame 0
    &constants 'nextIndicatorId', 'ObjectiveIndicator', 'attachMovie', 'category', 'isBonus', 'relX', 'relY', 'indicators', 'push', 'Pointer', 'encode', 'HeroIndicator', 'length', 'splice', 'removeMovieClip', '_x', 'sizer', '_width', '_y', '_height', 'initialized', 'FSCommand:', 'OnInitialized', 'qualifyName', '', 'OnDestroying', 'OnClipDestroyed', 'AddIndicator', 'AddHeroIndicator', 'RemoveIndicator', 'MoveIndicator', 'OnBound', 'onUnload', 'this', '_parent', 'OnContentLoaded', 'Array'  
    &function2 AddIndicator (r:5='category', r:4='isBonus') (r:1='this')
      &pushsdbgv 0							//'nextIndicatorId'
      &setRegister r:3
      &pop
      &pushsdb 0							//'nextIndicatorId'
      &pushsdbgv 0							//'nextIndicatorId'
      &increment
      &setVariable
      &pushregister 3
      &pushregister 3
      &toString
      &pushsdb 1							//'ObjectiveIndicator'
      &pushbyte 3
      &pushregister 1
      &pushsdb 2							//'attachMovie'
      &callMethod
      &setRegister r:2
      &pop
      &pushregister 2
      &pushsdb 3							//'category'
      &pushregister 5
      &setMember
      &pushregister 2
      &pushsdb 4							//'isBonus'
      &pushregister 4
      &pushzero
      &equals
      &not
      &setMember
      &pushregister 2
      &pushsdb 5							//'relX'
      &pushzero
      &setMember
      &pushregister 2
      &pushsdb 6							//'relY'
      &pushzero
      &setMember
      &pushregister 2
      &pushone
      &pushsdbgv 7							//'indicators'
      &dcallmp 8							// push()
      &pushregister 2
      &pushone
      &pushsdbgv 9							//'Pointer'
      &pushsdb 10							//'encode'
      &callMethod
      &return
    &end // of function AddIndicator

    &function2 AddHeroIndicator () (r:1='this')
      &pushsdbgv 0							//'nextIndicatorId'
      &setRegister r:3
      &pop
      &pushsdb 0							//'nextIndicatorId'
      &pushsdbgv 0							//'nextIndicatorId'
      &increment
      &setVariable
      &pushregister 3
      &pushregister 3
      &toString
      &pushsdb 11							//'HeroIndicator'
      &pushbyte 3
      &pushregister 1
      &pushsdb 2							//'attachMovie'
      &callMethod
      &setRegister r:2
      &pop
      &pushregister 2
      &pushsdb 5							//'relX'
      &pushzero
      &setMember
      &pushregister 2
      &pushsdb 6							//'relY'
      &pushzero
      &setMember
      &pushregister 2
      &pushone
      &pushsdbgv 7							//'indicators'
      &dcallmp 8							// push()
      &pushregister 2
      &pushone
      &pushsdbgv 9							//'Pointer'
      &pushsdb 10							//'encode'
      &callMethod
      &return
    &end // of function AddHeroIndicator

    &function2 RemoveIndicator (r:3='indicator') ()
      &pushsdbgv 7							//'indicators'
      &pushsdbgm 12							//'length'
      &setRegister r:2
      &pop
      &pushzero
      &setRegister r:1
      &pop
     label1:
      &pushregister 1
      &pushregister 2
      &lessThan
      &not
      &jnz label4      
      &pushregister 3
      &pushsdbgv 7							//'indicators'
      &pushregister 1
      &getMember
      &equals
      &not
      &jnz label3      
      &pushone
      &pushregister 1
      &pushbyte 2
      &pushsdbgv 7							//'indicators'
      &dcallmp 13							// splice()
      &pushsdbgv 7							//'indicators'
      &pushsdbgm 12							//'length'
      &pushzero
      &equals
      &not
      &jnz label2      
      &pushsdb 0							//'nextIndicatorId'
      &pushzerosv
     label2:
      &jmp label4      
     label3:
      &pushregister 1
      &increment
      &setRegister r:1
      &pop
      &jmp label1      
     label4:
      &pushzero
      &pushregister 3
      &dcallmp 14							// removeMovieClip()
    &end // of function RemoveIndicator

    &function2 MoveIndicator (r:1='indicator', r:3='newX', r:2='newY') ()
      &pushregister 1
      &pushsdb 5							//'relX'
      &pushregister 3
      &setMember
      &pushregister 1
      &pushsdb 6							//'relY'
      &pushregister 2
      &setMember
      &pushregister 1
      &pushsdb 15							//'_x'
      &pushsdbgv 16							//'sizer'
      &pushsdbgm 17							//'_width'
      &pushregister 3
      &multiply
      &setMember
      &pushregister 1
      &pushsdb 18							//'_y'
      &pushsdbgv 16							//'sizer'
      &pushsdbgm 19							//'_height'
      &pushregister 2
      &multiply
      &setMember
    &end // of function MoveIndicator

    &function2 Resize (r:5='newWidth', r:4='newHeight') ()
      &pushsdbgv 16							//'sizer'
      &pushsdb 17							//'_width'
      &pushregister 5
      &setMember
      &pushsdbgv 16							//'sizer'
      &pushsdb 19							//'_height'
      &pushregister 4
      &setMember
      &pushsdbgv 7							//'indicators'
      &pushsdbgm 12							//'length'
      &setRegister r:3
      &pop
      &pushzero
      &setRegister r:2
      &pop
     label5:
      &pushregister 2
      &pushregister 3
      &lessThan
      &not
      &jnz label6      
      &pushsdbgv 7							//'indicators'
      &pushregister 2
      &getMember
      &setRegister r:1
      &pop
      &pushregister 1
      &pushsdb 15							//'_x'
      &pushregister 1
      &pushsdbgm 5							//'relX'
      &pushregister 5
      &multiply
      &setMember
      &pushregister 1
      &pushsdb 18							//'_y'
      &pushregister 1
      &pushsdbgm 6							//'relY'
      &pushregister 4
      &multiply
      &setMember
      &pushregister 2
      &increment
      &setRegister r:2
      &pop
      &jmp label5      
     label6:
    &end // of function Resize

    &function2 OnBound () (r:1='this')
      &pushsdbgv 20							//'initialized'
      &not
      &jnz label7      
      &pushsdb 21							//'FSCommand:'
      &pushsdb 22							//'OnInitialized'
      &pushregister 1
      &pushbyte 2
      &pushsdb 23							//'qualifyName'
      &callFunction
      &concat
      &pushsdb 24							//''
      &getURL2
     label7:
    &end // of function OnBound

    &function2 OnDestroying () (r:1='this')
      &pushsdbgv 7							//'indicators'
      &pushsdbgm 12							//'length'
      &setRegister r:3
      &pop
      &pushzero
      &setRegister r:2
      &pop
     label8:
      &pushregister 2
      &pushregister 3
      &lessThan
      &not
      &jnz label9      
      &pushzero
      &pushsdbgv 7							//'indicators'
      &pushregister 2
      &getMember
      &dcallmp 25							// OnDestroying()
      &pushregister 2
      &increment
      &setRegister r:2
      &pop
      &jmp label8      
     label9:
      &pushsdbgv 20							//'initialized'
      &not
      &jnz label10      
      &pushsdb 21							//'FSCommand:'
      &pushsdb 26							//'OnClipDestroyed'
      &pushregister 1
      &pushbyte 2
      &pushsdb 23							//'qualifyName'
      &callFunction
      &concat
      &pushsdb 24							//''
      &getURL2
     label10:
    &end // of function OnDestroying

    &function onUnload (    )    
      &pushsdb 27							//'AddIndicator'
      &delete2
      &pop
      &pushsdb 28							//'AddHeroIndicator'
      &delete2
      &pop
      &pushsdb 29							//'RemoveIndicator'
      &delete2
      &pop
      &pushsdb 30							//'MoveIndicator'
      &delete2
      &pop
      &pushsdb 31							//'OnBound'
      &delete2
      &pop
      &pushsdb 25							//'OnDestroying'
      &delete2
      &pop
      &pushsdb 32							//'onUnload'
      &delete2
      &pop
    &end // of function onUnload

    &pushsdbgv 20							//'initialized'
    &not
    &not
    &jnz label11    
    &pushthisgv
    &pushone
    &pushsdbgv 34							//'_parent'
    &dcallmp 35							// OnContentLoaded()
    &pushsdb 7							//'indicators'
    &pushzero
    &pushsdb 36							//'Array'
    &new
    &setVariable
    &pushsdb 0							//'nextIndicatorId'
    &pushzerosv
    &pushsdb 20							//'initialized'
    &pushtrue
    &setVariable
    &pushsdb 21							//'FSCommand:'
    &pushsdb 22							//'OnInitialized'
    &pushthisgv
    &pushbyte 2
    &pushsdb 23							//'qualifyName'
    &callFunction
    &concat
    &pushsdb 24							//''
    &getURL2
   label11:
  &end // of frame 0

  &defineMovieClip 40 // total frames: 1
  &end // of defineMovieClip 40

  &placeMovieClip 40 &as 'sizer'
  
    &onClipEvent &load
      &pushs ''
      &pushbyte 7
      &pushfalse
      &setProperty
    &end
  &end // of placeMovieClip 40
&end
