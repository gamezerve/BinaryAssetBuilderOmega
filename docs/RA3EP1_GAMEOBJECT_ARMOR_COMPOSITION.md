# ArmorSet in the combined GameObject projection

October 15, 2026. Ordered ArmorSet native records and generated metadata now join
the reviewed root/destruction/ActiveBody graph. A second gate consumes a genuinely
new Core-compiled armor, not merely a stock name or synthetic dependency record.

## Production writer changes

- `Ep1GameObjectStringTokens.Inspect`: root count/items at 420/424 admit 0..32
  twelve-byte ArmorSet records before Behaviors. Nonempty lists require the root
  relocation at 424; absent lists require a null pointer. String allocations may
  not overlap armor records, even if the byte spans are identical.
- Native members are Armor import at +0, optional DamageFX import at +4 and
  Conditions at +8. Only nonzero reference words own IMP slots. Condition bits
  above the reviewed 23-bit mask refuse; other pre-behavior imports still refuse.
- `Ep1GameObjectDestructionProjection.Compose`: a present armor list receives an
  indirect root token (5E5AF575) and a generated count/marker/member graph of
  **12 + 60*N bytes**, with independently stock-audited field types 34E9127B,
  9A95EDFF and ED8BC8E9. No stock prefix is copied by the writer.
- Pointer values and RELO/IMP locations shift with the combined prefix; all
  reference indices stay in the original shared Core occurrence table.

With both Body and nonempty ArmorSet, the partial root has **89 fields**. Existing
Body-only and neither-present paths retain their 88/87-field contracts. An absent
armor list does not acquire an empty metadata table.

## Independent tests

`gameobject-armor-composition-self-test <static> <global>` runs sixteen cases:
0/1/2/32 armors, DamageFX absent/present, root EditorName string absent/present.
Raw XML validates in the reviewed complete-container schema candidate, followed
by actual Core reference normalization in a focused envelope. Copied armor member
attribute type/default/fixed/use declarations are checked against that candidate.
Shield/hook qualifications remain unchanged.

The named table combines root icon/intro, repeated armor/DamageFX, destruction FX,
body healing FX and a body range modifier. Complete native/RELO/IMP inverse shifting,
per-occurrence type/instance identities, literal condition values and deterministic
composed output are checked. Two independent stock object captures corroborate
the root/count/member metadata contracts.

Thirteen input/owner/source faults and thirteen detached metadata corruptions
are rejected in stock mode. The first authored test exposed reversed ArmorSet
order in its diagnostic DOM adapter: inserting each member before the first root
child reversed the sequence. Insertion now uses the fixed Behaviors anchor;
literal condition and occurrence-index checks caught the defect without changing
the native writer or relaxing validation.

## Newly authored armor consumer

`gameobject-authored-armor-composition-self-test <absolute-fresh-output> <static> <global>`
creates an owned source and uses the existing real Core armor build:

- New identity: ArmorTemplate:RebornComposedArmor, GUN Percent=250.5.
- Independently read published linked armor manifest; no collision with stock.
- The same sixteen combined consumer cases select this newly compiled identity.
- Removing the authored catalog makes the same consumer fail, proving it did not
  silently resolve a stock fallback. Authored mode has fourteen refusal cases.

This command publishes **the armor** through its existing scoped build/package
route. It does **not** publish the consuming GameObject manifest/BIG, establish a
mixed production stream or show gameplay. The command deliberately requires an
absolute fresh evidence path; relative/occupied/redirected paths are not relaxed.

## Files and next work

- `Ep1GameObjectStringTokens.cs`, `Ep1GameObjectDestructionProjection.cs`
- `GameObjectArmorCompositionSmokeTest.cs`
- `GameObjectBodyProjectionSmokeTest.Compile`: direct armor children no longer
  make it mistake the first root child for Behaviors.
- `tests/fixtures/RootArmorBodyPipeline.xsd`
- Two additional Focused groups, bringing the batch to 35.

WeaponSetUpdate/other required modules, remaining root/container graphs, generic
source preparation and a complete GameObject processor/publication path remain
open. The alpha and installation are unchanged. The next integration must not
assume KW weapon/locomotor layouts match EP1 merely because native class names
are already present.

## Final regression

`artifacts/GameObjectArmorComposition-a/batch-report.json`: **35/35 Focused groups
pass in 136.888 seconds**. Stock armor composition takes 5.950 seconds and the
newly authored armor consumer group 6.470 seconds. The incremental build passes
in 1.70 seconds with zero warnings/errors. All eight runtime hashes remain
unchanged; GameExecuted and InstalledFilesModified are false. No Full-suite or
gameplay result and no complete consuming GameObject package are claimed.
