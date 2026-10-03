# Actual core preparation to native audio diagnostic packages

Date: October 4, 2026. The opt-in encoder now has a real-core mode:
`core-audio-encoder-poc <absolute-audited-audio.dll>`. It successfully exercises
actual core AudioFile identities, frozen PCM, native XAS output, runtime/custom
validation and guarded two/three-record diagnostic packages. This is not a
production AudioFile plugin, a recovered stock producer identity, or a playable mod.

## Integration

`AudioEncoderPoc.Run(path, useCore:true)` writes raw AssetDeclaration sources with
distinct RAM/streamed identities. `CoreSource` does not serialize inserted defaults
as explicitly authored cross-platform settings. The existing hash-only core harness
parses each real source and `AudioFileCorePreparation` binds its current disk XML,
resolved WAV, core identity and immutable authored profile before native startup.

`Encode` receives the core instance and preparation together. It rechecks current
sources, writes a new owned PCM snapshot from the frozen preparation and holds a
read-sharing lease during encoding. Native getters and codec/container settings
come from the actual immutable preparation, not guessed metadata. Runtime bytes
come from `corePrepared.SerializeCurrent`, which rechecks the original source and
dependency after encoding. Generated framing, codec tag, samples and runtime tables
still pass the existing independent checks. Source/native output handles close in
the existing finally paths. Exhaustive native failure injection is not proven here.

`CoreAudioPackageGate.Verify` checks ordered RAM/streamed names, source filenames,
IDs, current disk/core preparation and serialized runtime against frozen encoded
records. `Publish` checks this gate before the existing no-overwrite staged
publisher. The same gate protects the optional local AudioEvent package. This is
not an atomic transaction against concurrent edits during staging. The event's
fixed local metadata preparation remains separate from general dependency resolution.

Core InstanceHash and diagnostic payload-derived InstanceHash remain different
domains. The manifest still uses diagnostic content hashes. `core-identities.txt`
is a separate evidence sidecar, not a manifest contract or production hash switch.

## Actual native result and repeatability

Audited library SHA256:
149DE43E1E7C914B8E44DD5E0CDCBED45DE33890EBD2708C8223B278874A610F.
Windows x86 standalone inspector process only; default compiler tests never load it.

First core run:
`C:\Users\drknt\AppData\Local\Temp\Reborn-AudioEncoder-b7640ff6263f413997614b49cdc62878`

Repeated core run:
`C:\Users\drknt\AppData\Local\Temp\Reborn-AudioEncoder-45161f7539494eba822559271c471e33`

The 28 generated files from the first run have identical SHA256 in the repeat,
including owned PCM snapshots, raw native output, packages and identity sidecar.

| Record | Current core hash | Diagnostic content hash | Native/custom evidence |
| --- | --- | --- | --- |
| RebornAudioRAM | 8F8D3317 | BBE648A4 | BIN/RELO/IMP 80/8/0; custom 7,160 bytes, one tag-04 block |
| RebornAudioStream | E73D3F1F | E7D8D8B8 | 88/12/0; custom 7,232 bytes, four tag-04 blocks; inline 0400BB8040002EE0 |

Payload SHA256 remains identical to the earlier authored-only PoC:

- RAM: 78EB78241914FB7F27512F8FC339FD250A90C1C4FD823E60E0C93C833141A9AD.
- Streamed: A1D26B42C1967A3CAC5DB166FA1A68041A54A251CAEF9E0098B1291E796DF2EC.

Both two-entry and local-event three-entry packages pass readback verification.
The three-entry linked BIN/RELO/IMP remains 352/36/20. A separate legacy authored-only
run also succeeds with the same payloads, retaining backward-compatible PoC behavior.
All outputs are fresh owned temporary evidence; no game/reference source is edited.

## Managed regression and remaining gates

`core-audio-package-gate-self-test` is compiler group 98. It uses synthetic compressed
bodies, not actual codec output, and covers ordered/unique bindings, wrong core
identity, same-size timestamp-preserving PCM edits, source XML settings and runtime
subtitle mismatch, rejection before destination staging, existing output preservation
and recovery. Synthetic framing cannot prove decoded audio matches the input PCM;
that distinction remains explicit. Native compressed quality/decoded equivalence is
also not established by this PoC.

Inspector Release/x86 builds; all **98 compiler groups** were executed successfully.
All 33 enum checks pass. Model/marshaller counts remain 785/1,390 and 762/1,390.
Overall weighted effort remains roughly **50% complete / 50% remaining**: this closes
the bounded core-to-native diagnostic integration gate, not general audio or SDK use.

Next: native error/cleanup and stale-source-after-encoding tests in isolated workers,
then a checked experimental AudioFile processor without production/cache admission.
Broader WAV settings, paths, inheritance, source graphs, stock producer seeds/type
tables, WorldBuilder and in-game loading remain open. No changes are sent to Qibbi.
