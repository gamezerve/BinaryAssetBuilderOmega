# EP1 ShaderOverride native proof and source variance

## Outcome

Recovered the native ShaderOverride root and ordered rule records from official
compiler metadata/IL, then checked bounded stock EP1 slices. Three supplied XML
literals match BIN/RELO/IMP exactly. PsychicCrush has one source-versus-stock
content difference, not a layout mismatch: a separately compiled, explicitly
labeled stock variant matches exactly. No original XML, schema or game stream
was changed. No asset-specific exception was added to the marshaller.

Observed identity: TypeId `BCC23F6C`, TypeHash `3D5B1D16`, Tokenized=0.
ShaderOverride remains unregistered in the production type table. This is the
fourth focused native asset-family proof, not a usable fourth production processor.

| Asset suffix (ShaderOverride:) | BIN/RELO/IMP | Provenance |
|---|---|---|
| ShaderOverride_ObjectsIronCurtain | 180/52/0 | Supplied source literal |
| ShaderOverride_BuildPlacementCursor | 96/28/0 | Supplied source literal |
| ShaderOverride_ObjectsWithoutXrayEffect | 44/16/0 | Supplied source literal |
| ShaderOverride_PsychicCrush | 460/132/0 | Explicit stock variant; original literal differs at 0x64 |

## Native layout and official compiler evidence

The root is 16 bytes: BaseAssetType at 0, unsigned Priority at 4 (default 1),
Rule count at 8 and Rule pointer at 12. Each rule is 16 bytes:

| Rule offset | Field | Representation |
|---|---|---|
| 0 | IfOriginalShaderIs | Optional pointer to a four-byte FXShaderMaterial POID |
| 4 | ReplaceShaderName | Inline four-byte FXShaderMaterial POID |
| 8 | ReplaceTechniqueName | ANSI string length and pointer, eight bytes |

Material names are POIDs, not strong imports or document WeakReference entries.
The official XSD uses Poid with refType metadata. Actual core document processing
confirms zero strong, weak and file dependencies for all four fixtures. Strings
include a terminating zero; the stored length excludes it. Rules retain source
order. An absent condition stays null, rather than allocating a zero-ID record.

Official reference assembly:
`Working RA3 Compiler for Reference/tools/BinaryAssetBuilder.XmlCompiler.dll`,
SHA-256 `D4B78E2DA951DD458357BEE7B358E6BC9689D1987D55867254D68B3F87BBF0E6`.
The prior metadata/IL inspection identifies rule marshaller `<Module>` token
`0600006C` (RVA `70BC4`) and root marshaller `0600006E` (RVA `8214C`).
Rule calls target the optional typed-ID pointer (`060003B5`), inline material ID
(`06000766`) and char string (`0600036E`), in that order. Root marshalling sets
Priority at 4 and the rule list at 8 (`060003B6`). Metadata reports root/rule sizes
of 16. These are evidence from the reference assembly, not callable managed
methods introduced by this port.

The old one-byte ShaderOverride placeholder was used only as a reference target.
Replacing it with the real root does not enlarge AssetReference's four-byte
import record. The modifier import regression still matches all four stock roots,
including IronCurtain's optional Shader pointer.

## PsychicCrush source-versus-stock difference

Supplied source's sixth rule (zero-based index 5): condition `Lightning.fx`,
replacement `Null.fx`, technique `Default`. The stock slice retains `Lightning.fx`
as its replacement. The word is at `16 + 5 * 16 + 4 = 100` (`0x64`):

- Supplied source: `A9C4BAA0` = invariant POID hash of `Null.fx`.
- Stock: `2AD67137` = invariant POID hash of `Lightning.fx`.

The initial whole-buffer comparison found exactly this one differing word.
`CompileStockPsychicVariant` clones the fixture, checks the exact source rule,
changes only this replacement attribute and compiles normally. Assertions prove
identical sizes, identical bytes before 100 and after 103, identical RELO, empty
IMP and an unchanged original XML rule. The resulting entire BIN/RELO matches
the stock slice. Console output labels it as a stock variant, never a literal match.
Unknown additional stock differences still fail the comparison.

