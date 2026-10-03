# AudioEvent in bounded diagnostic builds

Date: October 3, 2026. AudioEvent is the sixth narrow family admitted by
`diagnostic-build`. This is not production SDK output, encoded audio generation
or a playable Uprising mod. Overall effort remains approximately 50% complete /
50% remaining; opening one checked family does not close the large remaining gates.

## Use

Build the Release/x86 inspector, choose a new output directory under an existing
parent, and map an unpacked EP1 audio manifest explicitly:

```powershell
# Reborn: choose a fresh output child; existing output is never overwritten and external AudioFile payloads are not copied.
.\source\BinaryAssetBuilder.ManifestInspector\bin\x86\Release\net8.0\BinaryAssetBuilder.ManifestInspector.exe diagnostic-build tests/fixtures/DiagnosticAudioEventProbe.xml Release/MyAudioEventPoC "D:\TEMP\Red Alert 3 Uprising Source Data\EnglishAudio\data\audio.manifest=data/audio.manifest"
```

Create the parent directory separately if it does not exist. Change the child
name for each run. The checked-in example builds local AudioEvent → Multisound →
FX → AttributeModifier using two already existing external AudioFile identities.
Its actual CLI run produced native **368/36/36** and linked **376/44/44**
BIN/RELO/IMP bytes, four entries, a 489-byte manifest and the 665-byte diagnostic
notice. The left mapping side is the physical lookup file, the right side is a
relative runtime name. The actual run used a fresh owned temporary output,
not the illustrative Release path. No game files were modified.

## Admission

Changes:

- `DiagnosticSourceGraph.Visit`: admit direct AudioEvent roots, direct
  Attack/Sound/Decay and the five checked range leaves. Reject authored reference
  suffixes and formulas before core normalization could erase them.
- `DiagnosticAssetPipeline.xsd`: declare AudioEvent using unchanged official
  included audio schemas, not AudioFile or codec roots.
- `BoundedDiagnosticBuild.Build`: explicitly map Ra3Ep1AudioEventPlugin.
- Root ranking: shader/filter/AudioEvent/Multisound/FX/modifier, with actual local
  dependencies visited first. Prior family relative order remains unchanged.
- The existing shared external gate requires each selected AudioFile tuple to
  be unique, TypeHash 53C81E47 and Tokenized 0.

The [checked profile](RA3EP1_AUDIOEVENT_PROFILE.md) remains the native eligibility
boundary: finite official scalars, bounded current XML, up to 32 distinct prepared
AudioFile references, default child Volume 100, default or SOUNDFX slider,
supported control tokens and the five optional range types. LimitGroup,
VolumeSliderMultiplier, MinRangeShift/MaxRangeShift, other explicit sliders,
nondefault child Volume and unproven controls are not opened here.

No inheritance/override/definitions, authored TypeIds, formulas, nested event
references, DTDs, unknown structures or AudioFile roots are admitted.
The shared limits remain 32 roots, 16 XML files, 32 Include edges, depth eight,
eight external mappings, 1 MiB per source, 2 MiB aggregate XML characters,
16 MiB per external manifest and 1 MiB compiled native payload.

## Snapshot, publication and tests

`DiagnosticAudioEventBuildSmokeTest.Run` exercises the same Build service as the
CLI. A six-family Include chain selects one shader, weak filter, event,
Multisound, FX and modifier, excluding the unreferenced event whose AudioFile is
missing. Native **532/48/36** becomes linked **540/56/44**, with five ordered
reference tuples occupying 40 manifest bytes. Source names are the approved
snapshot document names, not original absolute paths.

Independent six-family checks assert stock type hashes, source attribution,
native totals and selectors at linked BIN offsets 324,336,376,480,496. Both
ManifestReader and Utility.Manifest verification remain mandatory before publication.

Tests prove:

- Repeated output is identical and source/settings are unchanged.
- Frozen Include snapshots survive later leaf changes; fresh builds resolve
  changed AudioFile identities.
- Approved XML/manifest snapshots survive caller file changes during staging.
- Wrong AudioFile hash/tokenization, duplicate selected records and repeated
  missing-record attempts reject; restored input recovers identical output.
- Attack/Sound/Decay authored suffixes/formulas reject explicitly in preflight.
- Unsupported options, wrong reference types, TypeIds, inheritance, duplicate
  case-insensitive root identities, nested content, AudioFile roots and 33 roots reject.
- Empty AudioEvent needs no mappings and emits a 152-byte root.
- A positive single-event test covers all three lists and all five range leaves:
  native 232/40/16, three ordered distinct concrete AudioFile identities.
- Existing output cannot be replaced; late stream corruption removes only owned
  staging files; a competing writer's new directory and owner marker survive.
- Staging cleanup and caller Settings restoration hold.

The actual optional run maps the unpacked EnglishAudio manifest and validates
the same six-family closure/fingerprints. External native audio data is neither
read nor rebuilt; fingerprint success is not a codec or game-loading proof.
The checked-in example was also run through the actual CLI, not only Build tests.

## Reproduce tests and remaining work

```powershell
.\source\BinaryAssetBuilder.ManifestInspector\bin\x86\Release\net8.0\BinaryAssetBuilder.ManifestInspector.exe diagnostic-audioevent-build-self-test
.\source\BinaryAssetBuilder.ManifestInspector\bin\x86\Release\net8.0\BinaryAssetBuilder.ManifestInspector.exe diagnostic-audioevent-build-self-test "D:\TEMP\Red Alert 3 Uprising Source Data\EnglishAudio\data\audio.manifest"
```

This group also runs in the full compiler suite and the aggregate
`diagnostic-build-self-test`. The compiler runner has 85 declared groups;
structural inventory remains 785/1,390 models and 762/1,390 typed marshallers.

Next: recover the actual AudioFile runtime/native header and streamed codec
boundaries before considering encoded audio generation. Wider audio options,
LimitGroup and other root types, complete type-table derivation, production
packaging and real Uprising loading remain unverified.
