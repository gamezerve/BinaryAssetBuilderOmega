# Pinned CC32 source/core/native-metadata review — October 8, 2026

## Outcome and limits

New read-only command:

```text
sdk-cc32-review <absolute-Uprising-source-root> <absolute-EP1-global.manifest>
```

This is a fingerprint-pinned review of one known source, not a new preprocessing
profile. Real graph admission stays **392 Validated / 4 RequiresPreprocessing**;
CC32 still refuses cross-QName child-ID collision. No source/schema/core edits,
native stream emission, output directory or in-game proof. Manual effort remains
**51% complete / 49% remaining**.

## Source and unchanged-core evidence

Owner: EP1/SkirmishAI/Personalities/CommandersChallenge/AIP_CC_32.xml.
Raw SHA-256:
`F9A2ED51CF5A2D3395013A174F0D6D39A5B20625CE5C72F762C71F9FD70D4B77`.
Different bytes are rejected rather than granting generic cross-QName authority.

The base closure is prepared by the existing state-readd profile, not by manually
flattening XML: BasePersonality, CoopBasePersonality, AlliedCoopBasePersonality,
AlliedCoopBaseSkirmishPersonality and AIP_CC_BaseAlliedBalanced. Each captured/
prepared identity is reported and rechecked before publication. The target is
StrategicState/AlliedCaptureTech_MEDIUM, introduced by the skirmish personality.
Resolved base SHA-256:
`701BFD7E212BBC9FB7BA67339E0336D69CF5F852CDC44C4F8A2E4D324FA8DC0C`.

CC32 authors BuildState/AlliedCaptureTech_MEDIUM with instance joinAction=Remove.
The existing graph refusal is checked first. Then a separate isolated experiment
consumes only the owner's inheritance marker, as declaration loading does, and
passes the original removal command to the unchanged NodeJoiner. Its ID-only
lookup removes the inherited StrategicState: one target before, zero after.
The isolated merged owner validates against the pinned effective EP1 schema.
Isolated prepared XML SHA-256:
`5B59965EBB7D4043727AD0DF86033908E16E322E63E305C2749B25CA2C22B0CA`.
This hash identifies the review wrapper, not graph admission or native bytes.

The two schema branches have different reference types: AIStrategicState.State
references AIStrategicStateDefinition; AIBuildState.State references
AIBuildStateDefinition. A payload-free removal does not supply a replacement
reference. Do not infer that cross-QName payload replacement is safe, and do not
rename the original XML merely to satisfy current guards.

## Native metadata corroboration

Input:
`D:\TEMP\Red Alert 3 Uprising Source Data\Global Data\data\global.manifest`.
Metadata SHA-256:
`08A415789062B1707EDBA3C456884B94791097505431E17A03D10EE34DF050FD`.
The parsed metadata validates and matches EP1 version 7 / AllTypesHash 0x5454A8E9.

| Evidence | Value |
| --- | --- |
| Native owner | AIPersonalityDefinition:AIP_CC_32_AlliedEnemy |
| Type / instance | 0xD6D4F18E / 0xDA2BCC6F |
| Native type hash | 0xC6B1A12F |
| Instance payload length | 3804 bytes |
| Manifest reference count | 109 |
| Target | AIStrategicStateDefinition:AlliedCaptureTech_MEDIUM |
| Target type / instance | 0x242FF6D4 / 0x443E7645 |
| Target pair among owner references | Absent |

The target asset must exist uniquely in the manifest for absence to be reported;
a missing target is not treated as evidence. Matching uses both type and
instance ID. The target's absence corroborates the XML/core removal outcome,
but it does not itself decode the BIN state list, resolve its import words or
prove native-layout/serializer equivalence. NativeStateLayoutVerified and
ProductionBuildReady remain false. The review reads at most 16 MiB of metadata
and 4 MiB per source; it does not read or dump the full native BIN.

## Verification and next step

`SdkCc32Review.Review` retains the existing confined captured source resolver,
base preparation, schema admission, source rechecks and an uncreated output
path. Metadata is fingerprinted from exactly the parsed bytes and rechecked.
This is consecutive snapshot evidence, not atomic filesystem locking.

Registered group 142, `sdk-cc32-review-self-test`, proves exact target identity,
wrong-type/same-instance and same-type/wrong-instance nonmatches, true presence,
ambiguous/missing target refusal. Existing core sibling fixtures continue to pin
destructive cross-QName removal/replacement and unchanged atomic graph refusals.
The real read-only review and unchanged full graph audit were also executed.
Repeating the review reproduces its source/output hashes and native reference
absence. Two CLI probes reject missing/extra arguments before any review.
All 142 compiler groups, 33 enum checks and three Include-classifier fixtures
pass; final Release/x86 build has zero warnings/errors. README/status are English.

Next: a separate narrowly bounded removal-only admission that requires one
existing resolved target, reviewed command/target QName and schema reference
types, literal empty command, source witnesses and predicted actual core result.
Keep cross-QName replacement, missing/ambiguous targets, arbitrary directives
and source edits closed. Native layout and game validation remain separate gates.

Models/marshallers remain 785/1390 and 762/1390; EP1-only coverage remains 48/48
for both. Four first blockers remain: Music/SoundEffects expressions, Voice's
element inventory bound and CC32 cross-QName removal. This review reduces
uncertainty but does not claim that any complete SDK or mod now works in-game.
