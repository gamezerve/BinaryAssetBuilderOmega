# Variable pool AudioEvent closure preflight

October 5, 2026. Explicit selected Sound references now resolve against a verified
1–8-leaf variable audio package and compile through the actual isolated core.
This milestone emits raw event evidence only. Variable mixed package publication,
production compilation and playback/game-load validation remain closed.
Overall weighted effort remains approximately 50% complete / 50% remaining.

## Usage and scope

`audio-pool-event-preflight <encoded-worker-directory> <event-xml>` consumes a
completed leaf worker directory, not its enclosing supervised job directory.
For example the first argument is an owned `Reborn-SupervisedAudio-<nonce>/worker`
directory produced by `supervised-audio-pool-package`. It is not an arbitrary
unchecked manifest or a source-only inventory. Before event work, the input goes
through full current core/raw evidence checks and exact two-reader package readback.
No acceptance marker or child-reported metadata is trusted instead of those checks.

The explicit event file retains the existing bounded UTF-8/no-BOM, one AudioEvent,
literal ID/Volume/Control and one/two Sound profile. Weights/defaults and four
proven control flags are unchanged. Each Sound must name one exact `AudioFile:<id>`
from the verified pool. Case aliases, unknown/repeated targets, empty/oversized
selection, Attack/Decay, Includes/inheritance and unproven fields remain rejected.
This does not admit eight Sound children just because eight leaves are available.

Raw caller XML and the entire encoded input artifact inventory are frozen before
compilation. A fresh owned `Reborn-PoolEvent-<guid>` directory contains the exact
event.xml copy and event.bin/relo/imp. Event core compilation is repeated in a
fresh isolated core context and full output buffers/content hash must agree.
Original/copy XML, input hashes, selected dependency fingerprints and all three raw
event artifacts are rechecked before success. No manifest, mixed custom package,
worker acceptance marker or production cache is written for the event.

## Implementation

Repository-relative paths/functions:

- `source/BinaryAssetBuilder.ManifestInspector/AuthoredAudioEventSource.cs`:
  `Read(bytes, string[] names)` maps exact Sound literals to unique actual SAGE IDs
  in a bounded pool. The old two-name overload delegates with its unchanged pair.
  Settings adds PoolCount, default 2, so selected slots are checked against 1–8
  source records while Sound count remains 1–2. Wire sizes/weights/flags are unchanged.
- `AudioFileLocalEventProbe.ValidateFiles/Compile/Build`: explicit `variable:true`
  requires immutable variable leaves and caller event source. It authorizes concrete
  core references by exact name/ID, assigns their checked diagnostic content hashes,
  invokes the existing AudioEvent plugin and captures only selected fingerprints.
  Entry remembers whether it belongs to a variable closure. Fixed admission now
  explicitly rejects variable records; existing defaults/output remain unchanged.
- `AudioPoolEventPreflight.Run/VerifyCurrent`: independently verifies the leaf
  package, freezes original bytes and SHA-256 inventory, installs only the event in
  a separate owned directory, compiles twice and checks source/core selection.
  VerifyCurrent checks source/copy/inventory, current package, selected dependencies
  and exact raw output bytes. Event placement is separate from leaf sources so a
  caller pool leaf named event.xml cannot be overwritten by the event copy.
- `AudioPoolEventSmokeTest.Run/NativeProof`: default managed synthetic-framing
  regression and separately requested real codec integration. Program exposes
  `audio-pool-event-self-test` and `audio-pool-event-native-proof <audited-dll>`.

## Critical index distinction

In an eight-leaf pool, selecting `[7,0]` yields two manifest reference identities in
that order. Event native selectors are `[1,2]`, not `[8,1]`: they refer to the
event's local reference list, not pool/manifest row positions. Imports point to
native Sound fields 152 and 164 with the existing terminator. PoolCount affects
admission, not the encoded Sound array length. Layout remains:

| Selected Sounds | BIN | RELO | IMP | Native selectors |
| --- | --- | --- | --- | --- |
| one, even pool slot 7 | 164 | 8 | 8 | 1 |
| two, e.g. pool slots 7/0 | 176 | 8 | 12 | 1/2 |

Event diagnostic hashes incorporate selected AudioFile ID/content-hash fingerprints
in selection order. Changed selected payloads revoke dependency evidence. Unselected
leaf hashes do not become event dependencies. Separately, the preflight's frozen
whole-input SHA-256 inventory rejects any encoded-input change, even an unselected
artifact change or a coherently regenerated package. This is not EA cache/hash parity.

## Executed evidence

Final Release/x86 build passed with zero warnings/errors. All 105 default compiler
groups passed, and all 33 enum checks passed. Default tests never execute the codec;
AudioEvent itself is compiled by managed EP1 code. Structural counts remain
785/1,390 models and 762/1,390 typed marshallers.

Managed fixtures construct core-prepared synthetic tag-04 AudioFiles in 1/3/8-leaf
packages. Actual event compilation/recompilation passes singleton slot 0, singleton
late slot 2/7 and reversed late/first pairs 2/0 and 7/0. Tests independently check
sizes, local selector words, reference IDs/order, selected hash revocation and
unselected dependency exclusion. They reject case aliases, unknown/duplicate targets,
three Sounds and Attack structures. Timestamp-preserving same-size Volume changes,
changed event copy, corrupted raw event BIN/RELO/IMP and altered input package all
revoke proof; exact restoration passes. Variable mixed Serialize still rejects.

Final real-codec integration encoded an eight-distinct-leaf package in owned job
`C:\Users\drknt\AppData\Local\Temp\Reborn-SupervisedAudio-78b87a5661914474953bef9caf52146a`.
The managed event closure then passed both selections against those actual XAS
payload fingerprints, including the staleness/raw corruption checks:

| Selection | Owned raw event suffix | BIN/RELO/IMP |
| --- | --- | --- |
| 7 | afd238b1c7784b8d9539000482acd171 | 164/8/8 |
| 7/0 | 832712d9f38d48fcaab7c651352975dd | 176/8/12 |

Suffixes belong to `C:\Users\drknt\AppData\Local\Temp\Reborn-PoolEvent-<suffix>`.
These events use Volume=37.5, Control=LOOP INTERRUPT, Weight=125 per selected Sound.
The codec encodes audio leaves; it does not interpret event loop/control behavior.

The existing fixed control native proof also passed streamed-only control 0,
RAM-only LOOP and reversed pair mask 129, including original/copy tamper rejection.
That confirms the default fixed path remains operational after closure refactoring.

## Next gate and remaining risks

Bind explicit frozen event source/settings into a new supervised variable mixed
publication mode. Recompile independently in the parent and require identical full
event bytes/selected fingerprints; derive mixed manifest reference/import offsets
from the verified event and all leaf lengths. Resolve each selected target uniquely
before the appended event row in both readers. Add forged-result, stale-source,
selector/reference/order/hash and custom tuple rejection tests before acceptance.
The existing variable package path deliberately rejects any event until this gate.

No general event/music graph, broader WAV/codecs, production hash/cache behavior,
WorldBuilder integration or Uprising game loading is established. Framing checks
do not decode compressed samples back to PCM. Evidence freezing/rechecks are not
an atomic concurrent filesystem transaction; native process isolation is not a
security sandbox. Source/schema replacement alone remains insufficient.
