# EP1 AssistedPlacementBehavior

There was no previous model for this family. `AssistedPlacementBehaviorModuleData`
is a 20-byte EP1 record (stock `GameObject:SovietRefinery` witness):

| Offset | Field | Storage |
| --- | --- | --- |
| 0, 4 | Module type ID `0xAE0EE9A0`, local ID hash | |
| 8 | PlacementHint | `AssistanceHintBitFlags`, one word; ordinals NONE=0, FACING_TOWARDS=1, FACING_AWAY=2, ALLOW_SHROUDED=3 |
| 12 | SnappingDistance | SageReal, default 0 |
| 16 | RequirementObjectFilter | pointer to the shared 120-byte ObjectFilter record |

Stock "FACING_TOWARDS ALLOW_SHROUDED" stores bits 1 and 3, which fixes the
ordinals. Metadata is 132 bytes without a filter and
`264 + 12·(1+include) + 12·(1+exclude)` bytes with one (288 on the refinery).
`AssistedPlacementContract` writes it literally and requires generated metadata
to equal stock.

## Supported scope

- Hint mask, snapping distance and the full optional filter (rules, masks,
  status masks, source-sized include/exclude lists), defaults and multiple instances.
- Refused: an authored filter `id`.
- Unverified assumption: an omitted PlacementHint uses the schema default `NONE`,
  encoded as bit 0 like other flag defaults in this compiler; the stock refinery
  authors the value explicitly.

## Verification — 2026-10-23

- Fresh toolchain build succeeded; SDK compiler synchronized afterwards.
- Stock qualification reproduced the refinery payload and 288-byte metadata.
- Source controls rejected invalid hint/KindOf/distance values and an authored filter id.
- Three complete-object stream/BIG cases passed (`placement-edited`,
  `placement-defaults`, `placement-multiple`); 115 native and 86 metadata
  corruptions were rejected.
- production/repair/engineer batches and the original Conscript regression also passed.
- Evidence: `alpha/.work/placement-20261023-053426` and sibling runs with the same stamp.
- Validation-only Reborn preparation now reaches
  `SovietRefinery.xml(60,3): InfiltratorContain`.

Tests did not launch a game or rebuild/publish the user's Reborn package.
