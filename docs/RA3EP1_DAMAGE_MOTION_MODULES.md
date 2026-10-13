# Tracker/freefall native and reference batch

## Outcome and scope

SecondaryDamageTrackerModuleData (B21C7E67) and FreefallUpdateModuleData (FC222F04)
now have dedicated models, marshalers and actual BehaviorModuleData polymorphic
dispatch. Ep1DamageMotionModules.cs and Marshaler.Ep1DamageMotionModules.cs
implement the native layer; Ep1DamageMotionSmokeTest.cs verifies it.
Official schemas/XML, installed game files and exported alpha h are unchanged.
There is no new tokenized GameObject stream/package or gameplay proof.

The two previously reviewed objects now have four absent native model families:
ToggleStatusSpecialAbilityUpdate, BuildingDestruction, TechUpgrade and
LUAEventListBehavior. Only ToggleStatusSpecialAbilityUpdate remains absent in the
Conscript module inventory. Presence of the other models does not establish
complete fields/defaults/dispatch/import semantics or full object support.

## Independently observed layouts

Tracker is 36 bytes: update base +0, UpdateDuration +8, DissipateAmount +12,
MaxDamageClampScaler +16, Color pointer +20, EffectInfo count/items +24/+28,
ObjectStatusValidation pointer +32. RGB is 12 bytes; the existing status validation
payload is 64 bytes.

Each 16-byte effect stores Fraction +0, modifier reference +4, option mask +8,
OnEnterDisable +12, OnEnterCancelSpecialAbilities +13 and SetUpgradedArmor +14.
The options are enum-indexed DIE_ON_ENTER/CRASH_IF_AIRBORNE bits. Native order
differs from XSD attribute declaration order and is anchored by stock member
token offsets/type tags, not guessed from source order.

Freefall is 80 bytes: update base +0, complete 60-byte model-condition mask +8,
InitialVelocity +68, Acceleration +72 and TerminalVelocity +76. Model condition
and motion parameter payloads match the Conscript stock slice exactly.

## Stock/native comparisons

| Selected variant | Diagnostic root BIN / RELO / IMP |
| --- | --- |
| Conscript tracker, five effects, color, no status child | 740 / 24 / 24 |
| Allied yard tracker, five effects, color, status child | 804 / 28 / 24 |
| Conscript freefall | 692 / 16 / 0 |

These dimensions include an incomplete diagnostic GameObject root and its module
container/item allocation, not linked-stream headers or a playable object.
The tracker comparisons include every selected root and nested payload byte
after rebasing only the three declared pointers and modifier reference words.
Import words are compared semantically by type/instance identity, not by local
ordinal coincidence. Freefall compares all 80 native bytes directly.

Own scalar offsets/type tags, effect Count anchor, list markers and all six effect
member fields bind to the independently read native records. Every generated
relocation and import slot is checked, including terminators. The input stock
stream is only read as a bounded fingerprint-admitted asset slice.

## Source/reference boundary

DamageMotionPipeline.xsd includes untouched official tracker/freefall, primitive,
reference and condition/status definitions. Its UpdateModuleData/AttributeModifier
bases are explicitly inheritance-only diagnostic stubs, not full official module
or GameObject schema authority. The copied status field inventory is drift-checked
against official BaseModules.xsd. Full source inheritance/Includes/definitions,
required complete object modules and output processors are not enabled here.

Plain AttributeModifer_CryoBeam_02/04/06/08/10 names pass through the actual Core
schema-driven reference normalizer. Each typed handle resolves uniquely in the
explicit linked stock manifest. Tracker native import slots select these handles.
No captured numeric dependency indices are used as authored input.
Reference XML provides module IDs and literals from BaseInfantry/BaseStructure;
the resolved diagnostic MAX_SECONDARY_DAMAGE_SCALER is the independently
stock-confirmed 200% value, not a new generic definition evaluator.

## Controls and remaining gates

Tests cover deterministic repeat, changed IDs, optional/default tracker with no
children, changed duration/dissipation/scalar/fraction/velocity, both effect option
bits and packed bools, wrong field offsets, missing required Fraction, unknown
option/model-condition names and unresolved modifier targets. Caller Core settings
are restored in a finally boundary.

This demonstrates selected complete native module graphs and named import
normalization, not generic recursive token encoding or arbitrary new GameObject
publication. Next work includes the toggle special-ability/nugget graph and its
multiple strong/weak references, then complete-object field/default/token
comparisons and a mixed GameObject/Armor Core stream. New W3X/TGA/DDS processing
remains separate.

## Final validation

artifacts/DamageMotion-a passes **26/26 Full groups** in **593.668 seconds**,
excluding the 4.86-second incremental compiler build (zero warnings/errors).
The damage/motion group passes in 9.356 seconds and the complete broad compiler
group in 538.820 seconds. All eight compiler/runtime hashes remain unchanged
during the matrix. Earlier basic-module, root-layout, AI, armor/source and weapon
regressions remain covered. The initial native-layer build had zero errors and
111 existing warnings; this is distinct from the clean incremental matrix build.

## Command

```powershell
# Reborn: offline selected nested-module/native/Core-reference validation only; no package/install/game.
& .\source\BinaryAssetBuilder.ManifestInspector\bin\x86\Release\net8.0\BinaryAssetBuilder.ManifestInspector.exe `
  ep1-damage-motion-self-test 'C:\YOUR\UNPACKED\data\static.manifest'
```
