# RA3 Uprising (EP1) migration status

This branch starts the migration with a read-only compatibility gate. It does
not yet claim that BinaryAssetBuilder can emit Uprising-compatible streams.

## Progress snapshot (2026-09-19)

The current conservative engineering estimate is **34% complete / 66%
remaining**. This is an effort estimate, not the percentage of C# files in the
tree. A pre-existing Kane's Wrath marshaller only counts as complete after its
RA3/EP1 layout, type hash and emitted streams have been checked.

| Workstream | Share of total effort | Complete | Weighted contribution |
|---|---:|---:|---:|
| Manifest/BIG/RefPack readers, v7 writer and safety gates | 15% | 80% | 12.0% |
| Official RA3-to-EP1 schema inventory and generated enums | 15% | 65% | 9.8% |
| Native layouts, processors, dispatch and final type table | 45% | 20% | 9.0% |
| Target-aware SDK scripts, dependencies and WorldBuilder packaging | 15% | 20% | 3.0% |
| Built-mod validation inside Uprising | 10% | 0% | 0.0% |

The reproducible structural counter is `scripts/Get-Ra3Ep1PortCoverage.ps1`.
At this snapshot the 843 EP1 XSD files declare 1,390 unique complex types.
The source tree contains models for 731 (52.6%) and typed marshallers for 707
(50.9%). These broad numbers are inventory coverage only. Of the 48 complex
types that exist only in EP1, 16 (33.3%) now have both a model and marshaller;
32 remain absent. The smaller audited set carries substantially more weight
than raw file presence in the 34% estimate above.

## Established facts

- Red Alert 3 manifests use version 6 and `AllTypesHash=0x54EEE764`.
- Uprising manifests use version 7 and `AllTypesHash=0x5454A8E9`.
- Version 7 changes the first four header bytes from
  `IsBigEndian, IsLinked, Version` to `Version, IsBigEndian, IsLinked`.
- The sampled Uprising BIG entries may be raw or RefPack-compressed.
- Uprising preserves the 48-byte version 6/7 asset-entry layout, including the
  tokenized word, but its changed schemas produce different per-type hashes.
- Referenced-manifest markers retain `1=normal` and `2=patch` semantics in the
  examined fixtures.
- A structural comparison of the official schema trees finds 22 added and 75
  changed XSD files. Therefore replacing only the source XML is insufficient.

Validated fixture fingerprints:

| Fixture | Version | Assets | RefPack | Result |
|---|---:|---:|---:|---|
| RA3 `worldbuilder.manifest` | 6 | 15,931 | no | pass |
| Uprising `WBData.big::data/worldbuilder.manifest` | 7 | 17,339 | no | pass |
| Uprising `StaticStream.big::data/static.manifest` | 7 | 13,872 | yes | pass |
| Uprising `GlobalStream.big::data/global.manifest` | 7 | 11,357 | yes | pass |

`D:\TEMP\Red Alert 3 Uprising Source Data` supplies unpacked raw stream sets
for `worldbuilder`, `static`, `static_l`, `static_m`, `global`, `locale`, and
`audio`, including their `.manifest/.bin/.relo/.imp` companions. The raw
`worldbuilder.bin` is 1,394,571,528 bytes. SHA-256 confirms that its
`static.manifest` and `static.bin` are byte-identical to the independently
recovered SDK build, so these are trustworthy cross-check fixtures rather than
two independent builds. The other raw streams remove the previous RefPack
random-access limitation and expand the asset-level validation surface. The
`static_l` and `static_m` manifests are LOD overlays referencing `static` as a
patch base: their header total describes the merged logical stream while their
BIN contains only local replacement chunks. Manifest validation now recognizes
this distinction and keeps strict chunk-total equality for standalone streams.

## Implemented compatibility gate

`BinaryAssetBuilder.ManifestInspector` can:

- identify TW, Kane's Wrath, RA3 and Uprising manifest fingerprints;
- read standalone manifests and selected entries from BIG4/BIGF archives;
- decompress bounded RefPack manifest payloads without loading large BIN
  streams;
- parse v5, v6 and v7 headers, asset entries, references and name tables;
- verify offsets, counts, duplicate IDs and instance-stream totals;
- compare type IDs and type hashes between two manifests;
- compare XSD trees structurally while ignoring formatting and annotations.

The official Uprising schemas are staged under `schemas/ra3ep1/xsd`. The full
528 MB XML source tree remains external and read-only; it should be supplied to
build scripts as an explicit source root instead of being duplicated here.

