# EP1 StructurePlacementBehavior

`StructurePlacementBehaviorModuleData` is a 56-byte record: the 8-byte module
header followed by an inline 48-byte `Shape` (`RestrictedAreaShape`). It matches
the stock `GameObject:SovietRefinery` record. Metadata is 252 bytes.

| Shape offset | Field | Storage |
| --- | --- | --- |
| 0 | Type | `GeometryType`, default SPHERE |
| 4 | ContactPointGeneration | `ContactPointGenerationType`, default NONE |
| 8 | Name | optional owned ASCII string (length + pointer) |
| 16, 20 | MajorRadius, MinorRadius | SageReal, default 1.0 |
| 24 | Other | optional owned ASCII string |
| 32, 36 | Height, FrontAngle | SageReal, Angle |
| 40 | Offset | optional pointer to an owned x/y/z vector |
| 44, 45 | Active, UsedForHealthBox | SageBool, default true |

`RestrictedAreaShape` is required. Schema-legal `1`/`0` booleans are
canonicalized to `true`/`false` before marshaling. The contract proves every
header byte, both optional strings, the optional Offset pointee, allocation
coverage, relocations (no imports) and the literal metadata writer.

## Verification — 2026-10-23

- Stock qualification reproduced the refinery BOX shape (30 × 45 × 5, Offset
  90/0/0) and its 252-byte metadata.
- Source controls rejected an invalid shape type, non-numeric radius/axis and a
  missing shape.
- Three complete-object stream/BIG cases passed (`structure-edited`,
  `structure-defaults`, `structure-multiple`); 252 native and 102 metadata
  corruptions were rejected.
- All earlier batches and the original Conscript regression still passed.
- Evidence: `alpha/.work` run 20261023-060738 and sibling runs.
- Validation-only Reborn preparation now reaches
  `SovietRefinery.xml(757,5): LUAEventList`.

Tests did not launch a game or rebuild/publish the user's Reborn package.
