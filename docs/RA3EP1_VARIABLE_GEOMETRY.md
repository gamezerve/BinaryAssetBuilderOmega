# Source-sized GameObject Geometry

<!-- Reborn: Geometry follows authored schema/native records rather than the captured infantry shape count. -->
## Supported source

The complete-object compiler now accepts a present `Geometry` with the official
source-defined `Shape` and `ContactPoint` lists, optional `PlacementShape` and
`RotationAnchorOffset`, optional shape offsets/strings, and schema-defaulted
attributes. At least one `Shape` is required by the official XSD. Omitting the
entire Geometry owner remains unsupported on this complete-object path.

The original one-Shape Conscript comparison is removed. Historical tokenizer
quotas of 32 shapes and 128 contact points are replaced with native buffer/record
stride bounds. The existing one-megabyte Geometry byte bound remains; no engine
unlimited-size claim is made.

## Independent proof

- `GameObjectGeometryContract.Mask` restores only an owned comparison copy.
  Actual authored Geometry is never replaced in compiled output.
- `SchemaNativeContract.Verify` independently derives every native value from
  schema-qualified source, with exact byte/RELO/IMP allocation ownership.
- `StockMetadataGrammar` learns real Geometry/Shape/ContactPoint row names and
  type tags from Conscript, SovietRefinery, AlliedConstructionYard,
  JapanFortressShip, AlliedPowerPlantTurbine and NanoSwarmHiveEffect stock.
- Optional Vector2/Vector3 wire tags bind directly to independently proved native
  coordinates, including a RotationAnchorOffset absent from the selected stock
  sources. This is an explicit vector-hack ABI rule, not acceptance of arbitrary
  new pointer types or a claim of a populated stock rotation witness.
- Generated standalone Geometry metadata and the enclosing GameObject splice are
  checked separately. All other root owners retain their existing checks.
- The original refinery's four Shape records are additionally checked against
  real refinery stock native bytes before the regression compiles them in the
  complete Conscript graph.

## Reborn boundary

The read-only preparation diagnostic passes the previous four-versus-one Shape
failure. It next reports `SovietRefinery.xml(82,4)` at `GameDependency`: further
root-owner structure differs from the current baseline. Full refinery compilation
or publication is not claimed. User Reborn XML, Documents mod outputs, and the
installed game are not changed by these developer checks.

Later milestone: [GameDependency support](RA3EP1_GAME_DEPENDENCY.md) now passes
that prerequisite owner too. Current Reborn differences are AudioArraySound,
ProjectedBuildabilityInfo and omitted ShadowInfo; the earlier diagnostic above
is retained as the Geometry milestone's historical boundary.

## Test scope

The `geometry` module-graph group includes real four-Shape refinery geometry,
minimal schema defaults, and 33 shapes plus 129 contacts with optional placement,
rotation, strings, offsets, RING geometry and a nonzero angle. Each case builds
twice deterministically, verifies complete native/metadata ownership, packages
linked streams into a BIG, and exercises native/metadata corruption controls.
Original Conscript and edited scripted-draw regressions are separate checks.
These checks do not execute the game.

Final results: **3/3 complete-object stream/BIG cases passed**, with **48 native
and 63 metadata corruption refusals**. The expanded case has a 100,152-byte
metadata prefix, 46,652 native bytes and 275 external reference occurrences.
The original Conscript regression passed (5 native/10 metadata controls), and
the edited scripted-draw regression passed (221 native/58 metadata controls).
The final incremental build succeeded with zero warnings and zero errors.

Evidence: `alpha/.work/geometry-flexible-20261023-c/module-graph-report.json`,
`alpha/.work/geometry-original-20261023-b/module-graph-report.json` and
`alpha/.work/geometry-draws-20261023-c/module-graph-report.json`.
