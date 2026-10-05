# Supervised variable mixed AudioEvent/AudioFile packages

October 5, 2026. An explicitly frozen event now publishes with a 1–8-leaf audio
pool through a hidden supervised worker and independent parent reconstruction.
Diagnostic-only: no production AudioFile registration, EA cache parity, general
music graph, playback behavior or Uprising game-load claim. Overall weighted
effort remains approximately 50% complete / 50% remaining.

## Explicit source and worker contract

`supervised-audio-pool-event <absolute-audited-audio.dll> <source-directory>`
requires `audio-pool.json`, its named direct AudioFile XML/WAV leaves, and explicit
`event.xml`. The inventory JSON remains version=1 with exactly version/sources.
The event profile remains one literal event with Volume/Control and one/two exact
selected Sound targets; existing weights/defaults and proven controls are unchanged.
Audio input remains canonical 250 ms mono 48 kHz PCM16/XAS with quality 75.

New protocol-v2 mode `encode-pool-event` must carry a validated event snapshot.
Leaf-only/raw/preflight modes must not carry it. An unlisted event beside a leaf
source directory is still ignored by leaf-only snapshot reading; no mode silently
imports it. Explicit mixed snapshot reading reserves event.xml, rejects collisions
with any case-alias AudioFile source, validates the selected targets before native
work, and freezes raw event bytes alongside existing inventory/XML/WAV evidence.

Mixed snapshots allow 4–18 files, at most 270,336 aggregate bytes and at most 8,192
event bytes. Leaf-only 3–17 file / 262,144 byte bounds remain unchanged. Sources
are read-only and reread before acceptance. Request inventories bind every input
path/length/SHA-256; altered input copies fail before codec dispatch. Result limits
retain the existing 80-file packaged bound (mixed maximum observed 72), and older
nonpackaged modes retain 64. Other timeout/JSON/log/file/depth/aggregate bounds and
pinned Windows x86 library admission are unchanged.

## Implementation and reconstruction

Repository-relative files/functions:

- `source/BinaryAssetBuilder.ManifestInspector/AuthoredAudioPool.Read(includeEvent)`
  captures EventName/EventSettings and raw event bytes only when explicitly enabled.
  VerifyCurrent reuses the same explicit mode and compares exact original bytes;
  Install/VerifyCopies include the event without overwriting existing files.
- `AudioEncoderSupervisor.Run/Worker/ValidateResult` admits only matching mixed
  snapshot/mode pairs, binds input/output inventories and process evidence, and
  passes frozen parent inputs into the independent gate before ACCEPTED.json.
- `AudioEncoderPoc.RunPool` rejects mixed snapshots outside package mode and invokes
  publication only after successful native shutdown and current core metadata.
- `AudioPoolResultGate.BuildEvent` independently recompiles the installed frozen
  event twice through actual isolated core contexts against parent-checked leaf
  identities/content fingerprints, requires settings to match the snapshot, and
  compares full native/relocation/import data and diagnostic hash. Worker-reported
  event metadata is never authoritative.
- `AudioPoolResultGate.Publish/Verify` binds current raw/core evidence, reconstructs
  the event, stages/verifies without overwrite, and regenerates exact mixed package
  bytes plus expected artifact membership. Parent Verify repeats this reconstruction
  independently; changed original or owned source bytes revoke acceptance.
- `AudioFilePackageProbe.Serialize/Verify` requires matching fixed/variable closure
  flags. Variable leaves may carry a variable event; fixed events cannot enter this
  path. The final event row is at entries.Length, and selected typed targets must
  resolve uniquely among all prior leaves in both readers. The fixed two-leaf path
  remains byte-compatible and separately admitted.
- `AudioPoolEventSmokeTest.MixedRun/MixedNativeProof` supplies default synthetic
  framing and explicitly requested native integration; one compiler group is added.
  Commands: `audio-pool-mixed-self-test` and `audio-pool-mixed-native-proof <audited-dll>`.

## Linked table behavior

