# Bounded local define diagnostics — October 6, 2026

## Outcome

The explicit `diagnostic-local-literals-v1` profile resolves 534 direct local
references in real Uprising `Sounds/Eva.xml`, after rechecking its raw fingerprint
and before schema binding. Eva now validates against the reviewed diagnostic
schema. Reference XML/XSD files remain untouched; no assets or output streams are
compiled. This is not a replacement for the EA expression evaluator.

The real global.xml snapshot still has 396 source documents / 664 Include edges:

| Status | Raw default | Explicit local profile |
| --- | ---: | ---: |
| Validated | 197 | 198 |
| RequiresPreprocessing | 194 | 198 |
| SchemaInvalid | 5 | 0 |

The last row does **not** mean all five raw failures were fixed. Only Eva was
resolved; AudioSettings, GameLOD, PlayerTemplates and PlayerTemplates_EP1 are now
explicitly blocked because their expressions require imported-definition review.
The original 194 inheritFrom documents remain blocked, without partial typed
fields. The six source-path issues and one unresolved typed AUDIO header remain.
No size/count limit was hit; the command returned exit 2 and output stayed absent.

Eva raw SHA-256:
`601738B84931D38B1AD85C91037C54BC1EB86C9C660AF18F7C7C8BE0C0D4DDF1`.
Processed in-memory XML SHA-256:
`43F6204B772239D94E9DD2D647AFFD4855C9C806E75C34CC4602F282280067BD`.
The latter includes deterministic XML serialization, not solely changed values;
it is diagnostic evidence, not a production compiler/cache identity.

## Command and implementation

```text
BinaryAssetBuilder.ManifestInspector.exe sdk-typed-source-graph ra3ep1 <absolute-schema-root> <absolute-source-root> <absolute-entry.xml> <absolute-new-output-directory> --local-defines [--art-root absolute-directory] [--audio-root absolute-directory] [absolute.manifest=runtime.manifest ...]
BinaryAssetBuilder.ManifestInspector.exe sdk-local-defines-self-test
```

`SdkLocalDefineProfile.Apply` parses already captured bytes with DTD/external
resolution disabled. It substitutes only exact `=$NAME` values in asset attributes
or text/CDATA, using case-sensitive ASCII names and local literal Define values.
There are no disk Include reads, evaluator DLL execution, environment variables,
registry discovery, arithmetic, function calls, imported definitions, overrides,
define chains or asset overlays. Documents needing those features fail closed.
Any nonempty Includes container blocks documents requiring substitutions, even
when the currently referenced name happens to be locally declared. This avoids
guessing imported duplicate/override semantics. Empty Includes is allowed.

The profile scans only top-level EA assets, excluding Defines/Includes/Tags.
Definitions are not themselves substituted. Duplicate or malformed definitions,
multiple Defines containers, true/invalid overrides, unsupported expressions and
missing names reject the **whole** transformation: no partial processed bytes,
processed digest, substitution count or trusted fields are published.

Bounds: raw/processed XML 4 MiB, recursion depth 128, 512 definitions, 128-character
names, 512-character literal values and 2,048 substitutions. Existing graph and
resource confinement bounds remain in force. Literal substitution cannot authorize
an escaped physical dependency path. No-expression sources keep identical bytes.

`SdkTypedSourceGraph.BindGraph` still verifies source identity before preprocessing
and refuses inheritFrom first. Its report records PreprocessingProfile, and each
processed/rejected eligible document has separate raw/processed evidence.
Schema validity and physical resource existence remain distinct. The default graph
still processes raw bytes, and the path-only command rejects `--local-defines`.
ProductionBuildReady and FullDependencyCoverage remain false in both profiles.

## Core evidence and remaining semantic review

`AssetDeclarationDocument.GatherDefines` collects name/value/override from local
Defines children. `DocumentProcessor.ProcessIncludedDocuments` processes children
first, imports each source-backed child's AllDefines via
`DefinitionSet.AddDefintions`, then calls `EvaluateDefinitions`. This occurs before
`ProcessDocumentContents` invokes ProcessExpressions, ProcessOverrides and Validate.
Include roles differ for asset visibility; source-backed definitions are imported
outside that role switch. Precompiled stream paths take a separate route.

`AssetDeclarationDocument.ProcessExpressionsInNode` triggers on a leading `=`.
`ExpressionEvaluatorWrapper` dynamically loads BinaryAssetBuilder.ExpressionEval;
the repository exposes the IExpressionEvaluator interface and reference DLL, but
no evaluator implementation source was found in this inspection. Literal
substitution here is deliberately a diagnostic subset, not a claim of equivalent
definition typing, arithmetic, unit parsing, duplicate/override or DLL semantics.
The successful Eva result is schema-validation evidence, not compiled/game proof.

Next: capture/recheck imported definition closures, preserve definition origin and
duplicate rules, distinguish source-backed versus precompiled references, and
review evaluator behavior before admitting any broader profile. Then handle asset
inheritance in core order with direct-instance/source and type compatibility checks.
Missing AUDIO/ART source inputs, final type tables/processors, WorldBuilder
integration, native stream validation and actual game loading remain open gates.

## Validation and progress

Owned tests cover attribute/text escaping, raw/processed hashes, source preservation,
atomic rejection after an earlier valid substitution, missing/case-mismatched names,
arithmetic, chains, duplicate/override/imported/inherited definitions, DTD/depth/
size/count gates, raw default isolation, graph integration, stale source rejection
and physical root confinement. No reference files or payload bodies are modified.

Inventory remains 785/1,390 models and 762/1,390 typed marshallers. The new registered
group brings the runner to 118 groups. Weighted effort remains approximately
50.25%, rounded to 50% complete / 50% remaining; passing one source-preprocessing
slice does not remove the larger native, dependency and game-validation gates.
