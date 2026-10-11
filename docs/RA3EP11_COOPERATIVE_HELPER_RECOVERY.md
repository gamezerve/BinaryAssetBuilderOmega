# Reborn: cooperative recovery after an observation failure

Date: October 11, 2026 (Europe/Istanbul). Fixed helper only; no game executed.

## Changed failure path

The admission helper supervisor now tracks whether it issued a breakpoint
command separately from whether a matching owned stop was verified. If an
error occurs while a breakpoint may remain installed, its `finally` path
does **not** force-kill the debugger. Cooperative recovery requires the
retained admitted helper, a verified owned breakpoint stop and a live helper.
Unknown/non-stopped armed states fail recovery and report user action rather
than treating forced debugger loss as safe.

At a verified stop, recovery performs these bounded steps:

1. Send `bc 0; bl` with a nonce acknowledgement; require an empty listing.
2. Independently read the original code byte through the retained helper handle.
3. Send normal `qd`; require debugger exit within five seconds.
4. Require the helper still alive and independently reread that byte after exit.
5. Require natural helper exit within eight seconds, exit code 0, exact retained
   PID/creation identity and `REBORN_NATURAL_COMPLETION`; require empty stderr
   and normal debugger exit code 0.

An observation failure remains in `Failure`. A successful cleanup sets
`CooperativeRecoveryValidated`, not successful config consumption. The legacy
`NaturalCompletionValidated` field is false for an observation-failed run;
the recovery flag separately validates its cleanup and natural helper exit.
Only the exact expected injected failure plus fully validated recovery lets
the private test control return successfully. Unrelated failures still throw.

The original implementation retained a generic unarmed forced-exit fallback.
The [initial-stop recovery follow-up](RA3EP11_INITIAL_STOP_RECOVERY.md) removes
that fallback and verifies cooperative recovery at an admitted initial stop.
Forced armed-loss execution remains unconditionally disabled.
Unknown/broken-stream recovery is not solved: closing the host/streams can
also affect a debugger, so no guarantee that an unresolved target remains
safely stopped is made. Such a result requires explicit user direction.

## Actual tests

`-InjectObservationFailure CaptureRefusal` injects a private supervisor
exception after a real owned breakpoint/context, without modifying target
memory, registers or files. It does not simulate an actual OS read failure.
`ObservationTimeout` withholds continuation for about one second and injects
the timeout exception at that same verified stop; it does not simulate an
unresponsive debugger or broken pipe.

| Ignored artifact directory | Helper / debugger PID | Result |
| --- | --- | --- |
| `RebornDebuggerAdmission-94a4d419060e4bea82fe9e815bbe747e` | 61148 / 28004 | Capture-refusal exception observed; cooperative cleanup and natural exit passed |
| `RebornDebuggerAdmission-4f29cb80c60a40ac94de78e7582db3e2` | 17564 / 42052 | Timeout exception observed; cooperative cleanup and natural exit passed |
| `RebornDebuggerAdmission-ab9c67d2925f45d79577de1e6be68d49` | 57728 / 51308 | Normal clear/resume regression passed without recovery |

Both injected runs preserved their original observation failure, had
`ForcedDebuggerExit=false`, `RecoveryFailure=null` and
`RecoveryNeedsUserAction=false`. Both helpers and debuggers exited normally.
The baseline also passed. Local `result.json` and bounded stdout/stderr remain
ignored generated evidence, not source files published to GitHub.

The detached admission suite still passes 4 positives / 16 refusals; transport
retains its 3/40 controls plus framing, marker, clear-list and armed-list tests.
No compiler asset suite was rerun for this standalone supervisor change.

## Remaining native boundary

This closes the helper's recoverable exception-at-a-known-stop path. It does
not solve lost debugger processes, lost streams, unknown stops, multiple
native breakpoints, process-launcher chains or game detachment semantics.
A native collector must bind its own stopped events and retained-handle
reads to the trusted native admission plan and classify unresolved cleanup
separately from successful observation. Native `-Run` is still disabled.

The next integration is the native observation supervisor using exact-probe
selection and cooperative stop-scoped cleanup; an actual game-debug trial
still needs separate authorization. Actual Windows file completion, native
parser consumption and a loaded mod remain unproved. Estimate: 52% / 48%.
