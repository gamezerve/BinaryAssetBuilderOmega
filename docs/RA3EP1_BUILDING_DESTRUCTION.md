# BuildingDestructionBehavior composition

The alpha complete-object route now connects the existing EP1 native writer for
`BuildingDestructionBehavior` to source admission, behavior metadata generation
and an independent source/native ownership proof.

Supported authored data:

- Local module ID and `AssociatedModule` (existing local-module resolution still
  applies), `MaxDebrisCloudsPerSecond`, and `FillerMesh`.
- Required `DieMuxData`: damage threshold, min/max killer angle, death types and
  forbidden death types, with actual schema defaults and native unit conversion.
- One or more ordered `Item` records, each with optional ASCII `BonePrefix`,
  optional strong `FX` reference, weak `CreationObject` identity and
  `FlingVelocity`.
- Absent versus explicit-empty bone strings, absent FX imports and repeated
  authored item rows. The former 256-row tokenizer experiment quota is removed;
  actual source/native buffer and allocation bounds remain.

The contract checks the entire 72-byte module header, 20-byte item records,
strings/terminators, padding and exact RELO/IMP ownership. FX targets are checked
against the active asset catalog and the real Core reference table. Weak object
and mesh fields are checked as their authored invariant-lowercase hashes; this
does not prove runtime target availability or model-bone existence.

Generic schema-derived metadata must equal a separate literal destruction writer
whose fields, inheritance, item tags and offsets were independently checked
against stock `AlliedConstructionYard`. Each metadata fragment is checked again
after insertion into the complete GameObject.

Still unsupported in this family's independent composition proof: populated
`VeterancyLevels` and authored `ExemptStatus`/`RequiredStatus` pointer storage,
including explicit-empty status attributes. These receive source errors rather
than being discarded or encoded speculatively. No death callback or Lua runtime
execution is certified by this work.

## Verification — 2026-10-23

- `ep1-allied-modules-self-test` passed existing stock-native/metadata controls;
  its item boundary checks now include 257 rows.
- `conscript-module-graph-self-test ... destruction`: three full native,
  metadata, linked stream and BIG cases passed: edited values with a real FX
  import, omitted optional item fields, and 257 ordered items with mixed
  present/absent imports and nonempty/empty strings.
- 63 native and 48 metadata corruptions were rejected; repeated builds matched.
- The unchanged original Conscript stream/BIG regression also passed.
- Evidence: `alpha/.work/building-destruction-20261023-c/module-graph-report.json`.
- Reborn preparation now reaches the next unconnected family:
  `SovietRefinery.xml(699,5): QueueDockBehavior`.

No Reborn source, published user package or game installation was changed by
these tests. Full refinery/building compilation is not yet complete.
