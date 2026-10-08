# Bounded MusicTrack Volume offsets — October 8, 2026

## Verified result

Independent `--instance-music-offsets` /
`diagnostic-direct-instance-music-offsets-v1` advances the real typed source
graph to **394 Validated / 2 RequiresPreprocessing** across 396 documents and
664 Include edges. Old/new comparison finds zero earlier-valid regressions.
Earlier `--instance-cross-state-removals` remains 393 / 3; defaults and earlier
expression profiles retain their original scope.

Whole `Sounds/Music.xml` validates with six overlays, one directly imported
AudioEvent base and two prepared sources (BaseSoundEffect and Music). Its single
arithmetic witness names MusicTrack `TEMP_RAM_Music360_LoadScreen`, authored
expression `=$TEMP_RA2_VOLUME + 5`, local definition value `70`, and result `75`.
The actual unchanged Core output is checked again for that exact owner/Volume
after inheritance and temporary imported-base removal, before evidence publication.

- Raw Music SHA-256: `33E767BE22F1D8AEFF5A32D6D996C5FE2641D41A635F05F3012916408365D6C9`.
- Expression-stage SHA-256: `806D6E3042D2042F22870FC4972F01192E324BBD13E213600482879848DEE2E8`.
- Final prepared Music SHA-256: `5F9410C92659FDC4BE9AE49211168A463ACA8E970A6EF6494165FCAB87928332`.

Repeated full audits reproduce the prepared hash. All 396 raw source hashes
match; post-audit hash checks find zero changed sources. Reference XML/XSD and
Core implementation remain unchanged. No output directory is created.

## Contract and code

`SdkMusicVolumeOffsets.Evaluate` permits only an unqualified Volume attribute
on an unprefixed direct EA MusicTrack under AssetDeclaration. The compiled
MusicTrack type must have no attribute wildcard and Volume must have the exact
EA Percentage type. Only `=$NAME + integer` or `=$NAME - integer` is admitted,
with horizontal whitespace on both sides of the operator, bounded identifier,
160-character expression limit and unsigned resolved decimal operands in 0..100.
The result must also remain in 0..100. No floats, percentages, units, signed
operands, multiplication, parentheses, chained operations or second definition
operand are admitted. At most 16 arithmetic slots per source are allowed.

This is a deliberately narrower diagnostic subset, not a replacement for the
EA expression evaluator. The Core wrapper dynamically loads
`BinaryAssetBuilder.ExpressionEval`; this change neither replaces that assembly
nor claims equivalence with its unrestricted grammar, numeric formatting or
other Percentage/real conversions. Existing exact `=$NAME` substitutions and
the reviewed three-form definition-expression subset retain their old rules.

`SdkLocalDefineProfile.ApplyMusicOffsets` retains existing identity/directive,
definition and resource limits and records arithmetic witnesses separately from
definition computations. Substitutions counts all resolved slots, including
arithmetic. Unsupported slots refuse the entire document and expose no partially
transformed bytes or witnesses. `SdkIncludeDefineProfile` optionally carries the
compiled schema while preserving confined captured-source Include visibility,
raw fingerprint rechecks, name-collision refusal and definition origins.

`SdkInstanceInheritanceProfile` selects this stage before owner inheritance and
inside each imported base's own defining context. A consumer's local definitions
cannot replay an imported base's calculation. `SdkMusicVolumeOffsets.Verify`
requires one final direct MusicTrack per witnessed owner id and the exact
calculated Volume. `SdkTypedSourceGraph` and Program expose the new profile
mutually exclusively. It composes previously admitted cross-removal/state-readd/
expression scopes without changing their earlier defaults.

Expression-stage evidence uses `diagnostic-music-volume-offsets-v1`; owner graph
evidence uses the direct-instance profile. Final whole-document schema binding
still applies; invalid scalar fields publish zero trusted dependencies.

## Tests and remaining work

`SdkInstanceMusicOffsetsSmokeTest.Run` is compiler group 144. It tests addition,
subtraction, zero/100 result boundaries, exact schema/slot, imported/local defining
contexts, actual Core Volume, altered post-Core values/missing owners, earlier
profile isolation, unsupported forms, ranges, identity/late/stale/slot-limit
atomic refusals and final invalid-schema zero-field binding. All 144 compiler
groups pass. All 33 enum checks, three Include-classifier fixtures and three
CLI isolation probes pass. Final incremental build has zero warnings/errors.

The global audit still exits 2. Six path issues and 198 missing AUDIO-header
dependency occurrences remain; ScopedGraphComplete, FullDependencyCoverage and
ProductionBuildReady remain false. Typed XML acceptance does not prove audio
payload resolution, codecs, manifest/bin/relo/imp serialization, WorldBuilder
packaging, native type/hash parity or game loading.

Remaining source-preparation blockers:

- SoundEffects.xml: arithmetic on other audio slots is not admitted. Its 13,973
  elements also exceed the existing 8192 pre-normalization bound, so expression
  support alone may expose another refusal.
- Voice.xml: 9,790 elements exceed that bound. Its observed expressions are exact
  references, but safe large-tree/resource/merge-complexity admission still needs
  independent proof.

Read-only owner inventory finds 1,996 assets / 1,698 inherited owners in Voice
and 2,031 assets / 1,800 inherited owners in SoundEffects. The largest individual
asset subtrees contain 21 and 124 elements respectively (including the owner).
The excess is therefore aggregate document breadth, not one giant owner; these
counts alone do not establish safe total merge cost or permit a higher limit.

Next: characterize large audio owner/child counts and Core merge costs before
choosing a bounded tree scope; separately inventory SoundEffects arithmetic by
schema field, literal definition type and value range. Do not globally lift the
tree limit or permit arbitrary audio expressions.

Inventory stays 785/1,390 models, 762/1,390 typed marshallers and 48/48 EP1-only
complex types for both. Manual weighted effort remains **51% complete / 49%
remaining**, not 394/396 usable-mod readiness; native/game gates dominate the
unfinished work.
