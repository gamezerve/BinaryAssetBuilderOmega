# Sound, shadow and projected buildability roots

<!-- Reborn: document independently verified source ownership without claiming unrestricted GameObject or runtime support. -->
The complete GameObject route now compiles source-owned `AudioArraySound`,
`ShadowInfo` and repeated `ProjectedBuildabilityInfo` records. Their presence and
list lengths no longer have to match the captured Conscript.

- `AudioArraySound` may be absent, empty or populated. Its entries use EP1 sound
  ordinals and typed sound references, not the legacy KW string-pointer layout.
- `ShadowInfo` may be absent, empty/defaulted or populated with implemented
  schema fields, including optional texture imports.
- `ProjectedBuildabilityInfo` is a source-sized inline root list. Each item has
  schema defaults, placement/status/model-condition flags and an optional
  supported `AllowedObjectFilter`. No synthetic list header is emitted into the
  native GameObject tail; metadata targets the actual root count/items slots.

Official schema validation still governs required fields, enums and ordering.
Native serialization and the independent verifier now agree that
`BuildPlacementType.INVALID` is ordinal zero; unlike selected other enum types,
it must not shift the remaining flag bits down by one.

## Verification — October 23, 2026

The `roots` filter of `conscript-module-graph-self-test` passed three complete
stream/package cases: refinery-shaped root presence; empty/default records; and
expanded sound, shadow, three projected records and optional filter. It rejected
51 native and 48 metadata corruptions. Evidence:
`alpha/.work/extended-roots-20261023-e`.

Original Conscript passed separately (5 native/10 metadata faults), as did
expanded Geometry (16 native/21 metadata faults). Each case includes stream
readback, deterministic fresh outputs and independent checksum/reference checks.
Sound/record metadata signatures and layouts are qualified against stock
Conscript, SovietPowerPlant and SovietRefinery captures, not generated solely
from the writer being tested. Historical diagnostic lanes are not a claim of a
fresh full-batch pass.

## Remaining boundary

The initial preparation-only inspection of the user's Reborn reached
`Soviet/structures/SovietRefinery.xml(92,4)`,
`SkirmishAIInformation/@BaseBuildingLocation=NEAR_RESOURCE_NODE`, where the
comparison contract expected `FRONT`. This boundary has since been removed by
the [Skirmish AI root contract](RA3EP1_SKIRMISH_AI_ROOT.md); the next observed
failure is omitted AIUpdate/UnitAITargetChooserData. Full refinery
compilation/publication is not proven.
No user XML, published mod or game installation was modified, and no game was
launched for these offline tests. General ART/W3X compilation remains separate.
