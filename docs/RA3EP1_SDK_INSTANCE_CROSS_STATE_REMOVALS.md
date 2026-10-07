# Empty cross-QName state removal admission — October 8, 2026

## Result and limits

Independent `--instance-cross-state-removals` /
`diagnostic-direct-instance-cross-state-removals-v1` advances the real typed
source graph to **393 Validated / 3 RequiresPreprocessing** across 396 documents
and 664 Include edges. Comparison with `--instance-state-readds` (392 / 4)
finds zero earlier-valid regressions; earlier flags and defaults stay unchanged.

Whole `EP1/SkirmishAI/Personalities/CommandersChallenge/AIP_CC_32.xml` now
validates, not merely the isolated owner wrapper reviewed in the previous
[read-only milestone](RA3EP1_SDK_CC32_REVIEW.md). Its authored
`BuildState id="AlliedCaptureTech_MEDIUM"` Remove deletes the unique inherited
StrategicState of that identity, without renaming or rewriting the command.
The owner also removes the ordinary same-QName AlliedTechBuildState.

- Raw source SHA-256: `F9A2ED51CF5A2D3395013A174F0D6D39A5B20625CE5C72F762C71F9FD70D4B77`.
- Prepared graph-owner SHA-256: `2A5398445FB5E4F7ADF885B1B9F638DAAE38F3C19A2B6E67562505AE5F64ECA4`.
- Imported prepared base SHA-256: `701BFD7E212BBC9FB7BA67339E0336D69CF5F852CDC44C4F8A2E4D324FA8DC0C`.

One overlay, one directly imported base and six prepared source witnesses retain
the BasePersonality → CoopBasePersonality → AlliedCoopBasePersonality →
AlliedCoopBaseSkirmishPersonality → AIP_CC_BaseAlliedBalanced → CC32 closure.
The six witnesses identify captured/prepared XML, not native assets or streams.

Repeated audits reproduce the same prepared owner hash. All 396 raw source
fingerprints match and a post-audit hash check finds no changed source.
No reference XML/XSD or Core implementation is edited and no output directory
is created. Audits still exit 2: six path issues and 198 missing AUDIO-header
dependency occurrences remain. ScopedGraphComplete, FullDependencyCoverage and
ProductionBuildReady stay false. Native serialization and game loading remain
unproved; the previous stock manifest reference absence is corroboration only.

## Narrow contract and implementation

`SdkCrossStateRemovals.Prove` requires a direct AIPersonalityDefinition sequence
and exactly one cross-QName collision per owner. The original command must be
an unprefixed EA BuildState with bounded literal id, exact instance-namespace
Remove action, no child nodes and no payload attributes. The fully resolved
base must contain exactly one direct child of that id across all sibling kinds:
an empty StrategicState with an explicit bounded literal State reference.

`ReferenceType` checks optional unbounded named empty AIBuildState and
AIStrategicState branches, no attribute wildcard, required AssetReference State
attributes and their exact schema-authored reference annotations:
AIBuildStateDefinition versus AIStrategicStateDefinition. The witness records
both QNames and both reference types; it does not claim that they are interchangeable.

`Prove` predicts the complete direct StrategicState **and** BuildState projection,
including explicit field presence/values and entry order after ordinary
additions, overlays and removals. `SdkSelfAttributeInheritance.CheckMerge`
permits only the exact proved command node, uses ordered live target presence,
and decrements the matched target branch's occurrence count, not the command's.
The unchanged Core NodeJoiner executes original commands. `Verify` requires
full projection equality and absence of the removed id among all direct children
before any cross-removal witness is published.

Same-owner cross-removal plus ordered Remove/re-add is explicitly refused;
separate contracts alone do not prove their combined order semantics. Reverse
direction, replacement, multiple cross commands, missing/ambiguous targets,
payload and schema-reference drift also remain closed. Existing bounds, raw
duplicate-expression guards, source-closure checks and final whole-owner schema
validation are retained. Failures clear operation/source evidence atomically;
final invalid XML cannot publish trusted dependency fields.

`SdkInstanceInheritanceProfile` carries the new opt-in scope through imported
base preparation. `SdkTypedSourceGraph` and Program keep it mutually exclusive
with earlier CLI profiles. Self-only evidence uses
`diagnostic-self-cross-state-removals-v1`; graph evidence uses the direct-instance
profile above. Prepared imported sources retain their original closure provenance.

## Validation and next work

`SdkInstanceCrossStateRemovalsSmokeTest.Run` covers actual Core state fields/order,
exact reference types, imported closure, source preservation, earlier isolation,
replacement/missing/ambiguous/reverse/multiple/mixed/directive/payload/schema-drift/
stale/late-owner refusals, final invalid-schema zero-fields binding and profile
conflicts. It is compiler test group 143; all 143 groups pass. The 33 enum checks,
three Include-classifier fixtures and three CLI isolation probes also pass.
Final incremental build has zero warnings and errors.

Model/marshaller inventory remains 785/1,390 and 762/1,390; all 48 EP1-only
complex types retain models and marshallers. Manual weighted effort stays
**51% complete / 49% remaining**, not 393/396 usable-mod readiness.

The remaining source-preparation blockers are Music.xml and SoundEffects.xml
(expressions beyond exact `=$NAME`) and Voice.xml (8192-element pre-normalization
tree bound). Next, inventory the exact audio expression forms and Voice tree
shape/size before defining another bounded, independently tested scope.

Read-only initial inventory (no new audio admission):

| Source | Bytes | XML elements | Expression occurrences |
| --- | ---: | ---: | ---: |
| Sounds/Music.xml | 3,073 | 25 | 1 |
| Sounds/SoundEffects.xml | 933,804 | 13,973 | 353 |
| Sounds/Voice.xml | 705,621 | 9,790 | 884 |

Music's expression is `=$TEMP_RA2_VOLUME + 5`. SoundEffects includes arithmetic
such as `=$AMB_MIN_RANGE + 0` (43 occurrences), `=$AMB_MAX_RANGE + 0` (42)
and `=$WEAPON_FIRE - 10` (20), as well as currently admitted exact references.
Voice's observed expressions are exact references: RA3_EVA_MINVOLUME and
RA3_EVA_VOLUME (431 each), FUTURETANK_SHRUNKENVOICE_PITCHMODIFIER and
FUTURETANK_SHRUNKENVOICE_VOLUMEMODIFIER (11 each).

These are syntactic counts, not definition-resolution or arithmetic-semantic
proof. SoundEffects also exceeds the current tree bound, so resolving its first
reported expression blocker alone may expose another refusal. Do not treat the
three remaining files as three trivial edits or lift the bound without a
separate resource and merge-complexity analysis.
