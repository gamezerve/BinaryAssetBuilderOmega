# EP1 standalone toggle AI native implementation

`AIToggleSpecialPowerInfo` and `AISpecialPowerWeaponData` now have native models
and marshalers. This is real standalone compilation, **not yet enclosing
ToggleStatusSpecialAbilityUpdate dispatch or GameObject publication**. Alpha h
is unchanged; the four observed missing module families remain four.

## Files

- `source/SageBinaryData/SageBinaryData/Ep1ToggleAi.cs`
- `source/BinaryAssetBuilder.XmlCompiler/Marshaler.Ep1ToggleAi.cs`
- `source/BinaryAssetBuilder.ManifestInspector/Ep1ToggleAiSmokeTest.cs`
- `source/BinaryAssetBuilder.ManifestInspector/ToggleStatusLayoutReview.cs`
- `tests/fixtures/ToggleAiPipeline.xsd`

## Implemented layout and behavior

The AI record is **156 bytes**, with thirteen optional four-byte pointer slots,
inline scalar/mask/reference fields, the optional weapon pointer at +144 and
five packed booleans at +148..152. The weapon's ten declared fields occupy
**28 bytes**, with four packed booleans at +24..27. The prior layout report's
32-byte weapon-size candidate is corrected: allocation spacing alone does not
establish the size of the declared structure.

The marshalers preserve absent pointers as null, and present literal zero as a
non-null pointer to zero. Named enums and enum-index bit masks reject unknown,
numeric, missing required or multiple scalar tokens. Official declaration order
is checked for toggle hints, micromanager states/tasks, target location and
weapon-location/emanation enums. Existing typed weak-ID and strong-reference
marshalers are reused; IDs use invariant hashing.

Native pointer layouts are stock-corroborated for four present optional fields:

| Field | AI pointer slot | Source value |
| --- | ---: | --- |
| UseOnlyWhenPowerAdvantageIsAbove | +4 | 1.0 |
| SelfDefenseDamageLookAheadTime | +44 | 2s |
| SelfDefenseDamageLookBehindTime | +48 | 2s |
| SelfDefenseDamageFraction | +52 | 0.2 |

The remaining nine optional fields have independent authored allocation/value
controls, but their present layouts **are not yet corroborated by stock**.
The report does not claim all thirteen present layouts are verified.

## Stock/source/reference evidence

Six selected AI graphs are compiled from literal source XML and compared with
actual linked stock data: both toggle states from Conscript, Ore Collector and
Terror Drone. The complete 156-byte AI and present 28-byte weapon records match
after rebasing only independently checked pointer/reference words. Pointed-to
values match exactly. These fixtures include absent weapon children and optional
values, present weapons, RETREAT/REPAIRING masks and default scalar/bool values.

Plain AK-47, Molotov and paralysis-weapon names are normalized through the actual
Core reference handler. Each dependency resolves uniquely in the stock manifest;
native one-biased import words identify the same type/instance as stock, not the
same captured dependency-array index. Relocation/import slots and sentinels are
checked. An additional authored upgrade-plus-weapon case verifies two distinct
imports; it is not a stock toggle-graph comparison.

The focused schema uses exact copied official AI/weapon declarations plus a
minimal reference-target stub set and an explicitly diagnostic asset wrapper.
The wrapper supplies Core's required asset ID; it is not an engine asset type.
Copied declaration shapes/defaults and target-location definitions are drift-
checked against staged official XSD before testing. This is not admission of
the complete official GameObject schema or a source-graph/inheritance build.

The batch also tests deterministic repeats, changed hints/masks/location/bools,
weapon settings, all thirteen optional allocation slots, non-null literal zero,
present-pointer corruptions, missing weapon lookup and invalid XML values.
Low-level enum refusals are checked without relying on schema validation alone.
Original stock files are never modified; no game is executed.

## Commands and next gates

```text
BinaryAssetBuilder.ManifestInspector.exe ep1-toggle-ai-self-test <static.manifest>
BinaryAssetBuilder.ManifestInspector.exe toggle-status-layout-review <static.manifest> [GameObject:Name]
```

The new batch gate is `ep1-toggle-ai` in `scripts/Test-Ra3Ep1Batch.ps1`.

Final Full evidence: `artifacts/ToggleAi-a/batch-report.json`, **28/28 groups
passed in 1087.326 seconds**, excluding the 1.33-second incremental batch build
(zero warnings/errors). All eight runtime-file hashes remained unchanged during
workers. The new AI gate took **10.017 seconds**; the existing broad compiler
suite accounted for **1018.463 seconds**. The initial dependency rebuild had
111 existing warnings and zero errors; later targeted/batch builds were clean.
Trailing whitespace in copied schema comments was cleaned afterward without
changing declarations; the schema-drift/native AI test was rerun separately.

Next: validate the existing special-ability base's actual marshaler payload and
enum semantics, implement the enclosing toggle/state and module/geometry child
lists, normalize their additional references, and compare complete selected
module graphs. Only then enable polymorphic dispatch. General GameObject
tokenization/mixed-stream publication and the other three absent module families
remain separate gates before Studio GameObject input can be opened.

Concrete next source fixtures are `Soviet/Units/SovietAntiStructureShip.xml`
(nonempty Module list, cost-advantage below 0.75 and health below 500) and
`EP1/Japan/Units/JapanFortressShip.xml` (ship_geom / air_geom lists). These are
located source candidates, not yet validated native graphs. The Dreadnought
source places Module before SkirmishAiInfo although the staged schema sequence
declares the reverse order; source preparation/order handling therefore needs
explicit review, not a silent assumption that every official snippet directly
passes the focused XSD.
