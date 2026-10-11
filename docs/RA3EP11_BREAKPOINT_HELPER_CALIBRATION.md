# Reborn: actual admitted-helper breakpoint transport

Date: October 11, 2026 (Europe/Istanbul).

## Implemented and observed

`Invoke-Ra3Ep11DebuggerAdmissionControl.ps1 -Run -CalibrateBreakpoint`
uses only a fresh compiled fixed helper, never a game or existing target.
It is mutually exclusive with context calibration, detached self-tests and
non-Admit modes. The existing shared identity/entry-byte gate runs first.

After admission, CDB resolves the previously calibrated helper delay site
`ntdll!NtDelayExecution` and reports its original byte. A retained-handle
one-byte read independently matches that byte; an existing `CC` trap refuses.
This is a helper baseline, not a native-game or complete ntdll code pin.

The supervisor installs only breakpoint ID 0 at that address, with a
nonce/PID/TID/EIP marker and **no automatic continuation** in its command.
It resumes the helper and waits within bounded output/time limits for one
owned hit. A separate nonce-bound register request while stopped must match
that same PID, TID and EIP. Then `bc 0; bl` clears the breakpoint and reports
the breakpoint table. The final implementation requires an empty listing,
not merely a printed acknowledgement: any listing entry/error refuses.
An independent actual one-byte read must again match the original code byte.
Only then does the helper resume and exit naturally with the debugger.

The byte was `B8` at address `77969EE0` in these controls. Neither address nor
byte is a portable ntdll/game contract. The helper may reach the delay site
during runtime startup or Main; no Main-absence guarantee is claimed at this
breakpoint. No native config snapshot decoding occurs at this unrelated site.

## Actual runs

Run directories are under ignored `artifacts/`, with bounded stdout/stderr
and `result.json`; raw artifacts are not published as source.

| Directory | Helper PID | Debugger PID | Result |
| --- | --- | --- | --- |
| `RebornDebuggerAdmission-096e06e539784a85bd44b07b80d207e0` | 19168 | 45704 | Owned hit/context, restored byte, natural completion |
| `RebornDebuggerAdmission-678ee3c083864c1390607398e8d7e016` | 38308 | 43796 | Repeat passed |
| `RebornDebuggerAdmission-451c6a0f033447649af01f7618d7f389` | 61068 | 16680 | Final empty-breakpoint-list guard also passed |

All successful helpers/debuggers exited normally; no forced debugger exit
occurred. The earlier experiment at the managed helper PE entry thunk did
**not** hit before the helper exited (`RebornDebuggerAdmission-b0e4cca9742a417f804747ca10df3e73`).
It is a failed experiment, not cleanup or hit proof. This establishes that
the admission entry-byte location must not be assumed to be an executed
observation site during this CLR startup path; the exact bypass mechanism
was not reverse engineered here.

## Detached regression and boundaries

The transport test retains 3 positives / 40 refusal-retry checks, plus a
completed-window positive, 10 framing faults and 5 partial-chunk refusals.
Two new helper site/hit positives and 16 faults cover wrong nonce/PID/TID/site,
missing/duplicate/unknown/error markers, zero address and preexisting trap.
One clear-list positive and 5 faults cover missing/duplicate/wrong-nonce
acknowledgement, an active breakpoint listing and a command error. The helper
admission detached suite still passes 4 positives / 16 refusals.

This proves one admitted helper's breakpoint hit → stopped register response
→ explicit clear/empty table → byte restoration → natural completion route.
It does not prove arbitrary fragmented prompt transport, timeout/debugger-loss
cleanup with armed breakpoints, multiple game breakpoints, probe-only selection
among stock reads, or native-game detachment safety. Existing `-pd` failure
cleanup can release the helper; it is not execution containment.

Next implement bounded probe-only entry filtering and connect admitted native
observation sites without interpreting unrelated config calls as probe reads.
A game-debugger trial still requires separate authorization. Windows request
completion, actual parser directives and a loaded mod remain unproved.
No game or stock-file modification occurred. Estimate remains 52% / 48%; the
compiler regression suite was not rerun for these standalone controls.