The first concrete game-data layout port covers `ArmorTemplate` and
`AttributeModifier`. Metadata-only disassembly of EA's RA3 compiler established
the original 60-byte `AttributeModifier` offsets. Uprising removes
`RemoveWhenDisabledByTypes`, producing the implemented 56-byte EP1 layout, and
adds `ArmorSetType`, `Shader`, `SHRINK` and seven attribute modifiers. The EP1
XSDs establish bit capacities of 463 model conditions (464 enum values minus
the non-bit `INVALID=-1` sentinel), 230 object statuses, 13
disabled types and 23 armor-set types. `layout-self-test` locks these sizes and
offsets against regression. `Sync-Ra3Ep1Enums.ps1` regenerates the complete
ModelCondition, ObjectStatus, Disabled, ArmorSet and AttributeModifier enum
ordering directly from the checked-in EP1 XSDs; its `-Check` mode is part of the
compatibility smoke test.

`compiler-self-test` now exercises the actual XML marshaller with EP1-only
values (`SHRINK`, `SPECIAL_POWER_SELECTED_PENDING`, `BRIDGE_DEAD`,
`SHRINK_EFFECT`, and `RADIATION_ARMOR`). It produces and verifies a relocatable
216-byte instance chunk, a 20-byte `.relo` chunk and a 16-byte `.imp` chunk.
This proves the migrated compiler core, but deliberately does not
bypass the incomplete-plugin manifest gate.

The next registered type audit exposed and corrected a larger inherited problem:
`LocomotorTemplate` in the KW-derived source was 348 bytes, while EA's
RA3 compiler metadata declares a 396-byte structure. The RA3 marshaller also
contains fields absent from this source, including `SpeedBasedHeightOffset`,
`ResubmergeDelay`, `WaterToAirTransitionFX`, `WaterSurfaceHeightOffset`,
`ForbiddenObjectStatus`, `BounceKickTerrainMap`, and `JetLocomotorData`.
The structure and marshaller have now been rebuilt to the official RA3 offsets,
then extended with Uprising's `IgnoreLowSpeedAngleMultiplier` at offset 393.
The final EP1 structure remains 396 bytes because the new boolean occupies RA3
tail padding. A second compiler PoC validates Uprising-only surface, height-mode,
jet-option and low-speed-angle values (`416 bin / 8 relo / 0 imp`).

`WeaponTemplate` is the third audited root type. EA's RA3 tokenizer declares a
328-byte structure; the inherited KW-oriented source was 392 bytes and could
not be repaired by merely adding the five EP1 attributes. It contained a
non-native `Name`, embedded object-status masks instead of pointers, one
combined anti-mask instead of required/forbidden masks, and omitted
`VirtualDamage`, `PreAttackWeapon`, `WeaponAiHintInfo`, and the incompatible
modifier list. The rebuilt structure matches all recovered RA3 offsets, then
inserts Uprising's two loop-sound references and three booleans for a 340-byte
EP1 layout. The compiler PoC verifies the Uprising-only `ScatterAlways`,
`ProjectileSelfUsesPathfinder`, `UpdateBarrelModelConditions`, expanded weapon
flags/anti-mask enums and `VirtualDamage` (`340 bin / 0 relo / 0 imp`).

`GameObject` is the fourth audited root type. Metadata from EA's RA3 compiler
establishes the original 592-byte layout and exposes several fields that were
missing, misplaced, or KW-specific in the inherited source. The rebuilt model
restores the transformed description/image fields, voice-transition timeouts,
path priority, invisibility opacity, health-bar and subgroup data, resource
costs, EVA fields, `UnitSpecificFX`, and the native list/pointer ordering. It
also removes the inherited root `WeaponSet`, dead-collision, display-damage,
build-cost/threat and selection-decal fields that are not present in the RA3 or
EP1 root schema.

EP1's `KindOfType` has exactly 291 values and therefore needs ten 32-bit spans.
Because `GameObject` embeds two `KindOfBitFlags` values, each grows from 36 to
40 bytes and the EP1 root becomes 600 bytes. The enum synchronizer now owns the
complete `KindOf`, `UnitCategory`, `WeaponCategory`, `BuildPlacementType`,
`BuildableStatus`, and `SkirmishAIBaseLocation` order and bit counts. This also
adds the EP1-only `RADIATION` weapon category and `BLOCKED` build-placement
value.

The nested EP1 `CrusherInfo` and `ProjectedBuildabilityInfo` changes have been
ported from the official XSD delta. Their provisional native layouts are
locked as 52 and 116 bytes respectively; these sizes combine recovered RA3
metadata with EP1 bit-count and member-order changes and still require direct
EP1 binary/disassembly confirmation. The compiler PoC now emits a 768-byte
`GameObject` graph with the expected nested pointers/list and a 12-byte
relocation stream. Numeric parsing is invariant-culture throughout the core
marshaller, preventing decimal values such as `0.25` from becoming `25` on a
Turkish-locale host.

