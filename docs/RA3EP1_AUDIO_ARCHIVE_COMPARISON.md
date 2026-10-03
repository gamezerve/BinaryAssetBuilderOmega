# Original audio archive comparison and read-only reconciliation

Date: October 3, 2026. The four previously rejected custom-audio records are
valid in the original archive and differ from the unpacked copies. No codec
rules were relaxed, no game files were changed, and no output was extracted.
Overall effort remains approximately 50% complete / 50% remaining.

## Evidence

Original archive located at
`D:\SteamLibrary\steamapps\common\Command and Conquer Red Alert 3 Uprising\Data\EnglishAudio.big`.
It has 12,955 entries and 775,841,450 stored bytes. Only the audio manifest and
the four exact selected cdata entries were read/decompressed, not the full BIG.
They are wrapped in RefPack; this is container decompression, not audio decoding.

The archive's decoded `data/audio.manifest` is byte-identical to
`D:\TEMP\Red Alert 3 Uprising Source Data\EnglishAudio\data\audio.manifest`,
SHA256 `0CF62FBECC89F15746E523042A6F0422869D98FE41EB742C3104324A81D9A2E4`.
Thus selected type/hash/instance/hash identities belong to the same manifest.

| AudioFile | Stored archive bytes | Decoded original bytes | Unpacked bytes | First differing byte | Original blocks |
| --- | --- | --- | --- | --- | --- |
| A08_KirovEntry | 1,795,953 | 1,832,528 | 1,048,588 | 1,042,042 | 1 |
| WAGreat_Outdoors_quad | 3,754,320 | 3,760,576 | 2,098,482 | 2,093,726 | 3,691 |
| WANight_Transylvania_quad | 5,475,991 | 5,478,016 | 5,245,542 | 3,142,630 | 4,905 |
| WAShima_Compound1_quad | 4,295,420 | 4,301,760 | 4,197,758 | 4,194,331 | 4,343 |

All four original records pass existing strict header/rate/channel/sample,
block-length/flags/sample-sum and EOF rules. Every local copy is shorter and
has a byte difference before local EOF. This establishes defective local
reference copies relative to the original entries, not the precise extractor
implementation or bug that produced them. It is not an unknown audio format
requiring weaker block validation.

Decoded original SHA256 fingerprints:

- A08_KirovEntry: `F28CEC2E84B0D1FA5131397B79BFB671B351F7DC187CBD936EEF5B2BB4A999EB`
- WAGreat_Outdoors_quad: `667C86263565ABBE5AB2E7A3F8E032AEA1D80B6EB3E3E0B90A8C4FBA004A1D5E`
- WANight_Transylvania_quad: `9E376AA53EFB7021B7E76853E9B8882FDFE12C7FF583BF4750583A5C7EDC92C3`
- WAShima_Compound1_quad: `71A9BFD507E4B7878751328559CFD98A106E542241D75AD2B5EF51E1EBAF868F`

## Explicit in-memory overlay

`audio-custom-reconciled-audit` first checks native envelopes, exact manifest
equality, unique BIG entries and the four original records' framing. Only then
does it use those four verified decoded buffers for a second full custom audit.
All other records still come from the local unpacked corpus.

Actual result: **12,951 validated, zero rejected, 281,614 blocks**, representing
**803,322,416** custom file bytes. Opaque header tags: 04 (11,662), 06 (1,289).
Framing uses eight-byte seeks and skips codec bodies; the archive phase must
fully decode the four selected small RefPack entries into memory. The represented
byte total is not the amount of data read or a full audio payload dump.

This is a reconciled mixed-source framing proof, **not** a claim that all local
unpacked files were repaired or that every other cdata file is byte-identical to
its original archive entry. The default local-only `audio-custom-audit` remains
strict and still reports INCOMPLETE / exit 1 for those four copies. No compiler,
encoder, source graph, production registry or build packaging uses the overlay.

## Commands and implementation

```powershell
# Reborn: compare only the selected original records and opt into a read-only in-memory overlay; local/game files remain unchanged.
.\source\BinaryAssetBuilder.ManifestInspector\bin\x86\Release\net8.0\BinaryAssetBuilder.ManifestInspector.exe audio-archive-self-test
.\source\BinaryAssetBuilder.ManifestInspector\bin\x86\Release\net8.0\BinaryAssetBuilder.ManifestInspector.exe audio-archive-compare "D:\TEMP\Red Alert 3 Uprising Source Data\EnglishAudio\data\audio.manifest" "D:\SteamLibrary\steamapps\common\Command and Conquer Red Alert 3 Uprising\Data\EnglishAudio.big"
.\source\BinaryAssetBuilder.ManifestInspector\bin\x86\Release\net8.0\BinaryAssetBuilder.ManifestInspector.exe audio-custom-reconciled-audit "D:\TEMP\Red Alert 3 Uprising Source Data\EnglishAudio\data\audio.manifest" "D:\SteamLibrary\steamapps\common\Command and Conquer Red Alert 3 Uprising\Data\EnglishAudio.big"
```

`source/BinaryAssetBuilder.ManifestInspector/AudioArchiveComparisonProbe.cs`:
`ReadCorrections` returns only the fixed four identities after byte-identical
manifest and native/framing checks; `Find` requires unique exact entry names
after separator normalization; `Decode` caps stored and decoded entries at
16 MiB; `FirstDifference` reads the local file in 4 KiB chunks.
`AudioFileRuntimeProbe.Run` requires four matching correction identities when
an overlay is explicitly supplied and otherwise retains its original behavior.
An unknown/missing entry, different manifest, oversized RefPack output or
failed original framing aborts rather than silently substituting data.

The new test group covers equal/shorter/longer comparisons, differing bytes on
both sides of chunk boundaries, and manifest content/length mismatch rejection.
Full compiler suite: **88 groups passed**; model/marshaller inventory unchanged
at 785/1,390 and 762/1,390. Actual archive comparison and full reconciled audit
were run separately, not inferred from those synthetic comparison tests.

## Remaining gates

The reference-data framing issue is now isolated and does not block further
codec research. Next: native audio library imports/exports and calling-signature
audit, followed by an owned minimal WAV → SNR/SNS experiment and an isolated
EP1 32-byte runtime serializer. Encoded frame validity, seeking behavior,
compiler-generated output, production packaging and real Uprising loading
remain unverified. Do not interpret this audit as a working AudioFile compiler.
