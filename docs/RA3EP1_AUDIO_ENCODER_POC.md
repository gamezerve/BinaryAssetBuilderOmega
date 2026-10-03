# Native audio API audit and minimal WAV encoder PoC

Latest follow-up: [Fixed local AudioEvent to AudioFile package](RA3EP1_LOCAL_AUDIO_PACKAGE.md)
also emits a separate three-entry diagnostic package with explicitly prepared
local event references. General audio compilation and game loading remain closed.

Packaging follow-up: [Fixed two-AudioFile diagnostic package](RA3EP1_AUDIOFILE_PACKAGE.md)
adds verified v7/native/custom output to the opted-in encoder. InstanceHash is
diagnostic and this still does not establish production or game compatibility.

Subsequent input gate: [Checked authored AudioFile input profile](RA3EP1_AUDIOFILE_INPUT_PROFILE.md)
freezes official-schema settings, identity and owned PCM before native initialization,
then rechecks them before encoding and runtime output. Current PoC settings are
derived from that preparation; the mono RAM/streamed custom hashes are unchanged.

October 4 follow-up: [Isolated EP1 AudioFile runtime serialization](RA3EP1_AUDIOFILE_SERIALIZATION.md)
adds raw runtime output to the opt-in experiment and validates custom blocks
against independently reread fields. The original observations below remain
historical evidence; a production AudioFile compiler and linked packaging are still absent.

Date: October 3, 2026. An isolated encoder experiment succeeded for one fixed
mono PCM input and codec setting. This is not an AudioFile compiler, decoded
audio/playback proof or Uprising loading proof. Overall effort remains
approximately 50% complete / 50% remaining.

## Concrete finding

The current AudioCompiler source calls `SIMEX_create(..., Type.LAYER3, ...)`
with **34**. The audited library rejects this with:
`The file format passed to SIMEX_create is not supported.`

Reference `Working RA3 Compiler for Reference/tools/BinaryAssetBuilder.AudioCompiler.dll`,
`Plugin.ProcessAudioFileInstance`, token 0600011B, **IL_0524** pushes **39**
before the SIMEX_create call at IL_0528. `Native.Audio.Type.SND` already has
value 39 in current source. Using SND 39 in the separate PoC succeeds.

Do not globally renumber LAYER3 34: the MP3 passthrough path uses it for input
identification. An output-container choice and an input-format identity are
not interchangeable. Production AudioCompiler and legacy runtime layouts have
not been changed; this finding belongs in the future isolated EP1 entry.

## PE/API evidence, without loading libraries

`NativeAudioApiProbe.Read/Run` reads PE directories only. Current root audio.dll
is I386/PE32, no CLI metadata, **23 named exports**, including all **20**
bindings declared in `source/BinaryAssetBuilder.AudioCompiler/Native/Audio.cs`.
Additional exports: SIMEX_about, SIMEX_filterabout, SIMEX_filterssound.
SHA256 `149DE43E1E7C914B8E44DD5E0CDCBED45DE33890EBD2708C8223B278874A610F`.
The copy under the AudioCompiler project has the same fingerprint.

Current dependencies include KERNEL32, MSVCP140, VCRUNTIME140 and UCRT API-set
DLLs for heap/runtime/stdio/filesystem/string/convert/math/utility. Import DLL
names are inventoried, not every imported function or forwarder/signature.

Reference AudioCompiler is I386/PE32 with CLI metadata and zero named exports;
its SIMEX implementations are native methods inside the mixed-mode module.
Module method evidence includes SIMEX_open(SByte*, Int64, Int32, SINSTANCE**),
SIMEX_create(SByte*, Int32, SINSTANCE**) and integer-returning info/read/write.
It uses MSVCR80/MSVCP80/msvcm80, KERNEL32, ADVAPI32 and mscoree.
SHA256 `A14D15ADDEF502C2DF6F6BA272D6098C4C131FBEA9C3ED0149C11C4556347139`.
The original SDK tools copy at `D:\RA3 MOD SDK (Working Original Copy)` matches.
Zero public exports there is not a missing API bug: it is a different linkage
model. Export names alone do not prove ABI or codec equivalence.

## Opt-in standalone-process experiment

