# Recursive direct-instance preparation — October 7, 2026

## Real result

The independent `--instance-chains` profile advances the 396-source / 664-Include
graph from **207 Validated / 189 RequiresPreprocessing** to **254 / 142**, with
zero SchemaInvalid and zero earlier-valid regressions. Exactly 47 documents newly
validate: 34 under maps, 12 under SkirmishAI and one under Sounds. This is diagnostic
XML preparation/schema evidence, not native asset compilation or a working mod.

Sounds/MissionDialogue.xml now validates with 1,272 owner overlays and a two-source
preparation closure. Its processed owner-only XML SHA-256 is
`F842D225C096361E2612E219BEB8600C27EECE556B436D1B65F910AA78ED93E7`.
CoopBasePersonality, SoloBasePersonality and several faction/campaign descendants
also validate. AlliedSoloBasePersonality has one owner overlay and three closure
sources:

| Source under SkirmishAI/Personalities | Raw SHA-256 | Prepared XML SHA-256 |
| --- | --- | --- |
| BasePersonality.xml | 5D088B64458371523955F82E6318AD558F943F211761797FA6DC70AECA1962DE | A9F665518DB102AE505F608B402E089E876549054D0C4D4F93F85572B153760B |
| SoloBasePersonality.xml | 0521B01F4ACDFFBDE471A6D83BC5828756C5F19A0058E86E13AB949D4722E3F3 | E88CBA3CA13FC15010D5F5917EBB5DF01BBD51D5C7FDC42E03733F0F5BE0AACF |
| AlliedSoloBasePersonality.xml | 10DF7A9A3A4111A27B0005C8912E6D1E22064A1826F2288CD2DB82A3BF047544 | A6A938A7DE34F5FCB7BDEEE6976D560994761080665DB5049D8B08EDC6548AC9 |

Hashes identify diagnostic XML snapshots only. They are not native type/asset hashes,
production cache identities or proof of atomic/live-disk state.

The graph command still exits 2. Six original path issues remain. There are still
198 unresolved typed dependencies, all occurrences of one unique AUDIO header.
No limit/stale/read failure was reported in the real comparison. The requested output
directory remains absent; reference XML/XSD bytes and production processors are
untouched. ScopedGraphComplete, FullDependencyCoverage and ProductionBuildReady
remain false. Engineering effort stays about 50.25%, rounded 50% complete / 50%
remaining; schema-valid document counts are not a usable-SDK percentage.

## Scope and direct-definition rule

Profile name: `diagnostic-direct-instance-chains-v1`. It combines child-first
preparation with the already tested empty-complex-child merge and explicit
DATA/ART/AUDIO imported-field scopes. Older flags retain their earlier admission
rules. All/reference/precompiled visibility, same-handle and cross-type overrides,
relative imported file fields, matched text/populated branches and general
expression/directive processing remain closed.

Each visited source is prepared in its own direct-instance scope. Only that
prepared document's **own** asset declarations are exposed to its parent. Descendant
declarations are not flattened into direct visibility. Consequently an owner that
includes Middle -> Leaf cannot inherit Leaf's asset unless Leaf's defining document
is also directly included by that owner. This follows the core
AssetDeclarationDocument.ValidateInheritFromSources/OverrideInstance rule, not
mere reachability in the Include graph.

Local handles still take priority. In this new profile, all direct Include candidates
of each prepared document are checked even if a local base suffices; the closure
contract cannot silently ignore unsupported edge roles or cycles. Direct duplicate
handles remain ambiguous. Shared descendant sources can be prepared once within an
owner invocation (a diamond), without acquiring new direct inheritance authority.

`Evidence.PreparedSources` records the complete visited preparation closure,
including the owner and candidates not ultimately selected as bases. Each direct
`ImportedBase.PreparedSources` separately records that base document's own closure.
Both use final owner-only XML hashes after temporary injected bases have been removed.
Earlier profiles publish empty arrays for these new fields. These are document
witnesses, not node/field provenance or claims that every visited source contributes
bytes to the final selected asset.

