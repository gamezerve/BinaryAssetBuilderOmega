# Reborn: bounded output windows feeding native supervisor decisions

Date: October 11, 2026. Detached integration and historical helper-format
replay only; no debugger or game was started in this follow-up.

## Implementation

`scripts/Ra3Ep11NativeOutputBridge.ps1` connects the already-shared asynchronous
output pump to the observation/cleanup controller:

- `New-Ep11NativeOutputWindow` freezes the controller phase, nonce, pump and
  stdout cursor for one future response. It permits only `AwaitStop`,
  `AwaitContext` and `AwaitClear`; overlapping windows are refused. The external
  driver must open it **before** sending the corresponding command/continuation
  and must have separately validated prior output. It does not drain or silently
  discard pending output at creation.
- `Get-Ep11NativeOutputFrame` caps the response at 4,096 characters and waits
  for a complete terminal CDB user-mode prompt, including its final space.
  Partial marker, prompt and CRLF chunks cannot dispatch. A single exact prompt
  prefix immediately before a `REBORN_` marker is normalized; arbitrary command
  echoes or diagnostics are not removed. Only the current phase/nonce's marker
  lines and whitespace are accepted. Existing validators check ordering,
  duplication, register fields and empty breakpoint listings again.
- `Update-Ep11NativeOutputWindow` pumps only available completed reads, checks
  stderr/EOF and controller deadlines, then dispatches one complete window to
  `Add-Ep11NativeSupervisorStop`, `Add-Ep11NativeSupervisorContext` or
  `Add-Ep11NativeSupervisorClear`. It performs no command writes and closes no
  reader/process. Post-detach identity/byte validation remains the separate
  controller operation, not an EOF interpretation.

An intact but malformed context window at a known stop chooses cooperative
cleanup and preserves observation failure. Read faults, stderr or EOF mean
transport is not trusted and become `Unresolved`, never a command/forced-exit
fallback. A deadline can interrupt a pending context window and free its
ownership for the distinct cleanup window. Consumed/failed windows cannot
replay. Late old responses remain unexpected data in subsequent windows; they
cannot be accepted as a fresh cleanup acknowledgement.

## Validation

`Test-Ra3Ep11NativeOutputBridge.ps1` passes **4 positives / 24 refusal and retry
checks**, in addition to the inherited supervisor, cleanup, observation and
output-pump suites. The full supplied five-stop chain and clear acknowledgement
are fed one character at a time, including prompt prefixes and CRLF splits.
No decoder captures occur before complete framing. The final reader stop is
retained for cleanup; all five pre/post-detach bytes are still validated.
A separate no-response timeout interrupts the old context window without
captures and chooses known-stop cleanup.

Two additional positives replay exact context/clear format excerpts from the
previous actual owned-helper recording
`RebornDebuggerAdmission-8c4a983f99cd479f9812af0b9ea5c7e5`. Its helper PID 49488,
thread 31520 and `NtDelayExecution` address are **not** native game evidence;
these positives exercise framing only, not the native decoder or live memory.

Refusals include stale nonce, duplicate stops/contexts, unknown lines, automatic
register diagnostics, response overflow, stderr, EOF, read-task faults,
nonempty breakpoint listings, overlapping windows and replay. No readers are
disposed and no partial/failed window authorizes capture. No compiler asset
suite was rerun.

## Actual format limitation found during review

The recorded helper breakpoint window includes startup `ModLoad` lines and an
automatic register/disassembly display after `REBORN_BREAK`. The strict native
stop frame intentionally refuses equivalent extra lines; it must **not** be
called a working live-native stop collector in the default CDB configuration.
Context and clear prompt shapes are backed by recorded excerpts, whereas the
new native stop envelope is still supplied test data.

Microsoft documents `.prompt_allow` controls for automatic register,
disassembly, effective-address, source and symbol display. A future separately
calibrated display profile may reduce that output; no such command was issued
or assumed successful here. See [Microsoft prompt-display documentation](https://learn.microsoft.com/en-us/windows-hardware/drivers/debuggercmds/-prompt-allow--control-prompt-display-).
Startup/event diagnostics and unexpected exceptions also need their own
bounded classification, not unrestricted line skipping or blanket filtering.

Follow-up: [actual helper display calibration](RA3EP11_DEBUGGER_DISPLAY_CALIBRATION.md)
now verifies that profile and strict helper-stop/context/clear suffix framing.
It adds a framing-only `AwaitHelperStop` mode; native controller phases remain
unchanged. The calibration does not classify earlier startup diagnostics or
prove native game stop handling.

Follow-up: [module-load prefix classification](RA3EP11_MODULE_LOAD_STOP_PREFIX.md)
adds a stop-window-only opt-in for bounded informational `ModLoad` records.
The default remains unchanged, and context/clear cannot enable that opt-in.
Unknown exceptions and automatic register diagnostics are still refused;
the complete recorded helper prefix is not silently discarded or accepted.

The prompt is a framing delimiter, not proof that Windows has stopped the
same retained process. The execution backend still must supply actual stop
state, owned handles, nonce-command updates, bounded waits and read provenance.
Custom asynchronous readers/capture callbacks can still block or fabricate
evidence; polling this bridge cannot cancel them. Unknown/lost-transport
cleanup remains unresolved, and native `-Run` remains disabled.

Overall estimate stays **52% complete / 48% remaining**. The next concrete step
is classification of remaining startup exceptions/event framing and then
connecting retained process evidence. Actual Uprising config consumption
and mod-package loading are still unproved.
