# Uprising retail decompilation review checklist

<!-- Reborn: Tracks Mod Builder representations that conflict with read-only retail binary evidence. -->

This file is a review queue, not an authorization to change compiler layouts. Each item must be independently confirmed against the native asset stream and existing compiler tests before implementation.

## Open

- [ ] **TemporaryOwnerSpecialPowerModuleData — verify the EP1 `SlaveStatus` writer and layout.**
  - Reborn: Retail `TemporaryOwnerSpecialPower` uses module-data `+0x1dc` as the status mask. Cleanup `00813040` passes that address to `007622e0` to clear the configured flags.
  - The named EP1 XSD declares `SlaveStatus` as `ObjectStatusBitFlags`. The current C# module contains only the base struct, and its marshaler delegates to the base writer without reading that attribute.
  - Compile named EP1 modules with the attribute omitted and with a nonzero mask. Compare the emitted mask words, record size, relocations and metadata against a stock or EA-compiled witness. Verify the ordinary GameObject route before changing the shared writer.


- [ ] **SupplyCenterDockUpdateModuleData — verify the named EP1 XML writer.**
  - Reborn: Retail dock action `00791d20` tests a byte at module-data `+0x20` (instruction `00791f98`). A set byte enables the credit-capacity check before payment. The retail valuation helper reads `ValueMultiplier` at `+0x1c`.
  - The EA EP1 schema declares only `ValueMultiplier` and `DistributedDeposit` after the base. The shared C# module and marshaler used by the ordinary GameObject switch insert `BonusScience` and `BonusScienceMultiplier` between them. The generated layout report estimates `DistributedDeposit` at `+0x28` for that model.
  - Compile named `SupplyCenterDockUpdate` modules with `ValueMultiplier="1.75"` and `DistributedDeposit` set to false and true. Compare emitted payload bytes, record size, and stream metadata with stock or EA-compiled EP1 data. Verify the ordinary named GameObject route and the association between the serialized field and the runtime byte. Keep a separate legacy-target fixture if the EP1 writer needs its own layout.

- [ ] **GateProxyBehaviorModuleData — verify the direct EP1 GameObject writer.**
  - Reborn: Retail descriptor `00d03b04` registers `GateProxyBehavior` as type `0x0d6bb67b`. Factory `006fc680` and constructor `00719fb0` create a derived gate. The added field at instance `+0x50` is a runtime target pointer populated by `00767670`.
  - The EP1 `AssetTypeGameObject.xsd` directly exposes `GateProxyBehavior`, and its data XSD extends the base gate with an empty sequence. A concrete C# marshaler or dispatch entry for the proxy was not found in the reviewed sources. The regular compile path has not been exercised for this type.
  - Compile a named proxy without an authored TypeId, set nondefault inherited gate attributes, and verify that the emitted module retains `0x0d6bb67b` and the inherited fields. Compare the payload, relocations and metadata with a stock or EA-compiled witness before changing the writer.

## Resolved

The resolved entries record both verified compiler support and unavailable authoring routes. The closing audit checked 24 concrete class names across 21 classified items, 38 direct and inherited XML probes, and 653 linked retail BIN entries. The known StraightLine control returned 30 aligned matches.

- [x] **EP1 movie-archive row layout — correct stale assertions.**
  - Reborn: Retail linked static `UIConfigList:DynamicUIConfigList` contains two movie archive components. Both have 1 General, 3 Scenario, and 17 Campaign rows. The source strings and unlock values align at strides 40, 44, and 48 bytes. Rows start with a zero base word. PreviewImage is at +12, Icon at +32, Scenario UnlockRequirement at +40, and Campaign Faction/ProgressLock at +40/+44.
  - Verified all 21 PreviewImage references in each component against the retail IMP slots and the manifest asset identities.
  - The current x86 writer uses the retail layout. Updated both layout and compiler smoke assertions without changing the writer. The focused movie compiler fixture passed with BIN/RELO/IMP sizes 248/64/16.
  - Rebuilt the x86 inspector in isolation and ran the full layout-self-test through the last assertion. It returned Uprising layout self-test: OK. No additional layout failures followed the movie-archive assertions.

- [x] **SquishCollideModuleData — verify the named EP1 XML route selects the retail layout.**
  - Retail factory 006fdb50, constructor 0073ee40 and collision callback 0080fc10 confirm a float CrushAnimTime at +0x08, UseVehicleStyleCrush at +0x0c and AllowAllyCrush at +0x0d. EA EP1 schema exposes the named SquishCollide element.
  - The reviewed TypeId 0xB5EB41ED route explicitly selects the matching 0x10-byte Ep1SquishCollideModuleData. The ordinary GameObject switch still selects the legacy SquishCollideModuleData for the same ID. That legacy model writes CrushKillDelay at +0x0c and UseDirectionCheck at +0x10.
  - Compile a named EP1 SquishCollide without authored TypeId, set the two booleans independently, and compare the emitted record and metadata with a stock asset. If that path reaches the legacy switch, route it to the EP1 model and retain a separate legacy-target test.
  - Reborn: The normal EP1 complete-object path normalizes named XML without an authored TypeId and selects Ep1SquishCollideModuleData. Both independent flag variants passed module graph compilation, stock metadata comparison, stream/package readback and corruption checks. CrushAnimTime is at +0x08 and the flag bytes at +0x0c/+0x0d. The shared legacy switch is outside this EP1 path.

- [x] **FireWeaponWhenDeadBehaviorModuleData — qualify the generic EP1 dispatch.**
  - Retail die callback `007dd7d0` reads FireAtGround at `+0x8f` and UseMyKillerAsWeaponSource at `+0x90`. Registration uses `0x90ceb49c`. The EP1 schema declares both flags.
  - The reviewed EP1 selection route uses the 148-byte Ep1FireWeaponWhenDeadBehaviorModuleData and writes both bytes. The generic GameObject switch still maps the same ID to the 144-byte legacy model, which has neither field.
  - Exercise the generic EP1 authoring path with both flags set and compare its data, imports, metadata and round trip to the reviewed selection route.
  - Reborn: The normal EP1 complete-object path selects the existing EP1 writer. The fire-weapon-death-self-test passed three original native/metadata witnesses and authored nondefault/optional/corruption checks. The legacy shared switch is outside this EP1 path.

