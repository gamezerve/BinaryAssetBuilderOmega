# EP1 1.1 wrapper-local loader lifecycle — October 9, 2026

## Result and actual version difference

Wrapper `004CFEB0` creates a fresh stack-local entry/descriptor context after
successful header reading. Both loader branches pass it as argument seven and
join normal cleanup, which calls `00449700` to release descriptor and entry
buffers. Separate invocations of this reviewed wrapper do not retain the prior
entry buffer capacity. Successful fresh preparation grows capacity to the
current requested count, satisfying the producer's count-equals-capacity
projection scope. The producer still loops over capacity, not requested count.

**The 1.1 wrapper pushes nine loader arguments and restores 24h/36 bytes.**
An independent disassembly of 1.0 confirmed its wrapper restores 20h/32 bytes
and has the older eight-argument call. The extra 1.1 argument cannot be removed
or given a meaning from address similarity. Context remains argument seven;
the additional argument is outside the lower seven positions. In the first
1.1 branch the eighth argument is literal 1, while the ninth is loaded from a
wrapper argument. Complete semantics of those arguments and every downstream
use are not recovered here. Do not treat all 1.1 changes as relocated 1.0 code.

These are scoped normal-flow findings, not proof of allocator failure safety,
exceptions/nonlocal exits, reentrant callbacks or all possible source bindings.
No native code, game, compiler or codec ran. No production behavior changed.

## Independent location and exact image

An overlapping raw E8 scan of the pinned 1.1 .text region for target `004CE8F0`
yielded candidate `004CFFD0`. Disassembly back to the preceding wrapper boundary
identified `004CFEB0`. Both branches, initialization and cleanup were then
reviewed at instruction boundaries. Cleanup directly targets `00449700`;
its complete body was inspected. The previously recovered constructor's full
107-byte body at `004AAC10` was also reviewed and pinned independently.
No fixed 1.0-to-1.1 delta was accepted as proof.

Target: `D:\Program Files (x86)\Steam\steamapps\common\Command and Conquer Red Alert 3 Uprising\Data\ra3ep1_1.1.game`.
SHA-256: `B1DA83EFD229570BAB9DE3A14E48B2D1E6128E302BD52384858FA8A413AE330B`.
All method addresses are preferred image VAs, not live ASLR addresses.

## Fresh context and stack mapping

Let B be wrapper ESP after its 60h local allocation and saved EDI push.
Header storage begins at B+30h. The header method is source virtual+0Ch;
the wrapper reaches context initialization only when its Boolean return is
true. EDI points to the source handle wrapper, whose first word is source object.

| Field | Location | Observed initialization |
| --- | --- | --- |
| Entry pointer | B+4 / context+0 | zero |
| Entry capacity | B+8 / context+4 | zero |
| Descriptor pointer | B+12 / context+8 | zero |
| Descriptor capacity | B+16 / context+12 | zero |
| Entry index | B+20 / context+16 | zero |
| Requested count | B+24 / context+20 | header+16 loaded from B+40h |
| Source handle pointer | B+28 / context+24 | EDI |

The first branch forms context address after two temporary pushes at
`004CFFB6`; B-adjusted ESP+0Ch is B+4. Its push at `004CFFBA` is the third of
nine right-to-left argument pushes, hence argument seven. The call at
`004CFFD0` targets `004CE8F0`. Before the producer call, that loader's context
load at ESP+B4h accounts for 90h locals and two saved registers:
`B4h - 90h - 2*4 = 1Ch`, again argument seven.

The other branch forms the same B+4 address at `004CFFED`, pushes it at
`004CFFF1`, and calls `004CF030` at `004D000A`. The complete second loader
body is not audited by this lifecycle report. Both wrapper branches restore
nine arguments at `004D000F`, set ECX=B+4 and call cleanup at `004D0016`.
The different branch dispatch depends on global `00CF1530`; its complete
configuration semantics are outside this report.

## Normal cleanup and reader construction

`00449700` traverses a context list rooted at context+20h, invokes each node's
source virtual+24h, releases list nodes, frees context+8 descriptor buffer,
then context+0 entry buffer through `004169B0`. It does not establish behavior
on invalid list links, allocation failures or nonlocal exits. The wrapper next
invokes its primary source virtual+24h at `004D0022`. Full concrete reader
cleanup, caches and ownership require the following factory/lifetime audits.

Constructor `004AAC10` writes vtable `00BF8EA0`, clears source/queue/linked
state fields including reader+18h, +50h and +54h, retains constructor arguments,
prepares path-related data and calls `004AA300`. This is construction evidence,
not a proof that every source/cache hit selects this reader. External helpers
and all constructor call sites are not fully audited here.

The pure capacity model assumes successful allocation. Successive counts
17,339, 11,357, 1 and 0 each start at zero capacity, with entry allocation
48*count and descriptor allocation 20*count. Zero count follows the producer's
early return. The 100,000-entry limit is diagnostic policy, not an engine cap.
Other callers that reuse an existing context remain unresolved.

## Exact pins and tests

| Raw offset | Bytes | SHA-256 |
| --- | ---: | --- |
| CFEB0 wrapper header dispatch | 30 | `BDC7C7B27A1A3E6130889372D07C7C7402BC29DE102F2ABB656A1989E01079FA` |
| CFF5C initialization/branches/normal cleanup | 205 | `2AA453BC67F93DD4A28BAD0FE41542AE1DF3DE16687F954A037EE2AF852592E0` |
| 49700 complete context cleanup | 79 | `F14029ACABF98742B30F48AC73571A9AE5F2C65369150A2D9A5568041FD048BB` |
| AAC10 complete reader constructor | 107 | `E7D7E6B7922257695D0534A545D4E544B0094FBF0E11A7F7F23B73F2DBDEB4A9` |

`Get-Ra3Ep11LoaderLifecycle.ps1` imports the pinned 1.1 descriptor audit,
checks the seven-of-nine stack mapping and rechecks image SHA at completion.
Four fresh-invocation fixtures and seven detached code faults pass. Faults
cover zero initialization, count load, both loader calls, destructor call,
buffer-free binding and vtable construction. `Test-Ra3Ep11LoaderLifecycle.ps1`
compares every stable nested report field against repeat JSON. No original 1.0
guard was relaxed; the older report remains correctly scoped to eight arguments.

```powershell
# Reborn: verify fresh 1.1 wrapper context, nine-argument call mapping and normal buffer release without running native code.
./scripts/Test-Ra3Ep11LoaderLifecycle.ps1 -ImagePath 'D:\Program Files (x86)\Steam\steamapps\common\Command and Conquer Red Alert 3 Uprising\Data\ra3ep1_1.1.game' -BaselineImagePath 'D:\Program Files (x86)\Steam\steamapps\common\Command and Conquer Red Alert 3 Uprising\Data\ra3ep1_1.0.game'
```

## Remaining migration gates

Follow-up: [loader argument provenance](RA3EP11_LOADER_ARGUMENTS.md) traces
the ninth argument to wrapper argument six and distinguishes it from the
preserved eighth flag already present in 1.0. Ninth-argument consumption and
full semantics remain open; this lifecycle report retains its original scope.

Next independently locate 1.1 factories, source ownership and reader lifetime,
and trace the added loader argument's consumers. This milestone strengthens
the fresh-capacity projection scope but does not establish complete stream
pointer provenance, authentic authoring ProcessingHash, native emission ABI or
modconfig package loading. The 165-group compiler suite was not rerun for this
static audit. Weighted effort stays **52% complete / 48% remaining**; no usable
SDK or successful game-load result is claimed.
