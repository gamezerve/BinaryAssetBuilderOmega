# EP1 native import encoding proof

## Outcome and scope

Four source-derived AttributeModifier roots match stock Uprising static data
byte-for-byte in BIN, RELO and IMP, including dependency order and identities.
This closes a shared final-import serialization bug missed by synthetic fixtures.
It does not activate production EP1 output or expand the experimental modifier
profile's no-dependency eligibility. No game file was written.

| Stock asset (AttributeModifier) | BIN/RELO/IMP bytes | Dependencies |
|---|---|---:|
| AttributeModifier_IronCurtain | 160/20/16 | 3 |
| AttributeModifier_KirovAfterburnersDummy | 56/0/8 | 1 |
| AttributeModifier_RapidLaunch | 64/8/12 | 2 |
| Modifier_Test_Suppression | 132/12/8 | 1 |

Together with the preceding eight no-import goldens, twelve individual native
modifier roots now have real-game evidence. This is not twelve validated root types.

## Contract and implementation

`AssetDeclarationDocument.HandleAssetReferenceType` normalizes references to
`name\index`, using zero-based positions in `ReferencedInstances`. Keep that
normalization unchanged. `Relo.Tracker.AddReference` also retains the zero-based
value. Only `Tracker.MakeRelocatable` writes **index + 1** into final BIN import
slots; zero represents null. IMP contains source slot offsets plus FFFFFFFF,
not dependency identities or biased offsets. The linker only concatenates chunks.

For IronCurtain, dependencies are Shader, StartFX, EndFX, in that order. BIN
StartFX at 0x0C stores 2; EndFX at 0x10 stores 3. Shader at 0x28 stores a relocated
pointer to offset 148; that payload stores 1. RELO includes the Shader pointer;
IMP includes the pointed payload. The root remains 56 bytes.

Changed implementation:

- `source/BinaryAssetBuilder.Utility/Relo/Tracker.cs`: checked final import bias.
- `source/SageBinaryData/SageBinaryData/AttributeModifier.cs`: optional Shader pointer.
- `source/BinaryAssetBuilder.XmlCompiler/BinaryAssetBuilder/XmlCompiler/Plugin.cs`:
  revision 3, preserving existing KW type hashes and aggregate hash.
- `source/BinaryAssetBuilder.Core/BinaryAssetBuilder/Core/SageXml/DocumentProcessor.cs`:
  revisions 19/20 (VERSION5/other); all plugin intermediate identities include this
  seed, so non-XML processors cannot reuse pre-fix import output either.
- `source/BinaryAssetBuilder.ManifestInspector/AttributeModifierImportSmokeTest.cs`:
  actual document loading/defaults/type injection/reference normalization, detached
  validated marshalling, determinism, ordered reference checks and bounded goldens.

The diagnostic root deliberately remains unregistered (TypeHash=0) in document
processing. Its detached native buffer is compared to observed EP1 metadata; this
does not claim full production dependency resolution or target processor support.

## Official compiler evidence

Local reference DLLs under `Working RA3 Compiler for Reference/tools`:

- Core SHA-256: `B36D5015F97532457D64072034F9F5C629A4C060A2D34C49C4F89FD6E6F79E47`.
- XmlCompiler SHA-256: `D4B78E2DA951DD458357BEE7B358E6BC9689D1987D55867254D68B3F87BBF0E6`.

Core method 0x06000064 normalizes newly added references using Count minus one
(IL_0203 through IL_0209). XmlCompiler's native C++/CLI methods are under
`<Module>`: AddReference 0x0600000F retains that value, while MakeRelocatable
0x0600000D loads the bookmark and explicitly adds one at IL_01E5/IL_01E6 before
endianness conversion and final storage. ShaderRef pointer marshaller 0x06000768
has a pointer-to-pointer signature. The real EP1 golden confirms this contract.

Stock golden manifest:
`D:\TEMP\Red Alert 3 Uprising Source Data\Static Data\data\static.manifest`
SHA-256 `74FACD9056EEE20942BB274A924BEFFB03D7ADBB71B9609E804A39619B2C6081`.
The harness verifies linked EP1 version/aggregate, stream magic/checksums,
native TypeId C5E07887, TypeHash 74425C11, Tokenized=0 and target presence.
BIN reads seek directly to the four small slices; no full binary dump is taken.

## Reproduction and remaining gates

Run the Release/x86 inspector with:

```
modifier-import-self-test "D:\TEMP\Red Alert 3 Uprising Source Data\Static Data\data\static.manifest"
compiler-self-test
layout-self-test
```

The shared serializer regression also checks absent/null slots, index zero,
maximum encodable index, overflow rejection and repeat serialization without
mutating source bookmarks or double-biasing. These tests cover Win32 only.
Full builder and inspector builds, all 61 compiler groups, layout checks and
33 schema enum mappings pass. Existing raw fixtures now expect the corrected
final values; their XML suffixes, weak IDs and pointer offsets remain unchanged.

Next: admit only proven reference kinds into an isolated diagnostic profile,
validate normalized index-to-identity consistency before compilation, and exercise
external target lookup without registering unported FXList/Shader processors.
Production target-table completion, inheritance, custom-data processors,
SDK/WorldBuilder packaging and actual Uprising mod loading remain open.
