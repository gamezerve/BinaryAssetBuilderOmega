# Reborn: bounded informational module-load prefixes

Date: October 11, 2026. Detached controls and historical helper-output replay
only. No new helper, debugger or Uprising process was started.

## Narrow implementation

`scripts/Ra3Ep11NativeOutputBridge.ps1` now offers an explicit
`-AllowModuleLoadPrefix` switch on `New-Ep11NativeOutputWindow` and
`Get-Ep11NativeOutputFrame`. The native window permits it only in `AwaitStop`;
context and clear windows refuse it. Existing callers remain strict by default.
Direct `AwaitHelperStop` framing can use it for historical helper replay only.

`Convert-Ep11ModuleLoadStopPrefix` classifies at most 32 `ModLoad` lines,
within the existing 4,096-character response cap, before the fresh stop marker.
It requires exactly eight hex digits per address, nonzero start and end greater
than start, and a bounded printable image name (synthetic image label, simple
DLL/EXE name or the calibrated drive-qualified Windows path subset).
Only one or two adjacent prompt prefixes immediately before a module line
are normalized, matching the recorded continuation shape. More prompts,
unrecognized paths, invalid ranges and module lines after the stop are refused.

Classification happens only after a complete terminal prompt. Module lines
alone never produce a stop response. Remaining text still passes the existing
phase/nonce/duplicate validators, then the observer's admitted PID, nonzero
thread and fixed-site checks. An independent context is still required before
any path/object/buffer capture. There is no new command or process API.

Classified records are retained as `Window.ModuleLoads` / `Frame.ModuleLoads`.
Their `IdentityValidated=false` is deliberate: these are debugger-printed
metadata, not independently read module mappings. Printed paths are never
opened, resolved or used as an executable allowlist; this does not prove
target ownership, code provenance or successful module initialization.

Unknown lines, command echoes, exceptions and automatic register/disassembly
diagnostics are not filtered out. An unclassified stop response leaves the
controller unresolved without memory captures or guessed continuation.

## Validation

`scripts/Test-Ra3Ep11ModuleLoadStopPrefix.ps1` passes **6 detached positives /
21 refusals**, alongside the inherited bridge's 4 positives / 24 refusal and
retry checks and the underlying admission/observation/cleanup/pump controls.

Positive controls cover calibrated prompt prefixes, simple/path/synthetic
image shapes, waiting for the final prompt space, module-only non-dispatch and character-fragmented
module-plus-stop dispatch without captures. Refusals cover zero/reversed
ranges, malformed/control/overlong names, excessive prompts, after-stop
module lines, 33-record overflow, stale/duplicate markers, unknown lines,
unknown exceptions, register output, invalid opt-in phases, strict-default
behavior, wrong admitted PID/site and retry after a faulted bridge. The test asserts 13 independent
fault fixtures so expression precedence cannot collapse that coverage.

The complete stop window from the previous actual helper artifact
`RebornDebuggerAdmission-68c70753e7224fe886ffcbf1a7c7448b` was separately replayed.
Its 2,234 characters include both module lines and:

```text
(d220.89f4): Unknown exception - code 04242420 (first chance)
```

The complete recorded window is **refused**, specifically by the unexpected
response-line validator. The exception is not presumed harmless, equated to
a native exception or removed to manufacture a successful full-window replay.
Its original helper run remains a suffix-only quiet-display calibration, not
proof that this new classifier accepts all actual startup output.

No asset/compiler suite was rerun for this standalone parser change. Native
`-Run` remains disabled. Initial debugger banners, unexpected exceptions,
real stopped-state evidence, native cleanup and config/mod consumption still
need their own validation. Overall estimate remains 52% complete / 48% remaining;
this narrows an output-classification gap, not runtime compatibility.
