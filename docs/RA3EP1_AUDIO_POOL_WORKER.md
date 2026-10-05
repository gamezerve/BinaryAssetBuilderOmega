# Supervised variable AudioFile raw encoding

October 4, 2026. Extends the [explicit pool preflight](RA3EP1_AUTHORED_AUDIO_POOL.md)
with separately supervised managed preparation and opt-in native XAS encoding.
No variable package, AudioEvent graph, production plugin or game-load proof is claimed.
Overall weighted effort remains approximately 50% complete / 50% remaining.

## Commands and retained boundaries

`supervised-audio-pool-preflight <source-directory>` runs an actual managed child
with an unused library path. It never loads a codec.

`supervised-audio-pool-encode <absolute-audited-audio.dll> <source-directory>` runs
native encoding in the hidden Windows x86 child. Library SHA-256 remains pinned to
149DE43E1E7C914B8E44DD5E0CDCBED45DE33890EBD2708C8223B278874A610F.

Both commands require the existing `audio-pool.json` version=1 contract, 1–8 ordered
direct AudioFile XML leaves, and their bounded shared/distinct canonical WAV inputs.
The old fixed RAM/streamed package/event commands are not generalized or replaced.
Broader WAV durations, channels, rates, codecs and source graphs remain closed.

The protocol remains version 2 with explicit new modes `preflight-pool` and
`encode-pool`. Parent-owned `inputs` contains the frozen inventory/XML/WAV bytes;
the request's complete path/length/SHA-256 inventory must agree before core/native
work. Inputs outside the validated pool are rejected. A fixed authored snapshot
cannot be mixed with a pool. Unknown modes still reject.

The existing supervisor retains its 30-second maximum timeout, hidden apphost,
bounded concurrent stdout/stderr drains, process-tree termination, strict result
JSON, nonce/exit/hash checks and owned evidence limits: 64 files, 16 directories,
depth 4, 1 MiB per artifact, 4 MiB aggregate, 32 KiB protocol JSON and 64 KiB per log.
It is process containment, not a native-code security sandbox or atomic filesystem
transaction. Production cache/hash parity and game compatibility remain unproven.

## Implementation and independent acceptance

Repository-relative files/functions:

- `source/BinaryAssetBuilder.ManifestInspector/AudioEncoderSupervisor.cs`:
  `Run` binds the frozen pool, `Worker` checks the full input inventory and dispatches
  pool modes, and `ValidateResult` verifies bounded artifact hashes before the
  independent pool gate. Original caller sources are rechecked before ACCEPTED.json.
- `AudioEncoderPoc.RunPool`: installs exclusive owned copies, builds real isolated
  core instances and preparations, and optionally initializes the pinned codec once.
  Each leaf is encoded using the existing audited `Encode` implementation. Generated
  ordinal prefixes `encoded/leaf0`, etc. cannot collide with caller XML/WAV names.
  Sources are checked before each leaf, after shutdown and before completion.
  Cleanup and module-unload policy retain the existing native failure boundaries.
- `AudioPoolResultGate.Verify`: reconstructs each core instance from the parent's
  frozen sources, checks name/ID/dependency/play location and source-derived runtime
  plus relocations. It compares frozen PCM, validates custom framing/tag-04 and
  exact streamed header, checks ordered core evidence and rejects extra/missing
  output artifacts. No codec is loaded in this parent reconstruction.
- `AudioPoolWorkerSmokeTest.Run` supplies the default managed worker regression;
  `NativeProof` is exposed separately through `audio-pool-native-proof <audited-dll>`.

Managed output is only installed source evidence plus `pool-core.json`. Native
output additionally contains per-leaf `.input.wav`, `.runtime.bin`, `.runtime.relo`
and `.snr`, plus `.sns` for streamed leaves. RAM custom data resides in `.snr`;
streamed `.snr` is the exact eight-byte header and `.sns` is custom payload.
`pool-core.json` lists source/name/ID/core hash/streamed in inventory order. The
parent independently regenerates its exact bytes; this sidecar is not authoritative.
The request mode distinguishes managed-only from encoded acceptance.

The fixed-slot AudioFilePackageProbe.Entry is reused only to validate a leaf's
play-location/native/custom envelope. Its ram.xml/streamed.xml labels are not
used as variable source bindings or passed into package publication. Actual pool
source filenames come from the frozen row and independently reconstructed core.

As in the existing fixed proof, framing and core preparation do not independently
decode the compressed samples back to PCM. General codec correctness and playback
remain separate validation gates.

## Executed validation

Final Release/x86 build passed with zero warnings/errors. All 103 default compiler
groups passed; default tests do not execute a codec. All 33 enum checks passed.
Structural counts remain 785/1,390 models and 762/1,390 typed marshallers.

Managed real-child tests pass 1/3-source distinct dependencies and 8-source shared
dependencies. They reject altered ordered metadata, unexpected output files,
changed original PCM and tampered input copies. Exact restoration passes again.
Separate fresh forged jobs contain modified metadata and a newly matching complete
hash inventory: full parent ValidateResult still rejects, with no acceptance marker.
Existing transport tests retain timeout/exit/crash-code/log/JSON/nonce/path coverage.

The final opt-in native proof passed all six cases below, encoding 29 leaves total.
Owned job directories use
`C:\Users\drknt\AppData\Local\Temp\Reborn-SupervisedAudio-<suffix>`.

| Sources | Dependencies/play locations | Accepted job suffix |
| --- | --- | --- |
| 1 | distinct, RAM | 876e8f2a23e544f6a1a35b7e045d5ede |
| 1 | distinct, streamed | 51ee40cabddd4299bfcffd12413411ba |
| 3 | distinct, alternating RAM/streamed | 99901c4ce358470a89ce38aa389c48cf |
| 8 | shared, alternating RAM/streamed | 16b7e1fff97a49d39dda0b7573c29236 |
| 8 | distinct, alternating RAM/streamed | 97e64aeab24840759c5dd54623fa740f |
| 8 | shared, all streamed | ef07e9a8a60f44339fc54301cb6daa05 |

Each case rejects owned runtime, frozen PCM, SNR and metadata corruption; exact
restoration revalidates. Additional fresh forged jobs alter runtime while updating
all reported hashes: parent semantic reconstruction rejects them. These corruption
checks occur after valid native results, not by injecting known-unsafe native calls.
The streamed-header negative correctly raises the existing unsupported-profile
exception; the test accepts that rejection as well as invalid-data rejection.

The unchanged `supervised-core-audio-poc` also passed both diagnostic package/core
readbacks in job suffix `6815705727d14c9a8109462042ff595d`.

## Next gate

October 5 update: [variable leaf-only package publication](RA3EP1_AUDIO_POOL_PACKAGE.md)
now implements the dynamic linked-table gate below through a separate explicit
worker mode. This report's historical raw-mode evidence remains unchanged; raw
commands still do not publish packages. Selected AudioEvent references remain open.

Generalize diagnostic package admission separately: immutable variable-source leaf
records with explicit play location, 1–8 ordered unique identities, dynamic linked
manifest/bin/relo/imp tables, and custom payload identity/readback through both
readers. Bind every record to its source/core preparation before staging. Add tests
for cardinality, order, aliases, missing/tampered payloads, stale sources and exact
readback. Then resolve selected AudioEvent references against that validated pool.

Do not widen production AudioFile registration, hash/cache behavior or supported
schemas merely because raw encoding succeeded. Remaining native layouts,
WorldBuilder integration and actual Uprising loading still require independent proof.
