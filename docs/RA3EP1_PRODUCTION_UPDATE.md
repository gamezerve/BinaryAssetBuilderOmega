# EP1 ProductionUpdate

EP1 `ProductionUpdateModuleData` is not the KW record. The legacy model carries
`DoorOpeningTimeMetaUnit`, `SetBonusModelConditionOnSpeedBonus`,
`SpeedBonusAudioLoop` and ProductionModifier `HeroPurchase`/`HeroRevive`; EP1 has
none of them and instead adds `NewObjectFadeInTime` plus the optional
`GiveUpgradeToProduced`/`UpgradeTranslate` record. The EP1 `ProductionQueueType`
also replaces `SPECIALPOWERS`/`BOOKMARKS` with `WATERCRAFT`. The isolated model is
`Ep1ProductionUpdateModuleData`; the legacy KW model and marshaler are unchanged.

Layout source: the stock `GameObject:SovietRefinery` instance slice, read from
`static.bin` at the manifest-computed offset, including its own reflective
metadata rows (field name, wire type, native target).

| Offset | Field | Wire type | Default |
| --- | --- | --- | --- |
| 0 | Module type ID `0x6EC75439` | SageInt | |
| 4 | Authored local module ID hash | | |
| 8 | MaxQueueEntries | SageUnsignedInt | 99 |
| 12 | NumDoorAnimations | SageInt | 0 |
| 16 | DoorOpeningTime | Time | 0s |
| 20 | DoorWaitOpenTime | Time | 0s |
| 24 | DoorCloseTime | Time | 0s |
| 28 | ConstructionCompleteDuration | SageUnsignedInt | 0 |
| 32 | DisabledTypesToProcess | DisabledBitFlags (one word) | empty |
| 36 | UnitInvulnerableTime | SageUnsignedInt | 0 |
| 40 | SpecialPrepModelconditionTime | SageUnsignedInt | 0 |
| 44 | BonusForType | GameObjectWeakRef | absent |
| 48 | Type | ProductionQueueType (`VEHICLE`=3 observed) | INVALID |
| 52 | ProductionTimeDelayScalar | Time | 0s |
| 56 | NewObjectFadeInTime | Time | 0s |
| 60 | QuantityModifier list (count, items) | | empty |
| 68 | ProductionModifier list (count, items) | | empty |
| 76 | GiveUpgradeToProduced pointer | | null |
| 80–83 | GiveNoXP, VeteranUnitsFromVeteranFactory, SecondaryQueue, IgnorePreReqs | SageBool | false |

The record is 84 bytes. Metadata is 408 bytes: a 12-byte concrete wrapper, the
22-row own table in schema order (17 attributes, then the three elements),
the Update/Contain/Behavior/Module base chain, and two one-row `Count` tables
for the empty lists. `ProductionUpdateContract` writes this literal metadata
independently, and its stock qualification requires byte equality with the
refinery after coordinate rebasing (checked offline before implementation).
The generic behavior tokenizer must produce the same bytes.

## Supported scope

- All 16 fixed scalar attributes above are authored values with schema
  defaults, including WATERCRAFT and NewObjectFadeInTime. Multiple local
  instances are independent.
- `GiveUpgradeToProduced` is now supported. It is optional; when present, its
  source-sized `UpgradeTranslate` list contains one or more rows, following the
  official XSD. Each row has independently optional `RequiredUpgrade` and
  `ProvidedUpgrade` strong references. Nonempty targets resolve against authored
  and stock upgrade assets; omitted references remain zero without imports.
- Still refused with a source-positioned error: `BonusForType`, any
  `QuantityModifier` or `ProductionModifier` child. Their populated item layouts
  are not qualified by this change.
- `DisabledTypesToProcess` is authorable only through the existing reviewed
  DisabledType flag codec; without it the word must stay empty.

Implementation: `Ep1ProductionUpdate.cs`, `Marshaler.Ep1ProductionUpdate.cs`,
`ProductionUpdateContract.cs`, tokenizer family/signature registration and the
`production` module-graph batch (`production-edited`, `production-defaults`,
`production-multiple`, plus source refusal controls).

## Verification

<!-- Reborn: the Barracks' production translations extend the earlier scalar-only scope without changing or omitting the authored XML. -->
## GiveUpgradeToProduced extension

The original `SovietBarracks` production module independently qualifies the
84-byte header, optional eight-byte GiveUpgradeToProduced header and eight-byte
translation rows against real stock bytes and import identities. The actual
`UpgradeTranslation[]` wire signature is `0xB38AB380`. Both refinery (absent) and
Barracks (three populated pairs) qualify the same source/native contract and
stock-learned recursive metadata grammar.

The producer reuses the enclosing Tracker and real Core reference occurrences;
it never copies the stock three entries. The new complete graph proof verifies
every header field, pointer/list count, upgrade target, padding byte, relocation,
import and metadata coordinate. Metadata grows with authored rows instead of
requiring the old 408-byte empty-profile size. Schema defaults, edited scalar
values, repeated modules and optional translation reference presence compose.
The pre-existing literal empty-profile metadata qualification remains in place.

The `production` batch covers omitted translations, three edited pairs and two
independent modules with five and one rows, including an omitted RequiredUpgrade.
Header/list/import corruptions are negative controls. A present but empty
GiveUpgradeToProduced remains an XSD error, not a silently dropped wrapper.

Reborn with SovietBarracks and SovietRefinery now prepares successfully across
17 source files. The next real native build stop is the original Barracks frozen
draw `Material` (`ObjectsFrozen.fx` and its polymorphic shader constants), at
`alpha/mods/Reborn/data/Soviet/structures/SovietBarracks.xml:194`.
This is not an invalid XML or missing include. Material tokenization is not
implemented by this production change; the source was not commented or replaced.
Evidence for this distinct next stop:
`alpha/.work/barracks-translations-20261023-b`.
No full Barracks package/gameplay success is claimed.

<!-- Reborn: report measured complete-object proof separately from the next unrelated Barracks draw failure. -->
The extended production batch passes all three complete-object stream/BIG cases,
rejecting **375 native and 98 metadata faults**. The edited case has 281 strong
reference occurrences; the five-plus-one-row/multiple-module case has 286. The
default case remains at 275, with no translation pointer/imports. Evidence:
`alpha/.work/production-translations-batch-20261023-b/module-graph-report.json`.
The original empty-profile regression also passes separately in
`alpha/.work/production-translations-default-20261023-a` (89 native/27 metadata
faults). All checks use private output and leave both live mod source trees,
Documents packages and game installation files unchanged.

## Verification — 2026-10-23

- Fresh toolchain build: succeeded, no errors; SDK compiler synchronized afterwards.
- Source controls rejected BonusForType, a QuantityModifier child, uint overflow,
  legacy `SPECIALPOWERS` and an invalid Time value.
- Three complete-object native/metadata/stream/BIG cases passed
  (`production-edited`, `production-defaults`, `production-multiple`).
- 351 native and 98 metadata corruptions were rejected.
- The unchanged original Conscript stream/BIG regression passed.
- Evidence: `alpha/.work/production-20261023-033417` and
  `alpha/.work/production-original-20261023-033417`.
- Validation-only Reborn preparation now reaches
  `Includes/GenericBuildingRepair.xml(3,3): RepairUpdate`.

Tests did not launch a game or rebuild/publish the user's Reborn package.
Offline validation is not a gameplay test.
