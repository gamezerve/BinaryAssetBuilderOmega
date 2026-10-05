# Immutable 1–8 Sound selection vectors

October 5, 2026. Variable pool AudioEvents now admit 1–8 distinct local Sound
targets, bounded by the actual pool size. Fixed pair commands retain their original
two-target bound/defaults. Diagnostic-only: production audio processing, EA cache
parity, playback behavior and Uprising loading remain unproven. Weighted overall
effort remains approximately 50% complete / 50% remaining.

## Implementation and contract

No new source format or worker mode is required. The explicit mixed command
`supervised-audio-pool-event <audited-dll> <source-directory>` accepts longer unique
Sound lists only when the validated pool supplies those actual identities. Existing
UTF-8/XML limits, controls, Volume, direct-source restrictions, PCM/XAS profile and
transport/output bounds are unchanged. Duplicate/unknown/case-alias targets reject;
selection count cannot exceed the 1–8-leaf pool. No Attack/Decay/music graph is added.

Repository-relative paths/functions:

- `source/BinaryAssetBuilder.ManifestInspector/AuthoredAudioEventSource.Settings`
  replaces the scalar pair record with a sealed immutable value object. Private
  cloned weight/slot arrays preserve selection order; Slots returns another clone.
  Equality compares all ordered contents and scalar fields, not array identities.
  The old named pair constructor and First/Second accessors preserve existing tests
  and defaults, including rejection of invalid pair reconstruction. ReadSettings
  creates vectors from every admitted Sound; Validate enforces cardinality, unique
  in-range slots, weights and controls. WeightAt indexes the actual ordinal vector.
- `AuthoredAudioEventSource.Read` bounds Sound count by the supplied concrete pool
  and maps exact literals before schema/core processing. Fixed overloads supply only
  their two names, so their admission does not silently become an eight-Sound path.
- `AudioFileLocalEventProbe.Compile` binds each exact concrete dependency and permits
  no more references than actual leaves. CheckNative compares every selector, weight,
  child volume and import offset against the validated vector. Entry diagnostic
  hashes/dependency fingerprints retain selected order and all selected targets.
- `AudioPoolEventPreflight.VerifyCurrent` bounded raw-read caps now accommodate the
  largest event (248 BIN / 8 RELO / 36 IMP); it still compares exact compiled bytes.
- `AudioEventVectorSmokeTest.Run/NativeProof` adds a managed regression group and
  explicit native proof commands: `audio-event-vector-self-test` and
  `audio-event-vector-native-proof <audited-dll>`.

Each weight independently allows unsigned literal 0..1,000,000; omitted Weight is
the official 1000 default. At least one selected weight must be positive. Volume
0..100 and existing four-control-bit admission are unchanged. Caller array edits
cannot alter frozen settings, identity comparisons, weights or reference selection.

## Dynamic layout

For N selected Sounds, native BIN is 152+12N bytes, RELO remains 8 bytes, and IMP
is 4(N+1) bytes. Selectors are event-local one-biased 1..N regardless of reversed
pool slots. Each import is the Sound native selector location 152+12i, followed by
FFFFFFFF. Each Sound child volume stays 3F800000; weights match source independently.
Mixed manifest reference bytes are 8N, and the final event follows all audio records.

| Selected Sounds | Event BIN | RELO | IMP | Reference bytes |
| --- | --- | --- | --- | --- |
| 1 | 164 | 8 | 8 | 8 |
| 2 | 176 | 8 | 12 | 16 |
| 3 | 188 | 8 | 16 | 24 |
| 8 | 248 | 8 | 36 | 64 |

## Executed evidence

Release/x86 build passed. All 107 default compiler groups and all 33 enum checks
passed. Default tests never execute the codec. Structural counts remain 785/1,390
models and 762/1,390 typed marshallers; these are not game compatibility percentages.

Managed tests prove caller/getter-array ownership, value equality and content-change
inequality. Empty/nine-element vectors, mismatched vector lengths, repeated/out-of-
range slots, excessive/all-zero weights and nonfinite volume reject. Omitted/zero/
maximum weights parse independently across a three-element selection. Oversized raw
selection rejects against the supplied pool, not merely native buffer length.

Three/eight-Sound real-core fixtures reverse all selected pool slots and compare
every native selector, weight, child volume, import offset, terminator and reference
ID against independently expected values. Synthetic mixed packages pass both readers.
Full default regressions retain prior pair/fixed wire goldens, controls, stale
source/copy/output checks and parent matching-inventory corruption tests.

Real native XAS mixed publication also passed these two cases. Owned jobs reside at
`C:\Users\drknt\AppData\Local\Temp\Reborn-SupervisedAudio-<suffix>`:

| Leaves/selected Sounds | Pool selection | Accepted job suffix |
| --- | --- | --- |
| 3/3 | 2/1/0 | 2c292a68854a44cea6776fa0ad53c026 |
| 8/8 | 7/6/5/4/3/2/1/0 | 3170cd067f1f4f5fabbb3abb19afb186 |

Events use Volume=37.5, LOOP INTERRUPT and independent ordinal weights 100..N+99.
The eight-selection job contains 68 inventoried files; manifest/BIN/RELO/IMP sizes
are 837/928/96/44 bytes. Its event IMP has 36 bytes after the eight-byte linked
prefix. Parent source/core recompilation and both mixed package readers agreed.
Fresh forged jobs alter the last event selector or last import with recomputed
full inventories; parent reconstruction still rejects, without an acceptance marker.

The old fixed control native proof passed all three selections/control combinations
and source/copy rejection cases after vectorization. Native encoding establishes
audio payload integration; it does not interpret event selection/playback behavior.

## Remaining work

Next broaden the canonical 250 ms mono WAV profile through separate bounded duration/
channel/rate/codec experiments and exact native/runtime/custom reconstruction. Do
not widen production plugin admission merely because longer Sound lists serialize.
Production hash/cache identity, remaining layouts, WorldBuilder integration and
actual Uprising game loading remain significant open gates.

Framing validation does not independently decode compressed samples to PCM. Native
containment is not a security sandbox and source rechecks/staged publication are not
an atomic concurrent-filesystem transaction. Replacing XML/XSD alone remains insufficient.
