# Reborn: x86 snapshot-to-reader-event adapter

Date: October 11, 2026 (Europe/Istanbul). Detached implementation only.

## Implemented bridge

`scripts/Ra3Ep11ConfigSnapshotAdapter.ps1` converts five supplied register
snapshots into the records required by the config-read correlation validator.
Memory is obtained through a caller-provided scriptblock, not any OS API in
this module. Each transfer must report the requested address, boolean success,
actual count and an exact-length byte array. Captures are cloned before use.
There are six bounded reads per complete invocation, each at most 241 bytes.

This is an ABI decoding adapter, **not a live collector or admission gate**.
A caller must bind snapshots and reads to its retained, admitted target and
keep it stopped throughout each capture. Supplying consistent fake data still
passes detached validation; it cannot establish authenticity, breakpoint
cleanup or process ownership. No debugger commands are generated or executed.

## Reviewed locations

The exact SHA-pinned reader body at `004D86B0` was reread from disk, not
disassembled in a running process. Its x86 calling sequence establishes:

| Snapshot site | Capture source |
| --- | --- |
| `004D86B0` before prologue | `[ESP]` caller return; `[ESP+4]` path pointer |
| `004D86F2` before `mov esi,eax` | EAX returned reader; capture its four-byte vtable |
| `004D8725` immediately after virtual read | ESI reader; EBX requested size; EDI destination; EAX returned count |
| `004D9040` splitter entry | `[ESP+4]` destination buffer pointer |
| `004D98AB` after top-level reader call | Invocation closure; no memory read |

The reader pushes EBX then EDI before its virtual read and invokes it with
ECX=ESI. It appends NUL at EDI+EBX before that call. After closing the reader,
it pushes the base path and EDI before calling the splitter. For the inert
probe, this adapter requires EBX=EAX=1 and captures LF at the read return,
then LF/NUL at splitter entry. The caller must already have validated the
24-range native plan before any software breakpoint is installed.

Probe-path bytes must exactly match the supplied ASCII path including NUL;
this adapter deliberately does not normalize alternate casing, separators,
relative paths or aliases. It accepts only integral x86 register values,
preferred base `00400000`, bounded sequences and same-thread invocation
lifetimes. Unknown sites, overlapping active objects/buffers and wrong stages
refuse before their corresponding capture. Sequential pointer reuse remains
permitted after closure; nested same-thread invocations are unsupported.

## Tests and remaining integration

`Test-Ra3Ep11ConfigSnapshotAdapter.ps1` passes two detached positives (both
reader vtables) and 26 private faults. Initial PID/order/site/register/base
and null-stack faults perform zero read callbacks. Each positive performs
six reads and its generated events pass the existing correlation validator.
A fresh baseline passes again after mutation tests. No process, game, helper
or debugger was queried or executed.

The actual initial-break native supervisor still needs a stopped-context
source, owned-process read binding, thread-scoped invocation tracking,
bounded streaming capture and independently tested breakpoint cleanup.
This batch adapter checks supplied snapshots; it does not provide those
operations. No runtime authorization is inferred from its existence.

Windows handle/request/provider correlation, actual directive consumption
and mod-package loading remain separate outstanding proof. Project estimate
remains 52% complete / 48% remaining; the 165 compiler groups were not rerun
for these standalone scripts.
