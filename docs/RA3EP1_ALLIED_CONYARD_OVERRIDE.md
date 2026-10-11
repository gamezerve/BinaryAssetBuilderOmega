# Reborn: AlliedConYardArmor GUN override probe

Date: October 11, 2026. User-selected gameplay target; offline package only.

## Requested change

`tests/fixtures/AlliedConYardArmorProbe.xml` now defines the existing asset
`ArmorTemplate:AlliedConYardArmor`, InstanceId `8B801765`. The only changed
damage coefficient is `GUN`, from `Percent="100"` to `Percent="5000"`.
Percentage marshalling yields a multiplier of 50.0 rather than 1.0. If the
override loads and applies, this makes the target more vulnerable to GUN
damage, not more resistant. Actual in-game damage/caps are not measured.

The source reference has been located at:

```text
D:\OneDrive\CNC Files\CnC_Modding_Support-main (Official XML, Schema, Script, Shader, Maps)\Uprising\Xml (Uprising)\GlobalData\Armor.xml
D:\OneDrive\CNC Files\CnC_Modding_Support-main (Official XML, Schema, Script, Shader, Maps)\Uprising\Xml (Uprising)\Allied\Structures\AlliedConstructionYard.xml
```

The yard's ArmorSet selects AlliedConYardArmor. Its armor definition inherits
BaseFactionStructureArmor. To remain inside the characterized standalone
armor compiler scope, the probe flattens that parent's thirteen entries;
it does not enable general inheritance support or modify the reference XML.
All other coefficients, ordering and scalar defaults are preserved.

## Validation

`Ep1ArmorDocumentSmokeTest.WriteConYardOverride` compiles a baseline copy with
GUN restored to 100. When an explicit static manifest is supplied, it requires
an exact **856 BIN + 8 RELO** byte match to the stock AlliedConYardArmor asset.
The existing golden reader now also checks the name-derived InstanceId.
Only bounded asset slices are read; no full BIN dump is taken.

The actual override goes through fresh schema/Core processing, the real armor
plugin and scoped OutputManager intermediate commit, manifest and link writers.
Checks require the existing TypeId/InstanceId, a 50.0 GUN float at tokenized BIN
offset 788, byte equality with baseline outside that four-byte slot, identical
relocations and zero imports. Package readback/corruption/no-overwrite controls
remain active. The old empty/repeat/edit/populated diagnostic controls remain
unchanged; the actual writer/package now uses this ConYard override instead of
the unused Ep1ArmorTokenProbe.

`Invoke-Ra3Ep1ArmorPoc.ps1` supplies the static manifest to the document step.
All five steps must pass to report `BehaviorOverridePrepared=true`; this means
prepared authored bytes, not a loaded mod. The report records target/damage,
100/5000 values and keeps `OverrideAppliedInGame=false`,
`LaunchConfigurationPrepared=false`, `GameExecuted=false` and `ModLoaded=false`.
The direct document command retains a fixture-only two-argument mode, which
does not claim a stock comparison; the runner does not use that fallback.

Successful local artifact: `RebornArmorPoc-26d2de6b80754cb5a6b38bd7529d19d9`.
All five steps pass. The four-entry `RebornArmorProbe.big` is now 1,222 bytes,
SHA-256 `001599D45018CF1BBECB3909A3C27E2B05DC0B11226B0C41441C6F8D466DEFDB`.
Its stream names remain `data\reborn_armor_probe.*`; stream names are separate
from the intentionally reused stock asset identity. No `.version`, SKU or
installation change is introduced by this request.
Fresh repeat `RebornArmorPoc-35b206bcf4ce41f9aa1f39a07f600b17` also passes
all five steps and yields identical archive size and SHA-256. The full existing
165-group suite is not rerun for this focused target change; the last full
pass is the earlier OutputManager milestone. No new group is counted.

## Remaining game-test gate

The selected ID is suitable for attempting an override, but identical ID alone
does not prove manifest loading or precedence over stock. Next prepare the
effective stream/config route and a separately authorized controlled game test.
An appropriate behavioral comparison is otherwise identical GUN attacks against
a deployed Allied construction yard, with the same attacker and game settings.
Do not infer a weapon's damage type merely from its visual appearance.
The current package should not be installed as an already validated mod.
