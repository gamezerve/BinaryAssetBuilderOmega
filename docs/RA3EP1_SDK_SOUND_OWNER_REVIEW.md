# Complete isolated sound owner review

## Outcome (2026-10-08)

Read-only `sdk-sound-owner-review <absolute-Uprising-source-root>` prepares the
captured BaseSoundEffect document using the existing sound-offset profile, then
independently predicts every explicit root field and every direct child of the
two known conflicting owners. Actual unchanged Core output must match before
isolated schema validity or owner witnesses are published.

Both real owners pass the complete explicit XML projection and final schema
checks. Two runs yield identical complete reports. This adds compiler group
148, not a preprocessing option: the existing profile still refuses the whole
SoundEffects source. Last measured global graph remains **395 valid / 1 blocked**.
No reference XML/XSD or Core implementation changes; no output directory,
codec/native serializer, stream emission or game execution.

## Captured source and direct base context

The exact entry is `Sounds/SoundEffects.xml`, SHA-256
`F054D586CAAB71C22AEEA42AA2A71CF91568913DA7FC50D5CE844AD191A2073C`.
Direct instance Include/base preparation uses `Sounds/BaseSoundEffect.xml`,
raw SHA-256 `69AF7CD3D9C7DDACA8B394AC7976CF4690BD2501E73716110E650950247D86EC`,
processed document SHA-256
`6E7D1FC145729543D9EFEB7B3A6C066A8C4FE656890FD5AC84EAD9490B52D1CD`.
Both owners explicitly inherit `AudioEvent:BaseSoundEffect`.

The selected base has no inherited handle or element children. Its prepared
explicit field projection must equal the original selected base. The remaining
base-document assets are prepared under existing guards, not reinterpreted by
this review. Expression processing of the entry records 353 substitutions in
its source-local captured Include context.

This command is intentionally snapshot-pinned. Other source/base hashes, base
handles, populated bases, owner identities or authored child bodies are refused.
Compiled schema admission uses the existing staged catalog and reviewed effective
schema. Every captured source is hash/length rechecked, with a 32 MiB aggregate
post-recheck cap. No partial report is printed if an earlier/later check fails.

## Complete explicit owner results

The prediction overlays the explicit owner scalar fields onto the original
base fields, consumes only inheritFrom in the isolated joining clone and uses
independently authored expected child bodies/order. It does not derive expected
children from Core output. Root and child QName/namespace, attribute presence
and values, child order/multiplicity and Sound reference text are compared.
Projection encoding length-prefixes fields to avoid separator ambiguity and
ignores only namespace declaration placement and XML trivia. Nested leaf
payloads and non-whitespace owner text remain closed.

| Owner | Explicit root fields | Final child sequence |
| --- | ---: | --- |
| BuildingInfiltrated1 | 17 | PitchShift, InitialDelay, Sound |
| StreetLampCrush | 18 | PitchShift, Delay, NonInterruptibleTime, Sound, Sound, Sound |

BuildingInfiltrated1 retains PitchShift -1/1, InitialDelay 0/50 and the exact
`WBSpy_infiltrateBldgP` reference. Owner Volume=55, VolumeShift=-10, Limit=2,
Type and SubmixSlider overlay the base; inherited MinVolume, priority, pitch/
volume modifiers, range, reverb/play and offscreen fields remain intact.

StreetLampCrush retains PitchShift -10/10, Delay 0/100 and NonInterruptibleTime
0.0s/0.8s. The three distinct Sound references remain A/B/C in authored order:
`WBStreetLamp_crushA`, `WBStreetLamp_crushB`, `WBStreetLamp_crushC`.
Owner Volume=50, VolumeShift=-10, Priority=LOW, Control=INTERRUPT and other
explicit overrides coexist with retained base range/offscreen/modifier fields.

Processed isolated-owner XML SHA-256:

- BuildingInfiltrated1:
  `58541F264C572F74D28969798BA07A90C2C3592174B6A78D6B3CB1507BFD2F18`
- StreetLampCrush:
  `1FD39DDDF8C5FA33BDC4C38913E244B52808B1C42D011A478197774259203CFE`

These are XML snapshot hashes, not native asset identity hashes.
The preceding [Core/native singleton characterization](RA3EP1_SDK_SOUND_SINGLETON_SEMANTICS.md)
independently corroborates the selected native range words. This command does
not verify whole native owner bytes, all reference identities or game behavior.

## Implementation and tests

`source/BinaryAssetBuilder.ManifestInspector/SdkSoundOwnerReview.cs`:

- `Review`: pinned source/base acquisition, unchanged-profile refusal assertion,
  existing guarded base/expression preparation, actual Core merge, schema check
  and source post-recheck.
- `Predict`: two exact authored bodies, empty direct base and literal scalar
  overlay; no generic singleton normalization.
- `Verify`, `Fields`, `Children`: whole explicit projection checks before
  publishing witnesses; nested/reference/order loss cannot be hidden by schema
  validity.

`SdkSoundOwnerReviewSmokeTest.cs` registers group 148 in CompilerSmokeTest and
Program. Owned fixtures check both complete Core/schema results and inherited
field retention. Removing/changing/adding fields, dropping/duplicating/reordering
children, changing Sound text/range values, nested payloads, owner text,
changed base handles, populated bases and remaining expressions are refused.
Default tests need no external source/game files and execute no codecs.

Validation: focused fixtures and all 148 compiler groups pass. Missing command
arguments and a relative source root return exit 1. Coverage stays 785/1390
complex models, 762/1390 typed marshallers and 48/48 EP1-only types. Build has
zero errors; the three previously documented Core/XmlCompiler warnings remain,
with no new review/test compiler warning.

The review command returns **2**, even when both isolated owners pass. Report
flags retain WholeSourceValidated, FullDependencyCoverage, ProductionBuildReady
and NativeOwnerBytesVerified=false; ExistingProfileRefused=true. This is not a
fallback when whole-owner/source preprocessing fails.

## Next gate and effort

The two isolated complete-owner XML projections now pass. A separately scoped
singleton admission can be considered next, but must retain original commands
and fields, predict/check complete owner output, reject unsupported shapes,
preserve earlier-profile refusals and atomically bind the entire source graph.
The new whole-graph run must prove no earlier-valid regressions and unchanged
source hashes before changing the 395/396 count.

Effort stays **51% complete / 49% remaining**: this review closes a proof step,
not the whole source, native emission, complete dependencies, packaging/
WorldBuilder or actual Uprising game-loading gates.
