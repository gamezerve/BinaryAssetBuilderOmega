# Source-driven nested upgrade fields

<!-- Reborn: new structure support changes real native allocations and metadata, not only XML admission. -->
UpgradeModuleData-based Behaviors already covered by the alpha compiler can now
author their nested trigger lists and optional animation record independently
of the captured Conscript example. Use the same `alpha/EALAModStudio.exe` and
normal EA XML/includes/definitions/inheritance workflow.

## Authoring contract

- `TriggeredBy`: zero or more existing UpgradeTemplate names, in authored order.
- `ConflictsWith`: zero or more existing UpgradeTemplate names, in authored order.
- `CustomAnimAndDuration`: absent or one nested record. `AnimState` uses the
  official ModelConditionFlagType; `Frames` and `Trigger` are SageUnsignedInt.
  Omitted numeric attributes use zero. Omitted AnimState retains native enum zero.
- Decimal, short/full hexadecimal and the official `0.0` unsigned-zero spelling
  are accepted. Definitions are resolved before native/source verification.

For example, inside an existing EA-namespaced GameObject Behaviors container:

```xml
<!-- Reborn: variable upgrade lists and optional animation use ordinary EA source syntax. -->
<ModelConditionUpgrade id="ModuleTag_FactionUpgrade" AddConditionFlags="USER_8">
    <CustomAnimAndDuration AnimState="USER_8" Frames="0xC" Trigger="0.0" />
    <TriggeredBy>Upgrade_JapanFaction</TriggeredBy>
    <TriggeredBy>Upgrade_AlliesFaction</TriggeredBy>
    <ConflictsWith>Upgrade_Veterancy_HEROIC</ConflictsWith>
</ModelConditionUpgrade>
```

Either list can be omitted entirely. CustomAnimAndDuration can also be omitted;
that removes its native allocation and metadata table, not merely its source
tag. The official sequence still applies: animation, triggers, then conflicts.
Repeated upgrade entries are preserved, not silently deduplicated.

Names must resolve to stock UpgradeTemplate assets in this alpha build route.
Producing new UpgradeTemplate assets, arbitrary GameObject families, other nested
list shapes, and W3X/TGA/DDS/audio compilation remain separate implementation work.
This is not a claim that every nested field in every module is now supported.

## Independent validation

ConscriptUpgradeStructure derives expected weak hashes and animation words from
source plus the catalog. SelectedNativeGraphComparer verifies exact count,
presence, bytes, alignment, allocation ownership and relocation membership.
Neither a stock list cardinality nor stock upgrade spelling controls admission.

UpgradeStructureMetadataProof reads captured field/base/tag grammar and checks
the actual output tree context by context. Variable list item markers bind to
independently walked native item addresses. The optional animation record has
three reviewed scalar rows plus its terminator. Every metadata row must be
owned exactly once, with no hidden tables, wrong targets or overlaps. The full
native tail and RELO/IMP transformations are verified separately. The whole
GameObject still passes graph-qualified manifest/BIN/RELO/IMP and BIG readback.

The former 4096-item tokenizer quota is replaced by item stride/native-buffer
bounds and checked allocation sizes. Other existing memory/depth/output safety
bounds remain; this does not assert unlimited engine/runtime capacity.

## Regression cases

| Case | Metadata bytes | Native bytes | Strong reference occurrences |
| --- | ---: | ---: | ---: |
| Three triggers + two conflicts + animation | 88920 | 39388 | 275 |
| Trigger/conflict lists and animation omitted | 88812 | 39356 | 275 |
| Animation with explicit values | 88872 | 39372 | 275 |
| Animation with omitted/default attributes | 88872 | 39372 | 275 |
| Animation with short hex and unsigned-zero lexical forms | 88872 | 39372 | 275 |
| 5000 triggers + animation | 148860 | 59368 | 275 |

All listed cases produce real stream/BIG packages; they are not XML-only tests.
Direct corruption cases cover list counts/hashes, nested scalar words, field
tags, item markers, native targets, optional presence and record terminators.
The final six stream/package cases reject 46 native and 146 metadata corruptions,
including direct calls into the independent flexible metadata oracle. Each also
rejects nested unsigned overflow at the original Frames attribute before native
allocation. Short hex handling was corrected in the shared uint marshaler;
ordinary decimal/default output remains unchanged.

The live-driver suite additionally combines the new structures with variable
Behaviors, a changed weapon, a new object ID and an explicit production command.
It checks list/optional-record omission, actual instance inheritance and
definitions; genuine missing upgrades and invalid Frames values fail with source
coordinates and retain the last good package. All nine live cases pass, and the
developer's source is restored afterward. User Reborn sources/output are not
modified. No game is launched; runtime behavior of the new animation/list values
has not been independently observed in-game.
