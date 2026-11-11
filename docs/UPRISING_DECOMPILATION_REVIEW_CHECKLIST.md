# Uprising retail decompilation review checklist

<!-- Reborn: Tracks Mod Builder representations that conflict with read-only retail binary evidence. -->

This file is a review queue, not an authorization to change compiler layouts. Each item must be independently confirmed against the native asset stream and existing compiler tests before implementation.

## Open

- [ ] **DozerAIUpdateModuleData — verify the inherited AI layout against a retail Dozer record.**
  - The Uprising schema derives from AIUpdateModuleData and adds `RepairHealthPercentPerSecond`, `BoredTime`, and `BoredRange`.
  - The active `0x8D093323` dispatch uses `DozerAIUpdateModuleData`, which embeds and marshals the legacy AIUpdate model. The compiler also has a separate 76-byte retail-qualified EP1 AI base.
  - Retail constructor `007fb5d0` delegates to the AI constructor `007ec990`; serializer `007d2ec0` delegates to `007c1ab0`. These confirm runtime inheritance. Verify the derived asset's inherited fields, the three native tail offsets, and metadata against an original compiled record before changing the model or marshaler.

- [ ] **RadiateFearUpdateModuleData — classify schema and compiler coverage.**
  - The EA schema includes the type through `CnC3Types.xsd`. The checked `AssetTypeGameObject.xsd` does not expose a direct behavior element.
  - Retail factory `006fcc10`, constructor `00748fb0`, update `00794820` and serializer `0072da60` establish a concrete runtime class. Runtime reads confirm radius, interval and victim filter at `+0x24/+0x28/+0x2c` and four boolean bytes at `+0x30..+0x33`.
  - No matching Mod Builder model, marshaler, dispatch or retail contract was found. Establish original asset usage and verify the inferred `0x34` extent and filter metadata before deciding whether to add direct compiler support.

- [ ] **SphereModuleUpdateModuleData — qualify direct compiler dispatch.**
  - EA's GameObject schema permits the direct `SphereModuleUpdate` element. Retail factory `006fcaf0`, constructor `0075f440`, name getter `0075f550`, and update `007c55b0` establish a concrete runtime class.
  - The active Mod Builder model and marshaler are used by derived sphere modules. Searches of compiler source found no direct polymorphic dispatch. The current layout test expects a `0xb0`-byte module record, and the Yuriko shield contract checks the inherited duration in a derived record.
  - Establish the direct module's wire identity and retail usage, then add a qualified dispatch and populated test or record why direct publication remains unsupported.

- [ ] **DestructibleEnvironmentUpdateModuleData — add or classify missing compiler coverage.**
  - EA's Uprising schema declares two required vectors (`LeftOffset`, `RightOffset`), `Enabled` (default `false`), and `Size` (default `LIGHT`).
  - Retail factory `006fcad0`, constructor `007297e0` and update `00729860` establish the `0x40` runtime allocation, module-data vector offsets `+0x0c/+0x18`, and module-data flag `+0x24`. Serializer `007299b0` saves the runtime copy at `+0x3c`.
  - Searches of active Mod Builder source, tests and documentation found no model, marshaler, dispatch entry or retail contract.
  - Confirm the concrete wire ID, `Size` encoding at the inferred `+0x08` offset, and the inferred `0x28` extent from a retail module record before implementing and testing compiler support.

- [ ] **StealUnitUpdateModuleData — replace the legacy AttachUpdate base with the qualified EP1 layout.**
  - Retail evidence: factory `006fca10`, constructor `007c6100`, lifecycle restore `00817c60`, attach-and-steal `007d7390`, ownership refresh `0077dcf0`, target resolver `0077dd90`, and runtime size `0x58`.
  - Runtime state stores `ownershipApplied +0x50` and `stolenOwnerId +0x54`; serializer `0072ef90` transfers both after the inherited AttachUpdate state.
  - Official Uprising adds no derived asset fields, so the module header is exactly the `0x170`-byte EP1 AttachUpdate record.
  - Current model embeds `AttachUpdateModuleData` and its marshaler casts to the legacy type. The same-size `Ep1AttachUpdateModuleData` has a different `AttachBoneName`, `ObjectFilter`, and modifier-list order, so size-only or empty-record tests cannot catch the error. No stock StealUnit contract was found.
  - Required gate: embed and marshal `Ep1AttachUpdateModuleData`, then qualify a populated original StealUnit record, inherited tail offsets, relocations/imports, defaults, and round trip.

