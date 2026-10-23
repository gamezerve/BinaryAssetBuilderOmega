# EP1 InfiltratorContain

The previously recovered 136-byte `InfiltratorContainModuleData` model already
matches the stock `GameObject:SovietRefinery` record field for field; this step
admits it and adds an independent contract. Metadata is 336 bytes.

| Offset | Field | Storage |
| --- | --- | --- |
| 8 | BlockedDuration | Time, default 2s |
| 12 | InfiltratedStatusFilter | inline 32-byte status mask, default `DESTROYED UNDER_CONSTRUCTION SOLD UNPACKING` |
| 44, 48 | CanEnterFilter, ReplaceWith | inline ObjectFilterAsset / ObjectCreationList imports |
| 52, 56 | voices (HostileEnter, InfiltrationDeath) | inline StringHash (case-sensitive FastHash) |
| 60, 64, 68 | two EvaEvents, FXForInfiltrate | inline imports |
| 72–84 | Duration, UnitDuration, Amount, Effect | Time, Time, SageReal, `InfiltratorEffectType` |
| 88 | ObjectRef | weak GameObject ID (lowercase FastHash) |
| 92, 96 | UnitFilter, StructureFilter | optional pointers to an ObjectFilterAsset import |
| 100 | ObjectStatusToSet | inline status mask, default NO_REFUND |
| 132 | ImmediatelyEnabled | SageBool |

Several references have schema-default names (`InfiltrationCanEnterObjectFilter`,
`EnemyBuildingInfiltrated`, `OurBuildingInfiltrated`,
`FX_Building_Infiltrated_Generic`). An omitted attribute uses that default and must
resolve to a real Core import; an explicit blank value means no reference. The
`infiltrator-defaults` case proves the default imports are produced and resolved.

## Verification — 2026-10-23

- Stock qualification reproduced the refinery payload (default imports, voice
  hashes, STEAL_MONEY/2000/30s) and its 336-byte metadata.
- Source controls rejected invalid status/effect/time values.
- Three complete-object stream/BIG cases passed (`infiltrator-edited`,
  `infiltrator-defaults`, `infiltrator-multiple`); 561 native and 106 metadata
  corruptions were rejected.
- All earlier batches and the original Conscript regression still passed.
- Evidence: `alpha/.work/infiltrator-20261023-054644` and sibling runs.
- Validation-only Reborn preparation now reaches
  `Includes/GenericRefineryStructurePlacementBehavior.xml(3,2): StructurePlacementBehavior`.

Tests did not launch a game or rebuild/publish the user's Reborn package.
