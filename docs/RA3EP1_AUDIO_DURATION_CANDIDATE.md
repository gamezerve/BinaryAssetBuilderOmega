# Explicit bounded audio duration preparation

Historical preparation milestone. The subsequent
[supervised duration pool proof](RA3EP1_AUDIO_DURATION_POOL.md) separately enables
version-2 leaf-only core/worker/package admission and records native evidence.
This report's 108-group counters and closed-path statements describe its original
managed-only stage; canonical/fixed/mixed commands still retain their old bounds.

October 5, 2026. This milestone adds **managed candidate preparation**, not native
duration support. Existing core, authored snapshot/pool, worker and package commands
still admit only 250 ms PCM. Overall engineering effort remains approximately
50% complete / 50% remaining; there is no new in-game compatibility evidence.

## Contract and implementation

`source/BinaryAssetBuilder.XmlCompiler/BinaryAssetBuilder/XmlCompiler/`
`Ra3Ep1AudioFileInputProfile.cs` adds `PrepareDurationCandidate`. Callers must
explicitly choose it. The existing `Prepare` API retains exactly 12,000 samples.
Both APIs share `PrepareChecked` for official-schema, identity, XML options,
subtitle, Win32, rate, channel, compression and quality checks.

The candidate admits a canonical 44-byte RIFF/WAVE header followed by contiguous
PCM16 mono 48 kHz data, with **12,000..96,000 samples inclusive** (250 ms..2 s).
RIFF size must equal buffer length minus eight; data size must equal buffer length
minus 44. Data size must be even. The maximum complete WAV is 192,044 bytes.
There are no extra RIFF chunks, stereo, resampling, changed quality or other codecs.
Arbitrary integral sample counts in this range are managed candidates, not a claim
that the native codec accepts every count or tail-padding configuration.

`CheckWave` checks length/alignment before slicing or interpreting header values.
`PreparedInput.Samples` comes from the checked data length rather than child/runtime
metadata. The snapshot stores the admission mode privately. `VerifyCurrent` uses
that same mode and still compares exact current XML, identity hash and owned PCM.
`SerializeCurrent` writes PCM-derived sample totals and rejects streamed headers
whose sample count/rate/channel/play-location contradict them. These headers are
synthetic in this milestone's tests.

No processor registration, native encoder path, filesystem limit, supervisor mode,
protocol version, package entry or manifest layout is changed. Ordinary callers
cannot accidentally acquire the new range by passing a longer WAV to `Prepare`.

## Executed managed evidence

Release/x86 build and all **108 default compiler test groups** passed, as did all
33 enum checks. Models remain 785/1,390; typed marshallers remain 762/1,390.
Default tests do not initialize the native codec.

`source/BinaryAssetBuilder.ManifestInspector/AudioDurationCandidateSmokeTest.cs`
adds one registered group. RAM and streamed candidates exercise sample totals
12,000, 12,001, 24,000, 47,999, 48,000, 95,999 and 96,000. Tests independently
read the serialized sample word and parse the runtime envelope. At 12,000 samples,
all BIN/RELO/IMP bytes are compared with the existing canonical preparation.
All longer candidates reject through the existing `Prepare` API.

Negative cases cover short/odd/oversized input, every structural header field,
zero/oversized/inconsistent declared sizes, trailing bytes, below/above sample
bounds, unsupported compression/platform/fingerprint, stale identity/PCM and
contradictory streamed sample headers. Detached PCM cannot mutate the snapshot;
restoring current source recovers verification. No compressed-body decode or
native duration proof is claimed.

Reproduce with the Release/x86 inspector's `compiler-self-test` command. The new
group prints `Audio duration candidate self-test: OK` before the overall success
line. `scripts/Get-Ra3Ep1PortCoverage.ps1` counts 108 declared groups but does not
execute tests.

## Next integration gates

Changing the WAV parser alone would be insufficient. The remaining duration work
must cross these boundaries together, preserving canonical/fixed callers:

1. `AuthoredAudioPool.Read/Limit`: explicitly version or select duration admission;
   raise only the chosen mode's WAV/snapshot limits, not every source profile.
   Eight distinct maximum-size WAVs total 1,536,352 bytes before XML/inventory;
   mixed source adds event.xml. Shared WAVs remain deduplicated.
2. `AudioFileCorePreparation.Prepare/VerifyCurrent`: carry the chosen immutable
   admission mode through actual core identity, normalized XML and current direct
   file dependencies. Metadata-only validation must precede native work.
3. `AudioEncoderPoc.Encode/ReadOwnedWave`: read only the selected bounded profile,
   encode frozen PCM, compare native sample/rate/channel getters against source,
   and inspect actual XAS tag-04 SNR/SNS block/tail totals. Keep process isolation,
   cleanup and safe output-parent checks; do not test the known crashing invalid
   native output-parent case.
4. `AudioPoolResultGate.Verify` and `AudioFilePackageProbe.Entry/CheckNative`:
   reconstruct expected sample counts from parent core preparation, never accept
   arbitrary worker runtime totals. Derive exact runtime and custom framing before
   staged publication; recomputed inventory alone must not authorize forged data.
5. `AudioEncoderSupervisor`: keep exact input/output membership, nonce, source
   rechecks, timeout and aggregate caps. Source WAVs plus encoder frozen copies alone
   can total 3,072,704 bytes for eight independent two-second leaves. The existing
   4 MiB artifact cap must also fit compressed output, packaged cdata and metadata;
   fitting must be measured on real jobs, not assumed from PCM size or solved by
   unbounded reads. Per-file 1 MiB and artifact-count bounds remain unchanged here.
6. Native proofs: RAM/streamed 250/500/1,000/2,000 ms, then non-frame-aligned sample
   counts; exact parent reconstruction; upper/lower/odd-size rejection; stale source
   and forged runtime/header/custom-total rejection. Replay canonical fixed and
   variable pool/mixed/vector proofs. Only then document new package admission.

This does not close production AudioFile processing, EA cache/instance-hash parity,
general music graphs, WorldBuilder packaging or actual Uprising loading.
