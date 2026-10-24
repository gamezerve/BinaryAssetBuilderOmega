# Uprising airfield behaviors

`RecallUnitsSpecialPower` and `ParkingPlaceBehavior` use dedicated EP1 native
records instead of the older Kane's Wrath layouts. Their identities and
metadata are qualified against the original Soviet and Allied airfield modules.

- Recall has a 484-byte header. `MaxAircraftToRecall` is at offset 476
  (schema default 4); `PlayReturnVoiceFromRecalledUnit` is at offset 480
  (default true). The inherited SpecialPower fields and imports remain authored.
- Parking has a 60-byte header. EP1 has no `HealAmount` field here. It includes
  `DoorsUnpackOpen`, `QueueDistance`, `LimitedProductionAircraftSpots`,
  `HelipadBoneName` and `ProducedAboveHeight`, alongside rows/columns,
  approach height, door timings, flags, and optional `CanParkHereFilter` and
  `DefaultRallyPointOffset` records.

Schema-valid values, local module IDs and optional children are source-driven.
Compilation verifies their actual native bytes, allocations, reference indices
and relocations, then checks generated metadata against the retail class tables.
No stock module payload is copied into the authored output.

Developer regression: `airfield-behaviors-self-test <static.manifest>
<global.manifest>` covers original sources, edited values, schema defaults,
optional filter/vector records and deliberately corrupted output, through the
real polymorphic dispatcher. It does not change editable mods or launch a game.

Validation: both families matched two original retail modules each (Soviet and
Allied airfields); 12 original/edit/default/optional/corruption checks passed.
A private current Reborn build also passed: 55 source files, 37 explicit assets
including four GameObjects, and linked BIN/RELO/IMP readback. Editable Reborn
sources and published mod output were not changed. This is offline evidence,
not an in-game airfield behavior test.
