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
the first is external to that manifest's named assets. It is now resolved from
global metadata as Multisound:SOV_Conscript_GunFireMS (not from the audio manifest).
Zero IMP bytes do not mean no dependencies.

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

## Damage token chain and direct names

The review now follows Nuggets through its count record and polymorphic class
table instead of scanning for a coincidental float. Observed list table: 1428;
class table: 1452; DamageNuggetType derived token table: 1464; native nugget: 2352.
The native list has one member. Damage token points to 2392 (native + 40),
float 5. DamageType points to 2428 (native + 76), enum 21, GUN.
The class hashes are C0F53E99 for DamageNuggetType and C7C2CC04 for its
WeaponEffectNugget base. Only these selected scalar/class/list relationships
are verified, not every nugget field or the complete 156-byte native tail.

`weapon-stock-review <static.manifest> <global.manifest>` verifies the three
direct reference names against EP1 metadata. Global SHA256:
`08A415789062B1707EDBA3C456884B94791097505431E17A03D10EE34DF050FD`.
The Multisound itself references AudioEvent:SOV_Conscript_GunFire and
AudioEvent:SOV_Conscript_GunFireDelayed; recursively proving those and the
underlying audio files remains separate from resolving weapon-level names.
Supplying audio.manifest alone as the additional manifest is refused because
it does not resolve the required Multisound. Static-only review still reports
unresolved external names without claiming direct-name verification.

Checks pass: existing five synthetic root corruptions, three detached copies
of the actual weapon slice (wrong damage, list count and nugget class), identical
repeat JSON and wrong-reference-manifest refusal. Incremental build has zero
warnings/errors. These changes do not add a weapon build to the exported e SDK.

## Real native XML marshaling comparison

`weapon-native-probe <reference-Weapon.xml> <ep1-static-manifest>` selects the
unchanged AK47 XML into an owned document, verifies its three expected textual
references, then prepares fixed diagnostic indices and the known nugget TypeId.
It invokes the actual weapon marshaler twice; this is not stock-byte patching.
It does not validate the full XML against XSD or prove general dependency
preparation, and does not create a tokenized stream or package.

The initial native compile differed at six byte positions. Corrections:

- `Marshaler.WeaponTemplate.cs`: FinishAttackOnceStarted defaults to false,
  matching the official EP1 XSD and stock byte, not the old true fallback.
- `WeaponTemplate.cs`: DamageNugget InvalidTargetStatus starts at native +108,
  before DamageScalarDetails at +140; the 156-byte struct size is unchanged.
- `Includes/DamageFX.cs`: VERSION7 uses the full EP1 schema enum order,
  DEFAULT=-1 and SOVIET_AK47=45; the legacy non-VERSION7 branch is retained.

After those corrections, all 528 normalized native BIN bytes match.
Compiled and normalized-stock SHA256:
`3DA83658B59755C6F5FDFDA98E2AE740E51798CC33CB85CBB7CB28BE8D70A841`.
The compiler emits 28 native RELO bytes and 16 IMP bytes; stock tokenized
representation has 28 RELO and zero IMP, so native equality is not full
tokenized output equality. Remaining gates include complete token/reference
encoding, dependency preparation and scoped publication. WeaponTemplate
remains disabled in the exported e alpha.

Independent stock RELO reads also match all normalized native relocation
locations. An actual second XML compile with Damage=50 changes only the
four-byte Damage slot at native offset 412; RELO/IMP stay identical. Repeated
baseline compilation is deterministic. The broad compiler self-test passes
with the corrected defaults/ABI/enum, and final focused layout/native checks
pass. An overlapping incremental build initially hit the running test's EXE
file lock; retry after the test completed succeeded with zero warnings/errors.
