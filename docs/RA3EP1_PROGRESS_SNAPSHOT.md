# Uprising SDK progress snapshot — October 15, 2026

## Scope and approximate completion

The target is the user's **full RA3-Mod-SDK-style Uprising authoring workflow**:
arbitrary supported asset IDs, new GameObjects/armors, instance Includes,
inheritance, definitions/expressions, art assets and familiar Studio diagnostics.
The existing limited local alpha is available now; that is not completion of the
full target.

For planning, the current subjective engineering estimate is **about 45% complete,
with a reasonable 40–50% range**. This is not a measured asset-coverage percentage,
test-pass ratio, time-spent ratio or release countdown. Many working components
have diagnostic/bounded evidence without general production integration. The
range could change when the first general GameObject output exposes new contracts.

Approximate maturity weighting, included so the estimate can be challenged and
updated against actual deliverables rather than conversation count:

| Workstream | Weight | Estimated engineering maturity | Main remaining gate |
| --- | ---: | ---: | --- |
| Source graph, schemas, Includes, inheritance, definitions | 20% | ~65% | Connect broader supported source semantics to general playable output; full schema/default diagnostics |
| EP1 identities, references, streams, build pipeline | 20% | ~65% | Recursive GameObject tokenization, mixed-type streams, cache/reuse and broader dependency closure |
| Gameplay types and modules | 25% | ~25% | General GameObjects/weapons, existing-model EP1 review, remaining modules and new consumer gameplay proof |
| Models, textures, audio/shaders, maps/WorldBuilder | 20% | ~20% | General TGA/DDS/W3X/custom-data production and map/editor integration |
| Studio, setup, portability and packaging | 10% | ~65% | General build UI/options, relocation/dependency setup and clear capability errors |
| Acceptance, gameplay matrix and distribution review | 5% | ~30% | Multi-new-object acceptance mod, clean-machine workflow and redistribution review |

The rounded weighted estimate is roughly 45%; the individual values are judgments,
not six independently measured completion counters. Infrastructure maturity does
**not** mean 45% of EA XML asset types are now build-enabled. The historical 52%
figures in archived notes used an older scope/calibration; they are superseded for
this full-product estimate, not evidence that completed code was lost.

## Already established

- Local EA-style Studio alpha with build logs, unsupported-input warnings/errors,
  corrected failed-build handling and reproducible bounded packaging.
- Positive user-observed armor overrides, GUN/CANNON separation and fixed AK-47
  damage modification. These are specific recipe observations, not every type.
- General armor identities and multiple-armor scoped Core builds, with a bounded
  source bridge for instance/definition Includes and reviewed inheritance.
- Extensive source/schema/dependency/checksum/stream diagnostics and batched tests.
- Two GameObject root inventories, six module-container topologies and selected
  stock/native module comparisons. New tracker/freefall and basic module ports.
- Standalone toggle AI/weapon native compilation; the enclosing toggle/state model
  and direct marshaler now reproduce three selected native graphs diagnostically.
- Destruction, TechUpgrade and LUAEventList native models/dispatch reproduce four
  selected stock graphs; no missing native model names remain in the original
  two-object inventory. This is not a complete compatibility or publication gate.
- Destruction named FX/defaults now pass actual Core normalization and complete
  selected native/import-identity comparison; repeated authored reference
  occurrences and missing/wrong/ambiguous dependency controls pass independently.

## Remaining work, in practical order

1. **Complete general toggle source preparation:** five selected literal modules
   now pass Core/native comparison, nonempty child-list stock proof and actual
   polymorphic dispatch (see the [dispatch milestone](RA3EP1_TOGGLE_DISPATCH.md)).
   Generic source ordering, definition/expression evaluation and authored local-ID
   context collection still need full source-graph integration.
2. **Complete the initial GameObject native/token graph:** review existing EP1
   models/processors and verify recursive field/token/relocation/import contracts.
   BuildingDestruction, TechUpgrade and LUAEventListBehavior now have selected
   native evidence (see [the module batch](RA3EP1_ALLIED_MODULES_NATIVE.md));
   selected destruction FX source normalization now has a focused Core gate,
   but inherited/nested tokenization and general source closure still need
   integration. No missing native model names in the
   initial inventory does not mean all those modules are EP1-compatible.
3. **Publish a general consumer:** build a new user-ID GameObject referencing a
   newly built armor/weapon in a mixed Core stream; validate default-reader package
   selection, dependencies/override precedence, then obtain controlled gameplay proof.
4. **Expose that shared path in Studio:** multiple input assets/files, instance
   Includes, inherit/definition semantics and actionable missing-definition,
   schema/XML/reference/art errors. Do not silently skip unsupported assets.
5. **Widen art and other asset processors:** textures/models first, then needed
   animation/material/shader/audio/custom-data closures and additional gameplay
   modules. Existing legacy source code is a starting point, not compatibility proof.
6. **Maps/WorldBuilder and release readiness:** map overrides/editor integration,
   portable dependency setup, clean extraction/build/launch workflow and license
   review. Test a representative multi-new-object mod against the final contract.

No honest calendar finish date follows from this percentage. A usable broader
gameplay alpha is a nearer milestone than complete SDK parity. Update this snapshot
when a gate actually opens, not whenever another regression or continuation passes.
