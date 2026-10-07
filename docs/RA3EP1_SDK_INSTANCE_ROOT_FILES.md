# Inherited explicit-root file fields — October 7, 2026

## Outcome and real witness

The separate `diagnostic-direct-instance-root-files-v1` profile admits schema-selected
imported file fields only when their logical value has an explicit DATA, ART or
AUDIO alias. Those aliases use the diagnostic command's declared roots, not the
defining or consuming XML directory. Imported relative/unknown-alias/native-absolute
values remain closed. The earlier `--instance-inheritance` file-free gate is unchanged.

Real `PathMusic/PathMusicEvents.xml` now validates with **197 local owner overlays**
using imported `PathMusicEvent:BasePathMusicEvent`. Its inherited field is
`PathfinderEventHeader="AUDIO:Pathfinder\RA3EPMus\PC\RA3EPMus.h"`.
Processed diagnostic owner XML SHA-256:
`1C127BE67D157A6DFF4689F77C4B0351A98A685634D05F4FEAA2C173B5A58F6F`.
Imported BasePathMusicEvent.xml raw/processed witnesses:
`B93CF51D38AC83CC807C6F3873EB94190181AE454B3129B9B30C8335AD35D750` and
`655C6590FCEE3483D7B369228A31DBA4BF77FD8E01658EB386E3BE5AB935EF95`.
These identify diagnostic XML only, not native assets or production cache entries.

The 396-document / 664-Include graph advances from 206/190 to **207 validated / 189
RequiresPreprocessing / zero SchemaInvalid**, with zero earlier-valid regressions.
Six original path issues remain. Typed unresolved dependencies grow from one to
**198 occurrences of the same AUDIO header**: its original base field plus 197
inherited fields. No new set of 197 distinct missing files was discovered. The AUDIO
root/header is still unavailable, so ScopedGraphComplete, FullDependencyCoverage
and ProductionBuildReady remain false. Exit 2, no limit/stale/read failures, requested
output absent. No source/XSD edits, processor execution, payload dump or game proof.

## Core finding and diagnostic boundary

`DocumentProcessor.ProcessIncludedDocuments` processes child documents before adding
their instances to owner visibility. `AssetDeclarationDocument.HandleFileReferenceType`
uses `TryGetFileHashItem` with that document's SourceDirectory and changes the XML
value to the cached physical path. The later imported overlay therefore sees already
prepared source file values. Copying a raw relative path and resolving it in the
consumer would not reproduce this preparation.

`FileNameResolver.ResolvePath/SearchPaths` also supports multiple configured search
paths, `*` meaning the current document directory, postfix preference and ART fanout.
Thus alias independence is **not universal production behavior**. This profile proves
only the existing scoped diagnostic resolver with one explicit root per alias,
no wildcard/search/postfix/registry fallback and no native cache/hash rewrite.

Literal DATA/ART/AUDIO values are copied unchanged. The graph still validates
physical confinement and checks existence/length under explicit roots. Missing roots,
macros, unsafe segments, traversal and embedded absolute paths cannot gain physical
authority. XML schema validity does not imply a valid or existing resource path:
such sources can be Validated while dependencies report ResourcePath/ResourceMissing
and scoped completion stays false. No body is read to infer payload identity.

## Command and implementation

```text
BinaryAssetBuilder.ManifestInspector.exe sdk-typed-source-graph ra3ep1 <absolute-schema-root> <absolute-source-root> <absolute-entry.xml> <absolute-new-output-directory> --instance-root-files [--art-root absolute-directory] [--audio-root absolute-directory] [absolute.manifest=runtime.manifest ...]
BinaryAssetBuilder.ManifestInspector.exe sdk-instance-root-files-self-test
```

The flag is exclusive with all earlier preprocessing flags, including the original
instance flag, and refused on the path-only command. `SdkTypedSourceGraph` selects
`SdkInstanceInheritanceProfile(..., rootFiles:true)` and its independent report name.
`RootQualified` recognizes supported alias authority only: lexical confinement,
availability and file identity are deliberately separate path/resource gates.

Selected imported bases are fully source-local-expanded/validated before file-field
inspection. Every selected file field must have a supported alias; one relative field
rejects the entire transformation even if a consumer might later override it. All
existing direct-instance/type/source/hash/tree/copy-only guards remain intact.

`ImportedBase.RootQualifiedFields` records **pre-overlay** selected schema fields.
It is not a map of final field origins or a claim that each field survives. For
example, a consumer-authored relative Header override replaces a DATA-root base
Header and correctly resolves under the consumer directory; the base witness still
records the original root-qualified field. Final dependencies describe the resulting
owner XML separately. Rejection never publishes partial bytes/digests/overlays/imports.

The same source/staging/owner 4 MiB, captured/expanded base 32 MiB, direct Include 64,
visible handle 4,096, tree element 8,192 and tree/inheritance depth 32 limits apply.
No output directory is created. Snapshots are not atomic across files and diagnostic
digests are not prevalidation/native cache hashes.

## Verification and next work

Owned fixtures use different base/consumer directories containing differently sized
same-name files. They prove DATA-root fields, case-insensitive AUDIO aliases, ART
basename fanout, element/attribute copying and consumer-relative overrides without
defining-directory fallback. Pre-overlay witnesses, raw/literal preservation and
absent output are asserted. Earlier instance-file rejection is unchanged.

Negative probes cover imported relative/unknown/native-absolute fields, missing AUDIO
roots, traversal, macros, trailing-dot segments and embedded absolute paths. Unsafe
or unconfigured root fields never acquire a physical path or scoped completion.
Profile conflicts are refused. All 126 default groups and 33 enum checks pass;
inventory remains 785/1,390 models, 762/1,390 typed marshallers and 48/48 EP1-only
coverage for both. The final build has zero warnings/errors.

Remaining first blockers are 178 imported-source Include gates, three populated-child
merges, three owner directive/expression gates, two unknown asset attributes, one child
occurrence, one child directive/expression and one delegated child directive/expression.
These are first document failures, not exhaustive asset eligibility or completed work.

Weighted effort remains about 50.25%, rounded 50% complete / 50% remaining. General
defining-document relative paths still need provenance-aware native/file preparation;
broader source-backed visibility and populated-child merging need separate proof.
Missing AUDIO/ART inputs, native processors/types/streams, WorldBuilder integration
and actual Uprising loading remain release gates.