- [x] **CreateObjectDieModuleData — qualify the generic EP1 dispatch.**
  - Retail die callback `007a3790` reads CreationList at `+0x34`, required count/pointer at `+0x38/+0x3c` and forbidden count/pointer at `+0x40/+0x44`.
  - The reviewed EP1 route uses the 72-byte Ep1CreateObjectDieModuleData. The generic GameObject route for `0x3b68b768` selects the 80-byte legacy model. Its extra DebrisPortionOfSelf length/pointer pair at `+0x38/+0x3c` shifts both lists eight bytes later.
  - Exercise the generic EP1 authoring route with both upgrade lists populated and compare native offsets, imports, metadata and round trip to the reviewed EP1 route.
  - Reborn: The normal EP1 path selects the 72-byte header. The death-expanded complete-object test passed with both upgrade lists populated, plus 65 native and 70 metadata corruption refusals. The shared legacy switch is outside this path.

- [x] **FormationAssistantTemplate — check equal cell counts across rows before emitting the asset.**
  - The EP1 XSD permits each Row to contain an independent number of Cell elements. A probe against the deployed x86 SDK passed both 1,1 and 1,2 cell counts through the admitted schema and StockAssetRootCompiler.Compile. The emitted instance buffers preserved those exact counts at the 8-byte row stride.
  - Retail create callback `00584ea0` compares each row count with the first at `00584ec3`. A mismatch branches to the null return at `00584f72` before runtime allocation. Replacement callback `00584f80` performs the same check at `00584fa0` and exits before touching the current wrapper.
  - Add an authored diagnostic for unequal widths and qualify it with the rectangular and uneven fixtures. Keep each cell's variable Definition list independent. The retail rule compares the number of cells per row.
  - Reborn: Added equal-row-width validation in the EP1 writer. The 1/1-cell fixture kept identical BIN/RELO/IMP digests and the 1/2-cell fixture failed at the authored input check.

- [x] **UIMouseTacticalLookAt — diagnose ViewBookmark lists longer than eight rows.**
  - The EP1 LookAt schema declares `ViewBookmark` with `maxOccurs="unbounded"`. The x86 source-tree Mod Builder compiler accepts eight and nine rows, preserving counts 8 and 9 at instance `+0x34` in 124-byte and 132-byte buffers.
  - Retail initializer `00a5fdf0` reads the serialized count at `00a60017`, substitutes the constant 8 at `00c8e00c` when it is exceeded, and uses the capped value as the loop bound at `00a600a3`. The ninth row is emitted but receives no runtime binding.
  - Add an EP1 authoring diagnostic for more than eight rows. Re-run the 8-row and 9-row fixtures from Reborn’s `Reconstruction/UIMouseTacticalLookAt/LookAtAssetProbe.cs` against the production authoring path.
  - Reborn: Added a maximum of eight bookmark rows to EP1 authoring. Eight rows kept identical BIN/RELO/IMP digests and nine rows were rejected.

- [x] **EP1 AnimationMode comments — correct LOOP_PINGPONG and PLAY_TO_FRAME descriptions.**
  - `schemas/ra3ep1/xsd/Includes/AnimationMode.xsd` associates the play-to-frame comment with ordinal 5 LOOP_PINGPONG and the reversal comment with ordinal 6 PLAY_TO_FRAME.
  - Retail draw update dispatch at `008d92e7` routes ordinal 5 to `008d9377`, which reverses direction at boundaries. Ordinal 6 skips automatic advancement in that loop. Single-clip render state setter `004ef750` treats ordinal 6 as a target sample and selects direction relative to the current sample.
  - Correct the explanatory comments while preserving enum order and numeric values.
  - Reborn: The two explanations now match the retail dispatch. The enum order and numeric values were preserved.

- [x] **EP1 ProjectileUpdate BalloonProjectile — support the schema-supported movement mode.**
  - Reborn: Scanned all 653 linked BIN entries in the installed Uprising archives, including maps and WorldBuilder. None contained an aligned 1A5B5A69 word. The known StraightLine class returned 30 hits.
  - The independent EA BinaryAssetBuilder 1.0.3344 output and Uprising reader agree on movement ordinal 2, the relocation at ProjectileUpdate +0x18, and a four-byte 1A5B5A69 allocation with no imports. EA metadata selects the same concrete wrapper with one ProjectileMoveBase inheritance row (ABD1B08A). The straight-line control had the expected retail concrete identity and 16-byte payload.
  - Added BalloonMoveToGround to EP1 source admission, native writing, metadata generation and independent proof. Dying-projectile self-tests passed 39 checks in developer and embedded-metadata modes, including a complete named GameObject, stock falling/straight-line controls and corruption rejection.

- [x] **EP1 ProductionUpdate QuantityModifier — support populated lists.**
  - Reborn: All 52 identified retail static ProductionUpdate modules had both lists empty. Independent EA BinaryAssetBuilder 1.0.3344 produced the EP1-compatible 0x54-byte header and count/pointer at +0x3c/+0x40. The two eight-byte rows preserve weak GameObject IDs and signed counts -3/7 in source order. The metadata rows target the exact Name and Count offsets.
  - Enabled the EP1 QuantityModifier writer without enabling ProductionModifier. Complete named GameObject tests verified two distinct weak IDs, signed counts, stock-compatible metadata and package readback.

- [x] **EP1 ProductionUpdate ProductionModifier — support populated lists with native EP1 filters and upgrade imports.**
  - Reborn: The independent EA record establishes the four outer metadata tags. The retail EP1 loader at 00457F80 confirms RequiredUpgrade uses the standard strong resolver at row +0x00 and the shared ObjectFilter reader 0044A200 writes the inline filter at +0x0c. The stock SovietWallPiece filter matches that reader, including the EP1 second KindOf mask at +0x38 rather than the RA3 +0x34. The EP1 list allocator uses 0x84-byte rows.
  - The normal source route now emits zero to two rows, preserves their source order and writes the existing EP1 ObjectFilter ABI. Two distinct stock upgrade identities were written as external EP1 IMP slots at the starts of their rows. The test exercised both multipliers, omitted defaults, inline masks, a status pointer, weak include/exclude lists, and corruption rejection.
  - All five production graph/package cases passed in embedded-metadata mode. The populated case passed developer mode with 104 native and 27 metadata corruption refusals. The x86 layout test remains green.

