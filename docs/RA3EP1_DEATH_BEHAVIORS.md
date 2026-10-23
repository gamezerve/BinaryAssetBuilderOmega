# Flexible CreateObjectDie and DestroyDie

Both families now accept authored DieMuxData rather than requiring the original
Conscript's frozen/psychic death examples. This covers the refinery's ordinary
ALL-death/forbidden-SUICIDED creation, its OVER_WATER variant, and the following
DestroyDie together.

## Supported source

- Required DieMuxData: death/forbidden-death flags, damage threshold, angle
  bounds, optional required/exempt status masks and ASCII veterancy lists.
- CreateObjectDie: optional/retargeted CreationList and independent source-sized
  strong UpgradeRequired/UpgradeForbidden arrays, preserving source order.
- DestroyDie: the same complete death-condition data without creation fields.
- Multiple local instances, optional/empty nested data and official defaults.

Populated strong references must resolve to unique compatible stock catalog
assets, with their exact emitted Core selectors checked independently. This
does not implement new ObjectCreationList/UpgradeTemplate asset processors.
Shared flag-codec limitations remain explicit errors, not skipped operations.

## Native and metadata validation

The existing isolated EP1 CreateObjectDie record is 72 bytes: 52-byte Die base,
CreationList at 52, UpgradeRequired header at 56/60 and UpgradeForbidden at
64/68. The legacy KW debris header is not used. DestroyDie is the 52-byte Die
base alone. Optional mask pointers, veterancy arrays/strings, strong reference
arrays, padding and exact RELO/IMP ownership are checked against source.

All three original Conscript creation/destroy witnesses are independently
recompiled and compared to stock native graphs, dependencies and complete
metadata. A separate literal metadata writer cross-checks variable list sizes,
optional status pointers and every field/table target against the production
schema-driven tokenizer. No native model/marshaler replacement was required.

## Verification — 2026-10-23

- Fresh compiler build succeeded with zero warnings/errors.
- Source controls rejected missing DieMuxData and invalid death/status/angle.
- Three complete native/metadata/stream/BIG cases passed, each with normal and
  water creation plus destroy: refinery values, expanded status/veterancy and
  strong upgrade arrays, and omitted/default values.
- 174 native and 210 metadata corruptions were rejected.
- Evidence: `alpha/.work/death-behaviors-20261023-a/module-graph-report.json`.
- Original Conscript stream/BIG regression passed, including the existing 584
  stream/dependency/profile refusal controls; evidence is under
  `alpha/.work/death-behaviors-original-20261023-a`.
- Reborn now passes Behavior-source admission and reaches the separate complete
  GameObject module-topology comparison. This is not proof that the full refinery
  native graph compiles or runs.

The topology diagnostic now reports per-container source/baseline counts and
module-kind mismatches rather than only a generic root failure. These baseline
counts describe the current verifier, not engine module limits.
For the current refinery source the actual differences are Draws=6 versus the
baseline's 4, and ClientUpdates=0 versus the baseline's 1. Counts must not be
silently forced to match; building module graphs need independent verification.
The diagnostic now resolves the real Draws source location,
`SovietRefinery.xml(99,4)`, rather than the comparison baseline's renamed asset
or a generic entry-document context.

Main implementation: `DeathBehaviorContract.cs` and source/native graph wiring.
Developer tests do not modify Reborn XML, publish its mod, launch the game or
change installed game files.
