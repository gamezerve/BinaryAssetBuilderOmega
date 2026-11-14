# Uprising retail decompilation review checklist

<!-- Reborn: Tracks Mod Builder representations that conflict with read-only retail binary evidence. -->

This file is a review queue, not an authorization to change compiler layouts. Each item must be independently confirmed against the native asset stream and existing compiler tests before implementation.

## Open

- [ ] **EP1 ProjectileUpdate BezierSplineProjectile — qualify the schema-supported movement mode.**
  - The EA and Mod Builder EP1 ProjectileUpdate.xsd files pair BezierSplineProjectile with Movement/BezierSpline and one or more ProjectilePathRef entries. The active Marshaler.ProjectileUpdate.cs rejects the selector. The EP1 metadata tokenizer and ManifestInspector movement dispatch recognize only the falling and straight-line movement records.
  - Retail factory 00748ec0 selects discriminator 3 and calls 00742ac0. The child constructed by 00742860 reads the Movement record at module data +0x18. BezierLaunchState entry 0075a440 uses the count at Movement +0x14 and the path-reference array at +0x18.
  - Retail dispatcher 004b7750 allocates 0x1c bytes for TypeId 0xb38f4984. Reader 0045d160 confirms Velocity +0x04, Range +0x08, Gravity +0x0c (default 1), FlightPathAdjustMaxDistancePerFrame +0x10 (default 0), Path count +0x14 and resolved array +0x18. Helper 0043fb10 invokes the reference resolver once per row. Verify serialized path imports and relocation metadata against an independently produced native asset. Exercise the named XML route and metadata readers with a multi-path compile-and-read fixture.

- [ ] **EP1 ProjectileUpdate BalloonProjectile — qualify the schema-supported movement mode.**
  - The official EA and Mod Builder EP1 ProjectileUpdate.xsd files pair BalloonProjectile with Movement/BalloonMoveToGround. BalloonProjectileMoveToGroundData adds no fields to ProjectileMoveBase.
  - Retail factory 00748ec0 selects discriminator 2 and calls 007427f0. The constructor reads Movement from module data +0x18, stores it at state +0x24 and installs BalloonMoveToGroundState vtable 00c4b000. Tick 00805e80 follows the owner AI goal and reads no balloon-specific movement fields.
  - The active Marshaler.ProjectileUpdate.cs explicitly rejects BalloonProjectile. The EP1 metadata tokenizer and ManifestInspector movement dispatch currently recognize only the BombProjectile and StraightLineProjectile movement classes.
  - Retail dispatcher 004b7750 allocates 0x04 bytes for TypeId 0x1a5b5a69. Reader 00432980 delegates to the ProjectileMoveBase TypeId reader. Confirm emitted relocation metadata against an independently produced native asset. Then exercise the named BalloonMoveToGround XML route and the matching metadata paths with a compile-and-read fixture.

- [ ] **EP1 ProductionUpdate QuantityModifier — qualify populated lists.**
  - EA permits up to two rows with a GameObjectWeakRef Name and signed Count. The active EP1 marshaler rejects populated QuantityModifier lists.
  - Retail 00482d80 passes module-data +0x3c/+0x40 to 00449340, which allocates 0x08 bytes per row. Reader 0042be00 copies the weak template ID at +0x00 and Count at +0x04. Queue insertion 0075eb00 resolves the ID and copies Count from the first matching template row into entry +0x28.
  - Compare populated EP1 list metadata and weak-reference encoding with an independently produced native reference before enabling the writer. Verify that the named GameObject route preserves row order.

- [ ] **EP1 ProductionUpdate ProductionModifier — qualify populated lists.**
  - EA's schema permits up to two rows. The active EP1 marshaler rejects any populated list.
  - Retail type `0x6ec75439` allocates a `0x54`-byte module record. Loader `00482d80` sends the list at `+0x44/+0x48` to `00464670`, which allocates `0x84` bytes per row. Reader `00457f80` resolves `RequiredUpgrade` at `+0x00`, defaults the cost and time floats at `+0x04/+0x08` to 1.0, and loads the embedded `ModifierFilter` at `+0x0c`.
  - Compare populated native stream metadata, strong upgrade imports and the nested filter against a stock or independently produced reference record before enabling the writer. The runtime row stride is confirmed independently of the serialized representation.

