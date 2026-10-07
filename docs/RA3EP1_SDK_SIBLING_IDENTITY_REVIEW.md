# Sibling identity operation review — October 8, 2026

## Outcome

The four remaining AI identity first blockers are not one generic duplicate-key
problem. Read-only inspection distinguishes identical duplicates, ordered
Remove/re-add command pairs and a cross-QName removal. The new registered
`SdkSiblingIdentitySemanticsSmokeTest` group (139 total) pins the unchanged core's
behavior with owned fixtures. It does not enable a new normalization profile.

The repeated real `--instance-expressions` audit remains **389 Validated / 7
RequiresPreprocessing** over 396 documents. Expected exit 2; no output directory.
ScopedGraphComplete and ProductionBuildReady remain false. Reference XML/XSD,
core implementation and identity admission guards are unchanged. No complete
prepared owner, native stream or game-load proof is inferred from these fixtures.

## Source-backed classification

Paths are relative to the explicit Uprising `Xml (Uprising)` source root:

| Source | Observed identity shape |
| --- | --- |
| maps/official/CAMP_S04_Geneva_Bass/AIP_S04_AlliedGroundBase.xml | Two identical StrategicState entries keyed AIState_S04_AlliedGroundBase_SimpleAttack_Water_HARD, with the same State and HARD BRUTAL Difficulty. |
| maps/official/CAMP_A04_Gibraltar_Hayes/AIP_A04_JapanMechaWarfare.xml | BaseDefenseController and HarvesterController each have Remove followed by a populated same-QName state, in four owners: eight command pairs. |
| EP1/SkirmishAI/Personalities/CommandersChallenge/AIP_CC_01.xml | AlliedCapturePriorityTech_HARD has Remove followed by a populated StrategicState with EASY MEDIUM HARD BRUTAL Difficulty. |
| EP1/SkirmishAI/Personalities/CommandersChallenge/AIP_CC_32.xml | BuildState/AlliedCaptureTech_MEDIUM Remove targets an inherited StrategicState with that ID, not a BuildState. |

The CC32 direct base AIP_CC_BaseAlliedBalanced inherits
DATA:SkirmishAI/Personalities/AlliedCoopBaseSkirmishPersonality.xml. The latter
declares StrategicState/AlliedCaptureTech_MEDIUM with MEDIUM Difficulty. A
direct-base-only text comparison misses this transitive source of the collision.

Raw SHA-256 witnesses, in table order:

- `5B7152D39DB209028FC0EABC134466C6AD65DE3EBB53AFC7D1C12F3E8DA061AC`
- `5C3E79122A53CFB3A66AA81408FE6D9D329A444BCE28653EFAC3C193CD97DC5B`
- `1BCEABDA58DE8BC85FE036193556BCD70C3D07A95357C9D0C91D186A5901509B`
- `F9A2ED51CF5A2D3395013A174F0D6D39A5B20625CE5C72F762C71F9FD70D4B77`

CC32 direct base raw SHA-256:
`BF93A12A78BE9F79652D21A5B87CB504D7854CE3C44816D1016B2009F100A89C`.
These are snapshot identities, not native asset hashes.

## Core evidence and regression tests

`Core/SageXml/NodeJoiner.SelectMatchingKey` compares id values without QName.
`SelectSame` uses this lookup for repeated sequence children. In
`ReplaceXmlNode`, a QName mismatch or Remove action deletes the selected node.
Same-QName repeated keys merge attributes, allowing later values to overwrite
earlier ones. `AppendCorrespondedXmlNode` places newly added children according
to sequence order and insertPosition, whose default is Bottom.

`sdk-sibling-identity-semantics-self-test` proves:

- Identical repeated empty-complex state entries coalesce into one.
- Conflicting same-key attributes use the later value; final XML still validates.
- Remove followed by re-add discards old-only attributes and moves the entry
  behind surviving entries of that kind.
- A shortcut ordinary overlay retains old-only attributes and the old position:
  it is not equivalent to the command pair.
- Reversing re-add/Remove leaves the entry absent. Operation order is semantic.
- A BuildState Remove can delete a StrategicState sharing its ID; cross-QName
  replacement can also discard the earlier node while final validation passes.
- Existing removal/upgrade/expression-stage local scopes still refuse these
  duplicate or cross-QName inputs atomically, without partial operation evidence.

Owned fixtures model the observed operation shapes, not the complete real AI
owners or their full reference closure. The real schema declares repeated
StrategicState/AIStrategicState and BuildState/AIBuildState with different
State reference types. A schema-valid result alone does not prove intended AI
behavior. Do not rename the real CC32 element automatically.

## Next bounded implementation

Start with a new independent profile limited to identical empty-complex
StrategicState repeats on AIPersonalityDefinition: exact effective attribute and
payload equality, bounded literal ID, no directives/expressions/nested payload,
captured source witnesses, predicted core result and final schema validation.
Do not admit conflicting repeats or change the older uniqueness gate.

Handle Remove/re-add separately. A future proof must resolve the actual base,
require an existing exact QName/key, execute the ordered pair, predict lost and
retained attributes, preserve reference order and record both operations.
Deleting the Remove command or merging the two authored entries is insufficient.
Cross-QName commands remain closed pending an explicit compatibility decision
and source-backed reference/type review.

All 139 compiler groups, 33 enum checks and three Include-classifier fixtures
pass. Final Release/x86 build has zero warnings/errors; whitespace checks pass.
All four real owner hashes were rechecked unchanged after the review.
Model/marshaller coverage is unchanged at
785/1390 and 762/1390; EP1-only remains 48/48 for both. The overall manual estimate
stays **51% complete / 49% remaining**. This review reduces implementation risk;
it does not close an additional source owner or the native/game gates.
