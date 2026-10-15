# GameObject root/module names in one Core reference table

October 15, 2026. The selected root-import projection now has a separate actual
Core name-normalization and real-catalog gate. This follows the
[diagnostic slot milestone](RA3EP1_GAMEOBJECT_ROOT_STRONG_SLOTS.md), without
changing the alpha or making complete GameObjects publishable.

## Delivered

`gameobject-root-strong-source-self-test <static.manifest> <global.manifest>`:

1. Admits the existing reviewed shield/hook full-schema candidate for independent
   raw GameObject validation. This is not clean, unmodified full-schema admission.
2. Builds a focused schema using the actual official reference types and
   destruction declarations. Root reference type/default/use/fixed contracts
   are compared with the reviewed GameObject schema before normalization.
3. Reads bounded, validated linked EP1 static/global manifest metadata. It never
   reads or dumps the full stock BIN streams.
4. Normalizes root and repeated destruction FX names in **one** actual Core
   occurrence table, then requires unique type/instance identity matches in the
   independent catalogs. No authored numeric reference suffixes are accepted.
5. Transfers the normalized attributes/module into a partial GameObject native
   dispatch input without constructing a second reference table. EditorName
   deliberately moves the behavior/module allocation.
6. Checks real target identities, expected one-biased native reference words and
   globally shifted import slots before/after generated token projection. Repeated
   FX identities retain two distinct ordered occurrences, not accidental index reuse.

## Actual stock availability

The supplied static/global catalogs contain:

| Target type | Count | Selected target |
| --- | ---: | --- |
| CrowdResponse | 0 | No synthetic positive substituted |
| UnitTypeIcon | 17 | ArcherIcon, D76B50C7:C624373A |
| UnitIntro | 76 | Allied_AircraftCarrier_UnitIntro, 6F78B1A4:7C4A9B69 |
| FXList | 655 | FX_Allied_Sonar_Hit, 86682E78:6D946BE5 |

Absence here concerns **these two catalogs**, not all possible Uprising archives
or whether the runtime could ever support CrowdResponse. Its schema/reference
normalizer and missing-target refusal are exercised; its positive native slot
proof remains the earlier explicitly diagnostic-index test. Existing but entirely
ambiguous CrowdResponse metadata cannot be silently treated as absence.

## Tests and limits

- Eight cases: all four available icon/intro presence combinations, each with
  plain and explicitly typed names, plus two repeated FX occurrences.
- Raw source passes the reviewed full candidate before the focused Core stage;
  normalized suffixes are internal only. Required Body remains validation context,
  not part of the native partial projection.
- Repeated native BIN/RELO/IMP equality and independent projection round trip.
- Twelve failure controls: three missing-name, three wrong-type and three
  authored-index cases across the root fields, one duplicate catalog target,
  one removed real target and one valid-but-wrong native dependency index.

The initial test required a CrowdResponse stock target and failed because there
was none. It was narrowed to explicitly reported available stock families rather
than fabricating catalog records. The initial failed probe is not counted as a
successful gameplay/build gate.

Focused target stubs establish reference inheritance only. These tests do not
compile target payloads, implement UnitIntro's native target model, process a
general source graph, or produce a complete GameObject manifest/BIG. They test
actual Core normalization through the existing diagnostic seam, not the complete
public build/document/OutputManager workflow. They do not expand Studio alpha,
run the game or modify the installation. Root metadata stays at 87 fields.

## Files and next gate

- `tests/fixtures/RootStrongPipeline.xsd`
- `source/BinaryAssetBuilder.ManifestInspector/GameObjectRootStrongSourceSmokeTest.cs`
- `source/BinaryAssetBuilder.ManifestInspector/Program.cs`
- `scripts/Test-Ra3Ep1Batch.ps1`: new independent Focused group, bringing it to 31.

Next integration work remains other required root/child graphs and recursive
module tokenization, followed by a complete processor/mixed stream and new
GameObject gameplay acceptance. This closes a selected named-reference gap,
not the full GameObject production gate.

## Final regression

`artifacts/GameObjectRootNamedSource-a/batch-report.json`: **31/31 Focused groups
pass in 84.376 seconds**. The new independent named-root group takes 3.417 seconds;
the incremental batch build passes in 4.66 seconds with zero warnings/errors.
All eight runtime hashes remain unchanged while workers run. GameExecuted and
InstalledFilesModified are false. The broader Full suite and gameplay were not
rerun; no alpha release or increased overall completion estimate is claimed.
