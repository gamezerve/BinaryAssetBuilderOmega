# QueueProductionExitUpdate compilation

The EP1 processor reads the actual schema field `ExitDelaySeconds` as `Time`.
The legacy KW model/writer used a different `ExitDelay` integer field; it is
retained separately rather than used to silently discard EP1 timing edits.

| Field | Native offset | Representation |
| --- | --- | --- |
| Update/Contain/Behavior/Module base | 0 | TypeId and local-ID, 8 bytes |
| PlacementViewAngle | 8 | Angle |
| ExitDelaySeconds | 12 | Time, default `0s` |
| InitialBurst | 16 | Unsigned integer, default 0 |
| UnitCreatePoint | 20 | Inline x/y/z floats, required element |
| NaturalRallyPoint | 32 | Inline x/y/z floats, required element |
| AllowAirborneCreation | 44 | Boolean, default false |
| NoExitPath | 45 | Boolean, default false |
| CanRallyToSlaughter | 46 | Boolean, default false |
| ClearAlliesFromDestination | 47 | Boolean, default true |

The 48-byte native header and complete 252-byte metadata prefix are independently
qualified against stock `GameObject:SovietRefinery`. The concrete engine type ID
is `0x9063FB6C`, not a hash of the isolated C# model's `Ep1`-prefixed name.
Vector3 metadata points directly to inline coordinates, not a nested token table.
The stock witness includes nonzero x coordinates (37.5 and 75); its delay is zero.
Nonzero delay tests therefore demonstrate authored Time encoding, not equality to
a stock nonzero-delay example or observed runtime behavior.

Both vector elements are schema-required; omitted components default to zero.
Coordinates and angle/delay/count/flags are not constrained to stock values.
Multiple modules with distinct local IDs are supported. This module has no
native heap allocations, relocation sources or imports. Every native byte and
metadata target is checked against source or the stock-qualified layout.

## Verification — 2026-10-23

- Toolchain build succeeded with two warnings and no errors.
- All three edited/default/multiple-instance complete-object stream/BIG cases
  passed; 207 native and 70 metadata faults were rejected.
- The unchanged original Conscript stream/BIG regression passed, including its
  existing stream/dependency/profile refusal checks.

See `alpha/.work/queue-exit-20261023-b/module-graph-report.json` for the complete
edited/default/multiple-instance batch. The separate unchanged Conscript
regression is under `alpha/.work/queue-exit-original-20261023-a`.
Tests use isolated stream/BIG output; they do not publish the user's Reborn mod,
change its XML, launch the game or modify installed game data.

Read-only Reborn preparation proceeds to
`SovietRefinery.xml(716,5): SpawnBehavior`. Full refinery publication is not yet
enabled. The legacy Spawn writer lacks the EP1 schema's
`ObjectStatusValidation` field; simply admitting that class would risk silently
dropping source data and still requires a native-layout implementation/review.
