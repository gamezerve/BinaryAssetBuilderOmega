# QueueDockBehavior compilation

The alpha now has a real EP1 `QueueDockModuleData` model and native marshaler,
concrete dispatcher entry, source admission, metadata generation and complete
source-bound native ownership verification. This was previously an absent native
model, not just an admission-list omission.

| Field | Native offset | Contract |
| --- | --- | --- |
| Create/Behavior/Module base | 0 | 8-byte TypeId/local-ID prefix |
| DockStartBoneName | 8 | ASCII header; default empty string |
| DockActionBoneName | 16 | ASCII header; default empty string |
| DockEndBoneName | 24 | ASCII header; default empty string |
| QueueBonePrefix | 32 | ASCII header; default empty string |
| TurnTableBoneName | 40 | ASCII header; default empty string |
| DockingAnimation | 48 | Optional ModelConditionFlagType |

The native header is 52 bytes. Its type identity is `0xAF4C1F21`. All six own
field tags/offsets, original bone text, docking enum, Create/Behavior/Module
inheritance chain and complete 192-byte metadata prefix are independently
qualified from stock `GameObject:SovietRefinery` before composition is accepted.

Authored names may differ from the stock model. Empty and omitted names follow
the schema's empty-string defaults; each native allocation, length, terminator,
enum, padding byte and relocation source is checked. No strong imports exist.
Multiple independently named modules are supported. Source errors reject
non-ASCII or NUL-containing names instead of silently replacing them.

This does not verify that a bone exists in a particular model or certify actual
runtime docking behavior. The source must still refer to meaningful model bones.

## Verification — 2026-10-23

- New toolchain build succeeded. The full dependency rebuild reported two
  warnings; the final incremental build had zero warnings/errors.
- Three complete native/metadata/linked-stream/BIG cases passed: edited bone
  names and enum, omitted fields/default empty names, and two reordered modules
  with different IDs and nonempty/empty text.
- 87 native and 62 metadata semantic faults were rejected.
- Original Conscript stream/BIG regression passed unchanged.
- Evidence: `alpha/.work/queue-dock-20261023-b/module-graph-report.json`.
- Empty-string pointer fault tests use genuinely invalid null pointers. Moving a
  terminator into equivalent zero padding is not counted as a semantic fault.
- Reborn source preparation now reaches
  `SovietRefinery.xml(707,5): QueueProductionExitUpdate`, the next unconnected
  family. Full refinery compilation is not yet complete.

The developer tests do not change Reborn sources, rebuild its published package,
launch the game or modify installed game data.
