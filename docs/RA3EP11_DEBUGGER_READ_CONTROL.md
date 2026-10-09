# Owned-helper native read calibration

## Result — October 9, 2026

The existing x64 CDB debugger successfully observed a newly compiled,
reviewed **non-game helper**, without WPR, administrator elevation, tool
installation, an existing-process attach or system-wide recording.
This establishes a useful alternative calibration route, **not an Uprising
read/load result**. The rejected WPR profile remains disabled.

First validated runner invocation:

| Field | Observed value |
| --- | --- |
| Helper PID | 2956 (`b8c` in the debugger) |
| Native thread | `a350` |
| Exact probe open | `\??\` followed by the checkout's absolute `fixtures\ra3ep11\phase-a\config-read-only.cfg` path |
| Open return | Status 0, handle `f0` |
| One-byte read | Same handle and thread, native PID matching managed helper PID |
| Native read return | Status 0; IO_STATUS_BLOCK.Status 0; Information 1; buffer byte `0A` |
| Managed result | `2956\|READ\|1\|10` |
| Total observed opens / one-byte reads | 18 / 1 |
| stdout SHA-256 | `322F9EF7C6826AF7A7D2FC207EB7CA00CD7B0BC676F5E51F3190CBA7BEBCF052` |
| Ignored run directory | `artifacts/RebornDebuggerControl-40d57508f74f44d8a0ec277cfa9d06ed` |

A second fresh compilation/run passed with helper PID 53260, 18 observed
opens and one successful one-byte read. Its ignored directory is
`artifacts/RebornDebuggerControl-8e3e4973f55a4c4ea78ed463f9c6621e`; stdout
SHA-256 is `6F3E34433223AF38ABB5038C8EBC6D6D67057BABB0C988CB1DD7567424B80172`.
Log hashes differ because process IDs, addresses and run paths differ;
the required semantic proof is the same. Detached tests and the original
control's four input/scope refusals passed again. `git diff --check` passed.

For this synchronous helper, the native return and IO_STATUS_BLOCK give
status and actual transfer size, independently of a managed success message.
Microsoft documents Information as bytes actually read for
[NtReadFile](https://learn.microsoft.com/en-us/windows-hardware/drivers/ddi/ntifs/nf-ntifs-ntreadfile).
Pending/asynchronous completion is deliberately rejected rather than inferred.

## Files and safety boundary

### x86 follow-up

`-Architecture x86` now selects the existing x86 CDB, `/platform:x86`
helper compilation and independently pinned `config-read-debugger-x86.txt`.
Default remains x64. The x86 recipe uses stack arguments: NtReadFile handle
at ESP+4, IO_STATUS_BLOCK pointer at ESP+14h, buffer at ESP+18h and length
at ESP+1Ch; its status/information fields are four bytes apart. It uses the
x86 OBJECT_ATTRIBUTES/UNICODE_STRING pointer layout for NtCreateFile.
This is explicit architecture-specific calibration, not a claim that changing
the debugger path alone makes the x64 command file portable.

First x86 run passed: PID 16416, 22 observed opens, one successful exact-path
one-byte read, IO_STATUS_BLOCK.Information=1 and buffer value 0A. Ignored
directory: `artifacts/RebornDebuggerControl-481509d4d0694b5fbc5fc67382ee474e`.
stdout SHA-256: `A6E878C39C090924CB30DEF75BE0787C764A9E716805407933233C8CC0795C8F`.
No administrator elevation, WPR, installed-file change or game execution.

The repeat x86 run passed with PID 24984 and the same semantic proof
(22 opens / one read), directory
`artifacts/RebornDebuggerControl-f74596d9db74487d8fca74a2e7555a41`, stdout SHA
`FF7B95432C29CC0C80FDA0916C896E253B0ABE7F8B2FC958019903EF6951B20B`.
The x64 regression also passed with PID 11340, 18 opens / one read and stdout
SHA `B09629CD746862C9882B52D1E5562F58B0EB875B6D7DF57CD85E11DBEAC00767`.
Both pointer-width fixtures, all nine evidence refusals, the detached
launcher observer's two valid/three invalid scope cases and diff whitespace
checks passed. No compiler/asset test groups were rerun.

- `scripts/Invoke-Ra3Ep11DebuggerReadControl.ps1`: no-argument preflight,
  detached `-SelfTest`, explicit helper-only `-Run`.
- `fixtures/ra3ep11/phase-a/config-read-debugger.txt`: reviewed, SHA-pinned
  x64 CDB commands; `config-read-debugger-x86.txt` supplies the separate x86
  recipe. All new code/comments use the Reborn convention.
- `fixtures/ra3ep11/phase-a/ConfigReadControl.cs`: unchanged reviewed helper,
  compiled for the explicitly selected architecture into a new ignored directory.
- `fixtures/ra3ep11/phase-a/config-read-only.cfg`: unchanged SHA-pinned one-LF probe.

The runner fixes the debugger and helper paths; it accepts no arbitrary
executable, PID, attach, config or command file argument. The debugger command
uses `-G -noshell -nosqm -sins`, an explicit local symbol directory and `-cf`.
It does not request child-process debugging, a server, installation, dumps,
stacks or network symbol locations. The initial experiment's `-netsym:no`
option was rejected by this installed CDB build; it is not in the runner.

Software breakpoints modify **only the owned helper's temporary process
memory** and affect timing. They do not patch its executable on disk or any
installed game. The debugger sees the helper's own opens/module paths;
raw logs remain local and ignored, not published. This is process-owned
debugging, not a claim that the old WPR filter now works.

`NtCreateFile` logs the native path and returned handle; `NtClose` invalidates
the handle mapping; `NtReadFile` records only one-byte calls and a one-shot
return breakpoint. The parser requires one complete exact-path read with
same-thread return, successful status, one actual byte, value 0A and matching
native/managed PID. Duplicate, incomplete, pending and overlapping observations
are refused. It is intentionally **not a general-purpose concurrent I/O tracer**.
Paths are printed as data, not substituted into debugger commands.

The helper/debugger input and output ancestry rejects reparse points.
The owned debugger has a 20-second deadline and stdout/stderr limits of
256 KiB / 8 KiB. Timeout termination targets only that newly owned debugger,
never a process-name sweep. No detach option is requested. The successful
runs finish naturally; timeout cleanup has not been fault-injection tested.
Polling limits may briefly overshoot between checks; they are not hard disk quotas.
Compilation uses the existing .NET Framework compiler and has no separate
supervision deadline yet. Normal compiler success was observed, not compiler
hang resilience.

## Reproduction

Use PowerShell 7 from the repo root. No administrator window is required.
Default invocation only checks inputs; `-SelfTest` executes neither debugger
nor compiler. `-Run` compiles and starts only the reviewed non-game helper.

```powershell
# Reborn: detached evidence validation, followed by owned-helper-only native observation; no game or WPR.
& 'C:\Users\drknt\.cache\codex-runtimes\codex-primary-runtime\dependencies\native\powershell\pwsh.exe' -NoProfile -File '.\scripts\Invoke-Ra3Ep11DebuggerReadControl.ps1' -SelfTest
& 'C:\Users\drknt\.cache\codex-runtimes\codex-primary-runtime\dependencies\native\powershell\pwsh.exe' -NoProfile -File '.\scripts\Invoke-Ra3Ep11DebuggerReadControl.ps1' -Run
# Reborn: calibrate the separate x86 recipe on an owned helper only, never an installed game.
& 'C:\Users\drknt\.cache\codex-runtimes\codex-primary-runtime\dependencies\native\powershell\pwsh.exe' -NoProfile -File '.\scripts\Invoke-Ra3Ep11DebuggerReadControl.ps1' -Architecture x86 -Run
```

Detached tests now include two valid pointer-width sequences and nine refusals covering wrong
path, pending status, transfer size, byte value, PID, thread, closed handle,
missing return and duplicate observation. These are observer tests, not new
compiler/asset test groups. The original 165 compiler groups were not rerun.

## Remaining runtime gate

Do **not** point these commands at `ra3ep1_1.1.game`: both helper architectures
are now calibrated, but game ownership, supervision and instrumentation policy
remain separate gates. The owned launch does not prove that a Steam launcher
forwards `-modconfig`, selects the intended child or tolerates debugging.
Instrumentation/timing changes also make this different from a normal startup.

Next substantive step: design a separate x86 owned-launch observation plan,
including exact launcher/child ownership, native calling convention,
baseline/probe comparison, cleanup and missing `MapsCampaign.big` policy.
Before running it, disclose the debugger-under-game implications and obtain
the necessary user choice. Do not automatically attach to an existing game
or replace this helper target. Alternatively, choose a separately authorized
capture method with its collection scope explicitly disclosed.

First mod-test readiness advances when those runtime gates pass, not when
another evidence parser is added. Overall estimate remains **52% complete /
48% remaining**, and game mod loading is still unproved.
