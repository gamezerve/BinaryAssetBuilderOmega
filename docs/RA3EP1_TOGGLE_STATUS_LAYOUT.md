# EP1 toggle-status layout checkpoint

This is a read-only stock layout prerequisite, **not a new compiler capability**.
The existing alpha h package and native module dispatch are unchanged. The four
missing native model families observed in the two reviewed objects remain four.

## Evidence and scope

`ToggleStatusLayoutReview.cs` selects Conscript's
`ToggleStatusSpecialAbilityUpdateModuleData` through the reviewed module topology
and reuses the bounded, fingerprint-admitted stock capture. No full binary dump,
game launch, stock edit, or package installation occurs.

The existing `SpecialAbilityUpdateModuleData` model is **252 bytes**. All 47
own field tokens correspond to current reflected offsets: 46 direct native
addresses and the absent `CustomAnimAndDuration` pointer (+244). This proves
name/offset coverage for this stock instance, not base marshaler values,
reference identities, enum ordinals, defaults, or other games' layouts.

The toggle module has a count/items list at +252/+256 and packed booleans at
+260/+261; a 264-byte concrete native model is the next implementation candidate.
There are two ordered **304-byte** toggle-state records. Their eleven direct
field anchors match both records. `SkirmishAiInfo` begins inline at state +132;
its `StateWeapon` pointer is at AI +144 (state +276), **not inline weapon data**.
Each present weapon record has ten field anchors and a 32-byte layout candidate.
AI has 33 schema-named field tokens, including 13 absent optionals. Module and
ActiveGeometry child lists are empty at state +288/+296.

Every nested table's field-name set is checked against the staged official XSD:

- `Modules/ToggleStatusSpecialAbilityUpdate.xsd`
- `SkirmishAI/AssetTypeAISpecialPower.xsd`

This name-set check does not compile or admit a full schema set. Non-null direct
AI/weapon field anchors, native pointer bounds, empty-list headers and zero
candidate optional slots are checked. The output labels unimplemented payload
semantics explicitly. Absent optional tokens cannot independently prove their
present-value storage mappings; present optional fixtures are required.

The official source `Soviet/Units/SovietAntiInfantryInfantry.xml` identifies
`ModuleTag_ToggleMolotovCocktail`, `SpecialPower_MolotovCocktail`, PackTime 1s,
RECONSTITUTE_STORED_COMMAND, AK-47/default and Molotov/weapon states, with
GENERIC_TOGGLE_STATE/ANTI_GARRISON, USER_5 and the Molotov-entry sound.
These source names have **not yet** been normalized and compared to native
reference identities in this checkpoint.

## Regression and commands

```text
BinaryAssetBuilder.ManifestInspector.exe toggle-status-layout-review <static.manifest>
BinaryAssetBuilder.ManifestInspector.exe toggle-status-layout-self-test <static.manifest>
```

The self-test rejects nine detached corruptions: list count, native item pointer,
base-field offset, concrete boolean offset, state-table target, nested state-field
offset, weapon pointer, empty child-list count and candidate optional storage.
Original stock bytes remain untouched. The gate is included in
`scripts/Test-Ra3Ep1Batch.ps1` as `toggle-status-layout`.

Final evidence: `artifacts/ToggleLayout-a/batch-report.json`, **25/25 focused
groups passed in 55.667 seconds** (excluding the 24.63-second, zero-warning/error
batch build). All eight runtime-file hashes remained unchanged during workers.
The initial dependency rebuild reported 111 existing warnings and zero errors;
the final batch build was clean. Full compiler tests were not rerun for this
inspector-only change; the preceding damage/motion milestone retains its 26/26
Full evidence. Do not mistake focused results for a new Full run.

Next present-value source fixtures have been located but not yet decoded:
`Soviet/Units/SovietMiner.xml` has 2s look-ahead/look-behind and 0.2 damage fraction;
`Soviet/Units/SovietScoutVehicle.xml` has power-advantage 1.0 with RETREAT/REPAIRING
restrictions. These provide specific stock candidates to resolve optional layouts.

## Next implementation gates

1. Implement layout-proven native toggle/state/AI/weapon structures; acquire
   independent stock fixtures for present optional AI values and nonempty lists.
2. Validate special-ability enum/flag ordering and actual base marshaler output,
   not only reflected offsets. Preserve optionality rather than invent defaults.
3. Normalize strong/weak/module references through Core; resolve identities and
   compare relocations/imports and complete selected native graphs.
4. Test changed states, order/count, literals, absent children and invalid inputs
   together. Then enable real polymorphic dispatch.
5. Recursive GameObject tokenization, remaining module payloads and mixed-stream
   publication still precede general GameObject authoring in Studio.
