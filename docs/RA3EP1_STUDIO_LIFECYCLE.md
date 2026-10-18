# Studio close confirmation and owned build cancellation

## User behavior

The user's entry remains `alpha/EALAModStudio.exe`. Restart a previously opened
Studio once after this update. No alternate launcher or external stop command
is needed. EA's original executable and the reference SDK are not modified.

An active build owned by this window produces a Turkish Yes/No close prompt.
No keeps the window and its build running. Yes cancels its worker tree, waits
for termination and closes. An idle/completed session does not prompt. Any
earlier EA FormClosing veto is preserved. If safe cancellation cannot complete,
the window remains open with an error instead of silently abandoning ownership.
Other Studio sessions, independent maintenance builds and game processes are
outside this ownership scope. Older workers launched before this update are
not retroactively adopted or killed.

`.sdk.lock` is held by an exclusive OS file handle. Worker termination releases
that handle; the empty file itself may remain. Deleting the path is neither
needed nor safe as a substitute for releasing the actual lock.

## Implementation

- `alpha/studio/RebornStudioLifecycle.cs` is built locally as an AnyCPU CLR4
  helper by SDK synchronization. It loads through reflection from EA's existing
  two-reference CodeDOM adapter, so the adapter does not gain a direct WinForms
  compile-time reference. Both 32-bit and 64-bit hosts can load it.
- The helper subscribes to the first main form after construction via
  Application.Idle, then uses FormClosing. It does not attach cancellation to
  later documentation/error dialogs. A build completing during the prompt and
  callbacks arriving from process-exit threads are accounted for.
- Before EA dispatches a build, the helper allocates a GUID-scoped Windows job,
  cancel/done/started events and a publication mutex. The small host launches
  the real PowerShell worker suspended, assigns it to the job, then resumes it.
  Failed assignment terminates the still-suspended owned process instead of
  allowing an uncontained worker to run. Standard output/error are forwarded.
- The GUI holds the sole long-lived job handle. Kill-on-job-close protects
  against abrupt GUI exit as well as deliberate cancellation. Descendants are
  contained by the same job, not discovered by process name or historical PID.
  These lifecycle guarantees use Windows [job-object semantics](https://learn.microsoft.com/en-us/windows/win32/procthread/job-objects).
- `Invoke-Ra3Ep1DevAlphaBuild.ps1 -StudioSession` opens only the supplied session
  controls. It marks its private work directory and holds the publication mutex
  around staging, replacing a managed output and rollback/cleanup. Cancellation
  sets the event before acquiring that same mutex and terminating the job.
  Thus the worker is never deliberately killed halfway through publication.
- Cancellation cleanup removes only the GUID-matched, marked private work tree
  after checking reparse ancestry/descendants. It does not target mod sources,
  final packages or another session's private files. Unknown trees are retained.
- Sync reuses an unchanged helper; changed helper binaries require Studio to
  be closed. Generated helper/tool files remain local and ignored by Git.

## Validation — October 18, 2026

The hidden synthetic-form harness executes the actual FormClosing handler with
injected responses rather than displaying UI or controlling a live Studio.
It exercises actual Windows jobs and the production host, including real
PowerShell/backend/compiler descendants. It never launches a game or builds
the user's Reborn mod. The original exact-reference CLR4 adapter loader also
passes with the helper installed.

Controls cover idle close; preserving an earlier EA veto; No/Yes for queued
and active work; late dispatch; stdout/stderr forwarding; completed work;
child/grandchild termination; exclusive-lock release; an unrelated concurrent
session surviving; completion while a prompt is open; publication exclusion;
real SDK worker cancellation and lock release; preserving user source and the
last-good BIG; marked-work cleanup; a successful real publication through the
new job/mutex path; restoring the developer source/output; and abrupt synthetic
owner exit killing its worker and grandchild.

All 24 lifecycle controls passed in both x86 and AnyCPU/64-bit harnesses;
the report remains under the ignored
`alpha/.work/lifecycle-test-<id>/result.txt`, with latest console output in
`alpha/.work/lifecycle-controls.log`.

The tests keep exactly Reborn/RebornTest in the UI list. Hidden helper processes
are created solely for these controls; no existing process is selected for
termination. This is lifecycle integration evidence, not another gameplay test.
