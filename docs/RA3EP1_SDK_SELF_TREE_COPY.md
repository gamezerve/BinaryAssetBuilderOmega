# Bounded one-sided sequence-tree inheritance — October 7, 2026

## Outcome: copy support, not a newly working build

The separate `diagnostic-self-tree-copy-v1` profile admits recursively nested
direct schema-sequence children and declared literal child IDs unique across all
direct siblings. It still requires exactly one populated side of each local
same-document/same-type overlay. The unchanged core NodeJoiner performs copying;
there is no generic XML clone shortcut or actual populated-child merge admission.

Owned fixtures prove recursive, anonymous and complex-content-extension schema
lookup, repeated-child order, scalar text and inherited attributes, one-sided
copying in both directions, and a two-link local inheritance chain. Earlier raw,
attribute, flat-child and complex-leaf profiles do not gain these capabilities.

The real global.xml graph stays **205 validated / 191 RequiresPreprocessing / zero
SchemaInvalid**, with 396 sources and 664 Include edges. No earlier-valid document
regresses, but no additional entire document becomes valid. Six original path
issues and one typed unresolved AUDIO header remain. No limit, stale-source or read
failure is observed. Exit 2, requested output absent, FullDependencyCoverage and
ProductionBuildReady false. Official/reference XML/XSD bytes are never changed.

The initial nested-tree trial with child IDs still closed exposed 15 unavailable
local-base blockers and 161 child identity/directive/expression blockers. Admitting
proven unique sibling IDs changes the first-failure distribution to:

| First document-level blocker | Documents |
| --- | ---: |
| Complex child directive/expression gate | 110 |
| Base unavailable as a unique same-document asset | 65 |
| Non-flat direct-sequence particle | 9 |
| Asset directive/expression gate | 3 |
| Unknown asset attribute | 2 |
| Child occurrence overflow | 1 |
| Duplicate/unsafe child ID | 1 |

These are first failures only, not exhaustive asset counts. The larger local-base
count means earlier tree/ID guards no longer obscure imported inheritance needs.
It does not mean Include resolution is implemented, and it does not prove all 65
documents have otherwise eligible instance bases. That eligibility is next work.

## Command and scope

```text
BinaryAssetBuilder.ManifestInspector.exe sdk-typed-source-graph ra3ep1 <absolute-schema-root> <absolute-source-root> <absolute-entry.xml> <absolute-new-output-directory> --self-tree-copy [--art-root absolute-directory] [--audio-root absolute-directory] [absolute.manifest=runtime.manifest ...]
BinaryAssetBuilder.ManifestInspector.exe sdk-self-tree-copy-self-test
```

The flag is exclusive with every earlier preprocessing flag and refused on the
path-only command. `SdkTypedSourceGraph.Inspect/BindGraph` selects its report
identity and calls `SdkSelfAttributeInheritance.Apply(..., treeCopy:true)`.
Non-inherited documents retain the previous bounded definition subset; inherited
documents remain expression-free and local only.

`CheckChildren` recursively follows effective `XmlSchemaElement.ElementSchemaType`
through compiled flat direct `XmlSchemaSequence` particles. Simple types,
simpleContent and empty-content leaves retain the preceding scope; ElementOnly
complex branches are now recursively admitted. Populated choice/group/wildcard
shapes, mixed content, attribute wildcards, unknown elements/attributes, CDATA,
processing instructions, meaningful branch text, expressions, list modifiers,
xsi:type, TypeId, nested inheritFrom and joinAction/insertPosition remain closed.

Core `GetObjectType` searches the supplied parent particle using node.Name, while
`FindInBase` also searches complex base types. The owned fixtures exercise both
paths. Prefix-qualified asset/tree element names are conservatively rejected,
because the unvalidated core lookup does not normalize Name to LocalName. Default
namespace XML remains supported. No production lookup behavior was modified.

## Why unique IDs are safe for copying

Core `SelectMatchingKey` compares IDs across **all sibling names**, not only one
QName. Consequently, the tree profile checks a fresh case-sensitive sibling ID
set at every branch, before core copying. IDs must be declared schema attributes
and bounded ASCII literal tokens (1–128 characters, letters/digits/underscore/dot/
hyphen). Empty, unsafe or repeated sibling IDs reject the entire transformation.
The same ID can recur under different parents; it cannot recur among siblings,
even when their element names differ. This is deliberately conservative.

With only one populated overlay side and unique IDs in that source tree, copying
cannot select an already copied sibling by ID. Repeated children without IDs also
remain distinct. Both populated base/derived sides are still rejected, even when
their child names or IDs are disjoint. No replacement/removal/insertion or key
collision policy has been opened, and imported or same-handle overrides remain
closed. Final full-document validation still gates every typed dependency field.

## Bounds and verification

Tree depth is limited to 32 child levels, including simple/empty leaves; the
original top-level asset is level zero. At most 8,192 original asset/tree elements
are admitted per document, excluding metadata containers. Originals are checked
once; resolution does not incorrectly recount them. Expanded copies are bounded
separately by the pre-merge 4 MiB serialization amplification budget and final
4 MiB output limit. Existing source/inheritance/path/schema bounds remain intact.
Resource checks stat existence/length only; no large binary payload dump occurs.

The new registered group tests exact depth/element boundaries and one-over failures,
nested amplification, duplicate same-name/cross-name IDs, parent-local ID reuse,
unsafe identities, expressions/directives, mixed/choice/prefix/unknown/occurrence
and content failures. It also checks nested schema-invalid numeric attributes
produce SchemaInvalid with no trusted fields, nested FileReference propagation,
raw source identity and absent output, earlier-profile isolation and option
conflicts. All 124 default compiler groups and 33 enum checks pass. Model and typed
marshaller inventories stay 785/1,390 and 762/1,390; all 48 EP1-only types have both.

Weighted effort stays about 50.25%, rounded 50% complete / 50% remaining. Next:
source-backed direct-instance inheritance with fresh captured source hashes,
inheritable-type eligibility, exact visibility and defining/consuming path context;
then separately prove populated-child matching. Native type tables/processors,
stream compatibility, missing AUDIO/ART source inputs, WorldBuilder integration
and actual Uprising game loading remain release gates.