- [ ] **HordeMemberCollideModuleData — establish the asset route and classify compiler coverage.**
  - EA EP1 includes the derived type without extra fields. GameObject exposes the base Collide element. The retail factory 006fdb70, constructor 0071e300 and callback 0078b7c0 identify a concrete class whose collision handler resolves horde and containment state.
  - No dedicated Mod Builder model, marshaler or dispatch was found. Check whether stock assets use a derived type under Collide or whether the engine attaches the module programmatically. Verify the wire identity and base-sized record against a stock asset before adding a compiler path.

- [ ] **SquishCollideModuleData — verify the named EP1 XML route selects the retail layout.**
  - Retail factory 006fdb50, constructor 0073ee40 and collision callback 0080fc10 confirm a float CrushAnimTime at +0x08, UseVehicleStyleCrush at +0x0c and AllowAllyCrush at +0x0d. EA EP1 schema exposes the named SquishCollide element.
  - The reviewed TypeId 0xB5EB41ED route explicitly selects the matching 0x10-byte Ep1SquishCollideModuleData. The ordinary GameObject switch still selects the legacy SquishCollideModuleData for the same ID. That legacy model writes CrushKillDelay at +0x0c and UseDirectionCheck at +0x10.
  - Compile a named EP1 SquishCollide without authored TypeId, set the two booleans independently, and compare the emitted record and metadata with a stock asset. If that path reaches the legacy switch, route it to the EP1 model and retain a separate legacy-target test.

- [ ] **ReflectDamageModuleData, EvacuateDamageModuleData, HordeTransportContainDamageModuleData and CallHelpOnDamageModuleData — establish the asset route and classify compiler coverage.**
  - The official EP1 schemas declare these types as DamageModuleData derivatives. The GameObject schema exposes a base Damage element. Searches of the checked Mod Builder source found no matching concrete model, marshaler or dispatch entry for these types.
  - ReflectDamage factory 006fda50, constructor 00720950 and callback 00811600 establish a concrete runtime class. The callback tests the first 32 bits of the mask at data +0x08 and reads the multiplier and minimum at +0x10/+0x14. The bytes at +0x0c remain unverified.
  - EvacuateDamage factory 006fda70, constructor 00759e50 and callback 0078e500 establish a second runtime class. Runtime reads place the weak weapon reference, damage type, panic threshold and frame window at +0x08/+0x0c/+0x10/+0x14.
  - HordeTransportContainDamageModuleData also derives from DamageModuleData and adds no fields. Factory 006fda90, constructor 00720890 and damage callback 00811280 establish the runtime class. The callback reads owner and event state, and no matching concrete Mod Builder model or dispatch was found.
  - CallHelpOnDamageModuleData is also declared in the EA EP1 schema and has a retail factory at 006fdab0. Callback 007708a0 reads the low 32 bits of DamageTypes at +0x08, CallRadius +0x10, CallDelay +0x14, ValidObjs +0x18, and MoveToAttacker +0x1c. No concrete model, marshaler or dispatch was found. Verify the full mask and the ObjectFilter relocation against a stock record.
  - Check which stock asset records instantiate each type, how the base Damage element resolves the derived data, and the native record extent, mask encoding and weak-reference metadata before adding a compiler model.


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

- [ ] **WeaponScatterEffectUpdate — qualify ScatterAmount values outside the runtime byte range.**
  - The EA schema declares `ScatterAmount` as `xs:int`; the active marshaler preserves the complete signed value at module-data `+0x0c`.
  - Retail `0081cfda` reads the low byte from `+0x0c` and the scatter handler uses it as an unsigned queue limit. At this consumer, `-1` behaves as 255 and 256 behaves as zero.
  - Keep the stock record width. Review authored-value validation or diagnostics for the range 0–255 and qualify boundary cases against the retail consumer before changing compiler behavior.

- [ ] **TemporarilyDefectUpdate — qualify the Uprising compiler route.**
  - Retail factory `006fd5d0` constructs the module. Start helper `007f13a0` adds module-data `+0x08` to the current frame when the caller supplies no deadline, matching `DefectionFrameCount` in EA's Uprising schema.
  - The compiler route for `0x4d76910c` is under `KANESWRATH`, with a comment that only discusses Tiberium Wars. Both current XmlCompiler configurations define the symbol, so this source inspection does not establish a default-build dispatch failure.
  - Locate a compiled Uprising witness and verify the type identity, duration encoding, native size and metadata. Update the dispatch comment and qualify the route against the EP1 schema after the stock check.