- [x] **DestructibleEnvironmentUpdateModuleData — support the named EP1 XML route.**
  - Reborn: The independent EA GameObject fixture emits a 0x28-byte 75FE8D90 record. Retail EP1 reader 00432CA0 matches its exact field tags and uses Size at +0x08, inline vectors at +0x0c and +0x18, and Enabled at +0x24. The shared UpdateModuleData metadata comes from the native EP1 SovietRefinery witness.
  - The named EP1 route compiles two independent authored records with both vector offsets and default/nondefault vehicle size and Enabled values. Developer and embedded-metadata checks pass through complete-object stream and package readback. No installed Uprising asset instance is claimed.

- [x] **SwitchLocomotorsSpecialAbilityUpdateModuleData — compile the named EP1 module with LocomotorToSet.**
  - Reborn: EA BinaryAssetBuilder 1.0.3344 emitted class 14E031A2, tag 845D12B2 and selector words 0 for omitted and 2 for FREEFALL. EP1 reader 00490915 writes the selector at +0x108. The 0x110-byte EP1 leaf retains the live SpecialAbilityUpdate parent, native floats at +0xfc/+0x100, the strong power import at +0x104, and byte flags at +0x10c..+0x10e.
  - Reborn: locomotor-ability passed the named two-module complete GameObject, metadata and package round trip in production and developer modes. The test checks omitted defaults, FREEFALL, separate flags, power identity, and rejects native selector and metadata-class corruption.

- [x] **ReplaceSelfUpgradeModuleData — compile the named EP1 module with its option mask and weak template list.**
  - Reborn: EA BinaryAssetBuilder 1.0.3344 emitted class 53FBA9B5, option tag D0708290, default word 8, explicit mask 0x15, and ordered weak GameObject IDs. EP1 readers 00492747 and 00492782 confirm the 0x34-byte Upgrade-based storage with timing +0x24, options +0x28 and count/pointer +0x2c/+0x30.
  - Reborn: replace-self-upgrade passed the named two-module complete GameObject, metadata and package round trip in production and developer modes. The test checks inherited triggers, an explicit mask, two ordered weak IDs, empty defaults, and rejects native option and metadata-class corruption.

- [x] **SubStructureBuildUpgradeModuleData — support the named SubStructureBuild element.**
  - Reborn: The original SDKs use the SubStructureBuild XML name. The authored EA fixture and EP1 readers prove class 3176B098, list headers +0x24/+0x2c, eight-byte weak upgrade/object pairs, and 72-byte offset records with a 60-byte mask followed by the vector.
  - Reborn: substructure-build passed the GameObject and package round trip in production and developer modes with two pairs, two offsets, and empty defaults. The independent verifier rejected vector corruption.

- [x] **BeamSpecialAbilityUpdateModuleData — quarantine the obsolete writer and model.**
  - Reborn: The legacy type and writer now name their legacy role explicitly and the function no longer participates in Marshal overload resolution. The actual GameObject dispatch and metadata mapping still select the independent EP1 model.
  - Reborn: allied-unit-modules-self-test verified the original AlliedFutureTank beam through the source, native and metadata paths after the quarantine.

- [x] **DelayedWeaponFireSpecialAbilityUpdateModuleData — compile the EP1 WeaponFire extension.**
  - Reborn: EA BinaryAssetBuilder 1.0.3344 emitted concrete class 368F2215, WeaponFire parent 1E45A393, and FireDelay tag 998669E8 with explicit 4.25s and default zero. EP1 code at 00722A51 reads +0x11c, after the independently qualified 0x11c-byte WeaponFire parent. The full record is 0x120 bytes.
  - Reborn: delayed-weapon-fire passed the GameObject, metadata and complete package round trip in production and developer modes with an inherited weapon import, explicit delay and defaults. The compiler test rejects native delay corruption. The game scheduler itself was not executed.

- [x] **LeechTargetingAttachUpdateModuleData — compile with the retail EP1 Attach base.**
  - Reborn: EA emitted class CA6038A6 with only parent row E9FD14C9 and the EP1 allocator 004C9653 confirms 0x170 bytes. Retail Scout and MoneyGain attachment data independently prove the optional base records and order. EP1 dispatch now uses the EP1 base without changing legacy dispatch.
  - Reborn: leech-targeting passed complete GameObject, metadata and package tests in production and developer modes. The test checks a nonempty bone string, filter relocation, modifier identity and defaults. It rejects bone corruption.

- [x] **RepairUpdateModuleData — support populated SelfAudio imports.**
  - Reborn: EA BinaryAssetBuilder emitted two distinct AudioEvent references at +0x20/+0x24. Those positions and metadata tag 63D7B468 agree with the EP1 sound selector and the original blank SovietRefinery record. Both source references are now verified against the real catalog and import slots.
  - Reborn: The repair regression tests include two populated imports and absent sounds, both validated in production and developer modes through GameObject package readback. Playback in the game was not tested.

- [x] **TransportAIUpdateModuleData — compile with the EP1 AI parent.**
  - Reborn: EA emitted BE9CD6FE with a single inherited row 08DBC00E. The EP1 reader and the stock AI parent prove the 76-byte record, FadeOnPortals at +0x49 and the optional chooser pointer at +0x44.
  - Reborn: The named transport-ai GameObject package test passed in production and developer modes with a live target chooser, distinct locomotor and spawn radius. Source/native and metadata corruption are rejected.

- [x] **WanderAIUpdateModuleData — compile the EP1 AI parent and its Wander fields.**
  - Reborn: EA emitted D813AF2A with the inherited AI row and the Wander tail. EP1 reads the 60-byte EntryCondition mask at +0x4c, unsigned WanderDistance at +0x88 and the flags at +0x8c/+0x8d in a 0x90-byte header.
  - Reborn: The named wander-ai GameObject package test passed in production and developer modes with a populated mask, chooser, explicit distance and distinct flags. Source/native and metadata corruption are rejected.

- [x] **EMPUpdateModuleData — compile optional EP1 RGB color records.**
  - Reborn: The EA fixture and EP1 reader agree on concrete D3302D7E, 0x28-byte header, optional color pointers +0x20/+0x24 and 028426A3 rows for separate three-float records.
  - Reborn: The named emp-update GameObject package test passed in production and developer modes with two colors, one color and absent colors. It proves the relocation slots and rejects corrupt component values.

