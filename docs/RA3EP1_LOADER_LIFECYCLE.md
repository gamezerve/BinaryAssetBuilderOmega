# EP1 wrapper-local loader lifecycle — October 9, 2026

## Result

The wrapper at preferred VA `004CFBF0` creates a **fresh stack-local entry/
descriptor context on every successful header-read path**. Both dispatch
branches pass this context to their loader and join normal cleanup at
`004CFD56`, which calls `00449650`. The destructor frees descriptor and entry
buffers. Entry capacity therefore is not reused across separate invocations
of this reviewed wrapper.

This resolves the previously open capacity-versus-count question **for this
wrapper scope**: fresh zero capacity grows to the current requested count on
successful preparation. The producer's capacity-based loop remains unchanged;
we do not rewrite it as count-based or claim all other callers are fresh.
Allocation failures, exceptions/nonlocal exits, other reader paths and all
possible reentrant/callback behavior are outside this proof.

Concrete reader construction at `004AA950` is independently pinned: it writes
vtable `00BF2E78`, zeroes source/queue/linked-state fields, records constructor
arguments and calls `004AA040`. Direct factory call candidates at `004AAB3A`
and `004AAD09` identify further binding targets; their complete factory/cache
and outer-loader flow is not yet proven. No all-source-object binding claim.

## Stack and ownership trace

Let B be wrapper ESP after its `60h` local allocation and saved EDI push.
Header storage starts at B+`30h`. After a successful header method returns:

| Context field | Location | Observed initialization |
| --- | --- | --- |
| Entry buffer pointer | B+4 / context+0 | zero |
| Entry capacity | B+8 / context+4 | zero |
| Descriptor buffer pointer | B+12 / context+8 | zero |
| Descriptor capacity | B+16 / context+12 | zero |
| Current entry index | B+20 / context+16 | zero |
| Requested count | B+24 / context+20 | header AssetCount, loaded from B+64 |
| Source handle wrapper | B+28 / context+24 | EDI, whose first word holds source object |

Both branches take the address of B+4 after accounting for temporary pushes.
It is argument seven of `004CE630` or `004CED70`. In the first loader, before
its producer call, `B4h - 90h - 2*4 = 1Ch` from original entry ESP: argument
seven. This matches the pointer passed to `004496A0`.

Both branches join at `004CFD4F`, restore 32 argument bytes, and call the
context destructor with ECX=B+4. That destructor releases additional list
resources, frees context+8, then context+0. The wrapper then invokes source
virtual slot `24h`; its full implementation/lifecycle is not claimed here.
Failed header reading does not enter this context initialization path.

The model covers successful fresh allocation only. Successive sample requests
17,339, 11,357, 1 and 0 each start at capacity zero, rather than retaining the
first request's capacity. Zero count follows the producer's early return.
These are pure fixtures, not an execution trace or memory allocator emulator.

## Exact code pins

Engine SHA:
`ABA6A8825A744A78487837D78AD5992962257CE68404F639D600F11DEEA2580B`.

| Raw offset | Bytes | SHA-256 |
| --- | ---: | --- |
| `000CFBF0` wrapper header dispatch | 30 | `BDC7C7B27A1A3E6130889372D07C7C7402BC29DE102F2ABB656A1989E01079FA` |
| `000CFC9C` fresh context/both branches/normal cleanup | 205 | `9403EC39651F48BC1A77C7E0C1265024E6095DD2D1651381AEFF4DB972007556` |
| `00049650` context destructor | 79 | `DDF9758DEDBDBA7E89052D720AB0F9C5F30247BB714AC57ECB4152258030BB4C` |
| `000AA950` reader constructor | 107 | `E6BB4354DEBD007AC2B4E171B70E143268944C3E8FD138D346EC9AE7CBD37B1A` |

Static disassembly was reviewed at complete instruction boundaries. These are
scoped code pins and normal-flow reasoning, not full program verification.

## Reproduce and validation

`scripts/Get-Ra3Ep1LoaderLifecycle.ps1` imports the existing descriptor audit.
`Assert-LoaderLifecycleCode` pins reviewed regions; `Get-FreshLoaderCapacity`
models only successful fresh preparation under a 100,000-entry diagnostic
bound. `Get-TraceEntryArgument` supplies the existing pure stack derivation.
Four authentic manifest counts remain independently pinned by inherited checks.
No native payload or engine/codec code is executed. Only the existing repo-owned
managed name-hash diagnostic runs, and engine SHA is rechecked after review.

```powershell
# Reborn: reproduce scoped wrapper ownership and reader construction without native execution or global reuse claims.
./scripts/Get-Ra3Ep1LoaderLifecycle.ps1 -ImagePath 'D:\SteamLibrary\steamapps\common\Command and Conquer Red Alert 3 Uprising\Data\ra3ep1_1.0.game' -AsJson
# Reborn: check fresh successive/zero-count fixtures, exact lifecycle pins, argument-seven binding and detached code faults.
./scripts/Test-Ra3Ep1LoaderLifecycle.ps1 -ImagePath 'D:\SteamLibrary\steamapps\common\Command and Conquer Red Alert 3 Uprising\Data\ra3ep1_1.0.game'
```

Tests cover repeat JSON, four fresh invocation fixtures, argument-seven binding
and seven detached code mutations. Descriptor-producer regressions also pass.
No C# build or full compiler-suite rerun is claimed; the previously executed
count stays 165 groups. No source/game files or production guards are changed.

## Next gate

Trace the factory/cache return into the outer loader's source list to bind
the concrete reader object, rather than inferring it from its vtable alone.
Keep other-call-site capacity reuse and allocator failure semantics separate.
Authentic authoring ProcessingHash, missing EP1 AUDIO header and game loading
remain unfinished. Estimate stays approximately **52% complete / 48% remaining**;
scoped ownership is resolved, not a usable SDK release.
