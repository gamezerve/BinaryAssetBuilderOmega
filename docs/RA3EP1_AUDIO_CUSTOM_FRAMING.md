# Custom audio framing: evidence and four rejected records

Follow-up: [Original archive comparison and reconciliation](RA3EP1_AUDIO_ARCHIVE_COMPARISON.md)
proves all four original entries pass unchanged framing rules; unpacked copies
differ. An explicit in-memory overlay passes the full corpus, while this default
local-only command remains strict and still rejects those unchanged local files.

Date: October 3, 2026. Read-only framing audit, not decoding, encoding,
production AudioFile admission or game-loading proof. Overall effort remains
approximately 50% complete / 50% remaining.

## Outcome

`audio-custom-audit` inventories each exact identity-mapped EnglishAudio cdata
file after the existing native-envelope checks. Out of **12,951** records,
**12,947** passed: **268,674** declared blocks representing **787,949,536** file
bytes. That byte total is not bytes read: bodies are skipped by seeking and
only eight-byte headers are read. Four records reject; the command reports
INCOMPLETE and exits 1, never claiming complete stock compatibility.

All 12,951 native envelopes still pass. Custom framing and native-envelope
success are separate results. No source/game files were changed.

Input: `D:\TEMP\Red Alert 3 Uprising Source Data\EnglishAudio\data\audio.manifest`,
SHA256 `0CF62FBECC89F15746E523042A6F0422869D98FE41EB742C3104324A81D9A2E4`.
Custom names are constructed from exact manifest TypeID/TypeHash/InstanceID/
InstanceHash under `data/audio/cdata`, not selected by fuzzy name searches.
This does not validate a content hash or the absence of orphan extra files.

## Outer framing observed in passing records

- Streamed files use the eight-byte sound header already embedded in BIN;
  their first block starts at custom offset zero. RAM files carry that header
  at custom offset zero and start blocks at offset eight.
- Sound header channels/rate/sample total agree with the native fields. High
  sample-word bits are `40000000` for streamed and zero for RAM in this corpus.
- Each block has eight header bytes: a big-endian word with kind in the upper
  byte and **total block length, including header**, in the low 24 bits; then
  a big-endian declared sample count. The rest is opaque codec payload.
- Admitted kinds are 00 and 80. Kind 80 must be the final block, ending exactly
  at EOF. Streamed records require a final 80; RAM records may end with 00.
- Declared samples are positive, sum exactly to the native total, and never
  exceed it partway through. Lengths must be at least eight and inside the file.
- Observed opaque sound-header codec tags among passing files: 04 (11,661),
  06 (1,286). **A tag is not a streamed/RAM discriminator**: tag 06 also occurs
  in RAM data. No codec name/version/decoder compatibility is inferred here.

Selected actual voice `Aeva_KeepCommandDirective`: 205,056 custom bytes, 320
blocks. First block at 0: `000014A600000480` (length 5,286, samples 1,152).
Final block at 203,142: `8000077A00000F16` (length 1,914, samples 3,862).
Earlier 319 blocks sum to 369,792 samples; final total equals native 373,654.
Selected RAM debris data: 28,516 bytes, one block at offset eight,
`00006F5C0000BB80` (length 28,508, samples 48,000).

## Four strict rejections

| AudioFile | Failing block offset | Kind / declared length | Actual custom bytes |
| --- | --- | --- | --- |
| A08_KirovEntry | 8 | 00 / 1,832,520 | 1,048,588 |
| WAGreat_Outdoors_quad | 2,093,983 | 00 / 0 | 2,098,482 |
| WANight_Transylvania_quad | 3,143,012 | 00 / 0 | 5,245,542 |
| WAShima_Compound1_quad | 4,195,164 | 4F / 16,212,828 | 4,197,758 |

Kirov identity: `166b084d.53c81e47.006ca685.2cd5ddd3.cdata`.
Its initial sound header is `0404BB8000178BF8`, followed by
`001BF64800178BF8`: declared sample count 1,543,160 agrees with native, but
the declared block extends past physical EOF. Actual length is 1 MiB + 12.
This is evidence consistent with an incomplete extraction, not proof of the
specific extractor bug or of damage in the original game archive.
The three quad failures similarly require comparison against original archive
entries before interpreting them as corruption versus an unhandled layout.
Do not relax lengths or skip zero/unknown block headers to manufacture success.

## Implementation, tests and commands

`source/BinaryAssetBuilder.ManifestInspector/AudioCustomDataProbe.cs`:
`Inspect` checks headers, block seeks, total samples and EOF; `SelfTest` covers
synthetic RAM/streamed, multiple blocks and malformed headers/flags/lengths/
sample totals/terminal placement. No compressed data is decoded.
`AudioFileRuntimeProbe.Run(..., auditCustom: true)` retains all native gates,
inventories strict custom failures, prints at most eight error examples and
returns failure after aggregate results. Missing files also fail the corpus.
Bounds: 256 MiB per custom file, 65,536 blocks per file; each framing read is
eight bytes. Existing runtime manifest/source caps remain unchanged.

```powershell
# Reborn: run pure framing regressions and a read-only stock corpus audit; the current four rejected records intentionally yield exit code 1.
.\source\BinaryAssetBuilder.ManifestInspector\bin\x86\Release\net8.0\BinaryAssetBuilder.ManifestInspector.exe audio-custom-self-test
.\source\BinaryAssetBuilder.ManifestInspector\bin\x86\Release\net8.0\BinaryAssetBuilder.ManifestInspector.exe audio-custom-audit "D:\TEMP\Red Alert 3 Uprising Source Data\EnglishAudio\data\audio.manifest"
```

The compiler runner now has **87 registered groups**, all executed successfully.
Layout tests passed. Model/marshaller counts remain 785/1,390 and 762/1,390.
The corpus command's deliberate failure is a detected reference-data limitation,
not a passing proof for those four records.

## Encoder evidence and next step

Root `audio.dll` and `source/BinaryAssetBuilder.AudioCompiler/audio.dll` are
byte-identical SHA256 `149DE43E1E7C914B8E44DD5E0CDCBED45DE33890EBD2708C8223B278874A610F`.
The reference tools folder has a managed/mixed AudioCompiler, not a matching
standalone reference audio.dll in that folder. Reference ProcessAudioFileInstance
IL calls SIMEX_filterabout/filterssound; current `Native/Audio.cs` binds
SIMEX_resample/getchannelconfig/getnumsamples instead. Presence of shared SIMEX
names is not proof of matching native ABI. An imports/exports and signature
audit is still required; no native audio library was loaded or invoked here.

Next: locate original audio archive entries, compare/re-extract the four
rejected identities into an owned temporary directory, and rerun framing checks.
Then audit encoder ABI and attempt a minimal owned WAV conversion. Preserve
legacy layouts, keep production codecs closed, and treat game loading as a
separate validation gate.
