# Isolated EP1 AudioFile runtime serialization

Date: October 4, 2026. Two real Uprising native records match byte for byte.
The encoder experiment now produces independently checked raw runtime buffers.
This is not yet an AudioFile XML compiler, linked mod package or game-loading proof.
Overall effort remains approximately 50% complete / 50% remaining; one narrow
audio gate does not close the outstanding type-table, processor and game gates.

## Implementation and boundary

`source/BinaryAssetBuilder.XmlCompiler/BinaryAssetBuilder/XmlCompiler/Ra3Ep1AudioFileRuntimeSerializer.cs`,
`Ra3Ep1AudioFileRuntimeSerializer.Serialize`, is an explicit managed wire serializer.
Its `NativePrefix` is 32 bytes; the legacy `SageBinaryData.AudioFileRuntime` remains
28 bytes. No production plugin, processor registry or old runtime definition changes.

| Offset | Serialized field |
| --- | --- |
| 0 | Zero BaseAssetType |
| 4 / 8 | Inline subtitle length / relative pointer |
| 12 / 16 | Samples / sample rate |
| 20 / 24 | Optional streamed header pointer / size |
| 28 | Channel count, followed by three zero padding bytes |

The subtitle starts at 32, includes a NUL terminator and zero padding to four-byte
alignment. A streamed eight-byte SNR follows that aligned string. RAM relocation
words are `8, FFFFFFFF`; streamed words are `8, 20, FFFFFFFF`. Imports are empty.
These offsets describe wire data, not pointers usable in a live process.

Admission is deliberately narrow: Win32 only, nonempty printable ASCII subtitles
of at most 1,024 characters, samples 1..3FFFFFFF, rates 400..96,000, channels
1/2/4/6, and either no header or exactly eight bytes. Embedded channels/rate/sample
count and the streamed sample flag must agree with runtime fields. The codec tag
remains opaque. This does not claim support for every schema-valid AudioFile.
BIN/RELO are fresh owned arrays, and the input header is copied.

## Independent stock comparisons

`AudioFileSerializationSmokeTest.Run` checks literal full hex goldens separately
from the runtime reader, field sizes/offsets, alignment boundaries, ownership and
platform/subtitle/scalar/header failures. It is the 91st declared compiler test
group; all 91 groups were executed. Default tests do not invoke native codecs.

The optional actual-manifest comparison also ran successfully against:
`D:\TEMP\Red Alert 3 Uprising Source Data\EnglishAudio\data\audio.manifest`.
It audits all 12,951 runtime envelopes first, then reads only selected bounded
native slices. Selected metadata must be AudioFile type 166B084D, type hash
53C81E47, non-tokenized, and have no references/imports.

| Stock record | Instance ID | BIN / RELO / IMP bytes |
| --- | --- | --- |
| WImpact_DebrisVsGrounda | 3CE327EB | 76 / 8 / 0 |
| Aeva_KeepCommandDirective | A2304C7F | 88 / 12 / 0 |

Both complete native slices match, not just their 32-byte prefixes. The first
has 48,000 samples, 48 kHz, mono and no inline header. The second has 373,654
samples, 48 kHz, stereo and header `0604BB804005B396`. No whole BIN dump is taken.

CLI: `audiofile-serializer-self-test [manifest...]`. The independent managed
goldens run without a manifest; optional manifests add actual stock comparisons.

## Encoder-to-runtime proof

`AudioEncoderPoc.Run` retains its pinned DLL, Windows x86 and fresh-owned-directory
requirements. Following native encoding, it serializes runtime fields, rereads
them with `AudioFileRuntimeProbe.Parse`, and validates custom block framing with
`AudioCustomDataProbe.Inspect` using those reread fields and inline bytes.
It no longer uses a fabricated runtime header for that framing check.

Actual successful output directory:
`C:\Users\drknt\AppData\Local\Temp\Reborn-AudioEncoder-cca40c6a131d4b628eedf9fde5ef184e`.

| Artifact | Bytes |
| --- | --- |
| input.wav | 24,044 |
| ram.snr | 7,160 |
| ram.runtime.bin / ram.runtime.relo | 80 / 8 |
| streamed.snr / streamed.sns | 8 / 7,232 |
| streamed.runtime.bin / streamed.runtime.relo | 88 / 12 |

The fixed input remains 12,000 PCM16 samples, mono 48 kHz, codec 29, SND output
container 39. RAM has one checked block; streamed output has four. PoC RAM size
80 differs from the stock golden's 76 because the subtitle is different.
No IMP file is necessary here because both native buffers have zero imports.
Files use exclusive creation; no existing game or user output is overwritten.

Custom payload SHA256 values match the preceding encoder experiments:

- RAM: `78EB78241914FB7F27512F8FC339FD250A90C1C4FD823E60E0C93C833141A9AD`.
- Streamed SNS: `A1D26B42C1967A3CAC5DB166FA1A68041A54A251CAEF9E0098B1291E796DF2EC`.
- Inline streamed SNR bytes: `0400BB8040002EE0`.

These raw outputs have no linked stream header, manifest entry or identity-based
custom-data filename. They must not be presented as a loadable mod package.

## Validation and next gates

Release/x86 ManifestInspector and BinaryAssetBuilder builds pass. The complete
91-group compiler suite, aggregate diagnostic build tests, native layout tests,
enum synchronization check and whitespace check pass. Inventory remains
785/1,390 complex models and 762/1,390 typed marshallers; this helper is not a
registered schema-generated marshaller.

Next work is a checked isolated AudioFile compiler entry: freeze/revalidate XML
and codec settings, bind concrete EP1 identity/fingerprint metadata, own custom
payload publication, then prove manifest/BIN/RELO/IMP plus custom-data packaging
in a mixed diagnostic stream. Unsupported inputs must reject before publication.
Wider audio formats, native error recovery, production registry activation,
final AllTypesHash and actual Uprising loading remain separate validation gates.
Do not globally replace the legacy ABI or renumber LAYER3 container 34.
