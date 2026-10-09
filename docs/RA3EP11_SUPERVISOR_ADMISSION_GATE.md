# Reborn: shared supervisor admission gate

Date: October 10, 2026 (Europe/Istanbul). Native execution remains disabled.

## What is now connected

`Ra3Ep11SupervisorAdmissionGate.ps1` connects the native range factory and the
helper-calibrated initial-break supervisor through one admission decision.
It exposes no process creation, attach, debugger commands or memory-writing API.

`Assert-Ep11SupervisorAdmissionIdentity` requires the initial debugger owner
record to match the retained process identity and the exact-PID snapshot:
PID, debugger parent, live-state flags, executable paths, creation window,
disk-image hash and module base/size. Snapshot creation ticks may differ by
at most one millisecond from the retained process creation ticks; this is our
explicit comparison tolerance, not a claimed Windows accuracy guarantee.

The native profile is restricted to the pinned 1.1 image at the reviewed Steam
path, base `00400000` and SizeOfImage 13,754,368. The helper profile admits only
the small `RebornDebuggerLifetimeControl.exe`, retaining its separate CLR
initial-break byte policy. Unknown profiles refuse.

`Get-Ep11SupervisorAdmissionDecision` checks identity and all range shapes
before calling a trusted read adapter. Limits are 32 ranges, 4,096 bytes per
range and 8,192 bytes total. Every returned record must be unique for that
requested range, correctly addressed, successful, full-length and hash-matched.
The native profile additionally passes the existing complete native range
comparison. The method returns a decision only, never sends `g` or `qd`.

The actual helper supervisor now calls this gate before its continuation
logic. It constructs identity from its owned process handle/CIM snapshot and
binds the read callback to that same helper handle. Native preparation uses
the same gate, but with synthetic identity and private disk copies only.

## Native detached integration

`Get-Ra3Ep11NativeSupervisorPreflight.ps1` builds the trusted pinned native
plan and supports optional `-SelfTest` and `-AsJson`. Its `-Run` argument is
unconditionally rejected **before any file inspection or other action**.
There is no game/debugger launch or process-query branch in this script.
It is not a launch-ready native supervisor disguised as preflight.

The real shared decision passes all 23 disk-derived ranges / 3,906 bytes with
a synthetic identity. Thirty-three integration refusals pass:

- 18 identity/transcript/profile faults refuse before the first read callback.
- 7 malformed range/budget faults also refuse before the first read callback.
- 8 read-result faults stop at the first failed callback; no partial decision
  allows continuation.

The valid 23-range decision is repeated after all private mutations to check
that the fault copies have not changed the trusted baseline.
Two complete `-SelfTest -AsJson` runs returned identical reports; native live
proof and execution flags remained false in both.

The execution-disabled guard passes independently. The native plan's existing
23 detached matches and 26 refusals remain a separate regression suite.
The helper's four detached positives and sixteen refusals also pass.
No detached success sets `LiveBytesValidated`, `GameExecuted`, `ConfigReadProven`
or `GameRecipeReady` true. Native `ProcessQueried` and `DebuggerExecuted` are false.

## Actual helper calibration

The updated supervisor was exercised on newly compiled helpers, not a game:

| Mode | Helper PID | Shared-gate result | Actual continuation |
| --- | --- | --- | --- |
| Admit | 50584 | Identity and actual six-byte read accepted | `g`, natural exit 0 |
| RejectLiveBytes | 8956 | Genuine baseline accepted; private expected-hash fault refused by shared gate | No `g`; cleanup `qd`, natural exit 0 |
| Timeout | 58548 | Genuine baseline accepted; outer policy withheld admission | No `g`; cleanup `qd`, natural exit 0 |

Local artifact directories:

- `RebornDebuggerAdmission-82064c26b8a54874bfc82695cfffab8f`
- `RebornDebuggerAdmission-c7c62c6c94e6483c97f2a83fca90f76d`
- `RebornDebuggerAdmission-f1b580996d9a461e97413f213a5033ae`

The refusal control only counts the expected shared byte-mismatch exception;
an unrelated OS/identity failure cannot masquerade as that successful test.
`SharedAdmissionDecision` records the genuine baseline decision, not the outer
policy's final action. `ObservationResumeIssued` and the injected-fault field
separately show whether observation actually continued and whether the fault
was refused. Reject/timeout cleanup releases the helper to execute; this is not
execution containment. These helper trials do not calibrate native game reads.

## Trust boundary and next work

The gate consumes a trusted factory plan and capture adapters in the same code
path; it does not authenticate user-supplied JSON, a fabricated identity object
or byte records as OS evidence. Identity snapshots are non-atomic. The caller
must retain the actual handles, capture genuine process/module data, maintain
the initial break and recheck liveness before any continuation command.
No native runtime capture adapter or narrow config-breakpoint recipe is enabled
yet. Source/debugger preflight pins do not prevent hostile concurrent file
replacement; the previous documentation's scope limits remain in force.

Next: complete native capture/supervision and thread/object/handle correlation
for config-reader evidence. A real game-debug trial still needs explicit
authorization, disclosure of its direct-native route and cleanup that can
resume the game. Do not follow descendants or attach to an existing game.
Actual config consumption and authored mod loading remain unproved.
Overall effort remains **52% complete / 48% remaining**; compiler groups 165,
not rerun for these standalone admission changes.
