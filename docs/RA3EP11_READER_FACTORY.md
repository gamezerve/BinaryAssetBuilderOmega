# EP1 1.1 referenced-reader factories — October 9, 2026

## Result and scope

The reviewed first loader dispatches reference paths by their signed-byte tag.
Tag 2 selects factory-owner vtable slot 2Ch; all other tags select slot 28h.
The pinned concrete vtable `00BF8EA0` supplies `004AAE80` and `004AAE60`
respectively. The ordinary slot wrapper delegates to `004AAC80`.

Both factory bodies return cached node+20h values on hits and create readers
through `004AAC10` on misses, with different constructor flags. Ordinary
construction publishes through a helper-returned cache slot; the reviewed
tag-two miss body returns the new reader without equivalent publication.
The loader separately distinguishes factory owner argument three from list
owner argument four. This is a scoped factory/cache/store rebase, not proof
that all live owners have the concrete vtable or all cached readers do.

Native game/compiler/codecs were not executed. No shipping source, game file,
official schema, compiler policy or original 1.0 fixed-offset guard changed.

## Independent location

Starting with previously pinned concrete vtable `00BF8EA0`, raw slots
`007F8EC8` and `007F8ECC` identify ordinary wrapper `004AAE60` and tag-two
factory `004AAE80`. Following the wrapper call at `004AAE67` identifies
`004AAC80`; complete bodies were disassembled through their returns.
Reference dispatch/storage in first loader `004CE8F0` identifies allocation
helper `00485F80`, also reviewed through its return. No fixed address delta
from 1.0 was accepted as proof.

Target is `D:\Program Files (x86)\Steam\steamapps\common\Command and Conquer Red Alert 3 Uprising\Data\ra3ep1_1.1.game`.
SHA-256: `B1DA83EFD229570BAB9DE3A14E48B2D1E6128E302BD52384858FA8A413AE330B`.
Method addresses are preferred image VAs, not observed live addresses.

## Routing and constructor flags

| Reference tag | Slot / concrete body | Cache object / end sentinel | Miss constructor arguments |
| --- | --- | --- | --- |
| 2 | 2Ch / `004AAE80` | `00CC4AF8` / `00CC4AFC` | prepared-path, 0, 0, 1, 0 |
| All others | 28h / `004AAE60` -> `004AAC80` | `00CC4ADC` / `00CC4AE0` | prepared-path, 1, 1, 0, 1 |

The ordinary factory's second parameter is supplied as 1 by the reviewed
wrapper, but that factory can take other values from other callers. The flags
remain positional constructor evidence, not recovered authoring/patch-mode
names. Raw tags 128..255 become signed values -128..-1 and still select the
ordinary route; malformed or unknown reference semantics are not thereby
accepted for authoring.

Both factories prepare a path using global prefix `00CC46C8` and normalization
helpers, then lookup through `00677520`. Full path-helper behavior and string
buffer safety are not established here. Ordinary compares the returned node
with sentinel `00CC4AE0`; tag two compares with `00CC4AFC`. A non-sentinel
result returns node+20h directly. Neither reviewed hit path validates that
returned reader's vtable equals `00BF8EA0`. Cache initialization/population and
all callback effects remain open.

Both miss paths allocate 6Ch bytes through `00416920`. Ordinary calls the
constructor at `004AADFA`, tag two at `004AAFC9`. Ordinary passes its new
reader's path-related field to `0047E090`, invokes `006A34B0` with ECX set to
cache `00CC4ADC`, then writes the reader to the returned slot at `004AAE31`.
This pins the **observed publication call/store**, not a reconstruction of
`006A34B0`'s complete container semantics. The tag-two miss body has no
equivalent publication sequence. It is not proof that no external caller ever
publishes a tag-two reader.

Ordinary allocation failure sets ESI to zero and subsequently reads ESI+10h.
This reviewed branch is not promoted to an allocation-failure safety guarantee.
The helper routines and caller policy may have additional assumptions, but
they are not established by this audit.

## Factory owner versus list owner

Loader reference records are read as signed tag followed by NUL-terminated
path. During virtual-factory owner loading, the path argument is still pushed:
`B0h - 90h - 10h - 4 = 0Ch`, argument three. After factory return, the
ordinary list-owner load has no temporary path argument:
`B0h - 90h - 10h = 10h`, argument four. The nine-argument wrapper contract
preserves these lower positions. The two owners must not be collapsed into
one argument simply because both loads use an apparent B0h displacement.

A non-null returned reader with tag !=2 calls `0045F060`. If its AL is zero,
the loader obtains a node from `00485F80`, links it around list owner+0Ch,
reloads that field and stores the returned reader at reloaded-node+8 at
`004CEA87`. The helper's membership semantics and the circular-list alias
that would prove this is the freshly inserted node are intentionally left
open in this factory milestone. These were reviewed for 1.0, but must be
independently rebased before claiming them for 1.1.

Tag-two handling takes a distinct helper/list path and may call returned
reader virtual+24h. Its complete ownership and cleanup policy is not recovered
here. Resource cleanup and actual object deletion must remain distinct.

## Exact pins and validation

| Raw offset | Bytes | SHA-256 |
| --- | ---: | --- |
| AAC80 ordinary factory | 472 | `3D20F517457A22F58957DD3337A48F137AF2EE745E1E070D4D0CB3FABEB5DF0A` |
| AAE60 ordinary wrapper | 18 | `337C86181301781B021A6A8F314F589B566900970C132979DFD67C68B386B1C5` |
| AAE80 tag-two factory | 354 | `045D82B013FDB303B5BD43E96F25C7EB0706562736D85024D1BCA8C639A75261` |
| CE9D3 reference dispatch/store | 183 | `22632B3E2E1E18202D78FD9B31113D405ABC81AE6567D2E050CF142FBFD058D2` |
| 7F8EA0 concrete vtable | 48 | `7EB7C63FDEE57177BA67FEDFD449A803CE32573039333224C4623CD27A1D612D` |
| 85F80 allocation helper | 81 | `69B47655BC52D2585469A55606D2EB7DF8C131CEC53595F88EBB491449089D3B` |

`Get-Ra3Ep11ReaderFactory.ps1` imports strict 1.1 lifecycle evidence and
rechecks target SHA at completion. `Test-Ra3Ep11ReaderFactory.ps1` compares
every stable nested report field against repeat JSON. Six routes (0,1,2,127,
128,255), two invalid tags (-1,256), and eleven detached instruction/vtable
faults pass. Faults cover hits, constructor calls, ordinary publication, both
dispatch slots, stored pointer and allocation size. Fixtures model routing,
not a live native cache or acceptance of malformed assets.

```powershell
# Reborn: validate scoped 1.1 factory dispatch/cache/store evidence without executing the game or inventing live reader provenance.
./scripts/Test-Ra3Ep11ReaderFactory.ps1 -ImagePath 'D:\Program Files (x86)\Steam\steamapps\common\Command and Conquer Red Alert 3 Uprising\Data\ra3ep1_1.1.game' -BaselineImagePath 'D:\Program Files (x86)\Steam\steamapps\common\Command and Conquer Red Alert 3 Uprising\Data\ra3ep1_1.0.game'
```

Next independently rebase membership helper, source-list iterator, healthy-list
insertion aliases and reader cleanup/delete paths. Added ninth-argument
caller/consumer semantics, universal source binding, authentic authoring
ProcessingHash, native emission ABI and game loading remain open. The
165-group compiler suite was not rerun for this standalone static audit.
Weighted effort stays **52% complete / 48% remaining**; no usable SDK is claimed.
