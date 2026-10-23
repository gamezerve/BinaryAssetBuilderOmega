# BuildingBehavior source support

<!-- Reborn: enable the existing native writer with independent stock/source/metadata proof, not fixed LIGHT_01/LIGHT_02 examples. -->
The complete GameObject route now admits `BuildingBehavior`. Its existing
40-byte native model and writer are reused unchanged. Four optional ASCII
subobject-name attributes are supported independently:

- NightWindowObject
- FireWindowObject
- GlowWindowObject
- NightFireWindowObject

Attributes may be omitted or explicitly empty. Names are not restricted to the
Power Plant examples. Multiple modules retain separate local IDs, native
storage, string allocations and metadata. These names identify subobjects inside
a model; they are not global asset references and do not create assets named
`LIGHT_01` or `LIGHT_02`.

Both original SovietPowerPlant BuildingBehavior modules independently qualify
the native field offsets, local/class IDs, inheritance and string values against
stock data. Generated metadata is checked against the learned stock grammar and
the source-bound native coordinates, then checked again in the enclosing
GameObject stream. Native ownership includes all string bytes, NUL terminators,
padding and relocations; no imports are invented.

Implementation: `BuildingBehaviorContract.cs`,
`ConscriptVariableBehaviorSource.cs`, `ConscriptVariableGraphVerifier.cs`,
`ConscriptModuleScalarContract.cs` and `Ep1BehaviorMetadataTokenizer.cs`.

## Verification scope

Source preparation of Reborn with the Power Plant and refinery enabled completes
across 17 source files and produces two GameObjects. The requested W3X include
comments remain in the user's Power Plant file; no compiler action silently
skips an active missing include. This change does not enable ART/W3X resource
compilation or validate the existence of subobjects inside a compiled model.

The `windows` regression filter exercises original Power Plant modules,
schema-default/absent strings and two edited modules containing all four fields,
including an explicit empty string. Final reports record complete stream/package
results and independent native/metadata corruption controls.

<!-- Reborn: record measured complete-package results separately from the current authored-mod stop. -->
Release/x86 compilation passes with zero warnings/errors. The final `windows`
batch passes all three complete-object stream/BIG cases, rejecting 224 native
and 70 metadata faults. Evidence:
`alpha/.work/buildingbehavior-final-20261023-a`.
An explicit empty string may legally target adjacent NUL padding; its pointer
control therefore uses a provably invalid target rather than requiring one
canonical allocation address. Source content, ownership and relocation checks
remain mandatory.

The actual two-object Studio backend validation proceeds past BuildingBehavior
and stops at `UpgradeTemplate:Upgrade_PowerPlantTurbine` in PowerUpgrade.
Evidence: `alpha/.work/buildingbehavior-studio-check-20261023-b`.
The name also appears in the original EA Power Plant XML but no matching asset
was found in the current static/global catalogs or copied source definitions.
Upgrade trigger fields are schema-typed weak references. The subsequent
[PowerUpgrade correction](RA3EP1_POWER_UPGRADE.md) changes absent-target failures
to located dependency warnings and preserves native hashes, without inventing an
upgrade or dropping the module. That correction passes three complete package
tests with unresolved weak names. The earlier two-object run above remains
failure evidence, not a claim of successful packaging.

Developer checks use private `.work` output. They do not publish a Documents mod,
change game files or launch a game. The user's entry point remains
`alpha/EALAModStudio.exe`; offline build success is not gameplay evidence.
