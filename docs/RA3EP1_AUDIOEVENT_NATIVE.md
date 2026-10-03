# Isolated EP1 AudioEvent native evidence

Date: October 3, 2026. This is native-only evidence, not a playable mod or a
production audio processor. Overall effort remains approximately 50% complete /
50% remaining: five recovered records do not close audio migration.

## Reference evidence and isolated implementation

Reference: `Working RA3 Compiler for Reference/tools/BinaryAssetBuilder.XmlCompiler.dll`,
SHA-256 `D4B78E2DA951DD458357BEE7B358E6BC9689D1987D55867254D68B3F87BBF0E6`.
Targeted IL methods:

- `0x0600003D`: BaseSingleSound fields and optional allocation order.
- `0x06000045`: Attack/Sound/Decay lists at 128/136/144, allocated before the base.
- `0x06000037`: reference at 0, Weight at 4, normalized inline Volume at 8.
- `0x0600038B`: optional TimeRange allocates eight bytes.

`source/BinaryAssetBuilder.XmlCompiler/Marshaler.Ep1AudioEvent.cs` defines
isolated nested records: Ep1BaseSingleSound (128), Ep1AudioEvent (152),
Ep1AudioFileRefWithWeight (12), Ep1TimeRange (8), for Win32.
Legacy SageBinaryData records/marshal overloads retain 96/120/8 bytes;
AudioEventOverridable remains an unproven legacy path.

| Base field | Offset |
| --- | ---: |
| ShrunkenPitchModifier / ShrunkenVolumeModifier | 20 / 24 |
| Control | 44 |
| SubmixSlider / PitchShift / PerFilePitchShift | 80 / 84 / 88 |
| Delay / InitialDelay | 92 / 96 |
| VolumeSliderMultiplier count / pointer | 100 / 104 |
| MinRangeShift / MaxRangeShift | 108 / 112 |
| LimitGroup count / pointer, reserved and rejected | 116 / 120 |
| NonInterruptibleTime | 124 |

EP1's nine AudioControlFlag values insert SMART_LIMITING before FADE_ON_KILL
and append IMMEDIATE_DECAY_ON_KILL. Reusing the legacy seven-value enum would
also shift several existing flags. An isolated enum follows official XSD order,
which the test checks. DryLevel's official fallback is 100, not legacy marshal
fallback 0. Missing SubmixSlider stays null; the runtime's complex slider
selection is not implemented or claimed. Explicit sliders allocate four bytes.

## Actual Uprising comparison

Unpacked input:
`D:\TEMP\Red Alert 3 Uprising Source Data\Global Data\data\global.manifest`,
SHA-256 `08A415789062B1707EDBA3C456884B94791097505431E17A03D10EE34DF050FD`.
Selected record: `AudioEvent:ImpactDebrisHitsGround`,
TypeID/InstanceID `844D7B9F:30A09D68`, TypeHash `560C2E45`, Tokenized 0.

`tests/fixtures/AudioEventProbe.xml` copies stock literals and explicitly flattens
BaseSoundEffect attributes from the user's official Uprising XML reference.
It does not prove inheritance processing. `AudioEventPipeline.xsd` includes
unchanged official EP1 schemas. Core validates defaults and normalizes 16
AudioFile selectors without registering a processor or emitting streams.

An independently assembled golden and compiled output both match complete
selected game slices: **364/20/68 BIN/RELO/IMP bytes**, with 16 ordered concrete
AudioFile identities. Sound table starts at 152, stride 12; SubmixSlider at 344,
PitchShift at 348, NonInterruptibleTime at 356. RELO entries: 140,80,84,124 and
sentinel. IMP entries: 152 + 12*i, i=0..15, and sentinel.
Stock readback checks exact slice lengths before reading. No full binary dump,
audio decoding or WorldBuilder dump occurs.

### Four additional real records

`AudioEventExtendedProbe.xml` adds four curated literals from the same official
Uprising `Sounds/SoundEffects.xml` source. BaseSoundEffect is explicitly flattened.
The Cryo weapon's `=$WEAPON_FIRE` becomes the reference define's value 50; formula
evaluation and inheritance admission are deliberately not claimed by this fixture.
`ExtendedGolden` constructs independent expected bytes from fixed observed
words, IDs, list offsets, weights and relocations, not the source XML or marshaller.

