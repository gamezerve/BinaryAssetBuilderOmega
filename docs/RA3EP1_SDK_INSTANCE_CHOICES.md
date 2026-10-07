# Bounded repeated-choice copying — October 8, 2026

Subsequent pipeline investigation is documented in
[consumed inheritance markers](RA3EP1_SDK_INSTANCE_MARKERS.md): core declaration
loading removes inheritFrom before joining, so the AI marker mismatch does not
require an XSD edit. The choice-only results below remain unchanged.

## Real result

The independent `--instance-choices` option selects
`diagnostic-direct-instance-choice-copy-v1`. On the same 396-source / 664-Include
graph used by `--instance-removals`, it advances admission from **374 Validated /
22 RequiresPreprocessing** to **385 / 11**. All eleven owners identified in
[the choice review](RA3EP1_SDK_CHOICE_REVIEW.md) newly validate: ten under
SkirmishAI/Personalities and one EP1 CommandersChallenge owner. There are zero
earlier-valid regressions and zero SchemaInvalid/Limit/StaleSource/SourceRead
documents. The earlier removal-only option still reports 374 / 22.

Both real audits return the expected exit 2 and create no output directory.
There are still 198 unresolved typed file-dependency occurrences. The existing
six path issues and the missing AUDIO header are not solved by particle copying.
ScopedGraphComplete, FullDependencyCoverage and ProductionBuildReady are false.
Official XML/XSD and the core NodeJoiner are unchanged. This is diagnostic source
preparation, not an emitted native stream or a game-loaded mod.

## Precisely admitted scope

This explicit option includes recursive direct-instance preparation,
root-qualified imported files, empty-child matching and strict keyed removal.
Earlier CLI options and constructor defaults retain their previous scope.

New particle admission is limited to a populated **nested** direct
`XmlSchemaChoice` with `MaxOccurs > 1`. Every direct alternative must be an
`XmlSchemaElement` with `MinOccurs = MaxOccurs = 1`. The number of authored direct
children must satisfy the choice particle's aggregate min/max before copying.
The same alternative can occupy multiple choice slots; its unit MaxOccurs is
not mistakenly treated as an overall per-QName limit.

Current tree guards still apply to every copied rule: declared literal
attributes only, unprefixed EA elements, bounded IDs unique across **all** sibling
QNames, no expressions/instance directives/list modifiers, 32-level tree and
8192-element authored bounds. The existing local/source chain, raw/prepared byte,
source snapshot/confinement and direct-only visibility guards are retained.
Final schema validation is mandatory before publishing trusted dependency fields.
An empty required choice is withheld by this final gate, not treated as valid
merely because there were no populated rules to copy.

No new nested matching is enabled. Two-sided top-level sequence overlays still
admit matching only for empty-complex children; a matching populated OpeningMove
branch is refused. Anonymous repeated OpeningMove branches stay separate;
`Name` is not promoted to an id key. Root-level choices, populated singleton
choices, non-unit alternatives, structural nested particles and wildcard items
remain closed. In particular, a populated singleton choice inside
AIStateLinearCombinationHeuristic is not implicitly admitted just because the
outer HeuristicChoice is supported.

## Code and reproducible checks

- `SdkSelfAttributeInheritance.Apply/CheckChildren` performs the new particle
  selection/cardinality checks under `choiceCopy`, profile
  `diagnostic-self-repeated-choice-copy-v1`; the original joiner performs copying.
- `SdkInstanceInheritanceProfile` propagates `choices` through child-first
  preparation, preserving own-only direct exports and raw/processed closures.
- `SdkTypedSourceGraph.Inspect/BindGraph` selects the independent option, rejects
  conflicting profiles and binds final owner-only XML.
- `Program` exposes `--instance-choices` only on `sdk-typed-source-graph`, plus
  `sdk-instance-choices-self-test`.
- `SdkInstanceChoicesSmokeTest` is registered in the compiler test runner in
  addition to the unchanged core characterization group.

Example (PowerShell, from the repository):

```powershell
# Reborn: audit explicit source/schema roots without creating a production output.
$choiceSourceRoot = 'D:\OneDrive\CNC Files\CnC_Modding_Support-main (Official XML, Schema, Script, Shader, Maps)\Uprising\Xml (Uprising)'
$choiceSchemaRoot = Join-Path (Get-Location).Path 'schemas\ra3ep1\xsd'
$choiceOutputPath = Join-Path (Get-Location).Path 'sdk-choice-poc-uncreated-output'
& .\source\BinaryAssetBuilder.ManifestInspector\bin\x86\Release\net8.0\BinaryAssetBuilder.ManifestInspector.exe sdk-typed-source-graph ra3ep1 $choiceSchemaRoot $choiceSourceRoot (Join-Path $choiceSourceRoot 'global.xml') $choiceOutputPath --instance-choices
```

