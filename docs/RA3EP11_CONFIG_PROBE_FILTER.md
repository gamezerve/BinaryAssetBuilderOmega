# Reborn: bounded exact-probe context selection

Date: October 11, 2026 (Europe/Istanbul). Detached tests only.

## Implemented selection

`New-Ep11ConfigSnapshotSession -FilterProbeEntries` explicitly enables the
filter in `Add-Ep11ConfigDebuggerResponse`; strict existing sessions retain
their previous behavior. `Ra3Ep11ConfigProbeFilter.ps1` classifies supplied
same-process integer contexts at the five reviewed native observation sites.
It neither sends debugger commands nor resumes/attaches to a process.

At reader entry `004D86B0`, the filter first reads eight stack bytes. If the
caller is not `004D98AB`, it records only `CallerMismatch` and does not read
the supplied path pointer. For the reviewed caller, it compares path bytes
one at a time against the exact ASCII probe plus NUL, stopping at the first
mismatch, including early NUL. It records `PathMismatch`, not the unrelated
path or a binary dump. Comparison is deliberately case-sensitive and does
not resolve aliases, separators, relative paths or symlinks.

A fully matched entry enters the existing decoder, which independently
recaptures stack/path before opening its lifetime. This costs up to 241
bounded one-byte path reads plus the initial eight-byte filter read, in
addition to the existing six decoder reads per completed invocation. The
caller must keep the admitted process stopped throughout both selection
and decoding. Recapture mismatch faults the session; the filter cannot
establish authenticity or absence of third-party memory modification.

Known non-entry contexts on an untracked thread record `UntrackedEvent`
without reading reader objects or parser buffers. Contexts on an active
probe thread proceed to the strict stage/object/buffer validator. A second
reader entry on that same thread faults even if its path might be unrelated:
nested invocation ambiguity must not be silently ignored. Unknown sites,
wrong PID, invalid pointers and failed/short/misaddressed captures also
fault; unreadable path memory is not a successful mismatch classification.

Filtered sessions accept at most **128 context responses total**, including
ignored and selected stops. A request at the limit refuses before generating
another command; response handling independently rechecks the limit before
capture. The existing accepted-event bound remains 320, though the filtered
total-stop limit is tighter. Counts/reason codes are trusted in-process state,
not a tamper-proof security boundary or a source of automatic resume authority.

Ignored contexts do not create invocation IDs, decoder sequence numbers or
successful read/parser evidence. Ignored-only completion fails the existing
nonempty complete-chain requirement. The filter records no full unrelated
file names, and a future supervisor must independently decide continuation
and cleanup. No native run path was enabled.

## Detached verification

`Test-Ra3Ep11ConfigProbeFilter.ps1` passes seven positive controls:

- Exact probe selection and a complete five-event chain.
- Wrong caller with only one eight-byte stack read, no path capture.
- First-byte mismatch, early NUL, longer shared prefix and changed path case,
  each stopping at its first mismatch.
- Mixed active-probe and untracked-thread contexts; four ignored known-site
  stops perform no extra reads and the exact probe completes normally.

Ten faults plus ten terminal retry refusals give 20 refusals: wrong PID,
unknown site, null stack/path, failed/misaddressed path reads, changing path
between filter and decoder, nested same-thread entry, total-stop limit and
ignored-only completion. The limit uses a private injected boundary state,
not an actual 128-stop game trace. Initial scope/limit/nesting failures invoke
no additional read; retries never invoke a callback.

Transport, snapshot/session and correlation detached regressions also pass.
The fixture scripts query no process, execute no debugger/game, and read no
actual config file contents. Earlier actual helper controls are not rerun by
this detached selection test. The 165 compiler groups were not rerun.

## Remaining integration

Connect the filter to actual admitted native breakpoint events, retained
owned-process reads and bounded prompt/chunk framing. Validate timeout or
debugger loss with armed breakpoints on a helper before treating cleanup as
safe for a game trial. A separate game-debugger authorization is still needed.
This filter alone does not prove that a probe was read in Uprising, which
provider transferred its bytes, that directives were consumed or a mod loaded.
Estimate remains 52% complete / 48% remaining.
