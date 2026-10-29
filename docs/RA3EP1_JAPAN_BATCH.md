# Japan XML batch

<!-- Reborn: retain the complete requested batch and its offline validation scope. -->
All **46 distinct requested XMLs** compile together. The repeated
`JapanWarFactoryEgg` entry is included once. Input selection uses the Reborn
source when present, otherwise the original Uprising XML under `alpha/SageXML`.
An isolated source copy excludes ART Includes; this checks complete object and
supporting-data compilation, without recompiling original model resources.

The source graph spans **86 files** and emits **54 GameObjects**, including
secondary forms, the ambush object and Fortress Ship cannon nodes. Supporting
declarations add **3 ArmorTemplates, 4 LocomotorTemplates, 1 AttributeModifier
and 6 ObjectCreationLists**, for **68 assets**. All **13 local / 6119 stock**
reference occurrences resolved. BIN/RELO/IMP readback passed with stream checksum
`54425154`. The complete measured compile/link/private-package/readback run took
**90.19 seconds**.

## Added support

Nine behavior families now use complete EP1 source/native/metadata processing:

- `ChargeWeaponUpdate`
- `PrematureDischargeSpecialPower`
- `SpiderHoleUpdate`
- `ActivateSpecialPowerOnCreate`
- `MovementSpecialPower`
- `TimedOpacityUpdate`
- `MonitorConditionUpdate`
- `UnitUnpackUpdate`
- `LifetimeUpdate`

Populated Physics fields and Sentinel's optional LinearDamageUpdate Locomotor
reference are qualified against original Uprising records. WeaponSetUpdate now
supports hierarchical turrets, their ParentID and populated turret vision/filter
lists. Mecha King's six hierarchical turrets are independently qualified using
its original **WorldBuilder** record; this object has no static-stream witness.
Ordinary turret-only builds do not require the hierarchical WorldBuilder check.
Contained weapon slots remain unsupported.

Production definition expansion now handles Egg KindOf expressions such as
`=$EGG_KINDOFS+' SKIRMISH_AI_DONT_BUILD'` through the existing checked string
evaluator. Undefined symbols still fail; older literal-only profiles retain
their existing behavior. Sentinel's original reversed sequence children are
normalized only in the stock witness copy, matching production preparation.

## Evidence

The checked-in `tests/fixtures/japan-batch/data/Static_Mod.xml` records all 46
explicit source paths. Private evidence is retained under
`alpha/.work/japan-batch-b036275aa49d40a3906959889e509d86/`:

- `verification.json`: requested/emitted identity coverage, stream hashes and results.
- `prepared4/`: prepared source graph and captured input receipts.
- `native7.log`, `native7/stream-readback/`: successful complete stream build.
- `modules8.log`: **14 generated/source-expression checks and 14 corrupt/undefined
  rejection checks**, including original native behavior, turret vision and hierarchy.
- `local-defines.log`, `definition-subset.log`: older definition profile regressions.

The alpha compiler tools are synchronized and their nine recorded hashes match.
No user Reborn source or published mod was changed, and no game was launched.
These results prove offline compilation and linkage; gameplay and newly authored
ART resources require their own validation.
