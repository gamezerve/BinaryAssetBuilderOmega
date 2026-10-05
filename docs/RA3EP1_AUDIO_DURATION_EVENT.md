# Supervised duration AudioEvent mixed packages

October 5, 2026. Explicit version-2 duration pools can now freeze `event.xml` and
publish selected AudioEvents with their AudioFile leaves. This extends the
[duration leaf proof](RA3EP1_AUDIO_DURATION_POOL.md), not production compilation.
Canonical/version-1 commands retain 250 ms bounds. Overall weighted effort remains
about 50% complete / 50% remaining; playback/game loading are still unproven.

## Admission and implementation

`supervised-audio-duration-event <audited-audio.dll> <source-directory>` selects
the exact worker mode `encode-pool-duration-event`. Both duration and event admission
must be explicit. Inventory still has exactly version=2 and 1..8 ordered source
XML leaves. `event.xml` selects 1..8 distinct concrete local Sounds, bounded by the
actual pool. PCM remains mono 48 kHz PCM16/XAS, 12,000..96,000 samples; weights,
controls, Volume and direct-source restrictions are unchanged.

Paths below are relative to `source/BinaryAssetBuilder.ManifestInspector/`:

- `AuthoredAudioPool.Read/VerifyCurrent`: freeze duration and event together only
  when requested; reserve event.xml and validate exact concrete names before core
  work. Mixed snapshot cap is 1,614,176 bytes (leaf cap plus 8,192-byte event source).
- `AudioEncoderSupervisor.EventMode/DurationMode/PackageMode`: enumerate exact
  modes, bind parent profile to child request, require matching frozen event source,
  and use 4..18 input-file bounds for either explicit mixed mode. Leaf-only duration
  or canonical mixed modes cannot accept a frozen mixed-duration snapshot.
- `AudioPoolResultGate.BuildEvent/Verify`: existing source-derived recompilation
  and exact settings/raw bytes remain authoritative. No event metadata is trusted
  merely because worker SHA inventories match.
- `AudioFileLocalEventProbe.Entry`: existing selected ordered AudioFile IDs/content
  fingerprints bind event diagnostic hash. No wire-layout change is required.
- `AudioDurationEventSmokeTest.Run`: managed synthetic framing by default; the
  explicit `audio-duration-event-native-proof <audited-dll>` runs real codec workers.

Native event BIN remains 152+12N bytes, IMP is 4(N+1), RELO is 8. Selectors are
event-local one-biased ordinals, not pool indices. Mixed references preserve actual
selection order. Longer AudioFile durations do not change the event wire ABI.
Worker caps remain 4 MiB/80 packaged files and 1 MiB per artifact.

## Verification

Release/x86 build, all 110 default compiler groups and all 33 enum checks passed.
Models remain 785/1,390 and typed marshallers 762/1,390. Default tests never load
the native codec. Managed cases use real core preparation/recompilation and synthetic
sample-matched custom bodies; these bodies are not codec evidence.

Actual native XAS cases passed under the previously audited DLL. Accepted jobs are
under `C:\Users\drknt\AppData\Local\Temp\Reborn-SupervisedAudio-<suffix>`:

| Pool / selected slots | PCM samples | Accepted suffix |
| --- | --- | --- |
| 1 / 0 | 96,000 | 9af8ffcd4c2c41188b830aa47d3cee5b |
| 4 / 3,0 | 12,001 / 24,000 / 47,999 / 96,000 | 98bd152ee99e43da9f59fa9d9cbf1443 |
| 8 / 7,6,5,4,3,2,1,0 | all 96,000, all streamed | 16f12564458c4e5e8aca05ee7cd760ce |

The maximum case has 72 inventoried worker files and 3,995,555 bytes, leaving
198,749 bytes below the unchanged 4 MiB cap. The singleton/subset cases contain
15/38 files and 499,471/938,379 bytes respectively. These are measured fixture
sizes, not a universal compressed-size guarantee. Oversized future jobs reject.

Tests independently compare every selector, ordinal weight/import and selected
reference ID. Changing selected custom payload content invalidates captured
dependencies and changes the rebuilt event hash. Changing an unselected payload
does neither. Both linked manifest readers and exact parent source/core/runtime/
custom reconstruction pass.

Fresh forged jobs recompute inventories after changing final event selectors,
weights, Volume, imports, selected cdata sample totals or event source; every
case rejects. Timestamp-preserving original event edits revoke the snapshot;
restoring exact source bytes recovers verification. Mode/profile mismatch rejects.

## Remaining work

Native output is not independently decoded back to PCM. Event selection, looping,
fade and playback semantics are not verified inside Uprising. Stereo/rate/codec
changes, general music graphs, production hash/cache parity, final type table,
SDK/WorldBuilder packaging and minimal-mod game loading remain open. Replacing
XML/XSD alone remains insufficient.
