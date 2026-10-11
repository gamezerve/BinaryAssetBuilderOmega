# Reborn: detached engine config-read correlation

Date: October 11, 2026 (Europe/Istanbul).

Follow-up: the [x86 snapshot adapter](RA3EP11_CONFIG_SNAPSHOT_ADAPTER.md) now
derives these events from supplied register snapshots and bounded read
callbacks. It does not implement or enable actual process collection.

## Outcome and boundary

`scripts/Ra3Ep11ConfigReadCorrelation.ps1` validates supplied records only.
It does not collect debugger events, access a process, launch a game or prove
Windows file I/O. `SuppliedEngineChainValidated` means the supplied chain is
internally consistent, not authentic live evidence. Native collection remains
unimplemented and disabled; the compiler regression suite was not rerun.

The narrow target is the inert, one-byte LF `probe_1.0.cfg` fixture. The path
must be absolute bounded ASCII and satisfy the reviewed filename limit.
Records require one expected PID, preferred module base `00400000`, contiguous
scoped sequence numbers and positive bounded thread/invocation identifiers.

## Required event chain

| Event / preferred VA | Required binding |
| --- | --- |
| ReaderEnter / `004D86B0` | Exact probe path; caller return `004D98AB` |
| OpenReturn / `004D86F2` | Non-null object; captured vtable `00BFA208` or `00BFA270` |
| ReadReturn / `004D8725` | Same object; requested and returned count 1; captured LF in a non-null buffer |
| ParserEnter / `004D9040` | Same buffer; successfully captured LF followed by NUL |
| ReaderExit / `004D98AB` | Matched parser input; close invocation/object/buffer lifetime |

Every event in an invocation must belong to its entry thread. Interleaved
distinct threads are accepted, but same-thread nested probe invocations,
shared active objects and overlapping two-byte parser buffers are refused.
Pointers may be reused after a completed invocation; invocation identifiers
may not be reused. Missing, duplicate, unknown, out-of-order or unfinished
chains fail closed. Captures require boolean success, actual captured count,
byte-array length and content equality, not merely requested counts.

The lifetime assumptions are intentionally restrictive. A real collector
must assign invocation IDs and retain arguments across calls per thread;
global debugger scratch registers are not a safe substitute. Internal worker
thread activity is outside this engine-reader boundary model.

## Verification performed

`scripts/Test-Ra3Ep11ConfigReadCorrelation.ps1` passes four detached positives:
memory-reader chain, source-reader chain, interleaved distinct threads and
sequential pointer reuse. It rejects 41 private faults covering identity,
ordering, sites, path/caller, reader contract, captures, counts, content,
termination and concurrent lifetime ambiguity. A valid baseline is repeated
after fault injection to check that private mutations did not contaminate it.

The native admission plan adds the previously reviewed 310-byte body at
`004D98AB`, making 24 ranges / 4,216 bytes. Its 24 disk-derived matches and
26 refusal cases pass. Shared-gate preflight also passes 18 identity, 8 read
and 7 range refusals. None of these tests queried or executed a process.

## Remaining proof and next implementation

A future owned-process native collector must first use the trusted admission
gate, verify code before inserting breakpoints, and produce actual bounded
captures with thread/invocation lifetimes and deterministic cleanup. Its
execution needs separate authorization; this validator does not enable it.

Actual disk-read proof is a separate layer: bind the exact `CreateFileW` path
to a valid handle lifetime, then capture `ReadFile`'s boolean, actual output
count and LF buffer before the wrapper overwrites EAX with the count. Memory
population, provider selection, pending worker completion and handle/request
correlation remain unproved. A supplied reader-to-splitter chain proves none
of these. Splitter input also does not prove directive consumption, dependency
loading, manifest compatibility or successful mod loading.

All result flags for live game read, Windows file read, parser directives,
mod loading, target execution and collector readiness therefore remain false.
The project estimate remains 52% complete / 48% remaining; this milestone
prepares evidence, rather than completing a runtime migration requirement.
