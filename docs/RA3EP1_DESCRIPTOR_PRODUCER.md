# EP1 concrete reader and descriptor producer — October 9, 2026

## Result

The intermediate descriptor producer is identified at preferred VA `004496A0`.
It reads 48-byte asset entries through a source object's virtual slot `18h`,
then builds 20-byte descriptors whose +12 pointer addresses those entries.
Descriptor offsets 0/4/8 accumulate entry sizes at 32/36/40: instance, relocation
and import chunk lengths. The first reviewed loader calls this producer at
`004CE680`; this connects to the previously recovered pending-record/hash-gate
caller path.

A concrete reader vtable at `00BF2E78`, installed by code at `004AA962`, maps
slot `0Ch` to `004AA3B0` and slot `18h` to `00449B80`. The reviewed header path
requests exactly **52 bytes**, while its entry reader seeks to byte 52 and
requests **count*48 bytes**. This matches the four pinned physical EP1 v7
manifests: a four-byte container prefix plus the inspector's 48-byte header.

This is static concrete-reader and producer evidence, not a native execution
trace. All four stock manifests have **IsLinked=1**. The concrete entry reader
conditionally calls `00449760` after reading the table. That linked-entry
transformation is **not recovered**, so complete disk-entry-to-hash-consumer
provenance remains open. The recovered vtable is one concrete implementation,
not proof that every source object on every call path uses that implementation.

## Reader/producer contracts

| Code/address | Reviewed observation |
| --- | --- |
| `004AA962` | Writes concrete vtable `00BF2E78` into the object base |
| `004AA4C4`–`004AA557` | Opens through file-manager call `004D8280`, stores source at object+24, reads 52 bytes, checks returned length, seeks past count*48, retains count/version/hash-like header fields |
| `00449B80` | Requires requested entry count equal object+28 |
| `00449B9D`–`00449BB7` | Seeks to physical offset 52; reads count*48 into supplied buffer |
| `00449BB9`–`00449BC3` | Non-null object+84 selects linked postprocessing at `00449760` |
| `004496A0` | Zero requested count returns true without allocating/reading |
| `004496B4`–`004496E0` | Grows 48-byte entry buffer when requested count exceeds capacity |
| `004496E2`–`004496F4` | Calls source virtual slot 24 with entry buffer/count |
| `00449703`–`00449707` | Allocates descriptor storage via `00417520` on context+8 |
| `00449727`–`0044974D` | Writes entry/source pointers and cumulative chunk offsets; advances entry by 48 and descriptor by 20 |

The producer's native loop endpoint uses **entry-buffer capacity**, not just
the requested count. The projection tool therefore only models a fresh
count-equals-capacity context. It does not silently replace the observed loop
with count-only semantics or assert safe reuse when a later stream is smaller.
Native allocation failure, integer-overflow behavior and linked processing are
not modeled as compiler policy. The allocation helpers' observed overflow
sentinel arithmetic is not a recovered complete memory-safety contract.

For the fresh projection, descriptor offsets are:

| Offset | Value before advancing to next entry |
| --- | --- |
| 0 | Sum of preceding InstanceDataSize values |
| 4 | Sum of preceding RelocationDataSize values |
| 8 | Sum of preceding ImportsDataSize values |
| 12 | Current 48-byte entry pointer |
| 16 | Source object pointer |

These are relative cumulative chunk positions, not guaranteed absolute file
offsets: stream prefixes, linked normalization and source-specific reading still
need independent tracing. No `.bin`, `.relo` or `.imp` payload was read here.

## Actual raw-manifest corroboration

All four inputs are independently pinned by
`docs/RA3EP1_TYPE_TABLE_EVIDENCE.json`; their raw SHA-256 values are rechecked.
Every file has zero DWORD prefix, version 7 at byte 4, little-endian flag at
byte 6, linked flag 1 at byte 7, and AssetCount at byte 16.

| File | Raw entries | Sum instance bytes | Sum relocation bytes | Sum import bytes |
| --- | ---: | ---: | ---: | ---: |
| Global Data/data/global.manifest | 11,357 | 2,768,540 | 239,400 | 221,628 |
| Static Data/data/static.manifest | 13,872 | 357,478,156 | 6,916,328 | 124,528 |
| WorldBuilder/data/worldbuilder.manifest | 17,339 | 1,394,571,520 | 21,616,296 | 113,724 |
| EnglishAudio/data/audio.manifest | 12,951 | 940,768 | 108,728 | 0 |