- [x] **WeaponSetUpdateModuleData — compile and verify EP1 contained weapon slots.**
  - Reborn: EA emitted a BA2D72E2 list of two 0x1c-byte slots. Each direct item carries parent row CE5853D7 and a zero native TypeId. The reader consumes count and pointer at +0x24/+0x28.
  - Reborn: The generic EP1 dispatch and complete GameObject test cover a mixed hardpoint/contained graph, authored IDs, a nested strong weapon import and native corruption. Package and metadata checks passed in both build modes.

- [x] **WeaponScatterEffectUpdate — qualify signed ScatterAmount boundaries.**
  - Reborn: The named EA fixture and EP1 package test retain signed words FFFFFFFF, 000000FF and 00000100 for -1, 255 and 256. EP1 runtime 0081CFDA reads only the low byte, so those values act as 255, 255 and zero at that consumer.
  - Reborn: Retained the signed schema and stock serializer behavior. Normal and developer runs verified the same words without imposing an unsigned-byte authoring restriction.

- [x] **GiveOrRestoreUpgradeSpecialPowerModuleData — compile the EP1 command and upgrade reference.**
  - Reborn: EA emits class 0453E648 with CommandButton tag 1A2933AA and UpgradeToGive tag 4ABA1F29. EP1 loader 00491490 maps them to +0xfc and +0x104 in the 0x108-byte allocation. The EP1 record ends after those two fields.
  - Reborn: The complete GameObject package regression passed in normal and developer modes with authored command text and a real upgrade import as well as omitted defaults. Corrupt command data was rejected.

- [x] **ActivateModuleSpecialAbilityUpdateModuleData — compile the EP1 local activation list.**
  - Reborn: EA emitted class CD33BCEE and list tag ADA54D29 with two distinct local Poid values. The retail reader uses count and pointer +0xfc/+0x100 and 12-byte items (ModuleId, TriggerType, TriggerAtTargetPosition). The supported class reuses the independently qualified SpecialAbilityUpdate base.
  - Reborn: The normal and developer GameObject package tests pass with forward and backward module links, mixed triggers, an empty list and corruption controls. An explicitly missing local target is rejected before compilation.

- [x] **AttributeModifierNavalModuleData — compile the EP1 naval attribute modifier.**
  - Reborn: EA emitted class A34CED5C with a CreateModuleData parent and reference tag 09DDCFF3. The retail create callback reads the strong modifier at +0x08 in the 12-byte record.
  - Reborn: Normal and developer GameObject package tests both verify the exact named modifier import, an omitted default and corrupt native input.

- [x] **InactiveBodyModuleData — compile the named EP1 Body child.**
  - Reborn: EA emitted class 29234E6E and one inherited BodyModuleData row (F49198C2). EP1 allocator 004BB5D9 verifies the eight-byte record.
  - Reborn: The named Body route passes the normal and developer package tests. They verify the root relocation, class identity and the local ID corruption control.

- [x] **CreateAndEnterObjectSpecialPowerModuleData — compile the EP1 OCL extension.**
  - Reborn: EA emits the named derived class and FXOffset field after OCLSpecialPower. The EP1 variant uses the independently validated 524-byte OCL base and a pointer at +0x20c.
  - Reborn: Production and developer GameObject packages verify authored offsets, ordered upgrade OCL imports, optional vector and native corruption.

- [x] **ObjectStatusSpecialAbilityUpdateModuleData — compile the EP1 status mask pair.**
  - Reborn: The original EA serializer emits class 079B6EAD with separate mask rows. The retail destinations +0xfc/+0x11c anchor two 32-byte inline masks over SpecialAbilityUpdate.
  - Reborn: Normal and developer GameObject package tests verify distinct authored flags, omitted defaults and native mask corruption.

- [x] **ShieldBodyModuleData — compile the EP1 ActiveBody extension and optional shield armor.**
  - Reborn: The EA fixture confirms the four FX fields, shield amount, armor reference, recharge time and active model condition. The EP1 reader and allocator prove a 0x6c-byte header with the tail at +0x4c through +0x68. The optional armor import is reached through a relocation from +0x60.
  - Reborn: Normal and developer GameObject package checks pass for populated shield data (including real FX and armor references plus an inherited DamageRangeModifier) and omitted shield values. Native corruption is rejected.

- [x] **SpecialPowerUpdateModuleData — emit the EP1 mask, pause flag and sound/EVA data.**
  - Reborn: The named EP1 GameObject route emits a 24-byte record. DisabledTypesToIgnore defaults to HELD at +0x0c. The optional InitiateSound pointer and StartsPaused byte are at +0x10 and +0x14.
  - Reborn: Normal and developer package tests check the default and explicit masks, kind and order of the sound/EVA imports, relocations and a corrupt native mask. The resulting BIN and BIG files match byte for byte.

- [x] **DamageFieldUpdateModuleData — compile the verified EP1 firing base and object filter.**
  - Reborn: The named GameObject route uses the 32-byte EP1 FireWeaponUpdate base. Radius, RequiredUpgrade and optional ObjectFilter follow at +0x20, +0x24 and +0x28.
  - Reborn: Normal and developer package tests exercise a real weapon nugget with offset and OneShot, status masks, populated filter, exact upgrade import, empty values and corruption rejection. BIN and BIG outputs match byte for byte.

- [x] **HordeAIUpdateModuleData — route the concrete class through the EP1 AI header.**
  - Reborn: The official named AI class keeps wire 55D77703 and a fieldless 0x4c-byte extent over the verified EP1 parent. The EA fixture independently confirms the inherited type, state-machine coordinate and FadeOnPortals byte.
  - Reborn: Normal and developer package tests exercise a populated inherited chooser with its real TargetingCompareList import, FREEFALL locomotor selector and state-machine ordinal 0x0d. The native corruption test is rejected and the BIN and BIG outputs match.
  - Reborn: The 653-entry linked retail BIN sweep had no aligned concrete class word. The named EA output supplies the concrete witness and the shared parent is independently verified against stock assets.

