# EP1 1.1 registry bodies and TypeHash consumer — October 9, 2026

## Result and limits

The 1.1 image has a TypeId-keyed registry lookup at `00417370`, insertion body
at `00417430` and inline TypeHash comparison at `004AB8D5` in `004AB880`.
All three use registry head `00CF1528`. The comparison reads entry-like record+8
and registered metadata object+8 and can enter an explicit mismatch diagnostic.

These bodies were independently located from the mismatch message and registry
head references, then disassembled at instruction boundaries. Their field
offsets/control decisions agree with the previously reviewed 1.0 evidence;
their code/global addresses and exact slice fingerprints differ.
No constant global address delta was assumed to locate them.

This closes a scoped **registry-body/hash-consumer rebase**. It does not yet
connect 1.1 startup initializer writes to all metadata hashes. The independently
reconciled runtime name/hash arrays do not by themselves prove that final link.
No full registry universe, complete stream-pointer provenance, authoring
ProcessingHash, diagnostic fatality or successful game loading is established.

## Registry field layout

| Field | Reviewed meaning |
| --- | --- |
| Registry global | `00CF1528`, first node or null |
| Node+0 | next node |
| Node+4 | TypeId used by lookup |
| Node+8 | repeated TypeId, **not TypeHash** |
| Node+12 | metadata object pointer |
| Metadata+4 | TypeId copied during registration |
| Metadata+8 | hash read by the inline comparison |
| Metadata+12 | opaque word used by separate loader contexts |

Lookup `00417370` follows node+0 until TypeId matches, then returns node+12;
absence returns zero. Its reviewed body does not read a TypeHash. Registration
reads metadata+4, checks for an existing non-null object, invokes conditional
diagnostics, allocates `10h` bytes through `00416920` and prepends a node.
Both node+4 and node+8 are populated from the same TypeId register. Allocation
failure, duplicate-registration diagnostics and callback behavior are not
converted into a safety or rejection guarantee.

This report pins the registry bodies, not the 150-object initializer sample
previously established for 1.0. No automatic 1.1 object-address remapping occurs.

## Hash gate and diagnostic

Consumer `004AB880` receives the entry-like pointer at its fourth stack argument
(24h locals and three saved registers before the 40h load). It reads TypeId
at entry+0 and InstanceId at entry+4. It inlines the same registry search.

| Condition in observed order | Outcome |
| --- | --- |
| Missing matching node or null metadata | skip to `004AB9C4` |
| Entry word+44 nonzero | skip comparison |
| Entry hash+8 zero | skip comparison |
| Entry hash+8 equals metadata hash+8 | continue without mismatch diagnostic |
| Otherwise | enter diagnostic path at `004AB8DE` |

The equality instruction is `004AB8D5`. On mismatch, call `0040EDE0` and a
virtual+68h call through global `00CF069C` can each skip diagnostics depending
on their Boolean-like results. Their control semantics remain unresolved.
The final virtual diagnostic call has not been identified as necessarily fatal,
recoverable or exception-throwing. **Unconditional mismatch rejection is not
proven. Engine skip branches are not permission to emit zero hashes or bypass
production identity validation.**

The complete mismatch message starts at `00BF8F78`, including the build-data
reminder and `Type hash mismatch for type`. Expected hash is read from metadata+8
at `004AB980`; actual hash is read from entry+8 at `004AB9A5`. Embedded source
location `00BF8FC0` is:
`C:\CNCRA3\Production\code\Libraries\Source\assetmanager\resourcemanager.cpp:224`.
This is embedded provenance, not an available source file.

Entry field interpretation is consistent with the inspected 48-byte manifest
record, but the complete 1.1 stream-reader/descriptor/entry-pointer trace is
not yet rebased. Word+44 remains unnamed in this consumer report. Metadata+12
must not be assumed to mean the same thing as entry+44.

## Separate direct lookup callers

Reviewed contexts at `004CED14`, `004CF454` and `004CFB8E` call the small lookup
at raw `000CED2B`, `000CF46B`, `000CFB98`. They consume **metadata+12**, not
metadata+8. Each considers the stack word at B8h, entry word+44, metadata presence
and the opaque word to form a local Boolean. These are not the hash gate.

