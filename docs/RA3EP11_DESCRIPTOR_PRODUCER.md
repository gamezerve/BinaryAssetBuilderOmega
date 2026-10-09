# EP1 1.1 concrete reader and descriptor producer — October 9, 2026

## Result and limits

The producer at `00449750` requests raw entries through reader virtual slot
18h, then writes 20-byte descriptors containing cumulative stream offsets,
entry pointer+12 and source pointer+16. The reviewed concrete reader's slot
18h points to `00449C30`, which seeks to byte 52 and requests count*48 bytes.
Together with the [descriptor-to-gate trace](RA3EP11_ENTRY_POINTER_TRACE.md),
this establishes a scoped concrete physical-read/producer/consumer connection.

It does not prove that every live source selects this reader, that the linked
post-read helper leaves every entry unchanged, or that reused capacity is safe.
No native code ran; no game installation, source assets, compiler admission
policy or original 1.0 evidence changed. All addresses are preferred VAs,
not observed live addresses.

Target: `D:\Program Files (x86)\Steam\steamapps\common\Command and Conquer Red Alert 3 Uprising\Data\ra3ep1_1.1.game`.
SHA-256: `B1DA83EFD229570BAB9DE3A14E48B2D1E6128E302BD52384858FA8A413AE330B`.

## Locating and binding the code

Following the first reviewed loader back from its gate call identified the
direct call at `004CE940` to producer `00449750`. Its indirect entry read is
through source wrapper context+18h, then source vtable+18h. An independent
whole-.text search for the entry method's first 20 instruction bytes yields
only raw `00049C30`. Searching aligned .rdata pointers to that body identifies
raw `007F8EB8`; subtracting slot 18h gives concrete vtable `00BF8EA0`.
Its slot 0Ch points to header method `004AA670`.

Two .text byte candidates write that vtable with `C7 06 A0 8E BF 00`:
raw `0009B702` and `000AAC22`. The constructor at `004AAC10` was disassembled
and its write at `004AAC22` pinned. The second candidate is not assumed to be
another constructor or an exhaustive lifecycle result. No constant 1.0-to-1.1
address delta was used as proof.

## Physical reads

Header path `004AA784–004AA818` opens the source through `004D84F0`, retains
the resulting underlying object at reader+18h, requests 34h bytes into the
header argument and checks the returned count equals 34h. On a short read it
closes through the source's virtual+8 and clears reader+18h. On success it seeks
forward by header+16 count times 48; a negative seek result closes/clears and
returns false. It retains count at reader+1Ch, header+8 at reader+24h and
version word header+4 at reader+20h. The producer loader separately requires
version 7. The earlier header-method path resolution is not fully audited here.

Entry method `00449C30` requires its requested count equals reader+1Ch,
seeks to byte 52 using underlying virtual+14h, and requests count*48 bytes
through underlying virtual+0Ch into the caller's buffer. The reviewed body
does **not** check the seek/read result before proceeding. Reader+54h non-null
routes to `00449810`; otherwise it returns true. The linked helper semantics
are unresolved in this 1.1 milestone, even though a corresponding 1.0 helper
was previously found to set up sidecars. Do not transfer that conclusion by
address resemblance alone.

## Producer layout and capacity caveat

| Context/record field | Reviewed use |
| --- | --- |
| Context+0 / +4 | entry buffer pointer / capacity |
| Context+8 / +12 | descriptor buffer pointer / capacity |
| Context+20 | requested entry count |
| Context+24 | source wrapper pointer |
| Descriptor+0 / +4 / +8 | cumulative bin / relo / imp positions, initially zero |
| Descriptor+12 | pointer to current 48-byte raw entry |
| Descriptor+16 | dereferenced source wrapper object |
| Entry+32 / +36 / +40 | sizes added to the three cumulative positions |

Zero requested count returns true early. If count exceeds entry capacity,
the producer frees the old buffer, stores the new capacity and allocates
capacity*48. It invokes the reader with requested count and the entry buffer.
If the reader reports failure, descriptor construction is skipped.

