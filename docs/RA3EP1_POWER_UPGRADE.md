# EP1 PowerUpgrade (legacy)

`PowerUpgrade` (`PowerUpgradeModuleData`, TypeId `0xC3A62C68`) is a legacy upgrade
module. It is not used by any EP1 stock object, but the class is still in the game
and applies the owner's `EnergyBonus` when its upgrade triggers. The SDK admits it
as a reviewed behavior family.

- **Layout:** the record adds no field to `UpgradeModuleData`. It is 36 bytes: the
  8-byte module header, the optional `CustomAnimAndDuration` pointer, the
  `TriggeredBy` and `ConflictsWith` weak lists, five SageBools and 3 padding bytes.
- **Identity:** a non-empty `id` is required. RA3 does not run an anonymous
  PowerUpgrade, and the shared module-identity contract already rejects a blank id.
- **Proof:** there is no stock PowerUpgrade instance to qualify against. The verifier
  therefore builds a comparison-only witness from a real stock upgrade module whose
  direct base is `UpgradeModuleData`. It relabels that module's class identity and
  keeps only its inherited `UpgradeModuleData` metadata row. The same native graph
  comparer and upgrade metadata proof used by other upgrade modules then check
  every byte, list, optional animation and metadata row. Real output is never
  changed. The witness refuses a basis whose base padding is not zero.

## Verification

Cases `power-edited`, `power-defaults` and `power-multiple`, with source controls for
a missing id, an invalid flag and an undeclared child. Edited/multiple cases
also include an unresolved `RebornMissingUpgrade` trigger to verify that the
native list retains its exact hash without creating a new upgrade or import.

<!-- Reborn: honor schema/Core weak-reference semantics without hiding missing upgrade diagnostics. -->
`TriggeredBy` and `ConflictsWith` are `UpgradeTemplateWeakRef` fields, serialized
as typed asset-ID hashes. Core's weak-reference loop tolerates an unresolved
target (`AssetDeclarationDocument.cs`, `AddOutputInstance` dependency handling).
The independent native verifier now does the same; it still rejects an ambiguous
known catalog target and checks every authored hash/list length. The dependency
audit checks both emitted mod assets and stock assets and reports missing upgrade
names as `MISSINGASSET` warnings with source coordinates, like other covered weak
references. Strong references remain errors when missing.

This removes the extra hard stop at `Upgrade_PowerPlantTurbine`, which is present
in the original EA Power Plant XML but absent from the configured stock catalogs.
A preserved trigger hash does not mean the upgrade exists or can be granted at
runtime; that missing-target warning remains useful for mod authors.

<!-- Reborn: record actual weak-reference regression evidence separately from gameplay. -->
Release/x86 build passes without warnings/errors. The dependency audit self-test
passes, including located missing-upgrade warnings, local/stock target union,
strong-reference failures and stale-source controls. The actual Reborn audit
passes with one warning at `SovietPowerPlant.xml(481,6)`: 2 mod assets plus 25229
stock identities, 143 checked references, 10 deferred; all 6 string uses resolve.
Evidence: `alpha/.work/weak-upgrades-reborn-audit-20261023-a.json`.
The `power` batch passes all 3 complete-object stream/BIG cases, including an
unresolved trigger, with 219 native and 58 metadata faults rejected.
Evidence: `alpha/.work/weak-upgrades-power-20261023-a/module-graph-report.json`.

The actual Studio backend validation now passes for the unchanged authored
Power Plant and refinery together (17 source files, 2 explicit stock overrides).
The receipt records `OfflineValidated=true`, `NewIdentity=false`, primary
`GameObject:SovietPowerPlant`, 103 primary reference occurrences and metadata
prefix 32796. Archive SHA256:
`F085AE1D4F25EE35E6A1ED70E725BB45B0256A38F7935C131012DDE3864B6E7F`.
Evidence: `alpha/.work/weak-upgrades-reborn-20261023-a/conscript/override/trial-plan.json`.
The driver rechecked every source hash before success. Validation-only mode did
not publish to Documents or launch the game. The stable alpha tools were synced;
the user's existing Studio entry point picks up the correction on its next build.

The game behavior (EnergyBonus applied in Uprising) still needs an in-game check.
