# WeaponTemplate schema support

<!-- Reborn: distinguish complete compiler schema coverage from retail witnesses and gameplay evidence. -->
The alpha compiler accepts **all 35 nugget choices** declared by the original
Uprising `AssetTypeWeaponTemplate.xsd`. Root coverage includes all **96 own
attributes and 13 child fields**, with native model and writer bindings checked.
There is no four-family authoring whitelist. Unknown XML, authored TypeId
discriminators, invalid field values and unresolved dependencies still fail.

## Nugget coverage

The supported choices are:

| Area | Official elements |
| --- | --- |
| Damage | DamageNugget, DOTNugget, DamageAndSpawnNugget, LeechHealthDamageNugget, LeechPercentMaxHealthDamageNugget, SecondaryDamageNugget, StripMaxHealthPercentNugget |
| Projectile and terrain | ProjectileNugget, ScatterProjectileNugget, TerrainCraterNugget, InformTargetNugget |
| Continuous effects | ActivateLaserNugget, ActivateStreamNugget, ActivateLiftObjectNugget, ActivateLinearDamageNugget, ActivateCircularDamageNugget |
| Impact and control | SuppressionNugget, MetaImpactNugget, ParalyzeNugget, InformationWarfareNugget, AttributeModifierNugget, TintObjectsNugget |
| Object and power actions | WeaponOCLNugget, SlavesAttackNugget, SpecialPowerNugget, DamageContainedNugget, LuaEventNugget, LineDamageNugget, SeedTiberiumNugget, SpendStolenTiberiumNugget, ReportWeaponFiredStatNugget, FireOnObjectsNugget, ContainedObjectAttackNugget, AttachNugget, GrapplingHookNugget |

Each concrete EP1 model retains its actual inherited prefix, reference/list
fields and packed flags. This replaces mismatched KW layouts without treating
an unsupported class as DamageNugget or discarding source fields. Official
defaults are materialized before native compilation. Required schema fields
remain required; for example DamageAndSpawn needs SpawnTemplate.

`InformTargetNugget` supports both MinTimeToImpactFudgeFactor and
MaxTimeToImpactFudgeFactor, including their original 0.85/1.15 defaults.
DOT uses EP1 time fields and RemoveIfHealed; projectile nuggets retain optional
barrel overrides; OCL, laser, attachment and damage modifiers retain their
EP1-only fields. Optional scalar child proof now reads the child's text, fixing
the second `SpecificBarrelOverride` value in the original Akula torpedo weapon.

## Validation

- **251 literal standalone original weapons**, selected from the base and EP1
  Weapon XML with EP1 override precedence, passed independent source/native
  comparison and rebuilt **byte-identically** to retail tokenized records.
- The static and WorldBuilder metadata inventory contains real uses of **27
  families**. The other eight are DamageAndSpawnNugget, TerrainCraterNugget,
  LuaEventNugget, SeedTiberiumNugget, InformationWarfareNugget,
  SpendStolenTiberiumNugget, ScatterProjectileNugget and
  ContainedObjectAttackNugget. These eight use explicit official schema-derived
  metadata row contracts, without replacing independently learned stock grammar.
- All **35 choices** passed authored source/native/metadata compilation. The
  eight families without retail examples also passed nondefault field/filter/
  reference cases. Three existing mixed-nugget authored weapons still pass.
- **86 optional-reference/storage/corruption controls** pass, including the
  28-weapon linked stream/package and its original populated token golden.
- The unmodified user `AlliedCoastalGun.xml` prepared across **12 source files**
  and compiled **6 authored assets**: two GameObjects, two WeaponTemplates, one
  FXList and one ObjectCreationList. Its **6 local / 67 stock** reference
  occurrences resolved, and BIN/RELO/IMP readback passed (checksum `5e3a204c`).

The original-record corpus deliberately excludes inherited/expression-valued
declarations from byte-identity comparison; those declarations still use the
ordinary source preparation and inheritance pipeline when authored in mods.
These are offline compiler checks. No game was launched, no user mod XML was
edited and no user mod output was published. Gameplay of the eight unused
retail schema families has not been established by these checks.

Evidence is retained privately under
`alpha/.work/weapon-complete-5148cea59c444143be8236c26a6dd81a/` as recorded by
`alpha/.work/weapon-complete-current-path.txt`: `corpus3.log`,
`retail-nugget-inventory.json`, `coastal-prepare.log`, `coastal-build.log`,
`coastal-build/stream-readback/` and `optional-references.log`.

## Implementation

The compiler registry and qualification are in
`source/BinaryAssetBuilder.ManifestInspector/WeaponTemplateCompiler.cs`.
EP1-specific additional records and writers are in
`source/SageBinaryData/SageBinaryData/Ep1WeaponNuggetsComplete.cs` and
`source/BinaryAssetBuilder.XmlCompiler/Marshaler.Ep1WeaponNuggetsComplete.cs`.
Independent field/allocation proof remains in SchemaNativeContract; stock and
schema metadata contracts remain in StockMetadataGrammar.
