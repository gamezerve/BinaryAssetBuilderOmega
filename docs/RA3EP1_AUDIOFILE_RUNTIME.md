# AudioFile runtime envelope and streamed boundary

Follow-up: [Custom audio framing and four rejected records](RA3EP1_AUDIO_CUSTOM_FRAMING.md)
checks all mapped custom files: 12,947 pass and four reject. Native envelope
success remains separate; no complete corpus/codec compatibility is claimed.

Date: October 3, 2026. Read-only stock evidence, not an AudioFile compiler,
codec decoder or playable SDK. Overall effort remains approximately 50% complete /
50% remaining; model/marshaller inventory is unchanged.

## Recovered ABI

Both reference AudioCompiler and XmlCompiler declare AudioFileRuntime as 32 bytes.
The current `source/SageBinaryData/SageBinaryData/Audio.cs` structure is 28 bytes.
The extra four bytes are not a trailing field: the reference uses an inline
subtitle string length/pointer pair, whereas the legacy model uses a pointer
to an AnsiString. Subsequent fields shift four bytes.

| Offset | Reference/observed EP1 field | Legacy offset |
| --- | --- | --- |
| 0 | BaseAssetType, four bytes | 0 |
| 4 | Subtitle length | Pointer to string at 4 |
| 8 | Subtitle pointer | No inline pointer slot |
| 12 | NumberOfSamples | 8 |
| 16 | SampleRate | 12 |
| 20 | HeaderData pointer | 16 |
| 24 | HeaderDataSize | 20 |
| 28 | NumberOfChannels byte, alignment padding | 24 |

Reference: `Working RA3 Compiler for Reference/tools/BinaryAssetBuilder.AudioCompiler.dll`,
SHA256 `A14D15ADDEF502C2DF6F6BA272D6098C4C131FBEA9C3ED0149C11C4556347139`.
`BinaryAssetBuilder.AudioCompiler.Plugin.ProcessAudioFileInstance`, token
`0600011B`: IL_02F1/0322 marshals the char string directly at runtime+4;
IL_033B selects channels at +28; IL_0346/0351 select samples/rate at +12/+16;
IL_035C/0367 select header pointer/size at +20/+24. The streamed branch uses
Tracker.Push<char> at IL_0A59–0A5F and writes header size at IL_0A72–0A76.

`AudioFileRuntimeProbe.Parse` is a checked serialized-envelope reader, not an
unsafe native marshaler. Legacy models and production plugins remain unchanged.

## Actual EnglishAudio results

Input: `D:\TEMP\Red Alert 3 Uprising Source Data\EnglishAudio\data\audio.manifest`.
SHA256 `0CF62FBECC89F15746E523042A6F0422869D98FE41EB742C3104324A81D9A2E4`.
All **12,951** AudioFile records passed TypeID `166B084D`, TypeHash `53C81E47`,
Tokenized 0, zero references/imports, runtime range/scalar/padding, subtitle
terminator and exact relocation-slot checks.

- **11,671**: no inline encoded header; HeaderData/Size both zero.
- **1,280**: eight-byte inline header; relocations at 8 and 20.
- All inline headers agree with native rate/channels/samples.
- BIN/RELO/IMP stock signatures are `BABB0000`/`BABE0000`/`BAB10000`, followed
  by checksum `395CE3A2`. Aggregate lengths match the manifest. These are not
  the zero-prefix owned diagnostic streams.

| Independent selected AudioFile | Instance ID | Native / RELO bytes | Samples / rate / channels | Inline |
| --- | --- | --- | --- | --- |
| WImpact_DebrisVsGrounda | 3CE327EB | 76 / 8 | 48000 / 48000 / 1 | absent |
| Aeva_KeepCommandDirective | A2304C7F | 88 / 12 | 373654 / 48000 / 2 | offset 80, size 8 |

Voice inline bytes: `0604BB804005B396`. Across all 1,280 records, byte 1 shifted
right two plus one equals channels; bytes 2–3 are the big-endian rate; bytes 4–7
contain samples in the low 30 bits and `40000000` in the high two bits. These
are cross-checked observations, not a complete codec specification. The codec
tag and compressed frames remain opaque. Grouped prefixes: `0600BB80` (898),
`0604AC44` (1), `0604BB80` (334), `060CBB80` (10), `0614BB80` (37).

Existing `source/BinaryAssetBuilder.AudioCompiler/BinaryAssetBuilder/AudioCompiler/Plugin.cs`,
`ProcessAudioFileInstance`, makes the intended split explicit: streamed output
moves SNS to CustomDataPath and embeds SNR into HeaderData; RAM output moves SNR
directly to CustomDataPath. This supports the streamed/RAM interpretation, not
encoder compatibility. Sampled voice custom data starts
`000014A600000480C9F8000900F2D617` (205,056 bytes). Sampled RAM Kenji custom data
starts `0404BB80000948E2000B06B8000948E2` (722,624 bytes). Only the first 16 bytes
of four representative custom files were read. Complete custom-data inventory,
block boundaries, decoding, seeking and codec version compatibility are unverified.

## Reproduce and limits

```powershell
# Reborn: inspect existing game streams without invoking audio.dll or modifying game files.
.\source\BinaryAssetBuilder.ManifestInspector\bin\x86\Release\net8.0\BinaryAssetBuilder.ManifestInspector.exe audiofile-runtime-self-test
.\source\BinaryAssetBuilder.ManifestInspector\bin\x86\Release\net8.0\BinaryAssetBuilder.ManifestInspector.exe audiofile-runtime-audit "D:\TEMP\Red Alert 3 Uprising Source Data\EnglishAudio\data\audio.manifest"
```

`AudioFileRuntimeProbe.Run` accepts unpacked linked/prefixed v7 EP1 streams,
not arbitrary audio profiles. Manifest cap: 16 MiB / 65,536 entries. Each read
is at most 4 KiB; subtitles at most 1,024 bytes. It seeks fixed prefixes,
optional strings/headers and small relocation slices, never loading full BIN
or custom payloads. Output is four samples and at most sixteen signature groups.
Custom lookup uses the exact type/hash/instance/hash filename; missing sampled
custom files are reported, not interpreted as envelope failure. This is not
a complete cdata packaging validator.

The compiler suite now has **86 groups**, all executed. The new group pins two
actual prefixes and tests synthetic ranges, legacy/truncated headers, overflowing,
dangling/overlapping pointers, scalar/padding errors, contradictory embedded
header fields and wrong stock stream magic/checksum/length. Actual full EnglishAudio
audit, diagnostic build suite and layout suite passed separately.

## Next gates

1. Inventory cdata and recover SNS block sizes/end markers with capped reads;
   correlate identity/native header. Prefix agreement alone is not decoding proof.
2. Audit current/reference audio.dll imports/exports and codec API differences;
   then attempt owned WAV → SNR/SNS output in a temporary directory with explicit
   field/byte validation, not a game install.
3. Implement an isolated 32-byte EP1 serializer, preserving the KW ABI.
4. Open checked AudioFile processing only after encoding, streamed/RAM packaging,
   type fingerprints, dependencies and independent validation are proven.
5. Validate real Uprising loading separately. No production admission, MP3
   equivalence, complete SDK packaging or game compatibility is established here.
