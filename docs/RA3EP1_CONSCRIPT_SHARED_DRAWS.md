# Complete selected Conscript Draws container

## Outcome and boundary

All four complete Conscript drawing modules now compile together through actual
Draw dispatch, **one Tracker and one Core reference occurrence table**. This is
not concatenation of separately normalized modules or separate mod assets.

| Shared stock-order graph | Result |
| --- | --- |
| Modules | Main scripted, parachute scripted, normal tracer, veterancy tracer |
| Scripted children retained | 10 model states / 84 animation states |
| Native BIN / RELO / IMP | 27,880 / 808 / 696 bytes |
| Strong reference slots | 173; each Core occurrence consumed exactly once |
| Complete independent metadata prefix | 58,608 bytes; every word matches |

The enclosing list count/item markers and all four complete recursive metadata
prefixes match the independently captured stock Draws section after explicit
stock-to-compiled native-coordinate mapping. Each leaf's complete typed native
graph also matches stock semantic reference identities. All current references
must resolve uniquely in the validated static/global catalogs.

The shared comparison retains the prior one-field parachute Script CRLF/LF
boundary. It does not rewrite source/producer script bytes or execute LUA.
Existing component tests still require complete per-component occurrence
consumption by default; only this shared caller defers that check until all
siblings' import slots are checked together.

## Source scenarios and refusals

Twelve cases cover an allocated-empty list, each individual module, both scripted
modules, both tracers, the full stock order, reversed and interleaved full order,
full authored module-ID/scalar edits, and removal of both optional tracer status
records. Every ordinary case repeats compilation/tokenization and compares all
three buffers. The stock-derived full metadata prefix remains the oracle for
the full authored-ID/scalar edit; no source graph is reduced for that edit.

The full edit changes all four weak module IDs, both scripted MaxRecoilDistance
values and both tracer Width values to 17.5, with literal native values/hash
checks. These are module-local IDs, **not a newly published GameObject ID**.
Removing both tracer status records must remove exactly 128 native bytes and
72 metadata bytes, leaving each tracer at 112 native bytes.

**14,682** negative controls pass: 14 malformed native/container cases, every
one of 14,652 metadata words, six detached native and two detached auxiliary
mutations, three shared occurrence failures and five source failures. Coverage
includes unknown classes, duplicate/case-colliding local module IDs, missing
texture targets, unknown XML children, count/offset bounds, unaligned/aliased
items, cross-leaf pointers, missing parent relocation ownership, duplicate slots,
orphan storage and missing/repeated/out-of-range shared import occurrences.

## Implementation and current limits

- `Ep1DrawsTokenizer.Tokenize`: owns no stock bytes or catalog; validates canonical
  Draws header/ordered leaf extents and exact parent/leaf auxiliary ownership.
  Supported concrete writers are scripted and tracer only. Other classes fail.
- Every leaf is validated/tokenized with its shared import values unchanged.
  Metadata/native coordinates are rebased separately; the complete shared native
  buffer is copied once and all pointer values/auxiliary slots shifted once.
  Input slot order is preserved. Inverse recovery must match every native byte.
- Module-local native records/text cannot cross sibling ownership boundaries;
  shared stock semantic assets use the common Core occurrence table, not shared
  native heap pointers. Duplicate weak module IDs fail across both classes.
- Bounds: native list and each auxiliary stream at most 4 MiB, metadata 8 MiB,
  list count at most 64. These are safety limits, not evidence that every possible
  64-module source graph or every derived Draw class is supported.
- A diagnostic `DrawsProbe` asset envelope uses the unchanged official
  `DrawModules` schema type under the reviewed in-memory schema set. Source
  preparation is the earlier explicit scripted particle-order/render-name seam.
  The two tracers are read directly from the actual official Conscript XML.
- The parachute leaf is still read explicitly from the known include source.
  **No general XInclude/XPointer engine, inheritance or source closure is proved.**
- Previously unsupported populated scripted Material/ShadowInfo/FXEvent/LuaEvent/
  Turret/SubObject/AttachModel/EmbedPortal families remain fail-closed.

Primary files:

- `source/BinaryAssetBuilder.XmlCompiler/BinaryAssetBuilder/XmlCompiler/Ep1DrawsTokenizer.cs`
- `source/BinaryAssetBuilder.ManifestInspector/Ep1DrawsCompositionSmokeTest.cs`
- `source/BinaryAssetBuilder.ManifestInspector/Ep1ScriptedDrawNativeSmokeTest.cs`
- `source/BinaryAssetBuilder.ManifestInspector/SelectedNativeGraphComparer.cs`

CLI: `ep1-draws-composition-self-test <official-xml-root> <static.manifest> <global.manifest>`.
The Focused batch adds this group only with explicit `-UprisingXmlRoot`.

## Next gate

Join this complete Draws container with the selected root/armor/movement/Body/
behavior/AI owners on a single whole-object graph and occurrence table, then
complete the remaining root/client/behavior owners before mixed-stream packaging
and first new-ID Conscript gameplay. The complete Conscript remains the target;
no reduced replacement is used. No general GameObject/alpha admission, package,
new model compilation, game launch or install mutation is claimed here.

## Regression evidence

Final Focused batch with explicit official XML root: **61/61** groups pass in
**225.340 seconds**; shared Draws group **15.610 seconds**. The batch's initial
incremental build passes in 1.27 seconds with zero warnings/errors. The earlier
post-commit dependency rebuild reported the 111 existing warnings and zero
errors; the final changed Inspector build reported zero warnings/errors.
All eight runtime hashes remain unchanged throughout the batch.

Local evidence: `artifacts/ConscriptSharedDraws-a/batch-report.json`,
`compiler-build.log` and `ep1-draws-composition.log`. The report records
`GameExecuted=false`, `InstalledFilesModified=false` and
`AlphaCapabilitiesExpanded=false`. No Full-profile run is claimed.
