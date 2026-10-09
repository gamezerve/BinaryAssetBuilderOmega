# EP1 1.1 linked chunk addressing and ranges — October 9, 2026

## Result and scope

The reviewed concrete reader queues a descriptor pointer and three destination
buffers; dispatch selects the linked reader `00418340` when reader+54h is
non-null. That reader adds eight bytes to each descriptor-relative stream
position. Bin is read from the binary handle; relo and imp are copied from
their loaded buffers through an independently identified `memcpy` import.

All **166,557 ranges** (three per entry) across **55,519 previously captured
stock entries** fit twelve sidecar file lengths, with exact final endpoints.
WorldBuilder is included; its 1,394,571,528-byte binary payload is not read.
This closes a scoped 1.1 concrete addressing/range rebase, not decoded payload
integrity, all runtime bindings, native bounds safety or game compatibility.

## Locating the concrete methods

The previously pinned vtable `00BF8EA0` supplies queue slot+1Ch = `00417210`
and dispatch slot+20h = `00418420`. Disassembly from those method boundaries
shows direct linked call `00418462` targeting `00418340`, and an alternate
nonlinked call `00418479` targeting `00418270`. The latter body is not audited
by this milestone. Direct copy calls at `004183D8` and `004183FE` target thunk
`004DA17A`, whose body jumps through IAT `00BD832C`. That slot contains
hint/name RVA `008B9F04`, with NUL-terminated `memcpy` at raw `008B9F06`.
This establishes copy semantics from the import, not a pointer-shaped guess.
No presumed 1.0-to-1.1 delta establishes these locations.

All code addresses are preferred image VAs; no native code or live process
was executed. The target remains the pinned 1.1 image at
`D:\Program Files (x86)\Steam\steamapps\common\Command and Conquer Red Alert 3 Uprising\Data\ra3ep1_1.1.game`.

## Queue and dispatch fields

`00417210` selects a 20-byte slot at reader+28h + queue-count*20 and increments
reader+50h count. Its five arguments populate:

| Queue field | Source / reviewed role |
| --- | --- |
| +0 | argument 1, opaque dispatch key; not established as TypeId |
| +4 | argument 3, bin destination |
| +8 | argument 4, relo destination |
| +12 | argument 5, imp destination |
| +16 | argument 2, descriptor pointer |

Dispatch checks two slots for a matching key; no match returns false. A match
with linked state invokes `00418340(key, entry, selected-slot)`, otherwise
`00418270`. If slot zero was consumed, slot one is copied into slot zero;
queue count is decremented. Queue-capacity protection and every caller's valid
entry/source bindings are not proven. The selected entry argument is distinct
from the queued descriptor pointer; the reader uses both.

## Concrete linked addressing

| Location | Observed use |
| --- | --- |
| `00418350` | store dispatch key into linked state+10h |
| `00418353–00418368` | queue+16 descriptor; +0/+4/+8 positions each receive +8 |
| `00418374–0041838D` | query binary position through virtual+30h; relative seek through virtual+14h if needed |
| `00418392–004183A6` | set relo cursor state+18h and imp cursor state+14h to header-relative positions |
| `004183B0–004183C0` | entry+32 bin size, queue+4 destination, binary virtual+0Ch read |
| `004183C2–004183E6` | nonzero entry+36 relo size; memcpy from state+8 buffer plus state+18h cursor into queue+8; advance cursor |
| `004183E9–0041840C` | nonzero entry+40 imp size; memcpy from state+0 buffer plus state+14h cursor into queue+12; advance cursor |
| `00418412` | return true |

Descriptor cumulative positions start at zero, as recovered in the producer.
Physical bin start is `8 + descriptor.bin`; buffered relo/imp starts are their
buffer base plus `8 + descriptor.relo/imp`. Both align with the separate
sidecar setup's eight-byte headers. The reviewed reader does not check the
binary seek/read result or buffer-copy bounds before returning true. Therefore
the audit's checked ranges are **diagnostic safeguards**, not engine behavior
or authoring permission to accept malformed streams.

## Metadata-only range validation

`Get-Ra3Ep11LinkedChunkRanges.ps1` imports the pinned 1.1 producer/sidecar
evidence, rereads/re-pins four manifests, and computes a half-open range
`[8 + cumulativeOffset, 8 + cumulativeOffset + entrySize)` for every bin,
relo and imp chunk. Uint64 calculations reject uint32 wrap and out-of-file
endpoints. Empty chunks, including those at the file end, are supported.
Each stream's final endpoint must equal its physical file length.

The ordered range digest covers entry index, stream kind, start and end.
Repeat JSON compares every stable nested report field. Existing raw evidence
comes from `D:\TEMP\Red Alert 3 Uprising Source Data`; no fresh 1.1 BIG
extraction or payload content validation occurs. The inherited sidecar audit
reads twelve eight-byte headers twice, followed by one final header/metadata
pass here: **288 sidecar header bytes per invocation, zero payload bytes**.
The test invokes this audit twice, so its aggregate is 576 header bytes.
Image/manifest reads are separate from those sidecar-only counters.
Repeated headers/lengths/timestamps are not an atomic snapshot or full payload
integrity guarantee.

## Exact pins and tests

Image SHA-256:
`B1DA83EFD229570BAB9DE3A14E48B2D1E6128E302BD52384858FA8A413AE330B`.

| Raw offset | Bytes | SHA-256 |
| --- | ---: | --- |
| 17210 queue | 53 | `D30FD07ECCB15324EC3572DD1539B84A1AF2C0D131A5A8DAE3D9321D223EBB4D` |
| 7F8EA0 vtable | 36 | `FD4ACEBBF76E701A03CD2236F93C18CC6A7896AFB990645CA30FB3A8AD2AB641` |
| 18340 linked reader | 216 | `C8AF501E61D4E1467225FE9658D788DD2EFFEAD0E6599BE49E8E311C51DA52CF` |
| 18420 dispatch | 137 | `707D9E1CC5731EC843DB249034A2B25B40DA5F751EF27C6B3E8F5A39B53F4FE9` |
| DA17A copy thunk | 6 | `0FA2BAE65AF459E550EF1A428C69F5A092CA133C076BE54DD14379E040CE1AEE` |

The import slot/name are additionally checked explicitly. Nine private-array
faults cover queued descriptor forwarding, all three header biases, copy call,
linked dispatch, thunk, IAT and import name. Four positive fixtures exercise
beginning/middle/exact-end/empty chunks; four invalid fixtures must reject
out-of-file endpoints, short headers and modeled address overflow.

```powershell
# Reborn: validate concrete 1.1 linked addressing and WorldBuilder-inclusive metadata ranges without native execution or payload reads.
./scripts/Test-Ra3Ep11LinkedChunkRanges.ps1 -ImagePath 'D:\Program Files (x86)\Steam\steamapps\common\Command and Conquer Red Alert 3 Uprising\Data\ra3ep1_1.1.game' -BaselineImagePath 'D:\Program Files (x86)\Steam\steamapps\common\Command and Conquer Red Alert 3 Uprising\Data\ra3ep1_1.0.game'
```

The original 1.0 fixed-offset guards remain unchanged. No engine/compiler/codec
ran, no production guard changed and the 165-group compiler suite was not
rerun for this standalone static audit. Next rebase 1.1 context creation and
cleanup, factory selection, reader ownership and source lifetime. Authentic
authoring ProcessingHash, unresolved AUDIO inputs, native emission ABI and
actual package loading remain open. Weighted engineering effort stays **52%
complete / 48% remaining**, not a claim of a usable SDK or loaded mod.
