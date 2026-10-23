# StructureSellUpdate composition support

The alpha GameObject path now accepts `StructureSellUpdate` as a complete
scalar-only behavior family, even when it is absent from the original Conscript
baseline. This connects the existing native marshaler to behavior source
admission, fixed-value codecs, metadata generation and whole-object verification.

| Source field | EP1 native offset | Contract |
| --- | --- | --- |
| Type identity | 0 | StructureSellUpdateModuleData |
| Local id | 4 | Source-bound local identity |
| SellTime | 8 | Time; schema default 0s |
| ActiveModelCondition | 12 | ModelConditionFlagType; optional |

The complete module is 16 bytes, with no own dynamic allocation, strong import or
relocation. Its independent native and metadata witness is captured from stock
`GameObject:AlliedConstructionYard`, not compiled from a frozen expected XML.
Authored scalar values are checked separately before stock comparison. The
source shape template does not inject a module into user XML or freeze its
values to 2s/SOLD. Multiple instances, changed local IDs and source ordering are
supported within existing whole-object ownership constraints.

Verification on 2026-10-23:

- Existing basic-module suite passed four stock payload comparisons, dispatch,
  edited scalar/ID controls and unknown-field checks.
- `conscript-module-graph-self-test ... sell` passed three complete native,
  metadata, linked stream and BIG cases: edited values, omitted optional fields,
  and two reordered instances. Repeated compilation was deterministic.
- 31 native and 34 metadata corruptions were rejected.
- Evidence: `alpha/.work/structure-sell-20261023-b/module-graph-report.json`.
- Reborn preparation passed the previous StructureSellUpdate refusal and reached
  `BuildingDestructionBehavior` at SovietRefinery.xml(657,5), which is not yet
  connected to this complete-object route.

This is not full refinery/building support or a runtime gameplay certification.
The preparation check does not change Reborn sources, replace its published
package, launch the game or modify the installation.
