# First normal EP1 baseline — October 9, 2026

## Result

The user explicitly authorized **one normal baseline start**, without a debugger,
system-wide recording, mod package or mod arguments. The user subsequently
confirmed that the main menu opened. This is a user-observed menu result,
not automated UI verification or a successful authored-mod loading test.

`scripts/Invoke-Ra3Ep11Baseline.ps1` started the reviewed `RA3EP1.exe` once,
with an empty argument list and the installation root as the child's working
directory. It did not modify the observer's working directory/PATH, attach to
an existing process, retry, or stop the game. Game-written settings/logs were
disclosed before authorization and are not inventoried here.

The reviewed root is:
`D:\Program Files (x86)\Steam\steamapps\common\Command and Conquer Red Alert 3 Uprising`.
Preflight verified launcher, native 1.0/1.1 images, 1.1 SKU and restored campaign
archive SHA-256 identities, absence of the five known loose override roots,
and no existing Uprising-named process. This is not complete stock authenticity.

Observed launcher PID **47260** and direct game child PID **17452**:

- Child executable: `Data\ra3ep1_1.1.game`.
- Child creation: `2026-10-09T20:12:50.5588120Z`.
- 41 qualifying snapshots, from `20:12:51.0953406Z` to `20:13:19.8497926Z`.
- One distinct child PID; no observation failure recorded.
- Visible command line includes the launcher path followed by
  `-config "<installation root>\RA3EP1_English_1.1.SkuDef"`.
- No `-modconfig` or `-runver` was requested in this baseline.

This corroborates default **1.1 executable selection** and the visible default
SKU argument for this installation/run. The snapshots do not prove the actual
config read, effective search paths, all archive loads, or mod-option forwarding.
Parent IDs and creation times were checked, but snapshots are non-atomic,
PID reuse is not fully excluded and indirect descendants are not observed.
The 30-second sampling loop is not a hard timeout for an individual CIM query.

Full local evidence remains ignored, not published:
`artifacts/Baseline-ddd1a541a81e45abbced9d537c3ebb46/baseline.json`.
SHA-256: `FBE73F539ABB61C0B9364119BD337185C2F3BA180805A458AFA5D1A819B3C373`.
The JSON's `MainMenuReachedProven=false` means the observer itself does not
verify UI state; the later human confirmation is recorded in this document.

## Next gate

Prepare a separately authorized inert `-modconfig` launch and observe the
child's exact argument. That establishes forwarding only, not a successful
file read. Actual native config consumption needs a separately scoped read
observation; the rejected WPR filter and helper-only debugger calibration are
not game evidence. Do not silently rerun the game or add instrumentation.

Authored XML emission still needs valid selected-type layout/hash/dependency
and package proof. The overall estimate remains approximately **52% / 48%**:
this removes a launch-selection uncertainty, not the remaining SDK gates.

## Reuse and validation

The script defaults to preflight only. `-Run` starts a real game and requires
new explicit authorization in an interactive workflow. It leaves the game
running; close it normally before another trial. Existing Uprising processes
cause refusal. Identity pins deliberately refuse changed installations.

PowerShell syntax validation passed. The live preflight and sole authorized
baseline completed successfully. Detached immediate-child selector tests in
`Get-Ra3Ep11LaunchObservation.ps1` remain separate from this live evidence.
