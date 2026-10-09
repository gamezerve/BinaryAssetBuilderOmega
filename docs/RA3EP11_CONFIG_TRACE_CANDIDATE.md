# EP1 config-read trace candidate

**Live counterexample:** the [actual non-game control](RA3EP11_READ_CONTROL_RESULT.md)
captured the nonmatching helper and 24 other PIDs. The requested process-name
scope failed. Static metadata acceptance below is historical evidence only;
do not use this candidate for a game recording or treat it as privacy-isolated.

## Prepared and validated statically

`fixtures/ra3ep11/phase-a/config-read.wprp` is accepted by installed Windows
Performance Recorder 10.0.26100 using its metadata-only `-profiles` and
`-profiledetails` commands. **No recording was started.**

Unlike the built-in FileIO profile, this candidate explicitly requests one
manifest provider, `Microsoft-Windows-Kernel-File` (GUID
`edd08927-9cc4-4e65-b970-c2560fb5c289`), informational level 4, keyword mask
`0x1D0`, five included event IDs and `ProcessExeFilter="ra3ep1_1.1.game"`.
It requests no stacks, network, registry, CPU sampling, broad SystemProvider
or unrelated event provider. A memory collector requests 64 buffers of 64 KiB:
4 MiB configured buffer product, not a guaranteed total process memory or
saved ETL-size bound. Only the Light/Memory profile flavor is defined.

Process-name and event-ID filters follow Microsoft's
[EventProvider](https://learn.microsoft.com/en-us/windows-hardware/test/wpt/eventprovider)
and [EventFilters](https://learn.microsoft.com/en-us/windows-hardware/test/wpt/eventfilters)
documentation; collector/profile structure follows the official
[custom profile example](https://devblogs.microsoft.com/performance-diagnostics/authoring-custom-profiles-part-1/).
These describe requested settings, **not validation that this kernel provider
will retain every necessary event under this process-name filter**.

The executable-name filter is not an absolute-path or single-PID filter.
Another installation's identically named process could match. Some events
can be issued/completed in another context; filtering might remove a needed
name/completion event. Do not compensate by silently removing the process
filter or collecting system-wide FileIO. Both privacy scope and correlation
completeness must be tested before runtime use.

## Locally inspected event contract

Read-only Windows provider metadata was queried on this machine. The initial
sandbox query was denied; a reviewed elevated metadata-only query succeeded.
The analytic channel was not enabled. No provider state or trace session was
modified. Manifest event/task IDs below are not legacy kernel opcode IDs.

| ID / versions observed | Role | Relevant named payload fields |
| --- | --- | --- |
| 10 / 0 | NameCreate | FileKey, FileName |
| 11 / 0 | NameDelete | FileKey, FileName |
| 12 / 0, 1 | Create | Irp, FileObject, FileName; ThreadId (v0) or IssuingThreadId (v1) |
| 15 / 0, 1 | Read | ByteOffset, Irp, FileObject, FileKey, IOSize; ThreadId (v0) or IssuingThreadId (v1); v1 also ExtraFlags |
| 24 / 0 | OperationEnd | Irp, ExtraInformation, Status |

The selected keyword bits are Filename `0x10`, OpEnd `0x40`, Create `0x80`,
Read `0x100`. Generic FileIO `0x20` is not requested; selected create/read/end
events also carry their corresponding specific bit. WPR metadata confirms
the requested mask and included IDs. These local definitions must be checked
again if the OS/provider version changes.

FileObject and FileKey are distinct fields: never equate them by name or
pointer appearance. Version-0 ThreadId is declared as a pointer while
version-1 IssuingThreadId is UInt32. Kernel pointer payload widths must be
decoded from trace metadata, not assumed to be 32-bit because the game is
PE32. OperationEnd has no filename; match its Irp to the correct live read,
including order/lifetime, rather than accepting an arbitrary successful end.
IOSize is a read event field; this inspection alone does not establish actual
transferred byte count or ExtraInformation's semantics. A requested read or
successful open is not proof of successful config content consumption.

## Validator and tests

Run `scripts/Test-Ra3Ep11ConfigTraceProfile.ps1 -ProfilePath <absolute .wprp>`
under PowerShell 7. `Get-Ra3Ep11ConfigTraceProfile.ps1 -AsJson` returns the
candidate settings and unresolved live gates. It supports only metadata
commands, no trace start/stop/cancel/save/export or executable launch.

The profile reader caps input at 16 KiB, rejects reparse ancestors, rereads
identity, and disables XML DTD/external resolution. Canonical root XML is pinned
to SHA-256 `330D94E9E661765D6AB06D1DDFE6D32FD4203980D12E62EAE307C3F8C1A8BEE2`;
formatting/comments do not authorize scope changes. Eight private mutations
test removed process filter, reversed event filter, write-event substitution,
all-keyword broadening, stacks, large buffers, file logging and added system
provider. Two XML-policy refusals cover DTD and oversized input. WPR metadata
acceptance and stable repeat JSON also pass. No malformed candidate is passed
to a trace start command. Compiler test groups remain 165 (not rerun).

## Required next experiment, before a game run

Prepare a separately reviewed, short-lived **non-game read control** and an
explicitly matching process-filter profile. Use distinct known test files and
a nonmatching-process negative control; do not rename the helper to impersonate
the game. Before starting, define a unique owned session, bounded duration,
local ignored output directory and cleanup that cannot cancel another trace.
Recording may need elevation; report refusal rather than bypass permissions.

Validate actual capture filtering, path-to-file identity, read/Irp/completion
correlation, relevant payload versions, process attribution and lost-event
reporting using a real control trace. Missing events, failed/ambiguous status,
reused pointer/Irp identities or unproved path aliases must produce an
inconclusive result, not a negative or positive game-load verdict. Do not
publish raw trace files: they may contain paths outside the intended probe
even when the settings are narrower than built-in FileIO.

Only after this control passes should the inert baseline/probe requests in
[the smoke plan](RA3EP11_CONFIG_READ_SMOKE_PLAN.md) be run against a verified
1.1 child and explicitly chosen installation profile. Successful one-byte
config reading remains distinct from manifest/BIG loading or authored assets.

Overall SDK effort stays approximately **52% / 48%**. The narrow trace request
is now syntax/scope validated, but live filtering, successful read correlation,
config consumption and game loading remain unproved.

Follow-up: [non-game control](RA3EP11_CONFIG_READ_CONTROL.md) now has pinned
source/scope, owned-instance cleanup and five-second helper deadlines. Both
helper reads passed without tracing; actual WPR start was denied (0x80070005)
because this host token is not Windows-admin elevated. Administrator capture
is required before live filtering/correlation can be validated.
