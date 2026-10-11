# Reborn: native admission-to-observation session

Date: October 11, 2026. Detached integration only; no process query, debugger
or game execution.

`scripts/Ra3Ep11NativeObservationSession.ps1` connects the shared native
admission gate, completed context transport, exact-probe selection and engine
correlation validator. It is a trusted in-process adapter, not a runnable
native debugger supervisor and not a JSON evidence importer.

## Boundaries and functions

- `New-Ep11NativeObservationSession` executes the shared admission decision
  using the pinned 1.1 plan and supplied identity/read callbacks. Each fixed
  observation site must lie in exactly one admitted range. The decoder always
  enables exact-probe filtering. Native execution is not enabled.
- `Start-Ep11NativeObservationStop` validates a bounded supplied marker:
  `REBORN_NATIVE_STOP <nonce> <id> <pid> <tid> <eip>`. IDs 0 through 4 map
  respectively to `004D86B0`, `004D86F2`, `004D8725`, `004D9040`, `004D98AB`.
  Wrong identity/site, duplicate markers, errors and the 128-stop limit refuse
  before decoder captures. It prepares, but does not send, a context request.
- `Add-Ep11NativeObservationContext` requires newline-terminated completion
  framing and an independent context nonce. Its PID, thread and EIP must match
  the pending stop **before** path, reader-object or buffer callbacks. Known
  untracked events still require the same binding before being ignored.
- `Complete-Ep11NativeObservationStop` records a caller-managed continuation
  boundary and rotates the next stop nonce. It sends no continuation command.
- `Complete-Ep11NativeObservationSession` accepts no pending stop/context and
  requires a completed engine chain; ignored-only streams cannot prove one.

Lifecycle: `AwaitStop -> AwaitContext -> AwaitContinuation -> AwaitStop`.
Parsing/capture failures fault both observer and decoder; retries cannot read.
Out-of-phase API calls refuse without progressing the lifecycle. Exporting a
valid chain does **not** clear `NeedsBreakpointCleanup`: normal completion is
not evidence that installed breakpoints were removed.

## Validation

`scripts/Test-Ra3Ep11NativeObservationSession.ps1` uses the actual pinned disk
plan and its existing detached shared-gate controls, then synthetic identity,
stop, register and byte records. It passes **3 positive cases / 39 refusal and
retry checks**: full five-stop chain, a known untracked return with no captures,
and a path mismatch stopping after stack plus one path-byte capture. Checks
include wrong stop/context identity, nonce, site and breakpoint ID, duplicates,
partial framing, replay, stop budget, pending completion, a failed path read
and three admission faults with no range/capture callback.

These records are not actual CDB stops or retained-handle reads. The result
keeps `LiveGameReadProven`, `GameExecuted`, `DebuggerCommandsSent` and
`NativeRunEnabled` false. The inherited adapter/filter tests also run; the
165-group compiler suite is unrelated and was not rerun.

## Remaining implementation

A future native supervisor must supply real retained-handle ownership, live
admission bytes, stopped events and bounded captures, and must keep the target
stopped throughout each callback. It must generate/update the five breakpoint
markers using the current stop nonce before continuing; this module installs
none and does not assume that generated commands have run. A printed marker
alone does not establish the Windows debugger stop state.

The helper's single-site cooperative clear/detach control is **not** copied
as native five-site cleanup. The follow-up [cleanup evidence session](RA3EP11_NATIVE_CLEANUP_SESSION.md)
now validates ordered supplied five-site evidence against private admission
copies. Actual native cleanup still needs an independently
validated empty listing and all five original bytes, before/after detach,
plus an explicit unresolved-transport policy. Lost transport and unknown stops
remain unresolved. See [helper recovery](RA3EP11_COOPERATIVE_HELPER_RECOVERY.md).

This integration does not authorize or enable an actual game-debug trial.
Windows file completion, native parser consumption and a loaded mod are still
unproved. Overall estimate stays **52% complete / 48% remaining**, because a
detached adapter is not a verified runtime-loading milestone.
