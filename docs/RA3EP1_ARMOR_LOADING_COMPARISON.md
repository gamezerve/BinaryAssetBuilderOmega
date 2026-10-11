# Armor trial: no observed effect and loading-route comparison

## Outcome — October 11, 2026

The first candidate reached the menu, but the user reported normal damage
after attacking the Allied Construction Yard. The expected GUN increase was
not observed. This is a failed behavioral acceptance check, not proof of
which loading stage failed. The user confirmed a newly started Allied
skirmish and a Peacekeeper attacking the Construction Yard. Reference
`Allied/Units/AlliedAntiInfantryInfantry.xml:1274` binds
`AlliedAntiInfantryInfantryShotgun`, and `GlobalData/Weapon.xml:3265`
specifies `DamageType="GUN"`. These files are under the user's Uprising
`Xml (Uprising)` reference directory, not edited inputs. Thus wrong weapon
category and an old save are not supported explanations for this test.
The user closed
the game; no second launch occurred in this investigation.

`scripts/Get-Ra3Ep1ArmorLoadingComparison.ps1` now reproduces a read-only
comparison of the user's EA-SDK RA3 Reborn and deneme stream packages against
the tested Uprising candidate. It caps directory metadata at four MiB,
selected manifests at four MiB and relevant raw markers at 64 bytes. It reads
no BIN payloads. Two complete runs produced identical JSON, retained locally
at `artifacts/ArmorLaunch-62ad536da6014faca2ad86c71628f6f5/loading-comparison.json`.
This script is a bounded trusted-local reference audit, not a hostile archive
validator or engine emulator. The general compiler suite was not rerun.

| Contract | RA3 Reborn 1.07 and deneme 1.00 | Tested Uprising candidate |
| --- | --- | --- |
| Main stream | `data\mod.manifest` | `data\reborn_armor_probe.manifest` |
| Suffix families | `mod`, `mod_l`, `mod_m` | `reborn_armor_probe`, `_l`, `_m` |
| Version markers | Each mod family has CRLF only (`0D0A`) | None |
| Manifest references | `static.manifest`, `global.manifest`, `audio.manifest`, all non-patch | None |
| Target format | RA3 v6 / `54EEE764` | Uprising v7 / `5454A8E9` |
| Config fragment | `mod-game 1.13` and two `add-big` lines, no `add-manifest` | One `add-big` plus explicit `add-manifest` |

The inspected deneme Streams archive contains 35 entries and no
`camp_a01_brightonbeach_smith/map.version` entry. A direct audit of that
guessed path correctly refused it. The earlier screenshot must not be
assumed to identify this particular archive; its map stream may be elsewhere.
Reborn has 1,865 entries; directory sizes are 114,897 and 1,304 bytes.
The Uprising candidate has twelve entries and a 488-byte directory.

## What this does and does not explain

The reviewed Uprising driver `0064D440` separately creates default
`static.manifest` and `mod.manifest` readers and handles config-queued
readers conditionally. Our differently named candidate exercised the queue
route, not the default `mod.manifest` namespace. That is a concrete routing
difference worth isolating. It is not evidence that the queue failed at runtime.

The existing reader probe checks an effective manifest path before its
alternative `.version` branches. A missing marker alone therefore cannot be
declared the cause. CRLF-only markers observed in these two RA3 mod families
do not select `_mod`; the user's suffix example remains valid for other
families, not a universal required mod marker.

Stock manifest references are another difference. Core
`OutputManager.CommitManifest` serializes reference inclusions as non-patch
references and `BasePatchStream` separately as a patch reference. These
contracts must not be conflated. The standalone armor has no imports; copying
RA3's three references blindly would not prove correct Uprising override
precedence. v6 headers and `mod-game 1.13` must not be copied into EP1.

## Next isolated candidate

Prepare a separate workspace package using the **same** admitted v7 armor
bytes, but the default `data\mod{,_l,_m}` namespace and matching CRLF-only
markers. Its short config should mount the BIG without also queuing that
same manifest, avoiding two reader routes for the same asset. Preserve the
first candidate and its evidence. Treat namespace/marker coverage as one
default-route experiment, not a certified fix. Do not simultaneously invent
patch references, change damage values, replace stock streams or relax
compiler identity guards.

Before any separately authorized game trial, validate all generated entries,
unchanged armor/header/checksum bytes, exact config, image pins and absence
of existing Uprising processes. Test a fresh yard with a confirmed GUN
weapon; collect startup and gameplay results separately. If no effect remains,
establish actual config/BIG reads and then inspect reader/asset replacement
ordering rather than expanding general debugger scaffolding. All effective
load and override claims remain unproved.