## Implementation and resource limits

`source/BinaryAssetBuilder.ManifestInspector/SdkInstanceInheritanceProfile.cs`:

- `Apply` selects old direct-only processing or the new transactional preparation.
  Raw/prepared caches reset per chain owner invocation, so a prior successful call
  cannot conceal a later stale nested source.
- `Prepare` checks active cycles before cache reuse, checks both current depth and
  cached semantic subtree height, and charges each uniquely prepared document's bytes.
- `ApplyDocument` rechecks ordered direct edges/path identities, prepares children,
  imports only their own declarations, guards selected file provenance and delegates
  to the core-backed empty-child subset. Failed transactions return no partial XML,
  overlays, imported bases, processed digest or source closure.

Limits: 4 MiB raw/processed document; 32 MiB aggregate captured raw bytes;
32 MiB aggregate uniquely prepared XML per owner invocation; 32 MiB direct-visible
expanded XML; 512 captured/prepared sources; 64 direct Includes per prepared document;
4,096 captured edges and 4,096 visible handles. Include depth is bounded at 32 edges,
including cached subtree height. Local inheritance's 32-link and tree-copy's
32-level/8,192-authored-element bounds remain independent guards. Leaf diagnostics
are propagated once so repeated prefixes cannot hide a deep failure's reason.

Source snapshots are confined and checked against captured lengths/SHA-256, not
treated as an atomic filesystem transaction. Root-qualified field recognition is
still separate from path safety/root availability/existence. No native wildcard,
postfix, file-cache rewriting, registry assumptions or production dependency binding
are emulated. Graph final schema validation is mandatory; an invalid final owner
exposes no trusted dependency fields.

## Verification

`SdkInstanceChainsSmokeTest.Run` is registered in the default compiler runner.
All 128 compiler groups pass, along with all 33 enum checks and three classifier
script fixtures. Model/marshaller coverage stays 785/1390 and 762/1390; all 48
EP1-only types retain both models and marshallers. Final build: zero warnings/errors.

Owned tests cover child-first overlays, owner-only export, forbidden transitive
versus explicitly direct ancestor inheritance, independent reproduction of child
processed hashes, per-base closures, diamond coalescing, stale cache reset, cycles,
forged edges, all/reference roles, relative file provenance, expressions, duplicate
handles, non-inheritable types and matched text rejection. They also prove an exact
32-edge chain and rejection on the intended cached-depth gate, prepared-byte
amplification distinct from raw-byte limits, invalid-owner no-fields, profile conflict
and no output publication. CLI duplicate/conflicting/path-only flag probes exit 1.

```text
BinaryAssetBuilder.ManifestInspector.exe sdk-instance-chains-self-test
BinaryAssetBuilder.ManifestInspector.exe sdk-typed-source-graph ra3ep1 <schema-root> <source-root> <entry.xml> <new-output-directory> --instance-chains [--art-root absolute-directory] [--audio-root absolute-directory]
```

## Next gates

Remaining first document-level blockers (check-order evidence, not exhaustive):
134 complex-child identity/directive/expression, three asset directive/expression,
two unknown asset attribute, one duplicate/unsafe child ID, one occurrence bound,
one unsupported Include visibility.

In SkirmishAI/Personalities/AlliedCoopBaseSkirmishPersonality.xml, lines 64–66 use
`xai:joinAction="Remove"` on StrategicState IDs. AlliedBalanced.xml line 69 also
removes a StrategicState. These are concrete candidates for an independently tested
bounded removal subset; they do not prove that all 134 blockers are removal directives.
Review absent-target behavior, matching identity/QName, required fields/cardinality,
source hashing and post-removal schema validation before admitting that scope.

Production type/hash/layout/dependency gates, a minimal packaged build, WorldBuilder
integration and an actual Uprising load test remain separate required workstreams.
