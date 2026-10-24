# FireWeaponWhenDead source authoring

The existing 148-byte EP1 writer is now connected to flexible source admission
and complete native/metadata verification. The old inherited-building template
no longer restricts the reactor's death conditions or the presence of optional
offsets/probability rows.

Supported fields include `DeathWeapon`, `DelayTime`, `InitiallyActive`,
`ActiveDuringConstruction`, `ContinueToBezierDestination`, `FireAtGround`,
`UseMyKillerAsWeaponSource`, `FocusFireKindOfFlag`, `WeaponOffset`, required
`DieMuxData`, and repeated `WeaponFireProbability` records. Original XSD rules
and the existing inherited upgrade-field processing still apply. Strong weapon
references are resolved against the combined local/stock asset catalog.

The independent stock anchors are the Super Reactor explosion, the generic
infantry catalyst weapon, and SovietMiner's included Terror Drone eggs module
(which supplies a populated probability-list metadata witness). Output uses
freshly compiled source data; no stock death-weapon payload is copied.

Developer regression: `fire-weapon-death-self-test <static.manifest>
<global.manifest>` exercises original reactor data, edited weapon/flags/offsets,
multiple probability rows, omitted optional fields/defaults and corrupt output.
Tests and private Reborn builds do not publish user mod output or launch games.

Validation: three original native/metadata witnesses and six
reactor/edit/default/optional/probability/corruption checks passed. This also
corrected the generic flag verifier: SAGE `ALL` fills the complete mask even
when an XSD explicitly lists `ALL` as a sentinel enumeration literal.

The current Reborn source preparation passes (58 source files, 38 assets).
An isolated compile of its complete `SovietPowerPlantAdvanced` reached the next
separate blocker: nonempty scripted-model `FXEvent` metadata is not enabled in
`Ep1ScriptedDrawTokenizer`. No successful complete reactor or 38-asset package
build is claimed. The broader private regression was stopped after the first
building passed, and the reactor was checked directly to avoid rebuilding
unrelated buildings. User XML and published mod packages were left unchanged.
