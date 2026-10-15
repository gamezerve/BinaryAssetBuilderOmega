# GameObject enum, inline flag and weak-ID root batch

Follow-up: the [string/ownership batch](RA3EP1_GAMEOBJECT_ROOT_STRINGS.md) adds six
headers and dynamic allocation decoding, extending the 78-field checkpoint below
to 84 represented fields. Strong imports and general publication remain closed.

## Outcome and native corrections

15 extra root descriptors extend the prior 62 scalars: six enum fields, two
40-byte KindOf masks and seven weak IDs. With Behaviors, the projection now emits
**78 root token fields**. This is a metadata count, not SDK completion or asset
coverage. Strong references, strings and other indirect graphs remain omitted;
general GameObject streams, registry/Studio publication and alpha are unchanged.

The audit found real legacy enum incompatibilities, corrected with GameObject-
specific EP1 types rather than changing the older shared KW enum declarations:

- RadarPriority: official forceValue rules require NOT_ON_RADAR=-2, INVALID=-1,
  STRUCTURE=0, UNIT=1 and COMMANDO=2. The old type used incompatible names/values.
  The omitted GameObject RadarPriority now explicitly compiles to INVALID=-1,
  matching the official default instead of zero/STRUCTURE.
- EditorSorting: EP1 adds DESTRUCTIBLE_PROPS, OPTIMIZED_PROPS, CAMPAIGN_UNITS and
  CAMPAIGN_STRUCTURES after the shared legacy prefix.
- ProductionQueue: stock MAIN_STRUCTURE=0 and WATERCRAFT=6 corroborate the engine
  INVALID=-1 convention; WATERCRAFT replaces unsupported legacy queue names in
  the GameObject-specific type. XSD zero-based ordinals alone would be wrong.
- Existing BuildPlacementType, UnitCategory and WeaponCategory are retained after
  explicit name/value-policy checks. Unit/weapon NONE=-1 is not guessed from XSD
  declaration position. Selected stock values independently pin the engine policy.

## Tests

`GameObjectRootValueSmokeTest` audits all 15 descriptors against AlliedConstructionYard,
SovietAntiInfantryInfantry and SovietAntiStructureShip. Selected native enum values
are pinned independently, including ship WATERCRAFT=6, VEHICLE=2 and CANNON=2.
These are resolved stock values; this test does not implement source inheritance.

All **50 enum literals** pass raw XML schema validation, native dispatch/value
comparison and generated token/projection round trips. The test uses explicit
schema forceValue annotations plus independently corroborated sentinel policies,
not `Enum.GetNames` sorting as a numerical oracle. Invalid enum strings fail.

All **291 KindOf names and numeric bit positions** must match the official schema.
Six selected bits cross word boundaries (0/31/32/63/64/last) in both inline masks;
every one of their ten native words is checked. Unknown flag names fail schema
validation. Full flag-name ordering is checked, but not every possible mask or
its runtime gameplay semantics.

## Actual Core weak identities

`RootWeakPipeline.xsd` directly includes official base/reference declarations.
Seven focused attribute types/defaults are compared to the reviewed GameObject
schema before use. PlayerTemplate/LogicCommandSet/PackedTextureImage/HealthBarTemplate
stubs establish only schema inheritance, not payload processors or target assets.

Core infers target types from the real XSD annotations, records seven weak
references and zero strong references, and native compilation must hash the same
invariant-lowercase instance identities without generating imports. Side,
CommandSet, portraits/buttons and HealthBar are checked. Four explicit official
Allied source names also match their real stock weak-ID words. Transformed image
and health-bar probe names are synthetic and have no stock target-existence proof.

Official WeakReference lexical rules reject authored `Type:Name` strings in this
fixture. The initial prefixed positive fixture was corrected to plain names;
the official schemas were not weakened. The negative prefixed case is a schema
refusal, not proof that a schema-invalid string reached Core's type checker.

**Weak identity normalization is not target existence/dependency closure.** Missing
synthetic weak names are not incorrectly claimed resolved, and no texture/health-
bar payload compiler is admitted. Full source preprocessing remains separate.

## Files and next gates

- `source/SageBinaryData/SageBinaryData/GameObject/Ep1RootEnums.cs`
- `source/SageBinaryData/SageBinaryData/GameObject.cs` / GameObject marshaler:
  scoped enum types and corrected radar default.
- `source/BinaryAssetBuilder.XmlCompiler/BinaryAssetBuilder/XmlCompiler/Ep1GameObjectRootValueTokens.cs`
- `source/BinaryAssetBuilder.ManifestInspector/GameObjectRootValueSmokeTest.cs`
- `tests/fixtures/RootWeakPipeline.xsd`

The combined 77 value descriptors now have **154** independent type/target
metadata corruption refusals, retaining prior root/module/input controls.
Tests use the explicit reviewed shield-first/hook schema candidate, not an
unmodified clean full official schema claim. Next gates include actual string
allocation/ownership, strong root references and indirect root/module writers
before full GameObject stream publication. No new game test is performed here.

## Final regression

`artifacts/GameObjectRootValues-a/batch-report.json`: **30/30 Focused groups pass**
in **81.046 seconds**. The root/native/source family checks take **6.748 seconds**;
actual Core/destruction/projection checks **3.512 seconds**. Selected composed
BIN/RELO/IMP is **2080/24/8**, including a 1356-byte partial token prefix.
All 154 root-value metadata corruptions and prior refusal controls pass. The
batch incremental build takes 1.17 seconds with zero warnings/errors. Eight
runtime hashes remain unchanged during workers. No game runs or installation
files change; no new Full-suite, complete GameObject stream or alpha release is
claimed. The older shared KW enum declarations are unchanged.
