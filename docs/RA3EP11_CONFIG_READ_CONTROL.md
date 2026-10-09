# Non-game file-read control: helper validated, capture blocked

**Superseded live-capture status:** the user's administrator capture succeeded,
but [actual ETL inspection](RA3EP11_READ_CONTROL_RESULT.md) found the negative
helper and unrelated processes in the record. The scope is rejected and
`-Record` is now disabled. Do not rerun the historical administrator command
below; it documents the experiment that produced this counterexample.

## Actual results on October 9, 2026

The first scoped WPR start attempt returned **Access is denied / 0x80070005**.
The requesting Windows token is not in the Administrator role. Approval to
run outside the filesystem sandbox does not supply an elevated Windows token.
No successful trace start, saved ETL, game execution or cancellation of another
session occurred. The initial attempt compiled two small local executables
before that failure but did not execute them.
An additional read-only `-status -instancename` query for that exact failed
instance returned `WPR is not recording`; no cancel command was needed.

The runner was then hardened to detect this prerequisite before compiling
or starting anything in recording mode. Detached source/scope checks and four
private refusal fixtures pass. A separate **helper-only** validation ran both
distinct executables successfully: each reported its own checked PID and
`READ|1|10` after one managed read of the single-LF fixture. Both processes
exited successfully within their five-second helper deadlines. This proves
the controlled read workload, not the ETW process filter or event correlation.

No raw trace exists to decode from this experiment. Live filter validation,
path-to-file/read/Irp/completion correlation and loss checking remain open;
the game trace candidate's live gates remain false.

## Files and modes

- `fixtures/ra3ep11/phase-a/ConfigReadControl.cs`: small .NET Framework helper;
  `ConfigReadControl.Main` rejects nonabsolute, reparse or non-one-byte inputs,
  reads one byte and checks LF. It does not launch or load a game.
- `fixtures/ra3ep11/phase-a/config-read-control.wprp`: same requested provider,
  keyword mask, event IDs and memory settings as the game candidate, but with
  an explicit `RebornConfigReadControl.exe` process-name filter and separate
  collector/profile names. The negative executable uses the identical source
  but the distinct name `RebornConfigReadNegative.exe`; it must not match.
- `scripts/Invoke-Ra3Ep11ConfigReadControl.ps1`: default is preflight only;
  `-SelfTest` uses detached faults; `-ValidateHelpers` compiles/runs only the two
  local helpers; **`-Record`** requires an elevated Windows PowerShell 7 token
  before compiling or attempting WPR recording. Modes are mutually exclusive.

`Assert-Ep11ControlInputs` pins LF-normalized source SHA-256
`195B7403B1D92BB7B9A1D18C135BC1B21381E9C6CC46281FCBB00FA415BC6B4C`
and canonical profile scope SHA-256
`607C3DD144805B3FAC0F3C7C2FA5F59DAFF85DF361A2374EE25177769C274FD6`.
It rejects DTD/external XML resolution and oversized texts. Scope/source
faults never reach a live trace command. Reviewed inputs are rechecked after
compilation before recording metadata/start commands.

The runner uses the installed Windows .NET Framework `csc.exe`, not an EA
compiler or native game codec. Generated executables, stdout and any later ETL
stay in a unique subdirectory of the already ignored `artifacts` tree. These
are local disposable control artifacts, not a mod package; none are committed.
No previous output is overwritten or recursively removed.

`Invoke-Ep11OwnedReadHelper` uses a hidden window, validates the returned PID,
limits helper stdout and waits at most five seconds per helper. On a helper
timeout it terminates only that newly created owned child. WPR native
start/stop command duration is not covered by this helper deadline; an
interrupted/hung recorder still needs explicit owned-instance recovery.

## Required user action for real capture

Open an **administrator PowerShell 7** session and run this single command:

```powershell
# Reborn: record only the reviewed non-game controls in their uniquely named owned WPR instance; no game launch.
& 'C:\Users\drknt\.codex\.chatgpt-projects\g-p-6aa6a9d9e7f48191931d3f26b6511794\BinaryAssetBuilderOmega2-uprising\scripts\Invoke-Ra3Ep11ConfigReadControl.ps1' -Record
```

Return the command output, particularly `TracePath`, `InstanceName`, both
helper results and elapsed time. Keep the ETL local; do not publish it to
GitHub or upload it blindly. It can contain unrelated metadata/paths added by
WPR despite the requested provider filter.

The runner requests a GUID-suffixed instance, passes `-instancename` as the
last argument on start/stop/cancel, and sets its ownership flag only after a
successful start. Normal completion saves the owned instance with
`-skipPdbGen`. On failure after successful start, cleanup targets only that
same instance; a failed start never authorizes cancellation. No unnamed/global
cancel command is used. If cleanup fails, the warning names the exact owned
instance; do not cancel somebody else's trace to proceed.

## What must be checked after capture

A successful runner result still reports `LiveFilterValidated=false` and
`SuccessfulReadCorrelationValidated=false`. It does not interpret ETL.
Decode the local trace using its real event metadata, inspect lost-event
statistics, and require attributable positive read/completion events for the
exact fixture. Check that the negative helper's events are absent and that
required name/completion records were not discarded by process filtering.
No-event or incomplete-correlation results are inconclusive, not a successful
privacy filter or evidence that the game will ignore the config.

Only after this calibration should the [game config-read plan](RA3EP11_CONFIG_READ_SMOKE_PLAN.md)
proceed with an observed 1.1 child and an explicitly resolved/incomplete-stock
profile. SDK effort remains approximately **52% / 48%**; no game-load success
and no additional compiler test groups (165 not rerun).
