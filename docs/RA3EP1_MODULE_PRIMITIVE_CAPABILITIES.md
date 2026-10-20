# Conscript module primitive capabilities

Updated October 20, 2026.

The live alpha now accepts **384 fixed-width primitive attribute slots** across the
49 existing Conscript modules. The inventory contains 225 per-family attributes across
32 reviewed module families, including families with no direct primitive attributes.
Repeated instances, such as seven SlowDeath modules, can be edited independently.
Use **EALAModStudio.exe → Build Mod** with the existing complete module graph.
In the supplied RebornTest project, these modules are defined in
`alpha/mods/RebornTest/data/ConscriptBase.xml`. The derived `Conscript.xml`
continues to contain root attribute overrides; general derived child merging
is still outside the frontend contract.

## Supported authoring

Only the direct module attributes listed below are enabled by this capability.
Official schema validation, defaults and native representability remain enforced.
The concrete field type determines whether a value is a real, signed/unsigned
integer, boolean, percentage, Time, Angle, Duration or Velocity. Percentages keep
the engine's percent conversion; Time, Angle, Duration and Velocity keep their
engine unit conversions. SageBool accepts true/false (including initial uppercase)
and 0/1. Supported unsigned fields retain hexadecimal source syntax.

Optional primitive attributes may be omitted and use their official default,
including ActiveBody.MaxHealth. Omitting the module or changing its id, position,
children, lists, enums, flags, resource bodies or non-root references is outside
this capability. Empty rows mean that family has no newly editable direct
primitive attributes; its previously qualified original graph still compiles.

## Per-family inventory

