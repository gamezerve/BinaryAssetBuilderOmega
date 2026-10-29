# Allied unit XML batch

<!-- Reborn: distinguish source/native compilation evidence from gameplay and ART-resource tests. -->
This batch covers the 25 requested Allied unit XML files, including their
explicitly authored ground/naval GameObject variants. It uses a private copy
of Reborn Data; ART includes are excluded only in that copy. User Reborn sources
and published mods are not rebuilt by these checks.

## Added or completed processor areas

- ActivateModuleSpecialPower: original unsuffixed wire class, SpecialPower base,
  source-sized Modules and inherited trigger/deactivation records.
- BeamSpecialAbilityUpdate: EP1 weapon, target status, beam options, duration,
  sweep/damage intervals and optional data.
- DominateEnemySpecialPower: EP1 conversion flags, effects and optional filter.
- InvisibilityBroadcastUpdate and MagneticEffectUpdate: filters, status validation,
  target/draw identities, range and pull settings. The required magnetic filter
  is inline, whereas optional filters/status records own separate allocations.
- LinearDamageUpdate: sweep data, terrain/locomotor settings and optional filters.
- ReconstituteStateSpecialAbility and StoreCurrentStateUpdate: local update identity,
  time history, position effects and modifier.
- ShieldSphereUpdate: the complete shield/sphere hierarchy, filter and status masks.
- SpecialDisguiseUpdate: EP1 effects, timing and source-sized terrain mappings.
- ModelConditionEvaEventSelectorClientBehavior and UpgradeSoundSelectorClientBehavior:
  actual EP1 nested EVA/voice/sound arrays and weak upgrade lists.
- HitReactionBehavior: optional populated ObjectStatusValidation.
- Client voice selectors: populated NamedEntry/ObjectSpecificEntry metadata.
- Multiple reviewed AIUpdate leaves use source-driven inline AI composition.
- The original anonymous Spy EVA selector retains its zero local Poid;
  duplicate named module identities are still rejected.
- Numeric asset expressions admit scalar `+`, `-`, `*`, `/`, unary signs and
  parentheses over visible case-sensitive definitions before inheritance.
  Exact literal substitutions and earlier diagnostic profiles are preserved.
  This is scalar arithmetic, not a general expression-language implementation.

## Developer verification

The source review reports each fully expanded object's admission result.
The native review collects independent object failures in one process and never
publishes a partial stream. Normal Studio builds still stop on errors.
Focused module controls compare original EP1 source/native records, verify
newly generated metadata and reject corrupt native/auxiliary data.

## Verified result

All 25 requested XML files passed source preparation and complete native compilation.
The emitted set contains 28 GameObjects: the 25 named units plus
`AlliedMiner_Naval`, `AlliedAntiInfantryVehicle_Ground` and `AlliedMCV_Naval`.
Supporting declarations add four ArmorTemplates, four ObjectCreationLists and
five LocomotorTemplates, for **41 assets**. All **9 local / 3424 stock** reference
occurrences resolved, and BIN/RELO/IMP readback passed (checksum `51537807`).

The final build took **44.65 seconds**, including source admission, compilation,
linkage, private BIG creation and stream readback. An explicit per-build scope
shares the immutable schema class-name catalog across stock reviews; every
stock slice and generated source/native/metadata graph is still checked. The
scope is released on success or failure and is not reused across Studio builds.

Focused controls passed for 13 original generated module families, 13 corrupt
native/auxiliary refusals and 12 arithmetic checks. The original local-definition,
include-definition and definition-subset suites also passed.

The checked-in `tests/fixtures/allied-unit-batch/data/Static_Mod.xml` lists the
25 explicit source paths. The measured user-source snapshot and receipts remain
private under `alpha/.work/allied-units-batch-65d203de32634712831cf7093e6ee65b/`:
`verification.json`, `modules11.log`, `native11.log` and `native11/stream-readback`.

These are offline compiler checks. No user mod source or published output was
changed, and no game was launched. ART resources were excluded only in the
private test copy; gameplay and newly authored art remain separate checks.
