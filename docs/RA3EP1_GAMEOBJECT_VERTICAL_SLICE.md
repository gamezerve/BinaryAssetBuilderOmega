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

## Next implementation gates

1. Prepare source-backed GameObject XML with official EP1 schema/defaults and
   real Core strong/weak reference normalization. Resolve stock references and
   newly authored armor IDs without using captured numeric dependency indices.
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
