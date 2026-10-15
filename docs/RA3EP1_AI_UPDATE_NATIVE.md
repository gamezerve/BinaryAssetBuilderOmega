# Common EP1 AIUpdate and unit target chooser native port

October 15, 2026. The common AIUpdateModuleData used by the selected Construction
Yard and Conscript does not have the legacy Kane's Wrath native layout. Separate
EP1 models and marshalers now implement the **76-byte AI record** and optional
**40-byte unit target chooser**. The concrete wire ID remains **08DBC00E**; it is
not a hash of the new C# type name. The concrete AI root dispatch now selects this
EP1 allocator. Other legacy derived AI classes are not automatically upgraded.

## AI record

| Field | Offset | Representation |
| --- | ---: | --- |
| UpdateModuleData base | 0 | Eight bytes: concrete TypeId and local id hash |
| AttackPriority | 8 | ASCII length/pointer |
| AutoAcquireEnemiesWhenIdle | 16 | Five enum-index bits |
| StopChaseDistance | 20 | Float |
| MinCowerTime / MaxCowerTime / BurningDeathTime | 24 / 28 / 32 | Float seconds |
| ComboLocoAttackDistance | 36 | Float |
| SpecialContactPoints | 40 / 44 | Inline count / ASCII-header array pointer |
| IdleTargetTime | 48 | Float seconds |
| MaxCombineChildren | 52 | Unsigned 32-bit integer |
| SpawnOffsetRadius | 56 | Float |
| StateMachine | 60 | Seventeen reviewed enum literals |
| DefaultLocomotorSet | 64 | INVALID=-1, NORMAL=0 through SPECIALABILITY_ACTIVE=18 |
| UnitAITargetChooserData | 68 | Optional forty-byte record pointer |
| StandGround / FadeOnPortals | 72 / 73 | Packed booleans; padding +74/+75 |

The KW model's removed fields cannot be retained as padding or allowed to shift
the new state-machine/default-locomotor/chooser words. State-machine and locomotor
literal inventories are checked against the reviewed official schema candidate;
the negative locomotor sentinel uses its actual value, not Enum.GetNames ordering.

## Unit target chooser

| Field | Offset | Default |
| --- | ---: | --- |
| IdleScanDelay / ReacquireDelay / StartupDelay | 0 / 4 / 8 | 0.8 / **60** / 0 seconds |
| SympathyRange | 12 | 25 |
| CanPickDynamicTargetType | 16 | ALWAYS bit 3, value 8 |
| ObjectVisionRange | 20 / 24 | Count/items, empty-only in this native profile |
| CheckVisionRange / CanPickDynamicTargets | 28 / 29 | false / true |
| RotateToTargetWhenAiming / CanAutoAcquireNonAutoAcquirable | 30 / 31 | **false** / false |
| CanChaseAttack / CanOnlyAttackTargetsInFront | 32 / 33 | true / false |
| Padding | 34 / 35 | Zero |
| TargetingCompareList | 36 | Strong DefaultTargetingCompareList reference |

Legacy chooser defaults were 5 seconds for reacquisition and true for rotation;
the new defaults come from the actual EP1 declaration and match the selected
stock chooser. Its dynamic flags, vision-list storage and extra booleans are also
new versus that legacy model. Nonempty ObjectVisionRange explicitly fails rather
than being ignored; its filter/list payload and recursive metadata remain open.

## Evidence

`ep1-ai-update-self-test <static> <global>` uses literal source recovered from the
read-only official XML references, then validates it in the actual GameObject AI
container against the reviewed schema candidate. A focused fixture includes the
actual official AI/chooser declarations. Attribute types/defaults/use constraints
are drift-checked, and actual Core reference normalization inserts defaults and
occurrence indices. Independent static/global metadata must contain each target
exactly once. No captured numeric index is accepted as authored source input.

Two complete selected native graphs match independent stock bytes:

- AlliedConstructionYard: all 76 AI bytes; NO acquire mask, StructureAIStateMachine,
  no chooser.
- SovietAntiInfantryInfantry: all 76 AI bytes and all 40 chooser bytes; YES acquire
  mask, UnitAIStateMachine and the real DefaultTargetingCompareList identity.

