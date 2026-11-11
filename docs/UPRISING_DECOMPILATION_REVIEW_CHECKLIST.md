# Uprising retail decompilation review checklist

<!-- Reborn: Tracks Mod Builder representations that conflict with read-only retail binary evidence. -->

This file is a review queue, not an authorization to change compiler layouts. Each item must be independently confirmed against the native asset stream and existing compiler tests before implementation.

## Open

- [ ] **ToggleMountedSpecialAbilityUpdateModuleData — add or explicitly classify missing compiler coverage.**
  - The official Uprising schema declares `OpacityTarget`, `TriggerInstantlyOnCreate`, `CancelDisguiseWhenDismounting`, `MountedTemplate`, and repeated `SynchronizeTimerOnSpecialPower` entries.
  - Retail `ra3ep1_1.1.game` evidence: factory `006fc790`, constructor `007a5db0`, custom xfer `007d2750`, main transition `007fa9b0`, opacity update `007a5e60`, custom update `007be520`, and cleanup `007be4c0`.
  - Retail derived layout after the `0xfc`-byte `SpecialAbilityUpdateModuleData` base:
    - `OpacityTarget` at `+0xfc`.
    - `MountedTemplate` at `+0x100`.
    - `SynchronizeTimerOnSpecialPower` count/data at `+0x104/+0x108`.
    - `TriggerInstantlyOnCreate` and `CancelDisguiseWhenDismounting` at `+0x10c/+0x10d`.
    - Complete aligned retail module-data size `0x110`.
  - Runtime size is `0xc4`, with a six-slot mounted interface at `+0xbc` and saved transition byte at `+0xc0`.
  - No managed model, module marshaler, or `Marshaler.GameObject` polymorphic dispatch entry was found under `source/`.
  - Required gate: prove repeated-reference wire layout against a native asset sample, then add model, marshaler, dispatch, layout assertion, and round-trip tests together.

- [ ] **ToggleHiddenSpecialAbilityUpdateModuleData — add or explicitly classify missing compiler coverage.**
  - The official Uprising schema declares `ToggleHiddenSpecialAbilityUpdateModuleData` with `ShowPalantirTimer` as a `SageBool`.
  - Retail `ra3ep1_1.1.game` evidence: factory `006fc7b0`, constructor `007a5d10`, custom xfer `007d2710`, toggle execution `007fa970`, timed update `007be3d0`, and extended visibility slots beginning at `00774850/007748c0`.
  - Retail derived layout after the `0xfc`-byte `SpecialAbilityUpdateModuleData` base:
    - `ShowPalantirTimer` at `+0xfc`.
    - Complete aligned retail module-data size `0x100`.
  - Runtime class size is `0xc0`, with saved visibility state at `+0xbc`; the primary vtable extends to 54 slots.
  - No managed model, module marshaler, or `Marshaler.GameObject` polymorphic dispatch entry was found under `source/`.
  - Required gate: decide whether this class is intentionally unsupported; if implemented, add the model, marshaler, dispatch, layout assertion, and a visibility-state round-trip test together.

- [ ] **ToggleDeploySpecialAbilityUpdateModuleData — add or explicitly classify missing compiler coverage.**
  - The official Uprising schema declares `ToggleDeploySpecialAbilityUpdateModuleData` with `SoundDeploy` and `SoundUndeploy` `AudioEventInfoRef` fields.
  - Retail `ra3ep1_1.1.game` evidence: factory `006fc7d0`, constructor `007a5c80`, toggle execution `007fa930`, deploy/undeploy primary slots 45/46 at `0095b990` and `0075b4a0`, and update status `00744730`.
  - Retail derived layout after the `0xfc`-byte `SpecialAbilityUpdateModuleData` base:
    - `SoundDeploy` at `+0xfc`.
    - `SoundUndeploy` at `+0x100`.
    - Complete retail module-data size `0x104`.
  - No managed model, module marshaler, or `Marshaler.GameObject` polymorphic dispatch entry was found under `source/`.
  - Required gate: decide whether this class is intentionally unsupported; if implemented, add the model, marshaler, dispatch, layout assertion, and an asset round-trip test together.

- [ ] **SwitchLocomotorsSpecialAbilityUpdateModuleData — restore `LocomotorToSet` and the derived tail offsets.**
  - The official Uprising schema declares `LocomotorToSet` as a `LocomotorSetType` attribute.
  - Retail `ra3ep1_1.1.game` evidence: factory `006fc890`, constructor `007a5970`, activation `007a5a00`, landing-range update `007be220`, locomotor switch `007f9da0`, and cleanup `007f9e80`.
  - Retail native module-data tail:
    - `BusyForDuration` at `+0xfc`.
    - `LandingRange` at `+0x100`.
    - `HordeMembersSpecialPowerTemplate` at `+0x104`.
    - `LocomotorToSet` at `+0x108`.
    - `UseUpgradedLocomotor`, `TriggersLanding`, and `TriggersFlight` at `+0x10c/+0x10d/+0x10e`.
    - Complete retail native module-data size `0x110`.
  - Current managed model omits `LocomotorToSet`, places its three booleans at managed offsets `+0x98/+0x99/+0x9a`, and reports size `0x9c`. With the missing four-byte field restored after `HordeMembersSpecialPowerTemplate`, the managed tail should move the booleans to `+0x9c/+0x9d/+0x9e` and require a `0xa0`-byte aligned record.
  - Files to review:
    - `source/SageBinaryData/SageBinaryData/Modules/SwitchLocomotorsSpecialAbilityUpdate.cs`
    - `source/BinaryAssetBuilder.XmlCompiler/Marshaler.Modules.SwitchLocomotorsSpecialAbilityUpdate.cs`
  - Required gate: confirm the `LocomotorSetType` wire width against a native asset sample, then update the model, marshaler, layout assertion, and round-trip tests together.

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

