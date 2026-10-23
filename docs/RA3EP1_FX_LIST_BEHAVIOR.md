# Source-sized FXListBehavior

The existing EP1-compatible native writer now supports variable nested source
instead of matching only the Conscript's three example death conditions.
`BaseStructure`'s `DeathTypes="ALL"`/building explosion is accepted without
changing the original XML or freezing it to the Conscript's CRUSHED/SLAUGHTERED
examples.

## Supported source

- Required DieMuxData: death/forbidden-death masks, damage threshold, angle
  bounds, optional exempt/required status allocations and ASCII veterancy labels.
- One or more ordered Event records, with source-driven count and no sample
  count quota. Each supports Index, optional FXList target, thresholds, direction
  and both boolean flags. Omitted values follow official schema/native defaults.
- All fourteen official EP1 event kinds. The four additional enum literals are
  appended without changing the first ten native ordinals: onTerrainCollision,
  onShieldSphereCollision, onSurfacing and onSubmerging.
- Multiple independently named FXListBehavior instances in a complete object.

This does not compile new FXList resources: a populated Event/@FX must resolve
to a unique compatible asset in the available stock catalog. No FX value is
required to match the Conscript example. Existing flag-codec restrictions, such
as unsupported prefixed +/- operations, still give explicit source errors.

## Native and metadata checks

The existing header is 60 bytes: Module/Behavior/Die base storage followed by
the Event list header at 52/56. Events are 24-byte records. Status/veterancy
allocations, event arrays, strings, relocations and semantic FX imports are
checked independently against source; padding and unowned data are rejected.

All three original Conscript FX graphs are separately compiled and compared to
stock native data, semantic dependencies and every metadata tag/target. A new
literal metadata writer independently checks variable Event/veterancy counts,
optional status pointers, table terminators and native field targets against
the production schema-driven tokenizer. Compilation is not its own value oracle.

## Verification — 2026-10-23

- Toolchain build succeeded; final build had zero warnings/errors.
- Three complete native/metadata/stream/BIG cases passed: exact building death
  shape, fourteen-event expanded death/status/veterancy data, and multiple
  modules including defaulted records without FX imports.
- 121 native and 90 metadata corruptions were rejected.
- Evidence: `alpha/.work/fx-list-20261023-a/module-graph-report.json`.
- The unchanged original Conscript stream/BIG regression passed, including its
  existing 584 stream/dependency/profile refusal controls; evidence is under
  `alpha/.work/fx-list-original-20261023-a`.
- Validation-only Reborn preparation reaches
  `BaseObjects/BaseStructure.xml(47,5): SecondaryDamageTracker`, the next nested
  layout that still needs source-sized admission/native verification.

Implementation: `FXListBehaviorContract.cs`, source matching/alignment and the
complete graph verifier. The native layout/writer is reused, not replaced.
Tests use isolated output; they do not modify Reborn sources, publish its mod,
launch the game or alter installed files. Offline proof does not establish
runtime behavior for the newly enabled event kinds.
