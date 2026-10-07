# Bounded keyed child removal — October 7, 2026

## Real result

The independent `--instance-removals` profile advances the 396-source / 664-Include
graph from **254 Validated / 142 RequiresPreprocessing** to **374 / 22**. Exactly
120 documents newly validate: 56 under EP1, 61 under maps and three under SkirmishAI.
There are zero SchemaInvalid documents and zero earlier-valid regressions. Across
105 validated documents, 481 owner-authored removal commands are recorded; inherited
commands are not replayed as new owner operations.

AlliedCoopBaseSkirmishPersonality.xml now validates with a four-source closure and
three StrategicState removals: AlliedNavalBase, AlliedExpansion and AlliedCaptureTech.
Its processed owner-only XML SHA-256 is
`988AF5F4DC50DEB9477AF624EAA21238227350E8F8BD7BAFCFB6F8809D62AA7C`.
EP1/SkirmishAI/Personalities/CommandersChallenge/AIP_CC_BaseAlliedBalanced.xml
validates with a five-source closure and a Cleanup removal; its processed SHA-256 is
`0426FFC50C3D816C08DA7302690C520DC13F74C3E1696A023597EA2BD2DBAB8F`.
The original SkirmishAI/Personalities/AlliedBalanced.xml remains blocked at a later
particle-shape gate. These hashes identify diagnostic XML, not native asset/cache IDs.

Exit 2, six original path issues, no limit/stale/read failures, requested output
directory absent. There are still 198 unresolved typed dependency occurrences of
one unique AUDIO header. ScopedGraphComplete, FullDependencyCoverage and
ProductionBuildReady remain false. Reference XML/XSD and the core NodeJoiner are
unchanged. This is source preparation/schema progress, not a packaged or game-loaded
Uprising mod.

## Deliberately strict removal contract

Profile name: `diagnostic-direct-instance-removals-v1`. It includes the earlier
recursive direct-definition, explicit-root imported-file and empty-child matching
scopes without widening the older flags. Only direct top-level asset children with
an exact instance-namespace `joinAction="Remove"` command are admitted. The child
must have:

- a direct sequence declaration with maxOccurs greater than one;
- an empty-content complex type declaring an unqualified id attribute;
- a bounded literal id unique across the owner's direct siblings;
- only id, the exact Remove command and namespace declaration attributes;
- no child nodes, payload attributes, nested content or extra directives;
- an inherited top-level owner and an existing same-QName/same-ID resolved base target.

Command stubs can omit required payload attributes because they are consumed by
the core before final schema validation. This does not loosen the final schema.
Projected occurrence counts account for deletions; subsequent additions still obey
the ordered pre-allocation bound. Deleting all required children fails final schema
validation and exposes no trusted file fields.

The core's SelectSame searches repeated-child IDs across sibling names. Its absent
Remove target behavior is warning/no-op. An owned fixture verifies that behavior,
while this diagnostic subset refuses missing and cross-QName targets. Singleton,
branch/subtree/nested deletion, Remove followed by re-adding the same ID, payload
attributes on command stubs, case variants and other joinAction/insertPosition
commands remain closed. This is stricter than general native behavior by design.

Scope refusal returns no partial XML, processed digest, overlays, removals, imported
bases or source closure. A later schema-invalid owner can retain diagnostic operation
and hash evidence but never trusted dependency fields. Imported relative paths remain
closed even if a derived command would ultimately remove such a field: selected-base
provenance is still checked before overlay.

## Implementation and verification

`SdkSelfAttributeInheritance.Apply(..., childRemoval:true)` selects the independently
named local subset and retains all earlier tree/chain/amplification guards.
CheckChildren admits only eligible removal stubs; CheckMerge proves an existing
exact target and decrements projected counts before calling the unchanged core.
`Evidence.Removals` identifies Type, DerivedId, resolved literal BaseId, ChildName
and ChildId for commands executed in that document. Older profiles return an empty
array. Prefix-qualified inheritFrom handles do not change the recorded literal base ID.

`SdkInstanceInheritanceProfile(..., removals:true)` selects child-first preparation
and publishes owner-only removal evidence alongside the existing source closures.
`SdkTypedSourceGraph.Inspect/BindGraph(..., instanceRemovals:true)` and the CLI flag
select this profile exclusively; duplicate/conflicting/path-only probes exit 1.
No production processor or codec is invoked.

`SdkInstanceRemovalsSmokeTest.Run` is registered in the default runner. All **129
compiler groups**, all 33 enum checks and three classifier script fixtures passed.
The first sandboxed full-suite attempt was blocked by an existing audio test moving
its own GUID-named temporary fixture; the full suite passed when rerun with the
required permission. No unrelated test code was changed. Final build: zero
warnings/errors. Model/marshaller counts remain 785/1390 and 762/1390; all 48 EP1-only
types retain both.

Owned fixtures prove required-payload command stubs, exact target/type selection,
chained owner-only witnesses, consumed directives, resource field closure and
post-removal required cardinality. Negative fixtures cover missing targets, QName
collisions, duplicate IDs, singleton/branch/nested/non-inherited commands, payload
attributes, wrong namespaces/case, other directives and atomic late failure.
Older profile refusal, unchanged owner bytes and no output publication are checked.

```text
BinaryAssetBuilder.ManifestInspector.exe sdk-instance-removals-self-test
BinaryAssetBuilder.ManifestInspector.exe sdk-typed-source-graph ra3ep1 <schema-root> <source-root> <entry.xml> <new-output-directory> --instance-removals [--art-root absolute-directory] [--audio-root absolute-directory]
```

## Remaining first blockers

These are first document-level/check-order failures, not exhaustive readiness:

| Gate | Documents | Examples |
| --- | ---: | --- |
| Non-flat direct sequence particle | 11 | AlliedBalanced, TestPersonalities, AIP_CC_29 |
| Asset directive/expression | 3 | Sounds/Music.xml, SoundEffects.xml, Voice.xml |
| Duplicate/unsafe sibling ID | 3 | AIP_S04_AlliedGroundBase, AIP_A04_JapanMechaWarfare, AIP_CC_01 |
| Unknown asset attribute | 2 | AITargetHeuristicLibrary, AIMicroManagerLibrary |
| Occurrence bound | 1 | GlobalData/Upgrade.xml |
| Unsupported Include visibility | 1 | GlobalData/ObjectCreationLists.xml |
| Cross-QName ID collision | 1 | AIP_CC_32 |

Next: inspect the exact effective particle shapes in the 11-document group before
admitting any broader child schema scope. Do not flatten choice/group/nested
sequence particles or allow namespace/ID collisions without separate core-backed
tests. Source validation still must lead into native type/hash/layout/dependency
gates, a minimal packaged build, WorldBuilder integration and actual Uprising loading.

## Engineering estimate

The manually assessed SDK tools/dependencies workstream rises from 25% to 30% after
the accumulated recursive preparation and removal milestones. Its 15% overall
weight adds 0.75 percentage point, bringing the weighted estimate from 50.25% to
**51% complete / 49% remaining**. Other workstreams are unchanged; in-game validation
remains zero proven. This reassessment is not automatic credit per test or commit,
nor does 374/396 schema-valid XML imply a similarly complete SDK.
