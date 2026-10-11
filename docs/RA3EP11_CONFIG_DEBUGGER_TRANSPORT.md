# Reborn: nonce-bound debugger context bridge

Date: October 11, 2026 (Europe/Istanbul). Detached protocol tests only.

Follow-up: [actual helper-only context calibration](RA3EP11_CONTEXT_HELPER_CALIBRATION.md)
now validates the command's two real round trips at an owned helper's initial
break. The statements below describe the original detached-only milestone;
no native game collector or game run has been enabled by the follow-up.

## Implemented connection

`Ra3Ep11ConfigDebuggerTransport.ps1` connects a supplied CDB-format register
response to the incremental snapshot session. It does not own any debugger
streams, create/attach to a process, install breakpoints, send commands or
resume a target. No native run path was enabled.

`New-Ep11ConfigContextRequest` prepares one `.printf`/`.echo` command containing
a fresh 32-character nonce and eight fields: PID, thread ID, EIP, EAX, EBX,
ESI, EDI and ESP. It has no continuation, memory-writing or shell command.
Only one request may be pending; a second request faults the session.

`Get-Ep11ConfigContextRecord` accepts a bounded response window of at most
4,096 characters. Exactly one `REBORN_` marker must match the expected nonce
and eight unsigned x86 hex fields. Missing/duplicate/unknown markers and
reviewed debugger error phrases refuse. Non-marker prompt/echo lines may be
present, but are not proof of a stopped target. The command's actual behavior
on CDB has **not been calibrated in this follow-up**; supplied fixtures are
not a successful debugger round trip.

`Add-Ep11ConfigDebuggerResponse` consumes the pending request, checks its PID,
assigns the next invocation ID on reader entry and obtains subsequent IDs
from the active thread map. No invocation ID comes from log text or global
debugger scratch registers. It immediately passes the snapshot to the
incremental decoder; the caller must keep its admitted process stopped until
all captures finish. Replay, stale nonce, unknown thread or wrong-stage
responses permanently fault the session. Completion with a pending response
is also refused before any further read.

The nonce is a response-correlation token, not authentication. An in-process
caller can fabricate text and read callbacks. Native admission, retained
handle ownership and a trustworthy stopped-context source remain separate
requirements; neither a marker nor a CDB prompt proves them.

## Detached verification

`Test-Ra3Ep11ConfigDebuggerTransport.ps1` passes three positive controls:

- Two sequential reader invocations, with assigned IDs 1 and 2 and 12 reads.
- Prompt/CRLF envelope parsing for one nonce-bound marker.
- Two interleaved threads with distinct reader objects/buffers, IDs 1 and 2
  and 12 reads, without shared scratch registers.

Twenty private faults and their twenty terminal retry refusals give 40
refusals. Faults cover nonce/marker framing, errors/size/register shape,
PID/site/thread, request overlap, unsolicited/replayed/stale response and
pending-response completion. Every initial refusal performs no memory read;
the replay/stale cases retain only their already accepted entry's reads.
The prior snapshot ABI suite (2 positives / 26 faults), session suite
(3 positives / 18 refusals) and engine correlation suite (4 / 41) also pass.
No helper, process, debugger or game was executed or queried.

## Next step and readiness

Calibrate the register command and chunk/window framing on a newly owned,
self-expiring non-game helper. Then connect trusted owned-process memory
reads and real stopped-context events, with admission before software
breakpoints and separately verified cleanup. The game-specific filter also
must distinguish this exact probe from stock config reads without decoding
unrelated buffers or treating ordinary out-of-scope activity as proof.

Actual process reads, Windows handle/request correlation, parser directive
consumption and loaded mod validation remain unproved. Estimate remains
52% complete / 48% remaining; the compiler suite was not rerun here.
