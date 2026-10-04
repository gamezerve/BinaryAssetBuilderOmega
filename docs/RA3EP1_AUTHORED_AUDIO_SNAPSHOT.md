# Bounded authored audio snapshots through the supervised worker

Date: October 4, 2026. The new opt-in command accepts a narrow caller-owned input
directory rather than generating all audio internally:
`supervised-authored-audio-poc <absolute-audited-audio.dll> <source-directory>`.
It reads originals, validates and freezes three files, encodes only owned copies
in a supervised child, then rechecks original sources before diagnostic acceptance.
This is not a general AudioFile importer or production SDK processor.

## Admitted source contract

Exactly these three named files are read; the caller's directory is not recursively
scanned and its other files are not changed:

- `ram.xml`: UTF-8 without BOM, at most 8,192 bytes, AssetDeclaration containing
  exactly one AudioFile with id RebornAudioRAM, File=input.wav and RAM play location.
- `streamed.xml`: same limits, id RebornAudioStream and streamed play location.
- `input.wav`: canonical 24,044-byte PCM16 mono 48 kHz / 12,000-sample WAV (250 ms).
  PCM sample content may differ from the old tone; WAV/chunk/header shape may not.

Explicit PCSampleRate=48000, PCCompression=XAS and IsStreamedOnPC are required.
PCQuality must remain 75 (official default is allowed). SubtitleStringName may vary
within the existing printable ASCII, 1–1,024-character profile; omission uses the
existing input filename subtitle default. Unused platform quality defaults must
remain defaults, not explicitly authored cross-platform settings. Paths, aliases,
Includes, DTD/entities, inheritance, formulas, arbitrary IDs, other durations/rates/
channels/codecs and resampling remain closed. No silent identity renaming occurs.

Example `ram.xml` (use the stream ID and IsStreamedOnPC=true in streamed.xml):

```xml
<!-- Reborn: diagnostic-only fixed RAM input; not a production AudioFile definition. -->
<AssetDeclaration xmlns="uri:ea.com:eala:asset">
  <AudioFile id="RebornAudioRAM" File="input.wav" PCSampleRate="48000"
    PCCompression="XAS" IsStreamedOnPC="false"
    SubtitleStringName="DIALOGEVENT:MySoundSubTitle" />
</AssetDeclaration>
```

## Snapshot and protocol ownership

`AuthoredAudioSnapshot.Read` validates trusted official-type schema includes but
disables authored XML resolution/DTDs. It checks the narrow profile before worker
launch, using bounded same-handle reads with no reparse ancestry. Exact raw XML and
PCM bytes are privately owned; Copy returns detached buffers. Install uses fresh
exclusive files and never overwrites caller sources.

Worker protocol is bumped to **version 2**. Optional Inputs metadata binds the three
owned copies by length/SHA256. Fixed and authored modes cannot be silently mixed.
The child verifies inventory before managed preparation or native startup. It
validates the copied authored XML/PCM again, then the existing core preparation
reconstructs current identities before encoding. Parent acceptance checks both
job input and worker copies against its immutable original snapshot before core
source loading. It independently validates runtime/custom/package output as before.

Immediately before ACCEPTED.json, the parent rereads caller originals and rejects
any changed bytes, even if size and timestamp are preserved. No atomic multi-file
snapshot or concurrent hostile-filesystem transaction is claimed. Failed jobs retain
owned evidence without acceptance; caller originals are never rewritten. Accepted
markers remain time-of-validation diagnostic evidence, not security certificates.

Authored subtitles change native record lengths. Mixed-package event offsets now
derive from independently reconstructed AudioFile lengths instead of the old fixed
352-byte fixture assumption. The event remains the same fixed local diagnostic
AudioEvent; user event/source graphs are not imported.

## Actual native evidence

`authored-audio-native-proof <dll>` generated an owned valid input directory using
silence PCM and SubtitleStringName=DIALOGEVENT:authored_snapshotSubTitle. This is a
test fixture representing caller input, not an assertion that original user game
assets were compiled. Both native records and local diagnostic packages were accepted.

Input fixture:
`C:\Users\drknt\AppData\Local\Temp\Reborn-AuthoredNative-8d27ae4410cd4ce3951a6124bd94667e`

Accepted job:
`C:\Users\drknt\AppData\Local\Temp\Reborn-SupervisedAudio-f15bd624c2f84bd88a5f9e28c751b126`

RAM/streamed runtime BIN lengths are 72/80, custom lengths remain 7,160/7,232 and the
mixed linked BIN/RELO/IMP is 336/36/20. Core identities are B73B1D7A/6FB36CE7;
diagnostic content identities are F56BD503/21AFD592. The silent RAM payload SHA256
823F8A283EFCA8FCE1D65C6E9EE569E5CDD5EE0B89AA34ACC5AC020F2DFC8C64 differs from the
old tone output, and runtime bytes retain the caller subtitle. Original input file
hashes are unchanged after normal encoding.

A same-size/timestamp-preserving caller WAV edit after snapshot capture is rejected
before parent acceptance, even though the child successfully encodes the frozen
copies. A separate job-input-copy edit is rejected by the child's inventory gate
before codec startup. Test edits affect only owned temporary fixtures, with original
fixture restoration in finally; no real user or game assets are modified.

## Validation and next gate

Managed snapshot admission/ownership/rejection is compiler group **101**. All 101
compiler groups execute successfully; Release/x86 builds and 33 enum checks pass.
The 12 managed worker transport cases, two opt-in native tamper tests and legacy
fixed supervised encoder still pass under protocol v2. Models/marshallers remain
785/1,390 and 762/1,390. Default tests never execute native codecs.

Overall weighted effort remains approximately **50% complete / 50% remaining**.
Next: generalize identity/package metadata beyond two fixed diagnostic slots while
keeping checked authored snapshots, child isolation and no production/cache admission.
General source graphs, inheritance, broader WAV settings, stock compiler identity,
WorldBuilder and in-game loading remain open.
