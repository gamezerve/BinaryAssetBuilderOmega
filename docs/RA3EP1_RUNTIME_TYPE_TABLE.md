# EP1 runtime name/hash table reconciliation — October 9, 2026

## Result

The pinned shipping EP1 engine contains **1,341 contiguous name-pointer/hash
associations**, plus a **separate Texture name pointer/hash slot**. All **254
root types observed independently in four stock manifests** match exact runtime
names and TypeHashes. The prior one-constant/adjacency hypothesis is now supported
by full observed-root reconciliation, not just the PathMusicEvent sample.

This is a substantial runtime identity evidence milestone, **not a complete
compiler plugin registration table**. The additional 1,088 names are not root
types observed in those four manifests; they include nested records such as
Color3f. We have not verified the table's callers, its complete game universe,
authoring ProcessingHashes, aggregate hash derivation, processor ABI, cache
policy, or in-game loading. No production gate or registration was changed.

## Exact provenance

Engine:
`D:\SteamLibrary\steamapps\common\Command and Conquer Red Alert 3 Uprising\Data\ra3ep1_1.0.game`

- Image SHA-256: `ABA6A8825A744A78487837D78AD5992962257CE68404F639D600F11DEEA2580B`.
- Image length: 9,484,336 bytes; PE32 preferred base `00400000`.
- Reviewed stock evidence: `docs/RA3EP1_TYPE_TABLE_EVIDENCE.json`.
- Evidence raw SHA-256: `1E6220A5BECA5039C20EE4AEE5DB9DDE0D28F56D8FE70D90E217928B86C75C58`.
- All four evidence manifests were reread and SHA-256 checked against this
  independently captured snapshot. No companion asset BIN/RELO/IMP was opened.

These are strict raw artifact pins, including evidence JSON line endings.
Another image build or reformatted evidence is rejected and needs a separate
review; the script has no public pin-bypass parameter.

## Decoded relationship and Texture exception

All coordinates are **raw file offsets**, not process addresses.

| Component | Start | End exclusive | Rows / bytes |
| --- | --- | --- | --- |
| Parallel name-pointer array | `008BC0A8` | `008BD59C` | 1,341 / 5,364 |
| Parallel hash array | `007E00D8` | `007E15CC` | 1,341 / 5,364 |
| Separate Texture name pointer | `008BC00C` | `008BC010` | 1 / 4 |
| Separate Texture hash | `007E00D4` | `007E00D8` | 1 / 4 |

For each contiguous row, the name-pointer slot is exactly `000DBFD0` bytes
after its hash slot. Each preferred VA maps into the pinned `.rdata` range;
each name is a unique, bounded ASCII identifier, and each hash is nonzero.
First parallel name: AudioFileRuntime. Last: DamageFXSettings.

Six weather enum pointers at `008BC090..008BC0A4` must **not** be included in
the type array. In particular, blindly extending the parallel relation one
row backward pairs the name SNOWY with the Texture hash. Texture instead uses
its own pointer at `008BC00C`, resolving to the string at `007DFE88`.
This explains the initial 253/254 result without replacing Texture identity
or pretending every nearby printable pointer is an asset type.

The complete 5,368-byte hash block from `007E00D4` has SHA-256
`856CD029215A3A1C756F1F99EE2FF46E71867E29558C561760371850CB97ECD4`.
Decoded row order is Texture first, then the 1,341 parallel rows. Its canonical
UTF-8 text concatenates `Name<TAB>0xUPPERCASEHASH<LF>` for each row; SHA-256:
`35AFC7028B8BA559C2AA654EEAD6C7C3E85872DF58C8F792152C11E7FB9E6AF1`.
These are forensic fingerprints, **not** EA's AllTypesHash algorithm.