- [ ] **EvacuateGarrisonSpecialPowerModuleData — qualify GameObject compiler support.**
  - EA's Uprising schema extends SpecialAbilityUpdateModuleData without derived fields. The inspected EP1 GameObject schema has no element for the module.
  - Mod Builder has no concrete model or dispatch entry for module-data type ID `0xdb0bac4c`. Its generic unknown-module route allocates the generic behavior base and omits the concrete type ID.
  - Retail factory `006fd6d0` constructs a `0xbc`-byte runtime object. Transfer `007d20c0` delegates to the base, and callback `007e1590` resolves inherited target ID `+0x4c` before submitting order kind `0x3d`.
  - Find a compiled original or establish an authored GameObject use case, then verify inherited field encoding, type ID, imports and round trip before adding a dispatch entry.

- [ ] **GiveOrRestoreUpgradeSpecialPowerModuleData — qualify the EP1 tail without the legacy WeaponFlags field.**
  - The TW and KW schemas declare WeaponFlags. EA's EP1 schema only declares CommandButton and UpgradeToGive beyond SpecialAbilityUpdateModuleData.
  - The shared Mod Builder model and active marshaler append and write a four-word WeaponFlags mask. Retail activation and the position eligibility callback consume UpgradeToGive at module-data `+0x104`. The traced paths do not read the mask.
  - Verify a compiled EP1 record's extent, metadata, command-button storage and upgrade import before choosing a separate EP1 model or changing the shared dispatch.

- [ ] **PillageModuleData — qualify GameObject compiler support.**
  - EA's EP1 schema declares PillageAmount, NumDamageEventsPerPillage and ObjectFilter. The inspected GameObject schema has no corresponding element and Mod Builder has no model or dispatch for type ID `0x6ce208f7`.
  - Retail factory `006fd730` creates a `0x1c`-byte runtime object. The damage callback `0071ab80` reads the amount at module-data `+0x08`, threshold at `+0x0c` and filter at `+0x10`. It uses transaction category `0x1c` (PILLAGE).
  - Find a compiled original or establish an authored GameObject use case, then qualify filter representation, metadata, type ID and round trip before introducing a compiler route.

- [ ] **ActivateModuleSpecialAbilityUpdateModuleData — add qualification for the EP1 type identity.**
  - The EP1 GameObject schema declares the type with ID `0xcd33bcee`. Mod Builder has a shared list item and a dispatch for the differently named ActivateModuleSpecialPowerModuleData (`0xa63dc46b`), but no dispatch for the EP1 ID.
  - Retail dispatcher `00771420` reads the list count at `+0xfc` and pointer at `+0x100`, with a `0x0c` stride. Entries use ModuleId at `+0x00` and TriggerType at `+0x04`. The shared item metadata places TriggerAtTargetPosition at `+0x08`; this dispatcher does not read it.
  - Verify a compiled or authored EP1 record with a populated list, its concrete type ID, metadata, linked module IDs, and round trip.

- [ ] **AttributeModifierNavalModuleData — qualify the missing EP1 dispatch.**
  - EA's Uprising GameObject schema exposes AttributeModifierNaval. The module type extends CreateModuleData with one AttributeModifierRef. Mod Builder has no model or dispatch case for type ID `0xa34ced5c`.
  - Retail create callback `00755f00` passes module-data `+0x08` to the attribute-modifier helper after the surface test.
  - Verify a compiled or authored EP1 record with a populated reference, including the imported ID, metadata, native extent and round trip.

- [ ] **PreorderCreateModuleData — determine whether EP1 assets require a compiler route.**
  - EA's EP1 schema declares an empty CreateModuleData extension. The inspected GameObject schema has no element for it, and Mod Builder has no concrete model or dispatch for type ID `0x2a36dc1f`.
  - Retail factory `006fd8d0` allocates the `0x18`-byte runtime object. The create callback updates the PREORDER model condition. Transfer delegates to the inherited creation-state path.
  - Locate a compiled EP1 record or an authored use case, then qualify the concrete type identity, inherited extent, metadata and round trip.

