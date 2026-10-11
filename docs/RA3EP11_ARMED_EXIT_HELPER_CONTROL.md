# Reborn: armed-breakpoint timeout and debugger-loss controls

Date: October 11, 2026 (Europe/Istanbul). No game executed.

## Outcome: normal detach passed; forced loss failed

The helper supervisor now distinguishes `ClearAndResume` (existing default),
`TimeoutDetachArmed` and the **disabled** `DebuggerLossArmed` experiment.
These dispositions require the separate fixed-helper breakpoint calibration
and cannot select a game or external PID. The shared identity/entry-byte gate
still precedes installation and continuation.

Before either armed-exit experiment, an owned hit and separate register
response match PID/TID/site. A bounded `bl` response must contain exactly
enabled breakpoint ID 0 at that site, plus the requested nonce marker; missing,
disabled, duplicate, extra or wrong-site entries refuse. Microsoft's
[breakpoint-list reference](https://learn.microsoft.com/en-us/windows-hardware/drivers/debuggercmds/bl--breakpoint-list-)
documents the listing status/address fields; the parser is restricted to
the observed x86 forms, with or without `Disable Clear` UI labels.

### Successful timeout-to-detach

Directory: `artifacts/RebornDebuggerAdmission-1292a142126d43c59c56f95171592909`.
Helper PID 24764; debugger PID 60476.

The supervisor withheld continuation for 2,029 ms at the armed breakpoint,
then issued `qd` without first clearing it. The debugger exited with code 0,
the helper was still alive, and a retained-handle actual one-byte read matched
the pre-breakpoint `B8` at `77969EE0`. The helper then exited naturally with
the exact identity and `REBORN_NATURAL_COMPLETION` marker. No forced kill.
This calibrates this helper's normal detach path, not every timeout/error
condition or native-game detach behavior.

### Failed forced debugger loss

Directory: `artifacts/RebornDebuggerAdmission-51c8cd10ef754e648f98d66ec037d101`.
Helper PID 33352; debugger PID 17988. Creation UTC ticks 639273127171935310.

After the same owned hit/context and enabled listing, only the owned debugger
was intentionally terminated, with exit code -1. The helper initially survived
and the independent code-byte read again matched the original `B8`.
Nevertheless the helper did not complete within the subsequent 10-second
deadline. `NaturalCompletionValidated` is false. Byte restoration therefore
must **not** be treated as successful cleanup, continuation or normal exit.

A later exact-PID read-only check found PID 33352 no longer present, while its
completion marker was still absent. The helper was not killed by the agent.
Its final exit code and exact reason for disappearance are unavailable; no
crash or successful normal completion is inferred. An initial event-log check
was sandbox-denied; a scoped elevated Application 1000 query found no events
in the five-minute helper window. That absence does not establish normal exit.

`-Run -CalibrateBreakpoint -BreakpointDisposition DebuggerLossArmed` now throws
before fixed-path checks, artifact creation, compilation or process creation.
The blocked call was verified; no further loss replay was performed. The
experimental implementation remains for analysis, behind that unconditional
run guard. The normal timeout-to-detach and clear/resume controls remain scoped
to newly created helpers.

## Tests and integration consequences

The transport suite adds two armed-list positives and ten refusal cases;
the previous transport, framing, marker and clear-list tests still pass.
The detached admission and exact-probe filter suites also remain passing.
Local generated evidence stays ignored, not published as source.

A future native supervisor must prioritize cooperative clear/detach and
acknowledgement, not assume `-pd` plus forced debugger termination can safely
release a target stopped at an armed software breakpoint. The current generic
helper failure fallback can still kill its own debugger, so it is **not** a
production game-cleanup recipe. This batch does not solve crashes, broken
streams or an unresponsive debugger with armed breakpoints. Native `-Run`
remains disabled, and game-debug authorization is not inferred.

The next design must explicitly separate successful observation, cooperative
timeout cleanup, and failed/unknown target state. Windows read completion,
native parser consumption and mod loading remain unproved. Project estimate
remains 52% complete / 48% remaining; compiler regressions were not rerun.
