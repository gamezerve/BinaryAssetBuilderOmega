# BinaryAssetBuilder.ManifestInspector

Read-only inspector for SAGE manifest versions 5, 6, and 7. It can read a
standalone manifest or locate manifest entries inside a BIG4/BIGF archive.
RefPack-compressed manifest entries are decompressed in memory with a bounded
output size; large BIN streams are never loaded.

Examples:

```powershell
dotnet run --project source/BinaryAssetBuilder.ManifestInspector -p:Platform=x86 -- inspect builtmods/worldbuilder.manifest
dotnet run --project source/BinaryAssetBuilder.ManifestInspector -p:Platform=x86 -- verify "D:\Games\RA3 Uprising\Data\WBData.big" --entry "data\worldbuilder.manifest"
dotnet run --project source/BinaryAssetBuilder.ManifestInspector -p:Platform=x86 -- inspect "D:\Games\RA3 Uprising\Data\StaticStream.big" --json
dotnet run --project source/BinaryAssetBuilder.ManifestInspector -p:Platform=x86 -- compare builtmods/worldbuilder.manifest "D:\Games\RA3 Uprising\Data\WBData.big" --right-entry "data\worldbuilder.manifest"
dotnet run --project source/BinaryAssetBuilder.ManifestInspector -p:Platform=x86 -- schema-diff schemas/ra3/xsd schemas/ra3ep1/xsd
dotnet run --project source/BinaryAssetBuilder.ManifestInspector -p:Platform=x86 -- writer-self-test artifacts/ep1-smoke.manifest
dotnet run --project source/BinaryAssetBuilder.ManifestInspector -p:Platform=x86 -- utility-verify "D:\Games\RA3 Uprising\Data\WBData.big" --entry "data\worldbuilder.manifest"
```

Known target fingerprints:

| Target | Manifest | AllTypesHash |
|---|---:|---:|
| Tiberium Wars 1.09 | 5 | `0xEB19D975` |
| Kane's Wrath 1.02 | 5 | `0x12B3E763` |
| Red Alert 3 | 6 | `0x54EEE764` |
| Red Alert 3 Uprising | 7 | `0x5454A8E9` |

The inspector is intentionally independent of the existing compile-time
`VERSION5` switch. Its validated models will be moved into the shared utility
layer after v7 fixtures cover standalone, uncompressed BIG, and RefPack entries.
