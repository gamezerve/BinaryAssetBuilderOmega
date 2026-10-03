# Fixed local AudioEvent / Multisound / FX stream evidence

Date: October 3, 2026. This is a fixed owned test graph, not general
`diagnostic-build` AudioEvent admission, AudioFile encoding or a playable SDK.
Overall engineering effort remains approximately 50% complete / 50% remaining.

## Graph and artifact

`AudioEventFXStreamSmokeTest.Run` writes a three-level Include graph under its
own unique temporary directory. Current core dependency preparation selects:

1. `event.xml`: RebornLocalAudio, an isolated checked AudioEvent referencing
   AudioFile:WImpact_DebrisVsGrounda and AudioFile:WImpact_DebrisVsGroundb.
2. `sound.xml`: RebornLocalMultisound, PLAY_ONE with one local AudioEvent child.
3. `fx.xml`: RebornLocalFX, one Sound nugget referring to local Multisound.

The leaf also declares an unused AudioEvent referencing a missing file; it
remains unselected and does not enter the compiled closure. External AudioFile
records resolve by metadata only; no audio payload is rebuilt or read.

The test schema `tests/fixtures/AudioEventFXPipeline.xsd` reuses official
components and separately drift-checked FX enum declarations.
Only explicitly isolated AudioEvent/Multisound/FX plugins are mapped.

| Compiled root | Native BIN/RELO/IMP bytes | Strong dependencies |
| --- | --- | --- |
| AudioEvent | 188/16/12 | 2 external AudioFiles |
| Multisound | 44/8/8 | local AudioEvent |
| FXList | 80/12/8 | local Multisound |
| Total | 312/36/28 | 4 ordered tuples, 32 manifest bytes |

Version-7 manifest has a four-byte container prefix and aggregate hash 5454A8E9.
Each native stream has its eight-byte linked header, making total linked
BIN/RELO/IMP sizes **320/44/36**. Each asset retains its actual source filename.
Runtime mapping names use the existing canonical backslash format.

The owned artifact consists of diagnostic.manifest, diagnostic.bin,
diagnostic.relo, diagnostic.imp and DIAGNOSTIC_ONLY.txt. The notice explicitly
identifies newly compiled local AudioEvent payloads and does not say these
payloads were left external. Prior graph notices remain unchanged.

## Admission and independent readback

Shared `BoundedDiagnosticBuild.ValidateExternalDependencies` now checks selected
external AudioFile TypeID 166B084D against stock TypeHash **53C81E47** and
Tokenized 0, alongside the preexisting AudioEvent/Multisound/shader rules.
Each selected tuple must occur exactly once across explicit mapped metadata.
Duplicate selected identities both across files and inside one parsed document
reject. Complete prepared tables are required for every local root.
This gate does not claim native AudioFile stream/header/codec compatibility.

The fixed graph reuses the existing checksum, runtime mapping and serialization
seams; it does not call OutputManager production commit/link functions.
`BoundedDiagnosticBuild.Verify` round-trips independent ManifestReader and
Utility.Manifest views, headers, exact lengths, entry order, source identities,
concrete references and all native slices. Separate checks assert:

- AudioEvent volume/control/defaults, 152-byte root and 12-byte weighted children,
  explicit slider/PitchShift pointers and values.
- Multisound PLAY_ONE/count/pointer/default weight/absent optional pointers.
- One-biased selectors at linked BIN offsets 160,172,212,316, each choosing its
  exact AudioFile or local AudioEvent/Multisound tuple.

## Failure, refresh and actual-game evidence

Tests include wrong AudioFile hash, tokenized metadata, duplicate and missing
selected records, followed by restored metadata admission.
Twenty independent corruptions alter both owned output and matching expected
snapshot, so plain byte-snapshot equality cannot hide the corruption:

- 12 stream magic/checksum/truncation/extra-tail cases.
- 4 zeroed native selectors.
- 2 relocation/import offset corruptions.
- 2 manifest reference-order/type corruptions.

Every corruption rejects during independent readback, then original owned bytes
are restored. Existing output directories cannot be overwritten.
Two repeated missing-leaf preparation attempts clear all three prepared tables;
all three plugin entries and the shared stream gate reject revoked state.
Restoring mappings recovers identical compiled buffers.

Changing the included event's second leaf to WImpact_DebrisVsGroundc refreshes
the concrete leaf and both local ancestors. A newly serialized artifact validates
the changed identities; restoring the source recovers original native bytes.
Repeated compilation and serialization are byte-identical.
Production/output/cache/reuse policies remain closed and Settings are restored.

The optional actual-game run uses:
`D:\TEMP\Red Alert 3 Uprising Source Data\EnglishAudio\data\audio.manifest`,
SHA-256 `0CF62FBECC89F15746E523042A6F0422869D98FE41EB742C3104324A81D9A2E4`.
Its selected two AudioFile records pass unique fingerprint checks and produce the
same local native buffers and valid linked readback. Synthetic default manifests
are metadata-only fixtures, not audio payload goldens.

## Reproduce and next gates

```powershell
.\source\BinaryAssetBuilder.ManifestInspector\bin\x86\Release\net8.0\BinaryAssetBuilder.ManifestInspector.exe audioevent-fx-stream-self-test
.\source\BinaryAssetBuilder.ManifestInspector\bin\x86\Release\net8.0\BinaryAssetBuilder.ManifestInspector.exe audioevent-fx-stream-self-test "D:\TEMP\Red Alert 3 Uprising Source Data\EnglishAudio\data\audio.manifest"
```

Registered in `compiler-self-test`: 84 declared groups. Structural inventory
remains 785/1,390 models and 762/1,390 typed marshallers.

Next: narrow general diagnostic AudioEvent admission with approved source and
metadata snapshots, immutable publication and actual local dependency ordering.
Wider AudioEvent options/stock evidence, LimitGroup, AudioFile runtime/codec
generation, aggregate type tables, packaging and game loading remain open.
