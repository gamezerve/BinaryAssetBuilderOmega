# EP1 bounded entry-pointer trace — October 9, 2026

## Result and limits

The ResourceManager TypeHash gate has three reviewed direct E8 call candidates:
preferred VAs `004CECE0`, `004CF420`, `004CFB66`. A bounded trace of the **first**
caller links a 20-byte descriptor row's entry pointer to a pending stack record,
then to the fourth argument of the hash gate. The pointer is not simply guessed
from matching field offsets. There is also a separate 48-byte copy branch.

This narrows the provenance gap but does **not** close it: construction of the
descriptor array and its connection to disk manifest bytes are unresolved.
The other two callers are not claimed as fully traced. The virtual I/O methods,
callee preservation of nonvolatile ESI under the observed x86 convention, and
all interprocedural effects have not been exhaustively verified.

The diagnostic-control wrapper at `0040EDB0` delegates to virtual byte offset
`64h` of the object held by `00CE959C`, forwarding both arguments. This resolves
the wrapper, **not** the implementation, policy, configuration or fatality of
the mismatch diagnostic. Compiler identity and admission rules remain unchanged.

## Reviewed path

| Preferred VA | Observation |
| --- | --- |
| `004CE630` | Caller entry; word at first argument object+4 must equal 7 for this path |
| `004CE88F` | Context pointer loaded; index at +16 checked against bound at +20 |
| `004CE8C0` | Descriptor array pointer loaded from context+8 |
| `004CE8C3`–`004CE8CD` | Row address is array + index*20; entry pointer read from row+12 |
| `004CE8DE` / `004CE8E1` | Descriptor row saved at pending record+8; entry pointer at pending record+4 |
| `004CE9CC`–`004CE9E8` | Separate branch requests 48 bytes, copies 12 dwords from the saved entry pointer |
| `004CEB12`–`004CEB1E` | Pending stack descriptor selected with 28-byte stride; saved entry pointer loaded into ESI |
| `004CEBE7`–`004CEC2B` | Entry word44 branches around relocation-like work and selects an additional transformation path |
| `004CEC8A` / `004CEC8D` | Entry+8 and entry+12 copied into local resource state |
| `004CECD3` | ESI pushed in the fourth-argument position for the hash gate |
| `004CECE0` | Direct call to `004AB5C0` |
| `004AB5CD` | Gate loads that fourth argument into ESI, before reading entry offsets 0/4/8/44 |

Context indices, descriptor rows and pending stack records are **different
layouts**. The descriptor's 20-byte stride and the pending record's 28-byte
stride are not changes to the on-disk manifest's 48-byte entry size.
Names in this report are analysis labels, not recovered C++ class definitions.
The version-word condition is observed on a caller object, not yet proven to
be a direct load from the file header. Allocation/copy evidence is not a proof
of allocation failure handling or full parser validation.

The gate allocates `24h` stack bytes and pushes three saved registers before
its `mov esi,[esp+40h]`. Thus the load is at original entry ESP + `10h`, or
argument four. The reviewed caller has eight right-to-left argument pushes;
the ESI push is fourth from the final push. This reasoning is implemented as
a pure arithmetic fixture, not live native execution.

Word44 being tested in additional work makes it operationally significant,
but does not yet prove its schema meaning or justify Boolean coercion.
Neither these branches nor the earlier hash-skip branches authorize compiler
bypass flags. Object metadata+12 remains a distinct opaque field.

## Exact evidence pins

Image SHA-256:
`ABA6A8825A744A78487837D78AD5992962257CE68404F639D600F11DEEA2580B`.
Preferred-image VAs are raw offsets + `00400000` for these code regions.
Seven bounded instruction slices are independently pinned:

| Raw offset | Bytes | SHA-256 |
| --- | ---: | --- |
| `000CE630` | 40 | `4CDEECD2EF5F73CB4693AAA168F6E6FC452FBB4E492A0B14E599C2F17BF8BB5F` |
| `000CE88F` | 85 | `8B7A74058728BB3DB25D8BD8E54DA2EEDF29622016C2FB195F8939712B82BBE6` |
| `000CE9CC` | 30 | `F540EF3F38161E24975B6E8BD0FADE73C86F8826D96988A888C8B4FC4A1C5FA5` |
| `000CEAF6` | 62 | `38724C11909FA9143840559C0F561AA4575388310006F45EA5DBA2DE8ADFAB0D` |
| `000CEBE7` | 70 | `C391AD9E1BD1B994014F519544965F658C1A02A5A957507059FC2EA1D11B9BF0` |
| `000CEC84` | 97 | `B20EC2B398A556FCCA1F6E4A6253F46E5F8CFAA54630C10F63E002EDBFA9AADD` |
| `0000EDB0` | 26 | `05D2F6D048EDD98FA3DB958279E0B9594EF6328363FAB6F981FBE28E2DF0CA98` |

These slices are not complete function/call-graph coverage. Microsoft dumpbin
static disassembly was checked at complete instruction boundaries; partial
instruction artifacts are excluded. The full image pin also binds intervening
bytes, without implying that every intervening instruction is understood.

## Reproduce and validation

`scripts/Get-Ra3Ep1EntryPointerTrace.ps1` imports the pinned hash-consumer and
registry audits. `Assert-EntryTraceSlices` pins reviewed code;
`Get-TraceEntryArgument` checks stack adjustment/push arithmetic. Its overlapping
raw E8 scan requires the three observed targets, but does not establish an
exhaustive call graph or indirect-call absence. The only executable invoked is
the already built repo-owned managed inspector for name hashing; the game,
native compiler and codecs are not executed. Bounded input policies and final
image SHA recheck are inherited. No source or game file is changed.

```powershell
# Reborn: reproduce the static descriptor/pending-record/fourth-argument trace without executing the engine.
./scripts/Get-Ra3Ep1EntryPointerTrace.ps1 -ImagePath 'D:\SteamLibrary\steamapps\common\Command and Conquer Red Alert 3 Uprising\Data\ra3ep1_1.0.game' -AsJson
# Reborn: check code evidence, stack fixtures, repeat JSON and private corruption rejection while preserving unresolved semantics.
./scripts/Test-Ra3Ep1EntryPointerTrace.ps1 -ImagePath 'D:\SteamLibrary\steamapps\common\Command and Conquer Red Alert 3 Uprising\Data\ra3ep1_1.0.game'
```

Validation covers three positive stack cases, two invalid-displacement cases,
seven detached code mutations, and repeat JSON. The existing hash-consumer
regression also passes. No C# build/full compiler-suite rerun is claimed;
the previously executed count remains 165 groups.

## Next gate and migration impact

Resolve the descriptor-array producer and the virtual source-reader methods
before claiming actual manifest-to-runtime entry provenance. A reviewed outer
dispatch at `004CFD08` invokes the first loader; an alternate dispatch at
`004CFD4A` invokes `004CED70`. The outer virtual method supplying its header
object and the descriptor pointer producer remain targeted RE work.
Separately identify the implementation behind diagnostic virtual slots `64h`
and `68h`, without guessing their policy from their return values.

Only after provenance and identity gates are independently satisfied should a
production processor profile be considered. Missing EP1 AUDIO/header inputs,
authoring ProcessingHash, other native layouts, SDK/WorldBuilder integration
and game-load validation still remain. Effort stays about **52% complete / 48%
remaining**; this milestone strengthens runtime provenance but adds no proven
working-game coverage.
