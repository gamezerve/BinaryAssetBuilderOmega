# Reborn: actual helper stop-display calibration

Date: October 11, 2026. Fixed newly compiled non-game helpers only; no Uprising
launch, existing-process attachment, system-wide tracing or game changes.

## Display-only profile

`scripts/Ra3Ep11DebuggerDisplayProfile.ps1` prepares the fixed session command
`.prompt_allow -reg -dis -ea -src -sym` followed by a fresh acknowledgement.
Microsoft documents these categories as automatic register, disassembly,
effective-address, source and symbol display controls, not target execution
commands. See [Microsoft prompt-display documentation](https://learn.microsoft.com/en-us/windows-hardware/drivers/debuggercmds/-prompt-allow--control-prompt-display-).

The pinned English CDB report is independently validated: one allow heading,
one disabled heading, exactly all five disabled categories, no enabled category,
then exactly one nonce acknowledgement and a complete terminal prompt.
The observed prompt-prefixed first heading and plain `None` empty allow list
are recognized explicitly. Missing/duplicate categories, command errors,
unknown lines, premature/duplicate acknowledgements, partial prompts and
oversized responses are refused.

`Invoke-Ra3Ep11DebuggerAdmissionControl.ps1 -CalibrateDisplayProfile` is limited
to its existing `-CalibrateBreakpoint` normal clear/resume helper path. It cannot
combine with detached admission tests, armed-exit disposition or observation
failure injection. The shared ownership/live-byte gate remains unchanged;
display configuration happens after that gate and before breakpoint creation.
The preceding site query must reach a complete prompt before configuring display.

After a validated helper hit, the suffix beginning at its owned marker is
checked with `Get-Ep11NativeOutputFrame` in the new **helper-only**
`AwaitHelperStop` framing mode. Context and clear replies are checked through
the same strict complete-prompt parser. The native controller still accepts
only its native stop/context/clear phases; helper markers are not translated
into native stop evidence. Original byte restoration, empty listing and natural
helper completion remain required before success.

## Actual results

| Ignored artifact directory | Helper / debugger PID | Outcome |
| --- | --- | --- |
| `RebornDebuggerAdmission-cb6b390b7e3c492881047086b812ad30` | 53616 / 48480 | Initial report-format refusal before installing a breakpoint; not a successful calibration |
| `RebornDebuggerAdmission-c0dfcc5a242c49b28359ea1e18de7ab6` | 37060 / 49936 | Display report, quiet stop/context/clear framing, original byte and natural completion passed |
| `RebornDebuggerAdmission-f748b736942b4859be362b8fc0c24c27` | 17544 / 1836 | Second calibration with final ordered-report validator passed |
| `RebornDebuggerAdmission-e0c95a93735c4f8bbd6119a9e1c67687` | 43100 / 57836 | Default display mode clear/resume regression passed |

Both quiet controls have `DisplayProfileReportValidated`,
`QuietHelperStopFramed`, `QuietHelperContextFramed` and `QuietHelperClearFramed`
true. Both helpers/debuggers exited normally, with restored original code byte
and `ForcedDebuggerExit=false`. The baseline also exited normally. No game
read or native five-site detach was exercised.

The first refused report used actual `None` and a prompt-prefixed heading rather
than the initially expected formatting. The old helper-only **unarmed** error
fallback terminated its retained debugger (exit -1); no observation continue
or helper breakpoint was issued. Later that exact helper PID was absent, and
`started.txt`/`completed.txt` were absent. Its helper exit reason/code is unknown,
so absence is **not** normal-completion proof. The unsuccessful artifact and
failure flags are retained, not overwritten by later successes. This again
limits the old unarmed fallback: it must not be exported as native-game cleanup
policy or treated as proof that a released helper ran normally.

The detached test initially had a PowerShell expression syntax error; it was
fixed before the corrected display control was repeated. Final
`Test-Ra3Ep11DebuggerDisplayProfile.ps1` passes **3 positives / 13 refusals**;
native output bridge regression still passes **4 positives / 24 refusal/retry
checks**, including inherited suites. No compiler suite was rerun.

## Remaining startup/cleanup boundary

Each quiet helper transcript contains 2,155 characters **before** its validated
breakpoint marker, including startup/module diagnostics. Calibration scopes
only the suffix after the owned hit; it does not bless, discard in a native
collector, or classify that earlier output as safe. Quiet prompt display does
not suppress all engine events, startup messages or unexpected exceptions.
Their bounded classification remains a separate native-driver requirement.

The earlier strict bridge still refuses default automatic register diagnostics.
The new actual calibration establishes a compatible helper marker suffix when
the profile is verified, not a complete native stop transport. A native backend
must independently verify display configuration, retained handles, actual stop
state, startup/event output and five-site cooperative detach. Lost transport,
unknown stops and the legacy unarmed helper failure path remain unresolved.
The armed-loss execution block is unchanged and native `-Run` stays disabled.

Next: bounded startup/event classification and replacing reliance on the old
unarmed forced-exit fallback, before native execution integration. Actual
Windows config-read completion, parser consumption and mod loading remain
unproved. Estimate stays **52% complete / 48% remaining**.