- [ ] **BoobyTrapUpdateModuleData — replace the legacy AttachUpdate base with the qualified EP1 layout.**
  - Retail evidence: factory `006fc9f0`, constructor `007c2a60`, entry filter `00778700`, update `00778630`, trigger decision `00778550`, weapon activation `0075d560`, runtime size `0x58`, and saved position-adjusted byte at `+0x54`.
  - The retail derived tail follows the `0x170`-byte EP1 AttachUpdate base: `FireWeaponConditionsOnParent +0x170`, `PreventEnteringSetStatusOnParent +0x190`, `SpecialWeapon +0x1b0`, `PreventEnteringFilter +0x1b4`, `AllowTriggerOnAllies +0x1b8`, complete size `0x1bc`.
  - Current model embeds `AttachUpdateModuleData` and its marshaler casts to that legacy type. Uprising instead requires the same-size `Ep1AttachUpdateModuleData`, whose `AttachBoneName`, `ObjectFilter`, and modifier-list order differs.
  - The derived offsets remain superficially correct because both base records are 368 bytes, so a size assertion cannot expose the inherited-tail corruption. No stock BoobyTrap contract was found.
  - Required gate: embed and marshal `Ep1AttachUpdateModuleData`, then qualify a populated original BoobyTrap record, inherited tail offsets, relocations/imports, defaults, and round trip.

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
  - Retail evidence: factory `006fc930`, constructor `0072c590`, update `007d6750`, scan helper `007d62f0`, and runtime size `0x24`.
  - The update passes an inline record starting at module-data `+0x08`. The helper reads four consecutive dwords, ending with scan width at `+0x14`. Confirm record extent and metadata from an original compiled asset.
  - No managed model, marshaler, polymorphic dispatch entry, retail contract, or layout test was found under `source/`.
  - Required gate: capture an original retail module record and metadata, prove every native offset/relocation and total size, then add model, marshaler, dispatch, defaults, and round-trip/corruption coverage together.

- [ ] **HordeNotifyTargetsOfImminentProbableCrushingUpdateModuleData — verify whether compiled retail assets require compiler support.**
  - Official XML reuses the required four-field `NotifyTargetsOfImminentProbableCrushingMuxDataType` child.
  - Retail factory `006fc950` allocates `0x24` bytes. Constructor `0072acb0` installs an update table whose slot 0 returns the sleep sentinel at `007e2ed0`.
  - No managed model, marshaler, polymorphic dispatch entry, retail contract, or layout test was found under `source/`.
  - Capture an original horde module record and metadata to verify the XML field offsets, record extent, and relocations before implementing support.

- [ ] **LeechTargetingAttachUpdateModuleData — replace the legacy AttachUpdate base with the qualified EP1 layout.**
  - Retail evidence: factory `006fc990`, constructor `007c39b0`, attach callback `00710870`, detach callback `0072bad0`, runtime size `0x50`, and no derived module-data fields.
  - Official Uprising schema extends `AttachUpdateModuleData`; the native header is the unchanged `0x170`-byte EP1 AttachUpdate record.
  - Current model embeds `AttachUpdateModuleData` and its marshaler casts to that legacy type, while the independently stock-qualified `Ep1AttachUpdateModuleData` places `AttachBoneName` before `ObjectFilter` and the modifier list.
  - The current 368-byte size assertion cannot detect this same-size tail-order error; the synthetic empty dispatch test also does not populate the affected fields.
  - Required gate: embed and marshal `Ep1AttachUpdateModuleData`, then verify a populated original LeechTargeting record, metadata offsets, relocations/imports, inherited defaults, and round trip.

