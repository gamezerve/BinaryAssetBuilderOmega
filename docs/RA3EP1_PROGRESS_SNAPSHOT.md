# Uprising SDK progress snapshot — October 16, 2026

## Scope and approximate completion

The target is the user's **full RA3-Mod-SDK-style Uprising authoring workflow**:
arbitrary supported asset IDs, new GameObjects/armors, instance Includes,
inheritance, definitions/expressions, art assets and familiar Studio diagnostics.
The existing limited local alpha is available now; that is not completion of the
full target.

For planning, the current subjective engineering estimate is **about 48% complete,
with a reasonable 40–50% range**. This is not a measured asset-coverage percentage,
test-pass ratio, time-spent ratio or release countdown. Many working components
have diagnostic/bounded evidence without general production integration. The
range could change when the first general GameObject output exposes new contracts.

Approximate maturity weighting, included so the estimate can be challenged and
updated against actual deliverables rather than conversation count:

| Workstream | Weight | Estimated engineering maturity | Main remaining gate |
| --- | ---: | ---: | --- |
| Source graph, schemas, Includes, inheritance, definitions | 20% | ~65% | Connect broader supported source semantics to general playable output; full schema/default diagnostics |
| EP1 identities, references, streams, build pipeline | 20% | ~72% | Remaining recursive GameObject tokenization, mixed-type streams, cache/reuse and broader dependency closure |
| Gameplay types and modules | 25% | ~33% | General GameObjects/weapons, remaining models/modules and new consumer gameplay proof |
| Models, textures, audio/shaders, maps/WorldBuilder | 20% | ~20% | General TGA/DDS/W3X/custom-data production and map/editor integration |
| Studio, setup, portability and packaging | 10% | ~65% | General build UI/options, relocation/dependency setup and clear capability errors |
| Acceptance, gameplay matrix and distribution review | 5% | ~30% | Multi-new-object acceptance mod, clean-machine workflow and redistribution review |

The weighted estimate is 47.65%, rounded to roughly 48%; the individual values are judgments,
not six independently measured completion counters. Infrastructure maturity does
**not** mean 48% of EA XML asset types are now build-enabled. The previous 47%
planning estimate is revised after the common AI native/chooser port, complete
stock-matching generated metadata and shared AI/armor/body/behavior/movement/
geometry/vision composition with a genuine new armor target, not because the
test or field count increased. The earlier geometry checkpoint revised 45% to 47%.
The limited alpha still has its existing admission boundaries. The historical 52%
figures in archived notes used an older scope/calibration; they are superseded for
this full-product estimate, not evidence that completed code was lost.

## Already established

See the [capability inventory](RA3EP1_CAPABILITY_MATRIX.md) for separate large
asset-family and smaller/shared-subsystem status tables, including alpha versus
diagnostic and gameplay boundaries.

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

### First new-ID GameObject gameplay gate

The first practical custom-object target is a new user-chosen ID based on the
**complete Conscript graph**, reusing stock art/audio dependencies, with a visible
gameplay change and an explicit way to spawn/build it in a fresh match. The user
explicitly rejected a reduced substitute on October 16: do not remove modules to
reach an easier first gameplay milestone. This is not a promise of arbitrary
GameObjects, newly compiled models/textures, or every source feature at once.

Three deliverable gates remain before that controlled game test:

1. Close the selected object's recursive root/module graph, including required
   Draw/client containers and their dependency/token ownership. The current
   101-field selected root projection is not a complete publishable object.
2. Publish that new-ID GameObject and selected authored dependencies through real
   mixed Core streams, validating IDs, imports, relocations and metadata offline.
3. Package a reachable test object (build/spawn path), then obtain approval for
   one controlled launch and verify creation, appearance, movement, attack and
   the intended changed behavior in a fresh match.

The general SDK need not be finished first. A credible calendar date cannot be
derived from field/test counts: required Draw/client graph and first mixed-output
failures still determine effort. Re-estimate after the complete selected-object
offline publication gate, not after another individual scalar passes.

