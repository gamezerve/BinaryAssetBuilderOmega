# EP1 source forwarding and pointer membership — October 9, 2026

## Result

The ordinary reference-path helper `0045F000` is now resolved as a **reader
pointer-membership test**, not a schema/hash/asset-content validator. It walks
three global lists at preferred VAs `00CEA44C`, `00CEA484`, `00CEA4BC`, spaced
by `38h`. Each list uses its head address as sentinel, next at node+0 and reader
pointer at node+8. Equality returns AL=1; exhausting all three returns AL=0.
The reviewed helper does not call methods or read pointed-to asset contents.
The caller separately skips null readers before calling it. No list-corruption,
cycle, concurrency or native memory-safety guarantee is inferred.

The factory handle in the reviewed first loader comes from **the same wrapper
source handle** used for header reading and normal source cleanup. The list
owner remains a different wrapper argument. The list iterator `004D01E0`
provides a scoped downstream source-forwarding path into that wrapper.

This closes two local dataflow questions. It does not establish every cached
object's type, a live concrete vtable for all sources, complete container
ownership, or that a specific earlier head+8 store is traversed later.

## Argument mapping

| Scope | Source/factory handle | List owner |
| --- | --- | --- |
| Wrapper `004CFBF0` | entry argument 2, saved in EDI | entry argument 3 |
| First loader `004CE630` | entry argument 3, EDI forwarded by wrapper | entry argument 4 |
| Iterator `004D01E0`, direct branch | address of current node+8 | iterator owner (entry argument 1) |
| Iterator alternate branch | address of local handle with word 0 copied from node+8 | same iterator owner |

In the wrapper, `6Ch - 60h - 4 = 8` identifies argument 2, and
`70h - 60h - 4 = 0Ch` identifies argument 3. EDI drives header slot+0Ch
at `004CFBF8`, is forwarded by `push edi` at `004CFD01` as first-loader
argument 3, and drives normal cleanup slot+24h at `004CFD5B`.
Wrapper argument 3 is loaded at `004CFCD1` and pushed at `004CFCF8`
as first-loader argument 4. The earlier factory audit independently accounts
for the pathname push at the loader's virtual factory dispatch.

The second dispatch branch calls `004CED70`; its full reference dispatch
and admission flow is not newly analyzed here. The wrapper's failed-header
diagnostic path is pinned as part of its complete body, but is not modeled
as a successful source-loading path.

## Scoped iterator consumer

`004D01E0` receives an owner pointer in EBP. It takes owner+0Ch as list sentinel
address, loads the first link and tests for an empty list. Within the loop:

- Direct branch: `004D02F1` forms node+8, passes this address as wrapper
  argument 2, and passes EBP as wrapper argument 3; call at `004D02FA`.
- Alternate branch: `004D024D` reads node+8 into a local handle's first word;
  node+10h is copied into another handle field. The local handle address is
  wrapper argument 2, EBP is argument 3; call at `004D02A5`.
- `004D0302` follows node+0 and compares with owner+0Ch before the next
  iteration. The wrapper calls clean up 20 argument bytes on return.

This establishes consumption of the list's node+8 source-handle field.
It **does not** reinterpret the previous loader's literal `[head+8]` store
as a write into its newly allocated node. Head movement/initialization,
payload layout, insertion validity and precise traversal of a particular
stored reader remain separate questions. The iterator's alternate cleanup
calls and workspace destruction are not claimed to establish reader ownership.

## Exact pins and tests

Engine SHA-256:
`ABA6A8825A744A78487837D78AD5992962257CE68404F639D600F11DEEA2580B`.

| Raw offset | Bytes | SHA-256 |
| --- | ---: | --- |
| `0005F000` complete membership helper | 55 | `E6804CB2E752BEA9009AD8AD011C43275DCF803C9AD5177162EDFBD00DA8F4FE` |
| `000CFBF0` complete wrapper | 377 | `47C0C9B3BC24EC1C4458BB469A712FD1B6D79817364BC946026EEFD7D335631C` |
| `000D01E0` complete iterator | 320 | `D35A1502107C2D5D192D5ED60E7EFBA8977ED083623E5968DD93409E94512BC8` |

```powershell
# Reborn: reproduce wrapper/iterator owner forwarding and ordinary pointer-membership semantics without native execution.
./scripts/Get-Ra3Ep1ReaderOwnership.ps1 -ImagePath 'D:\SteamLibrary\steamapps\common\Command and Conquer Red Alert 3 Uprising\Data\ra3ep1_1.0.game' -AsJson
# Reborn: verify scoped pins, six pure membership cases, two invalid group counts, eleven detached code mutations and repeat JSON.
./scripts/Test-Ra3Ep1ReaderOwnership.ps1 -ImagePath 'D:\SteamLibrary\steamapps\common\Command and Conquer Red Alert 3 Uprising\Data\ra3ep1_1.0.game'
```

The pure model compares uint32 reader values across three pre-enumerated
synthetic lists; it never dereferences native addresses. Cases cover empty
lists, an absent pointer, a hit in each list and zero-pointer equality. The
zero case describes helper equality only, not null admission by its caller.
The 100,000-values-per-group limit is diagnostic policy, not native behavior.
Code mutation tests operate only on cloned memory snapshots.

The inherited factory/lifecycle/manifest evidence remains checked. Only the
existing repo-owned managed name-hash diagnostic executes; native game, factory,
allocator and codec code does not. Engine SHA is rechecked at completion.
No asset payload is read, and no game/reference files are changed. Tests cover
repeat JSON and eleven private code faults; factory regressions pass separately.
The full compiler suite was not rerun; its previous total remains 165 groups.

## Remaining gates

Follow-up: [sentinel insertion and reader lifetime](RA3EP1_READER_LIST_LIFETIME.md)
resolves the head+8 store as fresh-node payload under healthy circular-sentinel
invariants, and distinguishes scoped resource cleanup from list-triggered
reader release. All live initializers/cache behavior remain unproven. A distinct
[1.1 image](RA3EP11_MODCONFIG_BASELINE.md) now requires independent rebasing.

Resolve list insertion/head movement and reader lifetime before claiming the
specific stored object is consumed. Trace initial root-reader construction and
cache lifetime to establish concrete vtable provenance. Raw-tag-2 admission
helpers remain unresolved. Authentic authoring ProcessingHash, missing EP1
AUDIO header, native emission and actual game loading still block a usable SDK.

Overall weighted effort estimate remains **52% complete / 48% remaining**.
This milestone narrows static-RE uncertainty; it is not a production build or
proof of a successfully loaded mod.