- [x] **SupplyTruckAIUpdateModuleData — use the EP1 AI base and verified truck tail.**
  - Reborn: The named AI route retains the 0x4c-byte EP1 base and emits the complete 0x74-byte truck. MaxBoxes is at +0x4c and the docking and harvesting flags are at +0x6f/+0x70.
  - Reborn: Normal and developer package tests verify distinct times and flag values, default announcement distance, chooser relocation and exact inherited import. A changed docking flag is rejected and the BIN and BIG outputs match.
  - Reborn: The 653-entry linked retail BIN sweep had no aligned concrete class word. The named EA output supplies the concrete witness and the shared parent is independently verified against stock assets.

- [x] **EP1 AI StateMachine selector — reject the generic OreRefinery selector with no runtime implementation.**
  - Reborn: Stock 1.1 generic AI dispatch resolves ordinal 0x0b to a zero return. The caller immediately dereferences the returned pointer. The generic GameObject AI route now reports the unsupported selector before allocating the module.
  - Reborn: The real inline GameObject dispatch test verifies the exact diagnostic for the unsupported selector. The valid TransportHelicopterAIStateMachine still compiles to ordinal 0x0d. Isolated native-layout tests continue to cover the enum values independently.

- [x] **EP1 ProjectileUpdate BezierSplineProjectile — compile the verified EP1 Bezier movement and path list.**
  - Reborn: EA 1.0.3344 emits B38F4984 and two ordered path imports even when given the EP1 leaf schema, but still omits the new adjustment and writes the RA3 0x18-byte form.
  - Reborn: EP1 reader 0045d160 binds the additional metadata row 90F269E4/645864F3 to +0x10. It passes +0x14 to list reader 0043fb10, which resolves a four-byte reference per item through the 2CCBA11C list tag. The source writer emits the 0x1c-byte EP1 form with count/items at +0x14/+0x18.
  - Reborn: Both package modes pass default gravity/adjustment, TRACK_TARGET, repeated occurrence slots and corrupted-class tests.
  - Reborn: The normal and developer BIN and BIG outputs match byte for byte.
  - Reborn: A separate mixed-stream build with two distinct local paths kept RebornRightPath before RebornLeftPath in the manifest and wrote both imports at the exact native row offsets. Its normal and developer BIN/BIG outputs also match.

- [x] **SphereModuleUpdateModuleData — compile the direct named EP1 sphere update.**
  - Reborn: Retail polymorphic reader 004b2a6c matches the direct 9D74F07A class and dispatches through the same sphere table as the inherited retail shield records. The compiler selects the existing x86 0xb0-byte sphere model through the named GameObject choice.
  - Reborn: Both package modes verify populated bone and vector relocations, both filters, duration and booleans with native fault rejection.
  - Reborn: The normal and developer BIN and BIG outputs match byte for byte.

- [x] **StealUnitUpdateModuleData — use the retail EP1 attachment ABI.**
  - Reborn: The named EA concrete class supplies 21068FF6 and its E9FD14C9 inheritance row. Retail EP1 dispatch 004b49a2 invokes 00493bc0, which delegates the complete inherited record to the EP1 attachment reader.
  - Reborn: Both modes verify a populated EP1 bone, filter and modifier import. The 0x170-byte model now reuses Ep1AttachUpdateModuleData, including its +0x15c Bone string.
  - Reborn: The normal and developer BIN and BIG outputs match byte for byte.

- [x] **BoobyTrapUpdateModuleData — write the EP1 attachment base and complete Booby Trap tail.**
  - Reborn: The EA header and retail type reader 00493c20 agree on the concrete E502295E wrapper and five owned rows. The retail EP1 reader validates both masks, weapon import, second filter and ally flag at +0x170/+0x190/+0x1b0/+0x1b4/+0x1b8.
  - Reborn: Both package modes verify inheritance and both filters, bone relocation, modifier and weapon imports, plus native corruption refusal.
  - Reborn: The normal and developer BIN and BIG outputs match byte for byte.

- [x] **EncloseBehaviorModuleData layout index — regenerate from x86 compiler field sizes.**
  - Reborn: The current-layout-index command emits x86-measured member and record sizes and offsets as JSON. It uses actual storage for two-word relocated strings, inline ObjectFilter fields, fixed-size flags and the rest of the Win32 model.
  - Reborn: The layout self-test verifies the serialized EncloseBehavior, SpecialPowerTemplate and its fixed flag buffer, WeaponTemplate, Shape and ProjectedBuildabilityInfo against the reviewed x86 coordinates. The old externally generated layout index is superseded for these types by the measured exporter.

- [x] **FXParticleSysBoneNugget layout index — export the measured two-word Bone and inline Offset.**
  - Reborn: X86 export and layout self-test verify the 0x54-byte FXParticleSysBoneNugget and 0x38-byte FXListAtBonePosFXNugget. A populated mixed stream verified Bone relocation, particle/FX imports, a nonzero vector and the update flag.
  - Reborn: Retail loader 004c7daa allocates 0x54 bytes and calls 0046d480. In the Offset branch, 0046d50a recognizes vector tag 29368077 and 0046d518/51e/524 copy XYZ into +0x44/+0x48/+0x4c. This qualifies the asset layout independently of whether a later FX consumer uses the vector.

- [x] **HordeMemberCollideModuleData — classify the unavailable EP1 authoring route.**
  - Reborn: The official RA3 and Uprising GameObject choices expose no direct XML element for this concrete type. The attempted inherited xsi:type route is rejected during source admission. Each of the five aligned retail matches belongs to a texture, as shown by the asset-range review. Coverage is classified as unavailable for Uprising XML authoring. Reopen if an independently serialized concrete record and a usable XML binding are identified.
  - Reborn: The official RA3 and EP1 derived XSDs match. EA BinaryAssetBuilder 1.0.3344 rejected the otherwise schema-derived Collide xsi:type route before emission, reporting that the xsi:type attribute lacks a schema type. The original serializer supplied no independent record for this route.
  - Reborn: The complete retail scan found five AA02AAFB matches. Mapping the offsets through static and WorldBuilder manifests identified all five as Texture payload words rather than GameObject module records. The effective source route remains open.

