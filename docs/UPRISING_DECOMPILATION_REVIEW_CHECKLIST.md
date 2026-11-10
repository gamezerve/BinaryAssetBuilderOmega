# Uprising retail decompilation review checklist

<!-- Reborn: Tracks Mod Builder representations that conflict with read-only retail binary evidence. -->

This file is a review queue, not an authorization to change compiler layouts. Each item must be independently confirmed against the native asset stream and existing compiler tests before implementation.

## Open

- [ ] **ReplaceSelfUpgradeModuleData — restore the retail `Options` bit mask and offsets.**
  - Retail `ra3ep1_1.1.game` evidence: `ReplaceSelfUpgrade` constructor `0074bd20`, eligibility `00796330`, application `007fde20`, and custom xfer `007308e0`.
  - Retail derived layout after the `0x24`-byte `UpgradeModuleData` base:
    - `NewObjectUnpackTime` at `+0x24`.
    - `ReplaceSelfUpgradeOptionsBitFlags Options` at `+0x28`.
    - `ReplacementTemplate` count/data at `+0x2c/+0x30`.
    - Complete retail module-data size `0x34`.
  - Current managed model places `ReplacementTemplate` immediately after time and exposes only `DisabledDuringUnpack` and `CheckBuildAssistant` booleans. It therefore omits `COPY_TARGET_OBJECT`, `IGNORE_TERRAIN_RESTRICTIONS`, and `COPY_UPGRADES`, and shifts the retail list offsets.
  - Files to review:
    - `source/SageBinaryData/SageBinaryData/Modules/ReplaceSelfUpgrade.cs`
    - `source/BinaryAssetBuilder.XmlCompiler/Marshaler.Modules.ReplaceSelfUpgrade.cs`
  - Required gate: prove the bit-mask wire representation and update model/marshaler/tests without assuming the current boolean representation is stream-compatible.

- [ ] **SubStructureBuildUpgradeModuleData — add or explicitly classify missing compiler coverage.**
  - The official Uprising schema declares `SubStructureBuildUpgradeModuleData`, repeated `AvailableSubStructure` pairs, and repeated `StructureOffset` entries.
  - Retail `ra3ep1_1.1.game` evidence: constructor `0074c080`, application `007d8aa0`, custom xfer `007d8b70`, and eligibility `007967a0`.
  - Retail derived layout after the `0x24`-byte `UpgradeModuleData` base:
    - `AvailableSubStructure` count/data at `+0x24/+0x28`, with 8-byte trigger/template pairs.
    - `StructureOffset` count/data at `+0x2c/+0x30`.
    - Complete retail module-data header size `0x34`.
  - No `SubStructureBuildUpgradeModuleData` model, marshaler, or polymorphic dispatch reference was found under `source/`.
  - Required gate: decide whether this is intended unsupported coverage or an omission; if implemented, prove `OffsetInfo` element size/field order against native asset bytes before adding the model and marshaler.

## Resolved

No items yet.