Only the independently checked chooser pointer and strong-reference index are
canonicalized before the full byte comparisons. Every own primitive token target
is separately anchored, including the chooser's inherited fields, six packed
booleans, dynamic flag word and empty vision-list count anchor. Stock contact lists
are empty; nonempty contact arrays currently have authored, not stock proof.

Authored coverage includes **680** state/locomotor/optional-chooser/string cases,
32 acquire masks, 16 dynamic-target masks and all 64 chooser-boolean combinations.
Repeated native/RELO/IMP buffers match. Scalar/time edits, unsigned decimal/hex/0.0
syntax, zero/default distinction and maximum unsigned values have explicit checks.
The bounded text profile admits non-NUL ASCII up to 4096 characters and at most
32 contact points, with positive boundary and over-limit/encoding refusal cases.
This guard precedes the legacy ANSI allocator; Unicode/multibyte support is not
implied. Schema-legal exponent overflow must still refuse nonfinite native fields.
All eight AI/chooser booleans also accept the official numeric 0/1 forms, not
only named true/false; the legacy generic bool.Parse path rejected numeric input.
Six official lexical forms have explicit packed-byte checks in this scoped port.
Nineteen source/dependency refusals cover invalid/unknown fields and enums,
fractional/overflowing unsigned values, missing/wrong/duplicate targets, nonempty
vision filters, nonfinite exponent results and unsupported text/size/list counts.

The real inline GameObject AI container is exercised at counts 0, 1 and 2, with a
single shared actual Core occurrence table. Two chooser references retain separate
indices and correct import slots. Three AI entries refuse. An otherwise selected
Body/SMC graph containing AI also refuses the existing partial token composer:
until AI metadata is implemented it cannot silently publish an incomplete object.

## Files and next gate

- `source/SageBinaryData/SageBinaryData/Modules/Ep1AIUpdate.cs`
- `source/BinaryAssetBuilder.XmlCompiler/Marshaler.Ep1AIUpdate.cs`
- `source/BinaryAssetBuilder.XmlCompiler/Marshaler.ModulesAIModuleData.cs`
- `source/BinaryAssetBuilder.ManifestInspector/Ep1AIUpdateSmokeTest.cs`
- `tests/fixtures/AIUpdatePipeline.xsd`

The next gate is generated AI/chooser inheritance/list metadata, followed by
integration of the inline AI list with the existing shared combat/geometry/root
projection and its cross-container identity/reference checks. Nonempty vision
filters, other concrete AI classes, Draw/client graphs and general GameObject
stream publication remain unfinished. The partial composed root remains at 99
fields: this native-only AI milestone does not increase that counter.

The Studio alpha is unchanged. No game launch, installation change, mod package,
new playable unit or new gameplay evidence is claimed.

## Regression checkpoints

The initial `artifacts/Ep1AIUpdate-a/batch-report.json` passes **52/52 Focused
groups in 289.062 seconds**, AI group 14.919 seconds, eight runtime hashes stable
and no game/installation changes. That checkpoint predates the final ASCII/list
guards and numeric-boolean helper; its AI test has 15 source/dependency refusals.
The final source version is rechecked separately before publication.
The portable PDB's compiler-recorded SHA-256 source hashes are independently
matched to the current `Marshaler.Ep1AIUpdate.cs` and `Ep1AIUpdateSmokeTest.cs`
files. Both match, confirming that the final guards and lexical checks are in
the compiled outputs rather than merely present in the working tree.

Final `artifacts/Ep1AIUpdate-b/batch-report.json`: **52/52 Focused groups pass
in 336.633 seconds**, AI group 22.782 seconds, eight runtime hashes stable.
The final AI group includes 19 source/dependency refusals, text/list boundaries,
six boolean spellings and the unpublished-AI composer refusal. The fresh batch
build succeeds in 110.81 seconds with two existing CS8632 audio nullable-context
warnings and no errors. GameExecuted, InstalledFilesModified and
AlphaCapabilitiesExpanded are false. No Full-suite or gameplay run is claimed.
