# GameObject armor-consumer vertical slice

## Current outcome — October 13, 2026

The existing EP1 GameObject native root is 600 bytes, with ArmorSet at offset
420 and its item pointer at 424. The new `GameObjectStockReview.cs` proves those
relationships against bounded real Uprising tokenized slices, then compiles
only the selected BuildTime/ArmorSet projection through the existing marshaler.
This is a diagnostic native subset, **not general GameObject source support**.
The exported h alpha, its copied compiler and package inventories are unchanged.

| Stock object | Slice bytes | Native root offset | Armor members | Diagnostic BIN / RELO / IMP |
| --- | ---: | ---: | ---: | --- |
| SovietAntiInfantryInfantry | 128188 | 88824 | 3 | 636 / 8 / 28 |
| AlliedConstructionYard | 51520 | 36504 | 2 | 624 / 8 / 20 |

Both root tables have 126 fields. KindOf, BuildTime and ArmorSet independently
anchor the root: 40-byte KindOf at +4, BuildTime at +140, native list at +420.
ArmorSet uses a count-delimited marker table followed immediately by contiguous
three-field member tables, not a list zero terminator. Each member maps Armor,
DamageFX and Conditions tokens to one ordered 12-byte native record. References
are one-based native indices into the manifest dependency list; zero is null.
The review resolves the selected reference names/type IDs using stock metadata.

Conscript has CryoBeamFrozenArmor, ShrinkRayEffectArmor and its normal
SovietAntiInfantryInfantryArmor. AlliedConstructionYard has CryoBeamFrozenArmor
and normal AlliedConYardArmor. Their condition masks remain distinct: 8192/16384/0
and 8192/0 respectively. BuildTime is 4 and 25. The masks are raw stock values,
not an assertion that all condition combinations are gameplay tested.

The compiled selected projection reproduces every stock ArmorSet record and
BuildTime exactly. Its item-pointer relocation at 424 and every Armor/DamageFX
import location are checked, including auxiliary-stream terminators. Captured
reference indices are deliberately supplied as diagnostic normalized input:
this does **not** prove source-name normalization, recursive dependency closure,
full native GameObject equality, complete tokenization, Core stream publication
or new-ID gameplay. No modules, models or textures are compiled here.

## Commands and boundaries

```powershell
# Reborn: bounded stock read and selected native projection; no package, installation or game launch.
& .\source\BinaryAssetBuilder.ManifestInspector\bin\x86\Release\net8.0\BinaryAssetBuilder.ManifestInspector.exe `
  gameobject-stock-review 'C:\YOUR\UNPACKED\data\static.manifest' 'GameObject:AlliedConstructionYard'