October 16 read-only target review: the complete stock Conscript has **4 Draws,
36 Behaviors, 1 AI, 1 Body, 1 ClientUpdate and 6 ClientBehaviors**. Its primary
`ScriptedModelDraw` source alone has 261 descendant elements. Its four source
Draw entries include an XInclude of `DATA:Includes/ParachuteDrawSoviet.xml`,
which expands into another scripted draw in the stock graph. All 49 module
instances and the include-expanded drawing/client dependencies remain in scope
for the complete Conscript target; a smaller passing projection is diagnostic
evidence only, not acceptance of the requested object.
The existing 216-byte scripted-draw and 196/140-byte model/animation-state models
are useful groundwork, not complete recursive Draw metadata/publication proof.
The [two tracer draws](RA3EP1_CONSCRIPT_TRACER_DRAWS.md) now have complete selected
stock native/metadata comparisons and actual Core texture resolution; 24 authored
slot/status/blend/FX graphs also check changed real/frame/color values. They are
not yet composed into a whole published object.
The [two complete scripted-draw native graphs](RA3EP1_CONSCRIPT_SCRIPTED_DRAW_NATIVE.md)
now pass actual Core named references and real Draw dispatch: 5+5 model states,
80+4 animation states, 70 string allocations and 171 fully consumed import slots.
The main graph matches stock; the parachute comparison explicitly admits only
one Script CRLF/LF equivalence, without changing source/producer text. Fourteen
stock particle metadata bindings exposed a wrong interior ABI/id-string type;
that defect and the omitted AnimationState NONE default are now corrected.
The [recursive metadata gate](RA3EP1_CONSCRIPT_SCRIPTED_DRAW_TOKENS.md) now passes
both complete stock prefixes (53,052/4,512 bytes), two full-source weak module-ID/
scalar edits, 66 malformed-native and 14,405 detached-output controls. The producer
uses no stock template and refuses currently unreviewed populated child families.
The [shared Draws container](RA3EP1_CONSCRIPT_SHARED_DRAWS.md) now joins all four
complete modules on one Core table/173 strong slots and one Tracker. Its complete
58,608-byte stock metadata prefix matches; 12 source cases and 14,682 negative
controls pass. The [selected root integration](RA3EP1_GAMEOBJECT_DRAWS_COMPOSITION.md)
now joins it with armor/movement/Body/three behaviors/common AI/geometry/vision
on one table and Tracker: up to 102 partial root fields/189 imports, 13 source
cases, cross-container identity and forward-link controls. This is not the
original whole Conscript: remaining root/behavior/client owners are next.
The actual XInclude/XPointer path, all client containers and remaining behavior/
root graphs still need integration; reading the two complete leaves separately
does not prove the include processor or whole-unit publication.
Implementing shared child writers and testing multiple selected graphs together
is preferred to stopping after each scalar or enabling a reduced alternative.
The legacy Draw dispatcher's silent unknown-type fallthrough is now replaced by
an explicit error with a refusal test; supported dispatch cases remain unchanged.

Latest checkpoint: [selected root + complete Draws](RA3EP1_GAMEOBJECT_DRAWS_COMPOSITION.md)
passes Focused **62/62** with explicit official XML root in **238.945s**,
root/Draws group **19.762s**, eight runtime hashes stable. The preceding shared
Draws checkpoint passed 61/61 in 225.340s; component metadata passed 60/60 in
265.734s. Both full scripted source graphs remain intact.
Whole-unit publication remains open. No Full run or new gameplay is implied;
full-product maturity remains about **48%**, not a type/field pass ratio.
The preceding [both complete Conscript tracers](RA3EP1_CONSCRIPT_TRACER_DRAWS.md)
pass Focused **58/58** in **368.540s**, with eight frozen runtime hashes stable.
This does not raise the full-product estimate or complete the enclosing object.
The preceding [group-audio client metadata fragment](RA3EP1_GROUP_AUDIO_CLIENT_TOKENS.md)
passes a complete stock prefix/native comparison, 12 author cases and 76 refusal
controls. Focused **57/57** passes in **289.857s**, with eight frozen runtime hashes
stable. The earlier timeout/default repair passes **56/56** in **368.721s**.
Neither checkpoint implies a Full run, whole GameObject publication or gameplay.

