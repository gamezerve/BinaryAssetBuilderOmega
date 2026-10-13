# EP1 direct toggle-module native checkpoint

New files: `Ep1ToggleStatus.cs`, `Marshaler.Ep1ToggleStatus.cs` and
`Ep1ToggleModuleSmokeTest.cs`. The module now has a 264-byte native model and
direct marshaler, with ordered 304-byte states and the existing 252-byte
special-ability base. **GameObject polymorphic dispatch remains unchanged.**
Alpha h and general GameObject publication remain closed.

## Evidence

Three complete selected native graphs reproduce Conscript, Ore Collector and
Terror Drone payloads: base/root, both states, AI, pointed-to optional values,
present weapon records and generic-event strings. Literal source names/type IDs
must match each independently anchored stock dependency slot. Repeated dependency
identities can legitimately have different indices; a unique-name/index search
is not a valid occurrence join.

This checkpoint substitutes **captured stock reference indices** for direct native
diagnostic input. It does not claim complete source/Core normalization, dependency
resolution, schema admission, GameObject tokenization or playable publication.
The previous standalone AI milestone retains its separate actual Core-normalized
weapon reference evidence; that does not automatically extend to this whole module.

Actual native diagnostic BIN/RELO/IMP dimensions, excluding stream headers:

| Selected object | BIN | RELO | IMP |
| --- | ---: | ---: | ---: |
| Conscript | 936 | 24 | 20 |
| Ore Collector | 892 | 28 | 16 |
| Terror Drone | 912 | 24 | 24 |

The byte comparison clears/rebases only independently checked pointer words;
captured import words remain equal to the stock occurrence indices. Slot inventories
and tracker sentinels are checked. Repeats, module-ID edits and detached root-list
pointer corruption controls pass.

The empty GenericLuaEventParameter default has a **non-null terminator-only
allocation and relocation**. It is not equivalent to a null native string. This
also explains the allocation spacing adjacent to the 28-byte weapon records.
State sound references may identify actual AudioEvent or Multisound assets, not
the legacy empty AudioEventInfo marker's hash.

EP1 locomotor names include ATTACKING, MOVING_TO_TRANSPORT,
EXITING_PRODUCTION_STRUCTURE and SPECIALABILITY_ACTIVE instead of blindly using
old KW names. The new state mapper preserves INVALID=-1 and uses strict named
scalar input. Existing generic KW locomotor marshalling is not changed.

Authored controls compile Module records, multiple geometry strings and strings
containing spaces, changed root booleans/enum values and module IDs. Missing
states/AI, excess Module children and a legacy COMBO2 locomotor label are refused.
Nonempty Module/geometry lists still need independent **stock** comparison, source
schema/reference validation and local module-ID resolution. Legacy Module trigger
parsing is reused here; schema/strict-trigger input admission is a remaining gate.
The base's selected defaults/literals match; custom base fields and non-null
CustomAnimAndDuration are not fully covered by these three fixtures.

Only three native model families remain absent in the original two-object name
inventory (BuildingDestruction, TechUpgrade, LUAEventListBehavior), but the new
toggle model is still diagnostic-only and not a dispatch/publication capability.

```text
BinaryAssetBuilder.ManifestInspector.exe ep1-toggle-module-self-test <static.manifest>
```

The batch gate is `ep1-toggle-module`. Next: real named reference/schema/source
integration and nonempty child-list stock proof, then polymorphic dispatch and
recursive GameObject tokenization/mixed-stream publication.

Final evidence: `artifacts/ToggleModule-a/batch-report.json`, **27/27 focused
groups passed in 66.833 seconds**, excluding a 1.22-second zero-warning/error
incremental build. Eight runtime-file hashes stayed unchanged during workers.
The new module gate took 9.735 seconds, and the previous actual-Core AI gate
took 9.925 seconds. The initial dependency rebuild had 111 existing warnings and
zero errors; subsequent shared builds reported two existing audio warnings,
and final incremental builds were clean. This checkpoint does **not** rerun or
claim a new Full-suite result; the preceding standalone AI milestone's 28/28
Full result applies to its earlier revision.