The first low-risk EP1-only behavior-module group is also wired into the
`BehaviorModuleData` polymorphic dispatch table. `SpawnedSlaveUpdate` is a
fieldless specialization of the existing 112-byte `SlavedUpdate` layout;
`GenericUnpackUpdate` and its fieldless `UnitUnpackUpdate` specialization use a
20-byte layout; and the Lift/Lure special-power modules append one 32-bit link
ID to the existing 256-byte `StoreObjectsSpecialPower` base. Compiler tests use
values taken from the official Desolator, Giga Fortress, and Yuriko XML and
verify the resulting 112-, 20-, and 260-byte chunks. These ports establish the
registration/marshalling pattern for new EP1 module types; modules with novel
nested data still require stronger native-layout evidence before being added.

`AudioDynamicsCollide` is the first nested EP1-only module recovered directly
from a real tokenized Uprising `GameObject` chunk. A bounded scan of
`GameObject:ClientFlingableExplodingBarrel` found type ID `0xD6C03AC2` at the
module data offset. The following bytes encode the module ID, the XML's `5.0`
minimum impact velocity, a four-entry merged selector list, and four 8-byte
`MinimumMagnitude`/audio-reference entries. The recovered native layout is a
20-byte root (`BehaviorModuleData`, float, list) plus 8 bytes per selector
entry. A minimal two-entry compiler fixture therefore emits `36 bin / 8 relo /
0 imp`, matching the recovered field ordering and sizes. `asset-bytes` now
supports `--find-u32 <hex>` so this analysis remains a bounded random-access
operation rather than a dump of the 1.4 GB archive.

The adjacent `DamageDynamicsCollide` recovery also corrected a major inherited
KW layout error shared by weapon nuggets. EA's official RA3 Tokenizer metadata
declares `WeaponEffectNugget=40` and `DamageNuggetType=152`; the previous source
used 152 and 240 bytes respectively. The base now uses optional pointers for
the two large status/model-condition masks, restores its `Radius`, and removes
the KW-only required-status mask. EP1 expands `ObjectStatusBitFlags` from seven
to eight spans, so its embedded `InvalidTargetStatus` grows the damage nugget
to 156 bytes. In the real Uprising fixture the 20-byte collision-module root
and two merged 156-byte nuggets occupy exactly the bytes up to the following
audio module. The compiler fixture emits one nugget as `176 bin / 8 relo / 0
imp` and checks the EP1 tail-boolean offsets.

`ReactionFXOnDamage` is the next EP1-only module recovered from the raw static
stream. `GameObject:JapanYurikoTech1` contains type ID `0x8C3B49E2`, followed
by list counts 5 and 2 exactly matching the official XML's damage and healing
triggers. Pointer targets and scalar payloads prove a 24-byte trigger layout:
five optional pointers (threshold, timer, source filter, voice hash, sound)
followed by the reset-timer boolean at offset 20. The module root is 24 bytes
(`DamageModuleData` plus two lists). The real payload contains the expected
`99.99/5`, `75/3`, `50/3`, `40/10`, `33/5`, and healing `2` values, including
the source-filter/sound references and reset flag. A standalone compiler
fixture emits the expected 92-byte graph and 32-byte relocation stream. The
polymorphic dispatch table now registers the recovered type ID.

`DamageSphereUpdate` and its `SphereModuleUpdate` base are recovered from
`GameObject:AlliedFutureTankNeutronScramblerNode`. The type ID is
`0x878CEA1F`; the binary contains the XML's radius `180/25`, scan/duration
`1/10`, sphere scale `16`, unpack time `1.35`, expansion `125`, object-filter
bits, four model/status masks, and `SHIELDLARGE` payload at the predicted
locations. This establishes a 176-byte sphere base and 372-byte damage module;
the string payload makes the standalone compiler fixture `384 bin / 8 relo / 0
imp`.

This recovery also corrected the shared KW-derived `ObjectFilter`. The old
model was 296 bytes and embedded KW status/model-condition masks. EA's official
RA3 compiler declares 112 bytes and both clean RA3/EP1 XSDs define two optional
status-mask attributes. The native representation uses pointers for those
masks and has no KW model-condition fields. EP1's wider pair of `KindOf` masks
makes the corrected EP1 structure 120 bytes, exactly matching the sphere
fixture. Because this type is widely embedded, the full compatibility suite is
required after the correction and passes.