| Existing module | Instances | Editable direct attributes |
| --- | ---: | --- |
| Draws/ScriptedModelDraw | 2 | `InitialRecoilSpeed`, `MaxRecoilDistance`, `RecoilDamping`, `RecoilSettleSpeed`, `HighDetailLODThreshold`, `LowDetailLODThreshold`, `AlphaCameraFadeOuterRadius`, `AlphaCameraFadeInnerRadius`, `AlphaCameraAtInnerRadius`, `BirthFadeTime`, `OkToChangeModelColor`, `AnimationsRequirePower`, `UseYAxisForTurretRotation`, `TrackMarksOnlyWhenCorneringQuickly`, `UseProducerTexture`, `NoRotate`, `UseFiringArcRotation`, `Selectable`, `RandomTextureFixedRandomIndex`, `ParticleBonesCheckDrawable`, `ShadowForceDisable`, `SwitchModelLODMode`, `StaticModelLODMode`, `ShowShadowWhileContained`, `MultiPlayerOnly`, `AffectedByStealth`, `InvertStealthOpacity`, `HighDetailOnly` |
| Draws/TracerModelDraw | 2 | `MinLength`, `MaxLength`, `Width`, `MinSpeed`, `MaxSpeed`, `SweepSpeed`, `SpreadAngle`, `MinTracersPerFrame`, `MaxTracersPerFrame`, `FrameLifeTime`, `WeaponSlotID`, `TextureIndex`, `UseAdditiveBlending` |
| Behaviors/ObjectSMCHelper | 1 | None |
| Behaviors/ObjectDefectionHelper | 1 | None |
| Behaviors/AttributeModifierPoolUpdate | 1 | None |
| Behaviors/SquishCollide | 1 | `CrushAnimTime`, `UseVehicleStyleCrush`, `AllowAllyCrush` |
| Behaviors/CrushDie | 1 | `TotalCrushSoundPercent`, `BackEndCrushSoundPercent`, `FrontEndCrushSoundPercent` |
| Behaviors/StancesBehavior | 1 | None |
| Behaviors/DestroyDie | 2 | None |
| Behaviors/CreateObjectDie | 1 | None |
| Behaviors/FXListBehavior | 3 | None |
| Behaviors/FireWeaponWhenDead | 1 | `RequiresAllTriggers`, `RequiresAllConflictingTriggers`, `Permanent`, `WaypointMode`, `WaypointModeTerminal`, `DelayTime`, `InitiallyActive`, `ActiveDuringConstruction`, `ContinueToBezierDestination`, `FireAtGround`, `UseMyKillerAsWeaponSource` |
| Behaviors/AutoHealBehavior | 1 | `RequiresAllTriggers`, `RequiresAllConflictingTriggers`, `Permanent`, `WaypointMode`, `WaypointModeTerminal`, `HealingAmount`, `HealingDelay`, `StartHealingDelay`, `Radius`, `RespawnMinimumDelay`, `InitiallyActive`, `AffectsWholePlayer`, `AffectsContained`, `HealOnlyIfNotUnderAttack`, `HealOnlyIfNotInCombat`, `HealOnlyOthers`, `NonStackable`, `RespawnNearbyHordeMembers` |
| Behaviors/SecondaryDamageTracker | 1 | `UpdateDuration`, `DissipateAmount`, `MaxDamageClampScaler` |
| Behaviors/RunOffMapBehavior | 1 | `RequiresSpecificTrigger`, `DieOnMap`, `FlyingOffMap` |
| Behaviors/LevelUpUpgrade | 3 | `RequiresAllTriggers`, `RequiresAllConflictingTriggers`, `Permanent`, `WaypointMode`, `WaypointModeTerminal`, `LevelsToGain`, `LevelCap`, `DoFlash` |
| Behaviors/SuppressionUpdate | 1 | `UpdateDelay`, `Suppressability`, `SuppressionDuration` |
| Behaviors/FreefallUpdate | 1 | `InitialVelocity`, `Acceleration`, `TerminalVelocity` |
| Behaviors/WeaponSetUpdate | 1 | None |
| Behaviors/Physics | 1 | `MaxXRotationVelocity`, `MaxYRotationVelocity`, `MaxZRotationVelocity`, `GravityMultiplier`, `ShockStunnedTimeLow`, `ShockStunnedTimeHigh`, `ShockStandingTime`, `FirstHeight`, `SecondHeight`, `FirstPercentIndent`, `SecondPercentIndent`, `BounceCount`, `BounceDistance`, `BounceFirstHeight`, `BounceSecondHeight`, `BounceFirstPercentIndent`, `BounceSecondPercentIndent`, `FirstPercentHeight`, `SecondPercentHeight`, `CurveFlattenMinDist`, `TumbleRandomly`, `AllowBouncing`, `KillWhenRestingOnGround`, `OrientToFlightPath`, `IgnoreTerrainHeight` |
| Behaviors/SlowDeath | 7 | `SinkRate`, `ProbabilityModifier`, `ModifierBonusPerOverkillPercent`, `SinkDelay`, `SinkDelayVariance`, `DestructionDelay`, `DestructionDelayVariance`, `DecayBeginTime`, `FlingForce`, `FlingForceVariance`, `FlingPitch`, `FlingPitchVariance`, `FadeTime`, `FadeDelay`, `ShadowWhenDead`, `Fade` |
| Behaviors/SpecialPower | 1 | `AttributeModifierRange`, `WeatherDuration`, `SetModelConditionTime`, `GiveLevels`, `BurnDecayModifier`, `MaxDistanceFromCommandCenter`, `UpdateModuleStartsAttack`, `StartsPaused`, `AttributeModifierAffectsSelf`, `AttributeModifierWeatherBased`, `DisableDuringAnimDuration`, `IdleWhenStartingPower`, `AffectGood`, `AffectEvil`, `AffectAllies`, `AvailableAtStart`, `AdjustVictim`, `UseDistanceFromCommandCenter`, `ChildModuleHandlesFX` |
| Behaviors/ToggleStatusSpecialAbilityUpdate | 1 | `StartAbilityRange`, `AbilityAbortRange`, `PreparationTime`, `PersistentPrepTime`, `PersistentCount`, `PackTime`, `UnpackTime`, `PreTriggerUnstealthTime`, `PackUnpackVariationFactor`, `ParalyzeDurationWhenCompleted`, `ParalyzeDurationWhenAborted`, `MaxSpecialObjects`, `EffectDuration`, `EffectValue`, `EffectRange`, `FleeRangeAfterCompletion`, `AwardXPForTriggering`, `SkillPointsForTriggering`, `UnpackingVariation`, `FreezeAfterTriggerDuration`, `AttributeModifierDuration`, `RequireAndSpendTiberiumOnCaster`, `MinimumUnpackTimeAfterSpecialPowerInitiation`, `StartRechargeOnExit`, `GoIdleInStartPreparation`, `FaceTarget`, `ReloadAllWeaponsOnToggle`, `ForceIdleAfterToggle` |
| Behaviors/StatusBitsUpgrade | 1 | `RequiresAllTriggers`, `RequiresAllConflictingTriggers`, `Permanent`, `WaypointMode`, `WaypointModeTerminal` |
| Behaviors/HitReactionBehavior | 1 | `LightDamageHitReactionLifeTimer`, `MediumDamageHitReactionLifeTimer`, `HeavyDamageHitReactionLifeTimer`, `LightDamageHitReactionThreshold`, `MediumDamageHitReactionThreshold`, `HeavyDamageHitReactionThreshold`, `RotateToSourceWhenDamaged`, `RotateToSourceIgnoreIdleCheck`, `GoIdleWhenDamaged` |
| Behaviors/ModelConditionUpgrade | 2 | `RequiresAllTriggers`, `RequiresAllConflictingTriggers`, `Permanent`, `WaypointMode`, `WaypointModeTerminal`, `TempConditionTime` |
| AI/AIUpdate | 1 | `StopChaseDistance`, `MinCowerTime`, `MaxCowerTime`, `BurningDeathTime`, `ComboLocoAttackDistance`, `IdleTargetTime`, `MaxCombineChildren`, `SpawnOffsetRadius`, `StandGround`, `FadeOnPortals` |
| Body/ActiveBody | 1 | `MaxHealth`, `MaxHealthDamagedPercentage`, `MaxHealthReallyDamagedPercentage`, `InitialHealth`, `DodgePercent`, `EnteringDamagedTransitionTime`, `EnteringReallyDamagedTransitionTime`, `CheerRadius`, `RemoveUpgradesOnDeath`, `BurningDeathBehavior` |
| ClientUpdates/LargeGroupAudioClientUpdate | 1 | `UnitWeight` |
| ClientBehaviors/ModelConditionAudioLoopClientBehavior | 3 | None |
| ClientBehaviors/ModelConditionSoundSelectorClientBehavior | 2 | None |
| ClientBehaviors/AnimationSoundClientBehavior | 1 | None |

