# EP1 external audio fingerprints and native migration gaps

Follow-up: [AudioFile runtime envelope and streamed boundary](RA3EP1_AUDIOFILE_RUNTIME.md)
recovers the inline subtitle pair and validates all 12,951 EnglishAudio native
envelopes, including 1,280 embedded headers. Encoding and production admission
remain closed; matching metadata alone still does not prove codec compatibility.

## Outcome

The bounded diagnostic build now checks the TypeHash and Tokenized flag of each
selected external AudioEvent/Multisound dependency, after exact identity and
unique mapped-stream resolution. It rejects a wrong hash or nonzero tokenization
before processing native output. Unselected external audio records are not gated.
Existing ShaderOverride fingerprint admission remains unchanged.

This is metadata admission, not an audio compiler, payload verifier, complete
type-table recovery or in-game validation. Production registration stays closed.

## Observed stock metadata

The local unpacked inputs below all pass structural validation and EP1 v7 /
AllTypesHash `5454A8E9`. Counts are records across these inputs, not unique assets
across the game. All listed fingerprints have Tokenized=0 and are unregistered
in the current production registry.

| Type | TypeId | TypeHash | Records | Current model / typed marshal |
|---|---|---|---:|---|
| AudioEvent | 844D7B9F | 560C2E45 | 4,824 | Present / present, native mismatch |
| Multisound | A3A7AF37 | F79C5A89 | 528 | Present / present, child mismatch |
| AudioEventOverridable | 10845383 | 8A267F59 | 54 | Present / present, inherited mismatch |
| AudioFile | 166B084D | 53C81E47 | 12,951 | Present / absent |
| AudioEventLimitGroup | E8D44EF5 | 95314DEA | 2 | Absent / absent |
| PathMusicTrack | A70BC1D6 | 9F12CF33 | 2 | Absent / absent |

Only the first two rows are admitted as selected external sound dependencies;
none are admitted as authored audio roots in diagnostic-build.

Input provenance (under `D:\TEMP\Red Alert 3 Uprising Source Data`):

- `Global Data\data\global.manifest`: 11,357 records;
  SHA256 `08A415789062B1707EDBA3C456884B94791097505431E17A03D10EE34DF050FD`.
- `Static Data\data\static.manifest`: 13,872 records;
  SHA256 `74FACD9056EEE20942BB274A924BEFFB03D7ADBB71B9609E804A39619B2C6081`.
- `EnglishAudio\data\audio.manifest`: 12,951 records;
  SHA256 `0CF62FBECC89F15746E523042A6F0422869D98FE41EB742C3104324A81D9A2E4`.

`TypeRegistryAudit.Print` obtains these fingerprints from manifest metadata only.
They describe observed stock builds, not every possible EP1 build or complete
native processor semantics. The guard intentionally rejects unproven variants.

## Why changing source XML/XSD is insufficient

Targeted metadata inspection of the reference
`Working RA3 Compiler for Reference/tools/BinaryAssetBuilder.XmlCompiler.dll`
and current Win32 model layout reveals:

| Native structure | Current bytes | Reference RA3 bytes |
|---|---:|---:|
| BaseSingleSound | 96 | 128 |
| AudioEvent | 120 | 152 |
| AudioFileRefWithWeight | 8 | 12 |
| MultisoundSubsoundRef | 8 | 28 |
| Multisound root | 16 | 16 |
| AudioFileRuntime | 28 | 32 |

The reference assembly is evidence for RA3, not an automatic EP1 ABI guarantee.
Two bounded reads of actual Uprising global BIN/RELO/IMP corroborate important
parts of the mismatch without dumping the stream:

- `AudioEvent:ImpactDebrisHitsGround`, identity `844D7B9F:30A09D68`,
  instance offset 731,980, size 364. The Sound list has count 16 at `+0x88`
  and pointer 152 at `+0x8C`; reference slots begin at `+0x98`, `+0xA4`,
  `+0xB0`, `+0xBC` (12-byte stride). Records contain a one-biased import
  selector, weight 1000 and float 1.0. RELO slots are `0x8C,0x50,0x54,0x7C`;
  there are 16 AudioFile dependencies/imports. This is consistent with the
  152-byte root and 12-byte child layout, not the current 120/8 layout.
