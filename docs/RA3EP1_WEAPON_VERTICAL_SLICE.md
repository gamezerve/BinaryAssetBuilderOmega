# First weapon vertical slice — October 12, 2026

## Target and current evidence

The next gameplay type candidate is
`WeaponTemplate:SovietAntiInfantryInfantryAK47`, the Conscript's normal primary
weapon. The reference `GlobalData/Weapon.xml` at lines 3733 onward declares
AttackRange=150, WeaponSpeed=999999, FireSoundPerClip=SOV_Conscript_GunFireMS,
FireFX/FireVeteranFX=FX_MuzzleSoviet, three range-duration children and one
DamageNugget with Damage=5, DamageType=GUN. This is not the Molotov weapon.
The reference source stays unchanged.

`WeaponStockReview.Inspect` and CLI `weapon-stock-review` now read the static
manifest metadata and only the selected 2508-byte BIN slice plus its eight-byte
header. They verify EP1 target/type fingerprint, tokenized status, checksum,
root token names/tags/bounds and the two known scalar/native offset relationships.
They do not dump the full stock BIN or compile/patch a weapon.

| Stock property | Observed value |
| --- | --- |
| TypeId / TypeHash | 94D4D96E / 27E8D984 |
| BIN slice offset / length | 344299100 / 2508 |
| RELO / IMP lengths | 28 / 0 |
| Root token entries | 109 |
| Native root base / native root size | 1980 / 340 |
| AttackRange token value offset | 1984 (base + 4), float 150 |
| WeaponSpeed token value offset | 2024 (base + 44), float 999999 |

Manifest SHA256: `74FACD9056EEE20942BB274A924BEFFB03D7ADBB71B9609E804A39619B2C6081`.
Slice SHA256: `2B0C3AABB5521C7C8315A1A5EA22DFCF8D681C6546E9AC58DBF0D0495B2A8C6B`.
Manifest references, in order: `A3A7AF37:9EF0AACB`, then twice
`86682E78:902735BD`. The latter is named FXList:FX_MuzzleSoviet in static;
the first is external to that manifest's named assets. Resolving it against
audio metadata is still required. Zero IMP bytes do not mean no dependencies.

## Why the existing processor is not enough yet

`SageBinaryData.WeaponTemplate` and `Marshaler.WeaponTemplate.cs` already implement
the 340-byte native root; compiler/layout fixtures cover selected fields.
The real tokenized payload is much larger and includes nested tables/allocations.
Root scalar recovery is not a complete native payload comparison, tokenizer,
schema/default proof or dependency/linking proof. The experimental armor
tokenizer cannot simply be applied to weapons. Alpha WeaponTemplate input
remains refused; the six-sample e alpha binaries are not changed by this probe.

## Next implementation gates

1. Resolve each stock reference and reproduce schema defaults, child order,
   enum/bitmask values and duration allocations for this unchanged weapon.
2. Recover the full root and nested DamageNugget token trees, including typed
   asset references and relocation words. Recompile from XML and compare all
   2508 BIN bytes, 28 RELO bytes and manifest dependency order independently.
3. Admit a narrow fixed-identity edit (initially Damage=5 to a distinct value),
   ensuring all other output is retained from actual compilation, not a stock
   binary patch. Add malformed/dependency/corruption/repeat controls.
4. Extend scoped Core publication and default-reader package verification only
   after this proof; keep arbitrary GameObject/Weapon builds closed.
5. Request a separately authorized fresh-skirmish test with stock armor and
   normal Conscript AK-47. Armor overrides must not confound the weapon test.

## Checks run

The focused decoder self-test passes a synthetic root and five corruptions
(tag, out-of-bounds pointer, duplicate field, wrong scalar, native offset drift).
Two real stock reviews return identical JSON. An EP1 global manifest lacking
the required AK47 identity is refused. Incremental build: zero warnings/errors;
the preceding rebuild had 111 existing warnings and zero errors. No new full
generic regression suite, package build, GUI or game run was performed here.
