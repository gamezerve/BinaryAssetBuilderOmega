# EP1 RepairUpdate

The EP1 `RepairUpdateModuleData` field order matches the shared model (stock
`GameObject:SovietRefinery` witness), but EP1's `HealAmountPerSecond` default
is 10, not the KW 90 compiled into the legacy marshaler. The isolated model is
`Ep1RepairUpdateModuleData`; the legacy KW model and marshaler are unchanged.

| Offset | Field | Wire type | Default |
| --- | --- | --- | --- |
| 0 | Module type ID `0x0A2B8743` | SageInt | |
| 4 | Authored local module ID hash | | |
| 8 | RepopThreshold | SageReal | 0.2 |
| 12 | HealAmountPerSecond | SageUnsignedInt | 10 |
| 16–28 | PercentOfBuildCostToRebuildInfo: Pristine, Damaged, ReallyDamaged, Rubble | Percentage (SageReal tag) | 50 |
| 32, 36 | SelfAudio: SelfRepairFromDamageLoop, SelfRepairFromRubbleLoop | AudioEventInfoRef | empty |
| 40, 41 | Toggleable, RepairableWhenDead | SageBool | true, false |

The record is 44 bytes (two padding bytes). Metadata is 312 bytes. The two
required child records are anonymous schema types; EP1 tags them as
`RepairUpdateModuleData::PercentOfBuildCostToRebuildInfo` and
`RepairUpdateModuleData::SelfAudio`. The behavior tokenizer now emits that
`Owner::Element` signature for anonymous inline records instead of refusing
them; named types keep their reviewed signatures. `RepairUpdateContract` writes
the literal metadata independently and its stock qualification requires byte
equality with the refinery (checked offline before implementation).

## Supported scope

- All four own scalars and all four inline percents are authored values with
  schema defaults; multiple local instances are independent.
- Populated `SelfAudio` references are refused at their source attribute until
  an import contract is added. `Includes/GenericBuildingRepair.xml` leaves them
  empty.

## Verification — 2026-10-23

- Fresh toolchain build succeeded; SDK compiler synchronized afterwards.
- Source controls rejected populated audio, an invalid percent, uint overflow
  and a missing SelfAudio record.
- Three complete-object native/metadata/stream/BIG cases passed
  (`repair-edited`, `repair-defaults`, `repair-multiple`); 191 native and
  102 metadata corruptions were rejected.
- The `production` batch (after the tokenizer change) and the unchanged original
  Conscript regression also passed.
- Evidence: `alpha/.work/repair-20261023-045142`, `alpha/.work/production-20261023-045142`,
  `alpha/.work/production-original-20261023-045142`.
- Validation-only Reborn preparation now reaches
  `Includes/GenericEngineerContain.xml(3,3): EngineerContain`.

Tests did not launch a game or rebuild/publish the user's Reborn package.
Offline validation is not a gameplay test.
