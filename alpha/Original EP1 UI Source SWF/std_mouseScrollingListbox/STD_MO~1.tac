movie 'E:\depot\CnC\RA3_EP1\PRODUC~1\Data\APTBUI~1\034A3~1.0-D\pc\Output\STFD9C~1\\STD_MO~1.eaf' // flash 7, total frames: 1, frame rate: 30 fps, 640x480 px

  &defineMovieClip 2 // total frames: 2

    &frame 1
      &stop
    &end // of frame 1
  &end // of defineMovieClip 2

  &defineMovieClip 5 // total frames: 1

    &placeMovieClip 2 &as 'renderImageCustomRender'
    
      &onClipEvent &construct
        &pushs '_type'
        &pushssv 'RenderImage'
        &pushs '_imageMap'
        &pushssv ''
        &pushs '_mode'
        &pushssv 'ALPHA'
      &end
    &end // of placeMovieClip 2
  &end // of defineMovieClip 5
  
  &exportAssets
    5 &as 'std_mouseListboxEntryRenderImageElementSymbol'
  &end // of exportAssets

  &defineMovieClip 9 // total frames: 1
  &end // of defineMovieClip 9
  
  &exportAssets
    9 &as 'std_mouseListboxEntryTextElementSymbol'
  &end // of exportAssets

  &defineMovieClip 10 // total frames: 1
  &end // of defineMovieClip 10

  &defineMovieClip 11 // total frames: 16

    &frame 7
      &stop
    &end // of frame 7

    &frame 15
      &stop
    &end // of frame 15
  &end // of defineMovieClip 11

  &defineMovieClip 13 // total frames: 1
  &end // of defineMovieClip 13
  
  &exportAssets
    13 &as 'std_mouseScrollingListboxEntrySymbol'
  &end // of exportAssets

  &defineMovieClip 15 // total frames: 1
  &end // of defineMovieClip 15

  &defineMovieClip 17 // total frames: 1
  &end // of defineMovieClip 17

  &defineMovieClip 18 // total frames: 1
  &end // of defineMovieClip 18

  &defineMovieClip 19 // total frames: 1
  &end // of defineMovieClip 19
  
  &exportAssets
    19 &as 'std_mouseScrollingListboxSymbol'
  &end // of exportAssets
  
  &exportAssets
    20 &as 'ListboxEntryColorSwatchElementHitRegion'
  &end // of exportAssets

  &defineMovieClip 21 // total frames: 2

    &frame 1
      &stop
    &end // of frame 1
  &end // of defineMovieClip 21
  
  &exportAssets
    20 &as 'ListboxEntryColorSwatchElementHitRegion'
  &end // of exportAssets

  &defineMovieClip 22 // total frames: 1

    &placeMovieClip 21 &as 'colorSwatchCustomRender'
    
      &onClipEvent &construct
        &pushs '_type'
        &pushssv 'ColorSwatch'
        &pushs '_colorIndex'
        &pushbyte -1
        &setVariable
      &end
    &end // of placeMovieClip 21
  &end // of defineMovieClip 22
  
  &exportAssets
    22 &as 'std_mouseListboxEntryColorSwatchElementSymbol'
  &end // of exportAssets

  &defineMovieClip 25 // total frames: 1
  &end // of defineMovieClip 25

  &defineMovieClip 28 // total frames: 1
  &end // of defineMovieClip 28

  &defineMovieClip 31 // total frames: 1
  &end // of defineMovieClip 31

  &defineMovieClip 34 // total frames: 1
  &end // of defineMovieClip 34

  &defineMovieClip 35 // total frames: 1
  &end // of defineMovieClip 35
  
  &exportAssets
    35 &as 'std_mouseListboxEntryGameStatusElementSymbol'
  &end // of exportAssets
&end