A separate overlapping raw E8 scan finds hash-gate call candidates at
`000CEFA0`, `000CF6E0`, `000CFE26`. The sets are bounded byte candidates,
not an exhaustive call graph or complete argument-provenance proof. Indirect
calls and code/data ambiguity outside reviewed instruction regions remain open.

## Exact pins and reproduction

Image SHA: `B1DA83EFD229570BAB9DE3A14E48B2D1E6128E302BD52384858FA8A413AE330B`.

| Raw offset / slice | Bytes | SHA-256 |
| --- | ---: | --- |
| AB880 gate | 96 | `2421A8766269EDE315C48AF93EDD47AB5CAD03817E5CA119C69A9EA434835E2B` |
| AB8DE diagnostic | 230 | `8AC663C230EEE087AEC40D5C70AA687A75279525610828CE238C27C55C39012C` |
| 17370 lookup | 34 | `E0B701CA34CD8B61B559EC150ECBD7267449EA61E98DA48680CFB15D7F7DCD5B` |
| 17430 registration | 211 | `C7E40AC48AC8EDD6D342BBBCA658A10B8D0F90BBA6FEC0B1E6C790F52D9D3106` |
| CED14 opaque-word context | 69 | `AB9B5F41F3F4B707D9F4E7589C15AEA5D5EB364AB3A7C180DCD7FCF4503485ED` |
| CF454 opaque-word context | 69 | `1B6CCD8075F7B3F784CF1F7561A35F168D58C1BFF68AC3FA98B64FD25208B827` |
| CFB8E opaque-word context | 50 | `E4B75876A83D4AA5127ACB22A3D8A577B0D03612D1F22C9F653B69A24BE966C4` |

```powershell
# Reborn: reproduce independently pinned 1.1 registry/consumer bodies without executing native code or changing compiler acceptance policy.
./scripts/Get-Ra3Ep11HashConsumer.ps1 -ImagePath 'D:\Program Files (x86)\Steam\steamapps\common\Command and Conquer Red Alert 3 Uprising\Data\ra3ep1_1.1.game' -BaselineImagePath 'D:\Program Files (x86)\Steam\steamapps\common\Command and Conquer Red Alert 3 Uprising\Data\ra3ep1_1.0.game' -AsJson
# Reborn: check exact repeat JSON, seven branch fixtures and nine detached registry/hash/diagnostic/opaque-word faults.
./scripts/Test-Ra3Ep11HashConsumer.ps1 -ImagePath 'D:\Program Files (x86)\Steam\steamapps\common\Command and Conquer Red Alert 3 Uprising\Data\ra3ep1_1.1.game' -BaselineImagePath 'D:\Program Files (x86)\Steam\steamapps\common\Command and Conquer Red Alert 3 Uprising\Data\ra3ep1_1.0.game'
```

The new audit imports the separately pinned version-table reconciliation for
bounded read-only snapshots, independent stock evidence and version guards.
It pins seven code slices, checks two candidate sets and rechecks 1.1 SHA at
completion. Seven pure fixtures include raw entry word 7 and zero metadata hash;
they model the first observed decision, not loading success. Nine code mutations
affect only cloned arrays. Repeat JSON and original 1.0 consumer regressions
pass. The new audit executes neither target code nor the managed name-hash
inspector; the separate old 1.0 regression still uses that repo-owned managed
diagnostic as previously documented. Native engine/compiler/codecs never run.
No full compiler-suite rerun: previously executed 165 groups.

## Next gate and migration impact

Recover 1.1 initializer writes/object TypeIds to connect runtime arrays to these
metadata consumers, then rebase factory/header/descriptor and linked-sidecar
pointer provenance. Separately recover modconfig file commands/package setup.
The identical runtime catalog supports reference reuse, not guessed authoring
hashes, marshaller ABI or native binary offsets. Overall weighted estimate stays
**52% complete / 48% remaining**; no usable SDK/game-load milestone is claimed.
