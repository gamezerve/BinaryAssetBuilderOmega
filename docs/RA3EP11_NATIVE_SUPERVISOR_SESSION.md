# Reborn: composed native observation and cleanup decisions

Date: October 11, 2026. Trusted in-process, detached control only. No game,
debugger, process-query, breakpoint or detach API is present.

## Integration correction

Closing the old observation API after releasing the final reader-return stop
removed `PendingStop`. The cleanup API correctly requires a known pending
stop, so that sequence could not clean up normally. The new
`scripts/Ra3Ep11NativeSupervisorSession.ps1` evaluates the completed engine
chain **at the final captured ReaderExit stop**, without calling continuation
or clearing that pending stop. It then opens the five-site cleanup session.
Ignored/untracked returns are not treated as newly captured exits; a chain
with active tracked threads is not closed prematurely.

## Decisions and deadlines

| Controller state | Next decision | Required boundary |
| --- | --- | --- |
| AwaitStop | WaitForOwnedStop | Fixed native admission already accepted |
| AwaitContext | ReadStoppedContext | Nonce-bound owned stop recorded |
| AwaitContinuation | ContinueObservation | Bound context/captures accepted; fresh supplied stop identity checked again on release |
| AwaitClear | ClearOwnedBreakpoints | Observation ended or failed at a known matching stop |
| AwaitDetach | NormalDetach | Empty listing and all five pre-detach bytes accepted |
| Completed | None | Normal supplied debugger exit and five post-exit bytes accepted |
| Unresolved | UserAction | No continue/detach/kill fallback |

These labels are decisions, not commands sent or permission to execute them.
Admission, retained identity and byte callbacks still need a trusted external
backend. Fresh supplied lifetime/stop identity is checked before each context
capture and again at the continuation boundary. A changed identity becomes
unresolved rather than allowing captures or cleanup of an unrelated target.

Caller-supplied monotonic milliseconds enforce a 30-second observation budget
and at most 5 seconds for stopped context/continuation, clear acknowledgement
and detach completion. Backward or overflowing clocks are unresolved. A
deadline reached at a known stop can enter cooperative cleanup; expiry while
awaiting an unknown/running stop cannot invent a safe stop. Clear or detach
expiry is unresolved. `Test-Ep11NativeSupervisorDeadline` lets an external pump
check deadlines **without** receiving a response. This module starts no timer;
the caller must poll it and independently bound OS waits and callbacks. It
cannot cancel a blocked callback or prove supplied time is actual elapsed time.

The first observation failure is preserved separately from cleanup failure.
Successful cleanup after a context failure or stopped timeout exports
`SuppliedObservationValidated=false`, even if
`SuppliedCleanupValidated=true`. Neither implies live OS evidence. The
observer's real cleanup requirement remains unmodified.

## Tests

`scripts/Test-Ra3Ep11NativeSupervisorSession.ps1` passes **4 positives / 28
refusal and retry checks**. Positive scenarios cover a complete five-stop
engine chain followed immediately by ten cleanup reads, a mismatched context
thread recovered at the known stop, a stopped context timeout and a deadline
reported with no response. The last three preserve observation failure after
successful supplied cleanup.

Refusals cover continuation at the final cleanup stop, unknown stops, running
timeouts, backward clocks, changed target/stop identity, expired clear/detach,
nonempty listings, changed pre/post-detach bytes, abnormal debugger exit,
missing responses and terminal-state retries. Failed envelopes/lifetimes/time
checks cause no cleanup reads; byte checks stop at their first failed site.
An additional cleanup-only failure after a valid chain stays separate from
observation failure and leaves the overall controller unresolved.
Inherited observation (3/39) and cleanup (2/207) detached suites also pass.
No compiler asset suite was rerun.

## Still required before runtime validation

This composes policy, not actual process management. An authorized execution
backend must still bind retained OS handles, initial live admission, actual
debugger stop state, nonce marker updates, bounded redirected output and
stopped reads to these decisions; then perform cooperative five-site clear
and normal detach with independently obtained evidence. Unknown/lost-transport
behavior must not silently close streams or force-kill the debugger. Native
`-Run` remains disabled and no actual game-debug trial was performed.

The successful non-game helper controls remain useful calibration but do not
validate native five-site detach semantics or game continuation. Windows file
completion, parser consumption and mod-package loading remain unproved.
Overall estimate remains **52% complete / 48% remaining**.
