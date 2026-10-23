# GameObject GameDependency

<!-- Reborn: compile authored build prerequisites through the normal complete-object graph, not an absent-only infantry profile. -->
## Supported storage

The optional `GameDependency` owner now reaches the complete-object compiler.
Absent, authored-empty, and populated records remain distinct. Supported native
storage includes source-sized `RequiredObject`, `ForbiddenUpgrade`, and
`NeededUpgrade` weak-ID lists; optional model-condition/status masks; and the
nested `ObjectFilter` record with its independently supported fields. Unknown
source fields are not silently discarded.

These are typed **weak hashes**, not strong imports. Adding a prerequisite does
not create a GameObject/upgrade asset, a command, or a new import occurrence.
Assets and commands must still be explicitly authored; existing dependency audits
remain separate from this source/native serialization proof. Gameplay prerequisite
behavior is not newly runtime-tested here.

## Integration and proof

- `Ep1GameObjectRootOwnersProjection` owns the optional root pointer at native
  slot 412. The actual marshaler allocates the dependency and its children on the
  same Tracker as all other modules/root records. No prebuilt native chunk is
  appended to emulate a compiled dependency.
- The root field is named GameDependency, but its independently observed wire
  type is GameDependencyType (`6D7966DE`). Absent owners emit a null declaration;
  present owners emit their full independently checked metadata.
- `GameObjectDependencyContract` qualifies source/native derivation against the
  original SovietRefinery prerequisite and real stock bytes. Stock metadata
  grammar supplies its field/list rows; the refinery engineer's real filter
  supplies the shared ObjectFilter grammar.
- Named optional record pointers may reuse grammar learned under another owner
  of the exact same stock wire type. Flag-pointer metadata targets the exact
  independently source-proved native bits under its stock flag wire tag. This
  does not invent grammar for record types with no stock witness.
- Native fields, weak IDs, mask bits, lists, all bytes, relocations/imports,
  complete metadata and the enclosing root splice are independently verified.

## Regression

Four complete-object stream/BIG cases pass: empty dependency, original refinery
RequiredObject, expanded object/upgrade lists plus all three optional masks, and
a populated filter with include/exclude weak names. Recompilation is deterministic.
**191 native and 84 metadata corruptions** are rejected.

Original Conscript passes separately with the owner omitted. Edited scripted
draws (221 native/58 metadata controls) and expanded Geometry (16 native/21
metadata controls) also pass. The final incremental build has zero warnings/errors.

Evidence:

- `alpha/.work/dependency-root-20261023-b/module-graph-report.json`
- `alpha/.work/dependency-original-20261023-a/module-graph-report.json`
- `alpha/.work/dependency-draws-20261023-b/module-graph-report.json`
- `alpha/.work/dependency-geometry-20261023-c/module-graph-report.json`

## Next Reborn boundary

Read-only source preparation passes GameDependency. The remaining root-family
differences are populated/present `AudioArraySound`, `ProjectedBuildabilityInfo`,
and omitted `ShadowInfo`. Full refinery compilation/publication remains unproven.
Comparison diagnostics now list added/missing owner names and remap by expanded
name/local identity/same-family ordinal instead of shifted absolute positions.
User Reborn XML, Documents packages, and game installations were not modified.

The final diagnostic regression passes **28 checks**. The actual source error now
maps to `SovietRefinery.xml(834,4)` at AudioArraySound, rather than incorrectly
shifting to the following VisionInfo after comparison-only owner removal.
