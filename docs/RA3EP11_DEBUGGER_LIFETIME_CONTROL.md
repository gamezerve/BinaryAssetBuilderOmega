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
helper, including actual forced debugger loss. They do not prove cleanup of
software breakpoints, restoration of arbitrary target state, exception handling,
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

## Remaining gate before a game trial

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