`AudioEncoderPoc.Run` requires Windows x86, unmanaged I386/PE32 and the exact
audited SHA256. It loads only the absolute named library and creates a fresh
`Reborn-AudioEncoder-<GUID>` child under the system temporary directory. A
24,044-byte WAV contains 12,000 signed PCM16 samples: mono, 48 kHz, 250 ms,
440 Hz tone. No user WAV, game file, schema, registry or build setting is changed.

The exercised cdecl bindings use repository-declared signatures with explicit
integer statuses. Native getters agree with the input samples/rate/channels.
The experiment sets codec **29** (the source enum's XAS_INT), RAM play location
2048 or streamed location 4096, and SND container 39. Each output handle closes
once; observed close status is 1. Native info/input handles and the pinned
module are released. Outputs remain available as evidence, not build artifacts.

| Output | Bytes | Declared blocks | Header/tag | SHA256 |
| --- | --- | --- | --- | --- |
| ram.snr | 7,160 | 1 | tag 04 | 78EB78241914FB7F27512F8FC339FD250A90C1C4FD823E60E0C93C833141A9AD |
| streamed.snr | 8 | header only | 0400BB8040002EE0 | not used as a custom payload |
| streamed.sns | 7,232 | 4 | inline SNR tag 04 | A1D26B42C1967A3CAC5DB166FA1A68041A54A251CAEF9E0098B1291E796DF2EC |

Both custom streams pass `AudioCustomDataProbe.Inspect`: header/native scalar
agreement, bounded block lengths, sample sum 12,000 and streamed terminal/EOF.
Two independent successful worker runs produced identical output hashes.
Only container framing was independently checked, not compressed frame decoding
or listening. Tag 04 from codec 29 is observed output; no equivalence to every
stock tag 06 stream is assumed.

Latest successful owned output directory:
`C:\Users\drknt\AppData\Local\Temp\Reborn-AudioEncoder-d160e94925574142ae9c962164fa9e01`.
The initial rejected 34 experiment retained only its own input.wav in a separate
GUID directory. Neither existing user output nor game installation was modified.

## Reproduce

```powershell
# Reborn: metadata-only checks do not initialize native code; the explicit encoder command creates only its own temporary artifacts.
.\source\BinaryAssetBuilder.ManifestInspector\bin\x86\Release\net8.0\BinaryAssetBuilder.ManifestInspector.exe native-audio-api-audit audio.dll
.\source\BinaryAssetBuilder.ManifestInspector\bin\x86\Release\net8.0\BinaryAssetBuilder.ManifestInspector.exe native-audio-api-self-test
.\source\BinaryAssetBuilder.ManifestInspector\bin\x86\Release\net8.0\BinaryAssetBuilder.ManifestInspector.exe audio-encoder-wav-self-test
.\source\BinaryAssetBuilder.ManifestInspector\bin\x86\Release\net8.0\BinaryAssetBuilder.ManifestInspector.exe audio-encoder-poc "C:\Users\drknt\.codex\.chatgpt-projects\g-p-6aa6a9d9e7f48191931d3f26b6511794\BinaryAssetBuilderOmega2-uprising\audio.dll"
```

The full compiler suite now has **90 groups**. The two new default groups inspect
actual current/reference PE data and managed WAV bytes only; **native encoding
is never invoked by the default suite**. PE bounds: 16 MiB image, 4,096 named
exports, 128 imported DLLs, 256-character ASCII names. No PE code is executed
during metadata inspection. Models/marshallers remain 785/1,390 and 762/1,390.

## Next gates

1. Add an isolated EP1 32-byte AudioFile runtime serializer with optional inline
   string/SNR allocation, exact relocation slots and independent stock goldens.
2. Bind generated custom data to an owned stream and validate metadata plus
   packaging; do not reuse the legacy 28-byte AudioFileRuntime.
3. Expand encoding evidence to stereo, alternative rates/codecs and controlled
   bad inputs. Resampling and quality controls were not exercised here.
4. Independently decode/listen to outputs and validate real Uprising loading.
   A narrow successful encoder call does not authorize production registration,
   wider audio options, MP3 passthrough equivalence or complete SDK release.
