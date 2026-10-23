# EP1 EngineerContain

The EP1 `EngineerContainModuleData` record differs from the KW model: the
`ObjectStatusToSet` mask is inline (32 bytes), the four EVA events are inline
strong references instead of string pointers, and the `CanEnterFilter` pointer
precedes the packed boolean. The isolated model is `Ep1EngineerContainModuleData`
(92 bytes, stock `GameObject:SovietRefinery` witness); the legacy KW model and
marshaler are unchanged.

| Offset | Field | Storage |
| --- | --- | --- |
| 0, 4 | Module type ID `0x0FF760E0`, local ID hash | |
| 8 | ReplaceWith | inline ObjectCreationList import |
| 12, 16 | NameOfVoiceToUseForFriendlyEnter / HostileEnter | pointer to StringHash (case-sensitive FastHash) |
| 20 | ObjectStatusToSet | inline 32-byte status mask, default NO_REFUND |
| 52–64 | FXForRepair / Capture / Replace / CaptureAndReplace | pointer to FXList import |
| 68–80 | EvaEventFor… (four) | inline EvaEvent import |
| 84 | CanEnterFilter | pointer to 120-byte ObjectFilter |
| 88 | ImmediatelyEnabled | SageBool |

The ObjectFilter pointee holds Rule, Relationship, Alignment, Include/Exclude
KindOf masks, optional status-mask pointers and IncludeThing/ExcludeThing weak
lists (lowercase FastHash). Metadata is 288 bytes without a filter and
`420 + 12·(1+include) + 12·(1+exclude)` bytes with one (480 on the stock
refinery). `EngineerContainContract` writes it literally and also requires the
generated metadata to equal stock.

Blank optional names and the schema's empty EVA default are absence: no pointee,
no import. Strong FX/EVA/OCL names must resolve to exactly one schema-compatible
catalog asset at their exact Core reference index.

## Supported scope

- All attributes and the full CanEnterFilter (source-sized include/exclude lists,
  optional status masks), defaults and multiple instances.
- Refused: an authored CanEnterFilter `id`.
- Not exercised by tests yet: an authored `ReplaceWith` and the
  missing/incompatible reference refusals (implemented in the contract).
- The variable-source alignment step skips `CanEnterFilter`; it carries only weak
  names and enum/flag values, no strong-reference occurrences.

## Verification — 2026-10-23

- Fresh toolchain build succeeded; SDK compiler synchronized afterwards.
- Stock qualification reproduced the complete refinery payload (voices, FX/EVA
  imports, three filter things) and its 480-byte metadata.
- Source controls rejected invalid status/KindOf values and an authored filter id.
- Three complete-object native/metadata/stream/BIG cases passed
  (`engineer-edited`, `engineer-defaults`, `engineer-multiple`); 409 native and
  96 metadata corruptions were rejected.
- `production`, `repair` and the unchanged original Conscript regression also passed.
- Evidence: `alpha/.work/engineer-20261023-052203` (plus production/repair/original runs with the same stamp).
- Validation-only Reborn preparation now reaches
  `Includes/GenericRefineryPlacementHint.xml(3,2): AssistedPlacementBehavior`.

Tests did not launch a game or rebuild/publish the user's Reborn package.
Offline validation is not a gameplay test.