Owned fixtures cover repeated alternatives/order, anonymous branch identity,
chained source/removal witnesses, retained root-qualified file fields, final
required-cardinality rejection with no trusted fields, and atomic refusal of
duplicate/cross-QName/unsafe IDs, unknown/prefixed children, expressions,
directives, singleton/structural/non-unit particles, matched populated branches,
stale children and source cycles. Refusal withholds processed hashes, overlays,
removals, imported bases and prepared-source evidence together.

Three real CLI probes reject conflicting flags, repeated choice flags, and
choice flags on the path-only preflight command (exit 1, no output directory).
The focused choice self-test, **all 131 compiler groups**, 33 enum checks and three
Include-classifier fixtures pass. The full suite requires permission for its
existing owned temporary audio-file move. Final build has zero warnings/errors.
Registered compiler group count is now 131; model/marshaller
inventories remain 785/1390 and 762/1390, including 48/48 EP1-only coverage.

## Concrete source witness

`SkirmishAI/Personalities/AlliedBalanced.xml` now validates with a five-source
closure. Its own overlay targets AlliedCoopBaseSkirmishPersonality; its own
StrategicState Cleanup removal is recorded once.

Raw SHA-256:
`FB1EF8928548425B0C0F03FAB40DAF0D8AE45CE45F1BE2374B296511D7A1B2D8`

Processed owner-only XML SHA-256:
`3D350A817407F5C588FEC2ED80DA09552C175BC0D637929EC61ACF97166A1606`

The imported base's processed hash remains
`988AF5F4DC50DEB9477AF624EAA21238227350E8F8BD7BAFCFB6F8809D62AA7C`,
matching the previous removal milestone. These are XML witness hashes, not native
asset/type identities or proof of current live-disk bytes.

## Remaining eleven first blockers

| Category | Count | Owners relative to Uprising XML root |
| --- | ---: | --- |
| Asset directive/expression | 3 | Sounds/Music.xml; Sounds/SoundEffects.xml; Sounds/Voice.xml |
| Duplicate/unsafe sibling ID | 3 | maps/official/CAMP_S04_Geneva_Bass/AIP_S04_AlliedGroundBase.xml; maps/official/CAMP_A04_Gibraltar_Hayes/AIP_A04_JapanMechaWarfare.xml; EP1/SkirmishAI/Personalities/CommandersChallenge/AIP_CC_01.xml |
| Unknown asset attribute | 2 | SkirmishAI/AITargetHeuristicLibrary.xml; SkirmishAI/AIMicroManagerLibrary.xml |
| Occurrence bound | 1 | GlobalData/Upgrade.xml |
| Include visibility | 1 | GlobalData/ObjectCreationLists.xml |
| Cross-QName ID collision | 1 | EP1/SkirmishAI/Personalities/CommandersChallenge/AIP_CC_32.xml |

These are first-failure classifications, not an exhaustive compatibility list.
Next, identify exact unknown attributes and expression/directive forms against
official schemas and native preprocessing semantics. Do not repair reference
XML or discard conflicting nodes to increase the validation counter. Duplicate
IDs and visibility differences require explicit semantic proofs and independent
admission if justified.

Read-only follow-up narrows the two unknown-attribute cases: the only authored
attribute absent from the corresponding raw named type plus BaseAssetType
attribute declarations is `inheritFrom` (five occurrences in
AITargetHeuristicLibrary.xml, 122 in AIMicroManagerLibrary.xml).
`AssetTypeTargetHeuristics.xsd` defines AITargetingHeuristic and
`AssetTypeAIMicroManager.xsd` defines AIMicroManagerData as extensions of
BaseAssetType, not BaseInheritableAsset. `Base/AssetBase.xsd` declares inheritFrom
only on the latter. Example handles are AITarget_A07_SuperWeapon inheriting
ClosestStructureHeuristic and StandardMicroManager inheriting BaseMicroManager.
This explains the current undeclared-attribute refusal; it does **not** authorize
a schema edit or establish native support for inheritance on these types.
Next, inspect native SDK preprocessing/validation ordering and official schema
evidence before considering any narrowly reviewed override scope.

The manual effort estimate stays **51% complete / 49% remaining**: source
admission is now 385/396, but that ratio is not working-mod readiness. Native
dispatch/type-table compatibility, production build integration and in-game
validation remain open; no automatic percentage increase per test or XML file.
