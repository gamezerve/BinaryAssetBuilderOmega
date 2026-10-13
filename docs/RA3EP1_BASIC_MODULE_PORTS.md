# First EP1 GameObject module-port batch

## Implemented

Three previously missing native model families now have dedicated models,
marshalers and real GameObject polymorphic dispatch:

| Family | TypeId | Native size | Added fields after base |
| --- | --- | ---: | --- |
| ObjectSMCHelperModuleData | 7F72F242 | 8 | none; ObjectHelper base |
| StructureSellUpdateModuleData | 4E9CB811 | 16 | SellTime +8, ActiveModelCondition +12 |
| LargeGroupAudioClientUpdateModuleData | 9826A297 | 28 | frame counters +8/+12, Key count/items +16/+20, UnitWeight +24 |

Implementation files: SageBinaryData/Modules/Ep1BasicModules.cs,
Marshaler.Ep1BasicModules.cs, the behavior switch in Marshaler.GameObject.cs and
the client update dispatcher. Existing similarly named KW types are unchanged.
Unknown client classes now throw rather than silently leave null entries.

ObjectSMCHelper is marked absolutely required in EA's BehaviorModules comments.
It inherits ObjectHelperModuleData and declares no own fields. This is not an
approximation using another helper's concrete ID.

## Independent evidence

Ep1BasicModuleSmokeTest.cs captures the same fingerprint-admitted, bounded stock
Conscript and Allied Construction Yard slices as the topology review. It checks
exact own-field token counts, offsets and type tags, real root dispatch and
complete selected native payload equality:

- ObjectSMCHelper: both objects, all 8 bytes including concrete ID and weak ID.
- StructureSellUpdate: Allied yard, all 16 bytes.
- LargeGroupAudioClientUpdate: Conscript, all 28 bytes with its Key pointer
  canonically rebased, plus the actual ordered Key array bytes.

Only the audio Key pointer word is rebased, not arbitrary unknown payload.
Relocation slots and the absence of strong imports are verified independently.
Complete source GameObject equality, recursive module tokenization and output
stream publication are not inferred from these selected module checks.

Reference XML establishes the source values: BaseObjects/BaseObject.xml contains
ObjectSMCHelper id=ObjectSMCHelper; BaseObjects/BaseStructure.xml contains
StructureSellUpdate id=ModuleTag_StructureSellUpdate, SellTime=2.0s and SOLD;
Soviet/Units/SovietAntiInfantryInfantry.xml contains LargeGroupKey with
HumanInfantryWithGearThatRuns. Stock payloads independently match these literals.
Reference files were read only and are not bundled as full source fixtures.

The raw module leaves are validated inside a minimal GameObject/Body declaration
against the existing explicitly reviewed in-memory Shield/dummy-hook schema
candidate. This is not pristine full official-schema compilation. Native dispatch
then uses diagnostic internally injected TypeIds, not authored TypeId input or
proof that the whole source preprocessor is connected to GameObject publication.

## Client frame semantics and controls

The stock audio client stores frame counters 3 and 2 directly. The old Duration
marshaler would convert those through milliseconds/logic frames. The new client
therefore uses raw unsigned frame integers; the legacy behavior audio class is
not changed.

The first fractional refusal test discovered that the general legacy uint
marshaler casts fractional floats. A module-specific parser now accepts unsigned
integer literals and rejects fractional/suffixed syntax without changing that
legacy parser. The full lexical repertoire of SageDurationUnsignedInt is not
claimed implemented from its permissive string schema.

Controls cover repeated native/relocation equality, changed weak IDs, SellTime
2s to 4s, SOLD to DAMAGED, frame counters 17/9, UnitWeight 7 and two ordered keys.
Wrong own-field offsets, unknown raw attributes, fractional client frames and
unknown client classes reject. The models' sizes and field offsets are separately
pinned in layout-self-test.

All three new classes also compile together in one diagnostic root: two Behaviors
plus one ClientUpdate. The combined 684/36/0 raw BIN/RELO/IMP graph checks all eight
pointer relocations, class IDs, scalar values and the client Key. This is one
native graph, NOT a mixed Core asset stream, mod package or playable object.

## Final regression evidence

artifacts/BasicModules-b passes **25/25 Full groups** in **655.435 seconds**,
excluding the 47.67-second compiler build (zero errors, 56 existing warnings).
The broad compiler group passes in 601.090 seconds and the new basic-module
group in 17.278 seconds. All eight compiler/runtime hashes stay unchanged while
workers run. Existing armor/source/weapon/module-topology tests remain covered.

The preceding BasicModules-a is a failed 24/25 result: its fractional-frame
control detected the legacy uint conversion. The final run uses the dedicated
strict frame parser and includes the combined native graph. Targeted iteration
used a separate owned BasicModulesCandidateCompiler output while the first broad
process was active; no running compiler/apphost was overwritten or process killed.
No reference schema/XML or installed game files were changed.

## Remaining work

The two reviewed objects now have six missing native model families:
SecondaryDamageTracker, FreefallUpdate, ToggleStatusSpecialAbilityUpdate,
BuildingDestruction, TechUpgrade and LUAEventListBehavior. This is not a list of
all missing families across Uprising. Existing models also need complete field,
default, dependency and dispatch review.

Next, port related tracker/special-ability families in a batch, with their nested
effects/status lists and imports independently checked. Complete GameObject
tokenization, source graph/Core integration and mixed GameObject/Armor publication
remain gates. No game launch, installation mutation, gameplay proof or alpha h
capability expansion occurred in this batch.

## Command

```powershell
# Reborn: offline selected module/schema/native regression; no GameObject package or game launch.
& .\source\BinaryAssetBuilder.ManifestInspector\bin\x86\Release\net8.0\BinaryAssetBuilder.ManifestInspector.exe `
  ep1-basic-module-self-test 'C:\YOUR\UNPACKED\data\static.manifest'
```
