# Complementary upgrade singleton admission — October 8, 2026

## Real result

The independent `--instance-upgrades` profile advances the 396-source /
664-Include graph from **387 Validated / 9 RequiresPreprocessing** to **388 / 8**.
The newly valid document is `GlobalData/Upgrade.xml`. Zero earlier-valid
regressions; no SchemaInvalid/Limit/StaleSource/SourceRead documents.
The older `--instance-filters` profile stays unchanged at 387 / 9.

This now validates the entire prepared Upgrade owner document, not merely the
two isolated fragments in the [earlier review](RA3EP1_SDK_UPGRADE_REVIEW.md).
It does not prove native upgrade encoding, reference hash closure or gameplay.
Audit exit remains 2. Six existing path issues and 198 missing AUDIO-header
dependency occurrences remain; ScopedGraphComplete, FullDependencyCoverage and
ProductionBuildReady remain false. No output directory is created.

Raw source SHA-256:
`A6083FCC3E990CC31EDBA7B4EE7D08C914181C7B2236C25AC7FB8691C2B47206`

Processed whole owner-only XML SHA-256:
`4246BB3BB9ADD787B781395FBDBF253AFA047D7F64E288E782EAB04C343D4102`

This is a whole-owner XML witness, distinct from the earlier isolated-owner
hashes and from native asset/type hashes. The owner has 22 inheritance overlays,
22 consumed markers, one prepared source, two normalization events and zero
typed file-dependency fields.

## Scope

Option/profile: `--instance-upgrades` /
`diagnostic-direct-instance-upgrades-v1`. Delegated local profile:
`diagnostic-self-upgrade-normalization-v1`. Earlier filter/bitflag/marker/choice/
removal/chain/root-file guards compose unchanged; selecting this new option does
not alter the scope or defaults of any earlier flag.

Only inherited, unprefixed EA UpgradeTemplate assets with exactly two direct
GameDependency branches and no other direct child elements are newly normalized.
The effective owner declares an optional singleton GameDependency of exact
named EA GameDependencyType. Its unit sequence must exactly declare optional
unbounded RequiredObject/GameObjectWeakRef, ForbiddenUpgrade/UpgradeTemplateWeakRef,
NeededUpgrade/UpgradeTemplateWeakRef, then optional singleton ObjectFilter.
The type must have element-only content and no attribute wildcard.

Exactly one authored dependency side has reference leaves. The other must supply
actual complementary condition attributes. Attributes on both sides must be
disjoint: even an identical duplicate field is refused. Allowed literal fields
are RequiredModelConditionsAny, ForbiddenModelConditions and RequiredObjectStatusAny;
values are at most 4096 ASCII letters/digits/underscores/spaces. IDs, namespaced
directives, signed modifiers, expressions and arbitrary fields are refused.

There are at most 128 total literal, unprefixed anonymous reference leaves,
each 1–128 ASCII letters/digits/underscores/hyphens/periods. Reference values are
not resolved game dependencies. Nested ObjectFilter payloads, two populated sides,
empty/oversized values, CDATA and text/directives remain closed. A redundant empty
second branch without complementary conditions is not admitted.

The original document tree is capped at 8192 elements before normalization;
the existing 4 MiB source/output, 4096 asset, closure/depth/node/amplification
guards still apply. This extra pre-fold check is applied only to documents with
candidate duplicate upgrade dependencies, so unrelated documents retain their
earlier guard ordering.

## Unchanged core and proof

Before copying, `SdkUpgradeNormalization.Normalize` predicts the disjoint
attribute union and ordered leaf payload. The actual NodeJoiner folds the source
asset against an empty same-attribute node. No reference XML/XSD or core source
is modified. The actual singleton result must match both predicted attributes
and ordered leaves exactly; duplicates are not deduplicated.

This happens before the existing full author-tree checks and inheritance
resolution. Invalid top-level fields and unknown schema content cannot ride
along. Original owner attributes and non-element content are checked before
the core can strip TypeId/directive metadata or execute list modifiers.
Normalized source assets retain their inheritance markers until the
already tested declaration-consumption step. Fully resolved base/derived child
matching rules remain unchanged: this is not general two-populated recursive
branch merging.

Evidence adds `UpgradeNormalizations(Id, Child, Before, After, Leaves, Attributes)`
only for actual source-owner normalization events. Inherited copies are not
counted as fresh events. Any failure atomically withholds transformed bytes,
hashes, overlays and all operation/import/prepared evidence. Final whole-owner
schema binding remains mandatory before trusted file fields are published.

## Real events

| Owner | Branches | Retained reference | Retained condition |
| --- | --- | --- | --- |
| Upgrade_AlliedTech2 | 2 → 1 | RequiredObject: AlliedRefinery | ForbiddenModelConditions: STRUCTURE_UNPACKING |
| Upgrade_AlliedTech3 | 2 → 1 | NeededUpgrade: Upgrade_AlliedTech2 | ForbiddenModelConditions: STRUCTURE_UNPACKING |

## Verification and code

`SdkUpgradeNormalization.Normalize` implements the proof and actual-core result
check. `SdkSelfAttributeInheritance.Apply` captures the original tree bound,
normalizes only candidate owners, then retains existing checks/resolution.
`SdkInstanceInheritanceProfile` carries the independent scope and owner-only
witnesses through child-first preparation. `SdkTypedSourceGraph` and `Program`
expose the separate flag and reject conflicting profiles.

`SdkInstanceUpgradesSmokeTest` is group 136, also exposed as
`sdk-instance-upgrades-self-test`. Owned fixtures cover complementary folding,
reversed order, local chains, retained conditions/duplicate references,
older-profile isolation, duplicate attributes, two populated sides, extra/empty
branches, identity/nested-filter/expression/CDATA/modifier/oversize rejection,
late atomic failure, wrong singleton schema, final invalid-payload zero-fields
binding and untouched source/absent output. Three CLI probes reject upgrade/filter
conflicts, duplicate upgrade flags and the flag on source-only preflight.

The focused group, all 136 compiler groups, 33 enum checks, three Include
classifier fixtures and three upgrade CLI isolation probes pass. Final
Release/x86 build has zero warnings/errors. The final real audit was repeated
after original-owner directive/list-modifier guards were added: the processed
whole-owner hash and 388 / 8 counts remained identical. Git whitespace checks
pass; external reference XML/XSD and core sources remain untouched.

Model/marshaller coverage stays 785/1390 and 762/1390; EP1-only coverage stays
48/48 for both. Manual effort stays **51% complete / 49% remaining**: XML
validation does not close native dispatch, production build or game-load gates.

## Remaining work

Eight first blockers remain: three Sounds directive/expression owners, three
duplicate/unsafe-ID owners, ObjectCreationLists unsupported Include-role
visibility and EP1 AIP_CC_32 cross-QName ID collision. Next, characterize the
ObjectCreationLists visibility requirements against direct/source-backed Include
roles. Do not treat precompiled/reference/all Includes as instance exports
without proving their ownership and dependency semantics.

Initial read-only next-blocker inspection: ObjectCreationLists has one local
inheritance (JUOdessaSupportVehicle_Die_OCL from JULightTransportVehicle_Die_OCL)
and an `all` Include of DATA:GlobalData/GlobalDefines.xml. That target contains
80 definitions, zero asset exports and zero Include children, plus Tags metadata.
A future definition-only Include exception may avoid granting any instance
visibility, but requires its own bounded captured-source/metadata/export proof;
the role guard was not relaxed in this milestone.
