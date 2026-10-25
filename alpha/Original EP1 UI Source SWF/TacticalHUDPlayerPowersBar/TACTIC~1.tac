movie 'E:\depot\CnC\RA3_EP1\PRODUC~1\Data\APTBUI~1\034A3~1.0-D\pc\Output\TADDB0~1\\TACTIC~1.eaf' &compressed // flash 7, total frames: 1, frame rate: 30 fps, 1024x768 px

  &frame 0
    &constants 'numRowsInFirstColumn', 'buttonSlotHeight', 'GetNumRowsInColumn', 'row', 'col', 'nextButtonSlotId', 'LoadMovieStage', 'attachMovie', '_x', '_y', 'helpSite', 'HelpBoxSite', 'GetCurrentFaction', 'Yuriko', 'buttonStageWidth', 'vertAlignment', 'top', 'horzAlignment', 'left', 'stage', 'buttonSlots', 'splice', 'FSCommand:', 'OnButtonSlotStageCreated', 'qualifyName', 'index=', '&name=', 'Pointer', 'encode', 'ComputeSlotPositionFromIndex', 'YurikoYOffset', 'YurikoXOffset', 'buttonSlotWidth', 'GetColumnVerticalOffset', 'columnHorizontalOffset', 'CreateNewButtonSlot', 'length', 'MoveButtonSlot', 'removeMovieClip', 'pop', 'InsertButtonSlot', 'EraseButtonSlot', 'SetButtonSlotCount', 'onUnload', 'initialized', 'buttonStageHeight', 'Array', 'YurikoXoffset', 'YurikoYoffset', 'OnInitialized', 'this', '', 'extern', 'InGame'  
    &function GetNumRowsInColumn ('col'    )    
      &pushsdbgv 0							//'numRowsInFirstColumn'
      &return
    &end // of function GetNumRowsInColumn

    &function2 GetColumnVerticalOffset (r:1='col') ()
      &pushsdbgv 1							//'buttonSlotHeight'
      &pushsdbgv 0							//'numRowsInFirstColumn'
      &pushregister 1
      &pushone
      &pushsdb 2							//'GetNumRowsInColumn'
      &callFunction
      &subtract
      &multiply
      &pushfloat 0.500000000000000
      &multiply
      &return
    &end // of function GetColumnVerticalOffset

    &function2 ComputeSlotPositionFromIndex (r:4='index') ()
      &pushsdbgv 0							//'numRowsInFirstColumn'
      &setRegister r:1
      &pop
      &pushzero
      &setRegister r:3
      &pop
      &pushregister 4
      &setRegister r:2
      &pop
     label1:
      &pushregister 2
      &pushregister 1
      &lessThan
      &not
      &not
      &jnz label2      
      &pushregister 3
      &increment
      &setRegister r:3
      &pop
      &pushregister 2
      &pushregister 1
      &subtract
      &setRegister r:2
      &pop
      &jmp label1      
     label2:
      &pushsdb 3							//'row'
      &pushregister 2
      &pushsdb 4							//'col'
      &pushregister 3
      &pushbyte 2
      &initObject
      &return
    &end // of function ComputeSlotPositionFromIndex

    &function2 CreateNewButtonSlot (r:10='slotIndex', r:9='slotX', r:8='slotY') (r:1='this', r:2='_root')
      &pushsdbgv 5							//'nextButtonSlotId'
      &pushsdb 5							//'nextButtonSlotId'
      &pushsdbgv 5							//'nextButtonSlotId'
      &increment
      &setVariable
      &setRegister r:4
      &pop
      &pushregister 4
      &toString
      &setRegister r:6
      &pop
      &pushshort 2048
      &pushregister 4
      &subtract
      &pushregister 4
      &toString
      &pushsdb 6							//'LoadMovieStage'
      &pushbyte 3
      &dcallfp 7							// attachMovie()
      &pushregister 1
      &pushregister 6
      &getMember
      &setRegister r:5
      &pop
      &pushregister 5
      &pushsdb 8							//'_x'
      &pushregister 9
      &setMember
      &pushregister 5
      &pushsdb 9							//'_y'
      &pushregister 8
      &setMember
      &pushsdb 10							//'helpSite'
      &pushregister 4
      &toString
      &add
      &setRegister r:6
      &pop
      &pushregister 4
      &pushlong 65536
      &add
      &pushsdb 10							//'helpSite'
      &pushregister 4
      &toString
      &add
      &pushsdb 11							//'HelpBoxSite'
      &pushbyte 3
      &dcallfp 7							// attachMovie()
      &pushregister 1
      &pushregister 6
      &getMember
      &setRegister r:3
      &pop
      &pushzero
      &pushregister 2
      &pushsdb 12							//'GetCurrentFaction'
      &callMethod
      &pushsdb 13							//'Yuriko'
      &equals
      &not
      &jnz label3      
      &pushregister 3
      &pushsdb 8							//'_x'
      &pushregister 9
      &setMember
      &pushregister 3
      &pushsdb 9							//'_y'
      &pushregister 8
      &pushshort 250
      &subtract
      &setMember
      &jmp label4      
     label3:
      &pushregister 3
      &pushsdb 8							//'_x'
      &pushregister 9
      &pushsdbgv 14							//'buttonStageWidth'
      &add
      &setMember
      &pushregister 3
      &pushsdb 9							//'_y'
      &pushregister 8
      &setMember
     label4:
      &pushregister 3
      &pushsdb 15							//'vertAlignment'
      &pushsdb 16							//'top'
      &setMember
      &pushregister 3
      &pushsdb 17							//'horzAlignment'
      &pushsdb 18							//'left'
      &setMember
      &pushregister 5
      &pushsdb 10							//'helpSite'
      &pushregister 3
      &setMember
      &pushsdb 19							//'stage'
      &pushregister 5
      &pushsdb 10							//'helpSite'
      &pushregister 3
      &pushbyte 2
      &initObject
      &setRegister r:7
      &pop
      &pushregister 7
      &pushzero
      &pushregister 10
      &pushbyte 3
      &pushsdbgv 20							//'buttonSlots'
      &dcallmp 21							// splice()
      &pushsdb 22							//'FSCommand:'
      &pushsdb 23							//'OnButtonSlotStageCreated'
      &pushregister 1
      &pushbyte 2
      &pushsdb 24							//'qualifyName'
      &callFunction
      &concat
      &pushsdb 25							//'index='
      &pushregister 10
      &add
      &pushsdb 26							//'&name='
      &add
      &pushregister 5
      &pushone
      &pushsdbgv 27							//'Pointer'
      &pushsdb 28							//'encode'
      &callMethod
      &add
      &getURL2
      &pushregister 7
      &return
    &end // of function CreateNewButtonSlot

    &function2 MoveButtonSlot (r:6='slotIndex', r:5='slotX', r:4='slotY') ()
      &pushsdbgv 20							//'buttonSlots'
      &pushregister 6
      &getMember
      &setRegister r:1
      &pop
      &pushregister 1
      &pushsdbgm 19							//'stage'
      &setRegister r:3
      &pop
      &pushregister 3
      &pushsdb 8							//'_x'
      &pushregister 5
      &setMember
      &pushregister 3
      &pushsdb 9							//'_y'
      &pushregister 4
      &setMember
      &pushregister 1
      &pushsdbgm 10							//'helpSite'
      &setRegister r:2
      &pop
      &pushregister 2
      &pushsdb 8							//'_x'
      &pushregister 5
      &pushsdbgv 14							//'buttonStageWidth'
      &add
      &setMember
      &pushregister 2
      &pushsdb 9							//'_y'
      &pushregister 4
      &setMember
    &end // of function MoveButtonSlot

    &function2 InsertButtonSlot (r:4='index') (r:1='_root')
      &pushregister 4
      &pushone
      &pushsdb 29							//'ComputeSlotPositionFromIndex'
      &callFunction
      &setRegister r:8
      &pop
      &pushregister 8
      &pushsdbgm 4							//'col'
      &setRegister r:2
      &pop
      &pushregister 8
      &pushsdbgm 3							//'row'
      &setRegister r:3
      &pop
      &pushzero
      &pushregister 1
      &pushsdb 12							//'GetCurrentFaction'
      &callMethod
      &pushsdb 13							//'Yuriko'
      &equals
      &not
      &jnz label5      
      &pushsdbgv 30							//'YurikoYOffset'
      &setRegister r:5
      &pop
      &pushsdbgv 31							//'YurikoXOffset'
      &pushregister 3
      &pushsdbgv 32							//'buttonSlotWidth'
      &pushbyte 5
      &subtract
      &multiply
      &add
      &setRegister r:6
      &pop
      &jmp label6      
     label5:
      &pushregister 2
      &pushone
      &pushsdb 33							//'GetColumnVerticalOffset'
      &callFunction
      &pushregister 3
      &pushsdbgv 1							//'buttonSlotHeight'
      &multiply
      &add
      &setRegister r:5
      &pop
      &pushregister 2
      &pushsdbgv 34							//'columnHorizontalOffset'
      &multiply
      &setRegister r:6
      &pop
     label6:
      &pushregister 5
      &pushregister 6
      &pushregister 4
      &pushbyte 3
      &dcallfp 35							// CreateNewButtonSlot()
      &pushregister 2
      &pushone
      &pushsdb 2							//'GetNumRowsInColumn'
      &callFunction
      &setRegister r:7
      &pop
      &pushregister 4
      &increment
      &setRegister r:4
      &pop
     label7:
      &pushregister 4
      &pushsdbgv 20							//'buttonSlots'
      &pushsdbgm 36							//'length'
      &lessThan
      &not
      &jnz label12      
      &pushregister 3
      &increment
      &setRegister r:3
      &pop
      &pushregister 3
      &pushregister 7
      &lessThan
      &not
      &jnz label10      
      &pushzero
      &pushregister 1
      &pushsdb 12							//'GetCurrentFaction'
      &callMethod
      &pushsdb 13							//'Yuriko'
      &equals
      &not
      &jnz label8      
      &pushregister 6
      &pushsdbgv 32							//'buttonSlotWidth'
      &pushbyte 3
      &subtract
      &add
      &setRegister r:6
      &pop
      &jmp label9      
     label8:
      &pushregister 5
      &pushsdbgv 1							//'buttonSlotHeight'
      &add
      &setRegister r:5
      &pop
     label9:
      &jmp label11      
     label10:
      &pushregister 2
      &increment
      &setRegister r:2
      &pop
      &pushzero
      &setRegister r:3
      &pop
      &pushregister 2
      &pushone
      &pushsdb 33							//'GetColumnVerticalOffset'
      &callFunction
      &setRegister r:5
      &pop
      &pushregister 6
      &pushsdbgv 34							//'columnHorizontalOffset'
      &add
      &setRegister r:6
      &pop
      &pushregister 2
      &pushone
      &pushsdb 2							//'GetNumRowsInColumn'
      &callFunction
      &setRegister r:7
      &pop
     label11:
      &pushregister 5
      &pushregister 6
      &pushregister 4
      &pushbyte 3
      &dcallfp 37							// MoveButtonSlot()
      &pushregister 4
      &increment
      &setRegister r:4
      &pop
      &jmp label7      
     label12:
    &end // of function InsertButtonSlot

    &function2 EraseButtonSlot (r:1='index') ()
      &pushsdbgv 20							//'buttonSlots'
      &pushregister 1
      &getMember
      &setRegister r:9
      &pop
      &pushregister 9
      &pushsdbgm 19							//'stage'
      &setRegister r:7
      &pop
      &pushregister 1
      &pushone
      &pushsdb 29							//'ComputeSlotPositionFromIndex'
      &callFunction
      &setRegister r:8
      &pop
      &pushregister 8
      &pushsdbgm 4							//'col'
      &setRegister r:2
      &pop
      &pushregister 8
      &pushsdbgm 3							//'row'
      &setRegister r:3
      &pop
      &pushregister 7
      &pushsdbgm 8							//'_x'
      &setRegister r:5
      &pop
      &pushregister 7
      &pushsdbgm 9							//'_y'
      &setRegister r:4
      &pop
      &pushone
      &pushregister 1
      &pushbyte 2
      &pushsdbgv 20							//'buttonSlots'
      &dcallmp 21							// splice()
      &pushzero
      &pushregister 7
      &dcallmp 38							// removeMovieClip()
      &pushzero
      &pushregister 9
      &pushsdbgm 10							//'helpSite'
      &dcallmp 38							// removeMovieClip()
      &pushsdbgv 20							//'buttonSlots'
      &pushsdbgm 36							//'length'
      &pushzero
      &greaterThan
      &not
      &jnz label18      
      &pushregister 1
      &pushsdbgv 20							//'buttonSlots'
      &pushsdbgm 36							//'length'
      &lessThan
      &not
      &jnz label17      
      &pushregister 2
      &pushone
      &pushsdb 2							//'GetNumRowsInColumn'
      &callFunction
      &setRegister r:6
      &pop
     label13:
      &pushfalse
      &jnz label17      
      &pushregister 4
      &pushregister 5
      &pushregister 1
      &pushbyte 3
      &dcallfp 37							// MoveButtonSlot()
      &pushregister 1
      &increment
      &setRegister r:1
      &pop
      &pushregister 1
      &pushsdbgv 20							//'buttonSlots'
      &pushsdbgm 36							//'length'
      &lessThan
      &not
      &not
      &jnz label14      
      &jmp label17      
     label14:
      &pushregister 3
      &increment
      &setRegister r:3
      &pop
      &pushregister 3
      &pushregister 6
      &lessThan
      &not
      &jnz label15      
      &pushregister 4
      &pushsdbgv 1							//'buttonSlotHeight'
      &add
      &setRegister r:4
      &pop
      &jmp label16      
     label15:
      &pushregister 2
      &increment
      &setRegister r:2
      &pop
      &pushzero
      &setRegister r:3
      &pop
      &pushregister 2
      &pushone
      &pushsdb 33							//'GetColumnVerticalOffset'
      &callFunction
      &setRegister r:4
      &pop
      &pushregister 5
      &pushsdbgv 34							//'columnHorizontalOffset'
      &add
      &setRegister r:5
      &pop
      &pushregister 2
      &pushone
      &pushsdb 2							//'GetNumRowsInColumn'
      &callFunction
      &setRegister r:6
      &pop
     label16:
      &jmp label13      
     label17:
      &jmp label19      
     label18:
      &pushsdb 5							//'nextButtonSlotId'
      &pushzerosv
     label19:
    &end // of function EraseButtonSlot

    &function2 SetButtonSlotCount (r:7='newCount') (r:1='_root')
      &pushregister 7
      &pushsdbgv 20							//'buttonSlots'
      &pushsdbgm 36							//'length'
      &lessThan
      &not
      &jnz label22      
     label20:
      &pushzero
      &pushsdbgv 20							//'buttonSlots'
      &pushsdb 39							//'pop'
      &callMethod
      &setRegister r:2
      &pop
      &pushzero
      &pushregister 2
      &pushsdbgm 19							//'stage'
      &dcallmp 38							// removeMovieClip()
      &pushzero
      &pushregister 2
      &pushsdbgm 10							//'helpSite'
      &dcallmp 38							// removeMovieClip()
      &pushsdbgv 20							//'buttonSlots'
      &pushsdbgm 36							//'length'
      &pushregister 7
      &greaterThan
      &jnz label20      
      &pushsdbgv 20							//'buttonSlots'
      &pushsdbgm 36							//'length'
      &pushzero
      &equals
      &not
      &jnz label21      
      &pushsdb 5							//'nextButtonSlotId'
      &pushzerosv
     label21:
      &jmp label30      
     label22:
      &pushregister 7
      &pushsdbgv 20							//'buttonSlots'
      &pushsdbgm 36							//'length'
      &greaterThan
      &not
      &jnz label30      
      &pushsdbgv 20							//'buttonSlots'
      &pushsdbgm 36							//'length'
      &pushone
      &pushsdb 29							//'ComputeSlotPositionFromIndex'
      &callFunction
      &setRegister r:9
      &pop
      &pushregister 9
      &pushsdbgm 4							//'col'
      &setRegister r:3
      &pop
      &pushregister 9
      &pushsdbgm 3							//'row'
      &setRegister r:4
      &pop
      &pushregister 3
      &pushone
      &pushsdb 2							//'GetNumRowsInColumn'
      &callFunction
      &setRegister r:8
      &pop
      &pushzero
      &pushregister 1
      &pushsdb 12							//'GetCurrentFaction'
      &callMethod
      &pushsdb 13							//'Yuriko'
      &equals
      &not
      &jnz label23      
      &pushsdbgv 30							//'YurikoYOffset'
      &setRegister r:5
      &pop
      &pushsdbgv 31							//'YurikoXOffset'
      &pushregister 4
      &pushsdbgv 32							//'buttonSlotWidth'
      &pushbyte 5
      &subtract
      &multiply
      &add
      &setRegister r:6
      &pop
      &jmp label24      
     label23:
      &pushregister 3
      &pushone
      &pushsdb 33							//'GetColumnVerticalOffset'
      &callFunction
      &pushregister 4
      &pushsdbgv 1							//'buttonSlotHeight'
      &multiply
      &add
      &setRegister r:5
      &pop
      &pushregister 3
      &pushsdbgv 34							//'columnHorizontalOffset'
      &multiply
      &setRegister r:6
      &pop
     label24:
      &pushfalse
      &jnz label30      
      &pushregister 5
      &pushregister 6
      &pushsdbgv 20							//'buttonSlots'
      &pushsdbgm 36							//'length'
      &pushbyte 3
      &dcallfp 35							// CreateNewButtonSlot()
      &pushsdbgv 20							//'buttonSlots'
      &pushsdbgm 36							//'length'
      &pushregister 7
      &lessThan
      &not
      &not
      &jnz label25      
      &jmp label30      
     label25:
      &pushregister 4
      &increment
      &setRegister r:4
      &pop
      &pushregister 4
      &pushregister 8
      &lessThan
      &not
      &jnz label28      
      &pushzero
      &pushregister 1
      &pushsdb 12							//'GetCurrentFaction'
      &callMethod
      &pushsdb 13							//'Yuriko'
      &equals
      &not
      &jnz label26      
      &pushregister 6
      &pushsdbgv 32							//'buttonSlotWidth'
      &pushbyte 5
      &subtract
      &add
      &setRegister r:6
      &pop
      &jmp label27      
     label26:
      &pushregister 5
      &pushsdbgv 1							//'buttonSlotHeight'
      &add
      &setRegister r:5
      &pop
     label27:
      &jmp label29      
     label28:
      &pushregister 3
      &increment
      &setRegister r:3
      &pop
      &pushzero
      &setRegister r:4
      &pop
      &pushregister 3
      &pushone
      &pushsdb 33							//'GetColumnVerticalOffset'
      &callFunction
      &setRegister r:5
      &pop
      &pushregister 6
      &pushsdbgv 34							//'columnHorizontalOffset'
      &add
      &setRegister r:6
      &pop
      &pushregister 3
      &pushone
      &pushsdb 2							//'GetNumRowsInColumn'
      &callFunction
      &setRegister r:8
      &pop
     label29:
      &jmp label24      
     label30:
    &end // of function SetButtonSlotCount

    &function onUnload (    )    
      &pushsdb 2							//'GetNumRowsInColumn'
      &delete2
      &pop
      &pushsdb 33							//'GetColumnVerticalOffset'
      &delete2
      &pop
      &pushsdb 29							//'ComputeSlotPositionFromIndex'
      &delete2
      &pop
      &pushsdb 35							//'CreateNewButtonSlot'
      &delete2
      &pop
      &pushsdb 37							//'MoveButtonSlot'
      &delete2
      &pop
      &pushsdb 40							//'InsertButtonSlot'
      &delete2
      &pop
      &pushsdb 41							//'EraseButtonSlot'
      &delete2
      &pop
      &pushsdb 42							//'SetButtonSlotCount'
      &delete2
      &pop
      &pushsdb 43							//'onUnload'
      &delete2
      &pop
    &end // of function onUnload

    &pushsdbgv 44							//'initialized'
    &not
    &not
    &jnz label31    
    &pushsdb 14							//'buttonStageWidth'
    &pushbyte 62
    &setVariable
    &pushsdb 45							//'buttonStageHeight'
    &pushbyte 62
    &setVariable
    &pushsdb 32							//'buttonSlotWidth'
    &pushsdbgv 14							//'buttonStageWidth'
    &pushone
    &subtract
    &setVariable
    &pushsdb 1							//'buttonSlotHeight'
    &pushsdbgv 45							//'buttonStageHeight'
    &pushone
    &subtract
    &setVariable
    &pushsdb 34							//'columnHorizontalOffset'
    &pushsdbgv 32							//'buttonSlotWidth'
    &setVariable
    &pushsdb 0							//'numRowsInFirstColumn'
    &pushbyte 6
    &setVariable
    &pushsdb 20							//'buttonSlots'
    &pushzero
    &pushsdb 46							//'Array'
    &new
    &setVariable
    &pushsdb 5							//'nextButtonSlotId'
    &pushzerosv
    &pushsdb 47							//'YurikoXoffset'
    &pushbyte 17
    &setVariable
    &pushsdb 48							//'YurikoYoffset'
    &pushshort -130
    &setVariable
    &pushsdb 44							//'initialized'
    &pushtrue
    &setVariable
    &pushsdb 22							//'FSCommand:'
    &pushsdb 49							//'OnInitialized'
    &pushthisgv
    &pushbyte 2
    &pushsdb 24							//'qualifyName'
    &callFunction
    &concat
    &pushsdb 51							//''
    &getURL2
    &pushsdbgv 52							//'extern'
    &pushsdbgm 53							//'InGame'
    &not
    &not
    &jnz label31    
    &pushbyte 3
    &pushone
    &dcallfp 42							// SetButtonSlotCount()
   label31:
    &stop
  &end // of frame 0
  
  &importAssets &from '../../common/hud/ig_libHUD.swf'
    'LoadMovieStage' &as 1
  &end // of importAssets
  
  &importAssets &from 'HelpBox.swf'
    'HelpBoxSite' &as 2
  &end // of importAssets

  &defineMovieClip 3 // total frames: 2

    &frame 0
      &stop
    &end // of frame 0

    &placeMovieClip 2 
    
      &onClipEvent &construct
        &constants 'vertAlignment', 'center', 'horzAlignment'  
        &pushsdb 0							//'vertAlignment'
        &pushsdb 1							//'center'
        &setVariable
        &pushsdb 2							//'horzAlignment'
        &pushsdb 1							//'center'
        &setVariable
      &end
    &end // of placeMovieClip 2
  &end // of defineMovieClip 3
&end