| Selected AudioEvent | Instance ID | BIN/RELO/IMP | AudioFile references | Real evidence |
| --- | --- | --- | ---: | --- |
| ALL_CryoLegionaire_FreezeWeaponLoop | A78AD6DD | 320/24/56 | 13 | Attack 5, Sound 5, Decay 3; PerFilePitchShift; LOOP + IMMEDIATE_DECAY_ON_KILL |
| ALL_CryoLegionaire_WaterExplosion | F9033CE0 | 228/24/20 | 4 | InitialDelay 0..30; PitchShift; NonInterruptibleTime 0..1.5s |
| ALL_MultiGunnerIFV_EngineerRepairLoop | 4E0BE175 | 212/16/20 | 4 | Last child Weight 100; LOOP + FADE_ON_KILL = 0x21 |
| JAP_Yuriko_FootstepBarefoot | C94ADFA4 | 396/24/76 | 18 | First four weights 500; Delay 0..15; NonInterruptibleTime 0..0.9s |

All four compiled native buffers exactly match the three selected game slices.
The reader requires all five selected names exactly once, exact lengths before
any selected payload read, stock type hash/tokenization and ordered AudioFile
reference tuples. No linked streams are emitted by these native tests.
Extended fixtures also compile repeatedly and recover identically after the
negative cases, with no production/audio codec activation.

A read-only query of the supplied `Sounds/*.xml` AudioEvent nodes did not find
explicit child Volume, LimitGroup, MinRangeShift, MaxRangeShift or SMART_LIMITING
examples. This is a source-directory observation, not proof those features are
unused by every game stream. Child Volume, range shifts and SMART_LIMITING still
have synthetic/reference-schema evidence only; group imports remain rejected.

## Synthetic tests, boundaries and next gates

`AudioEventNativeSmokeTest.Run` checks native sizes/offsets and legacy isolation,
empty/default 152/0/0 bytes, shifted controls, Attack/Sound/Decay order with
independent weights/volumes/defaults/explicit zeros (188/16/16), and every
implemented optional base allocation (220/40/0). The latter covers real/integer
ranges, VolumeSliderMultiplier, explicit zeros, seconds and milliseconds.
These combinations are synthetic goldens, not additional game records.
Malformed flag/scalar inputs reject via the official schema; LimitGroup is
explicitly rejected before native table allocation. Recovery produces identical
bytes, repeat compilation is deterministic, settings are restored, no streams
are emitted. Reserved LimitGroup slots are not a completed group marshaller.

There is no production plugin, cache/reuse eligibility,
AudioFile codec, general diagnostic AudioEvent admission or production registry entry.
The [checked isolated profile](RA3EP1_AUDIOEVENT_PROFILE.md) is now available
separately from these native-only helpers.
This public native helper is a trusted test primitive, not a hardened authored
XML entry point. The separate checked profile enforces current identities,
formula/selector controls and complete prepared AudioFile reference tables.

Next: additional real records covering explicit child volumes, range shifts,
group imports and SMART_LIMITING; LimitGroup import recovery; bounded stream admission
with stock fingerprint gates; mixed stream closure; AudioFile runtime/codec
generation; aggregate type tables, packaging and game loading.

## Reproduce

Use the Release/x86 inspector:

```powershell
.\source\BinaryAssetBuilder.ManifestInspector\bin\x86\Release\net8.0\BinaryAssetBuilder.ManifestInspector.exe audioevent-native-self-test
.\source\BinaryAssetBuilder.ManifestInspector\bin\x86\Release\net8.0\BinaryAssetBuilder.ManifestInspector.exe audioevent-native-self-test "D:\TEMP\Red Alert 3 Uprising Source Data\Global Data\data\global.manifest"
```

The group also runs in `compiler-self-test`: 83 declared groups including the separate checked profile.
Structural inventory stays 785/1,390 models and 762/1,390 typed marshallers;
these native-only nested structs deliberately do not expand that inventory.