Allocator `00417550` grows the descriptor buffer by count*20. The producer's
loop end, however, is **entry buffer + capacity*48**, not requested count*48.
If a reused buffer has capacity greater than count, the native loop can visit
entries beyond the freshly requested records. Descriptor capacity and stale
entry behavior are not established for that case. The audit projects only
fresh count-equals-capacity inputs and does not execute native allocation.
Multiplication overflow paths request all-bits-one allocation sizes; this is
not promoted to a successful allocation/bounds-safety guarantee. The projection
uses uint64 totals and rejects uint32 wrap as stricter audit policy.

## Exact pins and validation

| Raw offset | Bytes | SHA-256 |
| --- | ---: | --- |
| 7F8EA0 vtable | 28 | `42BA6936FD82F8B27014E50F59428A9F5A436796B40F76E3FDBC759120105DB8` |
| AAC22 constructor write | 6 | `9F0BCDF9084B1EA1E0664C4DF35F17F534A0DCA03F4C3DCE9EC0468905032687` |
| AA784 physical header path | 148 | `FCF6CABFBB420B20ABB6664567EE3833E31979B305CBAC7FB18D1100835FF084` |
| 49C30 entry method | 86 | `FAD82C53417A95CC7799D26D460D1AA0C75F62802F6DC6B510D620AB51089D7B` |
| 49750 producer | 187 | `73A26E28690D1BEF007C731CD71F3EE2F5C3C70026C2F0EB8728C081884B0678` |
| 17550 descriptor allocator | 59 | `DA8EADC5C5A1E7D529FC357D979062260D7D91C34FCF7BE15C60A2817C4D2684` |
| CE939 producer caller | 20 | `52E3D63D77422403FA5C77E10FA139ECC40FF5CEE35BEDF43FC993C6817146D9` |

`scripts/Get-Ra3Ep11DescriptorProducer.ps1` imports the pinned 1.1 trace and
rechecks target SHA at completion. It rereads/re-pins the four existing stock
manifests from `D:\TEMP\Red Alert 3 Uprising Source Data`: global 11,357,
static 13,872, WorldBuilder 17,339 and English audio 12,951 entries, totaling
55,519. These are previously captured stock files, **not newly extracted 1.1
BIG contents**. The projection hashes canonical entry offsets, cumulative
bin/relo/imp positions and unchanged opaque word44. Header bin totals agree.
No adjacent `.bin/.relo/.imp` bytes are read; in particular the large
WorldBuilder binary is not dumped or read as part of this milestone.

`scripts/Test-Ra3Ep11DescriptorProducer.ps1` passes seven evidence pins,
four stock projections, a three-entry fixture (zero chunks and word44=7),
five malformed-raw rejections, eight detached code faults and repeat JSON
comparison of every stable nested report field. Faults cover concrete reader
slot, constructor, header size, entry stride, capacity loop, entry pointer,
allocator size and direct producer call. Existing version guards remain strict.

```powershell
# Reborn: validate concrete 1.1 reader/producer evidence and fresh raw projections without invoking the game or native compiler.
./scripts/Test-Ra3Ep11DescriptorProducer.ps1 -ImagePath 'D:\Program Files (x86)\Steam\steamapps\common\Command and Conquer Red Alert 3 Uprising\Data\ra3ep1_1.1.game' -BaselineImagePath 'D:\Program Files (x86)\Steam\steamapps\common\Command and Conquer Red Alert 3 Uprising\Data\ra3ep1_1.0.game'
```

The 165-group compiler suite was not rerun for this standalone static audit.
Follow-up: [1.1 linked-sidecar setup](RA3EP11_LINKED_SIDECARS.md) reviews the
complete `00449810` body and resolves its scoped role as sidecar setup, not
direct entry transformation. This producer report's false helper/provenance
flags describe its original narrower scope and are not silently rewritten.

Next inspect the concrete linked chunk queue/dispatch/read paths,
then source factories and context lifetime. Complete pointer provenance,
authentic authoring ProcessingHash, marshaller compatibility and game loading
remain open. Engineering effort remains **52% complete / 48% remaining**;
this evidence is not a usable SDK release.
