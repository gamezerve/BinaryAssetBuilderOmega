# Skirmish AI root authoring

<!-- Reborn: document actual compiled root capabilities separately from the AIUpdate module's remaining chooser topology. -->
`SkirmishAIInformation` is now source-owned on the complete GameObject route.
It may be omitted, authored empty/defaulted, or populated with implemented
attributes and optional records. It is no longer compared to Conscript's fixed
`BaseBuildingLocation=FRONT` example.

Supported storage includes:

- Existing scalar/default attributes, including `BaseBuildingLocation`, resource
  type, rotation, distances, damage metrics and packed booleans.
- Optional DPS, inner/outer target distances, defending anti-mask and required
  dodge-status pointees. Absence remains distinct from an explicit zero mask.
- Optional `ConstructionBaseSelectionCriteria` and
  `ResourceNodeBaseClosenessTestSelectionCriteria`: four base-type masks and the
  sort ordinal. `PRIMARY_TYPE_MAX` aliases `CAPTURED` according to the official
  schema `forceValue`; subsequent names do not acquire an extra bit.
- Optional `DodgeWithUnits`, using the independently supported ObjectFilter
  representation.

`AIContextualPower` is **not implemented** by this root processor; a populated
entry reports an explicit unsupported-native-model error. This is not support
for every AI child family.

## Existing AIUpdate values

The already implemented `AIUpdate.AutoAcquireEnemiesWhenIdle` flag list and
`StateMachine` enum now participate in scalar admission and independent native
checking. The EP1 writer already supported them; missing descriptor mappings
had incorrectly retained stock-value equality. This does not open arbitrary
chooser or contact/vision list topology.

## Verification

Stock metadata proves the criteria's four flag spans at offsets 0/4/8/12 and
sort ordinal at 16. Root qualification uses original SovietRefinery plus
AlliedSupportAircraft and AlliedInfiltrationInfantry captures. ObjectFilter
grammar comes from the refinery's real EngineerContain witness.

The expanded complete-object stream/package case passed with both criteria,
optional scalar/mask pointers, a dodge filter and changed AIUpdate mask/state.
It rejected 40 native and 21 metadata faults. Evidence:
`alpha/.work/skirmish-expanded-20261023-c`.
The complete `skirmish` batch passed all four cases (original refinery criteria,
schema defaults, omission and expanded data), rejecting 111 native and 75
metadata faults. Evidence: `alpha/.work/skirmish-batch-20261023-d`.
Original Conscript passed separately (5 native/10 metadata faults), and all
three sound/shadow/projected-root regressions passed (54 native/54 metadata
faults), under `skirmish-original-20261023-d` and
`skirmish-roots-regression-20261023-d` respectively. These are offline tests,
not gameplay evidence or a fresh full historical regression batch.

## Current Reborn boundary

Source-only preparation now passes the SkirmishAI root and AIUpdate mask/state
differences. The next observed failure is
`Soviet/structures/SovietRefinery.xml(763,5)`:
`AI/AIUpdate/UnitAITargetChooserData` was absent, whereas the comparison contract
expected Conscript's chooser. This historical boundary has since been removed by
the [AIUpdate chooser contract](RA3EP1_AIUPDATE_CHOOSER.md); the next observed
failure is the refinery's AudioArrayVoice entry count. Full refinery
build/publication and gameplay are not claimed. User sources, published mods and
game files are not changed by the developer diagnostics.

Implementation: `GameObjectExtendedRootContract.cs`,
`Marshaler.Ep1RemainingRoots.cs`, `Ep1RemainingRoots.cs`,
`ConscriptModuleScalarContract.cs`, `SchemaNativeContract.cs` and
`StockMetadataGrammar.cs`.
