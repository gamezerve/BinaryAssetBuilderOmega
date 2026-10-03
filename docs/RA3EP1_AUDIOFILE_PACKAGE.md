# Fixed two-AudioFile diagnostic package

Follow-up: [Fixed local AudioEvent to AudioFile package](RA3EP1_LOCAL_AUDIO_PACKAGE.md)
adds an optional isolated parent and local native/reference readback. The original
two-entry mode and its tests are retained; general graph/production gates remain open.

Date: October 4, 2026. Actual encoded RAM and streamed audio now have a verified
v7 manifest, linked native streams and identity-named custom payloads. This is
a fixed diagnostic packaging proof, not a registered AudioFile SDK processor,
production hash algorithm, playable mod or game-loading proof.

## Implementation

`source/BinaryAssetBuilder.ManifestInspector/AudioFilePackageProbe.cs` adds:

- `Entry`: detached copies of native/custom bytes, fixed ID/source names,
  serializer-envelope/relocation validation and independent custom framing.
- `Serialize`: two records in dependency-free order, RAM then streamed; the
  existing utility v7 header/entry writer, recovered eight-byte linked headers,
  a diagnostic notice and two custom-data files.
- `Verify`: exact frozen bytes plus independent ManifestReader and utility
  Manifest readback; type/hash/tokenization, names, source names, offsets,
  references, linked magic/checksum/length and runtime/custom framing checks.
  Missing, extra or renamed custom-data identities reject.
- `Publish`: require absent destination and existing non-reparse parent ancestry,
  create exclusive files in a fresh sibling staging directory, verify, then
  rename without overwriting an existing destination. Failed staging is retained
  as evidence rather than recursively deleted.

Each custom path is:
`diagnostic/cdata/<typeId>.<typeHash>.<instanceId>.<instanceHash>.cdata`.
This matches the convention already used by the stock audit and core BinaryAsset.
RAM custom data is the whole SNR file; streamed custom data is SNS alone. The
eight-byte streamed SNR is embedded in native BIN, not copied as an extra cdata.
Files and custom payloads are bounded to 1 MiB in this fixed proof; native
snapshots are at most 2 KiB. No game BIN dump or archive extraction is performed.

### Hash boundary

Recovered AudioFile type ID/hash are 166B084D / 53C81E47 and tokenized=false.
AllTypesHash 5454A8E9 identifies the observed EP1 stream family, not a completed
production registry. The package's InstanceHash is deliberately diagnostic:
FastHash of native BIN concatenated with custom payload. It is not claimed to be
the EA compiler's source/dependency/processing hash. The stream checksum uses
the already-tested padded identity-tuple convention, with zero references.

A payload edit changes the diagnostic hash and therefore the custom filename.
Frozen copies, not checksum alone, enforce full payload equality at readback.
Do not feed this hash recipe into production cache/reuse or stock patch logic.

## Actual native experiment

`AudioEncoderPoc.Run` now prepares distinct `RebornAudioRAM` and
`RebornAudioStream` identities against unchanged official schemas, saves the
owned authored `ram.xml`/`streamed.xml` definitions alongside the WAV, then
encodes as before. Each result is captured only after current-source/runtime/
framing checks. Package staging begins after both encodes and native shutdown.
The existing `audio-encoder-poc <absolute-audited-audio.dll>` command remains
opt-in and repository-harness dependent.

Actual successful directory:
`C:\Users\drknt\AppData\Local\Temp\Reborn-AudioEncoder-6814ed79e57744828f5f6f25d8727810\package`.
The preceding run at `Reborn-AudioEncoder-01d1b86cb4c34b13a2dec16a45162565`
also passed; the final run repeats the proof with explicit header total/maxima
and utility tokenization/reference checks.

| Package file | Bytes |
| --- | --- |
| diagnostic.manifest | 222 |
| diagnostic.bin | 176 |
| diagnostic.relo | 28 |
| diagnostic.imp | 8 |
| DIAGNOSTIC_ONLY.txt | 152 |
| diagnostic/cdata/166b084d.53c81e47.44521d84.bbe648a4.cdata (RAM) | 7,160 |
| diagnostic/cdata/166b084d.53c81e47.05bcd850.e7d8d8b8.cdata (streamed) | 7,232 |

The two native chunks remain 80/8/0 and 88/12/0 bytes. The v7 manifest has no
external manifests or dependency references, total native BIN size 168, maximum
chunk 88 and maximum relocation chunk 12. An independent CLI inspect reports
zero validation errors; both utility and inspector readers pass in `Verify`.
Native input is still PCM16 mono 48 kHz, 12,000 samples, codec 29 / SND 39.
RAM has one checked block; streamed has four, with inline SNR
`0400BB8040002EE0`. Custom payload SHA256 values remain unchanged:

- RAM: `78EB78241914FB7F27512F8FC339FD250A90C1C4FD823E60E0C93C833141A9AD`.
- SNS: `A1D26B42C1967A3CAC5DB166FA1A68041A54A251CAEF9E0098B1291E796DF2EC`.

## Managed tests and validation

`AudioFilePackageSmokeTest.Run`, exposed as `audiofile-package-self-test`, uses
small synthetic compressed bodies. It does not encode, decode or establish
playback compatibility. It checks linked sizes 176/28/8, two-reader offsets,
custom tuple names, source/copy/dictionary ownership, repeated identical output,
wrong order/duplicate identities, existing directory/file preservation and
native/header/custom tag/size rejection.

For each of six manifest/native/custom files, truncation, trailing data, first/
last-byte changes and missing files reject; orphan custom files reject. Restored
files recover exact approval. A structurally valid custom body edit produces a
new diagnostic hash/name. Header framing alone is not proof of codec data validity.

All 93 compiler groups were executed successfully; ordinary tests never load
native codecs. The actual encoded packaging run above is separate. Both
Release/x86 projects build. Model/marshaller inventory is unchanged at 785/1,390
and 762/1,390. Overall effort remains approximately 50% complete / 50% remaining.

## Remaining boundaries

The general diagnostic-build command still does not admit AudioFile. A complete
isolated IAssetBuilderPlugin entry, core-normalized FileReference/dependency
checks, proven production InstanceHash and mixed AudioEvent-to-local-AudioFile
closure remain next steps. Broader durations/codecs, native failure recovery,
SDK/WorldBuilder distribution, final type table and actual Uprising loading are
not enabled by this fixed package.

Native failures may leave partial owned encoder evidence; failed packaging may
leave named owned staging. The destination is never intentionally replaced.
These checks are not a defense against arbitrary hostile concurrent filesystem
replacement. No original game assets, registry, user output or production plugin
have been changed. No Qibbi write or upstream PR is involved.
