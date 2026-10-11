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

## Default-reader candidate prepared

The next isolated candidate is now prepared at
`artifacts/RebornArmorDefault-abb50afbef8745af952b8cc879c43aa9`.
It contains fifteen BIG entries: three unchanged four-file v7 stream families
under `data\mod{,_l,_m}`, each followed by its own CRLF-only `.version`.
The BIG is 3,552 bytes, SHA-256
`DD7DE09E3A81B9714DC2861ADE0AA9ECDFE72A1851EDD1ED20BE2155EF31C2DC`.
Both `reborn_1.0.cfg` and the mod SKU fragment contain only
`add-big RebornArmorProbe.big` followed by LF. There is no explicit manifest
queue entry, no added stock reference, no patch base and no stock-file change.
This is a routing experiment, not a proven fix or a playable SDK release.

Reproduce under PowerShell 7 from the repository directory:

```powershell
# Reborn: generate a fresh default-reader candidate from successful ConYard PoC evidence; no game launch.
& .\scripts\New-Ra3Ep1ArmorTrial.ps1 -DefaultReader -PocReport .\artifacts\RebornArmorPoc-31a740f7284746ab971f5b2539e286e7\readiness.json
```

The CLI exposes separate `armor-default-trial-prepare` and
`armor-default-trial-controls` commands. Source admission is shared with the
original candidate and still reproduces the actual authored GUN-5000 bytes.
`ArmorDiagnosticPackage.Serialize` has a restricted opt-in default-route mode;
the default four-entry serializer and twelve-entry queue mode remain intact.
`ArmorTrialPreparation.Verify` independently checks fifteen names, contiguous
ranges, complete stream bytes, three markers and exact config fragments.

Completed validation:

- Build and default-route preparation/control checks passed, including occupied
  destination refusal and restored CFG/SKU/BIG corruption tests.
- Independent `stream-variant-audit` passed for `data/mod`, `data/mod_l` and
  `data/mod_m`: each v7 manifest and sidecar header has checksum `33DF1C99`.
- Existing queue controls passed. A newly prepared queue package retained
  its exact earlier SHA-256; using queue controls against the default candidate
  deliberately refused the different config. An initial regression invocation
  with relative directory arguments correctly refused them; rerunning with
  required absolute paths passed.
- A fresh five-step PoC passed at `RebornArmorPoc-31a740f7284746ab971f5b2539e286e7`;
  its original four-entry BIG remained 1,222 bytes with the same digest.
- `Invoke-Ra3Ep1ArmorTrial.ps1 -DefaultReader` passed **without `-Run`**:
  reviewed stock identities/campaign digest, archive presence, candidate
  digest/config and absence of existing Uprising processes were checked.

The launch helper now selects only one of the two fixed reviewed candidates;
it accepts no arbitrary path or caller-provided hash. Its original queue route
is unchanged. `-DefaultReader` alone does not launch. A future separately
authorized `-DefaultReader -Run` would start exactly once with the stock
launcher and installation working directory, retaining read-only candidate
handles during observation. At preparation completion no game had been started
with this second package; the subsequent authorized launch is recorded below.
The full compiler suite was not rerun in this focused change.

## Authorized default-reader launch

After separate user approval, `Invoke-Ra3Ep1ArmorTrial.ps1 -DefaultReader -Run`
repeated the preflight and started the stock launcher exactly once. Artifact:
`artifacts/ArmorLaunch-c550944c19c8488ab90d16d4eaaa72cd/launch.json`.
Launcher PID 36784 was recorded, with no observation failure. Thirty-three
direct selected 1.1 child snapshots matched the second candidate's config
argument. No debugger, installation writes, retry or process termination
occurred. These snapshots establish command-line visibility only, not config
reads, manifest loads or effective override. User menu/gameplay feedback is
pending; no success claim is made.
