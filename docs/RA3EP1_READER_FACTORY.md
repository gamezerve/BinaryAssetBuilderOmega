# EP1 referenced-reader factory/cache/storage — October 9, 2026

## Result and scope

The referenced-stream loop in `004CE630` reads a signed byte tag followed by
a NUL-terminated pathname. Tag **2** dispatches virtual slot `2Ch`; every
other byte value dispatches slot `28h`. This is observed routing, not a claim
that every possible tag is valid input or that tag 2 has proven patch semantics.

Concrete reader vtable `00BF2E78` maps these slots to `004AABC0` and `004AABA0`.
The latter calls `004AA9C0` with pathname and constant 1. Both factories can
return a cached pointer at lookup-node offset `20h` without checking its vtable.
Both successful allocation branches call the already pinned constructor
`004AA950`, which installs `00BF2E78`. This proves the scoped new-reader path,
not the provenance of every cached object or the live factory-owner vtable.

For the ordinary route, a non-null result is kept in EBP. If `0045F000`
returns AL=0, the loader performs list-link writes and stores EBP at
`[head+8]`, where head is loaded from `[list-owner+0Ch]`. This is **not** a
proven write into the freshly allocated node. The instruction sequence must
not be paraphrased as a generic container insertion with guessed ownership.

## Exact branch observations

| Observed property | Slot 28h route | Slot 2Ch / raw tag 2 route |
| --- | --- | --- |
| Concrete table target | `004AABA0` → `004AA9C0` | `004AABC0` |
| Cache address | `00CBDADC` | `00CBDAF8` |
| Cache sentinel compared | `00CBDAE0` | `00CBDAFC` |
| Cached reader load | `004AAB0A`, node+32 | `004AACE0`, node+32 |
| Object allocation size | `6Ch` | `6Ch` |
| Constructor call | `004AAB3A` | `004AAD09` |
| Constructor arguments in logical order | prepared path, 1, 1, 0, 1 (through wrapper) | prepared path, 0, 0, 1, 0 |
| Cache publication in reviewed body | `004AAB68` helper, `004AAB71` store | none on miss branch |
| Loader handling | helper `0045F000`, conditional head storage | `0077BFA0`, `00416FD0`, conditional virtual cleanup+24h |

Argument/field meanings and path-transform helper `009AB620` remain unnamed.
These are analysis labels, not recovered C++ symbols. The ordinary factory's
null-allocation branch reaches `004AAB45` with ESI=0 and dereferences ESI+10h;
it is not evidence of safe recoverable allocation failure. The other factory
returns zero on its reviewed null-allocation branch. Native allocator behavior
and initialization helper callbacks are not modeled or executed.

## Stack and storage precision

Four saved registers and `90h` locals are active in the reference loop.
At `004CE738` / `004CE748`, pathname is still pushed: the `B0h` stack load
maps to entry argument **3** (`B0h - 90h - 10h - 4 = 0Ch`). The concrete
factory methods return with `ret 4`, consuming that pathname argument.
At `004CE7A2`, the same displacement, now without the extra push, maps to
entry argument **4** (`B0h - 90h - 10h = 10h`). These are distinct owner
arguments; no aliasing or equality between their pointer values is proven.

At `004CE7AE`, helper `00485EC0` allocates `34h` bytes and initializes an
embedded structure at allocation+8. Its reviewed body does not write through
the supplied owner ECX and returns the allocation in EAX. The caller writes
links at allocation+0/+4, then reloads the existing head and writes the reader
at that head+8. Full container semantics, destruction and subsequent recursive
use of the stored object remain follow-up work. In particular, this trace does
not establish that the live dispatch owner has the concrete reader vtable.

## Reproduction and evidence pins

Engine SHA-256:
`ABA6A8825A744A78487837D78AD5992962257CE68404F639D600F11DEEA2580B`.

| Raw offset | Bytes | SHA-256 |
| --- | ---: | --- |
| `000AA9C0` ordinary factory | 472 | `E339978141CB185F24DDD32B8EA5F71578EFF4A0ADF55C5A1163E6477C5AE739` |
| `000AABA0` wrapper | 18 | `337C86181301781B021A6A8F314F589B566900970C132979DFD67C68B386B1C5` |
| `000AABC0` tag-two factory | 354 | `AE3CBA65D3860C6BF533E15070DBC23CDB11CBB6BE6DA2DD9F35C40511439651` |
| `000CE713` tag/dispatch/storage | 183 | `2F6436AB9A9A20C8E605FD9A1E9B55F720D15CF307CF6C4FC1C0C0A597DE8A00` |
| `007F2E78` concrete table | 48 | `8F602650019BA3A0D9152B05FFB5CFA9E43FAF40D2C6912918F95DCFEFE60D23` |
| `00085EC0` allocation helper | 81 | `1E52DCE34B1DDDD46811A0663CAB1B72D428C53232E7A052C66EBFADADD7533C` |

```powershell
# Reborn: reproduce scoped factory/cache/list-head evidence without executing the native engine or reading asset payloads.
./scripts/Get-Ra3Ep1ReaderFactory.ps1 -ImagePath 'D:\SteamLibrary\steamapps\common\Command and Conquer Red Alert 3 Uprising\Data\ra3ep1_1.0.game' -AsJson
# Reborn: test repeated reports, six signed-byte routing fixtures, two invalid tags and eleven private code mutations.
./scripts/Test-Ra3Ep1ReaderFactory.ps1 -ImagePath 'D:\SteamLibrary\steamapps\common\Command and Conquer Red Alert 3 Uprising\Data\ra3ep1_1.0.game'
```

The audit imports the existing lifecycle/descriptor evidence chain, checks
bounded engine snapshots and rechecks engine SHA at completion. Only the
existing repo-owned managed name-hash diagnostic executes. Native factories,
allocator, admission helpers and game code are never executed. No asset payload
is read; inherited manifest metadata is still independently checked.

Tests cover six exact code/table pins, distinct owner stack arguments, six
tag routes, two rejected out-of-byte tags, eleven detached code faults and
repeat JSON. Lifecycle regressions are checked separately. No full compiler
suite rerun is claimed; the previously executed total remains 165 groups.

## Next gates and remaining work

Trace argument-three factory-owner provenance from the outer wrapper, and
argument-four head storage consumers/destruction before asserting complete
source binding. Recover admission helper behavior and cache population/lifetime
without guessing ownership. Root-source construction, authoring ProcessingHash,
missing EP1 AUDIO header, native emission and in-game validation remain open.

Overall weighted effort remains approximately **52% complete / 48% remaining**.
This scoped static-RE milestone does not unlock a usable SDK or production build.
