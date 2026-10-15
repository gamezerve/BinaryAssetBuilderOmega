# EP1 WeaponSetUpdate hardpoint native port and token writer

October 15, 2026. A separate bounded EP1 weapon-set implementation now reproduces
the complete selected Peacekeeper and Conscript hardpoint graphs, including
generated metadata and actual Core weapon references. It does not replace the
legacy KW dispatch or yet join the combined GameObject's behavior list.

## Recovered native contracts

| Record | Size | Important EP1 fields |
| --- | ---: | --- |
| Ep1WeaponSetUpdateModuleData | 44 | Update base +0, ActiveWhenDisabledBy +8, hardpoint count/items +12/+16, three unsupported empty list headers +20..43 |
| Ep1WeaponSlotHardpoint | 28 | TypeId +0, ID +4, InterleavedStyle +8, choice +12, Weapon count/items +16/+20, AllowInterleavedFiring +24, AutoChooseWeapon +25 |
| Ep1WeaponSlotWeaponData | 80 | Ordering +0, Template +4, Upgrade +8, ObjectStatus +12 (32 bytes), ForbiddenObjectStatus +44 (32 bytes), permanent/metrics flags +76/+77 |

The KW models lack the forbidden-status mask, AutoChooseWeapon,
ConquerMetricsAlwaysCount, root disabled flags and contained list header. Reusing
them would lose the Conscript's AK-47/Molotov exclusion relationship even though
the class name already exists in the repository.

The polymorphic module root has concrete ID **431722A9**. In contrast, both selected
stock graphs have a **zero** TypeId word in each inline statically typed hardpoint.
The first comparison caught an incorrect class-hash initialization at bytes 44..47;
the EP1 inline marshaler now preserves the independently observed zero contract.
This is not a captured reference/address correction. The legacy classes remain
unchanged.

## Writer and admission

`Ep1WeaponSetTokenizer.Tokenize` admits 0..8 hardpoints with 0..16 already-ordered
weapon records each. Turret/hierarchical/contained headers must all be zero;
nonempty source instances of those families fail explicitly in the new marshaler.
No approximate hardpoint substitute or legacy fallback is used.

Canonical native storage consists of the 44-byte root, all 28-byte hardpoint
headers, then each ordered 80-byte weapon array. Missing/duplicate auxiliary
ownership, truncated records, wrong class/inline ID, invalid enums/booleans/padding,
unsorted Ordering and noncanonical pointers are refused.

For H hardpoints and W total weapons, the generated metadata prefix is
**252 + 120*H + 108*W bytes**. It contains the concrete module wrapper, all four
inherited bases (including ContainModuleData), native count bindings for all four
slot families, hardpoint and weapon markers, own fields and terminal identities.
The writer copies no stock metadata. Reference values stay unchanged; validated
native pointers and RELO/IMP slot locations shift once by the prefix.

## Independent evidence

- Peacekeeper: complete selected native/token metadata sizes **152/480 bytes**.
- Conscript: complete selected native/token metadata sizes **232/588 bytes**,
  covering AK-47 forbidden GENERIC_TOGGLE_STATE and Molotov required status.
- Actual Core normalization resolves the authored weapon names, and independent
  stock dependency tuples corroborate every normalized index before complete
  native equality. Only known native pointer words are rebased for comparison.
- Complete generated metadata prefixes match independently captured stock
  coordinates, not a prefix borrowed by the production writer.
- Raw source validates in the reviewed full GameObject candidate; the focused
  Core schema includes the official weapon-set declarations directly and checks
  the four base inheritance links/attribute counts. Target and TurretAIData stubs
  establish schema relationships only, never payload support.
- Five authored (hardpoints, weapons per hardpoint) shapes: (0,0), (1,0), (1,1),
  (2,3), (8,16). Real weapon and upgrade targets, repeated occurrences, explicit
  permanent/metrics and hardpoint choice flags, output repeat and complete
  native/RELO/IMP round trips are checked.
- Sixteen malformed source/native refusals and 54 detached metadata corruptions
  are rejected by separate producer/admission and output verification paths.

Only the two selected stock hardpoint graphs have full stock equality. Multi-
hardpoint/large-list cases have authored schema/Core/native/writer evidence,
not independent multi-hardpoint stock or gameplay acceptance. The diagnostic
marshaler requires pre-normalized source order rather than silently sorting.

## Files and remaining gate

- `source/SageBinaryData/SageBinaryData/Modules/Ep1WeaponSet.cs`
- `source/BinaryAssetBuilder.XmlCompiler/Marshaler.Ep1WeaponSet.cs`
- `source/BinaryAssetBuilder.XmlCompiler/BinaryAssetBuilder/XmlCompiler/Ep1WeaponSetTokenizer.cs`
- `source/BinaryAssetBuilder.ManifestInspector/Ep1WeaponSetSmokeTest.cs`
- `tests/fixtures/WeaponSetPipeline.xsd`
- `ep1-weapon-set-self-test <static.manifest> <global.manifest>`; Focused group 36.

Next: explicit EP1 module allocation and a multi-behavior root composition that
can combine destruction and this weapon module, sharing armor/body/root reference
indices. General turrets/contained slots, full source preparation, complete
GameObject processing/publication, alpha integration and gameplay remain open.
This standalone writer does not change the 89-field combined root contract.

## Final regression

`artifacts/Ep1WeaponSet-a/batch-report.json`: **36/36 Focused groups pass in
143.162 seconds**; weapon-set group 9.575 seconds. The batch build succeeds in
1.71 seconds with zero warnings/errors. All eight runtime hashes remain unchanged
while workers run; GameExecuted and InstalledFilesModified are false. No new
Full-suite run, complete GameObject package or gameplay result is claimed.