The shield branch is now extended through `ShieldSphereUpdate` and the EP1-only
`YurikoShieldSphereUpdate`. `GameObject:YurikoShieldProp` proves type ID
`0x95D855B6`, a 312-byte shield base and a 328-byte Yuriko root. Its real
payload contains the expected `24/24` radii, `0.25/10` timing, `1e10` maximum
damage, status/model bits, two FX references, and minor-damage mask
`0x00626008`. The compiler fixture reproduces a `460 bin / 12 relo / 0 imp`
graph including `SHIELDSMALL` and the 120-byte nested ignore filter.

The same fixture exposed the inherited KW `DamageType` ordering. It has been
replaced with the official 39-value EP1 order (`MELEE` through `NEUTRON`,
including `RADIATION`), while preserving the two-span native mask. `SageBinaryData`
and `BinaryAssetBuilder.XmlCompiler` now explicitly compile with `VERSION7`,
matching Utility, Core and AudioCompiler. The inspector's new `hash` command
prints FastHash type IDs used for bounded module discovery.

The independently supplied clean RA3 baseline at
`D:\OneDrive\CNC Files\CnC_Modding_Support-main (Official XML, Schema, Script, Shader, Maps)\Red Alert 3\Schemas (RA3)`
contains 821 XSD files and is structurally identical to `schemas/ra3/xsd`.
Known MP3-enabling SDK changes should remain an explicit SDK-extension layer;
they must not be counted as RA3-to-Uprising engine/schema differences.

`asset-bytes` performs bounded random-access reads from very large raw BIN files
or uncompressed BIG entries. It was used against the local 1.2 GB RA3
`worldbuilder.bin` and Uprising's 1.4 GB `WBData.big` without making a complete
binary dump. The shared sample `AttributeModifier_MechaKingSquishKillDelay` is
an 88-byte tokenized instance chunk in both games while its type hash changes
from `0xF901FE9B` to `0x74425C11`; tokenization means this observation does not
replace the native-layout checks above.
The command also accepts bounded `--offset` and `--count` ranges (maximum 16
KiB), allowing pointer payloads inside one selected asset to be decoded without
ever dumping an entire multi-gigabyte stream.

## Compiler work still required

The Uprising branch now builds its Utility, Core and AudioCompiler projects with
`VERSION7`. The writer emits the 4-byte EP1 prefix, reordered v7 header and
48-byte tokenized asset entries. Its output round-trips through both the new
inspector and the production Utility reader. The production reader also passes
against `worldbuilder`, `static` and `global` manifests extracted from the real
game archives, including RefPack inputs.

Remaining work:

1. Port the 22 new schema types and every binary-layout-affecting change among
   the 75 changed schemas into `SageBinaryData` and the processor registry.
2. Generate the Uprising type table and require the final
   `AllTypesHash=0x5454A8E9`; a schema-valid XML build is not sufficient.
3. Validate `.bin`, `.relo` and `.imp` chunks asset-by-asset against a known
   Uprising fixture. Manifest compatibility alone cannot prove binary layout.
4. Add target-aware SDK build scripts and settings for schema root, XML source
   root, output root, registry discovery and WorldBuilder data packaging.

The machine-specific KW dependency fallback has been removed. External
precompiled dependencies are now configured with the `ExternalManifests`
settings attribute or `/em` command-line option. Relative paths resolve from
`DataRoot`; multiple paths are separated with semicolons. The resolver parses
v7 manifests and matches exact `(TypeId, InstanceId)` pairs rather than scanning
arbitrary binary bytes for strings.

Example:

```xml
<Settings ExternalManifests="Base/global.manifest;Base/static.manifest" />
```

Production output also has a fail-closed gate: a `VERSION7` build refuses to
commit a manifest unless the active compiler plugin reports
`AllTypesHash=0x5454A8E9`. The current XML plugin still reports the KW hash, so
it cannot accidentally publish a v7 wrapper around KW binary layouts while the
EP1 type port is incomplete.

## Acceptance gates for the first PoC

- Compile one Uprising-only asset type from a minimal XML fixture.
- Emit manifest version 7 with the Uprising all-types hash.
- Produce `.manifest/.bin/.relo/.imp` files that pass the inspector.
- Load the stream in Uprising without an asset/type-hash rejection.
- Rebuild the same input twice and prove byte-for-byte deterministic output.
- Test a reference to an asset in `global.manifest` and one patch reference.

Until these gates pass, RA3 XML/assets -> Uprising XML/assets replacement must
be treated as source/schema staging, not as a completed SDK conversion.
