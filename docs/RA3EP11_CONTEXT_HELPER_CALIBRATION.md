# Reborn: actual helper-only CDB register calibration

Date: October 11, 2026 (Europe/Istanbul).

Follow-up: [admitted-helper breakpoint transport](RA3EP11_BREAKPOINT_HELPER_CALIBRATION.md)
now observes an actual helper delay breakpoint and separately verifies its
register response, explicit clearing and original-byte restoration. No
native game collector has been enabled.

## Actual result

`Invoke-Ra3Ep11DebuggerAdmissionControl.ps1 -Run -CalibrateContext` now performs
two nonce-bound register round trips at the initial break of one newly compiled
`RebornDebuggerLifetimeControl.exe`. This is restricted to `Mode=Admit` and
the existing fixed helper/debugger paths; it accepts no external target path
or attach PID. Native game execution remains disabled.

The retained helper identity, exact CIM parent/path/creation snapshot and six
initial-entry bytes first pass the shared admission gate. Then the supervisor
sends the read-only register command twice, with separate nonces and bounded
five-second / 4,096-character response windows. No `g`, breakpoint or context
write occurs between these captures. Both records must match the owned PID
and identical PID/TID/EIP/EAX/EBX/ESI/EDI/ESP values. The helper's Main marker
must remain absent. Four stack bytes at each returned ESP are independently
read through its retained handle; both successful actual four-byte transfers
must match. The previous real response is rejected against the second nonce.

After these checks, the normal admitted-helper `g` is issued. Both helper and
debugger exit normally, and the expected completion/identity files match.
This is not execution containment: the loader ran before the initial break,
and the existing failure cleanup can detach/release the self-expiring helper.

## Run evidence

Two actual admitted controls passed, including a repeat after adding the
newline-terminated completion-window guard:

| Run directory under ignored `artifacts/` | Helper PID | Debugger PID | Result |
| --- | --- | --- | --- |
| `RebornDebuggerAdmission-78d04707e3694ebd973a58ceced18322` | 11532 | 37036 | Two stable records, actual stack match, stale response refused, natural exit |
| `RebornDebuggerAdmission-2579628ecbfe40c1b7e93eef685f8cb3` | 42640 | 34676 | Same controls passed with final window guard |

Each directory contains `stdout.txt`, `stderr.txt` and `result.json`. These
local generated artifacts are not published as source. The actual initial
EIP in both runs was `77A08B88`, in the initial system break, not any pinned
Uprising config site. These results must not be relabeled as native-game
base, parser or config-read proof.

The first sandbox attempt failed at CIM access before context calibration;
its directory was `RebornDebuggerAdmission-c482c0b257c34fa0bed2a800975e096a`.
That failure is not counted as a passing control. The scoped reruns used
sandbox escalation for the same fixed helper-only operation.

## Framing learned and detached regression

The supervisor records the current pumped stdout length before sending each
command and parses only the new response window. In the combined transcript,
the previous CDB prompt can appear on the same line as `REBORN_CONTEXT`; the
scoped offset removes the already-received prompt. Therefore do not pass an
entire transcript to the strict register parser. A fragmented/not-yet-pumped
prompt can still cause a safe refusal; general chunk transport remains an
integration concern, not a claimed robust arbitrary transcript parser.

`Get-Ep11CompletedContextWindow` requires exactly one nonce-matching,
newline-terminated completion marker after the register response, with only
blank lines or ordinary prompts following it. It also runs the existing
bounded marker/error checks before returning the isolated window.
The detached transport suite passes 3 positives / 40 refusal-retry checks,
plus 1 completed-window positive, 10 window faults and 5 partial-chunk
refusals. The helper admission suite retains 4 positives / 16 refusals.

## Remaining work

The helper calibration validates the requested command's actual register
format, response correlation and independent ESP read at an initial break.
It does not run the native snapshot decoder against game sites, demonstrate
software-breakpoint event transport, select only the probe among stock reads,
or verify native-game cleanup. Those remain necessary before requesting a
game debugger trial. Windows file/request completion, directive consumption
and mod-package loading are still unproved. No game was executed, no stock
files modified, and no compiler regression suite rerun. Estimate: 52% / 48%.
