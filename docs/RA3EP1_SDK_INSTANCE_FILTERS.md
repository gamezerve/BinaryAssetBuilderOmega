# One-sided matched ObjectFilter copying — October 8, 2026

## Real result

The independent `--instance-filters` profile advances the 396-source /
664-Include graph from **386 Validated / 10 RequiresPreprocessing** to **387 / 9**.
The newly valid owner is `SkirmishAI/AIMicroManagerLibrary.xml`; there are zero
earlier-valid regressions and no SchemaInvalid/Limit/StaleSource/SourceRead
documents. The earlier `--instance-bitflags` scope remains unchanged at 386 / 10.

Expected audit exit is 2, not success for a production build. Six path issues
and 198 unresolved typed dependency occurrences of the missing AUDIO header
remain. ScopedGraphComplete, FullDependencyCoverage and ProductionBuildReady
are false. No output directory is created. External XML/XSD, native processors
and the core NodeJoiner are unchanged; no native stream or game-load proof.

Raw owner XML SHA-256:
`46665EB746F2E88BCFCD4B315209E65A5FCB523996BC5F01B634344A5475FF4E`

Processed owner-only XML SHA-256:
`D7A97496E82B56427338F74487491B6646965E212483AD9CCED44F9E1FF42AB3`

These are diagnostic XML witnesses, not native asset/type hashes.

The owner records 122 overlays, 122 consumed inheritance markers, one prepared
source, 12 matched filter events (seven empty, five populated), 15 copied leaf
references, no bitflag operations and no typed file-dependency fields. A repeat
audit confirmed the same processed hash and counts.

## Reviewed scope and reason

The library contains 13 authored IgnoreTargets blocks, five with child payloads.
An element-only ObjectFilter cannot be treated as a schema-empty leaf: doing so
would ignore IncludeThing/ExcludeThing reference content. The new gate instead
proves one-sided copying before invoking the unchanged core, then compares the
actual result's ordered `(Name, Value)` leaves to the prediction. Duplicate
references are preserved, not normalized or deduplicated.

Option/profile: `--instance-filters` /
`diagnostic-direct-instance-filters-v1`; local delegated scope:
`diagnostic-self-object-filter-copy-v1`. It composes earlier bitflag/marker/
choice/removal/chain/root-file scopes without changing their defaults or the
imported BaseInheritableAsset requirement.

Only matched **AIMicroManagerData / IgnoreTargets** branches are newly admitted.
The parent declaration must be optional singleton and have exact named EA
ObjectFilter element-only content, no attribute wildcard, and a unit sequence
of optional repeated IncludeThing then ExcludeThing, both exact named EA
WeakReference simple types. Arbitrary owners, names, schemas and recursive
populated merges remain closed.

At most one side can contain element payloads. Each payload has at most 128
unprefixed EA, attribute-free leaf elements. Values have 1–128 ASCII letters,
digits, underscores, hyphens or periods. Nested nodes, directives, expressions,
CDATA, empty/unsafe/oversized values and two populated sides are refused.
Existing author-tree checks, singleton/cardinality, 4 MiB output, closure/depth/
node bounds and final schema validation remain mandatory. Weak-reference values
are recorded literals, not resolved GameObject dependencies.

Filter attributes are overlaid by the existing core; missing fields retain base
values. Derived-only payloads are copied; base-only payloads survive a derived
attribute update. Two empty payloads are allowed. Evidence adds
`Filters(Type, DerivedId, BaseId, ChildName, PayloadSource, Leaves)` for actual
matched owner operations only. Failure withholds all transformed bytes/hashes
and overlay/marker/removal/modifier/filter/import/prepared evidence atomically.

## Populated real witnesses

| Derived owner | Source | IncludeThing count |
| --- | --- | --- |
| VindicatorMicroManager | derived | 5 |
| CenturyBomberMicroManager | derived | 5 |
| TanyaMicroManager | derived | 1 |
| A02_CoopAttackMicroManager | derived | 2 |
| A03_Coop_AttackMicroManager | derived | 2 |

The first two retain the same ordered five infantry names. Tanya retains
SovietPowerPlantAdvanced; A02 retains SovietAntiStructureShip then
SovietAntiNavyShipTech2; A03 retains SovietBaseDefenseAdvanced then
SovietBaseDefenseAir. Descendants inheriting those prepared branches keep their
payloads; they are not counted as fresh copy commands without a matched override.

## Implementation and validation

- `SdkFilterCopies.Prove/Leaves`: compiled-schema and bounded payload proof.
- `SdkSelfAttributeInheritance.CheckMerge/Resolve`: narrow owner/child gate,
  actual core call, ordered result check and atomic witnesses.
- `SdkInstanceInheritanceProfile`: independent child-first preparation scope.
- `SdkTypedSourceGraph` / `Program`: separate option and profile conflicts.
- `SdkInstanceFiltersSmokeTest`: owned fixtures, registered as compiler group
  134 and exposed as `sdk-instance-filters-self-test`.

Focused fixtures prove three-level resolved chains, ordered duplicate references,
base/derived payload copying, attribute overlay/retention, older-profile refusal,
two-sided and late atomic failure, unknown/attributed/nested/CDATA/unsafe/oversized
leaves, arbitrary branch and repeated-singleton rejection, final SchemaInvalid
with zero trusted fields, untouched source and absent output. CLI probes reject
filter/bitflag conflicts, duplicate filter flags and filter flags on source-only
preflight with exit 1 and no output.

The focused group and all 134 compiler groups passed, as did 33 enum checks,
three classifier fixtures and the three CLI probes. The full suite needs
permission only for its existing owned temporary audio-file move.
The final incremental Release/x86 build completed with zero warnings and errors.

Model/marshaller coverage is unchanged at 785/1390 and 762/1390; EP1-only coverage
is 48/48 for both. Registered compiler groups: 134. Manual engineering effort
stays **51% complete / 49% remaining**; the source-validation ratio does not
measure working mods.

## Remaining first blockers and next step

Nine first blockers remain: three Sounds directive/expression owners, three
duplicate/unsafe-ID owners, Upgrade child cardinality, ObjectCreationLists
unsupported Include visibility, and EP1 AIP_CC_32 cross-QName ID collision.
Next, characterize the Upgrade occurrence failure against the actual schema and
core; do not weaken cardinality checks or alter reference XML to make it pass.
Native type/dispatch/reference closure, production build integration and in-game
validation remain separate migration workstreams.

Initial read-only Upgrade inspection identifies Upgrade_AlliedTech2 and
Upgrade_AlliedTech3: each authors two GameDependency children, while
`schemas/ra3ep1/xsd/AssetTypeUpgradeTemplate.xsd` declares maxOccurs="1".
The first contains RequiredObject=AlliedRefinery or
NeededUpgrade=Upgrade_AlliedTech2; the second carries
ForbiddenModelConditions=STRUCTURE_UNPACKING. This is an authored XML/schema
conflict, not evidence that the occurrence guard is incorrect. Establish stock
compiler normalization or source-export provenance before proposing a repair;
silently dropping either branch would lose upgrade requirements.
