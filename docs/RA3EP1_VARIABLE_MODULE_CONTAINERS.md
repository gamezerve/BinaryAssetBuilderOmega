# Source-sized Draws and client containers

<!-- Reborn: document the actual compiler boundary, not a claim of arbitrary scripted-draw nested authoring. -->
## Scope

`Draws`, `ClientUpdates`, and `ClientBehaviors` no longer need the original
Conscript's module count or order. Reviewed complete leaf shapes may be removed,
reordered, repeated with distinct local IDs, or placed in empty containers.
Omitted and allocated-empty containers retain different native/metadata storage.
Fixed module attributes and compatible reference targets remain authored values.

This does **not** yet enable arbitrary model-condition/animation state lists or
every optional nested draw/client field. Such changes report the actual module
source file and line rather than a misleading Conscript count limit.

## Implementation

- `ConscriptVariableContainers` restores baseline containers only in comparison
  copies. Actual source, allocation and dependency occurrences stay unchanged.
- Admission selects a matching complete reviewed leaf independently for each
  source module. Reference-order alignment uses that selection, not its ordinal
  in the baseline container.
- `ConscriptVariableGraphVerifier` independently walks each native leaf against
  its stock witness. Each leaf's metadata is checked against its own stock block,
  with separately bound native/table coordinates. Repeated templates cannot
  collide in a global old-address map or alias each other's metadata.
- Actual list headers, wrappers, complete native allocation, RELO/IMP ownership,
  reference occurrences, and complete metadata ownership remain mandatory.
- The original-object metadata producer emits null root declarations for omitted
  containers. An allocated-empty container instead owns an eight-byte native
  header and a twelve-byte count metadata row.
- Historical 64-entry draw/client tokenizer and 128-entry module-address quotas
  are replaced by pointer-array bounds derived from the allocated buffer. Existing
  total byte/depth bounds and schema/class checks remain in force.
- Malformed pointers/counts are checked before narrowing, so corrupt data produces
  an explicit validation error instead of an incidental integer overflow.

## Reborn boundary

The preparation-only diagnostic now identifies
`alpha/mods/Reborn/data/Soviet/structures/SovietRefinery.xml(100,5)`:
the first `ScriptedModelDraw` has nested building draw/state structure beyond the
currently qualified leaf shapes. Six Draws and omitted ClientUpdates are no
longer the admission boundary. Full refinery compilation/publication is **not**
claimed. No user XML, Documents mod package, or installed game file is changed.

## Regression

The `containers` group in `conscript-module-graph-self-test` exercises reversed,
expanded, omitted, allocated-empty and 129-update containers through actual Core/native,
metadata, linked streams, and BIG readback, with deterministic recompilation and
corruption controls. The unchanged `original` group is a separate regression.
No game is launched by these checks.

Final verification: **5/5 complete-object stream/BIG cases passed**, rejecting
**64 native and 86 metadata corruptions**. The unchanged original Conscript case
also passed (five native and ten metadata controls). The 129-update case has
177 total modules, 114,936 metadata bytes, 43,968 native bytes, and 275 reference
occurrences. The expanded draw/client case has 593 reference occurrences.

Evidence: `alpha/.work/variable-containers-20261023-d/module-graph-report.json`
and `alpha/.work/variable-containers-original-20261023-d/module-graph-report.json`.
The final compiler build succeeded with zero errors and two existing dependency
warnings. User source files, Documents outputs and game installations were not
modified.
