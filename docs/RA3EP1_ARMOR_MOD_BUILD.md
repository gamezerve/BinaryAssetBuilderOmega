# One-command reproduction of the working armor mod

## Scope

`scripts/Build-Ra3Ep1ArmorMod.ps1` builds the fixed, user-tested
`AlliedConYardArmor` GUN-5000 example from current source and its tracked XML.
It is not yet a general compiler accepting arbitrary user XML, an installer,
or a launcher. It does not modify stock files or execute the game.

Run from the repository directory under PowerShell 7:

```powershell
# Reborn: compile current fixed armor XML, compare stock payloads and package the working default-mod recipe without launching.
& .\scripts\Build-Ra3Ep1ArmorMod.ps1
```

The default local stock paths are the unpacked Static Data and WorldBuilder
manifests under `D:\TEMP\Red Alert 3 Uprising Source Data`. Explicit
`-StaticManifest`, `-WorldBuilderManifest` and `-GameRoot` parameters can
select reference locations, but the reviewed installation hash requirements
still apply. No automatic downloads or dependency restores are performed.

The pipeline performs the existing five-step PoC: offline build, armor
profile, real scoped document/OutputManager path, Static golden, and bounded
WorldBuilder golden. It then prepares a fresh default-reader package with
all fifteen entries and runs corruption/no-overwrite controls. Finally it
requires the package to match the SHA-256 of the user-tested package exactly.
All stream bytes, identity and coefficients are therefore preserved.

Output is a fresh `artifacts/RebornArmorDefault-*` directory containing:

- `RebornArmorProbe.big`: three `data\mod{,_l,_m}` stream families plus
  CRLF-only version markers, 3,552 bytes.
- `reborn_1.0.cfg` and an identical mod SKU fragment: one `add-big` line.
- `preparation.json`, `trial-plan.json`, `build.log`, and warning text.
- `build-receipt.json`: XML path/hash, fresh PoC/preparation paths,
  package/config paths and digest, with this build's game-execution and
  observed-override flags false.

The receipt points to the earlier user-observed behavioral evidence without
claiming that every new output has independently been tested in-game. Byte
reproduction is distinct from a new runtime observation. The historical
validated package digest is
`DD7DE09E3A81B9714DC2861ADE0AA9ECDFE72A1851EDD1ED20BE2155EF31C2DC`.

The fixture `tests/fixtures/AlliedConYardArmorProbe.xml` is intentionally fixed.
Changing its values, identity or ordering does not grant a new general build
capability: fixed-profile/source admission or the recipe digest check will
refuse the change. Source mutation during this run also refuses the receipt.
Supporting safely admitted editable XML is the next compiler deliverable;
do not bypass the checks or reuse the old runtime success for changed bytes.

Detached receipt-admission checks are available separately:

```powershell
# Reborn: exercise recipe admission and refusal controls only; no compiler, installation read or game.
& .\scripts\Build-Ra3Ep1ArmorMod.ps1 -SelfTest
```

These require the default route, exact digest/count/marker and complete
false runtime claims. The general production/cache/reuse path stays closed.
The existing launch helper is intentionally pinned to the two previously
reviewed artifact directories; this build command does not retarget it or
launch a newly generated config. Any later game trial needs separate approval.

## Validation — October 11, 2026

The detached self-test passed one admitted recipe and ten refusal controls.
The complete build command then passed all five fresh PoC steps and default
package controls. Its receipt is retained locally at
`artifacts/RebornArmorDefault-1e74dc2145e44e3f8c649ffcf46ec2f4/build-receipt.json`;
the associated fresh PoC is `RebornArmorPoc-246a4cb1ec7144529d6b525fdf4f1dff`.
Its BIG digest exactly matches the previously user-tested default-reader
package. No game or codec ran in this workflow. WorldBuilder comparison used
the existing bounded golden path, not a full large-BIN dump. The full compiler
suite was not rerun for this composition-only script addition.
