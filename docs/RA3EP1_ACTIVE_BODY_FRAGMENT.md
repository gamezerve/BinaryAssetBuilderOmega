# EP1 ActiveBody native port and token fragment

Follow-up: [root/body/destruction composition](RA3EP1_GAMEOBJECT_BODY_COMPOSITION.md)
now connects this fragment through an explicit EP1 allocator. The standalone
milestone and its original evidence/limitations are preserved below.

October 15, 2026. A separate **76-byte EP1 ActiveBody model**, marshaler overload
and generated standalone token fragment now reproduce two selected stock bodies.
This is a new writer milestone, not merely another source inventory. It does not
yet join Body into the existing partial GameObject projection or enable alpha.

## Why the old model cannot be reused

The existing KW ActiveBody model has MaxHealthDamaged/MaxHealthReallyDamaged,
RecoveryTime, GrabFX/GrabDamage, GrabOffset, DamageCreation and other old fields.
The official EP1 schema instead has percentage-based thresholds and an ordered
DamageRangeModifier list. A shared class name/concrete ID is not ABI compatibility.

`Ep1ActiveBodyModuleData` preserves the KW model and its derived classes/dispatch.
It has the eight-byte Body/Behavior/Module base, native fields at these offsets:

| Offset | Field |
| ---: | --- |
| 8 / 12 / 16 | MaxHealth / MaxHealthDamagedPercentage / MaxHealthReallyDamagedPercentage |
| 20 / 24 | InitialHealth / DodgePercent |
| 28 / 32 | EnteringDamagedTransitionTime / EnteringReallyDamagedTransitionTime |
| 36 | GrabObject eight-byte string header |
| 44 / 48 / 52 | HealingBuffFX / DamagedAttributeModifier / ReallyDamagedAttributeModifier |
| 56 / 60 | CheerRadius / BurningDeathFX |
| 64 / 68 | DamageRangeModifier count/items |
| 72 / 73 | RemoveUpgradesOnDeath / BurningDeathBehavior |

Each range child is 12 bytes: percentage Min, percentage Max, modifier import.
Native percentages divide the XML number by 100, including unsuffixed official
defaults; time values become seconds. This is different from copying float
threshold names or treating schema defaults as raw native values.

## Generated fragment contract

`Ep1ActiveBodyTokenizer.Tokenize` admits a canonical 76-byte root and 0..64
contiguous range children at offset 76. GrabObject is currently absent only;
other string allocation layouts are explicitly refused. The marshaller can
allocate its string, but that is **not** token-writer support for it.

The metadata prefix is **324 + 60*N bytes**. It generates the concrete wrapper,
BodyModuleData -> BehaviorModuleData -> ModuleData chain, 15 own fields, terminal
TypeId/id tokens, range count/markers and three fields per child. Native indices
stay unchanged. Only the validated list pointer and its RELO slot shift; import
slot locations shift without renumbering their values. Exact auxiliary membership,
class/list bounds, zero string storage, booleans and padding are checked.

The writer does not copy a stock prefix. The independent stock test constructs
expected comparison coordinates from captured metadata, outside production code.
Legacy Body dispatch still uses its old types; callers must opt into the new
model overload. No EP1 body registry or general polymorphic GameObject activation
is inferred from the standalone fragment.

## Evidence

- AlliedConstructionYard (health 4000) and SovietAntiInfantryInfantry (health 100):
  complete 76-byte native equality and complete 324-byte generated metadata-prefix
  equality, including inherited/default fields and the empty range list.
- Actual official Body/ActiveBody declarations are included directly by the
  focused Core schema; reference target stubs provide inheritance only. Raw body
  XML separately validates in the reviewed full GameObject schema candidate.
- Actual Core normalization of four parent reference occurrences plus 0/1/3/64
  range children, using real static-catalog FXList and AttributeModifier names.
  Each occurrence's type/instance identity and one-biased native index is checked.
- Explicit scalar/time/percentage/flag and child percentage assertions;
  repeated native BIN/RELO/IMP equality and independent fragment round trip.
- Eleven malformed native/schema input refusals; sixty detached own/base/list/
  child metadata corruptions refused by the independent output verifier.

The selected stock bodies have empty range lists and null strong fields. Nonempty
children/imports have authored schema/Core/native evidence, **not** independent
stock nonempty-child or gameplay proof. No native target payloads are compiled.

The initial standalone test omitted concrete ID initialization normally performed
by polymorphic allocation. The caller now explicitly initializes the reviewed
class ID before invoking the overload; the shared base marshaller remains unchanged.
Output mutation tests also exposed a missing-field dictionary exception; explicit
child-table ownership/count/lookup checks now report a controlled validation failure.

## Files and next gate

- `source/SageBinaryData/SageBinaryData/Modules/Ep1ActiveBody.cs`
- `source/BinaryAssetBuilder.XmlCompiler/Marshaler.Ep1ActiveBody.cs`
- `source/BinaryAssetBuilder.XmlCompiler/BinaryAssetBuilder/XmlCompiler/Ep1ActiveBodyTokenizer.cs`
- `source/BinaryAssetBuilder.ManifestInspector/Ep1ActiveBodySmokeTest.cs`
- `tests/fixtures/ActiveBodyPipeline.xsd`
- `ep1-active-body-self-test <static.manifest>`; independent Focused group 32.

Next: explicitly select EP1 Body dispatch and compose its allocation/import/token
ownership with the root and behavior graphs. The current destruction composer
still rejects a Body owner. General bodies/derived classes, full source closure,
complete GameObject publication, Studio alpha expansion and gameplay remain open.
Root projection metadata remains at 87 fields; this standalone writer does not
increase that count. No game is launched or installed files changed.

## Final regression

`artifacts/Ep1ActiveBody-a/batch-report.json`: **32/32 Focused groups pass in
87.900 seconds**. The new body group takes 9.319 seconds. The incremental batch
build passes in 1.17 seconds with zero warnings/errors; all eight runtime hashes
remain unchanged during workers. GameExecuted and InstalledFilesModified are
false. No new Full-suite result, body/root composition or gameplay is claimed.
