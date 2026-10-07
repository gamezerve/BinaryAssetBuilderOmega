# One-sided complex leaf inheritance — October 7, 2026

## Outcome and evidence

The separately explicit `diagnostic-self-complex-child-copy-v1` profile extends
copy-only local inheritance to complex **leaf** children: attributed simpleContent
and empty-content elements. It does not admit arbitrary complex trees. The existing
core NodeJoiner still performs every overlay; no production joiner was changed.

Compared with the earlier flat-child profile, two real Uprising documents newly
validate against the reviewed diagnostic schema:

| Source-relative path | Local overlays | Processed diagnostic XML SHA-256 |
| --- | ---: | --- |
| Sounds/BaseSoundEffect.xml | 4 | B8D9330F77506C6C57C54E59B5B559C37945CCBE59ABDF491AF4F91424634950 |
| Sounds/Speech.xml | 1 | E77F0126523D0CB16AB600B1974C94785C4A4C022D1129C38A6F6B641D22734F |

BaseQuickChatDialog contributes PitchShift Low/High to an attribute-only base.
VolumeSampleMovie contributes VolumeSliderMultiplier Slider/Multiplier and Filename
to its same-document BaseDialogEvent. Speech.xml's Include of BaseSoundEffect.xml
is not used for inheritance: its admitted base is local. These digests identify
serialized diagnostic XML, not native asset hashes, audio payloads or game streams.

The global.xml graph has 396 documents, 664 Include edges, **205 validated / 191
RequiresPreprocessing / zero SchemaInvalid**. Six original path issues and one
typed unresolved AUDIO header remain. No limit, stale-source or read failure was
observed; command exit is 2 and the requested output directory stays absent.
FullDependencyCoverage and ProductionBuildReady remain false.

Remaining first document-level failures are 99 complex child identity/directive/
expression gates, 83 nested/mixed-content gates, five missing same-document bases,
three asset directive/expression gates and one unknown asset attribute. These are
first failures only. The higher missing-base count reveals later failures in two
documents after their earlier child-shape gate opens; it is not a regression.

## Commands and implementation

```text
BinaryAssetBuilder.ManifestInspector.exe sdk-typed-source-graph ra3ep1 <absolute-schema-root> <absolute-source-root> <absolute-entry.xml> <absolute-new-output-directory> --self-complex-child-copy [--art-root absolute-directory] [--audio-root absolute-directory] [absolute.manifest=runtime.manifest ...]
BinaryAssetBuilder.ManifestInspector.exe sdk-self-complex-child-copy-self-test
```

The new flag is exclusive with every other preprocessing flag and refused on the
path-only command. `SdkTypedSourceGraph.Inspect/BindGraph` select its independent
report identity and call `SdkSelfAttributeInheritance.Apply(...,
complexChildCopy:true)`. Non-inherited documents retain the existing bounded
definition-subset profile. Raw and all earlier preprocessing profiles are unchanged.

`CheckChildren` still requires a flat direct schema sequence. Complex child types
must have TextOnly or Empty content and no attribute wildcard. Effective declared
unqualified attributes are admitted, except id, inheritFrom and TypeId; namespaced
directives, unknown attributes, leading-equals values and list modifiers are refused.
Child IDs remain closed because core repeated-child selection matches by ID and
could coalesce even a one-sided copy. Negative numeric scalar values such as
PitchShift Low=-1 are not list modifiers and remain legal when the schema allows.

SimpleContent text preserves its value and attribute absence: schema defaults are
not authored into the diagnostic XML. Text expressions (including comment-split
nodes), CDATA, processing instructions and nested elements are refused. Empty-content
children cannot contain meaningful text. Both populated sides remain rejected even
with disjoint child names. Nested/mixed trees, choice/group/wildcard particles,
imported bases and general expression evaluation remain closed.

Final full-document schema binding checks attribute values, required attributes,
order and occurrence requirements; structurally admitted but invalid content yields
SchemaInvalid with **no trusted dependency fields**. Syntactic admission alone is
not XML validation. Unsupported admission rejects the entire transformation with
no bytes, processed digest or partial overlays.

## Bounds, tests and remaining work

All prior raw/processed 4 MiB, aggregate source 32 MiB, local asset 4,096, inheritance
chain 32 and source confinement/snapshot gates remain. Child serialization enters
the aggregate amplification budget before core merge; final output is checked again.
Formatting whitespace follows core's default parsing behavior; raw files are never
rewritten. Resource checks read only existence/length, not large payload bodies.

The new registered smoke-test group covers attributed simpleContent, empty leaves,
both one-sided directions, repeated text/order, scalar attributes/default absence,
formatting, amplification, old-profile isolation and typed FileReference propagation.
Negatives include IDs, directives, expressions, list modifiers, wildcards, nested
and CDATA content, excessive occurrences, populated sides and invalid schema values.
Invalid values are also tested through the graph to prove no trusted fields leak.
All 123 default groups and 33 enum checks pass. Model/marshaller inventories remain
785/1,390 and 762/1,390; all 48 EP1-only types have both.

Weighted engineering effort remains about 50.25%, rounded 50% complete / 50%
remaining. Small diagnostic admissions do not close native/game release gates.
Next work is bounded nested-tree copying with proven core schema lookup, followed
by actual populated-child matching and source-backed direct-instance inheritance.
Missing AUDIO/ART source inputs, native type tables/processors, stream compatibility,
WorldBuilder integration and actual game loading still require separate validation.