### Broader SDK work

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
   integration. The [selected destruction base review](RA3EP1_DESTRUCTION_BASE_TOKENS.md)
   now validates inheritance/terminal ID/inline DieMux tokens and selected
   empty/absent storage, but does not generate tokenized output.
   A separate [module-fragment writer](RA3EP1_DESTRUCTION_TOKEN_WRITER.md) now
   generates this reviewed graph from native output; general root/container
   composition, other module writers and shared stream ownership remain open.
   A [two-field root/container projection](RA3EP1_GAMEOBJECT_DESTRUCTION_PROJECTION.md)
   now composes the destruction fragment and native/auxiliary coordinates; it is
   deliberately not complete root token coverage or a publishable asset.
   The [scalar-root batch](RA3EP1_GAMEOBJECT_SCALAR_TOKENS.md) widens that projection
   with two-stock-audited scalar descriptors and batched explicit source/value
   checks; indirect/reference/string/enum/flag coverage and publication remain open.
   The [enum/flag/weak-ID batch](RA3EP1_GAMEOBJECT_ENUM_FLAG_WEAK.md) adds 15 fields,
   corrects scoped EP1 enum/default mismatches and verifies actual Core weak-ID
   conversion. Root metadata now covers 78 fields; strong references, strings,
   indirect graphs, target closure and publication remain open.
   The [root-string/ownership batch](RA3EP1_GAMEOBJECT_ROOT_STRINGS.md) adds six
   ASCII string headers and dynamic behavior/module allocation decoding (84 root
   fields); full encoding, strong root references and indirect writers remain open.
   The [strong root slot follow-up](RA3EP1_GAMEOBJECT_ROOT_STRONG_SLOTS.md) now adds
   three stock-audited root import descriptors (87 fields including Behaviors),
   exact root/module slot ownership and unchanged diagnostic reference indices.
   The [named source follow-up](RA3EP1_GAMEOBJECT_ROOT_NAMED_SOURCE.md) now checks
   actual Core shared icon/intro/FX name resolution against real catalogs;
   CrowdResponse has no target in the supplied catalogs and is a refusal case.
   General target payload/source closure remains open; the alpha is unchanged.
   A separate [EP1 ActiveBody writer](RA3EP1_ACTIVE_BODY_FRAGMENT.md) now matches
   two complete stock bodies and generates standalone inherited/range metadata.
   The [combined body/root writer](RA3EP1_GAMEOBJECT_BODY_COMPOSITION.md) now adds
   explicit EP1 allocation and disjoint root/destruction/body ownership with shared
   actual-Core names and 88 root fields when Body is present (87 otherwise).
   General source/processor integration and other body/consumer graphs remain
   open; legacy KW body dispatch and alpha are unchanged.
   The [ArmorSet composition follow-up](RA3EP1_GAMEOBJECT_ARMOR_COMPOSITION.md)
   joins ordered 0..32 armor records, optional DamageFX and a newly Core-compiled
   armor target to that graph (89 root fields with both present). Publication and
   gameplay of the complete consuming GameObject still remain open.
   The [EP1 hardpoint writer](RA3EP1_WEAPON_SET_FRAGMENT.md) now matches complete
   selected Peacekeeper/Conscript native/token graphs and actual Core references.
   The [combat composition follow-up](RA3EP1_GAMEOBJECT_COMBAT_COMPOSITION.md)
   now adds explicit selected allocation and one/two destruction/weapon behaviors
   in either order, sharing armor/body/root references and a real new armor target.
   Other object graphs, turret/contained and general consumer publication remain open.
   The [SMC helper follow-up](RA3EP1_SMC_COMBAT_COMPOSITION.md) adds independently
   stock-matching helper metadata and all six orders of the selected three-module
   graph, including a real newly compiled armor target.
   The [root movement follow-up](RA3EP1_MOVEMENT_COMPOSITION.md) adds scoped
   LocomotorSet records, independently stock-corroborated engine values and
   generated metadata to the same selected graph. General LocomotorTemplate/AI
   compilation, draw, other root/container graphs and publication remain open.
   The [selected module identity gate](RA3EP1_SELECTED_MODULE_IDENTITY.md) now
   rejects Body/behavior hash collisions and dangling AssociatedModule links at
   source and native boundaries; it is not the general cross-container resolver.
   The [geometry native port](RA3EP1_GEOMETRY_NATIVE.md) corrects the EP1
   PlacementShape/header difference and adds stock-proven placement/contact/RING
   graphs. The [geometry metadata follow-up](RA3EP1_GEOMETRY_TOKENS.md) now
   matches five complete selected stock prefixes.
   The [shared geometry composition](RA3EP1_GAMEOBJECT_GEOMETRY_COMPOSITION.md)
   joins those fragments with all selected root/movement/armor/body/behavior owners,
   including a genuine new armor target. Complete publication remains open.
   The [inline child correction](RA3EP1_INLINE_ROOT_CHILDREN.md) adds Buildable,
   ThingClass and packed DisplayColor without allocating legacy pointer payloads;
   the selected combination now has 94 partial root fields. General publication
   and independent nonzero inline-child runtime evidence remain open.
   The [pointed root children](RA3EP1_POINTED_ROOT_CHILDREN.md) add two name
   headers, Flammability and LiveCameraOffset to 288 selected shared-graph cases
   per stock/authored target mode, reaching 98 partial root fields. Positive stock
   names are verified; nonnull Flammability/camera stock evidence is still open.
   This does not change the approximate full-product maturity estimate.
   The [EP1 vision correction](RA3EP1_VISION_INFO.md) restores the missing height
   word, matches three complete stock native/table graphs and joins optional
   vision with 576 actual-Core combinations, reaching 99 partial root fields.
   Draw/AI/client and remaining root closures still block general publication.
   The [common AI native port](RA3EP1_AI_UPDATE_NATIVE.md) now matches two selected
   stock AI/chooser graphs with actual Core references and inline 0/1/2 AI dispatch.
   Generated AI metadata, nonempty vision filters, shared combat integration and
   other concrete AI classes remain open. This native-only milestone leaves the
   partial composed root at 99 fields and the approximate maturity estimate unchanged.
   The [generated AI fragment](RA3EP1_AI_UPDATE_TOKENS.md) matches both complete
   selected stock prefixes with empty contact/vision lists. Shared inline AI
   metadata composition, broader list elements and cross-container identity
   remain next; native-only nonempty contacts do not imply token support.
   The [shared inline AI composition](RA3EP1_GAMEOBJECT_AI_COMPOSITION.md) now
   closes that selected AI/root metadata and identity gate, with one actual Core
   occurrence table, 0/1/2 AI modules and stock/new-armor target modes. Both modes
   cover 864 combinations; the partial root reaches 100 fields. The native-only
   minimal-AI refusal becomes a positive projection control. Other AI/list/Draw/
   client/root graphs and complete asset publication remain open.
   The [duration default correction](RA3EP1_GAMEOBJECT_DURATION.md) adds the final
   reviewed flat scalar: the official 31000ms timeout is 155 stored frames, not
   raw milliseconds or the previous missing-attribute fallback 0. The selected
   root now reaches 101 fields with ten full shared-graph cases; this smaller
   scalar fix does not change the approximate full-product maturity estimate.
   No missing native model names in the
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