- [ ] **BeamSpecialAbilityUpdateModuleData — quarantine the stale legacy shadow model.**
  - The legacy model contains obsolete `DamagePerSecond` and omits `TargetObjectStatusAdd`, `WeaponTemplate`, `BeamOptions`, `BeamDuration`, `SweepDistance`, and `DamageInterval`.
  - Active dispatch correctly uses the retail-qualified `Ep1BeamSpecialAbilityUpdateModuleData`, so current EP1 publication is not affected.
  - Required gate: remove, rename, or explicitly quarantine the stale type and its marshaler so future code cannot accidentally select the non-retail layout.

- [ ] **DelayedWeaponFireSpecialAbilityUpdateModuleData — add missing derived compiler coverage.**
  - The official Uprising schema extends `WeaponFireSpecialAbilityUpdateModuleData` with `FireDelay`.
  - Retail evidence: factory `006fc870`, constructor `007bcfa0`, delay setup `00722a40`, delayed entry `007ffa50`, and serializer `007e1550`.
  - Retail derived layout: `FireDelay` at `+0x11c`, complete module-data size `0x120`; runtime object size `0xd8` with the saved absolute fire frame at `+0xd4`.
  - No managed model, marshaler, polymorphic dispatch entry, or layout test was found under `source/`.
  - Required gate: add the derived model, marshal the inherited WeaponFire base plus `FireDelay`, register dispatch, and qualify the `0x120` layout and delayed-frame round trip together.

- [ ] **FlingPassengerSpecialAbilityUpdateModuleData — add missing compiler coverage and qualify its module tail.**
  - Official fields are inline `FlingPassengerVelocity` (`Vector3`) and `FlingPassengerLandingWarhead` (`WeaponRef`).
  - Retail evidence: factory `006fc8d0`, constructor `007a4d60`, serializer `007d20c0`, and passenger-release action `007f7830`; runtime size is the base-only `0xbc`.
  - Schema-derived native tail is velocity at `+0xfc`, landing warhead at `+0x108`, complete module-data size `0x10c`; asset-byte qualification is still required because the action delegates field consumption.
  - No managed model, marshaler, polymorphic dispatch entry, or layout test was found under `source/`.
  - Required gate: prove the two offsets from an original asset record, then add model, marshaler, dispatch, size assertion, and round-trip coverage together.

- [ ] **NotifyTargetsOfImminentProbableCrushingUpdateModuleData — add missing compiler coverage and qualify the native Notify record.**
  - Official XML fields are `TimeBetweenUpdatesMS`, `ScanAheadTimeMS`, `ScanHeight`, and `ScanWidth` inside the required `Notify` child.
  - Retail evidence: factory `006fc930`, constructor `0072c590`, update `00748180`, runtime size `0x24`, and module-data accesses spanning `+0x08..+0x18`.
  - The executable range contains a native helper/reference value whose relationship to the four XML fields is not proven by schema order alone.
  - No managed model, marshaler, polymorphic dispatch entry, retail contract, or layout test was found under `source/`.
  - Required gate: capture an original retail module record and metadata, prove every native offset/relocation and total size, then add model, marshaler, dispatch, defaults, and round-trip/corruption coverage together.

- [ ] **HordeNotifyTargetsOfImminentProbableCrushingUpdateModuleData — add the missing horde variant without assuming sibling ABI identity.**
  - Official XML reuses the required four-field `NotifyTargetsOfImminentProbableCrushingMuxDataType` child.
  - Retail evidence: factory `006fc950`, constructor `0072acb0`, horde update `007795e0`, runtime size `0x24`, and native module-data accesses spanning `+0x08..+0x1c`.
  - The executable consumes a horde/prediction handle in the same native range, so simply aliasing the non-horde header would be unproven.
  - No managed model, marshaler, polymorphic dispatch entry, retail contract, or layout test was found under `source/`.
  - Required gate: capture an original horde module record and metadata, prove the handle and four XML fields plus total size/relocations, then add model, marshaler, dispatch, defaults, and round-trip/corruption coverage together.

- [ ] **LeechTargetingAttachUpdateModuleData — replace the legacy AttachUpdate base with the qualified EP1 layout.**
  - Retail evidence: factory `006fc990`, constructor `007c39b0`, attach callback `00710870`, detach callback `0072bad0`, runtime size `0x50`, and no derived module-data fields.
  - Official Uprising schema extends `AttachUpdateModuleData`; the native header is the unchanged `0x170`-byte EP1 AttachUpdate record.
  - Current model embeds `AttachUpdateModuleData` and its marshaler casts to that legacy type, while the independently stock-qualified `Ep1AttachUpdateModuleData` places `AttachBoneName` before `ObjectFilter` and the modifier list.
  - The current 368-byte size assertion cannot detect this same-size tail-order error; the synthetic empty dispatch test also does not populate the affected fields.
  - Required gate: embed and marshal `Ep1AttachUpdateModuleData`, then verify a populated original LeechTargeting record, metadata offsets, relocations/imports, inherited defaults, and round trip.

## Resolved

No items yet.