- [x] **ReflectDamageModuleData, EvacuateDamageModuleData, HordeTransportContainDamageModuleData and CallHelpOnDamageModuleData — classify the unavailable EP1 authoring route.**
  - Reborn: The official RA3 and Uprising GameObject choices expose no direct XML element for these four concrete types. The attempted inherited xsi:type route is rejected during source admission. The only aligned retail match belongs to a texture, as shown by the asset-range review. Coverage is classified as unavailable for Uprising XML authoring. Reopen if an independently serialized concrete record and a usable XML binding are identified.
  - Reborn: RA3 and EP1 module XSDs match. An EA authored probe through Damage xsi:type was rejected by the original compiler before emission, with the same xsi:type schema-type diagnostic as Collide. A supported route and actual module witness remain open.
  - Reborn: The single ReflectDamage class-word match belongs to WorldBuilder Texture:YP_Tree01. The other three names produced no aligned class-word match across the 653 linked BIN entries. No serialized module witness was identified.
  - EvacuateDamage factory 006fda70, constructor 00759e50 and callback 0078e500 establish a second runtime class. Runtime reads place the weak weapon reference, damage type, panic threshold and frame window at +0x08/+0x0c/+0x10/+0x14.

- [x] **DozerAIUpdateModuleData — classify the unavailable EP1 authoring route.**
  - Reborn: The official RA3 and Uprising GameObject choices expose no direct XML element for this concrete type. The schema-valid inherited xsi:type now fails at the authored attribute with an explicit concrete-writer diagnostic. The former path could admit the declaration and write its base class. The 653-stream retail sweep found no aligned occurrence of the concrete class. Coverage is classified as unavailable for Uprising XML authoring. Reopen if an independently serialized concrete record and a usable XML binding are identified.
  - Reborn: The official schema sources match. The original EA compiler refused the named base AIUpdate xsi:type route before it could emit the populated Dozer tail. No EP1 source/native comparison was possible for that route.
  - Reborn: The full retail BIG sweep produced no aligned 8D093323 word in a linked BIN. An independently authored native witness remains necessary.
  - Retail constructor `007fb5d0` delegates to the AI constructor `007ec990`; serializer `007d2ec0` delegates to `007c1ab0`. These confirm runtime inheritance. Verify the derived asset's inherited fields, the three native tail offsets, and metadata against an original compiled record before changing the model or marshaler.

- [x] **RadiateFearUpdateModuleData — classify the unavailable EP1 authoring route.**
  - Reborn: The official RA3 and Uprising GameObject choices expose no direct XML element for this concrete type. The 653-stream retail sweep found no aligned occurrence of the concrete class. Coverage is classified as unavailable for Uprising XML authoring. Reopen if an independently serialized concrete record and a usable XML binding are identified.
  - Reborn: The RA3 and EP1 leaf XSDs match. Neither GameObject schema declares a direct RadiateFearUpdate element. The original compiler probes for other xsi:type-based derived leaves failed before emission, so schema type presence alone cannot establish a usable route here.
  - Reborn: No aligned B145A627 class word was found across the retail linked BIN sweep.
  - Retail factory `006fcc10`, constructor `00748fb0`, update `00794820` and serializer `0072da60` establish a concrete runtime class. Runtime reads confirm radius, interval and victim filter at `+0x24/+0x28/+0x2c` and four boolean bytes at `+0x30..+0x33`.

- [x] **ToggleMountedSpecialAbilityUpdateModuleData — classify the unavailable EP1 authoring route.**
  - Reborn: The official RA3 and Uprising GameObject choices expose no direct XML element for this concrete type. The schema-valid inherited xsi:type now fails at the authored attribute with an explicit concrete-writer diagnostic. The former path could admit the declaration and write its base class. The 653-stream retail sweep found no aligned occurrence of the concrete class. Coverage is classified as unavailable for Uprising XML authoring. Reopen if an independently serialized concrete record and a usable XML binding are identified.
  - Reborn: The official RA3 and EP1 concrete leaf schemas match. Neither GameObject choice declares a direct ToggleMountedSpecialAbilityUpdate element. The EA xsi:type pipeline probes failed before emitting concrete derived base choices.
  - Retail `ra3ep1_1.1.game` evidence: factory `006fc790`, constructor `007a5db0`, custom xfer `007d2750`, main transition `007fa9b0`, opacity update `007a5e60`, custom update `007be520`, and cleanup `007be4c0`.
  - Retail derived layout after the `0xfc`-byte `SpecialAbilityUpdateModuleData` base:
  - Runtime size is `0xc4`, with a six-slot mounted interface at `+0xbc` and saved transition byte at `+0xc0`.

- [x] **ToggleHiddenSpecialAbilityUpdateModuleData — classify the unavailable EP1 authoring route.**
  - Reborn: The official RA3 and Uprising GameObject choices expose no direct XML element for this concrete type. The schema-valid inherited xsi:type now fails at the authored attribute with an explicit concrete-writer diagnostic. The former path could admit the declaration and write its base class. The 653-stream retail sweep found no aligned occurrence of the concrete class. Coverage is classified as unavailable for Uprising XML authoring. Reopen if an independently serialized concrete record and a usable XML binding are identified.
  - Reborn: The official RA3 and EP1 concrete leaf schemas match, but neither GameObject choice declares a direct ToggleHiddenSpecialAbilityUpdate element.
  - Retail `ra3ep1_1.1.game` evidence: factory `006fc7b0`, constructor `007a5d10`, custom xfer `007d2710`, toggle execution `007fa970`, timed update `007be3d0`, and extended visibility slots beginning at `00774850/007748c0`.
  - Retail derived layout after the `0xfc`-byte `SpecialAbilityUpdateModuleData` base:
  - Runtime class size is `0xc0`, with saved visibility state at `+0xbc`; the primary vtable extends to 54 slots.

- [x] **ToggleDeploySpecialAbilityUpdateModuleData — classify the unavailable EP1 authoring route.**
  - Reborn: The official RA3 and Uprising GameObject choices expose no direct XML element for this concrete type. The schema-valid inherited xsi:type now fails at the authored attribute with an explicit concrete-writer diagnostic. The former path could admit the declaration and write its base class. The 653-stream retail sweep found no aligned occurrence of the concrete class. Coverage is classified as unavailable for Uprising XML authoring. Reopen if an independently serialized concrete record and a usable XML binding are identified.
  - Reborn: The official RA3 and EP1 concrete leaf schemas match, but neither GameObject choice declares a direct ToggleDeploySpecialAbilityUpdate element.
  - Retail `ra3ep1_1.1.game` evidence: factory `006fc7d0`, constructor `007a5c80`, toggle execution `007fa930`, deploy/undeploy primary slots 45/46 at `0095b990` and `0075b4a0`, and update status `00744730`.
  - Retail derived layout after the `0xfc`-byte `SpecialAbilityUpdateModuleData` base:

