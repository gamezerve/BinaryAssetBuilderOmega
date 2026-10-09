# EP1 1.1 discovery and modconfig baseline — October 9, 2026

Later runtime update: the [normal baseline](RA3EP11_NORMAL_BASELINE.md) observed
default selection of 1.1 and its `-config` SKU argument, with user-confirmed main
menu arrival. The no-launch statements in the static discovery below describe
the earlier inspection, not current project state. The subsequent
[inert trial](RA3EP11_INERT_CONFIG_TRIAL.md) observed runtime `-modconfig`
forwarding but crashed; config-read proof remains unavailable.

## Version correction

The user supplied a second installation at
`D:\Program Files (x86)\Steam\steamapps\common\Command and Conquer Red Alert 3 Uprising`.
Its Data folder contains **both** ra3ep1_1.0.game and ra3ep1_1.1.game.
The previously inspected `D:\SteamLibrary\...\Data` contains only 1.0.
The 1.0 files in both locations have identical SHA-256. The earlier fixed-offset
audits remain valid for that 1.0 identity; they do not prove 1.1 compatibility.
The newer installation should be the next runtime-rebase target.

| Property | Existing 1.0 evidence image | Newly located 1.1 image |
| --- | --- | --- |
| Bytes | 9,484,336 | 13,381,632 |
| SHA-256 | `ABA6A8825A744A78487837D78AD5992962257CE68404F639D600F11DEEA2580B` | `B1DA83EFD229570BAB9DE3A14E48B2D1E6128E302BD52384858FA8A413AE330B` |
| PE format/base | I386 PE32 / 00400000 | I386 PE32 / 00400000 |
| Entry RVA | 000D9DA3 | 000DA163 |
| .rdata raw start | 007D2000 | 007D8000 |
| .data raw start | 008BC000 | 008C3000 |
| PDB path embedded in image | `E:\Projects\Ra3_ep1\Production\Run\RTS-final.pdb` | `C:\CNCRA3\Production\Run\RTS-final.pdb` |

The 1.1 CodeView GUID is D000CB3E-158A-4FDB-ADC3-4358433EE075, age 1.
Its PE timestamp is 2025-02-20 23:41:45 UTC (02:41:45 on February 21 in
Europe/Istanbul); this is header metadata, not an
independently established release date. No actual PDB was obtained.
`RA3EP1_english_1.1.SkuDef` selects `Data\ra3ep1_1.1.game`; the 1.0 SkuDef
selects 1.0. The inspected BIG/search-path lines otherwise match. This does not
prove which configuration a running launcher selects; no game was launched.

## Static -modconfig support

The 1.1 image contains **more than the option string**:

- `-modconfig\0` at raw 00823094 / preferred VA 00C23094.
- Its eight-byte table row at raw 00823104 pairs the literal with handler
  `00632180`. This is entry 1 in the four-entry table at 00C230FC.
- Caller `006327E0` passes that table and count 4 to parser `006321A0`
  at `00632809`. The parser uses `_strnicmp`, then calls row+4 on a match.
- Handler `00632180` checks remaining argument count >1, reads argv[1],
  calls `004D6960` and returns 2. Missing-argument diagnostics are not shown
  in this handler; do not infer strict rejection from it.
- Setter `004D6960` calls imported `strcpy_s` with destination 00CF2118,
  capacity 100h and the provided path. Return/error handling is not modeled.
- Slice `004D985E`..`004D989D` tests that buffer for nonempty, passes it into
  helper `004D69A0`, and invokes `004D6F10`, branching on AL. These downstream
  helpers are not fully recovered as a config-file parser in this milestone.

This verifies static option recognition, handler dispatch, path storage and
downstream consumption. It is **not a successful mod-load test**, proof of
launcher argument forwarding, or native asset/hash compatibility.

The same `-modconfig` literal also exists in 1.0 at raw 00821AE0. Its table
row at 00821B50 points to `0064AE90`, whose reviewed handler similarly takes
argv[1], calls `004D66E0` and returns 2. Therefore the presence of this option
is not established as a 1.1-only feature. Full cross-version config-loader
behavior is still a separate investigation.

## Reproduce and version boundaries

```powershell
# Reborn: reproduce pinned 1.1 option/table/parser/setter evidence without loading the game or writing config files.
./scripts/Get-Ra3Ep11ModConfigEvidence.ps1 -ImagePath 'D:\Program Files (x86)\Steam\steamapps\common\Command and Conquer Red Alert 3 Uprising\Data\ra3ep1_1.1.game' -AsJson
# Reborn: check seven evidence pins, import names, seven private code mutations and repeated JSON.
./scripts/Test-Ra3Ep11ModConfigEvidence.ps1 -ImagePath 'D:\Program Files (x86)\Steam\steamapps\common\Command and Conquer Red Alert 3 Uprising\Data\ra3ep1_1.1.game'
```

Tests pass. `Get-Ra3Ep1RuntimeTypeTable.ps1` was separately supplied the 1.1
image and correctly refused it with `Unreviewed EP1 executable identity; fixed
runtime table offsets are not admitted.` This check occurs before its fixed
table decoding. The guard was not relaxed to accept 1.1 with 1.0 offsets.
The new command-line audit admits only the reviewed 1.1 SHA and uses bounded
read-only PE snapshots, exact reviewed code pins and final SHA recheck.
No target code, game, launcher or native codec was executed.

## Next priorities

Follow-up: [1.1 runtime type/hash reconciliation](RA3EP11_RUNTIME_TYPE_TABLE.md)
independently locates the arrays and establishes exact ordered identity for
all 1,342 rows plus all 254 previously captured stock roots. This closes the
table rebase below, but not initializer/consumer/stream/authoring/game gates.

1. Independently recover 1.1 runtime name/type/hash table and correlate the
   observed stock manifests; retain a separate version-specific evidence profile.
2. Rebase factory/loader/sidecar/lifetime evidence using reviewed instruction
   boundaries and targets, not a guessed constant address delta.
3. Recover the config-file parser, accepted commands and package registration
   before designing a bounded modconfig loading PoC.
4. Validate authored stream identity/dependencies/serialization, then conduct
   controlled game loading only after the static and build gates are ready.

Overall effort stays **52% complete / 48% remaining**. The 1.1 discovery adds a
version-validation gate; option recognition alone does not advance game-load
completion. Authentic authoring ProcessingHash and EP1 AUDIO dependencies
remain open. No production profile or full compiler-suite rerun (previous 165).
