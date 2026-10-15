# Selected destruction inheritance and DieMux token review

This checkpoint extends the manually implemented Item checks from commit
`21ab5b89`. It validates selected stock metadata relationships, not a new asset
processor or general GameObject tokenized writer. Alpha capabilities are unchanged.

## Verified contract

`DestructionBaseTokenReview.Inspect` independently follows:

`BuildingDestructionModuleData → DieModuleData → BehaviorModuleData → ModuleData`

Every base edge must use marker 1 and the expected class-name hash. Tables must
be aligned, terminate inside the token prefix and not cycle/alias previously
visited tables. The concrete wrapper and native class word must identify
0D96A2F4. The terminal TypeId/id tokens must point to native +0/+4 with their
observed type tags. Prefix bounds come from the captured object's KindOf and
BuildTime root anchors, not hardcoded absolute file positions.

DieModuleData's DieMuxData token has type 0084001B and leads to an inline field
table, not a native allocation pointer. All eight fields are checked:

| Field | Type tag | Target relative to module native root |
| --- | --- | --- |
| VeterancyLevels | 6D66AE18 | List-token header binding native +8 |
| ExemptStatus | A7BD811E | Absent token target 0; native +16 is null |
| RequiredStatus | A7BD811E | Absent token target 0; native +20 is null |
| DamageAmountRequired | 645864F3 | +24 |
| MinKillerAngle | 4E16EA4D | +28 |
| MaxKillerAngle | 4E16EA4D | +32 |
| DeathTypes | 034DF1A1 | +36 |
| DeathTypesForbidden | 034DF1A1 | +44 |

The selected VeterancyLevels list is empty: the count token must bind +8, native
count +8 and item pointer +12 must both be zero. These empty/absent relationships
are checked independently, before existing full native byte/relo/import comparison.

## Refusal controls and integration

The reviewer supplies identified type/target/marker/native-storage word positions.
Each is independently changed on a fresh in-memory stock copy, leaving installed
or unpacked files untouched. The actual comparison path must reject every copy.
Field mutation slots are found by name, not source declaration order. The manual
BonePrefix shifted-target control remains independent.
The 38 identified-word mutations are joined by an explicit self-cycle and an
explicit base-table link into native storage, for 40 inherited/inline refusals.

Both the captured-index `ep1-allied-modules-self-test` and actual Core normalized
`ep1-destruction-source-self-test` call the strengthened comparison routine.
This makes the new checks shared rather than a detached demonstration.

## Evidence boundary and next work

The evidence is one selected AlliedConstructionYard destruction module. It does
not admit nonempty VeterancyLevels payloads, present Exempt/RequiredStatus masks,
other death modules, arbitrary inherited module schemas or generated token tables.
These tests consume stock tokens; they do not write them. General source closure,
mixed streams and publication/gameplay remain open. Broader recursive token writer
integration and representative present optional/list cases remain separate work.

Implementation: `source/BinaryAssetBuilder.ManifestInspector/DestructionBaseTokenReview.cs`.
Regression integration: `Ep1AlliedModuleSmokeTest.Compare` and its detached-copy
controls in `Run`. Reproduce through the existing 30-group Focused batch, without
game launch or installation changes.

## Final regression

`artifacts/DestructionBase-a/batch-report.json`: **30/30 Focused groups pass** in
**96.211 seconds**. Allied module checks take **6.363 seconds**, actual Core
destruction source checks **3.748 seconds**. All 40 inherited/inline corruptions
are rejected in addition to the earlier Item corruption. The batch incremental
build takes 4.61 seconds with zero warnings/errors; eight runtime hashes remain
unchanged during workers. No game runs or installation files change. No new
Full-suite result, alpha package expansion or gameplay acceptance is claimed.