All **55,519 raw records** fit the physical header/table bounds. Each instance
sum equals its header's TotalInstanceDataSize. Each fresh raw descriptor
projection repeats its ordered SHA digest. This includes WorldBuilder metadata
without dumping its large binary payload. Raw word44 is preserved in the
projection; its full runtime/schema semantics are not recovered here.

## Evidence pins

Pinned engine SHA:
`ABA6A8825A744A78487837D78AD5992962257CE68404F639D600F11DEEA2580B`.
Addresses are preferred-image VAs, not live observations.

| Raw offset | Bytes | SHA-256 |
| --- | ---: | --- |
| `007F2E78` vtable | 28 | `03F6FC4DB3D4D907CA8D003A92708A68505B84A2C35B8E985B5DB827F1B88AD1` |
| `000AA962` vtable write | 6 | `DE7CC2D0B7061436CF605F76164FC003D1DE092584CA07A3856CE42197888E49` |
| `000AA4C4` header region | 148 | `4319BB7821C2C32C7080B235D120D45B7EFAA1C7E6657CCCCFE556C0D72EF5CE` |
| `00049B80` entry reader | 86 | `FAD82C53417A95CC7799D26D460D1AA0C75F62802F6DC6B510D620AB51089D7B` |
| `000496A0` descriptor producer | 187 | `CD2354457736A189856ECE49A2C2A148632F9B5596175602DF4D72AF76116224` |
| `00017520` descriptor allocator | 59 | `DA8EADC5C5A1E7D529FC357D979062260D7D91C34FCF7BE15C60A2817C4D2684` |
| `000CE679` loader call region | 20 | `4C33E614BD43184C31838A68EFE9FD076147EC3BFB8B94C2206B9CE153D6C193` |

Static dumpbin review retained complete instruction boundaries. The header
region is a bounded slice of a larger function, not its complete implementation;
the vtable slice is seven slots, not a complete class ABI. File-manager modes
and the underlying low-level I/O implementations are not fully recovered.
Separately reviewed code at `00418290` uses
`%sassets\%08x.%08x.%08x.%08x.asset`: a distinct single-asset path, not evidence
that the shipping `.manifest/.bin/.relo/.imp` path is replaced by `.asset` files.

## Reproduce and validation

`scripts/Get-Ra3Ep1DescriptorProducer.ps1` imports existing bounded image,
runtime-table, registry and pointer-trace audits. `Assert-DescriptorProducerCode`
pins reviewed code; `Read-FreshDescriptorProjection` projects only fresh raw
records using wide arithmetic. It rejects overflow rather than emulating
native 32-bit wraparound; that is diagnostic tool policy, not claimed engine
behavior. Ordered projection digest rows contain entry offset, cumulative
instance/relocation/import bytes, and preserved raw word44, separated by tabs
and terminated by LF using invariant decimal formatting.

```powershell
# Reborn: reproduce concrete reader/producer evidence and four raw-manifest projections without native execution or payload dumps.
./scripts/Get-Ra3Ep1DescriptorProducer.ps1 -ImagePath 'D:\SteamLibrary\steamapps\common\Command and Conquer Red Alert 3 Uprising\Data\ra3ep1_1.0.game' -AsJson
# Reborn: check pinned code, repeated projections, detached raw faults and unknown-semantics refusal flags.
./scripts/Test-Ra3Ep1DescriptorProducer.ps1 -ImagePath 'D:\SteamLibrary\steamapps\common\Command and Conquer Red Alert 3 Uprising\Data\ra3ep1_1.0.game'
```

Validation covers four manifests/55,519 raw entries, repeated projection
digests, a private three-entry fixture with zero chunks and opaque word44=7,
five raw-fixture rejections (version/count/truncation/overflow/total), and seven
detached code/vtable faults. The previous entry-pointer regression also passes.
No game/compiler/codec target executes; only the existing repo-owned managed
name-hash diagnostic is invoked. No C# build/full-suite rerun is claimed;
previously executed compiler groups remain 165.

## Next gate

Review `00449760` and its callees to determine precisely how linked entries
change before descriptor creation. Keep requested-count/capacity reuse as a
separate question, then establish concrete source-object binding on the outer
load paths. The missing EP1 AUDIO header, authoring ProcessingHash, additional
native layouts and eventual game-load validation remain separate blockers.
Production guards are unchanged. Engineering effort remains approximately
**52% complete / 48% remaining**; concrete raw-reader evidence is not yet a
working production SDK or proof of linked resource loading.
