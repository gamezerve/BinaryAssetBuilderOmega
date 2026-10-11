# Reborn: admitted initial-stop recovery without generic forced exit

Date: October 11, 2026 (Europe/Istanbul). Fixed non-game helper only.

## Implementation

`scripts/Invoke-Ra3Ep11DebuggerAdmissionControl.ps1` no longer kills the
debugger as a generic fallback when no breakpoint was tracked. Recovery
routes are explicitly recorded as `OwnedInitialStop`, `OwnedBreakpoint`
or `Unresolved`; normal completion requires no recovery route (`None`).

Initial-stop ownership becomes known only after the shared admission gate
and independent actual six-byte entry validation. Before any potentially
releasing `g` or `qd` write, this proof is invalidated, including when that
write could fail partially. A later running/unknown state cannot reuse it.

The initial-stop route requires:

1. The retained admitted live helper PID, creation time, executable path,
   image base and image size, with no before-Main `started.txt` marker.
2. A fresh nonce-bound `bl` acknowledgement framed through the shared bounded
   output parser and a complete prompt; the breakpoint table must be empty.
3. An independent retained-handle entry read with Windows success, exact
   transfer count and all six original bytes matching.
4. Normal `qd`, debugger exit within five seconds and helper completion
   within eight seconds, both exit codes 0 and empty stderr.
5. Exact PID/creation marker and `REBORN_NATURAL_COMPLETION` file contents.

No breakpoint is inserted on this route. The entry bytes are not reread
after detach: the managed loader thunk may change during startup. This is
not a substitute for native five-site pre/post-detach byte validation.

`InitialStopRefusal` is a separate admitted-helper-only synthetic supervisor
exception, injected before any breakpoint/observation continuation. It does
not simulate a broken pipe, OS memory-read failure or unknown debugger stop.
The original observation failure remains recorded even when recovery passes;
`InitialStopRecoveryValidated` and `CooperativeRecoveryValidated` report
cleanup separately. Legacy `NaturalCompletionValidated` stays false on
observation-failed runs. Only an exact expected injected failure with verified
recovery is accepted as a successful implementation control.

## Actual controls

| Ignored artifact directory | Helper / debugger PID | Result |
| --- | --- | --- |
| `RebornDebuggerAdmission-f62c52b8002b4f83ab4a2e8d77a3b4b9` | 26436 / 58776 | Initial-stop exception preserved; empty table, six-byte reread, normal detach and natural completion passed |
| `RebornDebuggerAdmission-54da2cb8ced445a68b2064980cac6ffb` | 37020 / 12144 | Existing breakpoint CaptureRefusal recovery passed, including original-byte checks before/after detach |
| `RebornDebuggerAdmission-68c70753e7224fe886ffcbf1a7c7448b` | 53792 / 12908 | Normal quiet-display breakpoint/context/clear regression and natural completion passed |

All three controls report `ForcedDebuggerExit=false`, debugger exit code 0
and no debugger alive at cleanup end. Neither injected control erases its
original failure. Generated binaries/logs remain local ignored evidence.

`scripts/Test-Ra3Ep11InitialStopRecoveryGuards.ps1` verifies seven invalid
initial-stop injection combinations and the historical armed-loss replay
guard: eight refusals before file/process preparation. These are guard
tests, not live unresolved-transport tests. The detached admission suite
passes four positives / sixteen refusals. Asset/compiler tests were not
rerun for this standalone supervisor change.

## Remaining boundary

The historical failed display-format control remains documented in
[display calibration](RA3EP11_DEBUGGER_DISPLAY_CALIBRATION.md); it used the
old unarmed forced-exit fallback and never proved natural helper completion.
This correction does not retroactively make that run successful.

Unknown stops, lost transport and cleanup errors remain unresolved. No guessed
continue/detach/kill is sent by the generic fallback; stream/handle disposal
can still affect a debugger, so keeping an unresolved target safely stopped
is not guaranteed. `RecoveryNeedsUserAction` remains explicit. The old
intentional armed-loss experiment is blocked before execution and must not
be replayed. Native `-Run` remains disabled.

Next is bounded startup/event classification for the native output bridge,
then separately authorized real runtime validation. No Uprising was run;
config consumption and loaded-mod proof remain open. Estimate: 52% complete,
48% remaining; helper safety corrections do not advance compiler coverage
or establish runtime compatibility.
