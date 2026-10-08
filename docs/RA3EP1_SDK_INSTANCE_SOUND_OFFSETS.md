# Typed sound integer offsets and expression-stage review

## Outcome (2026-10-08)

The independent `--instance-sound-offsets` graph profile resolves selected
schema-typed sound integer expressions before guarded inheritance and verifies
the calculated fields against the unchanged Core's actual merged result.
Existing profiles retain their previous arithmetic scope and limits.

On the real Uprising global source graph, both the preceding audio-tree profile
and this profile report **395 Validated / 1 RequiresPreprocessing** across 396
documents and 664 Include edges. There are zero earlier-valid regressions,
six path issues and 198 missing dependency occurrences. Raw snapshot hashes
and the post-audit remain unchanged; the requested output directory is absent.
These are diagnostic source results, not complete dependency or native proof.

SoundEffects passes the separately reviewed expression stage, but its whole
owner still fails: `Child occurrence bound exceeded before copying.` The
failed instance result withholds processed hashes, arithmetic witnesses and
partial inheritance/dependency evidence. Do not interpret the independent
expression review as owner admission.

## Supported subset

Only direct, unprefixed EA AudioEvent/AudioEventOverridable owners under
AssetDeclaration are eligible. Selected unqualified attributes must resolve
to the exact named schema type. PitchShift must be an optional singleton
direct sequence child with the empty, non-wildcard named RealRange shape.

| Location / field | Schema type | Diagnostic integer bounds |
| --- | --- | --- |
| Owner Volume | Percentage | 0..200 |
| Owner MinVolume | Percentage | 0..100 |
| Owner VolumeShift | Percentage | -100..100 |
| Owner MinRange, MaxRange | SageReal | 0..2048 |
| Singleton PitchShift Low, High | SageReal | -12..12 |

These are bounded admission limits, not proven game physical limits.
The grammar is one visible `$NAME` plus/minus one unsigned integer offset
(0..1000), or one signed integer constant. Signed integer definition values
and results must satisfy the selected field bounds. Floating point, percent
suffixes, units, exponents, multiplication, division, chains, signed offsets,
unknown or differently cased names and unsupported fields remain refused.
Expression length is at most 160 characters, with at most 512 sound calculation
slots per source. Existing 4 MiB source and 2048 total-substitution limits apply.
No generic evaluator or interval-order equivalence is claimed.

The older MusicTrack.Volume offset evaluator remains separate: its unsigned
integer 0..100 operands/results, spacing and 16-slot rules are unchanged.
Actual final Core output must have exactly one matching owner/singleton and
the explicit calculated attributes. Failure rejects the entire preparation.
Include visibility, source-local contexts, captured hashes, path confinement,
schema catalog and inheritance guards remain authoritative.

## Reproducible expression review

`sdk-sound-expression-review ra3ep1 <schema-root> <source-root>
<source-entry.xml> <new-output-directory>` is a read-only review command.
It requires the staged schema catalog and admitted reviewed effective schema,
rechecks captured source hashes with a 32 MiB aggregate recheck cap, and emits
JSON without creating the requested output directory. Exit code **2** explicitly
means diagnostic review, not owner admission or production readiness.
It never runs Core merging, native codecs or the game, and writes no XML/XSD.

Real entry: `Sounds/SoundEffects.xml` under the user's Uprising XML root.
Two independent runs returned identical raw and expression-stage SHA-256:

- Raw: `F054D586CAAB71C22AEEA42AA2A71CF91568913DA7FC50D5CE844AD191A2073C`
- Expression stage: `B15F3C0871E29D56608D6469E1EF361194AB2633A9A52A485A0DC607A9018D99`
- 353 substitutions: 276 sound calculations, 77 exact definition references,
  zero MusicTrack calculations.
- Definition sources: SoundEffects.xml and BaseSoundEffect.xml; the latter
  SHA-256 is `69AF7CD3D9C7DDACA8B394AC7976CF4690BD2501E73716110E650950247D86EC`.

| Calculation field | Count |
| --- | ---: |
| AudioEvent Volume | 147 |
| AudioEvent MinRange / MaxRange | 53 / 53 |
| AudioEvent MinVolume / VolumeShift | 5 / 2 |
| AudioEvent PitchShift Low / High | 7 / 7 |
| AudioEventOverridable Volume | 2 |

The same reviewed stage inventories two authored singleton conflicts:

| Owner | Child | First attributes | Second attributes |
| --- | --- | --- | --- |
| AudioEvent:BuildingInfiltrated1 | PitchShift (RealRange) | Low=-10, High=-5 | Low=-1, High=1 |
| AudioEvent:StreetLampCrush | NonInterruptibleTime (TimeRange) | Low=0.0s, High=0.5s | Low=0.0s, High=0.8s |

The inventory records explicit attributes, not a complete content-equivalence
proof. It does not authorize deleting, folding or picking either occurrence.
Review flags keep OwnerInheritanceValidated, FullDependencyCoverage and
ProductionBuildReady false. Imported definition sources are hash-pinned.

## Implementation and validation

All paths below are relative to `source/BinaryAssetBuilder.ManifestInspector/`:

- `SdkSoundOffsets.cs`: typed Evaluate subset and final Verify projection.
- `SdkLocalDefineProfile.cs`: ApplySoundOffsets entry and separate sound witnesses;
  rejected preparation never publishes partial substitutions.
- `SdkIncludeDefineProfile.cs`: source-local/captured Include definition contexts.
- `SdkInstanceInheritanceProfile.cs`: independent sound scope, expression-before-
  inheritance composition and actual merged-output checks.
- `SdkTypedSourceGraph.cs` / `Program.cs`: exclusive graph option, CLI guards and
  standalone review command.
- `SdkSoundExpressionReview.cs`: bounded expression-stage evidence, field counts,
  repeat inventory and stale-source rejection; no owner validation claim.
- `SdkInstanceSoundOffsetsSmokeTest.cs`: registered compiler group 146, including
  signed constants/bases, boundaries, 512/513 slots, actual Core field retention,
  imported contexts, duplicate singleton refusal, review isolation, stale/schema/
  unsupported-expression atomic refusals and unchanged older Music/tree scope.

Validation: all 146 compiler groups pass, along with 33 enum checks and three
Include classifier fixtures. Exclusive/duplicate/path-only graph option probes
reject invalid combinations. Build has zero warnings and errors. Reference
XML/XSD and Core implementation are unchanged.

## Remaining work and next gate

Next, characterize both singleton conflicts with isolated unchanged-Core
fixtures: occurrence order, inherited field retention, differing attribute
sets and complete predicted output. Native evidence must be assessed separately
before asserting engine equivalence. Do not weaken maxOccurs, normalize the
reference XML or assume last-write-wins from appearance alone.

Effort estimate stays **51% complete / 49% remaining**. The arithmetic stage
advanced, but the last source owner is still blocked. Type hashes, native
layouts/stream emission, complete dependency closure, packaging/WorldBuilder
integration and actual Uprising game loading remain distinct major gates.
