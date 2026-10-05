# Supervised bounded-duration AudioFile pools

October 5, 2026. The [managed duration candidate](RA3EP1_AUDIO_DURATION_CANDIDATE.md)
now has an explicit actual-core, isolated native-worker and checked **leaf-only**
package path. Canonical commands retain version-1 inventories and 250 ms inputs.
Duration commands require version-2 inventories and 12,000..96,000 samples inclusive.
No production plugin, EA cache/instance-hash parity, playback or game-load proof is
claimed. Overall weighted effort remains approximately 50% complete / 50% remaining.

## Explicit source/command contract

`audio-pool.json` contains exactly `version` and `sources`. Duration mode requires
version 2; canonical mode requires version 1. Sources retain the existing ordered
1..8 unique direct AudioFile XML leaf contract. The source XML remains official
AudioFile syntax: explicit 48 kHz XAS, quality 75, mono PCM16 dependency and declared
RAM/streamed play location. Direct dependencies must be exact canonical RIFF/WAVE
files of 24,044..192,044 bytes with even PCM data length. No extra chunks, other
rates/channels/codecs, Includes, inheritance, formulas, paths or music graph are added.

Only these new inspector commands select duration admission:

- `supervised-audio-duration-preflight <source-directory>`: managed child/current
  core metadata, no codec or package.
- `supervised-audio-duration-encode <audited-dll> <source-directory>`: native raw
  output with parent source/core/runtime/custom reconstruction, no package.
- `supervised-audio-duration-package <audited-dll> <source-directory>`: native
  AudioFile leaves plus checked linked manifest/BIN/RELO/IMP/cdata publication.
- `audio-duration-pool-self-test`: managed-only owned fixtures.
- `audio-duration-native-proof <audited-dll>`: opt-in native boundary/tail/corruption
  proofs, never called by the default compiler suite.

No duration-event command exists. Combining duration admission with an explicit
event source is rejected. Merely editing inventory version does not opt an old
command into longer input: the caller-selected mode and frozen profile must agree.

## Source-to-parent trust boundary

All implementation paths below are relative to
`source/BinaryAssetBuilder.ManifestInspector/`:

- `AuthoredAudioPool.Read/VerifyCurrent/VerifyCopies/PrepareInput`: privately frozen
  `DurationCandidate` mode selects exact inventory version and PCM preparation.
  Candidate WAV limits are mode-local. The maximum distinct source snapshot is
  1,605,984 bytes: eight 192,044-byte WAVs, eight 8,192-byte XMLs, one 4,096-byte
  inventory. Old snapshot limits remain unchanged. Shared WAVs are deduplicated.
- `AudioFileCorePreparation.Prepare/VerifyCurrent`: immutable mode survives every
  current-file recheck. Actual core normalization, processing identity, direct-file
  attribution and independently recomputed XML/file hash must agree before native
  encoding. Old callers default to canonical admission.
- `AudioEncoderSupervisor.Run/Worker`: three exact new worker modes, no prefix-based
  mode admission. Frozen parent profile and request mode must match; the child reads
  inventory version under that mode. Existing nonce, protocol v2, exact input/output
  inventories, hidden x86 worker, timeout, log limits and rejection behavior remain.
- `AudioEncoderPoc.RunPool/Encode/ReadOwnedWave`: preparations derive sample totals
  from source PCM. Native getters must match samples/rate/channel. A leased private
  PCM copy is encoded; repeated reads require its exact prepared byte length. Real
  tag-04 SNR/SNS headers, blocks and sample totals must match before runtime output.
  Cleanup and known-crashing invalid-output-parent protection remain unchanged.
- `AudioPoolResultGate.Verify`: parent rebuilds actual core from frozen source,
  compares exact PCM/runtime/current-core evidence and supplies source-derived sample
  totals to immutable package entries. Worker fields and matching SHA inventories
  cannot authorize arbitrary totals or stale dependencies.
