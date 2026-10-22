# Common GameObject authoring across build routes

## Cause and correction

The previous fixed route admitted the historical six scalar edits, but the
variable route did not mask every supported nested value and its independent
native comparison still required stock values in non-module owners. Reborn's
valid `BuildCost Amount="111"` consequently failed against stock `100`.
The same gap affected VisionRange, ShroudClearingRange and Locomotor Speed;
MaxHealth already had a module-field contract but belongs to the same regression
coverage. Optional root strong-reference presence also retained stock-count
assumptions in the variable comparison's reference table.

`ConscriptSharedSourceContract` now supplies the existing common value validation
and structural masking to both admission routes. Mixed GameObject builds reuse
the same variable admission/verifier, not a third value whitelist.

| Existing capability | Shared admission | Native/output verification |
|---|---|---|
| Root primitives, enums, KindOf and typed weak IDs | Existing schema/native contracts | Actual typed values at ABI offsets |
| Inline root strings | Existing source text/presence contracts | Exact authored content and allocations |
| Cost, health, vision/shroud and speed | Shared nested value/default handling | Exact source bits at independently anchored owners |
| Optional root strong references | Shared source name/presence rules | Actual target IDs, selectors and IMP ownership |
| Module fixed fields and local IDs | Existing shared module contracts | Existing per-module checks |

Variable-only structural changes (Behaviors, pointed-name presence, ArmorSet and
hardpoint lists) still select the implemented variable route. The fixed serializer
is not relabeled as supporting arbitrary structures; supported source simply
reaches the correct path without losing its value capabilities.

## Proof remains source-bound

The variable native walker receives explicit expected primitive values only for
the independently located supported scalar fields. It compares output bits to
source, not merely to another run of the producer. All other fields, padding,
allocations, metadata tags and auxiliary ownership retain their existing checks.
The comparison never rewrites actual output to stock values.

Root reference expectations are bound to authored identity/presence. Other stock
owner comparisons retain a baseline-prefix reference table, independently of
actual Core occurrence indices. Removing or restoring root imports therefore
does not make unrelated module references fail a stock-count assumption.
Missing/wrong-type targets remain subject to the normal source/catalog checks.

Schema defaults and native primitive zero initialization are used for optional
covered attributes; required MoneyTransaction Amount remains required. Historical
six-field Min/Max values remain regression vectors, not authoring limits.
This does not add new root owner layouts, extra LocomotorSet/BuildCost structures,
module families or resource processors that were never implemented.

## Targeted checks

- Two combined native/stream/package cases exercise every root primitive and fixed
  attribute category, module scalar probes, dynamic text and the nested legacy
  values, then force changed/reordered behavior topology. The same value inputs
  are first admitted by the fixed route. A defaults variant omits covered optional
  attributes and restores one root import while leaving the other absent.
- Each combined case rejects native corruption in all six scalar slots, all root
  import selectors, module/container fields and auxiliary tables, plus metadata
  faults. Each generated stream is read back and packaged.
- One actual alpha build combines seven explicit local assets, variable Behaviors,
  cost/health/vision/speed edits and both absent root imports. Regression restores
  the developer source afterward and never builds the user's Reborn output.
- Read-only Reborn source preparation now passes its `Amount="111"` input; no
  user's XML or game installation is edited by that check.

October 22 results: both combined stream/package cases passed (28 native and 20
metadata corruptions rejected in total), the seven-asset Studio build and restored
developer/recruitment checks passed, and all 25 XML coordinate diagnostics passed.
The compiler built with zero warnings/errors and the stable alpha was synchronized.
