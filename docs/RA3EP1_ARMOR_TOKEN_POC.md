# ArmorTemplate native/tokenized PoC — 2026-10-01

## Outcome

The current 32-byte ArmorTemplate native root and 8-byte ArmorListType stride
match the official RA3 compiler. However, `Plugin.HandleType<T>` currently only
marshals the native record and calls `WriteAssetBuffer`; it never performs the
official tokenizer stage. Correcting the TypeHash/Tokenized flag alone would
therefore emit the wrong BIN representation.

`Ep1ArmorTokenizer.Tokenize` now reconstructs this missing stage in an
explicit, experimental Win32-only helper, separate from production registry
activation. A controlled XML fixture compiles through official EP1 schema
declarations, production `Marshaler.Marshal` and this helper. The resulting
bytes exactly match two game samples:

| Source | Asset | BIN / RELO / IMP |
|---|---|---:|
| `D:\TEMP\Red Alert 3 Uprising Source Data\Static Data\data\static.manifest` | A01_CoastalGunArmor | 744 / 8 / 0 |
| `D:\TEMP\Red Alert 3 Uprising Source Data\WorldBuilder\data\worldbuilder.manifest` | TH_ShinzoHouseArmorSet | 128 / 0 / 0 |

WorldBuilder's local manifest does not contain the static A01 asset. Its
empty-list house armor is compiled separately with `Default="10"`; all other
scalar defaults come from the schema. The test does not pretend that every
static asset is present in a WorldBuilder stream. The 1,394,571,528-byte
WorldBuilder BIN is read by bounded random access only: one 128-byte asset and
the eight-byte linked header. Companion headers are checked too; no full dump.

## Reverse-engineering evidence

Reference DLL:
`D:\OneDrive\Documents\GitHub\BinaryAssetBuilderOmega2\Working RA3 Compiler for Reference\tools\BinaryAssetBuilder.Tokenizer.dll`.

| Method token | Evidence |
|---|---|
| 0x06000030 | ArmorTemplate native marshalling: five Percentage scalars at offsets 4, 8, 12, 16, 20; Armor list at 24 |
| 0x06000845 | ArmorTemplate tokenization: six field records plus zero terminator, each record 12 bytes |
| 0x06000A42 | Armor list tokenization: count record followed by one nested-table record per item |
| 0x06000844 | ArmorListType tokenization: Damage and Percent records plus zero terminator |

These exact method tokens belong to the inspected reference DLL, not arbitrary
compiler versions. The behavior was additionally confirmed against EP1 payloads.

Each token is three little-endian uint32 words: field/name hash, type/tag hash,
and an asset-relative native value offset or nested table offset. Root fields
use production FastHash names; numeric tags come from the official IL and
matching game bytes. Token value offsets are **not** RELO entries. Only the
actual native Armor list pointer is relocated, with a trailing `0xFFFFFFFF`.

For `n` Armor entries:

| Region | Size |
|---|---:|
| Root field tokens + terminator | 84 |
| List count/item tokens | 12 + 12n |
| Nested Damage/Percent tokens + terminators | 36n |
| Native root + item values | 32 + 8n |
| Total BIN | 128 + 56n |

The native root begins at `96 + 48n`. A populated list has one RELO source at
`nativeRoot + 28`, pointing to `nativeRoot + 32`, followed by the sentinel.
An empty list needs no RELO or IMP bytes. This explains both observed sizes.

## Implementation and regression scope

- `source/BinaryAssetBuilder.XmlCompiler/BinaryAssetBuilder/XmlCompiler/Ep1ArmorTokenizer.cs`: validates canonical native input, creates tokens and adjusts the single native relocation without mutating input.
- `source/BinaryAssetBuilder.ManifestInspector/ArmorTokenSmokeTest.cs`: schema defaults, all five non-default scalars, eleven-item fixture, deterministic repeated compilation, invalid XML/native rejection, linked metadata/stream writing and optional golden comparison.
- `tests/fixtures/ArmorTokenPipeline.xsd`: includes official EP1 armor/damage declarations; BaseInheritableAsset is a deliberately minimal harness stub.
- `tests/fixtures/ArmorTokenProbe.xml`: controlled values reconstructed from the game native payload, **not** claimed as recovered original EA XML.
- The harness extracts Percentage directly from official EP1 `Includes/Base.xsd`, retaining its restriction instead of accepting arbitrary strings.
- `AssetStreamProbe.ReadRange` is shared for targeted reads. The helper's input contract is canonical little-endian native armor without imports/custom data; it is not a generic SAGE tokenizer.
- `Utility.Manifest.MaxInstanceChunkSize` was incorrectly reading MaxImportsChunkSize. The PoC checks distinct 744/8/0 maxima and now guards the corrected getter.

This adds one compiler test group, bringing the total to 49. Model/marshaller
inventory remains 784/760; tokenization is a different pipeline stage.

## Reproduce and inspect the one-asset fixture

Build the inspector for Release/x86, then run:

```powershell
# Reborn: validate the populated static golden and write only isolated diagnostic artifacts.
& .\source\BinaryAssetBuilder.ManifestInspector\bin\x86\Release\net8.0\BinaryAssetBuilder.ManifestInspector.exe armor-token-self-test `
  artifacts/ep1-armor-token-static `
  'D:\TEMP\Red Alert 3 Uprising Source Data\Static Data\data\static.manifest'

# Reborn: validate the separate empty-list golden using a small WorldBuilder BIN slice.
& .\source\BinaryAssetBuilder.ManifestInspector\bin\x86\Release\net8.0\BinaryAssetBuilder.ManifestInspector.exe armor-token-self-test `
  artifacts/ep1-armor-token-worldbuilder `
  'D:\TEMP\Red Alert 3 Uprising Source Data\WorldBuilder\data\worldbuilder.manifest'
```

Omit the game path for fixture-only tests. Each output directory contains
`armor-token.manifest`, `.bin`, `.relo`, `.imp` and `DIAGNOSTIC_ONLY.txt`.
The generated populated fixture always uses the eleven-item example, even
when the optional game comparison validates a different house-armor example.
The manifest uses observed Armor TypeId `0x3A6C5E8E`, TypeHash `0xA0E237D8`,
EP1 AllTypesHash `0x5454A8E9` and Tokenized=true. InstanceId uses the production
case-insensitive hash function. Linked stream headers use the existing
`OutputManager.WriteLinkedStreamHeader` implementation and matching test checksum.
Both manifest readers accept its metadata; real stream headers/checksums and
golden payload bytes are compared separately.

## Boundaries and remaining gates

This is a diagnostic PoC, **not a playable mod**. Its checksum is an arbitrary
fixed test marker and InstanceHash is zero; it does not establish production
cache/checksum policy. `OutputManager.CommitManifest` is not bypassed or called
with a fabricated production plugin. KW/EP1 registrations and the production
AllTypesHash safety gate are unchanged. Utility metadata acceptance with
`validateAssets=false` is not validation of legacy loose `.asset` files.

No big-endian, expression evaluation, inheritance/override merging,
special Damage="ALL" syntax, generalized tokenized types, packaging or game
loading is claimed. The native helper rejects noncanonical chunk layouts and
imports; schema validity alone does not establish support for every schema feature.

Next: design an explicit EP1 processor/type profile and route only validated
armor through its tokenizer. Audit ProcessingHash and tokenized cache reuse;
add a production schema/document build, then an isolated packaged load test.
Do not mark the complete registry or SDK ready based on this single processor.
The overall rounded estimate remains approximately 50% complete / 50% remaining.
