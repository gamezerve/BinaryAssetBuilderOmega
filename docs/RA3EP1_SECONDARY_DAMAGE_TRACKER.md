# Source-sized SecondaryDamageTracker

The existing native writer now participates in flexible complete-object source
compilation. Tracker children are no longer required to match the Conscript's
color, five-effect list or absent status-validation record.

## Supported source

- UpdateDuration, DissipateAmount and MaxDamageClampScaler, with official defaults
  and existing engine-unit/numeric validation.
- Optional Color with all three required finite float components. HDR values
  above 1 are retained, not clamped to an invented display range.
- Zero or more ordered EffectInfo records. Fraction is required; optional
  AttributeModifier references, both named option bits and all three boolean
  fields are source-bound. Empty lists and source counts above five are valid.
- Optional ObjectStatusValidation with forbidden/required status masks.
- Multiple independently named tracker modules in the same object.

Modifier names may retarget unique compatible stock catalog assets or be omitted;
there is no implicit modifier generation. This does not implement compilation of
new AttributeModifier assets. Existing shared flag-codec restrictions remain
explicit errors rather than silently ignored operations.

## Native and metadata proof

| Native offset | Field |
| --- | --- |
| 0..7 | TypeId and local-ID hash |
| 8 | UpdateDuration |
| 12 | DissipateAmount |
| 16 | MaxDamageClampScaler |
| 20 | Optional 12-byte RGBColor pointer |
| 24/28 | EffectInfo count/items |
| 32 | Optional 64-byte status-validation pointer |

Effects are 16-byte records: Fraction at 0, modifier import at 4, option mask at
8, booleans at 12/13/14 and zero padding at 15. All arrays/optional records,
relocations and semantic import identities are independently checked against
authored source. The generic tokenizer is cross-checked by a separate literal
metadata writer for variable counts and optional Color/status tables.

Both original infantry and AlliedConstructionYard building graphs are independently
compiled and compared with stock native data, dependencies and every metadata
tag/target before flexible compilation is accepted. No native layout or legacy
marshaler replacement was necessary.

## Verification — 2026-10-23

- Fresh compiler build succeeded with zero warnings/errors.
- Five source controls rejected missing/invalid Fraction, invalid option/status
  and incomplete RGB input before native compilation.
- Three complete stream/BIG cases passed: building status-filter shape, empty
  /default fields, and multiple trackers with a nine-entry effect list, optional
  modifiers, both options, HDR color and status masks.
- 199 native and 90 metadata corruptions were rejected.
- Evidence: `alpha/.work/secondary-tracker-20261023-a/module-graph-report.json`.
- The unchanged original Conscript stream/BIG regression passed, including its
  584 stream/dependency/profile refusal checks; evidence is under
  `alpha/.work/secondary-tracker-original-20261023-a`.
- Read-only Reborn preparation now reaches
  `Soviet/structures/SovietRefinery.xml(677,5): CreateObjectDie`. Its ALL-death /
  forbidden-SUICIDED conditions and second OVER_WATER variant still require
  flexible death-condition verification on that family.

Main implementation: `SecondaryDamageTrackerContract.cs`, source admission and
the complete graph verifier. Tests did not modify Reborn XML, publish its mod,
launch a game or change installed data. Offline proof is not gameplay validation.