- `Multisound:GDI_Generic_VoiceDieMS`, identity `A3A7AF37:DBF49D56`,
  offset 1,455,932, size 464. Subsound count 16 at `+0x8`, pointer 16 at
  `+0xC`, one relocation at `0xC`. Import slots begin `0x10,0x2C,0x48,0x64`
  (28-byte stride). `16 + 16 * 28 = 464`; current 8-byte children cannot match.
  The 16 concrete dependencies are AudioEvent identities.

Only the selected instance payloads and their own relocation/import slices were
read; displayed ranges were capped at 192 bytes (the tool also prints a bounded
256-byte preview). No complete BIN or audio-file payload dump was taken.

Source locations requiring work:

- `source/SageBinaryData/SageBinaryData/Audio.cs`: BaseSingleSound,
  AudioFileRefWithWeight, MultisoundSubsoundRef, AudioEvent, AudioFileRuntime.
- `source/BinaryAssetBuilder.XmlCompiler/Marshaler.Audio.cs`: corresponding
  Marshal overloads still emit old fields/child strides. Presence is not readiness.
- `schemas/ra3ep1/xsd/AssetTypeAudio.xsd`: BaseSingleSound includes
  ShrunkenPitchModifier, ShrunkenVolumeModifier, InitialDelay, MinRangeShift,
  MaxRangeShift, LimitGroup and NonInterruptibleTime missing from current models.
  AudioFileRefWithWeight also has Volume. MultisoundSubsoundRef has optional
  PitchShiftLow/High, Volume, PlayPercent and VolumeShift missing from current code.
- `source/BinaryAssetBuilder.ManifestInspector/BoundedDiagnosticBuild.cs`,
  Build: selected external metadata fingerprint gate. This does not call an audio
  payload processor or alter the production plugin registry.

Do not globally expand old layouts solely from total sizes: exact offsets,
optional-pointer semantics, enum order/defaults and legacy profile compatibility
must be recovered first. Existing Overridable synthetic tests inherit the old
base and are not stock-native goldens. MP3 passthrough is an SDK-specific path,
not evidence of compatibility with stock EP1 AudioFile/SNR/SNS processing.

## Verification and next implementation gate

`DiagnosticFXBuildSmokeTest.Run` now rejects wrong TypeHash and Tokenized for both
concrete sound types, checks the precise guard failure, restores approved metadata
and proves byte-identical recovery. A mapped but unselected wrong-fingerprint
audio record does not block the build. Previous duplicate, ambiguous, missing,
snapshot, publication and real stock FX golden tests remain in this same group.
Synthetic metadata now carries stock hashes/flags, but contains no audio payload;
it still cannot establish audio native compatibility.

Verification completed: both Release/x86 projects build; all 77 compiler groups,
layout checks and 33 enum mappings pass. The optional real-stock command test
still matches the three complete FX native goldens and concrete reference tuples.
Model/marshaller inventory is unchanged at 785/1,390 and 762/1,390. Existing
Overridable model/marshal comments now explicitly identify their synthetic scope.

Next bounded package:

Follow-up: [isolated Multisound native proof](RA3EP1_MULTISOUND_NATIVE.md) now
implements the first native step with three complete stock slice matches and
synthetic optional-pointer goldens. Production/profile admission remains closed;
the legacy-layout mismatch table above still describes the unchanged old path.

1. Recover field offsets and optional defaults for MultisoundSubsoundRef from
   reference marshaller IL and varied stock XML/BIN pairs. Start with default
   children, then explicit pitch/volume overrides, including null-vs-zero cases.
2. Add isolated EP1 layout/marshal support without opening production audio or
   silently changing the legacy KW path. Compare complete BIN/RELO/IMP buffers
   and concrete dependency tuples against more than one stock Multisound.
3. Recover BaseSingleSound's missing 32 bytes, list/pointer semantics and
   AudioEventLimitGroup dependencies before an isolated AudioEvent profile.
4. Separately reverse engineer AudioFileRuntime's extra four bytes and encoded
   audio/header processing; do not treat matching metadata as a codec compiler.
5. Admit additional bounded profiles only after independent native goldens,
   rejection/recovery tests and mixed stream readback. Full SDK packaging,
   WorldBuilder and Uprising game loading remain later gates.

Engineering effort remains approximately 50% complete / 50% remaining. This
package strengthens dependency admission and identifies concrete ABI gaps;
it does not add a native audio family or justify increasing game-readiness.

Related: [bounded FX command](RA3EP1_DIAGNOSTIC_FX_BUILD.md),
[general diagnostic limits](RA3EP1_BOUNDED_DIAGNOSTIC_BUILD.md).
