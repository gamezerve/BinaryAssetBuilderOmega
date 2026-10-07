# Direct-instance Include-chain triage — October 7, 2026

## Measured outcome

The current root-file graph still contains 396 documents and 664 Include edges:
207 Validated, 189 RequiresPreprocessing, zero SchemaInvalid. Of the blocked
documents, **178 first stop at the imported-source Include-free gate**. These
178 owners reduce to **23 distinct first imported source documents**. Every one
of those sources has only `instance` child Include edges; all 23 independently
remain RequiresPreprocessing. This is not evidence that 178 documents are ready
after removing one guard.

The largest first-source groups are AlliedSoloBasePersonality.xml (28 owners),
JapanSoloBasePersonality.xml (21), and SovietSoloBasePersonality.xml (20): together
69 owners. Eight first-source groups are under
EP1/SkirmishAI/Personalities/CommandersChallenge (31 owners); the remaining
15 groups are under SkirmishAI/Personalities (147 owners).

No new assets validate in this milestone. The graph command exits 2 and the
requested output directory remains absent. The production-ready flag remains
false. The engineering estimate stays approximately 50.25%, rounded 50% complete
and 50% remaining. This work identifies shared bottlenecks, not game compatibility.

## Reproducible read-only classifier

`scripts/Get-Ra3Ep1InstanceIncludeBlockers.ps1` takes an existing parsed
`sdk-typed-source-graph --instance-root-files` report through the pipeline:

```powershell
# Reborn: classify captured Include evidence without rereading or modifying official XML.
$classification = $report | & ./scripts/Get-Ra3Ep1InstanceIncludeBlockers.ps1
$classification.Groups | Format-Table FirstImportedSource,BlockedOwners,Classification
```

Owners retain their first imported source identity and captured raw SHA-256.
Groups coalesce identical source paths only for reporting. Declaration order is
preserved when selecting the first Include-bearing direct source. InstanceOnlyCandidate
means only that this source's captured child edges are all `instance`; it does not
mean its expressions, schema, merging or file provenance have passed. Later direct
sources and later blockers are not classified. No XML/payload reads, source writes,
transitive handle injection, schema changes or production processor calls occur.
Captured hashes are report evidence, not a current-disk verification.

`scripts/Test-Ra3Ep1InstanceIncludeBlockers.ps1` passes three owned fixtures:
shared-base grouping versus other visibility, partial-inventory rejection, and
contradictory diagnostic/edge rejection. These are separate script fixtures, not
additional compiler groups; the compiler registry remains 126 groups.

## Why simply opening transitive visibility is wrong

`source/BinaryAssetBuilder.Core/BinaryAssetBuilder/Core/SageXml/AssetDeclarationDocument.cs`,
`ValidateInheritFromSources`, checks that a Tentative base's defining document is
in the consumer's direct InclusionItems. `OverrideInstance` also requires direct
inclusion. Visibility through another source is not sufficient inheritance authority.

For example, the real chain is:

```text
AlliedSoloBasePersonality.xml -> SoloBasePersonality.xml -> BasePersonality.xml
```

Each arrow is a direct `instance` Include, with each owner inheriting the asset
defined by its immediate child. A future implementation should prepare each child
in its own scope, then expose only that child's own declarations to its parent.
It must not flatten BasePersonality into AlliedSolo's direct base set.

Both BasePersonality and SoloBasePersonality contain populated StrategicState
children. AlliedSolo also has its own populated children. Thus opening recursive
preparation alone is insufficient: the existing populated-child merge restriction
is a separate, concrete blocker on this chain. This observation is not an exhaustive
analysis of all 23 source documents.

## Next implementation gates

1. Prove a separately selected two-sided merge subset against the actual NodeJoiner:
   same-ID replacement, unmatched append, anonymous children, QName collisions,
   sequence order/cardinality and directive rejection. Do not silently change the
   older one-sided profiles.
2. Add recursive direct-instance preparation with active-cycle checks, bounded
   depth including cache hits, aggregate source/expanded-byte budgets and duplicate
   origins. Export only each prepared document's own assets.
3. Attach closure witnesses for every contributing source (raw/processed hashes),
   not just the final direct base document. Recheck captured inputs before trust.
4. Preserve the explicit-root-only imported file policy; inherited relative paths
   need field/node provenance before consumer-context resolution can be correct.
5. Use this classifier to compare the 23 shared sources first, then rerun the whole
   graph, checking earlier-valid regressions and reporting newly exposed blockers.
   Require owned negative tests before admitting any larger scope.

None of these gates substitutes for native type/hash/layout, stream dependency
validation, a minimal packaged build or an actual Uprising load test.