- [x] **FlingPassengerSpecialAbilityUpdateModuleData — classify the unavailable EP1 authoring route.**
  - Reborn: The official RA3 and Uprising GameObject choices expose no direct XML element for this concrete type. The schema-valid inherited xsi:type now fails at the authored attribute with an explicit concrete-writer diagnostic. The former path could admit the declaration and write its base class. The 653-stream retail sweep found no aligned occurrence of the concrete class. Coverage is classified as unavailable for Uprising XML authoring. Reopen if an independently serialized concrete record and a usable XML binding are identified.
  - Reborn: The complete sweep of 653 linked Uprising BIN entries found zero aligned B5B4595D words with 30 hits for the StraightLine control. Full RA3 and EP1 GameObject type searches found no alternate XML name for this type. Public source authoring and an independent native serialization witness remain open.
  - Reborn: The leaf XSDs match between official RA3 and EP1. Neither stock GameObject schema exposes this type through a direct named element.
  - Retail evidence: factory `006fc8d0`, constructor `007a4d60`, serializer `007d20c0`, and passenger-release action `007f7830`; runtime size is the base-only `0xbc`.

- [x] **NotifyTargetsOfImminentProbableCrushingUpdateModuleData — classify the unavailable EP1 authoring route.**
  - Reborn: The official RA3 and Uprising GameObject choices expose no direct XML element for this concrete type. The 653-stream retail sweep found no aligned occurrence of the concrete class. Coverage is classified as unavailable for Uprising XML authoring. Reopen if an independently serialized concrete record and a usable XML binding are identified.
  - Reborn: The complete sweep of 653 linked Uprising BIN entries found zero aligned C7EDB319 words with 30 hits for the StraightLine control. Full RA3 and EP1 GameObject type searches found no alternate XML name for this type. Public source authoring and an independent native serialization witness remain open.
  - Reborn: The leaf XSDs match between official RA3 and EP1. Neither stock GameObject schema exposes this type through a direct named element.
  - Retail evidence: factory `006fc930`, constructor `0072c590`, update `007d6750`, scan helper `007d62f0`, and runtime size `0x24`.

- [x] **HordeNotifyTargetsOfImminentProbableCrushingUpdateModuleData — classify the unavailable EP1 authoring route.**
  - Reborn: The official RA3 and Uprising GameObject choices expose no direct XML element for this concrete type. The 653-stream retail sweep found no aligned occurrence of the concrete class. Coverage is classified as unavailable for Uprising XML authoring. Reopen if an independently serialized concrete record and a usable XML binding are identified.
  - Reborn: The complete sweep of 653 linked Uprising BIN entries found zero aligned 98B3BC44 words with 30 hits for the StraightLine control. Full RA3 and EP1 GameObject type searches found no alternate XML name for this type. Public source authoring and an independent native serialization witness remain open.
  - Reborn: The leaf XSDs match between official RA3 and EP1. Neither stock GameObject schema exposes this type through a direct named element.
  - Retail factory `006fc950` allocates `0x24` bytes. Constructor `0072acb0` installs an update table whose slot 0 returns the sleep sentinel at `007e2ed0`.

- [x] **ProneUpdate — classify the unavailable EP1 authoring route.**
  - Reborn: The official RA3 and Uprising GameObject choices expose no direct XML element for this concrete type. The 653-stream retail sweep found no aligned occurrence of the concrete class. Coverage is classified as unavailable for Uprising XML authoring. Reopen if an independently serialized concrete record and a usable XML binding are identified.
  - Reborn: No aligned D0FEA437 word was found in 653 linked original Uprising BIN entries. Official RA3 and EP1 GameObject schemas have no alternate name using this concrete type. Runtime class evidence and a usable compiler authoring route remain separate. A serialized module witness is still required.
  - Retail factory `006fcd90` constructs a `0x28`-byte runtime object at `0072d870`. Update `00794800` and transfer `0072d8f0` use the countdown at object `+0x24`.

- [x] **HijackerUpdateModuleData — classify the unavailable EP1 authoring route.**
  - Reborn: The official RA3 and Uprising GameObject choices expose no direct XML element for this concrete type. The 653-stream retail sweep found no aligned occurrence of the concrete class. Coverage is classified as unavailable for Uprising XML authoring. Reopen if an independently serialized concrete record and a usable XML binding are identified.
  - Reborn: No aligned AA02FF52 word was found in 653 linked original Uprising BIN entries. Official RA3 and EP1 GameObject schemas have no alternate name using this concrete type. Runtime class evidence and a usable compiler authoring route remain separate. A serialized module witness is still required.
  - Retail factory `006fcdd0` allocates `0x38` bytes. The ejection path at `00793990` passes module-data `+0x10` into the parachute template lookup.

- [x] **TemporarilyDefectUpdate — classify the unavailable EP1 authoring route.**
  - Reborn: The official RA3 and Uprising GameObject choices expose no direct XML element for this concrete type. The 653-stream retail sweep found no aligned occurrence of the concrete class. Coverage is classified as unavailable for Uprising XML authoring. Reopen if an independently serialized concrete record and a usable XML binding are identified.
  - Reborn: Both official GameObject schemas lack a named element for this type. A sweep of 653 linked retail BIN entries found no aligned 4D76910C word. The generic branch identifies a runtime data type, while the public XML route and reference record remain open.
  - Retail factory `006fd5d0` constructs the module. Start helper `007f13a0` adds module-data `+0x08` to the current frame when the caller supplies no deadline, matching `DefectionFrameCount` in EA's Uprising schema.

