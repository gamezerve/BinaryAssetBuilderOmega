# EP1 linked chunk addressing — October 9, 2026

## Result

The concrete linked reader at preferred VA `00418360` adds **8 bytes** to
descriptor-relative instance, relocation and import positions. Instance data
is read from the binary handle after a position query/relative seek when needed;
relocation and import data are copied from their loaded sidecar buffers through
the independently verified `memcpy` import thunk at `004D9DBA`.

This connects the descriptor sums to physical sidecar addressing for the
reviewed concrete implementation. All **166,557 ranges** (three per asset)
from **55,519 raw entries** in four pinned EP1 manifests fit the corresponding
actual sidecar lengths; every final cumulative endpoint equals the file end.
The large local WorldBuilder stream is included without reading any payload.

The analysis remains metadata-only. In-range chunks are not proof of correct
payload layout, relocations, imports, ProcessingHash or game loading. Concrete
source-object binding on all outer load paths and capacity reuse are still
unresolved; no production admission policy changes.

## Queue and dispatch linkage

The expanded concrete vtable at `00BF2E78` maps slot `1Ch` to queue method
`004171E0`, and slot `20h` to dispatcher `00418440`. Queue records have a
20-byte stride; they are a separate layout from the producer's 20-byte
descriptors. The queue's +16 contains the producer descriptor pointer.

