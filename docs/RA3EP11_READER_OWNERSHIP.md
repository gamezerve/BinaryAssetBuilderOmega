# EP1 1.1 pointer membership and scoped source forwarding — October 9, 2026

## Result and limits

Helper `0045F060` compares the supplied reader pointer with node+8 across
three global circular lists. It does **not** validate XML/schema, TypeHash,
asset payload or contents. Calling it an asset-admission validator would
misrepresent the inspected instructions. Its true result means a matching
pointer was found; the ordinary factory caller links a non-null reader only
when this result is false.

Iterator `004D04A0` supplies node+8 as the wrapper source handle, directly on
one branch and through a local copy on the other. Both reviewed iterator
calls supply **zero as wrapper argument six**, which the independently
reviewed wrapper forwards as the **ninth loader argument**. This supplies
scoped caller-side provenance but still does not explain its ultimate consumer
or meaning. It is not proof that every caller supplies zero or that the
argument is globally unused.

No game/compiler/codec ran, no source/reference data changed and no production
guard was relaxed. Healthy-list insertion aliases, cleanup versus deletion,
cache lifetime and complete source binding remain open for the next audit.

## Image and independent location

Target: `D:\Program Files (x86)\Steam\steamapps\common\Command and Conquer Red Alert 3 Uprising\Data\ra3ep1_1.1.game`.
SHA-256: `B1DA83EFD229570BAB9DE3A14E48B2D1E6128E302BD52384858FA8A413AE330B`.
Addresses are preferred image VAs, not live ASLR addresses.

The ordinary reference-storage call at `004CEA56` identifies helper
`0045F060`; its complete 55-byte body was disassembled. An overlapping E8
scan for wrapper `004CFEB0` yields four raw candidates:
`000D0566`, `000D05BA`, `000D06DB`, `000D07F8`. Disassembling back from the
first two identifies iterator `004D04A0`, reviewed through its return at
`004D05DF`. The other two have only limited caller snippets inspected and
are **not complete reviewed caller paths**. The raw scan is not an exhaustive
direct/indirect call graph. No presumed version delta locates this evidence.

## Membership semantics

| Field / address | Reviewed use |
| --- | --- |
| `00CF154C` | first list sentinel/head |
| `00CF1584` | second list sentinel/head |
| `00CF15BC` | third list sentinel/head |
| Sentinel/node+0 | next link |
| Node+8 | reader pointer compared with supplied argument |
| 38h / 56 | stride between list sentinels |
| A8h / 168 | loop bound, exactly three lists |

The helper begins at the first sentinel, skips an empty list, follows next
links and compares node+8. It returns true at the first match and false after
all three lists. Native invalid-link handling, traversal bounds, global list
initialization and concurrency are not established. The pure test model
accepts three already-enumerated groups and never dereferences native addresses.
Its 100,000-value bound is stricter diagnostic policy, not recovered engine
behavior. A synthetic zero-pointer equality fixture is not a valid-native-reader
claim; the caller rejects null before its membership call.

## Owner and source pointer flow

Wrapper `004CFEB0` loads argument two at adjusted ESP+6Ch into EDI; `[EDI]`
is used for header virtual+0Ch and later cleanup virtual+24h. Its argument
three at adjusted ESP+70h becomes first-loader argument four, the list owner.
EDI is pushed as first-loader argument three, the factory-owner source handle.
Thus the header source and factory source are the same handle in this reviewed
wrapper, but the list owner is a different argument. This still does not prove
the source's live vtable equals the pinned concrete reader table.

Iterator `004D04A0` reads its first argument as owner, obtains sentinel
owner+0Ch, follows node+0 and stops at that sentinel. Its paths are:

| Iterator call | Source handle / wrapper arguments |
| --- | --- |
| `004D0566` | node+8 value copied into a local handle; address of copy becomes wrapper arg2; owner becomes arg3; arg4=1; iterator arg3 becomes wrapper arg5; arg6=0 |
| `004D05BA` | address of node+8 becomes wrapper arg2; owner becomes arg3; arg4=0; iterator arg3 becomes wrapper arg5; arg6=0 |

The first push at `004D0507` is EBX, initialized to zero at iterator entry;
it supplies the sixth wrapper argument on either branch. Five further pushes
complete the six-argument call, and the caller restores 18h/24 bytes. The
wrapper subsequently passes that arg6 as loader arg9, while the existing
loader arg8 is handled independently by wrapper branch selection.

Iterator owner mapping uses `78h - 6Ch - 2*4 = 4`, argument one. Its arg3
forwarding at ESP+8Ch occurs with four saved registers and one temporary push:
`8Ch - 6Ch - 4*4 - 4 = 0Ch`, argument three. Temporarily pushed arguments
must be included; raw displacement equality alone is insufficient.

The iterator writes node+10h back from a local field on its alternate path
and clears temporary resources through other helpers. Those helpers are not
fully audited here. A pointer loaded from this list is not yet proven to be
the same fresh node stored by a specific previous insertion; that requires
the circular-list alias/lifetime follow-up. No universal container-ownership
or reader deletion conclusion is drawn.

## Exact pins and validation

| Raw offset | Bytes | SHA-256 |
| --- | ---: | --- |
| 5F060 complete membership helper | 55 | `51D67A1A649BD0FBB06F994E1E119ED47AD95D043FF518E42EA1A4B3CCE4D6A7` |
| CFEB0 complete wrapper | 377 | `34F61F895AF698B410E4F8B154ADF9B471CE6D5826A08EC7A5A9F4989C733327` |
| D04A0 complete iterator | 320 | `64E91964806AC4B4BD8AE444D49419ED46052E34D28EFD163D320CDD092525B0` |

`Get-Ra3Ep11ReaderOwnership.ps1` imports the strict 1.1 factory evidence,
pins these bodies and the four-call candidate set, checks owner stack mappings
and rereads target SHA at completion. Six pointer-membership fixtures cover
empty/absent lists, a hit in each of the three lists and raw zero equality.
Two wrong-group-count fixtures reject. Twelve detached faults cover pointer
comparison, stride/bound, source/list loads, source forwarding, iterator payload,
the zero sixth argument, both wrapper calls and next-link traversal.
`Test-Ra3Ep11ReaderOwnership.ps1` compares all stable nested fields with repeat
JSON. The original 1.0 guards remain strict and separate.

```powershell
# Reborn: validate 1.1 reader-pointer membership and scoped iterator/source forwarding without native execution or invented asset-admission semantics.
./scripts/Test-Ra3Ep11ReaderOwnership.ps1 -ImagePath 'D:\Program Files (x86)\Steam\steamapps\common\Command and Conquer Red Alert 3 Uprising\Data\ra3ep1_1.1.game' -BaselineImagePath 'D:\Program Files (x86)\Steam\steamapps\common\Command and Conquer Red Alert 3 Uprising\Data\ra3ep1_1.0.game'
```

Next rebase healthy-list insertion, cleanup and conditional deletion, then
remaining caller/cache/global initialization paths. Authentic authoring
ProcessingHash, native emission ABI, unresolved AUDIO inputs and actual
modconfig package loading remain open. The 165-group compiler suite was not
rerun for this static audit. Weighted engineering effort remains **52%
complete / 48% remaining**; no usable SDK or loaded mod is claimed.