- [ ] **SpecialPowerCreateModuleData — establish whether EP1 assets require compiler support.**
  - EA's Uprising schema defines an empty CreateModuleData extension. The inspected EP1 GameObject schema has no corresponding element. Mod Builder has no concrete model or dispatch for type ID `0x6a8c7338`.
  - Retail factory `006fd950` allocates the module. Build-complete callback `0070cf30` notifies each attached special-power interface, consistent with EA's Zero Hour implementation.
  - Find a compiled EP1 GameObject record or an authored use case before qualifying the concrete type ID, inherited extent, metadata and round trip.

- [ ] **InactiveBodyModuleData — qualify dispatch for a named Body/InactiveBody child.**
  - EA's Uprising GameObject schema places InactiveBody under Body. Its concrete type adds no asset fields to BodyModuleData. Retail factory `006fdd10` allocates a separate `0x18`-byte runtime object.
  - GameObject.Body uses the exact `BodyModuleData**` overload. The `0x29234e6e` case exists in `BehaviorModuleData**`; the body switch currently leaves the destination untouched.
  - Verify an original or authored Body/InactiveBody record through the GameObject compiler, including the body pointer, concrete type ID, inherited fields and round trip.

- [ ] **ShieldBodyModuleData — qualify the EP1 body layout and compiler route.**
  - The generic Body dispatch allocates concrete type `0xd3565857` with the legacy ActiveBody base. The declared x86 layout places ShieldAmount at `+0x6c`; retail recharge helper `00758500` reads it at `+0x5c`. The explicit EP1 body allocator currently excludes ShieldBody.
  - Retail damage, armor and status paths also confirm the shield tail offsets `+0x50` through `+0x68`, including ShieldArmor at `+0x60`, recharge idle time at `+0x64`, and the active model condition at `+0x68`.
  - Verify a populated EP1 GameObject Body/ShieldBody record, preserving its concrete type, inherited ActiveBody semantics, all shield fields, imports, relocations and round trip.

- [ ] **CreateAndEnterObjectSpecialPowerModuleData — qualify the derived EP1 OCL layout.**
  - Retail action `007ff8d0` reads the optional FXOffset vector pointer at `+0x20c`, immediately after the 524-byte EP1 OCL base.
  - Mod Builder dispatches `0x38c496d8` through CreateAndEnterObjectSpecialPowerModuleData and the legacy OCL marshaler. That x86 record places FXOffset at `+0x208` and inherits the legacy location enum and UpgradeOCL item layout. The separate base OCL dispatch uses the EP1 record.
  - Qualify the derived route with a populated FXOffset, CreateLocationOffset and UpgradeOCL, checking concrete type ID, relocations, metadata and round trip.

- [ ] **ObjectStatusSpecialAbilityUpdateModuleData — qualify the missing EP1 compiler dispatch.**
  - EA's Uprising GameObject schema exposes this SpecialAbilityUpdate extension. Retail registration identifies type `0x079b6ead`; the reviewed compiler has no concrete model or dispatch for it.
  - Retail constructor `007a94c0` sets Status1. Callback `007fced0` alternates two 32-byte inline ObjectStatusBitFlags masks at `+0xfc` and `+0x11c`.
  - Qualify an original or authored GameObject record with distinct nonempty masks. Verify concrete ID, native field offsets, inherited fields, metadata and round trip.

- [ ] **WeaponModeSpecialPowerUpdateModuleData — establish whether EP1 assets require compiler support.**
  - EA's Uprising type schema declares Duration, AttributeModifier and LockWeaponSlot. Retail registers type `0x3041e295`. The inspected GameObject schema has no matching element and the compiler has no concrete model or dispatch.
  - Retail activation `0075fab0` reads Duration at `+0x18` and AttributeModifier at `+0x1c`. Expiry removes the modifier. The LockWeaponSlot access remains unverified.
  - Locate a populated original or an authored use case before qualifying the inherited layout, LockWeaponSlot, concrete type, metadata and round trip.

- [ ] **SpecialPowerUpdateModuleData — qualify the EP1 base mask and StartsPaused offsets.**
  - EA's Uprising schema adds DisabledTypesToIgnore with default HELD. Retail getter `00722920` reads the 32-bit mask at `+0x0c`; the owner gate at `007442c0` consumes it. Constructor `007441a0` reads StartsPaused at `+0x14`.
  - Mod Builder dispatch `0xb9b2a9b3` uses a model that puts InitiateSound at `+0x0c` and StartsPaused at `+0x10` on x86. Its marshaler omits DisabledTypesToIgnore.
  - Qualify the base and applicable derived types with a nonempty mask, StartsPaused and InitiateSound. Verify native offsets, defaults, imports, relocations, metadata and round trip.