| Queue offset | Observed role |
| --- | --- |
| +0 | Request matching key, not identified as an asset TypeId |
| +4 | Instance output buffer |
| +8 | Relocation output buffer |
| +12 | Import output buffer |
| +16 | Producer descriptor pointer (queue method's second argument) |

The dispatcher searches two queue slots, returns false if no matching key is
found, and chooses the linked reader when object+84 is nonzero. Otherwise it
calls the separate single-asset reader `00418290`. The first queue slot can be
compacted from the second after dispatch; count is decremented. This review
does not establish enqueue capacity safety, all queue lifecycle behavior or
callee return propagation. The linked and single-asset branches remain distinct.

## Linked reader field mapping

| Preferred VA | Operation |
| --- | --- |
| `00418373`–`0041837B` | Loads producer descriptor through queue+16, reads its +0/+4/+8 relative positions |
| `0041837E`, `00418385`, `00418388` | Adds 8 to relocation/import/instance positions |
| `00418392`–`004183AD` | Queries binary position (virtual slot `30h`); if different, passes target-current delta and mode 1 to seek slot `14h` |
| `004183AF`–`004183C6` | Sets relocation/import state cursors to their descriptor-relative positions plus 8 |
| `004183D0`–`004183E0` | Requests entry+32 bytes from binary handle into queue+4 output |
| `004183E2`–`00418406` | If entry+36 is nonzero, copies from relocation buffer+cursor into queue+8; advances cursor by that size |
| `00418409`–`0041842C` | If entry+40 is nonzero, copies from import buffer+cursor into queue+12; advances cursor by that size |
| `00418432`–`00418437` | Returns true |

The raw manifest entry remains the source of chunk lengths at offsets
32/36/40. The 8-byte bias is a physical stream header, not a change to the
48-byte manifest record. Empty relocation/import chunks skip their memcpy;
empty instance chunks still reach the observed virtual read call.

The linked reader does **not** check the returned binary read count or seek
result in this body. Nor does it directly check memcpy source/destination
bounds. Underlying I/O may have its own checks, but those are not recovered.
Our range tool's checked arithmetic/file-bound rejection is diagnostic policy,
**not a claim that the native engine implements the same guards**. Native
signed seek deltas and unchecked 32-bit addition are not emulated for arbitrary
oversized inputs; selected stock ranges fit the stricter model.

## Exact code/import evidence

Engine SHA-256:
`ABA6A8825A744A78487837D78AD5992962257CE68404F639D600F11DEEA2580B`.

| Region | Raw offset | Bytes | SHA-256 |
| --- | --- | ---: | --- |
| Queue | `000171E0` | 53 | `D30FD07ECCB15324EC3572DD1539B84A1AF2C0D131A5A8DAE3D9321D223EBB4D` |
| Nine concrete vtable slots | `007F2E78` | 36 | `0EC633EFAB5A8DE6DF257F51FA1058800B16FA1321771CE6BB0510E022AB88FB` |
| Linked reader | `00018360` | 216 | `62729B4F6566FB717C6FE6799DF138AE3BF88305F681036855F3148A7DB216E2` |
| Dispatcher | `00018440` | 137 | `707D9E1CC5731EC843DB249034A2B25B40DA5F751EF27C6B3E8F5A39B53F4FE9` |
| Copy thunk | `000D9DBA` | 6 | `F2DAC25FFAE8EAAD1408FC3DFFB1EF982396AF0112153C2C96F1FA344CBC9FCF` |

The copy thunk is `FF25 1C25BD00`, an indirect jump through preferred VA
`00BD251C`. Its raw import pointer is RVA `008B33EC`; the import-by-name bytes
at raw `008B33EE` are `memcpy` plus NUL. This verifies the copy primitive's
import identity instead of guessing from its arguments. Static disassembly
uses complete body boundaries; neither thunk nor engine is executed.

## Reproduce and validation

`scripts/Get-Ra3Ep1LinkedChunkRanges.ps1` imports prior sidecar/descriptor
evidence. `Assert-LinkedChunkCode` pins five regions and the memcpy binding.
`Get-LinkedChunkRange` computes checked half-open physical ranges:
`[8 + descriptorOffset, 8 + descriptorOffset + chunkSize)`.
All pinned raw manifest entries are walked in order, including zero-sized
chunks, and compared to inherited sidecar length snapshots. Ordered range
digests use invariant decimal fields `entryIndex`, `streamKind` (0/1/2),
start and end, separated by tabs and terminated with LF.

The last cumulative instance/relo/import endpoints equal the twelve exact
lengths recorded in [the sidecar report](RA3EP1_LINKED_SIDECARS.md), including
WorldBuilder.bin at 1,394,571,528 bytes and header-only audio.imp at 8 bytes.
No binary payload is read. The inherited audit reads 192 header bytes; the
final twelve header/metadata rechecks add 96, for **288 sidecar header bytes
per invocation**. Repeat tests invoke it twice. Header/timestamp equality is
not an atomic snapshot or full-payload integrity proof.

```powershell
# Reborn: reproduce concrete linked addressing and all raw-manifest range bounds without native execution or stream payload reads.
./scripts/Get-Ra3Ep1LinkedChunkRanges.ps1 -ImagePath 'D:\SteamLibrary\steamapps\common\Command and Conquer Red Alert 3 Uprising\Data\ra3ep1_1.0.game' -AsJson
# Reborn: test reviewed queue/reader/import identity, exact endpoints, repeated range digests and detached faults.
./scripts/Test-Ra3Ep1LinkedChunkRanges.ps1 -ImagePath 'D:\SteamLibrary\steamapps\common\Command and Conquer Red Alert 3 Uprising\Data\ra3ep1_1.0.game'
```

Tests cover four manifests, 55,519 entries, 166,557 ranges, repeat digests,
four positive pure-range cases (including empty-at-end/header-only), four
range rejections and nine detached code/import mutations. The previous linked
sidecar regression also passes. Only the existing repo-owned managed name-hash
diagnostic runs; no game, native compiler or codecs execute. C# compiler code
is unchanged, so no full-suite rerun is claimed; the prior count stays 165 groups.

## Next gate

Establish concrete source-object creation/binding on the outer loader paths,
then review the requested-count/capacity reuse lifecycle. Preserve native
read-result/diagnostic uncertainty separately from authoring metadata and
processor compatibility. The missing EP1 AUDIO header, ProcessingHash and
eventual in-game validation remain open. Effort stays approximately **52%
complete / 48% remaining**; stream addressing is clearer, game-ready coverage
has not increased.
