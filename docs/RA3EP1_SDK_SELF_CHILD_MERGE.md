# Bounded local empty-child matching — October 7, 2026

## Outcome

The independent `--self-child-merge` flag selects
`diagnostic-self-empty-child-merge-v1`. It extends the reviewed same-document,
same-type tree scope with two-sided matching only when the matched child's
effective schema is an **empty-content complex type**. Its literal attributes
can be overlaid by the existing core NodeJoiner. This is not generic XML merge
support, imported inheritance support or a production compiler.

The real 396-document / 664-Include graph remains **205 Validated / 191
RequiresPreprocessing / zero SchemaInvalid**, with zero regressions against
`--self-tree-copy`. No real document newly validates under this local-only flag.
The separate, unchanged root-file profile's best result remains **207 / 189**;
205 here is a different visibility scope, not a regression of that profile.
Both comparison commands exit 2, six original path issues remain, no limit/stale/read
failures occur, and the requested output directory remains absent. No official
XML/XSD changes, native processor calls, payload dumps or game proof.

## Why the scope is deliberately narrow

`NodeJoiner.SelectSame` matches singleton sequence children by node name and
repeated sequence children by `id`. Its ID search spans sibling names.
`ReplaceXmlNode` appends XmlText nodes; a matched singleton containing `left`
and `right` produces `leftright`, not `right`. An owned fixture verifies this
core behavior directly. Consequently matched simple/simpleContent text and
element-only branches remain closed rather than receiving assumed replacement
semantics. Cross-QName IDs are rejected even though the core can remove the
previous node and insert a different QName.

Admitted behavior:

- Singleton empty-complex children match by name, including a changed literal ID.
- Repeated empty-complex children match a unique literal sibling ID.
- Absent base attributes survive; derived literal attributes replace supplied ones.
- Unmatched named-ID or anonymous repeated children append in core sequence order.
- Unmatched text/branch children retain the earlier bounded tree-copy rules.
- Local chains can use the resolved result as a subsequent base.

Before core allocation, the gate checks projected direct-child occurrence counts.
Existing source/document amplification, 32-link inheritance, 32-level tree and
8,192-authored-element limits remain in force. Effective schema declaration lookup,
unprefixed names, unique sibling IDs, literal attributes and all prior directive,
expression, wildcard/choice/mixed/list-modifier restrictions remain unchanged.
Rejecting any asset withholds the entire processed document and all overlay evidence.
The graph's final schema validation is still mandatory; a merged invalid scalar
produces SchemaInvalid and no trusted file dependency fields.

## Implementation and tests

`source/BinaryAssetBuilder.ManifestInspector/SdkSelfAttributeInheritance.cs`:
`Apply(..., childMerge:true)` selects the separate name and retains tree guards.
`CheckMerge` mirrors core singleton-name/repeated-ID selection, rejects cross-QName
collisions and non-empty matched types, and bounds projected cardinality.
No changes were made to the core NodeJoiner itself.

`SdkTypedSourceGraph.Inspect/BindGraph(..., selfChildMerge:true)` admits this local
profile and preserves the existing definition subset for non-inherited documents.
The flag is mutually exclusive with all earlier preprocessing flags, cannot be
duplicated and is refused by `sdk-source-preflight`. CLI tests verified all three
refusals with exit 1. It does not widen either direct-instance profile.

`SdkSelfChildMergeSmokeTest.Run` is registered in `CompilerSmokeTest.Run`:
all **127 default compiler groups** passed. Owned fixtures cover singleton and
repeated matching, anonymous append, sequence order, retained/overridden attributes,
chains, copied file fields, the actual core text-append hazard, atomic rejection
of ID collisions/occurrence overflow/directives/expressions/prefixes/populated
branches/text/amplification, invalid-scalar no-fields, old-profile isolation,
source-byte preservation and no output publication. The initial build had three
existing warnings; the final incremental build passed with zero warnings/errors.
All 33 enum checks and three classifier script
fixtures also passed. Model/marshaller coverage remains 785/1390 and 762/1390.

```text
BinaryAssetBuilder.ManifestInspector.exe sdk-self-child-merge-self-test
BinaryAssetBuilder.ManifestInspector.exe sdk-typed-source-graph ra3ep1 <schema-root> <source-root> <entry.xml> <new-output-directory> --self-child-merge
```

## Remaining work

The earlier triage found 178 first Include-free blockers sharing 23 imported base
documents. This milestone proves one prerequisite but does not open their Include
chains. Next is a separately admitted recursive direct-instance preparation scope
with direct-definition eligibility, bounded cache/depth/expansion work and complete
source-closure witnesses. Expressions and matched populated branches can still
block those documents after that gate is opened. Relative imported file fields
remain closed until node/field provenance is supported.

FullDependencyCoverage and ProductionBuildReady stay false. Native type/hash/layout
validation, minimal packaged build and actual Uprising load tests remain required.
Weighted engineering effort stays about 50.25%, rounded **50% complete / 50% remaining**;
a new test group does not automatically increase it.
