# Allied destruction, tech and LUA native module checkpoint

Additional review: [destruction inheritance and DieMux tokens](RA3EP1_DESTRUCTION_BASE_TOKENS.md)
now binds the selected base-class chain, terminal IDs, eight inline fields and
empty/absent storage independently of native-byte equality. This narrows the
earlier inherited-token limitation for this selected module, not general schemas.

Follow-up: the [destruction source gate and walkthrough](RA3EP1_DESTRUCTION_SOURCE_WALKTHROUGH.md)
now independently checks actual Core named-FX/default normalization and semantic
import identity for the selected graph. The captured-index test below is retained
as its original separate diagnostic; general GameObject publication is still closed.

## Outcome and limits

Three native families now have models, marshalers and actual GameObject behavior
dispatch. Four selected `AlliedConstructionYard` instances match the stock native
root/base/inline data and child graphs after explicitly checked pointer rebasing:

| Family | Native class ID | Root bytes | Selected instances |
| --- | --- | ---: | --- |
| BuildingDestructionModuleData | 0D96A2F4 | 72 | ModuleTag_BuildingDestruction |
| TechUpgradeModuleData | FF7BCDB3 | 176 | ModuleTag_TechUpgrade_Tech2 / Tech3 |
| LUAEventListBehaviorModuleData | FA6AC8B6 | 16 | ModuleTag_LUAEventList |

This removes the last three **missing native model names in the original
two-object inventory**, not all SDK compatibility work. Existing models still
need EP1 review. Recursive tokenized GameObject output, broader source/reference
closure, mixed-stream publication and gameplay acceptance remain separate gates.
The exported alpha h is unchanged; these are not newly build-enabled Studio assets.
LUA string serialization does not prove script callback existence or execution.

## Evidence

`Ep1AlliedModuleSmokeTest.Run` captures a bounded 51,520-byte stock instance,
joins own-field tokens to independently observed native offsets, compiles literal
source through real polymorphic behavior dispatch and checks full selected native
graphs plus exact relocation/import slots. Literal XML is validated with the
explicit reviewed `diagnostic-shield-first-v1` / `diagnostic-dummy-hooks-v1`
candidate, not an unmodified clean official schema graph. Full inherited token
tables and nested item tokenization are not proven by the native byte comparison.

Building destruction uses the existing 52-byte Die base and ordered 20-byte
items. Bone strings, FX imports, weak CreationObject identities and velocity are
checked. `FillerMesh` is schema `Poid`/SubObject: invariant lowercase weak identity,
**not case-sensitive StringHash**. The initial comparison caught this difference.
The FX source name/type is checked against the stock dependency identity before
inserting a captured-index diagnostic reference. This is deliberately **not**
actual Core-normalized FX source resolution.

TechUpgrade uses the existing 36-byte Upgrade base and inline 120-byte ObjectFilter.
The two source fixtures reproduce upgrade and four ordered IncludeThing weak IDs,
the filter masks and defaults, RECTANGULAR distance and bone string. Authored
RadiusY=0 remains a present pointer, unlike absent RadiusY. An edited CIRCULAR
variant checks scalar and string changes. General weak-reference existence and
recursive source closure are not established by hashing literal identities.

LUAEventList keeps the Behavior base and required EventListName native string.
Repeat output, edited module IDs/strings, unknown schema attributes, invalid
distance names, missing required fields/items and detached own-token corruption
are refusal/change controls. A separate authored two-item destruction fixture
checks ordering, absent references, default velocity=1 and MaxDebris=2, default
FillerMesh identity and exact relocation slots.

## Files and reproduction

- Models: `source/SageBinaryData/SageBinaryData/Modules/Ep1AlliedModules.cs`
- Marshalers: `source/BinaryAssetBuilder.XmlCompiler/Marshaler.Ep1AlliedModules.cs`
- Behavior dispatch: `source/BinaryAssetBuilder.XmlCompiler/Marshaler.GameObject.cs`
- Evidence: `source/BinaryAssetBuilder.ManifestInspector/Ep1AlliedModuleSmokeTest.cs`
- Official schemas: `schemas/ra3ep1/xsd/Modules/{BuildingDestructionBehavior,TechUpgrade,LUAEventList}.xsd`
- Batch recipe: `scripts/Test-Ra3Ep1Batch.ps1`, case `ep1-allied-modules`

Build the x86 Release inspector, then run
`ep1-allied-modules-self-test <absolute-static.manifest>`.
No game launch, installation mutation, full binary dump or alpha replacement is
performed. The next priority is source/Core FX normalization and recursive native
token/reference integration towards a general GameObject stream, not more
fixed-identity armor recipes.

## Final regression

`artifacts/AlliedModules-b/batch-report.json`: **29/29 Focused groups pass** in
**85.380 seconds**, new Allied gate **7.141 seconds**. The batch build took
5.23 seconds, with zero warnings/errors on this incremental build. All eight
runtime hashes stayed unchanged during workers. No new Full-suite run is claimed.
The retained earlier `AlliedModules-a` report is 28/29: its new authored fixture
omitted schema-required DieMuxData. Adding that child fixed the test input; the
official schema was not weakened to accept it.
