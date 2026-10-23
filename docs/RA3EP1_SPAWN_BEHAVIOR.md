# EP1 SpawnBehavior

The EP1 native model is separate from the legacy KW `SpawnBehaviorModuleData`.
It retains engine type ID `0x0D916ADF`, a 96-byte header and the real
Upgrade/Behavior/Module inheritance chain.

| Native offset | Field/storage |
| --- | --- |
| 0..35 | UpgradeModuleData, including optional animation and weak upgrade lists |
| 36 | SpawnNumberData (signed integer) |
| 40 | SpawnReplaceDelayData (Time) |
| 44 | InitialBurst (signed integer) |
| 48 | DamageTypesToPropagateToSlaves (8-byte mask) |
| 56 | FadeInTime (Time) |
| 60 | InitialDelay (Time) |
| 64 | Optional DieMuxDataType pointer |
| 68 | Source-sized weak GameObject SpawnTemplate list |
| 76 | Optional ObjectStatusValidationDataType pointer |
| 80..95 | Sixteen packed boolean attributes in native declaration order |

The final three flags are `KillSpawnsWhenObjectInvalid`, `SpawnOnUnpack` and
`SetPurchasePrice`. They and the status-validation pointer were absent in the
legacy writer. The pointer shifts every own boolean relative to that writer;
adding admission alone would not produce the correct EP1 layout.

Stock qualification compares the complete 564-byte metadata prefix against
`GameObject:SovietRefinery`, including own fields, inherited fields, signatures,
list headers and native targets. Its native witness includes one SovietMiner
weak reference, a 9999-second delay and the unpack/purchase flags. It has no
populated Die/status-validation record: those paths additionally use existing
shared native/schema record models and independent authored-value/allocation
checks, not a claim of stock equivalence for that refinery's absent records.

The source/native contract covers every fixed attribute, optional animation,
upgrade lists, weak spawn rows, optional status masks and Die fields (including
veterancy strings and optional status pointers). Allocations must be disjoint;
relocations must match the actual owned pointers. This module emits no strong
imports. Authored list lengths and scalar values are not pinned to stock values.
ASCII veterancy labels are required by the existing native string encoding;
nonrepresentable labels receive a source-located error.

Implementation: `Ep1SpawnBehavior.cs`, `Marshaler.Ep1SpawnBehavior.cs` and
`SpawnBehaviorContract.cs`. Integrated tests use the `spawn` module graph batch,
with populated fields, omitted/default fields and two independently named
instances. Isolated tests never publish Reborn or change its source files.
Native and stream/package validation does not establish gameplay correctness.

## Verification — 2026-10-23

- The fresh toolchain build succeeded with two warnings and no errors.
- Three complete-object stream/BIG cases passed: populated records and values,
  omitted/default fields, and two independently named populated modules.
- 177 native and 74 metadata corruptions were rejected.
- The unchanged original Conscript stream/BIG regression also passed, including
  its existing 584 stream/dependency/profile refusal controls.
- Batch evidence: `alpha/.work/spawn-20261023-a/module-graph-report.json`.
- Original regression: `alpha/.work/spawn-original-20261023-a`.
- Read-only Reborn preparation now reaches
  `SovietRefinery.xml(727,5): StructureUnpackUpdate`. This next family also has
  a legacy/schema mismatch: its EP1 base is `GenericUnpackUpdateModuleData`,
  whereas the existing legacy structure uses `UpdateModuleData` directly.
  It is not enabled merely by removing an admission check.

The user's source and published package were not rebuilt or changed by these
tests. No game process was launched.