- `AudioFilePackageProbe.Entry/CheckNative/Verify`: expected sample totals are frozen
  explicitly, never inferred from untrusted runtime fields. Longer totals require
  explicit variable-entry admission; fixed slots still require 12,000 samples.
  Exact serializer bytes, native/custom totals, tag-04 framing, both manifest readers,
  source identities, offsets and staged no-overwrite publication are retained.

Per artifact reads remain at most 1 MiB. Worker inventory remains at most 4 MiB,
64 raw/80 packaged files, 16 directories and depth 4. No global bound was raised.
Native containment is not a security sandbox, and source rechecks/staged publication
are not an atomic concurrent-filesystem transaction.

## Executed evidence

Release/x86 build passed; the final incremental build has zero warnings/errors.
All **109 default compiler test groups** and all 33 enum checks passed. Structural
counts remain 785/1,390 models and 762/1,390 typed marshallers. Default tests never
initialize the codec. The new group exercises real managed children for four mixed
durations and eight maximum distinct/shared sources. Synthetic sample-matched
packages pass both readers; these synthetic bodies are not codec evidence.

Managed rejection covers wrong inventory version, legacy longer-WAV admission,
both directions of mode/profile mismatch, duration/event combination, out-of-range
samples, stale timestamp-preserving PCM, stale originals/input copies, wrong explicit
package totals and forged core metadata with freshly recomputed inventories. Current
source restoration recovers core/copy verification. Fixed package entries cannot
admit longer sample totals.

Real native proofs use the previously audited x86 `audio.dll`, SHA-256
`149DE43E1E7C914B8E44DD5E0CDCBED45DE33890EBD2708C8223B278874A610F`.
All inputs are reproducible owned 440 Hz mono PCM fixtures. Jobs below are under
`C:\Users\drknt\AppData\Local\Temp\Reborn-SupervisedAudio-<suffix>`:

| Shape | Files | Worker inventory bytes | Accepted job suffix |
| --- | ---: | ---: | --- |
| RAM+streamed pairs: 12,000 / 24,000 / 48,000 / 96,000 samples | 67 | 1,875,134 | 2a86a2732a44421f99965242897039d7 |
| RAM+streamed pairs: 12,001 / 47,999 / 95,999 samples | 52 | 1,624,293 | a1914a25e46c4bdf833674b127218296 |
| Eight distinct 96,000-sample WAVs, alternating RAM/streamed | 67 | 3,992,340 | 187897ec36f846188bda75e25cae92c3 |
| Eight leaves sharing one 96,000-sample WAV | 60 | 2,648,044 | 75b7ca43243540129e4897602bb68744 |
| Eight distinct 96,000-sample WAVs, all streamed | 71 | 3,994,573 | b1a9b300358a405a9f610bb49979d3fe |

The largest tested worker inventory leaves 199,731 bytes below the unchanged
4,194,304-byte cap. This is measured fixture evidence, not a universal compressed
size guarantee; any future input/output that exceeds a cap rejects. Job root logs,
request/result and parent input copies are outside the worker inventory accounting.

Each native case checks source-derived getter/runtime totals, exact current parent
core reconstruction and both linked package readers. Fresh forged jobs with updated
full inventories alter runtime sample words, PCM, SNR, core metadata, manifest/BIN,
packaged cdata sound totals and streamed SNS block totals; every case rejects with
no acceptance marker. Original valid jobs still verify afterward.

The public raw-duration command also passed on eight distinct maximum streamed
leaves: job `fe04fd13004e401a8ce47e0680ebef68`, no linked package requested.
Previous fixed-control native proofs (three selection/control cases) and previous
3/8-Sound mixed vector native proofs passed again after this change. The default
suite retains old fixed/variable wire, source, transport and corruption regressions.

## Remaining gates

Next prove duration leaves with selected AudioEvents in a separate explicit mixed
mode, including every selected dependency fingerprint, custom payload and reference
order. Stereo/rate/codec changes remain separate experiments. Native output is not
independently decoded back to PCM, and valid framing does not establish playback.
Final type-table/hash/cache parity, SDK source/dependency packaging, WorldBuilder
integration and actual minimal-mod Uprising loading remain open. Replacing XML/XSD
alone is still insufficient.
