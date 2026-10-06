# Core-backed local attribute inheritance PoC — October 6, 2026

## Outcome: fixture proof, not another real corpus completion

The explicit `diagnostic-self-attribute-inheritance-v1` profile proves the first
bounded asset-overlay slice using the existing core NodeJoiner: same-document,
same-type, attribute-only bases/derived assets, including local chains. Owned
fixtures inherit required file attributes, preserve derived identity, replace
scalar attributes and bind three confined file references without source writes.

The real global.xml graph **does not gain another validated document**: it remains
202 Validated / 194 RequiresPreprocessing out of 396 documents and 664 Include
edges. This profile retains the previous definition subset for non-inherited
documents. The first blocking reason in inherited documents is:

| First document-level blocker | Documents |
| --- | ---: |
| Non-attribute-only asset content | 191 |
| Base not available as a unique same-document asset | 3 |

These are first blockers, not an exhaustive count of every unsupported operation.
Even a leaf derived asset in Upgrade.xml cannot release the whole document: other
derived upgrades contain GameDependency children. AmbientStream/AudioEvent sources
also contain filename, sound or other child structures. The profile intentionally
withholds the complete document instead of publishing a misleading partial graph.
All six source-path issues and the unresolved typed AUDIO header remain. Exit 2,
no limits/stale/read failures in this run, output absent, production readiness false.

## Command and implementation

```text
BinaryAssetBuilder.ManifestInspector.exe sdk-typed-source-graph ra3ep1 <absolute-schema-root> <absolute-source-root> <absolute-entry.xml> <absolute-new-output-directory> --self-attribute-inheritance [--art-root absolute-directory] [--audio-root absolute-directory] [absolute.manifest=runtime.manifest ...]
BinaryAssetBuilder.ManifestInspector.exe sdk-self-attribute-inheritance-self-test
```

The profile is exclusive with the three earlier flags and cannot be used on the
path-only command. Earlier raw/local/Include/definition profiles still refuse asset
inheritance. Fresh source fingerprints are checked before invoking the overlay
helper. No Include file is opened by this helper and no runtime manifest supplies
an inheritance base. Non-inherited documents use the prior definition subset;
inherited documents require the entire asset slice to be expression-free leaves.

`SdkSelfAttributeInheritance.Apply/Resolve` builds a unique local type:id table.
Bare base names resolve within the derived type; qualified handles must explicitly
name that same type. Only exact case-sensitive literal identity tokens are admitted.
Same-handle overrides, missing bases, cross-type inheritance, duplicate identities
and cycles reject the entire transformation. Same-document inheritance does not
require BaseInheritableAsset eligibility, matching the core's Self-location branch.
Imported/direct-instance eligibility is deliberately not inferred or implemented.

`CheckLeaf` requires an exact named complex schema type without an attribute
wildcard. Asset content permits only comments and whitespace text; children,
CDATA, processing instructions, meaningful text, namespaced directives/xsi:type,
TypeId, unknown attributes, leading-equals expressions and list +/- modifiers are
rejected. No unsupported attribute can silently disappear through overlay copying.
`NodeJoiner.Override` performs the admitted attribute replacement, not a new
approximation of its child merging logic. Final full-document schema binding still
controls whether any typed fields can be published.

The original inheritFrom attribute remains in merged XML, as in the core joiner.
This PoC preserves top-level document order; it does not assert equivalence to the
core's remove/append processing order, prevalidation XML hashes or production cache.
Overlay evidence records raw/processed SHA-256 and each local derived/base handle.
Rejected transformations expose neither partial bytes/digest nor partial overlays.
Physical dependency stat/confinement remains separate from XML/overlay validity.

Bounds: 4 MiB raw/processed XML, 4,096 top-level assets, 32 active recursion levels
and 32 semantic inheritance links even when earlier bases are memoized. Aggregate
new inherited-attribute bytes are bounded **before** each merge, with serialization
headroom, to prevent one large base attribute amplifying into unbounded output.
All existing graph/source/schema/resource limits remain. Reference bytes are never
modified; no processors, native codecs, registry or game launcher are run.

## Core rules that still need review/admission

`AssetDeclarationDocument.OverrideInstance` searches Self, All, Tentative and
Reference locations. Same-handle lookup skips Self. Only Self or an eligible
Tentative source directly included by instance is admitted; imported bases must
be BaseInheritableAsset-derived. All/Reference locations are not equivalent to
instance source visibility. The profile does not flatten those role differences.

`NodeJoiner` selects repeated children by id, distinguishes sequence/choice
particles, honors namespaced joinAction and insertPosition, and applies enum-list
bitflag additions/removals. Its schema-order insertion and missing-removal behavior
are not equivalent to generic XML attribute/child copying. Expressions are processed
before overrides in the core. These rules require separate child-particle and
direct-instance fixtures before broadening the admitted scope.

Next admit a carefully tested child-copy/merge slice using the core joiner, with
sequence order, repeated IDs, replacement/removal and defaults/required attributes.
Then capture imported direct-instance bases with raw/processed identities,
inheritable-type eligibility, cycle bounds and defining/consuming path context.
Actual native layout/type table, missing AUDIO/ART, streams, WorldBuilder and game
load validation remain release gates.

## Validation and progress

Owned tests cover bare/qualified/local/chained handles, scalar and required-file
inheritance, retained identity/handle, missing/self/cyclic/cross-type bases, duplicate
IDs, child/text content, instance/xsi directives, modifiers, expressions, unknown
attributes, forward/reverse chain limits and attribute amplification. Graph fixtures
prove earlier-profile isolation, raw source preservation, metadata-only resources,
absent output and stale source rejection. All 121 default compiler groups and
33 enum checks pass; final Release/x86 build has zero warnings/errors.

Inventory remains 785/1,390 models and 762/1,390 typed marshallers. Weighted effort
remains about 50.25%, rounded 50% complete / 50% remaining. No real inherited
document or in-game compatibility gate was closed by this first leaf-only PoC.
