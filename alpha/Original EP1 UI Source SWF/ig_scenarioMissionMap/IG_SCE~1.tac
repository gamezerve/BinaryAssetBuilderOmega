movie 'E:\depot\CnC\RA3_EP1\PRODUC~1\Data\APTBUI~1\034A3~1.0-D\pc\Output\IG_SCE~1\\IG_SCE~1.eaf' &compressed // flash 7, total frames: 1, frame rate: 30 fps, 1024x768 px

  &defineMovieClip 4 // total frames: 1
  &end // of defineMovieClip 4

  &defineMovieClip 7 // total frames: 1
  &end // of defineMovieClip 7

  &defineMovieClip 9 // total frames: 20

    &frame 0
      &constants 'MoveIndicator', 'active', 'defeated', 'initialized', 'FSCommand:', 'OnInitialized', 'qualifyName', '', 'OnClipDestroyed', 'Move', 'SetDefeated', 'OnBound', 'OnDestroying', 'onUnload', 'indicatorColor', 'colorizer', 'frame', 'Color', 'setRGB', 'this'  
      &function2 Move (r:4='newX', r:3='newY') (r:1='this', r:2='_parent')
        &pushregister 3
        &pushregister 4
        &pushregister 1
        &pushbyte 3
        &pushregister 2
        &dcallmp 0							// MoveIndicator()
      &end // of function Move

      &function2 SetDefeated (r:2='defeatedArg') ()
        &pushregister 2
        &pushzero
        &equals
        &not
        &setRegister r:1
        &pop
        &pushregister 1
        &jnz label1        
        &pushsdb 1							//'active'
        &jmp label2        
       label1:
        &pushsdb 2							//'defeated'
       label2:
        &gotoAndPlay      &end // of function SetDefeated

      &function2 OnBound () (r:1='this')
        &pushsdbgv 3							//'initialized'
        &not
        &jnz label3        
        &pushsdb 4							//'FSCommand:'
        &pushsdb 5							//'OnInitialized'
        &pushregister 1
        &pushbyte 2
        &pushsdb 6							//'qualifyName'
        &callFunction
        &concat
        &pushsdb 7							//''
        &getURL2
       label3:
      &end // of function OnBound

      &function2 OnDestroying () (r:1='this')
        &pushsdbgv 3							//'initialized'
        &not
        &jnz label4        
        &pushsdb 4							//'FSCommand:'
        &pushsdb 8							//'OnClipDestroyed'
        &pushregister 1
        &pushbyte 2
        &pushsdb 6							//'qualifyName'
        &callFunction
        &concat
        &pushsdb 7							//''
        &getURL2
       label4:
      &end // of function OnDestroying

      &function onUnload (      )      
        &pushsdb 9							//'Move'
        &delete2
        &pop
        &pushsdb 10							//'SetDefeated'
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
      &jnz label6      
      &pushsdbgv 14							//'indicatorColor'
      &pushundef
      &equals
      &not
      &not
      &jnz label5      
      &pushsdb 15							//'colorizer'
      &pushsdbgv 16							//'frame'
      &pushone
      &pushsdb 17							//'Color'
      &new
      &varEquals
      &pushsdbgv 14							//'indicatorColor'
      &pushone
      &pushsdbgv 15							//'colorizer'
      &dcallmp 18							// setRGB()
     label5:
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
     label6:
    &end // of frame 0

    &placeMovieClip 4 &as 'renderImageMC'
    
      &onClipEvent &construct
        &pushs '_type'
        &pushssv 'RenderImage'
        &pushs '_imageMap'
        &pushssv ''
        &pushs '_mode'
        &pushssv 'ALPHA'
      &end
    
      &onClipEvent &load
        &pushs '_imageMap'
        &pushs 'Image'
        &pushsgv '_parent'
        &pushbyte 2
        &pushglobalgv
        &pushs 'qualifyName'
        &callmsv
      &end
    &end // of placeMovieClip 4

    &frame 9
      &stop
    &end // of frame 9

    &frame 19
      &stop
    &end // of frame 19
  &end // of defineMovieClip 9
  
  &exportAssets
    9 &as 'PlayerIcon'
  &end // of exportAssets

  &defineMovieClip 11 // total frames: 1
  &end // of defineMovieClip 11
  
  &exportAssets
    11 &as 'SimpleRenderingSurfaceContentPlaceholder'
  &end // of exportAssets

  &frame 0
    &constants '_x', '_y', '_width', '_height', 'initialized', 'Resize', 'sizer', 'backgroundMC', 'terrainSurface', 'ResizeRenderingSurface', 'playerLayerMC', 'ResizeIndicatorLayer', 'Pointer', 'encode', 'FSCommand:', 'OnInitialized', 'qualifyName', '', 'OnBound', 'ig_scenarioMissionMap OnDestroying()', 'OnDestroying', 'OnClipDestroyed', 'ig_scenarioMissionMap onUnload()', 'SetMapAspectRatio', 'GetTerrainRenderingSurface', 'OnContentLoaded', 'onUnload', 'ig_scenarioMissionMap loaded', 'this', '_parent'  
    &function2 ResizeRenderingSurface (r:1='surfaceClip', r:4='x', r:2='y', r:5='w', r:3='h') ()
      &pushregister 1
      &pushsdb 0							//'_x'
      &pushregister 4
      &setMember
      &pushregister 1
      &pushsdb 1							//'_y'
      &pushregister 2
      &setMember
      &pushregister 1
      &pushsdb 2							//'_width'
      &pushregister 5
      &setMember
      &pushregister 1
      &pushsdb 3							//'_height'
      &pushregister 3
      &setMember
    &end // of function ResizeRenderingSurface

    &function2 ResizeIndicatorLayer (r:1='layerClip', r:5='x', r:4='y', r:3='w', r:2='h') ()
      &pushregister 1
      &pushsdb 0							//'_x'
      &pushregister 5
      &setMember
      &pushregister 1
      &pushsdb 1							//'_y'
      &pushregister 4
      &setMember
      &pushregister 1
      &pushsdbgm 4							//'initialized'
      &not
      &jnz label1      
      &pushregister 2
      &pushregister 3
      &pushbyte 2
      &pushregister 1
      &dcallmp 5							// Resize()
      &jmp label2      
     label1:
      &pushregister 1
      &pushsdbgm 6							//'sizer'
      &pushsdb 2							//'_width'
      &pushregister 3
      &setMember
      &pushregister 1
      &pushsdbgm 6							//'sizer'
      &pushsdb 3							//'_height'
      &pushregister 2
      &setMember
     label2:
    &end // of function ResizeIndicatorLayer

    &function2 SetMapAspectRatio (r:9='mapAspectRatio') ()
      &pushsdbgv 7							//'backgroundMC'
      &pushsdbgm 0							//'_x'
      &setRegister r:7
      &pop
      &pushsdbgv 7							//'backgroundMC'
      &pushsdbgm 1							//'_y'
      &setRegister r:8
      &pop
      &pushsdbgv 7							//'backgroundMC'
      &pushsdbgm 2							//'_width'
      &setRegister r:5
      &pop
      &pushsdbgv 7							//'backgroundMC'
      &pushsdbgm 3							//'_height'
      &setRegister r:3
      &pop
      &pushregister 5
      &pushregister 3
      &divide
      &setRegister r:10
      &pop
      &pushundef
      &setRegister r:6
      &pop
      &pushundef
      &setRegister r:4
      &pop
      &pushundef
      &setRegister r:1
      &pop
      &pushundef
      &setRegister r:2
      &pop
      &pushregister 9
      &pushregister 10
      &lessThan
      &not
      &not
      &jnz label3      
      &pushregister 7
      &setRegister r:6
      &pop
      &pushregister 5
      &setRegister r:1
      &pop
      &pushregister 1
      &pushregister 9
      &divide
      &setRegister r:2
      &pop
      &pushregister 8
      &pushregister 3
      &pushregister 2
      &subtract
      &pushfloat 0.500000000000000
      &multiply
      &add
      &setRegister r:4
      &pop
      &jmp label4      
     label3:
      &pushregister 8
      &setRegister r:4
      &pop
      &pushregister 3
      &setRegister r:2
      &pop
      &pushregister 2
      &pushregister 9
      &multiply
      &setRegister r:1
      &pop
      &pushregister 7
      &pushregister 5
      &pushregister 1
      &subtract
      &pushfloat 0.500000000000000
      &multiply
      &add
      &setRegister r:6
      &pop
     label4:
      &pushregister 2
      &pushregister 1
      &pushregister 4
      &pushregister 6
      &pushsdbgv 8							//'terrainSurface'
      &pushbyte 5
      &dcallfp 9							// ResizeRenderingSurface()
      &pushregister 2
      &pushregister 1
      &pushregister 4
      &pushregister 6
      &pushsdbgv 10							//'playerLayerMC'
      &pushbyte 5
      &dcallfp 11							// ResizeIndicatorLayer()
    &end // of function SetMapAspectRatio

    &function GetTerrainRenderingSurface (    )    
      &pushsdbgv 8							//'terrainSurface'
      &pushone
      &pushsdbgv 12							//'Pointer'
      &pushsdb 13							//'encode'
      &callMethod
      &return
    &end // of function GetTerrainRenderingSurface

    &function GetPlayerLayer (    )    
      &pushsdbgv 10							//'playerLayerMC'
      &pushone
      &pushsdbgv 12							//'Pointer'
      &pushsdb 13							//'encode'
      &callMethod
      &return
    &end // of function GetPlayerLayer

    &function2 OnBound () (r:1='this')
      &pushsdbgv 4							//'initialized'
      &not
      &jnz label5      
      &pushsdb 14							//'FSCommand:'
      &pushsdb 15							//'OnInitialized'
      &pushregister 1
      &pushbyte 2
      &pushsdb 16							//'qualifyName'
      &callFunction
      &concat
      &pushsdb 17							//''
      &getURL2
     label5:
      &pushzero
      &pushsdbgv 8							//'terrainSurface'
      &dcallmp 18							// OnBound()
      &pushzero
      &pushsdbgv 10							//'playerLayerMC'
      &dcallmp 18							// OnBound()
    &end // of function OnBound

    &function2 OnDestroying () (r:1='this')
      &pushsdb 19							//'ig_scenarioMissionMap OnDestroying()'
      &trace
      &pushzero
      &pushsdbgv 10							//'playerLayerMC'
      &dcallmp 20							// OnDestroying()
      &pushzero
      &pushsdbgv 8							//'terrainSurface'
      &dcallmp 20							// OnDestroying()
      &pushsdbgv 4							//'initialized'
      &not
      &jnz label6      
      &pushsdb 14							//'FSCommand:'
      &pushsdb 21							//'OnClipDestroyed'
      &pushregister 1
      &pushbyte 2
      &pushsdb 16							//'qualifyName'
      &callFunction
      &concat
      &pushsdb 17							//''
      &getURL2
     label6:
    &end // of function OnDestroying

    &function onUnload (    )    
      &pushsdb 22							//'ig_scenarioMissionMap onUnload()'
      &trace
      &pushsdb 9							//'ResizeRenderingSurface'
      &delete2
      &pop
      &pushsdb 11							//'ResizeIndicatorLayer'
      &delete2
      &pop
      &pushsdb 23							//'SetMapAspectRatio'
      &delete2
      &pop
      &pushsdb 24							//'GetTerrainRenderingSurface'
      &delete2
      &pop
      &pushsdb 18							//'OnBound'
      &delete2
      &pop
      &pushsdb 20							//'OnDestroying'
      &delete2
      &pop
      &pushsdb 25							//'OnContentLoaded'
      &delete2
      &pop
      &pushsdb 26							//'onUnload'
      &delete2
      &pop
    &end // of function onUnload

    &pushsdbgv 4							//'initialized'
    &not
    &not
    &jnz label7    
    &pushsdb 27							//'ig_scenarioMissionMap loaded'
    &trace
    &pushthisgv
    &pushone
    &pushsdbgv 29							//'_parent'
    &dcallmp 25							// OnContentLoaded()
    &pushsdb 4							//'initialized'
    &pushtrue
    &setVariable
    &pushsdb 14							//'FSCommand:'
    &pushsdb 15							//'OnInitialized'
    &pushthisgv
    &pushbyte 2
    &pushsdb 16							//'qualifyName'
    &callFunction
    &concat
    &pushsdb 17							//''
    &getURL2
   label7:
  &end // of frame 0

  &defineMovieClip 13 // total frames: 1
  &end // of defineMovieClip 13

  &defineMovieClip 15 // total frames: 1
  &end // of defineMovieClip 15

  &defineMovieClip 16 // total frames: 1

    &frame 0
      &constants 'SimpleRenderingSurface OnDestroying()', 'initialized', 'FSCommand:', 'OnClipDestroyed', 'qualifyName', '', 'OnDestroying', 'onUnload', 'initContent', '_type', 'Render', 'this', '_x', 'shape', '_y', '_width', '_height', 'contentClip', 'content', 'SimpleRenderingSurfaceContentPlaceholder', 'attachMovie'  
      &function2 OnDestroying () (r:1='this')
        &pushsdb 0							//'SimpleRenderingSurface OnDestroying()'
        &trace
        &pushsdbgv 1							//'initialized'
        &not
        &jnz label1        
        &pushsdb 2							//'FSCommand:'
        &pushsdb 3							//'OnClipDestroyed'
        &pushregister 1
        &pushbyte 2
        &pushsdb 4							//'qualifyName'
        &callFunction
        &concat
        &pushsdb 5							//''
        &getURL2
       label1:
      &end // of function OnDestroying

      &function onUnload (      )      
        &pushsdb 6							//'OnDestroying'
        &delete2
        &pop
        &pushsdb 7							//'onUnload'
        &delete2
        &pop
      &end // of function onUnload

      &pushsdbgv 1							//'initialized'
      &not
      &not
      &jnz label2      
      &pushsdb 8							//'initContent'
      &pushsdb 9							//'_type'
      &pushsdb 10							//'Render'
      &pushthisgv
      &pushbyte 2
      &pushsdb 4							//'qualifyName'
      &callFunction
      &pushsdb 12							//'_x'
      &pushsdbgv 13							//'shape'
      &pushsdbgm 12							//'_x'
      &pushsdb 14							//'_y'
      &pushsdbgv 13							//'shape'
      &pushsdbgm 14							//'_y'
      &pushsdb 15							//'_width'
      &pushsdbgv 13							//'shape'
      &pushsdbgm 15							//'_width'
      &pushsdb 16							//'_height'
      &pushsdbgv 13							//'shape'
      &pushsdbgm 16							//'_height'
      &pushbyte 5
      &initObject
      &varEquals
      &pushsdb 17							//'contentClip'
      &pushsdbgv 8							//'initContent'
      &pushzero
      &pushsdb 18							//'content'
      &pushsdb 19							//'SimpleRenderingSurfaceContentPlaceholder'
      &pushbyte 4
      &pushsdb 20							//'attachMovie'
      &callFunction
      &varEquals
      &pushsdb 1							//'initialized'
      &pushtrue
      &setVariable
     label2:
    &end // of frame 0

    &placeMovieClip 15 &as 'shape'
    
      &onClipEvent &load
        &pushs ''
        &pushbyte 7
        &pushfalse
        &setProperty
      &end
    &end // of placeMovieClip 15
  &end // of defineMovieClip 16

  &defineMovieClip 17 // total frames: 1

    &frame 0
      &constants 'nextIndicatorId', 'PlayerIcon', 'attachMovie', 'relX', 'relY', 'indicatorColor', 'indicators', 'push', 'Pointer', 'encode', 'length', 'splice', 'removeMovieClip', '_x', 'sizer', '_width', '_y', '_height', 'initialized', 'FSCommand:', 'OnInitialized', 'qualifyName', '', 'OnDestroying', 'OnClipDestroyed', 'AddIndicator', 'RemoveIndicator', 'MoveIndicator', 'OnBound', 'onUnload', 'Array', 'this'  
      &function2 AddIndicator (r:4='indicatorColor') (r:1='this')
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
        &pushsdb 1							//'PlayerIcon'
        &pushbyte 3
        &pushregister 1
        &pushsdb 2							//'attachMovie'
        &callMethod
        &setRegister r:2
        &pop
        &pushregister 2
        &pushsdb 3							//'relX'
        &pushzero
        &setMember
        &pushregister 2
        &pushsdb 4							//'relY'
        &pushzero
        &setMember
        &pushregister 2
        &pushsdb 5							//'indicatorColor'
        &pushregister 4
        &setMember
        &pushregister 2
        &pushone
        &pushsdbgv 6							//'indicators'
        &dcallmp 7							// push()
        &pushregister 2
        &pushone
        &pushsdbgv 8							//'Pointer'
        &pushsdb 9							//'encode'
        &callMethod
        &return
      &end // of function AddIndicator

      &function2 RemoveIndicator (r:3='indicator') ()
        &pushsdbgv 6							//'indicators'
        &pushsdbgm 10							//'length'
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
        &pushsdbgv 6							//'indicators'
        &pushregister 1
        &getMember
        &equals
        &not
        &jnz label3        
        &pushone
        &pushregister 1
        &pushbyte 2
        &pushsdbgv 6							//'indicators'
        &dcallmp 11							// splice()
        &pushsdbgv 6							//'indicators'
        &pushsdbgm 10							//'length'
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
        &dcallmp 12							// removeMovieClip()
      &end // of function RemoveIndicator

      &function2 MoveIndicator (r:1='indicator', r:3='newX', r:2='newY') ()
        &pushregister 1
        &pushsdb 3							//'relX'
        &pushregister 3
        &setMember
        &pushregister 1
        &pushsdb 4							//'relY'
        &pushregister 2
        &setMember
        &pushregister 1
        &pushsdb 13							//'_x'
        &pushsdbgv 14							//'sizer'
        &pushsdbgm 15							//'_width'
        &pushregister 3
        &multiply
        &setMember
        &pushregister 1
        &pushsdb 16							//'_y'
        &pushsdbgv 14							//'sizer'
        &pushsdbgm 17							//'_height'
        &pushregister 2
        &multiply
        &setMember
      &end // of function MoveIndicator

      &function2 Resize (r:5='newWidth', r:4='newHeight') ()
        &pushsdbgv 14							//'sizer'
        &pushsdb 15							//'_width'
        &pushregister 5
        &setMember
        &pushsdbgv 14							//'sizer'
        &pushsdb 17							//'_height'
        &pushregister 4
        &setMember
        &pushsdbgv 6							//'indicators'
        &pushsdbgm 10							//'length'
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
        &pushsdbgv 6							//'indicators'
        &pushregister 2
        &getMember
        &setRegister r:1
        &pop
        &pushregister 1
        &pushsdb 13							//'_x'
        &pushregister 1
        &pushsdbgm 3							//'relX'
        &pushregister 5
        &multiply
        &setMember
        &pushregister 1
        &pushsdb 16							//'_y'
        &pushregister 1
        &pushsdbgm 4							//'relY'
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
        &pushsdbgv 18							//'initialized'
        &not
        &jnz label7        
        &pushsdb 19							//'FSCommand:'
        &pushsdb 20							//'OnInitialized'
        &pushregister 1
        &pushbyte 2
        &pushsdb 21							//'qualifyName'
        &callFunction
        &concat
        &pushsdb 22							//''
        &getURL2
       label7:
      &end // of function OnBound

      &function2 OnDestroying () (r:1='this')
        &pushsdbgv 6							//'indicators'
        &pushsdbgm 10							//'length'
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
        &pushsdbgv 6							//'indicators'
        &pushregister 2
        &getMember
        &dcallmp 23							// OnDestroying()
        &pushregister 2
        &increment
        &setRegister r:2
        &pop
        &jmp label8        
       label9:
        &pushsdbgv 18							//'initialized'
        &not
        &jnz label10        
        &pushsdb 19							//'FSCommand:'
        &pushsdb 24							//'OnClipDestroyed'
        &pushregister 1
        &pushbyte 2
        &pushsdb 21							//'qualifyName'
        &callFunction
        &concat
        &pushsdb 22							//''
        &getURL2
       label10:
      &end // of function OnDestroying

      &function onUnload (      )      
        &pushsdb 25							//'AddIndicator'
        &delete2
        &pop
        &pushsdb 26							//'RemoveIndicator'
        &delete2
        &pop
        &pushsdb 27							//'MoveIndicator'
        &delete2
        &pop
        &pushsdb 28							//'OnBound'
        &delete2
        &pop
        &pushsdb 23							//'OnDestroying'
        &delete2
        &pop
        &pushsdb 29							//'onUnload'
        &delete2
        &pop
      &end // of function onUnload

      &pushsdbgv 18							//'initialized'
      &not
      &not
      &jnz label11      
      &pushsdb 6							//'indicators'
      &pushzero
      &pushsdb 30							//'Array'
      &new
      &setVariable
      &pushsdb 0							//'nextIndicatorId'
      &pushzerosv
      &pushsdb 18							//'initialized'
      &pushtrue
      &setVariable
      &pushsdb 19							//'FSCommand:'
      &pushsdb 20							//'OnInitialized'
      &pushthisgv
      &pushbyte 2
      &pushsdb 21							//'qualifyName'
      &callFunction
      &concat
      &pushsdb 22							//''
      &getURL2
     label11:
    &end // of frame 0
  &end // of defineMovieClip 17
&end
