# Reborn: five-site native cleanup evidence session

Date: October 11, 2026. Detached validation only; no game/debugger launch,
process query, breakpoint operation, detach command or memory write.

## Admission-captured baseline

`New-Ep11NativeObservationSession` now privately copies the transfers accepted
by the shared admission gate. After the complete range/hash decision succeeds,
it extracts the original byte at each fixed site: `004D86B0`, `004D86F2`,
`004D8725`, `004D9040`, `004D98AB`. A preexisting `CC` byte refuses creation.
Cleanup cannot accept replacement originals supplied later by its byte reader.
In these tests the admission transfers are disk fixtures, **not live bytes**.

An initial callback wrapper exposed PowerShell dynamic-scope name shadowing
and recursive invocation. Its two detached test processes were interrupted;
no debugger/helper/game process was involved. A separate reader variable fixes
the recursion. Completed regression tests below cover the final wrapper.

## Ordered validation

`scripts/Ra3Ep11NativeCleanupSession.ps1` adds four public operations:

- `New-Ep11NativeCleanupSession` requires a pending known stop from an observer
  in `AwaitContext`, `AwaitContinuation` or `Faulted`. Supplied PID/debugger,
  creation identities, paths, hash and module identity must still match
  admission. Before detach, the supplied stopped flag, thread and address must
  match that pending stop. The observer is frozen in `Cleanup`; its original
  status is retained and its decoder cannot keep observing.
- `Add-Ep11NativeCleanupClear` requires a terminated fresh `REBORN_CLEAR`
  acknowledgement and an empty breakpoint listing. A marker alone, a remaining
  breakpoint, command error, stale nonce or duplicate acknowledgement refuses
  **before** byte callbacks. It then checks five exact-address full one-byte
  transfers against the copied baseline and stops at the first failure.
- `Complete-Ep11NativeCleanupSession` requires supplied normal debugger exit
  code 0 and the same retained target still alive. It repeats all five reads
  after debugger exit; a pre-detach restored byte alone cannot validate this.
- The internal identity/byte validators contain no continuation, kill or
  debugger command APIs. There is no forced-exit fallback.

Lifecycle: `AwaitClear -> AwaitDetach -> Validated`. Failed identity, envelope
or byte evidence faults the cleanup session, records the error and sets
`NeedsUserAction`; retry is rejected without further reads. Out-of-phase calls
also refuse. Unknown stops cannot open a session. Lost streams cannot produce
the required evidence and are not treated as safe cleanup.

## Test results and limits

`scripts/Test-Ra3Ep11NativeCleanupSession.ps1` passes **2 positives / 207 refusal
and retry checks**. Both normal and observation-failed sessions require ten
byte reads, five on each side of detach; the failed observation remains
`Faulted` in the result. Faults cover each of the five sites on both sides for
wrong bytes, trap bytes, incomplete transfers, failed reads, incorrect address
or status type, wrong buffer length, absent and duplicate transfers. Identity,
stop/thread/site, empty-list framing, ordering and replay refusals are also
checked. The inherited native observation suite still passes 3 positives / 39
refusals and retries, including the real pinned disk admission plan.

Supplied evidence is not Windows debugger-stop proof. A trusted live supervisor
must actually keep the target stopped during pre-detach reads, issue/observe
clear and normal detach, retain the same OS handles, bound every wait/read and
independently obtain post-detach bytes. A callback can still fabricate evidence
or block; this module neither establishes its provenance nor cancels it.
No actual native five-site cleanup was exercised, and no runnable native
supervisor was added. `NativeRunEnabled`, `LiveCleanupProven`,
`TargetCompletionProven` and `CommandsSent` remain false.

Even a successful supplied cleanup keeps the observer's
`NeedsBreakpointCleanup` requirement true: this detached validator must not
erase the need for real cleanup proof. Target survival also does not establish
normal subsequent game behavior. The failed helper debugger-loss experiment
and its disabled forced-loss execution path remain unchanged.

Next: connect actual supervisor lifetime/stop evidence to the native sessions,
with bounded cooperative clear/detach and an explicit unknown/lost-transport
policy. Actual config consumption and mod loading remain unproved. Estimate
stays **52% complete / 48% remaining**; no compiler asset suite was rerun.
