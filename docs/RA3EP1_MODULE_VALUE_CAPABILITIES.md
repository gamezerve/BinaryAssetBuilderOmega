# Reviewed GameObject module values and local IDs

<!-- Reborn: replace captured module literals with reusable schema/native contracts while retaining actual layout ownership proofs. -->

## Scope

The complete existing Conscript graph has 49 module instances in Draws,
Behaviors, AI, Body, ClientUpdates and ClientBehaviors. Its fixed module
attribute inventory now contains:

| Category | Editable instance/field slots |
| --- | ---: |
| Existing primitive attributes | 384 |
| Newly enabled enum attributes | 8 |
| Newly enabled fixed flag attributes | 20 |
| Total fixed-width attributes | 412 |
| Local module identity words, separately checked | 49 |

These are instance/field counts, not 412 different XML field names. The enum
and flag descriptors cover 23 distinct family/field combinations. Examples
include TracerModelDraw.WeaponSlotType, SpecialPower.ChangeWeather,
FreefallUpdate.ModelConditionsToSet, HitReactionBehavior.RequiredDeathTypes
and ModelConditionUpgrade flags.

The descriptor registry intersects inherited official schema attributes with
actual native fields and offsets, and only accepts already-reviewed metadata
wire identities. It does not synthesize a codec for an unknown native member.
Every descriptor is qualified against the complete captured stock token graph
before the changed source can produce a package. Omissions use the official
default or native zero, not the original Conscript's authored value.

Local IDs are no longer fixed to the original ModuleTag names. The complete
supported local scope must still have nonempty, case-insensitive hash-distinct
IDs. Explicit supported AssociatedModule links must resolve. This does not
rewrite arbitrary Lua or string-based references to renamed IDs, nor does it
claim a general EA Poid resolver.

Inheritance still matches repeated modules by their IDs. Giving a derived
module a different ID is not an automatic rename of its inherited counterpart;
it can mean an additional module under EA merge semantics. Edit the intended
module declaration/base or use the appropriate supported merge directive, and
do not interpret ID freedom as permission to change the final module count.

## Pipeline and implementation

- `ConscriptModuleScalarContract.cs`: schema/native discovery, enum/flag
  encodings, defaults, descriptor inventory, stock-token qualification and
  exact native value checking. The historical `module-scalars.json` filename
  now also contains enum and flag codec descriptions.
- `ConscriptModuleIdentityContract.cs`: source scope validation, comparison-only
  ID masking, expected header hashing and native/tokenized-tail validation.
- `ConscriptSourceInput.cs`: uses both contracts before structural comparison;
  actual authored XML is not replaced with captured names or values.
- `GameObjectConscriptTrialSmokeTest.cs`: grouped changed values plus all renamed
  IDs, native/metadata/stream proof and corruption/invalid-source controls.
- `BoundedDiagnosticBuild.cs`: rechecks values and module IDs in the final
  tokenized/native stream before publication.

Expected dependency tables used by the corruption matrix are cached only for
identical complete source XML inside one build with its fixed schema/catalog.
Changed sources miss that cache. No filename-only or cross-build cache is used.

## Remaining limits

- Module family, count, order and nested child/list topology still follow the
  supported complete Conscript graph. Arbitrary custom GameObjects are not
  yet production-enabled.
- Fixed flags accept ordinary enum names and `ALL`. Signed flag operations
  remain refused rather than silently passed to incompatible legacy parsing.
- Direct module strings, weak-reference values, nested enums/flags and dynamic
  lists are not enabled merely by adding these direct-attribute codecs.
- DisplayName changes, arbitrary module additions/removals, resource source
  compilation and arbitrary multi-asset GameObject graphs remain separate work.
- Schema-valid data is not necessarily sensible gameplay; these checks prove
  compilation/layout/linking, not the gameplay meaning of each chosen value.
- Use the existing alpha `EALAModStudio.exe`. No new user workflow, automatic
  command generation, installation or game launch is introduced.

## Verification — October 20, 2026

**10/10 focused live cases passed**: read-only reproduction of user Reborn;
combined local IDs, enums, flags, weapon retargeting, root import removal,
dynamic string and physics/AI edits; invalid enum/flag/duplicate-ID rejection;
three invalid-reference cases retaining the good package; restored developer
source build and explicit recruitment consumer. User Reborn sources/output were
unchanged and the developer sources were restored.

Every normal Conscript build also runs the grouped 412-attribute native/token
proof with all 49 local IDs renamed, per-field and per-header corruption
controls, malformed enum/flag checks and duplicate/empty ID refusal. Source
inheritance/fragment regression passed separately. Compiler build completed
with zero warnings/errors, and the live alpha compiler matches that built DLL.
The restored developer BIG retains SHA256
`87241958D0EC4CDB3A133FFC837E023A01B51716924F6B532A30BCBDE7EDF918`.

Evidence: `alpha/.work/dev-sdk-test.json`, `test-RebornSourceMirror.log`,
`test-ConscriptModuleValuesCombined.log` and `test-RestoredDeveloperSource.log`.
No game was launched. These results do not constitute general GameObject
topology support or new gameplay validation.
