movie 'E:\depot\CnC\RA3_EP1\PRODUC~1\Data\APTBUI~1\034A3~1.0-D\pc\Output\STED61~1\\STD_MO~1.eaf' &compressed // flash 7, total frames: 1, frame rate: 12 fps, 400x400 px

  &defineMovieClip 2 // total frames: 1
  &end // of defineMovieClip 2

  &defineMovieClip 4 // total frames: 1
  &end // of defineMovieClip 4

  &defineMovieClip 8 // total frames: 1
  &end // of defineMovieClip 8

  &defineMovieClip 15 // total frames: 25

    &frame 4
      &stop
    &end // of frame 4

    &frame 9
      &stop
    &end // of frame 9

    &frame 14
      &stop
    &end // of frame 14

    &frame 24
      &stop
    &end // of frame 24
  &end // of defineMovieClip 15

  &defineMovieClip 16 // total frames: 25

    &frame 4
      &stop
    &end // of frame 4

    &frame 9
      &stop
    &end // of frame 9

    &frame 14
      &stop
    &end // of frame 14

    &frame 24
      &stop
    &end // of frame 24
  &end // of defineMovieClip 16

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

  &defineMovieClip 24 // total frames: 1

    &frame 0
      &stop
    &end // of frame 0
  &end // of defineMovieClip 24
  
  &exportAssets
    24 &as 'std_mouseScrollbarSymbol'
  &end // of exportAssets
  
  &exportAssets
    24 &as 'std_mouseScrollbarSymbol'
  &end // of exportAssets

  &placeMovieClip 24 
  
    &onClipEvent &construct
      &pushs 'm_nLength'
      &pushbyte 100
      &setVariable
      &pushs 'm_nMax'
      &pushbyte 10
      &setVariable
      &pushs 'm_nMin'
      &pushzerosv
      &pushs 'm_nValue'
      &pushzerosv
    &end
  &end // of placeMovieClip 24
&end