```

`gameobject-stock-self-test [manifest]` uses an owned synthetic three-member
graph and fifteen detached corruption refusals. With a manifest it also checks
the two stock objects above. The parser refuses wrong root/type tags, native
anchor drift, excessive/mismatched list counts, aliased/out-of-range table/item
pointers, wrong ordered field offsets, null/out-of-range/wrong-type references,
high condition bits and duplicate token hashes. Stock slices are bounded to one
MiB with at most 512 root tokens and 32 ArmorSet entries; review requires the
observed linked-v7/type-table fingerprint and matching stream prefix/checksum.
Only selected relationships are verified; no whole-stream checksum or malicious
race/security guarantee is inferred from a bounded slice/header check.

The first batch (`GameObjStock-a`) preserved eighteen existing passes but
rejected the new group because a list terminator had incorrectly been assumed.
The actual count/table relationship corrected that assumption. The subsequent
stock/native runs pass nineteen groups, including all existing armor/weapon
regressions. Final local evidence is under `artifacts/GameObjStock-final`.
The final run passes **19/19**, with unchanged hashes for all eight runtime
files, in approximately **33.78 seconds**, excluding the **4.68-second** compiler
build (zero warnings/errors). The new stock/native group takes about 0.33 seconds.
Full is not required for this inspector-only addition: shared schemas, native
structures, marshalers, tokenizer and Core/output policies were not changed.

## Named source-reference follow-up

`GameObjectArmorSourceProbe.cs` now admits a separate standalone XML projection:
explicit `id`, literal BuildTime 0..10000 and 1..32 direct ArmorSet members, with
plain Armor/DamageFX names and optional schema-valid Conditions. It validates
against a focused harness, invokes the actual Core reference normalizer, resolves
each typed handle in explicit linked EP1 manifest metadata, and checks each native
one-based index/import slot against the resulting dependency list. It never
injects captured stock dependency indices. Includes, inheritance, expressions,
modules and other GameObject fields are refused by this diagnostic route.

The self-test compiles `RebornAuthoredArmor` (GUN 250.5) through the real general
armor Core path, then resolves a consuming GameObject against that actual output
manifest plus stock metadata. Repeated references preserve independent ordered
slots. Repeat/native equality, changed-BuildTime native inequality and fourteen
source/schema/lookup refusals pass, including missing custom armor and duplicate
catalog identities. Caller settings are restored even on failure.

The first full official schema attempt failed on the known two nonidentical
`ShieldSphereUpdateModuleData` declarations. Reference schemas were not edited,
and the diagnostic Shield candidate was not silently applied. Instead, the
focused harness uses untouched primitive/base/reference/armor Includes and
copied ArmorTemplateSet, DamageFXRef and BuildTime declarations. Before every
probe, expanded element/attribute names and all declaration values are compared
against staged official `AssetTypeGameObject.xsd`; declaration drift rejects.
The reduced GameObject container itself is a diagnostic choice, not an official
complete type. Receipts explicitly report `OfficialSchemaValidated=false`,
`FocusedOfficialFieldSchemaValidated=true` and publication/game flags false.

Final evidence: `artifacts/GameObjSource-d`, **20/20** focused groups in **37.272
seconds**, plus a **5.31-second** compiler build with zero warnings/errors. All
eight runtime-file hashes remain unchanged during the matrix. Earlier failed
attempts remain under `GameObjSource-b` (full schema duplicate) and
`GameObjSource-c` (focused harness missing GameObject-local DamageFXRef); these
are failures, not capability evidence. Both checked-in consumer examples also
pass CLI probes, selecting stock and newly built armor respectively.

```powershell
# Reborn: diagnostic source-to-native reference check only; not an alpha mod build or playable GameObject.
& .\source\BinaryAssetBuilder.ManifestInspector\bin\x86\Release\net8.0\BinaryAssetBuilder.ManifestInspector.exe `
  gameobject-armor-source-probe .\samples\gameobject\ArmorConsumerProbe.xml `
  'C:\YOUR\UNPACKED\data\static.manifest'
# Reborn: one owned batch also builds a real new armor target and verifies the named consumer.
& .\source\BinaryAssetBuilder.ManifestInspector\bin\x86\Release\net8.0\BinaryAssetBuilder.ManifestInspector.exe `
  gameobject-armor-source-self-test 'C:\YOUR\FRESH\OUTPUT' `
  'C:\YOUR\UNPACKED\data\static.manifest'
```

For the custom consumer sample, pass the generated authored armor manifest as
the optional final argument. Identical native bytes for different reference
names are expected when their local dependency indices are identical; the typed
dependency list must be checked too. A native hash alone is not reference proof.
The generated armor package is an armor-only candidate, not a published
GameObject package. No full object token tree, mixed stream or gameplay is proven.

## Next implementation gates

1. Extend the now-proven focused source-name/armor lookup path to the complete
   admitted object graph, with explicit schema-candidate/default policy, weak
   references and source Includes/inheritance/definitions. This follow-up does
   not close full official GameObject validation or complete dependency closure.
2. Compare a small complete GameObject native graph against independent stock
   evidence; cover inherited/source-defined fields and the required module set.
   A minimal schema-valid graph is not automatically a playable unit.
3. Recover/encode the complete selected token tree, preserving module class
   chains, strings, lists and relocation/import locations. Compare unchanged
   stock output plus independent edited-field/reference controls.
4. Bind the document/plugin path to a separate scoped GameObject stream, then
   allow a bounded mixed GameObject/ArmorTemplate stream with dependency closure
   and stable IDs. Do not route it through the armor-only scoped factory.
5. Export to Studio with explicit unsupported-type/module errors. Author a new
   armor and a consuming object under user-chosen IDs; prepare an independently
   authorized gameplay test. Reusing stock art is separate from compiling new
   W3X/TGA/DDS assets.

This milestone establishes the consumer-link native foundation; it does not
silently enable GameObject XML in `Invoke-Ra3Ep1AlphaBuild.ps1`.
