# Reborn: x86 debugger lifetime calibration

Date: October 10, 2026 (Europe/Istanbul). Scope: newly owned helper only.

## Outcome

`Invoke-Ra3Ep11DebuggerLifetimeControl.ps1` compiles the fixed reviewed
`DebuggerLifetimeControl.cs` into a fresh ignored directory. The helper writes
its PID and UTC creation ticks, lives for five seconds, writes a completion
marker and exits naturally. No game, asset, existing target or child process
is admitted as a debugger target.

Both initial local controls passed:

| Case | Helper PID | Debugger exit | Helper outcome |
| --- | --- | --- | --- |
| `Detach`: initial `qd` with `-pd` | 39812 | 0 | Alive after debugger exit; natural exit 0 |
| `DebuggerLoss`: initial `g`, then terminate owned CDB with `-pd` | 49348 | -1 | Alive after debugger exit; natural exit 0 |

Local evidence directories, under `artifacts/` (not published):

- `RebornDebuggerLifetime-a7bc37485b5c41e98446f24487ad0760`
- `RebornDebuggerLifetime-1411c4049691458e83c5d6f4eb5c5596`

Each contains compiler/debugger diagnostics and the helper's identity and
completion files. Additional repeats use fresh directories and new identities.

After adding source/debugger hash admission and output bounds, both cases
passed again (helper PIDs 34040 and 40776). The existing read-control runner
also passed its two pointer-width and nine refusal cases, then an actual x86
helper read with `-pd`: PID 33800, native status 0, one byte of value `0A`,
matching managed result. No game was executed in any of these controls.

## Documented behavior versus local observation

Microsoft documents that ordinary `q` can terminate the debuggee, `qd` quits
and detaches, and `-pd` requests detachment when the debugger ends for any reason:
[Ending a debugging session in CDB](https://learn.microsoft.com/en-us/windows-hardware/drivers/debugger/ending-a-debugging-session-in-cdb).
See also [CDB options](https://learn.microsoft.com/en-us/windows-hardware/drivers/debugger/cdb-command-line-options).

The controls validate this installed x86 CDB build on one simple managed
helper, including actual forced debugger loss. The additional breakpoint control
below checks one restored byte after normal detach, not forced-loss cleanup with
installed breakpoints, arbitrary target-state restoration, exception handling,
multithreaded engine behavior or safe detachment from Uprising.

Debugger version: `10.0.26100.3916`; SHA-256:
`E6771A874286D8A053C744D428D50A13091318DD1261F07B6C2C17BF42A486A5`.
Reviewed helper source normalized-LF SHA-256:
`1EFA171DAE77F686E2E120EB6530B37F0D359E41C836F74A11C247CD5FC1FB46`.

The runner refuses reparse ancestry and mismatching source/debugger hashes,
bounds compilation and observation, retains a process handle and compares
PID, creation time and executable path. It never kills the helper or searches
for processes by name. On failure it ends only its own debugger; the helper
is designed to self-expire. Failure leaves artifacts and makes no success claim.
Output length is checked after debugger exit, not a continuous disk quota.
Inputs are preflight-pinned, not atomically held through execution; hostile
concurrent filesystem replacement is outside this local control's guarantee.

The existing helper read-control runner now adds `-pd`; its architecture
selection and native read evidence rules are unchanged. Its x64 loss behavior
has not been calibrated by this x86-only lifetime test.

## Initial ownership and active-breakpoint follow-up

The runner now emits `REBORN_OWNER` at the initial debugger break, before `g`.
After the helper starts it retains its OS process handle, checks executable
path and creation ticks, and queries that exact PID's Windows process record
to confirm CDB is its direct parent. The initial CDB PID must match. This is
post-resume evidence validation, **not a fail-closed admission gate before
target execution**. No arbitrary PID, attach or child-follow option is exposed.

`BreakpointDetach` additionally records the address and first byte of
`ntdll!NtDelayExecution`, installs a software breakpoint and detaches with `qd`
when it hits. The helper's sleep supplies a short post-detach inspection window.
The supervisor independently reads exactly one byte from the recorded address
through the retained helper handle, requiring Windows success, an actual count
of one and equality with the original non-`CC` byte. It does not write memory.
The byte address comes from that target's symbol resolution, not a preferred
address copied between runs. API transfer semantics are documented in
[ReadProcessMemory](https://learn.microsoft.com/en-us/windows/win32/api/memoryapi/nf-memoryapi-readprocessmemory);
the temporary software breakpoint is described in
[CDB breakpoint commands](https://learn.microsoft.com/en-us/windows-hardware/drivers/debuggercmds/bp--bu--bm--set-breakpoint-).

Two actual breakpoint-detach controls passed (PIDs 43124 and 14536): initial
identity and direct-parent checks, one hit, restored byte, survived debugger
exit and natural helper exit 0. Local directories:

- `RebornDebuggerLifetime-006ac15261794eca91f3ff9c15d1d13d`
- `RebornDebuggerLifetime-4e6aee4d8c3d4e7bbd6c424cfbb5f58f`

Normal detach (PID 13660) and debugger loss without breakpoints (PID 52268)
also passed again with initial-owner and direct-parent validation. The policy
has two detached positives and twelve refusal cases for identity mismatch,
duplicate/missing/out-of-order records, null/pre-existing-trap sites, missing or
duplicate hits, unexpected breakpoints, oversized logs and diagnostic errors.
These do not inject OS-level identity races or memory-read failures.

The restricted environment denied the CIM process query; the runner refused
proof instead of bypassing that check. The scoped helper-only rerun used the
permitted external execution context, not an admin/UAC game launch.
Two subsequent attempts exposed inherited redirected-output handles after
detach. The bounded log reader now allows read/write sharing after CDB exits;
the reviewed helper never writes to standard output. Those failed attempts are
not successes and their artifacts remain local. Interop compilation occurs
before launch so it cannot consume the helper's five-second observation window.
Successful new runs persist `result.json` alongside raw logs.
The persisted-result control (PID 26416) passed in
`RebornDebuggerLifetime-5227d28a5e1648a4813b005ab452229e`; the restored byte
was `B8`. The supervisor does not wait indefinitely for inherited output
handles; if the bounded snapshot lacks complete records it refuses proof.

## Remaining gate before a game trial

The helper-only [initial-break admission control](RA3EP11_DEBUGGER_ADMISSION_CONTROL.md)
now verifies identity and stage-specific helper entry bytes before observation
continuation. Its cleanup can still resume rejected helpers; it is not a game
recipe or a general native-code relocation verifier.

Implement a separately reviewed early-ownership runner with relocation and
live-byte verification, thread/object/handle correlation and bounded observation.
Do not attach to existing games or enable descendant debugging. A direct-native
launch differs from the verified launcher baseline and must be disclosed.
Any game-debug launch requires explicit authorization covering temporary
debugger effects, scope, supervision and exit behavior. Neither config read
nor parser consumption nor authored mod loading is established here.

Overall estimate remains approximately **52% complete / 48% remaining**.
This safety gate is not a compiler coverage increase; the 165 compiler groups
were not rerun for these helper-only changes.
