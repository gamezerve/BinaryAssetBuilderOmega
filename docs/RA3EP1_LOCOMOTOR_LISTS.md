# Source-sized EP1 GameObject movement lists

<!-- Reborn: the implemented native movement list is driven by source cardinality, not the selected Conscript's one-row example. -->
GameObject may omit LocomotorSet or declare multiple entries. There is no
single-row admission rule or historical 32-row tokenizer quota. Checked buffer
size, twelve-byte record stride, schema types, pointer bounds and exact ownership
still apply. Other unimplemented list types are not automatically enabled.

Each row supports finite float32 Speed, the official EP1 Condition enum and
an optional typed LocomotorTemplate reference. Omitted scalar attributes retain
native zero; omitted references have no IMP entry. EP1-specific enum values such
as SPECIALABILITY_ACTIVE use Ep1RootLocomotorSetType, not the older KW enum.

ConscriptLocomotorSetContract independently verifies:

- Authored/native count and array allocation, including the empty count/pointer invariant.
- Every nonempty strong-reference selector against its authored typed target.
- Exact metadata list markers, row/field tags, types, sentinels and native coordinates.
- The full source-sized allocation and all relocation/import owners.

GameObjectNestedValueContract checks every row's scalar bytes against separately
encoded source/default values. The root comparer detaches the list and retains
its complete comparison for all other owners. Other owners' stock occurrence
coordinates are normalized only in a comparison source; actual source and
emitted bytes are never rewritten to stock movement data.

Complete original-object metadata retains a zero-count LocomotorSet declaration
even when the list is omitted. Legacy EditableValues.Speed is null when there
is no unique row; the receipt's LocomotorSets array reports all authored rows.
Mixed builds also report each GameObject's LocomotorSetCount.

## Developer verification

The `conscript-module-graph-self-test <static> <global> <fresh-output> movement`
group covers 40 rows with optional imports and EP1 enum literals, omission, and
a default-only empty row. Each case serializes and reads back full streams/BIG,
repeats the build, and injects native and metadata corruption. The expanded
case also corrupts the final row, not only the first row.

`Test-Ra3Ep1DevSdk.ps1 -MultipleObjectsOnly` combines 1/0/40-row GameObjects,
shared custom armor/weapons and three explicit production commands in an
eleven-asset package. Automated output stays inside alpha/.work; the original
RebornTest source is restored and the user's Reborn is not built or edited.

This is offline compiler/package evidence, not gameplay validation of every
movement condition, repeated condition combination or missing-locomotor behavior.

<!-- Reborn: record measured verification outcomes rather than inferring gameplay support from schema admission. -->
The focused movement group passed all three complete stream/package cases:
expanded/omitted/defaults reference occurrence counts were 301/274/274, with
28 deliberately corrupted native graphs and 39 metadata graphs rejected.
The compiler build completed with zero warnings and zero errors.

The existing combined fixed-value/variable-module regression also passed:
37 behavior modules, 273 references, and 14 native/10 metadata corruptions
rejected. XML diagnostic regression retained all 25 passing source-location
checks. The EP1 movement enum codec also covers the already implemented AI
DefaultLocomotorSet field, using its official schema alias.

The final SDK integration passed all three controls: the eleven-asset mixed
graph (three objects with 1/0/40 movement rows), restored developer source build,
and explicit local recruitment consumer. RebornTest source was restored, the
user's Reborn was neither built nor modified, and no game was executed.
The stable alpha tools match the built compiler.
