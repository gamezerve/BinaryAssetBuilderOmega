# EP1 1.1 runtime type/hash reconciliation — October 9, 2026

## Result

The independently located 1.1 arrays contain **1,341 contiguous name/hash
pairs plus a separate Texture pair**. All **1,342 ordered name/hash pairs**
match the pinned 1.0 runtime table exactly. All **254 root types in the four
previously captured stock manifests** match 1.1 runtime names and TypeHashes.
The complete raw hash block is byte-identical across these two reviewed images.

This closes the runtime-table rebase gate for this exact 1.1 image. It does not
prove authoring ProcessingHashes, runtime table caller semantics, complete
game type universe, processor ABI, stream-loader compatibility or mod loading.
The 1,088 other runtime rows are not roots observed in those four manifests.
No production registrations or compiler identity guards were changed.

The stock evidence is the existing unpacked-file snapshot, not a fresh extraction
of all BIG archives from the newly supplied 1.1 installation. The comparison
must not be described as a complete 1.1 archive or payload integrity audit.

## Independent location and exact provenance

Image:
`D:\Program Files (x86)\Steam\steamapps\common\Command and Conquer Red Alert 3 Uprising\Data\ra3ep1_1.1.game`

- Length: 13,381,632 bytes.
- SHA-256: `B1DA83EFD229570BAB9DE3A14E48B2D1E6128E302BD52384858FA8A413AE330B`.
- PE32 preferred base 00400000; reviewed .rdata raw/RVA range 007D8000..008C3000.
- Reference 1.0 image SHA: `ABA6A8825A744A78487837D78AD5992962257CE68404F639D600F11DEEA2580B`.
- Stock evidence SHA: `1E6220A5BECA5039C20EE4AEE5DB9DDE0D28F56D8FE70D90E217928B86C75C58`.

AudioFileRuntime, DamageFXSettings, Texture and PathMusicEvent were located
as bounded literal strings in 1.1 .rdata. Their preferred-VA references were
then found in .data. This yielded the name-array start/end and separate Texture
slot. The observed PathMusicEvent hash literal led to the hash-array candidate;
full ordered decoding and all stock root comparisons corroborate the association.
No global constant address-delta assumption was used to discover these arrays.

| Raw-file component | 1.0 | Independently reviewed 1.1 |
| --- | --- | --- |
| Separate Texture pointer | 008BC00C | 008C300C |
| Separate Texture hash | 007E00D4 | 007E61D4 |
| Parallel pointer start | 008BC0A8 | 008C30A8 |
| Parallel pointer end exclusive | 008BD59C | 008C459C |
| Parallel hash start | 007E00D8 | 007E61D8 |
| Parallel hash end exclusive | 007E15CC | 007E76CC |
| Pointer/hash slot difference | 000DBFD0 | 000DCED0 |

Pointer slots moved by 7000h while hash slots moved by 6100h. Their string
targets can move differently again; instruction/vtable rebasing is not licensed
by these differences. The decoder uses explicit reviewed starts and row indices.

Six weather enum pointers at 008C3090..008C30A4 are excluded. Including the
last enum SNOWY as the first type would misassociate its name with a type hash.
Texture independently resolves through 008C300C to string 007E5F88. The first
parallel row is AudioFileRuntime (string 007E5F74); the last is DamageFXSettings
(string 007DCFA4). The next pointer at 008C459C is outside the reviewed array.

| Example | 1.1 pointer slot | 1.1 hash slot | TypeHash |
| --- | --- | --- | --- |
| Texture | 008C300C | 007E61D4 | 9F5FF8DA |
| PathMusicEvent | 008C34D4 | 007E6604 | 599CDAF2 |
| DamageFXSettings | 008C4598 | 007E76C8 | 7A57612C |

The 5,368-byte hash block SHA-256 is
`856CD029215A3A1C756F1F99EE2FF46E71867E29558C561760371850CB97ECD4`.
Texture-first ordered UTF-8 `Name<TAB>0xUPPERCASEHASH<LF>` canonical SHA-256 is
`35AFC7028B8BA559C2AA654EEAD6C7C3E85872DF58C8F792152C11E7FB9E6AF1`.
Both fingerprints equal 1.0. Neither fingerprint is EA's AllTypesHash algorithm.

## Implementation and tests

`Get-Ra3Ep11RuntimeTypeTable.ps1` imports the unchanged 1.0 audit for bounded
regular-file/evidence checks and baseline decoding. Its parameters are restored
after dot-sourcing. It then pins 1.1 identity before applying separately reviewed
section bounds and array coordinates. `Read-Ep11RuntimeName` enforces bounded
ASCII identifiers; `Read-Ep11RuntimeRows` rejects duplicates/zero hashes and
checks boundaries; `Compare-Ep11BaselineRows` compares every ordered row, including
the 1,088 rows that stock-root-only checks would not establish.

Four manifest files are SHA-checked by the inherited baseline audit without
opening their BIN/RELO/IMP sidecars. Both engine versions and evidence are
rechecked after comparison. These snapshots are not an atomic multi-file view.
Native game/codec code and the managed hash inspector are not executed by this
runtime-table audit. No target/reference file is modified or added to Git.

```powershell
# Reborn: independently decode pinned 1.1 arrays and compare with separately pinned 1.0/stock evidence, never a guessed global address shift.
./scripts/Get-Ra3Ep11RuntimeTypeTable.ps1 -ImagePath 'D:\Program Files (x86)\Steam\steamapps\common\Command and Conquer Red Alert 3 Uprising\Data\ra3ep1_1.1.game' -BaselineImagePath 'D:\Program Files (x86)\Steam\steamapps\common\Command and Conquer Red Alert 3 Uprising\Data\ra3ep1_1.0.game' -AsJson
# Reborn: test repeat JSON, ten detached pointer/hash/row faults and four public version/path/evidence boundaries.
./scripts/Test-Ra3Ep11RuntimeTypeTable.ps1 -ImagePath 'D:\Program Files (x86)\Steam\steamapps\common\Command and Conquer Red Alert 3 Uprising\Data\ra3ep1_1.1.game' -BaselineImagePath 'D:\Program Files (x86)\Steam\steamapps\common\Command and Conquer Red Alert 3 Uprising\Data\ra3ep1_1.0.game'
```

Tests cover seven cloned image faults (pointer, duplicate, zero, Texture,
weather, tail, stock hash), three ordered-row faults (name, hash, order), four
public refusals (swapped versions, wrong current image, relative path, wrong
evidence) and exact repeated JSON. Original 1.0 runtime-table regressions also
pass. Full compiler suite not rerun: previously executed 165 groups.

## Next gates

Follow-up: [1.1 registry/hash consumer](RA3EP11_HASH_CONSUMER.md) independently
pins registry insertion/lookup and the metadata+8/entry+8 comparison. This closes
the scoped consumer-body rebase, not startup initializer-to-metadata binding
or complete loader pointer provenance.

Rebase native initializer/type registry and TypeHash consumer code, then
factory/header/descriptor/sidecar/cleanup evidence to 1.1. Separately recover
modconfig file commands and package registration. Runtime identity equality
supports reuse of the observed type catalog as a reference, not automatic reuse
of marshaller layouts, authoring hashes or native code addresses.

Overall weighted effort remains **52% complete / 48% remaining**. This closes
one newly identified version gate, not a usable SDK or successfully loaded mod.
