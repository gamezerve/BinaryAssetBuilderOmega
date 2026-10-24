# Super Reactor audio candidate

<!-- Reborn: preserve this reviewable candidate outside the Studio mod list; installing it into user-owned Reborn requires explicit approval after the automatic review block. -->
This candidate has not been installed into `alpha/mods/Reborn`.
`Data/Sounds/RebornReactorAudio.xml` defines two new events using retail samples:

- `SOV_RebornSuperReactor_AmbientLoop`: 14 Soviet reactor samples, Volume 58,
  PerFilePitchShift -14..-10, ambient slider, fade on start/kill.
- `SOV_RebornSuperReactor_Sparks`: three repair-spark samples, Volume 20,
  PitchShift -10..0, ambient slider.

`ReactorChanges.patch` shows the proposed include, ambient replacement and
AnimationSoundClientBehavior at frames 50/100/150/200/250/300 of
`SBPowerPlantAdvanced_IDLA`. It does not change the existing visual FX events.
No new WAV is generated and no retail sample payload is redistributed.

The two-event stream passed native stock qualification and linked stream/BIG
readback (17 retail AudioFile references; checksum `74e3c72a`). In-game balance,
possible multiple-draw animation triggers and subjective sound quality still
need listening tests. A full private reactor-plus-events build is recorded in
the implementation report when complete.