## Verification and implementation

`ConscriptModuleScalarContract.cs` intersects the effective official schema with
the concrete reviewed native module model. Each offset, width and wire type is
checked against the complete independently stock-qualified token directory.
Native pointers locate modules after dynamic root-string reallocation. Source
values are independently encoded, checked, and restored before the complete
stock ownership oracle validates all remaining bytes, relocations and imports.
The serializer checks the module values again before stream publication.

One grouped test edits all 384 slots together with root scalar/enum/reference
edits and root string resizing. Complete native, metadata and stream readback
passes; all 384 individual payload corruptions are rejected. Additional controls
cover default omission, unsigned hex and numeric boolean spelling. The full
authored-trial diagnostic rejects 524 source/native corruptions.

The Studio backend regression combines ScriptedModelDraw.InitialRecoilSpeed,
Physics.GravityMultiplier, SlowDeath.SinkDelay, AIUpdate.StopChaseDistance and
StandGround, and ActiveBody.MaxHealthDamagedPercentage in one package, alongside
dynamic root strings and removed root references. An invalid module scalar must
preserve the last successful package. The developer source is restored afterward;
the user's Reborn source and output are preserved. These checks run offline.

Final Studio backend regression: **25/25 passed**. The restored developer package
retains SHA-256 `87241958D0EC4CDB3A133FFC837E023A01B51716924F6B532A30BCBDE7EDF918`.
The live alpha compiler matches the verified Release/x86 generation. No game was
executed and the user's Reborn mod was neither built nor modified.