- [ ] **DeflectSpecialPowerModuleData — determine whether EP1 assets require a compiler route.**
  - EA's Uprising type schema declares a fieldless SpecialPowerUpdateModuleData extension. Retail registers type `0x7fa7defc` and allocates its runtime object through `006fdf70`. The GameObject schema and compiler have no matching route.
  - Activation `00722990` schedules the pending transition. Update `00742ec0` manages the active bit, and `00722930` dispatches through incoming object modules while active.
  - Locate an original or authored asset before qualifying the type ID, inherited EP1 mask and StartsPaused layout, metadata and round trip.

- [ ] **DamageFieldUpdateModuleData — use the EP1 FireWeaponUpdate base.**
  - Retail constructor `007d5de0` calls `007c3590`, which reads the status pointer at `+0x18` and walks the list at `+0x10/+0x14` with a 16-byte stride. Shared update `00778f30` reads FireInterval at `+0x08` and AliveOnly at `+0x1c`. The derived action `007eda00` reads Radius at `+0x20` and RequiredUpgrade at `+0x24`.
  - The generic `0xf3b802d5` route embeds the 24-byte legacy FireWeaponUpdate model and invokes its legacy marshaler. On x86 that places Radius at `+0x18` and RequiredUpgrade at `+0x1c`. The standalone EP1 FireWeaponUpdate route uses the verified 32-byte base.
  - Qualify a compiled DamageFieldUpdate with a nonzero FireInterval and Radius, a populated nugget, status validation, RequiredUpgrade and ObjectFilter. Verify concrete type ID, imports, relocations, metadata and round trip.

- [ ] **FireWeaponWhenDeadBehaviorModuleData — qualify the generic EP1 dispatch.**
  - Retail die callback `007dd7d0` reads FireAtGround at `+0x8f` and UseMyKillerAsWeaponSource at `+0x90`. Registration uses `0x90ceb49c`. The EP1 schema declares both flags.
  - The reviewed EP1 selection route uses the 148-byte Ep1FireWeaponWhenDeadBehaviorModuleData and writes both bytes. The generic GameObject switch still maps the same ID to the 144-byte legacy model, which has neither field.
  - Exercise the generic EP1 authoring path with both flags set and compare its data, imports, metadata and round trip to the reviewed selection route.

- [ ] **DamageFilteredCreateObjectDieModuleData — qualify the EP1 compiler route.**
  - The EP1 type schema declares this DieModuleData extension and retail registration uses `0x76ab5e78`. The current GameObject schema and compiler sources contain no concrete route for the type.
  - Die callback `0078e970` reads CreationList at `+0x34`, the instant subtype at `+0x38` and the post-trigger duration at `+0x40`. Damage callback `00770cd0` uses the duration subtype at `+0x3c` and records the frame and source player.
  - Locate a stock or authored GameObject witness before adding the route. Verify the inherited DieModuleData prefix, the creation-list import, both subtype values, the duration, metadata and round trip.

- [ ] **CreateObjectDieModuleData — qualify the generic EP1 dispatch.**
  - Retail die callback `007a3790` reads CreationList at `+0x34`, required count/pointer at `+0x38/+0x3c` and forbidden count/pointer at `+0x40/+0x44`.
  - The reviewed EP1 route uses the 72-byte Ep1CreateObjectDieModuleData. The generic GameObject route for `0x3b68b768` selects the 80-byte legacy model. Its extra DebrisPortionOfSelf length/pointer pair at `+0x38/+0x3c` shifts both lists eight bytes later.
  - Exercise the generic EP1 authoring route with both upgrade lists populated and compare native offsets, imports, metadata and round trip to the reviewed EP1 route.

- [ ] **HordeAIUpdateModuleData — route the inherited fields through the EP1 AI model.**
  - Retail registration uses `0x55d77703`. Constructor `007fb6d0` calls shared AI constructor `007ec990`. Update `0080a280` tail-jumps to `00806e60`, which reads module-data byte `+0x49` at `0080707f`.
  - The stock-qualified EP1 AI model places FadeOnPortals at `+0x49`. The active HordeAIUpdate marshaler delegates to the legacy AIUpdate model, where that byte falls inside SpawnOffsetRadius. The EP1 schema declares no HordeAIUpdate-specific fields.
  - Find a retail HordeAIUpdate asset and qualify inherited values, state-machine and locomotor fields, imports, metadata and round trip through the EP1 layout.

