# EP1 1.1 initializer/metadata sample binding — October 9, 2026

## Result and proof scope

Two narrowly reviewed straight-line x86 templates decode **150 initializers**
in the pinned 1.1 image: **43 short** and **107 vtable-writing** bodies. Each
loads a hash from an independently decoded runtime-table slot, writes it to
metadata object+8 and passes that same object to registry body `00417430`.
The separately reviewed registry/consumer retrieves metadata and compares its
hash+8 with entry+8. This connects the table → initializer → registry → hash
consumer dataflow **for the decoded sample**, subject to the body being invoked.

All 150 static metadata object+4 TypeIds match the existing repo-owned managed
FastHash name provider. **138** are roots in the pinned stock manifest evidence;
their TypeIds match that evidence too. Raw metadata+8 is zero before the code
copies the runtime hash. This distinguishes TypeId from initialized TypeHash.

There are still **1,192 runtime names outside these two templates**. These
are not automatically 1,192 missing root processors; many names describe nested
records. Full startup reachability, ordering and complete metadata/type coverage
are not established. **PathMusicEvent is not covered by this template sample.**
No authoring ProcessingHash, marshaller ABI or successful mod load is proven.

## Reviewed instruction templates

Both shapes begin with `mov eax,[hash-source]`, `push object`,
`mov [object+8],eax`, then `call 00417430`. Next they push a function address.
The short shape then calls `004D9E45`, restores eight stack bytes and returns
(34 bytes). The longer shape additionally writes a vtable candidate to the
same object, then makes the same second call and returns (44 bytes).

The decoder checks every opcode boundary, both exact relative-call targets,
16-byte alignment, mapped hash sources, object/store relationship and section
bounds. This is **not a general-purpose x86 disassembler**. Regex scanning
only locates candidates; the narrow decoder independently validates their shape.
The second call and pushed-function semantics remain unnamed; they are not
assumed to establish startup reachability or ownership safety.

### Concrete Texture and ArmorTemplate examples

| Property | Texture | ArmorTemplate |
| --- | --- | --- |
| Initializer preferred VA | 00BC40D0 | 00BC83E0 |
| Raw offset / bytes | 007C40D0 / 44 | 007C83E0 / 34 |
| Hash-source VA | 00BE61D4 | 00BE662C |
| Copied TypeHash | 9F5FF8DA | A0E237D8 |
| Metadata object VA | 00CC85C0 | 00CCED48 |
| Hash destination VA | 00CC85C8 | 00CCED50 |
| Registration target | 00417430 | 00417430 |
| Second call target | 004D9E45 | 004D9E45 |
| Pushed function VA | 00BD06A0 | 00BD2C70 |
| Vtable write | object+0 = 00BFC57C | absent in this shape |

These bodies were statically disassembled at complete instruction boundaries.
All 150 rows retain exact raw offsets, source/destination/call addresses,
per-body SHA-256 and the associated name/hash in reproducible report output.
No executable or multi-megabyte dump was added to Git.

## Static object fields and opaque metadata

Objects are constrained to reviewed raw .data offsets 008C3000..008F0000,
not virtual zero-fill bytes. Metadata+4 is read as TypeId, +8 must be zero in
the file, and +12 is retained verbatim as an opaque uint32. Initialization
writes +8, not +12. A detached C0DEFFFF fixture proves that the diagnostic
preserves +12 without converting it to Boolean or Tokenized.

Metadata+12 disagrees with stock Tokenized in **122/138** observed roots.
The remaining 16 coincidental agreements do not establish field semantics.
The distinct +12 consumers in the previous audit remain unresolved.

No native code executes. Only the already-built repo-owned managed inspector's
`hash` diagnostic computes the name TypeIds. The inspector is bounded/read as
a regular explicit file before invocation; the game, registration routines,
initializers, pushed functions and native codecs are never invoked.

## Reproduce and validation

1.1 image SHA:
`B1DA83EFD229570BAB9DE3A14E48B2D1E6128E302BD52384858FA8A413AE330B`.
The inherited runtime comparison independently pins the 1.0 baseline and stock
evidence; the inherited 1.1 consumer pins registration/lookup/hash bodies.
The initializer audit rechecks 1.1 image SHA after all sample observations.
Snapshot checks are not an atomic multi-file view or asset-payload integrity proof.

```powershell
# Reborn: reproduce scoped 1.1 initializer-to-metadata binding using only the existing managed name-hash diagnostic, never native initializer execution.
./scripts/Get-Ra3Ep11TypeInitializers.ps1 -ImagePath 'D:\Program Files (x86)\Steam\steamapps\common\Command and Conquer Red Alert 3 Uprising\Data\ra3ep1_1.1.game' -BaselineImagePath 'D:\Program Files (x86)\Steam\steamapps\common\Command and Conquer Red Alert 3 Uprising\Data\ra3ep1_1.0.game' -AsJson
# Reborn: check all sample identities, ten detached opcode/address/object faults, opaque-word preservation and repeated row/object JSON.
./scripts/Test-Ra3Ep11TypeInitializers.ps1 -ImagePath 'D:\Program Files (x86)\Steam\steamapps\common\Command and Conquer Red Alert 3 Uprising\Data\ra3ep1_1.1.game' -BaselineImagePath 'D:\Program Files (x86)\Steam\steamapps\common\Command and Conquer Red Alert 3 Uprising\Data\ra3ep1_1.0.game'
```

Tests pass: 150/43/107 coverage, 150 name-TypeIds, 138 stock roots, seven long
template faults, one short-template fault, two object identity faults, one
opaque preservation case and exact repeated JSON. Consumer regressions also
pass. No compiler changes or full-suite rerun: previously executed 165 groups.

## Next gate

Rebase 1.1 factory/header/descriptor/linked-sidecar flow to connect actual stream
entries to this hash gate. Keep remaining initializer templates and startup
reachability separate; do not fabricate a PathMusicEvent initializer from an
adjacent name or assume all runtime rows require identical registration.
Recover modconfig file/package handling before a controlled loading PoC.

Overall weighted effort remains **52% complete / 48% remaining**. The sample
binding question is resolved, not the entire initializer universe or usable SDK.