All audio leaves are encoded and packaged, regardless of event selection. The
event is appended after them and captures only selected leaf fingerprints in Sound
order. For an eight-leaf pool selecting 7/0, its references are those exact typed
IDs, while native selectors remain local one-biased 1/2, not global pool indices.
Sound native count/weight/control/import bytes must match source-derived settings.

Manifest asset count is leaf count plus one; selected reference buffer size is
8 per Sound. BIN/RELO contain all leaf chunks followed by event chunks. AudioFile
imports are empty, so IMP starts with the eight-byte linked prefix followed by the
event's 8-byte singleton or 12-byte pair import table. Both readers verify exact
row identity/source/flags, references, linked offsets, custom tuple names and lengths.
Full regenerated byte comparison is required, not only shape checks or file hashes.
Diagnostic content InstanceHash remains separate from core/EA production hashes.

## Executed evidence

Final Release/x86 build passed with zero warnings/errors. All 106 default compiler
groups and all 33 enum checks passed. Default tests never load the codec; synthetic
tag-04 bodies prove framing/linking, not compression correctness. Structural counts
remain 785/1,390 models and 762/1,390 typed marshallers.

Managed fixtures pass 1/3/8-leaf mixed packages, final event row identity, selected
reference order, both readers and parent reconstruction. Wrong mixed/leaf snapshot
modes reject. Fresh forged jobs modify package manifest/BIN/RELO/IMP, custom payload,
or frozen event source and recompute the complete reported inventory; parent checks
still reject. Targeted BIN mutations change event volume, control, first selector
and first weight, and an IMP mutation changes its first import offset; updated
inventories cannot authorize these mismatches. Changed worker event source rejects;
exact byte restoration passes.

Final real codec integration passed these three mixed publication cases:

| Leaves | Audio/selection | Accepted job suffix |
| --- | --- | --- |
| 1 | RAM, Sound slot 0 | 00a78d2e0aa44efa929768237507a58a |
| 3 | distinct mixed, Sound slots 2/0 | b4288c07b10e426bb9a14c78d2eaf3a7 |
| 8 | distinct all streamed, Sound slots 7/0 | 5fce7f3147f849918e7fb31b0efa131e |

Owned jobs reside under
`C:\Users\drknt\AppData\Local\Temp\Reborn-SupervisedAudio-<suffix>`.
The maximum case contains 72 inventoried files. Mixed manifest/BIN/RELO/IMP sizes
are 787/888/112/20 bytes: nine records, eight AudioFiles followed by the event,
16 reference bytes and 12 event import bytes after the stream prefix.

Each case also passes targeted matching-inventory rejection tests and rejects both
tampered input event copies and changed original event XML without ACCEPTED.json.
Source modifications are limited to freshly owned fixtures; caller/game files are
never written. Original byte restoration revalidates. The stale-original test lets
the child finish its old frozen package, then proves parent refusal of stale sources.

Existing fixed control native integration also passed streamed-only mask 0,
RAM-only LOOP and reversed pair mask 129, with its stale/copy rejection tests.
No known unsafe native output-parent calls were repeated.

## Remaining work and limits

Subsequent milestone: [immutable Sound vectors](RA3EP1_AUDIO_EVENT_VECTORS.md) now
admits 1–8 unique selections in variable pools and retains pair defaults for fixed
commands. The earlier one/two-source evidence in this report remains historical.
Broader WAV/codecs, production identity and game-loading gates remain open.

Next expand selected Sound cardinality beyond the current one/two pair without
fixed scalar/slot assumptions, retaining source-derived weights, reference order,
dynamic imports and selected content fingerprints. Broader WAV durations/channels/
rates/codecs need separate bounded processor proofs. Production hash/cache identity,
processor admission, remaining native layouts, WorldBuilder and actual Uprising
loading remain open; these packages are not yet a usable Mod SDK release.

Neither framing readback nor event serialization verifies audible playback or
loop/fade behavior. Custom payloads are not independently decoded back to PCM.
Process containment is not a native security sandbox; snapshot rechecks and staged
rename are not an atomic concurrent-filesystem transaction. Source/XSD replacement
alone still does not establish compatibility.
