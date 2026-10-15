# Selected armor/body/destruction/weapon GameObject composition

October 15, 2026. The combined partial GameObject writer now supports **one or two
selected behavior modules**, including both destruction and EP1 WeaponSetUpdate,
in either source order. Armor, health, weapons, upgrades and modifier references
share one actual Core occurrence table. This is not a complete published asset.

## Writer and native allocation

`Ep1GameObjectStringTokens.Inspect` decodes one/two ordered, non-aliasing behavior
pointers rather than fixing count=1 and the module at list+12. It requires the
canonical contiguous pointer array and exact root/list/item relocation ownership.
The first module begins immediately after that array; subsequent native owners
are increasing and bounded before the optional Body tail. Strings/ArmorSet and
their exact pre-module import/relocation ownership remain checked.

`Ep1GameObjectDestructionProjection.Compose` retains its existing public name,
but dispatches bounded native fragments explicitly by concrete ID:

- 0D96A2F4: reviewed destruction tokenizer.
- 431722A9: reviewed EP1 hardpoint WeaponSetUpdate tokenizer.
- All other module IDs fail; no legacy registry or guessed writer fallback.

Each native module is separately extracted and validated. Generated behavior
markers, fragment metadata targets, Body metadata and ArmorSet tables are joined
using their actual prefix sizes. Native pointers and RELO/IMP slot locations shift
once; stored strong occurrence indices never shift or get renumbered. Existing
single-destruction, body-only and armor/body regression contracts remain intact.

`Marshaler.MarshalEp1SelectedBehaviors` is an explicit opt-in allocator on the
same Tracker as the root and Body. It requires distinct nonempty local module IDs,
one/two reviewed classes and known internal TypeIds. `MarshalEp1WeaponSet` selects
the separate EP1 ABI/wire ID without altering legacy GameObject behavior dispatch.
No old list is allocated and then abandoned: the diagnostic adapter removes the
legacy behavior route before explicit allocation.

## Source/native/output evidence

`gameobject-combat-composition-self-test <static> <global>` runs sixteen combined
cases: two behavior orders, one/two weapons, zero/three body range modifiers and
absent/300-character root string allocation. A separate single-weapon behavior
case proves that an artificial destruction module is not required.

Raw source validates in the reviewed full GameObject schema candidate; actual
Core normalization then runs once over the focused envelope using official
reference types. Concrete target identities must uniquely exist in independently
validated static/global metadata. No source numeric suffixes are accepted.

Independent checks bind the root/count/marker/concrete native module chain,
weapon hardpoint/row fields, body and armor prefix extents, every ordered source
reference identity/index and the complete native/RELO/IMP inverse shift. Repeated
builds compare all three composed buffers. Eleven source/owner refusals and
thirteen detached composed metadata corruptions are exercised in stock mode.

`gameobject-authored-combat-composition-self-test <absolute-fresh-output> <static> <global>`
builds a real RebornComposedArmor stream and uses it in the same sixteen combined
cases alongside independently resolved stock weapon/upgrade/FX targets. Removing
the authored catalog refuses the same consumer; authored mode has twelve refusal
cases. The existing armor build may publish its own armor-only package, but no
consuming GameObject stream or mixed asset package is published here.

The two module token writers retain their separate complete selected stock
comparisons. The combined graph has authored schema/Core/native/writer evidence,
not full equality with a whole stock object or gameplay acceptance. The root
metadata count stays at **89 when Body and ArmorSet are present**; adding a second
behavior changes the list, not the number of root field names.

## Files and next gates

- `Ep1GameObjectStringTokens.cs`: bounded module array/owner decoding.
- `Ep1GameObjectDestructionProjection.cs`: selected writer dispatch and variable joins.
- `Marshaler.Ep1SelectedBehaviors.cs`: explicit EP1 native allocation and ID guards.
- `GameObjectBodyProjectionSmokeTest.Compile`: shared diagnostic opt-in path.
- `GameObjectCombatCompositionSmokeTest.cs`, `tests/fixtures/RootCombatPipeline.xsd`.
- Two additional Focused groups, bringing the batch to 38.

Remaining general GameObject work includes required helper/AI/draw/geometry and
other root graphs, additional module token writers, generic source/default/local-ID
closure, checked production registry/OutputManager integration, complete mixed
streams and new-unit gameplay. Turret/contained weapon families and general new
WeaponTemplate production are not enabled by this consumer module port. The alpha,
installation and overall completion estimate are unchanged by this diagnostic
composition milestone.

## Final regression

`artifacts/GameObjectCombatComposition-a/batch-report.json`: **38/38 Focused groups
pass in 104.923 seconds**. Stock/authored combat groups take 3.610/3.960 seconds.
The batch build succeeds in 1.22 seconds with zero warnings/errors; all eight
runtime hashes remain unchanged while workers run. GameExecuted and
InstalledFilesModified are false. No new Full-suite, consumer package or gameplay
result is claimed.
