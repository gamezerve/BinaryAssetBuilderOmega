# EP1 LUAEventList

The source element `LUAEventList` maps to `LUAEventListBehaviorModuleData`
(TypeId `0xFA6AC8B6`). The record is 16 bytes: the 8-byte module header and one
owned ASCII string, `EventListName` (length at +8, pointer at +12). The text and
its NUL follow the header, padded to 4 bytes. It matches the stock
`GameObject:SovietRefinery` record (`BuildingPowerFunctions`, 40 bytes).
Metadata is 108 bytes.

`EventListName` is required, must be non-empty printable ASCII, and the element
has no children. The SDK serializes the name only; it does not resolve or run
LUA callbacks.

The contract proves identity, the length/pointer header, every text byte and the
terminator, allocation coverage, the single relocation (no imports) and the
literal metadata writer, which is first qualified against stock.

## Verification — 2026-10-23

- Stock qualification reproduced the refinery `BuildingPowerFunctions` payload and
  its 108-byte metadata.
- Source controls rejected a missing, empty or non-ASCII name and a child element.
- Three complete-object stream/BIG cases passed (`lua-edited`, `lua-short`,
  `lua-multiple`); 141 native and 82 metadata corruptions were rejected.
- All earlier batches and the original Conscript regression still passed.
- Evidence: run 20261023-140601.
- Validation-only Reborn preparation now reaches
  `BaseObjects/BaseStructure.xml(43,5): FXListBehavior` (unimplemented nested layout).

Tests did not launch a game or rebuild/publish the user's Reborn package.
