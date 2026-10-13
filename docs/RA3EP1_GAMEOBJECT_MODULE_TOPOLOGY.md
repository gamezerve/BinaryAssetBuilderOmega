# GameObject module topology and inline AI correction

## Outcome

The previous +484 slot is resolved, not discarded. Official
AssetTypeGameObject.xsd declares AI as AIModule with xas:byValue=true, and
AIModule's choice allows zero to two modules. In both reviewed stock objects,
the AI list token's Count anchor is exactly native root +480. Native +484
points to the array of module pointers. The existing KW-derived model had
misread those eight bytes as a single UpdateModuleData pointer followed by a
PerUnitFX pointer.

GameObject.cs now declares inline PolymorphicList<UpdateModuleData> AI.
Marshaler.GameObject.cs marshals the inline container and rejects more than two
children. The AI dispatcher rejects unknown TypeIds instead of silently leaving
a null module. Body stays at +488 and the root stays 600 bytes. No official
schema or exported alpha h file is rewritten; general GameObject publication
remains disabled. The unused legacy PerUnitFX type itself is not broadly removed.

## Stock topology

GameObjectModuleReview.cs inspects six containers through the bounded stock
capture. It joins list Count tokens, native list/item pointers, marker tables,
concrete class wrappers, inherited class-table chains and terminal ModuleData.id
at native +4. Native concrete TypeIds must equal the wrapper class hashes.
Pointer aliasing, class-chain cycles, excessive counts and cross-region tables
reject. Body uses one concrete wrapper; AI uses an inline list; the other four
containers use pointer-to-list storage.

| Container | Conscript | Allied Construction Yard |
| --- | ---: | ---: |
| Draws | 4 | 5 |
| Behaviors | 36 | 32 |
| AI | 1 | 1 |
| Body | 1 | 1 |
| ClientUpdates | 1 | 0 |
| ClientBehaviors | 6 | 5 |

This covers 49 + 44 = 93 module instances, not 93 completely implemented
processors. The absent Allied ClientUpdates container has root-token target
zero and a null native pointer; it is not a table at offset zero.

Class names are identified from the existing explicitly named, fingerprint-pinned
in-memory diagnostic-shield-first-v1 / diagnostic-dummy-hooks-v1 schema candidate.
Its known duplicate-Shield normalization and reviewed warnings are not hidden
or applied to official files. It is used as a name catalog, not as authority to
publish all admitted asset types. NativeModelPresent is only an exact native
type-name match, not complete native layout, marshaler or gameplay proof.

At this topology milestone, nine schema-named families had no matching native
model. The subsequent [basic module batch](RA3EP1_BASIC_MODULE_PORTS.md) implements
ObjectSMCHelper, StructureSellUpdate and LargeGroupAudioClientUpdate with independent
payload/dispatch evidence. Six listed families still lack matching models:

- ObjectSMCHelperModuleData (a required official BehaviorModules member)
- SecondaryDamageTrackerModuleData
- FreefallUpdateModuleData
- ToggleStatusSpecialAbilityUpdateModuleData
- LargeGroupAudioClientUpdateModuleData
- StructureSellUpdateModuleData
- BuildingDestructionModuleData
- TechUpgradeModuleData
- LUAEventListBehaviorModuleData

Related differently named legacy classes are not automatically valid replacements.
Port these from official fields plus independent native/token evidence, in a
batch, before enabling a complete stock-derived object. Present models also need
field/default/dependency/class-dispatch review.

## Real AI marshaler controls

The diagnostic uses internally normalized TypeId input for AIGateUpdate, not
user-source TypeId authoring or full schema validation. The real root marshaler
produces:

| AI modules | Native BIN / RELO / IMP |
| --- | --- |
| 0 | 600 / 0 / 0 |
| 1 | 620 / 12 / 0 |
| 2 | 640 / 16 / 0 |

Count/Items bind at +480/+484. With modules, Items points to the array at 600;
each element points to its independently allocated concrete module. The test
checks every relocation (484, array slots, terminator), class ID, distinct weak
module ID and TriggerWidthX=12. These raw chunk dimensions are not linked-stream
header dimensions. Three modules and an unknown class refuse. Body remains null
in this deliberately incomplete diagnostic; it is not a playable schema-valid unit.

Thirteen detached stock corruptions cover count-anchor mismatch, bad marker,
native class-ID mismatch, inherited-table cycles, bad item pointers, excessive AI
count and the absent container's token/native null relationship.

## Evidence and limitations

Final focused matrix: artifacts/GameObjModules-d, 22/22 in 39.667 seconds;
incremental build 1.18 seconds, zero errors/warnings. All eight runtime hashes
remain unchanged during workers. The preceding functional run, GameObjModules-c,
also passed 22/22 in 41.731 seconds; its rebuilding compiler took 27.25 seconds
with zero errors and 56 existing warnings. Only comment-formatting changes
followed that functional run.

The first Full matrix, artifacts/GameObjModules-a, is NOT an all-pass result:
22/24 passed. Its broad compiler group passed (540.417 seconds), and expanded
armor fields passed. Its layout validator incorrectly used Marshal.SizeOf on a
generic unmanaged list, and its initial module-name lookup omitted schema-only
types. Later inspector/control fixes pass in the final focused run. The exact
SHA-256 hashes for all four shared Core/Utility/XmlCompiler/SageBinaryData
assemblies are identical between those runs; the native/marshaler changes were
already present in the successful broad group. The inspector assembly changed.
Do not describe this as a final 24/24 Full rerun.

An overlapping build attempt (GameObjModules-b) failed on the running apphost's
file lock and started no tests. Targeted iterations then used a separate owned
ModuleCandidateCompiler directory until the broad process completed. No process
was killed or installed files modified.

Next: compare actual complete module field/native graphs, beginning with required
missing helpers and AI/Body dispatch; recover recursive tokenization and auxiliary
streams; then bind a complete GameObject and new armor into a mixed Core stream.
No new GameObject mod, game launch, runtime observation or alpha capability
expansion is claimed by this milestone.

## Command

```powershell
# Reborn: bounded stock module topology inspection; no package, install or launch.
& .\source\BinaryAssetBuilder.ManifestInspector\bin\x86\Release\net8.0\BinaryAssetBuilder.ManifestInspector.exe `
  gameobject-module-review 'C:\YOUR\UNPACKED\data\static.manifest' `
  'GameObject:AlliedConstructionYard'
```