- [ ] **SupplyTruckAIUpdateModuleData — use the EP1 AI base in the generic compiler route.**
  - Retail registers type `0x9ab8094d`. `0070ea70` checks MaxBoxes at `+0x4c`, `00725d10` reads the center and warehouse delays at `+0x50/+0x54`, and `0070ead0` reads CanPathThroughUnitsWhileDocking at `+0x6f`. The last getter is SupplyTruckAIInterface slot 33 and is consumed by AIDockState entry.
  - `Marshaler.ModulesAIModuleData.cs` selects the legacy SupplyTruckAIUpdateModuleData and its marshaler delegates to AIUpdateModuleData. An x86 reflection probe of the deployed alpha compiler measured legacy AIUpdateModuleData at `0x5c` and Ep1AIUpdateModuleData at `0x4c`. The active model places those fields at `+0x5c`, `+0x60/+0x64` and `+0x7f`, respectively.
  - Qualify the EP1 route with distinct nonzero delays, MaxBoxes, both docking and harvesting flags, and a populated inherited AI record. Verify native bytes, imports, relocations, metadata and round trip. TransportHelicopterAIUpdate embeds the same legacy model, so qualify that route against its own retail evidence before reusing the fix.

- [ ] **EP1 AI StateMachine selector — verify authoring support for OreRefineryStateMachine.**
  - The EP1 XSD and Ep1AIStateMachineType expose OreRefineryStateMachine at ordinal `0x0b`. The generic Ep1AIUpdate marshaler writes that scalar at native `+0x3c`.
  - Retail base AI factory `0070f2d0` reads `+0x3c`. Jump-table entry `0070f5b8` goes directly to the null-return epilogue at `0070f584`. The common initializer `0070f620` uses the returned pointer immediately. This path needs an authoring guard or a qualified runtime witness before treating ordinal `0x0b` as usable with the generic AI module.
  - TransportHelicopterAIStateMachine at ordinal `0x0d` does construct through `008337e0`. This is a StateMachine value on the common AI module. The standalone TransportHelicopterAIUpdate compiler type `0xa9936c0d` had no matching name or ID in the inspected retail image and remains unqualified.

- [ ] **FormationAssistantTemplate — check equal cell counts across rows before emitting the asset.**
  - The EP1 XSD permits each Row to contain an independent number of Cell elements. A probe against the deployed x86 SDK passed both 1,1 and 1,2 cell counts through the admitted schema and StockAssetRootCompiler.Compile. The emitted instance buffers preserved those exact counts at the 8-byte row stride.
  - Retail create callback `00584ea0` compares each row count with the first at `00584ec3`. A mismatch branches to the null return at `00584f72` before runtime allocation. Replacement callback `00584f80` performs the same check at `00584fa0` and exits before touching the current wrapper.
  - Add an authored diagnostic for unequal widths and qualify it with the rectangular and uneven fixtures. Keep each cell's variable Definition list independent. The retail rule compares the number of cells per row.

- [ ] **UIMouseTacticalLookAt — diagnose ViewBookmark lists longer than eight rows.**
  - The EP1 LookAt schema declares `ViewBookmark` with `maxOccurs="unbounded"`. The x86 source-tree Mod Builder compiler accepts eight and nine rows, preserving counts 8 and 9 at instance `+0x34` in 124-byte and 132-byte buffers.
  - Retail initializer `00a5fdf0` reads the serialized count at `00a60017`, substitutes the constant 8 at `00c8e00c` when it is exceeded, and uses the capped value as the loop bound at `00a600a3`. The ninth row is emitted but receives no runtime binding.
  - Add an EP1 authoring diagnostic for more than eight rows. Re-run the 8-row and 9-row fixtures from Reborn’s `Reconstruction/UIMouseTacticalLookAt/LookAtAssetProbe.cs` against the production authoring path.

