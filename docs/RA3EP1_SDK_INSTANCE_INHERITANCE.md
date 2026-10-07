# Narrow direct-instance inheritance — October 7, 2026

## Outcome and real witness

The separate `diagnostic-direct-instance-inheritance-v1` profile admits a narrow
source-backed imported-base case. `Sounds/Voice_RetailOnly.xml` now validates after
**102 owner overlays** using `AudioEvent:BaseUnitResponse` from its direct instance
Include of `Sounds/BaseSoundEffect.xml`. This base document's local chain was
already admitted by the tree profile. No native processor or game loader is run.

Owner raw XML SHA-256:
`74971BEBA21DE6EC602204C273525740750160CCAB93FF5EDE26D5D9DABD9DCF`.
Processed diagnostic owner XML SHA-256:
`5260B109F8F2233D204B571EBEEE0761B40CF0E19467823AF271570878807063`.
Imported source raw/processed XML witnesses are respectively
`69AF7CD3D9C7DDACA8B394AC7976CF4690BD2501E73716110E650950247D86EC` and
`B8D9330F77506C6C57C54E59B5B559C37945CCBE59ABDF491AF4F91424634950`.
These are serialized XML/source identities, not native asset or production-cache hashes.

The global.xml graph improves from 205/191 to **206 validated / 190
RequiresPreprocessing / zero SchemaInvalid**, with 396 documents and 664 Include
edges. Zero earlier-valid regressions, no limits/stale/read failures, six original
path issues and one typed unresolved AUDIO header. Command exit 2, requested
output absent; FullDependencyCoverage and ProductionBuildReady remain false.
Sound children are symbolic AudioFile references, not proven resolved audio payloads.

## Core rules and deliberately restricted scope

Core `AssetDeclarationDocument.FindInstance` prefers Self, All, Tentative, then
External. `OverrideInstance` permits imported inheritance only for an inheritable
Tentative instance whose defining document is directly included. `ValidateInstances`
derives the IsInheritAble flag from schema BaseInheritableAsset descendants.

The diagnostic profile proves only a subset where this visibility is unambiguous:

- Each owner Include used for external lookup must be an exact captured,
  source-backed direct `instance` edge. All/reference/precompiled visibility is closed.
- Every imported document must contain no child Include edges. No transitive All,
  Tentative or reference visibility is guessed.
- Imported document-local inheritance is expanded using the existing tree profile
  and the entire imported document must validate. Needed handles must exist among
  its direct asset declarations. Cross-source duplicates reject instead of first-wins.
- The selected named type must derive from the reviewed schema's
  BaseInheritableAsset; spelling alone is not eligibility.
- Selected imported bases must contain **zero schema-selected FileReference fields**
  after source-local expansion. Defining/consuming file-path provenance remains closed.
- Local handles take priority and do not import a shadowing base. Same-handle imported
  overrides, cross-type inheritance, expressions and populated-child merging remain closed.

If no external handle is needed, the earlier local tree path is used without
asserting an imported closure. Non-inherited documents retain the previous bounded
definition subset. This does not implement the whole production Include pipeline.

## Implementation and command

```text
BinaryAssetBuilder.ManifestInspector.exe sdk-typed-source-graph ra3ep1 <absolute-schema-root> <absolute-source-root> <absolute-entry.xml> <absolute-new-output-directory> --instance-inheritance [--art-root absolute-directory] [--audio-root absolute-directory] [absolute.manifest=runtime.manifest ...]
BinaryAssetBuilder.ManifestInspector.exe sdk-instance-inheritance-self-test
```

The flag is exclusive with all other preprocessing flags and refused on the path-
only command. `SdkTypedSourceGraph` constructs one `SdkInstanceInheritanceProfile`
for its bounded run. `Capture` checks canonical confinement, captured length and
SHA-256; `Apply` compares every direct edge against freshly parsed raw XML and
resolves only paths already authorized by the source inventory.

Source-local imported chains are expanded/validated first. `Inheritable` checks
effective schema ancestry. An isolated selected-base binding proves absence of
imported physical file fields. Only needed bases are temporarily injected into an
in-memory owner declaration for the existing core-backed tree overlay. Their
already-expanded local inheritFrom marker is removed during delegation; imported
base declarations are removed afterward. Owner declarations/order and raw source
files remain untouched. No imported asset is accidentally emitted as owner output.

`SdkSelfAttributeInheritance.Evidence.ImportedBases` reports type, base ID, source
path and raw/processed imported-document fingerprints separately from overlay
handles. Local profiles report an empty imported witness array. The final owner
digest is recomputed after temporary-base removal; it is not the injected staging
document's digest. Rejection withholds bytes, processed digest, overlays and imported
witnesses atomically. Final owner schema validation still independently gates
typed dependency fields; invalid owner values produce SchemaInvalid with no fields.

At most 64 direct Includes, 4,096 direct visible handles and 512 captured sources
are considered. Raw captures and aggregate expanded base documents are each capped
at 32 MiB; source/owner/staging XML has the existing 4 MiB limits. Pre-injection size,
tree depth/node bounds and pre-merge amplification guards remain. Snapshots are
not atomic across files; no current-disk/native-cache equivalence is claimed.
Resource checks stat existence/length only; no payload dump is performed.

## Remaining first blockers and verification

178 documents first hit the Include-free imported-source gate. This is a changed
check order: it does not mean those documents' expressions, particles or other
assets are now supported. Earlier tree/ID first-failure counts are not directly
comparable with this imported-first distribution. The remaining first blockers are
three populated-child merges, three owner directive/expression gates, two unknown
asset attributes, one child occurrence, one child directive/expression, one delegated
child directive/expression, and one imported file-field provenance gate.

`PathMusic/PathMusicEvents.xml` explicitly stays blocked on inherited physical file
fields; consumer-relative resolution is not silently substituted. Populated-child
merge blockers include Sounds/MissionDialogue.xml and SkirmishAI/Personalities/
CoopBasePersonality.xml and SoloBasePersonality.xml. Transitive base documents and
broader expression/visibility support need separate implementation and proof.

Owned fixtures cover source-local and owner-local chains, imported witness hashes,
temporary-base removal, Self priority and typed consumer-local paths. Negatives
cover all/reference/transitive Includes, non-inheritable types, imported file fields,
populated sides, invalid bases/owners, same-handle/cross-type/missing handles,
duplicate same/different sources, expressions, stale owner/base bytes, forged edge/
inventory, incomplete graphs, direct-Include cap, amplification and profile conflicts.
All 125 default groups and 33 enum checks pass. Inventories remain 785/1,390 models,
762/1,390 typed marshallers and 48/48 EP1-only coverage for both.

Weighted engineering effort stays about 50.25%, rounded 50% complete / 50% remaining.
Next work is defining/consuming field provenance and broader source-backed visibility,
followed by actual populated-child matching. Native types/processors, streams,
missing AUDIO/ART sources, WorldBuilder integration and in-game loading remain gates.
