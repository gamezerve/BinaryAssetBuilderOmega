# Reborn: shared bounded debugger output pump

Date: October 11, 2026. Two actual fixed non-game helper controls; no Uprising
execution, existing-process attachment, forced debugger loss or broad tracing.

## Implementation and integration

`scripts/Ra3Ep11DebuggerOutputPump.ps1` extracts the admission helper's
redirected-output reader into reusable functions:

- `New-Ep11DebuggerOutputPump` accepts two distinct caller-owned `TextReader`
  objects, allocating an independent 1,024-character buffer and one pending
  `ReadAsync` task for each. Limits are fixed at 65,536 stdout characters and
  8,192 stderr characters, not byte counts.
- `Update-Ep11DebuggerOutputPump` consumes only completed tasks. Pending tasks
  are not awaited and do not start overlapping reads. EOF is retained and no
  further read is issued for that stream. The limit is checked **before** text
  is appended, so an overflow fragment is not retained in the bounded output.
- Faults remain recorded in that stream and permanently reject further pump
  updates. The reader is never closed/disposed by the module. Ownership,
  process cleanup and reader lifetime remain the external driver's job.

`Invoke-Ra3Ep11DebuggerAdmissionControl.ps1` now uses this shared pump for both
normal observation and cooperative error cleanup. Its ownership gate,
breakpoint handling, original-byte checks and armed-loss execution block remain
unchanged. Stream text/buffer/task fields stay compatible with its existing
bounded marker-window offsets and output artifacts.

The pump preserves raw chunks exactly, including fragmented prompts and CRLF;
it does not strip prompts, decode markers, merge stderr, interpret EOF as
successful completion or authorize commands. Protocol parsers still require
their complete nonce-bound windows. General fragmented-prompt **parsing** is
not solved merely by preserving those characters.

## Detached tests

`Test-Ra3Ep11DebuggerOutputPump.ps1` passes **5 positives / 9 refusal and retry
checks** using private string readers and a controlled pending-task reader.
It tests repeated polling with no completed task, partial prompt/marker/CRLF
fragments, independent stderr, EOF without disposal/re-read, both exact caps,
overflow and permanent refusal, injected read faults and duplicate reader
rejection. No process is created in that suite.

The helper's detached admission suite passes 4 positives / 16 refusals. The
context transport suite retains 3/40 controls plus framing, partial-chunk,
helper-marker, empty-list and armed-list controls. No compiler suite was rerun.

## Actual helper regression controls

| Ignored artifact directory | Helper / debugger PID | Result |
| --- | --- | --- |
| `RebornDebuggerAdmission-8c4a983f99cd479f9812af0b9ea5c7e5` | 49488 / 47076 | Shared admission, real breakpoint/context, empty-list clear, byte restoration and natural completion passed |
| `RebornDebuggerAdmission-1b132340631e42c2a5ef5e9e7fa5deb1` | 51044 / 15636 | Injected capture-refusal exception remained recorded; cooperative clear/normal detach, pre/post-exit byte checks and natural helper completion passed |

Both controls used the freshly compiled pinned helper and retained ownership;
neither forced debugger exit. Both debugger exits were normal. The recovered
control has `CooperativeRecoveryValidated=true` but legacy
`NaturalCompletionValidated=false` because its original observation failure
remains present. This is the documented distinction between successful
recovery and successful observation, not a new failure of natural cleanup.
Generated transcripts/results remain ignored local evidence.

## Remaining backend boundary

This is an actual reused output component, **not** a runnable native supervisor.
It contains no process creation, handle acquisition, command writes or waits.
An external bounded pump loop must check process lifetimes and supervisor
deadlines, supply fresh stop identity, scope fully terminated response windows,
and bind retained-handle captures to
[native supervisor decisions](RA3EP11_NATIVE_SUPERVISOR_SESSION.md).

Asynchronous reader implementations and native reads must themselves be trusted
and bounded; the module cannot cancel a blocked reader/callback or safely close
unknown debugger transport. Constructor failure after one read starts may
leave that caller-owned read pending; it does not transfer disposal authority.
Read-fault/overflow tests do not validate cleanup after actual lost debugger
transport. That remains unresolved and the native execution path is disabled.

Windows config-read completion, parser consumption, native five-site cleanup
and a loaded mod remain unproved. Estimate remains **52% complete / 48%
remaining**; the next integration is the native bounded output-to-state driver,
not a claim that XML/XSD replacement or stream naming alone is sufficient.