- [x] **EvacuateGarrisonSpecialPowerModuleData — classify the unavailable EP1 authoring route.**
  - Reborn: The official RA3 and Uprising GameObject choices expose no direct XML element for this concrete type. The schema-valid inherited xsi:type now fails at the authored attribute with an explicit concrete-writer diagnostic. The former path could admit the declaration and write its base class. The 653-stream retail sweep found no aligned occurrence of the concrete class. Coverage is classified as unavailable for Uprising XML authoring. Reopen if an independently serialized concrete record and a usable XML binding are identified.
  - Reborn: Neither official GameObject schema names this concrete type and the sweep found no aligned DB0BAC4C word in 653 linked retail BIN entries. An independent source/serialization witness is still needed.
  - Retail factory `006fd6d0` constructs a `0xbc`-byte runtime object. Transfer `007d20c0` delegates to the base, and callback `007e1590` resolves inherited target ID `+0x4c` before submitting order kind `0x3d`.

- [x] **PillageModuleData — classify the unavailable EP1 authoring route.**
  - Reborn: The official RA3 and Uprising GameObject choices expose no direct XML element for this concrete type. The 653-stream retail sweep found no aligned occurrence of the concrete class. Coverage is classified as unavailable for Uprising XML authoring. Reopen if an independently serialized concrete record and a usable XML binding are identified.
  - Reborn: Both official GameObject schemas lack a named route and no aligned 6CE208F7 word occurred in 653 linked retail BIN entries. The engine reader alone does not establish an independently serialized XML route.
  - Retail factory `006fd730` creates a `0x1c`-byte runtime object. The damage callback `0071ab80` reads the amount at module-data `+0x08`, threshold at `+0x0c` and filter at `+0x10`. It uses transaction category `0x1c` (PILLAGE).

- [x] **PreorderCreateModuleData — classify the unavailable EP1 authoring route.**
  - Reborn: The official RA3 and Uprising GameObject choices expose no direct XML element for this concrete type. The 653-stream retail sweep found no aligned occurrence of the concrete class. Coverage is classified as unavailable for Uprising XML authoring. Reopen if an independently serialized concrete record and a usable XML binding are identified.
  - Reborn: Both official GameObject schemas lack a named element for this create type. The 653 linked retail BIN entries had no aligned 2A36DC1F word (StraightLine control 30). An authored or serialized witness remains open.
  - Retail factory `006fd8d0` allocates the `0x18`-byte runtime object. The create callback updates the PREORDER model condition. Transfer delegates to the inherited creation-state path.

- [x] **SpecialPowerCreateModuleData — classify the unavailable EP1 authoring route.**
  - Reborn: The official RA3 and Uprising GameObject choices expose no direct XML element for this concrete type. The 653-stream retail sweep found no aligned occurrence of the concrete class. Coverage is classified as unavailable for Uprising XML authoring. Reopen if an independently serialized concrete record and a usable XML binding are identified.
  - Reborn: Neither official GameObject schema exposes a named route. The same sweep found zero aligned 6A8C7338 words. Runtime registration alone supplies no authoring contract.
  - Retail factory `006fd950` allocates the module. Build-complete callback `0070cf30` notifies each attached special-power interface, consistent with EA's Zero Hour implementation.

- [x] **WeaponModeSpecialPowerUpdateModuleData — classify the unavailable EP1 authoring route.**
  - Reborn: The official RA3 and Uprising GameObject choices expose no direct XML element for this concrete type. The schema-valid inherited xsi:type now fails at the authored attribute with an explicit concrete-writer diagnostic. The former path could admit the declaration and write its base class. The 653-stream retail sweep found no aligned occurrence of the concrete class. Coverage is classified as unavailable for Uprising XML authoring. Reopen if an independently serialized concrete record and a usable XML binding are identified.
  - Reborn: Neither official GameObject schema exposes the named element. A sweep of 653 linked retail BIN entries found zero aligned 3041E295 words, with 30 StraightLine control hits.
  - EA's Uprising type schema declares Duration, AttributeModifier and LockWeaponSlot. Retail registers type `0x3041e295`. The inspected GameObject schema has no matching element and the compiler has no concrete model or dispatch.
  - Retail activation `0075fab0` reads Duration at `+0x18` and AttributeModifier at `+0x1c`. Expiry removes the modifier. The LockWeaponSlot access remains unverified.

- [x] **DeflectSpecialPowerModuleData — classify the unavailable EP1 authoring route.**
  - Reborn: The official RA3 and Uprising GameObject choices expose no direct XML element for this concrete type. The schema-valid inherited xsi:type now fails at the authored attribute with an explicit concrete-writer diagnostic. The former path could admit the declaration and write its base class. The 653-stream retail sweep found no aligned occurrence of the concrete class. Coverage is classified as unavailable for Uprising XML authoring. Reopen if an independently serialized concrete record and a usable XML binding are identified.
  - Reborn: Neither official GameObject schema exposes a named element. The 653-entry retail sweep found zero aligned 7FA7DEFC words.
  - EA's Uprising type schema declares a fieldless SpecialPowerUpdateModuleData extension. Retail registers type `0x7fa7defc` and allocates its runtime object through `006fdf70`. The GameObject schema and compiler have no matching route.

- [x] **DamageFilteredCreateObjectDieModuleData — classify the unavailable EP1 authoring route.**
  - Reborn: The official RA3 and Uprising GameObject choices expose no direct XML element for this concrete type. The 653-stream retail sweep found no aligned occurrence of the concrete class. Coverage is classified as unavailable for Uprising XML authoring. Reopen if an independently serialized concrete record and a usable XML binding are identified.
  - Reborn: Neither official GameObject schema exposes a named element. The 653-entry retail sweep found zero aligned 76AB5E78 words.

- [x] **TransportHelicopterAIUpdateModuleData — classify the unavailable EP1 authoring route.**
  - Reborn: The official RA3 and Uprising GameObject choices expose no direct XML element for this concrete type. The attempted inherited xsi:type route is rejected during source admission. The 653-stream retail sweep found no aligned occurrence of the concrete class. Coverage is classified as unavailable for Uprising XML authoring. Reopen if an independently serialized concrete record and a usable XML binding are identified.
  - Reborn: This is the unresolved standalone type previously tracked under Supply Truck and the state-machine selector. The schema has no named GameObject route and the inspected 1.1 executable has no matching standalone class identity. The separate TransportHelicopterAIStateMachine selector is valid on a generic AIUpdate.