- [ ] **EncloseBehaviorModuleData layout index — verify x86 string and inline filter sizes.**
  - Retail callbacks `00719110` and `007ce1e0` read ScaledBoneName pointer at `+0x14` and EnclosureRadius at `+0x18`. `007b3d10` passes the inline filter at `+0x1c`.
  - The Mod Builder declaration uses x86 `Relo.String<sbyte>` (length and pointer) and the manifest inspector records the stock contract as 148 bytes. The generated Reborn layout index assigns four bytes to AnsiString and reports a 60-byte record with EnclosureRadius at `+0x14` and Filter at `+0x18`.
  - Revalidate the index against the x86 compiler layout. Cover AnsiString and the inline ObjectFilter and check the generated offsets against the stock contract before using the index to apply Ghidra types.
  - The same generated-index issue affects Ep1SpecialPowerTemplate. It reports TargetType at `+0x0c` and a zero-byte Flags field at `+0x14`. Retail `00806470` and `007fff70` read TargetType at `+0x14` and the flags dword at `+0x1c`. Verify both 8-byte AnsiString members and the fixed uint[1] flag buffer when requalifying the index.
  - WeaponTemplate is a second measured index regression. The deployed alpha and Release ManifestInspector x86 types both measure 0x154, with ClipSize +0x78, RestrictedHeightRange +0xb0, IncompatibleAttributeModifier +0x124, ScaleAttackRangeByAmmoRemaining +0x130 and UseCenterForRangeCheck +0x14f. Retail routines 0074d0c0, 00731880 and 007ad370 read those offsets. The generated index reports 0x120 and +0x60, +0x98, +0xf0, +0xfc and +0x11b. Use the measured x86 offsets as regression fixtures for the index.
  - Ep1Shape is another measured index case. Retail loader 00b10dd0 advances 0x30 bytes in the native shape array and 0x28 in the converted runtime array. The existing EP1 Mod Builder geometry smoke test already checks the native 48-byte type. The generated index currently reports 0x28 for the native Ep1Shape and shifts MajorRadius +0x10, Height +0x20 and Active +0x2c to +0x0c, +0x18 and +0x24. Include Name and Other as eight-byte x86 strings in the index fixture.
  - ProjectedBuildabilityInfo isolates the fixed-buffer issue. The deployed alpha x86 type and retail loop 00525ac0 both use 0x74 bytes. x86 places BuildPlacementTypes, StatusToReject and ModelConditionsToReject at +0x08, +0x0c and +0x2c, followed by height limit +0x68, filter +0x6c and PrimaryBuidability +0x70. The index assigns all three bitflag structs zero bytes and reports 0x14. Add this record as a fixed-buffer size and offset fixture.

- [ ] **FXParticleSysBoneNugget layout index — account for the two-word x86 Bone string.**
  - Type ID `0xba0b1771` selects factory `00551700` through `0056bd4f`. Binding reader `008b02c0` and the inline path at `008cd703` read the Bone pointer at `+0x30`, then TriggerType, HoldBetweenStateID, ActionType and Particle at `+0x34/+0x38/+0x3c/+0x40`. RequireFrequentUpdate is a byte at `+0x50`.
  - Reborn's generated EP1 Mod Builder layout index treats AnsiString as four bytes and reports Particle at `+0x3c` and RequireFrequentUpdate at `+0x4c`. The actual declaration uses `Relo.String<sbyte>`, whose x86 layout is a length and pointer. Add this record as a regression fixture for the index. For FXListAtBonePosFXNugget, retail `005653c0` reads FX at `+0x2c` and the second Bone word at `+0x34`; the index ends that record at `0x34`.
  - Verify emitted x86 instance data and relocations with a nonempty bone name, distinct trigger/state/action values, a referenced particle, nonzero Offset and RequireFrequentUpdate enabled. Qualify Offset against a retail consumer separately. The current finding concerns the generated index.

- [ ] **EP1 AnimationMode comments — correct LOOP_PINGPONG and PLAY_TO_FRAME descriptions.**
  - `schemas/ra3ep1/xsd/Includes/AnimationMode.xsd` associates the play-to-frame comment with ordinal 5 LOOP_PINGPONG and the reversal comment with ordinal 6 PLAY_TO_FRAME.
  - Retail draw update dispatch at `008d92e7` routes ordinal 5 to `008d9377`, which reverses direction at boundaries. Ordinal 6 skips automatic advancement in that loop. Single-clip render state setter `004ef750` treats ordinal 6 as a target sample and selects direction relative to the current sample.
  - Correct the explanatory comments while preserving enum order and numeric values.

## Resolved

No items yet.