The reason the supplied XML and this shipped binary differ is not established.
Do not silently treat supplied/decompiled XML as an exact stock build input or
change compiler behavior to conceal source differences. PsychicSlam and the other
five overrides have not been compared in this block.

## Source and schema fingerprints

Supplied XML:
`D:\OneDrive\CNC Files\CnC_Modding_Support-main (Official XML, Schema, Script, Shader, Maps)\Uprising\Xml (Uprising)\GlobalData\ShaderOverrides.xml`
SHA-256 `3CFD843B194E787D7E59F5DE440A6D9C4B3D2061D20F36173FA96E5F81997FC1`.
The four checked-in source fixtures retain their literal rules; unrelated Includes
are not needed for these definitions. No original source files were edited.

`schemas/ra3ep1/xsd/AssetTypeShaderOverride.xsd` SHA-256
`FC1C8620A1072639736606F4F43C1EE1F2A55EFF46BA79CF71BB6A7624DEA699`.
The focused graph uses official Base, AssetBase, Ref and ShaderOverride definitions.

Stock manifest:
`D:\TEMP\Red Alert 3 Uprising Source Data\Static Data\data\static.manifest`
SHA-256 `74FACD9056EEE20942BB274A924BEFFB03D7ADBB71B9609E804A39619B2C6081`.
Before comparison, the harness checks linked EP1 version/aggregate, type identity,
tokenization, dependency count, exact chunk lengths and stream magic/checksums.
Only the selected asset slices are read; no full BIN dump is required. All four
assets must be present in a supplied manifest; a partial proof is rejected.

## Implementation and reproduction

- `source/SageBinaryData/SageBinaryData/ShaderOverride.cs`: root and rule models;
  reference-only placeholder removed from `AttributeModifier.cs`.
- `source/BinaryAssetBuilder.XmlCompiler/Marshaler.ShaderOverride.cs`: ordered
  rule and root marshallers; no production processor registration.
- `source/BinaryAssetBuilder.ManifestInspector/ShaderOverrideNativeSmokeTest.cs`:
  defaults, deterministic output, schema negatives, actual unregistered document
  processing, explicit source variance checks and bounded stock comparisons.
- `tests/fixtures/ShaderOverridePipeline.xsd` and `ShaderOverrideProbe.xml`:
  focused official schema graph and four supplied source literals.

Run with the Release/x86 ManifestInspector executable:

```text
shader-override-self-test "D:\TEMP\Red Alert 3 Uprising Source Data\Static Data\data\static.manifest"
modifier-import-self-test "D:\TEMP\Red Alert 3 Uprising Source Data\Static Data\data\static.manifest"
compiler-self-test
layout-self-test
```

Without a manifest, the shader test runs the synthetic/source/document checks
and proves the explicit stock-variant delta, but does not claim a local stock
comparison. The full compiler runner includes this mode. Full Release/x86 builder
and inspector builds, all 65 compiler groups, layout checks and 33 schema enum
mappings pass. Inventory is 785 models and 762 typed marshallers of 1,390.
Overall effort remains approximately 50% complete; this bounded leaf proof does
not close the larger type-table, shader compilation or runtime gates.

## Remaining gates and next checkpoint

Next: isolate a diagnostic ShaderOverride compiler profile with fresh schema/type
identity and POID eligibility checks, retaining production/cache restrictions;
then test normalized modifier-to-shader resolution through that profile. Expand
stock comparisons to the remaining overrides without assuming source equality.

Unported FXShaderMaterial compilation/custom data, inheritance and expressions,
complete EP1 registration/AllTypesHash, native cache reuse, SDK scripts,
WorldBuilder packaging and actual Uprising mod loading remain open. No full SDK
build or in-game compatibility is claimed by these tests.