| Runtime name | TypeHash | Name-pointer slot | Hash slot |
| --- | --- | --- | --- |
| Texture | `9F5FF8DA` | `008BC00C` | `007E00D4` |
| PathMusicMapRuntime | `1151699C` | `008BC0AC` | `007E00DC` |
| AudioFile | `53C81E47` | `008BC138` | `007E0168` |
| AttributeModifier | `74425C11` | `008BC21C` | `007E024C` |
| WeaponTemplate | `27E8D984` | `008BC374` | `007E03A4` |
| LocomotorTemplate | `2A95936A` | `008BC38C` | `007E03BC` |
| PathMusicMap | `BA1B4CCB` | `008BC4CC` | `007E04FC` |
| PathMusicTrack | `9F12CF33` | `008BC4D0` | `007E0500` |
| PathMusicEvent | `599CDAF2` | `008BC4D4` | `007E0504` |
| PathMusicEventRuntime | `3AAC7132` | `008BC4D8` | `007E0508` |
| ArmorTemplate | `A0E237D8` | `008BC4FC` | `007E052C` |
| GameObject | `CEB9FE36` | `008BD520` | `007E1550` |

The authored and runtime music names have distinct hashes. A matching runtime
hash cannot substitute for the original authoring processor's ProcessingHash
or required Pathfinder header. Full row output is reproducible from the script;
no multi-megabyte executable dump or stock payload copy was added to Git.

## Implementation and verification

`scripts/Get-Ra3Ep1RuntimeTypeTable.ps1` uses `Read-RuntimeInput`,
`Read-RuntimeName`, `Read-RuntimeRows` and `Compare-RuntimeEvidence`.
It snapshots regular explicit absolute files, rejects reparse ancestors, caps
images/manifests at 16 MiB and evidence at 2 MiB, pins the exact reviewed image
and stock evidence before fixed offsets are interpreted, and rechecks the two
main inputs after observation. Names are capped at 256 bytes and constrained
to the pinned `.rdata` mapping. The public report always leaves production,
ProcessingHash, complete-universe and table-caller verification false.
The manifest SHA checks establish snapshot provenance, not atomic concurrency
or game payload integrity; there is no full-stream hash or cache activation.

```powershell
# Reborn: inspect the exact reviewed runtime image and pinned independent stock evidence without executing target code.
./scripts/Get-Ra3Ep1RuntimeTypeTable.ps1 -ImagePath 'D:\SteamLibrary\steamapps\common\Command and Conquer Red Alert 3 Uprising\Data\ra3ep1_1.0.game' -AsJson
# Reborn: verify all stock roots, repeat bytes and private-memory faults without modifying any game or evidence files.
./scripts/Test-Ra3Ep1RuntimeTypeTable.ps1 -ImagePath 'D:\SteamLibrary\steamapps\common\Command and Conquer Red Alert 3 Uprising\Data\ra3ep1_1.0.game'
```

Tests passed: all 254 roots; exact ordered rows/digests through repeat JSON;
six private image-memory faults (bad pointer, duplicate name, zero hash, wrong
Texture pointer, wrong end boundary, changed stock hash); six evidence-memory
faults (wrong target/count, duplicate/unknown name, wrong hash, empty fingerprint);
and three public rejections (unreviewed image, relative image path, unreviewed
evidence). Memory faults are reachable only after authentic baseline validation
via the self-test branch; no mutated image is admitted through public input.
The existing PE and archive regression scripts also passed. C# compiler code
was unchanged; no full compiler-suite rerun is claimed and groups remain 165.

## Next migration gate

Map the 1,342 decoded runtime names to official EP1 schema declarations,
distinguishing nested types, runtime-only names and actual authored asset roots.
Then review table callers and the authentic type-hash derivation separately;
do not automatically register all names or treat them as the 1,390 schema model
inventory. Production activation still requires native processors, authoring
identity/dependency contracts, the aggregate catalog guard and game validation.

Overall effort estimate remains **52% complete / 48% remaining**. This milestone
removes the uncertainty about observed runtime name/hash alignment; it does not
close the separate compiler or in-game acceptance gates.
