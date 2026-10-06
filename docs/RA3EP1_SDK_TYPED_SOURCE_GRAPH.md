# Rechecked typed source graph — October 6, 2026

## Outcome and command

The real Uprising global.xml graph contains 396 captured XML documents and 664
Include edges. All 396 documents were re-read/fingerprint-checked and classified:

| Status | Documents | Meaning |
| --- | ---: | --- |
| Validated | 197 | Raw document schema-valid; physical dependency status is separate |
| RequiresPreprocessing | 194 | inheritFrom overlay present; no trusted typed fields published |
| SchemaInvalid | 5 | Raw values include unevaluated expressions; no trusted fields published |

There were no stale/read failures or limits. One typed PathfinderEventHeader path
requires an explicit AUDIO root. The original six path-audit issues remain visible:
three AUDIO XML Includes, the AUDIO header and two trailing-dot map Includes.
The command returned exit 2; requested output remained absent.

```text
BinaryAssetBuilder.ManifestInspector.exe sdk-typed-source-graph ra3ep1 <absolute-schema-root> <absolute-source-root> <absolute-entry.xml> <absolute-new-output-directory> [--art-root absolute-directory] [--audio-root absolute-directory] [absolute.manifest=runtime.manifest ...]
BinaryAssetBuilder.ManifestInspector.exe sdk-typed-source-graph-self-test
```

JSON preserves Environment and SourcePaths and adds TypedSources.Schema/Graph.
The reviewed schema policy/warnings/source and candidate digests remain explicit.
Exit 0 requires the entire **scoped** graph to pass path/source/XML/resource checks;
exit 2 is diagnostic incompleteness, not build success. FullDependencyCoverage and
ProductionBuildReady always remain false; ReadOnly/SnapshotOnly remain true.
The original path-only command and environment wrapper contracts are unchanged.

## Implementation and safety

`SdkTypedSourceGraph.Inspect` compiles/reviews once through SdkEffectiveSchema,
receiving the exact admitted set only after schema catalog rechecks. It does not
compile once per XML, use registry discovery or run processors.

`BindGraph` requires captured source paths to be unique, canonical and inside their
attributed explicit DATA/ART/AUDIO root. Roots and reads retain the existing
device/ADS/reparse guards. Each raw source must match both its captured byte length
and SHA-256 before any trusted binding. A changed source reports StaleSource and
does not authorize following its new Includes. Missing/bad reads report SourceRead.

Documents with any EA inheritFrom attribute are withheld at document granularity,
including independent assets in that same document. This conservative profile does
not pretend to expand asset overlays. Otherwise Bind validates the captured bytes;
schema-invalid documents publish no partial typed fields. At most eight schema
diagnostics per document are retained (not an assertion of the total error count).

Only schema-selected file attributes/elements from admitted raw-valid documents
are passed to the existing confined literal resolver. Resources report logical
path, physical path if resolvable, existence/length and ResourcePath/ResourceMissing
issues. No payload body or content hash is read. A resource can be XML-schema-valid
but still outside the admitted physical path profile, e.g. a FileReference value
containing an expression. Such a graph cannot report scoped completion.

Bounds: 512 sources, 4 MiB each / 32 MiB aggregate recheck, 4,096 typed dependencies,
and existing per-source/schema/path bounds. Prior path limits propagate without
mislabeling a successfully validated source as a new dependency limit. Empty or
partial/limited inventories cannot report completion. Consecutive bounded reads
are snapshot-only, not an atomic filesystem transaction or a production cache key.

## Real raw-source failure interpretation

The corpus is the external Uprising Xml (Uprising) root documented in prior reports.
No reference XML/XSD was edited; no large BIN/WorldBuilder body was read.
SchemaInvalid here means failure **before preprocessing**, not proof that the EA
source is defective. Examples include AudioSettings stream/queue macros, Eva
priority/timing macros, GameLOD ambient-stream macros and PlayerTemplates account/
beacon macros, all with values of the form `=$...`.

Core `AssetDeclarationDocument.ProcessExpressionsInNode` evaluates attribute/node
values whose first character is `=`. DocumentProcessor.ProcessDocumentContents
orders ProcessExpressions, ProcessOverrides, then Validate; definition evaluation
occurs in the included-document processing path. Therefore blindly validating raw
files, or swapping source assets alone, is insufficient. The next implementation
must respect definition/include roles, evaluator behavior and overlay order, without
inventing macro values from unpacked binaries or silently dropping required fields.

The 197 count is not 197 successfully compiled game assets. This graph covers only
the admitted reachable global.xml slice, not all static/worldbuilder source files.
The one typed AUDIO header path has no supplied root, so ResourcePath is reported;
this is not a conclusion that no copy exists anywhere on disk.

## Validation and next work

Release/x86 builds; all 117 default compiler groups and 33 enum checks pass.
Owned two-source fixtures cover confined relative typed files, metadata inspection
with an exclusively locked payload, changed fingerprints, raw inheritance refusal,
invalid XML/no partial fields, missing resources, schema-valid expression path
rejection, prior limits, duplicate/escaped inventory and deleted sources.
The tests never alter reference input trees or run production/native processors.

Next map the core definitions/expression/overlay pipeline and integrate an admitted
bounded preprocessing slice before retrying real graph binding. Preserve original
and processed identities separately and keep physical file closure separate from
asset/import references. Missing original AUDIO/ART inputs, final type hashes and
processors, WorldBuilder packaging and actual game loading remain release gates.

Inventory remains 785/1,390 models and 762/1,390 typed marshallers. Weighted effort
remains approximately 50.25%, rounded to 50% complete / 50% remaining; the new
document-level counters are diagnostics, not a measure of working mods.