- [ ] **ProneUpdate — determine whether retail assets require compiler support.**
  - The official schema defines a single `DamageToFramesRatio` attribute (default zero) and is included by `CnC3Types.xsd`. No direct entry was found in the inspected `AssetTypeGameObject.xsd` or Mod Builder model and dispatch tree.
  - Retail factory `006fcd90` constructs a `0x28`-byte runtime object at `0072d870`. Update `00794800` and transfer `0072d8f0` use the countdown at object `+0x24`.
  - Find a compiled retail asset using the type and trace the damage-entry path before assigning a native offset, record extent, or compiler dispatch entry.

- [ ] **HijackerUpdateModuleData — verify whether compiled retail assets require compiler support.**
  - The Uprising schema defines `AttachToBone` and weak template reference `ParachuteId`. The inspected GameObject schema and Mod Builder source have no direct dispatch entry.
  - Retail factory `006fcdd0` allocates `0x38` bytes. The ejection path at `00793990` passes module-data `+0x10` into the parachute template lookup.
  - Find a retail module record and prove the string layout, reference encoding, and record extent before implementing the compiler entry.

- [ ] **RepairUpdateModuleData — qualify populated SelfAudio imports in the authored repair workflow.**
  - The EP1 schema permits `SelfRepairFromDamageLoop` and `SelfRepairFromRubbleLoop`; the marshaler exposes both, but `RepairUpdateContract.Validate` rejects either populated reference.
  - The current `SovietRefinery` witness covers the 44-byte native record with empty audio references. Retail start helper `00749100` selects damage and rubble audio from module-data `+0x20` and `+0x24` respectively and stores the runtime handle at object `+0x30`.
  - Find a populated original record and verify both import identities, metadata, and round trip before enabling populated values in the authored workflow.

- [ ] **TransportAIUpdateModuleData — use the EP1 AI base in the active transport dispatch.**
  - The Uprising schema derives from `AIUpdateModuleData` with no fields of its own. The current type and marshaler delegate to the legacy AI base.
  - Retail constructor `007fb980` calls the shared AI constructor `007ec990`. Shared update `00806e60` reads module-data byte `+0x49`, which is `FadeOnPortals` in the stock-qualified EP1 layout and part of `SpawnOffsetRadius` in the legacy layout.
  - Capture a retail Transport AI record to verify the full inherited payload and metadata, then route the compiler to the EP1 base and verify populated inherited fields and imports.

- [ ] **WanderAIUpdateModuleData — marshal the derived fields after the EP1 AI base.**
  - The active dispatch embeds and marshals the legacy `AIUpdateModuleData`.
  - Retail update `0080a4c0` consumes `EntryCondition` at `+0x4c` as a 60-byte mask, `WanderDistance` at `+0x88`, `AttackAll` at `+0x8c`, and `Selectable` at `+0x8d`.
  - Qualify a compiled retail record, then verify the EP1 base, the observed tail offsets, metadata and a round trip with populated inherited and derived fields.

- [ ] **EMPUpdateModuleData — correct the EP1 color-pointer offsets.**
  - The retail update `0081ba40` dereferences `StartColor` at `+0x20` and `EndColor` at `+0x24`, reading three floats from each optional pointee.
  - The active model and marshaler insert `DisableFXParticleSystem` and `SparksPerCubicFoot` in those slots and place the color pointers at `+0x28/+0x2c`. Those two inserted fields are absent from the Uprising schema.
  - Qualify a stock record with populated colors, verify native extent and relocations, and route the EP1 dispatch through a layout with the observed color offsets.

- [ ] **WeaponSetUpdateModuleData — align the generic EP1 dispatch and qualify contained slots.**
  - General GameObject dispatch `0x431722A9` selects the legacy model. The dedicated EP1 route writes the 44-byte header but rejects populated `WeaponSlotContained` lists.
  - Retail constructor `007d79b0` reads the four count/pointer pairs starting at `+0x0c`. The contained branch reads `+0x24/+0x28`, iterates with stride `0x1c`, and initializes a distinct runtime slot at `007d7cb3`.
  - Verify generic dispatch against already-qualified EP1 examples. Find a populated original contained-slot record and verify its nested fields, imports, metadata and round trip.

## Resolved

No items yet.
