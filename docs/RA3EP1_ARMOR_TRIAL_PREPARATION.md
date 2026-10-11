# AlliedConYardArmor controlled-trial preparation

## Outcome — October 11, 2026

The GUN 100 → 5000 override now has a workspace-only candidate loading
fragment and a twelve-entry BIG4. Preparation and detached checks pass.
Preparation started no game and modified no installed files. A subsequent
separately authorized single launch reached the main menu (user observation).
Neither config consumption nor an in-game armor override has been demonstrated.

Latest preparation artifact: `artifacts/RebornArmorTrial-0f2a34bd39dd4eef9946d7e3b818f57f`.
Its `preparation.json` records the input PoC, installation pins, archive
presence, build result, control result and deliberately false runtime claims.
Generated artifacts are local evidence, not committed distribution files.

## Candidate files and route

`reborn_1.0.cfg` contains exactly two commands (UTF-8 without BOM, LF):

```text
add-big RebornArmorProbe.big
add-manifest data\reborn_armor_probe.manifest
```

`reborn_1.0.skudef` has identical bytes. It is a **mod fragment**, not a
replacement for the installed stock SKU. The proposed test uses the CFG
with `-modconfig`; no installed SKU, search path, executable selection or
stock archive list is replaced. The basename identity `reborn` respects
the characterized short-name limit. Its `1.0` denotes the mod version,
not the selected game version: the installation selects `ra3ep1_1.1.game`.

The archive contains `.manifest/.bin/.relo/.imp` families named
`data\reborn_armor_probe`, `data\reborn_armor_probe_l`, and
`data\reborn_armor_probe_m`. All three carry the same admitted armor payload;
armor coefficients do not vary with these quality suffixes. The suffix
coverage follows the characterized reader variants, not proof of all
possible `.version` or `_vN` fallback branches. No `.version` is generated.
Engine case matching, effective lookup and asset override precedence still
require runtime evidence.

Candidate BIG: 3,650 bytes, SHA-256
`3FE964292FA72A1DF961EC93D3CFDA884E1334F55ACB294FF75FE1E4144F3F85`.
The original four-entry diagnostic package remains 1,222 bytes with its
previous digest unchanged.

Static evidence: [config consumer](RA3EP11_CONFIG_CONSUMER.md) resolves an
`add-big` relative to the config base and queues `add-manifest`;
[manifest queue](RA3EP11_MANIFEST_QUEUE.md) describes conditional dispatch;
[reader variants](RA3EP11_READER_VARIANTS.md) characterize the suffix slots.
These findings motivate the candidate; they do not establish its runtime
consumption. Later startup configuration may affect effective behavior.

## Reproduce preparation

Run the existing armor PoC first, then use its successful `readiness.json`
with PowerShell 7 from the repository directory:

```powershell
# Reborn: prepare only; no game execution or installation writes.
& .\scripts\New-Ra3Ep1ArmorTrial.ps1 -PocReport .\artifacts\RebornArmorPoc-6438741639f84ee0bdde8464f8bbd5c2\readiness.json
```

Use your newly generated report path if that local example is absent.
There is intentionally no `-Run` switch. Requested launch arguments are
recorded as data only. Output goes into a fresh GUID-named artifact folder;
existing destinations are refused.

Relevant implementation:

- `scripts/New-Ra3Ep1ArmorTrial.ps1`: strict PoC target admission, source
  digest, reviewed installation pins, stock archive presence, known loose
  override absence, offline build and preparation report.
- `ArmorTrialPreparation.ReadSource`: exact ConYard identity, manifest
  profile, reproduced compiler bytes, linked headers and checksum.
- `ArmorTrialPreparation.Prepare/Verify/Controls`: isolated publication,
  twelve ordered entries, exact config text and restored corruption tests.
- `ArmorDiagnosticPackage.Serialize`: opt-in three-family serialization;
  its default four-entry behavior is unchanged.

The reviewed launcher, selected 1.1 image and English stock SKU hashes
matched. All fourteen stock SKU archive paths existed, and the previously
identified loose map overrides were absent. This is presence/identity
evidence, **not authenticity verification of every stock archive payload**.

## Checks completed

Two fresh preparations passed build and detached controls. Controls refused
an occupied output directory and corruptions of each config fragment and
the BIG, restoring the original candidate bytes afterward. A package for
the old unused armor identity was refused before destination creation.
Independent archive inspection validated all three linked v7 manifests.
A fresh five-step armor PoC passed after the serializer change, retaining
the original package digest. The full 165-group compiler suite was not
rerun in this preparation step; its earlier result is not a new test here.

## Next gate: one separately authorized game trial

Before launch, repeat image/config/package checks and inspect existing
game processes. Do not touch an unrelated running process. Use the stock
launcher with the installation as working directory, and the exact
candidate CFG as `-modconfig`. Observe the actual child image/arguments;
do not infer config consumption merely from command-line presence.
No debugger, global trace, installed-file replacement or automatic retry
is part of this proposed trial.

A main menu proves startup only. The gameplay check must compare the same
Allied Construction Yard under the same GUN attack against a normal
baseline. This armor coefficient changes from 1.0 to 50.0, so incoming
GUN damage should be substantially greater if the override is effective;
other gameplay mechanics can affect exact timing. A normal menu with
unchanged damage is not success. Report no window, error, or unchanged
behavior separately; close the test normally rather than auto-killing
existing processes. Runtime load/override claims remain false until
the corresponding observation exists.

## First authorized candidate launch

`scripts/Invoke-Ra3Ep1ArmorTrial.ps1` admits only the fixed, reviewed candidate
directory and digest above. Without `-Run` it is preflight-only. With `-Run`
it repeats the existing strict stock baseline preflight, checks stock archive
presence, locks the exact candidate CFG/BIG read-only, refuses existing
Uprising processes and starts the launcher once. It observes only the direct
selected 1.1 child for approximately thirty seconds. No retry, debugger or
process termination is implemented; the inert baseline script was not relaxed.
This fixed-path trial is not a general-purpose mod launcher.

The first sandboxed preflight could not access CIM process information and
stopped before launch. The approved out-of-sandbox preflight passed, followed
by exactly one authorized launch. Artifact:
`artifacts/ArmorLaunch-62ad536da6014faca2ad86c71628f6f5/launch.json`.
Launcher PID 45516 was recorded, with no observation failure. Thirty-seven
direct `ra3ep1_1.1.game` snapshots matched the candidate `-modconfig` argument.
The user confirmed the main menu opened. A separate `user-observation.json`
retains that statement; it is not a process-derived assertion.

Command-line visibility and a menu do not prove config reads, manifest loads
or effective asset replacement. The user subsequently attacked the yard and
reported normal damage, then closed the game. The expected behavioral effect
was not observed; attacking-unit damage type and fresh-match conditions have
not yet been confirmed. The observer never stopped the game and changed no
installation files. See [the follow-up comparison](RA3EP1_ARMOR_LOADING_COMPARISON.md).
