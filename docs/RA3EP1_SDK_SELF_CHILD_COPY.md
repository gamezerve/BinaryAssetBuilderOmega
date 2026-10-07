# One-sided flat child inheritance — October 7, 2026

## Outcome

The explicit `diagnostic-self-child-copy-v1` profile admits a narrow extension of
the local leaf PoC: simple direct sequence children may come from the base **or**
the derived asset, never both. The existing core NodeJoiner performs copying and
schema-order insertion. Matching, overriding or removing populated children is
still closed. Official/reference XML and XSD bytes are never changed.

Real Uprising Sounds/AmbientStream.xml now validates after ten local overlays.
Its derived streams supply Filename children while BaseAmbientStream supplies
attributes only. Processed diagnostic XML SHA-256 is
`8C9413A824AA7CEFEB0BE84B7695F6E1E320638661795A0C660EDE06938E4AE0`.
This is not a native asset hash or a compiled audio/game proof.

The 396-document / 664-Include global.xml graph improves from 202 valid / 194
blocked to **203 valid / 193 blocked**. Remaining first blockers are unsupported
simple-child scope in 190 documents and absent same-document bases in three.
Those counts are first document-level blockers, not exhaustive per-asset counts.
Six original path issues and the unresolved typed AUDIO header remain. Exit 2,
no limits/stale/read failures in the measured run, requested output absent.
ProductionBuildReady and FullDependencyCoverage remain false.

## Commands and isolation

```text
BinaryAssetBuilder.ManifestInspector.exe sdk-typed-source-graph ra3ep1 <absolute-schema-root> <absolute-source-root> <absolute-entry.xml> <absolute-new-output-directory> --self-child-copy [--art-root absolute-directory] [--audio-root absolute-directory] [absolute.manifest=runtime.manifest ...]
BinaryAssetBuilder.ManifestInspector.exe sdk-self-child-copy-self-test
```

The new profile is mutually exclusive with all earlier preprocessing flags and
is rejected on the path-only command. The earlier local attribute flag still
refuses children. Non-inherited documents retain the previous definition subset;
inherited documents remain expression-free and same-document/same-type only.
There is no Include-base lookup, manifest substitution, registry search, native
processor invocation or source rewrite.

`SdkSelfAttributeInheritance.Apply(..., childCopy:true)` has a separate report
profile. `CheckChildren` requires flat direct XmlSchemaSequence declarations and
XmlSchemaSimpleType children. It refuses child attributes except namespace
declarations, complex/nested children, choice/group/wildcard particles, unknown
children, occurrence overflows, CDATA, directives and leading-equals text nodes.
Repeated simple children preserve their values/order within admitted maxOccurs.
Simple content is copied, not parsed as a general expression or native reference.

`Resolve` refuses any overlay where both the resolved base and derived asset have
children, even if their element names differ. This intentionally avoids unreviewed
child matching, repeated-ID behavior and joinAction/insertPosition semantics.
Entire documents are withheld on unsupported content; no partial transformed
bytes, processed digest or overlay evidence is published. Final full-document
schema binding still gates typed file fields and physical dependency inspection.

## Formatting finding and bounds

The first real AmbientStream trial hit core's `unexpected #whitespace` error:
NodeJoiner's child switch does not treat formatting XmlWhitespace as an ignorable
sequence child. The new profile parses with the core's default PreserveWhitespace
false behavior; simple child text is retained while formatting nodes are dropped.
The original leaf profile's parsing behavior is unchanged. Raw fingerprints
precede this in-memory normalization; processed hashes include serialization and
whitespace effects and are not production cache identities.

The 4 MiB raw/processed limit, 4,096 local assets, 32 active/semantic inheritance
links, exact local identities, schema attribute admission and cycle guards remain.
The pre-merge amplification budget now also includes inherited child serialization,
so many derived assets cannot multiply a large base child into unbounded memory/
output. Final serialized size is checked separately. Source snapshot/confinement,
schema and resource bounds remain unchanged; payload bodies are not read here.

## Validation and next work

Owned fixtures cover base-only/derived-only child copying, scalar order, repeated
simple children, formatted input, typed FileReference elements and unchanged raw
sources. Negative probes cover populated children on both sides, nested/attributed/
unknown children, expression text split by comments, CDATA, directives, maxOccurs
overflow and child amplification. Existing leaf-profile isolation and absent
production output are also tested.

All 122 default compiler groups and 33 enum checks pass. The final Release/x86
build has zero warnings/errors. Inventory stays 785/1,390 models and 762/1,390 typed
marshallers, with all 48 EP1-only complex types covered by both. Weighted effort
stays about 50.25%, rounded 50% complete / 50% remaining; one more diagnostic XML
document is not a closed native/game compatibility gate.

Next admit carefully tested complex child trees and actual populated-child merge
rules, then direct-instance imported bases with inheritable-type/source eligibility.
Keep defining/consuming path context and raw/processed identities distinct. Missing
AUDIO/ART source inputs, final native type tables/processors, output streams,
WorldBuilder integration and actual game loading remain release gates.
